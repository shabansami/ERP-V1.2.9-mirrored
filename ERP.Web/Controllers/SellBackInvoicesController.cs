using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellBackInvoicesController : Controller
    {
        // GET: SellBackInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public SellBackInvoicesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة فواتير مرتجع التوريد
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll(string dFrom, string dTo)
        {
            int? n = null;
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.SellBackInvoices.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList().Where(x => !x.IsDeleted && (x.Id.ToString() == txtSearch || x.PersonCustomer.Name.Contains(txtSearch) || x.Employee.Person.Name.Contains(txtSearch))).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.SellBackInvoice, IsFinalApproval = x.IsFinalApproval, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.SellBackInvoices.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                    list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);
                var branchesList = list.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList();

                return Json(new
                    {
                        data = branchesList.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.SellBackInvoice, IsFinalApproval = x.IsFinalApproval, Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region wizard step 2 اضافة اصناف الفاتورة
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new ItemDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && vm.ProductionOrderId == x.ProductionOrderId && vm.IsIntial == x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف او السيريال موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.SerialItemId != null && vm.SerialItemId != Guid.Empty)
            {
                if (vm.Quantity > 1)
                    return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من 1 " }, JsonRequestBehavior.AllowGet);
            }
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = vm.ItemDiscount, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل مصروفات الفاتورة
        public ActionResult GetDStSellBackInvoiceExpenses()
        {
            int? n = null;
            if (DSExpenses == null)
                return Json(new
                {
                    data = new InvoiceExpensesDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DSExpenses)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSellBackInvoiceExpenses(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            string expenseTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "الايراد المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                expenseTypeName = db.AccountsTrees.FirstOrDefault(x => x.Id == vm.ExpenseTypeId).AccountName;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseTypeId))
                    return Json(new { isValid = false, msg = "حساب الايراد ليس بحساب فرعى" });

                //if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                //    return Json(new { isValid = false, msg = "الايراد المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                //expenseTypeName = db.IncomeTypes.FirstOrDefault(x=>x.Id==vm.ExpenseTypeId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار ايراد " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل الايراد بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل فاتورة توريد وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name");

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.SellBackInvoices.Where(x => x.Id == guId).FirstOrDefault();

                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoiceId == vm.Id).Select(item => new
                                  ItemDetailsDT
                    {
                        Amount = item.Amount,
                        ItemId = item.ItemId,
                        ItemDiscount = item.ItemDiscount,
                        ItemName = item.Item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId,
                        StoreName = item.Store.Name,
                        SerialItemId = item.ItemSerialId,
                        //IsIntial = item.IsItemIntial ? 1 : 0,
                        //ProductionOrderId = item.ProductionOrderId

                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);
                    var expenses = db.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == vm.Id).Select(expense => new
                                 InvoiceExpensesDT
                    {
                        ExpenseAmount = expense.Amount,
                        ExpenseTypeId = expense.IncomeTypeAccountTreeId,
                        ExpenseTypeName = expense.IncomeTypeAccountTree.AccountName
                    }).ToList();
                    DSExpenses = JsonConvert.SerializeObject(expenses);

                    ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId && !x.IsDamages), "Id", "Name");
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
                    ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
                    //ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen && x.PersonId == vm.SaleMenId).Select(x => new { Id = x.PersonId, Name = x.Person.Name }), "Id", "Name", vm.SaleMenId);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
                    //ViewBag.InvoiceNum = vm.InvoiceNum;
                    Guid? departmentId = null;
                    Guid? empId = null;
                    if (vm.BySaleMen && vm.Employee != null)
                    {
                        departmentId = vm.Employee.DepartmentId;
                        empId = vm.EmployeeId;
                        var list = EmployeeService.GetSaleMens(departmentId);
                        ViewBag.EmployeeId = new SelectList(list, "Id", "Name", empId);

                    }
                    else
                        ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                    //ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == departmentId).Select(x => new { Id = x.Id, Name = x.Person.Name }), "Id", "Name", empId);

                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departmentId);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", vm.PersonCustomer.PersonCategoryId);
                    ViewBag.CustomerId = new SelectList(EmployeeService.GetCustomerByCategory(vm.PersonCustomer.PersonCategoryId, empId,vm.CustomerId), "Id", "Name", vm.CustomerId);


                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                DS = JsonConvert.SerializeObject(new List<ItemDetailsDT>());
                DSExpenses = JsonConvert.SerializeObject(new List<InvoiceExpensesDT>());

                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                //var safeId = db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId)?.FirstOrDefault().Id;
                var bankAccountId = db.BankAccounts.Where(x => !x.IsDeleted)?.FirstOrDefault().Id;

                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", defaultStore?.Id);
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name");
                //ViewBag.SaleMenId = new SelectList(new List<Person>(), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", bankAccountId);
                Random random = new Random();
                //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

                var vm = new SellBackInvoice();
                vm.InvoiceDate = Utility.GetDateTime();
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SellBackInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    if (vm.PaymentTypeId == (int)PaymentTypeCl.Deferred) //ف حالة السداد آجل
                    {
                        if (vm.PayedValue > 0)
                            return Json(new { isValid = false, message = "تم استلام مبلغ من العميل وحالة السداد آجل " });

                        vm.BankAccountId = null;
                        vm.SafeId = null;
                    }
                    else  // ف حالة السداد نقدى او جزئى
                    {
                        if (vm.BankAccountId == null && vm.SafeId == null)
                            return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" });
                        if (vm.PayedValue == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال المبلغ المدفوع بشكل صحيح" });
                    }


                    //الاصناف
                    List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                    List<SellBackInvoicesDetail> items = new List<SellBackInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new SellBackInvoicesDetail
                              {
                                  SellBackInvoiceId = vm.Id,
                                  ItemId = x.ItemId,
                                  StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  Price = x.Price,
                                  Amount = x.Amount,
                                  ItemDiscount = x.ItemDiscount,
                                  ItemSerialId = x.SerialItemId,
                                  //ProductionOrderId = x.ProductionOrderId,
                                  //IsItemIntial = x.IsIntial == 1 ? true : false
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                    //الايرادات
                    List<InvoiceExpensesDT> invoiceExpensesDT = new List<InvoiceExpensesDT>();
                    List<SellBackInvoiceIncome> invoicesExpens = new List<SellBackInvoiceIncome>();

                    if (DT_DatasourceExpenses != null)
                    {
                        invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        invoicesExpens = invoiceExpensesDT.Select(
                            x => new SellBackInvoiceIncome
                            {
                                SellBackInvoiceId = vm.Id,
                                IncomeTypeAccountTreeId = x.ExpenseTypeId,
                                Amount = x.ExpenseAmount
                            }
                            ).ToList();
                    }

                    var isInsert = false;
                    //Transaction
                    using (var context = new VTSaleEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                SellBackInvoice model = null;
                                if (vm.Id != Guid.Empty)
                                {
                                    model = context.SellBackInvoices.FirstOrDefault(x => x.Id == vm.Id);
                                    if (model.InvoiceDate.ToShortDateString() != vm.InvoiceDate.ToShortDateString())
                                        model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                }
                                else
                                {
                                    model = vm;
                                    model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                }

                                //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                                model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                                model.TotalDiscount = itemDetailsDT.Sum(x => x.ItemDiscount) + vm.InvoiceDiscount;//إجمالي خصومات الفاتورة 
                                model.TotalValue = itemDetailsDT.Sum(x => x.Amount);//إجمالي قيمة المشتريات 
                                model.TotalExpenses = invoiceExpensesDT.Sum(x => x.ExpenseAmount);//إجمالي قيمة المصروفات
                                model.Safy = (model.TotalValue + model.TotalExpenses + vm.SalesTax) - (model.TotalDiscount + vm.ProfitTax);

                                //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                                if (vm.PaymentTypeId == (int)PaymentTypeCl.Cash && vm.PayedValue != model.Safy)
                                    return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                                if (vm.Id != Guid.Empty)
                                {
                                    //delete all privous items 
                                    var privousItems = context.SellBackInvoicesDetails.Where(x => x.SellBackInvoiceId == vm.Id).ToList();
                                    foreach (var item in privousItems)
                                    {
                                        item.IsDeleted = true;
                                        context.Entry(item).State = EntityState.Modified;
                                    }
                                    //delete all privous expenses
                                    var privousExpenese = context.SellBackInvoiceIncomes.Where(x => x.SellBackInvoiceId == vm.Id).ToList();
                                    foreach (var expense in privousExpenese)
                                    {
                                        expense.IsDeleted = true;
                                        context.Entry(expense).State = EntityState.Modified;
                                    }

                                    //فى حالة تم الاعتماد النهائى للفاتورة ثم مطلوب تعديلها 
                                    if (model.CaseId == (int)CasesCl.BackInvoiceFinalApproval)
                                    {
                                        var generalDalies = context.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.SellReturn).ToList();
                                        // حذف كل القيود 
                                        foreach (var generalDay in generalDalies)
                                        {
                                            generalDay.IsDeleted = true;
                                            context.Entry(generalDay).State = EntityState.Modified;
                                        }

                                    }
                                    //اعادة فتح الفاتورة للاعتماد المحاسبى و المخزنى
                                    model.IsApprovalAccountant = false;
                                    model.IsApprovalStore = false;
                                    model.IsFinalApproval = false;

                                    // Update purchase invoice
                                    model.SellInvoiceId = vm.SellInvoiceId != null && vm.SellInvoiceId != Guid.Empty ? vm.SellInvoiceId : null;
                                    model.CustomerId = vm.CustomerId;
                                    model.BranchId = vm.BranchId;
                                    model.PaymentTypeId = vm.PaymentTypeId;
                                    model.SafeId = vm.SafeId;
                                    model.BankAccountId = vm.BankAccountId;
                                    model.PayedValue = vm.PayedValue;
                                    model.RemindValue = vm.RemindValue;
                                    model.SalesTax = vm.SalesTax;
                                    model.ProfitTax = vm.ProfitTax;
                                    model.InvoiceDiscount = vm.InvoiceDiscount;
                                    //model.ShippingAddress = vm.ShippingAddress;
                                    model.DueDate = vm.RemindValue > 0 ? vm.DueDate : null;
                                    model.Notes = vm.Notes;
                                    model.CaseId = (int)CasesCl.BackInvoiceModified;
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

                                    context.Entry(model).State = EntityState.Modified;
                                    context.SellBackInvoicesDetails.AddRange(items);
                                    context.SellBackInvoiceIncomes.AddRange(invoicesExpens);
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellBackInvoice = model,
                                        IsSellInvoice = false,
                                        CaseId = (int)CasesCl.BackInvoiceModified
                                    });

                                }
                                else
                                {

                                    isInsert = true;
                                    //اضافة رقم الفاتورة
                                    string codePrefix = Properties.Settings.Default.CodePrefix;
                                    model.InvoiceNumber = codePrefix + (db.SellBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                                    model.CaseId = (int)CasesCl.BackInvoiceCreated;
                                    model.SellBackInvoicesDetails = items;
                                    model.SellBackInvoiceIncomes = invoicesExpens;
                                    model.DueDate = vm.RemindValue > 0 ? vm.DueDate : null;
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


                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellBackInvoice = model,
                                        IsSellInvoice = false,
                                        CaseId = (int)CasesCl.BackInvoiceCreated
                                    });
                                    context.SellBackInvoices.Add(model);
                                }
                                //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
                                var approvalAfterSave = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
                                if (approvalAfterSave == "1")
                                {
                                    //الاعتماد المحاسبى 
                                    model.IsApprovalAccountant = true;
                                    model.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellBackInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalAccountant
                                    });
                                    //الاعتماد المخزنى
                                    model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                                    model.IsApprovalStore = true;
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellBackInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalStore
                                    });
                                }
                                context.SaveChanges(auth.CookieValues.UserId);
                                //الاعتماد النهائى 
                                if (approvalAfterSave == "1")
                                {
                                    //    //تسجيل القيود
                                    // General Dailies
                                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                    {
                                        //صافى قيمة الفاتورة= ( إجمالي قيمة مرتجع المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                                        double debit = 0;
                                        double credit = 0;
                                        // الحصول على حسابات من الاعدادات
                                        var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                                        //التأكد من عدم وجود حساب فرعى من الحساب
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب مرتجع المبيعات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(context.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId))
                                            return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                                        var expenses = context.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == model.Id);
                                        var customer = context.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault();

                                        if (expenses.Count() > 0)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                                                return Json(new { isValid = false, message = "حساب الايراد ليس بحساب فرعى" });
                                        }

                                        //تحديد نوع الجرد
                                        //var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                        //Guid accountTreeInventoryType;
                                        //if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                        //{
                                        //    //فى حالة الجرد المستمر
                                        //    //من ح/ المخزون
                                        //    accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                                        //    context.GeneralDailies.Add(new GeneralDaily
                                        //    {
                                        //        AccountsTreeId = accountTreeInventoryType,
                                        //        BranchId = model.BranchId,
                                        //        Debit = model.TotalValue,
                                        //        Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                        //        TransactionDate = model.InvoiceDate,
                                        //        TransactionId = model.Id,
                                        //        TransactionShared = model.Id,
                                        //        TransactionTypeId = (int)TransactionsTypesCl.Sell
                                        //    });
                                        //    // الى ح/  تكلفة بضاعه مباعه
                                        //    context.GeneralDailies.Add(new GeneralDaily
                                        //    {
                                        //        AccountsTreeId = Lookups.StockSellCost,
                                        //        BranchId = model.BranchId,
                                        //        Credit = model.TotalValue,
                                        //        Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                        //        TransactionDate = model.InvoiceDate,
                                        //        TransactionId = model.Id,
                                        //        TransactionShared = model.Id,
                                        //        TransactionTypeId = (int)TransactionsTypesCl.Sell
                                        //    });
                                        //}
                                        //else
                                        //    return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                                        ////============================================

                                        // حساب مرتجع المبيعات
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue),
                                            BranchId = model.BranchId,
                                            Debit = model.TotalValue,
                                            Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                        });
                                        debit = debit + model.TotalValue;
                                        // حساب العميل
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = customer.AccountsTreeCustomerId,
                                            BranchId = model.BranchId,
                                            Credit = model.Safy,
                                            Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                        });
                                        credit = credit + model.Safy;

                                        // القيمة المضافة
                                        if (model.SalesTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.SalesTax,
                                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                            debit = debit + model.SalesTax;


                                        }
                                        // الخصومات
                                        if (model.TotalDiscount > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.TotalDiscount,
                                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                            credit = credit + model.TotalDiscount;
                                        }

                                        // ضريبة ارباح تجارية
                                        if (model.ProfitTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.ProfitTax,
                                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                            credit = credit + model.ProfitTax;
                                        }

                                        //  المبلغ المدفوع من حساب العميل (مدين
                                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = context.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId,
                                                BranchId = model.BranchId,
                                                Debit = model.PayedValue,
                                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                            debit = debit + model.PayedValue;

                                            //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
                                            Guid? safeAccouyBank = null;
                                            if (model.SafeId != null)
                                                safeAccouyBank = context.Safes.FirstOrDefault(x => x.Id == model.SafeId).AccountsTreeId;
                                            else if (model.BankAccountId != null)
                                                safeAccouyBank = context.BankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId).AccountsTreeId;

                                            if (safeAccouyBank != null)
                                            {
                                                context.GeneralDailies.Add(new GeneralDaily
                                                {
                                                    AccountsTreeId = safeAccouyBank,
                                                    BranchId = model.BranchId,
                                                    Credit = model.PayedValue,
                                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                    TransactionDate = model.InvoiceDate,
                                                    TransactionId = model.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                                });
                                                credit = credit + model.PayedValue;
                                            }


                                        }

                                        // الايرادات (مدين
                                        foreach (var expense in expenses)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = expense.IncomeTypeAccountTreeId,
                                                BranchId = model.BranchId,
                                                Debit = expense.Amount,
                                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                            debit = debit + expense.Amount;

                                        }
                                        //التاكد من توازن المعاملة 
                                        if (debit == credit)
                                        {

                                            //حفظ القيود 
                                            // اعتماد النهائى للفاتورة
                                            model.CaseId = (int)CasesCl.BackInvoiceFinalApproval;
                                            model.IsFinalApproval = true;
                                            context.Entry(model).State = EntityState.Modified;
                                            //اضافة الحالة 
                                            context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                            {
                                                SellBackInvoice = model,
                                                IsSellInvoice = false,
                                                CaseId = (int)CasesCl.BackInvoiceFinalApproval
                                            });

                                            //add new row in history serial item 
                                            foreach (var item in model.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                            {
                                                if (item.ItemSerialId != null && item.ItemSerialId != Guid.Empty)
                                                {
                                                    //update item serial case history
                                                    var serial = context.ItemSerials.FirstOrDefault(x => x.Id == item.ItemSerialId);
                                                    if (serial != null)
                                                    {
                                                        serial.SerialCaseId = (int)SerialCaseCl.SellBack;
                                                        //تحديث حالة حقل فاتورة البيع للاصناف فى جدول السيريال بنل حتى يمكن بيعه مرة اخرى 
                                                        serial.SellInvoiceId = null;
                                                        serial.CurrentStoreId = item.StoreId;
                                                        context.Entry(serial).State = EntityState.Modified;
                                                        //add item serial case history
                                                        context.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                                        {
                                                            ItemSerialId = serial.Id,
                                                            ReferrenceId = model.Id,
                                                            SerialCaseId = (int)SerialCaseCl.SellBack
                                                        });
                                                    }
                                                }
                                            }

                                            context.SaveChanges(auth.CookieValues.UserId);
                                        }
                                        else
                                        {
                                            return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                                        }

                                    }
                                    else
                                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                                }
                                transaction.Commit();
                                if (isInsert)
                                {
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.SellBackInvoice, refGid = model.Id, isInsert, message = "تم الاضافة بنجاح" });

                                }
                                else
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.SellBackInvoice, refGid = model.Id, isInsert, message = "تم التعديل بنجاح" });



                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
                            }
                        }
                    }
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });


            }
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        public ActionResult Edit(string invoGuid)
        {
            Guid GuId;

            if (!Guid.TryParse(invoGuid, out GuId) || string.IsNullOrEmpty(invoGuid) || invoGuid == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = GuId;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.SellBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.BackInvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.SellBackInvoicesDetails.Where(x => x.SellBackInvoiceId == model.Id && !x.IsDeleted).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        // حذف كل حالات الاصناف فى جدول itemSerial history ان وجدت 
                        //تحديث رقم فاتورة البيع للسيريال بسبب انه تم حذفه اثناء عملية المرتجع
                        var itemSerial = detail.ItemSerial;
                        if (itemSerial != null)
                        {
                            var invoiceId = detail.ItemSerial.CasesItemSerialHistories.Where(x => !x.IsDeleted && x.SerialCaseId == (int)SerialCaseCl.Sell).OrderByDescending(x => x.Id).FirstOrDefault().ReferrenceId;
                            if (invoiceId != Guid.Empty)
                            {
                                itemSerial.SellInvoiceId = invoiceId;
                                itemSerial.CurrentStoreId = null;
                                itemSerial.SerialCaseId = (int)SerialCaseCl.Sell;
                                db.Entry(itemSerial).State = EntityState.Modified;
                            }
                            //foreach (var itemSerialsHistory in itemSerial.CasesItemSerialHistories.Where(x => !x.IsDeleted))
                            //{
                            //    itemSerialsHistory.IsDeleted = true;
                            //    db.Entry(itemSerialsHistory).State = EntityState.Modified;

                            //}
                        }


                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                    }

                    var expenses = db.SellBackInvoiceIncomes.Where(x => x.SellBackInvoiceId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    var casesSellInvoiceHistories = db.CasesSellInvoiceHistories.Where(x => x.SellBackInvoiceId == model.Id).ToList();
                    foreach (var cases in casesSellInvoiceHistories)
                    {
                        cases.IsDeleted = true;
                        db.Entry(cases).State = EntityState.Modified;
                    }
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.SellReturn).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
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
        #region فك الاعتماد 
        [HttpPost]
        public ActionResult UnApproval(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.SellBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsFinalApproval = false;
                    model.IsApprovalStore = false;
                    model.IsApprovalAccountant = false;
                    model.CaseId = (int)CasesCl.InvoiceUnApproval;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellBackInvoice = model,
                        IsSellInvoice = false,
                        CaseId = (int)CasesCl.InvoiceUnApproval
                    });

                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.SellReturn).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
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

        #endregion


        #region عرض بيانات فاتورة توريد بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowSellBackInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellBackInvoices.Where(x => x.Id == invoGuid && x.SellBackInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.SellBackInvoicesDetails = vm.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            vm.SellBackInvoiceIncomes = vm.SellBackInvoiceIncomes.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellBackInvoices.Where(x => x.Id == invoGuid && x.CasesSellInvoiceHistories.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.CasesSellInvoiceHistories = vm.CasesSellInvoiceHistories.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        #endregion


        #region الاعتماد النهائى للفاتورة 
        public ActionResult GetFinalApproval()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellBackInvoices.Where(x => !x.IsDeleted && x.IsApprovalStore && x.IsApprovalAccountant).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, CaseId = x.CaseId, CaseName = x.Case != null ? x.Case.Name : "", IsFinalApproval = x.IsFinalApproval, FinalApproval = x.IsFinalApproval ? "معتمده نهائيا" : "غير معتمده", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ApprovalFinalInvoice()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApprovalFinal(string invoGuid)
        {
            try
            {
                Guid Id;
                if (Guid.TryParse(invoGuid, out Id))
                {
                    var model = db.SellBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        //    //تسجيل القيود
                        // General Dailies
                        if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                        {
                            //صافى قيمة الفاتورة= ( إجمالي قيمة مرتجع المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                            double debit = 0;
                            double credit = 0;
                            // الحصول على حسابات من الاعدادات
                            var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                            //التأكد من عدم وجود حساب فرعى من الحساب
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب مرتجع المبيعات ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId))
                                return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                            var expenses = db.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == model.Id);
                            var customer = db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault();

                            if (expenses.Count() > 0)
                            {
                                if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                                    return Json(new { isValid = false, message = "حساب الايراد ليس بحساب فرعى" });
                            }

                            // حساب مرتجع المبيعات
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue),
                                BranchId = model.BranchId,
                                Debit = model.TotalValue,
                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                            });
                            debit = debit + model.TotalValue;
                            // حساب العميل
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                BranchId = model.BranchId,
                                Credit = model.Safy,
                                Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                            });
                            credit = credit + model.Safy;

                            // القيمة المضافة
                            if (model.SalesTax > 0)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                    BranchId = model.BranchId,
                                    Debit = model.SalesTax,
                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });
                                debit = debit + model.SalesTax;


                            }
                            // الخصومات
                            if (model.TotalDiscount > 0)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                    BranchId = model.BranchId,
                                    Credit = model.TotalDiscount,
                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });
                                credit = credit + model.TotalDiscount;
                            }

                            // ضريبة ارباح تجارية
                            if (model.ProfitTax > 0)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                                    BranchId = model.BranchId,
                                    Credit = model.ProfitTax,
                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });
                                credit = credit + model.ProfitTax;
                            }

                            //  المبلغ المدفوع من حساب العميل (مدين
                            if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId,
                                    BranchId = model.BranchId,
                                    Debit = model.PayedValue,
                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });
                                debit = debit + model.PayedValue;

                                //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
                                Guid? safeAccouyBank = null;
                                if (model.SafeId != null)
                                    safeAccouyBank = db.Safes.FirstOrDefault(x => x.Id == model.SafeId).AccountsTreeId;
                                else if (model.BankAccountId != null)
                                    safeAccouyBank = db.BankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId).AccountsTreeId;

                                if (safeAccouyBank != null)
                                {
                                    db.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = safeAccouyBank,
                                        BranchId = model.BranchId,
                                        Credit = model.PayedValue,
                                        Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                        TransactionDate = model.InvoiceDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                    });
                                    credit = credit + model.PayedValue;
                                }


                            }

                            // الايرادات (مدين
                            foreach (var expense in expenses)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = expense.IncomeTypeAccountTreeId,
                                    BranchId = model.BranchId,
                                    Debit = expense.Amount,
                                    Notes = $"فاتورة مرتجع بيع رقم : {model.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });
                                debit = debit + expense.Amount;

                            }
                            //التاكد من توازن المعاملة 
                            if (debit == credit)
                            {

                                //حفظ القيود 
                                // اعتماد النهائى للفاتورة
                                model.CaseId = (int)CasesCl.BackInvoiceFinalApproval;
                                model.IsFinalApproval = true;
                                db.Entry(model).State = EntityState.Modified;
                                //اضافة الحالة 
                                db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                {
                                    SellBackInvoice = model,
                                    IsSellInvoice = false,
                                    CaseId = (int)CasesCl.BackInvoiceFinalApproval
                                });

                                //add new row in history serial item 
                                foreach (var item in model.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                {
                                    if (item.ItemSerialId != null && item.ItemSerialId != Guid.Empty)
                                    {
                                        //update item serial case history
                                        var serial = db.ItemSerials.FirstOrDefault(x => x.Id == item.ItemSerialId);
                                        if (serial != null)
                                        {
                                            serial.SerialCaseId = (int)SerialCaseCl.SellBack;
                                            //تحديث حالة حقل فاتورة البيع للاصناف فى جدول السيريال بنل حتى يمكن بيعه مرة اخرى 
                                            serial.SellInvoiceId = null;
                                            serial.CurrentStoreId = item.StoreId;
                                            db.Entry(serial).State = EntityState.Modified;
                                            //add item serial case history
                                            db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                            {
                                                ItemSerialId = serial.Id,
                                                ReferrenceId = model.Id,
                                                SerialCaseId = (int)SerialCaseCl.SellBack
                                            });
                                        }
                                    }
                                }

                                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                                {
                                    return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                                }
                                else
                                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                            }
                            else
                            {
                                return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                            }

                        }
                        else
                            return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
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