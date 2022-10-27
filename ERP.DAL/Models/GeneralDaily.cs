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

    public partial class GeneralDaily : BaseModel
    {
        public Nullable<Guid> TransactionId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        [ForeignKey(nameof(AccountsTree))]
        public Nullable<Guid> AccountsTreeId { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        [ForeignKey(nameof(TransactionsType))]
        public Nullable<int> TransactionTypeId { get; set; }
        public string PaperNumber { get; set; }
        public Nullable<Guid> POSId { get; set; }
        public Nullable<Guid> ShiftId { get; set; }
        public string Notes { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        public Nullable<System.Guid> TransactionShared { get; set; }
        public virtual AccountsTree AccountsTree { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual TransactionsType TransactionsType { get; set; }
    }
}