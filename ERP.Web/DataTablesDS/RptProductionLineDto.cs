using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptProductionLineDto
    {
        public Nullable<Guid> ProductionId { get; set; }
        public string ProductionNumber { get; set; }
        public string ProductionOrderDate { get; set; }
        public double EmployeeCost { get; set; }//اجر الموظف فى امر الانتاج
        public Nullable<Guid> ProductionLineId { get; set; }
        public string ProductionLineName { get; set; }
        public int? Num { get; set; }
        public int? Actions { get; set; }

    }
}