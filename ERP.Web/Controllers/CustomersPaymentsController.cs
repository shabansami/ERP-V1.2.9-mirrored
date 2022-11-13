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
    public class CustomersPaymentsController : Controller
    {
        // GET: CustomersPayments
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public CustomersPaymentsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted&&(x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
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
                    data = db.CustomerPayments.Where(x => !x.IsDeleted && x.PersonCustomer.Name.Contains(txtSearch)).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, EmployeeName = x.Employee.Person.Name, CustomerName = x.PersonCustomer.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", SafeName = x.Safe.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.CustomerPayments.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                {
                    list = list.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo.Date);

                    return Json(new
                    {
                        data = list.Select(x => new { Id = x.Id, EmployeeName = x.Employee.Person.Name, CustomerName = x.PersonCustomer.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", SafeName = x.Safe.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        data = list.Select(x => new { Id = x.Id, EmployeeName = x.Employee.Person.Name, CustomerName = x.PersonCustomer.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", SafeName = x.Safe.Name, Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
            }
           
        }
        [HttpGet]
        public ActionResult CreateEdit(string invoGuid)// فى حالة الانتقال من شاشة (انشاء اقساط) الى سداد نقدية لعميل
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==id);
                    Guid? departmentId = null;
                    Guid? empId = null;
                    if (model.BySaleMen && model.Employee != null)
                    {
                        departmentId = model.Employee.DepartmentId;
                        empId = model.EmployeeId;
                        var list = EmployeeService.GetSaleMens(departmentId);
                        ViewBag.EmployeeId = new SelectList(list, "Id", "Name", empId);

                    }
                    else
                        ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                    //ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == departmentId).Select(x => new { Id = x.Id, Name = x.Person.Name }), "Id", "Name", empId);

                    var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departmentId);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", model.PersonCustomer.PersonCategoryId);
                    ViewBag.CustomerId = new SelectList(EmployeeService.GetCustomerByCategory(model.PersonCustomer.PersonCategoryId, null,null), "Id", "Name", model.CustomerId);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == model.BranchId), "Id", "Name", model.SafeId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                Guid? invoId = null; Guid? custId = null;
                if (Guid.TryParse(invoGuid, out Guid invGuid))
                {
                    var sellInvo = db.SellInvoices.Where(x => x.Id == invGuid).FirstOrDefault();
                    if (sellInvo != null)
                    {
                        invoId = sellInvo.Id;
                        custId = sellInvo.CustomerId;
                    }
                }
                //ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name",custId);
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                var safes = db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId);

                ViewBag.BranchId = new SelectList(branches, "Id", "Name",branchId);
                ViewBag.SafeId = new SelectList(safes, "Id", "Name", safes.FirstOrDefault()?.Id);
                ViewBag.LastRow = db.CustomerPayments.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new CustomerPayment() { PaymentDate = Utility.GetDateTime(), SellInvoiceId = invoId });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(CustomerPayment vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.CustomerId == null || vm.Amount == 0 || vm.BranchId == null || vm.PaymentDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد بشكل صحيح" });
                //فى حالة ادخال رقم فاتورة توريد يجب التأكد من صحة الرقم المدخلو وانه من ضمن فواتير البيع 
                if (vm.SellInvoiceId != null)
                {
                    if (int.TryParse(vm.SellInvoiceId.ToString(), out var id))
                    {
                        var sellIsExists = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == vm.SellInvoiceId).Count();
                        if (sellIsExists == 0)
                            return Json(new { isValid = false, message = "خطأ .... رقم الفاتورة غير موجود فى فواتير البيع" });
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال رقم فاتورة البيع بشكل صحيح " });

                }
                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //if (db.CustomerPayments.Where(x => !x.IsDeleted && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==vm.Id);
                    model.CustomerId = vm.CustomerId;
                    model.Amount = vm.Amount;
                    model.Notes = vm.Notes;
                    model.BySaleMen = vm.BySaleMen;
                    model.SafeId = vm.SafeId;
                    model.BranchId = vm.BranchId;
                    model.PaymentDate = vm.PaymentDate;
                    //فى حالة ان البيع من خلال مندوب
                    if (vm.BySaleMen)
                    {
                        if (vm.EmployeeId == null)
                            return Json(new { isValid = false, message = "تأكد من اختيار المندوب اولا" });
                        else
                            model.EmployeeId = vm.EmployeeId;
                    }
                    else
                        model.EmployeeId = null;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //if (db.CustomerPayments.Where(x => !x.IsDeleted && x.SupplierId == vm.SupplierId).Count() > 0) ///??
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                    isInsert = true;
                    var model = new CustomerPayment
                    {
                        BranchId = vm.BranchId,
                        SafeId = vm.SafeId,
                        CustomerId = vm.CustomerId,
                        Amount = vm.Amount,
                        Notes = vm.Notes,
                        PaymentDate = vm.PaymentDate,
                        SellInvoiceId = vm.SellInvoiceId,
                        BySaleMen=vm.BySaleMen
                    };
                    //فى حالة ان البيع من خلال مندوب
                    if (vm.BySaleMen)
                    {
                        if (vm.EmployeeId == null)
                            return Json(new { isValid = false, message = "تأكد من اختيار المندوب اولا" });
                        else
                            model.EmployeeId = vm.EmployeeId;
                    }
                    else
                        model.EmployeeId = null;
                    db.CustomerPayments.Add(model);

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
                var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==Id);
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

                    var paymentCustomer = db.CustomerPayments.Where(x => x.Id == Id).FirstOrDefault();
                    //التأكد من اختيار طريقة السداد فى حالة ان الفاتورة لمندوب 
                    if (paymentCustomer.BySaleMen && paymentCustomer.SafeId == null )
                        return Json(new { isValid = false, message = "تأكد من تحديد الخزينة للعملية اولا" });

                    //التأكد من عدم وجود حساب فرعى من حساب العميل
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(paymentCustomer.PersonCustomer.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });
                        //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(paymentCustomer.Safe.AccountsTreeId))
                            return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                    //التاكد من القيد لم يتم تسجيله مسبقا 
                    //if (db.GeneralDailies.Where(x=>!x.IsDeleted&&x.TransactionId==paymentCustomer.Id&&x.TransactionTypeId== (int)TransactionsTypesCl.CashIn).Any())
                    //    return Json(new { isValid = true, message = "تم اعتماد عملية استلام النقدية بنجاح" });
                    //التأكد من عدم تكرار اعتماد القيد
                    if (GeneralDailyService.GeneralDailaiyExists(paymentCustomer.Id, (int)TransactionsTypesCl.CashIn))
                        return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });
                    Guid? transactionShared = null;
                    if (paymentCustomer.SellInvoice != null)
                        transactionShared = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == paymentCustomer.SellInvoiceId).FirstOrDefault().Id;


                    // حساب الخزينة
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = paymentCustomer.Safe.AccountsTreeId,
                        Debit = paymentCustomer.Amount,
                        BranchId = paymentCustomer.BranchId,
                        Notes = $"تحصيل نقدية من {paymentCustomer.PersonCustomer.Name}  " + paymentCustomer.Notes,
                        TransactionDate = paymentCustomer.PaymentDate,
                        TransactionId = paymentCustomer.Id,
                        TransactionShared = transactionShared,
                        TransactionTypeId = (int)TransactionsTypesCl.CashIn
                    });


                    //العميل 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = paymentCustomer.PersonCustomer.AccountsTreeCustomerId,
                        Credit = paymentCustomer.Amount,
                        Notes =$"تحصيل نقدية من {paymentCustomer.PersonCustomer.Name}  "+ paymentCustomer.Notes,
                        BranchId = paymentCustomer.BranchId,
                        TransactionDate = paymentCustomer.PaymentDate,
                        TransactionId = paymentCustomer.Id,
                        TransactionShared = transactionShared,
                        TransactionTypeId = (int)TransactionsTypesCl.CashIn
                    });

                    //تحديث حالة الاعتماد 
                    paymentCustomer.IsApproval = true;
                    db.Entry(paymentCustomer).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد عملية استلام النقدية بنجاح" });
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
                var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.CashIn).ToList();
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