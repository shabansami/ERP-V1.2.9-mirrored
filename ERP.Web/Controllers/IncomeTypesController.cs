using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class IncomeTypesController : Controller
    {
        // GET: IncomeTypes
        VTSaleEntities db;
        VTSAuth auth;
        public IncomeTypesController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
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
                data = db.IncomeTypes.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn=x.CreatedOn, Name = x.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.IncomeTypes.FirstOrDefault(x=>x.Id==id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.LastRow = db.IncomeTypes.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new IncomeType());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(IncomeType vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.IncomeTypes.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.IncomeTypes.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    db.Entry(model).State = EntityState.Modified;
                    //update account tree
                    var accountTree = db.AccountsTrees.FirstOrDefault(x=>x.Id==model.AccountsTreeId);
                    accountTree.AccountName = vm.Name;
                    db.Entry(accountTree).State = EntityState.Modified;
                }
                else
                {
                    if (db.IncomeTypes.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //========================
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeGeneralRevenus).SValue;
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
                        TypeId = (int)AccountTreeSelectorTypesCl.Revenuse,
                        SelectedTree = false
                    };
                    vm.AccountsTree = newAccountTree;
                    db.IncomeTypes.Add(vm);
                    //==============================

                    //isSaved = InsertGeneralSettings<IncomeType>.AddAccountTree(GeneralSettingCl.AccountTreeGeneralRevenus, vm.Name, AccountTreeSelectorTypesCl.Revenuse, vm, auth.CookieValues.UserId);
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
                var model = db.IncomeTypes.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var accountTree = db.AccountsTrees.FirstOrDefault(x=>x.Id==model.AccountsTreeId);
                    accountTree.IsDeleted = true;
                    db.Entry(accountTree).State = EntityState.Modified;

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