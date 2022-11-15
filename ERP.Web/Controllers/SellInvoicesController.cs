using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Web.UI.WebControls;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoicesController : Controller
    {
        // GET: SellInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        ItemService itemService;
        public SellInvoicesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            itemService = new ItemService();
        }
        public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة فواتير البيع
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll(string dFrom, string dTo)
        {
            int? n = null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.SellInvoices.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList().Where(x => !x.IsDeleted && (x.Id.ToString() == txtSearch || x.PersonCustomer.Name.Contains(txtSearch) )).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNumPaper = x.InvoiceNumPaper, PaymentTypeName = x.PaymentType.Name, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.SellInvoice, IsFinalApproval=x.IsFinalApproval, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.SellInvoices.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                    list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);

                var branchesList = list.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList();
                    return Json(new
                    {
                        data = branchesList.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNumPaper = x.InvoiceNumPaper,  InvoiceNum = x.InvoiceNumber, PaymentTypeName = x.PaymentType.Name, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.SellInvoice, IsFinalApproval = x.IsFinalApproval, Actions = n, Num = n }).ToList()
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
            if (!bool.TryParse(vm.IsDiscountItemVal.ToString(), out var tt))
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);

            if (vm.ItemId != null)
            {
                //if (deDS.Where(x => x.ItemId == vm.ItemId&&x.Price==vm.Price&&vm.ProductionOrderId==x.ProductionOrderId&&vm.IsIntial==x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
                //    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
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
            //فى حالة البيع بوحدة 
            if (vm.ItemUnitsId != null && vm.ItemUnitsId != Guid.Empty)
            {
                var itemUnit = db.ItemUnits.Where(x => x.Id == vm.ItemUnitsId).FirstOrDefault();
                if (itemUnit != null&&itemUnit.Quantity>0)
                {
                    vm.Quantity = vm.Quantity * itemUnit.Quantity;
                    vm.Price = Math.Round(itemUnit.SellPrice / itemUnit.Quantity, 2, MidpointRounding.ToEven);
                }
            }
            //احتساب طريقة الخصم على الصنف (قيمة/نسبة)
            double itemDiscount = 0;
            if (vm.IsDiscountItemVal == true)
                itemDiscount = vm.ItemDiscount;
            else if (vm.IsDiscountItemVal == false)
                itemDiscount = ((vm.Price * vm.Quantity) * vm.ItemDiscount) / 100;
            else
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);


            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemUnitsId = vm.ItemUnitsId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = Math.Round(vm.Quantity * vm.Price, 2, MidpointRounding.ToEven), ItemDiscount = itemDiscount, IsDiscountItemVal = vm.IsDiscountItemVal, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), itemDiscount = itemDiscount, totalQuantity = deDS.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل ايرادات الفاتورة
        public ActionResult GetDStSellInvoiceExpenses()
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

        public ActionResult AddSellInvoiceExpenses(InvoiceExpensesDT vm)
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
                return Json(new { isValid = false, msg = "تأكد من اختيار إيراد " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل الإيراد بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل فاتورة بيع وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit(string shwTab)
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemUnitsId = new SelectList(new List<ItemUnit>(), "Id", "Name");

            if (shwTab != null && shwTab == "1")
                ViewBag.ShowTab = true;
            else
                ViewBag.ShowTab = false;

            //قبول اضافة صنف بدون رصيد
            int itemAcceptNoBalance = 0;
            var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
            if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                ViewBag.ItemAcceptNoBalance = itemAcceptNoBalance;

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.SellInvoices.Where(x => x.Id == guId).FirstOrDefault();

                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).Select(item => new
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
                        IsIntial = item.IsItemIntial ? 1 : 0,
                        ProductionOrderId = item.ProductionOrderId,
                        SerialItemId = item.ItemSerialId
                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);
                    var expenses = db.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).Select(expense => new
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
                    //ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted&&x.IsSaleMen&&x.PersonId==vm.SaleMenId).Select(x=>new {Id=x.PersonId,Name=x.Person.Name }), "Id", "Name", vm.SaleMenId);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);

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
                ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
                //ViewBag.SaleMenId = new SelectList(new List<Person>(), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", bankAccountId);
                Random random = new Random();
                //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

                var vm = new SellInvoice();
                vm.InvoiceDate = Utility.GetDateTime();
                vm.DueDate = Utility.GetDateTime().AddMonths(1);
                //اظهار تكلفة الصنف فى شاشة البيع
                int itemCostCalculateShowInSellReg = 0;
                var costCalculateShowInSellRegSet = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateShowInSellReg).FirstOrDefault();
                if (!string.IsNullOrEmpty(costCalculateShowInSellRegSet.SValue))
                    int.TryParse(costCalculateShowInSellRegSet.SValue, out itemCostCalculateShowInSellReg);
                ViewBag.ItemCostCalculateShowInSellRegId = itemCostCalculateShowInSellReg;
                ViewBag.ItemCostCalculateId = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SellInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses, bool? isInvoiceDisVal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    //فى حالة الخصم على الفاتورة نسية /قيمة 
                    if (!bool.TryParse(isInvoiceDisVal.ToString(), out var t))
                        return Json(new { isValid = false, message = "تأكد من اختيار احتساب الخصم على الفاتورة" });

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
                        if (vm.PayedValue == 0 && vm.PaymentTypeId != (int)PaymentTypeCl.Installment)
                            return Json(new { isValid = false, message = "تأكد من ادخال المبلغ المدفوع بشكل صحيح" });
                    }


                    //الاصناف
                    List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                    List<SellInvoicesDetail> items = new List<SellInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new SellInvoicesDetail
                              {
                                  SellInvoiceId = vm.Id,
                                  ItemId = x.ItemId,
                                  StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  Price = x.Price,
                                  Amount = x.Amount,
                                  ItemDiscount = x.ItemDiscount,
                                  ItemSerialId = x.SerialItemId,
                                  ProductionOrderId = x.ProductionOrderId,
                                  IsItemIntial = x.IsIntial == 1 ? true : false
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                    //الايرادات
                    List<InvoiceExpensesDT> invoiceExpensesDT = new List<InvoiceExpensesDT>();
                    List<SellInvoiceIncome> invoicesExpens = new List<SellInvoiceIncome>();

                    if (DT_DatasourceExpenses != null)
                    {
                        invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        invoicesExpens = invoiceExpensesDT.Select(
                            x => new SellInvoiceIncome
                            {
                                SellInvoiceId = vm.Id,
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
                                SellInvoice model = null;
                                if (vm.Id != Guid.Empty)
                                {
                                    model = context.SellInvoices.FirstOrDefault(x => x.Id == vm.Id);
                                    if (model.InvoiceDate.ToShortDateString() != vm.InvoiceDate.ToShortDateString())
                                        model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                }
                                else
                                {
                                    model = vm;
                                    model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                }

                                //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                                //فى حالة الخصم على الفاتورة نسية /قيمة 
                                double invoiceDiscount = 0;
                                if (isInvoiceDisVal == true)
                                {
                                    invoiceDiscount = vm.InvoiceDiscount;
                                    model.DiscountPercentage = 0;
                                }
                                else if (isInvoiceDisVal == false)
                                {
                                    invoiceDiscount = (itemDetailsDT.Sum(x => x.Amount) * vm.DiscountPercentage) / 100;
                                    model.DiscountPercentage = vm.DiscountPercentage;
                                }

                                model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                                model.TotalDiscount = itemDetailsDT.Sum(x => x.ItemDiscount) + invoiceDiscount;//إجمالي خصومات الفاتورة 
                                model.TotalValue = itemDetailsDT.Sum(x => x.Amount);//إجمالي قيمة المبيعات 
                                model.TotalExpenses = invoiceExpensesDT.Sum(x => x.ExpenseAmount);//إجمالي قيمة الايرادادت
                                model.Safy = (model.TotalValue + model.TotalExpenses + vm.SalesTax) - (model.TotalDiscount + vm.ProfitTax);
                                model.InvoiceNumPaper = vm.InvoiceNumPaper;

                                //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                                if (vm.PaymentTypeId == (int)PaymentTypeCl.Cash && vm.PayedValue != model.Safy)
                                    return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                                //التأكد من السماح البيع للعميل فى حالة تخطى رصيده حد الائتمانى الخطر المسموح به اولا حسب الاعدادات
                                int limitDangerSell = 0;
                                var limitDangerSellSett = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.LimitDangerSell).FirstOrDefault();
                                if (int.TryParse(limitDangerSellSett.SValue, out limitDangerSell))
                                {
                                    if (limitDangerSell == 0)//لايمكن البيع للعميل فى حالة تخطى رصيده حد الائتمانى الخطر المسموح به 
                                    {
                                        var customer = context.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault();
                                        if (customer != null)
                                        {
                                            CustomerService customerService = new CustomerService();
                                            var balance = customerService.GetPersonBalance(customer.AccountsTreeCustomerId);
                                            if (customer.LimitDangerSell != 0)
                                                if (balance + vm.Safy - vm.PayedValue > customer.LimitDangerSell)
                                                    return Json(new { isValid = false, message = "لا يمكن البيع لتخطى الحد الائتمانى للعميل " });
                                        }
                                        else
                                            return Json(new { isValid = false, message = "تأكد من اختيار العميل" });
                                    }
                                }
                                else
                                    return Json(new { isValid = false, message = "تأكد من تحديد حالة السماح بالبيع عند تخطى الحد الائتمانى للخطر فى شاشة الاعدادات" });


                                if (vm.Id != Guid.Empty)
                                {
                                    //delete all privous items 
                                    var privousItems = context.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).ToList();
                                    foreach (var item in privousItems)
                                    {
                                        //تحديث حالة السيريال ان وجد بارتباطه بمخزن البيع
                                        if (item.ItemSerial != null)
                                        {
                                            var serial = item.ItemSerial;
                                            serial.SellInvoiceId = null;
                                            serial.CurrentStoreId = item.StoreId;
                                            context.Entry(serial).State = EntityState.Modified;
                                        }

                                        item.IsDeleted = true;
                                        context.Entry(item).State = EntityState.Modified;

                                    }
                                    //delete all privous expenses
                                    var privousExpenese = context.SellInvoiceIncomes.Where(x => x.SellInvoiceId == vm.Id).ToList();
                                    foreach (var expense in privousExpenese)
                                    {
                                        expense.IsDeleted = true;
                                        context.Entry(expense).State = EntityState.Modified;
                                    }


                                    //فى حالة تم الاعتماد النهائى للفاتورة ثم مطلوب تعديلها 
                                    if (model.CaseId == (int)CasesCl.InvoiceFinalApproval)
                                    {
                                        var generalDalies = context.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
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
                                    model.CustomerId = vm.CustomerId;
                                    model.BranchId = vm.BranchId;
                                    model.PaymentTypeId = vm.PaymentTypeId;
                                    model.SafeId = vm.SafeId;
                                    model.BankAccountId = vm.BankAccountId;
                                    model.PayedValue = vm.PayedValue;
                                    model.RemindValue = vm.RemindValue;
                                    model.SalesTax = vm.SalesTax;
                                    model.ProfitTax = vm.ProfitTax;
                                    model.InvoiceDiscount = invoiceDiscount;
                                    //model.ShippingAddress = vm.ShippingAddress;
                                    model.DueDate =vm.RemindValue>0? vm.DueDate:null;
                                    model.Notes = vm.Notes;
                                    model.CaseId = (int)CasesCl.InvoiceModified;
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
                                    context.SellInvoicesDetails.AddRange(items);
                                    context.SellInvoiceIncomes.AddRange(invoicesExpens);
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceModified
                                    });

                                }
                                else
                                {

                                    isInsert = true;
                                    //اضافة رقم الفاتورة
                                    string codePrefix = Properties.Settings.Default.CodePrefix;
                                    model.InvoiceNumber = codePrefix + (context.SellInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                                    model.CaseId = (int)CasesCl.InvoiceCreated;
                                    model.SellInvoicesDetails = items;
                                    model.SellInvoiceIncomes = invoicesExpens;
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
                                        SellInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceCreated
                                    });

                                    context.SellInvoices.Add(model);
                                }

                                //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
                                var approvalAfterSave = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
                                Stopwatch stopwatch = new Stopwatch();
                                Stopwatch stopwatch1 = new Stopwatch();
                                if (approvalAfterSave == "1")
                                {
                                    stopwatch.Start();
                                    //الاعتماد المحاسبى 
                                    model.IsApprovalAccountant = true;
                                    model.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalAccountant
                                    });
                                    //الاعتماد المخزنى
                                    model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                                    model.IsApprovalStore = true;
                                    //اضافة الحالة 
                                    context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                    {
                                        SellInvoice = model,
                                        IsSellInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalStore
                                    });
                                }
                                context.SaveChanges(auth.CookieValues.UserId);
                                //الاعتماد النهائى 
                                if (approvalAfterSave == "1")
                                {
                                    //التأكد من اختيار طريقة السداد فى حالة ان الفاتورة لمندوب 
                                    if (model.BySaleMen && model.SafeId == null && model.BankAccountId == null && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        return Json(new { isValid = false, message = "تأكد من تحديد حساب الدفع للفاتورة اولا" });

                                    //    //تسجيل القيود
                                    // General Dailies
                                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                    {
                                        //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات
                                        //+ إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                                        double debit = 0;
                                        double credit = 0;
                                        // الحصول على حسابات من الاعدادات
                                        var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                                        //التأكد من عدم وجود حساب فرعى من الحساب
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب المبيعات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(context.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId))
                                            return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                                        var expenses = context.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == model.Id);
                                        var customer = context.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault();

                                        if (expenses.Count() > 0)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                                                return Json(new { isValid = false, message = "حساب الايراد ليس بحساب فرعى" });
                                        }

                                        //فى حالة فاتورة البيع من خلال المندوب التاكد من تحديد طريقة الدفع والسداد(خزنة/بنكى)
                                        if (model.BySaleMen && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        {
                                            if (model.SafeId == null && model.BankAccountId == null)
                                                return Json(new { isValid = false, message = "تأكد من تحديد طريقة السداد (خزينة/بنكى)للفاتورة" });
                                        }
                                        // فى حالة ان الفاتورة كلها عينات وبدون سعر
                                        if (model.TotalValue != 0)
                                        {
                                            ////تحديد نوع الجرد
                                            //var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                            //Guid accountTreeInventoryType;
                                            //if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                            //{
                                            //    //فى حالة الجرد المستمر
                                            //    // من ح/  تكلفة بضاعه مباعه
                                            //    accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                                            //    context.GeneralDailies.Add(new GeneralDaily
                                            //    {
                                            //        AccountsTreeId = Lookups.StockSellCost,
                                            //        BranchId = model.BranchId,
                                            //        Debit = model.TotalValue,
                                            //        Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                            //        TransactionDate = model.InvoiceDate,
                                            //        TransactionId = model.Id,
                                            //        TransactionShared = model.Id,
                                            //        TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            //    });
                                            //    //الى ح/ المخزون
                                            //    context.GeneralDailies.Add(new GeneralDaily
                                            //    {
                                            //        AccountsTreeId = accountTreeInventoryType,
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
                                            //return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                                            //============================================

                                            // من ح/  العميل
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                                BranchId = model.BranchId,
                                                Debit = model.Safy,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            //الى ح/ المبيعات
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.TotalValue,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });

                                            debit = debit + model.Safy;
                                            credit = credit + model.TotalValue;

                                        }


                                        // القيمة المضافة
                                        if (model.SalesTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.SalesTax,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            credit = credit + model.SalesTax;


                                        }
                                        // الخصومات
                                        if (model.TotalDiscount > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.TotalDiscount,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            debit = debit + model.TotalDiscount;
                                        }

                                        // ضريبة ارباح تجارية
                                        if (model.ProfitTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.ProfitTax,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            debit = debit + model.ProfitTax;
                                        }

                                        //  المبلغ المدفوع الى حساب العميل (دائن
                                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        {
                                            Guid? safeAccouyBank = null;
                                            if (model.SafeId != null)
                                                safeAccouyBank = context.Safes.FirstOrDefault(x => x.Id == model.SafeId).AccountsTreeId;
                                            else if (model.BankAccountId != null)
                                                safeAccouyBank = context.BankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId).AccountsTreeId;

                                            if (safeAccouyBank != null)
                                            {

                                                //  المبلغ المدفوع من حساب الخزينة (مدين
                                                context.GeneralDailies.Add(new GeneralDaily
                                                {
                                                    AccountsTreeId = safeAccouyBank,
                                                    BranchId = model.BranchId,
                                                    Debit = model.PayedValue,
                                                    Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                    TransactionDate = model.InvoiceDate,
                                                    TransactionId = model.Id,
                                                    TransactionShared = model.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.Sell
                                                });
                                                debit = debit + model.PayedValue;

                                            }


                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                                BranchId = model.BranchId,
                                                Credit = model.PayedValue,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            credit = credit + model.PayedValue;



                                        }

                                        // الايرادات (دائن
                                        foreach (var expense in expenses)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = expense.IncomeTypeAccountTreeId,
                                                BranchId = model.BranchId,
                                                Credit = expense.Amount,
                                                Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionShared = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                                            });
                                            credit = credit + expense.Amount;
                                        }
                                        //التاكد من توازن المعاملة 
                                        if (debit == credit)
                                        {

                                            //حفظ القيود 
                                            // اعتماد النهائى للفاتورة
                                            model.CaseId = (int)CasesCl.InvoiceFinalApproval;
                                            model.IsFinalApproval = true;
                                            context.Entry(model).State = EntityState.Modified;
                                            //اضافة الحالة 
                                            context.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                                            {
                                                SellInvoice = model,
                                                IsSellInvoice = true,
                                                CaseId = (int)CasesCl.InvoiceFinalApproval
                                            });

                                            //اضافة اشعار 
                                            if (model.RemindValue > 0 && model.DueDate != null)
                                            {
                                                //حذف اى اشعارات سابقة لنفس الفاتورة 
                                                var notificationsPrevious = context.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id).ToList();
                                                foreach (var item in notificationsPrevious)
                                                {
                                                    item.IsDeleted = true;
                                                }
                                                context.Notifications.Add(new Notification
                                                {
                                                    Name = $"استحقاق فاتورة بيع رقم: {model.InvoiceNumber} على العميل: {customer.Name}",
                                                    DueDate = model.DueDate,
                                                    RefNumber = model.Id,
                                                    NotificationTypeId = (int)NotificationTypeCl.SellInvoiceDueDateClient,
                                                    Amount = model.RemindValue
                                                });
                                            }
                                            stopwatch.Stop();
                                            stopwatch1.Start();
                                            //انشاء السيرالات نمبر الخاص بكل صنف 
                                            var itemDetails = model.SellInvoicesDetails.Where(x => !x.IsDeleted).ToList();
                                            foreach (var item in itemDetails)
                                            {
                                                var itm = context.Items.Where(x => x.Id == item.ItemId).FirstOrDefault();
                                                if (itm != null)
                                                {
                                                    bool createSerial = itm.CreateSerial;
                                                    if (createSerial)
                                                    {
                                                        //تسجيل سيريال جديد لكل قطعة من الكمية المسجلة
                                                        for (int i = 1; i < item.Quantity + 1; i++)
                                                        {
                                                            //التأكد من ان الصنف لم يكن له اى سيريال مسبقا 
                                                            var itemSerial = context.ItemSerials.Where(x => !x.IsDeleted && x.Id == item.ItemSerialId).FirstOrDefault();
                                                            if (itemSerial != null)
                                                            {
                                                                //السيريال موجود مسبقا
                                                                itemSerial.SellInvoiceId = model.Id;
                                                                itemSerial.CurrentStoreId = null;
                                                                itemSerial.ExpirationDate = model.InvoiceDate.AddMonths(1);
                                                                itemSerial.SerialCaseId = (int)SerialCaseCl.Sell;
                                                                context.Entry(itemSerial).State = EntityState.Modified;
                                                            }
                                                            else
                                                            {
                                                            generate:
                                                                var serial = GeneratBarcodes.GenerateRandomBarcode();
                                                                var isExists = context.ItemSerials.Where(x => x.SerialNumber == serial).Any();
                                                                if (isExists)
                                                                    goto generate;
                                                                itemSerial = new ItemSerial
                                                                {
                                                                    ItemId = item.ItemId,
                                                                    ProductionOrderId = item.ProductionOrderId,
                                                                    IsItemIntial = item.IsItemIntial ? true : false,
                                                                    ExpirationDate = model.InvoiceDate.AddMonths(1),
                                                                    SerialCaseId = (int)SerialCaseCl.Sell,
                                                                    SerialNumber = serial,
                                                                    SellInvoiceId = model.Id
                                                                };


                                                            }

                                                            //add item serial case history
                                                            context.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                                            {
                                                                ItemSerial = itemSerial,
                                                                ReferrenceId = model.Id,
                                                                SerialCaseId = (int)SerialCaseCl.Sell
                                                            });
                                                            //تحديث سيريال الصنف الجديد 
                                                            //item.ItemSerial = itemSerial;
                                                            //context.Entry(item).State = EntityState.Modified;
                                                        }

                                                    }

                                                }

                                            }
                                            stopwatch1.Stop();
                                            //===========================

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
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.SellInvoice, refGid = model.Id, isInsert, message = "تم الاضافة بنجاح" });
                                }
                                else
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.SellInvoice, refGid = model.Id, isInsert, message = "تم التعديل بنجاح" });

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
                var model = db.SellInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    ////هل الصنف يسمح بالسحب منه بالسالب
                    //foreach (var item in model.SellInvoicesDetails.Where(x=>!x.IsDeleted))
                    //{
                    //    var result = itemService.IsAllowNoBalance(item.ItemId, item.StoreId,item.Quantity);
                    //    if (!result.IsValid)
                    //        return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });
                    //}

                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.InvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        // حذف كل السيريالات الاصناف ان وجدت 
                        //var itemSerial = db.ItemSerials.Where(x => details.Any(y => y.ItemSerialId == x.Id)).ToList();
                        if (detail.ItemSerial != null)
                        {
                            var serial = detail.ItemSerial;

                            serial.IsDeleted = true;
                            db.Entry(serial).State = EntityState.Modified;
                            var itemSerialsHistories = db.CasesItemSerialHistories.Where(x => x.ItemSerialId == serial.Id).ToList();
                            foreach (var itemSerialHis in itemSerialsHistories)
                            {
                                itemSerialHis.IsDeleted = true;
                                db.Entry(itemSerialHis).State = EntityState.Modified;
                            }
                        }
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                    }

                    var expenses = db.SellInvoiceIncomes.Where(x => x.SellInvoiceId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    var casesSellInvoiceHistories = db.CasesSellInvoiceHistories.Where(x => x.SellInvoiceId == model.Id).ToList();
                    foreach (var cases in casesSellInvoiceHistories)
                    {
                        cases.IsDeleted = true;
                        db.Entry(cases).State = EntityState.Modified;
                    }
                    //db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    //{
                    //    SellInvoice = model,
                    //    IsSellInvoice = true,
                    //    CaseId = (int)CasesCl.InvoiceDeleted
                    //});
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
                    }
                    // حذف  اشعار الاستحقاق ان وجد 
                    var notifications = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceDueDateClient).ToList();
                    foreach (var notify in notifications)
                    {
                        notify.IsDeleted = true;
                        db.Entry(notify).State = EntityState.Modified;
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


        #region عرض بيانات فاتورة البيع بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowSellInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellInvoices.Where(x => x.Id == invoGuid && x.SellInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.SellInvoicesDetails = vm.SellInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            vm.SellInvoiceIncomes = vm.SellInvoiceIncomes.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellInvoices.Where(x => x.Id == invoGuid && x.CasesSellInvoiceHistories.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.CasesSellInvoiceHistories = vm.CasesSellInvoiceHistories.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        #endregion

        #region فك الاعتماد 
        [HttpPost]
        public ActionResult UnApproval(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.SellInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //هل الصنف يسمح بالسحب منه بالسالب
                    foreach (var item in model.SellInvoicesDetails.Where(x => !x.IsDeleted))
                    {
                        var result = itemService.IsAllowNoBalance(item.ItemId, item.StoreId, item.Quantity);
                        if (!result.IsValid)
                            return Json(new { isValid = false, message = $"لا يمكن فك الاعتماد بسبب تأثر ارصده للصنف  {result.ItemNotAllowed}" });
                    }

                    model.IsFinalApproval = false;
                    model.IsApprovalStore = false;
                    model.IsApprovalAccountant = false;
                    model.CaseId = (int)CasesCl.InvoiceUnApproval;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellInvoice = model,
                        IsSellInvoice = true,
                        CaseId = (int)CasesCl.InvoiceUnApproval
                    });
                    
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
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
        #region الاعتماد النهائى للفاتورة 
        public ActionResult GetFinalApproval()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellInvoices.Where(x => !x.IsDeleted && x.IsApprovalStore && x.IsApprovalAccountant).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, EmployeeName = x.Employee.Person.Name, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, CaseId = x.CaseId, CaseName = x.Case != null ? x.Case.Name : "", IsFinalApproval = x.IsFinalApproval, FinalApproval = x.IsFinalApproval ? "معتمده نهائيا" : "غير معتمده", Actions = n, Num = n }).ToList()
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
                    var model = db.SellInvoices.Where(x => x.Id == Id).FirstOrDefault();
                    if (model == null)
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    return InvoiveFinalApprov(model);

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }

        JsonResult InvoiveFinalApprov(SellInvoice model)
        {
            //التأكد من اختيار طريقة السداد فى حالة ان الفاتورة لمندوب 
            if (model.BySaleMen && model.SafeId == null && model.BankAccountId == null && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                return Json(new { isValid = false, message = "تأكد من تحديد حساب الدفع للفاتورة اولا" });

            //    //تسجيل القيود
            // General Dailies
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات
                //+ إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                double debit = 0;
                double credit = 0;
                // الحصول على حسابات من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                //التأكد من عدم وجود حساب فرعى من الحساب
                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب المبيعات ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault().AccountsTreeCustomerId))
                    return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                var expenses = db.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == model.Id);
                var customer = db.Persons.Where(x => x.Id == model.CustomerId).FirstOrDefault();

                if (expenses.Count() > 0)
                {
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                        return Json(new { isValid = false, message = "حساب الايراد ليس بحساب فرعى" });
                }

                //فى حالة فاتورة البيع من خلال المندوب التاكد من تحديد طريقة الدفع والسداد(خزنة/بنكى)
                if (model.BySaleMen && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                {
                    if (model.SafeId == null && model.BankAccountId == null)
                        return Json(new { isValid = false, message = "تأكد من تحديد طريقة السداد (خزينة/بنكى)للفاتورة" });
                }
                // فى حالة ان الفاتورة كلها عينات وبدون سعر
                if (model.TotalValue != 0)
                {
                    ////تحديد نوع الجرد
                    //var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                    //Guid accountTreeInventoryType;
                    //if (int.TryParse(inventoryType, out int inventoryTypeVal))
                    //    accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                    //else
                    //    return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                    ////اى حالة الجرد المستمر
                    //// من ح/  تكلفة بضاعه مباعه
                    //db.GeneralDailies.Add(new GeneralDaily
                    //{
                    //    AccountsTreeId = Lookups.StockSellCost,
                    //    BranchId = model.BranchId,
                    //    Debit = model.TotalValue,
                    //    Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                    //    TransactionDate = model.InvoiceDate,
                    //    TransactionId = model.Id,
                    //    TransactionShared = model.Id,
                    //    TransactionTypeId = (int)TransactionsTypesCl.Sell
                    //});
                    ////الى ح/ المخزون
                    //db.GeneralDailies.Add(new GeneralDaily
                    //{
                    //    AccountsTreeId = accountTreeInventoryType,
                    //    BranchId = model.BranchId,
                    //    Credit = model.TotalValue,
                    //    Notes = $"فاتورة بيع رقم : {model.InvoiceNumber} للعميل {customer.Name}",
                    //    TransactionDate = model.InvoiceDate,
                    //    TransactionId = model.Id,
                    //    TransactionShared = model.Id,
                    //    TransactionTypeId = (int)TransactionsTypesCl.Sell
                    //});
                    //============================================


                    // من ح/  العميل
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = customer.AccountsTreeCustomerId,
                        BranchId = model.BranchId,
                        Debit = model.Safy,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    //الى ح/ المبيعات
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Credit = model.TotalValue,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    debit = debit + model.Safy;

                    credit = credit + model.TotalValue;
                }


                // القيمة المضافة
                if (model.SalesTax > 0)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Credit = model.SalesTax,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    credit = credit + model.SalesTax;


                }
                // الخصومات
                if (model.TotalDiscount > 0)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Debit = model.TotalDiscount,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    debit = debit + model.TotalDiscount;
                }

                // ضريبة ارباح تجارية
                if (model.ProfitTax > 0)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Debit = model.ProfitTax,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    debit = debit + model.ProfitTax;
                }

                //  المبلغ المدفوع الى حساب العميل (دائن
                if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                {
                    Guid? safeAccouyBank = null;
                    if (model.SafeId != null)
                        safeAccouyBank = db.Safes.FirstOrDefault(x => x.Id == model.SafeId).AccountsTreeId;
                    else if (model.BankAccountId != null)
                        safeAccouyBank = db.BankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId).AccountsTreeId;

                    if (safeAccouyBank != null)
                    {
                        //  المبلغ المدفوع من حساب الخزينة (مدين
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = safeAccouyBank,
                            BranchId = model.BranchId,
                            Debit = model.PayedValue,
                            Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                            TransactionDate = model.InvoiceDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        debit = debit + model.PayedValue;


                    }


                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = customer.AccountsTreeCustomerId,
                        BranchId = model.BranchId,
                        Credit = model.PayedValue,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    credit = credit + model.PayedValue;



                }

                // الايرادات (دائن
                foreach (var expense in expenses)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = expense.IncomeTypeAccountTreeId,
                        BranchId = model.BranchId,
                        Credit = expense.Amount,
                        Notes = $"فاتورة بيع رقم : {model.Id} للعميل {customer.Name}",
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    credit = credit + expense.Amount;
                }
                //التاكد من توازن المعاملة 
                if (debit == credit)
                {

                    //حفظ القيود 
                    // اعتماد النهائى للفاتورة
                    model.CaseId = (int)CasesCl.InvoiceFinalApproval;
                    model.IsFinalApproval = true;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellInvoice = model,
                        IsSellInvoice = true,
                        CaseId = (int)CasesCl.InvoiceFinalApproval
                    });

                    //اضافة اشعار 
                    if (model.RemindValue > 0 && model.DueDate != null)
                    {
                        //حذف اى اشعارات سابقة لنفس الفاتورة 
                        var notificationsPrevious = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id).ToList();
                        foreach (var item in notificationsPrevious)
                        {
                            item.IsDeleted = true;
                        }
                        db.Notifications.Add(new Notification
                        {
                            Name = $"استحقاق فاتورة بيع رقم: {model.Id} على العميل: {customer.Name}",
                            DueDate = model.DueDate,
                            RefNumber = model.Id,
                            NotificationTypeId = (int)NotificationTypeCl.SellInvoiceDueDateClient,
                            Amount = model.RemindValue
                        });
                    }


                    //انشاء السيرالات نمبر الخاص بكل صنف 
                    var itemDetails = model.SellInvoicesDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in itemDetails)
                    {
                        var itm = db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault();
                        if (itm != null)
                        {
                            bool createSerial = itm.CreateSerial;
                            if (createSerial)
                            {
                                //تسجيل سيريال جديد لكل قطعة من الكمية المسجلة
                                for (int i = 1; i < item.Quantity + 1; i++)
                                {
                                    //التأكد من ان الصنف لم يكن له اى سيريال مسبقا 
                                    var itemSerial = db.ItemSerials.Where(x => !x.IsDeleted && x.Id == item.ItemSerialId).FirstOrDefault();
                                    if (itemSerial != null)
                                    {
                                        //السيريال موجود مسبقا
                                        itemSerial.SellInvoiceId = model.Id;
                                        itemSerial.CurrentStoreId = null;
                                        itemSerial.ExpirationDate = model.InvoiceDate.AddMonths(1);
                                        itemSerial.SerialCaseId = (int)SerialCaseCl.Sell;
                                        db.Entry(itemSerial).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                    generate:
                                        var serial = GeneratBarcodes.GenerateRandomBarcode();
                                        var isExists = db.ItemSerials.Where(x => x.SerialNumber == serial).Any();
                                        if (isExists)
                                            goto generate;
                                        itemSerial = new ItemSerial
                                        {
                                            ItemId = item.ItemId,
                                            ProductionOrderId = item.ProductionOrderId,
                                            IsItemIntial = item.IsItemIntial ? true : false,
                                            ExpirationDate = model.InvoiceDate.AddMonths(1),
                                            SerialCaseId = (int)SerialCaseCl.Sell,
                                            SerialNumber = serial,
                                            SellInvoiceId = model.Id
                                        };


                                    }

                                    //add item serial case history
                                    db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                    {
                                        ItemSerial = itemSerial,
                                        ReferrenceId = model.Id,
                                        SerialCaseId = (int)SerialCaseCl.Sell
                                    });
                                    //تحديث سيريال الصنف الجديد 
                                    //item.ItemSerial = itemSerial;
                                    //db.Entry(item).State = EntityState.Modified;
                                }


                            }
                        }


                    }

                    //===========================
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
        #endregion

        #region طباعة باركود الاصناف 
        public ActionResult PrintBarCode(Guid? invoGuid)
        {
            PrintBarcodeVM vm = new PrintBarcodeVM();
            if (invoGuid != null)
            {
                vm = db.SellInvoices
                    .Where(x => x.Id == invoGuid)
                    .Select(x => new PrintBarcodeVM
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        TotalQuantity = x.TotalQuantity,
                        ItemSerial = x.ItemSerials.Where(y => !y.IsDeleted && x.Id == y.SellInvoiceId)
                            .Select(i => new ItemSerialM
                            {
                                ItemName = i.Item.Name,
                                SerialNumber = i.SerialNumber
                            }).ToList()
                    }).FirstOrDefault();
                return View(vm);
            }
            else
                return View("ApprovalFinalInvoice");
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