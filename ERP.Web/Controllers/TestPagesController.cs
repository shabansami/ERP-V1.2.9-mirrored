using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    public class TestPagesController : Controller
    {
        // GET: TestPages
        public ActionResult Index()
        {
            return View();
        }
    }
}