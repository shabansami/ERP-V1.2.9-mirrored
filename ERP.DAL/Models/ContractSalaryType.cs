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

    public partial class ContractSalaryType : BaseModelInt
    {
        //TODO: Review
        public ContractSalaryType()
        {
            this.Contracts = new HashSet<Contract>();
        }
    
        public string Name { get; set; }
    
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
