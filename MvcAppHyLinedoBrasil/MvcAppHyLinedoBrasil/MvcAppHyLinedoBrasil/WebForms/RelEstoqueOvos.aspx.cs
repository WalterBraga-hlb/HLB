using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelEstoqueOvos : System.Web.UI.Page
    {
        public void VerificaSessao()
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("http://hlbapp.hyline.com.br");
            }
            else
            {
                if (Session["usuario"].ToString() == "0")
                {
                    Response.Redirect("http://hlbapp.hyline.com.br");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

            if (!IsPostBack)
            {
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";
                calDataFinal.SelectedDate = DateTime.Today;
                calDataInicial.SelectedDate = DateTime.Today;

                CheckDireitosIncubatorios();
            }
        }

        public void CheckDireitosIncubatorios()
        {
            ListItemCollection listaItens = new ListItemCollection();
            foreach (ListItem item in ddlIncubatorio.Items)
            {
                if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-Acesso" + item.Value.Substring(0, 2), (System.Collections.ArrayList)Session["Direitos"])
                    ||
                    MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoGranjas" + item.Value.Substring(0, 1), (System.Collections.ArrayList)Session["Direitos"]))
                {
                    listaItens.Add(item);
                }
            }

            ddlIncubatorio.Items.Clear();

            foreach (ListItem item in listaItens)
            {
                ddlIncubatorio.Items.Add(item);
            }
        }

        public string GeraRelatorioMatriz(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Matrizes.xlsx", destino);

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

            // Parâmetros
            string dataInicialStrSQLServer = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSQLServer = dataFinal.ToString("yyyy-MM-dd");

            string dataInicialStrOracle = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStrOracle = dataFinal.ToString("dd/MM/yyyy");

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Estoque Diário"];
            worksheet.Cells[2, 6] = dataInicial;
            worksheet.Cells[2, 8] = dataFinal;
            worksheet.Cells[5, 2] = dataInicial;

            Excel._Worksheet worksheetInventario = (Excel._Worksheet)oBook.Worksheets["Inventário de Ovos"];
            worksheetInventario.Cells[2, 5] = DateTime.Today;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                string commandTextCHICCabecalho = "";
                string commandTextCHICTabelas = "";
                string commandTextCHICCondicaoJoins = "";
                string commandTextCHICCondicaoFiltros = "";
                string commandTextCHICCondicaoParametros = "";
                string commandTextCHICAgrupamento = "";
                string commandTextCHICOrdenacao = "";

                //if (item.Name.Equals("Apolo"))
                //{
                //    commandTextCHICCabecalho =
                //        "select * ";

                //    commandTextCHICTabelas =
                //        "from " +
                //            "VU_Lanc_Estoque_Ovos ";

                //    commandTextCHICCondicaoJoins =
                //        "where [Geração] = 'PP' and ";

                //    commandTextCHICCondicaoFiltros = "";

                //    commandTextCHICCondicaoParametros =
                //            "[Data Mov.] between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                //    commandTextCHICAgrupamento = "";

                //    commandTextCHICOrdenacao =
                //        "order by " +
                //            "1, 2, 5, 14";

                //    item.OLEDBConnection.CommandText =
                //        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                //        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                //        commandTextCHICOrdenacao;
                //}
                if (item.Name.Equals("IncubacaoWEB"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "HATCHERY_EGG_DATA ";

                    commandTextCHICCondicaoJoins =
                        "where location = 'PP' and hatch_loc in ('CH','TB') and ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                            "Set_Date between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("DEOs"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "LayoutDiarioExpedicaos ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "DataHoraCarreg >= '2015-06-01' " 
                        + "and (Incubatorio in ('CH','TB') or Granja in ('CH','TB','CHC','TBC') "
                        + "or (Incubatorio = 'NM' and Granja not in ('PL','CH'))) and ";

                    commandTextCHICCondicaoParametros =
                        "DataHoraCarreg between '" + dataInicialStrSQLServer + " 00:00:00' and '" + dataFinalStrSQLServer + " 23:59:59' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("hatchery"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "Vu_Hatchery_Rel V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.Local in ('CH','TB') and ";

                    commandTextCHICCondicaoParametros =
                            "V.Data_Nascimento-21 between TO_DATE('" 
                            + dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('"
                            + dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("producao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_DIARIO_COMPLETO V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.\"Data de Produção\" >= TO_DATE('01/06/2015','dd/MM/yyyy HH24:MI:SS') and V.Location = 'PP' and ";

                    commandTextCHICCondicaoParametros =
                            "V.\"Data de Produção\" between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFDoacao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod in ('1','12','15') and I.FiscalItNFNomeProd like '%OVO%' " +
                        "and NatOpCodEstr like '%.910' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFExportacao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        //"F.FiscalNFIndMov = 'Saída' and F.EmpCod = '1' " +
                        "F.FiscalNFIndMov = 'Saída' " +
                        "and NatOpCodEstr like '7.%' and I.FiscalItNFNomeProd like '%OVOS%' " +
                        "and F.FiscalNFRazaoSocial not like '%HY LINE%' and F.FiscalNFRazaoSocial not like '%LOHMANN%'" +
                        "and F.FiscalNFRazaoSocial not like '%H&N%' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFOvosComerciais"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '1' " +
                        "and NatOpCodEstr like '%.101' and I.FiscalItNFNomeProd like '%OVO%COMERCIAIS%' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFVendaOvos"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "inner join ENTIDADE E on F.EntCod = E.EntCod " +
                        "inner join CIDADE C on E.CidCod = C.CidCod " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod in ('1','12','15') " +
                        "and (FiscalItNFNatOpCodEstr like '%.101' or FiscalItNFNatOpCodEstr like '%.102') " +
                        "and (I.FiscalItNFNomeProd like '%INCUBÁVEIS%' or I.FiscalItNFNomeProd like '%OVOS%FÉRTEIS%') " +
                        "and C.UfSigla <> 'EX' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                //else if (item.Name.Equals("IncubacaoFLIP"))
                //{
                //    commandTextCHICCabecalho =
                //        "select * ";

                //    commandTextCHICTabelas =
                //        "from " +
                //            "vu_hatchery_rel_trx_date V ";

                //    commandTextCHICCondicaoJoins =
                //        "where ";

                //    commandTextCHICCondicaoFiltros = "V.Local <> 'PH' and ";

                //    commandTextCHICCondicaoParametros =
                //            "V.Data_Nascimento-21 between TO_DATE('" +
                //            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                //            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                //    commandTextCHICAgrupamento = "";

                //    commandTextCHICOrdenacao = "";

                //    item.OLEDBConnection.CommandText =
                //        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                //        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                //        commandTextCHICOrdenacao;
                //}
                else if (item.Name.Equals("NFTransfHygen"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '1' " +
                        "and NatOpCodEstr like '%.901' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("EstoqueDetalhado"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '" 
                            + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "Order by 4, 6, 7";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Inventario"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios_Inventario ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "[Local Estoque] in ('CH','TB') and ";

                    //string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");
                    string dataInicialStrSQLServerPrd = dataInicial.ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by 1, 3, 2, 4";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
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

        public string GeraRelatorioAvos(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino, DateTime dataInicial, DateTime dataFinal)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Avos.xlsx", destino);

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

            // Parâmetros
            string dataInicialStrSQLServer = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSQLServer = dataFinal.ToString("yyyy-MM-dd");

            string dataInicialStrOracle = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStrOracle = dataFinal.ToString("dd/MM/yyyy");

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Estoque Diário"];
            worksheet.Cells[2, 6] = dataInicial;
            worksheet.Cells[2, 8] = dataFinal;
            worksheet.Cells[5, 2] = dataInicial;

            Excel._Worksheet worksheetInventario = (Excel._Worksheet)oBook.Worksheets["Inventário de Ovos"];
            worksheetInventario.Cells[2, 5] = DateTime.Today;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                string commandTextCHICCabecalho = "";
                string commandTextCHICTabelas = "";
                string commandTextCHICCondicaoJoins = "";
                string commandTextCHICCondicaoFiltros = "";
                string commandTextCHICCondicaoParametros = "";
                string commandTextCHICAgrupamento = "";
                string commandTextCHICOrdenacao = "";

                //if (item.Name.Equals("Apolo"))
                //{
                //    commandTextCHICCabecalho =
                //        "select * ";

                //    commandTextCHICTabelas =
                //        "from " +
                //            "VU_Lanc_Estoque_Ovos ";

                //    commandTextCHICCondicaoJoins =
                //        "where [Geração] = 'GP' and ";

                //    commandTextCHICCondicaoFiltros = "";

                //    commandTextCHICCondicaoParametros =
                //            "[Data Mov.] between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                //    commandTextCHICAgrupamento = "";

                //    commandTextCHICOrdenacao =
                //        "order by " +
                //            "1, 2, 5, 14";

                //    item.OLEDBConnection.CommandText =
                //        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                //        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                //        commandTextCHICOrdenacao;
                //}
                if (item.Name.Equals("IncubacaoWEB"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "HATCHERY_EGG_DATA ";

                    commandTextCHICCondicaoJoins =
                        "where Location = 'GP' and ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                            "Set_Date between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("DEOs"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "LayoutDiarioExpedicaos ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "DataHoraCarreg >= '2015-06-01' and Incubatorio = 'PH' and ";

                    commandTextCHICCondicaoParametros =
                            "DataHoraCarreg between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("hatchery"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "Vu_Hatchery_Rel V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.Local = 'PH' and ";

                    commandTextCHICCondicaoParametros =
                            "V.Data_Nascimento-21 between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("producao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_DIARIO_COMPLETO V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.\"Data de Produção\" >= TO_DATE('01/06/2015','dd/MM/yyyy HH24:MI:SS') and V.Location = 'GP' and ";

                    commandTextCHICCondicaoParametros =
                            "V.\"Data de Produção\" between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFExportacao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '1' " +
                        "and NatOpCodEstr like '7.%' and I.FiscalItNFNomeProd like '%OVOS%AVO%' and F.FiscalNFCancelada = 'Não' and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Lotes"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Flocks ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "Farm_ID like 'SB%' and ";

                    commandTextCHICCondicaoParametros =
                        "Trx_Date between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by 1, 2, 5";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("EstoqueDetalhado"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "[Local Estoque] in ('PH') and ";

                    string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '"
                            + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "Order by 4, 6, 7";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Inventario"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios_Inventario ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "[Local Estoque] in ('PH') and ";

                    string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '"
                            + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by 1, 3, 2, 4";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
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

        public string GeraRelatorioMatrizPorIncubatorio(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            string origem = incubatorio;
            if (origem == "NMNF") incubatorio = "NM";

            string empresaApolo = "";
            if (incubatorio.Equals("NM"))
                //empresaApolo = "20";
                empresaApolo = "30";
            else
                empresaApolo = "1";

            if (origem == "NMNF")
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Matrizes_" 
                    + incubatorio + "_Novo.xlsx", destino);
            else
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Matrizes_" 
                    + incubatorio + ".xlsx", destino);

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

            // Parâmetros
            string dataInicialStrSQLServer = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSQLServer = dataFinal.ToString("yyyy-MM-dd");

            string dataInicialStrOracle = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStrOracle = dataFinal.ToString("dd/MM/yyyy");

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Estoque Diário"];
            worksheet.Cells[2, 5] = dataInicial;
            worksheet.Cells[2, 7] = dataFinal;
            worksheet.Cells[5, 2] = dataInicial;

            Excel._Worksheet worksheetInventario = (Excel._Worksheet)oBook.Worksheets["Inventário de Ovos"];
            worksheetInventario.Cells[2, 5] = DateTime.Today;
            
            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                string commandTextCHICCabecalho = "";
                string commandTextCHICTabelas = "";
                string commandTextCHICCondicaoJoins = "";
                string commandTextCHICCondicaoFiltros = "";
                string commandTextCHICCondicaoParametros = "";
                string commandTextCHICAgrupamento = "";
                string commandTextCHICOrdenacao = "";

                //if (item.Name.Equals("Apolo"))
                //{
                //    commandTextCHICCabecalho =
                //        "select * ";

                //    commandTextCHICTabelas =
                //        "from " +
                //            "VU_Lanc_Estoque_Ovos ";

                //    commandTextCHICCondicaoJoins =
                //        "where [Geração] = 'PP' and " +
                //        "[Granja / Incubatório] = '" + incubatorio + "' and ";

                //    commandTextCHICCondicaoFiltros = "";

                //    commandTextCHICCondicaoParametros =
                //            "[Data Mov.] between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                //    commandTextCHICAgrupamento = "";

                //    commandTextCHICOrdenacao =
                //        "order by " +
                //            "1, 2, 5, 14";

                //    item.OLEDBConnection.CommandText =
                //        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                //        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                //        commandTextCHICOrdenacao;
                //}
                if (item.Name.Equals("IncubacaoWEB"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "HATCHERY_EGG_DATA ";

                    commandTextCHICCondicaoJoins =
                        "where location = 'PP' and " +
                        "hatch_loc = '" + incubatorio + "' and ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                            "Set_Date between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("DEOs"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "LayoutDiarioExpedicaos ";

                    commandTextCHICCondicaoJoins =
                        "where (Granja = '" + incubatorio + "' or Incubatorio = '" + incubatorio + "' or Granja = 'PL' or Granja = '" + incubatorio + "C') and ";

                    commandTextCHICCondicaoFiltros = "DataHoraCarreg >= '2015-06-01' and ";

                    commandTextCHICCondicaoParametros =
                            "((DataHoraCarreg between '" + dataInicialStrSQLServer + " 00:00:00' and '" 
                                + dataFinalStrSQLServer + " 23:59:59') or " +
                            " (DataHoraRecebInc between '" + dataInicialStrSQLServer + " 00:00:00' and '"
                                + dataFinalStrSQLServer + " 23:59:59')) ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("hatchery"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "Vu_Hatchery_Rel V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.Local = '" + incubatorio +"' and ";

                    commandTextCHICCondicaoParametros =
                            "V.Data_Nascimento-21 between TO_DATE('"
                            + dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('"
                            + dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("producao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_DIARIO_COMPLETO V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.\"Data de Produção\" >= TO_DATE('01/06/2015','dd/MM/yyyy HH24:MI:SS') and V.Location = 'PP' and ";

                    commandTextCHICCondicaoParametros =
                            "V.\"Data de Produção\" between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFDoacao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '" + empresaApolo + "' and I.FiscalItNFNomeProd like '%OVO%' " +
                        "and NatOpCodEstr like '%.910' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFExportacao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '" + empresaApolo + "' " +
                        //"F.FiscalNFIndMov = 'Saída' " +
                        "and NatOpCodEstr like '7.%' and I.FiscalItNFNomeProd like '%OVOS%' " +
                        "and F.FiscalNFRazaoSocial not like '%HY LINE%' and F.FiscalNFRazaoSocial not like '%LOHMANN%'" +
                        "and F.FiscalNFRazaoSocial not like '%H&N%' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFOvosComerciais"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '" + empresaApolo + "' " +
                        "and NatOpCodEstr like '%.101%' and I.FiscalItNFNomeProd like '%OVO%COM%' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFVendasOvosGranjas"))
                {
                    commandTextCHICCabecalho =
                        "select P.ProdNomeAlt1, F.*, I.* ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "inner join PRODUTO P on I.ProdCodEstr = P.ProdCodEstr " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Entrada' and F.EmpCod = '" + empresaApolo + "' " +
                        "and F.FiscalNFRazaoSocial like '%HY%LINE%' and I.FiscalItNFNomeProd like '%OVO%FERT%' and F.FiscalNFCancelada = 'Não' and " +
                        //"F.FiscalNFCnpj not in ('02924519000787') and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "inner join NOTA_FISCAL NF on INF.EmpCod = NF.EmpCod and INF.CtrlDFModForm = NF.CtrlDFModForm " +
                                "and INF.CtrlDFSerie = NF.CtrlDFSerie and INF.NFNum = NF.NFNum " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda' and F.EntCod = NF.EntCod) and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFVendasOvosGranjasAgrup"))
                {
                    commandTextCHICCabecalho =
                        "select F.EmpCod, F.FiscalNFDataEmis, F.FiscalNFCnpj, F.FiscalNFNum, SUM(I.FiscalItNFQtd) FiscalItNFQtd ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "inner join PRODUTO P on I.ProdCodEstr = P.ProdCodEstr " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Entrada' and F.EmpCod = '" + empresaApolo + "' " +
                        "and F.FiscalNFRazaoSocial like '%HY%LINE%' and I.FiscalItNFNomeProd like '%OVO%FERT%' and F.FiscalNFCancelada = 'Não' and " +
                        //"F.FiscalNFCnpj not in ('02924519000787') and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "inner join NOTA_FISCAL NF on INF.EmpCod = NF.EmpCod and INF.CtrlDFModForm = NF.CtrlDFModForm " +
                                "and INF.CtrlDFSerie = NF.CtrlDFSerie and INF.NFNum = NF.NFNum " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda' and F.EntCod = NF.EntCod) and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "group by F.EmpCod, F.FiscalNFCnpj, F.FiscalNFNum, F.FiscalNFDataEmis";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFsEntradaOvosComerciais"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Entrada' and F.EmpCod = '" + empresaApolo + "' " +
                        "and F.FiscalNFRazaoSocial like '%HY%LINE%' and I.FiscalItNFNomeProd like '%OVO%COM%' and F.FiscalNFCancelada = 'Não' and " +
                        //"F.FiscalNFCnpj not in ('02924519000787') and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "inner join NOTA_FISCAL NF on INF.EmpCod = NF.EmpCod and INF.CtrlDFModForm = NF.CtrlDFModForm " +
                                "and INF.CtrlDFSerie = NF.CtrlDFSerie and INF.NFNum = NF.NFNum " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda' and F.EntCod = NF.EntCod) and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFTransfNMparaNG"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '" + empresaApolo + "' " +
                        "and F.FiscalNFRazaoSocial like '%HY%LINE%' and I.FiscalItNFNomeProd like '%OVO%' and F.FiscalNFCancelada = 'Não' " +
                        "and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFVendaOvos"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "inner join ENTIDADE E on F.EntCod = E.EntCod " +
                        "inner join CIDADE C on E.CidCod = C.CidCod " +
                        "where ";

                    string empresasVendaOvos = "('1','12','15')";
                    if (incubatorio.Equals("NM")) empresasVendaOvos = "('20')";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod in " + empresasVendaOvos + " " +
                        "and (FiscalItNFNatOpCodEstr like '%.101' or FiscalItNFNatOpCodEstr like '%.102') " +
                        "and (I.FiscalItNFNomeProd like '%INCUBÁVEIS%' or I.FiscalItNFNomeProd like '%OVOS%FÉRTEIS%') " +
                        "and C.UfSigla <> 'EX' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("IncubacaoFLIP"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "vu_hatchery_rel_trx_date V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.Local = '" + incubatorio + "' and ";

                    commandTextCHICCondicaoParametros =
                            "V.Data_Nascimento-21 between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("NFTransfHygen"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "FISCAL_NF F ";

                    commandTextCHICCondicaoJoins =
                        "inner join FISCAL_ITEM_NF I on F.EmpCod = I.EmpCod and F.FiscalNFChv = I.FiscalNFChv " +
                        "where ";

                    commandTextCHICCondicaoFiltros =
                        "F.FiscalNFIndMov = 'Saída' and F.EmpCod = '1' " +
                        "and NatOpCodEstr like '%.901' and F.FiscalNFCancelada = 'Não' and " +
                        "0 = (select COUNT(1) from ITEM_NF INF " +
                             "where F.EmpCod = INF.EmpCod and F.FiscalNFEspec = INF.ItNFOrigEspec " +
                             "and F.FiscalNFSerie = INF.ItNFOrigSerie and F.FiscalNFNum = INF.ItNFOrigNum " +
                             "and INF.ItNFOrigMod = 'Doc Venda') and ";

                    commandTextCHICCondicaoParametros =
                            "F.FiscalNFDataEmis between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("EstoqueDetalhado"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '"
                            + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "Order by 4, 6, 7";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Inventario"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios_Inventario ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    string dataInicialStrSQLServerPrd = dataInicial.AddDays(-30).ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '"
                            + dataFinalStrSQLServer + "' and " +
                        "[Local Estoque] = '" + incubatorio + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by 1, 3, 2, 4";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("SaldoIncubaveis"))
                {
                    commandTextCHICCabecalho =
                        "select Local, Origem, DataSaldo, SUM(Qtde) Saldo ";

                    commandTextCHICTabelas =
                        "from " +
                            "VW_Saldo_Estq_Ovos_Incubaveis ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                        "DataSaldo < '" + dataFinalStrSQLServer + "' and " +
                        "Local = '" + incubatorio + "' ";

                    commandTextCHICAgrupamento = "group by Local, Origem, DataSaldo ";

                    commandTextCHICOrdenacao = "order by DataSaldo, Origem";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("SaldoOvosComercio"))
                {
                    commandTextCHICCabecalho =
                        "select Local, Origem, DataSaldo, SUM(Qtde) Saldo ";

                    commandTextCHICTabelas =
                        "from " +
                            "VW_Saldo_Estq_Ovos_Comercio ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                        "DataSaldo < '" + dataFinalStrSQLServer + "' and " +
                        "Local = '" + incubatorio + "C' ";

                    commandTextCHICAgrupamento = "group by Local, Origem, DataSaldo ";

                    commandTextCHICOrdenacao = "order by DataSaldo, Origem";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("DEOsEntrada"))
                {
                    commandTextCHICCabecalho =
                        "select Case When len(L.Granja) > 2 or L.Granja in ('SD','2I') Then 'BW'" +
                            " Else L.Granja End Origem, " +
                            " L.NFNum, P.Cor, SUM(QtdeOvos+ISNULL(QtdDiferenca ,0)) QtdeReal ";

                    commandTextCHICTabelas =
                        "from " +
                            "LayoutDiarioExpedicaos L With(Nolock) ";

                    commandTextCHICCondicaoJoins =
                        "inner join HLBAPP.dbo.LINHAGEM_GRUPO P With(Nolock) on L.Linhagem = P.LinhagemFLIP " +
                            "and ((P.LinhagemFLIP = 'LSLC' and P.Empresa = 'PL') " +
		                    "or (P.LinhagemFLIP = 'LBWN' and P.Empresa = 'PL') " +
		                    "or P.LinhagemFLIP not in ('LSLC','LBWN')) ";

                    commandTextCHICCondicaoFiltros =
                        "where (Incubatorio = 'NM' or Granja = 'PL') " + 
                            "and DataHoraCarreg >= '2015-06-01' " +
                            "and (TipoDEO+Incubatorio in ('Ovos IncubáveisNM') or " +
	                        "(TipoDEO+Incubatorio in ('Transf. Ovos IncubáveisNM') and Granja = 'CH')) " +
                            "and Importado = 'Conferido' ";

                    commandTextCHICCondicaoParametros =
                        "and ((DataHoraCarreg between '" + dataInicialStrSQLServer +
                            "' and '" + dataFinalStrSQLServer + "') or " +
                            "(DataHoraRecebInc between '" + dataInicialStrSQLServer + 
                            "' and '" + dataFinalStrSQLServer + "')) ";

                    commandTextCHICAgrupamento = "group by " +
                        "Case When len(L.Granja) > 2 or L.Granja in ('SD','2I') Then 'BW' Else L.Granja End, " +
                            "L.NFNum, P.Cor ";

                    commandTextCHICOrdenacao = "order by 2, 3";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
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

        public string GeraInventarioMatrizPorIncubatorio(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Inventario_Ovos_Matrizes_" + incubatorio + ".xlsx", destino);

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

            // Parâmetros
            string dataInicialStrSQLServer = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrSQLServer = dataFinal.ToString("yyyy-MM-dd");

            string dataInicialStrOracle = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStrOracle = dataFinal.ToString("dd/MM/yyyy");

            Excel._Worksheet worksheetInventario = (Excel._Worksheet)oBook.Worksheets["Inventário de Ovos"];
            //worksheetInventario.Cells[2, 5] = DateTime.Today;
            worksheetInventario.Cells[2, 5] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                string commandTextCHICCabecalho = "";
                string commandTextCHICTabelas = "";
                string commandTextCHICCondicaoJoins = "";
                string commandTextCHICCondicaoFiltros = "";
                string commandTextCHICCondicaoParametros = "";
                string commandTextCHICAgrupamento = "";
                string commandTextCHICOrdenacao = "";

                if (item.Name.Equals("IncubacaoWEB"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "HATCHERY_EGG_DATA ";

                    commandTextCHICCondicaoJoins =
                        "where location = 'PP' and " +
                        "hatch_loc = '" + incubatorio + "' and ";

                    commandTextCHICCondicaoFiltros = "";

                    commandTextCHICCondicaoParametros =
                            "Set_Date between '" + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("DEOs"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "LayoutDiarioExpedicaos ";

                    commandTextCHICCondicaoJoins =
                        "where (Incubatorio = '" + incubatorio + "' or Granja = 'PL' or Granja = '" + incubatorio + "C') and ";

                    commandTextCHICCondicaoFiltros = "DataHoraCarreg >= '2015-06-01' and ";

                    commandTextCHICCondicaoParametros =
                            "((DataHoraCarreg between '" + dataInicialStrSQLServer + " 00:00:00' and '"
                                + dataFinalStrSQLServer + " 23:59:59') or " +
                            " (DataHoraRecebInc between '" + dataInicialStrSQLServer + " 00:00:00' and '"
                                + dataFinalStrSQLServer + " 23:59:59')) ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("producao"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_DIARIO_COMPLETO V ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "V.\"Data de Produção\" >= TO_DATE('01/06/2015','dd/MM/yyyy HH24:MI:SS') and V.Location = 'PP' and ";

                    commandTextCHICCondicaoParametros =
                            "V.\"Data de Produção\" between TO_DATE('" +
                            dataInicialStrOracle + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                            dataFinalStrOracle + "','dd/MM/yyyy HH24:MI:SS') ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
                else if (item.Name.Equals("Inventario"))
                {
                    commandTextCHICCabecalho =
                        "select * ";

                    commandTextCHICTabelas =
                        "from " +
                            "VU_Estoque_Ovos_Incubatorios_Inventario ";

                    commandTextCHICCondicaoJoins =
                        "where ";

                    commandTextCHICCondicaoFiltros = "";

                    string dataInicialStrSQLServerPrd = dataInicial.ToString("yyyy-MM-dd");

                    commandTextCHICCondicaoParametros =
                        "[Data Produção] between '" + dataInicialStrSQLServerPrd + "' and '"
                            + dataFinalStrSQLServer + "' and " +
                        "[Local Estoque] = '" + incubatorio + "' ";

                    commandTextCHICAgrupamento = "";

                    commandTextCHICOrdenacao = "order by 1, 3, 2, 4";

                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
                }
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

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            string destino = "";
            string pesquisa = "";
            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";

            if (ddlIncubatorio.SelectedValue.Equals("PP"))
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
                pesquisa = "*Relatorio_Estoque_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

                Session["destinoRelEstoqueOvos"] = GeraRelatorioMatriz(pesquisa, true, pasta, destino, calDataInicial.SelectedDate,
                    calDataFinal.SelectedDate);
            }
            else if (ddlIncubatorio.SelectedValue.Equals("GP"))
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Avos_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
                pesquisa = "*Relatorio_Estoque_Ovos_Avos_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

                Session["destinoRelEstoqueOvos"] = GeraRelatorioAvos(pesquisa, true, pasta, destino, calDataInicial.SelectedDate,
                    calDataFinal.SelectedDate);
            }
            else if (ddlIncubatorio.SelectedValue.Equals("NMNF"))
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Estoque_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
                pesquisa = "*Relatorio_Estoque_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

                Session["destinoRelEstoqueOvos"] = GeraRelatorioMatrizPorIncubatorio(pesquisa, true, pasta, destino, calDataInicial.SelectedDate,
                    calDataFinal.SelectedDate, ddlIncubatorio.SelectedValue);
            }
            else
            {
                destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Inventario_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
                pesquisa = "*Relatorio_Inventario_Ovos_Matrizes_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

                Session["destinoRelEstoqueOvos"] = GeraInventarioMatrizPorIncubatorio(pesquisa, true, pasta, destino, calDataInicial.SelectedDate,
                    calDataFinal.SelectedDate, ddlIncubatorio.SelectedValue);
            }

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelEstoqueOvos"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            if (ddlIncubatorio.SelectedValue.Equals("PP"))
                Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_Estoque_Ovos_Matrizes_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            else if (ddlIncubatorio.SelectedValue.Equals("GP"))
                Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_Estoque_Ovos_Avos_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            else
                Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_Estoque_Ovos_" + ddlIncubatorio.SelectedValue + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}