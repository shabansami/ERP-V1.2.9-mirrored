using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ProductionOrderDamagesController : Controller
    {
        // GET: ProductionOrderDamages
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index(string ordrGud)
        {
            Guid guid;
            if (Guid.TryParse(ordrGud, out guid))
            {
                return View(db.ProductionOrders.Where(x => x.Id == guid).FirstOrDefault());
            }
            return RedirectToAction("Index","ProductionOrders");
        }

        //  
        public ActionResult RegisterDamages(string ordrGud, string data)
        {
            List<ItemsMaterialDT> itemMaterials = new List<ItemsMaterialDT>();
            if (data != null)
                itemMaterials = JsonConvert.DeserializeObject<List<ItemsMaterialDT>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            foreach (var item in itemMaterials)
            {
                if (item.Quantitydamage>item.Quantity)
                    return Json(new { isValid = false, message = "تأكد من ادخال كميات توالف اقل او تساوى الكمية الاصلية للاصناف" });
            }
            //التاكد من رصيد المخزن يسمح بالكمية التى تم اضافتها فى التوالف 
            foreach (var item in itemMaterials)
            {
                var currentBalance = BalanceService.GetBalance(item.ItemId, item.StoreId);
                if (item.Quantitydamage > currentBalance)
                    return Json(new { isValid = false, message = $"كمية التالف المدخلة اكبر من رصيد المخزن لصنف ... {item.ItemName}" });
            }
            Guid Id;
            if (Guid.TryParse(ordrGud, out Id))
            {
                var model = db.ProductionOrders.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    foreach (var item in itemMaterials)
                        {
                            if (item.Quantitydamage!=0)
                            {
                                var oredrDetails = db.ProductionOrderDetails.Where(x => !x.IsDeleted && x.Id == item.Id)
                                    .FirstOrDefault();
                                oredrDetails.Quantitydamage = item.Quantitydamage;
                                db.Entry(oredrDetails).State = EntityState.Modified;
                            }
                        }

                    // احتساب كل التكاليف (منتج واحد او اجمالى وتكلفة المواد الخام
                    var expensesDT = model.ProductionOrderExpenses.Where(x => !x.IsDeleted).Select(x => new InvoiceExpensesDT
                    {
                        ExpenseAmount = x.Amount,
                    }).ToList();
                    var GetProductionOrderCosts = ProductionOrderService.GetProductionOrderCosts(model.Id, itemMaterials, expensesDT,0 /*model.OrderQuantity*/);
                    if (!GetProductionOrderCosts.IsValid)
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                    //model.MaterialItemCost = GetProductionOrderCosts.MaterialItemCost;
                    //model.DamagesCost = GetProductionOrderCosts.DamagesCost;
                    //model.TotalExpenseCost = GetProductionOrderCosts.OrderExpenseCost;
                    model.TotalCost = GetProductionOrderCosts.TotalCost;
                    //model.FinalItemCost = GetProductionOrderCosts.FinalItemCost;
                    db.Entry(model).State = EntityState.Modified;

                    db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = true, message = "تم تسجيل التوالف بنجاح" });



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