using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class SellInvoiceInstallmentDto
    {
        public Guid InstallmentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public string InvoiceNum { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public bool AnySchedules { get; set; }
        public int? PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public double Safy { get; set; }
        public double PayedValue { get; set; }
        public double RemindValue { get; set; }
        public bool IsSell { get; set; }//فاتورة بيع/رصيد اول لعميل
        public string StatusTxt { get; set; } //فاتورة بيع/رصيد اول لعميل
        public int? Actions { get; set; }
        public int? Num { get; set; }

    }

    public class SellInvoiceInstallmentScheduleDto
    {
        public Guid? InvoiceId { get; set; }
        public Guid? ScheduleId { get; set; }
        public string InvoiceNum { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string StatusTxt { get; set; } //فاتورة بيع/رصيد اول لعميل
        public int? Actions { get; set; }
        public int? Num { get; set; }

        //فى شاشة عرض الاقساط
        public string InstallmentDate { get; set; }
        public string PaymentDate { get; set; }
        public bool? PaidInTime { get; set; }
        public double Amount { get; set; }
    }
}