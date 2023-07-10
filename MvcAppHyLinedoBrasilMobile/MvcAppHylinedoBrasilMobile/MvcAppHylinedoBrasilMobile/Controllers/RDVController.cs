using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using MvcAppHylinedoBrasilMobile.Infra;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Data.Objects;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using MvcAppHylinedoBrasilMobile.Models.CHICMobileDataSetTableAdapters;
using System.Diagnostics;
using System.Collections;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class RDVController : Controller
    {
        #region DataBase Entities

        public static HLBAPPEntities hlbappStatic = new HLBAPPEntities();
        public static bdApoloEntities apoloStatic = new bdApoloEntities();
        public static Apolo10Entities apolo2Static = new Apolo10Entities();

        #endregion

        #region List Methods

        public List<RDV> ListaRDV(DateTime dataInicial, DateTime dataFinal, string usuario, 
            string status, string tipoLancamento, string formaPagamento)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            bool permissaoFinanceiro = Convert.ToBoolean(Session["permissaoFinanceiro"]);
            string login = Session["login"].ToString().ToUpper();

            List<RDV> retorno = new List<RDV>();

            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                string empresa = Session["empresa"].ToString().Substring(i, 2);

                var listaRDV = hlbapp.RDV.Where(w => w.DataRDV >= dataInicial
                    && w.DataRDV <= dataFinal
                        && (w.Usuario == usuario || usuario == "(Todos)")
                        && (((!permissaoFinanceiro ||
                           (permissaoFinanceiro && 
                            ((w.TipoDespesa.Contains("(C") && w.Usuario != login) || w.Usuario == login)))
                            && status == "Pendente")
                            || status != "Pendente")
                        && (w.Status == status || status == "(Todos)")
                        && (w.TipoDespesa.Contains(tipoLancamento) || tipoLancamento == "(Todos)")
                        && (w.FormaPagamento == formaPagamento || formaPagamento == "(Todas)")
                        && w.Empresa == empresa).ToList();

                foreach (var item in listaRDV)
                {
                    bool entrou = false;
                    if (item.Usuario.Equals("CREZENDE@PLANALTOPOSTURA.COM.BR"))
                    {
                        entrou = true;
                    }
                    if (usuario == "(Todos)" && Session["ListaFuncionariosPesquisa"] != null)
                    {
                        List<SelectListItem> listaFuncionarios =
                            (List<SelectListItem>)Session["ListaFuncionariosPesquisa"];

                        foreach (var funcionario in listaFuncionarios)
                        {
                            if (funcionario.Value != null)
                                if (funcionario.Value.ToUpper() == item.Usuario.ToUpper())
                                    retorno.Add(item);
                        }
                    }
                    else
                    {
                        retorno.Add(item);
                    }
                }
            }

            return retorno.OrderBy(o => o.DataRDV).ToList();
        }

        public List<RDV> ListaFatura(int anoMesInicial, int anoMesFinal, string usuario, string status)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            List<RDV> retorno = new List<RDV>();

            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                string empresa = Session["empresa"].ToString().Substring(i, 2);

                string funcCodPreencheRDV = "";
                FUNCIONARIO preencheRDV = apolo.FUNCIONARIO
                    .Where(w => w.UsuCod == usuario.ToUpper()
                        && apolo.GRP_FUNC.Any(a => a.FuncCod == w.FuncCod
                            && a.GrpFuncObs == "Preenche RDV"))
                    .FirstOrDefault();
                if (preencheRDV != null)
                {
                    //usuario = "(Todos)";
                    funcCodPreencheRDV = preencheRDV.FuncCod;
                }

                var listaRDV = hlbapp.RDV.Where(w => w.AnoMes >= anoMesInicial
                    && w.AnoMes <= anoMesFinal
                        && w.AnoMes != null
                        && w.FormaPagamento == "Cartão Corp."
                        && (w.Usuario == usuario || funcCodPreencheRDV != "")
                        && (w.Status == status || status == "(Todos)")
                        && w.Empresa == empresa).ToList();

                foreach (var item in listaRDV)
                {
                    FUNCIONARIO funcionario = apolo.FUNCIONARIO
                        .Where(w => w.UsuCod == item.Usuario.ToUpper()
                            && apolo.GRP_FUNC.Any(a => a.GrpFuncCod == w.FuncCod
                                && a.GrpFuncObs == "Preenche RDV"
                                && a.FuncCod == funcCodPreencheRDV))
                        .FirstOrDefault();

                    //if ((usuario == "(Todos)" && funcionario != null) || usuario == item.Usuario)
                    if (funcionario != null || usuario == item.Usuario)
                        retorno.Add(item);
                }
            }

            return retorno.OrderBy(o => o.DataRDV).ToList();
        }

        public List<RDV> FilterListaRDV()
        {
            CleanSessions();

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());
            string usuario = Session["usuarioSelecionado"].ToString();
            string status = Session["statusSelecionado"].ToString();
            string tipoLancamento = Session["tipoLancamentoSelecionado"].ToString();
            string formaPagamento = Session["formaPagamentoSelecionada"].ToString();

            List<RDV> listaRDV = ListaRDV(dataInicial, dataFinal, usuario, status, tipoLancamento, formaPagamento);

            return listaRDV;
        }

        public List<RDV> FilterListaRDVMensal()
        {
            CleanSessions();

            string usuario = Session["usuarioSelecionado"].ToString();
            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialRDV"]);
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalRDV"]);
            string status = Session["statusSelecionado"].ToString();

            #region Calculo do Dia da Semana - INUTILIZADO

            //int semanaAnoInicial = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dataInicial,
            //            CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            //int anoInicial = (dataInicial.Month == 1 && semanaAnoInicial == 52 ? dataInicial.Year - 1 : dataInicial.Year);
            //DateTime primeiroDiaSemana = MvcAppHylinedoBrasilMobile.Controllers.RDVController
            //                .FirstDateOfWeekISO8601(anoInicial, semanaAnoInicial);
            //int semanaAnoFinal = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dataFinal,
            //            CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            //int anoFinal = (dataFinal.Month == 1 && semanaAnoFinal == 52 ? dataFinal.Year - 1 : dataFinal.Year);
            //DateTime ultimoDiaSemana = MvcAppHylinedoBrasilMobile.Controllers.RDVController
            //                .FirstDateOfWeekISO8601(anoFinal, semanaAnoFinal).AddDays(7);

            #endregion

            DateTime primeiroDiaMes = new DateTime(dataInicial.Year, dataInicial.Month, 1);
            DateTime ultimoDiaMes = new DateTime(dataFinal.Year, dataFinal.Month,
                DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month));

            var listaRDV = ListaRDV(primeiroDiaMes, ultimoDiaMes, usuario, status, "(Todos)", "(Todas)");
            Session["ListaRDV"] = listaRDV;

            //return MontaListaRDVMensal(listaRDV);
            return listaRDV;
        }

        public List<RDVMensal> MontaListaRDVMensal(List<RDV> listaRDV)
        {
            List<RDVMensal> listaMensal = new List<RDVMensal>();

            foreach (var item in listaRDV)
            {
                //int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(item.DataRDV,
                //        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                int mes = item.DataRDV.Month;

                RDVMensal rdvMensal = listaMensal
                    .Where(w => w.Mes == mes && w.Usuario == item.Usuario
                        && w.Empresa == item.Empresa && w.NumeroRDV == item.NumeroFechamentoRDV)
                    .FirstOrDefault();

                if (rdvMensal == null)
                {
                    rdvMensal = new RDVMensal();
                    rdvMensal.Empresa = item.Empresa;
                    rdvMensal.Mes = mes;
                    rdvMensal.Usuario = item.Usuario;
                    rdvMensal.NomeUsuario = item.NomeUsuario;
                    rdvMensal.NumeroRDV = item.NumeroFechamentoRDV;
                    if (item.TipoDespesa.Contains("(D)"))
                        rdvMensal.Valor = item.ValorDespesa * (-1);
                    else
                        rdvMensal.Valor = item.ValorDespesa;
                    rdvMensal.Ano = item.DataRDV.Year;
                    listaMensal.Add(rdvMensal);
                }
                else
                {
                    if (item.TipoDespesa.Contains("(D)"))
                        rdvMensal.Valor = rdvMensal.Valor + (item.ValorDespesa * (-1));
                    else
                        rdvMensal.Valor = rdvMensal.Valor + item.ValorDespesa;
                }
            }

            return listaMensal.OrderBy(o => o.Ano).ThenBy(t => t.Mes)
                .ThenBy(t => t.Usuario).ThenBy(t => t.NumeroRDV).ToList();
        }

        public List<RDV> FilterListaFatura()
        {
            CleanSessions();

            int anoMesInicial = Convert.ToInt32(Convert.ToDateTime(Session["dataInicialRDV"].ToString())
                .ToString("yyyyMM"));
            int anoMesFinal = Convert.ToInt32(Convert.ToDateTime(Session["dataFinalRDV"].ToString())
                .ToString("yyyyMM"));
            string usuario = Session["usuarioSelecionado"].ToString();
            string status = Session["statusSelecionado"].ToString();
            string tipoLancamento = Session["tipoLancamentoSelecionado"].ToString();

            List<RDV> listaRDV = ListaFatura(anoMesInicial, anoMesFinal, usuario, status);

            return listaRDV;
        }

        #endregion

        #region Geral

        public void CarregaRDV(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            RDV rdv = hlbapp.RDV.Where(w => w.ID == id).FirstOrDefault();

            Session["empresaSelecionadaRDV"] = rdv.Empresa;
            if (Session["ListaEmpresasRDV"] != null)
                AtualizaDDL(rdv.Empresa, (List<SelectListItem>)Session["ListaEmpresasRDV"]);
            Session["usuarioRDV"] = rdv.Usuario;
            if (Session["ListaFuncionarios"] != null)
                AtualizaDDL(rdv.Usuario, (List<SelectListItem>)Session["ListaFuncionarios"]);
            Session["dataRDV"] = rdv.DataRDV;
            if (Session["ListaFormaPag"] != null)
                AtualizaDDL(rdv.FormaPagamento, (List<SelectListItem>)Session["ListaFormaPag"]);
            Session["descricaoRDV"] = rdv.Descricao;
            if (Session["ListaPaises"] != null)
                AtualizaDDL(rdv.CodPais, (List<SelectListItem>)Session["ListaPaises"]);
            Session["cidadeRDV"] = rdv.NomeCidade;
            string origemRDV = "";
            if (rdv.NomeCidade != "Nacional")
            {
                origemRDV = "(DI)";
                Session["origemRDV"] = "Internacional";
                if (Session["ListaOrigem"] != null)
                    AtualizaDDL("Internacional", (List<SelectListItem>)Session["ListaOrigem"]);
            }
            else
            {
                origemRDV = "(DN)";
                Session["origemRDV"] = rdv.CodCidade;
                if (Session["ListaOrigem"] != null)
                    AtualizaDDL(rdv.CodCidade, (List<SelectListItem>)Session["ListaOrigem"]);
            }
            Session["TipoDespesaSelecionadaRDV"] = rdv.TipoDespesa;
            if (rdv.TipoDespesa.Contains("(C")) origemRDV = "(C";
            Session["ListaTipoDespesaRDV"] = CarregaListaTipoDespesa(origemRDV);
            if (Session["ListaTipoDespesaRDV"] != null)
                AtualizaDDL(rdv.TipoDespesa, (List<SelectListItem>)Session["ListaTipoDespesaRDV"]);
            //AtualizaDDL(rdv.CodCidade, (List<SelectListItem>)Session["ListaLocaisRDV"]);
            if (rdv.ValorDespesa == 0 && rdv.FormaPagamento == "Espécie"
                && rdv.NomeCidade == "Internacional")
            {
                Session["valorDespesaRDV"] = rdv.ValorMoedaEstrangeira;
                Session["valorMoedaEstrangeiraRDV"] = 0;
            }
            else
            {
                Session["valorDespesaRDV"] = rdv.ValorDespesa;
                Session["valorMoedaEstrangeiraRDV"] = rdv.ValorMoedaEstrangeira;
            }
            string indEconCod = "0000001";
            if (rdv.IndEconCod != null) indEconCod = rdv.IndEconCod;
            AtualizaDDL(indEconCod, (List<SelectListItem>)Session["ListaIndiceEconomico"]);
            Session["motivoRDV"] = rdv.Motivo;
            if (rdv.TipoDespesa.Equals("KILOMETRAGEM (DN)"))
            {
                //Session["qtdeKMRDV"] = rdv.QtdeDiarias;
                Session["qtdeKMRDV"] = String.Format("{0:N2}", rdv.QtdeDiarias);
                Session["qtdeDiariaRDV"] = 0;
                Session["valorDiariaDespesaRDV"] = 0;
            }
            else
            {
                Session["qtdeKMRDV"] = 0;
                Session["qtdeDiariaRDV"] = rdv.QtdeDiarias;
                Session["valorDiariaDespesaRDV"] = rdv.ValorDiaria;
            }

            if (rdv.TipoDespesa.Equals("COMBUSTÍVEL(DN)"))
            {
                Session["kmAtualRDV"] = rdv.Km;
                Session["qtdeLitrosRDV"] = String.Format("{0:N4}", rdv.QtdeLitros);
                Session["valorLitroRDV"] = String.Format("{0:N4}", rdv.ValorLitro);
                if (Session["ListaPaises"] != null && rdv.TipoCombustivel != null)
                    AtualizaDDL(rdv.TipoCombustivel, (List<SelectListItem>)Session["ListaTipoCombustivel"]);
                Session["placaRDV"] = rdv.Placa;
            }

            if (rdv.ImagemRecibo != null)
            {
                var base64 = Convert.ToBase64String(rdv.ImagemRecibo);
                Session["imagem"] = String.Format("data:image/gif;base64,{0}", base64);
            }
        }

        public ActionResult MenuRDV()
        {
            Session["ListaFormaPag"] = CarregaListaFormaPagamento();
            Session["ListaOrigem"] = CarregaListaOrigem();
            //Session["ListaIndiceEconomico"] = CarregaListaIndicesEconomicos();
            Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
            Session["ListaStatus"] = CarregaListaStatus();
            Session["ListaTipoLancamento"] = CarregaListaTipoLancamento();
            Session["ListaPaises"] = CarregaListaPaises(true);
            Session["ListaPaisesExterior"] = CarregaListaPaises(false);
            Session["ListaTipoCombustivel"] = CarregaListaTipoCombustivel();

            CleanSessions();

            return View("_MenuRDV");
        }

        public ActionResult VisualizaRDVFechado(string numRDV)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"] != "")
                {
                    List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
                    if (VerificaGerencia(Session["login"].ToString().ToUpper(), rdvUrl.FirstOrDefault().Usuario.ToUpper()))
                    {
                        Session["dataInicialRDV"] = rdvUrl.Min(m => m.DataRDV);
                        Session["dataFinalRDV"] = rdvUrl.Max(m => m.DataRDV);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";

                        Session["urlChamada"] = "";
                    }
                    else
                    {
                        ViewBag.Erro = "Você não tem acesso a esse RDV, pois você não tem permissão para aprová-lo!";

                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "Fechado";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["ListaRDV"] = FilterListaRDV();

                        return View("ListaRDVParaAprovacao");
                    }
                }
            }

            Session["ListaVisualizaRDV"] = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            ViewBag.Metodo = "VisualizaRDV";
            ViewBag.Titulo = "Visualização do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

            return View("VisualizaRDV");
        }

        public void EnviarEmail(string paraNome, string paraEmail, string copiaPara,
            string assunto, string corpoEmail, string anexos, string empresaApolo)
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
            email.WorkFlowEmailFormato = "Texto";
            if (assunto.Length > 80) assunto = assunto.Substring(0, 80);
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexos;
            email.WorkFlowEmailDocEmpCod = empresaApolo;

            apolo.WORKFLOW_EMAIL.AddObject(email);

            apolo.SaveChanges();
        }

        public string GeraRDVPDF(string numRDV)
        {
            string caminho = @"\\srv-riosoft-01\W\RDV\RDV_" + numRDV + ".pdf";

            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport =
                new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            MyReport.Load(Server.MapPath("~/Reports/RDV.rpt"));

            MyReport.ParameterFields["NumeroFechamentoRDV"].CurrentValues.AddValue(numRDV);

            MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, caminho);
            MyReport.Close();
            MyReport.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return caminho;
        }

        #endregion

        #region RDV - Pessoal

        #region CRUD Methods

        public ActionResult CreateDespesa()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["ListaIndiceEconomico"] = CarregaListaIndicesEconomicos(false);
            //Session["ListaFuncionarios"] = CarregaListaFuncionarios(false);
            Session["ListaTipoDespesaRDV"] = CarregaListaTipoDespesa("(DN)");
            CarregaEmpresas();

            #region Recupera Dados do último lançamento

            if (Session["dataUltimoLancamentoRDV"] != null)
                Session["dataRDV"] = Session["dataUltimoLancamentoRDV"];
            if (Session["origemRDVUltimoLancamento"] != null)
            {
                Session["origemRDV"] = Session["origemRDVUltimoLancamento"];
                if (Session["origemRDV"].ToString() != "Nacional")
                {
                    Session["ListaTipoDespesaRDV"] = CarregaListaTipoDespesa("(DI)");
                }
                else
                {
                    Session["ListaTipoDespesaRDV"] = CarregaListaTipoDespesa("(DN)");
                }
                AtualizaDDL(Session["origemRDV"].ToString(), (List<SelectListItem>)Session["ListaOrigem"]);
            }
            if (Session["valorUltimoLancamentoRDV"] != null 
                && Session["valorMoedaEstrangeiraUltimoLancamentoRDV"] != null)
            {
                if (Convert.ToInt32(Session["valorUltimoLancamentoRDV"]) == 0
                    && Session["origemRDV"].ToString() == "Internacional")
                {
                    Session["valorDespesaRDV"] = Session["valorMoedaEstrangeiraUltimoLancamentoRDV"];
                    Session["valorMoedaEstrangeiraRDV"] = 0;
                }
                else
                {
                    Session["valorDespesaRDV"] = Session["valorUltimoLancamentoRDV"];
                    Session["valorMoedaEstrangeiraRDV"] = Session["valorMoedaEstrangeiraUltimoLancamentoRDV"];
                }
            }
            if (Session["indiceEconomicoRDVUltimoLancamento"] != null)
                AtualizaDDL(Session["indiceEconomicoRDVUltimoLancamento"].ToString(), 
                    (List<SelectListItem>)Session["ListaIndiceEconomico"]);
            if (Session["tipoDespesaUltimoLancamentoRDV"] != null)
                AtualizaDDL(Session["tipoDespesaUltimoLancamentoRDV"].ToString(),
                    (List<SelectListItem>)Session["ListaTipoDespesaRDV"]);

            #endregion

            return View();
        }

        public ActionResult EditDespesa(int id)
        {
            CleanSessions();

            Session["idSelecionado"] = id;

            //Session["ListaLocaisRDV"] = Session["ListaLocaisOriginal"];
            Session["ListaFormaPag"] = CarregaListaFormaPagamento();
            Session["ListaOrigem"] = CarregaListaOrigem();
            Session["ListaIndiceEconomico"] = CarregaListaIndicesEconomicos(false);
            //Session["ListaFuncionarios"] = CarregaListaFuncionarios(false);
            CarregaEmpresas();

            CarregaRDV(id);

            return View("CreateDespesa");
        }

        public ActionResult ConfirmaDeleteDespesa(int id)
        {
            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteDespesa()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            RDV rdv = hlbapp.RDV.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.RDV.DeleteObject(rdv);
            hlbapp.SaveChanges();

            #region Insere LOG Lançamento

            InsereLOGLancamentoRDV(rdv, "Exclusão");

            #endregion

            Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
            Session["statusSelecionado"] = "Pendente";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();

            return View("ListaLancamentosPendentes");
        }

        public ActionResult SaveDespesa(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2 = new Apolo10Entities();
            bdApoloEntities apolo = new bdApoloEntities();

            #endregion

            if (model["dataRDV"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Empresa

                string empresa = "";
                string nomeUsuario = "";
                FUNCIONARIO funcionarioApolo = apolo2.FUNCIONARIO
                    .Where(w => w.UsuCod == usuario
                        && w.USERParticipaControleRDVWeb.Equals("Sim")).FirstOrDefault();
                if (funcionarioApolo != null)
                {
                    empresa = funcionarioApolo.USEREmpres;
                    nomeUsuario = funcionarioApolo.FuncNome.ToUpper();
                }
                else
                {
                    VENDEDOR vendedorApolo = apolo.VENDEDOR
                        .Where(w => w.USERLoginSite == usuario
                            && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();

                    if (vendedorApolo != null)
                    {
                        Empresas empresaObj = hlbapp.Empresas
                            .Where(w => w.DescricaoApoloVendedor == vendedorApolo.USEREmpresa)
                            .FirstOrDefault();

                        if (empresaObj != null) empresa = empresaObj.CodigoCHIC;
                        nomeUsuario = vendedorApolo.VendNome;
                    }
                }

                #endregion

                #region Data RDV

                DateTime dataRDV = new DateTime();
                if (model["dataRDV"] != null)
                {
                    dataRDV = Convert.ToDateTime(model["dataRDV"]);
                    Session["dataRDV"] = dataRDV.ToShortDateString();
                }
                else
                    dataRDV = Convert.ToDateTime(Session["dataRDV"].ToString());

                #endregion

                #region Tipo de Despesa

                string tipoDespesa = "";
                if (model["TipoDespesa"] != null)
                {
                    tipoDespesa = model["TipoDespesa"];
                    Session["TipoDespesaSelecionadaRDV"] = tipoDespesa;
                    AtualizaDDL(tipoDespesa, (List<SelectListItem>)Session["ListaTipoDespesaRDV"]);
                }
                else
                    tipoDespesa = Session["TipoDespesaSelecionadaRDV"].ToString();

                #endregion

                #region Forma de Pagamento

                string formaPag = "Espécie";
                //if (model["FormaPagamento"] != null)
                //{
                //    formaPag = model["FormaPagamento"];
                //    Session["FormaPagamentoSelecionadaRDV"] = formaPag;
                //    AtualizaDDL(formaPag, (List<SelectListItem>)Session["ListaFormaPag"]);
                //}
                //else
                //    formaPag = Session["FormaPagamentoSelecionadaRDV"].ToString();

                #endregion

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null)
                {
                    descricao = model["descricao"];
                    Session["descricaoRDV"] = descricao;
                }
                else
                    descricao = Session["descricaoRDV"].ToString();

                #endregion

                #region Local (DESATIVADO)

                //string local = "";
                //if (model["Local"] != null)
                //{
                //    local = model["Local"];
                //    Session["LocalSelecionado"] = local;
                //    AtualizaDDL(local, (List<SelectListItem>)Session["ListaLocaisRDV"]);
                //}
                //else
                //    local = Session["LocalSelecionado"].ToString();

                #endregion

                #region Origem

                string origem = "";
                if (model["Origem"] != null)
                {
                    origem = model["Origem"];
                    Session["OrigemSelecionadaRDV"] = origem;
                    AtualizaDDL(origem, (List<SelectListItem>)Session["ListaOrigem"]);
                }
                else
                    origem = Session["OrigemSelecionadaRDV"].ToString();

                #endregion

                #region Valor Despesa

                decimal valorDespesa = 0;
                if (model["valorDespesa"] != null)
                {
                    valorDespesa = Convert.ToDecimal(model["valorDespesa"].ToString().Replace(".",","));
                    if (valorDespesa <= 0)
                    {
                        ViewBag.Erro = "O Valor da Despesa tem que ser maior que zero!";
                        return View("CreateDespesa");
                    }

                    Session["valorDespesaRDV"] = valorDespesa;
                }

                #endregion

                #region Campos Tipo Hospedagem

                #region Qtde Diaria

                decimal qtdeDiaria = 0;
                if (model["qtdeDiarias"] != null)
                {
                    if (Decimal.TryParse(model["qtdeDiarias"], out qtdeDiaria))
                    {
                        Session["qtdeDiariaRDV"] = qtdeDiaria;
                    }
                }

                #endregion

                #region Valor Diaria

                decimal valorDiaria = 0;
                if (model["valorDiaria"] != null)
                {
                    if (Decimal.TryParse(model["valorDiaria"].ToString().Replace(".", ","), out valorDiaria))
                        Session["valorDiariaDespesaRDV"] = valorDiaria;
                }

                #endregion

                #endregion

                #region Campos Tipo Kilometragem

                #region Qtde KM

                decimal qtdeKM = 0;
                if (model["qtdeKM"] != null)
                {
                    //if (Decimal.TryParse(model["qtdeKM"], out qtdeKM))
                    if (Decimal.TryParse(model["qtdeKM"].ToString().Replace(".", ","), out qtdeKM))
                        Session["qtdeKMRDV"] = qtdeKM;
                }

                #endregion

                #endregion

                #region Campos Internacional

                #region Indice Economico

                string indEcon = "";
                if (model["IndiceEconomico"] != null)
                {
                    indEcon = model["IndiceEconomico"];
                    Session["IndiceEconomicoSelecionadoRDV"] = indEcon;
                    AtualizaDDL(indEcon, (List<SelectListItem>)Session["ListaIndiceEconomico"]);
                }
                else
                    indEcon = Session["IndiceEconomicoSelecionadoRDV"].ToString();

                #endregion

                #region Valor Moeda Estrangeira

                decimal valorMoedaEstrangeira = 0;
                if (model["valorMoedaEstrangeira"] != null)
                {
                    if (Decimal.TryParse(model["valorMoedaEstrangeira"].ToString().Replace(".", ","), out valorMoedaEstrangeira))
                    {
                        Session["valorMoedaEstrangeiraRDV"] = valorMoedaEstrangeira;

                        if (valorMoedaEstrangeira == 0 && origem.Equals("Internacional") && formaPag.Equals("Espécie"))
                        {
                            valorMoedaEstrangeira = valorDespesa;
                            valorDespesa = 0;
                        }
                    }
                }

                #endregion

                #region Pais (DESATIVADO)

                string pais = "";
                if (model["Pais"] != null)
                {
                    pais = model["Pais"];
                    Session["PaisSelecionadoRDV"] = pais;
                    AtualizaDDL(pais, (List<SelectListItem>)Session["ListaPaises"]);
                }
                else
                    pais = Session["PaisSelecionadoRDV"].ToString();

                #endregion

                #region Cidade / Origem

                string cidade = "";
                if (model["cidade"] != null)
                {
                    cidade = model["cidade"];
                    Session["cidadeRDV"] = cidade;
                }
                else
                    cidade = Session["cidadeRDV"].ToString();

                if (origem.Equals("Nacional") 
                    || (origem.Equals("Internacional") && formaPag.Equals("Espécie")))
                {
                    cidade = origem;
                    pais = origem;
                }

                #endregion

                #endregion

                #region Motivo

                string motivo = "";
                if (model["motivo"] != null)
                {
                    motivo = model["motivo"];
                    Session["motivoRDV"] = motivo;
                }
                else
                    motivo = Session["motivoRDV"].ToString();

                #endregion

                #region Campos Combustível

                #region KM Atual

                decimal kmAtual = 0;
                if (model["kmAtual"] != null)
                {
                    if (Decimal.TryParse(model["kmAtual"], out kmAtual))
                    {
                        Session["kmAtualRDV"] = kmAtual;
                    }
                }

                #endregion

                #region Qtde. Litros

                decimal qtdeLitros = 0;
                if (model["qtdeLitros"] != null)
                {
                    if (Decimal.TryParse(model["qtdeLitros"].ToString().Replace(".", ","), out qtdeLitros))
                        Session["qtdeLitrosRDV"] = qtdeLitros;
                }

                #endregion

                #region Valor do Litro

                decimal valorLitro = 0;
                if (model["valorLitro"] != null)
                {
                    if (Decimal.TryParse(model["valorLitro"].ToString().Replace(".", ","), out valorLitro))
                        Session["valorLitroRDV"] = valorLitro;
                }

                #endregion

                #region Tipo de Combustível

                string tipoCombustivel = "";
                if (model["TipoCombustivel"] != null)
                {
                    tipoCombustivel = model["TipoCombustivel"];
                    AtualizaDDL(tipoDespesa, (List<SelectListItem>)Session["ListaTipoCombustivel"]);
                }

                #endregion

                #region Placa

                string placa = "";
                if (model["placa"] != null)
                {
                    placa = model["placa"];
                    Session["placaRDV"] = placa;
                }
                else
                    placa = Session["placaRDV"].ToString();

                #endregion

                #endregion

                #endregion

                #region Insere no WEB

                RDV rdv = null;
                string operacao = "Inclusão";
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                    rdv = new RDV();
                else
                {
                    operacao = "Alteração";
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    rdv = hlbapp.RDV.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                rdv.Empresa = empresa;
                if (usuario == "")
                {
                    rdv.Usuario = Session["login"].ToString().ToUpper();
                    rdv.NomeUsuario = Session["usuario"].ToString().ToUpper();
                }
                else
                {
                    rdv.Usuario = usuario;
                    rdv.NomeUsuario = nomeUsuario;
                }
                rdv.DataHora = DateTime.Now;
                rdv.DataRDV = dataRDV;
                rdv.TipoDespesa = tipoDespesa;
                rdv.FormaPagamento = formaPag;
                rdv.Descricao = descricao;
                rdv.CodCidade = pais;
                rdv.NomeCidade = cidade;
                if (rdv.NomeCidade == "Nacional")
                {
                    rdv.CodPais = "BR";
                    rdv.NomePais = "BRASIL";
                }
                else
                {
                    rdv.CodPais = "";
                    rdv.NomePais = "";
                }
                rdv.ValorDespesa = valorDespesa;
                rdv.ValorMoedaEstrangeira = valorMoedaEstrangeira;
                if (valorMoedaEstrangeira > 0)
                {
                    rdv.IndEconCod = indEcon;
                    var listaIndEcon = (List<SelectListItem>)Session["ListaIndiceEconomico"];
                    rdv.IndEconNome = listaIndEcon.Where(w => w.Value == indEcon).FirstOrDefault().Text;
                }
                rdv.Motivo = motivo;

                if (tipoDespesa.Equals("KILOMETRAGEM (DN)"))
                    rdv.QtdeDiarias = qtdeKM;
                else
                    rdv.QtdeDiarias = qtdeDiaria;
                rdv.ValorDiaria = valorDiaria;

                if ((rdv.NumeroFechamentoRDV == null) || (rdv.NumeroFechamentoRDV == ""))
                    rdv.Status = "Pendente";
                else
                    rdv.Status = "Fechado";

                rdv.Km = kmAtual;
                rdv.QtdeLitros = qtdeLitros;
                rdv.ValorLitro = valorLitro;
                rdv.TipoCombustivel = tipoCombustivel;
                rdv.Placa = placa.ToUpper();

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.RDV.AddObject(rdv);

                #endregion

                #region Recupera Dados do último lançamento

                Session["dataUltimoLancamentoRDV"] = rdv.DataRDV;
                Session["valorUltimoLancamentoRDV"] = rdv.ValorDespesa;
                Session["valorMoedaEstrangeiraUltimoLancamentoRDV"] = rdv.ValorMoedaEstrangeira;
                Session["tipoDespesaUltimoLancamentoRDV"] = rdv.TipoDespesa;
                Session["origemRDVUltimoLancamento"] = rdv.CodCidade;
                Session["indiceEconomicoRDVUltimoLancamento"] = rdv.IndEconCod;

                #endregion

                hlbapp.SaveChanges();

                #region Insere LOG Lançamento

                InsereLOGLancamentoRDV(rdv, operacao);

                #endregion
            }

            Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
            Session["statusSelecionado"] = "Pendente";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();
            return View("ListaLancamentosPendentes");
        }

        public ActionResult PrintRDV(string numRDV, bool download)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"] != "")
                {
                    List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
                    if (Session["login"].ToString().ToUpper() != rdvUrl.FirstOrDefault().Usuario.ToUpper())
                    {
                        ViewBag.Erro = "Você não tem acesso a esse RDV, pois ele não pertence a você!";

                        CleanSessions();

                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["ListaRDV"] = FilterListaRDV();

                        Session["ListaRDV"] = ((List<RDV>)Session["ListaRDV"])
                            .Where(w => w.Status != "Pendente")
                            .OrderBy(o => o.DataRDV)
                            .ToList();

                        return View("ListaRDVFechados");
                    }
                    else
                    {
                        Session["urlChamada"] = "";
                    }
                }
            }

            try
            {
                string caminho = @"\\srv-riosoft-01\W\RDVs\RDV_" + Session["login"].ToString() + "_"
                    + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";

                Session["numRDVSelecionado"] = numRDV;

                CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport =
                    new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                MyReport.Load(Server.MapPath("~/Reports/RDV.rpt"));

                MyReport.ParameterFields["NumeroFechamentoRDV"].CurrentValues.AddValue(numRDV);

                MyReport.SetDatabaseLogon("sa", "");

                if (download)
                {
                    Stream stream = MyReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType
                        .PortableDocFormat);
                    return File(stream, "application/pdf", "RDV_" + numRDV + ".pdf");
                }
                else
                {
                    var response = System.Web.HttpContext.Current.Response;
                    MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                        response, false, "RDV_" + numRDV);
                    return new EmptyResult();
                }

                MyReport.Close();
                MyReport.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao gerar relatório: " + e.Message
                    + " | Linha de Erro no Código: " + linenum.ToString();
                return View("ListaRDVFechados");
            }
        }

        #endregion

        public ActionResult ListaLancamentosPendentes()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["statusSelecionado"] = "Pendente";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();

            return View();
        }

        public ActionResult SearchLancamentosPendentes(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["login"].ToString();
            string tipoLancamento = "(Todos)";
            string formaPagamento = "Espécie";
            string status = "Pendente";

            #endregion

            Session["ListaRDV"] = ListaRDV(dataInicial, dataFinal, usuario, status, tipoLancamento, formaPagamento);

            return View("ListaLancamentosPendentes");
        }

        public ActionResult FechamentoRDV()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["statusSelecionado"] = "Pendente";
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaVisualizaRDV"] = FilterListaRDV();

            if (((List<RDV>)Session["ListaVisualizaRDV"]).Count > 0)
            {
                ViewBag.Metodo = "FechamentoRDV";
                ViewBag.Titulo = "Fechamento de RDV";

                ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

                return View("VisualizaRDV");
            }
            else
            {
                ViewBag.Mensagem = "Não existe Lançamentos Pendentes para serem Fechados neste período!";
                return View("ListaLancamentosPendentes");
            }
        }

        public ActionResult ListaRDVFechados()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            //Session["usuarioSelecionado"] = "(Todos)";
            Session["usuarioSelecionado"] = Session["login"].ToString();
            Session["statusSelecionado"] = "(Todos)";
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();

            Session["ListaRDV"] = ((List<RDV>)Session["ListaRDV"])
                .Where(w => w.Status != "Pendente")
                .OrderBy(o => o.DataRDV)
                .ToList();

            return View();
        }

        public ActionResult SearchRDVFechado(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = new DateTime(dataFinal.Year, dataFinal.Month,
                    DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month));
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["login"].ToString();
            string status = "(Todos)";

            #endregion

            Session["statusSelecionado"] = status;
            Session["ListaRDV"] = ListaRDV(dataInicial, dataFinal, usuario, status, "(Todos)", "Espécie");

            Session["ListaRDV"] = ((List<RDV>)Session["ListaRDV"])
                .Where(w => w.Status != "Pendente")
                .OrderBy(o => o.DataRDV)
                .ToList();

            return View("ListaRDVFechados");
        }

        public ActionResult DeleteRDVFechado(string numRDV)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["statusSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaVisualizaRDV"] = FilterListaRDV()
                .Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            ViewBag.Erro = "CONFIRMA A EXCLUSÃO DO FECHAMENTO DO RDV ABAIXO??? SERÁ EXCLUÍDO SOMENTE O FECHAMENTO!!!"
                + " OS LANÇAMENTOS CONTINUARAM COMO PENDENTES!!!";
            ViewBag.Metodo = "DeleteRDVFechado";
            ViewBag.Titulo = "Exclui Fechamento do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

            return View("VisualizaRDV");
        }

        public ActionResult ConfirmaDeleteRDVFechado()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string numRDV = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            List<RDV> listaRDV = hlbapp.RDV
                .Where(w => w.NumeroFechamentoRDV == numRDV)
                .ToList();

            foreach (var item in listaRDV)
            {
                item.Status = "Pendente";
                item.NumeroFechamentoRDV = null;

                #region Insere LOG Lançamento

                InsereLOGLancamentoRDV(item, "Exclusão Fechamento");

                #endregion
            }

            hlbapp.SaveChanges();

            #region Insere LOG

            LOG_RDV log = new LOG_RDV();
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.Operacao = "Exclusão Fechamento";
            log.NumeroFechamentoRDV = numRDV;
            log.Status = "Pendente";

            hlbapp.LOG_RDV.AddObject(log);

            hlbapp.SaveChanges();

            #endregion

            Session["statusSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();

            Session["ListaRDV"] = ((List<RDV>)Session["ListaRDV"])
                .Where(w => w.Status != "Pendente")
                .OrderBy(o => o.DataRDV)
                .ToList();

            return View("ListaRDVFechados");
        }

        public ActionResult FechaRDVPendentes()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<RDV> listaRDV = (List<RDV>)Session["ListaVisualizaRDV"];

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2 = new Apolo10Entities();

            var listaMoedas = listaRDV
                .GroupBy(g => new
                    {
                        g.IndEconNome,
                        g.FormaPagamento
                    })
                .Select(s => new
                    {
                        s.Key.IndEconNome,
                        s.Key.FormaPagamento,
                        ValorReal = s.Sum(m => m.ValorDespesa),
                        ValorMoedaEstrangeira = s.Sum(m => m.ValorMoedaEstrangeira)
                    })
                .OrderBy(o => o.IndEconNome).ThenBy(t => t.FormaPagamento)
                .ToList();

            string status = "Fechado";
            if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                .GetGroup("HLBAPPM-RDVSemAprovacao", (System.Collections.ArrayList)Session["Direitos"]))
                status = "Aprovado";

            foreach (var moeda in listaMoedas)
            {
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo2.gerar_codigo("1", "GERA_FECH_RDV_WEB", numero);

                var listaRDVItens = listaRDV.Where(w => w.IndEconNome == moeda.IndEconNome
                    && w.FormaPagamento == moeda.FormaPagamento).ToList();

                foreach (var item in listaRDVItens)
                {
                    RDV rdvAtualiza = hlbapp.RDV.Where(w => w.ID == item.ID).FirstOrDefault();
                    rdvAtualiza.NumeroFechamentoRDV = numero.Value.ToString();
                    rdvAtualiza.Status = status;

                    if (status == "Aprovado")
                    {
                        rdvAtualiza.DataAprovacao = DateTime.Now;
                        rdvAtualiza.UsuarioAprovacao = Session["login"].ToString().ToUpper();
                    }

                    #region Insere LOG Lançamento

                    InsereLOGLancamentoRDV(rdvAtualiza, "Fechamento RDV");

                    #endregion
                }

                #region Enviar E-mail

                RDV rdv = listaRDVItens.FirstOrDefault();

                FUNCIONARIO gerente = apolo2.FUNCIONARIO
                    .Where(w => apolo2.GRP_FUNC
                        .Any(a => a.FuncCod == w.FuncCod
                            && a.GrpFuncObs == "RDV"
                            && apolo2.FUNCIONARIO
                                .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == rdv.Usuario)))
                    .FirstOrDefault();

                if (gerente != null)
                {
                    USUARIO usuarioGerente = apolo2.USUARIO.Where(w => w.UsuCod == gerente.UsuCod).FirstOrDefault();
                    USUARIO usuario = apolo2.USUARIO.Where(w => w.UsuCod == rdv.Usuario).FirstOrDefault();

                    string paraNome = gerente.FuncNome;
                    string paraEmail = usuarioGerente.UsuEmail;
                    //string copiaPara = usuario.UsuEmail;
                    //string paraNome = "Paulo Alves";
                    //string paraEmail = "palves@hyline.com.br";
                    string copiaPara = "";
                    string assunto = "RDV " + numero.Value + " - " + rdv.NomeUsuario + " P/ APROVAÇÃO";
                    string stringChar = "" + (char)13 + (char)10;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "";
                    if (rdv.Empresa == "BR") empresaApolo = "5";
                    else if (rdv.Empresa == "LB") empresaApolo = "7";
                    else if (rdv.Empresa == "HN") empresaApolo = "14";
                    else if (rdv.Empresa == "PL") empresaApolo = "20";

                    string porta = "";
                    //if (Request.Url.Port != 80)
                        //porta = ":" + Request.Url.Port.ToString();

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "O RDV " + numero.Value + " do funcionário " + usuario.UsuNome
                        + " foi fechado e está disponível para aprovação. " + stringChar + stringChar
                        + "Clique no link a seguir para poder realizar a aprovação: "
                        + "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                        + stringChar + stringChar
                        + "SISTEMA WEB";

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);
                }
                else
                {
                    bdApoloEntities apolo = new bdApoloEntities();

                    VENDEDOR gerenteVendedor = apolo.VENDEDOR
                        .Where(w => apolo.SUP_VENDEDOR
                            .Any(a => a.SupVendCod == w.VendCod
                                && apolo.VENDEDOR.Any(n => n.VendCod == a.VendCod
                                    && n.USERLoginSite.Trim() == rdv.Usuario)
                                && a.FxaCod.Equals("0000002")))
                        .FirstOrDefault();

                    if (gerenteVendedor != null)
                    {
                        USUARIO usuarioGerente = apolo2.USUARIO.Where(w => w.UsuCod == gerenteVendedor.UsuCod)
                            .FirstOrDefault();
                        VENDEDOR usuarioVendedor = apolo.VENDEDOR
                            .Where(w => w.USERLoginSite.Trim() == rdv.Usuario).FirstOrDefault();

                        string paraNome = gerenteVendedor.VendNome;
                        string paraEmail = usuarioGerente.UsuEmail;
                        //string copiaPara = usuario.UsuEmail;
                        //string paraNome = "Paulo Alves";
                        //string paraEmail = "palves@hyline.com.br";
                        string copiaPara = "";
                        string assunto = "RDV " + numero.Value + " - " + rdv.NomeUsuario + " P/ APROVAÇÃO";
                        string stringChar = "" + (char)13 + (char)10;
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "";
                        if (rdv.Empresa == "BR") empresaApolo = "5";
                        else if (rdv.Empresa == "LB") empresaApolo = "7";
                        else if (rdv.Empresa == "HN") empresaApolo = "14";
                        else if (rdv.Empresa == "PL") empresaApolo = "20";

                        string porta = "";
                        //if (Request.Url.Port != 80)
                            //porta = ":" + Request.Url.Port.ToString();

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "O RDV " + numero.Value + " do funcionário " + usuarioVendedor.VendNome
                            + " foi fechado e está disponível para aprovação. " + stringChar + stringChar
                            + "Clique no link a seguir para poder realizar a aprovação: "
                            + "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);
                    }
                }

                #endregion

                hlbapp.SaveChanges();

                #region Insere LOG

                LOG_RDV log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                log.Operacao = "Inclusão";
                log.NumeroFechamentoRDV = numero.Value.ToString();
                log.Status = status;

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                #endregion
            }

            Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["statusSelecionado"] = "Pendente";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();
            ViewBag.Mensagem = "Lançamentos Pendentes Fechado!";

            return View("ListaLancamentosPendentes");
        }

        #endregion

        #region RDV - Financeiro

        #region CRUD Methods

        public ActionResult CreateAdiantamento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["ListaIndiceEconomico"] = CarregaListaIndicesEconomicos(true);
            Session["ListaFuncionarios"] = CarregaListaFuncionarios(false);
            Session["ListaTipoDespesaRDV"] = CarregaListaTipoDespesa("(C");
            CarregaEmpresas();

            #region Recupera Dados do último lançamento

            if (Session["dataUltimoLancamentoRDV"] != null)
                Session["dataRDV"] = Session["dataUltimoLancamentoRDV"];
            if (Session["valorUltimoLancamentoRDV"] != null)
                Session["valorDespesaRDV"] = Session["valorUltimoLancamentoRDV"];
            if (Session["valorMoedaEstrangeiraUltimoLancamentoRDV"] != null)
                Session["valorMoedaEstrangeiraRDV"] = Session["valorMoedaEstrangeiraUltimoLancamentoRDV"];
            if (Session["tipoDespesaUltimoLancamentoRDV"] != null)
                AtualizaDDL(Session["tipoDespesaUltimoLancamentoRDV"].ToString(),
                    (List<SelectListItem>)Session["ListaTipoDespesaRDV"]);

            #endregion

            return View();
        }

        public ActionResult EditAdiantamento(int id)
        {
            CleanSessions();

            Session["idSelecionado"] = id;

            //Session["ListaLocaisRDV"] = Session["ListaLocaisOriginal"];
            Session["ListaFormaPag"] = CarregaListaFormaPagamento();
            Session["ListaOrigem"] = CarregaListaOrigem();
            Session["ListaIndiceEconomico"] = CarregaListaIndicesEconomicos(true);
            Session["ListaFuncionarios"] = CarregaListaFuncionarios(false);
            CarregaEmpresas();

            CarregaRDV(id);

            return View("CreateAdiantamento");
        }

        public ActionResult ConfirmaDeleteAdiantamento(int id)
        {
            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteAdiantamento()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            RDV rdv = hlbapp.RDV.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.RDV.DeleteObject(rdv);
            hlbapp.SaveChanges();

            Session["ListaRDV"] = FilterListaRDV();

            return View("ListaAdiantamentos");
        }

        public ActionResult SaveAdiantamento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2 = new Apolo10Entities();
            bdApoloEntities apolo = new bdApoloEntities();

            if (model["dataRDV"] != null)
            {
                #region Carrega Valores

                string empresa = "";
                string usuario = "";
                string nomeUsuario = "";
                if (model["Usuario"] != null)
                {
                    usuario = model["Usuario"];
                    Session["usuarioRDV"] = usuario;
                    AtualizaDDL(usuario, (List<SelectListItem>)Session["ListaFuncionarios"]);

                    #region Verifica Funcionario

                    FUNCIONARIO funcionarioApolo = apolo2.FUNCIONARIO
                        .Where(w => w.UsuCod == usuario
                            && w.USERParticipaControleRDVWeb.Equals("Sim")).FirstOrDefault();

                    if (funcionarioApolo != null)
                    {
                        empresa = funcionarioApolo.USEREmpres;
                        nomeUsuario = funcionarioApolo.FuncNome.ToUpper();
                    }

                    #endregion

                    #region Verifica Vendedor

                    VENDEDOR vendedorApolo = apolo.VENDEDOR
                        .Where(w => w.USERLoginSite == usuario).FirstOrDefault();

                    if (vendedorApolo != null)
                    {
                        nomeUsuario = vendedorApolo.VendNome.ToUpper();

                        Empresas empresaObj = hlbapp.Empresas
                            .Where(w => w.DescricaoApoloVendedor == vendedorApolo.USEREmpresa)
                            .FirstOrDefault();

                        if (empresaObj != null) empresa = empresaObj.CodigoCHIC;
                    }

                    #endregion

                    if (empresa == null || empresa == "")
                    {
                        ViewBag.Erro = "O Funcionário ou Vendedor não tem Empresa relacionada no Apolo!"
                            + " Para Funcionário, acessar o Apolo em Cadastros / Funcionário / Funcionários e na aba "
                            + "'Dados de Conta Celular VIVO' selecionar a Empresa no campo 'Empresa'!"
                            + " Para Vendedor, acessar o Apolo em Cadastros / Vendas / Vendedores e na aba "
                            + "'Comissao' selecionar a Empresa no campo 'Empresa do Vendedor'!";
                        return View("CreateAdiantamento");
                    }
                }

                DateTime dataRDV = new DateTime();
                if (model["dataRDV"] != null)
                {
                    dataRDV = Convert.ToDateTime(model["dataRDV"]);
                    Session["dataRDV"] = dataRDV.ToShortDateString();
                }
                else
                    dataRDV = Convert.ToDateTime(Session["dataRDV"].ToString());

                string tipoDespesa = "ADIANTAMENTO (C)";
                Session["TipoDespesaSelecionadaRDV"] = tipoDespesa;

                string formaPag = "Espécie";
                Session["FormaPagamentoSelecionadaRDV"] = formaPag;

                string descricao = "";
                if (model["descricao"] != null)
                {
                    descricao = model["descricao"];
                    Session["descricaoRDV"] = descricao;
                }
                else
                    descricao = Session["descricaoRDV"].ToString();

                //string local = "";
                //if (model["Local"] != null)
                //{
                //    local = model["Local"];
                //    Session["LocalSelecionado"] = local;
                //    AtualizaDDL(local, (List<SelectListItem>)Session["ListaLocaisRDV"]);
                //}
                //else
                //    local = Session["LocalSelecionado"].ToString();

                decimal valorDespesa = 0;
                if (model["valorDespesa"] != null)
                {
                    if (Decimal.TryParse(model["valorDespesa"], out valorDespesa))
                    {
                        if (valorDespesa <= 0)
                        {
                            ViewBag.Erro = "O Valor da Despesa tem que ser maior que zero!";
                            return View("CreateAdiantamento");
                        }
                    }
                    else
                    {
                        ViewBag.Erro = "O Valor da Despesa tem que ser maior que zero!";
                        return View("CreateAdiantamento");
                    }

                    Session["valorDespesaRDV"] = valorDespesa;
                }

                string indEcon = "";
                if (model["IndiceEconomico"] != null)
                {
                    indEcon = model["IndiceEconomico"];
                    Session["IndiceEconomicoSelecionadoRDV"] = indEcon;
                    AtualizaDDL(indEcon, (List<SelectListItem>)Session["ListaIndiceEconomico"]);
                }
                else
                    indEcon = Session["IndiceEconomicoSelecionadoRDV"].ToString();

                decimal valorMoedaEstrangeira = 0;
                string origem = "Nacional";
                if (!indEcon.Equals("0000001"))
                {
                    origem = "Internacional";
                    valorMoedaEstrangeira = valorDespesa;
                    valorDespesa = 0;
                }
                Session["OrigemSelecionadaRDV"] = origem;

                string cidade = origem;
                string pais = origem;
                
                string motivo = "";
                
                #endregion

                RDV rdv = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                    rdv = new RDV();
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    rdv = hlbapp.RDV.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                rdv.Empresa = empresa;
                if (usuario == "")
                {
                    rdv.Usuario = Session["login"].ToString().ToUpper();
                    rdv.NomeUsuario = Session["usuario"].ToString().ToUpper();
                }
                else
                {
                    rdv.Usuario = usuario;
                    rdv.NomeUsuario = nomeUsuario;
                }
                rdv.DataHora = DateTime.Now;
                rdv.DataRDV = dataRDV;
                rdv.TipoDespesa = tipoDespesa;
                rdv.FormaPagamento = formaPag;
                rdv.Descricao = descricao;
                //rdv.CodCidade = local;
                //rdv.NomeCidade = apolo.CIDADE.Where(w => w.CidCod == rdv.CodCidade).FirstOrDefault().CidNomeComp;
                rdv.CodCidade = pais;
                rdv.NomeCidade = cidade;
                if (rdv.NomeCidade == "Nacional")
                {
                    rdv.CodPais = "BR";
                    rdv.NomePais = "BRASIL";
                }
                else
                {
                    rdv.CodPais = "";
                    rdv.NomePais = "";
                }
                rdv.ValorDespesa = valorDespesa;
                rdv.ValorMoedaEstrangeira = valorMoedaEstrangeira;
                if (valorMoedaEstrangeira > 0)
                {
                    rdv.IndEconCod = indEcon;
                    var listaIndEcon = (List<SelectListItem>)Session["ListaIndiceEconomico"];
                    rdv.IndEconNome = listaIndEcon.Where(w => w.Value == indEcon).FirstOrDefault().Text;
                }
                else
                {
                    rdv.IndEconCod = null;
                    rdv.IndEconNome = null;
                }
                rdv.Motivo = motivo;

                if ((rdv.NumeroFechamentoRDV == null) || (rdv.NumeroFechamentoRDV == ""))
                    rdv.Status = "Pendente";
                else
                    rdv.Status = "Fechado";

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.RDV.AddObject(rdv);

                #region Recupera Dados do último lançamento

                Session["dataUltimoLancamentoRDV"] = rdv.DataRDV;
                Session["valorUltimoLancamentoRDV"] = rdv.ValorDespesa;
                Session["valorMoedaEstrangeiraUltimoLancamentoRDV"] = rdv.ValorMoedaEstrangeira;
                Session["tipoDespesaUltimoLancamentoRDV"] = rdv.TipoDespesa;

                #endregion

                hlbapp.SaveChanges();

                #region Enviar E-mail

                USUARIO usuarioObj = apolo2.USUARIO.Where(w => w.UsuCod == rdv.Usuario).FirstOrDefault();

                string simboloIndEconomico = "R$";
                decimal valorAdiantamento = rdv.ValorDespesa;
                if (rdv.IndEconCod != null)
                {
                    IND_ECONOMICO indEconomico = apolo2.IND_ECONOMICO
                        .Where(w => w.IndEconCod == rdv.IndEconCod).FirstOrDefault();
                    simboloIndEconomico = indEconomico.IndEconSimb;
                    valorAdiantamento = Convert.ToDecimal(rdv.ValorMoedaEstrangeira);
                }

                string paraNome = usuarioObj.UsuNome;
                string paraEmail = usuarioObj.UsuEmail;
                string copiaPara = "financeiro@hyline.com.br";
                string assunto = "ADIANTAMENTO " + rdv.ID.ToString() + " - " + simboloIndEconomico + " " + String.Format("{0:N2}", valorAdiantamento);
                string stringChar = "" + (char)13 + (char)10;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "";
                if (rdv.Empresa == "BR") empresaApolo = "5";
                else if (rdv.Empresa == "LB") empresaApolo = "7";
                else if (rdv.Empresa == "HN") empresaApolo = "14";
                else if (rdv.Empresa == "PL") empresaApolo = "20";

                //string porta = "";
                //if (Request.Url.Port != 80)
                    //porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "Adiantamento de " + simboloIndEconomico + " " + String.Format("{0:N2}", valorAdiantamento)
                    + " com o ID " + rdv.ID.ToString() +  " realizado." + stringChar + stringChar
                    + "Departamento Financeiro. " + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                #endregion
            }

            Session["ListaRDV"] = FilterListaRDV();
            return View("ListaAdiantamentos");
        }

        public ActionResult PrintAdiantamento(int id, bool download)
        {
            if (VerificaSessao()) { return RedirectToAction("Login", "AccountMobile"); }

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            string caminho = @"\\srv-riosoft-01\W\RDVs\Adiantamento_" + Session["login"].ToString() + "_"
                + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";

            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport =
                new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            MyReport.Load(Server.MapPath("~/Reports/Adiantamento.rpt"));

            MyReport.ParameterFields["ID"].CurrentValues.AddValue(id);

            if (download)
            {
                Stream stream = MyReport.ExportToStream(CrystalDecisions.Shared.ExportFormatType
                    .PortableDocFormat);
                return File(stream, "application/pdf", "Adiantamento_" + id.ToString() + ".pdf");
            }
            else
            {
                var response = System.Web.HttpContext.Current.Response;
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                    response, false, "Adiantamento_" + id.ToString());
                return new EmptyResult();
            }

            MyReport.Close();
            MyReport.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

        public ActionResult ListaAdiantamentos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["tipoLancamentoSelecionado"] = "(C";
            if (Session["usuarioSelecionado"] == null) Session["usuarioSelecionado"] = "(Todos)";
            if (Session["statusSelecionado"] == null) Session["statusSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "Espécie";
            Session["ListaRDV"] = FilterListaRDV();

            return View();
        }

        public ActionResult SearchAdiantamentos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["usuarioSelecionado"].ToString();
            if (model["Usuario"] != null)
            {
                usuario = model["Usuario"];
                AtualizaDDL(usuario, (List<SelectListItem>)Session["ListaFuncionariosPesquisa"]);
                Session["usuarioSelecionado"] = usuario;
            }

            string tipoLancamento = "(C";

            string status = Session["statusSelecionado"].ToString();
            if (model["Status"] != null)
            {
                status = model["Status"];
                AtualizaDDL(status, (List<SelectListItem>)Session["ListaStatus"]);
                Session["statusSelecionado"] = status;
            }

            #endregion

            Session["ListaRDV"] = ListaRDV(dataInicial, dataFinal, usuario, status, tipoLancamento, "Espécie");

            return View("ListaAdiantamentos");
        }

        public ActionResult ListaRDVFechadosGeral()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();
            if (Session["usuarioSelecionado"] == null) Session["usuarioSelecionado"] = "(Todos)";
            if (Session["statusSelecionado"] == null) Session["statusSelecionado"] = "(Todos)";
            if (Session["tipoLancamentoSelecionado"] == null) Session["tipoLancamentoSelecionado"] = "(Todos)";
            if (Session["formaPagamentoSelecionada"] == null) Session["formaPagamentoSelecionada"] = "(Todas)";
            Session["ListaRDV"] = FilterListaRDV();

            return View();
        }

        public ActionResult SearchRDVFechadoGeral(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = new DateTime(dataFinal.Year, dataFinal.Month,
                    DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month));
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["usuarioSelecionado"].ToString();
            if (model["Usuario"] != null)
            {
                usuario = model["Usuario"];
                AtualizaDDL(usuario, (List<SelectListItem>)Session["ListaFuncionariosPesquisa"]);
                Session["usuarioSelecionado"] = usuario;
            }

            string status = Session["statusSelecionado"].ToString();
            if (model["Status"] != null)
            {
                status = model["Status"];
                AtualizaDDL(status, (List<SelectListItem>)Session["ListaStatus"]);
                Session["statusSelecionado"] = status;
            }

            #endregion

            Session["ListaRDV"] = ListaRDV(dataInicial, dataFinal, usuario, status, "(Todos)", "(Todas)");

            return View("ListaRDVFechadosGeral");
        }

        public ActionResult VisualizaRDVFechadoGeral(string numRDV)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"] != "")
                {
                    List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
                    if (VerificaGerencia(Session["login"].ToString().ToUpper(), rdvUrl.FirstOrDefault().Usuario.ToUpper()))
                    {
                        Session["dataInicialRDV"] = rdvUrl.Min(m => m.DataRDV);
                        Session["dataFinalRDV"] = rdvUrl.Max(m => m.DataRDV);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["urlChamada"] = "";
                    }
                    else
                    {
                        ViewBag.Erro = "Você não tem acesso a esse RDV, pois você não tem permissão para aprová-lo!";

                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "Fechado";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["ListaRDV"] = FilterListaRDV();

                        return View("ListaRDVParaAprovacao");
                    }
                }
            }

            Session["ListaVisualizaRDV"] = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            ViewBag.Metodo = "VisualizaRDVFechadoGeral";
            ViewBag.Titulo = "Visualização do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

            return View("VisualizaRDV");
        }

        #region Relatórios Financeiro - Excel

        #region Relatório de Cartão Corporativo

        public ActionResult GerarRelatorioCartaoCorporativo()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            int anoMesInicial = Convert.ToInt32(Convert.ToDateTime(Session["dataInicialRDV"].ToString())
                .ToString("yyyyMM"));
            int anoMesFinal = Convert.ToInt32(Convert.ToDateTime(Session["dataFinalRDV"].ToString())
                .ToString("yyyyMM"));

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\RDV";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\RDV\\RDV_Cartao_Corporativo_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*RDV_Cartao_Corporativo_" + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioCartaoCorporativoExcel(pesquisa, true, pasta, destino,
                anoMesInicial, anoMesFinal);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RDV_Cartao_Corporativo_" + anoMesInicial.ToString() + "_" + anoMesFinal.ToString() + ".xlsx");
        }

        public string GeraRelatorioCartaoCorporativoExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, int anoMesInicial, int anoMesFinal)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\RDV\\RDV_Cartao_Corporativo.xlsx", destino);

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

            #region Carrega Empresas

            string empresas = "";
            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                string empresa = Session["empresa"].ToString().Substring(i, 2);
                empresas = empresas + "'" + empresa + "'";
                if (empresa != Session["empresa"].ToString().Substring(Session["empresa"].ToString().Length - 2, 2))
                    empresas = empresas + ",";
            }

            #endregion

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Rel_Fatura_Cartao_Corporativo V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string commandTextCHICCondicaoParametros =
                    "V.[Ano Mês] between '" + anoMesInicial.ToString() + "' and '" + anoMesFinal.ToString() + "' and " +
                    "V.Empresa in (" + empresas + ") ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1, 3";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("HLBAPP"))
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

        #region Event Methods

        #region Recebimento do RDV

        public ActionResult RecebimentoRDV(string numRDV)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"] != "")
                {
                    List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
                    if (VerificaGerencia(Session["login"].ToString().ToUpper(), rdvUrl.FirstOrDefault().Usuario.ToUpper()))
                    {
                        Session["dataInicialRDV"] = rdvUrl.Min(m => m.DataRDV);
                        Session["dataFinalRDV"] = rdvUrl.Max(m => m.DataRDV);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["urlChamada"] = "";
                    }
                    else
                    {
                        ViewBag.Erro = "Você não tem acesso a esse RDV, pois você não tem permissão para aprová-lo!";

                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "Fechado";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["ListaRDV"] = FilterListaRDV();

                        return View("ListaRDVParaAprovacao");
                    }
                }
            }

            Session["ListaVisualizaRDV"] = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            #region Verifica se o funcionário tem matrícula

            var rdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).FirstOrDefault();
            Apolo10Entities apolo2Session = new Apolo10Entities();
            FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()).FirstOrDefault();
            if (func != null && rdv.FormaPagamento == "Espécie")
            {
                if (func.FuncCodMix == null)
                {
                    ViewBag.Erro = "Cadastro do Funcionário no Apolo sem a Matrícula do Mix! Verifique!";
                    Session["ListaRDV"] = FilterListaRDV();
                    return View("ListaRDVFechadosGeral");
                }
            }

            #endregion

            #region Verifica se já existe RDV lançado no Apolo (Somente Espécie)

            if (rdv.FormaPagamento != "Cartão Corp.")
            {
                ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                    .Where(w => w.MovEstqDocEspec == "RDVWB"
                        //&& w.MovEstqDocSerie == "1" 
                        && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                        && w.TipoLancCod == "E0000444")
                    .FirstOrDefault();

                if (movEstq != null)
                {
                    ViewBag.Erro = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + movEstq.EmpCod + " - Chave Mov. Estq.: " + movEstq.MovEstqChv.ToString() + ")!";
                    Session["ListaRDV"] = FilterListaRDV();
                    return View("ListaRDVFechadosGeral");
                }
            }

            #endregion

            ViewBag.Erro = "CONFIRMA O RECEBIMENTO DO RDV ABAIXO???";
            ViewBag.Metodo = "RecebimentoRDV";
            ViewBag.Titulo = "Recebimento do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

            return View("VisualizaRDV");
        }

        public ActionResult ConfirmaRecebimentoRDV()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string numRDV = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2Session = new Apolo10Entities();
            List<RDV> listaRDV = hlbapp.RDV
                .Where(w => w.NumeroFechamentoRDV == numRDV)
                .ToList();

            foreach (var item in listaRDV)
            {
                item.Status = "Recebido Financeiro";
            }

            hlbapp.SaveChanges();

            #region Insere LOG

            LOG_RDV log = new LOG_RDV();
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.Operacao = "Recebimento pelo Financeiro";
            log.NumeroFechamentoRDV = numRDV;
            log.Status = "Recebido Financeiro";

            hlbapp.LOG_RDV.AddObject(log);

            hlbapp.SaveChanges();

            #endregion

            #region Se for Espécie, será integrado com o Apolo automaticamente

            var rdv = listaRDV.FirstOrDefault();
            if (rdv.FormaPagamento == "Espécie")
            {
                #region Carrega matrícula do Funcionário

                int? matriculaMIX = 0;
                FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()
                    && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();
                if (func != null && rdv.FormaPagamento == "Espécie")
                {
                    matriculaMIX = func.FuncCodMix;
                }

                #endregion

                var existeApolo = false;
                var empresa = "";
                var chave = 0;

                if (matriculaMIX > 0)
                {
                    #region Verifica se já existe RDV lançado no Apolo

                    ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                    ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                        //.Where(w => (w.MovEstqDocEspec == "RELAT" || w.MovEstqDocEspec == "RECIB")
                        .Where(w => w.MovEstqDocEspec == "RDVWB" && w.MovEstqDocSerie == "1" && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                            && w.TipoLancCod == "E0000444")
                        .FirstOrDefault();

                    if (movEstq != null)
                    {
                        empresa = movEstq.EmpCod;
                        chave = movEstq.MovEstqChv;
                        existeApolo = true;
                    }

                    #endregion

                    if (!existeApolo)
                    {
                        var anoMes = rdv.DataRDV.ToString("MM/yyyy");
                        var valorReembolso = listaRDV.Sum(s => (s.TipoDespesa.Contains("(C") ? 0 : s.ValorDespesa));
                        USUARIO usuario = apolo2Session.USUARIO.Where(w => w.UsuCod == rdv.UsuarioAprovacao.ToUpper()).FirstOrDefault();
                        var aprovador = usuario.UsuNome;
                        if (valorReembolso > 0)
                        {
                            Models.ProceduresApolo.ProceduresApolo procApolo = new Models.ProceduresApolo.ProceduresApolo();
                            procApolo.USER_Insere_RDV(matriculaMIX, Convert.ToInt32(rdv.NumeroFechamentoRDV), anoMes, valorReembolso, aprovador,
                                Convert.ToDateTime(rdv.DataAprovacao).ToString("dd/MM/yyyy hh:mm"), DateTime.Today);
                        }
                    }
                }

                #region Insere LOG Integração Apolo

                log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                if (existeApolo)
                    log.Operacao = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + empresa + " - Chave Mov. Estq.: " + chave.ToString() + ")!";
                else
                    log.Operacao = "Integração com o Apolo realizada!";
                log.NumeroFechamentoRDV = numRDV;
                log.Status = "Recebido Financeiro";

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                ViewBag.Mensagem = log.Operacao;

                #endregion
            }

            #endregion

            Session["ListaRDV"] = FilterListaRDV();
            return View("ListaRDVFechadosGeral");
        }

        public ActionResult ImportaFaturaApolo(string numRDV)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2Session = new Apolo10Entities();

            #region Integra com o Apolo automaticamente

            var listaFaturaRDV = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
            var rdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).FirstOrDefault();

            #region Carrega matrícula do Funcionário

            int? matriculaMIX = 0;
            FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()
                && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();
            if (func != null)
            {
                matriculaMIX = func.FuncCodMix;
            }

            #endregion

            var existeApolo = false;
            var empresaIntegApolo = "";
            var chave = 0;

            if (matriculaMIX > 0)
            {
                #region Verifica se já existe RDV lançado no Apolo

                ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                    //.Where(w => (w.MovEstqDocEspec == "RELAT" || w.MovEstqDocEspec == "RECIB")
                    .Where(w => w.MovEstqDocEspec == "RDVWB" && w.MovEstqDocSerie == "1" && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                        && w.TipoLancCod == "E0000444")
                    .FirstOrDefault();

                if (movEstq != null)
                {
                    empresaIntegApolo = movEstq.EmpCod;
                    chave = movEstq.MovEstqChv;
                    existeApolo = true;
                }

                #endregion

                if (!existeApolo)
                {
                    var ano = Convert.ToInt32(rdv.AnoMes.ToString().Substring(0, 4));
                    var mes = Convert.ToInt32(rdv.AnoMes.ToString().Substring(4, 2));
                    var cDataL = (new DateTime(ano, mes, 1)).AddMonths(-1);
                    var dataLancamento = new DateTime(cDataL.Year, cDataL.Month, DateTime.DaysInMonth(cDataL.Year, cDataL.Month));

                    var anoMes = dataLancamento.ToString("MM/yyyy");
                    var valorReembolso = listaFaturaRDV.Sum(s => s.ValorDespesa);
                    if (valorReembolso > 0)
                    {
                        Models.ProceduresApolo.ProceduresApolo procApolo = new Models.ProceduresApolo.ProceduresApolo();
                        procApolo.USER_Insere_RDV(matriculaMIX, Convert.ToInt32(rdv.NumeroFechamentoRDV), anoMes, valorReembolso, "",
                            "", dataLancamento);
                    }
                }
            }

            #region Insere LOG Integração Apolo

            LOG_RDV log = new LOG_RDV();
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            if (existeApolo)
                log.Operacao = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + empresaIntegApolo + " - Chave Mov. Estq.: " + chave.ToString() + ")!";
            else
                log.Operacao = "Integração com o Apolo realizada!";
            log.NumeroFechamentoRDV = rdv.NumeroFechamentoRDV;
            log.Status = "Pendente";

            hlbapp.LOG_RDV.AddObject(log);

            hlbapp.SaveChanges();

            ViewBag.Mensagem = log.Operacao;

            #endregion

            #endregion

            Session["ListaRDV"] = FilterListaRDV();
            return View("ListaRDVFechadosGeral");
        }

        #endregion

        #region LOG

        public ActionResult LogRDV(string numRDV)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["ListaLOGRDV"] = hlbapp.LOG_RDV
                .Where(w => w.NumeroFechamentoRDV == numRDV)
                .OrderBy(o => o.DataHora)
                .ToList();

            return View();
        }

        #endregion

        #endregion

        #endregion

        #region RDV - Aprovação

        public ActionResult ListaRDVParaAprovacao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            ViewBag.Metodo = "AprovacaoRDV";

            Session["usuarioSelecionado"] = "(Todos)";
            Session["statusSelecionado"] = "Fechado";
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "(Todas)";
            Session["ListaRDV"] = FilterListaRDV();

            return View();
        }

        public ActionResult SearchRDVParaAprovacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialRDV"]);
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalRDV"]);
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = new DateTime(dataFinal.Year, dataFinal.Month,
                    DateTime.DaysInMonth(dataFinal.Year, dataFinal.Month));
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["usuarioSelecionado"].ToString();
            if (model["Usuario"] != null)
            {
                usuario = model["Usuario"];
                AtualizaDDL(usuario, (List<SelectListItem>)Session["ListaFuncionariosPesquisa"]);
                Session["usuarioSelecionado"] = usuario;
            }

            #endregion

            Session["statusSelecionado"] = "Fechado";
            Session["ListaRDV"] = ListaRDV(dataInicial, dataFinal, usuario,
                Session["statusSelecionado"].ToString(), "(Todos)", "(Todas)");

            return View("ListaRDVParaAprovacao");
        }

        public ActionResult AprovaRDVFechado(string numRDV)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"].ToString() != "")
                {
                    List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

                    if (rdvUrl.Count > 0)
                    {
                        if (VerificaGerencia(Session["login"].ToString().ToUpper(), rdvUrl.FirstOrDefault().Usuario.ToUpper()))
                        {
                            Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                            Session["dataInicialRDV"] = rdvUrl.Min(m => m.DataRDV);
                            Session["dataFinalRDV"] = rdvUrl.Max(m => m.DataRDV);
                            Session["usuarioSelecionado"] = "(Todos)";
                            Session["statusSelecionado"] = "(Todos)";
                            Session["tipoLancamentoSelecionado"] = "(Todos)";
                            Session["formaPagamentoSelecionada"] = "(Todas)";
                            Session["urlChamada"] = "";
                        }
                        else
                        {
                            ViewBag.Erro = "Você não tem acesso a esse RDV, pois você não tem permissão para aprová-lo!";

                            CleanSessions();

                            Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                            Session["usuarioSelecionado"] = "(Todos)";
                            Session["statusSelecionado"] = "Fechado";
                            Session["tipoLancamentoSelecionado"] = "(Todos)";
                            Session["formaPagamentoSelecionada"] = "(Todas)";
                            Session["ListaRDV"] = FilterListaRDV();

                            return View("ListaRDVParaAprovacao");
                        }
                    }
                    else
                    {
                        ViewBag.Erro = "O RDV não existe mais! Provavelmente o Usuário já o deletou!";

                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = "(Todos)";
                        Session["statusSelecionado"] = "Fechado";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["formaPagamentoSelecionada"] = "(Todas)";
                        Session["ListaRDV"] = FilterListaRDV();

                        return View("ListaRDVParaAprovacao");
                    }
                }
            }

            //Session["ListaVisualizaRDV"] = FilterListaRDV()
            //    .Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            Session["ListaVisualizaRDV"] = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            ViewBag.Metodo = "AprovaRDV";
            ViewBag.Titulo = "Aprovação do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaVisualizaRDV"]).FirstOrDefault().NomeUsuario;

            return View("VisualizaRDV");
        }

        public ActionResult AprovarRDVFechado()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2 = new Apolo10Entities();

            List<RDV> listaRDV = (List<RDV>)Session["ListaVisualizaRDV"];

            foreach (var item in listaRDV)
            {
                RDV rdAtualiza = hlbapp.RDV.Where(w => w.ID == item.ID).FirstOrDefault();
                rdAtualiza.UsuarioAprovacao = Session["login"].ToString();
                rdAtualiza.DataAprovacao = DateTime.Now;
                rdAtualiza.Status = "Aprovado";

                #region Insere LOG Lançamento

                InsereLOGLancamentoRDV(rdAtualiza, "Aprovação");

                #endregion
            }

            hlbapp.SaveChanges();

            #region Insere LOG

            LOG_RDV log = new LOG_RDV();
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.Operacao = "Aprovação";
            log.NumeroFechamentoRDV = listaRDV.FirstOrDefault().NumeroFechamentoRDV;
            log.Status = "Aprovado";

            hlbapp.LOG_RDV.AddObject(log);

            hlbapp.SaveChanges();

            #endregion

            #region Enviar E-mail

            RDV rdv = listaRDV.FirstOrDefault();

            FUNCIONARIO gerente = apolo2.FUNCIONARIO
                .Where(w => apolo2.GRP_FUNC
                    .Any(a => a.FuncCod == w.FuncCod
                        && a.GrpFuncObs == "RDV"
                        && apolo2.FUNCIONARIO
                            .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == rdv.Usuario)))
                .FirstOrDefault();

            if (gerente != null)
            {
                USUARIO usuarioGerente = apolo2.USUARIO.Where(w => w.UsuCod == gerente.UsuCod).FirstOrDefault();
                USUARIO usuario = apolo2.USUARIO.Where(w => w.UsuCod == rdv.Usuario).FirstOrDefault();

                string paraNome = usuario.UsuNome;
                string paraEmail = usuario.UsuEmail;
                //string copiaPara = usuarioGerente.UsuEmail;
                //string paraNome = "Paulo Alves";
                //string paraEmail = "palves@hyline.com.br";
                string copiaPara = "";
                string assunto = "RDV " + rdv.NumeroFechamentoRDV + " - " + rdv.NomeUsuario + " APROVADO";
                string stringChar = "" + (char)13 + (char)10;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "";
                if (rdv.Empresa == "BR") empresaApolo = "5";
                else if (rdv.Empresa == "LB") empresaApolo = "7";
                else if (rdv.Empresa == "HN") empresaApolo = "14";
                else if (rdv.Empresa == "PL") empresaApolo = "20";

                //string porta = "";
                //if (Request.Url.Port != 80)
                    //porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "O RDV " + rdv.NumeroFechamentoRDV
                    + " foi aprovado e está disponível para impressão. " + stringChar + stringChar
                    + "Imprima o PDF em anexo e envie junto com os recibos ao "
                    + "Departamento Financeiro. " + stringChar + stringChar
                    //+ "Visualizar RDV no Navegador: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/PrintRDV?numRDV=" + rdv.NumeroFechamentoRDV
                    //    + "&download=False" + stringChar
                    //+ "Download do RDV: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/PrintRDV?numRDV=" + rdv.NumeroFechamentoRDV
                    //    + "&download=True"
                    //+ stringChar + stringChar
                    + "SISTEMA WEB";

                anexos = GeraRDVPDF(rdv.NumeroFechamentoRDV);

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);
            }
            else
            {
                bdApoloEntities apolo = new bdApoloEntities();

                VENDEDOR gerenteVendedor = apolo.VENDEDOR
                    .Where(w => apolo.SUP_VENDEDOR
                        .Any(a => a.SupVendCod == w.VendCod
                            && apolo.VENDEDOR.Any(n => n.VendCod == a.VendCod
                                && n.USERLoginSite.Trim() == rdv.Usuario)
                            && a.FxaCod.Equals("0000002")))
                    .FirstOrDefault();

                if (gerenteVendedor != null)
                {
                    USUARIO usuarioGerente = apolo2.USUARIO.Where(w => w.UsuCod == gerenteVendedor.UsuCod)
                        .FirstOrDefault();
                    VENDEDOR usuarioVendedor = apolo.VENDEDOR
                        .Where(w => w.USERLoginSite.Trim() == rdv.Usuario).FirstOrDefault();

                    string paraNome = usuarioVendedor.VendNome;
                    string paraEmail = usuarioVendedor.USERLoginSite;
                    //string copiaPara = usuarioGerente.UsuEmail;
                    //string paraNome = "Paulo Alves";
                    //string paraEmail = "palves@hyline.com.br";
                    string copiaPara = "";
                    string assunto = "RDV " + rdv.NumeroFechamentoRDV + " - " + rdv.NomeUsuario + " APROVADO";
                    string stringChar = "" + (char)13 + (char)10;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "";
                    if (rdv.Empresa == "BR") empresaApolo = "5";
                    else if (rdv.Empresa == "LB") empresaApolo = "7";
                    else if (rdv.Empresa == "HN") empresaApolo = "14";
                    else if (rdv.Empresa == "PL") empresaApolo = "20";

                    string porta = "";
                    //if (Request.Url.Port != 80)
                        //porta = ":" + Request.Url.Port.ToString();

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "O RDV " + rdv.NumeroFechamentoRDV
                        + " foi aprovado e está disponível para impressão. " + stringChar + stringChar
                        + "Imprima o PDF em anexo e envie junto com os recibos ao "
                        + "Departamento Financeiro. " + stringChar + stringChar
                        //+ "Visualizar RDV no Navegador: "
                        //+ "http://" + Request.Url.Host + porta + "/RDV/PrintRDV?numRDV=" + rdv.NumeroFechamentoRDV
                        //    + "&download=False" + stringChar
                        //+ "Download do RDV: "
                        //+ "http://" + Request.Url.Host + porta + "/RDV/PrintRDV?numRDV=" + rdv.NumeroFechamentoRDV
                        //    + "&download=True"
                        //+ stringChar + stringChar
                        + "SISTEMA WEB";

                    anexos = GeraRDVPDF(rdv.NumeroFechamentoRDV);

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);
                }
            }

            #endregion

            Session["usuarioSelecionado"] = "(Todos)";
            Session["statusSelecionado"] = "Fechado";
            Session["tipoLancamentoSelecionado"] = "(Todos)";
            Session["formaPagamentoSelecionada"] = "(Todas)";
            Session["ListaRDV"] = FilterListaRDV();

            return View("ListaRDVParaAprovacao");
        }

        #endregion

        #region RDV - Cartão Corporativo - Importação

        #region Bradesco

        public ActionResult FaturaCartaoCorporativoBradescoEXCEL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportaFaturaCartaoCorporativoBradescoEXCEL(HttpPostedFileBase file)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "AccountMobile");

            #region Salva Arquivo no Disco

            string caminho = @"C:\inetpub\wwwroot\Relatorios\FaturaCartaoCorporativoBradescoEXCEL_"
                + Session["login"].ToString() + Session.SessionID + ".xls";

            file.SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            int linhaErro = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2Session = new Apolo10Entities();

            #endregion

            try
            {
                #region Abre arquivo Excel e carrega lista de Planilhas

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                int existe = 0;

                string mesAno = "";
                string funcionario = "";
                string ano = "";
                string empresa = "";
                string usuario = "";
                string nomeUsuario = "";
                string vencimento = "";
                string numeroCartao = "";

                #endregion

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    #region Carrega Linhas da Planilha

                    string nomeAba = planilha.Name;

                    //string entrou = "";
                    //if (nomeAba.Equals("page 2"))
                    //    entrou = "1";

                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                        .Where(s => s.Name == planilha.Name)
                        .First().Id;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    #endregion

                    #region Carrega campo Mês Ano, Funcionário e Vencimento

                    if (nomeAba.Equals("page 1"))
                    {
                        Row linhaMesAno = sheetData.Elements<Row>().Where(r => r.RowIndex == 11).First();
                        Cell celulaMesAno = linhaMesAno.Elements<Cell>().Where(c => c.CellReference == "D11")
                            .FirstOrDefault();
                        if (celulaMesAno == null)
                            celulaMesAno = linhaMesAno.Elements<Cell>().Where(c => c.CellReference == "C11")
                                .FirstOrDefault();

                        Row linhaFuncionario = sheetData.Elements<Row>().Where(r => r.RowIndex == 13).First();
                        Cell celulaFuncionario = linhaFuncionario.Elements<Cell>()
                            .Where(c => c.CellReference == "D13").FirstOrDefault();
                        if (celulaFuncionario == null)
                            celulaFuncionario = linhaFuncionario.Elements<Cell>()
                                .Where(c => c.CellReference == "C13").FirstOrDefault();

                        Row linhaNumeroCartao = sheetData.Elements<Row>().Where(r => r.RowIndex == 14).First();
                        Cell celulaNumeroCartao = linhaNumeroCartao.Elements<Cell>()
                            .Where(c => c.CellReference == "B14").FirstOrDefault();

                        Row linhaVencimento = sheetData.Elements<Row>().Where(r => r.RowIndex == 15).First();
                        Cell celulaVencimento = linhaVencimento.Elements<Cell>().Where(c => c.CellReference == "B15")
                            .FirstOrDefault();

                        mesAno = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaMesAno, spreadsheetDocument.WorkbookPart);
                        funcionario = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaFuncionario, spreadsheetDocument.WorkbookPart);
                        numeroCartao = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaNumeroCartao, spreadsheetDocument.WorkbookPart);
                        numeroCartao = numeroCartao.Replace("Número do cartão: ", "");
                        vencimento = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaVencimento, spreadsheetDocument.WorkbookPart);

                        int posicaoInicioAno = mesAno.IndexOf("/") + 1;
                        int posicaoFimAno = mesAno.Length - posicaoInicioAno;
                        ano = mesAno.Substring(posicaoInicioAno, posicaoFimAno);

                        #region Carrega Funcionario

                        FUNCIONARIO funcionarioApolo = apolo2Session.FUNCIONARIO
                            .Where(w => w.USERNomeCartaoCorporativo == funcionario
                                && w.USERParticipaControleRDVWeb.Equals("Sim")).FirstOrDefault();
                        if (funcionarioApolo != null)
                        {
                            empresa = funcionarioApolo.USEREmpres;

                            if (empresa == null)
                            {
                                ViewBag.Erro = "Funcionário " + funcionario + " não tem Empresa selecionada no Cadastro "
                                    + "de funcionário do Apolo! Selecionar a Empresa "
                                    + "na aba 'Dados RDV'!";
                                arquivo.Close();
                                return View("FaturaCartaoCorporativoBradescoEXCEL");
                            }

                            usuario = funcionarioApolo.UsuCod;
                            nomeUsuario = funcionarioApolo.FuncNome;
                        }
                        else
                        {
                            ViewBag.Erro = "Funcionário " + funcionario + " não relacionado no Cadastro "
                                + "de funcionário do Apolo! Inserir o nome que está na fatura no campo "
                                + "'Nome Cartao Corporativo' da aba 'Dados RDV'!";
                            arquivo.Close();
                            return View("FaturaCartaoCorporativoBradescoEXCEL");
                        }

                        #endregion

                        #region Verifica se Existe a Fatura importada

                        var listaFatura = hlbapp.RDV
                            .Where(w => w.MesAnoFatura == mesAno && w.Banco == "Bradesco"
                                && w.Usuario == usuario
                                && w.NumeroCartao == numeroCartao)
                            .ToList();

                        if (listaFatura.Count > 0)
                        {
                            ViewBag.Erro = "Fatura " + mesAno + " do Bradesco do funcionário " + nomeUsuario + " REIMPORTADA com sucesso!";

                            HLBAPPEntities hlbappDelete = new HLBAPPEntities();
                            foreach (var item in listaFatura)
                            {
                                var itemDelete = hlbappDelete.RDV.Where(w => w.ID == item.ID).FirstOrDefault();
                                hlbappDelete.RDV.DeleteObject(itemDelete);
                            }
                            hlbappDelete.SaveChanges();
                        }
                        else
                        {
                            ViewBag.Mensagem = "Fatura " + mesAno + " do Bradesco do funcionário " + nomeUsuario + " IMPORTADA com sucesso!";
                        }

                        #endregion
                    }

                    #endregion

                    // Navega nas linhas da Planilha
                    foreach (var linha in listaLinhas)
                    {
                        Row linhaB = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                        string valorB = "";
                        if (linhaB.Elements<Cell>().Where(c => c.CellReference == "B" + linha.RowIndex).Count() > 0)
                        {
                            Cell celulaB = linhaB.Elements<Cell>()
                                .Where(c => c.CellReference == "B" + linha.RowIndex).First();

                            valorB = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaB, spreadsheetDocument.WorkbookPart);
                        }

                        if (valorB == "Total:")
                            break;

                        DateTime dataVerifica = new DateTime();

                        if ((linha.RowIndex >= 17 && nomeAba.Equals("page 1")
                                && DateTime.TryParse(valorB + "/" + ano, out dataVerifica))
                            ||
                            ((nomeAba.Equals("page 2") || nomeAba.Equals("page 3")) && valorB != "Total:"
                                && DateTime.TryParse(valorB + "/" + ano, out dataVerifica)))
                        {
                            linhaErro = Convert.ToInt32(linha.RowIndex.Value);

                            #region Data do Lançamento

                            DateTime dataLancamento = Convert.ToDateTime(valorB + "/" + ano);
                            if (Convert.ToDateTime(vencimento.Substring(20, 10)).Month == 1)
                                dataLancamento = Convert.ToDateTime(valorB + "/" +
                                    Convert.ToDateTime(vencimento.Substring(20, 10)).AddYears(-1).Year);

                            #endregion

                            #region Histórico

                            Row linhaHistorico = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaHistorico = linhaHistorico.Elements<Cell>()
                                .Where(c => c.CellReference == "C" + linha.RowIndex).First();

                            string historico = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaHistorico, spreadsheetDocument.WorkbookPart);

                            #endregion

                            #region Valor Moeda Estrangeira - Dólar (US$)

                            Row linhaValorMoedaEstrangeira = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaValorMoedaEstrangeira = linhaValorMoedaEstrangeira.Elements<Cell>()
                                .Where(c => c.CellReference == "D" + linha.RowIndex).First();
                            decimal valorMoedaEstrangeira = 0;
                            decimal resultValorMoedaEstrangeira = 0;
                            bool valorMoedaEstrangeiraColunaE = false;
                            if (Decimal.TryParse(MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController.FromExcelTextBollean(celulaValorMoedaEstrangeira,
                                    spreadsheetDocument.WorkbookPart).Replace(" ", ""), 
                                    out resultValorMoedaEstrangeira))
                            {
                                valorMoedaEstrangeira = resultValorMoedaEstrangeira;
                            }
                            else
                            {
                                valorMoedaEstrangeiraColunaE = true;
                                linhaValorMoedaEstrangeira = sheetData.Elements<Row>()
                                    .Where(r => r.RowIndex == linha.RowIndex).First();
                                celulaValorMoedaEstrangeira = linhaValorMoedaEstrangeira.Elements<Cell>()
                                    .Where(c => c.CellReference == "E" + linha.RowIndex).First();
                                valorMoedaEstrangeira = Convert.ToDecimal(MvcAppHylinedoBrasilMobile.Controllers
                                    .NavisionIntegrationAppController
                                    .FromExcelTextBollean(celulaValorMoedaEstrangeira,
                                        spreadsheetDocument.WorkbookPart).Replace(" ", ""));
                            }

                            #endregion

                            #region Valor Reais - R$

                            Row linhaValorReais= sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaValorReais = linhaValorReais.Elements<Cell>()
                                .Where(c => c.CellReference == "E" + linha.RowIndex).First();
                            decimal valorReais = 0;
                            if (!valorMoedaEstrangeiraColunaE)
                            {
                                valorReais = Convert.ToDecimal(MvcAppHylinedoBrasilMobile.Controllers
                                    .NavisionIntegrationAppController
                                    .FromExcelTextBollean(celulaValorReais, spreadsheetDocument.WorkbookPart)
                                    .Replace(" ",""));
                            }
                            else
                            {
                                linhaValorReais = sheetData.Elements<Row>()
                                    .Where(r => r.RowIndex == linha.RowIndex).First();
                                celulaValorReais = linhaValorReais.Elements<Cell>()
                                    .Where(c => c.CellReference == "F" + linha.RowIndex).First();
                                valorReais = Convert.ToDecimal(MvcAppHylinedoBrasilMobile.Controllers
                                    .NavisionIntegrationAppController
                                    .FromExcelTextBollean(celulaValorReais, spreadsheetDocument.WorkbookPart)
                                    .Replace(" ", ""));
                            }

                            #endregion

                            #region Insere Lançamento no RDV

                            RDV lancamento = new RDV();

                            lancamento.Empresa = empresa;
                            lancamento.Usuario = usuario;
                            lancamento.NomeUsuario = nomeUsuario;
                            lancamento.DataHora = DateTime.Now;
                            lancamento.DataRDV = dataLancamento;
                            lancamento.TipoDespesa = "";
                            lancamento.Descricao = historico;
                            if (valorMoedaEstrangeira == 0)
                            {
                                lancamento.CodCidade = "Nacional";
                                lancamento.NomeCidade = "Nacional";
                                lancamento.CodPais = "BR";
                                lancamento.NomePais = "BRASIL";
                            }
                            else
                            {
                                lancamento.CodCidade = "Internacional";
                                lancamento.NomeCidade = "Internacional";
                                lancamento.IndEconCod = "0000004";
                                lancamento.IndEconNome = "DÓLAR - U$";
                                lancamento.CodPais = "";
                                lancamento.NomePais = "";
                            }
                            lancamento.ValorDespesa = valorReais;
                            lancamento.Status = "Pendente";
                            lancamento.FormaPagamento = "Cartão Corp.";
                            lancamento.ValorMoedaEstrangeira = valorMoedaEstrangeira;
                            lancamento.MesAnoFatura = mesAno;
                            lancamento.AnoMes = Convert.ToInt32(Convert.ToDateTime(vencimento.Substring(20,10))
                                .ToString("yyyyMM"));
                            lancamento.Banco = "Bradesco";
                            lancamento.NumeroCartao = numeroCartao;

                            hlbapp.RDV.AddObject(lancamento);

                            #endregion
                        }
                    }
                }

                hlbapp.SaveChanges();

                #region Gera numero de fechamento da Fatura

                var listaFaturaRDV = hlbapp.RDV
                    .Where(w => w.Usuario == usuario && w.MesAnoFatura == mesAno
                        && w.Banco == "Bradesco").ToList();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                apolo2Session.gerar_codigo("1", "GERA_FECH_RDV_WEB", numero);

                foreach (var item in listaFaturaRDV)
                {
                    item.NumeroFechamentoRDV = numero.Value.ToString();

                    #region Insere LOG Lançamento

                    InsereLOGLancamentoRDV(item, "Importação Fatura Bradesco");

                    #endregion
                }

                hlbapp.SaveChanges();

                #endregion

                #region Insere LOG

                LOG_RDV log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                log.Operacao = "Importação";
                log.NumeroFechamentoRDV = numero.Value.ToString();
                log.Status = "Pendente";

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                #endregion

                arquivo.Close();

                #region Integra com o Apolo automaticamente

                var rdv = listaFaturaRDV.FirstOrDefault();
                
                #region Carrega matrícula do Funcionário

                int? matriculaMIX = 0;
                FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()
                    && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();
                if (func != null)
                {
                    matriculaMIX = func.FuncCodMix;
                }

                #endregion

                var existeApolo = false;
                var empresaIntegApolo = "";
                var chave = 0;

                if (matriculaMIX > 0)
                {
                    #region Verifica se já existe RDV lançado no Apolo

                    ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                    ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                        //.Where(w => (w.MovEstqDocEspec == "RELAT" || w.MovEstqDocEspec == "RECIB")
                        .Where(w => w.MovEstqDocEspec == "RDVWB" && w.MovEstqDocSerie == "1" && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                            && w.TipoLancCod == "E0000444")
                        .FirstOrDefault();

                    if (movEstq != null)
                    {
                        empresaIntegApolo = movEstq.EmpCod;
                        chave = movEstq.MovEstqChv;
                        existeApolo = true;
                    }

                    #endregion

                    if (!existeApolo)
                    {
                        var cDataL = ((Convert.ToDateTime(vencimento.Substring(20, 10))).AddMonths(-1));
                        var dataLancamento = new DateTime(cDataL.Year, cDataL.Month, DateTime.DaysInMonth(cDataL.Year, cDataL.Month));

                        var anoMes = dataLancamento.ToString("MM/yyyy");
                        var valorReembolso = listaFaturaRDV.Sum(s => s.ValorDespesa);
                        if (valorReembolso > 0)
                        {
                            Models.ProceduresApolo.ProceduresApolo procApolo = new Models.ProceduresApolo.ProceduresApolo();
                            procApolo.USER_Insere_RDV(matriculaMIX, Convert.ToInt32(rdv.NumeroFechamentoRDV), anoMes, valorReembolso, "Bradesco",
                                "", dataLancamento);
                        }
                    }
                }

                #region Insere LOG Integração Apolo

                log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                if (existeApolo)
                    log.Operacao = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + empresaIntegApolo + " - Chave Mov. Estq.: " + chave.ToString() + ")!";
                else
                    log.Operacao = "Integração com o Apolo realizada!";
                log.NumeroFechamentoRDV = rdv.NumeroFechamentoRDV;
                log.Status = "Pendente";

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                ViewBag.Mensagem = log.Operacao;

                #endregion

                #endregion

                #region Enviar E-mail

                USUARIO usuarioApolo = apolo2Session.USUARIO.Where(w => w.UsuCod == usuario).FirstOrDefault();
                string paraNome = usuarioApolo.UsuNome;
                string paraEmail = usuarioApolo.UsuEmail;
                string copiaPara = "";
                //string paraNome = "Paulo Alves";
                //string paraEmail = "palves@hyline.com.br";
                //string copiaPara = "";
                string assunto = "FATURA CARTÃO CORPORATIVO BRADESCO " + mesAno + " - " + paraNome + " IMPORTADA";
                string stringChar = "" + (char)13 + (char)10;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "";
                if (empresa == "BR") empresaApolo = "5";
                else if (empresa == "LB") empresaApolo = "7";
                else if (empresa == "HN") empresaApolo = "14";
                else if (empresa == "PL") empresaApolo = "20";

                string porta = "";
                //if (Request.Url.Port != 80) porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "A Fatura " + mesAno + " do Cartão Corporativo do Bradesco"
                    + " foi importada no sistemas de RDV."
                    + "Clique no link a seguir para poder realizar a classificação: "
                    + "http://" + Request.Url.Host + porta + "/RDV/ClassificaFatura?numRDV=" + numero.Value
                    + stringChar + stringChar
                    + "Departamento Financeiro. " + stringChar + stringChar;

                //anexos = GeraRDVPDF(numero.Value.ToString());

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                #endregion

                return RedirectToAction("ListaRDVFechadosGeral");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message
                    + " | Linha da Planilha: " + linhaErro.ToString()
                    + " | Linha de Erro no Código: " + linenum.ToString();
                arquivo.Close();
                return View("FaturaCartaoCorporativoBradescoEXCEL");
            }
        }

        #endregion

        #region Banco do Brasil

        public ActionResult FaturaCartaoCorporativoBBEXCEL()
        {
            Session["ListaAnoMesFaturaBBRDV"] = CarregaAnoMesFaturaBB();

            CarregaEmpresas();

            return View();
        }

        [HttpPost]
        public ActionResult ImportaFaturaCartaoCorporativoBBEXCEL(HttpPostedFileBase file,
            FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "AccountMobile");

            #region Verifica Cotação do Dólar

            string cotacaoDolarStr = model["cotacaoDolar"];
            decimal cotacaoDolar = 0;

            if (cotacaoDolarStr == null)
            {
                ViewBag.erro = "Necessário inserir a Cotação do Dólar!";
                return View("FaturaCartaoCorporativoBBEXCEL");
            }
            else
            {
                cotacaoDolar = Convert.ToDecimal(cotacaoDolarStr);
            }

            #endregion

            #region Salva Arquivo no Disco

            string caminho = @"C:\inetpub\wwwroot\Relatorios\FaturaCartaoCorporativoBBEXCEL_"
                + Session["login"].ToString() + Session.SessionID + ".xls";

            file.SaveAs(caminho);
            caminho = VerificaFormatoArquivo(caminho);
            Stream arquivo = System.IO.File.Open(caminho, FileMode.Open);

            int linhaErro = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            try
            {
                #region Abre arquivo Excel e carrega lista de Planilhas

                // Open a SpreadsheetDocument based on a stream.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(arquivo, true);

                // Lista de Planilhas do Documento Excel
                var lista = spreadsheetDocument.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToList();

                int existe = 0;

                string mesAno = "";
                string funcionario = "";
                string ano = "";
                string empresa = "";
                string usuario = "";
                string nomeUsuario = "";
                string vencimento = "";
                string tipoGasto = "";
                string empresaFatura = "";

                #endregion

                // Navega entre cada Planilha
                foreach (var planilha in lista)
                {
                    #region Carrega Linhas da Planilha

                    string nomeAba = planilha.Name;

                    //string entrou = "";
                    //if (nomeAba.Equals("page 2"))
                    //    entrou = "1";

                    // Caso o produto exista, ele irá percorrer as linhas da planilha para verificar os filhos
                    string relationshipId = spreadsheetDocument.WorkbookPart.Workbook.Descendants<Sheet>()
                        .Where(s => s.Name == planilha.Name)
                        .First().Id;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart
                                                    .GetPartById(relationshipId);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    var listaLinhas = sheetData.Descendants<Row>().ToList();

                    #endregion

                    #region Carrega campo Mês Ano, Tipo de Gasto, Vencimento e Empresa da Fatura

                    if (nomeAba.Equals("page 1"))
                    {
                        Row linhaTipoGasto = sheetData.Elements<Row>().Where(r => r.RowIndex == 7).First();
                        Cell celulaTipoGasto = linhaTipoGasto.Elements<Cell>().Where(c => c.CellReference == "C7")
                            .First();

                        Row linhaVencimento = sheetData.Elements<Row>().Where(r => r.RowIndex == 6).First();
                        Cell celulaVencimento = linhaVencimento.Elements<Cell>().Where(c => c.CellReference == "C6")
                            .First();

                        Row linhaEmpresa = sheetData.Elements<Row>().Where(r => r.RowIndex == 4).First();
                        Cell celulaEmpresa = linhaEmpresa.Elements<Cell>().Where(c => c.CellReference == "C4")
                            .First();

                        tipoGasto = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaTipoGasto, spreadsheetDocument.WorkbookPart);

                        vencimento = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaVencimento, spreadsheetDocument.WorkbookPart);
                        string fullMonthName = Convert.ToDateTime(vencimento)
                            .ToString("MMMM", CultureInfo.CreateSpecificCulture("pt-BR"));
                        fullMonthName = fullMonthName.Substring(0, 1).ToUpper()
                            + fullMonthName.Substring(1, fullMonthName.Length - 1);
                        mesAno = fullMonthName + "/" + Convert.ToDateTime(vencimento).ToString("yyyy");

                        int posicaoInicioAno = mesAno.IndexOf("/") + 1;
                        int posicaoFimAno = mesAno.Length - posicaoInicioAno;
                        ano = mesAno.Substring(posicaoInicioAno, posicaoFimAno);

                        empresaFatura = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                            .FromExcelTextBollean(celulaEmpresa, spreadsheetDocument.WorkbookPart);

                        #region Verifica se Existe a Fatura importada - DESATIVADA

                        //RDV lancamento = hlbapp.RDV
                        //    .Where(w => w.MesAnoFatura == mesAno && w.Banco == "Banco do Brasil"
                        //        && w.TipoGastoFatura == tipoGasto && w.EmpresaFatura == empresaFatura)
                        //    .FirstOrDefault();

                        //if (lancamento != null)
                        //{
                        //    ViewBag.Erro = "Fatura " + mesAno + " do Banco do Brasil do Tipo de Gasto "  + tipoGasto + " já importada!";
                        //    return View("FaturaCartaoCorporativoBBEXCEL");
                        //}

                        #endregion

                        #region Verifica se Existe a Fatura importada

                        var listaFatura = hlbapp.RDV
                            .Where(w => w.MesAnoFatura == mesAno && w.Banco == "Banco do Brasil"
                                && w.TipoGastoFatura == tipoGasto && w.EmpresaFatura == empresaFatura)
                            .ToList();

                        if (listaFatura.Count > 0)
                        {
                            ViewBag.Erro = "Fatura " + mesAno + " do Banco do Brasil do Tipo de Gasto " + tipoGasto + " REIMPORTADA com sucesso!";

                            HLBAPPEntities hlbappDelete = new HLBAPPEntities();
                            foreach (var item in listaFatura)
                            {
                                var itemDelete = hlbappDelete.RDV.Where(w => w.ID == item.ID).FirstOrDefault();
                                hlbappDelete.RDV.DeleteObject(itemDelete);
                            }
                            hlbappDelete.SaveChanges();
                        }
                        else
                        {
                            ViewBag.Mensagem = "Fatura " + mesAno + " do Banco do Brasil do Tipo de Gasto " + tipoGasto + " IMPORTADA com sucesso!";
                        }

                        #endregion
                    }

                    #endregion

                    // Navega nas linhas da Planilha
                    foreach (var linha in listaLinhas)
                    {
                        linhaErro = Convert.ToInt32(linha.RowIndex.ToString());

                        //Row linhaB = sheetData.Elements<Row>()
                        //        .Where(r => r.RowIndex == linha.RowIndex).FirstOrDefault();
                        Cell celulaB = linha.Elements<Cell>()
                            .Where(c => c.CellReference == "B" + linha.RowIndex).FirstOrDefault();

                        string valorB = "";
                        string valorBParte = "";

                        if (celulaB != null)
                        {
                            valorB = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaB, spreadsheetDocument.WorkbookPart);

                            int styleIndex = 0;

                            if (celulaB.StyleIndex != null)
                                styleIndex = (int)celulaB.StyleIndex.Value;
                            DocumentFormat.OpenXml.Spreadsheet.CellFormat cellFormat = 
                                (DocumentFormat.OpenXml.Spreadsheet.CellFormat)spreadsheetDocument
                                .WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ElementAt(styleIndex);
                            uint formatId = 0;
                            if (cellFormat != null)
                                formatId = cellFormat.NumberFormatId.Value;

                            DateTime dataSaidaB = new DateTime();
                            if (formatId == (uint)Formats.DateShort && valorB != "")
                            {
                                int valorBDataNumeral = Convert.ToInt32(valorB);
                                dataSaidaB = MvcAppHylinedoBrasilMobile.Controllers.NavisionIntegrationAppController
                                    .FromExcelSerialDate(valorBDataNumeral);
                                valorB = dataSaidaB.ToShortDateString();
                            }

                            valorBParte = "";
                            if (valorB.Length >= 9)
                                valorBParte = valorB.Substring(0, 9);
                            if (valorBParte == "Transação")
                                break;
                            if (valorB.Contains("https://"))
                                break;
                            if (valorB == "Serviço de Atendimento ao Consumidor - SAC 0800 729 0722")
                                break;
                        }

                        //Row linhaC = sheetData.Elements<Row>()
                        //        .Where(r => r.RowIndex == linha.RowIndex).First();
                        Cell celulaC = linha.Elements<Cell>()
                            .Where(c => c.CellReference == "C" + linha.RowIndex).FirstOrDefault();
                        string valorC = "";
                        if (celulaC != null)
                            valorC = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaC, spreadsheetDocument.WorkbookPart);

                        Cell celulaD = linha.Elements<Cell>()
                            .Where(c => c.CellReference == "D" + linha.RowIndex).FirstOrDefault();
                        string valorD = "";
                        if (celulaD != null)
                            valorD = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaD, spreadsheetDocument.WorkbookPart);

                        //if (valorC.Contains("Banco do Brasil"))
                        //    break;

                        DateTime dataSaida = new DateTime();

                        if ((linha.RowIndex >= 9 && nomeAba.Equals("page 1")
                                && valorBParte != "Transação" && valorB != ""
                                && !valorB.Contains("https://")
                                && !valorC.Contains("Banco do Brasil"))
                            ||
                            (!nomeAba.Equals("page 1") && valorBParte != "Transação" && valorB != ""
                                && !valorB.Contains("https://")
                                && !valorC.Contains("Banco do Brasil")
                                && !valorC.Contains("https://")
                                && !valorD.Contains("https://")
                                && DateTime.TryParse(valorB, out dataSaida)))
                        {
                            #region Carrega Funcionario

                            Row linhaCartao = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaCartao = linhaCartao.Elements<Cell>()
                                .Where(c => c.CellReference == "F" + linha.RowIndex).First();

                            string cartao = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaCartao, spreadsheetDocument.WorkbookPart);
                            string numero04DigitosCartao = cartao.Substring(15, 4);

                            Apolo10Entities apolo2Session = new Apolo10Entities();
                            FUNCIONARIO funcionarioApolo = apolo2Session.FUNCIONARIO
                                .Where(w => w.USER04UltimosDigitosCartaoBB == numero04DigitosCartao
                                    || w.USER04UltimosDigIdentifBB == numero04DigitosCartao)
                                .FirstOrDefault();

                            if (funcionarioApolo != null)
                            {
                                empresa = funcionarioApolo.USEREmpres;

                                if (empresa == null)
                                {
                                    ViewBag.Erro = "Funcionário " + funcionario + " não tem Empresa selecionada no Cadastro "
                                        + "de funcionário do Apolo! Selecionar a Empresa "
                                        + "na aba 'Dados RDV'!";
                                    return View("FaturaCartaoCorporativoBBEXCEL");
                                }

                                usuario = funcionarioApolo.UsuCod;
                                nomeUsuario = funcionarioApolo.FuncNome;
                            }
                            else
                            {
                                ViewBag.Erro = "Cartão / Cód. Identificador " + cartao + " não relacionado no Cadastro "
                                    + "de funcionário do Apolo! Inserir os últimos 04 dígitos do cartão no campo "
                                    + "'04 Últimos Dígitos Cartao BB' da aba 'Dados RDV'!";
                                return View("FaturaCartaoCorporativoBBEXCEL");
                            }

                            #endregion

                            #region Moeda

                            Row linhaMoeda = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaMoeda = linhaMoeda.Elements<Cell>()
                                .Where(c => c.CellReference == "D" + linha.RowIndex).First();

                            string moeda = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaMoeda, spreadsheetDocument.WorkbookPart);

                            #endregion

                            #region Tipo de Despesa

                            string tipoDespesa = "";
                            //if (tipoGasto.Equals("RESTAURANTES"))
                            //{
                            //    if (moeda.Equals("R$"))
                            //        tipoDespesa = "REFEIÇÃO(DN)";
                            //    else
                            //        tipoDespesa = "REFEIÇÃO(DI)";
                            //}
                            //else if (tipoGasto.Equals("HOTEIS"))
                            //{
                            //    if (moeda.Equals("R$"))
                            //        tipoDespesa = "HOSPEDAGEM(DN)";
                            //    else
                            //        tipoDespesa = "HOSPEDAGEM(DI)";
                            //}

                            #endregion

                            #region Data do Lançamento

                            DateTime dataLancamento = Convert.ToDateTime(valorB);

                            #endregion

                            #region Histórico

                            Row linhaHistorico = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaHistorico = linhaHistorico.Elements<Cell>()
                                .Where(c => c.CellReference == "C" + linha.RowIndex).First();

                            string historico = MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaHistorico, spreadsheetDocument.WorkbookPart);

                            #endregion

                            #region Valor Despesa

                            Row linhaValorDespesa = sheetData.Elements<Row>()
                                .Where(r => r.RowIndex == linha.RowIndex).First();
                            Cell celulaValorDespesa = linhaValorDespesa.Elements<Cell>()
                                .Where(c => c.CellReference == "E" + linha.RowIndex).First();
                            decimal valorDespesa = Convert.ToDecimal(MvcAppHylinedoBrasilMobile.Controllers
                                .NavisionIntegrationAppController
                                .FromExcelTextBollean(celulaValorDespesa, spreadsheetDocument.WorkbookPart));

                            decimal valorMoedaEstrangeira = 0;
                            decimal valorReais = 0;
                            if (moeda.Equals("R$"))
                                valorReais = valorDespesa;
                            else
                            {
                                valorReais = valorDespesa * cotacaoDolar;
                                valorMoedaEstrangeira = valorDespesa;
                            }

                            #endregion

                            #region Insere Lançamento no RDV

                            if (!historico.Contains("PGTO DEBITO CONTA"))
                            {
                                //RDV lancamento = hlbapp.RDV
                                //    .Where(w => w.Usuario == usuario && w.DataRDV == dataLancamento
                                //        && w.Descricao == historico
                                //        && w.ValorDespesa == valorReais
                                //        && w.ValorMoedaEstrangeira == valorMoedaEstrangeira)
                                //    .FirstOrDefault();

                                //if (lancamento == null)
                                RDV lancamento = new RDV();

                                //usuario = "PALVES";
                                //nomeUsuario = "TESTE";

                                lancamento.Empresa = empresa;
                                lancamento.Usuario = usuario;
                                lancamento.NomeUsuario = nomeUsuario;
                                lancamento.DataHora = DateTime.Now;
                                lancamento.DataRDV = dataLancamento;
                                lancamento.TipoDespesa = tipoDespesa;
                                lancamento.Descricao = historico;
                                if (valorMoedaEstrangeira == 0)
                                {
                                    lancamento.CodCidade = "Nacional";
                                    lancamento.NomeCidade = "Nacional";
                                    lancamento.CodPais = "BR";
                                    lancamento.NomePais = "BRASIL";
                                }
                                else
                                {
                                    lancamento.CodCidade = "Internacional";
                                    lancamento.NomeCidade = "Internacional";
                                    lancamento.IndEconCod = "0000004";
                                    lancamento.IndEconNome = "DÓLAR - U$";
                                    lancamento.CodPais = "";
                                    lancamento.NomePais = "";
                                }
                                lancamento.ValorDespesa = valorReais;
                                lancamento.Status = "Pendente";
                                lancamento.FormaPagamento = "Cartão Corp.";
                                lancamento.ValorMoedaEstrangeira = valorMoedaEstrangeira;
                                lancamento.MesAnoFatura = mesAno;
                                lancamento.AnoMes = Convert.ToInt32(Convert.ToDateTime(vencimento)
                                    .ToString("yyyyMM"));
                                lancamento.Banco = "Banco do Brasil";
                                lancamento.TipoGastoFatura = tipoGasto;
                                lancamento.EmpresaFatura = empresaFatura;
                                lancamento.NumeroCartao = cartao;

                                hlbapp.RDV.AddObject(lancamento);
                            }

                            #endregion
                        }
                    }
                }

                hlbapp.SaveChanges();

                #region Gera numero de fechamento da Fatura

                var listaFaturaRDVGrupo = hlbapp.RDV
                    .Where(w => w.MesAnoFatura == mesAno && w.Banco == "Banco do Brasil"
                        && w.TipoGastoFatura == tipoGasto && w.EmpresaFatura == empresaFatura)
                    .GroupBy(g => new
                        {
                            g.MesAnoFatura,
                            g.Banco,
                            g.TipoGastoFatura,
                            g.EmpresaFatura,
                            g.Usuario
                        })
                    .OrderBy(o => o.Key.Usuario)
                    .ToList();

                foreach (var grupo in listaFaturaRDVGrupo)
                {
                    #region Verifica Numero p/ utilizar existente ou criar novo

                    string numRDV = "";
                    RDV rdvNumRDV = hlbapp.RDV
                        .Where(w => w.MesAnoFatura == grupo.Key.MesAnoFatura
                            && w.Banco == grupo.Key.Banco
                            && w.Usuario == grupo.Key.Usuario
                            && w.EmpresaFatura == grupo.Key.EmpresaFatura
                            && w.NumeroFechamentoRDV != null && w.NumeroFechamentoRDV != "")
                        .FirstOrDefault();

                    if (rdvNumRDV != null)
                        numRDV = rdvNumRDV.NumeroFechamentoRDV;
                    else
                    {
                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                        Apolo10Entities apolo2Session = new Apolo10Entities();
                        apolo2Session.gerar_codigo("1", "GERA_FECH_RDV_WEB", numero);
                        numRDV = numero.Value.ToString();
                    }

                    #endregion

                    var listaFaturaRDV = hlbapp.RDV
                        .Where(w => w.MesAnoFatura == grupo.Key.MesAnoFatura
                            && w.Banco == grupo.Key.Banco
                            && w.TipoGastoFatura == grupo.Key.TipoGastoFatura
                            && w.EmpresaFatura == grupo.Key.EmpresaFatura
                            && w.Usuario == grupo.Key.Usuario
                            && (w.NumeroFechamentoRDV == null || w.NumeroFechamentoRDV == ""))
                        .ToList();

                    foreach (var item in listaFaturaRDV)
                    {
                        item.NumeroFechamentoRDV = numRDV;

                        #region Insere LOG Lançamento

                        InsereLOGLancamentoRDV(item, "Importação Fatura BB");

                        #endregion
                    }

                    #region Insere LOG

                    LOG_RDV log = new LOG_RDV();
                    log.DataHora = DateTime.Now;
                    log.Usuario = Session["login"].ToString().ToUpper();
                    log.Operacao = "Importação " + grupo.Key.TipoGastoFatura;
                    log.NumeroFechamentoRDV = numRDV;
                    log.Status = "Pendente";

                    hlbapp.LOG_RDV.AddObject(log);

                    #endregion
                }

                hlbapp.SaveChanges();

                #endregion

                arquivo.Close();

                //ViewBag.fileName = "Arquivo " + file.FileName + " importado com sucesso!";

                #region Enviar E-mail (DESATIVADO)

                //USUARIO usuarioApolo = apolo2Static.USUARIO.Where(w => w.UsuCod == usuario).FirstOrDefault();
                //string paraNome = usuarioApolo.UsuNome;
                //string paraEmail = usuarioApolo.UsuEmail;
                //string copiaPara = "";
                ////string paraNome = "Paulo Alves";
                ////string paraEmail = "palves@hyline.com.br";
                ////string copiaPara = "";
                //string assunto = "FATURA CARTÃO CORPORATIVA " + mesAno + " - " + paraNome + " IMPORTADA";
                //string stringChar = "" + (char)13 + (char)10;
                //string corpoEmail = "";
                //string anexos = "";

                //string porta = "";
                //if (Request.Url.Port != 80)
                //    porta = ":" + Request.Url.Port.ToString();

                //corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                //    + "A Fatura " + mesAno + " do Cartão Corporativo"
                //    + " foi importada no sistemas de RDV."
                //    + "Clique no link a seguir para poder realizar a classificação: "
                //    + "http://" + Request.Url.Host + porta + "/RDV/ClassificaFatura?numRDV=" + numero.Value
                //    + stringChar + stringChar
                //    + "Departamento Financeiro. " + stringChar + stringChar;

                ////anexos = GeraRDVPDF(numero.Value.ToString());

                //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos);

                #endregion

                //return View("ListaRDVFechadosGeral");
                return RedirectToAction("ListaRDVFechadosGeral");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message
                    + " | Linha da Planilha: " + linhaErro.ToString()
                    + " | Linha de Erro no Código: " + linenum.ToString();
                arquivo.Close();
                return View("FaturaCartaoCorporativoBBEXCEL");
            }
        }

        public ActionResult EnviarEmailsFaturasBB(FormCollection model)
        {
            #region Verifica Mês Ano Fatura

            string mesAnoFatura = model["AnoMesFaturaBB"];

            if (mesAnoFatura == null)
            {
                ViewBag.erro = "Necessário selecionar Mês Ano da Fatura!";
                return View("FaturaCartaoCorporativoBBEXCEL");
            }

            #endregion

            #region Verifica Empresa

            string empresa = model["EmpresaFaturaBB"];

            if (empresa == null)
            {
                ViewBag.erro = "Necessário selecionar Empresa!";
                return View("FaturaCartaoCorporativoBBEXCEL");
            }

            #endregion

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            //string empresas = Session["empresa"].ToString();

            var listaFaturaRDVGrupo = hlbapp.RDV
                .Where(w => w.MesAnoFatura == mesAnoFatura && w.Banco == "Banco do Brasil"
                    //&& empresas.IndexOf(w.Empresa) != -1)
                    //&& w.Usuario == "PALVES"
                    && w.Empresa == empresa)
                .GroupBy(g => new
                {
                    g.Empresa,
                    g.MesAnoFatura,
                    g.AnoMes,
                    g.Banco,
                    g.Usuario,
                    g.NumeroFechamentoRDV
                })
                .OrderBy(o => o.Key.Usuario)
                .ToList();

            foreach (var item in listaFaturaRDVGrupo)
            {
                #region Enviar E-mail

                Apolo10Entities apolo2Session = new Apolo10Entities();
                USUARIO usuarioApolo = apolo2Session.USUARIO.Where(w => w.UsuCod == item.Key.Usuario)
                    .FirstOrDefault();
                string paraNome = usuarioApolo.UsuNome;
                string paraEmail = usuarioApolo.UsuEmail;
                string copiaPara = "";
                //string paraNome = "Paulo Alves";
                //string paraEmail = "palves@hyline.com.br";
                //string copiaPara = "";
                string assunto = "FATURA CARTÃO CORPORATIVO BANCO DO BRASIL " + mesAnoFatura + " - "
                    + paraNome + " IMPORTADA";
                string stringChar = "" + (char)13 + (char)10;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "";
                string site = "";
                if (item.Key.Empresa == "BR")
                {
                    empresaApolo = "5";
                    site = "m.hlbapp.hyline.com.br";
                }
                else if (item.Key.Empresa == "LB")
                {
                    empresaApolo = "7";
                    site = "m.app.ltz.com.br";
                }
                else if (item.Key.Empresa == "HN")
                {
                    empresaApolo = "14";
                    site = "m.app.hnavicultura.com.br";
                }
                else if (item.Key.Empresa == "PL")
                {
                    empresaApolo = "20";
                    site = "m.app.planaltopostura.com.br";
                }

                string porta = "";
                //if (Request.Url.Port != 80) porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "A Fatura " + mesAnoFatura + " do Cartão Corporativo do Banco do Brasil"
                    + " foi importada no sistemas de RDV."
                    + "Clique no link a seguir para poder realizar a classificação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/ClassificaFatura?numRDV="
                    + "http://" + site + porta + "/RDV/ClassificaFatura?numRDV="
                        + item.Key.NumeroFechamentoRDV
                    + stringChar + stringChar
                    + "Departamento Financeiro. " + stringChar + stringChar;

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                #endregion

                #region Integra com o Apolo automaticamente

                var listaFaturaRDV = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == item.Key.NumeroFechamentoRDV).ToList();
                var rdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == item.Key.NumeroFechamentoRDV).FirstOrDefault();

                #region Carrega matrícula do Funcionário

                int? matriculaMIX = 0;
                FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()
                    && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();
                if (func != null)
                {
                    matriculaMIX = func.FuncCodMix;
                }

                #endregion

                var existeApolo = false;
                var empresaIntegApolo = "";
                var chave = 0;

                if (matriculaMIX > 0)
                {
                    #region Verifica se já existe RDV lançado no Apolo

                    ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                    ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                        //.Where(w => (w.MovEstqDocEspec == "RELAT" || w.MovEstqDocEspec == "RECIB")
                        .Where(w => w.MovEstqDocEspec == "RDVWB" && w.MovEstqDocSerie == "1" && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                            && w.TipoLancCod == "E0000444")
                        .FirstOrDefault();

                    if (movEstq != null)
                    {
                        empresaIntegApolo = movEstq.EmpCod;
                        chave = movEstq.MovEstqChv;
                        existeApolo = true;
                    }

                    #endregion

                    if (!existeApolo)
                    {
                        var ano = Convert.ToInt32(item.Key.AnoMes.ToString().Substring(0, 4));
                        var mes = Convert.ToInt32(item.Key.AnoMes.ToString().Substring(4, 2));
                        var cDataL = (new DateTime(ano, mes, 1)).AddMonths(-1);
                        var dataLancamento = new DateTime(cDataL.Year, cDataL.Month, DateTime.DaysInMonth(cDataL.Year, cDataL.Month));

                        var anoMes = dataLancamento.ToString("MM/yyyy");
                        var valorReembolso = listaFaturaRDV.Sum(s => s.ValorDespesa);
                        if (valorReembolso > 0)
                        {
                            Models.ProceduresApolo.ProceduresApolo procApolo = new Models.ProceduresApolo.ProceduresApolo();
                            procApolo.USER_Insere_RDV(matriculaMIX, Convert.ToInt32(rdv.NumeroFechamentoRDV), anoMes, valorReembolso, "BB",
                                "", dataLancamento);
                        }
                    }
                }

                #region Insere LOG Integração Apolo

                LOG_RDV log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                if (existeApolo)
                    log.Operacao = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + empresaIntegApolo + " - Chave Mov. Estq.: " + chave.ToString() + ")!";
                else
                    log.Operacao = "Integração com o Apolo realizada!";
                log.NumeroFechamentoRDV = rdv.NumeroFechamentoRDV;
                log.Status = "Pendente";

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                ViewBag.Mensagem = log.Operacao;

                #endregion

                #endregion

                #region Insere LOG

                log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                log.Operacao = "Envio de E-mail";
                log.NumeroFechamentoRDV = item.Key.NumeroFechamentoRDV;
                log.Status = "Pendente";

                hlbapp.LOG_RDV.AddObject(log);

                #endregion
            }

            hlbapp.SaveChanges();

            var descricaoEmpresa = ((List<SelectListItem>)Session["ListaEmpresasRDV"]).Where(w => w.Value == empresa).FirstOrDefault().Text;

            ViewBag.Mensagem = "E-mails da Fatura " + mesAnoFatura + " do Banco do Brasil da empresa " + descricaoEmpresa + " enviados com sucesso!";

            return RedirectToAction("ListaRDVFechadosGeral");
        }

        #endregion

        #region Itaú

        public ActionResult FaturaCartaoCorporativoItauEXCEL()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportaFaturaCartaoCorporativoItauEXCEL(HttpPostedFileBase file, FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "AccountMobile");

            #region Abre arquivo

            StreamReader reader = new StreamReader(file.InputStream);

            int linhaErro = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo2Session = new Apolo10Entities();

            #endregion

            #region Verifica se já existe esse Mês Ano da Fatura importada

            int mesAnoFatura = Convert.ToInt32(model["AnoMesFaturaItau"].Replace("-",""));

            var verificaAnoMesFaturaLancada = hlbapp.RDV
                .Where(w => w.AnoMes == mesAnoFatura && w.Banco == "Itaú").FirstOrDefault();

            if (verificaAnoMesFaturaLancada != null)
            {
                ViewBag.erro = "Fatura do mês " + Convert.ToDateTime(model["AnoMesFaturaItau"] + "-01").ToString("MMMM/yyyy") + " já importada!";
                return View("FaturaCartaoCorporativoItauEXCEL");
            }

            #endregion

            try
            {
                #region Lê o arquivo e insere os lançamentos

                // Pular o cabeçalho
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (!values[6].Trim().Contains("PAGAMENTO EFETUADO"))
                    {
                        var nomeColaboradorLinha = values[1].Trim();
                        var numeroCartao = values[3].Trim();

                        FUNCIONARIO colaborador = apolo2Session.FUNCIONARIO
                            .Where(w => w.USERNomeCartaoCorpItau == nomeColaboradorLinha
                                && w.USERNumeroCartaoCorpItau == numeroCartao)
                            .FirstOrDefault();

                        if (colaborador != null)
                        {
                            #region Insere Lançamento no RDV

                            RDV lancamento = new RDV();

                            lancamento.Empresa = colaborador.USEREmpres;
                            lancamento.Usuario = colaborador.UsuCod;
                            lancamento.NomeUsuario = values[1].Trim();
                            lancamento.DataHora = DateTime.Now;
                            lancamento.DataRDV = Convert.ToDateTime(values[0].Trim());
                            lancamento.TipoDespesa = "";
                            lancamento.Descricao = values[6].Trim();
                            if (values[5] == "NACIONAL")
                            {
                                lancamento.CodCidade = "Nacional";
                                lancamento.NomeCidade = "Nacional";
                                lancamento.CodPais = "BR";
                                lancamento.NomePais = "BRASIL";
                            }
                            else
                            {
                                lancamento.CodCidade = "Internacional";
                                lancamento.NomeCidade = "Internacional";
                                lancamento.IndEconCod = "0000004";
                                lancamento.IndEconNome = "DÓLAR - U$";
                                lancamento.CodPais = "";
                                lancamento.NomePais = values[11].Trim();
                            }
                            lancamento.ValorDespesa = Convert.ToDecimal(values[14].Trim().Replace(".", ","));
                            lancamento.Status = "Pendente";
                            lancamento.FormaPagamento = "Cartão Corp.";
                            if (values[13].Trim().Replace(".", ",") != "")
                                lancamento.ValorMoedaEstrangeira = Convert.ToDecimal(values[13].Trim().Replace(".", ","));
                            else
                                lancamento.ValorMoedaEstrangeira = 0;
                            lancamento.MesAnoFatura = Convert.ToDateTime(model["AnoMesFaturaItau"] + "-01").ToString("MMMM/yyyy");
                            lancamento.AnoMes = mesAnoFatura;
                            lancamento.Banco = "Itaú";
                            lancamento.NumeroCartao = values[3].Trim();

                            hlbapp.RDV.AddObject(lancamento);

                            #endregion
                        }
                        else
                        {
                            ViewBag.fileName = "";
                            ViewBag.erro = "Colaborador " + nomeColaboradorLinha + " - Nº Cartão: "+ numeroCartao + " não configurado! Por favor, configure na aba 'Dados RDV'" +
                                " no cadastro de funcionários do Apolo!";
                            return View("FaturaCartaoCorporativoItauEXCEL");
                        }
                    }
                }

                hlbapp.SaveChanges();

                #endregion

                #region Gera numero de fechamento da Fatura

                var listaFaturaRDVGrupo = hlbapp.RDV
                    .Where(w => w.AnoMes == mesAnoFatura && w.Banco == "Itaú")
                    .GroupBy(g => new
                    {
                        g.MesAnoFatura,
                        g.Banco,
                        g.Usuario
                    })
                    .OrderBy(o => o.Key.Usuario)
                    .ToList();

                foreach (var grupo in listaFaturaRDVGrupo)
                {
                    #region Verifica Numero p/ utilizar existente ou criar novo

                    string numRDV = "";
                    RDV rdvNumRDV = hlbapp.RDV
                        .Where(w => w.MesAnoFatura == grupo.Key.MesAnoFatura
                            && w.Banco == grupo.Key.Banco
                            && w.Usuario == grupo.Key.Usuario
                            && w.NumeroFechamentoRDV != null && w.NumeroFechamentoRDV != "")
                        .FirstOrDefault();

                    if (rdvNumRDV != null)
                        numRDV = rdvNumRDV.NumeroFechamentoRDV;
                    else
                    {
                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                        apolo2Session.gerar_codigo("1", "GERA_FECH_RDV_WEB", numero);
                        numRDV = numero.Value.ToString();
                    }

                    #endregion

                    var listaFaturaRDV = hlbapp.RDV
                        .Where(w => w.MesAnoFatura == grupo.Key.MesAnoFatura
                            && w.Banco == grupo.Key.Banco
                            && w.Usuario == grupo.Key.Usuario
                            && (w.NumeroFechamentoRDV == null || w.NumeroFechamentoRDV == ""))
                        .ToList();

                    foreach (var item in listaFaturaRDV)
                    {
                        item.NumeroFechamentoRDV = numRDV;

                        #region Insere LOG Lançamento

                        InsereLOGLancamentoRDV(item, "Importação Fatura Itaú");

                        #endregion
                    }

                    #region Insere LOG

                    LOG_RDV log = new LOG_RDV();
                    log.DataHora = DateTime.Now;
                    log.Usuario = Session["login"].ToString().ToUpper();
                    log.Operacao = "Importação Fatura Itaú - " + mesAnoFatura.ToString();
                    log.NumeroFechamentoRDV = numRDV;
                    log.Status = "Pendente";

                    hlbapp.LOG_RDV.AddObject(log);

                    #endregion
                }

                hlbapp.SaveChanges();

                #endregion

                var listaFaturaRDVGrupoEmail = hlbapp.RDV
                    .Where(w => w.AnoMes == mesAnoFatura && w.Banco == "Itaú")
                    .GroupBy(g => new
                    {
                        g.Empresa,
                        g.MesAnoFatura,
                        g.AnoMes,
                        g.Banco,
                        g.Usuario,
                        g.NumeroFechamentoRDV
                    })
                    .OrderBy(o => o.Key.Usuario)
                    .ToList();

                foreach (var item in listaFaturaRDVGrupoEmail)
                {
                    #region Enviar E-mail

                    USUARIO usuarioApolo = apolo2Session.USUARIO.Where(w => w.UsuCod == item.Key.Usuario)
                        .FirstOrDefault();
                    string paraNome = usuarioApolo.UsuNome;
                    string paraEmail = usuarioApolo.UsuEmail;
                    string copiaPara = "";
                    //paraNome = "Paulo Alves";
                    //paraEmail = "palves@hyline.com.br";
                    //copiaPara = "";
                    string assunto = "FATURA CARTÃO CORPORATIVO ITAÚ " + item.Key.MesAnoFatura + " - "
                        + paraNome + " IMPORTADA";
                    string stringChar = "" + (char)13 + (char)10;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "";
                    string site = "";
                    if (item.Key.Empresa == "BR")
                    {
                        empresaApolo = "5";
                        site = "m.hlbapp.hyline.com.br";
                    }
                    if (item.Key.Empresa == "LG")
                    {
                        empresaApolo = "32";
                        site = "m.hlbapp.hyline.com.br";
                    }
                    else if (item.Key.Empresa == "LB")
                    {
                        empresaApolo = "7";
                        site = "m.app.ltz.com.br";
                    }
                    else if (item.Key.Empresa == "HN")
                    {
                        empresaApolo = "14";
                        site = "m.app.hnavicultura.com.br";
                    }
                    else if (item.Key.Empresa == "PL")
                    {
                        empresaApolo = "20";
                        site = "m.app.planaltopostura.com.br";
                    }
                    else if (item.Key.Empresa == "NG")
                    {
                        empresaApolo = "40";
                        site = "m.hlbapp.hyline.com.br";
                    }

                    string porta = "";
                    //if (Request.Url.Port != 80) porta = ":" + Request.Url.Port.ToString();

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "A Fatura " + mesAnoFatura + " do Cartão Corporativo do Itaú"
                        + " foi importada no sistemas de RDV."
                        + "Clique no link a seguir para poder realizar a classificação: "
                        //+ "http://" + Request.Url.Host + porta + "/RDV/ClassificaFatura?numRDV="
                        + "http://" + site + porta + "/RDV/ClassificaFatura?numRDV="
                            + item.Key.NumeroFechamentoRDV
                        + stringChar + stringChar
                        + "Departamento Financeiro. " + stringChar + stringChar;

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                    #endregion

                    #region Integra com o Apolo automaticamente

                    var listaFaturaRDV = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == item.Key.NumeroFechamentoRDV).ToList();
                    var rdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == item.Key.NumeroFechamentoRDV).FirstOrDefault();

                    #region Carrega matrícula do Funcionário

                    int? matriculaMIX = 0;
                    FUNCIONARIO func = apolo2Session.FUNCIONARIO.Where(w => w.UsuCod == rdv.Usuario.ToUpper()
                        && w.USERParticipaControleRDVWeb == "Sim").FirstOrDefault();
                    if (func != null)
                    {
                        matriculaMIX = func.FuncCodMix;
                    }

                    #endregion

                    var existeApolo = false;
                    var empresaIntegApolo = "";
                    var chave = 0;

                    if (matriculaMIX > 0)
                    {
                        #region Verifica se já existe RDV lançado no Apolo

                        ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();
                        ImportaIncubacao.Data.Apolo.MOV_ESTQ movEstq = apoloService.MOV_ESTQ
                            //.Where(w => (w.MovEstqDocEspec == "RELAT" || w.MovEstqDocEspec == "RECIB")
                            .Where(w => w.MovEstqDocEspec == "RDVWB" && w.MovEstqDocSerie == "1" && w.MovEstqDocNum == rdv.NumeroFechamentoRDV
                                && w.TipoLancCod == "E0000444")
                            .FirstOrDefault();

                        if (movEstq != null)
                        {
                            empresaIntegApolo = movEstq.EmpCod;
                            chave = movEstq.MovEstqChv;
                            existeApolo = true;
                        }

                        #endregion

                        if (!existeApolo)
                        {
                            var ano = Convert.ToInt32(item.Key.AnoMes.ToString().Substring(0, 4));
                            var mes = Convert.ToInt32(item.Key.AnoMes.ToString().Substring(4, 2));
                            var cDataL = (new DateTime(ano, mes, 1)).AddMonths(-1);
                            var dataLancamento = new DateTime(cDataL.Year, cDataL.Month, DateTime.DaysInMonth(cDataL.Year, cDataL.Month));

                            var anoMes = dataLancamento.ToString("MM/yyyy");
                            var valorReembolso = listaFaturaRDV.Sum(s => s.ValorDespesa);
                            if (valorReembolso > 0)
                            {
                                Models.ProceduresApolo.ProceduresApolo procApolo = new Models.ProceduresApolo.ProceduresApolo();
                                procApolo.USER_Insere_RDV(matriculaMIX, Convert.ToInt32(rdv.NumeroFechamentoRDV), anoMes, valorReembolso, "Itaú",
                                    "", dataLancamento);
                            }
                        }
                    }

                    #region Insere LOG Integração Apolo

                    LOG_RDV log = new LOG_RDV();
                    log.DataHora = DateTime.Now;
                    log.Usuario = Session["login"].ToString().ToUpper();
                    if (existeApolo)
                        log.Operacao = "Integração com o Apolo: RDV já existe no Apolo (Empresa: " + empresaIntegApolo + " - Chave Mov. Estq.: " + chave.ToString() + ")!";
                    else
                        log.Operacao = "Integração com o Apolo realizada!";
                    log.NumeroFechamentoRDV = rdv.NumeroFechamentoRDV;
                    log.Status = "Pendente";

                    hlbapp.LOG_RDV.AddObject(log);

                    hlbapp.SaveChanges();

                    ViewBag.Mensagem = log.Operacao;

                    #endregion

                    #endregion

                    #region Insere LOG

                    log = new LOG_RDV();
                    log.DataHora = DateTime.Now;
                    log.Usuario = Session["login"].ToString().ToUpper();
                    log.Operacao = "Envio de E-mail";
                    log.NumeroFechamentoRDV = item.Key.NumeroFechamentoRDV;
                    log.Status = "Pendente";

                    hlbapp.LOG_RDV.AddObject(log);

                    #endregion
                }

                return RedirectToAction("ListaRDVFechadosGeral");
            }
            catch (Exception e)
            {
                int linenum = Convert.ToInt32(e.StackTrace.Substring(e.StackTrace.LastIndexOf(' ')));

                ViewBag.fileName = "";
                ViewBag.erro = "Erro ao realizar a importação: " + e.Message
                    + " | Linha da Planilha: " + linhaErro.ToString()
                    + " | Linha de Erro no Código: " + linenum.ToString();
                return View("FaturaCartaoCorporativoItauEXCEL");
            }
        }

        #endregion

        public ActionResult ListaFaturaCartaoCorporativo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
            //Session["usuarioSelecionado"] = "(Todos)";
            Session["statusSelecionado"] = "(Todos)";
            Session["ListaRDV"] = FilterListaFatura();

            return View();
        }

        public ActionResult SearchListaFaturaCartaoCorporativo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialRDV"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialRDV"]);
                Session["dataInicialRDV"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialRDV"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalRDV"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalRDV"]);
                Session["dataFinalRDV"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalRDV"].ToString());

            string usuario = Session["login"].ToString().ToUpper();
            //string usuario = "(Todos)";
            string status = "(Todos)";

            #endregion

            int anoMesInicial = Convert.ToInt32(Convert.ToDateTime(Session["dataInicialRDV"].ToString())
                .ToString("yyyyMM"));
            int anoMesFinal = Convert.ToInt32(Convert.ToDateTime(Session["dataFinalRDV"].ToString())
                .ToString("yyyyMM"));

            Session["ListaRDV"] = ListaFatura(anoMesInicial, anoMesFinal, usuario, status);

            return View("ListaFaturaCartaoCorporativo");
        }

        public ActionResult ClassificaFatura(string numRDV)
        {
            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            #region Carrega Variaveis Pequisa

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            List<RDV> rdvUrl = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

            #endregion

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"].ToString() != "")
                {
                    if (rdvUrl == null)
                    {
                        ViewBag.Erro = "Esse RDV não existe mais no sistema! Por favor, verifique!";
                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = Session["login"].ToString();
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["ListaClassificaRDV"] = FilterListaFatura();

                        return View("ListaFaturaCartaoCorporativo");
                    }

                    if (Session["login"].ToString().ToUpper() == rdvUrl.FirstOrDefault().Usuario.ToUpper())
                    {
                        CleanSessions();

                        Session["usuarioSelecionado"] = Session["login"].ToString();
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["urlChamada"] = "";
                        Session["ListaClassificaRDV"] = FilterListaFatura();
                    }
                    else
                    {
                        ViewBag.Erro = "Você não tem acesso a esse RDV, pois você não tem permissão para classificá-lo!";

                        CleanSessions();

                        Session["ListaFuncionariosPesquisa"] = CarregaListaFuncionarios(true);
                        Session["usuarioSelecionado"] = Session["login"].ToString();
                        Session["statusSelecionado"] = "(Todos)";
                        Session["tipoLancamentoSelecionado"] = "(Todos)";
                        Session["ListaClassificaRDV"] = FilterListaFatura();

                        return View("ListaFaturaCartaoCorporativo");
                    }
                }
            }

            Session["ListaTipoDespesaRDVNacional"] = CarregaListaTipoDespesa("(DN)");
            Session["ListaTipoDespesaRDVInternacional"] = CarregaListaTipoDespesa("(DI)");
            Session["ListaPaises"] = CarregaListaPaises(true);
            Session["ListaPaisesExterior"] = CarregaListaPaises(false);
            Session["ListaTipoCombustivel"] = CarregaListaTipoCombustivel();

            //Session["ListaClassificaRDV"] = FilterListaFatura()
            //    .Where(w => w.NumeroFechamentoRDV == numRDV).ToList();
            Session["ListaClassificaRDV"] = rdvUrl;

            ViewBag.Titulo = "Classificação do RDV"
                + " - Status: " + ((List<RDV>)Session["ListaClassificaRDV"]).FirstOrDefault().Status
                + " - Nº " + ((List<RDV>)Session["ListaClassificaRDV"]).FirstOrDefault().NumeroFechamentoRDV;

            ViewBag.NomeUsuario = ((List<RDV>)Session["ListaClassificaRDV"]).FirstOrDefault().NomeUsuario;

            return View("ClassificaFatura");
        }

        [HttpPost]
        public ActionResult SaveClassificaFatura(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            try
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var fileIds = ("," + model["id"]).Split(',');
                var selectedTipoDespesa = ("," + model["item.TipoDespesa"]).Split(',');
                var selectedPais = ("," + model["item.NomePais"]).Split(',');
                //var selectedMotivo = ("," + model["motivo"]).Split(',');
                var selectedQtdeDiarias = ("," + model["qtdeDiarias"]).Split(',');
                //var selectedValorDiaria = ("," + model["valorDiaria"]).Split(',');
                var selectedTipoCombustivel = ("," + model["item.TipoCombustivel"]).Split(',');
                string numRDV = "";
                string mesAnoFatura = "";

                string status = "Fechado";
                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController.GetGroup("HLBAPPM-RDVSemAprovacao", (System.Collections.ArrayList)Session["Direitos"]))
                    status = "Aprovado";
                
                for (int i = 0; i < selectedTipoDespesa.Count(); i++)
                {
                    string id = fileIds[i];
                    string tipoDespesa = selectedTipoDespesa[i];
                    string pais = selectedPais[i];
                    //string motivo = selectedMotivo[i];
                    string motivo = model["motivo_" + id];
                    string qtdeDiarias = selectedQtdeDiarias[i];
                    //string valorDiaria = selectedValorDiaria[i];
                    string valorDiaria = model["valorDiaria_" + id];

                    string kmAtual = model["kmAtual_" + id];
                    string qtdeLitros = model["qtdeLitros_" + id];
                    string valorLitro = model["valorLitro_" + id];
                    //string tipoCombustivel = selectedTipoCombustivel[i];
                    string placa = model["placa_" + id];
                    //ArrayList direitosUsuarioRDV = null;

                    int fileId;
                    if (int.TryParse(id, out fileId))
                    {
                        RDV lancamento = hlbapp.RDV.Where(w => w.ID == fileId).FirstOrDefault();

                        #region Carrega usuário do RDV para verificar se já sai aprovado

                        //if (direitosUsuarioRDV == null)
                        //{
                        //    direitosUsuarioRDV = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        //        .GetGroups(lancamento.Usuario.ToLower(), "LDAP://DC=hylinedobrasil,DC=com,DC=br");
                        //    if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                        //        .GetGroup("HLBAPPM-RDVSemAprovacao", direitosUsuarioRDV))
                        //        status = "Aprovado";
                        //}

                        #endregion

                        if (lancamento != null)
                        {
                            numRDV = lancamento.NumeroFechamentoRDV;
                            mesAnoFatura = lancamento.MesAnoFatura;
                            if (tipoDespesa != "")
                            {
                                lancamento.TipoDespesa = tipoDespesa;
                                //lancamento.Status = status;

                                //if (status == "Aprovado")
                                //{
                                //    lancamento.DataAprovacao = DateTime.Now;
                                //    lancamento.UsuarioAprovacao = Session["login"].ToString().ToUpper();
                                //}
                                //else
                                //{
                                //    lancamento.DataAprovacao = null;
                                //    lancamento.UsuarioAprovacao = null;
                                //}

                                decimal limite = VerificaLimiteMetodo(tipoDespesa);
                                decimal valorCompara =0;
                                if (tipoDespesa.Contains("HOSPEDAGEM") || tipoDespesa.Contains("KILOMETRAGEM"))
                                {
                                    if (valorDiaria != "") valorCompara = Convert.ToDecimal(valorDiaria);
                                }
                                else
                                {
                                    valorCompara = lancamento.ValorDespesa;
                                }

                                if (tipoDespesa.Contains("OUTROS") || tipoDespesa.Contains("KILOMETRAGEM")
                                    || tipoDespesa.Contains("OUTROS") || tipoDespesa.Contains("DESPESA NÃO PERMITIDA") || 
                                    (valorCompara > limite && limite > 0))
                                {
                                    if (motivo != "")
                                    {
                                        lancamento.Motivo = motivo;
                                    }
                                }
                                else
                                {
                                    lancamento.Motivo = "";
                                }
                            }
                            if (pais != "")
                            {
                                lancamento.CodPais = pais;
                                MvcAppHylinedoBrasilMobile.Models.PAIS paisObj = 
                                    hlbapp.PAIS.Where(w => w.Sigla == pais).FirstOrDefault();
                                lancamento.NomePais = paisObj.Nome;
                            }

                            if (tipoDespesa.Contains("HOSPEDAGEM") || tipoDespesa.Contains("KILOMETRAGEM"))
                            {
                                if (qtdeDiarias != "")
                                    lancamento.QtdeDiarias = Convert.ToDecimal(qtdeDiarias);
                                    //lancamento.QtdeDiarias = Convert.ToDecimal(qtdeDiarias.Replace(".", ","));
                                else
                                    lancamento.QtdeDiarias = 0;
                                if (valorDiaria != "")
                                    lancamento.ValorDiaria = Convert.ToDecimal(valorDiaria.Replace(".",","));
                                else
                                    lancamento.ValorDiaria = 0;
                            }
                            else
                            {
                                lancamento.QtdeDiarias = 0;
                                lancamento.ValorDiaria = 0;
                            }

                            //if (tipoDespesa.Contains("COMBUSTÍVEL(DN)"))
                            //{
                            //    if (kmAtual != "")
                            //        lancamento.Km = Convert.ToDecimal(kmAtual);
                            //    else
                            //        lancamento.Km = 0;
                            //    if (qtdeLitros != "")
                            //        lancamento.QtdeLitros = Convert.ToDecimal(qtdeLitros.Replace(".", ","));
                            //    else
                            //        lancamento.QtdeLitros = 0;
                            //    if (valorLitro != "")
                            //        lancamento.ValorLitro = Convert.ToDecimal(valorLitro.Replace(".", ","));
                            //    else
                            //        lancamento.ValorLitro = 0;
                            //    lancamento.TipoCombustivel = tipoCombustivel;
                            //    lancamento.Placa = placa;
                            //}
                            //else
                            //{
                                lancamento.Km = 0;
                                lancamento.QtdeLitros = 0;
                                lancamento.ValorLitro = 0;
                                lancamento.TipoCombustivel = "";
                                lancamento.Placa = "";
                            //}
                            //else if (tipoDepesa == "" 
                            //    && (lancamento.TipoDespesa == "" || lancamento.TipoDespesa == null))
                            //{
                            //    if (lancamento.CodCidade.Equals("Nacional"))
                            //        lancamento.TipoDespesa = ((List<SelectListItem>)Session["ListaTipoDespesaRDVNacional"])
                            //            .FirstOrDefault().Text;
                            //    else
                            //        lancamento.TipoDespesa = ((List<SelectListItem>)Session["ListaTipoDespesaRDVInternacional"])
                            //            .FirstOrDefault().Text;
                            //}

                            #region Insere LOG Lançamento

                            if (tipoDespesa != "") InsereLOGLancamentoRDV(lancamento, "Classificação Fatura");

                            #endregion
                        }
                    }
                }

                hlbapp.SaveChanges();

                List<RDV> listRdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).ToList();

                #region Verifica se todos foram preenchidos

                int existeNaoPreenchido = listRdv.Where(w => w.TipoDespesa == "").Count();
                if (existeNaoPreenchido > 0) status = "Pendente";

                foreach (var lancamento in listRdv)
                {
                    lancamento.Status = status;

                    if (status == "Aprovado")
                    {
                        lancamento.DataAprovacao = DateTime.Now;
                        lancamento.UsuarioAprovacao = Session["login"].ToString().ToUpper();
                    }
                    else
                    {
                        lancamento.DataAprovacao = null;
                        lancamento.UsuarioAprovacao = null;
                    }
                }

                hlbapp.SaveChanges();

                #endregion

                #region Insere LOG

                LOG_RDV log = new LOG_RDV();
                log.DataHora = DateTime.Now;
                log.Usuario = Session["login"].ToString().ToUpper();
                if (status == "Pendente")
                    log.Operacao = "Classificação Fatura Parcial";
                else
                    log.Operacao = "Classificação Fatura Total";
                log.NumeroFechamentoRDV = numRDV;
                log.Status = status;

                hlbapp.LOG_RDV.AddObject(log);

                hlbapp.SaveChanges();

                #endregion

                #region Enviar E-mail

                if (existeNaoPreenchido == 0 && status == "Fechado")
                {
                    Apolo10Entities apolo2 = new Apolo10Entities();
                    RDV rdv = hlbapp.RDV.Where(w => w.NumeroFechamentoRDV == numRDV).FirstOrDefault();

                    int existePendente = listRdv.Where(w => w.Status == "Pendente").Count();

                    FUNCIONARIO gerente = apolo2.FUNCIONARIO
                        .Where(w => apolo2.GRP_FUNC
                            .Any(a => a.FuncCod == w.FuncCod
                                && a.GrpFuncObs == "RDV"
                                && apolo2.FUNCIONARIO
                                    .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == rdv.Usuario)))
                        .FirstOrDefault();

                    if (gerente != null)
                    {
                        USUARIO usuarioGerente = apolo2.USUARIO.Where(w => w.UsuCod == gerente.UsuCod).FirstOrDefault();
                        USUARIO usuario = apolo2.USUARIO.Where(w => w.UsuCod == rdv.Usuario).FirstOrDefault();

                        string paraNome = gerente.FuncNome;
                        string paraEmail = usuarioGerente.UsuEmail;
                        string copiaPara = usuario.UsuEmail;
                        //string paraNome = "Paulo Alves";
                        //string paraEmail = "palves@hyline.com.br";
                        //string copiaPara = "";
                        string assunto = "FATURA " + mesAnoFatura + " - " + rdv.NomeUsuario + " P/ APROVAÇÃO";
                        string stringChar = "" + (char)13 + (char)10;
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "";
                        if (rdv.Empresa == "BR") empresaApolo = "5";
                        else if (rdv.Empresa == "LB") empresaApolo = "7";
                        else if (rdv.Empresa == "HN") empresaApolo = "14";
                        else if (rdv.Empresa == "PL") empresaApolo = "20";

                        string porta = "";
                        if (Request.Url.Port != 80)
                            porta = ":" + Request.Url.Port.ToString();

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "A Fatura de " + mesAnoFatura + " do funcionário " + usuario.UsuNome
                            + " foi classificada e está disponível para aprovação. " + stringChar + stringChar
                            + "Clique no link a seguir para poder realizar a aprovação: "
                            + "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numRDV
                            + stringChar + stringChar
                            + "SISTEMA WEB";

                        EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);
                    }
                }

                #endregion

                //Session["usuarioSelecionado"] = "(Todos)";
                Session["usuarioSelecionado"] = Session["login"].ToString().ToUpper();
                Session["statusSelecionado"] = "(Todos)";
                Session["ListaRDV"] = FilterListaFatura();

                if (existeNaoPreenchido == 0)
                    ViewBag.Mensagem = "Fatura Nº " + numRDV + " classificada e fechada!";
                else
                    ViewBag.Mensagem = "Alguns itens da Fatura Nº " + numRDV + " foram classificados, " 
                        + "porém existem alguns itens ainda pendentes! Classifique assim que possível!";

                return View("ListaFaturaCartaoCorporativo");
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

                return View("ListaFaturaCartaoCorporativo");

                #endregion
            }
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            bool permissaoFinanceiro = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                .GetGroup("HLBAPPM-RDVFinanceiro", (System.Collections.ArrayList)Session["Direitos"]);
            Session["permissaoFinanceiro"] = permissaoFinanceiro;
            bool permissaoAprovacao = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                .GetGroup("HLBAPPM-RDVAprovar", (System.Collections.ArrayList)Session["Direitos"]);
            Session["permissaoAprovacao"] = permissaoAprovacao;

            Session["idSelecionado"] = 0;
            Session["ListaRDV"] = new List<RDV>();
            Session["ListaRDVMensal"] = new List<RDVMensal>();
            if (Session["dataInicialRDV"] == null)
                Session["dataInicialRDV"] = new DateTime(DateTime.Today.Year, 1, 1);
            if (Session["dataFinalRDV"] == null) Session["dataFinalRDV"] = DateTime.Today;
            if (Session["usuarioSelecionado"] == null)
            {
                if (Convert.ToBoolean(Session["permissaoFinanceiro"]))
                    Session["usuarioSelecionado"] = "(Todos)";
                else
                    Session["usuarioSelecionado"] = Session["login"].ToString();
            }
            if (Session["statusSelecionado"] == null) Session["statusSelecionado"] = "(Todos)";
            if (Session["tipoLancamentoSelecionado"] == null) Session["tipoLancamentoSelecionado"] = "(Todos)";
            if (Session["formaPagamentoSelecionada"] == null) Session["formaPagamentoSelecionada"] = "(Todas)";

            Session["valorDiariaDespesaRDV"] = "";
            Session["valorMoedaEstrangeiraRDV"] = "";
            Session["qtdeKMRDV"] = "";
            Session["kmAtualRDV"] = "0";
            Session["qtdeLitrosRDV"] = "";
            Session["valorLitroRDV"] = "";

            Session["filtroLocalRDV"] = "";
            Session["empresaSelecionadaRDV"] = "";
            Session["dataRDV"] = DateTime.Today;
            Session["TipoDespesaSelecionadaRDV"] = "";
            Session["IndiceEconomicoSelecionadoRDV"] = "";
            Session["descricaoRDV"] = "";
            Session["LocalSelecionado"] = "";
            Session["valorDespesaRDV"] = "";
            Session["imagem"] = "";
            Session["cidadeRDV"] = "";
            Session["motivoRDV"] = "";
            Session["origemRDV"] = "";
            Session["placaRDV"] = "";
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

        public MemoryStream ComprimirImagem(Image imagem, long qualidade)
        {
            MemoryStream stream = new MemoryStream();
            var param = new EncoderParameters(1);
            param.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualidade);
            var codec = ObterCodec(imagem.RawFormat);
            imagem.Save(stream, codec, param);

            return stream;
        }

        private static ImageCodecInfo ObterCodec(ImageFormat formato)
        {
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == formato.Guid);
            if (codec == null) throw new NotSupportedException();
            return codec;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public static bool VerificaGerencia(string gerenteCod, string funcionarioCod)
        {
            bool retorno = false;

            Apolo10Entities apolo2 = new Apolo10Entities();

            FUNCIONARIO gerente = apolo2.FUNCIONARIO
                .Where(w => apolo2.GRP_FUNC
                    .Any(a => a.FuncCod == w.FuncCod
                        && a.GrpFuncObs == "RDV"
                        && apolo2.FUNCIONARIO
                            .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == funcionarioCod)))
                .FirstOrDefault();

            if (gerente != null)
            {
                if (gerente.UsuCod == gerenteCod)
                    retorno = true;
            }
            else
            {
                bdApoloEntities apolo = new bdApoloEntities();

                VENDEDOR gerenteVendedor = apolo.VENDEDOR
                    .Where(w => apolo.SUP_VENDEDOR
                        .Any(a => a.SupVendCod == w.VendCod
                            && apolo.VENDEDOR.Any(n => n.VendCod == a.VendCod
                                && n.USERLoginSite.Trim() == funcionarioCod)
                            && a.FxaCod.Equals("0000002")))
                    .FirstOrDefault();

                if (gerenteVendedor != null)
                {
                    if (gerenteVendedor.UsuCod == gerenteCod)
                        retorno = true;
                }
            }

            return retorno;
        }

        public string VerificaFormatoArquivo(string caminho)
        {
            string formatoArquivo = Request.Files[0].ContentType;

            if (formatoArquivo.Equals("application/vnd.ms-excel"))
            {
                object oMissing = System.Reflection.Missing.Value;
                Excel.Application oExcel = new Excel.Application();

                oExcel.Visible = false;
                oExcel.DisplayAlerts = false;
                Excel.Workbooks oBooks = oExcel.Workbooks;
                Excel._Workbook oBook = null;
                oBook = oBooks.Open(caminho, false, oMissing,
                    oMissing, oMissing, oMissing, true, oMissing, oMissing,
                    //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                    oMissing, false, oMissing, oMissing, oMissing, oMissing);

                caminho = caminho + "x";

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }

                oBook.SaveAs(caminho, Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, false, false, Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlOtherSessionChanges, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // Quit Excel and clean up.
                oBook.Close(true, oMissing, oMissing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
                oBook = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
                oBooks = null;
                oExcel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
                oExcel = null;

                //P.Kill();

                GC.Collect();
            }

            return caminho;
        }

        public static decimal VerificaLimiteMetodo(string id)
        {
            decimal limite = 0;

            Apolo10Entities apolo2Session = new Apolo10Entities();
            var tipoDespesa = apolo2Session.TIPO_DESPESA
                .Where(w => w.TipoDespNome == id)
                .FirstOrDefault();

            if (tipoDespesa != null)
            {
                limite = Convert.ToDecimal(tipoDespesa.TipoDespValUnitMax);
            }

            return limite;
        }

        public static List<TIPO_DESPESA> CarregaListaDespesasMetodo()
        {
            Apolo10Entities apolo2Session = new Apolo10Entities();
            var listaTipoDespesas = apolo2Session.TIPO_DESPESA.ToList();

            return listaTipoDespesas;
        }

        public void InsereLOGLancamentoRDV(RDV rdv, string operacao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            LOG_RDV_LANCAMENTO log = new LOG_RDV_LANCAMENTO();

            log.DataHoraOperacao = DateTime.Now;
            log.UsuarioOperacao = Session["login"].ToString().ToUpper();
            log.Operacao = operacao;
            log.IDRDV = rdv.ID;
            log.Empresa = rdv.Empresa;
            log.Usuario = rdv.Usuario;
            log.DataHora = rdv.DataHora;
            log.NomeUsuario = rdv.NomeUsuario;
            log.DataRDV = rdv.DataRDV;
            log.TipoDespesa = rdv.TipoDespesa;
            log.Descricao = rdv.Descricao;
            log.CodCidade = rdv.CodCidade;
            log.NomeCidade = rdv.NomeCidade;
            log.ValorDespesa = rdv.ValorDespesa;
            log.ImagemRecibo = rdv.ImagemRecibo;
            log.Status = rdv.Status;
            log.UsuarioAprovacao = rdv.UsuarioAprovacao;
            log.DataAprovacao = rdv.DataAprovacao;
            log.EmpresaDoc = rdv.EmpresaDoc;
            log.ChaveDoc = rdv.ChaveDoc;
            log.FormaPagamento = rdv.FormaPagamento;
            log.ValorMoedaEstrangeira = rdv.ValorMoedaEstrangeira;
            log.IndEconCod = rdv.IndEconCod;
            log.IndEconNome = rdv.IndEconNome;
            log.NumeroFechamentoRDV = rdv.NumeroFechamentoRDV;
            log.Motivo = rdv.Motivo;
            log.MesAnoFatura = rdv.MesAnoFatura;
            log.AnoMes = rdv.AnoMes;
            log.QtdeDiarias = rdv.QtdeDiarias;
            log.ValorDiaria = rdv.ValorDiaria;
            log.CodPais = rdv.CodPais;
            log.NomePais = rdv.NomePais;
            log.Banco = rdv.Banco;
            log.TipoGastoFatura = rdv.TipoGastoFatura;
            log.EmpresaFatura = rdv.EmpresaFatura;
            log.NumeroCartao = rdv.NumeroCartao;
            log.TipoCombustivel = rdv.TipoCombustivel;
            log.QtdeLitros = rdv.QtdeLitros;
            log.ValorLitro = rdv.ValorLitro;
            log.Placa = rdv.Placa;
            log.Km = rdv.Km;

            hlbapp.LOG_RDV_LANCAMENTO.AddObject(log);
            hlbapp.SaveChanges();
        }

        private enum Formats
        {
            General = 0,
            Number = 1,
            Decimal = 2,
            Currency = 164,
            Accounting = 44,
            DateShort = 14,
            DateLong = 165,
            Time = 166,
            Percentage = 10,
            Fraction = 12,
            Scientific = 11,
            Text = 49
        }

        #endregion

        #region Json Methods

        [HttpPost]
        public ActionResult FiltraLocal(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string filtroNome = id.ToUpper();

            Session["filtroLocalRDV"] = id;

            List<SelectListItem> lista2 = (List<SelectListItem>)Session["ListaLocaisOriginal"];

            List<SelectListItem> listaFiltro = lista2.Where(w => w.Text.Contains(filtroNome))
                .OrderBy(o => o.Text).ToList();

            Session["ListaLocaisRDV"] = listaFiltro;

            return Json(listaFiltro);
        }

        [HttpPost]
        public ActionResult VerificaLimite(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            decimal limite = VerificaLimiteMetodo(id);

            return Json(limite);
        }

        [HttpPost]
        public ActionResult CarregaListaDespesas(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            var listaTipoDespesas = CarregaListaDespesasMetodo();

            return Json(listaTipoDespesas);
        }

        [HttpPost]
        public ActionResult AtualizaTipoDespesa(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["origemRDV"] = id;

            string origemRDV = "";
            if (id.Equals("Internacional"))
                origemRDV = "(DI)";
            else
                origemRDV = "(DN)";

            List<SelectListItem> items = CarregaListaTipoDespesa(origemRDV);
            Session["ListaTipoDespesaRDV"] = items;

            return Json(items);
        }

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

        public List<SelectListItem> CarregaListaTipoDespesa(string tipoLancamento)
        {
            List<SelectListItem> listaTipoDespesa = new List<SelectListItem>();

            Apolo10Entities apolo2 = new Apolo10Entities();

            var listaTipoDespesaApolo = apolo2.TIPO_DESPESA
                .OrderBy(o => o.TipoDespNome).ToList();

            foreach (var item in listaTipoDespesaApolo)
            {
                //if (((permissaoCredito && item.TipoDespNome.Contains("(C)")
                //            || item.TipoDespNome.Contains("(DA)")
                //            || item.TipoDespNome.Contains(origemRDV))
                //        && adicionarRDV)
                //    || !adicionarRDV)
                if (item.TipoDespNome.Contains(tipoLancamento))
                {
                    listaTipoDespesa.Add(new SelectListItem
                    {
                        Text = item.TipoDespNome,
                        Value = item.TipoDespNome,
                        Selected = false
                    });
                }
            }

            return listaTipoDespesa;
        }

        public List<SelectListItem> CarregaListaLocais(string nome)
        {
            List<SelectListItem> listaFiltro = new List<SelectListItem>();

            bdApoloEntities apolo = new bdApoloEntities();

            var listaLocais = apolo.CIDADE
                .Where(w => w.CidNomeComp.Contains(nome))
                .GroupBy(g => new { g.CidNomeComp, g.UfSigla, g.PaisSigla })
                .Select(s => new { s.Key.CidNomeComp, s.Key.UfSigla, s.Key.PaisSigla, 
                    codigo = s.Max(m => m.CidCod) })
                .OrderBy(c => c.CidNomeComp).ToList();

            foreach (var item in listaLocais)
            {
                SelectListItem itemddl = new SelectListItem();
                itemddl.Text = item.CidNomeComp + "/" + item.UfSigla + "/" + item.PaisSigla;
                itemddl.Value = item.codigo;
                itemddl.Selected = false;
                listaFiltro.Add(itemddl);
            }

            return listaFiltro;
        }

        public void CarregaEmpresas()
        {
            List<SelectListItem> listaEmpresas = new List<SelectListItem>();
            string empresas = "HNBRLGLBPL";
            for (int i = 0; i < empresas.Length; i = i + 2)
            {
                int count = Session["empresa"].ToString().Length;
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                //string empStr = Session["empresa"].ToString().Substring(i, 2);
                string empStr = empresas.ToString().Substring(i, 2);

                Empresas emp = hlbapp.Empresas
                    .Where(w => w.CodigoCHIC == empStr)
                    .FirstOrDefault();

                listaEmpresas.Add(new SelectListItem
                {
                    Text = emp.Descricaro,
                    Value = empStr,
                    Selected = false
                });
            }

            Session["ListaEmpresasRDV"] = listaEmpresas;
        }

        public List<SelectListItem> CarregaListaFormaPagamento()
        {
            List<SelectListItem> listaFormaPag = new List<SelectListItem>();

            listaFormaPag.Add(new SelectListItem {
                Text = "Cartão Corp.",
                Value = "Cartão Corp.",
                Selected = false
            });
            listaFormaPag.Add(new SelectListItem
            {
                Text = "Espécie",
                Value = "Espécie",
                Selected = false
            });

            return listaFormaPag;
        }

        public List<SelectListItem> CarregaListaOrigem()
        {
            List<SelectListItem> listaFormaPag = new List<SelectListItem>();

            listaFormaPag.Add(new SelectListItem
            {
                Text = "Nacional",
                Value = "Nacional",
                Selected = true
            });
            listaFormaPag.Add(new SelectListItem
            {
                Text = "Internacional",
                Value = "Internacional",
                Selected = false
            });

            Session["origemRDV"] = "Nacional";

            return listaFormaPag;
        }

        public List<SelectListItem> CarregaListaIndicesEconomicos(bool consideraReal)
        {
            List<SelectListItem> listaIndEconomico = new List<SelectListItem>();

            Apolo10Entities apolo2 = new Apolo10Entities();

            var listaIndEconomicoApolo = apolo2.IND_ECONOMICO
                .Where(w => ((w.IndEconNome != "REAL" && !consideraReal) || consideraReal)
                    && w.IndEconSimb != null)
                .OrderBy(o => o.IndEconNome).ToList();

            foreach (var item in listaIndEconomicoApolo)
            {
                listaIndEconomico.Add(new SelectListItem
                {
                    Text = item.IndEconNome + " - " + item.IndEconSimb,
                    Value = item.IndEconCod,
                    Selected = false
                });
            }

            return listaIndEconomico;
        }

        public List<SelectListItem> CarregaListaFuncionarios(bool todos)
        {
            List<SelectListItem> listaFuncionario = new List<SelectListItem>();

            Apolo10Entities apolo2 = new Apolo10Entities();
            bdApoloEntities apolo = new bdApoloEntities();
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string login = Session["login"].ToString().ToUpper();

            #region Funcionários

            FUNCIONARIO usuario = apolo2.FUNCIONARIO
                .Where(w => w.UsuCod == login
                    && w.USERParticipaControleRDVWeb.Equals("Sim"))
                .FirstOrDefault();

            var listaFuncionarioApolo = apolo2.FUNCIONARIO
                .Where(w => w.FuncStat != "Demitido" && w.USERParticipaControleRDVWeb.Equals("Sim"))
                .OrderBy(o => o.FuncNome).ToList();

            if (todos)
                listaFuncionario.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = false
                });

            if (usuario != null)
            {
                foreach (var item in listaFuncionarioApolo)
                {
                    //string empresa = "HNBRLGLBPL";
                    for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
                    {
                        string count = Session["empresa"].ToString();
                        string empresa = Session["empresa"].ToString().Substring(i, 2);
                        //string empStr = empresa.ToString().Substring(i, 2);
                        if (empresa == item.USEREmpres)
                        {
                            if (!MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                                .GetGroup("HLBAPPM-RDVFinanceiro", (System.Collections.ArrayList)Session["Direitos"]))
                            {
                                int existeFuncionarioXUsuario = apolo2.GRP_FUNC
                                    .Where(w => w.FuncCod == usuario.FuncCod
                                        && w.GrpFuncCod == item.FuncCod)
                                    .Count();

                                if (existeFuncionarioXUsuario > 0)
                                {
                                    listaFuncionario.Add(new SelectListItem
                                    {
                                        Text = item.FuncNome,
                                        Value = item.UsuCod,
                                        Selected = false
                                    });
                                }
                            }
                            else
                            {
                                listaFuncionario.Add(new SelectListItem
                                {
                                    Text = item.FuncNome,
                                    Value = item.UsuCod,
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }

            #endregion

            #region Vendedores

            var listaVendedores = apolo.VENDEDOR
                .Where(w => w.USERParticipaControleRDVWeb == "Sim")
                .ToList();

            foreach (var item in listaVendedores)
            {
                for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
                {
                    string empresa = Session["empresa"].ToString().Substring(i, 2);

                    Empresas empresaObj = hlbapp.Empresas
                        .Where(w => w.DescricaoApoloVendedor == item.USEREmpresa)
                        .FirstOrDefault();

                    if (empresa == empresaObj.CodigoCHIC)
                    {
                        if (!MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                            .GetGroup("HLBAPPM-RDVFinanceiro", (System.Collections.ArrayList)Session["Direitos"]))
                        {
                            VENDEDOR supervisor = apolo.VENDEDOR
                                .Where(w => w.UsuCod == login
                                    && apolo.SUP_VENDEDOR
                                    .Any(a => a.SupVendCod == w.VendCod
                                        && a.FxaCod == "0000002"
                                        && a.VendCod == item.VendCod)).FirstOrDefault();

                            if (supervisor != null)
                            {
                                listaFuncionario.Add(new SelectListItem
                                {
                                    Text = item.VendNome,
                                    Value = item.USERLoginSite,
                                    Selected = false
                                });
                            }
                        }
                        else
                        {
                            listaFuncionario.Add(new SelectListItem
                            {
                                Text = item.VendNome,
                                Value = item.USERLoginSite,
                                Selected = false
                            });
                        }
                    }
                }
            }

            #endregion

            return listaFuncionario.OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> CarregaListaStatus()
        {
            List<SelectListItem> listaStatus = new List<SelectListItem>();

            listaStatus.Add(new SelectListItem
            {
                Text = "(Todos)",
                Value = "(Todos)",
                Selected = false
            });
            listaStatus.Add(new SelectListItem
            {
                Text = "Pendente",
                Value = "Pendente",
                Selected = false
            });
            listaStatus.Add(new SelectListItem
            {
                Text = "Fechado",
                Value = "Fechado",
                Selected = false
            });
            listaStatus.Add(new SelectListItem
            {
                Text = "Aprovado",
                Value = "Aprovado",
                Selected = false
            });
            listaStatus.Add(new SelectListItem
            {
                Text = "Importado",
                Value = "Importado",
                Selected = false
            });

            return listaStatus;
        }

        public List<SelectListItem> CarregaListaPaises(bool consideraBrasil)
        {
            List<SelectListItem> listaPaises = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaPaiseBD = hlbapp.PAIS
                .OrderBy(o => o.Nome).ToList();

            foreach (var item in listaPaiseBD)
            {
                if ((consideraBrasil && item.Sigla.Equals("BR")) || !item.Sigla.Equals("BR"))
                {
                    listaPaises.Add(new SelectListItem
                    {
                        Text = item.Nome,
                        Value = item.Sigla,
                        Selected = false
                    });
                }
            }

            return listaPaises;
        }

        public List<SelectListItem> CarregaListaTipoLancamento()
        {
            List<SelectListItem> listaTipoLancamento = new List<SelectListItem>();

            listaTipoLancamento.Add(new SelectListItem
            {
                Text = "(Todos)",
                Value = "(Todos)",
                Selected = false
            });
            listaTipoLancamento.Add(new SelectListItem
            {
                Text = "Débito",
                Value = "(D",
                Selected = false
            });
            listaTipoLancamento.Add(new SelectListItem
            {
                Text = "Crédito",
                Value = "(C",
                Selected = false
            });

            return listaTipoLancamento;
        }

        public List<SelectListItem> CarregaAnoMesFaturaBB()
        {
            List<SelectListItem> listaFaturas = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaFaturasBD = hlbapp.RDV
                .Where(w => w.Banco == "Banco do Brasil")
                .GroupBy(g => g.MesAnoFatura)
                .OrderBy(o => o.Key).ToList();

            foreach (var item in listaFaturasBD)
            {
                listaFaturas.Add(new SelectListItem
                {
                    Text = item.Key,
                    Value = item.Key,
                    Selected = false
                });
            }

            return listaFaturas;
        }

        public List<SelectListItem> CarregaListaTipoCombustivel()
        {
            List<SelectListItem> listaTipoCombustivel = new List<SelectListItem>();

            listaTipoCombustivel.Add(new SelectListItem
            {
                Text = "Etanol",
                Value = "Etanol",
                Selected = false
            });
            listaTipoCombustivel.Add(new SelectListItem
            {
                Text = "Diesel",
                Value = "Diesel",
                Selected = false
            });
            listaTipoCombustivel.Add(new SelectListItem
            {
                Text = "Gasolina",
                Value = "Gasolina",
                Selected = false
            });

            return listaTipoCombustivel;
        }

        #endregion

        #region Event Methods

        public ActionResult AprovarRDV(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            RDV rdv = hlbapp.RDV.Where(w => w.ID == id).FirstOrDefault();
            rdv.UsuarioAprovacao = Session["login"].ToString();
            rdv.DataAprovacao = DateTime.Now;
            rdv.Status = "Aprovado";
            hlbapp.SaveChanges();

            Session["ListaRDV"] = FilterListaRDV();

            return View("Index");
        }

        public ActionResult DesaprovarRDV(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            RDV rdv = hlbapp.RDV.Where(w => w.ID == id).FirstOrDefault();
            rdv.UsuarioAprovacao = null;
            rdv.DataAprovacao = null;
            rdv.Status = "Pendente";
            hlbapp.SaveChanges();

            Session["ListaRDV"] = FilterListaRDV();

            return View("Index");
        }

        public ActionResult AprovarRDVSemanal()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            List<RDV> listaRDV = (List<RDV>)Session["ListaVisualizaRDV"];

            foreach (var item in listaRDV)
            {
                RDV rdAtualiza = hlbapp.RDV.Where(w => w.ID == item.ID).FirstOrDefault();
                rdAtualiza.UsuarioAprovacao = Session["login"].ToString();
                rdAtualiza.DataAprovacao = DateTime.Now;
                rdAtualiza.Status = "Aprovado";
            }

            hlbapp.SaveChanges();

            Session["ListaRDVMensal"] = FilterListaRDVMensal();

            return View("ListaMensalRDV");
        }

        public ActionResult EnviarEmailRDVPendente()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            hlbappSession.USER_Envia_Email_Aviso_RDV();

            CleanSessions();
            if (Session["usuarioSelecionado"] == null) Session["usuarioSelecionado"] = "(Todos)";
            if (Session["statusSelecionado"] == null) Session["statusSelecionado"] = "(Todos)";
            if (Session["tipoLancamentoSelecionado"] == null) Session["tipoLancamentoSelecionado"] = "(Todos)";
            if (Session["formaPagamentoSelecionada"] == null) Session["formaPagamentoSelecionada"] = "(Todas)";
            Session["ListaRDV"] = FilterListaRDV();

            ViewBag.Mensagem = "E-mail de Aviso RDV em aberto / não aprovado enviado com sucesso!";

            return View("ListaRDVFechadosGeral");
        }

        #endregion
    }
}