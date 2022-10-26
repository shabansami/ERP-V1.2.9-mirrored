using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class GeneralDayDto
    {
        //public double LastDebit { get; set; }// الرصيد السابق قبل فترة مدين 
        //public double LastCredit { get; set; }// الرصيد السابق قبل فترة دائن 
        public Guid Id { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid? AccountsTreeId { get; set; }
        public int? TransactionTypeId { get; set; }
        public string TransactionDate { get; set; }
        public string AccountsTreeName { get; set; }
        public string AccountNumber { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double Balance { get; set; }
        public string TransactionTypeName { get; set; }
        public string Notes { get; set; }
        public int? Num { get; set; }

        //فى حالة عميل تابع يتم اعطاء خلفية مختلفة فى الجريد 
        public bool IsCustRelated { get; set; }
        //public string BackGroundColor { get; set; }
    }
}