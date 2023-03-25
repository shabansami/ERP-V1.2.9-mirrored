using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
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

    public class CustomerChequesController : Controller
    {
        // GET: CustomerCheques
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        CheckClosedPeriodServices closedPeriodServices;
        public CustomerChequesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            closedPeriodServices = new CheckClosedPeriodServices();

        }

        #region استعراض كل شيكات العملاء 
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.Cheques.Where(x => !x.IsDeleted && x.CustomerId != null && x.PersonCustomer.Name.Contains(txtSearch)).Select(x => new { Id = x.Id, CustomerName = x.PersonCustomer.Name, IsCollected = x.IsCollected, CheckStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : x.BankAccountId != null ? "تحت التحصيل" : "تحديدالحساب البنكى", IsRefused = x.IsRefused, CheckNumber = x.CheckNumber, BankAccountName = x.BankAccount != null ? x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name : null, Amount = x.Amount, CollectionDate = x.CollectionDate.ToString(), CheckDate = x.CheckDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
            {
                    data = db.Cheques.Where(x => !x.IsDeleted && x.CustomerId != null).Select(x => new { Id = x.Id, CustomerName = x.PersonCustomer.Name, IsCollected = x.IsCollected, CheckStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : x.BankAccountId != null ? "تحت التحصيل" : "تحديدالحساب البنكى", IsRefused = x.IsRefused, CheckNumber = x.CheckNumber, BankAccountName = x.BankAccount != null ? x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name : null, Amount = x.Amount, CollectionDate = x.CollectionDate.ToString(), CheckDate = x.CheckDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region اضافة عملية تسجيل شيك من عميل 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            // add
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.Branchcount = branches.Count();
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name",branchId);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            ViewBag.LastRow = db.Cheques.Where(x => !x.IsDeleted && x.CustomerId != null).OrderByDescending(x => x.Id).FirstOrDefault();
            return View(new Cheque() { CheckDate = Utility.GetDateTime(), CollectionDate = Utility.GetDateTime(), CheckDueDate = Utility.GetDateTime() });
        }
        [HttpPost]
        public JsonResult CreateEdit(Cheque vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.CustomerId == null || vm.Amount == 0 || vm.BranchId == null || vm.BankAccountId == null || string.IsNullOrEmpty(vm.CheckNumber))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.IsCollected)
                {
                    if (vm.CollectionDate == null)
                        return Json(new { isValid = false, message = "خطأ .... تأكد من اختيار تاريخ التحصيل" });
                }
                else
                {
                    if (vm.CheckDueDate == null)
                        return Json(new { isValid = false, message = "خطأ .... تأكد من اختيار تاريخ الاستحقاق" });
                }
                if (vm.InvoiceId != null)
                {
                    if (Guid.TryParse(vm.InvoiceId.ToString(), out var id))
                    {
                        var sellIsExists = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == vm.InvoiceId).Count();
                        if (sellIsExists == 0)
                            return Json(new { isValid = false, message = "خطأ .... رقم الفاتورة غير موجود فى فواتير البيع" });
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال رقم فاتورة البيع بشكل صحيح " });

                }
                var checkdate = closedPeriodServices.IsINPeriod(vm.CollectionDate.ToString());
                if (!checkdate)
                {
                    return Json(new { isValid = false, message = "تاريخ المعاملة خارج فترة التشغيل " });

                }
                var isInsert = false;

                if (db.Cheques.Where(x => !x.IsDeleted && x.CustomerId == vm.CustomerId && x.CheckNumber == vm.CheckNumber).Count() > 0) ///??
                    return Json(new { isValid = false, message = "رقم الشيك موجود مسبقا" });
                if (vm.IsCollected)
                    vm.CheckDueDate = null;
                else
                    vm.CollectionDate = null;

                Guid? accountBankTreeId;
                List<GeneralSetting> generalSetting;
                Person customer;
                //التأكد من عدم وجود حساب تشغيلى من الحسابات المستخدمة
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                    customer = db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault();
                    accountBankTreeId = db.BankAccounts.Where(x => x.Id == vm.BankAccountId).FirstOrDefault().AccountsTreeId;
                    //التأكد من عدم وجود حساب تشغيلى من حساب العميل
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(customer.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = "حساب العميل ليس بحساب تشغيلى" });

                    if (vm.IsCollected)
                    {
                        // التأكد من عدم وجود حساب تشغيلى من الحساب البنك
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(accountBankTreeId))
                            return Json(new { isValid = false, message = "حساب البنك ليس بحساب تشغيلى" });
                    }
                    else
                            //التأكد من عدم وجود حساب تشغيلى من شيكات تحت التحصيل
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب الشيكات تحت التحصيل ليس بحساب تشغيلى" });
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });



                //use transaction 
                using (var context = new VTSaleEntities())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            //    //تسجيل القيود
                            // General Dailies
                            var newCheck = new Cheque
                            {
                                BranchId = vm.BranchId,
                                BankAccountId = vm.BankAccountId,
                                InvoiceId = vm.InvoiceId,
                                CustomerId = vm.CustomerId,
                                Amount = vm.Amount,
                                Notes = vm.Notes,
                                CollectionDate = vm.CollectionDate,
                                CheckDate = vm.CheckDate,
                                CheckNumber = vm.CheckNumber,
                                CheckDueDate = vm.CheckDueDate,
                                IsCollected = vm.IsCollected

                            };
                            context.Cheques.Add(newCheck);
                            context.SaveChanges(auth.CookieValues.UserId);

                            // من حـ/ البنك/شيكات تحت التحصيل    الى حـ/ العميل
                            // من حـ/ البنك/شيكات تحت التحصيل    الى حـ/ العميل
                            Guid? transactionShared = null;
                            if (newCheck.InvoiceId != null)
                                transactionShared = context.SellInvoices.Where(x => !x.IsDeleted && x.Id == newCheck.InvoiceId).FirstOrDefault().Id;


                            if (vm.IsCollected)
                            {
                                // حساب  البنك
                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = accountBankTreeId,
                                    Debit = vm.Amount,
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    TransactionDate =vm.CollectionDate,
                                    TransactionId = newCheck.Id,
                                    TransactionShared = transactionShared,
                                    TransactionTypeId = (int)TransactionsTypesCl.ChequeIn
                                });
                            }
                            else
                            {
                                // حساب  شيكات تحت التحصيل
                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue),
                                    Debit = vm.Amount,
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    TransactionDate = Utility.GetDateTime(),
                                    TransactionId = newCheck.Id,
                                    TransactionShared = transactionShared,
                                    TransactionTypeId = (int)TransactionsTypesCl.ChequeIn
                                });

                                //اضافة اشعار 
                                if ( vm.CheckDueDate != null)
                                {
                                    context.Notifications.Add(new Notification
                                    {
                                        Name = $"شيك رقم : {vm.CheckNumber} على العميل: {customer.Name}",
                                        DueDate = vm.CheckDueDate,
                                        RefNumber = newCheck.Id,
                                        NotificationTypeId = (int)NotificationTypeCl.ChequesClients,
                                        Amount = vm.Amount
                                    });
                                }
                            }

                            //العميل 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                Credit = vm.Amount,
                                Notes = vm.Notes,
                                BranchId = vm.BranchId,
                                TransactionDate = vm.CollectionDate != null ? vm.CollectionDate : Utility.GetDateTime(),
                                TransactionId = newCheck.Id,
                                TransactionShared = transactionShared,
                                TransactionTypeId = (int)TransactionsTypesCl.ChequeIn
                            });

                            context.SaveChanges(auth.CookieValues.UserId);

                            transaction.Commit();
                            return Json(new { isValid = true, isInsert, message = "تم اضافة الشيك بنجاح" });

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                        }
                    }
                }

            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        #endregion

        #region حذف شيك بكل معاملاتة
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Cheques.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف كل القيود المرتبطة بالشيك
                    var generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.ChequeIn).ToList();
                    foreach (var item in generalDailies)
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