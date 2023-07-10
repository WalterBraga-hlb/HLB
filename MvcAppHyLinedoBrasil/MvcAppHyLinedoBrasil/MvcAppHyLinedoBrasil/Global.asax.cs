using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcAppHyLinedoBrasil.Infra;
using System.Data.Entity;

namespace MvcAppHyLinedoBrasil
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

            // Timer Programação Diária de Transportes
            MvcAppHyLinedoBrasil.WebForms.ProgDiarioTransp.IniciaTimer();

            // Timer Geração Nova Confirmação - AniPlan
            MvcAppHyLinedoBrasil.Controllers.ImportaPedidosCHICController.IniciaTimer();

            #endregion

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //string LanguageShortName = HttpContext.Current.Request["LANGUAGE"];
            //System.Threading.Thread.CurrentThread.CurrentCulture =
            //    new System.Globalization.CultureInfo(LanguageShortName);
            //System.Threading.Thread.CurrentThread.CurrentUICulture =
            //    new System.Globalization.CultureInfo(LanguageShortName);
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