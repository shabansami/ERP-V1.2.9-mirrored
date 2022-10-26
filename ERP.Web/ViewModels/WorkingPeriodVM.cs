using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class WorkingPeriodVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DelayFrom { get; set; }
        public string AbsenceOf { get; set; }
        public string Notes { get; set; }
    }
}