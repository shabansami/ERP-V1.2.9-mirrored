using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SalariesPayedController : Controller
    {
        // GET: SalariesPayed
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        static List<ContractScheduling> contractSchedulingDS = new List<ContractScheduling>();

        #region عرض الموظفين المعتمد رواتبهم حسب الشهر والسنة 
        public ActionResult Index()
        {
            ViewBag.ContractSalaryTypId = new SelectList(db.ContractSalaryTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.DepartmntId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");

            return View(new SalaryPayedVM() { dt = Utility.GetDateTime() });
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Index(SalaryPayedVM vm)
        {
            try
            {
                int salaryTypeId;
                Guid DepartmentId;
                DateTime dateSearch;
                List<Scheduling> contractSchedulingDS = new List<Scheduling>();

                if (int.TryParse(vm.ContractSalaryTypId.ToString(), out salaryTypeId) && DateTime.TryParse(vm.dt.ToString(), out dateSearch))
                {
                    var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name");
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branches.Select(y => y.Id).FirstOrDefault()), "Id", "Name", 1);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
                    ViewBag.ContractSalaryTypId = new SelectList(db.ContractSalaryTypes.Where(x => !x.IsDeleted), "Id", "Name");
                    ViewBag.DepartmntId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");

                    IQueryable<ContractScheduling> contractSchedulings = null;
                    //اليومى
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily)
                        contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId && x.MonthYear == dateSearch);
                    else if (salaryTypeId == (int)ContractSalaryTypeCl.Monthly || salaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                        contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId && dateSearch >= x.MonthYear && dateSearch <= x.ToDate);

                    if (Guid.TryParse(vm.DepartmntId.ToString(), out DepartmentId))
                        contractSchedulings = contractSchedulings.Where(x => x.Contract.Employee.DepartmentId == DepartmentId);

                    contractSchedulingDS = contractSchedulings.Select(x => new Scheduling
                    {
                        Id = x.Id,
                        AccountsTreeId = x.Contract.Employee.Person.AccountsTreeCustomerId,
                        EmployeeName = x.Contract.Employee.Person.Name,
                        SchedulingName = x.Name,
                        //Year = x.MonthYear.Value.Year,
                        Salary = x.Contract.Salary,
                        IsApproval = x.IsApproval,
                        IsPayed = x.IsPayed,
                        Safy=x.Safy,
                        RemindValue=x.RemindValue,
                        PayedValue=x.PayedValue

                    }).ToList(); ;


                    vm = new SalaryPayedVM
                    {
                        ContractSchedulings = contractSchedulingDS
                    };
                }
                else
                {
                    ViewBag.Msg = "حدث خطأ اثناء تنفيذ العملية";
                    return View(vm);
                }
                if (contractSchedulingDS.Count() == 0)
                    ViewBag.Msg = "تأكد من اعتماد الرواتب اولا";

                return View(vm);

            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        #endregion


        #region Salary pay
        //صرف الراتب
        public ActionResult SalaryPaid(string id, string payedValue, string remindValue, string dtPayed, Guid? safeId, Guid? bankAccountId)
        {
            Guid Id;
            double payedVal;
            double remindVal;
            DateTime dt;
            if (Guid.TryParse(id, out Id) && double.TryParse(payedValue, out payedVal) && double.TryParse(remindValue, out remindVal) && DateTime.TryParse(dtPayed, out dt))
            {
                if (safeId == null && bankAccountId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة الصرف (خزنة/بنك)" });

                var contractScheduling = db.ContractSchedulings.Where(x => x.Id == Id).FirstOrDefault();
                if (contractScheduling == null)
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    double debit = 0;
                    double credit = 0;
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    /* 
                     حساب الاجور والرواتب
                    حساب ذمم الموظف
                    حساب الايرادات الاخرى 
                    حساب سلف/قروض الموظف
                     حساب النقدية

                     */
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب الرواتب والاجور ليس بحساب فرعى" });

                    if (AccountTreeService.CheckAccountTreeIdHasChilds(contractScheduling.Contract.Employee.Person.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = "حساب ذمم الموظف ليس بحساب فرعى" });

                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب الالايرادات المتنوعه ليس بحساب فرعى" });

                    var EmpAccountTreeId = contractScheduling.Contract.Employee.Person.AccountsTreeCustomerId;
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                    var totalPenalties = contractScheduling.TotalAmountAbsences + contractScheduling.TotalEveryMonthSalaryPenaltie + contractScheduling.TotalSalaryPenaltie;
                    var loanVal = contractScheduling.LoanValue;
                    var totalAdditions = contractScheduling.TotalSalaryAddAllowances + contractScheduling.TotalSalaryAddition;
                    var branchId = contractScheduling.Contract.Employee?.EmployeeBranches.Where(x => !x.IsDeleted).FirstOrDefault()?.BranchId;
                    var month = contractScheduling.MonthYear.Value.Month;
                    var year = contractScheduling.MonthYear.Value.Year;

                    // قيد الاستقطاعات والخصومات 
                    //======================================
                    //من ح/ ذمم الموظف
                    if (totalPenalties > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            //AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
                            AccountsTreeId = EmpAccountTreeId,
                            BranchId = branchId,
                            Debit = totalPenalties,
                            Notes = $"اجمالى مستقطع من شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        debit = debit + totalPenalties;
                    }                    
                    if (loanVal > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            //AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
                            AccountsTreeId = EmpAccountTreeId,
                            BranchId = branchId,
                            Debit = loanVal,
                            Notes = $"استقطاع سلفة من شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        debit = debit + loanVal;
                    }
                    // الى حساب الايرادات المتنوعه
                    if (totalPenalties > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue),
                            BranchId = branchId,
                            Credit = totalPenalties ,
                            Notes = $"اجمالى خصومات شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        credit = credit + totalPenalties ;
                    }

                    // الى حساب السلف/قروض الموظف ان وجدت
                    if (contractScheduling.LoanValue > 0)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(contractScheduling.Contract.Employee.Person.AccountTreeSupplierId))
                            return Json(new { isValid = false, message = "حساب قرض/سلف الموظف ليس بحساب فرعى" });
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = contractScheduling.Contract.Employee.Person.AccountTreeSupplierId,
                            BranchId = branchId,
                            Credit = contractScheduling.LoanValue,
                            Notes = $"سداد سلفه/قرض من راتب شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        credit = credit + contractScheduling.LoanValue;
                    }

                    // قيد الاضافات 
                    //======================================
                    //من ح/ الرواتب والاجور
                    if (totalAdditions > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue),
                            BranchId = branchId,
                            Debit = totalAdditions,
                            Notes = $"اجمالى بدلات واضافى للموظف : {contractScheduling.Contract.Employee.Person.Name} شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        debit = debit + totalAdditions;
                    }
                    // الى حساب ذمم الموظف
                    if (totalAdditions > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = EmpAccountTreeId,
                            BranchId = branchId,
                            Credit = totalAdditions,
                            Notes = $"اجمالى بدلات واضافات شهر : {month} لسنه : {year}",
                            TransactionDate = dt,
                            TransactionId = contractScheduling.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                        });
                        credit = credit + totalAdditions;
                    }

                    // قيد الصرف 
                    //======================================
                    //من ح/ ذمم الموظف المبلغ المدفوع
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = EmpAccountTreeId,
                        BranchId = branchId,
                        Debit = payedVal,
                        Notes = $"صرف راتب شهر : {month} لسنه : {year}",
                        TransactionDate = dt,
                        TransactionId = contractScheduling.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                    });
                    debit = debit + payedVal;
                    // الى حساب الخزينة او البنك
                    Guid? accountTreePayedId;
                    if (safeId != null)
                        accountTreePayedId = db.Safes.FirstOrDefault(x=>x.Id==safeId).AccountsTreeId;
                    else
                        accountTreePayedId = db.BankAccounts.FirstOrDefault(x=>x.Id==bankAccountId).AccountsTreeId;

                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = accountTreePayedId,
                        BranchId = branchId,
                        Credit = payedVal,
                        Notes = $"صرف راتب الموظف : {contractScheduling.Contract.Employee.Person.Name} شهر : {month} لسنه : {year}",
                        TransactionDate = dt,
                        TransactionId = contractScheduling.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                    });
                    credit = credit + payedVal;



                    if (debit == credit)
                    {
                        //تحديث حالة الشهر بان تم صرفه
                        contractScheduling.IsPayed = true;
                        contractScheduling.PayedValue = payedVal;
                        contractScheduling.RemindValue = remindVal;

                        db.Entry(contractScheduling).State = EntityState.Modified;
                        if (contractScheduling.LoanValue > 0)
                        {
                            var loan = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).FirstOrDefault();
                            if (loan != null)
                            {
                                //تحديث شهر السلفه بان تم صرفها 
                                loan.IsPayed = true;
                                db.Entry(loan).State = EntityState.Modified;
                            }
                        }
                        //تحديث  حالة صرف الاضافات والخصومات 
                        var additions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).ToList();
                        foreach (var item in additions)
                        {
                            item.IsPayed = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        var penalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).ToList();
                        foreach (var item in penalties)
                        {
                            item.IsPayed = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //تحديث حالة صرف ايام الغياب
                        var absences = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).ToList();
                        foreach (var item in absences)
                        {
                            item.IsPayed = true;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        {
                            return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                        }
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                    }
                    else
                    {
                        return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                    }

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });



                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم صرف الراتب بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


        }


        #endregion

        //Releases unmanaged resources and optionally releases managed resources.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}