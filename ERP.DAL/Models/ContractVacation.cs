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

    public partial class ContractVacation : BaseModel
    {
        [ForeignKey(nameof(Contract))]
        public Nullable<Guid> ContractId { get; set; }
        [ForeignKey(nameof(VacationType))]
        public Nullable<Guid> VacationTypeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        [ForeignKey(nameof(Person))]
        public Nullable<Guid> ApprovalBy { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public string Reason { get; set; }
    
        public virtual Contract Contract { get; set; }
        public virtual Person Person { get; set; }
        public virtual VacationType VacationType { get; set; }
    }
}