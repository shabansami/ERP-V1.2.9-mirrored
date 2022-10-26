using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class CustomerPaymentVm
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerName { get; set; }
        public bool IsApproval { get; set; }
        public string IsApprovalStatus { get; set; }
        public string InvoType { get; set; }
        public string SafeName { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
        public string PaymentDate { get; set; }
        public string BranchName { get; set; }
    }
}
