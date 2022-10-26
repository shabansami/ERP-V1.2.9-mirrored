
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using System.Web.Mvc;
using System;using ERP.DAL.Utilites;

namespace ERP.Web.Controllers
{

    public class AreasController : Controller
    {
        // GET: Areas
        VTSaleEntities db;
        VTSAuth auth;
        public AreasController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
        }
        [Authorization]
        public ActionResult Index()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Areas.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn=x.CreatedOn, CountryName = x.City.Country.Name,CityName=x.City.Name, Name = x.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [Authorization]
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Areas.FirstOrDefault(x=>x.Id==id);

                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name",model.City.CountryId);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted&&x.CountryId== model.City.CountryId), "Id", "Name",model.CityId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
                ViewBag.LastRow = db.Areas.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Area());
            }
        }

        public ActionResult IndexNewStyle()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            return View();
        }
        [HttpGet]
        public ActionResult TestNewStyle()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Areas.FirstOrDefault(x=>x.Id==id);

                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name",model.City.CountryId);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted&&x.CountryId== model.City.CountryId), "Id", "Name",model.CityId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
                ViewBag.LastRow = db.Areas.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Area());
            }
        }
        [Authorization]
        [HttpPost]
        public JsonResult CreateEdit(Area vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.CityId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.Areas.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Areas.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.CityId= vm.CityId    ;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Areas.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    db.Areas.Add(new Area
                    {
                        Name = vm.Name,
                        CityId = vm.CityId
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
        [Authorization]
        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [Authorization]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Areas.FirstOrDefault(x=>x.Id==Id);
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