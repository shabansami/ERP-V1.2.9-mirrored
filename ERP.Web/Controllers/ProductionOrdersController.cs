using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.ViewModels;
using System.Windows.Documents;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ProductionOrdersController : Controller
    {
        // GET: ProductionOrders
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService _itemService;
        CheckClosedPeriodServices closedPeriodServices;
        public ProductionOrdersController()
        {
            db = new VTSaleEntities();
            _itemService = new ItemService();
            closedPeriodServices = new CheckClosedPeriodServices();

        }
        //public static string DSItemProduction { get; set; }
        public static string DSItemsIn { get; set; }
        //public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة أوامر الإنتاج
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
            var productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted);
            if (DateTime.TryParse(dFrom,out DateTime dtFrom)&&DateTime.TryParse(dTo,out DateTime dtTo))
                productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.ProductionOrderDate) <= dtTo.Date);

            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                data = productionOrders.Where(x => !x.IsDeleted&&(x.Id.ToString()==txtSearch||/*x.FinalItem.Name.Contains(txtSearch)||*/x.OrderBarCode==txtSearch)).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, OrderNumber = x.OrderNumber, ProductionOrderDate = x.ProductionOrderDate.ToString(), /*FinalItemName = x.FinalItem.ItemCode + "|" + x.FinalItem.Name, OrderQuantity = x.OrderQuantity,IsDone = x.IsDone, IsDoneTitle = x.IsDone ? "1" : "2",*/  typ = (int)UploalCenterTypeCl.ProductionOrder, Actions = n , Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
            {
                //string delimiter = ",";
                //var list = db.ProductionOrders.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, OrderNumber = x.OrderNumber, ProductionOrderDate = x.ProductionOrderDate.ToString(), ItemOutName = x.ProductionOrderDetails.Where(p => !p.IsDeleted && p.ProductionTypeId == (int)ProductionTypeCl.Out).ToList().Select(p => p.Item.Name + "|").Aggregate((i, j) => i + delimiter + j), OrderQuantity = x.ProductionOrderDetails.Where(p => !p.IsDeleted && p.ProductionTypeId == (int)ProductionTypeCl.Out).FirstOrDefault().Quantity, IsDone = x.IsDone, IsDoneTitle = x.IsDone ? "1" : "2", Actions = n, Num = n }).ToList();
                return Json(new
                {
                    data = productionOrders.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, OrderNumber = x.OrderNumber, ProductionOrderDate = x.ProductionOrderDate.ToString(), ItemProduction=x.ProductionOrderDetails.Where(d=>!d.IsDeleted).FirstOrDefault().ItemProduction.Name,  typ = (int)UploalCenterTypeCl.ProductionOrder, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion

        #region تسجيل أمر الانتاج


        [HttpGet]
        public ActionResult RegisterOrder()
        {
            var vm = new ProductionOrder();

            //التاكد من تحديد مخزن التصنيع الداخلى من الاعدادات اولا 
            var storeProductionSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault();
            //Guid? productionStoreId = null;
            Store productionStore = null;
            List<DropDownList> storesProduction = new List<DropDownList>();
            List<DropDownList> storesProductionUnder = new List<DropDownList>();
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (Guid.TryParse(storeProductionSetting.SValue, out Guid productionStoreId))
            {
                productionStore = db.Stores.Where(x => x.Id == productionStoreId).FirstOrDefault();
                if (branches.Any(x=>x.Id==productionStore.BranchId))
                    storesProduction = EmployeeService.GetStoresByUser(productionStore.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                else
                    ViewBag.Msg = "ليس لك صلاحية على مخزن التصنيع الداخلى";
            }
            else
                ViewBag.Msg = "تأكد من اختيار مخزن التصنيع الداخلى أولا من شاشة الاعدادات العامة";
            //مخازن المستخدم
            vm.ProductionStoreId = productionStoreId;
            ViewBag.ProductionStoreId = new SelectList(storesProduction, "Id","Name",productionStoreId);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", productionStore!=null? productionStore.BranchId:null);
            ViewBag.ProductionLineId = new SelectList(db.ProductionLines.Where(x=>!x.IsDeleted), "Id", "Name");
            //التاكد من تحديد مخزن تحت التصنيع من الاعدادات اولا 
            var storeProductionUnderSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreUnderProductionId).FirstOrDefault();
            //Guid? productionUnderStoreId = null;
            Store productionUnderStore = null;
            if (Guid.TryParse(storeProductionUnderSetting.SValue, out Guid productionUnderStoreId))
            {
                productionUnderStore = db.Stores.Where(x => x.Id == productionUnderStoreId).FirstOrDefault();
                if (branches.Any(x => x.Id == productionUnderStore.BranchId))
                    storesProductionUnder = EmployeeService.GetStoresByUser(productionUnderStore.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                else
                    ViewBag.Msg = "ليس لك صلاحية على مخزن تحت التصنيع ";

            }
            else
                ViewBag.Msg = "تأكد من اختيار مخزن تحت التصنيع أولا من شاشة الاعدادات العامة";

            //ViewBag.ProductionUnderStoreId = productionUnderStoreId;
            //ViewBag.ProductionUnderStoreName = productionUnderStore.Name;
            ViewBag.ProductionUnderStoreId = new SelectList(storesProductionUnder, "Id", "Name", productionUnderStoreId);

            //التاكد من تحديد احتساب تكلفة المنتج من الاعدادات اولا 
            var itemCostSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault();
            int? itemCostId = null;
            if (!string.IsNullOrEmpty(itemCostSetting.SValue))
                itemCostId = int.Parse(itemCostSetting.SValue);
            else
                ViewBag.Msg = "تأكد من اختيار طريقة احتساب تكلفة المنتج أولا من شاشة الاعدادات العامة";
            ViewBag.ItemCostCalculateId = itemCostId;


            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();

            ViewBag.ItemProductionId = new SelectList(db.ItemProductions.Where(x=>!x.IsDeleted), "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");

            //قبول اضافة صنف بدون رصيد
            int itemAcceptNoBalance = 0;
            var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
            if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                ViewBag.ItemAcceptNoBalance = itemAcceptNoBalance;

            // add
            DSItemsIn = JsonConvert.SerializeObject(new List<ItemProductionOrderDetailsDT>());
                DSExpenses = JsonConvert.SerializeObject(new List<InvoiceExpensesDT>());

            vm.ProductionOrderDate = Utility.GetDateTime();
                return View(vm);
        }
        [HttpPost]
        public JsonResult RegisterOrder(ProductionOrder vm,double orderQuantity, int? ItemCostCalculateId, string DT_DatasourceItemIn, string DT_DatasourceExpenses, string DT_DatasourceItemOut)
          {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.BranchId == null || vm.ProductionStoreId == null|| vm.ProductionUnderStoreId == null /*|| vm.FinalItemId == null|| vm.OrderQuantity == 0 */ || vm.ProductionOrderDate == null|| ItemCostCalculateId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    var checkdate = closedPeriodServices.IsINPeriod(vm.ProductionOrderDate.ToString());
                    if (!checkdate)
                    {
                        return Json(new { isValid = false, message = "تاريخ المعاملة خارج فترة التشغيل " });

                    }
                    //المواد الداخلة
                    List<ItemProductionOrderDetailsDT> itemInDT = new List<ItemProductionOrderDetailsDT>();
                        List<ItemProductionOrderDetailsDT> itemOutDT = new List<ItemProductionOrderDetailsDT>();
                    List<ProductionOrderDetail> productionOrderDetail = null;
                    //قبول اضافة صنف بدون رصيد
                    int itemAcceptNoBalance = 0;
                    var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                    if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))

                    if (DT_DatasourceItemIn != null && DT_DatasourceItemOut != null)
                    {
                        itemInDT = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DT_DatasourceItemIn);
                        if (itemInDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف داخل واحد على الاقل" });
                        itemOutDT = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DT_DatasourceItemOut);
                        if (itemOutDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من وجود اصناف خارجة" });
                        if (itemInDT.Any(x=>!x.IsAllQuantityDone))
                        {
                                if(itemAcceptNoBalance==0)
                                    return Json(new { isValid = false, message = "تأكد من وجود ارصدة تكفى فى مخزن تحت التصنيع " });
                        }
                             var proInOut = itemInDT.Select(x =>
                              new ProductionOrderDetail
                              {
                                  ItemProductionId=x.ItemProductionId,
                                  ProductionTypeId=(int)ProductionTypeCl.In,
                                  ItemId = x.ItemId,
                                  Quantity = x.QuantityRequired??0,
                                  ItemCost=x.ItemCost??0,
                                  ItemCostCalculateId=x.ItemCostCalculateId
                              }).ToList();    
                                var proOut = itemOutDT.Select(x =>
                              new ProductionOrderDetail
                              {
                                  ItemProductionId=x.ItemProductionId,
                                  ProductionTypeId=(int)ProductionTypeCl.Out,
                                  ItemId = x.ItemId,
                                  Quantity = x.QuantityRequired ?? 0,
                                  ItemCost = x.ItemCost??0,
                                  ItemCostCalculateId=x.ItemCostCalculateId
                              }).ToList();
                                proInOut.AddRange(proOut);
                                productionOrderDetail= proInOut;
                        
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من الضغط على إنشاء توليفة ومن وجود اصناف (داخلة/خارجة)" });

                    //المصروفات
                    List<InvoiceExpensesDT> productionOrderExpensesDT = new List<InvoiceExpensesDT>();
                    List<ProductionOrderExpens> productionOrderExpens = new List<ProductionOrderExpens>();

                    if (DT_DatasourceExpenses != null)
                    {
                        productionOrderExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        productionOrderExpens = productionOrderExpensesDT.Select(
                            x => new ProductionOrderExpens
                            {
                                ExpenseTypeAccountTreeId = x.ExpenseTypeId,
                                AccountTreeCreditId= x.AccountTreeCreditId,
                                Amount = x.ExpenseAmount,
                                Note=x.Notes
                            }
                            ).ToList();
                    }
                    //ProductionOrder model = null;
                    //    model = vm;

                    // تسجيل قيود تكاليف امر الانتاج


                    using (var context = new VTSaleEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //تسجيل امر الانتاج
                                vm.ProductionOrderDate = vm.ProductionOrderDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                //اضافة رقم الامر
                                string codePrefix = Properties.Settings.Default.CodePrefix;
                                vm.OrderNumber = codePrefix + (context.ProductionOrders.Count(x => x.OrderNumber.StartsWith(codePrefix)) + 1);
                               //انشاء الباركود
                                string barcode;
                                generate:
                                    barcode = GeneratBarcodes.GenerateRandomBarcode();
                                    var isExistInItems = context.ProductionOrders.Where(x => x.OrderBarCode == barcode).Any();
                                    if (isExistInItems)
                                        goto generate;
                       
                                    vm.OrderBarCode=barcode;

                                // احتساب كل التكاليف (منتج واحد او اجمالى وتكلفة المواد الخام
                                //List<ItemsMaterialDT> itemMaterials = itemInDT.Select(x => new ItemsMaterialDT
                                //{
                                //    Quantity=x.QuantityRequired??0,
                                //    ItemCost=x.ItemCost
                                //}).ToList();
                                var GetProductionOrderCosts = ProductionOrderService.GetProductionOrderCost(null, productionOrderDetail, productionOrderExpensesDT);
                                //var GetProductionOrderCost2s = ProductionOrderService.GetProductionOrderCosts(null, itemMaterials, productionOrderExpensesDT,0/*vm.OrderQuantity*/);
                                if (!GetProductionOrderCosts.IsValid)
                                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                                //vm.MaterialItemCost = GetProductionOrderCosts.MaterialItemCost;
                                //vm.TotalExpenseCost = GetProductionOrderCosts.OrderExpenseCost;
                                vm.TotalCost = GetProductionOrderCosts.TotalCost;
                                foreach (var item in productionOrderDetail)
                                {
                                    if(item.ProductionTypeId == (int)ProductionTypeCl.Out)
                                        item.ItemCost = GetProductionOrderCosts.FinalItemCost;
                                }
                                vm.ProductionOrderDetails = productionOrderDetail;
                                vm.ProductionOrderExpenses = productionOrderExpens;

                                //vm.FinalItemCost = GetProductionOrderCosts.FinalItemCost;
                                context.ProductionOrders.Add(vm);
                                context.SaveChanges(auth.CookieValues.UserId);

                                // المصروفات (مدين
                                foreach (var expense in productionOrderExpens)
                                {
                                        //var expressAccountsTreeId = db.ExpenseTypes.Where(x => x.Id == expense.ExpenseTypeId).FirstOrDefault().AccountsTreeId;
                                        var expressAccountsTreeId = expense.ExpenseTypeAccountTreeId;
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(expressAccountsTreeId))
                                            return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب تشغيلى" });

                                        var creditAccountsTreeId = expense.AccountTreeCreditId;
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(creditAccountsTreeId))
                                            return Json(new { isValid = false, message = "الحساب الدائن ليس بحساب تشغيلى" });

                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = expressAccountsTreeId,
                                        BranchId = vm.BranchId,
                                        Debit = expense.Amount,
                                        Notes = $"تكاليف أمر إنتاج رقم : {vm.OrderNumber}",
                                        TransactionDate = vm.ProductionOrderDate,
                                        TransactionId = vm.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.ProductionOrderExpenses
                                    });
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = creditAccountsTreeId,
                                        BranchId = vm.BranchId,
                                        Credit = expense.Amount,
                                        Notes = $"تكاليف أمر إنتاج رقم : {vm.OrderNumber}",
                                        TransactionDate = vm.ProductionOrderDate,
                                        TransactionId = vm.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.ProductionOrderExpenses
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);
                                }
                                //اضافة يوم الانتاج فى جدول جدوله ايام الموظف 
                                if (vm.ProductionLineId!=null)
                                {
                                    //تحديد اذا كان يوجد موظف فى خط الانتاج يتم احتساب راتبه بالانتاج 
                                    var productionLine = context.ProductionLines.Where(x => !x.IsDeleted && x.Id == vm.ProductionLineId).FirstOrDefault();
                                    var proLinEmps = productionLine.ProductionLineEmployees.Where(e => !e.IsDeleted && e.CalculatingHours).ToList();
                                    foreach (var emp in proLinEmps)
                                    {
                                        //فى حالة العقد بالانتاج وولا يوجد اى عمليات غير مدفوعه للموظف 
                                        var contract = context.Contracts.Where(x => x.IsActive && x.EmployeeId == emp.EmployeeId).FirstOrDefault();

                                        if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Production)
                                        {
                                            //التأكد من عدم وجود التاريخ مجدول مسبقا
                                            var contractScheduling = context.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id&&!x.IsApproval&&!x.IsPayed).FirstOrDefault();
                                            if (contractScheduling == null)//لم يتم اضافة جدولة مدفوعا مسبقا
                                             //اضافة حقل فى جدول الجدولة 
                                            {
                                                contractScheduling = new ContractScheduling
                                                {
                                                    Contract = contract,
                                                    //MonthYear = vm.ProductionOrderDate,
                                                    //ToDate = vm.PenaltyDate,
                                                    Name = $"اوامر انتاج"
                                                };
                                                contractScheduling.ContractSchedulingProductions.Add(new ContractSchedulingProduction
                                                {
                                                    ProductionOrderId = vm.Id,
                                                });
                                            }
                                            else //تم انشاء حقل مسبقا ولم يتم الدفع بعد 
                                            {
                                                contractScheduling.ContractSchedulingProductions.Add(new ContractSchedulingProduction
                                                {
                                                    ProductionOrderId = vm.Id,
                                                });
                                            }
                                            context.ContractSchedulings.Add(contractScheduling);
                                            context.SaveChanges(auth.CookieValues.UserId);

                                        }else if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Daily)
                                        {

                                        }
                                    }
                                }
                                transaction.Commit();
                                return Json(new { isValid = true, refGid = vm.Id, message = "تم إضافة أمر الإنتاج بنجاح" });

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
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }

        #endregion

        #region حذف امر انتاج
        [HttpPost]
        public ActionResult Delete(string ordrGud)
        {
            Guid Id;
            if (Guid.TryParse(ordrGud, out Id))
            {
                var model = db.ProductionOrders.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    var materials = db.ProductionOrderDetails.Where(x => x.ProductionOrderId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var material in materials)
                    {
                        material.IsDeleted = true;
                        db.Entry(material).State = EntityState.Modified;
                    }

                    var expenses = db.ProductionOrderExpenses.Where(x => x.ProductionOrderId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }              
                    
                    var receipts = db.ProductionOrderReceipts.Where(x => x.ProductionOrderId == model.Id).ToList();
                    foreach (var receipt in receipts)
                    {
                        receipt.IsDeleted = true;
                        db.Entry(receipt).State = EntityState.Modified;
                    }

                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id).ToList();
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

        #region wizard step 2 اضافة الاصناف الداخلة 
        public ActionResult GetDSItemIn(string itemProductionId, double? quantity, string isFirstInit, Guid? storeUnderId,int? ItemCostCalculateId)
        {
            int? n = null;
            Guid itemFinalID = Guid.Empty;
            // تم اضافة المواد الخام بكمياتها وتحديث الجريد 
            if (isFirstInit != null)
            {
                if (int.Parse(isFirstInit) == 0) // عرض التوليفة الاساسية من الداتابيس فى اول مرة لانشاءها 
                {
                    if (Guid.TryParse(itemProductionId, out Guid ItemProductionId) && storeUnderId != Guid.Empty)
                    {
                        var itemsIn = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.In && x.ItemProductionId == ItemProductionId);
                        if (itemsIn.Count() > 0)
                        {
                            var itemsMaterials = itemsIn.ToList().Select(x => new ItemProductionOrderDetailsDT
                            { 
                                ItemProductionId = x.ItemProductionId,
                                ItemId = x.ItemId,
                                ItemName = x.Item.Name,
                                QuantityRequired = x.Quantity * quantity,
                                QuantityAvailable = BalanceService.GetBalance(x.ItemId, storeUnderId),
                                ItemCost = _itemService.GetItemCostCalculation(ItemCostCalculateId ?? 0, x.ItemId ),
                                StoreUnderId = storeUnderId,
                                ItemCostCalculateId= ItemCostCalculateId,
                                IsAllQuantityDone= x.Quantity * quantity<= BalanceService.GetBalance(x.ItemId, storeUnderId)?true:false,
                                Actions = n
                            }).ToList();
                            DSItemsIn = JsonConvert.SerializeObject(itemsMaterials);
                            return Json(new
                            {
                                data = itemsMaterials
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                            {
                                data = new { }
                            }, JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new
                    {
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);
            }
                else
                    return Json(new
                    {
                        data = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DSItemsIn)
                    }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);


    }

    public ActionResult addOtherItemIn(ItemProductionOrderDetailsDT vm,double? balanceCurrentStore,int? itemAcceptNoBalance)
        {
            //if (finalItemId==vm.ItemId)
            //    return Json(new { isValid = false, msg = "لا يمكن اختيار الصنف للتصنيع والخام فى نفس الوقت" }, JsonRequestBehavior.AllowGet);

            if (balanceCurrentStore<vm.QuantityRequired)
            {
                if(itemAcceptNoBalance==0||itemAcceptNoBalance==null)
                return Json(new { isValid = false, msg = "رصيد المخزن لا يسمح بالكمية المطلوبة" }, JsonRequestBehavior.AllowGet);
            }

            List<ItemProductionOrderDetailsDT> deDSIn = new List<ItemProductionOrderDetailsDT>();
            // الاصناف الخام المدخلة
            if (vm.DT_DatasourceIn != null)
                deDSIn = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(vm.DT_DatasourceIn);
            // الاصناف الخام المدخلة
            List<ItemProductionOrderDetailsDT> deDSOut = new List<ItemProductionOrderDetailsDT>();
            if (vm.DT_DatasourceOut != null)
                deDSOut = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(vm.DT_DatasourceOut);
            string itemName = "";
            string storeName = "";
            string ItemCostCalculateName = "";
            if (vm.ItemId != null)
            {
                if (deDSOut.Where(x => x.ItemId == vm.ItemId).Any())
                    return Json(new { isValid = false, msg = "الصنف فى الاصناف الخارجة لايمكن اضافته فى الاصناف الداخلة " }, JsonRequestBehavior.AllowGet);
                if (deDSIn.Where(x => x.ItemId == vm.ItemId).Any())
                    return Json(new { isValid = false, msg = "تم اضافة نفس الصنف مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            
            if (vm.StoreUnderId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreUnderId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من تحديد مخزن تحت التصنيع من الاعدادات " }, JsonRequestBehavior.AllowGet);

            if (vm.ItemCostCalculateId != null)
                ItemCostCalculateName = db.ItemCostCalculations.FirstOrDefault(x=>x.Id==vm.ItemCostCalculateId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار طريقة احتساب التكلفة من الاعدادات " }, JsonRequestBehavior.AllowGet);

            double quantityAvailable = BalanceService.GetBalance(vm.ItemId, vm.StoreUnderId);

            var newItemMaterials = new ItemProductionOrderDetailsDT { ItemId = vm.ItemId, ItemName = itemName,ItemCost=vm.ItemCost,  ItemCostCalculateId = vm.ItemCostCalculateId , StoreUnderId = vm.StoreUnderId,QuantityRequired=vm.QuantityRequired, QuantityAvailable=quantityAvailable,IsAllQuantityDone=quantityAvailable>=vm.QuantityRequired ? true:false};
            deDSIn.Add(newItemMaterials);
            DSItemsIn = JsonConvert.SerializeObject(deDSIn);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح "  }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region wizard step 2 اضافة الاصناف الخارجة 
        public ActionResult GetDSItemOut(string itemProductionId, double? quantity, string isFirstInit, Guid? storeUnderId,int? ItemCostCalculateId)
        {
            int? n = null;
            Guid itemFinalID = Guid.Empty;
            // تم اضافة المواد الخام بكمياتها وتحديث الجريد 
            if (isFirstInit != null)
            {
                if (int.Parse(isFirstInit) == 0) // عرض التوليفة الاساسية من الداتابيس فى اول مرة لانشاءها 
                {
                    if (Guid.TryParse(itemProductionId, out Guid ItemProductionId) && storeUnderId != Guid.Empty)
                    {
                        var itemsOut = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.Out && x.ItemProductionId == ItemProductionId);
                        if (itemsOut .Count()>0)
                        {
                            var itemsMaterials = itemsOut.ToList().Select(x => new ItemProductionOrderDetailsDT
                            {
                                ItemProductionId = x.ItemProductionId,
                                ItemId = x.ItemId,
                                ItemName = x.Item.Name,
                                QuantityRequired = x.Quantity* quantity,
                                QuantityAvailable = BalanceService.GetBalance(x.ItemId, storeUnderId,null,null,null),
                                //ItemCost = _itemService.GetItemCostCalculation(ItemCostCalculateId ?? 0, x.ItemId ),
                                //StoreUnderId = storeUnderId,
                                //ItemCostCalculateId= ItemCostCalculateId,
                                //IsAllQuantityDone= x.Quantity * quantity<= BalanceService.GetBalance(x.ItemId, storeUnderId, null, null, null) ?true:false,
                                Actions = n
                            }).ToList();
                            DSItemsIn = JsonConvert.SerializeObject(itemsMaterials);
                            return Json(new
                            {
                                data = itemsMaterials
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new
                            {
                                data = new { }
                            }, JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(new
                    {
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);
            }
                else
                    return Json(new
                    {
                        data = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DSItemsIn)
                    }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);


    }

    public ActionResult AddItemOut(ItemProductionOrderDetailsDT vm,double? balanceCurrentStore, Guid? finalItemId,int? itemAcceptNoBalance)
        {
            if (finalItemId==vm.ItemId)
                return Json(new { isValid = false, msg = "لا يمكن اختيار الصنف للتصنيع والخام فى نفس الوقت" }, JsonRequestBehavior.AllowGet);

            if (balanceCurrentStore<vm.QuantityRequired)
            {
                if(itemAcceptNoBalance==0||itemAcceptNoBalance==null)
                return Json(new { isValid = false, msg = "رصيد المخزن لا يسمح بالكمية المطلوبة" }, JsonRequestBehavior.AllowGet);
            }

            List<ItemProductionOrderDetailsDT> deDSIn = new List<ItemProductionOrderDetailsDT>();
            // الاصناف الخام المدخلة
            if (vm.DT_DatasourceIn != null)
                deDSIn = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(vm.DT_DatasourceIn);

            // الاصناف الخام المدخلة
            List<ItemProductionOrderDetailsDT> deDSOut = new List<ItemProductionOrderDetailsDT>();
            if (vm.DT_DatasourceOut   != null)
                deDSOut = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(vm.DT_DatasourceOut);
            string itemName = "";
            string storeName = "";
            string ItemCostCalculateName = "";
            if (vm.ItemId != null)
            {
                if (deDSOut.Where(x => x.ItemId == vm.ItemId ).Any())
                    return Json(new { isValid = false, msg = "الصنف فى الاصناف الخارجة لايمكن اضافته فى الاصناف الداخلة " }, JsonRequestBehavior.AllowGet);
                if (deDSIn.Where(x => x.ItemId == vm.ItemId ).Any())
                    return Json(new { isValid = false, msg = "تم اضافة نفس الصنف مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreUnderId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreUnderId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من تحديد مخزن تحت التصنيع من الاعدادات " }, JsonRequestBehavior.AllowGet);

            if (vm.ItemCostCalculateId != null)
                ItemCostCalculateName = db.ItemCostCalculations.FirstOrDefault(x=>x.Id==vm.ItemCostCalculateId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار طريقة احتساب التكلفة من الاعدادات " }, JsonRequestBehavior.AllowGet);

            double quantityAvailable = BalanceService.GetBalance(vm.ItemId, vm.StoreUnderId);

            var newItemMaterials = new ItemProductionOrderDetailsDT { ItemId = vm.ItemId, ItemName = itemName,ItemCost=vm.ItemCost,  ItemCostCalculateId = vm.ItemCostCalculateId , StoreUnderId = vm.StoreUnderId,QuantityRequired=vm.QuantityRequired, QuantityAvailable=quantityAvailable,IsAllQuantityDone=quantityAvailable>=vm.QuantityRequired ? true:false};
            deDSIn.Add(newItemMaterials);
            DSItemsIn = JsonConvert.SerializeObject(deDSIn);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح "  }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 تسجيل مصروفات الفاتورة
        public ActionResult GetDStProductionOrderExpenses()
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

        public ActionResult AddProductionOrderExpenses(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            string expenseTypeName,accountCreditName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "المصروف المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                expenseTypeName = db.AccountsTrees.FirstOrDefault(x=>x.Id==vm.ExpenseTypeId).AccountName;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseTypeId))
                    return Json(new { isValid = false, msg = "حساب المصروف ليس بحساب تشغيلى" });
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار حساب المصروف " }, JsonRequestBehavior.AllowGet);
            if (vm.AccountTreeCreditId != null)
            {
                if (deDS.Where(x => x.AccountTreeCreditId == vm.AccountTreeCreditId).Count() > 0)
                    return Json(new { isValid = false, msg = "الحساب الدائن المحدده موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                accountCreditName = db.AccountsTrees.FirstOrDefault(x=>x.Id==vm.AccountTreeCreditId).AccountName;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeCreditId))
                    return Json(new { isValid = false, msg = "الحساب الدائن ليس بحساب تشغيلى" });
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الحساب الدائن " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, AccountTreeCreditName=accountCreditName, AccountTreeCreditId=vm.AccountTreeCreditId, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount ,Notes=vm.Notes};
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل التكلفة بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region عرض بيانات امر انتاج بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowProductionOrder(Guid? ordrGud)
        {
            if (ordrGud == null || ordrGud == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.ProductionOrders.Where(x => x.Id == ordrGud).FirstOrDefault();
            vm.ProductionOrderDetails = vm.ProductionOrderDetails.Where(x => !x.IsDeleted).ToList();
            vm.ProductionOrderExpenses = vm.ProductionOrderExpenses.Where(x => !x.IsDeleted).ToList();
            vm.ProductionOrderReceipts = vm.ProductionOrderReceipts.Where(x => !x.IsDeleted).ToList();
            if (vm.ProductionLineId != null)
                vm.ProductionLine = db.ProductionLines.Where(x => x.Id == vm.ProductionLineId).FirstOrDefault();
            return View(vm);
        }

        #endregion

        #region امر انتاج مجمع
        public ActionResult RegisterOrderComplex()
        {
            OrderSellVM vm = new OrderSellVM();

            // add

                    vm = GetData(vm);
            vm.ProductionOrderDate = Utility.GetDateTime();

            return View(vm);
        }
        [HttpPost]
        public ActionResult RegisterOrderComplex(OrderSellVM vm, string actionType)
        {
            // add

            vm = GetData(vm);
            //فى حالى الضغط على اضافة توليفة لامر الانتاج 
            if (actionType == "addProductionItem")
            {
                if (vm.QuantityItemProduction == 0)
                {
                    ViewBag.Msg = "تأكد من ادخال الكمية ";
                    return View(vm);
                }
                if (vm.ItemProductionId != null)
                {
                    if (vm.ItemProductionDetails.Where(x => x.ItemProductionId == vm.ItemProductionId).Count() > 0)
                    {
                        ViewBag.Msg = "التوليفة المحدده موجودة مسبقا ";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Msg = "تأكد من اختيار توليفة";
                    return View(vm);
                }
                var checkdate = closedPeriodServices.IsINPeriod(vm.ProductionOrderDate.ToString());
                if (!checkdate)
                {
                    return Json(new { isValid = false, message = "تاريخ المعاملة خارج فترة التشغيل " });

                }
                var itemProductionName = db.ItemProductions.Where(x => x.Id == vm.ItemProductionId).FirstOrDefault();
                vm.ItemProductionDetails.Add(new ItemProductionDetails
                {
                    ItemProductionId = vm.ItemProductionId,
                    ItemProductionName = itemProductionName != null ? itemProductionName.Name : null,
                    Quantity = vm.QuantityItemProduction
                });
                vm.QuantityItemProduction = 0;
                vm.ItemProductionId = null;
            }
            //فى حالة الضغط على زر حذف توليفة
            if (actionType == "deletItemProduction")
            {
                if (vm.ItemProductionDeleted != null)
                {
                    var item = vm.ItemProductionDetails.Where(x => x.ItemProductionId == vm.ItemProductionDeleted).FirstOrDefault();
                    vm.ItemProductionDetails.Remove(item);
                }
            }


                vm.OrderSellItems = vm.ItemProductionDetails
                     .Select(x => new OrderSellItemsDto
                     {
                         //Id = x.Id,
                         //ItemId = x.ItemId,
                         //ItemName = x.Item.Name,
                         Quantity = x.Quantity,
                         //Price = x.Price,
                         //Amount = x.Quantity * x.Price,
                         //CurrentBalance = BalanceService.GetBalance(x.ItemId, null, null),
                         ItemProductionId = x.ItemProductionId,
                         //ItemProductionList = ItemService.GetItemProduction(x.ItemId)
                     }).ToList();
                foreach (var item in vm.OrderSellItems)
                {
                    item.ItemProductionOrderDetailsIn.Clear();
                    item.ItemProductionOrderDetailsOut.Clear();
                }
                foreach (var itemOrder in vm.OrderSellItems)
                {
                    //الاصناف الداخلة 
                    var itemsIn = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.In && x.ItemProductionId == itemOrder.ItemProductionId);
                    if (itemsIn.Count() > 0)
                    {
                        itemOrder.ItemProductionOrderDetailsIn.AddRange(itemsIn.ToList().Select(x => new ItemProductionOrderDetailsDT
                        {
                            ItemProductionId = x.ItemProductionId,
                            ItemId = x.ItemId,
                            ItemName = x.Item.Name,
                            QuantityRequired = x.Quantity * itemOrder.Quantity,
                            QuantityAvailable = BalanceService.GetBalance(x.ItemId, vm.ProductionUnderStoreId),
                            ItemCost = _itemService.GetItemCostCalculation(vm.ItemCostCalculateId ?? 0, x.ItemId),
                            StoreUnderId = vm.ProductionUnderStoreId,
                            ItemCostCalculateId = vm.ItemCostCalculateId,
                            IsAllQuantityDone = x.Quantity * itemOrder.Quantity <= BalanceService.GetBalance(x.ItemId, vm.ProductionUnderStoreId) ? true : false,
                        }).ToList());
                    }
                    //الاصناف الخارجة
                    var itemsOut = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.Out && x.ItemProductionId == itemOrder.ItemProductionId);
                    if (itemsOut.Count() > 0)
                    {
                        itemOrder.ItemProductionOrderDetailsOut.AddRange(itemsOut.ToList().Select(x => new ItemProductionOrderDetailsDT
                        {
                            ItemProductionId = x.ItemProductionId,
                            ItemId = x.ItemId,
                            ItemName = x.Item.Name,
                            QuantityRequired = x.Quantity * itemOrder.Quantity,
                            QuantityAvailable = BalanceService.GetBalance(x.ItemId, vm.ProductionUnderStoreId, null, null, null),
                        }).ToList());
                    }



                }

            

            //فى حالى الضغط على اضافة تكلفة انتاج 
            if (actionType == "addExpense")
            {
                if (vm.ExpenseTypeId != null)
                {
                    if (vm.ProductionOrderExpens.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    {
                        ViewBag.Msg = "التكلفة المحدده موجود مسبقا ";
                        return View(vm);
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseTypeId))
                    {
                        ViewBag.Msg = "حساب المصروف ليس بحساب تشغيلى";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Msg = "تأكد من اختيار تكلفة";
                    return View(vm);
                }     
                if (vm.AccountTreeCreditId != null)
                {
                    if (vm.ProductionOrderExpens.Where(x => x.AccountTreeCreditId == vm.AccountTreeCreditId).Count() > 0)
                    {
                        ViewBag.Msg = "الحساب الدائن المحدده موجود مسبقا ";
                        return View(vm);
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeCreditId))
                    {
                        ViewBag.Msg = "الحساب الدائن ليس بحساب تشغيلى";
                        return View(vm);
                    }
                }
                else
                {
                    ViewBag.Msg = "تأكد من اختيار الحساب الدائن";
                    return View(vm);
                }

                vm.ProductionOrderExpens.Add(new InvoiceExpensesDT
                {
                    ExpenseAmount = vm.ExpenseAmount,
                    ExpenseTypeId = vm.ExpenseTypeId,
                    ExpenseTypeName = vm.ExpenseTypeId != null ? db.AccountsTrees.Where(x => x.Id == vm.ExpenseTypeId).FirstOrDefault()?.AccountName : null,
                    AccountTreeCreditId = vm.AccountTreeCreditId,
                    AccountTreeCreditName = vm.AccountTreeCreditId != null ? db.AccountsTrees.Where(x => x.Id == vm.AccountTreeCreditId).FirstOrDefault()?.AccountName : null,
                    Notes = vm.ExpenseNotes
                });
                vm.ExpenseAmount = 0;
                vm.ExpenseTypeId = null;
                vm.ExpenseNotes = null;
            }
            //فى حالة الضغط على زر حذف مصروف
            if (actionType == "deletExpense")
            {
                if (vm.ExpenseTypeIdDeleted != null)
                {
                    var item = vm.ProductionOrderExpens.Where(x => x.ExpenseTypeId == vm.ExpenseTypeIdDeleted).FirstOrDefault();
                    vm.ProductionOrderExpens.Remove(item);
                }
            }
            //فى حالة الضغط على تسجيل امر انتاج مجمع
            if (actionType == "save")
            {
                if (vm.BranchId == null || vm.ProductionStoreId == null || vm.ProductionUnderStoreId == null /*|| vm.FinalItemId == null|| vm.OrderQuantity == 0 */ || vm.ProductionOrderDate == null || vm.ItemCostCalculateId == null)
                {
                    ViewBag.Msg = "تأكد من تحديد البيانات بشكل صحيح";
                    return View(vm);
                }


                //المواد الداخلة
                List<ProductionOrderDetail> productionOrderDetail = null;
                //قبول اضافة صنف بدون رصيد
                int itemAcceptNoBalance = 0;
                var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))

                    if (vm.OrderSellItems.Any(x => x.ItemProductionOrderDetailsIn.Any()) || vm.OrderSellItems.Any(x => x.ItemProductionOrderDetailsOut.Any()))
                    {
                        if (vm.OrderSellItems.Any(x => x.ItemProductionOrderDetailsIn.Any(y => !y.IsAllQuantityDone)))
                        {
                            if (itemAcceptNoBalance == 0)
                            {
                                ViewBag.Msg = "تأكد من وجود ارصدة تكفى فى مخزن تحت التصنيع ";
                                return View(vm);
                            }
                        }

                        List<ProductionOrderDetail> proInOut = new List<ProductionOrderDetail>();
                        List<ProductionOrderDetail> proOut = new List<ProductionOrderDetail>();
                        for (int i = 0; i < vm.OrderSellItems.Count; i++)
                        {
                            var itemProductionOrderDetailsIn = vm.OrderSellItems[i].ItemProductionOrderDetailsIn.ToList();
                            proInOut.AddRange(itemProductionOrderDetailsIn.Select(x =>
                        new ProductionOrderDetail
                        {
                            ItemProductionId = x.ItemProductionId,
                            ProductionTypeId = (int)ProductionTypeCl.In,
                            ItemId = x.ItemId,
                            Quantity = x.QuantityRequired ?? 0,
                            ItemCost = x.ItemCost ?? 0,
                            ItemCostCalculateId = x.ItemCostCalculateId,
                            ComplexProductionIndex = i

                        }).ToList());

                            var itemProductionOrderDetailsOut = vm.OrderSellItems[i].ItemProductionOrderDetailsOut.ToList();
                            proOut.AddRange(itemProductionOrderDetailsOut.Select(x =>
                             new ProductionOrderDetail
                             {
                                 ItemProductionId = x.ItemProductionId,
                                 ProductionTypeId = (int)ProductionTypeCl.Out,
                                 ItemId = x.ItemId,
                                 Quantity = x.QuantityRequired ?? 0,
                                 ItemCost = x.ItemCost ?? 0,
                                 ItemCostCalculateId = x.ItemCostCalculateId,
                                 ComplexProductionIndex = i

                             }).ToList());
                        }

                        proInOut.AddRange(proOut);
                        productionOrderDetail = proInOut;

                    }
                    else
                    {
                        ViewBag.Msg = "تأكد من ادخال صنف خام واحد على الاقل";
                        return View(vm);
                    }

                //المصروفات
                List<ProductionOrderExpens> productionOrderExpens = new List<ProductionOrderExpens>();
                if (vm.ProductionOrderExpens.Count() > 0)
                {
                    productionOrderExpens = vm.ProductionOrderExpens.Select(
                        x => new ProductionOrderExpens
                        {
                            ExpenseTypeAccountTreeId = x.ExpenseTypeId,
                            AccountTreeCreditId = x.AccountTreeCreditId,
                            Amount = x.ExpenseAmount,
                            Note = x.Notes
                        }
                        ).ToList();
                }

                // تسجيل قيود تكاليف امر الانتاج


                using (var context = new VTSaleEntities())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            ProductionOrder productionOrder = new ProductionOrder();
                            productionOrder.BranchId = vm.BranchId;
                            productionOrder.Notes = vm.Notes;
                            productionOrder.OrderSellId = vm.QuoteOrderSellId;
                            productionOrder.ProductionStoreId = vm.ProductionStoreId;
                            productionOrder.ProductionUnderStoreId = vm.ProductionUnderStoreId;
                            productionOrder.ProductionOrderHours = vm.ProductionOrderHours;
                            productionOrder.ProductionLineId = vm.ProductionLineId;
                            //تسجيل امر الانتاج
                            productionOrder.ProductionOrderDate = vm.ProductionOrderDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                            //اضافة رقم الامر
                            string codePrefix = Properties.Settings.Default.CodePrefix;
                            productionOrder.OrderNumber = codePrefix + (context.ProductionOrders.Count(x => x.OrderNumber.StartsWith(codePrefix)) + 1);
                            //انشاء الباركود
                            string barcode;
                        generate:
                            barcode = GeneratBarcodes.GenerateRandomBarcode();
                            var isExistInItems = context.ProductionOrders.Where(x => x.OrderBarCode == barcode).Any();
                            if (isExistInItems)
                                goto generate;

                            productionOrder.OrderBarCode = barcode;
                            productionOrder.OrderSellId = vm.QuoteOrderSellId;
                            var GetProductionOrderCosts = ProductionOrderService.GetProductionOrderCost(null, productionOrderDetail, vm.ProductionOrderExpens);
                            if (!GetProductionOrderCosts.IsValid)
                                ViewBag.Msg = "حدث خطأ اثناء تنفيذ العملية";
                            productionOrder.TotalCost = GetProductionOrderCosts.TotalCost;
                            foreach (var item in productionOrderDetail)
                            {
                                if (item.ProductionTypeId == (int)ProductionTypeCl.Out)
                                    item.ItemCost = GetProductionOrderCosts.FinalItemCost;
                            }
                            productionOrder.ProductionOrderDetails = productionOrderDetail;
                            productionOrder.ProductionOrderExpenses = productionOrderExpens;

                            //vm.FinalItemCost = GetProductionOrderCosts.FinalItemCost;
                            context.ProductionOrders.Add(productionOrder);
                            context.SaveChanges(auth.CookieValues.UserId);

                            // المصروفات (مدين
                            foreach (var expense in productionOrderExpens)
                            {
                                //var expressAccountsTreeId = db.ExpenseTypes.Where(x => x.Id == expense.ExpenseTypeId).FirstOrDefault().AccountsTreeId;
                                var expressAccountsTreeId = expense.ExpenseTypeAccountTreeId;
                                if (AccountTreeService.CheckAccountTreeIdHasChilds(expressAccountsTreeId))
                                    return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب تشغيلى" });

                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = expressAccountsTreeId,
                                    BranchId = productionOrder.BranchId,
                                    Debit = expense.Amount,
                                    Notes = $"تكاليف أمر إنتاج مجمع رقم : {productionOrder.OrderNumber}",
                                    TransactionDate = productionOrder.ProductionOrderDate,
                                    TransactionId = productionOrder.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.ProductionOrderExpenses
                                });
                                var creditAccountsTreeId = expense.AccountTreeCreditId;
                                if (AccountTreeService.CheckAccountTreeIdHasChilds(creditAccountsTreeId))
                                    return Json(new { isValid = false, message = "الحساب الدائن ليس بحساب تشغيلى" });

                                context.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = creditAccountsTreeId,
                                    BranchId = productionOrder.BranchId,
                                    Credit = expense.Amount,
                                    Notes = $"تكاليف أمر إنتاج مجمع رقم : {productionOrder.OrderNumber}",
                                    TransactionDate = productionOrder.ProductionOrderDate,
                                    TransactionId = productionOrder.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.ProductionOrderExpenses
                                });
                                context.SaveChanges(auth.CookieValues.UserId);
                            }

                            transaction.Commit();
                            ViewBag.MsgSuccess = "تم إضافة أمر الإنتاج بنجاح";
                            return View(vm);

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            ViewBag.Msg = "حدث خطأ اثناء تنفيذ العملية";
                            return View(vm);
                        }
                    }
                }

            }

            return View(vm);
        }


        private OrderSellVM GetData(OrderSellVM vm)
        {
            var branchId = vm.BranchId;
            Guid productionUnderStoreId ;
            Guid productionStoreId ;
            Store productionStore = null;
            List<DropDownList> storesProduction = new List<DropDownList>();
            List<DropDownList> storesProductionUnder = new List<DropDownList>();
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (vm.ProductionUnderStoreId == null)
            {
                //التاكد من تحديد مخزن تحت التصنيع من الاعدادات اولا 
                var storeProductionUnderSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreUnderProductionId).FirstOrDefault();
                Store productionUnderStore = null;
                if (Guid.TryParse(storeProductionUnderSetting.SValue, out productionUnderStoreId))
                {
                    productionUnderStore = db.Stores.Where(x => x.Id == productionUnderStoreId).FirstOrDefault();
                    if (branches.Any(x => x.Id == productionUnderStore.BranchId))
                        storesProductionUnder = EmployeeService.GetStoresByUser(productionUnderStore.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                    else
                        ViewBag.Msg = "ليس لك صلاحية على مخزن تحت التصنيع ";

                }
                else
                    ViewBag.Msg = "تأكد من اختيار مخزن تحت التصنيع أولا من شاشة الاعدادات العامة";
            }
            else
            {
               var productionUnderStore = db.Stores.Where(x => x.Id == vm.ProductionUnderStoreId).FirstOrDefault();
                if (branches.Any(x => x.Id == productionUnderStore.BranchId))
                    storesProductionUnder = EmployeeService.GetStoresByUser(productionUnderStore.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                else
                    ViewBag.Msg = "ليس لك صلاحية على مخزن تحت التصنيع ";

                productionUnderStoreId = vm.ProductionUnderStoreId ?? new Guid();

            }
            ViewBag.ProductionUnderStoreId = new SelectList(storesProductionUnder, "Id", "Name", productionUnderStoreId);

            if (vm.ProductionStoreId == null)
            {
                //التاكد من تحديد مخزن التصنيع الداخلى من الاعدادات اولا 
                var storeProductionSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault();
                if (Guid.TryParse(storeProductionSetting.SValue, out productionStoreId))
                {
                    productionStore = db.Stores.Where(x => x.Id == productionStoreId).FirstOrDefault();
                    if (branches.Any(x => x.Id == productionStore.BranchId))
                        storesProduction = EmployeeService.GetStoresByUser(productionStore.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                    else
                        ViewBag.Msg = "ليس لك صلاحية على مخزن التصنيع الداخلى";

                }
                else
                    ViewBag.Msg = "تأكد من اختيار مخزن التصنيع الداخلى أولا من شاشة الاعدادات العامة";
            } else
            {
                var productionStore2 = db.Stores.Where(x => x.Id == vm.ProductionStoreId).FirstOrDefault();
                if (branches.Any(x => x.Id == productionStore2.BranchId))
                    storesProduction = EmployeeService.GetStoresByUser(productionStore2.BranchId.ToString(), auth.CookieValues.UserId.ToString());
                else
                    ViewBag.Msg = "ليس لك صلاحية على مخزن التصنيع الداخلى";
                productionStoreId = vm.ProductionStoreId ?? new Guid();

            }
            ViewBag.ProductionStoreId = new SelectList(storesProduction, "Id", "Name", productionStoreId);

            //التاكد من تحديد احتساب تكلفة المنتج من الاعدادات اولا 
            if (vm.ItemCostCalculateId == null)
            {
                var itemCostSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault();
                if (!int.TryParse(itemCostSetting.SValue, out int itemCostId))
                    ViewBag.Msg = "تأكد من اختيار طريقة احتساب تكلفة المنتج أولا من شاشة الاعدادات العامة";
                vm.ItemCostCalculateId = itemCostId;
            }

            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.Branchcount = branches.Count();
            ViewBag.ProductionLineId = new SelectList(db.ProductionLines.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemProductionId = new SelectList(db.ItemProductions.Where(x => !x.IsDeleted), "Id", "Name");
            vm.BranchId = branchId;
            //vm.OrderSellItems.ForEach(x => x.ItemProductionList = ItemService.GetItemProduction(x.ItemId));

            return vm;
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