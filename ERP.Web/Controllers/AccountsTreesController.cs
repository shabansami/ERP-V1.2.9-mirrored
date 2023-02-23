using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class AccountsTreesController : Controller
    {
        // GET: AccountsTrees
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region ادارة شجرة الحسابات
        public ActionResult Index()
        {
            ViewBag.TypeId = new SelectList(db.SelectorTypes.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.AccountsTrees.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id,ParentId = x.ParentId, ParentName = x.AccountsTreeParent.AccountName, AccountNumber = x.AccountNumber, AccountName = x.AccountName, TypeName = x.SelectorType.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion

        #region اضافة وتعديل وحذف حساب من الشجرة 
        [HttpGet]
        public ActionResult CreateEdit()
        {

            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.AccountsTrees.FirstOrDefault(x=>x.Id==id);
                    ViewBag.TypeId = new SelectList(db.SelectorTypes.Where(x => !x.IsDeleted), "Id", "Name", model.TypeId);
                    ViewBag.OrientationTypes_Id = new SelectList(db.OrientationTypes.Where(x => !x.IsDeleted), "Id", "Name",model.OrientationTypes.Id);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.TypeId = new SelectList(db.SelectorTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.OrientationTypes_Id = new SelectList(db.OrientationTypes.Where(x => !x.IsDeleted), "Id", "Name");

                ViewBag.LastRow = db.AccountsTrees.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new AccountsTree());
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(AccountsTree vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.AccountName)|| vm.AccountNumber==0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.AccountsTrees.Where(x => !x.IsDeleted && x.Id != vm.Id && x.AccountNumber == vm.AccountNumber  ).Count() > 0)
                        return Json(new { isValid = false, message = "حساب موجود مسبقا" });

                    var model = db.AccountsTrees.FirstOrDefault(x=>x.Id==vm.Id);
                    model.AccountName = vm.AccountName;
                    model.AccountNameEn = vm.AccountNameEn;
                    model.TypeId = vm.TypeId;
                    model.ParentId = vm.ParentId;
                    model.AccountNumber = vm.AccountNumber;
                    if (vm.ParentId == null)
                        model.AccountLevel = 1;
                    else
                    {
                        var levelParent = db.AccountsTrees.Where(x => x.Id == vm.ParentId).FirstOrDefault().AccountLevel;
                        model.AccountLevel = levelParent + 1;
                    }
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == vm.AccountNumber).Count() > 0)
                        return Json(new { isValid = false, message = "حساب موجود مسبقا" });

                    isInsert = true;
                    var newAccount = new AccountsTree
                    {
                        AccountName = vm.AccountName,
                        AccountNameEn = vm.AccountNameEn,
                        AccountNumber = vm.AccountNumber,
                        TypeId = vm.TypeId,
                        ParentId = vm.ParentId,
                        SelectedTree=true
                    };
                    if (vm.ParentId == null)
                        newAccount.AccountLevel = 1;
                    else
                    {
                        var levelParent = db.AccountsTrees.Where(x => x.Id == vm.ParentId).FirstOrDefault().AccountLevel;
                        newAccount.AccountLevel = levelParent + 1;
                    }
                    db.AccountsTrees.Add(newAccount);
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
                var model = db.AccountsTrees.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
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

        #endregion

        #region استعراض كل حسابات الشجرة بمعاملاتها المالية 

        public ActionResult ShowAccounts()
        {
            return View();
        }
        #endregion
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