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

    public partial class Store : BaseModel
    {
        public Store()
        {
            this.Employees = new HashSet<Employee>();
            this.InventoryInvoices = new HashSet<InventoryInvoice>();
            this.ItemIntialBalances = new HashSet<ItemIntialBalance>();
            this.ItemSerials = new HashSet<ItemSerial>();
            this.MaintenanceDamages = new HashSet<MaintenanceDamage>();
            this.MaintenancesStores = new HashSet<Maintenance>();
            this.MaintenancesStoreReceipts = new HashSet<Maintenance>();
            this.MaintenanceSpareParts = new HashSet<MaintenanceSparePart>();
            //this.ProductionOrderDetails = new HashSet<ProductionOrderDetail>();
            this.ProductionOrderReceipts = new HashSet<ProductionOrderReceipt>();
            this.ProductionOrders = new HashSet<ProductionOrder>();
            this.PurchaseBackInvoicesDetails = new HashSet<PurchaseBackInvoicesDetail>();
            this.PurchaseInvoicesDetails = new HashSet<PurchaseInvoicesDetail>();
            this.SaleMenStoreHistories = new HashSet<SaleMenStoreHistory>();
            this.SellBackInvoicesDetails = new HashSet<SellBackInvoicesDetail>();
            this.SellInvoicesDetails = new HashSet<SellInvoicesDetail>();
            this.StoreAdjustments = new HashSet<StoreAdjustment>();
            this.StoresTransfersFrom = new HashSet<StoresTransfer>();
            this.StoresTransfersTo = new HashSet<StoresTransfer>();
            this.DamageInvoices = new HashSet<DamageInvoice>();
        }

        [ForeignKey(nameof(AccountsTree))]
        public Nullable<Guid> AccountTreeId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }
        public bool IsDamages { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public virtual AccountsTree AccountsTree { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Employee Employee { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<InventoryInvoice> InventoryInvoices { get; set; }
        public virtual ICollection<ItemIntialBalance> ItemIntialBalances { get; set; }
        public virtual ICollection<ItemSerial> ItemSerials { get; set; }
        public virtual ICollection<MaintenanceDamage> MaintenanceDamages { get; set; }

        //Maintenances
        [InverseProperty(nameof(Maintenance.Store))]
        public virtual ICollection<Maintenance> MaintenancesStores { get; set; }

        //Maintenances1
        [InverseProperty(nameof(Maintenance.StoreReceipt))]
        public virtual ICollection<Maintenance> MaintenancesStoreReceipts { get; set; }

        public virtual ICollection<MaintenanceSparePart> MaintenanceSpareParts { get; set; }
        //public virtual ICollection<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual ICollection<ProductionOrderReceipt> ProductionOrderReceipts { get; set; }
        public virtual ICollection<ProductionOrder> ProductionOrders { get; set; }
        public virtual ICollection<PurchaseBackInvoicesDetail> PurchaseBackInvoicesDetails { get; set; }
        public virtual ICollection<PurchaseInvoicesDetail> PurchaseInvoicesDetails { get; set; }
        public virtual ICollection<SaleMenStoreHistory> SaleMenStoreHistories { get; set; }
        public virtual ICollection<SellBackInvoicesDetail> SellBackInvoicesDetails { get; set; }
        public virtual ICollection<SellInvoicesDetail> SellInvoicesDetails { get; set; }
        public virtual ICollection<StoreAdjustment> StoreAdjustments { get; set; }
        [InverseProperty(nameof(StoresTransfer.StoreFrom))]
        public virtual ICollection<StoresTransfer> StoresTransfersFrom { get; set; }
        [InverseProperty(nameof(StoresTransfer.StoreTo))]
        public virtual ICollection<StoresTransfer> StoresTransfersTo { get; set; }
        public virtual ICollection<PointOfSale> PointOfSales { get; set; }
        public virtual ICollection<DamageInvoice> DamageInvoices { get; set; }
    }
}
