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

    public class RptQuantityDamagesController : Controller
    {
        // GET: RptQuantityDamages
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

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
            ViewBag.ProductionOrderId = new SelectList(db.ProductionOrders.Where(x => !x.IsDeleted).Select(x=>new {Id=x.Id,Name="أمر الانتاج رقم ("+x.OrderNumber+")" }), "Id", "Name");

            return View();

        }
        public ActionResult GetData(Guid? productionOrderId,  string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                IQueryable<ProductionOrderDetail> productionOrderDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.ProductionOrder.ProductionOrderDate >= dtFrom && x.ProductionOrder.ProductionOrderDate <= dtTo);
                if (productionOrderId != null)
                    productionOrderDetails = productionOrderDetails.Where(x => x.ProductionOrderId == productionOrderId);
                var data = productionOrderDetails.Where(x=>x.Quantitydamage != 0)
                    .GroupBy(x => new { x.ItemId }).Select(x => new { ItemId = x.Key, ItemName=x.FirstOrDefault().Item.Name, TotalQuantity = x.Sum(s=>s.Quantitydamage), TotalDamageCost = x.Sum(d=>d.Quantitydamage*d.ItemCost), Num = n }).ToList();

                return Json(new
                {
                    data = data
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet); ;


        }

    }
}