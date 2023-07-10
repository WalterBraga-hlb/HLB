using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHylinedoBrasilMobile.Models;
using MvcAppHylinedoBrasilMobile.Models.bdApolo;
using MvcAppHylinedoBrasilMobile.Models.bdApolo2;
using MvcAppHylinedoBrasilMobile.Models.CHICMobileDataSetTableAdapters;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Net;
using System.IO;

namespace MvcAppHylinedoBrasilMobile.Reports
{
    public partial class CrystalReportViewer : System.Web.UI.Page
    {
        #region DataBase Entities

        public static HLBAPPEntities hlbappStatic = new HLBAPPEntities();
        public static bdApoloEntities apoloStatic = new bdApoloEntities();
        public static Apolo10Entities apolo2Static = new Apolo10Entities();

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            VerificaSessao();

            try
            {
                int id = Convert.ToInt32(Session["idSelecionado"].ToString());

                Pedido_Venda pedidoVenda = hlbappStatic.Pedido_Venda.Where(w => w.ID == id).FirstOrDefault();

                if (pedidoVenda.DataSaidaIncubatorio != null)
                {
                    bookedNavTableAdapter bNavTA = new bookedNavTableAdapter();
                    CHICMobileDataSet.bookedNavDataTable bDT = new CHICMobileDataSet.bookedNavDataTable();
                    DateTime calDate = Convert.ToDateTime(pedidoVenda.DataSaidaIncubatorio).AddDays(-21);
                    bNavTA.FillByCalDateAndCustNo(bDT, calDate, pedidoVenda.CodigoCliente);
                    string numPedido = "";
                    if (bDT.Count > 0)
                        numPedido = bDT[0].orderno.Trim();
                    else
                    {
                        Item_Pedido_Venda ipv = hlbappStatic.Item_Pedido_Venda
                            .Where(w => w.IDPedidoVenda == pedidoVenda.ID).FirstOrDefault();

                        numPedido = ipv.OrderNoCHIC;
                    }

                    List<ENT_OBJ> listDocExport = apolo2Static.ENT_OBJ
                        .Where(w => w.EntCod == pedidoVenda.CodigoCliente)
                        .OrderBy(o => o.EntObjSeq).ToList();

                    ENTIDADE entidade = apoloStatic.ENTIDADE.Where(w => w.EntCod == pedidoVenda.CodigoCliente).FirstOrDefault();
                    MvcAppHylinedoBrasilMobile.Models.bdApolo.CIDADE cidade = 
                        apoloStatic.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                    MvcAppHylinedoBrasilMobile.Models.bdApolo2.PAIS pais = apolo2Static.PAIS.Where(w => w.PaisSigla == cidade.PaisSigla).FirstOrDefault();
                    string relPath = @"~\Reports\DocsExport\";
                    PdfDocument outPdf = new PdfDocument();
                    string nomeCliente = pedidoVenda.NomeCliente.Replace(" ", "_").Replace(".", "").Replace("/", "");

                    #region Gerar Check List

                    CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport =
                                new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                    //MyReport.Load(Server.MapPath(@"~\Reports\DocsExport\CHECK_LIST_DOCS.rpt"));
                    //MyReport.ParameterFields["EntCod"].CurrentValues.AddValue(pedidoVenda.CodigoCliente);
                    string caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\CHECK_LIST_DOCS"
                                + "_" + nomeCliente + "_"
                                + calDate.AddDays(21).ToShortDateString().Replace("/", "-") + "_"
                                + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                                + DateTime.Now.ToLongTimeString().Replace(":", "-") + ".pdf";

                    //CrystalReportViewer1.ReportSource = MyReport;
                    //CrystalReportViewer1.RefreshReport();
                    //MyReport.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                    //    caminho);

                    //PdfDocument pdf = PdfReader.Open(caminho, PdfDocumentOpenMode.Import);
                    PdfDocument pdf = null;

                    //CopyPages(pdf, outPdf);
                    //System.IO.File.Delete(caminho);

                    #endregion

                    List<String> listaDocExportFilter = (List<string>)Session["listaDocExportSelectFilter"];

                    string tipoExportacaoRelatorioSelecionado =
                                Session["tipoExportacaoRelatorioSelecionado"].ToString();

                    string typeString = "";
                    if (tipoExportacaoRelatorioSelecionado.Equals("PDF"))
                        typeString = ".pdf";
                    else if (tipoExportacaoRelatorioSelecionado.Equals("Excel"))
                        typeString = ".xls";
                    else if (tipoExportacaoRelatorioSelecionado.Equals("Word"))
                        typeString = ".doc";

                    foreach (var docExport in listaDocExportFilter)
                    {
                        caminho = "";

                        OBJETO objeto = apolo2Static.OBJETO.Where(w => w.ObjCodEstr == docExport).FirstOrDefault();

                        MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();

                        string relPathFull = relPath;

                        if (objeto.USERTipoDocumento.Equals("País"))
                        {
                            string nomeRelatorio = objeto.ObjNome.Replace("_MAPA", "").Replace("_INCUBATORIO", "");

                            relPathFull = relPathFull + pais.PaisNome + @"\" + nomeRelatorio + "_" + pais.PaisNome + ".rpt";

                            if (File.Exists(Server.MapPath(relPathFull)))
                            {
                                MyReport.Load(Server.MapPath(relPathFull));
                                //MyReport.ParameterFields["OrderNo"].CurrentValues.AddValue(numPedido);
                                MyReport.ParameterFields["ID"].CurrentValues.AddValue(id);
                                if (objeto.ObjNome.Contains("MAPA"))
                                    MyReport.ParameterFields["Tipo_Atestado"].CurrentValues.AddValue("MAPA");
                                else if (objeto.ObjNome.Contains("INCUBATORIO"))
                                    MyReport.ParameterFields["Tipo_Atestado"].CurrentValues.AddValue("INCUBATÓRIO");
                                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
                                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\" + objeto.ObjNome + "_"
                                    + pais.PaisNome + "_" + nomeCliente + "_"
                                    + calDate.AddDays(21).ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + typeString;
                            }
                        }
                        else if (objeto.USERTipoDocumento.Equals("Proforma"))
                        {
                            relPathFull = relPathFull + @"\" + objeto.USERTipoDocumento.ToUpper() + ".rpt";

                            if (File.Exists(Server.MapPath(relPathFull)))
                            {
                                MyReport.Load(Server.MapPath(relPathFull));
                                MyReport.ParameterFields["ID"].CurrentValues.AddValue(id);
                                MyReport.ParameterFields["TipoRelatorio"].CurrentValues.AddValue(objeto.ObjNome);
                                //MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
                                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\" + objeto.ObjNome + "_"
                                    + pais.PaisNome + "_" + nomeCliente + "_"
                                    + calDate.AddDays(21).ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + typeString;
                            }
                        }
                        else if (objeto.USERTipoDocumento.Equals("Normal"))
                        {
                            relPathFull = relPathFull + @"\" + objeto.ObjNome + ".rpt";

                            if (File.Exists(Server.MapPath(relPathFull)))
                            {
                                MyReport.Load(Server.MapPath(relPathFull));
                                if (MyReport.ParameterFields.Count > 0)
                                {
                                    MyReport.ParameterFields["ID"].CurrentValues.AddValue(id);
                                }
                                MyReport.SetDatabaseLogon("na", "brnaps", "brflocks", "");
                                caminho = @"C:\inetpub\wwwroot\Relatorios\DocsExport\" + objeto.ObjNome + "_"
                                    + pais.PaisNome + "_" + nomeCliente + "_"
                                    + calDate.AddDays(21).ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                                    + DateTime.Now.ToLongTimeString().Replace(":", "-") + typeString;
                            }
                        }

                        if (caminho != "")
                        {
                            CrystalDecisions.Shared.ExportFormatType type =
                                CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                            if (tipoExportacaoRelatorioSelecionado.Equals("PDF"))
                                type = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat;
                            else if (tipoExportacaoRelatorioSelecionado.Equals("Excel"))
                                type = CrystalDecisions.Shared.ExportFormatType.Excel;
                            else if (tipoExportacaoRelatorioSelecionado.Equals("Word"))
                                type = CrystalDecisions.Shared.ExportFormatType.WordForWindows;

                            CrystalReportViewer1.ReportSource = MyReport;
                            CrystalReportViewer1.RefreshReport();
                            MyReport.ExportToDisk(type, caminho);

                            if (tipoExportacaoRelatorioSelecionado.Equals("PDF"))
                            {
                                int qtdCopias = 1;
                                if (objeto.USERDEQtdeCopias != null)
                                    qtdCopias = Convert.ToInt32(objeto.USERDEQtdeCopias);

                                pdf = PdfReader.Open(caminho, PdfDocumentOpenMode.Import);

                                for (int i = 0; i < qtdCopias; i++)
                                {
                                    CopyPages(pdf, outPdf);
                                }

                                System.IO.File.Delete(caminho);
                            }
                        }

                        MyReport.Close();
                        MyReport.Dispose();
                    }

                    string caminhoFinal = "";

                    if (tipoExportacaoRelatorioSelecionado.Equals("PDF"))
                    {
                        caminhoFinal = @"C:\inetpub\wwwroot\Relatorios\DocsExport\DocsExport_"
                            + nomeCliente + "_"
                            + calDate.AddDays(21).ToShortDateString().Replace("/", "-") + "_"
                            + DateTime.Now.ToShortDateString().Replace("/", "-") + "_"
                            + DateTime.Now.ToLongTimeString().Replace(":", "-") + ".pdf";

                        if (outPdf.PageCount > 0)
                        {
                            outPdf.Save(caminhoFinal);
                            Session["ErroCrystalReportViewer"] = null;
                        }
                        else
                        {
                            Session["ErroCrystalReportViewer"] = "Nenhum relatório foi gerado! "
                                + "Verifique se os relatórios selecionados existem para o país do cliente!";
                            //Response.Redirect("~/PedidoVenda/PrintDocExportationReturn");
                        }

                        Session["DocExportationPath"] = caminhoFinal;
                    }
                    else
                    {
                        Session["DocExportationPath"] = caminho;
                    }

                    CrystalReportViewer1.ReportSource = null;
                    CrystalReportViewer1.Dispose();
                    CrystalReportViewer1 = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    //Response.ContentType = "Application/pdf";
                    //Response.TransmitFile(caminhoFinal);

                    string CurrentURL = "http://" + Request.Url.Authority;

                    //Response.Redirect(CurrentURL + "/PedidoVenda/DownloadDocExportation",
                    //    "_blank", "menubar=0,width=100%,height=100%");

                    //Response.Redirect(CurrentURL + "/PedidoVenda/FinishPrintDocExportation",
                    //    "_self", "");

                    Response.Redirect("~/PedidoVenda/FinishPrintDocExportation", false);
                }
                else
                {
                    Response.Redirect("~/PedidoVenda/Index", false);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    Session["ErroCrystalReportViewer"] = "Erro ao Gerar Relatório: " + ex.Message;
                else
                    Session["ErroCrystalReportViewer"] = "Erro ao Gerar Relatório: " + ex.Message
                        + " / Erro Interno: " + ex.InnerException.Message;
                Response.Redirect("~/PedidoVenda/PrintDocExportationReturn");
            }
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

        public void VerificaSessao()
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("http://m.hlbapp.hyline.com.br");
            }
            else
            {
                if (Session["usuario"].ToString() == "0")
                {
                    Response.Redirect("http://m.hlbapp.hyline.com.br");
                }
            }
        }
    }

    public static class ResponseHelper
    {
        public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        {
            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase))
                && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;
                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);
                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }
    }
}



