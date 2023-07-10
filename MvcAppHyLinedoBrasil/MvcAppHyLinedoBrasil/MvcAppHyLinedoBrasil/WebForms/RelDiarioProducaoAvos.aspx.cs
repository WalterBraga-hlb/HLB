using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Access = Microsoft.Office.Interop.Access;
using Excel = Microsoft.Office.Interop.Excel;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using MvcAppHyLinedoBrasil.Models.Apolo;
using ImportaIncubacao.Data.Apolo;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelDiarioProducaoAvos : System.Web.UI.Page
    {
        public static string destino;
        Hashtable myHashtable;

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

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
                Session["empresasSelecionadas"] = "";
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";
                calDataFinal.SelectedDate = DateTime.Today;
                calDataInicial.SelectedDate = DateTime.Today;
                //CarregaListaFazendaCheck();
                //ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                //scriptManager.RegisterPostBackControl(this.lkbDownload);
            }
        }

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            //CheckExcellProcesses();

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Brazil-GP_Weekly_Follow-Up_Report_" 
                + Session["login"].ToString() + ".xlsm";

            if (System.IO.File.Exists(destino))
            {
                System.IO.File.Delete(destino);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Brazil-GP_Weekly_Follow-Up_Report.xlsm", 
                destino);

            // Object for missing (or optional) arguments.
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

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Relatório Diário Completo"];
            //worksheet = oBook.ActiveSheet;

            //if (ddlFazenda.SelectedValue == "(Todas)")
            //    worksheet.Cells[1, 6] = "";
            //else
            //    worksheet.Cells[1, 6] = ddlFazenda.SelectedValue;
            worksheet.Cells[1, 6] = LoadSelectedFarmsAll();
            if (ddlGranja.SelectedValue == "(All)")
                worksheet.Cells[2, 6] = "";
            else
                worksheet.Cells[2, 6] = ddlGranja.SelectedValue;
            //worksheet.Cells[2, 6] = ddlGranja.SelectedValue;
            worksheet.Cells[2, 9] = calDataInicial.SelectedDate;
            worksheet.Cells[2, 11] = calDataFinal.SelectedDate;

            //if (ddlGranja.SelectedValue.Equals("PP") || ddlGranja.SelectedValue.Equals("GP"))
            //{
            //    Excel._Worksheet worksheetEstq = (Excel._Worksheet)oBook.Worksheets["Estoque de Ovos"];
            //    worksheetEstq.Cells[3, 7] = DateTime.Now;
            //}

            //worksheet.Cells.EntireColumn.AutoFit();            

            // Run the macros.
            RunMacro(oExcel, new Object[] { "AtualizaRelatorio" });

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
            }

            oBook.RefreshAll();

            //oBook.Save();

            //RunMacro(oExcel, new Object[]{"DoKbTestWithParameter","Hello from C# Client."});
            //System.Threading.Thread.Sleep(60000);

            // Quit Excel and clean up.
            oBook.Close(true, oMissing, oMissing);
            //oBook.Close(0);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBook);
            oBook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oBooks);
            oBooks = null;
            oExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            oExcel = null;

            P.Kill();

            GC.Collect();

            //Process ProcessoExcel = Process.GetProcessById(idExcel);
            //ProcessoExcel.Kill();

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);
        }

        private void RunMacro(object oApp, object[] oRunArgs)
        {
            object retorno = oApp.GetType().InvokeMember("Run",
                System.Reflection.BindingFlags.Default |
                System.Reflection.BindingFlags.InvokeMethod,
                null, oApp, oRunArgs);
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Brazil-GP_Weekly_Follow-Up_Report_" 
                + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsm");
            //Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
            //Response.WriteFile("c:\\AMD\\book1.xlsm");
        }

        private void CheckExcellProcesses()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");
            myHashtable = new Hashtable();
            int iCount = 0;

            foreach (Process ExcelProcess in AllProcesses)
            {
                myHashtable.Add(ExcelProcess.Id, iCount);
                iCount = iCount + 1;
            }
        }

        private void KillExcel()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");

            // check to kill the right process
            foreach (Process ExcelProcess in AllProcesses)
            {
                if (myHashtable.ContainsKey(ExcelProcess.Id) == false)
                    ExcelProcess.Kill();
            }

            AllProcesses = null;
        }

        private void KillExcel(int idProcess)
        {
            Process AllProcesses = Process.GetProcessById(idProcess);

            AllProcesses.Kill();

            AllProcesses = null;
        }

        public string LoadSelectedFarmsAll()
        {
            string farmsSelecteds = "";

            foreach (ListItem item in chklFarms.Items)
            {
                if (item.Selected) farmsSelecteds = farmsSelecteds + "*" + item.Value + "*";
            }

            return farmsSelecteds;
        }

        protected void chkTodas_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in chklFarms.Items)
            {
                item.Selected = chkTodas.Checked;
            }
        }
    }
}