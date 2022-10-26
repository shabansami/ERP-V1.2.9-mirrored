﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SellInvoicePaymentVM
    {
        public SellInvoicePaymentVM()
        {
            sellInvoicePayments = new List<SellInvoicePaymentDetails>();
        }
        public Nullable<Guid> SellInvoiceId { get; set; }
        public Guid? InvoiceGuid { get; set; }
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
        public List<SellInvoicePaymentDetails> sellInvoicePayments { get; set; }
    }

    public class SellInvoicePaymentDetails
    {
        public Guid Id { get; set; }
        public Guid PayGuid { get; set; }
        public Guid? SellInvoiceId { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public double Amount { get; set; }
        public bool IsPayed { get; set; }
        //public string DT_Datasource { get; set; }

        //installments
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<Guid> SaleMenId { get; set; }
        public bool PaidInTime { get; set; }

        public string Actions { get; set; }

    }
}