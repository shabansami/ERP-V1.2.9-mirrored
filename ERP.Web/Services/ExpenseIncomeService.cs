using ERP.DAL;
using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Services
{
    public class ExpenseIncomeService
    {
        public List<ExpenseDto> GetExpenses(VTSaleEntities db,string expenseTypeId, string isApprovalStatus, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.ExpenseIncomes.Where(x => !x.IsDeleted && x.IsExpense);
            int isAppStatus;
            if (!string.IsNullOrEmpty(expenseTypeId))
            {
                if (Guid.TryParse(expenseTypeId, out Guid expTypId))
                    list = list.Where(x => x.ExpenseIncomeTypeAccountTreeId == expTypId);
            }
            if (!string.IsNullOrEmpty(isApprovalStatus))
            {
                if (int.TryParse(isApprovalStatus, out isAppStatus))
                    if (isAppStatus == 1)
                        list = list.Where(x => x.IsApproval);
                    else if (isAppStatus == 2)
                        list = list.Where(x => !x.IsApproval);
            }
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo.Date);

            var data= list.OrderByDescending(x=>x.CreatedOn).Select(x => new ExpenseDto
            {
                Id = x.Id,
                CreatedOn=x.CreatedOn,
                ExpenseTypeName = x.ExpenseIncomeTypeAccountsTree.AccountName,
                IsApproval = x.IsApproval,
                IsApprovalStatus = x.IsApproval ? "1" : "2",
                SafeName = x.Safe.Name,
                Amount = x.Amount,
                Notes = x.Notes,
                //UserName = x.UserCreater.UserName,
                PaymentDate = x.PaymentDate.ToString(),
                PaidTo = x.PaidTo,
                Actions = n,
                Num = n
            }).ToList();
            return data;
        }
        public List<IncomeDto> GetIncomes(VTSaleEntities db, string incomeTypeId, string isApprovalStatus, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense);
            int isAppStatus;
            if (!string.IsNullOrEmpty(incomeTypeId))
            {
                if (Guid.TryParse(incomeTypeId, out Guid incomTypId))
                    list = list.Where(x => x.ExpenseIncomeTypeAccountTreeId == incomTypId);
            }
            if (!string.IsNullOrEmpty(isApprovalStatus))
            {
                if (int.TryParse(isApprovalStatus, out isAppStatus))
                    if (isAppStatus == 1)
                        list = list.Where(x => x.IsApproval);
                    else if (isAppStatus == 2)
                        list = list.Where(x => !x.IsApproval);
            }

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.PaymentDate) <= dtTo.Date);

            var data = list.OrderByDescending(x=>x.CreatedOn).
                Select(x => new IncomeDto 
            { Id = x.Id, 
                    CreatedOn = x.CreatedOn,
                    IncomeTypeName = x.ExpenseIncomeTypeAccountsTree.AccountName, 
                    IsApproval = x.IsApproval, 
                    IsApprovalStatus = x.IsApproval ? "1" : "2", 
                    SafeName = x.Safe.Name,
                    Amount = x.Amount, 
                    Notes = x.Notes,
                    PaidTo = x.PaidTo, 
                    PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList();
            return data;
          

        }
    }
}