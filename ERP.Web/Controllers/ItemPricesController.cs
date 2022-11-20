﻿using Newtonsoft.Json;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
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

    public class ItemPricesController : Controller
    {
        // GET: ItemPrices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted&&x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name"); // سياسة الخصوصية

            return View(new ItemPriceVM());

        }
        [HttpPost]
        public ActionResult CreateEdit(ItemPriceVM vm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", vm.PersonCategoryId);
                ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", vm.CustomerId);
                ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.ItemtypeId);// item type (منتج خام - وسيط - نهائى 
                ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name", vm.PricingPolicyId); // سياسة الخصوصية

                if (vm.CustomerId == null || vm.PricingPolicyId == null /*|| vm.ItemtypeId == null*/)
                {
                    ViewBag.ErrorMsg = "تأكد من اختيار البيانات بشكل صحيح";
                    return View(vm);
                }

                var itemsQ = db.Items.Where(x => !x.IsDeleted  && x.AvaliableToSell && x.GroupBasic.GroupTypeId == (int)GroupTypeCl.Basic);
                if(!vm.ShowAllItems)
                     if (vm.ItemtypeId != null)
                          itemsQ = itemsQ.Where(x => x.ItemTypeId == vm.ItemtypeId);
                    else
                    {
                        ViewBag.ErrorMsg = "تأكد من تحديد عرض كل الاصناف او تحديد نوع الصنف";
                        return View(vm);
                    }
                var items = itemsQ.Select(x => new ItemCustomers
                {
                    ItemPriceId = x.ItemPrices.Where(ip => !ip.IsDeleted && ip.CustomerId == vm.CustomerId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault() != null ? x.ItemPrices.Where(ip => !ip.IsDeleted && ip.CustomerId == vm.CustomerId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault().Id : Guid.Empty,
                    ItemId = x.Id,
                    ItemName = x.Name,
                    SellPrice = x.SellPrice,
                    SellPriceCustome = x.ItemPrices.Where(ip => !ip.IsDeleted && ip.CustomerId == vm.CustomerId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault().SellPrice ?? 0
                }).ToList();

                return View(new ItemPriceVM
                {
                    CustomerId = vm.CustomerId,
                    ItemtypeId = vm.ItemtypeId,
                    PersonCategoryId = vm.PersonCategoryId,
                    PricingPolicyId = vm.PricingPolicyId,
                    ItemsDetails = items
                });
            }
            else
            {
                ViewBag.ErrorMsg = "تأكد من اختيار البيانات بشكل صحيح";
                return View(vm);
            }

        }
        [HttpPost]
        public ActionResult AddItemPriceCustomer(string PricingPolicyId, string CustomerId, string data)
        {
            if (string.IsNullOrEmpty(CustomerId) || string.IsNullOrEmpty(PricingPolicyId))
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

            List<ItemCustomers> DTItemPrices = new List<ItemCustomers>();
            if (data != null)
                DTItemPrices = JsonConvert.DeserializeObject<List<ItemCustomers>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            var currentItemPrices = db.ItemPrices.Where(x => !x.IsDeleted);
            foreach (var item in DTItemPrices)
            {
                double sellPriceCustome;
                if (double.TryParse(item.SellPriceCustome.ToString(), out sellPriceCustome))
                {
                    if (item.SellPriceCustome > 0)
                    {
                        if (item.ItemPriceId != Guid.Empty)//يوجد ادخال سابق لسياسة الاسعار 
                        {
                            var itemPrice = db.ItemPrices.FirstOrDefault(x=>x.Id==item.ItemPriceId);
                            itemPrice.SellPrice = item.SellPriceCustome;
                            db.Entry(itemPrice).State = EntityState.Modified;
                        }
                        else//يتم اضافة سياسة السعر للعميل لاول مرة 
                        {
                            db.ItemPrices.Add(new ItemPrice
                            {
                                ItemId = item.ItemId,
                                PricingPolicyId = Guid.Parse(PricingPolicyId),
                                SellPrice = item.SellPriceCustome,
                                CustomerId = Guid.Parse(CustomerId)
                            });

                        }
                    }

                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال الاسعار بشكل صحيح" });
            }

            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            //List<ItemPrice> itemPrices = new List<ItemPrice>();
            //itemPrices = DTItemPrices.Select(x => new ItemPrice
            //{
            //    ItemId = x.ItemId,
            //    PricingPolicyId = vm.PricingPolicyId,
            //    CustomerId = vm.CustomerId,
            //    SellPrice = x.SellPriceCustome
            //}).ToList();


            //return View();
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