using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
using MvcAppHyLinedoBrasil.EntityWebForms.PAT_BEM;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data.CHICOracleDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Models.Apolo;
using System.Data.Objects;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class Rastreabilidade : System.Web.UI.Page
    {
        #region Objetcs

        FLIPDataSet flip = new FLIPDataSet();
        //HLBAPPEntities hlbapp = new HLBAPPEntities();

        #endregion

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

            if (txtLocalizarPedido.Text.Equals(""))
                txtLocalizarPedido.Text = "0";

            if (txtLocalizarLotes.Text.Equals(""))
                txtLocalizarLotes.Text = "0";

            if (txtPesquisaLotesRelacionados.Text.Equals(""))
                txtPesquisaLotesRelacionados.Text = "0";

            if (ddlIncubatorios.SelectedValue.Equals("PH"))
            {
                Session["siteKey"] = "HYBRBRPG";
                Session["location"] = "PG";
            }
            else
            {
                Session["siteKey"] = "HYBRBRBR";
                Session["location"] = "PP";
            }

            if (IsPostBack == false)
            {
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                DeselecionarPedido();
                Session["numPedidoSelecionado"] = "";
                //Session["siteKey"] = "";

                Calendar1.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                #region Carrega Incubatorios

                ddlIncubatorios.Items.Clear();

                FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
                HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();

                hTA.Fill(hDT);

                foreach (var item in hDT)
                {
                    if (MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        string hatchLoc = item.HATCH_LOC;
                        //if (item.HATCH_LOC.Equals("TB")) hatchLoc = "AJ";
                        ddlIncubatorios.Items.Add(new ListItem { Text = item.HATCH_DESC, Value = hatchLoc, Selected = false });
                    }
                }

                #endregion

                AtualizaFLIP();

                //RefreshFLIPManual();

                gdvPedidos.DataBind();
            }
        }

        protected void ddlIncubatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizaFLIP();
            gdvLotes.DataBind();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            AtualizaFLIP();
            gdvLotes.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            gdvPedidos.DataBind();
            gdvLotes.DataBind();
        }

        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string codigoCliente = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[2].Text;
            string nomeCliente = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[3].Text;
            string uf = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[4].Text;
            string numPedido = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[5].Text;
            string linhagem = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[6].Text.Replace("amp;","");
            string qtdePedido = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[7].Text;
            string carga = gdvPedidos.Rows[gdvPedidos.SelectedIndex].Cells[8].Text;
            Session["numPedidoSelecionado"] = numPedido;
            Session["linhagemSelecionada"] = linhagem;

            SqlDataSource2.SelectParameters["NumPedido"].DefaultValue = numPedido;
            SqlDataSource2.SelectParameters["Linhagem"].DefaultValue = linhagem;

            lblPedidoSelecionado.Text = numPedido + " - " + nomeCliente + "/" + uf
                + " - " + linhagem + " - Qtde. " + qtdePedido + " - Carga: " + carga;

            SelecionarPedido();

            #region Carrega Variáveis

            string incubatorio = ddlIncubatorios.SelectedValue;
            DateTime setDate = Calendar1.SelectedDate;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HATCHERY_ORDER_DATA pedido = hlbapp.HATCHERY_ORDER_DATA
                .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == setDate
                    && w.OrderNoCHIC == numPedido
                    && (w.Variety == linhagem || w.Variety == null)).FirstOrDefault();

            if (pedido != null)
            {
                decimal pesoMedio = Convert.ToDecimal(pedido.PesoMedio);
                decimal uniformidade = Convert.ToDecimal(pedido.UniformidadePerc);
                DateTime? dataSaidaProgramada = null;
                if (pedido.SaidaProgramada != null) dataSaidaProgramada = pedido.SaidaProgramada;
                DateTime? dataSaidaReal = null;
                if (pedido.SaidaReal != null) dataSaidaReal = pedido.SaidaReal;
                int qtdeAmostra = Convert.ToInt32(pedido.QtdeAmostra);
                int qtdeVacinada = Convert.ToInt32(pedido.QtdeVacinada);
                int qtdePontoFioPretoUmbigo = Convert.ToInt32(pedido.QtdePontoFioPretoUmbigo);
                int qtdePesTortosDedosCurvos = Convert.ToInt32(pedido.QtdePesTortosDedosCurvos);
                int qtdeErroContagem = Convert.ToInt32(pedido.QtdeErroContagem);
                int qtdeErroSexagem = Convert.ToInt32(pedido.QtdeErroSexagem);
                int qtdeErroSelecao = Convert.ToInt32(pedido.QtdeErroSelecao);

                string observacoes = pedido.Observacao;
                string repInspFinal = pedido.RespInspecaoFinal;
                string repExpedicaoCarga = pedido.RespExpedicaoCarga;
                string respLiberacaoVeiculo = pedido.RespLiberacaoVeiculo;
                string rnc = pedido.RNC;
                string disposicaoRNC = pedido.DisposicaoRNC;
                string numeroRNC = pedido.NumeroRNC;

                //txtPesoMedio.Text = pedido.PesoMedio.ToString();
                //txtPercUniformidade.Text = pedido.UniformidadePerc.ToString();
                if (pedido.SaidaProgramada != null)
                {
                    txtDataSaidaProgramada.Text = Convert.ToDateTime(pedido.SaidaProgramada).ToString("dd/MM/yyyy");                    
                    txtHoraSaidaProgramada.Text = Convert.ToDateTime(pedido.SaidaProgramada).ToString("HH:mm");
                }
                if (pedido.SaidaReal != null)
                {
                    txtDataSaidaReal.Text = Convert.ToDateTime(pedido.SaidaReal).ToString("dd/MM/yyyy");
                    txtHoraSaidaReal.Text = Convert.ToDateTime(pedido.SaidaReal).ToString("HH:mm");
                }
                txtQtdeAmostra.Text = pedido.QtdeAmostra.ToString();
                txtQtdeVacinada.Text = pedido.QtdeVacinada.ToString();
                txtQtdePontoFioPretoUmbigo.Text = pedido.QtdePontoFioPretoUmbigo.ToString();
                txtQtdePesTortosDedosCurvos.Text = pedido.QtdePesTortosDedosCurvos.ToString();
                txtQtdeErroContagem.Text = pedido.QtdeErroContagem.ToString();
                txtQtdeErroSexagem.Text = pedido.QtdeErroSexagem.ToString();
                txtQtdeErroSelecao.Text = pedido.QtdeErroSelecao.ToString();

                txtObservacao.Text = pedido.Observacao;
                txtRepInspFinal.Text = pedido.RespInspecaoFinal;
                txtRepExpedicaoCarga.Text = pedido.RespExpedicaoCarga;
                txtRespLiberacaoVeiculo.Text = pedido.RespLiberacaoVeiculo;
                ddlRNC.SelectedValue = pedido.RNC;
                ddlDisposicaoRNC.SelectedValue = pedido.DisposicaoRNC;
                if (numeroRNC != "")
                {
                    lblNumeroRNC.Visible = true;
                    txtNumeroRNC.Visible = true;
                    txtNumeroRNC.Text = numeroRNC;
                    if (Calendar1.SelectedDate >= Convert.ToDateTime("01/01/2017"))
                        txtNumeroRNC.Enabled = false;
                    else
                        txtNumeroRNC.Enabled = true;
                }
                else
                {
                    lblNumeroRNC.Visible = false;
                    txtNumeroRNC.Visible = false;
                    txtNumeroRNC.Text = "";
                }
            }

            #endregion
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            DeselecionarPedido();
        }

        public void SelecionarPedido()
        {
            Calendar1.Enabled = false;
            ddlIncubatorios.Enabled = false;

            lblTituloPedidos.Visible = false;
            lblLocalizarPedido.Visible = false;
            txtLocalizarPedido.Visible = false;
            ddlCampoPedido.Visible = false;
            btnPesquisarPedido.Visible = false;
            gdvPedidos.Visible = false;

            lblPintosNascidos.Visible = true;
            lblLocalizarLotes.Visible = true;
            txtLocalizarLotes.Visible = true;
            ddlCamposLotes.Visible = true;
            ddlClassOvos.Visible = true;
            gdvLotes.Visible = true;
            btnPesquisarLotes.Visible = true;

            //if (txtLocalizarLotes.Text.Equals(""))
            //    txtLocalizarLotes.Text = "0";

            lblPedidoSelecionado.Visible = true;
            btnCancelar.Visible = true;
            btnSalvar.Visible = true;

            //lblPesoMedio.Visible = true;
            //txtPesoMedio.Visible = true;
            //txtPesoMedio.Text = "";
            //lblUniformidadePerc.Visible = true;
            //txtPercUniformidade.Visible = true;
            //txtPercUniformidade.Text = "";
            lblSaidaProgramada.Visible = true;
            txtDataSaidaProgramada.Visible = true;
            txtDataSaidaProgramada.Text = "";
            txtHoraSaidaProgramada.Visible = true;
            txtHoraSaidaProgramada.Text = "";
            lblSaidaReal.Visible = true;
            txtDataSaidaReal.Visible = true;
            txtDataSaidaReal.Text = "";
            txtHoraSaidaReal.Visible = true;
            txtHoraSaidaReal.Text = "";
            lblQtdeAmostra.Visible = true;
            txtQtdeAmostra.Visible = true;
            txtQtdeAmostra.Text = "";
            lblQtdeVacinada.Visible = true;
            txtQtdeVacinada.Visible = true;
            txtQtdeVacinada.Text = "";
            lblQtdePontoFioPretoUmbigo.Visible = true;
            txtQtdePontoFioPretoUmbigo.Visible = true;
            txtQtdePontoFioPretoUmbigo.Text = "";
            lblQtdePesTortosDedosCurvos.Visible = true;
            txtQtdePesTortosDedosCurvos.Visible = true;
            txtQtdePesTortosDedosCurvos.Text = "";
            lblQtdeErroContagem.Visible = true;
            txtQtdeErroContagem.Visible = true;
            txtQtdeErroContagem.Text = "";
            lblQtdeErroSexagem.Visible = true;
            txtQtdeErroSexagem.Visible = true;
            txtQtdeErroSexagem.Text = "";
            lblQtdeErroSelecao.Visible = true;
            txtQtdeErroSelecao.Visible = true;
            txtQtdeErroSelecao.Text = "";

            lblLotesRelacionados.Visible = true;
            lblLocalizarLotesRelacionados.Visible = true;
            txtPesquisaLotesRelacionados.Visible = true;
            ddlCamposLotesRelacionados.Visible = true;
            ddlTipoOvoLotesRelacionados.Visible = true;
            btnPesquisarLotesRelacionados.Visible = true;
            gdvLotesSelecionados.Visible = true;

            pnlInformacoesAdicionais.Visible = true;
            pnlVacinas.Visible = true;
            pnlRotulos.Visible = true;
        }

        public void DeselecionarPedido()
        {
            Calendar1.Enabled = true;
            ddlIncubatorios.Enabled = true;

            lblTituloPedidos.Visible = true;
            lblLocalizarPedido.Visible = true;
            txtLocalizarPedido.Visible = true;
            txtLocalizarPedido.Text = "0";
            ddlCampoPedido.Visible = true;
            btnPesquisarPedido.Visible = true;
            gdvPedidos.Visible = true;
            gdvPedidos.SelectedIndex = -1;

            lblPintosNascidos.Visible = false;
            lblLocalizarLotes.Visible = false;
            txtLocalizarLotes.Visible = false;
            ddlCamposLotes.Visible = false;
            ddlClassOvos.Visible = false;
            gdvLotes.SelectedIndex = -1;
            gdvLotes.Visible = false;
            HatchFormDataSource.SelectParameters["FlockId"].DefaultValue = "";
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = "";
            frvLoteSelecionado.ChangeMode(FormViewMode.ReadOnly);
            frvLoteSelecionado.Visible = false;
            btnPesquisarLotes.Visible = false;

            lblPedidoSelecionado.Text = "";
            lblPedidoSelecionado.Visible = false;
            btnCancelar.Visible = false;
            btnSalvar.Visible = false;

            //lblPesoMedio.Visible = false;
            //txtPesoMedio.Visible = false;
            //lblUniformidadePerc.Visible = false;
            //txtPercUniformidade.Visible = false;
            lblSaidaProgramada.Visible = false;
            txtDataSaidaProgramada.Visible = false;
            txtHoraSaidaProgramada.Visible = false;
            lblSaidaReal.Visible = false;
            txtDataSaidaReal.Visible = false;
            txtHoraSaidaReal.Visible = false;
            lblQtdeAmostra.Visible = false;
            txtQtdeAmostra.Visible = false;
            lblQtdeVacinada.Visible = false;
            txtQtdeVacinada.Visible = false;
            lblQtdePontoFioPretoUmbigo.Visible = false;
            txtQtdePontoFioPretoUmbigo.Visible = false;
            lblQtdePesTortosDedosCurvos.Visible = false;
            txtQtdePesTortosDedosCurvos.Visible = false;
            lblQtdeErroContagem.Visible = false;
            txtQtdeErroContagem.Visible = false;
            lblQtdeErroSexagem.Visible = false;
            txtQtdeErroSexagem.Visible = false;
            lblQtdeErroSelecao.Visible = false;
            txtQtdeErroSelecao.Visible = false;

            lblLotesRelacionados.Visible = false;
            lblLocalizarLotesRelacionados.Visible = false;
            txtPesquisaLotesRelacionados.Visible = false;
            ddlCamposLotesRelacionados.Visible = false;
            ddlTipoOvoLotesRelacionados.Visible = false;
            btnPesquisarLotesRelacionados.Visible = false;
            gdvLotesSelecionados.EditIndex = -1;
            gdvLotesSelecionados.DataBind();
            gdvLotesSelecionados.Visible = false;

            pnlInformacoesAdicionais.Visible = false;
            pnlVacinas.Visible = false;
            pnlRotulos.Visible = false;

            lblNumeroRNC.Visible = false;
            txtNumeroRNC.Visible = false;
            txtNumeroRNC.Text = "";
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {

        }

        protected void FormView1_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {

        }

        protected void FormView1_DataBound(object sender, EventArgs e)
        {

        }

        protected void btnPesquisarLotes_Click(object sender, EventArgs e)
        {
            gdvPedidos.DataBind();
            gdvLotes.DataBind();
        }

        protected void btnPesquisarLotesRelacionados_Click(object sender, EventArgs e)
        {
            gdvLotesSelecionados.DataBind();
        }

        protected void gdvLotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            frvLoteSelecionado.Visible = true;

            string loteCompleto = gdvLotes.Rows[gdvLotes.SelectedIndex].Cells[2].Text;
            string classOvo = gdvLotes.Rows[gdvLotes.SelectedIndex].Cells[5].Text;
            DateTime setDate = Convert.ToDateTime(gdvLotes.Rows[gdvLotes.SelectedIndex].Cells[1].Text);
            Session["qtdPintosVendaveisLote"] = gdvLotes.Rows[gdvLotes.SelectedIndex].Cells[6].Text.Replace(".","");

            HatchFormDataSource.SelectParameters["FlockId"].DefaultValue = loteCompleto;
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = classOvo;
            HatchFormDataSource.SelectParameters["SetDate"].DefaultValue = setDate.ToString("yyyy-MM-dd");

            lblMensagem.Visible = false;
            frvLoteSelecionado.ChangeMode(FormViewMode.Edit);
        }

        protected void UpdateButton_Click1(object sender, EventArgs e)
        {
            try
            {
                if (frvLoteSelecionado.CurrentMode == FormViewMode.Edit)
                {
                    #region Carrega variáveis

                    //DateTime dataIncubacao = Calendar1.SelectedDate;
                    string incubatorio = ddlIncubatorios.SelectedValue;
                    string numPedidoSelecionado = Session["numPedidoSelecionado"].ToString();

                    System.Web.UI.WebControls.Label lblDataIncubacao =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblDataIncubacao");
                    DateTime dataIncubacao = Convert.ToDateTime(lblDataIncubacao.Text);
                    System.Web.UI.WebControls.Label lblLoteCompleto =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblLoteCompleto");
                    string loteCompleto = lblLoteCompleto.Text;
                    System.Web.UI.WebControls.Label lblLinhagem =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblLinhagem");
                    string linhagemLote = lblLinhagem.Text;
                    System.Web.UI.WebControls.Label lblClassOvo =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblClassOvo");
                    string classOvo = lblClassOvo.Text;
                    System.Web.UI.WebControls.TextBox txtQtde =
                        (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtQtde");
                    System.Web.UI.WebControls.Label lblErroFrmLoteSelecionado =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblErroFrmLoteSelecionado");
                    int qtde = Convert.ToInt32(txtQtde.Text);
                    System.Web.UI.WebControls.DropDownList ddlRotulo =
                        (System.Web.UI.WebControls.DropDownList)frvLoteSelecionado.FindControl("ddlRotulo");
                    string rotulo = ddlRotulo.Text;

                    System.Web.UI.WebControls.DropDownList ddlMascara =
                        (System.Web.UI.WebControls.DropDownList)frvLoteSelecionado.FindControl("ddlMascara");
                    string mascara = ddlMascara.Text;
                    System.Web.UI.WebControls.DropDownList ddlPoderLampada =
                        (System.Web.UI.WebControls.DropDownList)frvLoteSelecionado.FindControl("ddlPoderLampada");
                    string podeLampada = ddlPoderLampada.Text;

                    // Peso e Uniformidade
                    //System.Web.UI.WebControls.TextBox txtPeso = (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtPeso");
                    decimal peso = 0;
                    //if (decimal.TryParse(txtPeso.Text, out peso)) peso = Convert.ToDecimal(txtPeso.Text);
                    //System.Web.UI.WebControls.TextBox txtUniformidade = (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtUniformidade");
                    decimal uniformidade = 0;
                    //if (decimal.TryParse(txtUniformidade.Text, out uniformidade)) uniformidade = Convert.ToDecimal(txtUniformidade.Text);

                    // Dados de Falta
                    System.Web.UI.WebControls.TextBox txtFaltaQtde =
                        (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtFaltaQtde");
                    int faltaQtde = 0;
                    if (int.TryParse(txtFaltaQtde.Text, out faltaQtde)) faltaQtde = Convert.ToInt32(txtFaltaQtde.Text);
                    System.Web.UI.WebControls.TextBox txtFaltaMotivo =
                        (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtFaltaMotivo");
                    string faltaMotivo = txtFaltaMotivo.Text;
                    System.Web.UI.WebControls.Label lblErroFrmLoteSelecionadoMotivo =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblErroFrmLoteSelecionadoMotivo");
                    
                    System.Web.UI.WebControls.LinkButton UpdateButton =
                            (System.Web.UI.WebControls.LinkButton)frvLoteSelecionado.FindControl("UpdateButton");

                    string linhagemSelecionada = Session["linhagemSelecionada"].ToString().Replace("amp;", "");

                    #endregion

                    #region Verifica Motivo da Falta caso tenha qtde. de falta informada

                    if (faltaQtde > 0 && faltaMotivo == "")
                    {
                        lblErroFrmLoteSelecionadoMotivo.Text = "Motivo da falta obrigatório!";
                        lblErroFrmLoteSelecionadoMotivo.Visible = true;
                        txtFaltaMotivo.Focus();
                        return;
                    }

                    #endregion

                    #region Verifica Saldo

                    HLBAPPEntities hlbapp = new HLBAPPEntities();
                    var listaLotesRelacionados = hlbapp.HATCHERY_ORDER_FLOCK_DATA
                        .Where(w => 
                            //w.Hatch_Loc == incubatorio
                            //&& w.Set_date == dataIncubacao
                            //&& w.Flock_id == loteCompleto && w.ClassOvo == classOvo
                            //&& 
                            w.Variety == linhagemSelecionada
                            && w.OrderNoCHIC == numPedidoSelecionado)
                        .ToList();

                    int qtdRelacionada = 0;
                    if (listaLotesRelacionados.Count > 0) qtdRelacionada = Convert.ToInt32(listaLotesRelacionados.Sum(s => s.Qtde));

                    //string siteKey = Session["siteKey"].ToString();
                    //CH_BOOKEDTableAdapter bTA = new CH_BOOKEDTableAdapter();
                    //CHICOracleDataSet.CH_BOOKEDDataTable bDT = new CHICOracleDataSet.CH_BOOKEDDataTable();
                    //bTA.FillVarietiesByOrderNoAndLocation(bDT, siteKey, numPedidoSelecionado, ddlIncubatorios.SelectedValue.Replace("TB","AJ"));
                    //CH_ITEMSTableAdapter iTA = new CH_ITEMSTableAdapter();
                    //CHICOracleDataSet.CH_ITEMSDataTable iDT = new CHICOracleDataSet.CH_ITEMSDataTable();
                    //iTA.Fill(iDT, siteKey);
                    //CH_VARTABLTableAdapter vTA = new CH_VARTABLTableAdapter();
                    //CHICOracleDataSet.CH_VARTABLDataTable vDT = new CHICOracleDataSet.CH_VARTABLDataTable();
                    //vTA.Fill(vDT, siteKey);

                    //Session["qtdePedidoSelecionado"] = bDT
                    //    .Where(w => iDT.Any(a => a.ITEM_NO == w.ITEM
                    //        && vDT.Any(v => v.VARIETY == a.VARIETY
                    //            && v.FLIP == linhagemSelecionada)))
                    //    .Sum(s => s.QUANTITY).ToString();

                    var localNascimento = ddlIncubatorios.SelectedValue.Replace("TB", "AJ");
                    Session["qtdePedidoSelecionado"] = carregaQtdePedido(numPedidoSelecionado, linhagemSelecionada, localNascimento);

                    int qtdPedido = Convert.ToInt32(Session["qtdePedidoSelecionado"]);

                    #endregion

                    if ((qtdPedido - qtdRelacionada) < qtde)
                    {
                        lblErroFrmLoteSelecionado.Text = "A quantidade informada somada a já relacionada é maior que "
                            //+ "a quantidade do Pedido ou a Linhagem não existe no Pedido!";
                            + "a quantidade do Pedido!";
                        lblErroFrmLoteSelecionado.Visible = true;
                        UpdateButton.Enabled = false;
                        txtQtde.Focus();
                        return;
                    }

                    lblErroFrmLoteSelecionado.Visible = false;
                    UpdateButton.Enabled = true;

                    #region Insere no WEB

                    HATCHERY_ORDER_FLOCK_DATA flock = hlbapp.HATCHERY_ORDER_FLOCK_DATA
                        .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                            && w.Flock_id == loteCompleto && w.ClassOvo == classOvo
                            && w.OrderNoCHIC == numPedidoSelecionado
                            && w.Rotulo == rotulo
                            && w.TIMascara == mascara
                            && w.TIPoderLampada == podeLampada)
                        .FirstOrDefault();

                    if (flock == null) flock = new HATCHERY_ORDER_FLOCK_DATA();

                    flock.Hatch_Loc = incubatorio;
                    flock.Set_date = dataIncubacao;
                    flock.OrderNoCHIC = numPedidoSelecionado;
                    flock.Variety = linhagemSelecionada;
                    flock.Flock_id = loteCompleto;
                    flock.NumLote = loteCompleto;
                    flock.ClassOvo = classOvo;
                    flock.Qtde = qtde;
                    flock.Rotulo = rotulo;
                    flock.TIMascara = mascara;
                    flock.TIPoderLampada = podeLampada;
                    flock.FaltaQtde = faltaQtde;
                    flock.FaltaMotivo = faltaMotivo;
                    flock.Peso = peso;
                    flock.Uniformidade = uniformidade;

                    if (flock.ID == 0) hlbapp.HATCHERY_ORDER_FLOCK_DATA.AddObject(flock);

                    hlbapp.SaveChanges();

                    #endregion

                    gdvLotes.DataBind();
                    gdvLotesSelecionados.DataBind();

                    frvLoteSelecionado.ChangeMode(FormViewMode.ReadOnly);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            gdvLotes.SelectedIndex = -1;
            frvLoteSelecionado.ChangeMode(FormViewMode.ReadOnly);
            frvLoteSelecionado.Visible = false;
        }

        protected void imgbSaveLoteRelacionado_Click(object sender, ImageClickEventArgs e)
        {
            int index = gdvLotesSelecionados.EditIndex;
            int id = 0;

            //Label lblID = (Label)gdvLotesSelecionados.Rows[index].FindControl("Label1");
            //id = Convert.ToInt32(lblID.Text);
            id = Convert.ToInt32(gdvLotesSelecionados.Rows[index].Cells[1].Text);

            TextBox txtQtde = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("TextBox1");
            decimal qtde = Convert.ToDecimal(txtQtde.Text);
            DropDownList ddlRotulo = (DropDownList)gdvLotesSelecionados.Rows[index].FindControl("ddlRotulo");
            string rotulo = ddlRotulo.SelectedValue;
            DropDownList ddlMascara = (DropDownList)gdvLotesSelecionados.Rows[index].FindControl("ddlMascara");
            string mascara = ddlMascara.SelectedValue;
            DropDownList ddlPoderLampada = (DropDownList)gdvLotesSelecionados.Rows[index].FindControl("ddlPoderLampada");
            string poderLampada = ddlPoderLampada.SelectedValue;

            //TextBox txtPeso = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("txtPeso");
            decimal peso = 0;
            //if (decimal.TryParse(txtPeso.Text, out peso)) peso = Convert.ToDecimal(txtPeso.Text);
            //TextBox txtUniformidade = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("txtUniformidade");
            decimal uniformidade = 0;
            //if (decimal.TryParse(txtUniformidade.Text, out uniformidade)) uniformidade = Convert.ToDecimal(txtUniformidade.Text);

            TextBox txtFaltaQtde = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("txtFaltaQtde");
            int faltaQtde = 0;
            if (int.TryParse(txtFaltaQtde.Text, out faltaQtde)) faltaQtde = Convert.ToInt32(txtFaltaQtde.Text);
            TextBox txtFaltaMotivo = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("txtFaltaMotivo");
            string faltaMotivo = txtFaltaMotivo.Text;
            Label lblErroFrmLoteSelecionadoMotivo = (Label)gdvLotesSelecionados.Rows[index].FindControl("lblErroFrmLoteSelecionadoMotivo");

            #region Verifica Motivo da Falta caso tenha qtde. de falta informada

            if (faltaQtde > 0 && faltaMotivo == "")
            {
                lblErroFrmLoteSelecionadoMotivo.Text = "Motivo da falta obrigatório!";
                lblErroFrmLoteSelecionadoMotivo.Visible = true;
                txtFaltaMotivo.Focus();
                return;
            }

            #endregion

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HATCHERY_ORDER_FLOCK_DATA loteRelacionado = hlbapp.HATCHERY_ORDER_FLOCK_DATA
                .Where(w => w.ID == id).FirstOrDefault();
            loteRelacionado.Qtde = Convert.ToInt32(qtde);
            loteRelacionado.Rotulo = rotulo;
            loteRelacionado.TIMascara = mascara;
            loteRelacionado.TIPoderLampada = poderLampada;
            loteRelacionado.FaltaQtde = faltaQtde;
            loteRelacionado.FaltaMotivo = faltaMotivo;
            loteRelacionado.Peso = peso;
            loteRelacionado.Uniformidade = uniformidade;
            hlbapp.SaveChanges();

            gdvLotesSelecionados.EditIndex = -1;
            gdvLotesSelecionados.DataBind();
            gdvLotes.DataBind();
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            int index = gdvLotesSelecionados.EditIndex;
            TextBox txtQtde = (TextBox)gdvLotesSelecionados.Rows[index].FindControl("TextBox1");
            int id = Convert.ToInt32(gdvLotesSelecionados.Rows[index].Cells[1].Text);

            ImageButton imgbSaveLoteRelacionado =
                (ImageButton)gdvLotesSelecionados.Rows[index].FindControl("imgbSaveLoteRelacionado");

            if (txtQtde.Text == "")
            {
                Label lblMensagemQtdeLoteRelacionado =
                        (Label)gdvLotesSelecionados.Rows[index].FindControl("lblMensagemQtdeLoteRelacionado");
                lblMensagemQtdeLoteRelacionado.Text = "Não pode deixar em branco!";
                imgbSaveLoteRelacionado.Enabled = false;
                txtQtde.Focus();
            }
            else
            {
                int qtde = Convert.ToInt32(txtQtde.Text);

                string linhagemLote = gdvLotesSelecionados.Rows[index].Cells[7].Text;
                string lote = gdvLotesSelecionados.Rows[index].Cells[5].Text;
                string tipoOvo = gdvLotesSelecionados.Rows[index].Cells[8].Text;
                string numPedidoSelecionado = Session["numPedidoSelecionado"].ToString();
                string siteKey = Session["siteKey"].ToString();
                //CH_BOOKEDTableAdapter bTA = new CH_BOOKEDTableAdapter();
                //CHICOracleDataSet.CH_BOOKEDDataTable bDT = new CHICOracleDataSet.CH_BOOKEDDataTable();
                //bTA.FillVarietiesByOrderNoAndLocation(bDT, siteKey, numPedidoSelecionado, ddlIncubatorios.SelectedValue.Replace("TB","AJ"));
                //CH_ITEMSTableAdapter iTA = new CH_ITEMSTableAdapter();
                //CHICOracleDataSet.CH_ITEMSDataTable iDT = new CHICOracleDataSet.CH_ITEMSDataTable();
                //iTA.Fill(iDT, siteKey);

                string linhagemSelecionada = Session["linhagemSelecionada"].ToString().Replace("amp;", "");

                //Session["qtdePedidoSelecionado"] = bDT
                //    .Where(w => iDT.Any(a => a.ITEM_NO == w.ITEM
                //        && a.VARIETY == linhagemSelecionada))
                //    .Sum(s => s.QUANTITY).ToString();

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var localNascimento = ddlIncubatorios.SelectedValue.Replace("TB", "AJ");
                Session["qtdePedidoSelecionado"] = carregaQtdePedido(numPedidoSelecionado, linhagemSelecionada, localNascimento);

                int qtdPedido = Convert.ToInt32(Session["qtdePedidoSelecionado"]);

                int qtdPintosVendaveis = Convert.ToInt32(hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Set_date == Calendar1.SelectedDate
                        && w.Hatch_Loc == ddlIncubatorios.SelectedValue
                        && w.Flock_id == lote && w.ClassOvo == tipoOvo)
                    .Sum(s => s.Pintos_Vendaveis));

                int qtdPintosVendaveisUtilizada = Convert.ToInt32(hlbapp.HATCHERY_ORDER_FLOCK_DATA
                    .Where(w => w.Set_date == Calendar1.SelectedDate
                        && w.Hatch_Loc == ddlIncubatorios.SelectedValue
                        && w.Flock_id == lote && w.ClassOvo == tipoOvo
                        && w.ID != id)
                    .Sum(s => s.Qtde));

                int qtdSaldo = qtdPintosVendaveis - qtdPintosVendaveisUtilizada;

                int qtdPintosVendaveisUtilizadaPedido = Convert.ToInt32(hlbapp.HATCHERY_ORDER_FLOCK_DATA
                    .Where(w => 
                        //w.Set_date == Calendar1.SelectedDate
                        //&& w.Hatch_Loc == ddlIncubatorios.SelectedValue
                        //&& 
                        w.Variety == linhagemSelecionada
                        && w.OrderNoCHIC == numPedidoSelecionado
                        && w.ID != id)
                    .Sum(s => s.Qtde));

                if (qtde > qtdSaldo)
                {
                    Label lblMensagemQtdeLoteRelacionado =
                        (Label)gdvLotesSelecionados.Rows[index].FindControl("lblMensagemQtdeLoteRelacionado");
                    lblMensagemQtdeLoteRelacionado.Text = "Qtde. maior que Saldo Disponível! ("
                        + qtdSaldo.ToString() + ")";
                    imgbSaveLoteRelacionado.Enabled = false;
                    txtQtde.Focus();
                }
                else if (qtde < 0)
                {
                    Label lblMensagemQtdeLoteRelacionado =
                        (Label)gdvLotesSelecionados.Rows[index].FindControl("lblMensagemQtdeLoteRelacionado");
                    lblMensagemQtdeLoteRelacionado.Text = "Qtde. não pode ser menor que zero!";
                    imgbSaveLoteRelacionado.Enabled = false;
                    txtQtde.Focus();
                }
                else if (qtdPedido < (qtdPintosVendaveisUtilizadaPedido + qtde))
                {
                    Label lblMensagemQtdeLoteRelacionado =
                        (Label)gdvLotesSelecionados.Rows[index].FindControl("lblMensagemQtdeLoteRelacionado");
                    lblMensagemQtdeLoteRelacionado.Text = "A quantidade informada somada a já relacionada é maior que "
                        + "a quantidade do Pedido!";
                    imgbSaveLoteRelacionado.Enabled = false;
                    txtQtde.Focus();
                }
                else
                {
                    Label lblMensagemQtdeLoteRelacionado =
                        (Label)gdvLotesSelecionados.Rows[index].FindControl("lblMensagemQtdeLoteRelacionado");
                    lblMensagemQtdeLoteRelacionado.Text = "";
                    imgbSaveLoteRelacionado.Enabled = true;
                }
            }
        }

        protected void gdvLotesSelecionados_RowEditing(object sender, GridViewEditEventArgs e)
        {
            
        }

        protected void imgbDeleteLoteSelecionado_Click(object sender, ImageClickEventArgs e)
        {
            gdvLotes.DataBind();
        }

        protected void txtQtde_TextChanged(object sender, EventArgs e)
        {
            if (Session["qtdPintosVendaveisLote"] != null)
            {
                System.Web.UI.WebControls.TextBox txtQtde =
                        (System.Web.UI.WebControls.TextBox)frvLoteSelecionado.FindControl("txtQtde");
                int qtde = Convert.ToInt32(txtQtde.Text);

                int qtdPintosVendaveisLote = Convert.ToInt32(Session["qtdPintosVendaveisLote"]);

                System.Web.UI.WebControls.Label lblErroFrmLoteSelecionado =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblErroFrmLoteSelecionado");
                System.Web.UI.WebControls.LinkButton UpdateButton =
                        (System.Web.UI.WebControls.LinkButton)frvLoteSelecionado.FindControl("UpdateButton");

                System.Web.UI.WebControls.Label lblLinhagem =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblLinhagem");
                string linhagemLote = lblLinhagem.Text;
                string numPedidoSelecionado = Session["numPedidoSelecionado"].ToString();
                string siteKey = Session["siteKey"].ToString();
                //CH_BOOKEDTableAdapter bTA = new CH_BOOKEDTableAdapter();
                //CHICOracleDataSet.CH_BOOKEDDataTable bDT = new CHICOracleDataSet.CH_BOOKEDDataTable();
                //bTA.FillVarietiesByOrderNoAndLocation(bDT, siteKey, numPedidoSelecionado,
                //    ddlIncubatorios.SelectedValue.Replace("TB","AJ"));
                //CH_ITEMSTableAdapter iTA = new CH_ITEMSTableAdapter();
                //CHICOracleDataSet.CH_ITEMSDataTable iDT = new CHICOracleDataSet.CH_ITEMSDataTable();
                //iTA.Fill(iDT, siteKey);
                //CH_VARTABLTableAdapter vTA = new CH_VARTABLTableAdapter();
                //CHICOracleDataSet.CH_VARTABLDataTable vDT = new CHICOracleDataSet.CH_VARTABLDataTable();
                //vTA.Fill(vDT, siteKey);

                string linhagemSelecionada = Session["linhagemSelecionada"].ToString().Replace("amp;","");

                //Session["qtdePedidoSelecionado"] = bDT
                //    .Where(w => iDT.Any(a => a.ITEM_NO == w.ITEM
                //        && vDT.Any(v => v.VARIETY == a.VARIETY
                //            && v.FLIP == linhagemSelecionada)))
                //    .Sum(s => s.QUANTITY).ToString();

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var localNascimento = ddlIncubatorios.SelectedValue.Replace("TB", "AJ");
                Session["qtdePedidoSelecionado"] = carregaQtdePedido(numPedidoSelecionado, linhagemSelecionada, localNascimento);

                int qtdPedido = Convert.ToInt32(Session["qtdePedidoSelecionado"]);

                if (qtdPintosVendaveisLote < qtde)
                {
                    lblErroFrmLoteSelecionado.Text = "O valor não pode ser maior que o dísponível!";
                    lblErroFrmLoteSelecionado.Visible = true;
                    UpdateButton.Enabled = false;
                    txtQtde.Focus();
                }
                else
                {
                    lblErroFrmLoteSelecionado.Visible = false;
                    UpdateButton.Enabled = true;
                }

                DateTime dataIncubacao = Calendar1.SelectedDate;
                string incubatorio = ddlIncubatorios.SelectedValue;
                System.Web.UI.WebControls.Label lblLoteCompleto =
                        (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblLoteCompleto");
                string loteCompleto = lblLoteCompleto.Text;
                System.Web.UI.WebControls.Label lblClassOvo =
                    (System.Web.UI.WebControls.Label)frvLoteSelecionado.FindControl("lblClassOvo");
                string classOvo = lblClassOvo.Text;

                var listaLotesRelacionados = hlbapp.HATCHERY_ORDER_FLOCK_DATA
                    .Where(w => 
                        //w.Hatch_Loc == incubatorio 
                        //&& w.Set_date == dataIncubacao
                        //&& w.Flock_id == loteCompleto && w.ClassOvo == classOvo
                        //&& 
                        w.Variety == linhagemSelecionada
                        && w.OrderNoCHIC == numPedidoSelecionado)
                    .ToList();

                int qtdRelacionada = 0;
                if (listaLotesRelacionados.Count > 0) qtdRelacionada = Convert.ToInt32(listaLotesRelacionados.Sum(s => s.Qtde));

                if ((qtdPedido - qtdRelacionada) < qtde)
                {
                    lblErroFrmLoteSelecionado.Text = "A quantidade informada somada a já relacionada é maior que "
                        //+ "a quantidade do Pedido ou a Linhagem não existe no Pedido!";
                        + "a quantidade do Pedido!";
                    lblErroFrmLoteSelecionado.Visible = true;
                    UpdateButton.Enabled = false;
                    txtQtde.Focus();
                }
                else
                {
                    lblErroFrmLoteSelecionado.Visible = false;
                    UpdateButton.Enabled = true;
                } 
            }
        }

        protected void carregaDadosVacinaSelecionada(GridViewRow row)
        {
            #region Carrega Vacina

            System.Web.UI.WebControls.DropDownList ddlVacinas = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlVacinas");
            string vacina = ddlVacinas.SelectedValue;

            //Models.Apolo2.Apolo10Entities2 apolo2 = new Models.Apolo2.Apolo10Entities2();
            //Models.Apolo2.PRODUTO vacinaSelecionada = apolo2.PRODUTO.Where(w => w.ProdNomeAlt1 == vacina).FirstOrDefault();
            Models.DiarioProducaoRacao.DiarioProducaoRacaoEntities apolo2 = new Models.DiarioProducaoRacao.DiarioProducaoRacaoEntities();
            var vacinaSelecionada = apolo2.PRODUTO.Where(w => w.ProdNomeAlt1 == vacina).FirstOrDefault();

            #endregion

            if (vacinaSelecionada != null)
            {
                #region Carrega Laboratorio

                System.Web.UI.WebControls.Label lblLaboratorio = (System.Web.UI.WebControls.Label)row.FindControl("lblLaboratorio");
                var laboratorio = apolo2.MARCA_PROD.Where(w => w.MarcaProdCod == vacinaSelecionada.MarcaProdCod).FirstOrDefault();
                if (laboratorio != null)
                {
                    lblLaboratorio.Text = laboratorio.MarcaProdNome;
                }
                else
                {
                    lblLaboratorio.Text = "";
                }

                #endregion

                #region Carrega Tipos de Doses

                var listaQtdeTipoDose = apolo2.PROD_UNID_MED
                    .Where(w => w.ProdCodEstr == vacinaSelecionada.ProdCodEstr && w.USERExibeRastrIncubatorio == "Sim")
                    .OrderBy(o => o.ProdUnidMedPeso)
                    .ToList();
                System.Web.UI.WebControls.DropDownList ddlQtdeDosesPorAmpola = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlQtdeDosesPorAmpola");
                ddlQtdeDosesPorAmpola.Items.Clear();

                #region Se for edição, marcar como selecionado o que está no BD

                bool selected = false;
                int qtdeDosesPorAmpola = 0;
                if (gdvVacinas.EditIndex == row.RowIndex)
                {
                    HLBAPPEntities hlbappSession = new HLBAPPEntities();
                    var ordernoCHIC = Session["numPedidoSelecionado"].ToString();
                    var linhagem = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
                    var vacinaBD = hlbappSession.HATCHERY_ORDER_VACC_DATA
                        .Where(w => w.Hatch_Loc == ddlIncubatorios.SelectedValue
                                && w.Set_date == Calendar1.SelectedDate
                                && w.OrderNoCHIC == ordernoCHIC
                                && w.Variety == linhagem
                                && w.Vacina == vacina)
                    .FirstOrDefault();
                    if (vacinaBD != null)
                        if (vacinaBD.QtdeDosesPorAmpola != null)
                            int.TryParse(vacinaBD.QtdeDosesPorAmpola.ToString(), out qtdeDosesPorAmpola);
                }

                #endregion

                foreach (var item in listaQtdeTipoDose)
                {
                    if (item.ProdUnidMedPeso == qtdeDosesPorAmpola)
                        selected = true;
                    else
                        selected = false;
                    ddlQtdeDosesPorAmpola.Items.Add(new ListItem
                    {
                        Text = String.Format("{0:N0}", item.ProdUnidMedPeso),
                        Value = Convert.ToInt32(item.ProdUnidMedPeso).ToString(),
                        Selected = selected
                    });
                }

                #endregion
            }
        }

        protected void calculaQtdeTotalVacinas(GridViewRow row)
        {
            System.Web.UI.WebControls.TextBox txtQtdeAmpolas = (System.Web.UI.WebControls.TextBox)row.FindControl("txtQtdeAmpolas");
            int qtdeAmpolas = 0;
            if (txtQtdeAmpolas.Text != "") qtdeAmpolas = Convert.ToInt32(txtQtdeAmpolas.Text);

            System.Web.UI.WebControls.DropDownList ddlQtdeDosesPorAmpola = (System.Web.UI.WebControls.DropDownList)row.FindControl("ddlQtdeDosesPorAmpola");
            int qtdeDosesPorAmpola = 0;
            qtdeDosesPorAmpola = Convert.ToInt32(ddlQtdeDosesPorAmpola.SelectedValue);

            System.Web.UI.WebControls.Label lblQtdeDosesTotal = (System.Web.UI.WebControls.Label)row.FindControl("lblQtdeDosesTotal");
            lblQtdeDosesTotal.Text = String.Format("{0:N0}", (qtdeAmpolas * qtdeDosesPorAmpola));
        }

        protected void imgbAdd_Click(object sender, ImageClickEventArgs e)
        {
            gdvVacinas.ShowFooter = true;
        }

        protected void txtQtdeAmpolas_TextChanged(object sender, EventArgs e)
        {
            GridViewRow row;
            if (gdvVacinas.EditIndex < 0)
                row = gdvVacinas.FooterRow;
            else
                row = gdvVacinas.Rows[gdvVacinas.EditIndex];
            calculaQtdeTotalVacinas(row);
        }

        protected void ddlVacinas_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row;
            if (gdvVacinas.EditIndex < 0)
                row = gdvVacinas.FooterRow;
            else
                row = gdvVacinas.Rows[gdvVacinas.EditIndex];
            carregaDadosVacinaSelecionada(row);
            calculaQtdeTotalVacinas(row);
        }

        protected void ddlQtdeDosesPorAmpola_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row;
            if (gdvVacinas.EditIndex < 0)
                row = gdvVacinas.FooterRow;
            else
                row = gdvVacinas.Rows[gdvVacinas.EditIndex];
            calculaQtdeTotalVacinas(row);
        }

        protected void txtPartida_TextChanged(object sender, EventArgs e)
        {
            //System.Web.UI.WebControls.TextBox txtPartida =
            //            (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex]
            //            .FindControl("txtPartida");
            //string partida = txtPartida.Text;

            //System.Web.UI.WebControls.Label lblMsgPartida =
            //            (System.Web.UI.WebControls.Label)gdvVacinas.Rows[gdvVacinas.EditIndex]
            //            .FindControl("lblMsgPartida");
            //System.Web.UI.WebControls.ImageButton imgbSaveVacina =
            //        (System.Web.UI.WebControls.ImageButton)gdvVacinas.Rows[gdvVacinas.EditIndex]
            //        .FindControl("imgbSaveVacina");

            //if (partida.Trim() == "/")
            //{
            //    lblMsgPartida.Text = "O campo Partida não pode ficar em branco!";
            //    lblMsgPartida.Visible = true;
            //    imgbSaveVacina.Visible = false;
            //}
            //else
            //{
            //    lblMsgPartida.Visible = false;
            //    imgbSaveVacina.Visible = true;
            //}
        }

        protected void imgbSaveVacina_Click(object sender, ImageClickEventArgs e)
        {
            #region Carrega Componentes

            lblMsgVerificaVacinas.Text = "";
            lblMsgVerificaVacinas.Visible = false;

            System.Web.UI.WebControls.TextBox txtPartida = (System.Web.UI.WebControls.TextBox)gdvVacinas.FooterRow.FindControl("txtPartida");
            string partida = txtPartida.Text;

            System.Web.UI.WebControls.TextBox txtDataFabricacao = (System.Web.UI.WebControls.TextBox)gdvVacinas.FooterRow.FindControl("txtDataFabricacao");
            DateTime dataFabricacao = DateTime.Today;

            System.Web.UI.WebControls.TextBox txtDataValidade = (System.Web.UI.WebControls.TextBox)gdvVacinas.FooterRow.FindControl("txtDataValidade");
            DateTime dataValidade = DateTime.Today;

            System.Web.UI.WebControls.DropDownList ddlVacinas = (System.Web.UI.WebControls.DropDownList)gdvVacinas.FooterRow.FindControl("ddlVacinas");
            System.Web.UI.WebControls.Label lblLaboratorio = (System.Web.UI.WebControls.Label)gdvVacinas.FooterRow.FindControl("lblLaboratorio");
            System.Web.UI.WebControls.TextBox txtQtdeAmpolas = (System.Web.UI.WebControls.TextBox)gdvVacinas.FooterRow.FindControl("txtQtdeAmpolas");
            System.Web.UI.WebControls.DropDownList ddlQtdeDosesPorAmpola = (System.Web.UI.WebControls.DropDownList)gdvVacinas.FooterRow.FindControl("ddlQtdeDosesPorAmpola");
            System.Web.UI.WebControls.Label lblQtdeDosesTotal = (System.Web.UI.WebControls.Label)gdvVacinas.FooterRow.FindControl("lblQtdeDosesTotal");
            System.Web.UI.WebControls.TextBox txtObservacoes = (System.Web.UI.WebControls.TextBox)gdvVacinas.FooterRow.FindControl("txtObservacoes");

            #endregion

            #region Verifica inserção dos Componentes

            HLBAPPEntities hlbappSession = new HLBAPPEntities();
            var ordernoCHIC = Session["numPedidoSelecionado"].ToString();
            var linhagem = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
            var vacinaBD = hlbappSession.HATCHERY_ORDER_VACC_DATA
                .Where(w => w.Hatch_Loc == ddlIncubatorios.SelectedValue
                        && w.Set_date == Calendar1.SelectedDate
                        && w.OrderNoCHIC == ordernoCHIC
                        && w.Variety == linhagem
                        && w.Vacina == ddlVacinas.SelectedValue
                        && w.Partida == txtPartida.Text)
                .FirstOrDefault();
            if (vacinaBD != null)
            {
                lblMsgVerificaVacinas.Text = "A vacina selecionada já foi lançada nesse pedido / linhagem com a mesma partida! Por favor, verifique!";
                lblMsgVerificaVacinas.Visible = true;
                return;
            }
            else if (txtPartida.Text.Trim() == "/")
            {
                lblMsgVerificaVacinas.Text = "Campo 'PARTIDA' da vacina obrigatório!";
                lblMsgVerificaVacinas.Visible = true;
                return;
            }
            else if (txtDataFabricacao.Text == "")
            {
                lblMsgVerificaVacinas.Text = "Campo 'DATA DE FABRICAÇÃO' da vacina obrigatório!";
                lblMsgVerificaVacinas.Visible = true;
                return;
            }
            else if (txtDataValidade.Text == "")
            {
                lblMsgVerificaVacinas.Text = "Campo 'DATA DE VALIDADE' da vacina obrigatório!";
                lblMsgVerificaVacinas.Visible = true;
                return;
            }
            else if (txtQtdeAmpolas.Text == "")
            {
                lblMsgVerificaVacinas.Text = "Campo 'QTDE. AMPOLAS UTILIZADAS' da vacina obrigatório!";
                lblMsgVerificaVacinas.Visible = true;
                return;
            }

            #endregion

            #region Insere no WEB

            HATCHERY_ORDER_VACC_DATA vacina = new HATCHERY_ORDER_VACC_DATA();
            vacina.Hatch_Loc = ddlIncubatorios.SelectedValue;
            vacina.Set_date = Calendar1.SelectedDate;
            vacina.OrderNoCHIC = Session["numPedidoSelecionado"].ToString();
            vacina.Vacina = ddlVacinas.SelectedValue;
            vacina.Laboratorio = lblLaboratorio.Text;
            vacina.Partida = txtPartida.Text;
            vacina.Variety = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
            if (DateTime.TryParse(txtDataFabricacao.Text, out dataFabricacao))
                vacina.DataFabricacao = dataFabricacao;
            if (DateTime.TryParse(txtDataValidade.Text, out dataValidade))
                vacina.DataValidade = dataValidade;
            vacina.QtdeAmpolas = Convert.ToInt32(txtQtdeAmpolas.Text);
            vacina.QtdeDosesPorAmpola = Convert.ToInt32(ddlQtdeDosesPorAmpola.SelectedValue);
            vacina.Observacao = txtObservacoes.Text;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            hlbapp.HATCHERY_ORDER_VACC_DATA.AddObject(vacina);
            hlbapp.SaveChanges();

            #endregion

            #region Atualiza Tabela de Vacinas

            gdvVacinas.DataBind();
            gdvVacinas.ShowFooter = false;

            #endregion
        }

        protected void imgbCancelVacina_Click(object sender, ImageClickEventArgs e)
        {
            gdvVacinas.ShowFooter = false;
        }

        protected void gdvVacinas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("CustomUpdate"))
            {
                #region Carrega Componentes

                HLBAPPEntities hlbappSession = new HLBAPPEntities();

                lblMsgVerificaVacinas.Text = "";
                lblMsgVerificaVacinas.Visible = false;

                System.Web.UI.WebControls.TextBox txtPartida = (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("txtPartida");
                string partida = txtPartida.Text;

                System.Web.UI.WebControls.TextBox txtDataFabricacao = (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("txtDataFabricacao");
                DateTime dataFabricacao = DateTime.Today;

                System.Web.UI.WebControls.TextBox txtDataValidade = (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("txtDataValidade");
                DateTime dataValidade = DateTime.Today;

                System.Web.UI.WebControls.DropDownList ddlVacinas = (System.Web.UI.WebControls.DropDownList)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("ddlVacinas");
                System.Web.UI.WebControls.Label lblLaboratorio = (System.Web.UI.WebControls.Label)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("lblLaboratorio");
                System.Web.UI.WebControls.TextBox txtQtdeAmpolas = (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("txtQtdeAmpolas");
                System.Web.UI.WebControls.DropDownList ddlQtdeDosesPorAmpola = (System.Web.UI.WebControls.DropDownList)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("ddlQtdeDosesPorAmpola");
                System.Web.UI.WebControls.Label lblQtdeDosesTotal = (System.Web.UI.WebControls.Label)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("lblQtdeDosesTotal");
                System.Web.UI.WebControls.TextBox txtObservacoes = (System.Web.UI.WebControls.TextBox)gdvVacinas.Rows[gdvVacinas.EditIndex].FindControl("txtObservacoes");

                int id = Convert.ToInt32(e.CommandArgument.ToString());

                #endregion

                #region Verifica inserção dos Componentes

                var ordernoCHIC = Session["numPedidoSelecionado"].ToString();
                var linhagem = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
                var vacinaBD = hlbappSession.HATCHERY_ORDER_VACC_DATA
                    .Where(w => w.Hatch_Loc == ddlIncubatorios.SelectedValue
                            && w.Set_date == Calendar1.SelectedDate
                            && w.OrderNoCHIC == ordernoCHIC
                            && w.Variety == linhagem
                            && w.Vacina == ddlVacinas.SelectedValue
                            && w.Partida == txtPartida.Text
                            && w.ID != id)
                    .FirstOrDefault();
                if (vacinaBD != null)
                {
                    lblMsgVerificaVacinas.Text = "A vacina selecionada já foi lançada nesse pedido / linhagem com a mesma partida! Por favor, verifique!";
                    lblMsgVerificaVacinas.Visible = true;
                    return;
                }
                else if (txtPartida.Text.Trim() == "/")
                {
                    lblMsgVerificaVacinas.Text = "Campo 'PARTIDA' da vacina obrigatório!";
                    lblMsgVerificaVacinas.Visible = true;
                    return;
                }
                else if (txtDataFabricacao.Text == "")
                {
                    lblMsgVerificaVacinas.Text = "Campo 'DATA DE FABRICAÇÃO' da vacina obrigatório!";
                    lblMsgVerificaVacinas.Visible = true;
                    return;
                }
                else if (txtDataValidade.Text == "")
                {
                    lblMsgVerificaVacinas.Text = "Campo 'DATA DE VALIDADE' da vacina obrigatório!";
                    lblMsgVerificaVacinas.Visible = true;
                    return;
                }
                else if (txtQtdeAmpolas.Text == "")
                {
                    lblMsgVerificaVacinas.Text = "Campo 'QTDE. AMPOLAS UTILIZADAS' da vacina obrigatório!";
                    lblMsgVerificaVacinas.Visible = true;
                    return;
                }

                #endregion
                
                HATCHERY_ORDER_VACC_DATA vacina = hlbappSession.HATCHERY_ORDER_VACC_DATA.Where(w => w.ID == id).FirstOrDefault();
                vacina.Hatch_Loc = ddlIncubatorios.SelectedValue;
                vacina.Set_date = Calendar1.SelectedDate;
                vacina.OrderNoCHIC = Session["numPedidoSelecionado"].ToString();
                vacina.Vacina = ddlVacinas.SelectedValue;
                vacina.Laboratorio = lblLaboratorio.Text;
                vacina.Partida = txtPartida.Text;
                vacina.Variety = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
                if (DateTime.TryParse(txtDataFabricacao.Text, out dataFabricacao))
                    vacina.DataFabricacao = dataFabricacao;
                if (DateTime.TryParse(txtDataValidade.Text, out dataValidade))
                    vacina.DataValidade = dataValidade;
                vacina.QtdeAmpolas = Convert.ToInt32(txtQtdeAmpolas.Text);
                vacina.QtdeDosesPorAmpola = Convert.ToInt32(ddlQtdeDosesPorAmpola.SelectedValue);
                vacina.Observacao = txtObservacoes.Text;
                hlbappSession.SaveChanges();
                gdvVacinas.EditIndex = -1;
                gdvVacinas.DataBind();
            }
        }

        protected void gdvVacinas_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            
        }

        protected void gdvVacinas_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            
        }

        protected void gdvVacinas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Web.UI.WebControls.Label lblID = (System.Web.UI.WebControls.Label)e.Row.FindControl("Label1");
            if (lblID != null)
                if (lblID.Text.Equals("5"))
                {
                    lblID.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbEdit = (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgbEdit");
                    imgbEdit.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbCancelEdit = (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgbCancelEdit");
                    imgbCancelEdit.Visible = false;
                    //System.Web.UI.WebControls.DropDownList ddlQtdeDosesPorAmpola = (System.Web.UI.WebControls.DropDownList)e.Row.FindControl("ddlQtdeDosesPorAmpola");
                    //ddlQtdeDosesPorAmpola.SelectedValue = "";
                    //ddlQtdeDosesPorAmpola.Visible = false;
                    //System.Web.UI.WebControls.Label lblQtdeDosesTotal = (System.Web.UI.WebControls.Label)e.Row.FindControl("lblQtdeDosesTotal");
                    //lblQtdeDosesTotal.Visible = false;
                }

            if ((e.Row.RowType == DataControlRowType.Footer && gdvVacinas.ShowFooter == true)
                || (e.Row.RowType == DataControlRowType.DataRow && gdvVacinas.EditIndex == e.Row.RowIndex))
            {
                carregaDadosVacinaSelecionada(e.Row);
            }
        }

        protected void gdvVacinas_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            lblMsgVerificaVacinas.Text = "";
            lblMsgVerificaVacinas.Visible = false;
        }

        protected void gdvPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            #region Rotina para exibir mais de uma linhagem na lista. Desativado, pois a Ana da Planalto disse que é por Linha

            //if (e.Row.Cells.Count > 1)
            //{
            //    string numPedido = e.Row.Cells[5].Text;
            //    string siteKey = Session["siteKey"].ToString();
            //    string incubatorio = ddlIncubatorios.SelectedValue;

            //    CH_BOOKEDTableAdapter bTA = new CH_BOOKEDTableAdapter();
            //    CHICOracleDataSet.CH_BOOKEDDataTable bDT = new CHICOracleDataSet.CH_BOOKEDDataTable();
            //    bTA.FillVarietiesByOrderNoAndLocation(bDT, siteKey, numPedido, incubatorio);
            //    var listaAgrupada = bDT
            //        .GroupBy(g => g.ITEM)
            //        .Select(s => new { ITEM = s.Key, QTY = s.Sum(u => u.QUANTITY) }).ToList();
            //    if (listaAgrupada.Count > 0)
            //    {
            //        string linhagens = "";
            //        string qtdes = "";
            //        foreach (var item in listaAgrupada)
            //        {
            //            CH_ITEMSTableAdapter iTA = new CH_ITEMSTableAdapter();
            //            CHICOracleDataSet.CH_ITEMSDataTable iDT = new CHICOracleDataSet.CH_ITEMSDataTable();
            //            iTA.FillByItemNo(iDT, siteKey, item.ITEM);
            //            if (iDT.Count > 0)
            //            {
            //                CH_VARTABLTableAdapter vTA = new CH_VARTABLTableAdapter();
            //                CHICOracleDataSet.CH_VARTABLDataTable vDT = new CHICOracleDataSet.CH_VARTABLDataTable();
            //                vTA.FillByVariety(vDT, siteKey, iDT[0].VARIETY);

            //                if (vDT.Count > 0)
            //                {
            //                    if (listaAgrupada.IndexOf(item) == (listaAgrupada.Count - 1))
            //                    {
            //                        if (!vDT[0].IsFLIPNull()) linhagens = linhagens + vDT[0].FLIP;
            //                        qtdes = qtdes + item.QTY.ToString("0,0");
            //                    }
            //                    else
            //                    {
            //                        if (!vDT[0].IsFLIPNull()) linhagens = linhagens + vDT[0].FLIP + " / ";
            //                        qtdes = qtdes + item.QTY.ToString("0,0") + " / ";
            //                    }
            //                }
            //            }
            //        }

            //        e.Row.Cells[6].Text = linhagens;
            //        e.Row.Cells[7].Text = qtdes;
            //    }
            //}

            #endregion
        }

        protected void gdvLotesSelecionados_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            gdvLotes.DataBind();
        }

        protected void imgbAddRotulo_Click(object sender, ImageClickEventArgs e)
        {
            gdvRotulos.ShowFooter = true;
        }

        protected void gdvRotulos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            System.Web.UI.WebControls.Label lblID =
                        (System.Web.UI.WebControls.Label)e.Row.FindControl("Label1");
            if (lblID != null)
                if (lblID.Text.Equals("1"))
                {
                    lblID.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgbEdit");
                    imgbEdit.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbCancelEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgbCancelEdit");
                    imgbCancelEdit.Visible = false;
                }
        }

        protected void gdvRotulos_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            int id = Convert.ToInt32(e.Keys[0]);

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HATCHERY_ORDER_ROTULO_DATA vacina = hlbapp.HATCHERY_ORDER_ROTULO_DATA
                .Where(w => w.ID == id).FirstOrDefault();
            vacina.Hatch_Loc = ddlIncubatorios.SelectedValue;
            vacina.Set_date = Calendar1.SelectedDate;
            vacina.OrderNoCHIC = Session["numPedidoSelecionado"].ToString();
            vacina.Variety = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
            hlbapp.SaveChanges();
            gdvRotulos.DataBind();
        }

        protected void imgbSaveRotulo_Click(object sender, ImageClickEventArgs e)
        {
            System.Web.UI.WebControls.TextBox txtPesoMedio =
                         (System.Web.UI.WebControls.TextBox)gdvRotulos.FooterRow
                         .FindControl("txtPesoMedio");
            string pesoMedio = txtPesoMedio.Text;
            System.Web.UI.WebControls.TextBox txtUniformidade =
                         (System.Web.UI.WebControls.TextBox)gdvRotulos.FooterRow
                         .FindControl("txtUniformidade");
            string uniformidade = txtUniformidade.Text;
            DropDownList ddlCores = (DropDownList)gdvRotulos.FooterRow.FindControl("ddlCores");

            HATCHERY_ORDER_ROTULO_DATA rotulo = new HATCHERY_ORDER_ROTULO_DATA();
            rotulo.Hatch_Loc = ddlIncubatorios.SelectedValue;
            rotulo.Set_date = Calendar1.SelectedDate;
            rotulo.OrderNoCHIC = Session["numPedidoSelecionado"].ToString();
            rotulo.Cor = ddlCores.SelectedValue;
            rotulo.Variety = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
            if (pesoMedio != "") 
                rotulo.PesoMedio = Convert.ToDecimal(pesoMedio);
            else
                rotulo.PesoMedio = 0;
            if (uniformidade != "")
                rotulo.Uniformidade = Convert.ToDecimal(uniformidade);
            else
                rotulo.Uniformidade = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            hlbapp.HATCHERY_ORDER_ROTULO_DATA.AddObject(rotulo);
            hlbapp.SaveChanges();

            gdvRotulos.DataBind();
            gdvRotulos.ShowFooter = false;
        }

        protected void imgbCancelRotulo_Click(object sender, ImageClickEventArgs e)
        {
            gdvRotulos.ShowFooter = false;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            #region Carrega Variáveis

            string numPedidoSelecionado = Session["numPedidoSelecionado"].ToString();
            string linhagemSelecionada = Session["linhagemSelecionada"].ToString().Replace("amp;", "");
            string incubatorio = ddlIncubatorios.SelectedValue;
            DateTime setDate = Calendar1.SelectedDate;

            //decimal pesoMedio = 0;
            //if (txtPesoMedio.Text == "") pesoMedio = 0; else pesoMedio = Convert.ToDecimal(txtPesoMedio.Text);
            //decimal uniformidade = 0;
            //if (txtPercUniformidade.Text == "") uniformidade = 0; else uniformidade = Convert.ToDecimal(txtPercUniformidade.Text);
            DateTime? dataSaidaProgramada = null;
            if (txtDataSaidaProgramada.Text != "")
                if (txtHoraSaidaProgramada.Text != "")
                    dataSaidaProgramada = Convert.ToDateTime(txtDataSaidaProgramada.Text + " " +
                        txtHoraSaidaProgramada.Text);
            DateTime? dataSaidaReal = null;
            if (txtDataSaidaReal.Text != "")
                if (txtHoraSaidaReal.Text != "")
                    dataSaidaReal = Convert.ToDateTime(txtDataSaidaReal.Text + " " +
                        txtHoraSaidaReal.Text);
            int qtdeAmostra = 0;
            if (txtQtdeAmostra.Text == "") qtdeAmostra = 0; else qtdeAmostra = Convert.ToInt32(txtQtdeAmostra.Text);
            int qtdeVacinada = 0;
            if (txtQtdeVacinada.Text == "") qtdeVacinada = 0; else qtdeVacinada = Convert.ToInt32(txtQtdeVacinada.Text);
            int qtdePontoFioPretoUmbigo = 0;
            if (txtQtdePontoFioPretoUmbigo.Text == "") qtdePontoFioPretoUmbigo = 0;
            else qtdePontoFioPretoUmbigo = Convert.ToInt32(txtQtdePontoFioPretoUmbigo.Text);
            int qtdePesTortosDedosCurvos = 0; 
            if (txtQtdePesTortosDedosCurvos.Text == "") qtdePesTortosDedosCurvos = 0;
            else qtdePesTortosDedosCurvos = Convert.ToInt32(txtQtdePesTortosDedosCurvos.Text);
            int qtdeErroContagem = 0;
            if (txtQtdeErroContagem.Text == "") qtdeErroContagem = 0; else qtdeErroContagem = Convert.ToInt32(txtQtdeErroContagem.Text);
            int qtdeErroSexagem = 0;
            if (txtQtdeErroSexagem.Text == "") qtdeErroSexagem = 0;
            else qtdeErroSexagem = Convert.ToInt32(txtQtdeErroSexagem.Text);
            int qtdeErroSelecao = 0;
            if (txtQtdeErroSelecao.Text == "") qtdeErroSelecao = 0; 
            else qtdeErroSelecao = Convert.ToInt32(txtQtdeErroSelecao.Text);

            string observacoes = txtObservacao.Text;
            string repInspFinal = txtRepInspFinal.Text;
            string repExpedicaoCarga = txtRepExpedicaoCarga.Text;
            string respLiberacaoVeiculo = txtRespLiberacaoVeiculo.Text;
            string rnc = ddlRNC.SelectedValue;
            string disposicaoRNC = ddlDisposicaoRNC.SelectedValue;
            string numeroRNC = txtNumeroRNC.Text;

            #endregion

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            HATCHERY_ORDER_DATA pedido = hlbapp.HATCHERY_ORDER_DATA
                .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == setDate
                    && w.OrderNoCHIC == numPedidoSelecionado
                    && w.Variety == linhagemSelecionada).FirstOrDefault();

            bool existe = false;
            if (pedido == null)
                pedido = new HATCHERY_ORDER_DATA();
            else
                existe = true;

            pedido.Hatch_Loc = incubatorio;
            pedido.Set_date = setDate;
            pedido.OrderNoCHIC = numPedidoSelecionado;
            pedido.Variety = linhagemSelecionada;
            //pedido.PesoMedio = pesoMedio;
            //pedido.UniformidadePerc = uniformidade;
            if (pedido.PesoMedio == null) pedido.PesoMedio = 0;
            if (pedido.UniformidadePerc == null) pedido.UniformidadePerc = 0;
            pedido.SaidaProgramada = dataSaidaProgramada;
            pedido.SaidaReal = dataSaidaReal;
            pedido.QtdeAmostra = qtdeAmostra;
            pedido.QtdeVacinada = qtdeVacinada;
            pedido.QtdePontoFioPretoUmbigo = qtdePontoFioPretoUmbigo;
            pedido.QtdePesTortosDedosCurvos = qtdePesTortosDedosCurvos;
            pedido.QtdeErroContagem = qtdeErroContagem;
            pedido.QtdeErroSexagem = qtdeErroSexagem;
            pedido.QtdeErroSelecao = qtdeErroSelecao;
            pedido.Observacao = observacoes;
            pedido.RespInspecaoFinal = repInspFinal;
            pedido.RespExpedicaoCarga = repExpedicaoCarga;
            pedido.RespLiberacaoVeiculo = respLiberacaoVeiculo;
            pedido.RNC = rnc;
            pedido.DisposicaoRNC = disposicaoRNC;
            if (!rnc.Equals("(Nenhum)"))
                pedido.NumeroRNC = numeroRNC;
            else
                pedido.NumeroRNC = "";

            if (!existe) hlbapp.HATCHERY_ORDER_DATA.AddObject(pedido);
            hlbapp.SaveChanges();

            AtualizaFLIPSetDateHatchLoc(incubatorio, setDate, numPedidoSelecionado);

            DeselecionarPedido();
        }

        public void AtualizaFLIPSetDateHatchLoc(string hatchLoc, DateTime setDate, string numPedido)
        {
            #region Deleta Lotes do Pedido

            ORDER_DATATableAdapter oTA = new ORDER_DATATableAdapter();
            oTA.DeleteQuery(hatchLoc, numPedido);

            #endregion

            #region Carrega Dados do Pedido

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var listaLotesPedido = hlbapp.HATCHERY_ORDER_FLOCK_DATA
                .Where(w => w.Hatch_Loc == hatchLoc 
                    //&& w.Set_date == setDate 
                    && w.OrderNoCHIC == numPedido)
                .ToList();

            CHICOracleDataSet.ORDER_DATADataTable oDT = new CHICOracleDataSet.ORDER_DATADataTable();

            CH_ORDERSTableAdapter ordTA = new CH_ORDERSTableAdapter();
            CHICOracleDataSet.CH_ORDERSDataTable ordDT = new CHICOracleDataSet.CH_ORDERSDataTable();
            ordTA.FillByOrderNo(ordDT, numPedido);
            string rotuloDescr = "";

            #endregion

            string custno = "";
            if (ordDT.Count > 0)
            {
                foreach (var item in listaLotesPedido)
                {
                    #region Carrega Pedido

                    custno = ordDT.FirstOrDefault().CUST_NO;

                    HATCHERY_ORDER_DATA pedido = hlbapp.HATCHERY_ORDER_DATA
                        .Where(w => w.Hatch_Loc == hatchLoc
                            //&& w.Set_date == setDate 
                            && w.OrderNoCHIC == numPedido
                            && w.Variety == item.Variety)
                        .FirstOrDefault();

                    #endregion

                    if (pedido != null)
                    {
                        #region Carrega Dados do Pedido

                        int qtdAmostra = 1;
                        if (pedido.QtdeAmostra > 0) qtdAmostra = Convert.ToInt32(pedido.QtdeAmostra);
                        decimal percErroSelecao = (decimal)(pedido.QtdeErroSelecao / (qtdAmostra * 1.00m));
                        decimal percQtdeVacinada = (decimal)(pedido.QtdeVacinada / (qtdAmostra * 1.00m));
                        decimal percUniformidade = (decimal)pedido.UniformidadePerc;
                        decimal pesoMedio = (decimal)pedido.PesoMedio;
                        decimal percErroSexagem = (decimal)(pedido.QtdeErroSexagem / (qtdAmostra * 1.00m));
                        decimal percErroContagem = (decimal)(pedido.QtdeErroContagem / (qtdAmostra * 1.00m));
                        string observacao = pedido.Observacao;
                        string horaProgramada = Convert.ToDateTime(pedido.SaidaProgramada).ToString("HH") + "H"
                            + Convert.ToDateTime(pedido.SaidaProgramada).ToString("mm");
                        string horaReal = Convert.ToDateTime(pedido.SaidaReal).ToString("HH") + "H"
                            + Convert.ToDateTime(pedido.SaidaReal).ToString("mm");

                        #endregion

                        #region Carrega Variáveis dos Itens

                        percUniformidade = (decimal)pedido.UniformidadePerc;
                        pesoMedio = (decimal)pedido.PesoMedio;

                        string varietyCHIC = "";
                        string varietyCHICForm = "";
                        string siteKey = Session["siteKey"].ToString();

                        DateTime dataIncubacao = Convert.ToDateTime(item.Set_date);

                        CH_VARTABLTableAdapter vTA = new CH_VARTABLTableAdapter();
                        CHICOracleDataSet.CH_VARTABLDataTable vDT = new CHICOracleDataSet.CH_VARTABLDataTable();
                        vTA.FillByVarietyFLIP(vDT, siteKey, item.Variety);
                        if (vDT.Count > 0) varietyCHIC = vDT.FirstOrDefault().VARIETY;

                        CH_BOOKEDTableAdapter bTA = new CH_BOOKEDTableAdapter();
                        CHICOracleDataSet.CH_BOOKEDDataTable bDT = new CHICOracleDataSet.CH_BOOKEDDataTable();
                        CH_ITEMSTableAdapter iTA = new CH_ITEMSTableAdapter();
                        CHICOracleDataSet.CH_ITEMSDataTable iDT = new CHICOracleDataSet.CH_ITEMSDataTable();
                        iTA.Fill(iDT, siteKey);
                        bTA.FillVarietiesByOrderNoAndLocation(bDT, siteKey, item.OrderNoCHIC, 
                            (item.Hatch_Loc == "TB" ? "AJ" : item.Hatch_Loc));
                        CHICOracleDataSet.CH_ITEMSRow iRow = iDT.Where(w => bDT.Any(b => w.ITEM_NO == b.ITEM)
                            && w.VARIETY == varietyCHIC).FirstOrDefault();

                        if (iRow != null)
                        {
                            varietyCHICForm = iRow.FORM + "-" + iRow.VARIETY;
                            oTA.FillBySetDateAndHatchLocAndOrderNoAndFlockIDAndVarietyCHIC(oDT,
                                Convert.ToDateTime(item.Set_date), (item.Hatch_Loc == "TB" ? "AJ" : item.Hatch_Loc), item.OrderNoCHIC, item.Flock_id,
                                varietyCHICForm);
                        }

                        decimal qtdPedido = bDT.Sum(s => s.QUANTITY);

                        #endregion

                        #region Atualiza / Insere Dados nos Lotes do Pedido

                        //if (oDT.Count > 0)
                        //{
                        //    CHICOracleDataSet.ORDER_DATARow oRow = oDT.FirstOrDefault();
                        //    oRow.ORD_QTY = qtdPedido;
                        //    oRow.DEL_QTY = (decimal)item.Qtde;
                        //    oRow.NUM_1 = percErroSelecao;
                        //    oRow.NUM_2 = percQtdeVacinada;
                        //    oRow.NUM_4 = percUniformidade;
                        //    oRow.NUM_5 = pesoMedio;
                        //    oRow.NUM_9 = percErroSexagem;
                        //    oRow.NUM_10 = percErroContagem;
                        //    oRow.TEXT_1 = observacao;
                        //    oRow.TEXT_4 = horaProgramada;
                        //    oRow.TEXT_5 = horaReal;
                        //    oTA.Update(oRow);
                        //}
                        //else
                        //{

                        HATCHERY_ORDER_ROTULO_DATA rotulo = hlbapp.HATCHERY_ORDER_ROTULO_DATA
                            .Where(w => w.Hatch_Loc == pedido.Hatch_Loc && w.Set_date == pedido.Set_date
                                && w.OrderNoCHIC == pedido.OrderNoCHIC && w.Cor == item.Rotulo
                                && w.Variety == item.Variety)
                            .FirstOrDefault();

                        if (rotulo != null)
                            rotuloDescr = rotulo.Cor;
                        else
                            rotuloDescr = "";

                        if (rotulo != null)
                            percUniformidade = (decimal)rotulo.Uniformidade;

                        if (rotulo != null)
                            pesoMedio = (decimal)rotulo.PesoMedio;

                        oTA.Insert("HYBR", "BR", Session["location"].ToString(), dataIncubacao, hatchLoc, numPedido,
                            "", "", varietyCHICForm, qtdPedido, null, null, item.Flock_id, item.Qtde, custno,
                            percErroSelecao, observacao, null, null, percQtdeVacinada, null, percUniformidade,
                            pesoMedio, null, null, null, percErroSexagem, percErroContagem, horaProgramada,
                            horaReal, null, null, null, null, null, null, null, null, item.ClassOvo, pedido.SaidaReal,
                            rotuloDescr);
                        //}

                        #endregion
                    }
                }

                #region Deleta Vacinas do Pedido

                HATCHERY_VACC_DATATableAdapter vacTA = new HATCHERY_VACC_DATATableAdapter();
                vacTA.DeleteQuery(hatchLoc, setDate, numPedido);

                #endregion

                #region Vacinas do Pedido

                var listaVacinasPedido = hlbapp.HATCHERY_ORDER_VACC_DATA
                    .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                        && w.OrderNoCHIC == numPedido)
                    .GroupBy(g => new
                    {
                        g.Vacina,
                        g.Laboratorio,
                        g.Partida,
                        g.DataFabricacao,
                        g.DataValidade
                    })
                    .Select(s => new
                    {
                        s.Key.Vacina,
                        s.Key.Laboratorio,
                        s.Key.Partida,
                        s.Key.DataFabricacao,
                        s.Key.DataValidade
                    })
                    .ToList();

                CHICOracleDataSet.HATCHERY_VACC_DATADataTable vacDT =
                        new CHICOracleDataSet.HATCHERY_VACC_DATADataTable();
                vacTA.FillByHatchLocAndSetDateAndOrderNo(vacDT, hatchLoc, setDate, numPedido);

                foreach (var vacina in listaVacinasPedido)
                {
                    CHICOracleDataSet.HATCHERY_VACC_DATARow vacRow = vacDT.Where(w => w.VACCINE == vacina.Vacina
                        && w.SERIAL_NO == vacina.Partida && w.MANUFACTURER == vacina.Laboratorio)
                        .FirstOrDefault();

                    if (vacRow == null)
                    {
                        string nomeDoenca = "";
                        MvcAppHyLinedoBrasil.Models.FormulaPPCP.FormulaPPCPEntities apolo =
                            new MvcAppHyLinedoBrasil.Models.FormulaPPCP.FormulaPPCPEntities();
                        var produto = apolo.DESCR_TECN_PROD
                            .Where(w => apolo.PRODUTO.Any(a => w.ProdCodEstr == a.ProdCodEstr
                                    && a.ProdNomeAlt1 == vacina.Vacina)
                                && w.DescrTecnProdTitulo == "DOENCA")
                            .FirstOrDefault();
                        if (produto != null)
                        {
                            if (produto.DescrTecnProdTexto.Length > 50)
                                nomeDoenca = produto.DescrTecnProdTexto.Substring(0, 50);
                            else
                                nomeDoenca = produto.DescrTecnProdTexto;
                        }

                        string nomeVacina = "";
                        if (vacina.Vacina.Length > 25)
                            nomeVacina = vacina.Vacina.Substring(0, 25);
                        else
                            nomeVacina = vacina.Vacina;
                        vacTA.Insert("HYBR", "BR", Session["location"].ToString(), setDate, hatchLoc, custno,
                            vacina.DataFabricacao, vacina.DataValidade, nomeVacina, vacina.Laboratorio,
                            vacina.Partida, null, null, null, null, null, null, numPedido, "", "", "", nomeDoenca);
                    }
                }

                #endregion
            }
        }

        public void AtualizaFLIP()
        {
            string incubatorio = ddlIncubatorios.SelectedValue;
            DateTime dateStartCH = Convert.ToDateTime("25/11/2019");
            DateTime dateStartTB = Convert.ToDateTime("07/01/2020");
            DateTime setDate = Calendar1.SelectedDate;
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var listaPedidos = hlbapp.HATCHERY_ORDER_DATA
                .Where(w => 
                    //w.Hatch_Loc == "TB" && w.Set_date >= dateStartTB
                    w.Hatch_Loc == incubatorio && w.Set_date == setDate
                    //w.Variety == "H&amp;N"
                    //&& w.OrderNoCHIC == "93667"
                )
            .ToList();

            //var listaPedidos = hlbapp.HATCHERY_ORDER_DATA.ToList();

            foreach (var pedido in listaPedidos)
            {
                if (pedido.Hatch_Loc == "NM" || 
                    (pedido.Hatch_Loc == "CH" && pedido.Set_date >= dateStartCH) || // Data de início de CH para rastreabilidade WEB
                    (pedido.Hatch_Loc == "TB" && pedido.Set_date >= dateStartTB)) // Data de início de T para rastreabilidade WEB
                {
                    setDate = Convert.ToDateTime(pedido.Set_date);
                    AtualizaFLIPSetDateHatchLoc(pedido.Hatch_Loc, setDate, pedido.OrderNoCHIC);
                }
            }
        }

        public void RefreshFLIPManual()
        {
            string incubatorio = "CH";
            DateTime dateStartCH = Convert.ToDateTime("25/11/2019");
            DateTime startSetDate = Convert.ToDateTime("25/11/2019");
            DateTime finishSetDate = Convert.ToDateTime("09/01/2020");
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var listaPedidos = hlbapp.HATCHERY_ORDER_DATA.Where(w =>
                //w.Hatch_Loc == "NM"
                w.Hatch_Loc == incubatorio
                && w.Set_date >= startSetDate && w.Set_date <= finishSetDate
                //&& w.OrderNoCHIC == "69426"
                ).ToList();

            //var listaPedidos = hlbapp.HATCHERY_ORDER_DATA.ToList();

            foreach (var pedido in listaPedidos)
            {
                if (pedido.Hatch_Loc == "NM" ||
                    (pedido.Hatch_Loc == "CH" && pedido.Set_date >= dateStartCH)) // Data de início de CH para rastreabilidade WEB
                {
                    DateTime setDate = Convert.ToDateTime(pedido.Set_date);
                    AtualizaFLIPSetDateHatchLoc(pedido.Hatch_Loc, setDate, pedido.OrderNoCHIC);
                }
            }
        }

        public string GeraNumeroRNC()
        {
            ApoloEntities apolo = new ApoloEntities();
            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));
            apolo.GerarCodigo("1", "USER_RNC_WEB", numero);

            return "2017" + Convert.ToInt32(numero.Value).ToString("D4");
        }

        protected void ddlRNC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlRNC.SelectedValue.Equals("(Nenhum)"))
            {
                lblNumeroRNC.Visible = true;
                txtNumeroRNC.Visible = true;
                if (txtNumeroRNC.Text == "")
                    txtNumeroRNC.Text = GeraNumeroRNC();

                if (Calendar1.SelectedDate >= Convert.ToDateTime("01/01/2017"))
                    txtNumeroRNC.Enabled = false;
                else
                    txtNumeroRNC.Enabled = true;

            }
            else
            {
                lblNumeroRNC.Visible = false;
                txtNumeroRNC.Visible = false;
            }
        }

        public int carregaQtdePedido(string numPedido, string linhagem, string incubatorio)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaVendido = hlbapp.VU_Pedidos_Vendas_CHIC_Matrizes
                .Where(w => w.CHICNum == numPedido
                    && w.LinhagemFLIP == linhagem
                    && w.LocalNascimento == incubatorio)
                .ToList();
            var listaReposicao = hlbapp.VU_Pedidos_Vendas_CHIC_Matrizes
                .Where(w => w.CHICNumReposicao == numPedido
                    && w.LinhagemFLIP == linhagem
                    && w.LocalNascimento == incubatorio)
                .ToList();

            var qtdeVendida = 0;
            if (listaVendido.Count > 0) qtdeVendida = listaVendido.Sum(s => s.QtdeVendida + s.QtdeBonificada);

            var qtdeReposicao = 0;
            if (listaReposicao.Count > 0) qtdeReposicao = listaReposicao.Sum(s => s.QtdeReposicao);

            return qtdeVendida + qtdeReposicao;
        }
    }
}