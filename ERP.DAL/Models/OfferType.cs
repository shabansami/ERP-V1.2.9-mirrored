using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public class OfferType:BaseModelInt
    {
        public OfferType()
        {
            this.Offers = new HashSet<Offer>();
        }
        public string Name { get; set; }

        public virtual ICollection<Offer> Offers { get; set; }  
    }
}
