using ERP.Web.DataTablesDS;
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

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoiceHasInstallmentsController : Controller
    {
        // GET: SellInvoiceHasInstallments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        SellInvoiceInstallmentService installmentService = new SellInvoiceInstallmentService();
        public ActionResult Index()
        {
            //ViewBag.StartDate = Utility.GetDateTime().Date;
            //ViewBag.EndDate = Utility.GetDateTime().AddMonths(1).Date;

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }

        public ActionResult GetAll(string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<SellInvoiceInstallmentDto> list;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = installmentService.GetSellInvoiceInstallments(dtFrom, dtTo, customerId, true);
            }
            else
                list = installmentService.GetSellInvoiceInstallments(null, null, customerId, true);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;


        }

        [HttpGet]
        public ActionResult Installments(string invoGuid, string typ)
        {
            if (!string.IsNullOrEmpty(typ))
            {
                if (typ.Trim() == "sell")
                {
                    Guid sellGuid;
                    if (Guid.TryParse(invoGuid, out sellGuid))
                    {
                        var sell = db.SellInvoices.Where(x => x.Id == sellGuid).FirstOrDefault();
                        SellInvoiceInstallmentScheduleCollectionVM vm = new SellInvoiceInstallmentScheduleCollectionVM();
                        vm = installmentService.GetInstallmentSchedulesCollections(sellGuid, null);
                        return View(vm);

                    }
                    else
                        return RedirectToAction("Index");
                }
                else if (typ.Trim() == "initial")
                {
                    Guid custInitialId;
                    if (Guid.TryParse(invoGuid, out custInitialId))
                    {
                        var custInitial = db.PersonIntialBalances.Where(x => x.Id == custInitialId).FirstOrDefault();
                        SellInvoiceInstallmentScheduleCollectionVM vm = new SellInvoiceInstallmentScheduleCollectionVM();
                        vm = installmentService.GetInstallmentSchedulesCollections(null, custInitialId);
                        return View(vm);

                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

        }

    }
}