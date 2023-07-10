using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using ImportaIncubacao.Data.Apolo;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.FLIPDataSetMobileTableAdapters;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Data.Objects;
using PS;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using System.Net;

namespace MvcAppHylinedoBrasilMobile.Controllers
{
    public class PedidoRacaoController : Controller
    {
        #region Objetos

        public static bdApoloEntities bdApolo = new bdApoloEntities();
        Apolo10EntitiesService apoloService = new Apolo10EntitiesService();
        public static HLBAPPEntities hlbapp = new HLBAPPEntities();

        FLOCKS1TableAdapter nucleos = new FLOCKS1TableAdapter();
        FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();
        MvcAppHyLinedoBrasil.Data.FLIPDataSet flip = new MvcAppHyLinedoBrasil.Data.FLIPDataSet();
        string serviceRoot = "https://bc-api.poultry-suite.com/PoultrySuite-Webservice/ODataV4/";

        #endregion

        #region Pedido de Ração

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //Test();

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            CarregaListaGranjas(false);
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

            int idPedidoRacao = Convert.ToInt32(Session["IDPedidoRacao"]);

            PedidoRacao item = hlbappSession.PedidoRacao
                .Where(w => w.DataInicial == null && w.ID == idPedidoRacao)
                .FirstOrDefault();

            if (item != null)
            {
                int count = hlbappSession.PedidoRacao_Item.Where(w => w.IDPedidoRacao == idPedidoRacao).Count();

                if (count > 0)
                {
                    string login = Session["login"].ToString();

                    item.Empresa = granja;
                    item.Usuario = login;
                    item.DataPedido = DateTime.Now;
                    item.DataInicial = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);
                    item.DataFinal = Convert.ToDateTime(Session["dataFinalPedidoRacao"]);
                    item.StatusPedido = Session["StatPedidoRacaoSelecionado"].ToString();
                }
                else
                {
                    hlbappSession.PedidoRacao.DeleteObject(item);
                }

                hlbappSession.SaveChanges();
            }

            return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
        }

        #region Pedido

        public ActionResult CreatePedidoRacao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            if ((DateTime.Now.DayOfWeek <= DayOfWeek.Thursday)
                ||
                (DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour <= 12 && DateTime.Now.Minute <= 00))
                Session["dataInicialPedidoRacao"] =
                    DateTime.Now.AddDays(8 - (int)DateTime.Now.DayOfWeek).ToString("dd/MM/yyyy");
            else
                Session["dataInicialPedidoRacao"] =
                    DateTime.Now.AddDays(15 - (int)DateTime.Now.DayOfWeek).ToString("dd/MM/yyyy");
            Session["dataFinalPedidoRacao"] = Session["dataInicialPedidoRacao"];
            Session["StatPedidoRacaoSelecionado"] = "Aberto";
            string granja = Session["granjaSelecionada"].ToString();

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

            if (granja == "")
            {
                ViewBag.Erro = "Para inserir um pedido, selecione uma granja!";
                return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
            }

            CarregaListaStatusPedidoRacao();

            Session["ListaRotaEntregaPedidosRacao"] =
                CarregaRotasEntrega(granja, Convert.ToDateTime(Session["dataInicialPedidoRacao"]));

            PedidoRacao pedidoRacao = new PedidoRacao();
            hlbappSession.PedidoRacao.AddObject(pedidoRacao);
            hlbappSession.SaveChanges();

            Session["IDPedidoRacao"] = pedidoRacao.ID;

            return View("PedidoRacao", hlbappSession.PedidoRacao_Item
                .Where(w => w.ID == -1)
                .ToList());
        }

        public ActionResult EditPedidoRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            CarregaListaStatusPedidoRacao();

            PedidoRacao pedido = hlbappSession.PedidoRacao.Where(w => w.ID == id).FirstOrDefault();

            string granja = Session["granjaSelecionada"].ToString();
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

            if (granja == "")
            {
                ViewBag.Erro = "Para alterar um pedido, selecione uma granja!";
                return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
            }

            Session["IDPedidoRacao"] = id;

            Session["ListaRotaEntregaPedidosRacao"] = CarregaRotasEntrega(pedido.Empresa,
                Convert.ToDateTime(pedido.DataInicial));
            AtualizaRotaSelecionada(pedido.RotaEntregaCod);

            if (pedido.DataInicial != null)
            {
                Session["dataInicialPedidoRacao"] = Convert.ToDateTime(pedido.DataInicial).ToString("dd/MM/yyyy");
                Session["dataFinalPedidoRacao"] = Convert.ToDateTime(pedido.DataFinal).ToString("dd/MM/yyyy");
                AtualizaStatusPedidoRacaoSelecionado(pedido.StatusPedido);
            }

            int idPedidoRacaoItem = Convert.ToInt32(Session["IDPedidoRacaoItem"]);

            #region Deleta Itens e Aditivos Incompletos

            List<PedidoRacao_Item> listItens = hlbappSession.PedidoRacao_Item
                .Where(w => w.Nucleo == null && w.IDPedidoRacao == id)
                .ToList();

            foreach (var item in listItens)
            {
                var listAditivos = hlbappSession.PedidoRacao_Item_Aditivo
                    .Where(w => w.IDPedidoRacao_Item == item.ID)
                    .ToList();

                foreach (var aditivo in listAditivos)
                {
                    hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
                }

                hlbappSession.PedidoRacao_Item.DeleteObject(item);
            }

            var listAditivosIDPedido = hlbappSession.PedidoRacao_Item_Aditivo
                    .Where(w => w.IDPedidoRacao == id
                        && w.ProdCodEstr == null)
                    .ToList();

            foreach (var aditivo in listAditivosIDPedido)
            {
                hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
            }

            hlbappSession.SaveChanges();

            #endregion

            var lista = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == id)
                .ToList();

            return View("PedidoRacao", lista);
        }

        public ActionResult SavePedidoRacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            string granja = Session["granjaSelecionada"].ToString();
            DateTime dataInicial = Convert.ToDateTime(model["dataInicialPedidoRacao"]);
            //DateTime dataFinal = Convert.ToDateTime(model["dataFinalPedidoRacao"]);
            DateTime dataFinal = Convert.ToDateTime(model["dataInicialPedidoRacao"]);
            string status = model["Text"].ToString();
            string login = Session["login"].ToString();

            int id = Convert.ToInt32(Session["IDPedidoRacao"]);

            PedidoRacao pedidoRacao = hlbappSession.PedidoRacao
                .Where(w => w.ID == id)
                .FirstOrDefault();

            pedidoRacao.Empresa = granja;
            pedidoRacao.Usuario = login;
            if (pedidoRacao.DataPedido == null)
                pedidoRacao.DataPedido = DateTime.Now;
            pedidoRacao.DataInicial = dataInicial;
            pedidoRacao.DataFinal = dataFinal;
            pedidoRacao.StatusPedido = status;

            hlbappSession.SaveChanges();

            var lista = CarregarListaPedidoRacao(granja, dataInicial, dataFinal);

            return View("Index", lista);
        }

        public ActionResult DeletePedidoRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var listaItens = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == id)
                .ToList();

            foreach (var item in listaItens)
            {
                var listaAditivo = hlbappSession.PedidoRacao_Item_Aditivo
                    .Where(w => w.IDPedidoRacao == item.IDPedidoRacao
                        && w.IDPedidoRacao_Item == item.ID)
                    .ToList();

                foreach (var aditivo in listaAditivo)
                {
                    hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
                }

                hlbappSession.PedidoRacao_Item.DeleteObject(item);
            }

            PedidoRacao pedidoRacao = hlbappSession.PedidoRacao
                .Where(w => w.ID == id)
                .FirstOrDefault();

            hlbappSession.PedidoRacao.DeleteObject(pedidoRacao);

            hlbappSession.SaveChanges();

            string granja = Session["granjaSelecionada"].ToString();
            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());

            ViewBag.Mensagem = "Pedido de Ração ID " + id.ToString() + " excluído com sucesso!";

            var lista = CarregarListaPedidoRacao(granja, dataInicial, dataFinal);

            return View("Index", lista);
        }

        #endregion

        #region Item

        public ActionResult CreatePedidoRacaoItem()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            try
            {
                //CarregaListaNucleos(false, "PR");
                CarregaListaNucleosPS(false, "PR");
                //CarregaFormulas();

                PedidoRacao_Item pedidoRacaoItem = new PedidoRacao_Item();
                pedidoRacaoItem.IDPedidoRacao = Convert.ToInt32(Session["IDPedidoRacao"]);
                hlbappSession.PedidoRacao_Item.AddObject(pedidoRacaoItem);
                hlbappSession.SaveChanges();

                Session["IDPedidoRacaoItem"] = pedidoRacaoItem.ID;

                Session["ListaGalpoesSelecionados"] = new List<SelectListItem>();
                Session["ListaLinhagensSelecionadas"] = new List<SelectListItem>();
                Session["ListaFormulas"] = new List<SelectListItem>();

                return View("PedidoRacaoItem", pedidoRacaoItem);
            }
            catch (Exception e)
            {
                if (e.InnerException == null)
                    ViewBag.Erro = "Erro ao inserir novo item na ração: " + e.Message;
                else
                    ViewBag.Erro = "Erro ao inserir novo item na ração: " + e.Message
                        + " / Erro interno: " + e.InnerException.Message;

                int id = Convert.ToInt32(Session["IDPedidoRacao"]);

                var lista = hlbappSession.PedidoRacao_Item
                    .Where(w => w.IDPedidoRacao == id)
                    .ToList();

                return View("PedidoRacao", lista);

            }
        }

        public ActionResult EditPedidoRacaoItem(int idPedidoRacao, int idPedidoRacaoItem)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            PedidoRacao_Item pedidoRacaoItem = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == idPedidoRacao && w.ID == idPedidoRacaoItem)
                .FirstOrDefault();

            PedidoRacao pedidoRacao = hlbappSession.PedidoRacao
                .Where(w => w.ID == idPedidoRacao)
                .FirstOrDefault();

            Session["IDPedidoRacaoItem"] = pedidoRacaoItem.ID;

            //CarregaListaNucleos(false, "PR");
            CarregaListaNucleosPS(false, "PR");
            AtualizaNucleoSelecionado(pedidoRacaoItem.Nucleo);
            CarregaGalpoes(pedidoRacaoItem.Nucleo, "PR");
            AtualizaGalpaoSelecionado(pedidoRacaoItem.Galpao);
            CarregaLinhagens(pedidoRacaoItem.Galpao);
            AtualizaLinhagemSelecionada(pedidoRacaoItem.Linhagem);

            int galpao = Convert.ToInt32(pedidoRacaoItem.Galpao);
            DateTime data = Convert.ToDateTime(pedidoRacao.DataFinal);

            decimal henHouse = LastHenHouse(pedidoRacaoItem.Nucleo, galpao,
                pedidoRacaoItem.Linhagem, data);

            int age = Convert.ToInt32(LastAge(pedidoRacaoItem.Nucleo, galpao,
                pedidoRacaoItem.Linhagem, data));

            CarregaFormulas(henHouse, age);
            AtualizaFormulaSelecionada(pedidoRacaoItem.CodFormulaRacao.ToString());

            return View("PedidoRacaoItem", pedidoRacaoItem);
        }

        public ActionResult SavePedidoRacaoItem(PedidoRacao_Item item)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            item.IDPedidoRacao = Convert.ToInt32(Session["IDPedidoRacao"]);
            item.ID = Convert.ToInt32(Session["IDPedidoRacaoItem"]);

            DateTime dataSelecionada = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);

            PedidoRacao_Item itemVerifica = hlbappSession.PedidoRacao_Item
                .Where(w => w.Nucleo == item.Nucleo
                    && w.Galpao == item.Galpao
                    && w.Linhagem == item.Linhagem
                    && w.ID != item.ID
                    && hlbappSession.PedidoRacao
                        .Any(p => p.ID == w.IDPedidoRacao
                            && p.DataInicial == dataSelecionada))
                .FirstOrDefault();

            if (itemVerifica != null)
            {
                ViewBag.Erro = "Núcleo " + item.Nucleo + ", Galpão " + item.Galpao
                    + " e Linhagem " + item.Linhagem
                    + " já cadastrados nesse Pedido! Verifique!";

                PedidoRacao_Item itemNull = hlbappSession.PedidoRacao_Item
                    .Where(w => w.Nucleo == null && w.IDPedidoRacao == item.IDPedidoRacao
                        && w.ID == item.ID)
                    .FirstOrDefault();

                if (itemNull != null)
                {
                    #region Verifica se tem aditivo no nulo para deletar

                    List<PedidoRacao_Item_Aditivo> listaAditivos = hlbappSession.PedidoRacao_Item_Aditivo
                        .Where(w => w.IDPedidoRacao == itemNull.IDPedidoRacao
                            && w.IDPedidoRacao_Item == itemNull.ID).ToList();

                    foreach (var aditivoNull in listaAditivos)
                    {
                        hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivoNull);
                    }

                    #endregion

                    hlbappSession.PedidoRacao_Item.DeleteObject(itemNull);
                    hlbappSession.SaveChanges();
                }

                var listaVerifica = hlbappSession.PedidoRacao_Item
                    .Where(w => w.IDPedidoRacao == item.IDPedidoRacao)
                    .ToList();

                return View("PedidoRacao", listaVerifica);
            }

            PedidoRacao_Item item1 = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == item.IDPedidoRacao && w.ID == item.ID)
                .FirstOrDefault();

            item1.Nucleo = item.Nucleo;
            item1.Galpao = item.Galpao;
            item1.Linhagem = item.Linhagem;
            item1.CodFormulaRacao = item.CodFormulaRacao;

            MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO1 produtoFormula =
                bdApolo.PRODUTO1.Where(w => w.USERNumFormula == item1.CodFormulaRacao).FirstOrDefault();
            item1.ProdCodEstr = produtoFormula.ProdCodEstr;
            //item1.QtdeKg = item.QtdeKg;

            hlbappSession.SaveChanges();

            var lista = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == item1.IDPedidoRacao)
                .ToList();

            return View("PedidoRacao", lista);
        }

        public ActionResult DeletePedidoRacaoItem(int idPedidoRacao, int idPedidoRacaoItem)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var listaAditivo = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == idPedidoRacao
                    && w.IDPedidoRacao_Item == idPedidoRacaoItem)
                .ToList();

            foreach (var aditivo in listaAditivo)
            {
                hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
            }

            PedidoRacao_Item item = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == idPedidoRacao
                    && w.ID == idPedidoRacaoItem)
                .FirstOrDefault();

            hlbappSession.PedidoRacao_Item.DeleteObject(item);

            hlbappSession.SaveChanges();

            var lista = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == idPedidoRacao)
                .ToList();

            return View("PedidoRacao", lista);
        }

        #endregion

        #region Aditivo

        public ActionResult CreatePedidoRacaoItemAditivo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            CarregaInsumos();

            PedidoRacao_Item_Aditivo pedidoRacaoItemAditivo = new PedidoRacao_Item_Aditivo();

            return View("PedidoRacaoItemAditivo", pedidoRacaoItemAditivo);
        }

        public ActionResult SavePedidoRacaoItemAditivo(PedidoRacao_Item_Aditivo aditivo)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            aditivo.IDPedidoRacao = Convert.ToInt32(Session["IDPedidoRacao"]);
            aditivo.IDPedidoRacao_Item = Convert.ToInt32(Session["IDPedidoRacaoItem"]);
            aditivo.Origem = "Manual";
            hlbappSession.PedidoRacao_Item_Aditivo.AddObject(aditivo);
            hlbappSession.SaveChanges();

            PedidoRacao_Item pedidoRacaoItem = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == aditivo.IDPedidoRacao && w.ID == aditivo.IDPedidoRacao_Item)
                .FirstOrDefault();

            return View("PedidoRacaoItem", pedidoRacaoItem);
        }

        public ActionResult DeletePedidoRacaoItemAditivo(int idPedidoRacao, int idPedidoRacaoItem,
            int idPedidoRacaoItemAditivo)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            PedidoRacao_Item_Aditivo aditivo = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == idPedidoRacao
                    && w.IDPedidoRacao_Item == idPedidoRacaoItem
                    && w.ID == idPedidoRacaoItemAditivo)
                .FirstOrDefault();

            hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
            hlbappSession.SaveChanges();

            PedidoRacao_Item pedidoRacaoItem = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == aditivo.IDPedidoRacao && w.ID == aditivo.IDPedidoRacao_Item)
                .FirstOrDefault();

            return View("PedidoRacaoItem", pedidoRacaoItem);
        }

        #endregion

        #region Relatório Excel

        public ActionResult GerarRelatorioPedidoRacao()
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

            string pasta = "C:\\inetpub\\wwwroot\\Relatorios\\Pedido_Racao";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Pedido_Racao\\Relatorio_Pedido_Racao_"
                + Session["login"].ToString() + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";

            string pesquisa = "*Relatorio_Pedido_Racao_"
                + Session["login"].ToString() + ".xlsx";

            destino = GeraRelatorioPedidoRacaoExcel(pesquisa, true, pasta, destino,
                dataInicial, dataFinal, granja);

            return File(destino, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Lista_Pedidos_" + granja + "_" + dataInicial.ToString("yyyy-MM-dd") +
                "_a_" + dataFinal.ToString("yyyy-MM-dd") + ".xlsx");
        }

        public string GeraRelatorioPedidoRacaoExcel(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Pedido_Racao\\Relatorio_Pedido_Racao.xlsx", destino);

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

            #region SQL

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Rel_Pedido_Racao V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStr = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "V.[Data Pedido] between '" + dataInicialStr + "' and '" + dataFinalStr + "' and " +
                    //"(V.[Cód. Unidade] = '" + empresa + "' or '" + empresa + "' = '') ";
                    "CHARINDEX(V.[Cód. Unidade], '" + filtroGranjas + "') > 0 ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "V.[Data Pedido], V.[Cód. Unidade]";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("PedidoRacao"))
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

        #region Pedido de Ração - Novo

        public void CarregaPR(int id)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            PedidoRacao pr = hlbappSession.PedidoRacao
                .Where(w => w.ID == id).FirstOrDefault();

            Session["IDPedidoRacao"] = id;
            Session["dataInicialPedidoRacao"] = pr.DataInicial;
            Session["ordemPedidoRacao"] = pr.Ordem;
            Session["observacao"] = "";

            Session["dentroPeriodoPermitido"] = MvcAppHylinedoBrasilMobile.Controllers.PedidoRacaoController
                .VerificaDataParaAlteracao(Convert.ToDateTime(pr.DataInicial), false);

            var logPR = hlbappSession.LOG_PedidoRacao.Where(w => w.IDPedidoRacao == id)
                .OrderByDescending(o => o.DataOperacao).FirstOrDefault();

            Session["statusPR"] = pr.StatusPedido;
            if (logPR != null)
            {
                if (logPR.Operacao.Contains("Pendente"))
                {
                    Session["statusPR"] = logPR.Operacao;
                    Session["observacao"] = logPR.Observacao;
                }
            }

            Session["ListaItensPedidoRacao"] = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == id).ToList();
            Session["ListaAdicionaisPedidoRacao"] = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == id && w.Origem == "Manual").ToList();
        }

        public ActionResult CreatePedidoRacaoNovo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            //if ((DateTime.Now.DayOfWeek <= DayOfWeek.Thursday)
            //    ||
            //    (DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour <= 12 && DateTime.Now.Minute <= 00))
            //    Session["dataInicialPedidoRacao"] =
            //        DateTime.Now.AddDays(8 - (int)DateTime.Now.DayOfWeek).ToString("dd/MM/yyyy");
            //else
            //    Session["dataInicialPedidoRacao"] =
            //        DateTime.Now.AddDays(15 - (int)DateTime.Now.DayOfWeek).ToString("dd/MM/yyyy");
            Session["dataInicialPedidoRacao"] = "";
            Session["ordemPedidoRacao"] = 1;
            Session["StatPedidoRacaoSelecionado"] = "Aberto";
            Session["observacao"] = "";
            Session["motivoReprovacao"] = "";
            Session["dentroPeriodoPermitido"] = true;
            Session["statusPR"] = "Aberto";
            string granja = Session["granjaSelecionada"].ToString();

            if (granja == "")
            {
                DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
                DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());

                ViewBag.Erro = "Para inserir um pedido, selecione uma granja!";
                return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
            }

            Session["IDPedidoRacao"] = 0;
            Session["ListaItensPedidoRacao"] = new List<PedidoRacao_Item>();
            Session["ListaItensPedidoRacaoDelete"] = new List<PedidoRacao_Item>();
            Session["ListaAdicionaisPedidoRacao"] = new List<PedidoRacao_Item_Aditivo>();
            Session["ListaAdicionaisPedidoRacaoDelete"] = new List<PedidoRacao_Item_Aditivo>();

            //CarregaListaNucleos(false, "PR");
            CarregaListaNucleosPS(false, "PR");
            Session["ListaAdicionaisPR"] = CarregaAdicionais(true);
            Session["ListaGalpoesSelecionados"] = new List<SelectListItem>();
            Session["ListaLinhagensSelecionadas"] = new List<SelectListItem>();
            Session["ListaFormulas"] = new List<SelectListItem>();

            return View("PedidoRacaoNovo");
        }

        public ActionResult EditPedidoRacaoNovo(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string granja = Session["granjaSelecionada"].ToString();
            CarregaPR(id);
            Session["motivoReprovacao"] = "";
            Session["ListaItensPedidoRacaoDelete"] = new List<PedidoRacao_Item>();
            Session["ListaAdicionaisPedidoRacaoDelete"] = new List<PedidoRacao_Item_Aditivo>();

            //CarregaListaNucleos(false, "PR");
            CarregaListaNucleosPS(false, "PR");
            Session["ListaAdicionaisPR"] = CarregaAdicionais(true);
            Session["ListaGalpoesSelecionados"] = new List<SelectListItem>();
            Session["ListaLinhagensSelecionadas"] = new List<SelectListItem>();
            Session["ListaFormulas"] = new List<SelectListItem>();

            return View("PedidoRacaoNovo");
        }

        [HttpPost]
        public ActionResult SavePedidoRacaoNovo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            Apolo10EntitiesService apoloSession = new Apolo10EntitiesService();

            #region Carrega Variáveis

            string operacao = "Inclusão";
            string observacao = "";
            if (model["observacao"] != null) observacao = model["observacao"];
            DateTime dataPedido = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);
            string granja = Session["granjaSelecionada"].ToString();
            string descricaoGranja = ((List<SelectListItem>)Session["ListaGranjas"])
                .Where(w => w.Value == granja).FirstOrDefault().Text;

            string detalhesDataPedido = "";
            string stringChar = "<br />";

            bool dentroPeriodoPermitido = VerificaDataParaAlteracao(dataPedido, false);

            var listaAdicionais = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacao"];
            var listaAdicionaisDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacaoDelete"];

            #endregion

            #region Verifica se existe item lançado

            var listaItens = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacao"];
            var listaItensDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacaoDelete"];

            if (listaItens.Count == 0)
            {
                ViewBag.Erro = "Obrigatório inserir pelo menos um item!";
                return View("PedidoRacaoNovo");
            }
            else
            {
                #region Verifica a data do pedido para refazer o modelo e configuração das fórmulas

                DateTime dataInicioNovoModelo = Convert.ToDateTime("29/03/2020");

                if (dataPedido >= dataInicioNovoModelo)
                {
                    foreach (var item in listaItens)
                    {
                        var cfr = hlbappSession.Config_Formula_Racao
                            .Where(w => hlbappSession.Config_Formula_Racao_Galpao
                                    .Any(a => w.ID == a.IDConfigFormulaRacao
                                        && a.CodNucleo == item.Nucleo && a.NumGalpao == item.Galpao)
                                && hlbappSession.Config_Formula_Racao_Linhagem
                                    .Any(a => w.ID == a.IDConfigFormulaRacao
                                        && a.Linhagem == item.Linhagem))
                            .FirstOrDefault();

                        if (cfr != null)
                        {
                            item.IDConfigFormulaRacao = cfr.ID;
                            ImportaIncubacao.Data.Apolo.PRODUTO1 produto1 = apoloService.PRODUTO1.Where(w => w.ProdCodEstr == cfr.ProdCodEstr).FirstOrDefault();
                            item.CodFormulaRacao = produto1.USERNumFormula;
                            item.ProdCodEstr = cfr.ProdCodEstr;
                        }
                        else
                        {
                            ViewBag.Erro = "Não existe Fórmula Configurada para o Núcleo " + item.Nucleo + ", Galpão " + item.Galpao
                                + " e Linhagem " + item.Linhagem
                                + "! Verifique com a Gabriela ou o Wellington sobre isso!";
                            return View("PedidoRacaoNovo");
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region PedidoRacao

            PedidoRacao pr = new PedidoRacao();
            if (id > 0)
            {
                operacao = "Alteração";
                pr = hlbappSession.PedidoRacao.Where(w => w.ID == id).FirstOrDefault();
            }

            pr.Empresa = granja;
            pr.Usuario = Session["login"].ToString();
            pr.DataPedido = DateTime.Now;
            if (pr.DataInicial != dataPedido)
                detalhesDataPedido = "Data do Pedido alterada de "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " para "
                    + dataPedido.ToShortDateString() + stringChar + stringChar;
            pr.DataInicial = dataPedido;
            pr.DataFinal = dataPedido;
            pr.StatusPedido = "Aberto";

            if (id == 0)
            {
                hlbappSession.PedidoRacao.AddObject(pr);
                hlbappSession.SaveChanges();
            }

            #endregion

            if (dentroPeriodoPermitido)
            {
                hlbappSession.SaveChanges();

                #region Insere no banco de dados

                #region PedidoRacao_Item

                foreach (var item in listaItensDelete)
                {
                    if (item.ID > 0)
                    {
                        PedidoRacao_Item pri = hlbappSession.PedidoRacao_Item
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbappSession.PedidoRacao_Item.DeleteObject(pri);
                    }
                }

                foreach (var item in listaAdicionaisDelete)
                {
                    if (item.ID > 0)
                    {
                        PedidoRacao_Item_Aditivo adicional = hlbappSession.PedidoRacao_Item_Aditivo
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                        hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(adicional);
                    }
                }

                hlbappSession.SaveChanges();

                foreach (var item in listaItens)
                {
                    PedidoRacao_Item pri = new PedidoRacao_Item();
                    if (item.ID > 0)
                        pri = hlbappSession.PedidoRacao_Item
                            .Where(w => w.ID == item.ID).FirstOrDefault();
                    else
                        pri.IDPedidoRacao = pr.ID;

                    pri.Nucleo = item.Nucleo;
                    pri.Galpao = item.Galpao;
                    pri.Linhagem = item.Linhagem;
                    pri.CodFormulaRacao = item.CodFormulaRacao;
                    pri.ProdCodEstr = item.ProdCodEstr;
                    pri.QtdeKg = item.QtdeKg;
                    pri.Sequencia = item.Sequencia;
                    pri.IDConfigFormulaRacao = item.IDConfigFormulaRacao;

                    if (pri.ID == 0) hlbappSession.PedidoRacao_Item.AddObject(pri);
                    hlbappSession.SaveChanges();

                    #region PedidoRacao_Item_Aditivo

                    var listaAdicionaisItem = listaAdicionais.Where(w => w.SeqItem == pri.Sequencia).ToList();

                    foreach (var add in listaAdicionaisItem)
                    {
                        PedidoRacao_Item_Aditivo adicional = new PedidoRacao_Item_Aditivo();
                        if (add.ID > 0)
                            adicional = hlbappSession.PedidoRacao_Item_Aditivo
                                .Where(w => w.ID == add.ID).FirstOrDefault();
                        else
                        {
                            adicional.IDPedidoRacao = pr.ID;
                            adicional.IDPedidoRacao_Item = pri.ID;
                        }

                        adicional.ProdCodEstr = add.ProdCodEstr;
                        adicional.QtdeKgPorTon = add.QtdeKgPorTon;
                        adicional.SeqItem = add.SeqItem;
                        adicional.Sequencia = add.Sequencia;
                        adicional.Origem = add.Origem;

                        if (adicional.ID == 0) hlbappSession.PedidoRacao_Item_Aditivo.AddObject(adicional);
                    }

                    #endregion
                }

                #endregion

                hlbappSession.SaveChanges();

                #endregion
            }
            else
            {
                operacao = operacao + " Pendente";
            }

            #region Insere LOG

            InsereLOGPedidoRacao(pr, listaItens, listaAdicionais, listaItensDelete, listaAdicionaisDelete,
                DateTime.Now, Session["login"].ToString(), operacao, observacao);

            #endregion

            if (dentroPeriodoPermitido)
                ViewBag.Mensagem = "Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                    + dataPedido.ToShortDateString() + " salvo com sucesso!";
            else
            {
                ViewBag.Mensagem = "Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                    + dataPedido.ToShortDateString() + " aguardando aprovação da Fábrica!";

                #region Envia E-mail

                #region Carrega pedido para ir no corpo do e-mail

                string detalhesPedido = CarregaDetalhesCorpoEmail(listaItens, listaAdicionais,
                    listaItensDelete, listaAdicionaisDelete);

                #endregion

                #region Gera o E-mail

                string assunto = "RAÇÃO - " + operacao.ToUpper() + " FORA DE PERÍODO - "
                    + Convert.ToDateTime(pr.DataInicial).ToString("dd/MM/yy")
                    + " - " + descricaoGranja;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                Apolo10Entities apolo = new Apolo10Entities();

                string login = Session["login"].ToString().ToUpper();
                USUARIO usuario = apolo.USUARIO.Where(w => w.UsuCod == login).FirstOrDefault();

                //string porta = "";
                //if (Request.Url.Port != 80)
                //    porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado," + stringChar + stringChar
                    + "Existe " + operacao + " do pedido da granja " + descricaoGranja + " do dia "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " realizada pelo usuário "
                    + usuario.UsuNome + " em " + DateTime.Now + " conforme dados abaixo: " + stringChar + stringChar
                    + detalhesDataPedido
                    + detalhesPedido + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Por favor, analisar e realizar a aprovação ou não para a alteração ser realizada!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail("FÁBRICA DE RAÇÃO",
                    "pedido.racao@hyline.com.br",
                    //"palves@hyline.com.br", 
                    usuario.UsuEmail, assunto, corpoEmail, anexos, empresaApolo, "Html");

                #endregion

                #endregion
            }

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());
            return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
        }

        public ActionResult ConfirmaDeletePedidoRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            Session["IDPedidoRacao"] = id;
            string granja = Session["granjaSelecionada"].ToString();
            string descricaoGranja = ((List<SelectListItem>)Session["ListaGranjas"])
                .Where(w => w.Value == granja).FirstOrDefault().Text;

            PedidoRacao pedidoRacao = hlbappSession.PedidoRacao
                .Where(w => w.ID == id)
                .FirstOrDefault();

            Session["dentroPeriodoPermitido"] = MvcAppHylinedoBrasilMobile.Controllers.PedidoRacaoController
                .VerificaDataParaAlteracao(Convert.ToDateTime(pedidoRacao.DataInicial), false);

            ViewBag.DadosPedido = " Granja: " + descricaoGranja + " - Entrega em: "
                + Convert.ToDateTime(pedidoRacao.DataInicial).ToShortDateString();

            return View();
        }

        [HttpPost]
        public ActionResult DeletePedidoRacaoNovo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega Variáveis

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);
            string granja = Session["granjaSelecionada"].ToString();
            string descricaoGranja = ((List<SelectListItem>)Session["ListaGranjas"])
                .Where(w => w.Value == granja).FirstOrDefault().Text;
            string observacao = "";
            if (model["observacao"] != null) observacao = model["observacao"];
            bool dentroPeriodoPermitido = Convert.ToBoolean(Session["dentroPeriodoPermitido"]);

            string operacaoLog = "Exclusão";
            if (!dentroPeriodoPermitido)
                operacaoLog = "Exclusão Pendente";

            List<PedidoRacao_Item> listaItensDelete = new List<PedidoRacao_Item>();
            List<PedidoRacao_Item_Aditivo> listaAdicionaisDelete = new List<PedidoRacao_Item_Aditivo>();

            PedidoRacao pedidoRacao = hlbappSession.PedidoRacao
                .Where(w => w.ID == id)
                .FirstOrDefault();

            var listaItens = hlbappSession.PedidoRacao_Item
                    .Where(w => w.IDPedidoRacao == id)
                    .ToList();

            #endregion

            #region Exclui o pedido

            foreach (var item in listaItens)
            {
                var listaAditivo = hlbappSession.PedidoRacao_Item_Aditivo
                    .Where(w => w.IDPedidoRacao == item.IDPedidoRacao
                        && w.IDPedidoRacao_Item == item.ID)
                    .ToList();

                foreach (var aditivo in listaAditivo)
                {
                    if (dentroPeriodoPermitido)
                        hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(aditivo);
                    listaAdicionaisDelete.Add(aditivo);
                }

                if (dentroPeriodoPermitido)
                    hlbappSession.PedidoRacao_Item.DeleteObject(item);
                listaItensDelete.Add(item);
            }

            if (dentroPeriodoPermitido)
                hlbappSession.PedidoRacao.DeleteObject(pedidoRacao);

            hlbappSession.SaveChanges();

            #endregion

            #region Insere LOG

            var listaItens2 = new List<PedidoRacao_Item>();
            var listaAdicionais = new List<PedidoRacao_Item_Aditivo>();

            InsereLOGPedidoRacao(pedidoRacao, listaItens2, listaAdicionais, listaItensDelete, listaAdicionaisDelete,
                DateTime.Now, Session["login"].ToString(), operacaoLog, "");

            #endregion

            if (dentroPeriodoPermitido)
                ViewBag.Mensagem = "Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                    + Convert.ToDateTime(pedidoRacao.DataInicial).ToShortDateString() + " excluído com sucesso!";
            else
            {
                ViewBag.Mensagem = "Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                    + Convert.ToDateTime(pedidoRacao.DataInicial).ToShortDateString()
                    + " aguardando aprovação da Fábrica!";

                #region Envia E-mail

                string stringChar = "<br />";

                #region Carrega pedido para ir no corpo do e-mail

                string detalhesPedido = CarregaDetalhesCorpoEmail(listaItens2, listaAdicionais,
                    listaItensDelete, listaAdicionaisDelete);

                #endregion

                #region Gera o E-mail

                string assunto = "RAÇÃO - PEDIDO EXCLUÍDO FORA DE PERÍODO - "
                    + Convert.ToDateTime(pedidoRacao.DataInicial).ToString("dd/MM/yy")
                    + " - " + descricaoGranja;
                string corpoEmail = "";
                string anexos = "";
                string empresaApolo = "5";

                Apolo10Entities apolo = new Apolo10Entities();

                string login = Session["login"].ToString().ToUpper();
                USUARIO usuario = apolo.USUARIO.Where(w => w.UsuCod == login).FirstOrDefault();

                //string porta = "";
                //if (Request.Url.Port != 80)
                //    porta = ":" + Request.Url.Port.ToString();

                corpoEmail = "Prezado," + stringChar + stringChar
                    + "O pedido da granja " + descricaoGranja + " do dia "
                    + Convert.ToDateTime(pedidoRacao.DataInicial).ToShortDateString()
                    + " foi excluído pelo usuário "
                    + usuario.UsuNome + " em " + DateTime.Now + " conforme dados abaixo: " + stringChar + stringChar
                    + detalhesPedido + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Por favor, analisar e realizar a aprovação ou não para a alteração ser realizada!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";

                EnviarEmail("FÁBRICA DE RAÇÃO",
                    "pedido.racao@hyline.com.br",
                    //"palves@hyline.com.br",
                    usuario.UsuEmail, assunto, corpoEmail, anexos, empresaApolo, "Html");

                #endregion

                #endregion
            }

            var lista = CarregarListaPedidoRacao(granja, dataInicial, dataFinal);
            return View("Index", lista);
        }

        #region Item - Novo

        [HttpPost]
        public ActionResult SaveItemPedidoRacaoNovo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);
            DateTime dataSelecionada = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);
            DateTime dataInicioNovoModelo = Convert.ToDateTime("29/03/2020");

            string nucleo = model["Nucleo"];
            string galpao = model["Galpao"];
            string linhagem = model["Linhagem"];
            //int codFormulaRacao = Convert.ToInt32(model["CodFormulaRacao"]);
            int codFormulaRacao = 0;
            Config_Formula_Racao cfr = new Config_Formula_Racao();
            if (dataSelecionada >= dataInicioNovoModelo)
            {
                cfr = hlbappSession.Config_Formula_Racao
                    .Where(w => hlbappSession.Config_Formula_Racao_Galpao
                            .Any(a => w.ID == a.IDConfigFormulaRacao
                                && a.CodNucleo == nucleo && a.NumGalpao == galpao)
                        && hlbappSession.Config_Formula_Racao_Linhagem
                            .Any(a => w.ID == a.IDConfigFormulaRacao
                                && a.Linhagem == linhagem))
                    .FirstOrDefault();

                if (cfr != null)
                {
                    codFormulaRacao = cfr.ID;
                }
                else
                {
                    ViewBag.Erro = "Não existe Fórmula Configurada para o Núcleo " + nucleo + ", Galpão " + galpao
                        + " e Linhagem " + linhagem
                        + "! Verifique com a Gabriela ou o Wellington sobre isso!";
                    return View("PedidoRacaoNovo");
                }
            }
            else
            {
                codFormulaRacao = Convert.ToInt32(model["CodFormulaRacao"]);
                cfr = hlbappSession.Config_Formula_Racao.Where(w => w.ID == codFormulaRacao).FirstOrDefault();
            }
            decimal qtdeKg = Convert.ToDecimal(model["QtdeKg"]);

            #region Carrega Ultimo lote do Galpão e Idade

            FLOCKSMobileTableAdapter fTA = new FLOCKSMobileTableAdapter();
            FLIPDataSetMobile.FLOCKSMobileDataTable fDT = new FLIPDataSetMobile.FLOCKSMobileDataTable();
            fTA.FillByFarmIdAndNumGalpao(fDT, nucleo, Convert.ToDecimal(galpao));

            var lote = fDT
                .Where(w => w.HATCH_DATE <= dataSelecionada
                    && w.VARIETY == linhagem)
                .OrderByDescending(o => o.HATCH_DATE).FirstOrDefault();

            string loteCompleto = "";
            int age = 0;
            if (lote != null)
            {
                //age = Convert.ToInt32(fdTA.LastAge(loteCompleto, dataSelecionada));
                age = Convert.ToInt32(Math.Floor((dataSelecionada - lote.HATCH_DATE).TotalDays / 7));
            }

            #endregion

            var listaItens = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacao"];

            // Verifica lançado no banco de dados
            int existeVerifica = hlbappSession.PedidoRacao_Item
                .Where(w => w.Nucleo == nucleo
                    && w.Galpao == galpao
                    && w.Linhagem == linhagem
                    && w.IDPedidoRacao != id
                    && hlbappSession.PedidoRacao
                        .Any(p => p.ID == w.IDPedidoRacao
                            && p.DataInicial == dataSelecionada))
                .Count();

            // Verifica nos itens do pedido vigente
            existeVerifica = existeVerifica + listaItens
                .Where(w => w.Nucleo == nucleo
                    && w.Galpao == galpao
                    && w.Linhagem == linhagem
                    && w.ID == 0)
                .Count();

            if (existeVerifica > 0)
            {
                ViewBag.Erro = "Núcleo " + nucleo + ", Galpão " + galpao
                    + " e Linhagem " + linhagem
                    + " já cadastrados nesse Pedido! Verifique!";
                return View("PedidoRacaoNovo");
            }

            // Adiciona na Lista de Itens
            int? seq = hlbappSession.LOG_PedidoRacao_Item.Where(w => w.IDPedidoRacao == id
                && id != 0).Max(m => m.Sequencia);
            if (seq == null)
                seq = listaItens.Max(m => m.Sequencia);
            if (seq == null) seq = 0;

            ImportaIncubacao.Data.Apolo.PRODUTO1 produto1 = apoloService.PRODUTO1.Where(w => w.ProdCodEstr == cfr.ProdCodEstr).FirstOrDefault();

            PedidoRacao_Item item = new PedidoRacao_Item();
            if (id > 0) item.IDPedidoRacao = id;
            item.Sequencia = seq + 1;
            item.Nucleo = nucleo;
            item.Galpao = galpao;
            item.Linhagem = linhagem;
            item.CodFormulaRacao = produto1.USERNumFormula;
            item.ProdCodEstr = cfr.ProdCodEstr;
            item.IDConfigFormulaRacao = codFormulaRacao;
            item.QtdeKg = qtdeKg;
            item.UltimoLoteGalpaoPorLinhagem = loteCompleto;
            item.IdadeLote = age;
            listaItens.Add(item);

            return View("PedidoRacaoNovo");
        }

        public ActionResult DeleteItemPedidoRacaoNovo(int sequencia)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista dos Itens
            var listaItens = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacao"];
            var listaItensDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacaoDelete"];
            PedidoRacao_Item item = listaItens
                .Where(w => w.Sequencia == sequencia).FirstOrDefault();
            listaItens.Remove(item);
            listaItensDelete.Add(item);

            if (item != null)
            {
                // Remove Adicionais do Item removido
                var listaItensAdicionais = ((List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                    Session["ListaAdicionaisPedidoRacao"]).Where(w => w.SeqItem == item.Sequencia).ToList();
                var listaItensAdicionaisDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                    Session["ListaAdicionaisPedidoRacaoDelete"];

                foreach (var adicional in listaItensAdicionais)
                {
                    listaItensAdicionaisDelete.Add(adicional);
                }
                listaItensAdicionais.RemoveAll(w => w.SeqItem == item.Sequencia);
            }

            return View("PedidoRacaoNovo");
        }

        #endregion

        #region Adicional - Novo

        [HttpPost]
        public ActionResult SaveAdicionalItemPedidoRacaoNovo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);

            string adicional = model["Adicional"];
            int sequenciaItem = Convert.ToInt32(model["sequenciaItem"]);
            decimal qtdeKgPorTon = Convert.ToDecimal(model["QtdeKgPorTon"]);

            var itemPR = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == id && w.Sequencia == sequenciaItem).FirstOrDefault();

            var listaAdicionais = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacao"];

            var listaAdicionaisItem = ((List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacao"]).Where(w => w.SeqItem == sequenciaItem).ToList();

            // Verifica nos itens do pedido vigente
            int existeVerifica = listaAdicionaisItem
                .Where(w => w.ProdCodEstr == adicional)
                .Count();

            if (existeVerifica > 0)
            {
                ImportaIncubacao.Data.Apolo.PRODUTO produto =
                    apoloService.PRODUTO.Where(w => w.ProdCodEstr == adicional).FirstOrDefault();

                ViewBag.Erro = "Adicional " + produto.ProdNome + " já cadastrado nesse Item! Verifique!";
                return View("PedidoRacaoNovo");
            }

            // Adiciona na Lista de Adicionais
            int? seq = hlbappSession.LOG_PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == id && w.SeqItem == sequenciaItem
                    && w.Origem == "Manual"
                    && id != 0).Max(m => m.Sequencia);
            if (seq == null)
                seq = listaAdicionaisItem.Where(w => w.Origem == "Manual").Max(m => m.Sequencia);
            if (seq == null) seq = 0;

            PedidoRacao_Item_Aditivo item = new PedidoRacao_Item_Aditivo();
            if (itemPR != null)
            {
                item.IDPedidoRacao = itemPR.IDPedidoRacao;
                item.IDPedidoRacao_Item = itemPR.ID;
            }
            item.Sequencia = seq + 1;
            item.SeqItem = sequenciaItem;
            item.ProdCodEstr = adicional;
            item.QtdeKgPorTon = qtdeKgPorTon;
            item.Origem = "Manual";
            listaAdicionais.Add(item);

            return View("PedidoRacaoNovo");
        }

        public ActionResult DeleteAdicionalItemPedidoRacaoNovo(int seqItem, int sequencia)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista dos Adicionais
            var listaItensAdicionais = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacao"];
            var listaItensAdicionaisDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item_Aditivo>)
                Session["ListaAdicionaisPedidoRacaoDelete"];
            PedidoRacao_Item_Aditivo adicional = listaItensAdicionais.Where(w => w.SeqItem == seqItem
                    && w.Sequencia == sequencia).FirstOrDefault();
            listaItensAdicionais.Remove(adicional);
            listaItensAdicionaisDelete.Add(adicional);

            return View("PedidoRacaoNovo");
        }

        #endregion

        #region Events Methods

        public void InsereLOGPedidoRacao(PedidoRacao pr, List<PedidoRacao_Item> listaItens,
            List<PedidoRacao_Item_Aditivo> listaAdicionaisGeral, List<PedidoRacao_Item> listaItensDelete,
            List<PedidoRacao_Item_Aditivo> listaAdicionaisGeralDelete, DateTime dataHoraOperacao, string usuario,
            string operacao, string observacao)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            #region LOG_PedidoRacao

            LOG_PedidoRacao lpr = new LOG_PedidoRacao();
            lpr.DataOperacao = dataHoraOperacao;
            lpr.UsuarioOperacao = usuario;
            lpr.Operacao = operacao;
            lpr.IDPedidoRacao = pr.ID;
            lpr.Empresa = pr.Empresa;
            lpr.DataInicial = pr.DataInicial;
            lpr.DataFinal = pr.DataFinal;
            lpr.StatusPedido = pr.StatusPedido;
            lpr.RotaEntregaCod = pr.RotaEntregaCod;
            lpr.Observacao = observacao;
            hlbappSession.LOG_PedidoRacao.AddObject(lpr);
            hlbappSession.SaveChanges();

            #endregion

            #region LOG_PedidoRacao_Item

            foreach (var item in listaItens)
            {
                string operacaoItem = "Inclusão";
                if (item.ID > 0) operacaoItem = "Alteração";

                LOG_PedidoRacao_Item lpri = new LOG_PedidoRacao_Item();
                lpri.IDLogPedidoRacao = lpr.ID;
                lpri.IDPedidoRacao = lpr.IDPedidoRacao;
                lpri.IDPedidoRacao_Item = item.ID;
                lpri.Nucleo = item.Nucleo;
                lpri.Galpao = item.Galpao;
                lpri.Linhagem = item.Linhagem;
                lpri.CodFormulaRacao = item.CodFormulaRacao;
                lpri.ProdCodEstr = item.ProdCodEstr;
                lpri.QtdeKg = item.QtdeKg;
                lpri.IDOrdemProducaoRacao = item.IDOrdemProducaoRacao;
                lpri.IDConfigFormulaRacao = item.IDConfigFormulaRacao;
                lpri.Sequencia = item.Sequencia;
                lpri.Operacao = operacaoItem;
                hlbappSession.LOG_PedidoRacao_Item.AddObject(lpri);
                hlbappSession.SaveChanges();

                #region LOG_PedidoRacao_Item_Aditivo

                var listaAdicionais = listaAdicionaisGeral
                    .Where(w => w.SeqItem == item.Sequencia).ToList();

                foreach (var add in listaAdicionais)
                {
                    string operacaoAdd = "Inclusão";
                    if (add.ID > 0) operacaoAdd = "Alteração";

                    LOG_PedidoRacao_Item_Aditivo ladd = new LOG_PedidoRacao_Item_Aditivo();
                    ladd.IDLogPedidoRacao = lpr.ID;
                    ladd.IDLogPedidoRacao_Item = lpri.ID;
                    ladd.IDPedidoRacao = lpr.IDPedidoRacao;
                    ladd.IDPedidoRacao_Item = lpri.IDPedidoRacao_Item;
                    ladd.IDPedidoRacao_Item_Aditivo = add.ID;
                    ladd.ProdCodEstr = add.ProdCodEstr;
                    ladd.QtdeKgPorTon = add.QtdeKgPorTon;
                    ladd.Sequencia = add.Sequencia;
                    ladd.SeqItem = add.SeqItem;
                    ladd.Operacao = operacaoAdd;
                    ladd.Origem = add.Origem;
                    hlbapp.LOG_PedidoRacao_Item_Aditivo.AddObject(ladd);
                }
                hlbapp.SaveChanges();

                #endregion

                #region LOG_PedidoRacao_Item_Aditivo - Delete

                var listaAdicionaisDelete = listaAdicionaisGeralDelete
                    .Where(w => w.IDPedidoRacao_Item == item.ID).ToList();

                foreach (var add in listaAdicionaisDelete)
                {
                    if (add.ID > 0)
                    {
                        LOG_PedidoRacao_Item_Aditivo ladd = new LOG_PedidoRacao_Item_Aditivo();
                        ladd.IDLogPedidoRacao = lpr.ID;
                        ladd.IDLogPedidoRacao_Item = lpri.ID;
                        ladd.IDPedidoRacao = add.IDPedidoRacao;
                        ladd.IDPedidoRacao_Item = add.IDPedidoRacao_Item;
                        ladd.IDPedidoRacao_Item_Aditivo = add.ID;
                        ladd.ProdCodEstr = add.ProdCodEstr;
                        ladd.QtdeKgPorTon = add.QtdeKgPorTon;
                        ladd.Sequencia = add.Sequencia;
                        ladd.SeqItem = add.SeqItem;
                        ladd.Operacao = "Exclusão";
                        ladd.Origem = add.Origem;
                        hlbapp.LOG_PedidoRacao_Item_Aditivo.AddObject(ladd);
                    }
                }
                hlbapp.SaveChanges();

                #endregion
            }

            hlbapp.SaveChanges();

            #endregion

            #region LOG_PedidoRacao_Item - Delete

            foreach (var item in listaItensDelete)
            {
                if (item.ID > 0)
                {
                    LOG_PedidoRacao_Item lpri = new LOG_PedidoRacao_Item();
                    lpri.IDLogPedidoRacao = lpr.ID;
                    lpri.IDPedidoRacao = lpr.IDPedidoRacao;
                    lpri.IDPedidoRacao_Item = item.ID;
                    lpri.Nucleo = item.Nucleo;
                    lpri.Galpao = item.Galpao;
                    lpri.Linhagem = item.Linhagem;
                    lpri.CodFormulaRacao = item.CodFormulaRacao;
                    lpri.ProdCodEstr = item.ProdCodEstr;
                    lpri.QtdeKg = item.QtdeKg;
                    lpri.IDOrdemProducaoRacao = item.IDOrdemProducaoRacao;
                    lpri.Sequencia = item.Sequencia;
                    lpri.Operacao = "Exclusão";
                    hlbappSession.LOG_PedidoRacao_Item.AddObject(lpri);
                    hlbappSession.SaveChanges();

                    #region LOG_PedidoRacao_Item_Aditivo - Delete

                    var listaAdicionais = listaAdicionaisGeralDelete
                        .Where(w => w.IDPedidoRacao_Item == item.ID).ToList();

                    foreach (var add in listaAdicionais)
                    {
                        if (add.ID > 0)
                        {
                            LOG_PedidoRacao_Item_Aditivo ladd = new LOG_PedidoRacao_Item_Aditivo();
                            ladd.IDLogPedidoRacao = lpr.ID;
                            ladd.IDLogPedidoRacao_Item = lpri.ID;
                            ladd.IDPedidoRacao = lpr.IDPedidoRacao;
                            ladd.IDPedidoRacao_Item = lpri.IDPedidoRacao_Item;
                            ladd.IDPedidoRacao_Item_Aditivo = add.ID;
                            ladd.ProdCodEstr = add.ProdCodEstr;
                            ladd.QtdeKgPorTon = add.QtdeKgPorTon;
                            ladd.Sequencia = add.Sequencia;
                            ladd.SeqItem = add.SeqItem;
                            ladd.Operacao = "Exclusão";
                            ladd.Origem = add.Origem;
                            hlbapp.LOG_PedidoRacao_Item_Aditivo.AddObject(ladd);
                        }
                    }
                    hlbapp.SaveChanges();

                    #endregion
                }
            }

            hlbapp.SaveChanges();

            #endregion
        }

        [HttpPost]
        public ActionResult AprovaAlteracaoPedidoRacaoNovo()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            Apolo10EntitiesService apoloSession = new Apolo10EntitiesService();

            #region Carrega Variáveis

            string granja = Session["granjaSelecionada"].ToString();
            string descricaoGranja = ((List<SelectListItem>)Session["ListaGranjas"])
                .Where(w => w.Value == granja).FirstOrDefault().Text;
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);

            var logPR = hlbappSession.LOG_PedidoRacao.Where(w => w.IDPedidoRacao == id)
                .OrderByDescending(o => o.DataOperacao).FirstOrDefault();

            string operacaoLog = "Inclusão Aprovada";
            if (logPR.Operacao.Contains("Alteração"))
                operacaoLog = "Alteração Aprovada";
            else if (logPR.Operacao.Contains("Exclusão"))
                operacaoLog = "Exclusão Aprovada";

            var listaLogItens = hlbappSession.LOG_PedidoRacao_Item
                .Where(w => w.IDLogPedidoRacao == logPR.ID).ToList();

            var listaLogAditivos = hlbappSession.LOG_PedidoRacao_Item_Aditivo
                .Where(w => w.IDLogPedidoRacao == logPR.ID).ToList();

            var pr = hlbappSession.PedidoRacao.Where(w => w.ID == id).FirstOrDefault();
            var listaItens = hlbappSession.PedidoRacao_Item.Where(w => w.IDPedidoRacao == id).ToList();
            var listaAdicionais = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == id).ToList();

            string stringChar = "<br />";
            string detalhesDataPedido = "";
            if (pr.DataInicial != logPR.DataInicial)
                detalhesDataPedido = "Data do Pedido alterada de "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " para "
                    + Convert.ToDateTime(logPR.DataInicial).ToShortDateString() + stringChar + stringChar;

            List<PedidoRacao_Item> listaItensDelete = new List<PedidoRacao_Item>();
            List<PedidoRacao_Item_Aditivo> listaAdicionaisDelete = new List<PedidoRacao_Item_Aditivo>();

            #endregion

            if (logPR.Operacao.Contains("Exclusão"))
            {
                #region Exclui o pedido

                #region PedidoRacao_Item_Aditivo

                foreach (var item in listaAdicionais)
                {
                    PedidoRacao_Item_Aditivo adicional = hlbappSession.PedidoRacao_Item_Aditivo
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    listaAdicionaisDelete.Add(adicional);
                    hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(adicional);
                }

                #endregion

                #region PedidoRacao_Item

                foreach (var item in listaItens)
                {
                    PedidoRacao_Item it = hlbappSession.PedidoRacao_Item
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    listaItensDelete.Add(it);
                    hlbappSession.PedidoRacao_Item.DeleteObject(it);
                }

                #endregion

                #region PedidoRacao

                hlbappSession.PedidoRacao.DeleteObject(pr);

                #endregion

                hlbappSession.SaveChanges();

                #endregion
            }
            else
            {
                #region Inclui / Altera o pedido

                #region PedidoRacao

                pr.DataInicial = logPR.DataInicial;
                pr.DataFinal = logPR.DataFinal;
                pr.StatusPedido = "Aberto";

                #endregion

                #region PedidoRacao_Item

                foreach (var logItem in listaLogItens)
                {
                    var item = listaItens.Where(w => w.ID == logItem.IDPedidoRacao_Item).FirstOrDefault();
                    if (item == null)
                    {
                        item = new PedidoRacao_Item();
                        item.IDPedidoRacao = pr.ID;
                    }

                    if (logItem.Operacao != "Exclusão")
                    {
                        item.Nucleo = logItem.Nucleo;
                        item.Galpao = logItem.Galpao;
                        item.Linhagem = logItem.Linhagem;
                        item.CodFormulaRacao = logItem.CodFormulaRacao;
                        item.ProdCodEstr = logItem.ProdCodEstr;
                        item.IDConfigFormulaRacao = logItem.IDConfigFormulaRacao;
                        item.QtdeKg = logItem.QtdeKg;
                        item.Sequencia = logItem.Sequencia;

                        if (item.ID == 0) hlbappSession.PedidoRacao_Item.AddObject(item);
                    }
                    else
                    {
                        if (item != null)
                        {
                            var listaAditivosDeleteItem = hlbappSession.PedidoRacao_Item_Aditivo
                                .Where(w => w.IDPedidoRacao_Item == item.ID).ToList();

                            foreach (var addItem in listaAditivosDeleteItem)
                            {
                                hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(addItem);
                            }

                            hlbappSession.SaveChanges();

                            hlbappSession.PedidoRacao_Item.DeleteObject(item);
                        }
                    }

                    hlbappSession.SaveChanges();

                    #region PedidoRacao_Item_Aditivo

                    var listaLogAdd = listaLogAditivos
                        .Where(w => w.IDLogPedidoRacao_Item == logItem.ID).ToList();

                    foreach (var logAdd in listaLogAdd)
                    {
                        var add = listaAdicionais
                            .Where(w => w.ID == logAdd.IDPedidoRacao_Item_Aditivo).FirstOrDefault();
                        if (add == null)
                        {
                            add = new PedidoRacao_Item_Aditivo();
                            add.IDPedidoRacao = pr.ID;
                            add.IDPedidoRacao_Item = item.ID;
                            add.SeqItem = logItem.Sequencia;
                        }

                        if (logAdd.Operacao != "Exclusão")
                        {
                            add.Origem = logAdd.Origem;
                            add.ProdCodEstr = logAdd.ProdCodEstr;
                            add.QtdeKgPorTon = logAdd.QtdeKgPorTon;
                            add.Sequencia = logAdd.Sequencia;

                            if (add.ID == 0) hlbappSession.PedidoRacao_Item_Aditivo.AddObject(add);
                        }
                        else
                        {
                            if (add != null)
                                hlbappSession.PedidoRacao_Item_Aditivo.DeleteObject(add);
                        }
                    }

                    #endregion
                }

                hlbappSession.SaveChanges();

                #endregion

                #endregion
            }

            #region Insere LOG

            listaItens = hlbappSession.PedidoRacao_Item.Where(w => w.IDPedidoRacao == id).ToList();
            listaAdicionais = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == id).ToList();

            InsereLOGPedidoRacao(pr, listaItens, listaAdicionais, listaItensDelete, listaAdicionaisDelete,
                DateTime.Now, Session["login"].ToString(), operacaoLog, "");

            #endregion

            #region Envia E-mail

            #region Carrega pedido para ir no corpo do e-mail

            string detalhesPedido = CarregaDetalhesCorpoEmail(listaItens, listaAdicionais,
                listaItensDelete, listaAdicionaisDelete);

            #endregion

            #region Gera o E-mail

            string assunto = "RAÇÃO - " + operacaoLog.ToUpper() + " FORA DE PERÍODO - "
                + Convert.ToDateTime(pr.DataInicial).ToString("dd/MM/yy")
                + " - " + descricaoGranja;
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "5";

            Apolo10Entities apolo = new Apolo10Entities();

            string login = Session["login"].ToString().ToUpper();
            USUARIO usuario = apolo.USUARIO.Where(w => w.UsuCod == login).FirstOrDefault();

            USUARIO usuarioDestino = apolo.USUARIO.Where(w => w.UsuCod == logPR.UsuarioOperacao).FirstOrDefault();

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            corpoEmail = "Prezado," + stringChar + stringChar
                + "A operação '" + operacaoLog + "' do pedido da granja " + descricaoGranja + " do dia "
                + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " foi realizada pelo usuário "
                + usuario.UsuNome + " em " + DateTime.Now + " conforme dados abaixo: " + stringChar + stringChar
                + detalhesDataPedido
                + detalhesPedido + stringChar + stringChar
                //+ "Clique no link a seguir para poder realizar a aprovação: "
                //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                + "SISTEMA WEB";

            EnviarEmail("FÁBRICA DE RAÇÃO",
                usuarioDestino.UsuEmail,
                //"palves@hyline.com.br","",
                "pedido.racao@hyline.com.br",
                assunto, corpoEmail, anexos, empresaApolo, "Html");

            #endregion

            #endregion

            ViewBag.Mensagem = logPR.Operacao + " do Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " aprovado com sucesso!";

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());
            return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
        }

        [HttpPost]
        public ActionResult ReprovaAlteracaoPedidoRacaoNovo(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            Apolo10EntitiesService apoloSession = new Apolo10EntitiesService();

            #region Carrega Variáveis

            string granja = Session["granjaSelecionada"].ToString();
            string descricaoGranja = ((List<SelectListItem>)Session["ListaGranjas"])
                .Where(w => w.Value == granja).FirstOrDefault().Text;
            int id = Convert.ToInt32(Session["IDPedidoRacao"]);
            string motivoReprovacao = model["motivoReprovacao"];

            var logPR = hlbappSession.LOG_PedidoRacao.Where(w => w.IDPedidoRacao == id)
                .OrderByDescending(o => o.DataOperacao).FirstOrDefault();

            var pr = hlbappSession.PedidoRacao.Where(w => w.ID == id).FirstOrDefault();

            string operacaoLog = "Inclusão Reprovada";
            if (logPR.Operacao.Contains("Alteração"))
                operacaoLog = "Alteração Reprovada";
            else if (logPR.Operacao.Contains("Exclusão"))
                operacaoLog = "Exclusão Reprovada";

            string stringChar = "<br />";
            string detalhesDataPedido = "";
            if (pr.DataInicial != logPR.DataInicial)
                detalhesDataPedido = "Data do Pedido alterada de "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " para "
                    + Convert.ToDateTime(logPR.DataInicial).ToShortDateString() + stringChar + stringChar;

            List<PedidoRacao_Item> listaItensDelete = new List<PedidoRacao_Item>();
            List<PedidoRacao_Item_Aditivo> listaAdicionaisDelete = new List<PedidoRacao_Item_Aditivo>();

            #endregion

            #region Deleta Pedido Caso seja inclusão

            if (logPR.Operacao.Contains("Inclusão"))
            {
                hlbappSession.PedidoRacao.DeleteObject(pr);
                hlbappSession.SaveChanges();

                var listaLogItens = hlbappSession.LOG_PedidoRacao_Item
                    .Where(w => w.IDLogPedidoRacao == logPR.ID).ToList();

                foreach (var item in listaLogItens)
                {
                    PedidoRacao_Item pri = new PedidoRacao_Item();
                    pri.Nucleo = item.Nucleo;
                    pri.Galpao = item.Galpao;
                    pri.Linhagem = item.Linhagem;
                    pri.CodFormulaRacao = item.CodFormulaRacao;
                    pri.ProdCodEstr = item.ProdCodEstr;
                    pri.QtdeKg = item.QtdeKg;
                    pri.IDOrdemProducaoRacao = item.IDOrdemProducaoRacao;
                    pri.Sequencia = item.Sequencia;
                    pri.IDOrdemProducaoRacao = item.IDOrdemProducaoRacao;
                    listaItensDelete.Add(pri);
                }

                var listaLogAdicionais = hlbappSession.LOG_PedidoRacao_Item_Aditivo
                    .Where(w => w.IDLogPedidoRacao == logPR.ID).ToList();

                foreach (var item in listaLogAdicionais)
                {
                    PedidoRacao_Item_Aditivo add = new PedidoRacao_Item_Aditivo();
                    add.ProdCodEstr = item.ProdCodEstr;
                    add.QtdeKgPorTon = item.QtdeKgPorTon;
                    add.Sequencia = item.Sequencia;
                    add.SeqItem = item.SeqItem;
                    listaAdicionaisDelete.Add(add);
                }
            }

            #endregion

            #region Insere LOG

            var listaItens = hlbappSession.PedidoRacao_Item.Where(w => w.IDPedidoRacao == id).ToList();
            var listaAdicionais = hlbappSession.PedidoRacao_Item_Aditivo
                .Where(w => w.IDPedidoRacao == id).ToList();

            InsereLOGPedidoRacao(pr, listaItens, listaAdicionais, listaItensDelete, listaAdicionaisDelete,
                DateTime.Now, Session["login"].ToString(), operacaoLog, motivoReprovacao);

            #endregion

            #region Envia E-mail

            #region Carrega pedido para ir no corpo do e-mail

            string detalhesPedido = CarregaDetalhesCorpoEmail(listaItens, listaAdicionais,
                listaItensDelete, listaAdicionaisDelete);

            #endregion

            #region Gera o E-mail

            string assunto = "RAÇÃO - " + operacaoLog.ToUpper() + " FORA DE PERÍODO REPROVADA - "
                + Convert.ToDateTime(pr.DataInicial).ToString("dd/MM/yy")
                + " - " + descricaoGranja;
            string corpoEmail = "";
            string anexos = "";
            string empresaApolo = "5";

            Apolo10Entities apolo = new Apolo10Entities();

            string login = Session["login"].ToString().ToUpper();
            USUARIO usuario = apolo.USUARIO.Where(w => w.UsuCod == login).FirstOrDefault();

            USUARIO usuarioDestino = apolo.USUARIO.Where(w => w.UsuCod == logPR.UsuarioOperacao).FirstOrDefault();

            //string porta = "";
            //if (Request.Url.Port != 80)
            //    porta = ":" + Request.Url.Port.ToString();

            if (logPR.Operacao.Contains("Alteração"))
                corpoEmail = "Prezado," + stringChar + stringChar
                    + "A alteração do pedido da granja " + descricaoGranja + " do dia "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " foi reprovada pelo usuário "
                    + usuario.UsuNome + " em " + DateTime.Now + " pelo seguinte motivo: "
                    + motivoReprovacao + "." + stringChar + stringChar
                    + "Os dados continuarão conforme dados abaixo: "
                    + stringChar + stringChar
                    + detalhesDataPedido
                    + detalhesPedido + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Qualquer dúvida, entrar em contato com o responsável pela fábrica de ração!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";
            else if (logPR.Operacao.Contains("Inclusão"))
                corpoEmail = "Prezado," + stringChar + stringChar
                    + "A inclusão do pedido da granja " + descricaoGranja + " do dia "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " foi reprovada pelo usuário "
                    + usuario.UsuNome + " em " + DateTime.Now + " pelo seguinte motivo: "
                    + motivoReprovacao + "." + stringChar + stringChar
                    + "O pedido será excluído." + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Qualquer dúvida, entrar em contato com o responsável pela fábrica de ração!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";
            else
                corpoEmail = "Prezado," + stringChar + stringChar
                    + "A exclusão do pedido da granja " + descricaoGranja + " do dia "
                    + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " foi reprovada pelo usuário "
                    + usuario.UsuNome + " em " + DateTime.Now + " pelo seguinte motivo: "
                    + motivoReprovacao + "." + stringChar + stringChar
                    + "O pedido será mantido conforme dados abaixo: "
                    + stringChar + stringChar + detalhesPedido + stringChar + stringChar
                    //+ "Clique no link a seguir para poder realizar a aprovação: "
                    //+ "http://" + Request.Url.Host + porta + "/RDV/AprovaRDVFechado?numRDV=" + numero.Value
                    + "Qualquer dúvida, entrar em contato com o responsável pela fábrica de ração!"
                    + stringChar + stringChar
                    + "SISTEMA WEB";

            EnviarEmail("FÁBRICA DE RAÇÃO",
                usuarioDestino.UsuEmail,
                //"palves@hyline.com.br", "",
                "pedido.racao@hyline.com.br",
                assunto, corpoEmail, anexos, empresaApolo, "Html");

            #endregion

            #endregion

            ViewBag.Mensagem = logPR.Operacao + " do Pedido de Ração da Granja " + descricaoGranja + " para entrega em "
                + Convert.ToDateTime(pr.DataInicial).ToShortDateString() + " reprovada com sucesso!";

            DateTime dataInicial = Convert.ToDateTime(Session["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(Session["dataFinal"].ToString());
            return View("Index", CarregarListaPedidoRacao(granja, dataInicial, dataFinal));
        }

        #endregion

        #region Logs

        public ActionResult LogPedidoRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["IDPedidoRacao"] = id;
            return View();
        }

        public ActionResult LogPedidoRacaoReturn()
        {
            return View("LogPedidoRacao");
        }

        public ActionResult LogPedidoRacaoItem(int idLog)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["IDLogPedidoRacao"] = idLog;
            return View();
        }

        #endregion

        #endregion

        #region Configuração da Fórmula de Ração

        public ActionResult ListaConfigFormulaRacao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        public void CarregaCFR(int id)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            Config_Formula_Racao cfr = hlbappSession.Config_Formula_Racao
                .Where(w => w.ID == id).FirstOrDefault();

            MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO1 produto = bdApolo
                .PRODUTO1.Where(w => w.ProdCodEstr == cfr.ProdCodEstr).FirstOrDefault();

            Session["formulaCFR"] = produto.USERNumFormula;
            AtualizaDDL(produto.USERNumFormula.ToString(), (List<SelectListItem>)Session["ListaFormulas"]);
            Session["descricaoCFR"] = cfr.Descricao;
            Session["ativaCFR"] = cfr.Ativa;
            Session["modeloCFR"] = cfr.Modelo;
            AtualizaDDL(cfr.Ativa.ToString(), (List<SelectListItem>)Session["ListaAtivaCFR"]);

            Session["ListaUnidadesConfigFormulaRacao"] = hlbappSession.Config_Formula_Racao_Unidade
                .Where(w => w.IDConfigFormulaRacao == id).ToList();
            Session["ListaGalpoesConfigFormulaRacao"] = hlbappSession.Config_Formula_Racao_Galpao
                .Where(w => w.IDConfigFormulaRacao == id).ToList();
            Session["ListaLinhagensConfigFormulaRacao"] = hlbappSession.Config_Formula_Racao_Linhagem
                .Where(w => w.IDConfigFormulaRacao == id).ToList();
            Session["ListaAdicionaisConfigFormulaRacao"] = hlbappSession.Config_Formula_Racao_Adicionais
                .Where(w => w.IDConfigFormulaRacao == id).ToList();
        }

        public ActionResult CreateConfigFormulaRacao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idCFR"] = 0;

            CarregaFormulas(0, 0);
            Session["ListaAtivaCFR"] = ListaAtivaCFR();
            Session["ListaGranjasCFROriginal"] = CarregaListaGranjasCFR();
            Session["ListaGranjasCFR"] = CarregaListaGranjasCFR();

            //CarregaListaNucleos(true, "CFR");
            CarregaListaNucleosPS(false, "CFR");
            Session["ListaGalpoesSelecionados"] = new List<SelectListItem>();

            Session["descricaoCFR"] = "";
            Session["formulaCFR"] = "";
            Session["ativaCFR"] = 1;
            Session["modeloCFR"] = "Por Galpão";

            Session["ListaUnidadesConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Unidade>();
            Session["ListaGalpoesConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Galpao>();
            Session["ListaLinhagensConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Linhagem>();
            Session["ListaAdicionaisConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Adicionais>();

            Session["ListaUnidadesConfigFormulaRacao"] = new List<Config_Formula_Racao_Unidade>();
            Session["ListaGalpoesConfigFormulaRacao"] = new List<Config_Formula_Racao_Galpao>();
            Session["ListaLinhagensConfigFormulaRacao"] = new List<Config_Formula_Racao_Linhagem>();
            Session["ListaAdicionaisConfigFormulaRacao"] = new List<Config_Formula_Racao_Adicionais>();

            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            Session["ListaAdicionaisCFR"] = CarregaAdicionais(false);
            Session["ListaAdicionaisCFROriginal"] = CarregaAdicionais(false);

            return View("ConfigFormulaRacao");
        }

        public ActionResult EditConfigFormulaRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["idCFR"] = id;

            CarregaFormulas(0, 0);
            Session["ListaAtivaCFR"] = ListaAtivaCFR();
            CarregaCFR(id);

            Session["ListaUnidadesConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Unidade>();
            Session["ListaGalpoesConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Galpao>();
            Session["ListaLinhagensConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Linhagem>();
            Session["ListaAdicionaisConfigFormulaRacaoDelete"] = new List<Config_Formula_Racao_Adicionais>();

            Session["ListaGranjasCFROriginal"] = CarregaListaGranjasCFR();
            Session["ListaGranjasCFR"] = CarregaListaGranjasCFR();

            //CarregaListaNucleos(false, "CFR");
            CarregaListaNucleosPS(false, "CFR");
            Session["ListaGalpoesSelecionados"] = new List<SelectListItem>();

            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            Session["ListaAdicionaisCFR"] = CarregaAdicionais(false);
            Session["ListaAdicionaisCFROriginal"] = CarregaAdicionais(false);

            return View("ConfigFormulaRacao");
        }

        [HttpPost]
        public ActionResult SaveConfigFormulaRacao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            Apolo10EntitiesService apoloSession = new Apolo10EntitiesService();

            int codFormula = Convert.ToInt32(Session["formulaCFR"]);
            string prodCodEstr = apoloSession.PRODUTO1.Where(w => w.USERNumFormula == codFormula).FirstOrDefault().ProdCodEstr;
            string modelo = Session["modeloCFR"].ToString();

            string descricao = Session["descricaoCFR"].ToString();
            int id = Convert.ToInt32(Session["idCFR"]);
            int ativa = Convert.ToInt32(Session["ativaCFR"]);

            #region Verifica as listas

            var listaUnidades = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacao"];
            var listaUnidadesDelete = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacaoDelete"];

            var listaGalpoes = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacao"];
            var listaGalpoesDelete = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacaoDelete"];

            if (modelo == "Por Granja")
            {
                if (listaUnidades.Count == 0 && ativa == 1)
                {
                    ViewBag.Erro = "Obrigatório relacionar pelo menos 01 granja!";
                    return View("ConfigFormulaRacao");
                }
            }
            else
            {
                if (listaGalpoes.Count == 0 && ativa == 1)
                {
                    ViewBag.Erro = "Obrigatório relacionar pelo menos 01 núcleo / galpão!";
                    return View("ConfigFormulaRacao");
                }
            }

            var listaLinhagens = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Linhagem>)
                Session["ListaLinhagensConfigFormulaRacao"];
            var listaLinhagensDelete = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Linhagem>)
                Session["ListaLinhagensConfigFormulaRacaoDelete"];

            if (listaLinhagens.Count == 0)
            {
                ViewBag.Erro = "Obrigatório relacionar pelo menos 01 linhagem!";
                return View("ConfigFormulaRacao");
            }

            var listaAdicionais = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Adicionais>)
                Session["ListaAdicionaisConfigFormulaRacao"];
            var listaAdicionaisDelete = (List<MvcAppHylinedoBrasilMobile.Models.Config_Formula_Racao_Adicionais>)
                Session["ListaAdicionaisConfigFormulaRacaoDelete"];

            // 30/07/2019 - Solicitação Fluig Nº 8366
            //if (listaAdicionais.Count == 0)
            //{
            //    ViewBag.Erro = "Obrigatório relacionar pelo menos 01 adicional!";
            //    return View("ConfigFormulaRacao");
            //}

            #endregion

            #region Insere no banco de dados

            #region Config_Formula_Racao

            Config_Formula_Racao cfr = new Config_Formula_Racao();
            if (id > 0) cfr = hlbappSession.Config_Formula_Racao.Where(w => w.ID == id).FirstOrDefault();

            cfr.ProdCodEstr = prodCodEstr;
            cfr.Descricao = descricao;
            cfr.Ativa = ativa;
            cfr.Modelo = Session["modeloCFR"].ToString();

            ImportaIncubacao.Data.Apolo.PRODUTO1 produtoApolo = apoloService.PRODUTO1.Where(w => w.ProdCodEstr == cfr.ProdCodEstr).FirstOrDefault();
            if (produtoApolo != null)
                cfr.NumFormula = produtoApolo.USERNumFormula;

            if (id == 0) hlbappSession.Config_Formula_Racao.AddObject(cfr);
            hlbappSession.SaveChanges();

            #endregion

            #region Config_Formula_Racao_Unidade

            foreach (var item in listaUnidadesDelete)
            {
                if (item.ID > 0)
                {
                    Config_Formula_Racao_Unidade unidade = hlbappSession.Config_Formula_Racao_Unidade
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Unidade.DeleteObject(unidade);
                }
            }

            foreach (var item in listaUnidades)
            {
                Config_Formula_Racao_Unidade unidade = new Config_Formula_Racao_Unidade();
                if (item.ID > 0)
                    unidade = hlbappSession.Config_Formula_Racao_Unidade
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                else
                    unidade.IDConfigFormulaRacao = cfr.ID;

                unidade.CodUnidade = item.CodUnidade;

                if (unidade.ID == 0) hlbappSession.Config_Formula_Racao_Unidade.AddObject(unidade);
            }

            #endregion

            #region Config_Formula_Racao_Galpao

            foreach (var item in listaGalpoesDelete)
            {
                if (item.ID > 0)
                {
                    Config_Formula_Racao_Galpao galpao = hlbappSession.Config_Formula_Racao_Galpao.Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Galpao.DeleteObject(galpao);
                }
            }

            foreach (var item in listaGalpoes)
            {
                Config_Formula_Racao_Galpao galpao = new Config_Formula_Racao_Galpao();
                if (item.ID > 0)
                    galpao = hlbappSession.Config_Formula_Racao_Galpao.Where(w => w.ID == item.ID).FirstOrDefault();
                else
                    galpao.IDConfigFormulaRacao = cfr.ID;

                galpao.CodNucleo = item.CodNucleo;
                galpao.NumGalpao = item.NumGalpao;

                if (galpao.ID == 0) hlbappSession.Config_Formula_Racao_Galpao.AddObject(galpao);
            }

            #endregion

            #region Config_Formula_Racao_Linhagem

            foreach (var item in listaLinhagensDelete)
            {
                if (item.ID > 0)
                {
                    Config_Formula_Racao_Linhagem linhagem = hlbappSession.Config_Formula_Racao_Linhagem
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Linhagem.DeleteObject(linhagem);
                }
            }

            foreach (var item in listaLinhagens)
            {
                Config_Formula_Racao_Linhagem linhagem = new Config_Formula_Racao_Linhagem();
                if (item.ID > 0)
                    linhagem = hlbappSession.Config_Formula_Racao_Linhagem
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                else
                    linhagem.IDConfigFormulaRacao = cfr.ID;

                linhagem.Linhagem = item.Linhagem;

                if (linhagem.ID == 0) hlbappSession.Config_Formula_Racao_Linhagem.AddObject(linhagem);
            }

            #endregion

            #region Config_Formula_Racao_Adicionais

            foreach (var item in listaAdicionaisDelete)
            {
                if (item.ID > 0)
                {
                    Config_Formula_Racao_Adicionais adicional = hlbappSession.Config_Formula_Racao_Adicionais
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Adicionais.DeleteObject(adicional);
                }
            }

            foreach (var item in listaAdicionais)
            {
                Config_Formula_Racao_Adicionais adicional = new Config_Formula_Racao_Adicionais();
                if (item.ID > 0)
                    adicional = hlbappSession.Config_Formula_Racao_Adicionais
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                else
                    adicional.IDConfigFormulaRacao = cfr.ID;

                adicional.ProdCodEstr = item.ProdCodEstr;
                adicional.QtdeKgPorTon = item.QtdeKgPorTon;

                if (adicional.ID == 0) hlbappSession.Config_Formula_Racao_Adicionais.AddObject(adicional);
            }

            #endregion

            hlbappSession.SaveChanges();

            #endregion

            ViewBag.Msg = "Configuração " + descricao + " salva com sucesso!";

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        public ActionResult DeleteConfigFormulaRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            Config_Formula_Racao cfr = hlbappSession.Config_Formula_Racao
                .Where(w => w.ID == id).FirstOrDefault();

            if (cfr != null)
            {
                #region Config_Formula_Racao

                hlbappSession.Config_Formula_Racao.DeleteObject(cfr);
                string descricao = cfr.Descricao;

                #endregion

                #region Config_Formula_Racao_Unidade

                List<Config_Formula_Racao_Unidade> listaUnidades = hlbappSession
                    .Config_Formula_Racao_Unidade.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaUnidades)
                {
                    Config_Formula_Racao_Unidade obj = hlbappSession.Config_Formula_Racao_Unidade
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Unidade.DeleteObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Galpoes

                List<Config_Formula_Racao_Galpao> listaGalpoes = hlbappSession
                    .Config_Formula_Racao_Galpao.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaGalpoes)
                {
                    Config_Formula_Racao_Galpao obj = hlbappSession.Config_Formula_Racao_Galpao
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Galpao.DeleteObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Linhagem

                List<Config_Formula_Racao_Linhagem> listaLinhagens = hlbappSession
                    .Config_Formula_Racao_Linhagem.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaLinhagens)
                {
                    Config_Formula_Racao_Linhagem obj = hlbappSession.Config_Formula_Racao_Linhagem
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Linhagem.DeleteObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Adicionais

                List<Config_Formula_Racao_Adicionais> listaAdicionais = hlbappSession
                    .Config_Formula_Racao_Adicionais.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaAdicionais)
                {
                    Config_Formula_Racao_Adicionais obj = hlbappSession.Config_Formula_Racao_Adicionais
                        .Where(w => w.ID == item.ID).FirstOrDefault();
                    hlbappSession.Config_Formula_Racao_Adicionais.DeleteObject(obj);
                }

                #endregion

                hlbappSession.SaveChanges();

                ViewBag.Msg = "Configuração " + descricao + " excluída com sucesso!";
            }

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        public ActionResult CopyConfigFormulaRacao(int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            Config_Formula_Racao cfr = hlbappSession.Config_Formula_Racao
                .Where(w => w.ID == id).FirstOrDefault();

            if (cfr != null)
            {
                #region Config_Formula_Racao

                Config_Formula_Racao cfrNew = new Config_Formula_Racao();
                cfrNew.Descricao = cfr.Descricao + " - COPIA";
                cfrNew.ProdCodEstr = cfr.ProdCodEstr;
                cfrNew.Modelo = cfr.Modelo;
                hlbappSession.Config_Formula_Racao.AddObject(cfrNew);
                hlbappSession.SaveChanges();

                string descricao = cfr.Descricao;

                #endregion

                #region Config_Formula_Racao_Unidade

                List<Config_Formula_Racao_Unidade> listaUnidades = hlbappSession
                    .Config_Formula_Racao_Unidade.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaUnidades)
                {
                    Config_Formula_Racao_Unidade obj = new Config_Formula_Racao_Unidade();
                    obj.IDConfigFormulaRacao = cfrNew.ID;
                    obj.CodUnidade = item.CodUnidade;
                    hlbappSession.Config_Formula_Racao_Unidade.AddObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Galpao

                List<Config_Formula_Racao_Galpao> listaGalpoes = hlbappSession.Config_Formula_Racao_Galpao.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaGalpoes)
                {
                    Config_Formula_Racao_Galpao obj = new Config_Formula_Racao_Galpao();
                    obj.IDConfigFormulaRacao = cfrNew.ID;
                    obj.CodNucleo = item.CodNucleo;
                    obj.NumGalpao = item.NumGalpao;
                    hlbappSession.Config_Formula_Racao_Galpao.AddObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Linhagem

                List<Config_Formula_Racao_Linhagem> listaLinhagens = hlbappSession
                    .Config_Formula_Racao_Linhagem.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaLinhagens)
                {
                    Config_Formula_Racao_Linhagem obj = new Config_Formula_Racao_Linhagem();
                    obj.IDConfigFormulaRacao = cfrNew.ID;
                    obj.Linhagem = item.Linhagem;
                    hlbappSession.Config_Formula_Racao_Linhagem.AddObject(obj);
                }

                #endregion

                #region Config_Formula_Racao_Adicionais

                List<Config_Formula_Racao_Adicionais> listaAdicionais = hlbappSession
                    .Config_Formula_Racao_Adicionais.Where(w => w.IDConfigFormulaRacao == id).ToList();
                foreach (var item in listaAdicionais)
                {
                    Config_Formula_Racao_Adicionais obj = new Config_Formula_Racao_Adicionais();
                    obj.IDConfigFormulaRacao = cfrNew.ID;
                    obj.ProdCodEstr = item.ProdCodEstr;
                    obj.QtdeKgPorTon = item.QtdeKgPorTon;
                    hlbappSession.Config_Formula_Racao_Adicionais.AddObject(obj);
                }

                #endregion

                hlbappSession.SaveChanges();

                ViewBag.Msg = "Configuração " + descricao + " copiada com sucesso!";
            }

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        #region Granja

        public ActionResult SaveUnidadeConfigFormulaRacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string granja = model["Granja"];

            // Adiciona na Lista de Unidades
            List<Config_Formula_Racao_Unidade> listaUnidades =
                (List<Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacao"];
            Config_Formula_Racao_Unidade unidade = new Config_Formula_Racao_Unidade();
            unidade.CodUnidade = granja;
            listaUnidades.Add(unidade);

            // Remove da Lista de Granjas
            List<SelectListItem> listaGranjas = (List<SelectListItem>)Session["ListaGranjasCFR"];
            SelectListItem item = listaGranjas.Where(w => w.Value == granja).FirstOrDefault();
            listaGranjas.Remove(item);

            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            return View("ConfigFormulaRacao");
        }

        public ActionResult DeleteUnidadeConfigFormulaRacao(string codUnidade)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista de Unidades
            List<Config_Formula_Racao_Unidade> listaUnidades =
                (List<Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacao"];
            List<Config_Formula_Racao_Unidade> listaUnidadesDelete =
                (List<Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacaoDelete"];
            Config_Formula_Racao_Unidade unidade = listaUnidades
                .Where(w => w.CodUnidade == codUnidade).FirstOrDefault();
            listaUnidades.Remove(unidade);
            listaUnidadesDelete.Add(unidade);

            // Adiciona na Lista de Granjas
            List<SelectListItem> listaGranjas = (List<SelectListItem>)Session["ListaGranjasCFR"];
            List<SelectListItem> listaGranjasOriginal = (List<SelectListItem>)Session["ListaGranjasCFROriginal"];
            SelectListItem item = listaGranjasOriginal.Where(w => w.Value == codUnidade).FirstOrDefault();
            listaGranjas.Add(item);
            Session["ListaGranjasCFR"] = listaGranjas.OrderBy(o => o.Text).ToList();

            AjustaLinhagemXGranja();

            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            return View("ConfigFormulaRacao");
        }

        #endregion

        #region Núcleo / Galpão

        public ActionResult SaveNucleoGalpaoConfigFormulaRacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string nucleo = model["Nucleo"];
            string galpao = model["Galpao"];

            // Adiciona na Lista de Núcleo / Galpão
            List<Config_Formula_Racao_Galpao> listaGalpao = (List<Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacao"];
            Config_Formula_Racao_Galpao galpaoObj = new Config_Formula_Racao_Galpao();
            galpaoObj.CodNucleo = nucleo;
            galpaoObj.NumGalpao = galpao;
            listaGalpao.Add(galpaoObj);

            if (ExisteNucleoGalpoesSelecionados(nucleo, ""))
            {
                // Remove da Lista de Núcleo / Galpão
                List<SelectListItem> listaNucleos = (List<SelectListItem>)Session["ListaNucleos"];
                SelectListItem item = listaNucleos.Where(w => w.Value == nucleo).FirstOrDefault();
                listaNucleos.Remove(item);
            }

            CarregaGalpoes(nucleo, "CPR");
            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            return View("ConfigFormulaRacao");
        }

        public bool ExisteNucleoGalpoesSelecionados(string nucleo, string numGalpao)
        {
            bool existe = false;
            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            //List<Lotes> listaLotes = RetornaListaLotes(nucleo);
            List<Lotes> listaLotes = RetornaListaLotesPS(nucleo);
            var galpoesAgrupados = listaLotes.GroupBy(g => g.Galpao).ToList();

            List<Config_Formula_Racao_Galpao> listaGalpaoSession = (List<Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacao"];
            var listaExisteGalpaoBD = hlbapp.Config_Formula_Racao_Galpao
                .Where(w => w.CodNucleo == nucleo && (w.NumGalpao == numGalpao || numGalpao == "")).ToList();
            var listaExisteGalpaoSession = listaGalpaoSession
                .Where(w => w.CodNucleo == nucleo && (w.NumGalpao == numGalpao || numGalpao == "") && w.ID == 0).ToList();

            int qtdeGalpoesTotal = galpoesAgrupados.Where(w => w.Key == numGalpao || numGalpao == "").Count();
            int qtdeGalpoesUtilizados = 0;
            foreach (var galpao in galpoesAgrupados)
            {
                qtdeGalpoesUtilizados = qtdeGalpoesUtilizados + listaExisteGalpaoBD.Where(w => w.NumGalpao == galpao.Key).Count();
                qtdeGalpoesUtilizados = qtdeGalpoesUtilizados + listaExisteGalpaoSession.Where(w => w.NumGalpao == galpao.Key).Count();
            }

            if (qtdeGalpoesTotal == qtdeGalpoesUtilizados) existe = true;

            return existe;
        }

        public ActionResult DeleteNucleoGalpaoConfigFormulaRacao(string nucleo, string galpao)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista de Núcleo / Galpão
            List<Config_Formula_Racao_Galpao> listaGalpoes = (List<Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacao"];
            List<Config_Formula_Racao_Galpao> listaGalpoesDelete = (List<Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacaoDelete"];
            Config_Formula_Racao_Galpao galpaoObj = listaGalpoes.Where(w => w.CodNucleo == nucleo && w.NumGalpao == galpao).FirstOrDefault();
            listaGalpoes.Remove(galpaoObj);
            listaGalpoesDelete.Add(galpaoObj);

            if (!ExisteNucleoGalpoesSelecionados(nucleo, ""))
            {
                // Adiciona Núcleo / Galpão
                List<SelectListItem> listaNucleos = (List<SelectListItem>)Session["ListaNucleos"];
                List<SelectListItem> listaNucleosOriginal = (List<SelectListItem>)Session["ListaNucleosOriginal"];
                SelectListItem item = listaNucleosOriginal.Where(w => w.Value == nucleo).FirstOrDefault();
                listaNucleos.Add(item);
                Session["ListaNucleos"] = listaNucleos.OrderBy(o => o.Text).ToList();
            }

            CarregaGalpoes(nucleo, "CPR");
            Session["ListaLinhagensCFR"] = CarregaListaLinhagensCFR();

            return View("ConfigFormulaRacao");
        }

        #endregion

        #region Linhagem

        public void AjustaLinhagemXGranja()
        {
            List<Config_Formula_Racao_Unidade> listaUnidades =
                (List<Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacao"];

            List<Config_Formula_Racao_Linhagem> listaLinhagens =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacao"];

            List<Config_Formula_Racao_Linhagem> listaLinhagensDelete =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacaoDelete"];

            // Carrega todos os lotes de todas as unidades selecionadas
            var listaLotes = new List<Lotes>();
            foreach (var unidade in listaUnidades)
            {
                //var listaRetorno = RetornaLotesGranja(unidade.CodUnidade);
                var listaRetorno = RetornaLotesGranjaPS(unidade.CodUnidade);
                foreach (var lote in listaRetorno)
                {
                    listaLotes.Add(lote);
                }
            }

            // Agrupa os lotes por linhagens
            var agrupaLinhagens = listaLotes
                .GroupBy(g => g.Linhagem).OrderBy(o => o.Key).ToList();

            var listaLinhagensAntigas = new List<Config_Formula_Racao_Linhagem>();
            foreach (var item in listaLinhagens)
            {
                listaLinhagensAntigas.Add(item);
            }

            foreach (var item in listaLinhagensAntigas)
            {
                var linhagem = agrupaLinhagens.Where(w => w.Key == item.Linhagem).FirstOrDefault();
                if (linhagem == null)
                {
                    listaLinhagensDelete.Add(item);
                    listaLinhagens.Remove(item);
                }
            }
        }

        public ActionResult SaveLinhagemConfigFormulaRacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string linha = model["Linhagem"];

            // Adiciona na Lista de Linhagem
            List<Config_Formula_Racao_Linhagem> listaLinhagens =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacao"];
            Config_Formula_Racao_Linhagem linhagem = new Config_Formula_Racao_Linhagem();
            linhagem.Linhagem = linha;
            listaLinhagens.Add(linhagem);

            // Remove da DDL de Linhagens
            List<SelectListItem> ddlLinhagens = (List<SelectListItem>)Session["ListaLinhagensCFR"];
            SelectListItem item = ddlLinhagens.Where(w => w.Value == linha).FirstOrDefault();
            ddlLinhagens.Remove(item);

            return View("ConfigFormulaRacao");
        }

        public ActionResult DeleteLinhagemConfigFormulaRacao(string linha)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista de Linhagens
            List<Config_Formula_Racao_Linhagem> listaLinhagens =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacao"];
            List<Config_Formula_Racao_Linhagem> listaLinhagensDelete =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacaoDelete"];
            Config_Formula_Racao_Linhagem linhagem = listaLinhagens
                .Where(w => w.Linhagem == linha).FirstOrDefault();
            listaLinhagens.Remove(linhagem);
            listaLinhagensDelete.Add(linhagem);

            // Adiciona na Dll de Linhagens
            List<SelectListItem> ddlLinhagens = (List<SelectListItem>)Session["ListaLinhagensCFR"];
            ddlLinhagens.Add(new SelectListItem { Text = linha, Value = linha, Selected = false });
            Session["ListaLinhagensCFR"] = ddlLinhagens.OrderBy(o => o.Text).ToList();

            return View("ConfigFormulaRacao");
        }

        #endregion

        #region Adicional

        public ActionResult SaveAdicionalConfigFormulaRacao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string adicionalSelecionado = model["Adicional"];
            decimal qtdeKgPorTon = Convert.ToDecimal(model["QtdeKgPorTon"]);

            // Adiciona na Lista de Adicionais
            List<Config_Formula_Racao_Adicionais> listaAdicionais =
                (List<Config_Formula_Racao_Adicionais>)Session["ListaAdicionaisConfigFormulaRacao"];
            Config_Formula_Racao_Adicionais adicional = new Config_Formula_Racao_Adicionais();
            adicional.ProdCodEstr = adicionalSelecionado;
            adicional.QtdeKgPorTon = qtdeKgPorTon;
            listaAdicionais.Add(adicional);

            // Remove da DDL de Adicionais
            List<SelectListItem> ddlAdicionais = (List<SelectListItem>)Session["ListaAdicionaisCFR"];
            SelectListItem item = ddlAdicionais.Where(w => w.Value == adicionalSelecionado).FirstOrDefault();
            ddlAdicionais.Remove(item);

            return View("ConfigFormulaRacao");
        }

        public ActionResult DeleteAdicionalConfigFormulaRacao(string adicional)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            // Remove da Lista de Adicionais
            List<Config_Formula_Racao_Adicionais> listaAdicionais =
                (List<Config_Formula_Racao_Adicionais>)Session["ListaAdicionaisConfigFormulaRacao"];
            List<Config_Formula_Racao_Adicionais> listaAdicionaisDelete =
                (List<Config_Formula_Racao_Adicionais>)Session["ListaAdicionaisConfigFormulaRacaoDelete"];
            Config_Formula_Racao_Adicionais adicionalObj = listaAdicionais
                .Where(w => w.ProdCodEstr == adicional).FirstOrDefault();
            listaAdicionais.Remove(adicionalObj);
            listaAdicionaisDelete.Add(adicionalObj);

            // Adiciona na Dll de Adicionais
            List<SelectListItem> ddlAdicionais = (List<SelectListItem>)Session["ListaAdicionaisCFR"];
            List<SelectListItem> ddlAdicionaisOriginal = (List<SelectListItem>)Session["ListaAdicionaisCFROriginal"];
            SelectListItem item = ddlAdicionaisOriginal.Where(w => w.Value == adicional).FirstOrDefault();
            item.Selected = false;
            ddlAdicionais.Add(item);
            Session["ListaAdicionaisCFR"] = ddlAdicionais.OrderBy(o => o.Text).ToList();

            return View("ConfigFormulaRacao");
        }

        #endregion

        #endregion

        #region Gerar Ordens de Produção

        public ActionResult GerarOrdemProducao()
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            return View();
        }

        [HttpPost]
        public ActionResult SaveGerarOrdemProducao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            #region Carrega variáveis

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            DateTime dataInicialProducao = Convert.ToDateTime(model["dataInicial"]);
            DateTime dataFinalProducao = Convert.ToDateTime(model["dataFinal"]);

            #endregion

            DateTime data = dataInicialProducao;

            while (data <= dataFinalProducao)
            {
                #region Deleta as Ordens Antigas

                var listaAdicional = hlbappSession.OrdemProducaoRacao_Adicional
                    .Where(w => hlbappSession.OrdemProducaoRacao
                        .Any(a => a.ID == w.IDOrdemProducaoRacao
                            && a.Status == "Aberto"
                            && a.Data == data))
                    .ToList();

                foreach (var item in listaAdicional)
                {
                    hlbappSession.OrdemProducaoRacao_Adicional.DeleteObject(item);
                }

                var listaFichaTecnica = hlbappSession.OrdemProducaoRacao_FichaTecnica
                    .Where(w => hlbappSession.OrdemProducaoRacao
                        .Any(a => a.ID == w.IDOrdemProducaoRacao
                            && a.Status == "Aberto"
                            && a.Data == data))
                    .ToList();

                foreach (var item in listaFichaTecnica)
                {
                    hlbappSession.OrdemProducaoRacao_FichaTecnica.DeleteObject(item);
                }

                var listaOP = hlbappSession.OrdemProducaoRacao
                    .Where(w => w.Data == data && w.Status == "Aberto").ToList();

                foreach (var item in listaOP)
                {
                    hlbappSession.OrdemProducaoRacao.DeleteObject(item);

                    #region Gera LOG da Exclusão

                    var listaPR = hlbapp.PedidoRacao
                        .Where(w => hlbapp.PedidoRacao_Item
                            .Any(a => a.IDPedidoRacao == w.ID
                                && a.IDOrdemProducaoRacao == item.ID))
                        .ToList();

                    foreach (var pr in listaPR)
                    {
                        var listaPRI = hlbapp.PedidoRacao_Item
                            .Where(w => w.IDPedidoRacao == pr.ID
                                && w.IDOrdemProducaoRacao == item.ID).ToList();

                        var listaPRIA = hlbapp.PedidoRacao_Item_Aditivo
                            .Where(w => w.IDPedidoRacao == pr.ID).ToList();

                        var listaPRIDelete = new List<PedidoRacao_Item>();
                        var ListaPRIADelete = new List<PedidoRacao_Item_Aditivo>();

                        InsereLOGPedidoRacao(pr, listaPRI, listaPRIA, listaPRIDelete, ListaPRIADelete,
                            DateTime.Now, Session["login"].ToString(), "Exclusão Ordem de Produção",
                            "Exclusão da Ordem de Produção " + item.ID.ToString());
                    }

                    #endregion
                }

                hlbappSession.SaveChanges();

                #endregion

                #region Gerar Nova OP

                string dataStr = data.ToString("yyyy-MM-dd");
                hlbappSession.Gera_Ordem_Producao_Pedido_Racao(dataStr);
                hlbappSession.SaveChanges();

                #endregion

                data = data.AddDays(1);
            }

            ViewBag.Msg = "Ordens de Produção do período " + dataInicialProducao.ToShortDateString()
                + " a " + dataFinalProducao.ToShortDateString() + " geradas com sucesso!";

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        #endregion

        #region Métodos p/ DropDown

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

                if (login.Equals("PALVES") || login.Equals("ADM"))
                    login = "RIOSOFT";

                var listaFiliais = bdApolo.EMPRESA_FILIAL
                    .Where(e => e.USERFLIPCod != null && e.USERFLIPCod != ""
                        && bdApolo.EMP_FIL_USUARIO.Any(u => u.UsuCod == login && u.EmpCod == e.EmpCod)
                        && (e.USERTipoUnidadeFLIP == "Granja" || e.USERTipoUnidadeFLIP == "Incubatório"))
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
                }
                //}

                Session["ListaGranjas"] = items;
            }
        }

        public List<SelectListItem> CarregaListaGranjasCFR()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string login = Session["login"].ToString().ToUpper();

            if (login.Equals("PALVES") || login.Equals("ADM"))
                login = "RIOSOFT";

            var listaFiliais = bdApolo.EMPRESA_FILIAL
                .Where(e => e.USERFLIPCod != null && e.USERFLIPCod != ""
                    && bdApolo.EMP_FIL_USUARIO.Any(u => u.UsuCod == login && u.EmpCod == e.EmpCod)
                    && (e.USERTipoUnidadeFLIP == "Granja"))
                .OrderBy(f => f.EmpNome)
                .ToList();

            foreach (var item in listaFiliais)
            {
                bool selected = false;
                if ((listaFiliais.IndexOf(item).Equals(0)) && (Session["granjaSelecionada"] == null))
                {
                    selected = true;
                    Session["granjaSelecionada"] = item.USERFLIPCod;
                }
                string codFLIP = item.USERFLIPCod;

                items.Add(new SelectListItem
                {
                    Text = codFLIP + " - " + item.EmpNome,
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
                ImportaIncubacao.Data.Apolo.ENTIDADE1 entidade1 =
                    apoloService.ENTIDADE1.Where(e1 => e1.EntCod == item.EntCod).FirstOrDefault();
                items.Add(new SelectListItem
                {
                    Text = entidade1.USERFLIPCodigo + " - " + item.EntNomeFant,
                    Value = entidade1.USERFLIPCodigo,
                    Selected = false
                });
            }

            return items.OrderBy(o => o.Text).ToList();
        }

        public List<Lotes> RetornaLotesGranja(string granja)
        {
            var listaLotes = new List<Lotes>();

            FLOCKSFarmsTableAdapter farms = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable farmsDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            farms.FillFarms(farmsDT);

            var listaNucleos = farmsDT.Where(f => f.FARM_ID.StartsWith(granja)).ToList();

            foreach (var nucleo in listaNucleos.Where(w => w.LOCATION == "PP").ToList())
            {
                Session["location"] = "PP";
                var retorno = RetornaListaLotes(nucleo.FARM_ID);
                foreach (var item in retorno)
                {
                    listaLotes.Add(item);
                }
            }

            foreach (var nucleo in listaNucleos.Where(w => w.LOCATION == "PG").ToList())
            {
                Session["location"] = "PG";
                var retorno = RetornaListaLotes(nucleo.FARM_ID);
                foreach (var item in retorno)
                {
                    listaLotes.Add(item);
                }
            }

            return listaLotes;
        }

        public List<SelectListItem> CarregaListaLinhagensCFR()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<Config_Formula_Racao_Linhagem> listaLinhagenSelecionadas =
                new List<Config_Formula_Racao_Linhagem>();

            List<Config_Formula_Racao_Unidade> listaUnidades =
                (List<Config_Formula_Racao_Unidade>)Session["ListaUnidadesConfigFormulaRacao"];

            List<Config_Formula_Racao_Galpao> listaGalpoes =
                (List<Config_Formula_Racao_Galpao>)Session["ListaGalpoesConfigFormulaRacao"];

            List<Config_Formula_Racao_Linhagem> listaLinhagens =
                (List<Config_Formula_Racao_Linhagem>)Session["ListaLinhagensConfigFormulaRacao"];

            if (listaUnidades.Count > 0)
            {
                foreach (var unidade in listaUnidades)
                {
                    //var listaLotes = RetornaLotesGranja(unidade.CodUnidade);
                    var listaLotes = RetornaLotesGranjaPS(unidade.CodUnidade);

                    var agrupaLinhagens = listaLotes
                        .GroupBy(g => g.Linhagem).OrderBy(o => o.Key).ToList();

                    foreach (var item in agrupaLinhagens)
                    {
                        if (listaLinhagens.Where(w => w.Linhagem == item.Key).Count() == 0)
                            listaLinhagenSelecionadas.Add(new Config_Formula_Racao_Linhagem
                            {
                                Linhagem = item.Key
                            });
                    }
                }

                var agrupaLinhagensSelecionadas = listaLinhagenSelecionadas
                    .GroupBy(g => g.Linhagem).OrderBy(o => o.Key).ToList();

                foreach (var item in agrupaLinhagensSelecionadas)
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.Key,
                        Value = item.Key,
                        Selected = false
                    });
                }
            }
            else if (listaGalpoes.Count > 0)
            {
                foreach (var galpao in listaGalpoes)
                {
                    //var listaLotes = RetornaListaLotes(galpao.CodNucleo);
                    var listaLotes = RetornaListaLotesPS(galpao.CodNucleo);
                    var listaLotesGalpao = listaLotes.Where(w => w.Galpao == galpao.NumGalpao).ToList();

                    var agrupaLinhagens = listaLotesGalpao
                        .GroupBy(g => g.Linhagem).OrderBy(o => o.Key).ToList();

                    foreach (var item in agrupaLinhagens)
                    {
                        if (listaLinhagens.Where(w => w.Linhagem == item.Key).Count() == 0)
                            listaLinhagenSelecionadas.Add(new Config_Formula_Racao_Linhagem
                            {
                                Linhagem = item.Key
                            });
                    }
                }

                var agrupaLinhagensSelecionadas = listaLinhagenSelecionadas
                    .GroupBy(g => g.Linhagem).OrderBy(o => o.Key).ToList();

                foreach (var item in agrupaLinhagensSelecionadas)
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.Key,
                        Value = item.Key,
                        Selected = false
                    });
                }
            }
            else
                items.Add(new SelectListItem
                {
                    Text = "(Necessário inserir pelo menos uma granja para listas as "
                        + "linhagens)",
                    Value = "",
                    Selected = false
                });

            return items.OrderBy(o => o.Text).ToList();
        }

        public List<SelectListItem> CarregaAdicionais(bool somenteAtivos)
        {
            Apolo10EntitiesService apoloServiceS = new Apolo10EntitiesService();

            var listaInsumos = apoloServiceS.PRODUTO
                .Where(w => w.ProdStat == "Ativado"
                    && apoloServiceS.PROD_GRUPO_SUBGRUPO.Any(s => s.ProdCodEstr == w.ProdCodEstr
                        && s.GrpProdCod == "003")
                    && apoloServiceS.PRODUTO1.Any(a => a.ProdCodEstr == w.ProdCodEstr
                        && ((a.USERStatusPedidoRacao == "Ativado" && somenteAtivos) || !somenteAtivos)))
                .OrderBy(o => o.ProdNome)
                .ToList();

            List<SelectListItem> listaInsumosDropBox = new List<SelectListItem>();

            foreach (var item in listaInsumos)
            {
                listaInsumosDropBox.Add(new SelectListItem
                {
                    Text = item.ProdNome,
                    Value = item.ProdCodEstr,
                    Selected = false
                });
            }

            return listaInsumosDropBox;
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

            Session["granjaSelecionada"] = granja;
            Session["ListaGranjas"] = granjas;
        }

        public void CarregaListaStatusPedidoRacao()
        {
            if (Session["usuario"].ToString() != "0")
            {
                List<SelectListItem> items = new List<SelectListItem>();

                items.Add(new SelectListItem { Text = "Aberto", Value = "Aberto", Selected = true });
                items.Add(new SelectListItem { Text = "Produzido", Value = "Produzido", Selected = false });

                Session["ListaStatusPedidosRacao"] = items;
            }
        }

        public void AtualizaStatusPedidoRacaoSelecionado(string statPedidoRacao)
        {
            List<SelectListItem> statusPedidoRacao = (List<SelectListItem>)Session["ListaStatusPedidosRacao"];

            foreach (var item in statusPedidoRacao)
            {
                if (item.Value == statPedidoRacao)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["StatPedidoRacaoSelecionado"] = statPedidoRacao;

            Session["ListaStatusPedidosRacao"] = statusPedidoRacao;
        }

        public void CarregaListaNucleos(bool todos, string origem)
        {
            {
                List<SelectListItem> items = new List<SelectListItem>();

                //nucleos.FillFarms(flip.FLOCKS1);

                FLOCKSFarmsTableAdapter farms = new FLOCKSFarmsTableAdapter();
                FLIPDataSetMobile.FLOCKSFarmsDataTable farmsDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
                farms.FillFarms(farmsDT);

                string granja = Session["granjaSelecionada"].ToString();
                if (origem == "CFR") granja = "";
                Session["location"] = "";
                string location = "";

                if (todos) granja = "";

                MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresa =
                    bdApolo.EMPRESA_FILIAL.Where(e => e.USERFLIPCod == granja
                        || bdApolo.EMP_FILIAL_CERTIFICACAO.Any(c => c.EmpCod == e.EmpCod
                            && c.EmpFilCertificNum == granja))
                    .FirstOrDefault();

                var lista = farmsDT.Where(f => f.FARM_ID.StartsWith(granja)).ToList();
                if (lista.Count == 0)
                    foreach (var item in farmsDT.ToList())
                    {
                        if (!item.IsTEXT_6Null())
                            if (item.TEXT_6 == granja)
                                lista.Add(item);
                    }

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
                        location = lista.FirstOrDefault().LOCATION;
                    }
                else
                {
                    location = lista.FirstOrDefault().LOCATION;
                }

                //foreach (var item in farmsDT.Where(f => f.FARM_ID.StartsWith(granja)).ToList())
                foreach (var item in lista)
                {
                    items.Add(new SelectListItem { Text = item.FARM_ID, Value = item.FARM_ID, Selected = false });
                }

                Session["ListaNucleos"] = items;
                var itemsOriginal = new List<SelectListItem>();
                foreach (var item in items)
                {
                    itemsOriginal.Add(new SelectListItem { Text = item.Text, Value = item.Value, Selected = item.Selected });
                }
                Session["ListaNucleosOriginal"] = itemsOriginal;
                Session["location"] = location;

                List<SelectListItem> itemsGalpoes = new List<SelectListItem>();

                itemsGalpoes.Add(new SelectListItem { Text = "01", Value = "01", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "02", Value = "02", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "03", Value = "03", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "04", Value = "04", Selected = false });

                Session["ListaGalpoes"] = itemsGalpoes;
            }
        }

        public void AtualizaNucleoSelecionado(string nucleo)
        {
            List<SelectListItem> nucleos = (List<SelectListItem>)Session["ListaNucleos"];

            foreach (var item in nucleos)
            {
                if (item.Value == nucleo)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaNucleos"] = nucleos;
            Session["nucleoSelecionadoPedidoRacao"] = nucleo;
        }

        public void AtualizaGalpaoSelecionado(string galpao)
        {
            List<SelectListItem> galpoes = (List<SelectListItem>)Session["ListaGalpoesSelecionados"];

            foreach (var item in galpoes)
            {
                if (item.Value == galpao)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaGalpoesSelecionados"] = galpoes;
            Session["galpaoSelecionadoPedidoRacao"] = galpao;
        }

        public void AtualizaLinhagemSelecionada(string linhagem)
        {
            List<SelectListItem> linhagens = (List<SelectListItem>)Session["ListaLinhagensSelecionadas"];

            foreach (var item in linhagens)
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

            Session["ListaLinhagensSelecionadas"] = linhagens;
            Session["linhagemSelecionadoPedidoRacao"] = linhagem;
        }

        public void AtualizaFormulaSelecionada(string formula)
        {
            List<SelectListItem> formulas = (List<SelectListItem>)Session["ListaFormulas"];

            foreach (var item in formulas)
            {
                if (item.Value == formula)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaFormulas"] = formulas;
        }

        public List<SelectListItem> CarregaFormulas(decimal henHouse, int idade)
        {
            var listaFormulas = bdApolo.PRODUTO
                .Where(w => w.ProdStat == "Ativado"
                    //&& bdApolo.GARANTIA.Any(g => g.GarCod == w.ProdGarCodVenda
                    //&& henHouse >= g.USERAprovOvosAveAlojMin
                    //&& henHouse <= g.USERAprovOvosAveAlojMax
                    //&& idade >= g.USERIdadeMinima
                    //&& idade <= g.USERIdadeMaxima
                    //   )
                    && bdApolo.PRODUTO1.Any(a => w.ProdCodEstr == a.ProdCodEstr
                    && a.USERNumFormula != null))
                .OrderBy(o => o.ProdNome)
                .ToList();

            List<SelectListItem> listaFormDropBox = new List<SelectListItem>();

            foreach (var item in listaFormulas)
            {
                MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO1 item1 =
                    bdApolo.PRODUTO1.Where(w => w.ProdCodEstr == item.ProdCodEstr)
                    .FirstOrDefault();

                //#region Excluir da lista caso já tenha configuração criada

                //HLBAPPEntities hlbappSession = new HLBAPPEntities();

                //int existeCFR = hlbappSession.Config_Formula_Racao
                //    .Where(w => w.ProdCodEstr == item1.ProdCodEstr).Count();

                //#endregion

                //if (existeCFR == 0)
                //{
                listaFormDropBox.Add(new SelectListItem
                {
                    Text = item1.USERNumFormula + " - " + item.ProdNome,
                    Value = item1.USERNumFormula.ToString(),
                    Selected = false
                });
                //}
            }

            Session["ListaFormulas"] = listaFormDropBox.OrderBy(o => int.Parse(o.Value)).ToList();

            return listaFormDropBox.OrderBy(o => int.Parse(o.Value)).ToList();
        }

        public List<SelectListItem> CarregaConfigFormulas(string granja, string linhagem)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var listaFormulas = hlbapp.Config_Formula_Racao
                .Where(w => w.Ativa == 1
                    && hlbapp.Config_Formula_Racao_Unidade
                        .Any(u => u.CodUnidade == granja && u.IDConfigFormulaRacao == w.ID)
                    && hlbapp.Config_Formula_Racao_Linhagem
                        .Any(l => l.Linhagem == linhagem && l.IDConfigFormulaRacao == w.ID))
                .ToList();

            List<SelectListItem> listaFormDropBox = new List<SelectListItem>();

            foreach (var item in listaFormulas)
            {
                //MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO1 item1 =
                //    bdApolo.PRODUTO1.Where(w => w.ProdCodEstr == item.ProdCodEstr)
                //    .FirstOrDefault();

                listaFormDropBox.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = item.ID.ToString(),
                    Selected = false
                });
            }

            return listaFormDropBox;
        }

        public List<SelectListItem> CarregaConfiguracaoFormulas(string granja, string linhagem)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var listaFormulas = hlbappSession.Config_Formula_Racao
                .Where(w =>
                    hlbappSession.Config_Formula_Racao_Unidade
                        .Any(a => a.IDConfigFormulaRacao == w.ID
                            && a.CodUnidade == granja)
                    &&
                    hlbappSession.Config_Formula_Racao_Linhagem
                        .Any(a => a.IDConfigFormulaRacao == w.ID
                            && a.Linhagem == linhagem))
                .ToList();

            List<SelectListItem> listaFormDropBox = new List<SelectListItem>();

            foreach (var item in listaFormulas)
            {
                MvcAppHylinedoBrasilMobile.Models.bdApolo.PRODUTO1 produto1 =
                    bdApolo.PRODUTO1.Where(w => w.ProdCodEstr == item.ProdCodEstr)
                    .FirstOrDefault();

                listaFormDropBox.Add(new SelectListItem
                {
                    Text = item.Descricao,
                    Value = produto1.USERNumFormula.ToString(),
                    Selected = false
                });
            }

            Session["ListaFormulas"] = listaFormDropBox.OrderBy(o => int.Parse(o.Value)).ToList();

            return listaFormDropBox.OrderBy(o => int.Parse(o.Value)).ToList();
        }

        public void CarregaInsumos()
        {
            Apolo10EntitiesService apoloServiceS = new Apolo10EntitiesService();

            var listaInsumos = apoloServiceS.PRODUTO
                .Where(w => w.ProdStat == "Ativado"
                    && apoloServiceS.PROD_GRUPO_SUBGRUPO.Any(s => s.ProdCodEstr == w.ProdCodEstr
                        && s.GrpProdCod == "003"))
                .OrderBy(o => o.ProdNome)
                .ToList();

            List<SelectListItem> listaInsumosDropBox = new List<SelectListItem>();

            foreach (var item in listaInsumos)
            {
                listaInsumosDropBox.Add(new SelectListItem
                {
                    Text = item.ProdNome,
                    Value = item.ProdCodEstr,
                    Selected = false
                });
            }

            Session["ListaAditivos"] = listaInsumosDropBox;
        }

        public List<SelectListItem> CarregaRotasEntrega(string granjaSelecionada, DateTime dataPedido)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            bdApoloEntities bdApoloSession = new bdApoloEntities();

            int idPedidoRacao = Convert.ToInt32(Session["IDPedidoRacao"]);

            var listaRotasEntrega = bdApoloSession.ROTA_ENTREGA
                .Where(w => bdApoloSession.ROTA_ENTREGA_CID_ENT.Any(a => a.RotaEntregaCod == w.RotaEntregaCod
                    && bdApoloSession.ENTIDADE1.Any(e => e.EntCod == a.EntCod
                        && e.USERFLIPCodigo == granjaSelecionada))
                    && ((w.RotaEntregaDomingo == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Sunday) ||
                        (w.RotaEntregaSegunda == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Monday) ||
                        (w.RotaEntregaTerca == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Tuesday) ||
                        (w.RotaEntregaQuarta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Wednesday) ||
                        (w.RotaEntregaQuinta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Thursday) ||
                        (w.RotaEntregaSexta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Friday) ||
                        (w.RotaEntregaSabado == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Saturday))
                    && w.RotaEntregaTipo == "Entrega")
                .ToList();

            List<SelectListItem> listaFormDropBox = new List<SelectListItem>();

            foreach (var item in listaRotasEntrega)
            {
                //int saldo = RetornaSaldoRotaNoDia(item, dataPedido, idPedidoRacao);

                //if (saldo > 0)
                //{
                listaFormDropBox.Add(new SelectListItem
                {
                    //Text = item.RotaEntregaNome + " - Saldo Disp.: " + saldo.ToString("0,0") + "kg",
                    Text = item.RotaEntregaNome,
                    Value = item.RotaEntregaCod,
                    Selected = false
                });
                //}
            }

            return listaFormDropBox;
        }

        public static List<SelectListItem> CarregaRotasEntregaStatic(string granjaSelecionada, DateTime dataPedido,
            int idPedidoRacao)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            bdApoloEntities bdApoloSession = new bdApoloEntities();

            var listaRotasEntrega = bdApoloSession.ROTA_ENTREGA
                .Where(w => bdApoloSession.ROTA_ENTREGA_CID_ENT.Any(a => a.RotaEntregaCod == w.RotaEntregaCod
                    && bdApoloSession.ENTIDADE1.Any(e => e.EntCod == a.EntCod
                        && e.USERFLIPCodigo == granjaSelecionada))
                    && ((w.RotaEntregaDomingo == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Sunday) ||
                        (w.RotaEntregaSegunda == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Monday) ||
                        (w.RotaEntregaTerca == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Tuesday) ||
                        (w.RotaEntregaQuarta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Wednesday) ||
                        (w.RotaEntregaQuinta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Thursday) ||
                        (w.RotaEntregaSexta == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Friday) ||
                        (w.RotaEntregaSabado == "Sim" && dataPedido.DayOfWeek == DayOfWeek.Saturday))
                    && w.RotaEntregaTipo == "Entrega")
                .ToList();

            List<SelectListItem> listaFormDropBox = new List<SelectListItem>();

            foreach (var item in listaRotasEntrega)
            {
                //int saldo = RetornaSaldoRotaNoDia(item, dataPedido, idPedidoRacao);

                //if (saldo > 0)
                //{
                listaFormDropBox.Add(new SelectListItem
                {
                    //Text = item.RotaEntregaNome + " - Saldo Disp.: " + saldo.ToString("0,0") + "kg",
                    Text = item.RotaEntregaNome,
                    Value = item.RotaEntregaCod,
                    Selected = false
                });
                //}
            }

            return listaFormDropBox;
        }

        public void AtualizaRotaSelecionada(string rota)
        {
            List<SelectListItem> rotas = (List<SelectListItem>)Session["ListaRotaEntregaPedidosRacao"];

            foreach (var item in rotas)
            {
                if (item.Value == rota)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["ListaRotaEntregaPedidosRacao"] = rotas;
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

        public List<SelectListItem> ListaAtivaCFR()
        {
            var ddlAtiva = new List<SelectListItem>();
            ddlAtiva.Add(new SelectListItem
            {
                Text = "Sim",
                Value = "1",
                Selected = false
            });
            ddlAtiva.Add(new SelectListItem
            {
                Text = "Não",
                Value = "0",
                Selected = false
            });

            return ddlAtiva;
        }

        #endregion

        #region Métodos de Lista

        public List<PedidoRacao> CarregarListaPedidoRacao(string Text, DateTime dataInicial, DateTime dataFinal)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            if (Text != "")
            {
                return hlbappSession.PedidoRacao
                    .Where(w => (w.Empresa == Text) &&
                        ((w.DataInicial >= dataInicial && w.DataInicial <= dataFinal)
                        || (w.DataFinal >= dataInicial && w.DataFinal <= dataFinal)))
                    .OrderBy(o => o.DataInicial).ThenBy(t => t.Empresa).ThenBy(t => t.Ordem)
                    .ToList();
            }
            else
            {
                List<SelectListItem> listaGranjas = (List<SelectListItem>)Session["ListaGranjas"];

                List<PedidoRacao> listaPR = new List<PedidoRacao>();

                foreach (var item in listaGranjas)
                {
                    List<PedidoRacao> listaPRGranja = hlbappSession.PedidoRacao
                        .Where(w => (w.Empresa == item.Value) &&
                            ((w.DataInicial >= dataInicial && w.DataInicial <= dataFinal)
                            || (w.DataFinal >= dataInicial && w.DataFinal <= dataFinal)))
                        .OrderBy(o => o.DataInicial).ThenBy(t => t.Empresa).ThenBy(t => t.Ordem)
                        .ToList();

                    foreach (var pr in listaPRGranja)
                    {
                        listaPR.Add(pr);
                    }
                }

                return listaPR.OrderBy(o => o.DataInicial).ThenBy(t => t.Empresa).ThenBy(t => t.Ordem)
                    .ToList();
            }
        }

        public ActionResult CarregarListaPedidoRacaoView(string Text, DateTime dataInicial, DateTime dataFinal)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            if (Session["usuario"] != null)
            {
                AtualizaGranjaSelecionada(Text);

                if (Session["dataInicial"] == null)
                {
                    Session["dataInicial"] = DateTime.Today.ToShortDateString();
                    Session["dataFinal"] = DateTime.Today.ToShortDateString();
                    dataInicial = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                    dataFinal = Convert.ToDateTime(DateTime.Today.ToShortDateString());
                }
                else
                {
                    Session["dataInicial"] = dataInicial;
                    Session["dataFinal"] = dataFinal;
                }

                return View("Index", CarregarListaPedidoRacao(Text, dataInicial, dataFinal));
            }
            else
            {
                return RedirectToAction("Login", "AccountMobile");
            }
        }

        public ActionResult CarregarListaConfigFormulaRacaoView(string descricaoCPR, string ativaCPR, string numFormulaCPR, string nucleoCPR)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["filtraDescricaoCPR"] = descricaoCPR;
            Session["filtraAtivaCPR"] = ativaCPR;
            Session["filtraNumFormulaCPR"] = numFormulaCPR;
            Session["filtraNucleoCPR"] = nucleoCPR;

            return View("ListaConfigFormulaRacao", FilterListaConfigFormulaRacao());
        }

        public List<Config_Formula_Racao> FilterListaConfigFormulaRacao()
        {
            #region Inicializa Variáveis de Sessão

            if (Session["filtraDescricaoCPR"] == null) Session["filtraDescricaoCPR"] = "";
            if (Session["filtraAtivaCPR"] == null) Session["filtraAtivaCPR"] = "Sim";
            if (Session["ListaAtivaCPR"] == null)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "(Todas)", Value = "(Todas)", Selected = false });
                items.Add(new SelectListItem { Text = "Sim", Value = "Sim", Selected = true });
                items.Add(new SelectListItem { Text = "Não", Value = "Não", Selected = false });
                Session["ListaAtivaCPR"] = items;
            }
            if (Session["filtraNumFormulaCPR"] == null) Session["filtraNumFormulaCPR"] = "";
            if (Session["filtraNucleoCPR"] == null) Session["filtraNucleoCPR"] = "";

            #endregion

            string descricao = Session["filtraDescricaoCPR"].ToString();
            string ativa = Session["filtraAtivaCPR"].ToString();
            AtualizaDDL(ativa, (List<SelectListItem>)Session["ListaAtivaCPR"]);
            string numFormula = Session["filtraNumFormulaCPR"].ToString();
            string nucleo = Session["filtraNucleoCPR"].ToString();
            int numFormulaInt = -1;
            if (numFormula != "") numFormulaInt = Convert.ToInt32(numFormula);

            return CarregarListaConfigFormulaRacao(descricao, ativa, numFormulaInt, nucleo);
        }

        public List<Config_Formula_Racao> CarregarListaConfigFormulaRacao(string descricao, string ativa, int numFormula, string nucleo)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            bdApoloEntities apoloSession = new bdApoloEntities();

            int ativaInt = -1;
            if (ativa == "Sim") ativaInt = 1;
            else if (ativa == "Não") ativaInt = 0;

            var lista01 = hlbappSession.Config_Formula_Racao
                .Where(w => w.Descricao.Contains(descricao)
                    && (w.Ativa == ativaInt || ativaInt == -1)
                    //&& (w.NumFormula == numFormula || numFormula == -1)
                    && (hlbappSession.Config_Formula_Racao_Galpao.Any(a => a.IDConfigFormulaRacao == w.ID
                        && a.CodNucleo.Contains(nucleo)) || nucleo == "")
                    )
                .OrderBy(o => o.NumFormula)
                .ToList();

            var listaCFR = new List<Config_Formula_Racao>();

            if (numFormula == -1)
            {
                listaCFR = lista01;
            }
            else
            {
                foreach (var item in lista01)
                {
                    int numFormulaApolo = 0;
                    Models.bdApolo.PRODUTO1 produto1 = apoloSession.PRODUTO1.Where(w => w.ProdCodEstr == item.ProdCodEstr).FirstOrDefault();
                    if (produto1 != null)
                    {
                        numFormulaApolo = (int)produto1.USERNumFormula;
                    }
                    if (numFormula == numFormulaApolo)
                        listaCFR.Add(item);
                }
            }

            return listaCFR;
        }

        #endregion

        #region Métodos Gerais

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

        public decimal LastHenHouse(string nucleo, int galpao, string linhagem,
            DateTime ultimaData)
        {
            FLIPDataSetMobile flipMobile = new FLIPDataSetMobile();
            FLOCK_DATAMobileTableAdapter flockData = new FLOCK_DATAMobileTableAdapter();

            return Convert.ToDecimal(flockData.HenHouse(nucleo, galpao, linhagem, ultimaData));
        }

        public decimal LastAge(string nucleo, int galpao, string linhagem,
            DateTime ultimaData)
        {
            FLIPDataSetMobile flipMobile = new FLIPDataSetMobile();
            FLOCK_DATAMobileTableAdapter flockData = new FLOCK_DATAMobileTableAdapter();

            return Convert.ToDecimal(flockData.Age(nucleo, galpao, linhagem, ultimaData));
        }

        public int RetornaSaldoRotaNoDia(ROTA_ENTREGA rota, DateTime data, int idPedidoAtual)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            MvcAppHylinedoBrasilMobile.Models.bdApolo.EQUIPAMENTO veiculo = bdApolo.EQUIPAMENTO
                    .Where(w => w.EquipVeicPlaca == rota.RotaEntregaObs).FirstOrDefault();

            int capacidade = 0;
            if (veiculo != null)
                capacidade = (Convert.ToInt32(veiculo.EquipVeicCapac.Replace(" Ton", "")) * 1000);

            int qtdSolicitada = 0;
            var listaPedidosRotaEntrega = hlbappSession.PedidoRacao_Item
                .Where(w => hlbappSession.PedidoRacao.Any(a => w.IDPedidoRacao == a.ID
                    && a.RotaEntregaCod == rota.RotaEntregaCod
                    && a.DataInicial == data
                    && a.ID != idPedidoAtual)).ToList();

            int saldo = 0;
            if (listaPedidosRotaEntrega != null)
                qtdSolicitada = Convert.ToInt32(listaPedidosRotaEntrega.Sum(s => s.QtdeKg));

            saldo = capacidade - qtdSolicitada;

            return saldo;
        }

        public int RetornaSaldoNoDia(DateTime data, string granja, int idItemPedidoAtual, int idPedidoAtual)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            bdApoloEntities bdApoloSession = new bdApoloEntities();

            #region Carrega Lista de Rotas de acordo com a granja e a data

            List<ROTA_ENTREGA> listaRotasEntrega = bdApoloSession.ROTA_ENTREGA
                .Where(w => bdApoloSession.ROTA_ENTREGA_CID_ENT.Any(a => a.RotaEntregaCod == w.RotaEntregaCod
                    && bdApoloSession.ENTIDADE1.Any(e => e.EntCod == a.EntCod
                        && e.USERFLIPCodigo == granja))
                    && ((w.RotaEntregaDomingo == "Sim" && data.DayOfWeek == DayOfWeek.Sunday) ||
                        (w.RotaEntregaSegunda == "Sim" && data.DayOfWeek == DayOfWeek.Monday) ||
                        (w.RotaEntregaTerca == "Sim" && data.DayOfWeek == DayOfWeek.Tuesday) ||
                        (w.RotaEntregaQuarta == "Sim" && data.DayOfWeek == DayOfWeek.Wednesday) ||
                        (w.RotaEntregaQuinta == "Sim" && data.DayOfWeek == DayOfWeek.Thursday) ||
                        (w.RotaEntregaSexta == "Sim" && data.DayOfWeek == DayOfWeek.Friday) ||
                        (w.RotaEntregaSabado == "Sim" && data.DayOfWeek == DayOfWeek.Saturday)))
                .ToList();

            #endregion

            #region Calcula Capacidade

            int capacidade = 0;
            foreach (var item in listaRotasEntrega)
            {
                MvcAppHylinedoBrasilMobile.Models.bdApolo.EQUIPAMENTO veiculo = bdApoloSession.EQUIPAMENTO
                    .Where(w => w.EquipVeicPlaca == item.RotaEntregaObs).FirstOrDefault();

                if (veiculo != null)
                    capacidade = capacidade + (Convert.ToInt32(veiculo.EquipVeicCapac.Replace(" Ton", "")) * 1000);
            }

            #endregion

            #region Calcula Qtde. Solicitada

            var listaEntidadeRota = bdApoloSession.ROTA_ENTREGA_CID_ENT
                .Where(a => bdApoloSession.ROTA_ENTREGA.Any(w => w.RotaEntregaCod == a.RotaEntregaCod
                            && bdApoloSession.ROTA_ENTREGA_CID_ENT.Any(b => b.RotaEntregaCod == w.RotaEntregaCod
                                && bdApoloSession.ENTIDADE1.Any(e => e.EntCod == b.EntCod
                                    && e.USERFLIPCodigo == granja))
                            && ((w.RotaEntregaDomingo == "Sim" && data.DayOfWeek == DayOfWeek.Sunday) ||
                                (w.RotaEntregaSegunda == "Sim" && data.DayOfWeek == DayOfWeek.Monday) ||
                                (w.RotaEntregaTerca == "Sim" && data.DayOfWeek == DayOfWeek.Tuesday) ||
                                (w.RotaEntregaQuarta == "Sim" && data.DayOfWeek == DayOfWeek.Wednesday) ||
                                (w.RotaEntregaQuinta == "Sim" && data.DayOfWeek == DayOfWeek.Thursday) ||
                                (w.RotaEntregaSexta == "Sim" && data.DayOfWeek == DayOfWeek.Friday) ||
                                (w.RotaEntregaSabado == "Sim" && data.DayOfWeek == DayOfWeek.Saturday))))
                .GroupBy(g => g.EntCod)
                .ToList();

            int qtdSolicitada = 0;

            foreach (var entidade in listaEntidadeRota)
            {
                MvcAppHylinedoBrasilMobile.Models.bdApolo.ENTIDADE1 entidade1 = bdApoloSession.ENTIDADE1
                    .Where(w => w.EntCod == entidade.Key).FirstOrDefault();

                var listaPedidos = hlbappSession.PedidoRacao_Item
                    .Where(w => hlbappSession.PedidoRacao.Any(a => w.IDPedidoRacao == a.ID
                        && a.Empresa == entidade1.USERFLIPCodigo
                        && a.DataInicial == data
                        //&& a.ID != idPedidoAtual
                        )
                        && w.ID != idItemPedidoAtual).ToList();

                qtdSolicitada = qtdSolicitada + Convert.ToInt32(listaPedidos.Sum(s => s.QtdeKg));
            }

            var listaItens = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                    Session["ListaItensPedidoRacao"];

            var listaItensDelete = (List<MvcAppHylinedoBrasilMobile.Models.PedidoRacao_Item>)
                Session["ListaItensPedidoRacaoDelete"];

            qtdSolicitada = qtdSolicitada
                + Convert.ToInt32(listaItens.Sum(s => s.QtdeKg))
                - Convert.ToInt32(listaItensDelete.Sum(s => s.QtdeKg));

            #endregion

            int saldo = 0;
            saldo = capacidade - qtdSolicitada;

            return saldo;
        }

        public static bool VerificaDataParaAlteracao(DateTime dataPedido, bool permiteAlterarPedido)
        {
            bool permissao = true;

            #region Permitir inserir pedido para próxima semana até sexta-feira

            DateTime diaAtual = DateTime.Today;
            DateTime diaAtualHora = DateTime.Now;

            int semanaAnoDataPedido = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                Convert.ToDateTime(dataPedido),
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            int semanaAnoDataAtual = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                Convert.ToDateTime(diaAtual),
                CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            int qtdDias = dataPedido.Subtract(diaAtual).Days;

            if ((
                 ((semanaAnoDataPedido - semanaAnoDataAtual) <= 1
                    && ((diaAtualHora.Hour > 12 && diaAtual.DayOfWeek == DayOfWeek.Friday)
                        || (diaAtual.DayOfWeek != DayOfWeek.Friday && qtdDias <= 3)))
                 ||
                 ((semanaAnoDataPedido - semanaAnoDataAtual) == 0)
                )
               && (!permiteAlterarPedido))
            {
                permissao = false;
            }

            #endregion

            return permissao;
        }

        public static bool VerificaAlteracaoEmpresaFilialApolo(string usuario, string empresa, DateTime dataPedido,
            bool permiteAlterarPedido, bool verificaData)
        {
            bool permissao = false;

            if (usuario != "ADM")
            {
                bdApoloEntities bdApoloSession = new bdApoloEntities();

                Apolo10EntitiesService apoloServiceSession = new Apolo10EntitiesService();

                MvcAppHylinedoBrasilMobile.Models.bdApolo.EMPRESA_FILIAL empresaFilial = bdApoloSession.EMPRESA_FILIAL
                    .Where(w => w.USERFLIPCod == empresa).FirstOrDefault();

                if (empresaFilial != null)
                {
                    EMP_FIL_USUARIO direitoUsuarioFilial = bdApoloSession.EMP_FIL_USUARIO
                        .Where(w => w.UsuCod == usuario && w.EmpCod == empresaFilial.EmpCod).FirstOrDefault();

                    if (direitoUsuarioFilial != null)
                        if (direitoUsuarioFilial.EmpFilUsuPermVerDet == "Sim")
                            permissao = true;
                }
                else
                {
                    CATEG_USUARIO categUsuario = apoloServiceSession.CATEG_USUARIO
                        .Where(w => w.CategCodEstr == "07.01" && w.UsuCod == usuario
                            && apoloServiceSession.ENT_CATEG.Any(c => w.CategCodEstr == c.CategCodEstr
                                && apoloServiceSession.ENTIDADE1.Any(e => e.EntCod == c.EntCod
                                    && e.USERFLIPCodigo == empresa)))
                        .FirstOrDefault();

                    if (categUsuario != null)
                        if (categUsuario.CategUsuDirAltera == "T")
                            permissao = true;
                }
            }
            else
                permissao = true;

            #region Permitir inserir pedido para próxima semana até sexta-feira

            if (verificaData)
                permissao = VerificaDataParaAlteracao(dataPedido, permiteAlterarPedido);

            #endregion

            return permissao;
        }

        public string CarregaDetalhesCorpoEmail(List<PedidoRacao_Item> listaItens,
            List<PedidoRacao_Item_Aditivo> listaAdicionais, List<PedidoRacao_Item> listaItensDelete,
            List<PedidoRacao_Item_Aditivo> listaAdicionaisDelete)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            string detalhesPedido = "";

            if (listaItens.Count > 0 || listaItensDelete.Count > 0)
                detalhesPedido =
                    "<table style=\"width: 100%; "
                        + "border-collapse: collapse; "
                        + "text-align: center;\">";

            foreach (var item in listaItens)
            {
                string operacao = "Inclusão";
                if (item.ID > 0) operacao = "Alteração";

                Config_Formula_Racao cfr = hlbappSession.Config_Formula_Racao
                    .Where(w => w.ID == item.IDConfigFormulaRacao).FirstOrDefault();

                string descricaoFormula = "";
                if (cfr != null) descricaoFormula = cfr.Descricao;

                detalhesPedido = detalhesPedido
                    + "<tr style=\"background: #333; "
                        + "color: white; "
                        + "font-weight: bold; "
                        + "border: 1px solid #fff; "
                        + "text-align: center;\">"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + operacao
                        + "</th>"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + item.Nucleo
                        + "</th>"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + "Galpão " + item.Galpao
                        + "</th>"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + item.Linhagem
                        + "</th>"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + descricaoFormula
                        + "</th>"
                        + "<th style=\"border: 1px solid #fff;\">"
                            + String.Format("{0:N0}", item.QtdeKg) + " Kg"
                        + "</th>"
                    + "</tr>";

                var listaAdd = listaAdicionais.Where(w => w.SeqItem ==
                    item.Sequencia).ToList();

                foreach (var add in listaAdd)
                {
                    string operacaoAdd = "Inclusão";
                    if (add.ID > 0) operacaoAdd = "Alteração";

                    ImportaIncubacao.Data.Apolo.PRODUTO produtoApolo = apoloService.PRODUTO
                        .Where(w => w.ProdCodEstr == add.ProdCodEstr).FirstOrDefault();

                    detalhesPedido = detalhesPedido
                        + "<tr>"
                            + "<td colspan=\"3\" style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + operacaoAdd
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + produtoApolo.ProdNome
                            + "</td>"
                            + "<td style=\"padding: 6px; "
                                + "border: 1px solid #ccc;\">"
                                    + String.Format("{0:N4}", add.QtdeKgPorTon) + " Kg/Ton"
                            + "</td>"
                        + "</tr>";
                }
            }

            foreach (var item in listaItensDelete)
            {
                if (item.ID > 0)
                {
                    string operacao = "Exclusão";

                    Config_Formula_Racao cfr = hlbappSession.Config_Formula_Racao
                        .Where(w => w.ID == item.IDConfigFormulaRacao).FirstOrDefault();

                    string descricaoFormula = "";
                    if (cfr != null) descricaoFormula = cfr.Descricao;

                    detalhesPedido = detalhesPedido
                        + "<tr style=\"background: #333; "
                            + "color: white; "
                            + "font-weight: bold; "
                            + "border: 1px solid #fff; "
                            + "text-align: center;\">"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + operacao
                            + "</th>"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + item.Nucleo
                            + "</th>"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + "Galpão " + item.Galpao
                            + "</th>"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + item.Linhagem
                            + "</th>"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + descricaoFormula
                            + "</th>"
                            + "<th style=\"border: 1px solid #fff;\">"
                                + String.Format("{0:N0}", item.QtdeKg) + " Kg"
                            + "</th>"
                        + "</tr>";

                    var listaAdd = listaAdicionaisDelete.Where(w => w.SeqItem ==
                        item.Sequencia).ToList();

                    foreach (var add in listaAdd)
                    {
                        if (add.ID > 0)
                        {
                            string operacaoAdd = "Exclusão";

                            ImportaIncubacao.Data.Apolo.PRODUTO produtoApolo = apoloService.PRODUTO
                                .Where(w => w.ProdCodEstr == add.ProdCodEstr).FirstOrDefault();

                            detalhesPedido = detalhesPedido
                                + "<tr>"
                                    + "<td colspan=\"3\" style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + operacaoAdd
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + produtoApolo.ProdNome
                                    + "</td>"
                                    + "<td style=\"padding: 6px; "
                                        + "border: 1px solid #ccc;\">"
                                            + String.Format("{0:N4}", add.QtdeKgPorTon) + " Kg/Ton"
                                    + "</td>"
                                + "</tr>";
                        }
                    }
                }
            }

            if (listaItens.Count > 0 || listaItensDelete.Count > 0)
                detalhesPedido = detalhesPedido + "</table>";

            return detalhesPedido;
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

        #endregion

        #region Métodos p/ JavaScript

        public List<Lotes> RetornaListaLotes(string nucleo)
        {
            FLOCKSFarmsTableAdapter farms = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable farmsDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            farms.FillFarms(farmsDT);

            var farmFLIP = farmsDT.Where(f => f.FARM_ID.StartsWith(nucleo)).FirstOrDefault();
            List<Lotes> listaLotes = new List<Lotes>();

            //string location = Session["location"].ToString();
            if (farmFLIP != null)
            {
                string location = farmFLIP.LOCATION;

                flocks.FillActivesByFarm(flip.FLOCKS, "HYBR", "BR", location, nucleo);

                List<FLIPDataSet.FLOCKSRow> flocksTable = flip.FLOCKS.Where(f => !f.FLOCK_ID.Contains("K")).ToList();

                for (int i = 0; i < flocksTable.Count; i++)
                {
                    string galpao = "";
                    if (!flocksTable[i].IsNUM_2Null())
                        galpao = flocksTable[i].NUM_2.ToString();
                    else
                        galpao = "(Existe algum lote sem o campo 'Núm. Galpão' preenchido no FLIP! Verifique!)";

                    listaLotes.Add(new Lotes
                    {
                        Granja = flocksTable[i].FARM_ID,
                        Linhagem = flocksTable[i].VARIETY,
                        LoteCompleto = flocksTable[i].FLOCK_ID,
                        NumeroLote = flocksTable[i].NUM_1.ToString(),
                        //DataNascimento = flocksTable[i].HATCH_DATE,
                        Location = flocksTable[i].LOCATION,
                        Galpao = galpao
                    });
                }
            }

            return listaLotes;
        }

        [HttpPost]
        public ActionResult CarregaGalpoes(string id, string origem)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            FLOCKSFarmsTableAdapter farms = new FLOCKSFarmsTableAdapter();
            FLIPDataSetMobile.FLOCKSFarmsDataTable farmsDT = new FLIPDataSetMobile.FLOCKSFarmsDataTable();
            farms.FillByFarmID(farmsDT, id);
            Session["location"] = "";
            if (farmsDT.Count > 0) Session["location"] = farmsDT[0].LOCATION;

            List<Lotes> listaLotes = RetornaListaLotes(id);
            List<SelectListItem> itemsGlp = new List<SelectListItem>();

            //var listaFarms = farmsDT.Where(w => w.IsTEXT_4Null() == false).ToList();
            //var galpoes = (listaFarms[0].TEXT_4).Split(',');

            var galpoesAgrupados = listaLotes
                .GroupBy(g => g.Galpao)
                .ToList();

            //foreach (var galpao in galpoes)
            foreach (var galpao in galpoesAgrupados)
            {
                if (origem == "CPR")
                {
                    if (!ExisteNucleoGalpoesSelecionados(id, galpao.Key))
                        itemsGlp.Add(new SelectListItem { Text = galpao.Key, Value = galpao.Key, Selected = false });
                }
                else
                    itemsGlp.Add(new SelectListItem { Text = galpao.Key, Value = galpao.Key, Selected = false });
            }

            itemsGlp = itemsGlp.OrderBy(o => o.Text).ToList();

            Session["listLotes"] = listaLotes;

            Session["ListaGalpoesSelecionados"] = itemsGlp;
            Session["nucleoSelecionadoPedidoRacao"] = id;

            // Comentar a atualização no processo novo.
            AtualizaNucleoSelecionado(id);

            return Json(itemsGlp);
        }

        [HttpPost]
        public ActionResult CarregaLinhagens(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            List<Lotes> listaLotes = (List<Lotes>)Session["listLotes"];
            List<SelectListItem> listaLinhagens = new List<SelectListItem>();

            var galpoesAgrupados = listaLotes
                .Where(w => w.Galpao == id && w.Galpao != "(Existe algum lote sem o campo 'Núm. Galpão' preenchido no FLIP! Verifique!)")
                .GroupBy(g => g.Linhagem)
                .ToList();

            foreach (var galpao in galpoesAgrupados)
            {
                listaLinhagens.Add(new SelectListItem { Text = galpao.Key, Value = galpao.Key, Selected = false });
            }

            Session["ListaLinhagensSelecionadas"] = listaLinhagens;
            Session["galpaoSelecionadoPedidoRacao"] = id;

            // Comentar a atualização no processo novo.
            AtualizaGalpaoSelecionado(id);

            return Json(listaLinhagens);
        }

        [HttpPost]
        public ActionResult RetornaCodigoProduto(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            //short codFormula = Convert.ToInt16(id.Substring(1, id.IndexOf("-") - 1).Replace(" ", ""));
            short codFormula = Convert.ToInt16(id);

            ImportaIncubacao.Data.Apolo.PRODUTO1 produto1 = apoloService.PRODUTO1
                .Where(w => w.USERNumFormula == codFormula)
                .FirstOrDefault();

            ImportaIncubacao.Data.Apolo.PRODUTO produto = apoloService.PRODUTO
                .Where(w => w.ProdCodEstr == produto1.ProdCodEstr)
                .FirstOrDefault();

            // Comentar a atualização no processo novo.
            AtualizaFormulaSelecionada(id);

            return Json(produto.ProdCodEstr);
        }

        [HttpPost]
        public ActionResult RetornaLinhagemSelecionada(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string nucleo = Session["nucleoSelecionadoPedidoRacao"].ToString();
            int galpao = Convert.ToInt32(Session["galpaoSelecionadoPedidoRacao"]);
            Session["linhagemSelecionadoPedidoRacao"] = id;
            string linhagem = Session["linhagemSelecionadoPedidoRacao"].ToString();
            DateTime data = Convert.ToDateTime(Session["dataFinalPedidoRacao"]);

            decimal henHouse = LastHenHouse(nucleo, galpao,
                linhagem, data);

            int age = Convert.ToInt32(LastAge(nucleo, galpao,
                linhagem, data));

            // Comentar a atualização no processo novo.
            AtualizaLinhagemSelecionada(id);

            return Json(CarregaFormulas(henHouse, age));
        }

        [HttpPost]
        public ActionResult RetornaLinhagemSelecionadaNovo(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string granja = Session["granjaSelecionada"].ToString();
            string nucleo = Session["nucleoSelecionadoPedidoRacao"].ToString();
            int galpao = Convert.ToInt32(Session["galpaoSelecionadoPedidoRacao"]);
            Session["linhagemSelecionadoPedidoRacao"] = id;
            string linhagem = Session["linhagemSelecionadoPedidoRacao"].ToString();
            DateTime data = Convert.ToDateTime(Session["dataFinalPedidoRacao"]);

            return Json(CarregaConfigFormulas(granja, linhagem));
        }

        [HttpPost]
        public ActionResult AtualizaSessionData(string id, string chamada)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";
            DateTime dataPedido = new DateTime();
            if (DateTime.TryParse(id, out dataPedido))
            {
                bool permiteAlterarPedido = MvcAppHylinedoBrasilMobile.Controllers.AccountMobileController
                    .GetGroup("HLBAPPM-PedidoRacaoAlteraPedido", (System.Collections.ArrayList)Session["Direitos"]);

                if (chamada == "1")
                    Session["dataInicialPedidoRacao"] = id;
                else if (chamada == "2")
                    Session["dataFinalPedidoRacao"] = id;
                else if (chamada == "3")
                    AtualizaStatusPedidoRacaoSelecionado(id);
                else if (chamada == "4")
                {
                    Session["dataInicialPedidoRacao"] = id;
                    permiteAlterarPedido = false;
                }

                #region Permitir inserir pedido para próxima semana até sexta-feira

                if (!VerificaDataParaAlteracao(dataPedido, permiteAlterarPedido))
                {
                    if (chamada == "4")
                        msgRetorno = "Obrigatório informar o motivo da alteração pois a data informada está fora do"
                        + " prazo permitido!";
                    else
                        msgRetorno = "Não pode ser gerado pedido nesse dia, pois o prazo é gerar pedido"
                            + " para a próxima semana até às 12h00 da sexta-feira da semana anterior!";

                    Session["dentroPeriodoPermitido"] = "False";
                }
                else
                    Session["dentroPeriodoPermitido"] = "True";

                #endregion
            }
            else
            {
                msgRetorno = "Obrigatório informar data do pedido!";
            }

            return Json(msgRetorno);
        }

        [HttpPost]
        public ActionResult RetornaQtdeKg(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";

            if (id == "")
            {
                msgRetorno = "A quantidade não pode ser em branco!";
                return Json(msgRetorno);
            }

            if (Convert.ToDecimal(id) == 0)
            {
                msgRetorno = "A quantidade não pode ser zerada!";
                return Json(msgRetorno);
            }

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            int idPR = Convert.ToInt32(Session["IDPedidoRacao"]);
            int idItemPR = Convert.ToInt32(Session["IDPedidoRacaoItem"]);
            string granja = Session["granjaSelecionada"].ToString();

            PedidoRacao pedido = hlbappSession.PedidoRacao.Where(w => w.ID == idPR).FirstOrDefault();

            DateTime dataPedido = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);

            PedidoRacao_Item item = hlbappSession.PedidoRacao_Item
                .Where(w => w.IDPedidoRacao == idPR && w.ID == idItemPR).FirstOrDefault();

            if (item != null)
            {
                ROTA_ENTREGA rota = bdApolo.ROTA_ENTREGA
                    .Where(w => w.RotaEntregaCod == pedido.RotaEntregaCod).FirstOrDefault();

                //int saldoRota = RetornaSaldoRotaNoDia(rota, dataPedido, idPR);
                int saldoRota = RetornaSaldoNoDia(dataPedido, granja, idItemPR, idPR);

                if ((saldoRota - Convert.ToDecimal(id)) >= 0)
                {
                    item.QtdeKg = Convert.ToDecimal(id);
                    hlbappSession.SaveChanges();
                }
                else
                {
                    msgRetorno = "Não existe saldo na rota selecionada para este pedido com a qtde informada!"
                        + " (Saldo Disponível: " + saldoRota.ToString("0,00") + ") Verifique!";
                }
            }

            return Json(msgRetorno);
        }

        [HttpPost]
        public ActionResult RetornaQtdeKgNovo(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";

            if (id == "")
            {
                msgRetorno = "A quantidade não pode ser em branco!";
                return Json(msgRetorno);
            }

            if (Convert.ToDecimal(id) == 0)
            {
                msgRetorno = "A quantidade não pode ser zerada!";
                return Json(msgRetorno);
            }

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            int idPR = Convert.ToInt32(Session["IDPedidoRacao"]);
            int idItemPR = Convert.ToInt32(Session["IDPedidoRacaoItem"]);
            string granja = Session["granjaSelecionada"].ToString();

            DateTime dataPedido = Convert.ToDateTime(Session["dataInicialPedidoRacao"]);

            int saldoRota = RetornaSaldoNoDia(dataPedido, granja, idItemPR, idPR);

            if ((saldoRota - Convert.ToDecimal(id)) < 0)
            {
                msgRetorno = "Não existe saldo na rota selecionada para este pedido com a qtde informada!"
                    + " (Saldo Disponível: " + saldoRota.ToString("0,00") + ") Verifique!";
            }

            return Json(msgRetorno);
        }

        [HttpPost]
        public ActionResult RetornaRotaEntrega(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            int idPR = Convert.ToInt32(Session["IDPedidoRacao"]);

            PedidoRacao item = hlbappSession.PedidoRacao
                .Where(w => w.ID == idPR).FirstOrDefault();

            if (item != null)
            {
                item.RotaEntregaCod = id;
                hlbappSession.SaveChanges();
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult RetornaRotaEntregaLista(string rota, int idPR)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            bdApoloEntities bdApoloSession = new bdApoloEntities();

            PedidoRacao item = hlbappSession.PedidoRacao
                .Where(w => w.ID == idPR).FirstOrDefault();

            int qtdPedido = Convert.ToInt32(hlbapp.PedidoRacao_Item.Where(w => w.IDPedidoRacao == idPR).ToList()
                .Sum(s => s.QtdeKg));

            ROTA_ENTREGA rotaObj = bdApoloSession.ROTA_ENTREGA
                .Where(w => w.RotaEntregaCod == rota).FirstOrDefault();

            if (rotaObj != null)
            {
                int saldo = RetornaSaldoRotaNoDia(rotaObj, Convert.ToDateTime(item.DataInicial), idPR);

                if ((saldo - qtdPedido) >= 0)
                {
                    item.RotaEntregaCod = rota;
                    hlbappSession.SaveChanges();
                }
                else
                {
                    msgRetorno = "Saldo indisponível para essa rota nesse dia (Saldo: "
                        + saldo.ToString("0,00") + ").";
                }
            }
            else
            {
                item.RotaEntregaCod = rota;
                hlbappSession.SaveChanges();
            }

            return Json(msgRetorno);
        }

        [HttpPost]
        public ActionResult AtualizaRotaEntrega(string id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            DateTime data = Convert.ToDateTime(id);
            string granja = Session["granjaSelecionada"].ToString();

            List<SelectListItem> listaRotaEntrega = new List<SelectListItem>();

            listaRotaEntrega = CarregaRotasEntrega(granja, data);

            Session["ListaRotaEntregaPedidosRacao"] = listaRotaEntrega;

            return Json(listaRotaEntrega);
        }

        [HttpPost]
        public ActionResult AtualizaSession(string valor, string campo)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";

            Session[campo] = valor;
            if (campo == "formulaCFR")
                AtualizaDDL(valor, (List<SelectListItem>)Session["ListaFormulas"]);
            if (campo == "ativaCFR")
                AtualizaDDL(valor, (List<SelectListItem>)Session["ListaAtivaCFR"]);

            return Json(msgRetorno);
        }

        [HttpPost]
        public ActionResult AtualizaOrdemPR(int ordem, int id)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            string msgRetorno = "";

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var pr = hlbapp.PedidoRacao.Where(w => w.ID == id).FirstOrDefault();
            pr.Ordem = ordem;
            hlbapp.SaveChanges();

            return Json(msgRetorno);
        }

        #endregion

        #region Métodos Consulta Poultry Suite

        public void Test()
        {
            //var serviceRoot = "https://bc-api.poultry-suite.com/PoultrySuite-Webservice/ODataV4/";
            var context = new PS.NAV.NAV(new Uri(serviceRoot));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            context.BuildingRequest += Context_BuildingRequest;

            var data = context.FlocksList.AddQueryOption("$filter", "Status eq 'Active'");
            foreach (var flock in data)
            {
                Console.WriteLine("{0} {1}", flock.Farm_Code, flock.No, flock.Status);
            }
        }

        public List<PS.NAV.FlocksList> ListaLotesAtivosPS(string granja, string nucleo)
        {
            var context = new PS.NAV.NAV(new Uri(serviceRoot));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            context.BuildingRequest += Context_BuildingRequest;

            var filtro = "";
            if (granja != "")
            {
                // Troca porque TP é Tabapuã no Apolo e os núcleos do Poultry Suite começam como TB.
                if (granja == "TP") granja = "TB";
                filtro = " and startswith(Farm_Code, '" + granja + "')";
            }
            else if (nucleo != "")
                filtro = " and Farm_Code eq '" + nucleo + "'";

            var data = context.FlocksList.AddQueryOption("$filter", "(Status eq 'Planning Confirmed' or Status eq 'Active')" + filtro);
            return data.ToList();
        }

        private void Context_BuildingRequest(object sender, Microsoft.OData.Client.BuildingRequestEventArgs e)
        {
            e.RequestUri = new Uri(e.RequestUri.ToString().Replace("V4/", "V4/Company('5440f05b-2eb9-eb11-a81d-005056bf7648')/"));
            e.Headers.Add("Authorization", "Basic RVdcUFNfQkNfQVBJX0xBVEFNOnJDMjhndjl0SypUWCFYdTY=");
        }

        public void CarregaListaNucleosPS(bool todos, string origem)
        {
            {
                List<SelectListItem> items = new List<SelectListItem>();

                string granja = Session["granjaSelecionada"].ToString();
                if (origem == "CFR") granja = "";
                Session["location"] = "";
                string location = "";

                if (todos) granja = "";

                var listaLotesPS = ListaLotesAtivosPS(granja, "");
                var lista = listaLotesPS.GroupBy(g => g.Farm_Code).OrderBy(o => o.Key).ToList();

                foreach (var item in lista)
                {
                    items.Add(new SelectListItem { Text = item.Key, Value = item.Key, Selected = false });
                }

                Session["ListaNucleos"] = items;
                var itemsOriginal = new List<SelectListItem>();
                foreach (var item in items)
                {
                    itemsOriginal.Add(new SelectListItem { Text = item.Text, Value = item.Value, Selected = item.Selected });
                }
                Session["ListaNucleosOriginal"] = itemsOriginal;
                Session["location"] = location;

                List<SelectListItem> itemsGalpoes = new List<SelectListItem>();

                itemsGalpoes.Add(new SelectListItem { Text = "01", Value = "01", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "02", Value = "02", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "03", Value = "03", Selected = false });
                itemsGalpoes.Add(new SelectListItem { Text = "04", Value = "04", Selected = false });

                Session["ListaGalpoes"] = itemsGalpoes;
            }
        }

        public List<Lotes> RetornaListaLotesPS(string nucleo)
        {
            var listaLotesPS = ListaLotesAtivosPS("", nucleo);
            var lista = listaLotesPS.Where(w => w.Farm_Code.StartsWith(nucleo)).OrderBy(o => o.No).ToList();

            List<Lotes> listaLotes = new List<Lotes>();

            if (lista != null)
            {
                for (int i = 0; i < lista.Count; i++)
                {
                    var varietyCode = lista[i].Variety_Code.ToString().Replace("L3","L1");
                    var linhagem = hlbapp.LINHAGEM_GRUPO.Where(w => w.LinhagemPS == varietyCode).FirstOrDefault();

                    listaLotes.Add(new Lotes
                    {
                        Granja = lista[i].Farm_Code,
                        Linhagem = (linhagem != null ? linhagem.LinhagemFLIP : "(Linhagem do Poultry Suite não configurada!"),
                        LoteCompleto = lista[i].No,
                        NumeroLote = lista[i].OriginalFlockID,
                        Galpao = Convert.ToInt32(lista[i].Location_Code.Substring(lista[i].Location_Code.IndexOf("-") + 1)).ToString()
                    });
                }
            }

            return listaLotes;
        }

        public List<Lotes> RetornaLotesGranjaPS(string granja)
        {
            var listaLotes = new List<Lotes>();

            var listaLotesPS = ListaLotesAtivosPS(granja, "");
            var listaNucleos = listaLotesPS.GroupBy(g => g.Farm_Code).OrderBy(o => o.Key).ToList();

            foreach (var nucleo in listaNucleos)
            {
                var retorno = RetornaListaLotesPS(nucleo.Key);
                foreach (var item in retorno)
                {
                    listaLotes.Add(item);
                }
            }

            return listaLotes;
        }

        [HttpPost]
        public ActionResult CarregaGalpoesPS(string id, string origem)
        {
            if (VerificaSessao()) return RedirectToAction("Login", "AccountMobile");

            Session["location"] = "";
            
            List<Lotes> listaLotes = RetornaListaLotesPS(id);
            List<SelectListItem> itemsGlp = new List<SelectListItem>();

            var galpoesAgrupados = listaLotes.GroupBy(g => g.Galpao).ToList();

            foreach (var galpao in galpoesAgrupados)
            {
                if (origem == "CPR")
                {
                    if (!ExisteNucleoGalpoesSelecionados(id, galpao.Key))
                        itemsGlp.Add(new SelectListItem { Text = galpao.Key, Value = galpao.Key, Selected = false });
                }
                else
                    itemsGlp.Add(new SelectListItem { Text = galpao.Key, Value = galpao.Key, Selected = false });
            }

            itemsGlp = itemsGlp.OrderBy(o => o.Text).ToList();

            Session["listLotes"] = listaLotes;

            Session["ListaGalpoesSelecionados"] = itemsGlp;
            Session["nucleoSelecionadoPedidoRacao"] = id;

            // Comentar a atualização no processo novo.
            AtualizaNucleoSelecionado(id);

            return Json(itemsGlp);
        }

        #endregion
    }
}