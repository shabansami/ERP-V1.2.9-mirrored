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
using ERP.Web.Identity;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PropertyRightsIntialBalancesController : Controller
    {
        // GET: PropertyRightsIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public PropertyRightsIntialBalancesController()
        {
            db = new VTSaleEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            //حساب رأس المال
            Guid capitalAccId;

            if (!Guid.TryParse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue, out capitalAccId))
                capitalAccId = Guid.Empty;
            int? n = null;
            //العملاء من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل عميل عمليتين يتم تسجيلهم فى جدول القيود اليومية
            var generalGrouping = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalancePropertyRights && x.AccountsTreeId != capitalAccId).Select(x => new { TransactionId = x.TransactionId, Amount = x.Credit + x.Debit, Notes = x.Notes, CreatedOn = x.TransactionDate.ToString(), Actions = n, Num = n }).GroupBy(x => new { x.TransactionId, x.Num, x.Actions, x.Notes, x.Amount, x.CreatedOn }).ToList();
            var data = generalGrouping.Select(x => new
            {
                TransactionId = x.FirstOrDefault().TransactionId,
                //CustomerName = x.FirstOrDefault().CustomerName,
                Amount = x.FirstOrDefault().Amount,
                CreatedOn = x.FirstOrDefault().CreatedOn,
                Notes = x.FirstOrDefault().Notes,
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
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };

            return View(new IntialBalanceVM() { DateIntial = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.DateIntial == null || vm.AccountId == null || vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                //var accountTreeCust = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.CustomerId).FirstOrDefault().AccountsTreeId;
                if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalancePropertyRights && x.TransactionId == vm.AccountId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للحساب من قبل" });
                isInsert = true;
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountId))
                        return Json(new { isValid = false, message = "الحساب المحدد ليس بحساب فرعى" });

                    AddGeneralDailies(vm, generalSetting);
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

        void AddGeneralDailies(IntialBalanceVM vm, List<GeneralSetting> generalSetting)
        {
            if (vm.DebitCredit == 1) // الرصيد مدين
            {
                // الحساب المحدد
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = vm.AccountId,
                    Debit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة حقوق ملكية  : {db.AccountsTrees.Where(x => x.Id == vm.AccountId).FirstOrDefault().AccountName} {vm.Notes}",
                    BranchId = vm.BranchId,
                    TransactionDate = vm.DateIntial,
                    TransactionId = vm.AccountId,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalancePropertyRights
                });
                //رأس المال 
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                    Credit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة حقوق ملكية  : {db.AccountsTrees.Where(x => x.Id == vm.AccountId).FirstOrDefault().AccountName} {vm.Notes}",
                    BranchId = vm.BranchId,
                    TransactionDate = vm.DateIntial,
                    TransactionId = vm.AccountId,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalancePropertyRights
                });
            }
            else if (vm.DebitCredit == 2) //الرصيد دائن
            {
                // الحساب المحدد
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = vm.AccountId,
                    Credit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة حقوق ملكية  : {db.AccountsTrees.Where(x => x.Id == vm.AccountId).FirstOrDefault().AccountName} {vm.Notes}",
                    BranchId = vm.BranchId,
                    TransactionDate = Utility.GetDateTime(),
                    TransactionId = vm.AccountId,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalancePropertyRights
                });
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                    Debit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة حقوق ملكية  : {db.AccountsTrees.Where(x => x.Id == vm.AccountId).FirstOrDefault().AccountName} {vm.Notes}",
                    BranchId = vm.BranchId,
                    TransactionDate = Utility.GetDateTime(),
                    TransactionId = vm.AccountId,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalancePropertyRights
                });
            }

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalancePropertyRights).ToList();
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