using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class VacationDayVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> DepartmentId { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
        public Nullable<bool> IsWeekly { get; set; }
        public Nullable<int> DayId { get; set; }
        public List<int> DayIds { get; set; }
        public bool chk_allDepart { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }

    }
}