using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemActionDto
    {
        public string ActionType { get; set; }//نوع الحركة 
        public string StoreName { get; set; }
        public string PersonName { get; set; }//المورد/العميل
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public int? Num { get; set; }
    }
}