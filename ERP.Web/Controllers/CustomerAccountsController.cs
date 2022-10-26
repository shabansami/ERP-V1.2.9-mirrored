using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class CustomerAccountsController : Controller
    {
        // GET: CustomerAccounts
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        CustomerService customerService = new CustomerService();

        #region  كشف حساب عميل  
        public ActionResult Index(CustomerAccountVM vm)
        {

            Guid? custId = null;
            custId = vm.CustomerId;
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", custId);

            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(vm.DtFrom, out dtFrom) && DateTime.TryParse(vm.DtTo, out dtTo))
                vm = customerService.GetCustomerAccount(vm.CustomerId, dtFrom, dtTo, vm.AllTimes, vm.IsFinalApproval);
            else
                vm = customerService.GetCustomerAccount(vm.CustomerId, null, null, vm.AllTimes, vm.IsFinalApproval);

            return View(vm);
        }
        #endregion

    }
}