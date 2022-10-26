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

    public class VacationDaysController : Controller
    {
        // GET: VacationDays
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
                data = db.VacationDays.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, DepartmentName = x.Department.Name, EmployeeName = x.Employee.Person.Name, DayName = x.WeekDay.Name, DateFrom = x.DateFrom.ToString(), DateTo = x.DateTo.ToString(), IsWeekly = x.IsWeekly ?? false ? "اسبوعى" : "سنوى", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //if (TempData["model"] != null) //edit
            //{
            //    int id;
            //    if (int.TryParse(TempData["model"].ToString(), out id))
            //    {
            //        var model = db.VacationDays.FirstOrDefault(x=>x.Id==id);
            //        ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name",model.Employee.DepartmentId);
            //        ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted&&x.DepartmentId==model.DepartmentId), "Id", "Name",model.EmployeeId);
            //        //ViewBag.FromDayId = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x=>x.Arrange), "Id", "Name",model.FromDayId);
            //        //ViewBag.ToDayId = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x=>x.Arrange), "Id", "Name",model.ToDayId);
            //       ViewBag.DayIds = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x => x.Arrange), "Id", "Name",);

            //        return View(model);
            //    }
            //    else
            //        return RedirectToAction("Index");
            //}
            //else
            //{                   // add
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.FromDayId = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x => x.Arrange), "Id", "Name");
            ViewBag.ToDayId = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x => x.Arrange), "Id", "Name");
            ViewBag.DayIds = new SelectList(db.WeekDays.Where(x => !x.IsDeleted).OrderBy(x => x.Arrange), "Id", "Name");
            ViewBag.LastRow = db.VacationDays.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            return View(new VacationDayVM() { chk_allDepart = true, IsWeekly = true, DateFrom = Utility.GetDateTime(), DateTo = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(VacationDayVM vm)
        {
            if (ModelState.IsValid)
            {
                if (!vm.chk_allDepart)
                {
                    if (vm.DepartmentId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                }
                if (vm.IsWeekly == true)
                    if (vm.DayIds == null || vm.DayIds.Count() == 0)
                        return Json(new { isValid = false, message = "تأكد من اختيار ايام الاجازات" });
                if (vm.IsWeekly == false)
                    if (vm.DateFrom == null || vm.DateTo == null)
                        return Json(new { isValid = false, message = "تأكد من اختيار ايام الاجازات" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                //if (vm.IsWeekly == true)
                //{
                //    vm.DateFrom = null;
                //    vm.DateTo = null;
                //}
                //if (vm.IsWeekly == false)
                //{
                //    DayIds = null;
                //}
                //if (vm.Id > 0)
                //{
                //    var model = db.VacationDays.FirstOrDefault(x=>x.Id==vm.Id);
                //    model.FromDayId = vm.FromDayId;
                //    model.ToDayId = vm.ToDayId;
                //    model.DateFrom = vm.DateFrom;
                //    model.DateTo = vm.DateTo;
                //    model.DepartmentId = vm.DepartmentId;
                //    model.EmployeeId = vm.EmployeeId;
                //    model.IsWeekly = vm.IsWeekly;

                //    db.Entry(model).State = EntityState.Modified;
                //}
                //else
                //{

                isInsert = true;
                if (vm.chk_allDepart)
                {
                    var departs = db.Departments.Where(x => !x.IsDeleted).ToList();
                    if (departs.Count() > 0)
                    {
                        foreach (var depart in departs)
                        {
                            if (vm.DayIds != null && vm.DayIds.Count() > 0)
                            {
                                foreach (var item in vm.DayIds)
                                {
                                    var isExist = db.VacationDays.Where(x => !x.IsDeleted && x.DepartmentId == depart.Id && x.EmployeeId == vm.EmployeeId && x.DayId == item).Count();
                                    if (isExist == 0)
                                    {
                                        db.VacationDays.Add(new VacationDay
                                        {
                                            DayId = item,
                                            DateFrom = null,
                                            DateTo = null,
                                            DepartmentId = depart.Id,
                                            EmployeeId = vm.EmployeeId,
                                            IsWeekly = vm.IsWeekly,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var isExist = db.VacationDays.Where(x => !x.IsDeleted && x.DepartmentId == depart.Id && x.EmployeeId == vm.EmployeeId && x.DateFrom == vm.DateFrom && x.DateTo == vm.DateTo).Count();
                                if (isExist == 0)
                                {
                                    db.VacationDays.Add(new VacationDay
                                    {
                                        DayId = null,
                                        DateFrom = vm.DateFrom,
                                        DateTo = vm.DateTo,
                                        DepartmentId = depart.Id,
                                        EmployeeId = vm.EmployeeId,
                                        IsWeekly = vm.IsWeekly,
                                    });

                                }
                            }

                        }
                    }
                }
                else
                {
                    if (vm.DayIds != null && vm.DayIds.Count() > 0)
                    {
                        foreach (var item in vm.DayIds)
                        {
                            var isExist = db.VacationDays.Where(x => !x.IsDeleted && x.DepartmentId == vm.DepartmentId && x.EmployeeId == vm.EmployeeId && x.DayId == item).Count();
                            if (isExist == 0)
                            {
                                db.VacationDays.Add(new VacationDay
                                {
                                    DayId = item,
                                    DateFrom = null,
                                    DateTo = null,
                                    DepartmentId = vm.DepartmentId,
                                    EmployeeId = vm.EmployeeId,
                                    IsWeekly = vm.IsWeekly,
                                });
                            }

                        }
                    }
                    else
                    {
                        var isExist = db.VacationDays.Where(x => !x.IsDeleted && x.DepartmentId == vm.DepartmentId && x.EmployeeId == vm.EmployeeId && x.DateFrom == vm.DateFrom && x.DateTo == vm.DateTo).Count();
                        if (isExist == 0)
                        {

                            db.VacationDays.Add(new VacationDay
                            {
                                DayId = null,
                                DateFrom = vm.DateFrom,
                                DateTo = vm.DateTo,
                                DepartmentId = vm.DepartmentId,
                                EmployeeId = vm.EmployeeId,
                                IsWeekly = vm.IsWeekly,
                            });

                        }


                    }


                }


                //}
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
                var model = db.VacationDays.FirstOrDefault(x => x.Id == Id);
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