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

    public partial class ProductionOrderReceipt : BaseModel
    {
        [ForeignKey(nameof(ProductionOrder))]
        public Nullable<Guid> ProductionOrderId { get; set; }

        [ForeignKey(nameof(FinalItemStore))]
        public Nullable<Guid> FinalItemStoreId { get; set; }


        public double ReceiptQuantity { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public string Notes { get; set; }
    

        public virtual ProductionOrder ProductionOrder { get; set; }

        //Store
        public virtual Store FinalItemStore { get; set; }
    }
}