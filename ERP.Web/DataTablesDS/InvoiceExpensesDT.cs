using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class InvoiceExpensesDT
    {
        public Guid? ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public double ExpenseAmount { get; set; }
        public string Notes { get; set; }
        public string DT_Datasource { get; set; }
        public string Actions { get; set; }
    }
}