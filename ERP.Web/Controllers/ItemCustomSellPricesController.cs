using ERP.DAL;
using ERP.DAL.Models;
using ERP.Web.Identity;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ItemCustomSellPricesController : Controller
    {
        // GET: ItemCustomSellPrices
        VTSaleEntities db;
        VTSAuth auth;
        StoreService storeService;
        public ItemCustomSellPricesController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            storeService = new StoreService();
        }
        #region تحديد سعر بيع الاصناف 

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ItemCustomSellPrices.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, BranchName = x.Branch != null ? x.Branch.Name : null, GroupBasicName = x != null ? x.Group.Name : null, ItemName = x.Item != null ? x.Item.Name : null, ProfitPercentage = x.ProfitPercentage, Actions = n, Num = n }).ToList()
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
                    var model = db.ItemCustomSellPrices.FirstOrDefault(x=>x.Id==id);
                    ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name", model.GroupBasicId); // item groups (مواد خام - كشافات ...)
                    ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", model.BranchId);
                    ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList(), "Id", "Name", model.ItemId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                var branches = db.Branches.Where(x => !x.IsDeleted);
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                //تحميل كل الاصناف فى اول تحميل للصفحة 
                var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
                ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
                return View(new ItemCustomSellPrice());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ItemCustomSellPrice vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ProfitPercentage == 0 || (vm.BranchId == null && vm.GroupBasicId == null && vm.ItemId == null))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id!=Guid.Empty)
                {
                    //if (db.Areas.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.ItemCustomSellPrices.FirstOrDefault(x=>x.Id==vm.Id);
                    model.BranchId = vm.BranchId;
                    model.GroupBasicId = vm.GroupBasicId;
                    model.ItemId = vm.ItemId;
                    model.ProfitPercentage = vm.ProfitPercentage;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //if (db.ItemCustomSellPrices.Where(x => !x.IsDeleted && x.ItemId == vm.ItemId&&x.BranchId==vm.BranchId).Count() > 0)
                    //    return Json(new { isValid = false, message = "تم تحديد سعر البيع للصنف للفرع موجود مسبقا" });

                    isInsert = true;
                    db.ItemCustomSellPrices.Add(new ItemCustomSellPrice
                    {
                        BranchId = vm.BranchId,
                        GroupBasicId = vm.GroupBasicId,
                        ItemId = vm.ItemId,
                        ProfitPercentage = vm.ProfitPercentage,
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
                var model = db.ItemCustomSellPrices.FirstOrDefault(x=>x.Id==Id);
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