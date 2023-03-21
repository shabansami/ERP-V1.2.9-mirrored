using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
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

    public class SaleMenAccountsController : Controller
    {
        // GET: SaleMenAccounts
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        #region  كشف حساب مندوب  
        public ActionResult Index(SaleMenAccountVM vm)
        {

            if (vm.EmployeeId == null)
            {
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

                //ViewBag.Msg = "تأكد من اختيار المندوب";
                return View(vm);

            }else
            {
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name",vm.DepartmentId);
                Guid? empId = null;
                    empId = vm.EmployeeId;
                    var list = EmployeeService.GetSaleMens(vm.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(list, "Id", "Name", empId);



            }
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(vm.DtFrom, out dtFrom) && DateTime.TryParse(vm.DtTo, out dtTo))
                vm = EmployeeService.GetSaleMenAccount(vm.EmployeeId,dtFrom, dtTo,vm.AllTimes,vm.IsFinalApproval);
            else
                vm = EmployeeService.GetSaleMenAccount(vm.EmployeeId, null, null, vm.AllTimes,vm.IsFinalApproval);
          
            return View(vm);
        }     
        #endregion

    }
}