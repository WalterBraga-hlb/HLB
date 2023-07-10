using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Models.Apolo;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters;
using System.IO;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using ImportaIncubacao.Data.Apolo;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using am = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class DiarioExpedicaoController : Controller
    {
        #region Objetos

        private LayoutDb db = new LayoutDb();
        //public static HLBAPPEntities hlbapp = new HLBAPPEntities();
        private ApoloEntities apolo = new ApoloEntities();

        MvcAppHyLinedoBrasil.Data.FLIPDataSet flip = new MvcAppHyLinedoBrasil.Data.FLIPDataSet();
        FLIPDataSetMobile flipMobile = new FLIPDataSetMobile();
        bdApoloEntities bdApolo = new bdApoloEntities();
        ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();

        FLOCKS1TableAdapter nucleos = new FLOCKS1TableAdapter();
        FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();
        HATCHERY_FLOCK_DATATableAdapter hatcheryFlockData = new HATCHERY_FLOCK_DATATableAdapter();

        FLOCK_DATATableAdapter flock_data = new FLOCK_DATATableAdapter();
        ImportaIncubacao.Data.FLIPDataSetTableAdapters.FARMS_IMPORTTableAdapter farms = new ImportaIncubacao.Data.FLIPDataSetTableAdapters.FARMS_IMPORTTableAdapter();

        EGGINV_DATATableAdapter eggInvData = new EGGINV_DATATableAdapter();

        //public static List<Lotes> listaLotes = new List<Lotes>();

        //private static List<ImportaDiarioExpedicao> listaItensDadosAntigosImport = new List<ImportaDiarioExpedicao>();
        //private static List<LayoutDiarioExpedicao> listaItensDadosAntigos = new List<LayoutDiarioExpedicao>();

        //public static string loteEscolhido;

        //public static string escondeLinkPrincipal = "Não";
        //public static string location = "";
        //public static int qtdItemAnteriorAlterada = 0;

        //public static string escondeAddItem = "Não";
        //public static string operacao = "";

        #endregion

        #region Menu

        public ActionResult MenuControleEstoqueOvos()
        {
            return View("_MenuDEO");
        }

        #endregion

        #region Métodos de E-mail

        public void EnviarEmail(string corpoEmail, string assunto, string paraNome, string paraEmail,
            string copiaPara, string anexo, string empresaApolo, string tipo)
        {
            MvcAppHyLinedoBrasil.Models.Apolo.WORKFLOW_EMAIL email = new MvcAppHyLinedoBrasil.Models.Apolo.WORKFLOW_EMAIL();

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

            apolo.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
            email.WorkFlowEmailStat = "Enviar";
            //email.WorkFlowEmailAssunto = "**** LOGIN PARA ACESSO AO HY-LINE APP ****";
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailData = DateTime.Now;
            email.WorkFlowEmailParaNome = paraNome;
            email.WorkFlowEmailParaEmail = paraEmail;
            //email.WorkFlowEmailParaNome = "Paulo Alves";
            //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
            email.WorkFlowEmailCopiaPara = copiaPara;
            email.WorkFlowEmailDeNome = "Sistema WEB";
            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
            email.WorkFlowEmailFormato = tipo;
            email.WorkFlowEmailDocEmpCod = empresaApolo;

            //corpoEmail = "Prezado," + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Para melhorarmos o controle de nossos processos, foi desenvolvida a ferramenta para preenchimento e importação de Pedidos. " + (char)13 + (char)10
            //    + "Através dela iremos diminuir os erros para acelerar e melhorar os processos." + (char)13 + (char)10
            //    + "Sendo assim, segue abaixo o login e senha para acesso ao site para dados da empresa " + empresa + "." + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Login: " + dsCHIC.salesman1[i].email.Trim() + (char)13 + (char)10
            //    + "Senha: " + dsCHIC.salesman1[i].senha.Trim() + (char)13 + (char)10 + (char)13 + (char)10
            //    + "Também, segue em anexo o manual para acesso ao site." + (char)13 + (char)10
            //    + "Qualquer dúvida, entrar em contato pelo e-mail ti@hyline.com.br." + (char)13 + (char)10 + (char)13 + (char)10
            //    + "SISTEMA WEB";

            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexo;

            apolo.WORKFLOW_EMAIL.AddObject(email);

            apolo.SaveChanges();
        }

        public void EnviarEmailImpressora(string caminho)
        {
            string corpoEmail = "";
            string assunto = "**** IMPRESSÃO DEO " + Session["dataHoraCarreg"].ToString() + " - " + Session["login"].ToString() + " ****";
            string paraNome = "Impressora";
            string paraEmail = "atdr3664uba35@hpeprint.com";
            string copiaPara = "";

            EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, caminho, "5", "Texto");
        }

        #endregion

        #region Métodos Cadastro DEO

        public ActionResult ListaDEOs()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["listLotes"] = new List<Lotes>();
            //Session["listaItensDadosAntigosImport"] = new List<ImportaDiarioExpedicao>();
            Session["listaItensDadosAntigos"] = new List<LayoutDiarioExpedicaos>();
            Session["loteEscolhido"] = "";
            Session["escondeLinkPrincipal"] = "Não";
            Session["location"] = "";
            Session["qtdItemAnteriorAlterada"] = 0;
            Session["escondeAddItem"] = "Não";
            Session["operacao"] = "";
            Session["incubatorioDestinoSelecionado"] = "";
            //Session["granjaSelecionada"] = "";

            CarregaListaGranjas(false);
            string granja = "";
            if (Session["granjaSelecionada"] != null)
                granja = Session["granjaSelecionada"].ToString();
            AtualizaGranjaSelecionada(granja);

            Session["isIncubatorio"] = IsIncubatorio(granja);

            if (granja.Equals("SB") || granja.Equals("PH"))
                Session["location"] = "GP";
            else
                Session["location"] = "PP";

            Session["ListaTiposDEOFiltro"] = CarregaListaTiposDEO(true);
            string tipoDEO = "(Todos os Tipos)";
            //if (Session["tipoDEOselecionado"] != null)
            //    tipoDEO = Session["tipoDEOselecionado"].ToString();
            Session["tipoDEOselecionado"] = tipoDEO;
            Session["ListaTiposDEOFiltro"] = AtualizaTipoDEOSelecionado(tipoDEO,
                (List<SelectListItem>)Session["ListaTiposDEOFiltro"]);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), false, false);
            CarregaListaTipoVisualizacaoQtde();

            return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
        }

        public ActionResult PrintDEO(DateTime dataFiltro, bool download, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    string caminho = @"\\srv-riosoft-01\W\DEOs\DEO_" + Session["login"].ToString() + "_" 
                        + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";

                    Session["dataHoraCarreg"] = dataFiltro;

                    CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = 
                        new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                    MyReport.Load(Server.MapPath("~/Reports/DiarioExpedicao.rpt"));

                    MyReport.ParameterFields["DataHoraCarreg"].CurrentValues.AddValue(dataFiltro);

                    if (download)
                    {
                        Stream stream = MyReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "DiarioExpedicao.pdf");
                    }
                    else
                    {
                        var response = System.Web.HttpContext.Current.Response;
                        MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, response, false, "DEO_" + dataFiltro.ToShortDateString());
                        return new EmptyResult();
                    }

                    MyReport.Close();
                    MyReport.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    //return View("ImpressaoDEO");
                    //return new EmptyResult();
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult PrintConfereDEO(DateTime dataFiltro, bool download, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    string caminho = @"\\srv-riosoft-01\W\DEOs\DEOConf_" + Session["login"].ToString() + "_"
                        + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";

                    Session["dataHoraCarreg"] = dataFiltro;

                    CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport =
                        new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                    MyReport.Load(Server.MapPath("~/Reports/DiarioExpedicaoConf.rpt"));

                    MyReport.ParameterFields["NumIdentificacao"].CurrentValues.AddValue(numIdentificacao);

                    if (download)
                    {
                        Stream stream = MyReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "DiarioExpedicaoConf.pdf");
                    }
                    else
                    {
                        var response = System.Web.HttpContext.Current.Response;
                        MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, response, false, "DEOConf_" + dataFiltro.ToShortDateString());
                        return new EmptyResult();
                    }

                    MyReport.Close();
                    MyReport.Dispose();

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    //return View("ImpressaoDEO");
                    //return new EmptyResult();
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult CreateDEO(bool transferenciaLinhagens)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                //Session["listaItensDadosAntigosImport"] = new List<ImportaDiarioExpedicao>();
                Session["listaItensDadosAntigos"] = new List<LayoutDiarioExpedicaos>();
                if (Session["usuario"].ToString() != "0")
                {
                    CarregaListaNucleos();
                    CarregaTipoOvo();
                    Session["dataProducaoSelecionada"] = DateTime.Today;
                    Session["loteCompletoSelecionado"] = "";

                    string granja = Session["granjaSelecionada"].ToString();

                    bool localOvosComercio = false;
                    if (granja.Length == 3) if (granja.Substring(2, 1) == "C") localOvosComercio = true;

                    Session["ListaTiposDEO"] = CarregaListaTiposDEO(false);

                    if (transferenciaLinhagens)
                    {
                        Session["TransferenciaLinhagens"] = transferenciaLinhagens;
                        CarregaLinhagensOrigem(granja);
                        Session["ListaLinhagemDestino"] = new List<SelectListItem>();
                        Session["tipoDEOselecionado"] = "Transferência entre Linhagens";
                    }
                    else
                    {
                        Session["tipoDEOselecionado"] = ((List<SelectListItem>)Session["ListaTiposDEO"])
                            .FirstOrDefault().Value;
                    }

                    bdApolo.CommandTimeout = 1000;
                    apoloService.CommandTimeout = 1000;

                    Session["operacao"] = "Create";

                    DateTime dataFiltro = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    DateTime dataVerifica = DateTime.Today;

                    Session["dataDEO"] = DateTime.Now.ToString("dd/MM/yyyy");
                    Session["dataRecebInc"] = "";
                    Session["horaDEO"] = DateTime.Now.ToString("HH:mm");
                    Session["dataHoraCarreg"] = dataFiltro;

                    //DateTime teste = Convert.ToDateTime(Session["dataHoraCarreg"]);

                    Session["nfNum"] = "";
                    Session["Observacao"] = "";
                    Session["GTA"] = "";
                    Session["Lacre"] = "";
                    string tipoDEO = Session["tipoDEOselecionado"].ToString();

                    bool isOCGranja = false;
                    bool isGranja = false;
                    Models.bdApolo.EMPRESA_FILIAL empresaGranja = bdApolo.EMPRESA_FILIAL
                        .Where(w => w.USERFLIPCod == granja).FirstOrDefault();
                    Models.bdApolo.ENTIDADE1 entidade = bdApolo.ENTIDADE1
                        .Where(w => w.USERFLIPCodigo == granja).FirstOrDefault();
                    if (empresaGranja != null)
                    {
                        if (empresaGranja.USERTipoUnidadeFLIP == "Granja" && tipoDEO == "Ovos p/ Comércio") isOCGranja = true;
                        if (empresaGranja.USERTipoUnidadeFLIP == "Granja") isGranja = true;
                    }
                    else if (entidade != null)
                    {
                        isGranja = true;
                    }

                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    if (dataVerifica >= Convert.ToDateTime("15/12/2021")
                        && (granja == "CH" || granja == "NM" || granja == "HL" || granja == "CG" || granja == "GE" || granja == "SD" || granja == "SJP01" || granja == "SJP02"))
                    {
                        ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR DEO A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";

                        //return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    if (ExisteFechamentoEstoque(dataVerifica, granja))
                    {
                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == granja)
                            .FirstOrDefault();

                        //string responsavel = "Miriene Gomes";
                        //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                        //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB") 
                        //    responsavel = "Sérica Doimo";
                        //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                        //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                        //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                        //                + " na empresa " + empresa.EmpNome
                        //                + "! Não pode ser inserido novo Diário de Expedição!"
                        //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                        string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                        ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                            + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                            + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    if (ExisteDEOSolicitacaoAjusteEstoqueAberto(granja))
                    {
                        ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                            + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    if (granja == "")
                    {
                        ViewBag.Erro = am.GetTextOnLanguage("Para inserir um DEO, selecione uma granja / incubatório!", Session["language"].ToString());
                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    if (granja.Equals("SB") || granja.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);
                    List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
                    Session["incubatorioDestinoSelecionado"] = items[0].Value;

                    if (Session["tipoDEOselecionado"].ToString() != "Transf. Ovos Incubáveis" && !isGranja)
                    {
                        Session["incubatorioDestinoSelecionado"] = granja;
                    }

                    Session["escondeAddItem"] = "Não";

                    if (localOvosComercio || isOCGranja)
                    {
                        Session["qtdeOvosComerciais"] = 0;
                    }

                    System.Data.Objects.ObjectParameter numero =
                        new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                    apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
                    Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);
                    string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                    //ViewBag.DataHoraExped = DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                    return View("Index",hlbapp.LayoutDiarioExpedicaos
                        //.Where(d => d.DataHoraCarreg == dataFiltro)
                        .Where(d => d.NumIdentificacao == numIdentificacao && d.Granja == granja)
                        .ToList());
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult EditDEO(DateTime dataFiltro, string nfNum, string tipoDEO, string gta, string lacre, string operacaoMetodo, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            CarregaListaNucleos();
            CarregaTipoOvo();
            Session["dataProducaoSelecionada"] = DateTime.Today;
            Session["loteCompletoSelecionado"] = "";
            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            bdApolo.CommandTimeout = 1000;
            apoloService.CommandTimeout = 1000;

            if (operacaoMetodo != null)
            {
                if (operacaoMetodo.Equals("Edit"))
                    Session["operacao"] = "Edit";
                else
                    Session["operacao"] = "Create";
            }
            else
                Session["operacao"] = "Create";

            string granja = Session["granjaSelecionada"].ToString();
            Session["dataHoraCarreg"] = dataFiltro;
            Session["dataDEO"] = dataFiltro.ToString("dd/MM/yyyy");
            Session["horaDEO"] = dataFiltro.ToString("HH:mm");
            Session["nfNum"] = nfNum;
            if (tipoDEO != null)
                Session["tipoDEOselecionado"] = tipoDEO;
            else
                if (Session["tipoDEOselecionado"] != null)
                    tipoDEO = Session["tipoDEOselecionado"].ToString();
            Session["GTA"] = gta;
            Session["Lacre"] = lacre;

            var lista = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
            if (lista.Count == 0)
                lista = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");

            if (tipoDEO.Equals("Transferência entre Linhagens"))
            {
                Session["TransferenciaLinhagens"] = true;
                Session["linhagemOrigemSelecionada"] = lista.FirstOrDefault().Linhagem;
                Session["linhagemDestinoSelecionada"] = lista.FirstOrDefault().GTANum;
                CarregaLinhagensOrigem(granja);
                AtualizaLinhagemOrigemSelecionada(Session["linhagemOrigemSelecionada"].ToString());
                //Session["ListaLinhagemDestino"] = new List<SelectListItem>();
                CarregaLinhagensDestino(lista.FirstOrDefault().Incubatorio, 
                    Session["linhagemOrigemSelecionada"].ToString());
                AtualizaLinhagemDestinoSelecionada(Session["linhagemDestinoSelecionada"].ToString());
                Session["tipoDEOselecionado"] = "Transferência entre Linhagens";
            }

            DateTime dataVerifica = Convert.ToDateTime(dataFiltro.ToShortDateString());

            if (ExisteFechamentoEstoque(dataVerifica, granja))
            {
                //DateTime dataInicial;
                //DateTime dataFinal;

                //if (Session["dataInicial"] == null)
                //{
                //    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                //    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                //    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                //    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                //}
                //else
                //{
                //    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                //    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                //}

                //string responsavel = "Miriene Gomes";
                //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                //    responsavel = "Sérica Doimo";
                //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                //                + "! Não pode ser alterado este Diário de Expedição!"
                //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                    + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                    + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                //return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(granja))
            {
                DateTime dataInicial;
                DateTime dataFinal;

                if (Session["dataInicial"] == null)
                {
                    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                }
                else
                {
                    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                }

                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            //Session["listaItensDadosAntigosImport"] = CarregarItensDEOImport(hlbapp, dataFiltro, granja);
            Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
            if (((List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"]).Count == 0)
            Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                "Cadastro");
            List<LayoutDiarioExpedicaos> listaItensDadosAntigos = 
                (List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"];

            LayoutDiarioExpedicaos diarioExpedicao = listaItensDadosAntigos.FirstOrDefault();

            Session["dataRecebInc"] = "";

            if (diarioExpedicao != null)
            {
                Session["Observacao"] = diarioExpedicao.Observacao;
                if (diarioExpedicao.DataHoraRecebInc.ToShortDateString() != "01/01/1899")
                    Session["dataRecebInc"] = diarioExpedicao.DataHoraRecebInc;
            }
            else
            {
                Session["Observacao"] = "";
                Session["dataRecebInc"] = "";
            }

            Session["ListaTiposDEO"] = CarregaListaTiposDEO(false);
            if (tipoDEO != null)
                Session["ListaTiposDEO"] = AtualizaTipoDEOSelecionado(tipoDEO,
                    (List<SelectListItem>)Session["ListaTiposDEO"]);

            Session["escondeLinkPrincipal"] = "Não";

            if (diarioExpedicao != null)
            {
                if (!diarioExpedicao.Importado.Equals("Conferido") || 
                    (diarioExpedicao.Importado.Equals("Conferido") && 
                        !diarioExpedicao.TipoDEO.Equals("Ovos Incubáveis"))
                    || listaItensDadosAntigos.Where(w => w.Importado == "Sim")
                        .Count() > 0)
                    Session["escondeAddItem"] = "Não";
                else
                    Session["escondeAddItem"] = "Sim";

                //if ((diarioExpedicao.Importado.Equals("Conferido")) && 
                //    diarioExpedicao.Incubatorio != diarioExpedicao.Granja)
                //    Session["escondeAddItem"] = "Sim";
                //else
                //    Session["escondeAddItem"] = "Não";
            }
            else
                Session["escondeAddItem"] = "Não";

            if (granja.Equals("SB") || granja.Equals("PH"))
                Session["location"] = "GP";
            else
                Session["location"] = "PP";

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);
            if ((Session["incubatorioDestinoSelecionado"] == null
                    || Session["incubatorioDestinoSelecionado"].ToString() == "")
                && lista.Count > 0)
            {
                if (lista.FirstOrDefault().Incubatorio != null && lista.FirstOrDefault().Incubatorio != "")
                {
                    AtualizaIncubatorioSelecionado(lista.FirstOrDefault().Incubatorio);
                    Session["incubatorioDestinoSelecionado"] = lista.FirstOrDefault().Incubatorio;
                }
                else
                {
                    List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
                    if (items.Count > 0)
                    {
                        Session["incubatorioDestinoSelecionado"] = items[0].Value;
                        AtualizaIncubatorioSelecionado(Session["incubatorioDestinoSelecionado"].ToString());
                    }
                }
            }
            else
            {
                AtualizaIncubatorioSelecionado(Session["incubatorioDestinoSelecionado"].ToString());
            }

            if (lista.Count > 0)
                Session["operacao"] = "Edit";

            bool localOvosComercio = false;
            if (granja.Length == 3) if (granja.Substring(2, 1) == "C") localOvosComercio = true;

            bool isOCGranja = false;
            Models.bdApolo.EMPRESA_FILIAL empresaGranja = bdApolo.EMPRESA_FILIAL
                .Where(w => w.USERFLIPCod == granja).FirstOrDefault();
            if (empresaGranja != null)
            {
                if (empresaGranja.USERTipoUnidadeFLIP == "Granja" && tipoDEO == "Ovos p/ Comércio") isOCGranja = true;
            }
            else
                if (tipoDEO == "Ovos p/ Comércio") isOCGranja = true;

            if (localOvosComercio || isOCGranja) Session["qtdeOvosComerciais"] = 
                lista.Sum(s => s.QtdeOvos);

            if ((MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                .GetGroup("HLBAPPM-GerarDiarioExpedicao", (System.Collections.ArrayList)Session["Direitos"]))
                && (Session["granjaSelecionada"].ToString().Equals("SB")))
            {
                List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
                Session["descricaoIncubatorioDestinoSelecionado"] = "Incubatório "
                    + items.Where(w => w.Value == "PH").FirstOrDefault().Text;

                foreach (var item in lista)
                {
                    Session["qtdOvos_"
                        + item.LoteCompleto.ToString() + "|"
                        + item.DataProducao.ToShortDateString()] = (int)item.QtdeOvos;
                }

                return View("DEOGerado", lista);
            }
            else
                return View("Index", lista);
        }

        public ActionResult ReturnEditDEO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    CarregaListaNucleos();
                    CarregaTipoOvo();
                    Session["dataProducaoSelecionada"] = DateTime.Today;
                    Session["loteCompletoSelecionado"] = "";

                    bdApolo.CommandTimeout = 1000;
                    apoloService.CommandTimeout = 1000;

                    string granja = Session["granjaSelecionada"].ToString();
                    DateTime dataFiltro = Convert.ToDateTime(Session["dataHoraCarreg"]);
                    string numIdentificacao = "Sem ID";
                    if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                    var lista = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (lista.Count == 0)
                        lista = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");

                    LayoutDiarioExpedicaos primeiro = lista.FirstOrDefault();
                    string tipoDEO = primeiro.TipoDEO;
                    string nfNum = primeiro.NFNum;
                    string gta = primeiro.GTANum;
                    string lacre = primeiro.Lacre;

                    Session["dataDEO"] = dataFiltro.ToString("dd/MM/yyyy");
                    Session["horaDEO"] = dataFiltro.ToString("HH:mm");
                    Session["nfNum"] = nfNum;
                    if (tipoDEO != null)
                        Session["tipoDEOselecionado"] = tipoDEO;
                    else
                        if (Session["tipoDEOselecionado"] != null)
                            tipoDEO = Session["tipoDEOselecionado"].ToString();
                    Session["GTA"] = gta;
                    Session["Lacre"] = lacre;

                    if (tipoDEO.Equals("Transferência entre Linhagens"))
                    {
                        Session["TransferenciaLinhagens"] = true;
                        Session["linhagemOrigemSelecionada"] = lista.FirstOrDefault().Linhagem;
                        Session["linhagemDestinoSelecionada"] = lista.FirstOrDefault().GTANum;
                        CarregaLinhagensOrigem(granja);
                        AtualizaLinhagemOrigemSelecionada(Session["linhagemOrigemSelecionada"].ToString());
                        //Session["ListaLinhagemDestino"] = new List<SelectListItem>();
                        CarregaLinhagensDestino(lista.FirstOrDefault().Incubatorio,
                            Session["linhagemOrigemSelecionada"].ToString());
                        AtualizaLinhagemDestinoSelecionada(Session["linhagemDestinoSelecionada"].ToString());
                        Session["tipoDEOselecionado"] = "Transferência entre Linhagens";
                    }

                    DateTime dataVerifica = Convert.ToDateTime(dataFiltro.ToShortDateString());

                    if (ExisteFechamentoEstoque(dataVerifica, granja))
                    {
                        //DateTime dataInicial;
                        //DateTime dataFinal;

                        //if (Session["dataInicial"] == null)
                        //{
                        //    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        //    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        //    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        //    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                        //}
                        //else
                        //{
                        //    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        //    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                        //}

                        //string responsavel = "Miriene Gomes";
                        //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                        //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                        //    responsavel = "Sérica Doimo";
                        //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                        //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                        //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                        //                + "! Não pode ser alterado este Diário de Expedição!"
                        //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                        string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                        ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                            + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                            + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                        //return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    //Session["listaItensDadosAntigosImport"] = CarregarItensDEOImport(hlbapp, dataFiltro, granja);
                    //Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataFiltro, granja, "",
                    //    "Crescente", "Cadastro");
                    Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (((List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"]).Count == 0)
                        Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                            "Cadastro");

                    List<LayoutDiarioExpedicaos> listaItensDadosAntigos =
                        (List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"];

                    LayoutDiarioExpedicaos diarioExpedicao = listaItensDadosAntigos.FirstOrDefault();

                    if (diarioExpedicao != null)
                    {
                        Session["Observacao"] = diarioExpedicao.Observacao;
                    }
                    else
                        Session["Observacao"] = "";

                    Session["ListaTiposDEO"] = CarregaListaTiposDEO(false);
                    if (tipoDEO != null)
                        Session["ListaTiposDEO"] = AtualizaTipoDEOSelecionado(tipoDEO,
                            (List<SelectListItem>)Session["ListaTiposDEO"]);

                    Session["escondeLinkPrincipal"] = "Não";

                    if (diarioExpedicao != null)
                    {
                        if (!diarioExpedicao.Importado.Equals("Conferido") ||
                            (diarioExpedicao.Importado.Equals("Conferido") &&
                                !diarioExpedicao.TipoDEO.Equals("Ovos Incubáveis")))
                            Session["escondeAddItem"] = "Não";
                        else
                            Session["escondeAddItem"] = "Sim";
                    }
                    else
                        Session["escondeAddItem"] = "Não";

                    if (granja.Equals("SB") || granja.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);
                    if ((Session["incubatorioDestinoSelecionado"] == null
                            || Session["incubatorioDestinoSelecionado"].ToString() == "")
                        && lista.Count > 0)
                    {
                        if (lista.FirstOrDefault().Incubatorio != null && lista.FirstOrDefault().Incubatorio != "")
                        {
                            AtualizaIncubatorioSelecionado(lista.FirstOrDefault().Incubatorio);
                            Session["incubatorioDestinoSelecionado"] = lista.FirstOrDefault().Incubatorio;
                        }
                        else
                        {
                            List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
                            if (items.Count > 0)
                            {
                                Session["incubatorioDestinoSelecionado"] = items[0].Value;
                                AtualizaIncubatorioSelecionado(Session["incubatorioDestinoSelecionado"].ToString());
                            }
                        }
                    }
                    else
                    {
                        AtualizaIncubatorioSelecionado(Session["incubatorioDestinoSelecionado"].ToString());
                    }

                    if (lista.Count > 0)
                        Session["operacao"] = "Edit";

                    bool localOvosComercio = false;
                    if (granja.Length == 3) if (granja.Substring(2, 1) == "C") localOvosComercio = true;

                    bool isOCGranja = false;
                    Models.bdApolo.EMPRESA_FILIAL empresaGranja = bdApolo.EMPRESA_FILIAL
                        .Where(w => w.USERFLIPCod == granja).FirstOrDefault();
                    if (empresaGranja != null)
                    {
                        if (empresaGranja.USERTipoUnidadeFLIP == "Granja" && tipoDEO == "Ovos p/ Comércio") isOCGranja = true;
                    }
                    else
                        if (tipoDEO == "Ovos p/ Comércio") isOCGranja = true;

                    if (localOvosComercio || isOCGranja) Session["qtdeOvosComerciais"] = lista.Sum(s => s.QtdeBandejas);

                    return View("Index", lista);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult SaveDEO(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    bdApolo.CommandTimeout = 1000;
                    apoloService.CommandTimeout = 1000;

                    #region Varíaveis Globais do Método

                    string granja = Session["granjaSelecionada"].ToString();
                    string incubatorio = Session["incubatorioDestinoSelecionado"].ToString();
                    string numIdentificacao = "Sem ID";
                    if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                    bool localOvosComercio = false;
                    if (granja.Length == 3) if (granja.Substring(2, 1) == "C") localOvosComercio = true;

                    if (Session["operacao"].ToString().Equals("Create"))
                    {
                        string dataDEO = model["dataDEO"].ToString();
                        string horaDEO = model["horaDEO"].ToString();

                        Session["dataDEO"] = Convert.ToDateTime(dataDEO).ToShortDateString();
                        Session["horaDEO"] = horaDEO.ToString();

                        //Session["dataHoraCarreg"] = (Session["dataDEO"].ToString().Substring(0,10) + " " + Session["horaDEO"].ToString().Substring(11,8));
                        Session["dataHoraCarreg"] = (dataDEO + " " + horaDEO);
                    }

                    DateTime dataFiltro = Convert.ToDateTime(Session["dataHoraCarreg"]);
                    string tipoDEO = Session["tipoDEOselecionado"].ToString();

                    DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                    DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");

                    DateTime dataVerifica = Convert.ToDateTime(dataFiltro.ToShortDateString());

                    if (ExisteFechamentoEstoque(dataVerifica, granja))
                    {
                        if (Session["dataInicial"] == null)
                        {
                            Session["dataInicial"] = DateTime.Today.ToShortDateString();
                            Session["dataFinal"] = DateTime.Today.ToShortDateString();
                            dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                        }
                        else
                        {
                            dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                        }

                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == granja)
                            .FirstOrDefault();

                        //string responsavel = "Miriene Gomes";
                        //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                        //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                        //    responsavel = "Sérica Doimo";
                        //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                        //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                        //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                        //    //+ " na empresa " + empresa.EmpNome  
                        //                + "! Não pode ser salvo este Diário de Expedição!"
                        //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                        string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                        ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                            + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                            + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                    }

                    var lista = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (lista.Count == 0)
                        lista = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");

                    bool isOCGranja = false;
                    Models.bdApolo.EMPRESA_FILIAL empresaGranja = bdApolo.EMPRESA_FILIAL
                        .Where(w => w.USERFLIPCod == granja).FirstOrDefault();
                    if (empresaGranja != null)
                    {
                        if (empresaGranja.USERTipoUnidadeFLIP == "Granja" && tipoDEO == "Ovos p/ Comércio") isOCGranja = true;
                    }
                    else
                        if (tipoDEO == "Ovos p/ Comércio") isOCGranja = true;

                    string login = Session["login"].ToString();

                    string nfAntiga = "";
                    if (lista.Count > 0)
                        nfAntiga = lista.FirstOrDefault().NFNum;

                    string assuntoEmail = "**** ERRO AO INTEGRAR DEO COM APOLO ****";
                    string paraNome = "T.I.";
                    string paraEmail = "ti@hyline.com.br";
                    string copiaPara = "";
                    string anexo = "";

                    string usuario = "";

                    if (login.Equals("palves"))
                        usuario = "RIOSOFT";
                    else
                        usuario = login.ToUpper();

                    int IDErro = 0;

                    string origemErro = "";

                    string observacao = model["Observacao"].ToString();
                    DateTime dataRecInc = Convert.ToDateTime("01/01/1988");
                    if (model["dataRecebInc"] != null)
                        if (model["dataRecebInc"] != "")
                            DateTime.TryParse(model["dataRecebInc"], out dataRecInc);

                    string loteErro = "";
                    DateTime dataProducaoErro = new DateTime();

                    //var listaImport = CarregarItensDEOImport(hlbapp, dataFiltro, granja);    

                    #endregion

                    try
                    {
                        #region Deleta e Insere na tabela ImportaDiarioExpedicao - DESATIVADO

                        #region Deleta ImportaDiarioExpedicao

                        //foreach (var item in listaImport)
                        //{
                        //    List<LayoutDEO_X_ImportaDEO> listDEOXIDEO = hlbapp.LayoutDEO_X_ImportaDEO
                        //        .Where(w => w.CodItemImportaDEO == item.CodItemImportaDEO).ToList();

                        //    foreach (var x in listDEOXIDEO)
                        //    {
                        //        hlbapp.LayoutDEO_X_ImportaDEO.DeleteObject(x);
                        //    }

                        //    hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                        //}
                        //hlbapp.SaveChanges();

                        #endregion

                        //var listaDEOpImport = lista
                        //    .GroupBy(g => new { g.LoteCompleto, g.DataProducao, g.TipoOvo })
                        //    .OrderBy(o => o.Key.LoteCompleto).ThenBy(t => t.Key.DataProducao)
                        //    .ToList();

                        //foreach (var item in listaDEOpImport)
                        //{
                        //    var listaDEOItens = lista.Where(w => w.LoteCompleto == item.Key.LoteCompleto
                        //        && w.DataProducao == item.Key.DataProducao
                        //        && w.TipoOvo == item.Key.TipoOvo).ToList();

                        //    #region Gera Código Relacionamento

                        //    ImportaDiarioExpedicao novoImporta = new ImportaDiarioExpedicao();

                        //    System.Data.Objects.ObjectParameter numeroIDEO =
                        //        new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                        //    apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numeroIDEO);

                        //    novoImporta.CodItemImportaDEO = Convert.ToInt32(numeroIDEO.Value);

                        //    #endregion

                        //    decimal qtdOvos = 0;

                        //    foreach (var itemLoteData in listaDEOItens)
                        //    {
                        //        qtdOvos = qtdOvos + itemLoteData.QtdeOvos;
                        //        itemLoteData.Incubatorio = incubatorio;

                        //        #region Ajusta Linhagem


                        //        FLOCKSMobileTableAdapter fTA = new FLOCKSMobileTableAdapter();
                        //        FLIPDataSetMobile.FLOCKSMobileDataTable fDT = new FLIPDataSetMobile.FLOCKSMobileDataTable();
                        //        fTA.FillByFlockID(fDT, itemLoteData.LoteCompleto);

                        //        if (fDT.Count > 0)
                        //        {
                        //            FLIPDataSetMobile.FLOCKSMobileRow fRow = fDT.FirstOrDefault();
                        //            if (itemLoteData.Linhagem != fRow.VARIETY)
                        //                itemLoteData.Linhagem = fRow.VARIETY;
                        //        }

                        //        #endregion

                        //        #region Gera Codigo Relacionamento no DEo caso não tenha

                        //        if (itemLoteData.CodItemDEO == null)
                        //        {
                        //            System.Data.Objects.ObjectParameter numero =
                        //                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                        //            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
                        //            itemLoteData.CodItemDEO = Convert.ToInt32(numero.Value);
                        //        }

                        //        #endregion

                        //        #region Insere Relacionamento

                        //        LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                        //        deoXimporta.CodItemDEO = Convert.ToInt32(itemLoteData.CodItemDEO);
                        //        deoXimporta.CodItemImportaDEO = (int)novoImporta.CodItemImportaDEO;

                        //        hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                        //        #endregion
                        //    }

                        //    #region Insere ImportaDiarioExpedicao

                        //    LayoutDiarioExpedicaos primeiro = listaDEOItens.FirstOrDefault();

                        //    novoImporta.Nucleo = primeiro.Nucleo;
                        //    novoImporta.Galpao = primeiro.Galpao;
                        //    novoImporta.Lacre = primeiro.Lote;
                        //    novoImporta.Idade = primeiro.Idade;
                        //    novoImporta.Linhagem = primeiro.Linhagem;
                        //    novoImporta.Lote = primeiro.Lote;
                        //    novoImporta.LoteCompleto = primeiro.LoteCompleto;
                        //    novoImporta.DataProducao = primeiro.DataProducao;
                        //    novoImporta.NumeroReferencia = primeiro.NumeroReferencia;
                        //    novoImporta.QtdeOvos = qtdOvos;
                        //    novoImporta.QtdeBandejas = novoImporta.QtdeOvos / 150;
                        //    novoImporta.Usuario = primeiro.Usuario;
                        //    novoImporta.DataHora = DateTime.Now;
                        //    novoImporta.DataHoraCarreg = primeiro.DataHoraCarreg;
                        //    novoImporta.DataHoraRecebInc = primeiro.DataHoraRecebInc;
                        //    novoImporta.ResponsavelCarreg = primeiro.ResponsavelCarreg;
                        //    novoImporta.ResponsavelReceb = primeiro.ResponsavelReceb;
                        //    novoImporta.NFNum = primeiro.NFNum;
                        //    novoImporta.Granja = primeiro.Granja;
                        //    novoImporta.Importado = primeiro.Importado;
                        //    novoImporta.Incubatorio = primeiro.Incubatorio;
                        //    novoImporta.TipoDEO = primeiro.TipoDEO;
                        //    novoImporta.GTANum = primeiro.GTANum;
                        //    novoImporta.Lacre = primeiro.Lacre;
                        //    novoImporta.NumIdentificacao = primeiro.NumIdentificacao;
                        //    novoImporta.TipoOvo = primeiro.TipoOvo;

                        //    hlbapp.ImportaDiarioExpedicao.AddObject(novoImporta);

                        //    #endregion
                        //}

                        //hlbapp.SaveChanges();
                        ////db.SaveChanges();
                        //listaImport = CarregarItensDEOImport(hlbapp, dataFiltro, granja);

                        #endregion

                        if (lista.Count == 0 && !localOvosComercio && !isOCGranja)
                        {
                            ViewBag.Erro = "É necessário pelo menos inserir um item!";
                            //return View("Index", lista);
                            Session["escondeLinkPrincipal"] = "Não";
                            return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                        }

                        //Session["ListaTiposDEOFiltro"] = CarregaListaTiposDEO(true);

                        #region (COMENTADO) Ajusta Lista DEO com a lista de Importação

                        //if ((listaImport.Count > 0) && (!tipoDEO.Equals("Inventário de Ovos")))
                        //{
                        //    foreach (var item in lista)
                        //    {
                        //        int existeDEO = hlbapp.ImportaDiarioExpedicao
                        //            .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(x => x.CodItemImportaDEO == d.CodItemImportaDEO
                        //                && x.CodItemDEO == item.CodItemDEO))
                        //            .Count();

                        //        if (existeDEO == 0)
                        //        {
                        //            db.DiarioExpedicao.Remove(item);
                        //        }
                        //    }

                        //    db.SaveChanges();
                        //}

                        #endregion

                        #region Variáveis e objetos locais no TRY

                        string nfNum = "";
                        if (model["nfNum"] != null) nfNum = model["nfNum"].ToString();
                        if (localOvosComercio || isOCGranja)
                        {
                            observacao = model["ObservacaoOC"].ToString();
                            if (model["nfNumOC"] != null) nfNum = model["nfNumOC"].ToString();
                        }
                        string gta = "";
                        if (model["GTA"] != null) gta = model["GTA"].ToString();
                        string lacre = "";
                        if (model["Lacre"] != null) lacre = model["Lacre"].ToString();
                        //string tipoDEO = model["Text"].ToString();
                        Session["tipoDEOselecionado"] = tipoDEO;

                        System.Data.Objects.ObjectParameter numeroNF =
                            new System.Data.Objects.ObjectParameter("numero", typeof(global::System.String));

                        if (nfNum != "")
                            nfNum = Convert.ToInt32(nfNum).ToString();

                        apoloService.CONCAT_ZERO_ESQUERDA(nfNum, 10, numeroNF);

                        nfNum = numeroNF.Value.ToString();
                        //string especie = "NF-e";
                        //string serie = "001";

                        /*ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == granja)
                            .FirstOrDefault();*/

                        MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == granja
                                || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod
                                    && c.EmpFilCertificNum == granja))
                            .FirstOrDefault();

                        int qtdeOvosComerciais = 0;
                        int qtdeCaixasComerciais = 0;
                        if (model["qtdeOvos"] != null)
                            if (int.TryParse(model["qtdeOvos"], out qtdeOvosComerciais))
                            {
                                if (model["tipoQtde"] == "1")
                                {
                                    qtdeOvosComerciais = Convert.ToInt32(model["qtdeOvos"]) * 360;
                                    qtdeCaixasComerciais = Convert.ToInt32(model["qtdeOvos"]);
                                }
                                else
                                {
                                    qtdeOvosComerciais = Convert.ToInt32(model["qtdeOvos"]);
                                    qtdeCaixasComerciais = Convert.ToInt32(
                                        Math.Ceiling(Convert.ToInt32(model["qtdeOvos"]) / 360.0m));
                                }
                            }

                        #endregion

                        #region DEO Normal

                        foreach (var item2 in lista)
                        {
                            if ((item2.TipoOvo == "" || item2.TipoOvo == null)
                                && tipoDEO == "Transf. Ovos Incubáveis"
                                && incubatorio == "NM" && granja == "PL")
                            {
                                ViewBag.Erro = "Não pode mudar o Tipo de DEO para " + tipoDEO
                                    + ", porque existem itens sem a classificação do ovo!";
                                return View("Index", lista);
                            }

                            if (!item2.Importado.Equals("Conferido"))
                            {
                                //if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                //    item2.Importado = "Conferido";
                                //else
                                //    item2.Importado = "Sim";

                                if (!tipoDEO.Equals("Ovos Incubáveis") && !tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                    item2.Importado = "Conferido";
                                else
                                    item2.Importado = "Sim";
                            }
                            item2.TipoDEO = tipoDEO;
                            item2.Incubatorio = incubatorio;
                            item2.NFNum = nfNum;
                            item2.GTANum = gta;
                            item2.Lacre = lacre;
                            item2.NumIdentificacao = numIdentificacao;
                            item2.Observacao = observacao;
                            if (dataRecInc != Convert.ToDateTime("1988-01-01 00:00:00.000"))
                                item2.DataHoraRecebInc = dataRecInc;

                            if (localOvosComercio || isOCGranja)
                            {
                                item2.QtdeOvos = qtdeOvosComerciais;
                                item2.QtdeBandejas = qtdeCaixasComerciais;
                            }

                            #region Gera LOG

                            origemErro = "Erro Gera LOG HLBAPP: ";

                            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                            hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now,
                                "Importação", usuario, 0, "", "", item2));
                            hlbappLOG.SaveChanges();

                            #endregion
                        }

                        #endregion

                        #region DEO com lista import - DESATIVADO

                        //foreach (var item in listaImport)
                        //{
                        //    #region Salva informações do DEO

                        //    origemErro = "Erro Salva DEO HLBAPP: ";

                        //    var listaOriginal = hlbapp.LayoutDiarioExpedicaos
                        //        .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                        //            && l.CodItemImportaDEO == item.CodItemImportaDEO))
                        //        .ToList();

                        //    foreach (var item2 in listaOriginal)
                        //    {
                        //        if ((item2.TipoOvo == "" || item2.TipoOvo == null)
                        //            && tipoDEO == "Transf. Ovos Incubáveis"
                        //            && incubatorio == "NM" && granja == "PL")
                        //        {
                        //            ViewBag.Erro = "Não pode mudar o Tipo de DEO para " + tipoDEO
                        //                + ", porque existem itens sem a classificação do ovo!";
                        //            return View("Index", lista);
                        //        }

                        //        if (!item2.Importado.Equals("Conferido"))
                        //        {
                        //            //if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                        //            //    item2.Importado = "Conferido";
                        //            //else
                        //            //    item2.Importado = "Sim";

                        //            if (!tipoDEO.Equals("Ovos Incubáveis"))
                        //                item2.Importado = "Conferido";
                        //            else
                        //                item2.Importado = "Sim";
                        //        }
                        //        item2.TipoDEO = tipoDEO;
                        //        item2.Incubatorio = incubatorio;
                        //        item2.NFNum = nfNum;
                        //        item2.GTANum = gta;
                        //        item2.Lacre = lacre;
                        //        item2.NumIdentificacao = "";
                        //        item2.Observacao = observacao;
                        //        if (dataRecInc != Convert.ToDateTime("1988-01-01 00:00:00.000")) 
                        //            item2.DataHoraRecebInc = dataRecInc;

                        //        if (localOvosComercio || isOCGranja)
                        //        {
                        //            item2.QtdeOvos = qtdeOvosComerciais;
                        //            item2.QtdeBandejas = qtdeCaixasComerciais;
                        //        }

                        //        #region Gera LOG

                        //        origemErro = "Erro Gera LOG HLBAPP: ";

                        //        HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                        //        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now,
                        //            "Importação", usuario, 0, "", "", item2));
                        //        hlbappLOG.SaveChanges();

                        //        #endregion
                        //    }

                        //    if (!item.Importado.Equals("Conferido"))
                        //    {
                        //        if (!tipoDEO.Equals("Ovos Incubáveis"))
                        //            item.Importado = "Conferido";
                        //        else
                        //            item.Importado = "Sim";
                        //    }
                        //    item.TipoDEO = tipoDEO;
                        //    item.Incubatorio = incubatorio;
                        //    item.NFNum = nfNum;
                        //    item.GTANum = gta;
                        //    item.Lacre = lacre;
                        //    item.NumIdentificacao = "";

                        //    #endregion
                        //}

                        #endregion

                        #region DEO Ovos de Comércio da Granja

                        if ((localOvosComercio || isOCGranja))
                        {
                            LayoutDiarioExpedicaos deo = new LayoutDiarioExpedicaos();

                            deo.Nucleo = "VARIOS";
                            deo.Galpao = null;
                            deo.Lote = null;
                            deo.Idade = 0;
                            deo.Linhagem = "VARIAS";
                            deo.LoteCompleto = "VARIOS";
                            deo.DataProducao = Convert.ToDateTime("01/01/1988");
                            deo.NumeroReferencia = "";
                            deo.QtdeOvos = qtdeOvosComerciais;
                            deo.QtdeBandejas = qtdeCaixasComerciais;
                            deo.Usuario = Session["login"].ToString();
                            deo.DataHora = DateTime.Now;
                            deo.DataHoraCarreg = dataFiltro;
                            deo.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                            deo.ResponsavelCarreg = null;
                            deo.ResponsavelReceb = null;
                            deo.NFNum = nfNum;
                            deo.Granja = granja;
                            deo.Importado = "Conferido";
                            if (isOCGranja)
                                deo.Incubatorio = incubatorio;
                            else
                                //deo.Incubatorio = incubatorio + "C";
                                deo.Incubatorio = granja;
                            deo.TipoDEO = tipoDEO;
                            deo.GTANum = "";
                            deo.Lacre = "";
                            deo.NumIdentificacao = numIdentificacao;

                            System.Data.Objects.ObjectParameter numeroNovo =
                                    new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numeroNovo);
                            deo.CodItemDEO = Convert.ToInt32(numeroNovo.Value);

                            deo.Observacao = observacao;
                            deo.TipoOvo = "";
                            deo.QtdDiferenca = null;

                            hlbapp.LayoutDiarioExpedicaos.AddObject(deo);

                            #region Gera LOG

                            origemErro = "Erro Gera LOG HLBAPP: ";

                            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                            hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now,
                                "Ovos p/ Comércio", usuario, 0, "", "", deo));
                            hlbappLOG.SaveChanges();

                            #endregion
                        }

                        #endregion

                        //db.SaveChanges();
                        hlbapp.SaveChanges();

                        Session["escondeLinkPrincipal"] = "Não";
                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                        #region **** DESATIVADO, POIS NÃO GERA ESTOQUE APOLO ****

                        /*
                        // Se for a Granja que estiver fazendo, fará a inclusão dos lotes na NF e integração de Tranferência.
                        if (empresa != null)
                        {
                            if (empresa.USERTipoUnidadeFLIP.Equals("Granja") || 
                                (granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                            {
                                bdErro = "Apolo";

                                #region DEO feito pela Granja ou Planalto (Entrada de Ovos)

                                #region Carrega Locais de Armazenagem

                                nucleos.FillFarmsAllLocation(flip.FLOCKS1);

                                //string location = flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;

                                //string localContra = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().LocArmazCodEstr;

                                //string incubatorio = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().USERCodigoFLIP;
                                string location = "";
                                string empresaEstoque = "";
                                if (granja.Equals("SB"))
                                {
                                    //incubatorio = "PH";
                                    location = "GP";
                                    empresaEstoque = "CH";
                                }
                                else
                                {
                                    if (granja.Equals("PL"))
                                    {
                                        //incubatorio = "NM";
                                        empresaEstoque = "PL";
                                    }
                                    else
                                    {
                                        //incubatorio = "CH";
                                        empresaEstoque = "CH";
                                    }
                                    location = "PP";
                                }

                                incubatorio = Session["incubatorioDestinoSelecionado"].ToString();

                                #endregion

                                origemErro = "Erro HLBAPP: ";

                                if (listaImport[0].Importado.Equals("Conferido"))
                                {
                                    foreach (var item in lista)
                                    {
                                        item.NFNum = nfNum;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                        item.Observacao = observacao;

                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, "Alteração Dados Capa", usuario, 0, item));
                                        //hlbapp.SaveChanges();
                                    }

                                    foreach (var item in listaImport)
                                    {
                                        item.NFNum = nfNum;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                    }

                                    db.SaveChanges();
                                    hlbapp.SaveChanges();

                                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                                }

                                #region DEO Ovos p/ Comércio

                                else if ((tipoDEO.Equals("Ovos p/ Comércio")) ||
                                    (tipoDEO.Equals("Inventário de Ovos")))
                                {
                                    foreach (var item in lista)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["usuario"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                        item.Observacao = observacao;

                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, tipoDEO, usuario, 0, item));
                                        //hlbapp.SaveChanges();
                                    }

                                    foreach (var item in listaImport)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["usuario"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                    }

                                    db.SaveChanges();
                                    hlbapp.SaveChanges();

                                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                                }

                                #endregion

                                #region DEO Ovos Incubáveis - Sem NF vinculada. Igual ao de Terceiro, gerando Transferência.

                                else if (tipoDEO.Equals("Ovos Incubáveis"))
                                {
                                    #region **** NÃO GERA MAIS APOLO ****

                                    /*
                                    ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                        .Where(e1 => e1.USERFLIPCodigo == granja)
                                        .FirstOrDefault();

                                    empresa = bdApolo.EMPRESA_FILIAL.Where(e => e.USERFLIPCod == empresaEstoque)
                                        .FirstOrDefault();

                                    #region Carrega variáveis e objetos

                                    string linhagemAnterior = "";
                                    MOV_ESTQ movEstq = new MOV_ESTQ();
                                    ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                                    LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                                    CTRL_LOTE_ITEM_MOV_ESTQ lote = new CTRL_LOTE_ITEM_MOV_ESTQ();

                                    TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();
                                    ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();
                                    decimal? qtdTotalItem = 0;

                                    DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                                    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                            .Where(l => l.USERCodigoFLIP == incubatorio && l.USERTipoProduto == tipoDEO)
                                            .FirstOrDefault();

                                    LOC_ARMAZ localArmazSaida = apoloService.LOC_ARMAZ
                                            .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                                            .FirstOrDefault();

                                    string tipoLanc = localArmazSaida.USERTipoLancSaidaInc;
                                    string unidadeMedida = "UN";

                                    DateTime dataMov = dataFiltro;

                                    #endregion

                                    #region Verifica se já tem a Transferência. Caso tenha, será deletada.

                                    ImportaDiarioExpedicao itemDeo = listaImport.First();

                                    int nfNumTransf;
                                    if (itemDeo.NumIdentificacao.Equals(""))
                                        nfNumTransf = 0;
                                    else
                                        nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                                    transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                                        .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                                        .FirstOrDefault();

                                    if (transfEstqLocArmaz != null)
                                    {
                                        //MOV_ESTQ saidaTransferencia = apoloService.MOV_ESTQ
                                        //    .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                        //        && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                        //    .FirstOrDefault();

                                        //if (saidaTransferencia != null)
                                        //{
                                        //    ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                                        //    apoloService.delete_movestq(saidaTransferencia.EmpCod, saidaTransferencia.MovEstqChv, usuario,
                                        //        rmensagem);
                                        //}

                                        origemErro = "Erro Exclusão APOLO: ";

                                        var listaMovEstq = apoloService.MOV_ESTQ
                                        .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                            && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                        .ToList();

                                        foreach (var item in listaMovEstq)
                                        {
                                            DeletaMovEstq(item);
                                        }

                                        apoloService.SaveChanges();

                                        apoloService.delete_transfestqlocarmaz(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                            usuario);
                                    }

                                    #endregion

                                    #region Insere Nova Transferência

                                    transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                                    numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                                    apolo.GerarCodigo("1", "TRANSF_ESTQ_LOC_ARMAZ", numero);

                                    transfEstqLocArmaz.EmpCod = empresa.EmpCod;
                                    transfEstqLocArmaz.TipoLancCod = tipoLanc;
                                    transfEstqLocArmaz.TransfEstqLocArmazData = Convert.ToDateTime(dataMov.ToShortDateString());
                                    transfEstqLocArmaz.TransfEstqLocArmazNum = Convert.ToInt32(numero.Value);
                                    transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis da Granja p/ Incubatório.";

                                    apoloService.TRANSF_ESTQ_LOC_ARMAZ.AddObject(transfEstqLocArmaz);

                                    apoloService.SaveChanges();

                                    #endregion

                                    short ultimaSequencia = 0;

                                    foreach (var item in listaImport)
                                    {
                                        IDErro = item.ID;

                                        if (IDErro == 92511)
                                        {
                                            IDErro = item.ID;
                                        }

                                        string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                        //item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                        #region Se mudou a linhagem da lista, insere um note Item

                                        if (linhagemAnterior != item.Linhagem)
                                        {
                                            ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem)
                                                .FirstOrDefault();

                                            //short ultimaSequencia = 0;

                                            if (!linhagemAnterior.Equals(""))
                                            {
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                                qtdTotalItem = 0;

                                                ultimaSequencia++;

                                                origemErro = "Erro Salva Item TLA Apolo: ";

                                                apoloService.SaveChanges();
                                            }
                                            else
                                            {
                                                ultimaSequencia = 1;
                                            }

                                            itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                                            itemTransfEstqLocArmaz.EmpCod = transfEstqLocArmaz.EmpCod;
                                            itemTransfEstqLocArmaz.TransfEstqLocArmazNum = transfEstqLocArmaz.TransfEstqLocArmazNum;
                                            itemTransfEstqLocArmaz.ProdCodEstr = produto.ProdCodEstr;
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq = ultimaSequencia;
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida = localArmazSaida.LocArmazCodEstr;
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada = localArmazCadastro.LocArmazCodEstr;
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazObs = transfEstqLocArmaz.TransfEstqLocArmazObs;

                                            apoloService.ITEM_TRANSF_ESTQ_LOC_ARMAZ.AddObject(itemTransfEstqLocArmaz);
                                            apoloService.SaveChanges();
                                        }

                                        #endregion

                                        #region Insere o Lote

                                        PROD_UNID_MED prodUnidMed = apoloService.PROD_UNID_MED
                                            .Where(p => p.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                                && p.ProdUnidMedCod == unidadeMedida)
                                            .FirstOrDefault();

                                        //int existeLote = apoloService.CTRL_LOTE
                                        //    .Where(c => c.EmpCod == itemTransfEstqLocArmaz.EmpCod 
                                        //        && c.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr 
                                        //        && c.CtrlLoteNum == item.LoteCompleto 
                                        //        && c.CtrlLoteDataValid == item.DataProducao)
                                        //    .Count();

                                        //if (existeLote == 0)
                                        //{
                                        //    ImportaIncubacao.Data.Apolo.CTRL_LOTE ctrlLote = new ImportaIncubacao.Data.Apolo.CTRL_LOTE();

                                        //    ctrlLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                        //    ctrlLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                        //    ctrlLote.CtrlLoteNum = item.LoteCompleto;
                                        //    ctrlLote.CtrlLoteDataValid = item.DataProducao;
                                        //    ctrlLote.CtrlLoteDataFab = item.DataProducao;
                                        //    ctrlLote.CtrlLoteQtdSaldo = 0;
                                        //    ctrlLote.CtrlLoteUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                        //    ctrlLote.CtrlLoteUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                        //    ctrlLote.CtrlLoteQtdSaldoCalc = ctrlLote.CtrlLoteQtdSaldo;

                                        //    apoloService.CTRL_LOTE.AddObject(ctrlLote);

                                        //    apoloService.SaveChanges();
                                        //}

                                        //int existeLote = 0;
                                        //existeLote = apoloService.CTRL_LOTE_LOC_ARMAZ
                                        //        .Where(c => c.EmpCod == itemTransfEstqLocArmaz.EmpCod
                                        //            && c.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                        //            && c.CtrlLoteNum == item.LoteCompleto
                                        //            && c.CtrlLoteDataValid == item.DataProducao
                                        //            && c.LocArmazCodEstr == itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida)
                                        //        .Count();

                                        //if (existeLote == 0)
                                        //{
                                        //    ImportaIncubacao.Data.Apolo.CTRL_LOTE_LOC_ARMAZ ctrlLoteLocArmaz = new CTRL_LOTE_LOC_ARMAZ();

                                        //    ctrlLoteLocArmaz.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                        //    ctrlLoteLocArmaz.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                        //    ctrlLoteLocArmaz.CtrlLoteNum = item.LoteCompleto;
                                        //    ctrlLoteLocArmaz.CtrlLoteDataValid = item.DataProducao;
                                        //    ctrlLoteLocArmaz.CtrlLoteLocArmazDataFab = item.DataProducao;
                                        //    ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldo = 0;
                                        //    ctrlLoteLocArmaz.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                        //    ctrlLoteLocArmaz.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                        //    ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldoCalc = ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldo;
                                        //    ctrlLoteLocArmaz.LocArmazCodEstr = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;

                                        //    apoloService.CTRL_LOTE_LOC_ARMAZ.AddObject(ctrlLoteLocArmaz);

                                        //    apoloService.SaveChanges();
                                        //}

                                        IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE itemTransfEstqLocArmazLote = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

                                        itemTransfEstqLocArmazLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                        itemTransfEstqLocArmazLote.TransfEstqLocArmazNum = itemTransfEstqLocArmaz.TransfEstqLocArmazNum;
                                        itemTransfEstqLocArmazLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSeq = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq;
                                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSaida = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida;
                                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazEntrada = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
                                        itemTransfEstqLocArmazLote.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                        itemTransfEstqLocArmazLote.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = item.QtdeOvos;
                                        itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                                        itemTransfEstqLocArmazLote.CtrlLoteNum = item.LoteCompleto;
                                        itemTransfEstqLocArmazLote.CtrlLoteDataValid = item.DataProducao;

                                        apoloService.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(itemTransfEstqLocArmazLote);
                                        apoloService.SaveChanges();

                                        #endregion

                                        qtdTotalItem = qtdTotalItem + item.QtdeOvos;

                                        #region Caso seja o último lote da linhagem, adiciona o total no item e salva

                                        if (listaImport.IndexOf(item) == (listaImport.Count - 1))
                                        {
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                            qtdTotalItem = 0;

                                            origemErro = "Erro Salva Item TLA APOLO: ";

                                            apoloService.SaveChanges();
                                        }

                                        #endregion

                                        #region Salva informações do DEO

                                        //var listaOriginal = db.DiarioExpedicao
                                        //    .Where(d => d.LoteCompleto == item.LoteCompleto && d.DataProducao == item.DataProducao
                                        //        && d.DataHoraCarreg == item.DataHoraCarreg && d.Granja == item.Granja)
                                        //    .ToList();

                                        origemErro = "Erro Salva DEO HLBAPP: ";

                                        var listaOriginal = hlbapp.LayoutDiarioExpedicaos
                                            .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                                                && l.CodItemImportaDEO == item.CodItemImportaDEO))
                                            .ToList();

                                        foreach (var item2 in listaOriginal)
                                        {
                                            item2.Importado = "Sim";
                                            item2.TipoDEO = tipoDEO;
                                            item2.Incubatorio = incubatorio;
                                            item2.NFNum = nfNum;
                                            item2.GTANum = gta;
                                            item2.Lacre = lacre;
                                            item2.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();
                                            item2.Observacao = observacao;

                                            #region Gera LOG

                                            origemErro = "Erro Gera LOG HLBAPP: ";

                                            //LayoutDiarioExpedicao diario = db.DiarioExpedicao.Where(d => d.ID == item2.ID).FirstOrDefault();

                                            //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOGs(DateTime.Now, "Importação", usuario, item2.QtdeOvos, item2));
                                            //hlbapp.SaveChanges();

                                            #endregion
                                        }

                                        item.Importado = "Sim";
                                        item.TipoDEO = tipoDEO;
                                        item.Incubatorio = incubatorio;
                                        item.NFNum = nfNum;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                        item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                        #endregion

                                        #region Importa FLIP

                                        List<ImportaDiarioExpedicao> listaItensDadosAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];

                                        var listaImportaFLIP = listaItensDadosAntigosImport
                                                .Where(a => a.DataHoraCarreg == item.DataHoraCarreg
                                                    && a.DataProducao == item.DataProducao
                                                    && a.Granja == item.Granja
                                                    && a.LoteCompleto == item.LoteCompleto)
                                                .ToList();

                                        if (listaImportaFLIP.Count > 0)
                                        {
                                            foreach (var itemAntigo in listaImportaFLIP)
                                            {
                                                TransferEggsFLIPImport(itemAntigo, localArmazSaida.USERGeracaoFLIP, usuario, "DEL", 
                                                    incubatorio);
                                            }
                                        }

                                        TransferEggsFLIPImport(item, localArmazSaida.USERGeracaoFLIP, usuario, "INS", incubatorio);

                                        #endregion

                                        linhagemAnterior = item.Linhagem;
                                        dataAnterior = item.DataHoraCarreg;
                                    }

                                    db.SaveChanges();
                                    apoloService.SaveChanges();
                                    hlbapp.SaveChanges();

                                    //#region Integra a transferência com o Estoque

                                    //apoloService.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                    //    usuario);

                                    //#endregion

                                    #endregion

                                    foreach (var item in listaImport)
                                    {
                                        #region Salva informações do DEO

                                        origemErro = "Erro Salva DEO HLBAPP: ";

                                        var listaOriginal = hlbapp.LayoutDiarioExpedicaos
                                            .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                                                && l.CodItemImportaDEO == item.CodItemImportaDEO))
                                            .ToList();

                                        foreach (var item2 in listaOriginal)
                                        {
                                            item2.Importado = "Sim";
                                            item2.TipoDEO = tipoDEO;
                                            item2.Incubatorio = incubatorio;
                                            item2.NFNum = nfNum;
                                            item2.GTANum = gta;
                                            item2.Lacre = lacre;
                                            item2.NumIdentificacao = "";
                                            item2.Observacao = observacao;

                                            #region Gera LOG

                                            origemErro = "Erro Gera LOG HLBAPP: ";

                                            #endregion
                                        }

                                        item.Importado = "Sim";
                                        item.TipoDEO = tipoDEO;
                                        item.Incubatorio = incubatorio;
                                        item.NFNum = nfNum;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                        item.NumIdentificacao = "";

                                        #endregion
                                    }

                                    db.SaveChanges();
                                    hlbapp.SaveChanges();

                                    Session["escondeLinkPrincipal"] = "Não";
                                }

                                #endregion

                                Session["escondeLinkPrincipal"] = "Não";
                                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                                #endregion
                            }
                            else
                            {
                                #region DEO feito pelo Incubatório - Gera Transferência

                                if (tipoDEO.Equals("Inventário de Ovos"))
                                {
                                    #region Inventario de Ovos

                                    foreach (var item in lista)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["usuario"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                        item.Observacao = observacao;

                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, tipoDEO, usuario, 0, item));
                                        //hlbapp.SaveChanges();
                                    }

                                    foreach (var item in listaImport)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["usuario"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = gta;
                                        item.Lacre = lacre;
                                    }

                                    db.SaveChanges();
                                    hlbapp.SaveChanges();

                                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                                    #endregion
                                }
                                else if (tipoDEO.Equals("Transferência entre Linhagens"))
                                {
                                    #region Transferência entre Linhagens

                                    #region Deleta Apolo

                                    ImportaDiarioExpedicao verificaImporta = listaImport.FirstOrDefault();

                                    string empCod = "1";
                                    System.Data.Objects.ObjectParameter numeroME =
                                        new System.Data.Objects.ObjectParameter("rmensagem", typeof(global::System.String));
                                    int chaveEntrada = 0;
                                    if (verificaImporta.ResponsavelReceb != "")
                                    {
                                        chaveEntrada = Convert.ToInt32(verificaImporta.ResponsavelReceb);
                                        apoloService.delete_movestq(empCod, chaveEntrada, usuario, numeroME);
                                    }
                                    int chaveSaida = 0;
                                    if (verificaImporta.ResponsavelCarreg != "")
                                    {
                                        chaveSaida = Convert.ToInt32(verificaImporta.ResponsavelCarreg);
                                        apoloService.delete_movestq(empCod, chaveSaida, usuario, numeroME);
                                    }
                                        
                                    #endregion

                                    string retornoTL = GeraMovimentacoesTransferenciaDeLinhagens(granja, dataFiltro,
                                        tipoDEO);

                                    #region Tratamento de Erro

                                    if (retornoTL != "")
                                    {
                                        ViewBag.Erro = retornoTL;

                                        retornoTL = retornoTL + (char)10 + (char)13 + "Usuário: " + usuario + (char)10 + (char)13
                                                                + "Granja: " + granja + (char)10 + (char)13
                                                                + "Data do DEO: " + dataFiltro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                                        EnviarEmail(retornoTL, assuntoEmail, paraNome, paraEmail, copiaPara, anexo);

                                        var lista2 = CarregarItensDEO(dataFiltro, granja);

                                        return View("Index", lista2);
                                    }

                                    #endregion

                                    List<ImportaDiarioExpedicao> listaItensDadosAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];
                                    if (Session["operacao"].ToString().Equals("Edit"))
                                        retornoTL = DeletaTransferenciaDeLinhagensFLIP(listaItensDadosAntigosImport);

                                    #region Tratamento de Erro

                                    if (retornoTL != "")
                                    {
                                        ViewBag.Erro = retornoTL;

                                        retornoTL = retornoTL + (char)10 + (char)13 + "Usuário: " + usuario + (char)10 + (char)13
                                                                + "Granja: " + granja + (char)10 + (char)13
                                                                + "Data do DEO: " + dataFiltro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                                        EnviarEmail(retornoTL, assuntoEmail, paraNome, paraEmail, copiaPara, anexo);

                                        var lista2 = CarregarItensDEO(dataFiltro, granja);

                                        return View("Index", lista2);
                                    }

                                    #endregion

                                    retornoTL = GeraTransferenciaDeLinhagensFLIP(granja, dataFiltro, tipoDEO);

                                    #region Tratamento de Erro

                                    if (retornoTL != "")
                                    {
                                        ViewBag.Erro = retornoTL;

                                        retornoTL = retornoTL + (char)10 + (char)13 + "Usuário: " + usuario + (char)10 + (char)13
                                                                + "Granja: " + granja + (char)10 + (char)13
                                                                + "Data do DEO: " + dataFiltro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                                        EnviarEmail(retornoTL, assuntoEmail, paraNome, paraEmail, copiaPara, anexo);

                                        var lista2 = CarregarItensDEO(dataFiltro, granja);

                                        return View("Index", lista2);
                                    }

                                    #endregion

                                    foreach (var item in lista)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["chaveMovEstqSaida"].ToString();
                                        item.ResponsavelReceb = Session["chaveMovEstqEntrada"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = Session["linhagemDestinoSelecionada"].ToString();
                                        item.Lacre = lacre;
                                        item.Observacao = observacao;

                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, tipoDEO, usuario, 0, item));
                                        //hlbapp.SaveChanges();
                                    }

                                    foreach (var item in listaImport)
                                    {
                                        item.NFNum = nfNum;
                                        item.ResponsavelCarreg = Session["chaveMovEstqSaida"].ToString();
                                        item.ResponsavelReceb = Session["chaveMovEstqEntrada"].ToString();
                                        item.Importado = "Sim";
                                        item.Incubatorio = incubatorio;
                                        item.TipoDEO = tipoDEO;
                                        item.GTANum = Session["linhagemDestinoSelecionada"].ToString();
                                        item.Lacre = lacre;
                                    }

                                    db.SaveChanges();
                                    hlbapp.SaveChanges();

                                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                                    #endregion
                                }
                                else
                                {
                                    #region Transferência de Ovos Incubáveis e Classificação de Ovos

                                    var listaClassOvo = listaImport
                                        .GroupBy(g => g.TipoOvo)
                                        .OrderBy(o => o.Key)
                                        .ToList();

                                    foreach (var tipoOvo in listaClassOvo)
                                    {
                                        #region Carrega variáveis e objetos

                                        var listaImportTipoOvo = listaImport
                                            .Where(w => w.TipoOvo == tipoOvo.Key)
                                            .OrderBy(o => o.Linhagem).ThenBy(o => o.LoteCompleto)
                                            .ThenBy(t => t.DataProducao)
                                            .ToList();

                                        string linhagemAnterior = "";
                                        MOV_ESTQ movEstq = new MOV_ESTQ();
                                        ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                                        LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                                        CTRL_LOTE_ITEM_MOV_ESTQ lote = new CTRL_LOTE_ITEM_MOV_ESTQ();

                                        TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();
                                        ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();
                                        decimal? qtdTotalItem = 0;

                                        DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                                        LOC_ARMAZ localArmazCadastro;

                                        string incubLocArmaz = "";
                                        string incubSaida = "";
                                        if (granja == "PL")
                                        {
                                            incubSaida = "NM";
                                            if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                                incubLocArmaz = tipoOvo.Key;
                                            else
                                                incubLocArmaz = incubSaida;

                                            if (lista.Where(w => w.Linhagem.Contains("DKB")).Count() == 0)
                                                empresa = bdApolo.EMPRESA_FILIAL.Where(w => w.USERFLIPCod == "CH")
                                                    .FirstOrDefault();
                                        }
                                        else
                                        {
                                            incubSaida = granja;
                                            //if (tipoDEO.Equals("Transf. Ovos Incubáveis"))  
                                            incubLocArmaz = incubatorio;
                                        }

                                        if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                            localArmazCadastro = apoloService.LOC_ARMAZ
                                                    .Where(l => l.USERCodigoFLIP == incubLocArmaz && l.USERTipoProduto == "Ovos Incubáveis")
                                                    .FirstOrDefault();
                                        else
                                            localArmazCadastro = apoloService.LOC_ARMAZ
                                                    .Where(l => l.USERCodigoFLIP == incubLocArmaz && l.USERTipoProduto == tipoDEO)
                                                    .FirstOrDefault();

                                        LOC_ARMAZ localArmazSaida = apoloService.LOC_ARMAZ
                                                .Where(l => l.USERCodigoFLIP == incubSaida && l.USERTipoProduto == "Ovos Incubáveis")
                                                .FirstOrDefault();

                                        nucleos.FillFarmsAllLocation(flip.FLOCKS1);

                                        string tipoLanc = "";
                                        if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                            if (granja == "PL")
                                                tipoLanc = "E0000553";
                                            else
                                                tipoLanc = "E0000508";
                                        else
                                            //tipoLanc = localArmazSaida.USERTipoLancSaidaCom;
                                            tipoLanc = localArmazCadastro.USERTipoLancSaidaCom;

                                        //if (tipoDEO.Equals("Exportação"))
                                        //tipoLanc = localArmazCadastro.USERTipoLancSaidaCom;

                                        string unidadeMedida = "UN";

                                        DateTime dataMov = dataFiltro;

                                        #endregion

                                        #region Verifica se já tem a Transferência. Caso tenha, será deletada.

                                        ImportaDiarioExpedicao itemDeo = listaImportTipoOvo.First();

                                        int nfNumTransf;
                                        if (itemDeo.NumIdentificacao.Equals(""))
                                            nfNumTransf = 0;
                                        else
                                            nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                                        transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                                            .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                                            .FirstOrDefault();

                                        if (transfEstqLocArmaz != null)
                                        {
                                            //MOV_ESTQ saidaTransferencia = apoloService.MOV_ESTQ
                                            //    .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                            //        && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                            //    .FirstOrDefault();

                                            //if (saidaTransferencia != null)
                                            //{
                                            //    ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                                            //    apoloService.delete_movestq(saidaTransferencia.EmpCod, saidaTransferencia.MovEstqChv, usuario,
                                            //        rmensagem);
                                            //}

                                            var listaMovEstq = apoloService.MOV_ESTQ
                                                .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                                    && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                                .ToList();

                                            foreach (var item in listaMovEstq)
                                            {
                                                DeletaMovEstq(item);
                                            }

                                            apoloService.SaveChanges();

                                            apoloService.delete_transfestqlocarmaz(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                                usuario);
                                        }

                                        #endregion

                                        #region Insere Nova Transferência

                                        transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                                        numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                                        apolo.GerarCodigo("1", "TRANSF_ESTQ_LOC_ARMAZ", numero);

                                        transfEstqLocArmaz.EmpCod = empresa.EmpCod;
                                        transfEstqLocArmaz.TipoLancCod = tipoLanc;
                                        transfEstqLocArmaz.TransfEstqLocArmazData = Convert.ToDateTime(dataMov.ToShortDateString());
                                        transfEstqLocArmaz.TransfEstqLocArmazNum = Convert.ToInt32(numero.Value);
                                        if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                            if (granja.Equals("PL"))
                                                transfEstqLocArmaz.TransfEstqLocArmazObs = "Classificação de Ovos.";
                                            else
                                                transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis do Incubatório de Nova Granada p/ Incubatório de Ajapi.";
                                        else if (tipoDEO.Equals("Exportação"))
                                            transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis para Exportação.";
                                        else if (tipoDEO.Equals("Venda de Ovos"))
                                            transfEstqLocArmaz.TransfEstqLocArmazObs = "Venda de Ovos Férteis";
                                        else if (tipoDEO.Equals("Doação"))
                                            transfEstqLocArmaz.TransfEstqLocArmazObs = "Doação de Ovos Férteis";
                                        else
                                            transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis p/ Comercial";

                                        apoloService.TRANSF_ESTQ_LOC_ARMAZ.AddObject(transfEstqLocArmaz);
                                        apoloService.SaveChanges();

                                        #endregion

                                        short ultimaSequencia = 0;

                                        foreach (var item in listaImportTipoOvo)
                                        {
                                            string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                            #region Se mudou a linhagem da lista, insere um note Item

                                            if (linhagemAnterior != item.Linhagem)
                                            {
                                                ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem)
                                                    .FirstOrDefault();

                                                if (!linhagemAnterior.Equals(""))
                                                {
                                                    itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                                    qtdTotalItem = 0;

                                                    ultimaSequencia++;

                                                    apoloService.SaveChanges();
                                                }
                                                else
                                                {
                                                    ultimaSequencia = 1;
                                                }

                                                itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                                                itemTransfEstqLocArmaz.EmpCod = transfEstqLocArmaz.EmpCod;
                                                itemTransfEstqLocArmaz.TransfEstqLocArmazNum = transfEstqLocArmaz.TransfEstqLocArmazNum;
                                                itemTransfEstqLocArmaz.ProdCodEstr = produto.ProdCodEstr;
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq = ultimaSequencia;
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida = localArmazSaida.LocArmazCodEstr;
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada = localArmazCadastro.LocArmazCodEstr;
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazObs = transfEstqLocArmaz.TransfEstqLocArmazObs;

                                                apoloService.ITEM_TRANSF_ESTQ_LOC_ARMAZ.AddObject(itemTransfEstqLocArmaz);
                                                apoloService.SaveChanges();
                                            }

                                            #endregion

                                            #region Insere o Lote

                                            PROD_UNID_MED prodUnidMed = apoloService.PROD_UNID_MED
                                                .Where(p => p.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                                    && p.ProdUnidMedCod == unidadeMedida)
                                                .FirstOrDefault();

                                            #region Existe Lote na Tabela de Saldo

                                            //int existeLote = apoloService.CTRL_LOTE_LOC_ARMAZ
                                            //        .Where(c => c.EmpCod == itemTransfEstqLocArmaz.EmpCod
                                            //            && c.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                            //            && c.CtrlLoteNum == item.LoteCompleto
                                            //            && c.CtrlLoteDataValid == item.DataProducao
                                            //            && c.LocArmazCodEstr == itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada)
                                            //        .Count();

                                            //if (existeLote == 0)
                                            //{
                                            //    ImportaIncubacao.Data.Apolo.CTRL_LOTE_LOC_ARMAZ ctrlLoteLocArmaz = new CTRL_LOTE_LOC_ARMAZ();

                                            //    ctrlLoteLocArmaz.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                            //    ctrlLoteLocArmaz.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                            //    ctrlLoteLocArmaz.CtrlLoteNum = item.LoteCompleto;
                                            //    ctrlLoteLocArmaz.CtrlLoteDataValid = item.DataProducao;
                                            //    ctrlLoteLocArmaz.CtrlLoteLocArmazDataFab = item.DataProducao;
                                            //    ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldo = 0;
                                            //    ctrlLoteLocArmaz.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                            //    ctrlLoteLocArmaz.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                            //    ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldoCalc = ctrlLoteLocArmaz.CtrlLoteLocArmazQtdSaldo;
                                            //    ctrlLoteLocArmaz.LocArmazCodEstr = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;

                                            //    apoloService.CTRL_LOTE_LOC_ARMAZ.AddObject(ctrlLoteLocArmaz);

                                            //    apoloService.SaveChanges();
                                            //}

                                            #endregion

                                            IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE itemTransfEstqLocArmazLote = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

                                            itemTransfEstqLocArmazLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                            itemTransfEstqLocArmazLote.TransfEstqLocArmazNum = itemTransfEstqLocArmaz.TransfEstqLocArmazNum;
                                            itemTransfEstqLocArmazLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                            itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSeq = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq;
                                            itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSaida = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida;
                                            itemTransfEstqLocArmazLote.ItTransfEstqLocArmazEntrada = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
                                            itemTransfEstqLocArmazLote.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                            itemTransfEstqLocArmazLote.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                            itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = item.QtdeOvos;
                                            itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                                            itemTransfEstqLocArmazLote.CtrlLoteNum = item.LoteCompleto;
                                            itemTransfEstqLocArmazLote.CtrlLoteDataValid = item.DataProducao;

                                            loteErro = item.LoteCompleto;
                                            dataProducaoErro = item.DataProducao;

                                            apoloService.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(itemTransfEstqLocArmazLote);
                                            apoloService.SaveChanges();

                                            #endregion

                                            qtdTotalItem = qtdTotalItem + item.QtdeOvos;

                                            #region Caso seja o último lote da linhagem, adiciona o total no item e salva

                                            if (listaImportTipoOvo.IndexOf(item) == (listaImportTipoOvo.Count - 1))
                                            {
                                                itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                                qtdTotalItem = 0;

                                                apoloService.SaveChanges();
                                            }

                                            #endregion

                                            #region Salva informações do DEO

                                            var listaOriginal = hlbapp.LayoutDiarioExpedicaos
                                                    .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                                                        && l.CodItemImportaDEO == item.CodItemImportaDEO))
                                                    .ToList();

                                            foreach (var item2 in listaOriginal)
                                            {
                                                item2.Importado = "Conferido";
                                                item2.TipoDEO = tipoDEO;
                                                item2.Incubatorio = incubatorio;
                                                item2.NFNum = nfNum;
                                                item2.GTANum = gta;
                                                item2.Lacre = lacre;
                                                item2.Observacao = observacao;
                                                item2.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                                #region Gera LOG

                                                //LayoutDiarioExpedicao diario = db.DiarioExpedicao.Where(d => d.ID == item2.ID).FirstOrDefault();
                                                //LOG_LayoutDiarioExpedicaos log = InsereLOGs(DateTime.Now, "Conferência", usuario, item2.QtdeOvos, item2);
                                                //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(log);
                                                //hlbapp.SaveChanges();

                                                #endregion
                                            }

                                            //try
                                            //{
                                            //hlbapp.SaveChanges();
                                            //}
                                            //catch (OptimisticConcurrencyException)
                                            //{
                                            //    //Atualiza a entidade contato,usando ClientWins;
                                            //    hlbapp.Refresh(System.Data.Objects.RefreshMode.ClientWins, log);
                                            //    //chama SaveChanges novamente
                                            //    hlbapp.SaveChanges();
                                            //}

                                            //hlbapp.SaveChanges();

                                            item.Importado = "Conferido";
                                            item.TipoDEO = tipoDEO;
                                            item.Incubatorio = incubatorio;
                                            item.NFNum = nfNum;
                                            item.GTANum = gta;
                                            item.Lacre = lacre;
                                            item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                            #endregion

                                            #region Importa FLIP

                                            List<ImportaDiarioExpedicao> listaItensDadosAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];

                                            var listaImportaFLIP = listaItensDadosAntigosImport
                                                    .Where(a => a.DataHoraCarreg == item.DataHoraCarreg
                                                        && a.DataProducao == item.DataProducao
                                                        && a.Granja == item.Granja
                                                        && a.LoteCompleto == item.LoteCompleto)
                                                    .ToList();

                                            if (listaImportaFLIP.Count > 0)
                                            {
                                                foreach (var itemAntigo in listaImportaFLIP)
                                                {
                                                    if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                                        TransferEggsFLIPImport(itemAntigo, localArmazSaida.USERGeracaoFLIP, usuario, "DEL", incubatorio);
                                                    else
                                                        ImportaSaidaFLIPImport(itemAntigo, localArmazSaida.USERGeracaoFLIP,
                                                            usuario, "DEL", tipoDEO);
                                                }
                                            }

                                            if (tipoDEO.Equals("Transf. Ovos Incubáveis"))
                                                TransferEggsFLIPImport(item, localArmazSaida.USERGeracaoFLIP, usuario, "INS", incubatorio);
                                            else
                                                ImportaSaidaFLIPImport(item, localArmazSaida.USERGeracaoFLIP,
                                                    usuario, "INS", tipoDEO);

                                            #endregion

                                            linhagemAnterior = item.Linhagem;
                                            dataAnterior = item.DataHoraCarreg;
                                        }

                                        db.SaveChanges();
                                        apoloService.SaveChanges();
                                        hlbapp.SaveChanges();

                                        #region Integra a transferência com o Estoque

                                        apoloService.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                            usuario);

                                        #endregion
                                    }

                                    #endregion
                                }

                                Session["escondeLinkPrincipal"] = "Não";
                                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                                #endregion
                            }
                        }
                        else
                        {
                            #region DEO feito por Terceiros

                            nucleos.FillFarmsAllLocation(flip.FLOCKS1);

                            string location = flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;

                            string localContra = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().LocArmazCodEstr;

                            incubatorio = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().USERCodigoFLIP;

                            if (tipoDEO.Equals("Inventário de Ovos"))
                            {
                                foreach (var item in lista)
                                {
                                    item.NFNum = nfNum;
                                    item.ResponsavelCarreg = Session["usuario"].ToString();
                                    item.Importado = "Sim";
                                    item.Incubatorio = incubatorio;
                                    item.TipoDEO = tipoDEO;
                                    item.GTANum = gta;
                                    item.Lacre = lacre;
                                    item.Observacao = observacao;

                                    //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, tipoDEO, usuario, 0, item));
                                    //hlbapp.SaveChanges();
                                }

                                foreach (var item in listaImport)
                                {
                                    item.NFNum = nfNum;
                                    item.ResponsavelCarreg = Session["usuario"].ToString();
                                    item.Importado = "Sim";
                                    item.Incubatorio = incubatorio;
                                    item.TipoDEO = tipoDEO;
                                    item.GTANum = gta;
                                    item.Lacre = lacre;
                                }

                                db.SaveChanges();
                                hlbapp.SaveChanges();

                                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                            }
                            else
                            {
                                ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                    .Where(e1 => e1.USERFLIPCodigo == granja)
                                    .FirstOrDefault();

                                empresa = bdApolo.EMPRESA_FILIAL.Where(e => e.EmpCod == entidade1.USERCodIncFLIPEntrada)
                                    .FirstOrDefault();

                                #region Carrega variáveis e objetos

                                string linhagemAnterior = "";
                                MOV_ESTQ movEstq = new MOV_ESTQ();
                                ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                                LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                                CTRL_LOTE_ITEM_MOV_ESTQ lote = new CTRL_LOTE_ITEM_MOV_ESTQ();

                                TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();
                                ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();
                                decimal? qtdTotalItem = 0;

                                DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                                LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                        .Where(l => l.USERCodigoFLIP == empresa.USERFLIPCod && l.USERTipoProduto == tipoDEO)
                                        .FirstOrDefault();

                                LOC_ARMAZ localArmazSaida = apoloService.LOC_ARMAZ
                                        .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                                        .FirstOrDefault();

                                string tipoLanc = localArmazSaida.USERTipoLancSaidaInc;
                                string unidadeMedida = "UN";

                                DateTime dataMov = dataFiltro;

                                #endregion

                                #region Verifica se já tem a Transferência. Caso tenha, será deletada.

                                ImportaDiarioExpedicao itemDeo = listaImport.First();

                                int nfNumTransf;
                                if (itemDeo.NumIdentificacao.Equals(""))
                                    nfNumTransf = 0;
                                else
                                    nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                                transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                                    .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                                    .FirstOrDefault();

                                if (transfEstqLocArmaz != null)
                                {
                                    //MOV_ESTQ saidaTransferencia = apoloService.MOV_ESTQ
                                    //        .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                    //            && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                    //        .FirstOrDefault();

                                    //if (saidaTransferencia != null)
                                    //{
                                    //    ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                                    //    apoloService.delete_movestq(saidaTransferencia.EmpCod, saidaTransferencia.MovEstqChv, usuario,
                                    //        rmensagem);
                                    //}

                                    var listaMovEstq = apoloService.MOV_ESTQ
                                            .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                                && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                                            .ToList();

                                    foreach (var item in listaMovEstq)
                                    {
                                        DeletaMovEstq(item);
                                    }

                                    apoloService.SaveChanges();

                                    apoloService.delete_transfestqlocarmaz(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                        usuario);
                                }

                                #endregion

                                #region Insere Nova Transferência

                                transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                                numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                                apolo.GerarCodigo("1", "TRANSF_ESTQ_LOC_ARMAZ", numero);

                                transfEstqLocArmaz.EmpCod = empresa.EmpCod;
                                transfEstqLocArmaz.TipoLancCod = tipoLanc;
                                transfEstqLocArmaz.TransfEstqLocArmazData = Convert.ToDateTime(dataMov.ToShortDateString());
                                transfEstqLocArmaz.TransfEstqLocArmazNum = Convert.ToInt32(numero.Value);
                                transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis p/ Comercial";

                                apoloService.TRANSF_ESTQ_LOC_ARMAZ.AddObject(transfEstqLocArmaz);
                                apoloService.SaveChanges();

                                #endregion

                                short ultimaSequencia = 0;

                                foreach (var item in listaImport)
                                {
                                    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                    //item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                    #region Se mudou a linhagem da lista, insere um note Item

                                    if (linhagemAnterior != item.Linhagem)
                                    {
                                        ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem)
                                            .FirstOrDefault();

                                        //short ultimaSequencia = 0;

                                        if (!linhagemAnterior.Equals(""))
                                        {
                                            itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                            qtdTotalItem = 0;

                                            ultimaSequencia++;

                                            apoloService.SaveChanges();
                                        }
                                        else
                                        {
                                            ultimaSequencia = 1;
                                        }

                                        itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                                        itemTransfEstqLocArmaz.EmpCod = transfEstqLocArmaz.EmpCod;
                                        itemTransfEstqLocArmaz.TransfEstqLocArmazNum = transfEstqLocArmaz.TransfEstqLocArmazNum;
                                        itemTransfEstqLocArmaz.ProdCodEstr = produto.ProdCodEstr;
                                        itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq = ultimaSequencia;
                                        itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida = localArmazSaida.LocArmazCodEstr;
                                        itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada = localArmazCadastro.LocArmazCodEstr;
                                        itemTransfEstqLocArmaz.ItTransfEstqLocArmazObs = transfEstqLocArmaz.TransfEstqLocArmazObs;

                                        apoloService.ITEM_TRANSF_ESTQ_LOC_ARMAZ.AddObject(itemTransfEstqLocArmaz);
                                        apoloService.SaveChanges();
                                    }

                                    #endregion

                                    #region Insere o Lote

                                    PROD_UNID_MED prodUnidMed = apoloService.PROD_UNID_MED
                                        .Where(p => p.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                            && p.ProdUnidMedCod == unidadeMedida)
                                        .FirstOrDefault();

                                    IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE itemTransfEstqLocArmazLote = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

                                    itemTransfEstqLocArmazLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                    itemTransfEstqLocArmazLote.TransfEstqLocArmazNum = itemTransfEstqLocArmaz.TransfEstqLocArmazNum;
                                    itemTransfEstqLocArmazLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                    itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSeq = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq;
                                    itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSaida = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida;
                                    itemTransfEstqLocArmazLote.ItTransfEstqLocArmazEntrada = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
                                    itemTransfEstqLocArmazLote.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                    itemTransfEstqLocArmazLote.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                    itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = item.QtdeOvos;
                                    itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                                    itemTransfEstqLocArmazLote.CtrlLoteNum = item.LoteCompleto;
                                    itemTransfEstqLocArmazLote.CtrlLoteDataValid = item.DataProducao;

                                    apoloService.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(itemTransfEstqLocArmazLote);
                                    apoloService.SaveChanges();

                                    #endregion

                                    qtdTotalItem = qtdTotalItem + item.QtdeOvos;

                                    #region Caso seja o último lote da linhagem, adiciona o total no item e salva

                                    if (listaImport.IndexOf(item) == (listaImport.Count - 1))
                                    {
                                        itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtdTotalItem;

                                        qtdTotalItem = 0;

                                        apoloService.SaveChanges();
                                    }

                                    #endregion

                                    #region Salva informações do DEO

                                    var listaOriginal = hlbapp.LayoutDiarioExpedicaos
                                                .Where(d => hlbapp.LayoutDEO_X_ImportaDEO.Any(l => l.CodItemDEO == d.CodItemDEO
                                                    && l.CodItemImportaDEO == item.CodItemImportaDEO))
                                                .ToList();

                                    foreach (var item2 in listaOriginal)
                                    {
                                        item2.Importado = "Sim";
                                        item2.TipoDEO = tipoDEO;
                                        item2.Incubatorio = incubatorio;
                                        item2.NFNum = nfNum;
                                        item2.GTANum = gta;
                                        item2.Lacre = lacre;
                                        item2.Observacao = observacao;
                                        item2.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                        #region Gera LOG

                                        //LayoutDiarioExpedicao diario = db.DiarioExpedicao.Where(d => d.ID == item2.ID).FirstOrDefault();

                                        //LOG_LayoutDiarioExpedicaos log = InsereLOGs(DateTime.Now, "Importação", usuario, item2.QtdeOvos, item2);
                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(log);
                                        //hlbapp.SaveChanges();

                                        //try
                                        //{
                                        //    hlbapp.SaveChanges();
                                        //}
                                        //catch (OptimisticConcurrencyException)
                                        //{
                                        //    //Atualiza a entidade contato,usando ClientWins;
                                        //    hlbapp.Refresh(System.Data.Objects.RefreshMode.ClientWins, log);
                                        //    //chama SaveChanges novamente
                                        //    hlbapp.SaveChanges();
                                        //}

                                        #endregion
                                    }

                                    item.Importado = "Sim";
                                    item.TipoDEO = tipoDEO;
                                    item.Incubatorio = incubatorio;
                                    item.NFNum = nfNum;
                                    item.GTANum = gta;
                                    item.Lacre = lacre;
                                    item.NumIdentificacao = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                    #endregion

                                    linhagemAnterior = item.Linhagem;
                                    dataAnterior = item.DataHoraCarreg;
                                }

                                db.SaveChanges();
                                apoloService.SaveChanges();
                                hlbapp.SaveChanges();

                                //#region Integra a transferência com o Estoque

                                //apoloService.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                //    usuario);

                                //#endregion
                            }

                            Session["escondeLinkPrincipal"] = "Não";
                            return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                            #endregion
                        }
                        //}

                        */

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        #region Tratamento de Erro

                        string retorno = "";
                        //string retornoVB = "";

                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                        if (ex.InnerException != null)
                            retorno = "Erro " + origemErro + ": " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                                + "Segundo erro: " + ex.InnerException.Message + (char)10 + (char)13
                                + "Lote: " + loteErro + (char)10 + (char)13
                                + "Data Produção: " + dataProducaoErro.ToShortDateString() + (char)10 + (char)13
                                + "ID: " + IDErro.ToString() + (char)10 + (char)13
                                + "Line Number: " + linenum.ToString();
                        else
                            retorno = "Erro " + origemErro + ": " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                                + "Lote: " + loteErro + (char)10 + (char)13
                                + "Data Produção: " + dataProducaoErro.ToShortDateString() + (char)10 + (char)13
                                + "ID: " + IDErro.ToString() + (char)10 + (char)13
                                + "Line Number: " + linenum.ToString();

                        //retornoVB = "ERRO AO SALVAR O DEO!!! POR FAVOR, TENTE SALVAR MAIS UMAS 02"
                        //    + " VEZES CASO OCORRA ERRO!!! SE NA TERCEIRA CONTINUAR, ENVIAR E-MAIL AO "
                        //    + "DEPTO. DE T.I. NO ti@hyline.com.br COM A DATA DO DEO E A EMPRESA!!!";

                        ViewBag.Erro = retorno;

                        retorno = retorno + (char)10 + (char)13 + "Usuário: " + usuario + (char)10 + (char)13
                                                + "Granja: " + granja + (char)10 + (char)13
                                                + "Data do DEO: " + dataFiltro.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                        EnviarEmail(retorno, assuntoEmail, paraNome, paraEmail, copiaPara, anexo, "5", "Texto");

                        var lista2 = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                        if (lista2.Count == 0)
                            lista2 = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");

                        //hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, lista2);
                        //hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, listaImport);

                        Session["escondeLinkPrincipal"] = "Não";
                        return View("ListaDEOs", lista2);

                        #endregion
                    }
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult DeleteDEO(DateTime dataFiltro, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"].ToString() != "0")
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                bdApolo.CommandTimeout = 1000;
                apoloService.CommandTimeout = 1000;

                Session["dataHoraCarreg"] = dataFiltro;
                Session["numIdentificacaoSelecionado"] = numIdentificacao;
                string granja = Session["granjaSelecionada"].ToString();
                string tipoDEO = Session["tipoDEOselecionado"].ToString();

                DateTime dataVerifica = Convert.ToDateTime(dataFiltro.ToShortDateString());

                if (ExisteFechamentoEstoque(dataVerifica, granja))
                {
                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                        .Where(e => e.USERFLIPCod == granja)
                        .FirstOrDefault();

                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString() 
                    //                //+ " na empresa " + empresa.EmpNome  
                    //                + "! Não pode ser excluído este Diário de Expedição!"
                    //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }

                if (ExisteDEOSolicitacaoAjusteEstoqueAberto(granja))
                {
                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                        + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }

                //Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                //    "Cadastro");
                Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                if (((List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"]).Count == 0)
                    Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                        "Cadastro");

                List<LayoutDiarioExpedicaos> listaItensDadosAntigos = (List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"];

                LayoutDiarioExpedicaos diarioExpedicao = listaItensDadosAntigos.FirstOrDefault();

                if (diarioExpedicao != null)
                {
                    if ((diarioExpedicao.Importado != null) && (diarioExpedicao.TipoDEO != null))
                    {
                        if ((diarioExpedicao.Importado.Equals("Conferido"))
                            && diarioExpedicao.TipoDEO.Equals("Ovos Incubáveis"))
                        {
                            DateTime dataInicial;
                            DateTime dataFinal;

                            if (Session["dataInicial"] == null)
                            {
                                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                            }
                            else
                            {
                                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                            }

                            ViewBag.Erro = "Diário de Expedição já conferido! Por favor, solicitar o Cancelamento da Conferência"
                                + " para realizar a exclusão!";
                            return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                        }
                    }
                }

                return View("DeleteDEO");
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public string DeleteTLAApolo(DateTime dataFiltro, string granja, string tipoDEO)
        {
            string retorno = "";

            try
            {
                #region Carrega Variáveis e Objetos Globais

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                string assuntoEmail = "**** ERRO AO INTEGRAR DEO COM APOLO ****";
                string paraNome = "T.I.";
                string paraEmail = "ti@hyline.com.br";
                string copiaPara = "";
                string anexo = "";

                string login = Session["login"].ToString();
                string usuario = "";
                if (login.Equals("palves"))
                    usuario = "RIOSOFT";
                else
                    usuario = login.ToUpper();

                var lista2 = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                    "Cadastro");
                var lista = CarregarItensDEOImport(hlbapp, dataFiltro, granja);

                ImportaDiarioExpedicao diarioExpedicao = lista.FirstOrDefault();

                string empresaEstoque = "CH";
                string incubatorio = granja;
                if (granja.Equals("PL"))
                {
                    empresaEstoque = "PL";
                    incubatorio = "NM";
                }

                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                        .Where(e => e.USERFLIPCod == empresaEstoque)
                        .FirstOrDefault();

                #endregion

                #region Delete Apolo

                #region Carrega variáveis e objetos

                TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                //LOC_ARMAZ localArmazSaida = apoloService.LOC_ARMAZ
                //        .Where(l => l.USERCodigoFLIP == incubatorio && l.USERTipoProduto == "Ovos Incubáveis")
                //        .FirstOrDefault();

                #endregion

                #region Verifica se já tem a Transferência. Caso tenha, será deletada.

                var listaTipoOvo = lista2.GroupBy(g => g.TipoOvo).ToList();

                foreach (var item in listaTipoOvo)
                {
                    LayoutDiarioExpedicaos itemDeo = lista2.Where(w => w.TipoOvo == item.Key).FirstOrDefault();

                    int nfNumTransf;
                    if (itemDeo.NumIdentificacao.Equals(""))
                        nfNumTransf = 0;
                    else
                        nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                    transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                        .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                        .FirstOrDefault();

                    if (transfEstqLocArmaz != null)
                    {
                        //MOV_ESTQ saidaTransferencia = apoloService.MOV_ESTQ
                        //    .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                        //        && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                        //    .FirstOrDefault();

                        //if (saidaTransferencia != null)
                        //{
                        //    System.Data.Objects.ObjectParameter rmensagem =
                        //        new System.Data.Objects.ObjectParameter("rmensagem", typeof(global::System.String));

                        //    apoloService.delete_movestq(saidaTransferencia.EmpCod, saidaTransferencia.MovEstqChv, usuario,
                        //        rmensagem);
                        //}

                        List<MOV_ESTQ> listaMovEstq = apoloService.MOV_ESTQ
                            .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                                && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                            .ToList();

                        foreach (var movEstq in listaMovEstq)
                        {
                            DeletaMovEstq(movEstq);
                        }

                        apoloService.SaveChanges();

                        apoloService.delete_transfestqlocarmaz(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                            usuario);
                    }
                }

                #endregion

                #endregion

                #region DEO de Terceiro ou Granjas (Rotina desativada)
                /*else
                {
                    #region Carrega variáveis e objetos

                    TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                    LOC_ARMAZ localArmazSaida = apoloService.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == granja)
                            .FirstOrDefault();

                    empresa = apoloService.EMPRESA_FILIAL
                        .Where(e => e.USERFLIPCod == diarioExpedicao.Incubatorio)
                        .FirstOrDefault();

                    #endregion

                    #region Verifica se já tem a Transferência. Caso tenha, será deletada.

                    //ENTIDADE1 entidade1 = apoloService.ENTIDADE1.Where(e1 => e1.USERFLIPCodigo == granja).FirstOrDefault();

                    LayoutDiarioExpedicao itemDeo = lista2.First();

                    int nfNumTransf;
                    if (itemDeo.NumIdentificacao.Equals(""))
                        nfNumTransf = 0;
                    else
                        nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                    transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                        .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                        .FirstOrDefault();

                    if (transfEstqLocArmaz != null)
                    {
                        MOV_ESTQ saidaTransferencia = apoloService.MOV_ESTQ
                        .Where(m => m.EmpCod == empresa.EmpCod && m.MovEstqDocEspec == "TLA"
                            && m.MovEstqDocSerie == "00" && m.MovEstqDocNum == itemDeo.NumIdentificacao)
                        .FirstOrDefault();

                        if (saidaTransferencia != null)
                        {
                            System.Data.Objects.ObjectParameter rmensagem =
                                new System.Data.Objects.ObjectParameter("rmensagem", typeof(global::System.String));

                            apoloService.delete_movestq(saidaTransferencia.EmpCod, saidaTransferencia.MovEstqChv, usuario,
                                rmensagem);
                        }

                        apoloService.delete_transfestqlocarmaz(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                            usuario);
                    }

                    #endregion
                }*/

                #endregion

                return retorno;
            }
            catch (Exception ex)
            {
                retorno = "Erro ao deletar Integração com o Apolo: "
                    + ex.Message;

                if (ex.InnerException != null)
                    retorno = retorno + " / " + ex.InnerException.Message;

                return retorno;
            }
        }

        public ActionResult DeleteDEOConfirma(DateTime dataFiltro, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"].ToString() != "0")
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                bdApolo.CommandTimeout = 1000;
                apoloService.CommandTimeout = 1000;

                string granja = Session["granjaSelecionada"].ToString();
                string tipoDEO = Session["tipoDEOselecionado"].ToString();

                DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");

                try
                {
                    #region Carrega Variáveis e Objetos Globais

                    string login = Session["login"].ToString();
                    string usuario = "";
                    if (login.Equals("palves"))
                        usuario = "RIOSOFT";
                    else
                        usuario = login.ToUpper();

                    //var lista2 = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");
                    var lista2 = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (lista2.Count == 0)
                        lista2 = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");
                    //var lista = CarregarItensDEOImport(hlbapp, dataFiltro, granja);

                    //ImportaDiarioExpedicao diarioExpedicao = lista.FirstOrDefault();

                    #endregion

                    #region **** NÃO INTEGRA APOLO ****

                        /*
                        if (diarioExpedicao != null)
                        {
                            if (diarioExpedicao.TipoDEO != null)
                            {
                                if (diarioExpedicao.TipoDEO.Equals("Transferência entre Linhagens"))
                                {
                                    #region Deleta Apolo

                                    string empresa = "1";
                                    if (granja.Equals("PL"))
                                        empresa = "20";
                                    
                                    int chaveSaida = Convert.ToInt32(diarioExpedicao.ResponsavelCarreg);
                                    int chaveEntrada = Convert.ToInt32(diarioExpedicao.ResponsavelReceb);
                                    System.Data.Objects.ObjectParameter numero =
                                        new System.Data.Objects.ObjectParameter("rmensagem", typeof(global::System.String));

                                    apoloService.delete_movestq(empresa, chaveEntrada, usuario, numero);
                                    apoloService.delete_movestq(empresa, chaveSaida, usuario, numero);

                                    #endregion

                                    DeletaTransferenciaDeLinhagensFLIP(lista);

                                    foreach (var item in lista)
                                    {
                                        hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                                    }
                                }
                                else
                                {
                                    #region Deleção do Estoque no Apolo

                                    string msgDeletaApolo = "";

                                    msgDeletaApolo = DeleteTLAApolo(dataFiltro, granja, tipoDEO);

                                    if (msgDeletaApolo != "")
                                    {
                                        ViewBag.Erro = msgDeletaApolo;
                                        return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                                    }

                                    #endregion

                                    #region Deleta FLIP

                                    foreach (var item in lista)
                                    {
                                        MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL verificaEmpresa = bdApolo.EMPRESA_FILIAL
                                            .Where(e => e.USERFLIPCod == item.Granja
                                                || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod
                                                    && c.EmpFilCertificNum == item.Granja))
                                            .FirstOrDefault();

                                        if (item.Granja.Equals("PL"))
                                        {
                                            flocks.Fill(flip.FLOCKS);

                                            FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                                .Where(f => f.FLOCK_ID == item.LoteCompleto)
                                                .FirstOrDefault();

                                            flock_data.FillFlockData(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                                item.DataProducao);

                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                        .Where(d => d.FLOCK_ID == item.LoteCompleto
                                                            && d.TRX_DATE == item.DataProducao)
                                                        .FirstOrDefault();

                                            if (dataRow != null)
                                            {
                                                if ((dataRow.NUM_1 - item.QtdeOvos) > 0)
                                                {
                                                    dataRow.NUM_1 = dataRow.NUM_1 - item.QtdeOvos;
                                                    dataRow.NUM_8 = 0;
                                                    flock_data.Update(dataRow);
                                                }
                                                else
                                                {
                                                    flock_data.Delete(dataRow.COMPANY, dataRow.REGION, dataRow.LOCATION, dataRow.FARM_ID, dataRow.FLOCK_ID,
                                                        dataRow.TRX_DATE);
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    LOC_ARMAZ localArmaz = apoloService.LOC_ARMAZ
                                        .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                                        .FirstOrDefault();

                                    foreach (var item in lista)
                                    {
                                        if (!item.TipoDEO.Equals("Inventário de Ovos"))
                                        {
                                            if ((item.Granja.Equals("CH")) || (item.Granja.Equals("PH"))
                                                 || (item.Granja.Equals("PL") && item.Incubatorio.Equals("PL")))
                                            {
                                                if (item.TipoDEO != null)
                                                {
                                                    if (item.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                                                        TransferEggsFLIPImport(item, localArmaz.USERGeracaoFLIP, usuario, "DEL",
                                                            item.Incubatorio);
                                                    else
                                                        ImportaSaidaFLIPImport(item, localArmaz.USERGeracaoFLIP,
                                                            usuario, "DEL", item.TipoDEO);
                                                }
                                            }
                                        }
                                        hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                                    }
                                }
                            }
                        }
                         * 
                         * */

                        #endregion

                    //foreach (var item in lista)
                    //{
                    //    hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                    //}

                    foreach (var item in lista2)
                    {
                        hlbapp.LayoutDiarioExpedicaos.DeleteObject(item);

                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, "Exclusão Total", usuario, 0, item));
                        hlbapp.SaveChanges();
                    }

                    //foreach (var item in lista2)
                    //{
                    //    foreach (var item2 in lista)
                    //    {
                    //        LayoutDEO_X_ImportaDEO deoXImporta = hlbapp.LayoutDEO_X_ImportaDEO
                    //            .Where(d => d.CodItemDEO == item.CodItemDEO
                    //                && d.CodItemImportaDEO == item2.CodItemImportaDEO)
                    //            .FirstOrDefault();

                    //        if (deoXImporta != null)
                    //            hlbapp.LayoutDEO_X_ImportaDEO.DeleteObject(deoXImporta);
                    //    }
                    //}

                    hlbapp.SaveChanges();
                    //db.SaveChanges();

                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }
                catch (Exception ex)
                {
                    ViewBag.Erro = "Erro ao excluir DEO " + dataFiltro.ToShortDateString()
                                        + ": " + ex.Message
                                        + " / Segundo erro: " + ex.InnerException.Message;

                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult LogDEO(string lote, DateTime dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            DateTime dataHoraDEO = Convert.ToDateTime(Session["dataHoraCarreg"]);

            var lista = hlbappSession.LOG_LayoutDiarioExpedicaos
                .Where(w => w.DataHoraCarreg == dataHoraDEO
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .OrderBy(o => o.DataHoraOper)
                .ToList();

            Session["ListaHistoricoDEO"] = lista;

            return View();
        }

        #endregion

        #region Métodos Gerar DEO

        public ActionResult ParametrosGerarDEO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["primeiraDataProducao"] = DateTime.Today.AddDays(-1);
            Session["ultimaDataProducao"] = DateTime.Today;

            return View();
        }

        public ActionResult GerarDEO(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega variáveis

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

            DateTime dataInicialProducao = Convert.ToDateTime(model["dataInicial"]);
            DateTime dataFinalProducao = Convert.ToDateTime(model["dataFinal"]);

            string granja = Session["granjaSelecionada"].ToString();
            Session["tipoDEOselecionado"] = "Ovos Incubáveis";
            
            bdApolo.CommandTimeout = 1000;
            apoloService.CommandTimeout = 1000;

            Session["operacao"] = "Create";

            DateTime dataFiltro = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            DateTime dataVerifica = DateTime.Today;

            Session["dataHoraCarreg"] = dataFiltro;

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
            Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);

            Session["nfNum"] = "";
            Session["Observacao"] = "";
            Session["GTA"] = "";
            Session["Lacre"] = "";
            string tipoDEO = Session["tipoDEOselecionado"].ToString();

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            if (ExisteFechamentoEstoque(dataVerifica, granja))
            {
                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod == granja)
                    .FirstOrDefault();

                //string responsavel = "Miriene Gomes";
                //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                //    responsavel = "Sérica Doimo";
                //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                //                + " na empresa " + empresa.EmpNome
                //                + "! Não pode ser inserido novo Diário de Expedição!"
                //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                    + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                    + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            if (granja == "")
            {
                ViewBag.Erro = "Para inserir um DEO, selecione uma granja!";
                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            string location = "";
            if (granja.Equals("SB") || granja.Equals("PH"))
                location = "GP";
            else
                location = "PP";
            Session["location"] = location;

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);
            List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
            string incubatorio = "PH";
            Session["incubatorioDestinoSelecionado"] = incubatorio;
            Session["descricaoIncubatorioDestinoSelecionado"] = "Incubatório " 
                + items.Where(w => w.Value == "PH").FirstOrDefault().Text;

            #endregion

            #region Gerar DEO

            #region Carrega Lista de Núcleos

            FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            fTA.FillFarmsDEO(fDT);

            var listaNucleos = fDT.Where(f => f.FARM_ID.StartsWith(granja) && f.LOCATION == location)
                .OrderBy(o => o.FARM_ID)
                .ToList();

            #endregion

            foreach (var nucleo in listaNucleos)
            {
                #region Carrega Lista de Lotes Ativos

                flocks.FillActivesByFarm(flip.FLOCKS, "HYBR", "BR", location, nucleo.FARM_ID);
                var listaLotes = flip.FLOCKS
                    .Where(w => !w.FLOCK_ID.Contains("BK")
                        && w.IsSELL_DATENull())
                    .OrderBy(o => o.NUM_2) // Galpão
                    .ThenBy(t => t.VARIETY)
                    .ToList();

                #endregion

                foreach (var lote in listaLotes)
                {
                    #region Gera as linhas do DEO de acordo com os lotes e o período informado

                    DateTime dataProducao = dataInicialProducao;

                    while (dataProducao <= dataFinalProducao)
                    {
                        LayoutDiarioExpedicaos deo = new LayoutDiarioExpedicaos();

                        #region Calcula Idade

                        int idade = ((Convert.ToDateTime(dataProducao) - lote.HATCH_DATE).Days) / 7;

                        #endregion

                        deo.Nucleo = lote.FARM_ID;
                        deo.Galpao = lote.NUM_2.ToString();
                        deo.Lote = lote.NUM_1.ToString();
                        deo.Idade = idade;
                        deo.Linhagem = lote.VARIETY;
                        deo.LoteCompleto = lote.FLOCK_ID;
                        deo.DataProducao = dataProducao;
                        deo.NumeroReferencia = dataProducao.DayOfYear.ToString();
                        deo.QtdeOvos = 0;
                        deo.QtdeBandejas = 0;
                        deo.Usuario = Session["login"].ToString();
                        deo.DataHora = DateTime.Now;
                        deo.DataHoraCarreg = dataFiltro;
                        deo.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                        deo.ResponsavelCarreg = null;
                        deo.ResponsavelReceb = null;
                        deo.NFNum = "";
                        deo.Granja = granja;
                        deo.Importado = "Não";
                        deo.Incubatorio = incubatorio;
                        deo.TipoDEO = tipoDEO;
                        deo.GTANum = "";
                        deo.Lacre = "";
                        deo.NumIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                        System.Data.Objects.ObjectParameter numeroNovo =
                                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                        apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numeroNovo);
                        deo.CodItemDEO = Convert.ToInt32(numeroNovo.Value);

                        deo.Observacao = "DEO Gerado automaticamente.";
                        deo.TipoOvo = "";
                        deo.QtdDiferenca = 0;
                        deo.QtdeConferencia = 0;

                        hlbapp.LayoutDiarioExpedicaos.AddObject(deo);

                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Gerado",
                            Session["login"].ToString(), 0, "", "", deo);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);

                        dataProducao = dataProducao.AddDays(1);

                        Session["qtdOvos_" + deo.LoteCompleto.ToString() + "|"
                            + deo.DataProducao.ToShortDateString()] = "";
                    }

                    #endregion
                }
            }

            hlbapp.SaveChanges();
            hlbappLOG.SaveChanges();

            #endregion

            return View("DEOGerado", hlbapp.LayoutDiarioExpedicaos
                .Where(d => d.DataHoraCarreg == dataFiltro)
                .ToList());
        }

        [HttpPost]
        public ActionResult SaveDEOGerado(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string tipoDEO = Session["tipoDEOselecionado"].ToString();
            string numIdentificacao = "Sem ID";
            if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            var listaDEO = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
            if (listaDEO.Count == 0)
                listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", "Geração");

            try
            {
                if (!ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    string login = Session["login"].ToString();
                    string usuario = login.ToUpper();
                    string nfNum = "";
                    if (model["nfNum"] != null) nfNum = model["nfNum"].ToString();
                    System.Data.Objects.ObjectParameter numeroNF =
                            new System.Data.Objects.ObjectParameter("numero", typeof(global::System.String));
                    if (nfNum != "")
                        nfNum = Convert.ToInt32(nfNum).ToString();
                    apoloService.CONCAT_ZERO_ESQUERDA(nfNum, 10, numeroNF);
                    nfNum = numeroNF.Value.ToString();

                    string observacao = "";
                    if (model["nfNum"] != null) observacao = model["Observacao"].ToString();
                    string gta = "";
                    if (model["GTA"] != null) gta = model["GTA"].ToString();
                    string lacre = "";
                    if (model["Lacre"] != null) lacre = model["Lacre"].ToString();

                    foreach (var item in listaDEO)
                    {
                        var qtdeInformada = Convert.ToInt32(model["qtdOvos_" + item.LoteCompleto.ToString() + "|"
                            + item.DataProducao.ToShortDateString()]);

                        item.Importado = "Sim";
                        item.QtdeOvos = qtdeInformada;
                        item.NFNum = nfNum;
                        item.Observacao = observacao;
                        item.GTANum = gta;
                        item.Lacre = lacre;

                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                            "Salvar DEO Gerado",
                            usuario, 0, "", "", item);

                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }

                    hlbapp.SaveChanges();
                    hlbappLOG.SaveChanges();

                    ViewBag.Mensagem = "DEO "
                        + dataCarreg.ToShortDateString() + " salvo com sucesso!";
                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "";
                //string retornoVB = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                {
                    retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                    if (ex.InnerException.InnerException != null)
                        retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                            + "Line Number: " + linenum.ToString();
                }
                else
                    retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                ViewBag.Erro = retorno;
                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));

                #endregion
            }
        }

        #endregion

        #region Métodos Carrega Listas DEO

        public List<LayoutDiarioExpedicaoPai> CarregarListaDEO()
        {
            //string acessoPlanalto = "";
            //if (Session["empresa"].ToString().Contains("PL"))
            //    acessoPlanalto = "NM";

            #region Carrega Acesso Incubatorios

            List<SelectListItem> listaIncubatoriosAcesso = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];

            string acessoIncubatorios = "";
            foreach (var item in listaIncubatoriosAcesso)
            {
                acessoIncubatorios = acessoIncubatorios + item.Value;
            }

            #endregion

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var lista = hlbapp.LayoutDiarioExpedicaos
                    //.Where(w => (w.Incubatorio == acessoPlanalto || acessoPlanalto == ""))
                    .Where(w => acessoIncubatorios.Contains(w.Incubatorio)
                        && w.TipoDEO != "Solicitação Ajuste de Estoque" && w.TipoDEO != "Classificação de Ovos")
                    .GroupBy(h => new
                    {
                        h.DataHoraCarreg,
                        h.NFNum,
                        h.NumIdentificacao
                    })
                    .OrderBy(j => j.Key.DataHoraCarreg)
                    .ToList();

            db.Database.ExecuteSqlCommand("delete from LayoutDiarioExpedicaoPais");
            db.SaveChanges();

            foreach (var item in lista)
            {
                LayoutDiarioExpedicaoPai pai = new LayoutDiarioExpedicaoPai();

                pai.DataHoraCarreg = item.Key.DataHoraCarreg;
                pai.NFNum = item.Key.NFNum;
                pai.NumIdentificacao = item.Key.NumIdentificacao;

                db.DiarioExpedicaoPai.Add(pai);
            }

            db.SaveChanges();

            return db.DiarioExpedicaoPai.ToList();
        }

        public List<LayoutDiarioExpedicaoPai> CarregarListaDEOFiltro(string Text, 
            DateTime dataInicial, DateTime dataFinal, string tipoDEO)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            //string acessoPlanalto = "";
            //if (Session["empresa"].ToString().Contains("PL") && Text.Equals("PL"))
            //    acessoPlanalto = "NM";

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

            #region Carrega Acesso Incubatorios

            List<SelectListItem> listaIncubatoriosAcesso = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];

            string acessoIncubatorios = "";
            foreach (var item in listaIncubatoriosAcesso)
            {
                acessoIncubatorios = acessoIncubatorios + item.Value;
            }

            #endregion

            var lista = hlbapp.LayoutDiarioExpedicaos
                    .Where(d => (d.Granja.Replace("PL","NM") == Text || Text == "")
                        && d.DataHoraCarreg >= dataInicial
                        && d.DataHoraCarreg <= dataFinal && 
                        (d.TipoDEO == tipoDEO || tipoDEO == "(Todos os Tipos)")
                        //&& (d.Incubatorio == acessoPlanalto || acessoPlanalto == "" || d.Incubatorio == null
                        && (acessoIncubatorios.Contains(d.Incubatorio) || d.Incubatorio == null
                            || d.Incubatorio == "" || d.Incubatorio == d.Granja)
                        && d.TipoDEO != "Solicitação Ajuste de Estoque" && d.TipoDEO != "Classificação de Ovos")
                    .GroupBy(h => new
                    {
                        h.DataHoraCarreg,
                        h.NFNum,
                        h.Granja,
                        h.TipoDEO,
                        h.GTANum,
                        h.Lacre,
                        h.Incubatorio,
                        h.NumIdentificacao
                    })
                    .OrderBy(j => j.Key.DataHoraCarreg)
                    .ToList();

            db.Database.ExecuteSqlCommand("delete from LayoutDiarioExpedicaoPais");
            db.SaveChanges();

            foreach (var item in lista)
            {
                LayoutDiarioExpedicaoPai pai = new LayoutDiarioExpedicaoPai();

                pai.DataHoraCarreg = item.Key.DataHoraCarreg;
                pai.NFNum = item.Key.NFNum;
                pai.Granja = item.Key.Granja;
                pai.TipoDEO = item.Key.TipoDEO;
                pai.GTANum = item.Key.GTANum;
                pai.Lacre = item.Key.Lacre;
                pai.IncubatorioDestino = item.Key.Incubatorio;
                pai.NumIdentificacao = item.Key.NumIdentificacao;

                db.DiarioExpedicaoPai.Add(pai);
            }

            db.SaveChanges();

            AtualizaGranjaSelecionada(Text);
            Session["granjaSelecionada"] = Text;

            //AtualizaTipoDEOSelecionado(tipoDEO);
            Session["tipoDEOselecionado"] = tipoDEO;

            return db.DiarioExpedicaoPai.ToList();
        }

        public List<LayoutDiarioExpedicaoPai> CarregarListaDEOStatus(string Text, string status, DateTime dataInicial, DateTime dataFinal)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            //string acessoPlanalto = "";
            //if (Session["empresa"].ToString().Contains("PL") && Session["empresa"].ToString().Count() == 2)
            //    acessoPlanalto = "NM";

            #region Carrega Acesso Incubatorios

            List<SelectListItem> listaIncubatoriosAcesso = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];

            string acessoIncubatorios = "";
            foreach (var item in listaIncubatoriosAcesso)
            {
                acessoIncubatorios = acessoIncubatorios + item.Value;
            }

            #endregion

            var lista = hlbapp.LayoutDiarioExpedicaos
                    .Where(d => d.Granja.Replace("PL", "NM") == Text && d.Importado == status 
                        && d.DataHoraCarreg >= dataInicial && d.DataHoraCarreg <= dataFinal
                        //&& (d.Incubatorio == acessoPlanalto || acessoPlanalto == "" || d.Incubatorio == null
                        && (acessoIncubatorios.Contains(d.Incubatorio) || d.Incubatorio == null
                            || d.Incubatorio == "")
                        && d.TipoDEO != "Solicitação Ajuste de Estoque" && d.TipoDEO != "Classificação de Ovos")
                    .GroupBy(h => new
                    {
                        h.DataHoraCarreg,
                        h.NFNum,
                        h.Granja,
                        h.TipoDEO,
                        h.GTANum,
                        h.Lacre,
                        h.Incubatorio,
                        h.NumIdentificacao
                    })
                    .OrderBy(j => j.Key.DataHoraCarreg)
                    .ToList();

            db.Database.ExecuteSqlCommand("delete from LayoutDiarioExpedicaoPais");
            db.SaveChanges();

            foreach (var item in lista)
            {
                LayoutDiarioExpedicaoPai pai = new LayoutDiarioExpedicaoPai();

                pai.DataHoraCarreg = item.Key.DataHoraCarreg;
                pai.NFNum = item.Key.NFNum;
                pai.Granja = item.Key.Granja;
                pai.TipoDEO = item.Key.TipoDEO;
                pai.GTANum = item.Key.GTANum;
                pai.Lacre = item.Key.Lacre;
                pai.IncubatorioDestino = item.Key.Incubatorio;
                pai.NumIdentificacao = item.Key.NumIdentificacao;

                db.DiarioExpedicaoPai.Add(pai);
            }

            db.SaveChanges();

            AtualizaGranjaSelecionada(Text);
            Session["granjaSelecionada"] = Text;

            return db.DiarioExpedicaoPai.ToList();
        }

        public List<LayoutDiarioExpedicaoPai> CarregarListaDEOConferido(string Text, DateTime dataInicial, 
            DateTime dataFinal)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            //string acessoPlanalto = "";
            //if (Session["empresa"].ToString().Contains("PL") && Session["empresa"].ToString().Count() == 2)
            //    acessoPlanalto = "NM";

            #region Carrega Acesso Incubatorios

            List<SelectListItem> listaIncubatoriosAcesso = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];

            string acessoIncubatorios = "";
            foreach (var item in listaIncubatoriosAcesso)
            {
                acessoIncubatorios = acessoIncubatorios + item.Value;
            }

            #endregion

            var lista = hlbapp.LayoutDiarioExpedicaos
                    .Where(d => d.Granja.Replace("PL", "NM") == Text && (d.Importado == "Conferido" || d.Importado == "Divergência")
                        && d.DataHoraCarreg >= dataInicial && d.DataHoraCarreg <= dataFinal
                        && (d.TipoDEO == "Ovos Incubáveis" || d.TipoDEO == "Transf. Ovos Incubáveis")
                        //&& (d.Incubatorio == acessoPlanalto || acessoPlanalto == "" || d.Incubatorio == null
                        && (acessoIncubatorios.Contains(d.Incubatorio) || d.Incubatorio == null
                            || d.Incubatorio == "")
                        && d.TipoDEO != "Solicitação Ajuste de Estoque" && d.TipoDEO != "Classificação de Ovos")
                    .GroupBy(h => new
                    {
                        h.DataHoraCarreg,
                        h.NFNum,
                        h.Granja,
                        h.TipoDEO,
                        h.GTANum,
                        h.Lacre,
                        h.Incubatorio,
                        h.NumIdentificacao
                    })
                    .OrderBy(j => j.Key.DataHoraCarreg)
                    .ToList();

            db.Database.ExecuteSqlCommand("delete from LayoutDiarioExpedicaoPais");
            db.SaveChanges();

            foreach (var item in lista)
            {
                LayoutDiarioExpedicaoPai pai = new LayoutDiarioExpedicaoPai();

                pai.DataHoraCarreg = item.Key.DataHoraCarreg;
                pai.NFNum = item.Key.NFNum;
                pai.Granja = item.Key.Granja;
                pai.TipoDEO = item.Key.TipoDEO;
                pai.GTANum = item.Key.GTANum;
                pai.Lacre = item.Key.Lacre;
                pai.IncubatorioDestino = item.Key.Incubatorio;
                pai.NumIdentificacao = item.Key.NumIdentificacao;

                db.DiarioExpedicaoPai.Add(pai);
            }

            db.SaveChanges();

            AtualizaGranjaSelecionada(Text);
            Session["granjaSelecionada"] = Text;

            return db.DiarioExpedicaoPai.ToList();
        }

        public ActionResult CarregarListaDEOFiltroView(string Text, DateTime dataInicial, 
            DateTime dataFinal, string TipoDEO)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    //Session["dataInicial"] = dataInicial.ToShortDateString();
                    //Session["dataFinal"] = dataFinal.ToShortDateString();

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        Session["dataInicial"] = dataInicial.ToShortDateString();
                        Session["dataFinal"] = dataFinal.ToShortDateString();
                        dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");
                    }

                    Session["granjaSelecionada"] = Text;
                    Session["isIncubatorio"] = IsIncubatorio(Text);
                    Session["ListaTiposDEOFiltro"] = CarregaListaTiposDEO(true);

                    if (Text.Equals("SB") || Text.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

                    return View("ListaDEOs", CarregarListaDEOFiltro(Text, dataInicial, dataFinal, TipoDEO));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult CarregarListaDEOFiltroConfView(string Text, string status, DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        Session["dataInicial"] = dataInicial.ToShortDateString();
                        Session["dataFinal"] = dataFinal.ToShortDateString();
                        dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");
                    }

                    if (Text.Equals("SB") || Text.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

                    return View("ListaConferenciaDEO", CarregarListaDEOStatus(Text, "Sim", dataInicial, dataFinal));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult CarregarListaDEOFiltroCancView(string Text, string status, DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        Session["dataInicial"] = dataInicial.ToShortDateString();
                        Session["dataFinal"] = dataFinal.ToShortDateString();
                        dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");
                    }

                    if (Text.Equals("SB") || Text.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

                    return View("ListaDEOConferidos", CarregarListaDEOStatus(Text, "Conferido", dataInicial, dataFinal));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public List<ImportaDiarioExpedicao> CarregarItensDEOImport(HLBAPPEntities hlbapp, DateTime dataFiltro, 
            string granja)
        {
            DateTime data = Convert.ToDateTime(dataFiltro.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture));
            DateTime data01 = data.AddMinutes(1);

            #region Consulta usada antes.
            //var lista = db.DiarioExpedicao
            //    .Where(d => d.DataHoraCarreg >= data && d.DataHoraCarreg <= data01 && d.Granja == granja)
            //    .GroupBy(g => new
            //    {
            //        g.Nucleo,
            //        g.Galpao,
            //        g.Lote,
            //        g.Idade,
            //        g.Linhagem,
            //        g.LoteCompleto,
            //        g.DataProducao,
            //        g.NumeroReferencia,
            //        g.Usuario,
            //        g.DataHoraCarreg,
            //        g.DataHoraRecebInc,
            //        g.ResponsavelCarreg,
            //        g.ResponsavelReceb,
            //        g.NFNum,
            //        g.Granja,
            //        g.Importado,
            //        g.Incubatorio,
            //        g.TipoDEO,
            //        g.GTANum,
            //        g.Lacre,
            //        g.NumIdentificacao,
            //    })
            //    .OrderBy(o => o.Key.Linhagem)
            //    .Select(s => new
            //    {
            //        s.Key.Nucleo,
            //        s.Key.Galpao,
            //        s.Key.Lote,
            //        s.Key.Idade,
            //        s.Key.Linhagem,
            //        s.Key.LoteCompleto,
            //        s.Key.DataProducao,
            //        s.Key.NumeroReferencia,
            //        s.Key.Usuario,
            //        s.Key.DataHoraCarreg,
            //        s.Key.DataHoraRecebInc,
            //        s.Key.ResponsavelCarreg,
            //        s.Key.ResponsavelReceb,
            //        s.Key.NFNum,
            //        s.Key.Granja,
            //        s.Key.Importado,
            //        s.Key.Incubatorio,
            //        s.Key.TipoDEO,
            //        s.Key.GTANum,
            //        s.Key.Lacre,
            //        s.Key.NumIdentificacao,
            //        QtdeOvos = s.Sum(u => u.QtdeOvos),
            //        QtdeBandejas = s.Sum(t => t.QtdeBandejas)
            //    })
            //    .ToList();

            //List<LayoutDiarioExpedicao> listaRetorno = new List<LayoutDiarioExpedicao>();

            //foreach (var item in lista)
            //{
            //    LayoutDiarioExpedicao deo = new LayoutDiarioExpedicao();

            //    deo.Nucleo = item.Nucleo;
            //    deo.Galpao = item.Galpao;
            //    deo.Lote = item.Lote;
            //    deo.Idade = item.Idade;
            //    deo.Linhagem = item.Linhagem;
            //    deo.LoteCompleto = item.LoteCompleto;
            //    deo.DataProducao = item.DataProducao;
            //    deo.NumeroReferencia = item.NumeroReferencia;
            //    deo.Usuario = item.Usuario;
            //    deo.DataHoraCarreg = item.DataHoraCarreg;
            //    deo.DataHoraRecebInc = item.DataHoraRecebInc;
            //    deo.ResponsavelCarreg = item.ResponsavelCarreg;
            //    deo.ResponsavelReceb = item.ResponsavelReceb;
            //    deo.NFNum = item.NFNum;
            //    deo.Granja = item.Granja;
            //    deo.Importado = item.Importado;
            //    deo.Incubatorio = item.Incubatorio;
            //    deo.TipoDEO = item.TipoDEO;
            //    deo.GTANum = item.GTANum;
            //    deo.Lacre = item.Lacre;
            //    deo.NumIdentificacao = item.NumIdentificacao;
            //    deo.QtdeOvos = item.QtdeOvos;
            //    deo.QtdeBandejas = item.QtdeBandejas;

            //    listaRetorno.Add(deo);
            //}

            //return listaRetorno.OrderBy(o => o.Linhagem).ToList();
            #endregion

            return hlbapp.ImportaDiarioExpedicao
                .Where(d => d.DataHoraCarreg >= data && d.DataHoraCarreg <= data01 && d.Granja == granja)
                //.Where(d => d.DataHoraCarreg == dataFiltro && d.Granja == granja)
                .OrderBy(o => o.Linhagem)
                .ToList();
        }

        public List<LayoutDiarioExpedicaos> CarregarItensDEO(HLBAPPEntities hlbapp, DateTime dataFiltro, 
            string granja, string status, string ordem, string origem)
        {
            DateTime data = Convert.ToDateTime(dataFiltro.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture));
            DateTime data01 = data.AddMinutes(1);

            if (origem == "Cadastro")
            {
                if (ordem == "Crescente")
                    return hlbapp.LayoutDiarioExpedicaos
                        .Where(d => d.DataHoraCarreg >= data && d.DataHoraCarreg <= data01 && d.Granja.Replace("PL","NM") == granja
                            && (d.Importado == status || status == ""))
                        .OrderBy(o => o.ID)
                        .ToList();
                else
                    return hlbapp.LayoutDiarioExpedicaos
                        .Where(d => d.DataHoraCarreg >= data && d.DataHoraCarreg <= data01 && d.Granja.Replace("PL", "NM") == granja
                            && (d.Importado == status || status == ""))
                        .OrderByDescending(o => o.ID)
                        .ToList();
            }
            else
            {
                if (ordem == "Crescente")
                    return hlbapp.LayoutDiarioExpedicaos
                        .Where(d => d.DataHoraCarreg == dataFiltro && d.Granja.Replace("PL", "NM") == granja
                            && (d.Importado == status || status == ""))
                        .OrderBy(o => o.ID)
                        .ToList();
                else
                    return hlbapp.LayoutDiarioExpedicaos
                        .Where(d => d.DataHoraCarreg == dataFiltro && d.Granja.Replace("PL", "NM") == granja
                            && (d.Importado == status || status == ""))
                        .OrderByDescending(o => o.ID)
                        .ToList();
            }
        }

        public List<LayoutDiarioExpedicaos> CarregarItensDEO(HLBAPPEntities hlbapp, string granja, string numIdentificacao, string status, string ordem)
        {
            if (ordem == "Crescente")
                return hlbapp.LayoutDiarioExpedicaos
                    .Where(d => d.NumIdentificacao == numIdentificacao && d.Granja.Replace("PL", "NM") == granja && (d.Importado == status || status == ""))
                    .OrderBy(o => o.ID)
                    .ToList();
            else
                return hlbapp.LayoutDiarioExpedicaos
                    .Where(d => d.NumIdentificacao == numIdentificacao && d.Granja.Replace("PL", "NM") == granja && (d.Importado == status || status == ""))
                    .OrderByDescending(o => o.ID)
                    .ToList();
        }

        #endregion        
        
        #region Métodos Conferência DEO

        public ActionResult ListaConferenciaDEO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    CarregaListaGranjas(true);
                    string granja = "";
                    if (Session["granjaSelecionada"] != null)
                        granja = Session["granjaSelecionada"].ToString();
                    AtualizaGranjaSelecionada(granja);

                    CarregaListaNucleos();
                    CarregaTipoOvo();
                    Session["dataProducaoSelecionada"] = DateTime.Today;
                    Session["loteCompletoSelecionado"] = "";

                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    if (granja.Equals("SB") || granja.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

                    return View("ListaConferenciaDEO", CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult ListaDEOConferidos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    CarregaListaGranjas(true);
                    string granja = "";
                    if (Session["granjaSelecionada"] != null)
                        granja = Session["granjaSelecionada"].ToString();
                    AtualizaGranjaSelecionada(granja);

                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        Session["dataInicial"] = DateTime.Today.ToShortDateString();
                        Session["dataFinal"] = DateTime.Today.ToShortDateString();
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    if (granja.Equals("SB") || granja.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";

                    //CarregaListaIncubatorios();
                    Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

                    return View("ListaDEOConferidos", CarregarListaDEOConferido(granja, dataInicial, dataFinal));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult ListaItensDEO(DateTime dataFiltro, string nfNum, string tipoDEO, string origem, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string granja = Session["granjaSelecionada"].ToString();
            Session["dataHoraCarreg"] = dataFiltro;
            Session["nfNum"] = nfNum;
            Session["tipoDEOselecionado"] = tipoDEO;
            Session["dataRecebInc"] = "";
            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            var listaItens = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
            if (listaItens.Count == 0)
                listaItens = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cadastro");
            var incubatorioDestino = listaItens.FirstOrDefault().Incubatorio;

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorioDestino))
            {
                DateTime dataInicial;
                DateTime dataFinal;

                if (Session["dataInicial"] == null)
                {
                    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                }
                else
                {
                    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                }

                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                var listaRetorno = CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal);
                return View("ListaConferenciaDEO", listaRetorno);
            }

            Session["operacao"] = "Edit";

            Session["incubatorioDestinoSelecionado"] = incubatorioDestino;

            #region Carrega Sessions Valores

            var listaAgrupada = listaItens
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao
                })
                .OrderBy(o => o.Key.LoteCompleto)
                .ThenBy(t => t.Key.DataProducao)
                .Select(s => new
                {
                    Lote = s.Key.LoteCompleto,
                    Data = s.Key.DataProducao,
                    Status = s.Max(m => m.Importado),
                    QtdOvos = s.Sum(m => m.QtdeOvos),
                    QtdDif = s.Sum(m => (m.QtdDiferenca == null ? 0 : m.QtdDiferenca))
                })
                .ToList();

            foreach (var item in listaAgrupada)
            {
                if (origem == "Inicio")
                {
                    Session["qtdDiferenca_"+ item.Lote.ToString() + "|" + item.Data.ToShortDateString()] = "";
                    Session["temperaturaInternaOvo_" + item.Lote.ToString() + "|" + item.Data.ToShortDateString()] = "";
                    Session["temperaturaInternaOvoMeio_" + item.Lote.ToString() + "|" + item.Data.ToShortDateString()] = "";
                    Session["temperaturaInternaOvoFim_" + item.Lote.ToString() + "|" + item.Data.ToShortDateString()] = "";
                    Session["obs_" + item.Lote.ToString() + "|" + item.Data.ToShortDateString()] = "";
                }
            }

            #endregion

            return View("ItemConfereDEO", listaItens);
        }

        public ActionResult CancelaConfDEO(DateTime dataFiltro, string tipoDEO, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            bdApolo.CommandTimeout = 60;
            apoloService.CommandTimeout = 1000;

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            string granja = Session["granjaSelecionada"].ToString();

            ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

            var lista = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
            if (lista.Count == 0)
                lista = CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente", "Cancelamento Conferência");

            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

            try
            {
                //dataFiltro = Convert.ToDateTime("01/01/1899 12:00");

                if (!ExisteFechamentoEstoque(dataFiltro, granja))
                {
                    string login = Session["login"].ToString();

                    string usuario = login.ToUpper();

                    string dataIdentific = dataFiltro.ToShortDateString();

                    //MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                    //    .Where(m => m.USERFLIPDataCarregDEO == dataIdentific
                    //    && apoloService.EMPRESA_FILIAL.Any(e => e.EmpCod == m.EmpCod && e.USERFLIPCod == incubatorio))
                    //    .FirstOrDefault();

                    //if (movEstq != null)
                    //{
                    //    //rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    //    //apoloService.delete_movestq(movEstq.EmpCod, movEstq.MovEstqChv, usuario, rmensagem);

                    //    DeletaMovEstq(movEstq);
                    //}

                    LayoutDiarioExpedicaos deo = lista.FirstOrDefault();

                    //var listamovEstq = apoloService.MOV_ESTQ
                    //    .Where(m => m.MovEstqDocEspec == "TLA" && m.MovEstqDocNum == deo.NumIdentificacao
                    //    && apoloService.EMPRESA_FILIAL.Any(e => e.EmpCod == m.EmpCod && e.USERFLIPCod == incubatorio))
                    //    .ToList();

                    //foreach (var item in listamovEstq)
                    //{
                    //    //rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    //    //apoloService.delete_movestq(item.EmpCod, item.MovEstqChv, usuario, rmensagem);

                    //    //if (rmensagem.Value.ToString() != "Não")
                    //    //{
                    //    //    ViewBag.Mensagem = "Erro ao deletar estoque :" + rmensagem.Value + " Verifique se não Saídas ou Transferências posteriores a data!";
                    //    //    return View("ListaDEOConferidos", CarregarListaDEOStatus(granja, "Conferido", dataInicial, dataFinal));
                    //    //}

                    //    DeletaMovEstq(item);
                    //}

                    var incubatorioDestino = lista.FirstOrDefault().Incubatorio;
                    if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorioDestino))
                    {
                        ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                            + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                        var listaRetorno = CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal);
                        return View("ListaConferenciaDEO", listaRetorno);
                    }

                    foreach (var item in lista)
                    {
                        if (item.QtdeOvos > 0)
                        {
                            int qtdSaldo = Convert.ToInt32(item.QtdeOvos) +
                                Convert.ToInt32(item.QtdDiferenca);

                            if ((ExisteSaldo(item.Incubatorio, item.DataHoraCarreg, item.LoteCompleto,
                                item.DataProducao, qtdSaldo)
                                && item.Importado == "Conferido")
                                || item.Importado == "Divergência")
                            {
                                item.Importado = "Sim";
                                item.QtdeConferencia = 0;
                                item.QtdDiferenca = 0;
                            }
                        }
                        else
                        {
                            LayoutDiarioExpedicaos deoDelete = hlbapp.LayoutDiarioExpedicaos
                                .Where(w => w.ID == item.ID).FirstOrDefault();

                            hlbapp.LayoutDiarioExpedicaos.DeleteObject(deoDelete);
                        }

                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, 
                            "Cancelamento Conferência", usuario, 0, "", "", item);

                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }

                    //foreach (var item in lista2)
                    //{
                    //    item.Importado = "Sim";
                    //    //incubatorio = item.Incubatorio;
                    //}

                    hlbapp.SaveChanges();
                    hlbappLOG.SaveChanges();
                    //db.SaveChanges();
                    //apoloService.SaveChanges();

                    ViewBag.Mensagem = "Cancelamento do DEO " + dataFiltro.ToString() + " da granja " + granja + " realizado com sucesso!";
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                }

                return View("ListaDEOConferidos", CarregarListaDEOStatus(granja, "Conferido", dataInicial, dataFinal));
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "Erro na rotina de integração de Estoque do Apolo: " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                    + "Segundo erro: " + ex.InnerException.Message
                    + "Erro Retorno Procedure: " + rmensagem.Value;

                ViewBag.Erro = retorno;

                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, lista);
                //hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, lista2);

                return View("ListaDEOConferidos", CarregarListaDEOStatus(granja, "Conferido", dataInicial, dataFinal));

                #endregion
            }
        }

        [HttpPost]
        public ActionResult ConfereDEO(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    //bdApolo.CommandTimeout = 1000;
                    //apoloService.CommandTimeout = 1000;

                    DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
                    string granja = Session["granjaSelecionada"].ToString();
                    string tipoDEO = Session["tipoDEOselecionado"].ToString();
                    string numIdentificacao = "Sem ID";
                    if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                    var listaDEO = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (listaDEO.Count == 0)
                        listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", "Conferência");

                    string empresaEstoque = "";
                    string empresaContra = "";
                    if (granja.Equals("PL") && (listaDEO.Where(w => w.Linhagem.Contains("DKB")).Count() > 0))
                    {
                        empresaEstoque = "PL";
                        empresaContra = "20";
                    }
                    else
                    {
                        empresaEstoque = "CH";
                        empresaContra = "1";
                    }

                    try
                    {
                        var fileIds = ("," + model["id"]).Split(',');

                        var selectedIndices = model["importa"].Replace("true,false", "true")
                                    .Split(',')
                                    .Select((item, index) => new { item = item, index = index })
                                    .Where(row => row.item == "true")
                                    .Select(row => row.index).ToArray();

                        if (!ExisteFechamentoEstoque(dataCarreg, granja))
                        {
                            //if (listaDEO.Count == selectedIndices.Count())
                            //{
                                //var listaQtdFalta = ("," + model["qtdFalta"]).Split(',');

                                //MOV_ESTQ movEstq = new MOV_ESTQ();
                                //ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
                                //LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = new LOC_ARMAZ_ITEM_MOV_ESTQ();
                                //CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = new CTRL_LOTE_ITEM_MOV_ESTQ();

                                string login = Session["login"].ToString();

                                string usuario;
                                if (login.Equals("palves"))
                                    usuario = "RIOSOFT";
                                else
                                    usuario = login.ToUpper();

                                DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                                //MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empGranja = bdApolo.EMPRESA_FILIAL
                                //                .Where(e => e.USERFLIPCod == granja)
                                //                .FirstOrDefault();

                                string nfnum = Session["nfNum"].ToString();
                                //string vEmpresaGranja = "";

                                HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                                #region **** NÃO INTEGRA APOLO ****

                                /*
                                #region Desativada Rotina de DEO Granja vinculada na NF
                                //// Verifica se é Terceiro
                                //if (empGranja != null)
                                //{
                                //    // Se existir empresa, não é terceiro
                                //    #region Chama a Baixa Estoque NF para lançar Estoque no Incubatório

                                //    vEmpresaGranja = empGranja.EmpCod;

                                //    ObjectParameter numero = new ObjectParameter("numero", typeof(global::System.String)); ;

                                //    apoloService.CONCAT_ZERO_ESQUERDA(nfnum, 10, numero);

                                //    nfnum = numero.Value.ToString();

                                //    string especie = "NF-e";
                                //    string serie = "001";

                                //    nucleos.FillFarmsAllLocation(flip.FLOCKS1);

                                //    string location = flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;

                                //    string localContra = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().LocArmazCodEstr;

                                //    string incubatorio = apoloService.LOC_ARMAZ.Where(l => l.USERGeracaoFLIP == location && l.USERTipoProduto == tipoDEO).FirstOrDefault().USERCodigoFLIP;

                                //    string natOper = "";

                                //    if (granja.Equals("CG"))
                                //        natOper = "2.151";
                                //    else
                                //        natOper = "1.151";

                                //    LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                                //        .Where(l => l.USERCodigoFLIP == granja)
                                //        .FirstOrDefault();

                                //    string local = locArmaz.LocArmazCodEstr;

                                //    NOTA_FISCAL notaFiscal = apoloService.NOTA_FISCAL
                                //        .Where(p => p.CtrlDFModForm == especie && p.CtrlDFSerie == serie && p.NFNum == nfnum &&
                                //            p.EmpCod == empGranja.EmpCod)
                                //        .FirstOrDefault();

                                //    ObjectParameter vMensagem = new ObjectParameter("vMensagem", typeof(global::System.String));

                                //    apoloService.BAIXA_ESTOQUE_NF(notaFiscal.NFNum, especie, serie, empGranja.EmpCod, notaFiscal.NFDataEmis,
                                //        local, empresaContra, natOper, localContra, usuario, vMensagem);

                                //    if (!vMensagem.Value.Equals(""))
                                //    {
                                //        string assuntoEmail = "**** ERRO AO INTEGRAR DEO COM APOLO ****";
                                //        string paraNome = "T.I.";
                                //        string paraEmail = "ti@hyline.com.br";
                                //        string copiaPara = "";
                                //        string anexo = "";

                                //        string retorno = "Erro ao realizar integração com o Estoque do Apolo:" + (char)10 + (char)13
                                //                + "Erro na Transferência da NF " + notaFiscal.NFNum + " da empresa " + empGranja.EmpCod + "." + (char)10 + (char)13
                                //                + "Erro gerado pelo retorno da procedure: " + vMensagem.Value;

                                //        string retornoView = "Erro ao realizar integração com o Estoque do Apolo: <br/> "
                                //                + "Erro na Transferência da NF " + notaFiscal.NFNum + " da empresa " + empGranja.EmpCod + ". <br/> "
                                //                + "Erro gerado pelo retorno da procedure: " + vMensagem.Value
                                //                + " <br/> Entrar em contato com o Paulo da T.I. pelo (17) 9 9771-7538.";

                                //        ViewBag.Erro = retornoView;

                                //        retorno = retorno + (char)10 + (char)13 + "Usuário: " + usuario + (char)10 + (char)13
                                //                    + "Granja: " + granja + (char)10 + (char)13
                                //                    + "Data do DEO: " + dataCarreg.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture);

                                //        EnviarEmail(retorno, assuntoEmail, paraNome, paraEmail, copiaPara, anexo);

                                //        return View("ItemConfereDEO", listaDEO);
                                //    }

                                //    #endregion
                                //}
                                //else
                                //{
                                #endregion
                                // Caso não existe, é terceiro.
                                #region Integra a transferência com o Estoque

                                LayoutDiarioExpedicao itemDeo = db.DiarioExpedicao
                                    .Where(d => d.Granja == granja && d.DataHoraCarreg == dataCarreg)
                                    .FirstOrDefault();

                                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                    .Where(e => e.USERFLIPCod == empresaEstoque)
                                    .FirstOrDefault();

                                vEmpresaGranja = empresaFilial.EmpCod;

                                int nfNumTransf = Convert.ToInt32(itemDeo.NumIdentificacao);

                                TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = apoloService.TRANSF_ESTQ_LOC_ARMAZ
                                    .Where(t => t.EmpCod == empresaFilial.EmpCod && t.TransfEstqLocArmazNum == nfNumTransf)
                                    .FirstOrDefault();

                                if (transfEstqLocArmaz == null)
                                {
                                    #region Tratamento de Erro - Não Integrado com o Apolo

                                    string retorno = "";
                                    //string retornoVB = "";

                                    retorno = "DEO não integrado corretamente com o Apolo! Por favor, entrar no DEO e salvar ele novamente!";

                                    ViewBag.Erro = retorno;

                                    return View("ItemConfereDEO", listaDEO);

                                    #endregion
                                }
                                
                                apoloService.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                    usuario);

                                #endregion
                                //}

                                foreach (var index in selectedIndices)
                                {
                                    int fileId;
                                    if (int.TryParse(fileIds[index], out fileId))
                                    {
                                        int qtdFalta;
                                        int.TryParse(listaQtdFalta[index], out qtdFalta);

                                        LayoutDiarioExpedicao itemDEO = db.DiarioExpedicao
                                            .Where(d => d.ID == fileId)
                                            .FirstOrDefault();

                                        #region Atualiza Dados do Lote de Destino

                                        //MvcAppHylinedoBrasilMobile.Models.bdApolo.CTRL_LOTE loteEmpresaOrigem =
                                        //    bdApolo.CTRL_LOTE.Where(c => c.EmpCod == vEmpresaGranja
                                        //        && c.CtrlLoteNum == itemDEO.LoteCompleto
                                        //        && c.CtrlLoteDataValid == itemDEO.DataProducao)
                                        //    .FirstOrDefault();

                                        MvcAppHylinedoBrasilMobile.Models.bdApolo.CTRL_LOTE loteEmpresaDestino =
                                            bdApolo.CTRL_LOTE.Where(c => c.EmpCod == empresaContra
                                                && c.CtrlLoteNum == itemDEO.LoteCompleto
                                                && c.CtrlLoteDataValid == itemDEO.DataProducao)
                                            .FirstOrDefault();

                                        //loteEmpresaDestino.USERGranjaNucleoFLIP = loteEmpresaOrigem.USERGranjaNucleoFLIP;
                                        //loteEmpresaDestino.USERIdateLoteFLIP = loteEmpresaOrigem.USERIdateLoteFLIP;
                                        //loteEmpresaDestino.USERPercMediaIncUlt4SemFLIP = loteEmpresaOrigem.USERPercMediaIncUlt4SemFLIP;

                                        loteEmpresaDestino.USERGranjaNucleoFLIP = itemDEO.Nucleo;
                                        loteEmpresaDestino.USERIdateLoteFLIP = (short)itemDEO.Idade;

                                        ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                        string flockIDHatch = itemDEO.Nucleo + "-" + itemDEO.LoteCompleto;

                                        loteEmpresaDestino.USERPercMediaIncUlt4SemFLIP = service.AVG_LST4WK_HATCH("HYBR", flockIDHatch);

                                        #endregion

                                        #region Gera Entrada de Itens Conferidos

                                        //string natOpCodEstr = "1.949";
                                        //decimal? valorUnitario = 0.25m;
                                        //string unidadeMedida = "UN";
                                        //short? posicaoUnidadeMedida = 1;
                                        //string tribCod = "040";
                                        //string itMovEstqClasFiscCodNbm = "04079000";
                                        //string clasFiscCod = "0000129";
                                        //string operacao = "Entrada";

                                        //DateTime dataMov = Convert.ToDateTime(item.DataHoraCarreg.ToShortDateString());

                                        //LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                        //        .Where(l => l.USERCodigoFLIP == item.Incubatorio && l.USERTipoProduto == tipoDEO)
                                        //        .FirstOrDefault();

                                        //string tipoLanc = localArmazCadastro.USERTipoLancSaidaAjuste;

                                        //string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                        //#region Inseria lote para cada item. Como terá repetido, terá que localizar e atualizar existente.
                                        ////if (dataAnterior != itemDEO.DataHoraCarreg)
                                        ////{
                                        ////    movEstq = service.InsereMovEstq(empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov, usuario);
                                        ////    movEstq.USERFLIPDataCarregDEO = itemDEO.DataHoraCarreg;
                                        ////    apoloService.MOV_ESTQ.AddObject(movEstq);
                                        ////}

                                        ////if (linhagemAnterior != itemDEO.Linhagem)
                                        ////{
                                        ////    if (!linhagemAnterior.Equals(""))
                                        ////    {
                                        ////        itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                        ////        itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                        ////        itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                        ////        localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                        ////        localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                        ////        qtdTotalItem = 0;

                                        ////        apoloService.SaveChanges();

                                        ////        apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");
                                        ////    }

                                        ////    itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov,
                                        ////        itemDEO.Linhagem, natOpCodEstr, qtdFalta, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                                        ////        tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                                        ////    apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        ////    localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa.EmpCod, itemMovEstq.ItMovEstqSeq,
                                        ////        itemMovEstq.ProdCodEstr, qtdFalta, localArmazenagem);

                                        ////    apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);
                                        ////}

                                        ////lote = service.InsereLote(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        ////        itemDEO.LoteCompleto, itemDEO.DataProducao, qtdFalta, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        ////apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);

                                        ////qtdTotalItem = qtdTotalItem + qtdFalta;

                                        ////if ((selectedIndices.Max() == (index)))
                                        ////{
                                        ////    itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                        ////    itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                        ////    itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                        ////    localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                        ////    localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                        ////    qtdTotalItem = 0;

                                        ////    apoloService.SaveChanges();

                                        ////    apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");

                                        ////    apoloService.calcula_mov_estq(empresa.EmpCod, movEstq.MovEstqChv);
                                        ////}
                                        //#endregion

                                        //string dataIdentific = item.DataHoraCarreg.ToShortDateString();

                                        //#region Carrega Lote
                                        //loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                        //    .Where(l => l.CtrlLoteNum == item.LoteCompleto && l.CtrlLoteDataValid == item.DataProducao
                                        //        && l.EmpCod == empresaFilial.EmpCod
                                        //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                        //            && m.USERFLIPDataCarregDEO == dataIdentific && m.TipoLancCod == tipoLanc))
                                        //    .FirstOrDefault();

                                        //#endregion

                                        //if (loteItemMovEstq == null)
                                        //{
                                        //    #region Carrega Produto

                                        //    string replace = "" + (char)13 + (char)10;

                                        //    ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                        //    string dataIdentificacao = item.DataHoraCarreg.ToShortDateString();

                                        //    itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        //        && im.ProdCodEstr == produto.ProdCodEstr
                                        //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        //            && m.USERFLIPDataCarregDEO == dataIdentificacao && m.TipoLancCod == tipoLanc))
                                        //    .FirstOrDefault();
                                        //    #endregion

                                        //    if (itemMovEstq == null)
                                        //    {
                                        //        #region Carrega Movimentação. Se não existe, insere.

                                        //        string dataCarregDEOApolo = item.DataHoraCarreg.ToShortDateString();

                                        //        movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                        //            && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc)
                                        //        .FirstOrDefault();

                                        //        if (movEstq == null)
                                        //        {
                                        //            movEstq = service.InsereMovEstq(empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod, dataMov, usuario);

                                        //            movEstq.USERFLIPDataCarregDEO = dataCarregDEOApolo;

                                        //            apoloService.MOV_ESTQ.AddObject(movEstq);

                                        //            apoloService.SaveChanges();
                                        //        }
                                        //        #endregion

                                        //        #region Se Item não existe, insere item, local e lote
                                        //        itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod,
                                        //            dataMov, item.Linhagem, natOpCodEstr,
                                        //            qtdInserir, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                        //            clasFiscCod);

                                        //        apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        //        localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresaFilial.EmpCod, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        //            qtdInserir, localArmazenagem);

                                        //        apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                        //        loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                        //            item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //        apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //        apoloService.SaveChanges();

                                        //        //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //        //    dataMov, "INS");

                                        //        apoloService.calcula_mov_estq(empresaFilial.EmpCod, movEstq.MovEstqChv);
                                        //        #endregion
                                        //    }
                                        //    else
                                        //    {
                                        //        #region Se existe Item, insere lote e atuliza item e local

                                        //        loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                        //            item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //        apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //        itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                        //        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        //        localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        //            && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        //            && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        //            .FirstOrDefault();

                                        //        localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                        //        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        //        apoloService.SaveChanges();

                                        //        //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //        //    dataMov, "UPD");

                                        //        apoloService.calcula_mov_estq(empresaFilial.EmpCod, itemMovEstq.MovEstqChv);

                                        //        #endregion
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    #region Se existe o lote e a quantidade é diferente, atualiza a quantidade

                                        //    loteItemMovEstq.CtrlLoteItMovEstqQtd = loteItemMovEstq.CtrlLoteItMovEstqQtd + qtdInserir;
                                        //    loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                        //    ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                        //    string dataCarregDEOApolo = item.DataHoraCarreg.ToShortDateString();

                                        //    itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        //        && im.ProdCodEstr == produto.ProdCodEstr
                                        //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        //            && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc))
                                        //    .FirstOrDefault();

                                        //    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                        //    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        //    localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        //        && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        //        && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        //        .FirstOrDefault();

                                        //    localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                        //    localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        //    apoloService.SaveChanges();

                                        //    //apoloService.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //    //    dataMov, "UPD");

                                        //    apoloService.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

                                        //    #endregion
                                        //}

                                        #endregion

                                        if (qtdFalta > 0)
                                        {
                                            var listaImportaFalta = hlbapp.ImportaDiarioExpedicao
                                                .Where(i => hlbapp.LayoutDEO_X_ImportaDEO
                                                    .Any(x => x.CodItemImportaDEO == i.CodItemImportaDEO && x.CodItemDEO == itemDEO.CodItemDEO))
                                                .OrderBy(o => o.DataProducao)
                                                .ToList();

                                            int qtdInserir = 0;

                                            foreach (var item in listaImportaFalta)
                                            {
                                                if (qtdFalta > item.QtdeOvos)
                                                {
                                                    qtdInserir = Convert.ToInt32(item.QtdeOvos);

                                                    string natOpCodEstr = "5.101";
                                                    decimal? valorUnitario = 0.25m;
                                                    string unidadeMedida = "UN";
                                                    short? posicaoUnidadeMedida = 1;
                                                    string tribCod = "040";
                                                    string itMovEstqClasFiscCodNbm = "04079000";
                                                    string clasFiscCod = "0000129";
                                                    string operacao = "Saída";

                                                    DateTime dataMov = Convert.ToDateTime(item.DataHoraCarreg.ToShortDateString());

                                                    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                                            .Where(l => l.USERCodigoFLIP == item.Incubatorio && l.USERTipoProduto == tipoDEO)
                                                            .FirstOrDefault();

                                                    string tipoLanc = localArmazCadastro.USERTipoLancSaidaAjuste;

                                                    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                                    #region Inseria lote para cada item. Como terá repetido, terá que localizar e atualizar existente.
                                                    //if (dataAnterior != itemDEO.DataHoraCarreg)
                                                    //{
                                                    //    movEstq = service.InsereMovEstq(empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov, usuario);
                                                    //    movEstq.USERFLIPDataCarregDEO = itemDEO.DataHoraCarreg;
                                                    //    apoloService.MOV_ESTQ.AddObject(movEstq);
                                                    //}

                                                    //if (linhagemAnterior != itemDEO.Linhagem)
                                                    //{
                                                    //    if (!linhagemAnterior.Equals(""))
                                                    //    {
                                                    //        itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                                    //        itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                                    //        itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                                    //        localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                                    //        localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                                    //        qtdTotalItem = 0;

                                                    //        apoloService.SaveChanges();

                                                    //        apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");
                                                    //    }

                                                    //    itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov,
                                                    //        itemDEO.Linhagem, natOpCodEstr, qtdFalta, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                                                    //        tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                                                    //    apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                    //    localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa.EmpCod, itemMovEstq.ItMovEstqSeq,
                                                    //        itemMovEstq.ProdCodEstr, qtdFalta, localArmazenagem);

                                                    //    apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);
                                                    //}

                                                    //lote = service.InsereLote(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                                    //        itemDEO.LoteCompleto, itemDEO.DataProducao, qtdFalta, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                    //apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);

                                                    //qtdTotalItem = qtdTotalItem + qtdFalta;

                                                    //if ((selectedIndices.Max() == (index)))
                                                    //{
                                                    //    itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                                    //    itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                                    //    itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                                    //    localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                                    //    localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                                    //    qtdTotalItem = 0;

                                                    //    apoloService.SaveChanges();

                                                    //    apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");

                                                    //    apoloService.calcula_mov_estq(empresa.EmpCod, movEstq.MovEstqChv);
                                                    //}
                                                    #endregion

                                                    string dataIdentific = item.DataHoraCarreg.ToShortDateString();

                                                    #region Carrega Lote
                                                    loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                                        .Where(l => l.CtrlLoteNum == item.LoteCompleto && l.CtrlLoteDataValid == item.DataProducao
                                                            && l.EmpCod == empresaFilial.EmpCod
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataIdentific && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();

                                                    #endregion

                                                    if (loteItemMovEstq == null)
                                                    {
                                                        #region Carrega Produto

                                                        string replace = "" + (char)13 + (char)10;

                                                        ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                                        string dataIdentificacao = item.DataHoraCarreg.ToShortDateString();

                                                        itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                                            && im.ProdCodEstr == produto.ProdCodEstr
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataIdentificacao && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();
                                                        #endregion

                                                        if (itemMovEstq == null)
                                                        {
                                                            #region Carrega Movimentação. Se não existe, insere.

                                                            string dataCarregDEOApolo = item.DataHoraCarreg.ToShortDateString();

                                                            movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc)
                                                            .FirstOrDefault();

                                                            if (movEstq == null)
                                                            {
                                                                movEstq = service.InsereMovEstq(empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod, dataMov, usuario);

                                                                movEstq.USERFLIPDataCarregDEO = dataCarregDEOApolo;

                                                                apoloService.MOV_ESTQ.AddObject(movEstq);

                                                                apoloService.SaveChanges();
                                                            }
                                                            #endregion

                                                            #region Se Item não existe, insere item, local e lote
                                                            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod,
                                                                dataMov, item.Linhagem, natOpCodEstr,
                                                                qtdInserir, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                                                clasFiscCod);

                                                            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                            localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresaFilial.EmpCod, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                                                qtdInserir, localArmazenagem);

                                                            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                                            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                                                item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                                            apoloService.SaveChanges();

                                                            //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                            //    dataMov, "INS");

                                                            apoloService.calcula_mov_estq(empresaFilial.EmpCod, movEstq.MovEstqChv);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            #region Se existe Item, insere lote e atuliza item e local

                                                            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                                                item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                                            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                                            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                                            localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                                                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                                                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                                                .FirstOrDefault();

                                                            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                                            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                                            apoloService.SaveChanges();

                                                            //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                            //    dataMov, "UPD");

                                                            apoloService.calcula_mov_estq(empresaFilial.EmpCod, itemMovEstq.MovEstqChv);

                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        #region Se existe o lote e a quantidade é diferente, atualiza a quantidade

                                                        loteItemMovEstq.CtrlLoteItMovEstqQtd = loteItemMovEstq.CtrlLoteItMovEstqQtd + qtdInserir;
                                                        loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                                        ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                                        string dataCarregDEOApolo = item.DataHoraCarreg.ToShortDateString();

                                                        itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                                            && im.ProdCodEstr == produto.ProdCodEstr
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();

                                                        itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                                        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                                        localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                                            && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                                            && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                                            .FirstOrDefault();

                                                        localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                                        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                                        apoloService.SaveChanges();

                                                        //apoloService.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                        //    dataMov, "UPD");

                                                        apoloService.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

                                                        #endregion
                                                    }
                                                }
                                                else
                                                {
                                                    qtdInserir = qtdFalta;

                                                    string natOpCodEstr = "5.101";
                                                    decimal? valorUnitario = 0.25m;
                                                    string unidadeMedida = "UN";
                                                    short? posicaoUnidadeMedida = 1;
                                                    string tribCod = "040";
                                                    string itMovEstqClasFiscCodNbm = "04079000";
                                                    string clasFiscCod = "0000129";
                                                    string operacao = "Saída";

                                                    DateTime dataMov = Convert.ToDateTime(item.DataHoraCarreg.ToShortDateString());

                                                    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                                            .Where(l => l.USERCodigoFLIP == item.Incubatorio && l.USERTipoProduto == tipoDEO)
                                                            .FirstOrDefault();

                                                    string tipoLanc = localArmazCadastro.USERTipoLancSaidaAjuste;

                                                    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                                    #region Inseria lote para cada item. Como terá repetido, terá que localizar e atualizar existente.
                                                    //if (dataAnterior != itemDEO.DataHoraCarreg)
                                                    //{
                                                    //    movEstq = service.InsereMovEstq(empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov, usuario);
                                                    //    movEstq.USERFLIPDataCarregDEO = itemDEO.DataHoraCarreg;
                                                    //    apoloService.MOV_ESTQ.AddObject(movEstq);
                                                    //}

                                                    //if (linhagemAnterior != itemDEO.Linhagem)
                                                    //{
                                                    //    if (!linhagemAnterior.Equals(""))
                                                    //    {
                                                    //        itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                                    //        itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                                    //        itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                                    //        localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                                    //        localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                                    //        qtdTotalItem = 0;

                                                    //        apoloService.SaveChanges();

                                                    //        apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");
                                                    //    }

                                                    //    itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, empresa.EntCod, dataMov,
                                                    //        itemDEO.Linhagem, natOpCodEstr, qtdFalta, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                                                    //        tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                                                    //    apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                    //    localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa.EmpCod, itemMovEstq.ItMovEstqSeq,
                                                    //        itemMovEstq.ProdCodEstr, qtdFalta, localArmazenagem);

                                                    //    apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);
                                                    //}

                                                    //lote = service.InsereLote(movEstq.MovEstqChv, empresa.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                                    //        itemDEO.LoteCompleto, itemDEO.DataProducao, qtdFalta, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                    //apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);

                                                    //qtdTotalItem = qtdTotalItem + qtdFalta;

                                                    //if ((selectedIndices.Max() == (index)))
                                                    //{
                                                    //    itemMovEstq.ItMovEstqQtdProd = qtdTotalItem;
                                                    //    itemMovEstq.ItMovEstqValProd = valorUnitario * qtdTotalItem;
                                                    //    itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;

                                                    //    localArmaz.LocArmazItMovEstqQtd = qtdTotalItem;
                                                    //    localArmaz.LocArmazItMovEstqQtdCalc = qtdTotalItem;

                                                    //    qtdTotalItem = 0;

                                                    //    apoloService.SaveChanges();

                                                    //    apoloService.atualiza_saldoestqdata(empresa.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq, dataMov, "INS");

                                                    //    apoloService.calcula_mov_estq(empresa.EmpCod, movEstq.MovEstqChv);
                                                    //}
                                                    #endregion

                                                    #region Carrega Lote

                                                    string dataCarregDEOApolo = item.DataHoraCarreg.ToShortDateString();

                                                    loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                                        .Where(l => l.CtrlLoteNum == item.LoteCompleto && l.CtrlLoteDataValid == item.DataProducao
                                                            && l.EmpCod == empresaFilial.EmpCod
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();

                                                    #endregion

                                                    if (loteItemMovEstq == null)
                                                    {
                                                        #region Carrega Produto

                                                        string replace = "" + (char)13 + (char)10;

                                                        ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                                        itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                                            && im.ProdCodEstr == produto.ProdCodEstr
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();
                                                        #endregion

                                                        if (itemMovEstq == null)
                                                        {
                                                            #region Carrega Movimentação. Se não existe, insere.
                                                            movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc)
                                                            .FirstOrDefault();

                                                            if (movEstq == null)
                                                            {
                                                                movEstq = service.InsereMovEstq(empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod, dataMov, usuario);

                                                                movEstq.USERFLIPDataCarregDEO = dataCarregDEOApolo;

                                                                apoloService.MOV_ESTQ.AddObject(movEstq);

                                                                apoloService.SaveChanges();
                                                            }
                                                            #endregion

                                                            #region Se Item não existe, insere item, local e lote
                                                            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, empresaFilial.EntCod,
                                                                dataMov, item.Linhagem, natOpCodEstr,
                                                                qtdInserir, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod, itMovEstqClasFiscCodNbm,
                                                                clasFiscCod);

                                                            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                            localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresaFilial.EmpCod, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                                                qtdInserir, localArmazenagem);

                                                            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                                            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                                                item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                                            apoloService.SaveChanges();

                                                            //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                            //    dataMov, "INS");

                                                            apoloService.calcula_mov_estq(empresaFilial.EmpCod, movEstq.MovEstqChv);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            #region Se existe Item, insere lote e atuliza item e local

                                                            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresaFilial.EmpCod, tipoLanc, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, item.LoteCompleto,
                                                                item.DataProducao, qtdInserir, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                                            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                                            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                                            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                                            localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                                                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                                                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                                                .FirstOrDefault();

                                                            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                                            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                                            apoloService.SaveChanges();

                                                            //apoloService.atualiza_saldoestqdata(empresaFilial.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                            //    dataMov, "UPD");

                                                            apoloService.calcula_mov_estq(empresaFilial.EmpCod, itemMovEstq.MovEstqChv);

                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        #region Se existe o lote e a quantidade é diferente, atualiza a quantidade

                                                        loteItemMovEstq.CtrlLoteItMovEstqQtd = loteItemMovEstq.CtrlLoteItMovEstqQtd + qtdInserir;
                                                        loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                                        ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == item.Linhagem).FirstOrDefault();

                                                        itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                                            && im.ProdCodEstr == produto.ProdCodEstr
                                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                                                && m.USERFLIPDataCarregDEO == dataCarregDEOApolo && m.TipoLancCod == tipoLanc))
                                                        .FirstOrDefault();

                                                        itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtdInserir;
                                                        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                                        localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                                            && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                                            && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                                            .FirstOrDefault();

                                                        localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtdInserir;
                                                        localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                                        apoloService.SaveChanges();

                                                        //apoloService.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                                        //    dataMov, "UPD");

                                                        apoloService.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

                                                        #endregion
                                                    }

                                                    break;
                                                }

                                                qtdFalta = qtdFalta - qtdInserir;
                                            }
                                        }

                                        itemDEO.Importado = "Conferido";

                                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, "Conferência", usuario, qtdFalta, itemDEO));
                                        //hlbapp.SaveChanges();
                                    }
                                }
                                */

                                #endregion

                                foreach (var index in selectedIndices)
                                {
                                    int fileId;
                                    if (int.TryParse(fileIds[index], out fileId))
                                    {
                                        int qtdFalta = 0;
                                        //int.TryParse(listaQtdFalta[index], out qtdFalta);

                                        //LayoutDiarioExpedicao itemDEO = db.DiarioExpedicao
                                        //    .Where(d => d.ID == fileId)
                                        //    .FirstOrDefault();
                                        LayoutDiarioExpedicaos itemDEO = hlbapp.LayoutDiarioExpedicaos
                                            .Where(d => d.ID == fileId)
                                            .FirstOrDefault();

                                        itemDEO.QtdDiferenca = qtdFalta;
                                        itemDEO.Importado = "Conferido";

                                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, 
                                            "Conferência",
                                            usuario, 0, "", "", itemDEO);

                                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                                    }
                                }

                                //var listaImporta = hlbapp.ImportaDiarioExpedicao
                                //    .Where(d => d.Granja == granja && d.DataHoraCarreg == dataCarreg)
                                //    .ToList();

                                //foreach (var item in listaImporta)
                                //{
                                //    item.Importado = "Conferido";
                                //}
                                //db.SaveChanges();
                                //apoloService.SaveChanges();
                                hlbapp.SaveChanges();
                                hlbappLOG.SaveChanges();

                                ViewBag.mensagemConferencia = "Conferência da Diário " + dataAnterior.ToShortDateString() + " realizada com sucesso!";

                                DateTime dataInicial;
                                DateTime dataFinal;

                                if (Session["dataInicial"] == null)
                                {
                                    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                                    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                                }
                                else
                                {
                                    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                                    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                                }

                                var listaRetorno = CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal);

                                return View("ListaConferenciaDEO", listaRetorno);
                            //}
                            //else
                            //{
                            //    ViewBag.Erro = "Nem todos os itens foram selecionados como Conferidos! Por favor, selecionar todos após conferir os mesmos."
                            //            + "Caso não sejam todos selecionados, a conferência não pode ser salva!";
                            //    return View("ItemConfereDEO", listaDEO);
                            //}
                        }
                        else
                        {
                            //string responsavel = "Miriene Gomes";
                            //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                            //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                            //    responsavel = "Sérica Doimo";
                            //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                            //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                            //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                            //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                            string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                            ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                                + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                                + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                            return View("ItemConfereDEO", listaDEO);
                        }
                    }
                    catch (Exception ex)
                    {
                        #region Tratamento de Erro

                        string retorno = "";
                        //string retornoVB = "";

                        int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                        if (ex.InnerException != null)
                        {
                            retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                                + "Line Number: " + linenum.ToString();
                            
                            if (ex.InnerException.InnerException != null)
                                retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                                    + "Line Number: " + linenum.ToString();
                        }
                        else
                            retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                                + "Line Number: " + linenum.ToString();

                        ViewBag.Erro = retorno;

                        hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, listaDEO);                            

                        return View("ItemConfereDEO", listaDEO);

                        #endregion
                    }
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult ConfereDEOListaCegaAntigo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string tipoDEO = Session["tipoDEOselecionado"].ToString();

            var listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "Sim", "Decrescente", "Conferência");

            try
            {
                var fileIds = ("," + model["id"])
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.item).ToArray();

                var selectedIndices = model["qtdFalta"]
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.index).ToArray();

                var listaQtdFalta = ("," + model["qtdFalta"])
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.item).ToArray();

                if (!ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    if (listaDEO.Count == selectedIndices.Count())
                    {
                        string login = Session["login"].ToString();

                        string usuario = login.ToUpper();

                        DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                        string nfnum = Session["nfNum"].ToString();

                        HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                        foreach (var index in selectedIndices)
                        {
                            int fileId;
                            if (int.TryParse(fileIds[index], out fileId))
                            {
                                int qtdInformada = 0;
                                if (int.TryParse(listaQtdFalta[index], out qtdInformada))
                                {
                                    LayoutDiarioExpedicaos itemDEO = hlbapp.LayoutDiarioExpedicaos
                                        .Where(d => d.ID == fileId)
                                        .FirstOrDefault();

                                    string status = "Conferido";
                                    if (qtdInformada != itemDEO.QtdeOvos)
                                        status = "Divergência";

                                    int qtdDiferenca = qtdInformada - (int)itemDEO.QtdeOvos;

                                    itemDEO.QtdDiferenca = qtdDiferenca;
                                    itemDEO.Importado = status;

                                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                                        "Conferência Cega",
                                        usuario, itemDEO.QtdDiferenca, "", "", itemDEO);

                                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                                }
                            }
                        }

                        hlbapp.SaveChanges();
                        hlbappLOG.SaveChanges();

                        ViewBag.Mensagem = "Conferência do Diário "
                            + dataCarreg.ToShortDateString() + " realizada sem divergências!";

                        DateTime dataInicial;
                        DateTime dataFinal;

                        if (Session["dataInicial"] == null)
                        {
                            dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                        }
                        else
                        {
                            dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                        }

                        var listaDivergentes = CarregarItensDEO(hlbapp, dataCarreg, granja, "Divergência", 
                            "Decrescente", "Conferência");

                        if (listaDivergentes.Count > 0)
                        {
                            ViewBag.Mensagem = "Conferência do Diário "
                                + dataCarreg.ToShortDateString() + " realizada com divergências!"
                                + " Verifique as divergências para realizar a correção!";

                            #region Enviar E-mail para Responsável(is) da Análise das Divergências

                            string stringChar = "<br />";

                            #region Carrega responsáveis

                            string paraNome = "Depto. de Análise de Conferência de Estoque";
                            string paraEmail = "";
                            if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("CH"))
                                paraEmail = "analise.conf.estq-ch@hyline.com.br";
                            else if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("NM"))
                                paraEmail = "analise.conf.estq-nm@planaltopostura.com.br";
                            else if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("PH"))
                                paraEmail = "analise.conf.estq-ia@hyline.com.br";
                            else
                                paraEmail = "analise.conf.estq-aj@hyline.com.br";
                            //paraEmail = "palves@hyline.com.br";

                            string copiaPara = "";
                            if (listaDivergentes.FirstOrDefault().Granja.Equals("BP"))
                                copiaPara = "analise.conf.estq-bp@hyline.com.br";
                            else if (listaDivergentes.FirstOrDefault().Granja.Equals("HL"))
                                copiaPara = "analise.conf.estq-ob@hyline.com.br";
                            else if (listaDivergentes.FirstOrDefault().Granja.Equals("CG"))
                                copiaPara = "analise.conf.estq-cg@hyline.com.br";
                            else if (listaDivergentes.FirstOrDefault().Granja.Equals("GE"))
                                copiaPara = "analise.conf.estq-ge@hyline.com.br";
                            else if (listaDivergentes.FirstOrDefault().Granja.Equals("SB"))
                                copiaPara = "analise.conf.estq-av@hyline.com.br";
                            else
                                copiaPara = "analise.conf.estq-it@hyline.com.br";

                            #endregion

                            #region Carrega Lista de Itens Divergentes

                            #region Carrega Lista de Itens Divergentes no corpo do e-mail

                            string itensDivergentes = "";

                            if (listaDivergentes.Count > 0)
                                itensDivergentes =
                                    "<table style=\"width: 100%; "
                                        + "border-collapse: collapse; "
                                        + "text-align: center;\">";

                            itensDivergentes = itensDivergentes
                                + "<tr style=\"background: #333; "
                                    + "color: white; "
                                    + "font-weight: bold; "
                                    + "text-align: center;\">"
                                    + "<th>"
                                        + "Lote"
                                    + "</th>"
                                    + "<th>"
                                        + "Data Produção"
                                    + "</th>"
                                    + "<th>"
                                        + "Qtde. Granja"
                                    + "</th>"
                                    + "<th>"
                                        + "Qtde. Inc."
                                    + "</th>"
                                    + "<th>"
                                        + "Diferença"
                                    + "</th>"
                                + "</tr>";

                            foreach (var item in listaDivergentes)
                            {
                                itensDivergentes = itensDivergentes
                                    + "<tr>"
                                        + "<td style=\"padding: 6px; "
                                            + "border: 1px solid #ccc;\">"
                                                + item.LoteCompleto
                                        + "</td>"
                                        + "<td style=\"padding: 6px; "
                                            + "border: 1px solid #ccc;\">"
                                                + String.Format("{0:dd/MM/yyyy}", item.DataProducao)
                                        + "</td>"
                                        + "<td style=\"padding: 6px; "
                                            + "border: 1px solid #ccc;\">"
                                                + String.Format("{0:N0}", item.QtdeOvos)
                                        + "</td>"
                                        + "<td style=\"padding: 6px; "
                                            + "border: 1px solid #ccc;\">"
                                                + @String.Format("{0:N0}", (item.QtdeOvos + item.QtdDiferenca))
                                        + "</td>"
                                        + "<td style=\"padding: 6px; "
                                            + "border: 1px solid #ccc;\">"
                                                + @String.Format("{0:N0}", item.QtdDiferenca)
                                        + "</td>"
                                    + "</tr>";
                            }

                            if (listaDivergentes.Count > 0)
                                itensDivergentes = itensDivergentes + "</table>";

                            #endregion

                            #region Gera o E-mail

                            string assunto = "DEO DIVERGENTE - "
                                + listaDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                                + " - " + listaDivergentes.FirstOrDefault().Granja;
                            string corpoEmail = "";
                            string anexos = "";
                            string empresaApolo = "5";

                            //string porta = "";
                            //if (Request.Url.Port != 80)
                            //    porta = ":" + Request.Url.Port.ToString();

                            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                                + "Seguem abaixo os itens divergentes do DEO "
                                + listaDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                                + " - " + listaDivergentes.FirstOrDefault().Granja
                                + ":" + stringChar + stringChar
                                + itensDivergentes + stringChar + stringChar
                                //+ "Clique no link a seguir para poder realizar a aprovação: "
                                //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                                + "Por favor, analisar e solucionar a divergência até as 12h00 do dia de hoje, "
                                + "caso contrário a correção será realizada no incubatório!"
                                + stringChar + stringChar
                                + "SISTEMA WEB";

                            EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, anexos, empresaApolo, "Html");

                            #endregion

                            #endregion

                            #endregion
                        }

                        var listaRetorno = CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal);
                        return View("ListaConferenciaDEO", listaRetorno);
                    }
                    else
                    {
                        ViewBag.Erro = "Nem todos os itens foram informados! Por favor, informar qtde. para"
                            + " todos os itens."
                            + "Caso não sejam todos informados, a conferência não pode ser salva!";
                        return View("ItemConfereDEO", listaDEO);
                    }
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel +" sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                    return View("ItemConfereDEO", listaDEO);
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "";
                //string retornoVB = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                {
                    retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                    if (ex.InnerException.InnerException != null)
                        retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                            + "Line Number: " + linenum.ToString();
                }
                else
                    retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                ViewBag.Erro = retorno;

                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, listaDEO);

                return View("ItemConfereDEO", listaDEO);

                #endregion
            }
        }

        [HttpPost]
        public ActionResult ConfereDEOListaCega(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HLBAPPEntities hlbappLOG = new HLBAPPEntities();
            Models.bdApolo2.Apolo10Entities apolo = new Models.bdApolo2.Apolo10Entities();

            decimal metaTemperaturaOvoMinimo = 18m;
            decimal metaTemperaturaOvoMaximo = 24m;
            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string tipoDEO = Session["tipoDEOselecionado"].ToString();
            string numIdentificacao = "Sem ID";
            if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            DateTime dataRecInc = new DateTime();
            if (model["dataRecebInc"] != null)
                if (model["dataRecebInc"] != "")
                    DateTime.TryParse(model["dataRecebInc"], out dataRecInc);

            var listaDEO = CarregarItensDEO(hlbapp, granja, numIdentificacao, "Sim", "Decrescente");
            if (listaDEO.Count == 0)
                listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "Sim", "Decrescente", "Conferência");

            var listaAgrupada = listaDEO
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao
                })
                .OrderBy(o => o.Key.LoteCompleto)
                .ThenBy(t => t.Key.DataProducao)
                .Select(s => new
                {
                    Lote = s.Key.LoteCompleto,
                    Data = s.Key.DataProducao,
                    Status = s.Max(m => m.Importado),
                    QtdOvos = s.Sum(m => m.QtdeOvos)
                })
                .ToList();

            try
            {
                if (!ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    string login = Session["login"].ToString();
                    string usuario = login.ToUpper();
                    DateTime dataAnterior = Convert.ToDateTime("01/01/2014");
                    string nfnum = Session["nfNum"].ToString();

                    foreach (var loteData in listaAgrupada)
                    {
                        var listaItens = listaDEO
                            .Where(w => w.LoteCompleto == loteData.Lote
                                && w.DataProducao == loteData.Data)
                            .OrderByDescending(o => o.QtdeOvos)
                            .ToList();

                        var qtdeInformada = Convert.ToInt32(model["qtdDiferenca_" + loteData.Lote.ToString() + "|" + loteData.Data.ToShortDateString()]);

                        decimal temperaturaInternaOvo = 0;
                        Decimal.TryParse(model["temperaturaInternaOvo_" + loteData.Lote.ToString() + "|" + loteData.Data.ToShortDateString()], out temperaturaInternaOvo);
                        //var temperaturaInternaOvo = Convert.ToDecimal(model["temperaturaInternaOvo_" + loteData.Lote.ToString() + "|"
                        //    + loteData.Data.ToShortDateString()]);

                        decimal temperaturaInternaOvoMeio = 0;
                        Decimal.TryParse(model["temperaturaInternaOvoMeio_" + loteData.Lote.ToString() + "|" 
                            + loteData.Data.ToShortDateString()], out temperaturaInternaOvoMeio);

                        decimal temperaturaInternaOvoFim = 0;
                        Decimal.TryParse(model["temperaturaInternaOvoFim_" + loteData.Lote.ToString() + "|" 
                            + loteData.Data.ToShortDateString()], out temperaturaInternaOvoFim);

                        string obs = model["obs_" + loteData.Lote.ToString() + "|" + loteData.Data.ToShortDateString()].ToString();

                        string status = "Conferido";
                        if (qtdeInformada != loteData.QtdOvos)
                            status = "Divergência";

                        var qtdeInfoCalc = qtdeInformada;

                        foreach (var itemDEO in listaItens)
                        {
                            if (itemDEO.QtdeOvos <= qtdeInfoCalc)
                            {
                                itemDEO.QtdDiferenca = 0;
                                qtdeInfoCalc = qtdeInfoCalc - (int)itemDEO.QtdeOvos;
                                itemDEO.QtdeConferencia = (int)itemDEO.QtdeOvos;

                                if (listaItens.IndexOf(itemDEO) + 1 == listaItens.Count)
                                {
                                    itemDEO.QtdDiferenca = qtdeInfoCalc;
                                    itemDEO.QtdeConferencia = qtdeInfoCalc + (int)itemDEO.QtdeOvos;
                                }
                            }
                            else
                            {
                                //itemDEO.QtdeConferencia = qtdeInfoCalc - (int)itemDEO.QtdeOvos;
                                if (qtdeInfoCalc > 0)
                                {
                                    itemDEO.QtdDiferenca = qtdeInfoCalc - (int)itemDEO.QtdeOvos;
                                    itemDEO.QtdeConferencia = qtdeInfoCalc;
                                }
                                else
                                {
                                    itemDEO.QtdDiferenca = ((int)itemDEO.QtdeOvos * -1);
                                    itemDEO.QtdeConferencia = 0;
                                }
                                qtdeInfoCalc = qtdeInfoCalc - (int)itemDEO.QtdeOvos;
                            }

                            itemDEO.Importado = status;
                            itemDEO.DataHoraRecebInc = dataRecInc;
                            itemDEO.ResponsavelReceb = Session["login"].ToString();
                            itemDEO.TemperaturaOvoInterna = temperaturaInternaOvo;

                            itemDEO.TemperaturaOvoInternaMeio = temperaturaInternaOvoMeio;
                            itemDEO.TemperaturaOvoInternaFim = temperaturaInternaOvoFim;
                            itemDEO.Obs = obs;

                            LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                                "Conferência Cega",
                                usuario, itemDEO.QtdDiferenca, "", "", itemDEO);

                            hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);

                            #region Geração de Não Conformidades da Temperatura Interna do Ovo

                            if (temperaturaInternaOvo > 0 && (temperaturaInternaOvo < metaTemperaturaOvoMinimo) || (temperaturaInternaOvo > metaTemperaturaOvoMaximo))
                            {
                                string placa = "";
                                string empresaApolo = RetornaEmpresaApolo(itemDEO.Nucleo);
                                Models.bdApolo2.NOTA_FISCAL nfObj = apolo.NOTA_FISCAL
                                    .Where(w => w.EmpCod == empresaApolo
                                        && w.CtrlDFModForm == "NF-e"
                                        && w.CtrlDFSerie == "001"
                                        && w.NFNum == itemDEO.NFNum).FirstOrDefault();

                                if (nfObj != null)
                                    placa = nfObj.NFVeicPlaca;

                                string naoConformidade = "Temperatura do Ovo";
                                string observacao = "Fora da Meta! "
                                    + "(Meta: Mínima - " + String.Format("{0:N2}", metaTemperaturaOvoMinimo) + " / "
                                    + "Máxima - " + String.Format("{0:N2}", metaTemperaturaOvoMaximo) + ")"
                                    + ". Analisar e responder possível causa!";
                                InsereLOGAQO(itemDEO.ID, "Conferência Cega DEO", naoConformidade, observacao, "", "Pendente");

                                #region Envia E-mail para o Incubatório

                                #region Carrega Dados p/ E-mail

                                string stringChar = "" + (char)13 + (char)10;

                                FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                                FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
                                fTA.FillByFarmID(fDT, itemDEO.Nucleo);

                                string paraNome = "Incubatório de Matrizes Nova Granada";
                                string paraEmail = "sdoimo@hyline.com.br";
                                string copiaPara = "incubacao-ng@hyline.com.br";
                                if (itemDEO.Incubatorio == "NM")
                                {
                                    paraNome = "Incubatório de Matrizes Novo Mundo";
                                    paraEmail = "aneves@hyline.com.br";
                                    copiaPara = "incubacao-nm@hyline.com.br";
                                }
                                else if (itemDEO.Incubatorio == "TB")
                                {
                                    paraNome = "Incubatório de Matrizes Ajapi";
                                    paraEmail = "cfreire@hnavicultura.com.br";
                                    copiaPara = "incubatorio.hygen@gmail.com";
                                }
                                else if (itemDEO.Incubatorio == "PH")
                                {
                                    paraNome = "Incubatório de Bisavós";
                                    paraEmail = "jsegura@hyline.com.br";
                                    copiaPara = "aprates@hyline.com.br;administrativo-ia@hyline.com.br;producao-ga@hyline.com.br";
                                }

                                //string paraEmail = "palves@hyline.com.br";
                                //string copiaPara = "";

                                #endregion

                                #region Gera o E-mail

                                string assunto = "NÃO CONFORMIDADE \"" + naoConformidade.ToUpper() + "\" - "
                                    + itemDEO.LoteCompleto + " - " + itemDEO.DataProducao.ToShortDateString();
                                string corpoEmail = "";
                                string anexos = "";
                                string empresaEmail = "5";

                                corpoEmail = "Prezado " + paraNome + ", " + stringChar + stringChar
                                    + "Existe não conformidade na \"" + naoConformidade + "\" do DEO ID " + itemDEO.NumIdentificacao
                                    + " referente ao lote " + itemDEO.LoteCompleto + ", produzido em " + itemDEO.DataProducao.ToShortDateString()
                                    + " NF: " + itemDEO.NFNum + " - Placa: " + placa
                                    + " com o valor de " + String.Format("{0:N2}", temperaturaInternaOvo) + "°C"
                                    + " analisado em " + dataRecInc.ToShortDateString()
                                    + "." + stringChar + stringChar
                                    + "Por favor, verificar e realizar a resolução e informar no sistema!"
                                    + stringChar + stringChar
                                    + "SISTEMA WEB";

                                EnviarEmail(corpoEmail, assunto, "Incubatório", paraEmail, copiaPara, anexos, empresaEmail, "Texto");

                                #endregion

                                #endregion
                            }

                            #endregion
                        }
                    }

                    hlbapp.SaveChanges();
                    hlbappLOG.SaveChanges();

                    ViewBag.Mensagem = "Conferência do Diário "
                        + dataCarreg.ToShortDateString() + " realizada sem divergências!";

                    DateTime dataInicial;
                    DateTime dataFinal;

                    if (Session["dataInicial"] == null)
                    {
                        dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                    }
                    else
                    {
                        dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                        dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                    }

                    var listaDivergentes = CarregarItensDEO(hlbapp, granja, numIdentificacao, "Divergência", "Decrescente");
                    if (listaDivergentes.Count == 0)
                        listaDivergentes = CarregarItensDEO(hlbapp, dataCarreg, granja, "Divergência", "Decrescente", "Conferência");


                    var listaAgrupadaDiv = listaDivergentes
                        .GroupBy(g => new
                        {
                            g.LoteCompleto,
                            g.DataProducao
                        })
                        .OrderBy(o => o.Key.LoteCompleto)
                        .ThenBy(t => t.Key.DataProducao)
                        .Select(s => new
                        {
                            Lote = s.Key.LoteCompleto,
                            Data = s.Key.DataProducao,
                            Status = s.Max(m => m.Importado),
                            QtdOvos = s.Sum(m => m.QtdeOvos),
                            QtdDif = s.Sum(m => m.QtdDiferenca)
                        })
                        .ToList();

                    if (listaAgrupadaDiv.Count > 0)
                    {
                        ViewBag.Mensagem = "Conferência do Diário "
                            + dataCarreg.ToShortDateString() + " realizada com divergências!"
                            + " Verifique as divergências para realizar a correção!";

                        #region Enviar E-mail para Responsável(is) da Análise das Divergências

                        string stringChar = "<br />";

                        #region Carrega responsáveis

                        string paraNome = "Depto. de Análise de Conferência de Estoque";
                        string paraEmail = "";
                        if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("CH"))
                            paraEmail = "analise.conf.estq-ch@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("NM"))
                            paraEmail = "analise.conf.estq-nm@planaltopostura.com.br";
                        else if (listaDivergentes.FirstOrDefault().Incubatorio.Equals("PH"))
                            paraEmail = "analise.conf.estq-ia@hyline.com.br";
                        else
                            paraEmail = "analise.conf.estq-aj@hyline.com.br";
                        //paraEmail = "palves@hyline.com.br";

                        string copiaPara = "";
                        if (listaDivergentes.FirstOrDefault().Granja.Equals("BP"))
                            copiaPara = "analise.conf.estq-bp@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("HL"))
                            copiaPara = "analise.conf.estq-ob@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("CG"))
                            copiaPara = "analise.conf.estq-cg@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("GE"))
                            copiaPara = "analise.conf.estq-ge@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("SB"))
                            copiaPara = "analise.conf.estq-av@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("CH"))
                            copiaPara = "analise.conf.estq-ch@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("NM"))
                            copiaPara = "analise.conf.estq-nm@planaltopostura.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("PH"))
                            copiaPara = "analise.conf.estq-ia@hyline.com.br";
                        else if (listaDivergentes.FirstOrDefault().Granja.Equals("TB"))
                            copiaPara = "analise.conf.estq-aj@hyline.com.br";
                        else
                            copiaPara = "analise.conf.estq-it@hyline.com.br";

                        #endregion

                        #region Carrega Lista de Itens Divergentes

                        #region Carrega Lista de Itens Divergentes no corpo do e-mail

                        string itensDivergentes = "";

                        if (listaAgrupadaDiv.Count > 0)
                            itensDivergentes =
                                "<table style=\"width: 100%; "
                                    + "border-collapse: collapse; "
                                    + "text-align: center;\">";

                        itensDivergentes = itensDivergentes
                            + "<tr style=\"background: #333; "
                                + "color: white; "
                                + "font-weight: bold; "
                                + "text-align: center;\">"
                                + "<th>"
                                    + "Lote"
                                + "</th>"
                                + "<th>"
                                    + "Data Produção"
                                + "</th>"
                                + "<th>"
                                    + "Qtde. Granja"
                                + "</th>"
                                + "<th>"
                                    + "Qtde. Inc."
                                + "</th>"
                                + "<th>"
                                    + "Diferença"
                                + "</th>"
                            + "</tr>";

                        foreach (var item in listaAgrupadaDiv)
                        {
                            itensDivergentes = itensDivergentes
                                + "<tr>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + item.Lote
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + String.Format("{0:dd/MM/yyyy}", item.Data)
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + String.Format("{0:N0}", item.QtdOvos)
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + @String.Format("{0:N0}", (item.QtdOvos + item.QtdDif))
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + @String.Format("{0:N0}", item.QtdDif)
                                    + "</td>"
                                + "</tr>";
                        }

                        if (listaDivergentes.Count > 0)
                            itensDivergentes = itensDivergentes + "</table>";

                        #endregion

                        #region Gera o E-mail

                        string assunto = "DEO DIVERGENTE - "
                            + listaDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                            + " - " + listaDivergentes.FirstOrDefault().Granja;
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        //string porta = "";
                        //if (Request.Url.Port != 80)
                        //    porta = ":" + Request.Url.Port.ToString();

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Seguem abaixo os itens divergentes do DEO "
                            + listaDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                            + " - " + listaDivergentes.FirstOrDefault().Granja
                            + ":" + stringChar + stringChar
                            + itensDivergentes + stringChar + stringChar
                            //+ "Clique no link a seguir para poder realizar a aprovação: "
                            //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                            + "Por favor, analisar e solucionar a divergência até as 12h00 do dia de hoje, "
                            + "caso contrário a correção será realizada no incubatório!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, anexos, empresaApolo, "Html");

                        #endregion

                        #endregion

                        #endregion
                    }

                    var listaRetorno = CarregarListaDEOStatus(granja, "Sim", dataInicial, dataFinal);
                    return View("ListaConferenciaDEO", listaRetorno);
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                        ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                            + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                            + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                    return View("ItemConfereDEO", listaDEO);
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "";
                //string retornoVB = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                {
                    retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                    if (ex.InnerException.InnerException != null)
                        retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                            + "Line Number: " + linenum.ToString();
                }
                else
                    retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                ViewBag.Erro = retorno;

                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, listaDEO);

                return View("ItemConfereDEO", listaDEO);

                #endregion
            }
        }

        #endregion

        #region Métodos Divergência DEO

        public ActionResult ListaDivergenciaDEO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CarregaListaGranjas(true);
            string granja = "";
            if (Session["granjaSelecionada"] != null)
                granja = Session["granjaSelecionada"].ToString();
            AtualizaGranjaSelecionada(granja);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            if (granja.Equals("SB") || granja.Equals("PH"))
                Session["location"] = "GP";
            else
                Session["location"] = "PP";

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

            return View("ListaDEODivergencia", CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal));
        }

        public ActionResult CarregarListaDEOFiltroDivergenciaView(string Text, string status, 
            DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                Session["dataInicial"] = dataInicial.ToShortDateString();
                Session["dataFinal"] = dataFinal.ToShortDateString();
                dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");
            }

            if (Text.Equals("SB") || Text.Equals("PH"))
                Session["location"] = "GP";
            else
                Session["location"] = "PP";

            //CarregaListaIncubatorios();
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), true, true);

            return View("ListaDEODivergencia", CarregarListaDEOStatus(Text, "Divergência", dataInicial, dataFinal));
        }

        public ActionResult ListaItensDEODivergencia(DateTime dataFiltro, string nfNum, string tipoDEO, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string granja = Session["granjaSelecionada"].ToString();
            Session["dataHoraCarreg"] = dataFiltro;
            Session["nfNum"] = nfNum;
            Session["tipoDEOselecionado"] = tipoDEO;
            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            var listaItens = CarregarItensDEO(hlbapp, granja, numIdentificacao, "Divergência", "Crescente");
            if (listaItens.Count == 0)
                listaItens = CarregarItensDEO(hlbapp, dataFiltro, granja, "Divergência", "Crescente", "Divergência");
            var incubatorioDestino = listaItens.FirstOrDefault().Incubatorio;

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorioDestino))
            {
                DateTime dataInicial;
                DateTime dataFinal;

                if (Session["dataInicial"] == null)
                {
                    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                }
                else
                {
                    dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                    dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                }

                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                var listaRetorno = CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal);
                return View("ListaDEODivergencia", listaRetorno);
            }

            //var listaItens = CarregarItensDEO(hlbapp, dataFiltro, granja, "Divergência", "Crescente", "Divergência");

            #region Carrega Sessions Valores

            var listaAgrupada = listaItens
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao
                })
                .OrderBy(o => o.Key.LoteCompleto)
                .ThenBy(t => t.Key.DataProducao)
                .Select(s => new
                {
                    Lote = s.Key.LoteCompleto,
                    Data = s.Key.DataProducao,
                    Status = s.Max(m => m.Importado),
                    QtdDif = s.Sum(m => (m.QtdDiferenca == null ? 0 : m.QtdDiferenca))
                })
                .ToList();

            foreach (var item in listaAgrupada)
            {
                LOG_LayoutDiarioExpedicaos ultimoLOGDivergencia =
                    hlbapp.LOG_LayoutDiarioExpedicaos
                    .Where(w => w.Granja == granja
                        && w.DataHoraCarreg == dataFiltro
                        && w.LoteCompleto == item.Lote
                        && w.DataProducao == item.Data
                        && w.Operacao == "Item Divergente Inserido")
                    .OrderByDescending(o => o.DataHoraOper)
                    .FirstOrDefault();

                if (ultimoLOGDivergencia != null)
                {
                    Session["ListaMotivoDivergenciaDEO_"
                        + item.Lote.ToString() + "|"
                        + item.Data.ToShortDateString()] = CarregaListaMotivoDivergenciaDEO();
                    string motivoDivergenciaDEO = ultimoLOGDivergencia.MotivoDivergenciaDEO;
                    AtualizaDDL(motivoDivergenciaDEO, 
                        (List<SelectListItem>)Session["ListaMotivoDivergenciaDEO_"
                            + item.Lote.ToString() + "|"
                            + item.Data.ToShortDateString()]);
                }
                else
                    Session["ListaMotivoDivergenciaDEO_"
                        + item.Lote.ToString() + "|"
                        + item.Data.ToShortDateString()] = CarregaListaMotivoDivergenciaDEO();
            }

            #endregion

            return View("ItemDivergeDEO", listaItens);
        }

        [HttpPost]
        public ActionResult ConfereDEODivergenteAntiga(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string tipoDEO = Session["tipoDEOselecionado"].ToString();

            var listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "Divergência", "Crescente",
                "Divergência");

            try
            {
                var fileIds = ("," + model["id"])
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.item).ToArray();

                var selectedIndices = model["motivoDivergenciaDEO"]
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.index).ToArray();

                var listaMotivoDivergenciaDEO = model["motivoDivergenciaDEO"]
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.item).ToArray();

                var listaMotivos = ("," + model["motivo"])
                    .Split(',')
                    .Select((item, index) => new { item = item, index = index })
                    .Where(row => row.item != "")
                    .Select(row => row.item).ToArray();

                if (!ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    if (listaDEO.Count == selectedIndices.Count())
                    {
                        string login = Session["login"].ToString();

                        string usuario = login.ToUpper();

                        DateTime dataAnterior = Convert.ToDateTime("01/01/2014");

                        string nfnum = Session["nfNum"].ToString();

                        HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                        foreach (var index in selectedIndices)
                        {
                            int fileId;
                            if (int.TryParse(fileIds[index], out fileId))
                            {
                                string motivoDivergenciaDEO = listaMotivoDivergenciaDEO[index];
                                string motivo = listaMotivos[index];
                                
                                LayoutDiarioExpedicaos itemDEO = hlbapp.LayoutDiarioExpedicaos
                                    .Where(d => d.ID == fileId)
                                    .FirstOrDefault();

                                if (itemDEO != null)
                                {
                                    itemDEO.Importado = "Conferido";

                                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                                        "Aprovação Divergência",
                                        usuario, itemDEO.QtdDiferenca, motivoDivergenciaDEO, motivo, itemDEO);

                                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                                }
                            }
                        }

                        hlbapp.SaveChanges();
                        hlbappLOG.SaveChanges();

                        ViewBag.Mensagem = "Correção das Divergências do Diário " + dataCarreg.ToShortDateString() + " realizada com sucesso!";

                        DateTime dataInicial;
                        DateTime dataFinal;

                        if (Session["dataInicial"] == null)
                        {
                            dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
                        }
                        else
                        {
                            dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                            dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
                        }

                        var listaRetorno = CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal);

                        return View("ListaDEODivergencia", listaRetorno);
                    }
                    else
                    {
                        ViewBag.Erro = "Nem todos os itens foram informados! Por favor, informar qtde. para"
                            + " todos os itens."
                            + "Caso não sejam todos informados, a conferência não pode ser salva!";
                        return View("ItemConfereDEO", listaDEO);
                    }
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a correção das divergências não pode ser realizada!";
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                    return View("ItemDivergeDEO", listaDEO);
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "";
                //string retornoVB = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                {
                    retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                    if (ex.InnerException.InnerException != null)
                        retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                            + "Line Number: " + linenum.ToString();
                }
                else
                    retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                ViewBag.Erro = retorno;

                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, listaDEO);

                return View("ListaDEODivergencia", listaDEO);

                #endregion
            }
        }

        [HttpPost]
        public ActionResult ConfereDEODivergente(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string tipoDEO = Session["tipoDEOselecionado"].ToString();
            string numIdentificacao = "Sem ID";
            if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            //var listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "Divergência", "Decrescente", "Divergência");
            var listaDEO = CarregarItensDEO(hlbapp, granja, numIdentificacao, "Divergência", "Decrescente");
            if (listaDEO.Count == 0)
                listaDEO = CarregarItensDEO(hlbapp, dataCarreg, granja, "Divergência", "Decrescente", "Divergência");

            var listaAgrupada = listaDEO
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao
                })
                .OrderBy(o => o.Key.LoteCompleto)
                .ThenBy(t => t.Key.DataProducao)
                .Select(s => new
                {
                    Lote = s.Key.LoteCompleto,
                    Data = s.Key.DataProducao,
                    Status = s.Max(m => m.Importado),
                    QtdOvos = s.Sum(m => m.QtdeOvos)
                })
                .ToList();

            try
            {
                if (!ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    string login = Session["login"].ToString();
                    string usuario = login.ToUpper();
                    DateTime dataAnterior = Convert.ToDateTime("01/01/2014");
                    string nfnum = Session["nfNum"].ToString();

                    foreach (var loteData in listaAgrupada)
                    {
                        var listaItens = listaDEO
                            .Where(w => w.LoteCompleto == loteData.Lote
                                && w.DataProducao == loteData.Data)
                            .OrderByDescending(o => o.QtdeOvos)
                            .ToList();

                        string motivoDivergenciaDEO = model["motivoDivergenciaDEO_" + loteData.Lote.ToString() + "|"
                            + loteData.Data.ToShortDateString()];
                        string motivo = model["motivo_" + loteData.Lote.ToString() + "|"
                            + loteData.Data.ToShortDateString()];

                        foreach (var itemDEO in listaItens)
                        {
                            if (motivoDivergenciaDEO == "Erro Contagem no Incubatório")
                                itemDEO.QtdDiferenca = 0;

                            itemDEO.Importado = "Conferido";

                            LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                                "Aprovação Divergência",
                                usuario, itemDEO.QtdDiferenca, motivoDivergenciaDEO, motivo, itemDEO);

                            hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                        }
                    }

                    hlbapp.SaveChanges();
                    hlbappLOG.SaveChanges();

                    ViewBag.Mensagem = "Correção das Divergências do Diário " 
                        + dataCarreg.ToShortDateString() + " realizada com sucesso!";

                    var listaRetorno = CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal);
                    return View("ListaDEODivergencia", listaRetorno);
                }
                else
                {
                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Estoque já fechado! Verifique com " + responsavel + " sobre a possibilidade da abertura!"
                    //        + "Caso não seja aberto, a conferência não pode ser realizada!";
                    var listaRetorno = CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal);
                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());
                    return View("ListaDEODivergencia", listaRetorno);
                }
            }
            catch (Exception ex)
            {
                #region Tratamento de Erro

                string retorno = "";
                //string retornoVB = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                if (ex.InnerException != null)
                {
                    retorno = "Erro: " + ex.InnerException.Message + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                    if (ex.InnerException.InnerException != null)
                        retorno = "Erro: " + ex.InnerException.InnerException.Message + (char)10 + (char)13
                            + "Line Number: " + linenum.ToString();
                }
                else
                    retorno = "Erro : " + ex.Message + (char)10 + (char)13 + (char)10 + (char)13
                        + "Line Number: " + linenum.ToString();

                ViewBag.Erro = retorno;

                var listaRetorno = CarregarListaDEOStatus(granja, "Divergência", dataInicial, dataFinal);
                return View("ListaDEODivergencia", listaRetorno);

                #endregion
            }
        }

        #endregion

        #region Classificação dos Ovos

        #region Lista Classificação dos Ovos

        public List<LayoutDiarioExpedicaos> CarregaClassificacaoOvos(string incubatorio, DateTime dataInicial,
            DateTime dataFinal)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            DateTime dataHoraIni = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            DateTime dataHoraFim = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            var lista = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.TipoDEO == "Classificação de Ovos"
                    && w.DataHoraCarreg >= dataHoraIni
                    && w.DataHoraCarreg <= dataHoraFim)
                .OrderBy(o => o.DataHoraCarreg)
                .ToList();

            return lista;
        }

        public ActionResult FiltraListaClassificacaoOvos(string incubatorio,
            DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioSelecionado"] = incubatorio;
            Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);
            Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            Session["dataInicial"] = dataInicial.ToShortDateString();
            Session["dataFinal"] = dataFinal.ToShortDateString();
            dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            Session["ListaClassificacaoOvos"] = CarregaClassificacaoOvos(incubatorio, dataInicial, dataFinal);

            return View("ListaClassificacaoOvos");
        }

        public void RefreshListaClassificacaoOvos()
        {
            string incubatorio = "";
            if (Session["incubatorioSelecionado"] != null)
                incubatorio = Session["incubatorioSelecionado"].ToString();
            else
            {
                incubatorio = ((List<SelectListItem>)Session["ListaIncubatorios"]).FirstOrDefault().Value;
                Session["incubatorioSelecionado"] = incubatorio;
                Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
                Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            }
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            Session["ListaClassificacaoOvos"] = CarregaClassificacaoOvos(incubatorio, dataInicial, dataFinal);
        }

        public ActionResult ListaClassificacaoOvos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["ListaIncubatorios"] = CarregaListaIncubatoriosCO("", false, false);
            RefreshListaClassificacaoOvos();

            return View();
        }

        #endregion

        #region CRUD Classificação dos Ovos

        public ActionResult CreateClassificacaoOvo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
            Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);

            var listItens = new List<LayoutDiarioExpedicaos>();

            Session["dataRecebInc"] = "";
            Session["dataClassificacao"] = DateTime.Now;
            Session["ListaItensClassificacaoOvos"] = listItens;

            var incubatorio = Session["incubatorioSelecionado"].ToString();

            if (DateTime.Today >= Convert.ToDateTime("15/12/2021") && (incubatorio == "CH" || incubatorio == "NM"))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR CLASSIFICAÇÃO DE OVOS A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";
            }

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
            {
                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                RefreshListaClassificacaoOvos();
                return View("ListaClassificacaoOvos");
            }

            return View("ClassificacaoOvo");
        }

        public ActionResult EditClassificacaoOvo(string incubatorio, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
            {
                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                RefreshListaClassificacaoOvos();
                return View("ListaClassificacaoOvos");
            }

            if (Session["incubatorioSelecionado"] == null)
            {
                Session["incubatorioSelecionado"] = incubatorio;
                AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatoriosDestino"]);
                Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatoriosDestino"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            }
            Session["numIdentificacaoSelecionado"] = numIdentificacao;
            Session["dataRecebInc"] = listItens.FirstOrDefault().DataHoraRecebInc;
            Session["dataClassificacao"] = listItens.FirstOrDefault().DataHoraCarreg;
            Session["ListaItensClassificacaoOvos"] = listItens;

            return View("ClassificacaoOvo");
        }

        public ActionResult ReturnClassificacaoOvo(string incubatorio, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("ClassificacaoOvo");
        }

        public ActionResult SaveClassificacaoOvo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime dataHoraClassificacao = DateTime.Now;
            if (model["dataClassificacao"] != null)
            {
                dataHoraClassificacao = Convert.ToDateTime(model["dataClassificacao"]);

                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                var listItens = bd.LayoutDiarioExpedicaos
                    .Where(w => w.NumIdentificacao == numIdentificacao)
                    .ToList();

                DateTime dataRecInc = Convert.ToDateTime("01/01/1988");
                if (model["dataRecebInc"] != null)
                    if (model["dataRecebInc"] != "")
                        DateTime.TryParse(model["dataRecebInc"], out dataRecInc);

                foreach (var item in listItens)
                {
                    item.DataHoraCarreg = dataHoraClassificacao;
                    if (dataRecInc != Convert.ToDateTime("1988-01-01 00:00:00.000"))
                        item.DataHoraRecebInc = dataRecInc;
                }

                bd.SaveChanges();

                if (listItens.Count > 0)
                {
                    ViewBag.ClasseMsg = "msgSucesso";
                    ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString())
                        + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                    RefreshListaClassificacaoOvos();
                    return View("ListaClassificacaoOvos");
                }
                else
                {
                    ViewBag.ClasseMsg = "msgWarning";
                    ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhuma classificação", Session["language"].ToString())
                        + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                    return View("ClassificacaoOvo");
                }
            }

            RefreshListaClassificacaoOvos();
            return View("ListaClassificacaoOvos");
        }

        public ActionResult DeleteClassificacaoOvo(string incubatorio, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioSelecionado"] = incubatorio;
            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
            {
                ViewBag.Erro = am.GetTextOnLanguage("Existe Solicitação de Ajuste de Estoque em aberto! "
                    + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!", Session["language"].ToString());

                RefreshListaClassificacaoOvos();
                return View("ListaClassificacaoOvos");
            }

            return View();
        }

        public ActionResult DeleteClassificacaoOvoConfirma()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string incubatorio = Session["incubatorioSelecionado"].ToString();
            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString())
                + " " + am.GetTextOnLanguage("excluída com sucesso!", Session["language"].ToString());

            RefreshListaClassificacaoOvos();
            return View("ListaClassificacaoOvos");
        }

        #endregion

        #region CRUD Classificação do Item dos Ovos

        public ActionResult CreateClassificacaoOvoItem()
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = new List<LayoutDiarioExpedicaos>();

            CarregaClassificacaoOvoItem(listLote);

            return View("ClassificacaoOvoItem");
        }

        public ActionResult EditClassificacaoOvoItem(string incubatorio, string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            CarregaClassificacaoOvoItem(listLote);

            return View("ClassificacaoOvoItem");
        }

        public void CarregaClassificacaoOvoItem(List<LayoutDiarioExpedicaos> listLote)
        {
            if (listLote.Count > 0)
            {
                var item = listLote.FirstOrDefault();

                Session["DDLNucleo"] = CarregaListaNucleosFLIP(item.Granja);
                AtualizaDDL(item.Nucleo, (List<SelectListItem>)Session["DDLNucleo"]);
                Session["DDLLotes"] = CarregaLotesFLIP(item.Granja, item.Nucleo);
                AtualizaDDL(item.Lote, (List<SelectListItem>)Session["DDLLotes"]);
                Session["DDLGalpoes"] = CarregaDDLGalpoes(item.Lote);
                AtualizaDDL(item.Galpao, (List<SelectListItem>)Session["DDLGalpoes"]);
                Session["loteCompleto"] = item.LoteCompleto;
                Session["DataProducao"] = item.DataProducao;
                Session["QtdeTotal"] = RetornaSaldo(item.Incubatorio, item.LoteCompleto, item.DataProducao);
                Session["idadeLote"] = item.Idade;
                
                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == item.Granja
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    var itemLote = listLote.Where(w => w.TipoOvo == tipoOvo.CodigoTipo)
                        .FirstOrDefault();
                    if (itemLote != null)
                    {
                        Session[tipoOvo.CodigoTipo] = itemLote.QtdeOvos;
                        int saldo = RetornaSaldo(tipoOvo.CodigoTipo, item.LoteCompleto, item.DataProducao);
                        if ((saldo - item.QtdeOvos) >= 0)
                            Session["existeSaldo" + tipoOvo.CodigoTipo] = false;
                        else
                            Session["existeSaldo" + tipoOvo.CodigoTipo] = true;
                    }
                    else
                    {
                        Session[tipoOvo.CodigoTipo] = 0;
                        Session["existeSaldo" + tipoOvo.CodigoTipo] = false;
                    }
                }
            }
            else
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                Session["DDLNucleo"] = CarregaListaNucleosFLIP(incubatorio);
                Session["DDLLotes"] = new List<SelectListItem>();
                Session["DDLGalpoes"] = new List<SelectListItem>();
                Session["loteCompleto"] = "";
                Session["DataProducao"] = DateTime.Today;
                Session["QtdeTotal"] = 0;
                Session["idadeLote"] = "";

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    Session[tipoOvo.CodigoTipo] = 0;
                    Session["existeSaldo" + tipoOvo.CodigoTipo] = true;
                }
            }
        }

        [HttpPost]
        public ActionResult SaveClassificacaoOvoItem(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();

            #region Load General Variables

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();
            string incubatorio = Session["incubatorioSelecionado"].ToString();
            DateTime dataRec = Convert.ToDateTime("01/01/1988");
            if (Session["dataRecebInc"] != "")
                dataRec = Convert.ToDateTime(Session["dataRecebInc"]);
            DateTime dataClassificacao = Convert.ToDateTime(Session["dataClassificacao"]);
            string nucleo = model["Nucleo"];
            string lote = model["Lote"];
            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
            var loteSelecionado = listaLotes.Where(s => s.NumeroLote == lote).FirstOrDefault();
            string galpao = model["Galpao"];
            string loteCompleto = model["loteCompleto"];
            if (model["loteCompleto"] == "") loteCompleto = loteSelecionado.LoteCompleto;
            string linhagem = model["linhagem"];
            if (model["linhagem"] == "") linhagem = loteSelecionado.Linhagem;
            DateTime dataProducao = Convert.ToDateTime(model["dataProducaoCO"]);
            int idade = Convert.ToInt32(model["idade"]);
            int qtdGerada = 0;

            #endregion

            if (dataClassificacao >= Convert.ToDateTime("15/12/2021") && (incubatorio == "CH" || incubatorio == "NM"))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR CLASSIFICAÇÃO DE OVOS A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";
                return View("ClassificacaoOvoItem");
            }

            var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                .Where(w => w.Unidade == incubatorio
                     && w.Origem == "Interna")
                .ToList();

            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                int qtdClassificada = Convert.ToInt32(model[tipoOvo.CodigoTipo]);

                var item = bd.LayoutDiarioExpedicaos
                    .Where(w => w.Incubatorio == incubatorio
                        && w.NumIdentificacao == numIdentificacao
                        && w.LoteCompleto == loteCompleto
                        && w.DataProducao == dataProducao
                        && w.TipoDEO == "Classificação de Ovos"
                        && w.TipoOvo == tipoOvo.CodigoTipo)
                    .FirstOrDefault();

                if (item == null && qtdClassificada > 0)
                {
                    item = new LayoutDiarioExpedicaos();
                    item.Granja = incubatorio;
                    item.Incubatorio = incubatorio;
                    item.NumIdentificacao = numIdentificacao;
                    item.DataHoraCarreg = dataClassificacao;
                    item.Nucleo = nucleo;
                    item.Linhagem = linhagem;
                    item.LoteCompleto = loteCompleto;
                    item.Lote = lote;
                    item.Galpao = galpao;
                    item.DataProducao = dataProducao;
                    item.Idade = idade;
                    item.NumeroReferencia = dataProducao.DayOfYear.ToString();
                    item.TipoDEO = "Classificação de Ovos";
                    item.TipoOvo = tipoOvo.CodigoTipo;
                    item.NFNum = "";
                    item.Importado = "Conferido";
                    item.Usuario = Session["login"].ToString();
                    item.DataHora = DateTime.Now;
                    item.DataHoraRecebInc = dataRec;
                    item.ResponsavelCarreg = "";
                    item.ResponsavelReceb = "";
                    item.GTANum = "";
                    item.Lacre = "";
                    item.Observacao = "Gerado automaticamente pela Classificação de Ovos - Tipo do Ovo: "
                        + tipoOvo.DescricaoTipo;
                    item.QtdDiferenca = 0;
                    item.QtdeConferencia = 0;
                }

                if (item != null)
                {
                    qtdGerada++;

                    if (qtdClassificada == 0 && item.ID > 0)
                        bd.LayoutDiarioExpedicaos.DeleteObject(item);
                    else
                        item.QtdeOvos = qtdClassificada;

                    if (item.ID == 0 && qtdClassificada > 0) bd.LayoutDiarioExpedicaos.AddObject(item);
                }

                #region LOG

                HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                if (item != null)
                {
                    if (item.ID == 0)
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Inserido",
                            item.Usuario, 0, "", "", item);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                    else
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Alterado",
                            item.Usuario, 0, "", "", item);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                }

                hlbappLOG.SaveChanges();

                #endregion
            }

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["ListaItensClassificacaoOvos"] = listItens;

            if (qtdGerada > 0)
            {
                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                return View("ClassificacaoOvo");
            }
            else
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhuma classificação", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                return View("ClassificacaoOvoItem");
            }
        }

        public ActionResult DeleteClassificacaoOvoItem(string incubatorio, string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString()) + " - "
                + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                + lote + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                + dataProducao.ToShortDateString()
                + " " + am.GetTextOnLanguage("excluída com sucesso!", Session["language"].ToString());

            Session["ListaItensClassificacaoOvos"] = listItens;

            return View("ClassificacaoOvo");
        }

        #endregion

        #region CRUD Classificação do Item dos Ovos Por Período

        public ActionResult CreateClassificacaoOvoItemPorPeriodo()
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = new List<LayoutDiarioExpedicaos>();

            CarregaClassificacaoOvoItemPorPeriodo(listLote);

            return View("ClassificacaoOvoItemPorPeriodo");
        }

        public ActionResult EditClassificacaoOvoItemPorPeriodo(string incubatorio, string numIdentificacao,
            string lote, DateTime dataProducaoInicial, DateTime dataProducaoFinal)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao >= dataProducaoInicial
                    && w.DataProducao <= dataProducaoFinal)
                .ToList();

            CarregaClassificacaoOvoItemPorPeriodo(listLote);

            return View("ClassificacaoOvoItemPorPeriodo");
        }

        public void CarregaClassificacaoOvoItemPorPeriodo(List<LayoutDiarioExpedicaos> listLote)
        {
            if (listLote.Count > 0)
            {
                var item = listLote.FirstOrDefault();
                
                Session["DDLNucleo"] = CarregaListaNucleosFLIP(item.Granja);
                AtualizaDDL(item.Nucleo, (List<SelectListItem>)Session["DDLNucleo"]);
                Session["DDLLotes"] = CarregaLotesFLIP(item.Granja, item.Nucleo);
                AtualizaDDL(item.Lote, (List<SelectListItem>)Session["DDLLotes"]);
                Session["DDLGalpoes"] = CarregaDDLGalpoes(item.Lote);
                AtualizaDDL(item.Galpao, (List<SelectListItem>)Session["DDLGalpoes"]);
                Session["loteCompleto"] = item.LoteCompleto;
                Session["DataProducaoInicial"] = listLote.Min(m => m.DataProducao);
                Session["DataProducaoFinal"] = listLote.Max(m => m.DataProducao);

                int saldoTotal = 0;
                DateTime data = Convert.ToDateTime(Session["DataProducaoInicial"]);
                while (data <= Convert.ToDateTime(Session["DataProducaoFinal"]))
                {
                    saldoTotal = saldoTotal + RetornaSaldo(item.Incubatorio, item.LoteCompleto, data);
                    data = data.AddDays(1);
                }

                Session["QtdeTotal"] = saldoTotal;
                Session["idadeLote"] = item.Idade;

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == item.Granja
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    var qtdClassificada = listLote.Where(w => w.TipoOvo == tipoOvo.CodigoTipo).Sum(s => s.QtdeOvos);
                    if (qtdClassificada > 0)
                    {
                        Session[tipoOvo.CodigoTipo] = qtdClassificada;
                        int saldoClassificado = 0;
                        data = Convert.ToDateTime(Session["DataProducaoInicial"]);
                        while (data <= Convert.ToDateTime(Session["DataProducaoFinal"]))
                        {
                            saldoClassificado = saldoClassificado + RetornaSaldo(tipoOvo.CodigoTipo, item.LoteCompleto, data);
                            data = data.AddDays(1);
                        }
                    }
                    else
                    {
                        Session[tipoOvo.CodigoTipo] = 0;
                    }
                }
            }
            else
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                Session["DDLNucleo"] = CarregaListaNucleosFLIP(incubatorio);
                Session["DDLLotes"] = new List<SelectListItem>();
                Session["DDLGalpoes"] = new List<SelectListItem>();
                Session["loteCompleto"] = "";
                Session["DataProducaoInicial"] = DateTime.Today;
                Session["DataProducaoFinal"] = DateTime.Today;
                Session["QtdeTotal"] = 0;
                Session["idadeLote"] = "";

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    Session[tipoOvo.CodigoTipo] = 0;
                }
            }
        }

        [HttpPost]
        public ActionResult SaveClassificacaoOvoItemPorPeriodo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();

            #region Load General Variables

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();
            string incubatorio = Session["incubatorioSelecionado"].ToString();
            DateTime dataClassificacao = Convert.ToDateTime(Session["dataClassificacao"]);
            string nucleo = model["Nucleo"];
            string lote = model["Lote"];
            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
            var loteSelecionado = listaLotes.Where(s => s.NumeroLote == lote).FirstOrDefault();
            string galpao = model["Galpao"];
            string loteCompleto = model["loteCompleto"];
            if (model["loteCompleto"] == "") loteCompleto = loteSelecionado.LoteCompleto;
            string linhagem = model["linhagem"];
            if (model["linhagem"] == "") linhagem = loteSelecionado.Linhagem;
            DateTime dataProducaoInicial = Convert.ToDateTime(model["dataProducaoInicialCO"]);
            DateTime dataProducaoFinal = Convert.ToDateTime(model["dataProducaoFinalCO"]);
            int idade = Convert.ToInt32(model["idade"]);
            int qtdGerada = 0;

            #endregion

            if (dataClassificacao >= Convert.ToDateTime("15/12/2021") && (incubatorio == "CH" || incubatorio == "NM"))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR CLASSIFICAÇÃO DE OVOS A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";
                return View("ClassificacaoOvoItem");
            }

            #region TESTE

            //foreach (var tipoOvo in listaTipoClassificacaoOvo)
            //{
            //    int qtdClassificada = Convert.ToInt32(model[tipoOvo.CodigoTipo]);
            //    decimal percRateio = qtdClassificada / qtdTotalClassificada;

            //    while (dataProducao <= dataProducaoFinal)
            //    {
            //        int saldoLoteDataProducao = RetornaSaldo(incubatorio, loteCompleto, dataProducao);
            //        int qtdeClassificadaProporcional = Convert.ToInt32(Math.Round(saldoLoteDataProducao * percRateio, 0));

            //        var item = bd.LayoutDiarioExpedicaos
            //        .Where(w => w.Incubatorio == incubatorio
            //            && w.NumIdentificacao == numIdentificacao
            //            && w.LoteCompleto == loteCompleto
            //            && w.DataProducao == dataProducao
            //            && w.TipoDEO == "Classificação de Ovos"
            //            && w.TipoOvo == tipoOvo.CodigoTipo)
            //        .FirstOrDefault();

            //        if (item == null && qtdClassificada > 0)
            //        {
            //            item = new LayoutDiarioExpedicaos();
            //            item.Granja = incubatorio;
            //            item.Incubatorio = incubatorio;
            //            item.NumIdentificacao = numIdentificacao;
            //            item.DataHoraCarreg = dataClassificacao;
            //            item.Nucleo = nucleo;
            //            item.Linhagem = linhagem;
            //            item.LoteCompleto = loteCompleto;
            //            item.Lote = lote;
            //            item.Galpao = galpao;
            //            item.DataProducao = dataProducao;
            //            item.Idade = idade;
            //            item.NumeroReferencia = dataProducao.DayOfYear.ToString();
            //            item.TipoDEO = "Classificação de Ovos";
            //            item.TipoOvo = tipoOvo.CodigoTipo;
            //            item.NFNum = "";
            //            item.Importado = "Conferido";
            //            item.Usuario = Session["login"].ToString();
            //            item.DataHora = DateTime.Now;
            //            item.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
            //            item.ResponsavelCarreg = "";
            //            item.ResponsavelReceb = "";
            //            item.GTANum = "";
            //            item.Lacre = "";
            //            item.Observacao = "Gerado automaticamente pela Classificação de Ovos - Tipo do Ovo: "
            //                + tipoOvo.DescricaoTipo;
            //            item.QtdDiferenca = 0;
            //            item.QtdeConferencia = 0;
            //        }

            //        if (item != null)
            //        {
            //            qtdGerada++;

            //            if (qtdClassificada == 0 && item.ID > 0)
            //                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            //            else
            //                item.QtdeOvos = qtdeClassificadaProporcional;

            //            saldoAjuste = saldoAjuste - qtdeClassificadaProporcional;

            //            if (item.ID == 0 && qtdClassificada > 0) bd.LayoutDiarioExpedicaos.AddObject(item);
            //        }

            //        dataProducao = dataProducao.AddDays(1);
            //    }
            //}

            //bd.SaveChanges();

            #endregion

            #region Volta Saldo para refazer rateio

            var listaVoltaSaldo = bd.LayoutDiarioExpedicaos
                .Where(w => w.Incubatorio == incubatorio
                    && w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == loteCompleto
                    && w.DataProducao >= dataProducaoInicial && w.DataProducao <= dataProducaoFinal
                    && w.TipoDEO == "Classificação de Ovos")
                .ToList();

            foreach (var item in listaVoltaSaldo)
            {
                item.Importado = "Não";
            }

            bd.SaveChanges();

            #endregion

            //qtdGerada = RateiaQtdeClassificadaPorLoteDataProducao(incubatorio, model, qtdTotalClassificada, dataProducaoInicial,
            //    dataProducaoFinal, loteCompleto, numIdentificacao, dataClassificacao, nucleo, linhagem, lote, galpao, idade);

            qtdGerada = RateiaQtdeClassificadaPorLoteDataProducaoFIFO(incubatorio, model, dataProducaoInicial,
                dataProducaoFinal, loteCompleto, numIdentificacao, dataClassificacao, nucleo, linhagem, lote, galpao, idade);

            //bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["ListaItensClassificacaoOvos"] = listItens;

            if (qtdGerada > 0)
            {
                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducaoInicial.ToShortDateString() + " a " + dataProducaoFinal.ToShortDateString()
                    + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                return View("ClassificacaoOvo");
            }
            else
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhuma classificação", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducaoInicial.ToShortDateString() + " a " + dataProducaoFinal.ToShortDateString()
                    + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                return View("ClassificacaoOvoItemPorPeriodo");
            }
        }

        public int RateiaQtdeClassificadaPorLoteDataProducao(string incubatorio, FormCollection model, int qtdTotalClassificada, 
            DateTime dataProducaoInicial, DateTime dataProducaoFinal, string loteCompleto, string numIdentificacao,
            DateTime dataClassificacao, string nucleo, string linhagem, string lote, string galpao, int idade)
        {
            HLBAPPEntities bd = new HLBAPPEntities();

            int qtdGerada = 0;

            var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                .Where(w => w.Unidade == incubatorio
                     && w.Origem == "Interna")
                .ToList();

            int saldoPeriodo = 0;
            DateTime data = dataProducaoInicial;
            while (data <= dataProducaoFinal)
            {
                saldoPeriodo = saldoPeriodo + RetornaSaldo(incubatorio, loteCompleto, data);
                data = data.AddDays(1);
            }

            List<LayoutDiarioExpedicaos> listaClassificacao = new List<LayoutDiarioExpedicaos>();
            List<LayoutDiarioExpedicaos> listaClassificacaoDelete = new List<LayoutDiarioExpedicaos>();

            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                DateTime dataProducao = dataProducaoInicial;

                int qtdClassificada = Convert.ToInt32(model[tipoOvo.CodigoTipo]);

                while (dataProducao <= dataProducaoFinal)
                {
                    int saldoLoteDataProducao = RetornaSaldo(incubatorio, loteCompleto, dataProducao);
                    decimal percRateio = saldoLoteDataProducao / Convert.ToDecimal(saldoPeriodo);
                    //int qtdeClassificadaProporcional = Convert.ToInt32(Math.Round(saldoLoteDataProducao * percRateio, 0));
                    int qtdeClassificadaProporcional = Convert.ToInt32(Math.Round(qtdClassificada * percRateio, 0));

                    var item = bd.LayoutDiarioExpedicaos
                        .Where(w => w.Incubatorio == incubatorio
                            && w.NumIdentificacao == numIdentificacao
                            && w.LoteCompleto == loteCompleto
                            && w.DataProducao == dataProducao
                            && w.TipoDEO == "Classificação de Ovos"
                            && w.TipoOvo == tipoOvo.CodigoTipo)
                        .FirstOrDefault();

                    if (item == null && qtdeClassificadaProporcional > 0)
                    {
                        item = new LayoutDiarioExpedicaos();
                        item.Granja = incubatorio;
                        item.Incubatorio = incubatorio;
                        item.NumIdentificacao = numIdentificacao;
                        item.DataHoraCarreg = dataClassificacao;
                        item.Nucleo = nucleo;
                        item.Linhagem = linhagem;
                        item.LoteCompleto = loteCompleto;
                        item.Lote = lote;
                        item.Galpao = galpao;
                        item.DataProducao = dataProducao;
                        item.Idade = idade;
                        item.NumeroReferencia = dataProducao.DayOfYear.ToString();
                        item.TipoDEO = "Classificação de Ovos";
                        item.TipoOvo = tipoOvo.CodigoTipo;
                        item.NFNum = "";
                        item.Importado = "Conferido";
                        item.Usuario = Session["login"].ToString();
                        item.DataHora = DateTime.Now;
                        item.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                        item.ResponsavelCarreg = "";
                        item.ResponsavelReceb = "";
                        item.GTANum = "";
                        item.Lacre = "";
                        item.Observacao = "Gerado automaticamente pela Classificação de Ovos - Tipo do Ovo: " + tipoOvo.DescricaoTipo;
                        item.QtdDiferenca = 0;
                        item.QtdeConferencia = 0;
                    }

                    if (qtdeClassificadaProporcional > 0)
                    {
                        item.QtdeOvos = qtdeClassificadaProporcional;
                        listaClassificacao.Add(item);
                    }

                    if (qtdeClassificadaProporcional == 0 && item != null)
                        if (item.ID > 0)
                            listaClassificacaoDelete.Add(item);

                    dataProducao = dataProducao.AddDays(1);
                }

                int qtdeRateadaTotalPorTipo = (int)listaClassificacao.Where(w => w.TipoOvo == tipoOvo.CodigoTipo).Sum(s => s.QtdeOvos);

                if (qtdeRateadaTotalPorTipo != qtdClassificada)
                {
                    int diferenca = qtdClassificada - qtdeRateadaTotalPorTipo;

                    var itemAjustaDiferenca = listaClassificacao.Where(w => w.TipoOvo == tipoOvo.CodigoTipo).FirstOrDefault();
                    itemAjustaDiferenca.QtdeOvos = itemAjustaDiferenca.QtdeOvos + diferenca;
                }
            }

            var qtdeTotal = listaClassificacao.Sum(s => s.QtdeOvos);

            foreach (var item in listaClassificacao)
            {
                qtdGerada++;
                if (item.ID == 0) bd.LayoutDiarioExpedicaos.AddObject(item);
            }

            foreach (var item in listaClassificacaoDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            return qtdGerada;
        }

        public int RateiaQtdeClassificadaPorLoteDataProducaoFIFO(string incubatorio, FormCollection model,
            DateTime dataProducaoInicial, DateTime dataProducaoFinal, string loteCompleto, string numIdentificacao,
            DateTime dataClassificacao, string nucleo, string linhagem, string lote, string galpao, int idade)
        {
            HLBAPPEntities bd = new HLBAPPEntities();

            int qtdGerada = 0;

            var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                .Where(w => w.Unidade == incubatorio
                     && w.Origem == "Interna")
                .ToList();
            
            int totalClassificado = 0;
            DateTime data = dataProducaoInicial;
            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                totalClassificado = totalClassificado + Convert.ToInt32(model[tipoOvo.CodigoTipo]);
            }

            int saldoClassificado = totalClassificado;

            List<LayoutDiarioExpedicaos> listaClassificacao = new List<LayoutDiarioExpedicaos>();
            List<LayoutDiarioExpedicaos> listaClassificacaoDelete = new List<LayoutDiarioExpedicaos>();

            DateTime dataProducao = dataProducaoInicial;

            while (dataProducao <= dataProducaoFinal)
            {
                int saldoLoteDataProducao = RetornaSaldo(incubatorio, loteCompleto, dataProducao);
                int qtdeCalcularRateio = 0;
                if (saldoClassificado > saldoLoteDataProducao)
                    qtdeCalcularRateio = saldoLoteDataProducao;
                else
                    qtdeCalcularRateio = saldoClassificado;

                if (saldoClassificado > 0)
                {
                    foreach (var tipoOvo in listaTipoClassificacaoOvo)
                    {
                        int qtdClassificada = Convert.ToInt32(model[tipoOvo.CodigoTipo]);
                        decimal percRateio = qtdClassificada / Convert.ToDecimal(totalClassificado);

                        int qtdeClassificadaProporcional = Convert.ToInt32(Math.Round(qtdeCalcularRateio * percRateio, 0));

                        var item = bd.LayoutDiarioExpedicaos
                            .Where(w => w.Incubatorio == incubatorio
                                && w.NumIdentificacao == numIdentificacao
                                && w.LoteCompleto == loteCompleto
                                && w.DataProducao == dataProducao
                                && w.TipoDEO == "Classificação de Ovos"
                                && w.TipoOvo == tipoOvo.CodigoTipo)
                            .FirstOrDefault();

                        if (item == null && qtdeClassificadaProporcional > 0)
                        {
                            item = new LayoutDiarioExpedicaos();
                            item.Granja = incubatorio;
                            item.Incubatorio = incubatorio;
                            item.NumIdentificacao = numIdentificacao;
                            item.DataHoraCarreg = dataClassificacao;
                            item.Nucleo = nucleo;
                            item.Linhagem = linhagem;
                            item.LoteCompleto = loteCompleto;
                            item.Lote = lote;
                            item.Galpao = galpao;
                            item.DataProducao = dataProducao;
                            item.Idade = idade;
                            item.NumeroReferencia = dataProducao.DayOfYear.ToString();
                            item.TipoDEO = "Classificação de Ovos";
                            item.TipoOvo = tipoOvo.CodigoTipo;
                            item.NFNum = "";
                            item.Importado = "Conferido";
                            item.Importado = "Não";
                            item.Usuario = Session["login"].ToString();
                            item.DataHora = DateTime.Now;
                            item.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                            item.ResponsavelCarreg = "";
                            item.ResponsavelReceb = "";
                            item.GTANum = "";
                            item.Lacre = "";
                            item.Observacao = "Gerado automaticamente pela Classificação de Ovos - Tipo do Ovo: " + tipoOvo.DescricaoTipo;
                            item.QtdDiferenca = 0;
                            item.QtdeConferencia = 0;
                        }

                        if (qtdeClassificadaProporcional > 0)
                        {
                            item.Importado = "Conferido";
                            item.QtdeOvos = qtdeClassificadaProporcional;
                            listaClassificacao.Add(item);
                        }

                        if (qtdeClassificadaProporcional == 0 && item != null)
                            if (item.ID > 0)
                                listaClassificacaoDelete.Add(item);

                        #region LOG

                        if (item != null)
                        {
                            HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                            if (item.ID == 0)
                            {
                                LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Inserido",
                                    item.Usuario, 0, "", "", item);
                                hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                            }
                            else
                            {
                                LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Alterado",
                                    item.Usuario, 0, "", "", item);
                                hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                            }

                            hlbappLOG.SaveChanges();
                        }

                        #endregion
                    }

                    //int qtdeRateadaTotalPorData = (int)listaClassificacao.Where(w => w.DataProducao == dataProducao).Sum(s => s.QtdeOvos);

                    //if (qtdeRateadaTotalPorData != qtdeCalcularRateio)
                    //{
                    //    int diferenca = qtdeCalcularRateio - qtdeRateadaTotalPorData;

                    //    var itemAjustaDiferenca = listaClassificacao.Where(w => w.DataProducao == dataProducao).FirstOrDefault();
                    //    itemAjustaDiferenca.QtdeOvos = itemAjustaDiferenca.QtdeOvos + diferenca;
                    //}

                    saldoClassificado = saldoClassificado - qtdeCalcularRateio;
                }

                dataProducao = dataProducao.AddDays(1);
            }

            #region Ajuste do Rateio depois de gerar tudo para jogar para a quantidade correta.

            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                var totalClassificadoPorTipo = Convert.ToInt32(model[tipoOvo.CodigoTipo]);
                int qtdeRateadaTotalPorTipo = (int)listaClassificacao.Where(w => w.TipoOvo == tipoOvo.CodigoTipo).Sum(s => s.QtdeOvos);

                if (totalClassificadoPorTipo != qtdeRateadaTotalPorTipo)
                {
                    int diferenca = totalClassificadoPorTipo - qtdeRateadaTotalPorTipo;

                    var itemAjustaDiferenca = listaClassificacao.Where(w => w.TipoOvo == tipoOvo.CodigoTipo).OrderBy(o => o.DataProducao).FirstOrDefault();
                    if (itemAjustaDiferenca != null)
                        itemAjustaDiferenca.QtdeOvos = itemAjustaDiferenca.QtdeOvos + diferenca;
                }
            }

            #endregion

            #region Ajustar as datas de produção onde a quantidade está maior que o saldo

            var listaLoteData = listaClassificacao
                .GroupBy(g => new { g.LoteCompleto, g.DataProducao })
                .Select(s => new { s.Key.LoteCompleto, s.Key.DataProducao })
                .OrderBy(o => o.LoteCompleto).ThenBy(t => t.DataProducao)
                .ToList();

            foreach (var item in listaLoteData)
            {
                int saldoLoteDataProducao = RetornaSaldo(incubatorio, item.LoteCompleto, item.DataProducao);
                var qtdeLoteDataClassificada = listaClassificacao
                    .Where(w => w.LoteCompleto == item.LoteCompleto && w.DataProducao == item.DataProducao)
                    .Sum(s => s.QtdeOvos);

                if (qtdeLoteDataClassificada != saldoLoteDataProducao)
                {
                    var diferenca = qtdeLoteDataClassificada - saldoLoteDataProducao;
                    var itemC = listaClassificacao
                        .Where(w => w.LoteCompleto == item.LoteCompleto && w.DataProducao == item.DataProducao).FirstOrDefault();
                    //var proximaData = itemC.DataProducao.AddDays(1);
                    var itemO = listaClassificacao
                        //.Where(w => w.LoteCompleto == itemC.LoteCompleto && w.DataProducao == proximaData && w.TipoOvo == itemC.TipoOvo).FirstOrDefault();
                        .Where(w => w.LoteCompleto == itemC.LoteCompleto && w.DataProducao > itemC.DataProducao && w.TipoOvo == itemC.TipoOvo)
                        .OrderBy(o => o.DataProducao)
                        .FirstOrDefault();

                    if (itemO != null)
                    {
                        itemC.QtdeOvos = itemC.QtdeOvos - diferenca;
                        itemO.QtdeOvos = itemO.QtdeOvos + diferenca;
                    }
                    else
                    {
                        #region Verifica se tem na próxima data para inserir

                        saldoLoteDataProducao = RetornaSaldo(incubatorio, item.LoteCompleto, item.DataProducao.AddDays(1));

                        if (saldoLoteDataProducao >= diferenca && diferenca > 0)
                        {
                            TIPO_CLASSFICACAO_OVO tipoOvo = bd.TIPO_CLASSFICACAO_OVO
                                .Where(w => w.Unidade == itemC.Incubatorio && w.CodigoTipo == itemC.TipoOvo)
                                .FirstOrDefault();

                            LayoutDiarioExpedicaos itemNovo = new LayoutDiarioExpedicaos();
                            itemNovo.Granja = itemC.Granja;
                            itemNovo.Incubatorio = itemC.Incubatorio;
                            itemNovo.NumIdentificacao = itemC.NumIdentificacao;
                            itemNovo.DataHoraCarreg = itemC.DataHoraCarreg;
                            itemNovo.Nucleo = itemC.Nucleo;
                            itemNovo.Linhagem = itemC.Linhagem;
                            itemNovo.LoteCompleto = itemC.LoteCompleto;
                            itemNovo.Lote = itemC.Lote;
                            itemNovo.Galpao = itemC.Galpao;
                            itemNovo.DataProducao = itemC.DataProducao.AddDays(1);
                            itemNovo.Idade = itemC.Idade;
                            itemNovo.NumeroReferencia = itemNovo.DataProducao.DayOfYear.ToString();
                            itemNovo.TipoDEO = "Classificação de Ovos";
                            itemNovo.TipoOvo = itemC.TipoOvo;
                            itemNovo.NFNum = "";
                            itemNovo.Importado = "Conferido";
                            itemNovo.Importado = "Não";
                            itemNovo.Usuario = Session["login"].ToString();
                            itemNovo.DataHora = DateTime.Now;
                            itemNovo.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                            itemNovo.ResponsavelCarreg = "";
                            itemNovo.ResponsavelReceb = "";
                            itemNovo.GTANum = "";
                            itemNovo.Lacre = "";
                            itemNovo.Observacao = "Gerado automaticamente pela Classificação de Ovos - Tipo do Ovo: " + tipoOvo.DescricaoTipo;
                            itemNovo.QtdDiferenca = 0;
                            itemNovo.QtdeConferencia = 0;
                            itemNovo.Importado = "Conferido";
                            itemNovo.QtdeOvos = diferenca;
                            listaClassificacao.Add(itemNovo);

                            itemC.QtdeOvos = itemC.QtdeOvos - diferenca;
                        }
                        else
                        {
                            itemO = listaClassificacao
                                .Where(w => w.LoteCompleto == itemC.LoteCompleto && w.DataProducao > itemC.DataProducao)
                                .OrderBy(o => o.DataProducao)
                                .FirstOrDefault();

                            if (itemO != null)
                            {
                                itemC.QtdeOvos = itemC.QtdeOvos - diferenca;
                                itemO.QtdeOvos = itemO.QtdeOvos + diferenca;
                            }
                        }

                        #endregion
                    }
                }
            }

            #endregion

            foreach (var item in listaClassificacao)
            {
                qtdGerada++;
                if (item.ID == 0) bd.LayoutDiarioExpedicaos.AddObject(item);
            }

            foreach (var item in listaClassificacaoDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            return qtdGerada;
        }

        public ActionResult DeleteClassificacaoOvoItemPorPeriodo(string incubatorio, string numIdentificacao,
            string lote, DateTime dataProducaoInicial, DateTime dataProducaoFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao >= dataProducaoInicial
                    && w.DataProducao <= dataProducaoFinal)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.Granja == incubatorio
                    && w.NumIdentificacao == numIdentificacao)
                .ToList();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Classificações de Ovos", Session["language"].ToString()) + " - "
                + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                + lote + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                + dataProducaoInicial.ToShortDateString() + " " + am.GetTextOnLanguage("até", Session["language"].ToString()) + " "
                + dataProducaoFinal.ToShortDateString()
                + " " + am.GetTextOnLanguage("excluídas com sucesso!", Session["language"].ToString());

            Session["ListaItensClassificacaoOvos"] = listItens;

            return View("ClassificacaoOvo");
        }

        #endregion

        #region Métodos Classificação dos Ovos

        public List<SelectListItem> CarregaListaIncubatoriosCO(string unidadeSelecionada, bool destino, bool consideraComercio)
        {
            FLIPDataSetMobile.HATCHERY_CODESDataTable hDT = new FLIPDataSetMobile.HATCHERY_CODESDataTable();
            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();

            hTA.Fill(hDT);

            List<SelectListItem> items = new List<SelectListItem>();

            bdApoloEntities bd = new bdApoloEntities();

            var isGranja = bd.EMPRESA_FILIAL.Where(w => w.USERFLIPCod == unidadeSelecionada && w.USERTipoUnidadeFLIP == "Granja").Count();

            foreach (var item in hDT)
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                {
                    if (destino)
                    {
                        if (item.HATCH_LOC != unidadeSelecionada)
                            items.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                        else
                        {
                            if (isGranja == 0 && consideraComercio)
                                items.Add(new SelectListItem
                                {
                                    Text = item.HATCH_DESC + " - " + am.GetTextOnLanguage("OVOS DE COMÉRCIO", Session["language"].ToString()),
                                    Value = item.HATCH_LOC + "C",
                                    Selected = false
                                });
                        }
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                    }
                }
            }

            return items;
        }

        public List<SelectListItem> CarregaListaNucleosFLIP(string incubatorio)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            #region Load Hatch Loc

            MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                new MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter();
            MvcAppHyLinedoBrasil.Data.FLIPDataSet.HATCHERY_CODESDataTable hcDT = 
                new MvcAppHyLinedoBrasil.Data.FLIPDataSet.HATCHERY_CODESDataTable();
            hcTA.FillByHatchLoc(hcDT, incubatorio);

            #endregion

            if (hcDT.FirstOrDefault().COMPANY == "HYBR")
            {
                #region HYBR

                FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
                FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                fTA.FillFarmsDEO(fDT);

                foreach (var item in fDT.ToList())
                {
                    items.Add(new SelectListItem { Text = item.FARM_ID, Value = item.FARM_ID, Selected = false });
                }

                #endregion
            }
            else if (hcDT.FirstOrDefault().COMPANY == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCKSDataTable fDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCKSDataTable();
                fTA.Fill(fDT);

                var listaNucleos = fDT.GroupBy(g => g.FARM_ID).ToList();

                foreach (var item in listaNucleos)
                {
                    items.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
                }

                #endregion
            }

            return items.OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> CarregaLotesFLIP(string incubatorio, string nucleo)
        {
            #region Load Hatch Loc

            MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                new MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter();
            MvcAppHyLinedoBrasil.Data.FLIPDataSet.HATCHERY_CODESDataTable hcDT =
                new MvcAppHyLinedoBrasil.Data.FLIPDataSet.HATCHERY_CODESDataTable();
            hcTA.FillByHatchLoc(hcDT, incubatorio);

            #endregion

            List<SelectListItem> items = new List<SelectListItem>();
            Session["listLotes"] = new List<Lotes>();
            List<Lotes> listaLotes = new List<Lotes>();

            if (hcDT.FirstOrDefault().COMPANY == "HYBR")
            {
                #region HYBR

                MvcAppHyLinedoBrasil.Data.FLIPDataSet flip = new MvcAppHyLinedoBrasil.Data.FLIPDataSet();
                FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();
                string location = "PP";

                flocks.FillActivesByFarm(flip.FLOCKS, "HYBR", "BR", location, nucleo);

                List<FLIPDataSet.FLOCKSRow> flocksTable = flip.FLOCKS
                        .OrderBy(o => o.NUM_1).ToList();

                for (int i = 0; i < flocksTable.Count; i++)
                {
                    string bkp = "";
                    if (flocksTable[i].FLOCK_ID.ToString().Contains("K"))
                        bkp = "-BKP";

                    if (items.Where(t => t.Text == flocksTable[i].NUM_1.ToString() + bkp).Count() == 0)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = flocksTable[i].NUM_1.ToString() + bkp,
                            Value = flocksTable[i].NUM_1.ToString() + bkp,
                            Selected = false
                        });
                    }
                    if (!flocksTable[i].IsHATCH_DATENull())
                    {
                        listaLotes.Add(new Lotes
                        {
                            Granja = flocksTable[i].FARM_ID,
                            Linhagem = flocksTable[i].VARIETY,
                            LoteCompleto = flocksTable[i].FLOCK_ID,
                            NumeroLote = flocksTable[i].NUM_1.ToString() + bkp,
                            DataNascimento = flocksTable[i].HATCH_DATE,
                            Location = flocksTable[i].LOCATION,
                            Galpao = flocksTable[i].NUM_2.ToString()
                        });
                    }
                }

                Session["listLotes"] = listaLotes;

                #endregion
            }
            else if (hcDT.FirstOrDefault().COMPANY == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKS flip = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter flocks =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();

                flocks.Fill(flip.FLOCKS);

                var listFLIPLotes = flip.FLOCKS
                    .Where(w => w.FARM_ID == nucleo
                        && w.ACTIVE == 1)
                    .ToList();

                List<ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCKSRow> flocksTable = listFLIPLotes
                        .OrderBy(o => o.NUM_1).ToList();

                for (int i = 0; i < flocksTable.Count; i++)
                {
                    if (items.Where(t => t.Text == flocksTable[i].NUM_1.ToString()).Count() == 0)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = flocksTable[i].NUM_1.ToString(),
                            Value = flocksTable[i].NUM_1.ToString(),
                            Selected = false
                        });
                    }
                    listaLotes.Add(new Lotes
                    {
                        Granja = flocksTable[i].FARM_ID,
                        Linhagem = flocksTable[i].VARIETY,
                        //LoteCompleto = flocksTable[i].FLOCK_ID,
                        LoteCompleto = flocksTable[i].FLOCK_ID.Substring(0, 6) + flocksTable[i].FLOCK_ID.Substring(7, 3),
                        NumeroLote = flocksTable[i].NUM_1.ToString(),
                        DataNascimento = flocksTable[i].HATCH_DATE,
                        Location = flocksTable[i].LOCATION,
                        Galpao = flocksTable[i].TEXT_2.ToString()
                    });
                }

                Session["listLotes"] = listaLotes;

                #endregion
            }

            return items.OrderBy(o => o.Text).ToList();
        }

        public List<Lotes> CarregaGalpoesFLIP(string id)
        {
            List<Lotes> items = new List<Lotes>();

            List<SelectListItem> itemsGlp = new List<SelectListItem>();

            string galpao = "";

            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

            var loteSelecionado = listaLotes
                .Where(s => s.NumeroLote == id)
                .OrderBy(o => o.Galpao)
                .ToList();

            foreach (var lote in loteSelecionado)
            {
                Session["loteEscolhido"] = lote.LoteCompleto;
                int tamanho = lote.LoteCompleto.Length - 1;

                if ((lote.Galpao != null) && (lote.Galpao != ""))
                    galpao = lote.Galpao;
                else
                    galpao = "";

                if (galpao.Equals(""))
                {
                    for (int i = tamanho; i >= 0; i--)
                    {
                        double Num;
                        bool isNum = double.TryParse(lote.LoteCompleto.Substring(i, 1), out Num);

                        if (isNum)
                        {
                            galpao = "0" + lote.LoteCompleto.Substring(i, 1);
                            items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                            itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });

                            foreach (var item in listaLotes)
                            {
                                if (item.LoteCompleto == lote.LoteCompleto)
                                {
                                    item.Galpao = galpao;
                                }
                            }
                        }
                    }
                }
                else
                {
                    items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                    itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });
                }
            }

            Session["listLotes"] = listaLotes;

            return items;
        }

        public List<SelectListItem> CarregaDDLGalpoes(string id)
        {
            List<Lotes> items = new List<Lotes>();

            List<SelectListItem> itemsGlp = new List<SelectListItem>();

            string galpao = "";

            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

            var loteSelecionado = listaLotes
                .Where(s => s.NumeroLote == id)
                .OrderBy(o => o.Galpao)
                .ToList();

            foreach (var lote in loteSelecionado)
            {
                Session["loteEscolhido"] = lote.LoteCompleto;
                int tamanho = lote.LoteCompleto.Length - 1;

                if ((lote.Galpao != null) && (lote.Galpao != ""))
                    galpao = lote.Galpao;
                else
                    galpao = "";

                if (galpao.Equals(""))
                {
                    for (int i = tamanho; i >= 0; i--)
                    {
                        double Num;
                        bool isNum = double.TryParse(lote.LoteCompleto.Substring(i, 1), out Num);

                        if (isNum)
                        {
                            galpao = "0" + lote.LoteCompleto.Substring(i, 1);
                            items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                            itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });

                            foreach (var item in listaLotes)
                            {
                                if (item.LoteCompleto == lote.LoteCompleto)
                                {
                                    item.Galpao = galpao;
                                }
                            }
                        }
                    }
                }
                else
                {
                    items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                    itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });
                }
            }

            Session["listLotes"] = listaLotes;
            
            return itemsGlp;
        }

        public string GetFieldValueHatchCodeTable(string hatchLoc, string field)
        {
            string fieldValue = "";

            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                new MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();
            FLIPDataSetMobile.HATCHERY_CODESDataTable hDT =
                new FLIPDataSetMobile.HATCHERY_CODESDataTable();
            hTA.FillByHatchLoc(hDT, hatchLoc);
            if (hDT.Count > 0)
            {
                var hc = hDT.FirstOrDefault();
                fieldValue = hc[field].ToString();
            }

            return fieldValue;
        }

        #endregion

        #region Métodos p/ JavaScript

        [HttpPost]
        public ActionResult CarregaLotesJS(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<SelectListItem> items = CarregaLotesFLIP(Session["incubatorioSelecionado"].ToString(), id);

            return Json(items);
        }

        [HttpPost]
        public ActionResult CarregaGalpoesJS(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return Json(CarregaGalpoesFLIP(id));
        }

        [HttpPost]
        public ActionResult RetornaLoteCompletoJS(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Lotes retornoLote;

            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

            retornoLote = listaLotes
                .Where(l => l.NumeroLote == id)
                .FirstOrDefault();

            Session["loteCompleto"] = retornoLote.LoteCompleto;

            return Json(retornoLote);
        }

        [HttpPost]
        public ActionResult RetornaSaldoLoteJS(string numeroLote, string dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime data = new DateTime();
            int age = 0;
            int saldo = 0;
            if (DateTime.TryParse(dataProducao, out data))
            {
                //DateTime data = Convert.ToDateTime(dataProducao);
                string incubatorio = Session["incubatorioSelecionado"].ToString();

                List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
                var listaLotesPorGalpao = listaLotes
                    .Where(l => l.NumeroLote == numeroLote)
                    .FirstOrDefault();

                saldo = RetornaSaldo(incubatorio, listaLotesPorGalpao.LoteCompleto, data);

                #region Carrega qtde Sem Conferir

                HLBAPPEntities hlbapp = new HLBAPPEntities();
                int existe = hlbapp.LayoutDiarioExpedicaos
                    .Where(e => e.LoteCompleto == listaLotesPorGalpao.LoteCompleto
                        && e.DataProducao == data
                        && e.Granja == incubatorio
                        && e.Importado != "Conferido"
                        && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                    .Count();

                if (existe > 0)
                {
                    existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == listaLotesPorGalpao.LoteCompleto
                            && e.DataProducao == data
                            && e.Granja == incubatorio
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                        .Sum(s => s.QtdeOvos));

                    saldo = saldo - existe;
                }

                #endregion

                age = ((data - listaLotesPorGalpao.DataNascimento).Days) / 7;
            }

            List<string> retorno = new List<string>();
            retorno.Add(saldo.ToString());
            retorno.Add(age.ToString());

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult RetornaSaldoLotePorTipoOvoJS(string numeroLote, string dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime data = new DateTime();
            List<Lotes> listaTipoOvos = new List<Lotes>();

            if (DateTime.TryParse(dataProducao, out data))
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
                var listaLotesPorGalpao = listaLotes
                    .Where(l => l.NumeroLote == numeroLote)
                    .FirstOrDefault();
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                var listaTipoOvoIncubavelUnidade = hlbapp.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio && w.AproveitamentoOvo == "Incubável" && w.Origem == "Interna")
                    .ToList();

                var itemSemClassificarSemSalvar = hlbapp.LayoutDiarioExpedicaos
                    .Where(w => w.LoteCompleto == numeroLote
                        && w.DataProducao == data
                        && w.TipoDEO == "Transf. Ovos Classificados"
                        && w.Granja == incubatorio
                        && w.Importado != "Conferido").ToList();
                int qtdeSemClassificarSemSalvar = 0;
                if (itemSemClassificarSemSalvar.Count > 0) qtdeSemClassificarSemSalvar = Convert.ToInt32(itemSemClassificarSemSalvar.Sum(s => s.QtdeOvos));

                Lotes novoLoteSC = new Lotes();
                novoLoteSC.TipoOvo = incubatorio;
                novoLoteSC.DescricaoTipoOvo = "Não Classificado";
                novoLoteSC.Saldo = (RetornaSaldo(incubatorio, listaLotesPorGalpao.LoteCompleto, data) - qtdeSemClassificarSemSalvar);
                novoLoteSC.SaldoString = String.Format("{0:N0}", novoLoteSC.Saldo);
                novoLoteSC.Idade = ((data - listaLotesPorGalpao.DataNascimento).Days) / 7;
                listaTipoOvos.Add(novoLoteSC);

                foreach (var tipoOvo in listaTipoOvoIncubavelUnidade)
                {
                    var itemSemSalvar = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.LoteCompleto == numeroLote
                            && w.DataProducao == data
                            && w.TipoDEO == "Transf. Ovos Classificados"
                            && w.Granja == tipoOvo.CodigoTipo
                            && w.Importado != "Conferido").ToList();

                    int qtdeSemSalvar = 0;
                    if (itemSemSalvar.Count > 0) qtdeSemSalvar = Convert.ToInt32(itemSemSalvar.Sum(s => s.QtdeOvos));

                    Lotes novoLote = new Lotes();
                    novoLote.TipoOvo = tipoOvo.CodigoTipo;
                    novoLote.DescricaoTipoOvo = tipoOvo.DescricaoTipo;
                    novoLote.Saldo = (RetornaSaldo(tipoOvo.CodigoTipo, listaLotesPorGalpao.LoteCompleto, data) - qtdeSemSalvar);
                    novoLote.SaldoString = String.Format("{0:N0}", novoLote.Saldo);
                    novoLote.Idade = ((data - listaLotesPorGalpao.DataNascimento).Days) / 7;
                    listaTipoOvos.Add(novoLote);
                }
            }

            return Json(listaTipoOvos);
        }

        [HttpPost]
        public ActionResult RetornaSaldoLoteJSPorPeriodo(string numeroLote, string dataProducaoInicial, string dataProducaoFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime dataI = new DateTime();
            DateTime dataF = new DateTime();
            int age = 0;
            int saldo = 0;
            if (DateTime.TryParse(dataProducaoInicial, out dataI) && DateTime.TryParse(dataProducaoFinal, out dataF))
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();

                List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
                var listaLotesPorGalpao = listaLotes
                    .Where(l => l.NumeroLote == numeroLote)
                    .FirstOrDefault();

                #region Carrega a primeira data com estoque disponível para não dar problema de lentidão

                DateTime data = dataI;
                DateTime dataAtual = DateTime.Today;
                
                var primeiraDataSaldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                    .Where(w => w.Local == incubatorio && w.LoteCompleto == listaLotesPorGalpao.LoteCompleto 
                        && w.DataProducao >= dataI && w.Qtde > 0)
                    .OrderBy(o => o.DataProducao)
                    .FirstOrDefault();

                if (primeiraDataSaldo != null)
                    data = primeiraDataSaldo.DataProducao;
                else
                    data = dataAtual.AddMonths(-6);

                #endregion

                while (data <= dataF)
                {
                    saldo = saldo + RetornaSaldo(incubatorio, listaLotesPorGalpao.LoteCompleto, data);
                    data = data.AddDays(1);
                }

                #region Carrega qtde Sem Conferir

                int existe = hlbapp.LayoutDiarioExpedicaos
                    .Where(e => e.LoteCompleto == listaLotesPorGalpao.LoteCompleto
                        && e.DataProducao >= dataI && e.DataProducao <= dataF
                        && e.Granja == incubatorio
                        && e.Importado != "Conferido"
                        && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                    .Count();

                if (existe > 0)
                {
                    existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == listaLotesPorGalpao.LoteCompleto
                            && e.DataProducao >= dataI && e.DataProducao <= dataF
                            && e.Granja == incubatorio
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                        .Sum(s => s.QtdeOvos));

                    saldo = saldo - existe;
                }

                #endregion

                age = ((data - listaLotesPorGalpao.DataNascimento).Days) / 7;
            }

            List<string> retorno = new List<string>();
            retorno.Add(saldo.ToString());
            retorno.Add(age.ToString());

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult RetornaSaldoLotePorTipoOvoDescarteJS(string numeroLote, string dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime data = new DateTime();
            List<Lotes> listaTipoOvos = new List<Lotes>();

            if (DateTime.TryParse(dataProducao, out data))
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
                var listaLotesPorGalpao = listaLotes
                    .Where(l => l.NumeroLote == numeroLote)
                    .FirstOrDefault();
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                var listaTipoOvoIncubavelUnidade = hlbapp.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio && w.AproveitamentoOvo == "Incubável" && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoOvoIncubavelUnidade)
                {
                    var itemSemSalvar = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.LoteCompleto == numeroLote
                            && w.DataProducao == data
                            && w.TipoDEO == "Ovos Classfic. p/ Comércio"
                            && w.Granja == tipoOvo.CodigoTipo
                            && w.Importado != "Conferido").ToList();

                    int qtdeSemSalvar = 0;
                    if (itemSemSalvar.Count > 0) qtdeSemSalvar = Convert.ToInt32(itemSemSalvar.Sum(s => s.QtdeOvos));

                    Lotes novoLote = new Lotes();
                    novoLote.TipoOvo = tipoOvo.CodigoTipo;
                    novoLote.DescricaoTipoOvo = tipoOvo.DescricaoTipo;
                    novoLote.Saldo = (RetornaSaldo(tipoOvo.CodigoTipo, listaLotesPorGalpao.LoteCompleto, data) - qtdeSemSalvar);
                    novoLote.SaldoString = String.Format("{0:N0}", novoLote.Saldo);
                    novoLote.Idade = ((data - listaLotesPorGalpao.DataNascimento).Days) / 7;
                    listaTipoOvos.Add(novoLote);
                }
            }

            return Json(listaTipoOvos);
        }

        #endregion

        #endregion

        #region Relatório Excel

        public ActionResult GerarRelatorioSinteticoDEO()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            string granja = "";
            if (Session["granjaSelecionada"] != null)
                granja = Session["granjaSelecionada"].ToString();
            AtualizaGranjaSelecionada(granja);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString());
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());
            }

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\DEO";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\DEO\\Relatorio_DEO_Sintetico_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_DEO_Sintetico_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioDEOSinteticoExcel(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, granja);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_DEO_Sintetico_" + granja + "_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioDEOSinteticoExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string empresa)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\DEO\\Relatorio_DEO_Sintetico.xlsx", destino);

            object oMissing = System.Reflection.Missing.Value;

            Process[] P0, P1;
            P0 = Process.GetProcessesByName("Excel");

            Excel.Application oExcel = new Excel.Application();

            int I, J;
            P1 = Process.GetProcessesByName("Excel");
            I = 0;
            if (P1.Length > 1)
            {
                for (I = 0; I < P1.Length; I++)
                {
                    for (J = 0; J < P0.Length; J++)
                        if (P0[J].Id == P1[I].Id) break;
                    if (J == P0.Length) break;
                }
            }
            Process P = P1[I];

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            string filtroGranjas = "";
            List<SelectListItem> listaGranjas = (List<SelectListItem>)Session["ListaGranjas"];
            if (empresa != "")
            {
                filtroGranjas = empresa;
            }
            else
            {
                foreach (var item in listaGranjas)
                {
                    filtroGranjas = filtroGranjas + item.Value;
                }
            }

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["DEOs"];
            worksheet.Cells[4, 2] = "Origem: " + listaGranjas.Where(w => w.Selected == true).FirstOrDefault().Text;

            #region SQL Exibição

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Rel_DEO_Agrupado V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd") + " 00:00:00";
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd") + " 23:59:59";

            string commandTextCHICCondicaoParametros =
                    "V.[Data Hora Carregamento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                //"(V.[Cód. Unidade] = '" + empresa + "' or '" + empresa + "' = '') ";
                    "CHARINDEX(V.Origem, '" + filtroGranjas + "') > 0 ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "V.[Data Hora Carregamento], V.Origem";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("DEOs"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros +
                        commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
            }

            oBook.RefreshAll();

            // Quit Excel and clean up.
            oBook.Close(true, oMissing, oMissing);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
            oBook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
            oBooks = null;
            oExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            oExcel = null;

            P.Kill();

            GC.Collect();

            return destino;
        }

        #endregion

        #region Métodos p/ DropDown

        public void CarregaListaNucleos()
        {
            if (Session["usuario"].ToString() != "0")
            {
                List<SelectListItem> items = new List<SelectListItem>();

                //nucleos.FillFarms(flip.FLOCKS1);
                FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
                FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                fTA.FillFarmsDEO(fDT);

                string granja = Session["granjaSelecionada"].ToString();    
                Session["location"] = "";
                string location = "";

                MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresa =
                    bdApolo.EMPRESA_FILIAL.Where(e => e.USERFLIPCod == granja
                        || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod 
                            && c.EmpFilCertificNum == granja))
                    .FirstOrDefault();

                if (empresa != null)
                    if (empresa.USERTipoUnidadeFLIP.Equals("Incubatório"))
                    {
                        LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == granja)
                            .FirstOrDefault();
                        location = locArmaz.USERGeracaoFLIP;
                        granja = "";
                    }
                    else
                    {
                        //location = flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;
                        if (fDT.Where(f => f.FARM_ID.StartsWith(granja)).Count() > 0)
                            location = fDT.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;
                        else
                            location = fDT.FirstOrDefault().LOCATION;
                    }
                else
                {
                    //location = flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;
                    if (fDT.Where(f => f.FARM_ID.StartsWith(granja)).Count() > 0)
                        location = fDT.Where(f => f.FARM_ID.StartsWith(granja)).FirstOrDefault().LOCATION;
                    else
                        location = fDT.FirstOrDefault().LOCATION;
                }

                //for (int i = 0; i < flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja)).Count(); i++)
                //{
                //    items.Add(new SelectListItem { Text = flip.FLOCKS1[i].FARM_ID, Value = flip.FLOCKS1[i].FARM_ID, Selected = false });
                //}

                //foreach (var item in flip.FLOCKS1.Where(f => f.FARM_ID.StartsWith(granja) && f.LOCATION == location).ToList())
                foreach (var item in fDT.Where(f => f.FARM_ID.StartsWith(granja) && f.LOCATION == location).ToList())
                {
                    items.Add(new SelectListItem { Text = item.FARM_ID, Value = item.FARM_ID, Selected = false });
                }

                Session["ListaNucleos"] = items;
                Session["location"] = location;

                Session["ListaLotes"] = new List<SelectListItem>();
                Session["ListaGalpoes"] = new List<SelectListItem>();

                //CarregaLotes(items[0].Text);

                //List<SelectListItem> itemsGalpoes = new List<SelectListItem>();

                //itemsGalpoes.Add(new SelectListItem { Text = "01", Value = "01", Selected = false });
                //itemsGalpoes.Add(new SelectListItem { Text = "02", Value = "02", Selected = false });
                //itemsGalpoes.Add(new SelectListItem { Text = "03", Value = "03", Selected = false });
                //itemsGalpoes.Add(new SelectListItem { Text = "04", Value = "04", Selected = false });

                //Session["ListaGalpoes"] = itemsGalpoes;
            }
        }

        public void CarregaListaGranjas(bool isConferencia)
        {
            if (Session["usuario"].ToString() != "0")
            {
                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem
                {
                    Text = "(Todas)",
                    Value = "",
                    Selected = false
                });

                string login = Session["login"].ToString().ToUpper();

                if (login.Equals("PALVES"))
                    login = "RIOSOFT";

                var listaFiliais = bdApolo.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod != null && e.USERFLIPCod != ""
                        && bdApolo.EMP_FIL_USUARIO.Any(u => u.UsuCod == login && u.EmpCod == e.EmpCod)
                        //&& (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
                        && (e.USERTipoUnidadeFLIP == "Granja"))
                    /*.GroupJoin(
                        bdApolo.EMP_FILIAL_CERTIFICACAO,
                        e => e.EmpCod,
                        c => c.EmpCod,
                        (e, c) => new { EMPRESA_FILIAL = e, EMP_FILIAL_CERTIFICACAO = c })*/
                    .SelectMany(
                        x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
                        (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
                    .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
                    .ToList();

                //var listaFiliais02 = bdApolo

                foreach (var item in listaFiliais)
                {
                    bool selected = false;
                    if ((listaFiliais.IndexOf(item).Equals(0)) && (Session["granjaSelecionada"] == null))
                    {
                        selected = true;
                        Session["granjaSelecionada"] = item.EMPRESA_FILIAL.USERFLIPCod;
                    }
                    string codFLIP = "";
                    if (item.EMP_FILIAL_CERTIFICACAO == null)
                        codFLIP = item.EMPRESA_FILIAL.USERFLIPCod;
                    else
                        codFLIP = item.EMP_FILIAL_CERTIFICACAO.EmpFilCertificNum;

                    bool localOvosComercio = false;
                    if (codFLIP.Length == 3)
                        if (codFLIP.Substring(2, 1) == "C")
                            localOvosComercio = true;

                    string ovosComercio = "";
                    if (localOvosComercio) ovosComercio = " - SALA OVOS DE COMÉRCIO";

                    items.Add(new SelectListItem
                    {
                        //Text = codFLIP + " - " + item.EMPRESA_FILIAL.EmpNome + ovosComercio,
                        Text = codFLIP + " - " + item.EMPRESA_FILIAL.USERRelatorio + ovosComercio,
                        Value = codFLIP,
                        Selected = selected
                    });
                }

                var listaEntidadesTerceiros = apoloService.ENTIDADE
                    .Where(e => apoloService.ENTIDADE1.Any(e1 => e1.EntCod == e.EntCod && e1.USERFLIPCodigo != null
                        && apoloService.ENT_CATEG.Any(c => c.EntCod == e1.EntCod && c.CategCodEstr == "07.01"
                            && apoloService.CATEG_USUARIO.Any(u => u.CategCodEstr == c.CategCodEstr && u.UsuCod == login))))
                    .OrderBy(e => e.EntNomeFant)
                    .ToList();

                //if (isConferencia)
                //{
                foreach (var item in listaEntidadesTerceiros)
                {
                    ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1.Where(e1 => e1.EntCod == item.EntCod).FirstOrDefault();
                    items.Add(new SelectListItem
                    {
                        Text = entidade1.USERFLIPCodigo + " - " + item.EntNomeFant,
                        Value = entidade1.USERFLIPCodigo,
                        Selected = false
                    });

                    bool selected = false;
                    if ((listaEntidadesTerceiros.IndexOf(item).Equals(0)) && (Session["granjaSelecionada"] == null))
                    {
                        selected = true;
                        Session["granjaSelecionada"] = entidade1.USERFLIPCodigo;
                    }
                }
                //}

                #region Carrega Nova Lista Incubatórios

                var listaIncubatorios = CarregaListaIncubatoriosCO(Session["granjaSelecionada"].ToString(), false, false);
                foreach (var item in listaIncubatorios)
                {
                    bool selected = false;
                    if ((listaIncubatorios.IndexOf(item).Equals(0)) && (Session["granjaSelecionada"] == null))
                    {
                        selected = true;
                        Session["granjaSelecionada"] = item.Value;
                    }

                    items.Add(new SelectListItem
                    {
                        Text = item.Value + " - " + item.Text.ToUpper(),
                        Value = item.Value,
                        Selected = selected
                    });

                    // Ovos de Comércio
                    items.Add(new SelectListItem
                    {
                        Text = item.Value + "C - " + item.Text.ToUpper() + " - " + am.GetTextOnLanguage("OVOS DE COMÉRCIO", Session["language"].ToString()),
                        Value = item.Value + "C",
                        Selected = selected
                    });
                }

                #endregion

                if (Session["granjaSelecionada"] == null)
                    Session["incubatorioSelecionadoNome"] = items.Where(w => w.Selected == true).FirstOrDefault().Text;
                else
                {
                    string unidadeSelecionada = Session["granjaSelecionada"].ToString();
                    Session["incubatorioSelecionadoNome"] = items.Where(w => w.Value == unidadeSelecionada).FirstOrDefault().Text;
                }

                Session["ListaGranjas"] = items.OrderBy(o => o.Text).ToList();
            }
        }

        public void AtualizaGranjaSelecionada(string granja)
        {
            List<SelectListItem> granjas = (List<SelectListItem>)Session["ListaGranjas"];

            foreach (var item in granjas)
            {
                if (item.Value == granja)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["incubatorioSelecionadoNome"] = granjas.Where(w => w.Selected == true).FirstOrDefault().Text;
            Session["ListaGranjas"] = granjas;
        }

        public List<SelectListItem> CarregaListaTiposDEOAntigo(bool todos)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            ImportaIncubacao.Data.Apolo.CRIA_CAMPO criaCampo = apoloService.CRIA_CAMPO
                .Where(c => c.TabSistCod == "LOC_ARMAZ" && c.CriaCampoNome == "USERTipoProduto")
                .FirstOrDefault();

            string local = Session["granjaSelecionada"].ToString();
            bool localOvosComercio = false;
            if (local.Length == 3)
                if (local.Substring(2, 1) == "C")
                    localOvosComercio = true;

            MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresa =
                bdApolo.EMPRESA_FILIAL.Where(e => e.USERFLIPCod == local
                || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod
                        && c.EmpFilCertificNum == local))
                .FirstOrDefault();

            var listaTipoDEO = criaCampo.CriaCampoItem.Replace("\n","").Split((char)13);

            if (todos)
            {
                items.Add(new SelectListItem { Text = "(Todos os Tipos)", 
                    Value = "(Todos os Tipos)", Selected = true });
                if (Convert.ToBoolean(Session["isIncubatorio"])
                    && MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController.GetGroup("HLBAPPM-TransferenciaLinhagens",
                        (System.Collections.ArrayList)Session["Direitos"]))
                    items.Add(new SelectListItem { Text = "Transferência entre Linhagens", 
                        Value = "Transferência entre Linhagens", Selected = false });
            }

            if (!localOvosComercio)
            {
                foreach (var item in listaTipoDEO)
                {
                    bool selected = false;
                    //if ((empresa == null) && (item.Equals("Ovos Incubáveis")))
                    //    items.Add(new SelectListItem { Text = item, Value = item, Selected = selected });
                    //if (empresa != null)
                    //    if ((!empresa.USERTipoUnidadeFLIP.Equals("Incubatório")) || (!item.Equals("Ovos Incubáveis")) || (!item.Equals("Transf. Ovos Incubáveis")))
                    //        items.Add(new SelectListItem { Text = item, Value = item, Selected = selected });

                    #region Carrega DDL se não for ovos de comércio

                    if ((empresa == null) && (item.Equals("Ovos Incubáveis") || item.Equals("Ovos p/ Comércio")))
                    {
                        if ((Session["tipoDEOselecionado"] == null) || (!todos))
                        {
                            if (!todos)
                            {
                                if ((Session["tipoDEOselecionado"].ToString() == "(Todos os Tipos)")
                                    || (Session["tipoDEOselecionado"] == null))
                                {
                                    selected = true;
                                    Session["tipoDEOselecionado"] = item;
                                }
                            }
                            else
                            {
                                Session["tipoDEOselecionado"] = "(Todos os Tipos)";
                            }
                        }
                        items.Add(new SelectListItem { Text = item, Value = item, Selected = selected });
                    }
                    if (empresa != null)
                    {
                        if ((empresa.USERTipoUnidadeFLIP.Equals("Incubatório")) &&
                            //(!item.Equals("Ovos Incubáveis") || empresa.USERFLIPCod.Equals("PL")))
                            (!item.Equals("Ovos Incubáveis")))
                        {
                            if ((Session["tipoDEOselecionado"] == null) || (!todos))
                            {
                                if (!todos)
                                {
                                    if ((Session["tipoDEOselecionado"].ToString() == "(Todos os Tipos)")
                                        || (Session["tipoDEOselecionado"] == null))
                                    {
                                        selected = true;
                                        Session["tipoDEOselecionado"] = item;
                                    }
                                }
                                else
                                {
                                    Session["tipoDEOselecionado"] = "(Todos os Tipos)";
                                }
                            }
                            items.Add(new SelectListItem { Text = item, Value = item, Selected = selected });
                        }
                        if ((!empresa.USERTipoUnidadeFLIP.Equals("Incubatório")) && (!item.Equals("Transf. Ovos Incubáveis"))
                                && (!item.Equals("Exportação")))
                        {
                            if ((Session["tipoDEOselecionado"] == null) || (!todos))
                            {
                                if (!todos)
                                {
                                    if ((Session["tipoDEOselecionado"].ToString() == "(Todos os Tipos)")
                                        || (Session["tipoDEOselecionado"] == null))
                                    {
                                        selected = true;
                                        Session["tipoDEOselecionado"] = item;
                                    }
                                }
                                else
                                {
                                    Session["tipoDEOselecionado"] = "(Todos os Tipos)";
                                }
                            }
                            items.Add(new SelectListItem { Text = item, Value = item, Selected = selected });
                        }
                    }

                    #endregion
                }
            }
            else
            {
                if(Session["tipoDEOselecionado"] == null) Session["tipoDEOselecionado"] = "Doação";
                items.Add(new SelectListItem { Text = "Doação", Value = "Doação", Selected = true });
                items.Add(new SelectListItem { Text = "Venda de Ovos", Value = "Venda de Ovos", Selected = false });
                items.Add(new SelectListItem { Text = "Inventário de Ovos", Value = "Inventário de Ovos", Selected = false });
                items.Add(new SelectListItem { Text = "Ajuste de Estoque", Value = "Ajuste de Estoque", Selected = false });
                items.Add(new SelectListItem { Text = "Ovos Perdidos", Value = "Ovos Perdidos", Selected = false });
            }

            //Session["ListaTiposDEO"] = items;
            return items;
        }

        public List<SelectListItem> CarregaListaTiposDEO(bool todos)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string local = Session["granjaSelecionada"].ToString();

            bdApoloEntities bd = new bdApoloEntities();
            //var isGranja = bd.EMPRESA_FILIAL.Where(w => w.USERFLIPCod == local && w.USERTipoUnidadeFLIP == "Granja").Count();
            var isIncubatorio = IsIncubatorio(local);

            bool localOvosComercio = false;
            if (local.Length == 3)
                if (local.Substring(2, 1) == "C")
                    localOvosComercio = true;

            if (todos)
            {
                if (Session["tipoDEOselecionado"] == null) Session["tipoDEOselecionado"] = "(Todos os Tipos)";
                items.Add(new SelectListItem { Text = "(Todos os Tipos)", Value = "(Todos os Tipos)", Selected = true });
                items.Add(new SelectListItem { Text = "Ovos Incubáveis", Value = "Ovos Incubáveis", Selected = false });
                items.Add(new SelectListItem { Text = "Ovos p/ Comércio", Value = "Ovos p/ Comércio", Selected = false });
                items.Add(new SelectListItem { Text = "Transf. Ovos Incubáveis", Value = "Transf. Ovos Incubáveis", Selected = false });
                items.Add(new SelectListItem { Text = "Exportação", Value = "Exportação", Selected = false });
                items.Add(new SelectListItem { Text = "Doação", Value = "Doação", Selected = false });
                items.Add(new SelectListItem { Text = "Venda de Ovos", Value = "Venda de Ovos", Selected = false });
                items.Add(new SelectListItem { Text = "Ovos Perdidos", Value = "Ovos Perdidos", Selected = false });
            }
            else
            {
                if (!localOvosComercio)
                {
                    if (!isIncubatorio)
                    {
                        if (Session["tipoDEOselecionado"] == null) Session["tipoDEOselecionado"] = "Ovos Incubáveis";
                        items.Add(new SelectListItem { Text = "Ovos Incubáveis", Value = "Ovos Incubáveis", Selected = true });
                        items.Add(new SelectListItem { Text = "Ovos p/ Comércio", Value = "Ovos p/ Comércio", Selected = false });
                        items.Add(new SelectListItem { Text = "Ovos Perdidos", Value = "Ovos Perdidos", Selected = false });
                        items.Add(new SelectListItem { Text = "Ovos Descartados", Value = "Ovos Descartados", Selected = false });
                    }
                    else
                    {
                        if (Session["tipoDEOselecionado"] == null) Session["tipoDEOselecionado"] = "Ovos p/ Comércio";
                        items.Add(new SelectListItem { Text = "Ovos p/ Comércio", Value = "Ovos p/ Comércio", Selected = true });
                        items.Add(new SelectListItem { Text = "Transf. Ovos Incubáveis", Value = "Transf. Ovos Incubáveis", Selected = false });
                        items.Add(new SelectListItem { Text = "Exportação", Value = "Exportação", Selected = false });
                        items.Add(new SelectListItem { Text = "Doação", Value = "Doação", Selected = false });
                        items.Add(new SelectListItem { Text = "Venda de Ovos", Value = "Venda de Ovos", Selected = false });
                        items.Add(new SelectListItem { Text = "Ovos Perdidos", Value = "Ovos Perdidos", Selected = false });
                    }
                }
                else
                {
                    if (Session["tipoDEOselecionado"] == null) Session["tipoDEOselecionado"] = "Doação";
                    items.Add(new SelectListItem { Text = "Doação", Value = "Doação", Selected = true });
                    items.Add(new SelectListItem { Text = "Venda de Ovos", Value = "Venda de Ovos", Selected = false });
                    items.Add(new SelectListItem { Text = "Ovos Perdidos", Value = "Ovos Perdidos", Selected = false });
                }
            }

            return items;
        }

        public List<SelectListItem> AtualizaTipoDEOSelecionado(string tipoDEO, List<SelectListItem> listaTipoDEO)
        {
            //List<SelectListItem> tiposDEO = (List<SelectListItem>)Session["ListaTiposDEO"];
            List<SelectListItem> tiposDEO = listaTipoDEO;

            foreach (var item in tiposDEO)
            {
                if (item.Value == tipoDEO)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            //if (tiposDEO.Count > 0)
            //    if (tiposDEO.Where(w => w.Selected == true).Count() == 0)
            //        Session["tipoDEOselecionado"] = tiposDEO[0].Text;

            //Session["ListaTiposDEO"] = tiposDEO;
            return tiposDEO;
        }

        public void AtualizaIncubatorioSelecionado(string incubatorio)
        {
            List<SelectListItem> listIncubatorios = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];

            foreach (var item in listIncubatorios)
            {
                if (item.Value == incubatorio)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaIncubatoriosDestino"] = listIncubatorios;
        }

        public void CarregaLinhagensOrigem(string incubatorio)
        {
            string categProduto = "";
            if (incubatorio == "PH")
                categProduto = "2";
            else
                categProduto = "1";

            var lista = bdApolo.PRODUTO
                .Where(w => bdApolo.PROD_GRUPO_SUBGRUPO.Any(a => a.ProdCodEstr == w.ProdCodEstr
                    && a.GrpProdCod == "039") && w.CategProdCod == categProduto)
                .ToList();

            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in lista)
            {
                items.Add(new SelectListItem { Text = item.ProdNomeAlt1, Value = item.ProdNomeAlt1, Selected = false });
            }

            Session["ListaLinhagemOrigem"] = items;
        }

        public void AtualizaLinhagemOrigemSelecionada(string linhagem)
        {
            List<SelectListItem> linhagensOrigem = (List<SelectListItem>)Session["ListaLinhagemOrigem"];

            foreach (var item in linhagensOrigem)
            {
                if (item.Value == linhagem)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaLinhagemOrigem"] = linhagensOrigem;
        }

        public void CarregaLinhagensDestino(string incubatorio, string linhagemOrigem)
        {
            string categProduto = "";
            if (incubatorio == "PH")
                categProduto = "2";
            else
                categProduto = "1";

            var lista = bdApolo.PRODUTO
                .Where(w => bdApolo.PROD_GRUPO_SUBGRUPO
                    .Any(a => a.ProdCodEstr == w.ProdCodEstr
                        && a.GrpProdCod == "040"
                        && bdApolo.SUBGRUPO_PROD.Any(b => a.SubGrpProdCod == b.SubGrpProdCod
                            && b.SubGrpProdNome == linhagemOrigem)) 
                    && w.CategProdCod == categProduto)
                .ToList();

            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in lista)
            {
                items.Add(new SelectListItem { Text = item.ProdNomeAlt1, Value = item.ProdNomeAlt1, Selected = false });
            }

            Session["ListaLinhagemDestino"] = items;
        }

        public void AtualizaLinhagemDestinoSelecionada(string linhagem)
        {
            List<SelectListItem> linhagensDestino = (List<SelectListItem>)Session["ListaLinhagemDestino"];

            foreach (var item in linhagensDestino)
            {
                if (item.Value == linhagem)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaLinhagemDestino"] = linhagensDestino;
        }

        public void CarregaTipoOvo()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "T0", Value = "T0", Selected = false });
            items.Add(new SelectListItem { Text = "T1", Value = "T1", Selected = false });
            items.Add(new SelectListItem { Text = "T2", Value = "T2", Selected = false });

            Session["ListaTipoOvo"] = items;
        }

        public void CarregaListaIncubatorios()
        {
            string granja = Session["granjaSelecionada"].ToString();

            if (granja.Equals("SB") || granja.Equals("PH"))
                Session["location"] = "GP";
            else
                Session["location"] = "PP";

            string location = Session["location"].ToString();

            FLIPDataSetMobile.HATCHERY_CODESDataTable hDT = new FLIPDataSetMobile.HATCHERY_CODESDataTable();
            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();

            hTA.FillByLocation(hDT, location);

            List<SelectListItem> items = new List<SelectListItem>();

            bool localOvosComercio = false;
            if (granja.Length == 3)
                if (granja.Substring(2, 1) == "C")
                    localOvosComercio = true;

            if (localOvosComercio)
            {
                string granjaNormal = granja.Substring(0,2);
                string descOvosComercio = hDT.Where(w => w.HATCH_LOC == granjaNormal).FirstOrDefault().HATCH_DESC;
                items.Add(new SelectListItem { Text = descOvosComercio + " - SALA OVOS DE COMÉRCIO" , Value = granja, Selected = true });
            }
            else
            {
                foreach (var item in hDT)
                {
                    if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        items.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                    }
                }

                if (granja == "PL")
                {
                    items.Add(new SelectListItem { Text = "Ovo Tipo 0", Value = "T0", Selected = false });
                    items.Add(new SelectListItem { Text = "Ovo Tipo 1", Value = "T1", Selected = false });
                    items.Add(new SelectListItem { Text = "Ovo Tipo 2", Value = "T2", Selected = false });
                }
            }

            //if (items.Count > 0)
            //    if (items.Where(w => w.Selected == true).Count() == 0)
            //        Session["incubatorioDestinoSelecionado"] = items[0].Value;

            Session["ListaIncubatoriosDestino"] = items;
        }

        public void CarregaListaTipoVisualizacaoQtde()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Ovos", Value = "1", Selected = true });
            items.Add(new SelectListItem { Text = "Bandejas (150)", Value = "150", Selected = false });
            items.Add(new SelectListItem { Text = "Caixas (360)", Value = "360", Selected = false });

            Session["ListaTipoVisualizacaoQtde"] = items;
        }

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

        public List<SelectListItem> CarregaListaMotivoDivergenciaDEO()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Erro Digitação na Granja", Value = "Erro Digitação na Granja", Selected = false });
            items.Add(new SelectListItem { Text = "Erro Contagem no Incubatório", Value = "Erro Contagem no Incubatório", Selected = false });
            items.Add(new SelectListItem { Text = "Data e/ou Lote Incorreto", Value = "Data e/ou Lote Incorreto", Selected = false });
            items.Add(new SelectListItem { Text = "Ovos Perdidos no Carregamento", Value = "Ovos Perdidos no Carregamento", Selected = false });
            items.Add(new SelectListItem { Text = "Ovos Perdidos no Transporte", Value = "Ovos Perdidos no Transporte", Selected = false });
            items.Add(new SelectListItem { Text = "Ovos Perdidos no Descarregamento", Value = "Ovos Perdidos no Descarregamento", Selected = false });

            return items;
        }

        public List<SelectListItem> CarregaListaLocaisFechLanc()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            #region HYBR

            #region Carrega Locais de Fechamento de Lançamentos

            Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter dflTA =
                new Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter();
            FLIPDataSetMobile.DATA_FECH_LANCDataTable dflDT = new FLIPDataSetMobile.DATA_FECH_LANCDataTable();
            dflTA.Fill(dflDT);

            #endregion

            #region Carrega Lista de Incubatórios

            FLIPDataSetMobile.HATCHERY_CODESDataTable hDT = new FLIPDataSetMobile.HATCHERY_CODESDataTable();
            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();
            hTA.Fill(hDT);

            #endregion

            foreach (var item in dflDT.ToList())
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-AcessoFechamento" + item.LOCATION.Replace(" ",""), (System.Collections.ArrayList)Session["Direitos"]))
                {
                    var hRow = hDT.Where(w => w.HATCH_LOC == item.LOCATION).FirstOrDefault();
                    string descricao = "Brasil - " + item.LOCATION + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": "
                        + item.DATA_FECH_LANC.ToShortDateString();
                    if (hRow != null) descricao = "Brasil - " + am.GetTextOnLanguage("Incubatório", Session["language"].ToString()) + " " + hRow.HATCH_DESC
                        + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": " + item.DATA_FECH_LANC.ToShortDateString();

                    items.Add(new SelectListItem { Text = descricao, Value = item.LOCATION, Selected = false });
                }
            }

            #endregion

            #region HYCL

            #region Carrega Locais de Fechamento de Lançamentos

            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dflCLTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
            ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable dflCLDT = new ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable();
            dflCLTA.Fill(dflCLDT);

            #endregion

            foreach (var item in dflCLDT.ToList())
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-AcessoFechamentoCL" + item.LOCATION.Replace(" ", ""), (System.Collections.ArrayList)Session["Direitos"]))
                {
                    var hRow = hDT.Where(w => w.HATCH_LOC == item.LOCATION).FirstOrDefault();
                    string descricao = "Chile - " + item.LOCATION + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": " 
                        + item.DATA_FECH_LANC.ToShortDateString();
                    if (hRow != null) descricao = "Chile - " + am.GetTextOnLanguage("Incubatório", Session["language"].ToString()) + " " + hRow.HATCH_DESC
                        + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": " + item.DATA_FECH_LANC.ToShortDateString();

                    items.Add(new SelectListItem { Text = descricao, Value = item.LOCATION, Selected = false });
                }
            }

            #endregion

            #region HYCO

            #region Carrega Locais de Fechamento de Lançamentos

            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dflCOTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
            ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable dflCODT = new ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable();
            dflCOTA.Fill(dflCODT);

            #endregion

            foreach (var item in dflCODT.ToList())
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-AcessoFechamentoCO" + item.LOCATION.Replace(" ", ""), (System.Collections.ArrayList)Session["Direitos"]))
                {
                    var hRow = hDT.Where(w => w.HATCH_LOC == item.LOCATION).FirstOrDefault();
                    string descricao = "Colombia - " + item.LOCATION + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": " 
                        + item.DATA_FECH_LANC.ToShortDateString();
                    if (hRow != null) descricao = "Colombia - " + am.GetTextOnLanguage("Incubatório", Session["language"].ToString()) + " " + hRow.HATCH_DESC
                        + " - " + am.GetTextOnLanguage("Lanc. Fechado até", Session["language"].ToString()) + ": " + item.DATA_FECH_LANC.ToShortDateString();

                    items.Add(new SelectListItem { Text = descricao, Value = item.LOCATION, Selected = false });
                }
            }

            #endregion

            return items;
        }

        #endregion

        #region Métodos p/ JavaScript

        [HttpPost]
        public ActionResult CarregaLotes(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<SelectListItem> items = new List<SelectListItem>();

            Session["listLotes"] = new List<Lotes>();
            string location = Session["location"].ToString();

            List<Lotes> listaLotes = new List<Lotes>();

            flocks.FillActivesByFarm(flip.FLOCKS, "HYBR", "BR", location, id);

            string variety = "";
            if (Convert.ToBoolean(Session["TransferenciaLinhagens"]))
                variety = Session["linhagemOrigemSelecionada"].ToString();
            else
                variety = "";

            List<FLIPDataSet.FLOCKSRow> flocksTable = flip.FLOCKS
                .Where(f => //!f.FLOCK_ID.Contains("K") && 
                    (f.VARIETY == variety || variety == ""))
                    .OrderBy(o=> o.NUM_1).ToList();

            //for (int i = 0; i < flip.FLOCKS.Count; i++)
            for (int i = 0; i < flocksTable.Count; i++)
            {
                string bkp = "";
                if (flocksTable[i].FLOCK_ID.ToString().Contains("K"))
                    bkp = "-BKP";

                if (items.Where(t => t.Text == flocksTable[i].NUM_1.ToString() + bkp).Count() == 0)
                {
                    items.Add(new SelectListItem
                    {
                        Text = flocksTable[i].NUM_1.ToString() + bkp,
                        Value = flocksTable[i].NUM_1.ToString() + bkp,
                        Selected = false
                    });
                }
                if (!flocksTable[i].IsHATCH_DATENull())
                {
                    listaLotes.Add(new Lotes
                    {
                        Granja = flocksTable[i].FARM_ID,
                        Linhagem = flocksTable[i].VARIETY,
                        LoteCompleto = flocksTable[i].FLOCK_ID,
                        NumeroLote = flocksTable[i].NUM_1.ToString() + bkp,
                        DataNascimento = flocksTable[i].HATCH_DATE,
                        Location = flocksTable[i].LOCATION,
                        Galpao = flocksTable[i].NUM_2.ToString()
                    });
                }
            }

            Session["listLotes"] = listaLotes;
            Session["ListaLotes"] = items;

            Session["ListaNucleos"] = AtualizaDDL(id, (List<SelectListItem>)Session["ListaNucleos"]);

            return Json(items);
        }

        [HttpPost]
        public ActionResult CarregaGalpoes(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    List<Lotes> items = new List<Lotes>();

                    List<SelectListItem> itemsGlp = new List<SelectListItem>();

                    string galpao = "";

                    List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

                    var loteSelecionado = listaLotes
                        .Where(s => s.NumeroLote == id)
                        .ToList();

                    foreach (var lote in loteSelecionado)
                    {
                        Session["loteEscolhido"] = lote.LoteCompleto;
                        int tamanho = lote.LoteCompleto.Length - 1;

                        if ((lote.Galpao != null) && (lote.Galpao != ""))
                            galpao = lote.Galpao;
                        else
                            galpao = "";

                        if (galpao.Equals(""))
                        {
                            for (int i = tamanho; i >= 0; i--)
                            {
                                double Num;
                                bool isNum = double.TryParse(lote.LoteCompleto.Substring(i, 1), out Num);

                                if (isNum)
                                {
                                    galpao = "0" + lote.LoteCompleto.Substring(i, 1);
                                    items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                                    if (Session["location"].ToString().Equals("GP"))
                                        itemsGlp.Add(new SelectListItem { Text = galpao + " - " + lote.Linhagem, Value = galpao + " - " + lote.Linhagem, Selected = false });
                                    else
                                        itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });

                                    foreach (var item in listaLotes)
                                    {
                                        if (item.LoteCompleto == lote.LoteCompleto)
                                        {
                                            item.Galpao = galpao;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                            if (Session["location"].ToString().Equals("GP"))
                                itemsGlp.Add(new SelectListItem { Text = galpao + " - " + lote.Linhagem, Value = galpao + " - " + lote.Linhagem, Selected = false });
                            else
                                itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });
                        }
                    }

                    Session["listLotes"] = listaLotes;
                    Session["ListaGalpoes"] = itemsGlp;

                    Session["ListaLotes"] = AtualizaDDL(id, (List<SelectListItem>)Session["ListaLotes"]);

                    return Json(items);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult RetornaLoteCompleto(string id, string id2)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    Lotes retornoLote;

                    List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

                    if (Session["location"].ToString().Equals("PP"))
                        retornoLote = listaLotes
                            .Where(l => l.NumeroLote == id && l.Galpao == id2)
                            .FirstOrDefault();
                    else
                        retornoLote = listaLotes
                            .Where(l => l.NumeroLote == id && l.Galpao + " - " + l.Linhagem == id2)
                            .FirstOrDefault();

                    Session["linhagemSelecionada"] = retornoLote.Linhagem;
                    Session["loteCompletoSelecionado"] = retornoLote.LoteCompleto;

                    Session["ListaGalpoes"] = AtualizaDDL(id2, (List<SelectListItem>)Session["ListaGalpoes"]);

                    return Json(retornoLote);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult RetorndaIdade(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

                    string loteEscolhido = Session["loteEscolhido"].ToString();
                    string retorno = "";

                    Lotes retornoLote = listaLotes
                        .Where(l => l.LoteCompleto == loteEscolhido)
                        .FirstOrDefault();

                    int age = 0;
                    DateTime data;
                    if (DateTime.TryParse(id, out data))
                    {
                        DateTime dataDEO = Convert.ToDateTime(Session["dataDEO"]);
                        if (data > dataDEO)
                        {
                            retorno = "Data de Produção NÃO pode ser maior que a Data do DEO! Verifique!";
                        }
                        else
                        {
                            age = ((Convert.ToDateTime(data) - retornoLote.DataNascimento).Days) / 7;

                            Session["dataProducaoSelecionada"] = id;
                            Session["idadeSelecionada"] = age;

                            if (!Session["qtdOvos"].ToString().Equals("0"))
                                retorno = VerificaEstoqueJS(id, loteEscolhido, Session["qtdOvos"].ToString(), 0);
                        }
                    }

                    List<Lotes> retornoLista = new List<Lotes>();
                    retornoLista.Add(new Lotes { Galpao = age.ToString(), Linhagem = retorno, 
                        LoteCompleto = "", Location = "" });

                    return Json(retornoLista);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult VerificaEstoque(string id, string id2, string id3)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    string retorno = VerificaEstoqueJS(id, id2, id3, 0);

                    Session["qtdOvos"] = id3;

                    return Json(retorno);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult VerificaEstoqueOC(string qtde)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int retorno = VerificaEstoqueOvosComercioWEB(Convert.ToInt32(qtde), Session["granjaSelecionada"].ToString());
            Session["qtdOvos"] = qtde;

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult AtualizaSessionDataDEO(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    DateTime dataFiltroAntigo = Convert.ToDateTime(Session["dataDEO"].ToString() + " " + Session["horaDEO"].ToString());

                    DateTime dataFiltro = Convert.ToDateTime(id + " " + Session["horaDEO"].ToString());
                    string granja = Session["granjaSelecionada"].ToString();
                    string tipoDEO = Session["tipoDEOselecionado"].ToString();

                    var listaAntigos = (List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"];

                    DateTime data01 = dataFiltro.AddMinutes(1);

                    int existeDataDEOBD = hlbapp.LayoutDiarioExpedicaos
                        .Where(l => l.DataHoraCarreg >= dataFiltro && l.DataHoraCarreg <= data01 && l.Granja == granja)
                        .Count();

                    int existeDataDEOLocal = listaAntigos
                        .Where(l => l.DataHoraCarreg == dataFiltro && l.DataHoraCarreg <= data01 && l.Granja == granja)
                        .Count();

                    if (Convert.ToDateTime(id) >= Convert.ToDateTime("15/12/2021")
                        && (granja == "CH" || granja == "NM" || granja == "HL" || granja == "CG" || granja == "GE" || granja == "SD" || granja == "SJP01" || granja == "SJP02"))
                    {
                        return Json("NÃO É POSSÍVEL MAIS CRIAR DEO A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!");
                    }

                    if (existeDataDEOBD > 0 && existeDataDEOLocal == 0)
                    {
                        return Json("Já existe DEO com a Data / Hora selecionada! Informe outra data hora!"
                            + " Caso seja no mesmo horário, informe uma Data / Hora maior que 10 minutos da atual!");
                    }
                    else
                    {
                        Session["dataDEO"] = id;

                        var lista = CarregarItensDEO(hlbapp, dataFiltroAntigo, granja, "", "Crescente",
                            "Cadastro");

                        foreach (var item in lista)
                        {
                            item.DataHoraCarreg = dataFiltro;
                        }

                        //var listaAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];
                        //var listaImport = CarregarItensDEOImport(hlbapp, dataFiltroAntigo, granja);

                        //foreach (var item in listaImport)
                        //{
                        //    item.DataHoraCarreg = dataFiltro;
                        //}

                        //db.SaveChanges();
                        hlbapp.SaveChanges();

                        Session["dataHoraCarreg"] = dataFiltro;
                    }

                    return Json("");
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult AtualizaSessionHoraDEO(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    DateTime dataFiltroAntigo = Convert.ToDateTime(Session["dataDEO"].ToString() + " " + Session["horaDEO"].ToString());
                    
                    DateTime dataFiltro = Convert.ToDateTime(Session["dataDEO"].ToString() + " " + id);
                    string granja = Session["granjaSelecionada"].ToString();
                    string tipoDEO = Session["tipoDEOselecionado"].ToString();

                    var listaAntigos = (List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"];

                    DateTime data01 = dataFiltro.AddMinutes(1);

                    int existeDataDEOBD = hlbapp.LayoutDiarioExpedicaos
                        .Where(l => l.DataHoraCarreg >= dataFiltro && l.DataHoraCarreg <= data01 && l.Granja == granja)
                        .Count();

                    int existeDataDEOLocal = listaAntigos
                        .Where(l => l.DataHoraCarreg == dataFiltro && l.DataHoraCarreg <= data01 && l.Granja == granja)
                        .Count();

                    if (existeDataDEOBD > 0 && existeDataDEOLocal == 0)
                    {
                        return Json("Já existe DEO com a Data / Hora selecionada! Informe outra data hora!"
                            + " Caso seja no mesmo horário, informe uma Data / Hora maior que 10 minutos da atual!");
                    }
                    else
                    {
                        Session["horaDEO"] = id;

                        var lista = CarregarItensDEO(hlbapp, dataFiltroAntigo, granja, "", "Crescente", 
                            "Cadastro");

                        foreach (var item in lista)
                        {
                            item.DataHoraCarreg = dataFiltro;
                        }

                        //var listaAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];
                        //var listaImport = CarregarItensDEOImport(hlbapp, dataFiltroAntigo, granja);

                        //foreach (var item in listaImport)
                        //{
                        //    item.DataHoraCarreg = dataFiltro;
                        //}

                        //db.SaveChanges();
                        hlbapp.SaveChanges();

                        Session["dataHoraCarreg"] = dataFiltro;
                    }

                    return Json("");
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult AtualizaSession(string value, string nameSession)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session[nameSession] = value;

            return Json("");
        }

        [HttpPost]
        public ActionResult AtualizaSessionReclassificacao(string value)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["reclassificacao"] = Convert.ToBoolean(value.Replace("false,true", "true"));

            return Json("");
        }

        [HttpPost]
        public ActionResult SelecionaTipoDEO(string id, string tipoEmp)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if ((tipoEmp == "Granja" && id != "Ovos Incubáveis" && id != "Ovos p/ Comércio")
                || (tipoEmp == "Incubatório" && id != "Transf. Ovos Incubáveis"))
                if (Session["granjaSelecionada"].ToString() == "PL")
                    Session["incubatorioDestinoSelecionado"] = "NM";
                else
                    Session["incubatorioDestinoSelecionado"] = Session["granjaSelecionada"];
            else
            {
                List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
                var inc = items.Where(w => w.Selected == true).FirstOrDefault();
                if (inc != null)
                    Session["incubatorioDestinoSelecionado"] = inc.Value;
                else
                    Session["incubatorioDestinoSelecionado"] = items[0].Value;
            }
                
            Session["tipoDEOselecionado"] = id;

            Session["ListaTiposDEO"] = AtualizaTipoDEOSelecionado(id,
                (List<SelectListItem>)Session["ListaTiposDEO"]);

            return Json("");
        }

        [HttpPost]
        public ActionResult SelecionaIncubatorioDestino(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioDestinoSelecionado"] = id;

            AtualizaIncubatorioSelecionado(id);

            return Json("");
        }

        [HttpPost]
        public ActionResult SelecionaLinhagemOrigem(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["linhagemOrigemSelecionada"] = id;

            AtualizaLinhagemOrigemSelecionada(id);

            string granja = Session["granjaSelecionada"].ToString();

            CarregaLinhagensDestino(granja, id);

            List<SelectListItem> items = (List<SelectListItem>)Session["ListaLinhagemDestino"];

            return Json(items);
        }

        [HttpPost]
        public ActionResult SelecionaLinhagemDestino(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["linhagemDestinoSelecionada"] = id;

            AtualizaLinhagemDestinoSelecionada(id);

            return Json("");
        }

        [HttpPost]
        public ActionResult AtualizaSessionQtdeInformada(string valor, string qtdOvosGranja, 
            string lote, string data)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int qtdOvosGranjaInt = Convert.ToInt32(qtdOvosGranja);

            int qtdeInformada = 0; 
            if (int.TryParse(valor, out qtdeInformada))
                Session["qtdDiferenca_" + lote+"|"+data] = qtdeInformada;

            string retorno = VerificaEstoqueJS(data, lote, valor, qtdOvosGranjaInt);

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult AtualizaSessionQtdeInformadaAjusteEstoque(string valor, string qtdOvosGranja,
            string local, string lote, string data)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int qtdOvosGranjaInt = Convert.ToInt32(qtdOvosGranja);
            DateTime dataPrd = Convert.ToDateTime(data);
            string retorno = "";

            int qtdeInformada = 0;
            if (int.TryParse(valor, out qtdeInformada))
                Session["qtdAjuste_" + local + "|" + lote + "|" + data] = qtdeInformada;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            FLOCK_DATA fd = hlbapp.FLOCK_DATA
                .Where(w => w.Flock_ID == lote && w.Trx_Date == dataPrd)
                .FirstOrDefault();

            int hatchEggs = 0;
            if (fd != null) hatchEggs = Convert.ToInt32(fd.Hatch_Eggs);
            if (hatchEggs < qtdeInformada)
                retorno = am.GetTextOnLanguage("A quantidade não pode ser maior que a produzida na granja! (Qtde: " 
                    + String.Format("{0:N0}", hatchEggs) + ")", Session["language"].ToString());

            #region Verifica se já tem Incubação para descontar

            List<HATCHERY_EGG_DATA> hed = hlbapp.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == local
                    && w.Flock_id.Contains(lote)
                    && w.Lay_date == dataPrd)
                .ToList();

            int qtdeIncubada = 0;
            if (hed.Count > 0) qtdeIncubada = Convert.ToInt32(hed.Sum(s => s.Eggs_rcvd));

            if (qtdeIncubada > 0)
                retorno = retorno + "<br/>" + am.GetTextOnLanguage("Existe quantidade já incubada que não pode ser considerada! (Qtde: "
                    + String.Format("{0:N0}", qtdeIncubada) + ")", Session["language"].ToString());

            if (qtdeIncubada > 0 && hatchEggs < qtdeInformada)
                retorno = retorno + "<br/>" + am.GetTextOnLanguage("Saldo disponível para Ajuste: "
                    + String.Format("{0:N0}", (hatchEggs - qtdeIncubada)) + ")", Session["language"].ToString());

            #endregion

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult AtualizaSessionQtdeOvos(string valor, string lote, string data)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int qtdeInformada = 0;
            if (int.TryParse(valor, out qtdeInformada))
                Session["qtdOvos_" + lote + "|" + data] = qtdeInformada;

            string retorno = VerificaEstoqueJS(data, lote, valor, 0);

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult VerificaNFNum(string valor, string tipo, string destino)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Apolo10EntitiesService apoloSession = new Apolo10EntitiesService();
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            #region Verifica se campo está vazio

            string origem = Session["granjaSelecionada"].ToString();
            string origemInc = origem;
            if (origem == "PL") origemInc = "NM";

            if (valor == "" &&
                (tipo == "Ovos Incubáveis" || 
                    (tipo == "Transf. Ovos Incubáveis" && origem != destino) ||
                    (tipo == "Ovos p/ Comércio" && origem != destino)))
                return Json("Obrigatório informar número da nota fiscal!");

            #endregion

            #region Verifica se nota existe

            ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empApolo = apoloSession.EMPRESA_FILIAL
                .Where(w => w.USERFLIPCod == origemInc).FirstOrDefault();

            #region Verifica a data para selecionar a empresa correta para Planalto após a troca da 20 para a 30

            DateTime dataHoraCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            DateTime dataComparacao = Convert.ToDateTime("01/10/2019");
            if (dataHoraCarreg >= dataComparacao && origem == "PL")
            {
                empApolo = apoloSession.EMPRESA_FILIAL.Where(w => w.EmpCod == "30").FirstOrDefault();
            }

            #endregion

            System.Data.Objects.ObjectParameter numeroNF =
                    new System.Data.Objects.ObjectParameter("numero", typeof(global::System.String));
            if (valor != "")
                valor = Convert.ToInt32(valor).ToString();
            apoloService.CONCAT_ZERO_ESQUERDA(valor, 10, numeroNF);
            valor = numeroNF.Value.ToString();

            if (empApolo != null && (tipo == "Ovos Incubáveis" ||
                    (tipo == "Transf. Ovos Incubáveis" && origemInc != destino) ||
                    (tipo == "Ovos p/ Comércio" && origemInc != destino)))
            {
                ImportaIncubacao.Data.Apolo.NOTA_FISCAL nfApolo = apoloSession.NOTA_FISCAL
                    .Where(w => w.EmpCod == empApolo.EmpCod
                        && w.CtrlDFModForm == "NF-e"
                        && w.CtrlDFSerie == "001"
                        && w.NFNum == valor).FirstOrDefault();

                if (nfApolo == null)
                    return Json("Não existe essa nota faturada! Verifique!");
                else
                {
                    #region Verifica se está relacionada a outro DEO

                    LayoutDiarioExpedicaos deo = hlbappSession.LayoutDiarioExpedicaos
                        .Where(w => w.Granja == origem 
                            && w.TipoDEO == tipo
                            && w.NFNum == valor
                            && (
                                (empApolo.EmpCod == "20" && w.DataHoraCarreg < dataComparacao)
                                ||
                                (empApolo.EmpCod == "30" && w.DataHoraCarreg >= dataComparacao)
                               )).FirstOrDefault();

                    if (deo != null)
                    {
                        if (origem.Equals("SB") || origem.Equals("PH"))
                            Session["location"] = "GP";
                        else
                            Session["location"] = "PP";
                        string location = Session["location"].ToString();
                        FLIPDataSetMobile.HATCHERY_CODESDataTable hDT =
                            new FLIPDataSetMobile.HATCHERY_CODESDataTable();
                        MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                            new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();
                        hTA.FillByLocation(hDT, location);

                        string destinoDEO = hDT.Where(w => w.HATCH_LOC == deo.Incubatorio).FirstOrDefault()
                            .HATCH_DESC;

                        return Json("Nota já relacionada no DEO da data " +
                            deo.DataHoraCarreg.ToString("dd/MM/yyyy HH:mm") + " - " +
                            deo.TipoDEO + " - Destino: " + destinoDEO);
                    }
                    else
                        return Json("");

                    #endregion
                }
            }
            else
            {
                #region Verifica se está relacionada a outro DEO

                LayoutDiarioExpedicaos deo = hlbappSession.LayoutDiarioExpedicaos
                    .Where(w => w.Granja == origem && w.TipoDEO == tipo
                        && w.NFNum == valor).FirstOrDefault();

                if (deo != null)
                {
                    if (origem.Equals("SB") || origem.Equals("PH"))
                        Session["location"] = "GP";
                    else
                        Session["location"] = "PP";
                    string location = Session["location"].ToString();
                    FLIPDataSetMobile.HATCHERY_CODESDataTable hDT =
                        new FLIPDataSetMobile.HATCHERY_CODESDataTable();
                    MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                        new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();
                    hTA.FillByLocation(hDT, location);

                    string destinoDEO = hDT.Where(w => w.HATCH_LOC == deo.Incubatorio).FirstOrDefault()
                        .HATCH_DESC;

                    return Json("Nota já relacionada no DEO da data " +
                        deo.DataHoraCarreg.ToString("dd/MM/yyyy HH:mm") + " - " +
                        deo.TipoDEO + " - Destino: " + destinoDEO);
                }
                else
                    return Json("");

                #endregion
            }

            #endregion
        }

        #endregion

        #region Métodos para Itens

        public ActionResult Create(string origem)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["origemCreateItemDEO"] = origem;

            string granja = Session["granjaSelecionada"].ToString();
            DateTime dataFiltro = Convert.ToDateTime(Session["dataHoraCarreg"].ToString());
            if (origem != "Conferência")
            {
                dataFiltro = Convert.ToDateTime(Session["dataDEO"].ToString() + " "
                    + Session["horaDEO"].ToString());
            }
            else
            {
                Session["dataDEO"] = dataFiltro.ToString("dd/MM/yyyy");
                Session["horaDEO"] = dataFiltro.ToString("HH:mm");
            }
            string tipoDEO = Session["tipoDEOselecionado"].ToString();
            Session["qtdOvos"] = 0;

            if (!granja.Equals("PL")
                || (granja.Equals("PL") && !tipoDEO.Equals("Transf. Ovos Incubáveis")))
            {
                CarregaListaNucleos();
                CarregaTipoOvo();
            }

            string retorno = VerificaLinhagemOrigemSelecionada(granja, dataFiltro);
            if (retorno != "")
            {
                ViewBag.Erro = retorno;
                return View("Index", CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                    "Cadastro"));
            }

            retorno = VerificaLinhagemDestinoSelecionada(granja, dataFiltro);
            if (retorno != "")
            {
                ViewBag.Erro = retorno;
                return View("Index", CarregarItensDEO(hlbapp, dataFiltro, granja, "", "Crescente",
                    "Cadastro"));
            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateItem(LayoutDiarioExpedicaos layoutdiarioexpedicao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            string granja = Session["granjaSelecionada"].ToString();
            string origem = Session["origemCreateItemDEO"].ToString();
            string numIdentificacao = "Sem ID";
            if (Session["numIdentificacaoSelecionado"] != null) numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            try
            {
                HLBAPPEntities hlbappNew = new HLBAPPEntities();

                string usuario = Session["login"].ToString();
                string tipoDEO = Session["tipoDEOselecionado"].ToString();
                string incubatorio = Session["incubatorioDestinoSelecionado"].ToString();

                var listaItens = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                if (listaItens.Count == 0)
                    listaItens = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", "Cadastro");

                if (dataCarreg >= Convert.ToDateTime("15/12/2021") 
                    && (granja == "CH" || granja == "NM" || granja == "HL" || granja == "CG" || granja == "GE" || granja == "SD" || granja == "SJP01" || granja == "SJP02"))
                {
                    ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR DEO A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";

                    if (origem == "Cadastro")
                        return View("Index", listaItens);
                    else
                        return View("ItemConfereDEO", listaItens);
                }

                if (ExisteFechamentoEstoque(dataCarreg, granja))
                {
                    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                        .Where(e => e.USERFLIPCod == granja)
                        .FirstOrDefault();

                    //string responsavel = "Miriene Gomes";
                    //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                    //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                    //    responsavel = "Sérica Doimo";
                    //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                    //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                    //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataCarreg.ToShortDateString()
                    //                + "! Não pode ser excluído este Diário de Expedição!"
                    //                + " Verificar com " + responsavel + " a possibilidade da liberação!";

                    string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                    ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                        + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                        + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                    if (origem == "Cadastro")
                        return View("Index", listaItens);
                    else
                        return View("ItemConfereDEO", listaItens);
                }

                //System.Data.Objects.ObjectParameter numero =
                //    new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                        
                //layoutdiarioexpedicao.QtdeBandejas = layoutdiarioexpedicao.QtdeOvos;
                //layoutdiarioexpedicao.QtdeOvos = layoutdiarioexpedicao.QtdeOvos * 150;

                if (layoutdiarioexpedicao.QtdeBandejas.Equals(0))
                    layoutdiarioexpedicao.QtdeBandejas = layoutdiarioexpedicao.QtdeOvos / 150;
                else
                {
                    layoutdiarioexpedicao.QtdeBandejas = layoutdiarioexpedicao.QtdeOvos;
                    layoutdiarioexpedicao.QtdeOvos = layoutdiarioexpedicao.QtdeOvos * 150;
                }

                //apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
                //string retorno = VerificaEstoqueInterno(layoutdiarioexpedicao.DataProducao.ToShortDateString(), layoutdiarioexpedicao.LoteCompleto, layoutdiarioexpedicao.QtdeOvos.ToString());

                string empresaEstoque = "";
                if (granja == "PL" && tipoDEO == "Ovos Incubáveis" && incubatorio == "NM")
                    empresaEstoque = incubatorio;
                else
                    if (granja != "PL")
                        empresaEstoque = granja;
                    else
                        empresaEstoque = "NM";

                string retorno = VerificaEstoqueWEB(layoutdiarioexpedicao.DataProducao, 
                    layoutdiarioexpedicao.LoteCompleto, Convert.ToInt32(layoutdiarioexpedicao.QtdeOvos),
                    empresaEstoque, granja, 0);

                bool ovosIncubaveisPL = false;
                if (granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis"))
                    ovosIncubaveisPL = true;

                if (!retorno.Equals("") && !retorno.Equals("0") && !ovosIncubaveisPL)
                {
                    ViewBag.Erro = retorno;
                    return View("Create", layoutdiarioexpedicao);
                }
                else
                {
                    #region Insere no FLIP se não tiver a produção do dia digitada

                    if ((layoutdiarioexpedicao.DataProducao == DateTime.Today
                            && tipoDEO.Equals("Ovos Incubáveis")
                            && (retorno.Equals("0"))) ||
                        (granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                    {
                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == granja).FirstOrDefault();

                        ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                .Where(e1 => e1.USERFLIPCodigo == granja)
                                .FirstOrDefault();

                        if (verificaEmpresa != null ||
                            (entidade1 != null && verificaEmpresa == null))
                        {
                            if (verificaEmpresa == null)
                            {
                                verificaEmpresa = new ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL();
                                verificaEmpresa.USERTipoUnidadeFLIP = "";
                            }
                            if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || granja.Equals("PL")
                                || entidade1 != null)
                            {
                                flocks.Fill(flip.FLOCKS);

                                int existeLoteCadastradoFLIP = flip.FLOCKS
                                    .Where(f => f.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto)
                                    .Count();

                                if (existeLoteCadastradoFLIP > 0)
                                {
                                    #region Inclui no FLIP

                                    FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto)
                                            .FirstOrDefault();

                                    flock_data.FillByFlockData2(flip.FLOCK_DATA, "HYBR", "BR",
                                        flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                        layoutdiarioexpedicao.DataProducao);

                                    int existeDiarioLoteCadastradoFLIP = flip.FLOCK_DATA
                                        .Where(d => d.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto
                                            && d.TRX_DATE == layoutdiarioexpedicao.DataProducao)
                                        .Count();

                                    if (existeDiarioLoteCadastradoFLIP == 0)
                                    {
                                        int age = (((DateTime.Today - flock.MOVE_DATE).Days) / 7) + 1;

                                        flock_data.Insert(flock.COMPANY, flock.REGION, flock.LOCATION,
                                            flock.FARM_ID, flock.FLOCK_ID, 1,
                                                layoutdiarioexpedicao.DataProducao, age, null, null, null,
                                                null, null, null, null, null, null, null, null,
                                                layoutdiarioexpedicao.QtdeOvos, null,
                                                null, null, null, null, null, 0, null, null, null, null,
                                                null, null, null, null, null, null, null, null, null, null, null,
                                                null, null, null, null);
                                    }
                                    else
                                    {
                                        int existeItensImportadoApoloFLIP = hlbapp.LayoutDiarioExpedicaos
                                            .Where(l => l.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                                && l.DataProducao == layoutdiarioexpedicao.DataProducao
                                                && l.Importado == "ImportadoApoloFLIP")
                                            .Count();

                                        if (existeItensImportadoApoloFLIP > 0)
                                        {
                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto
                                                    && d.TRX_DATE == layoutdiarioexpedicao.DataProducao)
                                                .FirstOrDefault();

                                            if (!dataRow.IsNUM_1Null())
                                                dataRow.NUM_1 = dataRow.NUM_1
                                                    + layoutdiarioexpedicao.QtdeOvos;
                                            else
                                                dataRow.NUM_1 = layoutdiarioexpedicao.QtdeOvos;

                                            dataRow.NUM_8 = 0;

                                            flock_data.Update(dataRow);
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    retorno = "Lote não cadastrado no FLIP. Por favor, "
                                        + "primeiro cadastro o Lote no FLIP!";
                                }
                            }
                        }
                        retorno = "";
                    }

                    if (!retorno.Equals(""))
                    {
                        ViewBag.Erro = retorno;
                        return View("Create", layoutdiarioexpedicao);
                    }

                    #endregion

                    if (origem == "Cadastro")
                    {
                        if (tipoDEO == "Ovos Incubáveis"
                            || tipoDEO == "Transf. Ovos Incubáveis"
                            || tipoDEO == "Inventário de Ovos")
                            layoutdiarioexpedicao.Importado = "Não";
                        else
                            layoutdiarioexpedicao.Importado = "Conferido";
                    }
                    else // Conferência
                    {
                        layoutdiarioexpedicao.Importado = "Divergência";
                        layoutdiarioexpedicao.QtdDiferenca = (int)layoutdiarioexpedicao.QtdeOvos;
                        layoutdiarioexpedicao.QtdeBandejas = 0;
                        layoutdiarioexpedicao.QtdeOvos = 0;
                    }
                }

                if (layoutdiarioexpedicao.Linhagem == null) layoutdiarioexpedicao.Linhagem = Session["linhagemSelecionada"].ToString();
                if (layoutdiarioexpedicao.LoteCompleto == null) layoutdiarioexpedicao.LoteCompleto = Session["loteCompletoSelecionado"].ToString();
                if (layoutdiarioexpedicao.DataProducao == null) layoutdiarioexpedicao.DataProducao = Convert.ToDateTime(Session["dataProducaoSelecionada"].ToString());

                if (dataCarreg < layoutdiarioexpedicao.DataProducao)
                {
                    ViewBag.Erro = "A data da produção não pode ser maior que a data do transporte (Data do DEO)."
                        + " Por favor, verifique!";

                    if (origem == "Cadastro")
                        return View("Index", listaItens);
                    else
                        return View("ItemConfereDEO", listaItens);
                }

                if (Session["idadeSelecionada"] == null)
                {
                    List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

                    string loteEscolhido = Session["loteEscolhido"].ToString();

                    Lotes retornoLote = listaLotes
                        .Where(l => l.LoteCompleto == loteEscolhido)
                        .FirstOrDefault();

                    int age = ((Convert.ToDateTime(layoutdiarioexpedicao.DataProducao) - 
                        retornoLote.DataNascimento).Days) / 7;

                    Session["idadeSelecionada"] = age;

                    layoutdiarioexpedicao.Idade = Convert.ToInt32(Session["idadeSelecionada"].ToString());
                }
                else
                {
                    if (layoutdiarioexpedicao.Idade == 0)
                        layoutdiarioexpedicao.Idade = Convert.ToInt32(Session["idadeSelecionada"].ToString());
                }

                if (Session["nfNum"] != null)
                    layoutdiarioexpedicao.NFNum = Session["nfNum"].ToString();
                else
                    layoutdiarioexpedicao.NFNum = "";
                if (Session["GTA"] != null)
                    layoutdiarioexpedicao.GTANum = Session["GTA"].ToString();
                else
                    layoutdiarioexpedicao.GTANum = "";
                if (Session["Lacre"] != null)
                    layoutdiarioexpedicao.Lacre = Session["Lacre"].ToString();
                else
                    layoutdiarioexpedicao.Lacre = "";
                if (Session["Observacao"] != null)
                    layoutdiarioexpedicao.Observacao = Session["Observacao"].ToString();
                else
                    layoutdiarioexpedicao.Observacao = "";
                if (Session["tipoDEOselecionado"] != null)
                    layoutdiarioexpedicao.TipoDEO = Session["tipoDEOselecionado"].ToString();
                else
                    layoutdiarioexpedicao.TipoDEO = "";

                layoutdiarioexpedicao.Incubatorio = incubatorio;
                layoutdiarioexpedicao.Usuario = Session["login"].ToString();
                layoutdiarioexpedicao.DataHora = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                //layoutdiarioexpedicao.NumeroReferencia = DateTime.Now.DayOfYear.ToString();
                layoutdiarioexpedicao.NumeroReferencia = layoutdiarioexpedicao.DataProducao.DayOfYear.ToString();
                layoutdiarioexpedicao.DataHoraCarreg = dataCarreg;
                layoutdiarioexpedicao.TipoDEO = tipoDEO;
                layoutdiarioexpedicao.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
                layoutdiarioexpedicao.ResponsavelCarreg = "";
                layoutdiarioexpedicao.ResponsavelReceb = "";
                layoutdiarioexpedicao.Granja = granja;
                layoutdiarioexpedicao.NumIdentificacao = numIdentificacao;
                layoutdiarioexpedicao.CodItemDEO = 0;
                if (layoutdiarioexpedicao.TipoOvo == null) layoutdiarioexpedicao.TipoOvo = "";
                layoutdiarioexpedicao.QtdeConferencia = 0;

                #region Verifica se já existe na conferencia para não duplicar

                if (origem != "Cadastro")
                {
                    var listaItensConf = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                    if (listaItensConf.Count == 0)
                        listaItensConf = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", "Conferência");

                    int existe = listaItensConf
                        .Where(w => w.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                            && w.DataProducao == layoutdiarioexpedicao.DataProducao)
                        .Count();

                    if (existe > 0)
                    {
                        ViewBag.Erro = "Lote " + layoutdiarioexpedicao.LoteCompleto
                            + " - Data " + layoutdiarioexpedicao.DataProducao.ToShortDateString()
                            + " já existe no DEO!"
                            + " Ajustar a sua quantidade no item existente!";
                        return View("ItemConfereDEO", listaItensConf);
                    }
                }

                #endregion

                #region (COMENTADO POR CAUSA MUITOS PROBLEMAS) Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                //if (DateTime.Today != layoutdiarioexpedicao.DataProducao)
                //{
                //    LOC_ARMAZ localArmazenagem = apoloService.LOC_ARMAZ
                //        .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                //        .FirstOrDefault();

                //    int existe = apoloService.CTRL_LOTE_LOC_ARMAZ
                //        .Where(c => c.CtrlLoteNum == layoutdiarioexpedicao.LoteCompleto
                //            && c.CtrlLoteDataValid == layoutdiarioexpedicao.DataProducao
                //            && c.EmpCod == "1"
                //            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                //            && c.CtrlLoteLocArmazQtdSaldo > 0
                //            && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                //                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                //        //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                //        .Count();

                //    if (existe > 0)
                //    {
                //        #region Localiza os Lotes

                //        var listaLotes = apoloService.CTRL_LOTE_LOC_ARMAZ
                //            .Where(c => c.CtrlLoteNum == layoutdiarioexpedicao.LoteCompleto
                //                //&& c.CtrlLoteDataValid <= layoutdiarioexpedicao.DataProducao
                //                && c.CtrlLoteDataValid == layoutdiarioexpedicao.DataProducao
                //                && c.EmpCod == "1"
                //                && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                //                && c.CtrlLoteLocArmazQtdSaldo > 0
                //                && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                //                    && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                //            //&& l.USERGranjaNucleoFLIP.Contains(granja)))
                //            .OrderByDescending(o => o.CtrlLoteDataValid)
                //            .ToList();

                //        #endregion

                //        int saldo = Convert.ToInt32(layoutdiarioexpedicao.QtdeOvos);
                //        int disponivel = 0;

                //        foreach (var item in listaLotes)
                //        {
                //            #region Verifica quantidade já inserida

                //            int saldoDisponivel = 0;
                //            int qtdInseridaNaoBaixada = 0;

                //            int existeDEOInserido = hlbapp.ImportaDiarioExpedicao
                //                .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                //                    && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                //                    //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                //                    && i.DataProducao == item.CtrlLoteDataValid
                //                    && i.Importado != "Conferido"
                //                    && i.TipoDEO == layoutdiarioexpedicao.TipoDEO)
                //                .Count();

                //            if (existeDEOInserido == 0)
                //            {
                //                qtdInseridaNaoBaixada = 0;
                //            }
                //            else
                //            {
                //                decimal qtdDeoInserido = hlbapp.ImportaDiarioExpedicao
                //                    .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                //                        && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                //                        //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                //                        && i.DataProducao == item.CtrlLoteDataValid
                //                        && i.Importado != "Conferido"
                //                        && i.TipoDEO == layoutdiarioexpedicao.TipoDEO)
                //                    .Sum(s => s.QtdeOvos);

                //                qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                //            }

                //            saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                //            #endregion

                //            if (saldoDisponivel > 0)
                //            {
                //                if (saldo > saldoDisponivel)
                //                {
                //                    #region Se saldo maior que o disponível, insere o disponivel para a Data

                //                    saldo = saldo - saldoDisponivel;
                //                    disponivel = disponivel + saldoDisponivel;

                //                    ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                //                        .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                //                            && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                //                            && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                //                            && i.DataProducao == item.CtrlLoteDataValid)
                //                        .FirstOrDefault();

                //                    if (importaDEO == null)
                //                    {
                //                        #region Se não existe o DEO de Importação, será adicionado

                //                        importaDEO = new ImportaDiarioExpedicao();

                //                        importaDEO.Nucleo = layoutdiarioexpedicao.Nucleo;
                //                        importaDEO.Galpao = layoutdiarioexpedicao.Galpao;
                //                        importaDEO.Lote = layoutdiarioexpedicao.Lote;
                //                        importaDEO.Idade = layoutdiarioexpedicao.Idade;
                //                        importaDEO.Linhagem = layoutdiarioexpedicao.Linhagem;
                //                        importaDEO.LoteCompleto = layoutdiarioexpedicao.LoteCompleto;
                //                        importaDEO.DataProducao = item.CtrlLoteDataValid;
                //                        importaDEO.NumeroReferencia = layoutdiarioexpedicao.NumeroReferencia;
                //                        importaDEO.QtdeOvos = saldoDisponivel;
                //                        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //                        importaDEO.Usuario = layoutdiarioexpedicao.Usuario;
                //                        importaDEO.DataHora = DateTime.Now;
                //                        importaDEO.DataHoraCarreg = layoutdiarioexpedicao.DataHoraCarreg;
                //                        importaDEO.DataHoraRecebInc = layoutdiarioexpedicao.DataHoraRecebInc;
                //                        importaDEO.ResponsavelCarreg = layoutdiarioexpedicao.ResponsavelCarreg;
                //                        importaDEO.ResponsavelReceb = layoutdiarioexpedicao.ResponsavelReceb;
                //                        importaDEO.Granja = layoutdiarioexpedicao.Granja;
                //                        importaDEO.NFNum = layoutdiarioexpedicao.NFNum;
                //                        importaDEO.Importado = layoutdiarioexpedicao.Importado;
                //                        importaDEO.TipoDEO = layoutdiarioexpedicao.TipoDEO;
                //                        importaDEO.GTANum = layoutdiarioexpedicao.GTANum;
                //                        importaDEO.Lacre = layoutdiarioexpedicao.Lacre;
                //                        importaDEO.NumIdentificacao = layoutdiarioexpedicao.NumIdentificacao;

                //                        numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                //                        apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                //                        importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                //                        hlbapp.ImportaDiarioExpedicao.AddObject(importaDEO);

                //                        LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                //                        deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //                        deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //                        hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                //                        #endregion
                //                    }
                //                    else
                //                    {
                //                        #region Se existe, será a atualizada a quantidade

                //                        LayoutDEO_X_ImportaDEO deoXimporta = hlbapp.LayoutDEO_X_ImportaDEO
                //                            .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                //                                && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                //                            .FirstOrDefault();

                //                        if (deoXimporta == null)
                //                        {
                //                            deoXimporta = new LayoutDEO_X_ImportaDEO();

                //                            deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //                            deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //                            hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                //                        }

                //                        importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                //                        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //                        importaDEO.Importado = layoutdiarioexpedicao.Importado;

                //                        #endregion
                //                    }

                //                    #endregion
                //                }
                //                else if (saldo > 0)
                //                {
                //                    #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                //                    ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                //                        .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                //                            && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                //                            && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                //                            && i.DataProducao == item.CtrlLoteDataValid)
                //                        .FirstOrDefault();

                //                    if (importaDEO == null)
                //                    {
                //                        #region Se não existe o DEO de Importação, será adicionado

                //                        importaDEO = new ImportaDiarioExpedicao();

                //                        importaDEO.Nucleo = layoutdiarioexpedicao.Nucleo;
                //                        importaDEO.Galpao = layoutdiarioexpedicao.Galpao;
                //                        importaDEO.Lote = layoutdiarioexpedicao.Lote;
                //                        importaDEO.Idade = layoutdiarioexpedicao.Idade;
                //                        importaDEO.Linhagem = layoutdiarioexpedicao.Linhagem;
                //                        importaDEO.LoteCompleto = layoutdiarioexpedicao.LoteCompleto;
                //                        importaDEO.DataProducao = item.CtrlLoteDataValid;
                //                        importaDEO.NumeroReferencia = layoutdiarioexpedicao.NumeroReferencia;
                //                        importaDEO.QtdeOvos = saldo;
                //                        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //                        importaDEO.Usuario = layoutdiarioexpedicao.Usuario;
                //                        importaDEO.DataHora = DateTime.Now;
                //                        importaDEO.DataHoraCarreg = layoutdiarioexpedicao.DataHoraCarreg;
                //                        importaDEO.DataHoraRecebInc = layoutdiarioexpedicao.DataHoraRecebInc;
                //                        importaDEO.ResponsavelCarreg = layoutdiarioexpedicao.ResponsavelCarreg;
                //                        importaDEO.ResponsavelReceb = layoutdiarioexpedicao.ResponsavelReceb;
                //                        importaDEO.Granja = layoutdiarioexpedicao.Granja;
                //                        importaDEO.NFNum = layoutdiarioexpedicao.NFNum;
                //                        importaDEO.Importado = layoutdiarioexpedicao.Importado;
                //                        importaDEO.TipoDEO = layoutdiarioexpedicao.TipoDEO;
                //                        importaDEO.GTANum = layoutdiarioexpedicao.GTANum;
                //                        importaDEO.Lacre = layoutdiarioexpedicao.Lacre;
                //                        importaDEO.NumIdentificacao = layoutdiarioexpedicao.NumIdentificacao;

                //                        numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                //                        apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                //                        importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                //                        hlbapp.ImportaDiarioExpedicao.AddObject(importaDEO);

                //                        LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                //                        deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //                        deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //                        hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                //                        #endregion
                //                    }
                //                    else
                //                    {
                //                        #region Se existe, será a atualizada a quantidade

                //                        LayoutDEO_X_ImportaDEO deoXimporta = hlbapp.LayoutDEO_X_ImportaDEO
                //                            .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                //                                && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                //                            .FirstOrDefault();

                //                        if (deoXimporta == null)
                //                        {
                //                            deoXimporta = new LayoutDEO_X_ImportaDEO();

                //                            deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //                            deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //                            hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                //                        }

                //                        importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                //                        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //                        importaDEO.Importado = layoutdiarioexpedicao.Importado;

                //                        #endregion
                //                    }

                //                    disponivel = disponivel + saldo;
                //                    saldo = 0;
                //                    break;

                //                    #endregion
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    layoutdiarioexpedicao.Importado = "ImportadoApoloFLIP";

                //    ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                //            .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                //                && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                //                && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                //                && i.DataProducao == layoutdiarioexpedicao.DataProducao)
                //            .FirstOrDefault();

                //    if (importaDEO == null)
                //    {
                //        #region Se não existe o DEO de Importação, será adicionado

                //        importaDEO = new ImportaDiarioExpedicao();

                //        importaDEO.Nucleo = layoutdiarioexpedicao.Nucleo;
                //        importaDEO.Galpao = layoutdiarioexpedicao.Galpao;
                //        importaDEO.Lote = layoutdiarioexpedicao.Lote;
                //        importaDEO.Idade = layoutdiarioexpedicao.Idade;
                //        importaDEO.Linhagem = layoutdiarioexpedicao.Linhagem;
                //        importaDEO.LoteCompleto = layoutdiarioexpedicao.LoteCompleto;
                //        importaDEO.DataProducao = layoutdiarioexpedicao.DataProducao;
                //        importaDEO.NumeroReferencia = layoutdiarioexpedicao.NumeroReferencia;
                //        importaDEO.QtdeOvos = layoutdiarioexpedicao.QtdeOvos;
                //        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //        importaDEO.Usuario = layoutdiarioexpedicao.Usuario;
                //        importaDEO.DataHora = DateTime.Now;
                //        importaDEO.DataHoraCarreg = layoutdiarioexpedicao.DataHoraCarreg;
                //        importaDEO.DataHoraRecebInc = layoutdiarioexpedicao.DataHoraRecebInc;
                //        importaDEO.ResponsavelCarreg = layoutdiarioexpedicao.ResponsavelCarreg;
                //        importaDEO.ResponsavelReceb = layoutdiarioexpedicao.ResponsavelReceb;
                //        importaDEO.Granja = layoutdiarioexpedicao.Granja;
                //        importaDEO.NFNum = layoutdiarioexpedicao.NFNum;
                //        importaDEO.Importado = layoutdiarioexpedicao.Importado;
                //        importaDEO.TipoDEO = layoutdiarioexpedicao.TipoDEO;
                //        importaDEO.GTANum = layoutdiarioexpedicao.GTANum;
                //        importaDEO.Lacre = layoutdiarioexpedicao.Lacre;
                //        importaDEO.NumIdentificacao = layoutdiarioexpedicao.NumIdentificacao;

                //        numero = new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                //        apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                //        importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                //        hlbapp.ImportaDiarioExpedicao.AddObject(importaDEO);

                //        LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                //        deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //        deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //        hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                //        #endregion
                //    }
                //    else
                //    {
                //        #region Se existe, será a atualizada a quantidade

                //        LayoutDEO_X_ImportaDEO deoXimporta = hlbapp.LayoutDEO_X_ImportaDEO
                //            .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                //                && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                //            .FirstOrDefault();

                //        if (deoXimporta == null)
                //        {
                //            deoXimporta = new LayoutDEO_X_ImportaDEO();

                //            deoXimporta.CodItemDEO = layoutdiarioexpedicao.CodItemDEO;
                //            deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                //            hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                //        }

                //        importaDEO.QtdeOvos = importaDEO.QtdeOvos + layoutdiarioexpedicao.QtdeOvos;
                //        importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                //        importaDEO.Importado = layoutdiarioexpedicao.Importado;

                //        #endregion
                //    }
                //}

                #endregion

                hlbappNew.LayoutDiarioExpedicaos.AddObject(layoutdiarioexpedicao);

                HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                if (origem == "Cadastro")
                {
                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Inserido",
                        usuario, 0, "", "", layoutdiarioexpedicao);
                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                }
                else
                {
                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Divergente Inserido",
                        usuario, layoutdiarioexpedicao.QtdDiferenca,
                        "Data e/ou Lote Incorreto", "", layoutdiarioexpedicao);
                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                }

                hlbappNew.SaveChanges();
                hlbappLOG.SaveChanges();

                //Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente",
                //    origem);
                //Session["listaItensDadosAntigosImport"] = CarregarItensDEOImport(hlbapp, dataCarreg, granja);
                Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                if (((List<LayoutDiarioExpedicaos>)Session["listaItensDadosAntigos"]).Count == 0)
                    Session["listaItensDadosAntigos"] = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", origem);

                listaItens = CarregarItensDEO(hlbapp, granja, numIdentificacao, "", "Crescente");
                if (listaItens.Count == 0)
                    listaItens = CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente", "Cadastro");

                if (origem == "Cadastro")
                    return View("Index", listaItens);
                else
                    return View("ItemConfereDEO", listaItens);
            }
            catch(Exception ex)
            {
                if (ex.InnerException == null)
                    ViewBag.Erro = "Erro ao inserir Item do DEO: " + ex.Message;
                else
                    ViewBag.Erro = "Erro ao inserir Item do DEO: " + ex.Message
                        + " / Erro Interno: " + ex.InnerException.Message;
                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, layoutdiarioexpedicao);

                if (origem == "Cadastro")
                {
                    return View("Index", CarregarItensDEO(hlbapp, dataCarreg, granja, "", "Crescente",
                        origem));
                }
                else
                {
                    var listaItens = CarregarItensDEO(hlbapp, dataCarreg, granja, "Sim", "Crescente",
                        origem);
                    return View("ItemConfereDEO", listaItens);
                }
            }
        }

        public ActionResult Edit(int id = 0)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    LayoutDiarioExpedicaos layoutdiarioexpedicao = 
                        hlbapp.LayoutDiarioExpedicaos.Where(w => w.ID == id).First();
                    if (layoutdiarioexpedicao == null)
                    {
                        return HttpNotFound();
                    }
                    CarregaListaNucleos();
                    CarregaLotes(layoutdiarioexpedicao.Nucleo);
                    CarregaGalpoes(layoutdiarioexpedicao.Lote);
                    if (layoutdiarioexpedicao.NumeroReferencia == null) { layoutdiarioexpedicao.NumeroReferencia = ""; }

                    Session["qtdItemAnteriorAlterada"] = Convert.ToInt32(layoutdiarioexpedicao.QtdeOvos);

                    return View(layoutdiarioexpedicao);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        [HttpPost]
        public ActionResult EditItem(LayoutDiarioExpedicaos layoutdiarioexpedicao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    if (ModelState.IsValid)
                    {
                        DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
                        string granja = Session["granjaSelecionada"].ToString();
                        string tipoDEO = Session["tipoDEOselecionado"].ToString();

                        layoutdiarioexpedicao.Granja = granja;
                        layoutdiarioexpedicao.DataHoraCarreg = dataCarreg;
                        layoutdiarioexpedicao.NumeroReferencia = layoutdiarioexpedicao.DataProducao.DayOfYear.ToString();
                        layoutdiarioexpedicao.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
                        if (Session["nfNum"] != null)
                            layoutdiarioexpedicao.NFNum = Session["nfNum"].ToString();
                        else
                            layoutdiarioexpedicao.NFNum = "";
                        if (Session["GTA"] != null)
                            layoutdiarioexpedicao.GTANum = Session["GTA"].ToString();
                        else
                            layoutdiarioexpedicao.GTANum = "";
                        if (Session["Lacre"] != null)
                            layoutdiarioexpedicao.Lacre = Session["Lacre"].ToString();
                        else
                            layoutdiarioexpedicao.Lacre = "";
                        if (Session["Observacao"] != null)
                            layoutdiarioexpedicao.Observacao = Session["Observacao"].ToString();
                        else
                            layoutdiarioexpedicao.Observacao = "";
                        if (layoutdiarioexpedicao.ResponsavelCarreg == null)
                            layoutdiarioexpedicao.ResponsavelCarreg = "";
                        if (layoutdiarioexpedicao.ResponsavelReceb == null)
                            layoutdiarioexpedicao.ResponsavelReceb = "";

                        var listaImport = CarregarItensDEOImport(hlbapp, dataCarreg, granja);
                        int saldo = 0;

                        #region (COMENTADO POR CAUSA MUITOS PROBLEMAS) Exclusão e Ajuste da quantidade deletada.

                        if (listaImport.Count > 0)
                        {
                            var listaImportDEO = hlbapp.ImportaDiarioExpedicao
                                .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                                    && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                    && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                    && i.DataProducao <= layoutdiarioexpedicao.DataProducao)
                                .ToList();

                            saldo = (int)Session["qtdItemAnteriorAlterada"];

                            foreach (var item in listaImportDEO)
                            {
                                if (item.QtdeOvos > saldo)
                                {
                                    item.QtdeOvos = item.QtdeOvos - saldo;
                                    item.QtdeBandejas = item.QtdeOvos / 150;
                                    break;
                                }
                                else
                                {
                                    saldo = saldo - Convert.ToInt32(item.QtdeOvos);

                                    LayoutDEO_X_ImportaDEO relacionamento = hlbapp.LayoutDEO_X_ImportaDEO
                                        .Where(r => r.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                                            && r.CodItemImportaDEO == item.CodItemImportaDEO)
                                        .FirstOrDefault();

                                    if (relacionamento != null)
                                    {
                                        hlbapp.LayoutDEO_X_ImportaDEO.DeleteObject(relacionamento);
                                        hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region (COMENTADO POR CAUSA MUITOS PROBLEMAS) Insere na tabela de DEO p/ Importação (Rateio a quantidade caso não tenha somente em uma data, desde que tenha Estoque

                        //if (listaImport.Count > 0)
                        //{
                            #region Localiza os Lotes

                            LOC_ARMAZ localArmazenagem = apoloService.LOC_ARMAZ
                                .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                                .FirstOrDefault();

                            var listaLotes = apoloService.CTRL_LOTE_LOC_ARMAZ
                                .Where(c => c.CtrlLoteNum == layoutdiarioexpedicao.LoteCompleto
                                    //&& c.CtrlLoteDataValid <= layoutdiarioexpedicao.DataProducao
                                    && c.CtrlLoteDataValid == layoutdiarioexpedicao.DataProducao
                                    && c.EmpCod == "1"
                                    && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                                    && c.CtrlLoteLocArmazQtdSaldo > 0
                                    && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                                        && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid
                                    //&& l.USERGranjaNucleoFLIP.Contains(granja)
                                        ))
                                .OrderByDescending(o => o.CtrlLoteDataValid)
                                .ToList();

                            #endregion

                            saldo = Convert.ToInt32(layoutdiarioexpedicao.QtdeOvos);
                            int disponivel = 0;

                            foreach (var item in listaLotes)
                            {
                                #region Verifica quantidade já inserida

                                int saldoDisponivel = 0;
                                int qtdInseridaNaoBaixada = 0;

                                int existeDEOInserido = hlbapp.ImportaDiarioExpedicao
                                        .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                                            && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                            //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                            && i.DataProducao == item.CtrlLoteDataValid
                                            && i.Importado != "Conferido")
                                        .Count();

                                if (existeDEOInserido == 0)
                                {
                                    qtdInseridaNaoBaixada = 0;
                                }
                                else
                                {
                                    decimal qtdDeoInserido = hlbapp.ImportaDiarioExpedicao
                                        .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                                            && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                            //&& i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                            && i.DataProducao == item.CtrlLoteDataValid
                                            && i.Importado != "Conferido")
                                        .Sum(s => s.QtdeOvos);

                                    qtdInseridaNaoBaixada = Convert.ToInt32(qtdDeoInserido);
                                }

                                saldoDisponivel = Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo) - qtdInseridaNaoBaixada;

                                #endregion

                                if (saldoDisponivel > 0)
                                {
                                    if (saldo > saldoDisponivel)
                                    {
                                        #region Se saldo maior que o disponível, insere o disponivel para a Data

                                        saldo = saldo - saldoDisponivel;
                                        disponivel = disponivel + saldoDisponivel;

                                        ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                                                .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                                                    && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                                    && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                                    && i.DataProducao == item.CtrlLoteDataValid)
                                                .FirstOrDefault();

                                        if (importaDEO == null)
                                        {
                                            #region Se não existe o DEO de Importação, será adicionado

                                            importaDEO = new ImportaDiarioExpedicao();

                                            importaDEO.Nucleo = layoutdiarioexpedicao.Nucleo;
                                            importaDEO.Galpao = layoutdiarioexpedicao.Galpao;
                                            importaDEO.Lote = layoutdiarioexpedicao.Lote;
                                            importaDEO.Idade = layoutdiarioexpedicao.Idade;
                                            importaDEO.Linhagem = layoutdiarioexpedicao.Linhagem;
                                            importaDEO.LoteCompleto = layoutdiarioexpedicao.LoteCompleto;
                                            importaDEO.DataProducao = item.CtrlLoteDataValid;
                                            importaDEO.NumeroReferencia = layoutdiarioexpedicao.NumeroReferencia;
                                            importaDEO.QtdeOvos = saldoDisponivel;
                                            importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                            importaDEO.Usuario = layoutdiarioexpedicao.Usuario;
                                            importaDEO.DataHora = layoutdiarioexpedicao.DataHoraCarreg;
                                            importaDEO.DataHoraCarreg = layoutdiarioexpedicao.DataHoraCarreg;
                                            importaDEO.DataHoraRecebInc = layoutdiarioexpedicao.DataHoraRecebInc;
                                            importaDEO.ResponsavelCarreg = layoutdiarioexpedicao.ResponsavelCarreg;
                                            importaDEO.ResponsavelReceb = layoutdiarioexpedicao.ResponsavelReceb;
                                            importaDEO.Granja = layoutdiarioexpedicao.Granja;
                                            importaDEO.NFNum = layoutdiarioexpedicao.NFNum;
                                            importaDEO.Importado = layoutdiarioexpedicao.Importado;
                                            importaDEO.TipoDEO = layoutdiarioexpedicao.TipoDEO;
                                            importaDEO.GTANum = layoutdiarioexpedicao.GTANum;
                                            importaDEO.Lacre = layoutdiarioexpedicao.Lacre;
                                            importaDEO.NumIdentificacao = layoutdiarioexpedicao.NumIdentificacao;

                                            System.Data.Objects.ObjectParameter numero =
                                                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                                            apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                            importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                            hlbapp.ImportaDiarioExpedicao.AddObject(importaDEO);

                                            LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                            deoXimporta.CodItemDEO = Convert.ToInt32(layoutdiarioexpedicao.CodItemDEO);
                                            deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                            hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                            #endregion
                                        }
                                        else
                                        {
                                            #region Se existe, será a atualizada a quantidade

                                            LayoutDEO_X_ImportaDEO deoXimporta = hlbapp.LayoutDEO_X_ImportaDEO
                                                .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                                                    && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                                .FirstOrDefault();

                                            if (deoXimporta == null)
                                            {
                                                deoXimporta = new LayoutDEO_X_ImportaDEO();

                                                deoXimporta.CodItemDEO = 
                                                    Convert.ToInt32(layoutdiarioexpedicao.CodItemDEO);
                                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                                hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                            }

                                            importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldoDisponivel;
                                            importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                            #endregion
                                        }

                                        #endregion
                                    }
                                    else if (saldo > 0)
                                    {
                                        #region Se saldo for menor que o disponível e maior que zero, adiciona esse restante

                                        ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                                                .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                                                    && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                                                    && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                                                    && i.DataProducao == item.CtrlLoteDataValid)
                                                .FirstOrDefault();

                                        if (importaDEO == null)
                                        {
                                            #region Se não existe o DEO de Importação, será adicionado

                                            importaDEO = new ImportaDiarioExpedicao();

                                            importaDEO.Nucleo = layoutdiarioexpedicao.Nucleo;
                                            importaDEO.Galpao = layoutdiarioexpedicao.Galpao;
                                            importaDEO.Lote = layoutdiarioexpedicao.Lote;
                                            importaDEO.Idade = layoutdiarioexpedicao.Idade;
                                            importaDEO.Linhagem = layoutdiarioexpedicao.Linhagem;
                                            importaDEO.LoteCompleto = layoutdiarioexpedicao.LoteCompleto;
                                            importaDEO.DataProducao = item.CtrlLoteDataValid;
                                            importaDEO.NumeroReferencia = layoutdiarioexpedicao.NumeroReferencia;
                                            importaDEO.QtdeOvos = saldo;
                                            importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;
                                            importaDEO.Usuario = layoutdiarioexpedicao.Usuario;
                                            importaDEO.DataHora = layoutdiarioexpedicao.DataHoraCarreg;
                                            importaDEO.DataHoraCarreg = layoutdiarioexpedicao.DataHoraCarreg;
                                            importaDEO.DataHoraRecebInc = layoutdiarioexpedicao.DataHoraRecebInc;
                                            importaDEO.ResponsavelCarreg = layoutdiarioexpedicao.ResponsavelCarreg;
                                            importaDEO.ResponsavelReceb = layoutdiarioexpedicao.ResponsavelReceb;
                                            importaDEO.Granja = layoutdiarioexpedicao.Granja;
                                            importaDEO.NFNum = layoutdiarioexpedicao.NFNum;
                                            importaDEO.Importado = layoutdiarioexpedicao.Importado;
                                            importaDEO.TipoDEO = layoutdiarioexpedicao.TipoDEO;
                                            importaDEO.GTANum = layoutdiarioexpedicao.GTANum;
                                            importaDEO.Lacre = layoutdiarioexpedicao.Lacre;
                                            importaDEO.NumIdentificacao = layoutdiarioexpedicao.NumIdentificacao;

                                            System.Data.Objects.ObjectParameter numero =
                                                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

                                            apoloService.gerar_codigo("1", "ImportaDiarioExpedicao", numero);

                                            importaDEO.CodItemImportaDEO = Convert.ToInt32(numero.Value);

                                            hlbapp.ImportaDiarioExpedicao.AddObject(importaDEO);

                                            LayoutDEO_X_ImportaDEO deoXimporta = new LayoutDEO_X_ImportaDEO();

                                            deoXimporta.CodItemDEO = (int)layoutdiarioexpedicao.CodItemDEO;
                                            deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                            hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);

                                            #endregion
                                        }
                                        else
                                        {
                                            #region Se existe, será a atualizada a quantidade

                                            LayoutDEO_X_ImportaDEO deoXimporta = hlbapp.LayoutDEO_X_ImportaDEO
                                                .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO
                                                    && l.CodItemImportaDEO == importaDEO.CodItemImportaDEO)
                                                .FirstOrDefault();

                                            if (deoXimporta == null)
                                            {
                                                deoXimporta = new LayoutDEO_X_ImportaDEO();

                                                deoXimporta.CodItemDEO = (int)layoutdiarioexpedicao.CodItemDEO;
                                                deoXimporta.CodItemImportaDEO = (int)importaDEO.CodItemImportaDEO;

                                                hlbapp.LayoutDEO_X_ImportaDEO.AddObject(deoXimporta);
                                            }

                                            importaDEO.QtdeOvos = importaDEO.QtdeOvos + saldo;
                                            importaDEO.QtdeBandejas = importaDEO.QtdeOvos / 150;

                                            #endregion
                                        }

                                        disponivel = disponivel + saldo;
                                        saldo = 0;
                                        break;

                                        #endregion
                                    }
                                }
                            }
                        //}
                        #endregion

                        //db.Entry(layoutdiarioexpedicao).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();
                        hlbapp.SaveChanges();

                        string usuario = Session["login"].ToString();

                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, "Alteração", usuario, 0, layoutdiarioexpedicao));

                        //hlbapp.SaveChanges();
                    }

                    //escondeLinkPrincipal = "Sim";
                    return View("Index", CarregarItensDEO(hlbapp, layoutdiarioexpedicao.DataHoraCarreg,
                        layoutdiarioexpedicao.Granja, "", "Crescente", "Cadastro"));
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult Delete(int id = 0, string origem = "")
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            
            Session["origemCreateItemDEO"] = origem;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LayoutDiarioExpedicaos layoutdiarioexpedicao = 
                hlbapp.LayoutDiarioExpedicaos.Where(w=> w.ID == id).FirstOrDefault();
            if (layoutdiarioexpedicao == null)
            {
                return HttpNotFound();
            }

            return View(layoutdiarioexpedicao);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LayoutDiarioExpedicaos layoutdiarioexpedicao = 
                hlbapp.LayoutDiarioExpedicaos.Where(w => w.ID == id).FirstOrDefault();

            try
            {
                if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

                string origem = Session["origemCreateItemDEO"].ToString();

                if (Session["usuario"] != null)
                {
                    if (Session["usuario"].ToString() != "0")
                    {
                        #region (COMENTADO POR CAUSA MUITOS PROBLEMAS) Exclusão e Ajuste da quantidade deletada.

                        //var listaImport = CarregarItensDEOImport(layoutdiarioexpedicao.DataHoraCarreg, layoutdiarioexpedicao.Granja);

                        //if (listaImport.Count > 0)
                        //{
                        //    var listaImportDEO = hlbapp.ImportaDiarioExpedicao
                        //        .Where(i => i.Granja == layoutdiarioexpedicao.Granja
                        //            && i.LoteCompleto == layoutdiarioexpedicao.LoteCompleto
                        //            && i.DataHoraCarreg == layoutdiarioexpedicao.DataHoraCarreg
                        //            && i.DataProducao == layoutdiarioexpedicao.DataProducao)
                        //        .ToList();

                        //    int saldo = Convert.ToInt32(layoutdiarioexpedicao.QtdeOvos);

                        //    foreach (var item in listaImportDEO)
                        //    {
                        //        if (item.QtdeOvos > saldo)
                        //        {
                        //            item.QtdeOvos = item.QtdeOvos - saldo;
                        //            item.QtdeBandejas = item.QtdeOvos / 150;
                        //            break;
                        //        }
                        //        else
                        //        {
                        //            saldo = saldo - Convert.ToInt32(item.QtdeOvos);

                        //            hlbapp.ImportaDiarioExpedicao.DeleteObject(item);
                        //        }
                        //    }

                        //    var listaRelacionamento = hlbapp.LayoutDEO_X_ImportaDEO
                        //        .Where(l => l.CodItemDEO == layoutdiarioexpedicao.CodItemDEO)
                        //        .ToList();

                        //    foreach (var item in listaRelacionamento)
                        //    {
                        //        hlbapp.LayoutDEO_X_ImportaDEO.DeleteObject(item);
                        //    }
                        //}

                        #endregion

                        #region Se for Importado p/ Apolo / FLIP na verificação do Estoque, precisa ser deletado

                        if ((layoutdiarioexpedicao.Importado.Equals("ImportadoApoloFLIP"))
                            || (layoutdiarioexpedicao.Importado.Equals("Conferido") || layoutdiarioexpedicao.Granja.Equals("PL")))
                        {
                            #region Deleção do Estoque no Apolo - DESATIVADO

                            //string msgErroDeleteApolo = "";
                            //msgErroDeleteApolo = DeleteTLAApolo(layoutdiarioexpedicao.DataHoraCarreg, 
                            //    layoutdiarioexpedicao.Granja, layoutdiarioexpedicao.TipoDEO);
                            //if (msgErroDeleteApolo != "")
                            //{
                            //    ViewBag.Erro = msgErroDeleteApolo;
                            //    return View("Delete");
                            //}

                            #endregion

                            #region Deleta Saídas do FLIP (Comentado pois não necessário)

                            /*if ((layoutdiarioexpedicao.Granja.Equals("CH")) || (layoutdiarioexpedicao.Granja.Equals("PH")))
                        {
                            if (layoutdiarioexpedicao.TipoDEO != null)
                            {
                                string login = Session["login"].ToString();
                                string usuarioFLIP = "";
                                if (login.Equals("palves"))
                                    usuarioFLIP = "RIOSOFT";
                                else
                                    usuarioFLIP = login.ToUpper();

                                LOC_ARMAZ localArmaz = apoloService.LOC_ARMAZ
                                            .Where(l => l.USERCodigoFLIP == layoutdiarioexpedicao.Granja 
                                                && l.USERTipoProduto == "Ovos Incubáveis")
                                            .FirstOrDefault();

                                if (layoutdiarioexpedicao.TipoDEO.Equals("Transf. Ovos Incubáveis"))
                                    TransferEggsFLIP(layoutdiarioexpedicao, localArmaz.USERGeracaoFLIP,
                                        usuarioFLIP, "DEL");
                                else
                                    ImportaSaidaFLIP(layoutdiarioexpedicao, localArmaz.USERGeracaoFLIP,
                                        usuarioFLIP, "DEL", layoutdiarioexpedicao.TipoDEO);
                            }
                        }*/

                            #endregion

                            #region Deleta FLIP

                            MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL verificaEmpresa = bdApolo.EMPRESA_FILIAL
                                .Where(e => e.USERFLIPCod == layoutdiarioexpedicao.Granja
                                    || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod
                                        && c.EmpFilCertificNum == layoutdiarioexpedicao.Granja))
                                .FirstOrDefault();

                            if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || layoutdiarioexpedicao.Granja.Equals("PL"))
                            {
                                if (layoutdiarioexpedicao.Importado.Equals("ImportadoApoloFLIP"))
                                {
                                    flocks.Fill(flip.FLOCKS);

                                    FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                        .Where(f => f.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto)
                                        .FirstOrDefault();

                                    flock_data.FillFlockData(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                        layoutdiarioexpedicao.DataProducao);

                                    FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == layoutdiarioexpedicao.LoteCompleto
                                                    && d.TRX_DATE == layoutdiarioexpedicao.DataProducao)
                                                .FirstOrDefault();

                                    if (dataRow != null)
                                    {
                                        if ((dataRow.NUM_1 - layoutdiarioexpedicao.QtdeOvos) > 0)
                                        {
                                            dataRow.NUM_1 = dataRow.NUM_1 - layoutdiarioexpedicao.QtdeOvos;
                                            dataRow.NUM_8 = 0;
                                            flock_data.Update(dataRow);
                                        }
                                        else
                                        {
                                            flock_data.Delete(dataRow.COMPANY, dataRow.REGION, dataRow.LOCATION, dataRow.FARM_ID, dataRow.FLOCK_ID,
                                                dataRow.TRX_DATE);
                                        }
                                    }
                                }
                            }

                            #endregion
                        }

                        #endregion

                        //db.DiarioExpedicao.Remove(layoutdiarioexpedicao);
                        //db.SaveChanges();
                        hlbapp.LayoutDiarioExpedicaos.DeleteObject(layoutdiarioexpedicao);

                        HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Deletado",
                            Session["usuario"].ToString(), 0, "", "", layoutdiarioexpedicao);

                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                        hlbappLOG.SaveChanges();

                        hlbapp.SaveChanges();

                        string usuario = Session["login"].ToString();

                        //hlbapp.LOG_LayoutDiarioExpedicaos.AddObject(InsereLOG(DateTime.Now, "Exclusão", usuario, 0, layoutdiarioexpedicao));

                        //hlbapp.SaveChanges();

                        var lista = CarregarItensDEO(hlbapp, layoutdiarioexpedicao.Granja, layoutdiarioexpedicao.NumIdentificacao, "", "Crescente");
                        if (lista.Count == 0)
                            lista = CarregarItensDEO(hlbapp, layoutdiarioexpedicao.DataHoraCarreg, layoutdiarioexpedicao.Granja, "", "Crescente", origem);

                        Session["listaItensDadosAntigos"] = lista;
                        //Session["listaItensDadosAntigosImport"] = CarregarItensDEOImport(hlbapp,
                        //    layoutdiarioexpedicao.DataHoraCarreg, layoutdiarioexpedicao.Granja);

                        //if (lista.Count == 0)
                        //    escondeLinkPrincipal = "Não";
                        //else
                        //    escondeLinkPrincipal = "Sim";

                        if ((MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                            .GetGroup("HLBAPPM-GerarDiarioExpedicao", (System.Collections.ArrayList)Session["Direitos"]))
                            && (Session["granjaSelecionada"].ToString().Equals("SB")))
                        {
                            return View("DEOGerado", lista);
                        }
                        else
                        {
                            if (origem == "Cadastro")
                                return View("Index", lista);
                            else
                                return View("ItemConfereDEO", lista);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "AccountMobile");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            catch (Exception ex)
            {
                hlbapp.Refresh(System.Data.Objects.RefreshMode.StoreWins, layoutdiarioexpedicao);

                var lista = CarregarItensDEO(hlbapp, layoutdiarioexpedicao.DataHoraCarreg,
                    layoutdiarioexpedicao.Granja, "", "Crescente", "Cadastro");
                string msg = "";
                if (ex.InnerException != null)
                    msg = ex.Message + " / " + ex.InnerException.Message;
                else
                    msg = ex.Message;

                ViewBag.Erro = "Erro ao deletar item " + layoutdiarioexpedicao.ID.ToString() + " do DEO: " + msg;
                return View("Index", lista);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region LOG

        public LOG_LayoutDiarioExpedicaos InsereLOG(DateTime datahora, string operacao, string usuarioOperacao, 
            decimal? qtdFalta, string motivoDivergenciaDEO, string observacao, LayoutDiarioExpedicaos deo)
        {
            LOG_LayoutDiarioExpedicaos log = new LOG_LayoutDiarioExpedicaos();

            log.DataHoraOper = datahora;
            log.Operacao = operacao;
            log.UsuarioOperacao = usuarioOperacao;
            log.QtdFalta = qtdFalta;

            log.Nucleo = deo.Nucleo;
            log.Galpao = deo.Galpao;
            log.Lote = deo.Lote;
            log.Idade = deo.Idade;
            log.Linhagem = deo.Linhagem;
            log.LoteCompleto = deo.LoteCompleto;
            log.DataProducao = deo.DataProducao;
            log.NumeroReferencia = deo.NumeroReferencia;
            log.QtdeOvos = deo.QtdeOvos;
            log.QtdeBandejas = deo.QtdeBandejas;
            log.Usuario = deo.Usuario;
            log.DataHora = deo.DataHora;
            log.DataHoraCarreg = deo.DataHoraCarreg;
            log.DataHoraRecebInc = deo.DataHoraRecebInc;
            log.ResponsavelCarreg = deo.ResponsavelCarreg;
            log.ResponsavelReceb = deo.ResponsavelReceb;
            log.NFNum = deo.NFNum;
            log.Granja = deo.Granja;
            log.Importado = deo.Importado;
            log.Incubatorio = deo.Incubatorio;
            log.TipoDEO = deo.TipoDEO;

            log.MotivoDivergenciaDEO = motivoDivergenciaDEO;
            log.Observacao = observacao;
            log.QtdeConferencia = deo.QtdeConferencia;
            log.TipoOvo = deo.TipoOvo;
            log.NumIdentificacao = deo.NumIdentificacao;
            log.TemperaturaOvoInterna = deo.TemperaturaOvoInterna;

            return log;
        }

        public void InsereLOGAQO(int idAQO, string operacao, string naoConformidade, string observacao,
            string resposta, string status)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LOG_Analise_Qualidade_Ovo log = new LOG_Analise_Qualidade_Ovo();

            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.Operacao = operacao;
            log.IDAQO = idAQO;
            log.NaoConformidade = naoConformidade;
            log.Observacao = observacao;
            log.Resposta = resposta;
            log.Status = status;

            hlbapp.LOG_Analise_Qualidade_Ovo.AddObject(log);

            hlbapp.SaveChanges();
        }

        #endregion

        #region Métodos de Rastreabilidade

        public static bool ExisteSaldo(string local, DateTime dataHoraCarreg, string lote,
            DateTime dataProducao, int qtd)
        {
            HLBAPPEntities hlbappOK = new HLBAPPEntities();

            string loteSelecionado = "";
            DateTime dataProducaoSelecionada =
                Convert.ToDateTime("2016-10-25 00:00:00.000");
            if (lote.Equals("BFD39403DW")
                && dataProducao.Equals(dataProducaoSelecionada))
                loteSelecionado = lote;

            DateTime dataCarreg = Convert.ToDateTime(dataHoraCarreg.ToShortDateString());

            int qtdSaldo = 0;
            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbappOK.CTRL_LOTE_LOC_ARMAZ_WEB
                 .Where(w => w.Local == local 
                 //.Where(w => (w.Local.Substring(0, 1) == local || w.Local == local)
                     && w.LoteCompleto == lote 
                     && w.DataProducao == dataProducao)
                 .FirstOrDefault();

            if (saldo != null) qtdSaldo = Convert.ToInt32(saldo.Qtde);

            if ((qtdSaldo - qtd) >= 0) return true;

            return false;
        }

        public static bool ExisteSaldoDEO(string granja, DateTime dataHoraCarreg, string numIdentificacao, bool comercio)
        {
            HLBAPPEntities hlbappOK = new HLBAPPEntities();

            var listaItensDEO = hlbappOK.LayoutDiarioExpedicaos
                .Where(d =>
                    (d.DataHoraCarreg == dataHoraCarreg && d.Granja == granja && numIdentificacao == "")
                    ||
                    (d.NumIdentificacao == numIdentificacao && numIdentificacao != "")
                )
                .GroupBy(g => new {
                    g.LoteCompleto,
                    g.DataProducao,
                    g.TipoOvo,
                    g.Incubatorio
                })
                .Select(s => new {
                    LoteCompleto = s.Key.LoteCompleto,
                    DataProducao = s.Key.DataProducao,
                    TipoOvo = s.Key.TipoOvo,
                    Incubatorio = s.Key.Incubatorio,
                    QtdeOvos = s.Sum(m => 
                        m.Importado == "Conferido" ?
                            m.QtdeOvos + (m.QtdDiferenca == null ? 0 : m.QtdDiferenca) :
                            0)
                })
                .ToList();

            bool existeSaldo = true;

            foreach (var item in listaItensDEO)
            {
                bool verificaSaldo = true;
                string local = "";
                if (item.TipoOvo == "")
                    local = item.Incubatorio;
                else
                {
                    local = item.TipoOvo;
                    var naoEIncubavel = hlbappOK.TIPO_CLASSFICACAO_OVO
                        .Where(w => w.CodigoTipo == local && w.AproveitamentoOvo != "Incubável").FirstOrDefault();
                    if (naoEIncubavel != null) verificaSaldo = false;
                }

                if (verificaSaldo)
                {
                    int qtdOvos = Convert.ToInt32(item.QtdeOvos);

                    //existeSaldo = ExisteSaldo(item.Incubatorio, dataHoraCarreg, item.LoteCompleto, item.DataProducao, qtdOvos)
                    string lote = item.LoteCompleto;
                    DateTime dataPrd = item.DataProducao;
                    if (comercio)
                    {
                        if (local.Substring(local.Length - 1, 1) != "C") local = local + "C";
                        lote = "VARIOS";
                        dataPrd = Convert.ToDateTime("1988-01-01 00:00:00.000");
                    }
                    existeSaldo = ExisteSaldo(local, dataHoraCarreg, lote, dataPrd, qtdOvos);
                    if (existeSaldo == false)
                        break;
                }
            }

            return existeSaldo;
        }

        public static bool ExisteItensConferidosDEO(string granja, DateTime dataHoraCarreg, string numIdentificacao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaItensDEO = hlbapp.LayoutDiarioExpedicaos
                .Where(d =>
                    ((d.DataHoraCarreg == dataHoraCarreg && d.Granja == granja && numIdentificacao == "")
                    ||
                    (d.NumIdentificacao == numIdentificacao && numIdentificacao != ""))
                    && d.Importado == "Conferido")
                .ToList();

            if (listaItensDEO.Count() > 0)
                return true;
            else
                return false;
        }

        public static bool ExisteItensDivergentesDEO(string granja, DateTime dataHoraCarreg, string numIdentificacao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaItensDEO = hlbapp.LayoutDiarioExpedicaos
                .Where(d =>
                    ((d.DataHoraCarreg == dataHoraCarreg && d.Granja == granja && numIdentificacao == "")
                    ||
                    (d.NumIdentificacao == numIdentificacao && numIdentificacao != ""))
                    && d.Importado == "Divergência")
                .ToList();

            if (listaItensDEO.Count() > 0)
                return true;
            else
                return false;
        }

        public ActionResult RastreabilidadeDEO(string granja, DateTime dataHoraCarreg, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["dataHoraCarreg"] = dataHoraCarreg;
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaItensDEO = hlbapp.LayoutDiarioExpedicaos
                .Where(d => d.Granja == granja 
                    && ((d.DataHoraCarreg == dataHoraCarreg && numIdentificacao == "")
                            || (d.NumIdentificacao == numIdentificacao)))
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao,
                    g.TipoOvo,
                    g.Incubatorio
                })
                .Select(s => new
                {
                    LoteCompleto = s.Key.LoteCompleto,
                    DataProducao = s.Key.DataProducao,
                    TipoOvo = s.Key.TipoOvo,
                    Incubatorio = s.Key.Incubatorio,
                    QtdeOvos = s.Sum(m =>
                        m.Importado == "Conferido" ?
                            m.QtdeOvos + (m.QtdDiferenca == null ? 0 : m.QtdDiferenca) :
                            0)
                })
                .ToList();

            List<LayoutDiarioExpedicaos> listaItensSemSaldo = new List<LayoutDiarioExpedicaos>();

            bool existeSaldo = true;

            foreach (var item in listaItensDEO)
            {
                existeSaldo = ExisteSaldo(item.Incubatorio, dataHoraCarreg, item.LoteCompleto,
                    item.DataProducao, Convert.ToInt32(item.QtdeOvos));

                if (existeSaldo == false)
                {
                    var listaItensDEOSemSaldo = hlbapp.LayoutDiarioExpedicaos
                        .Where(d => d.Granja == granja
                            && ((d.DataHoraCarreg == dataHoraCarreg && numIdentificacao == "")
                                || (d.NumIdentificacao == numIdentificacao))
                            && d.LoteCompleto == item.LoteCompleto
                            && d.DataProducao == item.DataProducao)
                        .ToList();

                    foreach (var itemSemSaldo in listaItensDEOSemSaldo)
                    {
                        listaItensSemSaldo.Add(itemSemSaldo);		 
                    }
                }
            }

            return View(listaItensSemSaldo);
        }

        public ActionResult RastreabilidadeDEOReturn()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string granja = Session["granjaSelecionada"].ToString();
            DateTime dataHoraCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaItensDEO = hlbapp.LayoutDiarioExpedicaos
                .Where(d => d.DataHoraCarreg == dataHoraCarreg && d.Granja == granja)
                .GroupBy(g => new
                {
                    g.LoteCompleto,
                    g.DataProducao,
                    g.TipoOvo,
                    g.Incubatorio
                })
                .Select(s => new
                {
                    LoteCompleto = s.Key.LoteCompleto,
                    DataProducao = s.Key.DataProducao,
                    TipoOvo = s.Key.TipoOvo,
                    Incubatorio = s.Key.Incubatorio,
                    QtdeOvos = s.Sum(m =>
                        m.Importado == "Conferido" ?
                            m.QtdeOvos + (m.QtdDiferenca == null ? 0 : m.QtdDiferenca) :
                            0)
                })
                .ToList();

            List<LayoutDiarioExpedicaos> listaItensSemSaldo = new List<LayoutDiarioExpedicaos>();

            bool existeSaldo = true;

            foreach (var item in listaItensDEO)
            {
                existeSaldo = ExisteSaldo(item.Incubatorio, dataHoraCarreg, item.LoteCompleto,
                    item.DataProducao, Convert.ToInt32(item.QtdeOvos));

                if (existeSaldo == false)
                {
                    var listaItensDEOSemSaldo = hlbapp.LayoutDiarioExpedicaos
                            .Where(d => d.DataHoraCarreg == dataHoraCarreg && d.Granja == granja
                                && d.LoteCompleto == item.LoteCompleto
                                && d.DataProducao == item.DataProducao)
                            .ToList();

                    foreach (var itemSemSaldo in listaItensDEOSemSaldo)
                    {
                        listaItensSemSaldo.Add(itemSemSaldo);
                    }
                }
            }

            return View("RastreabilidadeDEO", listaItensSemSaldo);
        }

        public ActionResult RastreabilidadeLote(string lote, DateTime dataProducao, string chamada)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["chamadaRastreabilidade"] = chamada;
            Session["loteRatreabilidadeLote"] = lote;
            Session["dataProducaoRatreabilidadeLote"] = dataProducao;

            return View();
        }

        public static int RetornaSaldo(string local, string lote,
            DateTime dataProducao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int qtdSaldo = 0;
            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                 .Where(w => w.Local == local && w.LoteCompleto == lote && w.DataProducao == dataProducao
                    && w.Qtde != 0)
                 .FirstOrDefault();

            if (saldo != null) qtdSaldo = Convert.ToInt32(saldo.Qtde);

            return qtdSaldo;
        }

        #endregion

        #region Métodos Verificação

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

        public string VerificaEstoqueInterno(string dataPrd, string numLote, string qtdeOvos)
        {
            string retorno = "";
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (!Session["tipoDEOselecionado"].ToString().Equals("Inventário de Ovos"))
            {
                string mensagemExiste = "";
                int qtdeExiste = 0;

                int qtd = Convert.ToInt32(qtdeOvos);

                DateTime dataProducao = Convert.ToDateTime(dataPrd);
                string empresa = Session["granjaSelecionada"].ToString();
                string tipoDEO = Session["tipoDEOselecionado"].ToString();

                DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);

                string empresaEstoque = "";
                string empresaLocal = "";
                if (empresa.Equals("PL"))
                {
                    if (!tipoDEO.Equals("Ovos Incubáveis"))
                        empresaLocal = "NM";
                    else
                        empresaLocal = empresa;
                    if (Session["linhagemSelecionada"].ToString().Contains("DKB"))
                        empresaEstoque = "PL";
                    else
                        empresaEstoque = "CH";
                }
                else
                {
                    empresaLocal = empresa;
                    empresaEstoque = "CH";
                }

                var existe = hlbapp.LayoutDiarioExpedicaos
                    .Where(e => e.LoteCompleto == numLote
                        && e.DataProducao == dataProducao
                        && e.Granja == empresa
                        //&& e.Incubatorio == empresaLocal
                        //&& e.DataHoraCarreg == dataCarreg
                        && e.Importado != "Conferido"
                        && e.TipoDEO != "Inventário de Ovos")
                    //.FirstOrDefault();
                    .Count();

                if (existe > 0)
                {
                    existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == numLote
                            && e.DataProducao == dataProducao
                            && e.Granja == empresa
                            //&& e.Incubatorio == empresaLocal
                            //&& e.DataHoraCarreg == dataCarreg
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos")
                        //.FirstOrDefault();
                        .Sum(s => s.QtdeOvos));

                    mensagemExiste = " (Qtde. já adicionada não conferida: " + Convert.ToInt32(existe).ToString() + " ovos.) ";
                    qtdeExiste = Convert.ToInt32(existe);
                }

                MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresaObject = bdApolo.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod == empresaEstoque)
                    .FirstOrDefault();

                #region Código antigo de verificação, onde a quantidade do DEO era a mesma a ser importada

                CTRL_LOTE_LOC_ARMAZ lote;

                if (empresaObject != null)
                {
                    LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                               .Where(l => l.USERCodigoFLIP == empresaLocal 
                                   && l.USERTipoProduto == "Ovos Incubáveis")
                               .FirstOrDefault();

                    if (locArmaz != null)
                    {
                        lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                            .Where(c => c.CtrlLoteNum == numLote && c.CtrlLoteDataValid == dataProducao
                                && c.EmpCod == empresaObject.EmpCod
                                && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                && c.CtrlLoteLocArmazQtdSaldo > 0)
                            .FirstOrDefault();
                    }
                    else
                    {
                        locArmaz = apoloService.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == empresaLocal)
                            .FirstOrDefault();

                        lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                            .Where(c => c.CtrlLoteNum == numLote && c.CtrlLoteDataValid == dataProducao
                                && c.EmpCod == empresaObject.EmpCod
                                && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                && c.CtrlLoteLocArmazQtdSaldo > 0)
                            .FirstOrDefault();
                    }
                }
                else
                {
                    ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 =
                        apoloService.ENTIDADE1.Where(e1 => e1.USERFLIPCodigo == empresaLocal).FirstOrDefault();

                    LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == empresa)
                            .FirstOrDefault();

                    lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                        .Where(c => c.CtrlLoteNum == numLote && c.CtrlLoteDataValid == dataProducao
                            && c.EmpCod == entidade1.USERCodIncFLIPEntrada
                            && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                            && c.CtrlLoteLocArmazQtdSaldo > 0)
                        .FirstOrDefault();
                }

                if (lote != null)
                {
                    if (dataProducao == DateTime.Today ||
                        (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                    {
                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                        if (verificaEmpresa != null)
                        {
                            if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                            {
                                flocks.Fill(flip.FLOCKS);

                                int existeLoteCadastradoFLIP = flip.FLOCKS
                                    .Where(f => f.FLOCK_ID == numLote)
                                    .Count();

                                if (existeLoteCadastradoFLIP > 0)
                                {
                                    #region Inclui no FLIP

                                    FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == numLote)
                                            .FirstOrDefault();

                                    flock_data.FillByFlockData2(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                        dataProducao);

                                    int existeDiarioLoteCadastradoFLIP = flip.FLOCK_DATA
                                        .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                        .Count();

                                    if (existeDiarioLoteCadastradoFLIP == 0)
                                    {
                                        int age = (((DateTime.Today - flock.MOVE_DATE).Days) / 7) + 1;

                                        flock_data.Insert(flock.COMPANY, flock.REGION, flock.LOCATION,
                                            flock.FARM_ID, flock.FLOCK_ID, 1, dataProducao, age, null, null, null,
                                                null, null, null, null, null, null, null, null, qtd, null,
                                                null, null, null, null, null, 0, null, null, null, null,
                                                null, null, null, null, null, null, null, null, null, null, null,
                                                null, null, null, null);

                                        #region Inclui no Apolo

                                        //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                        //{
                                        //    #region Carrega Lote

                                        //    string naturezaOperacao = "1.556.001";
                                        //    decimal? valorUnitario = 0.25m;
                                        //    string unidadeMedida = "UN";
                                        //    short? posicaoUnidadeMedida = 1;
                                        //    string tribCod = "040";
                                        //    string itMovEstqClasFiscCodNbm = "04079000";
                                        //    string clasFiscCod = "0000129";
                                        //    string operacao = "Entrada";

                                        //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                        //        .Where(l => l.USERCodigoFLIP == empresa)
                                        //        .FirstOrDefault();

                                        //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                        //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                        //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                        //    farms.Fill(farmsImport);

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                        //        .Where(f => f.FARM.StartsWith(empresa))
                                        //        .FirstOrDefault();

                                        //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                        //        .Where(e => e.EmpCod == "1")
                                        //        .FirstOrDefault();

                                        //    string entCod = "";

                                        //    if (farm.TERCEIRO.Equals("SIM"))
                                        //    {
                                        //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                        //            .Where(e => e.USERFLIPCodigo == empresa)
                                        //            .FirstOrDefault();

                                        //        entCod = entidade1.EntCod;
                                        //    }
                                        //    else
                                        //    {
                                        //        entCod = empresaFilial.EntCod;
                                        //    }

                                        //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                        //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();

                                        //    #endregion

                                        //    if (loteItemMovEstq == null)
                                        //    {
                                        //        #region Carrega Produto

                                        //        string replace = "" + (char)13 + (char)10;

                                        //        PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                        //        ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        //            && im.ProdCodEstr == produto.ProdCodEstr
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();
                                        //        #endregion

                                        //        if (itemMovEstq == null)
                                        //        {
                                        //            #region Carrega Movimentação. Se não existe, insere.
                                        //            MOV_ESTQ movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO)
                                        //            .FirstOrDefault();

                                        //            if (movEstq == null)
                                        //            {
                                        //                movEstq = service.InsereMovEstq(empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao, "RIOSOFT");

                                        //                apoloService.MOV_ESTQ.AddObject(movEstq);

                                        //                apoloService.SaveChanges();
                                        //            }
                                        //            #endregion

                                        //            #region Se Item não existe, insere item, local e lote
                                        //            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao,
                                        //                flock.VARIETY, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                                        //                itMovEstqClasFiscCodNbm,
                                        //                clasFiscCod);

                                        //            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        //                qtd, localArmazenagem);

                                        //            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                        //            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "INS");

                                        //            apoloService.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                        //            #endregion
                                        //        }
                                        //        else
                                        //        {
                                        //            #region Se existe Item, insere lote e atuliza item e local

                                        //            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                        //            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        //                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        //                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        //                .FirstOrDefault();

                                        //            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                        //            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "UPD");

                                        //            apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                        //            #endregion
                                        //        }
                                        //    }
                                        //}

                                        #endregion

                                        retorno = "ImportadoApoloFLIP";
                                    }
                                    else
                                    {
                                        int existeItensImportadoApoloFLIP = hlbapp.LayoutDiarioExpedicaos
                                            .Where(l => l.LoteCompleto == numLote && l.DataProducao == dataProducao
                                                && l.Importado == "ImportadoApoloFLIP")
                                            .Count();

                                        if (existeItensImportadoApoloFLIP > 0)
                                        {
                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                                .FirstOrDefault();

                                            if (!dataRow.IsNUM_1Null())
                                                dataRow.NUM_1 = dataRow.NUM_1 + qtd;
                                            else
                                                dataRow.NUM_1 = qtd;

                                            dataRow.NUM_8 = 0;

                                            flock_data.Update(dataRow);

                                            #region Atualiza Apolo

                                            //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                            //{
                                            //    #region Se existe o lote atualiza a quantidade

                                            //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                            //    .Where(l => l.USERCodigoFLIP == empresa)
                                            //    .FirstOrDefault();

                                            //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                            //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                            //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                            //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                            //    farms.Fill(farmsImport);

                                            //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                            //        .Where(f => f.FARM.StartsWith(empresa))
                                            //        .FirstOrDefault();

                                            //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                            //        .Where(e => e.EmpCod == "1")
                                            //        .FirstOrDefault();

                                            //    string entCod = "";

                                            //    if (farm.TERCEIRO.Equals("SIM"))
                                            //    {
                                            //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                            //            .Where(e => e.USERFLIPCodigo == empresa)
                                            //            .FirstOrDefault();

                                            //        entCod = entidade1.EntCod;
                                            //    }
                                            //    else
                                            //    {
                                            //        entCod = empresaFilial.EntCod;
                                            //    }

                                            //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                            //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                            //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                            //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                            //        .FirstOrDefault();

                                            //    decimal? qtdAntiga = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            //    loteItemMovEstq.CtrlLoteItMovEstqQtd = loteItemMovEstq.CtrlLoteItMovEstqQtd + qtd;
                                            //    loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            //    PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                            //    ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                            //        && im.ProdCodEstr == produto.ProdCodEstr
                                            //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                            //            && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                            //    .FirstOrDefault();

                                            //    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                            //    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                            //    LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                            //        && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                            //        && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                            //        .FirstOrDefault();

                                            //    localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                            //    localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                            //    apoloService.SaveChanges();

                                            //    apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                            //    #endregion
                                            //}

                                            #endregion
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                }
                            }
                        }
                        else
                        {
                            ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                .Where(e1 => e1.USERFLIPCodigo == empresa)
                                .FirstOrDefault();

                            if (entidade1 != null)
                            {
                                flocks.Fill(flip.FLOCKS);

                                int existeLoteCadastradoFLIP = flip.FLOCKS
                                    .Where(f => f.FLOCK_ID == numLote)
                                    .Count();

                                if (existeLoteCadastradoFLIP > 0)
                                {
                                    #region Inclui no FLIP

                                    FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == numLote)
                                            .FirstOrDefault();

                                    flock_data.FillByFlockData2(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                        dataProducao);

                                    int existeDiarioLoteCadastradoFLIP = flip.FLOCK_DATA
                                        .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                        .Count();

                                    if (existeDiarioLoteCadastradoFLIP == 0)
                                    {
                                        int age = (((DateTime.Today - flock.MOVE_DATE).Days) / 7) + 1;

                                        flock_data.Insert(flock.COMPANY, flock.REGION, flock.LOCATION,
                                            flock.FARM_ID, flock.FLOCK_ID, 1, dataProducao, age, null, null, null,
                                                null, null, null, null, null, null, null, null, qtd, null,
                                                null, null, null, null, null, 0, null, null, null, null,
                                                null, null, null, null, null, null, null, null, null, null, null,
                                                null, null, null, null);

                                        #region Inclui no Apolo

                                        //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                        //{
                                        //    #region Carrega Lote

                                        //    string naturezaOperacao = "1.556.001";
                                        //    decimal? valorUnitario = 0.25m;
                                        //    string unidadeMedida = "UN";
                                        //    short? posicaoUnidadeMedida = 1;
                                        //    string tribCod = "040";
                                        //    string itMovEstqClasFiscCodNbm = "04079000";
                                        //    string clasFiscCod = "0000129";
                                        //    string operacao = "Entrada";

                                        //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                        //        .Where(l => l.USERCodigoFLIP == empresa)
                                        //        .FirstOrDefault();

                                        //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                        //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                        //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                        //    farms.Fill(farmsImport);

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                        //        .Where(f => f.FARM.StartsWith(empresa))
                                        //        .FirstOrDefault();

                                        //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                        //        .Where(e => e.EmpCod == "1")
                                        //        .FirstOrDefault();

                                        //    string entCod = "";

                                        //    if (farm.TERCEIRO.Equals("SIM"))
                                        //    {
                                        //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                        //            .Where(e => e.USERFLIPCodigo == empresa)
                                        //            .FirstOrDefault();

                                        //        entCod = entidade1.EntCod;
                                        //    }
                                        //    else
                                        //    {
                                        //        entCod = empresaFilial.EntCod;
                                        //    }

                                        //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                        //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();

                                        //    #endregion

                                        //    if (loteItemMovEstq == null)
                                        //    {
                                        //        #region Carrega Produto

                                        //        string replace = "" + (char)13 + (char)10;

                                        //        PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                        //        ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        //            && im.ProdCodEstr == produto.ProdCodEstr
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();
                                        //        #endregion

                                        //        if (itemMovEstq == null)
                                        //        {
                                        //            #region Carrega Movimentação. Se não existe, insere.
                                        //            MOV_ESTQ movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO)
                                        //            .FirstOrDefault();

                                        //            if (movEstq == null)
                                        //            {
                                        //                movEstq = service.InsereMovEstq(empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao, "RIOSOFT");

                                        //                apoloService.MOV_ESTQ.AddObject(movEstq);

                                        //                apoloService.SaveChanges();
                                        //            }
                                        //            #endregion

                                        //            #region Se Item não existe, insere item, local e lote
                                        //            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao,
                                        //                flock.VARIETY, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                                        //                itMovEstqClasFiscCodNbm,
                                        //                clasFiscCod);

                                        //            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        //                qtd, localArmazenagem);

                                        //            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                        //            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "INS");

                                        //            apoloService.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                        //            #endregion
                                        //        }
                                        //        else
                                        //        {
                                        //            #region Se existe Item, insere lote e atuliza item e local

                                        //            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                        //            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        //                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        //                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        //                .FirstOrDefault();

                                        //            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                        //            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "UPD");

                                        //            apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                        //            #endregion
                                        //        }
                                        //    }
                                        //}

                                        #endregion

                                        retorno = "ImportadoApoloFLIP";
                                    }
                                    else
                                    {
                                        int existeItensImportadoApoloFLIP = hlbapp.LayoutDiarioExpedicaos
                                            .Where(l => l.LoteCompleto == numLote && l.DataProducao == dataProducao
                                                && l.Importado == "ImportadoApoloFLIP")
                                            .Count();

                                        if (existeItensImportadoApoloFLIP > 0)
                                        {
                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                                .FirstOrDefault();

                                            if (!dataRow.IsNUM_1Null())
                                                dataRow.NUM_1 = dataRow.NUM_1 + qtd;
                                            else
                                                dataRow.NUM_1 = qtd;

                                            dataRow.NUM_8 = 0;

                                            flock_data.Update(dataRow);
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lote.CtrlLoteLocArmazQtdSaldo < (qtd + qtdeExiste))
                        {
                            retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + (char)13 + (char)10
                                + "Saldo Disponível: " + Convert.ToInt32(lote.CtrlLoteLocArmazQtdSaldo - qtdeExiste).ToString()
                                + " ovos";
                            //+ " ovos / " 
                            //+ Decimal.Round(Convert.ToDecimal((lote.CtrlLoteLocArmazQtdSaldo - qtdeExiste) / 150),1).ToString()
                            //+ " bandejas.";

                            var listaDeosLote = hlbapp.LayoutDiarioExpedicaos
                                .Where(l => l.LoteCompleto == lote.CtrlLoteNum && l.DataProducao == lote.CtrlLoteDataValid
                                    && ((l.Granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis") && l.TipoDEO.Equals("Ovos Incubáveis"))
                                                ||
                                        (l.Granja.Equals("PL") && !tipoDEO.Equals("Ovos Incubáveis") && !l.TipoDEO.Equals("Ovos Incubáveis")))
                                    && l.Granja == empresaLocal && l.TipoDEO != "Inventário de Ovos")
                                .OrderBy(o => o.DataHoraCarreg)
                                .ToList();

                            if (listaDeosLote.Count > 0)
                            {
                                int qtdProduzida = Convert.ToInt32(apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                    .Where(c => c.CtrlLoteNum == lote.CtrlLoteNum
                                        && c.CtrlLoteDataValid == lote.CtrlLoteDataValid
                                        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod
                                            && c.MovEstqChv == m.MovEstqChv
                                            && apoloService.TIPO_LANC.Any(t => m.TipoLancCod == t.TipoLancCod
                                                && t.TipoLancNome.Contains("ENTRADA DE OVOS"))))
                                    .FirstOrDefault().CtrlLoteItMovEstqQtd);

                                retorno = retorno + (char)13 + (char)10 + "**** INFORMAÇÕES DO LOTE / DATA DE PRODUÇÃO ****";
                                retorno = retorno + (char)13 + (char)10 + "Quantidade produzida: " + qtdProduzida.ToString();
                                retorno = retorno + (char)13 + (char)10 + "Lista de DEOs em que já existe o Lote:"
                                    + (char)13 + (char)10;
                                foreach (var item in listaDeosLote)
                                {
                                    retorno = retorno + "DEO de " + item.TipoDEO + ": " + item.DataHoraCarreg.ToShortDateString() + " "
                                        + item.DataHoraCarreg.ToShortTimeString() + " - Qtde: " + item.QtdeOvos.ToString()
                                        + " ovos." + (char)10;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (dataProducao == DateTime.Today ||
                        (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                    {
                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa = apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                        if (verificaEmpresa != null)
                        {
                            if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                            {
                                flocks.Fill(flip.FLOCKS);

                                int existeLoteCadastradoFLIP = flip.FLOCKS
                                    .Where(f => f.FLOCK_ID == numLote)
                                    .Count();

                                if (existeLoteCadastradoFLIP > 0)
                                {
                                    #region Inclui no FLIP

                                    FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == numLote)
                                            .FirstOrDefault();

                                    flock_data.FillByFlockData2(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                        dataProducao);

                                    int existeDiarioLoteCadastradoFLIP = flip.FLOCK_DATA
                                        .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                        .Count();

                                    if (existeDiarioLoteCadastradoFLIP == 0)
                                    {
                                        int age = (((DateTime.Today - flock.MOVE_DATE).Days) / 7) + 1;

                                        flock_data.Insert(flock.COMPANY, flock.REGION, flock.LOCATION,
                                            flock.FARM_ID, flock.FLOCK_ID, 1, dataProducao, age, null, null, null,
                                                null, null, null, null, null, null, null, null, qtd, null,
                                                null, null, null, null, null, 0, null, null, null, null,
                                                null, null, null, null, null, null, null, null, null, null, null,
                                                null, null, null, null);

                                        #region Inclui no Apolo

                                        //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                        //{
                                        //    #region Carrega Lote

                                        //    string naturezaOperacao = "1.556.001";
                                        //    decimal? valorUnitario = 0.25m;
                                        //    string unidadeMedida = "UN";
                                        //    short? posicaoUnidadeMedida = 1;
                                        //    string tribCod = "040";
                                        //    string itMovEstqClasFiscCodNbm = "04079000";
                                        //    string clasFiscCod = "0000129";
                                        //    string operacao = "Entrada";

                                        //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                        //        .Where(l => l.USERCodigoFLIP == empresa)
                                        //        .FirstOrDefault();

                                        //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                        //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                        //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                        //    farms.Fill(farmsImport);

                                        //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                        //        .Where(f => f.FARM.StartsWith(empresa))
                                        //        .FirstOrDefault();

                                        //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                        //        .Where(e => e.EmpCod == "1")
                                        //        .FirstOrDefault();

                                        //    string entCod = "";

                                        //    if (farm.TERCEIRO.Equals("SIM"))
                                        //    {
                                        //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                        //            .Where(e => e.USERFLIPCodigo == empresa)
                                        //            .FirstOrDefault();

                                        //        entCod = entidade1.EntCod;
                                        //    }
                                        //    else
                                        //    {
                                        //        entCod = empresaFilial.EntCod;
                                        //    }

                                        //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                        //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();

                                        //    #endregion

                                        //    if (loteItemMovEstq == null)
                                        //    {
                                        //        #region Carrega Produto

                                        //        string replace = "" + (char)13 + (char)10;

                                        //        PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                        //        ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                        //            && im.ProdCodEstr == produto.ProdCodEstr
                                        //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                        //        .FirstOrDefault();
                                        //        #endregion

                                        //        if (itemMovEstq == null)
                                        //        {
                                        //            #region Carrega Movimentação. Se não existe, insere.
                                        //            MOV_ESTQ movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                        //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO)
                                        //            .FirstOrDefault();

                                        //            if (movEstq == null)
                                        //            {
                                        //                movEstq = service.InsereMovEstq(empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao, "RIOSOFT");

                                        //                apoloService.MOV_ESTQ.AddObject(movEstq);

                                        //                apoloService.SaveChanges();
                                        //            }
                                        //            #endregion

                                        //            #region Se Item não existe, insere item, local e lote
                                        //            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao,
                                        //                flock.VARIETY, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                                        //                itMovEstqClasFiscCodNbm,
                                        //                clasFiscCod);

                                        //            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                        //                qtd, localArmazenagem);

                                        //            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                        //            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "INS");

                                        //            apoloService.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                        //            #endregion
                                        //        }
                                        //        else
                                        //        {
                                        //            #region Se existe Item, insere lote e atuliza item e local

                                        //            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                        //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                        //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                        //            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                        //            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                        //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                        //                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                        //                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                        //                .FirstOrDefault();

                                        //            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                        //            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                        //            apoloService.SaveChanges();

                                        //            //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                        //            //    dataAtual, "UPD");

                                        //            apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                        //            #endregion
                                        //        }
                                        //    }
                                        //}

                                        #endregion

                                        retorno = "ImportadoApoloFLIP";
                                    }
                                    else
                                    {
                                        int existeItensImportadoApoloFLIP = hlbapp.LayoutDiarioExpedicaos
                                            .Where(l => l.LoteCompleto == numLote && l.DataProducao == dataProducao
                                                && l.Importado == "ImportadoApoloFLIP")
                                            .Count();

                                        if (existeItensImportadoApoloFLIP > 0)
                                        {
                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                                .FirstOrDefault();

                                            if (!dataRow.IsNUM_1Null())
                                                dataRow.NUM_1 = dataRow.NUM_1 + qtd;
                                            else
                                                dataRow.NUM_1 = qtd;

                                            flock_data.Update(dataRow);

                                            #region Atualiza Apolo

                                            //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                            //{
                                            //    #region Se existe o lote atualiza a quantidade

                                            //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                            //    .Where(l => l.USERCodigoFLIP == empresa)
                                            //    .FirstOrDefault();

                                            //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                            //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                            //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                            //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                            //    farms.Fill(farmsImport);

                                            //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                            //        .Where(f => f.FARM.StartsWith(empresa))
                                            //        .FirstOrDefault();

                                            //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                            //        .Where(e => e.EmpCod == "1")
                                            //        .FirstOrDefault();

                                            //    string entCod = "";

                                            //    if (farm.TERCEIRO.Equals("SIM"))
                                            //    {
                                            //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                            //            .Where(e => e.USERFLIPCodigo == empresa)
                                            //            .FirstOrDefault();

                                            //        entCod = entidade1.EntCod;
                                            //    }
                                            //    else
                                            //    {
                                            //        entCod = empresaFilial.EntCod;
                                            //    }

                                            //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                            //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                            //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                            //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                            //        .FirstOrDefault();

                                            //    decimal? qtdAntiga = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            //    loteItemMovEstq.CtrlLoteItMovEstqQtd = loteItemMovEstq.CtrlLoteItMovEstqQtd + qtd;
                                            //    loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

                                            //    PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                            //    ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                            //        && im.ProdCodEstr == produto.ProdCodEstr
                                            //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                            //            && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                            //    .FirstOrDefault();

                                            //    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                            //    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                            //    LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                            //        && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                            //        && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                            //        .FirstOrDefault();

                                            //    localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                            //    localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                            //    apoloService.SaveChanges();

                                            //    apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                            //    #endregion
                                            //}

                                            #endregion
                                        }
                                        else
                                        {
                                            FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                                .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                                .FirstOrDefault();

                                            dataRow.NUM_1 = qtd;
                                            dataRow.NUM_8 = 0;

                                            flock_data.Update(dataRow);
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                }
                            }
                        }
                        ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                .Where(e1 => e1.USERFLIPCodigo == empresa)
                                .FirstOrDefault();

                        if (entidade1 != null)
                        {
                            flocks.Fill(flip.FLOCKS);

                            int existeLoteCadastradoFLIP = flip.FLOCKS
                                .Where(f => f.FLOCK_ID == numLote)
                                .Count();

                            if (existeLoteCadastradoFLIP > 0)
                            {
                                #region Inclui no FLIP

                                FLIPDataSet.FLOCKSRow flock = flip.FLOCKS
                                        .Where(f => f.FLOCK_ID == numLote)
                                        .FirstOrDefault();

                                flock_data.FillByFlockData2(flip.FLOCK_DATA, "HYBR", "BR", flock.LOCATION, flock.FARM_ID, flock.FLOCK_ID,
                                    dataProducao);

                                int existeDiarioLoteCadastradoFLIP = flip.FLOCK_DATA
                                    .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                    .Count();

                                if (existeDiarioLoteCadastradoFLIP == 0)
                                {
                                    int age = (((DateTime.Today - flock.MOVE_DATE).Days) / 7) + 1;

                                    flock_data.Insert(flock.COMPANY, flock.REGION, flock.LOCATION,
                                        flock.FARM_ID, flock.FLOCK_ID, 1, dataProducao, age, null, null, null,
                                            null, null, null, null, null, null, null, null, qtd, null,
                                            null, null, null, null, null, 0, null, null, null, null,
                                                null, null, null, null, null, null, null, null, null, null, null,
                                                null, null, null, null);

                                    #region Inclui no Apolo

                                    //if (!ExisteFechamentoEstoque(dataProducao, empresa))
                                    //{
                                    //    #region Carrega Lote

                                    //    string naturezaOperacao = "1.556.001";
                                    //    decimal? valorUnitario = 0.25m;
                                    //    string unidadeMedida = "UN";
                                    //    short? posicaoUnidadeMedida = 1;
                                    //    string tribCod = "040";
                                    //    string itMovEstqClasFiscCodNbm = "04079000";
                                    //    string clasFiscCod = "0000129";
                                    //    string operacao = "Entrada";

                                    //    LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                                    //        .Where(l => l.USERCodigoFLIP == empresa)
                                    //        .FirstOrDefault();

                                    //    string localArmazenagem = localArmazCadastro.LocArmazCodEstr;

                                    //    ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

                                    //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable farmsImport =
                                    //        new ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTDataTable();

                                    //    farms.Fill(farmsImport);

                                    //    ImportaIncubacao.Data.FLIPDataSet.FARMS_IMPORTRow farm = farmsImport
                                    //        .Where(f => f.FARM.StartsWith(empresa))
                                    //        .FirstOrDefault();

                                    //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresaFilial = apoloService.EMPRESA_FILIAL
                                    //        .Where(e => e.EmpCod == "1")
                                    //        .FirstOrDefault();

                                    //    string entCod = "";

                                    //    if (farm.TERCEIRO.Equals("SIM"))
                                    //    {
                                    //        ENTIDADE1 entidade1 = apoloService.ENTIDADE1
                                    //            .Where(e => e.USERFLIPCodigo == empresa)
                                    //            .FirstOrDefault();

                                    //        entCod = entidade1.EntCod;
                                    //    }
                                    //    else
                                    //    {
                                    //        entCod = empresaFilial.EntCod;
                                    //    }

                                    //    CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                    //        .Where(l => l.CtrlLoteNum == numLote && l.CtrlLoteDataValid == dataProducao && l.EmpCod == empresaFilial.EmpCod
                                    //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == l.EmpCod && m.MovEstqChv == l.MovEstqChv
                                    //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                    //        .FirstOrDefault();

                                    //    #endregion

                                    //    if (loteItemMovEstq == null)
                                    //    {
                                    //        #region Carrega Produto

                                    //        string replace = "" + (char)13 + (char)10;

                                    //        PRODUTO produto = apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == flock.VARIETY).FirstOrDefault();

                                    //        ITEM_MOV_ESTQ itemMovEstq = apoloService.ITEM_MOV_ESTQ.Where(im => im.EmpCod == empresaFilial.EmpCod
                                    //            && im.ProdCodEstr == produto.ProdCodEstr
                                    //            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == im.EmpCod && m.MovEstqChv == im.MovEstqChv
                                    //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO))
                                    //        .FirstOrDefault();
                                    //        #endregion

                                    //        if (itemMovEstq == null)
                                    //        {
                                    //            #region Carrega Movimentação. Se não existe, insere.
                                    //            MOV_ESTQ movEstq = apoloService.MOV_ESTQ.Where(m => m.EmpCod == empresaFilial.EmpCod
                                    //                && m.MovEstqDataMovimento == dataProducao && m.TipoLancCod == farm.TIPOLANCENTRADAAPOLO)
                                    //            .FirstOrDefault();

                                    //            if (movEstq == null)
                                    //            {
                                    //                movEstq = service.InsereMovEstq(empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao, "RIOSOFT");

                                    //                apoloService.MOV_ESTQ.AddObject(movEstq);

                                    //                apoloService.SaveChanges();
                                    //            }
                                    //            #endregion

                                    //            #region Se Item não existe, insere item, local e lote
                                    //            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, entCod, dataProducao,
                                    //                flock.VARIETY, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                                    //                itMovEstqClasFiscCodNbm,
                                    //                clasFiscCod);

                                    //            apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                    //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = service.InsereLocalArmazenagem(movEstq.MovEstqChv, empresa, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr,
                                    //                qtd, localArmazenagem);

                                    //            apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(localArmaz);

                                    //            loteItemMovEstq = service.InsereLote(movEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                    //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                    //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                    //            apoloService.SaveChanges();

                                    //            //bdApolo.atualiza_saldoestqdata(empresa, movEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                    //            //    dataAtual, "INS");

                                    //            apoloService.calcula_mov_estq(empresa, movEstq.MovEstqChv);
                                    //            #endregion
                                    //        }
                                    //        else
                                    //        {
                                    //            #region Se existe Item, insere lote e atuliza item e local

                                    //            loteItemMovEstq = service.InsereLote(itemMovEstq.MovEstqChv, empresa, farm.TIPOLANCENTRADAAPOLO, itemMovEstq.ItMovEstqSeq, itemMovEstq.ProdCodEstr, numLote,
                                    //                dataProducao, qtd, operacao, unidadeMedida, posicaoUnidadeMedida, localArmazenagem);

                                    //            apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(loteItemMovEstq);

                                    //            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                    //            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                    //            LOC_ARMAZ_ITEM_MOV_ESTQ localArmaz = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(l => l.EmpCod == itemMovEstq.EmpCod
                                    //                && l.MovEstqChv == itemMovEstq.MovEstqChv && l.ProdCodEstr == itemMovEstq.ProdCodEstr
                                    //                && l.ItMovEstqSeq == itemMovEstq.ItMovEstqSeq && l.LocArmazCodEstr == localArmazenagem)
                                    //                .FirstOrDefault();

                                    //            localArmaz.LocArmazItMovEstqQtd = localArmaz.LocArmazItMovEstqQtd + qtd;
                                    //            localArmaz.LocArmazItMovEstqQtdCalc = localArmaz.LocArmazItMovEstqQtd;

                                    //            apoloService.SaveChanges();

                                    //            //bdApolo.atualiza_saldoestqdata(empresa, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr, itemMovEstq.ItMovEstqSeq,
                                    //            //    dataAtual, "UPD");

                                    //            apoloService.calcula_mov_estq(empresa, itemMovEstq.MovEstqChv);

                                    //            #endregion
                                    //        }
                                    //    }
                                    //}

                                    #endregion

                                    retorno = "ImportadoApoloFLIP";
                                }
                                else
                                {
                                    int existeItensImportadoApoloFLIP = hlbapp.LayoutDiarioExpedicaos
                                        .Where(l => l.LoteCompleto == numLote && l.DataProducao == dataProducao
                                            && l.Importado == "ImportadoApoloFLIP")
                                        .Count();

                                    if (existeItensImportadoApoloFLIP > 0)
                                    {
                                        FLIPDataSet.FLOCK_DATARow dataRow = flip.FLOCK_DATA
                                            .Where(d => d.FLOCK_ID == numLote && d.TRX_DATE == dataProducao)
                                            .FirstOrDefault();

                                        if (!dataRow.IsNUM_1Null())
                                            dataRow.NUM_1 = dataRow.NUM_1 + qtd;
                                        else
                                            dataRow.NUM_1 = qtd;

                                        dataRow.NUM_8 = 0;

                                        flock_data.Update(dataRow);
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                            }
                        }
                    }
                    else
                    {
                        retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo Disponível: 0)";
                    }
                }

                #endregion

                #region (DESATIVADA) Método antigo de verificação de saldo com rateio de dias anteriores

                //LOC_ARMAZ localArmazenagem = apoloService.LOC_ARMAZ
                //        .Where(l => l.USERCodigoFLIP == empresa && l.USERTipoProduto == "Ovos Incubáveis")
                //        .FirstOrDefault();

                //existe = 0;
                //existe = apoloService.CTRL_LOTE_LOC_ARMAZ
                //            .Where(c => c.CtrlLoteNum == numLote && c.CtrlLoteDataValid == dataProducao
                //                && c.EmpCod == empresaObject.EmpCod
                //                && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                //                && c.CtrlLoteLocArmazQtdSaldo > 0
                //                && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                //                    && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                //    //&& l.USERGranjaNucleoFLIP.Contains(empresa)))
                //            .OrderBy(o => o.CtrlLoteDataValid)
                //            .Count();

                //if (existe > 0)
                //{
                //    var listaLotes = apoloService.CTRL_LOTE_LOC_ARMAZ
                //        .Where(c => c.CtrlLoteNum == numLote && c.CtrlLoteDataValid <= dataProducao
                //            && c.EmpCod == empresaObject.EmpCod
                //            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                //            && c.CtrlLoteLocArmazQtdSaldo > 0
                //            && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                //                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                //        //&& l.USERGranjaNucleoFLIP.Contains(empresa)))
                //        .OrderBy(o => o.CtrlLoteDataValid)
                //        .ToList();

                //    //int saldo = qtd;
                //    int saldo = qtd + qtdeExiste;
                //    int disponivel = 0;

                //    foreach (var item in listaLotes)
                //    {
                //        if (saldo > item.CtrlLoteLocArmazQtdSaldo)
                //        {
                //            saldo = saldo - Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo);
                //            disponivel = disponivel + Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo);
                //        }
                //        else if (saldo > 0)
                //        {
                //            //disponivel = disponivel + saldo;
                //            saldo = 0;
                //            break;
                //        }
                //    }

                //    if (saldo > 0)
                //    {
                //        retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo: " + disponivel.ToString() + " ovos.)";
                //    }
                //}
                //else
                //{
                //    retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo: 0 ovos.)";
                //}

                #endregion
            }

            return retorno;
        }

        public static bool ExisteFechamentoEstoque(DateTime dataMov, string granja)
        {
            #region Fechamento Estoque Apolo - DESATIVADO
            //MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
            //                        .Where(e => e.USERFLIPCod == granja)
            //                        .FirstOrDefault();

            //if (empresa != null)
            //{
            //    int existe = 0;
            //    existe = bdApolo.Fech_Estq.Where(f => f.FechEstqData >= dataMov && f.EmpCod == empresa.EmpCod)
            //        .Count();

            //    if (existe > 0)
            //        return true;
            //    else
            //        return false;
            //}
            //else
            //    return false;
            #endregion

            #region Fechamento Estoque - Tabela FLIP

            FLIPDataSetMobile.DATA_FECH_LANCDataTable DfDT = new FLIPDataSetMobile.DATA_FECH_LANCDataTable();
            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter DfTA = 
                new MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter();
            DfTA.Fill(DfDT);

            string location = "PP";
            if (granja != "")
            {
                if (granja.Equals("SB") || granja.Equals("PH")) location = "GP";

                string filtroLocal = granja.Substring(0, 2);
                if (!filtroLocal.Equals("NM") && !filtroLocal.Equals("CH") && !filtroLocal.Equals("PH")
                    && !filtroLocal.Equals("TB"))
                {
                    if (location == "PP")
                        filtroLocal = "Granjas Matrizes";
                    else
                        filtroLocal = "Granjas Avos";
                }

                if (DfDT.Count > 0)
                {
                    FLIPDataSetMobile.DATA_FECH_LANCRow DfRow = DfDT.Where(w => w.DATA_FECH_LANC >= dataMov
                        && w.LOCATION == filtroLocal)
                        .FirstOrDefault();

                    if (DfRow != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;

            #endregion
        }

        public void DeletaMovEstq(MOV_ESTQ movestq)
        {
            var listaLotes = apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                        .ToList();

            foreach (var lote in listaLotes)
            {
                apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(lote);
            }

            var listaLocal = apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var local in listaLocal)
            {
                apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.DeleteObject(local);
            }

            var listaItens = apoloService.ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var itens in listaItens)
            {
                apoloService.ITEM_MOV_ESTQ.DeleteObject(itens);
            }

            apoloService.MOV_ESTQ.DeleteObject(movestq);
        }

        public static bool IsIncubatorio(string granja)
        {
            //var lista = bdApolo.EMPRESA_FILIAL
            //            .Where(e => e.USERFLIPCod != null
            //                //&& (e.USERTipoUnidadeFLIP == "Incubatório"))
            //                && (e.USERTipoUnidadeFLIP == "Granja"))
            //            .SelectMany(
            //                x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
            //                (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
            //            .Where(w => w.EMP_FILIAL_CERTIFICACAO.EmpFilCertificNum == granja)
            //            .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
            //            .ToList();

            bdApoloEntities bd = new bdApoloEntities();
            Apolo10EntitiesService apolo = new Apolo10EntitiesService();
            var listaInterna = bd.EMPRESA_FILIAL.Where(w => w.USERFLIPCod == granja && w.USERTipoUnidadeFLIP == "Granja").Count();
            var listaTerceiro = apolo.ENTIDADE
                    .Where(e => apolo.ENTIDADE1.Any(e1 => e1.EntCod == e.EntCod && e1.USERFLIPCodigo == granja
                        && apolo.ENT_CATEG.Any(c => c.EntCod == e1.EntCod && c.CategCodEstr == "07.01")))
                    .Count();

            if (listaInterna == 0 && listaTerceiro == 0)
                return true;
            else
                return false;
        }

        public string VerificaLinhagemOrigemSelecionada(string granja, DateTime dataFiltro)
        {
            string retorno = "";

            if (Session["tipoDEOselecionado"].ToString() == "Transferência entre Linhagens")
            {
                if (Session["linhagemOrigemSelecionada"] == null)
                {
                    return retorno = "Primeiro selecione a Linhagem de Origem para inserir um item!";
                }
                else if (Session["linhagemOrigemSelecionada"].ToString() == "")
                {
                    return retorno = "Primeiro selecione a Linhagem de Origem para inserir um item!";
                }
            }

            return retorno;
        }

        public string VerificaLinhagemDestinoSelecionada(string granja, DateTime dataFiltro)
        {
            string retorno = "";

            if (Session["tipoDEOselecionado"].ToString() == "Transferência entre Linhagens")
            {
                if (Session["linhagemDestinoSelecionada"] == null)
                {
                    return retorno = "Primeiro selecione a Linhagem de Destino para inserir um item!";
                }
                else if (Session["linhagemDestinoSelecionada"].ToString() == "")
                {
                    return retorno = "Primeiro selecione a Linhagem de Destino para inserir um item!";
                }
            }

            return retorno;
        }

        public string VerificaEstoqueWEB(DateTime dataPrd, string numLote, int qtdOvos, string local,
            string localDEO, int qtdOvosDesconsiderar)
        {
            string retorno = "";

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.LoteCompleto == numLote && w.DataProducao == dataPrd
                    && w.Local == local).FirstOrDefault();

            int existe = hlbapp.LayoutDiarioExpedicaos
                .Where(e => e.LoteCompleto == numLote
                    && e.DataProducao == dataPrd
                    && e.Granja == localDEO
                    && e.Importado != "Conferido"
                    && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                .Count();

            if (existe > 0)
            {
                existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                    .Where(e => e.LoteCompleto == numLote
                        && e.DataProducao == dataPrd
                        && e.Granja == localDEO
                        && e.Importado != "Conferido"
                        && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                    .Sum(s => s.QtdeOvos + (s.QtdDiferenca == null ? 0 : s.QtdDiferenca)));

                existe = existe - qtdOvosDesconsiderar;
            }
            else
                existe = 0;

            if (saldo != null)
            {
                if ((saldo.Qtde-existe) < qtdOvos)
                {
                    retorno = (saldo.Qtde).ToString();
                }
            }
            else
                retorno = "0";

            return retorno;
        }

        public string VerificaEstoqueJS(string id, string id2, string id3, int qtdOvosDesconsiderar)
        {
            string retorno = "";
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (!Session["tipoDEOselecionado"].ToString().Equals("Inventário de Ovos"))
            {
                string mensagemExiste = "";
                int qtdeExiste = -1;

                int qtd = Convert.ToInt32(id3);

                DateTime dataProducao = Convert.ToDateTime(id);
                string empresa = Session["granjaSelecionada"].ToString();
                string tipoDEO = Session["tipoDEOselecionado"].ToString();
                string incubatorio = Session["incubatorioDestinoSelecionado"].ToString();

                DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);

                #region Verifica Estoque no WEB

                string empresaEstoque = "";
                if (empresa == "PL" && tipoDEO == "Ovos Incubáveis" && incubatorio == "NM")
                    empresaEstoque = incubatorio;
                else
                    if (empresa != "PL")
                        empresaEstoque = empresa;
                    else
                        if (tipoDEO == "Transf. Ovos Incubáveis")
                            empresaEstoque = "NM";
                        else
                            empresaEstoque = incubatorio;

                string verificaSaldo = VerificaEstoqueWEB(dataProducao, id2, qtd, empresaEstoque, empresa,
                    qtdOvosDesconsiderar);

                int existe = hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == id2
                            && e.DataProducao == dataProducao
                            && e.Granja == empresa
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                        .Count();

                if (existe > 0)
                {
                    existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == id2
                            && e.DataProducao == dataProducao
                            && e.Granja == empresa
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos" && e.TipoDEO != "Solicitação Ajuste de Estoque")
                        .Sum(s => s.QtdeOvos));

                    existe = existe - qtdOvosDesconsiderar;
                }

                int qtdSemConferir = 0;

                if (existe > 0)
                {
                    //mensagemExiste = "Qtde. já adicionada não conferida: "
                    //    + Convert.ToInt32(existe).ToString() + " ovos." + (char)10;
                    mensagemExiste = "- Itens não conferidos:" + (char)10;

                    #region Monta Mensagem Existe

                    var listaExiste = hlbapp.LayoutDiarioExpedicaos
                        .Where(e => e.LoteCompleto == id2
                            && e.DataProducao == dataProducao
                            && e.Granja == empresa
                            && e.Importado != "Conferido"
                            && e.TipoDEO != "Inventário de Ovos")
                        .ToList();

                    foreach (var itemExiste in listaExiste)
                    {
                        if (itemExiste.TipoOvo.Equals(""))
                            mensagemExiste = mensagemExiste + "*" + itemExiste.TipoDEO
                                + " de " + itemExiste.Granja + " em ";
                        else
                            mensagemExiste = mensagemExiste + "*" + itemExiste.TipoDEO
                                + " para " + itemExiste.TipoOvo + " em ";

                        mensagemExiste = mensagemExiste
                            + itemExiste.DataHoraCarreg.ToShortDateString() + " "
                            + itemExiste.DataHoraCarreg.ToShortTimeString() + ": "
                            + "-" + (itemExiste.QtdeOvos
                                + (itemExiste.QtdDiferenca == null ? 0 : itemExiste.QtdDiferenca)).ToString() 
                            + " ovos" + (char)10;

                        qtdSemConferir = qtdSemConferir + Convert.ToInt32(itemExiste.QtdeOvos
                            + (itemExiste.QtdDiferenca == null ? 0 : itemExiste.QtdDiferenca));
                    }

                    mensagemExiste = mensagemExiste +(char)13 + (char)10;

                    #endregion
                }

                if (verificaSaldo != "")
                    qtdeExiste = Convert.ToInt32(verificaSaldo);

                if (verificaSaldo != "")
                {
                    if (((dataProducao == DateTime.Today && tipoDEO.Equals("Ovos Incubáveis")) ||
                        (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                        && qtdeExiste == 0)
                    {
                        #region Verifica Lote Cadastrado FLIP

                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa =
                            apoloService.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                        if (verificaEmpresa != null)
                        {
                            if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                            {
                                flocks.Fill(flip.FLOCKS);

                                int existeLoteCadastradoFLIP = flip.FLOCKS
                                    .Where(f => f.FLOCK_ID == id2)
                                    .Count();

                                if (existeLoteCadastradoFLIP == 0)
                                {
                                    retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                }
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region Monta Mensagem de Retorno

                        decimal saldoCalculado = 0;

                        retorno = retorno + (char)13 + (char)10
                                + "**** INFORMAÇÕES DO LOTE "
                                + id2 + " - " + dataProducao.ToShortDateString()
                                + " ****";
                        

                        #region Verifica Produção
                        
                        string verificaProducao = "";

                        string tipoUnidade = VerificaTipoUnidade(empresaEstoque);

                        //if (tipoDEO.Equals("Ovos Incubáveis"))
                        if (tipoUnidade == "Granja")
                        {
                            FLIPDataSetMobile.FLOCK_DATAMobileDataTable fdDT =
                                new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();

                            FLOCK_DATAMobileTableAdapter fdTA = new FLOCK_DATAMobileTableAdapter();
                            fdTA.FillByFlockAndDate(fdDT, id2, dataProducao);

                            if (fdDT.Count > 0)
                            {
                                int qtdProduzida = Convert.ToInt32(fdDT[0].NUM_1);

                                verificaProducao = verificaProducao
                                    + (char)13 + (char)10
                                    + "* Qtde. Produzida: "
                                    + qtdProduzida.ToString()
                                    + (char)13 + (char)10;

                                saldoCalculado = saldoCalculado + qtdProduzida;
                            }
                            else
                                verificaProducao = verificaProducao
                                    + (char)13 + (char)10
                                    + "Não existe produção para esse lote no FLIP!";
                        }

                        #endregion

                        retorno = "Quantidade solicitada maior que a disponível!" + (char)13 + (char)10
                            + retorno
                            + verificaProducao
                            + mensagemExiste;

                        var listaDeosLote = hlbapp.LayoutDiarioExpedicaos
                            .Where(l => l.LoteCompleto == id2
                                && l.DataProducao == dataProducao
                                && (l.Granja == empresaEstoque || l.Incubatorio == empresaEstoque)
                                && l.Importado == "Conferido"
                                && l.TipoDEO != "Inventário de Ovos")
                            .OrderBy(o => o.DataHoraCarreg)
                            .ToList();

                        if (listaDeosLote.Count > 0)
                        {
                            retorno = retorno + (char)13 + (char)10
                                + "- Lista de DEOs conferidos em que já existe o Lote:"
                                + (char)13 + (char)10;

                            #region Verifica os DEOs

                            foreach (var item in listaDeosLote)
                            {
                                string origem = item.Granja;
                                if (item.Granja.Equals("PL") && !item.TipoDEO.Equals("Ovos Incubáveis"))
                                    origem = "NM";

                                string sinal = "";
                                int dif = 0;
                                if (item.QtdDiferenca != null)
                                    dif = Convert.ToInt32(item.QtdDiferenca);

                                if (origem.Equals(empresaEstoque))
                                {
                                    saldoCalculado = saldoCalculado - (item.QtdeOvos + dif);
                                    sinal = "-";
                                }
                                else if (item.Incubatorio.Equals(empresaEstoque))
                                {
                                    saldoCalculado = saldoCalculado + (item.QtdeOvos + dif);
                                }

                                //if (item.TipoDEO.Equals("Ovos Incubáveis")
                                //    || (item.TipoDEO.Equals("Transf. Ovos Incubáveis")
                                //        && item.Incubatorio.Equals(empresaEstoque)
                                //        && item.TipoOvo.Equals("")))
                                //{
                                //    if (!tipoDEO.Equals("Ovos Incubáveis"))
                                //        saldoCalculado = saldoCalculado + item.QtdeOvos;
                                //    else
                                //    {
                                //        saldoCalculado = saldoCalculado - item.QtdeOvos;
                                //        sinal = "-";
                                //    }
                                //}
                                //else
                                //{
                                //    saldoCalculado = saldoCalculado - item.QtdeOvos;
                                //    sinal = "-";
                                //}

                                if (item.TipoOvo.Equals(""))
                                    retorno = retorno + "*" + item.TipoDEO
                                        + " de " + item.Granja + " em ";
                                else
                                    retorno = retorno + "*" + item.TipoDEO
                                        + " para " + item.TipoOvo + " em ";

                                retorno = retorno
                                    + item.DataHoraCarreg.ToShortDateString() + " "
                                    + item.DataHoraCarreg.ToShortTimeString() + ": "
                                    + sinal + item.QtdeOvos.ToString() + " ovos" + (char)10;
                            }

                            #endregion

                            #region Verifica Incubações

                            string nucleo = listaDeosLote.FirstOrDefault().Nucleo;
                            string nucleoLote = nucleo + "-" + id2;

                            var listaIncubacoes = hlbapp.HATCHERY_EGG_DATA
                                .Where(w => w.Flock_id == nucleoLote && w.Lay_date == dataProducao
                                    && w.Hatch_loc == empresaEstoque && w.Hatch_loc != "NM"
                                    && w.Hatch_loc == w.ClassOvo)
                                .ToList();

                            foreach (var item in listaIncubacoes)
                            {
                                saldoCalculado = saldoCalculado - Convert.ToDecimal(item.Eggs_rcvd);

                                retorno = retorno + "*Incubação em " + item.Set_date.ToShortDateString();

                                if (item.Hatch_loc != item.ClassOvo)
                                    retorno = retorno
                                        + " - " + item.ClassOvo + ": ";

                                retorno = retorno
                                    + "-" + item.Eggs_rcvd.ToString()
                                    + " ovos." + (char)10;
                            }

                            #endregion

                            if (Convert.ToDecimal(verificaSaldo) != saldoCalculado)
                            {
                                retorno = retorno + (char)13 + (char)10
                                    + "DIVERGÊNCIA DE CÁLCULO DE MOVIMENTAÇÕES COM O SALDO!!! "
                                    + "(Saldo Estoque: " + verificaSaldo
                                    + " / Saldo Calculado: " + saldoCalculado.ToString() + ") "
                                    + "TIRE UM PRINT DESSE ERRO E ENVIE PARA ti@hyline.com.br "
                                    + "PARA VERIFICAÇÃO!!!";
                            }
                        }

                        retorno = retorno + (char)13 + (char)10 + "Saldo Disponível em " + empresaEstoque + ": "
                            + (Convert.ToInt32(verificaSaldo) - qtdSemConferir).ToString()
                            + " ovos";

                        #endregion
                    }
                }

                #endregion
            }

            return retorno;
        }

        public int VerificaEstoqueOvosComercioWEB(int qtdOvos, string local)
        {
            int retorno = 0;

            bdApoloEntities apolo = new bdApoloEntities();
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Models.bdApolo.EMPRESA_FILIAL empFilial = apolo.EMPRESA_FILIAL
                .Where(w => w.USERFLIPCod == local).FirstOrDefault();

            if (empFilial != null)
            {
                if (empFilial.USERTipoUnidadeFLIP == "Granja") local = local + "C";
            }
            else
            {
                Models.bdApolo.ENTIDADE1 entidade = apolo.ENTIDADE1
                    .Where(w => w.USERFLIPCodigo == local).FirstOrDefault();

                if (entidade != null) local = local + "C";
            }

            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.Local == local
                    && w.LoteCompleto == "VARIOS").FirstOrDefault();

            if (saldo != null)
                retorno = Convert.ToInt32(saldo.Qtde);
            
            return retorno;
        }

        public bool ExisteEntradaAnteriorDataHoraCarreg(DateTime dataHoraCarreg, string tipoDEO,
            string loteCompleto, DateTime dataProducao)
        {
            bool existe = false;

            

            return existe;
        }

        public string VerificaTipoUnidade(string unidade)
        {
            string retorno = "";

            var listaUnidades = bdApolo.EMPRESA_FILIAL
                .Where(e => e.USERFLIPCod != null && e.USERFLIPCod != ""
                    && (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
                .SelectMany(
                    x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
                    (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
                .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
                .ToList();

            foreach (var item in listaUnidades)
            {
                string codFLIP = "";
                if (item.EMP_FILIAL_CERTIFICACAO == null)
                    codFLIP = item.EMPRESA_FILIAL.USERFLIPCod;
                else
                    codFLIP = item.EMP_FILIAL_CERTIFICACAO.EmpFilCertificNum;

                if (codFLIP == unidade)
                    retorno = item.EMPRESA_FILIAL.USERTipoUnidadeFLIP;
            }
            
            var listaEntidadesTerceiros = apoloService.ENTIDADE
                .Where(e => apoloService
                    .ENTIDADE1.Any(e1 => e1.EntCod == e.EntCod && e1.USERFLIPCodigo != null
                    && apoloService.ENT_CATEG.Any(c => c.EntCod == e1.EntCod && c.CategCodEstr == "07.01"
                        && apoloService.CATEG_USUARIO.Any(u => u.CategCodEstr == c.CategCodEstr))))
                .OrderBy(e => e.EntNomeFant)
                .ToList();

            foreach (var item in listaEntidadesTerceiros)
            {
                ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 = 
                    apoloService.ENTIDADE1.Where(e1 => e1.EntCod == item.EntCod).FirstOrDefault();

                if (entidade1.USERFLIPCodigo == unidade)
                {
                    retorno = "Granja";
                }
            }

            return retorno;
        }

        public bool ExisteDEOSolicitacaoAjusteEstoqueAberto(string unidade)
        {
            bool retorno = false;

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var existe = hlbappSession.LayoutDiarioExpedicaos
                .Where(w => w.TipoDEO == "Solicitação Ajuste de Estoque"
                    && (
                        (w.Incubatorio == unidade)
                        ||
                        (hlbappSession.TIPO_CLASSFICACAO_OVO.Any(a => a.Unidade == unidade
                            && a.CodigoTipo == w.Incubatorio && a.AproveitamentoOvo == "Incubável"))
                       )
                    && w.Importado != "Conferido")
                .Count();

            if (existe > 0) retorno = true;

            return retorno;
        }

        public string RetornaEmpresaApolo(string nucleo)
        {
            string empresaApolo = "";

            ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService =
                    new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();

            ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                .Where(w => nucleo.Contains(w.USERFLIPCod)).FirstOrDefault();

            if (empresa != null)
                empresaApolo = empresa.EmpCod;

            return empresaApolo;
        }

        #region Verifica Estoque JS Antigo

        public ActionResult VerificaEstoqueAntigo(string id, string id2, string id3)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    string retorno = "";
                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                    if (!Session["tipoDEOselecionado"].ToString().Equals("Inventário de Ovos"))
                    {
                        string mensagemExiste = "";
                        int qtdeExiste = -1;

                        int qtd = Convert.ToInt32(id3);

                        DateTime dataProducao = Convert.ToDateTime(id);
                        string empresa = Session["granjaSelecionada"].ToString();
                        string tipoDEO = Session["tipoDEOselecionado"].ToString();
                        string incubatorio = Session["incubatorioDestinoSelecionado"].ToString();

                        DateTime dataCarreg = Convert.ToDateTime(Session["dataHoraCarreg"]);

                        #region *** NÃO UTILIZA MAIS APOLO ****

                        /*
                        string empresaEstoque = "";
                        string empresaLocal = "";
                        if (empresa.Equals("PL"))
                        {
                            if (!tipoDEO.Equals("Ovos Incubáveis"))
                                empresaLocal = "NM";
                            else
                                empresaLocal = empresa;
                            if (Session["linhagemSelecionada"].ToString().Contains("DKB"))
                                empresaEstoque = "PL";
                            else
                                empresaEstoque = "CH";
                        }
                        else
                        {
                            empresaLocal = empresa;
                            empresaEstoque = "CH";
                        }

                        var existe = db.DiarioExpedicao
                            .Where(e => e.LoteCompleto == id2
                                && e.DataProducao == dataProducao
                                && e.Granja == empresa
                                //&& (e.Incubatorio == empresaLocal || empresaLocal == "")
                                //&& e.DataHoraCarreg == dataCarreg
                                && e.Importado != "Conferido"
                                && e.TipoDEO != "Inventário de Ovos")
                            //.FirstOrDefault();
                            .Count();

                        if (existe > 0)
                        {
                            existe = 0;
                            existe = Convert.ToInt32(db.DiarioExpedicao
                                .Where(e => e.LoteCompleto == id2
                                    && e.DataProducao == dataProducao
                                    && e.Granja == empresa
                                    //&& e.Incubatorio == empresaLocal
                                    //&& e.DataHoraCarreg == dataCarreg
                                    && e.Importado != "Conferido"
                                    && e.TipoDEO != "Inventário de Ovos")
                                //.FirstOrDefault();
                                .Sum(s => s.QtdeOvos));

                            mensagemExiste = " (Qtde. já adicionada não conferida: " + Convert.ToInt32(existe).ToString() + " ovos.) ";
                            qtdeExiste = Convert.ToInt32(existe);
                        }

                        MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresaObject = bdApolo.EMPRESA_FILIAL
                            .Where(e => e.USERFLIPCod == empresaEstoque)
                            .FirstOrDefault();

                        #region Código antigo de verificação, onde a quantidade do DEO era a mesma a ser importada

                        CTRL_LOTE_LOC_ARMAZ lote;

                        if (empresaObject != null)
                        {
                            LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                                    .Where(l => l.USERCodigoFLIP == empresaLocal 
                                        && l.USERTipoProduto == "Ovos Incubáveis")
                                    .FirstOrDefault();

                            if (locArmaz != null)
                            {
                                lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                                    .Where(c => c.CtrlLoteNum == id2 && c.CtrlLoteDataValid == dataProducao
                                        && c.EmpCod == empresaObject.EmpCod
                                        && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                        && c.CtrlLoteLocArmazQtdSaldo > 0)
                                    .FirstOrDefault();
                            }
                            else
                            {
                                locArmaz = apoloService.LOC_ARMAZ
                                    .Where(l => l.USERCodigoFLIP == empresaLocal)
                                    .FirstOrDefault();

                                lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                                    .Where(c => c.CtrlLoteNum == id2 && c.CtrlLoteDataValid == dataProducao
                                        && c.EmpCod == empresaObject.EmpCod
                                        && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                        && c.CtrlLoteLocArmazQtdSaldo > 0)
                                    .FirstOrDefault();
                            }
                        }
                        else
                        {
                            ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 =
                                apoloService.ENTIDADE1.Where(e1 => e1.USERFLIPCodigo == empresaLocal).FirstOrDefault();

                            LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                                    .Where(l => l.USERCodigoFLIP == empresa)
                                    .FirstOrDefault();

                            lote = apoloService.CTRL_LOTE_LOC_ARMAZ
                                .Where(c => c.CtrlLoteNum == id2 && c.CtrlLoteDataValid == dataProducao
                                    && c.EmpCod == entidade1.USERCodIncFLIPEntrada
                                    && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                    && c.CtrlLoteLocArmazQtdSaldo > 0)
                                .FirstOrDefault();
                        }

                        if (lote != null)
                        {
                            if (dataProducao == DateTime.Today ||
                                (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                            {
                                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa = 
                                    apoloService.EMPRESA_FILIAL
                                    .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                                if (verificaEmpresa != null)
                                {
                                    if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                                    {
                                        flocks.Fill(flip.FLOCKS);

                                        int existeLoteCadastradoFLIP = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == id2)
                                            .Count();

                                        if (existeLoteCadastradoFLIP == 0)
                                        {
                                            retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (lote.CtrlLoteLocArmazQtdSaldo < (qtd + qtdeExiste))
                                {
                                    retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + (char)13 + (char)10
                                        + "Saldo Disponível em " + empresaLocal + ": " 
                                        + Convert.ToInt32(lote.CtrlLoteLocArmazQtdSaldo - qtdeExiste).ToString()
                                        + " ovos";
                                    //+ " ovos / " 
                                    //+ Decimal.Round(Convert.ToDecimal((lote.CtrlLoteLocArmazQtdSaldo - qtdeExiste) / 150),1).ToString()
                                    //+ " bandejas.";

                                    var listaDeosLote = hlbapp.LayoutDiarioExpedicaos
                                        .Where(l => l.LoteCompleto == lote.CtrlLoteNum && l.DataProducao == lote.CtrlLoteDataValid
                                            //&& l.Granja != "CH" && l.Granja != "PH")
                                            //&& ((l.Granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis") && l.TipoDEO.Equals("Ovos Incubáveis"))
                                            //    ||
                                            //    (l.Granja.Equals("PL") && !tipoDEO.Equals("Ovos Incubáveis") && !l.TipoDEO.Equals("Ovos Incubáveis"))
                                            //    ||
                                            //    l.Granja == empresa)
                                            //&& l.Incubatorio == empresaLocal 
                                            && l.TipoDEO != "Inventário de Ovos")
                                        .OrderBy(o => o.DataHoraCarreg)
                                        .ToList();

                                    if (listaDeosLote.Count > 0)
                                    {
                                        int qtdProduzida = Convert.ToInt32(apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                            .Where(c => c.CtrlLoteNum == lote.CtrlLoteNum
                                                && c.CtrlLoteDataValid == lote.CtrlLoteDataValid
                                                && apoloService.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod
                                                    && c.MovEstqChv == m.MovEstqChv
                                                    && apoloService.TIPO_LANC.Any(t => m.TipoLancCod == t.TipoLancCod
                                                        && t.TipoLancNome.Contains("ENTRADA DE OVOS"))))
                                            .ToList().Sum(s => s.CtrlLoteItMovEstqQtd));

                                        retorno = retorno + (char)13 + (char)10 + "**** INFORMAÇÕES DO LOTE / DATA DE PRODUÇÃO ****";
                                        retorno = retorno + (char)13 + (char)10 + "Quantidade produzida: " + qtdProduzida.ToString();
                                        retorno = retorno + (char)13 + (char)10 + "Lista de DEOs em que já existe o Lote:"
                                            + (char)13 + (char)10;
                                        foreach (var item in listaDeosLote)
                                        {
                                            retorno = retorno + item.TipoDEO 
                                                + " de " + item.Granja + " p/ " + item.Incubatorio 
                                                + ((item.Incubatorio != item.TipoOvo) ? (" - " + item.TipoOvo) : "") + ": " 
                                                + item.DataHoraCarreg.ToShortDateString() + " "
                                                + item.DataHoraCarreg.ToShortTimeString() + " - " + item.QtdeOvos.ToString()
                                                + " ovos." + (char)10;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dataProducao == DateTime.Today ||
                                (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                            {
                                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa = apoloService.EMPRESA_FILIAL
                                    .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                                if (verificaEmpresa != null)
                                {
                                    if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                                    {
                                        flocks.Fill(flip.FLOCKS);

                                        int existeLoteCadastradoFLIP = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == id2)
                                            .Count();

                                        if (existeLoteCadastradoFLIP == 0)
                                        {
                                            retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo Disponível: 0)";

                                var listaDeosLote = hlbapp.LayoutDiarioExpedicaos
                                    .Where(l => l.LoteCompleto == id2 && l.DataProducao == dataProducao
                                        //&& l.Granja != "CH" && l.Granja != "PH")
                                        //&& ((l.Granja.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis") && l.TipoDEO.Equals("Ovos Incubáveis"))
                                        //        ||
                                        //    (l.Granja.Equals("PL") && !tipoDEO.Equals("Ovos Incubáveis") && !l.TipoDEO.Equals("Ovos Incubáveis"))
                                        //        ||
                                        //    l.Granja == empresaLocal)//&& l.Incubatorio == empresaLocal 
                                        && l.TipoDEO != "Inventário de Ovos")
                                    .OrderBy(o => o.DataHoraCarreg)
                                    .ToList();

                                if (listaDeosLote.Count > 0)
                                {
                                    int qtdProduzida = Convert.ToInt32(apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                        .Where(c => c.CtrlLoteNum == id2
                                            && c.CtrlLoteDataValid == dataProducao
                                            && apoloService.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod
                                                && c.MovEstqChv == m.MovEstqChv
                                                && apoloService.TIPO_LANC.Any(t => m.TipoLancCod == t.TipoLancCod
                                                    && t.TipoLancNome.Contains("ENTRADA DE OVOS"))))
                                        .ToList().Sum(s => s.CtrlLoteItMovEstqQtd));

                                    retorno = retorno + (char)13 + (char)10 + "**** INFORMAÇÕES DO LOTE / DATA DE PRODUÇÃO ****";
                                    retorno = retorno + (char)13 + (char)10 + "Quantidade produzida: " + qtdProduzida.ToString();
                                    retorno = retorno + (char)13 + (char)10 + "Lista de DEOs em que já existe o Lote:"
                                        + (char)13 + (char)10;
                                    foreach (var item in listaDeosLote)
                                    {
                                        retorno = retorno + "DEO de " + item.TipoDEO
                                                + " - De " + item.Granja + " p/ " + item.Incubatorio
                                                + ((item.Incubatorio != item.TipoOvo) ? (" - " + item.TipoOvo) : "") + ": "
                                                + item.DataHoraCarreg.ToShortDateString() + " "
                                                + item.DataHoraCarreg.ToShortTimeString() + " - " + item.QtdeOvos.ToString()
                                                + " ovos." + (char)10;
                                    }
                                }
                            }
                        }

                        #endregion
                         * 
                         * */

                        #endregion

                        #region Verifica Estoque no WEB

                        string empresaEstoque = "";
                        if (empresa == "PL" && tipoDEO != "Ovos Incubáveis")
                            empresaEstoque = incubatorio;
                        else
                            empresaEstoque = empresa;

                        string verificaSaldo = VerificaEstoqueWEB(dataProducao, id2, qtd, empresaEstoque, 
                            empresa, 0);

                        int existe = hlbapp.LayoutDiarioExpedicaos
                                .Where(e => e.LoteCompleto == id2
                                    && e.DataProducao == dataProducao
                                    && e.Granja == empresa
                                    && e.Importado != "Conferido"
                                    && e.TipoDEO != "Inventário de Ovos")
                                .Count();

                        if (existe > 0)
                        {
                            existe = Convert.ToInt32(hlbapp.LayoutDiarioExpedicaos
                                .Where(e => e.LoteCompleto == id2
                                    && e.DataProducao == dataProducao
                                    && e.Granja == empresa
                                    && e.Importado != "Conferido"
                                    && e.TipoDEO != "Inventário de Ovos")
                                .Sum(s => s.QtdeOvos));
                        }

                        if (existe > 0)
                        {
                            mensagemExiste = " (Qtde. já adicionada não conferida: "
                                + Convert.ToInt32(existe).ToString() + " ovos.) ";
                        }

                        if (verificaSaldo != "")
                            qtdeExiste = Convert.ToInt32(verificaSaldo);

                        //if (qtdeExiste >= 0)
                        if (verificaSaldo != "")
                        {
                            if ((dataProducao == DateTime.Today ||
                                (empresa.Equals("PL") && tipoDEO.Equals("Ovos Incubáveis")))
                                && qtdeExiste == 0)
                            {
                                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL verificaEmpresa =
                                    apoloService.EMPRESA_FILIAL
                                    .Where(e => e.USERFLIPCod == empresa).FirstOrDefault();

                                if (verificaEmpresa != null)
                                {
                                    if (verificaEmpresa.USERTipoUnidadeFLIP.Equals("Granja") || empresa.Equals("PL"))
                                    {
                                        flocks.Fill(flip.FLOCKS);

                                        int existeLoteCadastradoFLIP = flip.FLOCKS
                                            .Where(f => f.FLOCK_ID == id2)
                                            .Count();

                                        if (existeLoteCadastradoFLIP == 0)
                                        {
                                            retorno = "Lote não cadastrado no FLIP. Por favor, primeiro cadastro o Lote no FLIP!";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                decimal saldoCalculado = 0;

                                retorno = "Quantidade solicitada maior que a disponível!"
                                    + mensagemExiste + (char)13 + (char)10
                                    + "Saldo Disponível em " + empresaEstoque + ": "
                                    + verificaSaldo
                                    + " ovos";

                                var listaDeosLote = hlbapp.LayoutDiarioExpedicaos
                                    .Where(l => l.LoteCompleto == id2
                                        && l.DataProducao == dataProducao
                                        && (l.Granja == empresaEstoque || l.Incubatorio == empresaEstoque)
                                        && l.TipoDEO != "Inventário de Ovos")
                                    .OrderBy(o => o.DataHoraCarreg)
                                    .ToList();

                                if (listaDeosLote.Count > 0)
                                {
                                    #region QtdProduzida FLIP (Comentada)

                                    //int qtdProduzida = Convert.ToInt32(apoloService.CTRL_LOTE_ITEM_MOV_ESTQ
                                    //    .Where(c => c.CtrlLoteNum == id2
                                    //        && c.CtrlLoteDataValid == dataProducao
                                    //        && apoloService.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod
                                    //            && c.MovEstqChv == m.MovEstqChv
                                    //            && apoloService.TIPO_LANC.Any(t => m.TipoLancCod == t.TipoLancCod
                                    //                && t.TipoLancNome.Contains("ENTRADA DE OVOS"))))
                                    //    .ToList().Sum(s => s.CtrlLoteItMovEstqQtd));

                                    //FLIPDataSetMobile.FLOCK_DATAMobileDataTable fdDT =
                                    //    new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();

                                    //FLOCK_DATAMobileTableAdapter fdTA = new FLOCK_DATAMobileTableAdapter();
                                    //fdTA.FillByFlockAndDate(fdDT, id2, dataProducao);

                                    //int qtdProduzida = Convert.ToInt32(fdDT[0].NUM_1);

                                    #endregion

                                    retorno = retorno + (char)13 + (char)10
                                        + "**** INFORMAÇÕES DO LOTE "
                                        + id2 + " - " + dataProducao.ToShortDateString()
                                        + " ****";
                                    retorno = retorno + (char)13 + (char)10
                                        //    + "Quantidade produzida: " + qtdProduzida.ToString();
                                        //retorno = retorno + (char)13 + (char)10 
                                        + "Lista de DEOs em que já existe o Lote:"
                                        + (char)13 + (char)10;
                                    foreach (var item in listaDeosLote)
                                    {
                                        string sinal = "";
                                        if (item.TipoDEO.Equals("Ovos Incubáveis")
                                            || (item.TipoDEO.Equals("Transf. Ovos Incubáveis")
                                                && item.Incubatorio.Equals(empresaEstoque)
                                                && item.TipoOvo.Equals("")))
                                        {
                                            saldoCalculado = saldoCalculado + item.QtdeOvos;
                                        }
                                        else
                                        {
                                            saldoCalculado = saldoCalculado - item.QtdeOvos;
                                            sinal = "-";
                                        }

                                        if (item.TipoOvo.Equals(""))
                                            retorno = retorno + item.TipoDEO
                                                + " de " + item.Granja + " em ";
                                        else
                                            retorno = retorno + item.TipoDEO
                                                + " para " + item.TipoOvo + " em ";

                                        retorno = retorno
                                            + item.DataHoraCarreg.ToShortDateString() + " "
                                            + item.DataHoraCarreg.ToShortTimeString() + ": "
                                            + sinal + item.QtdeOvos.ToString() + " ovos." + (char)10;
                                    }

                                    string nucleo = listaDeosLote.FirstOrDefault().Nucleo;
                                    string nucleoLote = nucleo + "-" + id2;

                                    var listaIncubacoes = hlbapp.HATCHERY_EGG_DATA
                                        .Where(w => w.Flock_id == nucleoLote && w.Lay_date == dataProducao
                                            && w.Hatch_loc == empresaEstoque)
                                        .ToList();

                                    foreach (var item in listaIncubacoes)
                                    {
                                        saldoCalculado = saldoCalculado - Convert.ToDecimal(item.Eggs_rcvd);

                                        retorno = retorno + "Incubação em " + item.Set_date.ToShortDateString();

                                        if (item.Hatch_loc != item.ClassOvo)
                                            retorno = retorno
                                                + " - " + item.ClassOvo + ": ";

                                        retorno = retorno
                                            + "-" + item.Eggs_rcvd.ToString()
                                            + " ovos." + (char)10;
                                    }

                                    if (Convert.ToDecimal(verificaSaldo) != saldoCalculado)
                                    {
                                        retorno = retorno + (char)13 + (char)10
                                            + "DIVERGÊNCIA DE CÁLCULO DE MOVIMENTAÇÕES COM O SALDO!!! "
                                            + "(Saldo Estoque: " + verificaSaldo
                                            + " / Saldo Calculado: " + saldoCalculado.ToString() + ") "
                                            + "TIRE UM PRINT DESSE ERRO E ENVIE PARA ti@hyline.com.br "
                                            + "PARA VERIFICAÇÃO!!!";
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Verificação p/ a tabela de Importação (COMENTADO POR CAUSA MUITOS PROBLEMAS)

                        //LOC_ARMAZ localArmazenagem = apoloService.LOC_ARMAZ
                        //        .Where(l => l.USERCodigoFLIP == empresa && l.USERTipoProduto == "Ovos Incubáveis")
                        //        .FirstOrDefault();

                        //existe = 0;
                        //existe = apoloService.CTRL_LOTE_LOC_ARMAZ
                        //            .Where(c => c.CtrlLoteNum == id2 && c.CtrlLoteDataValid == dataProducao
                        //                && c.EmpCod == empresaObject.EmpCod
                        //                && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        //                && c.CtrlLoteLocArmazQtdSaldo > 0
                        //                && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                        //                    && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        //                    //&& l.USERGranjaNucleoFLIP.Contains(empresa)))
                        //            .OrderBy(o => o.CtrlLoteDataValid)
                        //            .Count();

                        //if (existe > 0)
                        //{
                        //    var listaLotes = apoloService.CTRL_LOTE_LOC_ARMAZ
                        //        .Where(c => c.CtrlLoteNum == id2 && c.CtrlLoteDataValid <= dataProducao
                        //            && c.EmpCod == empresaObject.EmpCod
                        //            && c.LocArmazCodEstr == localArmazenagem.LocArmazCodEstr
                        //            && c.CtrlLoteLocArmazQtdSaldo > 0
                        //            && apoloService.CTRL_LOTE.Any(l => l.EmpCod == c.EmpCod && l.ProdCodEstr == c.ProdCodEstr
                        //                && l.CtrlLoteNum == c.CtrlLoteNum && l.CtrlLoteDataValid == c.CtrlLoteDataValid))
                        //        //&& l.USERGranjaNucleoFLIP.Contains(empresa)))
                        //        .OrderBy(o => o.CtrlLoteDataValid)
                        //        .ToList();

                        //    //int saldo = qtd;
                        //    int saldo = qtd + qtdeExiste;
                        //    int disponivel = 0;

                        //    foreach (var item in listaLotes)
                        //    {
                        //        if (saldo > item.CtrlLoteLocArmazQtdSaldo)
                        //        {
                        //            saldo = saldo - Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo);
                        //            disponivel = disponivel + Convert.ToInt32(item.CtrlLoteLocArmazQtdSaldo);
                        //        }
                        //        else if (saldo > 0)
                        //        {
                        //            //disponivel = disponivel + saldo;
                        //            saldo = 0;
                        //            break;
                        //        }
                        //    }

                        //    if (saldo > 0)
                        //    {
                        //        retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo: " + disponivel.ToString() + " ovos.)";
                        //    }
                        //}
                        //else
                        //{
                        //    retorno = "Quantidade solicitada maior que a disponível!" + mensagemExiste + " (Saldo: 0 ovos.)";
                        //}

                        #endregion
                    }

                    return Json(retorno);
                }
                else
                {
                    return RedirectToAction("Login", "AccountMobile");
                }
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        #endregion

        #endregion

        #region Métodos FLIP

        public void ImportaSaidaFLIP(LayoutDiarioExpedicaos deoItem, string location, string usuario, string operacao, 
            string tipoOperacao)
        {
            string trackNo = "EXP" + deoItem.DataProducao.ToString("yyMMdd");

            FLIPDataSet.EGGINV_DATADataTable eggInvDataOpen = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.EGGINV_DATADataTable eggInvDataTipoOperacao = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.STATUS_TABLEDataTable statusEggInv = new FLIPDataSet.STATUS_TABLEDataTable();

            STATUS_TABLETableAdapter statusData = new STATUS_TABLETableAdapter();
            statusData.FillByDescricao(statusEggInv, tipoOperacao);

            string status = statusEggInv[0].STATUS;

            eggInvData.FillByFlockLayDateStatus(eggInvDataOpen, deoItem.LoteCompleto, "O", deoItem.DataProducao, 
                deoItem.Granja);

            if (eggInvDataOpen.Count > 0)
            {
                decimal qtd = 0;

                eggInvData.FillByFlockLayDateStatus(eggInvDataTipoOperacao, deoItem.LoteCompleto, status, 
                    deoItem.DataProducao, deoItem.Granja);

                #region Inserção a Saída

                if (operacao.Equals("INS"))
                {
                    #region Atualiza / Insere a Saída

                    if (eggInvDataTipoOperacao.Count > 0)
                    {
                        qtd = eggInvDataTipoOperacao[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, status, 
                            deoItem.Granja);
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo,
                            deoItem.DataProducao, deoItem.QtdeOvos, status, null, null, null, usuario, null, 
                            null, null, null, deoItem.Granja, DateTime.Now);
                    }

                    #endregion

                    #region Atualiza / Deleta o Open

                    if (eggInvDataOpen[0].EGG_UNITS == deoItem.QtdeOvos)
                    {
                        eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            "O", deoItem.Granja);
                    }
                    else
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS - deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                    }

                    #endregion
                }

                #endregion

                #region Deleção a Saída

                else if (operacao.Equals("DEL"))
                {
                    #region Atualiza / Deleta a Saída

                    if (eggInvDataTipoOperacao.Count > 0)
                    {
                        if (eggInvDataTipoOperacao[0].EGG_UNITS == deoItem.QtdeOvos)
                        {
                            eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo,
                                deoItem.DataProducao, status, deoItem.Granja);
                        }
                        else
                        {
                            qtd = eggInvDataTipoOperacao[0].EGG_UNITS - deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, status, 
                                deoItem.Granja);
                        }
                    }

                    #endregion

                    #region Atualiza / Insere o Open

                    if (eggInvDataOpen.Count > 0)
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, 
                            deoItem.DataProducao, deoItem.QtdeOvos, "O", null, null, null, usuario, null, null,
                            null, null, deoItem.Granja, DateTime.Now);
                    }

                    #endregion
                }

                #endregion
            }
        }

        public void TransferEggsFLIP(LayoutDiarioExpedicaos deoItem, string location, string usuario, string operacao)
        {
            string trackNo = "EXP" + deoItem.DataProducao.ToString("yyMMdd");

            FLIPDataSet.EGGINV_DATADataTable eggInvDataOpen = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.EGGINV_DATADataTable eggInvDataMarket = new FLIPDataSet.EGGINV_DATADataTable();

            eggInvData.FillByFlockLayDateStatus(eggInvDataOpen, deoItem.LoteCompleto, "O", deoItem.DataProducao, deoItem.Granja);

            if (eggInvDataOpen.Count > 0)
            {
                decimal qtd = 0;

                eggInvData.FillByFlockLayDateStatus(eggInvDataMarket, deoItem.LoteCompleto, "O", deoItem.DataProducao, "TB");

                #region Inserção do Market

                if (operacao.Equals("INS"))
                {
                    #region Atualiza / Insere o Market

                    if (eggInvDataMarket.Count > 0)
                    {
                        qtd = eggInvDataMarket[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", "TB");
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            deoItem.QtdeOvos, "O", null, null, null, usuario, null, null, null, null, "TB", DateTime.Now);
                    }

                    #endregion

                    #region Atualiza / Deleta o Open

                    if (eggInvDataOpen[0].EGG_UNITS == deoItem.QtdeOvos)
                    {
                        eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            "O", deoItem.Granja);
                    }
                    else
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS - deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                    }

                    #endregion
                }

                #endregion

                #region Deleção do Market

                else if (operacao.Equals("DEL"))
                {
                    if (eggInvDataMarket.Count > 0)
                    {
                        #region Atualiza / Deleta o Market

                        if (eggInvDataMarket[0].EGG_UNITS == deoItem.QtdeOvos)
                        {
                            eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                                "O", "TB");
                        }
                        else
                        {
                            qtd = eggInvDataMarket[0].EGG_UNITS - deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", "TB");
                        }


                        #endregion

                        #region Atualiza / Insere o Open

                        if (eggInvDataOpen.Count > 0)
                        {
                            qtd = eggInvDataOpen[0].EGG_UNITS + deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                        }
                        else
                        {
                            eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                                deoItem.QtdeOvos, "O", null, null, null, usuario, null, null, null, null, deoItem.Granja, DateTime.Now);
                        }

                        #endregion
                    }
                }

                #endregion
            }
        }

        public void ImportaSaidaFLIPImport(ImportaDiarioExpedicao deoItem, string location, string usuario, 
            string operacao, string tipoOperacao)
        {
            string trackNo = "EXP" + deoItem.DataProducao.ToString("yyMMdd");

            FLIPDataSet.EGGINV_DATADataTable eggInvDataOpen = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.EGGINV_DATADataTable eggInvDataSaida = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.STATUS_TABLEDataTable statusEggInv = new FLIPDataSet.STATUS_TABLEDataTable();

            STATUS_TABLETableAdapter statusData = new STATUS_TABLETableAdapter();
            statusData.FillByDescricao(statusEggInv, tipoOperacao);

            string status = statusEggInv[0].STATUS;

            eggInvData.FillByFlockLayDateStatus(eggInvDataOpen, deoItem.LoteCompleto, "O", deoItem.DataProducao, deoItem.Granja);

            if (eggInvDataOpen.Count > 0)
            {
                decimal qtd = 0;

                eggInvData.FillByFlockLayDateStatus(eggInvDataSaida, deoItem.LoteCompleto, status, 
                    deoItem.DataProducao, deoItem.Granja);

                #region Inserção da Saída

                if (operacao.Equals("INS"))
                {
                    #region Atualiza / Insere a Saída

                    if (eggInvDataSaida.Count > 0)
                    {
                        qtd = eggInvDataSaida[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, status, 
                            deoItem.Granja);
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo,
                            deoItem.DataProducao, deoItem.QtdeOvos, status, null, null, null, usuario, null, 
                            null, null, null, deoItem.Granja, DateTime.Now);
                    }

                    #endregion

                    #region Atualiza / Deleta o Open

                    if (eggInvDataOpen[0].EGG_UNITS == deoItem.QtdeOvos)
                    {
                        eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            "O", deoItem.Granja);
                    }
                    else
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS - deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                    }

                    #endregion
                }

                #endregion

                #region Deleção a Saída

                else if (operacao.Equals("DEL"))
                {
                    #region Atualiza / Deleta a Saída

                    if (eggInvDataSaida.Count > 0)
                    {
                        if (eggInvDataSaida[0].EGG_UNITS == deoItem.QtdeOvos)
                        {
                            eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo,
                                deoItem.DataProducao, status, deoItem.Granja);
                        }
                        else
                        {
                            qtd = eggInvDataSaida[0].EGG_UNITS - deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, status, 
                                deoItem.Granja);
                        }
                    }

                    #endregion

                    #region Atualiza / Insere o Open

                    if (eggInvDataOpen.Count > 0)
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", deoItem.Granja);
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            deoItem.QtdeOvos, "O", null, null, null, usuario, null, null, null, null, deoItem.Granja, DateTime.Now);
                    }

                    #endregion
                }

                #endregion
            }
        }

        public void TransferEggsFLIPImport(ImportaDiarioExpedicao deoItem, string location, string usuario, string operacao,
            string incubatorio)
        {
            string trackNo = "EXP" + deoItem.DataProducao.ToString("yyMMdd");

            FLIPDataSet.EGGINV_DATADataTable eggInvDataOpen = new FLIPDataSet.EGGINV_DATADataTable();
            FLIPDataSet.EGGINV_DATADataTable eggInvDataMarket = new FLIPDataSet.EGGINV_DATADataTable();

            eggInvData.FillByFlockLayDateStatus(eggInvDataOpen, deoItem.LoteCompleto, "O", deoItem.DataProducao, "CH");

            if (eggInvDataOpen.Count > 0)
            {
                decimal qtd = 0;

                eggInvData.FillByFlockLayDateStatus(eggInvDataMarket, deoItem.LoteCompleto, "O", deoItem.DataProducao, incubatorio);

                #region Inserção do Market

                if (operacao.Equals("INS"))
                {
                    #region Atualiza / Insere o Market

                    if (eggInvDataMarket.Count > 0)
                    {
                        qtd = eggInvDataMarket[0].EGG_UNITS + deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", incubatorio);
                    }
                    else
                    {
                        eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            deoItem.QtdeOvos, "O", null, null, null, usuario, null, null, null, null, incubatorio, DateTime.Now);
                    }

                    #endregion

                    #region Atualiza / Deleta o Open

                    if (eggInvDataOpen[0].EGG_UNITS == deoItem.QtdeOvos)
                    {
                        eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                            "O", "CH");
                    }
                    else
                    {
                        qtd = eggInvDataOpen[0].EGG_UNITS - deoItem.QtdeOvos;

                        eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", "CH");
                    }

                    #endregion
                }

                #endregion

                #region Deleção do Market

                else if (operacao.Equals("DEL"))
                {
                    if (eggInvDataMarket.Count > 0)
                    {
                        #region Atualiza / Deleta o Market

                        if (eggInvDataMarket[0].EGG_UNITS == deoItem.QtdeOvos)
                        {
                            eggInvData.Delete("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                                "O", incubatorio);
                        }
                        else
                        {
                            qtd = eggInvDataMarket[0].EGG_UNITS - deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", incubatorio);
                        }


                        #endregion

                        #region Atualiza / Insere o Open

                        if (eggInvDataOpen.Count > 0)
                        {
                            qtd = eggInvDataOpen[0].EGG_UNITS + deoItem.QtdeOvos;

                            eggInvData.UpdateQtdOvos(qtd, deoItem.LoteCompleto, deoItem.DataProducao, "O", "CH");
                        }
                        else
                        {
                            eggInvData.Insert("HYBR", "BR", location, deoItem.Nucleo, deoItem.LoteCompleto, trackNo, deoItem.DataProducao,
                                deoItem.QtdeOvos, "O", null, null, null, usuario, null, null, null, null, "CH", DateTime.Now);
                        }

                        #endregion
                    }
                }

                #endregion
            }
        }

        public string GetResponsableByHatchery(string hatchLoc)
        {
            string responsable = "Miriene Gomes";

            FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter hTA = 
                new MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters.HATCHERY_CODESTableAdapter();

            hTA.Fill(hDT);

            foreach (var item in hDT)
            {
                if (item.HATCH_LOC == hatchLoc)
                    responsable = item.ORDENT_LOC;
            }

            return responsable;
        }

        #endregion

        #region Métodos Ajuste p/ Inventário - APOLO - DESATIVADO

        public ActionResult VerificaAjusteInventario(string granja, DateTime dataFiltro)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string tipoDEO = Session["tipoDEOselecionado"].ToString();
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            bdApolo.CommandTimeout = 1000;
            apoloService.CommandTimeout = 1000;

            Session["dataHoraCarreg"] = dataFiltro;
            //string granja = Session["granjaSelecionada"].ToString();

            DateTime dataVerifica = Convert.ToDateTime(dataFiltro.ToShortDateString());

            if (ExisteFechamentoEstoque(dataVerifica, granja))
            {
                ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = apoloService.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod == granja)
                    .FirstOrDefault();

                //string responsavel = "Miriene Gomes";
                //if (granja.Substring(0, 2) == "NM") responsavel = "Ana Carolina Neves";
                //else if (granja.Substring(0, 2) == "CH" || granja.Substring(0, 2) == "TB")
                //    responsavel = "Sérica Doimo";
                //else if (granja.Substring(0, 2) == "PH") responsavel = "Jonatan Segura";
                //else if (granja.Substring(0, 2) == "SB") responsavel = "Alex Prates";

                //ViewBag.Erro = "Existe Fechamento de Estoque na data " + dataFiltro.ToShortDateString()
                //                + " na empresa " + empresa.EmpNome
                //                + "! Não pode ser excluído este Diário de Expedição!"
                //                + " Verificar com o " + responsavel + " a possibilidade da liberação!";

                string responsavel = GetResponsableByHatchery(granja.Substring(0, 2));
                ViewBag.Erro = am.GetTextOnLanguage("Estoque já fechado! Verifique com", Session["language"].ToString()) + " "
                    + responsavel + " " + am.GetTextOnLanguage("sobre a possibilidade da abertura!", Session["language"].ToString())
                    + am.GetTextOnLanguage("Caso não seja aberto, a incubação não pode ser realizada!", Session["language"].ToString());

                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            var listaDivergenciaAntiga  = bdApolo.Divergencia.ToList();

            foreach (var item in listaDivergenciaAntiga)
            {
                bdApolo.DeleteObject(item);
            }

            bdApolo.SaveChanges();

            var listaItens = hlbapp.ImportaDiarioExpedicao
                        .Where(w => w.Granja == granja && w.DataHoraCarreg == dataFiltro)
                        .GroupBy(g => g.Linhagem)
                        .OrderBy(o => o.Key)
                        .ToList();

            string empresaApolo = "1";
            string mensagemRetornoDivergencia = "";

            if (listaItens.Count > 0)
            {
                foreach (var item in listaItens)
                {
                    MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO produtoApolo =
                        bdApolo.PRODUTO.Where(w => w.ProdNomeAlt1 == item.Key)
                        .FirstOrDefault();

                    bdApolo.UserRelConfTab(empresaApolo, produtoApolo.ProdCodEstr,
                        produtoApolo.ProdCodEstr);
                }

                var listaDivergencia = bdApolo.Divergencia.ToList();

                if (listaDivergencia.Count > 0)
                {
                    mensagemRetornoDivergencia = "Existem Linhagens com Divergência de Estoque neste DEO: <br /><br />";

                    foreach (var itemDivergente in listaDivergencia)
                    {
                        MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO produtoApolo =
                        bdApolo.PRODUTO.Where(w => w.ProdCodEstr == itemDivergente.ProdCodEstr)
                        .FirstOrDefault();

                        mensagemRetornoDivergencia = mensagemRetornoDivergencia + "Linhagem: "
                            + produtoApolo.ProdNomeAlt1 + " <br />";
                    }

                    mensagemRetornoDivergencia = mensagemRetornoDivergencia + " <br />"
                        + "INFORMAR AO DEPARTAMENTO DE T.I. PARA QUE POSSA SER AJUSTADO!!!";

                    ViewBag.Erro = mensagemRetornoDivergencia;

                    return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
                }

                ImportaDiarioExpedicao importaDiarioExpedicao = hlbapp.ImportaDiarioExpedicao
                            .Where(w => w.Granja == granja && w.DataHoraCarreg == dataFiltro
                                && w.TipoDEO == "Inventário de Ovos")
                            .FirstOrDefault();

                if (importaDiarioExpedicao != null)
                    ViewBag.ControleInventario = importaDiarioExpedicao.NumIdentificacao;
                else
                    ViewBag.ControleInventario = "";
            }
            else
            {
                ViewBag.Erro = "DEO " + dataFiltro.ToShortDateString() 
                    + " de Inventário de Ovos não tem dados para Importação! Verifique com o Depto. de T.I.!";

                return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
            }

            return View("VerificaAjusteInventario");
        }

        public ActionResult GeraAjusteInventario(string granja, DateTime dataHoraCarreg)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string localErro = "";
            string tipoDEO = Session["tipoDEOselecionado"].ToString();

            try
            {
                apoloService.CommandTimeout = 1000;
                bdApolo.CommandTimeout = 1000;
                hlbapp.CommandTimeout = 300;

                var existe = hlbapp.ImportaDiarioExpedicao
                    .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                        && w.TipoDEO == "Inventário de Ovos")
                    .Count();

                if (existe > 0)
                {
                    ImportaDiarioExpedicao importaDiarioExpedicao = hlbapp.ImportaDiarioExpedicao
                        .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                            && w.TipoDEO == "Inventário de Ovos")
                        .FirstOrDefault();

                    string empresa = "1";

                    if (!importaDiarioExpedicao.NumIdentificacao.Equals("") &&
                        importaDiarioExpedicao.NumIdentificacao != null)
                    {
                        localErro = "Erro ao Deletar Controle de Inventário: ";
                        bdApolo.DeleteCtrlinv(empresa, importaDiarioExpedicao.NumIdentificacao);
                    }

                    CTRL_INV ctrlInv = new CTRL_INV();

                    System.Data.Objects.ObjectParameter numero =
                        new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
                    apoloService.gerar_codigo(empresa, "CTRL_INV", numero);

                    ctrlInv.EmpCod = empresa;
                    ctrlInv.CtrlInvNum = numero.Value.ToString();
                    ctrlInv.CtrlInvDataEmis = dataHoraCarreg.Date;
                    ctrlInv.CtrlInvDataRealiz = DateTime.Today;
                    ctrlInv.CtrlInvContag1Analisada = "Não";
                    ctrlInv.CtrlInvContag2Analisada = "Não";
                    ctrlInv.CtrlInvContag3Analisada = "Não";
                    ctrlInv.CtrlInvVisPocket = "Não";

                    bdApolo.CTRL_INV.AddObject(ctrlInv);

                    var listaItens = hlbapp.ImportaDiarioExpedicao
                        .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                            && w.TipoDEO == "Inventário de Ovos")
                        .GroupBy(g => g.Linhagem)
                        .OrderBy(o => o.Key)
                        .ToList();

                    foreach (var item in listaItens)
                    {
                        MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO produtoApolo =
                            bdApolo.PRODUTO.Where(w => w.ProdNomeAlt1 == item.Key)
                            .FirstOrDefault();

                        ITEM_CTRL_INV1 itemCtrlInv = new ITEM_CTRL_INV1();
                        itemCtrlInv.EmpCod = ctrlInv.EmpCod;
                        itemCtrlInv.CtrlInvNum = ctrlInv.CtrlInvNum;
                        itemCtrlInv.ProdCodEstr = produtoApolo.ProdCodEstr;
                        itemCtrlInv.ItCtrlInv1QtdDifEstq = 0;

                        bdApolo.ITEM_CTRL_INV1.AddObject(itemCtrlInv);

                        ITEM_CTRL_INV_LOC_ARM1 itemCtrlInvLoc = new ITEM_CTRL_INV_LOC_ARM1();
                        itemCtrlInvLoc.EmpCod = itemCtrlInv.EmpCod;
                        itemCtrlInvLoc.CtrlInvNum = itemCtrlInv.CtrlInvNum;
                        itemCtrlInvLoc.ProdCodEstr = itemCtrlInv.ProdCodEstr;

                        LOC_ARMAZ locArmaz = apoloService.LOC_ARMAZ
                            .Where(w => w.USERCodigoFLIP == granja
                                && w.USERTipoProduto == "Ovos Incubáveis")
                            .FirstOrDefault();

                        itemCtrlInvLoc.LocArmazCodEstr = locArmaz.LocArmazCodEstr;
                        itemCtrlInvLoc.ItCtrlInvLocArm1QtdDifEstq = 0;
                        itemCtrlInvLoc.ItCtrlInvLocArm1DataDig = dataHoraCarreg.Date;
                        itemCtrlInvLoc.ItCtrlInvLocArm1DataContag = DateTime.Today;
                        itemCtrlInvLoc.ItCtrlInvLocArm1Analisado = "Não";
                        itemCtrlInvLoc.ItCtrlInvLocArm1Seq = 1;

                        var listaLotes = apoloService.CTRL_LOTE_LOC_ARMAZ
                            .Where(w => w.EmpCod == empresa
                                && w.ProdCodEstr == produtoApolo.ProdCodEstr
                                && w.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                && w.CtrlLoteLocArmazQtdSaldo != 0)
                            .ToList();

                        decimal? qtdLocal = 0;

                        foreach (var lote in listaLotes)
                        {
                            CTRL_LOTE_INV_LOC_ARM1 loteCtrlInv = new CTRL_LOTE_INV_LOC_ARM1();

                            loteCtrlInv.EmpCod = itemCtrlInvLoc.EmpCod;
                            loteCtrlInv.CtrlInvNum = itemCtrlInvLoc.CtrlInvNum;
                            loteCtrlInv.ProdCodEstr = itemCtrlInvLoc.ProdCodEstr;
                            loteCtrlInv.LocArmazCodEstr = itemCtrlInvLoc.LocArmazCodEstr;
                            loteCtrlInv.CtrlLoteNum = lote.CtrlLoteNum;
                            loteCtrlInv.CtrlLoteDataValid = lote.CtrlLoteDataValid;

                            ImportaDiarioExpedicao importaDEO = hlbapp.ImportaDiarioExpedicao
                                .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                                    && w.LoteCompleto == lote.CtrlLoteNum
                                    && w.DataProducao == lote.CtrlLoteDataValid
                                    && w.TipoDEO == "Inventário de Ovos")
                                .FirstOrDefault();

                            if (importaDEO != null)
                                loteCtrlInv.CtrlLoteInvLocArm1Qtd = importaDEO.QtdeOvos;
                            else
                                loteCtrlInv.CtrlLoteInvLocArm1Qtd = 0;

                            loteCtrlInv.CtrlLoteInvLocArm1QtdCalc = loteCtrlInv.CtrlLoteInvLocArm1Qtd;
                            qtdLocal = qtdLocal + loteCtrlInv.CtrlLoteInvLocArm1QtdCalc;

                            loteCtrlInv.CtrlLoteInvLocArm1QtdDifEstq = 0;
                            loteCtrlInv.CtrlLoteInvLocArm1UnidMedCod = "UN";
                            loteCtrlInv.CtrlLoteInvLocArm1UnidMedPos = 1;
                            loteCtrlInv.CtrlLoteInvLocArm1DataDig = dataHoraCarreg.Date;
                            loteCtrlInv.CtrlLoteInvLocArm1DataContag = DateTime.Today;
                            loteCtrlInv.CtrlLoteInvLocArm1Analisado = "Não";

                            bdApolo.CTRL_LOTE_INV_LOC_ARM1.AddObject(loteCtrlInv);
                        }

                        itemCtrlInvLoc.ItCtrlInvLocArm1Qtd = qtdLocal;
                        bdApolo.ITEM_CTRL_INV_LOC_ARM1.AddObject(itemCtrlInvLoc);
                    }

                    localErro = "Erro ao Salvar Controle de Inventário: ";
                    bdApolo.SaveChanges();

                    foreach (var item in listaItens)
                    {
                        MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO produtoApolo =
                            bdApolo.PRODUTO.Where(w => w.ProdNomeAlt1 == item.Key)
                            .FirstOrDefault();

                        localErro = "Erro ao Analisar Contagem da Linahgem " + item.Key + ": ";
                        //bdApolo.AnalisaContagemCtrlInv(ctrlInv.EmpCod, ctrlInv.CtrlInvNum, 1,
                        //    produtoApolo.ProdCodEstr, produtoApolo.ProdCodEstr);
                    }

                    localErro = "Erro ao Ajustar Contagem: ";
                    bdApolo.GeraAjustePrimeiraContagem(ctrlInv.EmpCod, ctrlInv.CtrlInvNum);
                    localErro = "Erro ao Gerar Mov. Estoque de Entrada: ";
                    bdApolo.CtrlInvGeraMovEstq(ctrlInv.EmpCod, ctrlInv.CtrlInvNum, "RIOSOFT", "Entrada", "Sim");
                    localErro = "Erro ao Gerar Mov. Estoque de Saída: ";
                    bdApolo.CtrlInvGeraMovEstq(ctrlInv.EmpCod, ctrlInv.CtrlInvNum, "RIOSOFT", "Saída", "Sim");

                    var deosL = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                            && w.TipoDEO == "Inventário de Ovos")
                        .ToList();

                    foreach (var deo in deosL)
                    {
                        deo.NumIdentificacao = ctrlInv.CtrlInvNum;
                    }

                    var deos = hlbapp.ImportaDiarioExpedicao
                        .Where(w => w.Granja == granja && w.DataHoraCarreg == dataHoraCarreg
                            && w.TipoDEO == "Inventário de Ovos")
                        .ToList();

                    foreach (var deo in deos)
                    {
                        deo.NumIdentificacao = ctrlInv.CtrlInvNum;
                    }

                    hlbapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ViewBag.Erro = localErro
                        + ex.Message + " / " + ex.InnerException.Message;
                else
                    ViewBag.Erro = localErro + ex.Message;
            }

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");

            return View("ListaDEOs", CarregarListaDEOFiltro(granja, dataInicial, dataFinal, tipoDEO));
        }

        #endregion

        #region Métodos Transferência de Linhagens

        public ImportaIncubacao.Data.Apolo.PRODUTO RetornaProdutoPelaLinha(string linha)
        {
            return apoloService.PRODUTO.Where(p => p.ProdNomeAlt1 == linha).First();
        }

        public MOV_ESTQ InsereMovEstq(string empresa, string tipoLanc, string entCod, 
            DateTime dataMovimentacao, string usuario)
        {
            MOV_ESTQ movEstq = new MOV_ESTQ();

            System.Data.Objects.ObjectParameter chave =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));

            apoloService.gerar_codigo(empresa, "MOV_ESTQ", chave);

            movEstq.EmpCod = empresa;
            movEstq.MovEstqChv = (int)chave.Value;
            movEstq.TipoLancCod = tipoLanc;
            movEstq.MovEstqDataMovimento = dataMovimentacao;
            movEstq.MovEstqDataEmissao = dataMovimentacao;
            movEstq.MovEstqDocEmpCod = "1";
            movEstq.MovEstqDocEspec = "TLIN";
            movEstq.MovEstqDocSerie = "0";
            movEstq.MovEstqDocNum = dataMovimentacao.ToShortDateString();
            movEstq.EntCod = entCod;
            movEstq.MovEstqRatValDespDiv = "Sim";
            movEstq.UsuCod = usuario;
            movEstq.MovEstqDataHoraDig = DateTime.Now;
            movEstq.MovEstqIntegFisc = "Não";
            movEstq.MovEstqValIssDedTot = "Não";
            movEstq.MovEstqValInssDedTot = "Não";
            movEstq.MovEstq = "Sim";
            movEstq.MovEstqValIrrfDedTot = "Não";
            movEstq.MovEstqOrig = "Estoque";
            movEstq.MovEstqRejPat = "Não";
            movEstq.MovEstqDataEntrada = dataMovimentacao;
            movEstq.TipoPagRecCod = "0000002";
            movEstq.MovEstqValCsllDedTot = "Não";
            movEstq.MovEstqValCofinsDedTot = "Não";
            movEstq.MovEstqValPisDedTot = "Não";
            movEstq.MovEstqIcmsFreteSomaIcmsST = "Não";
            movEstq.MovEstqRateioFretePorPeso = "Não";
            movEstq.MovEstqRateioCapPorPeso = "Sim";
            movEstq.MovEstqValPagRecAntIcmsST = "Não";
            movEstq.MovEstqSelec = "Não";
            movEstq.MovEstqGeraFiscal = "Não";
            movEstq.MovEstqValOutrDespCompValDoc = "Não";
            movEstq.MovEstqDesabRecalcVal = "Não";
            movEstq.MovEstqIndTipoFrete = "Sem Frete";
            movEstq.MovEstqValCofinsProdDedTot = "Não";
            movEstq.MovEstqValPisProdDedTot = "Não";
            movEstq.MovEstqValFunruralDedTot = "Não";
            movEstq.MovEstqEntNome = "HY LINE DO BRASIL LTDA";
            movEstq.MovEstqEntCpfCgc = "02924519000787";
            movEstq.MovEstqRGIE = "478.008.918.115";
            movEstq.MovEstqEntEnder = "MARGINAL BR 153";
            movEstq.MovEstqEntEnderNo = "S/N";
            movEstq.MovEstqEntBair = "DIST. INDUSTRIAL";
            movEstq.MovEstqCodPais = "BRA";
            movEstq.MovEstqCidNome = "NOVA GRANADA";
            movEstq.MovEstqUfSigla = "SP";
            movEstq.MovEstqCodCid = "00085839";
            movEstq.MovEstqDeduzPISParc = "Não";
            movEstq.MovEstqDeduzCofinsParc = "Não";
            movEstq.MovEstqDeduzCsllParc = "Não";
            movEstq.MovEstqDataEntrega = dataMovimentacao;
            movEstq.MOVESTQCONFINTEGSIS = "Regular";
            movEstq.MovEstqRatTxaMarMercPorPeso = "Não";
            movEstq.MovEstqRatSiscomexPorPeso = "Não";
            movEstq.MovEstqRatDespImportPorPeso = "Não";

            return movEstq;
        }

        public ITEM_MOV_ESTQ InsereItemMovEstq(int chave, string empresa, string tipoLanc, string entCod,
            DateTime dataMovimentacao, string linha, string naturezaOperacao, decimal? quantidade,
            decimal? valorUnitario, string unidadeMedida, short? posicaoUnidadeMedida, string tribCod,
            string itMovEstqClasFiscCodNbm, string clasFiscCod)
        {
            string mensagem = "";

            ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();

            itemMovEstq.EmpCod = empresa;

            ImportaIncubacao.Data.Apolo.PRODUTO produto = RetornaProdutoPelaLinha(linha);

            itemMovEstq.ProdCodEstr = produto.ProdCodEstr;
            itemMovEstq.MovEstqChv = chave;

            //short sequencia = RetornaUltimaSequenciaItemMovEstq(empresa, chave);
            short sequencia = 0;

            itemMovEstq.ItMovEstqSeq = ++sequencia;
            itemMovEstq.ItMovEstqDataMovimento = dataMovimentacao;
            itemMovEstq.TipoLancCod = tipoLanc;
            itemMovEstq.NatOpCodEstr = naturezaOperacao;
            itemMovEstq.ItMovEstqQtdProd = Convert.ToDecimal(quantidade);
            itemMovEstq.ItMovEstqValProd = Convert.ToDecimal(valorUnitario * quantidade);

            PROD_UNID_MED prodUnidMed = produto.PROD_UNID_MED
                .Where(u => u.ProdUnidMedCod == unidadeMedida && u.ProdUnidMedPos == posicaoUnidadeMedida)
                .FirstOrDefault();

            if (prodUnidMed != null)
            {
                itemMovEstq.ItMovEstqUnidMedCod = unidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPos = posicaoUnidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPeso = prodUnidMed.ProdUnidMedPeso;
                itemMovEstq.ItMovEstqUnidMedPesoFD = prodUnidMed.ProdUnidMedPesoFD;
                itemMovEstq.ItMovEstqUnidMedCodVal = unidadeMedida;
                itemMovEstq.ItMovEstqUnidMedPosVal = posicaoUnidadeMedida;
            }
            else
            {
                itemMovEstq.ItMovEstqObs = "Unidade de Medida não cadastrada no Produto! Verifique!";
                return itemMovEstq;
            }

            itemMovEstq.ItMovEstqBaseCustoMed = itemMovEstq.ItMovEstqValProd;
            itemMovEstq.ItMovEstqCustoUnit = Convert.ToDecimal(valorUnitario);
            itemMovEstq.ItMovEstqServ = "Não";
            itemMovEstq.TribCod = tribCod;
            itemMovEstq.ItMovEstqClasFiscCodNbm = itMovEstqClasFiscCodNbm;
            itemMovEstq.ItMovEstqQtdCalcProd = Convert.ToDecimal(quantidade);
            itemMovEstq.ItMovEstq = "Sim";
            itemMovEstq.ItMovEstqSeqItOrig = itemMovEstq.ItMovEstqSeq;
            itemMovEstq.ItMovEstqSeqDesm = 1;
            itemMovEstq.TribACod = tribCod.Substring(0, 1);
            itemMovEstq.TribBCod = tribCod.Substring(1, 2);
            itemMovEstq.ClasFiscCod = clasFiscCod;
            itemMovEstq.ItMovEstqUnidMedPosVal = itemMovEstq.ItMovEstqUnidMedPos;
            itemMovEstq.EntCod = entCod;
            itemMovEstq.ItMovEstqProdNome = produto.ProdNome;
            itemMovEstq.ItMovEstqChvOrd = chave;
            itemMovEstq.ItMovEstqMotDesonICMS = "Nenhum";
            itemMovEstq.USERCalculadoSaldoServico = "Não";

            mensagem = "OK" + itemMovEstq.ProdCodEstr;

            return itemMovEstq;
        }

        public LOC_ARMAZ_ITEM_MOV_ESTQ InsereLocalArmazenagem(int chave, string empresa, short sequencia, string prodCodEstr,
            decimal? quantidade, string localArmazenagem)
        {
            LOC_ARMAZ_ITEM_MOV_ESTQ local = new LOC_ARMAZ_ITEM_MOV_ESTQ();

            local.EmpCod = empresa;
            local.MovEstqChv = chave;
            local.ProdCodEstr = prodCodEstr;
            local.ItMovEstqSeq = sequencia;
            local.LocArmazCodEstr = localArmazenagem;
            local.LocArmazItMovEstqQtd = quantidade;
            local.LocArmazItMovEstqQtdCalc = quantidade;

            return local;
        }

        public CTRL_LOTE_ITEM_MOV_ESTQ InsereLote(int chave, string empresa, short sequencia, string prodCodEstr,
            string numLote, DateTime dataProducao, decimal? quantidade, string operacao, string unidadeMedida,
            short? posicaoUnidadeMedida, string localArmazenagem)
        {
            CTRL_LOTE_ITEM_MOV_ESTQ loteItemMovEstq = new CTRL_LOTE_ITEM_MOV_ESTQ();

            loteItemMovEstq.EmpCod = empresa;
            loteItemMovEstq.ProdCodEstr = prodCodEstr;
            loteItemMovEstq.CtrlLoteNum = numLote;
            loteItemMovEstq.CtrlLoteDataValid = dataProducao;
            loteItemMovEstq.MovEstqChv = chave;
            loteItemMovEstq.ItMovEstqSeq = sequencia;
            loteItemMovEstq.CtrlLoteItMovEstqQtd = quantidade;
            loteItemMovEstq.CtrlLoteItMovEstqOper = operacao;
            loteItemMovEstq.CtrlLoteItMovEstqDataFab = dataProducao;
            loteItemMovEstq.CtrlLoteItMovEstqUnidMedCod = unidadeMedida;
            loteItemMovEstq.CtrlLoteItMovEstqUnidMedPos = posicaoUnidadeMedida;
            loteItemMovEstq.LocArmazCodEstr = localArmazenagem;
            loteItemMovEstq.CtrlLoteItMovEstqQtdCalc = loteItemMovEstq.CtrlLoteItMovEstqQtd;

            return loteItemMovEstq;
        }

        public string GeraMovimentacoesTransferenciaDeLinhagens(string granja, DateTime dataHoraCarreg, string tipoDEO)
        {
            string localErro = "";

            try
            {
                #region Carrega Variáveis e Objetos

                HLBAPPEntities hlbapp = new HLBAPPEntities();
                
                apoloService.CommandTimeout = 1000;
                bdApolo.CommandTimeout = 1000;
                hlbapp.CommandTimeout = 300;

                var lista = CarregarItensDEOImport(hlbapp, dataHoraCarreg, granja);

                //List<ImportaDiarioExpedicao> listaItensDadosAntigosImport = (List<ImportaDiarioExpedicao>)Session["listaItensDadosAntigosImport"];
                //ImportaDiarioExpedicao importaDiarioExpedicao = listaItensDadosAntigosImport.FirstOrDefault();
                ImportaDiarioExpedicao importaDiarioExpedicao = new ImportaDiarioExpedicao();

                string empresa = "1";
                string usuario = "";
                if (Session["login"].ToString().ToUpper().Equals("PALVES"))
                    usuario = "RIOSOFT";
                else
                    usuario = Session["login"].ToString().ToUpper();
                Session["login"].ToString().ToUpper();
                string linhaOrigem = Session["linhagemOrigemSelecionada"].ToString();
                string linhaDestino = Session["linhagemDestinoSelecionada"].ToString();

                string naturezaOperacao = "1.556.001";
                decimal? valorUnitario = 0.25m;
                string unidadeMedida = "UN";
                short? posicaoUnidadeMedida = 1;
                string tribCod = "040";
                string itMovEstqClasFiscCodNbm = "99999999";
                string clasFiscCod = "013";

                LOC_ARMAZ localArmazCadastro = apoloService.LOC_ARMAZ
                        .Where(l => l.USERCodigoFLIP == granja && l.USERTipoProduto == "Ovos Incubáveis")
                        .FirstOrDefault();

                decimal qtdTotal = lista.Sum(s => s.QtdeOvos);

                #endregion

                #region Deleta Movimentação de Entrada caso exista para substituir

                if (!importaDiarioExpedicao.ResponsavelCarreg.Equals("") &&
                    importaDiarioExpedicao.ResponsavelCarreg != null)
                {
                    int movestqchv = Convert.ToInt32(importaDiarioExpedicao.ResponsavelCarreg);

                    localErro = "Erro ao Deletar a Entrada da Transferência de Linhagens: ";
                    MOV_ESTQ movestq = apoloService.MOV_ESTQ
                        .Where(w => w.EmpCod == empresa && w.MovEstqChv == movestqchv)
                        .FirstOrDefault();

                    if (movestq != null) DeletaMovEstq(movestq);
                }

                #endregion

                #region Deleta Movimentação de Saída caso exista para substituir

                if (!importaDiarioExpedicao.ResponsavelReceb.Equals("") &&
                    importaDiarioExpedicao.ResponsavelReceb != null)
                {
                    int movestqchv = Convert.ToInt32(importaDiarioExpedicao.ResponsavelReceb);

                    localErro = "Erro ao Deletar a Saída da Transferência de Linhagens: ";
                    MOV_ESTQ movestq = apoloService.MOV_ESTQ
                        .Where(w => w.EmpCod == empresa && w.MovEstqChv == movestqchv)
                        .FirstOrDefault();

                    if (movestq != null) DeletaMovEstq(movestq);
                }

                #endregion

                #region Insere Saída

                localErro = "Erro ao Inserir a Movimentação de Saída da Transferência de Linhagens: ";
                MOV_ESTQ movestqSaida = InsereMovEstq(empresa, "E0000492", null, dataHoraCarreg.Date, usuario);
                apoloService.MOV_ESTQ.AddObject(movestqSaida);
                apoloService.SaveChanges();

                localErro = "Erro ao Inserir o Item da Movimentação de Saída da Transferência de Linhagens: ";
                ITEM_MOV_ESTQ itemMovEstqSaida = InsereItemMovEstq(movestqSaida.MovEstqChv, movestqSaida.EmpCod, 
                    movestqSaida.TipoLancCod, movestqSaida.EntCod, movestqSaida.MovEstqDataMovimento, linhaOrigem,
                    naturezaOperacao, qtdTotal, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                    itMovEstqClasFiscCodNbm, clasFiscCod);
                apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstqSaida);

                localErro = "Erro ao Inserir o Local do Item da Movimentação de Saída da Transferência de Linhagens: ";
                LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstqSaida = InsereLocalArmazenagem(itemMovEstqSaida.MovEstqChv,
                    itemMovEstqSaida.EmpCod, itemMovEstqSaida.ItMovEstqSeq, itemMovEstqSaida.ProdCodEstr, qtdTotal,
                    localArmazCadastro.LocArmazCodEstr);
                apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstqSaida);

                foreach (var item in lista)
                {
                    string loteTL = item.LoteCompleto + "_TL";

                    localErro = "Erro ao Inserir o Lote " + item.LoteCompleto + " / " 
                        + item.DataProducao.ToShortDateString() 
                        + " do Item da Movimentação de Saída da Transferência de Linhagens: ";
                    CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(itemMovEstqSaida.MovEstqChv, itemMovEstqSaida.EmpCod,
                         itemMovEstqSaida.ItMovEstqSeq, itemMovEstqSaida.ProdCodEstr, item.LoteCompleto,
                         item.DataProducao, item.QtdeOvos, "Saída", unidadeMedida, posicaoUnidadeMedida,
                         localArmazCadastro.LocArmazCodEstr);
                    apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                }

                apoloService.SaveChanges();
                localErro = "Erro ao Calcular Mov. Estq. de Saída: ";
                apoloService.calcula_mov_estq(movestqSaida.EmpCod, movestqSaida.MovEstqChv);

                #endregion

                #region Insere Entrada

                localErro = "Erro ao Inserir a Movimentação de Entrada da Transferência de Linhagens: ";
                MOV_ESTQ movestqEntrada = InsereMovEstq(empresa, "E0000502", null, dataHoraCarreg.Date, usuario);
                apoloService.MOV_ESTQ.AddObject(movestqEntrada);
                apoloService.SaveChanges();

                localErro = "Erro ao Inserir o Item da Movimentação de Entrada da Transferência de Linhagens: ";
                ITEM_MOV_ESTQ itemMovEstqEntrada = InsereItemMovEstq(movestqEntrada.MovEstqChv, movestqEntrada.EmpCod,
                    movestqEntrada.TipoLancCod, movestqEntrada.EntCod, movestqEntrada.MovEstqDataMovimento, linhaDestino,
                    naturezaOperacao, qtdTotal, valorUnitario, unidadeMedida, posicaoUnidadeMedida, tribCod,
                    itMovEstqClasFiscCodNbm, clasFiscCod);
                apoloService.ITEM_MOV_ESTQ.AddObject(itemMovEstqEntrada);

                localErro = "Erro ao Inserir o Local do Item da Movimentação de Entrada da Transferência de Linhagens: ";
                LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstqEntrada = InsereLocalArmazenagem(itemMovEstqEntrada.MovEstqChv,
                    itemMovEstqEntrada.EmpCod, itemMovEstqEntrada.ItMovEstqSeq, itemMovEstqEntrada.ProdCodEstr, qtdTotal,
                    localArmazCadastro.LocArmazCodEstr);
                apoloService.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstqEntrada);

                foreach (var item in lista)
                {
                    string loteTL = item.LoteCompleto + "_TL";

                    localErro = "Erro ao Inserir o Lote " + item.LoteCompleto + " / "
                        + item.DataProducao.ToShortDateString()
                        + " do Item da Movimentação de Entrada da Transferência de Linhagens: ";
                    CTRL_LOTE_ITEM_MOV_ESTQ lote = InsereLote(itemMovEstqEntrada.MovEstqChv, itemMovEstqEntrada.EmpCod,
                         itemMovEstqEntrada.ItMovEstqSeq, itemMovEstqEntrada.ProdCodEstr, loteTL,
                         item.DataProducao, item.QtdeOvos, "Entrada", unidadeMedida, posicaoUnidadeMedida,
                         localArmazCadastro.LocArmazCodEstr);
                    apoloService.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                }

                apoloService.SaveChanges();
                localErro = "Erro ao Calcular Mov. Estq. de Entrada: ";
                apoloService.calcula_mov_estq(movestqEntrada.EmpCod, movestqEntrada.MovEstqChv);

                #endregion

                Session["chaveMovEstqSaida"] = movestqSaida.MovEstqChv;
                Session["chaveMovEstqEntrada"] = movestqEntrada.MovEstqChv;

                localErro = "";

                return localErro;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return localErro
                        + ex.Message + " / " + ex.InnerException.Message;
                else
                    return localErro + ex.Message;
            }
        }

        public string GeraTransferenciaDeLinhagensFLIP(string granja, DateTime dataHoraCarreg, string tipoDEO)
        {
            string localErro = "";

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            try
            {
                #region Lista os Lotes do DEO

                var listaLotes = CarregarItensDEOImport(hlbapp, dataHoraCarreg, granja)
                    .GroupBy(g => g.LoteCompleto)
                    .OrderBy(o => o.Key)
                    .ToList();

                #endregion

                foreach (var item in listaLotes)
                {
                    #region Verifica se existe o Lote na Transferência (_TL). Caso não tenha, será inserido.

                    FLOCKSMobileTableAdapter flocksMobile = new FLOCKSMobileTableAdapter();

                    string loteTL = item.Key + "_TL";

                    localErro = "Erro ao verificar Lote " + loteTL + " no FLIP: ";

                    FLIPDataSetMobile.FLOCKSMobileDataTable flockDataTableVerifica =
                            new FLIPDataSetMobile.FLOCKSMobileDataTable();

                    //flocksMobile.FillByFlockID(flockDataTableVerifica, loteTL);
                    decimal? existeF = flocksMobile.ExistsFlockID(loteTL);

                    if (existeF == 0)
                    {
                        FLIPDataSetMobile.FLOCKSMobileDataTable flockDataTable = 
                            new FLIPDataSetMobile.FLOCKSMobileDataTable();

                        flocksMobile.FillByFlockID(flockDataTable, item.Key);

                        FLIPDataSetMobile.FLOCKSMobileRow row = flockDataTable[0];

                        string linhagemDestino = Session["linhagemDestinoSelecionada"].ToString();

                        //flocksMobile.Insert(row.COMPANY, row.REGION, row.LOCATION, row.FARM_ID, row.ACTIVE,
                        //    loteTL, row.PULLET_ID, importa.GTANum, row.GEN, row.HATCH_DATE, row.MOVE_DATE,
                        //    row.SELL_DATE, row.HENS_HATCHED, row.HENS_MOVED, row.HENS_SOLD, row.MALES_HATCHED,
                        //    row.MALES_MOVED, row.MALES_SOLD, row.PULLET_HEN_FC, null, row.TEXT_1, row.NUM_1,
                        //    row.DATE_1, row.TEXT_2, row.NUM_2, row.DATE_2, row.FARM_KEY, row.TEXT_3, row.NUM_3,
                        //    row.TEXT_4);

                        localErro = "Erro ao cadastrar Lote " + loteTL + " no FLIP: ";

                        flocksMobile.InsertQuery(row.COMPANY, row.REGION, row.LOCATION, row.FARM_ID, row.ACTIVE,
                            loteTL, row.PULLET_ID, linhagemDestino, row.GEN, row.HATCH_DATE);
                    }

                    #endregion

                    #region Lista as Datas de Produção para cada Lote do DEO

                    var listaDatas = CarregarItensDEOImport(hlbapp, dataHoraCarreg, granja)
                        .Where(w => w.LoteCompleto == item.Key)
                        .OrderBy(o => o.DataProducao)
                        .ToList();

                    #endregion

                    foreach (var data in listaDatas)
                    {
                        #region Verifica se existe a Data de Produção para o Lote de Transferência. Caso não tenha, será inserido. Caso tenha, será atualizado.

                        localErro = "Erro ao verificar Data de Produção " + data.DataProducao.ToShortDateString() 
                            + " do Lote " + loteTL + " no FLIP: ";

                        FLOCK_DATAMobileTableAdapter flockDataMobile = new FLOCK_DATAMobileTableAdapter();

                        FLIPDataSetMobile.FLOCK_DATAMobileDataTable flockDataDataTableVerifica =
                                new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();

                        flockDataMobile.FillByFlockAndDate(flockDataDataTableVerifica, loteTL, data.DataProducao);

                        if (flockDataDataTableVerifica.Count == 0)
                        {
                            FLIPDataSetMobile.FLOCK_DATAMobileDataTable flockDataDataTable =
                                new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();

                            flockDataMobile.FillByFlockAndDate(flockDataDataTable, data.LoteCompleto, data.DataProducao);

                            FLIPDataSetMobile.FLOCK_DATAMobileRow rowData = flockDataDataTable[0];

                            //flocksMobile.FillByFlockID(flockDataTableVerifica, loteTL);

                            string flockKey = flocksMobile.ReturnFlockKey(loteTL).ToString();

                            //flockDataMobile.Insert(rowData.COMPANY, rowData.REGION, rowData.LOCATION, rowData.FARM_ID,
                            //    loteTL, rowData.ACTIVE, rowData.TRX_DATE, rowData.AGE, rowData.HEN_MORT, rowData.HEN_WT,
                            //    rowData.MALE_MORT, rowData.HEN_FEED_DEL, rowData.TOTAL_EGGS_PROD, rowData.EGG_WT,
                            //    flipMobile.FLOCK_DATAMobile[0].FLOCK_KEY, rowData.DATE_1, rowData.DATE_2, rowData.TEXT_1,
                            //    rowData.TEXT_2, data.QtdeOvos, rowData.NUM_2, rowData.NUM_3, rowData.NUM_4,
                            //    rowData.NUM_5, rowData.NUM_6, rowData.NUM_7, rowData.NUM_8, rowData.NUM_9,
                            //    rowData.NUM_10, rowData.NUM_11, rowData.NUM_12, rowData.NUM_13, rowData.TEXT_3,
                            //    rowData.NUM_14);

                            localErro = "Erro ao inserir Data de Produção " + data.DataProducao.ToShortDateString()
                            + " do Lote " + loteTL + " no FLIP: ";

                            flockDataMobile.InsertQuery(rowData.COMPANY, rowData.REGION, rowData.LOCATION, rowData.FARM_ID,
                                loteTL, rowData.ACTIVE, rowData.TRX_DATE, rowData.AGE,
                                flockKey, data.QtdeOvos);
                        }
                        else
                        {
                            localErro = "Erro ao atualizar Data de Produção " + data.DataProducao.ToShortDateString()
                            + " do Lote " + loteTL + " no FLIP: ";

                            decimal qtd = flockDataDataTableVerifica[0].NUM_1 + data.QtdeOvos;

                            flockDataMobile.UpdateHE(qtd, loteTL, data.DataProducao);
                        }

                        #endregion

                        #region Atualiza o Lote Antigo, subtraindo a quantidade transferida.
                        
                        localErro = "Erro ao atualizar Data de Produção " + data.DataProducao.ToShortDateString()
                            + " do Lote Origem " + data.LoteCompleto + " no FLIP: ";

                        FLIPDataSetMobile.FLOCK_DATAMobileDataTable flockDataDataTableAntigo =
                            new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();
                        flockDataMobile.FillByFlockAndDate(flockDataDataTableAntigo, data.LoteCompleto,
                            data.DataProducao);

                        decimal qtdAntiga = flockDataDataTableAntigo[0].NUM_1 - data.QtdeOvos;

                        flockDataMobile.UpdateHE(qtdAntiga, data.LoteCompleto, data.DataProducao);

                        #endregion
                    }
                }

                localErro = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return localErro
                        + ex.Message + " / " + ex.InnerException.Message;
                else
                    return localErro + ex.Message;
            }

            return localErro;
        }

        public string DeletaTransferenciaDeLinhagensFLIP(List<ImportaDiarioExpedicao> listaItensDadosAntigosImport)
        {
            string localErro = "";

            try
            {
                #region Lista os Lotes do DEO

                var listaLotes = listaItensDadosAntigosImport
                    .GroupBy(g => g.LoteCompleto)
                    .OrderBy(o => o.Key)
                    .ToList();

                #endregion

                foreach (var item in listaLotes)
                {
                    string loteTL = item.Key + "_TL";

                    #region Lista as Datas de Produção para cada Lote do DEO

                    var listaDatas = listaItensDadosAntigosImport
                        .Where(w => w.LoteCompleto == item.Key)
                        .OrderBy(o => o.DataProducao)
                        .ToList();

                    #endregion

                    FLOCK_DATAMobileTableAdapter flockDataMobile = new FLOCK_DATAMobileTableAdapter();

                    foreach (var data in listaDatas)
                    {
                        #region Verifica se existe a Data de Produção para o Lote de Transferência. Caso exista, será verificada a quantidade para deletar ou atualizar.

                        FLIPDataSetMobile.FLOCK_DATAMobileDataTable flockDataDataTableVerifica =
                            new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();

                        flockDataMobile.FillByFlockAndDate(flockDataDataTableVerifica, loteTL, data.DataProducao);

                        if (flockDataDataTableVerifica[0].NUM_1 == data.QtdeOvos)
                        {
                            flockDataMobile.DeleteFlockTL(loteTL, data.DataProducao);
                        }
                        else
                        {
                            decimal qtd = flockDataDataTableVerifica[0].NUM_1 - data.QtdeOvos;

                            flockDataMobile.UpdateHE(qtd, loteTL, data.DataProducao);
                        }

                        #endregion

                        #region Atualiza o Lote Antigo, somando a quantidade transferida.

                        FLIPDataSetMobile.FLOCK_DATAMobileDataTable flockDataDataTableAntigo =
                            new FLIPDataSetMobile.FLOCK_DATAMobileDataTable();
                        flockDataMobile.FillByFlockAndDate(flockDataDataTableAntigo, data.LoteCompleto,
                            data.DataProducao);

                        decimal qtdAntiga = flockDataDataTableAntigo[0].NUM_1 + data.QtdeOvos;

                        flockDataMobile.UpdateHE(qtdAntiga, data.LoteCompleto, data.DataProducao);

                        #endregion
                    }

                    #region Verifica se existe o Lote na Transferência (_TL). Caso não tenha, será deletado.

                    FLOCKSMobileTableAdapter flocksMobile = new FLOCKSMobileTableAdapter();

                    decimal? existe = flockDataMobile.ExistsFlockData(loteTL);

                    if (existe == 0)
                    {
                        flocksMobile.DeleteFlockID(loteTL);
                    }

                    #endregion
                }

                localErro = "";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return localErro
                        + ex.Message + " / " + ex.InnerException.Message;
                else
                    return localErro + ex.Message;
            }

            return localErro;
        }

        #endregion

        #region Métodos para Fechamento de Lançamentos de Produção

        public ActionResult FechamentoLancamentosProducao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["ListaLocaisFechLanc"] = CarregaListaLocaisFechLanc();

            return View();
        }

        [HttpPost]
        public ActionResult FecharDataLancamentos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega parâmetros

            var listaLocaisFechLanc = (List<SelectListItem>)Session["ListaLocaisFechLanc"];

            DateTime data = Convert.ToDateTime(model["data"]);
            string localFechLan = model["LocaisFechLanc"];
            string descricao = listaLocaisFechLanc.Where(w => w.Value == localFechLan).FirstOrDefault().Text;
            string motivo = model["motivo"];

            #endregion

            UpdateCloseDataDate(data, localFechLan, descricao, motivo);

            Session["ListaLocaisFechLanc"] = CarregaListaLocaisFechLanc();

            ViewBag.Mensagem = am.GetTextOnLanguage("Fechamento de Lançamentos em", Session["language"].ToString()) + " " + descricao 
                + " " + am.GetTextOnLanguage("para a data", Session["language"].ToString())
                + " " + data.ToShortDateString() + " " + am.GetTextOnLanguage("realizado com sucesso!", Session["language"].ToString());

            return View("FechamentoLancamentosProducao");
        }

        public void UpdateCloseDataDate(DateTime date, string local, string description, string cause)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            bool closed = false;

            if (description.Contains("Brasil - "))
            {
                #region HYBR

                Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter dflTA =
                    new Models.FLIPDataSetMobileTableAdapters.DATA_FECH_LANCTableAdapter();
                FLIPDataSetMobile.DATA_FECH_LANCDataTable dflDT = new FLIPDataSetMobile.DATA_FECH_LANCDataTable();
                dflTA.Fill(dflDT);

                foreach (var item in dflDT)
                {
                    if (item.LOCATION == local)
                    {
                        dflTA.UpdateDataFechLanc(date, item.LOCATION);
                        closed = true;
                    }
                }

                #endregion
            }
            else if (description.Contains("Chile - "))
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dflCLTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable dflCLDT = new ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable();
                dflCLTA.Fill(dflCLDT);

                foreach (var item in dflCLDT)
                {
                    if (item.LOCATION == local)
                    {
                        dflCLTA.UpdateDataFechLanc(date, item.LOCATION);
                        closed = true;
                    }
                }

                #endregion
            }
            else if (description.Contains("Colombia - "))
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter dflCOTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable dflCODT = new ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable();
                dflCOTA.Fill(dflCODT);

                foreach (var item in dflCODT)
                {
                    if (item.LOCATION == local)
                    {
                        dflCOTA.UpdateDataFechLanc(date, item.LOCATION);
                        closed = true;
                    }
                }

                #endregion
            }

            if (closed)
            {
                #region Gera LOG

                LOG_DATA_FECH_LANC log = new LOG_DATA_FECH_LANC();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                log.Location = local;
                log.Data_Fech_Lanc = date;
                log.Motivo = cause;

                hlbappSession.LOG_DATA_FECH_LANC.AddObject(log);
                hlbappSession.SaveChanges();

                #endregion
            }
        }

        #endregion

        #region Métodos Ajuste p/ Inventário - WEB

        #region Lista Ajuste de Estoque

        public List<LayoutDiarioExpedicaos> CarregaAjusteEstoque(string incubatorio, DateTime dataInicial,
            DateTime dataFinal)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            DateTime dataHoraIni = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            DateTime dataHoraFim = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            var lista = bd.LayoutDiarioExpedicaos
                .Where(w => 
                    (w.Granja == incubatorio || bd.TIPO_CLASSFICACAO_OVO
                        .Any(a => a.Unidade == incubatorio
                            && a.CodigoTipo == w.Granja
                            && a.AproveitamentoOvo == "Incubável"
                            && a.Origem == "Interna"))
                    && w.TipoDEO == "Solicitação Ajuste de Estoque"
                    && w.DataHoraCarreg >= dataHoraIni
                    && w.DataHoraCarreg <= dataHoraFim)
                .ToList();

            return lista;
        }

        public ActionResult FiltraListaAjusteEstoque(string incubatorio,
            DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioSelecionado"] = incubatorio;
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);
            Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            Session["dataInicial"] = dataInicial.ToShortDateString();
            Session["dataFinal"] = dataFinal.ToShortDateString();
            dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            Session["ListaAjusteEstoque"] = CarregaAjusteEstoque(incubatorio, dataInicial, dataFinal);

            return View("ListaAjusteEstoque");
        }

        public void RefreshListaAjusteEstoque()
        {
            string incubatorio = "";
            if (Session["ListaIncubatorios"] == null) Session["ListaIncubatorios"] = CarregaListaIncubatoriosCO("", false, false);
            incubatorio = ((List<SelectListItem>)Session["ListaIncubatorios"]).FirstOrDefault().Value;
            Session["incubatorioSelecionado"] = incubatorio;
            Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            if (Session["incubatorioSelecionado"] != null)
                incubatorio = Session["incubatorioSelecionado"].ToString();
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            Session["ListaAjusteEstoque"] = CarregaAjusteEstoque(incubatorio, dataInicial, dataFinal);
        }

        public ActionResult ListaAjusteEstoque()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["ListaIncubatorios"] == null)
                Session["ListaIncubatorios"] = CarregaListaIncubatoriosCO("", false, false);
            RefreshListaAjusteEstoque();

            return View();
        }

        #endregion

        #region CRUD Ajuste Estoque

        public List<LayoutDiarioExpedicaos> GenerateAjusteEstoqueList(string local)
        {
            var listItens = new List<LayoutDiarioExpedicaos>();

            #region Load Egg Inv in the local

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var listEggInvByLocal = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.Local == local
                    && w.Qtde > 0
                    && w.LoteCompleto != "VARIOS")
                .OrderBy(o => o.LoteCompleto).ThenBy(t => t.DataProducao)
                //.OrderBy(o => o.DataProducao)
                .ToList();
                //.Take(5);

            foreach (var item in listEggInvByLocal)
            {
                FLOCK_DATA flockData = hlbapp.FLOCK_DATA
                    .Where(w => w.Flock_ID == item.LoteCompleto
                        && w.Trx_Date == item.DataProducao)
                    .FirstOrDefault();

                LayoutDiarioExpedicaos newDEO = new LayoutDiarioExpedicaos();
                newDEO.Nucleo = item.Nucleo;
                newDEO.Galpao = "0" + flockData.num_galpao.ToString();
                newDEO.Lote = item.NumLote;
                newDEO.Idade = Convert.ToInt32(flockData.Age);
                newDEO.Linhagem = item.Linhagem;
                newDEO.LoteCompleto = item.LoteCompleto;
                newDEO.DataProducao = item.DataProducao;
                newDEO.NumeroReferencia = item.DataProducao.DayOfYear.ToString();
                newDEO.QtdeOvos = 0;
                newDEO.QtdeBandejas = 0;
                newDEO.Usuario = Session["login"].ToString();
                newDEO.DataHora = DateTime.Now;
                //newDEO.DataHoraCarreg = dataHoraCarreg;
                newDEO.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
                newDEO.ResponsavelCarreg = "1 - Sem Classificar";
                newDEO.ResponsavelReceb = "";
                newDEO.NFNum = "";
                newDEO.Granja = item.Local;
                newDEO.Importado = "Inserindo";
                newDEO.Incubatorio = item.Local;
                newDEO.TipoDEO = "Solicitação Ajuste de Estoque";
                newDEO.GTANum = "";
                newDEO.Lacre = "";
                newDEO.NumIdentificacao = "";
                newDEO.CodItemDEO = 0;
                newDEO.Observacao = "";
                newDEO.TipoOvo = "";
                newDEO.QtdDiferenca = 0;

                listItens.Add(newDEO);
            }

            #endregion

            #region Load Egg Inv in classficated

            var listEggInvClassificated = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.Qtde > 0
                    && hlbapp.TIPO_CLASSFICACAO_OVO
                        .Any(a => a.Unidade == local
                            && a.CodigoTipo == w.Local
                            && a.AproveitamentoOvo == "Incubável"
                            && a.Origem == "Interna"))
                .OrderBy(o => o.Local)
                .ThenBy(o => o.LoteCompleto).ThenBy(t => t.DataProducao)
                //.ThenBy(d => d.DataProducao)
                .ToList();
                //.Take(5);

            foreach (var item in listEggInvClassificated)
            {
                FLOCK_DATA flockData = hlbapp.FLOCK_DATA
                    .Where(w => w.Flock_ID == item.LoteCompleto
                        && w.Trx_Date == item.DataProducao)
                    .FirstOrDefault();

                LayoutDiarioExpedicaos newDEO = new LayoutDiarioExpedicaos();
                newDEO.Nucleo = item.Nucleo;
                newDEO.Galpao = "0" + flockData.num_galpao.ToString();
                newDEO.Lote = flockData.NumLote;
                newDEO.Idade = Convert.ToInt32(flockData.Age);
                newDEO.Linhagem = item.Linhagem;
                newDEO.LoteCompleto = item.LoteCompleto;
                newDEO.DataProducao = item.DataProducao;
                newDEO.NumeroReferencia = item.DataProducao.DayOfYear.ToString();
                newDEO.QtdeOvos = 0;
                newDEO.QtdeBandejas = 0;
                newDEO.Usuario = Session["login"].ToString();
                newDEO.DataHora = DateTime.Now;
                //newDEO.DataHoraCarreg = dataHoraCarreg;
                newDEO.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
                newDEO.ResponsavelCarreg = "2 - Classificado";
                newDEO.ResponsavelReceb = "";
                newDEO.NFNum = "";
                newDEO.Granja = item.Local;
                newDEO.Importado = "Inserindo";
                newDEO.Incubatorio = item.Local;
                newDEO.TipoDEO = "Solicitação Ajuste de Estoque";
                newDEO.GTANum = "";
                newDEO.Lacre = "";
                newDEO.NumIdentificacao = "";
                newDEO.CodItemDEO = 0;
                newDEO.Observacao = "";
                newDEO.TipoOvo = "";
                newDEO.QtdDiferenca = 0;

                listItens.Add(newDEO);
            }

            #endregion

            #region Carrega Sessions Valores

            foreach (var item in listItens)
            {
                Session["qtdAjuste_" + item.Granja + "|" + item.LoteCompleto.ToString() + "|"
                            + item.DataProducao.ToShortDateString()] = "";
            }

            #endregion

            return listItens.OrderBy(o => o.ResponsavelCarreg)
                .ThenBy(t=> t.Granja).ThenBy(t => t.Lote).ThenBy(t => t.DataProducao)
                .ToList();
        }

        public ActionResult CreateAjusteEstoque()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
            Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);

            var listItens = new List<LayoutDiarioExpedicaos>();

            //Session["dataAjusteEstq"] = DateTime.Now;
            Session["dataAjusteEstq"] = null;
            string incubatorio = Session["incubatorioSelecionado"].ToString();

            if (DateTime.Today >= Convert.ToDateTime("15/12/2021") && (incubatorio == "CH" || incubatorio == "NM"))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = "NÃO É POSSÍVEL MAIS CRIAR AJUSTE DE INVENTÁRIO DE OVOS A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO DOS SISTEMA PARA O POULTRY SUITE!!!";
                RefreshListaAjusteEstoque();
                return View("ListaAjusteEstoque");
            }

            Session["ListaItensAjusteEstoque"] = GenerateAjusteEstoqueList(incubatorio);

            Session["operacaoAnaliseEstoque"] = "Create";

            return View("AjusteEstoque");
        }

        public ActionResult EditAjusteEstoque(string numIdentificacao)
        {
            //if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities bd = new HLBAPPEntities();
            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            #region Carrega Sessions Valores

            foreach (var item in listItens)
            {
                Session["qtdAjuste_" + item.Granja + "|" + item.LoteCompleto.ToString() + "|" + item.DataProducao.ToShortDateString()] = item.QtdeConferencia;
            }

            #endregion

            if (Session["incubatorioSelecionado"] == null)
            {
                string incubatorio = listItens.OrderBy(o => o.Granja).FirstOrDefault().Granja;
                Session["incubatorioSelecionado"] = incubatorio;
                Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["incubatorioSelecionado"].ToString(), false, false);
                AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatoriosDestino"]);
                Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatoriosDestino"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            }
            Session["numIdentificacaoSelecionado"] = numIdentificacao;
            Session["dataAjusteEstq"] = listItens.FirstOrDefault().DataHoraCarreg;
            Session["ListaItensAjusteEstoque"] = listItens;

            Session["operacaoAnaliseEstoque"] = "Edit";

            return View("AjusteEstoque");
        }

        public ActionResult ReturnAjusteEstoque(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("AjusteEstoque");
        }

        public ActionResult SaveAjusteEstoque(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime dataHoraAjusteEstq = DateTime.Now;
            if (model["dataAjusteEstq"] != null)
            {
                dataHoraAjusteEstq = Convert.ToDateTime(model["dataAjusteEstq"] + " " + model["horaAjusteEstq"]);
                bool enviarAnalise = Convert.ToBoolean(model["enviarAnalise"].Replace("false,true", "true"));
                string login = Session["login"].ToString();
                string usuario = login.ToUpper();
                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                HLBAPPEntities hlbappLOG = new HLBAPPEntities();
                var lista = (List<MvcAppHylinedoBrasilMobile.Models.LayoutDiarioExpedicaos>)Session["ListaItensAjusteEstoque"];

                foreach (var item in lista)
                {
                    LayoutDiarioExpedicaos deo = bd.LayoutDiarioExpedicaos.Where(w => w.ID == item.ID).FirstOrDefault();
                    if (deo == null) deo = item;

                    var qtdeInformada = Convert.ToInt32(model["qtdAjuste_" + item.Granja + "|" + item.LoteCompleto.ToString() + "|"
                            + item.DataProducao.ToShortDateString()]);
                    var qtdeSistema = Convert.ToInt32(bd.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(w => w.Local == item.Granja && w.LoteCompleto == item.LoteCompleto
                            && w.DataProducao == item.DataProducao)
                        .FirstOrDefault().Qtde);
                    deo.QtdeBandejas = qtdeSistema;
                    deo.QtdeConferencia = qtdeInformada;
                    deo.QtdeOvos = qtdeSistema - qtdeInformada;
                    deo.DataHoraCarreg = dataHoraAjusteEstq;
                    deo.NumIdentificacao = numIdentificacao;
                    if (enviarAnalise)
                        deo.Importado = "Em Análise";
                    deo.ResponsavelReceb = Session["email"].ToString();

                    if (deo.ID == 0) bd.LayoutDiarioExpedicaos.AddObject(deo);
                }

                bd.SaveChanges();

                var listaSalva = bd.LayoutDiarioExpedicaos.Where(w => w.NumIdentificacao == numIdentificacao).ToList();
                var qtdeTotalAjuste = listaSalva.Sum(s => s.QtdeOvos);
                foreach (var item in listaSalva)
                {
                    if (qtdeTotalAjuste == 0) item.Importado = "Conferido";
                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                        "Gera Solicitação Ajuste de Estoque", usuario, item.QtdDiferenca, "", "", item);
                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                }

                bd.SaveChanges();
                hlbappLOG.SaveChanges();

                string msgRetorno = "salva com sucesso!";
                if (enviarAnalise && qtdeTotalAjuste != 0)
                {
                    #region Enviar E-mail para Responsável(is) da Análise do Ajuste

                    string stringChar = "<br />";

                    #region Carrega responsáveis

                    string paraNome = "Depto. de Análise de Ajuste de Estoque";
                    string paraEmail = "jcolavite@hyline.com.br";
                    string copiaPara = "";

                    #endregion

                    #region Carrega Lista de Itens Divergentes

                    #region Carrega Lista de Itens Divergentes no corpo do e-mail

                    string itensDivergentes = "";
                    var listaItensDivergentes = lista.Where(w => w.QtdeOvos != 0).ToList();

                    if (listaItensDivergentes.Count > 0)
                        itensDivergentes =
                            "<table style=\"width: 100%; "
                                + "border-collapse: collapse; "
                                + "text-align: center;\">";

                    itensDivergentes = itensDivergentes
                        + "<tr style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\">"
                            + "<th>"
                                + "Lote"
                            + "</th>"
                            + "<th>"
                                + "Data Produção"
                            + "</th>"
                            + "<th>"
                                + "Qtde. Sistema"
                            + "</th>"
                            + "<th>"
                                + "Qtde. Informada"
                            + "</th>"
                            + "<th>"
                                + "Diferença"
                            + "</th>"
                        + "</tr>";

                    var totalAjuste = 0;

                    foreach (var item in listaItensDivergentes)
                    {
                        var qtdeSistema = Convert.ToInt32(bd.CTRL_LOTE_LOC_ARMAZ_WEB
                            .Where(w => w.Local == item.Granja && w.LoteCompleto == item.LoteCompleto
                                && w.DataProducao == item.DataProducao)
                            .FirstOrDefault().Qtde);

                        itensDivergentes = itensDivergentes
                            + "<tr>"
                                + "<td style=\"padding: 6px; "
                                    + "border: 1px solid #ccc;\">"
                                        + item.LoteCompleto
                                + "</td>"
                                + "<td style=\"padding: 6px; "
                                    + "border: 1px solid #ccc;\">"
                                        + String.Format("{0:dd/MM/yyyy}", item.DataProducao)
                                + "</td>"
                                + "<td style=\"padding: 6px; "
                                    + "border: 1px solid #ccc;\">"
                                        + String.Format("{0:N0}", qtdeSistema)
                                + "</td>"
                                + "<td style=\"padding: 6px; "
                                    + "border: 1px solid #ccc;\">"
                                        + @String.Format("{0:N0}", item.QtdeConferencia)
                                + "</td>"
                                + "<td style=\"padding: 6px; "
                                    + "border: 1px solid #ccc;\">"
                                        + @String.Format("{0:N0}", Math.Abs(item.QtdeOvos))
                                + "</td>"
                            + "</tr>";

                        totalAjuste = totalAjuste + Convert.ToInt32(Math.Abs(item.QtdeOvos));
                    }

                    if (listaItensDivergentes.Count > 0)
                        itensDivergentes = itensDivergentes + "</table>";

                    #endregion

                    #region Gera o E-mail

                    string assunto = "AJUSTE ESTOQUE - "
                        + listaItensDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                        + " - " + listaItensDivergentes.FirstOrDefault().Granja;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "5";

                    string porta = "";
                    if (Request.Url.Port != 80)
                        porta = ":" + Request.Url.Port.ToString();

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "Seguem abaixo os itens divergentes para Ajuste de Estoque solicitado em "
                        + listaItensDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                        + " - " + listaItensDivergentes.FirstOrDefault().Granja
                        + " por " + login
                        + ":" + stringChar + stringChar
                        + itensDivergentes + stringChar + stringChar
                        + "<b>QTDE. TOTAL DE DIFERENÇA: " + String.Format("{0:N0}", Math.Abs(totalAjuste)) + "</b>" + stringChar + stringChar
                        + "Clique no link a seguir para aprovar ou reprovar: "
                        + "<a href='http://" + Request.Url.Host + porta + "/DiarioExpedicao/EditAjusteEstoque?numIdentificacao=" + numIdentificacao
                            + "'>Ajuste de Estoque nº " + numIdentificacao + "</a>"
                        + stringChar + stringChar
                        + "Por favor, analisar para aprovar ou reprovar o ajuste!"
                        + stringChar + stringChar
                        + "SISTEMA WEB";

                    EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, anexos, empresaApolo, "Html");

                    #endregion

                    #endregion

                    #endregion

                    msgRetorno = "salva e enviada para aprovação com sucesso!";
                }

                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Solicitação de Ajuste de Estoque", Session["language"].ToString())
                    + " " + am.GetTextOnLanguage(msgRetorno, Session["language"].ToString());
            }

            RefreshListaAjusteEstoque();
            return View("ListaAjusteEstoque");
        }

        public ActionResult DeleteAjusteEstoque(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            return View();
        }

        public ActionResult DeleteAjusteEstoqueConfirma()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            HLBAPPEntities bd = new HLBAPPEntities();
            HLBAPPEntities hlbappLOG = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
                LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Exclusão da Solicitação Ajuste de Estoque", Session["login"].ToString(), 0, "", "", item);
                hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
            }

            bd.SaveChanges();
            hlbappLOG.SaveChanges();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Solicitação de Ajuste de Estoque", Session["language"].ToString())
                + " " + am.GetTextOnLanguage("excluída com sucesso!", Session["language"].ToString());

            RefreshListaAjusteEstoque();
            return View("ListaAjusteEstoque");
        }

        #endregion

        #region Events Ajuste de Estoque

        public ActionResult AprovaAjusteEstoque(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (!MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController.GetGroup("HLBAPPM-AjusteEstoqueAnalise",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Você não tem acesso para fazer a aprovação!", Session["language"].ToString());
                RefreshListaAjusteEstoque();
                return View("ListaAjusteEstoque");
            }

            if (model["btnAprovaAjusteEstoque"] != null)
            {
                string login = Session["login"].ToString();
                string usuario = login.ToUpper();
                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                HLBAPPEntities hlbappLOG = new HLBAPPEntities();
                var lista = (List<MvcAppHylinedoBrasilMobile.Models.LayoutDiarioExpedicaos>)Session["ListaItensAjusteEstoque"];

                foreach (var item in lista)
                {
                    LayoutDiarioExpedicaos deo = bd.LayoutDiarioExpedicaos.Where(w => w.ID == item.ID).FirstOrDefault();
                    deo.Importado = "Conferido";

                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                        "Solicitação Ajuste de Estoque Aprovada", usuario, item.QtdDiferenca, "", "", item);

                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                }

                bd.SaveChanges();
                hlbappLOG.SaveChanges();

                #region Enviar E-mail para Responsável(is) pelo Ajuste

                string stringChar = "<br />";

                #region Carrega responsáveis

                string paraNome = lista.FirstOrDefault().Usuario.ToUpper();
                string paraEmail = lista.FirstOrDefault().ResponsavelReceb;
                string copiaPara = "";

                #endregion

                #region Carrega Lista de Itens Divergentes

                #region Carrega Lista de Itens Divergentes no corpo do e-mail

                string itensDivergentes = "Não existe divergência!";
                var listaItensDivergentes = lista.Where(w => w.QtdeOvos != 0).ToList();

                if (listaItensDivergentes.Count > 0)
                {
                    itensDivergentes =
                        "<table style=\"width: 100%; "
                            + "border-collapse: collapse; "
                            + "text-align: center;\">";

                    itensDivergentes = itensDivergentes
                        + "<tr style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\">"
                            + "<th>"
                                + "Lote"
                            + "</th>"
                            + "<th>"
                                + "Data Produção"
                            + "</th>"
                            + "<th>"
                                + "Qtde. Sistema"
                            + "</th>"
                            + "<th>"
                                + "Qtde. Informada"
                            + "</th>"
                            + "<th>"
                                + "Diferença"
                            + "</th>"
                        + "</tr>";
                }

                var totalAjuste = 0;

                foreach (var item in listaItensDivergentes)
                {
                    var qtdeSistema = Convert.ToInt32(bd.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(w => w.Local == item.Granja && w.LoteCompleto == item.LoteCompleto
                            && w.DataProducao == item.DataProducao)
                        .FirstOrDefault().Qtde);

                    itensDivergentes = itensDivergentes
                        + "<tr>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + item.LoteCompleto
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + String.Format("{0:dd/MM/yyyy}", item.DataProducao)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + String.Format("{0:N0}", qtdeSistema)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + @String.Format("{0:N0}", item.QtdeConferencia)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + @String.Format("{0:N0}", Math.Abs(item.QtdeOvos))
                            + "</td>"
                        + "</tr>";

                    totalAjuste = totalAjuste + Convert.ToInt32(Math.Abs(item.QtdeOvos));
                }

                if (listaItensDivergentes.Count > 0)
                    itensDivergentes = itensDivergentes + "</table>";

                #endregion

                #region Gera o E-mail

                string assunto = "AJUSTE ESTOQUE APROVADO - " + lista.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss") + " - " + lista.FirstOrDefault().Granja;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                //string porta = "";
                //if (Request.Url.Port != 80)
                //    porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado," + stringChar + stringChar
                    + "O Ajuste de Estoque"
                    + lista.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                    + " - " + lista.FirstOrDefault().Granja
                    + " foi aprovado pelo usuário " + login
                    + ". Segue abaixo o detalhamento do ajuste aprovado:" + stringChar + stringChar
                    + itensDivergentes + stringChar + stringChar
                    + "<b>QTDE. TOTAL DE DIFERENÇA: " + String.Format("{0:N0}", Math.Abs(totalAjuste)) + "</b>" + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "SISTEMA WEB";

                EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, anexos, empresaApolo, "Html");

                #endregion

                #endregion

                #endregion

                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Solicitação de Ajuste de Estoque", Session["language"].ToString())
                    + " " + am.GetTextOnLanguage("aprovada com sucesso!", Session["language"].ToString());
            }

            RefreshListaAjusteEstoque();
            return View("ListaAjusteEstoque");
        }

        public ActionResult ReprovaAjusteEstoque(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (!MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController.GetGroup("HLBAPPM-AjusteEstoqueAnalise",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Você não tem acesso para fazer a reprovação!", Session["language"].ToString());
                RefreshListaAjusteEstoque();
                return View("ListaAjusteEstoque");
            }

            if (model["motivoReprovaAjusteEstoque"] != null)
            {
                string motivo = model["motivoReprovaAjusteEstoque"];
                string login = Session["login"].ToString();
                string usuario = login.ToUpper();
                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                HLBAPPEntities hlbappLOG = new HLBAPPEntities();
                var lista = (List<MvcAppHylinedoBrasilMobile.Models.LayoutDiarioExpedicaos>)Session["ListaItensAjusteEstoque"];

                foreach (var item in lista)
                {
                    LayoutDiarioExpedicaos deo = bd.LayoutDiarioExpedicaos.Where(w => w.ID == item.ID).FirstOrDefault();
                    deo.Importado = "Inserindo";

                    LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now,
                        "Solicitação Ajuste de Estoque Reprovada", usuario, item.QtdDiferenca, motivo, "", item);

                    hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                }

                bd.SaveChanges();
                hlbappLOG.SaveChanges();

                #region Enviar E-mail para Responsável(is) pelo Ajuste

                string stringChar = "<br />";

                #region Carrega responsáveis

                string paraNome = usuario;
                string paraEmail = lista.FirstOrDefault().ResponsavelReceb;
                string copiaPara = "";

                #endregion

                #region Carrega Lista de Itens Divergentes

                #region Carrega Lista de Itens Divergentes no corpo do e-mail

                string itensDivergentes = "";
                var listaItensDivergentes = lista.Where(w => w.QtdeOvos != 0).ToList();

                if (listaItensDivergentes.Count > 0)
                    itensDivergentes =
                        "<table style=\"width: 100%; "
                            + "border-collapse: collapse; "
                            + "text-align: center;\">";

                itensDivergentes = itensDivergentes
                    + "<tr style=\"background: #333; "
                        + "color: white; "
                        + "font-weight: bold; "
                        + "text-align: center;\">"
                        + "<th>"
                            + "Lote"
                        + "</th>"
                        + "<th>"
                            + "Data Produção"
                        + "</th>"
                        + "<th>"
                            + "Qtde. Sistema"
                        + "</th>"
                        + "<th>"
                            + "Qtde. Informada"
                        + "</th>"
                        + "<th>"
                            + "Diferença"
                        + "</th>"
                    + "</tr>";

                var totalAjuste = 0;

                foreach (var item in listaItensDivergentes)
                {
                    var qtdeSistema = Convert.ToInt32(bd.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(w => w.Local == item.Granja && w.LoteCompleto == item.LoteCompleto
                            && w.DataProducao == item.DataProducao)
                        .FirstOrDefault().Qtde);

                    itensDivergentes = itensDivergentes
                        + "<tr>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + item.LoteCompleto
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + String.Format("{0:dd/MM/yyyy}", item.DataProducao)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + String.Format("{0:N0}", qtdeSistema)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + @String.Format("{0:N0}", item.QtdeConferencia)
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + @String.Format("{0:N0}", Math.Abs(item.QtdeOvos))
                            + "</td>"
                        + "</tr>";

                    totalAjuste = totalAjuste + Convert.ToInt32(Math.Abs(item.QtdeOvos));
                }

                if (listaItensDivergentes.Count > 0)
                    itensDivergentes = itensDivergentes + "</table>";

                #endregion

                #region Gera o E-mail

                string assunto = "AJUSTE ESTOQUE REPROVADO - "
                    + listaItensDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                    + " - " + listaItensDivergentes.FirstOrDefault().Granja;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                //string porta = "";
                //if (Request.Url.Port != 80)
                //    porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado," + stringChar + stringChar
                    + "O Ajuste de Estoque "
                    + listaItensDivergentes.FirstOrDefault().DataHoraCarreg.ToString("dd/MM/yy HH:mm:ss")
                    + " - " + listaItensDivergentes.FirstOrDefault().Granja
                    + " foi reprovado pelo usuário " + login + " pelo seguinte motivo:"
                    + stringChar + stringChar + "\"" + "<i>" + motivo + "</i>" + "\"" + stringChar + stringChar
                    + "Segue abaixo o detalhamento do ajuste reprovado:" + stringChar + stringChar
                    + itensDivergentes + stringChar + stringChar
                    + "<b>QTDE. TOTAL DE DIFERENÇA: " + String.Format("{0:N0}", Math.Abs(totalAjuste)) + "</b>" + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Por favor, realizar as mudanças solicitadas e enviar para aprovação novamente!" + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail(corpoEmail, assunto, paraNome, paraEmail, copiaPara, anexos, empresaApolo, "Html");

                #endregion

                #endregion

                #endregion

                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Solicitação de Ajuste de Estoque", Session["language"].ToString())
                    + " " + am.GetTextOnLanguage("reprovada com sucesso!", Session["language"].ToString());
            }

            RefreshListaAjusteEstoque();
            return View("ListaAjusteEstoque");
        }

        #endregion

        #endregion

        #region Transferência de Ovos Classificados

        #region Lista Transferência de Ovos Classificados

        public List<LayoutDiarioExpedicaos> CarregaTransferenciaOvosClassificados(string incubatorio, DateTime dataInicial,
            DateTime dataFinal)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            DateTime dataHoraIni = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            DateTime dataHoraFim = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            var lista = bd.LayoutDiarioExpedicaos
                .Where(w => w.TipoDEO == "Transf. Ovos Classificados"
                    && w.DataHoraCarreg >= dataHoraIni
                    && w.DataHoraCarreg <= dataHoraFim
                    && (bd.TIPO_CLASSFICACAO_OVO
                        .Any(a => a.CodigoTipo == w.Granja && a.AproveitamentoOvo == "Incubável" && a.Origem == "Interna" && a.Unidade == incubatorio)
                        || w.Granja == incubatorio))
                .ToList();

            return lista;
        }

        public ActionResult FiltraListaTransferenciasOvosClassificados(string incubatorio,
            DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioSelecionado"] = incubatorio;
            Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);
            Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            Session["dataInicial"] = dataInicial.ToShortDateString();
            Session["dataFinal"] = dataFinal.ToShortDateString();
            dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            Session["ListaTransferenciasOvosClassificados"] = CarregaTransferenciaOvosClassificados(incubatorio, dataInicial, dataFinal);

            return View("ListaTransferenciasOvosClassificados");
        }

        public void RefreshListaTransferenciasOvosClassificados()
        {
            string incubatorio = "";
            if (Session["incubatorioSelecionado"] != null)
                incubatorio = Session["incubatorioSelecionado"].ToString();
            else
            {
                incubatorio = ((List<SelectListItem>)Session["ListaIncubatorios"]).FirstOrDefault().Value;
                Session["incubatorioSelecionado"] = incubatorio;
                Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
                Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            }
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            Session["ListaTransferenciasOvosClassificados"] = CarregaTransferenciaOvosClassificados(incubatorio, dataInicial, dataFinal);
        }

        public ActionResult ListaTransferenciasOvosClassificados()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["ListaIncubatorios"] = CarregaListaIncubatoriosCO("", false, false);
            RefreshListaTransferenciasOvosClassificados();

            return View();
        }

        #endregion

        #region CRUD Transferência de Ovos Classificados

        public ActionResult CreateTransferenciaOvosClassificados()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
            Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);

            var listItens = new List<LayoutDiarioExpedicaos>();

            Session["dataTransferencia"] = DateTime.Now;
            Session["reclassificacao"] = false;
            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["incubatorioSelecionado"].ToString(), true, false);
            List<SelectListItem> items = (List<SelectListItem>)Session["ListaIncubatoriosDestino"];
            if (items.Count > 0) Session["incubatorioDestinoSelecionado"] = items[0].Value;
            Session["ListaItensTransferenciaOvosClassificados"] = listItens;

            return View("TransferenciaOvosClassificados");
        }

        public ActionResult EditTransferenciaOvosClassificados(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["ListaIncubatoriosDestino"] = CarregaListaIncubatoriosCO(Session["incubatorioSelecionado"].ToString(), true, false);
            AtualizaIncubatorioSelecionado(listItens.FirstOrDefault().Incubatorio);
            Session["incubatorioDestinoSelecionado"] = listItens.FirstOrDefault().Incubatorio;
            Session["numIdentificacaoSelecionado"] = numIdentificacao;
            Session["dataTransferencia"] = listItens.FirstOrDefault().DataHoraCarreg;
            Session["reclassificacao"] = listItens.FirstOrDefault().ResponsavelCarreg; 
            Session["ListaItensTransferenciaOvosClassificados"] = listItens;

            return View("TransferenciaOvosClassificados");
        }

        public ActionResult ReturnTransferenciaOvosClassificados(string incubatorio, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("TransferenciaOvosClassificados");
        }

        public ActionResult SaveTransferenciaOvosClassificados(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime dataTransferencia = DateTime.Now;
            if (model["dataTransferencia"] != null)
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                dataTransferencia = Convert.ToDateTime(model["dataTransferencia"]);
                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();
                string reclassificacao = Session["reclassificacao"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                var listItens = bd.LayoutDiarioExpedicaos
                    .Where(w => w.NumIdentificacao == numIdentificacao)
                    .ToList();

                foreach (var item in listItens)
                {
                    item.DataHoraCarreg = dataTransferencia;
                    item.ResponsavelCarreg = reclassificacao;

                    string to = item.Incubatorio;
                    if (item.Granja != incubatorio)
                    {
                        to = item.Granja + "-" + item.Incubatorio;
                        if (Convert.ToBoolean(reclassificacao)) to = item.Incubatorio;
                    }

                    item.TipoOvo = to;
                    item.Importado = "Conferido";
                }

                bd.SaveChanges();

                if (listItens.Count > 0)
                {
                    ViewBag.ClasseMsg = "msgSucesso";
                    ViewBag.Erro = am.GetTextOnLanguage("Transferência de Ovos Classificados", Session["language"].ToString())
                        + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                    RefreshListaTransferenciasOvosClassificados();
                    return View("ListaTransferenciasOvosClassificados");
                }
                else
                {
                    ViewBag.ClasseMsg = "msgWarning";
                    ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhuma transferência", Session["language"].ToString())
                        + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                    return View("TransferenciaOvosClassificadossificacaoOvo");
                }
            }

            RefreshListaTransferenciasOvosClassificados();
            return View("ListaTransferenciasOvosClassificados");
        }

        public ActionResult DeleteTransferenciaOvosClassificados(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            return View();
        }

        public ActionResult DeleteTransferenciaOvosClassificadosConfirma()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Transferência de Ovos Classificados", Session["language"].ToString())
                + " " + am.GetTextOnLanguage("excluída com sucesso!", Session["language"].ToString());

            RefreshListaTransferenciasOvosClassificados();
            return View("ListaTransferenciasOvosClassificados");
        }

        #endregion

        #region CRUD Item da Transferência dos Ovos Classificados

        public ActionResult CreateTransferenciaOvosClassificadosItem()
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = new List<LayoutDiarioExpedicaos>();

            CarregaTransferenciaOvosClassificadosItem(listLote);

            return View("TransferenciaOvosClassificadosItem");
        }

        public ActionResult EditTransferenciaOvosClassificadosItem(string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            CarregaTransferenciaOvosClassificadosItem(listLote);

            return View("TransferenciaOvosClassificadosItem");
        }

        public void CarregaTransferenciaOvosClassificadosItem(List<LayoutDiarioExpedicaos> listLote)
        {
            if (listLote.Count > 0)
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var incubatorio = Session["incubatorioSelecionado"].ToString();
                var item = listLote.FirstOrDefault();
                var itemSemClassificar = listLote.Where(w => w.Granja == incubatorio).Sum(s => s.QtdeOvos);
                var itemSemClassificarSemSalvar = hlbapp.LayoutDiarioExpedicaos
                    .Where(w => w.LoteCompleto == item.LoteCompleto
                        && w.DataProducao == item.DataProducao
                        && w.TipoDEO == "Transf. Ovos Classificados"
                        && w.Granja == incubatorio
                        && w.Importado != "Conferido").ToList();
                int qtdeSemClassificarSemSalvar = 0;
                if (itemSemClassificarSemSalvar.Count > 0) qtdeSemClassificarSemSalvar = Convert.ToInt32(itemSemClassificarSemSalvar.Sum(s => s.QtdeOvos));

                Session["DDLNucleo"] = CarregaListaNucleosFLIP(item.Granja);
                AtualizaDDL(item.Nucleo, (List<SelectListItem>)Session["DDLNucleo"]);
                Session["DDLLotes"] = CarregaLotesFLIP(item.Granja, item.Nucleo);
                AtualizaDDL(item.Lote, (List<SelectListItem>)Session["DDLLotes"]);
                Session["DDLGalpoes"] = CarregaDDLGalpoes(item.Lote);
                AtualizaDDL(item.Galpao, (List<SelectListItem>)Session["DDLGalpoes"]);
                Session["loteCompleto"] = item.LoteCompleto;
                Session["DataProducao"] = item.DataProducao;
                Session["QtdeTotal_" + incubatorio] = (RetornaSaldo(incubatorio, item.LoteCompleto, item.DataProducao) - qtdeSemClassificarSemSalvar);
                Session["idadeLote"] = item.Idade;
                Session[incubatorio] = itemSemClassificar;

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    var itemSemSalvar = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.LoteCompleto == item.LoteCompleto
                            && w.DataProducao == item.DataProducao
                            && w.TipoDEO == "Transf. Ovos Classificados"
                            && w.Granja == tipoOvo.CodigoTipo
                            && w.Importado != "Conferido").ToList();

                    int qtdeSemSalvar = 0;
                    if (itemSemSalvar.Count > 0) qtdeSemSalvar = Convert.ToInt32(itemSemSalvar.Sum(s => s.QtdeOvos));

                    var itemLote = listLote.Where(w => w.Granja == tipoOvo.CodigoTipo)
                        .FirstOrDefault();
                    if (itemLote != null)
                    {
                        Session[tipoOvo.CodigoTipo] = itemLote.QtdeOvos;
                        Session["QtdeTotal_" + tipoOvo.CodigoTipo] = (RetornaSaldo(tipoOvo.CodigoTipo, item.LoteCompleto, item.DataProducao) - qtdeSemSalvar);
                    }
                    else
                    {
                        Session[tipoOvo.CodigoTipo] = 0;
                        Session["existeSaldo" + tipoOvo.CodigoTipo] = false;
                    }
                }
            }
            else
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                Session["DDLNucleo"] = CarregaListaNucleosFLIP(incubatorio);
                Session["DDLLotes"] = new List<SelectListItem>();
                Session["DDLGalpoes"] = new List<SelectListItem>();
                Session["loteCompleto"] = "";
                Session["DataProducao"] = DateTime.Today;
                Session["QtdeTotal_" + incubatorio] = 0;
                Session["idadeLote"] = "";
                Session[incubatorio] = 0;

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio
                         && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    Session["QtdeTotal_" + tipoOvo.CodigoTipo] = 0;
                    Session[tipoOvo.CodigoTipo] = 0;
                }
            }
        }

        [HttpPost]
        public ActionResult SaveTransferenciaOvosClassificadosItem(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();

            #region Load General Variables

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();
            string incubatorio = Session["incubatorioSelecionado"].ToString();
            DateTime dataClassificacao = Convert.ToDateTime(Session["dataTransferencia"]);
            string incubatorioDestino = Session["incubatorioDestinoSelecionado"].ToString();
            string reclassificacao = Session["reclassificacao"].ToString();
            string nucleo = model["Nucleo"];
            string lote = model["Lote"];
            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
            var loteSelecionado = listaLotes.Where(s => s.NumeroLote == lote).FirstOrDefault();
            string galpao = model["Galpao"];
            string loteCompleto = model["loteCompleto"];
            if (model["loteCompleto"] == "") loteCompleto = loteSelecionado.LoteCompleto;
            string linhagem = model["linhagem"];
            if (model["linhagem"] == "") linhagem = loteSelecionado.Linhagem;
            DateTime dataProducao = Convert.ToDateTime(model["dataProducaoCO"]);
            int idade = Convert.ToInt32(model["idade"]);
            int qtdGerada = 0;

            #endregion

            #region Qtde Sem Classificar

            int qtdTransferidaSC = Convert.ToInt32(model[incubatorio]);

            var itemSC = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == loteCompleto
                    && w.DataProducao == dataProducao
                    && w.TipoDEO == "Transf. Ovos Classificados"
                    && w.Granja == incubatorio
                    && w.Incubatorio == incubatorioDestino)
                .FirstOrDefault();

            if (itemSC == null && qtdTransferidaSC > 0)
            {
                itemSC = new LayoutDiarioExpedicaos();
                itemSC.Granja = incubatorio;
                itemSC.Incubatorio = incubatorioDestino;
                itemSC.NumIdentificacao = numIdentificacao;
                itemSC.DataHoraCarreg = dataClassificacao;
                itemSC.Nucleo = nucleo;
                itemSC.Linhagem = linhagem;
                itemSC.LoteCompleto = loteCompleto;
                itemSC.Lote = lote;
                itemSC.Galpao = galpao;
                itemSC.DataProducao = dataProducao;
                itemSC.Idade = idade;
                itemSC.NumeroReferencia = dataProducao.DayOfYear.ToString();
                itemSC.TipoDEO = "Transf. Ovos Classificados";
                itemSC.TipoOvo = incubatorioDestino;
                itemSC.NFNum = "";
                itemSC.Importado = "Sim";
                itemSC.Usuario = Session["login"].ToString();
                itemSC.DataHora = DateTime.Now;
                itemSC.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                itemSC.ResponsavelCarreg = reclassificacao;
                itemSC.ResponsavelReceb = incubatorio;
                itemSC.GTANum = "";
                itemSC.Lacre = "";
                itemSC.Observacao = "Gerado automaticamente pela Transferência de Ovos Classificados - Sem Classificação";
                itemSC.QtdDiferenca = 0;
                itemSC.QtdeConferencia = 0;

                #region LOG

                HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                if (itemSC != null)
                {
                    if (itemSC.ID == 0)
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Inserido",
                            itemSC.Usuario, 0, "", "", itemSC);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                    else
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Alterado",
                            itemSC.Usuario, 0, "", "", itemSC);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                }

                hlbappLOG.SaveChanges();

                #endregion
            }

            if (itemSC != null)
            {
                qtdGerada++;

                if (qtdTransferidaSC == 0 && itemSC.ID > 0)
                    bd.LayoutDiarioExpedicaos.DeleteObject(itemSC);
                else
                {
                    itemSC.QtdeOvos = qtdTransferidaSC;
                    itemSC.Importado = "Sim";
                }

                if (itemSC.ID == 0 && qtdTransferidaSC > 0) bd.LayoutDiarioExpedicaos.AddObject(itemSC);
            }

            #endregion

            #region Qtde Classificado

            var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                .Where(w => w.Unidade == incubatorio && w.AproveitamentoOvo == "Incubável" && w.Origem == "Interna")
                .ToList();

            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                int qtdTransferida = Convert.ToInt32(model[tipoOvo.CodigoTipo]);

                string to = tipoOvo.CodigoTipo + "-" + incubatorioDestino;
                if (Convert.ToBoolean(reclassificacao)) to = incubatorioDestino;

                var item = bd.LayoutDiarioExpedicaos
                    .Where(w => w.NumIdentificacao == numIdentificacao
                        && w.LoteCompleto == loteCompleto
                        && w.DataProducao == dataProducao
                        && w.TipoDEO == "Transf. Ovos Classificados"
                        && w.Granja == tipoOvo.CodigoTipo)
                    .FirstOrDefault();

                if (item == null && qtdTransferida > 0)
                {
                    item = new LayoutDiarioExpedicaos();
                    item.Granja = tipoOvo.CodigoTipo;
                    item.Incubatorio = incubatorioDestino;
                    item.NumIdentificacao = numIdentificacao;
                    item.DataHoraCarreg = dataClassificacao;
                    item.Nucleo = nucleo;
                    item.Linhagem = linhagem;
                    item.LoteCompleto = loteCompleto;
                    item.Lote = lote;
                    item.Galpao = galpao;
                    item.DataProducao = dataProducao;
                    item.Idade = idade;
                    item.NumeroReferencia = dataProducao.DayOfYear.ToString();
                    item.TipoDEO = "Transf. Ovos Classificados";
                    item.TipoOvo = to;
                    item.NFNum = "";
                    item.Importado = "Sim";
                    item.Usuario = Session["login"].ToString();
                    item.DataHora = DateTime.Now;
                    item.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                    item.ResponsavelCarreg = reclassificacao;
                    item.ResponsavelReceb = incubatorio;
                    item.GTANum = "";
                    item.Lacre = "";
                    item.Observacao = "Gerado automaticamente pela Transferência de Ovos Classificados - Tipo do Ovo: "
                        + tipoOvo.DescricaoTipo;
                    item.QtdDiferenca = 0;
                    item.QtdeConferencia = 0;
                }

                if (item != null)
                {
                    qtdGerada++;

                    if (qtdTransferida == 0 && item.ID > 0)
                        bd.LayoutDiarioExpedicaos.DeleteObject(item);
                    else
                    {
                        item.QtdeOvos = qtdTransferida;
                        item.TipoOvo = to;
                        item.Importado = "Sim";
                    }

                    if (item.ID == 0 && qtdTransferida > 0) bd.LayoutDiarioExpedicaos.AddObject(item);
                }

                #region LOG

                HLBAPPEntities hlbappLOG = new HLBAPPEntities();

                if (item != null)
                {
                    if (item.ID == 0)
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Inserido",
                            item.Usuario, 0, "", "", item);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                    else
                    {
                        LOG_LayoutDiarioExpedicaos log = InsereLOG(DateTime.Now, "Item Alterado",
                            item.Usuario, 0, "", "", item);
                        hlbappLOG.LOG_LayoutDiarioExpedicaos.AddObject(log);
                    }
                }

                hlbappLOG.SaveChanges();

                #endregion
            }

            #endregion

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["ListaItensTransferenciaOvosClassificados"] = listItens;

            if (qtdGerada > 0)
            {
                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Transferência de Ovos Classificados", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                return View("TransferenciaOvosClassificados");
            }
            else
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhuma transferência", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                return View("TransferenciaOvosClassificadosItem");
            }
        }

        public ActionResult DeleteTransferenciaOvosClassificadosItem(string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Transferência de Ovos Classificados", Session["language"].ToString()) + " - "
                + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                + lote + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                + dataProducao.ToShortDateString()
                + " " + am.GetTextOnLanguage("excluída com sucesso!", Session["language"].ToString());

            Session["ListaItensTransferenciaOvosClassificados"] = listItens;

            return View("TransferenciaOvosClassificados");
        }

        #endregion

        #endregion

        #region Descarte de Ovos Classificados para Comércio

        #region Lista Descarte de Ovos Classificados para Comércio

        public List<LayoutDiarioExpedicaos> CarregaDescarteOvosClassificadosParaComercio(string incubatorio, DateTime dataInicial,
            DateTime dataFinal)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            DateTime dataHoraIni = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            DateTime dataHoraFim = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            var lista = bd.LayoutDiarioExpedicaos
                .Where(w => w.TipoDEO == "Ovos Classfic. p/ Comércio"
                    && w.DataHoraCarreg >= dataHoraIni
                    && w.DataHoraCarreg <= dataHoraFim
                    && (bd.TIPO_CLASSFICACAO_OVO
                        .Any(a => a.CodigoTipo == w.Granja && a.AproveitamentoOvo == "Incubável" && a.Origem == "Interna" && a.Unidade == incubatorio)
                        || w.Granja == incubatorio))
                .ToList();

            return lista;
        }

        public ActionResult FiltraListaDescarteOvosClassificadosParaComercio(string incubatorio,
            DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["incubatorioSelecionado"] = incubatorio;
            Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);
            Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            Session["dataInicial"] = dataInicial.ToShortDateString();
            Session["dataFinal"] = dataFinal.ToShortDateString();
            dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            Session["ListaDescarteOvosClassificadosParaComercio"] = CarregaDescarteOvosClassificadosParaComercio(incubatorio, dataInicial, dataFinal);

            return View("ListaDescarteOvosClassificadosParaComercio");
        }

        public void RefreshListaDescarteOvosClassificadosParaComercio()
        {
            string incubatorio = "";
            if (Session["incubatorioSelecionado"] != null)
                incubatorio = Session["incubatorioSelecionado"].ToString();
            else
            {
                incubatorio = ((List<SelectListItem>)Session["ListaIncubatorios"]).FirstOrDefault().Value;
                Session["incubatorioSelecionado"] = incubatorio;
                Session["tipoClassificacaoOvos"] = GetFieldValueHatchCodeTable(incubatorio, "CLAS_EGG");
                Session["incubatorioSelecionadoNome"] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == incubatorio).FirstOrDefault().Text;
            }
            AtualizaDDL(incubatorio, (List<SelectListItem>)Session["ListaIncubatorios"]);

            DateTime dataInicial;
            DateTime dataFinal;

            if (Session["dataInicial"] == null)
            {
                Session["dataInicial"] = DateTime.Today.ToShortDateString();
                Session["dataFinal"] = DateTime.Today.ToShortDateString();
                dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString() + " 23:59:59");
            }
            else
            {
                dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString() + " 00:00:00");
                dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString() + " 23:59:59");
            }

            Session["ListaDescarteOvosClassificadosParaComercio"] = CarregaDescarteOvosClassificadosParaComercio(incubatorio, dataInicial, dataFinal);
        }

        public ActionResult ListaDescarteOvosClassificadosParaComercio()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["ListaIncubatorios"] = CarregaListaIncubatoriosCO("", false, false);
            RefreshListaDescarteOvosClassificadosParaComercio();

            return View();
        }

        #endregion

        #region CRUD Descarte de Ovos Classificados para Comércio

        public ActionResult CreateDescarteOvosClassificadosParaComercio()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            System.Data.Objects.ObjectParameter numero =
                new System.Data.Objects.ObjectParameter("codigo", typeof(global::System.String));
            apoloService.gerar_codigo("1", "LayoutDiarioExpedicaos", numero);
            Session["numIdentificacaoSelecionado"] = Convert.ToInt32(numero.Value);

            var listItens = new List<LayoutDiarioExpedicaos>();

            Session["dataDescarte"] = DateTime.Now;
            Session["ListaItensDescarteOvosClassificadosParaComercio"] = listItens;

            return View("DescarteOvosClassificadosParaComercio");
        }

        public ActionResult EditDescarteOvosClassificadosParaComercio(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["numIdentificacaoSelecionado"] = numIdentificacao;
            Session["dataDescarte"] = listItens.FirstOrDefault().DataHoraCarreg;
            Session["ListaItensDescarteOvosClassificadosParaComercio"] = listItens;

            return View("DescarteOvosClassificadosParaComercio");
        }

        public ActionResult ReturnDescarteOvosClassificadosParaComercio(string incubatorio, string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View("DescarteOvosClassificadosParaComercio");
        }

        public ActionResult SaveDescarteOvosClassificadosParaComercio(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            DateTime dataDescarte = DateTime.Now;
            if (model["dataDescarte"] != null)
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                dataDescarte = Convert.ToDateTime(model["dataDescarte"]);
                string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

                HLBAPPEntities bd = new HLBAPPEntities();
                var listItens = bd.LayoutDiarioExpedicaos
                    .Where(w => w.NumIdentificacao == numIdentificacao)
                    .ToList();

                foreach (var item in listItens)
                {
                    item.DataHoraCarreg = dataDescarte;
                    item.Importado = "Conferido";
                }

                bd.SaveChanges();

                if (listItens.Count > 0)
                {
                    ViewBag.ClasseMsg = "msgSucesso";
                    ViewBag.Erro = am.GetTextOnLanguage("Descarte de Ovos Classificados para Comércio", Session["language"].ToString())
                        + " " + am.GetTextOnLanguage("salva com sucesso!", Session["language"].ToString());
                    RefreshListaDescarteOvosClassificadosParaComercio();
                    return View("ListaDescarteOvosClassificadosParaComercio");
                }
                else
                {
                    ViewBag.ClasseMsg = "msgWarning";
                    ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhum descarte", Session["language"].ToString())
                        + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                    return View("DescarteOvosClassificadosParaComercio");
                }
            }

            RefreshListaDescarteOvosClassificadosParaComercio();
            return View("ListaDescarteOvosClassificadosParaComercio");
        }

        public ActionResult DeleteDescarteOvosClassificadosParaComercio(string numIdentificacao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["numIdentificacaoSelecionado"] = numIdentificacao;

            return View();
        }

        public ActionResult DeleteDescarteOvosClassificadosParaComercioConfirma()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Descarte de Ovos Classificados para Comércio", Session["language"].ToString())
                + " " + am.GetTextOnLanguage("excluído com sucesso!", Session["language"].ToString());

            RefreshListaDescarteOvosClassificadosParaComercio();
            return View("ListaDescarteOvosClassificadosParaComercio");
        }

        #endregion

        #region CRUD Item do Descarte de Ovos Classificados para Comércio

        public ActionResult CreateDescarteOvosClassificadosParaComercioItem()
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = new List<LayoutDiarioExpedicaos>();

            CarregaDescarteOvosClassificadosParaComercioItem(listLote);

            return View("DescarteOvosClassificadosParaComercioItem");
        }

        public ActionResult EditDescarteOvosClassificadosParaComercioItem(string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            HLBAPPEntities bd = new HLBAPPEntities();
            var listLote = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            CarregaDescarteOvosClassificadosParaComercioItem(listLote);

            return View("DescarteOvosClassificadosParaComercioItem");
        }

        public void CarregaDescarteOvosClassificadosParaComercioItem(List<LayoutDiarioExpedicaos> listLote)
        {
            if (listLote.Count > 0)
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var incubatorio = Session["incubatorioSelecionado"].ToString();
                var item = listLote.FirstOrDefault();
                var itemSemClassificar = listLote.Where(w => w.Granja == incubatorio).Sum(s => s.QtdeOvos);
                var itemSemClassificarSemSalvar = hlbapp.LayoutDiarioExpedicaos
                    .Where(w => w.LoteCompleto == item.LoteCompleto
                        && w.DataProducao == item.DataProducao
                        && w.TipoDEO == "Ovos Classfic. p/ Comércio"
                        && w.Granja == incubatorio
                        && w.Importado != "Conferido").ToList();

                Session["DDLNucleo"] = CarregaListaNucleosFLIP(incubatorio);
                AtualizaDDL(item.Nucleo, (List<SelectListItem>)Session["DDLNucleo"]);
                Session["DDLLotes"] = CarregaLotesFLIP(incubatorio, item.Nucleo);
                AtualizaDDL(item.Lote, (List<SelectListItem>)Session["DDLLotes"]);
                Session["DDLGalpoes"] = CarregaDDLGalpoes(item.Lote);
                AtualizaDDL(item.Galpao, (List<SelectListItem>)Session["DDLGalpoes"]);
                Session["loteCompleto"] = item.LoteCompleto;
                Session["DataProducao"] = item.DataProducao;
                Session["idadeLote"] = item.Idade;
                
                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    var itemSemSalvar = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.LoteCompleto == item.LoteCompleto
                            && w.DataProducao == item.DataProducao
                            && w.TipoDEO == "Ovos Classfic. p/ Comércio"
                            && w.Granja == tipoOvo.CodigoTipo
                            && w.Importado != "Conferido").ToList();

                    int qtdeSemSalvar = 0;
                    if (itemSemSalvar.Count > 0) qtdeSemSalvar = Convert.ToInt32(itemSemSalvar.Sum(s => s.QtdeOvos));

                    var itemLote = listLote.Where(w => w.Granja == tipoOvo.CodigoTipo)
                        .FirstOrDefault();
                    if (itemLote != null)
                    {
                        Session[tipoOvo.CodigoTipo] = itemLote.QtdeOvos;
                        Session["QtdeTotal_" + tipoOvo.CodigoTipo] = (RetornaSaldo(tipoOvo.CodigoTipo, item.LoteCompleto, item.DataProducao) - qtdeSemSalvar);
                    }
                    else
                    {
                        Session[tipoOvo.CodigoTipo] = 0;
                        Session["existeSaldo" + tipoOvo.CodigoTipo] = false;
                    }
                }
            }
            else
            {
                string incubatorio = Session["incubatorioSelecionado"].ToString();
                Session["DDLNucleo"] = CarregaListaNucleosFLIP(incubatorio);
                Session["DDLLotes"] = new List<SelectListItem>();
                Session["DDLGalpoes"] = new List<SelectListItem>();
                Session["loteCompleto"] = "";
                Session["DataProducao"] = DateTime.Today;
                Session["idadeLote"] = "";

                HLBAPPEntities bd = new HLBAPPEntities();
                var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == incubatorio && w.Origem == "Interna")
                    .ToList();

                foreach (var tipoOvo in listaTipoClassificacaoOvo)
                {
                    Session["QtdeTotal_" + tipoOvo.CodigoTipo] = 0;
                    Session[tipoOvo.CodigoTipo] = 0;
                }
            }
        }

        [HttpPost]
        public ActionResult SaveDescarteOvosClassificadosParaComercioItem(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();

            #region Load General Variables

            string numIdentificacao = Session["numIdentificacaoSelecionado"].ToString();
            string incubatorio = Session["incubatorioSelecionado"].ToString();
            DateTime dataDescarte = Convert.ToDateTime(Session["dataDescarte"]);
            string nucleo = model["Nucleo"];
            string lote = model["Lote"];
            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
            var loteSelecionado = listaLotes.Where(s => s.NumeroLote == lote).FirstOrDefault();
            string galpao = model["Galpao"];
            string loteCompleto = model["loteCompleto"];
            if (model["loteCompleto"] == "") loteCompleto = loteSelecionado.LoteCompleto;
            string linhagem = model["linhagem"];
            if (model["linhagem"] == "") linhagem = loteSelecionado.Linhagem;
            DateTime dataProducao = Convert.ToDateTime(model["dataProducaoCO"]);
            int idade = Convert.ToInt32(model["idade"]);
            int qtdGerada = 0;

            #endregion

            #region Qtde Classificado

            var listaTipoClassificacaoOvo = bd.TIPO_CLASSFICACAO_OVO
                .Where(w => w.Unidade == incubatorio && w.AproveitamentoOvo == "Incubável" && w.Origem == "Interna")
                .ToList();

            foreach (var tipoOvo in listaTipoClassificacaoOvo)
            {
                int qtdDescartada = Convert.ToInt32(model[tipoOvo.CodigoTipo]);

                var item = bd.LayoutDiarioExpedicaos
                    .Where(w => w.NumIdentificacao == numIdentificacao
                        && w.LoteCompleto == loteCompleto
                        && w.DataProducao == dataProducao
                        && w.TipoDEO == "Ovos Classfic. p/ Comércio"
                        && w.Granja == tipoOvo.CodigoTipo)
                    .FirstOrDefault();

                if (item == null && qtdDescartada > 0)
                {
                    item = new LayoutDiarioExpedicaos();
                    item.Granja = tipoOvo.CodigoTipo;
                    item.Incubatorio = incubatorio + "C";
                    item.NumIdentificacao = numIdentificacao;
                    item.DataHoraCarreg = dataDescarte;
                    item.Nucleo = nucleo;
                    item.Linhagem = linhagem;
                    item.LoteCompleto = loteCompleto;
                    item.Lote = lote;
                    item.Galpao = galpao;
                    item.DataProducao = dataProducao;
                    item.Idade = idade;
                    item.NumeroReferencia = dataProducao.DayOfYear.ToString();
                    item.TipoDEO = "Ovos Classfic. p/ Comércio";
                    item.TipoOvo = "";
                    item.NFNum = "";
                    item.Importado = "Sim";
                    item.Usuario = Session["login"].ToString();
                    item.DataHora = DateTime.Now;
                    item.DataHoraRecebInc = Convert.ToDateTime("01/01/1988");
                    item.ResponsavelCarreg = "";
                    item.ResponsavelReceb = incubatorio;
                    item.GTANum = "";
                    item.Lacre = "";
                    item.Observacao = "Gerado automaticamente pelo Descarte de Ovos Classificados para Comércio - Tipo do Ovo: "
                        + tipoOvo.DescricaoTipo;
                    item.QtdDiferenca = 0;
                    item.QtdeConferencia = 0;
                }

                if (item != null)
                {
                    qtdGerada++;

                    if (qtdDescartada == 0 && item.ID > 0)
                        bd.LayoutDiarioExpedicaos.DeleteObject(item);
                    else
                    {
                        item.QtdeOvos = qtdDescartada;
                        item.Importado = "Sim";
                    }

                    if (item.ID == 0 && qtdDescartada > 0) bd.LayoutDiarioExpedicaos.AddObject(item);
                }
            }

            #endregion

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            Session["ListaItensDescarteOvosClassificadosParaComercio"] = listItens;

            if (qtdGerada > 0)
            {
                ViewBag.ClasseMsg = "msgSucesso";
                ViewBag.Erro = am.GetTextOnLanguage("Descarte de Ovos Classificados para Comércio", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + " " + am.GetTextOnLanguage("salvo com sucesso!", Session["language"].ToString());
                return View("DescarteOvosClassificadosParaComercio");
            }
            else
            {
                ViewBag.ClasseMsg = "msgWarning";
                ViewBag.Erro = am.GetTextOnLanguage("Não foi realizada nenhum descarte", Session["language"].ToString()) + " - "
                    + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                    + loteCompleto + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                    + dataProducao.ToShortDateString()
                    + "! " + am.GetTextOnLanguage("Verifique!", Session["language"].ToString());
                return View("DescarteOvosClassificadosParaComercioItem");
            }
        }

        public ActionResult DeleteDescarteOvosClassificadosParaComercioItem(string numIdentificacao,
            string lote, DateTime dataProducao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities bd = new HLBAPPEntities();
            var listaDelete = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao
                    && w.LoteCompleto == lote
                    && w.DataProducao == dataProducao)
                .ToList();

            foreach (var item in listaDelete)
            {
                bd.LayoutDiarioExpedicaos.DeleteObject(item);
            }

            bd.SaveChanges();

            var listItens = bd.LayoutDiarioExpedicaos
                .Where(w => w.NumIdentificacao == numIdentificacao)
                .ToList();

            ViewBag.ClasseMsg = "msgSucesso";
            ViewBag.Erro = am.GetTextOnLanguage("Descarte de Ovos Classificados para Comércio", Session["language"].ToString()) + " - "
                + am.GetTextOnLanguage("Lote", Session["language"].ToString()) + " "
                + lote + " - " + am.GetTextOnLanguage("Data de Produção", Session["language"].ToString()) + " "
                + dataProducao.ToShortDateString()
                + " " + am.GetTextOnLanguage("excluído com sucesso!", Session["language"].ToString());

            Session["ListaItensDescarteOvosClassificadosParaComercio"] = listItens;

            return View("DescarteOvosClassificadosParaComercio");
        }

        #endregion

        #endregion
    }
}