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

    public partial class Item : BaseModel
    {
        public Item()
        {
            this.InventoryInvoiceDetails = new HashSet<InventoryInvoiceDetail>();
            this.ItemIntialBalanceDetails = new HashSet<ItemIntialBalanceDetail>();
            this.ItemPrices = new HashSet<ItemPrice>();
            this.ItemProductionDetails = new HashSet<ItemProductionDetail>();
            this.ItemSerials = new HashSet<ItemSerial>();
            this.ItemUnits = new HashSet<ItemUnit>();
            this.MaintenanceDamageDetails = new HashSet<MaintenanceDamageDetail>();
            this.MaintenanceDetails = new HashSet<MaintenanceDetail>();
            this.MaintenanceSpareParts = new HashSet<MaintenanceSparePart>();
            this.PriceInvoicesDetails = new HashSet<PriceInvoicesDetail>();
            this.PricesChanges = new HashSet<PricesChanx>();
            this.ProductionOrderDetails = new HashSet<ProductionOrderDetail>();
            this.PurchaseBackInvoicesDetails = new HashSet<PurchaseBackInvoicesDetail>();
            this.PurchaseInvoicesDetails = new HashSet<PurchaseInvoicesDetail>();
            this.SellBackInvoicesDetails = new HashSet<SellBackInvoicesDetail>();
            this.SellInvoicesDetails = new HashSet<SellInvoicesDetail>();
            this.StoreAdjustments = new HashSet<StoreAdjustment>();
            this.StoresTransferDetails = new HashSet<StoresTransferDetail>();
            this.DamageInvoiceDetails = new HashSet<DamageInvoiceDetail>();
            this.ItemCustomSellPrices = new HashSet<ItemCustomSellPrice>();
            this.OfferDetails = new HashSet<OfferDetail>();
            this.QuoteOrderSellDetails = new HashSet<QuoteOrderSellDetail>();
        }

        [ForeignKey(nameof(GroupBasic))]
        public Nullable<Guid> GroupBasicId { get; set; }

        [ForeignKey(nameof(GroupSell))]
        public Nullable<Guid> GroupSellId { get; set; }

        [ForeignKey(nameof(ItemType))]
        public Nullable<Guid> ItemTypeId { get; set; }
        public string Name { get; set; }
        public double SellPrice { get; set; }
        public Nullable<double> MaxPrice { get; set; }
        public Nullable<double> MinPrice { get; set; }
        public string ItemCode { get; set; }
        public Nullable<double> RequestLimit1 { get; set; }
        public Nullable<double> RequestLimit2 { get; set; }
        public string BarCode { get; set; }
        public string TechnicalSpecifications { get; set; }
        [ForeignKey(nameof(Unit))]
        public Nullable<Guid> UnitId { get; set; }
        [ForeignKey(nameof(UnitConvertFrom))]
        public Nullable<Guid> UnitConvertFromId { get; set; }
        public double UnitConvertFromCount { get; set; }
        public bool AvaliableToSell { get; set; }
        public bool CreateSerial { get; set; }
        public string ImageName { get; set; }

        //������� ����� �-���� ����
        public string Thickness { get; set; }
        public string ItemSize { get; set; }

        //Group
        public virtual Group GroupBasic { get; set; }
        //Group1
        public virtual Group GroupSell { get; set; }
        public virtual ItemType ItemType { get; set; }
        public virtual Unit Unit { get; set; }
        //unit1
        public virtual Unit UnitConvertFrom { get; set; }


        public virtual ICollection<InventoryInvoiceDetail> InventoryInvoiceDetails { get; set; }
        public virtual ICollection<ItemIntialBalanceDetail> ItemIntialBalanceDetails { get; set; }
        public virtual ICollection<ItemPrice> ItemPrices { get; set; }
        public virtual ICollection<ItemProductionDetail> ItemProductionDetails { get; set; }
        public virtual ICollection<ItemSerial> ItemSerials { get; set; }
        public virtual ICollection<ItemUnit> ItemUnits { get; set; }
        public virtual ICollection<MaintenanceDamageDetail> MaintenanceDamageDetails { get; set; }
        public virtual ICollection<MaintenanceDetail> MaintenanceDetails { get; set; }
        public virtual ICollection<MaintenanceSparePart> MaintenanceSpareParts { get; set; }
        public virtual ICollection<PriceInvoicesDetail> PriceInvoicesDetails { get; set; }
        public virtual ICollection<PricesChanx> PricesChanges { get; set; }
        public virtual ICollection<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual ICollection<PurchaseBackInvoicesDetail> PurchaseBackInvoicesDetails { get; set; }
        public virtual ICollection<PurchaseInvoicesDetail> PurchaseInvoicesDetails { get; set; }
        public virtual ICollection<SellBackInvoicesDetail> SellBackInvoicesDetails { get; set; }
        public virtual ICollection<SellInvoicesDetail> SellInvoicesDetails { get; set; }
        public virtual ICollection<StoreAdjustment> StoreAdjustments { get; set; }
        public virtual ICollection<StoresTransferDetail> StoresTransferDetails { get; set; }
        public virtual ICollection<DamageInvoiceDetail> DamageInvoiceDetails { get; set; }
        public virtual ICollection<ItemCustomSellPrice> ItemCustomSellPrices { get; set; }
        public virtual ICollection<OfferDetail> OfferDetails { get; set; }
        public virtual ICollection<QuoteOrderSellDetail> QuoteOrderSellDetails { get; set; }



    }
}
