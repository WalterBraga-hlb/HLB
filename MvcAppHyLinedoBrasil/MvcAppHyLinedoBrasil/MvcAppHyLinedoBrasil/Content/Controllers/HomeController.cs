using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using System.Text;
using System.Collections;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class HomeController : Controller
    {
        private string _path;
        private string _filterAttribute;

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

        public ActionResult Index()
        {
            try
            {
                if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

                if (Session["usuario"].ToString() != "0")
                {
                    _path = "LDAP://DC=hylinedobrasil,DC=com,DC=br";
                    _filterAttribute = Session["usuario"].ToString();

                    //GetGroups();

                    ViewBag.Title = ViewBag.Message;

                    if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-DashBoardProducao", (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        return RedirectToAction("GranjasDiarioHome", "DashBoardProduction");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("LogOn", "Account");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("LogOn", "Account");
            }
        }

        public ActionResult About()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        //public void GetGroups()
        //{
        //    DirectorySearcher search = new DirectorySearcher(_path);
        //    search.Filter = "(cn=" + _filterAttribute + ")";
        //    search.PropertiesToLoad.Add("memberOf");
        //    StringBuilder groupNames = new StringBuilder();

        //    ArrayList Direitos = new ArrayList();

        //    try
        //    {
        //        SearchResult result = search.FindOne();
        //        int propertyCount = result.Properties["memberOf"].Count;
        //        string dn;
        //        int equalsIndex, commaIndex;

        //        for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
        //        {
        //            dn = (string)result.Properties["memberOf"][propertyCounter];
        //            equalsIndex = dn.IndexOf("=", 1);
        //            commaIndex = dn.IndexOf(",", 1);
        //            //if (-1 == equalsIndex)
        //            //{
        //            //    return null;
        //            //}
        //            //groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
        //            //groupNames.Append("|");

        //            AccountController.Direitos.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error obtaining group names. " + ex.Message);
        //    }
        //    //return groupNames.ToString();
        //}
    }
}
