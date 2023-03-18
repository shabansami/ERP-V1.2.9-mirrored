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
using ERP.Web.Identity;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class InventoryInvoicesController : Controller
    {
        // GET: InventoryInvoices
        // GET: InventoryInvoices
        VTSaleEntities db;
        CheckClosedPeriodServices closedPeriodServices;
        public static string DS { get; set; }

        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService _itemService;
        public InventoryInvoicesController()
        {
            db = new VTSaleEntities();
            _itemService = new ItemService();
            closedPeriodServices = new CheckClosedPeriodServices();

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
                data = db.InventoryInvoices.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceDate = x.InvoiceDate.ToString(), BranchName = x.Branch.Name, StoreName = x.Store.Name, TotalDifferenceAmount = x.TotalDifferenceAmount, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult NewInventoryInvoice()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.GroupBasicId = new SelectList(db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == (int)GroupTypeCl.Basic), "Id", "Name"); // item groups (مواد خام - كشافات ...)
            ViewBag.ItemTypeId = new SelectList(db.ItemTypes.Where(x => !x.IsDeleted), "Id", "Name");// item type (منتج خام - وسيط - نهائى 
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.Branchcount = branches.Count();
            //احتساب تكلفة المنتج 
            var model = db.GeneralSettings.ToList();
            var itemCost = model.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
            ViewBag.ItemCostCalculateId = itemCost;

            return View();
        }
        //public ActionResult SearchItemBalances(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId, int isFirstInitPage)
        //{
        //    int? n = null;
        //    List<ItemBalanceDto> list;
        //    if (isFirstInitPage == 1)
        //        return Json(new
        //        {
        //            data = new { }
        //        }, JsonRequestBehavior.AllowGet);
        //    else
        //    {
        //        list = BalanceService.SearchItemBalanceInventory(itemCode, barCode, groupId, itemtypeId, itemId, branchId, storeId);
        //        return Json(new
        //        {
        //            data = list
        //        }, JsonRequestBehavior.AllowGet); ;

        //    }

        //}
        #region  اضافة اصناف الفاتورة
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new ItemBalanceDto()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemBalanceDto>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(ItemBalanceDto vm)
        {
            List<ItemBalanceDto> deDS = new List<ItemBalanceDto>();
            string itemName = "";
            string groupName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemBalanceDto>>(vm.DT_Datasource);

            if (vm.Id != null)
            {
                if (deDS.Where(x => x.Id == vm.Id ).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                if (deDS.Where(x => x.StoreId != vm.StoreId ).Count() > 0)
                    return Json(new { isValid = false, msg = "لا يمكن اختيار اكثر من مخزن للفاتورة الواحدة " }, JsonRequestBehavior.AllowGet);

                var item = db.Items.FirstOrDefault(x => x.Id == vm.Id);
                itemName = item.Name;
                groupName = item.GroupBasic?.Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            if (vm.BalanceReal <= 0)
                return Json(new { isValid = false, msg = "تأكد من ادخال رقم صحيح للرصيد " }, JsonRequestBehavior.AllowGet);

            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);


            var newItemDetails = new ItemBalanceDto { Id = vm.Id,Balance=vm.Balance,BalanceReal=vm.BalanceReal, GroupName= groupName,  ItemName= itemName, StoreId=vm.StoreId,StoreName=storeName};
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح " }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //حفظ الفاتورة
        public ActionResult SaveInvoice(string InvoiceDate, Guid? BranchId, Guid? StoreId, int? ItemCostCalculateId, string Notes, string data)
        {
            List<ItemBalanceDto> itemBalanceDtos = new List<ItemBalanceDto>();
            if (data != null)
                itemBalanceDtos = JsonConvert.DeserializeObject<List<ItemBalanceDto>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            var checkdate = closedPeriodServices.IsINPeriod(InvoiceDate);
            if (!checkdate)
            {
                return Json(new { isValid = false, message = "تاريخ المعاملة خارج فترة التشغيل " });

            }
            foreach (var item in itemBalanceDtos)
            {
                if (!double.TryParse(item.BalanceReal.ToString(), out var balancRel))
                    return Json(new { isValid = false, message = "تأكد من ادخال الكميات بشكل صحيح" });
            }
            int itemCost;
            if (ItemCostCalculateId == null)
            {
                //احتساب تكلفة المنتج 
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة احتساب تكلفة المنتج من الاعدادت العامة" });
            }
            else
                itemCost = ItemCostCalculateId ?? 1;

            //الاصناف
            List<InventoryInvoiceDetail> items = new List<InventoryInvoiceDetail>();
            if (itemBalanceDtos.Count() == 0)
                return Json(new { isValid = false, message = "تأكد من وجود صنف واحد على الاقل" });
            else
            {
                items = itemBalanceDtos.Select(x =>
                  new InventoryInvoiceDetail
                  {
                      ItemId = x.Id,
                      Balance = x.Balance,
                      BalanceReal = x.BalanceReal,
                      DifferenceCount = x.BalanceReal - x.Balance,
                      DifferenceAmount = (x.BalanceReal - x.Balance) * _itemService.GetItemCostCalculation(itemCost, x.Id)
                  }).ToList();
            }

            InventoryInvoice model = new InventoryInvoice();
            DateTime invoiceDate;
            if (DateTime.TryParse(InvoiceDate, out invoiceDate))
                model.InvoiceDate = invoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
            else
                model.InvoiceDate = Utility.GetDateTime();

            model.BranchId = BranchId;
            model.StoreId = StoreId;
            model.TotalDifferenceAmount = items.Sum(x => x.DifferenceAmount);
            model.ItemCostCalculateId = itemCost;
            model.Notes = Notes;
            model.InventoryInvoiceDetails = items;

            db.InventoryInvoices.Add(model);

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
                var model = db.InventoryInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.InventoryInvoiceId == model.Id).ToList();
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

        #region عرض بيانات فاتورة الجرد بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowInventoryInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.InventoryInvoices.Where(x => x.Id == invoGuid && x.InventoryInvoiceDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.InventoryInvoiceDetails = vm.InventoryInvoiceDetails.Where(x => !x.IsDeleted).ToList();
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