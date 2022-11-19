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

    public class SafeIntialBalancesController : Controller
    {
        // GET: SafeIntialBalances
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            // اسماء الخزن من جدول بيرسون
            var safes = db.Safes.Where(x => !x.IsDeleted);
            //العملاء من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل عميل عمليتين يتم تسجيلهم فى جدول القيود اليومية
            var generalGrouping = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceSafe && safes.Any(p => p.AccountsTreeId == x.TransactionId && !p.IsDeleted)).OrderBy(x => x.CreatedOn).Select(x => new { TransactionId = x.TransactionId, SafeName = safes.Where(p => p.AccountsTreeId == x.TransactionId).FirstOrDefault().Name, Amount = x.Credit + x.Debit, CreatedOn = x.TransactionDate.ToString(), Actions = n, Num = n }).GroupBy(x => new { x.TransactionId, x.SafeName, x.Num, x.Actions, x.Amount, x.CreatedOn }).ToList();
            var data = generalGrouping.Select(x => new
            {
                TransactionId = x.FirstOrDefault().TransactionId,
                SafeName = x.FirstOrDefault().SafeName,
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
            //        ViewBag.SafeId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", model.TransactionId);
            //        return View(model);
            //    }
            //    else
            //        return RedirectToAction("Index");
            //}
            //else
            //{                   // add
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");

            return View(new IntialBalanceVM() { DateIntial = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.DateIntial == null || vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                //if (vm.Id > 0)
                //{
                //    if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId != vm.SafeId).Count() > 0)
                //        return Json(new { isValid = false, message = "تم تسجيل رصيد للمورد من قبل" });

                //    var oldGeneralDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId == vm.SafeId).ToList();
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
                var safe = db.Safes.Where(x => !x.IsDeleted && x.Id == vm.SafeId).FirstOrDefault();
                if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceSafe && x.TransactionId == safe.AccountsTreeId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للخزنة من قبل" });
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

                    AddGeneralDailies(vm, generalSetting, safe);
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

        void AddGeneralDailies(IntialBalanceVM vm, List<GeneralSetting> generalSetting, Safe safe)
        {
            // حساب الخزنة
            db.GeneralDailies.Add(new GeneralDaily
            {
                AccountsTreeId = safe.AccountsTreeId,
                Debit = vm.Amount ?? 0,
                BranchId = safe.BranchId,
                Notes = $"رصيد أول المدة للخزنة : {db.Safes.Where(x => x.Id == safe.Id).FirstOrDefault().Name}",
                TransactionDate = vm.DateIntial,
                TransactionId = safe.AccountsTreeId,
                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSafe
            });
            //رأس المال 
            db.GeneralDailies.Add(new GeneralDaily
            {
                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                Credit = vm.Amount ?? 0,
                BranchId = safe.BranchId,
                Notes = $"رصيد أول المدة للخزنة : {db.Safes.Where(x => x.Id == safe.Id).FirstOrDefault().Name}",
                TransactionDate = vm.DateIntial,
                TransactionId = safe.AccountsTreeId,
                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSafe
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
                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceSafe).ToList();
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