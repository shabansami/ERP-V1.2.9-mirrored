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
    
    public partial class MaintenanceDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MaintenanceDetail()
        {
            this.MaintenanceDamages = new HashSet<MaintenanceDamage>();
            this.MaintenanceIncomes = new HashSet<MaintenanceIncome>();
            this.MaintenanceSpareParts = new HashSet<MaintenanceSparePart>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid RecordGuid { get; set; }
        public Nullable<System.Guid> MaintenanceId { get; set; }
        public Nullable<System.Guid> ItemId { get; set; }
        public Nullable<System.Guid> ItemSerialId { get; set; }
        public Nullable<System.Guid> MaintenProblemTypeId { get; set; }
        public Nullable<int> MaintenanceCaseId { get; set; }
        public double TotalItemDiscount { get; set; }
        public double TotalItemIncomes { get; set; }
        public double TotalItemSpareParts { get; set; }
        public double ItemSafy { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
    
        public virtual Item Item { get; set; }
        public virtual ItemSerial ItemSerial { get; set; }
        public virtual MaintenanceCa MaintenanceCa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaintenanceDamage> MaintenanceDamages { get; set; }
        public virtual Maintenance Maintenance { get; set; }
        public virtual MaintenProblemType MaintenProblemType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaintenanceIncome> MaintenanceIncomes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaintenanceSparePart> MaintenanceSpareParts { get; set; }
    }
}
