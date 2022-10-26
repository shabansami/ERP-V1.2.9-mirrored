using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class Nationality:BaseModelInt
    {
        public Nationality()
        {
            this.Persons = new HashSet<Person>();
        }
        public string Name { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
    }
}
