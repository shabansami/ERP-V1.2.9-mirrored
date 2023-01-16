using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ERP.Web.ViewModels;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoiceInstallmentSchedulesController : Controller
    {
        // GET: SellInvoiceInstallmentSchedules
        // GET: SellInvoiceInstallmentSchedules
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        SellInvoiceInstallmentService installmentService = new SellInvoiceInstallmentService();
        public ActionResult Index()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }

        public ActionResult GetAll(bool isPaid, string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<SellInvoiceInstallmentScheduleDto> list;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = installmentService.GetInstallmentSchedules(isPaid, dtFrom, dtTo, customerId);
            }
            else
                list = installmentService.GetInstallmentSchedules(isPaid, null, null, customerId);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;


        }
        public ActionResult Paid(string scId)
        {
            //الخزينة فى حالة السداد

            Guid scheduleGuid;
            SellInvoiceInstallmentScheduleVM vm = new SellInvoiceInstallmentScheduleVM();
            if (Guid.TryParse(scId, out scheduleGuid))
            {
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", 1);
                var model = db.InstallmentSchedules.Where(x => x.Id == scheduleGuid).FirstOrDefault();
                if (model != null)
                {
                    vm = new SellInvoiceInstallmentScheduleVM
                    {
                        ScheduleGuid = scheduleGuid,
                        Amount = model.Amount,
                        PayedAmount = model.Amount,
                        InstallmentDate = model.InstallmentDate,
                        InstallmentId = model.InstallmentId,
                        PaymentDate = Utility.GetDateTime(),
                        CustomerName = model.Installment?.SellInvoice != null ? model.Installment.SellInvoice.PersonCustomer.Name : model.Installment.CustomerIntialBalance.Person.Name
                    };
                    ViewBag.OtherScheduleNotPaidId = new SelectList(model.Installment.InstallmentSchedules.Where(x => !x.IsDeleted && !x.IsPayed && x.Id != model.Id).OrderBy(x=>x.InstallmentDate).Select(x => new { Id = x.Id, Name = x.InstallmentDate.Value.ToString("yyyy-MM-dd") }).ToList(), "Id", "Name");
                    //ViewBag.OtherScheduleNotPaidId = new SelectList(db.InstallmentSchedules.Where(x=>!x.IsDeleted&&x.InstallmentId==model.InstallmentId&&!x.IsPayed&& x.Id != model.Id), "Id", "Name", 1);
                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Paid(SellInvoiceInstallmentScheduleVM vm)
        {
            if (vm.SafeId == null)
                return Json(new { isValid = false, message = "تأكد من اختيار الخزنة" });
            if (vm.PaymentDate == null)
                return Json(new { isValid = false, message = "تأكد من اختيار تاريخ التحصيل" });
            if (vm.PayedAmount == 0 || vm.PayedAmount < 0)
                return Json(new { isValid = false, message = "تأكد من ادخال المبلغ المدفوع بشكل صحيح" });

            var model = db.InstallmentSchedules.Where(x => x.Id == vm.ScheduleGuid).FirstOrDefault();
            bool isHasRemindAmount = false;
            if (vm.DifValue != 0)
            {
                if (vm.OtherScheduleNotPaidId == null)
                {
                    var otherSchedulesNotPaid = model.Installment.InstallmentSchedules.Where(x => !x.IsDeleted && !x.IsPayed && x.Id != model.Id).Any();
                    if (otherSchedulesNotPaid)
                        return Json(new { isValid = false, message = "تأكد من اختيار القسط المراد تحميل الزيادة/النقصان عليه" });
                    else
                        isHasRemindAmount = true;
                }
                //التأكد من ان المبلغ المدفوع 
            }
            if (model != null)
            {
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                Guid? customerId;
                Guid? branchId;
                string notes, notesRemindVal;
                //string noteProfit=string.Empty;
                Guid? invoGuid = null;
                var sellInvoice = db.SellInvoices.Where(x => x.Id == model.Installment.SellInvoiceId).FirstOrDefault();
                if (sellInvoice == null)
                {
                    var customerInit = model.Installment.CustomerIntialBalance;
                    if (customerInit != null)
                    {
                        customerId = customerInit.PersonId;
                        branchId = customerInit.BranchId;
                        invoGuid = customerInit.Id;
                        notes = $"تحصيل قسط من رصيد اول رقم  : {customerInit.Id} من العميل {customerInit.Person?.Name}";
                        //البيان فى حالة سداد قسط اكبر /اقل من قيمة وتحميل باقى المبلغ على حساب العميل 
                        notesRemindVal = $"مبلغ متبقى من اخر قسط من رصيد اول رقم  : {customerInit.Id} من العميل {customerInit.Person?.Name}";
                    }
                    else
                        return Json(new { isValid = false, message = "خطأ فى بيانات تسجيل القسط ... تأكد من ان القسط مرتبط بفاتورة بيع او رصيد او المدة" });

                }
                else
                {
                    if (!sellInvoice.IsFinalApproval)
                        return Json(new { isValid = false, message = "لا يمكن السداد والفاتورة لم يتم اعتمادها بعد" });
                    customerId = sellInvoice.CustomerId;
                    branchId = sellInvoice.BranchId;
                    invoGuid = sellInvoice.Id;
                    notes = $"تحصيل قسط من فاتورة بيع رقم : {sellInvoice.Id} من العميل {sellInvoice.PersonCustomer?.Name}";
                    //البيان فى حالة سداد قسط اكبر /اقل من قيمة وتحميل باقى المبلغ على حساب العميل 
                    notesRemindVal = $"مبلغ متبقى من اخر قسط  من فاتورة بيع رقم : {sellInvoice.Id} من العميل {sellInvoice.PersonCustomer?.Name}";

                }
                // الحصول على حسابات من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                //التأكد من عدم وجود حساب فرعى من الحساب
                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeInstallmentsBenefits).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب فوائد الاقساط ليس بحساب فرعى" });

                var customerAccountId = db.Persons.Where(x => x.Id == customerId).FirstOrDefault().AccountsTreeCustomerId;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(customerAccountId))
                    return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                //تسجيل قيود الدفعة
                //  المبلغ المدفوع .. من حساب الخزينة (مدين
                var safeAccountId = db.Safes.Where(x => x.Id == vm.SafeId).FirstOrDefault().AccountsTreeId;
                var fullDate = vm.PaymentDate.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = safeAccountId,
                    BranchId = branchId,
                    Debit = vm.PayedAmount,
                    Notes = notes,
                    TransactionDate = fullDate,
                    TransactionId = model.Id,
                    TransactionShared = invoGuid,
                    TransactionTypeId = (int)TransactionsTypesCl.Installment
                });
                //الى حساب العميل
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = customerAccountId,
                    BranchId = branchId,
                    Credit = vm.PayedAmount,
                    Notes = notes,
                    TransactionDate = fullDate,
                    TransactionId = model.Id,
                    TransactionShared = invoGuid,
                    TransactionTypeId = (int)TransactionsTypesCl.Installment
                });

                if (isHasRemindAmount)
                {
                    //فى حالة وجود مبلغ متبقى بالزيادة
                    if (vm.DifValue > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = customerAccountId,
                            BranchId = branchId,
                            Debit = vm.DifValue,
                            Notes = notesRemindVal,
                            TransactionDate = fullDate,
                            TransactionId = model.Id,
                            TransactionShared = invoGuid,
                            TransactionTypeId = (int)TransactionsTypesCl.Installment
                        });
                    }
                    else
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = customerAccountId,
                            BranchId = branchId,
                            Credit = vm.DifValue * -1,
                            Notes = notesRemindVal,
                            TransactionDate = fullDate,
                            TransactionId = model.Id,
                            TransactionShared = invoGuid,
                            TransactionTypeId = (int)TransactionsTypesCl.Installment
                        });
                    }
                }
                //سداد 
                model.IsPayed = true;
                model.PaymentDate = fullDate;
                model.SafeId = vm.SafeId;
                model.Amount = vm.PayedAmount;
                //التأكد من السداد تم فى الموعد المسموح به ام متأخر 
                var general = db.GeneralSettings.Where(x => !x.IsDeleted && x.Id == (int)GeneralSettingCl.PeriodAllowedPayInstallment).FirstOrDefault().SValue;
                int days;
                if (int.TryParse(general, out days))
                {
                    if (vm.PaymentDate > model.InstallmentDate.Value.AddDays(days))
                        model.PaidInTime = false;
                    else
                        model.PaidInTime = true;
                }

                db.Entry(model).State = EntityState.Modified;
                if (vm.DifValue != 0)
                {
                    var otherSch = db.InstallmentSchedules.Where(x => x.Id == vm.OtherScheduleNotPaidId).FirstOrDefault();
                    if (otherSch != null)
                    {
                        otherSch.Amount = otherSch.Amount + vm.DifValue;
                        db.Entry(otherSch).State = EntityState.Modified;
                    }
                }
                //تحديث طريقة السداد فى فاتورة البيع
                //sellInvoice.SafeId = SafeId;
                //db.Entry(sellInvoice).State = EntityState.Modified;

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم السداد بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


        }

        [HttpPost]
        public ActionResult UnApprovalSchedule(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.InstallmentSchedules.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //تحديث حالة الاعتماد 
                    model.IsPayed = false;
                    model.PaidInTime = false;
                    model.PaymentDate = null;
                    model.SafeId = null;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.Notes.Contains("تحصيل قسط") && x.TransactionTypeId == (int)TransactionsTypesCl.Installment).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم فك الاعتماد بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }


    }
}