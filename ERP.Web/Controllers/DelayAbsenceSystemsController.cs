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

    public class DelayAbsenceSystemsController : Controller
    {
        // GET: DelayAbsenceSystems
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.DelayAbsenceSystems.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, IsDelay = x.IsDelay==true?"تأخير":"غياب", Penalty = x.IsDelay==true?x.DelayHourPenalty+" ساعه":x.AbsenceDayPenalty+" يوم", DelayMin = x.DelayMinNumber, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.DelayMinNumber = new SelectList(new List<SelectListItem>() { 
            new SelectListItem{Value="15",Text="15 دقيقة"},
            new SelectListItem{Value="30",Text="30 دقيقة"},
            new SelectListItem{Value="45",Text="45 دقيقة"},
            new SelectListItem{Value="60",Text="60 دقيقة"},
            },"Value","Text");

            return View(new DelayAbsenceSystem() { IsDelay=true});
        }
        [HttpPost]
        public JsonResult CreateEdit(DelayAbsenceSystem vm)
        {
                if ( vm.IsDelay == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار نوع النظام" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                   
                if (db.DelayAbsenceSystems.Where(x => !x.IsDeleted && !x.IsDelay == !vm.IsDelay).Count() > 0)
                        return Json(new { isValid = false, message = "نظام الغياب موجود مسبقا" });

                if (vm.IsDelay==false)
                {
                    if (vm.AbsenceDayPenalty==0)
                        return Json(new { isValid = false, message = "تأكد من ادخال عدد ايام خصم الغياب" });
                vm.DelayHourPenalty = 0;
                vm.DelayMinNumber = 0;
            }
                else if (vm.IsDelay == true)
                {
                    if (vm.DelayMinNumber == 0||vm.DelayHourPenalty==0)
                        return Json(new { isValid = false, message = "تأكد من ادخال عدد ساعات التأخير والخصم" });
                vm.AbsenceDayPenalty = 0;
                }
                db.DelayAbsenceSystems.Add(new DelayAbsenceSystem
                    {
                        IsDelay=vm.IsDelay,
                        AbsenceDayPenalty=vm.AbsenceDayPenalty,
                        DelayHourPenalty=vm.DelayHourPenalty,
                        DelayMinNumber=vm.DelayMinNumber
                    });
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.DelayAbsenceSystems.FirstOrDefault(x=>x.Id==Id);
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