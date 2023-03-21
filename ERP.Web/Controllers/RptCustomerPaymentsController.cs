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

    public class RptCustomerPaymentsController : Controller
    {
        // GET: RptCustomerPayments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region التحصيلات النقدية 

        [HttpGet]
        public ActionResult SearchPayments()
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
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");

            return View();
        }
        public ActionResult GetPayments(Guid? branchId, Guid? customerId, Guid? employeeId, bool? isFinalApproval, Guid? areaId, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<CustomerPayment> payments = db.CustomerPayments.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                payments = payments.Where(x => x.IsApproval);
            if (branchId != null)
                payments = payments.Where(x => x.BranchId == branchId);

            if (customerId != null)
                payments = payments.Where(x => x.CustomerId == customerId);

            if (employeeId != null)
                payments = payments.Where(x => x.EmployeeId == employeeId);

            if (areaId != null)
                payments = payments.Where(x => x.PersonCustomer.AreaId == areaId);


            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                payments = payments.Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo);
           

            return Json(new
            {
                data = payments.Select(x => new { Id = x.Id,  InvoiceNum = x.Id, PaymentDate = x.PaymentDate.ToString(), CustomerName = x.PersonCustomer.Name, Amount = x.Amount, SafeName = x.Safe!=null?x.Safe.Name:"لم تحدد بعد",  Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    data = new { }
            //}, JsonRequestBehavior.AllowGet); ;


        }

        #endregion
        #region الشيكات 

        [HttpGet]
        public ActionResult SearchCheques()
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
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", 1);
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");

            return View();
        }
        public ActionResult GetCheques(Guid? customerId, Guid? employeeId, Guid? areaId, bool? isCollected, bool? isRefused, Guid? bankAccountId, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<Cheque> cheques = db.Cheques.Where(x => !x.IsDeleted);
            if (isCollected == true)
                cheques = cheques.Where(x => x.IsCollected);

            if (isRefused == true)
                cheques = cheques.Where(x => x.IsRefused);

            if (customerId != null)
                cheques = cheques.Where(x => x.CustomerId == customerId);
           
            if (areaId != null)
                cheques = cheques.Where(x => x.PersonCustomer.AreaId == areaId);


            if (employeeId != null)
                cheques = cheques.Where(x => x.EmployeeId == employeeId);

            if (bankAccountId != null)
                cheques = cheques.Where(x => x.BankAccountId == bankAccountId);


            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                cheques = cheques.Where(x => x.CheckDate >= dtFrom && x.CheckDate <= dtTo);


            return Json(new
            {
                data = cheques.Select(x => new { Id = x.Id,  CheckDate = x.CheckDate.ToString(), CustomerName = x.PersonCustomer.Name, CheckNumber = x.CheckNumber, Amount = x.Amount,  Actions = n, Num = n }).ToList()
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