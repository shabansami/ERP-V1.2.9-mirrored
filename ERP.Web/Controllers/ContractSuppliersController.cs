﻿using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Identity;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class ContractSuppliersController : Controller
    {
        // GET: ContractSuppliers
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public ContractSuppliersController()
        {
            db = new VTSaleEntities();
        }
        #region ادارة 

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.PersonTypeId == (int)PersonTypeCl.Supplier).Select(x => new { Id = x.Id, SupplierName = x.Supplier != null ? x.Supplier.Name : null, FromDate = x.FromDate.ToString(), ToDate = x.ToDate.ToString(), DayCount = x.DayCount.ToString(), Actions = n, Num = n }).ToList()
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
                    var model = db.ContractCustomerSuppliers.FirstOrDefault(x => x.Id == id);
                    ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) && x.IsActive), "Id", "Name", model.SupplierId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) && x.IsActive), "Id", "Name");
                ViewBag.LastRow = db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.PersonTypeId == (int)PersonTypeCl.Supplier).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new ContractCustomerSupplier() { FromDate = Utility.GetDateTime(), ToDate = Utility.GetDateTime().AddMonths(1) });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ContractCustomerSupplier vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.SupplierId == null || vm.FromDate == null || vm.ToDate == null || vm.DayCount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //if (db.Areas.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                    if (db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.Id != vm.Id && x.SupplierId == vm.SupplierId && vm.FromDate >= DbFunctions.TruncateTime(x.FromDate) && vm.ToDate <= DbFunctions.TruncateTime(x.ToDate)).Any())
                        return Json(new { isValid = false, message = "المدة المدخلة متداخلة مع مدد سابقة تم ادخالها" });

                    var model = db.ContractCustomerSuppliers.FirstOrDefault(x => x.Id == vm.Id);
                    model.SupplierId = vm.SupplierId;
                    model.FromDate = vm.FromDate;
                    model.ToDate = vm.ToDate;
                    model.DayCount = vm.DayCount;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //if (db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.ItemId == vm.ItemId&&x.BranchId==vm.BranchId).Count() > 0)
                    //    return Json(new { isValid = false, message = "تم تحديد سعر البيع للصنف للفرع موجود مسبقا" });
                    if (db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.Id != vm.Id && x.SupplierId == vm.SupplierId && vm.FromDate >= DbFunctions.TruncateTime(x.FromDate) && vm.ToDate <= DbFunctions.TruncateTime(x.ToDate)).Any())
                        return Json(new { isValid = false, message = "المدة المدخلة متداخلة مع مدد سابقة تم ادخالها" });

                    isInsert = true;
                    db.ContractCustomerSuppliers.Add(new ContractCustomerSupplier
                    {
                        SupplierId = vm.SupplierId,
                        FromDate = vm.FromDate,
                        ToDate = vm.ToDate,
                        DayCount = vm.DayCount,
                        PersonTypeId = (int)PersonTypeCl.Supplier
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
                var model = db.ContractCustomerSuppliers.FirstOrDefault(x => x.Id == Id);
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