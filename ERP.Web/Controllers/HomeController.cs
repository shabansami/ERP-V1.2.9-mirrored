
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Linq;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using System.Collections.Generic;
using ERP.Web.Identity;
using BarcodeLib;
using ERP.Web.DataTablesDS;
using System.Text.RegularExpressions;
using System.Security.Cryptography.Xml;

namespace ERP.Web.Controllers
{

    public class HomeController : Controller
    {
        // GET: Home
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        //VTSAuth auth2 => TempData["userInfo"] as VTSAuth;

        #region Index
        public ActionResult Index()
        {
            //if (UsersRoleService.PagesLoad == null)
            if (!auth.CheckCookies())
            {
                return RedirectToAction("Login", "Default",new { MsgBadLogin = 1 });
            }
            var vm = new AdminHomeVM();
            try
            {
                var dayNow = Utility.GetDateTime().Date;
                vm.Customers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).Count();
                vm.SellInvoicesTotal = db.SellInvoices.Where(x => !x.IsDeleted).Count();
                vm.SellInvoiceDay = db.SellInvoices.Where(x => !x.IsDeleted && dayNow == x.InvoiceDate).Count();
                vm.SellBackInvoices = db.SellBackInvoices.Where(x => !x.IsDeleted).Count();

                vm.Suppliers = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).Count();
                vm.PurchaseInvoices = db.PurchaseInvoices.Where(x => !x.IsDeleted).Count();
                vm.PurchaseBackInvoices = db.PurchaseBackInvoices.Where(x => !x.IsDeleted).Count();

                vm.Items = db.Items.Where(x => !x.IsDeleted).Count();
                vm.ExpenseAmount = db.ExpenseIncomes.Where(x => !x.IsDeleted && x.IsExpense).Select(x=>x.Amount).DefaultIfEmpty(0).Sum();
                vm.IncomeAmount = db.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense).Select(x => x.Amount).DefaultIfEmpty(0).Sum();

                vm.Stores = db.Stores.Where(x => !x.IsDeleted).Count();
                vm.Employees = db.Employees.Where(x => !x.IsDeleted).Count();
                vm.ProductionOrders = db.ProductionOrders.Where(x => !x.IsDeleted).Count();
                vm.UploadFiles = db.UploadCenters.Where(x => !x.IsDeleted && !x.IsFolder).Count();
                vm.Notifications = GetNotify();

                //فواتير تحتاج الى اعتمماد 
                var sellApprovalStore = db.SellInvoices.Where(x => !x.IsDeleted && !x.IsApprovalStore);
                if (sellApprovalStore.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير بيع تحتاج الى اعتماد مخزنى", Count = sellApprovalStore.Count(), Url = "/SellInvoiceStores/Index" });
                var sellApprovalAccounting = db.SellInvoices.Where(x => !x.IsDeleted && !x.IsApprovalAccountant);
                if (sellApprovalAccounting.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير بيع تحتاج الى اعتماد محاسبى", Count = sellApprovalAccounting.Count(), Url = "/SellInvoiceAccounting/Index" });
                var sellApprovalFinal = db.SellInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval&&x.IsApprovalAccountant&&x.IsApprovalStore);
                if (sellApprovalFinal.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير بيع تحتاج الى اعتماد نهائى", Count = sellApprovalFinal.Count(), Url = "/SellInvoices/ApprovalFinalInvoice" });

                var sellBackApprovalStore = db.SellBackInvoices.Where(x => !x.IsDeleted && !x.IsApprovalStore);
                if (sellBackApprovalStore.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع بيع تحتاج الى اعتماد مخزنى", Count = sellBackApprovalStore.Count(), Url = "/SellBackInvoiceStores/Index" });
                var sellBackApprovalAccounting = db.SellBackInvoices.Where(x => !x.IsDeleted && !x.IsApprovalAccountant);
                if (sellBackApprovalAccounting.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع بيع تحتاج الى اعتماد محاسبى", Count = sellBackApprovalAccounting.Count(), Url = "/SellBackInvoiceAccounting/Index" });
                var sellBackApprovalFinal = db.SellBackInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval && x.IsApprovalAccountant && x.IsApprovalStore);
                if (sellBackApprovalFinal.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع بيع تحتاج الى اعتماد نهائى", Count = sellBackApprovalFinal.Count(), Url = "/SellBackInvoices/ApprovalFinalInvoice" });

                var purchaseApprovalStore = db.PurchaseInvoices.Where(x => !x.IsDeleted && !x.IsApprovalStore);
                if (purchaseApprovalStore.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير توريد تحتاج الى اعتماد مخزنى", Count = purchaseApprovalStore.Count(), Url = "/PurchaseInvoiceStores/Index" });
                var purchaseApprovalAccounting = db.PurchaseInvoices.Where(x => !x.IsDeleted && !x.IsApprovalAccountant);
                if (purchaseApprovalAccounting.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير توريد تحتاج الى اعتماد محاسبى", Count = purchaseApprovalAccounting.Count(), Url = "/PurchaseInvoiceAccounting/Index" });
                var purchaseApprovalFinal = db.PurchaseInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval && x.IsApprovalAccountant && x.IsApprovalStore);
                if (purchaseApprovalFinal.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير توريد تحتاج الى اعتماد نهائى", Count = purchaseApprovalFinal.Count(), Url = "/PurchaseInvoices/ApprovalFinalInvoice" });

                var purchaseBackApprovalStore = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && !x.IsApprovalStore);
                if (purchaseBackApprovalStore.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع توريد تحتاج الى اعتماد مخزنى", Count = purchaseBackApprovalStore.Count(), Url = "/PurchaseBackInvoiceStores/Index" });
                var purchaseBackApprovalAccounting = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && !x.IsApprovalAccountant);
                if (purchaseBackApprovalAccounting.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع توريد تحتاج الى اعتماد محاسبى", Count = purchaseBackApprovalAccounting.Count(), Url = "/PurchaseBackInvoiceAccounting/Index" });
                var purchaseBackApprovalFinal = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval && x.IsApprovalAccountant && x.IsApprovalStore);
                if (purchaseBackApprovalFinal.Any())
                    vm.ApprovalInvoices.Add(new ElementDetails { Name = "فواتير مرتجع توريد تحتاج الى اعتماد نهائى", Count = purchaseBackApprovalFinal.Count(), Url = "/PurchaseBackInvoices/ApprovalFinalInvoice" });

                //اصناف تحتاج الى حد الطلب الامان والخطر 
                var listSafetly = BalanceService.ItemRequestLimitSafety(null, null, null, null, null, null, null);
                if(listSafetly.Any())
                    vm.LimitedItem.Add(new ElementDetails {Name="اصناف بلغت حد الامان",Count= listSafetly.Count,Url= "/RptLimitItems/SearchItemSafety" });
                var listDanger = BalanceService.ItemRequestLimitSafety(null, null, null, null, null, null, null);
                if(listDanger.Any())
                    vm.LimitedItem.Add(new ElementDetails {Name="اصناف بلغت حد الخطر",Count= listDanger.Count,Url= "/RptLimitItems/SearchItemDanger" });
            }
            catch (Exception)
            {

                throw;
            }

            return View(vm);
        }

        #endregion
        
        #region الاشعارات 
         int GetNotify()
        {
            try
            {
                var dateNow = Utility.GetDateTime().Date;
                var notifies = db.Notifications
                    .Where(x => !x.IsDeleted && !x.IsClosed).ToList();
                //المصروفات الدورية
                var ExpenesePeriodic =
                    notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ExpenesePeriodic && x.IsPeriodic
                    && dateNow >= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day).AddDays(-2)
                    && dateNow <= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day))
                    .Count();
                //المصروفات الغير الدورية
                var ExpeneseNotPeriodic =
                   notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ExpenesePeriodic && !x.IsPeriodic
                    && dateNow >= x.DueDate.Value.AddDays(-2)
                    && dateNow <= x.DueDate.Value)
                    .Count();

                //الايرادات الدورية
                var IncomePeriodic =
                    notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic && x.IsPeriodic
                    && dateNow >= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day).AddDays(-2)
                    && dateNow <= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day))
                    .Count();
                //الايرادات الغير الدورية
                var IncomeNotPeriodic =
                   notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic && !x.IsPeriodic
                    && dateNow >= x.DueDate.Value.AddDays(-2)
                    && dateNow <= x.DueDate.Value)
                    .Count();


                //الفواتير المستحقة
                var SellInvoiceDueDateClient =
                    notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceDueDateClient
                    && dateNow >= x.DueDate.Value.AddDays(-2)
                    && dateNow <= x.DueDate.Value)
                    .Count();

                //الشيكات المستحقة
                var ChequesClients =
                   notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ChequesClients
                    && dateNow >= x.DueDate.Value.AddDays(-2)
                    && dateNow <= x.DueDate.Value)
                    .Count();

                return ExpenesePeriodic + ExpeneseNotPeriodic + IncomePeriodic + IncomeNotPeriodic + SellInvoiceDueDateClient + ChequesClients;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }

        }
        #endregion

        #region تعديل بياناتى الشخصية
        [Authorization(true)]
        public ActionResult Update()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            var user = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault();
                var model=new UserProfileVM
            {
                    Id=user.Id,
                UserName = user.UserName,
                PersonId=user.PersonId,
                PersonName=user.Person!=null?user.Person.Name:null,
                Address=user.Person!=null?user.Person.Address:null,
                Mob1=user.Person!=null?user.Person.Mob1:null,
                Mob2=user.Person!=null?user.Person.Mob2:null,
                AreaId=user.Person!=null?user.Person.AreaId:null,
            };
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", user.Person?.Area.City.Country.Id);
            ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name", user.Person?.Area.City.Id);
            ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name", user.Person?.AreaId);

            return View(model);

     }
        [Authorization(true)]
        [HttpPost]
        public JsonResult Update(UserProfileVM vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.UserName))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.Users.Where(x => !x.IsDeleted && x.UserName == vm.UserName.Trim() && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "اسم المستخدم موجود مسبقا" });

                    var user = db.Users.FirstOrDefault(x => x.Id == vm.Id);
                    user.UserName = vm.UserName;

                    var person = db.Persons.Where(x => x.Id == vm.PersonId).FirstOrDefault();
                    if (person != null)
                    {
                        person.Name = vm.PersonName;
                        person.Address = vm.Address;
                        person.Mob1 = vm.Mob1;
                        person.Mob2 = vm.Mob2;
                        person.AreaId = vm.AreaId;
                    }
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم التحديث بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        #endregion
        #region تغيير كلمة المرور
        [Authorization(true)]
        [HttpPost]
        public JsonResult ChangePassword(string oldPassword,string newPassword1,string newPassword2)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(oldPassword)||string.IsNullOrEmpty(newPassword1) ||string.IsNullOrEmpty(newPassword2))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                var user = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault();
                //التأكد من ان كلمة المرور القديمة صحيحة
                if(user != null)
                {
                    if (VTSAuth.Decrypt(user.Pass) != oldPassword.Trim())
                        return Json(new { isValid = false, message = "كلمة المرور القديمة غير صحيحة" });
                }
                else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                //التأكد من كلمات السر الجديدة غير مكررة 
                if (newPassword1.Trim()==newPassword2.Trim())
                {
                    user.Pass=VTSAuth.Encrypt(newPassword1.Trim());
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم تغيير كلمة المرور بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                }
                else
                    return Json(new { isValid = false, message = "كلمة السر الجديدة غير متطابقة" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        #endregion

        #region تحديد نوع الجرد للبرنامج
        public ActionResult CheckInventoryType()
        {
            var model = db.GeneralSettings.ToList();
            InventoryTypeVM vm = new InventoryTypeVM();
            //تاريخ بداية ونهاية السنة المالية
            vm.FinancialYearStartDate = DateTime.TryParse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue, out var dt) ? DateTime.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue) : Utility.GetDateTime();
            vm.FinancialYearEndDate = DateTime.TryParse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue, out var dt2) ? DateTime.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue) : Utility.GetDateTime();

            return View(vm);
        }

        [HttpPost]
        public JsonResult CheckInventoryType(InventoryTypeVM vm)
        {
            if (ModelState.IsValid)
            {
                if (!auth.LoadDataFromCookies())
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                if (string.IsNullOrEmpty(vm.InventoryTypeId))
                    return Json(new { isValid = false, message = "تأكد من اختيار نوع الجرد" });
                var generalSetting = db.GeneralSettings.ToList();
                var inventoryType = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault();
                if (inventoryType != null)
                    inventoryType.SValue = vm.InventoryTypeId;
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                if (vm.FinancialYearStartDate>=vm.FinancialYearEndDate)
                    return Json(new { isValid = false, message = "تأكد من ادخال تواريخ صحيحة" });

                //تاريخ بداية ونهاية السنة المالية
                var startDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault();
                    if (startDate!=null)
                    startDate.SValue = vm.FinancialYearStartDate.Value.ToString("yyyy-MM-dd");
                var endDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault();
                if (endDate!=null)
                    endDate.SValue = vm.FinancialYearEndDate.Value.ToString("yyyy-MM-dd");
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    return Json(new { isValid = true, message = "تم الحفظ بنجاح" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من اختيار نوع الجرد" });

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