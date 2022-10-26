using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ERP.Web.Services
{
    public class SupplierService
    {
        #region تقارير فواتير الموردين

        public SupplierAccountVM GetSupplierAccount(Guid? supplierId, DateTime? dtFrom, DateTime? dtTo, bool allTimes, bool isFinalApproval)
        {
            using (var db = new VTSaleEntities())
            {
                SupplierAccountVM supplierAccount = new SupplierAccountVM();
                //فواتير التوريد 
                var purchase = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.SupplierId == supplierId);
                if (isFinalApproval)
                    purchase = purchase.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    purchase = purchase.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                supplierAccount.PurchaseInvoices = purchase.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonSupplier.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                supplierAccount.TotalPurchaseInvoices = supplierAccount.PurchaseInvoices.Sum(x => x.Amount);
                //فواتير مرتجع التوريد 
                var purchaseBack = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.SupplierId == supplierId);
                if (isFinalApproval)
                    purchaseBack = purchaseBack.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    purchaseBack = purchaseBack.Where(x =>DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom &&DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                supplierAccount.PurchaseBackInvoices = purchaseBack.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonSupplier.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                supplierAccount.TotalPurchaseBackInvoices = supplierAccount.PurchaseBackInvoices.Sum(x => x.Amount);
                // التحصيل النقدى 
                var supplierPayment = db.SupplierPayments.Where(x => !x.IsDeleted && x.SupplierId == supplierId&&x.SafeId!=null);
                if (isFinalApproval)
                    supplierPayment = supplierPayment.Where(x => x.IsApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    supplierPayment = supplierPayment.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo);
                supplierAccount.SupplierPayments = supplierPayment.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.PaymentDate,
                    CustomerName = x.PersonSupplier.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                var totalVal = supplierAccount.SupplierPayments.Sum(x => x.Amount);
                supplierAccount.TotalSupplierPayments = totalVal;
                // الشيكات 
                var cheques = db.SupplierPayments.Where(x => !x.IsDeleted && x.SupplierId == supplierId && x.BankAccountId != null);
                if (isFinalApproval)
                    cheques = cheques.Where(x => x.IsApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    cheques = cheques.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo);
                supplierAccount.Cheques = cheques.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.PaymentDate,
                    CustomerName = x.PersonSupplier.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                var totalCheques = supplierAccount.Cheques.Sum(x => x.Amount);
                supplierAccount.TotalCheques = totalCheques;
                return supplierAccount;
            }

        }

        #endregion

    }
}