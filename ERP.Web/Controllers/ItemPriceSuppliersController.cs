using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using Newtonsoft.Json;
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
    public class ItemPriceSuppliersController : Controller
    {
        // GET: ItemPriceSuppliers
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name"); // سياسة الخصوصية

            return View(new ItemPriceVM());

        }
        [HttpPost]
        public ActionResult CreateEdit(ItemPriceVM vm)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name", vm.PersonCategoryId);
                ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", vm.SupplierId);
                ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.ItemtypeId);// item type (منتج خام - وسيط - نهائى 
                ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name", vm.PricingPolicyId); // سياسة الخصوصية

                if (vm.SupplierId == null || vm.PricingPolicyId == null /*|| vm.ItemtypeId == null*/)
                {
                    ViewBag.ErrorMsg = "تأكد من اختيار البيانات بشكل صحيح";
                    return View(vm);
                }

                var itemsQ = db.Items.Where(x => !x.IsDeleted && x.AvaliableToSell && x.GroupBasic.GroupTypeId == (int)GroupTypeCl.Basic);
                if (!vm.ShowAllItems)
                    if (vm.ItemtypeId != null)
                        itemsQ = itemsQ.Where(x => x.ItemTypeId == vm.ItemtypeId);
                        else
                    {
                        ViewBag.ErrorMsg = "تأكد من تحديد عرض كل الاصناف او تحديد نوع الصنف";
                        return View(vm);
                    }

                var items = itemsQ.Select(x => new ItemCustomers
                {
                    ItemPriceId = x.ItemPrices.Where(ip => !ip.IsDeleted && ip.SupplierId == vm.SupplierId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault() != null ? x.ItemPrices.Where(ip => !ip.IsDeleted && ip.SupplierId == vm.SupplierId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault().Id : Guid.Empty,
                    ItemId = x.Id,
                    ItemName = x.Name,
                    SellPrice = x.PurchaseInvoicesDetails.Where(p=>!p.IsDeleted).OrderByDescending(p => p.CreatedOn).Select(p=>p.Price).FirstOrDefault(),
                    SellPriceCustome = x.ItemPrices.Where(ip => !ip.IsDeleted && ip.SupplierId == vm.SupplierId && ip.PricingPolicyId == vm.PricingPolicyId).FirstOrDefault().SellPrice ?? 0
                }).ToList();

                return View(new ItemPriceVM
                {
                    SupplierId = vm.SupplierId,
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
        public ActionResult AddItemPriceSupplier(string PricingPolicyId, string SupplierId, string data)
        {
            if (string.IsNullOrEmpty(SupplierId)||!Guid.TryParse(SupplierId,out Guid supplierId)||supplierId==Guid.Empty || string.IsNullOrEmpty(PricingPolicyId))
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

            List<ItemCustomers> DTItemPrices = new List<ItemCustomers>();
            if (data != null)
                DTItemPrices = JsonConvert.DeserializeObject<List<ItemCustomers>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            var currentItemPrices = db.ItemPrices.Where(x => !x.IsDeleted);
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            foreach (var item in DTItemPrices)
            {
                double sellPriceCustome;
                if (double.TryParse(item.SellPriceCustome.ToString(), out sellPriceCustome))
                {
                    if (item.SellPriceCustome > 0)
                    {
                        if (item.ItemPriceId != Guid.Empty)//يوجد ادخال سابق لسياسة الاسعار 
                        {
                            var itemPrice = db.ItemPrices.FirstOrDefault(x => x.Id == item.ItemPriceId);
                            itemPrice.SellPrice = item.SellPriceCustome;
                            db.Entry(itemPrice).State = EntityState.Modified;
                        }
                        else//يتم اضافة سياسة السعر للمورد لاول مرة 
                        {
                            db.ItemPrices.Add(new ItemPrice
                            {
                                ItemId = item.ItemId,
                                PricingPolicyId = Guid.Parse(PricingPolicyId),
                                SellPrice = item.SellPriceCustome,
                                SupplierId = supplierId
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
            //    SupplierId = vm.SupplierId,
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