using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptInvoiceDto
    {
        public Guid Id { get; set; }  
        public string InvoiceNumber { get; set; }//رقم الفاتورة
        public DateTime? InvoDate { get; set; }  // تاريخ الفاتورة 
        public string CustomerName { get; set; }// اسم العميل
        public double Amount { get; set; } //القيمة 
        public string ApprovalStatus { get; set; } // حالة الاعتما د
    }
}