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

    public partial class ContractAttendanceLeaving : BaseModel
    {
        [ForeignKey(nameof(ContractScheduling))]
        public Nullable<Guid> ContractSchedulingId { get; set; }
        public System.DateTime AttendanceLeavingDate { get; set; }
        public Nullable<System.TimeSpan> AttendanceTime { get; set; }
        public Nullable<System.TimeSpan> LeavingTime { get; set; }
        public string Notes { get; set; }
    
        public virtual ContractScheduling ContractScheduling { get; set; }
    }
}
