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
        public string Notes { get; set; }

        public Nullable<Guid> SelectedAccountTreeId { get; set; }
        public double InsertedAmount { get; set; }
        public string InsertedNotes { get; set; }       

    }
    public class GeneralRecordDetailDT
    {
        //public Guid GeneralRecordDetailId { get; set; }
        //public Guid GeneralRecordId { get; set; }
        public Nullable<Guid> AccountTreeId { get; set; }
        public Nullable<long> AccountTreeNum { get; set; }
        public string AccountTreeName { get; set; }
        public Nullable<int> DebitCredit { get; set; }
        public double DebitAmount { get; set; }
        public double CreditAmount { get; set; }
        public string Notes { get; set; }
        public string Actions { get; set; }

    }
}