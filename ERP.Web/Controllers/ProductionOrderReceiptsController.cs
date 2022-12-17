using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ProductionOrderReceiptsController : Controller
    {
        // GET: ProductionOrderReceipts
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public ProductionOrderReceiptsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }

        #region ادارة أوامر الإنتاج
        public ActionResult Index()
        {
            ViewBag.FinalItemId = new SelectList(new List<Item>(), "Id", "Name");

            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ProductionOrders.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, OrderNumber = x.OrderNumber, ProductionOrderDate = x.ProductionOrderDate.ToString(), /*FinalItemName = x.FinalItem.ItemCode + "|" + x.FinalItem.Name, OrderQuantity = x.OrderQuantity,IsDone = x.IsDone, IsDoneTitle = x.IsDone ? "تم التسليم" : "لم يتم التسليم بعد",*/  Actions = n, Num=n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region  Register Receipts

        public ActionResult RegisterReceipts(string ordrGud)
        {
            Guid guid;
            if (Guid.TryParse(ordrGud, out guid))
            {
                var productionOrder = db.ProductionOrders.Where(x => x.Id == guid).FirstOrDefault();
                var vm = new ProductionOrderReceiptVM
                {
                    ProductionOrderId = productionOrder.Id,
                    OrderNumber = productionOrder.OrderNumber,
                    //OrderQuantity = productionOrder.OrderQuantity,
                    ReceiptQuantityOld=productionOrder.ProductionOrderReceipts.Where(x=>!x.IsDeleted).Select(x=>x.ReceiptQuantity).DefaultIfEmpty(0).Sum(),
                    //FinalItemId = productionOrder.FinalItemId,
                    //FinalItemName = productionOrder.FinalItem.Name,
                    ReceiptDate = Utility.GetDateTime(),
                    OrderDate = productionOrder.ProductionOrderDate
                };
                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.FinalItemStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name", defaultStore?.Id);

                return View(vm);

            }
            return RedirectToAction("Index", "ProductionOrderReceipts");
        }

        [HttpPost]
        public ActionResult RegisterReceipts(ProductionOrderReceiptVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ReceiptQuantity == 0 || vm.ProductionOrderId == null || vm.FinalItemStoreId == null || vm.ReceiptDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                //التأكد من ان اجمالى الكميات المستلمة السابقة والحالية اقل او تساوى الكمية المنتجة 
                var receiptQuantityOld = db.ProductionOrderReceipts.Where(x => !x.IsDeleted && x.ProductionOrderId == vm.ProductionOrderId).Select(x => x.ReceiptQuantity).DefaultIfEmpty(0).Sum();
                if (receiptQuantityOld + vm.ReceiptQuantity > vm.OrderQuantity)
                    return Json(new { isValid = false, message = "خطأ ... الكمية المستلمة اكبر من المنتجة" });

                db.ProductionOrderReceipts.Add(new ProductionOrderReceipt()
                {
                    ReceiptQuantity = vm.ReceiptQuantity,
                    FinalItemStoreId = vm.FinalItemStoreId,
                    Notes = vm.Notes,
                    ProductionOrderId = vm.ProductionOrderId,
                    ReceiptDate = vm.ReceiptDate
                });
                if (receiptQuantityOld + vm.ReceiptQuantity == vm.OrderQuantity)
                {
                    var productionOrder = db.ProductionOrders.Where(x => x.Id == vm.ProductionOrderId).FirstOrDefault();
                    //productionOrder.IsDone = true;
                    db.Entry(productionOrder).State = EntityState.Modified;
                }


                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم التسليم بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

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