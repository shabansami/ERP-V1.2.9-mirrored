using ERP.Web.DataTablesDS;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using ERP.DAL.Models;

namespace ERP.Web.Services
{
    public static class BalanceService
    {
        #region Balance
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
                var storeTranFromBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval);
                if (storeId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFromId == storeId);
                if (branchId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFrom.BranchId == branchId);
                if (dtFrom != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                if (dtTo != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);
                //تحويل مخزنى الى
                var storeTranToBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval );
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
                var productionOrders = db.ProductionOrderDetails.Where(x => !x.IsDeleted&&x.ProductionTypeId==(int)ProductionTypeCl.Out&&x.ItemId==itemId);
                if (storeId != null)
                    productionOrders = productionOrders.Where(x => x.ProductionOrder.ProductionStoreId == storeId);
                if (branchId != null)
                    productionOrders = productionOrders.Where(x => x.ProductionOrder.BranchId == branchId);
                if (dtFrom != null)
                    productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom);
                if (dtTo != null)
                    productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);
                //خامات الانتاج
                var productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted &&  x.ProductionTypeId == (int)ProductionTypeCl.In && x.ItemId == itemId);
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
                var productionOrdersQuantity = productionOrders.Count() > 0 ? productionOrders.Sum(p =>  (double?)p.Quantity??0):0;
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
        //اجمالى رصيد الوارد
        public static double GetBalanceIn(Guid? itemId, Guid? storeId, Guid? branchId = null, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            if (itemId == null) return 0;
            using (var db = new VTSaleEntities())
            {
                double balance = 0;
                var supplyBalance = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    supplyBalance = supplyBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    supplyBalance = supplyBalance.Where(x => x.PurchaseInvoice.BranchId == branchId);
                if (dtFrom != null)
                    supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    supplyBalance = supplyBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);

                var sellBackBalance = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    sellBackBalance = sellBackBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    sellBackBalance = sellBackBalance.Where(x => x.SellBackInvoice.BranchId == branchId);
                if (dtFrom != null)
                    sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    sellBackBalance = sellBackBalance.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) <= dtTo);

                var storeTranToBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval);
                if (storeId != null)
                    storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreToId == storeId);
                if (branchId != null)
                    storeTranToBalance = storeTranToBalance.Where(x => x.StoresTransfer.StoreTo.BranchId == branchId);
                if (dtFrom != null)
                    storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                if (dtTo != null)
                    storeTranToBalance = storeTranToBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);

                var itemIntialBalance = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId && x.IsApproval);
                if (storeId != null)
                    itemIntialBalance = itemIntialBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    itemIntialBalance = itemIntialBalance.Where(x => x.Store.BranchId == branchId);
                if (dtFrom != null)
                    itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) >= dtFrom);
                if (dtTo != null)
                    itemIntialBalance = itemIntialBalance.Where(x => DbFunctions.TruncateTime(x.DateIntial) <= dtTo);

                //var productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted /*&& x.FinalItemId == itemId*/);
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


                //var productionReceiptOrders = db.ProductionOrderReceipts.Where(x => !x.IsDeleted /*&& x.ProductionOrder.FinalItemId == itemId*/);
                //if (storeId != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStoreId == storeId);
                //if (branchId != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => x.FinalItemStore.BranchId == branchId);
                //if (dtFrom != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) >= dtFrom);
                //if (dtTo != null)
                //    productionReceiptOrders = productionReceiptOrders.Where(x => DbFunctions.TruncateTime(x.ReceiptDate) <= dtTo);

                var maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval && x.Maintenance.StoreReceiptId != null);
                if (storeId != null)
                    maintenances = maintenances.Where(x => x.Maintenance.StoreId == storeId);
                if (branchId != null)
                    maintenances = maintenances.Where(x => x.Maintenance.BranchId == branchId);
                if (dtFrom != null)
                    maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);

                var inventoriesBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                if (storeId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.StoreId == storeId);
                if (branchId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.BranchId == branchId);
                if (dtFrom != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) <= dtTo);


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



                var supplyBalanceQuantity = supplyBalance.Count() > 0 ? supplyBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var sellBackBalanceQuantity = sellBackBalance.Count() > 0 ? sellBackBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var storeTranToBalanceQuantity = (storeId == null && branchId == null) ? 0 : storeTranToBalance.Count() > 0 ? storeTranToBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var itemIntialBalanceQuantity = itemIntialBalance.Count() > 0 ? itemIntialBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var productionOrdersQuantity = productionOrders.Count() > 0 ? productionOrders.Sum(p => (double?)p.Quantity ?? 0) : 0;
                //var productionReceiptOrdersQuantity = productionReceiptOrders.Count() > 0 ? productionReceiptOrders.Sum(x => (double?)x.ReceiptQuantity) ?? 0 : 0;
                var maintenancesQuantity = maintenances.Count();
                var StorePermissionReceiveQuantity = StorePermissionReceive.Count() > 0 ? StorePermissionReceive.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var inventy = inventoriesBalance.Count() > 0 ? (inventoriesBalance.Sum(x => x.DifferenceCount)) > 0 ? (inventoriesBalance.Sum(x => x.DifferenceCount)) : 0 : 0;
                balance = (supplyBalanceQuantity +
                    sellBackBalanceQuantity +
                    storeTranToBalanceQuantity +
                    itemIntialBalanceQuantity +
                    productionOrdersQuantity +
                    //productionReceiptOrdersQuantity +
                    maintenancesQuantity + inventy + StorePermissionReceiveQuantity);
                return balance;
            }
        }
        //اجمالى رصيد الصادر
        public static double GetBalanceOut(Guid? itemId, Guid? storeId, Guid? branchId = null, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            if (itemId == null) return 0;
            using (var db = new VTSaleEntities())
            {
                double balance = 0;

                var supplyBackBalance = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    supplyBackBalance = supplyBackBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    supplyBackBalance = supplyBackBalance.Where(x => x.PurchaseBackInvoice.BranchId == branchId);
                if (dtFrom != null)
                    supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    supplyBackBalance = supplyBackBalance.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) <= dtTo);

                var sellBalance = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.IsFinalApproval && x.ItemId == itemId);
                if (storeId != null)
                    sellBalance = sellBalance.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    sellBalance = sellBalance.Where(x => x.SellInvoice.BranchId == branchId);
                if (dtFrom != null)
                    sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    sellBalance = sellBalance.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);

                var storeTranFromBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.IsFinalApproval);
                if (storeId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFromId == storeId);
                if (branchId != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => x.StoresTransfer.StoreFrom.BranchId == branchId);
                if (dtFrom != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) >= dtFrom);
                if (dtTo != null)
                    storeTranFromBalance = storeTranFromBalance.Where(x => DbFunctions.TruncateTime(x.StoresTransfer.TransferDate) <= dtTo);



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

                var maintenanceReceipts = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval);
                if (storeId != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.StoreReceiptId == storeId);
                if (branchId != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => x.Maintenance.BranchId == branchId);
                if (dtFrom != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenanceReceipts = maintenanceReceipts.Where(x => DbFunctions.TruncateTime(x.Maintenance.InvoiceDate) <= dtTo);

                var maintenanceSpareParts = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId && x.MaintenanceDetail.Maintenance.IsFinalApproval);
                if (storeId != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => x.StoreId == storeId);
                if (branchId != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => x.Store.BranchId == branchId);
                if (dtFrom != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    maintenanceSpareParts = maintenanceSpareParts.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) <= dtTo);

                var inventoriesBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                if (storeId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.StoreId == storeId);
                if (branchId != null)
                    inventoriesBalance = inventoriesBalance.Where(x => x.InventoryInvoice.BranchId == branchId);
                if (dtFrom != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    inventoriesBalance = inventoriesBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) <= dtTo);

                var damagesBalance = db.DamageInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
                if (storeId != null)
                    damagesBalance = damagesBalance.Where(x => x.DamageInvoice.StoreId == storeId);
                if (branchId != null)
                    damagesBalance = damagesBalance.Where(x => x.DamageInvoice.Store.BranchId == branchId);
                if (dtFrom != null)
                    damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) >= dtFrom);
                if (dtTo != null)
                    damagesBalance = damagesBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) <= dtTo);

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




                var sellBalanceQuantity = sellBalance.Count() > 0 ? sellBalance.Sum(x => (double?)x.Quantity) : 0;
                var supplyBackBalanceQuantity = supplyBackBalance.Count() > 0 ? supplyBackBalance.Sum(x => (double?)x.Quantity) : 0;
                var storeTranFromBalanceQuantity = (storeId == null && branchId == null) ? 0 : storeTranFromBalance.Count() > 0 ? storeTranFromBalance.Sum(x => (double?)x.Quantity) : 0;
                var productionOrderDetailsQuantity = productionOrderDetails.Count() > 0 ? productionOrderDetails.Sum(x => (double?)(x.Quantity + x.Quantitydamage)) ?? 0 : 0;
                var maintenanceReceiptsQuantity = maintenanceReceipts.Count();
                var maintenanceSparePartsQuantity = maintenanceSpareParts.Count() > 0 ? maintenanceSpareParts.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var inventy = inventoriesBalance.Count() < 0 ? (inventoriesBalance.Sum(x => x.DifferenceCount)) > 0 ? (inventoriesBalance.Sum(x => x.DifferenceCount)) : 0 : 0;
                var damagesBalanceQuantity = damagesBalance.Count() > 0 ? damagesBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var StorePermissionLeaveQuantity = StorePermissionLeave.Count() > 0 ? StorePermissionLeave.Sum(x => (double?)x.Quantity) ?? 0 : 0;

                balance = (
                    sellBalanceQuantity +
                   supplyBackBalanceQuantity +
                    storeTranFromBalanceQuantity +
                    productionOrderDetailsQuantity +
                    maintenanceReceiptsQuantity +
                   maintenanceSparePartsQuantity + damagesBalanceQuantity + StorePermissionLeaveQuantity +
                    inventy) ?? 0;
                return balance;
                // الحصول على بيانات المؤسسة من الاعدادات
                //var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                //var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                //if (!string.IsNullOrEmpty(schema))
                //     return db.Database.SqlQuery<double>($"select [{schema}].[GetBalance](@ItemId,@StoreId)", new SqlParameter("@ItemId", itemId), new SqlParameter("@StoreId", storeId)).FirstOrDefault();
                //else
                //    return 0;

            }
        }
        //رصيد الصنف فى كل المخازن بدلالة فرع
        public static double GetBalanceByBranchAllStores(int? itemId, int? branchId)
        {
            using (var db = new VTSaleEntities())
            {
                // الحصول على بيانات المؤسسة من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                if (!string.IsNullOrEmpty(schema))
                    return db.Database.SqlQuery<double>($"select [{schema}].[GetBalanceByBranchAllStores](@ItemId,@BranchId)", new SqlParameter("@ItemId", itemId), new SqlParameter("@BranchId", branchId)).FirstOrDefault();
                else
                    return 0;
            }
        }
        //رصيد الصنف فى كل المخازن فى كل الفرع
        public static double GetBalanceByItemAllStores(int? itemId)
        {
            using (var db = new VTSaleEntities())
            {
                // الحصول على بيانات المؤسسة من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                if (!string.IsNullOrEmpty(schema))
                    return db.Database.SqlQuery<double>($"select [{schema}].[GetBalanceByItemAllStores](@ItemId)", new SqlParameter("@ItemId", itemId)).FirstOrDefault();
                else
                    return 0;
            }

        }
        //رصيد الصنف فى امر انتاج 
        public static double GetBalanceByProductionOrder(Guid? itemId, Guid? storeId, Guid? productionOrderId)
        {
            using (var db = new VTSaleEntities())
            {
                double balance = 0;
                var sellBalance = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoice.IsFinalApproval && x.StoreId == storeId && x.ItemId == itemId && x.ItemSerial.ProductionOrderId == productionOrderId);
                var sellBackBalance = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoice.IsFinalApproval && x.StoreId == storeId && x.ItemId == itemId && x.ItemSerial.ProductionOrderId == productionOrderId);
                var storeTranFromBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.StoreFromId == storeId && x.StoresTransfer.IsFinalApproval  && x.ProductionOrderId == productionOrderId);
                var storeTranToBalance = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StoresTransfer.StoreToId == storeId && x.StoresTransfer.IsFinalApproval && x.StoresTransfer.IsFinalApproval && x.ProductionOrderId == productionOrderId);
                //var productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted /*&& x.FinalItemId == itemId*/ && x.ProductionStoreId == storeId && x.Id == productionOrderId);
                var productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted && x.ProductionOrderDetails.Where(d => !d.IsDeleted && d.ProductionTypeId == (int)ProductionTypeCl.Out && d.ItemId == itemId && x.ProductionStoreId == storeId).Any());

                var productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.ProductionOrder.ProductionUnderStoreId == storeId && x.ProductionOrderId == productionOrderId);
                //var productionReceiptOrders = db.ProductionOrderReceipts.Where(x => !x.IsDeleted && x.FinalItemStoreId == itemId && x.FinalItemStoreId == storeId && x.ProductionOrderId == productionOrderId);
                var maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.Maintenance.IsFinalApproval && x.Maintenance.StoreReceiptId != null && x.Maintenance.StoreId == storeId && x.ItemSerial.ProductionOrderId == productionOrderId);
                var maintenanceReceipts = db.MaintenanceDetails.Where(x => !x.IsDeleted && x.Maintenance.StoreReceiptId == storeId && x.ItemId == itemId && x.Maintenance.IsFinalApproval && x.ItemSerial.ProductionOrderId == productionOrderId);
                var maintenanceSpareParts = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId && x.MaintenanceDetail.Maintenance.IsFinalApproval && x.StoreId == storeId && x.ProductionOrderId == productionOrderId);
                //var inventoriesBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId&& x.InventoryInvoice.StoreId == storeId);

                var sellBackBalanceQuantity = sellBackBalance.Count() > 0 ? sellBackBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var storeTranToBalanceQuantity = storeTranToBalance.Count() > 0 ? storeTranToBalance.Sum(x => (double?)x.Quantity) ?? 0 : 0;
                var productionOrdersQuantity = productionOrders.Count() > 0 ? productionOrders.Sum(p => p.ProductionOrderDetails.Sum(c => (double?)c.Quantity ?? 0)) : 0;
                //var productionReceiptOrdersQuantity = productionReceiptOrders.Count() > 0 ? productionReceiptOrders.Sum(x => (double?)x.ReceiptQuantity) ?? 0 : 0;
                var maintenancesQuantity = maintenances.Count();
                var sellBalanceQuantity = sellBalance.Count() > 0 ? sellBalance.Sum(x => (double?)x.Quantity) : 0;
                var storeTranFromBalanceQuantity = storeTranFromBalance.Count() > 0 ? storeTranFromBalance.Sum(x => (double?)x.Quantity) : 0;
                var productionOrderDetailsQuantity = productionOrderDetails.Count() > 0 ? productionOrderDetails.Sum(x => (double?)(x.Quantity + x.Quantitydamage)) ?? 0 : 0;
                var maintenanceReceiptsQuantity = maintenanceReceipts.Count();
                var maintenanceSparePartsQuantity = maintenanceSpareParts.Count() > 0 ? maintenanceSpareParts.Sum(x => (double?)x.Quantity) ?? 0 : 0;


                balance = (sellBackBalanceQuantity +
                   storeTranToBalanceQuantity +
                    productionOrdersQuantity +
                    //productionReceiptOrdersQuantity +
                    maintenancesQuantity) - (
                    sellBalanceQuantity +
                    storeTranFromBalanceQuantity +
                    productionOrderDetailsQuantity +
                    maintenanceReceiptsQuantity +
                   maintenanceSparePartsQuantity) ?? 0;
                return balance;
                // الحصول على بيانات المؤسسة من الاعدادات
                //var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                //var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                //if (!string.IsNullOrEmpty(schema))
                //    return db.Database.SqlQuery<double>($"select [{schema}].[GetBalanceByProductionOrder](@ItemId,@StoreId,@ProductionOrderId)", new SqlParameter("@ItemId", itemId), new SqlParameter("@StoreId", storeId), new SqlParameter("@ProductionOrderId", productionOrderId)).FirstOrDefault();
                //else
                //    return 0;
            }
        }
        //رصيد الصنف اول المدة للمنتجات النهائية التى يتم بيعها مع اوامر التشغيل 
        public static double GetBalanceItemIntial(int? itemId, int? storeId)
        {
            using (var db = new VTSaleEntities())
            {
                // الحصول على بيانات المؤسسة من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
                if (!string.IsNullOrEmpty(schema))
                    return db.Database.SqlQuery<double>($"select [{schema}].[GetBalanceItemIntial](@ItemId,@StoreId)", new SqlParameter("@ItemId", itemId), new SqlParameter("@StoreId", storeId)).FirstOrDefault();
                else
                    return 0;
            }
        }
        #endregion

        #region تقارير أرصدة الاصناف 
        //بحث الارصدة
        public static List<ItemBalanceDto> SearchItemBalance(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId, bool isFirstInitPage, string txtSearch = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> items = null;
            using (var db = new VTSaleEntities())
            {
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


                    //.Select(n=>new ItemBalanceDto {
                    //    Id = n.Id,
                    //    ItemName = n.ItemName,
                    //    ItemCode = n.ItemCode.ToString(),
                    //    BarCode = n.ItemCode,
                    //    GroupName = n.GroupName,
                    //    ItemTypeName = n.ItemTypeName,
                    //    Balance = storeId != null ? GetBalance(n.Id, storeId) : branchId != null ? GetBalanceByBranchAllStores(n.Id, branchId) : GetBalanceByItemAllStores(n.Id)

                    //}).ToList();

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

                //فى حالة عدم ادخال بيانات للصنف او الاصناف يتم اظهار كل ارصدة الاصناف 
                //if (itemsList.Count==0)
                //{
                //    itemsList = db.Items.Where(x => !x.IsDeleted).Select(x => new ItemBalanceDto
                //    {
                //        Id = x.Id,
                //        ItemName = x.Name,
                //        ItemCode = x.ItemCode.ToString(),
                //        BarCode = x.BarCode,
                //        GroupName = x.Group.Name,
                //        ItemTypeName = x.ItemType.Name
                //    }).ToList();
                //}
                stopwatch.Stop();
                //foreach (var item in itemsList)
                //{
                //    if (storeId!=null)
                //        //رصيد الصنف فى المخزن 
                //        item.Balance = GetBalance(item.Id, storeId);
                //    else if(branchId!=null)
                //        //رصيد الصنف فى الفرع 
                //        item.Balance = GetBalanceByBranchAllStores(item.Id, branchId);
                //    else
                //        //رصيد الصنف فى كل الفروع 
                //        item.Balance = GetBalanceByItemAllStores(item.Id);
                //}

                return itemsList;
                //if (itemId != null)
                //    items.Where(x => x.Id == itemId);
                //if (itemCode != null)
                //    items.Where(x => x.ItemCode.ToString().Contains(itemCode));
                //if (barCode != null)
                //    items.Where(x => x.BarCode.Contains(barCode));
                //if (groupId != null)
                //    items.Where(x => x.GroupSellId==groupId);
                //if (itemtypeId != null)
                //    items.Where(x => x.ItemTypeId== itemtypeId);


            }
        }
        #endregion

        #region ارصدة بدلالة اوامر التشغيل
        //عرض ارصدة الصنف فى كل امر انتاج 
        public class SelectedListDropDown
        {
            public string Text { get; set; }
            public string Val { get; set; }
        }
        public static List<SelectedListDropDown> GetBalanceProductionOrder(Guid storeId, Guid itemId)
        {
            List<SelectedListDropDown> list = new List<SelectedListDropDown>();
            using (var db = new VTSaleEntities())
            {
                //var itemList = db.ProductionOrders.Where(x => !x.IsDeleted && x.ProductionOrderDetails.Where(p=>!p.IsDeleted&&p.ItemId==itemId).Any() && x.ProductionStoreId == storeId);
                ////var itemIntialList = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.IsApproval && x.ItemId == itemId && x.StoreId == storeId && x.Item.AvaliableToSell).ToList();
                //var serial = db.ItemSerials.Where(x => !x.IsDeleted && (x.SellInvoiceId == null || x.MaintenanceId == null) && x.CurrentStoreId == storeId && x.ItemId == itemId);
                //var serialCount = serial.Count();
                //تخزين رصيد اول المدة اولا ثم اوامر الانتاج وبالارصده 
                //if (itemIntialList.Count > 0)
                //{
                //    var balance = GetBalanceItemIntial(itemId, storeId).ToString();
                //    list.Add(new SelectedListDropDown
                //    {
                //        Text = $"رصيد اول المدة :: رصيد الصنف : {balance}",
                //        Val = $"{{\"balance\":{balance},\"productOrder\":null,\"isIntial\":1}}"
                //    });
                //}

                //if (itemList.Count() > 0)  // الصنف منتج نهائى وله امر انتاج
                //{
                //    foreach (var productionOrder in itemList)
                //    {
                //        var balance = GetBalanceByProductionOrder(/*productionOrder.FinalItemId*/null, storeId, productionOrder.Id);
                //        //لمعرفة عدد الاصناف الموجود فى المخزن حسب السيريال
                //        if (serialCount > 0)
                //            balance = balance - serialCount;

                //        list.Add(new SelectedListDropDown
                //        {
                //            Text = $"امر تشغيل : {productionOrder.Id} || رصيد الصنف : {balance}",
                //            Val = $"{{\"balance\":{balance},\"productOrder\":{productionOrder.Id},\"serialItemId\":null,\"isIntial\":null,\"currentStore\":null}}"
                //        });


                //    }
                //}
                //else //الصنف ليس منتج نهائى وليس له امر انتاج
                //{
                    //الرصيد الفعلى للصنف بالاصناف التى بها سيريال 
                    var balance = GetBalance(itemId, storeId);

                    //لمعرفة عدد الاصناف الموجود فى المخزن حسب السيريال
                    //if (serialCount > 0)
                    //    balance = balance - serialCount;
                    list.Add(new SelectedListDropDown
                    {
                        Text = $"الرصيد المتاح للصنف : {balance.ToString()}",
                        Val = $"{{\"balance\":{balance},\"productOrder\":null,\"isIntial\":null,\"serialItemId\":null,\"currentStore\":null}}"
                    });







                //}


                //اضافة اى اصناف بسيريال وحالتها مرتجع او صيانه
                //var serialItems = serial.ToList();
                //foreach (var item in serialItems)
                //{
                //    var lastCase = "";
                //    string productionOrderId = "null";
                //    string currentStore = "null";
                //    if (item.SerialCaseId == (int)SerialCaseCl.SellBack)
                //        lastCase = "مرتجع بيع";
                //    if (item.SerialCaseId == (int)SerialCaseCl.RepairedReceiptStore)
                //        lastCase = " صيانه بالمخزن";
                //    if (item.ProductionOrderId != null)
                //        productionOrderId = item.ProductionOrderId.ToString();
                //    if (item.CurrentStoreId != null)
                //        currentStore = item.CurrentStoreId.ToString();
                //    list.Add(new SelectedListDropDown
                //    {
                //        Text = $"سيريال : {item.SerialNumber} | {lastCase}",
                //        Val = $"{{\"balance\":{1},\"productOrder\":{productionOrderId},\"serialItemId\":{item.Id},\"isIntial\":null,\"currentStore\":{currentStore}}}"
                //    });
                //}
                return list;
            }
        }
        #endregion


        #region تقارير اصناف بلغت الطلب الامان 
        public static List<ItemBalanceDto> ItemRequestLimitSafety(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? BranchId, Guid? StoreId)
        {
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            //IQueryable<Item> itemss = null;
            using (var db = new VTSaleEntities())
            {
                var itemss = db.Items.Where(x => !x.IsDeleted).ToList();
                var items = new List<Item>();
                if (StoreId != null)
                    items = itemss.Where(x => x.RequestLimit1 >= GetBalance(x.Id, StoreId, null)).ToList();
                else if (BranchId != null)
                    items = itemss.Where(x => x.RequestLimit1 >= GetBalance(x.Id, null, BranchId)).ToList();
                else
                    items = itemss.Where(x => x.RequestLimit1 >= GetBalance(x.Id, null, null)).ToList();


                if (itemId != null)
                    items.Where(x => x.Id == itemId);
                else
                    items.Where(x =>
                     x.ItemCode.ToString().Trim().Contains(itemCode)
                     || x.BarCode.Trim().Contains(barCode)
                     || x.GroupBasicId == groupId
                     || x.ItemTypeId == itemtypeId);
                List<ItemBalanceDto> itemsList = new List<ItemBalanceDto>();
                itemsList = items.Select(x => new ItemBalanceDto
                {
                    Id = x.Id,
                    ItemName = x.Name,
                    Limit = x.RequestLimit1 ?? 0,
                    Balance = StoreId != null ? GetBalance(x.Id, StoreId, null) : BranchId != null ? GetBalance(x.Id, null, BranchId) : GetBalance(x.Id, null, null),
                    GroupName = x.GroupBasic.Name,
                    ItemTypeName = x.ItemType.Name
                }).ToList();


                return itemsList;
            }
        }
        #endregion
        #region تقارير اصناف بلغت الطلب الخطر 
        public static List<ItemBalanceDto> ItemRequestLimitDanger(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? BranchId, Guid? StoreId)
        {
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            //IQueryable<Item> items = null;
            using (var db = new VTSaleEntities())
            {
                //var items = db.Items.Where(x => !x.IsDeleted).ToList().Where(x => x.RequestLimit2 >= GetBalance(x.Id, null, null));
                var itemss = db.Items.Where(x => !x.IsDeleted).ToList();
                var items = new List<Item>();
                if (StoreId != null)
                    items = itemss.Where(x => x.RequestLimit2 >= GetBalance(x.Id, StoreId, null)).ToList();
                else if (BranchId != null)
                    items = itemss.Where(x => x.RequestLimit2 >= GetBalance(x.Id, null, BranchId)).ToList();
                else
                    items = itemss.Where(x => x.RequestLimit2 >= GetBalance(x.Id, null, null)).ToList();


                if (itemId != null)
                    items.Where(x => x.Id == itemId);
                else
                    items.Where(x =>
                     x.ItemCode.ToString().Trim().Contains(itemCode)
                     || x.BarCode.Trim().Contains(barCode)
                     || x.GroupBasicId == groupId
                     || x.ItemTypeId == itemtypeId);
                List<ItemBalanceDto> itemsList = new List<ItemBalanceDto>();
                itemsList = items.Select(x => new ItemBalanceDto
                {
                    Id = x.Id,
                    ItemName = x.Name,
                    Limit = x.RequestLimit2 ?? 0,
                    Balance = StoreId != null ? GetBalance(x.Id, StoreId, null) : BranchId != null ? GetBalance(x.Id, null, BranchId) : GetBalance(x.Id, null, null),
                    GroupName = x.GroupBasic.Name,
                    ItemTypeName = x.ItemType.Name
                }).ToList();


                return itemsList;
            }
        }
        #endregion
        #region تقارير اصناف الاعلى والاقل مبيعا 
        public static List<TopLowestSellingDto> SearchTopLowestSelling(Guid? CustomerId, Guid? BranchId, int? TypeSellingId, string dFrom, string dTo)
        {
            IQueryable<SellInvoicesDetail> sellItemInvoices = null;
            List<TopLowestSellingDto> items = new List<TopLowestSellingDto>();
            using (var db = new VTSaleEntities())
            {
                sellItemInvoices = db.SellInvoicesDetails.Where(x => !x.IsDeleted);
                if (BranchId != null)
                    sellItemInvoices = sellItemInvoices.Where(x => x.SellInvoice.BranchId == BranchId);
                if (CustomerId != null)
                    sellItemInvoices = sellItemInvoices.Where(x => x.SellInvoice.CustomerId == CustomerId);
                if (DateTime.TryParse(dFrom, out DateTime dtFrom))
                    sellItemInvoices = sellItemInvoices.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom);
                if (DateTime.TryParse(dTo, out DateTime dtTo))
                    sellItemInvoices = sellItemInvoices.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);

                items = sellItemInvoices.GroupBy(x => x.ItemId).Select(x => new TopLowestSellingDto
                {
                    ItemId = x.Key,
                    ItemCode = x.FirstOrDefault().Item.ItemCode,
                    ItemName = x.FirstOrDefault().Item.Name,
                    UnitName = x.FirstOrDefault().Item.Unit.Name,
                    Quantity = x.Count(),
                    TopSellPrice = x.Max(i => i.Price),
                    LowSellPrice = x.Min(i => i.Price)
                }).ToList();
                if (TypeSellingId == 1)//الاعلى مبيعا
                    items = items.OrderByDescending(x => x.Quantity).ToList();
                else//الاقل مبيعا
                    items = items.OrderBy(x => x.Quantity).ToList();
                return items;
            }
        }
        #endregion

        #region فاتورة الجرد
        public static List<ItemBalanceDto> SearchItemBalanceInventory(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId)
        {
            if (storeId == null)
                return new List<ItemBalanceDto>();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> items = null;
            using (var db = new VTSaleEntities())
            {
                items = db.Items.Where(x => !x.IsDeleted);
                if (itemId != null)
                    items = items.Where(x => x.Id == itemId);
                else
                    items = items.Where(x =>
                    x.ItemCode.ToString().Trim().Contains(itemCode)
                    || x.BarCode.Trim().Contains(barCode)
                    || x.GroupBasicId == groupId
                    || x.ItemTypeId == itemtypeId);

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
        }

        #endregion       
        #region فاتورة الهالك
        public static List<ItemBalanceDto> SearchItemQuantityDamages(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? storeId)
        {
            if (storeId == null)
                return new List<ItemBalanceDto>();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> items = null;
            using (var db = new VTSaleEntities())
            {
                items = db.Items.Where(x => !x.IsDeleted);
                if (itemId != null)
                    items = items.Where(x => x.Id == itemId);
                else
                    items = items.Where(x =>
                    x.ItemCode.ToString().Trim().Contains(itemCode)
                    || x.BarCode.Trim().Contains(barCode)
                    || x.GroupBasicId == groupId
                    || x.ItemTypeId == itemtypeId);

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
        }

        #endregion

        #region تقرير حركة صنف 
        //بحث الارصدة
        public static List<ItemActionDto> SearchItemActions(string itemCode, string barCode, Guid? itemId, Guid? storeId, int? ActionTypeId, DateTime? dtFrom, DateTime? dtTo)
        {
            /*
             ActionType
            1: رصيد اول
            2: مشتريات
            3: مرتجع مشتريات
            4: بيع
            5: مرتجع بيع
            6: قطع غيار
            7: اذن استلام
            8: اذن صرف
             */
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> itemIQ = null;
            using (var db = new VTSaleEntities())
            {
                itemIQ = db.Items.Where(x => !x.IsDeleted);
                if (itemId != null)
                    itemIQ = itemIQ.Where(x => x.Id == itemId);
                else if (itemCode != null)
                {
                    if (!string.IsNullOrEmpty(itemCode))
                        itemIQ = itemIQ.Where(x => x.ItemCode == itemCode);
                }
                else if (barCode != null)
                    itemIQ = itemIQ.Where(x => x.BarCode == barCode);
                else
                    return new List<ItemActionDto>();

                var item = itemIQ.FirstOrDefault();
                List<ItemActionDto> itemsList = new List<ItemActionDto>();
                if (item != null)
                {
                    //        1: رصيد اول
                    //2: مشتريات
                    //3: مرتجع مشتريات
                    //4: بيع
                    //5: مرتجع بيع
                    //6: قطع غيار
                    // 7: اذن استلام
                    //8: اذن صرف
                    if (ActionTypeId != null)
                    {
                        if (!int.TryParse(ActionTypeId.ToString(), out var actionType))
                            return itemsList;
                        switch (actionType)
                        {
                            case 0:
                                //الكل  
                                var list = new List<ItemActionDto>();
                                list.AddRange(InitialBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(PurchaseBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(PurchaseBackBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(SellBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(SellBackBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(MaintenanceSparePartBalances(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(StorePermissionBalances(db, item.Id, storeId, dtFrom, dtTo, true)); //اذن صرف
                                list.AddRange(StorePermissionBalances(db, item.Id, storeId, dtFrom, dtTo, false));//اذن استلام
                                return list;
                            case 1:
                                //رصيد اول  
                                return InitialBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 2:
                                //2: مشتريات
                                return PurchaseBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 3:
                                //3: مرتجع مشتريات
                                return PurchaseBackBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 4:
                                //4: بيع
                                return SellBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 5:
                                //5: مرتجع بيع
                                return SellBackBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 6:
                                //6: قطع غيار
                                return MaintenanceSparePartBalances(db, item.Id, storeId, dtFrom, dtTo);
                            case 7:
                                //7: اذن استلام
                                return StorePermissionBalances(db, item.Id, storeId, dtFrom, dtTo, true);
                            case 8:
                                //8: اذن صرف
                                return StorePermissionBalances(db, item.Id, storeId, dtFrom, dtTo, false);
                            default:
                                break;
                        }
                    }


                }
                stopwatch.Stop();
                return itemsList;

            }
        }


        //رصيد اول
        static List<ItemActionDto> InitialBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<ItemIntialBalance> initialBalanceIQ = null;
            initialBalanceIQ = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => DbFunctions.TruncateTime(x.DateIntial) >= dtFrom && DbFunctions.TruncateTime(x.DateIntial) <= dtTo);
            return initialBalanceIQ
                 .Select(x => new ItemActionDto
                 {
                     ActionType = "رصيد اول",
                     InvoiceDate = x.DateIntial.ToString(),
                     InvoiceNumber = null,
                     PersonName = "رصيد اول",
                     StoreName = x.Store != null ? x.Store.Name : null,
                     Quantity = x.Quantity,
                     Price = x.Price,
                     Amount = x.Amount
                 }).ToList();
        }

        static List<ItemActionDto> PurchaseBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<PurchaseInvoicesDetail> purchaseBalanceIQ = null;
            purchaseBalanceIQ = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                purchaseBalanceIQ = purchaseBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                purchaseBalanceIQ = purchaseBalanceIQ.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);
            return purchaseBalanceIQ
                .Select(x => new ItemActionDto
                {
                    ActionType = "مشتريات",
                    InvoiceDate = x.PurchaseInvoice.InvoiceDate.ToString(),
                    InvoiceNumber = x.PurchaseInvoice.InvoiceNumber,
                    PersonName = x.PurchaseInvoice.PersonSupplier.Name,
                    StoreName = x.Store != null ? x.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        //مرتجع مشتريات
        static List<ItemActionDto> PurchaseBackBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<PurchaseBackInvoicesDetail> purchaseBackBalanceIQ = null;
            purchaseBackBalanceIQ = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                purchaseBackBalanceIQ = purchaseBackBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                purchaseBackBalanceIQ = purchaseBackBalanceIQ.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) <= dtTo);
            return purchaseBackBalanceIQ
                .Select(x => new ItemActionDto
                {
                    ActionType = "مرتجع مشتريات",
                    InvoiceDate = x.PurchaseBackInvoice.InvoiceDate.ToString(),
                    InvoiceNumber = x.PurchaseBackInvoice.InvoiceNumber,
                    PersonName = x.PurchaseBackInvoice.PersonSupplier.Name,
                    StoreName = x.Store != null ? x.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        //بيع
        static List<ItemActionDto> SellBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<SellInvoicesDetail> sellBalanceIQ = null;
            sellBalanceIQ = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                sellBalanceIQ = sellBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                sellBalanceIQ = sellBalanceIQ.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
            return sellBalanceIQ
                .Select(x => new ItemActionDto
                {
                    ActionType = "بيع",
                    InvoiceDate = x.SellInvoice.InvoiceDate.ToString(),
                    InvoiceNumber = x.SellInvoice.InvoiceNumber,
                    PersonName = x.SellInvoice.PersonCustomer.Name,
                    StoreName = x.Store != null ? x.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        //مرتجع بيع
        static List<ItemActionDto> SellBackBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<SellBackInvoicesDetail> sellBackBalanceIQ = null;
            sellBackBalanceIQ = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                sellBackBalanceIQ = sellBackBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                sellBackBalanceIQ = sellBackBalanceIQ.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) <= dtTo);
            return sellBackBalanceIQ
                .Select(x => new ItemActionDto
                {
                    ActionType = "مرتجع بيع ",
                    InvoiceDate = x.SellBackInvoice.InvoiceDate.ToString(),
                    InvoiceNumber = x.SellBackInvoice.InvoiceNumber,
                    PersonName = x.SellBackInvoice.PersonCustomer.Name,
                    StoreName = x.Store != null ? x.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        //قطع غيار
        static List<ItemActionDto> MaintenanceSparePartBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<MaintenanceSparePart> maintenanceSparePartBalanceIQ = null;
            maintenanceSparePartBalanceIQ = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                maintenanceSparePartBalanceIQ = maintenanceSparePartBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                maintenanceSparePartBalanceIQ = maintenanceSparePartBalanceIQ.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) <= dtTo);
            return maintenanceSparePartBalanceIQ
                .Select(x => new ItemActionDto
                {
                    ActionType = "قطع غيار",
                    InvoiceDate = x.MaintenanceDetail != null ? x.MaintenanceDetail.Maintenance.InvoiceDate.ToString() : null,
                    InvoiceNumber = x.MaintenanceDetail != null ? x.MaintenanceDetail.Maintenance.InvoiceNumber : null,
                    PersonName = x.MaintenanceDetail != null ? x.MaintenanceDetail.Maintenance.Person.Name : null,
                    StoreName = x.Store != null ? x.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        //اذن استلام
        static List<ItemActionDto> StorePermissionBalances(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo, bool isRecieve)
        {
            IQueryable<StorePermissionItem> storePermission = null;
            storePermission = db.StorePermissionItems.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.StorePermission.IsReceive == isRecieve);
            if (storeId != null)
                storePermission = storePermission.Where(x => x.StorePermission.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                storePermission = storePermission.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom && DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);
            return storePermission
                .Select(x => new ItemActionDto
                {
                    ActionType = x.StorePermission.IsReceive == true ? "اذن استلام" : "اذن صرف",

                    InvoiceDate = x.StorePermission != null ? x.StorePermission.PermissionDate.ToString() : null,
                    InvoiceNumber = null,
                    PersonName = x.StorePermission != null ? x.StorePermission.Person.Name : null,
                    StoreName = x.StorePermission.Store != null ? x.StorePermission.Store.Name : null,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Amount
                }).ToList();
        }
        #endregion




































        #region تقرير حركة صنف 3
        public static List<ItemMovementdDto> SearchItemMovement(string itemCode, string barCode, Guid? itemId, Guid? storeId, int? ActionTypeId, DateTime? dtFrom, DateTime? dtTo)
        {
            /*
             ActionType
            1: رصيد اول
            2: مشتريات
            3: مرتجع مشتريات
            4: بيع
            5: مرتجع بيع
            6: قطع غيار
            7: اذن استلام
            8: اذن صرف
            9: هالك
            10:فرق جرد
            11:امر انتاج صادر
            12:امر انتاج وارد
             */

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (itemCode == string.Empty)
                itemCode = null;
            if (barCode == string.Empty)
                barCode = null;
            IQueryable<Item> itemIQ = null;
            using (var db = new VTSaleEntities())
            {
                itemIQ = db.Items.Where(x => !x.IsDeleted);
                if (itemId != null)
                    itemIQ = itemIQ.Where(x => x.Id == itemId);
                else if (itemCode != null)
                {
                    if (!string.IsNullOrEmpty(itemCode))
                        itemIQ = itemIQ.Where(x => x.ItemCode == itemCode);
                }
                else if (barCode != null)
                    itemIQ = itemIQ.Where(x => x.BarCode == barCode);
                else
                    return new List<ItemMovementdDto>();

                var item = itemIQ.FirstOrDefault();
                List<ItemMovementdDto> itemsList = new List<ItemMovementdDto>();
                if (item != null)
                {
                    //        1: رصيد اول
                    //2: مشتريات
                    //3: مرتجع مشتريات
                    //4: بيع
                    //5: مرتجع بيع
                    //6: قطع غيار
                    //7: اذن استلام
                    //8: اذن صرف
                    //9: هالك
                    //10:فرق جرد
                    //11:امر انتاج صادر
                    //12:امر انتاج وارد
                    if (ActionTypeId != null)
                    {
                        if (!int.TryParse(ActionTypeId.ToString(), out var actionType))
                            return itemsList;
                        switch (actionType)
                        {
                            case 0:
                                //الكل  
                                var list = new List<ItemMovementdDto>();
                                list.AddRange(InitialBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(PurchaseBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(PurchaseBackBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(SellBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(SellBackBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(MaintenanceSparePartBalancesMovement(db, item.Id, storeId, dtFrom, dtTo));
                                list.AddRange(StorePermissionBalancesMovement(db, item.Id, storeId, dtFrom, dtTo, true)); //اذن صرف
                                list.AddRange(StorePermissionBalancesMovement(db, item.Id, storeId, dtFrom, dtTo, false));//اذن استلام
                                list.AddRange(DamagesBalanceMovement(db, item.Id, storeId, dtFrom, dtTo));//هالك
                                list.AddRange(InventoryBalanceMovement(db, item.Id, storeId, dtFrom, dtTo));//فرق جرد
                                list.AddRange(ProductionOrderBalanceOutcoming(db, item.Id, storeId, dtFrom, dtTo));//امر انتاج صادر
                                list.AddRange(ProductionOrderBalanceIncoming(db, item.Id, storeId, dtFrom, dtTo));//امر انتاج وارد
                                return list;
                            case 1:
                                //رصيد اول  
                                return InitialBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 2:
                                //2: مشتريات
                                return PurchaseBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 3:
                                //3: مرتجع مشتريات
                                return PurchaseBackBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 4:
                                //4: بيع
                                return SellBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 5:
                                //5: مرتجع بيع
                                return SellBackBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 6:
                                //6: قطع غيار
                                return MaintenanceSparePartBalancesMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 7:
                                //7: اذن استلام
                                return StorePermissionBalancesMovement(db, item.Id, storeId, dtFrom, dtTo, true);
                            case 8:
                                //8: اذن صرف
                                return StorePermissionBalancesMovement(db, item.Id, storeId, dtFrom, dtTo, false);
                            case 9:
                                //9: هالك
                                return DamagesBalanceMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 10:
                                //10:فرق جرد
                                return InventoryBalanceMovement(db, item.Id, storeId, dtFrom, dtTo);
                            case 11:
                                //11:امر انتاج صادر
                                return ProductionOrderBalanceOutcoming(db, item.Id, storeId, dtFrom, dtTo);
                            case 12:
                                //12:امر انتاج وارد
                                return ProductionOrderBalanceIncoming(db, item.Id, storeId, dtFrom, dtTo);
                            default:
                                break;
                        }
                    }


                }
                stopwatch.Stop();
                return itemsList;

            }
        }
        //رصيد اول
        static List<ItemMovementdDto> InitialBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<ItemIntialBalance> initialBalanceIQ = null;
            initialBalanceIQ = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => DbFunctions.TruncateTime(x.DateIntial) >= dtFrom && DbFunctions.TruncateTime(x.DateIntial) <= dtTo);
            return initialBalanceIQ
                 .Select(x => new ItemMovementdDto
                 {
                     ActionType = "رصيد اول",
                     ActionDate = x.DateIntial.ToString(),
                     IncomingQuantity = x.Quantity,
                     IncomingCost = x.Amount,
                 }).ToList();
        }

        static List<ItemMovementdDto> PurchaseBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<PurchaseInvoicesDetail> purchaseBalanceIQ = null;
            purchaseBalanceIQ = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                purchaseBalanceIQ = purchaseBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                purchaseBalanceIQ = purchaseBalanceIQ.Where(x => DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.PurchaseInvoice.InvoiceDate) <= dtTo);
            return purchaseBalanceIQ
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "مشتريات",
                    ActionNumber=x.PurchaseInvoice.InvoiceNumber,
                    ActionDate = x.PurchaseInvoice.InvoiceDate.ToString(),
                    IncomingQuantity = x.Quantity,
                    IncomingCost = x.Amount,
                }).ToList();
        }
        //مرتجع مشتريات
        static List<ItemMovementdDto> PurchaseBackBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<PurchaseBackInvoicesDetail> purchaseBackBalanceIQ = null;
            purchaseBackBalanceIQ = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                purchaseBackBalanceIQ = purchaseBackBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                purchaseBackBalanceIQ = purchaseBackBalanceIQ.Where(x => DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.PurchaseBackInvoice.InvoiceDate) <= dtTo);
            return purchaseBackBalanceIQ
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "مرتجع مشتريات",
                    ActionNumber = x.PurchaseBackInvoice.InvoiceNumber,
                    ActionDate = x.PurchaseBackInvoice.InvoiceDate.ToString(),
                    OutcomingQuantity = x.Quantity,
                    OutcomingCost = x.Amount,
                }).ToList();
        }
        //بيع
        static List<ItemMovementdDto> SellBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<SellInvoicesDetail> sellBalanceIQ = null;
            sellBalanceIQ = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                sellBalanceIQ = sellBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                sellBalanceIQ = sellBalanceIQ.Where(x => DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellInvoice.InvoiceDate) <= dtTo);
            return sellBalanceIQ
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "بيع",
                    ActionNumber = x.SellInvoice.InvoiceNumber,
                    ActionDate = x.SellInvoice.InvoiceDate.ToString(),
                    OutcomingQuantity = x.Quantity,
                    OutcomingCost = x.Amount,
                }).ToList();
        }
        //مرتجع بيع
        static List<ItemMovementdDto> SellBackBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<SellBackInvoicesDetail> sellBackBalanceIQ = null;
            sellBackBalanceIQ = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                sellBackBalanceIQ = sellBackBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                sellBackBalanceIQ = sellBackBalanceIQ.Where(x => DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.SellBackInvoice.InvoiceDate) <= dtTo);
            return sellBackBalanceIQ
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "مرتجع بيع ",
                    ActionNumber = x.SellBackInvoice.InvoiceNumber,
                    ActionDate = x.SellBackInvoice.InvoiceDate.ToString(),
                    IncomingQuantity = x.Quantity,
                    IncomingCost = x.Amount,
                }).ToList();
        }
        //قطع غيار
        static List<ItemMovementdDto> MaintenanceSparePartBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<MaintenanceSparePart> maintenanceSparePartBalanceIQ = null;
            maintenanceSparePartBalanceIQ = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                maintenanceSparePartBalanceIQ = maintenanceSparePartBalanceIQ.Where(x => x.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                maintenanceSparePartBalanceIQ = maintenanceSparePartBalanceIQ.Where(x => DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.MaintenanceDetail.Maintenance.InvoiceDate) <= dtTo);
            return maintenanceSparePartBalanceIQ
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "قطع غيار",
                    ActionNumber = x.MaintenanceDetail != null ? x.MaintenanceDetail.Maintenance.InvoiceNumber : null,
                    ActionDate = x.MaintenanceDetail != null ? x.MaintenanceDetail.Maintenance.InvoiceDate.ToString() : null,
                    IncomingQuantity = x.Quantity,
                    IncomingCost = x.Amount,
                }).ToList();
        }
        //اذن استلام
        static List<ItemMovementdDto> StorePermissionBalancesMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo, bool isRecieve)
        {
            IQueryable<StorePermissionItem> storePermission = null;
            storePermission = db.StorePermissionItems.Where(x => !x.IsDeleted && x.ItemId == itemId && x.StorePermission.IsApproval && !x.StorePermission.IsDeleted && x.StorePermission.IsReceive == isRecieve);
            if (storeId != null)
                storePermission = storePermission.Where(x => x.StorePermission.StoreId == storeId);
            if (dtFrom != null && dtTo != null)
                storePermission = storePermission.Where(x => DbFunctions.TruncateTime(x.StorePermission.PermissionDate) >= dtFrom && DbFunctions.TruncateTime(x.StorePermission.PermissionDate) <= dtTo);
            return storePermission
                .Select(x => new ItemMovementdDto
                {
                    ActionType = x.StorePermission.IsReceive == true ? "اذن استلام" : "اذن صرف",
                    ActionDate = x.StorePermission != null ? x.StorePermission.PermissionDate.ToString() : null,
                    IncomingQuantity = x.StorePermission.IsReceive ? x.Quantity : 0,
                    IncomingCost = x.StorePermission.IsReceive ? x.Amount : 0,
                    OutcomingQuantity = x.StorePermission.IsReceive == false ? x.Quantity : 0,
                    OutcomingCost = x.StorePermission.IsReceive == false ? x.Amount : 0
                }).ToList();
        }

        static List<ItemMovementdDto> DamagesBalanceMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<DamageInvoiceDetail> damageBalance = null;
            damageBalance = db.DamageInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                damageBalance = damageBalance.Where(x => x.DamageInvoice.StoreId == storeId && !x.DamageInvoice.IsDeleted);
            if (dtFrom != null && dtTo != null)
                damageBalance = damageBalance.Where(x => DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.DamageInvoice.InvoiceDate) <= dtTo);
            return damageBalance
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "هالك",
                    ActionDate = x.DamageInvoice.InvoiceDate.ToString(),
                    OutcomingQuantity = x.Quantity,
                    OutcomingCost = x.CostQuantity,
                }).ToList();
        }
        static List<ItemMovementdDto> InventoryBalanceMovement(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<InventoryInvoiceDetail> inventoryBalance = null;
            inventoryBalance = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.ItemId == itemId);
            if (storeId != null)
                inventoryBalance = inventoryBalance.Where(x => x.InventoryInvoice.StoreId == storeId && !x.InventoryInvoice.IsDeleted);
            if (dtFrom != null && dtTo != null)
                inventoryBalance = inventoryBalance.Where(x => DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InventoryInvoice.InvoiceDate) <= dtTo);
            return inventoryBalance
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "فرق جرد",
                    ActionNumber = x.InventoryInvoice.InvoiceNumber,
                    ActionDate = x.InventoryInvoice.InvoiceDate.ToString(),
                    IncomingQuantity = x.DifferenceCount > 0 ? x.DifferenceCount : 0,
                    IncomingCost = x.DifferenceAmount > 0 ? x.DifferenceAmount : 0,
                    OutcomingQuantity = x.DifferenceCount < 0 ? x.DifferenceCount : 0,
                    OutcomingCost = x.DifferenceAmount < 0 ? x.DifferenceAmount : 0,
                }).ToList();
        }
        //1 مدخلات
        //2 مخرجات
        static List<ItemMovementdDto> ProductionOrderBalanceOutcoming(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<ProductionOrderDetail> productionOrderOuting = null;
            productionOrderOuting = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ItemId == itemId&&x.ProductionTypeId==(int)ProductionTypeCl.Out);
            if (storeId != null)
                productionOrderOuting = productionOrderOuting.Where(x => x.ProductionOrder.ProductionStoreId == storeId && !x.ProductionOrder.IsDeleted);
            if (dtFrom != null && dtTo != null)
                productionOrderOuting = productionOrderOuting.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom && DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);

            return productionOrderOuting
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "امر انتاج وارد",
                    ActionNumber = x.ProductionOrder.OrderNumber,
                    ActionDate = x.ProductionOrder.ProductionOrderDate.ToString(),
                    IncomingQuantity = x.Quantity + x.Quantitydamage ,
                    IncomingCost = x.ItemCost,
                }).ToList();
        }
        static List<ItemMovementdDto> ProductionOrderBalanceIncoming(VTSaleEntities db, Guid? itemId, Guid? storeId, DateTime? dtFrom, DateTime? dtTo)
        {
            IQueryable<ProductionOrderDetail> productionOrderIncoming = null;
            productionOrderIncoming = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ItemId == itemId && x.ProductionTypeId == (int)ProductionTypeCl.In);
            if (storeId != null)
                productionOrderIncoming = productionOrderIncoming.Where(x => x.ProductionOrder.ProductionUnderStoreId == storeId && !x.ProductionOrder.IsDeleted);
            if (dtFrom != null && dtTo != null)
                productionOrderIncoming = productionOrderIncoming.Where(x => DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) >= dtFrom && DbFunctions.TruncateTime(x.ProductionOrder.ProductionOrderDate) <= dtTo);
            var yy = productionOrderIncoming.ToList();
            return productionOrderIncoming
                .Select(x => new ItemMovementdDto
                {
                    ActionType = "امر انتاج صادر",
                    ActionNumber = x.ProductionOrder.OrderNumber,
                    ActionDate = x.ProductionOrder.ProductionOrderDate.ToString(),
                    OutcomingQuantity = x.Quantity + x.Quantitydamage,
                    OutcomingCost = x.ItemCost,
                }).ToList();
        }
        #endregion
    }
}