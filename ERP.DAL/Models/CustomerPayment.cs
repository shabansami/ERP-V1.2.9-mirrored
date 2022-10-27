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

    public partial class CustomerPayment : BaseModel
    {
        [ForeignKey(nameof(PersonCustomer))]
        public Nullable<Guid> CustomerId { get; set; }
        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }
        public bool BySaleMen { get; set; }
        [ForeignKey(nameof(SellInvoice))]
        public Nullable<Guid> SellInvoiceId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(Safe))]
        public Nullable<Guid> SafeId { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public bool IsApproval { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Person PersonCustomer { get; set; }
        public virtual Safe Safe { get; set; }
        public virtual SellInvoice SellInvoice { get; set; }
    }
}