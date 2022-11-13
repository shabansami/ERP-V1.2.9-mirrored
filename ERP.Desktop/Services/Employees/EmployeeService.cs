using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL;
using ERP.Desktop.DTOs;
using System.Data.Entity;
using ERP.DAL.Dtos;
using ERP.Web.DataTablesDS;

namespace ERP.Desktop.Services.Employees
{
    internal class EmployeeService
    {
        public List<IDNameVM> GetEmployees(Guid? deparmentId = null)
        {
            using (var db = new VTSaleEntities())
            {
                IQueryable<Employee> employees = null;
                employees = db.Employees.Where(x => !x.IsDeleted);
                if (deparmentId != null&&deparmentId!=Guid.Empty)
                    employees = employees.Where(x => x.DepartmentId == deparmentId);
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
                return employees.Select(x => new IDNameVM { ID = x.Id, Name = x.Person.Name }).ToList();
            }
        }

        public List<IDNameVM> GetContractSchedulingEmployee(string id, VTSaleEntities db)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == Id).FirstOrDefault();
                var list = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id && !x.IsApproval);
                return list.Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            }
            else
                return new List<IDNameVM>();
        }
        public DtoResultObj<ContractLoan> AddNew(ContractLoan contractLoan, VTSaleEntities db)
        {
            var result = new DtoResultObj<ContractLoan>();

            if (contractLoan.Amount == 0 || contractLoan.BranchId == null || contractLoan.LoanDate == null || contractLoan.ContractSchedulingId == null || contractLoan.AmountMonth == 0 || contractLoan.NumberMonths == 0)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }

            if (contractLoan.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }


            db.ContractLoans.Add(contractLoan);


            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
            {
                result.IsSuccessed = true;
                result.Message = "تم الاضافة بنجاح";
                result.Object = contractLoan;
            }
            else
            {
                result.Message = "حدث خطأ اثناء تنفيذ العملية";
            }
            return result;
        }
        public DtoResultObj<ContractLoan> Edit(ContractLoan contractLoan, VTSaleEntities db)
        {
            var result = new DtoResultObj<ContractLoan>();

            if (contractLoan.Amount == 0 || contractLoan.BranchId == null || contractLoan.LoanDate == null || contractLoan.ContractSchedulingId == null || contractLoan.AmountMonth == 0 || contractLoan.NumberMonths == 0)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }

            if (contractLoan.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }

            if (contractLoan.Id!=Guid.Empty)
            {

                var model = db.ContractLoans.Where(x => x.Id == contractLoan.Id).FirstOrDefault();
                model.ContractSchedulingId = contractLoan.ContractSchedulingId;
                model.Amount = contractLoan.Amount;
                model.AmountMonth = contractLoan.AmountMonth;
                model.NumberMonths = contractLoan.NumberMonths;
                model.Notes = contractLoan.Notes;
                model.BranchId = contractLoan.BranchId;
                model.SafeId = contractLoan.SafeId;
                model.LoanDate = contractLoan.LoanDate;
                //model.IncomeTypeId = expenseIncome.IncomeTypeId;
                db.Entry(model).State = EntityState.Modified;

                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                {
                    result.Message = "تم التعديل بنجاح";
                    result.IsSuccessed = true;
                    result.Object = contractLoan;
                }
                else
                {
                    result.Message = "حدث خطأ اثناء تنفيذ العملية";
                }
            }
            else
            {
                result.Message = "خطأ في رقم السلفة ";
            }
            return result;
        }

        public DtoResult Delete(Guid id, VTSaleEntities db)
        {
            var result = new DtoResult();
            var model = db.ContractLoans.Where(x => x.Id == id).FirstOrDefault();
            if (model != null)
            {
                //التأكد من العملية لم يتم اعتمادها 
                if (model.IsApproval)
                    result.Message = "لا يمكن حذف عملية معتمده";

                model.IsDeleted = true;
                db.Entry(model).State = EntityState.Modified;
                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                {
                    result.Message = "تم الحذف بنجاح";
                    result.IsSuccessed = true;
                }
                else
                    result.Message = "حدث خطأ اثناء تنفيذ العملية";
            }
            else
                result.Message = "حدث خطأ اثناء تنفيذ العملية";
            return result;
        }

        #region فروع اليوزر المسموحة له
        public static List<IDNameVM> GetBranchesByUser(UserInfo userInfo)
        {
            if (userInfo == null)
                return new List<IDNameVM>();
            IQueryable<Branch> branches = null;
            using (var db = new VTSaleEntities())
            {
                branches = db.Branches.Where(x => !x.IsDeleted);
                if (!userInfo.AccessAllBranch)//اليوزر ليس له صلاحية على كل الفروع
                {
                    if (userInfo.BranchId != null)//اليوزر له صلاحية على فرع محدد 
                        branches = branches.Where(x => x.Id == userInfo.BranchId);
                    else//اليوزر ليس له صلاحية على فرع محدد 
                        return new List<IDNameVM>();
                }
                return branches.Select(x => new IDNameVM { ID = x.Id, Name = x.Name }).ToList();
            }

        }
        #endregion

    }
}
