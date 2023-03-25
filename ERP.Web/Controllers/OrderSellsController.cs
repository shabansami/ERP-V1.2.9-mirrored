using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.ViewModels;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class OrderSellsController : Controller
    {
        // GET: OrderSells
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        ItemService _itemService;
        public OrderSellsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            _itemService = new ItemService();
        }
        //public static string DS { get; set; }

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

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.QuoteOrderSells.Where(x => !x.IsDeleted && x.QuoteOrderSellType == (int)QuoteOrderSellTypeCl.OrderSell);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);
            if (customerId != null)
                list = list.Where(x => x.CustomerId == customerId);

            var items = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, OrderSellInvoiceNumber = x.InvoiceNumber, QuoteInvoiceNumber = x.Quote.InvoiceNumber, CustomerName = x.Customer.Name, InvoiceDate = x.InvoiceDate.ToString(), TotalValue = x.TotalValue,RegSell=x.SellInvoices.Where(s=>!s.IsDeleted).Any(),RegOrderProduction=x.ProductionOrders.Where(p=>!p.IsDeleted).Any(), Actions = n, Num = n }).ToList();
            return Json(new
            {
                data = items
            }, JsonRequestBehavior.AllowGet);

        }

        #region اضافة/حذف امر بيع 
        [HttpGet]
        public ActionResult CreateEdit(Guid? quoteId)
        {
            //DS = null;
            OrderSellVM vm = new OrderSellVM();
            Guid? customerId = null;
            Guid? branchId = null;
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == id);
                    vm.CustomerId = model?.CustomerId;
                    vm.BranchId = model?.BranchId;
                    vm.OrderSellItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted)
                        .Select(x => new OrderSellItemsDto
                        {
                            Id = x.Id,
                            ItemId = x.ItemId,
                            ItemName = x.Item.Name,
                            Quantity = x.Quantity,
                            Price = x.Price,
                            Amount = x.Quantity * x.Price,
                            OrderSellItemType = x.OrderSellItemType,
                            CurrentBalance = BalanceService.GetBalance(x.ItemId, null, vm.BranchId)
                        }).ToList();
                    vm.InvoiceDate = model.InvoiceDate;
                    vm.Notes = model.Notes;
                    vm.Id = model.Id;
                    branchId = model.BranchId;
                    customerId = model.CustomerId;

                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                if (quoteId != null && quoteId != Guid.Empty)
                {
                    var quote = db.QuoteOrderSells.Where(x => x.Id == quoteId).FirstOrDefault();
                    if (quote != null)
                    {
                        branchId = quote.BranchId;
                        customerId = quote.CustomerId;
                        vm.OrderSellItems = quote.QuoteOrderSellDetails.Where(x => !x.IsDeleted)
                             .Select(x => new OrderSellItemsDto
                             {
                                 Id = x.Id,
                                 ItemId = x.ItemId,
                                 ItemName = x.Item.Name,
                                 Quantity = x.Quantity,
                                 Price = x.Price,
                                 Amount = x.Quantity * x.Price,
                                 CurrentBalance = BalanceService.GetBalance(x.ItemId, null, vm.BranchId)
                             }).ToList();
                        vm.InvoiceDate = quote.InvoiceDate;
                        vm.QuoteOrderSellId = quote.Id;

                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            //نوع اصناف امر البيع (اصناف للبيع/اصناف للانتاج)
            //ViewBag.OrderSellItemType = new List<SelectListItem> { new SelectListItem { Text = "صنف بيع", Value = "1", Selected = true }, new SelectListItem { Text = "صنف انتاج", Value = "2" } };
            ViewBag.DropDownListSellItemType = new List<DropDownListInt> { new DropDownListInt { Id = 1, Name = "صنف بيع" }, new DropDownListInt { Id = 2, Name = "صنف انتاج" } };
            vm.BranchId = branchId;
            vm.CustomerId = customerId;
            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(OrderSellVM vm)
        {
            if (ModelState.IsValid)
            {
                DateTime invoiceDate;
                if (vm.InvoiceDate == null || vm.CustomerId == null || vm.BranchId == null)
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                invoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                //الاصناف
                List<QuoteOrderSellDetail> items = new List<QuoteOrderSellDetail>();
                items = vm.OrderSellItems.Select(x => new QuoteOrderSellDetail
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    OrderSellItemType = x.OrderSellItemType,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Quantity * x.Price,
                }).ToList();
                if (vm.OrderSellItems.Count() == 0)
                    return Json(new { isValid = false, message = "تأكد من وجود صنف واحد على الاقل" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //تحديث اى اصناف تم تسجيلها مسباق 

                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == vm.Id);
                    var currentItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in currentItems)
                    {
                        var t = items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (t != null)
                        {
                            item.OrderSellItemType = t.OrderSellItemType;
                            item.Quantity = t.Quantity;
                            item.Price = t.Price;
                            item.Amount = t.Quantity * t.Price;

                        }

                    }

                    model.InvoiceDate = invoiceDate;
                    model.Notes = vm.Notes;
                    model.TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0);
                    model.TotalValue = items.Sum(x => (double?)x.Amount ?? 0);
                    //model.QuoteOrderSellDetails = items;
                    //db.Entry(model).State = EntityState.Modified;
                }
                else
                {

                    isInsert = true;
                    //اضافة رقم الفاتورة
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    QuoteOrderSell quoteOrderSell = new QuoteOrderSell
                    {
                        InvoiceNumber = codePrefix + (db.QuoteOrderSells.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1),
                        InvoiceDate = invoiceDate,
                        TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0),
                        TotalValue = items.Sum(x => (double?)x.Amount ?? 0),
                        QuoteOrderSellType = (int)QuoteOrderSellTypeCl.OrderSell,
                        BranchId = vm.BranchId,
                        CustomerId = vm.CustomerId,
                        QuoteId = vm.QuoteOrderSellId,
                        Notes = vm.Notes,
                    };

                    quoteOrderSell.QuoteOrderSellDetails = items;
                    db.QuoteOrderSells.Add(quoteOrderSell);

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
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        [Authorization]
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
                var model = db.QuoteOrderSells.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف الاصناف
                    var items = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in items)
                    {
                        item.IsDeleted = true;
                        //db.Entry(item).State = EntityState.Modified;
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

        #region تجهيز لفاتورة بيع 
        [HttpGet]
        public ActionResult OrderForSell(Guid? orderId)
        {
            //DS = null;
            OrderSellVM vm = new OrderSellVM();
            Guid? customerId = null;
            Guid? branchId = null;
                 // add
                if (orderId != null&&orderId!=Guid.Empty)
                {
                    var orderSell = db.QuoteOrderSells.Where(x => x.Id == orderId).FirstOrDefault();
                    if (orderSell != null)
                    {
                        branchId = orderSell.BranchId;
                        customerId = orderSell.CustomerId;
                    vm.InvoiceDate = orderSell.InvoiceDate;
                    vm.QuoteOrderSellId = orderSell.Id;
                    vm.OrderSellItems = orderSell.QuoteOrderSellDetails.Where(x => !x.IsDeleted&&x.OrderSellItemType==(int)OrderSellItemTypeCl.Sell)
                             .Select(x => new OrderSellItemsDto
                             {
                                 Id = x.Id,
                                 ItemId = x.ItemId,
                                 ItemName = x.Item.Name,
                                 Quantity = x.Quantity,
                                 Price = x.Price,
                                 Amount = x.Quantity * x.Price,
                                 CurrentBalance = x.StoreId!=null? BalanceService.GetBalance(x.ItemId, x.StoreId, null):0,
                                 StoreId=x.StoreId,
                                 StoreItemList=db.Stores.Where(y => !y.IsDeleted && y.BranchId == branchId && !y.IsDamages).Select(y=>new DropDownList { Id=y.Id,Name=y.Name}).ToList()
                             }).ToList();

                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            vm.BranchId = branchId;
            vm.CustomerId = customerId;
            return View(vm);
        }
        [HttpPost]
        public JsonResult OrderForSell(OrderSellVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.InvoiceDate == null || vm.CustomerId == null || vm.BranchId == null)
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (vm.OrderSellItems.Where(x=>x.Quantity>x.CurrentBalance).Any())
                {
                    //قبول اضافة صنف بدون رصيد
                    int itemAcceptNoBalance = 0;
                    var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                    if (!int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                        return Json(new { isValid = false, message = "تأكد من تحديد قبول اصناف بدون رصيد من الاعدادات" });
                    if (itemAcceptNoBalance == 0)
                        return Json(new { isValid = false, message = "الكمية المدخلة اكبر من الرصيد المتاح" });
                }

                //الاصناف
                List<QuoteOrderSellDetail> items = new List<QuoteOrderSellDetail>();
                items = vm.OrderSellItems.Select(x => new QuoteOrderSellDetail
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Quantity * x.Price,
                    StoreId=x.StoreId
                }).ToList();
                if (vm.OrderSellItems.Count() == 0)
                    return Json(new { isValid = false, message = "تأكد من وجود صنف واحد على الاقل" });

                if (vm.QuoteOrderSellId != Guid.Empty&&vm.QuoteOrderSellId!=null)
                {
                    //تحديث اى اصناف تم تسجيلها مسباق 

                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == vm.QuoteOrderSellId);
                    var currentItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in currentItems)
                    {
                        var t = items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (t != null)
                        {
                            item.Quantity = t.Quantity;
                            item.Price = t.Price;
                            item.Amount = t.Quantity * t.Price;
                            item.StoreId=t.StoreId;
                        }

                    }

                    model.TotalQuantity = currentItems.Sum(x => (double?)x.Quantity ?? 0);
                    model.TotalValue = currentItems.Sum(x => (double?)x.Amount ?? 0);
                }else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم التجهيز للبيع بنجاح والانتقال لفاتورة البيع" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        #endregion

        #region تجهيز امر انتاج مجمع 
        [HttpGet]
        public ActionResult OrderForProduction(Guid? orderId,int? fi)
        {
            //DS = null;
            OrderSellVM vm = new OrderSellVM();
            Guid? customerId = null;
            Guid? branchId = null;
                 // add
                if (orderId != null&&orderId!=Guid.Empty)
                {
                    var orderSell = db.QuoteOrderSells.Where(x => x.Id == orderId).FirstOrDefault();
                    if (orderSell != null)
                    {
                    vm = GetData(vm, orderSell.BranchId);
                    vm.InvoiceDate = orderSell.InvoiceDate;
                    vm.QuoteOrderSellId = orderSell.Id;
                    vm.OrderSellItems = orderSell.QuoteOrderSellDetails.Where(x => !x.IsDeleted && x.OrderSellItemType == (int)OrderSellItemTypeCl.ProductionOrder)
                         .Select(x => new OrderSellItemsDto
                         {
                             Id = x.Id,
                             ItemId = x.ItemId,
                             ItemName = x.Item.Name,
                             Quantity = x.Quantity,
                             Price = x.Price,
                             Amount = x.Quantity * x.Price,
                             CurrentBalance = BalanceService.GetBalance(x.ItemId, null, null),
                             ItemProductionId = x.ItemProductionId,
                             ItemProductionList = ItemService.GetItemProduction(x.ItemId)
                         }).ToList();
                    branchId = orderSell.BranchId;
                    customerId = orderSell.CustomerId;
                }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.ProductionLineId = new SelectList(db.ProductionLines.Where(x => !x.IsDeleted), "Id", "Name");
            vm.BranchId = branchId;
            vm.CustomerId = customerId;
            vm.ProductionOrderDate = Utility.GetDateTime();

            return View(vm);
        }
        [HttpPost]
        public ActionResult OrderForProduction(OrderSellVM vm,string actionType)
        {
            Guid? customerId = null;
            Guid? branchId = null;
            // add
            if (vm.QuoteOrderSellId != null && vm.QuoteOrderSellId != Guid.Empty)
            {
                var orderSell = db.QuoteOrderSells.Where(x => x.Id == vm.QuoteOrderSellId).FirstOrDefault();
                if (orderSell != null)
                {
                  vm=  GetData(vm, orderSell.BranchId);
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
                    branchId = orderSell.BranchId;
                    customerId = orderSell.CustomerId;
                    vm.InvoiceDate = orderSell.InvoiceDate;
                    vm.QuoteOrderSellId = orderSell.Id;
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.ProductionLineId = new SelectList(db.ProductionLines.Where(x => !x.IsDeleted), "Id", "Name");
            vm.BranchId = branchId;
            vm.CustomerId = customerId;
            vm.OrderSellItems.ForEach(x => x.ItemProductionList = ItemService.GetItemProduction(x.ItemId));

            //فى حالى الضغط على اضافة تكلفة انتاج 
            if (actionType== "addExpense")
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
            if (actionType== "deletExpense")
            {
                if(vm.ExpenseTypeIdDeleted != null)
                {
                    var item = vm.ProductionOrderExpens.Where(x => x.ExpenseTypeId == vm.ExpenseTypeIdDeleted).FirstOrDefault();
                    vm.ProductionOrderExpens.Remove(item);
                }
            }
            //فى حالة الضغط على تسجيل امر انتاج مجمع
            if (actionType=="save")
            {
                if (vm.BranchId == null || vm.ProductionStoreId == null || vm.ProductionUnderStoreId == null /*|| vm.FinalItemId == null|| vm.OrderQuantity == 0 */ || vm.ProductionOrderDate == null || vm.ItemCostCalculateId == null)
                {
                    ViewBag.Msg= "تأكد من تحديد البيانات بشكل صحيح";
                    return View(vm);
                }
                    

                //المواد الداخلة
                List<ProductionOrderDetail> productionOrderDetail = null;
                //قبول اضافة صنف بدون رصيد
                int itemAcceptNoBalance = 0;
                var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))

                    if (vm.OrderSellItems.Any(x=>x.ItemProductionOrderDetailsIn.Any()) || vm.OrderSellItems.Any(x => x.ItemProductionOrderDetailsOut.Any()))
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
                           ComplexProductionIndex=i

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
                if (vm.ProductionOrderExpens.Count()>0)
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
                            productionOrder.BranchId=vm.BranchId;
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
                                ViewBag.Msg= "حدث خطأ اثناء تنفيذ العملية";
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
                                    Notes = $"تكاليف أمر إنتاج رقم : {productionOrder.OrderNumber}",
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


        private OrderSellVM GetData(OrderSellVM vm,Guid? branchId)
        {
            Guid productionUnderStoreId = Guid.NewGuid();
            Guid productionStoreId = Guid.NewGuid();
            if (vm.ProductionUnderStoreId == null)
            {
                //التاكد من تحديد مخزن تحت التصنيع من الاعدادات اولا 
                var storeProductionUnderSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreUnderProductionId).FirstOrDefault();
                //Guid? productionUnderStoreId = null;
                if (!Guid.TryParse(storeProductionUnderSetting.SValue, out productionUnderStoreId))
                    ViewBag.Msg = "تأكد من اختيار مخزن تحت التصنيع أولا من شاشة الاعدادات العامة";
            }
            //مخازن المستخدم
            var stores = EmployeeService.GetStoresByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());

            ViewBag.ProductionUnderStoreId = new SelectList(stores, "Id", "Name", productionUnderStoreId);

            if (vm.ProductionStoreId == null)
            {
                //التاكد من تحديد مخزن التصنيع الداخلى من الاعدادات اولا 
                var storeProductionSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault();
                if (!Guid.TryParse(storeProductionSetting.SValue, out productionStoreId))
                    ViewBag.Msg = "تأكد من اختيار مخزن التصنيع الداخلى أولا من شاشة الاعدادات العامة";
            }
            ViewBag.ProductionStoreId = new SelectList(stores, "Id", "Name", productionStoreId);

            //التاكد من تحديد احتساب تكلفة المنتج من الاعدادات اولا 
            if (vm.ItemCostCalculateId == null)
            {
                var itemCostSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault();
                if (!int.TryParse(itemCostSetting.SValue, out int itemCostId))
                    ViewBag.Msg = "تأكد من اختيار طريقة احتساب تكلفة المنتج أولا من شاشة الاعدادات العامة";
                vm.ItemCostCalculateId = itemCostId;
            }
            return vm;
        }

        public JsonResult OrderForProduction2(OrderSellVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.InvoiceDate == null || vm.CustomerId == null || vm.BranchId == null)
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (vm.OrderSellItems.Where(x=>x.Quantity>x.CurrentBalance).Any())
                {
                    //قبول اضافة صنف بدون رصيد
                    int itemAcceptNoBalance = 0;
                    var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                    if (!int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                        return Json(new { isValid = false, message = "تأكد من تحديد قبول اصناف بدون رصيد من الاعدادات" });
                    if (itemAcceptNoBalance == 0)
                        return Json(new { isValid = false, message = "الكمية المدخلة اكبر من الرصيد المتاح" });
                }

                //الاصناف
                List<QuoteOrderSellDetail> items = new List<QuoteOrderSellDetail>();
                items = vm.OrderSellItems.Select(x => new QuoteOrderSellDetail
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Quantity * x.Price,
                }).ToList();
                if (vm.OrderSellItems.Count() == 0)
                    return Json(new { isValid = false, message = "تأكد من وجود صنف واحد على الاقل" });

                if (vm.QuoteOrderSellId != Guid.Empty&&vm.QuoteOrderSellId!=null)
                {
                    //تحديث اى اصناف تم تسجيلها مسباق 

                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == vm.QuoteOrderSellId);
                    var currentItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in currentItems)
                    {
                        var t = items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (t != null)
                        {
                            item.Quantity = t.Quantity;
                            item.Price = t.Price;
                            item.Amount = t.Quantity * t.Price;

                        }

                    }

                    model.TotalQuantity = currentItems.Sum(x => (double?)x.Quantity ?? 0);
                    model.TotalValue = currentItems.Sum(x => (double?)x.Amount ?? 0);
                }else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم التجهيز للبيع بنجاح والانتقال لفاتورة البيع" });
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