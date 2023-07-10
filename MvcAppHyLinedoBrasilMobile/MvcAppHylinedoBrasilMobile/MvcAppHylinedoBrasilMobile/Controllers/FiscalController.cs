using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using System.Data.Objects;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class FiscalController : Controller
    {
        #region Menus

        public ActionResult MenuFiscal()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["FiltroListaConfigImportaNFe"] = CarregaListaConfigImportaNFe(true);

            return View();
        }

        #endregion

        #region Configurações

        #region Tabela de Configuração de Importação de NF-e

        #region List Methods

        public List<Configuracao_Importa_NFe> ListConfiguracaoImportaNFe(string descricao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            List<Configuracao_Importa_NFe> retorno = new List<Configuracao_Importa_NFe>();

            var listaConfiguracaoImportaNFe = hlbapp.Configuracao_Importa_NFe
                .Where(w => w.Descricao.Contains(descricao)).ToList();

            retorno = listaConfiguracaoImportaNFe;

            return retorno.OrderBy(o => o.Descricao).ToList();
        }

        public List<Configuracao_Importa_NFe> FilterListConfiguracaoImportaNFe()
        {
            CleanSessions();

            string descricao = Session["pesquisaDescricao"].ToString();

            List<Configuracao_Importa_NFe> listaConfiguracaoImportaNFe = ListConfiguracaoImportaNFe(descricao);

            return listaConfiguracaoImportaNFe;
        }

        #endregion

        #region Lista Configuração de Importação de NF-e

        public ActionResult ListaConfiguracaoImportaNFe()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //CleanSessions();
            Session["ListaConfiguracaoImportaNFe"] = FilterListConfiguracaoImportaNFe();
            return View();
        }

        public ActionResult SearchConfiguracaoImportaNFe(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            string descricao = "";
            if (model["pesquisaDescricao"] != null)
            {
                descricao = model["pesquisaDescricao"];
                Session["pesquisaDescricao"] = descricao;
            }
            else
                descricao = Session["pesquisaDescricao"].ToString();

            #endregion

            Session["ListaConfiguracaoImportaNFe"] = ListConfiguracaoImportaNFe(descricao);
            return View("ListaConfiguracaoImportaNFe");
        }

        #endregion

        #region CRUD Methods

        public void CarregaConfiguracaoImportaNFe(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Configuracao_Importa_NFe config = hlbapp.Configuracao_Importa_NFe.Where(w => w.ID == id).FirstOrDefault();

            Session["descricaoCIN"] = config.Descricao;
            Session["tipoLancCIN"] = config.TipoLancCod;
            Session["clasFiscalCIN"] = config.ClasFiscCod;
            Session["ddlClasFiscalCIN"] = CarregaListaOrigemConfig();
            if (((List<SelectListItem>)Session["ddlClasFiscalCIN"]).Where(w => w.Text == config.ClasFiscCod).Count() > 0)
                AtualizaDDL(config.ClasFiscCod, (List<SelectListItem>)Session["ddlClasFiscalCIN"]);
            else
                AtualizaDDL("Fixa", (List<SelectListItem>)Session["ddlClasFiscalCIN"]);
            Session["ddlDataMovimentoCIN"] = CarregaListaOrigemConfigData();
            AtualizaDDL(config.DataMovimento, (List<SelectListItem>)Session["ddlDataMovimentoCIN"]);
            Session["natOperacaoCIN"] = config.NaturezaOperacao;
            Session["locArmazCIN"] = config.LocArmazCod;
            Session["contaDebitoCIN"] = config.ContaDebito;
            Session["ddlContaDebitoCIN"] = CarregaListaOrigemConfig();
            if (((List<SelectListItem>)Session["ddlContaDebitoCIN"]).Where(w => w.Text == config.ContaDebito).Count() > 0)
                AtualizaDDL(config.ContaDebito, (List<SelectListItem>)Session["ddlContaDebitoCIN"]);
            else
                AtualizaDDL("Fixa", (List<SelectListItem>)Session["ddlContaDebitoCIN"]);
        }

        public ActionResult CreateConfiguracaoImportaNFe()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("ConfiguracaoImportaNFe");
        }

        public ActionResult EditConfiguracaoImportaNFe(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaConfiguracaoImportaNFe(id);

            return View("ConfiguracaoImportaNFe");
        }

        public ActionResult SaveConfiguracaoImportaNFe(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            bdApoloEntities bdApolo = new bdApoloEntities();

            #endregion

            if (model["descricao"] != null)
            {
                #region Carrega Valores

                #region Descricao

                string descricao = model["descricao"];

                #endregion

                #region Tipo de Lançamento

                string tipoLancamento = "";
                if (model["tipoLancamento"] != null)
                {
                    tipoLancamento = model["tipoLancamento"];
                }

                #endregion

                #region Classificação Fiscal

                string clasFiscal = "";
                if (model["ddlClasFiscal"] != null)
                {
                    if (model["ddlClasFiscal"] == "Fixa")
                        clasFiscal = model["txtClasFiscal"];
                    else
                        clasFiscal = model["ddlClasFiscal"];
                }

                #endregion

                #region Data Movimento

                string dataMovimento = "";
                if (model["ddldataMovimento"] != null)
                    dataMovimento = model["ddldataMovimento"];

                #endregion

                #region Natureza de Operação

                string natOperacao = "";
                if (model["natOperacao"] != null)
                    natOperacao = model["natOperacao"];

                #endregion

                #region Local de Armazenagem

                string localArmazenagem = "";
                if (model["localArmazenagem"] != null)
                    localArmazenagem = model["localArmazenagem"];

                #endregion

                #region Conta Débito

                string contaDebito = "";
                if (model["ddlContaDebito"] != null)
                {
                    if (model["ddlContaDebito"] == "Fixa")
                        contaDebito = model["txtContaDebito"];
                    else
                        contaDebito = model["ddlContaDebito"];
                }

                #endregion

                #endregion

                #region Insere Configuração de Importação de NF-e no WEB

                Configuracao_Importa_NFe configNFE = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    configNFE = new Configuracao_Importa_NFe();
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    configNFE = hlbapp.Configuracao_Importa_NFe.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                configNFE.Descricao = descricao;
                configNFE.TipoLancCod = tipoLancamento;
                configNFE.ClasFiscCod = clasFiscal;
                configNFE.DataMovimento = dataMovimento;
                configNFE.NaturezaOperacao = natOperacao;
                configNFE.LocArmazCod = localArmazenagem;
                configNFE.ContaDebito = contaDebito;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Configuracao_Importa_NFe.AddObject(configNFE);

                hlbapp.SaveChanges();

                #endregion
            }

            Session["ListaConfiguracaoImportaNFe"] = FilterListConfiguracaoImportaNFe();
            return View("ListaConfiguracaoImportaNFe");
        }

        public ActionResult ConfirmaDeleteConfiguracaoImportaNFe(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteConfiguracaoImportaNFe()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            Configuracao_Importa_NFe configNFE = hlbapp.Configuracao_Importa_NFe.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Configuracao_Importa_NFe.DeleteObject(configNFE);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Configuração de Importação de NF-e " + configNFE.Descricao + " excluída com sucesso!";

            Session["ListaConfiguracaoImportaNFe"] = FilterListConfiguracaoImportaNFe();
            return View("ListaConfiguracaoImportaNFe");
        }

        #endregion

        #endregion

        #endregion

        #region Manutenção

        #region Recebimento de Documentos

        #region List Methods

        public List<Recebimento_Documento> ListRecebimentoDocumentos(DateTime dataInicial, DateTime dataFinal, int idConfigImportaNFe)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            List<Recebimento_Documento> retorno = new List<Recebimento_Documento>();

            dataInicial = Convert.ToDateTime(dataInicial.ToShortDateString() + " 00:00:00");
            dataFinal = Convert.ToDateTime(dataFinal.ToShortDateString() + " 23:59:59");

            var listaRecebimentoDocumentos = hlbapp.Recebimento_Documento
                .Where(w => w.DataHoraCadastro >= dataInicial && w.DataHoraCadastro <= dataFinal
                    && (w.IDConfigImportaNFe == idConfigImportaNFe || idConfigImportaNFe == 0)).ToList();

            retorno = listaRecebimentoDocumentos;

            return retorno.OrderBy(o => o.DataHoraCadastro).ToList();
        }

        public List<Recebimento_Documento> FilterListRecebimentoDocumentos()
        {
            CleanSessions();

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialRecDoc"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalRecDoc"].ToString());
            int idConfigImportaNFe = Convert.ToInt32(Session["idConfigImportaNFe"].ToString());

            List<Recebimento_Documento> listaRecebimentoDocumentos = ListRecebimentoDocumentos(dataInicial, dataFinal, idConfigImportaNFe);

            return listaRecebimentoDocumentos;
        }

        #endregion

        #region Lista Recebimento de Documentos

        public ActionResult ListaRecebimentoDocumentos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //CleanSessions();
            Session["ListaRecebimentoDocumentos"] = FilterListRecebimentoDocumentos();

            return View();
        }

        public ActionResult SearchRecebimentoDocumentos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRecDoc"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRecDoc"]);
                Session["dataInicialRecDoc"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRecDoc"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRecDoc"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRecDoc"]);
                Session["dataFinalRecDoc"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRecDoc"].ToString());

            int idConfigImportaNFe = 0;
            if (model["ConfigImportaNFe"] != null)
            {
                idConfigImportaNFe = Convert.ToInt32(model["ConfigImportaNFe"]);
                AtualizaDDL(model["ConfigImportaNFe"], (List<SelectListItem>)Session["FiltroListaConfigImportaNFe"]);
            }

            #endregion

            Session["ListaRecebimentoDocumentos"] = ListRecebimentoDocumentos(dataInicial, dataFinal, idConfigImportaNFe);

            return View("ListaRecebimentoDocumentos");
        }

        #endregion

        #region CRUD Methods

        public void CarregaRecebimentoDocumento(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Recebimento_Documento recDoc = hlbapp.Recebimento_Documento.Where(w => w.ID == id).FirstOrDefault();

            Session["chaveEletronicaRD"] = recDoc.ChaveEletronica;
            Session["numeroPedidoCompraRD"] = recDoc.NumeroPedidoCompra;
            Session["dataEntradaRD"] = recDoc.DataEntrada;
            if (Session["ListaConfigImportaNFe"] != null)
                AtualizaDDL(recDoc.IDConfigImportaNFe.ToString(), (List<SelectListItem>)Session["ListaConfigImportaNFe"]);
        }

        public ActionResult CreateRecebimentoDocumento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("RecebimentoDocumento");
        }

        public ActionResult EditRecebimentoDocumento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaRecebimentoDocumento(id);

            return View("RecebimentoDocumento");
        }

        public ActionResult SaveRecebimentoDocumento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            bdApoloEntities bdApolo = new bdApoloEntities();

            #endregion

            if (model["chaveEletronica"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Chave Eletrônica

                string chaveEletronica = model["chaveEletronica"];

                #endregion

                #region Número do Pedido de Compra

                string numeroPedidoCompra = "";
                if (model["numeroPedidoCompra"] != null)
                {
                    ObjectParameter pedCompraCompleto = new ObjectParameter("nUMERO", typeof(global::System.String));
                    bdApolo.CONCAT_ZERO_ESQUERDA(model["numeroPedidoCompra"], 7, pedCompraCompleto);

                    numeroPedidoCompra = pedCompraCompleto.Value.ToString();
                }

                #endregion

                #region Data Entrada

                DateTime? dataEntrada = null;
                if (model["dataEntrada"] != null)
                {
                    dataEntrada = Convert.ToDateTime(model["dataEntrada"]);
                }

                #endregion

                #region Configuração Tabela NF-e

                int idConfigTabelaNFe = 0;
                if (model["ConfigTabelaNFe"] != null)
                    if (model["ConfigTabelaNFe"] != "")
                        idConfigTabelaNFe = Convert.ToInt32(model["ConfigTabelaNFe"]);

                #endregion

                #endregion

                #region Insere Recebimento de Documentos no WEB

                Recebimento_Documento recDov = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    recDov = new Recebimento_Documento();
                    recDov.Usuario = usuario;
                    recDov.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    recDov = hlbapp.Recebimento_Documento.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                recDov.ChaveEletronica = chaveEletronica;
                recDov.NumeroPedidoCompra = numeroPedidoCompra;
                recDov.DataEntrada = dataEntrada;
                recDov.IDConfigImportaNFe = idConfigTabelaNFe;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Recebimento_Documento.AddObject(recDov);

                hlbapp.SaveChanges();

                #endregion
            }

            Session["ListaRecebimentoDocumentos"] = FilterListRecebimentoDocumentos();
            return View("ListaRecebimentoDocumentos");
        }

        public ActionResult ConfirmaDeleteRecebimentoDocumento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteRecebimentoDocumento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            Recebimento_Documento recDoc = hlbapp.Recebimento_Documento.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Recebimento_Documento.DeleteObject(recDoc);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Recebimento do Documento " + recDoc.ChaveEletronica + " excluído com sucesso!";

            Session["ListaRecebimentoDocumentos"] = FilterListRecebimentoDocumentos();
            return View("ListaRecebimentoDocumentos");
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

        public List<SelectListItem> CarregaListaConfigImportaNFe(bool todos)
        {
            List<SelectListItem> ddlConfigImportaNFe = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (todos)
            {
                ddlConfigImportaNFe.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "0",
                    Selected = true
                });
            }

            var listaConfigImportaNFe = hlbapp.Configuracao_Importa_NFe
                .OrderBy(o => o.Descricao)
                .ToList();

            foreach (var item in listaConfigImportaNFe)
            {
                ddlConfigImportaNFe.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.ID.ToString(),
                    Selected = false
                });
            }

            return ddlConfigImportaNFe;
        }

        public List<SelectListItem> CarregaListaOrigemConfig()
        {
            List<SelectListItem> ddlOrigemConfig = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            ddlOrigemConfig.Add(new SelectListItem
            {
                Text = "Fixa",
                Value = "Fixa",
                Selected = false
            });
            ddlOrigemConfig.Add(new SelectListItem
            {
                Text = "Produto",
                Value = "Produto",
                Selected = false
            });
            ddlOrigemConfig.Add(new SelectListItem
            {
                Text = "Parâmetro",
                Value = "Parâmetro",
                Selected = false
            });

            return ddlOrigemConfig;
        }

        public List<SelectListItem> CarregaListaOrigemConfigData()
        {
            List<SelectListItem> ddlOrigemConfig = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            ddlOrigemConfig.Add(new SelectListItem
            {
                Text = "Parâmetro",
                Value = "Parâmetro",
                Selected = false
            });

            return ddlOrigemConfig;
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region Geral

            Session["idSelecionado"] = 0;

            #endregion

            #region Configuração de Importação de NF-e

            Session["pesquisaDescricao"] = "";

            Session["descricaoCIN"] = "";
            Session["tipoLancCIN"] = "";
            Session["clasFiscalCIN"] = "";
            Session["ddlClasFiscalCIN"] = CarregaListaOrigemConfig();
            Session["ddlDataMovimentoCIN"] = CarregaListaOrigemConfigData();
            Session["natOperacaoCIN"] = "";
            Session["locArmazCIN"] = "";
            Session["contaDebitoCIN"] = "";
            Session["ddlContaDebitoCIN"] = CarregaListaOrigemConfig();

            #endregion

            #region Recebimento de Documentos

            Session["chaveEletronicaRD"] = "";
            Session["numeroPedidoCompraRD"] = "";
            Session["dataEntradaRD"] = DateTime.Today;
            Session["ListaConfigImportaNFe"] = CarregaListaConfigImportaNFe(false);

            if (Session["dataInicialRecDoc"] == null) Session["dataInicialRecDoc"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (Session["dataFinalRecDoc"] == null) Session["dataFinalRecDoc"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 
                DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
            if (Session["idConfigImportaNFe"] == null) Session["idConfigImportaNFe"] = 0;

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

        #endregion
    }
}
