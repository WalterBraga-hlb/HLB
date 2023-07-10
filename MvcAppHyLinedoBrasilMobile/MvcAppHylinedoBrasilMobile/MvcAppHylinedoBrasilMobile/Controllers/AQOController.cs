using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters;
using System.Data.Objects;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class AQOController : Controller
    {
        #region Menu

        public ActionResult MenuAQO()
        {
            return View();
        }

        #endregion

        #region Manutenção

        #region AQO

        #region List Methods

        public List<Analise_Qualidade_Ovo> ListAQO(string incubatorio, DateTime dataInicial, DateTime dataFinal,
            string naoConformidade, string status, string lote)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var lista = hlbapp.Analise_Qualidade_Ovo
                .Where(w => w.DataAQO >= dataInicial && w.DataAQO <= dataFinal
                    && (w.Incubatorio == incubatorio || incubatorio == "")
                    && (w.LoteCompleto.Contains(lote) || lote == "")
                    && hlbapp.LOG_Analise_Qualidade_Ovo.Any(a => a.IDAQO == w.ID
                        && (a.NaoConformidade == naoConformidade || naoConformidade == "")))
                .ToList();

            List<Analise_Qualidade_Ovo> listaFiltrada = new List<Analise_Qualidade_Ovo>();

            foreach (var item in lista)
            {
                LOG_Analise_Qualidade_Ovo log = hlbapp.LOG_Analise_Qualidade_Ovo
                    .Where(w => w.IDAQO == item.ID)
                    .OrderByDescending(o => o.DataHora).FirstOrDefault();

                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-Acesso" + item.Incubatorio, (System.Collections.ArrayList)Session["Direitos"])
                    && (log.Status == status || status == ""))
                {
                    listaFiltrada.Add(item);
                }
            }

            return listaFiltrada;
        }

        public List<Analise_Qualidade_Ovo> FilterListAQO()
        {
            CleanSessions();

            string incubatorio = ((List<SelectListItem>)Session["FiltroDDLIncubatorio"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialAQO"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalAQO"].ToString());
            string naoConformidade = ((List<SelectListItem>)Session["FiltroDDLNaoConformidade"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            string status = ((List<SelectListItem>)Session["FiltroDDLStatus"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            string lote = Session["filtroLote"].ToString();

            return ListAQO(incubatorio, dataInicial, dataFinal, naoConformidade, status, lote);
        }

        #endregion

        #region Lista AQO

        public ActionResult ListaAQO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["msg"] = "";

            Session["ListaAQO"] = FilterListAQO();
            return View();
        }

        public ActionResult SearchAQO(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["Incubatorio"] != null)
                AtualizaDDL(model["Incubatorio"], (List<SelectListItem>)Session["FiltroDDLIncubatorio"]);

            DateTime dataInicial = new DateTime();
            if (model["dataInicialAQO"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialAQO"]);
                Session["dataInicialAQO"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialAQO"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalAQO"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalAQO"]);
                Session["dataFinalAQO"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalAQO"].ToString());

            if (model["NaoConformidade"] != null)
                AtualizaDDL(model["NaoConformidade"], (List<SelectListItem>)Session["FiltroDDLNaoConformidade"]);

            if (model["Status"] != null)
                AtualizaDDL(model["Status"], (List<SelectListItem>)Session["FiltroDDLStatus"]);

            if (model["lote"] != null)
                Session["filtroLote"] = model["lote"];

            #endregion

            Session["ListaAQO"] = ListAQO(model["Incubatorio"], dataInicial, dataFinal,
                model["NaoConformidade"], model["Status"], model["lote"]);
            return View("ListaAQO");
        }

        #endregion

        #region CRUD Methods

        public void CarregaAQO(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Analise_Qualidade_Ovo aqo = hlbapp.Analise_Qualidade_Ovo.Where(w => w.ID == id).FirstOrDefault();

            AtualizaDDL(aqo.Incubatorio, (List<SelectListItem>)Session["DDLIncubatorio"]);
            Session["dataAQO"] = aqo.DataAQO;
            Session["DDLNucleo"] = CarregaListaNucleos(false, "", aqo.Incubatorio);
            AtualizaDDL(aqo.Nucleo, (List<SelectListItem>)Session["DDLNucleo"]);
            Session["DDLLotes"] = CarregaLotes(aqo.Nucleo, aqo.Incubatorio);
            AtualizaDDL(aqo.Lote, (List<SelectListItem>)Session["DDLLotes"]);
            Session["DDLGalpoes"] = CarregaGalpoes(aqo.Lote);
            //Session["DDLGalpoes"] = CarregaGalpoesLotes(aqo.Lote);
            if (aqo.Incubatorio == "PH")
                AtualizaDDL(aqo.Galpao + " - " + aqo.Linhagem, (List<SelectListItem>)Session["DDLGalpoes"]);
            else
                AtualizaDDL(aqo.Galpao, (List<SelectListItem>)Session["DDLGalpoes"]);
            Session["Idade"] = aqo.Idade;
            Session["Linhagem"] = aqo.Linhagem;
            Session["loteCompleto"] = aqo.LoteCompleto;
            Session["DataProducao"] = aqo.DataProducao;
            Session["ResponsavelColeta"] = aqo.ResponsavelColeta;
            Session["Amostra"] = aqo.Amostra;
            Session["Sujo"] = aqo.Sujo;
            Session["Trincado"] = aqo.Trincado;
            Session["Virado"] = aqo.Virado;
            Session["Pequeno"] = aqo.Pequeno;
            Session["Grande"] = aqo.Grande;
            Session["Defeituoso"] = aqo.Defeituoso;
            Session["GravidadeEspecificaOvo"] = aqo.GravidadeEspecificaOvo;
            Session["TemperaturaOvo"] = aqo.TemperaturaOvo;
            Session["nfNum"] = aqo.NFNum;
            if (aqo.Observacao == null)
                Session["observacao"] = "";
            else
                Session["observacao"] = aqo.Observacao;
            Session["Sangue"] = aqo.Sangue;
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

        public ActionResult CreateAQO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("AQO");
        }

        public ActionResult EditAQO(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaAQO(id);

            return View("AQO");
        }

        public ActionResult SaveAQO(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();
            
            #endregion

            if (model["dataAQO"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Incubatório

                string incubatorio = model["incubatorio"];

                #endregion

                #region Data AQO

                DateTime dataAQO = Convert.ToDateTime(model["dataAQO"]);

                #endregion

                #region Núcleo

                string nucleo = model["Nucleo"];

                #endregion

                #region Lote

                string lote = model["Lote"];
                string loteCompleto = Session["loteCompleto"].ToString();

                #endregion

                #region Galpão

                string galpao = model["Galpao"];

                #endregion

                #region Data Produção

                DateTime dataProducao = Convert.ToDateTime(model["dataProducao"]);

                #endregion

                #region Carrega dados do lote

                List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

                Lotes retornoLote = listaLotes
                    .Where(l => l.LoteCompleto == loteCompleto)
                    .FirstOrDefault();

                #endregion

                #region Idade

                
                int age = 0;
                if (retornoLote != null)
                    age = ((Convert.ToDateTime(dataProducao) -
                         retornoLote.DataNascimento).Days) / 7;
                else
                {
                    ViewBag.Erro = "INCONSISTÊNCIA NO LANÇAMENTO DOS DADOS! POR FAVOR, REFAZER O LANÇAMENTO!";
                    Session["ListaAQO"] = FilterListAQO();
                    return View("ListaAQO");
                }

                #endregion

                #region Linhagem

                string linhagem = retornoLote.Linhagem;

                #endregion

                #region Responsavel Coleta

                string responsavelColeta = model["ResponsavelColeta"];

                #endregion

                #region Amostra

                int amostra = Convert.ToInt32(model["Amostra"]);

                #endregion

                #region Sujo (Fezes)

                int sujo = Convert.ToInt32(model["Sujo"]);

                #endregion

                #region Trincado

                int trincado = Convert.ToInt32(model["Trincado"]);

                #endregion

                #region Virado

                int virado = Convert.ToInt32(model["Virado"]);

                #endregion

                #region Pequeno

                int pequeno = Convert.ToInt32(model["Pequeno"]);

                #endregion

                #region Grande

                int grande = Convert.ToInt32(model["Grande"]);

                #endregion

                #region Defeituoso

                int defeituoso = Convert.ToInt32(model["Defeituoso"]);

                #endregion

                #region Gravidade Específica do Ovo (Densidade)

                decimal gravidadeEspecificaOvo =
                    Convert.ToDecimal(model["GravidadeEspecificaOvo"].ToString().Replace(".", ","));

                #endregion

                #region Temperatura do Ovo - DESATIVADO, INSERIDO NO DEO

                //decimal temperaturaOvo = Convert.ToDecimal(model["TemperaturaOvo"].ToString().Replace(".", ","));
                decimal temperaturaOvo = 0;

                #endregion

                #region Nº NF

                string nfNum = model["nfNum"];

                System.Data.Objects.ObjectParameter numeroNF =
                            new System.Data.Objects.ObjectParameter("numero", typeof(global::System.String));

                ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = 
                    new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                apoloService.CONCAT_ZERO_ESQUERDA(nfNum, 10, numeroNF);

                nfNum = numeroNF.Value.ToString();

                #endregion

                #region Observação

                string observacaoValor = model["observacao"];

                #endregion

                #region Sangue

                int sangue = Convert.ToInt32(model["Sangue"]);

                #endregion

                #endregion

                #region Insere AQO no WEB

                Analise_Qualidade_Ovo aqo = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    aqo = new Analise_Qualidade_Ovo();
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    aqo = hlbapp.Analise_Qualidade_Ovo.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                aqo.Incubatorio = incubatorio;
                aqo.DataAQO = dataAQO;
                aqo.Nucleo = nucleo;
                aqo.Galpao = galpao;
                aqo.Lote = lote;
                aqo.Idade = age;
                aqo.Linhagem = linhagem;
                aqo.LoteCompleto = loteCompleto;
                aqo.DataProducao = dataProducao;
                aqo.ResponsavelColeta = responsavelColeta;
                aqo.Amostra = amostra;
                aqo.Sujo = sujo;
                aqo.Trincado = trincado;
                aqo.Virado = virado;
                aqo.Pequeno = pequeno;
                aqo.Grande = grande;
                aqo.Defeituoso = defeituoso;
                aqo.GravidadeEspecificaOvo = gravidadeEspecificaOvo;
                aqo.TemperaturaOvo = temperaturaOvo;
                aqo.NFNum = nfNum;
                aqo.Observacao = observacaoValor;
                aqo.Sangue = sangue;

                string operacao = "Alteração";
                if (Convert.ToInt32(Session["idSelecionado"]) == 0) 
                {
                    operacao = "Inclusão";
                    hlbapp.Analise_Qualidade_Ovo.AddObject(aqo);
                }

                hlbapp.SaveChanges();

                #region Carrega %

                decimal metaSujo = 0.05m;
                /* 
                 * 26/07/2019 - Solicitado Por Sérica
                 * 
                 * Alteração de meta de ovos sujos para Onda Branca devido a entrada do processo do Butantan.
                */
                if (aqo.Nucleo.Substring(0, 2) == "HL")
                    metaSujo = 0.00m;
                decimal percSujo = ((aqo.Sujo * 1.00m) / aqo.Amostra) * 100.00m;
                decimal percSangue = ((Convert.ToInt32(aqo.Sangue) * 1.00m) / aqo.Amostra) * 100.00m;

                decimal metaTrincado = 1.00m;
                decimal percTrincado = ((aqo.Trincado * 1.00m) / aqo.Amostra) * 100.00m;

                decimal metaVirado = 0.50m;
                decimal percVirado = ((aqo.Virado * 1.00m) / aqo.Amostra) * 100.00m;

                decimal metaDefeituoso = 0.50m;
                decimal percDefeituoso = ((aqo.Defeituoso * 1.00m) / aqo.Amostra) * 100.00m;

                decimal metaGravidadeEspecificaOvoMinimo = 1075m;
                decimal metaGravidadeEspecificaOvoMaximo = 1090m;

                decimal metaTemperaturaOvoMinimo = 18m;
                decimal metaTemperaturaOvoMaximo = 24m;

                #endregion

                #region Caso existe LOG de não conformidade lançado e o valor foi alterado para um valor abaixo da meta, será deletado

                #region Sujo (Fezes)

                if (percSujo <= metaSujo)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Sujo (Fezes)").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                #region Trincado

                if (percTrincado <= metaTrincado)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Trincado").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                #region Virado

                if (percVirado <= metaVirado)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Virado").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                #region Defeituoso

                if (percDefeituoso <= metaDefeituoso)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Defeituoso").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                #region Gravidade Específica do Ovo (Densidade)

                if (gravidadeEspecificaOvo >= metaGravidadeEspecificaOvoMinimo
                    && gravidadeEspecificaOvo <= metaGravidadeEspecificaOvoMaximo)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Gravidade Específica do Ovo").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                #region Temperatura do Ovo - DESATIVADO E INSERIDA NA CONFERÊNCIA CEGA DO DEO

                //if (temperaturaOvo >= metaTemperaturaOvoMinimo
                //    && temperaturaOvo <= metaTemperaturaOvoMaximo)
                //{
                //    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                //        .Where(w => w.IDAQO == aqo.ID
                //            && w.NaoConformidade == "Temperatura do Ovo").ToList();

                //    foreach (var item in listaLogDelete)
                //    {
                //        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                //            .Where(w => w.ID == item.ID).FirstOrDefault();
                //        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                //    }
                //}

                #endregion

                #region Sangue

                if (percSangue <= metaSujo)
                {
                    var listaLogDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                        .Where(w => w.IDAQO == aqo.ID
                            && w.NaoConformidade == "Sujo (Sangue)").ToList();

                    foreach (var item in listaLogDelete)
                    {
                        LOG_Analise_Qualidade_Ovo logDelete = hlbapp.LOG_Analise_Qualidade_Ovo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbapp.LOG_Analise_Qualidade_Ovo.DeleteObject(logDelete);
                    }
                }

                #endregion

                hlbapp.SaveChanges();

                #endregion

                #region Insere LOG

                if ((percSujo > metaSujo) || (percSangue > metaSujo) || (percTrincado > metaTrincado) || (percVirado > metaVirado) || (percDefeituoso > metaDefeituoso) ||
                    (gravidadeEspecificaOvo < metaGravidadeEspecificaOvoMinimo && gravidadeEspecificaOvo > 0) ||
                    (gravidadeEspecificaOvo > metaGravidadeEspecificaOvoMaximo && gravidadeEspecificaOvo > 0) ||
                    (temperaturaOvo < metaTemperaturaOvoMinimo && temperaturaOvo > 0) ||
                    ((temperaturaOvo > metaTemperaturaOvoMaximo && temperaturaOvo > 0)))
                {
                    #region Carrega Dados p/ E-mail

                    string stringChar = "" + (char)13 + (char)10;

                    FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                    FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
                    fTA.FillByFarmID(fDT, aqo.Nucleo);

                    string emailResponsavelLote = "bvieira@hyline.com.br";
                    if (fDT.Count > 0)
                    {
                        FLIPDataSetMobile.FLOCKSFarmsRow fRow = fDT.FirstOrDefault();
                        if (fRow != null) if (!fRow.IsTEXT_7Null()) emailResponsavelLote = fRow.TEXT_7.Trim();
                    }

                    string paraNome = "Responsável pelo núcleo " + aqo.Nucleo;
                    string paraEmail = emailResponsavelLote;
                    string copiaPara = "";

                    //string paraNome = "Paulo Alves";
                    //string paraEmail = "palves@hyline.com.br";
                    //string copiaPara = "";

                    #endregion

                    #region Sujo (Fezes)

                    if (percSujo > metaSujo)
                    {
                        string naoConformidade = "Sujo (Fezes)";
                        string observacao = "Meta ultrapassada! (Meta: " + String.Format("{0:N2}", metaSujo) + "%). Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"OVO " + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade \"Ovo " + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o percentual de " + String.Format("{0:N2}", percSujo)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Trincado

                    if (percTrincado > metaTrincado)
                    {
                        string naoConformidade = "Trincado";
                        string observacao = "Meta ultrapassada! (Meta: " + String.Format("{0:N2}", metaTrincado) + "%). Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"OVO " + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade \"Ovo " + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o percentual de " + String.Format("{0:N2}", percTrincado)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Virado

                    if (percVirado > metaVirado)
                    {
                        string naoConformidade = "Virado";
                        string observacao = "Meta ultrapassada! (Meta: " + String.Format("{0:N2}", metaVirado) + "%). Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"OVO " + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade \"Ovo " + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o percentual de " + String.Format("{0:N2}", percVirado)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Defeituoso

                    if (percDefeituoso > metaDefeituoso)
                    {
                        string naoConformidade = "Defeituoso";
                        string observacao = "Meta ultrapassada! (Meta: " + String.Format("{0:N2}", metaDefeituoso) + "%). Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"OVO " + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade \"Ovo " + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o percentual de " + String.Format("{0:N2}", percDefeituoso)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Gravidade Específica do Ovo (Densidade)

                    if ((gravidadeEspecificaOvo < metaGravidadeEspecificaOvoMinimo && gravidadeEspecificaOvo > 0)
                        || (gravidadeEspecificaOvo > metaGravidadeEspecificaOvoMaximo && gravidadeEspecificaOvo > 0))
                    {
                        string naoConformidade = "Gravidade Específica do Ovo";
                        //string observacao = "Meta abaixo! (Meta: " + String.Format("{0:N2}", metaGravidadeEspecificaOvo) + "). Analisar e responder possível causa!";
                        string observacao = "Fora da Meta! "
                            + "(Meta: Mínima - " + String.Format("{0:N2}", metaGravidadeEspecificaOvoMinimo) + " / "
                            + "Máxima - " + String.Format("{0:N2}", metaGravidadeEspecificaOvoMaximo) + ")"
                            + ". Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"" + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade na \"" + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o valor de " + String.Format("{0:N2}", gravidadeEspecificaOvo)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion

                    #region Temperatura do Ovo - DESATIVADO E INSERIDA NA CONFERÊNCIA CEGA DO DEO

                    //if ((temperaturaOvo < metaTemperaturaOvoMinimo) || (temperaturaOvo > metaTemperaturaOvoMaximo))
                    //{
                    //    string placa = "";
                    //    string empresaApolo = RetornaEmpresaApolo(nucleo);
                    //    NOTA_FISCAL nfObj = apolo.NOTA_FISCAL
                    //        .Where(w => w.EmpCod == empresaApolo
                    //            && w.CtrlDFModForm == "NF-e"
                    //            && w.CtrlDFSerie == "001"
                    //            && w.NFNum == nfNum).FirstOrDefault();

                    //    if (nfObj != null)
                    //        placa = nfObj.NFVeicPlaca;

                    //    string naoConformidade = "Temperatura do Ovo";
                    //    string observacao = "Fora da Meta! "
                    //        + "(Meta: Mínima - " + String.Format("{0:N2}", metaTemperaturaOvoMinimo) + " / "
                    //        + "Máxima - " + String.Format("{0:N2}", metaTemperaturaOvoMaximo) + ")"
                    //        + ". Analisar e responder possível causa!";
                    //    InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                    //    #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                    //    #region Gera o E-mail

                    //    string assunto = "AQO - NÃO CONFORMIDADE \"" + naoConformidade.ToUpper() + "\" - "
                    //        + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                    //    string corpoEmail = "";
                    //    string anexos = "";
                    //    string empresaEmail = "5";

                    //    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    //        + "Existe não conformidade na \"" + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                    //        + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                    //        + " NF: " + nfNum + " - Placa: " + placa
                    //        + " com o valor de " + String.Format("{0:N2}", temperaturaOvo) + "°C"
                    //        + " analisado em " + aqo.DataAQO.ToShortDateString()
                    //        + "." + stringChar + stringChar
                    //        + "Por favor, verificar e realizar a resolução e informar no sistema!"
                    //        + stringChar + stringChar
                    //        + "SISTEMA WEB";

                    //    //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaEmail, "Texto");

                    //    #endregion

                    //    #endregion
                    //}

                    #endregion

                    #region Sujo

                    if (percSangue > metaSujo)
                    {
                        string naoConformidade = "Sujo (Sangue)";
                        string observacao = "Meta ultrapassada! (Meta: " + String.Format("{0:N2}", metaSujo) + "%). Analisar e responder possível causa!";
                        InsereLOGAQO(aqo.ID, operacao, naoConformidade, observacao, "", "Pendente");

                        #region Envia E-mail para Reponsável pelos Ovos - DESABILITADO (SOLICITADO POR JONATHAN COLAVITE EM 09/01/2020)

                        #region Gera o E-mail

                        string assunto = "AQO - NÃO CONFORMIDADE \"OVO " + naoConformidade.ToUpper() + "\" - "
                            + aqo.LoteCompleto + " - " + aqo.DataProducao.ToShortDateString();
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "5";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "Existe não conformidade \"Ovo " + naoConformidade + "\" da AQO ID " + aqo.ID.ToString()
                            + " referente ao lote " + aqo.LoteCompleto + ", produzido em " + aqo.DataProducao.ToShortDateString()
                            + " com o percentual de " + String.Format("{0:N2}", percSujo)
                            + " analisado em " + aqo.DataAQO.ToShortDateString()
                            + "." + stringChar + stringChar
                            + "Por favor, verificar e realizar a resolução e informar no sistema!"
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region Nenhuma Não Conformidade

                    string observacao = "";
                    InsereLOGAQO(aqo.ID, operacao, "Nenhuma", observacao, "", "Aprovado");

                    #endregion
                }

                #endregion

                #endregion
            }

            Session["ListaAQO"] = FilterListAQO();
            return View("ListaAQO");
        }

        public ActionResult ConfirmaDeleteAQO(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteAQO()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            Analise_Qualidade_Ovo aqo = hlbapp.Analise_Qualidade_Ovo.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Analise_Qualidade_Ovo.DeleteObject(aqo);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "AQO ID " + aqo.ID.ToString() + " excluído com sucesso!";

            Session["ListaAQO"] = FilterListAQO();
            return View("ListaAQO");
        }

        public ActionResult OK()
        {
            return View();
        }

        #endregion

        #region Relatórios Excel

        #region Relatório de AQO

        public ActionResult GerarRelatorioAQO()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialAQO"]);
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalAQO"]);
            string incubatorio = ((List<SelectListItem>)Session["FiltroDDLIncubatorio"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Value;

            incubatorio = incubatorio.Replace("TB", "AJ");

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\AQO";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\AQO\\Relatorio_AQO_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_AQO_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioAQOExcel(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, incubatorio);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_AQO_" + incubatorio.Replace("(","").Replace(")","") + "_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioAQOExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\AQO\\Relatorio_AQO.xlsx", destino);

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["DADOS"];

            string filtroIncubatorio = ((List<SelectListItem>)Session["FiltroDDLIncubatorio"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Text;

            worksheet.Cells[4, 3] = filtroIncubatorio;

            #region SQL Exibição

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_AQO V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "V.[DATA ANALISE] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "(V.[INCUBATÓRIO] = '" + incubatorio + "' or '" + incubatorio + "' = '') ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("AQO"))
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

        #endregion

        #endregion

        #region Não Conformidade - AQO

        #region List Methods

        public List<LOG_Analise_Qualidade_Ovo> ListAQONaoConforme(DateTime dataInicial, DateTime dataFinal,
            string naoConformidade, string status, string incubatorio, string lote)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            CarregaListaGranjas();
            List<SelectListItem> listaGranjas = (List<SelectListItem>)Session["ListaGranjas"];
            List<SelectListItem> listaNucleos = new List<SelectListItem>();

            foreach (var granja in listaGranjas)
            {
                List<SelectListItem> listaN = CarregaListaNucleos(true, granja.Value, incubatorio);

                foreach (var nucleo in listaN)
                {
                    listaNucleos.Add(nucleo);
                }
            }

            Session["nucleosUsuario"] = listaNucleos;

            var lista = hlbapp.LOG_Analise_Qualidade_Ovo
                .Where(l => 
                    (
                        hlbapp.Analise_Qualidade_Ovo.Any(w => w.DataAQO >= dataInicial && w.DataAQO <= dataFinal
                            && w.ID == l.IDAQO
                            && (w.Incubatorio == incubatorio || incubatorio == "")
                            && (w.LoteCompleto.Contains(lote) || lote == ""))
                        ||
                        hlbapp.LayoutDiarioExpedicaos.Any(w => w.DataHoraCarreg >= dataInicial && w.DataHoraCarreg <= dataFinal
                            && w.ID == l.IDAQO
                            && (w.Incubatorio == incubatorio || incubatorio == "")
                            && (w.LoteCompleto.Contains(lote) || lote == ""))
                    )
                    && (l.NaoConformidade == naoConformidade || naoConformidade == "")
                    && l.NaoConformidade != "Nenhuma")
                .GroupBy(g => new 
                {
                    g.IDAQO,
                    g.NaoConformidade
                })
                .Select(s => new
                {
                    s.Key.IDAQO,
                    s.Key.NaoConformidade,
                    ID = s.Max(m => m.ID)
                })
                .ToList();

            List<LOG_Analise_Qualidade_Ovo> listaFiltrada = new List<LOG_Analise_Qualidade_Ovo>();

            foreach (var item in lista)
            {
                LOG_Analise_Qualidade_Ovo log = hlbapp.LOG_Analise_Qualidade_Ovo
                    .Where(w => w.ID == item.ID)
                    .OrderByDescending(o => o.DataHora).FirstOrDefault();

                Analise_Qualidade_Ovo aqo = hlbapp.Analise_Qualidade_Ovo.Where(w => w.ID == item.IDAQO).FirstOrDefault();

                string nucleo = "";
                if (aqo != null)
                    nucleo = aqo.Nucleo;
                else
                {
                    LayoutDiarioExpedicaos deo = hlbapp.LayoutDiarioExpedicaos.Where(w => w.ID == item.IDAQO).FirstOrDefault();
                    nucleo = deo.Nucleo;
                }

                int existeFiltroNucleo = listaNucleos.Where(w => w.Text == nucleo).Count();

                if ((log.Status == status || status == "")
                    && (existeFiltroNucleo > 0 || MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController.GetGroup("HLBAPPM-AQOVisualizarTodos",
                                                    (System.Collections.ArrayList)Session["Direitos"])))
                {
                    listaFiltrada.Add(log);
                }
            }

            return listaFiltrada;
        }

        public List<LOG_Analise_Qualidade_Ovo> FilterListAQONaoConforme()
        {
            CleanSessions();

            string incubatorio = ((List<SelectListItem>)Session["FiltroDDLIncubatorio"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialAQO"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalAQO"].ToString());
            string naoConformidade = ((List<SelectListItem>)Session["FiltroDDLNaoConformidade"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            string status = ((List<SelectListItem>)Session["FiltroDDLStatus"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            string lote = Session["filtroLote"].ToString();

            return ListAQONaoConforme(dataInicial, dataFinal, naoConformidade, status, incubatorio, lote);
        }

        #endregion

        #region Lista Não Conformidade - AQO

        public ActionResult ListaAQONaoConforme()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["msg"] = "";

            Session["ListaAQONaoConforme"] = FilterListAQONaoConforme();
            return View();
        }

        public ActionResult SearchAQONaoConforme(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["Incubatorio"] != null)
                AtualizaDDL(model["Incubatorio"], (List<SelectListItem>)Session["FiltroDDLIncubatorio"]);

            DateTime dataInicial = new DateTime();
            if (model["dataInicialAQO"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialAQO"]);
                Session["dataInicialAQO"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialAQO"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalAQO"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalAQO"]);
                Session["dataFinalAQO"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalAQO"].ToString());

            if (model["NaoConformidade"] != null)
                AtualizaDDL(model["NaoConformidade"], (List<SelectListItem>)Session["FiltroDDLNaoConformidade"]);

            if (model["Status"] != null)
                AtualizaDDL(model["Status"], (List<SelectListItem>)Session["FiltroDDLStatus"]);

            if (model["lote"] != null)
                Session["filtroLote"] = model["lote"];

            #endregion

            Session["ListaAQONaoConforme"] = ListAQONaoConforme(dataInicial, dataFinal,
                model["NaoConformidade"], model["Status"], model["Incubatorio"], model["lote"]);
            return View("ListaAQONaoConforme");
        }

        #endregion

        #region Event Methods

        public void CarregaResolucaoAQO(int id, string status)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LOG_Analise_Qualidade_Ovo log = hlbapp.LOG_Analise_Qualidade_Ovo
                .Where(w => w.ID == id).FirstOrDefault();

            string texto = "";
            
            LOG_Analise_Qualidade_Ovo logResolucao = hlbapp
                .LOG_Analise_Qualidade_Ovo.Where(w => w.IDAQO == log.IDAQO
                    && w.NaoConformidade == log.NaoConformidade
                    && w.Status == status)
                .OrderByDescending(o => o.DataHora)
                .FirstOrDefault();

            if (logResolucao != null) texto = logResolucao.Resposta;
            
            Session["resolucaoAQONaoConforme"] = texto;
        }

        public ActionResult ResolucaoNaoConforme(int id, string status)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idAQOSelecionado"] = id;
            Session["statusAQOSelecionado"] = status;

            CarregaResolucaoAQO(id, status);

            return View("ResolucaoNaoConforme");
        }

        public ActionResult ReturnListaAQONaoConforme()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            return View("ListaAQONaoConforme");
        }

        public ActionResult SaveResolucaoNaoConforme(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            if (model["resolucao"] != null)
            {
                #region Carrega Valores

                #region Login

                string login = Session["login"].ToString().ToUpper();

                #endregion

                #region Resolução

                string resolucao = "";
                if (model["resolucao"] != null) resolucao = model["resolucao"];

                #endregion

                #endregion

                #region Salva na WEB

                #region Insere LOG

                int id = Convert.ToInt32(Session["idAQOSelecionado"]);
                string status = Session["statusAQOSelecionado"].ToString();
                string operacao = "Resolução";
                if (status == "Reprovado") operacao = "Reprovação";

                LOG_Analise_Qualidade_Ovo log = hlbapp.LOG_Analise_Qualidade_Ovo.Where(w => w.ID == id).FirstOrDefault();
                InsereLOGAQO(log.IDAQO, operacao, log.NaoConformidade, log.Observacao, resolucao, status);

                #endregion

                #endregion

                #region Envia E-mail

                #region Gera o E-mail

                #region Carrega Dados

                string stringChar = "" + (char)13 + (char)10;

                Analise_Qualidade_Ovo aqo = hlbapp.Analise_Qualidade_Ovo
                    .Where(w => w.ID == log.IDAQO).FirstOrDefault();

                string perc = "";
                string lote = "";
                string dataProducao = "";
                string incubatorio = "";
                if (aqo != null)
                {
                    incubatorio = aqo.Incubatorio;
                    lote = aqo.LoteCompleto;
                    dataProducao = aqo.DataProducao.ToShortDateString();
                    if (log.NaoConformidade == "Sujo")
                        perc = String.Format("{0:N2}", ((aqo.Sujo * 1.00m) / aqo.Amostra) * 100.00m) + "%";
                    else if (log.NaoConformidade == "Trincado")
                        perc = String.Format("{0:N2}", ((aqo.Trincado * 1.00m) / aqo.Amostra) * 100.00m) + "%";
                    else if (log.NaoConformidade == "Virado")
                        perc = String.Format("{0:N2}", ((aqo.Virado * 1.00m) / aqo.Amostra) * 100.00m) + "%";
                }
                else
                {
                    LayoutDiarioExpedicaos deo = hlbapp.LayoutDiarioExpedicaos.Where(w => w.ID == log.IDAQO).FirstOrDefault();
                    incubatorio = deo.Incubatorio;
                    lote = deo.LoteCompleto;
                    dataProducao = deo.DataProducao.ToShortDateString();
                    perc = Convert.ToDecimal(deo.TemperaturaOvoInterna).ToString("{0:N2}");
                }

                string paraNome = "Incubatório de Matrizes Nova Granada";
                string paraEmail = "sdoimo@hyline.com.br";
                string copiaPara = "incubacao-ng@hyline.com.br";
                if (incubatorio == "NM")
                {
                    paraNome = "Incubatório de Matrizes Novo Mundo";
                    paraEmail = "aneves@hyline.com.br";
                    copiaPara = "incubacao-nm@hyline.com.br";
                } 
                else if (incubatorio == "TB")
                {
                    paraNome = "Incubatório de Matrizes Ajapi";
                    paraEmail = "cfreire@hnavicultura.com.br";
                    copiaPara = "incubatorio.hygen@gmail.com";
                }
                else if (incubatorio == "PH")
                {
                    paraNome = "Incubatório de Bisavós";
                    paraEmail = "jsegura@hyline.com.br";
                    copiaPara = "aprates@hyline.com.br;administrativo-ia@hyline.com.br;producao-ga@hyline.com.br";
                }

                USUARIO usuarioOperacao = apolo.USUARIO
                    .Where(w => w.UsuCod == login).FirstOrDefault();

                //if (status == "Resolvido")
                //{
                //    USUARIO responsavelAuditoriaApolo = apolo.USUARIO
                //        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                //            && r.FuncCod == "0000169"))
                //        .FirstOrDefault();

                //    paraNome = responsavelAuditoriaApolo.UsuNome;
                //    paraEmail = responsavelAuditoriaApolo.UsuEmail;
                //    copiaPara = "";
                //}
                //else
                //{
                //    FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                //    FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
                //    fTA.FillByFarmID(fDT, aqo.Nucleo);

                //    string emailResponsavelLote = "bvieira@hyline.com.br";
                //    if (fDT.Count > 0)
                //    {
                //        FLIPDataSetMobile.FLOCKSFarmsRow fRow = fDT.FirstOrDefault();
                //        if (fRow != null) if (!fRow.IsTEXT_7Null()) emailResponsavelLote = fRow.TEXT_7.Trim();
                //    }

                //    paraNome = "Responsável pelo núcleo " + aqo.Nucleo;
                //    paraEmail = emailResponsavelLote;
                //    copiaPara = "";
                //}

                //string paraNome = "Paulo Alves";
                //string paraEmail = "palves@hyline.com.br";
                //string copiaPara = "";

                #endregion

                string assunto = "NÃO CONFORMIDADE \"OVO " + log.NaoConformidade.ToUpper() + "\" " + status.ToUpper();
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "A não conformidade \"Ovo " + log.NaoConformidade + "\" da AQO ID " + log.IDAQO.ToString() 
                    + " referente ao lote " + lote + ", produzido em " + dataProducao
                    + " com o valor de " + perc
                    + " foi " + status + " pelo usuário " + usuarioOperacao.UsuNome + " em "
                    + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString("HH:mm")
                    + "." + stringChar + stringChar
                    + "Segue " + operacao + ": " + resolucao + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Por favor, verificar e realizar a operação necessária!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                #endregion

                #endregion
            }

            Session["metodoRetorno"] = "ListaAQONaoConforme";
            return RedirectToAction("OK", "AQO");
        }

        public ActionResult AprovarResolucaoAQO(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Salva na WEB

            #region Insere LOG

            string login = Session["login"].ToString().ToUpper();
            LOG_Analise_Qualidade_Ovo log = hlbapp.LOG_Analise_Qualidade_Ovo.Where(w => w.ID == id).FirstOrDefault();
            InsereLOGAQO(log.IDAQO, "Aprovação", log.NaoConformidade, log.Observacao, "", "Aprovado");

            #endregion

            #region Envia E-mail para Responsável pela granja avisando da aprovação da resolução

            #region Gera o E-mail

            #region Carrega Dados

            string stringChar = "" + (char)13 + (char)10;

            Analise_Qualidade_Ovo aqo = hlbapp.Analise_Qualidade_Ovo
                .Where(w => w.ID == log.IDAQO).FirstOrDefault();

            string perc = "";
            string lote = "";
            string dataProducao = "";
            string incubatorio = "";
            if (aqo != null)
            {
                incubatorio = aqo.Incubatorio;
                lote = aqo.LoteCompleto;
                dataProducao = aqo.DataProducao.ToShortDateString();
                if (log.NaoConformidade == "Sujo")
                    perc = String.Format("{0:N2}", ((aqo.Sujo * 1.00m) / aqo.Amostra) * 100.00m) + "%";
                else if (log.NaoConformidade == "Trincado")
                    perc = String.Format("{0:N2}", ((aqo.Trincado * 1.00m) / aqo.Amostra) * 100.00m) + "%";
                else if (log.NaoConformidade == "Virado")
                    perc = String.Format("{0:N2}", ((aqo.Virado * 1.00m) / aqo.Amostra) * 100.00m) + "%";
            }
            else
            {
                LayoutDiarioExpedicaos deo = hlbapp.LayoutDiarioExpedicaos.Where(w => w.ID == log.IDAQO).FirstOrDefault();
                incubatorio = deo.Incubatorio;
                lote = deo.LoteCompleto;
                dataProducao = deo.DataProducao.ToShortDateString();
                perc = Convert.ToDecimal(deo.TemperaturaOvoInterna).ToString("{0:N2}");
            }

            USUARIO usuarioResolvido = apolo.USUARIO
                .Where(w => w.UsuCod == login).FirstOrDefault();

            //FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            //FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
            //fTA.FillByFarmID(fDT, aqo.Nucleo);

            //string emailResponsavelLote = "bvieira@hyline.com.br";
            //if (fDT.Count > 0)
            //{
            //    FLIPDataSetMobile.FLOCKSFarmsRow fRow = fDT.FirstOrDefault();
            //    if (fRow != null) if (!fRow.IsTEXT_7Null()) emailResponsavelLote = fRow.TEXT_7.Trim();
            //}

            //string paraNome = "Responsável pelo núcleo " + aqo.Nucleo;
            //string paraEmail = emailResponsavelLote;
            //string copiaPara = "";

            string paraNome = "Incubatório de Matrizes Nova Granada";
            string paraEmail = "sdoimo@hyline.com.br";
            string copiaPara = "incubacao-ng@hyline.com.br";
            if (incubatorio == "NM")
            {
                paraNome = "Incubatório de Matrizes Novo Mundo";
                paraEmail = "aneves@hyline.com.br";
                copiaPara = "incubacao-nm@hyline.com.br";
            }
            else if (incubatorio == "TB")
            {
                paraNome = "Incubatório de Matrizes Ajapi";
                paraEmail = "cfreire@hnavicultura.com.br";
                copiaPara = "incubatorio.hygen@gmail.com";
            }
            else if (incubatorio == "PH")
            {
                paraNome = "Incubatório de Bisavós";
                paraEmail = "jsegura@hyline.com.br";
                copiaPara = "aprates@hyline.com.br;administrativo-ia@hyline.com.br;producao-ga@hyline.com.br";
            }

            //string paraNome = "Paulo Alves";
            //string paraEmail = "palves@hyline.com.br";
            //string copiaPara = "";

            #endregion

            string assunto = "NÃO CONFORMIDADE \"OVO " + log.NaoConformidade.ToUpper() + "\" APROVADA";
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "5";

            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                + "A não conformidade \"Ovo " + log.NaoConformidade + "\" da AQO ID " + log.IDAQO.ToString()
                + " referente ao lote " + lote + ", produzido em " + dataProducao
                + " com o percentual de " + perc
                + " foi aprovado pelo usuário " + usuarioResolvido.UsuNome + " em "
                + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString("HH:mm")
                + "." + stringChar + stringChar
                + "SISTEMA WEB";

            EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

            #endregion

            #endregion

            #endregion

            Session["metodoRetorno"] = "ListaAQONaoConforme";
            return RedirectToAction("OK", "AQO");
        }

        #endregion

        #region Relatórios Excel

        #region Relatório de Indicadores de Responsável pela classificação

        public ActionResult GerarRelatorioEvolucaoAQO()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialAQO"]);
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalAQO"]);
            string incubatorio = ((List<SelectListItem>)Session["FiltroDDLIncubatorio"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Value;

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\AQO";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\AQO\\Relatorio_Evolucao_AQO_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_Evolucao_AQO_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioEvolucaoAQOExcel(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, incubatorio);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Evolucao_AQO_" + incubatorio.Replace("(", "").Replace(")", "") + "_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioEvolucaoAQOExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string incubatorio)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\AQO\\Relatorio_Evolucao_AQO.xlsx", destino);

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

            #region SQL Exibição

            #region Carrega Nucleos

            string nucleos = "";
            List<SelectListItem> ddlNucleos = (List<SelectListItem>)Session["nucleosUsuario"];
            foreach (var item in ddlNucleos)
            {
                nucleos = nucleos + "'" + item.Value + "'";
                if (ddlNucleos.IndexOf(item) < (ddlNucleos.Count - 1))
                    nucleos = nucleos + ",";
            }

            #endregion

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_AQO V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "V.[DATA ANALISE] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "(V.[INCUBATÓRIO] = '" + incubatorio + "' or '" + incubatorio + "' = '') and " +
                    "V.[NÚCLEO] in (" + nucleos + ") ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("AQO"))
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

        #endregion

        #endregion

        #endregion

        #region Populate / Update Lists

        public List<SelectListItem> CarregaListaIncubatorios(bool todos)
        {
            string location = "PP";

            FLIPDataSetMobile.HATCHERY_CODESDataTable hDT = new FLIPDataSetMobile.HATCHERY_CODESDataTable();
            MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter hTA =
                new Models.FLIPDataSetMobileTableAdapters.HATCHERY_CODESTableAdapter();

            hTA.FillByLocation(hDT, location);

            List<SelectListItem> items = new List<SelectListItem>();

            if (todos)
                items.Add(new SelectListItem { Text = "(Todos)", Value = "", Selected = true });

            foreach (var item in hDT)
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                {
                    items.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                }
            }

            #region Avós

            hTA.FillByLocation(hDT, "GP");

            foreach (var item in hDT)
            {
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                {
                    items.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                }
            }

            #endregion

            return items;
        }

        public List<SelectListItem> CarregaListaNaoConformidade()
        {
            List<SelectListItem> ddlLista = new List<SelectListItem>();

            ddlLista.Add(new SelectListItem
            {
                Text = "(Todas)",
                Value = "",
                Selected = true
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Nenhuma",
                Value = "Nenhuma",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Trincado",
                Value = "Trincado",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Sujo",
                Value = "Sujo",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Virado",
                Value = "Virado",
                Selected = false
            });
            ddlLista.Add(new SelectListItem
            {
                Text = "Gravidade Específica do Ovo",
                Value = "Gravidade Específica do Ovo",
                Selected = false
            });
            ddlLista.Add(new SelectListItem
            {
                Text = "Temperatura do Ovo",
                Value = "Temperatura do Ovo",
                Selected = false
            });

            return ddlLista;
        }

        public List<SelectListItem> CarregaListaStatus()
        {
            List<SelectListItem> ddlLista = new List<SelectListItem>();

            ddlLista.Add(new SelectListItem
            {
                Text = "(Todos)",
                Value = "",
                Selected = true
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Pendente",
                Value = "Pendente",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Resolvido",
                Value = "Resolvido",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Aprovado",
                Value = "Aprovado",
                Selected = false
            });

            ddlLista.Add(new SelectListItem
            {
                Text = "Reprovado",
                Value = "Reprovado",
                Selected = false
            });

            return ddlLista;
        }

        public List<SelectListItem> CarregaListaNucleos(bool filtroGranja, string granja, string incubatorio)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string geracao = "PP";
            if (incubatorio == "PH") geracao = "GP";

            FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            fTA.FillFarmsDEO(fDT);

            foreach (var item in fDT.Where(f => f.LOCATION == geracao).ToList())
            {
                if ((filtroGranja && item.FARM_ID.StartsWith(granja)) || !filtroGranja)
                    items.Add(new SelectListItem { Text = item.FARM_ID, Value = item.FARM_ID, Selected = false });
            }

            return items;            
        }

        public List<SelectListItem> CarregaLotes(string id, string incubatorio)
        {
            MvcAppHyLinedoBrasil.Data.FLIPDataSet flip = new MvcAppHyLinedoBrasil.Data.FLIPDataSet();
            FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();

            List<SelectListItem> items = new List<SelectListItem>();

            Session["listLotes"] = new List<Lotes>();
            string location = "PP";
            if (incubatorio == "PH") location = "GP";

            List<Lotes> listaLotes = new List<Lotes>();

            flocks.FillActivesByFarm(flip.FLOCKS, "HYBR", "BR", location, id);

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

            return items;
        }

        public List<SelectListItem> CarregaGalpoes(string id)
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
                    if (lote.Granja.Contains("SB"))
                    {
                        items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                        itemsGlp.Add(new SelectListItem { Text = galpao + " - " + lote.Linhagem, Value = galpao + " - " + lote.Linhagem, Selected = false });
                    }
                    else
                    {
                        items.Add(new Lotes { Galpao = galpao, Linhagem = lote.Linhagem, LoteCompleto = lote.LoteCompleto, Location = lote.Location });
                        itemsGlp.Add(new SelectListItem { Text = galpao, Value = galpao, Selected = false });
                    }
                }
            }

            Session["listLotes"] = listaLotes;
            
            return itemsGlp;
        }

        public List<Lotes> CarregaGalpoesLotes(string id)
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

        public void CarregaListaGranjas()
        {
            if (Session["usuario"].ToString() != "0")
            {
                bdApoloEntities bdApolo = new bdApoloEntities();
                ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();

                List<SelectListItem> items = new List<SelectListItem>();

                string login = Session["login"].ToString().ToUpper();

                var listaFiliais = bdApolo.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod != null && e.USERFLIPCod != ""
                        && bdApolo.EMP_FIL_USUARIO.Any(u => u.UsuCod == login && u.EmpCod == e.EmpCod)
                        && (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
                    .SelectMany(
                        x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
                        (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
                    .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
                    .ToList();

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
                        Text = codFLIP + " - " + item.EMPRESA_FILIAL.EmpNome + ovosComercio,
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

                Session["ListaGranjas"] = items;
            }
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

        #endregion

        #region Métodos p/ JavaScript

        [HttpPost]
        public ActionResult CarregaNucleosJS(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<SelectListItem> items = CarregaListaNucleos(false, "", id);

            return Json(items);
        }

        [HttpPost]
        public ActionResult CarregaLotesJS(string id, string id2)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<SelectListItem> items = CarregaLotes(id, id2);

            return Json(items);
        }

        [HttpPost]
        public ActionResult CarregaGalpoesJS(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return Json(CarregaGalpoesLotes(id));
        }

        [HttpPost]
        public ActionResult RetornaLoteCompleto(string id, string id2)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Lotes retornoLote;

            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];

            retornoLote = listaLotes
                .Where(l => l.NumeroLote == id && l.Galpao == id2)
                .FirstOrDefault();

            if (retornoLote == null)
            {
                retornoLote = listaLotes
                    .Where(l => l.NumeroLote == id && l.Galpao + " - " + l.Linhagem == id2)
                    .FirstOrDefault();
            }

            Session["loteCompleto"] = retornoLote.LoteCompleto;

            return Json(retornoLote);
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region AQO

            Session["idSelecionado"] = 0;

            if (Session["FiltroDDLIncubatorio"] == null) Session["FiltroDDLIncubatorio"] = CarregaListaIncubatorios(true);
            if (Session["dataInicialAQO"] == null) Session["dataInicialAQO"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (Session["dataFinalAQO"] == null) Session["dataFinalAQO"] = DateTime.Today;
            if (Session["FiltroDDLNaoConformidade"] == null) Session["FiltroDDLNaoConformidade"] = CarregaListaNaoConformidade();
            if (Session["FiltroDDLStatus"] == null) Session["FiltroDDLStatus"] = CarregaListaStatus();

            if (Session["DDLIncubatorio"] == null) Session["DDLIncubatorio"] = CarregaListaIncubatorios(false);
            if (Session["dataAQO"] == null) Session["dataAQO"] = DateTime.Today;
            if (Session["filtroLote"] == null) Session["filtroLote"] = "";

            Session["DDLNucleo"] = new List<SelectListItem>();
            if (((List<SelectListItem>)Session["DDLIncubatorio"]).Where(w => w.Selected == true).Count() > 0)
            {
                var inc = ((List<SelectListItem>)Session["DDLIncubatorio"]).Where(w => w.Selected == true).FirstOrDefault().Value;
                Session["DDLNucleo"] = CarregaListaNucleos(false, "", inc);
            }
                Session["DDLLotes"] = new List<SelectListItem>();
            Session["DDLGalpoes"] = new List<SelectListItem>();
            Session["Idade"] = 0;
            Session["Linhagem"] = "";
            Session["loteCompleto"] = "";
            Session["DataProducao"] = DateTime.Today;
            Session["ResponsavelColeta"] = "";
            Session["Amostra"] = 0;
            Session["Sujo"] = 0;
            Session["Trincado"] = 0;
            Session["Virado"] = 0;
            Session["Pequeno"] = 0;
            Session["Grande"] = 0;
            Session["Defeituoso"] = 0;
            Session["GravidadeEspecificaOvo"] = 0;
            Session["TemperaturaOvo"] = 0;
            Session["nfNum"] = "";
            Session["observacao"] = "";
            Session["Sangue"] = 0;

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
            email.WorkFLowEmailDeEmail = "web@hyline.com.br";
            email.WorkFlowEmailFormato = formato;
            if (assunto.Length > 80) assunto = assunto.Substring(0, 80);
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexos;
            email.WorkFlowEmailDocEmpCod = empresaApolo;

            apolo.WORKFLOW_EMAIL.AddObject(email);

            apolo.SaveChanges();
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

        #endregion
    }
}
