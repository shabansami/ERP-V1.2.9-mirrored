using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Identity;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Models;
using ERP.Web.Services;
using Newtonsoft.Json;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class NotificationTasksController : Controller
    {
        // GET: NotificationTasks
        VTSaleEntities db ;
        VTSAuth auth ;
        public NotificationTasksController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll( string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.Notifications.Where(x => !x.IsDeleted && x.NotificationTypeId == (int)NotificationTypeCl.Task);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.DueDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.DueDate) <= dtTo.Date);

            return Json(new
            {
                data = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, DueDate =  x.DueDate.ToString(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Notifications.FirstOrDefault(x => x.Id == id);
                    var empList = model.NotificationEmployees.Where(x => !x.IsDeleted).Select(x => x.EmployeeId).ToList();
                    ViewBag.EmployeesList = new SelectList(EmployeeService.GetEmployees(), "Id", "Name",empList);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.EmployeesList = new SelectList(EmployeeService.GetEmployees(), "Id", "Name");
                ViewBag.LastRow = db.Notifications.Where(x => !x.IsDeleted && x.NotificationTypeId == (int)NotificationTypeCl.Task).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Notification() { DueDate = Utility.GetDateTime(),  StartAlertDate = Utility.GetDateTime().AddDays(-3) });
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(Notification vm,string EmpsList)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.DueDate == null || vm.StartAlertDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                var EmployeesList = JsonConvert.DeserializeObject<List<Guid>>(EmpsList);
                if (vm.Id != Guid.Empty)
                {
                    var model = db.Notifications.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    model.DueDate = vm.DueDate;
                    model.StartAlertDate = vm.StartAlertDate;

                    //حذف اى موظفين سابقين مسجلين لللمهمة
                    var oldEmp = model.NotificationEmployees.Where(x => !x.IsDeleted).ToList();
                    foreach (var emp in oldEmp)
                    {
                        emp.IsDeleted = true;
                    }
                    if (EmployeesList.Count()>0)
                    {
                        foreach (var emp in EmployeesList)
                        {
                            db.NotificationEmployees.Add(new DAL.Models.NotificationEmployee
                            {
                                EmployeeId = emp,
                                NotificationId = model.Id
                            });
                        }
                    }
                }
                else
                {
                    //if (db.Notifications.Where(x => !x.IsDeleted && x.Name == vm.Name && x.NotificationTypeId == (int)NotificationTypeCl.Task).Count() > 0)
                    //    return Json(new { isValid = false, message = "مسمى المصروف موجود مسبقا" });

                    isInsert = true;
                   
                    var task = new Notification
                    {
                        Name = vm.Name,
                        DueDate = vm.DueDate,
                        IsPeriodic = false,
                        StartAlertDate = vm.StartAlertDate,
                        NotificationTypeId = (int)NotificationTypeCl.Task
                    };
                    List<NotificationEmployee> notificationEmployees = new List<NotificationEmployee>();
                    if (EmployeesList.Count() > 0)
                    {
                        foreach (var emp in EmployeesList)
                        {
                            notificationEmployees.Add(new NotificationEmployee
                            {
                                EmployeeId = emp,
                                NotificationId = task.Id
                            });
                        }
                    }
                    task.NotificationEmployees = notificationEmployees;
                    db.Notifications.Add(task);

                    //db.NotificationEmployees.AddRange(notificationEmployees);
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
                var model = db.Notifications.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    var emps = model.NotificationEmployees.Where(x => !x.IsDeleted);
                    if (emps.Count()>0)
                    {
                        foreach (var emp in emps)
                        {
                            emp.IsDeleted = true;
                        }
                    }
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