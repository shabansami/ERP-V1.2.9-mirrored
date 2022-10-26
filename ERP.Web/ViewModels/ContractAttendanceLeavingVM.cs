using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ContractAttendanceLeavingVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> ContractSchedulingId { get; set; }
        public DateTime AttendanceLeavingDate { get; set; }
        public Guid? EmployeeId { get; set; }
        public string AttendanceTime { get; set; }
        public string LeavingTime { get; set; }
        public string Notes { get; set; }

    }
}