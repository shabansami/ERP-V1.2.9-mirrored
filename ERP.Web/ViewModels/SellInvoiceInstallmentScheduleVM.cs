using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SellInvoiceInstallmentScheduleVM
    {
        public Guid Id { get; set; }
        public Guid ScheduleGuid { get; set; }
        public Guid? SafeId { get; set; }
        public string CustomerName { get; set; }
        public Guid? InstallmentId { get; set; }
        public double Amount { get; set; }
        public double PayedAmount { get; set; }
        public DateTime? InstallmentDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public Guid? OtherScheduleNotPaidId { get; set; }
        public double DifValue { get; set; } //الفرق بين قيمة القسط وما تم دفعة 
    }
}