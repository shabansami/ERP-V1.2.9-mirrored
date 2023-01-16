using ERP.DAL;
using ERP.DAL.Utilites;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptProductionLinesController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        // GET: RptProductionLines
        #region تقرير موظفى خطوط الانتاج 

        [HttpGet]
        public ActionResult SearchProductionOrderEmployee()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion
            return View();
        }
        public ActionResult GetProductionEmp(Guid? branchId, Guid? employeeId, DateTime? dtFrom, DateTime? dtTo)
        {
            int? n = null;
            List<RptProductionLineDto> list;
                list = ProductionOrderService.GetProductionOrderByEmployee(branchId,employeeId, dtFrom, dtTo);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion
        #region تقرير خطوط الانتاج 

        [HttpGet]
        public ActionResult SearchProductionLine()
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
            ViewBag.ProductionLineId = new SelectList(db.ProductionLines.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetProductionLine(Guid? branchId, Guid? productionLineId, DateTime? dtFrom, DateTime? dtTo)
        {
            int? n = null;
            List<RptProductionLineDto> list;
                list = ProductionOrderService.GetProductionOrderByProLine(branchId,productionLineId, dtFrom, dtTo);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }

        #endregion

    }
}