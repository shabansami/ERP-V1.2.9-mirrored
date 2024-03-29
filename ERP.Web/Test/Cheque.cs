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
    
    public partial class Cheque
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> EmployeeId { get; set; }
        public bool BySaleMen { get; set; }
        public Nullable<System.Guid> BranchId { get; set; }
        public Nullable<System.Guid> SupplierId { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Nullable<System.Guid> BankAccountId { get; set; }
        public Nullable<System.Guid> InvoiceId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<System.DateTime> CheckDueDate { get; set; }
        public Nullable<System.DateTime> CollectionDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public bool IsCollected { get; set; }
        public bool IsRefused { get; set; }
        public Nullable<System.DateTime> RefuseDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
    
        public virtual BankAccount BankAccount { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Person Person { get; set; }
        public virtual Person Person1 { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
