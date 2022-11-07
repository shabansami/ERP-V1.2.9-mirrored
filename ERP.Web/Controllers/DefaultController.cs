using Newtonsoft.Json;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Dtos;
using System.Web.Security;
using ERP.Web.ViewModels;

namespace ERP.Web.Controllers
{
    public class DefaultController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        // GET: Default
        public ActionResult Index()
        {
            return View();
        }       

        public ActionResult Login(int? MsgBadLogin,string ErroeMsg)
        {
            //Lookups.ExcuteFirstInit();
            //var t = VTSAuth.Encrypt("p@ssw0rd");
            //add pages
            //List<Page> pages = new List<Page>();
            //Guid roleID = new Guid("B55F8D95-96DC-47A1-AD3A-98EB6CBFC8B1");
            //Guid userId = new Guid("9B5D4273-1321-4870-B7D4-32E90801C987");
            ////pages.Add(new Page() { Id = 390, ParentId = 88, Icon = "menu-bullet menu-bullet-line", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 391, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 392, ParentId = 390, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractCustomers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 393, ParentId = 66, Icon = "icon-md fas fa-coins ylow", IsPage = false, Name = "مدة استحقاق السداد", OrderNum = 0, Url = "", OtherUrls = null });
            //pages.Add(new Page() { Id = 394, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "اضافة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/CreateEdit", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 395, ParentId = 393, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "ادارة مدة استحقاق", OrderNum = 0, Url = "/ContractSuppliers/Index", OtherUrls = null, PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });
            //pages.Add(new Page() { Id = 396, ParentId = 49, Icon = "menu-bullet menu-bullet-line", IsPage = true, Name = "تسجيل سياسات اسعار الموردين", OrderNum = 0, Url = "/ItemPriceSuppliers/CreateEdit", OtherUrls = "/ItemPriceSuppliers/AddItemPriceSupplier", PagesRoles = new List<PagesRole>() { new PagesRole() { RoleId = roleID } } });

            //db.Pages.AddRange(pages);
            //db.SaveChanges(userId);



            ////////ItemImportDataZumurada itemImportDataZumurada = new ItemImportDataZumurada();
            ////////itemImportDataZumurada.Excute2();
            ////ItemImportMohRamadan itemImportMohRamadan = new ItemImportMohRamadan();
            ////////itemImportMohRamadan.Excute();
            ////////FixInitialBalance fixInitialBalance = new FixInitialBalance();
            ////fixInitialBalance.Excute();
            //////itemImportMohRamadan.ExcuteV1_2();

            if (MsgBadLogin!=null)
                if(MsgBadLogin==1)
                ViewBag.MsgBadLogin = "انتهاء صلاحية الجلسة";
            if (MsgBadLogin == 2)
                ViewBag.MsgBadLogin = "حدث خطأ اثناء تنفيذ العملية " + " ErroeMsg= "+ ErroeMsg;
            if (MsgBadLogin == 3)
                ViewBag.MsgBadLogin = "غير مصرح لك الوصول لهذه الشاشة او انتهاء صلاحية الجلسة ";
            if (MsgBadLogin == 4)
                ViewBag.MsgBadLogin =  "تم تسجيل الدخول فى جهاز آخر";
            //تسجيل الدخول مباشرا فى حالة وجود كوكيز ومدته صالحة
            VTSAuth auth = new VTSAuth();
            if (auth.LoadDataFromCookies())
            {
                var roleId = auth.CookieValues.RoleId;
                //التأكد من عمد انتها صلاحية الجلسة
                if (Utility.GetDateTime() > auth.CookieValues.SessionKey.ExpireDate)
                    return View();
                else
                {
                    if (!VTSAuth.IsDemo)
                    {
                        using (var db = new VTSaleEntities())
                        {
                            var sessionDb = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault().SessionKey;
                            var sessionCookie = VTSAuth.Encrypt(JsonConvert.SerializeObject(auth.CookieValues.SessionKey));
                            if (sessionDb != sessionCookie)
                                return View();
                            return RedirectToAction("Index", "Home");
                        }
                    }else
                        return RedirectToAction("Index", "Home");

                }

            }
                    //if (TempData["MsgErrorActive"]!=null)
                    //    ViewBag.MsgBadLogin = TempData["MsgErrorActive"];
                    return View();
        }      
        public ActionResult LogOut()
        {
            auth.ClearCookies();
            //UsersRoleService.PagesLoad = null;
            //Session["pagesSession"] = null;
            return View("Login");
        }
        //List<ListPages> GetPages(int catId, IQueryable<PagesRole> pageRoless)
        //{
        //    return db.Pages.Where(p => !p.IsDeleted && p.PageCatogryId == catId && pageRoless.Any(pr => pr.PageId == p.Id)).Select(p => new ListPages
        //    {
        //        PageName = p.Name,
        //        Url = p.Url
        //    }).ToList();
        //}
        //bool HasPages(int catId, IQueryable<PagesRole> pageRolee)
        //{
        //    return db.Pages.Where(p => !p.IsDeleted && pageRolee.Any(pr => pr.PageId == p.Id)).Any();
        //}
        [HttpPost]
        public ActionResult Login(string userName, string pass, string returnUrlVal)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pass))
            {
                ViewBag.MsgBadLogin = "تأكد من ادخال اسم المستخدم وكلمة المرور";
                return View();
            }
            ////التأكد من صلاحية فتح الموقع فى حالة الافلاين
            //if (!SecurityService.isSecured())
            //{
            //    ViewBag.MsgBadLogin = "ليس لك صلاحية لاستخدام الموقع ";
            //    return View();
            //}
            UserInfo user = UsersRoleService.VaildLogin(userName, pass);
            if (user != null)
            {
                List<DrawSideBar> pages = new List<DrawSideBar>();
                var userDb = db.Users.FirstOrDefault(x=>x.Id==user.UserId);
                var roleId = userDb.RoleId;
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //pages = UsersRoleService.GetPagesSideBar(roleId);
                //stopwatch.Stop();
                //refactor service -- remote 800 mili
                //remote 33 seconds,172 Mili
                //local 511 Mili
                //user.MyPages = pages.Select(x => x.PageId).ToList();
                auth.SaveToCookies(user);
                userDb.LastLogin = Utility.GetDateTime();
                userDb.SessionKey = VTSAuth.Encrypt(JsonConvert.SerializeObject(user.SessionKey));
                db.Entry(userDb).State = EntityState.Modified;
                db.SaveChanges(user.UserId);

                //UsersRoleService.PagesLoad = pages;
                //Session["pagesSession"] = pages;

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //var roleId = 1;

                //UsersRoleService.GetPages(roleId);
                //stopwatch.Stop();

                //Stopwatch stopwatch3 = new Stopwatch();
                //stopwatch3.Start();
                //var pageRoles = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == userDb.RoleId).GroupBy(pp => pp.Page.PageCatogryId);
                //var pages = pageRoles.Select(x => new SideBarPages
                //{
                //    CategoryName = x.Select(c => c.Page.PageCategory.Name).FirstOrDefault(),
                //    Pages = x.Select(p => new ListPages
                //    {
                //        PageName = p.Page.Name,
                //        Url = p.Page.Url
                //    }).ToList()
                //}).ToList();

                //stopwatch3.Stop();

                //Session["pagesSession"] = pages;
                if (!string.IsNullOrEmpty(returnUrlVal))
                    return Redirect(returnUrlVal);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.MsgBadLogin = "اسم المستخدم او كلمة المرور خاطئة";

            return View();
        }
        public ActionResult Error(string ErroeMsg)
        {
            //auth.ClearCookies();
            ViewBag.MsgBadLogin = ErroeMsg;
            return View();
        }
        // تفعيل الموقع افلاين
        [HttpPost]
        public ActionResult Activation(string activePass)
        {
            try
            {
                if (string.IsNullOrEmpty(activePass))
                {
                    TempData["MsgErrorActive"] = "رقم التفعيل خاطئ";
                    return RedirectToAction("Login");
                }
                string hashedValue = "F854C93C2F3F8D34257DD9B07024FB4B";

                var hashedString = VTSAuth.CreateMD5(activePass);
                if (hashedString == hashedValue)
                {
                    string pcSerial = SecurityService.getSerialID();
                    string serialEncrypted = VTSAuth.Encrypt(pcSerial);
                    var model = db.GeneralSettings.Where(x => !x.IsDeleted && x.Id == (int)GeneralSettingCl.EntityDataSecurity).FirstOrDefault();
                    model.SValue = serialEncrypted;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(null) > 0)
                    {
                        TempData["MsgSuccessActive"] = "تم التفعيل بنجاح";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["MsgErrorActive"] = "حدث خطأ اثناء تنفيذ العملية";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                        TempData["MsgErrorActive"] = "رقم التفعيل خاطئ";
                        return RedirectToAction("Login");
                                    }
            }
            catch (Exception ex)
            {
                {
                    TempData["MsgErrorActive"] = ex.Message;
                    return RedirectToAction("Login");
                }
            }

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