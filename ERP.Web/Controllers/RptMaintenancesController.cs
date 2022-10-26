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
using System.Data.Entity;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class RptMaintenancesController : Controller
    {
        // GET: RptMaintenances
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        #region فواتير الصيانة 
        [HttpGet]
        public ActionResult SearchMaintenance()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", 1);
            //ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentSaleMenId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeSaleMenId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.DepartmentResponseId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeResponseId = new SelectList(new List<Employee>(), "Id", "Name");

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetMaintenanceInvo(Guid? branchId, Guid? customerId, Guid? employeeResponseId, Guid? employeeSaleMenId, bool? isFinalApproval, bool? hasCost, bool? hasGuarantee, Guid? itemId, string receiptDate, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo, receiptDte;
            IQueryable<Maintenance> maintenances = db.Maintenances.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                maintenances = maintenances.Where(x => x.IsFinalApproval);
            if (branchId != null)
                maintenances = maintenances.Where(x => x.BranchId == branchId);

            if (customerId != null)
                maintenances = maintenances.Where(x => x.CustomerId == customerId);

            if (employeeResponseId != null)
                maintenances = maintenances.Where(x => x.EmployeeResponseId == employeeResponseId);

            if (employeeSaleMenId != null)
                maintenances = maintenances.Where(x => x.EmployeeSaleMenId == employeeSaleMenId);

            if (itemId != null)
                maintenances = maintenances.Where(x => x.MaintenanceDetails.Where(d => !d.IsDeleted && d.ItemId == itemId).Any());

            if (hasCost == true)
                maintenances = maintenances.Where(x => x.HasCost ==hasCost);

            if (hasGuarantee == true)
                maintenances = maintenances.Where(x => x.HasGuarantee ==hasGuarantee);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                maintenances = maintenances.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo);
            if (receiptDate!=null)
            {
            if (DateTime.TryParse(receiptDate, out receiptDte))
                maintenances = maintenances.Where(x => x.ReceiptDate == receiptDte );
            }


            return Json(new
            {
                data = maintenances.Select(x => new { Id = x.Id, InvoiceGuid = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.Person.Name, EmployeeResponseName = x.EmployeeResponse.Person.Name, Safy = x.Safy, PaymentTypeName = x.PaymentType.Name, TotalQuantity = x.MaintenanceDetails.Where(y=>!y.IsDeleted).Count(), CaseName = x.MaintenanceCas != null ? x.MaintenanceCas.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

            //return Json(new
            //{
            //    data = new { }
            //}, JsonRequestBehavior.AllowGet); ;


        }

        #endregion

        #region اعطال الصيانة 
        [HttpGet]
        public ActionResult SearchMaintenanceProblem()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", 1);
            ViewBag.MaintenProblemTypeId = new SelectList(db.MaintenProblemTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.DepartmentSaleMenId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeSaleMenId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.DepartmentResponseId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeResponseId = new SelectList(new List<Employee>(), "Id", "Name");

            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetMaintenanceProblem(Guid? branchId, Guid? customerId, Guid? maintenProblemTypeId, Guid? employeeResponseId, Guid? employeeSaleMenId, bool? isFinalApproval, bool? hasCost, bool? hasGuarantee, Guid? itemId, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<MaintenanceDetail> maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                maintenances = maintenances.Where(x => x.Maintenance.IsFinalApproval);

            if (branchId != null)
                maintenances = maintenances.Where(x => x.Maintenance.BranchId == branchId);

            if (maintenProblemTypeId != null)
                maintenances = maintenances.Where(x => x.MaintenProblemTypeId == maintenProblemTypeId);

            if (customerId != null)
                maintenances = maintenances.Where(x => x.Maintenance.CustomerId == customerId);

            if (employeeResponseId != null)
                maintenances = maintenances.Where(x => x.Maintenance.EmployeeResponseId == employeeResponseId);

            if (employeeSaleMenId != null)
                maintenances = maintenances.Where(x => x.Maintenance.EmployeeSaleMenId == employeeSaleMenId);

            if (itemId != null)
                maintenances = maintenances.Where(x => x.ItemId == itemId);

            if (hasCost == true)
                maintenances = maintenances.Where(x => x.Maintenance.HasCost ==hasCost);

            if (hasGuarantee == true)
                maintenances = maintenances.Where(x => x.Maintenance.HasGuarantee ==hasGuarantee);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                maintenances = maintenances.Where(x => x.Maintenance.InvoiceDate >= dtFrom && x.Maintenance.InvoiceDate <= dtTo);

            var maintenanceList = maintenances.GroupBy(x => x.ItemId).ToList();
            var data = maintenanceList.Select(x => new RptMaintenanceProblemDto
            {
                Id = x.Select(y => y.ItemId).FirstOrDefault(),
                ItemName = x.Select(y => y.Item.Name).FirstOrDefault(),
                TotalItemSafy = x.Select(y => y.ItemSafy).DefaultIfEmpty().Sum(),
                InvoicesData = x.Select(y => new MaintenanceInvoice
                {
                    MaintenanceId = y.MaintenanceId,
                    InvoiceNum = y.Maintenance?.InvoiceNumber,
                    CustomerName = y.Maintenance?.Person.Name,
                    InvoiceDate = y.Maintenance?.InvoiceDate.ToString(),
                    ItemSafy = y.ItemSafy,
                    MaintenProblemTypeName=y.MaintenProblemType!=null?y.MaintenProblemType.Name:null

                }).ToList()
            }).ToList();

            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region احصاء الاعطال الاكثر شيوعا الصيانة 
        [HttpGet]
        public ActionResult MaintenanceProblemStatistics()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", 1);
            ViewBag.MaintenProblemTypeId = new SelectList(db.MaintenProblemTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name"); // final item

            return View();
        }
        public ActionResult GetMaintenanceProblemStatistics(Guid? branchId, Guid? maintenProblemTypeId, bool? isFinalApproval, bool? hasCost, bool? hasGuarantee, Guid? itemId, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            IQueryable<MaintenanceDetail> maintenances = db.MaintenanceDetails.Where(x => !x.IsDeleted);
            if (isFinalApproval == true)
                maintenances = maintenances.Where(x => x.Maintenance.IsFinalApproval);

            if (branchId != null)
                maintenances = maintenances.Where(x => x.Maintenance.BranchId == branchId);

            if (maintenProblemTypeId != null)
                maintenances = maintenances.Where(x => x.MaintenProblemTypeId == maintenProblemTypeId);

            if (itemId != null)
                maintenances = maintenances.Where(x => x.ItemId == itemId);

            if (hasCost == true)
                maintenances = maintenances.Where(x => x.Maintenance.HasCost ==hasCost);

            if (hasGuarantee == true)
                maintenances = maintenances.Where(x => x.Maintenance.HasGuarantee ==hasGuarantee);

            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                maintenances = maintenances.Where(x => x.Maintenance.InvoiceDate >= dtFrom && x.Maintenance.InvoiceDate <= dtTo);
            //    .Select(x=>x.OrderByDescending(y=>y.MaintenProblemTypeId))
            var maintenanceList = maintenances.GroupBy(x => x.ItemId)
                .Select(g => new {
                    name = g.Select(c=>c.Item.Name).FirstOrDefault(),
                    //max = g.Max(c => c.MaintenProblemTypeId),
                    problemTypeGroup = g.GroupBy(v => v.MaintenProblemTypeId).OrderByDescending(d=>d.Count())
                   })
                .ToList();
            var data = maintenanceList.Select(x => new
            {
                ItemName = x.name,
                //prom = x.pro.Max(y=>y.Count()),
                MaintenProblemTypeTheMostCount = x.problemTypeGroup.FirstOrDefault().Count(),
                MaintenProblemTypeName = x.problemTypeGroup.FirstOrDefault().FirstOrDefault().MaintenProblemType!=null ? x.problemTypeGroup.FirstOrDefault().FirstOrDefault().MaintenProblemType.Name:"لم يتم تحديد نوع العطل بعد"
                //MaintenProblemTypeCount=x.problemTypeGroup
            }).ToList();

            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet);
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

    public class CommentStats
    {
        public int ContextId { get; set; }
        public int CommentCount { get; set; }
    }
}