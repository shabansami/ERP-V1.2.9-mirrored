using ERP.DAL.Utilites;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Web.Identity;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SupplierCategoriesController : Controller
    {
        // GET: SupplierCategories
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public SupplierCategoriesController()
        {
            db = new VTSaleEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.PersonCategories.FirstOrDefault(x => x.Id == id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.LastRow = db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new PersonCategory());
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(PersonCategory vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.PersonCategories.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    //update account tree
                    var accountTree = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeId);
                    accountTree.AccountName = vm.Name;
                }
                else
                {
                    if (db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //========================اضافة حساب الفئة فى الدليل المحاسبى
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeSupplierAccount).SValue;
                    if (val == null)
                        return Json(new { isValid = false, isInsert, message = "تأكد من تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                    var accountTree = db.AccountsTrees.Find(Guid.Parse(val));
                    var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                    if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                        newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                    else
                        newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                    // add new account tree 
                    var newAccountTree = new AccountsTree
                    {
                        AccountLevel = accountTree.AccountLevel + 1,
                        AccountName = vm.Name,
                        AccountNumber = newAccountNum,
                        ParentId = accountTree.Id,
                        TypeId = (int)AccountTreeSelectorTypesCl.Sub,
                        SelectedTree = false
                    };
                    vm.AccountTree = newAccountTree;
                    vm.IsCustomer = false;

                    db.PersonCategories.Add(vm);
                }
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    if (isInsert)
                        return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
                    else
                        return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });

                }
                else
                    return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.PersonCategories.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    var accountTree = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeId);
                    if (accountTree.AccountsTreesChildren.Where(x=>!x.IsDeleted).Any())
                        return Json(new { isValid = false, message = "لا يمكن حذف الفئة لارتباط موردين بها فى الدليل المحاسبى" });
                    accountTree.IsDeleted = true;

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;


                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        //Releases unmanaged resources and optionally releases managed resources.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}