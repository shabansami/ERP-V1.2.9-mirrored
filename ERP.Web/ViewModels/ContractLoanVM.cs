using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ContractLoanVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> ContractId { get; set; }
        public Nullable<Guid> BankAccountId { get; set; }
        public Nullable<Guid> SafeId { get; set; }
        public Nullable<Guid> ContractSchedulingId { get; set; }
        public double Amount { get; set; }
        public int NumberMonths { get; set; }
        public string Notes { get; set; }
        public bool IsApproval { get; set; }
        public double AmountMonth { get; set; }
        public Nullable<System.DateTime> LoanDate { get; set; }
    }
}