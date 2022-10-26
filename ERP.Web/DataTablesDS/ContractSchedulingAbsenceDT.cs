using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ContractSchedulingAbsenceDT
    {
        public Guid Id { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public double AbsenceDayNumber { get; set; }
        //public double PenaltyNumber { get; set; }//قيمة الخصم فى حالة العدد بالايام  
        public string VacationTypeName { get; set; } //نوع الاجازة المخصوم منها 
    }
}