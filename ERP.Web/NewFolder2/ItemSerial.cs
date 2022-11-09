//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.Web.NewFolder2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemSerial
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ItemSerial()
        {
            this.MaintenanceDetails = new HashSet<MaintenanceDetail>();
            this.MaintenanceSpareParts = new HashSet<MaintenanceSparePart>();
            this.SellBackInvoicesDetails = new HashSet<SellBackInvoicesDetail>();
            this.SellInvoicesDetails = new HashSet<SellInvoicesDetail>();
            this.StoresTransferDetails = new HashSet<StoresTransferDetail>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> SerialCaseId { get; set; }
        public Nullable<int> ProductionOrderId { get; set; }
        public Nullable<bool> IsItemIntial { get; set; }
        public Nullable<int> MaintenanceId { get; set; }
        public Nullable<int> SellInvoiceId { get; set; }
        public Nullable<int> CurrentStoreId { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual Maintenance Maintenance { get; set; }
        public virtual ProductionOrder ProductionOrder { get; set; }
        public virtual SellInvoice SellInvoice { get; set; }
        public virtual SerialCas SerialCas { get; set; }
        public virtual Store Store { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaintenanceDetail> MaintenanceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaintenanceSparePart> MaintenanceSpareParts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellBackInvoicesDetail> SellBackInvoicesDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SellInvoicesDetail> SellInvoicesDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoresTransferDetail> StoresTransferDetails { get; set; }
    }
}
