using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
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

    public class PeriodicIncomesController : Controller
    {
        // GET: PeriodicIncomes
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Notifications.OrderBy(x=>x.CreatedOn).Where(x => !x.IsDeleted && x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic).Select(x => new { Id = x.Id, Name = x.Name, Amount = x.Amount, type = x.IsPeriodic ? "دورى" : "محدد بتاريخ", DueDate = x.IsPeriodic ? "يوم (" + x.DueDate.Value.Day.ToString() + ") من كل شهر" : x.DueDate.ToString(), Actions = n, Num = n }).ToList()
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
                    var model = db.Notifications.FirstOrDefault(x=>x.Id==id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.LastRow = db.Notifications.Where(x => !x.IsDeleted && x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Notification() { DueDate = Utility.GetDateTime(), IsPeriodic = true, StartAlertDate = Utility.GetDateTime().AddDays(-3) });
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(Notification vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.DueDate == null|| vm.StartAlertDate == null || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.Notifications.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id && x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic).Count() > 0)
                        return Json(new { isValid = false, message = "مسمى المصروف موجود مسبقا" });

                    var model = db.Notifications.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.DueDate = vm.DueDate;
                    model.IsPeriodic = vm.IsPeriodic;
                    model.Amount = vm.Amount;
                    model.StartAlertDate = vm.StartAlertDate;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Notifications.Where(x => !x.IsDeleted && x.Name == vm.Name && x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic).Count() > 0)
                        return Json(new { isValid = false, message = "مسمى المصروف موجود مسبقا" });

                    isInsert = true;
                    var expenese = new Notification
                    {
                        Name = vm.Name,
                        DueDate = vm.DueDate,
                        IsPeriodic = vm.IsPeriodic,
                        Amount = vm.Amount,
                        StartAlertDate = vm.StartAlertDate,
                        NotificationTypeId = (int)NotificationTypeCl.IncomePeriodic
                    };
                    db.Notifications.Add(expenese);
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
                var model = db.Notifications.FirstOrDefault(x=>x.Id==Id);
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