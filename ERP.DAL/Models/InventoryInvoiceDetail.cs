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

    public partial class InventoryInvoiceDetail : BaseModel
    {
        [ForeignKey(nameof(InventoryInvoice))]
        public Nullable<Guid> InventoryInvoiceId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        public double Balance { get; set; }
        public double BalanceReal { get; set; }
        public double DifferenceCount { get; set; }
        public double DifferenceAmount { get; set; }
    
        public virtual InventoryInvoice InventoryInvoice { get; set; }
        public virtual Item Item { get; set; }
    }
}
