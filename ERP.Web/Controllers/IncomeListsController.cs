using ERP.Web.DataTablesDS;
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

    public class IncomeListsController : Controller
    {
        // GET: IncomeLists
        VTSaleEntities db = new VTSaleEntities();
        [HttpGet]
        public ActionResult Index()
        {
            List<IncomeListDto> vm = new List<IncomeListDto>();

            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                var financialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                 var financialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;

                ViewBag.FinancialYearStartDate = financialYearStartDate;
                ViewBag.FinancialYearEndDate = financialYearEndDate;
                //vm = ReportsAccountTreeService.IncomeLists(DateTime.Parse(financialYearStartDate), DateTime.Parse(financialYearEndDate));

            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.AssetsDepreciationStatus = new SelectList(new List<DropDownListInt>
            {
                new DropDownListInt {Id=1,Name="ضمن المصروفات"},
                new DropDownListInt {Id=2,Name="منفصل عن المصروفات"},
            }, "Id", "Name");
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(string dFrom, string dTo,int? AssetsDepreciationStatus)
        {
            DateTime dtFrom, dtTo;
            List<IncomeListDto> vm=new List<IncomeListDto>();
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                vm =ReportsAccountTreeService.IncomeLists(dtFrom, dtTo, AssetsDepreciationStatus);
            }
            else
                vm = ReportsAccountTreeService.IncomeLists(null, null, AssetsDepreciationStatus);

            ViewBag.AssetsDepreciationStatus = new SelectList(new List<DropDownListInt>
            {
                new DropDownListInt {Id=1,Name="ضمن المصروفات"},
                new DropDownListInt {Id=2,Name="منفصل عن المصروفات"},
            },"Id","Name",AssetsDepreciationStatus);
            return View(vm);

        }
    }
}