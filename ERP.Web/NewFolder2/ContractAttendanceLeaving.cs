//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.Web.NewFolder2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContractAttendanceLeaving
    {
        public int Id { get; set; }
        public Nullable<int> ContractSchedulingId { get; set; }
        public System.DateTime AttendanceLeavingDate { get; set; }
        public Nullable<System.TimeSpan> AttendanceTime { get; set; }
        public Nullable<System.TimeSpan> LeavingTime { get; set; }
        public string Notes { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ContractScheduling ContractScheduling { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
