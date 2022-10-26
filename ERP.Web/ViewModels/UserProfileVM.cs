using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class UserProfileVM
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public Guid? PersonId { get; set; }
        public string PersonName { get; set; }
        public string Address { get; set; }
        public string Mob1 { get; set; }
        public string Mob2 { get; set; }
        public Guid? AreaId { get; set; }
        public string Notes { get; set; }
    }
}