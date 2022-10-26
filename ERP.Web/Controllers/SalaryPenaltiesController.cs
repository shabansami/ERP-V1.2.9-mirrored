using ERP.Web.Identity;
using ERP.DAL;
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
    public class SalaryPenaltiesController : Controller
    {
        // GET: SalaryPenalties
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SalaryPenalties.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, SalaryPenaltyTypeName = x.SalaryPenaltyType.Name, Actions = n, Num = n }).ToList()
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
                    var model = db.SalaryPenalties.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name", model.SalaryPenaltyTypeId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.SalaryPenalties.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new SalaryPenalty());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SalaryPenalty vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.SalaryPenaltyTypeId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.SalaryPenalties.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "العقوبة موجوده مسبقا" });

                    var model = db.SalaryPenalties.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.SalaryPenaltyTypeId = vm.SalaryPenaltyTypeId;


                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.SalaryPenalties.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "العقوبة موجوده مسبقا" });

                    isInsert = true;
                    db.SalaryPenalties.Add(new SalaryPenalty
                    {
                        Name = vm.Name,
                        SalaryPenaltyTypeId = vm.SalaryPenaltyTypeId
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
                var model = db.SalaryPenalties.FirstOrDefault(x=>x.Id==Id);
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