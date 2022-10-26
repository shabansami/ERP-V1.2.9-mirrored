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
    public class PagesRolesController : Controller
    {
        // GET: PagesRoles
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            ViewBag.RoleId = new SelectList(db.Roles.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.PagesRoles.Where(x => !x.IsDeleted).Select(x => new { Id = x.RoleId, RoleName = x.Role.Name, Actions = n, Num = n }).Distinct().ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult AssignPages()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.PagesRoles.Where(x=>!x.IsDeleted&&x.RoleId==id).FirstOrDefault();
                    ViewBag.RoleName = model.Role.Name;
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.PagesRoles.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new PagesRole());

        }
    }
        [HttpPost]
        public JsonResult AssignPages(PagesRole vm,string pageIds)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(pageIds) || vm.RoleId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                //الصفحات السابقة للصلاحية 
                var prevoiusPageRole = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == vm.RoleId);
                //cast ids from view to list<int>
                var pagesRoleIds = pageIds.Split(',');
                List<int> list = new List<int>();
                foreach (var item in pagesRoleIds)
                {
                    int id;
                    if (int.TryParse(item, out id))
                    {
                        list.Add(id);
                    }
                }

                if (vm.Id != Guid.Empty)
                {
                    //حذف اى صفحات سابقة مرتبطة بالصلاحية
                    foreach (var page in prevoiusPageRole)
                    {
                        page.IsDeleted = true;
                        db.Entry(page).State = EntityState.Modified;
                    }
                }
                else
                {
                    isInsert = true;
                if (prevoiusPageRole.Count()>0)
                    return Json(new { isValid = false, message = "تم تحديد الصفحات للصلاحية المحددة مسبقا" });
                }
                //اضافة الصفحات الى الصلاحية المحددة 
                foreach (var page in list)
                {
                    var isPage = db.Pages.FirstOrDefault(x=>x.Id==page).IsPage;
                    if (isPage)
                    {
                        db.PagesRoles.Add(new PagesRole
                        {
                            RoleId = vm.RoleId,
                            PageId = page
                        });
                    }
                  
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
            return RedirectToAction("AssignPages");

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {

                var model = db.Roles.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    foreach (var item in db.PagesRoles.Where(x=>x.RoleId==Id).ToList())
                    {
                        item.IsDeleted = true;
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