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
    public class SuppliersPaymentsController : Controller
    {
        // GET: SuppliersPayments
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public SuppliersPaymentsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
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

            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo)
        {
            int? n = null;
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.SupplierPayments.Where(x => !x.IsDeleted  && x.PersonSupplier.Name.Contains(txtSearch)).Select(x => new { Id = x.Id, SupplierName = x.PersonSupplier.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", PaymentTypeName = x.SafeId != null ? x.Safe.Name : x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.SupplierPayments.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                {
                    list = list.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo.Date);

                    return Json(new
                    {
                        data = list.Select(x => new { Id = x.Id, SupplierName = x.PersonSupplier.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", PaymentTypeName = x.SafeId != null ? x.Safe.Name : x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        data = list.Select(x => new { Id = x.Id, SupplierName = x.PersonSupplier.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", PaymentTypeName = x.SafeId != null ? x.Safe.Name : x.BankAccount.AccountName + "/" + x.BankAccount.Bank.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.SupplierPayments.FirstOrDefault(x=>x.Id==id);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name", model.PersonSupplier.PersonCategoryId);
                    ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted&& (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", model.SupplierId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                //var safes = db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId);
                
                ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.BranchId = new SelectList(branches, "Id", "Name",branchId);
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");
                ViewBag.LastRow = db.SupplierPayments.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new SupplierPayment() { PaymentDate=Utility.GetDateTime(),CheckDate=Utility.GetDateTime(),CheckDueDate=Utility.GetDateTime()});
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SupplierPayment vm)
        {
            if (ModelState.IsValid)
            {
                if ( vm.SupplierId == null||vm.Amount==0||vm.BranchId==null||vm.PaymentDate==null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.BankAccountId == null && vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" });
              //فى حالة ادخال رقم فاتورة توريد يجب التأكد من صحة الرقم المدخلو وانه من ضمن فواتير التوريد 
                if (vm.PurchaseInvoiceId!=null)
                {
                    if(int.TryParse(vm.PurchaseInvoiceId.ToString(), out var id))
                        {
                        var purchaseIsExists = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.Id == vm.PurchaseInvoiceId).Count();
                        if (purchaseIsExists==0)
                            return Json(new { isValid = false, message = "خطأ .... رقم الفاتورة غير موجود فى فواتير التوريد" });
                       }else
                        return Json(new { isValid = false, message = "تأكد من ادخال رقم فاتورة التوريد بشكل صحيح " });

                }
                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //if (db.SupplierPayments.Where(x => !x.IsDeleted && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.SupplierPayments.FirstOrDefault(x=>x.Id==vm.Id);
                    model.SupplierId = vm.SupplierId;
                    model.Amount = vm.Amount;
                    model.Notes = vm.Notes;
                   
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //if (db.SupplierPayments.Where(x => !x.IsDeleted && x.SupplierId == vm.SupplierId).Count() > 0) ///??
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                    if (vm.SafeId!=null)
                    {
                        vm.CheckDate = null;
                        vm.CheckDueDate = null;
                        vm.CheckNumber = null;
                    }
                    isInsert = true;
                    db.SupplierPayments.Add(new SupplierPayment
                    {
                        BranchId = vm.BranchId,
                        BankAccountId = vm.BankAccountId,
                        SafeId = vm.SafeId,
                        SupplierId = vm.SupplierId,
                        Amount = vm.Amount,
                        Notes = vm.Notes,
                        PaymentDate = vm.PaymentDate,
                        CheckDate=vm.CheckDate,
                        CheckNumber=vm.CheckNumber,
                        CheckDueDate=vm.CheckDueDate,
                        PurchaseInvoiceId=vm.PurchaseInvoiceId
                        
                    }) ;

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
                var model = db.SupplierPayments.FirstOrDefault(x=>x.Id==Id);
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
        [HttpPost]
        public ActionResult Approval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    var paymentSupplier = db.SupplierPayments.Where(x => x.Id == Id).FirstOrDefault();
                    //التأكد من عدم وجود حساب فرعى من حساب المورد
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(paymentSupplier.PersonSupplier.AccountTreeSupplierId))
                        return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });
                    if (paymentSupplier.SafeId != null)
                    {
                        //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(paymentSupplier.Safe.AccountsTreeId))
                            return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                    }
                    else
                    {
                        // التأكد من عدم وجود حساب فرعى من الحساب البنك
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(paymentSupplier.BankAccount.AccountsTreeId))
                            return Json(new { isValid = false, message = "حساب البنك ليس بحساب فرعى" });
                    }
                    //التاكد من القيد لم يتم تسجيله مسبقا 
                    if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == paymentSupplier.Id && x.TransactionTypeId == (int)TransactionsTypesCl.CashOut).Any())
                        return Json(new { isValid = true, message = "تم اعتماد عملية الصرف لمورد بنجاح" });
                    Guid? transactionShared = null;
                    if (paymentSupplier.PurchaseInvoiceId != null)
                        transactionShared = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.Id == paymentSupplier.PurchaseInvoiceId).FirstOrDefault().Id;

                    // حساب الخزينة او البنك
                    db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = paymentSupplier.SafeId!=null ? paymentSupplier.Safe.AccountsTreeId:paymentSupplier.BankAccount.AccountsTreeId,
                            Credit = paymentSupplier.Amount,
                            BranchId=paymentSupplier.BranchId,
                        Notes = $"صرف نقدية الى {paymentSupplier.PersonSupplier.Name}  " + paymentSupplier.Notes,
                        TransactionDate = paymentSupplier.PaymentDate,
                            TransactionId = paymentSupplier.Id,
                        TransactionShared = transactionShared,
                        TransactionTypeId = (int)TransactionsTypesCl.CashOut
                        });
                   

                    //المورد 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = paymentSupplier.PersonSupplier.AccountTreeSupplierId,
                        Debit = paymentSupplier.Amount,
                        Notes = $"صرف نقدية الى {paymentSupplier.PersonSupplier.Name}  " + paymentSupplier.Notes,
                        BranchId = paymentSupplier.BranchId,
                        TransactionDate = paymentSupplier.PaymentDate,
                        TransactionId = paymentSupplier.Id,
                        TransactionShared = transactionShared,
                        TransactionTypeId = (int)TransactionsTypesCl.CashOut
                    });

                    //تحديث حالة الاعتماد 
                    paymentSupplier.IsApproval = true;
                    db.Entry(paymentSupplier).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد عملية الصرف لمورد بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        [HttpPost]
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.SupplierPayments.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.CashOut).ToList();
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