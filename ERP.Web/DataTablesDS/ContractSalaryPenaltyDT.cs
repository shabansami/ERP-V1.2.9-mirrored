using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ContractSalaryPenaltyDT
    {
        public Guid Id { get; set; }
        public string SalaryPenaltyTypeName { get; set; }
        public Guid? SalaryPenaltyId { get; set; }
        public string SalaryPenaltyName { get; set; }
        public double SalaryPenaltyAmount { get; set; }
        public string SalaryPenaltyNotes { get; set; }
        public string Actions { get; set; }
        public string DT_Datasource { get; set; }
        //تستخدم مع اعتماد الرواتب
        public double SalaryPenaltyAmountPayed { get; set; }

    }
}