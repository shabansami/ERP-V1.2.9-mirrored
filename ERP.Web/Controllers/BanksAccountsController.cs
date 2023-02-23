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
    public class BanksAccountsController : Controller
    {
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public BanksAccountsController()
        {
            db = new VTSaleEntities();
        }        // GET: BanksAccounts
        public ActionResult Index()
        {
            ViewBag.BankId = new SelectList(db.Banks.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn), "Id", "Name");
            return View();
        }


        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.BankAccounts.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn=x.CreatedOn, BankName = x.Bank.Name, AccountName = x.AccountName,AccountNo=x.AccountNo,IBAN=x.IBAN,SwiftCode=x.SwiftCode, Actions = n, Num = n }).ToList()
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
                    var model = db.BankAccounts.FirstOrDefault(x=>x.Id==id);
                    ViewBag.BankId = new SelectList(db.Banks.Where(x => !x.IsDeleted), "Id", "Name", model.BankId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.BankId = new SelectList(db.Banks.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn), "Id", "Name");
                ViewBag.LastRow = db.BankAccounts.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new BankAccount());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(BankAccount vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.AccountName) || string.IsNullOrEmpty(vm.AccountNo) || vm.BankId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.BankAccounts.Where(x => !x.IsDeleted && x.AccountName == vm.AccountName && x.Id != vm.Id).Count() > 0) ///???
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" }); 
                    var model = db.BankAccounts.FirstOrDefault(x=>x.Id==vm.Id);
                    model.AccountName = vm.AccountName;
                    model.BankId = vm.BankId;
                    model.AccountNo = vm.AccountNo;
                    model.IBAN = vm.IBAN;
                    model.SwiftCode = vm.SwiftCode;
                    db.Entry(model).State = EntityState.Modified;
                    //update account tree
                    var accountTree = db.AccountsTrees.FirstOrDefault(x=>x.Id==model.AccountsTreeId);
                    accountTree.AccountName = vm.AccountName;
                    db.Entry(accountTree).State = EntityState.Modified;

                }
                else
                {
                    if (db.BankAccounts.Where(x => !x.IsDeleted && x.AccountName == vm.AccountName).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //========================
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeBankAccount).SValue;
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
                        AccountName = vm.AccountName,
                        AccountNumber = newAccountNum,
                        ParentId = accountTree.Id,
                        TypeId = (int)AccountTreeSelectorTypesCl.Operational,
                        SelectedTree = false
                    };
                    vm.AccountsTree = newAccountTree;
                    db.BankAccounts.Add(vm);
                    //==============================

                    //isSaved = InsertGeneralSettings<BankAccount>.AddAccountTree(GeneralSettingCl.AccountTreeBankAccount, vm.AccountName, AccountTreeSelectorTypesCl.Bank, vm, auth.CookieValues.UserId);

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
                var model = db.BankAccounts.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
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