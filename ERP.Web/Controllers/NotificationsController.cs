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
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class NotificationsController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        // GET: Notifications
        public ActionResult Index()
        {
            NotificationVM vm = new NotificationVM();
            var dateNow = Utility.GetDateTime().Date;
            var notifies = db.Notifications
                .Where(x => !x.IsDeleted && !x.IsClosed).ToList();

            //المصروفات الدورية
            vm.ExpenesePeriodic= 
                notifies.Where(x=>x.NotificationTypeId==(int)NotificationTypeCl.ExpenesePeriodic&& x.IsPeriodic
                //&& dateNow >= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day).AddDays(-2)
                &&dateNow>=x.StartAlertDate
                ).Select(x => new NotificationDto
                {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                DueDate = new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day)
                }).ToList();
            //المصروفات الغير الدورية
            vm.ExpenesePeriodic.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ExpenesePeriodic && !x.IsPeriodic
                 //&& dateNow >= x.DueDate.Value.AddDays(-2)
                 && dateNow >= x.StartAlertDate
                ).Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate = x.DueDate
                }).ToList()
                );
            //الايرادات الدورية
            vm.IncomePeriodic= 
                notifies.Where(x=>x.NotificationTypeId==(int)NotificationTypeCl.IncomePeriodic&& x.IsPeriodic
                //&& dateNow >= new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day).AddDays(-2)
                 && dateNow >= x.StartAlertDate
                 ).Select(x => new NotificationDto
                {
                Id = x.Id,
                Name = x.Name,
                Amount = x.Amount,
                DueDate = new DateTime(dateNow.Year, dateNow.Month, x.DueDate.Value.Day)
                }).ToList();
            //الايرادات الغير الدورية
            vm.IncomePeriodic.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.IncomePeriodic && !x.IsPeriodic
                 //&& dateNow >= x.DueDate.Value.AddDays(-2)
                 && dateNow >= x.StartAlertDate
                ).Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate = x.DueDate
                }).ToList()
                );
         //المهام الادارية
            vm.TaskEmployees.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.Task 
                 && dateNow >= x.StartAlertDate
                ).Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Employees=x.NotificationEmployees.Where(e=>!e.IsDeleted).Select(e=>e.Employee.Person.Name).ToList(),
                    DueDate = x.DueDate
                }).ToList()
                );
                      
            //فواتير البيع المستحقة
            vm.SellInvoiceDueDateClient.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceDueDateClient
                && dateNow >= x.DueDate.Value.AddDays(-3)
                /*&& dateNow <= x.DueDate.Value*/)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate =x.DueDate
                }).ToList()
                );
          //فواتير التوريد المستحقة
            vm.PurchaseInvoiceDueDateSupplier.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.PurchaseInvoiceDueDateSupplier
                && dateNow >= x.DueDate.Value.AddDays(-3)
                /*&& dateNow <= x.DueDate.Value*/)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate =x.DueDate
                }).ToList()
                );

            //الشيكات المستحقة على العملاء
            vm.ChequesClients.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ChequesClients
                && dateNow >= x.DueDate.Value.AddDays(-3)
               /* && dateNow <= x.DueDate.Value*/)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate = x.DueDate
                }).ToList()
                );
            //الشيكات المستحقة للموردين
            vm.ChequesSuppliers.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.ChequesSuppliers
                && dateNow >= x.DueDate.Value.AddDays(-3)
               /* && dateNow <= x.DueDate.Value*/)
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate = x.DueDate
                }).ToList()
                );
            //موعد استحقاق دفعة من فاتورة آجل
            //vm.ChequesClients.AddRange(
            //   notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.SellInvoicePayments
            //    && dateNow >= x.DueDate.Value.AddDays(-2)
            //    //&& dateNow <= x.DueDate.Value
            //    )
            //    .Select(x => new NotificationDto
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        Amount = x.Amount,
            //        DueDate = x.DueDate
            //    }).ToList()
            //    );
            //موعد استحقاق قسط من فاتورة تقسيط
            vm.SellInvoiceInstallments.AddRange(
               notifies.Where(x => x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceInstallments
                && dateNow >= x.DueDate.Value.AddDays(-3)
                //&& dateNow <= x.DueDate.Value
                )
                .Select(x => new NotificationDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Amount = x.Amount,
                    DueDate = x.DueDate
                }).ToList()
                );

            return View(vm);
        }

        [HttpPost]
        public ActionResult CloseAlert(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Notifications.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    model.IsClosed = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الاغلاق " });
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