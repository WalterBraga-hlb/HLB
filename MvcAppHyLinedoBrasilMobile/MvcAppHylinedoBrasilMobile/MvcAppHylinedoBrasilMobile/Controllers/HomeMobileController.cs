using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    [HandleError]
    public class HomeMobileController : Controller
    {
        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"].ToString() != "0")
            {
                ViewBag.Message = "Web Mobile Apps";
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }

            return View();
        }

        public ActionResult About()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"].ToString() != "0")
            {
                ViewBag.Message = "Your app description page.";
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }

            return View();
        }

        public ActionResult Contact()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            ViewBag.Message = "Your contact page.";

            return View();
        }

        public bool VerificaSessao()
        {
            if (Session["usuario"] == null)
            {
                return true;
            }
            else
            {
                if (Session["usuario"].ToString() == "0")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
