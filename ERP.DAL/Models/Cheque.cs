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

    public partial class Cheque : BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }
        public bool BySaleMen { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(PersonSupplier))]
        public Nullable<Guid> SupplierId { get; set; }
        [ForeignKey(nameof(PersonCustomer))]
        public Nullable<Guid> CustomerId { get; set; }
        [ForeignKey(nameof(BankAccount))]
        public Nullable<Guid> BankAccountId { get; set; }
        public Nullable<Guid> InvoiceId { get; set; }
        public string CheckNumber { get; set; }
        public Nullable<System.DateTime> CheckDate { get; set; }
        public Nullable<System.DateTime> CheckDueDate { get; set; }
        public Nullable<System.DateTime> CollectionDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public bool IsCollected { get; set; }
        public bool IsRefused { get; set; }
        public Nullable<System.DateTime> RefuseDate { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Branch Branch { get; set; }
        //Person
        public virtual Person PersonCustomer { get; set; }
        //Person1
        public virtual Person PersonSupplier { get; set; }
        public virtual BankAccount BankAccount { get; set; }
    }
}
