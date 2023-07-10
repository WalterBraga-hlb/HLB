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
using System.Globalization;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelDiarioProducao : System.Web.UI.Page
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
                LoadLabels();
                Session["empresasSelecionadas"] = "";
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";
                calDataFinal.SelectedDate = DateTime.Today;
                calDataInicial.SelectedDate = DateTime.Today;
                CheckDireitosTiposGranjas();
                LoadFarms();
                //ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                //scriptManager.RegisterPostBackControl(this.lkbDownload);
            }
        }

        protected void btnGerar_Click(object sender, EventArgs e)
        {
            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Rel_Diario_Completo\\Rel_Diario_Completo_" + ddlGranja.SelectedValue
                + "_" + Session["login"].ToString() + ".xlsm";

            if (System.IO.File.Exists(destino))
            {
                System.IO.File.Delete(destino);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Rel_Diario_Completo\\Rel_Diario_Completo_" + ddlGranja.SelectedValue
                + ".xlsm", destino);

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

            string listaFazendas = LoadSelectedFarmsAll();

            Excel._Worksheet worksheet = null;
            if (!ddlGranja.SelectedItem.Text.Contains("Brasil"))
            {
                worksheet = (Excel._Worksheet)oBook.Worksheets["Reporte de Produccion"];
                worksheet.Cells[4, 5] = listaFazendas;
                worksheet.Cells[5, 5] = calDataInicial.SelectedDate;
                worksheet.Cells[5, 7] = calDataFinal.SelectedDate;
            }
            else
            {
                worksheet = (Excel._Worksheet)oBook.Worksheets["Relatório Diário Completo"];
                //worksheet = oBook.ActiveSheet;

                //if (ddlFazenda.SelectedValue == "(Todas)")
                //    worksheet.Cells[1, 6] = "";
                //else
                //    worksheet.Cells[1, 6] = ddlFazenda.SelectedValue;
                worksheet.Cells[1, 6] = listaFazendas;
                if (ddlGranja.SelectedValue == "(Todas)")
                    worksheet.Cells[2, 6] = "";
                else
                    worksheet.Cells[2, 6] = ddlGranja.SelectedValue;
                //worksheet.Cells[2, 6] = ddlGranja.SelectedValue;
                worksheet.Cells[2, 9] = calDataInicial.SelectedDate;
                worksheet.Cells[2, 11] = calDataFinal.SelectedDate;
            }

            //if (ddlGranja.SelectedValue.Equals("PP") || ddlGranja.SelectedValue.Equals("GP"))
            //{
            //    Excel._Worksheet worksheetEstq = (Excel._Worksheet)oBook.Worksheets["Estoque de Ovos"];
            //    worksheetEstq.Cells[3, 7] = DateTime.Now;
            //}

            //worksheet.Cells.EntireColumn.AutoFit();     

            #region FLIP

            string commandTextCHICCabecalhoFLIP =
                "select * ";

            string commandTextCHICTabelasFLIP =
                "from " +
                    "VU_DIARIO_COMPLETO V ";

            string commandTextCHICCondicaoJoinsFLIP =
                "where ";

            string commandTextCHICCondicaoFiltrosFLIP = "";

            //string dataInicialStrFLIP = calDataInicial.SelectedDate.ToString("dd/MM/yyyy");
            string dataInicialStrFLIP = calDataInicial.SelectedDate.ToString(CultureInfo.GetCultureInfo("pt-BR"));
            //string dataFinalStrFLIP = calDataFinal.SelectedDate.ToString("dd/MM/yyyy");
            string dataFinalStrFLIP = calDataFinal.SelectedDate.ToString(CultureInfo.GetCultureInfo("pt-BR"));

            string commandTextCHICCondicaoParametrosFLIP =
                    "V.\"Data de Produção\" between TO_DATE('" + dataInicialStrFLIP + "','dd/MM/yyyy HH24:MI:SS') "
                    + "and TO_DATE('" + dataFinalStrFLIP + "','dd/MM/yyyy HH24:MI:SS') "
                    + "and (V.Location = '" + ddlGranja.SelectedValue + "' or '" 
                        + ddlGranja.SelectedValue + "' is null) "
                    + "and (Instr('" + listaFazendas + "','*'||V.\"Núcleo\"||'*') > 0 or "
                        + "Instr('" + listaFazendas + "','*'||Substr(V.\"Núcleo\",1,2)||'*') > 0 or "
                        + "Instr('" + listaFazendas + "','*'||V.\"Cod. FLIP Granja\"||'*') > 0) ";

            if (!ddlGranja.SelectedItem.Text.Contains("Brasil"))
            {
                commandTextCHICCondicaoParametrosFLIP =
                    "V.\"Fecha de Producción\" between TO_DATE('" + dataInicialStrFLIP + "','dd/MM/yyyy HH24:MI:SS') "
                    + "and TO_DATE('" + dataFinalStrFLIP + "','dd/MM/yyyy HH24:MI:SS') "
                    //+ "and (V.Location = '" + ddlGranja.SelectedValue + "' or '" 
                        //+ ddlGranja.SelectedValue + "' is null) "
                    + "and (Instr('" + listaFazendas + "','*'||V.\"Modulo\"||'*') > 0 or "
                        + "Instr('" + listaFazendas + "','*'||Substr(V.\"Modulo\",1,2)||'*') > 0) ";
            }

            string commandTextCHICAgrupamentoFLIP = "";

            string commandTextCHICOrdenacaoFLIP = "";

            #endregion

            // Run the macros.
            RunMacro(oExcel, new Object[] { "AtualizaRelatorio" });

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;

                if (item.Name.Equals("FLIP"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoFLIP + commandTextCHICTabelasFLIP + 
                        commandTextCHICCondicaoJoinsFLIP +
                        commandTextCHICCondicaoFiltrosFLIP + commandTextCHICCondicaoParametrosFLIP +
                        commandTextCHICAgrupamentoFLIP + commandTextCHICOrdenacaoFLIP;
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
            if (ddlGranja.SelectedValue.Equals("PG") || ddlGranja.SelectedValue.Equals("GG"))
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=Rel_Diario_Recria_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsm");
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=Rel_Diario_Completo_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsm");
            }
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

        public void LoadFarms()
        {
            chkTodas.Checked = false;
            if (ddlGranja.SelectedValue == "MX")
            {
                chklFarms.DataSourceID = "LXFARMS";
                chklFarms.DataTextField = "FARM_NAME";
                chklFarms.DataValueField = "FARM_ID";
                chklFarms.DataBind();
            }
            else if (ddlGranja.SelectedValue == "CL")
            {
                chklFarms.DataSourceID = "CLFARMS";
                chklFarms.DataTextField = "FARM_NAME";
                chklFarms.DataValueField = "FARM_ID";
                chklFarms.DataBind();
            }
            else if (ddlGranja.SelectedValue == "HC")
            {
                chklFarms.DataSourceID = "HCFARMS";
                chklFarms.DataTextField = "FARM_NAME";
                chklFarms.DataValueField = "FARM_ID";
                chklFarms.DataBind();
            }
            else
            {
                chklFarms.DataSourceID = "FarmsSqlDataSource";
                chklFarms.DataTextField = "Nome";
                chklFarms.DataValueField = "Codigo";
                chklFarms.DataBind();
            }
        }

        protected void chkTodas_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in chklFarms.Items)
            {
                item.Selected = chkTodas.Checked;
            }
        }

        protected void ddlGranja_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFarms();
        }

        protected void CheckDireitosTiposGranjas()
        {
            ListItemCollection listaItens = new ListItemCollection();
            foreach (ListItem item in ddlGranja.Items)
            {
                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-AcessoGranjas" + item.Value.Substring(0, 1), (System.Collections.ArrayList)Session["Direitos"])
                    ||
                    MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-AcessoGranjas" + item.Value, (System.Collections.ArrayList)Session["Direitos"]))
                {
                    listaItens.Add(item);
                }
            }

            ddlGranja.Items.Clear();

            foreach (ListItem item in listaItens)
            {
                ddlGranja.Items.Add(item);
            }
        }

        public void LoadLabels()
        {
            Label5.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("Title_Report_Rel_Diario_Completo_WebDesktop",
                        Session["language"].ToString());
            lblTitulo.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Description_Report_Rel_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            lblGranja0.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Field_Granja_Report_Rel_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            lblFazenda.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Field_Farm_Report_Rel_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            lblDataInicial.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Field_DateIni_Report_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            lblDataFinal0.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Field_DateFim_Report_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            btnGerar.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("Btn_Gerar_Report_Diario_Completo_WebDesktop",
                    Session["language"].ToString());
            hlRelatoriosFLIP.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetTextOnLanguage("HL_FLIP_Reports",
                    Session["language"].ToString());
            //lblAguarde.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
            //    .GetTextOnLanguage("Lbl_Aguarde_Report_Diario_Completo_WebDesktop",
            //        Session["language"].ToString());
        }
    }
}