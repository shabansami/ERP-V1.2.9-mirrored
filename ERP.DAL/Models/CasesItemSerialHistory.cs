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

    public partial class CasesItemSerialHistory : BaseModel
    {
        [ForeignKey(nameof(SerialCas))]
        public Nullable<int> SerialCaseId { get; set; }
        [ForeignKey(nameof(ItemSerial))]
        public Nullable<Guid> ItemSerialId { get; set; }
        public Nullable<Guid> ReferrenceId { get; set; }
    
        public virtual ItemSerial ItemSerial { get; set; }
        public virtual SerialCas SerialCas { get; set; }
    }
}
