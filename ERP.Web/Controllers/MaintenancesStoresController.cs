using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class MaintenancesStoresController : Controller
    {
        // GET: MaintenancesStores
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Maintenances.Where(x => !x.IsDeleted && x.StoreReceiptId != null).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceGuid = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.Person.Name, Safy = x.Safy, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", IsApprovalStore = x.IsApprovalStore, InvoStore = x.IsApprovalStore ? "1" : "2", CaseName = x.MaintenanceCas != null ? x.MaintenanceCas.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult ApprovalInvoice(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.Maintenances.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    // اعتماد المخزن
                    model.IsApprovalStore = true;
                    model.MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalStore;
                    //اضافة حالة الطلب 
                    db.MaintenanceCaseHistories.Add(new MaintenanceCaseHistory
                    {
                        Maintenance = model,
                        MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalStore
                    });

                    db.Entry(model).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        return Json(new { isValid = true, message = "تم الاعتماد المخزنى بنجاح" });
                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        }

        #region عرض بيانات فاتورة صيانة بالتفصيل
        public ActionResult ShowMaintenanceStore(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.Maintenances.Where(x => x.Id == invoGuid && x.MaintenanceDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.MaintenanceDetails = vm.MaintenanceDetails.Where(x => !x.IsDeleted).ToList();
            return View(vm);
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