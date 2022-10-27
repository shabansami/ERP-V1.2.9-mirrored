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
    using ERP.DAL.Models;

    public partial class ItemCostCalculation : BaseModelInt
    {
        public ItemCostCalculation()
        {
            this.InventoryInvoices = new HashSet<InventoryInvoice>();
            this.ProductionOrderDetails = new HashSet<ProductionOrderDetail>();
            this.DamageInvoices = new HashSet<DamageInvoice>();
        }

        public string Name { get; set; }
    
        public virtual ICollection<InventoryInvoice> InventoryInvoices { get; set; }
        public virtual ICollection<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual ICollection<DamageInvoice> DamageInvoices { get; set; }
    }
}