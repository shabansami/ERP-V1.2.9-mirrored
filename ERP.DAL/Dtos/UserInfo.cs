using ERP.DAL.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Dtos
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? PersonId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? StoreId { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; } // الحساب مفعل
        public SessionKey SessionKey { get; set; }
        public string EntityName { get; set; }
        public string WaterMarkUrl { get; set; }
        public bool IsAdmin { get; set; }
    }
}
