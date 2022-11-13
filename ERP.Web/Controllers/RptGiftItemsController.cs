using ERP.DAL;
using ERP.DAL.Utilites;
using ERP.Web.Identity;
using ERP.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class RptGiftItemsController : Controller
    {
        // GET: RptGiftItems
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService itemService;
        public RptGiftItemsController()
        {
            db = new VTSaleEntities();
            itemService = new ItemService();
        }

        #region اصناف عينات وبونص من مورد 
        [HttpGet]
        public ActionResult SearchItemGiftPurchase()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetItemGiftPurchases(Guid? branchId, Guid? supplierId, bool? isFinalApproval, Guid? itemTypeId, Guid? itemId, string dFrom, string dTo)
        {
            var list = itemService.GetItemPurchaseGifts(db, branchId, supplierId, itemTypeId, itemId, dFrom, dTo, isFinalApproval);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion
        #region اصناف عينات وبونص لعميل 
        [HttpGet]
        public ActionResult SearchItemGiftSell()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetItemGiftSells(Guid? branchId, Guid? customerId, bool? isFinalApproval, Guid? itemTypeId, Guid? itemId, string dFrom, string dTo)
        {
            var list = itemService.GetItemSellGifts(db, branchId, customerId, itemTypeId, itemId, dFrom, dTo, isFinalApproval);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}