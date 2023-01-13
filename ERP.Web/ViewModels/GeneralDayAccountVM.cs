using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class GeneralDayAccountVM
    {
        public GeneralDayAccountVM()
        {
            GeneralDalies = new List<GeneralDayDto>();
            GeneralDayMultiAccounts = new List<GeneralDayAccountVM>();
        }
        public List<GeneralDayDto> GeneralDalies { get; set; }
        public double LastBalanceDebit { get; set; } //رصيد سابق مدين
        public double LastBalanceCredit { get; set; } //رصيد سابق دائن
        public double TotalDebit { get; set; }//اجمالى مدين
        public double TotalCredit { get; set; }//اجمالى دائن
        public double TotalBalanceDebit { get; set; }//اجمالى الرصيد مدين
        public double TotalBalanceCredit { get; set; }//اجمالى الرصيد دائن
        public double SafyBalanceDebit { get; set; }//الرصيد النهائى للحساب حتى نهاية الفترة مدين
        public double SafyBalanceCredit { get; set; }//الرصيد النهائى للحساب حتى نهاية الفترة دائن
        public DateTime? dtFrom { get; set; }
        public DateTime? dtTo { get; set; }
        public bool ShowRptEn { get; set; }
        public Guid? AccountTreeId { get; set; }
        public bool? CustomerRelated { get; set; }

        //مع تقرير رصيد الموردين والعملاء
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public long AccountNumber { get; set; }
        public string AccountsTreeName { get; set; }
        public int? Num { get; set; }

        //فى حالة كشف حساب المتعدد
        public List<GeneralDayAccountVM> GeneralDayMultiAccounts { get; set; }
        public string AccountTreeIds { get; set; }

    }
}