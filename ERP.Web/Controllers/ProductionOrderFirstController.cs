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
namespace ERP.Web.Controllers
{
    [Authorization] //الشاشة القديمة لامر الانتاج

    public class ProductionOrderFirstController : Controller
    {
        // GET: ProductionOrderFirst
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DSItemProduction { get; set; }
        public static string DSItemsMaterial { get; set; }
        //public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة أوامر الإنتاج
        public ActionResult Index()
        {
            ViewBag.FinalItemId = new SelectList(new List<Item>(), "Id", "Name");

            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ProductionOrders.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, OrderNumber = x.OrderNumber, ProductionOrderDate = x.ProductionOrderDate.ToString(), /*FinalItemName = x.FinalItem.ItemCode + "|" + x.FinalItem.Name, OrderQuantity = x.OrderQuantity,IsDone = x.IsDone, IsDoneTitle = x.IsDone ? "تم التسليم" : "لم يتم التسليم بعد",*/  Actions = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region تسجيل أمر الانتاج


        [HttpGet]
        public ActionResult RegisterOrder()
        {
            //التاكد من تحديد مخزن التصنيع الداخلى من الاعدادات اولا 
            var storeProductionSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault();
            Guid? productionStoreId = null;
            if (!string.IsNullOrEmpty(storeProductionSetting.SValue))
                productionStoreId = Guid.Parse(storeProductionSetting.SValue);
            else
                ViewBag.Msg = "تأكد من اختيار مخزن التصنيع الداخلى أولا من شاشة الاعدادات العامة";
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.FinalItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemCostCalculateId = new SelectList(db.ItemCostCalculations.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");


            // add
            DSItemsMaterial = JsonConvert.SerializeObject(new List<ItemsMaterialDT>());
            DSExpenses = JsonConvert.SerializeObject(new List<InvoiceExpensesDT>());

            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            //Random random = new Random();
            //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();

            var vm = new ProductionOrder();
            vm.ProductionStoreId = productionStoreId;
            ViewBag.ProductionStoreName = db.Stores.Where(x => x.Id == productionStoreId).FirstOrDefault().Name;
            vm.ProductionOrderDate = Utility.GetDateTime();
            return View(vm);
        }
        [HttpPost]
        public JsonResult RegisterOrder(ProductionOrder vm, string DT_DatasourceItemProduction, string DT_DatasourceItemMaterials, string DT_DatasourceExpenses)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.BranchId == null || vm.ProductionStoreId == null /*|| vm.FinalItemId == null || vm.OrderQuantity == 0*/ || vm.ProductionOrderDate == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    //المواد الخام التى تم توليفها 
                    List<ItemProductionOrderDetailsDT> itemProductionDT = new List<ItemProductionOrderDetailsDT>();

                    if (DT_DatasourceItemProduction != null)
                    {
                        itemProductionDT = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DT_DatasourceItemProduction);
                        if (itemProductionDT.Count() > 0)
                            if (itemProductionDT.Any(x => !x.IsAllQuantityDone))
                                return Json(new { isValid = false, message = "تأكد من ادخال كل اصناف التوليفة كاملا" });
                    }

                    //المواد الخام
                    List<ItemsMaterialDT> itemMaterialDT = new List<ItemsMaterialDT>();
                    List<ProductionOrderDetail> productionOrderDetail = new List<ProductionOrderDetail>();

                    if (DT_DatasourceItemMaterials != null)
                    {
                        itemMaterialDT = JsonConvert.DeserializeObject<List<ItemsMaterialDT>>(DT_DatasourceItemMaterials);
                        if (itemMaterialDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف خام واحد على الاقل" });
                        else
                        {
                            productionOrderDetail = itemMaterialDT.Select(x =>
                              new ProductionOrderDetail
                              {
                                  ItemId = x.ItemId,
                                  //StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  ItemCost = x.ItemCost??0,
                                  ItemCostCalculateId = x.ItemCostCalculateId
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف خام واحد على الاقل" });

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
                                Amount = x.ExpenseAmount,
                                Note = x.Notes
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
                                vm.OrderNumber = codePrefix + (db.ProductionOrders.Count(x => x.OrderNumber.StartsWith(codePrefix)) + 1);

                                vm.ProductionOrderDetails = productionOrderDetail;
                                vm.ProductionOrderExpenses = productionOrderExpens;
                                // احتساب كل التكاليف (منتج واحد او اجمالى وتكلفة المواد الخام
                                var GetProductionOrderCosts = ProductionOrderService.GetProductionOrderCosts(null, itemMaterialDT, productionOrderExpensesDT,0/*vm.OrderQuantity*/);
                                if (!GetProductionOrderCosts.IsValid)
                                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                                //vm.MaterialItemCost = GetProductionOrderCosts.MaterialItemCost;
                                //vm.TotalExpenseCost = GetProductionOrderCosts.OrderExpenseCost;
                                vm.TotalCost = GetProductionOrderCosts.TotalCost;
                                //vm.FinalItemCost = GetProductionOrderCosts.FinalItemCost;
                                context.ProductionOrders.Add(vm);
                                context.SaveChanges(auth.CookieValues.UserId);

                                // المصروفات (مدين
                                foreach (var expense in productionOrderExpens)
                                {
                                    var expressAccountsTreeId = db.ExpenseTypes.Where(x => x.Id == expense.ExpenseTypeAccountTreeId).FirstOrDefault().AccountsTreeId;
                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(expressAccountsTreeId))
                                        return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });

                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = expressAccountsTreeId,
                                        BranchId = vm.BranchId,
                                        Debit = expense.Amount,
                                        Notes = $"تكاليف أمر إنتاج رقم : {vm.Id}",
                                        TransactionDate = vm.ProductionOrderDate,
                                        TransactionId = vm.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.ProductionOrderExpenses
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);
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

        #region wizard step 2 اظهار توليفة الصنف النهائى 
        public ActionResult GetDSItemProduction(string itemFinalId, double? quantity, Guid? branchId, string isFirstInit)
        {
            int? n = null;
            Guid itemFinalID = Guid.Empty;
            // تم اضافة المواد الخام بكمياتها وتحديث الجريد 
            if (isFirstInit != null)
            {
                if (Guid.Parse(isFirstInit) == Guid.Empty) // عرض التوليفة الاساسية من الداتابيس فى اول مرة لانشاءها 
                {
                    if (Guid.TryParse(itemFinalId, out itemFinalID) && branchId != Guid.Empty)
                    {
                        var itemPro = db.ItemProductions.Where(x => !x.IsDeleted /*&& x.ItemFinalId == itemFinalID*/).OrderByDescending(x => x.Id).FirstOrDefault();
                        if (itemPro != null)
                        {
                            var itemsMaterials = itemPro.ItemProductionDetails.Where(x => !x.IsDeleted).Select(x => new ItemProductionOrderDetailsDT
                            {
                                ItemProductionId = x.ItemProductionId,
                                ItemId = x.ItemId,
                                ItemName = x.Item.Name,
                                QuantityRequired = x.Quantity * quantity,
                                QuantityAvailable = BalanceService.GetBalance(x.ItemId, null, branchId),
                                Actions = n
                            }).ToList();
                            DSItemProduction = JsonConvert.SerializeObject(itemsMaterials);
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
                        data = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DSItemProduction)
                    }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);




        }

        [HttpPost]
        public ActionResult UpdateItemProduction(string itemId, string DT_DatasourceItemProduction, string DT_DatasourceMaterial)
        {
            Guid itemMaterialId;
            bool updateItemOrderDT = false;
            if (Guid.TryParse(itemId, out itemMaterialId) && !string.IsNullOrEmpty(DT_DatasourceItemProduction) && !string.IsNullOrEmpty(DT_DatasourceMaterial))
            {
                DSItemsMaterial = DT_DatasourceMaterial;

                List<ItemProductionOrderDetailsDT> deDSItemProduction = new List<ItemProductionOrderDetailsDT>();
                deDSItemProduction = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DT_DatasourceItemProduction);
                if (deDSItemProduction.Any(x => x.ItemId == itemMaterialId))
                {
                    foreach (var item in deDSItemProduction.Where(x => x.ItemId == itemMaterialId))
                    {
                        item.IsAllQuantityDone = false;
                        updateItemOrderDT = true;
                    }
                    DSItemProduction = JsonConvert.SerializeObject(deDSItemProduction);
                }
                return Json(new { isValid = true, message = "تم الحذف بنجاح", updateItemOrderDT });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        #endregion

        #region wizard step 2 اضافة المواد الخام 
        public ActionResult GetDSItemMaterials()
        {
            int? n = null;
            if (DSItemsMaterial == null)
                return Json(new
                {
                    data = new ItemsMaterialDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemsMaterialDT>>(DSItemsMaterial)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemMaterials(ItemsMaterialDT vm, double? orderQuantity, Guid? finalItemId, string DT_DatasourceItemProduction)
        {
            if (finalItemId == vm.ItemId)
                return Json(new { isValid = false, msg = "لا يمكن اختيار الصنف للتصنيع والخام فى نفس الوقت" }, JsonRequestBehavior.AllowGet);

            List<ItemProductionOrderDetailsDT> deDSItemProduction = new List<ItemProductionOrderDetailsDT>();
            List<ItemsMaterialDT> deDSMaterials = new List<ItemsMaterialDT>();
            string itemName = "";
            string storeName = "";
            string ItemCostCalculateName = "";
            if (vm.ItemId != null)
            {
                if (deDSMaterials.Where(x => x.ItemId == vm.ItemId && x.StoreId == vm.StoreId).Count() > 0)
                    return Json(new { isValid = false, msg = "تم اضافة نفس الصنف من نفس المخزن مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            if (vm.ItemCostCalculateId != null)
                ItemCostCalculateName = db.ItemCostCalculations.FirstOrDefault(x=>x.Id==vm.ItemCostCalculateId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار طريقة احتساب التكلفة " }, JsonRequestBehavior.AllowGet);


            bool isAllQuantityDone = false;
            ItemProductionOrderDetailsDT itemExistInProduction = null;

            // الاصناف الخام
            if (vm.DT_DatasourceMaterial != null)
                deDSMaterials = JsonConvert.DeserializeObject<List<ItemsMaterialDT>>(vm.DT_DatasourceMaterial);

            //اصناف التوليفة 
            if (DT_DatasourceItemProduction != null)
            {
                deDSItemProduction = JsonConvert.DeserializeObject<List<ItemProductionOrderDetailsDT>>(DT_DatasourceItemProduction);
                var quantityMaterials = deDSMaterials.Where(x => x.ItemId == vm.ItemId).Sum(x => x.Quantity);
                itemExistInProduction = deDSItemProduction.Where(x => x.ItemId == vm.ItemId).FirstOrDefault();
                if (itemExistInProduction != null)
                {
                    if (quantityMaterials == deDSItemProduction.Where(x => x.ItemId == vm.ItemId).FirstOrDefault().QuantityRequired)
                        return Json(new { isValid = false, msg = "تم ادخال الكمية المطلوبة بالفعل ولا يمكن اضافه المزيد" }, JsonRequestBehavior.AllowGet);

                }
            }
            else
                return Json(new { isValid = false, msg = "تأكد من ادخال الكمية المطلوب انتاجها او توليفه الصنف المنتج " }, JsonRequestBehavior.AllowGet);


            if (orderQuantity != null && orderQuantity > 0 && finalItemId != null && finalItemId != Guid.Empty)
            {
                var itemPro = db.ItemProductions.Where(x => !x.IsDeleted /*&& x.ItemFinalId == finalItemId*/).OrderByDescending(x => x.Id).FirstOrDefault();
                if (itemPro != null)
                {
                    var itemsMaterials = itemPro.ItemProductionDetails.Where(x => !x.IsDeleted && x.ItemId == vm.ItemId).Select(x => new { QuantityRequired = x.Quantity * orderQuantity, QuantityAvailable = BalanceService.GetBalance(vm.ItemId, vm.StoreId) }).FirstOrDefault();
                    if (itemsMaterials != null)
                        if (itemsMaterials.QuantityRequired > itemsMaterials.QuantityAvailable)
                            return Json(new { isValid = false, msg = "الكمية المطلوبة اكبر من الكمية المتاحة فى المخزن" }, JsonRequestBehavior.AllowGet);
                    //if (vm.Quantity>itemsMaterials.QuantityRequired)
                    //    return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من الكمية المطلوبة" }, JsonRequestBehavior.AllowGet);

                    if (itemExistInProduction != null)
                    {
                        if ((vm.Quantity + deDSMaterials.Where(x => x.ItemId == vm.ItemId).Sum(x => x.Quantity)) > (itemExistInProduction.QuantityRequired))
                            return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من المطلوب فى التوليفة" }, JsonRequestBehavior.AllowGet);

                        if ((vm.Quantity + deDSMaterials.Where(x => x.ItemId == vm.ItemId).Sum(x => x.Quantity)) == (itemExistInProduction.QuantityRequired))
                        {
                            foreach (var item in deDSItemProduction.Where(x => x.ItemId == vm.ItemId))
                            {
                                item.IsAllQuantityDone = true;
                                isAllQuantityDone = true;
                            }
                            DSItemProduction = JsonConvert.SerializeObject(deDSItemProduction);
                        }

                    }

                }
            }




            var newItemMaterials = new ItemsMaterialDT { ItemId = vm.ItemId, ItemName = itemName, ItemCost = vm.ItemCost, Quantity = vm.Quantity, ItemCostCalculateId = vm.ItemCostCalculateId, ItemCostCalculateName = ItemCostCalculateName, StoreId = vm.StoreId, StoreName = storeName };
            deDSMaterials.Add(newItemMaterials);
            DSItemsMaterial = JsonConvert.SerializeObject(deDSMaterials);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", isAllQuantityDone }, JsonRequestBehavior.AllowGet);
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
            string expenseTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "التكلفة المحدده موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                expenseTypeName = db.ExpenseTypes.FirstOrDefault(x=>x.Id==vm.ExpenseTypeId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار تكلفة " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount, Notes = vm.Notes };
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
            return View(vm);
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