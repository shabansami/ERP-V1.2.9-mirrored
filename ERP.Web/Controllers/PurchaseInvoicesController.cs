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
using System.Runtime.Remoting.Contexts;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PurchaseInvoicesController : Controller
    {
        // GET: PurchaseInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public PurchaseInvoicesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة فواتير التوريد
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
                    data = db.PurchaseInvoices.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList().Where(x => !x.IsDeleted && (x.Id.ToString() == txtSearch || x.PersonSupplier.Name.Contains(txtSearch))).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNumPaper = x.InvoiceNumPaper, PaymentTypeName = x.PaymentType.Name, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.PurchaseInvoice, IsFinalApproval = x.IsFinalApproval, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.PurchaseInvoices.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                    list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);
                var branchesList = list.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList();

                return Json(new
                    {
                        data = branchesList.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNumPaper = x.InvoiceNumPaper, PaymentTypeName = x.PaymentType.Name, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.PurchaseInvoice, IsFinalApproval = x.IsFinalApproval, Actions = n, Num = n }).ToList()
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
            string containerName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && x.Price == vm.Price).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);
            if (vm.Quantity <= 0)
                return Json(new { isValid = false, msg = "تأكد من ادخال رقم صحيح للكمية " }, JsonRequestBehavior.AllowGet);

            if (vm.ContainerId != null)
                containerName = db.Containers.FirstOrDefault(x => x.Id == vm.ContainerId).Name;

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = Math.Round(vm.Quantity * vm.Price, 2, MidpointRounding.ToEven), ContainerId = vm.ContainerId, ContainerName = containerName, ItemDiscount = vm.ItemDiscount, ItemEntryDate = vm.ItemEntryDate, StoreId = vm.StoreId, StoreName = storeName };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), totalQuantity = deDS.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل مصروفات الفاتورة
        public ActionResult GetDStPurchaseInvoiceExpenses()
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

        public ActionResult AddPurchaseInvoiceExpenses(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            string expenseTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "المصروف المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                expenseTypeName = db.AccountsTrees.FirstOrDefault(x => x.Id == vm.ExpenseTypeId).AccountName;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseTypeId))
                    return Json(new { isValid = false, msg = "حساب المصروفات ليس بحساب فرعى" });

            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار مصروف " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل المصروف بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل فاتورة توريد وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit(string shwTab)
        {
            ViewBag.ContainerId = new SelectList(db.Containers.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name");

            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");
            if (shwTab != null && shwTab == "1")
                ViewBag.ShowTab = true;
            else
                ViewBag.ShowTab = false;

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.PurchaseInvoices.Where(x => x.Id == guId).FirstOrDefault();

                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == vm.Id).Select(item => new
                                  ItemDetailsDT
                    {
                        Amount = item.Amount,
                        ContainerId = item.ContainerId,
                        ContainerName = item.Container != null ? item.Container.Name : null,
                        ItemId = item.ItemId,
                        ItemDiscount = item.ItemDiscount,
                        ItemEntryDate = item.ItemEntryDate.ToString()/*!=null? item.ItemEntryDate.Value.ToShortDateString():null*/,
                        ItemName = item.Item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId,
                        StoreName = item.Store.Name
                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);
                    var expenses = db.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == vm.Id).Select(expense => new
                                 InvoiceExpensesDT
                    {
                        ExpenseAmount = expense.Amount,
                        ExpenseTypeId = expense.ExpenseTypeAccountTreeId,
                        ExpenseTypeName = expense.ExpenseTypeAccountTree.AccountName
                    }).ToList();
                    DSExpenses = JsonConvert.SerializeObject(expenses);

                    ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId && !x.IsDamages), "Id", "Name");
                    ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) || x.Id == vm.SupplierId), "Id", "Name", vm.SupplierId);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
                    ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name", vm.PersonSupplier.PersonCategoryId);
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

                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && (x.BranchId == branchId) && !x.IsDamages), "Id", "Name", defaultStore?.Id);
                ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", bankAccountId);
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");
                Random random = new Random();
                //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();

                var vm = new PurchaseInvoice();
                vm.InvoiceDate = Utility.GetDateTime();
                vm.DueDate = Utility.GetDateTime().AddMonths(1);
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(PurchaseInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses)
        {

            if (ModelState.IsValid)
            {
                if (vm.SupplierId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                //if (vm.InvoiceDiscount ==0)
                //    return Json(new { isValid = false, message = "تأكد من ادخال مبلغ الخصم على الفاتورة" });
                //if (vm.PayedValue == 0)
                //    return Json(new { isValid = false, message = "تأكد من ادخال المبلغ المدفوع بشكل صحيح" });
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
                List<PurchaseInvoicesDetail> items = new List<PurchaseInvoicesDetail>();

                if (DT_DatasourceItems != null)
                {
                    itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                    if (itemDetailsDT.Count() == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    else
                    {
                        items = itemDetailsDT.Select(x =>
                          new PurchaseInvoicesDetail
                          {
                              PurchaseInvoiceId = vm.Id,
                              ItemId = x.ItemId,
                              StoreId = x.StoreId,
                              Quantity = x.Quantity,
                              Price = x.Price,
                              Amount = x.Amount,
                              ItemDiscount = x.ItemDiscount,
                              ContainerId = x.ContainerId,
                              ItemEntryDate = DateTime.TryParse(x.ItemEntryDate, out var dt) ? DateTime.Parse(x.ItemEntryDate) : new Nullable<DateTime>()
                          }).ToList();
                    }
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                //المصروفات
                List<InvoiceExpensesDT> invoiceExpensesDT = new List<InvoiceExpensesDT>();
                List<PurchaseInvoicesExpens> invoicesExpens = new List<PurchaseInvoicesExpens>();

                if (DT_DatasourceExpenses != null)
                {
                    invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                    invoicesExpens = invoiceExpensesDT.Select(
                        x => new PurchaseInvoicesExpens
                        {
                            PurchaseInvoiceId = vm.Id,
                            ExpenseTypeAccountTreeId = x.ExpenseTypeId,
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
                            PurchaseInvoice model = null;
                            if (vm.Id != Guid.Empty)
                            {
                                model = context.PurchaseInvoices.FirstOrDefault(x => x.Id == vm.Id);
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
                                var privousItems = context.PurchaseInvoicesDetails.Where(x => x.PurchaseInvoiceId == vm.Id).ToList();
                                foreach (var item in privousItems)
                                {
                                    item.IsDeleted = true;
                                    context.Entry(item).State = EntityState.Modified;
                                }
                                //delete all privous expenses
                                var privousExpenese = context.PurchaseInvoicesExpenses.Where(x => x.PurchaseInvoiceId == vm.Id).ToList();
                                foreach (var expense in privousExpenese)
                                {
                                    expense.IsDeleted = true;
                                    context.Entry(expense).State = EntityState.Modified;
                                }

                                //فى حالة تم الاعتماد النهائى للفاتورة ثم مطلوب تعديلها 
                                if (model.CaseId == (int)CasesCl.InvoiceFinalApproval)
                                {
                                    var generalDalies = context.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Purchases).ToList();
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
                                model.InvoiceNumPaper = vm.InvoiceNumPaper;
                                model.SupplierId = vm.SupplierId;
                                model.BranchId = vm.BranchId;
                                model.PaymentTypeId = vm.PaymentTypeId;
                                model.SafeId = vm.SafeId;
                                model.BankAccountId = vm.BankAccountId;
                                model.PayedValue = vm.PayedValue;
                                model.RemindValue = vm.RemindValue;
                                model.SalesTax = vm.SalesTax;
                                model.ProfitTax = vm.ProfitTax;
                                model.ProfitTaxPercentage = vm.ProfitTaxPercentage;
                                model.SalesTaxPercentage = vm.SalesTaxPercentage;
                                model.InvoiceDiscount = vm.InvoiceDiscount;
                                //model.ShippingAddress = vm.ShippingAddress;
                                model.DueDate = vm.RemindValue > 0 ? vm.DueDate : null;
                                model.Notes = vm.Notes;
                                model.CaseId = (int)CasesCl.InvoiceModified;
                                context.Entry(model).State = EntityState.Modified;
                                context.PurchaseInvoicesDetails.AddRange(items);
                                context.PurchaseInvoicesExpenses.AddRange(invoicesExpens);
                                //اضافة الحالة 
                                context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseInvoice = model,
                                    IsPurchaseInvoice = true,
                                    CaseId = (int)CasesCl.InvoiceModified
                                });

                            }
                            else
                            {

                                isInsert = true;
                                //اضافة رقم الفاتورة
                                string codePrefix = Properties.Settings.Default.CodePrefix;
                                model.InvoiceNumber = codePrefix + (db.PurchaseInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                                model.CaseId = (int)CasesCl.InvoiceCreated;
                                model.PurchaseInvoicesDetails = items;
                                model.PurchaseInvoicesExpenses = invoicesExpens;
                                model.DueDate = vm.RemindValue > 0 ? vm.DueDate : null;
                                //اضافة الحالة 
                                context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseInvoice = model,
                                    IsPurchaseInvoice = true,
                                    CaseId = (int)CasesCl.InvoiceCreated
                                });
                                context.PurchaseInvoices.Add(model);
                            }
                            //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
                            var approvalAfterSave = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
                            if (approvalAfterSave == "1")
                            {
                                //الاعتماد المحاسبى 
                                model.IsApprovalAccountant = true;
                                model.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                                //اضافة الحالة 
                                context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseInvoice = model,
                                    IsPurchaseInvoice = true,
                                    CaseId = (int)CasesCl.InvoiceApprovalAccountant
                                });
                                //الاعتماد المخزنى
                                model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                                model.IsApprovalStore = true;
                                //اضافة الحالة 
                                context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseInvoice = model,
                                    IsPurchaseInvoice = true,
                                    CaseId = (int)CasesCl.InvoiceApprovalStore
                                });
                            }

                            //if (context.SaveChanges(auth.CookieValues.UserId) > 0)
                            //{
                            context.SaveChanges(auth.CookieValues.UserId);
                            //الاعتماد النهائى 
                            if (approvalAfterSave == "1")
                            {
                                //    //تسجيل القيود
                                // General Dailies
                                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                {
                                    //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                                    double debit = 0;
                                    double credit = 0;
                                    // الحصول على حسابات من الاعدادات
                                    var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                                    //التأكد من عدم وجود حساب فرعى من الحساب
                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue)))
                                        return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                                        return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                        return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                        return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                        return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                                    var expenses = context.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == model.Id);

                                    if (expenses.Count() > 0)
                                    {
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountTreeId))
                                            return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                                    }
                                    var supplier = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault();


                                    // حساب المشتريات
                                    if (model.TotalValue != 0)
                                    {
                                        //=============================================================
                                        //تحديد نوع الجرد
                                        var inventoryType = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                        if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                        {
                                            //فى حالة الجرد المستمر
                                            if (inventoryTypeVal == 2) //نوع الجرد مستمر 
                                            {
                                                if (Guid.TryParse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesCost).FirstOrDefault().SValue, out Guid accountTreeSalesCost))
                                                {

                                                    //من ح/ المخزون
                                                    foreach (var item in model.PurchaseInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                                    {
                                                        var store = context.Stores.Where(x => x.Id == item.StoreId).FirstOrDefault();
                                                        if (store != null)
                                                        {
                                                            if (store.AccountTreeId != null)
                                                            {
                                                                if (AccountTreeService.CheckAccountTreeIdHasChilds(store.AccountTreeId))
                                                                    return Json(new { isValid = false, message = $"حساب المخزن {store.Name} ليس بحساب فرعى" });

                                                                context.GeneralDailies.Add(new GeneralDaily
                                                                {
                                                                    AccountsTreeId = store.AccountTreeId,
                                                                    BranchId = model.BranchId,
                                                                    Debit = item.Amount,
                                                                    Notes = $"فاتورة توريد رقم : {model.Id} من المورد {supplier.Name}",
                                                                    TransactionDate = model.InvoiceDate,
                                                                    TransactionId = model.Id,
                                                                    TransactionShared = model.Id,
                                                                    TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                                                });
                                                            }
                                                            else
                                                                return Json(new { isValid = false, message = $"حساب المخزون للمخزن {store.Name} غير موجود" });
                                                        }
                                                    }

                                                    debit = debit + model.TotalValue;
                                                }
                                                else
                                                    return Json(new { isValid = false, message = "تأكد من تحديد حساب تكلفة بضاعه مباعه من الاعدادات اولا" });
                                            }
                                            else
                                            {

                                                //من ح/ المشتريات
                                                context.GeneralDailies.Add(new GeneralDaily
                                                {
                                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
                                                    BranchId = model.BranchId,
                                                    Debit = model.TotalValue,
                                                    Notes = $"فاتورة توريد رقم : {model.Id} من المورد {supplier.Name}",
                                                    PaperNumber = model.InvoiceNumPaper,
                                                    TransactionDate = model.InvoiceDate,
                                                    TransactionId = model.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                                });
                                                //context.SaveChanges(auth.CookieValues.UserId);
                                                debit = debit + model.TotalValue;
                                            }

                                        }
                                        else
                                            return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                                        //============================================

                                        // حساب المورد
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = supplier.AccountTreeSupplierId,
                                            BranchId = model.BranchId,
                                            Credit = model.Safy,
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
                                        credit = credit + model.Safy;

                                    }


                                    // القيمة المضافة
                                    if (model.SalesTax > 0)
                                    {
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                            BranchId = model.BranchId,
                                            Debit = model.SalesTax,
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
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
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
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
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
                                        credit = credit + model.ProfitTax;
                                    }

                                    //  المبلغ المدفوع من حساب المورد (مدين
                                    if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                    {
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = supplier.AccountTreeSupplierId,
                                            BranchId = model.BranchId,
                                            Debit = model.PayedValue,
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
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
                                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                                PaperNumber = model.InvoiceNumPaper,
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                            });
                                            //context.SaveChanges(auth.CookieValues.UserId);
                                            credit = credit + model.PayedValue;
                                        }


                                    }

                                    // المصروفات (مدين
                                    foreach (var expense in expenses)
                                    {
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = expense.ExpenseTypeAccountTreeId,
                                            BranchId = model.BranchId,
                                            Debit = expense.Amount,
                                            Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                            PaperNumber = model.InvoiceNumPaper,
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                        });
                                        //context.SaveChanges(auth.CookieValues.UserId);
                                        debit = debit + expense.Amount;
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
                                        context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                        {
                                            PurchaseInvoice = model,
                                            IsPurchaseInvoice = true,
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
                                                Name = $"استحقاق فاتورة توريد رقم: {model.InvoiceNumber} للمورد: {supplier.Name}",
                                                DueDate = model.DueDate,
                                                RefNumber = model.Id,
                                                NotificationTypeId = (int)NotificationTypeCl.PurchaseInvoiceDueDateSupplier,
                                                Amount = model.RemindValue
                                            });
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
                                return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.PurchaseInvoice, refGid = model.Id, isInsert, message = "تم الاضافة بنجاح" });
                            else
                                return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.PurchaseInvoice, refGid = model.Id, isInsert, message = "تم التعديل بنجاح" });

                            //}
                            //else
                            //    return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });






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


            //return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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
                var model = db.PurchaseInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.InvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.PurchaseInvoicesDetails.Where(x => x.PurchaseInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                    }

                    var expenses = db.PurchaseInvoicesExpenses.Where(x => x.PurchaseInvoiceId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    var casesInvoiceHistories = db.CasesPurchaseInvoiceHistories.Where(x => x.PurchaseInvoiceId == model.Id).ToList();
                    foreach (var cases in casesInvoiceHistories)
                    {
                        cases.IsDeleted = true;
                        db.Entry(cases).State = EntityState.Modified;
                    }

                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Purchases).ToList();
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
                var model = db.PurchaseInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsFinalApproval = false;
                    model.IsApprovalStore = false;
                    model.IsApprovalAccountant = false;
                    model.CaseId = (int)CasesCl.InvoiceUnApproval;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                    {
                        PurchaseInvoice = model,
                        IsPurchaseInvoice = true,
                        CaseId = (int)CasesCl.InvoiceUnApproval
                    });

                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Purchases).ToList();
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
        public ActionResult ShowPurchaseInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.PurchaseInvoices.Where(x => x.Id == invoGuid && x.PurchaseInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.PurchaseInvoicesDetails = vm.PurchaseInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            vm.PurchaseInvoicesExpenses = vm.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.PurchaseInvoices.Where(x => x.Id == invoGuid && x.CasesPurchaseInvoiceHistories.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.CasesPurchaseInvoiceHistories = vm.CasesPurchaseInvoiceHistories.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).ToList();
            return View(vm);
        }

        #endregion


        #region الاعتماد النهائى للفاتورة 
        public ActionResult GetFinalApproval()
        {
            int? n = null;
            return Json(new
            {
                data = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.IsApprovalStore && x.IsApprovalAccountant).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, CaseId = x.CaseId, CaseName = x.Case != null ? x.Case.Name : "", IsFinalApproval = x.IsFinalApproval, FinalApproval = x.IsFinalApproval ? "معتمده نهائيا" : "غير معتمده", Actions = n, Num = n }).ToList()
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
                    var model = db.PurchaseInvoices.Where(x => x.Id == Id).FirstOrDefault();
                    if (model == null)
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    //    //تسجيل القيود
                    // General Dailies
                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                    {
                        //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                        double debit = 0;
                        double credit = 0;
                        // الحصول على حسابات من الاعدادات
                        var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                        //التأكد من عدم وجود حساب فرعى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                            return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                        var expenses = db.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == model.Id);
                        var supplier = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault();

                        if (expenses.Count() > 0)
                        {
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountTreeId))
                                return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                        }

                        // حساب المشتريات
                        if (model.TotalValue != 0)
                        {
                            //=============================================================
                            //تحديد نوع الجرد
                            var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                            if (int.TryParse(inventoryType, out int inventoryTypeVal))
                            {
                                //فى حالة الجرد المستمر
                                if (inventoryTypeVal == 2) //نوع الجرد مستمر 
                                {
                                    if (Guid.TryParse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesCost).FirstOrDefault().SValue, out Guid accountTreeSalesCost))
                                    {

                                        //من ح/ المخزون
                                        foreach (var item in model.PurchaseInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                        {
                                            var store = db.Stores.Where(x => x.Id == item.StoreId).FirstOrDefault();
                                            if (store != null)
                                            {
                                                if (store.AccountTreeId != null)
                                                {
                                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(store.AccountTreeId))
                                                        return Json(new { isValid = false, message = $"حساب المخزن {store.Name} ليس بحساب فرعى" });

                                                    db.GeneralDailies.Add(new GeneralDaily
                                                    {
                                                        AccountsTreeId = store.AccountTreeId,
                                                        BranchId = model.BranchId,
                                                        Debit = item.Amount,
                                                        Notes = $"فاتورة توريد رقم : {model.Id} من المورد {supplier.Name}",
                                                        TransactionDate = model.InvoiceDate,
                                                        TransactionId = model.Id,
                                                        TransactionShared = model.Id,
                                                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                                    });
                                                }
                                                else
                                                    return Json(new { isValid = false, message = $"حساب المخزون للمخزن {store.Name} غير موجود" });
                                            }
                                        }

                                        debit = debit + model.TotalValue;
                                    }
                                    else
                                        return Json(new { isValid = false, message = "تأكد من تحديد حساب تكلفة بضاعه مباعه من الاعدادات اولا" });
                                }
                                else
                                {

                                    //من ح/ المشتريات
                                    db.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
                                        BranchId = model.BranchId,
                                        Debit = model.TotalValue,
                                        Notes = $"فاتورة توريد رقم : {model.Id} من المورد {supplier.Name}",
                                        PaperNumber = model.InvoiceNumPaper,
                                        TransactionDate = model.InvoiceDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                    });
                                    //db.SaveChanges(auth.CookieValues.UserId);
                                    debit = debit + model.TotalValue;
                                }

                            }
                            else
                                return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                            //============================================

                            // حساب المورد
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = supplier.AccountTreeSupplierId,
                                BranchId = model.BranchId,
                                Credit = model.Safy,
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
                            });
                            //context.SaveChanges(auth.CookieValues.UserId);
                            credit = credit + model.Safy;
                        }


                        // القيمة المضافة
                        if (model.SalesTax > 0)
                        {
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                BranchId = model.BranchId,
                                Debit = model.SalesTax,
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
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
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
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
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
                            });
                            credit = credit + model.ProfitTax;
                        }

                        //  المبلغ المدفوع من حساب المورد (مدين
                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                        {
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = supplier.AccountTreeSupplierId,
                                BranchId = model.BranchId,
                                Debit = model.PayedValue,
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
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
                                    Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                    PaperNumber = model.InvoiceNumPaper,
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.Purchases
                                });
                                credit = credit + model.PayedValue;
                            }


                        }

                        // المصروفات (مدين
                        foreach (var expense in expenses)
                        {
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = expense.ExpenseTypeAccountTreeId,
                                BranchId = model.BranchId,
                                Debit = expense.Amount,
                                Notes = $"فاتورة توريد رقم : {model.InvoiceNumber} من المورد {supplier.Name}",
                                PaperNumber = model.InvoiceNumPaper,
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Purchases
                            });
                            debit = debit + expense.Amount;
                        }
                        //التاكد من توازن المعاملة 
                        if (debit == credit)
                        {

                            //حفظ القيود 
                            // اعتماد النهائى للفاتورة
                            model.CaseId = (int)CasesCl.InvoiceFinalApproval;
                            model.IsFinalApproval = true;
                            db.Entry(model).State = EntityState.Modified;
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
                                    Name = $"استحقاق فاتورة توريد رقم: {model.InvoiceNumber} للمورد: {supplier.Name}",
                                    DueDate = model.DueDate,
                                    RefNumber = model.Id,
                                    NotificationTypeId = (int)NotificationTypeCl.PurchaseInvoiceDueDateSupplier,
                                    Amount = model.RemindValue
                                });
                            }
                            //اضافة الحالة 
                            db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                            {
                                PurchaseInvoice = model,
                                IsPurchaseInvoice = true,
                                CaseId = (int)CasesCl.InvoiceFinalApproval
                            });


                            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            {
                                return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                            }
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية1" });


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
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }

        JsonResult InvoiveFinalApprov(PurchaseInvoice model)
        {
            //    //تسجيل القيود
            // General Dailies
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                double debit = 0;
                double credit = 0;
                // الحصول على حسابات من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                //التأكد من عدم وجود حساب فرعى من الحساب
                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                    return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                var expenses = db.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == model.Id);

                if (expenses.Count() > 0)
                {
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountTreeId))
                        return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                }
                //تحديد نوع الجرد
                //var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                //Guid accountTreeInventoryType;
                //if (int.TryParse(inventoryType, out int inventoryTypeVal))
                //{
                //    if (inventoryTypeVal == 1)//جرد دورى
                //                              //حساب المشتريات
                //        accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue);
                //    else if (inventoryTypeVal == 2)//جرد مستمر
                //                                   //حساب المخزون
                //        accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                //    else
                //        return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                //}
                //else
                //    return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });

                // حساب المشتريات
                //var transactionDate = ;
                //var t = db.CasesPurchaseInvoiceHistories.Where(x => x.IsPurchaseInvoice && x.PurchaseInvoiceId == model.Id && x.CaseId == (int)CasesCl.InvoiceFinalApproval).Count() > 0;
                //if (model.CasesPurchaseInvoiceHistories.Any(x => x.CaseId == (int)CasesCl.InvoiceFinalApproval))
                if (model.TotalValue != 0)
                {
                    //من ح/ المشتريات
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Debit = model.TotalValue,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    debit = debit + model.TotalValue;
                    // حساب المورد
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                        BranchId = model.BranchId,
                        Credit = model.Safy,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    credit = credit + model.Safy;
                }


                // القيمة المضافة
                if (model.SalesTax > 0)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                        BranchId = model.BranchId,
                        Debit = model.SalesTax,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
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
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
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
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    credit = credit + model.ProfitTax;
                }

                //  المبلغ المدفوع من حساب المورد (مدين
                if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                        BranchId = model.BranchId,
                        Debit = model.PayedValue,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    debit = debit + model.PayedValue;

                    //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = model.SafeId != null ? model.Safe.AccountsTreeId : model.BankAccount.AccountsTreeId,
                        BranchId = model.BranchId,
                        Credit = model.PayedValue,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    credit = credit + model.PayedValue;

                }

                // المصروفات (مدين
                foreach (var expense in expenses)
                {
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = expense.ExpenseTypeAccountTreeId,
                        BranchId = model.BranchId,
                        Debit = expense.Amount,
                        Notes = $"فاتورة توريد رقم : {model.Id}",
                        PaperNumber = model.InvoiceNumPaper,
                        TransactionDate = model.InvoiceDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Purchases
                    });
                    debit = debit + expense.Amount;
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
                    db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                    {
                        PurchaseInvoice = model,
                        IsPurchaseInvoice = true,
                        CaseId = (int)CasesCl.InvoiceFinalApproval
                    });


                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية1" });


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