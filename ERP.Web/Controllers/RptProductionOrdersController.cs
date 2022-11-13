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
using System.Data.Entity;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptProductionOrdersController : Controller
    {
        // GET: RptProductionOrders
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        [HttpGet]
        public ActionResult SearchProductionOrders()
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
            ViewBag.ColorId = new SelectList(db.ProductionOrderColors.Where(x => !x.IsDeleted), "Id", "Name");

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }

        #region اوامر الانتاج 
        public ActionResult GetProductionOrders(Guid? branchId, Guid? colorId, string OrderNumber, bool? isDone, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<ProductionOrder> productionOrders = db.ProductionOrders.Where(x => !x.IsDeleted);
            
            if (OrderNumber != null)
                productionOrders = productionOrders.Where(x => x.OrderNumber == OrderNumber);
           
            if (isDone == true)
                productionOrders = productionOrders.Where(x => x.IsDone);
            if (branchId != null)
                productionOrders = productionOrders.Where(x => x.BranchId == branchId);

            if (colorId != null)
                productionOrders = productionOrders.Where(x => x.OrderColorId == colorId);

            //if (itemId != null)
            //    productionOrders = productionOrders.Where(x => x.FinalItemId == itemId
            //    ||x.ProductionOrderDetails.Where(y=>!y.IsDeleted&&y.ItemId==itemId).Any());

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                productionOrders = productionOrders.Where(x => DbFunctions.TruncateTime(x.ProductionOrderDate) >= dtFrom && DbFunctions.TruncateTime(x.ProductionOrderDate) <= dtTo);

            var productionOrdersList = productionOrders.ToList();
            var data = productionOrdersList.Select(x => new RptProductionOrderDto
            {
                Id = x.Id,
                OrderNumber=x.OrderNumber,
                ProductionOrderDate=x.ProductionOrderDate.ToString("yyyy-MM-dd"),
                //FinalItemName=x.FinalItem.Name,
                //FinalItemCost=Math.Round(x.FinalItemCost,2,MidpointRounding.ToEven),
                IsDone=x.IsDone,
                //MaterialItemCost= Math.Round(x.MaterialItemCost, 2, MidpointRounding.ToEven),
                //OrderQuantity=x.OrderQuantity,
                TotalCost=x.TotalCost,
                //Actions=null,
                MaterialItems= x.ProductionOrderDetails.Where(y=>!y.IsDeleted).Select(y=> new MaterialItem
                {
                    ItemName=y.Item.Name,
                    Quantity=y.Quantity,
                    Quantitydamage=y.Quantitydamage,
                    ItemCost=y.ItemCost

                }).ToList()
            }).ToList();

            return Json(new
            {
                data = data,
            }, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    data = new { }
            //}, JsonRequestBehavior.AllowGet); ;


        }

        #endregion
        #region فواتير مرتجع المبيعات 
        public ActionResult GetSellBackInvo(Guid? branchId, Guid? customerId, Guid? employeeId, bool? isFinalApproval, Guid? itemId, bool? isDiscount, double? sellAmount, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<SellBackInvoicesDetail> sellBack = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                sellBack = sellBack.Where(x => x.SellBackInvoice.IsFinalApproval);
            if (branchId != null)
                sellBack = sellBack.Where(x => x.SellBackInvoice.BranchId == branchId);

            if (customerId != null)
                sellBack = sellBack.Where(x => x.SellBackInvoice.CustomerId == customerId);

            if (employeeId != null)
                sellBack = sellBack.Where(x => x.SellBackInvoice.EmployeeId == employeeId);

            if (itemId != null)
                sellBack = sellBack.Where(x => x.ItemId == itemId);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sellBack = sellBack.Where(x => x.SellBackInvoice.InvoiceDate >= dtFrom && x.SellBackInvoice.InvoiceDate <= dtTo);

            var sellList = sellBack.GroupBy(x => x.ItemId).ToList();
            var data = sellList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty().Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty().Sum(),
                InvoicesData = x.Select(y => new InvoiceData
                {
                    SellInvoiceId = y.SellBackInvoiceId,
                    InvoiceNum = y.SellBackInvoiceId,
                    CustomerName = y.SellBackInvoice.PersonCustomer.Name,
                    InvoiceDate = y.SellBackInvoice.InvoiceDate.ToString(),
                    ItemAmount = y.Amount

                }).ToList()
            }).ToList();

            return Json(new
            {
                data = data,
            }, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    data = new { }
            //}, JsonRequestBehavior.AllowGet); ;


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