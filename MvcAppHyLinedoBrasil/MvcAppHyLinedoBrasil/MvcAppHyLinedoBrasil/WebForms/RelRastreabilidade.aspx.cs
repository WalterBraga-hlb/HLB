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
    public partial class RelRastreabilidade : System.Web.UI.Page
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

        public string GeraRelatorioRastreabilidade(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            string empresasAcesso = "";
            for (int i = 0; i < Session["empresa"].ToString().Length; i = i + 2)
            {
                empresasAcesso = empresasAcesso + Session["empresa"].ToString().Substring(i, 2);
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Rel_Rastreabilidade.xlsx", destino);

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
                "select " +
                        @"v.""Data de Nascimento"", " +
                        @"v.""Lote"", " +
                        @"v.""Linhagem"", " +
                        @"v.""Cliente"", " +
                        @"v.""UF"", " +
                        @"v.""Qtde.Lote"", " +
                        @"v.""Idade"", " +
                        @"v.""Classificação Idade"", " +
                        @"v.""Peso Pintinho"", " +
                        @"v.""Uniformidade"", " +
                        @"v.""Erros de Contagem"", " +
                        @"v.""Erros de Sexagem"", " +
                        @"v.""Tipo de Caixa"", " +
                        @"v.""Pintos / Caixa"", " +
                        @"v.""Tempo Permanência Máximo"", " +
                        @"v.""Tempo Permanência Mínimo"", " +
                        @"v.""Pedido"", " +
                        @"v.Hatch_Loc ""Incubatório"", " +
                        @"v.""Empresa"", " +
                        @"v.""Estoque Médio Ovos"" ";

            string commandTextCHICTabelas =
                "from " +
                    "vu_rel_controle_carga_pintos v ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataInicialStr = dataInicial.ToString("dd/MM/yyyy");
            string dataFinalStr = dataFinal.ToString("dd/MM/yyyy");

            string commandTextCHICCondicaoParametros =
                    @"v.""Data de Nascimento"" between TO_DATE('" +
                    dataInicialStr + "','dd/MM/yyyy HH24:MI:SS') and TO_DATE('" +
                    dataFinalStr + "','dd/MM/yyyy HH24:MI:SS') and " +
                    "InStr('" + empresasAcesso + @"',v.""Empresa"") > 0 ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "1,2";

            #endregion

            #region DadosEntrega

            string commandTextCHICCabecalhoDE =
                "select " +
                    "V.USERPEDCHIC, " +
                    "V.USERRETempoViagem [Tempo de Viagem], " +
                    "V.PedVendaVeicPlaca [Placa], " +
                    "V.EquipVeicTipo [Tipo Baú], " +
                    "V.Data_Entrega ";


            string commandTextCHICTabelasDE =
                "from " +
                    "VU_USER_Veiculo_Entrega_Pedido V ";

            string commandTextCHICCondicaoJoinsDE =
                "where ";

            string commandTextCHICCondicaoFiltrosDE = "V.USERPEDCHIC is not null and ";

            string dataInicialStrDE = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrDE = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosDE =
                    "V.Data_Entrega between '" +
                    dataInicialStrDE + "' and '" + dataFinalStrDE + "' ";

            string commandTextCHICAgrupamentoDE = "";

            string commandTextCHICOrdenacaoDE = "";

            #endregion

            #region WEB

            string commandTextCHICCabecalhoWEB =
                "select " +
                    "DataEntrega, " +
	                "PlacaVeiculo, " +
	                "NumeroLogger, " +
	                "AVG(Temperatura) TempMedia, " +
	                "AVG(Umidade) UmidadeMedia ";

            string commandTextCHICTabelasWEB =
                "from " +
                    "Dados_Loggers ";

            string commandTextCHICCondicaoJoinsWEB =
                "where ";

            string commandTextCHICCondicaoFiltrosWEB = "";

            string dataInicialStrWEB = dataInicial.ToString("yyyy-MM-dd");
            string dataFinalStrWEB = dataFinal.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametrosWEB =
                    "DataEntrega between '" +
                    dataInicialStrWEB + "' and '" + dataFinalStrWEB + "' ";

            string commandTextCHICAgrupamentoWEB = 
                "group by " +
                    "DataEntrega, " +
                    "PlacaVeiculo, " +
                    "NumeroLogger";

            string commandTextCHICOrdenacaoWEB = "";

            #endregion

            #region Dados Lotes WEB

            string commandTextCHICCabecalhoLotesWEB =
                "select " +
                    "* ";

            string commandTextCHICTabelasLotesWEB =
                "from " +
                    "VU_Rastreabilidade_Lote ";

            string commandTextCHICCondicaoJoinsLotesWEB =
                "where ";

            string commandTextCHICCondicaoFiltrosLotesWEB = "";

            string commandTextCHICCondicaoParametrosLotesWEB =
                    "Hatch_date between '" +
                    dataInicialStrWEB + "' and '" + dataFinalStrWEB + "' ";

            string commandTextCHICAgrupamentoLotesWEB = "";

            string commandTextCHICOrdenacaoLotesWEB = "";

            #endregion

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("brflocks"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + 
                        commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;

                if (item.Name.Equals("Dados_Entrega"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoDE + commandTextCHICTabelasDE + commandTextCHICCondicaoJoinsDE +
                        commandTextCHICCondicaoFiltrosDE + commandTextCHICCondicaoParametrosDE +
                        commandTextCHICAgrupamentoDE +
                        commandTextCHICOrdenacaoDE;

                if (item.Name.Equals("HLBAPP"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoWEB + commandTextCHICTabelasWEB + commandTextCHICCondicaoJoinsWEB +
                        commandTextCHICCondicaoFiltrosWEB + commandTextCHICCondicaoParametrosWEB +
                        commandTextCHICAgrupamentoWEB +
                        commandTextCHICOrdenacaoWEB;

                if (item.Name.Equals("Dados_Lotes_Web"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalhoLotesWEB + commandTextCHICTabelasLotesWEB + commandTextCHICCondicaoJoinsLotesWEB +
                        commandTextCHICCondicaoFiltrosLotesWEB + commandTextCHICCondicaoParametrosLotesWEB +
                        commandTextCHICAgrupamentoLotesWEB +
                        commandTextCHICOrdenacaoLotesWEB;
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
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Rel_Rastreabilidade_" 
                + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Rel_Rastreabilidade_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            Session["destinoRelNascEmbrio"] = GeraRelatorioRastreabilidade(pesquisa, true, pasta, destino, 
                calDataInicial.SelectedDate, calDataFinal.SelectedDate);

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelNascEmbrio"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; " +
                "filename=Rel_Rastreabilidade_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }
    }
}