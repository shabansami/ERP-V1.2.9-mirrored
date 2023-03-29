//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.Web.Test
{
    using System;
    using System.Collections.Generic;
    
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.Cheques = new HashSet<Cheque>();
            this.Cheques1 = new HashSet<Cheque>();
            this.ContractCustomerSuppliers = new HashSet<ContractCustomerSupplier>();
            this.ContractCustomerSuppliers1 = new HashSet<ContractCustomerSupplier>();
            this.ContractCustomerSuppliers2 = new HashSet<ContractCustomerSupplier>();
            this.ContractCustomerSuppliers3 = new HashSet<ContractCustomerSupplier>();
            this.ContractVacations = new HashSet<ContractVacation>();
            this.CustomerPayments = new HashSet<CustomerPayment>();
            this.Employees = new HashSet<Employee>();
            this.InstallmentSchedules = new HashSet<InstallmentSchedule>();
            this.ItemPrices = new HashSet<ItemPrice>();
            this.ItemPrices1 = new HashSet<ItemPrice>();
            this.ItemPrices2 = new HashSet<ItemPrice>();
            this.ItemProductions = new HashSet<ItemProduction>();
            this.Maintenances = new HashSet<Maintenance>();
            this.People1 = new HashSet<Person>();
            this.PersonIntialBalances = new HashSet<PersonIntialBalance>();
            this.PointOfSales = new HashSet<PointOfSale>();
            this.PointOfSales1 = new HashSet<PointOfSale>();
            this.PriceInvoices = new HashSet<PriceInvoice>();
            this.PurchaseBackInvoices = new HashSet<PurchaseBackInvoice>();
            this.PurchaseInvoices = new HashSet<PurchaseInvoice>();
            this.QuoteOrderSells = new HashSet<QuoteOrderSell>();
            this.Safes = new HashSet<Safe>();
            this.SaleMenAreas = new HashSet<SaleMenArea>();
            this.SaleMenCustomers = new HashSet<SaleMenCustomer>();
            this.SellBackInvoices = new HashSet<SellBackInvoice>();
            this.SellInvoices = new HashSet<SellInvoice>();
            this.StoreAdjustments = new HashSet<StoreAdjustment>();
            this.StorePermissions = new HashSet<StorePermission>();
            this.SupplierPayments = new HashSet<SupplierPayment>();
            this.Users = new HashSet<User>();
            this.Users1 = new HashSet<User>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> AccountsTreeCustomerId { get; set; }
        public Nullable<System.Guid> AccountTreeSupplierId { get; set; }
        public Nullable<System.Guid> AccountTreeEmpCustodyId { get; set; }
        public Nullable<int> PersonTypeId { get; set; }
        public Nullable<System.Guid> PersonCategoryId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Mob1 { get; set; }
        public string Mob2 { get; set; }
        public string Tel { get; set; }
        public Nullable<System.Guid> AreaId { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string EntityName { get; set; }
        public Nullable<int> GenderId { get; set; }
        public string ImageName { get; set; }
        public string Notes { get; set; }
        public string LocationPath { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
        public string Address2 { get; set; }
        public string TaxNumber { get; set; }
        public string CommercialRegistrationNo { get; set; }
        public string WebSite { get; set; }
        public string Job { get; set; }
        public string Email { get; set; }
        public string Transfer { get; set; }
        public string NameEn { get; set; }
        public Nullable<int> NationalityId { get; set; }
        public double LimitDangerSell { get; set; }
        public Nullable<System.Guid> DistrictId { get; set; }
        public bool IsActive { get; set; }
    
        public virtual AccountsTree AccountsTree { get; set; }
        public virtual AccountsTree AccountsTree1 { get; set; }
        public virtual AccountsTree AccountsTree2 { get; set; }
        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cheque> Cheques { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cheque> Cheques1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractCustomerSupplier> ContractCustomerSuppliers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractCustomerSupplier> ContractCustomerSuppliers1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractCustomerSupplier> ContractCustomerSuppliers2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractCustomerSupplier> ContractCustomerSuppliers3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContractVacation> ContractVacations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerPayment> CustomerPayments { get; set; }
        public virtual District District { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual Gender Gender { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstallmentSchedule> InstallmentSchedules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemPrice> ItemPrices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemPrice> ItemPrices1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemPrice> ItemPrices2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemProduction> ItemProductions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance> Maintenances { get; set; }
        public virtual Nationality Nationality { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> People1 { get; set; }
        public virtual Person Person1 { get; set; }
        public virtual PersonCategory PersonCategory { get; set; }
        public virtual PersonType PersonType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonIntialBalance> PersonIntialBalances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PointOfSale> PointOfSales1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceInvoice> PriceInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseBackInvoice> PurchaseBackInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuoteOrderSell> QuoteOrderSells { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Safe> Safes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleMenArea> SaleMenAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleMenCustomer> SaleMenCustomers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellBackInvoice> SellBackInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellInvoice> SellInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreAdjustment> StoreAdjustments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StorePermission> StorePermissions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SupplierPayment> SupplierPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users1 { get; set; }
    }
}
