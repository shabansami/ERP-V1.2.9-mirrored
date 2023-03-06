using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Dtos;

namespace ERP.Web.Services
{
    public static class EmployeeService
    {
        //كل العملاء الساريه تعاقداتهم
        public static List<DropDownList> GetEmployees(Guid? departmentId = null)
        {
            using (var db = new VTSaleEntities())
            {
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                IQueryable<Employee> employees = null;
                employees = db.Employees.Where(x => !x.IsDeleted);
                if (departmentId != null)
                    employees = employees.Where(x => x.DepartmentId == departmentId);
                employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));

                return employees.Select(x => new DropDownList { Id = x.Id, Name = x.Person.Name }).ToList();
            }
        }

        //كل المناديب الساريه تعاقداتهم
        public static List<DropDownList> GetSaleMens(Guid? departmentId = null)
        {
            using (var db = new VTSaleEntities())
            {
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                IQueryable<Employee> employees = null;
                employees = db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen);
                if (departmentId != null)
                    employees = employees.Where(x => x.DepartmentId == departmentId);
                employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));

                return employees.Select(x => new DropDownList { Id = x.Id, Name = x.Person.Name }).ToList();
            }
        }

        //التأكد من ان المندوب يتبع اى مخزن حاليا
        public static Guid? GetSaleMenByStore(Guid? storeId)
        {
            if (storeId == null)
                return null;
            using (var db = new VTSaleEntities())
            {
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                IQueryable<Employee> employees = null;
                employees = db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen );
                employees=employees.Where(x=>x.EmployeeStores.Any(s => s.StoreId == storeId));
                employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
                var t = employees.ToList();
                //الحصول على اخر مندوب تم تحديده للمخزن من جدول حركات تحديد مخازن المناديب 
                var saleMenStoreHistoryrr = db.SaleMenStoreHistories.Where(x => !x.IsDeleted && x.StoreId == storeId).OrderByDescending(x => x.Id);
                var saleMenStoreHistory = db.SaleMenStoreHistories.Where(x => !x.IsDeleted && x.StoreId == storeId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (saleMenStoreHistory != null)
                {
                    var employeeId = saleMenStoreHistory.EmployeeId;
                    if (employees.Any(e => e.Id == employeeId))
                        return employeeId;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        //العملاء بدلالة المندوب فى 
        public static List<DropDownList> GetCustomerByCategory(Guid? categoryId, Guid? saleMenId, Guid? customerId) // category id
        {
            if (categoryId != null)
            {
                using (var db = new VTSaleEntities())
                {
                    if (saleMenId != null)
                        return db.SaleMenCustomers.Where(x => !x.IsDeleted && x.EmployeeId == saleMenId && x.CustomerPerson.PersonCategoryId == categoryId).Select(x => new DropDownList { Id = x.CustomerPerson.Id, Name = x.CustomerPerson.Name }).ToList();
                    else
                        return db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) && x.PersonCategoryId == categoryId && x.IsActive || x.Id == customerId).Select(x => new DropDownList { Id = x.Id, Name = x.Name }).ToList();
                }
            }
            else
                return new List<DropDownList>();
        }
        //اشهر/اسابيع/اليوم الموظف المتبقية المتعاقد عليها 
        public static List<DropDownList> GetContractSchedulingEmployee(string employeeId)
        {
            Guid Id;
            if (Guid.TryParse(employeeId, out Id))
            {
                using (var db = new VTSaleEntities())
                {
                    var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == Id).FirstOrDefault();
                    var list = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id && !x.IsApproval).OrderBy(x => x.Name).Select(x => new DropDownList { Id = x.Id, Name = x.Name /*x.MonthYear.ToString().Substring(0,7)*/ }).ToList();
                    return list;

                }
            }
            else
                return new List<DropDownList>();
        }

        #region تقارير المناديب

        public static SaleMenAccountVM GetSaleMenAccount(Guid? employeeId, DateTime? dtFrom, DateTime? dtTo, bool allTimes, bool isFinalApproval)
        {
            using (var db = new VTSaleEntities())
            {
                SaleMenAccountVM saleMenAccountVM = new SaleMenAccountVM();
                //فواتير البيع 
                var sell = db.SellInvoices.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.BySaleMen);
                if (isFinalApproval)
                    sell = sell.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    sell = sell.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                saleMenAccountVM.SellInvoices = sell.Select(x => new RptInvoiceDto
                {
                    InvoiceNumber = x.InvoiceNumber,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                saleMenAccountVM.TotalSellInvoices = saleMenAccountVM.SellInvoices.Sum(x => x.Amount);
                //فواتير مرتجع البيع 
                var sellBack = db.SellBackInvoices.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.BySaleMen);
                if (isFinalApproval)
                    sellBack = sellBack.Where(x => x.IsFinalApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    sellBack = sellBack.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
                saleMenAccountVM.SellBackInvoices = sellBack.Select(x => new RptInvoiceDto
                {
                    InvoiceNumber = x.InvoiceNumber,
                    InvoDate = x.InvoiceDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Safy,
                    ApprovalStatus = x.IsFinalApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                saleMenAccountVM.TotalSellBackInvoices = saleMenAccountVM.SellBackInvoices.Sum(x => x.Amount);
                // التحصيل النقدى 
                var customerPayment = db.CustomerPayments.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.BySaleMen);
                if (isFinalApproval)
                    customerPayment = customerPayment.Where(x => x.IsApproval);
                if (dtFrom != null && dtTo != null && !allTimes)
                    customerPayment = customerPayment.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo);
                saleMenAccountVM.CustomerPayments = customerPayment.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.PaymentDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsApproval ? "معتمده" : "لم يتم اعتمادها بعد"
                }).ToList();
                var totalVal = saleMenAccountVM.CustomerPayments.Sum(x => x.Amount);
                saleMenAccountVM.TotalCustomerPayments = totalVal;
                //العمولة على التحصيل 
                var empCommission = db.Employees.FirstOrDefault(x => x.Id == employeeId).CommissionPercentage;
                if (empCommission > 0)
                {
                    var is100Percentage = db.Employees.Where(x => x.Id == employeeId).FirstOrDefault().Is100Percentage;
                    saleMenAccountVM.CommissionPayment = Math.Round((totalVal * empCommission) / (is100Percentage ? 100 : 1000), 2);
                    saleMenAccountVM.CommissionSell = Math.Round((saleMenAccountVM.TotalSellInvoices * empCommission) / (is100Percentage ? 100 : 1000), 2);
                    saleMenAccountVM.CommissionSellBack = Math.Round((saleMenAccountVM.TotalSellBackInvoices * empCommission) / (is100Percentage ? 100 : 1000), 2);
                }
                // الشيكات 
                var cheque = db.Cheques.Where(x => !x.IsDeleted && x.EmployeeId == employeeId && x.BySaleMen);
                if (isFinalApproval)
                    cheque = cheque.Where(x => x.IsCollected || x.IsRefused);
                if (dtFrom != null && dtTo != null && !allTimes)
                    cheque = cheque.Where(x => DbFunctions.TruncateTime(x.CheckDate) >= dtFrom && DbFunctions.TruncateTime(x.CheckDate) <= dtTo);
                saleMenAccountVM.Cheques = cheque.Select(x => new RptInvoiceDto
                {
                    Id = x.Id,
                    InvoDate = x.CheckDate,
                    CustomerName = x.PersonCustomer.Name,
                    Amount = x.Amount,
                    ApprovalStatus = x.IsCollected ? "تم تحصيله" : x.IsRefused ? "تم رفضه/إرجاعه" : "تحت التحصيل"
                }).ToList();
                saleMenAccountVM.TotalCheques = saleMenAccountVM.Cheques.Sum(x => x.Amount);

                return saleMenAccountVM;
            }

        }

        #endregion

        #region فروع اليوزر المسموحة له
        public static List<DropDownList> GetBranchesByUser(UserInfo userInfo)
        {
            if (userInfo == null)
                return new List<DropDownList>();
            IQueryable<Branch> branches = null;
            using (var db=new VTSaleEntities())
            {
                branches = db.Branches.Where(x => !x.IsDeleted);
                if (!userInfo.IsAdmin)//اليوزر ليس له صلاحية على كل الفروع
                {
                    var empBranches = db.EmployeeBranches.Where(x => !x.IsDeleted && x.EmployeeId == userInfo.EmployeeId);
                    if (empBranches.Any())//اليوزر له صلاحية على فرع محدد 
                        branches = branches.Where(x => empBranches.Any(e=>e.BranchId==x.Id));
                    else//اليوزر ليس له صلاحية على فرع محدد 
                            return new List<DropDownList>();                 
                }
                return branches.Select(x=>new DropDownList { Id=x.Id,Name=x.Name}).ToList();
            }
            
        }
        #endregion
        //public static IQueryable<Employee> GetEmployees(VTSaleEntities db, int? departmentId = null)
        //{
        //    //using (var db=new VTSaleEntities())
        //    //{
        //    var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
        //    IQueryable<Employee> employees = null;
        //    employees = db.Employees.Where(x => !x.IsDeleted);
        //    if (departmentId != null)
        //        employees = employees.Where(x => x.DepartmentId == departmentId);
        //    employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
        //    return employees;
        //    //}
        //}
    }
}