using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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
using ERP.Web.ViewModels;
using ERP.DAL.Models;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ItemProductionsController : Controller
    {
        // GET: ItemProductions
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DSItems { get; set; }
        public static string DSItemDetails { get; set; }

        public ActionResult Index()
        {
            //ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            return View();
        }
        //string ReturnDataFormat(DateTime? dateTime)
        //{
        //    return dateTime.ToString("yyyy/MM/dd");
        //}
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ItemProductions.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).Select(x => new { Id = x.Id, ItemProductionName = x.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }


        #region اضافة الاصناف الخام 
        public ActionResult GetDSItemProDetails()
        {
            int? n = null;
            if (DSItemDetails == null)
                return Json(new
                {
                    data = new ItemProductionDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(DSItemDetails)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProductionDetails(Guid? itemId, double quantity, string DT_Datasource)
        {
            List<ItemProductionDetailsDT> deDS = new List<ItemProductionDetailsDT>();
            string itemName = "";
            string UnitOut = "";

            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(DT_Datasource);
            if (itemId != null)
            {
                if (deDS.Where(x => x.ItemId == itemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == itemId).Name;
                UnitOut = db.Items.FirstOrDefault(x => x.Id == itemId).Unit.Name;

            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            var newProductionDetails = new ItemProductionDetailsDT { ItemId = itemId, ItemName = itemName, Quantity = quantity, UnitOut = UnitOut };
            deDS.Add(newProductionDetails);
            DSItemDetails = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم الاضافة بنجاح " }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region اضافة الاصناف النهائية او المنتج الكامل 
        public ActionResult GetDSItemProItems()
        {
            int? n = null;
            if (DSItems == null)
                return Json(new
                {
                    data = new ItemProductionDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(DSItems)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult addProductionItems(Guid? ItemInOutId, double QuantityIn, string DT_DatasourceItem)
        {
            List<ItemProductionDetailsDT> deDS = new List<ItemProductionDetailsDT>();
            string itemName = "";
            string UnitIn = "";

            if (DT_DatasourceItem != null)
                deDS = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(DT_DatasourceItem);
            if (ItemInOutId != null)
            {
                if (deDS.Where(x => x.ItemId == ItemInOutId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == ItemInOutId).Name;
                UnitIn = db.Items.FirstOrDefault(x => x.Id == ItemInOutId).Unit.Name;

            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            var newProductionItems = new ItemProductionDetailsDT { ItemId = ItemInOutId, ItemName = itemName, QuantityIn = QuantityIn, UnitIn = UnitIn };
            deDS.Add(newProductionItems);
            DSItems = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم الاضافة بنجاح " }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            DSItemDetails = null;
            DSItems = null;
            var list = new List<SelectListItem> {
                new SelectListItem{
                    Text="تجميع وتصنيع",
                    Value="1",
                    Selected=true
                 }, new SelectListItem
                 {
                     Text = "تقطيع وتكسير",
                     Value = "2",
                 }};

            ViewBag.ItemProductionTypeId = new SelectList(list, "Value", "Text");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemInOutId = new SelectList(itemList, "Id", "Name");
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            ViewBag.ItemtypeIdDetails = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 

            ViewBag.LastRow = db.ItemProductions.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
            return View(new ItemProductionVM());
        }
        [HttpPost]
        public JsonResult CreateEdit(ItemProductionVM vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.ItemProductionTypeId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (db.ItemProductions.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                    return Json(new { isValid = false, message = "مسمى التوليفة موجود مسبقا" });
                //الاصناف النهائية او الكاملة
                List<ItemProductionDetailsDT> deDSItem = new List<ItemProductionDetailsDT>();
                List<ItemProductionDetail> itemProItems = null;

                if (vm.DT_DatasourceItems != null)
                {
                    itemProItems = new List<ItemProductionDetail>();
                    deDSItem = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(vm.DT_DatasourceItems);
                    if (deDSItem.Count == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل " });

                    itemProItems = deDSItem.Select(x => new ItemProductionDetail
                    {
                        ItemId = x.ItemId,
                        Quantity = x.QuantityIn,
                        ProductionTypeId = vm.ItemProductionTypeId == 1 ? (int)ProductionTypeCl.Out : (int)ProductionTypeCl.In
                    }).ToList();
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل " });

                //الاصناف الخام او الجزئية
                List<ItemProductionDetailsDT> deDS = new List<ItemProductionDetailsDT>();
                List<ItemProductionDetail> itemProDetails = null;

                if (vm.DT_DatasourceDetails != null)
                {
                    itemProDetails = new List<ItemProductionDetail>();
                    deDS = JsonConvert.DeserializeObject<List<ItemProductionDetailsDT>>(vm.DT_DatasourceDetails);
                    if (deDS.Count == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل لتكوين المنتج" });

                    itemProDetails = deDS.Select(x => new ItemProductionDetail
                    {
                        ItemId = x.ItemId,
                        Quantity = x.Quantity,
                        ProductionTypeId = vm.ItemProductionTypeId == 1 ? (int)ProductionTypeCl.In : (int)ProductionTypeCl.Out
                    }).ToList();
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل لتكوين المنتج" });

                itemProItems.AddRange(itemProDetails);
                var itemGroupCount = itemProItems.GroupBy(x => x.ItemId).Count();
                if (itemProItems.Count() != itemGroupCount)
                    return Json(new { isValid = false, message = "يوجد تكرار لصنف فى جانبى تكوين التوليفة" });

                var itemProduction = new ItemProduction
                {
                    Name = vm.Name,
                    ItemProductionDetails = itemProItems,
                    ProductionTypeId=vm.ItemProductionTypeId
                };
                db.ItemProductions.Add(itemProduction);

                //var itemProduction = new ItemProduction
                //{
                //    Name = vm.Name,
                //    ItemFinalId = vm.ItemFinalId,
                //};
                //foreach (var item in itemProDetails)
                //{
                //    item.ItemProduction = itemProduction;
                //    db.ItemProductionDetails.Add(item);
                //}

                //}
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    DSItems = null;
                    DSItemDetails = null;
                    return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
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
        public ActionResult Copy(string id)
        {
            Guid GuId;

            if (!Guid.TryParse(id, out GuId) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = GuId;
            return RedirectToAction("SaveCopied");
        }

        [HttpGet]
        public ActionResult SaveCopied()
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            List<ItemProductionDetailsDT> deDSItem = new List<ItemProductionDetailsDT>();
            Guid guId;

            if (Guid.TryParse(TempData["model"].ToString(), out guId))
            {
                var vm = db.ItemProductions.Where(x => x.Id == guId).FirstOrDefault();
                ItemProductionVM itemProductionVM = new ItemProductionVM();
                itemProductionVM.Name = vm.Name;
                var itemsIn = db.ItemProductionDetails.Where(x => x.ItemProductionId == guId && x.ProductionTypeId == (int)ProductionTypeCl.In).ToList();
                var itemsoOut = db.ItemProductionDetails.Where(x => x.ItemProductionId == guId && x.ProductionTypeId == (int)ProductionTypeCl.Out).ToList();
               
                List<ItemProductionDetailsDT> deDSout = new List<ItemProductionDetailsDT>();
                string itemName = "";
                string UnitIn = "";
               
                


                List<ItemProductionDetailsDT> deDSin = new List<ItemProductionDetailsDT>();
                string itemNameout = "";
                string UnitOut = "";

             
                if (vm.ProductionTypeId==1)
                {
                    foreach (var item in itemsoOut)
                    {
                        itemName = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Name;
                        UnitIn = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Unit.Name;
                        var newProductionItems = new ItemProductionDetailsDT { ItemId = item.ItemId, ItemName = itemName, QuantityIn = item.Quantity, UnitIn = UnitIn };
                        deDSout.Add(newProductionItems);
                    }
                    DSItems = JsonConvert.SerializeObject(deDSout);
                    foreach (var item in itemsIn)
                    {
                        itemNameout = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Name;
                        UnitOut = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Unit.Name;
                        var newProductionDetails = new ItemProductionDetailsDT { ItemId = item.ItemId, ItemName = itemNameout, Quantity = item.Quantity, UnitOut = UnitOut };
                        deDSin.Add(newProductionDetails);
                    }
                    DSItemDetails = JsonConvert.SerializeObject(deDSin);
                    var list = new List<SelectListItem> {
                new SelectListItem{
                    Text="تجميع وتصنيع",
                    Value="1",
                  Selected=true
                 }, new SelectListItem
                 {
                     Text = "تقطيع وتكسير",
                     Value = "2"
                 }};
                    ViewBag.ItemProductionTypeId = new SelectList(list, "Value", "Text",1);
                }
                else
                {
                    foreach (var item in itemsIn)
                    {
                        itemName = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Name;
                        UnitIn = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Unit.Name;
                        var newProductionItems = new ItemProductionDetailsDT { ItemId = item.ItemId, ItemName = itemName, QuantityIn = item.Quantity, UnitIn = UnitIn };
                        deDSout.Add(newProductionItems);
                    }
                    DSItems = JsonConvert.SerializeObject(deDSout);
                    foreach (var item in itemsoOut)
                    {
                        itemNameout = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Name;
                        UnitOut = db.Items.FirstOrDefault(x => x.Id == item.ItemId).Unit.Name;
                        var newProductionDetails = new ItemProductionDetailsDT { ItemId = item.ItemId, ItemName = itemNameout, Quantity = item.Quantity, UnitOut = UnitOut };
                        deDSin.Add(newProductionDetails);
                    }
                    DSItemDetails = JsonConvert.SerializeObject(deDSin);
                    var list = new List<SelectListItem> {
                new SelectListItem{
                    Text="تجميع وتصنيع",
                    Value="1"
                 }, new SelectListItem
                 {
                     Text = "تقطيع وتكسير",
                     Value = "2",
                  Selected=true

                 }};
                    ViewBag.ItemProductionTypeId = new SelectList(list, "Value", "Text",2);
                }
                itemProductionVM.ItemProductionTypeId = vm.ProductionTypeId;
                ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
                ViewBag.ItemInOutId = new SelectList(itemList, "Id", "Name");
                ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
                ViewBag.ItemtypeIdDetails = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 

                ViewBag.LastRow = db.ItemProductions.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(itemProductionVM);


            }
            else

                return View("Index");
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ItemProductions.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    var itemProductionOrderExist = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ItemProductionId == Id).Any();
                    if (itemProductionOrderExist)
                        return Json(new { isValid = false, message = "لايمكن حذف التوليفة لارتباطها بأوامر انتاج" });

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //remove all item production details
                    var itemProDetails = db.ItemProductionDetails.Where(x => !x.IsDeleted && x.ItemProductionId == Id).ToList();
                    foreach (var item in itemProDetails)
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
        #region عرض بيانات توليفة بالتفصيل 
        public ActionResult ShowItemProduction(Guid? itemProId)
        {
            if (itemProId == null || itemProId == Guid.Empty)
                return RedirectToAction("Index");


            var vm = db.ItemProductions.Where(x => x.Id == itemProId && x.ItemProductionDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.ItemProductionDetails = vm.ItemProductionDetails.Where(x => !x.IsDeleted).ToList();
            return View(vm);
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