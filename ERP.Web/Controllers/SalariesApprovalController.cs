﻿using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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

    public class SalariesApprovalController : Controller
    {
        // GET: SalariesApproval
        VTSaleEntities db ;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        SalaryApprovalVM vm;
        List<ProductionOrderEmployee> productionOrderEmployee;

        public SalariesApprovalController()
        {
            db = new VTSaleEntities();
            productionOrderEmployee = new List<ProductionOrderEmployee>();
            vm = new SalaryApprovalVM();
        }

        #region عرض الموظفين حسب الشهر والسنة 
        public ActionResult Index()
        {
            ViewBag.ContractSalaryTypId = new SelectList(db.ContractSalaryTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.DepartmntId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string ContractSalaryTypId, string DepartmntId, string dt)
        {
            int? n = null;
            int salaryTypeId;
            Guid DepartmentId;
            DateTime dateSearch;
            List<Scheduling> contractSchedulingDS = new List<Scheduling>();
            if (int.TryParse(ContractSalaryTypId, out salaryTypeId) && DateTime.TryParse(dt, out dateSearch))
            {
                IQueryable<ContractScheduling> contractSchedulings = null;
                //اليومى
                if (salaryTypeId == (int)ContractSalaryTypeCl.Daily)
                    contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId && x.MonthYear == dateSearch);
                else if (salaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                    contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId && dateSearch >= x.MonthYear && dateSearch <= x.ToDate);
                else if (salaryTypeId == (int)ContractSalaryTypeCl.Monthly)
                    contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId && x.MonthYear.Value.Month == dateSearch.Month);
                else if(salaryTypeId == (int)ContractSalaryTypeCl.Production)
                    contractSchedulings = db.ContractSchedulings.Where(x => !x.IsDeleted && x.Contract.IsActive && x.Contract.ContractSalaryTypeId == salaryTypeId);

                if (Guid.TryParse(DepartmntId, out DepartmentId))
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
                    IsPayed = x.IsPayed

                }).ToList(); ;

            }
            return Json(new
            {
                data = contractSchedulingDS.Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsPayed = x.IsPayed, Employee = x.EmployeeName, SchedulingName = x.SchedulingName, Actions = n, ActionStatus = n, Num = n })
            }, JsonRequestBehavior.AllowGet); ;


            //}
            //return Json(contractSchedulingDS, JsonRequestBehavior.AllowGet); ;

        }
        #endregion

        #region تعديل واستعراض راتب موظف 

        public double AllowanceAbsence(double payedAmount, double totalAbsencePenalty)
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
                    var contractScheduling = db.ContractSchedulings.Where(x => x.Id == id).FirstOrDefault();
                    var employee = contractScheduling.Contract.Employee;
                    GetSalary(contractScheduling, employee);
                    double salary = vm.IsEmployeeProduction? productionOrderEmployee.Sum(x=>x.ProductionOrderHours*x.HourlyWage): contractScheduling.Contract.Salary;
                    vm.ProductionAmount = productionOrderEmployee.Sum(x => x.ProductionOrderHours * x.HourlyWage);
                    //الطريقة الاول لاحتساب راتب الموظف لليوم الواحد 
                    var dayCostEmp = Math.Round(salary / 30, 2);
                    //الطريقة الادق لاحتساب راتب الموظف لليوم الواحد 
                    //var dayCostEmp = Math.Round((contractScheduling.Contract.Salary*12) / 365, 2);
                    //ايام الغياب بخصم بعدد ايام
                    var totalAbsencePenalty = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id && x.IsPenalty).Select(x => x.PenaltyNumber).DefaultIfEmpty(0).Sum();
                    //اجمالى قيمة ايام الغياب بخصم 
                    double totalAllAmountAbsences = Math.Round(dayCostEmp * totalAbsencePenalty, 2);

                    //احتساب ايام الغياب تلقائى من ماكينة البصمة 
                    // فترات عمل الموظف 
                    //var shifts = db.Shifts.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id);
                    //TimeSpan? startTime; //بداية الدوام
                    //TimeSpan? endTime;//نهاية الدوام
                    //TimeSpan? delayFrom;//التأخير يتم من وقت
                    //TimeSpan? absenceOf;//يتم احتساب الموظف غائب من وقت 

                    //if (shifts.Count() > 0)//الموظف له فترة مخصصة مختلفة عن الادارة التابع لها 
                    //{
                    //    var shift = shifts.FirstOrDefault();
                    //    startTime = shift.WorkingPeriod.StartTime;
                    //    endTime = shift.WorkingPeriod.EndTime;
                    //    delayFrom = shift.WorkingPeriod.DelayFrom;
                    //    absenceOf = shift.WorkingPeriod.AbsenceOf;
                    //}
                    //else
                    //{
                    //    shifts = shifts.Where(x => x.DepartmentId == employee.DepartmentId);
                    //    if (shifts.Count() > 0)
                    //    {
                    //        var shift = shifts.FirstOrDefault();
                    //        startTime = shift.WorkingPeriod.StartTime;
                    //        endTime = shift.WorkingPeriod.EndTime;
                    //        delayFrom = shift.WorkingPeriod.DelayFrom;
                    //        absenceOf = shift.WorkingPeriod.AbsenceOf;
                    //    }
                    //    else
                    //    {
                    //        TempData["errorMsg"] = "لابد من تحديد نظام الوردية للموظف اولا";
                    //        return RedirectToAction("Index");
                    //    }
                    //}

                    ////الحضور والانصراف للموظف
                    //var attendanceLeavings = db.ContractAttendanceLeavings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id);
                    ////عدد ساعات الفترة
                    //var hours = endTime - startTime;
                    ////نظام التأخير والغياب ان وجدت
                    //var delayAbsenceSystems = db.DelayAbsenceSystems.Where(x => !x.IsDeleted);
                    //var delaies = delayAbsenceSystems.Where(x => x.IsDelay == true).OrderBy(x => x.DelayMinNumber);
                    //var absence = delayAbsenceSystems.Where(x => x.IsDelay == false);

                    ////الايام التى تم الحضور بها كاملا
                    //var delay = 0;
                    //if (delaies.Count() > 0)
                    //{
                    //    delay = delaies.FirstOrDefault().DelayMinNumber - 1;
                    //}
                    //TimeSpan timeSpan = new TimeSpan(0, delay, 0);
                    //foreach (var day in attendanceLeavings)
                    //{

                    //}
                    //var dayFull = attendanceLeavings.Where(x => x.AttendanceTime <= x.AttendanceTime.Value.Add(timeSpan));

                    ////الايام التأخير

                    ////ايام الغياب

                    ////ايام الاجازات فى الفترة المحدده للشهرى/الاسبوعى


                    ////عدد ساعات الافرتايم 


                    ////الوصول الى نظام تعريف الغياب 
                    //var absenceSystem = db.DelayAbsenceSystems.Where(x => !x.IsDeleted && x.IsDelay == false).FirstOrDefault();
                    //if (absenceSystem != null)
                    //{
                    //    //نظام حتساب يوم الغياب تلقائيا
                    //    var absenceDayPenalty = absenceSystem.AbsenceDayPenalty;
                    //}
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

                    double totalSalaryAdditionAllowances = totalAllowances + totalSalaryAdditions;
                    double totalSalaryEveryMonthPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    double totalSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum();
                    double totalAllSalaryPenalties = Math.Round(db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum() + db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.AmountPayed).DefaultIfEmpty(0).Sum() + totalAllAmountAbsences, 2);

                    //اجمالى السلف
                    double totalLoans = db.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id).Select(x => x.ContractLoan.AmountMonth).DefaultIfEmpty(0).Sum();

                    //صافى الراتب
                    double safy = (salary + totalSalaryAdditionAllowances+ vm.ProductionAmount) - (totalAllSalaryPenalties + totalLoans);
                    try
                    {
                            vm.ContractSchedulingId = contractScheduling.Id;
                            vm.EmployeeName = employee.Person.Name;
                            vm.DepartmentName = contractScheduling.Contract.Employee.Department.Name;
                            vm.Salary = salary;
                            vm.Safy = safy;
                            vm.SchedulingName = contractScheduling.Name;
                            vm.Month = contractScheduling.MonthYear!=null? (int)contractScheduling?.MonthYear.Value.Month :0;
                            vm.Year = contractScheduling.MonthYear!=null? (int)contractScheduling?.MonthYear.Value.Year :0;
                            //Year = (int)contractScheduling?.MonthYear.Value.Year;

                            vm.TotalAllowances = totalAllowances;
                            vm.TotalSalaryAdditions = totalSalaryAdditions;
                            vm.TotalSalaryAdditionAllowances = totalSalaryAdditionAllowances;
                            vm.TotalSalaryEveryMonthPenalties = totalSalaryEveryMonthPenalties;
                            vm.TotalSalaryPenalties = totalSalaryPenalties;
                            vm.TotalAllSalaryPenalties = totalAllSalaryPenalties;

                            //ايام الغياب المسموح بها
                            vm.TotalAbsenceAllowed = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id && !x.IsPenalty).Select(x => x.AbsenceDayNumber).DefaultIfEmpty(0).Sum();
                            //ايام الغياب بخصم بعدد ايام
                            vm.TotalAbsencePenalty = totalAbsencePenalty;
                            //اجمالى قيمة ايام الغياب بخصم 
                            vm.TotalAllAmountAbsences = totalAllAmountAbsences;
                            //اجمالى السلف
                            vm.TotalLoans = totalLoans;
                            //تكلفة اجر اليوم للموظف
                            vm.DayCost = dayCostEmp;

                        //البدلات
                        vm.ContractAllowances = contractAllowancesList
              .Select(c => new ContractSalaryAdditionsDT
              {
                  Id = c.Id,
                  SalaryAdditionAmount = c.Amount,
                  SalaryAdditionAmountPayed = c.AmountPayed,
                  SalaryAdditionName = c.SalaryAddition.Name,
                  SalaryAdditionTypeName = c.SalaryAddition.SalaryAdditionType.Name,
                  SalaryAdditionNotes = c.Notes,
                  IsAffactAbsence = c.IsAffactAbsence
              }).ToList();
                            //الاضافات 
                            vm.ContractSalaryAdditions = contractSalaryAdditionsList
                  .Select(c => new ContractSalaryAdditionsDT
                  {
                      Id = c.Id,
                      SalaryAdditionAmount = c.Amount,
                      SalaryAdditionAmountPayed = c.AmountPayed,
                      SalaryAdditionName = c.SalaryAddition.Name,
                      SalaryAdditionTypeName = c.SalaryAddition.SalaryAdditionType.Name,
                      SalaryAdditionNotes = c.Notes,
                      IsAffactAbsence = c.IsAffactAbsence
                  }).ToList();
                            //الخصومات كل شهر  
                           vm.SalaryEveryMonthPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId)
                  .Select(c => new ContractSalaryPenaltyDT
                  {
                      Id = c.Id,
                      SalaryPenaltyAmount = c.Amount,
                      SalaryPenaltyAmountPayed = c.AmountPayed,
                      SalaryPenaltyName = c.SalaryPenalty.Name,
                      SalaryPenaltyTypeName = c.SalaryPenalty.SalaryPenaltyType.Name,
                      SalaryPenaltyNotes = c.Notes
                  }).ToList();
                            //الخصومات حسب كل شهر  
                            vm.ContractSalaryPenalties = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSalaryPenaltyDT
                  {
                      Id = c.Id,
                      SalaryPenaltyAmount = c.Amount,
                      SalaryPenaltyAmountPayed = c.AmountPayed,
                      SalaryPenaltyName = c.SalaryPenalty.Name,
                      SalaryPenaltyTypeName = c.SalaryPenalty.SalaryPenaltyType.Name,
                      SalaryPenaltyNotes = c.Notes,
                  }).ToList();
                            //الغياب المسموح به
                            vm.ContractSchedulingAllowAbsencs = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && !x.IsPenalty && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSchedulingAbsenceDT
                  {
                      Id = c.Id,
                      FromDate = c.FromDate.Value.Year.ToString() + "-" + c.FromDate.Value.Month.ToString() + "-" + c.FromDate.Value.Day.ToString(),
                      ToDate = c.ToDate.Value.Year.ToString() + "-" + c.ToDate.Value.Month.ToString() + "-" + c.ToDate.Value.Day.ToString(),
                      AbsenceDayNumber = c.AbsenceDayNumber,
                      VacationTypeName = c.VacationType.Name,
                  }).ToList();
                            //الغياب بخصم
                           vm.ContractSchedulingPenaltyAbsencs = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.IsPenalty && x.ContractSchedulingId == contractScheduling.Id)
                  .Select(c => new ContractSchedulingAbsenceDT
                  {
                      Id = c.Id,
                      FromDate = c.FromDate.Value.Year.ToString() + "-" + c.FromDate.Value.Month.ToString() + "-" + c.FromDate.Value.Day.ToString(),
                      ToDate = c.ToDate.Value.Year.ToString() + "-" + c.ToDate.Value.Month.ToString() + "-" + c.ToDate.Value.Day.ToString(),
                      AbsenceDayNumber = c.PenaltyNumber,
                  }).ToList();
                            //السلف 
                            vm.ContractLoans = contractScheduling.ContractLoanSchedulings.Where(x => !x.IsDeleted && x.ContractLoan.IsApproval && x.ContractSchedulingId == contractScheduling.Id)
                 .Select(c => new ContractLoansDT
                 {
                     Id = c.Id,
                     ContractSchedulingId = c.ContractSchedulingId,
                     Amount = c.ContractLoan.AmountMonth,
                     TotalAmount = c.ContractLoan.Amount,
                     LoanDate = c.ContractLoan.LoanDate.Value.ToString("yyyy-MM-dd"),
                     LoanNotes = c.ContractLoan.Notes
                 }).ToList();
                        

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
                var addition = db.ContractSalaryAdditions.FirstOrDefault(x => x.Id == item.Id);
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
                var contractScheduling = db.ContractSchedulings.FirstOrDefault(x => x.Id == Id);
                var employee = contractScheduling.Contract.Employee;
                GetSalary(contractScheduling, employee);
                vm.ProductionAmount = productionOrderEmployee.Sum(x => x.ProductionOrderHours * x.HourlyWage);

                //الطريقة الاول لاحتساب راتب الموظف لليوم الواحد 
                var dayCostEmp = Math.Round(contractScheduling.Contract.Salary / 30, 2);
                //الطريقة الادق لاحتساب راتب الموظف لليوم الواحد 
                //var dayCostEmp = Math.Round((contractScheduling.Contract.Salary * 12) / 365, 2);
                double salary = vm.IsEmployeeProduction ? productionOrderEmployee.Sum(x => x.ProductionOrderHours * x.HourlyWage) : contractScheduling.Contract.Salary;
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
                double safy = (salary + totalSalaryAdditionAllowances+vm.ProductionAmount) - (totalAllSalaryPenalties + totalLoans + totalAllAmountAbsences);

                contractScheduling.TotalSalaryAddAllowances = totalAllowances;
                contractScheduling.TotalSalaryAddition = totalSalaryAdditions;
                contractScheduling.TotalEveryMonthSalaryPenaltie = totalSalaryEveryMonthPenalties;
                contractScheduling.TotalSalaryPenaltie = totalSalaryPenalties;
                contractScheduling.TotalAmountAbsences = totalAllAmountAbsences;
                contractScheduling.IsApproval = true;
                contractScheduling.Safy = safy;
                contractScheduling.LoanValue = totalLoans;
                db.Entry(contractScheduling).State = EntityState.Modified;

                //تحديث البدلات والاضافات بالقيم المدفوعه كاول مرة دفع 
                var list = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.IsEveryMonth && x.ContractId == contractScheduling.ContractId || x.ContractSchedulingId == contractScheduling.Id).ToList();
                foreach (var item in list)
                {
                    item.IsFirstTime = true;
                    item.AmountPayed = item.Amount;
                    db.Entry(item).State = EntityState.Modified;
                }

                //=======================================
                //تسجيل  قيد الاستحقاق تحت حساب الرواتب والاجور 
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

                    if (AccountTreeService.CheckAccountTreeIdHasChilds(employee.Person.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = $"حساب الموظف : {employee.Person.Name} ليس بحساب تشغيلى" });

                    // 
                    var defaultBranchId = contractScheduling.Contract?.Employee?.EmployeeBranches.Where(x => !x.IsDeleted).FirstOrDefault()?.BranchId;

                    //من ح/  الرواتب والاجور 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue),
                        Debit = salary,
                        Notes = $"راتب مستحق عن : {contractScheduling.Name}",
                        TransactionDate = Utility.GetDateTime(),
                        TransactionId = contractScheduling.Id,
                        BranchId = defaultBranchId,
                        TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                    });
                    debit = debit + salary;
                    //تسجيل قيد لذمم الموظفين 
                    //الى ح/ذمم الموظف 1 .... 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = employee.Person.AccountsTreeCustomerId,
                        Credit = salary,
                        Notes = $"  راتب مستحق للموظف: {employee.Person.Name} عن : {contractScheduling.Name}",
                        TransactionDate = Utility.GetDateTime(),
                        TransactionId = contractScheduling.Id,
                        BranchId = defaultBranchId,
                        TransactionTypeId = (int)TransactionsTypesCl.EmployeeSalaries
                    });
                    credit = credit + salary;

                    //التاكد من توازن المعاملة 
                    if (debit == credit)
                    {
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم اعتماد الراتب بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                    }
                    else
                        return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


        }


        #endregion

        void GetSalary(ContractScheduling contractScheduling, Employee employee)
        {
            //فى حالة الموظف بالانتاج 
            if (contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Production)
            {
                List<ContractSchedulingProduction> schedulProductionEmp = contractScheduling.ContractSchedulingProductions.Where(x => !x.IsDeleted).ToList();
                foreach (var item in schedulProductionEmp)
                {
                    //اجر العامل فى الساعه فى خط الانتاج
                    var productionOrder = db.ProductionOrders.Where(x => x.Id == item.ProductionOrderId).FirstOrDefault();
                    var productionLine = productionOrder.ProductionLine.ProductionLineEmployees.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id).FirstOrDefault();
                    productionOrderEmployee.Add(new ProductionOrderEmployee
                    {
                        ProductionOrderId = productionOrder.Id,
                        ProductionOrderHours = productionOrder.ProductionOrderHours,
                        HourlyWage = productionLine != null ? productionLine.HourlyWage : 0
                    });

                }
                vm.IsEmployeeProduction = true;
            }
            else
            {
                //فى حالة موظف ليس بالانتاج ولكن له اجر فى خطوط الانتاج
                var productionLinEmp = db.ProductionLineEmployees.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id);
                //كل اوامر الانتاج للموظف الحالى وتم تسجيله فى خط انتاج
                var productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted && productionLinEmp.Any(e => e.ProductionLineId == x.ProductionLineId));
                if (contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Daily)
                    productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) == contractScheduling.MonthYear);
                else if (contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Weekly)
                    productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) >= contractScheduling.MonthYear && DbFunctions.TruncateTime(x.ProductionOrderDate) < contractScheduling.ToDate);
                else if (contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly)
                    productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate).Value.Month == contractScheduling.MonthYear.Value.Month);


                foreach (var productionOrder in productionOrders.ToList())
                {
                    var productionLine = db.ProductionLines.Where(x => x.Id == productionOrder.ProductionLineId).FirstOrDefault().ProductionLineEmployees.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id).FirstOrDefault();
                    productionOrderEmployee.Add(new ProductionOrderEmployee
                    {
                        ProductionOrderId = productionOrder.Id,
                        ProductionOrderHours = productionOrder.ProductionOrderHours,
                        HourlyWage = productionLine != null ? productionLine.HourlyWage : 0
                    });
                }

            }
        }
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