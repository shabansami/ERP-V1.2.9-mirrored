using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class AuditBalanceDto // ميزان المراجعة
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public long AccountNumber { get; set; } //رقم الحساب
        public string AccountName { get; set; }//اسم الحساب
        public int? AccountLevel { get; set; }//مستوى الحساب
        public bool SelectedTree { get; set; }//true الحسابات التى لها ابناء  ... false الحسابات الاخيرة بدون ابناء
        public double IntialBalanceFrom { get; set; }//رصيد اول المدة(مدين
        public double IntialBalanceTo { get; set; }//رصيد اول المدة(دائن
        public double ActionsFrom { get; set; }//حركات الفترة(مدين)
        public double ActionsTo { get; set; }//حركات الفترة(دائن)
        public double SumFrom { get; set; }//ميزان المراجعة بالمجاميع(مدين 
        public double SumTo { get; set; }//ميزان المراجعة بالمجاميع (دائن
        public double ResultFrom { get; set; }//ميزان المراجعة بالارصدة مدين 
        public double ResultTo { get; set; }//ميزان المراجعة بالارصدة دائن 
        public int? Num { get; set; } = null;
        public List<AuditBalanceDto> children { get; set; }

    }

}