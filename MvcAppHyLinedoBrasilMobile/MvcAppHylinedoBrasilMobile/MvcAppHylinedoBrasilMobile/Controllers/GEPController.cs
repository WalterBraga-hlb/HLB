using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using System.Globalization;
using System.Timers;
using System.Data.Objects;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class GEPController : Controller
    {
        #region Timer

        private static Timer _oTimerHora;

        public static void IniciaTimer()
        {
            // Hora
            _oTimerHora = new Timer(3600 * 1000);
            //_oTimerHora = new Timer(60 * 1000); // Teste Minuto
            _oTimerHora.Elapsed += Atualizacao_Tick;
            _oTimerHora.Start();
        }

        #endregion

        #region Menus

        public ActionResult MenuGEP()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                DateTime.Today,
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            DateTime data = FirstDateOfWeekISO8601(DateTime.Today.Year, semanaAno);

            Session["FiltroListaPilares"] = CarregaListaPilares(true);

            return View();
        }

        #endregion

        #region Cadastros

        #region Pilares

        #region List Methods

        public List<GEP_Pilar> ListPilares(string pesquisa)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaPilares = hlbapp.GEP_Pilar
                .Where(w => (w.Descricao.Contains(pesquisa) || pesquisa == ""))
                .OrderBy(o => o.Descricao)
                .ToList();

            return listaPilares;
        }

        public List<GEP_Pilar> FilterListPilares()
        {
            CleanSessions();

            return ListPilares(Session["pesquisaSession"].ToString());
        }

        #endregion

        #region Lista Pilares

        public ActionResult ListaPilares()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["ListaPilares"] = FilterListPilares();

            return View();
        }

        public ActionResult SearchPilar(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["pesquisa"] != null)
            {
                Session["pesquisaSession"] = model["pesquisa"];
            }

            #endregion

            Session["ListaPilares"] = ListPilares(Session["pesquisaSession"].ToString());

            return View("ListaPilares");
        }

        #endregion

        #region CRUD Methods

        public void CarregaPilar(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Pilar pilar = hlbapp.GEP_Pilar.Where(w => w.ID == id).FirstOrDefault();

            Session["descricaoPilar"] = pilar.Descricao;
        }

        public ActionResult CreatePilar()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("Pilar");
        }

        public ActionResult EditPilar(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaPilar(id);

            return View("Pilar");
        }

        public ActionResult SavePilar(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["descricao"] != null)
            {
                #region Carrega Valores

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null) descricao = model["descricao"];

                #endregion

                #endregion

                #region Insere no WEB

                GEP_Pilar pilar = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    pilar = new GEP_Pilar();
                    pilar.Usuario = Session["login"].ToString().ToUpper();
                    pilar.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    pilar = hlbapp.GEP_Pilar.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                pilar.Descricao = descricao;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.GEP_Pilar.AddObject(pilar);

                #endregion

                hlbapp.SaveChanges();
            }

            Session["ListaPilares"] = FilterListPilares();
            return View("ListaPilares");
        }

        public ActionResult ConfirmaDeletePilar(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Pilar pilar = hlbapp.GEP_Pilar.Where(w => w.ID == id).FirstOrDefault();

            int existe = hlbapp.GEP_Objetivo.Where(w => w.IDPilar == id).Count();

            if (existe > 0)
            {
                ViewBag.Erro = "Não é possível excluir o Pilar " + pilar.Descricao
                    + " pois existem Objetivos relacionados!";
                Session["ListaPilares"] = FilterListPilares();
                return View("ListaPilares");
            }

            return View();
        }

        public ActionResult DeletePilar()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            GEP_Pilar pilar = hlbapp.GEP_Pilar.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.GEP_Pilar.DeleteObject(pilar);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Pilar " + pilar.Descricao + " excluído com sucesso!";

            Session["ListaPilares"] = FilterListPilares();
            return View("ListaPilares");
        }

        #endregion

        #endregion

        #region Objetivos

        #region List Methods

        public List<GEP_Objetivo> ListObjetivos(string pesquisa, string pilar)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int idPilar = 0;
            if (pilar != "(Todos)") idPilar = Convert.ToInt32(pilar);

            var listaObjetivos = hlbapp.GEP_Objetivo
                .Where(w => (w.Descricao.Contains(pesquisa) || pesquisa == "")
                    && (w.IDPilar == idPilar || idPilar == 0))
                .OrderBy(o => o.Descricao)
                .ToList();

            return listaObjetivos;
        }

        public List<GEP_Objetivo> FilterListObjetivos()
        {
            CleanSessions();

            string pesquisa = Session["pesquisaSession"].ToString();
            string pilar = ((List<SelectListItem>)Session["FiltroDDLListaPilares"])
                .Where(w => w.Selected == true).FirstOrDefault().Value;

            return ListObjetivos(pesquisa, pilar);
        }

        #endregion

        #region Lista Objetivos

        public ActionResult ListaObjetivos()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["FiltroDDLListaPilares"] = CarregaListaPilares(true);
            Session["ListaObjetivos"] = FilterListObjetivos();

            return View();
        }

        public ActionResult SearchObjetivo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["pesquisa"] != null)
                Session["pesquisaSession"] = model["pesquisa"];

            if (model["Pilar"] != null)
                AtualizaDDL(model["Pilar"], (List<SelectListItem>)Session["FiltroDDLListaPilares"]);

            #endregion

            Session["ListaObjetivos"] = ListObjetivos(model["pesquisa"], model["Pilar"]);

            return View("ListaObjetivos");
        }

        #endregion

        #region CRUD Methods

        public void CarregaObjetivo(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Objetivo objetivo = hlbapp.GEP_Objetivo.Where(w => w.ID == id).FirstOrDefault();

            Session["descricaoObjetivo"] = objetivo.Descricao;
            AtualizaDDL(objetivo.IDPilar.ToString(), (List<SelectListItem>)Session["DLListaPilares"]);
        }

        public ActionResult CreateObjetivo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("Objetivo");
        }

        public ActionResult EditObjetivo(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaObjetivo(id);

            return View("Objetivo");
        }

        public ActionResult SaveObjetivo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["descricao"] != null)
            {
                #region Carrega Valores

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null) descricao = model["descricao"];

                #endregion

                #region Pilar

                string pilar = "";
                if (model["Pilar"] != null) pilar = model["Pilar"];

                #endregion

                #endregion

                #region Insere no WEB

                GEP_Objetivo objetivo = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    objetivo = new GEP_Objetivo();
                    objetivo.Usuario = Session["login"].ToString().ToUpper();
                    objetivo.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    objetivo = hlbapp.GEP_Objetivo.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                objetivo.Descricao = descricao;
                objetivo.IDPilar = Convert.ToInt32(pilar);

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.GEP_Objetivo.AddObject(objetivo);

                #endregion

                hlbapp.SaveChanges();
            }

            Session["ListaObjetivos"] = FilterListObjetivos();
            return View("ListaObjetivos");
        }

        public ActionResult ConfirmaDeleteObjetivo(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Objetivo objetivo = hlbapp.GEP_Objetivo.Where(w => w.ID == id).FirstOrDefault();

            int existe = hlbapp.GEP_Acao.Where(w => w.Objetivo == objetivo.Descricao).Count();

            if (existe > 0)
            {
                ViewBag.Erro = "Não é possível excluir o Objetivo \"" + objetivo.Descricao
                    + "\" pois existem Ações relacionadas!";
                Session["ListaObjetivos"] = FilterListObjetivos();
                return View("ListaObjetivos");
            }

            return View();
        }

        public ActionResult DeleteObjetivo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            GEP_Objetivo objetivo = hlbapp.GEP_Objetivo.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.GEP_Objetivo.DeleteObject(objetivo);
            hlbapp.SaveChanges();

            ViewBag.Mensagem = "Objetivo " + objetivo.Descricao + " excluído com sucesso!";

            Session["ListaObjetivos"] = FilterListObjetivos();
            return View("ListaObjetivos");
        }

        #endregion

        #endregion

        #endregion

        #region Manutenção

        #region Ações

        #region List Methods

        public List<GEP_Acao> ListAcoes(string pesquisa, string pilar, string objetivo, 
            string semanaInicialStr, string semanaFinalStr)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int semanaInicial = Convert.ToInt32(semanaInicialStr.Substring(6,2));
            int anoInicial = Convert.ToInt32(semanaInicialStr.Substring(0,4));
            int semanaFinal = Convert.ToInt32(semanaFinalStr.Substring(6,2));
            int anoFinal = Convert.ToInt32(semanaFinalStr.Substring(0,4));

            var listaAcoes = hlbapp.GEP_Acao
                .Where(w => (w.Acao.Contains(pesquisa) || pesquisa == "")
                    && (w.Pilar == pilar || pilar == "(Todos)")
                    && (w.Objetivo == objetivo || objetivo == "(Todos)")
                    && w.Ano >= anoInicial && w.Ano <= anoFinal
                    && w.SemanaDoAno >= semanaInicial && w.SemanaDoAno <= semanaFinal)
                .OrderBy(o => o.SemanaDoAno).ThenBy(t => t.Acao)
                .ToList();

            return listaAcoes;
        }

        public List<GEP_Acao> FilterListAcoes()
        {
            CleanSessions();

            string pesquisa = Session["pesquisaSession"].ToString();
            string pilar = ((List<SelectListItem>)Session["FiltroDDLListaPilares"])
                .Where(w => w.Selected == true).FirstOrDefault().Text;
            SelectListItem firstItemObjetivo = ((List<SelectListItem>)Session["FiltroDDLListaObjetivos"])
                .Where(w => w.Selected == true).FirstOrDefault();
            string objetivo = "(Todos)";
            if (firstItemObjetivo != null) objetivo = firstItemObjetivo.Text;
            string semanaInicial = Session["semanaAnoInicialSession"].ToString();
            string semanaFinal = Session["semanaAnoFinalSession"].ToString();

            return ListAcoes(pesquisa, pilar, objetivo, semanaInicial, semanaFinal);
        }

        #endregion

        #region Lista Ações

        public ActionResult ListaAcoes()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["msg"] = "";

            int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                DateTime.Today,
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            if (Session["semanaAnoInicialSession"] == null)
                Session["semanaAnoInicialSession"] = DateTime.Today.Year.ToString() + "-W01";
            if (Session["semanaAnoFinalSession"] == null)
                Session["semanaAnoFinalSession"] = DateTime.Today.Year.ToString() + "-W" + semanaAno.ToString();
            if (Session["FiltroDDLListaPilares"] == null)
                Session["FiltroDDLListaPilares"] = CarregaListaPilares(true);
            if (Session["FiltroDDLListaObjetivos"] == null)
                Session["FiltroDDLListaObjetivos"] = new List<SelectListItem>();
            Session["DLListaStatusAcao"] = CarregaListaStatusAcao();
            Session["ListaAcoes"] = FilterListAcoes();

            return View();
        }

        public ActionResult SearchAcao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Valores

            if (model["semanaAnoInicial"] != null)
                Session["semanaAnoInicialSession"] = model["semanaAnoInicial"];

            if (model["semanaAnoFinal"] != null)
                Session["semanaAnoFinalSession"] = model["semanaAnoFinal"];

            if (model["pesquisa"] != null)
                Session["pesquisaSession"] = model["pesquisa"];

            if (model["Pilar"] != null)
                AtualizaDDL(model["Pilar"], (List<SelectListItem>)Session["FiltroDDLListaPilares"]);

            string objetivo = "(Todos)";
            if (model["Objetivo"] != null)
            {
                objetivo = model["Objetivo"];
                AtualizaDDL(model["Objetivo"], (List<SelectListItem>)Session["FiltroDDLListaObjetivos"]);
            }

            #endregion

            Session["ListaAcoes"] = ListAcoes(model["pesquisa"], model["Pilar"], objetivo,
                Session["semanaAnoInicialSession"].ToString(), Session["semanaAnoFinalSession"].ToString());

            return View("ListaAcoes");
        }

        #endregion

        #region CRUD Methods

        public void CarregaAcao(int id)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Acao acao = hlbapp.GEP_Acao.Where(w => w.ID == id).FirstOrDefault();

            //Session["semanaAno"] = FirstDateOfWeekISO8601(acao.Ano, acao.SemanaDoAno);
            Session["semanaAno"] = acao.Ano.ToString() + "-W" + acao.SemanaDoAno.ToString();
            Session["descricaoAcao"] = acao.Acao;
            AtualizaDDL(acao.Pilar, (List<SelectListItem>)Session["DLListaPilares"]);
            Session["DLListaObjetivos"] = CarregaListaObjetivos(false, acao.Pilar);
            AtualizaDDL(acao.Objetivo, (List<SelectListItem>)Session["DLListaObjetivos"]);
            Session["comentariosAcao"] = acao.Comentarios;
            Session["prazoAcao"] = acao.Prazo;
            string status = "";
            if (acao.Status == 0) status = "Não Realizado"; status = "Realizado";
            AtualizaDDL(status, (List<SelectListItem>)Session["DLListaStatusAcao"]);
        }

        public ActionResult CreateAcao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            return View("Acao");
        }

        public ActionResult EditAcao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CleanSessions();

            Session["idSelecionado"] = id;

            CarregaAcao(id);

            return View("Acao");
        }

        public ActionResult SaveAcao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Inicializa Entity

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            #endregion

            if (model["semanaAno"] != null)
            {
                #region Carrega Valores

                #region Semana do Ano

                int semanaAno = 0;
                if (model["semanaAno"] != null)
                    semanaAno = Convert.ToInt32(model["semanaAno"].Substring(6, 2));

                #endregion

                #region Ano

                int ano = Convert.ToInt32(model["semanaAno"].Substring(0, 4)); ;

                #endregion

                #region Descrição

                string descricao = "";
                if (model["descricao"] != null) descricao = model["descricao"];

                #endregion

                #region Pilar

                string pilar = "";
                if (model["Pilar"] != null) pilar = model["Pilar"];

                #endregion

                #region Objetivo

                string objetivo = "";
                if (model["Objetivo"] != null) objetivo = model["Objetivo"];

                #endregion

                #region Comentários

                string comentarios = "";
                if (model["comentarios"] != null) comentarios = model["comentarios"];

                #endregion

                #region Prazo

                DateTime prazo = new DateTime();
                if (model["prazo"] != null) prazo = Convert.ToDateTime(model["prazo"]);

                #endregion

                #region Status

                string status = "0";
                if (model["Status"] != null) status = model["Status"];

                #endregion

                #endregion

                #region Insere no WEB

                GEP_Acao acao = null;
                if (Convert.ToInt32(Session["idSelecionado"]) == 0)
                {
                    acao = new GEP_Acao();
                    acao.Usuario = Session["login"].ToString().ToUpper();
                    acao.DataHoraCadastro = DateTime.Now;
                }
                else
                {
                    int idSelecionado = Convert.ToInt32(Session["idSelecionado"]);
                    acao = hlbapp.GEP_Acao.Where(w => w.ID == idSelecionado).FirstOrDefault();
                }

                acao.SemanaDoAno = semanaAno;
                acao.Ano = ano;
                acao.Acao = descricao;
                acao.Pilar = pilar;
                acao.Objetivo = objetivo;
                acao.Comentarios = comentarios;
                acao.Prazo = prazo;
                acao.Status = Convert.ToInt32(status);
                acao.SemanaOriginal = semanaAno;
                acao.AnoOriginal = ano;

                if (Convert.ToInt32(Session["idSelecionado"]) == 0) hlbapp.GEP_Acao.AddObject(acao);

                #endregion

                hlbapp.SaveChanges();

                Session["msg"] = "Ação " + acao.Acao + " da semana "
                    + acao.SemanaDoAno.ToString() + "/" + acao.Ano.ToString() + " inserida com sucesso!";
            }

            Session["ListaAcoes"] = FilterListAcoes();
            //return View("ListaAcoes");
            return RedirectToAction("OK", "GEP");
        }

        public ActionResult ConfirmaDeleteAcao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idSelecionado"] = id;

            return View();
        }

        public ActionResult DeleteAcao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int id = Convert.ToInt32(Session["idSelecionado"]);

            GEP_Acao acao = hlbapp.GEP_Acao.Where(w => w.ID == id).FirstOrDefault();
            hlbapp.GEP_Acao.DeleteObject(acao);
            hlbapp.SaveChanges();

            Session["msg"] = "Ação " + acao.Acao + " da semana "
                + acao.SemanaDoAno.ToString() + "/" + acao.Ano.ToString() + " excluída com sucesso!";

            Session["ListaAcoes"] = FilterListAcoes();
            return RedirectToAction("OK", "GEP");
        }

        #endregion

        #region Event Methods

        public ActionResult EncerrarAcao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Acao acao = hlbapp.GEP_Acao.Where(w => w.ID == id).FirstOrDefault();
            acao.Status = 1;
            hlbapp.SaveChanges();

            Session["msg"] = "Ação " + acao.Acao + " da semana " 
                + acao.SemanaDoAno.ToString() + "/" + acao.Ano.ToString() + " encerrada com sucesso!";

            Session["ListaAcoes"] = FilterListAcoes();
            return RedirectToAction("OK", "GEP");
        }

        public ActionResult CancelarEncerramentoAcao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            GEP_Acao acao = hlbapp.GEP_Acao.Where(w => w.ID == id).FirstOrDefault();
            acao.Status = 0;
            hlbapp.SaveChanges();

            Session["msg"] = "Cancelado encerramento da ação " + acao.Acao + " da semana "
                + acao.SemanaDoAno.ToString() + "/" + acao.Ano.ToString() + " com sucesso!";

            Session["ListaAcoes"] = FilterListAcoes();
            return RedirectToAction("OK", "GEP");
        }

        public ActionResult OK()
        {
            if (Session["msg"] != null) ViewBag.Mensagem = Session["msg"];

            return View();
        }

        public static void Atualizacao_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 23)
            //if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday && DateTime.Now.Hour == 17)
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();
                Apolo10Entities apolo = new Apolo10Entities();

                int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int ano = DateTime.Today.Year;

                int semanaAnoProxima = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        DateTime.Today.AddDays(7), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int anoProximo = DateTime.Today.AddDays(7).Year;

                #region Verifica Fechamento Semana Atual

                var listaNaoFechadosSemanaAtual = hlbapp.GEP_Acao
                    .Where(w => w.Status == 0 && w.SemanaDoAno == semanaAno && w.Ano == ano)
                    .GroupBy(g => new
                    {
                        g.Usuario
                    })
                    .ToList();

                foreach (var item in listaNaoFechadosSemanaAtual)
                {
                    var listaAcoesPendentes = hlbapp.GEP_Acao
                        .Where(w => w.Status == 0 && w.SemanaDoAno == semanaAno && w.Ano == ano
                            && w.Usuario == item.Key.Usuario)
                        .OrderBy(o => o.Acao)
                        .ToList();

                    #region Relançar tarefas pendentes para a próxima semana

                    foreach (var acao in listaAcoesPendentes)
                    {
                        GEP_Acao acaoNova = new GEP_Acao();
                        acaoNova.SemanaDoAno = semanaAnoProxima;
                        acaoNova.Ano = anoProximo;
                        acaoNova.Acao = acao.Acao;
                        acaoNova.Pilar = acao.Pilar;
                        acaoNova.Objetivo = acao.Objetivo;
                        acaoNova.Comentarios = acao.Comentarios;
                        acaoNova.Prazo = acao.Prazo;
                        acaoNova.Status = acao.Status;
                        acaoNova.Usuario = acao.Usuario;
                        acaoNova.DataHoraCadastro = DateTime.Now;
                        acaoNova.SemanaOriginal = acao.SemanaOriginal;
                        acaoNova.AnoOriginal = acao.AnoOriginal;

                        hlbapp.GEP_Acao.AddObject(acaoNova);
                    }

                    #endregion

                    #region Enviar E-mail

                    #region Carrega Gerente

                    FUNCIONARIO gerente = apolo.FUNCIONARIO
                        .Where(w => apolo.GRP_FUNC
                            .Any(a => a.FuncCod == w.FuncCod
                                && a.GrpFuncObs == "RDV"
                                && apolo.FUNCIONARIO
                                    .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == item.Key.Usuario)))
                        .FirstOrDefault();

                    FUNCIONARIO funcionarioObj = apolo.FUNCIONARIO
                        .Where(n => n.UsuCod == item.Key.Usuario).FirstOrDefault();

                    #endregion

                    USUARIO usuarioGerente = apolo.USUARIO.Where(w => w.UsuCod == gerente.UsuCod).FirstOrDefault();
                    USUARIO usuarioObj = apolo.USUARIO.Where(w => w.UsuCod == item.Key.Usuario).FirstOrDefault();

                    string paraNome = usuarioObj.UsuNome;
                    string paraEmail = usuarioObj.UsuEmail;
                    string copiaPara = "";
                    //string copiaPara = usuarioGerente.UsuEmail + ";tlourenco@hyline.com.br";
                    string assunto = "GEP - AÇÕES NÃO CONCLUÍDAS - " + semanaAno.ToString() + "/" + ano.ToString()
                        + " - " + usuarioObj.UsuNome;
                    string stringChar = "" + (char)13 + (char)10;
                    string corpoEmail = "";
                    string anexos = "";
                    string empresaApolo = "";
                    if (funcionarioObj.USEREmpres == "BR") empresaApolo = "5";
                    else if (funcionarioObj.USEREmpres == "LB") empresaApolo = "7";
                    else if (funcionarioObj.USEREmpres == "HN") empresaApolo = "14";
                    else if (funcionarioObj.USEREmpres == "PL") empresaApolo = "20";

                    string listaAcoesPendenteStr = "";
                    foreach (var acao in listaAcoesPendentes)
                    {
                        listaAcoesPendenteStr = listaAcoesPendenteStr + "*" + acao.Acao + stringChar;
                    }

                    corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                        + "Existem ações não concluídas da semana " + semanaAno.ToString() + "/" + ano.ToString() + "." + stringChar
                        + "Segue abaixo a lista: " + stringChar + stringChar
                        + listaAcoesPendenteStr + stringChar
                        + "Elas serão relançadas para a próxima semana! " + stringChar + stringChar
                        + "SISTEMA WEB";

                    EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                    #endregion

                    hlbapp.SaveChanges();
                }

                #endregion

                #region Verifica Ações Lançadas Próxima Semana

                var listaFuncionarios = apolo.FUNCIONARIO
                    .Where(w => w.USERParticipaGEP == "Sim")
                    .ToList();

                foreach (var item in listaFuncionarios)
                {
                    int existe = hlbapp.GEP_Acao
                       .Where(w => w.SemanaDoAno == semanaAnoProxima && w.Ano == anoProximo
                            && w.Usuario == item.UsuCod)
                       .Count();

                    if (existe < 3)
                    {
                        #region Enviar E-mail

                        #region Carrega Gerente

                        FUNCIONARIO gerente = apolo.FUNCIONARIO
                            .Where(w => apolo.GRP_FUNC
                                .Any(a => a.FuncCod == w.FuncCod
                                    && a.GrpFuncObs == "RDV"
                                    && apolo.FUNCIONARIO
                                        .Any(n => n.FuncCod == a.GrpFuncCod && n.UsuCod == item.UsuCod)))
                            .FirstOrDefault();

                        FUNCIONARIO funcionarioObj = apolo.FUNCIONARIO
                            .Where(n => n.UsuCod == item.UsuCod).FirstOrDefault();

                        #endregion

                        USUARIO usuarioGerente = apolo.USUARIO.Where(w => w.UsuCod == gerente.UsuCod).FirstOrDefault();
                        USUARIO usuarioObj = apolo.USUARIO.Where(w => w.UsuCod == item.UsuCod).FirstOrDefault();

                        string paraNome = usuarioObj.UsuNome;
                        string paraEmail = usuarioObj.UsuEmail;
                        string copiaPara = "";
                        //string copiaPara = usuarioGerente.UsuEmail + ";tlourenco@hyline.com.br";
                        string assunto = "GEP - AÇÕES NÃO LANÇADAS OU MENORES QUE 03 - " + semanaAnoProxima.ToString() + "/" + anoProximo.ToString()
                            + " - " + usuarioObj.UsuNome;
                        string stringChar = "" + (char)13 + (char)10;
                        string corpoEmail = "";
                        string anexos = "";
                        string empresaApolo = "";
                        if (funcionarioObj.USEREmpres == "BR") empresaApolo = "5";
                        else if (funcionarioObj.USEREmpres == "LB") empresaApolo = "7";
                        else if (funcionarioObj.USEREmpres == "HN") empresaApolo = "14";
                        else if (funcionarioObj.USEREmpres == "PL") empresaApolo = "20";

                        corpoEmail = "Prezado " + paraNome + "," + stringChar + stringChar
                            + "As ações da semana " + semanaAnoProxima.ToString() + "/" + anoProximo.ToString() 
                            + " não foram lançadas ou são menores que 03 que é o mínimo permitido." + stringChar
                            + "Por favor, realizar os lançamentos " + stringChar + stringChar
                            + "SISTEMA WEB";

                        EnviarEmail(paraNome, paraEmail, copiaPara, assunto, corpoEmail, anexos, empresaApolo);

                        #endregion
                    }
                }

                #endregion
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

        public List<SelectListItem> CarregaListaPilares(bool todos)
        {
            List<SelectListItem> ddlListaPilares = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (todos)
            {
                ddlListaPilares.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaPilares = hlbapp.GEP_Pilar
                .OrderBy(o => o.Descricao)
                .ToList();

            foreach (var item in listaPilares)
            {
                ddlListaPilares.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.Descricao,
                    Selected = false
                });
            }

            return ddlListaPilares;
        }

        public List<SelectListItem> CarregaListaObjetivos(bool todos, string pilar)
        {
            List<SelectListItem> ddlListaObjetivos = new List<SelectListItem>();

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            if (todos)
            {
                ddlListaObjetivos.Add(new SelectListItem
                {
                    Text = "(Todos)",
                    Value = "(Todos)",
                    Selected = true
                });
            }

            var listaObjetivos = hlbapp.GEP_Objetivo
                .Where(w => hlbapp.GEP_Pilar.Any(a => a.ID == w.IDPilar && a.Descricao == pilar))
                .OrderBy(o => o.Descricao)
                .ToList();

            foreach (var item in listaObjetivos)
            {
                ddlListaObjetivos.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.Descricao,
                    Selected = false
                });
            }

            return ddlListaObjetivos;
        }

        public List<SelectListItem> CarregaListaStatusAcao()
        {
            List<SelectListItem> ddlListaStatusAcao = new List<SelectListItem>();

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Não Realizado",
                Value = "0",
                Selected = true
            });

            ddlListaStatusAcao.Add(new SelectListItem
            {
                Text = "Realizado",
                Value = "1",
                Selected = false
            });

            return ddlListaStatusAcao;
        }

        #endregion

        #region Other Methods

        public void CleanSessions()
        {
            #region Geral

            Session["idSelecionado"] = 0;
            if (Session["pesquisaSession"] == null) Session["pesquisaSession"] = "";

            #endregion

            #region Pilares

            Session["descricaoPilar"] = "";

            #endregion
            
            #region Objetivos

            Session["descricaoObjetivo"] = "";
            Session["DLListaPilares"] = CarregaListaPilares(false);

            #endregion

            #region Ação

            Session["FiltroDDLListaObjetivos"] = new List<SelectListItem>();

            int semanaAno = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                DateTime.Today.AddDays(7),
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            Session["semanaAno"] = DateTime.Today.AddDays(7).Year.ToString() + "-W" + semanaAno.ToString();
            Session["descricaoAcao"] = "";
            Session["DLListaPilares"] = CarregaListaPilares(false);
            Session["DLListaObjetivos"] = new List<SelectListItem>();
            Session["comentariosAcao"] = "";
            Session["prazoAcao"] = DateTime.Today;
            Session["DLListaStatusAcao"] = CarregaListaStatusAcao();

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

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            //return result.AddDays(-3).AddDays(6);
            return result.AddDays(-3);
        }

        public static void EnviarEmail(string paraNome, string paraEmail, string copiaPara,
            string assunto, string corpoEmail, string anexos, string empresaApolo)
        {
            string deEmail = "hyline.com.br";
            if (empresaApolo == "7") deEmail = "ltz.com.br";
            if (empresaApolo == "14") deEmail = "hnavicultura.com.br";
            if (empresaApolo == "20") deEmail = "planaltopostura.com.br";

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
            email.WorkFLowEmailDeEmail = "sistemas@" + deEmail;
            email.WorkFlowEmailFormato = "Texto";
            if (assunto.Length > 80) assunto = assunto.Substring(0, 80);
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexos;
            email.WorkFlowEmailDocEmpCod = empresaApolo;

            apolo.WORKFLOW_EMAIL.AddObject(email);

            apolo.SaveChanges();
        }

        public string VerificaAcaoSemana(string semanaAno)
        {
            string msg = "";
            string login = Session["login"].ToString().ToUpper();

            int semanaAnoAtual = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                DateTime.Today, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            int anoAtual = DateTime.Today.Year;

            int semana = Convert.ToInt32(semanaAno.Substring(6, 2));
            int ano = Convert.ToInt32(semanaAno.Substring(0, 4));

            if ((semana == semanaAnoAtual && ano == anoAtual
                && DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
                ||
                (semana > semanaAnoAtual && ano >= anoAtual))
            {
                HLBAPPEntities hlbapp = new HLBAPPEntities();

                #region Verifica Qtde. Máxima - 05 Ações

                int existe = hlbapp.GEP_Acao
                    .Where(w => w.Usuario == login && w.SemanaDoAno == semana && w.Ano == ano)
                    .Count();

                if (existe >= 5)
                {
                    msg = "O número máximo de ações por semana é de 05!";
                    return msg;
                }

                #endregion

                #region Verifica Qtde. Mínima - 03 Ações

                DateTime data = FirstDateOfWeekISO8601(ano, semana);
                data = data.AddDays(-7);
                int semanaAnterior = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                    data, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                int anoAnterior = data.Year;

                existe = 0;
                existe = hlbapp.GEP_Acao
                    .Where(w => w.Usuario == login && w.SemanaDoAno == semanaAnterior && w.Ano == anoAnterior)
                    .Count();

                if (existe < 3)
                {
                    msg = "Não é possível inserir ações na semana " + semana.ToString() + " de " + ano.ToString()
                        + " porque a semana anterior tem menos que 03 ações!";
                    return msg;
                }

                #endregion

                #region Insere Ações próximas se as anteriores estiverem todas realizadas

                if (semana > semanaAnoAtual && ano >= anoAtual)
                {
                    existe = 0;
                    existe = hlbapp.GEP_Acao
                        .Where(w => w.Usuario == login && w.SemanaDoAno == semanaAnoAtual && w.Ano == anoAtual
                            && w.Status == 0)
                        .Count();

                    if (existe > 0)
                    {
                        msg = "Só é possível programar a semana seguinte assim que todas as tarefas da semana atual estiverem"
                            + " concluídas! Caso não concluam as tarefas essa semana, automaticamente elas serão reinseridas"
                            + " para a próxima semana!";
                        return msg;
                    }
                }

                #endregion
            }

            return msg;
        }

        #endregion

        #region Json Methods

        [HttpPost]
        public ActionResult CarregaObjetivos(string pilar, string todos)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            bool todosB = Convert.ToBoolean(todos);

            List<SelectListItem> items = new List<SelectListItem>();

            items = CarregaListaObjetivos(todosB, pilar);

            return Json(items);
        }

        [HttpPost]
        public ActionResult VerificaQtdeAcaoSemana(string semanaAno)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msg = VerificaAcaoSemana(semanaAno);
            return Json(msg);
        }

        #endregion
    }
}
