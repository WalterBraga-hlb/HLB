using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OracleClient;
using MvcAppHyLinedoBrasil.Data;
using System.Configuration;
using CrystalDecisions.Shared;
using System.Globalization;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class MapaIncubacao : System.Web.UI.Page
    {
        //public static string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

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
            CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            MyReport.Load(Server.MapPath("MapaIncubacaoCrystalReport.rpt"));

            var local = Session["hatchLocal"].ToString();
            var setDate = Convert.ToDateTime(Session["setDate"].ToString()).ToString(CultureInfo.GetCultureInfo("pt-BR"));

            MyReport.ParameterFields["@pLocal"].CurrentValues.AddValue(local);
            MyReport.ParameterFields["@pSetDate"].CurrentValues.AddValue(setDate);

            CrystalReportViewer1.ReportSource = MyReport;

            DateTime setDate2 = Convert.ToDateTime(Session["setDate"].ToString());

            // Exportar para Excel como download
            if (Session["tipoRelatorio"].ToString().Equals("Excel"))
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.Excel, Response, false, "MapaIncubacao " + setDate2.ToString("yyyyMMdd"));
            else
                MyReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, Response, false, "MapaIncubacao " + setDate2.ToString("yyyyMMdd"));

            MyReport.Dispose();
            CrystalReportViewer1.Dispose();

            MyReport.Close();
            MyReport.Dispose();

            CrystalReportViewer1.ReportSource = null;
            CrystalReportViewer1.Dispose();
            CrystalReportViewer1 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

            if (IsPostBack == false)
            {
                //EggInvData conexao = new EggInvData(connStr);

                //conexao.ExecutaProcedureMapaIncubacao(Session["hatchLocal"].ToString(), Convert.ToDateTime(Session["setDate"]));
                //conexao.ExecutaProcedureMapaIncubacao("CH", Convert.ToDateTime("11/06/2013"));

                //Session["hatchLocal"] = "CH";
                //Session["setDate"] = "26/06/2013";

                //Label1.Text = Session["hatchLocal"].ToString();
                //Label2.Text = Session["setDate"].ToString();
            }

            //CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //MyReport.Load(Server.MapPath("MapaIncubacaoCrystalReport.rpt"));

            //var local = Session["hatchLocal"];
            //var setDate = Session["setDate"];

            //MyReport.ParameterFields["@pLocal"].CurrentValues.AddValue(local);
            //MyReport.ParameterFields["@pSetDate"].CurrentValues.AddValue(setDate);

            //CrystalReportViewer1.ReportSource = MyReport;
        }

        protected void MapaIncubacaoSource_Load(object sender, EventArgs e)
        {
            //var local = Session["hatchLocal"];
            //var setDate = Session["setDate"];

            //CrystalDecisions.CrystalReports.Engine.ReportDocument MyReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            //MyReport.Load(Server.MapPath("MapaIncubacaoCrystalReport.rpt"));

            ////CrystalDecisions.Shared.ParameterField pLocal = MyReport.ParameterFields["@pLocal"];
            ////pLocal.CurrentValues.Clear();
            ////pLocal.CurrentValues.AddValue(local);

            ////CrystalDecisions.Shared.ParameterField pSetDate = MyReport.ParameterFields["@pSetDate"];
            ////pSetDate.CurrentValues.Clear();
            ////pSetDate.CurrentValues.AddValue(setDate);

            //MyReport.ParameterFields["@pLocal"].CurrentValues.Clear();
            //MyReport.ParameterFields["@pLocal"].CurrentValues.AddValue(local);
            //MyReport.ParameterFields["@pSetDate"].CurrentValues.Clear();
            //MyReport.ParameterFields["@pSetDate"].CurrentValues.AddValue(setDate);
            //MyReport.SetDataSource(SqlDataSource1);
            ////MyReport.Refresh();

            //CrystalReportViewer1.ReportSource = MyReport;

            ////CrystalReportViewer1.RefreshReport();
        }

    }
}