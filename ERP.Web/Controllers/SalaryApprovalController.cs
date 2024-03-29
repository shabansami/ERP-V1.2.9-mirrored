﻿using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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
    [Authorization] // الطريقة القديمة لاعتماد وصرف الرواتب 

    public class SalaryApprovalController : Controller
    {
        // GET: SalaryApproval اعتماد الرواتب قبل صرفها 
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        static List<Scheduling> contractSchedulingDS=new List<Scheduling>();

        #region عرض الموظفين حسب الشهر والسنة 
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            //if (contractSchedulingDS.Count()>0)
            //{
                return Json(new
                {
                    data = contractSchedulingDS.Select(x => new { Id = x.Id,IsApproval=x.IsApproval,IsPayed=x.IsPayed,Employee=x.EmployeeName,Month=x.Month,Year=x.Year, Actions = n, Num = n })
                }, JsonRequestBehavior.AllowGet); ;


            //}
            //return Json(contractSchedulingDS, JsonRequestBehavior.AllowGet); ;

        }
        [HttpPost]
        public ActionResult Index(string dtMonthYear)
        {
            try
            {
                DateTime dtSalary;
                if (DateTime.TryParse(dtMonthYear, out dtSalary))
                {
                        if (TempData["userInfo"] != null)
                            auth = TempData["userInfo"] as VTSAuth;
                        else
                            RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    //تسجيل اجمالى الرواتب فى قيد الاستحقاق تحت حساب الرواتب والاجور 
                    //من ح/الرواتب والاجور 
                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                    {
                        double debit = 0;
                        double credit = 0;
                        // الحصول على حسابات من الاعدادات
                        var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                        //التأكد من عدم وجود حساب تشغيلى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب الرواتب والاجور ليس بحساب تشغيلى" });

                        var month = dtSalary.Month;
                        var year = dtSalary.Year;

                        var contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.MonthYear.Value.Year == year && x.MonthYear.Value.Month == month)
                             .Select(x => new Scheduling
                             {
                                 Id = x.Id,
                                 AccountsTreeId = x.Contract.Employee.Person.AccountsTreeCustomerId,
                                 EmployeeName = x.Contract.Employee.Person.Name,
                                 Month = x.MonthYear.Value.Month,
                                 Year = x.MonthYear.Value.Year,
                                 Salary = x.Contract.Salary,
                                 IsApproval=x.IsApproval,
                                 IsPayed=x.IsPayed
                                
                             }).ToList(); 
                        foreach (var emp in contractSchedulings)
                        {
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(emp.AccountsTreeId))
                                return Json(new { isValid = false, message = $"حساب الموظف : {emp.EmployeeName} ليس بحساب تشغيلى" });
                        }

                        // 

                        //من ح/  الرواتب والاجور 
                        if (contractSchedulings.Count()==0)
                            return Json(new { isValid = false, message ="تأكد من تسجيل عقود الموظفين اولا " });
                           
                        var totalSalaries = contractSchedulings.Sum(x => x.Salary);
                        var schedulingMonth = db.ContractSchedulingsMains.Where(x => !x.IsDeleted && x.MonthNum == month && x.YearNum == year).FirstOrDefault();
                        if (schedulingMonth==null)
                        {
                            using (var context = new VTSaleEntities())
                            {
                                using (DbContextTransaction tran = context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var newSchedulingMonth = new ContractSchedulingsMain
                                        {
                                            MonthNum = month,
                                            YearNum = year,
                                            MonthYear = dtSalary,
                                            TotalSalaries = totalSalaries,
                                            Status = 1
                                        };
                                        context.ContractSchedulingsMains.Add(newSchedulingMonth);
                                        context.SaveChanges(auth.CookieValues.UserId);

                                        var tt = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue);
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue),
                                            Debit = totalSalaries,
                                            Notes = $"رواتب مستحقة شهر : {month} لسنه {year}",
                                            TransactionDate = Utility.GetDateTime(),
                                            TransactionId = newSchedulingMonth.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                                        });
                                        context.SaveChanges(auth.CookieValues.UserId);

                                        debit = debit + totalSalaries;

                                        //تسجيل قيد لذمم الموظفين 
                                        //الى ح/ذمم الموظف 1 .... 
                                        foreach (var contractScheduling in contractSchedulings.ToList())
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = contractScheduling.AccountsTreeId,
                                                Credit = contractScheduling.Salary,
                                                Notes = $"  راتب مستحق للموظف: {contractScheduling.EmployeeName} لشهر : {month} لسنة : {year}",
                                                TransactionDate = Utility.GetDateTime(),
                                                TransactionId = contractScheduling.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                                            });
                                            context.SaveChanges(auth.CookieValues.UserId);
                                            credit = credit + contractScheduling.Salary;
                                        }
                                        //التاكد من توازن المعاملة 
                                        if (debit == credit)
                                        {
                                            //تحديث الجريد 
                                            contractSchedulingDS = contractSchedulings;
                                            tran.Commit();
                                            return Json(new { isValid = true, message = "تم تسجيل استحقاق الرواتب بنجاح" });
                                        }
                                        else
                                            return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });

                                    }
                                    catch (Exception ex)
                                    {
                                        tran.Rollback();
                                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (totalSalaries!= schedulingMonth.TotalSalaries)
                                return Json(new { isValid = false, message = "تم الاعتماد للشهر مسبقا" });
                            contractSchedulingDS = contractSchedulings.ToList();
                            return Json(new { isValid = true, message = "تم تسجيل استحقاق الرواتب بنجاح" });

                        }
                    }
                    else
                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        #endregion

        #region تعديل واستعراض راتب موظف 

        public double AllowanceAbsence(double payedAmount,double totalAbsencePenalty)
        {
            //قيمة البدل لليوم الواحد
            if (totalAbsencePenalty > 0)
                return Math.Round(totalAbsencePenalty * (payedAmount / 30));
            else
                return payedAmount;
        }

        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==id);
                    //الطريقة الاول لاحتساب راتب الموظف لليوم الواحد 
                    var dayCostEmp = Math.Round(contractScheduling.Contract.Salary / 30, 2);
                    //الطريقة الادق لاحتساب راتب الموظف لليوم الواحد 
                    //var dayCostEmp = Math.Round((contractScheduling.Contract.Salary*12) / 365, 2);
                    double salary = contractScheduling.Contract.Salary;
                    //ايام الغياب بخصم بعدد ايام
                    var totalAbsencePenalty = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id && x.IsPenalty).Select(x => x.PenaltyNumber).DefaultIfEmpty(0).Sum();
                    //اجمالى قيمة ايام الغياب بخصم 
                    double totalAllAmountAbsences = Math.Round(dayCostEmp * totalAbsencePenalty, 2);

                    //البدلات
                    var contractAllowances = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId);
                    foreach (var item in contractAllowances)
                    {
                        if (item.IsAffactAbsence)
                            if (item.IsFirstTime)
                                item.AmountPayed = item.AmountPayed - AllowanceAbsence(item.AmountPayed, totalAbsencePenalty);
                    }
                    var contractAllowancesList = contractAllowances.ToList();
                    var totalAllowances = contractAllowancesList.Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    //double totalAllowances = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();

                    //الاضافات
                    var contractSalaryAdditions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id);
                    foreach (var item in contractSalaryAdditions)
                    {
                        if (item.IsAffactAbsence)
                        {
                            if(item.IsFirstTime)
                            item.AmountPayed = item.AmountPayed - AllowanceAbsence(item.AmountPayed, totalAbsencePenalty);
                        }
                    }
                    var contractSalaryAdditionsList=contractSalaryAdditions.ToList();
                    var totalSalaryAdditions = contractSalaryAdditionsList.Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    //double totalSalaryAdditions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                   
                    double totalSalaryAdditionAllowances = totalAllowances+ totalSalaryAdditions;
                    double totalSalaryEveryMonthPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    double totalSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    double totalAllSalaryPenalties = Math.Round(db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum() + db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum() + totalAllAmountAbsences,2);

                    //اجمالى السلف
                    double totalLoans = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.ContractLoan.AmountMonth).DefaultIfEmpty(0).Sum();

                    //صافى الراتب
                    double safy = (salary + totalSalaryAdditionAllowances) - (totalAllSalaryPenalties + totalLoans);
                    SalaryApprovalVM vm = new SalaryApprovalVM();
                    try
                    {
                        vm = new SalaryApprovalVM
                        {
                            ContractSchedulingId = contractScheduling.Id,
                            EmployeeName = contractScheduling.Contract.Employee.Person.Name,
                            DepartmentName = contractScheduling.Contract.Employee.Department.Name,
                            Salary = salary,
                            Safy = safy,
                            Month = contractScheduling.MonthYear.Value.Month,
                            Year = contractScheduling.MonthYear.Value.Year,

                            TotalAllowances = totalAllowances,
                            TotalSalaryAdditions = totalSalaryAdditions,
                            TotalSalaryAdditionAllowances = totalSalaryAdditionAllowances,
                            TotalSalaryEveryMonthPenalties = totalSalaryEveryMonthPenalties,
                            TotalSalaryPenalties = totalSalaryPenalties,
                            TotalAllSalaryPenalties = totalAllSalaryPenalties,

                            //ايام الغياب المسموح بها
                            TotalAbsenceAllowed = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id && !x.IsPenalty).Select(x => x.AbsenceDayNumber).DefaultIfEmpty(0).Sum(),
                            //ايام الغياب بخصم بعدد ايام
                            TotalAbsencePenalty = totalAbsencePenalty,
                            //اجمالى قيمة ايام الغياب بخصم 
                            TotalAllAmountAbsences = totalAllAmountAbsences,
                            //اجمالى السلف
                            TotalLoans = totalLoans,
                            //تكلفة اجر اليوم للموظف
                            DayCost = dayCostEmp,

                            //البدلات
                            ContractAllowances = contractAllowancesList
                  .Select(c => new ContractSalaryAdditionsDT
                  {
                      Id = c.Id,
                      SalaryAdditionAmount = c.Amount,
                      SalaryAdditionAmountPayed = c.AmountPayed,
                      SalaryAdditionName = c.SalaryAddition.Name,
                      SalaryAdditionTypeName = c.SalaryAddition.SalaryAdditionType.Name,
                      SalaryAdditionNotes = c.Notes,
                      IsAffactAbsence = c.IsAffactAbsence
                  }).ToList(),
                            //الاضافات 
                            ContractSalaryAdditions = contractSalaryAdditionsList
                  .Select(c => new ContractSalaryAdditionsDT
                  {
                      Id = c.Id,
                      SalaryAdditionAmount = c.Amount,
                      SalaryAdditionAmountPayed = c.AmountPayed,
                      SalaryAdditionName = c.SalaryAddition.Name,
                      SalaryAdditionTypeName = c.SalaryAddition.SalaryAdditionType.Name,
                      SalaryAdditionNotes = c.Notes,
                      IsAffactAbsence=c.IsAffactAbsence
                  }).ToList(),
                            //الخصومات كل شهر  
                            SalaryEveryMonthPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId)
                  .Select(c => new ContractSalaryPenaltyDT
                  {
                      Id = c.Id,
                      SalaryPenaltyAmount = c.Amount,
                      SalaryPenaltyAmountPayed = c.AmountPayed,
                      SalaryPenaltyName = c.SalaryPenalty.Name,
                      SalaryPenaltyTypeName = c.SalaryPenalty.SalaryPenaltyType.Name,
                      SalaryPenaltyNotes = c.Notes
                  }).ToList(),
                            //الخصومات حسب كل شهر  
                            ContractSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSalaryPenaltyDT
                  {
                      Id = c.Id,
                      SalaryPenaltyAmount = c.Amount,
                      SalaryPenaltyAmountPayed = c.AmountPayed,
                      SalaryPenaltyName = c.SalaryPenalty.Name,
                      SalaryPenaltyTypeName = c.SalaryPenalty.SalaryPenaltyType.Name,
                      SalaryPenaltyNotes = c.Notes,
                  }).ToList(),
                            //الغياب المسموح به
                            ContractSchedulingAllowAbsencs = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && !x.IsPenalty && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSchedulingAbsenceDT
                  {
                      Id = c.Id,
                      FromDate = c.FromDate.Value.Year.ToString() + "-" + c.FromDate.Value.Month.ToString() + "-" + c.FromDate.Value.Day.ToString(),
                      ToDate = c.ToDate.Value.Year.ToString() + "-" + c.ToDate.Value.Month.ToString() + "-" + c.ToDate.Value.Day.ToString(),
                      AbsenceDayNumber = c.AbsenceDayNumber,
                      VacationTypeName = c.VacationType.Name,
                  }).ToList(),
                            //الغياب بخصم
                            ContractSchedulingPenaltyAbsencs = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.IsPenalty && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSchedulingAbsenceDT
                  {
                      Id = c.Id,
                      FromDate = c.FromDate.Value.Year.ToString() + "-" + c.FromDate.Value.Month.ToString() + "-" + c.FromDate.Value.Day.ToString(),
                      ToDate = c.ToDate.Value.Year.ToString() + "-" + c.ToDate.Value.Month.ToString() + "-" + c.ToDate.Value.Day.ToString(),
                      AbsenceDayNumber = c.PenaltyNumber,
                  }).ToList(),
                            //السلف 
                            ContractLoans = contractScheduling.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractLoan.IsApproval && x.ContractSchedulingId == contractScheduling.Id)
                 .Select(c => new ContractLoansDT
                 {
                     Id = c.Id,
                     ContractSchedulingId = c.ContractSchedulingId,
                     Amount = c.ContractLoan.AmountMonth,
                     TotalAmount = c.ContractLoan.Amount,
                     LoanDate = c.ContractLoan.LoanDate.Value.ToString("yyyy-MM-dd"),
                     LoanNotes = c.ContractLoan.Notes
                 }).ToList(),
                        };

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");


        }
        [HttpPost]
        public JsonResult CreateEdit(SalaryApprovalVM vm)
        {
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                // البدلات والاضافات
                List<EditDatatable> allowanceAdditions = new List<EditDatatable>();
                if (vm.currentAllowances != null)
                    allowanceAdditions = JsonConvert.DeserializeObject<List<EditDatatable>>(vm.currentAllowances);
                if (vm.currentSalaryAdditions != null)
                    allowanceAdditions.AddRange(JsonConvert.DeserializeObject<List<EditDatatable>>(vm.currentSalaryAdditions));
                // الاستقطاعات والخصومات
                List<EditDatatable> penalties = new List<EditDatatable>();
                if (vm.currentSalaryEveryMonthPenalties != null)
                    penalties = JsonConvert.DeserializeObject<List<EditDatatable>>(vm.currentSalaryEveryMonthPenalties);
                if (vm.currentSalaryPenalties != null)
                    penalties.AddRange(JsonConvert.DeserializeObject<List<EditDatatable>>(vm.currentSalaryPenalties));

                //تحديث البدلات والاضافات
                foreach (var item in allowanceAdditions)
                {
                    var addition = db.ContractSalaryAdditions.FirstOrDefault(x=>x.Id==item.Id);
                    addition.AmountPayed = item.AmountPayed;
                    addition.IsFirstTime = false;
                    db.Entry(addition).State = EntityState.Modified;
                }        
                //تحديث الاستقطاعات والخصومات
                foreach (var item in penalties)
                {
                    var penalty = db.ContractSalaryPenalties.FirstOrDefault(x=>x.Id==item.Id);
                    penalty.AmountPayed = item.AmountPayed;
                    db.Entry(penalty).State = EntityState.Modified;
                }
            if (allowanceAdditions.Count()==0&& penalties.Count()==0)
                return Json(new { isValid = true, message = "تم الحفظ بنجاح" });

            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم الحفظ بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }

        #endregion

        #region Approval Salary
        //اعتماد الراتب
        public ActionResult ApprovalSalary(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==Id);
                //الطريقة الاول لاحتساب راتب الموظف لليوم الواحد 
                var dayCostEmp = Math.Round(contractScheduling.Contract.Salary / 30, 2);
                //الطريقة الادق لاحتساب راتب الموظف لليوم الواحد 
                //var dayCostEmp = Math.Round((contractScheduling.Contract.Salary * 12) / 365, 2);
                double salary = contractScheduling.Contract.Salary;
                //ايام الغياب بخصم بعدد ايام
                var totalAbsencePenalty = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id && x.IsPenalty).Select(x => x.PenaltyNumber).DefaultIfEmpty(0).Sum();

                //البدلات
                var contractAllowances = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId);
                foreach (var item in contractAllowances)
                {
                    if (item.IsAffactAbsence)
                        if (item.IsFirstTime)
                            item.AmountPayed = item.AmountPayed - AllowanceAbsence(item.AmountPayed, totalAbsencePenalty);
                }
                var contractAllowancesList = contractAllowances.ToList();
                var totalAllowances = contractAllowancesList.Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                //double totalAllowances = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();

                //الاضافات
                var contractSalaryAdditions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id);
                foreach (var item in contractSalaryAdditions)
                {
                    if (item.IsAffactAbsence)
                    {
                        if (item.IsFirstTime)
                            item.AmountPayed = item.AmountPayed - AllowanceAbsence(item.AmountPayed, totalAbsencePenalty);
                    }
                }
                var contractSalaryAdditionsList = contractSalaryAdditions.ToList();
                var totalSalaryAdditions = contractSalaryAdditionsList.Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                //double totalSalaryAdditions = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();

                double totalSalaryAdditionAllowances = totalAllowances + totalSalaryAdditions; ;
                double totalSalaryEveryMonthPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                double totalSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                double totalAllSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum() + db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();

                //اجمالى قيمة ايام الغياب بخصم 
                double totalAllAmountAbsences = Math.Round(dayCostEmp * totalAbsencePenalty, 2);
                //اجمالى السلف
                double totalLoans = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.ContractLoan.AmountMonth).DefaultIfEmpty(0).Sum();
                //صافى الراتب
                double safy = (salary + totalSalaryAdditionAllowances) - (totalAllSalaryPenalties + totalLoans + totalAllAmountAbsences);

                contractScheduling.TotalSalaryAddAllowances = totalAllowances;
                contractScheduling.TotalSalaryAddition = totalSalaryAdditions;
                contractScheduling.TotalEveryMonthSalaryPenaltie = totalSalaryEveryMonthPenalties;
                contractScheduling.TotalSalaryPenaltie = totalSalaryPenalties;
                contractScheduling.TotalAmountAbsences = totalAllAmountAbsences;
                contractScheduling.IsApproval = true;
                contractScheduling.Safy = safy;
                db.Entry(contractScheduling).State = EntityState.Modified;

                //تحديث البدلات والاضافات بالقيم المدفوعه كاول مرة دفع 
                var list = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId|| x.ContractSchedulingId == contractScheduling.Id).ToList();
                foreach (var item in list)
                {
                    item.IsFirstTime = true;
                    item.AmountPayed = item.Amount;
                    db.Entry(item).State = EntityState.Modified;
                }
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد الراتب بنجاح" });
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