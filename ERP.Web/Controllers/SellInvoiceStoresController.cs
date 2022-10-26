using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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

    public class SellInvoiceStoresController : Controller
    {
        // GET: SellInvoiceStores
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
                data = db.SellInvoices.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ApprovalStore(string invoGuid)
        {
            Guid guid;
            if (Guid.TryParse(invoGuid, out guid))
            {
                return View(db.SellInvoices.Where(x => x.Id == guid).FirstOrDefault());
            }
            return RedirectToAction("Index");
        }

        //اعتماد الفاتور مخزنيا
        public ActionResult ApprovalInvoice(string invoGuid, string data)
        {
            List<ItemDetailsStor> itemDetailsStor = new List<ItemDetailsStor>();
            if (data != null)
                itemDetailsStor = JsonConvert.DeserializeObject<List<ItemDetailsStor>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            //if (itemDetailsStor.Sum(x => x.QuantityReal) > itemDetailsStor.Sum(x => x.Quantity))
            //    return Json(new { isValid = false, message = "تأكد من ادخال كميات اقل او تساوى الكمية الاصلية للاصناف" });
            if (itemDetailsStor.Any(x => x.QuantityReal == 0))
                return Json(new { isValid = false, message = "تأكد من ادخال كميات مستلمة للاصناف" });

            foreach (var item in itemDetailsStor)
            {
                if (item.QuantityReal > item.Quantity)
                    return Json(new { isValid = false, message = "تأكد من ادخال كميات اقل او تساوى الكمية الاصلية للاصناف" });
            }
            //    تحديث الحالة
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.SellInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //اضافة الحالة 
                    var casesSellInvoiceHistory = new CasesSellInvoiceHistory
                    {
                        SellInvoice = model,
                        IsSellInvoice = true,
                    };

                    foreach (var item in itemDetailsStor)
                    {
                        var purchaseDetails = db.SellInvoicesDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                        purchaseDetails.QuantityReal = item.QuantityReal;
                        db.Entry(purchaseDetails).State = EntityState.Modified;
                    }

                    if (itemDetailsStor.Sum(x => x.QuantityReal) != itemDetailsStor.Sum(x => x.Quantity))
                    {
                        model.CaseId = (int)CasesCl.InvoiceQuantityDiffReal;
                        model.IsApprovalStore = false;
                        model.IsApprovalAccountant = false;
                        db.Entry(model).State = EntityState.Modified;

                        casesSellInvoiceHistory.CaseId = (int)CasesCl.InvoiceQuantityDiffReal;
                        db.CasesSellInvoiceHistories.Add(casesSellInvoiceHistory);
                        db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = false, isQuantitDiff = true, message = " لم يتم الاعتماد بسبب اختلاف الكميات..تم ارجاعها للمحاسب" });
                    }
                    else
                    {
                        model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                        model.IsApprovalStore = true;
                        db.Entry(model).State = EntityState.Modified;
                        casesSellInvoiceHistory.CaseId = (int)CasesCl.InvoiceApprovalStore;
                        db.CasesSellInvoiceHistories.Add(casesSellInvoiceHistory);
                        db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                    }



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