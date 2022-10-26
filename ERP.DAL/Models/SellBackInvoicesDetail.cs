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

    public partial class SellBackInvoicesDetail : BaseModel
    {
        [ForeignKey(nameof(SellBackInvoice))]
        public Nullable<Guid> SellBackInvoiceId { get; set; }
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        [ForeignKey(nameof(ItemSerial))]
        public Nullable<Guid> ItemSerialId { get; set; }


        public double Quantity { get; set; }
        public double QuantityReal { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double ItemDiscount { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual ItemSerial ItemSerial { get; set; }
        public virtual SellBackInvoice SellBackInvoice { get; set; }
        public virtual Store Store { get; set; }
    }
}
