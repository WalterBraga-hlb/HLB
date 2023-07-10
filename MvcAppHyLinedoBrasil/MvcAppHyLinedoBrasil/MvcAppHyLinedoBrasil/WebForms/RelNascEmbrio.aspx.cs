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
    public partial class RelNascEmbrio : System.Web.UI.Page
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
            }
        }

        public string GeraRelatorioListaPedidos(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Nascimento_Embrio.xlsx", destino);

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

            #region FLIP

            string commandTextCHICCabecalho =
                "select * ";

            string commandTextCHICTabelas =
                "from " +
                    "vu_rel_setting_excel V ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStr = dataFinal.ToString("dd/MM/yyyy");

            string commandTextCHICCondicaoParametros =
                    "V.Data_Nascimento between TO_DATE('" +
                    dataInicialStr + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                    dataFinalStr + "','dd/MM/yyyy HH24:MI:SS') ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao = "";

            #endregion

            #region WEB

            string commandTextCHICCabecalhoWEB =
                "select " +
                 "* ";

            string commandTextCHICTabelasWEB =
                "from " +
                    "HATCHERY_FLOCK_SETTER_DATA ";

            string commandTextCHICCondicaoJoinsWEB =
                "where ";

            string commandTextCHICCondicaoFiltrosWEB = "";

            string dataInicialStrWEB = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrWEB = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosWEB =
                    "set_date+21 between '" +
                    dataInicialStrWEB + "' and '" + dataFinalStrWEB + "' ";

            string commandTextCHICAgrupamentoWEB = "";

            string commandTextCHICOrdenacaoWEB =
                "order by " +
                    "2,3,6,4";

            #endregion

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Analise de Linhagens"];

            worksheet.Cells[2, 8] = dataInicial;
            worksheet.Cells[3, 8] = dataFinal;

            Excel._Worksheet worksheet02 = (Excel._Worksheet)oBook.Worksheets["Embriodiagnóstico - Simples"];

            worksheet02.Cells[2, 8] = dataInicial;
            worksheet02.Cells[3, 8] = dataFinal;

            Excel._Worksheet worksheet04 = (Excel._Worksheet)oBook.Worksheets["Embriodiagnóstico - Pond."];

            worksheet04.Cells[2, 8] = dataInicial;
            worksheet04.Cells[3, 8] = dataFinal;

            Excel._Worksheet worksheet03 = (Excel._Worksheet)oBook.Worksheets["Embriodiagnóstico - WEB"];

            worksheet03.Cells[2, 8] = dataInicial;
            worksheet03.Cells[3, 8] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("brflocks (padrão)"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + 
                        commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;

                if (item.Name.Equals("HLBAPP"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoWEB + commandTextCHICTabelasWEB + commandTextCHICCondicaoJoinsWEB +
                        commandTextCHICCondicaoFiltrosWEB + commandTextCHICCondicaoParametrosWEB +
                        commandTextCHICAgrupamentoWEB +
                        commandTextCHICOrdenacaoWEB;
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
            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Nascimento_Embrio_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Relatorio_Nascimento_Embrio_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            Session["destinoRelNascEmbrio"] = GeraRelatorioListaPedidos(pesquisa, true, pasta, destino, calDataInicial.SelectedDate,
                calDataFinal.SelectedDate);

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelNascEmbrio"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_Nascimento_Embrio_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}