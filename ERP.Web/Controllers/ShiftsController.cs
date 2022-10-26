using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ShiftsController : Controller
    {
        // GET: Shifts
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Shifts.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, DepartmentName = x.Department.Name, EmployeeName = x.Employee.Person.Name, WorkingPeriodName = x.WorkingPeriod.Name, Actions = n, Num = n }).ToList()
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
                    var model = db.Shifts.FirstOrDefault(x=>x.Id==id);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", model.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(model.DepartmentId), "Id", "Name", model.EmployeeId);
                    ViewBag.WorkPeriodId = new SelectList(db.WorkingPeriods.Where(x => !x.IsDeleted), "Id", "Name",model.WorkPeriodId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.WorkPeriodId = new SelectList(db.WorkingPeriods.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.Shifts.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            return View(new Shift());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Shift vm)
        {
            if (ModelState.IsValid)
            {
                    if (vm.DepartmentId == null||vm.WorkPeriodId==null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                var isDepartment = true;
                if (vm.DepartmentId != null && vm.EmployeeId == null)
                    isDepartment = true;
                else
                    isDepartment = false;

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (vm.EmployeeId != null)
                    {
                        if (db.Shifts.Where(x => !x.IsDeleted && x.Id != vm.Id && x.EmployeeId == vm.EmployeeId).Count() > 0)
                            return Json(new { isValid = false, message = "الموظف له نظام الوردية موجود مسبقا" });
                    }
                    else
                    {
                        if (db.Shifts.Where(x => !x.IsDeleted && x.Id != vm.Id && x.DepartmentId == vm.DepartmentId&&x.IsDepartment==true).Count() > 0)
                            return Json(new { isValid = false, message = "الادارة لها نظام الوردية موجود مسبقا" });
                    }
                    var model = db.Shifts.FirstOrDefault(x=>x.Id==vm.Id);
                    model.DepartmentId = vm.DepartmentId;
                    model.EmployeeId = vm.EmployeeId;
                    model.WorkPeriodId = vm.WorkPeriodId;
                    model.IsDepartment = isDepartment;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (vm.EmployeeId!=null)
                    {
                        if (db.Shifts.Where(x => !x.IsDeleted && x.EmployeeId == vm.EmployeeId ).Count() > 0)
                            return Json(new { isValid = false, message = "الموظف له نظام الوردية موجود مسبقا" });
                    }else
                    {
                        if (db.Shifts.Where(x => !x.IsDeleted && x.DepartmentId == vm.DepartmentId&&x.IsDepartment==true).Count() > 0)
                            return Json(new { isValid = false, message = "الادارة لها نظام الوردية موجود مسبقا" });
                    }

                    isInsert = true;
                    db.Shifts.Add(new Shift
                    {
                        DepartmentId = vm.DepartmentId,
                        EmployeeId = vm.EmployeeId,
                        WorkPeriodId = vm.WorkPeriodId,
                        IsDepartment =isDepartment
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
                var model = db.Shifts.FirstOrDefault(x=>x.Id==Id);
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