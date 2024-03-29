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
    
    public partial class PointOfSale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PointOfSale()
        {
            this.ShiftsOfflines = new HashSet<ShiftsOffline>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> SafeID { get; set; }
        public Nullable<System.Guid> BankAccountID { get; set; }
        public Nullable<System.Guid> DefaultPricePolicyID { get; set; }
        public Nullable<System.Guid> DefaultCustomerID { get; set; }
        public Nullable<System.Guid> DefaultSupplierID { get; set; }
        public Nullable<int> DefaultPaymentID { get; set; }
        public bool CanDiscount { get; set; }
        public bool CanChangePrice { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
        public Nullable<System.Guid> StoreID { get; set; }
        public Nullable<System.Guid> BrunchId { get; set; }
        public Nullable<System.Guid> DefaultBankWalletId { get; set; }
        public Nullable<System.Guid> DefaultBankCardId { get; set; }
    
        public virtual BankAccount BankAccount { get; set; }
        public virtual BankAccount BankAccount1 { get; set; }
        public virtual BankAccount BankAccount2 { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual Person Person { get; set; }
        public virtual Person Person1 { get; set; }
        public virtual PricingPolicy PricingPolicy { get; set; }
        public virtual Safe Safe { get; set; }
        public virtual Store Store { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShiftsOffline> ShiftsOfflines { get; set; }
    }
}
