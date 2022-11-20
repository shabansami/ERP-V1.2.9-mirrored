using Newtonsoft.Json;
using ERP.Web.Identity;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DAL;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ItemUnitsController : Controller
    {
        // GET: ItemUnits
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public ItemUnitsController()
        {
            db = new VTSaleEntities();
        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.UnitId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 

            return View(new ItemUnitVM());

        }
        [HttpPost]
        public ActionResult CreateEdit(ItemUnitVM vm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.UnitId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name", vm.UnitId);
                ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.ItemtypeId);// item type (منتج خام - وسيط - نهائى 

                if (vm.UnitId == null || vm.GeneralQuantity == 0 || vm.GeneralSellPrice == 0 || vm.ItemtypeId == null)
                {
                    ViewBag.ErrorMsg = "تأكد من اختيار البيانات بشكل صحيح";
                    return View(vm);
                }


                var items = db.Items.Where(x => !x.IsDeleted && x.ItemTypeId == vm.ItemtypeId && x.AvaliableToSell && x.GroupBasic.GroupTypeId == (int)GroupTypeCl.Basic).Select(x => new ItemUnitDetails
                {
                    ItemUnitId = x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault() != null ? x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault().Id : Guid.Empty,
                    ItemId = x.Id,
                    ItemName = x.Name,
                    SellPrice = x.SellPrice,
                    ItemUnitBase = x.UnitId,
                    SellPriceCustome = x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault() != null ? x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault().SellPrice : vm.GeneralSellPrice,
                    Quantity = x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault() != null ? x.ItemUnits.Where(ip => !ip.IsDeleted && ip.UnitId == vm.UnitId).FirstOrDefault().Quantity : vm.GeneralQuantity
                }).ToList();

                return View(new ItemUnitVM
                {
                    UnitId = vm.UnitId,
                    ItemtypeId = vm.ItemtypeId,
                    GeneralQuantity = vm.GeneralQuantity,
                    GeneralSellPrice = vm.GeneralSellPrice,
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
        public ActionResult AddItemUnit(string unitId, string data)
        {
            if (string.IsNullOrEmpty(unitId))
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

            List<ItemUnitDetails> DTItemUnits = new List<ItemUnitDetails>();
            if (data != null)
                DTItemUnits = JsonConvert.DeserializeObject<List<ItemUnitDetails>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            //var currentItemUnits = db.ItemUnits.Where(x => !x.IsDeleted);
            Guid unitID;
            if (!Guid.TryParse(unitId, out unitID))
                return Json(new { isValid = false, message = "تأكد من اختيار الوحدة" });

            var unitBaseEqualUnit = DTItemUnits.Where(x => x.ItemUnitBase == unitID && x.SellPriceCustome > 0).Select(x => x.ItemName).ToList();
            if (unitBaseEqualUnit.Count() > 0)
                return Json(new { isValid = false, message = $"الاصناف الاتية تم اختيار الوحدة بنفس الوحدة الاساسية للصنف {string.Join(",", unitBaseEqualUnit)}" });

            foreach (var item in DTItemUnits)
            {
                double sellPriceCustome;
                if (double.TryParse(item.SellPriceCustome.ToString(), out sellPriceCustome))
                {
                    if (sellPriceCustome > 0)
                    {
                        if (item.ItemUnitId != Guid.Empty)//يوجد ادخال سابق للوحدة 
                        {
                            var itemUnit = db.ItemUnits.FirstOrDefault(x=>x.Id==item.ItemUnitId);
                            itemUnit.SellPrice = sellPriceCustome;
                            itemUnit.Quantity = item.Quantity;
                            db.Entry(itemUnit).State = EntityState.Modified;
                        }
                        else//يتم اضافة للوحدة لاول مرة 
                        {
                            db.ItemUnits.Add(new ItemUnit
                            {
                                ItemId = item.ItemId,
                                UnitId = unitID,
                                SellPrice = item.SellPriceCustome,
                                Quantity = item.Quantity
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
            //itemPrices = DTItemUnits.Select(x => new ItemPrice
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