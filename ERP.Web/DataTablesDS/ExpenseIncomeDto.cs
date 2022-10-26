using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ExpenseIncomeDto
    {

    }
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ExpenseTypeName { get; set; }
        public bool? IsApproval { get; set; }
        public string IsApprovalStatus { get; set; }
        public string SafeName { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string PaymentDate { get; set; }
        public string PaidTo { get; set; }
        public int? Actions { get; set; }
        public int? Num { get; set; }
    }   
    public class IncomeDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string IncomeTypeName { get; set; }
        public bool? IsApproval { get; set; }
        public string IsApprovalStatus { get; set; }
        public string SafeName { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string PaymentDate { get; set; }
        public string PaidTo { get; set; }
        public int? Actions { get; set; }
        public int? Num { get; set; }
    }
}