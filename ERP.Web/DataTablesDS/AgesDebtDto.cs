using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class AgesDebtDto
    {
        public string CustomerName { get; set; }
        public double DebtAmount { get; set; }//قيمة الدين
        public string InvoiceDate { get; set; }//تاريخ الفاتورة
        //public double TotalAmount { get; set; }//اجمالى ماتم تحصيله
                                               //public double TotalRemind { get; set; }//اجمالى المتبقى 
        public string InvoiceNumber { get; set; }//رقم فاتورة البيع
        public double AgeDebt { get; set; }//عمر الدين
        public double AgeDebtNotDue { get; set; }//عمر الدين لم يستحق بعد
        public double AgeDebt30 { get; set; }//عمر الدين
        public double AgeDebt60 { get; set; }//عمر الدين
        public double AgeDebt90 { get; set; }//عمر الدين
        public double AgeDebt120 { get; set; }//عمر الدين
        public double AgeDebt150 { get; set; }//عمر الدين
        public double AgeDebt180 { get; set; }//عمر الدين
        public double AgeDebtOver180 { get; set; }//عمر الدين

        public int? Num { get; set; } = null;
    }
}