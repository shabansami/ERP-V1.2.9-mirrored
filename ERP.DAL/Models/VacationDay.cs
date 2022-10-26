//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.DAL.Models;

    public partial class VacationDay : BaseModel
    {
        [ForeignKey(nameof(Department))]
        public Nullable<Guid> DepartmentId { get; set; }
        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }
        public Nullable<bool> IsWeekly { get; set; }
        [ForeignKey(nameof(WeekDay))]
        public Nullable<int> DayId { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
    
        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual WeekDay WeekDay { get; set; }
    }
}
