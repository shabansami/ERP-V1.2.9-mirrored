using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptDueInvoicesController : Controller
    {
        // GET: RptDueInvoices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        #region فواتير المتأخر تحصيلها 

        [HttpGet]
        public ActionResult SearchDueSell()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", 1);
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

            return View();
        }
        public ActionResult GetDueSellInvo(Guid? branchId, Guid? customerId, Guid? employeeId, bool? allCustomers)
        {
            int? n = null;
            var dateNow = Utility.GetDateTime().Date;

            IQueryable<SellInvoice> sell = db.SellInvoices.Where(x => !x.IsDeleted&&x.IsFinalApproval&&x.DueDate != null);
            //if (isFinalApproval == true)
            //    sell = sell.Where(x => x.IsFinalApproval);
            if (branchId != null)
                sell = sell.Where(x => x.BranchId == branchId);
            if (allCustomers==false)
            {
            if (customerId != null)
                sell = sell.Where(x => x.CustomerId == customerId);
            }

            if (employeeId != null)
                sell = sell.Where(x => x.EmployeeId == employeeId);

            //فواتير البيع التى مر تاريخ استحقاقها 
                //sell = sell.Where(x => dateNow > x.DueDate.Value&&x.RemindValue>0 &&PaymentServices.GetAmount(x.Id,x.CustomerId)!=x.Safy);
            var sellList = sell.ToList();

            var data = sellList.Where(x => dateNow > x.DueDate.Value && x.RemindValue > 0 && PaymentServices.GetAmount(x.Id, x.CustomerId) != x.Safy);

            return Json(new
            {
                data = data.Select(x => new { Id = x.Id,  InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, EmployeeName = x.Employee!=null?x.Employee.Person.Name:null, AmountCollected= PaymentServices.GetAmount(x.Id, x.CustomerId), Actions = n, Num = n }).ToList()
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