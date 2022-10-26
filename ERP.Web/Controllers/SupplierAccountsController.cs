﻿using ERP.Web.Identity;
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

    public class SupplierAccountsController : Controller
    {
        // GET: SupplierAccounts
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        SupplierService supplierService = new SupplierService(); 

        #region  كشف حساب مورد  
        public ActionResult Index(SupplierAccountVM vm)
        {

            Guid? suppId = null;
            suppId = vm.SupplierId;
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", suppId);

            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(vm.DtFrom, out dtFrom) && DateTime.TryParse(vm.DtTo, out dtTo))
                vm = supplierService.GetSupplierAccount(vm.SupplierId, dtFrom, dtTo, vm.AllTimes, vm.IsFinalApproval);
            else
                vm = supplierService.GetSupplierAccount(vm.SupplierId, null, null, vm.AllTimes, vm.IsFinalApproval);

            return View(vm);
        }
        #endregion

    }
}