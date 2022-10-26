using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class StorePermission : BaseModel
    {
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }
        [ForeignKey(nameof(Safe))]
        public Nullable<Guid> SafeId { get; set; }
        [ForeignKey(nameof(Person))]
        public Nullable<Guid> PersonId { get; set; }
        [ForeignKey(nameof(AccountTree))]
        public Nullable<Guid> AccountTreeId { get; set; }
        public Nullable<System.DateTime> PermissionDate { get; set; }
        public bool IsApproval { get; set; }
        public bool IsReceive { get; set; }
        public virtual Store Store { get; set; }
        public virtual Safe Safe { get; set; }
        public virtual Person Person { get; set; }
        public virtual AccountsTree AccountTree { get; set; }
        public virtual ICollection<StorePermissionItem> StorePermissionItems { get; set; }
    }
}
