﻿using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class OrderSellsController : Controller
    {
        // GET: OrderSells
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public OrderSellsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public static string DS { get; set; }

        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.QuoteOrderSells.Where(x => !x.IsDeleted && x.QuoteOrderSellType == (int)QuoteOrderSellTypeCl.OrderSell);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);
            if (customerId != null)
                list = list.Where(x => x.CustomerId == customerId);

            var items = list.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, CustomerName = x.Customer.Name, InvoiceDate = x.InvoiceDate.ToString(), TotalValue = x.TotalValue, Actions = n, Num = n }).ToList();
            return Json(new
            {
                data = items
            }, JsonRequestBehavior.AllowGet);

        }

        #region  اضافة الاصناف 
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new List<ItemDetailsDT>()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit(Guid? quoteId)
        {
            DS = null;
            QuoteOrderSell vm = new QuoteOrderSell();
            Guid? customerId = null;
            Guid? branchId = null;
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == id);
                    customerId = model?.CustomerId;
                    branchId = model?.BranchId;
                    var itemDetails = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).Select(x => new ItemDetailsDT { ItemId = x.ItemId, ItemName = x.Item.Name, Quantity = x.Quantity, Price = x.Price, Amount = x.Quantity * x.Price }).ToList();
                    DS = JsonConvert.SerializeObject(itemDetails);
                    vm = model;
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                if(quoteId!=null)
                {
                    var quote = db.QuoteOrderSells.Where(x => x.Id == quoteId).FirstOrDefault();
                    if (quote!=null)
                    {
                        branchId=quote.BranchId;
                        customerId=quote.CustomerId;
                        var itemDetails = quote.QuoteOrderSellDetails.Where(x => !x.IsDeleted).Select(x => new ItemDetailsDT { ItemId = x.ItemId, ItemName = x.Item.Name, Quantity = x.Quantity, Price = x.Price, Amount = x.Quantity * x.Price }).ToList();
                        DS = JsonConvert.SerializeObject(itemDetails);
                        vm.InvoiceDate = quote.InvoiceDate;

                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");

            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(QuoteOrderSell vm, string DT_Datasource)
        {
            if (ModelState.IsValid)
            {
                DateTime invoiceDate;
                if (vm.InvoiceDate == null || vm.CustomerId == null || vm.BranchId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                invoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                //الاصناف
                List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                List<QuoteOrderSellDetail> items = new List<QuoteOrderSellDetail>();

                if (DT_Datasource != null)
                {
                    itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_Datasource);
                    if (itemDetailsDT.Count() == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    else
                        items = itemDetailsDT.Select(x => new QuoteOrderSellDetail { ItemId = x.ItemId, Amount = x.Amount, Quantity = x.Quantity, Price = x.Price }).ToList();
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //حذف اى اصناف تم تسجيلها مسباق 

                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == vm.Id);
                    var previousItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in previousItems)
                    {
                        item.IsDeleted = true;
                    }

                    model.CustomerId = vm.CustomerId;
                    model.InvoiceDate = invoiceDate;
                    model.BranchId = vm.BranchId;
                    model.Notes = vm.Notes;
                    model.TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0);
                    model.TotalValue = items.Sum(x => (double?)x.Amount ?? 0);
                    model.QuoteOrderSellDetails = items;
                    //db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    //اضافة رقم الفاتورة
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    vm.InvoiceNumber = codePrefix + (db.QuoteOrderSells.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);
                    vm.InvoiceDate = invoiceDate;
                    vm.TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0);
                    vm.TotalValue = items.Sum(x => (double?)x.Amount ?? 0);
                    vm.QuoteOrderSellType = (int)QuoteOrderSellTypeCl.OrderSell;
                    vm.QuoteOrderSellDetails = items;
                    db.QuoteOrderSells.Add(vm);

                }
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    DS = null;
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
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.QuoteOrderSells.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف الاصناف
                    var items = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in items)
                    {
                        item.IsDeleted = true;
                        //db.Entry(item).State = EntityState.Modified;
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