using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using System.Data.Objects;
using System.Text.RegularExpressions;
using System.IO;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class AssistenciaTecnicaController : Controller
    {
        #region Menu

        public ActionResult MenuAssistenciaTecnica()
        {
            return View();
        }

        #endregion

        #region Manutenção

        #region RRC

        #region List Methods

        public List<VW_Dados_RRC> ListRRC(string pesquisaCliente, DateTime dataInicial, DateTime dataFinal,
            string tipoData, string status)
        {
            bdApoloEntities apolo = new bdApoloEntities();

            var lista = apolo.VW_Dados_RRC
                .Where(w => ((w.Data_do_Nascimento >= dataInicial && w.Data_do_Nascimento <= dataFinal && tipoData == "Nascimento")
                    || (w.Data_da_RRC >= dataInicial && w.Data_da_RRC <= dataFinal && tipoData == "RRC"))
                    && ((w.Data_Resposta_SAC != null && status == "Respondida") || (w.Data_Resposta_SAC == null && status == "Sem Resposta")
                        || (status == "(Todos)"))
                    && (w.Nome_Cliente.Contains(pesquisaCliente) || pesquisaCliente == ""))
                .ToList();

            List<VW_Dados_RRC> listaFiltrada = new List<VW_Dados_RRC>();

            foreach (var item in lista)
            {
                for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
                {
                    string empStr = Session["empresa"].ToString().Substring(i, 2);

                    if (item.Empresa == empStr) listaFiltrada.Add(item);
                }
            }

            return listaFiltrada;
        }

        public List<VW_Dados_RRC> FilterListRRC()
        {
            CleanSessions();

            string pesquisaCliente = Session["pesquisaClienteRRC"].ToString();
            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialRRC"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalRRC"].ToString());
            string tipoData = ((List<SelectListItem>)Session["FiltroDDLTipoData"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;
            string status = ((List<SelectListItem>)Session["FiltroDDLStatus"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;

            return ListRRC(pesquisaCliente, dataInicial, dataFinal, tipoData, status);
        }

        #endregion

        #region Lista RRC

        public ActionResult ListaRRC()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["msg"] = "";

            Session["ListaRRC"] = FilterListRRC();
            return View();
        }

        public ActionResult SearchRRC(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRRC"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRRC"]);
                Session["dataInicialRRC"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRRC"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRRC"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRRC"]);
                Session["dataFinalRRC"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRRC"].ToString());

            if (model["TipoData"] != null)
                AtualizaDDL(model["TipoData"], (List<SelectListItem>)Session["FiltroDDLTipoData"]);

            if (model["Status"] != null)
                AtualizaDDL(model["Status"], (List<SelectListItem>)Session["FiltroDDLStatus"]);

            if (model["pesquisaCliente"] != null) Session["pesquisaClienteRRC"] = model["pesquisaCliente"];

            #endregion

            Session["ListaRRC"] = ListRRC(Session["pesquisaClienteRRC"].ToString(), dataInicial, dataFinal,
                model["TipoData"], model["Status"]);
            return View("ListaRRC");
        }

        #endregion

        #region CRUD Methods

        public void CarregaRRC(string empresa, string especie, string serie, string nfnum)
        {
            bdApoloEntities apolo = new bdApoloEntities();

            VW_Dados_RRC rrc = apolo.VW_Dados_RRC.Where(w => w.EmpCod == empresa 
                && w.CtrlDFModForm == especie
                && w.CtrlDFSerie == serie
                && w.Nº_NF == nfnum).FirstOrDefault();

            if (rrc.Resposta != null)
                Session["respostaRRC"] = rrc.Resposta;
        }

        public ActionResult OK()
        {
            //if (Session["msg"] != null) ViewBag.Mensagem = Session["msg"];

            return View();
        }

        #endregion

        #region Event Methods

        public ActionResult SolucaoRRC(string empresa, string especie, string serie, string nfnum)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["empresaSelecionada"] = empresa;
            Session["especieSelecionada"] = especie;
            Session["serieSelecionada"] = serie;
            Session["nfnumSelecionada"] = nfnum;

            CarregaRRC(empresa, especie, serie, nfnum);

            return View("SolucaoRRC");
        }

        public ActionResult ReturnListaRRC()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            return View("ListaRRC");
        }

        public ActionResult SaveSolucaoRRC(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            Models.bdApolo2.Apolo10Entities apolo = new Models.bdApolo2.Apolo10Entities();
            bdApoloEntities apolo2 = new bdApoloEntities();

            #endregion

            if (model["solucaoRRC"] != null)
            {
                #region Carrega Valores

                #region Login

                string login = Session["login"].ToString().ToUpper();

                #endregion

                #region Solução

                string solucaoRRC = "";
                if (model["solucaoRRC"] != null) solucaoRRC = model["solucaoRRC"];

                #endregion

                #endregion

                #region Salva na RRC

                string empresa = Session["empresaSelecionada"].ToString();
                string especie = Session["especieSelecionada"].ToString();
                string serie = Session["serieSelecionada"].ToString();
                string nfnum = Session["nfnumSelecionada"].ToString();

                Models.bdApolo2.NOTA_FISCAL nf = apolo.NOTA_FISCAL
                    .Where(w => w.EmpCod == empresa && w.CtrlDFModForm == especie && w.CtrlDFSerie == serie
                        && w.NFNum == nfnum).FirstOrDefault();

                nf.USERDataRespSAC = DateTime.Now;

                #region Preenche Solução Apolo

                int qtdeCaracteresTotal = solucaoRRC.Length;
                int qtdeCaracteres = 0;
                int qtdeCaracteresFinal = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (qtdeCaracteres <= qtdeCaracteresTotal)
                    {
                        if ((qtdeCaracteresTotal - qtdeCaracteres) <= 255)
                            qtdeCaracteresFinal = (qtdeCaracteresTotal - qtdeCaracteres);
                        else
                            qtdeCaracteresFinal = 255;

                        string parteSolucao = solucaoRRC.Substring(qtdeCaracteres, qtdeCaracteresFinal);

                        if (i == 0) nf.USERRespSAC01 = parteSolucao;
                        if (i == 1) nf.USERRespSAC02 = parteSolucao;
                        if (i == 2) nf.USERRespSAC03 = parteSolucao;
                        if (i == 3) nf.USERRespSAC04 = parteSolucao;
                        if (i == 4) nf.USERRespSAC06 = parteSolucao;
                    }

                    qtdeCaracteres = qtdeCaracteres + qtdeCaracteresFinal;
                }

                #endregion

                #region Gera Log

                VW_LOG_NFE_REL vlog = apolo2.VW_LOG_NFE_REL
                    .Where(w => w.LogNFERelEmpCod == nf.EmpCod && w.LogNFERelModelo == nf.CtrlDFModForm
                        && w.LogNFERelSerie == nf.CtrlDFSerie && w.LogNFERelNum == nf.NFNum
                        && w.LogNFERelString == "Resposta de RRC.")
                    .FirstOrDefault();

                if (vlog == null)
                {
                    LOG_NFE_REL log = new LOG_NFE_REL();
                    log.LogNFERelEmpCod = nf.EmpCod;
                    log.LogNFERelModelo = nf.CtrlDFModForm;
                    log.LogNFERelSerie = nf.CtrlDFSerie;
                    log.LogNFERelNum = nf.NFNum;
                    log.LogNFERelDtHoraImp = DateTime.Now;
                    log.LogNFERelQtdCopia = 1;
                    log.LogNFERelString = "Resposta de RRC.";
                    log.LogNFERelUsuCod = login;

                    apolo2.LOG_NFE_REL.AddObject(log);
                }

                apolo2.SaveChanges();

                #endregion

                apolo.SaveChanges();

                #endregion

                #region Envia E-mail da Solução

                #region Gera o E-mail

                #region Carrega Dados

                string stringChar = "" + (char)13 + (char)10;

                string emailSAC = "hyline.com.br";
                if (empresa == "12" || empresa == "21") emailSAC = "ltz.com.br";
                if (empresa == "15") emailSAC = "hnavicultura.com.br";
                if (empresa == "20") emailSAC = "planaltopostura.com.br";
                
                Models.bdApolo2.USUARIO usuarioResolvido = apolo.USUARIO
                    .Where(w => w.UsuCod == login).FirstOrDefault();

                PED_VENDA1 pedVenda1 = apolo2.PED_VENDA1
                    .Where(w => w.EmpCod == empresa && w.PedVendaNum == nf.NFPedVenda).FirstOrDefault();

                string paraNome = "SAC";
                string paraEmail = "sac@" + emailSAC;
                //string paraEmail = "palves@hyline.com.br";
                string copiaPara = "";

                #endregion

                string assunto = "RRC - SOLUÇÃO NF " + nfnum;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "A RRC da nota fiscal " + nfnum + " foi respondida pelo usuário " + usuarioResolvido.UsuNome + "." + stringChar
                    + "Segue relatório de rastreabilidade em anexo e abaixo a resposta: " + stringChar + stringChar
                    + solucaoRRC
                    + stringChar + stringChar
                    + "SISTEMA WEB";

                if (pedVenda1 != null)
                    anexos = GeraRelRastreabilidadePedido(pedVenda1.USERPEDCHIC, nf.NFENTNOME);

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                #endregion

                #endregion
            }

            Session["metodoRetorno"] = "ListaRRC";
            return RedirectToAction("OK", "AssistenciaTecnica");
        }

        public string GeraRelRastreabilidadePedido(string orderNo, string cliente)
        {
            string pattern = @"(?i)[^0-9a-z\s]";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string nameFileOld = cliente.Replace("\\", "").Replace("/", "");
            string nameFileNew = rgx.Replace(nameFileOld, replacement);

            string pattern2 = @"(?i)[^0-9a-z]";
            Regex rgx2 = new Regex(pattern2);
            string replacement2 = "_";
            string nameFileNew2 = rgx2.Replace(nameFileNew, replacement2);

            string caminho = @"\\srv-riosoft-01\W\RRC\" + nameFileNew2 + "_" + orderNo + ".pdf";

            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            MyReport.Load("C:\\inetpub\\wwwroot\\Relatorios\\Crystal\\RastreabilidadePedido.rpt");
            MyReport.SetParameterValue("@pPedido", orderNo);
            MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
            MyReport.SetDatabaseLogon("sa", "", "APOLO", "Apolo10");
            MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminho);

            MyReport.Close();
            MyReport.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return caminho;
        }

        public ActionResult GerarRelRastreabilidadePedido(string empresa, string especie, string serie, string nfnum,
            bool download)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Inicializa Entity

            Models.bdApolo2.Apolo10Entities apolo = new Models.bdApolo2.Apolo10Entities();
            bdApoloEntities apolo2 = new bdApoloEntities();

            #endregion

            Models.bdApolo2.NOTA_FISCAL nf = apolo.NOTA_FISCAL
                .Where(w => w.EmpCod == empresa && w.CtrlDFModForm == especie && w.CtrlDFSerie == serie
                    && w.NFNum == nfnum).FirstOrDefault();

            PED_VENDA1 pedVenda1 = apolo2.PED_VENDA1
                .Where(w => w.EmpCod == empresa && w.PedVendaNum == nf.NFPedVenda).FirstOrDefault();

            string pattern = @"(?i)[^0-9a-z\s]";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string nameFileOld = nf.NFENTNOME.Replace("\\", "").Replace("/", "");
            string nameFileNew = rgx.Replace(nameFileOld, replacement);

            string pattern2 = @"(?i)[^0-9a-z]";
            Regex rgx2 = new Regex(pattern2);
            string replacement2 = "_";
            string nameFileNew2 = rgx2.Replace(nameFileNew, replacement2);

            //string destino = GeraRelRastreabilidadePedido(pedVenda1.USERPEDCHIC, nf.NFENTNOME);

            string caminho = @"\\srv-riosoft-01\W\RRC\" + nameFileNew2 + "_" + pedVenda1.USERPEDCHIC + ".pdf";

            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            MyReport.Load("C:\\inetpub\\wwwroot\\Relatorios\\Crystal\\RastreabilidadePedido.rpt");
            MyReport.SetParameterValue("@pPedido", pedVenda1.USERPEDCHIC);
            MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
            MyReport.SetDatabaseLogon("sa", "", "APOLO", "Apolo10");
            MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminho);

            if (download)
            {
                Stream stream = MyReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType
                    .PortableDocFormat);

                MyReport.Close();
                MyReport.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return File(stream, "application/pdf", nameFileNew2 + "_" + pedVenda1.USERPEDCHIC + ".pdf");
            }
            else
            {
                var response = System.Web.HttpContext.Current.Response;
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                    response, false, nameFileNew2 + "_" + pedVenda1.USERPEDCHIC + ".pdf");

                MyReport.Close();
                MyReport.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                return new EmptyResult();
            }
        }

        #endregion

        #endregion

        #endregion

        #region Populate / Update Lists

        public List<SelectListItem> AtualizaDDL(string text, List<SelectListItem> lista)
        {
            List<SelectListItem> listItens = lista;

            foreach (var item in listItens)
            {
                if (item.Value == text)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            return listItens;
        }

        public List<SelectListItem> CarregaListaTipoData()
        {
            List<SelectListItem> ddlLista = new List<SelectListItem>();

            ddlLista.Add(new SelectListItem
            {
                Text = "Nascimento",
                Value = "Nascimento",
                Selected = true
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "RRC",
                Value = "RRC",
                Selected = false
            });

            return ddlLista;
        }

        public List<SelectListItem> CarregaListaStatus(bool todos)
        {
            List<SelectListItem> ddlLista = new List<SelectListItem>();

            if (todos)
            {
                ddlLista.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            ddlLista.Add(new SelectListItem
            {
                Text = "Respondida",
                Value = "Respondida",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Sem Resposta",
                Value = "Sem Resposta",
                Selected = false
            });

            return ddlLista;
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region RRC

            if (Session["pesquisaClienteRRC"] == null) Session["pesquisaClienteRRC"] = "";
            if (Session["dataInicialRRC"] == null) Session["dataInicialRRC"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (Session["dataFinalRRC"] == null) Session["dataFinalRRC"] = DateTime.Today;
            if (Session["FiltroDDLTipoData"] == null) Session["FiltroDDLTipoData"] = CarregaListaTipoData();
            if (Session["FiltroDDLStatus"] == null) Session["FiltroDDLStatus"] = CarregaListaStatus(true);

            Session["empresaSelecionada"] = "";
            Session["especieSelecionada"] = "";
            Session["serieSelecionada"] = "";
            Session["nfnumSelecionada"] = "";
            Session["respostaRRC"] = "";

            #endregion
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

        public void EnviarEmail(string paraNome, string paraEmail, string copiaPara,
            string assunto, string corpoEmail, string anexos, string empresaApolo, string formato)
        {
            MvcAppHyLinedoBrasil.Models.Apolo.WORKFLOW_EMAIL email =
                new MvcAppHyLinedoBrasil.Models.Apolo.WORKFLOW_EMAIL();

            email.WorkFlowEmailCopiaPara = copiaPara;

            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

            MvcAppHyLinedoBrasil.Models.Apolo.ApoloEntities apolo =
                new MvcAppHyLinedoBrasil.Models.Apolo.ApoloEntities();

            apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
            email.WorkFlowEmailStat = "Enviar";
            email.WorkFlowEmailData = DateTime.Now;
            email.WorkFlowEmailParaNome = paraNome;
            email.WorkFlowEmailParaEmail = paraEmail;
            //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
            //email.WorkFlowEmailParaNome = "Teste";
            //email.WorkFlowEmailCopiaPara = email.WorkFlowEmailCopiaPara + ";programacao@hyline.com.br";
            email.WorkFlowEmailCopiaPara = email.WorkFlowEmailCopiaPara;
            email.WorkFlowEmailDeNome = "Sistema WEB";
            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
            email.WorkFlowEmailFormato = formato;
            if (assunto.Length > 80) assunto = assunto.Substring(0, 80);
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexos;
            email.WorkFlowEmailDocEmpCod = empresaApolo;

            apolo.WORKFLOW_EMAIL.AddObject(email);

            apolo.SaveChanges();
        }

        #endregion
    }
}
