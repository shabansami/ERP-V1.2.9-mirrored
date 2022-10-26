using ERP.Web.DataTablesDS;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.Services
{
    public static class SalaryServices
    {
        public static AttendanceLeavingDT CalculateAttendanceLeaving(Guid contractSchedulingId)
        {
            using (var db=new VTSaleEntities())
            {
                var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==contractSchedulingId);
                var employee = contractScheduling.Contract.Employee;
                AttendanceLeavingDT attendanceLeaving = new AttendanceLeavingDT();

                //الطريقة الاول لاحتساب راتب الموظف لليوم الواحد 
                var dayCostEmp = Math.Round(contractScheduling.Contract.Salary / 30, 2);
                //الطريقة الادق لاحتساب راتب الموظف لليوم الواحد 
                //var dayCostEmp = Math.Round((contractScheduling.Contract.Salary*12) / 365, 2);
                double salary = contractScheduling.Contract.Salary;

                //احتساب ايام الغياب تلقائى من ماكينة البصمة 

                //مدة الراتب شهرى/اسبوعى/يومى
                var fromDate = contractScheduling.MonthYear??new DateTime(); //بداية الراتب
                var toDate = contractScheduling.ToDate;//نهاية الراتب
                var dayNum = (int)Math.Abs(Math.Round((toDate - fromDate).Value.TotalDays));//عدد ايام الراتب
                List<Tuple<DateTime, int>> months = new List<Tuple<DateTime, int>>();
                for (DateTime dt = fromDate; dt <= toDate; dt.AddDays(1))
                {
                    months.Add(new Tuple<DateTime, int>(dt, (int)dt.DayOfWeek == 0 ? 7 : (int)dt.DayOfWeek));
                }
                // فترات عمل الموظف 
                var shifts = db.Shifts.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id);
                TimeSpan? startTime; //بداية الدوام
                TimeSpan? endTime;//نهاية الدوام
                TimeSpan? delayFrom;//التأخير يتم من وقت
                TimeSpan? absenceOf;//يتم احتساب الموظف غائب من وقت 

                if (shifts.Count() > 0)//الموظف له فترة مخصصة مختلفة عن الادارة التابع لها 
                {
                    var shift = shifts.FirstOrDefault();
                    startTime = shift.WorkingPeriod.StartTime;
                    endTime = shift.WorkingPeriod.EndTime;
                    delayFrom = shift.WorkingPeriod.DelayFrom;
                    absenceOf = shift.WorkingPeriod.AbsenceOf;
                }
                else
                {
                    shifts = shifts.Where(x => x.DepartmentId == employee.DepartmentId);
                    if (shifts.Count() > 0)
                    {
                        var shift = shifts.FirstOrDefault();
                        startTime = shift.WorkingPeriod.StartTime;
                        endTime = shift.WorkingPeriod.EndTime;
                        delayFrom = shift.WorkingPeriod.DelayFrom;
                        absenceOf = shift.WorkingPeriod.AbsenceOf;
                    }
                    else
                        return new AttendanceLeavingDT() { Isvalid = false };
                    //else
                    //{
                    //    TempData["errorMsg"] = "لابد من تحديد نظام الوردية للموظف اولا";
                    //    return RedirectToAction("Index");
                    //}
                }

                //الحضور والانصراف للموظف
                var attendanceLeavings = db.ContractAttendanceLeavings.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractScheduling.Id);
                //عدد ساعات الفترة
                var hours = (endTime - startTime).Value.TotalHours;
                //اجر الموظف فى الساعة
                var hourCostEmp = dayCostEmp / hours;
                //نظام التأخير والغياب ان وجدت
                var delayAbsenceSystems = db.DelayAbsenceSystems.Where(x => !x.IsDeleted);
                var delaies = delayAbsenceSystems.Where(x => x.IsDelay == true).OrderByDescending(x => x.DelayMinNumber);
                var absence = delayAbsenceSystems.Where(x => x.IsDelay == false);

                //الايام التى تم الحضور بها كاملا
                var delay = 0;
                bool breakout = false;
                bool isAbsence = false;
                foreach (var day in attendanceLeavings)
                {
                    //التأكد من تسجيل الحضور بدأ من وقت الدوام
                    //ايام الغياب
                    if (absence.Count() > 0)
                    {
                        if (day.AttendanceTime >= absenceOf )
                        {
                            attendanceLeaving.AbsenenceDayPenalty +=1;
                            isAbsence = true;
                        }
                    }
                    //الايام التأخير
                    if (delaies.Count() > 0&&!isAbsence)
                    {
                        foreach (var dayDelay in delaies)
                        {
                            delay = dayDelay.DelayMinNumber - 1;
                            TimeSpan timeSpan = new TimeSpan(0, delay, 0);
                            delayFrom.Value.Add(timeSpan);
                            //احتساب التأخير بداية من اعلى وقت للتأخير 
                            //بسبب عدم معرفة عدد حالات التأخير المسجلة (15-45 ...
                            if (day.AttendanceTime>= delayFrom)
                            {
                                attendanceLeaving.DelayDayNumber += 1;
                                attendanceLeaving.DelayHourPenalty += dayDelay.DelayHourPenalty;
                                breakout = true;
                                break;
                            }
                        }
                        if (breakout)
                            break;
                    }

                    //عدد ساعات الافرتايم 
                    var dyEndNormal = day.AttendanceLeavingDate.Add(endTime ?? new TimeSpan());//وقت انتهاء العمل الطبيعى
                    var dyEndAcually = day.AttendanceLeavingDate.Add(day.LeavingTime ?? new TimeSpan());//وقت انصراف الموظف 
                    if (dyEndAcually > dyEndNormal)
                        attendanceLeaving.OverTimeHour += (dyEndAcually - dyEndNormal).TotalHours;
                }

                //ايام الاجازات فى الفترة المحدده للشهرى/الاسبوعى
                var vacationDayEmp = db.VacationDays.Where(x => !x.IsDeleted && x.EmployeeId == employee.Id);
               //فى حالة ان الموظف له ايام اجازات مخصصة
                if (vacationDayEmp.Count() > 0)
                {
                    foreach (var dy in vacationDayEmp)
                    {
                        //فى حالة الاجازة اسبوعية
                        if (dy.IsWeekly == true)
                        {
                           var monthCount = months.Where(x => x.Item2 == dy.DayId).Count();
                            if (monthCount > 0)
                                attendanceLeaving.VacationDays = monthCount;
                        }
                        else
                        {
                            //فى حالة الاجازة من/الى
                            var monthCount = months.Where(x => x.Item1>=dy.DateFrom&&x.Item1<=dy.DateTo).Count();
                            if (monthCount > 0)
                                attendanceLeaving.VacationDays = monthCount;
                        }
                    }
                }
                else
                {
                    //فى حالة ان اجازات الادارة 
                    var vacationDayDepartment = db.VacationDays.Where(x => !x.IsDeleted && x.DepartmentId == employee.DepartmentId);
                    if (vacationDayDepartment.Count() > 0)
                    {
                        foreach (var dy in vacationDayDepartment)
                        {
                            //فى حالة الاجازة اسبوعية
                            if (dy.IsWeekly == true)
                            {
                                var monthCount = months.Where(x => x.Item2 == dy.DayId).Count();
                                if (monthCount > 0)
                                    attendanceLeaving.VacationDays = monthCount;
                            }
                            else
                            {
                                //فى حالة الاجازة من/الى
                                var monthCount = months.Where(x => x.Item1 >= dy.DateFrom && x.Item1 <= dy.DateTo).Count();
                                if (monthCount > 0)
                                    attendanceLeaving.VacationDays = monthCount;
                            }
                        }
                    }
                }


                //الوصول الى نظام تعريف الغياب 
                //احتساب قيمة الغياب بالايام
                if (absence.Count() > 0)
                    attendanceLeaving.AbsenceAmount = Math.Round((attendanceLeaving.AbsenenceDayPenalty * absence.FirstOrDefault().AbsenceDayPenalty) * dayCostEmp,2);
                //احتساب قيمة التأخير 
                attendanceLeaving.DelayAmount = Math.Round(attendanceLeaving.DelayHourPenalty * hourCostEmp,2);
                //احتساب قيمة الافرتايم
                if (attendanceLeaving.OverTimeHour>1)
                {
                    //معرفة قيمة احتساب الافرتايم من عقد الموظف
                    var overTimeValue = contractScheduling.Contract.OverTime;
                    attendanceLeaving.OverTimeAmount = Math.Round(overTimeValue * hourCostEmp, 2);
                }

            }
            return null;
        }
    }
}