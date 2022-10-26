using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ResponsibleDT
    {
        public Guid Id { get; set; }
        public Guid? ResponsibleId { get; set; }
        public string ResponsibleName { get; set; }
        public string ResponsibleMob { get; set; }
        public string ResponsibleTel { get; set; }
        public string ResponsibleJob { get; set; }
        public string ResponsibleEmail { get; set; }
        public string ResponsibleTransfer { get; set; }
        public string Actions { get; set; }
        public string Num { get; set; }

    }
}