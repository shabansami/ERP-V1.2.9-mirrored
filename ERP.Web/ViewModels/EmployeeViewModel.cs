using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {
            this.BranchIds = new List<Guid>();
        }
        public Guid Id { get; set; }
        public Nullable<Guid> PersonHidId { get; set; }
        public List<Guid> BranchIds { get; set; }
        public Nullable<Guid> JobId { get; set; }
        public Nullable<Guid> DepartmentId { get; set; }
        public string NationalID { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        public Nullable<int> SocialStatusId { get; set; }
        public Nullable<System.DateTime> DateOfHiring { get; set; }
        public bool HasRole { get; set; }
        public Nullable<double> Salary { get; set; }
        public double CommissionPercentage { get; set; }
        public bool Is100Percentage { get; set; } //نسبة العمولة فى المائة او فى الالف 
        public string PassportNumber { get; set; } //رقم جواز السفر
        public DateTime? PassportExpirationDate { get; set; } //تاريخ انتهاء الجواز
        public string PassportIssuer { get; set; } //جهة الاصدار جواز السفر
        public string PassportJob { get; set; } //المهنة فى جواز السفر

        public string ResidenceNumber { get; set; } //رقم الاقامة
        public DateTime? ResidenceExpirationDate { get; set; } //تاريخ انتهاء الاقامة
        public string ResidenceIssuer { get; set; } //جهة الاصدار الاقامة
        public string ResidenceJob { get; set; } //المهنة الاقامة
        public int? MedicalInsurancePolicyNumber { get; set; } //رقم البوليصة فى التامين الصحى
        public string MedicalInsuranceCategory { get; set; } //فئة التامين الصحى
        public DateTime? MedicalInsuranceExpirationDate { get; set; } //تاريخ الانتهاء التامين الصحى
        public DateTime? MedicalInsuranceReleaseDate { get; set; } //تاريخ الاصدار التامين الصحى
        public string MedicalInsuranceCompany { get; set; } //شركة التامين الصحى
        public string SocialSecurityNumber { get; set; } //الرقم التأمينى التامينات الاجتماعية
        public double SocialSecurityCurrentBalance { get; set; } //الرصيد الحالى التامينات الاجتماعية

        public bool IsSaleMen { get; set; }
        public Nullable<int> GenderId { get; set; }
        public Nullable<Guid> AreaId { get; set; }
        public Nullable<Guid> StoreId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Job Job { get; set; }
        public virtual Person Person { get; set; }
        public virtual SocialStatus SocialStatus { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}