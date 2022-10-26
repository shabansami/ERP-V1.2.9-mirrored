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
    using ERP.DAL.Utilites;

    public partial class EmployeeReturnCustody : BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }

        //ExpenseTypeId
        [ForeignKey(nameof(ExpenseTypeAccountTree)), ReNamed("ExpenseTypeId")]
        public Nullable<Guid> ExpenseTypeAccountTreeId { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public bool IsApproval { get; set; }

        //AccountTree
        public virtual AccountsTree ExpenseTypeAccountTree { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
