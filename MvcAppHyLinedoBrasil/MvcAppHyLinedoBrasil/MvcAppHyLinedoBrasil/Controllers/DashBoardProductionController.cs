using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using System.Data;
using System.Collections;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class DashBoardProductionController : Controller
    {
        //
        // GET: /DashBoardProduction/

        #region Objetos
        FLIPDataSet flip = new FLIPDataSet();
        
        VU_OvosIncPorMes1TableAdapter ovosIncPorMesAnoTbAdapter1 = new VU_OvosIncPorMes1TableAdapter();
        VU_PesoOvoPorMesTableAdapter pesoOvoPorMesTbAdapter = new VU_PesoOvoPorMesTableAdapter();
        VU_OvosSujosPorMesTableAdapter ovosSujosPorMesTbAdapter = new VU_OvosSujosPorMesTableAdapter();
        VU_OvosTrincadosPorMesTableAdapter ovosTrincadosPorMesTbAdapter = new VU_OvosTrincadosPorMesTableAdapter();
        VU_MediaIdadePorMesTableAdapter mediaIdadesPorMesTbAdapter = new VU_MediaIdadePorMesTableAdapter();

        VU_OvosIncPorSemanaDoAnoTableAdapter ovosIncPorSemanaTbAdapter = new VU_OvosIncPorSemanaDoAnoTableAdapter();
        VU_PesoOvoPorSemanaTableAdapter pesoOvoPorSemanaTbAdapter = new VU_PesoOvoPorSemanaTableAdapter();
        VU_OvosSujosPorSemanaTableAdapter ovosSujosPorSemanaTbAdapter = new VU_OvosSujosPorSemanaTableAdapter();
        VU_OvosTrincadosPorSemanaTableAdapter ovosTrincadosPorSemanaTbAdapter = new VU_OvosTrincadosPorSemanaTableAdapter();
        VU_MediaIdadeProSemanaTableAdapter mediaIdadesPorSemanaTbAdapter = new VU_MediaIdadeProSemanaTableAdapter();

        VU_DIARIO_COMPLETO_DIARIOTableAdapter ovosIncPorDiaTbAdapter = new VU_DIARIO_COMPLETO_DIARIOTableAdapter();
        VU_OvosSujosPorDiaTableAdapter ovosSujosPorDiaTbAdapter = new VU_OvosSujosPorDiaTableAdapter();
        VU_OvosTrincadosPorDiaTableAdapter ovosTrincadosPorDiaTbAdapater = new VU_OvosTrincadosPorDiaTableAdapter();
        VU_MediaIdadePorDiaTableAdapter mediaIdadesPorDiaTbAdpter = new VU_MediaIdadePorDiaTableAdapter();

        VU_ProducaoPorLoteTableAdapter producaoPorLoteTbAdapter = new VU_ProducaoPorLoteTableAdapter();

        // Incubatórios
        VU_CapacidadeAnualTableAdapter capacidadeAnualTbAdapter = new VU_CapacidadeAnualTableAdapter();
        VU_MediaEclosaoAnualTableAdapter mediaEclosaoAnualTbAdapter = new VU_MediaEclosaoAnualTableAdapter();
        VU_MediaRefugosAnualTableAdapter mediaRefugosAnualTbAdapter = new VU_MediaRefugosAnualTableAdapter();
        VU_MediaDestruidosAnualTableAdapter mediaDestruidosAnualTbAdapter = new VU_MediaDestruidosAnualTableAdapter();

        VU_CapacidadeSemanalTableAdapter capacidadeSemanalTbAdapter = new VU_CapacidadeSemanalTableAdapter();
        VU_MediaEclosaoSemanalTableAdapter mediaEclosaoSemanalTbAdapter = new VU_MediaEclosaoSemanalTableAdapter();
        VU_MediaRefugosSemanalTableAdapter mediaRefugosSemanalTbAdapter = new VU_MediaRefugosSemanalTableAdapter();
        VU_MediaDestruidosSemanalTableAdapter mediaDestruidosSemanalTbAdapater = new VU_MediaDestruidosSemanalTableAdapter();

        VU_CapacidadeDiariaTableAdapter capacidadeDiariaTbAdapater = new VU_CapacidadeDiariaTableAdapter();
        VU_MediaEclosaoDiariaTableAdapter mediaEclosaoDiariaTbAdapter = new VU_MediaEclosaoDiariaTableAdapter();
        VU_MediaRefugosDiariaTableAdapter mediaRefugosDiariaTbAdapter = new VU_MediaRefugosDiariaTableAdapter();
        VU_MediaDestruidosDiariaTableAdapter mediaDestruidosDiariaTbAdapter = new VU_MediaDestruidosDiariaTableAdapter();
        
        #endregion

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

        public void CarregaLinhagens()
        {
            List<SelectListItem> linhagens = new List<SelectListItem>();

            linhagens.Add(new SelectListItem { Text = "(Todas)", Value = "", Selected = true });
            linhagens.Add(new SelectListItem { Text = "W-36", Value = "W-36", Selected = false });
            linhagens.Add(new SelectListItem { Text = "BRWN", Value = "BRWN", Selected = false });
            linhagens.Add(new SelectListItem { Text = "LSLC", Value = "LSLC", Selected = false });
            linhagens.Add(new SelectListItem { Text = "LBWN", Value = "LBWN", Selected = false });
            linhagens.Add(new SelectListItem { Text = "H&N", Value = "H&N", Selected = false });

            Session["Linhagens"] = linhagens;
        }

        public void CarregaFazendas()
        {
            List<SelectListItem> fazendas = new List<SelectListItem>();

            fazendas.Add(new SelectListItem { Text = "(Todas)", Value = "", Selected = true });
            fazendas.Add(new SelectListItem { Text = "Hy-Line - Matriz", Value = "HL", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Comendador Gomes", Value = "CG", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Hy-Line - Avós", Value = "SB", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Samambaia", Value = "SM", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Brodowski - São José I e II", Value = "SJ", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Brodowski - 2 Irmãos", Value = "2I", Selected = false });
            fazendas.Add(new SelectListItem { Text = "Brodowski - São Judas", Value = "SD", Selected = false });

            Session["Fazendas"] = fazendas;
        }

        public void CarregaGranjas()
        {
            List<SelectListItem> granjas = new List<SelectListItem>();

            granjas.Add(new SelectListItem { Text = "(Todas)", Value = "", Selected = true });
            granjas.Add(new SelectListItem { Text = "Produção", Value = "PP", Selected = false });
            granjas.Add(new SelectListItem { Text = "Avós", Value = "GP", Selected = false });
            granjas.Add(new SelectListItem { Text = "Recria - Produção", Value = "PG", Selected = false });
            granjas.Add(new SelectListItem { Text = "Recria - Avós", Value = "GG", Selected = false });

            Session["Granjas"] = granjas;
        }

        public void CarregaIncubatorios()
        {
            List<SelectListItem> incubatorios = new List<SelectListItem>();

            incubatorios.Add(new SelectListItem { Text = "(Todas)", Value = "", Selected = true });
            incubatorios.Add(new SelectListItem { Text = "Incubatório de Matrizes - Nova Granada", Value = "CH", Selected = false });
            incubatorios.Add(new SelectListItem { Text = "Incubatório de Matrizes - Ajapi", Value = "TB", Selected = false });
            incubatorios.Add(new SelectListItem { Text = "Incubatório de Avós - Nova Granada", Value = "PH", Selected = false });

            Session["Incubatorios"] = incubatorios;
        }

        public void AtualizaLinhagemSelecionada(string linhagem)
        {
            List<SelectListItem> linhagens = (List<SelectListItem>)Session["Linhagens"];

            foreach (var item in linhagens)
            {
                if (item.Text == linhagem)
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Session["Linhagens"] = linhagens;
        }

        public void AtualizaFazendaSelecionada(string linhagem)
        {
            List<SelectListItem> fazendas = (List<SelectListItem>)Session["Fazendas"];

            foreach (var item in fazendas)
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

            Session["Fazendas"] = fazendas;
        }

        public void AtualizaGranjaSelecionada(string linhagem)
        {
            List<SelectListItem> granjas = (List<SelectListItem>)Session["Granjas"];

            foreach (var item in granjas)
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

            Session["Granjas"] = granjas;
        }

        public void AtualizaIncubatorioSelecionado(string linhagem)
        {
            List<SelectListItem> incubatorios = (List<SelectListItem>)Session["Incubatorios"];

            foreach (var item in incubatorios)
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

            Session["Incubatorios"] = incubatorios;
        }
        
        public ActionResult GranjasMensal()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaLinhagens();
            CarregaFazendas();
            CarregaGranjas();
            CarregaIncubatorios();

            DateTime dataInicial = Convert.ToDateTime("01/" + DateTime.Today.ToShortDateString().Substring(3,7));
            DateTime dataFinal = DateTime.Today;

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = "(Todas)";
            Session["sFazenda"] = "(Todas)";
            Session["sGranja"] = "PP";
            Session["sIncubatorio"] = "(Todos)";

            Session["sDataInicialTabela"] = dataFinal.AddDays(-1).ToShortDateString();

            AtualizaGranjaSelecionada("PP");

            //ViewBag.DataInicial = dataInicial.ToString("dd/MM/yyyy");
            //ViewBag.DataFinal = dataFinal.ToString("dd/MM/yyyy");
            //ViewBag.Linhagem = "(Todas)";

            // % Ovos Incubáveis p/ Dia
            /*diario.Fill(flip.VU_DIARIO_COMPLETO_DIARIO);

            DataTable data = flip.VU_DIARIO_COMPLETO_DIARIO;

            ViewBag.Data = new Bortosky.Google.Visualization.GoogleDataTable(data).GetJson();
            ViewBag.QtdeRegistrosEixoXDia = flip.VU_DIARIO_COMPLETO_DIARIO.Count;

            // % Ovos Incubáveis p/ Semana do Ano
            ovosIncPorSemanaTbAdapter.Fill(flip.VU_OvosIncPorSemanaDoAno);

            DataTable ovosIncPorSemana = flip.VU_OvosIncPorSemanaDoAno;

            ViewBag.OvosIncPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoX = flip.VU_OvosIncPorSemanaDoAno.Count;*/

            // % Ovos Incubáveis p/ Mês/Ano
            ovosIncPorMesAnoTbAdapter1.FillWithFilter2(flip.VU_OvosIncPorMes1, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosIncPorMesAno = flip.VU_OvosIncPorMes1;
            ViewBag.OvosIncPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorMesAno).GetJson();
            ViewBag.QtdeRegistrosEixoXMesAno = flip.VU_OvosIncPorMes1.Count;

            // Peso do Ovo (gr)
            pesoOvoPorMesTbAdapter.Fill(flip.VU_PesoOvoPorMes, "", "PP", dataInicial, dataFinal, "");
            DataTable pesoOvoPorMes = flip.VU_PesoOvoPorMes;
            ViewBag.PesoOvoPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(pesoOvoPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXPesoOvoMesAno = flip.VU_PesoOvoPorMes.Count;

            // Ovos Sujos
            ovosSujosPorMesTbAdapter.FillWithFilter(flip.VU_OvosSujosPorMes, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosSujosPorMes = flip.VU_OvosSujosPorMes;
            ViewBag.ovosSujosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosMesAno = flip.VU_OvosSujosPorMes.Count;

            // Ovos Trincados 
            ovosTrincadosPorMesTbAdapter.Fill(flip.VU_OvosTrincadosPorMes, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosTrincadosPorMes = flip.VU_OvosTrincadosPorMes;
            ViewBag.ovosTrincadosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosMesAno = flip.VU_OvosTrincadosPorMes.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorMesTbAdapter.Fill(flip.VU_MediaIdadePorMes, dataInicial, dataFinal, "", "", "PP");
            //DataTable mediaIdadePorMes = flip.VU_MediaIdadePorMes;
            //ViewBag.mediaIdadePorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorMes).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = flip.VU_MediaIdadePorMes.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = 0;

            return View();
        }

        public ActionResult GranjasMensal2()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string fazenda = Session["sFazenda"].ToString();
            string granja = Session["sGranja"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis p/ Mês/Ano
            ovosIncPorMesAnoTbAdapter1.FillWithFilter2(flip.VU_OvosIncPorMes1, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorMesAno = flip.VU_OvosIncPorMes1;
            ViewBag.OvosIncPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorMesAno).GetJson();
            ViewBag.QtdeRegistrosEixoXMesAno = flip.VU_OvosIncPorMes1.Count;

            // Peso do Ovo (gr)
            pesoOvoPorMesTbAdapter.Fill(flip.VU_PesoOvoPorMes, fazenda, granja, dataInicial, dataFinal, linhagem);
            DataTable pesoOvoPorMes = flip.VU_PesoOvoPorMes;
            ViewBag.PesoOvoPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(pesoOvoPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXPesoOvoMesAno = flip.VU_PesoOvoPorMes.Count;

            // Ovos Sujos
            ovosSujosPorMesTbAdapter.FillWithFilter(flip.VU_OvosSujosPorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorMes = flip.VU_OvosSujosPorMes;
            ViewBag.ovosSujosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosMesAno = flip.VU_OvosSujosPorMes.Count;

            // Ovos Trincados 
            ovosTrincadosPorMesTbAdapter.Fill(flip.VU_OvosTrincadosPorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorMes = flip.VU_OvosTrincadosPorMes;
            ViewBag.ovosTrincadosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosMesAno = flip.VU_OvosTrincadosPorMes.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorMesTbAdapter.Fill(flip.VU_MediaIdadePorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorMes = flip.VU_MediaIdadePorMes;
            //ViewBag.mediaIdadePorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorMes).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = flip.VU_MediaIdadePorMes.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = 0;

            return View("GranjasMensal");
        }

        public ActionResult GranjasSemanal()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string fazenda = Session["sFazenda"].ToString();
            string granja = Session["sGranja"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis
            ovosIncPorSemanaTbAdapter.Fill(flip.VU_OvosIncPorSemanaDoAno, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorSemana = flip.VU_OvosIncPorSemanaDoAno;
            ViewBag.OvosIncPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosIncSemana = flip.VU_OvosIncPorSemanaDoAno.Count;

            // Peso do Ovo (gr)
            pesoOvoPorSemanaTbAdapter.FillWithFilter(flip.VU_PesoOvoPorSemana, fazenda, granja, dataInicial, dataFinal, linhagem);
            DataTable pesoOvoPorSemana = flip.VU_PesoOvoPorSemana;
            ViewBag.PesoOvoPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(pesoOvoPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXPesoOvoSemana = flip.VU_PesoOvoPorSemana.Count;

            // Ovos Sujos
            ovosSujosPorSemanaTbAdapter.Fill(flip.VU_OvosSujosPorSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorSemana = flip.VU_OvosSujosPorSemana;
            ViewBag.ovosSujosPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosSemana = flip.VU_OvosSujosPorSemana.Count;

            // Ovos Trincados 
            ovosTrincadosPorSemanaTbAdapter.Fill(flip.VU_OvosTrincadosPorSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorSemana = flip.VU_OvosTrincadosPorSemana;
            ViewBag.ovosTrincadosPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosSemana = flip.VU_OvosTrincadosPorSemana.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorSemanaTbAdapter.Fill(flip.VU_MediaIdadeProSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorSemana = flip.VU_MediaIdadeProSemana;
            //ViewBag.mediaIdadePorSemana = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorSemana).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeSemana = flip.VU_MediaIdadeProSemana.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeSemana = 0;

            return View("GranjasSemanal");
        }

        public ActionResult GranjasDiario()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string fazenda = Session["sFazenda"].ToString();
            string granja = Session["sGranja"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis
            ovosIncPorDiaTbAdapter.Fill(flip.VU_DIARIO_COMPLETO_DIARIO, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorDia = flip.VU_DIARIO_COMPLETO_DIARIO;
            ViewBag.OvosIncPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosIncDia = flip.VU_DIARIO_COMPLETO_DIARIO.Count;

            // Peso do Ovo (gr)
            // Medição feita semanalmente, não havendo diário.

            // Ovos Sujos
            ovosSujosPorDiaTbAdapter.Fill(flip.VU_OvosSujosPorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorDia = flip.VU_OvosSujosPorDia;
            ViewBag.ovosSujosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosDia = flip.VU_OvosSujosPorDia.Count;

            // Ovos Trincados 
            ovosTrincadosPorDiaTbAdapater.Fill(flip.VU_OvosTrincadosPorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorDia = flip.VU_OvosTrincadosPorDia;
            ViewBag.ovosTrincadosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosDia = flip.VU_OvosTrincadosPorDia.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorDiaTbAdpter.Fill(flip.VU_MediaIdadePorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorDia = flip.VU_MediaIdadePorDia;
            //ViewBag.mediaIdadePorDia = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorDia).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeDia = flip.VU_MediaIdadePorDia.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeDia = 0;

            return View("GranjasDiario");
        }

        public ActionResult GranjasDiarioHome()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            CarregaLinhagens();
            CarregaFazendas();
            CarregaGranjas();
            CarregaIncubatorios();

            DateTime dataInicial = Convert.ToDateTime("01/" + DateTime.Today.ToShortDateString().Substring(3, 7));
            DateTime dataFinal = DateTime.Today;

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = "(Todas)";
            Session["sFazenda"] = "(Todas)";
            Session["sGranja"] = "PP";
            Session["sIncubatorio"] = "(Todos)";

            Session["sDataInicialTabela"] = dataFinal.AddDays(-1).ToShortDateString();

            AtualizaGranjaSelecionada("PP");

            // % Ovos Incubáveis
            ovosIncPorDiaTbAdapter.Fill(flip.VU_DIARIO_COMPLETO_DIARIO, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosIncPorDia = flip.VU_DIARIO_COMPLETO_DIARIO;
            ViewBag.OvosIncPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosIncDia = flip.VU_DIARIO_COMPLETO_DIARIO.Count;

            // Peso do Ovo (gr)
            // Medição feita semanalmente, não havendo diário.

            // Ovos Sujos
            ovosSujosPorDiaTbAdapter.Fill(flip.VU_OvosSujosPorDia, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosSujosPorDia = flip.VU_OvosSujosPorDia;
            ViewBag.ovosSujosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosDia = flip.VU_OvosSujosPorDia.Count;

            // Ovos Trincados 
            ovosTrincadosPorDiaTbAdapater.Fill(flip.VU_OvosTrincadosPorDia, dataInicial, dataFinal, "", "", "PP");
            DataTable ovosTrincadosPorDia = flip.VU_OvosTrincadosPorDia;
            ViewBag.ovosTrincadosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosDia = flip.VU_OvosTrincadosPorDia.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorDiaTbAdpter.Fill(flip.VU_MediaIdadePorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorDia = flip.VU_MediaIdadePorDia;
            //ViewBag.mediaIdadePorDia = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorDia).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeDia = flip.VU_MediaIdadePorDia.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeDia = 0;

            return View("GranjasDiario");
        }

        public ActionResult IncubatoriosMensal()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string incubatorio = Session["sIncubatorio"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todos)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeAnualTbAdapter.Fill(flip.VU_CapacidadeAnual, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeAnual = flip.VU_CapacidadeAnual;
            ViewBag.CapacidadeAnual = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeAnual = flip.VU_CapacidadeAnual.Count;

            // Média de Eclosão
            mediaEclosaoAnualTbAdapter.Fill(flip.VU_MediaEclosaoAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoAnual = flip.VU_MediaEclosaoAnual;
            ViewBag.MediaEclosaoAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoAnual = flip.VU_MediaEclosaoAnual.Count;
            
            // Média de Refugo
            mediaRefugosAnualTbAdapter.Fill(flip.VU_MediaRefugosAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosAnual = flip.VU_MediaRefugosAnual;
            ViewBag.MediaRefugosAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosAnual = flip.VU_MediaRefugosAnual.Count;

            // Média de Destruídos
            mediaDestruidosAnualTbAdapter.Fill(flip.VU_MediaDestruidosAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosAnual = flip.VU_MediaDestruidosAnual;
            ViewBag.MediaDestruidosAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosAnual = flip.VU_MediaDestruidosAnual.Count;

            return View();
        }

        public ActionResult IncubatoriosSemanal()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string incubatorio = Session["sIncubatorio"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todos)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeSemanalTbAdapter.Fill(flip.VU_CapacidadeSemanal, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeSemanal = flip.VU_CapacidadeSemanal;
            ViewBag.CapacidadeSemanal = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeSemanal = flip.VU_CapacidadeSemanal.Count;

            // Média de Eclosão
            mediaEclosaoSemanalTbAdapter.Fill(flip.VU_MediaEclosaoSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoSemanal = flip.VU_MediaEclosaoSemanal;
            ViewBag.MediaEclosaoSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoSemanal = flip.VU_MediaEclosaoSemanal.Count;

            // Média de Refugo
            mediaRefugosSemanalTbAdapter.Fill(flip.VU_MediaRefugosSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosSemanal = flip.VU_MediaRefugosSemanal;
            ViewBag.MediaRefugosSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosSemanal = flip.VU_MediaRefugosSemanal.Count;

            // Média de Destruídos
            mediaDestruidosSemanalTbAdapater.Fill(flip.VU_MediaDestruidosSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosSemanal = flip.VU_MediaDestruidosSemanal;
            ViewBag.MediaDestruidosSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosSemanal = flip.VU_MediaDestruidosSemanal.Count;

            return View();
        }

        public ActionResult IncubatoriosDiario()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(Session["sDataInicial"]);
            DateTime dataFinal = Convert.ToDateTime(Session["sDataFinal"]);
            string linhagem = Session["sLinhagem"].ToString();
            string incubatorio = Session["sIncubatorio"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todos)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeDiariaTbAdapater.Fill(flip.VU_CapacidadeDiaria, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeDiaria = flip.VU_CapacidadeDiaria;
            ViewBag.CapacidadeDiaria = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeDiaria = flip.VU_CapacidadeDiaria.Count;

            // Média de Eclosão
            mediaEclosaoDiariaTbAdapter.Fill(flip.VU_MediaEclosaoDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoDiaria = flip.VU_MediaEclosaoDiaria;
            ViewBag.MediaEclosaoDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoDiaria = flip.VU_MediaEclosaoDiaria.Count;

            // Média de Refugo
            mediaRefugosDiariaTbAdapter.Fill(flip.VU_MediaRefugosDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosDiaria = flip.VU_MediaRefugosDiaria;
            ViewBag.MediaRefugosDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosDiaria = flip.VU_MediaRefugosDiaria.Count;

            // Média de Destruídos
            mediaDestruidosDiariaTbAdapter.Fill(flip.VU_MediaDestruidosDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosDiaria = flip.VU_MediaDestruidosDiaria;
            ViewBag.MediaDestruidosDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosDiaria = flip.VU_MediaDestruidosDiaria.Count;

            return View();
        }

        public ActionResult TabelaProducao()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataTabela = Convert.ToDateTime(Session["sDataInicialTabela"]);

            producaoPorLoteTbAdapter.Fill(flip.VU_ProducaoPorLote, dataTabela, dataTabela, "", "", "PP");
            var lista = flip.VU_ProducaoPorLote;
            
            return View("TabelaProducao", lista);
        }

        [HttpPost]
        public ActionResult AtualizaGranjasMensal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string fazenda = model["Fazenda"].ToString();
            string granja = model["Granja"].ToString();

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sFazenda"] = fazenda;
            Session["sGranja"] = granja;

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis p/ Mês/Ano
            ovosIncPorMesAnoTbAdapter1.FillWithFilter2(flip.VU_OvosIncPorMes1, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorMesAno = flip.VU_OvosIncPorMes1;
            ViewBag.OvosIncPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorMesAno).GetJson();
            ViewBag.QtdeRegistrosEixoXMesAno = flip.VU_OvosIncPorMes1.Count;

            // Peso do Ovo (gr)
            pesoOvoPorMesTbAdapter.Fill(flip.VU_PesoOvoPorMes, fazenda, granja, dataInicial, dataFinal, linhagem);
            DataTable pesoOvoPorMes = flip.VU_PesoOvoPorMes;
            ViewBag.PesoOvoPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(pesoOvoPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXPesoOvoMesAno = flip.VU_PesoOvoPorMes.Count;

            // Ovos Sujos
            ovosSujosPorMesTbAdapter.FillWithFilter(flip.VU_OvosSujosPorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorMes = flip.VU_OvosSujosPorMes;
            ViewBag.ovosSujosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosMesAno = flip.VU_OvosSujosPorMes.Count;

            // Ovos Trincados 
            ovosTrincadosPorMesTbAdapter.Fill(flip.VU_OvosTrincadosPorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorMes = flip.VU_OvosTrincadosPorMes;
            ViewBag.ovosTrincadosPorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorMes).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosMesAno = flip.VU_OvosTrincadosPorMes.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorMesTbAdapter.Fill(flip.VU_MediaIdadePorMes, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorMes = flip.VU_MediaIdadePorMes;
            //ViewBag.mediaIdadePorMesAno = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorMes).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = flip.VU_MediaIdadePorMes.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeMesAno = 0;

            return View("GranjasMensal");
        }

        [HttpPost]
        public ActionResult AtualizaGranjasSemanal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string fazenda = model["Fazenda"].ToString();
            string granja = model["Granja"].ToString();

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sFazenda"] = fazenda;
            Session["sGranja"] = granja;

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis
            ovosIncPorSemanaTbAdapter.Fill(flip.VU_OvosIncPorSemanaDoAno, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorSemana = flip.VU_OvosIncPorSemanaDoAno;
            ViewBag.OvosIncPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosIncSemana = flip.VU_OvosIncPorSemanaDoAno.Count;

            // Peso do Ovo (gr)
            pesoOvoPorSemanaTbAdapter.FillWithFilter(flip.VU_PesoOvoPorSemana, fazenda, granja, dataInicial, dataFinal, linhagem);
            DataTable pesoOvoPorSemana = flip.VU_PesoOvoPorSemana;
            ViewBag.PesoOvoPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(pesoOvoPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXPesoOvoSemana = flip.VU_PesoOvoPorSemana.Count;

            // Ovos Sujos
            ovosSujosPorSemanaTbAdapter.Fill(flip.VU_OvosSujosPorSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorSemana = flip.VU_OvosSujosPorSemana;
            ViewBag.ovosSujosPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosSemana = flip.VU_OvosSujosPorSemana.Count;

            // Ovos Trincados 
            ovosTrincadosPorSemanaTbAdapter.Fill(flip.VU_OvosTrincadosPorSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorSemana = flip.VU_OvosTrincadosPorSemana;
            ViewBag.ovosTrincadosPorSemana = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorSemana).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosSemana = flip.VU_OvosTrincadosPorSemana.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorSemanaTbAdapter.Fill(flip.VU_MediaIdadeProSemana, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorSemana = flip.VU_MediaIdadeProSemana;
            //ViewBag.mediaIdadePorSemana = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorSemana).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeSemana = flip.VU_MediaIdadeProSemana.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeSemana = 0;

            return View("GranjasSemanal");
        }

        [HttpPost]
        public ActionResult AtualizaGranjasDiario(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string fazenda = model["Fazenda"].ToString();
            string granja = model["Granja"].ToString();

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sFazenda"] = fazenda;
            Session["sGranja"] = granja;

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaFazendaSelecionada(fazenda);
            AtualizaGranjaSelecionada(granja);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (fazenda.Equals("(Todas)")) { fazenda = ""; }
            if (granja.Equals("(Todas)")) { granja = ""; }

            // % Ovos Incubáveis
            ovosIncPorDiaTbAdapter.Fill(flip.VU_DIARIO_COMPLETO_DIARIO, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosIncPorDia = flip.VU_DIARIO_COMPLETO_DIARIO;
            ViewBag.OvosIncPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosIncPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosIncDia = flip.VU_DIARIO_COMPLETO_DIARIO.Count;

            // Peso do Ovo (gr)
            // Medição feita semanalmente, não havendo diário.

            // Ovos Sujos
            ovosSujosPorDiaTbAdapter.Fill(flip.VU_OvosSujosPorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosSujosPorDia = flip.VU_OvosSujosPorDia;
            ViewBag.ovosSujosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosSujosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosSujosDia = flip.VU_OvosSujosPorDia.Count;

            // Ovos Trincados 
            ovosTrincadosPorDiaTbAdapater.Fill(flip.VU_OvosTrincadosPorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            DataTable ovosTrincadosPorDia = flip.VU_OvosTrincadosPorDia;
            ViewBag.ovosTrincadosPorDia = new Bortosky.Google.Visualization.GoogleDataTable(ovosTrincadosPorDia).GetJson();
            ViewBag.QtdeRegistrosEixoXOvosTrincadosDia = flip.VU_OvosTrincadosPorDia.Count;

            // Média da Idade dos Lotes p/ descarte 
            //mediaIdadesPorDiaTbAdpter.Fill(flip.VU_MediaIdadePorDia, dataInicial, dataFinal, linhagem, fazenda, granja);
            //DataTable mediaIdadePorDia = flip.VU_MediaIdadePorDia;
            //ViewBag.mediaIdadePorDia = new Bortosky.Google.Visualization.GoogleDataTable(mediaIdadePorDia).GetJson();
            //ViewBag.QtdeRegistrosEixoXmediaIdadeDia = flip.VU_MediaIdadePorDia.Count;
            ViewBag.QtdeRegistrosEixoXmediaIdadeDia = 0;

            return View("GranjasDiario");
        }

        [HttpPost]
        public ActionResult AtualizaIncubatoriosMensal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string incubatorio = model["Incubatorio"].ToString();

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sIncubatorio"] = incubatorio;

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todas)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeAnualTbAdapter.Fill(flip.VU_CapacidadeAnual, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeAnual = flip.VU_CapacidadeAnual;
            ViewBag.CapacidadeAnual = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeAnual = flip.VU_CapacidadeAnual.Count;

            // Média de Eclosão
            mediaEclosaoAnualTbAdapter.Fill(flip.VU_MediaEclosaoAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoAnual = flip.VU_MediaEclosaoAnual;
            ViewBag.MediaEclosaoAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoAnual = flip.VU_MediaEclosaoAnual.Count;

            // Média de Refugo
            mediaRefugosAnualTbAdapter.Fill(flip.VU_MediaRefugosAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosAnual = flip.VU_MediaRefugosAnual;
            ViewBag.MediaRefugosAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosAnual = flip.VU_MediaRefugosAnual.Count;

            // Média de Destruídos
            mediaDestruidosAnualTbAdapter.Fill(flip.VU_MediaDestruidosAnual, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosAnual = flip.VU_MediaDestruidosAnual;
            ViewBag.MediaDestruidosAnual = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosAnual).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosAnual = flip.VU_MediaDestruidosAnual.Count;

            return View("IncubatoriosMensal");
        }

        [HttpPost]
        public ActionResult AtualizaIncubatoriosSemanal(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string incubatorio = model["Incubatorio"].ToString();

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sIncubatorio"] = incubatorio;

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todas)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeSemanalTbAdapter.Fill(flip.VU_CapacidadeSemanal, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeSemanal = flip.VU_CapacidadeSemanal;
            ViewBag.CapacidadeSemanal = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeSemanal = flip.VU_CapacidadeSemanal.Count;

            // Média de Eclosão
            mediaEclosaoSemanalTbAdapter.Fill(flip.VU_MediaEclosaoSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoSemanal = flip.VU_MediaEclosaoSemanal;
            ViewBag.MediaEclosaoSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoSemanal = flip.VU_MediaEclosaoSemanal.Count;

            // Média de Refugo
            mediaRefugosSemanalTbAdapter.Fill(flip.VU_MediaRefugosSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosSemanal = flip.VU_MediaRefugosSemanal;
            ViewBag.MediaRefugosSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosSemanal = flip.VU_MediaRefugosSemanal.Count;

            // Média de Destruídos
            mediaDestruidosSemanalTbAdapater.Fill(flip.VU_MediaDestruidosSemanal, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosSemanal = flip.VU_MediaDestruidosSemanal;
            ViewBag.MediaDestruidosSemanal = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosSemanal).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosSemanal = flip.VU_MediaDestruidosSemanal.Count;

            return View("IncubatoriosSemanal");
        }

        [HttpPost]
        public ActionResult AtualizaIncubatoriosDiario(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataInicial = Convert.ToDateTime(model["dataInicial"].ToString());
            DateTime dataFinal = Convert.ToDateTime(model["dataFinal"].ToString());
            string linhagem = model["Linhagem"].ToString();
            string incubatorio = model["Incubatorio"].ToString();

            Session["sDataInicial"] = dataInicial.ToShortDateString();
            Session["sDataFinal"] = dataFinal.ToShortDateString();
            Session["sLinhagem"] = linhagem;
            Session["sIncubatorio"] = incubatorio;

            AtualizaLinhagemSelecionada(linhagem);
            AtualizaIncubatorioSelecionado(incubatorio);

            if (linhagem.Equals("(Todas)")) { linhagem = ""; }
            if (incubatorio.Equals("(Todas)")) { incubatorio = ""; }

            // Utilização do Incubatório
            capacidadeDiariaTbAdapater.Fill(flip.VU_CapacidadeDiaria, dataInicial, dataFinal, incubatorio);
            DataTable capacidadeDiaria = flip.VU_CapacidadeDiaria;
            ViewBag.CapacidadeDiaria = new Bortosky.Google.Visualization.GoogleDataTable(capacidadeDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXCapacidadeDiaria = flip.VU_CapacidadeDiaria.Count;

            // Média de Eclosão
            mediaEclosaoDiariaTbAdapter.Fill(flip.VU_MediaEclosaoDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaEclosaoDiaria = flip.VU_MediaEclosaoDiaria;
            ViewBag.MediaEclosaoDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaEclosaoDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaEclosaoDiaria = flip.VU_MediaEclosaoDiaria.Count;

            // Média de Refugo
            mediaRefugosDiariaTbAdapter.Fill(flip.VU_MediaRefugosDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaRefugosDiaria = flip.VU_MediaRefugosDiaria;
            ViewBag.MediaRefugosDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaRefugosDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaRefugosDiaria = flip.VU_MediaRefugosDiaria.Count;

            // Média de Destruídos
            mediaDestruidosDiariaTbAdapter.Fill(flip.VU_MediaDestruidosDiaria, dataInicial, dataFinal, linhagem, incubatorio);
            DataTable mediaDestruidosDiaria = flip.VU_MediaDestruidosDiaria;
            ViewBag.MediaDestruidosDiaria = new Bortosky.Google.Visualization.GoogleDataTable(mediaDestruidosDiaria).GetJson();
            ViewBag.QtdeRegistrosEixoXMediaDestruidosDiaria = flip.VU_MediaDestruidosDiaria.Count;

            return View("IncubatoriosDiario");
        }

        [HttpPost]
        public ActionResult AtualizaTabelaProducao(FormCollection model)
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            DateTime dataTabela = Convert.ToDateTime(model["dataTabela"].ToString());

            Session["sDataInicialTabela"] = dataTabela.ToShortDateString();

            producaoPorLoteTbAdapter.Fill(flip.VU_ProducaoPorLote, dataTabela, dataTabela, "", "", "PP");
            var lista = flip.VU_ProducaoPorLote;

            return View("TabelaProducao", lista);
        }

        /*public ActionResult Chart()
        {
            diario.Fill(flip.VU_DIARIO_COMPLETO_DIARIO);

            List<MvcAppHyLinedoBrasil.Data.FLIPDataSet.VU_DIARIO_COMPLETO_DIARIORow> resultadoDiario =
                flip.VU_DIARIO_COMPLETO_DIARIO.ToList();
                        
            string themePathName = @"~\Content\themes\ThemeChartHyline.xml";

            //var myChart = new Chart(width: 1000, height: 400, themePath: themePathName)
            var myChart = new Chart(width: 1000, height: 400)
               .AddTitle("% Ovos Incubáveis p/ Dia")
                //.SetXAxis("Período")
                //.SetYAxis("Qtde.")
               .AddSeries("% Incubável", chartType: "Line", xValue: resultadoDiario, xField: "Data_Producao",
               yValues: resultadoDiario, yFields: "Incubavel")
               .AddLegend("Legenda")
               .AddSeries("% Meta", chartType: "Line", xValue: resultadoDiario, xField: "Data_Producao",
                    yValues: resultadoDiario, yFields: "Meta")
                //.DataBindCrossTable(resultadoDiario, 
                //     "Data de Produção",
                //     "Data de Produção",
                //     "% Incubável")
                //.DataBindTable(dataSource: resultadoDiario, xField: "Data de Produção")
                //.AddSeries("% Ovos Incubáveis p/ Dia", "Line")
                //.AddSeries(name: "default",
                //     chartType: "Line",
                //     chartArea: "default",
                //     axisLabel: "teste",
                //     legend: "teste2",
                //     markerStep: 1,
                //     xValue: resultadoDiario, xField: "Data de Produção",
                //     yValues: resultadoDiario, yFields: "% Incubável")
            .Write();

            return null;
        }*/

    }
}
