using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelConfIncWebXFlip : System.Web.UI.Page
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

                Session["destino"] = "";
                Session["extensaoArquivo"] = "";
                calDataInicial.SelectedDate = DateTime.Today;
                calDataFinal.SelectedDate = DateTime.Today;
            }
        }

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            string destino = "";

            if (ddlRelatorio.SelectedValue.Equals("WEB X FLIP"))
            {
                #region Relatorio WEB X FLIP

                Session["extensaoArquivo"] = ".xlsm";
                Session["destino"] = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Conf_Web_X_FLIP_" + Session["login"].ToString() + Session.SessionID + ".xlsm";
                destino = Session["destino"].ToString();

                string pesquisa = "*Relatorio_Conf_Web_X_FLIP_" + Session["login"].ToString() + "*.xlsm";

                string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }

                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Conf_Web_X_FLIP.xlsm", destino);

                // Object for missing (or optional) arguments.
                object oMissing = System.Reflection.Missing.Value;

                // Create an instance of Microsoft Excel, make it visible,
                // and open Book1.xls.
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

                Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Conferência"];
                //worksheet = oBook.ActiveSheet;

                if (ddlOrigem.SelectedValue == "(Todas)")
                    worksheet.Cells[3, 10] = "";
                else
                    worksheet.Cells[3, 10] = ddlOrigem.SelectedValue;

                if (ddlLotes.SelectedValue == "(Todos)")
                    worksheet.Cells[3, 11] = "";
                else
                    worksheet.Cells[3, 11] = ddlLotes.SelectedValue;

                if (ddlSetters.SelectedValue == "(Todos)")
                    worksheet.Cells[3, 12] = "";
                else
                    worksheet.Cells[3, 12] = ddlSetters.SelectedValue;

                worksheet.Cells[4, 8] = calDataInicial.SelectedDate;
                worksheet.Cells[4, 10] = calDataFinal.SelectedDate;
                worksheet.Cells[3, 8] = ddlTipoData.SelectedValue;

                oBook.Save();

                // Run the macros.
                RunMacro(oExcel, new Object[] { "AtualizaRelatorio" });
                System.Threading.Thread.Sleep(30000);

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
            }
            else if (ddlRelatorio.SelectedValue.Equals("ESTOQUE FUTURO"))
            {
                #region Relatório INCUBAÇÕES FUTURAS

                Session["extensaoArquivo"] = ".xlsx";
                Session["destino"] = "C:\\inetpub\\wwwroot\\Relatorios\\IncubacoesFuturasNaoImportadas\\"
                    + "IncubacoesFuturasNaoImportadas_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
                destino = Session["destino"].ToString();

                string pesquisa = "*IncubacoesFuturasNaoImportadas_" + Session["login"].ToString() + "*.xlsx";

                string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios\\" 
                    + "IncubacoesFuturasNaoImportadas", pesquisa);

                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }

                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\IncubacoesFuturasNaoImportadas\\"
                    + "IncubacoesFuturasNaoImportadas.xlsx", destino);

                // Object for missing (or optional) arguments.
                object oMissing = System.Reflection.Missing.Value;

                // Create an instance of Microsoft Excel, make it visible,
                // and open Book1.xls.
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

                string commandTextCHICCabecalho =
                "select " +
                      "* ";

                string commandTextCHICTabelas =
                    "from " +
                        "VW_Incubacoes_Futuras_Nao_Importadas ";

                string commandTextCHICCondicaoJoins =
                    "where ";

                string commandTextCHICCondicaoFiltros = "";

                string dataInicialStrSQLServer = calDataInicial.SelectedDate.ToString("yyyy-MM-dd");
                string dataFinalStrSQLServer = calDataFinal.SelectedDate.ToString("yyyy-MM-dd");

                string commandTextCHICCondicaoParametros =
                        "(('" + ddlTipoData.SelectedValue + "' = 'I' and [Data Incubação] between '" 
                            + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "') or " +
                        "('" + ddlTipoData.SelectedValue + "' = 'N' and [Data Nascimento] between '"
                            + dataInicialStrSQLServer + "' and '" + dataFinalStrSQLServer + "')) and " +
                        "([Incubatório] = '" + ddlOrigem.SelectedValue + "' or '" 
                            + ddlOrigem.SelectedValue + "' = '(Todas)') and " +
                        "([Incubadora] = '" + ddlSetters.SelectedValue + "' or '"
                            + ddlSetters.SelectedValue + "' = '(Todos)') and " +
                        "([Lote Completo] = '" + ddlLotes.SelectedValue + "' or '" 
                            + ddlLotes.SelectedValue + "' = '(Todos)') ";

                string commandTextCHICAgrupamento = "";

                string commandTextCHICOrdenacao =
                    "order by " +
                        "2, 4, 5, 6, 8";

                Connections lista = oBook.Connections;

                foreach (Excel.WorkbookConnection item in lista)
                {
                    item.OLEDBConnection.BackgroundQuery = false;
                    if (item.Name.Equals("HLBAPP"))
                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
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

                #endregion
            }

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);
        }

        private void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run",
                System.Reflection.BindingFlags.Default |
                System.Reflection.BindingFlags.InvokeMethod,
                null, oApp, oRunArgs);
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destino"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=" 
                + "Conferencia_Incubacao_" + ddlRelatorio.SelectedValue.Replace(" ","_") + "_"
                + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + Session["extensaoArquivo"].ToString());
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}