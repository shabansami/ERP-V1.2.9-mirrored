using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class MaintenanceVM : Maintenance
    {
        public string SellInvoiceNumber { get; set; }
    }
}