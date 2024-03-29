//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.DAL.Models;

    public partial class Branch : BaseModel
    {
        public Branch()
        {
            this.Assets = new HashSet<Asset>();
            this.Cheques = new HashSet<Cheque>();
            this.ContractLoans = new HashSet<ContractLoan>();
            this.CustomerPayments = new HashSet<CustomerPayment>();
            this.EmployeeGivingCustodies = new HashSet<EmployeeGivingCustody>();
            this.EmployeeReturnCashCustodies = new HashSet<EmployeeReturnCashCustody>();
            this.ExpenseIncomes = new HashSet<ExpenseIncome>();
            this.GeneralDailies = new HashSet<GeneralDaily>();
            this.InventoryInvoices = new HashSet<InventoryInvoice>();
            this.Maintenances = new HashSet<Maintenance>();
            this.ProductionOrders = new HashSet<ProductionOrder>();
            this.PurchaseBackInvoices = new HashSet<PurchaseBackInvoice>();
            this.PurchaseInvoices = new HashSet<PurchaseInvoice>();
            this.Safes = new HashSet<Safe>();
            this.SellBackInvoices = new HashSet<SellBackInvoice>();
            this.SellInvoices = new HashSet<SellInvoice>();
            this.Stores = new HashSet<Store>();
            this.SupplierPayments = new HashSet<SupplierPayment>();
            this.Vouchers = new HashSet<Voucher>();
            this.ItemCustomSellPrices = new HashSet<ItemCustomSellPrice>();
            this.PersonIntialBalances = new HashSet<PersonIntialBalance>();
            this.PointOfSales = new HashSet<PointOfSale>();
            this.Offers = new HashSet<Offer>();
            this.QuoteOrderSells = new HashSet<QuoteOrderSell>();
            this.EmployeeBranches = new HashSet<EmployeeBranch>();
            this.EmployeeReturnCustodies = new HashSet<EmployeeReturnCustody>();
            this.AccountTreeIntialBalances = new HashSet<AccountTreeIntialBalance>();
            this.GeneralRecords = new HashSet<GeneralRecord>();
            this.ItemIntialBalances = new HashSet<ItemIntialBalance>();
        }

        public string Name { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }

        [ForeignKey(nameof(Area))]
        public Nullable<Guid> AreaId { get; set; }
        public string Address { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<Cheque> Cheques { get; set; }
        public virtual ICollection<ContractLoan> ContractLoans { get; set; }
        public virtual ICollection<CustomerPayment> CustomerPayments { get; set; }
        public virtual ICollection<EmployeeGivingCustody> EmployeeGivingCustodies { get; set; }
        public virtual ICollection<EmployeeReturnCashCustody> EmployeeReturnCashCustodies { get; set; }
        public virtual ICollection<ExpenseIncome> ExpenseIncomes { get; set; }
        public virtual ICollection<GeneralDaily> GeneralDailies { get; set; }
        public virtual ICollection<InventoryInvoice> InventoryInvoices { get; set; }
        public virtual ICollection<Maintenance> Maintenances { get; set; }
        public virtual ICollection<ProductionOrder> ProductionOrders { get; set; }
        public virtual ICollection<PurchaseBackInvoice> PurchaseBackInvoices { get; set; }
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        public virtual ICollection<Safe> Safes { get; set; }
        public virtual ICollection<SellBackInvoice> SellBackInvoices { get; set; }
        public virtual ICollection<SellInvoice> SellInvoices { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
        public virtual ICollection<ItemCustomSellPrice> ItemCustomSellPrices { get; set; }
        public virtual ICollection<PersonIntialBalance> PersonIntialBalances { get; set; }
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<QuoteOrderSell> QuoteOrderSells { get; set; }
        public virtual ICollection<EmployeeBranch> EmployeeBranches { get; set; }
        public virtual ICollection<EmployeeReturnCustody> EmployeeReturnCustodies { get; set; }
        public virtual ICollection<AccountTreeIntialBalance> AccountTreeIntialBalances { get; set; }
        public virtual ICollection<GeneralRecord> GeneralRecords { get; set; }
        public virtual ICollection<ItemIntialBalance> ItemIntialBalances { get; set; }
    }
}
