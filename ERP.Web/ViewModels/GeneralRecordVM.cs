using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class GeneralRecordVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> BranchId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        //قيد منفصل
        public Nullable<Guid> AccountTreeFromId { get; set; }
        public Nullable<Guid> AccountTreeToId { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }       
        
        //قيد مركب
        public Nullable<Guid> ComplexAccountTreeId { get; set; }
        public Nullable<long> ComplexAccountTreeNum { get; set; }
        public string ComplexAccountTreeName { get; set; }
        public double ComplexAmount { get; set; }
        public string ComplexNotes { get; set; }      
        public Nullable<int> ComplexDebitCredit { get; set; }
        public double? ComplexDebitName { get; set; }
        public double? ComplexCreditName { get; set; }
        public string Actions { get; set; }

    }
}