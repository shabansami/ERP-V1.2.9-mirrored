using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class District : BaseModel
    {
        public District()
        {

        }
        [ForeignKey(nameof(Region))]
        public Nullable<Guid> RegionId { get; set; }
        public string Name { get; set; }

        public virtual Region Region { get; set; }
    }
}
