using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SaleMenAreaVM
    {
        public Guid Id { get; set; }
        public Guid? SaleMenId { get; set; }
        public Guid? AreaId { get; set; }
        public Guid? CityId { get; set; }
        public string AreaName { get; set; }
    }
}