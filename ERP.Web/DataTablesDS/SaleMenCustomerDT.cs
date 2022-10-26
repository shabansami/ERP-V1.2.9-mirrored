using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class SaleMenCustomerDT
    {
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string DT_Datasource { get; set; }
        public string Actions { get; set; }
    }
}