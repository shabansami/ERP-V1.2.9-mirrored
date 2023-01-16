using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using static ERP.Web.Utilites.Lookups;
using System.Runtime.Remoting.Contexts;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoiceHasInstallmentsController : Controller
    {
        // GET: SellInvoiceHasInstallments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        SellInvoiceInstallmentService installmentService = new SellInvoiceInstallmentService();
        public ActionResult Index()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }

        public ActionResult GetAll(string dFrom, string dTo, Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<SellInvoiceInstallmentDto> list;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = installmentService.GetSellInvoiceInstallments(dtFrom, dtTo, customerId, true);
            }
            else
                list = installmentService.GetSellInvoiceInstallments(null, null, customerId, true);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;


        }

        [HttpGet]
        public ActionResult Installments(string invoGuid, string typ)
        {
            if (!string.IsNullOrEmpty(typ))
            {
                if (typ.Trim() == "sell")
                {
                    Guid sellGuid;
                    if (Guid.TryParse(invoGuid, out sellGuid))
                    {
                        var sell = db.SellInvoices.Where(x => x.Id == sellGuid).FirstOrDefault();
                        SellInvoiceInstallmentScheduleCollectionVM vm = new SellInvoiceInstallmentScheduleCollectionVM();
                        vm = installmentService.GetInstallmentSchedulesCollections(sellGuid, null);
                        return View(vm);

                    }
                    else
                        return RedirectToAction("Index");
                }
                else if (typ.Trim() == "initial")
                {
                    Guid custInitialId;
                    if (Guid.TryParse(invoGuid, out custInitialId))
                    {
                        var custInitial = db.PersonIntialBalances.Where(x => x.Id == custInitialId).FirstOrDefault();
                        SellInvoiceInstallmentScheduleCollectionVM vm = new SellInvoiceInstallmentScheduleCollectionVM();
                        vm = installmentService.GetInstallmentSchedulesCollections(null, custInitialId);
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

        [HttpPost]
        public ActionResult Delete(string invoGuid, string typ)
        {
            if (!string.IsNullOrEmpty(typ))
            {
                if (typ.Trim() == "sell")
                {
                    Guid sellGuid;
                    if (Guid.TryParse(invoGuid, out sellGuid))
                    {
                        var sell = db.SellInvoices.Where(x => x.Id == sellGuid).FirstOrDefault();
                        if (sell!=null)
                        {
                            //حذف الاقساط
                            var installments = db.Installments.Where(x => !x.IsDeleted && x.SellInvoiceId == sell.Id).FirstOrDefault();
                            if (installments!=null)
                            {
                                installments.IsDeleted = true;
                                db.Entry(installments).State = EntityState.Modified;

                                var generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == installments.Id).ToList();
                                foreach (var item in generalDailies)
                                {
                                    item.IsDeleted = true;
                                    db.Entry(item).State = EntityState.Modified;

                                }
                                var installmentSchedules = installments.InstallmentSchedules.Where(x => !x.IsDeleted).ToList();
                                foreach (var item in installmentSchedules)
                                {
                                    var generalDailiesSchedules = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == item.Id).ToList();
                                    foreach (var item2 in generalDailiesSchedules)
                                    {
                                        item2.IsDeleted = true;
                                        db.Entry(item2).State = EntityState.Modified;

                                    }
                                    item.IsDeleted = true;
                                    db.Entry(item).State = EntityState.Modified;

                                }
                                var prevouisNotyInstallmentSchedules = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == sell.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceInstallments).ToList();
                                foreach (var item in prevouisNotyInstallmentSchedules)
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
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else if (typ.Trim() == "initial")
                {
                    Guid custInitialId;
                    if (Guid.TryParse(invoGuid, out custInitialId))
                    {
                        var custInitial = db.PersonIntialBalances.Where(x => x.Id == custInitialId).FirstOrDefault();
                        if (custInitial != null)
                        {
                            //حذف الاقساط
                            var installments = db.Installments.Where(x => !x.IsDeleted && x.IntialCustomerId == custInitial.Id).FirstOrDefault();
                            if (installments != null)
                            {
                                installments.IsDeleted = true;
                                db.Entry(installments).State = EntityState.Modified;

                                var generalDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == installments.Id).ToList();
                                foreach (var item in generalDailies)
                                {
                                    item.IsDeleted = true;
                                    db.Entry(item).State = EntityState.Modified;

                                }
                                var installmentSchedules = installments.InstallmentSchedules.Where(x => !x.IsDeleted).ToList();
                                foreach (var item in installmentSchedules)
                                {
                                    var generalDailiesSchedules = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == item.Id).ToList();
                                    foreach (var item2 in generalDailiesSchedules)
                                    {
                                        item2.IsDeleted = true;
                                        db.Entry(item2).State = EntityState.Modified;

                                    }
                                    item.IsDeleted = true;
                                    db.Entry(item).State = EntityState.Modified;

                                }
                                var prevouisNotyInstallmentSchedules = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == custInitial.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceInstallments).ToList();
                                foreach (var item in prevouisNotyInstallmentSchedules)
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
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });



        }

    }
}