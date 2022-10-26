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

    public class ContractAttendanceLeavingsController : Controller
    {
        // GET: ContractAttendanceLeavings
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        //IQueryable<Employee> GetEmployees()
        //{
        //    var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
        //    var employees = db.Employees.Where(x => !x.IsDeleted);
        //    employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
        //    return employees;
        //}
        public ActionResult Index()
        {
            ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name");
            ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;

            return Json(new
            {
                data = db.ContractAttendanceLeavings.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Employee = x.ContractScheduling.Contract.Employee.Person.Name, MonthYear = x.ContractScheduling.Name /*x.ContractScheduling.MonthYear.ToString().Substring(0, 7)*/, AttendanceLeavingDate = x.AttendanceLeavingDate.ToString(), AttendanceTime = x.AttendanceTime.ToString().Substring(0, 8), LeavingTime = x.LeavingTime.ToString().Substring(0, 8), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.ContractAttendanceLeavings.FirstOrDefault(x=>x.Id==id);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", model.ContractScheduling.Contract.Employee.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(model.ContractScheduling.Contract.Employee.DepartmentId), "Id", "Name", model.ContractScheduling.Contract.EmployeeId);
                    ViewBag.ContractSchedulingId = new SelectList(db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == model.ContractScheduling.ContractId && !x.IsApproval).Select(x => new { Id = x.Id, Name = x.Name /*x.MonthYear.ToString().Substring(0, 7)*/ }), "Id", "Name", model.ContractSchedulingId);
                    ViewBag.salaryTypeName = $"نوع العقد : {model.ContractScheduling.Contract.ContractSalaryType.Name}";
                    ViewBag.salaryTypeId = model.ContractScheduling.Contract.ContractSalaryTypeId;

                    return View(new ContractAttendanceLeavingVM
                    {
                        AttendanceTime = model.AttendanceTime.ToString(),
                        LeavingTime = model.LeavingTime.ToString(),
                        AttendanceLeavingDate = model.AttendanceLeavingDate,
                        Id = model.Id,
                        Notes = model.Notes
                    });
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
                ViewBag.LastRow = db.ContractAttendanceLeavings.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                ViewBag.salaryTypeName = "";
                ViewBag.salaryTypeId = null;

                return View(new ContractAttendanceLeavingVM() { AttendanceLeavingDate = Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ContractAttendanceLeavingVM vm, int? salaryTypeId)
        {
            if (ModelState.IsValid)
            {
                if (vm.EmployeeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار الموظف" });
                if (salaryTypeId != (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار الشهر/الاسبوع" });

                if (vm.AttendanceTime == null && vm.LeavingTime == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال وقت الحضور او الانصراف" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                DateTime attendanceDt = new DateTime();
                DateTime leavingDt = new DateTime();
                if (!string.IsNullOrEmpty(vm.AttendanceTime))
                {
                    if (!DateTime.TryParse(vm.AttendanceTime, out attendanceDt))
                        return Json(new { isValid = false, message = "تأكد من ادخال وقت الحضور بشكل صحيح" });
                }
                if (!string.IsNullOrEmpty(vm.LeavingTime))
                {
                    if (!DateTime.TryParse(vm.LeavingTime, out leavingDt))
                        return Json(new { isValid = false, message = "تأكد من ادخال وقت الانصراف بشكل صحيح" });
                }

                if (vm.Id != Guid.Empty)
                {
                    var model = db.ContractAttendanceLeavings.FirstOrDefault(x=>x.Id==vm.Id);
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);
                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == vm.EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.AttendanceLeavingDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist > 0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear = vm.AttendanceLeavingDate,
                            ToDate = vm.AttendanceLeavingDate,
                            Name = $"يوم : {vm.AttendanceLeavingDate.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }
                    model.AttendanceLeavingDate = vm.AttendanceLeavingDate;
                    model.Notes = vm.Notes;
                    model.AttendanceTime = vm.AttendanceTime != null ? attendanceDt.TimeOfDay : new Nullable<TimeSpan>();
                    model.LeavingTime = vm.LeavingTime != null ? leavingDt.TimeOfDay : new Nullable<TimeSpan>();
                    model.ContractScheduling = contractScheduling;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //التأكد من عدم تسجيل نفس اليوم سابقا 
                    var isExists = db.ContractAttendanceLeavings.Where(x => !x.IsDeleted && x.ContractSchedulingId == vm.ContractSchedulingId && DbFunctions.TruncateTime(x.AttendanceLeavingDate) == vm.AttendanceLeavingDate).Count();

                    if (isExists > 0)
                        return Json(new { isValid = false, message = "تم تسجيل اليوم سابقا" });

                    isInsert = true;
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);

                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == vm.EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.AttendanceLeavingDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist > 0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear = vm.AttendanceLeavingDate,
                            ToDate = vm.AttendanceLeavingDate,
                            Name = $"يوم : {vm.AttendanceLeavingDate.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }
                    else
                    {
                        //التأكد من وقوع يوم الحضور فى نطاق الشهر/الاسبوع
                        if (!(salaryTypeId == (int)ContractSalaryTypeCl.Daily))
                        {
                            // فى حالة الشهري/الاسبوعى
                            var contractSalaryTpe = contractScheduling.Contract.ContractSalaryTypeId == (int)ContractSalaryTypeCl.Monthly ? "الشهر" : "الاسبوع";
                            if (!(vm.AttendanceLeavingDate >= contractScheduling.MonthYear && vm.AttendanceLeavingDate <= contractScheduling.ToDate))
                                return Json(new { isValid = false, message = $" تاريخ الحضور لا يقع فى نطاق {contractSalaryTpe} " });
                        }
                        else
                        {
                            // فى حالة اليومى
                            if (vm.AttendanceLeavingDate != contractScheduling.MonthYear)
                                return Json(new { isValid = false, message = $" تاريخ الحضور لا يساوى اليوم " });
                        }
                    }
                    var ContractAttendanceLeavings = new ContractAttendanceLeaving
                    {
                        AttendanceLeavingDate = vm.AttendanceLeavingDate,
                        Notes = vm.Notes,
                        AttendanceTime = vm.AttendanceTime != null ? attendanceDt.TimeOfDay : new Nullable<TimeSpan>(),
                        LeavingTime = vm.LeavingTime != null ? leavingDt.TimeOfDay : new Nullable<TimeSpan>(),
                        ContractScheduling = contractScheduling
                    };

                    db.ContractAttendanceLeavings.Add(ContractAttendanceLeavings);

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
                var model = db.ContractAttendanceLeavings.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

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