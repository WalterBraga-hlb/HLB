using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;

namespace MvcAppHyLinedoBrasil.Controllers
{
    public class RelatoriosProducaoController : Controller
    {
        #region Menu

        public ActionResult Index()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            ChangeLanguage();
            LoadSessionVariables();

            return View();
        }

        #endregion

        #region Relatório de Incubação de Ovos - WEB

        public ActionResult RelIncubacaoWEB()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelIncubacaoWEB(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Incubatório

            Session["incubatorio"] = model["incubatorio"].ToString();
            AtualizaDDL(model["incubatorio"], (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Data de Incubação Inicial

            if (model["dataInicialRelPrd"] != null)
                Session["dataInicialRelPrd"] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #region Data de Incubação Final

            if (model["dataFinalRelPrd"] != null)
                Session["dataFinalRelPrd"] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB\\Rel_Incubacoes_WEB_" + result + ".xlsx";
            pesquisa = "*Rel_Incubacoes_WEB_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB\\Rel_Incubacoes_WEB.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Cargue de Huevos"];

            //worksheet.Unprotect("hyline2020");

            worksheet.Cells[4, 5] = model["incubatorio"];
            worksheet.Cells[5, 5] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString());
            worksheet.Cells[6, 5] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString());
            
            //worksheet.Protect("hyline2020", true, true);

            #endregion

            #region Atualiza Consultas SQL

            #region VU_Incubacoes_WEB

            string dataInicialStr = Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToString("yyyy-MM-dd");
            string dataFinalStr = Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToString("yyyy-MM-dd");

            string commandTextCabecalho =
                "select " +
                    "* ";

            string commandTextTabelas =
                "from " +
                    "VU_Incubacoes_WEB ";

            string commandTextCondicaoJoins =
                "where ";

            string commandTextCCondicaoFiltros =
                "Hatch_loc = '" + model["incubatorio"].ToString() + "' and " +
                "Set_date between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextOrderBy = 
                "order by " +
                    "1, 2, 3, 4, 5";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Incubacoes_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCabecalho + commandTextTabelas + commandTextCondicaoJoins +
                        commandTextCCondicaoFiltros + commandTextOrderBy;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download",
                "Rel_" + Translate("Incubação WEB").Replace(" ","") + "_" + model["incubatorio"] + "_" + dataInicialStr + "_a_" + dataFinalStr + ".xlsx");
        }

        #endregion

        #region Relatório de Incubação de Ovos Por Tipo - WEB

        public ActionResult RelIncubacaoPorTipoOvoWEB()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelIncubacaoPorTipoOvoWEB(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Incubatório

            Session["incubatorio"] = model["incubatorio"].ToString();
            AtualizaDDL(model["incubatorio"], (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Data de Incubação Inicial

            if (model["dataInicialRelPrd"] != null)
                Session["dataInicialRelPrd"] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #region Data de Incubação Final

            if (model["dataFinalRelPrd"] != null)
                Session["dataFinalRelPrd"] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB\\Rel_Incubacoes_Por_Tipo_Ovo_WEB_" + result + ".xlsx";
            pesquisa = "*Rel_Incubacoes_Por_Tipo_Ovo_WEB_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\IncubacoesWEB\\Rel_Incubacoes_Por_Tipo_Ovo_WEB.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Incubação de Ovos"];

            //worksheet.Unprotect("hyline2020");

            worksheet.Cells[4, 5] = model["incubatorio"];
            worksheet.Cells[5, 5] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString());
            worksheet.Cells[6, 5] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString());

            //worksheet.Protect("hyline2020", true, true);

            #endregion

            #region Atualiza Consultas SQL

            #region VU_Incubacoes_WEB

            string dataInicialStr = Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToString("yyyy-MM-dd");
            string dataFinalStr = Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToString("yyyy-MM-dd");

            string commandTextCabecalho =
                "select " +
                    "* ";

            string commandTextTabelas =
                "from " +
                    "VU_Incubacoes_Por_Tipo_Ovo_WEB ";

            string commandTextCondicaoJoins =
                "where ";

            string commandTextCCondicaoFiltros =
                "[Incubatório] = '" + model["incubatorio"].ToString() + "' and " +
                "[Data Incubação] between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextOrderBy =
                "order by " +
                    "1, 2, 3, 4, 5, 6";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Incubacoes_Por_Tipo_Ovo_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCabecalho + commandTextTabelas + commandTextCondicaoJoins +
                        commandTextCCondicaoFiltros + commandTextOrderBy;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download",
                "Rel_" + Translate("Incubacao_Por_Tipo_Ovo").Replace(" ", "") + "_" + model["incubatorio"] + "_" + dataInicialStr + "_a_" + dataFinalStr + ".xlsx");
        }

        #endregion

        #region Relatório de Nascimento - WEB

        public ActionResult RelNascimentoWEB()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelNascimentoWEB(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Incubatório

            Session["incubatorio"] = model["incubatorio"].ToString();
            AtualizaDDL(model["incubatorio"], (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Data de Incubação Inicial

            if (model["dataInicialRelPrd"] != null)
                Session["dataInicialRelPrd"] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #region Data de Incubação Final

            if (model["dataFinalRelPrd"] != null)
                Session["dataFinalRelPrd"] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB\\Rel_Nascimento_WEB_" + result + ".xlsx";
            pesquisa = "*Rel_Nascimento_WEB_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB\\Rel_Nascimento_WEB.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Nacimiento"];

            //worksheet.Unprotect("hyline2020");

            worksheet.Cells[4, 5] = model["incubatorio"];
            worksheet.Cells[5, 5] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString());
            worksheet.Cells[6, 5] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString());

            //worksheet.Protect("hyline2020", true, true);

            #endregion

            #region Atualiza Consultas SQL

            #region VU_Nascimentos_WEB

            string dataInicialStr = Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToString("yyyy-MM-dd");
            string dataFinalStr = Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToString("yyyy-MM-dd");

            string commandTextCabecalho =
                "select " +
                    "* ";

            string commandTextTabelas =
                "from " +
                    "VU_Nascimentos_WEB ";

            string commandTextCondicaoJoins =
                "where ";

            string commandTextCCondicaoFiltros =
                "Hatch_loc = '" + model["incubatorio"].ToString() + "' and " +
                "Hath_Date between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextOrderBy =
                "order by " +
                    "1, 2, 3, 4, 5";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Nascimentos_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCabecalho + commandTextTabelas + commandTextCondicaoJoins +
                        commandTextCCondicaoFiltros + commandTextOrderBy;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download",
                "Rel_" + Translate("Nascimento WEB").Replace(" ", "") + "_" + model["incubatorio"] + "_" + dataInicialStr + "_a_" + dataFinalStr + ".xlsx");
        }

        #endregion

        #region Relatório de Embriodiagnóstico - WEB

        public ActionResult RelEmbrioWEB()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelEmbrioWEB(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Incubatório

            Session["incubatorio"] = model["incubatorio"].ToString();
            AtualizaDDL(model["incubatorio"], (List<SelectListItem>)Session["ListaIncubatorios"]);

            #endregion

            #region Data de Incubação Inicial

            if (model["dataInicialRelPrd"] != null)
                Session["dataInicialRelPrd"] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #region Data de Incubação Final

            if (model["dataFinalRelPrd"] != null)
                Session["dataFinalRelPrd"] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\EmbrioWEB\\Rel_Embrio_WEB_" + result + ".xlsx";
            pesquisa = "*Rel_Embrio_WEB_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\EmbrioWEB", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\EmbrioWEB\\Rel_Embrio_WEB.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Embrodiagnosis"];

            //worksheet.Unprotect("hyline2020");

            worksheet.Cells[4, 5] = model["incubatorio"];
            worksheet.Cells[5, 5] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString());
            worksheet.Cells[6, 5] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString());

            //worksheet.Protect("hyline2020", true, true);

            #endregion

            #region Atualiza Consultas SQL

            #region VU_Embrio_WEB

            string dataInicialStr = Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToString("yyyy-MM-dd");
            string dataFinalStr = Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToString("yyyy-MM-dd");

            string commandTextCabecalho =
                "select " +
                    "* ";

            string commandTextTabelas =
                "from " +
                    "VU_Embrio_WEB ";

            string commandTextCondicaoJoins =
                "where ";

            string commandTextCCondicaoFiltros =
                "Hatch_loc = '" + model["incubatorio"].ToString() + "' and " +
                "Hath_Date between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextOrderBy =
                "order by " +
                    "1, 2, 3, 5";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Embrio_WEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCabecalho + commandTextTabelas + commandTextCondicaoJoins +
                        commandTextCCondicaoFiltros + commandTextOrderBy;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download",
                "Rel_" + Translate("Embrio WEB").Replace(" ", "") + "_" + model["incubatorio"] + "_" + dataInicialStr + "_a_" + dataFinalStr + ".xlsx");
        }

        #endregion

        #region Relatório de Nascimento / Embriodiagnostico Novo - WEB

        public ActionResult RelNascimentoEmbrioNovoWEB()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelNascimentoEmbrioNovoWEB(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Incubatório

            Session["incubatorio"] = model["incubatorio"].ToString();
            AtualizaDDL(model["incubatorio"], (List<SelectListItem>)Session["ListaIncubatorios"]);

            string incubatorios = "";
            if (model["incubatorio"] == "")
            {
                foreach (var item in (List<SelectListItem>)Session["ListaIncubatorios"])
                {
                    if (item.Value != "")
                    {
                        incubatorios = incubatorios + item.Value;
                        if (((List<SelectListItem>)Session["ListaIncubatorios"]).IndexOf(item) < (((List<SelectListItem>)Session["ListaIncubatorios"]).Count - 1))
                            incubatorios = incubatorios + " / ";
                    }
                }

                AtualizaDDL("", (List<SelectListItem>)Session["ListaIncubatorios"]);
            }

            #endregion

            #region Data de Nascimento Inicial

            if (model["dataInicialRelPrd"] != null)
                Session["dataInicialRelPrd"] = Convert.ToDateTime(model["dataInicialRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #region Data de Nascimento Final

            if (model["dataFinalRelPrd"] != null)
                Session["dataFinalRelPrd"] = Convert.ToDateTime(model["dataFinalRelPrd"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB\\Rel_Nascimento_Embrio_Novo_" + result + ".xlsx";
            pesquisa = "*Rel_Nascimento_Embrio_Novo_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Incubatorio\\NascimentoWEB\\Rel_Nascimento_Embrio_Novo.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Dados"];

            //worksheet.Unprotect("hyline2020");

            if (incubatorios == "")
                worksheet.Cells[4, 4] = ((List<SelectListItem>)Session["ListaIncubatorios"]).Where(w => w.Value == model["incubatorio"]).FirstOrDefault().Text;
            else
                worksheet.Cells[4, 4] = incubatorios;
            worksheet.Cells[5, 4] = "De " + Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToShortDateString()
                + " a " + Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToShortDateString();

            //worksheet.Protect("hyline2020", true, true);

            #endregion

            #region Atualiza Consultas SQL

            #region VU_Nascimento_Embrio_Web_Novo

            string dataInicialStr = Convert.ToDateTime(model["dataInicialRelPrd"].ToString()).ToString("yyyy-MM-dd");
            string dataFinalStr = Convert.ToDateTime(model["dataFinalRelPrd"].ToString()).ToString("yyyy-MM-dd");

            string commandTextCabecalho =
                "select " +
                    "* ";

            string commandTextTabelas =
                "from " +
                    "VU_Nascimento_Embrio_Web_Novo ";

            string commandTextCondicaoJoins =
                "where ";

            string commandTextCCondicaoFiltros =
                "(CharIndex([Incubatório],'" + incubatorios + "') > 0 or [Incubatório] = '" + model["incubatorio"].ToString() + "') and " +
                "[Data Nascimento] between '" + dataInicialStr + "' and '" + dataFinalStr + "' ";

            string commandTextOrderBy =
                "order by " +
                    "1, 3, 5, 4, 6, 8";

            #endregion

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("VU_Nascimento_Embrio_Web_Novo"))
                    item.OLEDBConnection.CommandText =
                        commandTextCabecalho + commandTextTabelas + commandTextCondicaoJoins +
                        commandTextCCondicaoFiltros + commandTextOrderBy;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download", "Rel_Nascimento_Embrio_Novo_" + model["incubatorio"] + "_" + dataInicialStr + "_a_" + dataFinalStr + ".xlsx");
        }

        #endregion

        #region Posição de Estoque de Ovos Férteis

        public ActionResult RelPosicaoEstoque()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelPosicaoEstoque(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Posição de Estoque

            if (model["posicaoEstoque"] != null)
                Session["posicaoEstoque"] = Convert.ToDateTime(model["posicaoEstoque"].ToString())
                    .ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Estoque\\Rel_Posicao_Estoque_" + result + ".xlsx";
            pesquisa = "*Rel_Posicao_Estoque_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Estoque", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Estoque\\Rel_Posicao_Estoque.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheetGeral = (Excel._Worksheet)oBook.Worksheets["Inventário Geral"];
            Excel._Worksheet worksheetHLB = (Excel._Worksheet)oBook.Worksheets["Inventário HLB"];
            Excel._Worksheet worksheetLTZ = (Excel._Worksheet)oBook.Worksheets["Inventário LTZ"];
            Excel._Worksheet worksheetHEN = (Excel._Worksheet)oBook.Worksheets["Inventário H&N"];
            Excel._Worksheet worksheetPPA = (Excel._Worksheet)oBook.Worksheets["Inventário PPA"];

            worksheetGeral.Cells[2, 7] = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToShortDateString();
            worksheetHLB.Cells[2, 7] = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToShortDateString();
            worksheetLTZ.Cells[2, 7] = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToShortDateString();
            worksheetHEN.Cells[2, 7] = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToShortDateString();
            worksheetPPA.Cells[2, 7] = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToShortDateString();

            string posicaoEstoqueSQL = Convert.ToDateTime(model["posicaoEstoque"].ToString()).ToString("yyyy-MM-dd");

            #endregion

            #region Atualiza Consultas SQL

            string commandTextProcedureGeral = "exec Saldo_ESTQ_WEB_MAT_BR '" + posicaoEstoqueSQL + "' ";
            string commandTextProcedureHLB = "exec Saldo_ESTQ_WEB_MAT_BR_HLB '" + posicaoEstoqueSQL + "' ";
            string commandTextProcedureLTZ = "exec Saldo_ESTQ_WEB_MAT_BR_LTZ '" + posicaoEstoqueSQL + "' ";
            string commandTextProcedureHEN = "exec Saldo_ESTQ_WEB_MAT_BR_HEN '" + posicaoEstoqueSQL + "' ";
            string commandTextProcedurePPA = "exec Saldo_ESTQ_WEB_MAT_BR_PPA '" + posicaoEstoqueSQL + "' ";

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("Saldo_ESTQ_WEB_MAT_BR")) item.OLEDBConnection.CommandText = commandTextProcedureGeral;
                else if (item.Name.Equals("Saldo_ESTQ_WEB_MAT_BR_HLB")) item.OLEDBConnection.CommandText = commandTextProcedureHLB;
                else if (item.Name.Equals("Saldo_ESTQ_WEB_MAT_BR_LTZ")) item.OLEDBConnection.CommandText = commandTextProcedureLTZ;
                else if (item.Name.Equals("Saldo_ESTQ_WEB_MAT_BR_HEN")) item.OLEDBConnection.CommandText = commandTextProcedureHEN;
                else if (item.Name.Equals("Saldo_ESTQ_WEB_MAT_BR_PPA")) item.OLEDBConnection.CommandText = commandTextProcedurePPA;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download", "Rel_Posicao_Estoque_" + posicaoEstoqueSQL + ".xlsx");
        }

        #endregion

        #region Ficha de Estoque de Ovos Férteis

        public ActionResult RelFichaEstoque()
        {
            if (VerificaSessao()) return RedirectToAction("LogOn", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult DownloadRelFichaEstoque(FormCollection model)
        {
            #region Tratamento de Parâmetros

            #region Data Inicial de Movimentação

            if (model["dataIniMov"] != null)
                Session["dataIniMov"] = Convert.ToDateTime(model["dataIniMov"].ToString()).ToShortDateString();

            #endregion

            #region Data Final de Movimentação

            if (model["dataFimMov"] != null)
                Session["dataFimMov"] = Convert.ToDateTime(model["dataFimMov"].ToString()).ToShortDateString();

            #endregion

            #endregion

            #region Localiza Arquivos Gerados Antigos e Deleta

            string destino = "";
            string pesquisa = "";

            // Gera codigo randomico
            var chars = "0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Estoque\\Ficha_Estoque_Ovos_" + result + ".xlsx";
            pesquisa = "*Ficha_Estoque_Ovos_" + result + ".xlsx";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\Estoque", pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #endregion

            #region Copia o arquivo do Atualizado a salva o ID do Processo

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Estoque\\Ficha_Estoque_Ovos.xlsx", destino);

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

            #endregion

            #region Abre o Excel e Salva os Parâmetros nas Células

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            string dataIniMovSQL = Convert.ToDateTime(model["dataIniMov"].ToString()).ToString("yyyy-MM-dd");
            string dataFimMovSQL = Convert.ToDateTime(model["dataFimMov"].ToString()).ToString("yyyy-MM-dd");

            #endregion

            #region Atualiza Consultas SQL

            string commandTextProcedureGeral = "exec USER_Ficha_Estq_Ovos_Incubaveis_Classificacao '" + dataIniMovSQL + "', '" + dataFimMovSQL + "'";
            
            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("USER_Ficha_Estq_Ovos_Incubaveis_Classificacao")) item.OLEDBConnection.CommandText = commandTextProcedureGeral;
            }

            #endregion

            #region Atualiza as Consultas e Fecha o Excel

            oBook.RefreshAll();
            Thread.Sleep(5000);

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

            #endregion

            return File(destino, "Download", "Ficha_Estoque_Ovos_" + dataIniMovSQL + "_a_" + dataFimMovSQL + ".xlsx");
        }

        #endregion

        #region DropDown Methods

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

        public List<SelectListItem> ListaIncubatorios()
        {
            List<SelectListItem> ddl = new List<SelectListItem>();

            FLIPDataSet flip = new FLIPDataSet();
            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
            hTA.Fill(flip.HATCHERY_CODES);

            foreach (var item in flip.HATCHERY_CODES.OrderBy(o => o.HATCH_DESC).ToList())
            {
                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                {
                    ddl.Add(new SelectListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                }
            }

            if (ddl.Count() > 1)
                ddl.Add(new SelectListItem { Text = "(Todos)", Value = "", Selected = false });

            return ddl.OrderBy(o => o.Text).ToList();
        }

        #endregion

        #region Other Methods

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

        public string Translate(string text)
        {
            string language = Session["language"].ToString();
            return AccountController.Translate(text.Replace(":", ""), language);
        }

        public void ChangeLanguage()
        {
            string lg = Session["language"].ToString();

            if (lg != "pt-BR")
            {
                ViewBag.Title = Translate("Title_Menu_Reports_FLIP_WebDesktop");
            }
        }

        public void LoadSessionVariables()
        {
            Session["ListaIncubatorios"] = ListaIncubatorios();
        }

        #endregion
    }
}
