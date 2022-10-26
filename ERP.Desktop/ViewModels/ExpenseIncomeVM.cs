using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class ExpenseIncomeVM
    {
        public Guid Id { get; set; }
        public Guid RowGuid { get; set; }
        public string ExpenseTypeName { get; set; }
        public bool IsApproval { get; set; }
        public string IsApprovalStatus { get; set; }
        public string SafeName { get; set; }
        public string BranchName { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string UserName { get; set; }
        public string PaymentDate { get; set; }
        public string IncomeTypeName { get; set; }
        //تستخدم مع سلف الموظف
        public string EmployeeName { get; set; }
        public string ContractSchedulingName { get; set; }

    }
}
