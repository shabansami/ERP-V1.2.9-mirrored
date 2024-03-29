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

    public partial class ExpenseIncome : BaseModel
    {
        //ExpenseIncomeTypeId
        [ForeignKey(nameof(ExpenseIncomeTypeAccountsTree)), ReNamed("ExpenseIncomeTypeId")]
        public Nullable<Guid> ExpenseIncomeTypeAccountTreeId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(Safe))]
        public Nullable<Guid> SafeId { get; set; }
        [ForeignKey(nameof(ShiftsOffline))]
        public Nullable<Guid> ShiftOffLineID { get; set; }

        //public System.Guid RowGuid { get; set; }
        public bool IsExpense { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public bool IsApproval { get; set; }
        public string PaidTo { get; set; } //��� ����� ������� ������ �� ������� �� ������ ������� 

        public string OperationNumber { get; set; }
        //AccountsTree
        public virtual AccountsTree ExpenseIncomeTypeAccountsTree { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Safe Safe { get; set; }
        public virtual ShiftsOffline ShiftsOffline { get; set; }
    }
}
