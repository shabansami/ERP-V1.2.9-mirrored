using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptQuantityItemProductionsController : Controller
    {
        // GET: RptQuantityItemProductions
        VTSaleEntities db;
        VTSAuth auth;
        ItemService _itemService;
        public RptQuantityItemProductionsController()
        {
            auth = new VTSAuth();
            db = new VTSaleEntities();
            _itemService = new ItemService();
        }
        public static string DSItemsMaterial { get; set; }

        public ActionResult Index()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemFinalId = new SelectList(new List<Item>(), "Id", "Name"); // final item
            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");

            //التاكد من تحديد احتساب تكلفة المنتج من الاعدادات اولا 
            var itemCostSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault();
            int? itemCostId = null;
            if (!string.IsNullOrEmpty(itemCostSetting.SValue))
                itemCostId = int.Parse(itemCostSetting.SValue);
            else
                ViewBag.Msg = "تأكد من اختيار طريقة احتساب تكلفة المنتج أولا من شاشة الاعدادات العامة";
            ViewBag.ItemCostCalculateId = itemCostId;
            return View();

        }
        public ActionResult GetDSItemMaterials(string itemFinalId, double quantity, Guid? storeId, int? ItemCostCalculateId, bool? allStores)
        {
            int? n = null;
            Guid itemFinalID = Guid.Empty;
            // تم اضافة المواد الخام بكمياتها وتحديث الجريد 
            if (Guid.TryParse(itemFinalId, out itemFinalID))
            {
                var itemPro = db.ItemProductions.Where(x => !x.IsDeleted /*&& x.ItemFinalId == itemFinalID*/).OrderByDescending(x => x.Id).FirstOrDefault();
                if (itemPro != null)
                {
                    bool hasStore = false;
                    if (storeId != null && storeId != Guid.Empty)
                        hasStore = true;
                    if (allStores == true) //فى حالة اختيار مخزن ولكن تم اختيار (كل المخازن) فى الاتشيكبوكس
                        hasStore = false;

                    var itemsMaterials = itemPro.ItemProductionDetails.Where(x => !x.IsDeleted).Select(x => new ItemProductionOrderDetailsDT
                    {
                        ItemId = x.ItemId,
                        ItemName = x.Item.Name,
                        QuantityRequired = x.Quantity * quantity,
                        QuantityAvailable = hasStore ? BalanceService.GetBalance(x.ItemId, storeId) : BalanceService.GetBalance(x.ItemId, null, null),
                        ItemCost = _itemService.GetItemCostCalculation(ItemCostCalculateId ?? 0, x.ItemId ),
                        StoreUnderId = storeId,
                        ItemCostCalculateId = ItemCostCalculateId,
                        IsAllQuantityDone = x.Quantity * quantity <= (hasStore ? BalanceService.GetBalance(x.ItemId, storeId) : BalanceService.GetBalance(x.ItemId, null, null)) ? true : false,
                        Actions = n
                    }).ToList();
                    DSItemsMaterial = JsonConvert.SerializeObject(itemsMaterials);
                    return Json(new
                    {
                        data = itemsMaterials
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new
                    {
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetQuantityItem(string itemFinalId, Guid? storeId, bool? allStores) //itemFinal id
        {
            Guid Id;
            int? quentityExpected = 0;

            if (Guid.TryParse(itemFinalId, out Id))
            {
                var itemPro = db.ItemProductions.Where(x => !x.IsDeleted /*&& x.ItemFinalId == Id*/).OrderByDescending(x => x.Id).FirstOrDefault();
                if (itemPro != null)
                {
                    bool hasStore = false;
                    if (storeId != null && storeId != Guid.Empty)
                        hasStore = true;
                    if (allStores == true) //فى حالة اختيار مخزن ولكن تم اختيار (كل المخازن) فى الاتشيكبوكس
                        hasStore = false;

                    var itemsMaterials = itemPro.ItemProductionDetails.Where(x => !x.IsDeleted).Select(x => new ItemProductionQuantity
                    {
                        ItemId = x.ItemId,
                        QuantityItem = x.Quantity,
                        QuantityAvailable = hasStore ? BalanceService.GetBalance(x.ItemId, storeId) : BalanceService.GetBalance(x.ItemId, null, null),
                    }).ToList();

                    quentityExpected = (int)itemsMaterials.Select(x => x.QuantityAvailable / x.QuantityItem).Min();
                    if (quentityExpected < 0)
                        return Json(new { isValid = false, message = "لا يوجد ارصده للانتاج" });
                    else
                        return Json(new { isValid = true, quentityExpected = quentityExpected, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, quentityExpected = quentityExpected, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            else
                return Json(new { isValid = false, quentityExpected = quentityExpected, message = "تأكد من اختيار الصنف " });
        }

    }
}