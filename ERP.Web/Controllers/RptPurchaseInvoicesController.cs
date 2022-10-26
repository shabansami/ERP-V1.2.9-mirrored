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
    public class RptPurchaseInvoicesController : Controller
    {
        // GET: RptPurchaseInvoices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        #region فواتير التوريد 

        [HttpGet]
        public ActionResult SearchPurchases()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";
            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name",1);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetPurchases(Guid? branchId, Guid? supplierId, Guid? employeeId, bool? isFinalApproval, Guid? itemId, bool? isDiscount, double? purchaseAmount, string dFrom, string dTo, int? PaymentTypeId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<PurchaseInvoice> purchase = db.PurchaseInvoices.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                purchase = purchase.Where(x => x.IsFinalApproval);
            if (branchId != null)
                purchase = purchase.Where(x => x.BranchId == branchId);

            if (supplierId != null)
                purchase = purchase.Where(x => x.SupplierId == supplierId);

            if (itemId != null)
                purchase = purchase.Where(x => x.PurchaseInvoicesDetails.Where(d => !d.IsDeleted && d.ItemId == itemId).Any());

            if (PaymentTypeId != null)
                purchase = purchase.Where(x => x.PaymentTypeId == PaymentTypeId);

            if (isDiscount == true)
                purchase = purchase.Where(x => x.TotalDiscount > 0);

            if (purchaseAmount != null && purchaseAmount > 0)
                purchase = purchase.Where(x => x.PurchaseInvoicesDetails.Where(d => !d.IsDeleted && d.Amount == purchaseAmount).Any());

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                purchase = purchase.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);


            return Json(new
            {
                data = purchase.Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, PaymentTypeName = x.PaymentType.Name, TotalQuantity = x.TotalQuantity, TotalValue = x.TotalValue, CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region فواتير مرتجع التوريد 

        [HttpGet]
        public ActionResult SearchBackPurchases()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";
            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name",1);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetBackPurchases(Guid? branchId, Guid? supplierId, Guid? employeeId, bool? isFinalApproval, Guid? itemId, bool? isDiscount, double? purchaseAmount, string dFrom, string dTo, int? PaymentTypeId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<PurchaseBackInvoice> purchaseBack = db.PurchaseBackInvoices.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                purchaseBack = purchaseBack.Where(x => x.IsFinalApproval);
            if (branchId != null)
                purchaseBack = purchaseBack.Where(x => x.BranchId == branchId);

            if (supplierId != null)
                purchaseBack = purchaseBack.Where(x => x.SupplierId == supplierId);

            if (itemId != null)
                purchaseBack = purchaseBack.Where(x => x.PurchaseBackInvoicesDetails.Where(d => !d.IsDeleted && d.ItemId == itemId).Any());

            if (PaymentTypeId != null)
                purchaseBack = purchaseBack.Where(x => x.PaymentTypeId == PaymentTypeId);

            if (isDiscount == true)
                purchaseBack = purchaseBack.Where(x => x.TotalDiscount > 0);

            if (purchaseAmount != null && purchaseAmount > 0)
                purchaseBack = purchaseBack.Where(x => x.PurchaseBackInvoicesDetails.Where(d => !d.IsDeleted && d.Amount == purchaseAmount).Any());

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                purchaseBack = purchaseBack.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);


            return Json(new
            {
                data = purchaseBack.Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, PaymentTypeName = x.PaymentType.Name, TotalQuantity = x.TotalQuantity, TotalValue = x.TotalValue, CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);


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