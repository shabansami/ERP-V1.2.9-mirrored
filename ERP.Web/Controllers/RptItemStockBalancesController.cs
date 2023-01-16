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
using ERP.Web.Identity;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class RptItemStockBalancesController : Controller
    {
        // GET: RptItemStockBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService _itemService;
        public RptItemStockBalancesController()
        {
            db = new VTSaleEntities();
            _itemService = new ItemService();
        }
        #region تقرير المخزون 
        public ActionResult Index(ItemStockBalanceVM vm)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            //if(vm.CostCalculationSellId!=null)
            selectListItems = new List<SelectListItem> {
                new SelectListItem { Text = "متوسط سعر البيع", Value = "1", Selected = true },
                new SelectListItem { Text = "اخر سعر بيع", Value = "2" },
                new SelectListItem { Text = "اعلى سعر بيع", Value = "3" },
                new SelectListItem { Text = "اقل سعر بيع", Value = "4" } };
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.CostCalculationSellId = new SelectList(selectListItems, "Value", "Text", vm.CostCalculationSellId);
            ViewBag.CostCalculationPurchaseId = new SelectList(db.ItemCostCalculations.Where(x => !x.IsDeleted), "Id", "Name", vm.CostCalculationPurchaseId);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted && x.Id == vm.ItemId), "Id", "Name", vm.ItemId);
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId), "Id", "Name", vm.StoreId);
            //محددات العرض
            if (vm.ParemeterReportList.Count() == 0)
                vm.ParemeterReportList.AddRange(new List<int> { 1, 2 });
            var parList = new List<SelectListItem> {
                new SelectListItem { Text = "اجمالى وارد", Value = "1", Selected = vm.ParemeterReportList.Contains(1)?true:vm.ParemeterReportList.Count()==0?true:false },
                new SelectListItem { Text = "اجمالى صادر", Value = "2", Selected = vm.ParemeterReportList.Contains(2)?true:vm.ParemeterReportList.Count()==0?true:false},
                new SelectListItem { Text = "رصيد اول", Value = "3", Selected = vm.ParemeterReportList.Contains(3)?true:false },
                new SelectListItem { Text = "شراء", Value = "4" , Selected = vm.ParemeterReportList.Contains(4)?true:false} ,
                new SelectListItem { Text = "مرتجع شراء", Value = "5" , Selected = vm.ParemeterReportList.Contains(5)?true:false},
                new SelectListItem { Text = "بيع", Value = "6" , Selected = vm.ParemeterReportList.Contains(6)?true:false} ,
                new SelectListItem { Text = "مرتجع بيع", Value = "7" , Selected = vm.ParemeterReportList.Contains(7)?true:false},
                new SelectListItem { Text = "تحويل من", Value = "8", Selected = vm.ParemeterReportList.Contains(8)?true:false },
                new SelectListItem { Text = "تحويل الى", Value = "9" , Selected = vm.ParemeterReportList.Contains(9)?true:false},
                new SelectListItem { Text = "جرد", Value = "10", Selected = vm.ParemeterReportList.Contains(10)?true:false },
                new SelectListItem { Text = "اوامر الانتاج", Value = "11", Selected = vm.ParemeterReportList.Contains(11)?true:false },
                new SelectListItem { Text = "خامات اوامر انتاج", Value = "12" , Selected = vm.ParemeterReportList.Contains(12)?true:false},
                new SelectListItem { Text = "صيانة", Value = "13", Selected = vm.ParemeterReportList.Contains(13)?true:false },
                new SelectListItem { Text = "قطع غيار صيانة", Value = "14" , Selected = vm.ParemeterReportList.Contains(14)?true:false},
                new SelectListItem { Text = "هالك", Value = "15" , Selected = vm.ParemeterReportList.Contains(15)?true:false},
                new SelectListItem { Text = "اذن استلام", Value = "16" , Selected = vm.ParemeterReportList.Contains(16)?true:false},
                new SelectListItem { Text = "اذن صرف", Value = "17" , Selected = vm.ParemeterReportList.Contains(17)?true:false},
        };
            ViewBag.ParemeterReportList = new SelectList(parList, "Value", "Text", vm.ParemeterReportList);

            if (vm.dtFrom == null || vm.dtTo == null)
            {
                #region تاريخ البداية والنهاية فى البحث
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                    var dFromS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                    var dToS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
                
                #endregion

                DateTime dFrom, dTo;
                if (DateTime.TryParse(dFromS, out dFrom) && DateTime.TryParse(dToS, out dTo))
                {
                    vm.dtFrom = dFrom;
                    vm.dtTo = dTo;
                }
                else
                    vm.dtFrom = vm.dtTo = Utility.GetDateTime();
            }
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(vm.dtFrom.ToString(), out dtFrom) && DateTime.TryParse(vm.dtTo.ToString(), out dtTo))
                vm.ItemStocks = _itemService.ItemStocks(vm.ItemId, vm.ItemGroup, vm.BranchId, vm.StoreId, dtFrom, dtTo, vm.CostCalculationPurchaseId, vm.CostCalculationSellId, vm.ParemeterReportList);
            else
                vm = new ItemStockBalanceVM();
            return View(vm);
        }
        #endregion


    }
}