using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PricesChangesController : Controller
    {
        // GET: PricesChanges
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult PricesPuchachaseChanges()
        {
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");

            return View();
        }

        public ActionResult GetPricesPurchaseChanges(string itemId)
        {
            int? n = null;
            Guid id;
            if (Guid.TryParse(itemId, out id))
            {

                return Json(new
                {
                    //data = db.PricesChanges.Where(x => !x.IsDeleted&&x.IsPurchasePrice ).Select(x => new { Id = x.Id, ItemId = x.ItemId, ItemName = x.Item.Name, Price = x.Price, Actions = n, Num = n }).ToList()
                    data = db.PricesChanges.Where(x => !x.IsDeleted && x.ItemId == id).Select(x => new { Id = x.Id, ItemId = x.ItemId, Notes = x.Notes, Price = x.Price, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { data = new { } }, JsonRequestBehavior.AllowGet);

        }

    }
}