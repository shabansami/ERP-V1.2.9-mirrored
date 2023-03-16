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
using ERP.Web.ViewModels;
using System.Windows.Documents;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class RptAccountStatementsController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        // GET: RptAccountStatements
        #region كشف حساب  
        public ActionResult Search(GeneralDayAccountVM vm)
        {
            if (vm.dtFrom == null || vm.dtTo == null)
            {
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {

                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                    var dFromS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                    var dToS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
                    DateTime dFrom, dTo;
                    if (DateTime.TryParse(dFromS, out dFrom) && DateTime.TryParse(dToS, out dTo))
                    {
                        vm.dtFrom = dFrom;
                        vm.dtTo = dTo;
                    }else
                        vm.dtFrom=vm.dtTo = Utility.GetDateTime();
                    }
                else
                    vm.dtFrom = vm.dtTo = Utility.GetDateTime();
            }
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(vm.dtFrom.ToString(), out dtFrom) && DateTime.TryParse(vm.dtTo.ToString(), out dtTo) && vm.AccountTreeId != null)
            {
                var list = GeneralDailyService.SearchAccountGeneralDailies(dtFrom, dtTo, vm.AccountTreeId, vm.CustomerRelated == true ? 1 : 0, vm.ShowRptEn);
                list.AccountTreeId = null;
                return View(list);
            }
            else
                return View(vm);

        }

        #endregion
        #region بحث قيود كشف حساب لاكثر من حساب  
        public ActionResult SearchMultiple(GeneralDayAccountVM vm, string accountTreeIds)
        {
            if (vm.dtFrom == null || vm.dtTo == null)
            {
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {

                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                    var dFromS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                    var dToS = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
                    DateTime dFrom, dTo;
                    if (DateTime.TryParse(dFromS, out dFrom) && DateTime.TryParse(dToS, out dTo))
                    {
                        vm.dtFrom = dFrom;
                        vm.dtTo = dTo;
                    }
                    else
                        vm.dtFrom = vm.dtTo = Utility.GetDateTime();
                }
                else
                    vm.dtFrom = vm.dtTo = Utility.GetDateTime();
            }
            var accounts =accountTreeIds!=null? accountTreeIds.Split(',').Select(Guid.Parse).ToList():new List<Guid>();
            List<GeneralDayAccountVM> generalDayMultiAccounts = new List<GeneralDayAccountVM>();
            DateTime dtFrom, dtTo;
            foreach (var item in accounts)
            {
                if (DateTime.TryParse(vm.dtFrom.ToString(), out dtFrom) && DateTime.TryParse(vm.dtTo.ToString(), out dtTo))
                {
                    generalDayMultiAccounts.Add(GeneralDailyService.SearchAccountGeneralDailies(dtFrom, dtTo, item, vm.CustomerRelated == true ? 1 : 0, vm.ShowRptEn));
                }
            }

            vm.GeneralDayMultiAccounts = generalDayMultiAccounts;
            return View(vm);
        }
        //الطريقة القديمة
        //public ActionResult SearchMultiple()
        //{
        //    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
        //    {
        //        var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
        //        ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
        //        ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
        //    }
        //    else
        //        ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

        //    ViewBag.TransactionTypeId = new SelectList(db.TransactionsTypes.Where(x => !x.IsDeleted), "Id", "Name");
        //    return View();
        //}
        //public ActionResult SearchMultipleData(string dFrom, string dTo, string accountTreeIds, int isFirstInitPage, string bgColor)
        //{
        //    int? n = null;
        //    DateTime dtFrom, dtTo;
        //    List<GeneralDayDto> list = new List<GeneralDayDto>();
        //    bool isFirstInit = false;
        //    if (isFirstInitPage == 1)
        //        isFirstInit = true;
        //    if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
        //    {
        //        if (string.IsNullOrEmpty(accountTreeIds))
        //            return Json(new
        //            {
        //                data = new { }
        //            }, JsonRequestBehavior.AllowGet);

        //        var accounts = accountTreeIds.Split(',').Select(Guid.Parse).ToList();
        //        foreach (var account in accounts)
        //        {
        //            list.AddRange(GeneralDailyService.SearchAccountGeneralDailies(dtFrom, dtTo, account).GeneralDalies);
        //        }
        //    }
        //    else
        //        list = new List<GeneralDayDto> { };

        //    return Json(new
        //    {
        //        data = list
        //    }, JsonRequestBehavior.AllowGet); ;

        //}
        public ActionResult SearchBalanceMultiAccounts(string dFrom, string dTo, string accountTreeIds)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<GeneralDayAccountVM> list = new List<GeneralDayAccountVM>();
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                if (string.IsNullOrEmpty(accountTreeIds))
                    return Json(new
                    {
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);

                var accounts = accountTreeIds.Split(',').Select(Guid.Parse).ToList();
                    list=GeneralDailyService.SearchBalanceMultiAccounts(accounts,dtFrom, dtTo);
            }
            else
                list = new List<GeneralDayAccountVM> { };

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }
        #endregion+

        #region ارصدة موردين
        public ActionResult BalanceSuppliers()
        {
            ViewBag.DrpPersonId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
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
        public ActionResult SearchBalancePersons(string DrpPersonId, string CountryId, string CityId, string AreaId, string dtFrom, string dtTo, int isFirstInitPage, bool isCustomer)
        {
            int? n = null;
            List<GeneralDayAccountVM> list;
            //البحث من الصفحة الرئيسية
            if (isFirstInitPage == 1)
                return Json(new
                {
                    data = new List<GeneralDayAccountVM>()
                }, JsonRequestBehavior.AllowGet);
            else
                list = GeneralDailyService.SearchBalanceSupplier(DrpPersonId, CountryId, CityId, AreaId, dtFrom, dtTo, isCustomer);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;
        }

        #endregion
        #region ارصدة عملاء
        public ActionResult BalanceCustomers()
        {
            ViewBag.DrpPersonId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
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