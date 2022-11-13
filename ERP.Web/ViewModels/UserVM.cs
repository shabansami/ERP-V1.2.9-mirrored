using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class UserVM
    {
        public Guid Id { get; set; }
        public Nullable<Guid> EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Pass { get; set; }
        public Nullable<Guid> RoleId { get; set; }  
        public bool IsActive { get; set; }

    }
}