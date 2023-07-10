using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using System.Collections;
using MvcAppHyLinedoBrasil.Models;
using System.DirectoryServices;
using System.Text;
using MvcAppHylinedoBrasilMobile.Models.MIX;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    [Authorize]
    [HandleError]
    public class AccountMobileController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        private static string _path;
        private static string _filterAttribute;

        //public static ArrayList Direitos = new ArrayList();

        //CHICDataSet chic = new CHICDataSet();

        //salesmanTableAdapter salesman = new salesmanTableAdapter();

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            _path = "LDAP://DC=hylinedobrasil,DC=com,DC=br";

            base.Initialize(requestContext);
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"] == "") Session.RemoveAll();
            }
            else
            {
                Session.RemoveAll();
                Session["urlChamada"] = "";
            }

            Session["email"] = "";
            Session["usuario"] = null;
            Session["empresa"] = "";
            Session["empresaApolo"] = "";
            Session["Direitos"] = new ArrayList();
            if (Session["language"] == null)
                Session["language"] = "pt-BR";
            GetLanguagesList();
            UpdateLanguageSelected(Session["language"].ToString());

            //if (Request.Url.Host.Equals("m.app.planaltopostura.com.br") ||
            //    Request.Url.Host.Equals("mappppa.ddns.net"))
            //    Session["corBody"] = "";
            //else
            Session["corBody"] = "page";

            return View();
        }

        public ActionResult SelecionaEmpresa(string Text)
        {
            Session["empresa"] = Text;

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

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (returnUrl == null)
                returnUrl = "AccountMobile/Login";

            Session["empresaApolo"] = "";
            Session["empresa"] = "";

            if (ModelState.IsValid)
            {
                try
                {
                    bdApoloEntities apoloStatic = new bdApoloEntities();

                    if (Session["language"] == null)
                        Session["language"] = "pt-BR";
                    GetLanguagesList();
                    UpdateLanguageSelected(Session["language"].ToString());

                    Session["login"] = model.UserName;
                    Session["vendedorParticipaControleRDV"] = "Não";

                    #region Se o usuario for adm para ter acesso a tudo

                    if (model.UserName == "adm" && model.Password == "@admhyl1n3")
                    {
                        Session["tipoUsuario"] = "ADMIN";
                        Session["usuario"] = "adm";

                        ArrayList listaDireitos = new ArrayList();
                        listaDireitos.Add("ADMIN");
                        Session["Direitos"] = listaDireitos;

                        Session["empresa"] = "BRLBHNPLLGNG";
                        Session["empresaApolo"] = "TODAS";

                        if (Session["urlChamada"] != "" && Session["urlChamada"] != null)
                        {
                            return Redirect(Session["urlChamada"].ToString());
                        }
                        else
                            return RedirectToAction("Index", "HomeMobile");
                    }

                    #endregion

                    #region Verifica se Usuario é do MIX para acessar as pesquisas

                    MixEntities mix = new MixEntities();

                    DateTime dataNascimento = DateTime.Today;
                    int resultVerificaSenha = 0;
                    long resultVerificaUsuario = 0;
                    if (model.Password.Length == 8 && Int32.TryParse(model.Password, out resultVerificaSenha)
                        && Int64.TryParse(model.UserName, out resultVerificaUsuario))
                        dataNascimento = new DateTime(Convert.ToInt32(model.Password.Substring(4, 4)), 
                            Convert.ToInt32(model.Password.Substring(2, 2)), Convert.ToInt32(model.Password.Substring(0, 2)));

                    FO_FUNCIONARIO funcionarioMIX = mix.FO_FUNCIONARIO
                        .Where(f => mix.FO_PESSOA.Any(a => f.PE_CODIGO == a.PE_CODIGO
                                && a.PE_CPF == model.UserName && a.PE_DT_NASC == dataNascimento)
                            && f.FU_STF_CODIGO != "D").FirstOrDefault();

                    #endregion

                    resultVerificaUsuario = 0;

                    if (funcionarioMIX != null)
                    {
                        #region Se for, carrega as variáveis de sessões

                        FO_PESSOA pessoaMIX = mix.FO_PESSOA.Where(p => p.PE_CODIGO == funcionarioMIX.PE_CODIGO).FirstOrDefault();

                        Session["tipoUsuario"] = "MIX";
                        Session["usuario"] = pessoaMIX.PE_NOME;

                        BS_ESTRUTURA empresa = mix.BS_ESTRUTURA
                            .Where(w => w.ES_ID_ESTRUTURA == funcionarioMIX.FU_ES_COD_EMPRESA).FirstOrDefault();

                        if (empresa.ES_RAZAO_SOCIAL.Contains("HY-LINE"))
                            Session["empresa"] = "BR";
                        else if (empresa.ES_RAZAO_SOCIAL.Contains("LOHMANN"))
                            Session["empresa"] = "LB";
                        else if (empresa.ES_RAZAO_SOCIAL.Contains("H E N"))
                            Session["empresa"] = "HN";
                        else if (empresa.ES_RAZAO_SOCIAL.Contains("PLANALTO"))
                            Session["empresa"] = "PL";
                        else if (empresa.ES_RAZAO_SOCIAL.Contains("LAYER"))
                            Session["empresa"] = "LG";

                        Session["empresaApolo"] = "MIX";

                        //if ((!Request.Url.Host.Equals("m.app.planaltopostura.com.br") &&
                        //            !Request.Url.Host.Equals("mappppa.ddns.net"))
                        //        && Session["empresa"].ToString().Equals("PL"))
                        //{
                        //    Session["usuario"] = "0";
                        //    ViewBag.Erro = "O usuário ou a senha estão incorretos. Verifique!";
                        //    return View("Login");
                        //}
                        //else
                        //{
                            if (Session["urlChamada"] != "" && Session["urlChamada"] != null)
                            {
                                return Redirect(Session["urlChamada"].ToString());
                            }
                            else
                                return RedirectToAction("Index", "HomeMobile");
                        //}

                        #endregion
                    }
                    else if (model.UserName.IndexOf("@") > 0)
                    {
                        #region Carrega as informações se o usuário for um vendedor do CHIC

                        //salesman.FillByEmail(chic.salesman, model.UserName, model.Password);
                        var vendedores = apoloStatic.VU_Vendedores
                            .Where(w => w.Login == model.UserName && w.Senha == model.Password)
                            .ToList();

                        if (vendedores.Count > 0)
                        {
                            Session["tipoUsuario"] = "CHIC";
                            Session["usuario"] = vendedores.FirstOrDefault().VendNome;
                            Session["email"] = model.UserName;

                            foreach (var item in vendedores.GroupBy(g => g.CodigoCHIC).ToList())
                            {
                                Session["empresa"] = Session["empresa"] + item.Key;
                            }

                            //salesman.FillAllByEmail(chic.salesman, model.UserName);

                            string email = model.UserName;

                            Models.bdApolo.bdApoloEntities apolo = new Models.bdApolo.bdApoloEntities();
                            //Models.bdApolo.VENDEDOR vendApolo = apolo.VENDEDOR
                            //    .Where(w => w.USERLoginSite == email).FirstOrDefault();

                            //Session["empresaApolo"] = vendApolo.USEREmpresa;

                            var listVendApolo = apolo.VENDEDOR
                                .Where(w => w.USERLoginSite == email)
                                .GroupBy(g =>
                                    new { g.USEREmpresa, g.USERParticipaControleRDVWeb }).ToList();

                            foreach (var item in listVendApolo)
                            {
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / " + item.Key.USEREmpresa;
                                else
                                    Session["empresaApolo"] = item.Key.USEREmpresa;

                                if (item.Key.USERParticipaControleRDVWeb != null)
                                    Session["vendedorParticipaControleRDV"] = item.Key.USERParticipaControleRDVWeb;
                            }

                            List<SelectListItem> items = new List<SelectListItem>();

                            foreach (var item in vendedores)
                            {
                                string nome = "";

                                if (item.CodigoCHIC == "BR") { nome = "HY-LINE"; }
                                else if (item.CodigoCHIC == "LB") { nome = "LOHMANN"; }
                                else if (item.CodigoCHIC == "HN") { nome = "H&N"; }

                                bool select;

                                if (Session["empresa"].ToString() == item.CodigoCHIC) { select = true; } else { select = false; }

                                items.Add(new SelectListItem { Text = nome, Value = item.CodigoCHIC, Selected = select });
                            }

                            Session["ListaEmpresas"] = items;

                            Session["Direitos"] = new ArrayList();

                            //if ((!Request.Url.Host.Equals("m.app.planaltopostura.com.br") &&
                            //        !Request.Url.Host.Equals("mappppa.ddns.net") &&
                            //        !Request.Url.Host.Contains("localhost"))
                            //    && Session["empresa"].ToString().Equals("PL"))
                            //{
                            //    Session["usuario"] = "0";
                            //    //ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique!");
                            //    //return Redirect(returnUrl);
                            //    ViewBag.Erro = "O usuário ou a senha estão incorretos. Verifique!";
                            //    return View("Login");
                            //}
                            //else
                            //{
                                if (Session["urlChamada"] != "" && Session["urlChamada"] != null)
                                    return Redirect(Session["urlChamada"].ToString());
                                else
                                    return RedirectToAction("Index", "HomeMobile");
                            //}
                        }
                        else
                        {
                            //if (Url.IsLocalUrl(returnUrl))
                            //{
                            //    Session["usuario"] = "0";
                            //    ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique!");
                            //    return Redirect(returnUrl);
                            //}
                            Session["usuario"] = "0";
                            //ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique!");
                            ViewBag.Erro = "O usuário ou a senha estão incorretos. Verifique!";
                            return View("Login");
                        }

                        #endregion
                    }
                    else if (!Int64.TryParse(model.UserName, out resultVerificaUsuario))
                    {
                        #region Se o usuário for do AD

                        //Session["empresa"] = "BR";
                        Session["tipoUsuario"] = "AD";

                        Session["vendedorParticipaControleRDV"] = "Não";

                        List<SelectListItem> items = new List<SelectListItem>();

                        //items.Add(new SelectListItem { Text = "HY-LINE", Value = "BR", Selected = true });

                        //Session["ListaEmpresas"] = items;

                        string domainAndUsername = "hyline" + @"\" + model.UserName;
                        //DirectoryEntry entry = new DirectoryEntry(@"\" + @"\hylinebr", domainAndUsername, model.Password);
                        DirectoryEntry entry = new DirectoryEntry();

                        entry.Path = _path;
                        entry.Username = domainAndUsername;
                        entry.Password = model.Password;

                        //Bind to the native AdsObject to force authentication.
                        object obj = entry.NativeObject;

                        DirectorySearcher search = new DirectorySearcher(entry);

                        search.Filter = "(SAMAccountName=" + model.UserName + ")";
                        //search.PropertiesToLoad.Add("cn");
                        SearchResult result = search.FindOne();

                        if (null == result)
                        {
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                Session["usuario"] = "0";
                                ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique! ");
                                //return Redirect(returnUrl);
                            }
                        }
                        else
                        {
                            //Update the new path to the user in the directory.
                            _path = result.Path;
                            _filterAttribute = (string)result.Properties["cn"][0];
                            Session["usuario"] = _filterAttribute;
                            Session["email"] = (string)result.Properties["mail"][0];
                            Session["Direitos"] = GetGroups();

                            #region Acesso Empresas

                            if (GetGroup("HLBAPP-AcessoHyline", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = "BR";
                                Session["empresaApolo"] = "HY-LINE";
                            }

                            if (GetGroup("HLBAPP-AcessoLohmann", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "LB";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / LOHMANN";
                                else
                                    Session["empresaApolo"] = "LOHMANN";
                            }

                            if (GetGroup("HLBAPP-AcessoH&N", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "HN";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / H & N";
                                else
                                    Session["empresaApolo"] = "H & N";
                            }

                            if (GetGroup("HLBAPP-AcessoPlanalto", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "PL";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / PLANALTO";
                                else
                                    Session["empresaApolo"] = "PLANALTO";
                            }
                            if (GetGroup("HLBAPP-AcessoLayer", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "LG";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / LAYER";
                                else
                                    Session["empresaApolo"] = "LAYER";
                            }
                            if (GetGroup("HLBAPP-AcessoNOVOGEN", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                Session["empresa"] = Session["empresa"] + "NG";
                                if (!Session["empresaApolo"].ToString().Equals(""))
                                    Session["empresaApolo"] = Session["empresaApolo"] + " / NOVOGEN";
                                else
                                    Session["empresaApolo"] = "NOVOGEN";
                            }

                            if (Session["empresa"].ToString().Length == 10)
                            {
                                Session["empresaApolo"] = "TODAS";
                            }

                            #endregion

                            if (Session["urlChamada"] != "" && Session["urlChamada"] != null)
                                return Redirect(Session["urlChamada"].ToString());
                            else
                                return RedirectToAction("Index", "HomeMobile");
                        }

                        //if ((!Request.Url.Host.Equals("m.app.planaltopostura.com.br") &&
                        //            !Request.Url.Host.Equals("mappppa.ddns.net"))
                        //        && Session["empresa"].ToString().Equals("PL"))
                        //{
                        //    Session["usuario"] = "0";
                        //    //ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique!");
                        //    //return Redirect(returnUrl);
                        //    ViewBag.Erro = "O usuário ou a senha estão incorretos. Verifique!";
                        //    return View("Login");
                        //}
                        //else
                        //{
                            if (Session["urlChamada"] != "" && Session["urlChamada"] != null)
                            {
                                return Redirect(Session["urlChamada"].ToString());
                            }
                            else
                                return RedirectToAction("Index", "HomeMobile");
                        //}

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    #region Erro ao carregar o usuário

                    Session["usuario"] = "0";
                    //throw new Exception("Error authenticating user. " + ex.Message);
                    //ModelState.AddModelError("", "O usuário ou a senha estão incorretos. Verifique! ");
                    string msg = "";
                    string tratMsg = "";
                    if (ex.Message.Equals("Falha de logon: nome de usuário desconhecido ou senha incorreta.\r\n"))
                        tratMsg = " Se a senha estiver correta, pode ser que a senha esteja expirada! " +
                            "Acesse o Terminal para mudar a senha ou entre em contato pelo ti@hyline.com.br!";
                    msg = ex.Message;
                    if (ex.InnerException != null)
                        msg = msg + " / " + ex.InnerException.Message;
                    ViewBag.Erro = "Erro ao realizar Login: " + msg + tratMsg;
                    return View("Login");

                    #endregion
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public static ArrayList GetGroups()
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

                return Direitos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            //return groupNames.ToString();
        }

        //
        // GET: /Account/LogOff

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            Session.RemoveAll();

            Session["usuario"] = null;
            Session["empresa"] = "BR";
            Session["Direitos"] = new ArrayList();
            if (Session["language"] == null)
                Session["language"] = "pt-BR";
            GetLanguagesList();
            UpdateLanguageSelected(Session["language"].ToString());

            return RedirectToAction("Login", "AccountMobile");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(MvcAppHylinedoBrasilMobile.Models.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(MvcAppHylinedoBrasilMobile.Models.ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public static bool GetGroup(string direito, ArrayList Direitos)
        {
            bool resultado = false;
            for (int i = 0; i < Direitos.Count; i++)
                {
                    if (Direitos[i].ToString() == direito || Direitos[i].ToString() == "ADMIN")
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
            string result = caption;
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Languages objLanguage = hlbapp.Languages
                .Where(w => w.Language == language && w.Caption == caption)
                .FirstOrDefault();

            if (objLanguage != null)
                result = objLanguage.Text;

            return result;
        }

        public void GetLanguagesList()
        {
            List<SelectListItem> languagesList = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var list = hlbapp.Languages
                .GroupBy(g => g.Language)
                .ToList();

            foreach (var item in list)
            {
                languagesList.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
            }

            Session["LanguagesList"] = languagesList;
        }

        public void UpdateLanguageSelected(string language)
        {
            List<SelectListItem> languagesList = (List<SelectListItem>)Session["LanguagesList"];

            foreach (var item in languagesList)
            {
                if (item.Value == language)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["LanguagesList"] = languagesList;
        }

        [AllowAnonymous]
        public ActionResult ChangeLanguage(string Text)
        {
            if (Text != null)
            {
                Session["language"] = Text;
                UpdateLanguageSelected(Text);
            }
            return View("Login");
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "A Usuário ou Senha estão incorretos! Verifique!";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
