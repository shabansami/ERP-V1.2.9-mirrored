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
    using ERP.DAL.Models;

    public partial class WeekDay : BaseModelInt
    {
        public WeekDay()
        {
            this.VacationDays = new HashSet<VacationDay>();
        }
    
        public string Name { get; set; }
        public Nullable<int> Arrange { get; set; }
    
        public virtual ICollection<VacationDay> VacationDays { get; set; }
    }
}
