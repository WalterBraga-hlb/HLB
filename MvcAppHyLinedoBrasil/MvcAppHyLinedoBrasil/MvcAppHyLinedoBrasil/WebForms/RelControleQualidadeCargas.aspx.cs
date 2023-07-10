using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelControleQualidadeCargas : System.Web.UI.Page
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

        protected void Page_Init(object sender, EventArgs e)
        {
            string origem = Request.QueryString["origem"];
            string caminho = "";
            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            DateTime dataNascimento = new DateTime();
            var numPedido = Session["numPedido"];
            string origemRelatorio = "";
            string nomeCliente = "";
            string relatorioControleQualidadeCargasPintos = "";
            if (origem != "fluig")
            {
                VerificaSessao();
                dataNascimento = Convert.ToDateTime(Session["dataNascimento"]);
                numPedido = Session["numPedido"];
                origemRelatorio = Session["origemRelatorio"].ToString();
                nomeCliente = Session["nomeCliente"].ToString().Replace(" ", "_").Replace(".", "");
                relatorioControleQualidadeCargasPintos = Session["relatorioControleQualidadeCargasPintos"].ToString();
            }
            else
            {
                origemRelatorio = "ControleCargas";
                relatorioControleQualidadeCargasPintos = "ControleQualidadeCargas.rpt";
                numPedido = Request.QueryString["numPedido"];
            }

            if (origemRelatorio.Equals("ControleCargas"))
            {
                MyReport.Load(Server.MapPath(relatorioControleQualidadeCargasPintos));
                //MyReport.ParameterFields["@pDataNascimento"].CurrentValues.AddValue(dataNascimento);
                MyReport.ParameterFields["@pPedido"].CurrentValues.AddValue(numPedido);
                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
                MyReport.SetDatabaseLogon("sa", "", "srv-sql", "Apolo10");
                //MyReport.SetDatabaseLogon("sa", "", "srv-sql_4", "HLBAPP");

                CrystalReportViewer1.ReportSource = MyReport;

                CrystalReportViewer1.RefreshReport();
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, 
                    Response, false, "ControleQualidadeCargasPintos_" + numPedido);
            }
            if (origemRelatorio.Equals("FichaDescrLote"))
            {
                MyReport.Load(Server.MapPath(relatorioControleQualidadeCargasPintos));
                //MyReport.ParameterFields["@pDataNascimento"].CurrentValues.AddValue(dataNascimento);
                MyReport.ParameterFields["@pPedido"].CurrentValues.AddValue(numPedido);
                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");

                CrystalReportViewer1.ReportSource = MyReport;

                CrystalReportViewer1.RefreshReport();
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                    Response, false, "FichaDescricaoLote_" + numPedido);
            }
            else if (origemRelatorio.Equals("ExportDocs"))
            {
                string pais = Session["pais"].ToString();

                #region CZI

                string czi = @"~\Reports\DocsExport\" + pais + @"\CZI_" + pais + ".rpt";

                MyReport.Load(Server.MapPath(czi));
                MyReport.ParameterFields["OrderNo"].CurrentValues.AddValue(numPedido);
                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");

                CrystalReportViewer1.ReportSource = MyReport;

                CrystalReportViewer1.RefreshReport();
                //MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, 
                //    Response, false, "CZI_" + nomeCliente + "_" + dataNascimento.ToShortDateString());

                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\CZI_" + pais + "_" + nomeCliente + "_"
                    + dataNascimento.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + ".pdf";
                MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, 
                    caminho);

                PdfDocument pdfCZI = PdfReader.Open(caminho, PdfDocumentOpenMode.Import);

                #endregion

                #region Atestado

                string atestado = @"~\Reports\DocsExport\" + Session["pais"].ToString()
                    + @"\Atestado_" + Session["pais"] + ".rpt";

                MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                MyReport.Load(Server.MapPath(atestado));
                MyReport.ParameterFields["OrderNo"].CurrentValues.AddValue(numPedido);
                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");

                CrystalReportViewer1.ReportSource = MyReport;

                CrystalReportViewer1.RefreshReport();
                //MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                //    Response, false, "Atestado_" + nomeCliente + "_" + dataNascimento.ToShortDateString());

                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\Atestado_" + pais + "_" + nomeCliente + "_"
                    + dataNascimento.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + ".pdf";
                MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                    caminho);

                PdfDocument pdAtestado = PdfReader.Open(caminho, PdfDocumentOpenMode.Import);

                #endregion

                #region Merge PDFs

                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\DocsExport_" + pais + "_" + nomeCliente + "_"
                    + dataNascimento.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + ".pdf";

                using (PdfDocument outPdf = new PdfDocument())
                {
                    CopyPages(pdfCZI, outPdf);
                    CopyPages(pdAtestado, outPdf);

                    outPdf.Save(caminho);
                }                

                #endregion
            }

            MyReport.Close();
            MyReport.Dispose();

            CrystalReportViewer1.ReportSource = null;
            CrystalReportViewer1.Dispose(); 
            CrystalReportViewer1 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Response.ContentType = "Application/pdf";
            Response.TransmitFile(caminho);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();
        }

        void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}