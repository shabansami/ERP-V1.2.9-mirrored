﻿using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Services.ReportsAccountTreeService;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class AuditBalancesController : Controller
    {
        // GET: AuditBalances ميزان المراجعة
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.AccountLevel = new SelectList(db.AccountsTrees.Where(x => !x.IsDeleted).Select(x => new {  x.AccountLevel }).Distinct(), "AccountLevel", "AccountLevel");
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");

            return View();
            
        }
        public ActionResult GetAll(string dFrom, string dTo, int? accountlevel, Guid? branchId,bool? StatusVal)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<AuditBalanceDto> list;
            if (DateTime.TryParse(dFrom,out dtFrom)&&DateTime.TryParse(dTo,out dtTo))
            {
                list = AuditBalances(dtFrom, dtTo, accountlevel, null, branchId, StatusVal);
            }
            else
                list = AuditBalances(null, null, accountlevel, null, branchId, StatusVal);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

    }
}