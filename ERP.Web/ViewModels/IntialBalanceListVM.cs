using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class IntialBalanceListVM
    {
        public DateTime? DateIntial { get; set; }
        public Guid? BranchId { get; set; }
        public List<IntialBalanceVM> Persons { get; set; }
    }
}