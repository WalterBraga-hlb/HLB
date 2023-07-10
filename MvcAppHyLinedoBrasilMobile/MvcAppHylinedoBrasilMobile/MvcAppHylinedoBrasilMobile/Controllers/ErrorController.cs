using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error()
        {
            //return View();
            return RedirectToAction("GenericError", 
                new HandleErrorInfo(new HttpException(403, "Dont allow access the error pages"), "ErrorController", 
                    "Index"));
        }

        //public ActionResult GenericError(HandleErrorInfo exception)
        public ActionResult GenericError()
        {
            HandleErrorInfo exception = (HandleErrorInfo)Session["handleErrorInfo"];
            return View("Error", exception);
        }

        public ViewResult NotFound(HandleErrorInfo exception)
        {
            ViewBag.Title = "Page Not Found";
            return View("Error", exception);
        } 

    }
}
