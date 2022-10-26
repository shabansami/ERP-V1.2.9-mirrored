using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ContractSalaryAdditionsDT
    {
        public Guid Id { get; set; }
        public string SalaryAdditionTypeName { get; set; }
        public Guid? SalaryAdditionId { get; set; }
        public string SalaryAdditionName { get; set; }
        public double SalaryAdditionAmount { get; set; }
        public string SalaryAdditionNotes { get; set; }
        public string Actions { get; set; }
        public string DT_Datasource { get; set; }

        //تستخدم مع اعتماد الرواتب
        public double SalaryAdditionAmountPayed { get; set; }
        public bool IsAffactAbsence { get; set; }

    }
}