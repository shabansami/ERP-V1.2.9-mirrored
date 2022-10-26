using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class PurchaseInvoiceVM
    {
        public int Id { get; set; }
        public Guid InvoiceGuid { get; set; }
        public string InvoiceNum { get; set; }
        public string InvoiceNumPaper { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<int> SafeId { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<double> TotalQuantity { get; set; }
        public Nullable<double> TotalValue { get; set; }
        public Nullable<double> PayedValue { get; set; }
        public Nullable<double> RemindValue { get; set; }
        public Nullable<double> Tax { get; set; }
        public Nullable<double> TotalDiscount { get; set; }
        public Nullable<bool> IsApproval { get; set; }
        public string ShippingAddress { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public string Notes { get; set; }
        public int IsClosed { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<double> InvoiceDiscount { get; set; }
    }
}