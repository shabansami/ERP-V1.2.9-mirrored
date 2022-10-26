using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PurchaseInvoiceAccountingController : Controller
    {
        // GET: PurchaseInvoiceAccounting
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.PurchaseInvoices.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApprovalInvoice(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.PurchaseInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    // اعتماد المحاسب
                    model.IsApprovalAccountant = true;
                    model.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                    {
                        PurchaseInvoice = model,
                        IsPurchaseInvoice = true,
                        CaseId = (int)CasesCl.InvoiceApprovalAccountant
                    });

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
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