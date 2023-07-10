using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Data.Objects;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class AuditoriaController : Controller
    {
        #region Menu

        public ActionResult MenuAuditoria()
        {
            return View();
        }

        #endregion

        #region Cadastros

        #region Requisitos

        #region List Methods

        public List<Auditoria_Requisito> ListRequisitos(string pesquisa, string tipoUnidade)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var lista = hlbapp.Auditoria_Requisito
                .Where(w => (w.Descricao.Contains(pesquisa) || pesquisa == "")
                    && (w.TipoUnidade == tipoUnidade || tipoUnidade == "(Todos)"))
                .OrderBy(o => o.TipoUnidade).ThenBy(t => t.Codigo)
                .ToList();

            return lista;
        }

        public List<Auditoria_Requisito> FilterListRequisitos()
        {
            CleanSessions();

            string tipoUnidade = ((List<SelectListItem>)Session["FiltroDDLListaTiposUnidade"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;

            return ListRequisitos(Session["pesquisaSession"].ToString(), tipoUnidade);
        }

        #endregion

        #region Lista Requisitos

        public ActionResult ListaRequisitos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            //Session["FiltroDDLListaDepartamentos"] = CarregaListaDepartamento(true);
            if (Session["FiltroDDLListaTiposUnidade"] == null) Session["FiltroDDLListaTiposUnidade"] = CarregaListaTiposUnidade(true);

            Session["msg"] = "";

            Session["ListaRequisitos"] = FilterListRequisitos();
            return View();
        }

        public ActionResult SearchRequisito(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["pesquisa"] != null)
                Session["pesquisaSession"] = model["pesquisa"];

            if (model["TipoUnidade"] != null)
                AtualizaDDL(model["TipoUnidade"], (List<SelectListItem>)Session["FiltroDDLListaTiposUnidade"]);

            #endregion

            Session["ListaRequisitos"] = ListRequisitos(model["pesquisa"], model["TipoUnidade"]);
            return View("ListaRequisitos");
        }

        #endregion

        #region CRUD Methods

        public void CarregaRequisito(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Auditoria_Requisito requisito = hlbapp.Auditoria_Requisito.Where(w => w.ID == id).FirstOrDefault();

            Session["grupoRequisito"] = requisito.Grupo;
            Session["codigoRequisito"] = requisito.Codigo;
            Session["descricaoRequisito"] = requisito.Descricao;
            AtualizaDDL(requisito.TipoUnidade, (List<SelectListItem>)Session["DLListaTiposUnidade"]);
            Session["ListaGruposRequisitos"] = CarregaListaGrupoRequisito(false, requisito.TipoUnidade);
            AtualizaDDL(requisito.Grupo, (List<SelectListItem>)Session["ListaGruposRequisitos"]);
        }

        public ActionResult CreateRequisito()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("Requisito");
        }

        public ActionResult EditRequisito(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaRequisito(id);

            return View("Requisito");
        }

        public ActionResult SaveRequisito(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["descricao"] != null)
            {
                #region Carrega Valores

                #region Grupo

                string grupo = "";
                if (model["Grupo"] != null) grupo = model["Grupo"];

                #endregion

                #region Código

                string codigo = "";
                if (model["codigo"] != null) codigo = model["codigo"];

                #endregion

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null) descricao = model["descricao"];

                #endregion

                #region Tipo de Unidade

                string tipoUnidade = "";
                if (model["TipoUnidade"] != null) tipoUnidade = model["TipoUnidade"];

                #endregion

                #endregion

                #region Insere no WEB

                Auditoria_Requisito requisito = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    requisito = new Auditoria_Requisito();
                    requisito.Usuario = Session["login"].ToString().ToUpper();
                    requisito.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    requisito = hlbapp.Auditoria_Requisito.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                decimal codigoA = 0;
                if (requisito.Codigo != "" && requisito.Codigo != null)
                    codigoA = Convert.ToDecimal(requisito.Codigo.Replace(".", ","));

                requisito.Codigo = codigo;
                requisito.Grupo = grupo;
                requisito.Descricao = descricao;
                requisito.TipoUnidade = tipoUnidade;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Auditoria_Requisito.AddObject(requisito);

                hlbapp.SaveChanges();

                #region Atualiza Códigos do Requisito

                var listaRequisitosTipoUnidade = hlbapp.Auditoria_Requisito
                    .Where(w => w.TipoUnidade == tipoUnidade 
                        && w.ID != requisito.ID
                        )
                    .OrderBy(o => o.Codigo)
                    .ToList();

                List<Auditoria_Requisito> listaOrdenada = new List<Auditoria_Requisito>();

                foreach (var item in listaRequisitosTipoUnidade)
                {
                    decimal codigoItem = Convert.ToDecimal(item.Codigo.Replace(".",","));
                    if (item.Codigo == requisito.Codigo && codigoA >= codigoItem)
                        listaOrdenada.Add(requisito);
                    listaOrdenada.Add(item);
                    if (item.Codigo == requisito.Codigo && codigoA < codigoItem)
                        listaOrdenada.Add(requisito);
                }

                //if (listaOrdenada.Where(w => w.ID == requisito.ID).Count() == 0)
                //    listaOrdenada.Add(requisito);

                decimal codigoP = Convert.ToDecimal(codigo.Replace(".", ","));
                decimal codigoD = 1.00m;
                //decimal codigoD = Convert.ToDecimal(codigo.Replace(".", ","));

                foreach (var item in listaOrdenada)
                {
                    //decimal codigoItem = Convert.ToDecimal(item.Codigo.Replace(".",","));
                    //if (codigoItem >= codigoP)
                    //if (item.ID != requisito.ID)
                    //{
                        codigoD = codigoD + 0.01m;
                        //if (codigoD == codigoP) codigoD = codigoD + 0.01m;
                        item.Codigo = codigoD.ToString().Replace(",",".");
                    //}
                }

                #endregion

                #endregion

                hlbapp.SaveChanges();
            }

            Session["ListaRequisitos"] = FilterListRequisitos();
            return View("ListaRequisitos");
        }

        public ActionResult ConfirmaDeleteRequisito(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Auditoria_Requisito requisito = hlbapp.Auditoria_Requisito.Where(w => w.ID == id).FirstOrDefault();

            int existe = hlbapp.Auditoria_Visita_Requisito.Where(w => w.IDRequisito == id).Count();

            if (existe > 0)
            {
                ViewBag.Erro = "Não é possível excluir o Requisito " + requisito.Descricao
                    + " pois existem visitas relacionadas!";
                Session["ListaRequisitos"] = FilterListRequisitos();
                return View("ListaRequisitos");
            }

            return View();
        }

        public ActionResult DeleteRequisito()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            Auditoria_Requisito requisito = hlbapp.Auditoria_Requisito.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Auditoria_Requisito.DeleteObject(requisito);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Requisito " + requisito.Descricao + " excluído com sucesso!";

            Session["ListaRequisitos"] = FilterListRequisitos();
            return View("ListaRequisitos");
        }

        #endregion

        #endregion

        #endregion

        #region Manutenção

        #region Visitas

        #region List Methods

        public List<Auditoria_Visita> ListVisitas(string departamento, DateTime dataInicial, DateTime dataFinal)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            string login = Session["login"].ToString().ToUpper();

            FUNCIONARIO usuario = apolo.FUNCIONARIO
                .Where(w => w.UsuCod == login)
                .FirstOrDefault();
            
            var lista = hlbapp.Auditoria_Visita
                .Where(w => (w.Departamento == departamento || departamento == "(Todos)")
                    && w.DataVisita >= dataInicial && w.DataVisita <= dataFinal)
                .OrderBy(o => o.DataVisita)
                .ToList();

            List<Auditoria_Visita> listaFiltrada = new List<Auditoria_Visita>();

            foreach (var item in lista)
            {
                int existeFuncionarioXUsuario = apolo.GRP_FUNC
                    .Where(w => w.FuncCod == item.Departamento
                        && w.GrpFuncCod == usuario.FuncCod)
                    .Count();

                if (existeFuncionarioXUsuario > 0)
                {
                    listaFiltrada.Add(item);
                }
            }

            return listaFiltrada;
        }

        public List<Auditoria_Visita> FilterListVisitas()
        {
            CleanSessions();

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialVisita"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalVisita"].ToString());
            string departamento = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;

            return ListVisitas(departamento, dataInicial, dataFinal);
        }

        #endregion

        #region Lista Visitas

        public ActionResult ListaVisitas()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["FiltroDDLListaDepartamentos"] = CarregaListaDepartamento(true);

            Session["msg"] = "";

            Session["ListaVisitas"] = FilterListVisitas();
            return View();
        }

        public ActionResult SearchVisita(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialVisita"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialVisita"]);
                Session["dataInicialVisita"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialVisita"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalVisita"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalVisita"]);
                Session["dataFinalVisita"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalVisita"].ToString());

            if (model["Departamento"] != null)
                AtualizaDDL(model["Departamento"], (List<SelectListItem>)Session["FiltroDDLListaDepartamentos"]);

            #endregion

            Session["ListaVisitas"] = ListVisitas(model["Departamento"], dataInicial, dataFinal);
            return View("ListaVisitas");
        }

        #endregion

        #region CRUD Methods

        public void CarregaVisita(int id, string origem)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            Auditoria_Visita visita = hlbapp.Auditoria_Visita.Where(w => w.ID == id).FirstOrDefault();
            FUNCIONARIO departamento = apolo.FUNCIONARIO.Where(w => w.FuncCod == visita.Departamento).FirstOrDefault();

            if (origem == "Edit")
            {
                Session["FiltraDDLGrupoRequisito"] = CarregaListaGrupoRequisito(true, departamento.USERTipoUnidade);
                Session["FiltraDDLStatus"] = CarregaListaStatusVisita(true);
            }

            AtualizaDDL(visita.Departamento, (List<SelectListItem>)Session["DLListaDepartamentos"]);
            Session["dataVisita"] = visita.DataVisita;
            Session["nucleoVisita"] = visita.Nucleo;
            AtualizaDDL(visita.Responsavel, (List<SelectListItem>)Session["DLListaResponsaveis"]);
            Session["comentariosGeralVisita"] = visita.ComentarioGeral;

            string filtroGrupoRequisito = ((List<SelectListItem>)Session["FiltraDDLGrupoRequisito"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            string filtroStatus = ((List<SelectListItem>)Session["FiltraDDLStatus"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;
            Session["ListaRequisitosVisita"] = CarregaRequisitosVisita(id, filtroGrupoRequisito, filtroStatus);
        }

        public List<Auditoria_Visita_Requisito> CarregaRequisitosVisita(int idVisita, string grupo, string status)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int idGrupo = 0;
            if (grupo != "(Todos)") idGrupo = Convert.ToInt32(grupo);

            List<Auditoria_Visita_Requisito> lista = hlbapp.Auditoria_Visita_Requisito
                .Where(w => w.IDVisita == idVisita
                    && (w.Status == status || status == "(Todos)")).ToList();
            List<Auditoria_Visita_Requisito> listaRetorno = new List<Auditoria_Visita_Requisito>();

            foreach (var item in lista)
            {
                int existe = hlbapp.Auditoria_Requisito
                    .Where(w => w.ID == item.IDRequisito
                        && hlbapp.Auditoria_Grupo.Any(a => a.Descricao == w.Grupo
                            && a.TipoUnidade == w.TipoUnidade
                            && a.ID == idGrupo))
                    .Count();

                if (existe > 0 || idGrupo == 0)
                    listaRetorno.Add(item);
            }

            return listaRetorno;
        }

        public ActionResult SearchRequisitosVisitaPorGrupo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int idVisita = Convert.ToInt32(Session["idSelecionado"]);

            #region Carrega Valores

            if (model["Grupo"] != null)
                AtualizaDDL(model["Grupo"], (List<SelectListItem>)Session["FiltraDDLGrupoRequisito"]);

            if (model["Status"] != null)
                AtualizaDDL(model["Status"], (List<SelectListItem>)Session["FiltraDDLStatus"]);

            #endregion

            Session["ListaRequisitosVisita"] = CarregaRequisitosVisita(idVisita, model["Grupo"], model["Status"]);
            return View("Visita");
        }

        public ActionResult GeraVisita()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["ListaNucleos"] = new List<SelectListItem>();

            return View();
        }

        public ActionResult SaveGeraVisita(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            if (model["Departamento"] != null)
            {
                #region Carrega Valores

                #region Departamento

                string departamento = "";
                if (model["Departamento"] != null)
                {
                    departamento = model["Departamento"];
                    Session["DLListaDepartamentos"] = AtualizaDDL(departamento, (List<SelectListItem>)Session["DLListaDepartamentos"]);
                }

                #endregion

                #region Núcleo

                string nucleo = "";
                if (model["Nucleo"] != null)
                {
                    nucleo = model["Nucleo"];
                    Session["ListaNucleos"] = AtualizaDDL(nucleo, (List<SelectListItem>)Session["ListaNucleos"]);
                }

                #endregion

                #region Data Visita

                DateTime dataVisita = DateTime.Today;
                if (model["dataVisita"] != null)
                {
                    dataVisita = Convert.ToDateTime(model["dataVisita"]);
                    Session["dataVisita"] = dataVisita;
                }

                #endregion

                #region Responsável

                string responsavel = "";
                if (model["Responsavel"] != null)
                {
                    responsavel = model["Responsavel"];
                    Session["DLListaResponsaveis"] = AtualizaDDL(responsavel, (List<SelectListItem>)Session["DLListaResponsaveis"]);
                }

                #endregion

                #endregion

                #region Verifica se existem itens pendentes. Caso exista, primeiro exibe para o usuário confirmar.

                List<Auditoria_Visita_Requisito> listaRequisitoPendentes = hlbapp.Auditoria_Visita_Requisito
                    .Where(w => w.StatusResolucao != "Aprovado" && w.Status == "Não Conforme"
                        && hlbapp.Auditoria_Visita.Any(a => a.ID == w.IDVisita
                            && a.Departamento == departamento && a.Nucleo == nucleo))
                    .ToList();

                if (listaRequisitoPendentes.Count > 0 && Session["verificaRequisitosPendentes"].ToString() != "OK")
                {
                    Session["verificaRequisitosPendentes"] = "OK";
                    Session["ListaRequisitosVisita"] = listaRequisitoPendentes;
                    ViewBag.Erro = "EXISTEM OS ITENS ABAIXO PENDENTES! SE GERAR A VISITA, ELES NÃO SERÃO INSERIDOS! CONFIRMA A GERAÇÃO DA VISITA?";
                    return View("ConfirmaGeraVisita");
                }

                #endregion

                Session["verificaRequisitosPendentes"] = "";

                #region Insere no WEB

                Auditoria_Visita visita = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    visita = new Auditoria_Visita();
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    visita = hlbapp.Auditoria_Visita.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                visita.DataVisita = dataVisita;
                visita.Departamento = departamento;
                visita.Nucleo = nucleo;
                visita.Responsavel = responsavel;
                visita.ComentarioGeral = "";

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Auditoria_Visita.AddObject(visita);

                hlbapp.SaveChanges();

                #region Insere Requisitos da Visita no WEB de acordo com o Departamento

                FUNCIONARIO departamentoApolo = apolo.FUNCIONARIO
                    .Where(w => w.FuncCod == departamento).FirstOrDefault();

                var listaRequisitos = hlbapp.Auditoria_Requisito
                    .Where(w => w.TipoUnidade == departamentoApolo.USERTipoUnidade).ToList();

                foreach (var item in listaRequisitos)
                {
                    #region Verifica se existe o requisito pendente. Caso existe, informe ao auditor e não insere na visita.

                    Auditoria_Visita_Requisito existeRequisitoPendente = hlbapp.Auditoria_Visita_Requisito
                        .Where(w => w.IDRequisito == item.ID && w.StatusResolucao != "Aprovado" && w.Status == "Não Conforme"
                            && hlbapp.Auditoria_Visita.Any(a => a.ID == w.IDVisita
                                && a.Departamento == visita.Departamento && a.Nucleo == visita.Nucleo))
                        .FirstOrDefault();

                    #endregion

                    if (existeRequisitoPendente == null)
                    {
                        Auditoria_Visita_Requisito requisitoVisita = hlbapp.Auditoria_Visita_Requisito
                            .Where(w => w.ID == item.ID).FirstOrDefault();

                        requisitoVisita = new Auditoria_Visita_Requisito();
                        requisitoVisita.IDVisita = visita.ID;
                        requisitoVisita.IDRequisito = item.ID;
                        requisitoVisita.Status = "Não Auditado";
                        requisitoVisita.SolucaoNaoConforme = "";
                        requisitoVisita.Observacao = "";
                        requisitoVisita.StatusResolucao = "Pendente";
                        requisitoVisita.ObservacaoResolucao = "";

                        hlbapp.Auditoria_Visita_Requisito.AddObject(requisitoVisita);
                    }
                }

                #endregion

                #endregion

                hlbapp.SaveChanges();

                #region Inserir LOG Requisitos da Visita

                var listaRequisitosVisita = hlbapp.Auditoria_Visita_Requisito
                    .Where(w => w.IDVisita == visita.ID).ToList();

                foreach (var item in listaRequisitosVisita)
                {
                    InsereLOGVisitaRequisito(item, "Inclusão");
                }

                #endregion

                FUNCIONARIO respObj = apolo.FUNCIONARIO.Where(w => w.FuncCod == responsavel).FirstOrDefault();

                ViewBag.Mensagem = "Visita no departamento " + departamentoApolo.FuncNome + " agendada para "
                    + dataVisita.ToShortDateString() + " para o responsável " + respObj.FuncNome + "!";
            }

            Session["ListaVisitas"] = FilterListVisitas();
            Session["metodoRetorno"] = "ListaVisitas";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult CreateVisita()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("Visita");
        }

        public ActionResult EditVisita(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            //Session["DLListaStatus"] = CarregaListaStatusVisita();

            CarregaVisita(id, "Edit");

            return View("Visita");
        }

        public ActionResult SavePreencheVisita(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["comentarioGeralVis"] != null)
            {
                #region Carrega Valores

                #region Comentário Geral da Visita

                string comentarioGeralVis = "";
                if (model["comentarioGeralVis"] != null) comentarioGeralVis = model["comentarioGeralVis"];

                #endregion

                #endregion

                #region Insere no WEB

                Auditoria_Visita visita = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    visita = new Auditoria_Visita();
                    //visita.Usuario = Session["login"].ToString().ToUpper();
                    //visita.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    visita = hlbapp.Auditoria_Visita.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                visita.ComentarioGeral = comentarioGeralVis;
                
                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.Auditoria_Visita.AddObject(visita);

                hlbapp.SaveChanges();

                #endregion

                hlbapp.SaveChanges();
            }

            Session["ListaVisitas"] = FilterListVisitas();
            Session["metodoRetorno"] = "ListaVisitas";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult ConfirmaDeleteVisita(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            CarregaVisita(id, "Edit");

            return View();
        }

        public ActionResult DeleteVisita()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            var listaRequisitosVisita = (List<Auditoria_Visita_Requisito>)Session["ListaRequisitosVisita"];

            foreach (var item in listaRequisitosVisita)
            {
                Auditoria_Visita_Requisito delRequisito = hlbapp.Auditoria_Visita_Requisito
                    .Where(w => w.ID == item.ID).FirstOrDefault();
                hlbapp.Auditoria_Visita_Requisito.DeleteObject(delRequisito);

                InsereLOGVisitaRequisito(delRequisito, "Exclusão");
            }

            hlbapp.SaveChanges();

            Auditoria_Visita visita = hlbapp.Auditoria_Visita.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.Auditoria_Visita.DeleteObject(visita);
            hlbapp.SaveChanges();

            FUNCIONARIO departamento = apolo.FUNCIONARIO.Where(w => w.FuncCod == visita.Departamento)
                .FirstOrDefault();

            ViewBag.Mensagem = "Visita " + visita.DataVisita.ToShortDateString() + " - " + departamento.FuncNome
                + " excluída com sucesso!";

            Session["ListaVisitas"] = FilterListVisitas();
            Session["metodoRetorno"] = "ListaVisitas";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult OK()
        {
            //if (Session["msg"] != null) ViewBag.Mensagem = Session["msg"];

            return View();
        }

        #endregion

        #region Event Methods

        public ActionResult AtualizaStatusRequisito(int id, string status)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Auditoria_Visita_Requisito requisito = hlbapp.Auditoria_Visita_Requisito
                .Where(w => w.ID == id).FirstOrDefault();
            requisito.Status = status;
            requisito.SolucaoNaoConforme = "";
            requisito.Observacao = "";
            hlbapp.SaveChanges();
            InsereLOGVisitaRequisito(requisito, "Alteração");

            CarregaVisita(requisito.IDVisita, "Refresh");

            Session["metodoRetorno"] = "ReturnVisita";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult ReturnVisita()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            return View("Visita");
        }

        public void CarregaRequisitoVisita(int id, string chamada)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Auditoria_Visita_Requisito requisito = hlbapp.Auditoria_Visita_Requisito.Where(w => w.ID == id).FirstOrDefault();

            Session["DDLListaNaoConforme"] = CarregaListaNaoConforme(false);
            AtualizaDDL(requisito.SolucaoNaoConforme, (List<SelectListItem>)Session["DDLListaNaoConforme"]);
            if (chamada == "Reprovado")
            {
                LOG_Auditoria_Visita_Requisito log = hlbapp.LOG_Auditoria_Visita_Requisito
                    .Where(w => w.IDVisitaRequisito == requisito.ID && w.StatusResolucao == "Reprovado")
                    .OrderByDescending(o => o.DataHora).FirstOrDefault();

                if (log != null)
                    Session["observacaoVisitaRequisito"] = log.ObservacaoResolucao;
                else
                    Session["observacaoVisitaRequisito"] = "";
            }
            else
                Session["observacaoVisitaRequisito"] = requisito.Observacao;
            Session["resolucaoRequisitoNaoConforme"] = requisito.ObservacaoResolucao;
        }

        public ActionResult RequisitoNaoConforme(int id, string chamada)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["chamadaTipoSolucao"] = chamada;

            Session["idRequisitoSelecionado"] = id;

            CarregaRequisitoVisita(id, chamada);

            return View("RequisitoVisita");
        }

        public ActionResult SaveRequisitoNaoConforme(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            #endregion

            if (model["avaliacao"] != null)
            {
                #region Carrega Valores

                #region Tipo de Solução Não Conforme

                string tipoSolucaoNaoConforme = "";
                if (model["TipoSolucaoNaoConforme"] != null) tipoSolucaoNaoConforme = model["TipoSolucaoNaoConforme"];

                #endregion

                #region Avaliação

                string avaliacao = "";
                if (model["avaliacao"] != null) avaliacao = model["avaliacao"];

                #endregion

                #endregion

                #region Salva na WEB

                int id = Convert.ToInt32(Session["idRequisitoSelecionado"]);
                Auditoria_Visita_Requisito requisito = hlbapp.Auditoria_Visita_Requisito.Where(w => w.ID == id).FirstOrDefault();
                requisito.SolucaoNaoConforme = tipoSolucaoNaoConforme;
                if (Session["chamadaTipoSolucao"].ToString() != "Reprovado")
                {
                    requisito.Status = Session["chamadaTipoSolucao"].ToString();
                    requisito.Observacao = avaliacao;
                }
                else
                {
                    requisito.StatusResolucao = Session["chamadaTipoSolucao"].ToString();
                    requisito.ObservacaoResolucao = avaliacao;
                }

                hlbapp.SaveChanges();

                InsereLOGVisitaRequisito(requisito, "Alteração");

                if (Session["chamadaTipoSolucao"].ToString() == "Reprovado")
                {
                    #region Enviar E-mail para Responsável(is) da Unidade

                    Auditoria_Visita visita = hlbapp.Auditoria_Visita
                        .Where(w => w.ID == requisito.IDVisita).FirstOrDefault();

                    string stringChar = "<br />";

                    #region Carrega lista de responsáveis da unidade e o responsável pela auditoria para o envio do e-mail

                    FUNCIONARIO unidadeApolo = apolo.FUNCIONARIO
                        .Where(w => w.FuncCod == visita.Departamento)
                        .FirstOrDefault();

                    USUARIO responsavelAuditoriaApolo = apolo.USUARIO
                        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                            && r.FuncCod == visita.Responsavel))
                        .FirstOrDefault();

                    List<USUARIO> responsaveisUnidade = apolo.USUARIO
                        .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                            && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == unidadeApolo.FuncCod
                                && g.FuncCod == r.FuncCod)))
                        .ToList();

                    if (responsaveisUnidade.Count == 0)
                    {
                        ViewBag.Erro = "Não existem responsáveis vinculados a unidade! Verifique!";
                        Session["ListaVisitas"] = FilterListVisitas();
                        return View("ListaVisitas");
                    }

                    string paraNome = responsaveisUnidade.FirstOrDefault().UsuNome;
                    string paraEmail = responsaveisUnidade.FirstOrDefault().UsuEmail;
                    string copiaPara = responsavelAuditoriaApolo.UsuEmail;

                    //string paraNome = "Paulo Alves";
                    //string paraEmail = "palves@hyline.com.br";
                    //string copiaPara = "";

                    foreach (var item in responsaveisUnidade)
                    {
                        //if (responsaveisUnidade.IndexOf(item) < (responsaveisUnidade.Count - 1))
                        //copiaPara = copiaPara + ";";
                        copiaPara = copiaPara + ";" + item.UsuEmail;
                    }

                    #endregion

                    #region Requisitos Reprovados

                    #region Carrega lista de requisitos não conformes para ir no corpo do e-mail

                    string requisitosNaoConforme = "";

                    var listaGrupoRequisitosNaoConforme = hlbapp.Auditoria_Grupo
                        .Where(g => hlbapp.Auditoria_Requisito.Any(w => w.Grupo == g.Descricao && w.TipoUnidade == g.TipoUnidade
                            && hlbapp.Auditoria_Visita_Requisito.Any(a => w.ID == a.IDRequisito
                                && a.ID == requisito.ID && a.StatusResolucao == "Reprovado")))
                        .GroupBy(b => new
                        {
                            b.Descricao,
                            b.TipoUnidade,
                            b.Ordem
                        })
                        .OrderBy(o => o.Key.Ordem)
                        .ToList();

                    if (listaGrupoRequisitosNaoConforme.Count > 0)
                        requisitosNaoConforme =
                            "<table style=\"width: 100%; "
                                + "border-collapse: collapse; "
                                + "text-align: center;\">";

                    foreach (var grupo in listaGrupoRequisitosNaoConforme)
                    {
                        requisitosNaoConforme = requisitosNaoConforme
                            + "<tr style=\"background: #333; "
                                + "color: white; "
                                + "font-weight: bold; "
                                + "text-align: center;\">"
                                + "<th colspan=\"4\">"
                                    + grupo.Key.Descricao
                                + "</th>"
                            + "</tr>";

                        var listaRequisitosNaoConforme = hlbapp.Auditoria_Visita_Requisito
                            .Where(w => w.ID == requisito.ID && w.StatusResolucao == "Reprovado"
                                && hlbapp.Auditoria_Requisito.Any(a => w.IDRequisito == a.ID
                                    && a.Grupo == grupo.Key.Descricao && a.TipoUnidade == grupo.Key.TipoUnidade))
                            .OrderBy(o => o.ID)
                            .ToList();

                        foreach (var item in listaRequisitosNaoConforme)
                        {
                            Auditoria_Requisito requisitoBD = hlbapp.Auditoria_Requisito
                                .Where(w => w.ID == item.IDRequisito).FirstOrDefault();

                            string prazo = "";
                            if (item.SolucaoNaoConforme == "Imediata")
                                prazo = item.SolucaoNaoConforme + " (5 dias)";
                            else
                                prazo = item.SolucaoNaoConforme + " (90 dias)";

                            requisitosNaoConforme = requisitosNaoConforme
                                + "<tr>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + requisitoBD.Codigo + " - " + requisitoBD.Descricao
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + "Avaliação: "
                                            + item.Observacao
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + prazo
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + "Obs. Reprovação: "
                                            + item.ObservacaoResolucao
                                    + "</td>"
                                + "</tr>";
                        }
                    }

                    if (listaGrupoRequisitosNaoConforme.Count > 0)
                        requisitosNaoConforme = requisitosNaoConforme + "</table>";

                    #endregion

                    #region Gera o E-mail

                    string assunto = "AUDITORIA - NÃO CONFORME' REPROVADO - " + visita.DataVisita.ToString("dd/MM/yy")
                        + " - " + unidadeApolo.FuncNome + " - " + visita.Nucleo;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "5";

                    //string porta = "";
                    //if (Request.Url.Port != 80)
                    //    porta = ":" + Request.Url.Port.ToString();

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "Seguem abaixo os requisitos 'Não Conforme' reprovados na visita do dia " + visita.DataVisita.ToShortDateString() + " pelo auditor "
                        + responsavelAuditoriaApolo.UsuNome + " na unidade " + unidadeApolo.FuncNome + " - " + visita.Nucleo
                        + ":" + stringChar + stringChar
                        + requisitosNaoConforme + stringChar + stringChar
                        //+ "Clique no link a seguir para poder realizar a aprovação: "
                        //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                        + "Por favor, reavaliar a solução da não conformidade e informar no sistema após a conclusão!"
                        + stringChar + stringChar
                        + "SISTEMA WEB";

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

                    #endregion

                    #endregion

                    #endregion
                }

                #endregion

                #region Atualiza Dados Visita

                CarregaVisita(requisito.IDVisita, "Refresh");

                #endregion
            }

            Session["metodoRetorno"] = "ReturnVisita";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult EnviaRequisitosNaoConforme(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            Auditoria_Visita visita = hlbapp.Auditoria_Visita
                .Where(w => w.ID == id).FirstOrDefault();

            List<Auditoria_Visita_Requisito> listaRequisitos = hlbapp.Auditoria_Visita_Requisito
                .Where(w => w.IDVisita == id).ToList();

            foreach (var item in listaRequisitos)
            {
                item.StatusResolucao = "Enviado";
                item.UsuarioEnvio = Session["login"].ToString().ToUpper();
                item.DataHoraEnvio = DateTime.Now;

                InsereLOGVisitaRequisito(item, "Alteração");
            }

            #region Enviar E-mail para Responsável(is) da Unidade

            //string stringChar = "" + (char)13 + (char)10;
            string stringChar = "<br />";

            #region Carrega lista de responsáveis da unidade e o responsável pela auditoria para o envio do e-mail

            FUNCIONARIO unidadeApolo = apolo.FUNCIONARIO
                .Where(w => w.FuncCod == visita.Departamento)
                .FirstOrDefault();

            USUARIO responsavelAuditoriaApolo = apolo.USUARIO
                .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                    && r.FuncCod == visita.Responsavel))
                .FirstOrDefault();

            List<USUARIO> responsaveisUnidade = apolo.USUARIO
                .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                    && apolo.GRP_FUNC.Any(g => g.GrpFuncCod == unidadeApolo.FuncCod
                        && g.FuncCod == r.FuncCod
                        && g.GrpFuncObs.Contains("Responsável Auditoria")
                        )))
                .ToList();

            if (responsaveisUnidade.Count == 0)
            {
                ViewBag.Erro = "Não existem responsáveis vinculados a unidade! Verifique!";
                Session["ListaVisitas"] = FilterListVisitas();
                return View("ListaVisitas");
            }

            string paraNome = responsaveisUnidade.FirstOrDefault().UsuNome;
            string paraEmail = responsaveisUnidade.FirstOrDefault().UsuEmail;
            string copiaPara = responsavelAuditoriaApolo.UsuEmail;

            //string paraNome = "Paulo Alves";
            //string paraEmail = "palves@hyline.com.br";
            //string copiaPara = "";

            foreach (var item in responsaveisUnidade.Where(w => w.UsuNome != paraNome).ToList())
            {
                //if (responsaveisUnidade.IndexOf(item) < (responsaveisUnidade.Count - 1))
                //copiaPara = copiaPara + ";";
                copiaPara = copiaPara + ";" + item.UsuEmail;
            }

            #endregion

            #region Requisitos Não Conforme - Ação Imediata

            #region Carrega lista de requisitos não conformes para ir no corpo do e-mail

            string requisitosNaoConforme = "";

            var listaGrupoRequisitosNaoConforme = hlbapp.Auditoria_Grupo
                .Where(g => hlbapp.Auditoria_Requisito.Any(w => w.Grupo ==g.Descricao && w.TipoUnidade == g.TipoUnidade
                    && hlbapp.Auditoria_Visita_Requisito.Any(a => w.ID == a.IDRequisito
                        && a.IDVisita == id && a.Status == "Não Conforme" && a.SolucaoNaoConforme == "Imediata")))
                .GroupBy(b => new 
                {
                    b.Descricao,
                    b.TipoUnidade,
                    b.Ordem
                })
                .OrderBy(o => o.Key.Ordem)
                .ToList();

            if (listaGrupoRequisitosNaoConforme.Count > 0)
                requisitosNaoConforme = 
                    "<table style=\"width: 100%; " 
                        + "border-collapse: collapse; " 
                        + "text-align: center;\">";

            foreach (var grupo in listaGrupoRequisitosNaoConforme)
            {
                requisitosNaoConforme = requisitosNaoConforme 
                    + "<tr style=\"background: #333; "
		                + "color: white; "
		                + "font-weight: bold; "
		                + "text-align: center;\">" 
                        + "<th colspan=\"3\">"
                            + grupo.Key.Descricao
                        + "</th>"
                    + "</tr>";

                var listaRequisitosNaoConforme = hlbapp.Auditoria_Visita_Requisito
                    .Where(w => w.IDVisita == id && w.Status == "Não Conforme" && w.SolucaoNaoConforme == "Imediata"
                        && hlbapp.Auditoria_Requisito.Any(a => w.IDRequisito == a.ID
                            && a.Grupo == grupo.Key.Descricao && a.TipoUnidade == grupo.Key.TipoUnidade))
                    .OrderBy(o => o.ID)
                    .ToList();

                foreach (var item in listaRequisitosNaoConforme)
                {
                    Auditoria_Requisito requisito = hlbapp.Auditoria_Requisito
                        .Where(w => w.ID == item.IDRequisito).FirstOrDefault();

                    string prazo = "";
                    if (item.SolucaoNaoConforme == "Imediata")
                        prazo = item.SolucaoNaoConforme + " (5 dias)";
                    else
                        prazo = item.SolucaoNaoConforme + " (90 dias)";

                    requisitosNaoConforme = requisitosNaoConforme
                        + "<tr>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + requisito.Codigo + " - " + requisito.Descricao 
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + "Avaliação: "
                                    + item.Observacao
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + prazo
                            + "</td>"
                        + "</tr>";
                }
            }

            if (listaGrupoRequisitosNaoConforme.Count > 0)
                requisitosNaoConforme = requisitosNaoConforme + "</table>";

            #endregion

            #region Gera o E-mail

            string assunto = "AUDITORIA - 'NÃO CONFORME' IMEDIATO - " + visita.DataVisita.ToString("dd/MM/yy")
                + " - " + unidadeApolo.FuncNome + " - " + visita.Nucleo;
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "5";

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                + "Seguem abaixo os requisitos 'Não Conforme' avaliados na visita do dia " + visita.DataVisita.ToShortDateString() + " pelo auditor "
                + responsavelAuditoriaApolo.UsuNome + " na unidade " + unidadeApolo.FuncNome + " - " + visita.Nucleo
                + ":" + stringChar + stringChar
                + requisitosNaoConforme + stringChar + stringChar
                //+ "Clique no link a seguir para poder realizar a aprovação: "
                //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                + "Por favor, solucionar a não conformidade e informar no sistema após a conclusão!"
                + stringChar + stringChar
                + "SISTEMA WEB";

            EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

            #endregion

            #endregion

            #region Requisitos Não Conforme - Ação Programada

            #region Carrega lista de requisitos não conformes para ir no corpo do e-mail

            requisitosNaoConforme = "";

            listaGrupoRequisitosNaoConforme = hlbapp.Auditoria_Grupo
                .Where(g => hlbapp.Auditoria_Requisito.Any(w => w.Grupo == g.Descricao && w.TipoUnidade == g.TipoUnidade
                    && hlbapp.Auditoria_Visita_Requisito.Any(a => w.ID == a.IDRequisito
                        && a.IDVisita == id && a.Status == "Não Conforme" && a.SolucaoNaoConforme == "Programada")))
                .GroupBy(b => new
                {
                    b.Descricao,
                    b.TipoUnidade,
                    b.Ordem
                })
                .OrderBy(o => o.Key.Ordem)
                .ToList();

            if (listaGrupoRequisitosNaoConforme.Count > 0)
                requisitosNaoConforme =
                    "<table style=\"width: 100%; "
                        + "border-collapse: collapse; "
                        + "text-align: center;\">";

            foreach (var grupo in listaGrupoRequisitosNaoConforme)
            {
                requisitosNaoConforme = requisitosNaoConforme
                    + "<tr style=\"background: #333; "
                        + "color: white; "
                        + "font-weight: bold; "
                        + "text-align: center;\">"
                        + "<th colspan=\"3\">"
                            + grupo.Key.Descricao
                        + "</th>"
                    + "</tr>";

                var listaRequisitosNaoConforme = hlbapp.Auditoria_Visita_Requisito
                    .Where(w => w.IDVisita == id && w.Status == "Não Conforme" && w.SolucaoNaoConforme == "Programada"
                        && hlbapp.Auditoria_Requisito.Any(a => w.IDRequisito == a.ID
                            && a.Grupo == grupo.Key.Descricao && a.TipoUnidade == grupo.Key.TipoUnidade))
                    .OrderBy(o => o.ID)
                    .ToList();

                foreach (var item in listaRequisitosNaoConforme)
                {
                    Auditoria_Requisito requisito = hlbapp.Auditoria_Requisito
                        .Where(w => w.ID == item.IDRequisito).FirstOrDefault();

                    string prazo = "";
                    if (item.SolucaoNaoConforme == "Imediata")
                        prazo = item.SolucaoNaoConforme + " (5 dias)";
                    else
                        prazo = item.SolucaoNaoConforme + " (90 dias)";

                    requisitosNaoConforme = requisitosNaoConforme
                        + "<tr>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + requisito.Codigo + " - " + requisito.Descricao
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + "Avaliação: "
                                    + item.Observacao
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + prazo
                            + "</td>"
                        + "</tr>";
                }
            }

            if (listaGrupoRequisitosNaoConforme.Count > 0)
                requisitosNaoConforme = requisitosNaoConforme + "</table>";

            #endregion

            #region Gera o E-mail

            assunto = "AUDITORIA - 'NÃO CONFORME' PROGRAMADO - " + visita.DataVisita.ToString("dd/MM/yy")
                + " - " + unidadeApolo.FuncNome + " - " + visita.Nucleo;
            corpoEmail = "";
            anexos = "";
            empresaApolo = "5";

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                + "Seguem abaixo os requisitos 'Não Conforme' avaliados na visita do dia " + visita.DataVisita.ToShortDateString() + " pelo auditor "
                + responsavelAuditoriaApolo.UsuNome + " na unidade " + unidadeApolo.FuncNome + " - " + visita.Nucleo
                + ":" + stringChar + stringChar
                + requisitosNaoConforme + stringChar + stringChar
                //+ "Clique no link a seguir para poder realizar a aprovação: "
                //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                + "Por favor, solucionar a não conformidade e informar no sistema após a conclusão!"
                + stringChar + stringChar
                + "SISTEMA WEB";

            EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Html");

            #endregion

            #endregion

            #endregion

            hlbapp.SaveChanges();

            Session["ListaVisitas"] = FilterListVisitas();
            Session["metodoRetorno"] = "ListaVisitas";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult AprovarResolucaoRequisito(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            #region Salva na WEB

            Auditoria_Visita_Requisito requisito = hlbapp.Auditoria_Visita_Requisito.Where(w => w.ID == id).FirstOrDefault();
            requisito.UsuarioAprovacao = Session["login"].ToString().ToUpper();
            requisito.DataHoraAprovacao = DateTime.Now;
            requisito.StatusResolucao = "Aprovado";

            InsereLOGVisitaRequisito(requisito, "Alteração");

            hlbapp.SaveChanges();

            #endregion

            #region Atualiza Dados Visita

            CarregaVisita(requisito.IDVisita, "Refresh");

            #endregion

            Session["metodoRetorno"] = "ReturnVisita";
            return RedirectToAction("OK", "Auditoria");
        }

        public ActionResult HistoricoRequisito(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            Session["ListaLOGRequisitosVisita"] = hlbapp.LOG_Auditoria_Visita_Requisito
                .Where(w => w.IDVisitaRequisito == id).ToList();
            
            return View();
        }

        #endregion

        #region Relatórios Excel

        #region Relatório de Visitas - Geral

        public ActionResult GerarRelatorioVisitasGeral()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialVisita"]);
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalVisita"]);
            string departamento = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Value;

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\Auditoria";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Auditoria\\Relatorio_Visitas_Geral_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_Visitas_Geral_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioVisitasGeralExcel(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, departamento);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Visitas_Geral_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioVisitasGeralExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string departamento)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Auditoria\\Relatorio_Visitas_Geral.xlsx", destino);

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Dados das Visitas"];

            string descDepto = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Text;

            worksheet.Cells[5, 2] = "Departamento: " + descDepto;

            #region SQL Exibição

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Rel_Auditoria V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "V.[Data Visita] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "(V.CodigoDepartamento = '" + departamento + "' or '" + departamento + "' = '(Todos)') ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "2, 3, 4, 6";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Visitas"))
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

        #region Requisitos Não Conforme

        #region List Methods

        public List<Auditoria_Visita_Requisito> ListRequisitosNaoConforme(string departamento, DateTime dataInicial, DateTime dataFinal)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            Apolo10Entities apolo = new Apolo10Entities();

            string login = Session["login"].ToString().ToUpper();

            FUNCIONARIO usuario = apolo.FUNCIONARIO
                .Where(w => w.UsuCod == login)
                .FirstOrDefault();

            var lista = hlbapp.Auditoria_Visita
                .Where(w => (w.Departamento == departamento || departamento == "(Todos)")
                    && w.DataVisita >= dataInicial && w.DataVisita <= dataFinal
                    && hlbapp.Auditoria_Visita_Requisito
                        .Any(a => a.IDVisita == w.ID && a.Status == "Não Conforme"
                            && (a.StatusResolucao == "Enviado" || a.StatusResolucao == "Reprovado")
                            ))
                .OrderBy(o => o.DataVisita)
                .ToList();

            List<Auditoria_Visita_Requisito> listaFiltrada = new List<Auditoria_Visita_Requisito>();

            foreach (var item in lista)
            {
                int existeFuncionarioXUsuario = apolo.GRP_FUNC
                    .Where(w => w.GrpFuncCod == item.Departamento
                        && w.FuncCod == usuario.FuncCod
                        && w.GrpFuncObs.Contains("Responsável Auditoria"))
                    .Count();

                if (existeFuncionarioXUsuario > 0)
                {
                    var listaRequisitos = hlbapp.Auditoria_Visita_Requisito
                        .Where(w => w.IDVisita == item.ID
                            && w.Status == "Não Conforme"
                            && (w.StatusResolucao == "Enviado" || w.StatusResolucao == "Reprovado")
                            )
                        .OrderBy(o => o.IDRequisito)
                        .ToList();

                    foreach (var requisito in listaRequisitos)
                    {
                        listaFiltrada.Add(requisito);
                    }
                }
            }

            return listaFiltrada;
        }

        public List<Auditoria_Visita_Requisito> FilterListRequisitosNaoConforme()
        {
            CleanSessions();

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialVisita"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalVisita"].ToString());
            string departamento = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;

            return ListRequisitosNaoConforme(departamento, dataInicial, dataFinal);
        }

        #endregion

        #region Lista Requisitos Não Conforme

        public ActionResult ListaRequisitosNaoConforme()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            if (Session["FiltroDDLListaDepartamentos"] == null)
                Session["FiltroDDLListaDepartamentos"] = CarregaListaDepartamento(true);

            Session["msg"] = "";

            Session["ListaRequisitosNaoConforme"] = FilterListRequisitosNaoConforme();
            return View();
        }

        public ActionResult SearchRequisitosNaoConforme(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            DateTime dataInicial = new DateTime();
            if (model["dataInicialVisita"] != null)
            {
                dataInicial = Convert.ToDateTime(model["dataInicialVisita"]);
                Session["dataInicialVisita"] = dataInicial.ToShortDateString();
            }
            else
                dataInicial = Convert.ToDateTime(Session["dataInicialVisita"].ToString());

            DateTime dataFinal = new DateTime();
            if (model["dataFinalVisita"] != null)
            {
                dataFinal = Convert.ToDateTime(model["dataFinalVisita"]);
                Session["dataFinalVisita"] = dataFinal.ToShortDateString();
            }
            else
                dataFinal = Convert.ToDateTime(Session["dataFinalVisita"].ToString());

            if (model["Departamento"] != null)
                AtualizaDDL(model["Departamento"], (List<SelectListItem>)Session["FiltroDDLListaDepartamentos"]);

            #endregion

            Session["ListaRequisitosNaoConforme"] = ListRequisitosNaoConforme(model["Departamento"], dataInicial, dataFinal);
            return View("ListaRequisitosNaoConforme");
        }

        #endregion

        #region Event Methods

        public ActionResult ResolucaoNaoConforme(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idRequisitoSelecionado"] = id;

            CarregaRequisitoVisita(id, "");

            return View("ResolucaoNaoConforme");
        }

        public ActionResult ReturnListaRequisitosNaoConforme()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");
            return View("ListaRequisitosNaoConforme");
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

                int id = Convert.ToInt32(Session["idRequisitoSelecionado"]);
                Auditoria_Visita_Requisito requisito = hlbapp.Auditoria_Visita_Requisito.Where(w => w.ID == id).FirstOrDefault();
                requisito.ObservacaoResolucao = resolucao;
                requisito.UsuarioResolucao = login;
                requisito.DataHoraResolucao = DateTime.Now;
                requisito.StatusResolucao = "Resolvido";

                hlbapp.SaveChanges();

                InsereLOGVisitaRequisito(requisito, "Alteração");

                #endregion

                #region Envia E-mail para Auditor avisando da inserção da resolução

                #region Gera o E-mail

                #region Carrega Dados

                string stringChar = "" + (char)13 + (char)10;

                Auditoria_Visita visita = hlbapp.Auditoria_Visita
                    .Where(w => w.ID == requisito.IDVisita).FirstOrDefault();

                Auditoria_Requisito req = hlbapp.Auditoria_Requisito
                    .Where(w => w.ID == requisito.IDRequisito).FirstOrDefault();

                USUARIO responsavelAuditoriaApolo = apolo.USUARIO
                    .Where(u => apolo.FUNCIONARIO.Any(r => r.UsuCod == u.UsuCod
                        && r.FuncCod == visita.Responsavel))
                    .FirstOrDefault();

                FUNCIONARIO unidade = apolo.FUNCIONARIO
                    .Where(w => w.FuncCod == visita.Departamento).FirstOrDefault();

                USUARIO usuarioResolvido = apolo.USUARIO
                    .Where(w => w.UsuCod == login).FirstOrDefault();

                string paraNome = responsavelAuditoriaApolo.UsuNome;
                string paraEmail = responsavelAuditoriaApolo.UsuEmail;
                string copiaPara = "";

                //string paraNome = "Paulo Alves";
                //string paraEmail = "palves@hyline.com.br";
                //string copiaPara = "";

                #endregion

                string assunto = "AUDITORIA - REQUISITO 'NÃO CONFORME' RESOLVIDO - " + req.Codigo + " - " 
                    + visita.DataVisita.ToShortDateString() + " - "
                    + unidade.FuncNome;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                    + "O requisito \"Não Conforme\" \"" + req.Codigo + " - " + req.Descricao
                    + "\" avaliado na visita do dia " + visita.DataVisita.ToShortDateString()
                    + " na unidade " + unidade.FuncNome
                    + " foi resolvido pelo usuário " + usuarioResolvido.UsuNome + " em " 
                    + Convert.ToDateTime(requisito.DataHoraResolucao).ToShortDateString()
                    + " às " + Convert.ToDateTime(requisito.DataHoraResolucao).ToString("HH:mm")
                    + "." + stringChar + stringChar
                    + "Segue resolução: " + requisito.ObservacaoResolucao + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Por favor, verificar e realizar a aprovação!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo, "Texto");

                #endregion

                #endregion
            }

            Session["metodoRetorno"] = "ListaRequisitosNaoConforme";
            return RedirectToAction("OK", "Auditoria");
        }

        #endregion

        #region Relatórios Excel

        #region Relatório de Visitas - Geral

        public ActionResult GerarRelatorioRequisitosNaoConforme()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            #region Tratamento de Parâmetros

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicialVisita"]);
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinalVisita"]);
            string departamento = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Value;

            #endregion

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\Auditoria";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Auditoria\\Relatorio_Nao_Conformidade_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_Nao_Conformidade_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioRequisitosNaoConforme(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, departamento);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Relatorio_Nao_Conformidade_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioRequisitosNaoConforme(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal, string departamento)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Auditoria\\Relatorio_Nao_Conformidade.xlsx", destino);

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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Req. Não Conforme"];

            string descDepto = ((List<SelectListItem>)Session["FiltroDDLListaDepartamentos"])
                .Where(w => w.Selected == true)
                .FirstOrDefault().Text;

            worksheet.Cells[6, 2] = "Departamento: " + descDepto;

            #region SQL Exibição

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Rel_Nao_Conformidade V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "V.[Data] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    "(V.Departamento = '" + departamento + "' or '" + departamento + "' = '(Todos)') ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "2, 3, 4, 6";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Rel_Nao_Conformidade"))
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

            string login = Session["login"].ToString().ToUpper();

            FUNCIONARIO usuario = apolo.FUNCIONARIO
                .Where(w => w.UsuCod == login)
                .FirstOrDefault();

            var listaDepartamentos = apolo.FUNCIONARIO
                .Where(w => w.USERUnidadeSistAuditoria == "Sim")
                .OrderBy(o => o.FuncNome)
                .ToList();

            foreach (var item in listaDepartamentos)
            {
                int existeFuncionarioXUsuario = apolo.GRP_FUNC
                    .Where(w => w.FuncCod == item.FuncCod
                        && w.GrpFuncCod == usuario.FuncCod)
                    .Count();

                if (existeFuncionarioXUsuario > 0)
                {
                    listaDepartamento.Add(new SelectListItem
                    {
                        Text = item.FuncNome,
                        Value = item.FuncCod,
                        Selected = false
                    });
                }
            }

            return listaDepartamento;
        }

        public List<SelectListItem> CarregaListaResponsaveis()
        {
            List<SelectListItem> ddlResponsaveis = new List<SelectListItem>();

            Apolo10Entities apolo = new Apolo10Entities();

            var listaResponsaveis = apolo.FUNCIONARIO
                .Where(w => w.USERResponsavelAuditoria == "Sim")
                .OrderBy(o => o.FuncNome)
                .ToList();

            foreach (var item in listaResponsaveis)
            {
                ddlResponsaveis.Add(new SelectListItem
                {
                    Text = item.FuncNome,
                    Value = item.FuncCod,
                    Selected = false
                });
            }

            return ddlResponsaveis;
        }

        public List<SelectListItem> CarregaListaStatusVisita(bool todos)
        {
            List<SelectListItem> ddlListaStatusAcao = new List<SelectListItem>();

            if (todos)
            {
                ddlListaStatusAcao.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Conforme",
                Value = "Conforme",
                Selected = false
            });

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Não Conforme",
                Value = "Não Conforme",
                Selected = false
            });

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Não Aplicável",
                Value = "Não Aplicável",
                Selected = false
            });

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Não Auditado",
                Value = "Não Auditado",
                Selected = false
            });

            return ddlListaStatusAcao;
        }

        public List<SelectListItem> CarregaListaUnidades(bool todas)
        {
            MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities bdApolo =
                new MvcAppHylinedoBrasilMobile.Models.bdApolo.bdApoloEntities();
            ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService = 
                new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();

            List<SelectListItem> items = new List<SelectListItem>();

            if (todas)
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
                    && (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
                .SelectMany(
                    x => x.EMP_FILIAL_CERTIFICACAO.DefaultIfEmpty(),
                    (x, y) => new { EMPRESA_FILIAL = x, EMP_FILIAL_CERTIFICACAO = y })
                .OrderBy(f => f.EMPRESA_FILIAL.EmpNome)
                .ToList();

            foreach (var item in listaFiliais)
            {
                bool selected = false;
                if ((listaFiliais.IndexOf(item).Equals(0)) && (Session["unidadeSelecionada"] == null))
                {
                    selected = true;
                    Session["unidadeSelecionada"] = item.EMPRESA_FILIAL.USERFLIPCod;
                }
                string codFLIP = "";
                if (item.EMP_FILIAL_CERTIFICACAO == null)
                    codFLIP = item.EMPRESA_FILIAL.USERFLIPCod;
                else
                    codFLIP = item.EMP_FILIAL_CERTIFICACAO.EmpFilCertificNum;

                items.Add(new SelectListItem
                {
                    Text = codFLIP + " - " + item.EMPRESA_FILIAL.EmpNome,
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
            }

            return items;
        }

        public List<SelectListItem> CarregaListaTiposUnidade(bool todos)
        {
            List<SelectListItem> ddlListaTiposUnidade = new List<SelectListItem>();

            ImportaIncubacao.Data.Apolo.Apolo10EntitiesService apoloService =
                new ImportaIncubacao.Data.Apolo.Apolo10EntitiesService();

            ImportaIncubacao.Data.Apolo.CRIA_CAMPO criaCampo = apoloService.CRIA_CAMPO
                .Where(c => c.TabSistCod == "FUNCIONARIO" && c.CriaCampoNome == "USERTipoUnidade")
                .FirstOrDefault();

            if (todos)
                ddlListaTiposUnidade.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });

            var listaTipoUnidade = criaCampo.CriaCampoItem.Replace("\n", "").Split((char)13);

            foreach (var item in listaTipoUnidade)
            {
                ddlListaTiposUnidade.Add(new SelectListItem
                {
                    Text = item,
                    Value = item,
                    Selected = false
                });
            }

            return ddlListaTiposUnidade;
        }

        public List<SelectListItem> CarregaListaNucleos(string unidade)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            Apolo10Entities apolo = new Apolo10Entities();

            FLOCKSFarmsTableAdapter fTA = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable fDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            fTA.FillFarms(fDT);

            FUNCIONARIO unidadeObj = apolo.FUNCIONARIO.Where(w => w.FuncCod == unidade).FirstOrDefault();

            if (unidadeObj != null)
            {
                if (unidadeObj.USERCodigoFLIP != "" && unidadeObj.USERCodigoFLIP != null)
                {
                    var listaUnidadesFLIP = unidadeObj.USERCodigoFLIP.Split(';');

                    foreach (var item in listaUnidadesFLIP)
                    {
                        foreach (var nucleo in fDT.Where(f => f.FARM_ID.StartsWith(item)).ToList())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = nucleo.FARM_ID,
                                Value = nucleo.FARM_ID,
                                Selected = false
                            });
                        }
                    }
                }
            }

            return items;
        }

        public List<SelectListItem> CarregaListaNaoConforme(bool todos)
        {
            List<SelectListItem> ddlListaNaoConforme = new List<SelectListItem>();

            if (todos)
            {
                ddlListaNaoConforme.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            ddlListaNaoConforme.Add(new SelectListItem
            {
                Text = "Imediata",
                Value = "Imediata",
                Selected = false
            });

            ddlListaNaoConforme.Add(new SelectListItem
            {
                Text = "Programada",
                Value = "Programada",
                Selected = false
            });

            return ddlListaNaoConforme;
        }

        public List<SelectListItem> CarregaListaGrupoRequisito(bool todos,string tipoUnidade)
        {
            List<SelectListItem> listaGrupoRequisito = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (todos)
            {
                listaGrupoRequisito.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaGrupoRequisitoDB = hlbapp.Auditoria_Grupo
                .Where(w => (w.TipoUnidade == tipoUnidade || tipoUnidade == ""))
                .OrderBy(w => w.ID).ToList();

            foreach (var item in listaGrupoRequisitoDB)
            {
                listaGrupoRequisito.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.Descricao,
                    Selected = false
                });
            }

            return listaGrupoRequisito;
        }

        #endregion

        #region Json Methods

        [HttpPost]
        public ActionResult CarregaListaNucleosJS(string unidade)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = CarregaListaNucleos(unidade);
            
            Session["ListaNucleos"] = items;

            return Json(items);
        }

        [HttpPost]
        public ActionResult CarregaListaGruposRequisitoJS(string unidade)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items = CarregaListaGrupoRequisito(false, unidade);

            Session["ListaGruposRequisitos"] = items;

            return Json(items);
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region Geral

            Session["idSelecionado"] = 0;
            if (Session["pesquisaSession"] == null) Session["pesquisaSession"] = "";
            Session["unidadeSelecionada"] = "";

            #endregion

            #region Requisitos

            Session["DLListaTiposUnidade"] = CarregaListaTiposUnidade(false);
            Session["ListaGruposRequisitos"] = new List<SelectListItem>();
            Session["grupoRequisito"] = "";
            Session["codigoRequisito"] = "";
            Session["descricaoRequisito"] = "";

            #endregion

            #region Visitas

            Session["verificaRequisitosPendentes"] = "";
            Session["dataVisita"] = DateTime.Today;
            Session["DLListaDepartamentos"] = CarregaListaDepartamento(false);
            Session["DLListaResponsaveis"] = CarregaListaResponsaveis();
            Session["ListaRequisitosVisita"] = new List<Auditoria_Visita_Requisito>();
            Session["comentariosGeralVisita"] = "";

            if (Session["dataInicialVisita"] == null) Session["dataInicialVisita"] = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (Session["dataFinalVisita"] == null) Session["dataFinalVisita"] = new DateTime(DateTime.Today.Year,
                DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));

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

        public void InsereLOGVisitaRequisito(Auditoria_Visita_Requisito visitaRequisito, string operacao)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            LOG_Auditoria_Visita_Requisito log = new LOG_Auditoria_Visita_Requisito();

            log.Operacao = operacao;
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.IDVisitaRequisito = visitaRequisito.ID;
            log.IDVisita = visitaRequisito.IDVisita;
            log.IDRequisito = visitaRequisito.IDRequisito;
            log.Status = visitaRequisito.Status;
            log.SolucaoNaoConforme = visitaRequisito.SolucaoNaoConforme;
            log.Observacao = visitaRequisito.Observacao;
            log.ObservacaoResolucao = visitaRequisito.ObservacaoResolucao;
            log.StatusResolucao = visitaRequisito.StatusResolucao;

            hlbapp.LOG_Auditoria_Visita_Requisito.AddObject(log);

            hlbapp.SaveChanges();
        }

        #endregion
    }
}