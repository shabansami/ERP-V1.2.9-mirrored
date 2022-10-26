using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public class NotificationEmployee:BaseModel
    {
        [ForeignKey(nameof(Notification))]
        public Nullable<Guid> NotificationId { get; set; }

        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }

        public virtual Notification Notification { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
