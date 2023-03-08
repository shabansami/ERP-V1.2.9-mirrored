using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SaleMenSellInvoiceVM
    {
        public Guid Id { get; set; }
        public System.Guid InvoiceGuid { get; set; }
        public bool BySaleMen { get; set; }
        public Nullable<Guid> SaleMenEmployeeId { get; set; }
        //public Nullable<Guid> SaleMenStoreId { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<Guid> PersonCategoryId { get; set; }
        public Nullable<Guid> CustomerId { get; set; }
        public Nullable<Guid> BranchId { get; set; }
        public Nullable<Guid> SafeId { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalValue { get; set; }
        public double PayedValue { get; set; }
        public double RemindValue { get; set; }
        public double SalesTax { get; set; }
        public double SalesTaxPercentage { get; set; } //نسبة الضريبة المدخلة
        public double ProfitTax { get; set; }
        public double ProfitTaxPercentage { get; set; } //نسبة ضريبة ارباح تجارية المدخلة
        public double InvoiceDiscount { get; set; }
        public double DiscountPercentage { get; set; }
        public double TotalItemDiscount { get; set; }
        public double TotalDiscount { get; set; }
        public double Safy { get; set; }
        public Nullable<int> CaseId { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Notes { get; set; }

        public Nullable<Guid> SellInvoiceId { get; set; }//in sell back invoice

    }
}