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

    public partial class Job : BaseModel
    {
        public Job()
        {
            this.Employees = new HashSet<Employee>();
        }
    
        public string Name { get; set; }
        public string Tasks { get; set; } //������
        public string Requirements { get; set; } //���������
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
