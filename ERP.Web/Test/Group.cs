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
    
    public partial class Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Group()
        {
            this.Groups1 = new HashSet<Group>();
            this.ItemCustomSellPrices = new HashSet<ItemCustomSellPrice>();
            this.Items = new HashSet<Item>();
            this.Items1 = new HashSet<Item>();
        }
    
        public System.Guid Id { get; set; }
        public Nullable<int> GroupTypeId { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group> Groups1 { get; set; }
        public virtual Group Group1 { get; set; }
        public virtual GroupType GroupType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ItemCustomSellPrice> ItemCustomSellPrices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items1 { get; set; }
    }
}
