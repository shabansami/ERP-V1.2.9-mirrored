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

            var items = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, OrderSellInvoiceNumber = x.InvoiceNumber, QuoteInvoiceNumber = x.Quote.InvoiceNumber, CustomerName = x.Customer.Name, InvoiceDate = x.InvoiceDate.ToString(), TotalValue = x.TotalValue, Actions = n, Num = n }).ToList();
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
                            CurrentBalance = BalanceService.GetBalance(x.ItemId, null, null)
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
                                 CurrentBalance = BalanceService.GetBalance(x.ItemId, null, null)
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
            vm.BranchId = branchId;
            vm.CustomerId = customerId;


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
                        else
                        {
                            //الاصناف الخارجة
                            var itemsOut = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.Out && x.ItemProductionId == itemOrder.ItemProductionId);
                            if (itemsOut.Count() > 0)
                            {
                                var itemsMaterials = itemsOut.ToList().Select(x => new ItemProductionOrderDetailsDT
                                {
                                    ItemProductionId = x.ItemProductionId,
                                    ItemId = x.ItemId,
                                    ItemName = x.Item.Name,
                                    QuantityRequired = x.Quantity * itemOrder.Quantity,
                                    QuantityAvailable = BalanceService.GetBalance(x.ItemId, vm.ProductionUnderStoreId, null, null, null),
                                }).ToList();
                            }

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
            vm.BranchId = branchId;
            vm.CustomerId = customerId;
            vm.OrderSellItems.ForEach(x => x.ItemProductionList = ItemService.GetItemProduction(x.ItemId));
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
            ViewBag.ProductionUnderStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && !x.IsDamages && x.BranchId == branchId), "Id", "Name", productionUnderStoreId);

            if (vm.ProductionStoreId == null)
            {
                //التاكد من تحديد مخزن التصنيع الداخلى من الاعدادات اولا 
                var storeProductionSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault();
                if (!Guid.TryParse(storeProductionSetting.SValue, out productionStoreId))
                    ViewBag.Msg = "تأكد من اختيار مخزن التصنيع الداخلى أولا من شاشة الاعدادات العامة";
            }
            ViewBag.ProductionStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && !x.IsDamages && x.BranchId == branchId), "Id", "Name", productionStoreId);

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