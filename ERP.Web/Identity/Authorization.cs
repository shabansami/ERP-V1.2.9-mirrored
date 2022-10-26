using Newtonsoft.Json;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ERP.DAL.Utilites;
namespace ERP.Web.Identity
{
    public class Authorization : ActionFilterAttribute, IExceptionFilter
    {
        bool OutMenu=false;
        public Authorization(bool outMenu=false)
        {
            OutMenu = outMenu;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = filterContext.RequestContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RequestContext.RouteData.Values["action"].ToString();

            var controller2 = filterContext.Controller as Controller;
            if (controller != null)
            {
                VTSAuth auth = new VTSAuth();
                if (auth.LoadDataFromCookies())
                {
                    filterContext.Controller.TempData["userInfo"] = auth;

                    var roleId = auth.CookieValues.RoleId;
                    //التأكد من عمد انتها صلاحية الجلسة
                    if (Utility.GetDateTime()>auth.CookieValues.SessionKey.ExpireDate)
                        CheckReturn(filterContext);
                    IQueryable<PagesRole> pages = null;
                    using (var db = new VTSaleEntities())
                    {
                        if(!VTSAuth.IsDemo)
                        {
                            var sessionDb = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault().SessionKey;
                            var sessionCookie = VTSAuth.Encrypt(JsonConvert.SerializeObject(auth.CookieValues.SessionKey));
                            if (sessionDb != sessionCookie)
                                CheckReturn(filterContext, false, 4);
                        }

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        pages = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == roleId);
                        if (pages.Count()>0)
                        {
                            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                string url = "";
                                var param = filterContext.ActionParameters.Keys.Contains("id");
                                var tempdata = filterContext.Controller.TempData["model"];
                                if ((action == "Edit" && param)||(action == "CreateEdit" && tempdata != null))
                                {
                                    action = "Index";
                                    //var param = filterContext.ActionParameters["id"].ToString();
                                    url = $"/{controller.ToLower()}/{action.ToLower()}";
                                }
                                else if (action=="Edit")
                                {
                                    action = "CreateEdit";
                                    //var param = filterContext.ActionParameters["id"].ToString();
                                   url  = $"/{controller.ToLower()}/{action.ToLower()}";
                                }else if (controller == "PagesRoles")
                                {
                                    action = "AssignPages";
                                    url = $"/{controller.ToLower()}/{action.ToLower()}";
                                }else
                                    url = $"/{controller.ToLower()}/{action.ToLower()}";

                                stopwatch.Stop();

                                if(OutMenu)
                                    base.OnActionExecuting(filterContext);
                                else
                                {
                                    if (pages.Where(x => x.Page.Url.ToLower() == url || x.Page.OtherUrls.ToLower().Contains(url)).Any() /*&& pagesSession*/)
                                        base.OnActionExecuting(filterContext);
                                    else
                                        CheckReturn(filterContext, true);
                                }
                            }

                        }
                        else
                            CheckReturn(filterContext);
                    }
                }
                else
                    CheckReturn(filterContext);
            }

        }
        private bool CanBeHijaxed(ControllerContext controllerContext)
        {
            //https://gist.github.com/benfoster/5933215
            var request = controllerContext.RequestContext.HttpContext.Request;
            return request.IsAjaxRequest() && request.HttpMethod == "POST";
        }
        void CheckReturn(ActionExecutingContext filterContext,bool unAuth=false,int? ErrorNum=null)
        {
            VTSAuth auth = new VTSAuth();
            int typ = 1;
            if (unAuth)
                typ = 3;           
            if (ErrorNum!=null) //تستخدم فى حالة استخدام جلسه مختلفه 
                typ = 4;
            auth.ClearCookies();
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {

                filterContext.Result = new JsonResult
                {
                    Data = new { Error = "UnAutorized", Url = "/Default/Login" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return;
            }
            else
            {
                if (typ==3||typ==4)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Default", action = "Login", MsgBadLogin = typ }));
                    return;
                }else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Default", action = "Login", MsgBadLogin = typ, returnUrl = filterContext.HttpContext.Request.Url.AbsoluteUri.ToString() }));
                    return;
                }
            }

        }
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Default", action = "Error", ErroeMsg = filterContext.Exception.Message, returnUrl = filterContext.HttpContext.Request.Url.AbsoluteUri.ToString() }));
        }
    }
}