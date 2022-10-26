using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class InstallmentNotPaidDto
    {
        public string CustomerName { get; set; }
        public string Mob { get; set; }
        public int SchedulesCount { get; set; }
        public double ScheduleAmount { get; set; }
        public double ScheduleTotalAmount { get; set; }
        public int? Actions { get; set; }
        public int? Num { get; set; }
    }

}