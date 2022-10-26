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

    public class PurchaseBackInvoicesController : Controller
    {
        // GET: PurchaseBackInvoices
        VTSaleEntities db;
        VTSAuth auth;
        StoreService storeService;
        public PurchaseBackInvoicesController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
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
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && (x.Id.ToString() == txtSearch || x.PersonSupplier.Name.Contains(txtSearch))).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                DateTime dtFrom, dtTo;
                var list = db.PurchaseBackInvoices.Where(x => !x.IsDeleted);
                if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                {
                    list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);

                    return Json(new
                    {
                        data = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, Actions = n, Num = n }).ToList()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        data = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, Actions = n, Num = n }).ToList()
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
                if (deDS.Where(x => x.ItemId == vm.ItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            if (vm.ContainerId != null)
                containerName = db.Containers.FirstOrDefault(x => x.Id == vm.ContainerId).Name;

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ContainerId = vm.ContainerId, ContainerName = containerName, ItemDiscount = vm.ItemDiscount, ItemEntryDate = vm.ItemEntryDate, StoreId = vm.StoreId, StoreName = storeName };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل مصروفات الفاتورة
        public ActionResult GetDStPurchaseBackInvoiceExpenses()
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

        public ActionResult AddPurchaseBackInvoiceExpenses(InvoiceExpensesDT vm)
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
        public ActionResult CreateEdit()
        {
            ViewBag.ContainerId = new SelectList(db.Containers.Where(x => !x.IsDeleted), "Id", "Name");
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");


            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.PurchaseBackInvoices.Where(x => x.Id == guId).FirstOrDefault();

                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == vm.Id).Select(item => new
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
                    var expenses = db.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == vm.Id).Select(expense => new
                                 InvoiceExpensesDT
                    {
                        ExpenseAmount = expense.Amount,
                        ExpenseTypeId = expense.ExpenseTypeAccountsTreeID,
                        ExpenseTypeName = expense.ExpenseTypeAccountsTree.AccountName
                    }).ToList();
                    DSExpenses = JsonConvert.SerializeObject(expenses);

                    ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId && !x.IsDamages), "Id", "Name");
                    ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) || x.Id == vm.SupplierId), "Id", "Name", vm.SupplierId);
                    ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", vm.BranchId);
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
                var branches = db.Branches.Where(x => !x.IsDeleted);
                var safeId = db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId)?.FirstOrDefault().Id;
                var bankAccountId = db.BankAccounts.Where(x => !x.IsDeleted)?.FirstOrDefault().Id;

                //Guid storeDefaultId = Guid.Empty;
                //Guid? branchId = null;
                //var storeDefault = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreDefaultlId).FirstOrDefault();
                //if (!string.IsNullOrEmpty(storeDefault.SValue))
                //    if (Guid.TryParse(storeDefault.SValue, out storeDefaultId))
                //        branchId = db.Stores.Where(x => x.Id == storeDefaultId).FirstOrDefault().BranchId;

                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && (x.BranchId == branchId) && !x.IsDamages), "Id", "Name", defaultStore?.Id);
                ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name", safeId);
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", bankAccountId);
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");
                //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();

                var vm = new PurchaseBackInvoice();
                vm.InvoiceDate = Utility.GetDateTime();
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(PurchaseBackInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses)
        {
            try
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
                    List<PurchaseBackInvoicesDetail> items = new List<PurchaseBackInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new PurchaseBackInvoicesDetail
                              {
                                  PurchaseBackInvoiceId = vm.Id,
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
                    List<PurchaseBackInvoicesExpens> invoicesExpens = new List<PurchaseBackInvoicesExpens>();

                    if (DT_DatasourceExpenses != null)
                    {
                        invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        invoicesExpens = invoiceExpensesDT.Select(
                            x => new PurchaseBackInvoicesExpens
                            {
                                PurchaseBackInvoiceId = vm.Id,
                                ExpenseTypeAccountsTreeID = x.ExpenseTypeId,
                                Amount = x.ExpenseAmount
                            }
                            ).ToList();
                    }

                    var isInsert = false;
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //Transaction
                    using (var context = new VTSaleEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                PurchaseBackInvoice model = null;
                                if (vm.Id != Guid.Empty)
                                {
                                    model = context.PurchaseBackInvoices.FirstOrDefault(x => x.Id == vm.Id);
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
                                    var privousItems = context.PurchaseBackInvoicesDetails.Where(x => x.PurchaseBackInvoiceId == vm.Id).ToList();
                                    foreach (var item in privousItems)
                                    {
                                        item.IsDeleted = true;
                                        context.Entry(item).State = EntityState.Modified;
                                    }
                                    //delete all privous expenses
                                    var privousExpenese = context.PurchaseBackInvoicesExpenses.Where(x => x.PurchaseBackInvoiceId == vm.Id).ToList();
                                    foreach (var expense in privousExpenese)
                                    {
                                        expense.IsDeleted = true;
                                        context.Entry(expense).State = EntityState.Modified;
                                    }

                                    //فى حالة تم الاعتماد النهائى للفاتورة ثم مطلوب تعديلها 
                                    if (model.CaseId == (int)CasesCl.BackInvoiceFinalApproval)
                                    {
                                        var generalDalies = context.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.PurchasesReturn).ToList();
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
                                    model.PurchaseInvoiceId = vm.PurchaseInvoiceId == Guid.Empty ? null : vm.PurchaseInvoiceId;
                                    model.SupplierId = vm.SupplierId;
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
                                    model.DueDate = vm.DueDate;
                                    model.Notes = vm.Notes;
                                    model.CaseId = (int)CasesCl.BackInvoiceModified;
                                    context.Entry(model).State = EntityState.Modified;
                                    context.PurchaseBackInvoicesDetails.AddRange(items);
                                    context.PurchaseBackInvoicesExpenses.AddRange(invoicesExpens);
                                    //اضافة الحالة 
                                    context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = false,
                                        CaseId = (int)CasesCl.BackInvoiceModified
                                    });

                                }
                                else
                                {

                                    isInsert = true;
                                    //اضافة رقم الفاتورة
                                    string codePrefix = Properties.Settings.Default.CodePrefix;
                                    model.InvoiceNumber = codePrefix + (db.PurchaseBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                                    model.CaseId = (int)CasesCl.BackInvoiceCreated;
                                    model.PurchaseBackInvoicesDetails = items;
                                    model.PurchaseBackInvoicesExpenses = invoicesExpens;

                                    //اضافة الحالة 
                                    context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = false,
                                        CaseId = (int)CasesCl.BackInvoiceCreated
                                    });
                                    context.PurchaseBackInvoices.Add(model);
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
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalAccountant
                                    });
                                    //الاعتماد المخزنى
                                    model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                                    model.IsApprovalStore = true;
                                    //اضافة الحالة 
                                    context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = true,
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
                                        //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                                        double debit = 0;
                                        double credit = 0;
                                        // الحصول على حسابات من الاعدادات
                                        var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                                        //التأكد من عدم وجود حساب فرعى من الحساب
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                                            return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                                        var expenses = context.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == model.Id);
                                        var supplier = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault();

                                        if (expenses.Count() > 0)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountsTreeID))
                                                return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                                        }

                                        //تحديد نوع الجرد
                                        var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                        Guid accountTreeInventoryType;
                                        if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                        {
                                            if (inventoryTypeVal == 1)//جرد دورى
                                                                      //حساب مرتجع المشتريات
                                                accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue);
                                            else if (inventoryTypeVal == 2)//جرد مستمر
                                                                           //حساب المخزون
                                                accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                                            else
                                                return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                                        }
                                        else
                                            return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });

                                        // حساب مرتجع المشتريات
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = accountTreeInventoryType,
                                            BranchId = model.BranchId,
                                            Credit = model.TotalValue,
                                            Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                        });
                                        credit = credit + model.TotalValue;
                                        // حساب المورد
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                                            BranchId = model.BranchId,
                                            Debit = model.Safy,
                                            Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                            TransactionDate = model.InvoiceDate,
                                            TransactionId = model.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                        });
                                        debit = debit + model.Safy;

                                        // القيمة المضافة
                                        if (model.SalesTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.SalesTax,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + model.SalesTax;


                                        }
                                        // الخصومات
                                        if (model.TotalDiscount > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.TotalDiscount,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
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
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            debit = debit + model.ProfitTax;
                                        }

                                        //  المبلغ المدفوع من حساب المورد (مدين
                                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                                                BranchId = model.BranchId,
                                                Credit = model.PayedValue,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + model.PayedValue;

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
                                                    Debit = model.PayedValue,
                                                    Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                    TransactionDate = model.InvoiceDate,
                                                    TransactionId = model.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                                });
                                                debit = debit + model.PayedValue;
                                            }


                                        }

                                        // المصروفات (مدين
                                        foreach (var expense in expenses)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = expense.ExpenseTypeAccountsTreeID,
                                                //AccountsTreeId = expense.ExpenseType.AccountsTreeId,
                                                BranchId = model.BranchId,
                                                Credit = expense.Amount,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + expense.Amount;

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
                                            context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                            {
                                                PurchaseBackInvoice = model,
                                                IsPurchaseInvoice = false,
                                                CaseId = (int)CasesCl.BackInvoiceFinalApproval
                                            });
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
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, refGid = model.Id, isInsert, message = "تم الاضافة بنجاح" });
                                else
                                    return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, refGid = model.Id, isInsert, message = "تم التعديل بنجاح" });
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
                var model = db.PurchaseBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.BackInvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.PurchaseBackInvoicesDetails.Where(x => x.PurchaseBackInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                    }

                    var expenses = db.PurchaseBackInvoicesExpenses.Where(x => x.PurchaseBackInvoiceId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    var casesInvoiceHistories = db.CasesPurchaseInvoiceHistories.Where(x => x.PurchaseBackInvoiceId == model.Id).ToList();
                    foreach (var cases in casesInvoiceHistories)
                    {
                        cases.IsDeleted = true;
                        db.Entry(cases).State = EntityState.Modified;
                    }
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.PurchasesReturn).ToList();
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


        #region عرض بيانات فاتورة توريد بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowPurchaseBackInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.PurchaseBackInvoices.Where(x => x.Id == invoGuid && x.PurchaseBackInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.PurchaseBackInvoicesDetails = vm.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            vm.PurchaseBackInvoicesExpenses = vm.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.PurchaseBackInvoices.Where(x => x.Id == invoGuid && x.CasesPurchaseInvoiceHistories.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.CasesPurchaseInvoiceHistories = vm.CasesPurchaseInvoiceHistories.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }

        #endregion


        #region الاعتماد النهائى للفاتورة 
        public ActionResult GetFinalApproval()
        {
            int? n = null;
            return Json(new
            {
                data = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.IsApprovalStore && x.IsApprovalAccountant).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, CaseId = x.CaseId, CaseName = x.Case != null ? x.Case.Name : "", IsFinalApproval = x.IsFinalApproval, FinalApproval = x.IsFinalApproval ? "معتمده نهائيا" : "غير معتمده", Actions = n, Num = n }).ToList()
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
                    var model = db.PurchaseBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        if (TempData["userInfo"] != null)
                            auth = TempData["userInfo"] as VTSAuth;
                        else
                            RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
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
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                                return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                            var expenses = db.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == model.Id);
                            var supplier = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault();

                            if (expenses.Count() > 0)
                            {
                                if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountsTreeID))
                                    return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                                //if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseType.AccountsTreeId))
                                //    return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                            }

                            //تحديد نوع الجرد
                            var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                            Guid accountTreeInventoryType;
                            if (int.TryParse(inventoryType, out int inventoryTypeVal))
                            {
                                if (inventoryTypeVal == 1)//جرد دورى
                                                          //حساب مرتجع المشتريات
                                    accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue);
                                else if (inventoryTypeVal == 2)//جرد مستمر
                                                               //حساب المخزون
                                    accountTreeInventoryType = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                                else
                                    return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                            }
                            else
                                return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });


                            // حساب المشتريات
                            //var transactionDate = ;
                            //var t = db.CasesPurchaseBackInvoiceHistories.Where(x => x.IsPurchaseBackInvoice && x.PurchaseBackInvoiceId == model.Id && x.CaseId == (int)CasesCl.BackInvoiceFinalApproval).Count() > 0;
                            //if (model.CasesPurchaseBackInvoiceHistories.Any(x => x.CaseId == (int)CasesCl.BackInvoiceFinalApproval))

                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = accountTreeInventoryType,
                                BranchId = model.BranchId,
                                Credit = model.TotalValue,
                                Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                            });
                            credit = credit + model.TotalValue;
                            // حساب المورد
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = supplier.AccountTreeSupplierId,
                                BranchId = model.BranchId,
                                Debit = model.Safy,
                                Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                TransactionDate = model.InvoiceDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                            });
                            debit = debit + model.Safy;

                            // القيمة المضافة
                            if (model.SalesTax > 0)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                    BranchId = model.BranchId,
                                    Credit = model.SalesTax,
                                    Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                });
                                credit = credit + model.SalesTax;


                            }
                            // الخصومات
                            if (model.TotalDiscount > 0)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                    BranchId = model.BranchId,
                                    Debit = model.TotalDiscount,
                                    Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
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
                                    Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                });
                                debit = debit + model.ProfitTax;
                            }

                            //  المبلغ المدفوع من حساب المورد (مدين
                            if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                                    BranchId = model.BranchId,
                                    Credit = model.PayedValue,
                                    Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                });
                                credit = credit + model.PayedValue;

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
                                        Debit = model.PayedValue,
                                        Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                        TransactionDate = model.InvoiceDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                    });
                                    debit = debit + model.PayedValue;
                                }


                            }

                            // المصروفات (مدين
                            foreach (var expense in expenses)
                            {
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = expense.ExpenseTypeAccountsTreeID,
                                    //AccountsTreeId = expense.ExpenseType.AccountsTreeId,
                                    BranchId = model.BranchId,
                                    Credit = expense.Amount,
                                    Notes = $"فاتورة مرتجع توريد رقم : {model.Id} الى المورد {supplier.Name}",
                                    TransactionDate = model.InvoiceDate,
                                    TransactionId = model.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                });
                                credit = credit + expense.Amount;

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
                                db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseBackInvoice = model,
                                    IsPurchaseInvoice = false,
                                    CaseId = (int)CasesCl.BackInvoiceFinalApproval
                                });


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