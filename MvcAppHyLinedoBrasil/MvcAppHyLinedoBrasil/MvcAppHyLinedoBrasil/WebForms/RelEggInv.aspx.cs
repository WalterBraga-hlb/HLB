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
    public partial class RelEggInv : System.Web.UI.Page
    {
        public static string destino;
        public static string mesmaSessao;

        //[DllImport("user32.dll")]
        //static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

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
                calDataProdFinal.SelectedDate = DateTime.Today;
                calDataProdInicial.SelectedDate = DateTime.Today;
            }
        }

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_MovEstoqueOvos_" + Session["login"].ToString() + Session.SessionID + ".xlsm";

            string pesquisa = "*Relatorio_MovEstoqueOvos_" + Session["login"].ToString() + "*.xlsm";

            string[] files = Directory.GetFiles("C:\\inetpub\\wwwroot\\Relatorios", pesquisa);

            foreach (var item in files)
            {
                //mesmaSessao = item;
                System.IO.File.Delete(item);
            }

            //if ((mesmaSessao != destino) && (mesmaSessao != null))
            //{
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_MovEstoqueOvos.xlsm", destino);
            //}

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

            //uint idExcel;
            //GetWindowThreadProcessId((IntPtr)oExcel.Hwnd, out idExcel);

            oExcel.Visible = true;
            Excel.Workbooks oBooks = oExcel.Workbooks;
            Excel._Workbook oBook = null;
            oBook = oBooks.Open(destino, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                //oMissing, oMissing, oMissing, oMissing, oMissing, XlCorruptLoad.xlRepairFile); // Quando abre arquivo corrompido
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Movimentações de Ovos"];
            //worksheet = oBook.ActiveSheet;

            if (ddlOrigem.SelectedValue == "(Todas)")
                worksheet.Cells[4, 6] = "";
            else
                worksheet.Cells[4, 6] = ddlOrigem.SelectedValue;
            if (ddlLotes.SelectedValue == "(Todos)")
                worksheet.Cells[5, 6] = "";
            else
                worksheet.Cells[5, 6] = ddlLotes.SelectedValue;
            worksheet.Cells[4, 8] = calDataInicial.SelectedDate;
            worksheet.Cells[5, 8] = calDataFinal.SelectedDate;

            worksheet.Cells[4, 9] = calDataProdInicial.SelectedDate;
            worksheet.Cells[5, 9] = calDataProdFinal.SelectedDate;
            
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
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_MovEstoqueOvos_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsm");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}