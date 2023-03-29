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
    
    public partial class PurchaseBackInvoicesDetail
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ContainerId { get; set; }
        public Nullable<System.Guid> ItemId { get; set; }
        public Nullable<System.Guid> PurchaseBackInvoiceId { get; set; }
        public Nullable<System.Guid> StoreId { get; set; }
        public bool IsItemApprovalStore { get; set; }
        public double Quantity { get; set; }
        public double QuantityReal { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double ItemDiscount { get; set; }
        public Nullable<System.DateTime> ItemEntryDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
    
        public virtual Container Container { get; set; }
        public virtual Item Item { get; set; }
        public virtual PurchaseBackInvoice PurchaseBackInvoice { get; set; }
        public virtual Store Store { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
