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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.DAL.Models;

    public partial class Page : BaseModelInt
    {
        public Page()
        {
            this.PagesRoles = new HashSet<PagesRole>();
        }
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string OtherUrls { get; set; }
        public string Icon { get; set; }
        public bool IsPage { get; set; }
        public int OrderNum { get; set; }

        public virtual ICollection<PagesRole> PagesRoles { get; set; }
    }
}
