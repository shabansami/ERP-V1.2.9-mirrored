using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoiceInstallmentNotPaidController : Controller
    {
        // GET: SellInvoiceInstallmentNotPaid
        VTSaleEntities db;
        VTSAuth auth;
        SellInvoiceInstallmentService installmentService;
        public SellInvoiceInstallmentNotPaidController()
        {
                db= new VTSaleEntities(); 
                auth= new VTSAuth();
            installmentService=new SellInvoiceInstallmentService();
        }

        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            var list = installmentService.InstallmentNotPaid();

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

    }
}