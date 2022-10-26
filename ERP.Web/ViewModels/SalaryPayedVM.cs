using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SalaryPayedVM
    {
        public SalaryPayedVM()
        {
            ContractSchedulings = new List<Scheduling>();
        }
        public DateTime MonthYear { get; set; }
        public List<Scheduling> ContractSchedulings { get; set; }

        // تستخدم مع الطريقة الثانية من صرف رواتب الموظفين(شهرى/اسبوعى/يومى)
        public int? ContractSalaryTypId { get; set; }
        public Guid? DepartmntId { get; set; }
        public DateTime? dt { get; set; }
    }
    public class Scheduling
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public string SchedulingName { get; set; }
        public Guid? AccountsTreeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double Salary { get; set; }
        public double Safy { get; set; }
        public double PayedValue { get; set; }
        public double RemindValue { get; set; }
        public bool IsApproval { get; set; }
        public bool IsPayed { get; set; }

    }
}