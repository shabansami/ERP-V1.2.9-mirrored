using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Services.Transactions
{
    public class ExpensIncomeServices
    {
        VTSaleEntities db;
        public ExpensIncomeServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public List<ExpenseIncomeVM> GetAllExpenses(Guid? sheftId)
        {
            return db.ExpenseIncomes.Where(x => !x.IsDeleted && x.IsExpense&&x.ShiftOffLineID==sheftId).Select(x => new ExpenseIncomeVM { Id = x.Id,  ExpenseTypeName = x.ExpenseIncomeTypeAccountsTree.AccountName, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمد" : "غير معتمد", SafeName = x.Safe.Name,BranchName = x.Branch.Name, Amount = x.Amount, Notes = x.Notes, UserName = x.UserCreater.UserName, PaymentDate = x.PaymentDate.ToString() }).ToList();
        }

        public List<ExpenseIncomeVM> GetAllIncomes(Guid? sheftId)
        {
            return db.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense && x.ShiftOffLineID == sheftId).Select(x => new ExpenseIncomeVM { Id = x.Id,  IncomeTypeName = x.ExpenseIncomeTypeAccountsTree.AccountName, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمد" : "غير معتمد", SafeName = x.Safe.Name, BranchName = x.Branch.Name, Amount = x.Amount, Notes = x.Notes, UserName = x.UserCreater.UserName, PaymentDate = x.PaymentDate.ToString() }).ToList();
        }

        public DtoResultObj<ExpenseIncome> AddNew(ExpenseIncome expenseIncome)
        {
            var result = new DtoResultObj<ExpenseIncome>();

            if (expenseIncome.Amount == 0 || expenseIncome.BranchId == null || expenseIncome.PaymentDate == null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }

            if (expenseIncome.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }
            if (expenseIncome.IsExpense)
            {
                if (expenseIncome.ExpenseIncomeTypeAccountTreeId == null)
                {
                    result.Message = "تأكد من ادخال نوع المصروف";
                    return result;
                }
            }
            else
            {
                if (expenseIncome.ExpenseIncomeTypeAccountTreeId == null)
                {
                    result.Message = "تأكد من ادخال نوع الايراد";
                    return result;
                }
            }


            //expenseIncome.RowGuid = Guid.NewGuid();
            db.ExpenseIncomes.Add(expenseIncome);


            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
            {
                result.IsSuccessed = true;
                result.Message = "تم الاضافة بنجاح";
                result.Object = expenseIncome;
            }
            else
            {
                result.Message = "حدث خطأ اثناء تنفيذ العملية";
            }
            return result;
        }

        public DtoResultObj<ExpenseIncome> Edit(ExpenseIncome expenseIncome)
        {
            var result = new DtoResultObj<ExpenseIncome>();

            if (expenseIncome.Amount == 0 || expenseIncome.BranchId == null || expenseIncome.PaymentDate == null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }

            if (expenseIncome.SafeId == null)
            {
                result.Message = "تأكد من اختيار طريقة السداد بشكل صحيح";
                return result;
            }

            if (expenseIncome.IsExpense)
            {
                if (expenseIncome.ExpenseIncomeTypeAccountTreeId == null)
                {
                    result.Message = "تأكد من ادخال نوع المصروف";
                    return result;
                }
            }
            else
            {
                if (expenseIncome.ExpenseIncomeTypeAccountTreeId == null)
                {
                    result.Message = "تأكد من ادخال نوع الايراد";
                    return result;
                }
            }


            if (expenseIncome.Id != Guid.Empty)
            {

                var model = db.ExpenseIncomes.Where(x => x.Id == expenseIncome.Id).FirstOrDefault();
                model.Amount = expenseIncome.Amount;
                model.Notes = expenseIncome.Notes;
                model.BranchId = expenseIncome.BranchId;
                model.SafeId = expenseIncome.SafeId;
                model.PaymentDate = expenseIncome.PaymentDate;
                model.ExpenseIncomeTypeAccountTreeId = expenseIncome.ExpenseIncomeTypeAccountTreeId;
                //model.IncomeTypeId = expenseIncome.IncomeTypeId;
                db.Entry(model).State = EntityState.Modified;

                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                {
                    result.Message = "تم التعديل بنجاح";
                    result.IsSuccessed = true;
                    result.Object = expenseIncome;
                }
                else
                {
                    result.Message = "حدث خطأ اثناء تنفيذ العملية";
                }
            }
            else
            {
                result.Message = "خطأ في رقم " + (expenseIncome.IsExpense ? "مصروف" : "ايراد");
            }
            return result;
        }

        public DtoResult Delete(Guid id)
        {
            var result = new DtoResult();
            var model = db.ExpenseIncomes.Where(x => x.Id == id).FirstOrDefault();
            if (model != null)
            {
                //التأكد من العملية لم يتم اعتمادها 
                if (model.IsApproval)
                    result.Message = "لا يمكن حذف عملية معتمده .. لحذفها قم بفك الاعتماد من اصدار الويب";

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

    }

}
