using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using System.Data.Entity.SqlServer;
using System.Data.Objects;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class OrcamentoController : Controller
    {
        #region Menus

        public ActionResult MenuOrcamento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            //Session["ListaFornecedoresInvOriginal"] = CarregaFornecedores();
            //Session["ListaProdutosApoloOriginal"] = CarregaProdutosApolo()

            return View();
        }

        #endregion

        #region Configurações

        #region Ano Fiscal

        #region List Methods

        public List<AnoFiscal> ListAnoFiscal(int anoInicial, int anoFinal)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            List<AnoFiscal> retorno = new List<AnoFiscal>();

            string anoInicialStr = anoInicial.ToString();

            var listaAnoFiscalAll = hlbapp.AnoFiscal.ToList();

            foreach (var item in listaAnoFiscalAll)
            {
                int itemAnoInicial = Convert.ToInt32(item.AnoFiscal1.Substring(0, 4));
                int itemAnoFinal = Convert.ToInt32(item.AnoFiscal1.Substring(5, 4));

                if (itemAnoInicial >= anoInicial && itemAnoFinal <= anoFinal)
                    retorno.Add(item);
            }

            return retorno.OrderBy(o => o.AnoFiscal1).ToList();
        }

        public List<AnoFiscal> FilterListAnoFiscal()
        {
            CleanSessions();

            int anoInicial = Convert.ToInt32(Session["anoInicialConf"]);
            int anoFinal = Convert.ToInt32(Session["anoFinalConf"]);

            List<AnoFiscal> listaAnoFiscal = ListAnoFiscal(anoInicial, anoFinal);

            return listaAnoFiscal;
        }

        #endregion

        #region Lista Ano Fiscal

        public ActionResult ListaAnoFiscal()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["ListaFechaAlteracaoInv"] = CarregaListaSimNao();
            Session["ListaAnoFiscal"] = FilterListAnoFiscal();
            return View();
        }

        public ActionResult SearchAnoFiscal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            int anoInicial = 0;
            if (model["anoInicial"] != null)
            {
                anoInicial = Convert.ToInt32(model["anoInicial"]);
                Session["anoInicialConf"] = anoInicial;
            }

            int anoFinal = 0;
            if (model["anoFinal"] != null)
            {
                anoFinal = Convert.ToInt32(model["anoFinal"]);
                Session["anoFinalConf"] = anoFinal;
            }

            #endregion

            Session["ListaAnoFiscal"] = ListAnoFiscal(anoInicial, anoFinal);

            return View("ListaAnoFiscal");
        }

        #endregion

        #region CRUD Methods

        public void CarregaAnoFiscal(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            AnoFiscal af = hlbapp.AnoFiscal.Where(w => w.ID == id).FirstOrDefault();

            Session["anoFiscalConf"] = af.AnoFiscal1;

            Session["anoInicialConf"] = Convert.ToInt32(af.AnoFiscal1.Substring(0, 4));
            Session["anoFinalConf"] = Convert.ToInt32(af.AnoFiscal1.Substring(5, 4));

            if (Session["ListaFechaAlteracaoInv"] != null)
                AtualizaDDL(af.FechaAlteracaoInvestimento, (List<SelectListItem>)Session["ListaFechaAlteracaoInv"]);
        }

        public ActionResult ConfirmaCreateAnoFiscal()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View();
        }
        
        public ActionResult CreateAnoFiscal()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            MvcAppHylinedoBrasilMobile.Models.HLBAPPEntities hlbapp = new MvcAppHylinedoBrasilMobile.Models.HLBAPPEntities();

            string novoAnoFiscal = "2018-2019";
            MvcAppHylinedoBrasilMobile.Models.AnoFiscal marioAnoFiscalObj = hlbapp.AnoFiscal.OrderByDescending(o => o.AnoFiscal1).FirstOrDefault();
            if (marioAnoFiscalObj != null)
            {
                novoAnoFiscal = (Convert.ToInt32(marioAnoFiscalObj.AnoFiscal1.Substring(0, 4)) + 1).ToString() + "-"
                    + (Convert.ToInt32(marioAnoFiscalObj.AnoFiscal1.Substring(5, 4)) + 1).ToString();
            }

            AnoFiscal afObj = new AnoFiscal();
            afObj.Usuario = Session["login"].ToString().ToUpper();
            afObj.DataCadastro = DateTime.Now;
            afObj.AnoFiscal1 = novoAnoFiscal;
            afObj.FechaAlteracaoInvestimento = "Não";
            hlbapp.AnoFiscal.AddObject(afObj);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Ano Fiscal " + novoAnoFiscal + " criado com sucesso!";

            Session["ListaAnoFiscal"] = FilterListAnoFiscal();
            return View("ListaAnoFiscal");
        }

        public ActionResult EditAnoFiscal(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaAnoFiscal(id);

            return View("AnoFiscal");
        }

        public ActionResult SaveAnoFiscal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["FechaAlteracaoInvestimento"] != null)
            {
                #region Carrega Valores

                #region Fecha Alteração de Investimento

                string fechaAlteracaoInvestimento = "";
                if (model["FechaAlteracaoInvestimento"] != null) 
                    fechaAlteracaoInvestimento = model["fechaAlteracaoInvestimento"];

                #endregion

                #endregion

                #region Altera no WEB

                int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                AnoFiscal anoFiscalObj = hlbapp.AnoFiscal.Where(w => w.ID == idSelecionado).FirstOrDefault();

                anoFiscalObj.FechaAlteracaoInvestimento = fechaAlteracaoInvestimento;

                #endregion

                hlbapp.SaveChanges();
            }

            Session["ListaAnoFiscal"] = FilterListAnoFiscal();
            return View("ListaAnoFiscal");
        }

        public ActionResult ConfirmaDeleteAnoFiscal(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            AnoFiscal anoFiscalObj = hlbapp.AnoFiscal.Where(w => w.ID == id).FirstOrDefault();
            int anoMesInicial = Convert.ToInt32(anoFiscalObj.AnoFiscal1.Substring(0, 4) + "07");
            int anoMesFinal = Convert.ToInt32(anoFiscalObj.AnoFiscal1.Substring(5, 4) + "06");

            int existe = hlbapp.Investimento.Where(w => w.AnoMesInicial == anoMesInicial
                && w.AnoMesFinal == anoMesFinal).Count();

            if (existe > 0)
            {
                ViewBag.Erro = "Não é possível excluir o Ano Fiscal " + anoFiscalObj.AnoFiscal1
                    + " pois existem Investimentos cadastrados!";
                Session["ListaAnoFiscal"] = FilterListAnoFiscal();
                return View("ListaAnoFiscal");
            }

            return View();
        }

        public ActionResult DeleteAnoFiscal()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            AnoFiscal afObj = hlbapp.AnoFiscal.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.AnoFiscal.DeleteObject(afObj);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Ano Fiscal " + afObj.AnoFiscal1 + " excluído com sucesso!";

            Session["ListaAnoFiscal"] = FilterListAnoFiscal();
            return View("ListaAnoFiscal");
        }

        #endregion

        #endregion

        #endregion

        #region Investimentos

        #region Cadastro de Investimentos

        #region List Methods

        public List<Investimento> ListInvestimentos(int anoMesInicial, int anoMesFinal, string departamento)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();
            string login = Session["login"].ToString().ToUpper();

            List<Investimento> retorno = new List<Investimento>();

            var listaInvestimentos = hlbapp.Investimento
                .Where(w => w.AnoMesInicial >= anoMesInicial && w.AnoMesFinal <= anoMesFinal
                    && (w.Departamento == departamento || departamento == "(Todos)"))
                .ToList();

            if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                .GetGroup("HLBAPPM-OrcamentoInvestimentoTodos", (System.Collections.ArrayList)Session["Direitos"]))
            {
                retorno = listaInvestimentos;
            }
            else
            {
                foreach (var inv in listaInvestimentos)
                {
                    USUARIO responsavel = apolo.USUARIO
                        .Where(w => apolo.FUNCIONARIO.Any(a => a.FuncCod == inv.Responsavel
                                && w.UsuCod == a.UsuCod)
                            && w.UsuCod == login)
                        .FirstOrDefault();

                    List<USUARIO> gerentesResponsavelInvestimento01 = apolo.USUARIO
                        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                            && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                                && g.FuncCod == r.FuncCod
                                && g.GrpFuncObs == "RDV"))
                            && u.UsuCod == login)
                        .ToList();

                    List<USUARIO> gerentesResponsavelInvestimento02 = apolo.USUARIO
                        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                            && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == r.FuncCod
                                && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                                    && g.FuncCod == g2.GrpFuncCod
                                    && g.GrpFuncObs == "RDV")
                                && g2.GrpFuncObs == "RDV"))
                            && u.UsuCod == login)
                        .ToList();

                    List<USUARIO> gerentesResponsavelInvestimento03 = apolo.USUARIO
                        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                            && apolo.GRP_FUNC.Any(g3 => g3.FuncCod == r.FuncCod
                                && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == g3.GrpFuncCod
                                    && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                                        && g.FuncCod == g2.GrpFuncCod
                                        && g.GrpFuncObs == "RDV")
                                    && g2.GrpFuncObs == "RDV")
                                && g3.GrpFuncObs == "RDV"))
                            && u.UsuCod == login)
                        .ToList();

                    if (responsavel != null || gerentesResponsavelInvestimento01.Count > 0
                        || gerentesResponsavelInvestimento02.Count > 0 || gerentesResponsavelInvestimento03.Count > 0)
                    {
                        retorno.Add(inv);
                    }
                }
            }

            return retorno.OrderBy(o => o.AnoMesInicial).ThenBy(b => b.NomeProjeto).ToList();
        }

        public List<Investimento> FilterListInvestimentos()
        {
            CleanSessions();

            int anoMesInicial = Convert.ToInt32(Convert.ToDateTime(Session["mesAnoInicialInv"].ToString()).ToString("yyyyMM"));
            int anoMesFinal = Convert.ToInt32(Convert.ToDateTime(Session["mesAnoFinalInv"].ToString()).ToString("yyyyMM"));
            string departamento = ((List<SelectListItem>)Session["FiltroListaDepartamentosInv"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;

            List<Investimento> listaInvestimento = ListInvestimentos(anoMesInicial, anoMesFinal, departamento);

            return listaInvestimento;
        }

        #endregion

        #region Lista Investimentos

        public ActionResult ListaInvestimentos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //CleanSessions();
            Session["ListaInvestimentos"] = FilterListInvestimentos();

            return View();
        }

        public ActionResult SearchInvestimentos(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            string anoFiscal = "";
            if (model["AnoFiscal"] != null)
            {
                anoFiscal = model["AnoFiscal"];
                AtualizaDDL(anoFiscal, (List<SelectListItem>)Session["FiltroListaAnoFiscalInv"]);
            }

            int anoMesInicial = Convert.ToInt32(anoFiscal.Substring(0, 4) + "07");
            int anoMesFinal = Convert.ToInt32(anoFiscal.Substring(5, 4) + "06");

            string departamento = "";
            if (model["Departamento"] != null)
            {
                departamento = model["Departamento"];
                AtualizaDDL(departamento, (List<SelectListItem>)Session["FiltroListaDepartamentosInv"]);
            }

            #endregion

            Session["ListaInvestimentos"] = ListInvestimentos(anoMesInicial, anoMesFinal, departamento);

            return View("ListaInvestimentos");
        }

        #endregion

        #region Tabela de Investimentos / Mês

        public ActionResult TabelaInvestimentoMes()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //CleanSessions();
            Session["ListaInvestimentos"] = FilterListInvestimentos();
            return View();
        }

        public ActionResult SearchTabelaInvestimentoMes(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            string anoFiscal = "";
            if (model["AnoFiscal"] != null)
            {
                anoFiscal = model["AnoFiscal"];
                AtualizaDDL(anoFiscal, (List<SelectListItem>)Session["FiltroListaAnoFiscalInv"]);
            }

            int anoMesInicial = Convert.ToInt32(anoFiscal.Substring(0, 4) + "07");
            int anoMesFinal = Convert.ToInt32(anoFiscal.Substring(5, 4) + "06");

            string departamento = "";
            if (model["Departamento"] != null)
            {
                departamento = model["Departamento"];
                AtualizaDDL(departamento, (List<SelectListItem>)Session["FiltroListaDepartamentosInv"]);
            }

            #endregion

            Session["ListaInvestimentos"] = ListInvestimentos(anoMesInicial, anoMesFinal, departamento);

            return View("TabelaInvestimentoMes");
        }

        public ActionResult DetalhesSolicitacaoInvestimentoMes(int idInv, int anoMes)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            #region Carrega Lista de Solicitações de Investimento

            DateTime dataInicial = new DateTime(Convert.ToInt32(anoMes.ToString().Substring(0,4)),
                Convert.ToInt32(anoMes.ToString().Substring(4,2)), 1);
            DateTime dataFinal = dataInicial.AddMonths(1).AddDays(-1);

            var listaSolInvMes = hlbapp.Investimento_Solicitacao
                .Where(w => w.IDInvestimento == idInv
                    && w.DataInicio >= dataInicial && w.DataInicio <= dataFinal)
                .ToList();

            #endregion

            Session["ListaSolicitacaoInvestimentoMes"] = listaSolInvMes;
            return View();
        }

        #endregion

        #region CRUD Methods

        public void CarregaInvestimento(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Investimento inv = hlbapp.Investimento.Where(w => w.ID == id).FirstOrDefault();

            Session["numProjetoInv"] = inv.NumeroProjeto;
            Session["nomeProjetoInv"] = inv.NomeProjeto;
            if (Session["ListaDepartamentosInv"] != null)
                AtualizaDDL(inv.Departamento, (List<SelectListItem>)Session["ListaDepartamentosInv"]);
            Session["ListaResponsavelInv"] = CarregaListaResponsaveis(inv.Departamento);
            if (Session["ListaResponsavelInv"] != null)
                AtualizaDDL(inv.Responsavel, (List<SelectListItem>)Session["ListaResponsavelInv"]);
            if (Request.Browser.IsMobileDevice)
                Session["valorInv"] = inv.ValorAprovado;
            else
                Session["valorInv"] = String.Format("{0:N2}", inv.ValorAprovado);
            string anoFiscal = inv.AnoMesInicial.ToString().Substring(0, 4) + "-" + inv.AnoMesFinal.ToString().Substring(0, 4);
            if (Session["ListaAnoFiscalInv"] != null)
                AtualizaDDL(anoFiscal, (List<SelectListItem>)Session["ListaAnoFiscalInv"]);

            List<Investimento_Mes> listaInvMes = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == id).ToList();
            CarregaSessionValorMes(listaInvMes);
        }

        public ActionResult CreateInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();
            Session["ListaDepartamentosInv"] = CarregaListaDepartamento(false);
            Session["ListaResponsavelInv"] = new List<SelectListItem>();
            Session["ListaAnoFiscalInv"] = CarregaListaAnoFiscal(false, true);

            List<Investimento_Mes> invMes = new List<Investimento_Mes>();
            CarregaSessionValorMes(invMes);

            return View("Investimento");
        }

        public ActionResult EditInvestimento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            Session["ListaDepartamentosInv"] = CarregaListaDepartamento(false);
            Session["ListaAnoFiscalInv"] = CarregaListaAnoFiscal(false, true);
            CarregaInvestimento(id);

            return View("Investimento");
        }

        public ActionResult SaveInvestimento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["numProjeto"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Número do Projeto

                string numeroProjeto = model["numProjeto"];

                #endregion

                #region Nome do Projeto

                string nomeProjeto = "";
                if (model["nomeProjeto"] != null) nomeProjeto = model["nomeProjeto"];

                #endregion

                #region Departamento

                string departamento = "";
                if (model["Departamento"] != null) departamento = model["Departamento"];

                #endregion

                #region Responsável

                string responsavel = "";
                if (model["Responsavel"] != null) responsavel = model["Responsavel"];

                #endregion

                #region Valor Aprovado

                decimal valorAprovado = 0;
                if (model["valor"] != null)
                {
                    if (Request.Browser.IsMobileDevice)
                        valorAprovado = Convert.ToDecimal(model["valor"].ToString().Replace(".", ","));
                    else
                        valorAprovado = Convert.ToDecimal(model["valor"]);
                }

                #endregion

                #region Ano Fiscal

                string anoFiscal = "";
                if (model["AnoFiscal"] != null)
                {
                    anoFiscal = model["AnoFiscal"];
                    AtualizaDDL(anoFiscal, (List<SelectListItem>)Session["FiltroListaAnoFiscalInv"]);
                }

                int anoMesInicial = Convert.ToInt32(anoFiscal.Substring(0, 4) + "07");
                int anoMesFinal = Convert.ToInt32(anoFiscal.Substring(5, 4) + "06");

                #endregion

                #endregion

                #region Insere Investimento no WEB

                Investimento investimento = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    investimento = new Investimento();
                    investimento.Usuario = usuario;
                    investimento.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    investimento = hlbapp.Investimento.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                investimento.NumeroProjeto = numeroProjeto;
                investimento.NomeProjeto = nomeProjeto;
                investimento.Departamento = departamento;
                investimento.Responsavel = responsavel;
                investimento.Origem = VerificaOrigemInvestimento(anoFiscal);

                if (investimento.Origem != "Alemanha") valorAprovado = 0;
                investimento.ValorAprovado = valorAprovado;

                investimento.AnoMesInicial = anoMesInicial;
                investimento.AnoMesFinal = anoMesFinal;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Investimento.AddObject(investimento);

                hlbapp.SaveChanges();

                #endregion

                #region Insere Valores por Mês no WEB

                #region Deleta os dados antigos

                var listaInvMesDel = hlbapp.Investimento_Mes.Where(w => w.IDInvestimento == investimento.ID).ToList();

                foreach (var item in listaInvMesDel)
                {
                    Investimento_Mes invMesDel = hlbapp.Investimento_Mes.Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbapp.Investimento_Mes.DeleteObject(invMesDel);
                }

                hlbapp.SaveChanges();

                #endregion

                
                DateTime dataInicial = new DateTime(Convert.ToInt32(anoFiscal.Substring(0, 4)), 7, 1);
                DateTime dataFinal = new DateTime(Convert.ToInt32(anoFiscal.Substring(5, 4)), 6, 1);

                while (dataInicial <= dataFinal)
                {
                    #region Carrega Valores

                    decimal valorMes = 0;
                    if (investimento.Origem == "Alemanha")
                    {
                        if (Request.Browser.IsMobileDevice)
                            valorMes = Convert.ToDecimal(model["valorMes_" + dataInicial.ToString("MMM")].ToString().Replace(".", ","));
                        else
                            valorMes = Convert.ToDecimal(model["valorMes_" + dataInicial.ToString("MMM")]);
                    }

                    int anoMes = Convert.ToInt32(dataInicial.ToString("yyyyMM"));

                    #endregion

                    #region Insere no WEB

                    Investimento_Mes invMes = hlbapp.Investimento_Mes
                        .Where(w => w.IDInvestimento == investimento.ID && w.AnoMes == anoMes).FirstOrDefault();

                    if (invMes == null)
                    {
                        invMes = new Investimento_Mes();
                        invMes.IDInvestimento = investimento.ID;
                    }

                    invMes.AnoMes = anoMes;
                    invMes.ValorOrcado = valorMes;
                    invMes.ValorUtilizado = 0;
                    invMes.Saldo = valorMes;

                    if (invMes.ID == 0) hlbapp.Investimento_Mes.AddObject(invMes);

                    #endregion

                    dataInicial = dataInicial.AddMonths(1);
                }

                hlbapp.SaveChanges();

                #endregion
            }

            Session["ListaInvestimentos"] = FilterListInvestimentos();
            return View("ListaInvestimentos");
        }

        public ActionResult ConfirmaDeleteInvestimento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            List<Investimento_Mes> listaInvestimentoMes = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == id)
                .ToList();
            foreach (var item in listaInvestimentoMes)
            {
                Investimento_Mes invDel = hlbapp.Investimento_Mes.Where(w => w.ID == item.ID).FirstOrDefault();
                hlbapp.Investimento_Mes.DeleteObject(invDel);
            }

            Investimento inv = hlbapp.Investimento.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Investimento.DeleteObject(inv);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Investimento " + inv.NumeroProjeto + " - " + inv.NomeProjeto + " excluído com sucesso!";

            Session["ListaInvestimentos"] = FilterListInvestimentos();
            return View("ListaInvestimentos");
        }

        #endregion

        #region Event Methods

        #region Atualização do Valor Utilizado

        [HttpGet]
        public ActionResult AlteraValorUtilizadoInvestimentoMes(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["idSelecionado"] = id;

            Investimento_Mes invMes = hlbapp.Investimento_Mes
                .Where(w => w.ID == id).FirstOrDefault();

            return View("AtualizaValorUtilizado");
        }

        [HttpPost]
        public ActionResult AtualizaValorUtilizadoInvestimento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["valorUtilizado"] != null)
            {
                #region Carrega Valores

                #region Valor Aprovado

                decimal valorUtilizado = 0;
                if (model["valorUtilizado"] != null)
                {
                    if (Request.Browser.IsMobileDevice)
                        valorUtilizado = Convert.ToDecimal(model["valorUtilizado"].ToString().Replace(".", ","));
                    else
                        valorUtilizado = Convert.ToDecimal(model["valorUtilizado"]);
                }

                #endregion

                #endregion

                #region Salva Valor Utilizado e Atualizar Saldo no WEB

                int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                Investimento_Mes Investimento_Mes = hlbapp.Investimento_Mes.Where(w => w.ID == idSelecionado).FirstOrDefault();

                Investimento_Mes.Saldo = Investimento_Mes.Saldo + Investimento_Mes.ValorUtilizado;
                Investimento_Mes.ValorUtilizado = valorUtilizado;
                Investimento_Mes.Saldo = Investimento_Mes.Saldo - Investimento_Mes.ValorUtilizado;

                hlbapp.SaveChanges();

                #endregion
            }

            ViewBag.Mensagem = "Valor Utilizado atualizado com sucesso!";

            Session["ListaInvestimentos"] = FilterListInvestimentos();
            return View("TabelaInvestimentoMes");
        }

        #endregion

        #region Transferência de Saldo

        [HttpGet]
        public ActionResult SolicitacaoTransferenciaSaldoInvestimentoMes(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["idSelecionadoMes"] = id;

            #region Carrega Lista de Investimentos para selecionar

            Investimento_Mes invMes = hlbapp.Investimento_Mes
                .Where(w => w.ID == id).FirstOrDefault();

            Investimento inv = hlbapp.Investimento
                .Where(w => w.ID == invMes.IDInvestimento).FirstOrDefault();

            var listaInv = FilterListInvestimentos();

            List<SelectListItem> ddlInv = new List<SelectListItem>();

            foreach (var item in listaInv)
            {
                bool selected = false;
                if (item.ID == invMes.IDInvestimento) selected = true;
                ddlInv.Add(new SelectListItem
                {
                    Text = item.NumeroProjeto + " - " + item.NomeProjeto,
                    Value = item.ID.ToString(),
                    Selected = selected
                });
            }

            Session["ListaInvestimentosTransf"] = ddlInv;

            #endregion

            Session["ListaAnoMesTransf"] = CarregaListaAnoMes(inv.AnoMesInicial, inv.AnoMesFinal);

            Investimento_Mes_Movimentacao_Saldo mov = hlbapp.Investimento_Mes_Movimentacao_Saldo
                .Where(w => w.IDInvestimentoMesOrigem == id
                    && w.UsuarioAprovacao == null).FirstOrDefault();

            Session["idSelecionadoMov"] = 0;
            Session["valorTransferirInv"] = 0;

            if (mov != null)
            {
                #region Se existe, carrega variáveis

                Session["idSelecionadoMov"] = mov.ID;

                if (Request.Browser.IsMobileDevice)
                    Session["valorTransferirInv"] = mov.Valor;
                else
                    Session["valorTransferirInv"] = String.Format("{0:N2}", mov.Valor);

                Investimento_Mes invMesDestino = hlbapp.Investimento_Mes.Where(w => w.ID == mov.IDInvestimentoMesDestino).FirstOrDefault();

                AtualizaDDL(invMesDestino.IDInvestimento.ToString(), (List<SelectListItem>)Session["ListaInvestimentosTransf"]);
                AtualizaDDL(invMesDestino.AnoMes.ToString(), (List<SelectListItem>)Session["ListaAnoMesTransf"]);

                #endregion
            }

            return View("TransferenciaSaldoMes");
        }

        [HttpPost]
        public ActionResult ConfirmaSolicitacaoTransferenciaSaldoInvestimentoMes(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int idSelecionado = Convert.ToInt32(Session["idSelecionadoMes"]);
            Investimento_Mes invMes = hlbapp.Investimento_Mes
                .Where(w => w.ID == idSelecionado).FirstOrDefault();

            #endregion

            if (model["valorTransferir"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Valor a Transferir

                decimal valorUtilizado = 0;
                if (model["valorTransferir"] != null)
                {
                    if (Request.Browser.IsMobileDevice)
                        valorUtilizado = Convert.ToDecimal(model["valorTransferir"].ToString().Replace(".", ","));
                    else
                        valorUtilizado = Convert.ToDecimal(model["valorTransferir"]);
                }

                #endregion

                #region AnoMes a receber

                int mesAno = Convert.ToInt32(model["AnoMesReceber"]);

                #endregion

                #region Investimento a receber

                int invReceber = 0;
                if (model["InvestimentoReceber"] != null)
                {
                    int idInvDest = Convert.ToInt32(model["InvestimentoReceber"]);
                    Investimento_Mes invMesDest = hlbapp.Investimento_Mes
                        //.Where(w => w.IDInvestimento == idInvDest && w.AnoMes == invMes.AnoMes).FirstOrDefault();
                        .Where(w => w.IDInvestimento == idInvDest && w.AnoMes == mesAno).FirstOrDefault();

                    invReceber = invMesDest.ID;
                }

                #endregion

                #endregion

                #region Insere Solicitação de Transferência no WEB

                Investimento_Mes_Movimentacao_Saldo investimento = null;
                if (Convert.ToInt32(Session["idSelecionadoMov"]) == 0)
                {
                    investimento = new Investimento_Mes_Movimentacao_Saldo();
                }
                else
                {
                    int idMov = Convert.ToInt32(Session["idSelecionadoMov"]);
                    investimento = hlbapp.Investimento_Mes_Movimentacao_Saldo.Where(w => w.ID == idMov).FirstOrDefault();
                }

                investimento.Usuario = usuario;
                investimento.DataHoraCadastro = DateTime.Now;
                investimento.IDInvestimentoMesOrigem = invMes.ID;
                investimento.IDInvestimentoMesDestino = invReceber;
                investimento.Valor = valorUtilizado;

                if (Convert.ToInt32(Session["idSelecionadoMov"]) == 0) hlbapp.Investimento_Mes_Movimentacao_Saldo.AddObject(investimento);

                hlbapp.SaveChanges();

                #endregion

                if (MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPPM-OrcamentoInvestimentoTransferenciaSemAprovacao", (System.Collections.ArrayList)Session["Direitos"]))
                {
                    #region Realiza a transferência sem aprovação

                    Investimento_Mes origem = hlbapp.Investimento_Mes.Where(w => w.ID == investimento.IDInvestimentoMesOrigem).FirstOrDefault();
                    origem.Saldo = origem.Saldo - investimento.Valor;
                    Investimento invOrigem = hlbapp.Investimento.Where(w => w.ID == origem.IDInvestimento).FirstOrDefault();
                    Investimento_Mes destino = hlbapp.Investimento_Mes.Where(w => w.ID == investimento.IDInvestimentoMesDestino).FirstOrDefault();
                    destino.Saldo = destino.Saldo + investimento.Valor;
                    Investimento invDestino = hlbapp.Investimento.Where(w => w.ID == destino.IDInvestimento).FirstOrDefault();

                    investimento.UsuarioAprovacao = Session["login"].ToString().ToUpper();
                    investimento.DataHoraAprovacao = DateTime.Now;

                    hlbapp.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Enviar E-mail

                    Apolo10Entities apolo = new Apolo10Entities();

                    USUARIO usuarioObj = apolo.USUARIO.Where(w => w.UsuCod == investimento.Usuario).FirstOrDefault();
                    Investimento invOrigem = hlbapp.Investimento.Where(w => hlbapp.Investimento_Mes.Any(a => a.IDInvestimento == w.ID
                        && a.ID == investimento.IDInvestimentoMesOrigem)).FirstOrDefault();
                    string origem = invOrigem.NumeroProjeto + " - " + invOrigem.NomeProjeto;

                    Investimento invDestino = hlbapp.Investimento.Where(w => hlbapp.Investimento_Mes.Any(a => a.IDInvestimento == w.ID
                        && a.ID == investimento.IDInvestimentoMesDestino)).FirstOrDefault();
                    string destino = invDestino.NumeroProjeto + " - " + invDestino.NomeProjeto;

                    string paraNome = "Davi Nogueira";
                    string paraEmail = "dnogueira@hyline.com.br";
                    string copiaPara = usuarioObj.UsuEmail;
                    string assunto = "SOLICITAÇÃO DE TRANSFERÊNCIA DE SALDO ENTRE INVESTIMENTOS";
                    string stringChar = "" + (char)13 + (char)10;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "";

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "Foi solicitada a transferência de saldo de \"" + String.Format("{0:C}", investimento.Valor)
                        + "\" do investimento " + origem + " para o investimento \"" + destino
                        + "\" pelo usuário " + investimento.Usuario + "." + stringChar + stringChar
                        + "Por favor, avaliar para realizar a aprovação! " + stringChar + stringChar
                        + "SISTEMA WEB";

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                    #endregion
                }
            }

            ViewBag.Mensagem = "Solicitação de Transferência de Valor realizada com sucesso!";
            Session["ListaInvestimentos"] = FilterListInvestimentos();
            return View("TabelaInvestimentoMes");
        }

        #endregion

        #region Aprovação de Transferência

        #region List Methods

        public List<Investimento_Mes_Movimentacao_Saldo> ListInvestimentoMesMovimentacaoSaldo()
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            List<Investimento_Mes_Movimentacao_Saldo> retorno = new List<Investimento_Mes_Movimentacao_Saldo>();

            var lista = hlbapp.Investimento_Mes_Movimentacao_Saldo
                //.Where(w => w.UsuarioAprovacao == null)
                .ToList();

            retorno = lista;

            return retorno;
        }

        public List<Investimento_Mes_Movimentacao_Saldo> FilterListInvestimentoMesMovimentacaoSaldo()
        {
            CleanSessions();

            List<Investimento_Mes_Movimentacao_Saldo> lista = ListInvestimentoMesMovimentacaoSaldo();

            return lista;
        }

        #endregion

        #region Lista Movimentação de Saldo

        public ActionResult ListaInvestimentoMesMovimentacaoSaldo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["msg"] = "";

            //CleanSessions();
            Session["ListaInvestimentoMesMovimentacaoSaldo"] = FilterListInvestimentoMesMovimentacaoSaldo();
            return View();
        }

        #endregion

        #region Aprovação

        public ActionResult AprovarTransferenciaSaldo(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Investimento_Mes_Movimentacao_Saldo mov = hlbapp.Investimento_Mes_Movimentacao_Saldo
                .Where(w => w.ID == id).FirstOrDefault();

            Investimento_Mes origem = hlbapp.Investimento_Mes.Where(w => w.ID == mov.IDInvestimentoMesOrigem).FirstOrDefault();
            origem.Saldo = origem.Saldo - mov.Valor;
            Investimento invOrigem = hlbapp.Investimento.Where(w => w.ID == origem.IDInvestimento).FirstOrDefault();
            Investimento_Mes destino = hlbapp.Investimento_Mes.Where(w => w.ID == mov.IDInvestimentoMesDestino).FirstOrDefault();
            destino.Saldo = destino.Saldo + mov.Valor;
            Investimento invDestino = hlbapp.Investimento.Where(w => w.ID == destino.IDInvestimento).FirstOrDefault();

            string origemStr = invOrigem.NumeroProjeto + " - " + invOrigem.NomeProjeto;
            string destinoStr = invDestino.NumeroProjeto + " - " + invDestino.NomeProjeto;

            mov.UsuarioAprovacao = Session["login"].ToString().ToUpper();
            mov.DataHoraAprovacao = DateTime.Now;

            hlbapp.SaveChanges();

            Session["msg"] = "Transferência de " + String.Format("{0:C}", mov.Valor) + " do Investimento " + origemStr
                + " para o Investimento " + destinoStr + " no mês " + destino.AnoMes + " realizada com sucesso!";

            #region Enviar E-mail

            Apolo10Entities apolo = new Apolo10Entities();

            USUARIO usuarioObj = apolo.USUARIO.Where(w => w.UsuCod == mov.Usuario).FirstOrDefault();

            string paraNome = usuarioObj.UsuNome;
            string paraEmail = usuarioObj.UsuEmail;
            //string paraEmail = "palves@hyline.com.br";
            string copiaPara = "";
            string assunto = "SOLICITAÇÃO DE TRANSFERÊNCIA DE SALDO ENTRE INVESTIMENTOS APROVADA";
            string stringChar = "" + (char)13 + (char)10;
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "";

            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                + "A solicitação de transferência de saldo de \"" + String.Format("{0:C}", mov.Valor)
                + "\" do investimento \"" + origemStr + "\" para o investimento \"" + destinoStr
                + "\" foi aprovada." + stringChar + stringChar
                + "SISTEMA WEB";

            EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

            #endregion

            Session["ListaInvestimentoMesMovimentacaoSaldo"] = FilterListInvestimentoMesMovimentacaoSaldo();
            Session["metodoRetorno"] = "ListaInvestimentoMesMovimentacaoSaldo";
            return RedirectToAction("OK", "Orcamento");
        }

        public ActionResult DesaprovarTransferenciaSaldo(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Investimento_Mes_Movimentacao_Saldo mov = hlbapp.Investimento_Mes_Movimentacao_Saldo
                .Where(w => w.ID == id).FirstOrDefault();

            Investimento_Mes origem = hlbapp.Investimento_Mes.Where(w => w.ID == mov.IDInvestimentoMesOrigem).FirstOrDefault();
            Investimento invOrigem = hlbapp.Investimento.Where(w => w.ID == origem.IDInvestimento).FirstOrDefault();
            Investimento_Mes destino = hlbapp.Investimento_Mes.Where(w => w.ID == mov.IDInvestimentoMesDestino).FirstOrDefault();
            Investimento invDestino = hlbapp.Investimento.Where(w => w.ID == destino.IDInvestimento).FirstOrDefault();

            if (mov.UsuarioAprovacao != null)
            {
                origem.Saldo = origem.Saldo + mov.Valor;
                destino.Saldo = destino.Saldo - mov.Valor;
            }

            //mov.UsuarioAprovacao = null;
            //mov.DataHoraAprovacao = null;

            hlbapp.Investimento_Mes_Movimentacao_Saldo.DeleteObject(mov);

            hlbapp.SaveChanges();

            Session["msg"] = "Transferência de " + String.Format("{0:C}", mov.Valor) + " do Investimento " + invOrigem.NomeProjeto
                + " para o Investimento " + invDestino.NomeProjeto + " no mês " + destino.AnoMes + " cancelada!";

            Session["ListaInvestimentoMesMovimentacaoSaldo"] = FilterListInvestimentoMesMovimentacaoSaldo();
            Session["metodoRetorno"] = "ListaInvestimentoMesMovimentacaoSaldo";
            return RedirectToAction("OK", "Orcamento");
        }

        public ActionResult OK()
        {
            if (Session["msg"] != null) ViewBag.Mensagem = Session["msg"];

            return View();
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Solicitação de Investimentos

        #region List Methods

        public List<Investimento_Solicitacao> ListSolicitacaoInvestimento(DateTime dataInicio, DateTime dataFim, string nome)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();
            string login = Session["login"].ToString().ToUpper();

            List<Investimento_Solicitacao> retorno = new List<Investimento_Solicitacao>();

            var listaSolicitacaoInvestimentos = hlbapp.Investimento_Solicitacao
                .Where(w => w.DataInicio >= dataInicio && w.DataTermino <= dataFim
                    && (w.NomeProjeto.Contains(nome) || nome == "")).ToList();

            foreach (var item in listaSolicitacaoInvestimentos)
            {
                Investimento inv = hlbapp.Investimento.Where(w => w.ID == item.IDInvestimento).FirstOrDefault();

                USUARIO responsavel = apolo.USUARIO
                    .Where(w => apolo.FUNCIONARIO.Any(a => a.FuncCod == inv.Responsavel
                            && w.UsuCod == a.UsuCod)
                        && w.UsuCod == login)
                    .FirstOrDefault();

                List<USUARIO> gerentesResponsavelInvestimento01 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                            && g.FuncCod == r.FuncCod
                            && g.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                List<USUARIO> gerentesResponsavelInvestimento02 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == r.FuncCod
                            && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                                && g.FuncCod == g2.GrpFuncCod
                                && g.GrpFuncObs == "RDV")
                            && g2.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                List<USUARIO> gerentesResponsavelInvestimento03 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g3 => g3.FuncCod == r.FuncCod
                            && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == g3.GrpFuncCod
                                && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                                    && g.FuncCod == g2.GrpFuncCod
                                    && g.GrpFuncObs == "RDV")
                                && g2.GrpFuncObs == "RDV")
                            && g3.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                if (responsavel != null || gerentesResponsavelInvestimento01.Count > 0
                    || gerentesResponsavelInvestimento02.Count > 0 || gerentesResponsavelInvestimento03.Count > 0)
                {
                    retorno.Add(item);
                }
            }

            //retorno = listaSolicitacaoInvestimentos;

            return retorno.OrderBy(o => o.DataInicio).ThenBy(t => t.NomeProjeto).ToList();
        }

        public List<Investimento_Solicitacao> FilterListSolicitacaoInvestimentos()
        {
            CleanSessions();

            DateTime dataInicio = Convert.ToDateTime(Session["dataInicialSolInv"].ToString());
            DateTime dataFim = Convert.ToDateTime(Session["dataFimSolInv"].ToString());
            string nome = Session["nomeSolInv"].ToString();

            List<Investimento_Solicitacao> listaInvestimento = ListSolicitacaoInvestimento(dataInicio, dataFim, nome);

            return listaInvestimento;
        }

        #endregion

        #region Lista Solicitação de Investimentos

        public ActionResult ListaSolicitacaoInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["msgErroMetodoRetorno"] != null)
            {
                ViewBag.Erro = Session["msgErroMetodoRetorno"].ToString();
                Session["msgErroMetodoRetorno"] = "";
            }
            Session["ListaSolicitacaoInvestimento"] = FilterListSolicitacaoInvestimentos();

            return View();
        }

        public ActionResult SearchSolicitacaoInvestimento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (model["dataInicialSolInv"] != null)
            {
                #region Carrega Valores

                DateTime dataInicio = new DateTime();
                if (model["dataInicialSolInv"] != null)
                {
                    dataInicio = Convert.ToDateTime(model["dataInicialSolInv"]);
                    Session["dataInicialSolInv"] = dataInicio.ToShortDateString();
                }
                else
                    dataInicio = Convert.ToDateTime(Session["dataInicioSolInv"].ToString());

                DateTime dataFim = new DateTime();
                if (model["dataFimSolInv"] != null)
                {
                    dataFim = Convert.ToDateTime(model["dataFimSolInv"]);
                    Session["dataFimSolInv"] = dataFim.ToShortDateString();
                }
                else
                    dataFim = Convert.ToDateTime(Session["dataFimSolInv"].ToString());

                string nome = "";
                if (model["nome"] != null)
                {
                    nome = model["nome"];
                    Session["nomeSolInv"] = nome;
                }
                else
                    nome = Session["nomeSolInv"].ToString();

                #endregion

                Session["ListaSolicitacaoInvestimento"] = ListSolicitacaoInvestimento(dataInicio, dataFim, nome);
            }

            return View("ListaSolicitacaoInvestimento");
        }

        #endregion

        #region CRUD Methods - Investimento_Solicitacao

        public void CarregaSolicitacaoInvestimento(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == id).FirstOrDefault();
            Session["Status"] = solInv.Status;

            #region Verifica se o usuário é gerente ou responsável do investimento para realizar alterações

            string usuarioLogado = Session["login"].ToString().ToUpper();
            Session["permissaoSolicitacao"] = false;
            Investimento inv = hlbapp.Investimento.Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();
            
            USUARIO responsavelInvestimentoApolo = apolo.USUARIO
                .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                    && r.FuncCod == inv.Responsavel)
                    && u.UsuCod == usuarioLogado)
                .FirstOrDefault();
            if (responsavelInvestimentoApolo != null)
                Session["permissaoSolicitacao"] = true;

            List<USUARIO> gerentesResponsavelInvestimento = apolo.USUARIO
                .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                    && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
                        && g.FuncCod == r.FuncCod
                        && g.GrpFuncObs == "RDV"))
                    && u.UsuCod == usuarioLogado)
                .ToList();

            if (gerentesResponsavelInvestimento.Count > 0)
                Session["permissaoSolicitacao"] = true;

            #endregion

            // Cabeçalho do Projeto
            AtualizaDDL(solInv.IDInvestimento.ToString(), (List<SelectListItem>)Session["ListaInvestimentos"]);

            #region Atualiza data mínima e máxima do projeto e do início do funcionamento

            Session["dataMinProj"] = inv.AnoMesInicial.ToString().Substring(0, 4) + '-' + inv.AnoMesInicial.ToString().Substring(4, 2) + "-01";
            Session["dataMaxProj"] = inv.AnoMesFinal.ToString().Substring(0, 4) + '-' + inv.AnoMesFinal.ToString().Substring(4, 2) + "-30";
            Session["dataInicioMinProj"] = solInv.DataInicio.ToString("yyyy-MM-dd");

            #endregion

            Session["saldoValorInvestimento"] = CarregaMsgSaldoInvestimento(solInv.IDInvestimento, DateTime.Today);
            Session["hdSaldoInvSession"] = CarregaValorSaldoInvestimento(solInv.IDInvestimento, DateTime.Today);
            Session["nomeProjetoInv"] = solInv.NomeProjeto;
            AtualizaDDL(solInv.Motivo, (List<SelectListItem>)Session["ListaMotivoSolInv"]);
            AtualizaDDL(solInv.TipoProjeto, (List<SelectListItem>)Session["ListaTiposProjetoSolInv"]);
            if (Request.Browser.IsMobileDevice)
                Session["valorTotalProjetoSolInv"] = solInv.ValorProjeto;
            else
                Session["valorTotalProjetoSolInv"] = String.Format("{0:N2}", solInv.ValorProjeto);
            Session["dataInicioProjetoSolInv"] = solInv.DataInicio;
            Session["dataTerminoProjetoSolInv"] = solInv.DataTermino;
            Session["inicioFuncionamentoProjetoSolInv"] = solInv.InicioFuncionamento;

            // Detalhes do Projeto
            Session["motivoInicioSolInv"] = solInv.MotivoInicio;
            Session["descricaoSolInv"] = solInv.Descricao;
            Session["justificativaSolInv"] = solInv.Justificativa;
            Session["alternativasSolInv"] = solInv.Alternativas;
            Session["riscosFatoresSolInv"] = solInv.RiscosFatores;

            // Detalhes do Custo do Projeto
            Session["ListaInvestimentoSolicitacaoItem"] = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == id).ToList();
            Session["ListaInvestimentoSolicitacaoItemCotacao"] = hlbapp.Investimento_Solicitacao_Item_Cotacao
                .Where(w => hlbapp.Investimento_Solicitacao_Item.Any(a => a.ID == w.IDInvestimentoSolicitacaoItem
                    && a.IDInvestimentoSolicitacao == id)).ToList();            

            // Projeção de Pagamentos
            Session["ListaInvestimentoSolicitacaoProjecaoPagamento"] = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                .Where(w => w.IDInvestimentoSolicitacao == id).ToList();
            CarregaSessionProjPagValorMes((List<Investimento_Solicitacao_Projecao_Pagamento>)Session["ListaInvestimentoSolicitacaoProjecaoPagamento"], solInv.IDInvestimento);
        }

        public void CarregaSessionProjPagValorMes(List<Investimento_Solicitacao_Projecao_Pagamento> listInvMes, int idSol)
        {
            DateTime dataInicial = new DateTime(2018, 7, 1);
            DateTime dataFinal = new DateTime(2019, 6, 1);

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Investimento inv = hlbapp.Investimento.Where(w => w.ID == idSol).FirstOrDefault();

            if (inv != null)
            {
                int anoInicial = Convert.ToInt32(inv.AnoMesInicial.ToString().Substring(0, 4));
                int anoFinal = Convert.ToInt32(inv.AnoMesFinal.ToString().Substring(0, 4));

                dataInicial = new DateTime(anoInicial, 7, 1);
                dataFinal = new DateTime(anoFinal, 6, 1);
            }

            while (dataInicial <= dataFinal)
            {
                decimal valor = 0;
                string origemPagamento = "";
                int anoMes = Convert.ToInt32(dataInicial.ToString("yyyyMM"));

                Investimento_Solicitacao_Projecao_Pagamento invMes = listInvMes.Where(w => w.AnoMes == anoMes).FirstOrDefault();
                if (invMes != null)
                {
                    valor = invMes.Valor;
                    origemPagamento = invMes.OrigemPagamento;
                }

                if (Request.Browser.IsMobileDevice)
                    Session["valorMesInv_" + dataInicial.ToString("MMM")] = valor;
                else
                    Session["valorMesInv_" + dataInicial.ToString("MMM")] = String.Format("{0:N2}", valor);

                Session["ListaOrigemPagamento_" + dataInicial.ToString("MMM")] = CarregaListaOrigemPagamento();
                AtualizaDDL(origemPagamento, (List<SelectListItem>)Session["ListaOrigemPagamento_" + dataInicial.ToString("MMM")]);

                dataInicial = dataInicial.AddMonths(1);
            }
        }

        public ActionResult CreateSolicitacaoInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["OperacaoSolInv"] = "Inclusão";

            CleanSessions();

            return View("SolicitacaoInvestimento");
        }

        public ActionResult EditSolicitacaoInvestimento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["OperacaoSolInv"] = "Alteração";

            Session["idSelecionado"] = id;

            CarregaSolicitacaoInvestimento(id);

            return View("SolicitacaoInvestimento");
        }

        public ActionResult SaveSolicitacaoInvestimento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["Investimento"] != null)
            {
                #region Carrega Valores

                #region Cabeçalho do Projeto

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Investimento

                string investimento = "";
                if (model["Investimento"] != null) investimento = model["Investimento"];

                #endregion

                #region Nome do Projeto

                int idInvestimento = Convert.ToInt32(investimento);
                string nomeProjeto = hlbapp.Investimento
                    .Where(w => w.ID == idInvestimento).FirstOrDefault().NumeroProjeto
                    + " - " + hlbapp.Investimento
                    .Where(w => w.ID == idInvestimento).FirstOrDefault().NomeProjeto;

                #endregion

                #region Motivo

                string motivo = "";
                if (model["Motivo"] != null) motivo = model["Motivo"];

                #endregion

                #region Tipo de Projeto

                string tipoProjeto = "";
                if (model["TipoProjeto"] != null) tipoProjeto = model["TipoProjeto"];

                #endregion

                #region Valor Projeto

                decimal valorProjeto = 0;
                if (model["valorProjeto"] != null)
                {
                    if (Request.Browser.IsMobileDevice)
                        valorProjeto = Convert.ToDecimal(model["valorProjeto"].ToString().Replace(".", ","));
                    else
                        valorProjeto = Convert.ToDecimal(model["valorProjeto"]);
                }

                #endregion

                #region Data Inicio

                DateTime dataInicioProjeto = DateTime.Today;
                if (model["dataInicioProjeto"] != null) dataInicioProjeto = Convert.ToDateTime(model["dataInicioProjeto"]);

                #endregion

                #region Data Término

                DateTime dataTerminoProjeto = DateTime.Today;
                if (model["dataTerminoProjeto"] != null) dataTerminoProjeto = Convert.ToDateTime(model["dataTerminoProjeto"]);

                #endregion

                #region Início Funcionamento

                DateTime inicioFuncionamentoProjeto = DateTime.Today;
                if (model["inicioFuncionamentoProjeto"] != null) inicioFuncionamentoProjeto = Convert.ToDateTime(model["inicioFuncionamentoProjeto"]);

                #endregion

                #endregion

                #region Detalhes do Projeto

                #region Motivo Início

                string motivoInicio = "";
                if (model["motivoInicio"] != null) motivoInicio = model["motivoInicio"];

                #endregion

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null) descricao = model["descricao"];

                #endregion

                #region Justificativa

                string justificativa = "";
                if (model["justificativa"] != null) justificativa = model["justificativa"];

                #endregion

                #region Alternativas

                string alternativas = "";
                if (model["alternativas"] != null) alternativas = model["alternativas"];

                #endregion

                #region Riscos e Fatores

                string riscosFatores = "";
                if (model["riscosFatores"] != null) riscosFatores = model["riscosFatores"];

                #endregion

                #endregion

                #region Disponibilizar Solicitação de Investimento para o depto. de compras

                string disponibilizaCompras = "";
                if (model["DisponibilizaCompras"] != null) disponibilizaCompras = model["DisponibilizaCompras"];

                #endregion

                #endregion

                #region Solicitação de Investimento no WEB

                #region Insere Solicitação de Investimento no WEB

                string operacao = "Inclusão";

                Investimento_Solicitacao solInv = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    solInv = new Investimento_Solicitacao();
                }
                else
                {
                    operacao = "Alteração";
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                solInv.IDInvestimento = Convert.ToInt32(investimento);
                solInv.NomeProjeto = nomeProjeto;
                solInv.ValorProjeto = valorProjeto;
                solInv.DataInicio = dataInicioProjeto;
                solInv.DataTermino = dataTerminoProjeto;
                solInv.Motivo = motivo;
                solInv.TipoProjeto = tipoProjeto;
                solInv.InicioFuncionamento = inicioFuncionamentoProjeto;
                solInv.MotivoInicio = motivoInicio;
                solInv.Descricao = descricao;
                solInv.Justificativa = justificativa;
                solInv.Alternativas = alternativas;
                solInv.RiscosFatores = riscosFatores;
                if (disponibilizaCompras == "Sim")
                    solInv.Status = "Em Cotação";
                else
                    solInv.Status = Session["Status"].ToString();

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Investimento_Solicitacao.AddObject(solInv);

                hlbapp.SaveChanges();

                #endregion

                #region Carrega Lista Filhos da Solicitacao de Investimento

                List<Investimento_Solicitacao_Item> listaItems = (List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItem"];
                List<Investimento_Solicitacao_Item> listaItemsDelete = (List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItemDelete"];
                List<Investimento_Solicitacao_Item_Cotacao> listaCotacaoItens = (List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacao"];
                List<Investimento_Solicitacao_Item_Cotacao> listaCotacaoItensDelete = 
                    (List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacaoDelete"];
                List<Investimento_Solicitacao_Projecao_Pagamento> listaProjPag = 
                    (List<Investimento_Solicitacao_Projecao_Pagamento>)Session["ListaInvestimentoSolicitacaoProjecaoPagamento"];

                #endregion

                #region Itens da Solicitação de Investimento

                #region Deleta as cotações dos itens do BD

                foreach (var item in listaCotacaoItensDelete)
                {
                    Investimento_Solicitacao_Item_Cotacao del =
                        hlbapp.Investimento_Solicitacao_Item_Cotacao.Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbapp.Investimento_Solicitacao_Item_Cotacao.DeleteObject(del);
                }

                hlbapp.SaveChanges();

                #endregion

                #region Deleta os itens do BD

                foreach (var item in listaItemsDelete)
                {
                    Investimento_Solicitacao_Item del =
                        hlbapp.Investimento_Solicitacao_Item.Where(w => w.ID == item.ID).FirstOrDefault();

                    ExcluiItemRequisicaoCompra(del.IDInvestimentoSolicitacao, del.Sequencia, del.CodigoProdutoApolo);

                    hlbapp.Investimento_Solicitacao_Item.DeleteObject(del);
                }

                hlbapp.SaveChanges();

                #endregion

                foreach (var item in listaItems)
                {
                    Investimento_Solicitacao_Item itemSol = new Investimento_Solicitacao_Item();
                    if (item.ID != 0)
                        itemSol = hlbapp.Investimento_Solicitacao_Item.Where(w => w.ID == item.ID).FirstOrDefault();
                    else
                        itemSol.IDInvestimentoSolicitacao = solInv.ID;

                    itemSol.Sequencia = item.Sequencia;
                    itemSol.Categoria = item.Categoria;
                    itemSol.CodigoProdutoApolo = item.CodigoProdutoApolo;
                    itemSol.Descricao = item.Descricao;
                    itemSol.Qtde = item.Qtde;
                    itemSol.IDCotacaoEscolhida = item.IDCotacaoEscolhida;
                    itemSol.EmpresaPedidoCompraApolo = item.EmpresaPedidoCompraApolo;
                    itemSol.NumeroPedidoCompraApolo = item.NumeroPedidoCompraApolo;
                    itemSol.RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao = item.RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao;
                    itemSol.RazaoNaoTer03Cotacoes = item.RazaoNaoTer03Cotacoes;

                    if (itemSol.ID == 0) hlbapp.Investimento_Solicitacao_Item.AddObject(itemSol);

                    hlbapp.SaveChanges();

                    #region Cotações do Item da Solicitação de Investimento

                    var listaCotacaoItem = listaCotacaoItens
                        .Where(w => w.SequenciaItem == item.Sequencia).ToList();

                    foreach (var cotacao in listaCotacaoItem)
                    {
                        Investimento_Solicitacao_Item_Cotacao itemCot = new Investimento_Solicitacao_Item_Cotacao();
                        if (cotacao.ID != 0)
                            itemCot = hlbapp.Investimento_Solicitacao_Item_Cotacao.Where(w => w.ID == cotacao.ID).FirstOrDefault();
                        else
                            itemCot.IDInvestimentoSolicitacaoItem = itemSol.ID;

                        itemCot.SequenciaItem = cotacao.SequenciaItem;
                        itemCot.Sequencia = cotacao.Sequencia;
                        itemCot.FornecedorCodigo = cotacao.FornecedorCodigo;
                        itemCot.FornecedorDescricao = cotacao.FornecedorDescricao;
                        itemCot.Valor = cotacao.Valor;

                        if (itemCot.ID == 0) hlbapp.Investimento_Solicitacao_Item_Cotacao.AddObject(itemCot);
                    }

                    hlbapp.SaveChanges();

                    #endregion
                }

                #endregion

                #region Projeção de Pagamentos da Solicitação de Investimento

                //foreach (var item in listaProjPag)
                //{
                //    Investimento_Solicitacao_Projecao_Pagamento itemProjPag = new Investimento_Solicitacao_Projecao_Pagamento();
                //    if (item.ID != 0)
                //        itemProjPag = hlbapp.Investimento_Solicitacao_Projecao_Pagamento.Where(w => w.ID == item.ID).FirstOrDefault();
                //    else
                //        itemProjPag.IDInvestimentoSolicitacao = solInv.ID;

                //    itemProjPag.AnoMes = item.AnoMes;
                //    itemProjPag.Valor = item.Valor;

                //    if (itemProjPag.ID == 0) hlbapp.Investimento_Solicitacao_Projecao_Pagamento.AddObject(itemProjPag);
                //}

                #region Insere Valores por Mês no WEB

                #region Deleta os valores antigos para inserirem os novos

                List<Investimento_Solicitacao_Projecao_Pagamento> listaProjPagDel = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                    .Where(w => w.IDInvestimentoSolicitacao == solInv.ID).ToList();

                foreach (var item in listaProjPagDel)
                {
                    Investimento_Solicitacao_Projecao_Pagamento delProjPag = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbapp.Investimento_Solicitacao_Projecao_Pagamento.DeleteObject(delProjPag);
                }

                hlbapp.SaveChanges();

                #endregion

                Investimento inv = hlbapp.Investimento.Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();
                int anoInicial = Convert.ToInt32(inv.AnoMesInicial.ToString().Substring(0, 4));
                int anoFinal = Convert.ToInt32(inv.AnoMesFinal.ToString().Substring(0, 4));

                DateTime dataInicial = new DateTime(anoInicial, 7, 1);
                DateTime dataFinal = new DateTime(anoFinal, 6, 1);

                while (dataInicial <= dataFinal)
                {
                    if (model["valorMes_" + dataInicial.ToString("MMM")] != null)
                    {
                        #region Carrega Valores

                        decimal valorMes = 0;
                        if (Request.Browser.IsMobileDevice)
                            valorMes = Convert.ToDecimal(model["valorMes_" + dataInicial.ToString("MMM")].ToString().Replace(".", ","));
                        else
                            valorMes = Convert.ToDecimal(model["valorMes_" + dataInicial.ToString("MMM")]);

                        string origemPagamento = model["OrigemPagamento_" + dataInicial.ToString("MMM")];

                        int anoMes = Convert.ToInt32(dataInicial.ToString("yyyyMM"));

                        #endregion

                        #region Insere no WEB

                        Investimento_Solicitacao_Projecao_Pagamento invMes = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                            .Where(w => w.IDInvestimentoSolicitacao == solInv.ID && w.AnoMes == anoMes).FirstOrDefault();

                        if (invMes == null)
                        {
                            invMes = new Investimento_Solicitacao_Projecao_Pagamento();
                            invMes.IDInvestimentoSolicitacao = solInv.ID;
                        }

                        invMes.AnoMes = anoMes;
                        invMes.Valor = valorMes;
                        if (valorMes > 0)
                            invMes.OrigemPagamento = origemPagamento;
                        else
                            invMes.OrigemPagamento = "";

                        invMes.EmpresaOrigemApolo = "";
                        invMes.NumeroOrigemApolo = "";

                        if (invMes.ID == 0) hlbapp.Investimento_Solicitacao_Projecao_Pagamento.AddObject(invMes);

                        #endregion
                    }

                    dataInicial = dataInicial.AddMonths(1);
                }

                hlbapp.SaveChanges();

                #endregion

                hlbapp.SaveChanges();

                #endregion

                #region Gerar LOG

                LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

                string status = "Pendente";
                LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == solInv.ID)
                    .OrderByDescending(o => o.DataHora).FirstOrDefault();
                if (ultimoLog != null) status = ultimoLog.Status;

                log.IDInvestimentoSolicitacao = solInv.ID;
                log.Operacao = operacao;
                log.Usuario = usuario;
                log.DataHora = DateTime.Now;
                log.Status = status;
                log.Motivo = "";

                hlbapp.LOG_Investimento_Solicitacao.AddObject(log);
                hlbapp.SaveChanges();

                #endregion

                #region Gera Requisição de Compra se todos os itens tiverem o código do produto relacionado

                if (listaItems.Where(w => w.CodigoProdutoApolo == null || w.CodigoProdutoApolo == "").Count() == 0)
                {
                    GeraRequisicaoCompra(solInv.ID);
                }

                #endregion

                #endregion
            }

            Session["ListaSolicitacaoInvestimento"] = FilterListSolicitacaoInvestimentos();
            return View("ListaSolicitacaoInvestimento");
        }

        public ActionResult ConfirmaDeleteSolicitacaoInvestimento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["OperacaoSolInv"] = "Exclusão";

            Session["idSelecionado"] = id;
            return View();
        }

        public ActionResult DeleteSolicitacaoInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == id).FirstOrDefault();

            if (solInv != null)
            {
                ExcluiRequisicaoCompra(solInv.ID);

                #region Deletando a Projeção de Pagamentos

                List<Investimento_Solicitacao_Projecao_Pagamento> listaProjPag = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                    .Where(w => w.IDInvestimentoSolicitacao == id)
                    .ToList();
                foreach (var item in listaProjPag)
                {
                    Investimento_Solicitacao_Projecao_Pagamento del =
                        hlbapp.Investimento_Solicitacao_Projecao_Pagamento.Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbapp.Investimento_Solicitacao_Projecao_Pagamento.DeleteObject(del);
                }

                #endregion

                #region Deletando os Itens

                List<Investimento_Solicitacao_Item> listaItens = hlbapp.Investimento_Solicitacao_Item
                    .Where(w => w.IDInvestimentoSolicitacao == id)
                    .ToList();
                foreach (var item in listaItens)
                {
                    Investimento_Solicitacao_Item del =
                        hlbapp.Investimento_Solicitacao_Item.Where(w => w.ID == item.ID).FirstOrDefault();

                    #region Deletando as Cotações

                    List<Investimento_Solicitacao_Item_Cotacao> listaCotacoes = hlbapp.Investimento_Solicitacao_Item_Cotacao
                        .Where(w => w.IDInvestimentoSolicitacaoItem == del.ID)
                        .ToList();
                    foreach (var cotacao in listaCotacoes)
                    {
                        Investimento_Solicitacao_Item_Cotacao delCotacao =
                            hlbapp.Investimento_Solicitacao_Item_Cotacao.Where(w => w.ID == cotacao.ID).FirstOrDefault();
                        hlbapp.Investimento_Solicitacao_Item_Cotacao.DeleteObject(delCotacao);
                    }

                    hlbapp.SaveChanges();

                    #endregion

                    hlbapp.Investimento_Solicitacao_Item.DeleteObject(del);
                }

                #endregion

                hlbapp.SaveChanges();

                #region Deletando Solicitação de Investimento

                hlbapp.Investimento_Solicitacao.DeleteObject(solInv);
                hlbapp.SaveChanges();

                #endregion

                #region Gerar LOG

                LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

                string status = "Pendente";
                LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == solInv.ID)
                    .OrderByDescending(o => o.DataHora).FirstOrDefault();
                if (ultimoLog != null) status = ultimoLog.Status;

                log.IDInvestimentoSolicitacao = solInv.ID;
                log.Operacao = "Exclusão";
                log.Usuario = Session["login"].ToString().ToUpper();
                log.DataHora = DateTime.Now;
                log.Status = status;
                log.Motivo = "";

                #endregion

                hlbapp.SaveChanges();

                ViewBag.Mensagem = "Solicitação de Investimento " + solInv.NomeProjeto + " - " + solInv.Descricao + " excluída com sucesso!";

                Session["ListaSolicitacaoInvestimento"] = FilterListSolicitacaoInvestimentos();
            }
            return View("ListaSolicitacaoInvestimento");
        }

        #endregion

        #region CRUD Methods - Investimento_Solicitacao_Item

        public void CarregaSolicitacaoInvestimentoItem(int sequencia)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Investimento_Solicitacao_Item item = ((List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItem"])
                .Where(w => w.Sequencia == sequencia).FirstOrDefault();

            AtualizaDDL(item.Categoria, (List<SelectListItem>)Session["ListaCategoriasItemProjeto"]);
            if (item.CodigoProdutoApolo != "" && item.CodigoProdutoApolo != null)
                Session["ListaProdutoApolo"] = CarregaProdutosApolo(item.CodigoProdutoApolo);
            AtualizaDDL(item.CodigoProdutoApolo, (List<SelectListItem>)Session["ListaProdutoApolo"]);
            Session["descricaoInvItem"] = item.Descricao;
            Session["quantidadeInvItem"] = item.Qtde;

            var listaCotacao = ((List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacao"])
                .Where(w => w.SequenciaItem == item.Sequencia).ToList();

            Session["ListaInvestimentoSolicitacaoItemCotacaoExibe"] = listaCotacao;
            
            int cont = 1;
            foreach (var cotacao in listaCotacao)
            {
                string nameFornecedorSession = "ListaFornecedor0" + cont.ToString();
                if (cotacao.FornecedorCodigo != "" && cotacao.FornecedorCodigo != null)
                    Session[nameFornecedorSession] = CarregaFornecedores(cotacao.FornecedorCodigo);
                AtualizaDDL(cotacao.FornecedorCodigo, (List<SelectListItem>)Session[nameFornecedorSession]);
                string nameValorSession = "ValorCotacao0" + cont.ToString();
                if (Request.Browser.IsMobileDevice)
                    Session[nameValorSession] = cotacao.Valor;
                else
                    Session[nameValorSession] = String.Format("{0:N2}", cotacao.Valor);
                cont++;
            }

            Session["razaoNaoTer03CotacoesSolInv"] = item.RazaoNaoTer03Cotacoes;
            Session["razaoExcederOrcamentoOuNaoUtilizarMenorCotacaoSolInv"] = item.RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao;

            AtualizaDDL(item.IDCotacaoEscolhida.ToString(), (List<SelectListItem>)Session["ListaNumeroCotacoes"]);
        }

        public ActionResult ReturnSolicitacaoInvestimento()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            return View("SolicitacaoInvestimento");
        }

        public ActionResult CreateSolicitacaoInvestimentoItem()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessionsSolicitacaoInvestimentoItem();

            return View("SolicitacaoInvestimentoItem");
        }

        public ActionResult EditSolicitacaoInvestimentoItem(int sequencia)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessionsSolicitacaoInvestimentoItem();

            Session["sequenciaItem"] = sequencia;

            CarregaSolicitacaoInvestimentoItem(sequencia);

            return View("SolicitacaoInvestimentoItem");
        }

        public ActionResult SaveSolicitacaoInvestimentoItem(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            Models.bdApolo.bdApoloEntities apolo = new Models.bdApolo.bdApoloEntities();

            #endregion

            if (model["Categoria"] != null)
            {
                #region Carrega Valores

                #region Usuario

                string usuario = Session["login"].ToString().ToUpper();

                #endregion

                #region Categoria

                string categoria = "";
                if (model["Categoria"] != null) categoria = model["Categoria"];

                #endregion

                #region Código do Produto APOLO

                string codigoApolo = "";
                if (model["CodigoApolo"] != null) codigoApolo = model["CodigoApolo"];

                #endregion

                #region Descrição do item

                string descricaoItem = "";
                if (model["descricaoItem"] != null) descricaoItem = model["descricaoItem"];

                #endregion

                #region Quantidade do item

                int quantidadeItem = 0;
                if (model["quantidadeItem"] != null) quantidadeItem = Convert.ToInt32(model["quantidadeItem"]);

                #endregion

                #region Detalhes do Custo do Projeto

                #region Razões para não ter 03 cotações

                string razaoNaoTer03Cotacoes = "";
                if (model["razaoNaoTer03Cotacoes"] != null) razaoNaoTer03Cotacoes = model["razaoNaoTer03Cotacoes"];

                #endregion

                #region Razões exceder orçamento ou não utilizar menor cotação

                string razaoExcederOrcamentoOuNaoUtilizarMenorCotacao = "";
                if (model["razaoExcederOrcamentoOuNaoUtilizarMenorCotacao"] != null)
                    razaoExcederOrcamentoOuNaoUtilizarMenorCotacao = model["razaoExcederOrcamentoOuNaoUtilizarMenorCotacao"];

                #endregion

                #endregion

                #region Cotacao Escolhida

                int cotacaoEscolhida = 0;
                if (model["CotacaoEscolhida"] != null) cotacaoEscolhida = Convert.ToInt32(model["CotacaoEscolhida"]);

                #endregion

                #endregion

                #region Atualiza Dados na Session de Itens da Solicitação

                var listaItens = (List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItem"];

                Investimento_Solicitacao_Item item = null;
                if (Convert.ToInt32(Session["sequenciaItem"]) == 0)
                {
                    item = new Investimento_Solicitacao_Item();
                    if (listaItens.Count > 0)
                        item.Sequencia = listaItens.Max(m => m.Sequencia) + 1;
                    else
                        item.Sequencia = 1;
                }
                else
                {
                    int sequenciaSelecionada = Convert.ToInt32(Session["sequenciaItem"]);
                    item = listaItens.Where(w => w.Sequencia == sequenciaSelecionada).FirstOrDefault();
                }

                item.Categoria = categoria;
                item.Descricao = descricaoItem;
                item.Qtde = quantidadeItem;
                item.CodigoProdutoApolo = codigoApolo;
                item.IDCotacaoEscolhida = cotacaoEscolhida;
                item.EmpresaPedidoCompraApolo = "";
                item.NumeroPedidoCompraApolo = "";
                item.RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao = razaoExcederOrcamentoOuNaoUtilizarMenorCotacao;
                item.RazaoNaoTer03Cotacoes = razaoNaoTer03Cotacoes;

                if (Convert.ToInt32(Session["sequenciaItem"]) == 0) listaItens.Add(item);

                #region Atualiza Dados na Session das Cotações dos Itens

                var listaCotacao = ((List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacao"])
                    .Where(w => w.SequenciaItem == item.Sequencia).ToList();

                if (listaCotacao.Count > 0)
                {
                    int cont = 1;
                    foreach (var cotacao in listaCotacao)
                    {
                        #region Carrega Valores das Cotações

                        #region Fornecedor

                        string nameFornecedorSession = "Fornecedor0" + cont.ToString();
                        string fornecedor = "";
                        if (model[nameFornecedorSession] != null) fornecedor = model[nameFornecedorSession];
                        cotacao.FornecedorCodigo = fornecedor;
                        string descricaoFornecedor = "";
                        Models.bdApolo.ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == fornecedor).FirstOrDefault();
                        if (entidade != null) descricaoFornecedor = entidade.EntNome;
                        cotacao.FornecedorDescricao = descricaoFornecedor;

                        #endregion

                        #region Valor

                        string nameValorSession = "valor0" + cont.ToString();
                        decimal valor = 0;
                        if (model[nameValorSession] != null)
                        {
                            if (Request.Browser.IsMobileDevice)
                                valor = Convert.ToDecimal(model[nameValorSession].ToString().Replace(".", ","));
                            else
                                valor = Convert.ToDecimal(model[nameValorSession]);
                        }
                        cotacao.Valor = valor;

                        #endregion

                        #endregion

                        cont++;
                    }
                }
                else
                {
                    for (int cont = 1; cont <= 3; cont++)
                    {
                        Investimento_Solicitacao_Item_Cotacao cotacao = new Investimento_Solicitacao_Item_Cotacao();

                        cotacao.SequenciaItem = item.Sequencia;
                        cotacao.Sequencia = cont;

                        #region Carrega Valores das Cotações

                        #region Fornecedor

                        string nameFornecedorSession = "Fornecedor0" + cont.ToString();
                        string fornecedor = "";
                        if (model[nameFornecedorSession] != null) fornecedor = model[nameFornecedorSession];
                        cotacao.FornecedorCodigo = fornecedor;
                        string descricaoFornecedor = "";
                        Models.bdApolo.ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == fornecedor).FirstOrDefault();
                        if (entidade != null) descricaoFornecedor = entidade.EntNome;
                        cotacao.FornecedorDescricao = descricaoFornecedor;

                        #endregion

                        #region Valor

                        string nameValorSession = "valor0" + cont.ToString();
                        decimal valor = 0;
                        if (model[nameValorSession] != null)
                        {
                            if (Request.Browser.IsMobileDevice)
                                valor = Convert.ToDecimal(model[nameValorSession].ToString().Replace(".", ","));
                            else
                                valor = Convert.ToDecimal(model[nameValorSession]);
                        }
                        cotacao.Valor = valor;

                        #endregion

                        #endregion

                        ((List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacao"]).Add(cotacao);
                    }
                }

                #endregion

                //Session["ListaInvestimentoSolicitacaoItem"] = listaItens;
                //Session["ListaInvestimentoSolicitacaoItemCotacao"] = listaCotacao;

                #endregion
            }

            return View("SolicitacaoInvestimento");
        }

        public ActionResult ConfirmaDeleteSolicitacaoInvestimentoItem(int sequencia)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["sequenciaItem"] = sequencia;
            return View();
        }

        public ActionResult DeleteSolicitacaoInvestimentoItem()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int sequenciaSelecionada = Convert.ToInt32(Session["sequenciaItem"]);
            var listaItens = (List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItem"];
            var listaItensDeleteBD = (List<Investimento_Solicitacao_Item>)Session["ListaInvestimentoSolicitacaoItemDelete"];
            Investimento_Solicitacao_Item item = listaItens.Where(w => w.Sequencia == sequenciaSelecionada).FirstOrDefault();

            if (item != null)
            {
                #region Deletando as Cotações do Item

                var listaCotacao = (List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacao"];
                var listaCotacaoDeleteBD = (List<Investimento_Solicitacao_Item_Cotacao>)Session["ListaInvestimentoSolicitacaoItemCotacaoDelete"];
                List<Investimento_Solicitacao_Item_Cotacao> listaCotacaoDel = new List<Investimento_Solicitacao_Item_Cotacao>();

                foreach (var cotacao in listaCotacao)
                {
                    if (cotacao.SequenciaItem == sequenciaSelecionada)
                    {
                        listaCotacaoDel.Add(cotacao);
                        if (cotacao.ID != 0)
                            listaCotacaoDeleteBD.Add(cotacao);
                    }
                }

                foreach (var cotacao in listaCotacaoDel)
                {
                    listaCotacao.Remove(cotacao);
                }

                Session["ListaInvestimentoSolicitacaoItemCotacaoDelete"] = listaCotacaoDeleteBD;

                #endregion

                #region Deletando Item da Solicitação de Investimento


                listaItens.Remove(item);
                if (item.ID != 0)
                {
                    listaItensDeleteBD.Add(item);
                    Session["ListaInvestimentoSolicitacaoItemDelete"] = listaItensDeleteBD;
                }

                #endregion

                ViewBag.Mensagem = "Item " + item.Descricao + " da Solicitação de Investimento excluído com sucesso!";
            }

            return View("SolicitacaoInvestimento");
        }

        #endregion

        #region Event Methods

        public ActionResult ConfirmaAcao(int id, string acao, string msg)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["msgConfirmaAcao"] = msg;
            Session["acao"] = acao;
            Session["idSelecionado"] = id;

            return View("ConfirmaAcao");
        }

        public ActionResult EnviarParaAprovacaoGerencia()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int id = Convert.ToInt32(Session["idSelecionado"]);

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Salva na WEB

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == id).FirstOrDefault();
            solInv.Status = "Cotado";

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = solInv.Status;
            log.Motivo = "";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);

            #endregion

            #region Verifica / Atualiza o Saldo

            decimal valorTotalSolInv = 0;

            var listaItens = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == solInv.ID).ToList();

            foreach (var item in listaItens)
            {
                Investimento_Solicitacao_Item_Cotacao cotacao = hlbapp.Investimento_Solicitacao_Item_Cotacao
                    .Where(w => w.IDInvestimentoSolicitacaoItem == item.ID
                        && w.Sequencia == item.IDCotacaoEscolhida).FirstOrDefault();

                valorTotalSolInv = valorTotalSolInv + cotacao.Valor;
            }

            int anoMesSolInv = Convert.ToInt32(solInv.DataInicio.ToString("yyyyMM"));

            Investimento_Mes invMes = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == solInv.IDInvestimento
                    && w.AnoMes == anoMesSolInv).FirstOrDefault();

            if (valorTotalSolInv > invMes.Saldo)
            {
                ViewBag.Erro = "O valor da solicitação ID " + solInv.ID.ToString() + " é maior que o saldo disponível! "
                    + "(Valor Solicitação: " + String.Format("{0:C2}", valorTotalSolInv) + " - "
                    + "Saldo Disponível: " + String.Format("{0:C2}", invMes.Saldo) + ").";
                Session["ListaSolicitacaoInvestimento"] = FilterListSolicitacaoInvestimentos();
                return View("ListaSolicitacaoInvestimento");
            }
            else
            {
                invMes.Saldo = invMes.Saldo - valorTotalSolInv;
                invMes.ValorSolicitado = invMes.ValorSolicitado + valorTotalSolInv;
            }

            #endregion

            hlbapp.SaveChanges();

            #endregion

            #region Enviar E-mail para Gerentes dos Responsáveis (COMENTADO PARA TESTES)

            //string stringChar = "<br />";

            //#region Carrega lista de gerentes do responsável para o envio do e-mail

            //Investimento inv = hlbapp.Investimento.Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();

            //FUNCIONARIO departamentoApolo = apolo.FUNCIONARIO
            //    .Where(w => w.FuncCod == inv.Departamento)
            //    .FirstOrDefault();

            //USUARIO responsavelInvestimentoApolo = apolo.USUARIO
            //    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
            //        && r.FuncCod == inv.Responsavel))
            //    .FirstOrDefault();

            //List<USUARIO> gerentesResponsavelInvestimento = apolo.USUARIO
            //    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
            //        && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == inv.Responsavel
            //            && g.FuncCod == r.FuncCod
            //            && g.GrpFuncObs == "RDV")))
            //    .ToList();

            ////string paraNome = gerentesResponsavelInvestimento.FirstOrDefault().UsuNome;
            ////string paraEmail = gerentesResponsavelInvestimento.FirstOrDefault().UsuEmail;
            ////string copiaPara = responsavelInvestimentoApolo.UsuEmail;

            //string paraNome = "Paulo Alves";
            //string paraEmail = "palves@hyline.com.br";
            //string copiaPara = "";

            ////foreach (var item in gerentesResponsavelInvestimento.Where(w => w.UsuNome != paraNome).ToList())
            ////{
            ////    //if (responsaveisUnidade.IndexOf(item) < (responsaveisUnidade.Count - 1))
            ////    //copiaPara = copiaPara + ";";
            ////    copiaPara = copiaPara + ";" + item.UsuEmail;
            ////}

            //#endregion

            //#region Carrega o corpo do E-mail com a Solicitação

            //string solicitacaoInvestimento = CorpoEmailSolInvHtml(solInv.ID);

            //#endregion

            //#region Gera o E-mail

            //string assunto = "INVESTIMENTO - SOLICITAÇÃO DE INVESTIMENTO P/ APROVAÇÃO";
            //string corpoEmail = "";
            //string anexos = "";
            //string empresaApolo = "5";

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            //corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
            //    + "Segue abaixo os dados da solicitação de investimento para aprovação:"
            //    + solicitacaoInvestimento + stringChar + stringChar
            //    + "Clique no link a seguir para poder realizar a aprovação: "
            //    + "http://" + Request.Url.Host + porta + "/Orcamento/AprovaSolicitacaoInvestimento?id=" + solInv.ID.ToString()
            //        + "&origem=gerencia"
            //    + stringChar + stringChar
            //    + "Caso queira reprovar a solicitação de investimento, clique no link a seguir: "
            //    + "http://" + Request.Url.Host + porta + "/Orcamento/ReprovaSolicitacaoInvestimento?id=" + solInv.ID.ToString()
            //        + "&origem=gerencia"
            //    + stringChar + stringChar
            //    + "SISTEMA WEB";

            //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

            //#endregion

            #endregion

            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
            return RedirectToAction("OK", "Orcamento");
        }

        public ActionResult VoltarParaPendente()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int id = Convert.ToInt32(Session["idSelecionado"]);

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Salva na WEB

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == id).FirstOrDefault();
            solInv.Status = "Pendente";

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = solInv.Status;
            log.Motivo = "";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);

            #endregion

            hlbapp.SaveChanges();

            #endregion

            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
            return RedirectToAction("OK", "Orcamento");
        }

        public ActionResult AprovaSolicitacaoInvestimento(int id, string origem)
        {
            #region Verifica Login do Usuário. Caso não esteja logado, redireciona para o login.

            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            #endregion

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Caso esteja, verifica se o usuário tem direito de aprovação.

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"].ToString() != "")
                {
                    Investimento invVerifica = hlbapp.Investimento
                        .Where(w => hlbapp.Investimento_Solicitacao.Any(a => a.IDInvestimento == w.ID
                            && a.ID == id)).FirstOrDefault();

                    if (invVerifica != null)
                    {
                        string usuarioLogado = Session["login"].ToString().ToUpper();

                        USUARIO gerenteResponsavelInvestimento = apolo.USUARIO
                            .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                                && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == invVerifica.Responsavel
                                    && g.FuncCod == r.FuncCod
                                    && g.GrpFuncObs == "RDV"))
                                && u.UsuCod == usuarioLogado)
                            .FirstOrDefault();

                        if ((gerenteResponsavelInvestimento == null && origem == "gerencia")
                            && (origem == "diretoria" && usuarioLogado != "TLOURENCO")
                            && (usuarioLogado != "PALVES"))
                        {
                            Session["msgErroMetodoRetorno"] = "Você não tem direito suficientes para realizar a aprovação! Verifique!";
                            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
                            return RedirectToAction("OK", "Orcamento");
                        }
                        else
                        {
                            Session["urlChamada"] = "";
                        }
                    }
                    else
                    {
                        ViewBag.Erro = "Solicitação de investimento não existe mais no sistema! Provavelmente o usuário já deletou! Verifique com o mesmo!";
                        Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
                        return RedirectToAction("OK", "Orcamento");
                    }
                }
            }

            #endregion

            #region Salva na WEB

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == id).FirstOrDefault();
            if (origem == "gerencia")
                solInv.Status = "Aprovado Gerência";
            else
                solInv.Status = "Aprovado Diretoria";

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = solInv.Status;
            log.Motivo = "";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);

            #endregion

            hlbapp.SaveChanges();

            #endregion

            #region Enviar E-mail para Responsável do Projeto (COMENTADO PARA TESTES)

            //string stringChar = "<br />";

            //#region Carrega os dados do responsável pela solicitação para o envio do e-mail

            //Investimento inv = hlbapp.Investimento.Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();

            //FUNCIONARIO departamentoApolo = apolo.FUNCIONARIO
            //    .Where(w => w.FuncCod == inv.Departamento)
            //    .FirstOrDefault();

            //USUARIO responsavelInvestimentoApolo = apolo.USUARIO
            //    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
            //        && r.FuncCod == inv.Responsavel))
            //    .FirstOrDefault();

            ////string paraNome = responsavelInvestimentoApolo.UsuNome;
            ////string paraEmail = responsavelInvestimentoApolo.UsuEmail;
            ////string copiaPara = "compras@hyline.com.br";

            //string paraNome = "Paulo Alves";
            //string paraEmail = "palves@hyline.com.br";
            //string copiaPara = "";

            //#endregion

            //#region Carrega o corpo do E-mail com a Solicitação

            //string solicitacaoInvestimento = CorpoEmailSolInvHtml(solInv.ID);

            //#endregion

            //#region Gera o E-mail

            //string assunto = "INVESTIMENTO - SOLICITAÇÃO DE INVESTIMENTO " + solInv.Status.ToUpper();
            //string corpoEmail = "";
            //string anexos = "";
            //string empresaApolo = "5";

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            //string msgDiretoria = "";
            //if (origem == "gerencia") 
            //    msgDiretoria = "Por favor, aguarde a aprovação da diretoria para concluir as aprovações da solicitação.";
            //else
            //    msgDiretoria = "Com todas as aprovações, o compras irá dar o prosseguimento da solicitação!";

            //corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
            //    + "Segue abaixo os dados da solicitação de investimento " + solInv.Status.ToUpper() + ":"
            //    + solicitacaoInvestimento + stringChar + stringChar
            //    + msgDiretoria
            //    + stringChar + stringChar
            //    + "SISTEMA WEB";

            //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

            //#endregion

            #endregion

            if (origem == "gerencia")
            {
                #region Enviar E-mail para Diretoria realizar Aprovação (COMENTADO PARA TESTES)

                //#region Carrega os dados do diretor para o envio do e-mail

                ////paraNome = "Tiago Lourenço";
                ////paraEmail = "tlourenco@hyline.com.br";
                ////copiaPara = "compras@hyline.com.br";

                //paraNome = "Paulo Alves";
                //paraEmail = "palves@hyline.com.br";
                //copiaPara = "";

                //#endregion

                //#region Gera o E-mail

                //assunto = "INVESTIMENTO - SOLICITAÇÃO DE INVESTIMENTO P/ APROVAÇÃO";
                //corpoEmail = "";
                //anexos = "";
                //empresaApolo = "5";

                //corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                //    + "Segue abaixo os dados da solicitação de investimento para aprovação:"
                //    + solicitacaoInvestimento + stringChar + stringChar
                //    + "Clique no link a seguir para poder realizar a aprovação: "
                //    + "http://" + Request.Url.Host + porta + "/Orcamento/AprovaSolicitacaoInvestimento?id=" + solInv.ID.ToString()
                //        + "&origem=diretoria"
                //    + stringChar + stringChar
                //    + "Caso queira reprovar a solicitação de investimento, clique no link a seguir: "
                //    + "http://" + Request.Url.Host + porta + "/Orcamento/ReprovaSolicitacaoInvestimento?id=" + solInv.ID.ToString()
                //        + "&origem=diretoria"
                //    + stringChar + stringChar
                //    + "SISTEMA WEB";

                //EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

                //#endregion

                #endregion
            }

            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
            return RedirectToAction("OK", "Orcamento");
        }

        public ActionResult ReprovaSolicitacaoInvestimento(int id, string origem)
        {
            #region Verifica Login do Usuário. Caso não esteja logado, redireciona para o login.

            if (VerificaSessao())
            {
                Session["urlChamada"] = Request.Url;
                return RedirectToAction("Login", "AccountMobile");
            }

            #endregion

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Caso esteja, verifica se o usuário tem direito de aprovação.

            if (Session["urlChamada"] != null)
            {
                if (Session["urlChamada"].ToString() != "")
                {
                    Investimento invVerifica = hlbapp.Investimento
                        .Where(w => hlbapp.Investimento_Solicitacao.Any(a => a.IDInvestimento == w.ID
                            && a.ID == id)).FirstOrDefault();

                    if (invVerifica != null)
                    {
                        string usuarioLogado = Session["login"].ToString().ToUpper();

                        USUARIO gerenteResponsavelInvestimento = apolo.USUARIO
                            .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                                && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == invVerifica.Responsavel
                                    && g.FuncCod == r.FuncCod
                                    && g.GrpFuncObs == "RDV"))
                                && u.UsuCod == usuarioLogado)
                            .FirstOrDefault();

                        if ((gerenteResponsavelInvestimento == null && origem == "gerencia")
                            && (origem == "diretoria" && usuarioLogado != "TLOURENCO")
                            && (usuarioLogado != "PALVES"))
                        {
                            Session["msgErroMetodoRetorno"] = "Você não tem direito suficientes para realizar a reprovação! Verifique!";
                            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
                            return RedirectToAction("OK", "Orcamento");
                        }
                        else
                        {
                            Session["urlChamada"] = "";
                        }
                    }
                    else
                    {
                        ViewBag.Erro = "Solicitação de investimento não existe mais no sistema! Provavelmente o usuário já deletou! Verifique com o mesmo!";
                        Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
                        return RedirectToAction("OK", "Orcamento");
                    }
                }
            }

            #endregion

            CleanSessions();

            Session["idSelecionado"] = id;
            Session["origemEventoSolInv"] = origem;
            Session["OperacaoSolInv"] = "Reprovação";

            CarregaSolicitacaoInvestimento(id);

            return View("SolicitacaoInvestimento");
        }

        public ActionResult ConfirmaReprovaSolicitacaoInvestimento(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            #region Motivo

            string motivo = "";
            if (model["motivo"] != null) motivo = model["motivo"];

            #endregion

            int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
            string origem = Session["origemEventoSolInv"].ToString();

            #endregion

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Salva na WEB

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao.Where(w => w.ID == idSelecionado).FirstOrDefault();
            solInv.Status = "Em Cotação";
            
            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            if (origem == "gerencia")
                log.Status = "Reprovado Gerência";
            else
                log.Status = "Reprovado Diretoria";
            log.Motivo = motivo;

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);

            #endregion

            #region Atualiza o Saldo

            decimal valorTotalSolInv = 0;

            var listaItens = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == solInv.ID).ToList();

            foreach (var item in listaItens)
            {
                Investimento_Solicitacao_Item_Cotacao cotacao = hlbapp.Investimento_Solicitacao_Item_Cotacao
                    .Where(w => w.IDInvestimentoSolicitacaoItem == item.ID
                        && w.Sequencia == item.IDCotacaoEscolhida).FirstOrDefault();

                valorTotalSolInv = valorTotalSolInv + cotacao.Valor;
            }

            int anoMesSolInv = Convert.ToInt32(solInv.DataInicio.ToString("yyyyMM"));

            Investimento_Mes invMes = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == solInv.IDInvestimento
                    && w.AnoMes == anoMesSolInv).FirstOrDefault();

            invMes.Saldo = invMes.Saldo + valorTotalSolInv;
            invMes.ValorSolicitado = invMes.ValorSolicitado - valorTotalSolInv;

            #endregion

            hlbapp.SaveChanges();

            #endregion

            #region Enviar E-mail para Responsável do Projeto

            string stringChar = "<br />";

            #region Carrega os dados do responsável pela solicitação para o envio do e-mail

            Investimento inv = hlbapp.Investimento.Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();

            FUNCIONARIO departamentoApolo = apolo.FUNCIONARIO
                .Where(w => w.FuncCod == inv.Departamento)
                .FirstOrDefault();

            USUARIO responsavelInvestimentoApolo = apolo.USUARIO
                .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                    && r.FuncCod == inv.Responsavel))
                .FirstOrDefault();

            //string paraNome = responsavelInvestimentoApolo.UsuNome;
            //string paraEmail = responsavelInvestimentoApolo.UsuEmail;
            //string copiaPara = "compras@hyline.com.br";

            string paraNome = "Paulo Alves";
            string paraEmail = "palves@hyline.com.br";
            string copiaPara = "";

            #endregion

            #region Carrega o corpo do E-mail com a Solicitação

            string solicitacaoInvestimento = CorpoEmailSolInvHtml(solInv.ID);

            #endregion

            #region Gera o E-mail

            string assunto = "INVESTIMENTO - SOLICITAÇÃO DE INVESTIMENTO - " + log.Status.ToUpper();
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "5";

            string porta = "";
            if (Request.Url.Port != 80)
                porta = ":" + Request.Url.Port.ToString();

            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                + "A solicitação de investimento abaixo foi - " + log.Status.ToUpper() + "." + stringChar + stringChar
                + "Segue motivo: " + motivo.Replace("\r\n", "<br />")
                + solicitacaoInvestimento + stringChar + stringChar
                + "SISTEMA WEB";

            EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

            #endregion

            #endregion

            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
            return RedirectToAction("OK", "Orcamento");
        }

        public string CorpoEmailSolInvHtml(int id)
        {
            #region Carrega o corpo do E-mail com a Solicitação

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao
                .Where(w => w.ID == id).FirstOrDefault();
            string stringChar = "<br />";

            string solicitacaoInvestimento = "";
            var listaProjecaoPag = hlbapp.Investimento_Solicitacao_Projecao_Pagamento
                .Where(w => w.IDInvestimentoSolicitacao == solInv.ID).ToList();

            #region Carrega Solicitação de Investimento

            solicitacaoInvestimento =
                "<table style=\"width: 100%; "
                    + "border-collapse: collapse; "
                    + "text-align: center;\">"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Investimento:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.NomeProjeto
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Valor do projeto:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + String.Format("{0:C}", listaProjecaoPag.Sum(s => s.Valor))
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Data de início do projeto:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.DataInicio.ToShortDateString()
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Data de término do projeto:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.DataTermino.ToShortDateString()
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Motivo do projeto:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.Motivo
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Tipo de projeto:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.TipoProjeto
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Início do funcionamento:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.InicioFuncionamento.ToShortDateString()
                        + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<th style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "text-align: center;\" colspan=\"3\">"
                            + "Descrição:"
                        + "</th>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\" colspan=\"3\">"
                                + solInv.Descricao.Replace("\r\n", "<br />")
                        + "</td>"
                    + "</tr>";

            #endregion

            #region Carrega Itens da Solicitação de Investimento

            var listaItensSolicitacaoInvestimento = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == solInv.ID).ToList();

            #region Cabeçalho dos Itens

            if (listaItensSolicitacaoInvestimento.Count > 0)
                solicitacaoInvestimento = solicitacaoInvestimento
                    + "<tr>"
                        + "<th colspan=\"6\">"
                            + "-"
                        + "</th>"
                    + "</tr>"
                    + "<tr style=\"background: #333; "
                        + "color: white; "
                        + "font-weight: bold; "
                        + "text-align: center;\">"
                        + "<th>"
                            + "Categoria"
                        + "</th>"
                        + "<th>"
                            + "Descrição do item"
                        + "</th>"
                        + "<th>"
                            + "Fornecedor escolhido"
                        + "</th>"
                        + "<th>"
                            + "Valor"
                        + "</th>"
                        + "<th>"
                            + "Razão por não ter 03 cotações"
                        + "</th>"
                        + "<th>"
                            + "Razão por não utilizar menor cotação"
                        + "</th>"
                    + "</tr>";

            #endregion

            #region Detalhamento dos Itens

            decimal valorTotalItens = 0;
            foreach (var item in listaItensSolicitacaoInvestimento)
            {
                Investimento_Solicitacao_Item_Cotacao cotacaoEscolhida = hlbapp.Investimento_Solicitacao_Item_Cotacao
                    .Where(w => w.Sequencia == item.IDCotacaoEscolhida && w.IDInvestimentoSolicitacaoItem == item.ID).FirstOrDefault();

                solicitacaoInvestimento = solicitacaoInvestimento
                    + "<tr>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + item.Categoria
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + item.Descricao.Replace("\r\n", "<br />")
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + cotacaoEscolhida.FornecedorCodigo + " - " + cotacaoEscolhida.FornecedorDescricao
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + String.Format("{0:C}", cotacaoEscolhida.Valor)
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + item.RazaoNaoTer03Cotacoes.Replace("\r\n", "<br />")
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + item.RazaoExcederOrcamentoOuNaoUtilizarMenorCotacao.Replace("\r\n", "<br />")
                        + "</td>"
                    + "</tr>";

                valorTotalItens = valorTotalItens + cotacaoEscolhida.Valor;
            }

            #endregion

            #region Totalizador dos Itens

            solicitacaoInvestimento = solicitacaoInvestimento +
                "<tr style=\"background: #333; "
                    + "color: white; "
                    + "font-weight: bold; "
                    + "text-align: center;\">"
                    + "<th colspan=\"5\">"
                        + "Total dos itens:"
                    + "</th>"
                    + "<th>"
                        + String.Format("{0:C}", valorTotalItens)
                    + "</th>"
                + "</tr>";

            #endregion

            solicitacaoInvestimento = solicitacaoInvestimento + "</table><br />";

            #endregion

            #region Carrega a Projeção de Pagamentos

            solicitacaoInvestimento = stringChar + stringChar + solicitacaoInvestimento
                + "<table style=\"width: 100%; "
                    + "border-collapse: collapse; "
                    + "text-align: center;\">"
                    + "<tr style=\"background: #333; "
                        + "color: white; "
                        + "font-weight: bold; "
                        + "text-align: center;\">"
                        + "<th>"
                            + "Mês/Ano"
                        + "</th>"
                        + "<th>"
                            + "Valor"
                        + "</th>"
                        + "<th>"
                            + "Origem pagamento"
                        + "</th>"
                    + "</tr>";

            foreach (var item in listaProjecaoPag.Where(w => w.Valor > 0).ToList())
            {
                solicitacaoInvestimento = solicitacaoInvestimento
                    + "<tr>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + Convert.ToDateTime("01/" + item.AnoMes.ToString().Substring(4, 2) + "/" + item.AnoMes.ToString().Substring(0, 4)).ToString("MMM/yyyy")
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + String.Format("{0:C}", item.Valor)
                        + "</td>"
                        + "<td style=\"padding: 6px; "
                            + "border: 1px solid #ccc;\">"
                                + item.OrigemPagamento
                        + "</td>"
                    + "</tr>";
            }

            #endregion

            solicitacaoInvestimento = solicitacaoInvestimento + "</table>";

            #endregion

            return solicitacaoInvestimento;
        }

        public ActionResult HistoricoSolicitacaoInvestimento(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["ListaLOGSolicitacaoInvestimento"] = hlbapp.LOG_Investimento_Solicitacao
                .Where(w => w.IDInvestimentoSolicitacao == id).ToList();

            return View();
        }

        #region Event Methods Apolo

        public void GeraRequisicaoCompra(int id)
        {
            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Carrega dados da Solicitação

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao
                .Where(w => w.ID == id).FirstOrDefault();

            Investimento inv = hlbapp.Investimento
                .Where(w => w.ID == solInv.IDInvestimento).FirstOrDefault();

            List<Investimento_Solicitacao_Item> listaItens = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == id).ToList();

            LOG_Investimento_Solicitacao logSolInv = hlbapp.LOG_Investimento_Solicitacao
                .Where(w => w.IDInvestimentoSolicitacao == solInv.ID
                    && w.Operacao == "Inclusão")
                .OrderBy(o => o.DataHora).FirstOrDefault();

            #endregion

            #region Gera Requisição de Compra no Apolo

            #region Carrega Dados do Apolo

            FUNCIONARIO departamento = apolo.FUNCIONARIO
                .Where(w => w.FuncCod == inv.Departamento).FirstOrDefault();

            USUARIO usuarioCriacaoSolInv = apolo.USUARIO
                .Where(w => w.UsuCod == logSolInv.Usuario).FirstOrDefault();

            APROV_COMP_FINALIDADE aprovador = apolo.APROV_COMP_FINALIDADE
                .Where(a => a.AprovCompTipo == "Requisição" && a.FinCompCod == departamento.USERFinCompraDeptoOrcamento)
                .FirstOrDefault();

            #endregion

            #region Insere / Atualiza Requisição de Compra

            REQ_COMP requisicao = apolo.REQ_COMP
                .Where(w => w.EmpCod == solInv.EmpCod && w.ReqCompNum == solInv.ReqCompNum).FirstOrDefault();

            bool novoRC = false;
            if (requisicao == null)
            {
                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
                apolo.gerar_codigo(departamento.EmpCod, "REQ_COMP", numero);
                ObjectParameter numeroConcat = new ObjectParameter("numero", typeof(global::System.String));
                apolo.CONCAT_ZERO_ESQUERDA(numero.Value.ToString(), 7, numeroConcat);
                requisicao = new REQ_COMP();
                requisicao.EmpCod = departamento.EmpCod;
                requisicao.ReqCompEmpCodLocEnt = departamento.EmpCod;
                requisicao.ReqCompNum = numeroConcat.Value.ToString();

                solInv.EmpCod = requisicao.EmpCod;
                solInv.ReqCompNum = requisicao.ReqCompNum;

                novoRC = true;
            }

            requisicao.ReqCompData = solInv.DataInicio;
            requisicao.ReqCompDataNec = solInv.DataInicio;
            
            #region Preenchimento de Campos Obrigatórios

            requisicao.ReqCompStat = "Aberto";
            requisicao.ReqCompCot = "Não";
            requisicao.ReqCompEmpCodDoc = requisicao.EmpCod;
            requisicao.ReqCompEspecDoc = "RC";
            requisicao.ReqCompSerieDoc = "0";
            requisicao.ReqCompNumDoc = requisicao.ReqCompNum;
            requisicao.ReqCompAprov = "Total";
            requisicao.UsuCod = logSolInv.Usuario;
            requisicao.ReqCompPed = "Não";
            requisicao.ReqCompReprov = "Não";
            requisicao.ReqCompRes = "Não";
            requisicao.ReqCompTipoReq = "Nenhuma";
            requisicao.ReqCompControle = "Nenhum";
            requisicao.ReqCompTerc = "Não";
            requisicao.ReqCompValLimite = 0;
            requisicao.USEREnviarAprovacao = "Sim";

            #endregion

            requisicao.FuncCod = inv.Responsavel;
            requisicao.ReqCompDescr = inv.NumeroProjeto + " - " + inv.NomeProjeto;
            requisicao.CCtrlCodEstr = departamento.CCtrlCodEstr;
            requisicao.ReqCompTexto = "Criado via Solicitação de Investimento WEB pelo usuário " 
                + usuarioCriacaoSolInv.UsuNome + " em " + logSolInv.DataHora.ToString("dd/MM/yyyy hh:mm")
                + ": " + solInv.Descricao;
            requisicao.FinCompCod = departamento.USERFinCompraDeptoOrcamento;

            if (novoRC) apolo.REQ_COMP.AddObject(requisicao);

            #endregion

            #region Deleta itens não existentes na Solicitação

            var listaIRC = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == requisicao.EmpCod
                && w.ReqCompNum == requisicao.ReqCompNum).ToList();

            foreach (var item in listaIRC)
            {
                Investimento_Solicitacao_Item itemSI = hlbapp.Investimento_Solicitacao_Item
                    .Where(w => w.IDInvestimentoSolicitacao == solInv.ID
                        && w.CodigoProdutoApolo == item.ProdCodEstr && w.Sequencia == item.ItReqCompSeq)
                    .FirstOrDefault();

                if (itemSI == null)
                {
                    ITEM_REQ_COMP iRCDelete = apolo.ITEM_REQ_COMP
                        .Where(w => w.EmpCod == item.EmpCod && w.ReqCompNum == item.ReqCompNum
                            && w.ItReqCompSeq == item.ItReqCompSeq && w.ProdCodEstr == item.ProdCodEstr)
                        .FirstOrDefault();

                    apolo.ITEM_REQ_COMP.DeleteObject(iRCDelete);
                }
            }

            #endregion

            #region Insere / Altera Itens da Requisição de Compra

            foreach (var item in listaItens)
            {
                ITEM_REQ_COMP iRC = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == requisicao.EmpCod
                        && w.ReqCompNum == requisicao.ReqCompNum && w.ProdCodEstr == item.CodigoProdutoApolo
                        && w.ItReqCompSeq == item.Sequencia)
                    .FirstOrDefault();

                bool novo = false;
                if (iRC == null)
                {
                    iRC = new ITEM_REQ_COMP();
                    iRC.EmpCod = requisicao.EmpCod;
                    iRC.ReqCompNum = requisicao.ReqCompNum;
                    iRC.ProdCodEstr = item.CodigoProdutoApolo;
                    iRC.ItReqCompSeq = (short)item.Sequencia;
                    iRC.ItReqCompSeqCompItOrig = (short)item.Sequencia;
                    novo = true;
                }

                iRC.ItReqCompServ = "Não";

                PROD_UNID_MED unidMed = apolo.PROD_UNID_MED
                    .Where(w => w.ProdCodEstr == iRC.ProdCodEstr
                        && w.ProdUnidMedPos == 1).FirstOrDefault();

                iRC.ItReqCompUnidMedCod = unidMed.ProdUnidMedCod;
                iRC.ItReqCompUnidMedPos = 1;
                iRC.ItReqCompQtd = item.Qtde;
                iRC.ItReqCompQtdCalc = item.Qtde;
                iRC.ItReqCompDataNec = solInv.DataInicio;
                iRC.ItReqCompFinalidade = "Troca";
                iRC.ItReqCompObs = item.Descricao;
                iRC.ItReqCompAprov = "Aprovado";
                iRC.ItReqCompAprovUsuCod = aprovador.UsuCod;
                iRC.ItReqCompAprovData = logSolInv.DataHora;

                #region Preenchimento de Campos Obrigatórios

                iRC.ItReqCompCot = "F";
                iRC.ItReqCompPed = "F";
                iRC.ItReqCompUrgente = "Não";
                iRC.ItReqCompQtdEntregaParc = 0;
                iRC.ItReqCompUtilizProm = "Não";
                iRC.ItReqCompUtilizShortForm = "Não";
                iRC.ItReqCompValShortForm = 0;
                iRC.ItReqCompPrzComercDias = 0;
                iRC.ItReqCompRes = "F";
                iRC.ItReqCompSelec = "Sim";
                iRC.ItReqCompTipoEntrega = "Total";
                iRC.ItReqCompValLimite = 0;
                iRC.ITREQCOMPSTATDISPONIB = "Disponível";
                iRC.ITREQCOMPEMUSOPOR = "RC";

                #endregion

                if (novo) apolo.ITEM_REQ_COMP.AddObject(iRC);
            }

            #endregion

            apolo.SaveChanges();

            #endregion

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            string status = "Pendente";
            LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == solInv.ID)
                .OrderByDescending(o => o.DataHora).FirstOrDefault();
            if (ultimoLog != null) status = ultimoLog.Status;

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = status;
            if (novoRC)
                log.Motivo = "Requisição de Compra " + requisicao.EmpCod + " - " + requisicao.ReqCompNum + " inserida.";
            else
                log.Motivo = "Requisição de Compra " + requisicao.EmpCod + " - " + requisicao.ReqCompNum + " atualizada.";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);
            hlbapp.SaveChanges();

            #endregion
        }

        public void ExcluiRequisicaoCompra(int id)
        {
            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Carrega dados da Solicitação

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao
                .Where(w => w.ID == id).FirstOrDefault();

            #endregion

            #region Exclui Requisição de Compra no Apolo

            REQ_COMP requisicao = apolo.REQ_COMP
                .Where(w => w.EmpCod == solInv.EmpCod && w.ReqCompNum == solInv.ReqCompNum).FirstOrDefault();

            if (requisicao != null)
            {
                apolo.REQ_COMP.DeleteObject(requisicao);

                #region Deleta itens

                var listaIRC = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == requisicao.EmpCod
                    && w.ReqCompNum == requisicao.ReqCompNum).ToList();

                foreach (var item in listaIRC)
                {
                    ITEM_REQ_COMP iRCDelete = apolo.ITEM_REQ_COMP
                        .Where(w => w.EmpCod == item.EmpCod && w.ReqCompNum == item.ReqCompNum
                            && w.ItReqCompSeq == item.ItReqCompSeq && w.ProdCodEstr == item.ProdCodEstr)
                        .FirstOrDefault();
                    
                    apolo.ITEM_REQ_COMP.DeleteObject(iRCDelete);
                }

                #endregion
            }

            apolo.SaveChanges();
            hlbapp.SaveChanges();

            #endregion

            #region Atualiza Dados na Solicitação

            solInv.EmpCod = null;
            solInv.ReqCompNum = null;

            #endregion

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            string status = "Pendente";
            LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == solInv.ID)
                .OrderByDescending(o => o.DataHora).FirstOrDefault();
            if (ultimoLog != null) status = ultimoLog.Status;

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = status;
            log.Motivo = "Requisição de Compra " + requisicao.EmpCod + " - " + requisicao.ReqCompNum + " excluída.";
            
            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);
            hlbapp.SaveChanges();

            #endregion
        }

        public void ExcluiItemRequisicaoCompra(int id, int sequencia, string produto)
        {
            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Carrega dados da Solicitação

            Investimento_Solicitacao solInv = hlbapp.Investimento_Solicitacao
                .Where(w => w.ID == id).FirstOrDefault();

            Investimento_Solicitacao_Item itemSI = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == id
                    && w.Sequencia == sequencia && w.CodigoProdutoApolo == produto).FirstOrDefault();

            #endregion

            #region Exclui Requisição de Compra no Apolo

            REQ_COMP requisicao = apolo.REQ_COMP
                .Where(w => w.EmpCod == solInv.EmpCod && w.ReqCompNum == solInv.ReqCompNum).FirstOrDefault();

            if (requisicao != null)
            {
                #region Deleta item

                var listaIRC = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == requisicao.EmpCod
                    && w.ReqCompNum == requisicao.ReqCompNum
                    && w.ItReqCompSeq == sequencia && w.ProdCodEstr == produto).ToList();

                foreach (var item in listaIRC)
                {
                    ITEM_REQ_COMP iRCDelete = apolo.ITEM_REQ_COMP
                        .Where(w => w.EmpCod == item.EmpCod && w.ReqCompNum == item.ReqCompNum
                            && w.ItReqCompSeq == item.ItReqCompSeq && w.ProdCodEstr == item.ProdCodEstr)
                        .FirstOrDefault();

                    apolo.ITEM_REQ_COMP.DeleteObject(iRCDelete);
                }

                #endregion
            }

            apolo.SaveChanges();
            hlbapp.SaveChanges();

            #endregion

            #region Atualiza Dados na Solicitação

            var listaIRCVerifica = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == requisicao.EmpCod
                && w.ReqCompNum == requisicao.ReqCompNum).ToList();

            if (listaIRCVerifica.Count == 0)
            {
                solInv.EmpCod = null;
                solInv.ReqCompNum = null;
            }

            #endregion

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            string status = "Pendente";
            LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == solInv.ID)
                .OrderByDescending(o => o.DataHora).FirstOrDefault();
            if (ultimoLog != null) status = ultimoLog.Status;

            log.IDInvestimentoSolicitacao = solInv.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = status;
            log.Motivo = "Item " + itemSI.Descricao + " da Requisição de Compra " + requisicao.EmpCod + " - " + requisicao.ReqCompNum + " excluído.";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);
            hlbapp.SaveChanges();

            #endregion
        }

        public ActionResult GeraPedidoCompra(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;
            Session["ListaCondPagApolo"] = new List<SelectListItem>();
            
            return View();
        }

        public ActionResult ConfirmaGeraPedidoCompra(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            #region Carrega Valores

            #region Login

            string login = Session["login"].ToString().ToUpper();

            #endregion

            #region Código do Fornecedor

            string codigoFornecedor = "";
            if (model["codigoFornecedor"] != null) codigoFornecedor = model["codigoFornecedor"];

            #endregion

            #region Condição de Pagamento

            string condicaoPagamento = "";
            if (model["condicaoPagamento"] != null) condicaoPagamento = model["condicaoPagamento"];

            #endregion

            int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);

            #endregion

            #region Carrega dados da Solicitação

            var listaItens = hlbapp.Investimento_Solicitacao_Item_Cotacao
                .Where(w => hlbapp.Investimento_Solicitacao_Item
                    .Any(a => a.ID == w.IDInvestimentoSolicitacaoItem && a.IDInvestimentoSolicitacao == idSelecionado
                        && a.IDCotacaoEscolhida == w.Sequencia)
                    && w.FornecedorCodigo == codigoFornecedor)
                .OrderBy(o => o.SequenciaItem)
                .ToList();

            Investimento_Solicitacao_Item_Cotacao iSIC = listaItens.FirstOrDefault();

            Investimento_Solicitacao_Item iSI = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.ID == iSIC.IDInvestimentoSolicitacaoItem).FirstOrDefault();

            Investimento_Solicitacao sI = hlbapp.Investimento_Solicitacao
                .Where(w => w.ID == idSelecionado).FirstOrDefault();

            #endregion

            #region Gera Pedido de Compra no Apolo

            #region Se existir, deleta o pedido

            PED_COMP pc = apolo.PED_COMP
                .Where(w => w.EmpCod == iSI.EmpresaPedidoCompraApolo && w.PedCompNum == iSI.NumeroPedidoCompraApolo)
                .FirstOrDefault();

            if (pc != null)
            {
                apolo.delete_pedcomp(pc.EmpCod, pc.PedCompNum, pc.EntCod);
            }

            #endregion

            #region Após deletar, insere novamente

            REQ_COMP rC = apolo.REQ_COMP.Where(w => w.EmpCod == sI.EmpCod && w.ReqCompNum == sI.ReqCompNum).FirstOrDefault();

            if (rC == null)
            {
                ViewBag.Erro = "A requisição de compra " + sI.EmpCod + " - " + sI.ReqCompNum + " não existe mais no Apolo!"
                    + " Sendo assim, não é possível gerar o pedido! Deve ser salva a solicitação novamente para gerar nova requisição de compra!";
                return View("GeraPedidoCompra");
            }

            ObjectParameter pedcompragerado = new ObjectParameter("pedcompragerado", typeof(global::System.String));
            apolo.requisicao_gera_pedcomp(rC.EmpCod, rC.ReqCompNum, iSIC.FornecedorCodigo, "Automático", login, "Total", "0000001", pedcompragerado, sI.EmpCod);
            string numeroPedidoCompraGerado = pedcompragerado.Value.ToString();

            #endregion

            #region Insere os itens no pedido gerado

            foreach (var item in listaItens)
            {
                Investimento_Solicitacao_Item iSII = hlbapp.Investimento_Solicitacao_Item
                    .Where(w => w.ID == item.IDInvestimentoSolicitacaoItem).FirstOrDefault();

                iSII.EmpresaPedidoCompraApolo = rC.EmpCod;
                iSII.NumeroPedidoCompraApolo = numeroPedidoCompraGerado;

                ITEM_REQ_COMP iRC = apolo.ITEM_REQ_COMP.Where(w => w.EmpCod == rC.EmpCod
                    && w.ReqCompNum == rC.ReqCompNum && w.ProdCodEstr == iSII.CodigoProdutoApolo
                    && w.ItReqCompSeq == iSII.Sequencia).FirstOrDefault();

                iRC.ItReqCompPed = "T";

                apolo.requisicao_gera_itempedcomp(iRC.EmpCod, iRC.ReqCompNum, iSII.NumeroPedidoCompraApolo, iRC.ProdCodEstr, iRC.ItReqCompSeq,
                    "", 0, sI.EmpCod);
            }

            apolo.SaveChanges();

            #endregion

            #region Atualiza o status da requisição

            apolo.atualiza_requisicao_pedcomp(sI.EmpCod, sI.ReqCompNum, "Insercao/Alteracao");

            #endregion

            #region Insere as condições de pagamento

            COND_PAG cp = apolo.COND_PAG.Where(w => w.CondPagCod == condicaoPagamento).FirstOrDefault();

            COND_PAG_PED_COMP condPagPC = new COND_PAG_PED_COMP();
            condPagPC.EmpCod = rC.EmpCod;
            condPagPC.PedCompNum = numeroPedidoCompraGerado;
            condPagPC.CondPagCod = condicaoPagamento;
            condPagPC.CondPagPedCompNome = cp.CondPagNome;

            apolo.COND_PAG_PED_COMP.AddObject(condPagPC);

            apolo.SaveChanges();

            apolo.parcela_ped_comp(rC.EmpCod, numeroPedidoCompraGerado);

            #endregion

            #region Atualiza dados gerais do pedido

            PED_COMP pC = apolo.PED_COMP.Where(w => w.EmpCod == rC.EmpCod
                && w.PedCompNum == numeroPedidoCompraGerado).FirstOrDefault();

            pC.USERLiberaAprovacao = "Sim";
            pC.USERPedidoCotado = "Sim";

            apolo.SaveChanges();

            #endregion

            #region Aprova os itens

            LOG_Investimento_Solicitacao logAprovacao = hlbapp.LOG_Investimento_Solicitacao
                .Where(w => w.IDInvestimentoSolicitacao == sI.ID && w.Status == "Aprovado Gerência")
                .OrderByDescending(o => o.DataHora)
                .FirstOrDefault();

            var listaItensPC = apolo.ITEM_PED_COMP
                .Where(w => w.EmpCod == rC.EmpCod &&
                    w.PedCompNum == numeroPedidoCompraGerado).ToList();

            foreach (var item in listaItensPC)
            {
                item.ItPedCompAprovData = logAprovacao.DataHora;
                item.ItPedCompAprovUsuCod = logAprovacao.Usuario;
                item.ItPedCompBloq = "Não";
            }

            apolo.SaveChanges();

            #endregion

            #region Atualiza o status do pedido de compra

            apolo.atualiza_status_pedcomp(rC.EmpCod, numeroPedidoCompraGerado);

            #endregion

            hlbapp.SaveChanges();

            #endregion

            #region Gerar LOG

            LOG_Investimento_Solicitacao log = new LOG_Investimento_Solicitacao();

            string status = "Pendente";
            LOG_Investimento_Solicitacao ultimoLog = hlbapp.LOG_Investimento_Solicitacao.Where(w => w.IDInvestimentoSolicitacao == sI.ID)
                .OrderByDescending(o => o.DataHora).FirstOrDefault();
            if (ultimoLog != null) status = ultimoLog.Status;

            var listaItensVerifica = hlbapp.Investimento_Solicitacao_Item
                .Where(w => w.IDInvestimentoSolicitacao == sI.ID
                    && w.NumeroPedidoCompraApolo == "").ToList();

            if (listaItensVerifica.Count > 0)
                status = "Pedido Parcial";
            else
                status = "Pedido Total";

            log.IDInvestimentoSolicitacao = sI.ID;
            log.Operacao = "Alteração";
            log.Usuario = Session["login"].ToString().ToUpper();
            log.DataHora = DateTime.Now;
            log.Status = status;
            log.Motivo = "Pedido de Compra " + rC.EmpCod + " - " + numeroPedidoCompraGerado + " gerado.";

            hlbapp.LOG_Investimento_Solicitacao.AddObject(log);

            sI.Status = status;

            hlbapp.SaveChanges();

            #endregion

            Session["metodoRetorno"] = "ListaSolicitacaoInvestimento";
            return RedirectToAction("OK", "Orcamento");
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region Geral

            Session["idSelecionado"] = 0;
            if (Session["FiltroListaDepartamentosInv"] == null) Session["FiltroListaDepartamentosInv"] = CarregaListaDepartamento(true);
            if (Session["FiltroListaAnoFiscalInv"] == null) Session["FiltroListaAnoFiscalInv"] = CarregaListaAnoFiscal(false, false);

            #endregion

            #region Ano Fiscal

            Session["anoInicialConf"] = DateTime.Today.Year;
            Session["anoFinalConf"] = DateTime.Today.AddYears(1).Year;

            #endregion

            #region Investimento

            Session["numProjetoInv"] = "";
            Session["nomeProjetoInv"] = "";
            Session["valorInv"] = 0;

            SelectListItem anoFiscalSI = ((List<SelectListItem>)Session["FiltroListaAnoFiscalInv"])
                .Where(w => w.Selected == true).FirstOrDefault();
            int anoInicial = Convert.ToInt32(anoFiscalSI.Text.Substring(0, 4));
            int anoFinal = Convert.ToInt32(anoFiscalSI.Text.Substring(5, 4));

            Session["mesAnoInicialInv"] = new DateTime(anoInicial, 7, 1);
            Session["mesAnoFinalInv"] = new DateTime(anoFinal, 6, 30);

            #endregion

            #region Solicitação de Investimento

            Session["Status"] = "Pendente";
            Session["permissaoSolicitacao"] = false;
            if (Session["nomeSolInv"] == null) Session["nomeSolInv"] = "";
            if (Session["dataInicialSolInv"] == null) Session["dataInicialSolInv"] = new DateTime(DateTime.Today.Year, 7, 1);
            if (Session["dataFimSolInv"] == null) Session["dataFimSolInv"] = new DateTime(DateTime.Today.AddYears(1).Year, 6, 30);
            Session["dataMinProj"] = new DateTime(DateTime.Today.Year, 7, 1);
            Session["dataMaxProj"] = new DateTime(DateTime.Today.AddYears(1).Year, 6, 30);

            Session["ListaInvestimentos"] = CarregaListaInvestimentos(false);
            Session["ListaMotivoSolInv"] = CarregaListaMotivosInvestimento();
            Session["ListaTiposProjetoSolInv"] = CarregaListaTipoProjeto();
            Session["ListaCategoriasItemProjetoSolInv"] = CarregaListaCategoriasItemProjeto();

            Session["saldoValorInvestimento"] = "";
            Session["hdSaldoInvSession"] = 0;
            Session["nomeProjetoInv"] = "";
            Session["valorTotalProjetoSolInv"] = 0;
            Session["dataInicioProjetoSolInv"] = DateTime.Today;
            Session["dataTerminoProjetoSolInv"] = DateTime.Today;
            Session["dataInicioMinProj"] = DateTime.Today;
            Session["inicioFuncionamentoProjetoSolInv"] = DateTime.Today;
            Session["motivoInicioSolInv"] = "";
            Session["descricaoSolInv"] = "";
            Session["justificativaSolInv"] = "";
            Session["alternativasSolInv"] = "";
            Session["riscosFatoresSolInv"] = "";

            Session["ListaInvestimentoSolicitacaoItem"] = new List<Investimento_Solicitacao_Item>();
            Session["ListaInvestimentoSolicitacaoItemDelete"] = new List<Investimento_Solicitacao_Item>();
            Session["ListaInvestimentoSolicitacaoItemCotacao"] = new List<Investimento_Solicitacao_Item_Cotacao>();
            Session["ListaInvestimentoSolicitacaoItemCotacaoDelete"] = new List<Investimento_Solicitacao_Item_Cotacao>();
            Session["ListaInvestimentoSolicitacaoProjecaoPagamento"] = new List<Investimento_Solicitacao_Projecao_Pagamento>();

            Session["ListaSimNao"] = CarregaListaSimNao();

            CarregaSessionProjPagValorMes((List<Investimento_Solicitacao_Projecao_Pagamento>)Session["ListaInvestimentoSolicitacaoProjecaoPagamento"], 0);

            //Session["ListaOrigemPagamento"] = CarregaListaOrigemPagamento();

            #endregion
        }

        public void CleanSessionsSolicitacaoInvestimentoItem()
        {
            Session["sequenciaItem"] = 0;

            Session["ListaCategoriasItemProjeto"] = CarregaListaCategoriasItemProjeto();
            
            //List<SelectListItem> listaProduto = new List<SelectListItem>();
            //foreach (var item in (List<SelectListItem>)Session["ListaProdutosApoloOriginal"])
            //{
            //    SelectListItem itemNovo = new SelectListItem();
            //    itemNovo.Text = item.Text;
            //    itemNovo.Value = item.Value;
            //    itemNovo.Selected = item.Selected;
            //    listaProduto.Add(itemNovo);
            //}
            //Session["ListaProdutoApolo"] = listaProduto;

            Session["ListaProdutoApolo"] = new List<SelectListItem>();

            Session["descricaoInvItem"] = "";
            Session["quantidadeInvItem"] = 0;
            Session["ListaInvestimentoSolicitacaoItemCotacaoExibe"] = new List<Investimento_Solicitacao_Item_Cotacao>();

            //List<SelectListItem> lista01 = new List<SelectListItem>();
            //foreach (var item in (List<SelectListItem>)Session["ListaFornecedoresInvOriginal"])
            //{
            //    SelectListItem itemNovo = new SelectListItem();
            //    itemNovo.Text = item.Text;
            //    itemNovo.Value = item.Value;
            //    itemNovo.Selected = item.Selected;
            //    lista01.Add(itemNovo);
            //}
            //Session["ListaFornecedor01"] = lista01;
            Session["ListaFornecedor01"] = new List<SelectListItem>();
            Session["ValorCotacao01"] = 0;

            //List<SelectListItem> lista02 = new List<SelectListItem>();
            //foreach (var item in (List<SelectListItem>)Session["ListaFornecedoresInvOriginal"])
            //{
            //    SelectListItem itemNovo = new SelectListItem();
            //    itemNovo.Text = item.Text;
            //    itemNovo.Value = item.Value;
            //    itemNovo.Selected = item.Selected;
            //    lista02.Add(itemNovo);
            //}
            //Session["ListaFornecedor02"] = lista02;
            Session["ListaFornecedor02"] = new List<SelectListItem>();
            Session["ValorCotacao02"] = 0;

            //List<SelectListItem> lista03 = new List<SelectListItem>();
            //foreach (var item in (List<SelectListItem>)Session["ListaFornecedoresInvOriginal"])
            //{
            //    SelectListItem itemNovo = new SelectListItem();
            //    itemNovo.Text = item.Text;
            //    itemNovo.Value = item.Value;
            //    itemNovo.Selected = item.Selected;
            //    lista03.Add(itemNovo);
            //}
            //Session["ListaFornecedor03"] = lista03;
            Session["ListaFornecedor03"] = new List<SelectListItem>();
            Session["ValorCotacao03"] = 0;

            Session["ListaNumeroCotacoes"] = CarregaListaNumeroCotacoes();

            Session["razaoNaoTer03CotacoesSolInv"] = "";
            Session["razaoExcederOrcamentoOuNaoUtilizarMenorCotacaoSolInv"] = "";
        }

        public void CarregaSessionValorMes(List<Investimento_Mes> listInvMes)
        {
            DateTime dataInicial = new DateTime(2018, 7, 1);
            DateTime dataFinal = new DateTime(2019, 6, 1);

            while (dataInicial <= dataFinal)
            {
                decimal valor = 0;
                int anoMes = Convert.ToInt32(dataInicial.ToString("yyyyMM"));

                Investimento_Mes invMes = listInvMes.Where(w => w.AnoMes == anoMes).FirstOrDefault();
                if (invMes != null) valor = invMes.ValorOrcado;

                if (Request.Browser.IsMobileDevice)
                    Session["valorMesInv_" + dataInicial.ToString("MMM")] = valor;
                else
                    Session["valorMesInv_" + dataInicial.ToString("MMM")] = String.Format("{0:N2}", valor);

                dataInicial = dataInicial.AddMonths(1);
            }
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

            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

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

        public string CarregaMsgSaldoInvestimento(int id, DateTime data)
        {
            string retorno = "Saldo: R$ 0,00";
            int anoMesAtual = Convert.ToInt32(data.ToString("yyyyMM"));
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Investimento_Mes saldoAtual = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == id && w.AnoMes == anoMesAtual).FirstOrDefault();
            if (saldoAtual != null)
                //retorno = "Saldo em " + DateTime.Today.ToString("MMM/yyyy") + ": " + String.Format("{0:C}", saldoAtual.Saldo);
                retorno = "Saldo: " + String.Format("{0:C}", saldoAtual.Saldo);

            return retorno;
        }

        public decimal CarregaValorSaldoInvestimento(int id, DateTime data)
        {
            decimal retorno = 0;
            int anoMesAtual = Convert.ToInt32(data.ToString("yyyyMM"));
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Investimento_Mes saldoAtual = hlbapp.Investimento_Mes
                .Where(w => w.IDInvestimento == id && w.AnoMes == anoMesAtual).FirstOrDefault();
            if (saldoAtual != null)
                retorno = saldoAtual.Saldo;

            return retorno;
        }

        public string VerificaOrigemInvestimento(string anoFiscal)
        {
            string origem = "Alemanha";

            int anoInicial = Convert.ToInt32(anoFiscal.Substring(0, 4));
            int anoFinal = Convert.ToInt32(anoFiscal.Substring(5, 4));

            if ((DateTime.Today.Year == anoInicial && DateTime.Today.Month > 6) 
                || (DateTime.Today.Year == anoFinal && DateTime.Today.Month < 7))
                origem = "Manual";

            return origem;
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

        public List<SelectListItem> CarregaListaDepartamento(bool todos)
        {
            List<SelectListItem> listaDepartamento = new List<SelectListItem>();

            Apolo10Entities apolo = new Apolo10Entities();

            if (todos)
            {
                listaDepartamento.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaDepartamentos = apolo.FUNCIONARIO
                .Where(w => w.USERDeptoSistemaOrcamento == "Sim")
                .OrderBy(o => o.FuncNome)
                .ToList();

            foreach (var item in listaDepartamentos)
            {
                listaDepartamento.Add(new SelectListItem
                {
                    Text = item.FuncNome,
                    Value = item.FuncCod,
                    Selected = false
                });
            }

            return listaDepartamento;
        }

        public List<SelectListItem> CarregaListaResponsaveis(string departamento)
        {
            List<SelectListItem> ddlResponsaveis = new List<SelectListItem>();

            Apolo10Entities apolo = new Apolo10Entities();

            var listaResponsaveis = apolo.FUNCIONARIO
                .OrderBy(o => o.FuncNome)
                .ToList();

            foreach (var item in listaResponsaveis)
            {
                int existeFuncionarioXDepartamento = apolo.GRP_FUNC
                    .Where(w => w.FuncCod == departamento
                        && w.GrpFuncObs.Contains("Investimentos")
                        && w.GrpFuncCod == item.FuncCod)
                    .Count();

                if (existeFuncionarioXDepartamento > 0)
                {
                    ddlResponsaveis.Add(new SelectListItem
                    {
                        Text = item.FuncNome,
                        Value = item.FuncCod,
                        Selected = false
                    });
                }
            }

            return ddlResponsaveis;
        }

        public List<SelectListItem> CarregaListaAnoFiscal(bool todos, bool desconsiderarAnteriores)
        {
            List<SelectListItem> listaAnoFiscal = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (todos)
            {
                listaAnoFiscal.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaAnoFiscalBD = hlbapp.AnoFiscal
                .OrderBy(o => o.AnoFiscal1).ToList();

            foreach (var item in listaAnoFiscalBD)
            {
                int anoFinal = Convert.ToInt32(item.AnoFiscal1.Substring(5, 4));

                if (!(desconsiderarAnteriores && anoFinal <= DateTime.Today.Year && DateTime.Today.Month > 6))
                {
                    bool selected = false;
                    //if (!todos && listaAnoFiscalBD.IndexOf(item) == 0)
                    if (item.AnoFiscal1.Substring(0, 4) == DateTime.Today.Year.ToString())
                        selected = true;

                    listaAnoFiscal.Add(new SelectListItem
                    {
                        Text = item.AnoFiscal1,
                        Value = item.AnoFiscal1,
                        Selected = selected
                    });
                }
            }

            return listaAnoFiscal;
        }

        public List<SelectListItem> CarregaListaAnoMes(int anoMesInicial, int anoMesFinal)
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            DateTime dataInicial = new DateTime(Convert.ToInt32(anoMesInicial.ToString().Substring(0, 4)), 7, 1);
            //DateTime dataInicial = DateTime.Today;
            DateTime dataFinal = new DateTime(Convert.ToInt32(anoMesFinal.ToString().Substring(0, 4)), 6, 30);

            while (dataInicial < dataFinal)
            {
                lista.Add(new SelectListItem
                {
                    Text = dataInicial.ToString("yyyyMM"),
                    Value = dataInicial.ToString("yyyyMM"),
                    Selected = false
                });

                dataInicial = dataInicial.AddMonths(1);
            }

            return lista;
        }

        public List<SelectListItem> CarregaListaSimNao()
        {
            List<SelectListItem> listaSimNao = new List<SelectListItem>();

            listaSimNao.Add(new SelectListItem
            {
                Text = "Sim",
                Value = "Sim",
                Selected = false
            });

            listaSimNao.Add(new SelectListItem
            {
                Text = "Não",
                Value = "Não",
                Selected = false
            });

            return listaSimNao;
        }

        public List<SelectListItem> CarregaListaInvestimentos(bool todos)
        {
            List<SelectListItem> ddl = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            string login = Session["login"].ToString().ToUpper();

            if (todos)
            {
                ddl.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaInvestimentos = hlbapp.Investimento
                .OrderBy(o => o.NumeroProjeto)
                .ToList();

            foreach (var item in listaInvestimentos)
            {
                USUARIO responsavel = apolo.USUARIO
                    .Where(w => apolo.FUNCIONARIO.Any(a => a.FuncCod == item.Responsavel
                            && w.UsuCod == a.UsuCod)
                        && w.UsuCod == login)
                    .FirstOrDefault();

                List<USUARIO> gerentesResponsavelInvestimento01 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == item.Responsavel
                            && g.FuncCod == r.FuncCod
                            && g.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                List<USUARIO> gerentesResponsavelInvestimento02 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == r.FuncCod
                            && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == item.Responsavel
                                && g.FuncCod == g2.GrpFuncCod
                                && g.GrpFuncObs == "RDV")
                            && g2.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                List<USUARIO> gerentesResponsavelInvestimento03 = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && apolo.GRP_FUNC.Any(g3 => g3.FuncCod == r.FuncCod
                            && apolo.GRP_FUNC.Any(g2 => g2.FuncCod == g3.GrpFuncCod
                                && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == item.Responsavel
                                    && g.FuncCod == g2.GrpFuncCod
                                    && g.GrpFuncObs == "RDV")
                                && g2.GrpFuncObs == "RDV")
                            && g3.GrpFuncObs == "RDV"))
                        && u.UsuCod == login)
                    .ToList();

                if (responsavel != null || gerentesResponsavelInvestimento01.Count > 0 || gerentesResponsavelInvestimento02.Count > 0
                     || gerentesResponsavelInvestimento03.Count > 0)
                {
                    ddl.Add(new SelectListItem
                    {
                        Text = item.NumeroProjeto + " - " + item.NomeProjeto,
                        Value = item.ID.ToString(),
                        Selected = false
                    });
                }
            }

            return ddl;
        }
        
        public List<SelectListItem> CarregaListaMotivosInvestimento()
        {
            List<SelectListItem> listaMotivosInvestimento = new List<SelectListItem>();

            listaMotivosInvestimento.Add(new SelectListItem
            {
                Text = "Sanidade",
                Value = "Sanidade",
                Selected = false
            });

            listaMotivosInvestimento.Add(new SelectListItem
            {
                Text = "Redução de custo",
                Value = "Redução de custo",
                Selected = false
            });

            listaMotivosInvestimento.Add(new SelectListItem
            {
                Text = "Aumento de capacidade",
                Value = "Aumento de capacidade",
                Selected = false
            });

            listaMotivosInvestimento.Add(new SelectListItem
            {
                Text = "Reposição",
                Value = "Reposição",
                Selected = false
            });

            listaMotivosInvestimento.Add(new SelectListItem
            {
                Text = "Legal",
                Value = "Legal",
                Selected = false
            });

            return listaMotivosInvestimento;
        }

        public List<SelectListItem> CarregaListaTipoProjeto()
        {
            List<SelectListItem> listaTipoProjeto = new List<SelectListItem>();

            listaTipoProjeto.Add(new SelectListItem
            {
                Text = "Troca",
                Value = "Troca",
                Selected = false
            });

            listaTipoProjeto.Add(new SelectListItem
            {
                Text = "Novo item",
                Value = "Novo item",
                Selected = false
            });

            listaTipoProjeto.Add(new SelectListItem
            {
                Text = "Reparo ou reforma",
                Value = "Reparo ou reforma",
                Selected = false
            });

            return listaTipoProjeto;
        }

        public List<SelectListItem> CarregaListaCategoriasItemProjeto()
        {
            List<SelectListItem> listaCategoriasItemProjeto = new List<SelectListItem>();

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Terreno",
                Value = "Terreno",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Prédio",
                Value = "Prédio",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Máquina",
                Value = "Máquina",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Carro",
                Value = "Carro",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Outros Veículos",
                Value = "Outros Veículos",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Hardware",
                Value = "Hardware",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Móveis e utensílios",
                Value = "Móveis e utensílios",
                Selected = false
            });

            listaCategoriasItemProjeto.Add(new SelectListItem
            {
                Text = "Software",
                Value = "Software",
                Selected = false
            });

            return listaCategoriasItemProjeto;
        }

        public List<SelectListItem> CarregaFornecedores(string pesquisa)
        {
            List<SelectListItem> listaClientesDDL = new List<SelectListItem>();
            List<String> listaClientesOriginalApolo = new List<string>();

            MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities apoloStatic = 
                new MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities();

            var listaClientes = apoloStatic.ENTIDADE
                .Where(w => w.StatEntCod != "05"
                    && (w.EntNome.Contains(pesquisa) || w.EntCpfCgc.Contains(pesquisa) || w.EntCod.Contains(pesquisa) || pesquisa == "")
                )
                .Join(
                    apoloStatic.ENT_CATEG.Where(c => c.CategCodEstr == "02"),
                    e => e.EntCod,
                    c => c.EntCod,
                    (e, c) => new { ENTIDADE = e, ENT_CATEG = c })
                .GroupJoin(
                    apoloStatic.CIDADE,
                    ecid => ecid.ENTIDADE.CidCod,
                    c => c.CidCod,
                    (ecid, c) => new { ENTIDADE = ecid, CIDADE = c })
                         .SelectMany(n => n.CIDADE.DefaultIfEmpty(),
                                    (n, c) => new { n.ENTIDADE, CIDADE = c })
                .OrderBy(o => o.ENTIDADE.ENTIDADE.EntNome)
                .Select(c => new
                {
                    c.ENTIDADE.ENTIDADE.EntCod,
                    c.ENTIDADE.ENTIDADE.EntNome,
                    c.ENTIDADE.ENTIDADE.EntEnder,
                    c.ENTIDADE.ENTIDADE.EntEnderNo,
                    c.ENTIDADE.ENTIDADE.EntEnderComp,
                    c.ENTIDADE.ENTIDADE.EntBair,
                    c.CIDADE.CidNomeComp,
                    c.CIDADE.UfSigla,
                    c.CIDADE.PaisSigla,
                    c.ENTIDADE.ENTIDADE.EntCpfCgc,
                    c.ENTIDADE.ENTIDADE.EntRgIe,
                    c.ENTIDADE.ENTIDADE.EntAgropInsc,
                    c.ENTIDADE.ENTIDADE.EntTipoFJ
                }).ToList();

            foreach (var item in listaClientes)
            {
                string cidadeStr = "";
                //CIDADE cidade = apoloStatic.CIDADE.Where(w => w.CidCod == item.CidCod).FirstOrDefault();
                if (item.CidNomeComp != null)
                    cidadeStr = " - " + item.CidNomeComp + " - " + item.UfSigla
                         + " - " + item.PaisSigla;

                //bool select = false;
                //if (item.EntCod == codigoCliente)
                //    select = true;

                string ie = "";
                if (item.EntRgIe != "" && item.EntRgIe != null)
                    ie = item.EntRgIe;
                else
                    ie = item.EntAgropInsc;

                string tipoNacional = " - CNPJ: ";
                if (item.EntTipoFJ.Equals("Física")) tipoNacional = " - CPF: ";

                string tipoEstadual = " - IE: ";
                if (item.EntTipoFJ.Equals("Física")) tipoEstadual = " - RG: ";

                listaClientesDDL.Add(new SelectListItem
                {
                    Text = item.EntCod + " - " + item.EntNome 
                        //+ " - " + item.EntEnder + " " + item.EntEnderNo + " - " + item.EntEnderComp + " - " + item.EntBair
                        + cidadeStr + tipoNacional + item.EntCpfCgc
                        // + tipoEstadual + ie
                        ,
                    Value = item.EntCod,
                    Selected = false
                });

                listaClientesOriginalApolo.Add(item.EntCod);
            }

            //Session["listaFornecedoresOriginalApolo"] = listaClientesOriginalApolo;

            return listaClientesDDL;
        }

        public List<SelectListItem> CarregaListaNumeroCotacoes()
        {
            List<SelectListItem> listaNumeroCotacoes = new List<SelectListItem>();

            for (int i = 1; i <= 3; i++)
            {
                listaNumeroCotacoes.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = false
                }); 
            }

            return listaNumeroCotacoes;
        }

        public List<SelectListItem> CarregaListaOrigemPagamento()
        {
            List<SelectListItem> listaOrigemPagamento = new List<SelectListItem>();

            listaOrigemPagamento.Add(new SelectListItem
            {
                Text = "Adiantamento",
                Value = "Adiantamento",
                Selected = false
            });

            listaOrigemPagamento.Add(new SelectListItem
            {
                Text = "Nota de Venda",
                Value = "Nota de Venda",
                Selected = false
            });

            return listaOrigemPagamento;
        }

        public List<SelectListItem> CarregaProdutosApolo(string pesquisa)
        {
            List<SelectListItem> listaProdutosDDL = new List<SelectListItem>();
            
            MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities apoloStatic =
                new MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities();

            var listaProdutos = apoloStatic.PRODUTO
                .Where(w => w.ProdStat == "Ativado"
                    && (w.ProdNome.Contains(pesquisa) || w.ProdCodEstr.Contains(pesquisa) || pesquisa == "") 
                )
                .OrderBy(o => o.ProdNome)
                .ToList();

            foreach (var item in listaProdutos)
            {
                listaProdutosDDL.Add(new SelectListItem
                {
                    Text = item.ProdCodEstr + " - " + item.ProdNome,
                    Value = item.ProdCodEstr,
                    Selected = false
                });
            }

            return listaProdutosDDL;
        }

        public List<SelectListItem> CarregaCondPag(string pesquisa)
        {
            List<SelectListItem> listaDDL = new List<SelectListItem>();

            MvcAppHylinedoBrasilMobile.Models.bdApolo2.Apolo10Entities apoloStatic =
                new MvcAppHylinedoBrasilMobile.Models.bdApolo2.Apolo10Entities();

            var lista = apoloStatic.COND_PAG
                .Where(w => w.CondPagOper == "Pagar" && (w.CondPagDataFim == null || w.CondPagDataFim >= DateTime.Today)
                    && (w.CondPagNome.Contains(pesquisa) || pesquisa == "")
                )
                .OrderBy(o => o.CondPagNome)
                .ToList();

            foreach (var item in lista)
            {
                listaDDL.Add(new SelectListItem
                {
                    Text = item.CondPagNome,
                    Value = item.CondPagCod,
                    Selected = false
                });
            }

            return listaDDL;
        }

        public List<SelectListItem> CarregaStatusCompras()
        {
            List<SelectListItem> lista = new List<SelectListItem>();

            lista.Add(new SelectListItem
            {
                Text = "Voltar para responsável",
                Value = "Pendente",
                Selected = false
            });

            lista.Add(new SelectListItem
            {
                Text = "Liberar para Aprovação",
                Value = "Cotado",
                Selected = false
            });

            return lista;
        }

        #endregion

        #region Json Methods

        #region Solicitação de Investimento

        [HttpPost]
        public ActionResult AtualizaCampoSolInv(string valor, string sessionName, string tipoControle, string nameDiv)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string retorno = "";

            if (tipoControle == "ddl")
                AtualizaDDL(valor, (List<SelectListItem>)Session[sessionName]);
            else if (tipoControle == "txt")
                Session[sessionName] = valor;

            if (sessionName == "ListaInvestimentos")
            {
                int id = Convert.ToInt32(valor);
                retorno = CarregaValorSaldoInvestimento(id, DateTime.Today).ToString();
                Session[nameDiv] = retorno;
                //Session["hdSaldoInvSession"] = CarregaValorSaldoInvestimento(id, DateTime.Today);
            }

            return Json(retorno);
        }

        [HttpPost]
        public ActionResult LocalizaInvestimentoObj(string valor)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(valor);
            Investimento inv = hlbapp.Investimento.Where(w => w.ID == id).FirstOrDefault();

            return Json(inv);
        }

        #region Item da Solicitação de Investimento

        [HttpPost]
        public ActionResult FiltraFornecedor(string pesquisa, string nameLista)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string filtroNome = pesquisa.ToUpper();

            //List<SelectListItem> listaFiltroOriginal = (List<SelectListItem>)Session["ListaFornecedoresInvOriginal"];
            List<SelectListItem> listaFiltroOriginal = CarregaFornecedores(filtroNome);

            List<SelectListItem> listaFiltro = listaFiltroOriginal
                .Where(w => w.Text.ToUpper().Contains(filtroNome)).ToList();

            Session[nameLista] = listaFiltro;

            return Json(listaFiltro);
        }

        [HttpPost]
        public ActionResult FiltraCodigoApolo(string pesquisa, string nameLista)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string filtroNome = pesquisa.ToUpper();

            //List<SelectListItem> listaFiltroOriginal = (List<SelectListItem>)Session["ListaProdutosApoloOriginal"];
            List<SelectListItem> listaFiltroOriginal = CarregaProdutosApolo(filtroNome);

            List<SelectListItem> listaFiltro = listaFiltroOriginal
                .Where(w => w.Text.ToUpper().Contains(filtroNome)).ToList();

            Session[nameLista] = listaFiltro;

            return Json(listaFiltro);
        }

        [HttpPost]
        public ActionResult FiltraCondPag(string pesquisa, string nameLista)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string filtroNome = pesquisa.ToUpper();

            //List<SelectListItem> listaFiltroOriginal = (List<SelectListItem>)Session["ListaProdutosApoloOriginal"];
            List<SelectListItem> listaFiltroOriginal = CarregaCondPag(filtroNome);

            List<SelectListItem> listaFiltro = listaFiltroOriginal
                .Where(w => w.Text.ToUpper().Contains(filtroNome)).ToList();

            Session[nameLista] = listaFiltro;

            return Json(listaFiltro);
        }

        #endregion

        #endregion

        #region Investimentos

        [HttpPost]
        public ActionResult CarregaResponsaveis(string departamento)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                if (Session["usuario"].ToString() != "0")
                {
                    List<SelectListItem> items = new List<SelectListItem>();

                    items = CarregaListaResponsaveis(departamento);
                    //Session["ListaResponsavelInv"] = items;
                    //Session["ListaResponsavelInv"] = AtualizaDDL(departamento, (List<SelectListItem>)Session["ListaDepartamentoInv"]);

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
        public ActionResult VerificaNumeroDepartamento(string departamento, string nProjeto, string anoFiscal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            int anoMesInicial = Convert.ToInt32(anoFiscal.Substring(0, 4) + "07");
            int anoMesFinal = Convert.ToInt32(anoFiscal.Substring(5, 4) + "06");

            string msg = "";

            int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);

            int existe = hlbapp.Investimento.Where(w => w.NumeroProjeto == nProjeto
                && w.Departamento == departamento
                && w.AnoMesInicial == anoMesInicial && w.AnoMesFinal == anoMesFinal
                && w.ID != idSelecionado).Count();

            if (existe > 0)
            {
                FUNCIONARIO depObj = apolo.FUNCIONARIO.Where(w => w.FuncCod == departamento).FirstOrDefault();

                msg = "O número do projeto " + nProjeto + " já existe cadastro no departamento " + depObj.FuncNome 
                    + " no Ano Fiscal " + anoFiscal + "!";
            }

            return Json(msg);
        }

        [HttpPost]
        public ActionResult VerificaAnoFiscal(string aFiscal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return Json(VerificaOrigemInvestimento(aFiscal));
        }

        #endregion

        #region Ano Fiscal

        [HttpPost]
        public ActionResult AtualizaAnoFiscal(string aFiscal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["ListaAnoFiscalInv"] != null)
                AtualizaDDL(aFiscal, (List<SelectListItem>)Session["ListaAnoFiscalInv"]);

            return Json("");
        }

        #endregion

        #endregion
    }
}
