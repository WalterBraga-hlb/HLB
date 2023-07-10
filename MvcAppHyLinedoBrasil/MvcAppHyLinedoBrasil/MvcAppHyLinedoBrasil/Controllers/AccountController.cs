using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcAppHyLinedoBrasil.Models;
using System.DirectoryServices;
using System.Text;
using System.Collections;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using System.Threading;
using System.Globalization;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class AccountController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        private string _path;
        private string _filterAttribute;

        //public static ArrayList Direitos = new ArrayList();

        CHICDataSet chic = new CHICDataSet();

        salesmanTableAdapter salesman = new salesmanTableAdapter();

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            _path = "LDAP://DC=hylinedobrasil,DC=com,DC=br";

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            if (Session["alteracaoSenha"] != null)
            {
                if (Session["alteracaoSenha"].ToString() == "") Session.RemoveAll();
            }
            else
            {
                Session.RemoveAll();
                Session["alteracaoSenha"] = "";
            }

            Session["usuario"] = "0";
            Session["empresa"] = "BR";
            Session["empresaLayout"] = "BR";
            Session["Direitos"] = new ArrayList();
            Session["logo"] = "";
            Session["language"] = "pt-BR";

            //if (Request.Browser.IsMobileDevice)
            //{
            //    return Redirect("http://m.hlbapp.hyline.com.br");
            //}
            //else
            //{
            //    return View();
            //}

            //bookedTableAdapter bTA = new bookedTableAdapter();
            //bTA.UpdateLocationByCompany("CH", "BR");
            //bTA.UpdateLocationByCompany("AJ", "HN");
            //bTA.UpdateLocationByCompany("NM", "LB");
            //bTA.UpdateLocationByCompany("NM", "PL");


            //DateTime data = Convert.ToDateTime("10/07/2020 13:00");
            //var horas = (data - DateTime.Now).TotalHours;

            return View();
        }

        public void ConvertPDFtoExcel()
        {
            SautinSoft.ExcelToPdf f = new SautinSoft.ExcelToPdf();
            f.ConvertFile("", "");
        }

        public ActionResult SelecionaEmpresa(string Text)
        {
            //Session["empresa"] = Text;
            Session["empresaLayout"] = Text;

            List<SelectListItem> items = (List<SelectListItem>)Session["ListaEmpresas"];

            foreach (var item in items)
            {
                if (item.Value == Text)
                    item.Selected = true;
                else
                    item.Selected = false;
            }

            Session["ListaEmpresas"] = items;
                
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            Session["alteracaoSenha"] = "";

            if (ModelState.IsValid)
            {
                //if (MembershipService.ValidateUser(model.UserName, model.Password))
                //{
                //    FormsService.SignIn(model.UserName, model.RememberMe);
                //    if (Url.IsLocalUrl(returnUrl))
                //    {
                //        return Redirect(returnUrl);
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                //}

                try
                {
                    //if (Session == null)
                    //    Session = session;

                    Session["login"] = model.UserName;

                    if (model.UserName.IndexOf("@") > 0)
                    {
                        salesman.FillByEmail(chic.salesman, model.UserName, model.Password);

                        if (chic.salesman.Count > 0)
                        {
                            Session["tipoUsuario"] = "CHIC";
                            Session["usuario"] = chic.salesman[0].salesman;
                            Session["empresa"] = chic.salesman[0].inv_comp;
                            Session["empresaLayout"] = chic.salesman[0].inv_comp;

                            salesman.FillAllByEmail(chic.salesman, model.UserName);

                            List<SelectListItem> items = new List<SelectListItem>();

                            for (int i = 0; i < chic.salesman.Count ; i++)
                            {
                                string nome = "";

                                if (chic.salesman[i].inv_comp == "BR") { nome = "HY-LINE" ; }
                                else if (chic.salesman[i].inv_comp == "LB") { nome = "LOHMANN" ; }
                                else if (chic.salesman[i].inv_comp == "HN") { nome = "H&N"; }

                                bool select;

                                if (Session["empresaLayout"].ToString() == chic.salesman[i].inv_comp) { select = true; } else { select = false; }

                                items.Add(new SelectListItem { Text = nome, Value = chic.salesman[i].inv_comp, Selected = select });
                            }

                            Session["ListaEmpresas"] = items;

                            Session["QtdListaEmpresas"] = items.Count;
                            
                            //salesman.FillByAllInfoEmail(chic.salesman, model.UserName);

                            //string codVends = "'";

                            //for (int i = 0; i < chic.salesman.Count; i++)
                            //{
                            //    codVends = codVends + "'0" + chic.salesman[i].sl_code + "',";
                            //}

                            //codVends = codVends.Substring(1,codVends.Length-2);

                            //Session["codsVend"] = codVends;

                            Session["nomeEmpresa"] = CarregaLayout();

                            Session["Direitos"] = new ArrayList();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            //if (Url.IsLocalUrl(returnUrl))
                            //{
                                Session["usuario"] = "0";
                                ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique!");
                                //return Redirect(returnUrl);
                            //}
                        }
                    }
                    else
                    {
                        Session["empresa"] = "";
                        Session["empresaLayout"] = "";
                        Session["empresaApolo"] = "";
                        Session["tipoUsuario"] = "AD";

                        List<SelectListItem> items = new List<SelectListItem>();

                        items.Add(new SelectListItem { Text = "HY-LINE", Value = "BR", Selected = true });

                        Session["ListaEmpresas"] = items;

                        Session["QtdListaEmpresas"] = items.Count;

                        string domainAndUsername = "hyline" + @"\" + model.UserName;
                        //DirectoryEntry entry = new DirectoryEntry(@"\" + @"\hylinebr", domainAndUsername, model.Password);
                        DirectoryEntry entry = new DirectoryEntry();

                        entry.Path = _path;
                        entry.Username = domainAndUsername;
                        entry.Password = model.Password;

                        string retorno = VerificaSenha("hyline", model.UserName, model.Password);

                        // Senha Expirada (1330) ou Alteração Senha no Próximo Login (1907)
                        if (retorno.Equals("1330") || retorno.Equals("1907"))
                        {
                            ViewBag.PasswordLength = 8;
                            ChangePasswordModel changePassword = new ChangePasswordModel();
                            changePassword.OldPassword = model.Password;
                            return View("ChangePassword", changePassword);
                        }

                        //Bind to the native AdsObject to force authentication.
                        object obj = entry.NativeObject;

                        DirectorySearcher search = new DirectorySearcher(entry);

                        search.Filter = "(SAMAccountName=" + model.UserName + ")";
                        search.PropertiesToLoad.Add("cn");
                        SearchResult result = search.FindOne();

                        if (null == result)
                        {
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                Session["usuario"] = "0";
                                return Redirect(returnUrl);
                            }
                        }
                        else
                        {
                            //Update the new path to the user in the directory.
                            _path = result.Path;
                            _filterAttribute = (string)result.Properties["cn"][0];
                            Session["usuario"] = _filterAttribute;
                            GetGroups();
                            LoadSessionLanguage((System.Collections.ArrayList)Session["Direitos"]);

                            if (GetGroup("HLBAPP-AcessoHyline", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = "BR";
                                Session["empresaLayout"] = "BR";
                                Session["empresaApolo"] = "HY-LINE";
                            }
                            
                            if (GetGroup("HLBAPP-AcessoLohmann", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "LB";
                                Session["empresaLayout"] = "LB";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / LOHMANN";
                                else
                                    Session["empresaApolo"] = "LOHMANN";
                            }

                            if (GetGroup("HLBAPP-AcessoH&N", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "HN";
                                Session["empresaLayout"] = "HN";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / H & N";
                                else
                                    Session["empresaApolo"] = "H & N";
                            }

                            if (GetGroup("HLBAPP-AcessoPlanalto", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "PL";
                                Session["empresaLayout"] = "PL";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / PLANALTO";
                                else
                                    Session["empresaApolo"] = "PLANALTO";
                            }

                            if (Session["empresa"].ToString().Length == 8)
                            {
                                Session["empresaApolo"] = "TODAS";
                                Session["empresaLayout"] = "BR";
                            }
                            
                            //Session["empresaLayout"] = "LB";

                            Session["nomeEmpresa"] = CarregaLayout();
                            
                            if (GetGroup("HLBAPP-DashBoardProducao", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                return RedirectToAction("GranjasDiarioHome", "DashBoardProduction");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["usuario"] = "0";
                    //throw new Exception("Error authenticating user. " + ex.Message);
                    ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique! ");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            Session.RemoveAll();

            return RedirectToAction("LogOn", "Account");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                //{
                string retorno = ResetPassword(Session["login"].ToString(), model.OldPassword, model.NewPassword);
                //return RedirectToAction("ChangePasswordSuccess");
                if (retorno != "")
                    Session["alteracaoSenha"] = retorno;
                else
                    Session["alteracaoSenha"] = "Senha alterada com sucesso! Faça o Login com a Senha Nova!";
                return RedirectToAction("LogOn");
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                //}
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            //return View();
            ViewBag.Mensagem = "Senha alterada com sucesso! Faça o Login com a Senha Nova!";
            Session.RemoveAll();

            Session["usuario"] = "0";
            Session["empresa"] = "BR";
            Session["empresaLayout"] = "BR";
            Session["Direitos"] = new ArrayList();

            if (Request.Browser.IsMobileDevice)
            {
                return Redirect("http://m.hlbapp.hyline.com.br");
            }
            else
            {
                return View();
            }
        }
        
        public void GetGroups()
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + _filterAttribute + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();

            ArrayList Direitos = new ArrayList();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    //if (-1 == equalsIndex)
                    //{
                    //    return null;
                    //}
                    //groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                    //groupNames.Append("|");

                    Direitos.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                }

                Session["Direitos"] = Direitos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            //return groupNames.ToString();
        }

        public static bool GetGroup(string direito, ArrayList Direitos)
        {
            bool resultado = false;
            for (int i = 0; i < Direitos.Count; i++)
            {
                if (Direitos[i].ToString() == direito)
                {
                    resultado = true;
                    break;
                }
                else
                {
                    resultado = false;
                }
            }

            return resultado;
        }

        public static string GetTextOnLanguage(string caption, string language)
        {
            string result = "";
            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            Languages objLanguage = hlbapp.Languages
                .Where(w => w.Language == language && w.Caption == caption)
                .FirstOrDefault();

            if (objLanguage != null)
                result = objLanguage.Text;

            return result;
        }

        public static string Translate(string text, string language)
        {
            string result = AccountController.GetTextOnLanguage(text.Replace(":", ""), language);
            if (result == "") result = text;
            return result;
        }

        public void LoadSessionLanguage(ArrayList Direitos)
        {
            for (int i = 0; i < Direitos.Count; i++)
            {
                if (Direitos[i].ToString().Contains("HLBAPP-Language_"))
                {
                    int start = 16;
                    int end = Direitos[i].ToString().Length - start;
                    Session["language"] = Direitos[i].ToString().Substring(start, end);
                }
            }
        }

        public string VerificaSenha(string domain, string userName, string password)
        {
            string retorno = "";

            LoginAPIWindows verificaLogin = new LoginAPIWindows();
            retorno = verificaLogin.LogonUserAPI(domain, userName, password);

            return retorno;
        }

        public string ResetPassword(string userName, string oldPassword, string newPassword)
        {
            string retorno = "";
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                    {
                        //user.SetPassword("newpassword");
                        // or
                        user.ChangePassword(oldPassword, newPassword);
                        user.Save();
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
                return retorno;
            }
        }

        public string CarregaLayout()
        {
            string empresa = "";
            if (Session["empresa"].ToString() == "BR")
            {
                Session["logo"] = "BR";
                empresa = "Hy-Line do Brasil - Apps";
            }
            if (Session["empresa"].ToString() == "LB")
            {
                Session["logo"] = "LB";
                empresa = "Lohmann do Brasil - Apps";
            }
            if (Session["empresa"].ToString() == "HN")
            {
                Session["logo"] = "HN";
                empresa = "H&N Avicultura - Apps";
            }
            if (Session["empresa"].ToString().Count() > 2)
            {
                Session["logo"] = "ILD";
                empresa = "ILD - International Layer Distribuition - Apps";
            }
            //if (Session["empresa"].ToString().Contains("PL"))
            if (Session["empresa"].ToString() == "PL")
            {
                Session["logo"] = "PL";
                empresa = "Planalto Postura - Apps";
            }

            return empresa;
        }
    }
}
