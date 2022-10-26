using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptMaintenanceProblemDto
    {
        public RptMaintenanceProblemDto()
        {
            InvoicesData = new List<MaintenanceInvoice>();
        }
        public Guid? Id { get; set; }//item id
        public string ItemName { get; set; }
        public double TotalItemSafy { get; set; }
        //مع تقارير الاعطال الاكثر شيوعا
        public string MaintenProblemTypeName { get; set; } // نوع العطل الاكثر شيوعا
        public double MaintenProblemTypeCount { get; set; } //عدد الاعطال
        public List<MaintenanceInvoice> InvoicesData { get; set; }

    }
    public class MaintenanceInvoice
    {
        public Guid? MaintenanceId { get; set; }
        public string InvoiceNum { get; set; }
        //public Guid? InvoiceGuid { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public double ItemSafy { get; set; }
        public string MaintenProblemTypeName { get; set; }
    }

}