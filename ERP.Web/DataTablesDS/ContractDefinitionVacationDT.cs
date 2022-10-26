using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ContractDefinitionVacationDT
    {
        public Guid Id { get; set; }
        public string VacationTypeName { get; set; }
        public Guid? VacationTypeId { get; set; }
        public int DayNumber { get; set; }
        public string Actions { get; set; }
        public string DT_Datasource { get; set; }

    }
}