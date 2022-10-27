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

    public partial class ItemPrice : BaseModel
    {
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }

        [ForeignKey(nameof(PricingPolicy))]
        public Nullable<Guid> PricingPolicyId { get; set; }

        public Nullable<double> SellPrice { get; set; }

        [ForeignKey(nameof(Person))]
        public Nullable<Guid> CustomerId { get; set; }

        [ForeignKey(nameof(Unit))]
        public Nullable<Guid> UnitId { get; set; }

        public virtual Item Item { get; set; }
        public virtual PricingPolicy PricingPolicy { get; set; }
        public virtual Person Person { get; set; }
        public virtual Unit Unit { get; set; }
    }
}