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

    public partial class ContractSalaryPenalty : BaseModel
    {
        [ForeignKey(nameof(Contract))]
        public Nullable<Guid> ContractId { get; set; }
        [ForeignKey(nameof(ContractScheduling))]
        public Nullable<Guid> ContractSchedulingId { get; set; }
        [ForeignKey(nameof(SalaryPenalty))]
        public Nullable<Guid> SalaryPenaltyId { get; set; }
        public Nullable<System.DateTime> PenaltyDate { get; set; }
        public string EmployeeNotes { get; set; }
        public double Amount { get; set; }
        public double AmountPayed { get; set; }
        public bool IsEveryMonth { get; set; }
        public bool IsPayed { get; set; }
        public string Notes { get; set; }
    
        public virtual Contract Contract { get; set; }
        public virtual ContractScheduling ContractScheduling { get; set; }
        public virtual SalaryPenalty SalaryPenalty { get; set; }
    }
}
