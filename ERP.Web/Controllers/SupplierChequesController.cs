using ERP.DAL;
using ERP.Web.Identity;
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

    public class SupplierChequesController : Controller
    {
        // GET: SupplierCheques
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        CheckClosedPeriodServices closedPeriodServices;
        public SupplierChequesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            closedPeriodServices = new CheckClosedPeriodServices();

        }

        #region استعراض كل شيكات الموردين 
        public ActionResult Index()
        {
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
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
                    data = db.Cheques.Where(x => !x.IsDeleted && x.SupplierId != null && x.PersonSupplier.Name.Contains(txtSearch)).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, SupplierName = x.PersonSupplier.Name, IsCollected = x.IsCollected, CheckStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : x.BankAccountId != null ? "تحت التحصيل" : "تحديدالحساب البنكى", IsRefused = x.IsRefused, CheckNumber = x.CheckNumber, BankAccountName = x.BankAccount != null ? x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name : null, Amount = x.Amount, CollectionDate = x.CollectionDate.ToString(), CheckDate = x.CheckDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = db.Cheques.Where(x => !x.IsDeleted && x.SupplierId != null).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, SupplierName = x.PersonSupplier.Name, IsCollected = x.IsCollected, CheckStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : x.BankAccountId != null ? "تحت التحصيل" : "تحديدالحساب البنكى", IsRefused = x.IsRefused, CheckNumber = x.CheckNumber, BankAccountName = x.BankAccount != null ? x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name : null, Amount = x.Amount, CollectionDate = x.CollectionDate.ToString(), CheckDate = x.CheckDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
        }
        #endregion

        #region اضافة عملية تسجيل شيك الى مورد 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            // add
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            ViewBag.LastRow = db.Cheques.Where(x => !x.IsDeleted && x.SupplierId != null).OrderByDescending(x => x.Id).FirstOrDefault();
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");

            return View(new Cheque() { CheckDate = Utility.GetDateTime(), CollectionDate = Utility.GetDateTime(), CheckDueDate = Utility.GetDateTime() });
        }
        [HttpPost]
        public JsonResult CreateEdit(Cheque vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.SupplierId == null || vm.Amount == 0 || vm.BranchId == null || vm.BankAccountId == null || string.IsNullOrEmpty(vm.CheckNumber))
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
                        var sellIsExists = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.Id == vm.InvoiceId).Count();
                        if (sellIsExists == 0)
                            return Json(new { isValid = false, message = "خطأ .... رقم الفاتورة غير موجود فى فواتير التوريد" });
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
                if (db.Cheques.Where(x => !x.IsDeleted && x.SupplierId == vm.SupplierId && x.CheckNumber == vm.CheckNumber).Count() > 0) ///??
                    return Json(new { isValid = false, message = "رقم الشيك موجود مسبقا" });
                if (vm.IsCollected)
                    vm.CheckDueDate = null;
                else
                    vm.CollectionDate = null;

                Guid? accountBankTreeId;
                List<GeneralSetting> generalSetting;
                Person supplier;
                //التأكد من عدم وجود حساب فرعى من الحسابات المستخدمة
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                    supplier = db.Persons.Where(x => x.Id == vm.SupplierId).FirstOrDefault();
                    accountBankTreeId = db.BankAccounts.Where(x => x.Id == vm.BankAccountId).FirstOrDefault().AccountsTreeId;
                    //التأكد من عدم وجود حساب فرعى من حساب العميل
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(supplier.AccountTreeSupplierId))
                        return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                    if (vm.IsCollected)
                    {
                        // التأكد من عدم وجود حساب فرعى من الحساب البنك
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(accountBankTreeId))
                            return Json(new { isValid = false, message = "حساب البنك ليس بحساب فرعى" });
                    }
                    else
                            //التأكد من عدم وجود حساب فرعى من شيكات تحت التحصيل
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب الشيكات تحت التحصيل (برسم السداد) ليس بحساب فرعى" });
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
                                SupplierId = vm.SupplierId,
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
                            Guid? transactionShared = null;
                            if (newCheck.InvoiceId != null)
                                transactionShared = context.PurchaseInvoices.Where(x => !x.IsDeleted && x.Id == newCheck.InvoiceId).FirstOrDefault().Id;

                            if (vm.IsCollected)
                            {
                                // حساب  البنك
                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = accountBankTreeId,
                                    Credit = vm.Amount,
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    TransactionDate = vm.CollectionDate,
                                    TransactionId = newCheck.Id,
                                    TransactionShared = transactionShared,
                                    TransactionTypeId = (int)TransactionsTypesCl.ChequeOut
                                });
                            }
                            else
                            {
                                // حساب  شيكات تحت التحصيل
                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue),
                                    Credit = vm.Amount,
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    TransactionDate = Utility.GetDateTime(),
                                    TransactionId = newCheck.Id,
                                    TransactionShared = transactionShared,
                                    TransactionTypeId = (int)TransactionsTypesCl.ChequeOut
                                });

                                //اضافة اشعار 
                                if (vm.CheckDueDate != null)
                                {
                                    context.Notifications.Add(new Notification
                                    {
                                        Name = $"شيك رقم : {vm.CheckNumber} للمورد: {supplier.Name}",
                                        DueDate = vm.CheckDueDate,
                                        RefNumber = newCheck.Id,
                                        NotificationTypeId = (int)NotificationTypeCl.ChequesSuppliers,
                                        Amount = vm.Amount
                                    });
                                }
                            }

                            //المورد 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = supplier.AccountTreeSupplierId,
                                Debit = vm.Amount,
                                Notes = vm.Notes,
                                BranchId = vm.BranchId,
                                TransactionDate = vm.CollectionDate!=null?vm.CollectionDate:Utility.GetDateTime(),
                                TransactionId = newCheck.Id,
                                TransactionShared = transactionShared,
                                TransactionTypeId = (int)TransactionsTypesCl.ChequeOut
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
                    var generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.ChequeOut).ToList();
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