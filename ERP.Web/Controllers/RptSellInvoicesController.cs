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

    public class RptSellInvoicesController : Controller
    {
        // GET: RptSellInvoices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region فواتير المبيعات 

        [HttpGet]
        public ActionResult SearchSell()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetSellInvo(Guid? branchId, Guid? customerId, Guid? employeeId, bool? isFinalApproval, Guid? itemId,bool? isDiscount,double? sellAmount, string dFrom, string dTo, int? PaymentTypeId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<SellInvoice> sell = db.SellInvoices.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                sell = sell.Where(x => x.IsFinalApproval);
            if (branchId != null)
                sell = sell.Where(x => x.BranchId == branchId);

            if (customerId != null)
                sell = sell.Where(x => x.CustomerId == customerId);

            if (employeeId != null)
                sell = sell.Where(x => x.EmployeeId == employeeId);

            if (itemId !=null)
                sell = sell.Where(x => x.SellInvoicesDetails.Where(d => !d.IsDeleted && d.ItemId == itemId).Any());

            if (PaymentTypeId != null)
                sell = sell.Where(x => x.PaymentTypeId == PaymentTypeId);

            if (isDiscount ==true)
                sell = sell.Where(x => x.TotalDiscount>0);

            if (sellAmount != null&&sellAmount>0)
                sell = sell.Where(x => x.SellInvoicesDetails.Where(d=>!d.IsDeleted&&d.Amount==sellAmount).Any() );

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sell = sell.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);


            return Json(new
            {
                data = sell.Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, PaymentTypeName = x.PaymentType.Name, TotalQuantity = x.TotalQuantity, TotalValue = x.TotalValue, CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); 

                //return Json(new
                //{
                //    data = new { }
                //}, JsonRequestBehavior.AllowGet); ;


        }

        #endregion

        #region فواتير مرتجع المبيعات 

        [HttpGet]
        public ActionResult SearchBackSell()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetSellBackInvo(Guid? branchId, Guid? customerId, Guid? employeeId, bool? isFinalApproval, Guid? itemId, bool? isDiscount, double? sellAmount, string dFrom, string dTo, int? PaymentTypeId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;

            IQueryable<SellBackInvoice> sellBack = db.SellBackInvoices.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                sellBack = sellBack.Where(x => x.IsFinalApproval);
            if (branchId != null)
                sellBack = sellBack.Where(x => x.BranchId == branchId);

            if (customerId != null)
                sellBack = sellBack.Where(x => x.CustomerId == customerId);

            if (employeeId != null)
                sellBack = sellBack.Where(x => x.EmployeeId == employeeId);

            if (itemId != null)
                sellBack = sellBack.Where(x => x.SellBackInvoicesDetails.Where(d => !d.IsDeleted && d.ItemId == itemId).Any());

            if (PaymentTypeId != null)
                sellBack = sellBack.Where(x => x.PaymentTypeId == PaymentTypeId);

            if (isDiscount == true)
                sellBack = sellBack.Where(x => x.TotalDiscount > 0);

            if (sellAmount != null && sellAmount > 0)
                sellBack = sellBack.Where(x => x.SellBackInvoicesDetails.Where(d => !d.IsDeleted && d.Amount == sellAmount).Any());


            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                sellBack = sellBack.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);


            return Json(new
            {
                data = sellBack.Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, PaymentTypeName = x.PaymentType.Name, TotalQuantity = x.TotalQuantity, TotalValue = x.TotalValue, CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
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