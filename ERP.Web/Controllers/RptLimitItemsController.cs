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
    public class RptLimitItemsController : Controller
    {
        // GET: RptLimitItems
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        #region االاصناف التى بلغت حد الطلب الامان 

        [HttpGet]
        public ActionResult SearchItemSafety()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");


            return View();
        }
        public ActionResult SearchSafety(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? BranchId, Guid? StoreId)
        {
            List<ItemBalanceDto> list;
            list = BalanceService.ItemRequestLimitSafety(itemCode, barCode, groupId, itemtypeId, itemId, BranchId, StoreId);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion
        #region االاصناف التى بلغت حد الطلب الخطر 

        [HttpGet]
        public ActionResult SearchItemDanger()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");

            return View();
        }
        public ActionResult SearchDanger(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? BranchId, Guid? StoreId)
        {
            List<ItemBalanceDto> list;
            list = BalanceService.ItemRequestLimitDanger(itemCode, barCode, groupId, itemtypeId, itemId, BranchId, StoreId);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion

        #region الاصناف الاعلى والاقل مبيعا
        [HttpGet]
        public ActionResult ItemsTopLowestSelling()
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
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", 1);
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");

            return View();
        }
        public ActionResult SearchTopLowestSelling(Guid? CustomerId, Guid? BranchId, int? TypeSellingId, string dtFrom, string dtTo)
        {
            List<TopLowestSellingDto> list;
            list = BalanceService.SearchTopLowestSelling(CustomerId, BranchId, TypeSellingId, dtFrom, dtTo);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }
        #endregion
    }
}