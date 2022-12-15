using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SalaryApprovalVM
    {
        public Guid ContractSchedulingId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public double Salary { get; set; }
        public double Safy { get; set; }
        public string SchedulingName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalVacationDays { get; set; } // ايام الغياب المسموح بها فى مدة العقد 
        public double DayCost { get; set; } //تكلفة اجر الموظف لليوم الواحد 

        public List<ContractSalaryAdditionsDT> ContractAllowances { get; set; } //بدلات تضاف تلقائيا مع كل شهر 
        public List<ContractSalaryAdditionsDT> ContractSalaryAdditions { get; set; }//اضافات تضاف حسب كل شهر 
        public double TotalAllowances { get; set; } // اجمالى قيمة البدلات
        public double TotalSalaryAdditions { get; set; }// اجمالى قيمة الاضافات
        public double TotalSalaryAdditionAllowances { get; set; }// اجمالى قيمة البدلات والاضافات


        public List<ContractSalaryPenaltyDT> SalaryEveryMonthPenalties { get; set; }  //خصومات تخصم تلقائيا مع كل شهر 
        public List<ContractSalaryPenaltyDT> ContractSalaryPenalties { get; set; }  //خصومات تخصم حسب كل شهر 
        public double TotalSalaryEveryMonthPenalties { get; set; }// اجمالى قيمة الخصومات الشهرية
        public double TotalSalaryPenalties { get; set; } //اجمالى قيمة الخصومات حسب كل شهر 
        public double TotalAllSalaryPenalties { get; set; } // اجمالى قيمة الخصومات حسب كل شهر والخصومات الشهرية 

        public List<ContractLoansDT> ContractLoans { get; set; }  //سلف الموظف 
        public double TotalLoans { get; set; } // اجمالى قيمة السلف 

        public List<ContractSchedulingAbsenceDT> ContractSchedulingAllowAbsencs { get; set; }  //غياب مسموح به الموظفين 
        public List<ContractSchedulingAbsenceDT> ContractSchedulingPenaltyAbsencs { get; set; }  //غياب بخصم الموظفين 
        public double TotalAbsenceAllowed { get; set; } // ايام الغياب المسموح به 
        public double TotalAbsencePenalty { get; set; } //  اجمالى الغياب بخصم بعدد ايام  
        public double TotalAllAmountAbsences { get; set; } // اجمالى قيمة الغياب 

        //الداتيبل عند التعديل على جريد البدلات والخصومات 
        public string currentAllowances { get; set; }
        public string currentSalaryAdditions { get; set; }
        public string currentSalaryEveryMonthPenalties { get; set; }
        public string currentSalaryPenalties { get; set; }

        //فى حالة ان الموظف بالانتاج 
        public bool IsEmployeeProduction { get; set; }//الموظف بالانتاج 
        //فى حالة موظف ليس بالانتاج ولكن له اجر فى خطوط الانتاج
        public double ProductionAmount { get; set; }
    }
    public class EditDatatable
    {
        public Guid Id { get; set; }
        public double AmountPayed { get; set; }
    }

    //فى حالة ان الموظف بالانتاج 
    public class ProductionOrderEmployee
    {
        public Guid ProductionOrderId { get; set; }
        public int ProductionOrderHours { get; set; } //عدد ساعات امر الانتاج 
        public double HourlyWage { get; set; }//اجر الساعة للعامل 
    }
}