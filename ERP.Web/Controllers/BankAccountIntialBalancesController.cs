using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
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

    public class BankAccountIntialBalancesController : Controller
    {
        // GET: BankAccountIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public BankAccountIntialBalancesController()
        {
            db = new VTSaleEntities();
            storeService= new StoreService();
        }
        public ActionResult Index()
        {
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            // اسماء حسابات البنوك من جدول بيرسون
            var bankAccounts = db.BankAccounts.Where(x => !x.IsDeleted );
            //حسابات البنوك من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل حساب بنكى عمليتين يتم تسجيلهم فى جدول القيود اليومية
            var generalGrouping = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceBank && bankAccounts.Any(p => p.AccountsTreeId == x.TransactionId && !p.IsDeleted)).OrderBy(x=>x.CreatedOn).Select(x => new { TransactionId = x.TransactionId, BankAccountName = bankAccounts.Where(p => p.AccountsTreeId == x.TransactionId).FirstOrDefault().AccountName + " / " + bankAccounts.Where(p => p.AccountsTreeId == x.TransactionId).FirstOrDefault().Bank.Name, Amount = x.Credit + x.Debit, CreatedOn = x.TransactionDate.ToString(), Actions = n, Num = n }).GroupBy(x => new { x.TransactionId, x.BankAccountName, x.Num, x.Actions, x.Amount, x.CreatedOn }).ToList();
            var data = generalGrouping.Select(x => new
            {
                TransactionId = x.FirstOrDefault().TransactionId,
                BankAccountName = x.FirstOrDefault().BankAccountName,
                Amount = x.FirstOrDefault().Amount,
                CreatedOn = x.FirstOrDefault().CreatedOn,
                Actions = n,
                Num = n
            }).ToList();
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //if (TempData["model"] != null) //edit
            //{
            //    int id;
            //    if (int.TryParse(TempData["model"].ToString(), out id))
            //    {
            //        var model = db.GeneralDailies.FirstOrDefault(x=>x.Id==id);
            //        ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", model.TransactionId);
            //        return View(model);
            //    }
            //    else
            //        return RedirectToAction("Index");
            //}
            //else
            //{                   // add
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            return View(new IntialBalanceVM() { DateIntial = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null|| vm.DateIntial == null || vm.BankAccountId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                //if (vm.Id != Guid.Empty)
                //{
                //    if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId != vm.CustomerId).Count() > 0)
                //        return Json(new { isValid = false, message = "تم تسجيل رصيد للمورد من قبل" });

                //    var oldGeneralDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId == vm.CustomerId).ToList();
                //    foreach (var item in oldGeneralDailies)
                //    {
                //        item.IsDeleted = true;
                //        db.Entry(item).State = EntityState.Modified;
                //    }

                //    //add new general dailies
                //    // الحصول على حسابات من الاعدادات
                //    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                //    AddGeneralDailies(vm, generalSetting);

                //}
                //else
                //{
                var bankAccount = db.BankAccounts.Where(x => !x.IsDeleted && x.Id == vm.BankAccountId).FirstOrDefault();
                if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceBank && x.TransactionId == bankAccount.AccountsTreeId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للحساب البنكى من قبل" });
                isInsert = true;
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب تشغيلى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب تشغيلى" });

                    AddGeneralDailies(vm, generalSetting, bankAccount);
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                //}
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

        void AddGeneralDailies(IntialBalanceVM vm, List<GeneralSetting> generalSetting, BankAccount bankAccount)
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;

            // حساب البنك
            //var bankAccount = db.BankAccounts.Where(x => !x.IsDeleted && x.Id == vm.BankAccountId).FirstOrDefault();
            db.GeneralDailies.Add(new GeneralDaily
            {
                AccountsTreeId = bankAccount.AccountsTreeId,
                Debit = vm.Amount ?? 0,
                Notes = $"رصيد أول المدة للحساب البنكى : {bankAccount.AccountName + " / " + bankAccount.Bank.Name}",
                TransactionDate = vm.DateIntial,
                TransactionId = bankAccount.AccountsTreeId,
                BranchId= branchId,
                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceBank
            });
            //رأس المال 
            db.GeneralDailies.Add(new GeneralDaily
            {
                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                Credit = vm.Amount ?? 0,
                Notes = $"رصيد أول المدة للحساب البنكى : {bankAccount.AccountName + " / " + bankAccount.Bank.Name}",
                TransactionDate = vm.DateIntial,
                TransactionId = bankAccount.AccountsTreeId,
                BranchId= branchId,
                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceBank
            });
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
                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceBank).ToList();
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
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