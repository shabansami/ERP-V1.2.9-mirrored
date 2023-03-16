using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Identity;
using ERP.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.DataTablesDS;
using ERP.Web.ViewModels;
using BarcodeLib;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PrintInvoicesController : Controller
    {
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        PrintInvoiceService printInvoiceService;
        public PrintInvoicesController()
        {
            db = new VTSaleEntities();
            printInvoiceService = new PrintInvoiceService();
        }

        // GET: PrintInvoices

        #region طباعه وعرض فاتورة  
        public ActionResult ShowPrintInvoice(string id, string typ,string lang)
        {
            //اسطر الطباعه من الاعدادات 
            var list = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
            ViewBag.EntityData = list;

            PrintInvoiceDto vm = new PrintInvoiceDto();
            if (string.IsNullOrEmpty(id))
                return View(vm);
            vm= printInvoiceService.GetInvoiceData(id,typ,lang);
            if (lang == "en")
            {
                vm.LanguagePage = "en";
                vm.ToggleUrl = $"/PrintInvoices/ShowPrintInvoice/?id={id}&typ={typ}&lang=ar";
            }
            else
            {
                vm.LanguagePage = "ar";
                vm.ToggleUrl = $"/PrintInvoices/ShowPrintInvoice/?id={id}&typ={typ}&lang=en";
            }
            return View(vm);

        }
        #endregion
        #region طباعه السندات وقيود اليومية  
        public ActionResult PrintGeneralRecord(string id, string typ)
        {
            //اسطر الطباعه من الاعدادات 
            var list = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
            ViewBag.EntityData = list;

            PrintGeneralRecordDto vm = new PrintGeneralRecordDto();
            if (string.IsNullOrEmpty(id))
                return View(vm);
            vm= printInvoiceService.PrintGeneralRecordData(id,typ); 
            return View(vm);

        }
        #endregion

        #region طباعه وعرض كشف حساب  
        public ActionResult PrintRptAccountStatements(string accountId,string dFrom,string dTo, string lang="ar")
        {
            //اسطر الطباعه من الاعدادات 
            var list = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
            ViewBag.EntityData = list;
            if (DateTime.TryParse(dFrom, out DateTime dtFrom) && DateTime.TryParse(dTo, out DateTime dtTo) &&Guid.TryParse(accountId ,out Guid acccountId))
            {
                var isEn=lang== "en"?true:false;
               var accountGeneralDailies = GeneralDailyService.SearchAccountGeneralDailies(dtFrom, dtTo, acccountId, 0, isEn);
                if (lang == "en")
                {
                    accountGeneralDailies.ShowRptEn = true;
                    accountGeneralDailies.ToggleUrl = $"/PrintInvoices/PrintRptAccountStatements/?accountId={accountId}&dFrom={dFrom}&dTo={dTo}&lang=ar";

                }
                else
                {
                    accountGeneralDailies.ShowRptEn = false;
                    accountGeneralDailies.ToggleUrl = $"/PrintInvoices/PrintRptAccountStatements/?accountId={accountId}&dFrom={dFrom}&dTo={dTo}&lang=en";

                }
                return View(accountGeneralDailies);
            }
            else
                return View(new GeneralDayAccountVM());


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