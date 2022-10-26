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

    public class UserRolesController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        // GET: UserRoles
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
                data = db.Users.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, IsActive = x.IsActive, UserName = x.UserName, DepartmentName = x.Person!=null?x.Person.Employees.FirstOrDefault().Department.Name:null, EmployeeName = x.Person!=null?x.Person.Name:null, RoleName = x.Role.Name, Actions = n, Num = n, Status = n }).ToList()
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
                    var model = db.Users.Where(x=>x.Id==id).
                        Select(x=>new UserVM
                        {
                            Id=x.Id,
                            EmployeeId=x.Person.Employees.FirstOrDefault().Id,
                            IsActive=x.IsActive,
                            RoleId=x.RoleId,
                            UserName=x.UserName,
                            Pass=x.Pass
                        }
                            ).FirstOrDefault();
                    var employee = db.Employees.Where(x => x.Id == model.EmployeeId).FirstOrDefault();
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", employee.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(employee.DepartmentId), "Id", "Name", employee.Id);
                    ViewBag.RoleId = new SelectList(db.Roles.Where(x => !x.IsDeleted), "Id", "Name",model.RoleId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.Users.Where(x => !x.IsDeleted&&x.PersonId!=null).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new UserVM() {IsActive=true });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(UserVM vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.UserName) || vm.EmployeeId == null|| vm.RoleId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                var emp = db.Employees.Where(x => x.Id == vm.EmployeeId).FirstOrDefault();
                if (vm.Id != Guid.Empty)
                {
                    if (db.Users.Where(x => !x.IsDeleted && x.UserName == vm.UserName && x.Id != vm.Id).Count() > 0) ///???
                        return Json(new { isValid = false, message = "اسم المستخدم موجود مسبقا" });
                    var model = db.Users.FirstOrDefault(x=>x.Id==vm.Id);
                    model.RoleId = vm.RoleId;
                    model.PersonId = emp.PersonId;
                    model.UserName = vm.UserName;
                    model.IsActive = vm.IsActive;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (string.IsNullOrEmpty(vm.Pass))
                        return Json(new { isValid = false, message = "تأكد من ادخال كلمة المرور" });

                    if (db.Users.Where(x => !x.IsDeleted && x.UserName == vm.UserName).Count() > 0)
                        return Json(new { isValid = false, message = "اسم المستخدم موجود مسبقا" });

                    isInsert = true;
                    db.Users.Add(new User
                    {
                        RoleId = vm.RoleId,
                        PersonId = emp.PersonId,
                        UserName = vm.UserName,
                        Pass = VTSAuth.Encrypt(vm.Pass),
                        IsActive = vm.IsActive
                    });
                }
                if (db.SaveChanges(auth.CookieValues.UserId)>0)
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
                var model = db.Users.FirstOrDefault(x=>x.Id==Id);
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