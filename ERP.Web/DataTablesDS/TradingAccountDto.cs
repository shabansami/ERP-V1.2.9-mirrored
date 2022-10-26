using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class TradingAccountDto //حساب المتاجرة
    {
        public Guid? ParentId { get; set; }
        public long AccountNumber { get; set; } //رقم الحساب
        public string AccountName { get; set; }//اسم الحساب
        public double Debit { get; set; }
        public double Credit { get; set; }
        public int Order { get; set; }
        public int? Num { get; set; } = null;
        public bool Visiable { get; set; } = true;
    }
}