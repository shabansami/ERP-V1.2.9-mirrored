using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Services;
using ERP.Web.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using System.Xml.Linq;
using ERP.Web.Identity;
using ERP.DAL.Models;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class OffersController : Controller
    {
        // GET: Offers
        VTSaleEntities db;
        VTSAuth auth;
        StoreService storeService;
        public OffersController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            storeService = new StoreService();
        }
        public static string DS { get; set; }

        public ActionResult Index()
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", defaultStore?.Id);
            ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(DateTime? dtFrom,DateTime? dtTo)
        {
            int? n = null;
            var offer = db.Offers.Where(x => !x.IsDeleted);
            if (dtFrom!=null&&dtTo!=null)
                offer=offer.Where(x=>DbFunctions.TruncateTime(x.StartDate)>=dtFrom&&DbFunctions.TruncateTime(x.StartDate)<=dtTo);
            var list = offer.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                OfferTypeName = x.OfferType.Name,
                StartDate=x.StartDate.ToString(),
                EndDate=x.EndDate.ToString(),
                AmountBefore=x.AmountBefore,
                AmountAfter=x.AmountAfter,
                Limit=x.Limit,Num=n,
                Actions = n
            }).ToList();
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);

        }

        #region  اضافة الاصناف 
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new List<ItemDetailsDT>()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && x.StoreId == vm.StoreId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit()
        {
            DS = null;
            // add
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = db.Branches.Where(x => !x.IsDeleted);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            ViewBag.OfferTypeId = new SelectList(db.OfferTypes.Where(x => !x.IsDeleted), "Id", "Name");

            return View(new Offer() { StartDate=Utility.GetDateTime(),EndDate=Utility.GetDateTime(),Limit=0});
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(Offer vm,string DT_Datasource,bool IsDiscountItemVal)
        {

            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            if (vm.OfferTypeId==0||vm.BranchId==null||string.IsNullOrEmpty(vm.Name)||vm.StartDate==null||vm.EndDate==null)
                return Json(new { isValid = false, message = "تأكد من ادخال واختيار البيانات المطلوبة" });

            List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
            List<OfferDetail> items = new List<OfferDetail>();
            Offer offer = new Offer()
            {
                Name=vm.Name,
                OfferTypeId=vm.OfferTypeId,
                StartDate=vm.StartDate, 
                EndDate=vm.EndDate, 
                BranchId=vm.BranchId,
                Limit=vm.Limit,
                Notes=vm.Notes,
            };
            if (vm.OfferTypeId==(int)OfferTypeCl.ForItem)
            {
                if(vm.AmountBefore==0||vm.AmountAfter==0)
                    return Json(new { isValid = false, message = "تأكد من ادخال قيمة العرض قبل وبعد " });

                //الاصناف
                if (DT_Datasource != null)
                {
                    itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_Datasource);
                    if (itemDetailsDT.Count() == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    else
                    {
                        items = itemDetailsDT.Select(x => new OfferDetail
                        {
                            ItemId = x.ItemId,
                            Quantity = x.Quantity,
                        }).ToList();
                        offer.OfferDetails = items;
                        offer.AmountBefore = vm.AmountBefore;
                        offer.AmountAfter = vm.AmountAfter;
                        if(IsDiscountItemVal)
                            offer.DiscountAmount = vm.DiscountAmount;
                        else
                        {
                            offer.DiscountAmount = vm.AmountBefore * vm.DiscountAmount / 100;
                            offer.DiscountPercentage=vm.DiscountAmount;
                        }
                    }
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
            }else if(vm.OfferTypeId==(int)OfferTypeCl.ForInvoice)
            {
                if (vm.InvoiceAmountFrom == 0 || vm.InvoiceAmountTo == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال اجمالى قيمة الفاتورة قبل وبعد " });
                if(vm.InvoiceAmountTo<vm.InvoiceAmountFrom)
                    return Json(new { isValid = false, message = "خطأ ... تأكد من ان اجمالي الفاتورة (الى ) بشكل صحيح " });

                offer.InvoiceAmountFrom = vm.InvoiceAmountFrom;
                offer.InvoiceAmountTo=vm.InvoiceAmountTo;
                offer.DiscountAmount = vm.DiscountAmount;
            }else
                return Json(new { isValid = false, message = "تأكد من اختيار نوع العرض بشكل صحيح" });

            db.Offers.Add(offer);
            DS = null;
            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                return Json(new { isValid = true, message = "تم اضافة العرض بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });



        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Offers.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف الاصناف
                    var items =model.OfferDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in items)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        [HttpPost]
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
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