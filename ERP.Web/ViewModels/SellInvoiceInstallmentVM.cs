using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SellInvoiceInstallmentVM
    {
        public SellInvoiceInstallmentVM()
        {
            sellInvoiceInstallmentSchedules = new List<SellInvoiceInstallmentSchedules>();
        }
        public Nullable<Guid> SellInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public double Safy { get; set; } //صافى الفاتورة 
        public Nullable<System.DateTime> FirstDueDate { get; set; }//تاريخ اول دفعة
        public double RemindValue { get; set; }
        public double PayValue { get; set; } // قيمة القسط/الدفعة

        public double FinalSafy { get; set; } //صافى المبلغ المراد تقسيطة سواء بادخال المبلغ الاجمالى او احتساب عمولة 

        public double PayedValue { get; set; }//المبلغ المدفوع كمقدم
        public double TotalValue { get; set; }//المبلغ الاجمالى عبارة عن المتبقى والعمولة
        public double CommissionVal { get; set; }//قيمةالعمولة
        public double CommissionPerc { get; set; }//نسبة العمولة
        public double ProfitValue { get; set; }//قيمة الربح
        public int Duration { get; set; }
        public bool IsSell { get; set; } = true;
        public List<SellInvoiceInstallmentSchedules> sellInvoiceInstallmentSchedules { get; set; }
    }

    public class SellInvoiceInstallmentSchedules
    {
        public Guid ScheduleId { get; set; }
        public Guid? SellInvoiceId { get; set; }
        public double Amount { get; set; }
        public bool IsPayed { get; set; }
        //public string DT_Datasource { get; set; }

        //installments
        public Nullable<System.DateTime> InstallmentDate { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<Guid> SaleMenId { get; set; }
        public bool PaidInTime { get; set; }

        public string Actions { get; set; }

    }
}