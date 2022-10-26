using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SaleMenStoreVM
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? StoreId { get; set; }
    }
}