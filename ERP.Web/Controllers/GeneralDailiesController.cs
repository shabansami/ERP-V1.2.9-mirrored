using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    public class GeneralDailiesController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        // GET: GeneralDailies
        //TransactionId رقم المعاملة الاساسية مثلا (دفعة من عميل -قسط شهر -فاتورة بيع
        //TransactionShared رقم المعاملة المشتركة بين المعاملات (فاتورة البيع) مثلا (فاتورة بيع -دفعة من عميل تحت نفس فاتورة البيع - قسط شهر من حسابنفس فاتورة البيع
        public ActionResult Index(string tranId, string tranTypeId, string shw)
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            Guid transactionId;
            int transactionsTypeId;
            if (Guid.TryParse(tranId, out transactionId) && int.TryParse(tranTypeId, out transactionsTypeId))
            {
                IQueryable<GeneralDaily> generaldays = null;
                List<GeneralDaily> vm = new List<GeneralDaily>();
                generaldays = db.GeneralDailies.Where(x => !x.IsDeleted);
                generaldays = generaldays.Where(x => x.TransactionTypeId == transactionsTypeId && x.TransactionId == transactionId);
                if (!string.IsNullOrEmpty(shw))
                {
                    Guid? tranShared;
                    var generaldailies = generaldays.Where(x => x.TransactionTypeId == transactionsTypeId && x.TransactionId == transactionId).FirstOrDefault();
                    if (generaldailies != null)
                    {
                        tranShared = generaldailies.TransactionShared;
                        if (tranShared != null)
                            generaldays = generaldays.Where(x => x.TransactionShared == tranShared);
                    }
                }
                vm = generaldays.ToList();
                //generaldays = generaldays.Where(x => x.TransactionTypeId == transactionsTypeId);

                if (vm.Count > 0)
                    return View(vm);
                else
                {
                    ViewBag.ErrorMsg = "لا يوجد قيود مسجلة";
                    return View(new List<GeneralDaily>());
                }
            }
            else
            {
                ViewBag.ErrorMsg = "خطأ فى محددات البحث  برجاء البحث مرة اخرى";
                return View(new List<GeneralDaily>());
            }
        }

    }
}