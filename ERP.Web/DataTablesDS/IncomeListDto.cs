using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class IncomeListDto // قائمة الدخل
    {
        public Guid? ParentId { get; set; }
        public long AccountNumber { get; set; } //رقم الحساب
        public string AccountName { get; set; }//اسم الحساب
        public double Amount { get; set; }//المبلغ الرصيد
        public double Whole { get; set; } //كلى
        public double Partial { get; set; }//جزئى
        public int? Num { get; set; } = null;
        public bool IsTotal { get; set; }
        public int? AssetsDepreciationStatus { get; set; }//حالة مصروف الاهلاك من ضمن الصمروفات او مفصول عنها
        public bool IsThirdLevel { get; set; }//المستوى الثالث فى الدليل المحاسبى لتمميزه بلون مخلتف ف تقرير قائمة الدخل
    }

}