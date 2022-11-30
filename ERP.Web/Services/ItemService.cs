using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Services.AccountTreeService;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public class ItemService
    {
        #region تكلفة الصنف (شراء - بيع)
        // تكلفة المنتج بدلالة (متوسط سعر الشراء - اخر سعر شراء - اعلى او اقل سعر شراء )
        public double GetItemCostCalculation(int costCalculation, Guid? itemId)
        {
            // اذا كان الصنف تم انتاجه من امر انتاج يتم اختساب متوسط تكلفته فى جميع اوامر الانتاج
            using (var db = new VTSaleEntities())
            {
                var productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.ProductionTypeId == (int)ProductionTypeCl.Out);
                if (productionOrderDetails.Any())
                {
                    return Math.Round(productionOrderDetails.Sum(x => (double?)x.ItemCost ?? 0) / productionOrderDetails.Count());
                }
            }
            switch (costCalculation)
            {
                case (int)ItemCostCalculationCl.AveragePurchasePrice: // متوسط سعر الشراء 
                    using (var db = new VTSaleEntities())
                    {
                        //متوسط سعر الشراء =(قيمة الرصيد اول المدة*قيمة المشتريات)/(كمية الرصيد اول المدة+كمية المشتريات+
                        var totalSumOfQuantity = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price * x.Quantity).DefaultIfEmpty(0).Sum();
                        var totalSumOfQuantityFirst = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price * x.Quantity).DefaultIfEmpty(0).Sum();

                        var totlaQuantity = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                        var totlaQuantityFirst = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

                        var totalSum = totalSumOfQuantity + totalSumOfQuantityFirst;
                        var totalQuantity = totlaQuantity + totlaQuantityFirst;

                        return totalQuantity > 0 ? Math.Round(totalSum / totalQuantity, 2) : 0;
                    }
                case (int)ItemCostCalculationCl.LastPurchasePrice: //اخر سعر شراء
                    using (var db = new VTSaleEntities())
                    {
                        return db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).OrderByDescending(x => x.Id).Select(x => x.Price).FirstOrDefault();
                    }
                case (int)ItemCostCalculationCl.HighestPurchasePrice: //اعلى سعر شراء
                    using (var db = new VTSaleEntities())
                    {
                        return db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price).DefaultIfEmpty(0).Max();
                    }
                case (int)ItemCostCalculationCl.LowestPurchasePrice: //اقل سعر شراء 
                    using (var db = new VTSaleEntities())
                    {
                        return db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price).DefaultIfEmpty(0).Min();
                    }
                default:
                    return 0;
            }

        }

        // تكلفة المنتج فى البيع (متوسط سعر البيع - اخر سعر بيع - اعلى او اقل سعر بيع )
        public double GetItemCostCalculationSell(int costCalculation, Guid itemId)
        {
            switch (costCalculation)
            {
                case (int)ItemCostCalculationSellCl.AverageSellPrice: // متوسط سعر البيع 
                    using (var db = new VTSaleEntities())
                    {
                        //متوسط سعر البيع =(قيمة المبيعات)/(كمية المبيعات+
                        double totalSumOfQuantity = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price * x.Quantity).DefaultIfEmpty(0).Sum();
                        double totlaQuantity = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

                        return totlaQuantity > 0 ? Math.Round(totalSumOfQuantity / totlaQuantity, 2) : 0;
                    }
                case (int)ItemCostCalculationSellCl.LastSellPrice: //اخر سعر بيع
                    using (var db = new VTSaleEntities())
                    {
                        return db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).OrderByDescending(x => x.Id).Select(x => x.Price).FirstOrDefault();
                    }
                case (int)ItemCostCalculationSellCl.HighestSellPrice: //اعلى سعر بيع
                    using (var db = new VTSaleEntities())
                    {
                        return db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price).DefaultIfEmpty(0).Max();
                    }
                case (int)ItemCostCalculationSellCl.LowestSellPrice: //اقل سعر بيع 
                    using (var db = new VTSaleEntities())
                    {
                        return db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price).DefaultIfEmpty(0).Min();
                    }
                default:
                    return 0;
            }

        }

        #endregion

        #region تقرير المخزون

        public List<ItemStock> ItemStocks(Guid? itemId, Guid? itemGroup, Guid? branchId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo, int? costCalculationPurchase, int? costCalculationSell, List<int> ParemeterReportList)
        {
            using (var db = new VTSaleEntities())
            {
                IQueryable<Item> items = null;
                List<ItemStock> list = new List<ItemStock>();
                Store store = null;
                items = db.Items.Where(x => !x.IsDeleted);
                if (itemId != null)
                    items = items.Where(x => x.Id == itemId);
                if (itemGroup != null)
                    items.Where(x => x.GroupBasicId == itemGroup);
                if (storeId != null)
                    store = db.Stores.FirstOrDefault(x => x.Id == storeId);
                if (items.Count() > 0)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    list = items.ToList().Select(x => new ItemStock
                    {
                        ItemId = x.Id,
                        ItemName = x.Name,
                        ItemBalance = Math.Round(BalanceService.GetBalance(x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven),
                        ItemUnit = x.Unit != null ? x.Unit.Name : null,
                        StoreName = store != null ? store.Name : null,
                        ItemCostPurchase = Math.Round(GetItemCostCalculation(costCalculationPurchase ?? 1, x.Id), 2, MidpointRounding.ToEven),
                        ItemCostSell = Math.Round(GetItemCostCalculationSell(costCalculationSell ?? 1, x.Id), 2, MidpointRounding.ToEven),
                        TotalItemCostPurchase = Math.Round(GetItemCostCalculation(costCalculationPurchase ?? 1, x.Id), 2, MidpointRounding.ToEven) * Math.Round(BalanceService.GetBalance(x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven),
                        TotalItemCostSell = Math.Round(GetItemCostCalculationSell(costCalculationSell ?? 1, x.Id) * BalanceService.GetBalance(x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven),
                        Purchases = ParemeterReportList.Contains((int)ParemeterReport.Purchase) ? Math.Round(GetBalanceBy(ParemeterReport.Purchase, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        BackPurchases = ParemeterReportList.Contains((int)ParemeterReport.PurchaseBack) ? Math.Round(GetBalanceBy(ParemeterReport.PurchaseBack, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        InitialBalances = ParemeterReportList.Contains((int)ParemeterReport.Intial) ? Math.Round(GetBalanceBy(ParemeterReport.Intial, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        Sell = ParemeterReportList.Contains((int)ParemeterReport.Sell) ? Math.Round(GetBalanceBy(ParemeterReport.Sell, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        SellBack = ParemeterReportList.Contains((int)ParemeterReport.SellBack) ? Math.Round(GetBalanceBy(ParemeterReport.SellBack, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        StoreTranFrom = ParemeterReportList.Contains((int)ParemeterReport.StoreTranTo) ? Math.Round(GetBalanceBy(ParemeterReport.StoreTranFrom, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        StoreTranTo = ParemeterReportList.Contains((int)ParemeterReport.StoreTranFrom) ? Math.Round(GetBalanceBy(ParemeterReport.StoreTranTo, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        Inventories = ParemeterReportList.Contains((int)ParemeterReport.Inventory) ? Math.Round(GetBalanceBy(ParemeterReport.Inventory, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        ProductionOrders = ParemeterReportList.Contains((int)ParemeterReport.ProductionOrder) ? Math.Round(GetBalanceBy(ParemeterReport.ProductionOrder, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        ProductionOrderDetails = ParemeterReportList.Contains((int)ParemeterReport.ProductionOrderDetails) ? Math.Round(GetBalanceBy(ParemeterReport.ProductionOrderDetails, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        Maintenances = ParemeterReportList.Contains((int)ParemeterReport.Maintenance) ? Math.Round(GetBalanceBy(ParemeterReport.Maintenance, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        MaintenanceSpareParts = ParemeterReportList.Contains((int)ParemeterReport.MaintenanceSpareParts) ? Math.Round(GetBalanceBy(ParemeterReport.MaintenanceSpareParts, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        TotalIn = ParemeterReportList.Contains((int)ParemeterReport.TotalIn) ? Math.Round(BalanceService.GetBalanceIn(x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        TotalOut = ParemeterReportList.Contains((int)ParemeterReport.TotalOut) ? Math.Round(BalanceService.GetBalanceOut(x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        Damage = ParemeterReportList.Contains((int)ParemeterReport.Damages) ? Math.Round(GetBalanceBy(ParemeterReport.Damages, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        StorePermissionReceive = ParemeterReportList.Contains((int)ParemeterReport.StorePermissionReceive) ? Math.Round(GetBalanceBy(ParemeterReport.StorePermissionReceive, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                        StorePermissionLeave = ParemeterReportList.Contains((int)ParemeterReport.StorePermissionLeave) ? Math.Round(GetBalanceBy(ParemeterReport.StorePermissionLeave, x.Id, storeId, branchId, dtFrom, dtTo), 2, MidpointRounding.ToEven) : 0,
                    }).ToList();
                    stopwatch.Stop();
                    return list;
                }
                else
                    return new List<ItemStock>();
            }
        }

        //
        double GetBalanceBy(ParemeterReport paremeterReport, Guid itemId, Guid? storeId, Guid? branchId, DateTime? dtFrom, DateTime? dtTo)
        {
            using (var db = new VTSaleEntities())
            {
                switch (paremeterReport)
                {
                    case ParemeterReport.Intial:
                        var itemIntialBalance = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId && x.IsApproval);
                        if (storeId != null)
                            itemIntialBalance = itemIntialBalance.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            itemIntialBalance = itemIntialBalance.Where(x => x.Store.BranchId == branchId);
                        if (dtFrom != null)
                            itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) >= dtFrom);
                        if (dtTo != null)
                            itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) <= dtTo);
                        return itemIntialBalance.Count() > 0 ? itemIntialBalance.Sum(x => x.Quantity) : 0;

                    case ParemeterReport.Purchase: //رصيد الشراء
                        var supplyBalance = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoice.IsFinalApproval && x.ItemId == itemId);
                        if (storeId != null)
                            supplyBalance = supplyBalance.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            supplyBalance = supplyBalance.Where(x => x.PurchaseInvoice.BranchId == branchId);
                        if (dtFrom != null)
                            supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);
                        return supplyBalance.Count() > 0 ? supplyBalance.Sum(x => x.Quantity) : 0;

                    case ParemeterReport.PurchaseBack://رصيد مرتجع الشراء
                        var supplyBackBalance = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoice.IsFinalApproval && x.ItemId == itemId);
                        if (storeId != null)
                            supplyBackBalance = supplyBackBalance.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            supplyBackBalance = supplyBackBalance.Where(x => x.PurchaseBackInvoice.BranchId == branchId);
                        if (dtFrom != null)
                            supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) <= dtTo);
                        return supplyBackBalance.Count() > 0 ? supplyBackBalance.Sum(x => x.Quantity) : 0;

                    case ParemeterReport.Sell:
                        var sellBalance = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.IsFinalApproval && x.ItemId == itemId);
                        if (storeId != null)
                            sellBalance = sellBalance.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            sellBalance = sellBalance.Where(x => x.SellInvoice.BranchId == branchId);
                        if (dtFrom != null)
                            sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
                        return sellBalance.Count() > 0 ? sellBalance.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.SellBack:
                        var sellBackBalance = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoice.IsFinalApproval && x.ItemId == itemId);
                        if (storeId != null)
                            sellBackBalance = sellBackBalance.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            sellBackBalance = sellBackBalance.Where(x => x.SellBackInvoice.BranchId == branchId);
                        if (dtFrom != null)
                            sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) <= dtTo);
                        return sellBackBalance.Count() > 0 ? sellBackBalance.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.StoreTranFrom: //تحويل مخزنى(من)
                        var storeTranFromBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval);
                        if (storeId != null)
                            storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFromId == storeId);
                        if (branchId != null)
                            storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFrom.BranchId == branchId);
                        if (dtFrom != null)
                            storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                        if (dtTo != null)
                            storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);
                        return storeTranFromBalance.Count() > 0 ? storeTranFromBalance.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.StoreTranTo: //تحويل مخزنى الى
                        var storeTranToBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval);
                        if (storeId != null)
                            storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreToId == storeId);
                        if (branchId != null)
                            storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreTo.BranchId == branchId);
                        if (dtFrom != null)
                            storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                        if (dtTo != null)
                            storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);
                        return storeTranToBalance.Count() > 0 ? storeTranToBalance.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.Inventory:
                        var inventoriesBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                        if (storeId != null)
                            inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.StoreId == storeId);
                        if (branchId != null)
                            inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.BranchId == branchId);
                        if (dtFrom != null)
                            inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) <= dtTo);
                        return inventoriesBalance.Count() > 0 ? inventoriesBalance.Sum(x => x.DifferenceCount) : 0;

                    case ParemeterReport.ProductionOrder:
                        //اوامر الانتاج الخارجة 
                        var productionOrders = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.Out && x.ItemId == itemId);
                        if (storeId != null)
                            productionOrders = productionOrders.Where(x => x.ProductionOrder.ProductionStoreId == storeId);
                        if (branchId != null)
                            productionOrders = productionOrders.Where(x => x.ProductionOrder.BranchId == branchId);
                        if (dtFrom != null)
                            productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom);
                        if (dtTo != null)
                            productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);
                        return productionOrders.Count() > 0 ? productionOrders.Sum(p => (double?)p.Quantity ?? 0) : 0;
                    //var productionReceiptOrders = db.ProductionOrderReceipts.Where(x => !x.IsDeleted /*&& x.ProductionOrder.FinalItemId == itemId*/);
                    //if (storeId != null)
                    //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStoreId == storeId);
                    //if (branchId != null)
                    //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStore.BranchId == branchId);
                    //if (dtFrom != null)
                    //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) >= dtFrom);
                    //if (dtTo != null)
                    //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) <= dtTo);
                    //    return productionOrders.Count() > 0 ? productionOrders.Sum(x => /*x.OrderQuantity*/0 - x.ProductionOrderReceipts.Where(p => !p.IsDeleted).Sum(p => p.ReceiptQuantity)) : 0 +
                    //productionReceiptOrders.Count() > 0 ? productionReceiptOrders.Sum(x => x.ReceiptQuantity) : 0;
                    case ParemeterReport.ProductionOrderDetails:
                        var productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.In && x.ItemId == itemId);
                        if (storeId != null)
                            productionOrderDetails = productionOrderDetails.Where(x => x.ProductionOrder.ProductionUnderStoreId == storeId);
                        if (branchId != null)
                            productionOrderDetails = productionOrderDetails.Where(x => x.ProductionOrder.BranchId == branchId);
                        if (dtFrom != null)
                            productionOrderDetails = productionOrderDetails.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom);
                        if (dtTo != null)
                            productionOrderDetails = productionOrderDetails.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);
                        return productionOrderDetails.Count() > 0 ? productionOrderDetails.Sum(x => (double?)(x.Quantity + x.Quantitydamage)) ?? 0 : 0;

                    case ParemeterReport.Maintenance:
                        var maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval && x.Maintenance.StoreReceiptId != null);
                        if (storeId != null)
                            maintenances = maintenances.Where(x => x.Maintenance.StoreId == storeId);
                        if (branchId != null)
                            maintenances = maintenances.Where(x => x.Maintenance.BranchId == branchId);
                        if (dtFrom != null)
                            maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);

                        var maintenanceReceipts = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval);
                        if (storeId != null)
                            maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.StoreReceiptId == storeId);
                        if (branchId != null)
                            maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.BranchId == branchId);
                        if (dtFrom != null)
                            maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);
                        return maintenances.Count() - maintenanceReceipts.Count();

                    case ParemeterReport.MaintenanceSpareParts:
                        var maintenanceSpareParts = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId && x.MaintenanceDetail.Maintenance.IsFinalApproval);
                        if (storeId != null)
                            maintenanceSpareParts = maintenanceSpareParts.Where(x => x.StoreId == storeId);
                        if (branchId != null)
                            maintenanceSpareParts = maintenanceSpareParts.Where(x => x.Store.BranchId == branchId);
                        if (dtFrom != null)
                            maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) <= dtTo);
                        return maintenanceSpareParts.Count() > 0 ? maintenanceSpareParts.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.Damages:
                        var damagesBalance = db.DamageInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                        if (storeId != null)
                            damagesBalance = damagesBalance.Where(x => x.DamageInvoice.StoreId == storeId);
                        if (branchId != null)
                            damagesBalance = damagesBalance.Where(x => x.DamageInvoice.Store.BranchId == branchId);
                        if (dtFrom != null)
                            damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) >= dtFrom);
                        if (dtTo != null)
                            damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) <= dtTo);
                        return damagesBalance.Count() > 0 ? damagesBalance.Sum(x => x.Quantity) : 0;
                    case ParemeterReport.StorePermissionReceive:
                        var StorePermissionReceive = db.StorePermissionItems.Where(x => !x.IsDeleted && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.ItemId == itemId && x.StorePermission.IsReceive);
                        if (storeId != null)
                            StorePermissionReceive = StorePermissionReceive.Where(x => x.StorePermission.StoreId == storeId);
                        if (branchId != null)
                            StorePermissionReceive = StorePermissionReceive.Where(x => x.StorePermission.Store.BranchId == branchId);
                        if (dtFrom != null)
                            StorePermissionReceive = StorePermissionReceive.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom);
                        if (dtTo != null)
                            StorePermissionReceive = StorePermissionReceive.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);
                        return StorePermissionReceive.Count() > 0 ? StorePermissionReceive.Sum(x => x.Quantity) : 0;

                    case ParemeterReport.StorePermissionLeave:
                        var StorePermissionLeave = db.StorePermissionItems.Where(x => !x.IsDeleted && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.ItemId == itemId && !x.StorePermission.IsReceive);
                        if (storeId != null)
                            StorePermissionLeave = StorePermissionLeave.Where(x => x.StorePermission.StoreId == storeId);
                        if (branchId != null)
                            StorePermissionLeave = StorePermissionLeave.Where(x => x.StorePermission.Store.BranchId == branchId);
                        if (dtFrom != null)
                            StorePermissionLeave = StorePermissionLeave.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom);
                        if (dtTo != null)
                            StorePermissionLeave = StorePermissionLeave.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);
                        return StorePermissionLeave.Count() > 0 ? StorePermissionLeave.Sum(x => x.Quantity) : 0;
                    default:
                        return 0;
                }

            }

        }


        #endregion

        #region تقرير اصناف عينات وبونص من مورد ولعميل
        public List<ItemGift> GetItemPurchaseGifts(VTSaleEntities db, Guid? BranchId, Guid? SupplierId, Guid? ItemTypeId, Guid? ItemId, string dFrom, string dTo, bool? IsFinalApproval)
        {
            if (SupplierId == null)
                return new List<ItemGift>();
            IQueryable<PurchaseInvoicesDetail> purchases = null;
            purchases = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.Price == 0);
            if (BranchId != null)
                purchases = purchases.Where(x => x.PurchaseInvoice.BranchId == BranchId);
            if (SupplierId != null)
                purchases = purchases.Where(x => x.PurchaseInvoice.SupplierId == SupplierId);
            if (IsFinalApproval == true)
                purchases = purchases.Where(x => x.PurchaseInvoice.IsFinalApproval == IsFinalApproval);
            if (ItemTypeId != null)
                purchases = purchases.Where(x => x.Item.ItemTypeId == ItemTypeId);
            if (ItemId != null)
                purchases = purchases.Where(x => x.Item.Id == ItemId);
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                purchases = purchases.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);
            var list = purchases.ToList().GroupBy(x => x.ItemId).Select(x => new ItemGift
            {
                Id = x.Key.Value,
                ItemCode = x.FirstOrDefault().Item.ItemCode,
                ItemName = x.FirstOrDefault().Item.Name,
                UnitName = x.FirstOrDefault().Item.Unit.Name,
                Quantity = x.Sum(y => y.Quantity),
                CostItem = GetItemCostCalculation(1, x.Key.Value),
                Amount = x.Sum(y => y.Quantity) * GetItemCostCalculation(1, x.Key.Value)
            }).ToList();

            return list;
        }
        public List<ItemGift> GetItemSellGifts(VTSaleEntities db, Guid? BranchId, Guid? CustomerId, Guid? ItemTypeId, Guid? ItemId, string dFrom, string dTo, bool? IsFinalApproval)
        {
            if (CustomerId == null)
                return new List<ItemGift>();
            IQueryable<SellInvoicesDetail> sells = null;
            sells = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.Price == 0);
            if (BranchId != null)
                sells = sells.Where(x => x.SellInvoice.BranchId == BranchId);
            if (CustomerId != null)
                sells = sells.Where(x => x.SellInvoice.CustomerId == CustomerId);
            if (IsFinalApproval == true)
                sells = sells.Where(x => x.SellInvoice.IsFinalApproval == IsFinalApproval);
            if (ItemTypeId != null)
                sells = sells.Where(x => x.Item.ItemTypeId == ItemTypeId);
            if (ItemId != null)
                sells = sells.Where(x => x.Item.Id == ItemId);
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sells = sells.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
            var list = sells.ToList().GroupBy(x => x.ItemId).Select(x => new ItemGift
            {
                Id = x.Key.Value,
                ItemCode = x.FirstOrDefault().Item.ItemCode,
                ItemName = x.FirstOrDefault().Item.Name,
                UnitName = x.FirstOrDefault().Item.Unit.Name,
                Quantity = x.Sum(y => y.Quantity),
                CostItem = GetItemCostCalculation(1, x.Key.Value),
                Amount = x.Sum(y => y.Quantity) * GetItemCostCalculation(1, x.Key.Value)
            }).ToList();

            return list;
        }
        #endregion

        #region اخر سعر شراء للصنف
        public double GetLastPurchasePrice(Guid? itemId, VTSaleEntities db)
        {

            var item = db.PurchaseInvoicesDetails.Where(i => !i.IsDeleted && i.ItemId == itemId).OrderByDescending(i => i.Id).FirstOrDefault();
            if (item != null)
                return item.Price;
            else
                return 0;


        }
        #endregion

        #region هل الصنف يسمح بالسحب منه بالسالب
        public Result IsAllowNoBalance(Guid? itemId, Guid? storeId, double quantity)
        {
            //quantity الكمية المراد بيعها او مرتجع توريد او رصيد اول او تحويل مخزنى تكون اكبر من او تساوى رصيد الصنف الحالى 
            using (var db = new VTSaleEntities())
            {
                if (itemId != null && itemId != Guid.Empty && storeId != null && storeId != Guid.Empty)
                {
                    int itemAcceptNoBalance = 0;
                    var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
                    if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                    {
                        if (itemAcceptNoBalance == 0)// رفض السحب بالسالب 
                        {
                            var balance = BalanceService.GetBalance(itemId, storeId);
                            if (balance <= 0 || quantity > balance)
                            {
                                var itemName = db.Items.Where(x => x.Id == itemId).FirstOrDefault().Name;
                                return new Result
                                {
                                    IsValid = false,
                                    ItemNotAllowed = itemName
                                };
                            }

                        }
                        return new Result
                        {
                            IsValid = true,
                        };
                    }
                    else
                        return new Result
                        {
                            IsValid = false
                        };

                }
                else
                    return new Result
                    {
                        IsValid = false
                    };

            }
        }

        public class Result
        {
            public bool IsValid { get; set; }
            public string ItemNotAllowed { get; set; }
        }
        #endregion

        #region استعراض المجموعات فى شكل شجرى 
        public List<TreeViewDraw> GetGroups()
        {
            List<TreeViewDraw> accountTrees = new List<TreeViewDraw>();
            using (var db = new VTSaleEntities())
            {
                //8 milliseconds after maping AccountTree to DTO class
                var parentss = db.Groups.Where(x => !x.IsDeleted).ToList();

                return GetAll(parentss, null);
            }
        }
        public List<TreeViewDraw> GetAll(List<Group> groups, Guid? parent)
        {
            var parents = groups.Where(x => !x.IsDeleted && x.ParentId == parent);
            return parents.Select(x => new TreeViewDraw
            {
                text = x.Name,
                children = GetAll(groups, x.Id)
            }).ToList();
        }
        #endregion
        #region تقرير بكميات الاصناف المباعه خلال فترة 
        public List<RptItemSellDto> GetItemSell(Guid? branchId, DateTime? dtFrom, DateTime? dtTo)
        {
            using (var db=new VTSaleEntities())
            {
                IQueryable<SellInvoicesDetail> sellInvoicesDetails = null;
                sellInvoicesDetails = db.SellInvoicesDetails.Where(x => !x.IsDeleted&&!x.SellInvoice.IsDeleted&&x.SellInvoice.IsFinalApproval);
                if (branchId != null)
                    sellInvoicesDetails = sellInvoicesDetails.Where(x => x.SellInvoice.BranchId == branchId);
                else
                    return new List<RptItemSellDto>();
                //if (IsFinalApproval)
                //    sellInvoicesDetails = sellInvoicesDetails.Where(x => x.SellInvoice.IsFinalApproval);
                //if (!AllTimes)
                    if (dtFrom != null && dtTo != null)
                        sellInvoicesDetails = sellInvoicesDetails.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
                var itemsGroup = sellInvoicesDetails.GroupBy(x => x.ItemId).Select(x => new RptItemSellDto
                {
                    ItemId = x.Key,
                    ItemName = x.FirstOrDefault().Item.Name,
                    ItemCode=x.FirstOrDefault().Item.ItemCode,
                    Quantity = x.Sum(i => (double?)i.Quantity ?? 0)
                }).ToList();
                return itemsGroup;
            }

        }


        #endregion

        #region توليفة بدلالة اسم الصنف
        public static List<DropDownList> GetItemProduction(Guid? itemId)
        {
            using (var db=new VTSaleEntities())
            {
                var itemGroups = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                var list = itemGroups.Select(x => new DropDownList
                {
                    Id = x.ItemProductionId,
                    Name = x.ItemProduction.Name
                }).ToList();
                return list;
            }
        }
        #endregion
    }
    public class ItemStock
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }
        public string StoreName { get; set; }
        public double InitialBalances { get; set; }
        public double ItemBalance { get; set; }//رصيد الصنف
        public double Purchases { get; set; }
        public double BackPurchases { get; set; }
        public double Sell { get; set; }
        public double SellBack { get; set; }
        public double StoreTranFrom { get; set; } //تحويل مخزنى من
        public double StoreTranTo { get; set; } //تحويل مخزنى الى
        public double Inventories { get; set; } //جرد
        public double ProductionOrders { get; set; }//اوامر الانتاج
        public double ProductionOrderDetails { get; set; }//خامات اوامر الانتاج
        public double Maintenances { get; set; }//الصيانة
        public double MaintenanceSpareParts { get; set; }//قطع غيار الصيانة
        public double ItemCostPurchase { get; set; }//سعر تكلفة الشراء
        public double ItemCostSell { get; set; }//سعر تكلفة البيع
        public double TotalItemCostPurchase { get; set; }//اجمالى رصيد الشراء
        public double TotalItemCostSell { get; set; }//سعر رصيد البيع
        public double TotalIn { get; set; }//اجمالى وارد 
        public double TotalOut { get; set; }//اجمالى صادر 
        public double Damage { get; set; }//الهالك 
        public double StorePermissionReceive { get; set; }//اذن استلام 
        public double StorePermissionLeave { get; set; }//اذن صرف 
    }

    public enum ParemeterReport
    {
        TotalIn = 1,//اجمالى وارد 
        TotalOut, //اجمالى صادر 
        Intial, //رصيد اول
        Purchase,//شراء
        PurchaseBack,//مرتجع شراء
        Sell, //بيع
        SellBack,//مرتجع بيع
        StoreTranFrom,//تحويل من
        StoreTranTo,//تحويل الى
        Inventory,//جرد
        ProductionOrder,//اوامر الانتاج
        ProductionOrderDetails,//خامات اوامر انتاج
        Maintenance,//صيانة
        MaintenanceSpareParts,//قطع غيار صيانة
        Damages,//هالك
        StorePermissionReceive,//اذن صرف
        StorePermissionLeave,//اذن استلام
    }

    public class ItemGift
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double CostItem { get; set; }
        public double Amount { get; set; }
        public int? Num { get; set; }
    }
}