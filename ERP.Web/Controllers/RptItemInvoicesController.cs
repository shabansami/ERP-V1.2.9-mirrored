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
using ERP.DAL.Models;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptItemInvoicesController : Controller
    {
        // GET: RptItemInvoices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        [HttpGet]
        public ActionResult SearchInvoices()
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
            //ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }

        #region رصيد اول المدة 
        public ActionResult GetIntial(Guid? branchId, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<ItemIntialBalanceDetail> initialBalanceIQ = null;
            initialBalanceIQ = db.ItemIntialBalanceDetails.Where(x => !x.IsDeleted);
            if (branchId != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => x.Store.BranchId == branchId);
            if (itemId != null)
                initialBalanceIQ = initialBalanceIQ.Where(x => x.ItemId == itemId);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                initialBalanceIQ = initialBalanceIQ.Where(x => DbFunctions.TruncateTime(x.ItemIntialBalance.DateIntial) >= dtFrom && DbFunctions.TruncateTime(x.ItemIntialBalance.DateIntial) <= dtTo);

            var initialList = initialBalanceIQ.GroupBy(x => x.ItemId).ToList();
            var data = initialList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty(0).Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
            }).ToList();

            return Json(new
            {
                data = data,
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion
        #region فواتير المبيعات 
        public ActionResult GetSellInvo(Guid? branchId, bool? isFinalApproval, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<SellInvoicesDetail> sell = db.SellInvoicesDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                sell = sell.Where(x => x.SellInvoice.IsFinalApproval);
            if (branchId != null)
                sell = sell.Where(x => x.SellInvoice.BranchId == branchId);

            //if (customerId != null)
            //    sell = sell.Where(x => x.SellInvoice.CustomerId == customerId);

            //if (employeeId != null)
            //    sell = sell.Where(x => x.SellInvoice.EmployeeId == employeeId);

            if (itemId != null)
                sell = sell.Where(x => x.ItemId == itemId);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sell = sell.Where(x => x.SellInvoice.InvoiceDate >= dtFrom && x.SellInvoice.InvoiceDate <= dtTo);

            var sellList = sell.GroupBy(x => x.ItemId).ToList();
            var data = sellList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty(0).Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
                InvoicesData = x.Select(y => new InvoiceData
                {
                    SellInvoiceId = y.SellInvoiceId,
                    InvoiceNum = y.SellInvoiceId,
                    CustomerName = y.SellInvoice.PersonCustomer.Name,
                    InvoiceDate = y.SellInvoice.InvoiceDate.ToString(),
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
        #region فواتير مرتجع المبيعات 
        public ActionResult GetSellBackInvo(Guid? branchId, bool? isFinalApproval, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<SellBackInvoicesDetail> sellBack = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                sellBack = sellBack.Where(x => x.SellBackInvoice.IsFinalApproval);
            if (branchId != null)
                sellBack = sellBack.Where(x => x.SellBackInvoice.BranchId == branchId);

            //if (customerId != null)
            //    sellBack = sellBack.Where(x => x.SellBackInvoice.CustomerId == customerId);

            //if (employeeId != null)
            //    sellBack = sellBack.Where(x => x.SellBackInvoice.EmployeeId == employeeId);

            if (itemId != null)
                sellBack = sellBack.Where(x => x.ItemId == itemId);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sellBack = sellBack.Where(x => x.SellBackInvoice.InvoiceDate >= dtFrom && x.SellBackInvoice.InvoiceDate <= dtTo);

            var sellList = sellBack.GroupBy(x => x.ItemId).ToList();
            var data = sellList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty(0).Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
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
        #region فواتير التوريدات 
        public ActionResult GetPurchaseInvo(Guid? branchId, bool? isFinalApproval, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<PurchaseInvoicesDetail> Purchase = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                Purchase = Purchase.Where(x => x.PurchaseInvoice.IsFinalApproval);
            if (branchId != null)
                Purchase = Purchase.Where(x => x.PurchaseInvoice.BranchId == branchId);

            if (itemId != null)
                Purchase = Purchase.Where(x => x.ItemId == itemId);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                Purchase = Purchase.Where(x => x.PurchaseInvoice.InvoiceDate >= dtFrom && x.PurchaseInvoice.InvoiceDate <= dtTo);

            var PurchaseList = Purchase.GroupBy(x => x.ItemId).ToList();
            var data = PurchaseList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty(0).Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
                InvoicesData = x.Select(y => new InvoiceData
                {
                    SellInvoiceId = y.PurchaseInvoiceId,
                    InvoiceNum = y.PurchaseInvoiceId,
                    CustomerName = y.PurchaseInvoice.PersonSupplier.Name,
                    InvoiceDate = y.PurchaseInvoice.InvoiceDate.ToString(),
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
        #region فواتير مرتجع التوريد 
        public ActionResult GetPurchaseBackInvo(Guid? branchId, bool? isFinalApproval, Guid? itemId, string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            IQueryable<PurchaseBackInvoicesDetail> PurchaseBack = db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                PurchaseBack = PurchaseBack.Where(x => x.PurchaseBackInvoice.IsFinalApproval);
            if (branchId != null)
                PurchaseBack = PurchaseBack.Where(x => x.PurchaseBackInvoice.BranchId == branchId);

            if (itemId != null)
                PurchaseBack = PurchaseBack.Where(x => x.ItemId == itemId);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                PurchaseBack = PurchaseBack.Where(x => x.PurchaseBackInvoice.InvoiceDate >= dtFrom && x.PurchaseBackInvoice.InvoiceDate <= dtTo);

            var PurchaseList = PurchaseBack.GroupBy(x => x.ItemId).ToList();
            var data = PurchaseList.Select(x => new RptItemInvoiceDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalQuantity = x.Select(y => y.Quantity).DefaultIfEmpty(0).Sum(),
                TotalAmount = x.Select(y => y.Amount).DefaultIfEmpty(0).Sum(),
                InvoicesData = x.Select(y => new InvoiceData
                {
                    SellInvoiceId = y.PurchaseBackInvoiceId,
                    InvoiceNum = y.PurchaseBackInvoiceId,
                    CustomerName = y.PurchaseBackInvoice.PersonSupplier.Name,
                    InvoiceDate = y.PurchaseBackInvoice.InvoiceDate.ToString(),
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