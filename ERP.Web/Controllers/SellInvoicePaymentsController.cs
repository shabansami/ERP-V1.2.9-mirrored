using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
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

    public class SellInvoicePaymentsController : Controller
    {
        // GET: SellInvoicePayments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public static string DSPyments { get; set; }

        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellInvoices.Where(x => !x.IsDeleted&&x.PaymentTypeId==(int)PaymentTypeCl.Deferred).Select(x => new { Id = x.Id, InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name,AnyPaid=x.SellInvoicePayments.Where(p=>!p.IsDeleted&&p.IsPayed).Any(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult RegisterPyments(string invoGuid, string pay)
        {
            Guid sellGuid;
            if (Guid.TryParse(invoGuid, out sellGuid))
            {
                var sell = db.SellInvoices.Where(x => x.Id == sellGuid).FirstOrDefault();
                List<SellInvoicePaymentDetails> generatePaymentList = new List<SellInvoicePaymentDetails>();
                if (DSPyments != null)
                    generatePaymentList = JsonConvert.DeserializeObject<List<SellInvoicePaymentDetails>>(DSPyments);

                //الخزينة فى حالة السداد
                ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name",1);

                var vm = new SellInvoicePaymentVM
                {
                    InvoiceDate = sell.InvoiceDate,
                    RemindValue = sell.RemindValue,
                    Safy = sell.Safy,
                    CustomerName = sell.PersonCustomer!=null?sell.PersonCustomer.Name:null,
                    SellInvoiceId = sell.Id,
                    FirstDueDate=Utility.GetDateTime(),
                    sellInvoicePayments = generatePaymentList.Count()>0? generatePaymentList : sell.SellInvoicePayments.Where(x => !x.IsDeleted).Select(x => new SellInvoicePaymentDetails { Id = x.Id, Amount = x.Amount, DueDate = x.DueDate, IsPayed = x.IsPayed,PayGuid=x.PayGuid }).ToList()
                };
               if(pay!=null)
                    ViewBag.Pay = true;
                return View(vm);

            }
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult RegisterPyments(string data)
        {
            List<SellInvoicePaymentDetails> deDS = new List<SellInvoicePaymentDetails>();
            deDS = JsonConvert.DeserializeObject<List<SellInvoicePaymentDetails>>(data);

            if (deDS.Count()==0)
                return Json(new { isValid = false, message = "تأكد من انشاء دفعات اولا" });

            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            List<SellInvoicePayment> sellInvoicePayments = new List<SellInvoicePayment>();
            List<Notification> notifications = new List<Notification>();
            var invoId = deDS[0].SellInvoiceId;
            var sellInvo = db.SellInvoices.Where(x=>x.Id==invoId).Select(x=>new {Id=x.Id, RemindValue = x.RemindValue, customerName = x.PersonCustomer.Name }).FirstOrDefault();
            if (sellInvo.RemindValue!=deDS.Sum(x=>x.Amount))
                return Json(new { isValid = false, message = "اجمالى المبلغ المتبقى لا يتساوى مع الدفعات المدخلة" });

            foreach (var item in deDS)
            {
                sellInvoicePayments.Add(new SellInvoicePayment
                {
                    Id = item.Id,
                    DueDate = item.DueDate,
                    Amount = item.Amount,
                    SellInvoiceId = sellInvo.Id,
                    PayGuid=Guid.NewGuid()
                });
                notifications.Add(new Notification
                {
                    Name = $"استحقاق دفعة من فاتورة بيع رقم: {sellInvo.Id} على العميل: {sellInvo.customerName}",
                    DueDate = item.DueDate,
                    RefNumber = sellInvo.Id,
                    NotificationTypeId = (int)NotificationTypeCl.SellInvoicePayments,
                    Amount = item.Amount
                });
            }
            //حذف اى دفعات سابقة 
            var prevouisPayments = db.SellInvoicePayments.Where(x => !x.IsDeleted && x.SellInvoiceId == sellInvo.Id).ToList();
            foreach (var item in prevouisPayments)
            {
                item.IsDeleted = true;
                db.Entry(item).State = EntityState.Modified;            
            }

            //حذف اى دفعات من الاشعارات
            var prevouisNotyPayments = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == sellInvo.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoicePayments).ToList();
            foreach (var item in prevouisNotyPayments)
            {
                item.IsDeleted = true;
                db.Entry(item).State = EntityState.Modified;            
            }
            //اضافة الدفعات الجديدة
            db.SellInvoicePayments.AddRange(sellInvoicePayments);
            //اضافة الاشعارات 
             db.Notifications.AddRange(notifications);
          
            if (db.SaveChanges(auth.CookieValues.UserId)>0)
                return Json(new { isValid = true, message = "تم تسجيل الدفعات بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        [HttpPost]
        public ActionResult GeneratePyments(SellInvoicePaymentVM vm)
        {
            if (vm.Duration==0||vm.FirstDueDate == null||vm.RemindValue == 0||vm.PayValue == 0)
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
            List<SellInvoicePaymentDetails> list = new List<SellInvoicePaymentDetails>();
            var pay =Math.Round(vm.PayValue ,2);
            for (int i = 0; i < vm.Duration; i++)
            {
                var duDate = vm.FirstDueDate.Value.AddMonths(i);
                list.Add(new SellInvoicePaymentDetails
                {
                  DueDate= duDate,
                  Amount= pay
                });
            }
            string listSerilize = JsonConvert.SerializeObject(list);
            DSPyments = JsonConvert.SerializeObject(list);
            return Json(new { isValid = true, invoGuid = vm.InvoiceGuid, message="تم انشاء الدفعات بنجاح"});
        }

        [HttpPost]
        public ActionResult Paid(string id, Guid? SafeId)
        {
            Guid invoGuid;
            if (Guid.TryParse(id, out invoGuid))
            {
                var model = db.SellInvoicePayments.Where(x=>x.PayGuid==invoGuid).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    var sellInvoice = db.SellInvoices.Where(x => x.Id == model.SellInvoiceId).FirstOrDefault();
                    if (!sellInvoice.IsFinalApproval)
                        return Json(new { isValid = false, message = "لا يمكن السداد والفاتورة لم يتم اعتمادها بعد" });

                    var customerAccountId = db.Persons.Where(x => x.Id == sellInvoice.CustomerId).FirstOrDefault().AccountsTreeCustomerId;
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(customerAccountId))
                        return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });
                    if (SafeId == null )
                        return Json(new { isValid = false, message = "تأكد من تحديد الخزينة" });
                    //تسجيل قيود الدفعة
                    //  المبلغ المدفوع .. من حساب الخزينة (مدين
                    var safeAccountId = db.Safes.Where(x => x.Id == SafeId).FirstOrDefault().AccountsTreeId;
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = safeAccountId,
                        BranchId = sellInvoice.BranchId,
                        Debit = model.Amount,
                        Notes = $"سداد دفعة من فاتورة بيع رقم : {sellInvoice.Id}",
                        TransactionDate = Utility.GetDateTime(),
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.CashIn
                    });
                   //الى حساب العميل
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = customerAccountId,
                        BranchId = sellInvoice.BranchId,
                        Credit = model.Amount,
                        Notes = $"سداد دفعة فاتورة بيع رقم : {sellInvoice.Id}",
                        TransactionDate = Utility.GetDateTime(),
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.CashIn
                    });

                    //سداد 
                       model.IsPayed = true;
                        db.Entry(model).State = EntityState.Modified;
                    //تحديث طريقة السداد فى فاتورة البيع
                    sellInvoice.SafeId = SafeId;
                    db.Entry(sellInvoice).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم السداد بنجاح" });
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