using ERP.DAL.Utilites;
using ERP.DAL;
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

    public class RptItemsController : Controller
    {
        // GET: RptItems
        VTSaleEntities db;
        ItemService itemService;
        public RptItemsController()
        {
            db = new VTSaleEntities();
            itemService = new ItemService();    
        }
         
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        #region كميات  الاصناف المباعه خلال فترة 

        [HttpGet]
        public ActionResult SearchItemSell()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
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
        public ActionResult GetItemSell(Guid? branchId, DateTime? dtFrom,DateTime? dtTo)
        {
            List<RptItemSellDto> list=new List<RptItemSellDto>();
            if (branchId!=null)
              list = itemService.GetItemSell(branchId, dtFrom, dtTo);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

    }
}