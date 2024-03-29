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

    public partial class StoresTransferDetail : BaseModel
    {
        [ForeignKey(nameof(StoresTransfer))]
        public Nullable<Guid> StoresTransferId { get; set; }
        [ForeignKey(nameof(ProductionOrder))]
        public Nullable<Guid> ProductionOrderId { get; set; }
        [ForeignKey(nameof(ItemSerial))]
        public Nullable<Guid> ItemSerialId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }


        public double Quantity { get; set; }
        public double QuantityReal { get; set; }
        public Nullable<bool> IsItemIntial { get; set; }

        public virtual Item Item { get; set; }
        public virtual ItemSerial ItemSerial { get; set; }
        public virtual ProductionOrder ProductionOrder { get; set; }
        public virtual StoresTransfer StoresTransfer { get; set; }
    }
}
