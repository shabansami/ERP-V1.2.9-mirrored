using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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

    public class SellInvoiceInstallmentsController : Controller
    {
        // GET: SellInvoiceInstallments
        VTSaleEntities db;
        VTSAuth auth;
        SellInvoiceInstallmentService installmentService;
        public static string DSPyments { get; set; }
        public static string vmDs { get; set; }
        public SellInvoiceInstallmentsController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            installmentService = new SellInvoiceInstallmentService();
        }
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

        public ActionResult GetAll(string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<SellInvoiceInstallmentDto> list;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = installmentService.GetSellInvoiceInstallments(dtFrom, dtTo, customerId, false);
            }
            else
                list = installmentService.GetSellInvoiceInstallments(null, null, customerId, false);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;


        }


        [HttpGet]
        public ActionResult RegisterInstallments(string invoGuid, string typ, string fi)//fi=null firist init false لمعالجة الداتا سورس ستاتيك 
        {
            if (fi == null)
            {
                DSPyments = null;
                vmDs = null;
            }
            Guid sellGuid;
            Guid custInitialId;
            List<SellInvoiceInstallmentSchedules> generatePaymentList = new List<SellInvoiceInstallmentSchedules>();
            SellInvoiceInstallmentVM vm = new SellInvoiceInstallmentVM();
            SellInvoiceInstallmentVM installVm = new SellInvoiceInstallmentVM();

            if (!string.IsNullOrEmpty(typ))
            {
                if (typ.Trim() == "sell") //فاتورة البيع 
                {
                    if (Guid.TryParse(invoGuid, out sellGuid))
                    {
                        var sell = db.SellInvoices.Where(x => x.Id == sellGuid).FirstOrDefault();
                        if (DSPyments != null)
                            generatePaymentList = JsonConvert.DeserializeObject<List<SellInvoiceInstallmentSchedules>>(DSPyments);
                        if (vmDs != null)
                        {
                            vm = JsonConvert.DeserializeObject<SellInvoiceInstallmentVM>(vmDs);
                            vm.CustomerName = sell.PersonCustomer != null ? sell.PersonCustomer.Name : null;
                            vm.InvoiceDate = sell.InvoiceDate;
                            vm.Safy = sell.Safy;
                        }

                        //الخزينة فى حالة السداد
                        //ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", 1);
                        if (vmDs != null)
                        {
                            vm.sellInvoiceInstallmentSchedules = generatePaymentList;
                            installVm = vm;

                        }
                        else
                        {
                            installVm = new SellInvoiceInstallmentVM
                            {
                                InvoiceDate = sell.InvoiceDate,
                                RemindValue = sell.RemindValue,
                                PayedValue = sell.PayedValue,
                                Safy = sell.Safy,
                                IsSell = true,
                                CustomerName = sell.PersonCustomer != null ? sell.PersonCustomer.Name : null,
                                SellInvoiceId = sell.Id,
                                InvoiceNumber=sell.InvoiceNumber,
                                FirstDueDate = Utility.GetDateTime(),
                                sellInvoiceInstallmentSchedules = generatePaymentList.Count() > 0 ? generatePaymentList : sell.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? sell.Installments.Where(x => !x.IsDeleted).FirstOrDefault().InstallmentSchedules.Where(x => !x.IsDeleted).Select(x => new SellInvoiceInstallmentSchedules { ScheduleId = x.Id, Amount = x.Amount, InstallmentDate = x.InstallmentDate, IsPayed = x.IsPayed }).ToList() : new List<SellInvoiceInstallmentSchedules>()
                            };
                            var inst = sell.Installments.Where(x => !x.IsDeleted).FirstOrDefault();
                            if (inst != null)
                            {
                                installVm.Duration = inst.Duration;
                                installVm.FirstDueDate = inst.StartDate;
                                installVm.ProfitValue = inst.ProfitValue;
                                installVm.FinalSafy = inst.TotalValue;
                                installVm.PayValue = inst.InstallmentSchedules.Where(x => !x.IsDeleted).FirstOrDefault().Amount;
                            }

                        }
                    }
                    else
                        return RedirectToAction("Index");
                }
                else if (typ.Trim() == "initial") // رصيد اول العملاء
                {
                    if (Guid.TryParse(invoGuid, out custInitialId))
                    {
                        var custInitial = db.PersonIntialBalances.Where(x => x.Id == custInitialId).FirstOrDefault();

                        if (DSPyments != null)
                            generatePaymentList = JsonConvert.DeserializeObject<List<SellInvoiceInstallmentSchedules>>(DSPyments);
                        if (vmDs != null)
                        {
                            vm = JsonConvert.DeserializeObject<SellInvoiceInstallmentVM>(vmDs);
                            vm.CustomerName = custInitial.Person != null ? custInitial.Person.Name : null;
                            vm.InvoiceDate = custInitial.OperationDate;
                            vm.Safy = custInitial.Amount;
                        }

                        if (vmDs != null)
                        {
                            vm.sellInvoiceInstallmentSchedules = generatePaymentList;
                            installVm = vm;
                        }
                        else
                        {
                            installVm = new SellInvoiceInstallmentVM
                            {
                                InvoiceDate = custInitial.OperationDate,
                                RemindValue = custInitial.Amount,
                                //InvoiceGuid = sell.InvoiceGuid,
                                Safy = custInitial.Amount,
                                IsSell = false,
                                CustomerName = custInitial.Person != null ? custInitial.Person.Name : null,
                                SellInvoiceId = custInitial.Id,
                                InvoiceNumber = custInitial.Id.ToString(),
                                FirstDueDate = Utility.GetDateTime(),
                                sellInvoiceInstallmentSchedules = generatePaymentList.Count() > 0 ? generatePaymentList : custInitial.Installments.Where(x => !x.IsDeleted).FirstOrDefault() != null ? custInitial.Installments.Where(x => !x.IsDeleted).FirstOrDefault().InstallmentSchedules.Where(x => !x.IsDeleted).Select(x => new SellInvoiceInstallmentSchedules { ScheduleId = x.Id, Amount = x.Amount, InstallmentDate = x.InstallmentDate, IsPayed = x.IsPayed}).ToList() : new List<SellInvoiceInstallmentSchedules>()
                            };
                            var inst = custInitial.Installments.Where(x => !x.IsDeleted).FirstOrDefault();
                            if (inst != null)
                            {
                                installVm.Duration = inst.Duration;
                                installVm.FirstDueDate = inst.StartDate;
                                installVm.ProfitValue = inst.ProfitValue;
                                installVm.FinalSafy = inst.TotalValue;
                                installVm.PayValue = inst.InstallmentSchedules.Where(x => !x.IsDeleted).FirstOrDefault().Amount;
                            }

                        }
                    }
                    else
                        return RedirectToAction("Index");

                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

            return View(installVm);
        }
        [HttpPost]
        public ActionResult RegisterInstallments(string data, double? finalSafy, bool IsSell)
        {
            List<SellInvoiceInstallmentSchedules> deDS = new List<SellInvoiceInstallmentSchedules>();
            deDS = JsonConvert.DeserializeObject<List<SellInvoiceInstallmentSchedules>>(data);

            SellInvoiceInstallmentVM vm = new SellInvoiceInstallmentVM();
            if (vmDs != null)
                vm = JsonConvert.DeserializeObject<SellInvoiceInstallmentVM>(vmDs);

            if (deDS.Count() == 0)
                return Json(new { isValid = false, message = "تأكد من انشاء دفعات اولا" });

            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            List<InstallmentSchedule> installmentSchedules = new List<InstallmentSchedule>();
            List<Notification> notifications = new List<Notification>();
            Guid? invoId;
            string notyName = "";
            Guid? refNumber;
            double remind;
            Person customer;
            Guid? branchId;
            string noteProfit = string.Empty;
            invoId = deDS[0].SellInvoiceId;
            Guid? invoGuid;
            using (var context = new VTSaleEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (IsSell)
                        {
                            var sellInvo = context.SellInvoices.Where(x => x.Id == invoId).FirstOrDefault();
                            notyName = $"استحقاق قسط من فاتورة بيع رقم: {sellInvo.InvoiceNumber} على العميل: {sellInvo.PersonCustomer.Name}";
                            noteProfit = $"فوائد فاتورة بيع رقم  : {sellInvo.InvoiceNumber} على العميل: {sellInvo.PersonCustomer.Name}";
                            refNumber = sellInvo.Id;
                            remind = sellInvo.RemindValue;
                            customer = sellInvo.PersonCustomer;
                            branchId = sellInvo.BranchId;
                            invoGuid = sellInvo.Id;
                            if (context.InstallmentSchedules.Where(x => !x.IsDeleted && x.Installment.SellInvoiceId == sellInvo.Id && x.IsPayed).Any())
                                return Json(new { isValid = false, message = "لا يمكن انشاء دفعات لوجود اقساط مدفوعه بالفعل" });

                        }
                        else
                        {
                            var custIni = context.PersonIntialBalances.Where(x => x.Id == invoId).FirstOrDefault();
                            notyName = $"استحقاق قسط من رصيد اول رقم: {custIni.Id} على العميل: {custIni.Person.Name}";
                            noteProfit = $"فوائد رصيد اول رقم  : {custIni.Id} على العميل: {custIni.Person.Name}";
                            refNumber = custIni.Id;
                            remind = custIni.Amount;
                            customer = custIni.Person;
                            branchId = custIni.BranchId;
                            invoGuid = custIni.Id;
                            if (context.InstallmentSchedules.Where(x => !x.IsDeleted && x.Installment.IntialCustomerId == custIni.Id && x.IsPayed).Any())
                                return Json(new { isValid = false, message = "لا يمكن انشاء دفعات لوجود اقساط مدفوعه بالفعل" });

                        }
                        var tt = Math.Round(deDS.Sum(x => x.Amount), 0);
                        if (int.Parse(finalSafy.ToString()) != Math.Round(deDS.Sum(x => x.Amount), 0))
                            return Json(new { isValid = false, message = "اجمالى المبلغ المتبقى لا يتساوى مع الدفعات المدخلة" });


                        foreach (var item in deDS)
                        {
                            installmentSchedules.Add(new InstallmentSchedule
                            {
                                Id = item.ScheduleId,
                                Amount = item.Amount,
                                InstallmentDate = item.InstallmentDate,
                                PaymentDate = null
                            });
                            notifications.Add(new Notification
                            {
                                Name = notyName,
                                DueDate = item.InstallmentDate,
                                RefNumber = refNumber,
                                NotificationTypeId = (int)NotificationTypeCl.SellInvoiceInstallments,
                                Amount = item.Amount
                            });
                        }
                        //حذف اى اقساط سابقة 
                        if (IsSell)
                        {
                            var prevouisInstallments = context.Installments.Where(x => !x.IsDeleted && x.SellInvoiceId == invoId).ToList();
                            foreach (var item in prevouisInstallments)
                            {
                                item.IsDeleted = true;
                                context.Entry(item).State = EntityState.Modified;
                                var prevouisInstallmentSchedules = item.InstallmentSchedules.Where(x => !x.IsDeleted).ToList();
                                foreach (var itemSchedule in prevouisInstallmentSchedules)
                                {
                                    itemSchedule.IsDeleted = true;
                                    context.Entry(itemSchedule).State = EntityState.Modified;
                                }
                            }

                        }
                        else
                        {
                            var prevouisInstallments = context.Installments.Where(x => !x.IsDeleted && x.IntialCustomerId == invoId).ToList();
                            foreach (var item in prevouisInstallments)
                            {
                                item.IsDeleted = true;
                                context.Entry(item).State = EntityState.Modified;
                                var prevouisInstallmentSchedules = item.InstallmentSchedules.Where(x => !x.IsDeleted).ToList();
                                foreach (var itemSchedule in prevouisInstallmentSchedules)
                                {
                                    itemSchedule.IsDeleted = true;
                                    context.Entry(itemSchedule).State = EntityState.Modified;
                                }
                            }
                        }


                        //حذف اى اقساط من الاشعارات
                        var prevouisNotyInstallmentSchedules = context.Notifications.Where(x => !x.IsDeleted && x.RefNumber == invoId && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceInstallments).ToList();
                        foreach (var item in prevouisNotyInstallmentSchedules)
                        {
                            item.IsDeleted = true;
                            context.Entry(item).State = EntityState.Modified;
                        }
                        //اضافة الاقساط الجديدة
                        var installment = new Installment
                        {
                            SellInvoiceId = IsSell ? invoId : null,
                            IntialCustomerId = !IsSell ? invoId : null,
                            Duration = vm.Duration,
                            PayedValue = vm.PayedValue,
                            ProfitValue = vm.ProfitValue,
                            RemindValue = remind,
                            StartDate = vm.FirstDueDate,
                            CommissionVal = vm.CommissionVal,
                            TotalValue = vm.FinalSafy,
                            InstallmentSchedules = installmentSchedules,
                        };
                        //            //اضافة رقم العملية
                        string codePrefix = Properties.Settings.Default.CodePrefix;
                        installment.OperationNumber = codePrefix + (db.Installments.Count(x => installment.OperationNumber.StartsWith(codePrefix)) + 1);
                        context.Installments.Add(installment);

                        //اضافة الاشعارات 
                        context.Notifications.AddRange(notifications);
                        context.SaveChanges(auth.CookieValues.UserId);

                        //اضافة قيود الفوائد 
                        // الحصول على حسابات من الاعدادات
                        var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                        //التأكد من عدم وجود حساب تشغيلى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeInstallmentsBenefits).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب فوائد الاقساط ليس بحساب تشغيلى" });

                        var customerAccountId = customer.AccountsTreeCustomerId;
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(customerAccountId))
                            return Json(new { isValid = false, message = "حساب العميل ليس بحساب تشغيلى" });

                        //الى حساب فوائد الاقساط
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeInstallmentsBenefits).FirstOrDefault().SValue),
                            BranchId = branchId,
                            Credit = vm.ProfitValue,
                            Notes = noteProfit,
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = installment.Id,
                            TransactionShared = invoGuid,
                            TransactionTypeId = (int)TransactionsTypesCl.Installment
                        });

                        //العميل مدين ربط القيود بمعامله واحدة 
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = customer.AccountsTreeCustomerId,
                            BranchId = branchId,
                            Debit = vm.ProfitValue,
                            Notes = noteProfit,
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = installment.Id,
                            TransactionShared = invoGuid,
                            TransactionTypeId = (int)TransactionsTypesCl.Installment
                        });

                        context.SaveChanges(auth.CookieValues.UserId);
                        vmDs = null;
                        DSPyments = null;

                        transaction.Commit();
                        return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.Installment, refGid = installment.Id, message = "تم تسجيل الاقساط بنجاح" });

                        //return Json(new { isValid = true, message = "تم تسجيل الاقساط بنجاح" });

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                    }
                }
            }




        }
        [HttpPost]
        public ActionResult GeneratePyments(SellInvoiceInstallmentVM vm)
        {
            if (vm.Duration == 0 || vm.FirstDueDate == null || vm.RemindValue == 0 || vm.PayValue == 0)
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
            List<SellInvoiceInstallmentSchedules> list = new List<SellInvoiceInstallmentSchedules>();
            var pay = Math.Round(vm.PayValue, 2);
            for (int i = 0; i < vm.Duration; i++)
            {
                var duDate = vm.FirstDueDate.Value.AddMonths(i);
                list.Add(new SellInvoiceInstallmentSchedules
                {
                    InstallmentDate = duDate,
                    Amount = pay
                });
            }
            var sellVm = new SellInvoiceInstallmentVM
            {
                Duration = vm.Duration,
                FirstDueDate = vm.FirstDueDate,
                SellInvoiceId = vm.SellInvoiceId,
                InvoiceNumber=vm.InvoiceNumber,
                PayedValue = vm.PayedValue,
                PayValue = vm.PayValue,
                CommissionPerc = vm.CommissionPerc,
                CommissionVal = vm.CommissionVal,
                ProfitValue = vm.ProfitValue,
                RemindValue = vm.RemindValue,
                TotalValue = vm.TotalValue,
                Safy = vm.Safy,
                IsSell = vm.IsSell,
                FinalSafy = vm.FinalSafy,
                CustomerName = vm.CustomerName,
                InvoiceDate = vm.InvoiceDate,
            };
            //string listSerilize = JsonConvert.SerializeObject(list);
            DSPyments = JsonConvert.SerializeObject(list);
            vmDs = JsonConvert.SerializeObject(sellVm);
            return Json(new { isValid = true, invoGuid = vm.SellInvoiceId, SellInvoiceId = vm.SellInvoiceId, message = "تم انشاء الدفعات بنجاح" });
        }

        [HttpPost]//تغيير طريقة السداد من (جزئى / اجل ) الى تقسيط
        public ActionResult ChangePaymentTypeToInstallment(string id)
        {
            Guid invoGuid;
            if (Guid.TryParse(id, out invoGuid))
            {
                var model = db.SellInvoices.Where(x => x.Id == invoGuid).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.PaymentTypeId = (int)Lookups.PaymentTypeCl.Installment;
                    db.Entry(model).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم التغيير بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


        }

        #region حاسبة القسط 
        public ActionResult Calculate()
        {
            return View(new SellInvoiceInstallmentVM());
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