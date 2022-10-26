using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pekar.Models;

namespace Pekar.Controllers
{
    public class BasicTapsController : Controller
    {
        PekarEntities db = new PekarEntities();
        // GET: BasicTaps
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.BasicTaps.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.Tapname, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        public ActionResult Edit(int? id)
        {
            var update = db.BasicTaps.Find(id);
            return View(update);
        }
        [HttpPost]
        public ActionResult Edit(BasicTap bas)
        {
            return View();
        }
    }
}