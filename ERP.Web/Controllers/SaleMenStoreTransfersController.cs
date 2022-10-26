using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SaleMenStoreTransfersController : Controller
    {
        // GET: SaleMenStoreTransfers
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
            if (auth.CookieValues.StoreId == null) //فى حالة ان الموظف غير محدد له مخزن اى انه ليس مندوب
            {
                ViewBag.ErrorMsg = "لابد من تحديد مخزن للمندوب اولا لعرض هذه الشاشة";
                return View();
            }
            return View();
        }
        public ActionResult GetAll()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
            int? n = null;
            return Json(new
            {
                data = db.StoresTransfers.Where(x => !x.IsDeleted&&x.StoreToId==auth.CookieValues.StoreId).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id,  SaleMenIsApproval = x.IsApprovalStore,  StoreFromName = x.StoreFrom.Name, TransferDate = x.TransferDate.ToString(), Notes = x.Notes,Status=(!x.IsApprovalStore&&!x.IsRefusStore)?"فى الانتظار":x.IsApprovalStore?"تم الاعتماد":"تم الرفض", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpPost]
        public ActionResult Approval(string id,string app) // store transfer id - type 1=>
        {
            Guid Id;
            int approve;
            if (Guid.TryParse(id, out Id)&&int.TryParse(app,out approve))
            {
                var model = db.StoresTransfers.Where(x=>x.Id ==Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //model.SaleMenStatus = true;
                    //if (approve==1) //type(1) 1=>SaleMenIsApproval=true تم الموافقة والاعتماد التحويل 
                    //    model.SaleMenIsApproval = true;
                    //else //type(1) 1=>SaleMenIsApproval=false تم رفض  التحويل 
                    //    model.SaleMenIsApproval = false;

                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        if(approve==1)
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                       else
                        return Json(new { isValid = true, message = "تم الرفض بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        }

        #region عرض بيانات عملية تحويل مخزنى بالتفصيل وطباعتها
        public ActionResult Show(string id)
        {
            Guid Id;
            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("ShowDetails");
        }

        public ActionResult ShowDetails()
        {
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var vm = db.StoresTransfers.Where(x => x.Id == id).FirstOrDefault();
                    if (vm != null)
                    {
                        vm.StoresTransferDetails = vm.StoresTransferDetails.Where(x => !x.IsDeleted).ToList();
                        return View(vm);
                    }
                    else
                        return RedirectToAction("Index");
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
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