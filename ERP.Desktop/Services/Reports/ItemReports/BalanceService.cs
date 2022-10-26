using ERP.Desktop.DTOs;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using System.Windows.Controls;

namespace ERP.Desktop.Services.Reports.ItemReports
{
    public class BalanceService
    {
        VTSaleEntities db;

        public BalanceService(VTSaleEntities db)
        {
            this.db = db;
        }


        #region Balance
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
        // الرصيد بدلالة الصنف والمخزن
        public static double GetBalance(Guid? itemId, Guid? storeId, Guid? branchId = null, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            if (itemId == null) return 0;
            using (var db = new VTSaleEntities())
            {
                //var tt=0;
                //if (itemId == new Guid("3F125BE4-B30F-4439-8B9D-D78C8E92665C"))
                //    tt = 7;
                double balance = 0;
                //التوريد
                var supplyBalance = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    supplyBalance = supplyBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    supplyBalance = supplyBalance.Where(x => x.PurchaseInvoice.BranchId == branchId);
                if (dtFrom != null)
                    supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);
                //مرتجع التوريد
                var supplyBackBalance = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    supplyBackBalance = supplyBackBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    supplyBackBalance = supplyBackBalance.Where(x => x.PurchaseBackInvoice.BranchId == branchId);
                if (dtFrom != null)
                    supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) <= dtTo);
                //البيع
                var sellBalance = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    sellBalance = sellBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    sellBalance = sellBalance.Where(x => x.SellInvoice.BranchId == branchId);
                if (dtFrom != null)
                    sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
                //مرتجع البيع
                var sellBackBalance = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    sellBackBalance = sellBackBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    sellBackBalance = sellBackBalance.Where(x => x.SellBackInvoice.BranchId == branchId);
                if (dtFrom != null)
                    sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) <= dtTo);
                //تحويل مخزنى من
                var storeTranFromBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval && !x.StoresTransfer.IsRefusStore);
                if (storeId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFromId == storeId);
                if (branchId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFrom.BranchId == branchId);
                if (dtFrom != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                if (dtTo != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);
                //تحويل مخزنى الى
                var storeTranToBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval && !x.StoresTransfer.IsRefusStore);
                if (storeId != null)
                    storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreToId == storeId);
                if (branchId != null)
                    storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreTo.BranchId == branchId);
                if (dtFrom != null)
                    storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                if (dtTo != null)
                    storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);
                //ارصدة اول المدة
                var itemIntialBalance = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId && x.IsApproval);
                if (storeId != null)
                    itemIntialBalance = itemIntialBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    itemIntialBalance = itemIntialBalance.Where(x => x.Store.BranchId == branchId);
                if (dtFrom != null)
                    itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) >= dtFrom);
                if (dtTo != null)
                    itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) <= dtTo);
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
                //خامات الانتاج
                var productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ProductionTypeId == (int)ProductionTypeCl.In && x.ItemId == itemId);
                if (storeId != null)
                    productionOrderDetails = productionOrderDetails.Where(x => x.ProductionOrder.ProductionUnderStoreId == storeId);
                if (branchId != null)
                    productionOrderDetails = productionOrderDetails.Where(x => x.ProductionOrder.BranchId == branchId);
                if (dtFrom != null)
                    productionOrderDetails = productionOrderDetails.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom);
                if (dtTo != null)
                    productionOrderDetails = productionOrderDetails.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);
                //تسليم اوامر الانتاج
                //var productionReceiptOrders = db.ProductionOrderReceipts.Where(x => !x.IsDeleted /*&& x.ProductionOrder.FinalItemId == itemId*/);
                //if (storeId != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStoreId == storeId);
                //if (branchId != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStore.BranchId == branchId);
                //if (dtFrom != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) >= dtFrom);
                //if (dtTo != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) <= dtTo);
                //الصيانة
                var maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval && x.Maintenance.StoreReceiptId != null);
                if (storeId != null)
                    maintenances = maintenances.Where(x => x.Maintenance.StoreId == storeId);
                if (branchId != null)
                    maintenances = maintenances.Where(x => x.Maintenance.BranchId == branchId);
                if (dtFrom != null)
                    maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);
                //مخزن التسليم للصيانة
                var maintenanceReceipts = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval);
                if (storeId != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.StoreReceiptId == storeId);
                if (branchId != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.BranchId == branchId);
                if (dtFrom != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);
                //قطع غيار الصيانة
                var maintenanceSpareParts = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId && x.MaintenanceDetail.Maintenance.IsFinalApproval);
                if (storeId != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => x.Store.BranchId == branchId);
                if (dtFrom != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) <= dtTo);
                //الجرد
                var inventoriesBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                if (storeId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.StoreId == storeId);
                if (branchId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.BranchId == branchId);
                if (dtFrom != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) <= dtTo);

                //الهالك
                var damagesBalance = db.DamageInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                if (storeId != null)
                    damagesBalance = damagesBalance.Where(x => x.DamageInvoice.StoreId == storeId);
                if (branchId != null)
                    damagesBalance = damagesBalance.Where(x => x.DamageInvoice.Store.BranchId == branchId);
                if (dtFrom != null)
                    damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) <= dtTo);



                //اذن الاستلام
                var StorePermissionReceive = db.StorePermissionItems.Where(x => !x.IsDeleted && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.ItemId == itemId && x.StorePermission.IsReceive);
                if (storeId != null)
                    StorePermissionReceive = StorePermissionReceive.Where(x => x.StorePermission.StoreId == storeId);
                if (branchId != null)
                    StorePermissionReceive = StorePermissionReceive.Where(x => x.StorePermission.Store.BranchId == branchId);
                if (dtFrom != null)
                    StorePermissionReceive = StorePermissionReceive.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom);
                if (dtTo != null)
                    StorePermissionReceive = StorePermissionReceive.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);


                //اذن الصرف
                var StorePermissionLeave = db.StorePermissionItems.Where(x => !x.IsDeleted && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.ItemId == itemId && !x.StorePermission.IsReceive);
                if (storeId != null)
                    StorePermissionLeave = StorePermissionLeave.Where(x => x.StorePermission.StoreId == storeId);
                if (branchId != null)
                    StorePermissionLeave = StorePermissionLeave.Where(x => x.StorePermission.Store.BranchId == branchId);
                if (dtFrom != null)
                    StorePermissionLeave = StorePermissionLeave.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom);
                if (dtTo != null)
                    StorePermissionLeave = StorePermissionLeave.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);





                var supplyBalanceQuantity = supplyBalance.Count() > 0 ? supplyBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var sellBackBalanceQuantity = sellBackBalance.Count() > 0 ? sellBackBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var storeTranToBalanceQuantity = (storeId == null && branchId == null) ? 0 : storeTranToBalance.Count() > 0 ? storeTranToBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var itemIntialBalanceQuantity = itemIntialBalance.Count() > 0 ? itemIntialBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var productionOrdersQuantity = productionOrders.Count() > 0 ? productionOrders.Sum(p => (double?)p.Quantity ?? 0) : 0;
                //var productionOrdersQuantity = productionOrders.Count() > 0 ? productionOrders.Sum(x => (double?)(x.OrderQuantity - x.ProductionOrderReceipts.Where(p => !p.IsDeleted).Sum(p => p.ReceiptQuantity)) ?? 0) : 0;
                //var productionReceiptOrdersQuantity = productionReceiptOrders.Count() > 0 ? productionReceiptOrders.Sum(x => (double?)x.ReceiptQuantity) ?? 0 : 0;
                var maintenancesQuantity = maintenances.Count();
                var sellBalanceQuantity = sellBalance.Count() > 0 ? sellBalance.Sum(x => (double?)x.Quantity) : 0;
                var supplyBackBalanceQuantity = supplyBackBalance.Count() > 0 ? supplyBackBalance.Sum(x => (double?)x.Quantity) : 0;
                var storeTranFromBalanceQuantity = (storeId == null && branchId == null) ? 0 : storeTranFromBalance.Count() > 0 ? storeTranFromBalance.Sum(x => (double?)x.Quantity) : 0;
                var productionOrderDetailsQuantity = productionOrderDetails.Count() > 0 ? productionOrderDetails.Sum(x => (double?)(x.Quantity + x.Quantitydamage)) ?? 0 : 0;
                var maintenanceReceiptsQuantity = maintenanceReceipts.Count();
                var maintenanceSparePartsQuantity = maintenanceSpareParts.Count() > 0 ? maintenanceSpareParts.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var inventoriesBalanceQuantity = inventoriesBalance.Count() > 0 ? inventoriesBalance.Sum(x => (double?)x.DifferenceCount) ?? 0 : 0;
                var damagesBalanceQuantity = damagesBalance.Count() > 0 ? damagesBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var StorePermissionReceiveQuantity = StorePermissionReceive.Count() > 0 ? StorePermissionReceive.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var StorePermissionLeaveQuantity = StorePermissionLeave.Count() > 0 ? StorePermissionLeave.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                balance = ((supplyBalanceQuantity +
                    sellBackBalanceQuantity +
                    storeTranToBalanceQuantity +
                    itemIntialBalanceQuantity +
                    productionOrdersQuantity +
                    //productionReceiptOrdersQuantity +
                    StorePermissionReceiveQuantity +
                   maintenancesQuantity) - (
                    sellBalanceQuantity +
                    supplyBackBalanceQuantity +
                    storeTranFromBalanceQuantity +
                    productionOrderDetailsQuantity +
                    maintenanceReceiptsQuantity +
                    maintenanceSparePartsQuantity + damagesBalanceQuantity + StorePermissionLeaveQuantity
                     ) + inventoriesBalanceQuantity) ?? 0;
                return Math.Round(balance, 2, MidpointRounding.ToEven);
                // الحصول على بيانات المؤسسة من الاعدادات
                //var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                //var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                //if (!string.IsNullOrEmpty(schema))
                //     return db.Database.SqlQuery<double>($"select [{schema}].[GetBalance](@ItemId,@StoreId)", new SqlParameter("@ItemId", itemId), new SqlParameter("@StoreId", storeId)).FirstOrDefault();
                //else
                //    return 0;

            }
        }

        #endregion

        #region تقارير أرصدة الاصناف 
        //بحث الارصدة
        public  List<ItemBalanceDto> SearchItemBalance(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId, bool isFirstInitPage, string txtSearch = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> items = null;
            items = db.Items.Where(x => !x.IsDeleted);
            if (itemId != null)
                items = items.Where(x => x.Id == itemId);
            else
                items = items.Where(x =>
                x.ItemCode.ToString().Trim().Contains(itemCode) || x.ItemCode.ToString().Trim().Contains(txtSearch)
                || x.BarCode.Trim().Contains(barCode) || x.BarCode.Trim().Contains(txtSearch)
                || x.GroupBasicId == groupId || x.Name.Contains(txtSearch)
                || x.ItemTypeId == itemtypeId);

            string storeName = string.Empty;
            if (storeId != null)
                storeName = db.Stores.Where(x => x.Id == storeId).FirstOrDefault().Name;

            List<ItemBalanceDto> itemsList = new List<ItemBalanceDto>();
            if (items.Count() == 0)
            {
                itemsList = db.Items.Where(x => !x.IsDeleted).Select(x => new ItemBalanceDto
                {
                    Id = x.Id,
                    ItemName = x.Name,
                    ItemCode = x.ItemCode.ToString(),
                    BarCode = x.BarCode,
                    GroupName = x.GroupBasic.Name,
                    ItemTypeName = x.ItemType.Name,
                    StoreName = storeName
                }).ToList();
                if (storeId != null)
                    itemsList.ForEach(y => y.Balance = GetBalance(y.Id, storeId));
                else if (branchId != null)
                    itemsList.ForEach(y => y.Balance = GetBalance(y.Id, null, branchId));
                else
                    itemsList.ForEach(y => y.Balance = GetBalance(y.Id, null, null));



            }
            else
            {
                itemsList = items.ToList().Select(x => new ItemBalanceDto
                {
                    Id = x.Id,
                    ItemName = x.Name,
                    ItemCode = x.ItemCode.ToString(),
                    BarCode = x.BarCode,
                    GroupName = x.GroupBasic.Name,
                    ItemTypeName = x.ItemType.Name,
                    StoreName = storeName,
                    Balance = storeId != null ? GetBalance(x.Id, storeId) : branchId != null ? GetBalance(x.Id, null, branchId) : GetBalance(x.Id, null, null)
                }).ToList();

            }

            stopwatch.Stop();

            return itemsList;


        }
        #endregion


        #region فاتورة الجرد
        public List<ItemBalanceDto> SearchItemBalanceInventory(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId)
        {
            if (storeId == null)
                return new List<ItemBalanceDto>();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> items = null;
            items = db.Items.Where(x => !x.IsDeleted);
            if (itemId != null)
                items = items.Where(x => x.Id == itemId);
            else
            {
                if(itemCode != null)
                    items = items.Where(x => x.ItemCode.ToString().Trim().Contains(itemCode));

                if(barCode != null)
                    items = items.Where((x) => x.BarCode.Trim().Contains(barCode));

                if (groupId != null)
                    items = items.Where(x => x.GroupBasicId == groupId);

                if (itemtypeId != null)
                    items = items.Where(x => x.ItemTypeId == itemtypeId);
            }

            List<ItemBalanceDto> itemsList = new List<ItemBalanceDto>();
            itemsList = items.ToList().Select(x => new ItemBalanceDto
            {
                Id = x.Id,
                ItemName = x.Name,
                ItemCode = x.ItemCode.ToString(),
                BarCode = x.BarCode,
                GroupName = x.GroupBasic.Name,
                ItemTypeName = x.ItemType.Name,
                Balance = GetBalance(x.Id, storeId),
                BalanceReal = 0
            }).ToList();

            return itemsList;
        }

        #endregion
    }
}
