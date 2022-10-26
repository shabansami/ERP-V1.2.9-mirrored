using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class Region : BaseModel
    {
        public Region()
        {

        }
        [ForeignKey(nameof(Area))]
        public Nullable<Guid> AreaId { get; set; }
        public string Name { get; set; }

        public virtual Area Area { get; set; }
        public virtual ICollection<District> Districts { get; set; }

    }
}
