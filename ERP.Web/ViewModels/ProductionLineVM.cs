using ERP.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ProductionLineVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public List<ProductionLineEmpDto> ProductionLineEmps { get; set; }

    }
    public class ProductionLineEmpDto
    {
        public Guid ProductionLineId { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string JobName { get; set; }
        public bool IsProductionEmp { get; set; }//موظف بالانتاج ام لا 
        public string ProductionEmpType { get; set; }//موظف بالانتاج ام لا 
        public string DT_Datasource { get; set; }
        public int? Actions { get; set; }


    }
}