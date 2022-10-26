using ERP.DAL;
using ERP.DAL.Models;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using Newtonsoft.Json;
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

    public class DamageInvoicesController : Controller
    {
        // GET: DamageInvoices
        VTSaleEntities db;
        VTSAuth auth;
        ItemService _itemService;
        public DamageInvoicesController()
        {
            auth = new VTSAuth();
            db = new VTSaleEntities();
            _itemService = new ItemService();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.DamageInvoices.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, InvoiceGuid = x.Id, InvoiceDate = x.InvoiceDate.ToString(), BranchName = x.Store.Branch.Name, StoreName = x.Store.Name, TotalQuantity = x.DamageInvoiceDetails.Where(d => !d.IsDeleted).Select(d => d.Quantity).Sum(), TotalCostQuantityAmount = x.TotalCostQuantityAmount, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult NewDamageInvoice()
        {
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            //احتساب تكلفة المنتج 
            var model = db.GeneralSettings.ToList();
            var itemCost = model.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
            ViewBag.ItemCostCalculateId = new SelectList(db.ItemCostCalculations.Where(x => !x.IsDeleted), "Id", "Name", itemCost != null ? itemCost : null);

            return View();
        }
        public ActionResult SearchItemBalances(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId, int isFirstInitPage)
        {
            int? n = null;
            List<ItemBalanceDto> list;
            if (isFirstInitPage == 1)
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);
            else
            {
                list = BalanceService.SearchItemQuantityDamages(itemCode, barCode, groupId, itemtypeId, itemId, storeId);
                return Json(new
                {
                    data = list
                }, JsonRequestBehavior.AllowGet); ;

            }

        }
        [HttpPost]
        //حفظ الفاتورة
        public ActionResult SaveInvoice(string InvoiceDate, Guid? StoreId, int? ItemCostCalculateId, string Notes, string data)
        {
            List<ItemBalanceDto> itemBalanceDtos = new List<ItemBalanceDto>();
            if (data != null)
                itemBalanceDtos = JsonConvert.DeserializeObject<List<ItemBalanceDto>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });


            foreach (var item in itemBalanceDtos)
            {
                if (!double.TryParse(item.BalanceReal.ToString(), out var balancRel))
                    return Json(new { isValid = false, message = "تأكد من ادخال الكميات بشكل صحيح" });
            }
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            int itemCost;
            if (ItemCostCalculateId == null)
            {
                //احتساب تكلفة المنتج 
                var generals = db.GeneralSettings.ToList();
                var itemCot = generals.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
                if (!int.TryParse(itemCot, out itemCost))
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة الاحتساب" });
            }
            else
                itemCost = ItemCostCalculateId ?? 1;

            //الاصناف
            List<DamageInvoiceDetail> items = new List<DamageInvoiceDetail>();
            if (itemBalanceDtos.Count() == 0)
                return Json(new { isValid = false, message = "تأكد من وجود صنف واحد على الاقل" });
            else
            {
                items = itemBalanceDtos.Select(x =>
                  new DamageInvoiceDetail
                  {
                      ItemId = x.Id,
                      Quantity = x.BalanceReal,
                      CostQuantity = x.BalanceReal * _itemService.GetItemCostCalculation(itemCost, x.Id)
                  }).ToList();
            }

            DamageInvoice model = new DamageInvoice();
            DateTime invoiceDate;
            if (DateTime.TryParse(InvoiceDate, out invoiceDate))
                model.InvoiceDate = invoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
            else
                model.InvoiceDate = Utility.GetDateTime();

            //التأكد من تخزين الاصناف التى تم ادخال كميات لها فقط 
            items = items.Where(x => x.Quantity > 0).ToList();
            model.StoreId = StoreId;
            model.TotalCostQuantityAmount = items.Sum(x => x.CostQuantity);
            model.ItemCostCalculateId = itemCost;
            model.Notes = Notes;
            model.DamageInvoiceDetails = items;

            db.DamageInvoices.Add(model);

            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        [HttpPost]
        public ActionResult Delete(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.DamageInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.DamageInvoiceDetails.Where(x => !x.IsDeleted && x.DamageInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
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

        #region عرض بيانات فاتورة الهالك بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowDamageInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.DamageInvoices.Where(x => x.Id == invoGuid && x.DamageInvoiceDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.DamageInvoiceDetails = vm.DamageInvoiceDetails.Where(x => !x.IsDeleted).ToList();
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