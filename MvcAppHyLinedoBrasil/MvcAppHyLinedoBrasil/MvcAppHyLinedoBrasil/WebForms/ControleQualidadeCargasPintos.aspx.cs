using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Models.Apolo;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.IO;
using MvcAppHyLinedoBrasil.Models.Apolo.ApoloDataSetTableAdapters;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class ControleQualidadeCargasPintos : System.Web.UI.Page
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

            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }

            if (!IsPostBack)
            {
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }
            GridView3.DataBind();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            DateTime data = Calendar1.SelectedDate;
            DateTime dataTrocaSistema = Convert.ToDateTime("01/03/2021");

            if (data < dataTrocaSistema)
                GridView3.DataSourceID = "EggInvDataSource";
            else
                GridView3.DataSourceID = "PedidosAniPlan";

            GridView3.DataBind();
        }

        protected void UpdateProgress1_DataBinding(object sender, EventArgs e)
        {
            //Panel1.Visible = false;   
        }

        protected void UpdateProgress1_Load(object sender, EventArgs e)
        {
            //Panel1.Visible = true;
        }

        protected void UpdateProgress1_PreRender(object sender, EventArgs e)
        {
            //Panel1.Visible = false;
        }

        protected void UpdateProgress1_Unload(object sender, EventArgs e)
        {
            //Panel1.Visible = true;
        }

        protected void UpdateProgress1_Disposed(object sender, EventArgs e)
        {
            //Panel1.Visible = true;
        }

        protected static void Redirect(string url, string target, string windowFeatures)
        {
            HttpContext context = HttpContext.Current;
            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
            String.IsNullOrEmpty(windowFeatures))
            {
                context.Response.Redirect(url);
            }
            else
            {
                var page = (System.Web.UI.Page)context.Handler;
                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);
                string script = !String.IsNullOrEmpty(windowFeatures) ? @"window.open(""{0}"", ""{1}"", ""{2}"");" : @"window.open(""{0}"", ""{1}"");";
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(System.Web.UI.Page), "Redirect", script, true);
            }
        }

        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*Session["numPedido"] = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;
            Session["dataNascimento"] = Calendar1.SelectedDate;
            Session["empresaSelecionadaControleCargaPintos"] = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            Session["relatorioControleQualidadeCargasPintos"] = "ControleQualidadeCargas_BR.rpt";

            //Response.Write("<script>window.open('RelControleQualidadeCargas.aspx')</script>");
            Redirect("RelControleQualidadeCargas.aspx", "_blank", "");*/
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Session["origemRelatorio"] = "ControleCargas";

            Session["numPedido"] = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;
            Session["dataNascimento"] = Calendar1.SelectedDate;
            Session["empresaSelecionadaControleCargaPintos"] = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            Session["nomeCliente"] = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;
            Session["relatorioControleQualidadeCargasPintos"] = "ControleQualidadeCargas.rpt";

            //Response.Write("<script>window.open('RelControleQualidadeCargas.aspx')</script>");
            Redirect("RelControleQualidadeCargas.aspx", "_blank", "");
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            Session["origemRelatorio"] = "ControleCargas";

            Session["numPedido"] = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;
            Session["dataNascimento"] = Calendar1.SelectedDate;
            Session["empresaSelecionadaControleCargaPintos"] = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            Session["nomeCliente"] = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;
            Session["relatorioControleQualidadeCargasPintos"] = "ControleQualidadeCargas_Interno.rpt";

            //Response.Write("<script>window.open('RelControleQualidadeCargas.aspx')</script>");
            Redirect("RelControleQualidadeCargas.aspx", "_blank", "");
        }

        protected void imgFichaDescrLote_Click(object sender, ImageClickEventArgs e)
        {
            Session["origemRelatorio"] = "FichaDescrLote";

            Session["numPedido"] = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;
            Session["dataNascimento"] = Calendar1.SelectedDate;
            Session["empresaSelecionadaControleCargaPintos"] = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            Session["nomeCliente"] = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;

            //string codCliente = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;

            //CHICDataSet.custDataTable cDT = new CHICDataSet.custDataTable();
            //custTableAdapter cTA = new custTableAdapter();
            //cTA.FillByCustoNo(cDT, codCliente);

            //string pais = cDT[0].country.Trim();

            //Session["relatorioControleQualidadeCargasPintos"] = @"~\Reports\DocsExport\" + pais.ToUpper() + @"\CZI_" + pais.ToUpper() + ".rpt";
            //Session["relatorioControleQualidadeCargasPintos"] = "";
            Session["relatorioControleQualidadeCargasPintos"] = "FichaDescrLote.rpt";
            //Session["pais"] = pais.ToUpper();

            Redirect("RelControleQualidadeCargas.aspx", "_blank", "");
        }

        protected void imgDadosLogger_Click(object sender, ImageClickEventArgs e)
        {
            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\SAC\\Rel_Dados_Logger_"
                + Session["login"].ToString() + Session.SessionID 
                + DateTime.Now.ToShortDateString().Replace("/","-")
                + DateTime.Now.ToLongTimeString().Replace(":", "_") + ".xlsx";

            Session["destinoRelExcel"] = destino;

            string pesquisa = "*Rel_Dados_Logger_"
                + Session["login"].ToString() + Session.SessionID + "*";

            string numPedido = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;

            ApoloDataSet apoloDS = new ApoloDataSet();
            VU_USER_Veiculo_Entrega_PedidoTableAdapter vTA = new VU_USER_Veiculo_Entrega_PedidoTableAdapter();
            ApoloDataSet.VU_USER_Veiculo_Entrega_PedidoDataTable vDT = 
                new ApoloDataSet.VU_USER_Veiculo_Entrega_PedidoDataTable();
            vTA.FillByUSERPEDCHIC(vDT, numPedido);

            if (vDT.Count > 0)
            {
                ApoloDataSet.VU_USER_Veiculo_Entrega_PedidoRow vRow = vDT.FirstOrDefault();

                destino = GeraDadosLogger(vRow.PedVendaVeicPlaca, vRow.Data_Entrega, 
                    pesquisa, true, pasta, destino);

                lkbDownload.Visible = true;

                ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                scriptManager.RegisterPostBackControl(this.lkbDownload);
            }
        }

        public string GeraDadosLogger(string placaVeiculo, DateTime dataEntrega, string pesquisa, 
            bool deletaArquivoAntigo, string pasta, string destino)
        {
            #region Deleta Arquivos Antigos e faz uma cópia do mais atual

            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\SAC\\Rel_Dados_Logger"
                + ".xlsx", destino);

            #endregion

            #region Abre o EXCEL e grava o ID do Processo

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

            #endregion

            #region Carrega as Consultas

            string commandTextCHICCabecalho =
                "select " +
                    "* ";

            string commandTextCHICTabelas =
                "from " +
                    "Dados_Loggers with(Nolock) ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string commandTextCHICCondicaoParametros =
                    "Replace(PlacaVeiculo,' ','') = Replace('" + placaVeiculo + "',' ','') and " +
                    //"NumeroLogger '" + numerLogger + "' and " +
                    "DataEntrega = '" + dataEntrega.ToString("yyyy-MM-dd") + "' ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao = "";

            #endregion

            #region Atualiza as Consultas no EXCEL

            Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                if (item.Name.Equals("HLBAPP"))
                {
                    item.OLEDBConnection.BackgroundQuery = false;
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros +
                        commandTextCHICAgrupamento + commandTextCHICOrdenacao;
                }
            }

            #endregion

            #region Atualiza a Planilha e Fecha o EXCEL

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

            return destino;
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelExcel"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Dados_Logger_" +
                GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}