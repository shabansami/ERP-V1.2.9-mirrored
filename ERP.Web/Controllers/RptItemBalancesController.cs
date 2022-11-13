using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptItemBalancesController : Controller
    {
        // GET: RptItemBalances
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        #region ارصدة الاصناف 

        [HttpGet]
        public ActionResult SearchItemBalance()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");

            return View();
        }
        public ActionResult SearchItemBalanceMovement()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");

            return View();
        }
        public ActionResult GetItemMovement(string itemCode, string barCode, Guid? itemId, int? actionTypeId, Guid? storeId, int isFirstInitPage, DateTime? dtFrom, DateTime? dtTo)
        {
            int? n = null;
            List<ItemMovementdDto> list;

            if (isFirstInitPage == 1)
                return Json(new
                {
                    data = new List<ItemMovementdDto>()
                }, JsonRequestBehavior.AllowGet);
            else
                list = BalanceService.SearchItemMovement(itemCode, barCode, itemId, storeId, actionTypeId, dtFrom, dtTo);
            list = list.OrderBy(x => x.DateReal).ToList();
            double BalanceQuatnitiy = 0;
            double BalanceCost = 0;
            foreach (var item in list)
            {
                BalanceQuatnitiy += (item.IncomingQuantity - item.OutcomingQuantity);
                BalanceCost += (item.IncomingCost - item.OutcomingCost);
                item.BalanceQuantity = BalanceQuatnitiy;
                item.BalanceCost =Math.Round(BalanceCost,2,MidpointRounding.ToEven);
            }
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }
        public ActionResult SearchItemBalances(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId, int isFirstInitPage)
        {
            int? n = null;
            List<ItemBalanceDto> list;
            bool isFirstInit = false;

            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            bool isSearch = false;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                isSearch = true;
            }
            if (isFirstInitPage == 1 && !isSearch)
                return Json(new
                {
                    data = new List<ItemBalanceDto>()
                }, JsonRequestBehavior.AllowGet);
            else
                list = BalanceService.SearchItemBalance(itemCode, barCode, groupId, itemtypeId, itemId, branchId, storeId, isFirstInit, txtSearch);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion

        #region بحث عن صنف بالسيريال 
        public ActionResult SearchItemBySerial(ItemSerial cc, string serial)
        {
            var vm = db.ItemSerials.Where(x => !x.IsDeleted && x.SerialNumber == cc.SerialNumber).OrderBy(x => x.CreatedOn).FirstOrDefault();
            if (vm != null)
                return View(vm);
            else
                return View(new ItemSerial());
        }
        #endregion


        #region حركة صنف 

        [HttpGet]
        public ActionResult SearchItemAction()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");

            return View();
        }
        public ActionResult GetItemAction(string itemCode, string barCode, Guid? itemId, int? actionTypeId, Guid? storeId, int isFirstInitPage, DateTime? dtFrom, DateTime? dtTo)
        {
            int? n = null;
            List<ItemActionDto> list;

            if (isFirstInitPage == 1)
                return Json(new
                {
                    data = new List<ItemActionDto>()
                }, JsonRequestBehavior.AllowGet);
            else
                list = BalanceService.SearchItemActions(itemCode, barCode, itemId, storeId, actionTypeId, dtFrom, dtTo);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

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