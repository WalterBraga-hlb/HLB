using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcAppHylinedoBrasilMobile.Infra;
using MvcAppHylinedoBrasilMobile.Models;
using System.Collections;
using System.Timers;

namespace MvcAppHylinedoBrasilMobile
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            #region Rotinas Automáticas

            // Timer GEP
            MvcAppHylinedoBrasilMobile.Controllers.GEPController.IniciaTimer();

            #endregion

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "AccountMobile", action = "Login", id = UrlParameter.Optional } // Parameter defaults
            );

            //routes.MapRoute(
            //    "Default", // Route name
            //    "{controller}/{action}/{id}", // URL with parameters
            //    new { controller = "NavisionIntegrationApp", action = "OrdersCalendar", id = UrlParameter.Optional } // Parameter defaults
            //);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Exception exception = Server.GetLastError();
            //System.Diagnostics.Debug.WriteLine(exception);
            //Response.Redirect("/Error/Error");

            var httpContext = ((MvcApplication)sender).Context;
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null 
                    && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null 
                    && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();
            //var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "GenericError";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;

                    // others if any
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;
            routeData.Values["exception"] = new HandleErrorInfo(ex, currentController, currentAction);

            #region Reinicia as sessões - DESATIVADO

            //Session.RemoveAll();

            //Session["usuario"] = null;
            //Session["empresa"] = "BR";
            //Session["Direitos"] = new ArrayList();
            //if (Session["language"] == null)
            //    Session["language"] = "pt-BR";

            //List<SelectListItem> languagesList = new List<SelectListItem>();

            //HLBAPPEntities hlbapp = new HLBAPPEntities();
            //var list = hlbapp.Languages
            //    .GroupBy(g => g.Language)
            //    .ToList();

            //foreach (var item in list)
            //{
            //    languagesList.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
            //}

            //foreach (var item in languagesList)
            //{
            //    if (item.Value == Session["language"].ToString())
            //    {
            //        item.Selected = true;
            //    }
            //    else
            //    {
            //        item.Selected = false;
            //    }
            //}

            //Session["LanguagesList"] = languagesList;

            Session["handleErrorInfo"] = new HandleErrorInfo(ex, currentController, currentAction);

            #endregion

            //IController errormanagerController = new MvcAppHylinedoBrasilMobile.Controllers.ErrorController();
            //HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
            //var rc = new RequestContext(wrapper, routeData);
            //errormanagerController.Execute(rc);
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session != null)
            {
                if (Session["language"] != null)
                {
                    string languageShortName = Session["language"].ToString();
                    System.Threading.Thread.CurrentThread.CurrentCulture =
                        new System.Globalization.CultureInfo(languageShortName);
                    System.Threading.Thread.CurrentThread.CurrentUICulture =
                        new System.Globalization.CultureInfo(languageShortName);
                }
            }
        }
    }
}