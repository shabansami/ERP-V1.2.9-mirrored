using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ContractLoansController : Controller
    {
        // GET: ContractLoans
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        
        public ActionResult Index()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.ContractLoans.Where(x => !x.IsDeleted);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = list.Where(x => DbFunctions.TruncateTime(x.LoanDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.LoanDate) <= dtTo.Date);

                return Json(new
                {
                    data = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, ConNum = x.ContractScheduling.ContractId, EmployeeName = x.ContractScheduling.Contract.Employee.Person.Name, Salary = x.ContractScheduling.Contract.Salary, Notes = x.Notes, Amount = x.Amount, NumberMonths = x.NumberMonths, ContractSchedulingDate = x.ContractScheduling.Name/*x.ContractScheduling.MonthYear.ToString().Substring(0,7)*/, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, ConNum = x.ContractScheduling.ContractId, EmployeeName = x.ContractScheduling.Contract.Employee.Person.Name, Salary = x.ContractScheduling.Contract.Salary, Notes = x.Notes, Amount = x.Amount, NumberMonths = x.NumberMonths, ContractSchedulingDate = x.ContractScheduling.Name/*x.ContractScheduling.MonthYear.ToString().Substring(0,7)*/, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.ContractLoans.FirstOrDefault(x => x.Id == id);
                    //var departId = model.ContractScheduling.Contract.Employee.DepartmentId;
                    Guid? departmentId = model.ContractScheduling.Contract.Employee.DepartmentId;
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(departmentId), "Id", "Name", model.ContractScheduling.Contract.EmployeeId);
                    ViewBag.ContractSchedulingId = new SelectList(EmployeeService.GetContractSchedulingEmployee(model.ContractScheduling.Contract.EmployeeId.ToString()), "Id", "Name", model.ContractSchedulingId);

                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == model.BranchId), "Id", "Name", model.SafeId);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", model.BankAccountId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.BranchId = new SelectList(branches, "Id", "Name");
                ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");

                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
                ViewBag.LastRow = db.ContractLoans.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new ContractLoan() { LoanDate = Utility.GetDateTime() });
                //}
            }
        }
        bool IsMonthNumberValid(int numberMonths, Guid? contractId)
        {
            var contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contractId && !x.IsPayed).Count();
            if (numberMonths > contractSchedulings)
                return false;
            else
                return true;
        }
        [HttpPost]
        public JsonResult CreateEdit(ContractLoan vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ContractSchedulingId == null || vm.NumberMonths == 0 || vm.Amount == 0 || vm.AmountMonth == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.BankAccountId == null && vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة سداد السلفه/القرض (بنكى-خزنة) بشكل صحيح" });

                var isInsert = false;

                //التأكد من ان عدد الاقساط اقل من او يساوى عدد الاشهر المتبقية للموظف
                var contract = db.ContractSchedulings.FirstOrDefault(x => x.Id == vm.ContractSchedulingId).Contract;
                //الاشهر
                if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
                {
                    if (!IsMonthNumberValid(vm.NumberMonths, contract.Id))
                        return Json(new { isValid = false, message = "عدد الاقساط اكبر من عدد الاشهر المتبقية " });
                }
                //الاسابيع
                if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                {
                    if (!IsMonthNumberValid(vm.NumberMonths, contract.Id))
                        return Json(new { isValid = false, message = "عدد الاقساط اكبر من عدد الاسابيع المتبقية " });
                }

                if (vm.Id != Guid.Empty)
                {
                    var model = db.ContractLoans.FirstOrDefault(x => x.Id == vm.Id);
                    model.ContractSchedulingId = vm.ContractSchedulingId;
                    model.Amount = vm.Amount;
                    model.AmountMonth = vm.AmountMonth;
                    model.NumberMonths = vm.NumberMonths;
                    model.Notes = vm.Notes;
                    model.BranchId = vm.BranchId;
                    model.BankAccountId = vm.BankAccountId;
                    model.SafeId = vm.SafeId;
                    model.LoanDate = vm.LoanDate;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    db.ContractLoans.Add(new ContractLoan
                    {
                        ContractSchedulingId = vm.ContractSchedulingId,
                        Amount = vm.Amount,
                        AmountMonth = vm.AmountMonth,
                        NumberMonths = vm.NumberMonths,
                        Notes = vm.Notes,
                        BranchId = vm.BranchId,
                        BankAccountId = vm.BankAccountId,
                        SafeId = vm.SafeId,
                        LoanDate = vm.LoanDate

                    });
                }
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    if (isInsert)
                        return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
                    else
                        return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });

                }
                else
                    return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ContractLoans.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        #region الاعتماد النهائى للسلفة  
        [HttpPost]
        public ActionResult ApprovalContractLoan(string id)
        {

            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                using (var context = new VTSaleEntities())
                {
                    using (DbContextTransaction transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var model = context.ContractLoans.Where(x => x.Id == Id).FirstOrDefault();
                            if (model != null)
                            {
                                //التأكد من عدم تكرار اعتماد القيد
                                if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.Loans))
                                    return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                                //اعتماد  السلفة 
                                //التأكد من ان عدد الاقساط اقل من او يساوى عدد الاشهر المتبقية للموظف
                                var contract = db.ContractSchedulings.FirstOrDefault(x => x.Id == model.ContractSchedulingId).Contract;
                                //الاشهر
                                if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
                                {
                                    if (!IsMonthNumberValid(model.NumberMonths, contract.Id))
                                        return Json(new { isValid = false, message = "عدد الاقساط اكبر من عدد الاشهر المتبقية " });
                                }
                                //الاسابيع
                                if (contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                                {
                                    if (!IsMonthNumberValid(model.NumberMonths, contract.Id))
                                        return Json(new { isValid = false, message = "عدد الاقساط اكبر من عدد الاسابيع المتبقية " });
                                }

                                //===================
                                List<GeneralSetting> generalSetting;
                                Employee employee;
                                //التأكد من عدم وجود حساب فرعى من الحسابات المستخدمة
                                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                {
                                    // الحصول على حسابات من الاعدادات
                                    generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                                    employee = context.Employees.Where(x => x.Id == model.ContractScheduling.Contract.EmployeeId).FirstOrDefault();
                                    ////التأكد من عدم وجود حساب فرعى من الحساب
                                    //if (AccountTreeService.CheckAccountTreeIdHasChilds(int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeLoans).FirstOrDefault().SValue)))
                                    //    return Json(new { isValid = false, message = "حساب السلف/القروض ليس بحساب فرعى" });

                                    //accountLoanTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeLoans).FirstOrDefault().SValue);
                                    //التأكد من انشاء حساب سلف للموظف 
                                    AccountsTree accountTreeLoanEmp;
                                    Guid? accountTreeLoanEmpId;
                                    if (employee.Person.AccountTreeSupplierId == null)
                                    {
                                        //تحديث بيانات الموظف فى جدول بيرسون
                                        var person = context.Persons.FirstOrDefault(x => x.Id == employee.PersonId);
                                        //add as loan employee in account tree
                                        accountTreeLoanEmp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeLoans, "قروض وسلف الموظف : " + employee.Person.Name, AccountTreeSelectorTypesCl.Loan);
                                        context.AccountsTrees.Add(accountTreeLoanEmp);
                                        context.SaveChanges(auth.CookieValues.UserId);
                                        accountTreeLoanEmpId = accountTreeLoanEmp.Id;

                                        person.AccountsTreeSupplier = accountTreeLoanEmp;
                                        context.Entry(person).State = EntityState.Modified;
                                        context.SaveChanges(auth.CookieValues.UserId);

                                    }
                                    else
                                    {
                                        //التأكد من عدم وجود حساب فرعى من حساب سلف الموظف
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(employee.Person.AccountTreeSupplierId))
                                            return Json(new { isValid = false, message = "حساب سلف الموظف ليس بحساب فرعى" });
                                        accountTreeLoanEmpId = employee.Person.AccountTreeSupplierId;
                                    }


                                    //جدولة اشهر العقد
                                    bool start = false; // بداية الشهر المحدد من شاشة اضافة سلفة مثلا السلفة هاتبدأ من شهر 3
                                    int yearMonth = 1;// رقم الشهر/الاسبوع 
                                    var contractSchedulings = context.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == model.ContractScheduling.ContractId && !x.IsPayed).OrderBy(x => x.Id).OrderBy(x=>x.MonthYear).ToList();
                                    foreach (var month in contractSchedulings)
                                    {
                                        if (month.Id == model.ContractSchedulingId || start)
                                        {
                                            if (yearMonth <= model.NumberMonths)
                                            {
                                                start = true;
                                                context.ContractLoanSchedulings.Add(new ContractLoanScheduling
                                                {
                                                    ContractLoanId = model.Id,
                                                    ContractSchedulingId = month.Id,
                                                });
                                                context.SaveChanges(auth.CookieValues.UserId);
                                                yearMonth++;
                                            }
                                            else
                                                break;
                                        }
                                    }


                                    // من ح/  قروض الموظف
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = accountTreeLoanEmpId,
                                        BranchId = model.BranchId,
                                        Debit = model.Amount,
                                        Notes = $"سلفة للموظف : {employee.Person.Name}",
                                        TransactionDate = model.LoanDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.Loans
                                    });

                                    //الى ح/ النقدية
                                    context.GeneralDailies.Add(new GeneralDaily
                                    {
                                        AccountsTreeId = model.SafeId != null ? model.Safe.AccountsTreeId : model.BankAccount.AccountsTreeId,
                                        BranchId = model.BranchId,
                                        Credit = model.Amount,
                                        Notes = $"سلفة للموظف : {employee.Person.Name}",
                                        TransactionDate = model.LoanDate,
                                        TransactionId = model.Id,
                                        TransactionTypeId = (int)TransactionsTypesCl.Loans
                                    });

                                    //اعتماد السلفه
                                    model.IsApproval = true;
                                    context.Entry(model).State = EntityState.Modified;
                                    context.SaveChanges(auth.CookieValues.UserId);

                                    transaction.Commit();
                                    return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                                }
                                else
                                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });



                                //===============
                            }
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                        }
                    }


                }
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        }




        #endregion

        #region فك اعتماد السلفة 
        [HttpPost]
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ContractLoans.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //التأكد من ان السلفة لم يتم اعتمادها وصرفها فى الرواتب 
                    var loanSchedulingExsits = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractLoanId == model.Id && x.IsPayed).Any();
                    if (loanSchedulingExsits)
                        return Json(new { isValid = false, message = "لا يمكن فك الاعتماد بسبب صرف السلفة للموظف فى بعض الاشهر بالفعل" });

                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف اى جدولة اشهر للسلفه 
                    var loanSchedulings = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractLoanId == model.Id).ToList();
                    foreach (var item in loanSchedulings)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Loans).ToList();
                    foreach (var item in generalDailies)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم فك الاعتماد بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
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