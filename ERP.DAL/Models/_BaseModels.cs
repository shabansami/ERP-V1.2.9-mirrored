using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public abstract class Base
    {
        public Nullable<System.DateTime> CreatedOn { get; set; }

        [ForeignKey(nameof(UserCreater))]
        public Nullable<Guid> CreatedBy { get; set; }
        public virtual User UserCreater { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        [ForeignKey(nameof(UserModifier))]
        public Nullable<Guid> ModifiedBy { get; set; }
        public virtual User UserModifier { get; set; }

        public bool IsDeleted { get; set; }

        public Nullable<System.DateTime> DeletedOn { get; set; }

        [ForeignKey(nameof(UserDeletion))]
        public Nullable<Guid> DeletedBy { get; set; }
        public virtual User UserDeletion { get; set; }
    }

    public class BaseModel : Base
    {
        public Guid Id { get; set; }
    }


    public class BaseModelInt : Base
    {
        public int Id { get; set; }
    }
}
