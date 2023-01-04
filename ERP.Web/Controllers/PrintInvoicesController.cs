﻿using ERP.DAL.Utilites;
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
        public ActionResult ShowPrintInvoice(string id, string typ)
        {
            //اسطر الطباعه من الاعدادات 
            var list = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
            ViewBag.EntityData = list;

            PrintInvoiceDto vm = new PrintInvoiceDto();
            if (string.IsNullOrEmpty(id))
                return View(vm);
            vm= printInvoiceService.GetInvoiceData(id,typ); 
            return View(vm);

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