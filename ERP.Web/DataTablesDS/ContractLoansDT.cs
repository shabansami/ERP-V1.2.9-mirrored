using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ContractLoansDT
    {
        public Guid Id { get; set; }
        public Guid? ContractSchedulingId { get; set; }
        public string LoanDate { get; set; }
        public double Amount { get; set; }        
        public double TotalAmount { get; set; }
        public string LoanNotes { get; set; }
    }
}