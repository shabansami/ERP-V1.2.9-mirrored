using Newtonsoft.Json;
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

    public class ItemGroupsController : Controller
    {
        // GET: Groups
        VTSaleEntities db ;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public ItemGroupsController()
        {
            db = new VTSaleEntities();
        }

        #region عرض البيانات
        public ActionResult Index()
        {
            //ViewBag.GroupTypeId = new SelectList(db.GroupTypes.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Groups.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, GroupTypeName = x.GroupType.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Groups.FirstOrDefault(x => x.Id == Id);
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

        #endregion

        #region تعديل واضافة
        [HttpGet]
        public ActionResult CreateEdit()
        {

            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Groups.FirstOrDefault(x => x.Id == id);
                    ViewBag.GroupTypeId = new SelectList(db.GroupTypes.Where(x => !x.IsDeleted), "Id", "Name", model.GroupTypeId);
                    ViewBag.GroupTypeIdTree = model.GroupTypeId;
                    //var data =Services.GetItemGroupTree(model.GroupTypeId);
                    //ViewBag.dataSou =JsonConvert.SerializeObject(data).ToString();
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.GroupTypeId = new SelectList(db.GroupTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.Groups.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                ViewBag.GroupTypeIdTree = "0";
                return View(new Group());
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(Group vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.Groups.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Groups.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    model.ParentId = vm.ParentId;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Groups.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    var newGroup = new Group
                    {
                        Name = vm.Name,
                        ParentId = vm.ParentId,
                        GroupTypeId = 1
                    };
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    newGroup.GroupCode = codePrefix + (db.Groups.Count(x => x.GroupCode.StartsWith(codePrefix)) + 1);

                    db.Groups.Add(newGroup);
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

        #endregion

        #region استعراض المجموعات
        public ActionResult ShowGroups()
        {
            return View();
        }
        #endregion
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