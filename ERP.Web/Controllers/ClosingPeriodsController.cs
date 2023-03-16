using ERP.DAL;
using ERP.DAL.Utilites;
using ERP.Web.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ClosingPeriodsController : Controller
    {
        VTSaleEntities db;
        public static string DS { get; set; }
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public ClosingPeriodsController()
        {
            db = new VTSaleEntities();
        }
        // GET: ClosingPeriods
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ClosingPeriods.Where(x => !x.IsDeleted).OrderBy(X => X.CreatedOn).Select(x => new { Id = x.Id, CreatedOn = x.CreatedOn.ToString(), Name = x.Name, StartDate = x.StartDate.ToString(), EndDate = x.EndDate.ToString(), Actions = n, Num = n }).ToList()
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
                    var model = db.ClosingPeriods.FirstOrDefault(x => x.Id == id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.LastRow = db.ClosingPeriods.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new ClosingPeriods());
            }


        }

        [HttpPost]
        public JsonResult CreateEdit(ClosingPeriods vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.StartDate == null|| vm.EndDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.ClosingPeriods.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.ClosingPeriods.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    model.StartDate = vm.StartDate;
                    model.EndDate = vm.EndDate;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.ClosingPeriods.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    db.ClosingPeriods.Add(new ClosingPeriods
                    {
                        Name = vm.Name,
                        StartDate = vm.StartDate,
                        EndDate = vm.EndDate

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
                var model = db.ClosingPeriods.FirstOrDefault(x => x.Id == Id);
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