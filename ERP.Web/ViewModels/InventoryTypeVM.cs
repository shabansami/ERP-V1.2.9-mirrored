using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class InventoryTypeVM
    {
        public string InventoryTypeId { get; set; }
        //تاريخ بداية ونهاية السنة المالية
        public DateTime? FinancialYearStartDate { get; set; }
        public DateTime? FinancialYearEndDate { get; set; }

    }
}