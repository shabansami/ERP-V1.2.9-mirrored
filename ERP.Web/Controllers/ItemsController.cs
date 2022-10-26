using Newtonsoft.Json;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.DataTablesDS;
using ERP.Web.Services;
using System.IO;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class ItemsController : Controller
    {
        // GET: Items
        VTSaleEntities db;
        VTSAuth auth;
        ItemService itemService;
        public ItemsController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            itemService = new ItemService();
        }
        public ActionResult Index()
        {
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.GroupSellId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Sell), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.UnitId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name"); // وحدة قياس الصنف
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                var items = db.Items.Where(x => !x.IsDeleted && (x.ItemCode == txtSearch || x.Name.Contains(txtSearch) || x.BarCode.Contains(txtSearch))).OrderBy(x => x.CreatedOn).ToList().Select(x => new { Id = x.Id, LastPurchasePrice = itemService.GetLastPurchasePrice(x.Id, db).ToString(), GroupBasicName = x.GroupBasic.Name, GroupSellName = x.GroupSell?.Name, ItemTypeName = x.ItemType.Name, Name = x.Name, SellPrice = x.SellPrice, Actions = n, Num = n }).ToList();

                return Json(new
                {
                    data = items
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var items = db.Items.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).ToList().Select(x => new { Id = x.Id, LastPurchasePrice = itemService.GetLastPurchasePrice(x.Id, db).ToString(), GroupBasicName = x.GroupBasic.Name, GroupSellName = x.GroupSell?.Name, ItemTypeName = x.ItemType.Name, Name = x.Name, SellPrice = x.SellPrice, Actions = n, Num = n }).ToList();
                return Json(new
                {
                    data = items
                }, JsonRequestBehavior.AllowGet); ;
            }


        }

        #region add Sell Pricing
        public static string DS { get; set; }
        public ActionResult GetDSPricingSell()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new PricingPolicyDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<PricingPolicyDT>>(DS)
                }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddSellPricies(Guid? pricingPolicyId, Guid? customerId, double SellPricePolicy, string DT_Datasource)
        {
            List<PricingPolicyDT> deDS = new List<PricingPolicyDT>();
            string customerName = "";
            string pricingPolicyName = "";
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<PricingPolicyDT>>(DT_Datasource);
            if (pricingPolicyId != null)
                pricingPolicyName = db.PricingPolicies.FirstOrDefault(x => x.Id == pricingPolicyId).Name;
            if (customerId != null)
            {
                customerName = db.Persons.FirstOrDefault(x => x.Id == customerId).Name;
                if (deDS.Any(x => x.PricingPolicyId == pricingPolicyId && x.CustomerId == customerId))
                    return Json(new { isValid = false, message = " تم تحديد سياسة اسعار للعميل بالفعل مسبقا " }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (deDS.Any(x => x.PricingPolicyId == pricingPolicyId))
                    return Json(new { isValid = false, message = " سياسة الاسعار موجودة بالفعل مسبقا " }, JsonRequestBehavior.AllowGet);
            }
            var newSellPrice = new PricingPolicyDT { CustomerId = customerId, CustomerName = customerName, PricingPolicyName = pricingPolicyName, PricingPolicyId = pricingPolicyId, SellPricePolicy = SellPricePolicy };
            //if (deDS.Contains(newSellPrice))
            //    return Json(new { isValid = false }, JsonRequestBehavior.AllowGet);
            //else
            deDS.Add(newSellPrice);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region add Unit Item
        public static string DSUnits { get; set; }
        public ActionResult GetDSItemUnit()
        {
            int? n = null;
            if (DSUnits == null)
                return Json(new
                {
                    data = new ItemUnitDto()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemUnitDto>>(DSUnits)
                }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddItemUnit(string unitBaseId, string unitNewId, double quantity, double unitSellPrice, string DT_Datasource)
        {
            Guid unitNwId;
            Guid unitBsId;
            List<ItemUnitDto> deDS = new List<ItemUnitDto>();
            string unitNewName = "";
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemUnitDto>>(DT_Datasource);
            if (Guid.TryParse(unitBaseId, out unitBsId))
            {
                if (Guid.TryParse(unitNewId, out unitNwId))
                {
                    if (unitNwId == unitBsId)
                        return Json(new { isValid = false, message = "خطأ ... الوحدة المحدده هى نفسها الوحدة الاساسية " }, JsonRequestBehavior.AllowGet);
                    if (deDS.Any(x => x.UnitNewId == unitNwId))
                        return Json(new { isValid = false, message = " تم تحديد الوحدة بالفعل مسبقا " }, JsonRequestBehavior.AllowGet);

                    unitNewName = db.Units.FirstOrDefault(x => x.Id == unitNwId).Name;
                }
                else
                    return Json(new { isValid = false, message = "لابد من اختيار الوحدة بشكل صحيح" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isValid = false, message = "لابد من اختيار الوحدة الاساسية للصنف اولا" }, JsonRequestBehavior.AllowGet);

            var newItemUnit = new ItemUnitDto { UnitNewId = unitNwId, UnitName = unitNewName, Quantity = quantity, UnitSellPrice = unitSellPrice };
            //if (deDS.Contains(newSellPrice))
            //    return Json(new { isValid = false }, JsonRequestBehavior.AllowGet);
            //else
            deDS.Add(newItemUnit);
            DSUnits = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name"); // سياسة الخصوصية
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name"); // سياسة الخصوصية
            ViewBag.UnitNewId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name"); //وحدات الاصناف

            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Items.FirstOrDefault(x => x.Id == id);
                    //ViewBag.ItemGroupId = new SelectList(db.ItemGroups.Where(x => !x.IsDeleted), "Id", "Name",model.ItemGroupId); // item groups (مواد خام - كشافات ...)
                    ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name", model.ItemTypeId);// item type (منتج خام - وسيط - نهائى 
                    ViewBag.UnitId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name", model.UnitId); // وحدة قياس الصنف
                    ViewBag.UnitConvertFromId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name", model.UnitConvertFromId); // وحدة قياس الصنف
                    //سياسة الاسعار
                    var itemPrice = db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == id)
                        .Select(x => new PricingPolicyDT
                        {
                            CustomerId = x.CustomerId,
                            CustomerName = x.Person != null ? x.Person.Name : null,
                            Id = x.Id,
                            PricingPolicyId = x.PricingPolicyId,
                            PricingPolicyName = x.PricingPolicy != null ? x.PricingPolicy.Name : null,
                            SellPricePolicy = x.SellPrice
                        }).ToList();
                    DS = JsonConvert.SerializeObject(itemPrice);
                    //وحدات الصنف
                    var itemUnits = db.ItemUnits.Where(x => !x.IsDeleted && x.ItemId == id)
                        .Select(x => new ItemUnitDto
                        {
                            Id = x.Id,
                            UnitNewId = x.UnitId,
                            UnitName = x.Unit != null ? x.Unit.Name : null,
                            Quantity = x.Quantity,
                            UnitSellPrice = x.SellPrice
                        }).ToList();
                    DSUnits = JsonConvert.SerializeObject(itemUnits);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                DS = DSUnits = null;
                //ViewBag.ItemGroupId = new SelectList(db.ItemGroups.Where(x => !x.IsDeleted), "Id", "Name"); // item groups (مواد خام - كشافات ...)
                ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
                ViewBag.UnitId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name"); // وحدة قياس الصنف
                ViewBag.UnitConvertFromId = new SelectList(db.Units.Where(x => !x.IsDeleted), "Id", "Name"); // وحدة قياس الصنف
                ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name"); // سياسة الخصوصية
                ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted), "Id", "Name"); // سياسة الخصوصية

                ViewBag.LastRow = db.Items.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Item() { AvaliableToSell = true });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Item vm, string DT_Datasource, string DT_DatasourceUnits, HttpPostedFileBase ImageName)
        {
            //ModelState.Remove("GroupSellId");
            //vm.SellPrice=vm.
            //if (ModelState.IsValid)
            //{
            if (string.IsNullOrEmpty(vm.Name) || vm.GroupBasicId == null || vm.ItemTypeId == null || vm.UnitId == null)
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
            if (vm.UnitConvertFromId != null)
            {
                if (vm.UnitConvertFromCount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال كمية الوحدة المحول منها" });
            }
            var isInsert = false;
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            List<PricingPolicyDT> deDS = new List<PricingPolicyDT>();
            List<ItemPrice> itemPrices = new List<ItemPrice>();
            List<ItemUnitDto> deDSUnits = new List<ItemUnitDto>();
            List<ItemUnit> itemUnits = new List<ItemUnit>();

            if (DT_Datasource != null)
            {
                deDS = JsonConvert.DeserializeObject<List<PricingPolicyDT>>(DT_Datasource);
                itemPrices = deDS.Select(x => new ItemPrice
                {
                    ItemId = vm.Id,
                    PricingPolicyId = x.PricingPolicyId,
                    CustomerId = x.CustomerId,
                    SellPrice = x.SellPricePolicy
                }).ToList();
            }
            if (DT_DatasourceUnits != null)
            {
                deDSUnits = JsonConvert.DeserializeObject<List<ItemUnitDto>>(DT_DatasourceUnits);
                itemUnits = deDSUnits.Select(x => new ItemUnit
                {
                    ItemId = vm.Id,
                    UnitId = x.UnitNewId,
                    Quantity = x.Quantity,
                    SellPrice = x.UnitSellPrice
                }).ToList();
            }
            string ImageFullName = null;
            if (ImageName != null)
            {
                ImageFullName = Guid.NewGuid() + Path.GetFileNameWithoutExtension(ImageName.FileName) + ".jpg";
            }
            if (vm.Id != Guid.Empty)
            {
                if (db.Items.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                string itemCode = null;
                if (string.IsNullOrEmpty(vm.ItemCode))
                {
                    var groubCode = db.Groups.Where(x => x.Id == vm.GroupBasicId).FirstOrDefault();
                    if (groubCode != null)
                        itemCode = groubCode.GroupCode + "-" + (db.Items.Where(x => x.GroupBasicId == vm.GroupBasicId && !x.IsDeleted).Count() + 1).ToString();
                }
                else
                {
                    itemCode = vm.ItemCode;
                }
                var model = db.Items.FirstOrDefault(x => x.Id == vm.Id);
                model.Name = vm.Name;
                model.GroupBasicId = vm.GroupBasicId;
                model.GroupSellId = vm.GroupSellId;
                model.ItemTypeId = vm.ItemTypeId;
                model.SellPrice = vm.SellPrice;
                model.MaxPrice = vm.MaxPrice;
                model.MinPrice = vm.MinPrice;
                model.RequestLimit1 = vm.RequestLimit1;
                model.RequestLimit2 = vm.RequestLimit2;
                model.TechnicalSpecifications = vm.TechnicalSpecifications;
                model.UnitId = vm.UnitId;
                model.AvaliableToSell = vm.AvaliableToSell;
                model.UnitConvertFromId = vm.UnitConvertFromId;
                model.UnitConvertFromCount = vm.UnitConvertFromCount;
                model.CreateSerial = vm.CreateSerial;
                model.BarCode = vm.BarCode;
                model.ItemCode = itemCode;
                if (ImageName != null)
                {
                    string folderName = "~/Files/ItemImages";
                    string oldImagePath = Path.Combine(Server.MapPath(folderName) + "/" + model.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    model.ImageName = ImageFullName;

                    string FinalImagePath = Path.Combine(folderName, ImageFullName);
                    ImageName.SaveAs(Server.MapPath(FinalImagePath));
                }

                if (itemPrices.Count > 0)
                {
                    //remove all old item prices 
                    var oldItemPrice = db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == vm.Id).ToList();
                    foreach (var item in oldItemPrice)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.Entry(model).State = EntityState.Modified;
                    //add new item price
                    db.ItemPrices.AddRange(itemPrices);
                }

                if (itemUnits.Count > 0)
                {
                    //remove all old item units 
                    var oldItemUnits = db.ItemUnits.Where(x => !x.IsDeleted && x.ItemId == vm.Id).ToList();
                    foreach (var item in oldItemUnits)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.Entry(model).State = EntityState.Modified;
                    //add new item units
                    db.ItemUnits.AddRange(itemUnits);

                }
            }
            else
            {
                if (db.Items.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                isInsert = true;
                //تكويد الاصناف حسب المجموعات
                //var itemCodeExists = db.Items.Where(x => x.GroupBasicId == vm.GroupBasicId&&!x.IsDeleted).ToList();
                string itemCode = null;
                //if (itemCodeExists.Count > 0)
                //{
                //    var t = itemCodeExists.OrderByDescending(x => x.CreatedOn).FirstOrDefault().ItemCode;
                if (string.IsNullOrEmpty(vm.ItemCode))
                {
                    var groubCode = db.Groups.Where(x => x.Id == vm.GroupBasicId).FirstOrDefault();
                    if (groubCode != null)
                        itemCode = groubCode.GroupCode +"-"+ (db.Items.Where(x => x.GroupBasicId == vm.GroupBasicId && !x.IsDeleted).Count() + 1).ToString();
                }
                else
                {
                    itemCode = vm.ItemCode;
                }


                //}
                //else
                //    itemCode = vm.GroupBasicId + "01";
                string barcode;
                if (string.IsNullOrEmpty(vm.BarCode))
                {
                generate:
                    barcode = GeneratBarcodes.GenerateRandomBarcode();
                    var isExistInItems = db.Items.Where(x => x.BarCode == barcode).Any();
                    var isExistInItemSerials = db.ItemSerials.Where(x => x.SerialNumber == barcode).Any();
                    if (isExistInItems)
                        goto generate;
                }
                else
                {
                    barcode = vm.BarCode;
                }


                var item = new Item
                {
                    Name = vm.Name,
                    GroupBasicId = vm.GroupBasicId,
                    GroupSellId = vm.GroupSellId,
                    ItemTypeId = vm.ItemTypeId,
                    SellPrice = vm.SellPrice,
                    MaxPrice = vm.MaxPrice,
                    MinPrice = vm.MinPrice,
                    ItemCode = itemCode,
                    RequestLimit1 = vm.RequestLimit1,
                    RequestLimit2 = vm.RequestLimit2,
                    BarCode = barcode,
                    TechnicalSpecifications = vm.TechnicalSpecifications,
                    UnitId = vm.UnitId,
                    AvaliableToSell = vm.AvaliableToSell,
                    ItemPrices = itemPrices,
                    ItemUnits = itemUnits,
                    UnitConvertFromId = vm.UnitConvertFromId,
                    UnitConvertFromCount = vm.UnitConvertFromCount,
                    CreateSerial = vm.CreateSerial,
                    ImageName = ImageFullName
                };
                db.Items.Add(item);
                if (ImageName != null)
                {
                    string folderName = "~/Files/ItemImages";
                    Directory.CreateDirectory(Server.MapPath(folderName));
                    string FinalImagePath = Path.Combine(folderName, ImageFullName);
                    ImageName.SaveAs(Server.MapPath(FinalImagePath));
                }

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
        //    else
        //    {
        //        var errors = ModelState.Select(x => x.Value.Errors)
        //                   .Where(y => y.Count > 0)
        //                   .ToList();
        //        return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحييييييح" });

        //    }

        //}

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
                var model = db.Items.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //Remove Item Image
                    string folderName = "~/Files/ItemImages";
                    string oldImagePath = Path.Combine(Server.MapPath(folderName) + "/" + model.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    //remove all item prices
                    var itemPrices = db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == Id).ToList();
                    foreach (var item in itemPrices)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    //remove all item units
                    var itemUnits = db.ItemUnits.Where(x => !x.IsDeleted && x.ItemId == Id).ToList();
                    foreach (var item in itemUnits)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
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