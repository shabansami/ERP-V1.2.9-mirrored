using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class StoresTransferApprovalsController : Controller
    {
        // GET: StoresTransferApprovals
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        #region الاعتماد المخزنى
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
            int? n = null;
            var sroresTran = db.StoresTransfers.Where(x => !x.IsDeleted);
            if (auth.CookieValues.StoreId != null)
                sroresTran = sroresTran.Where(x => x.StoreToId == auth.CookieValues.StoreId);
            var list = sroresTran.OrderBy(x => x.CreatedOn)
                .Select(x => new
                {
                    Id = x.Id,
                    StoreTranNumber = x.StoreTransferNumber,
                    InvoiceDate = x.TransferDate.ToString(),
                    cmbo_approval = (!x.IsApprovalStore && !x.IsRefusStore) ? "فى الانتظار" : x.IsApprovalStore ? "تم الاعتماد" : "تم الرفض",
                    IsApprovalStore = x.IsApprovalStore,
                    IsRefusStore = x.IsRefusStore,
                    BranchFromName = x.StoreFrom.Branch.Name,
                    StoreFromName = x.StoreFrom.Name,
                    BranchToName = x.StoreTo.Branch.Name,
                    StoreToName = x.StoreTo.Name,
                    TransferDate = x.TransferDate.ToString(),
                    Actions = n,
                    Num = n
                }).ToList();
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ApprovalStore(string invoGuid)
        {
            Guid guid;
            if (Guid.TryParse(invoGuid, out guid))
            {
                return View(db.StoresTransfers.Where(x => x.Id == guid).FirstOrDefault());
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
                var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //اضافة الحالة 
                    var casesStoretranHistory = new StoresTransferHistory
                    {
                        StoresTransfer = model,
                    };

                    foreach (var item in itemDetailsStor)
                    {
                        var storeDetails = db.StoresTransferDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                        storeDetails.QuantityReal = item.QuantityReal;
                        db.Entry(storeDetails).State = EntityState.Modified;
                    }

                    if (itemDetailsStor.Sum(x => x.QuantityReal) != itemDetailsStor.Sum(x => x.Quantity))
                    {
                        model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferQuantityDiffReal;
                        model.IsApprovalStore = false;
                        db.Entry(model).State = EntityState.Modified;

                        casesStoretranHistory.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferQuantityDiffReal;
                        db.StoresTransferHistories.Add(casesStoretranHistory);
                        db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = false, isQuantitDiff = true, message = " لم يتم الاعتماد بسبب اختلاف الكميات..فى انتظار تعديل الكميات " });
                    }
                    else
                    {
                        model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferStoreApproval;
                        model.IsApprovalStore = true;
                        db.Entry(model).State = EntityState.Modified;
                        casesStoretranHistory.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferStoreApproval;
                        db.StoresTransferHistories.Add(casesStoretranHistory);
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

        #endregion
        #region عرض تفاصيل عملية تحويل
        public ActionResult ShowDetails(string id)
        {

            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var vm = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
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
        #endregion
        #region رفض امر تحويل مخزنى
        [HttpPost]
        public ActionResult Refused(string id) // رفض امر تحويل مخزنى
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsRefusStore = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
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