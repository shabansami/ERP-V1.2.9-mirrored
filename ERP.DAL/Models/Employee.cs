//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.DAL.Models;

    public partial class Employee : BaseModel
    {
        public Employee()
        {
            this.Cheques = new HashSet<Cheque>();
            this.Contracts = new HashSet<Contract>();
            this.CustomerPayments = new HashSet<CustomerPayment>();
            this.EmployeeGivingCustodies = new HashSet<EmployeeGivingCustody>();
            this.EmployeeReturnCashCustodies = new HashSet<EmployeeReturnCashCustody>();
            this.EmployeeReturnCustodies = new HashSet<EmployeeReturnCustody>();
            this.MaintenanceSaleMens = new HashSet<Maintenance>();
            this.MaintenanceRespones = new HashSet<Maintenance>();
            this.SaleMenCustomers = new HashSet<SaleMenCustomer>();
            this.SaleMenStoreHistories = new HashSet<SaleMenStoreHistory>();
            this.SellBackInvoices = new HashSet<SellBackInvoice>();
            this.SellInvoices = new HashSet<SellInvoice>();
            this.Shifts = new HashSet<Shift>();
            this.ShiftsOfflines = new HashSet<ShiftsOffline>();
            this.StoresTransfersFrom = new HashSet<StoresTransfer>();
            this.StoresTransfersTo = new HashSet<StoresTransfer>();
            this.VacationDays = new HashSet<VacationDay>();
            this.NotificationEmployees = new HashSet<NotificationEmployee>();
            this.EmployeeBranches = new HashSet<EmployeeBranch>();
            //this.EmployeeProductions = new HashSet<ProductionOrder>();
            //this.EmployeeOperations = new HashSet<ProductionOrder>();
            this.ProductionLineEmployees = new HashSet<ProductionLineEmployee>();
            this.EmployeeStores = new HashSet<EmployeeStore>();
            this.EmployeeSafes = new HashSet<EmployeeSafe>();

        }

        [ForeignKey(nameof(Person))]
        public Nullable<Guid> PersonId { get; set; }
        [ForeignKey(nameof(Department))]
        public Nullable<Guid> DepartmentId { get; set; }
        [ForeignKey(nameof(Job))]
        public Nullable<Guid> JobId { get; set; }
        public bool IsSaleMen { get; set; }
        public string NationalID { get; set; }
        public Nullable<System.DateTime> BirthDay { get; set; }
        [ForeignKey(nameof(SocialStatus))]
        public Nullable<int> SocialStatusId { get; set; }
        public Nullable<System.DateTime> DateOfHiring { get; set; }
        public bool HasRole { get; set; }
        public double CommissionPercentage { get; set; }
        public bool Is100Percentage { get; set; } //���� ������� �� ������ �� �� ����� 
        public string PassportNumber { get; set; } //��� ���� �����
        public DateTime? PassportExpirationDate { get; set; } //����� ������ ������
        public string PassportIssuer { get; set; } //��� ������� ���� �����
        public string PassportJob { get; set; } //������ �� ���� �����

        public string ResidenceNumber { get; set; } //��� �������
        public DateTime? ResidenceExpirationDate { get; set; } //����� ������ �������
        public string ResidenceIssuer { get; set; } //��� ������� �������
        public string ResidenceJob { get; set; } //������ �������
        public int? MedicalInsurancePolicyNumber { get; set; } //��� �������� �� ������� �����
        public string MedicalInsuranceCategory { get; set; } //��� ������� �����
        public DateTime? MedicalInsuranceExpirationDate { get; set; } //����� �������� ������� �����
        public DateTime? MedicalInsuranceReleaseDate { get; set; } //����� ������� ������� �����
        public string MedicalInsuranceCompany { get; set; } //���� ������� �����
        public string SocialSecurityNumber { get; set; } //����� �������� ��������� ����������
        public double SocialSecurityCurrentBalance { get; set; } //������ ������ ��������� ����������

        public virtual Person Person { get; set; }
        public virtual Department Department { get; set; }
        public virtual Job Job { get; set; }
        public virtual SocialStatus SocialStatus { get; set; }


        public virtual ICollection<Cheque> Cheques { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<CustomerPayment> CustomerPayments { get; set; }
        public virtual ICollection<EmployeeGivingCustody> EmployeeGivingCustodies { get; set; }
        public virtual ICollection<EmployeeReturnCashCustody> EmployeeReturnCashCustodies { get; set; }
        public virtual ICollection<EmployeeReturnCustody> EmployeeReturnCustodies { get; set; }
        //Maintenances
        [InverseProperty(nameof(Maintenance.EmployeeSaleMen))]
        public virtual ICollection<Maintenance> MaintenanceSaleMens { get; set; }
        //Maintenances1
        [InverseProperty(nameof(Maintenance.EmployeeResponse))]
        public virtual ICollection<Maintenance> MaintenanceRespones { get; set; }
        public virtual ICollection<SaleMenCustomer> SaleMenCustomers { get; set; }
        public virtual ICollection<SaleMenStoreHistory> SaleMenStoreHistories { get; set; }
        public virtual ICollection<SellBackInvoice> SellBackInvoices { get; set; }
        public virtual ICollection<SellInvoice> SellInvoices { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<ShiftsOffline> ShiftsOfflines { get; set; }
        [InverseProperty(nameof(StoresTransfer.EmployeeFrom))]
        public virtual ICollection<StoresTransfer> StoresTransfersFrom { get; set; }
        [InverseProperty(nameof(StoresTransfer.EmployeeTo))]
        public virtual ICollection<StoresTransfer> StoresTransfersTo { get; set; }
        public virtual ICollection<VacationDay> VacationDays { get; set; }
        public virtual ICollection<NotificationEmployee> NotificationEmployees { get; set; }
        public virtual ICollection<EmployeeBranch> EmployeeBranches { get; set; }
        //public virtual ICollection<ProductionOrder> EmployeeProductions { get; set; }
        //public virtual ICollection<ProductionOrder> EmployeeOperations { get; set; }
        public virtual ICollection<ProductionLineEmployee> ProductionLineEmployees { get; set; }
        public virtual ICollection<EmployeeStore> EmployeeStores { get; set; }
        public virtual ICollection<EmployeeSafe> EmployeeSafes { get; set; }

    }
}
