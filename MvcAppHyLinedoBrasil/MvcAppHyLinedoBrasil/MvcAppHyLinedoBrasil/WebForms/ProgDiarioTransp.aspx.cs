using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Models.HLBAPP;
using MvcAppHyLinedoBrasil.Data.CHICDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data.CHICParentDataSetTableAdapters;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Models;
using System.Globalization;
using System.Drawing;
using System.Net;
using System.IO;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using ImportaCHICService.Embarcador;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Collections;
using System.Data.Objects;
using System.Timers;
//using MvcAppHyLinedoBrasil.br.com.hyline.fluigteste;
using MvcAppHyLinedoBrasil.br.com.hyline.fluig;
using MvcAppHyLinedoBrasil.br.com.transportesbra.dev;
using System.Linq.Expressions;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
//using ImportaCHICService.Data;
using DocumentFormat.OpenXml.Office2013.Excel;
using Microsoft.Office.Interop.PowerPoint;
//using CIDADE = MvcAppHyLinedoBrasil.Models.CIDADE;
//using Prog_Diaria_Transp_Pedidos = MvcAppHyLinedoBrasil.Models.HLBAPP.Prog_Diaria_Transp_Pedidos;
//using ENTIDADE = MvcAppHyLinedoBrasil.Models.ENTIDADE;
//using ENT_FONE = MvcAppHyLinedoBrasil.Models.ENT_FONE;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class ProgDiarioTransp : System.Web.UI.Page
    {
        #region Timer

        private static System.Timers.Timer _oTimerHora;

        public static void IniciaTimer()
        {
            _oTimerHora = new System.Timers.Timer(3600 * 1000); // Hora
            //_oTimerHora = new System.Timers.Timer(60 * 1000); // Teste Minuto
            _oTimerHora.Elapsed += Atualizacao_Tick;
            _oTimerHora.Start();
        }

        #endregion

        #region Carrega Entitys

        HLBAPPEntities1 hlbapp = new HLBAPPEntities1();
        FinanceiroEntities apolo = new FinanceiroEntities();

        #endregion

        #region DropDownList

        public void CarregaListaTransportadora()
        {
            #region Método Antigo

            //string empresa = Session["empresa"].ToString();
            //if (empresa.Length > 0)
            //{
            //    if (empresa.Contains("BR") || empresa.Contains("LB")
            //        || empresa.Contains("HN"))
            //    {
            //        ListItem item = new ListItem();
            //        item.Text = "Transema";
            //        item.Value = "TR";
            //        ddlEmpresaTransportadora.Items.Add(item);
            //    }

            //    if (empresa.Contains("PL"))
            //    {
            //        ListItem item = new ListItem();
            //        item.Text = "Planalto";
            //        item.Value = "PL";
            //        ddlEmpresaTransportadora.Items.Add(item);
            //    }

            //    if (empresa.Contains("HN"))
            //    {
            //        ListItem item = new ListItem();
            //        item.Text = "H&N";
            //        item.Value = "HN";
            //        ddlEmpresaTransportadora.Items.Add(item);
            //    }
            //}

            #endregion

            #region Método Novo

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspTransema",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "Transema";
                item.Value = "TR";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspPlanalto",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "Planalto";
                item.Value = "PL";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspH&N",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "H&N";
                item.Value = "HN";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspExportacao",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "Exportação";
                item.Value = "EX";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspAlojInterno",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "Alojamento Interno";
                item.Value = "AI";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoTranspTransfOvos",
                    (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListItem item = new ListItem();
                item.Text = "Transferência de Ovos";
                item.Value = "TO";
                ddlEmpresaTransportadora.Items.Add(item);
            }

            #endregion
        }

        #endregion

        #region Atualiza Página

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

            //ImportaPlanilhaCheckListClassificadoraOvosFluig();

            DateTime data = DateTime.Today;

            if (!IsPostBack)
            {
                //InserePedidoEmbarcador(1657462);

                //IntegraTargetCIOTPadraoEValePedagioSemParar(46838);
                //IntegraTargetCIOTPadraoEValePedagioSemParar(46872);
                //var retorno = IntegraTargetCIOTPadraoEValePedagioSemParar(18247);
                //var retorno = EncerrarOperacaoTransporte(18247);
                //var retorno = ValePedagioAvulsoSemParar(18246);

                #region Se os horários de origem inicial e final estiverem vazio, chama o método "retornaVeiculoPassagens"

                //DateTime dataProgramacao = Convert.ToDateTime("13/11/2020");
                //XDocument xmlRetornoVeiculoPassagens = Embarcador.retornaVeiculoPassagens("OVT1990", dataProgramacao);

                //#region Verifica XML OK

                //DateTime dataHoraInicioCarregamento = new DateTime();
                //DateTime dataHoraFimCarregamento = new DateTime();

                //foreach (XElement retorno in xmlRetornoVeiculoPassagens.Descendants("return"))
                //{
                //    var listaItens = retorno.Nodes();

                //    foreach (XElement item in listaItens)
                //    {
                //        var listaSubItens = item.Nodes()
                //            .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                //        XElement tipoUnidadeObj = (XElement)listaSubItens[2];

                //        if (tipoUnidadeObj.Value == "INCUBATORIO")
                //        {
                //            XElement chegadaObj = (XElement)listaSubItens[3];
                //            DateTime dataChegada = Convert.ToDateTime(chegadaObj.Value);

                //            XElement saidaObj = (XElement)listaSubItens[4];
                //            DateTime dataSaida = Convert.ToDateTime(saidaObj.Value);

                //            if (dataHoraInicioCarregamento == Convert.ToDateTime("01/01/0001")
                //                || dataHoraInicioCarregamento > dataChegada)
                //                dataHoraInicioCarregamento = dataChegada;

                //            if (dataHoraFimCarregamento == Convert.ToDateTime("01/01/0001")
                //                || dataHoraFimCarregamento < dataSaida)
                //                dataHoraFimCarregamento = dataSaida;
                //        }
                //    }
                //}

                //#endregion

                #endregion

                imgLogo.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                Session["qtdLinhas"] = 1;
                Session["destinoRelExcel"] = "";

                if (Session["modoImpressao"] == null)
                    Session["modoImpressao"] = false;

                if (Session["modoExcel"] == null)
                    Session["modoExcel"] = false;

                if ((Convert.ToBoolean(Session["modoImpressao"]))
                    || (Convert.ToBoolean(Session["modoExcel"])))
                    ModoImpressao();
                else
                {
                    ModoCompleto();
                    ValidaDireitos();
                }

                if (Session["dataSelecionada"] == null)
                {
                    txtDataProgramacao.Text = DateTime.Today.ToShortDateString();
                    Session["dataSelecionada"] = DateTime.Today.ToShortDateString();
                }
                else
                {
                    txtDataProgramacao.Text = Convert.ToDateTime(Session["dataSelecionada"]).ToShortDateString();
                }

                //Calendar1.SelectedDate = DateTime.Today;

                data = Convert.ToDateTime(txtDataProgramacao.Text);

                var culture = new System.Globalization.CultureInfo("pt-BR");
                string day = culture.DateTimeFormat.GetDayName(data.DayOfWeek);

                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                lblDataNasc1.Text = data.ToShortDateString() + " - " + textInfo.ToTitleCase(day);

                CarregaListaTransportadora();
            }

            //List<string> listaPedidos = new List<string>();
            //listaPedidos.Add("94380");
            ////listaPedidos.Add("92988");
            ////////listaPedidos.Add("87477");
            ////////listaPedidos.Add("88735");
            //DateTime dataNascTeste = Convert.ToDateTime("05/05/2020");
            //IntegraRoteiroEntregaFluig("PL", "PL", dataNascTeste, listaPedidos);

            //GeraRoteirosEntregaFluig();

            //AtualizaDadosViagem();

            //RetornaDadosViagem(47088);

            //ImportaCargasEmbarcador();

            //GeraManutencaoPreventivaIncNMConfigApoloFluig();

            //DateTime dataEmbarque = Convert.ToDateTime("05/05/2020");
            //IntegraPedidoEmbarcador(8960472, dataEmbarque);

            //if (DateTime.Today == Convert.ToDateTime(txtDataProgramacao.Text))
            //    ImportaCargasEmbarcador();

            //GeraPDI(2020);

            ValidaExibicaoTransportadora();
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Convert.ToBoolean(Session["modoImpressao"]))
                {
                    Session["modoImpressao"] = false;
                    HttpContext.Current.Response
                        .Write("<script>window.print();setTimeout(function(){window.close();}, 1);</script>");
                }

                if (Convert.ToBoolean(Session["modoExcel"]))
                {
                    Session["modoExcel"] = false;
                    lkbDownload.Visible = true;
                    ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                    scriptManager.RegisterPostBackControl(this.lkbDownload);
                }
            }
        }

        protected void txtDataProgramacao_TextChanged(object sender, EventArgs e)
        {
            DateTime data = Convert.ToDateTime(txtDataProgramacao.Text);

            lblMensagem2.Visible = false;
            var culture = new System.Globalization.CultureInfo("pt-BR");
            string day = culture.DateTimeFormat.GetDayName(data.DayOfWeek);

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            lblDataNasc1.Text = txtDataProgramacao.Text
                + " - " + textInfo.ToTitleCase(day);

            Session["dataSelecionada"] = data;
        }

        #endregion

        #region Tabela de Pedidos

        #region GridView1 Methods

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            lblMensagem2.Visible = false;
            GridView1.EditIndex = e.NewEditIndex;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            #region Esconder Campo Qtde Caixas p/ Transema

            #region ItemTemplate

            Label lblID = (Label)e.Row.FindControl("Label9");
            if (lblID != null)
            {
                if (ddlEmpresaTransportadora.SelectedValue.Equals("TR"))
                {
                    GridView1.HeaderRow.Cells[7].Visible = false;
                    e.Row.Cells[7].Visible = false;
                }
                else
                {
                    GridView1.HeaderRow.Cells[7].Visible = true;
                    e.Row.Cells[7].Visible = true;
                }
            }

            #endregion

            #region EditTemplate

            TextBox txtQtdeCaixa = (TextBox)e.Row.FindControl("txtQuantidadeCaixa");
            if (txtQtdeCaixa != null)
            {
                if (ddlEmpresaTransportadora.SelectedValue.Equals("TR"))
                {
                    GridView1.HeaderRow.Cells[7].Visible = false;
                    e.Row.Cells[7].Visible = false;
                }
                else
                {
                    GridView1.HeaderRow.Cells[7].Visible = true;
                    e.Row.Cells[7].Visible = true;
                }
            }

            #endregion

            #endregion

            #region Cor no Incubatório / Botões Editar e Inserir

            if (lblID != null)
            {
                int id = Convert.ToInt32(lblID.Text);

                Models.HLBAPP.Prog_Diaria_Transp_Pedidos pedido = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.ID == id).FirstOrDefault();

                if (pedido != null)
                {
                    Label localNascimento = (Label)e.Row.FindControl("Label15");
                    if (localNascimento.Text.Equals("AJ"))
                    {
                        localNascimento.ForeColor = System.Drawing.Color.Red;
                        localNascimento.Font.Bold = true;
                        e.Row.Cells[15].BackColor = System.Drawing.Color.Yellow;
                    }

                    //if (pedido.CHICNum != "" && pedido.CHICNum != null && pedido.CHICNumReposicao != "" && pedido.CHICNumReposicao != null)
                    //{
                    ImageButton imgDeletePD = (ImageButton)e.Row.FindControl("imgDelete");
                    imgDeletePD.Visible = false;
                    //}

                    if (!MvcAppHyLinedoBrasil.Controllers.AccountController
                            .GetGroup("HLBAPP-ProgDiariaTranspAlterarDadosPedidos",
                            (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        ImageButton imgbEditItem = (ImageButton)e.Row.FindControl("imgbEditItem");
                        imgbEditItem.Visible = false;
                        ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
                        imgDelete.Visible = false;
                    }
                }
            }

            #endregion

            #region Cor no Status

            Label lblStatus = (Label)e.Row.FindControl("Label16");
            if (lblStatus != null)
            {
                if (lblStatus.Text.Equals("Pendente"))
                    e.Row.Cells[26].BackColor = System.Drawing.Color.Red;
                else if (lblStatus.Text.Equals("Preenchido"))
                    e.Row.Cells[26].BackColor = System.Drawing.Color.Yellow;
                else if (lblStatus.Text.Equals("Conferido"))
                    e.Row.Cells[26].BackColor = System.Drawing.Color.Green;
            }

            #endregion

            #region Modo Impressão

            if (e.Row.Cells.Count == 25)
            {
                if ((Convert.ToBoolean(Session["modoImpressao"]))
                    || (Convert.ToBoolean(Session["modoExcel"])))
                {
                    e.Row.Cells[0].Visible = false;
                    e.Row.Cells[1].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    e.Row.Cells[21].Visible = false;
                    e.Row.Cells[22].Visible = false;
                    e.Row.Cells[23].Visible = false;
                    e.Row.Cells[24].Visible = false;
                    e.Row.Cells[25].Visible = false;

                    #region Inserir Linhas entre Caminhões

                    Label lblNCaminhao = (Label)e.Row.FindControl("Label1");
                    if (lblNCaminhao != null)
                    {
                        if (e.Row.RowIndex >= 1)
                        {
                            int indexAnterior = e.Row.RowIndex - 1;
                            string caminhaoAnterior =
                                ((Label)GridView1.Rows[indexAnterior].Cells[4].FindControl("Label1")).Text;
                            if (!lblNCaminhao.Text.Equals(caminhaoAnterior))
                            //if (lblNCaminhao.Text.Equals(caminhaoAnterior))
                            {
                                GridViewRow row = new GridViewRow(indexAnterior, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
                                row.BackColor = ColorTranslator.FromHtml("#F9F9F9");
                                //row.Cells.AddRange(new TableCell[3] { new TableCell (), //Empty Cell
                                //new TableCell { Text = "-", HorizontalAlign = HorizontalAlign.Right},
                                //new TableCell { Text = "-", HorizontalAlign = HorizontalAlign.Right } });
                                row.Cells.AddRange(new TableCell[1] { new TableCell { Height = 20, ColumnSpan = 25 } });
                                int indexInsertLinha = e.Row.RowIndex + Convert.ToInt32(Session["qtdLinhas"]);
                                GridView1.Controls[0].Controls.AddAt(indexInsertLinha, row);
                                Session["qtdLinhas"] = Convert.ToInt32(Session["qtdLinhas"]) + 1;
                                //int indexAnteriorBranco = e.Row.RowIndex - 1;
                                //GridViewRow row = GridView1.Rows[indexAnteriorBranco];
                                //GridView1.Controls[0].Controls.Remove(row);
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    e.Row.Cells[0].Visible = true;
                    e.Row.Cells[1].Visible = true;
                    e.Row.Cells[5].Visible = true;
                    e.Row.Cells[21].Visible = true;
                    e.Row.Cells[22].Visible = true;
                    e.Row.Cells[23].Visible = true;
                    e.Row.Cells[24].Visible = true;
                    e.Row.Cells[25].Visible = true;
                }
            }

            #endregion

            #region Esconde Linha Em Branco

            if (lblID != null)
                if (lblID.Text.Equals("2933932"))
                {
                    lblID.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgbEditItem");
                    imgbEdit.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbCancelEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("imgDelete");
                    imgbCancelEdit.Visible = false;
                }

            #endregion
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label txtID = (Label)GridView1.Rows[GridView1.SelectedIndex].FindControl("Label9");
            int id = Convert.ToInt32(txtID.Text);

            Models.HLBAPP.Prog_Diaria_Transp_Pedidos pedido = hlbapp.Prog_Diaria_Transp_Pedidos
                .Where(w => w.ID == id).FirstOrDefault();

            hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
            hlbapp.SaveChanges();

            GridView1.SelectedIndex = -1;
            GridView1.DataBind();

            lblMensagem2.Visible = true;
            lblMensagem2.Text = "Pedido de " + pedido.NomeCliente + " de " + pedido.Quantidade.ToString()
                + " excluído com sucesso!";
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            #region Inserir Linhas entre Caminhões

            //foreach (GridViewRow item in GridView1.Rows)
            //{
            //    Label lblNCaminhao = (Label)item.FindControl("Label1");
            //    if (lblNCaminhao != null)
            //    {
            //        if (item.RowIndex >= 2)
            //        {
            //            int indexProximo = item.RowIndex + 1;

            //            if (indexProximo < GridView1.Rows.Count)
            //            {
            //                Label lblNCaminhaoProximo =
            //                    (Label)GridView1.Rows[indexProximo].Cells[3].FindControl("Label1");

            //                if (lblNCaminhaoProximo != null)
            //                {
            //                    string caminhaoAnterior = lblNCaminhaoProximo.Text;
            //                    if (!lblNCaminhao.Text.Equals(caminhaoAnterior))
            //                    {
            //                        GridViewRow row = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Normal);
            //                        row.BackColor = ColorTranslator.FromHtml("#F9F9F9");
            //                        //row.Cells.AddRange(new TableCell[3] { new TableCell (), //Empty Cell
            //                        //new TableCell { Text = "-", HorizontalAlign = HorizontalAlign.Right},
            //                        //new TableCell { Text = "-", HorizontalAlign = HorizontalAlign.Right } });
            //                        row.Cells.AddRange(new TableCell[1] { new TableCell { Height = 20, ColumnSpan = 25 } });
            //                        GridView1.Controls[0].Controls.Add(row);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        #endregion

        #region CRUD Methods

        protected void imgbSaveItem_Click(object sender, ImageClickEventArgs e)
        {
            int index = GridView1.EditIndex;
            int id = 0;
            try
            {
                #region Pega Dados dos Controles

                Label lblID = (Label)GridView1.Rows[index].FindControl("Label1");
                id = Convert.ToInt32(lblID.Text);

                DropDownList ddlNumVeiculo = (DropDownList)GridView1.Rows[index].FindControl("ddlNumVeiculo");
                int numVeiculo = Convert.ToInt32(ddlNumVeiculo.SelectedValue);

                TextBox txtOrdem = (TextBox)GridView1.Rows[index].FindControl("txtOrdem");
                int ordem = 0;
                if (txtOrdem.Text != "") ordem = Convert.ToInt32(txtOrdem.Text);

                DropDownList ddlEmbalagem = (DropDownList)GridView1.Rows[index].FindControl("ddlEmbalagem");
                string embalagem = ddlEmbalagem.SelectedValue;

                TextBox txtInicioCarregEsperado = (TextBox)GridView1.Rows[index].FindControl("txtInicioCarregEsperado");
                string inicioCarregEsperado = txtInicioCarregEsperado.Text;

                TextBox txtChegadaClienteEsperado = (TextBox)GridView1.Rows[index].FindControl("txtChegadaClienteEsperado");
                string chegadaClienteEsperado = txtChegadaClienteEsperado.Text;

                int KM = 0;
                TextBox txtKM = (TextBox)GridView1.Rows[index].FindControl("txtKM");
                if (txtKM.Text != "") KM = Convert.ToInt32(txtKM.Text);

                int qtdeCaixa = 0;
                TextBox txtQuantidadeCaixa = (TextBox)GridView1.Rows[index].FindControl("txtQuantidadeCaixa");
                if (txtQuantidadeCaixa.Text != "") qtdeCaixa = Convert.ToInt32(txtQuantidadeCaixa.Text);

                TextBox txtInicioCarregamentoReal = (TextBox)GridView1.Rows[index].FindControl("txtInicioCarregamentoReal");
                string inicioCarregamentoReal = txtInicioCarregamentoReal.Text;

                TextBox txtChegadaClienteReal = (TextBox)GridView1.Rows[index].FindControl("txtChegadaClienteReal");
                string chegadaClienteReal = txtChegadaClienteReal.Text;

                TextBox txtObs = (TextBox)GridView1.Rows[index].FindControl("txtObs");
                string obs = txtObs.Text;

                #endregion

                #region Atualiza Pedido

                Models.HLBAPP.Prog_Diaria_Transp_Pedidos pedido = hlbapp.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.ID == id).FirstOrDefault();

                Models.HLBAPP.Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.DataProgramacao == pedido.DataProgramacao 
                        && w.NumVeiculo == numVeiculo
                        && w.EmpresaTranportador == pedido.EmpresaTranportador).FirstOrDefault();

                pedido.NumVeiculo = numVeiculo;
                pedido.Ordem = ordem;
                pedido.Embalagem = embalagem;
                pedido.InicioCarregamentoEsperado = inicioCarregEsperado;
                if (pedido.ChegadaClienteEsperado != chegadaClienteEsperado)
                    GeraLOGIntegracaoEmbarcador(progVeiculo.ID, "Alteração Manual no Web", 0, "Hora Chegada Cliente: " + chegadaClienteEsperado);
                pedido.ChegadaClienteEsperado = chegadaClienteEsperado;
                pedido.KM = KM;
                pedido.InicioCarregamentoReal = inicioCarregamentoReal;
                pedido.ChegadaClienteReal = chegadaClienteReal;
                pedido.Observacao = obs;
                pedido.QuantidadeCaixa = qtdeCaixa;

                int qtdPintosCaixa = 0;
                if (progVeiculo != null)
                    qtdPintosCaixa = Convert.ToInt32(progVeiculo.QuantidadePorCaixa);

                if (pedido.NumVeiculo != 0 && pedido.Embalagem != "" && qtdPintosCaixa > 0)
                    pedido.Status = "Preenchido";
                else
                    pedido.Status = "Pendente";

                hlbapp.SaveChanges();

                #endregion

                #region Integra com o Embarcador - DESATIVADO

                //string retornoEmbarcador = IntegraPedidoEmbarcador(pedido.ID, Convert.ToDateTime(txtDataProgramacao.Text));
                //if (retornoEmbarcador != "")
                //{
                //    lblMensagem2.Visible = true;
                //    lblMensagem2.Text = retornoEmbarcador;
                //}
                //else
                //{
                //    lblMensagem2.Visible = false;
                //    lblMensagem2.Text = "";
                //}

                #endregion

                DateTime data = Convert.ToDateTime(pedido.DataProgramacao);
                AtualizaValoresVeiculos(data, "PDT");

                GridView1.Rows[index].RowState = DataControlRowState.Normal;
                GridView1.EditIndex = -1;
                GridView1.SelectedIndex = index;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                GridView1.Rows[index].RowState = DataControlRowState.Normal;
                lblMensagem3.Visible = true;

                if (ex.Message.Length >= 35)
                {
                    if (ex.Message.Substring(0, 35) == "ORA-20102: Cannot update records!!!")
                    {
                        lblMensagem3.Text = "Erro na linha " + (id).ToString() + ": " + "Não existe esse Lote nesta Data de Produção no Estoque informada! Verifique!";
                    }
                    else
                    {
                        lblMensagem3.Text = "Erro na linha " + (id).ToString() + ": " + ex.Message;
                    }
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            TextBox txtCliente = (TextBox)GridView1.FooterRow.FindControl("txtCliente");
            DropDownList ddlNumVeiculo = (DropDownList)GridView1.FooterRow.FindControl("ddlNumVeiculo");
            TextBox txtQtde = (TextBox)GridView1.FooterRow.FindControl("txtQtde");
            TextBox txtLocalEntrega = (TextBox)GridView1.FooterRow.FindControl("txtLocalEntrega");
            TextBox txtLinhagem = (TextBox)GridView1.FooterRow.FindControl("txtLinhagem");
            DropDownList ddlEmbalagem = (DropDownList)GridView1.FooterRow.FindControl("ddlEmbalagem");
            TextBox txtInicioCarregEsperado = (TextBox)GridView1.FooterRow.FindControl("txtInicioCarregEsperado");
            TextBox txtDataEntrega = (TextBox)GridView1.FooterRow.FindControl("txtDataEntrega");
            TextBox txtChegadaClienteEsperado = (TextBox)GridView1.FooterRow.FindControl("txtChegadaClienteEsperado");
            TextBox txtKM = (TextBox)GridView1.FooterRow.FindControl("txtKM");
            TextBox txtInicioCarregamentoReal = (TextBox)GridView1.FooterRow.FindControl("txtInicioCarregamentoReal");
            TextBox txtChegadaClienteReal = (TextBox)GridView1.FooterRow.FindControl("txtChegadaClienteReal");
            TextBox txtObs = (TextBox)GridView1.FooterRow.FindControl("txtObs");

            Models.HLBAPP.Prog_Diaria_Transp_Pedidos pedido = new Models.HLBAPP.Prog_Diaria_Transp_Pedidos();
            pedido.DataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);
            pedido.NomeCliente = txtCliente.Text;
            pedido.NumVeiculo = Convert.ToInt32(ddlNumVeiculo.SelectedValue);
            if (txtQtde.Text != "") pedido.Quantidade = Convert.ToInt32(txtQtde.Text);
            pedido.LocalEntrega = txtLocalEntrega.Text;
            pedido.Linhagem = txtLinhagem.Text;
            pedido.Embalagem = ddlEmbalagem.SelectedValue;
            pedido.InicioCarregamentoEsperado = txtInicioCarregEsperado.Text;
            if (txtDataEntrega.Text != "" && txtDataEntrega.Text != "  /  /    ") pedido.DataEntrega = Convert.ToDateTime(txtDataEntrega.Text);
            pedido.ChegadaClienteEsperado = txtChegadaClienteEsperado.Text;
            if (txtKM.Text != "") pedido.KM = Convert.ToInt32(txtKM.Text);
            pedido.InicioCarregamentoReal = txtInicioCarregamentoReal.Text;
            pedido.ChegadaClienteReal = txtChegadaClienteReal.Text;
            pedido.Observacao = txtObs.Text;
            pedido.EmpresaTranportador = ddlEmpresaTransportadora.SelectedValue;
            pedido.Empresa = "BR";

            hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(pedido);
            hlbapp.SaveChanges();

            GridView1.DataBind();

            foreach (GridViewRow item in GridView1.Rows)
            {
                Label id = (Label)item.FindControl("Label9");
                if (id.Text == pedido.ID.ToString())
                    GridView1.SelectedIndex = item.RowIndex;
            }

            DateTime data = Convert.ToDateTime(pedido.DataProgramacao);
            AtualizaValoresVeiculos(data, "PDT");

            GridView1.ShowFooter = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            GridView1.ShowFooter = false;
        }

        protected void imgbEditItem_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void imgbAdd_Click(object sender, ImageClickEventArgs e)
        {
            GridView1.ShowFooter = true;
        }

        #endregion

        #endregion

        #region Event Methods

        #region CHIC Refresh

        protected void imgbRefresh_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(txtDataProgramacao.Text) >= DateTime.Today
                     ||
                    MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetGroup("HLBAPP-ProgDiariaTranspAtualizaCHICFaturado",
                        (System.Collections.ArrayList)Session["Direitos"]))
                {
                    string retorno = AtualizaPedidosCHICProgDiariaTransp(Convert.ToDateTime(txtDataProgramacao.Text), ddlEmpresaTransportadora.SelectedValue, "PDT");

                    if (retorno != "")
                    {
                        lblMensagem2.Visible = true;
                        lblMensagem2.Text = "Erro ao importar dados do CHIC para a Programação WEB: " + retorno;
                        return;
                    }

                    GridView1.DataBind();
                    ListView1.DataBind();

                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "Dados Atualizados conforme CHIC!";
                }
                else
                {
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "Dados não podem ser atualizados porque os Pedidos já foram enviados!";
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                lblMensagem2.Visible = true;
                if (ex.InnerException == null)
                    lblMensagem2.Text = ex.Message;
                else
                    lblMensagem2.Text = ex.Message + " / " + ex.InnerException.Message;
            }
        }

        public void AtualizaValoresVeiculos(DateTime data, string origem)
        {
            #region Atualiza Veículos da Programação Diária de Transporte

            var listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                .Where(w => w.DataProgramacao == data).ToList();

            #region Atualização Transema

            for (int i = 0; i <= 10; i++)
            {
                Models.HLBAPP.Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i
                        && w.EmpresaTranportador == "TR").FirstOrDefault();

                bool existe = true;

                if (progVeiculo == null)
                {
                    existe = false;
                    progVeiculo = new Models.HLBAPP.Prog_Diaria_Transp_Veiculos();
                    progVeiculo.DataEmbarque = data;
                }

                progVeiculo.DataProgramacao = data;
                progVeiculo.NumVeiculo = i;
                progVeiculo.QuantidadeTotal = listaPedidos
                    .Where(w => w.NumVeiculo == i && w.EmpresaTranportador == "TR").Sum(s => s.Quantidade);
                if (progVeiculo.QuantidadePorCaixa != null)
                {
                    if (progVeiculo.QuantidadePorCaixa > 0)
                    {
                        decimal qtdCaixaDecimal = Convert.ToDecimal(progVeiculo.QuantidadeTotal) /
                            Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);
                        int qtdCaixaInt = Convert.ToInt32(progVeiculo.QuantidadeTotal) /
                            Convert.ToInt32(progVeiculo.QuantidadePorCaixa);

                        if ((qtdCaixaDecimal - qtdCaixaInt) > 0)
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt + 1;
                        else
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt;
                    }
                }
                else
                    progVeiculo.QuantidadePorCaixa = 100;

                progVeiculo.EmpresaTranportador = "TR";
                progVeiculo.EntCod = "0000807";

                //progVeiculo.ValorTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.ValorTotal);

                if (!existe) hlbapp.Prog_Diaria_Transp_Veiculos.AddObject(progVeiculo);

                List<Models.HLBAPP.Prog_Diaria_Transp_Pedidos> listPedidosVeiculos = hlbapp.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i
                        && w.EmpresaTranportador == "TR"
                        && w.Status == "Pendente").ToList();

                foreach (var item in listPedidosVeiculos)
                {
                    if (item.NumVeiculo != 0 && item.Embalagem != "" && progVeiculo.QuantidadePorCaixa > 0)
                        item.Status = "Preenchido";
                }
            }

            #endregion

            #region Atualização Outras

            var listaVeiculosPlanalto = hlbapp.Prog_Diaria_Transp_Veiculos
                .Where(w => w.EmpresaTranportador != "TR"
                    && w.DataProgramacao == data)
                .ToList();

            foreach (var item in listaVeiculosPlanalto)
            {
                var listaPedidosVeiculo = listaPedidos
                    .Where(w => w.NumVeiculo == item.NumVeiculo
                        && w.EmpresaTranportador.Equals(item.EmpresaTranportador)).ToList();

                item.QuantidadeTotal = listaPedidosVeiculo.Sum(s => s.Quantidade);
                item.QunatidadeCaixa = listaPedidosVeiculo.Sum(s => s.QuantidadeCaixa);
                item.QunatidadeCaixa = (item.QunatidadeCaixa == 0 ? 1 : item.QunatidadeCaixa);
                item.QuantidadePorCaixa = item.QuantidadeTotal / item.QunatidadeCaixa;
                decimal calculoQtdeCaixa = Convert.ToDecimal(item.QuantidadeTotal / (item.QunatidadeCaixa * 1.00m));
                if ((calculoQtdeCaixa - item.QuantidadePorCaixa) > 0) 
                    item.QuantidadePorCaixa = item.QuantidadePorCaixa + 1;
                item.ValorTotal = listaPedidosVeiculo.Sum(s => s.KM) * item.ValorKM;
            }

            #endregion

            hlbapp.SaveChanges();

            #endregion

            if (origem == "PDT")
            {
                //ListView1.DataBind();
                GridView1.DataBind();
                gdvVeiculos.DataBind();
            }
        }

        public string AtualizaPedidosCHICProgDiariaTransp(DateTime data, string empresaTransportadora, string origem)
        {
            string retorno = "";
            try
            {
                if (data >= DateTime.Today ||
                    MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetGroup("HLBAPP-ProgDiariaTranspAtualizaCHICFaturado",
                        (System.Collections.ArrayList)Session["Direitos"]))
                {
                    //string empresaTransportadora = ddlEmpresaTransportadora.SelectedValue;

                    #region Insere Pedidos da Programação Diária de Transporte - CHIC Matrizes

                    ImportaCHICService.Data.HLBAPPServiceEntities hlbappService = new ImportaCHICService.Data.HLBAPPServiceEntities();
                    hlbappService.CommandTimeout = 1000;

                    ordersTableAdapter oTA = new ordersTableAdapter();
                    Data.CHICDataSet.ordersDataTable oDT = new Data.CHICDataSet.ordersDataTable();

                    itemsTableAdapter iTA = new itemsTableAdapter();
                    Data.CHICDataSet.itemsDataTable iDT = new Data.CHICDataSet.itemsDataTable();
                    iTA.Fill(iDT);

                    #region Deleta Pedidos não Existentes

                    var listaPedidosData = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.DataProgramacao == data
                            //&& w.CHICNum == "74040"
                            && ((w.EmpresaTranportador == empresaTransportadora || empresaTransportadora == "")
                                && w.EmpresaTranportador != "AI")
                            && w.CHICOrigem == "Matriz"
                            ).ToList();

                    foreach (var item in listaPedidosData)
                    {
                        if (item.CHICNum != null && item.CHICNum != "")
                        {
                            oTA.FillByOrderNo(oDT, item.CHICNum);

                            if (oDT.Count == 0)
                            {
                                hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Pedido não encontrado no CHIC!");
                            }
                            else
                            {
                                #region Carrega Itens CHIC

                                Data.CHICDataSet.ordersRow oRow = oDT.FirstOrDefault();
                                bookedTableAdapter bTA = new bookedTableAdapter();
                                Data.CHICDataSet.bookedDataTable bDT = new Data.CHICDataSet.bookedDataTable();
                                bTA.FillByOrderNo(bDT, oRow.orderno);

                                #endregion

                                #region Ajusta pedidos de Ovos como Nascimento e não Retirada

                                DateTime setDateErro = data.AddDays(-21);
                                int existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("H")))
                                    && w.cal_date == setDateErro)
                                .Count();

                                if (existeErro > 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Ajuste de pedido de Ovos como Nascimento e não Retirada!");
                                }

                                #endregion

                                #region Ajusta Pedido que foi alterada data

                                existeErro = 0;
                                existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("D")))
                                    && w.cal_date != setDateErro)
                                .Count();

                                if (existeErro > 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Ajuste de pedido que foi alterado a data!");
                                }

                                #endregion

                                #region Verifica se a Linhagem ainda existe no Pedido no mesmo incubatório

                                existeErro = 0;
                                existeErro = bDT.Where(w => iDT.Any(a => a.item_no == w.item
                                        && a.variety.Trim() == item.Linhagem
                                        && a.form.Trim() == item.Produto)
                                        && w.location.Trim() == item.LocalNascimento)
                                    .Count();

                                if (existeErro == 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Verificação se a Linhagem ainda existe no Pedido no mesmo incubatório!");
                                }

                                #endregion

                                #region Verifica se o Incubatório ainda existe no Pedido

                                existeErro = 0;
                                existeErro = bDT.Where(w => w.location.Trim() == item.LocalNascimento
                                        && item.LocalNascimento != null)
                                    .Count();

                                if (existeErro == 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(item, "Exclusão", "Verificação se o Incubatório ainda existe no Pedido!");
                                }

                                #endregion
                            }
                        }
                    }

                    hlbapp.SaveChanges();

                    #endregion

                    #region Pintos

                    #region Filtra Pedidos

                    //DateTime data = DateTime.Today;
                    //oTA.FillSalesByHatchDate2(oDT, data);
                    DateTime setDate = data.AddDays(-21);
                    oTA.FillSalesByCalDate(oDT, setDate);

                    var listaOrders = oDT
                        //.Where(w => w.orderno == "99299")
                        .ToList();

                    #endregion

                    foreach (var order in listaOrders)
                    {
                        #region Dados Item

                        iTA.Fill(iDT);

                        bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        Data.CHICDataSet.bookedDataTable bDTCommercial = new Data.CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, order.orderno);

                        var listaItens = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1).Equals("D")
                                //&& a.variety != "DKBW" && a.variety != "DKBB")
                                )))
                            .Join(
                                iDT,
                                b => b.item,
                                i => i.item_no,
                                (b, i) => new { BOOKED = b, ITEM = i })
                            .GroupBy(g => new
                            {
                                //g.BOOKED.item,
                                g.BOOKED.location,
                                g.ITEM.variety,
                                g.ITEM.form
                            })
                            .Select(s => new
                            {
                                //s.Key.item,
                                s.Key.location,
                                s.Key.variety,
                                s.Key.form,
                                qtdeBonif = s.Sum(w => w.BOOKED.alt_desc.Contains("Extra") ? w.BOOKED.quantity : 0),
                                qtdeVend = s.Sum(w => !w.BOOKED.alt_desc.Contains("Extra") ? w.BOOKED.quantity : 0),
                                qtde = s.Sum(u => u.BOOKED.quantity),
                                price = s.Max(m => m.BOOKED.price)
                            })
                            .ToList();

                        #endregion

                        #region Dados Custom Table

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        Data.CHICDataSet.int_commDataTable icDT = new Data.CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, order.orderno);

                        salesmanTableAdapter slTA = new salesmanTableAdapter();
                        Data.CHICDataSet.salesmanDataTable slDT = new Data.CHICDataSet.salesmanDataTable();
                        slTA.FillByCod(slDT, order.salesrep);

                        string codigoCliente = order.cust_no.Trim();
                        if (slDT.Count == 0)
                        {
                            custTableAdapter cTA = new custTableAdapter();
                            Data.CHICDataSet.custDataTable cDT = new Data.CHICDataSet.custDataTable();
                            cTA.FillByCustoNo(cDT, order.cust_no);
                            if (cDT.Count > 0)
                            {
                                slTA.FillByCod(slDT, cDT.FirstOrDefault().salesman);
                            }
                        }

                        #region Verifica Transportadora

                        CIDADE cidadeEntidade = apolo.CIDADE
                            .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                                && a.EntCod == codigoCliente)).FirstOrDefault();

                        Data.CHICDataSet.int_commRow iR = icDT.FirstOrDefault();
                        string empresaTransportadoraCHIC = "";
                        if (cidadeEntidade.UfSigla == "EX")
                        {
                            empresaTransportadoraCHIC = "EX";
                        }
                        else
                        {
                            if (iR != null)
                            {
                                string transportadoraPedido = iR.tranport.Trim();

                                if (transportadoraPedido.Equals("Planalto")
                                    || (slDT[0].inv_comp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                    empresaTransportadoraCHIC = "PL";
                                else if (((transportadoraPedido.Equals("H&N"))
                                            || (!slDT[0].inv_comp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                    && data >= Convert.ToDateTime("31/07/2017")) // Data da Implantação
                                    empresaTransportadoraCHIC = "HN";
                                else
                                    empresaTransportadoraCHIC = "TR";
                            }
                            else
                                empresaTransportadoraCHIC = "TR";
                        }

                        #endregion

                        #endregion

                        #region Verifica se é Reposição

                        CHICDataSet.int_commDataTable icDTVerificaRepo = new CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDTVerificaRepo, order.orderno.Trim());

                        bool eReposicao = false;
                        if (icDTVerificaRepo.Count > 0)
                        {
                            if (icDTVerificaRepo[0].npedrepo > 0) eReposicao = true;
                        }

                        #endregion

                        foreach (var item in listaItens)
                        {
                            #region Carrega Valores

                            string linhagem = item.variety.Trim();
                            string produto = item.form.Trim();

                            bool entrou = false;
                            if (order.orderno.Equals("69400"))
                                entrou = true;

                            int? numVeiculo = 0;
                            string embalagem = "";
                            string status = "Pendente";
                            string observacao = "";
                            int? ordem = 0;
                            string inicioCarregamentoEsperado = "";
                            string chegadaClienteEsperada = "";
                            int? km = 0;
                            string inicioCarregamentoReal = "";
                            string chegadaCarregamentoReal = "";
                            int? qtdCaixas = 0;
                            string numRoteiroEntregaFluig = "";

                            List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                .Where(w => w.CHICNum == order.orderno.Trim()
                                && w.Linhagem == linhagem && w.Produto == produto
                                && w.LocalNascimento == item.location
                                && w.CHICOrigem == "Matriz").ToList();

                            #endregion

                            string empresaTransp = empresaTransportadora;
                            if (listProdDiariaTranspPedido.Count > 0) empresaTransp = listProdDiariaTranspPedido.FirstOrDefault().EmpresaTranportador;

                            if ((empresaTransportadora == empresaTransp) || empresaTransportadora == "")
                            {
                                #region Verifica Se existe já lançado

                                if (listProdDiariaTranspPedido.Count > 1)
                                {
                                    foreach (var pedido in listProdDiariaTranspPedido)
                                    {
                                        numVeiculo = pedido.NumVeiculo;
                                        embalagem = pedido.Embalagem;
                                        status = pedido.Status;
                                        observacao = pedido.Observacao;
                                        ordem = pedido.Ordem;
                                        inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                        chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                        km = pedido.KM;
                                        inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                        chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                        qtdCaixas = pedido.QuantidadeCaixa;
                                        numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                        InsereLOG_Prog_Diaria_Transp_Pedidos(pedido, "Exclusão", "Exclusão do Pedido duplicado para lançamento de novo copiando os dados de transporte!");
                                    }
                                }

                                hlbapp.SaveChanges();

                                Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                    .Where(w => w.CHICNum == order.orderno.Trim()
                                    && w.Linhagem == linhagem && w.Produto == produto
                                    && w.LocalNascimento == item.location
                                    && w.CHICOrigem == "Matriz").FirstOrDefault();

                                if (prodDiariaTranspPedido != null)
                                {
                                    numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                                    embalagem = prodDiariaTranspPedido.Embalagem;
                                    status = prodDiariaTranspPedido.Status;
                                    observacao = prodDiariaTranspPedido.Observacao;
                                    ordem = prodDiariaTranspPedido.Ordem;
                                    inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                                    chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                                    km = prodDiariaTranspPedido.KM;
                                    inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                                    chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                    qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                                    numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Exclusão", "Exclusão do Pedido para lançamento de novo copiando os dados de transporte!");
                                }

                                #endregion

                                if(!eReposicao)
                                {
                                    #region Insere Programação Diária

                                    prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                                    prodDiariaTranspPedido.CHICOrigem = "Matriz";
                                    prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                                    prodDiariaTranspPedido.Embalagem = embalagem;
                                    prodDiariaTranspPedido.Status = status;
                                    prodDiariaTranspPedido.Observacao = observacao;
                                    prodDiariaTranspPedido.Ordem = ordem;
                                    prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                                    prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                                    prodDiariaTranspPedido.KM = km;
                                    prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                                    prodDiariaTranspPedido.ChegadaClienteReal = chegadaCarregamentoReal;
                                    prodDiariaTranspPedido.QuantidadeCaixa = qtdCaixas;
                                    prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                                    prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date.AddDays(21);
                                    prodDiariaTranspPedido.CodigoCliente = codigoCliente;

                                    ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                        .FirstOrDefault();

                                    //if (entidade.EntNome.Length > 15)
                                    ////    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0,15) + "...";
                                    //else
                                    if (entidade != null)
                                        prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                                    else
                                        prodDiariaTranspPedido.NomeCliente = "NÃO EXISTE ENTIDADE "
                                            + prodDiariaTranspPedido.CodigoCliente + " NO APOLO! VERIFICAR COM A PROGRAMAÇÃO!";
                                    prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                                    string condPag = "";
                                    if (order.delivery.IndexOf("(") > 0)
                                        condPag = (order.delivery.Substring(0, (order.delivery.IndexOf("(") - 1))).Trim();
                                    else
                                        condPag = order.delivery.Trim();
                                    prodDiariaTranspPedido.CondicaoPagamento = condPag;

                                    #region Local de Entrega

                                    if (order.contact_no != 0)
                                    {
                                        shippingTableAdapter sTA = new shippingTableAdapter();
                                        Data.CHICDataSet.shippingDataTable sDT = new Data.CHICDataSet.shippingDataTable();
                                        sTA.FillByCustNo(sDT, order.cust_no);

                                        if (sDT.Count > 0)
                                        {
                                            Data.CHICDataSet.shippingRow enderecoEntrega = sDT
                                                .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                                            if (enderecoEntrega != null)
                                            {
                                                prodDiariaTranspPedido.LocalEntrega = enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (entidade != null)
                                        {
                                            CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                            //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                            //    cidade.PaisSigla;
                                            if (cidade != null)
                                                prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                            else
                                                prodDiariaTranspPedido.LocalEntrega = "";
                                        }
                                        else
                                            prodDiariaTranspPedido.LocalEntrega = "";
                                    }

                                    #endregion

                                    prodDiariaTranspPedido.Produto = produto;
                                    prodDiariaTranspPedido.Linhagem = linhagem;
                                    prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                                    prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                                    prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                                    if (entidade != null)
                                    {
                                        ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                            .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                        if (fone != null)
                                        {
                                            prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                        }
                                    }

                                    prodDiariaTranspPedido.DataEntrega = order.del_date;
                                    prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                                    if (icDT.Count > 0)
                                    {
                                        prodDiariaTranspPedido.ObservacaoCHIC = icDT[0].hatchinf.Trim();
                                        prodDiariaTranspPedido.ObsProgramacao = icDT[0].comments.Trim();
                                    }
                                    else
                                    {
                                        prodDiariaTranspPedido.ObservacaoCHIC = "";
                                        prodDiariaTranspPedido.ObsProgramacao = "";
                                    }

                                    #region Debicagem

                                    int existeDebicagem = bDTCommercial.Where(w => w.item == "169").Count();
                                    if (existeDebicagem > 0)
                                        prodDiariaTranspPedido.Debicagem = "X";
                                    else
                                        prodDiariaTranspPedido.Debicagem = "";

                                    #endregion

                                    if (slDT[0].salesman.Trim().Length > 15)
                                        prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim().Substring(0, 15) + "...";
                                    else
                                        prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                                    #region Verifica Transportadora

                                    prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                                    #endregion

                                    #region Campos Novos para Programação Fluig

                                    int idPV = 0;
                                    if (order.po_number.Trim() != "")
                                    {
                                        if (int.TryParse(order.po_number.Trim(), out idPV))
                                        {
                                            var existePVWEB = hlbappService.Pedido_Venda.Where(w => w.ID == idPV).FirstOrDefault();
                                            if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = idPV;
                                        }
                                        else
                                        {
                                            var orderno = order.orderno.Trim();
                                            var existePVWEB = hlbappService.Item_Pedido_Venda
                                                .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                                                .FirstOrDefault();
                                            if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                                        }
                                    }
                                    else
                                    {
                                        var orderno = order.orderno.Trim();
                                        var existePVWEB = hlbappService.Item_Pedido_Venda
                                            .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                                            .FirstOrDefault();
                                        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                                    }
                                    prodDiariaTranspPedido.EnderEntSeq = Convert.ToInt32(order.contact_no);
                                    prodDiariaTranspPedido.QtdeVendida = Convert.ToInt32(item.qtdeVend);
                                    prodDiariaTranspPedido.QtdeBonificada = Convert.ToInt32(item.qtdeBonif);

                                    var qtdeVendidaParaCalculoPercBonificacao = listaItens
                                        .Where(w => w.variety == item.variety && w.price > 0)
                                        .Sum(s => s.qtdeVend);

                                    if (prodDiariaTranspPedido.QtdeBonificada != 0 && prodDiariaTranspPedido.QtdeVendida != 0)
                                        //prodDiariaTranspPedido.PercBonificacao = Convert.ToInt32(((prodDiariaTranspPedido.QtdeBonificada * 1.00m)
                                        //    / (prodDiariaTranspPedido.QtdeVendida * 1.00m)) * 100.00m);
                                        prodDiariaTranspPedido.PercBonificacao = Convert.ToInt32(((prodDiariaTranspPedido.QtdeBonificada * 1.00m)
                                            / (qtdeVendidaParaCalculoPercBonificacao * 1.00m)) * 100.00m);
                                    prodDiariaTranspPedido.MotivoSobra = "";
                                    prodDiariaTranspPedido.QtdeReposicao = 0;
                                    prodDiariaTranspPedido.PrecoProduto = item.price;

                                    #region Carrega Pedido de Reposição Caso Exista

                                    ImportaCHICService.Data.CHICDataSet.int_commDataTable icDTReposicao = new ImportaCHICService.Data.CHICDataSet.int_commDataTable();
                                    ImportaCHICService.Data.CHICDataSetTableAdapters.int_commTableAdapter icTA2 =
                                        new ImportaCHICService.Data.CHICDataSetTableAdapters.int_commTableAdapter();
                                    icTA2.FillByNpedrepo(icDTReposicao, Convert.ToDecimal(order.orderno.Trim()));

                                    string orderNoCHICReposicao = null;
                                    if (icDTReposicao.Count > 0)
                                    {
                                        orderNoCHICReposicao = icDTReposicao[0].orderno;
                                    }

                                    if (orderNoCHICReposicao != null)
                                    {
                                        CHICDataSet.bookedDataTable bDTReposicao = new CHICDataSet.bookedDataTable();
                                        bTACommercial.FillByOrderNo(bDTReposicao, orderNoCHICReposicao);
                                        CHICDataSet.bookedRow bRowReposicao = bDTReposicao
                                            .Where(w => iDT.Any(a => a.item_no == w.item
                                                            && a.form == item.form
                                                            && a.variety == item.variety)
                                                    && w.location == item.location)
                                            .FirstOrDefault();

                                        if (bRowReposicao != null)
                                        {
                                            prodDiariaTranspPedido.CHICNumReposicao = orderNoCHICReposicao;
                                            prodDiariaTranspPedido.QtdeReposicao = Convert.ToInt32(bRowReposicao.quantity);
                                            if (bRowReposicao.comment_1.Contains("Acerto"))
                                                prodDiariaTranspPedido.MotivoReposicao = "Acerto Comercial";
                                            else if (bRowReposicao.comment_1.Contains("Mortalidade"))
                                                prodDiariaTranspPedido.MotivoReposicao = "Mortalidade";
                                        }
                                    }

                                    #endregion

                                    #region Campos Customizados do Item - Sobra

                                    ImportaCHICService.Data.CHICDataSetTableAdapters.custitemTableAdapter ciTA =
                                        new ImportaCHICService.Data.CHICDataSetTableAdapters.custitemTableAdapter();
                                    ImportaCHICService.Data.CHICDataSet.custitemDataTable ciDT =
                                        new ImportaCHICService.Data.CHICDataSet.custitemDataTable();
                                    ciTA.FillByVarietyFormLocation(ciDT, item.variety, item.form, item.location, order.orderno.Trim());

                                    if (ciDT.Count > 0)
                                    {
                                        if (ciDT[0].sobra.Trim() == "Sim")
                                            prodDiariaTranspPedido.QtdeSobra = Convert.ToInt32(item.qtde);
                                    }

                                    #endregion

                                    prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde) + prodDiariaTranspPedido.QtdeReposicao;

                                    #endregion

                                    prodDiariaTranspPedido.Empresa = slDT[0].inv_comp.Trim();
                                    prodDiariaTranspPedido.MotivoSobra = "";

                                    hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Inclusão", "Nova inclusão do pedido no Web!");

                                    #endregion
                                }
                            }
                        }
                    }

                    #endregion

                    #region Ovos

                    #region Filtra Pedidos

                    //DateTime data = DateTime.Today;
                    //oTA.FillSalesByHatchDate2(oDT, data);
                    setDate = data;
                    oTA.FillSalesByCalDate(oDT, setDate);

                    listaOrders = oDT
                        //.Where(w => w.orderno == "59994")
                        .ToList();

                    #endregion

                    foreach (var order in listaOrders)
                    {
                        #region Dados Item

                        bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        Data.CHICDataSet.bookedDataTable bDTCommercial = new Data.CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, order.orderno);

                        var listaItens = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1).Equals("H")
                                //&& a.variety != "DKBW" && a.variety != "DKBB"
                                )))
                            .Join(
                                iDT,
                                b => b.item,
                                i => i.item_no,
                                (b, i) => new { BOOKED = b, ITEM = i })
                            .GroupBy(g => new
                            {
                                //g.BOOKED.item,
                                g.BOOKED.location,
                                g.ITEM.variety,
                                g.ITEM.form
                            })
                            .Select(s => new
                            {
                                //s.Key.item,
                                s.Key.location,
                                s.Key.variety,
                                s.Key.form,
                                qtde = s.Sum(u => u.BOOKED.quantity),
                                price = s.Max(m => m.BOOKED.price)
                            })
                            .ToList();

                        #endregion

                        #region Dados Custom Table

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        Data.CHICDataSet.int_commDataTable icDT = new Data.CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, order.orderno);

                        salesmanTableAdapter slTA = new salesmanTableAdapter();
                        Data.CHICDataSet.salesmanDataTable slDT = new Data.CHICDataSet.salesmanDataTable();
                        slTA.FillByCod(slDT, order.salesrep);
                        
                        string codigoCliente = order.cust_no.Trim();

                        if (slDT.Count == 0)
                        {
                            custTableAdapter cTA = new custTableAdapter();
                            Data.CHICDataSet.custDataTable cDT = new Data.CHICDataSet.custDataTable();
                            cTA.FillByCustoNo(cDT, order.cust_no);
                            if (cDT.Count > 0)
                            {
                                slTA.FillByCod(slDT, cDT.FirstOrDefault().salesman);
                            }
                        }
                        
                        #region Verifica Transportadora

                        CIDADE cidadeEntidade = apolo.CIDADE
                            .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                                && a.EntCod == codigoCliente)).FirstOrDefault();

                        Data.CHICDataSet.int_commRow iR = icDT.FirstOrDefault();
                        string empresaTransportadoraCHIC = "";
                        if (cidadeEntidade.UfSigla == "EX")
                        {
                            empresaTransportadoraCHIC = "EX";
                        }
                        else
                        {
                            if (iR != null)
                            {
                                string transportadoraPedido = iR.tranport.Trim();

                                if (transportadoraPedido.Equals("Planalto")
                                    || (slDT[0].inv_comp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                    empresaTransportadoraCHIC = "PL";
                                else if (((transportadoraPedido.Equals("H&N"))
                                            || (!slDT[0].inv_comp.Equals("PL") && transportadoraPedido.Equals("Outras")))
                                    && data >= Convert.ToDateTime("31/07/2017")) // Data da Implantação
                                    empresaTransportadoraCHIC = "HN";
                                else
                                    empresaTransportadoraCHIC = "TR";
                            }
                            else
                                empresaTransportadoraCHIC = "TR";
                        }

                        #endregion

                        #endregion

                        foreach (var item in listaItens)
                        {
                            #region Carrega Dados

                            string linhagem = item.variety;
                            string produto = item.form;

                            int? numVeiculo = 0;
                            string embalagem = "";
                            string status = "Pendente";
                            string observacao = "";
                            int? ordem = 0;
                            string inicioCarregamentoEsperado = "";
                            string chegadaClienteEsperada = "";
                            int? km = 0;
                            string inicioCarregamentoReal = "";
                            string chegadaClienteReal = "";
                            string chegadaCarregamentoReal = "";
                            int? qtdCaixas = 0;
                            string numRoteiroEntregaFluig = "";

                            List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                .Where(w => w.CHICNum == order.orderno.Trim()
                                && w.Linhagem == linhagem && w.Produto == produto
                                    //&& w.LocalNascimento == item.location).ToList();
                                && w.CHICOrigem == "Matriz"
                                ).ToList();

                            #endregion

                            string empresaTransp = empresaTransportadora;
                            if (listProdDiariaTranspPedido.Count > 0) empresaTransp = listProdDiariaTranspPedido.FirstOrDefault().EmpresaTranportador;

                            if ((empresaTransportadora == empresaTransp) || empresaTransportadora == "")
                            {
                                #region Verifica Se existe já lançado

                                if (listProdDiariaTranspPedido.Count > 1)
                                {
                                    foreach (var pedido in listProdDiariaTranspPedido)
                                    {
                                        numVeiculo = pedido.NumVeiculo;
                                        embalagem = pedido.Embalagem;
                                        status = pedido.Status;
                                        observacao = pedido.Observacao;
                                        ordem = pedido.Ordem;
                                        inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                        chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                        km = pedido.KM;
                                        inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                        chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                        qtdCaixas = pedido.QuantidadeCaixa;
                                        numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                        InsereLOG_Prog_Diaria_Transp_Pedidos(pedido, "Exclusão", "Exclusão do Pedido duplicado para lançamento de novo copiando os dados de transporte!");
                                    }
                                }

                                hlbapp.SaveChanges();

                                Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                    .Where(w => w.CHICNum == order.orderno.Trim()
                                    && w.Linhagem == linhagem && w.Produto == produto
                                        //&& w.LocalNascimento == item.location).FirstOrDefault();
                                    && w.CHICOrigem == "Matriz"
                                    ).FirstOrDefault();

                                if (prodDiariaTranspPedido != null)
                                {
                                    numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                                    embalagem = prodDiariaTranspPedido.Embalagem;
                                    status = prodDiariaTranspPedido.Status;
                                    observacao = prodDiariaTranspPedido.Observacao;
                                    ordem = prodDiariaTranspPedido.Ordem;
                                    inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                                    chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                                    km = prodDiariaTranspPedido.KM;
                                    inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                                    chegadaClienteReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                    chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                    qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                                    numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                                    InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Exclusão", "Exclusão do Pedido para lançamento de novo copiando os dados de transporte!");
                                }

                                #endregion

                                #region Insere Programação Diária

                                prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                                prodDiariaTranspPedido.CHICOrigem = "Matriz";
                                prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                                prodDiariaTranspPedido.Embalagem = embalagem;
                                prodDiariaTranspPedido.Status = status;
                                prodDiariaTranspPedido.Observacao = observacao;
                                prodDiariaTranspPedido.Ordem = ordem;
                                prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                                prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                                prodDiariaTranspPedido.KM = km;
                                prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                                prodDiariaTranspPedido.ChegadaClienteReal = chegadaClienteReal;
                                prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                                prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date;
                                prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                                ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                    .FirstOrDefault();

                                if (entidade != null)
                                {
                                    //if (entidade.EntNome.Length > 15)
                                    //    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0, 15) + "...";
                                    //else
                                        prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                                }

                                string condPag = "";
                                if (order.delivery.IndexOf("(") > 0)
                                    condPag = (order.delivery.Substring(0, (order.delivery.IndexOf("(") - 1))).Trim();
                                else
                                    condPag = order.delivery.Trim();
                                prodDiariaTranspPedido.CondicaoPagamento = condPag;

                                prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);
                                prodDiariaTranspPedido.QtdeVendida = Convert.ToInt32(item.qtde);
                                prodDiariaTranspPedido.MotivoSobra = "";
                                prodDiariaTranspPedido.QtdeReposicao = 0;
                                prodDiariaTranspPedido.PrecoProduto = item.price;

                                int idPV = 0;
                                if (order.po_number.Trim() != "")
                                {
                                    if (int.TryParse(order.po_number.Trim(), out idPV))
                                    {
                                        var existePVWEB = hlbappService.Pedido_Venda.Where(w => w.ID == idPV).FirstOrDefault();
                                        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = idPV;
                                    }
                                    else
                                    {
                                        var orderno = order.orderno.Trim();
                                        var existePVWEB = hlbappService.Item_Pedido_Venda
                                            .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                                            .FirstOrDefault();
                                        if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                                    }
                                }
                                else
                                {
                                    var orderno = order.orderno.Trim();
                                    var existePVWEB = hlbappService.Item_Pedido_Venda
                                        .Where(w => (w.OrderNoCHIC == orderno || w.OrderNoCHICReposicao == orderno))
                                        .FirstOrDefault();
                                    if (existePVWEB != null) prodDiariaTranspPedido.IDPedidoVenda = existePVWEB.IDPedidoVenda;
                                }
                                prodDiariaTranspPedido.EnderEntSeq = Convert.ToInt32(order.contact_no);

                                #region Local de Entrega

                                if (order.contact_no != 0)
                                {
                                    shippingTableAdapter sTA = new shippingTableAdapter();
                                    Data.CHICDataSet.shippingDataTable sDT = new Data.CHICDataSet.shippingDataTable();
                                    sTA.FillByCustNo(sDT, order.cust_no);

                                    if (sDT.Count > 0)
                                    {
                                        Data.CHICDataSet.shippingRow enderecoEntrega = sDT
                                            .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                                        if (enderecoEntrega != null)
                                        {
                                            prodDiariaTranspPedido.LocalEntrega =
                                                enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                                        }
                                    }
                                }
                                else
                                {
                                    if (entidade != null)
                                    {
                                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                        //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                        //    cidade.PaisSigla;
                                        prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                    }
                                }

                                #endregion

                                prodDiariaTranspPedido.Produto = produto;
                                prodDiariaTranspPedido.Linhagem = linhagem;
                                prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                                prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                                prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                                if (entidade != null)
                                {
                                    ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                        .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                    if (fone != null)
                                    {
                                        prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                    }
                                }

                                prodDiariaTranspPedido.DataEntrega = order.del_date;
                                prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                                if (icDT.Count > 0)
                                {
                                    prodDiariaTranspPedido.ObservacaoCHIC = icDT[0].hatchinf.Trim();
                                    prodDiariaTranspPedido.ObsProgramacao = icDT[0].comments.Trim();
                                }
                                else
                                {
                                    prodDiariaTranspPedido.ObservacaoCHIC = "";
                                    prodDiariaTranspPedido.ObsProgramacao = "";
                                }

                                #region Debicagem

                                int existeDebicagem = bDTCommercial.Where(w => w.item == "169").Count();
                                if (existeDebicagem > 0)
                                    prodDiariaTranspPedido.Debicagem = "X";
                                else
                                    prodDiariaTranspPedido.Debicagem = "";

                                #endregion

                                if (slDT[0].salesman.Trim().Length > 15)
                                    prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim().Substring(0, 15) + "...";
                                else
                                    prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                                #region Verifica Transportadora

                                prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                                #endregion

                                prodDiariaTranspPedido.Empresa = slDT[0].inv_comp.Trim();

                                hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);
                                InsereLOG_Prog_Diaria_Transp_Pedidos(prodDiariaTranspPedido, "Inclusão", "Nova inclusão do pedido no Web!");

                                #endregion
                            }
                        }
                    }

                    #endregion

                    hlbapp.SaveChanges();

                    #endregion

                    #region Insere Pedidos da Programação Diária de Transporte - CHIC Avós

                    if (origem != "UPD")
                    {
                        ordersParentTableAdapter opTA = new ordersParentTableAdapter();
                        CHICParentDataSet.ordersParentDataTable opDT = new CHICParentDataSet.ordersParentDataTable();

                        itemsParentTableAdapter ipTA = new itemsParentTableAdapter();
                        CHICParentDataSet.itemsParentDataTable ipDT = new CHICParentDataSet.itemsParentDataTable();
                        ipTA.Fill(ipDT);

                        #region Deleta Pedidos não Existentes

                        var listaPedidosDataAvos = hlbapp.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.DataProgramacao == data
                                //&& w.CHICNum == "74040"
                                && w.CHICOrigem == "Avós"
                                ).ToList();

                        foreach (var item in listaPedidosDataAvos)
                        {
                            if (item.CHICNum != null && item.CHICNum != "")
                            {
                                opTA.FillByOrderNo(opDT, item.CHICNum);

                                if (opDT.Count == 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                }
                                else
                                {
                                    #region Carrega Itens CHIC

                                    CHICParentDataSet.ordersParentRow opRow = opDT.FirstOrDefault();
                                    bookedParentTableAdapter bpTA = new bookedParentTableAdapter();
                                    CHICParentDataSet.bookedParentDataTable bpDT = new CHICParentDataSet.bookedParentDataTable();
                                    bpTA.FillByOrderNo(bpDT, opRow.orderno);

                                    #endregion

                                    #region Ajusta pedidos de Ovos como Nascimento e não Retirada

                                    DateTime setDateErro = data.AddDays(-21);
                                    int existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                                        && (a.form.Substring(0, 1).Equals("H")))
                                        && w.cal_date == setDateErro)
                                    .Count();

                                    if (existeErro > 0)
                                    {
                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    }

                                    #endregion

                                    #region Ajusta Pedido que foi alterada data

                                    existeErro = 0;
                                    existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                                        && (a.form.Substring(0, 1).Equals("P")))
                                        && w.cal_date != setDateErro)
                                    .Count();

                                    if (existeErro > 0)
                                    {
                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    }

                                    #endregion

                                    #region Verifica se a Linhagem ainda existe no Pedido no mesmo incubatório

                                    existeErro = 0;
                                    existeErro = bpDT.Where(w => ipDT.Any(a => a.item_no == w.item
                                            && a.variety.Trim() == item.Linhagem
                                            && a.form.Trim() == item.Produto)
                                            && w.location.Trim() == item.LocalNascimento)
                                        .Count();

                                    if (existeErro == 0)
                                    {
                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    }

                                    #endregion

                                    #region Verifica se o Incubatório ainda existe no Pedido

                                    existeErro = 0;
                                    existeErro = bpDT.Where(w => w.location.Trim() == item.LocalNascimento
                                            && item.LocalNascimento != null)
                                        .Count();

                                    if (existeErro == 0)
                                    {
                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                    }

                                    #endregion
                                }
                            }
                        }

                        hlbapp.SaveChanges();

                        #endregion

                        #region Pintos

                        #region Filtra Pedidos

                        //DateTime data = DateTime.Today;
                        //oTA.FillSalesByHatchDate2(oDT, data);
                        setDate = data.AddDays(-21);
                        opTA.FillSalesByCalDate(opDT, setDate);

                        var listaOrdersParent = opDT
                            //.Where(w => w.orderno == "81060")
                            .ToList();

                        #endregion

                        foreach (var order in listaOrdersParent)
                        {
                            #region Dados Item

                            ipTA.Fill(ipDT);

                            bookedParentTableAdapter bpTACommercial =
                                new Data.CHICParentDataSetTableAdapters.bookedParentTableAdapter();
                            CHICParentDataSet.bookedParentDataTable bpDTCommercial = new CHICParentDataSet.bookedParentDataTable();
                            bpTACommercial.FillByOrderNo(bpDTCommercial, order.orderno);

                            var listaItens = bpDTCommercial
                                .Where(w => ipDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("P")
                                    //&& a.variety != "DKBW" && a.variety != "DKBB")
                                    )))
                                .Join(
                                    ipDT,
                                    b => b.item,
                                    i => i.item_no,
                                    (b, i) => new { BOOKED = b, ITEM = i })
                                .GroupBy(g => new
                                {
                                //g.BOOKED.item,
                                g.BOOKED.location,
                                    g.ITEM.variety,
                                    g.ITEM.form
                                })
                                .Select(s => new
                                {
                                //s.Key.item,
                                s.Key.location,
                                    s.Key.variety,
                                    s.Key.form,
                                    qtde = s.Sum(u => u.BOOKED.quantity),
                                    price = s.Max(m => m.BOOKED.price)
                                })
                                .ToList();

                            #endregion

                            #region Dados Custom Table

                            int_commParentTableAdapter icpTA = new int_commParentTableAdapter();
                            CHICParentDataSet.int_commParentDataTable icpDT = new CHICParentDataSet.int_commParentDataTable();
                            icpTA.FillByOrderNo(icpDT, order.orderno);

                            salesmanParentTableAdapter slpTA = new salesmanParentTableAdapter();
                            CHICParentDataSet.salesmanParentDataTable slpDT = new CHICParentDataSet.salesmanParentDataTable();
                            slpTA.FillByCod(slpDT, order.salesrep);

                            custParentTableAdapter cpTA = new custParentTableAdapter();
                            CHICParentDataSet.custParentDataTable cpDT = new CHICParentDataSet.custParentDataTable();
                            cpTA.FillByCustNo(cpDT, order.cust_no);

                            if (slpDT.Count == 0)
                            {
                                slpTA.FillByCod(slpDT, cpDT[0].salesman);
                            }

                            string codigoCliente = order.cust_no.Trim();

                            #region Verifica Transportadora

                            CIDADE cidadeEntidade = apolo.CIDADE
                                .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                                    && a.EntCod == codigoCliente)).FirstOrDefault();

                            CHICParentDataSet.int_commParentRow ipR = icpDT.FirstOrDefault();
                            string empresaTransportadoraCHIC = "";
                            //if (order.cust_no.Trim() == "0000178")
                            if (cpDT.FirstOrDefault().name.Contains("HY LINE DO BRASIL"))
                            {
                                empresaTransportadoraCHIC = "AI";
                            }
                            else
                            {
                                empresaTransportadoraCHIC = "EX";
                            }

                            #endregion

                            #endregion

                            if (empresaTransportadora == empresaTransportadoraCHIC)
                            {
                                foreach (var item in listaItens)
                                {
                                    #region Verifica Se existe já lançado

                                    string linhagem = item.variety.Trim();
                                    string produto = item.form.Trim();

                                    bool entrou = false;
                                    if (order.orderno.Equals("69400"))
                                        entrou = true;

                                    int? numVeiculo = 0;
                                    string embalagem = "";
                                    string status = "Pendente";
                                    string observacao = "";
                                    int? ordem = 0;
                                    string inicioCarregamentoEsperado = "";
                                    string chegadaClienteEsperada = "";
                                    int? km = 0;
                                    string inicioCarregamentoReal = "";
                                    string chegadaCarregamentoReal = "";
                                    int? qtdCaixas = 0;
                                    string numRoteiroEntregaFluig = "";

                                    List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                        .Where(w => w.CHICNum == order.orderno.Trim()
                                        && w.Linhagem == linhagem && w.Produto == produto
                                        && w.LocalNascimento == item.location
                                        && w.CHICOrigem == "Avós"
                                        ).ToList();

                                    if (listProdDiariaTranspPedido.Count > 1)
                                    {
                                        foreach (var pedido in listProdDiariaTranspPedido)
                                        {
                                            numVeiculo = pedido.NumVeiculo;
                                            embalagem = pedido.Embalagem;
                                            status = pedido.Status;
                                            observacao = pedido.Observacao;
                                            ordem = pedido.Ordem;
                                            inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                            chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                            km = pedido.KM;
                                            inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                            chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                            qtdCaixas = pedido.QuantidadeCaixa;
                                            numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                            hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                        }
                                    }

                                    hlbapp.SaveChanges();

                                    Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                        .Where(w => w.CHICNum == order.orderno.Trim()
                                        && w.Linhagem == linhagem && w.Produto == produto
                                        && w.LocalNascimento == item.location
                                        && w.CHICOrigem == "Avós"
                                        ).FirstOrDefault();

                                    if (prodDiariaTranspPedido != null)
                                    {
                                        numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                                        embalagem = prodDiariaTranspPedido.Embalagem;
                                        status = prodDiariaTranspPedido.Status;
                                        observacao = prodDiariaTranspPedido.Observacao;
                                        ordem = prodDiariaTranspPedido.Ordem;
                                        inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                                        chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                                        km = prodDiariaTranspPedido.KM;
                                        inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                                        chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                        qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                                        numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                                    }

                                    #endregion

                                    #region Insere Programação Diária

                                    prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                                    prodDiariaTranspPedido.CHICOrigem = "Avós";
                                    prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                                    prodDiariaTranspPedido.Embalagem = embalagem;
                                    prodDiariaTranspPedido.Status = status;
                                    prodDiariaTranspPedido.Observacao = observacao;
                                    prodDiariaTranspPedido.Ordem = ordem;
                                    prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                                    prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                                    prodDiariaTranspPedido.KM = km;
                                    prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                                    prodDiariaTranspPedido.ChegadaClienteReal = chegadaCarregamentoReal;
                                    prodDiariaTranspPedido.QuantidadeCaixa = qtdCaixas;
                                    prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                                    prodDiariaTranspPedido.DataProgramacao = bpDTCommercial[0].cal_date.AddDays(21);
                                    prodDiariaTranspPedido.CodigoCliente = codigoCliente;

                                    ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                        .FirstOrDefault();

                                    //if (entidade.EntNome.Length > 15)
                                    ////    prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0,15) + "...";
                                    //else
                                    if (entidade != null)
                                        prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                                    else
                                        prodDiariaTranspPedido.NomeCliente = "NÃO EXISTE ENTIDADE "
                                            + prodDiariaTranspPedido.CodigoCliente + " NO APOLO! VERIFICAR COM A PROGRAMAÇÃO!";
                                    prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                                    #region Local de Entrega

                                    if (entidade != null)
                                    {
                                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                        if (cidade != null)
                                            prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                        else
                                            prodDiariaTranspPedido.LocalEntrega = "";
                                    }
                                    else
                                        prodDiariaTranspPedido.LocalEntrega = "";

                                    #endregion

                                    prodDiariaTranspPedido.Produto = produto;
                                    prodDiariaTranspPedido.Linhagem = linhagem;
                                    prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                                    prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                                    prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                                    if (entidade != null)
                                    {
                                        ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                            .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                        if (fone != null)
                                        {
                                            prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                        }
                                    }

                                    prodDiariaTranspPedido.DataEntrega = order.del_date;
                                    prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                                    if (icpDT.Count > 0)
                                        prodDiariaTranspPedido.ObservacaoCHIC = icpDT[0].hatchinf.Trim();
                                    else
                                        prodDiariaTranspPedido.ObservacaoCHIC = "";

                                    #region Debicagem

                                    int existeDebicagem = bpDTCommercial.Where(w => w.item == "169").Count();
                                    if (existeDebicagem > 0)
                                        prodDiariaTranspPedido.Debicagem = "X";
                                    else
                                        prodDiariaTranspPedido.Debicagem = "";

                                    #endregion

                                    if (slpDT.Count > 0)
                                    {
                                        prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim();
                                        prodDiariaTranspPedido.Empresa = slpDT[0].inv_comp.Trim();
                                    }
                                    else
                                    {
                                        prodDiariaTranspPedido.NomeRepresentante = "";
                                        prodDiariaTranspPedido.Empresa = slpDT[0].inv_comp.Trim();
                                    }

                                    prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                                    hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);

                                    #endregion
                                }
                            }
                        }

                        #endregion

                        #region Ovos

                        #region Filtra Pedidos

                        //DateTime data = DateTime.Today;
                        //oTA.FillSalesByHatchDate2(oDT, data);
                        setDate = data;
                        opTA.FillSalesByCalDate(opDT, setDate);

                        listaOrdersParent = opDT
                            //.Where(w => w.orderno == "59994")
                            .ToList();

                        #endregion

                        foreach (var order in listaOrders)
                        {
                            #region Dados Item

                            bookedParentTableAdapter bpTACommercial =
                                new Data.CHICParentDataSetTableAdapters.bookedParentTableAdapter();
                            CHICParentDataSet.bookedParentDataTable bpDTCommercial = new CHICParentDataSet.bookedParentDataTable();
                            bpTACommercial.FillByOrderNo(bpDTCommercial, order.orderno);

                            var listaItens = bpDTCommercial
                                .Where(w => ipDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("H")
                                    //&& a.variety != "DKBW" && a.variety != "DKBB"
                                    )))
                                .Join(
                                    ipDT,
                                    b => b.item,
                                    i => i.item_no,
                                    (b, i) => new { BOOKED = b, ITEM = i })
                                .GroupBy(g => new
                                {
                                //g.BOOKED.item,
                                g.BOOKED.location,
                                    g.ITEM.variety,
                                    g.ITEM.form
                                })
                                .Select(s => new
                                {
                                //s.Key.item,
                                s.Key.location,
                                    s.Key.variety,
                                    s.Key.form,
                                    qtde = s.Sum(u => u.BOOKED.quantity),
                                    price = s.Max(m => m.BOOKED.price)
                                })
                                .ToList();

                            #endregion

                            #region Dados Custom Table

                            int_commParentTableAdapter icpTA = new int_commParentTableAdapter();
                            CHICParentDataSet.int_commParentDataTable icpDT = new CHICParentDataSet.int_commParentDataTable();
                            icpTA.FillByOrderNo(icpDT, order.orderno);

                            salesmanParentTableAdapter slpTA = new salesmanParentTableAdapter();
                            CHICParentDataSet.salesmanParentDataTable slpDT = new CHICParentDataSet.salesmanParentDataTable();
                            slpTA.FillByCod(slpDT, order.salesrep);

                            string codigoCliente = order.cust_no.Trim();

                            #region Verifica Transportadora

                            CIDADE cidadeEntidade = apolo.CIDADE
                                .Where(w => apolo.ENTIDADE.Any(a => a.CidCod == w.CidCod
                                    && a.EntCod == codigoCliente)).FirstOrDefault();

                            CHICParentDataSet.int_commParentRow ipR = icpDT.FirstOrDefault();
                            string empresaTransportadoraCHIC = "";
                            if (cidadeEntidade.UfSigla == "EX")
                            {
                                empresaTransportadoraCHIC = "EX";
                            }
                            else
                            {
                                if (order.cust_no.Trim() == "0000178")
                                    empresaTransportadoraCHIC = "AI";
                            }

                            #endregion

                            #endregion

                            if (empresaTransportadora == empresaTransportadoraCHIC)
                            {
                                foreach (var item in listaItens)
                                {
                                    #region Verifica Se existe já lançado

                                    string linhagem = item.variety;
                                    string produto = item.form;

                                    int? numVeiculo = 0;
                                    string embalagem = "";
                                    string status = "Pendente";
                                    string observacao = "";
                                    int? ordem = 0;
                                    string inicioCarregamentoEsperado = "";
                                    string chegadaClienteEsperada = "";
                                    int? km = 0;
                                    string inicioCarregamentoReal = "";
                                    string chegadaClienteReal = "";
                                    string chegadaCarregamentoReal = "";
                                    int? qtdCaixas = 0;
                                    string numRoteiroEntregaFluig = "";

                                    List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                        .Where(w => w.CHICNum == order.orderno.Trim()
                                        && w.Linhagem == linhagem && w.Produto == produto
                                        //&& w.LocalNascimento == item.location).ToList();
                                        && w.CHICOrigem == "Avós"
                                        ).ToList();

                                    if (listProdDiariaTranspPedido.Count > 1)
                                    {
                                        foreach (var pedido in listProdDiariaTranspPedido)
                                        {
                                            numVeiculo = pedido.NumVeiculo;
                                            embalagem = pedido.Embalagem;
                                            status = pedido.Status;
                                            observacao = pedido.Observacao;
                                            ordem = pedido.Ordem;
                                            inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                            chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                            km = pedido.KM;
                                            inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                            chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                            qtdCaixas = pedido.QuantidadeCaixa;
                                            numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                            hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                        }
                                    }

                                    hlbapp.SaveChanges();

                                    Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                        .Where(w => w.CHICNum == order.orderno.Trim()
                                        && w.Linhagem == linhagem && w.Produto == produto
                                        //&& w.LocalNascimento == item.location).FirstOrDefault();
                                        && w.CHICOrigem == "Avós"
                                        ).FirstOrDefault();

                                    if (prodDiariaTranspPedido != null)
                                    {
                                        numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                                        embalagem = prodDiariaTranspPedido.Embalagem;
                                        status = prodDiariaTranspPedido.Status;
                                        observacao = prodDiariaTranspPedido.Observacao;
                                        ordem = prodDiariaTranspPedido.Ordem;
                                        inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                                        chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                                        km = prodDiariaTranspPedido.KM;
                                        inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                                        chegadaClienteReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                        chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                        qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                                        numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                                        hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                                    }

                                    #endregion

                                    #region Insere Programação Diária

                                    prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                                    prodDiariaTranspPedido.CHICOrigem = "Avós";
                                    prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                                    prodDiariaTranspPedido.Embalagem = embalagem;
                                    prodDiariaTranspPedido.Status = status;
                                    prodDiariaTranspPedido.Observacao = observacao;
                                    prodDiariaTranspPedido.Ordem = ordem;
                                    prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                                    prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                                    prodDiariaTranspPedido.KM = km;
                                    prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                                    prodDiariaTranspPedido.ChegadaClienteReal = chegadaClienteReal;
                                    prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                                    prodDiariaTranspPedido.DataProgramacao = bpDTCommercial[0].cal_date;
                                    prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                                    ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                        .FirstOrDefault();

                                    if (entidade != null)
                                    {
                                        if (entidade.EntNome.Length > 15)
                                            prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0, 15) + "...";
                                        else
                                            prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                                    }
                                    prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                                    #region Local de Entrega

                                    if (entidade != null)
                                    {
                                        CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                        //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                        //    cidade.PaisSigla;
                                        prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                    }

                                    #endregion

                                    prodDiariaTranspPedido.Produto = produto;
                                    prodDiariaTranspPedido.Linhagem = linhagem;
                                    prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                                    prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                                    prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                                    if (entidade != null)
                                    {
                                        ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                            .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                        if (fone != null)
                                        {
                                            prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                        }
                                    }

                                    prodDiariaTranspPedido.DataEntrega = order.del_date;
                                    prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                                    if (icpDT.Count > 0)
                                        prodDiariaTranspPedido.ObservacaoCHIC = icpDT[0].hatchinf.Trim();
                                    else
                                        prodDiariaTranspPedido.ObservacaoCHIC = "";

                                    #region Debicagem

                                    int existeDebicagem = bpDTCommercial.Where(w => w.item == "169").Count();
                                    if (existeDebicagem > 0)
                                        prodDiariaTranspPedido.Debicagem = "X";
                                    else
                                        prodDiariaTranspPedido.Debicagem = "";

                                    #endregion

                                    if (slpDT[0].salesman.Trim().Length > 15)
                                        prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim().Substring(0, 15) + "...";
                                    else
                                        prodDiariaTranspPedido.NomeRepresentante = slpDT[0].salesman.Trim();

                                    #region Verifica Transportadora

                                    prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                                    #endregion

                                    prodDiariaTranspPedido.Empresa = slpDT[0].inv_comp.Trim();

                                    hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);

                                    #endregion
                                }
                            }
                        }

                        #endregion

                        hlbapp.SaveChanges();
                    }

                    #endregion

                    #region Insere Pedidos da Programação Diária de Transporte - CHIC Matrizes / Transferência entre Incubatórios

                    iTA.Fill(iDT);

                    #region Deleta Pedidos não Existentes

                    var listaPedidosDataTI = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.DataProgramacao == data
                            //&& w.CHICNum == "74040"
                            && ((w.EmpresaTranportador == empresaTransportadora) || empresaTransportadora == "")
                            && w.CHICOrigem == "Matriz"
                            ).ToList();

                    foreach (var item in listaPedidosDataTI)
                    {
                        if (item.CHICNum != null && item.CHICNum != "")
                        {
                            oTA.FillByOrderNo(oDT, item.CHICNum);

                            if (oDT.Count == 0)
                            {
                                hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                            }
                            else
                            {
                                #region Carrega Itens CHIC

                                Data.CHICDataSet.ordersRow otiRow = oDT.FirstOrDefault();
                                bookedTableAdapter btiTA = new bookedTableAdapter();
                                Data.CHICDataSet.bookedDataTable btiDT = new Data.CHICDataSet.bookedDataTable();
                                btiTA.FillByOrderNo(btiDT, otiRow.orderno);

                                #endregion

                                #region Ajusta pedidos de Ovos como Nascimento e não Retirada

                                DateTime setDateErro = data.AddDays(-21);
                                int existeErro = btiDT.Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("H")))
                                    && w.cal_date == setDateErro)
                                .Count();

                                if (existeErro > 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                }

                                #endregion

                                #region Ajusta Pedido que foi alterada data

                                existeErro = 0;
                                existeErro = btiDT.Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("D")))
                                    && w.cal_date != setDateErro)
                                .Count();

                                if (existeErro > 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                }

                                #endregion

                                #region Verifica se a Linhagem ainda existe no Pedido no mesmo incubatório

                                existeErro = 0;
                                existeErro = btiDT.Where(w => iDT.Any(a => a.item_no == w.item
                                        && a.variety.Trim() == item.Linhagem
                                        && a.form.Trim() == item.Produto)
                                        && w.location.Trim() == item.LocalNascimento)
                                    .Count();

                                if (existeErro == 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                }

                                #endregion

                                #region Verifica se o Incubatório ainda existe no Pedido

                                existeErro = 0;
                                existeErro = btiDT.Where(w => w.location.Trim() == item.LocalNascimento
                                        && item.LocalNascimento != null)
                                    .Count();

                                if (existeErro == 0)
                                {
                                    hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(item);
                                }

                                #endregion
                            }
                        }
                    }

                    hlbapp.SaveChanges();

                    #endregion

                    #region Ovos

                    #region Filtra Pedidos

                    //DateTime data = DateTime.Today;
                    //oTA.FillSalesByHatchDate2(oDT, data);
                    setDate = data;
                    oTA.FillTransfersIncByCalDate(oDT, setDate);

                    listaOrders = oDT
                        //.Where(w => w.orderno == "59994")
                        .ToList();

                    #endregion

                    foreach (var order in listaOrders)
                    {
                        #region Dados Item

                        bookedTableAdapter bTACommercial =
                            new Data.CHICDataSetTableAdapters.bookedTableAdapter();
                        Data.CHICDataSet.bookedDataTable bDTCommercial = new Data.CHICDataSet.bookedDataTable();
                        bTACommercial.FillByOrderNo(bDTCommercial, order.orderno);

                        var listaItens = bDTCommercial
                            .Where(w => iDT.Any(a => a.item_no == w.item
                                && (a.form.Substring(0, 1).Equals("H")
                                //&& a.variety != "DKBW" && a.variety != "DKBB"
                                )))
                            .Join(
                                iDT,
                                b => b.item,
                                i => i.item_no,
                                (b, i) => new { BOOKED = b, ITEM = i })
                            .GroupBy(g => new
                            {
                                //g.BOOKED.item,
                                g.BOOKED.location,
                                g.ITEM.variety,
                                g.ITEM.form
                            })
                            .Select(s => new
                            {
                                //s.Key.item,
                                s.Key.location,
                                s.Key.variety,
                                s.Key.form,
                                qtde = s.Sum(u => u.BOOKED.quantity),
                                price = s.Max(m => m.BOOKED.price)
                            })
                            .ToList();

                        #endregion

                        #region Dados Custom Table

                        int_commTableAdapter icTA = new int_commTableAdapter();
                        Data.CHICDataSet.int_commDataTable icDT = new Data.CHICDataSet.int_commDataTable();
                        icTA.FillByOrderNo(icDT, order.orderno);

                        #endregion

                        if (icDT.Count > 0)
                        {
                            if (icDT[0].transinc)
                            {
                                #region Dados Salesman Table

                                salesmanTableAdapter slTA = new salesmanTableAdapter();
                                Data.CHICDataSet.salesmanDataTable slDT = new Data.CHICDataSet.salesmanDataTable();
                                slTA.FillByCod(slDT, order.salesrep);

                                string codigoCliente = order.cust_no.Trim();

                                if (slDT.Count == 0)
                                {
                                    custTableAdapter cTA = new custTableAdapter();
                                    Data.CHICDataSet.custDataTable cDT = new Data.CHICDataSet.custDataTable();
                                    cTA.FillByCustoNo(cDT, order.cust_no);
                                    if (cDT.Count > 0)
                                    {
                                        slTA.FillByCod(slDT, cDT.FirstOrDefault().salesman);
                                    }
                                }

                                #region Verifica Transportadora

                                string empresaTransportadoraCHIC = "TO";

                                #endregion

                                #endregion

                                foreach (var item in listaItens)
                                {
                                    #region Carrega Dados

                                    string linhagem = item.variety;
                                    string produto = item.form;

                                    int? numVeiculo = 0;
                                    string embalagem = "";
                                    string status = "Pendente";
                                    string observacao = "";
                                    int? ordem = 0;
                                    string inicioCarregamentoEsperado = "";
                                    string chegadaClienteEsperada = "";
                                    int? km = 0;
                                    string inicioCarregamentoReal = "";
                                    string chegadaClienteReal = "";
                                    string chegadaCarregamentoReal = "";
                                    int? qtdCaixas = 0;
                                    string numRoteiroEntregaFluig = "";

                                    List<Prog_Diaria_Transp_Pedidos> listProdDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                        .Where(w => w.CHICNum == order.orderno.Trim()
                                        && w.Linhagem == linhagem && w.Produto == produto
                                        //&& w.LocalNascimento == item.location).ToList();
                                        && w.CHICOrigem == "Matriz"
                                        ).ToList();

                                    #endregion

                                    string empresaTransp = empresaTransportadora;
                                    if (listProdDiariaTranspPedido.Count > 0) empresaTransp = listProdDiariaTranspPedido.FirstOrDefault().EmpresaTranportador;

                                    if ((empresaTransportadora == empresaTransp) || empresaTransportadora == "")
                                    {
                                        #region Verifica Se existe já lançado

                                        if (listProdDiariaTranspPedido.Count > 1)
                                        {
                                            foreach (var pedido in listProdDiariaTranspPedido)
                                            {
                                                numVeiculo = pedido.NumVeiculo;
                                                embalagem = pedido.Embalagem;
                                                status = pedido.Status;
                                                observacao = pedido.Observacao;
                                                ordem = pedido.Ordem;
                                                inicioCarregamentoEsperado = pedido.InicioCarregamentoEsperado;
                                                chegadaClienteEsperada = pedido.ChegadaClienteEsperado;
                                                km = pedido.KM;
                                                inicioCarregamentoReal = pedido.InicioCarregamentoReal;
                                                chegadaCarregamentoReal = pedido.ChegadaClienteReal;
                                                qtdCaixas = pedido.QuantidadeCaixa;
                                                numRoteiroEntregaFluig = pedido.NumRoteiroEntregaFluig;

                                                hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(pedido);
                                            }
                                        }

                                        hlbapp.SaveChanges();

                                        Prog_Diaria_Transp_Pedidos prodDiariaTranspPedido = hlbapp.Prog_Diaria_Transp_Pedidos
                                            .Where(w => w.CHICNum == order.orderno.Trim()
                                            && w.Linhagem == linhagem && w.Produto == produto
                                            //&& w.LocalNascimento == item.location).FirstOrDefault();
                                            && w.CHICOrigem == "Matriz"
                                            ).FirstOrDefault();

                                        if (prodDiariaTranspPedido != null)
                                        {
                                            numVeiculo = prodDiariaTranspPedido.NumVeiculo;
                                            embalagem = prodDiariaTranspPedido.Embalagem;
                                            status = prodDiariaTranspPedido.Status;
                                            observacao = prodDiariaTranspPedido.Observacao;
                                            ordem = prodDiariaTranspPedido.Ordem;
                                            inicioCarregamentoEsperado = prodDiariaTranspPedido.InicioCarregamentoEsperado;
                                            chegadaClienteEsperada = prodDiariaTranspPedido.ChegadaClienteEsperado;
                                            km = prodDiariaTranspPedido.KM;
                                            inicioCarregamentoReal = prodDiariaTranspPedido.InicioCarregamentoReal;
                                            chegadaClienteReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                            chegadaCarregamentoReal = prodDiariaTranspPedido.ChegadaClienteReal;
                                            qtdCaixas = prodDiariaTranspPedido.QuantidadeCaixa;
                                            numRoteiroEntregaFluig = prodDiariaTranspPedido.NumRoteiroEntregaFluig;

                                            hlbapp.Prog_Diaria_Transp_Pedidos.DeleteObject(prodDiariaTranspPedido);
                                        }

                                        #endregion

                                        #region Insere Programação Diária

                                        prodDiariaTranspPedido = new Prog_Diaria_Transp_Pedidos();
                                        prodDiariaTranspPedido.CHICOrigem = "Matriz";
                                        prodDiariaTranspPedido.NumVeiculo = numVeiculo;
                                        prodDiariaTranspPedido.Embalagem = embalagem;
                                        prodDiariaTranspPedido.Status = status;
                                        prodDiariaTranspPedido.Observacao = observacao;
                                        prodDiariaTranspPedido.Ordem = ordem;
                                        prodDiariaTranspPedido.InicioCarregamentoEsperado = inicioCarregamentoEsperado;
                                        prodDiariaTranspPedido.ChegadaClienteEsperado = chegadaClienteEsperada;
                                        prodDiariaTranspPedido.KM = km;
                                        prodDiariaTranspPedido.InicioCarregamentoReal = inicioCarregamentoReal;
                                        prodDiariaTranspPedido.ChegadaClienteReal = chegadaClienteReal;
                                        prodDiariaTranspPedido.NumRoteiroEntregaFluig = numRoteiroEntregaFluig;

                                        prodDiariaTranspPedido.DataProgramacao = bDTCommercial[0].cal_date;
                                        prodDiariaTranspPedido.CodigoCliente = order.cust_no.Trim();

                                        ENTIDADE entidade = apolo.ENTIDADE.Where(w => w.EntCod == prodDiariaTranspPedido.CodigoCliente)
                                            .FirstOrDefault();

                                        if (entidade != null)
                                        {
                                            if (entidade.EntNome.Length > 15)
                                                prodDiariaTranspPedido.NomeCliente = entidade.EntNome.Substring(0, 15) + "...";
                                            else
                                                prodDiariaTranspPedido.NomeCliente = entidade.EntNome;
                                        }
                                        prodDiariaTranspPedido.Quantidade = Convert.ToInt32(item.qtde);

                                        #region Local de Entrega

                                        if (order.contact_no != 0)
                                        {
                                            shippingTableAdapter sTA = new shippingTableAdapter();
                                            Data.CHICDataSet.shippingDataTable sDT = new Data.CHICDataSet.shippingDataTable();
                                            sTA.FillByCustNo(sDT, order.cust_no);

                                            if (sDT.Count > 0)
                                            {
                                                Data.CHICDataSet.shippingRow enderecoEntrega = sDT
                                                    .Where(w => w.contact_no == order.contact_no).FirstOrDefault();

                                                if (enderecoEntrega != null)
                                                {
                                                    prodDiariaTranspPedido.LocalEntrega =
                                                        enderecoEntrega.address2.Trim() + " - " + enderecoEntrega.address3.Trim();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (entidade != null)
                                            {
                                                CIDADE cidade = apolo.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                                                //prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla + " / " +
                                                //    cidade.PaisSigla;
                                                prodDiariaTranspPedido.LocalEntrega = cidade.CidNomeComp + " / " + cidade.UfSigla;
                                            }
                                        }

                                        #endregion

                                        prodDiariaTranspPedido.Produto = produto;
                                        prodDiariaTranspPedido.Linhagem = linhagem;
                                        prodDiariaTranspPedido.ValorTotal = item.qtde * item.price;
                                        prodDiariaTranspPedido.CHICNum = order.orderno.Trim();
                                        prodDiariaTranspPedido.LocalNascimento = item.location.Trim();

                                        if (entidade != null)
                                        {
                                            ENT_FONE fone = apolo.ENT_FONE.Where(w => w.EntCod == entidade.EntCod)
                                                .OrderBy(o => o.EntFoneSeq).FirstOrDefault();
                                            if (fone != null)
                                            {
                                                prodDiariaTranspPedido.TelefoneCliente = fone.EntFoneDDD + fone.EntFoneNum;
                                            }
                                        }

                                        prodDiariaTranspPedido.DataEntrega = order.del_date;
                                        prodDiariaTranspPedido.CodigoRepresentante = order.salesrep.Trim();

                                        if (icDT.Count > 0)
                                            prodDiariaTranspPedido.ObservacaoCHIC = icDT[0].hatchinf.Trim();
                                        else
                                            prodDiariaTranspPedido.ObservacaoCHIC = "";

                                        #region Debicagem

                                        int existeDebicagem = bDTCommercial.Where(w => w.item == "169").Count();
                                        if (existeDebicagem > 0)
                                            prodDiariaTranspPedido.Debicagem = "X";
                                        else
                                            prodDiariaTranspPedido.Debicagem = "";

                                        #endregion

                                        if (slDT[0].salesman.Trim().Length > 15)
                                            prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim().Substring(0, 15) + "...";
                                        else
                                            prodDiariaTranspPedido.NomeRepresentante = slDT[0].salesman.Trim();

                                        #region Verifica Transportadora

                                        prodDiariaTranspPedido.EmpresaTranportador = empresaTransportadoraCHIC;

                                        #endregion

                                        prodDiariaTranspPedido.Empresa = slDT[0].inv_comp.Trim();

                                        hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(prodDiariaTranspPedido);

                                        #endregion
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    hlbapp.SaveChanges();

                    #endregion

                    #region Insere Veículos da Programação Diária de Transporte

                    AtualizaValoresVeiculos(data, origem);

                    #endregion

                    #region Integra Embarcador (DESATIVADO)

                    //HLBAPPEntities1 hlbapp1 = new HLBAPPEntities1();

                    //var listaPedidos = hlbapp1.Prog_Diaria_Transp_Pedidos
                    //    .Where(w => w.EmpresaTranportador == empresaTransportadora
                    //        && w.DataProgramacao == data)
                    //    .OrderBy(o => o.NumVeiculo).ThenBy(t => t.Ordem)
                    //    .ToList();

                    //foreach (var item in listaPedidos)
                    //{
                    //    string retornoEmbarcador = IntegraPedidoEmbarcador(item.ID, Convert.ToDateTime(txtDataProgramacao.Text));
                    //    if (retornoEmbarcador != "")
                    //    {
                    //        lblMensagem2.Visible = true;
                    //        lblMensagem2.Text = retornoEmbarcador;
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        lblMensagem2.Visible = false;
                    //        lblMensagem2.Text = "";
                    //    }
                    //}

                    #endregion
                }

                return "";
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                retorno = "Erro linha: " + linenum.ToString();
                retorno = retorno + " / Erro 01: " + ex.Message;
                if (ex.InnerException != null)
                    retorno = retorno + " / Erro 02: " + ex.InnerException.Message;

                return retorno;
            }
        }

        public void InsereLOG_Prog_Diaria_Transp_Pedidos(Models.HLBAPP.Prog_Diaria_Transp_Pedidos pdtp, string operacao, string observacao)
        {
            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();

            Models.HLBAPP.LOG_Prog_Diaria_Transp_Pedidos log = new Models.HLBAPP.LOG_Prog_Diaria_Transp_Pedidos();
            log.DataHora = DateTime.Now;
            log.Usuario = Session["login"].ToString().ToUpper();
            log.Operacao = operacao;
            log.ObsLog = observacao;
            log.DataProgramacao = pdtp.DataProgramacao;
            log.CodigoCliente = pdtp.CodigoCliente;
            log.NomeCliente = pdtp.NomeCliente;
            log.NumVeiculo = pdtp.NumVeiculo;
            log.Quantidade = pdtp.Quantidade;
            log.LocalEntrega = pdtp.LocalEntrega;
            log.Produto = pdtp.Produto;
            log.Linhagem = pdtp.Linhagem;
            log.Embalagem = pdtp.Embalagem;
            log.ValorTotal = pdtp.ValorTotal;
            log.NFEspecie = pdtp.NFEspecie;
            log.NFSerie = pdtp.NFSerie;
            log.NFNum = pdtp.NFNum;
            log.CHICNum = pdtp.CHICNum;
            log.LocalNascimento = pdtp.LocalNascimento;
            log.TelefoneCliente = pdtp.TelefoneCliente;
            log.InicioCarregamentoEsperado = pdtp.InicioCarregamentoEsperado;
            log.DataEntrega = pdtp.DataEntrega;
            log.ChegadaClienteEsperado = pdtp.ChegadaClienteEsperado;
            log.KM = pdtp.KM;
            log.CodigoRepresentante = pdtp.CodigoRepresentante;
            log.NomeRepresentante = pdtp.NomeRepresentante;
            log.InicioCarregamentoReal = pdtp.InicioCarregamentoReal;
            log.ChegadaClienteReal = pdtp.ChegadaClienteReal;
            log.Observacao = pdtp.Observacao;
            log.Status = pdtp.Status;
            log.ObservacaoCHIC = pdtp.ObservacaoCHIC;
            log.Debicagem = pdtp.Debicagem;
            log.Ordem = pdtp.Ordem;
            log.EmpresaTranportador = pdtp.EmpresaTranportador;
            log.Empresa = pdtp.Empresa;
            log.QuantidadeCaixa = pdtp.QuantidadeCaixa;
            log.CHICOrigem = pdtp.CHICOrigem;
            log.NumRoteiroEntregaFluig = pdtp.NumRoteiroEntregaFluig;
            log.DataChegadaClienteReal = pdtp.DataChegadaClienteReal;
            log.IDProgDiariaTranspPedidos = pdtp.ID;
            log.QtdeVendida = pdtp.QtdeVendida;
            log.QtdeBonificada = pdtp.QtdeBonificada;
            log.QtdeReposicao = pdtp.QtdeReposicao;
            log.QtdeSobra = pdtp.QtdeSobra;
            log.MotivoSobra = pdtp.MotivoSobra;
            log.CHICNumReposicao = pdtp.CHICNumReposicao;
            log.MotivoReposicao = pdtp.MotivoReposicao;
            log.IDPedidoVenda = pdtp.IDPedidoVenda;
            log.EnderEntSeq = pdtp.EnderEntSeq;
            log.PercBonificacao = pdtp.PercBonificacao;
            log.PrecoProduto = pdtp.PrecoProduto;
            log.ObsProgramacao = pdtp.ObsProgramacao;
            log.CondicaoPagamento = pdtp.CondicaoPagamento;

            hlbappSession.LOG_Prog_Diaria_Transp_Pedidos.AddObject(log);
            hlbappSession.SaveChanges();
        }

        protected void imgbRefreshCHIC_Click(object sender, ImageClickEventArgs e)
        {
            DateTime data = Convert.ToDateTime(txtDataProgramacao.Text);
            string empresa = Session["empresa"].ToString();

            List<Prog_Diaria_Transp_Pedidos> listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                .Where(w => w.Status == "Preenchido" && w.DataProgramacao == data
                    //&& w.Produto.Substring(0,1) != "H"
                    && empresa.IndexOf(w.Empresa) != -1
                    && w.EmpresaTranportador == ddlEmpresaTransportadora.SelectedValue
                    && w.CHICNum != null && w.CHICNum != "").ToList();

            foreach (var item in listaPedidos)
            {
                #region Pedidos do CHIC Commercial

                bookedTableAdapter bTA = new bookedTableAdapter();
                Data.CHICDataSet.bookedDataTable bDT = new Data.CHICDataSet.bookedDataTable();
                bTA.FillByOrderNo(bDT, item.CHICNum);

                if (bDT.Count > 0)
                {
                    string location = bDT.Min(m => m.location);

                    DateTime dataCaldate = bDT[0].cal_date;
                    string tipoProduto = " PINTOS";
                    if (item.Produto.Substring(0, 1) == "H")
                    {
                        dataCaldate = dataCaldate.AddDays(-21);
                        tipoProduto = " OVOS";
                    }

                    #region Atualiza Transema

                    if (ddlEmpresaTransportadora.SelectedValue.Equals("TR")
                        && item.Produto.Substring(0, 1) != "H")
                    {
                        Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                                .Where(w => w.DataProgramacao == item.DataProgramacao
                                    && w.NumVeiculo == item.NumVeiculo
                                    && w.EmpresaTranportador == ddlEmpresaTransportadora.SelectedValue)
                                .FirstOrDefault();

                        if (progVeiculo != null)
                        {
                            #region Atualiza Pintos p/ Caixa

                            Data.CHICDataSet.bookedRow pintosPorCaixa = bDT.Where(w => w.item == "600").FirstOrDefault();

                            if (pintosPorCaixa != null)
                            {
                                decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                                bTA.UpdateQuantity(qtdPorCaixa, pintosPorCaixa.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                ////tables.FillByName(tablesDT, "booked");
                                //tables.FillByName(tablesDT, "booked");
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, bDT[0].cal_date, bDT[0].customer, "600",
                                    Convert.ToInt32(progVeiculo.QuantidadePorCaixa), 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Embalagem

                            bTA.FillByOrderNo(bDT, item.CHICNum);

                            string codItemEmbalagem = "";
                            if (item.Embalagem.Equals("PL")) codItemEmbalagem = "601";
                            if (item.Embalagem.Equals("PA")) codItemEmbalagem = "602";

                            Data.CHICDataSet.bookedRow embalagem = bDT
                                .Where(w => w.item == "601" || w.item == "602").FirstOrDefault();

                            if (codItemEmbalagem != "")
                            {
                                if (embalagem != null)
                                {
                                    if ((embalagem.item.Equals("601") && !item.Embalagem.Equals("PL"))
                                        ||
                                        (embalagem.item.Equals("602") && !item.Embalagem.Equals("PA"))
                                        )
                                    {
                                        bTA.UpdateItem(codItemEmbalagem, embalagem.book_id);
                                    }
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, bDT[0].cal_date, bDT[0].customer, codItemEmbalagem,
                                        0, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Número do Caminhão

                            bTA.FillByOrderNo(bDT, item.CHICNum);

                            Data.CHICDataSet.bookedRow numCaminhao = bDT.Where(w => w.item == "650").FirstOrDefault();

                            if (numCaminhao != null)
                            {
                                decimal numVeiculo = Convert.ToDecimal(item.NumVeiculo);

                                bTA.UpdateQuantity(numVeiculo, numCaminhao.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, bDT[0].cal_date, bDT[0].customer, "650",
                                    Convert.ToInt32(item.NumVeiculo), 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            item.Status = "Conferido";
                        }
                    }

                    #endregion

                    #region Atualiza Planalto

                    if (ddlEmpresaTransportadora.SelectedValue.Equals("PL"))
                    {
                        Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                            .Where(w => w.DataProgramacao == item.DataProgramacao
                                && w.NumVeiculo == item.NumVeiculo
                                && w.EmpresaTranportador == ddlEmpresaTransportadora.SelectedValue)
                            .FirstOrDefault();

                        if (progVeiculo != null)
                        {
                            #region Atualiza Pintos p/ Caixa

                            if (item.Embalagem.Equals("PL") || item.Embalagem.Equals("PA"))
                            {
                                decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                                Data.CHICDataSet.bookedRow pintosPorCaixa = bDT.Where(w => w.item == "600").FirstOrDefault();
                                string textoQtdPintosPorCaixa = qtdPorCaixa.ToString()
                                    + " PINTOS P/ CX";

                                if (pintosPorCaixa != null)
                                {
                                    bTA.UpdateAltDesc(textoQtdPintosPorCaixa, pintosPorCaixa.book_id);
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, bDT[0].cal_date, bDT[0].customer, "600",
                                        0, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", textoQtdPintosPorCaixa, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Ovos p/ Caixa

                            if (item.Embalagem.Equals("CX"))
                            {
                                decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                                Data.CHICDataSet.bookedRow ovosPorCaixa = bDT.Where(w => w.item == "604").FirstOrDefault();
                                string textoQtdOvosPorCaixa = qtdPorCaixa.ToString()
                                    + " OVOS P/ CX";

                                if (ovosPorCaixa != null)
                                {
                                    bTA.UpdateAltDesc(textoQtdOvosPorCaixa, ovosPorCaixa.book_id);
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, bDT[0].cal_date, bDT[0].customer, "604",
                                        0, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", textoQtdOvosPorCaixa, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Embalagem

                            bTA.FillByOrderNo(bDT, item.CHICNum);

                            string codItemEmbalagem = "";
                            if (item.Embalagem.Equals("PL")) codItemEmbalagem = "601";
                            if (item.Embalagem.Equals("PA")) codItemEmbalagem = "602";
                            if (item.Embalagem.Equals("CX")) codItemEmbalagem = "606";

                            var listaItensPedidoCHIC = hlbapp.Prog_Diaria_Transp_Pedidos
                                .Where(w => w.CHICNum == item.CHICNum).ToList();

                            //int qtdCaixas = Convert.ToInt32(item.QuantidadeCaixa);
                            int qtdCaixas = Convert.ToInt32(listaItensPedidoCHIC.Sum(s => s.QuantidadeCaixa));

                            Data.CHICDataSet.bookedRow embalagem = bDT
                                .Where(w => w.item == "601" || w.item == "602" || w.item == "606").FirstOrDefault();

                            if (codItemEmbalagem != "")
                            {
                                if (embalagem != null)
                                {
                                    if ((embalagem.item.Equals("601") && !item.Embalagem.Equals("PL"))
                                        ||
                                        (embalagem.item.Equals("602") && !item.Embalagem.Equals("PA"))
                                        ||
                                        (embalagem.item.Equals("606") && !item.Embalagem.Equals("CX"))
                                        ||
                                        (embalagem.quantity != qtdCaixas)
                                        )
                                    {
                                        bTA.UpdateItem(codItemEmbalagem, embalagem.book_id);
                                        bTA.UpdateQuantity(qtdCaixas, embalagem.book_id);
                                    }
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, dataCaldate, bDT[0].customer, codItemEmbalagem,
                                        qtdCaixas, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Placa do Caminhão

                            Data.CHICDataSet.bookedRow placaCaminhao = bDT.Where(w => w.item == "360").FirstOrDefault();
                            string textoPlacaCaminhao = "PLACA DO CAMINHÃO: " + progVeiculo.Placa;

                            if (placaCaminhao != null)
                            {
                                bTA.UpdateAltDesc(textoPlacaCaminhao, placaCaminhao.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "360",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoPlacaCaminhao, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Transportador

                            Data.CHICDataSet.bookedRow transportadora = bDT.Where(w => w.item == "361").FirstOrDefault();
                            string textoTransportadora = "TRANSPORTADORA: " + progVeiculo.Tranportadora;

                            if (transportadora != null)
                            {
                                bTA.UpdateAltDesc(textoTransportadora, transportadora.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "361",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoTransportadora, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Nº Carga

                            Data.CHICDataSet.bookedRow numeroCarga = bDT.Where(w => w.item == "362").FirstOrDefault();
                            string textoNumeroCarga = "Nº DA CARGA: " + progVeiculo.NumVeiculo;

                            if (numeroCarga != null)
                            {
                                bTA.UpdateAltDesc(textoNumeroCarga, numeroCarga.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "362",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoNumeroCarga, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Horário de Carregamento

                            Data.CHICDataSet.bookedRow horaCarregamento = bDT.Where(w => w.item == "363").FirstOrDefault();
                            string textoHoraCarregamento = "HORA CARREG.: " + item.InicioCarregamentoEsperado;

                            if (horaCarregamento != null)
                            {
                                bTA.UpdateAltDesc(textoHoraCarregamento, horaCarregamento.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "363",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoHoraCarregamento, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Data do Carregamento

                            Data.CHICDataSet.bookedRow dataCarregamento = bDT.Where(w => w.item == "364").FirstOrDefault();
                            string textoDataCarregamento = "DATA CARREG.: "
                                + Convert.ToDateTime(item.DataProgramacao).ToString("dd/MM/yy");

                            if (dataCarregamento != null)
                            {
                                bTA.UpdateAltDesc(textoDataCarregamento, dataCarregamento.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "364",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoDataCarregamento, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Data de Chegada

                            Data.CHICDataSet.bookedRow dataChegada = bDT.Where(w => w.item == "366").FirstOrDefault();
                            string textoDataChegada = "DATA ENTREGA: "
                                + Convert.ToDateTime(item.DataEntrega).ToString("dd/MM/yy");

                            if (dataChegada != null)
                            {
                                bTA.UpdateAltDesc(textoDataChegada, dataChegada.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "366",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoDataChegada, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza KM Previsto

                            Data.CHICDataSet.bookedRow km = bDT.Where(w => w.item == "367").FirstOrDefault();
                            string textoKM = "KM PREVISTO: " + item.KM;

                            if (km != null)
                            {
                                bTA.UpdateAltDesc(textoKM, km.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "367",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoKM, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Valor p/ KM

                            Data.CHICDataSet.bookedRow valorpkm = bDT.Where(w => w.item == "370").FirstOrDefault();
                            string textoValorpkm = "VALOR P/ KM: "
                                + String.Format("{0:c}", Convert.ToDecimal(progVeiculo.ValorKM));

                            if (valorpkm != null)
                            {
                                bTA.UpdateAltDesc(textoValorpkm, valorpkm.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "370",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoValorpkm, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Valor Total Frete

                            Data.CHICDataSet.bookedRow valorTotalFrete = bDT.Where(w => w.item == "371").FirstOrDefault();
                            string textoValorTotalFrete = "VALOR TOTAL FRETE: "
                                + String.Format("{0:c}", Convert.ToDecimal(progVeiculo.ValorTotal));

                            if (valorTotalFrete != null)
                            {
                                bTA.UpdateAltDesc(textoValorTotalFrete, valorTotalFrete.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "371",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoValorTotalFrete, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            item.Status = "Conferido";
                        }
                    }

                    #endregion

                    #region Atualiza H&N ou Exportação

                    if (ddlEmpresaTransportadora.SelectedValue.Equals("HN") || ddlEmpresaTransportadora.SelectedValue.Equals("EX"))
                    {
                        Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                                .Where(w => w.DataProgramacao == item.DataProgramacao
                                    && w.NumVeiculo == item.NumVeiculo
                                    && w.EmpresaTranportador == ddlEmpresaTransportadora.SelectedValue)
                                .FirstOrDefault();

                        if (progVeiculo != null)
                        {
                            #region Atualiza Pintos p/ Caixa

                            decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                            Data.CHICDataSet.bookedRow pintosPorCaixa = bDT.Where(w => w.item == "600").FirstOrDefault();
                            string textoQtdPintosPorCaixa = qtdPorCaixa.ToString()
                                + tipoProduto + " P/ CX";

                            if (pintosPorCaixa != null)
                            {
                                bTA.UpdateAltDesc(textoQtdPintosPorCaixa, pintosPorCaixa.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "600",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoQtdPintosPorCaixa, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Embalagem

                            bTA.FillByOrderNo(bDT, item.CHICNum);

                            string codItemEmbalagem = "";
                            if (item.Embalagem.Equals("PL")) codItemEmbalagem = "601";
                            if (item.Embalagem.Equals("PA")) codItemEmbalagem = "602";

                            var listaItensPedidoCHIC = hlbapp.Prog_Diaria_Transp_Pedidos
                                .Where(w => w.CHICNum == item.CHICNum).ToList();

                            //int qtdCaixas = Convert.ToInt32(item.QuantidadeCaixa);
                            int qtdCaixas = Convert.ToInt32(listaItensPedidoCHIC.Sum(s => s.QuantidadeCaixa));

                            Data.CHICDataSet.bookedRow embalagem = bDT
                                .Where(w => w.item == "601" || w.item == "602").FirstOrDefault();

                            if (codItemEmbalagem != "")
                            {
                                if (embalagem != null)
                                {
                                    if ((embalagem.item.Equals("601") && !item.Embalagem.Equals("PL"))
                                        ||
                                        (embalagem.item.Equals("602") && !item.Embalagem.Equals("PA"))
                                        ||
                                        (embalagem.quantity != qtdCaixas)
                                        )
                                    {
                                        bTA.UpdateItem(codItemEmbalagem, embalagem.book_id);
                                        bTA.UpdateQuantity(qtdCaixas, embalagem.book_id);
                                    }
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, dataCaldate, bDT[0].customer, codItemEmbalagem,
                                        qtdCaixas, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Placa do Caminhão

                            Data.CHICDataSet.bookedRow placaCaminhao = bDT.Where(w => w.item == "360").FirstOrDefault();
                            string textoPlacaCaminhao = "PLACA DO CAMINHÃO: " + progVeiculo.Placa;

                            if (placaCaminhao != null)
                            {
                                bTA.UpdateAltDesc(textoPlacaCaminhao, placaCaminhao.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "360",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoPlacaCaminhao, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Transportador

                            Data.CHICDataSet.bookedRow transportadora = bDT.Where(w => w.item == "361").FirstOrDefault();
                            string textoTransportadora = "TRANSPORTADORA: " + progVeiculo.Tranportadora;

                            if (transportadora != null)
                            {
                                bTA.UpdateAltDesc(textoTransportadora, transportadora.book_id);
                            }
                            else
                            {
                                //tablesTableAdapter tables = new tablesTableAdapter();
                                //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "361",
                                    0, 0, bDT[0].orderno,
                                    "O", "", "", "", location, "", textoTransportadora, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            if (ddlEmpresaTransportadora.SelectedValue.Equals("EX"))
                            {
                                #region Atualiza Aeroporto / Despachante

                                Data.CHICDataSet.bookedRow aereo = bDT.Where(w => w.item == "676").FirstOrDefault();
                                string textoAereo = "AEREO: " + progVeiculo.AeroportoOrigem + " / " + progVeiculo.Despachante;

                                if (aereo != null)
                                {
                                    bTA.UpdateAltDesc(textoAereo, aereo.book_id);
                                }
                                else
                                {
                                    //tablesTableAdapter tables = new tablesTableAdapter();
                                    //Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bTA.Insert(booked_id, dataCaldate, bDT[0].customer, "676",
                                        0, 0, bDT[0].orderno,
                                        "O", "", "", "", location, "", textoAereo, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bDT[0].itm_ddate, 0, bDT[0].salesrep, bDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }

                                #endregion

                                #region Observacao Pedido - DESATIVADO POIS NÃO É UTILIZADO NO CHIC COMERCIAL

                                if (item.Observacao != null)
                                {
                                    if (item.Observacao.Length > 0)
                                    {
                                        ordersTableAdapter oTA = new ordersTableAdapter();
                                        Data.CHICDataSet.ordersDataTable oDT = new Data.CHICDataSet.ordersDataTable();
                                        oTA.FillByOrderNo(oDT, item.CHICNum);

                                        Data.CHICDataSet.ordersRow oR = oDT.FirstOrDefault();

                                        #region Carrega Observacao no campos do CHIC

                                        string com1 = "";
                                        string com2 = "";
                                        string com3 = "";

                                        int cont = 0;

                                        while (cont <= item.Observacao.Length)
                                        {
                                            int final = 80;
                                            if ((item.Observacao.Length - cont) < 80)
                                                final = (item.Observacao.Length - cont);

                                            if (com1 == "") com1 = item.Observacao.Substring(cont, final);
                                            if (com2 == "") com2 = item.Observacao.Substring(cont, final);
                                            if (com3 == "") com3 = item.Observacao.Substring(cont, final);

                                            cont = cont + 80;
                                        }

                                        #endregion

                                        if (oR != null)
                                        {
                                            oTA.UpdateQuery(oR.order_date, oR.cust_no, oR.del_date, com1, com2, com3, oR.salesrep, oR.orderno);
                                        }
                                    }
                                }

                                #endregion
                            }

                            item.Status = "Conferido";
                        }
                    }

                    #endregion
                }

                #endregion

                #region Pedidos do CHIC Parent

                bookedParentTableAdapter bpTA = new bookedParentTableAdapter();
                CHICParentDataSet.bookedParentDataTable bpDT = new CHICParentDataSet.bookedParentDataTable();
                bpTA.FillByOrderNo(bpDT, item.CHICNum);

                if (bpDT.Count > 0)
                {
                    string location = bpDT.Min(m => m.location);

                    DateTime dataCaldate = bpDT[0].cal_date;
                    string tipoProduto = "";
                    if (item.Produto.Substring(0, 1) == "H")
                    {
                        dataCaldate = dataCaldate.AddDays(-21);
                        tipoProduto = " OVOS";
                    }

                    #region Atualiza Exportação e Alojamento Interno

                    if (ddlEmpresaTransportadora.SelectedValue.Equals("EX") || ddlEmpresaTransportadora.SelectedValue.Equals("AI"))
                    {
                        Prog_Diaria_Transp_Veiculos progVeiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                            .Where(w => w.DataProgramacao == item.DataProgramacao
                                && w.NumVeiculo == item.NumVeiculo
                                && w.EmpresaTranportador == ddlEmpresaTransportadora.SelectedValue)
                            .FirstOrDefault();

                        if (progVeiculo != null)
                        {
                            #region Atualiza Pintos p/ Caixa

                            if (item.Embalagem.Equals("PL") || item.Embalagem.Equals("PA"))
                            {
                                decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                                CHICParentDataSet.bookedParentRow pintosPorCaixa = bpDT.Where(w => w.item == "600").FirstOrDefault();
                                string textoQtdPintosPorCaixa = qtdPorCaixa.ToString()
                                    + tipoProduto + " P/ CX";

                                if (pintosPorCaixa != null)
                                {
                                    bpTA.UpdateAltDesc(textoQtdPintosPorCaixa, pintosPorCaixa.book_id);
                                }
                                else
                                {
                                    //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                    //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                    //tables.FillByName(tablesDT, "booked");
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "600",
                                        0, 0, bpDT[0].orderno,
                                        "O", "", "", "", location, "", textoQtdPintosPorCaixa, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Ovos p/ Caixa

                            if (item.Embalagem.Equals("CX"))
                            {
                                decimal qtdPorCaixa = Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);

                                CHICParentDataSet.bookedParentRow ovosPorCaixa = bpDT.Where(w => w.item == "604").FirstOrDefault();
                                string textoQtdOvosPorCaixa = qtdPorCaixa.ToString()
                                    + " OVOS P/ CX";

                                if (ovosPorCaixa != null)
                                {
                                    bpTA.UpdateAltDesc(textoQtdOvosPorCaixa, ovosPorCaixa.book_id);
                                }
                                else
                                {
                                    //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                    //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "604",
                                        0, 0, bpDT[0].orderno,
                                        "O", "", "", "", location, "", textoQtdOvosPorCaixa, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Embalagem

                            bpTA.FillByOrderNo(bpDT, item.CHICNum);

                            string codItemEmbalagem = "";
                            if (item.Embalagem.Equals("PL")) codItemEmbalagem = "692";
                            if (item.Embalagem.Equals("PA")) codItemEmbalagem = "670";
                            if (item.Embalagem.Equals("CX")) codItemEmbalagem = "692";

                            var listaItensPedidoCHIC = hlbapp.Prog_Diaria_Transp_Pedidos
                                .Where(w => w.CHICNum == item.CHICNum).ToList();

                            //int qtdCaixas = Convert.ToInt32(item.QuantidadeCaixa);
                            int qtdCaixas = Convert.ToInt32(listaItensPedidoCHIC.Sum(s => s.QuantidadeCaixa));

                            CHICParentDataSet.bookedParentRow embalagem = bpDT
                                .Where(w => w.item == "692" || w.item == "670").FirstOrDefault();

                            if (codItemEmbalagem != "")
                            {
                                if (embalagem != null)
                                {
                                    if ((embalagem.item.Equals("692") && !item.Embalagem.Equals("PL"))
                                        ||
                                        (embalagem.item.Equals("670") && !item.Embalagem.Equals("PA"))
                                        ||
                                        (embalagem.quantity != qtdCaixas)
                                        )
                                    {
                                        bpTA.UpdateItem(codItemEmbalagem, embalagem.book_id);
                                        bpTA.UpdateQuantity(qtdCaixas, embalagem.book_id);
                                    }
                                }
                                else
                                {
                                    //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                    //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, codItemEmbalagem,
                                        qtdCaixas, 0, bpDT[0].orderno,
                                        "O", "", "", "", location, "", "", itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }
                            }

                            #endregion

                            #region Atualiza Placa do Caminhão

                            CHICParentDataSet.bookedParentRow placaCaminhao = bpDT.Where(w => w.item == "678").FirstOrDefault();
                            string textoPlacaCaminhao = "PLACA DO CAMINHÃO: " + progVeiculo.Placa;

                            if (placaCaminhao != null)
                            {
                                bpTA.UpdateAltDesc(textoPlacaCaminhao, placaCaminhao.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "678",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoPlacaCaminhao, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Transportador

                            CHICParentDataSet.bookedParentRow transportadora = bpDT.Where(w => w.item == "361").FirstOrDefault();
                            string textoTransportadora = "TRANSPORTADORA: " + progVeiculo.Tranportadora;

                            if (transportadora != null)
                            {
                                bpTA.UpdateAltDesc(textoTransportadora, transportadora.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "361",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoTransportadora, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Nº Carga

                            CHICParentDataSet.bookedParentRow numeroCarga = bpDT.Where(w => w.item == "650").FirstOrDefault();
                            string textoNumeroCarga = "Nº DA CARGA: " + progVeiculo.NumVeiculo;

                            if (numeroCarga != null)
                            {
                                bpTA.UpdateAltDesc(textoNumeroCarga, numeroCarga.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "650",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoNumeroCarga, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Horário de Carregamento

                            CHICParentDataSet.bookedParentRow horaCarregamento = bpDT.Where(w => w.item == "679").FirstOrDefault();
                            string textoHoraCarregamento = "HORA CARREG.: " + item.InicioCarregamentoEsperado;

                            if (horaCarregamento != null)
                            {
                                bpTA.UpdateAltDesc(textoHoraCarregamento, horaCarregamento.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "679",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoHoraCarregamento, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Data do Carregamento

                            CHICParentDataSet.bookedParentRow dataCarregamento = bpDT.Where(w => w.item == "364").FirstOrDefault();
                            string textoDataCarregamento = "DATA CARREG.: "
                                + Convert.ToDateTime(item.DataProgramacao).ToString("dd/MM/yy");

                            if (dataCarregamento != null)
                            {
                                bpTA.UpdateAltDesc(textoDataCarregamento, dataCarregamento.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "364",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoDataCarregamento, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            #region Atualiza Data de Chegada

                            CHICParentDataSet.bookedParentRow dataChegada = bpDT.Where(w => w.item == "366").FirstOrDefault();
                            string textoDataChegada = "DATA ENTREGA: "
                                + Convert.ToDateTime(item.DataEntrega).ToString("dd/MM/yy");

                            if (dataChegada != null)
                            {
                                bpTA.UpdateAltDesc(textoDataChegada, dataChegada.book_id);
                            }
                            else
                            {
                                //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                //tables.FillByName(tablesDT, "booked");;
                                //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                string itemOrd = "";
                                if (item_ord.ToString().Length == 1)
                                    itemOrd = "0" + item_ord;
                                else
                                    itemOrd = item_ord.ToString();

                                bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "366",
                                    0, 0, bpDT[0].orderno,
                                    "O", "", "", "", location, "", textoDataChegada, itemOrd, "web", DateTime.Today,
                                    "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                            }

                            #endregion

                            if (ddlEmpresaTransportadora.SelectedValue.Equals("EX"))
                            {
                                #region Atualiza Aeroporto / Despachante

                                CHICParentDataSet.bookedParentRow aereo = bpDT.Where(w => w.item == "676").FirstOrDefault();
                                string textoAereo = "AEREO: " + progVeiculo.AeroportoOrigem + " / " + progVeiculo.Despachante;

                                if (aereo != null)
                                {
                                    bpTA.UpdateAltDesc(textoAereo, aereo.book_id);
                                }
                                else
                                {
                                    //tablesParentTableAdapter tables = new tablesParentTableAdapter();
                                    //CHICParentDataSet.tablesParentDataTable tablesDT = new CHICParentDataSet.tablesParentDataTable();
                                    //tables.FillByName(tablesDT, "booked");;
                                    //int booked_id = Convert.ToInt32(tablesDT[0].lastno) + 1;
                                    int booked_id = Convert.ToInt32(NextCodeCHIC("booked"));

                                    int item_ord = Convert.ToInt32(bpDT.Max(m => m.item_ord)) + 1;
                                    string itemOrd = "";
                                    if (item_ord.ToString().Length == 1)
                                        itemOrd = "0" + item_ord;
                                    else
                                        itemOrd = item_ord.ToString();

                                    bpTA.Insert(booked_id, dataCaldate, bpDT[0].customer, "676",
                                        0, 0, bpDT[0].orderno,
                                        "O", "", "", "", location, "", textoAereo, itemOrd, "web", DateTime.Today,
                                        "web", DateTime.Today, bpDT[0].itm_ddate, 0, bpDT[0].salesrep, bpDT[0].bookkey);

                                    //tables.UpdateQuery(Convert.ToDecimal(booked_id), "booked");
                                }

                                #endregion

                                #region Observacao Pedido

                                if (item.Observacao != null)
                                {
                                    ordersParentTableAdapter opTA = new ordersParentTableAdapter();
                                    CHICParentDataSet.ordersParentDataTable opDT = new CHICParentDataSet.ordersParentDataTable();
                                    opTA.FillByOrderNo(opDT, item.CHICNum);

                                    CHICParentDataSet.ordersParentRow opR = opDT.FirstOrDefault();

                                    #region Carrega Observacao no campos do CHIC

                                    string delivery = "";
                                    string com1 = "";
                                    string com2 = "";
                                    string com3 = "";

                                    int cont = 0;

                                    if (item.Observacao.Length <= 80)
                                        delivery = item.Observacao;
                                    else
                                    {
                                        delivery = "OBS. DE TRANSPORTES VIDE ABAIXO.";
                                        while (cont <= item.Observacao.Length)
                                        {
                                            int final = 80;
                                            if ((item.Observacao.Length - cont) < 80)
                                                final = (item.Observacao.Length - cont);

                                            if (com1 == "") com1 = item.Observacao.Substring(cont, final);
                                            if (com2 == "") com2 = item.Observacao.Substring(cont, final);
                                            if (com3 == "") com3 = item.Observacao.Substring(cont, final);

                                            cont = cont + 80;
                                        }
                                    }

                                    #endregion

                                    if (opR != null)
                                    {
                                        opTA.UpdateQuery(opR.order_date, opR.cust_no, opR.del_date, delivery, com1, com2, com3, opR.salesrep, opR.orderno);
                                    }
                                }

                                #endregion
                            }

                            item.Status = "Conferido";
                        }
                    }

                    #endregion
                }

                #endregion
            }

            hlbapp.SaveChanges();

            GridView1.DataBind();
            lblMensagem2.Visible = true;
            lblMensagem2.Text = "Dados Atualizados no CHIC com Sucesso!";
        }

        public string NextCodeCHIC(string tableName)
        {
            tablesTableAdapter tables = new tablesTableAdapter();
            Data.CHICDataSet.tablesDataTable tablesDT = new Data.CHICDataSet.tablesDataTable();
            string code = "";

            tables.FillByName(tablesDT, tableName);
            code = (Convert.ToInt32(tablesDT[0].lastno) + 1).ToString();
            tables.UpdateQuery(Convert.ToDecimal(code), tableName);

            return code;
        }

        #endregion

        public static void Atualizacao_Tick(object sender, EventArgs e)
        {
            #region Importa Cargas para Embarcador

            ImportaCargasEmbarcador();

            #endregion

            #region Atualiza Dados da Viagem

            AtualizaDadosViagem();

            #endregion

            #region Gera os Roteiros de Entrega no Fluig

            GeraRoteirosEntregaFluig();

            #endregion
        }

        #region Embarcador

        public static string IntegraPedidoEmbarcador(int idProgDiariaTransp, DateTime dataEmbarque)
        {
            string msg = "";

            #region Carrega os dados do pedido e da carga

            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            Prog_Diaria_Transp_Pedidos pedido = hlbapp.Prog_Diaria_Transp_Pedidos
                .Where(w => w.ID == idProgDiariaTransp).FirstOrDefault();

            Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
                .Where(w => w.DataProgramacao == pedido.DataProgramacao
                    && w.EmpresaTranportador == pedido.EmpresaTranportador
                    && w.NumVeiculo == pedido.NumVeiculo)
                .FirstOrDefault();

            #endregion

            if (dataEmbarque >= DateTime.Today && pedido.Produto.Substring(0, 1) != "H")
            {
                if (carga != null && pedido.CHICNum != "" && pedido.CHICNum != null && pedido.NumVeiculo != 0)
                {
                    #region Carrega dados da carga

                    string ret = "";
                    int idCarga = 0;

                    #endregion

                    #region Carrega carga do Embarcador

                    ret = Embarcador.buscaCargaCodigo(carga.ID);
                    if (ret.Contains("Erro") && !ret.Contains("nao encontrado"))
                    {
                        return "Erro ao buscar carga: " + ret;
                    }
                    else
                    {
                        if (int.TryParse(ret, out idCarga))
                            idCarga = Convert.ToInt32(ret);
                    }

                    #endregion

                    #region Carrega Unidade Base Atual

                    var listaPedidosCarga = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.DataProgramacao == carga.DataProgramacao
                            && w.EmpresaTranportador == carga.EmpresaTranportador
                            && w.NumVeiculo == carga.NumVeiculo)
                        .ToList();

                    string unidadeBaseAtual = "";
                    DateTime dataHoraMaisCedo = new DateTime();
                    DateTime dataUltimaEntrega = new DateTime();
                    DateTime dataHoraUltimaEntrega = new DateTime();

                    foreach (var item in listaPedidosCarga)
                    {
                        DateTime dataHoraPrevistaCarregamento = new DateTime();
                        if (DateTime.TryParse(Convert.ToDateTime(item.DataProgramacao).ToShortDateString() + " " + item.InicioCarregamentoEsperado,
                            out dataHoraPrevistaCarregamento))
                        {
                            if (dataHoraMaisCedo == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaCarregamento < dataHoraMaisCedo)
                            {
                                dataHoraMaisCedo = dataHoraPrevistaCarregamento;
                                unidadeBaseAtual = item.LocalNascimento;
                            }
                        }

                        if (dataUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataUltimaEntrega < item.DataEntrega)
                            dataUltimaEntrega = Convert.ToDateTime(item.DataEntrega);

                        DateTime dataHoraPrevistaChegadaCliente = new DateTime();
                        if (DateTime.TryParse(Convert.ToDateTime(item.DataEntrega).ToShortDateString() + " " + item.ChegadaClienteEsperado,
                            out dataHoraPrevistaChegadaCliente))
                        {
                            if (dataHoraUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaChegadaCliente < dataHoraUltimaEntrega)
                                dataHoraUltimaEntrega = dataHoraPrevistaCarregamento;
                        }
                    }

                    int codigoIncubatorioBase = 0;
                    if (unidadeBaseAtual == "CH") codigoIncubatorioBase = 2;
                    else if (unidadeBaseAtual == "PH") codigoIncubatorioBase = 4;
                    else if (unidadeBaseAtual == "NM") codigoIncubatorioBase = 3;
                    else if (unidadeBaseAtual == "AJ") codigoIncubatorioBase = 1;

                    #endregion

                    if (idCarga > 0)
                    {
                        bool cargaApagada = false;
                        string retornoCarga = "";
                        if (carga.UnidadeBaseEmbarcador != unidadeBaseAtual)
                        {
                            #region Se a unidade da base da carga foi alterada, é necessário deletar a carga para criar novamente.

                            #region Apaga a carga antiga

                            retornoCarga = Embarcador.apagaCarga(idCarga);
                            if (!Boolean.TryParse(retornoCarga, out cargaApagada)) return "Erro ao apagar carga: " + retornoCarga;

                            #endregion

                            #region Insere nova carga (DESABILITADO, POIS SERÁ CRIADA JUNTO COM A INSERÇÃO DOS PEDIDOS)

                            //if (Boolean.TryParse(retornoCarga, out cargaApagada))
                            //{
                            //    retornoCarga = Embarcador.insereCarga(carga.ID, codigoIncubatorioBase, pedido.Observacao, aliasCarga);

                            //    if (!retornoCarga.Contains("Erro"))
                            //    {
                            //        idCarga = Convert.ToInt32(retornoCarga);
                            //    }
                            //    else
                            //    {
                            //        return "Erro ao inserir nova carga: " + retornoCarga;
                            //    }
                            //}
                            //else
                            //{
                            //    return "Erro ao apagar carga: " + retornoCarga;
                            //}

                            #endregion

                            #region Cria nova carga, insere os pedidos e vincula o veículo caso tenha placa

                            if (cargaApagada)
                            {
                                retornoCarga = InsereNovaCargaComPedidos(listaPedidosCarga);
                                if (retornoCarga != "") return "Erro ao inserir carga com pedidos: " + retornoCarga;
                            }

                            #endregion

                            #endregion
                        }
                        else
                        {
                            #region Se não teve alteração da carga, será excluído o pedido e inserido novamente

                            #region Carrega Item do CHIC

                            itemsTableAdapter iTA = new itemsTableAdapter();
                            Data.CHICDataSet.itemsDataTable iDT = new Data.CHICDataSet.itemsDataTable();
                            iTA.Fill(iDT);
                            bookedTableAdapter bTA = new bookedTableAdapter();
                            Data.CHICDataSet.bookedDataTable bDT = new Data.CHICDataSet.bookedDataTable();
                            bTA.FillByOrderNo(bDT, pedido.CHICNum);

                            Data.CHICDataSet.bookedRow bRow = bDT
                                .Where(w => iDT.Any(a => a.item_no == w.item
                                    && (a.form.Substring(0, 1).Equals("D"))
                                    && a.variety.Trim() == pedido.Linhagem))
                                .FirstOrDefault();

                            // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                            int qtdeCrypto = 0;
                            qtdeCrypto = Convert.ToInt32(pedido.Quantidade) * 17;

                            #endregion

                            #region Verifica se existe o pedido em alguma carga no Embarcador

                            string placaCargaPedido = "";
                            int idCargaPedido = 0;

                            if (bRow != null)
                            {
                                #region Busca o Pedido no Embarcador e retorna ID da Carga e Placa do Veículo

                                XDocument xmlExistePedido = Embarcador.buscaPedido(pedido.CHICNum, bRow.item);

                                foreach (XElement retorno in xmlExistePedido.Descendants("return"))
                                {
                                    var listaItens = retorno.Nodes();

                                    foreach (XElement item in listaItens)
                                    {
                                        var listaSubItens = item.Nodes()
                                            .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                        #region Carrega valores do retorno

                                        XElement objIDCarga = (XElement)listaSubItens[0];
                                        XElement objPlaca = (XElement)listaSubItens[1];
                                        XElement objQuantidade = (XElement)listaSubItens[2];
                                        XElement objPeso = (XElement)listaSubItens[3];

                                        #endregion

                                        if (objIDCarga.Value != "") idCargaPedido = Convert.ToInt32(objIDCarga.Value);
                                        if (objPlaca.Value != "") placaCargaPedido = objPlaca.Value;
                                    }
                                }

                                #endregion
                            }

                            #endregion

                            if (idCargaPedido > 0)
                            {
                                #region Se existe, primeiro remove da carga

                                #region Carrega parâmetros

                                OrderedDictionary parametrosRemovePedidosCarga = new OrderedDictionary();
                                parametrosRemovePedidosCarga.Add("ID_CARGA", idCargaPedido);
                                parametrosRemovePedidosCarga.Add("PLACA", placaCargaPedido);

                                ArrayList arrayPedidosRC = new ArrayList();
                                OrderedDictionary pedidoRC = new OrderedDictionary();
                                pedidoRC.Add("CODIGO", pedido.CHICNum);
                                pedidoRC.Add("NR_ITEM", bRow.item);
                                arrayPedidosRC.Add(pedidoRC);

                                parametrosRemovePedidosCarga.Add("PEDIDO", arrayPedidosRC);

                                #endregion

                                string pedidoRemovidaCarga = Embarcador.removePedidosCarga(parametrosRemovePedidosCarga);
                                if (pedidoRemovidaCarga.Contains("Erro")) return "Erro ao remover pedido ID " + pedido.ID.ToString()
                                    + " da carga " + idCargaPedido.ToString() + ": " + pedidoRemovidaCarga;

                                #endregion
                            }

                            #region Deleta pedido

                            OrderedDictionary parametrosApagaPedido = new OrderedDictionary();
                            parametrosApagaPedido.Add("CODIGO", pedido.CHICNum);
                            parametrosApagaPedido.Add("NR_ITEM", bRow.item);
                            parametrosApagaPedido.Add("APAGA_CARREGADO", false);

                            string retornoApagaPedido = Embarcador.apagaPedido(parametrosApagaPedido);
                            if (retornoApagaPedido.Contains("Erro") && !retornoApagaPedido.Contains("Pedido nao Encontrado"))
                                return "Erro ao apagar pedido ID " + pedido.ID.ToString() + ": " + retornoApagaPedido;

                            #endregion

                            #region Insere o pedido

                            string retornoPedido = InserePedidoEmbarcador(pedido.ID, true);
                            if (retornoPedido != "") return "Erro ao inserir pedido ID " + pedido.ID.ToString() + ": " + retornoPedido;

                            #endregion

                            #region Adiciona pedido na carga

                            retornoPedido = Embarcador.adicionaPedidosCarga(idCarga, "", Convert.ToInt32(pedido.CHICNum),
                                Convert.ToInt32(bRow.item), qtdeCrypto, 1);
                            if (retornoPedido.Contains("Carga Invalida"))
                            {
                                #region Apaga a carga antiga

                                retornoCarga = Embarcador.apagaCarga(idCarga);
                                if (!Boolean.TryParse(retornoCarga, out cargaApagada)) return "Erro ao apagar carga: " + retornoCarga;

                                #endregion

                                #region Cria nova carga, insere os pedidos e vincula o veículo caso tenha placa

                                if (cargaApagada)
                                {
                                    retornoCarga = InsereNovaCargaComPedidos(listaPedidosCarga);
                                    if (retornoCarga != "") return "Erro ao inserir carga com pedidos: " + retornoCarga;
                                }

                                #endregion
                            }
                            else if (retornoPedido.Contains("Erro"))
                                return "Erro ao adicionar pedido ID " + pedido.ID.ToString() + " na carga " + idCarga + ": " + retornoPedido;

                            #endregion

                            #endregion
                        }
                    }
                    else
                    {
                        #region Se não existe carga inserida, insere a carga e faz os vínculos do pedido e veículo caso tenha placa

                        #region Cria nova carga, insere os pedidos e vincula o veículo caso tenha a placa

                        string retornoCarga = InsereNovaCargaComPedidos(listaPedidosCarga);
                        if (retornoCarga != "") return "Erro ao inserir carga com pedidos: " + retornoCarga;

                        #endregion

                        #endregion
                    }

                    carga.UnidadeBaseEmbarcador = unidadeBaseAtual;
                    hlbapp.SaveChanges();
                }
                else if (pedido.NumVeiculo == 0)
                {
                    #region Se não existe carga vinculada, será excluído o pedido da acarga anterior.

                    #region Carrega Item do CHIC

                    itemsTableAdapter iTA = new itemsTableAdapter();
                    Data.CHICDataSet.itemsDataTable iDT = new Data.CHICDataSet.itemsDataTable();
                    iTA.Fill(iDT);
                    bookedTableAdapter bTA = new bookedTableAdapter();
                    Data.CHICDataSet.bookedDataTable bDT = new Data.CHICDataSet.bookedDataTable();
                    bTA.FillByOrderNo(bDT, pedido.CHICNum);

                    Data.CHICDataSet.bookedRow bRow = bDT
                        .Where(w => iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1).Equals("D"))))
                        .FirstOrDefault();

                    // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                    int qtdeCrypto = 0;
                    qtdeCrypto = Convert.ToInt32(pedido.Quantidade) * 17;

                    #endregion

                    #region Verifica se existe o pedido em alguma carga no Embarcador

                    string placaCargaPedido = "";
                    int idCargaPedido = 0;

                    if (bRow != null)
                    {
                        #region Busca o Pedido no Embarcador e retorna ID da Carga e Placa do Veículo

                        XDocument xmlExistePedido = Embarcador.buscaPedido(pedido.CHICNum, bRow.item);

                        foreach (XElement retorno in xmlExistePedido.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                #region Carrega valores do retorno

                                XElement objIDCarga = (XElement)listaSubItens[0];
                                XElement objPlaca = (XElement)listaSubItens[1];
                                XElement objQuantidade = (XElement)listaSubItens[2];
                                XElement objPeso = (XElement)listaSubItens[3];

                                #endregion

                                if (objIDCarga.Value != "") idCargaPedido = Convert.ToInt32(objIDCarga.Value);
                                if (objPlaca.Value != "") placaCargaPedido = objPlaca.Value;
                            }
                        }

                        #endregion
                    }

                    #endregion

                    if (idCargaPedido > 0)
                    {
                        #region Se existe, primeiro remove da carga

                        #region Carrega parâmetros

                        OrderedDictionary parametrosRemovePedidosCarga = new OrderedDictionary();
                        parametrosRemovePedidosCarga.Add("ID_CARGA", idCargaPedido);
                        parametrosRemovePedidosCarga.Add("PLACA", placaCargaPedido);

                        ArrayList arrayPedidosRC = new ArrayList();
                        OrderedDictionary pedidoRC = new OrderedDictionary();
                        pedidoRC.Add("CODIGO", pedido.CHICNum);
                        pedidoRC.Add("NR_ITEM", bRow.item);
                        arrayPedidosRC.Add(pedidoRC);

                        parametrosRemovePedidosCarga.Add("PEDIDO", arrayPedidosRC);

                        #endregion

                        string pedidoRemovidaCarga = Embarcador.removePedidosCarga(parametrosRemovePedidosCarga);
                        if (pedidoRemovidaCarga.Contains("Erro")) return "Erro ao remover pedido ID " + pedido.ID.ToString()
                            + " da carga " + idCargaPedido.ToString() + ": " + pedidoRemovidaCarga;

                        #endregion
                    }

                    #region Deleta pedido

                    OrderedDictionary parametrosApagaPedido = new OrderedDictionary();
                    parametrosApagaPedido.Add("CODIGO", pedido.CHICNum);
                    parametrosApagaPedido.Add("NR_ITEM", bRow.item);
                    parametrosApagaPedido.Add("APAGA_CARREGADO", false);

                    string retornoApagaPedido = Embarcador.apagaPedido(parametrosApagaPedido);
                    if (retornoApagaPedido.Contains("Erro") && !retornoApagaPedido.Contains("Pedido nao Encontrado"))
                        return "Erro ao apagar pedido ID " + pedido.ID.ToString() + ": " + retornoApagaPedido;

                    #endregion

                    #endregion
                }
            }
            return msg;
        }

        public static string InserePedidoEmbarcador(int iDPedidoProgDiarioTransp, bool inserirNoEmbarcador)
        {
            string erroPedido = "";
            string erroRetorno = "";

            try
            {
                #region Carrega Entitys

                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();
                ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();
                FinanceiroEntities apolo = new FinanceiroEntities();

                #endregion

                #region Localiza os pedidos a serem importados

                var listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.ID == iDPedidoProgDiarioTransp)
                    .ToList();

                #endregion

                #region Montagem e carregamento dos parâmetros

                //Cria um arrayList dos pedidos
                foreach (var pedidoCHIC in listaPedidos)
                {
                    erroPedido = pedidoCHIC.CHICNum;

                    #region Carrega Item do CHIC

                    itemsTableAdapter iTA = new itemsTableAdapter();
                    Data.CHICDataSet.itemsDataTable iDT = new Data.CHICDataSet.itemsDataTable();
                    iTA.Fill(iDT);
                    bookedTableAdapter bTA = new bookedTableAdapter();
                    Data.CHICDataSet.bookedDataTable bDT = new Data.CHICDataSet.bookedDataTable();
                    bTA.FillByOrderNo(bDT, pedidoCHIC.CHICNum);

                    Data.CHICDataSet.bookedRow bRow = bDT
                        .Where(w => iDT.Any(a => a.item_no == w.item
                            && (a.form.Substring(0, 1).Equals("D"))
                            && a.variety.Trim() == pedidoCHIC.Linhagem))
                        .FirstOrDefault();

                    // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                    int qtdeCrypto = 0;
                    qtdeCrypto = Convert.ToInt32(pedidoCHIC.Quantidade) * 17;

                    #endregion

                    #region Verifica se existe o pedido no Embarcador e deleta para inserir novamente

                    bool pedidoApagado = false;
                    OrderedDictionary parametrosBuscaPedido = new OrderedDictionary();
                    parametrosBuscaPedido.Add("CODIGO", pedidoCHIC.CHICNum);
                    parametrosBuscaPedido.Add("NR_ITEM", bRow.item);

                    XDocument xmlExistePedido = Embarcador.buscaPedido(parametrosBuscaPedido);

                    #region Verifica retorno se existe pedido

                    foreach (XElement retorno in xmlExistePedido.Descendants("return"))
                    {
                        var listaItens = retorno.Nodes();

                        foreach (XElement itemE in listaItens)
                        {
                            var listaSubItens = itemE.Nodes()
                                .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                            #region Carrega valores do retorno

                            XElement objIDCarga = (XElement)listaSubItens[0];
                            XElement objPlaca = (XElement)listaSubItens[1];
                            XElement objQuantidade = (XElement)listaSubItens[2];
                            XElement objPeso = (XElement)listaSubItens[3];

                            #endregion

                            // Se existe o pedido, deleta ele para inserir novamente
                            if (objQuantidade.Value != "" && objPeso.Value != "")
                            {
                                bool cargaRemovida = true;

                                if (objIDCarga.Value != "")
                                {
                                    #region Se existe carga, primeiro remove da carga

                                    #region Carrega parâmetros

                                    OrderedDictionary parametrosRemovePedidosCarga = new OrderedDictionary();
                                    parametrosRemovePedidosCarga.Add("ID_CARGA", objIDCarga.Value);
                                    parametrosRemovePedidosCarga.Add("PLACA", objPlaca.Value);

                                    ArrayList arrayPedidosRC = new ArrayList();
                                    OrderedDictionary pedidoRC = new OrderedDictionary();
                                    pedidoRC.Add("CODIGO", pedidoCHIC.CHICNum);
                                    pedidoRC.Add("NR_ITEM", bRow.item);
                                    arrayPedidosRC.Add(pedidoRC);

                                    parametrosRemovePedidosCarga.Add("PEDIDO", arrayPedidosRC);

                                    #endregion

                                    string cargaRemovidaStr = Embarcador.removePedidosCarga(parametrosRemovePedidosCarga);

                                    if (!Boolean.TryParse(cargaRemovidaStr, out cargaRemovida))
                                    {
                                        string corpoEmail = "";

                                        corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                            + "Número do Pedido CHIC: " + pedidoCHIC.CHICNum + (char)13 + (char)10
                                            + "Processo ao ser realizado: remoção do pedido de carga existente para atualização" + (char)13 + (char)10
                                            + "Mensagem do Erro: " + cargaRemovidaStr;

                                        return erroRetorno = corpoEmail;
                                    }

                                    #endregion
                                }

                                #region Deleta pedido

                                if (cargaRemovida)
                                {
                                    OrderedDictionary parametrosApagaPedido = new OrderedDictionary();
                                    parametrosApagaPedido.Add("CODIGO", pedidoCHIC.CHICNum);
                                    parametrosApagaPedido.Add("NR_ITEM", bRow.item);
                                    parametrosApagaPedido.Add("APAGA_CARREGADO", false);

                                    pedidoApagado = Convert.ToBoolean(Embarcador.apagaPedido(parametrosApagaPedido));
                                }

                                #endregion
                            }
                        }
                    }

                    #endregion

                    #endregion

                    OrderedDictionary parametros = new OrderedDictionary();
                    ArrayList arrayPedidos = new ArrayList();
                    OrderedDictionary pedido = new OrderedDictionary();

                    #region Carrega parâmetros do pedido

                    int codigoIncubatorio = 0;
                    if (pedidoCHIC.LocalNascimento == "CH") codigoIncubatorio = 2;
                    else if (pedidoCHIC.LocalNascimento == "PH") codigoIncubatorio = 4;
                    else if (pedidoCHIC.LocalNascimento == "NM") codigoIncubatorio = 3;
                    else if (pedidoCHIC.LocalNascimento == "AJ") codigoIncubatorio = 1;

                    #region Insere / Atualiza Unidade

                    string retornoUnidade = InsereAtualizaUnidade(pedidoCHIC.CodigoCliente.Trim(), codigoIncubatorio);
                    if (retornoUnidade != "") return "Erro ao inserir / atualizar unidade " + pedidoCHIC.CodigoCliente + ": " + retornoUnidade;

                    #endregion

                    //Cria um arrayList com os dados da unidade base
                    ArrayList arrayUnidadeBase = new ArrayList();
                    OrderedDictionary unidadeBase = new OrderedDictionary();
                    unidadeBase.Add("codigo", codigoIncubatorio);
                    unidadeBase.Add("diferenciador", "");
                    arrayUnidadeBase.Add(unidadeBase);
                    pedido.Add("base", arrayUnidadeBase);

                    //Cria um arrayList com os dados da origem
                    ArrayList arrayOrigem = new ArrayList();
                    OrderedDictionary origem = new OrderedDictionary();
                    origem.Add("codigo", codigoIncubatorio);
                    origem.Add("diferenciador", "");
                    arrayOrigem.Add(origem);
                    pedido.Add("origem", arrayOrigem);

                    int codigodestino = Convert.ToInt32(pedidoCHIC.CodigoCliente);
                    //Cria um arrayList com os dados da unidade base
                    ArrayList arrayDestino = new ArrayList();
                    OrderedDictionary destino = new OrderedDictionary();
                    destino.Add("codigo", codigodestino);
                    destino.Add("diferenciador", "");
                    destino.Add("NOME", "");
                    destino.Add("CIDADE", "");
                    destino.Add("UF", "");
                    destino.Add("TELEFONE", "");
                    destino.Add("ENDERECO", "");
                    destino.Add("NUMERO", "");
                    destino.Add("BAIRRO", "");
                    destino.Add("CEP", "");
                    destino.Add("COMPLEMENTO", "");
                    destino.Add("LATITUDE", "");
                    destino.Add("LONGITUDE", "");
                    destino.Add("TIPO", "");
                    destino.Add("CPF_CPNJ", "");
                    destino.Add("PESSOA", "");
                    arrayDestino.Add(destino);
                    pedido.Add("destino", arrayDestino);

                    //Cria um arrayList com os dados do transbordo
                    ArrayList arrayTransbordo = new ArrayList();
                    OrderedDictionary transbordo = new OrderedDictionary();
                    transbordo.Add("codigo", "");
                    transbordo.Add("diferenciador", "");
                    arrayTransbordo.Add(transbordo);
                    pedido.Add("TRANSBORDO", arrayTransbordo);

                    pedido.Add("codigo", pedidoCHIC.CHICNum);
                    pedido.Add("data_embarque", Convert.ToDateTime(pedidoCHIC.DataProgramacao).ToShortDateString());
                    pedido.Add("data_entrega", Convert.ToDateTime(pedidoCHIC.DataEntrega).ToShortDateString());
                    pedido.Add("tipo_data_entrega", "E");
                    pedido.Add("tipo_pedido", 107);
                    pedido.Add("TIPO_CARGA", 1561); // Transporte de Pintos
                    //pedido.Add("TIPO_CARGA", 2623); // Transporte de Pintos - Integração
                    //pedido.Add("tipo_operacao", 1783);
                    pedido.Add("tipo_operacao", 2623);
                    pedido.Add("EMPACOTAMENTO", "");
                    pedido.Add("MICRO_REGIAO", 0);
                    pedido.Add("OBSERVACAO", "");
                    pedido.Add("representante", pedidoCHIC.CodigoRepresentante.Trim());
                    pedido.Add("CLIENTE_UNICO", "");
                    pedido.Add("PRIORIDADE", "");
                    pedido.Add("COD_CARGA", "");
                    pedido.Add("ALIAS_CARGA", "");

                    #endregion

                    ArrayList arrayProdutos = new ArrayList();

                    #region Carrega parâmetros do item

                    OrderedDictionary produto = new OrderedDictionary();
                    produto.Add("codigo", bRow.item);
                    produto.Add("item", bRow.item);
                    produto.Add("descricao", pedidoCHIC.Linhagem + " - " + pedidoCHIC.Produto);
                    produto.Add("quantidade", qtdeCrypto);
                    produto.Add("PESO_UNITARIO", 1);
                    produto.Add("PESO_TOTAL", 1);
                    produto.Add("VOLUME", "");
                    produto.Add("OBSERVACAO", "");
                    produto.Add("TIPO_CARGA", 1561);
                    produto.Add("data_embarque", Convert.ToDateTime(pedidoCHIC.DataProgramacao).ToShortDateString());
                    produto.Add("data_entrega", Convert.ToDateTime(pedidoCHIC.DataProgramacao).ToShortDateString());
                    produto.Add("TIPO_DATA_ENTREGA", "E");
                    produto.Add("PRIORIDADE", "B");
                    arrayProdutos.Add(produto);

                    #endregion

                    pedido.Add("produto", arrayProdutos);

                    arrayPedidos.Add(pedido);

                    parametros.Add("pedido", arrayPedidos);

                    if (inserirNoEmbarcador)
                    {
                        #region Execução do WebService

                        XDocument xmlRetorno = Embarcador.inserePedidosLote(parametros);

                        foreach (XElement retorno in xmlRetorno.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                bool entidadeResolvida = false;

                                foreach (XElement subItem in listaSubItens)
                                {
                                    var listaErros = subItem.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    XElement objParametroPedido = (XElement)listaErros[0];
                                    XElement objParametroErro = (XElement)listaErros[1];
                                    XElement objMsgErro = (XElement)listaErros[2];

                                    if (objMsgErro.Value.Contains("Unidade")
                                        && objMsgErro.Value.Contains("Nao Encontrada"))
                                    {
                                        #region Carrega Entidade

                                        ObjectParameter value = new ObjectParameter("numero", typeof(global::System.String));
                                        apoloService.CONCAT_ZERO_ESQUERDA(objParametroErro.Value, 7, value);
                                        string codigoEntidadeApolo = value.Value.ToString();

                                        ENTIDADE entidadeApolo = apolo.ENTIDADE
                                            .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                                        #endregion

                                        #region Insere / Atualiza Unidade

                                        retornoUnidade = InsereAtualizaUnidade(codigoEntidadeApolo, codigoIncubatorio);
                                        if (retornoUnidade != "") return "Erro ao inserir / atualizar unidade " + pedidoCHIC.CodigoCliente + ": " + retornoUnidade;

                                        #endregion
                                    }
                                    else
                                    {
                                        if (objMsgErro.Value != "")
                                        {
                                            if (!entidadeResolvida && objMsgErro.Value.Contains("Unidade"))
                                            {
                                                #region Verifica Se unidade esta como destino. Caso não, atualize a mesma.

                                                bool unidadeAtualizada = false;

                                                if (objMsgErro.Value.Contains("Unidade de Destino Nao Esta Cadastrada Como Destino na Operacao"))
                                                {
                                                    #region Carrega Entidade

                                                    ObjectParameter value = new ObjectParameter("numero", typeof(global::System.String));
                                                    apoloService.CONCAT_ZERO_ESQUERDA(objParametroErro.Value, 7, value);
                                                    string codigoEntidadeApolo = value.Value.ToString();

                                                    ENTIDADE entidadeApolo = apolo.ENTIDADE
                                                        .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                                                    #endregion

                                                    #region Insere / Atualiza Unidade

                                                    retornoUnidade = InsereAtualizaUnidade(codigoEntidadeApolo, codigoIncubatorio);
                                                    if (retornoUnidade != "") return "Erro ao inserir / atualizar unidade " + pedidoCHIC.CodigoCliente + ": " + retornoUnidade;

                                                    #endregion

                                                    unidadeAtualizada = true;
                                                }

                                                if (!unidadeAtualizada)
                                                {
                                                    string corpoEmail = "";

                                                    corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                                        + "Número do Pedido CHIC: " + objParametroPedido.Value + (char)13 + (char)10
                                                        + "Parâmetro do Erro: " + objParametroErro.Value + (char)13 + (char)10
                                                        + "Mensagem do Erro: " + objMsgErro.Value;

                                                    return erroRetorno = corpoEmail;
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                return erroRetorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erroRetorno = "Erro ao Atualizar Prog. Diária de Transportes com Embarcador - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    erroRetorno = erroRetorno + " Erro Secundário: " + ex.InnerException.Message;

                return erroRetorno;
            }
        }

        public static string InsereNovaCargaComPedidos(List<Models.HLBAPP.Prog_Diaria_Transp_Pedidos> listaPedidosCarga)
        {
            #region Carrega Variáveis

            string unidadeBaseAtual = "";
            DateTime dataHoraMaisCedo = new DateTime();
            DateTime dataUltimaEntrega = new DateTime();
            DateTime dataHoraUltimaEntrega = new DateTime();
            //Cria um arrayList com os dados do pedido
            ArrayList arrayPedido = new ArrayList();

            #endregion

            #region Carrega dados do pedido e da carga

            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            Prog_Diaria_Transp_Pedidos pedido = listaPedidosCarga.FirstOrDefault();

            Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
                .Where(w => w.DataProgramacao == pedido.DataProgramacao
                    && w.EmpresaTranportador == pedido.EmpresaTranportador
                    && w.NumVeiculo == pedido.NumVeiculo)
                .FirstOrDefault();

            int idCarga = 0;
            string empresaResponsavel = "Trans Ema";
            if (carga.EmpresaTranportador == "HN") empresaResponsavel = "HeN";
            else if (carga.EmpresaTranportador == "PL") empresaResponsavel = "Planalto";
            string aliasCarga = Convert.ToDateTime(carga.DataProgramacao).ToShortDateString()
                + " - CM Nº: " + carga.NumVeiculo + " - Resp. Transp.: " + empresaResponsavel;

            #endregion

            foreach (var item in listaPedidosCarga)
            {
                #region Carrega Unidade Base e data / hora mais cedo e mais tarde

                DateTime dataHoraPrevistaCarregamento = new DateTime();
                if (DateTime.TryParse(Convert.ToDateTime(item.DataProgramacao).ToShortDateString() + " " + item.InicioCarregamentoEsperado,
                    out dataHoraPrevistaCarregamento))
                {
                    if (dataHoraMaisCedo == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaCarregamento < dataHoraMaisCedo)
                    {
                        dataHoraMaisCedo = dataHoraPrevistaCarregamento;
                        unidadeBaseAtual = item.LocalNascimento;
                    }
                }

                if (dataUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataUltimaEntrega < item.DataEntrega)
                    dataUltimaEntrega = Convert.ToDateTime(item.DataEntrega);

                DateTime dataHoraPrevistaChegadaCliente = new DateTime();
                if (DateTime.TryParse(Convert.ToDateTime(item.DataEntrega).ToShortDateString() + " " + item.ChegadaClienteEsperado,
                    out dataHoraPrevistaChegadaCliente))
                {
                    if (dataHoraUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaChegadaCliente < dataHoraUltimaEntrega)
                        dataHoraUltimaEntrega = dataHoraPrevistaCarregamento;
                }

                #endregion

                #region Insere os pedidos

                string retornoPedido = InserePedidoEmbarcador(item.ID, true);
                if (retornoPedido != "") return "Erro ao inserir pedido ID " + item.ID.ToString() + ": " + retornoPedido;

                #endregion

                #region Carrega Item do CHIC

                itemsTableAdapter iTA = new itemsTableAdapter();
                Data.CHICDataSet.itemsDataTable iDTN = new Data.CHICDataSet.itemsDataTable();
                iTA.Fill(iDTN);
                bookedTableAdapter bTA = new bookedTableAdapter();
                Data.CHICDataSet.bookedDataTable bDTN = new Data.CHICDataSet.bookedDataTable();
                bTA.FillByOrderNo(bDTN, item.CHICNum);

                Data.CHICDataSet.bookedRow bRowN = bDTN
                    .Where(w => iDTN.Any(a => a.item_no == w.item
                        && (a.form.Substring(0, 1).Equals("D"))
                        && a.variety.Trim() == item.Linhagem))
                    .FirstOrDefault();

                // Quantidade criptografada para mascarar. Solicitado por Davi Nogueira.
                int qtdeCrypto = 0;
                qtdeCrypto = Convert.ToInt32(item.Quantidade) * 17;

                #endregion

                #region Insere no array de Pedidos para importar no embarcador

                OrderedDictionary pedidoOD = new OrderedDictionary();
                pedidoOD.Add("COD_PEDIDO", item.CHICNum);
                pedidoOD.Add("NR_ITEM", bRowN.item);
                pedidoOD.Add("QUANTIDADE", qtdeCrypto);
                pedidoOD.Add("PESO_TOTAL", 1);
                arrayPedido.Add(pedidoOD);

                #endregion
            }

            #region Carrega código Unidade Base

            int codigoIncubatorioBase = 0;
            if (unidadeBaseAtual == "CH") codigoIncubatorioBase = 2;
            else if (unidadeBaseAtual == "PH") codigoIncubatorioBase = 4;
            else if (unidadeBaseAtual == "NM") codigoIncubatorioBase = 3;
            else if (unidadeBaseAtual == "AJ") codigoIncubatorioBase = 1;

            #endregion

            #region Cria a carga com os pedidos já relacionados

            string retornoCarga = Embarcador.insereCargaComPedidos(carga.ID, codigoIncubatorioBase, "", aliasCarga, arrayPedido);

            GeraLOGIntegracaoEmbarcador(carga.ID, "Inserção Nova Carga", idCarga, retornoCarga);
            
            if (int.TryParse(retornoCarga, out idCarga))
            {
                idCarga = Convert.ToInt32(retornoCarga);
                carga.IDCargaEmbarcador = idCarga;
            }
            else if (retornoCarga.Contains("Pedido ja esta carregado"))
            {
                string codigoPedido = retornoCarga.Substring(79, 5);
                string itemPedido = retornoCarga.Substring(retornoCarga.IndexOf("|") + 1, 3);
                idCarga = Embarcador.retornaIDCargaPedido(codigoPedido, itemPedido);
                carga.IDCargaEmbarcador = idCarga;
            }
            else
                return "Erro ao inserir nova carga: " + retornoCarga;

            #endregion

            #region Vincula veículo na carga se tiver placa inserida

            if (carga.Placa != null)
                if (carga.Placa.Trim() != "")
                {
                    string retornoVinculaVeiculoCarga = Embarcador.vincularCargaAoVeiculo(idCarga, carga.Placa, 
                        Convert.ToDateTime(carga.DataEmbarque), dataHoraMaisCedo, dataUltimaEntrega, dataHoraUltimaEntrega, true);
                    GeraLOGIntegracaoEmbarcador(carga.ID, "Vínculo do Veículo na Carga", idCarga, retornoVinculaVeiculoCarga);
                    if (retornoVinculaVeiculoCarga != "true") 
                        return "Erro ao vincular o veículo " + carga.Placa + " na carga " + idCarga + ": " + retornoVinculaVeiculoCarga;
                }

            #endregion

            hlbapp.SaveChanges();

            return "";
        }

        public static void GeraLOGIntegracaoEmbarcador(int idProgDiariaTranspVeiculos, string operacao,
            int idCargaEmbarcador, string retornoIntegracao)
        {
            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            LOG_Prog_Diaria_Transp_Veiculos log = new LOG_Prog_Diaria_Transp_Veiculos();
            log.IDProgDiariaTranspVeiculos = idProgDiariaTranspVeiculos;
            log.DataHora = DateTime.Now;
            log.Operacao = operacao;
            log.IDCargaEmbarcador = idCargaEmbarcador;
            log.RetornoIntegracao = retornoIntegracao;

            hlbapp.LOG_Prog_Diaria_Transp_Veiculos.AddObject(log);
            hlbapp.SaveChanges();
        }

        public static string InsereAtualizaUnidade(string codigoEntidadeApolo, int codigoIncubatorio)
        {
            string erroPedido = "";
            string erroRetorno = "";

            try
            {
                #region Carrega Entitys

                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();
                ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();
                FinanceiroEntities apolo = new FinanceiroEntities();

                #endregion

                #region Insere Unidade

                #region Carrega Entidade

                ENTIDADE entidadeApolo = apolo.ENTIDADE
                    .Where(w => w.EntCod == codigoEntidadeApolo).FirstOrDefault();

                #endregion

                if (entidadeApolo != null)
                {
                    #region Carrega Dados Entidade

                    CIDADE cidade = apolo.CIDADE
                        .Where(w => w.CidCod == entidadeApolo.CidCod).FirstOrDefault();

                    #endregion

                    #region Carrega Parâmetros

                    //Cria um arrayList com os dados da unidade pai
                    ArrayList arrayUnidadePai = new ArrayList();
                    OrderedDictionary arrayUnidadePaiItens = new OrderedDictionary();
                    arrayUnidadePaiItens.Add("cod_unidade", codigoIncubatorio);
                    arrayUnidadePaiItens.Add("diferenciador", "");
                    arrayUnidadePai.Add(arrayUnidadePaiItens);

                    //Cria um arrayList com os dados da referencia da unidade
                    ArrayList arrayReferencia = new ArrayList();
                    OrderedDictionary arrayReferenciaItens = new OrderedDictionary();
                    arrayReferenciaItens.Add("lat", 0);
                    arrayReferenciaItens.Add("lon", 0);
                    arrayReferencia.Add(arrayReferenciaItens);

                    //Cria um arrayList com os dados do(s) tipo(s) de operacao da unidade
                    ArrayList arrayTipoOperacao = new ArrayList();
                    OrderedDictionary arrayTipoOperacaoItens = new OrderedDictionary();
                    arrayTipoOperacaoItens.Add("codigo", 2623);
                    arrayTipoOperacaoItens.Add("origem", false);
                    arrayTipoOperacaoItens.Add("destino", true);
                    arrayTipoOperacaoItens.Add("passagem", false);
                    arrayTipoOperacao.Add(arrayTipoOperacaoItens);

                    string complemento = "";
                    if (entidadeApolo.EntEnderComp != null) complemento = entidadeApolo.EntEnderComp;
                    string cep = "";
                    if (entidadeApolo.EntCep != null) cep = entidadeApolo.EntCep;
                    string numeroEnder = "";
                    if (entidadeApolo.EntEnderNo != null) numeroEnder = entidadeApolo.EntEnderNo;
                    string bairro = "";
                    if (entidadeApolo.EntBair != null) bairro = entidadeApolo.EntBair;

                    //Cria um objeto e adiciona os parametros necessarios na ordem estabelecida (deve obrigatoriamente seguir a ordem especificada no manual)
                    OrderedDictionary parametrosUnidade = new OrderedDictionary();
                    parametrosUnidade.Add("cod_unidade", Convert.ToInt32(entidadeApolo.EntCod));
                    parametrosUnidade.Add("diferenciador", "");
                    parametrosUnidade.Add("descricao", entidadeApolo.EntNome);
                    parametrosUnidade.Add("responsavel", "");
                    parametrosUnidade.Add("telefone", "");
                    parametrosUnidade.Add("endereco", entidadeApolo.EntEnder);
                    parametrosUnidade.Add("observacao", "");
                    parametrosUnidade.Add("unidade_pai", arrayUnidadePai);
                    parametrosUnidade.Add("cidade", cidade.CidNomeComp);
                    parametrosUnidade.Add("uf", cidade.UfSigla);
                    parametrosUnidade.Add("tipo", 2622);
                    parametrosUnidade.Add("zona", "");
                    parametrosUnidade.Add("regiao", "");
                    parametrosUnidade.Add("referencia", arrayReferencia);
                    parametrosUnidade.Add("tipo_operacao", arrayTipoOperacao);
                    parametrosUnidade.Add("cnpj", "");
                    parametrosUnidade.Add("numero", numeroEnder);
                    parametrosUnidade.Add("bairro", bairro);
                    parametrosUnidade.Add("cep", cep);
                    parametrosUnidade.Add("complemento", complemento);
                    parametrosUnidade.Add("tipo_pessoa", entidadeApolo.EntTipoFJ.Substring(0, 1));
                    parametrosUnidade.Add("rg_ie", "");

                    #endregion

                    #region Execução WebService

                    string retornoEntidade = Embarcador.insereAtualizaUnidade(parametrosUnidade);

                    if (retornoEntidade.Contains("BAD")
                        || retornoEntidade.Contains("Msg. do Erro"))
                    {
                        erroRetorno = "Erro ao cadastrar entidade "
                            + entidadeApolo.EntCod + " - "
                            + entidadeApolo.EntNome + " no Embarcador por não existir: "
                            + retornoEntidade;

                        return erroRetorno;
                    }

                    #endregion
                }

                #endregion

                return erroRetorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                erroRetorno = "Erro ao Atualizar Entidade com Embarcador - Erro Linha: " + linenum.ToString()
                    + " / Erro primário: " + ex.Message;
                if (ex.InnerException != null)
                    erroRetorno = erroRetorno + " Erro Secundário: " + ex.InnerException.Message;

                return erroRetorno;
            }
        }

        public string AtualizaVeiculoCargaEmbarcador(int idCargaProgDiariaTransp, string placaAntiga, string placaNova)
        {
            string msg = "";
            int idCarga = 0;

            if (Convert.ToDateTime(txtDataProgramacao.Text) >= DateTime.Today)
            {
                #region Carrega Entitys

                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

                #endregion

                #region Carrega dados da carga

                Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.ID == idCargaProgDiariaTransp).FirstOrDefault();

                #endregion

                #region Carrega carga do Embarcador

                msg = Embarcador.buscaCargaCodigo(carga.ID);
                if (!msg.Contains("Erro"))
                {
                    idCarga = Convert.ToInt32(msg);
                }
                else
                {
                    if (!msg.Contains("Codigo de Carga do Cliente nao encontrado"))
                        return "Erro ao buscar carga: " + msg;
                }

                #endregion

                if (idCarga > 0)
                {
                    #region Carrega dados dos pedidos

                    var listaPedidosCarga = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.DataProgramacao == carga.DataProgramacao
                            && w.EmpresaTranportador == carga.EmpresaTranportador
                            && w.NumVeiculo == carga.NumVeiculo)
                        .ToList();

                    DateTime dataHoraMaisCedo = new DateTime();
                    DateTime dataUltimaEntrega = new DateTime();
                    DateTime dataHoraUltimaEntrega = new DateTime();

                    foreach (var item in listaPedidosCarga)
                    {
                        DateTime dataHoraPrevistaCarregamento = new DateTime();
                        if (DateTime.TryParse(Convert.ToDateTime(item.DataProgramacao).ToShortDateString() + " " + item.InicioCarregamentoEsperado,
                            out dataHoraPrevistaCarregamento))
                        {
                            if (dataHoraMaisCedo == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaCarregamento < dataHoraMaisCedo)
                            {
                                dataHoraMaisCedo = dataHoraPrevistaCarregamento;
                            }
                        }

                        if (dataUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataUltimaEntrega < item.DataEntrega)
                            dataUltimaEntrega = Convert.ToDateTime(item.DataEntrega);

                        DateTime dataHoraPrevistaChegadaCliente = new DateTime();
                        if (DateTime.TryParse(Convert.ToDateTime(item.DataEntrega).ToShortDateString() + " " + item.ChegadaClienteEsperado,
                            out dataHoraPrevistaChegadaCliente))
                        {
                            if (dataHoraUltimaEntrega == Convert.ToDateTime("01/01/0001") || dataHoraPrevistaChegadaCliente < dataHoraUltimaEntrega)
                                dataHoraUltimaEntrega = dataHoraPrevistaCarregamento;
                        }
                    }

                    #endregion

                    if (placaAntiga != null)
                        if (placaAntiga.Trim() != "")
                        {
                            #region Remove vinculo do veículo antigo da carga

                            string retornoVinculaVeiculoCarga = Embarcador.removerVinculoCargaVeiculo(idCarga, placaAntiga);
                            if (retornoVinculaVeiculoCarga != "true" && !retornoVinculaVeiculoCarga.Contains("Viagem nao encontrada"))
                                return "Erro ao remover vinculo do veículo " + placaAntiga + " na carga " + idCarga + ": " + retornoVinculaVeiculoCarga;

                            #endregion
                        }

                    if (placaNova != null)
                        if (placaNova.Trim() != "")
                        {
                            #region Vincula veículo novo na carga

                            string retornoVinculaVeiculoCarga = Embarcador.vincularCargaAoVeiculo(idCarga, placaNova, Convert.ToDateTime(carga.DataEmbarque),
                                dataHoraMaisCedo, dataUltimaEntrega, dataHoraUltimaEntrega, false);
                            if (retornoVinculaVeiculoCarga != "true") return "Erro ao vincular o veículo " + placaNova + " na carga " + idCarga + ": " + retornoVinculaVeiculoCarga;

                            #endregion
                        }
                }
            }

            return "";
        }

        public static string RetornaDadosViagem(int idCarga)
        {
            string msgRetorno = "";

            try
            {
                #region Busca ID Embarcador

                HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

                Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.ID == idCarga).FirstOrDefault();
                string placa = "";
                if (carga != null) placa = carga.Placa;

                string retornoBusca = Embarcador.buscaCargaCodigo(idCarga);

                int idCargaEmbarcador = 0;

                #endregion

                if (int.TryParse(retornoBusca, out idCargaEmbarcador))
                {
                    #region Retorna Dados da Viagem

                    XDocument xmlRetornoDadosViagem = Embarcador.retornaDadosViagem(idCargaEmbarcador, placa);

                    if (xmlRetornoDadosViagem != null)
                    {
                        #region Verifica XML OK

                        foreach (XElement retorno in xmlRetornoDadosViagem.Descendants("return"))
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                var listaSubItens = item.Nodes()
                                    .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                #region Carrega valores do retorno

                                XElement obj = (XElement)listaSubItens[0];

                                #region ORIGENS

                                if (obj.Value == "ORIGENS")
                                {
                                    XElement objValues = (XElement)listaSubItens[1];
                                    var listaObjs = objValues.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    foreach (XElement origem in listaObjs)
                                    {
                                        var listaCampos = origem.Nodes().ToList();

                                        string codIncubatorio = ((XElement)((XElement)listaCampos[0]).Nodes().ToList()[1]).Value;
                                        string descricaoOrigem = ((XElement)((XElement)listaCampos[2]).Nodes().ToList()[1]).Value;

                                        //DateTime dataChegada = Convert.ToDateTime(((XElement)((XElement)listaCampos[6]).Nodes().ToList()[1]).Value);
                                        string horaChegada = ((XElement)((XElement)listaCampos[4]).Nodes().ToList()[1]).Value;
                                        //DateTime dataSaida = Convert.ToDateTime(((XElement)((XElement)listaCampos[8]).Nodes().ToList()[1]).Value);
                                        string horaSaida = ((XElement)((XElement)listaCampos[6]).Nodes().ToList()[1]).Value;
                                        List<Prog_Diaria_Transp_Veiculos> listaCargas = hlbapp.Prog_Diaria_Transp_Veiculos
                                            .Where(w => w.DataProgramacao == carga.DataProgramacao
                                                && w.NumVeiculo == carga.NumVeiculo
                                                && w.EmpresaTranportador == carga.EmpresaTranportador).ToList();

                                        foreach (var cargaWeb in listaCargas)
                                        {
                                            if (descricaoOrigem == "INCUBATÓRIO DE MATRIZES - NOVA GRANADA" ||
                                                descricaoOrigem == "INCUBATÓRIO INTEGRADO - AJAPI - CHEGADA" ||
                                                descricaoOrigem == "CARREGAMENTO DE PINTOS - PLANALTO")
                                            {
                                                if (horaChegada != "" && cargaWeb.InicioCarregamentoReal == null)
                                                    cargaWeb.InicioCarregamentoReal = horaChegada;
                                            }
                                            else if (codIncubatorio == "1" || codIncubatorio == "2" || codIncubatorio == "3")
                                            {
                                                if (horaSaida != "" && cargaWeb.TerminoCarregamentoReal == null)
                                                    cargaWeb.TerminoCarregamentoReal = horaSaida;
                                            }
                                        }
                                    }
                                }

                                #endregion

                                #region DESTINOS

                                if (obj.Value == "DESTINOS")
                                {
                                    XElement objValues = (XElement)listaSubItens[1];
                                    var listaObjs = objValues.Nodes()
                                        .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                                    foreach (XElement destino in listaObjs)
                                    {
                                        var listaCampos = destino.Nodes().ToList();

                                        string codUnidade = ((XElement)((XElement)listaCampos[0]).Nodes().ToList()[1]).Value;
                                        DateTime dataChegada = new DateTime();
                                        if (DateTime.TryParse(((XElement)((XElement)listaCampos[6]).Nodes().ToList()[1]).Value, out dataChegada))
                                            dataChegada = Convert.ToDateTime(((XElement)((XElement)listaCampos[6]).Nodes().ToList()[1]).Value);
                                        string horaChegada = ((XElement)((XElement)listaCampos[7]).Nodes().ToList()[1]).Value;
                                        //DateTime dataSaida = Convert.ToDateTime(((XElement)((XElement)listaCampos[8]).Nodes().ToList()[1]).Value);
                                        string horaSaida = ((XElement)((XElement)listaCampos[9]).Nodes().ToList()[1]).Value;

                                        string codUnidadeCompleto = codUnidade.PadLeft(7, '0');

                                        List<Prog_Diaria_Transp_Pedidos> listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                                            .Where(w => w.DataProgramacao == carga.DataProgramacao
                                                && w.NumVeiculo == carga.NumVeiculo
                                                && w.EmpresaTranportador == carga.EmpresaTranportador
                                                && w.CodigoCliente == codUnidadeCompleto).ToList();

                                        foreach (var pedido in listaPedidos)
                                        {
                                            if (horaChegada != "") pedido.ChegadaClienteReal = horaChegada;
                                            if (dataChegada != Convert.ToDateTime("01/01/0001")) pedido.DataChegadaClienteReal = dataChegada;
                                        }
                                    }
                                }

                                #endregion

                                //XElement objPlaca = (XElement)listaSubItens[1];
                                //XElement objQuantidade = (XElement)listaSubItens[2];
                                //XElement objPeso = (XElement)listaSubItens[3];

                                #endregion

                                //if (objIDCarga.Value != "") idCargaPedido = Convert.ToInt32(objIDCarga.Value);
                                //if (objPlaca.Value != "") placaCargaPedido = objPlaca.Value;
                            }
                        }

                        #endregion

                        #region Verifica XML Erro

                        foreach (XElement retorno in xmlRetornoDadosViagem.Descendants())
                        {
                            if (retorno.Name.LocalName == "Fault")
                            {
                                var listaItens = retorno.Nodes();

                                foreach (XElement item in listaItens)
                                {
                                    if (item.Name.LocalName == "Code")
                                    {
                                        var listaItensCode = item.Nodes().ToList();
                                        XElement objCode = (XElement)listaItensCode[0];
                                        msgRetorno = "Código do Erro: " + objCode.Value + "<br />";
                                    }

                                    if (item.Name.LocalName == "Reason")
                                    {
                                        var listaItensReason = item.Nodes().ToList();
                                        XElement objReason = (XElement)listaItensReason[0];
                                        msgRetorno = msgRetorno + "Msg. do Erro: " + objReason.Value + "<br />";
                                    }

                                    if (item.Name.LocalName == "Detail")
                                    {
                                        msgRetorno = msgRetorno + "Código da Carga: " + item.Value + "<br />";
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else
                    msgRetorno = retornoBusca;

                #region Se os horários de origem inicial e final estiverem vazio, chama o método "retornaVeiculoPassagens"

                DateTime dataEmbarque = Convert.ToDateTime(carga.DataEmbarque);
                XDocument xmlRetornoVeiculoPassagens = Embarcador.retornaVeiculoPassagens(carga.Placa, dataEmbarque);

                #region Verifica XML OK

                DateTime dataHoraInicioCarregamento = new DateTime();
                DateTime dataHoraFimCarregamento = new DateTime();

                if (xmlRetornoVeiculoPassagens != null)
                {
                    foreach (XElement retorno in xmlRetornoVeiculoPassagens.Descendants("return"))
                    {
                        var listaItens = retorno.Nodes();

                        foreach (XElement item in listaItens)
                        {
                            var listaSubItens = item.Nodes()
                                .Where(w => w.NodeType == System.Xml.XmlNodeType.Element).ToList();

                            XElement tipoUnidadeObj = (XElement)listaSubItens[2];

                            //if (tipoUnidadeObj.Value == "INCUBATORIO")
                            if (tipoUnidadeObj.Value == "CARREGAMENTO")
                            {
                                XElement chegadaObj = (XElement)listaSubItens[3];
                                DateTime dataChegada = new DateTime();
                                if (DateTime.TryParse(chegadaObj.Value, out dataChegada))
                                {
                                    if (dataHoraInicioCarregamento == Convert.ToDateTime("01/01/0001")
                                    || dataHoraInicioCarregamento > dataChegada)
                                        dataHoraInicioCarregamento = dataChegada;
                                }

                                XElement saidaObj = (XElement)listaSubItens[4];
                                DateTime dataSaida = new DateTime();
                                if (DateTime.TryParse(saidaObj.Value, out dataSaida))
                                {
                                    if (dataHoraFimCarregamento == Convert.ToDateTime("01/01/0001")
                                    || dataHoraFimCarregamento < dataSaida)
                                        dataHoraFimCarregamento = dataSaida;
                                }
                            }
                            else if (tipoUnidadeObj.Value == "INCUBATORIO")
                            {
                                XElement chegadaObj = (XElement)listaSubItens[3];
                                DateTime dataChegada = new DateTime();
                                if (DateTime.TryParse(chegadaObj.Value, out dataChegada))
                                {
                                    if (dataHoraInicioCarregamento == Convert.ToDateTime("01/01/0001")
                                    || dataHoraInicioCarregamento > dataChegada)
                                        dataHoraInicioCarregamento = dataChegada;
                                }

                                XElement saidaObj = (XElement)listaSubItens[4];
                                DateTime dataSaida = new DateTime();
                                if (DateTime.TryParse(saidaObj.Value, out dataSaida))
                                {
                                    if (dataHoraFimCarregamento == Convert.ToDateTime("01/01/0001")
                                    || dataHoraFimCarregamento < dataSaida)
                                        dataHoraFimCarregamento = dataSaida;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Verifica XML Erro

                if (xmlRetornoVeiculoPassagens != null)
                {
                    string msgErroRetornoVeiculoPassagens = "";

                    foreach (XElement retorno in xmlRetornoVeiculoPassagens.Descendants())
                    {
                        if (retorno.Name.LocalName == "Fault")
                        {
                            var listaItens = retorno.Nodes();

                            foreach (XElement item in listaItens)
                            {
                                if (item.Name.LocalName == "Code")
                                {
                                    var listaItensCode = item.Nodes().ToList();
                                    XElement objCode = (XElement)listaItensCode[0];
                                    msgErroRetornoVeiculoPassagens = "Código do Erro: " + objCode.Value + "<br />";
                                }

                                if (item.Name.LocalName == "Reason")
                                {
                                    var listaItensReason = item.Nodes().ToList();
                                    XElement objReason = (XElement)listaItensReason[0];
                                    msgErroRetornoVeiculoPassagens = msgErroRetornoVeiculoPassagens + "Msg. do Erro: " + objReason.Value + "<br />";
                                }

                                if (item.Name.LocalName == "Detail")
                                {
                                    msgErroRetornoVeiculoPassagens = msgErroRetornoVeiculoPassagens + "Código da Placa: " + item.Value;
                                }
                            }
                        }
                    }

                    #region Envia E-mail caso haja erro

                    ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();
                    var idCargaStr = idCarga.ToString();

                    var verificaExisteEmail = apoloService.WORKFLOW_EMAIL
                        .Where(w => w.WorkFlowEmailDocNum == idCargaStr && w.WorkFlowEmailDocEspec == "HCAR")
                        .Count();

                    if (verificaExisteEmail == 0 && msgErroRetornoVeiculoPassagens != "")
                    {
                        ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                        apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                        email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                        email.WorkFlowEmailStat = "Enviar";
                        email.WorkFlowEmailAssunto = "ERRO - COLETA HORÁRIOS CARREGAMENTO";
                        email.WorkFlowEmailData = DateTime.Now;
                        email.WorkFlowEmailParaNome = "Logística";
                        email.WorkFlowEmailParaEmail = "logistica@hyline.com.br";
                        //email.WorkFlowEmailParaEmail = "palves@hyline.com.br";
                        email.WorkFlowEmailDeNome = "Integração WEB x Embarcador";
                        email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                        email.WorkFlowEmailFormato = "Html";
                        email.WorkFlowEmailCopiaPara = "";
                        email.WorkFlowEmailDocEspec = "HCAR";
                        email.WorkFlowEmailDocNum = idCarga.ToString();
                        email.WorkFlowEmailDocSerie = "";

                        var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                            + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                            + carga.EmpresaTranportador;

                        string corpoEmail = "Prezados, <br /><br />" +
                            "Ocorreu o erro abaixo ao tentar coletar os horários de carregamento da carga do Embarcador! "
                            + "Segue abaixo os dados:<br /><br />"
                            + "<b>Dados da Carga:</b> " + nomeRoteiro + "<br /><br />"
                            + "<b>Dados do Erro:</b><br />" + msgErroRetornoVeiculoPassagens + "<br /><br />"
                            + "Por favor, verifique e realize as correções necessárias!<br /><br />"
                            + "Integração WEB x Embarcador";

                        email.WorkFlowEmailCorpo = corpoEmail;

                        apoloService.WORKFLOW_EMAIL.AddObject(email);
                        apoloService.SaveChanges();

                        return "";
                    }

                    #endregion
                }

                #endregion

                //if (carga.InicioCarregamentoReal == null && dataHoraInicioCarregamento != Convert.ToDateTime("01/01/0001"))
                if (dataHoraInicioCarregamento != Convert.ToDateTime("01/01/0001"))
                    carga.InicioCarregamentoReal = dataHoraInicioCarregamento.ToString("HH:mm");
                //if (carga.TerminoCarregamentoReal == null && dataHoraFimCarregamento != Convert.ToDateTime("01/01/0001"))
                if (dataHoraFimCarregamento != Convert.ToDateTime("01/01/0001"))
                    carga.TerminoCarregamentoReal = dataHoraFimCarregamento.ToString("HH:mm");

                #endregion

                carga.IDCargaEmbarcador = idCargaEmbarcador;
                hlbapp.SaveChanges();
            }
            catch (Exception ex)
            {
                #region Envia E-mail caso haja erro

                ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

                ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "**** ERRO NA COLETA DOS DADOS DA VIAGEM DO EMBARCADOR ****";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "TI";
                email.WorkFlowEmailParaEmail = "ti@hyline.com.br";
                email.WorkFlowEmailDeNome = "Integração WEB x Embarcador";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Html";
                email.WorkFlowEmailCopiaPara = "";
                email.WorkFlowEmailDocEspec = "";
                email.WorkFlowEmailDocNum = idCarga.ToString();
                email.WorkFlowEmailDocSerie = "";

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string erro = "Linha do código do erro: " + linenum.ToString()
                    + " - " + ex.Message;
                if (ex.InnerException != null)
                    erro = erro + " / Erro interno: " + ex.InnerException.Message;

                string corpoEmail = "";
                corpoEmail = "Erro ao coletar os Dados da Viagem no Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "ID Carga: " + idCarga.ToString() + (char)13 + (char)10
                    + "Mensagem do Erro: " + erro;

                email.WorkFlowEmailCorpo = corpoEmail;

                apoloService.WORKFLOW_EMAIL.AddObject(email);
                apoloService.SaveChanges();

                #endregion
            }

            return msgRetorno;
        }

        public static void AtualizaDadosViagem()
        {
            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            DateTime dataMinima = DateTime.Today.AddDays(-7);
            //DateTime dataMaxima = DateTime.Today.AddDays(-3);
            DateTime dataMaxima = DateTime.Today;

            var listaCargasAtualizaDados = hlbapp.Prog_Diaria_Transp_Veiculos
                .Where(w => w.DataEmbarque >= dataMinima && w.DataEmbarque <= dataMaxima
                    //&& (w.IDCargaEmbarcador == null || w.IDCargaEmbarcador == 0)
                    && hlbapp.Prog_Diaria_Transp_Pedidos
                        .Any(a => a.EmpresaTranportador == w.EmpresaTranportador
                            && a.DataProgramacao == w.DataProgramacao
                            && a.NumVeiculo == w.NumVeiculo
                            //&& (a.ChegadaClienteReal.Replace(":", "").Trim() == "" || a.ChegadaClienteReal == null)
                            )
                    //&& w.CargaLiberada == 1
                    && w.Placa != null
                    //&& w.ID == 15485
                    )
                .ToList();

            foreach (var carga in listaCargasAtualizaDados)
            {
                try
                {
                    string retorno = RetornaDadosViagem(carga.ID);

                    HLBAPPEntities1 hlbapp1 = new HLBAPPEntities1();
                    Prog_Diaria_Transp_Veiculos objCarga = hlbapp1.Prog_Diaria_Transp_Veiculos
                        .Where(w => w.ID == carga.ID).FirstOrDefault();
                    int idCargaEmbarcador = 0;
                    if (objCarga.IDCargaEmbarcador != null) idCargaEmbarcador = (int)objCarga.IDCargaEmbarcador;

                    GeraLOGIntegracaoEmbarcador(carga.ID, "Retorno Dados da Viagem", idCargaEmbarcador, retorno);
                }
                catch (Exception ex)
                {
                    #region Envia E-mail caso haja erro

                    ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

                    ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();

                    ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                    apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                    email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                    email.WorkFlowEmailStat = "Enviar";
                    email.WorkFlowEmailAssunto = "**** ERRO NA COLETA DOS DADOS DA VIAGEM DO EMBARCADOR ****";
                    email.WorkFlowEmailData = DateTime.Now;
                    email.WorkFlowEmailParaNome = "Logística";
                    email.WorkFlowEmailParaEmail = "ti@hyline.com.br";
                    email.WorkFlowEmailDeNome = "Serviço de Importação";
                    email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                    email.WorkFlowEmailFormato = "Texto";
                    email.WorkFlowEmailCopiaPara = "";
                    email.WorkFlowEmailDocEspec = carga.NumVeiculo.ToString();
                    email.WorkFlowEmailDocNum = Convert.ToDateTime(carga.DataProgramacao).ToShortDateString();
                    email.WorkFlowEmailDocSerie = carga.EmpresaTranportador;

                    int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                    string erro = "Linha do código do erro: " + linenum.ToString()
                        + " - " + ex.Message;
                    if (ex.InnerException != null)
                        erro = erro + " / Erro interno: " + ex.InnerException.Message;

                    string corpoEmail = "";
                    corpoEmail = "Erro ao coletar os Dados da Viagem no Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                        + "Data do Nascimento: " + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + (char)13 + (char)10
                        + "Número da Carga: " + carga.NumVeiculo.ToString() + (char)13 + (char)10
                        + "Empresa Responsável pelo Transporte: " + carga.EmpresaTranportador + (char)13 + (char)10
                        + "Mensagem do Erro: " + erro;

                    email.WorkFlowEmailCorpo = corpoEmail;

                    apoloService.WORKFLOW_EMAIL.AddObject(email);
                    apoloService.SaveChanges();

                    #endregion
                }
            }

            hlbapp.SaveChanges();
        }

        public static void ImportaCargasEmbarcador()
        {
            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            #region Importa Cargas para Embarcador

            var listaCargas = hlbapp.Prog_Diaria_Transp_Veiculos
                .Where(w =>
                    //w.DataProgramacao == DateTime.Today
                    w.DataEmbarque == DateTime.Today
                    &&
                    (w.CargaLiberada == null || w.CargaLiberada == 0)
                        //&& w.ID == 15431
                    && w.NumVeiculo > 0
                    && w.QuantidadeTotal > 0)
                .ToList();

            foreach (var item in listaCargas)
            {
                var listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.DataProgramacao == item.DataProgramacao
                        && w.EmpresaTranportador == item.EmpresaTranportador
                        && w.NumVeiculo == item.NumVeiculo
                        && w.Produto != null)
                    .ToList();

                if (listaPedidos.Count > 0)
                {
                    if (item.Placa != "" && item.Placa != null)
                    {
                        var primeiroPedido = listaPedidos
                            .OrderBy(o => o.DataEntrega).ThenBy(t => t.InicioCarregamentoEsperado)
                            .FirstOrDefault();

                        if (primeiroPedido != null)
                        {
                            if (primeiroPedido.InicioCarregamentoEsperado.Replace(":", "").Replace(" ", "") != "")
                            {
                                DateTime dataCarregamento = Convert.ToDateTime(Convert.ToDateTime(item.DataEmbarque).ToShortDateString()
                                    + " " + primeiroPedido.InicioCarregamentoEsperado);

                                if ((dataCarregamento - DateTime.Now).TotalHours <= 2)
                                {
                                    string msg = IntegraPedidoEmbarcador(primeiroPedido.ID, Convert.ToDateTime(item.DataEmbarque));
                                    //int idCarga = Convert.ToInt32(Embarcador.buscaCargaCodigo(item.ID));
                                    //Embarcador.liberaCarga(idCarga);
                                    if (msg != "")
                                    {
                                        #region Envia E-mail caso haja erro

                                        ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

                                        ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();

                                        string data = Convert.ToDateTime(item.DataProgramacao).ToShortDateString();
                                        string numVeiculo = item.NumVeiculo.ToString();
                                        int existe = apoloService.WORKFLOW_EMAIL
                                            .Where(w => w.WorkFlowEmailDocEspec == numVeiculo
                                                && w.WorkFlowEmailDocNum == data
                                                && w.WorkFlowEmailDocSerie == item.EmpresaTranportador
                                                && (w.WorkFlowEmailAssunto == "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****"
                                                    || w.WorkFlowEmailAssunto == "ERRO - INTEGRAÇÃO WEB x EMBARCADOR"))
                                            .Count();

                                        if (existe == 0)
                                        {
                                            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                                            apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                                            string emailPara = "hyline.com.br";
                                            if (primeiroPedido.Empresa == "LB") emailPara = "ltz.com.br";
                                            else if (primeiroPedido.Empresa == "HN") emailPara = "hnavicultura.com.br";
                                            else if (primeiroPedido.Empresa == "PL") emailPara = "planaltopostura.com.br";

                                            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                                            email.WorkFlowEmailStat = "Enviar";
                                            email.WorkFlowEmailAssunto = "ERRO - INTEGRAÇÃO WEB x EMBARCADOR";
                                            email.WorkFlowEmailData = DateTime.Now;
                                            email.WorkFlowEmailParaNome = "Logística";
                                            email.WorkFlowEmailParaEmail = "programacao@" + emailPara;
                                            email.WorkFlowEmailDeNome = "Integração WEB x Embarcador";
                                            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                                            email.WorkFlowEmailFormato = "Html";
                                            email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";
                                            email.WorkFlowEmailDocEspec = item.NumVeiculo.ToString();
                                            email.WorkFlowEmailDocNum = Convert.ToDateTime(item.DataProgramacao).ToShortDateString();
                                            email.WorkFlowEmailDocSerie = item.EmpresaTranportador;

                                            string corpoEmail = "Prezados, <br /><br />"
                                                + "Ocorreu o erro abaixo ao realizar INTEGRAÇÃO WEB x EMBARCADOR! "
                                                + "Segue abaixo os dados:<br /><br />"
                                                + "<b>Data do Nascimento:<b/> " + Convert.ToDateTime(item.DataProgramacao).ToShortDateString() + "<br />"
                                                + "<b>Número da Carga:<b/> " + item.NumVeiculo.ToString() + "<br />"
                                                + "<b>Empresa Responsável pelo Transporte: " + item.EmpresaTranportador + "<br />"
                                                + "<b>Mensagem do Erro:<b/>" + msg + "<br /><br />"
                                                + "Por favor, verifique e realize as correções necessárias!<br /><br />"
                                                + "Integração WEB x Embarcador";

                                            email.WorkFlowEmailCorpo = corpoEmail;

                                            apoloService.WORKFLOW_EMAIL.AddObject(email);
                                            apoloService.SaveChanges();
                                        }

                                        if (msg.Contains("Pedido ja esta carregado"))
                                            item.CargaLiberada = 1;
                                        else
                                            item.CargaLiberada = 0;

                                        #endregion
                                    }
                                    else
                                    {
                                        //item.IDCargaEmbarcador = idCarga;
                                        item.CargaLiberada = 1;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        #region Envia E-mail caso haja erro

                        ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

                        ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();

                        string data = Convert.ToDateTime(item.DataProgramacao).ToShortDateString();
                        string numVeiculo = item.NumVeiculo.ToString();
                        int existe = apoloService.WORKFLOW_EMAIL
                            .Where(w => w.WorkFlowEmailDocEspec == numVeiculo
                                && w.WorkFlowEmailDocNum == data
                                && w.WorkFlowEmailDocSerie == item.EmpresaTranportador
                                && w.WorkFlowEmailAssunto == "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****")
                            .Count();

                        if (existe == 0)
                        {
                            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                            apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                            string emailPara = "";
                            if (item.EmpresaTranportador == "TR") emailPara = "hyline.com.br";
                            else if (item.EmpresaTranportador == "HN") emailPara = "hnavicultura.com.br";
                            else if (item.EmpresaTranportador == "PL") emailPara = "planaltopostura.com.br";

                            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                            email.WorkFlowEmailStat = "Enviar";
                            email.WorkFlowEmailAssunto = "**** ERRO IMPORTAÇÃO AUTOMÁTICA CHIC P/ EMBARCADOR ****";
                            email.WorkFlowEmailData = DateTime.Now;
                            email.WorkFlowEmailParaNome = "Logística";
                            email.WorkFlowEmailParaEmail = "programacao@" + emailPara;
                            email.WorkFlowEmailDeNome = "Serviço de Importação";
                            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                            email.WorkFlowEmailFormato = "Texto";
                            email.WorkFlowEmailCopiaPara = "logistica@hyline.com.br";
                            email.WorkFlowEmailDocEspec = item.NumVeiculo.ToString();
                            email.WorkFlowEmailDocNum = Convert.ToDateTime(item.DataProgramacao).ToShortDateString();
                            email.WorkFlowEmailDocSerie = item.EmpresaTranportador;

                            string erro = "";
                            if (item.Placa == "" || item.Placa == null) erro = "Carga sem placa vinculada!";
                            //if (listaPedidos.Count == 0)
                            //{
                            //    if (erro == "")
                            //        erro = "Carga sem pedidos vinculados!";
                            //    else
                            //        erro = erro + " / Carga sem pedidos vinculados!";
                            //}

                            string corpoEmail = "";
                            corpoEmail = "Erro ao realizar Importação Automática de Pedidos do CHIC p/ o Embarcador: " + (char)13 + (char)10 + (char)13 + (char)10
                                + "Data do Nascimento: " + Convert.ToDateTime(item.DataProgramacao).ToShortDateString() + (char)13 + (char)10
                                + "Número da Carga: " + item.NumVeiculo.ToString() + (char)13 + (char)10
                                + "Empresa Responsável pelo Transporte: " + item.EmpresaTranportador + (char)13 + (char)10
                                + "Mensagem do Erro: " + erro;

                            email.WorkFlowEmailCorpo = corpoEmail;

                            apoloService.WORKFLOW_EMAIL.AddObject(email);
                            apoloService.SaveChanges();
                        }

                        #endregion
                    }
                }
            }

            hlbapp.SaveChanges();

            #endregion
        }

        #endregion

        #region Fluig

        public static void GeraRoteirosEntregaFluig()
        {
            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
            Models.Apolo.ApoloEntities apoloSession = new Models.Apolo.ApoloEntities();

            DateTime dataInicioPlanalto = Convert.ToDateTime("19/08/2019");
            //DateTime dataHoje = Convert.ToDateTime("12/04/2021");
            DateTime dataIni = Convert.ToDateTime("02/05/2021");
            DateTime dataFim = Convert.ToDateTime("08/05/2021");
            DateTime dataOntem = DateTime.Today.AddDays(-1);
            DateTime dataHoje = DateTime.Today;
            //DateTime dataHoje = Convert.ToDateTime("02/04/2021");
            DateTime dataInicioHEN = Convert.ToDateTime("27/04/2020");

            var listaREFluig = hlbappSession.Prog_Diaria_Transp_Pedidos
                .Where(w => 
                    (w.NumRoteiroEntregaFluig == "" || w.NumRoteiroEntregaFluig == null)
                    //&& w.CHICNum == "10327"
                    && hlbappSession.Prog_Diaria_Transp_Veiculos
                        .Any(a => a.EmpresaTranportador == w.EmpresaTranportador
                            && a.DataProgramacao == w.DataProgramacao
                            && a.NumVeiculo == w.NumVeiculo
                            //&& a.DataEmbarque == dataHoje
                            //&& a.ID == 47857
                            && a.DataEmbarque >= dataOntem && a.DataEmbarque <= dataHoje
                            //&& a.DataEmbarque >= dataIni && a.DataEmbarque <= dataFim
                            && a.Placa != null
                            // Data que vai iniciar o processo na Transema
                            && a.DataEmbarque >= dataInicioPlanalto)
                    && (w.EmpresaTranportador == "TR" 
                        || (w.EmpresaTranportador == "HN" && w.DataProgramacao >= dataInicioHEN)
                        || w.EmpresaTranportador == "PL"
                        )
                    && w.NumVeiculo > 0
                    && !w.Produto.Contains("H"))
                .GroupBy(g => new
                {
                    g.EmpresaTranportador,
                    g.DataProgramacao,
                    g.Empresa,
                    g.CodigoCliente,
                    g.NumVeiculo
                })
                .Select(s => new
                {
                    s.Key.EmpresaTranportador,
                    s.Key.DataProgramacao,
                    s.Key.Empresa,
                    s.Key.CodigoCliente,
                    s.Key.NumVeiculo
                })
                .ToList();

            foreach (var item in listaREFluig)
            {
                var listaPedidos = new List<string>();

                var listaPedidosVenda = hlbappSession.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == item.EmpresaTranportador
                        && w.DataProgramacao == item.DataProgramacao
                        && w.Empresa == item.Empresa
                        && w.CodigoCliente == item.CodigoCliente
                        && w.NumVeiculo == item.NumVeiculo
                        && w.CHICNum != "")
                    .GroupBy(g => new
                    {
                        g.CHICNum,
                    })
                    .Select(s => new
                    {
                        s.Key.CHICNum
                    })
                    .ToList();

                foreach (var venda in listaPedidosVenda)
                {
                    listaPedidos.Add(venda.CHICNum);
                }

                var listaPedidosReposicao = hlbappSession.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == item.EmpresaTranportador
                        && w.DataProgramacao == item.DataProgramacao
                        && w.Empresa == item.Empresa
                        && w.CodigoCliente == item.CodigoCliente
                        && w.NumVeiculo == item.NumVeiculo
                        && w.CHICNumReposicao != "")
                    .GroupBy(g => new
                    {
                        g.CHICNumReposicao,
                    })
                    .Select(s => new
                    {
                        s.Key.CHICNumReposicao
                    })
                    .ToList();

                foreach (var reposicao in listaPedidosReposicao)
                {
                    listaPedidos.Add(reposicao.CHICNumReposicao);
                }

                List<string> pedidos = new List<string>();

                foreach (var pedido in listaPedidos)
                {
                    #region Verifica se todos pedidos tem NF. Caso algum falte, não gera o Roteiro

                    var nf = apoloSession.NOTA_FISCAL
                        .Where(w => apoloSession.PED_VENDA1.Any(a => a.EmpCod == w.EmpCod && a.PedVendaNum == w.NFPedVenda && a.USERPEDCHIC == pedido)
                            && apoloSession.ITEM_NF.Any(i => i.EmpCod == w.EmpCod && i.CtrlDFModForm == w.CtrlDFModForm && i.CtrlDFSerie == w.CtrlDFSerie
                                && i.NFNum == w.NFNum
                                && apoloSession.PRODUTO.Any(p => i.ProdCodEstr == p.ProdCodEstr
                                        && (p.FxaProdCod == "7" || p.FxaProdCod == "8"))))
                        .FirstOrDefault();

                    if (nf != null)
                    {
                        if ((DateTime.Now - Convert.ToDateTime(nf.NFDataSaidaEntrada)).TotalHours > 4)
                            pedidos.Add(pedido);
                    }
                    else
                    {
                        pedidos = new List<string>();
                        break;
                    }

                    #endregion
                }

                if (pedidos.Count > 0)
                    IntegraRoteiroEntregaFluig(item.Empresa, item.EmpresaTranportador, Convert.ToDateTime(item.DataProgramacao), pedidos);
            }
        }

        public static void IntegraRoteiroEntregaFluig(string empresa, string empresaTransportador, 
            DateTime dataNascimento, List<string> listaPedidosCHIC)
        {
            try
            {
                #region Cria Variáveis

                HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();

                List<keyValueDto> listaDadosFormulario = new List<keyValueDto>();
                string nomeCliente = "";
                string codigoCliente = "";
                string cidadeUf = "";
                string representante = "";
                string nfs = "";
                string motorista01 = "";
                string motorista02 = "";
                string horaCarregamentoEsperado = "";
                string horaCarregamentoReal = "";
                string atrasoCarregamento = "";
                string responsavelPreenchimento = "";
                string placa = "";
                DateTime? dataHoraChegadaEsperadaCliente = null;
                string dataHoraChegadaEsperadaClienteStr = "";
                int count = 0;
                int countItens = 1;

                #endregion

                foreach (var chicNum in listaPedidosCHIC)
                {
                    #region Carrega Dados dos Motoristas / Entrega

                    var pdtP = hlbappSession.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.CHICNum == chicNum || w.CHICNumReposicao == chicNum).FirstOrDefault();

                    if (pdtP.ChegadaClienteEsperado != null && pdtP.ChegadaClienteEsperado != "" 
                        && pdtP.ChegadaClienteEsperado.Contains(":") && pdtP.ChegadaClienteEsperado.Length == 5)
                    {
                        DateTime dataHoraChegadaEsperadaClientePedido = Convert.ToDateTime(Convert.ToDateTime(pdtP.DataEntrega).ToString("dd/MM/yyyy") 
                            + " " + pdtP.ChegadaClienteEsperado);
                        if (dataHoraChegadaEsperadaCliente == null || dataHoraChegadaEsperadaClientePedido < dataHoraChegadaEsperadaCliente)
                        {
                            horaCarregamentoEsperado = pdtP.InicioCarregamentoEsperado;
                            dataHoraChegadaEsperadaCliente = dataHoraChegadaEsperadaClientePedido;
                            dataHoraChegadaEsperadaClienteStr = 
                                Convert.ToDateTime(dataHoraChegadaEsperadaCliente).ToString("yyyy-MM-dd")+"T"
                                    +Convert.ToDateTime(dataHoraChegadaEsperadaCliente).ToString("HH:mm");
                        }
                    }

                    var pdtV = hlbappSession.Prog_Diaria_Transp_Veiculos
                        .Where(w => w.EmpresaTranportador == pdtP.EmpresaTranportador
                            && w.DataProgramacao == pdtP.DataProgramacao
                            && w.NumVeiculo == pdtP.NumVeiculo)
                        .FirstOrDefault();

                    if (pdtV != null)
                    {
                        motorista01 = pdtV.Motorista01;
                        motorista02 = pdtV.Motorista02;
                        horaCarregamentoReal = pdtV.InicioCarregamentoReal;
                        placa = pdtV.Placa.Replace(" ","").ToUpper();

                        if (empresaTransportador == "TR")
                            responsavelPreenchimento = "w1tlgzalex97d5hu1538511723111";
                        else
                        {
                            var emailApolo = apoloSession.ENT_WEB
                                .Where(w => w.EntCod == pdtV.EntCod && w.EntWebEMailPrinc == "Sim")
                                .FirstOrDefault();
                            if (emailApolo != null) responsavelPreenchimento = emailApolo.EntWebEMail.Trim()
                                    .Substring(0, emailApolo.EntWebEMail.Trim().IndexOf("@"));
                        }
                    }

                    #endregion

                    #region Carrega NF do Pedido

                    Models.Apolo.ApoloEntities apolo02 = new Models.Apolo.ApoloEntities();
                    Models.Apolo.NOTA_FISCAL nf = apolo02.NOTA_FISCAL
                        .Where(w => apolo02.PED_VENDA1.Any(a => a.EmpCod == w.EmpCod
                            && a.PedVendaNum == w.NFPedVenda
                            && a.USERPEDCHIC == chicNum)
                            && apolo02.ITEM_NF.Any(i => i.EmpCod == w.EmpCod
                                && i.CtrlDFModForm == w.CtrlDFModForm
                                && i.CtrlDFSerie == w.CtrlDFSerie
                                && i.NFNum == w.NFNum
                                && apolo02.PRODUTO.Any(p => i.ProdCodEstr == p.ProdCodEstr
                                    && (p.FxaProdCod == "7" || p.FxaProdCod == "8"))))
                        .FirstOrDefault();

                    #endregion

                    if (nf != null)
                    {
                        #region Dados da NF

                        count++;

                        nomeCliente = nf.NFENTNOME;
                        codigoCliente = nf.EntCod;
                        cidadeUf = nf.NFCIDNOME + "/" + nf.NFUFSIGLA;
                        if (count != listaPedidosCHIC.Count)
                            nfs = nfs + nf.NFNum + " / ";
                        else
                            nfs = nfs + nf.NFNum;

                        Models.Apolo.VENDEDOR vendedor = apolo02.VENDEDOR
                            .Where(w => apolo02.VEND_NF.Any(a => a.VendCod == w.VendCod
                                && a.EmpCod == nf.EmpCod && a.CtrlDFModForm == nf.CtrlDFModForm
                                && a.CtrlDFSerie == nf.CtrlDFSerie && a.NFNum == nf.NFNum))
                            .FirstOrDefault();

                        if (vendedor != null) representante = vendedor.VendNome;

                        var listaItens = apolo02.ITEM_NF
                            .Where(w => w.EmpCod == nf.EmpCod && w.CtrlDFModForm == nf.CtrlDFModForm
                                && w.CtrlDFSerie == nf.CtrlDFSerie && w.NFNum == nf.NFNum)
                            .ToList();

                        #endregion

                        #region Carrega os itens

                        for (int i = 0; i < listaItens.Count; i++)
                        {
                            #region Carrega Linhagem Comercial

                            string prodcodestr = listaItens[i].ProdCodEstr;
                            PRODUTO produto = apoloSession.PRODUTO
                                .Where(w => w.ProdCodEstr == prodcodestr).FirstOrDefault();
                            LINHAGEM_GRUPO linha = hlbappSession.LINHAGEM_GRUPO
                                .Where(w => w.LinhagemFLIP == produto.ProdNomeAlt2
                                    && w.Empresa == empresa).FirstOrDefault();
                            string linhaComercial = produto.ProdNome;
                            if (linha != null) linhaComercial = linha.LinhagemComercial;

                            #endregion

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "chic_num___" + countItens,
                                value = chicNum
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "prodcodestr_item___" + countItens,
                                value = listaItens[i].ProdCodEstr
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "produto_item___" + countItens,
                                value = linhaComercial
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "qtde_item___" + countItens,
                                value = String.Format("{0:N0}", listaItens[i].ItNFQtd)
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "existe_doa_item___" + countItens,
                                value = ""
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "empresa_item___" + countItens,
                                value = nf.EmpCod
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "especie_item___" + countItens,
                                value = nf.CtrlDFModForm
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "serie_item___" + countItens,
                                value = nf.CtrlDFSerie
                            });

                            listaDadosFormulario.Add(new keyValueDto
                            {
                                key = "nf_item___" + countItens,
                                value = nf.NFNum
                            });

                            countItens++;
                        }

                        #endregion
                    }
                }

                ECMWorkflowEngineServiceService client = new ECMWorkflowEngineServiceService();

                #region Carrega Dados Formulario

                listaDadosFormulario.Add(new keyValueDto
                {
                    key = "filtra_roteiro",
                    value = placa + " - " + dataNascimento.ToString("dd/MM/yy") + " - " + nomeCliente
                });
                listaDadosFormulario.Add(new keyValueDto
                {
                    key = "data_nascimento",
                    value = dataNascimento.ToString("dd/MM/yyyy")
                });
                listaDadosFormulario.Add(new keyValueDto { key = "nome_cliente", value = nomeCliente });
                listaDadosFormulario.Add(new keyValueDto { key = "codigo_cliente", value = codigoCliente });
                listaDadosFormulario.Add(new keyValueDto { key = "cidade_uf", value = cidadeUf });
                listaDadosFormulario.Add(new keyValueDto { key = "representante", value = representante });
                listaDadosFormulario.Add(new keyValueDto { key = "nfnum", value = nfs });
                listaDadosFormulario.Add(new keyValueDto { key = "distancia_percorrida_km", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "horario_chegada_granja", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "horario_saida_granja", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "data_entrega", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "motorista_01", value = motorista01 });
                listaDadosFormulario.Add(new keyValueDto { key = "motorista_02", value = motorista02 });
                listaDadosFormulario.Add(new keyValueDto { key = "observacoes_motorista", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "recebedor", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "recebedor_cpf", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "recebedor_celular", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "avaliacao_transporte", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "avaliacao_motorista", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "avaliacao_pintinho", value = "" });
                listaDadosFormulario.Add(new keyValueDto { key = "responsavel_preenchimento", value = responsavelPreenchimento });
                listaDadosFormulario.Add(new keyValueDto { key = "data_hora_esperada_granja", value = dataHoraChegadaEsperadaClienteStr });

                listaDadosFormulario.Add(new keyValueDto { key = "placa", value = placa });
                listaDadosFormulario.Add(new keyValueDto { key = "horario_carregamento_esperado", value = horaCarregamentoEsperado });
                listaDadosFormulario.Add(new keyValueDto { key = "horario_carregamento_real", value = horaCarregamentoReal });

                #region Calcular horas em atraso / adiantado de carregamento

                if (horaCarregamentoEsperado != "" && horaCarregamentoReal != "" && horaCarregamentoEsperado != null && horaCarregamentoReal != null)
                {
                    TimeSpan horaEsperada = new TimeSpan(Convert.ToInt32(horaCarregamentoEsperado.Substring(0, 2)), Convert.ToInt32(horaCarregamentoEsperado.Substring(3, 2)), 0);
                    TimeSpan horaReal = new TimeSpan(Convert.ToInt32(horaCarregamentoReal.Substring(0, 2)), Convert.ToInt32(horaCarregamentoReal.Substring(3, 2)), 0);
                    atrasoCarregamento = String.Format("{0:N2}",Math.Round((horaReal - horaEsperada).TotalHours, 1));
                }

                #endregion

                listaDadosFormulario.Add(new keyValueDto { key = "horas_atraso_adiantado_carreg", value = atrasoCarregamento });

                keyValueDto[] dadosFormulario = listaDadosFormulario.ToArray();

                #endregion

                processAttachmentDto[] attachments = new processAttachmentDto[] { };
                processTaskAppointmentDto[] apps = new processTaskAppointmentDto[] { };

                string processo = "ROTEIROENTREGA";
                if (empresaTransportador == "PL") processo = "ROTEIROENTREGAPPA";
                if (empresaTransportador == "HN") processo = "ROTEIROENTREGAHEN";

                var retorno = client.startProcessClassic("sistemas@hyline.com.br", "123", 1, processo, 0,
                    new String[] { }, "", "fluig", true, attachments, dadosFormulario,
                    apps, false);

                if (retorno.Length > 1)
                {
                    #region Solicitação Gerada, retorna numero para vincular nos pedidos.

                    string numSolicitacaoFluig = retorno[5].value;

                    foreach (var chicNum in listaPedidosCHIC)
                    {
                        var listapdtP = hlbappSession.Prog_Diaria_Transp_Pedidos
                            .Where(w => w.CHICNum == chicNum || w.CHICNumReposicao == chicNum)
                            .ToList();

                        foreach (var item in listapdtP)
                        {
                            item.NumRoteiroEntregaFluig = numSolicitacaoFluig;
                        }
                    }

                    hlbappSession.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Solicitação com erro. É gerado um e-mail e enviado para TI e logística.

                    ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                    ImportaCHICService.Data.ApoloServiceEntities apoloService =
                        new ImportaCHICService.Data.ApoloServiceEntities();

                    if (retorno.Length == 1)
                    {
                        string msgErro = retorno[0].value;

                        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                        apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                        string pedidos = "";
                        foreach (var chicNUM in listaPedidosCHIC)
                        {
                            if (listaPedidosCHIC.IndexOf(chicNUM) != (listaPedidosCHIC.Count - 1))
                                pedidos = pedidos + chicNUM + " / ";
                            else
                                pedidos = pedidos + chicNUM;
                        }

                        email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                        email.WorkFlowEmailStat = "Enviar";
                        email.WorkFlowEmailAssunto = "[ERRO] ERRO NO FLUIG AO GERAR ROTEIRO DE ENTREGA NO FLUIG";
                        email.WorkFlowEmailData = DateTime.Now;
                        email.WorkFlowEmailParaNome = "Logística";
                        email.WorkFlowEmailParaEmail = "logistica@hyline.com.br";
                        email.WorkFlowEmailDeNome = "Serviço de Importação";
                        email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                        email.WorkFlowEmailFormato = "Texto";
                        email.WorkFlowEmailCopiaPara = "ti@hyline.com.br";
                        email.WorkFlowEmailDocEspec = "REF";
                        email.WorkFlowEmailDocNum = dataNascimento.ToShortDateString();
                        email.WorkFlowEmailDocSerie = empresa;

                        string corpoEmail = "";
                        corpoEmail = "Erro ao realizar Geração de Roteiro de Entrega no Fluig: " + (char)13 + (char)10 + (char)13 + (char)10
                            + "Data do Nascimento: " + dataNascimento.ToShortDateString() + (char)13 + (char)10
                            + "Pedidos: " + pedidos + (char)13 + (char)10
                            + "Empresa Responsável pelo Transporte: " + empresaTransportador + (char)13 + (char)10
                            + "Mensagem do Erro do FLUIG: " + msgErro;

                        email.WorkFlowEmailCorpo = corpoEmail;

                        apoloService.WORKFLOW_EMAIL.AddObject(email);
                        apoloService.SaveChanges();
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Se ocorrer erro na rotina, será gerado um e-mail e enviado para TI.

                ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                ImportaCHICService.Data.ApoloServiceEntities apoloService =
                    new ImportaCHICService.Data.ApoloServiceEntities();

                string msgErro = ex.Message;
                if (ex.InnerException != null)
                    msgErro = msgErro + " / Erro interno: " + ex.InnerException.Message;

                ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String)); ;

                apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

                string pedidos = "";
                foreach (var chicNUM in listaPedidosCHIC)
                {
                    if (listaPedidosCHIC.IndexOf(chicNUM) != (listaPedidosCHIC.Count - 1))
                        pedidos = pedidos + chicNUM + " / ";
                    else
                        pedidos = pedidos + chicNUM;
                }

                email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
                email.WorkFlowEmailStat = "Enviar";
                email.WorkFlowEmailAssunto = "[ERRO] ERRO NO WEB DESKTOP AO GERAR ROTEIRO DE ENTREGA NO FLUIG";
                email.WorkFlowEmailData = DateTime.Now;
                email.WorkFlowEmailParaNome = "Logística";
                email.WorkFlowEmailParaEmail = "logistica@hyline.com.br";
                email.WorkFlowEmailDeNome = "Serviço de Importação";
                email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
                email.WorkFlowEmailFormato = "Texto";
                email.WorkFlowEmailCopiaPara = "ti@hyline.com.br";
                email.WorkFlowEmailDocEspec = "REF";
                email.WorkFlowEmailDocNum = dataNascimento.ToShortDateString();
                email.WorkFlowEmailDocSerie = empresa;

                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                string corpoEmail = "";
                corpoEmail = "Erro ao realizar Geração de Roteiro de Entrega no Fluig: " + (char)13 + (char)10 + (char)13 + (char)10
                    + "Data do Nascimento: " + dataNascimento.ToShortDateString() + (char)13 + (char)10
                    + "Pedidos: " + pedidos + (char)13 + (char)10
                    + "Empresa Responsável pelo Transporte: " + empresaTransportador + (char)13 + (char)10
                    + "Linha de Código do Erro: " + linenum.ToString() + (char)13 + (char)10
                    + "Mensagem do Erro do WEB Desktop: " + msgErro;

                email.WorkFlowEmailCorpo = corpoEmail;

                apoloService.WORKFLOW_EMAIL.AddObject(email);
                apoloService.SaveChanges();

                #endregion
            }
        }

        public static void ImportaPlanilhaCheckListClassificadoraOvosFluig()
        {
            HLBAPPEntities1 hlbappSession02 = new HLBAPPEntities1();

            DateTime dataTeste = Convert.ToDateTime("2019-07-25 00:00:00.000");
            var lancamentosCKCOData = hlbappSession02.Lancamentos_Classificadora_Excel_02
                //.Where(w => w.LoteCompleto == "P027872W36" && w.Data == dataTeste)
                .GroupBy(g => new
                {
                    g.Data,
                    g.Lote,
                    g.LoteCompleto
                })
                .OrderBy(o => o.Key.Data).ThenBy(t => t.Key.Lote)
                .ToList();

            foreach (var inspecao in lancamentosCKCOData)
            {
                DateTime data = Convert.ToDateTime(inspecao.Key.Data);
                double lote = Convert.ToDouble(inspecao.Key.Lote);
                ImportaCheckListClassificadoraOvosFluig(data, lote, inspecao.Key.LoteCompleto);
            }
        }

        public static void ImportaCheckListClassificadoraOvosFluig(DateTime data, double lote, string loteCompleto)
        {
            try
            {
                #region Cria Variáveis

                HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
                
                List<keyValueDto> listaDadosFormulario = new List<keyValueDto>();

                // Pegar nome do campo
                //var listaColunas01 = GetMemberName(
                //    (Lancamentos_Classificadora_Excel_02 c) => c.C1_C2_C3).Split('_');
                
                #endregion

                #region Carrega linhas

                var listaLinhas = hlbappSession.Lancamentos_Classificadora_Excel_02
                    .Where(w => w.Data == data
                        && w.Lote == lote)
                    .ToList();

                #endregion

                int count = 1;

                foreach (var item in listaLinhas)
                {
                    #region Carrega dados das linhas

                    int tamanhoBandeja = 30;
                    //int qtdeColunas = 5;
                    int linha = Convert.ToInt32(item.Balança.Substring(1,1));
                    if (item.C22_C23_C24 > 0)
                    {
                        tamanhoBandeja = 150;
                        //qtdeColunas = 25;
                    }

                    #endregion

                    #region Grupo de Colunas 01

                    string coluna01 = "";
                    if (linha == 1 || linha == 6) coluna01 = "1";
                    if (linha == 2 || linha == 5) coluna01 = "2";
                    if (linha == 3 || linha == 4) coluna01 = "3";

                    if (coluna01 != "" && item.C1_C2_C3 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna01
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C1_C2_C3.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 02

                    string coluna02 = "";
                    if (tamanhoBandeja == 30)
                    {
                        if (linha == 1 || linha == 6) coluna02 = "5";
                        if (linha == 2 || linha == 5) coluna02 = "4";
                    }
                    else if (tamanhoBandeja == 150)
                    {
                        if (linha == 1 || linha == 6) coluna02 = "6";
                        if (linha == 2 || linha == 5) coluna02 = "5";
                        if (linha == 3 || linha == 4) coluna02 = "4";
                    }

                    if (coluna02 != "" && item.C4_C5_C6 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna02
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C4_C5_C6.ToString()
                        });

                        #endregion

                        count++;
                    }

                    #endregion

                    #region Grupo de Colunas 03

                    string coluna03 = "";
                    if (linha == 1 || linha == 6) coluna03 = "7";
                    if (linha == 2 || linha == 5) coluna03 = "8";
                    if (linha == 3 || linha == 4) coluna03 = "9";

                    if (coluna03 != "" && item.C7_C8_C9 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna03
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C7_C8_C9.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 04

                    string coluna04 = "";
                    if (linha == 1 || linha == 6) coluna04 = "12";
                    if (linha == 2 || linha == 5) coluna04 = "11";
                    if (linha == 3 || linha == 4) coluna04 = "10";

                    if (coluna04 != "" && item.C10_C11_C12 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna04
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C10_C11_C12.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 05

                    string coluna05 = "";
                    if (linha == 1 || linha == 6) coluna05 = "13";
                    if (linha == 2 || linha == 5) coluna05 = "14";
                    if (linha == 3 || linha == 4) coluna05 = "15";

                    if (coluna05 != "" && item.C13_C14_C15 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna05
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C13_C14_C15.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 06

                    string coluna06 = "";
                    if (linha == 1 || linha == 6) coluna06 = "18";
                    if (linha == 2 || linha == 5) coluna06 = "17";
                    if (linha == 3 || linha == 4) coluna06 = "16";

                    if (coluna06 != "" && item.C16_C17_C18 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna06
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C16_C17_C18.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 07

                    string coluna07 = "";
                    if (linha == 1 || linha == 6) coluna07 = "19";
                    if (linha == 2 || linha == 5) coluna07 = "20";
                    if (linha == 3 || linha == 4) coluna07 = "21";

                    if (coluna07 != "" && item.C19_C20_C21 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna07
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C19_C20_C21.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Grupo de Colunas 08

                    string coluna08 = "";
                    if (linha == 1 || linha == 6) coluna08 = "24";
                    if (linha == 2 || linha == 5) coluna08 = "23";
                    if (linha == 3 || linha == 4) coluna08 = "22";

                    if (coluna08 != "" && item.C22_C23_C24 > 0)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = coluna08
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Peso"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = item.C22_C23_C24.ToString()
                        });

                        #endregion
                    }

                    count++;

                    #endregion

                    #region Ovos Virados

                    int colunaOvoVirado = 2;

                    for (int i = 0; i < item.Quantos_Ovos_Virados_; i++)
                    {
                        #region Cabeçalho Linha da Bandeja

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "sequencia___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "numero_saida___" + count,
                            value = item.Saída.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "num_posicoes_bandeja___" + count,
                            value = tamanhoBandeja.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_ovo___" + count,
                            value = item.Tipo
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "linha___" + count,
                            value = linha.ToString()
                        });

                        #endregion

                        #region Cabeçalho Coluna

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "coluna___" + count,
                            value = colunaOvoVirado.ToString()
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "tipo_valor___" + count,
                            value = "Ovo Virado"
                        });

                        listaDadosFormulario.Add(new keyValueDto
                        {
                            key = "valor___" + count,
                            value = "X"
                        });

                        #endregion

                        count++;
                        colunaOvoVirado++;
                    }

                    #endregion
                }

                ECMWorkflowEngineServiceService client = new ECMWorkflowEngineServiceService();

                #region Carrega Dados Formulario

                listaDadosFormulario.Add(new keyValueDto { key = "prazo", value = "2019-09-10 23:59" });
                listaDadosFormulario.Add(new keyValueDto { key = "descricao", 
                    value = "Lote: " + loteCompleto + " - Data: " + data.ToShortDateString() });
                listaDadosFormulario.Add(new keyValueDto { key = "lote", value = loteCompleto });
                listaDadosFormulario.Add(new keyValueDto { key = "data_inspecao", 
                    value = data.ToString("yyyy-MM-dd") });
                listaDadosFormulario.Add(new keyValueDto { key = "responsavel_inspecao", value = "Adriana" });

                keyValueDto[] dadosFormulario = listaDadosFormulario.ToArray();

                #endregion

                processAttachmentDto[] attachments = new processAttachmentDto[] { };
                processTaskAppointmentDto[] apps = new processTaskAppointmentDto[] { };

                var retorno = client.startProcessClassic("sistemas@hyline.com.br", "123", 1, 
                    "CHECKLISTOVOSCLASSIFICADOS", 0,
                    new String[] { }, "", "classificadora-ng", true, attachments, dadosFormulario,
                    apps, false);

                if (retorno.Length > 1)
                {
                    #region Solicitação Gerada, retorna numero para vincular nos pedidos.

                    string numSolicitacaoFluig = retorno[5].value;

                    #endregion
                }
                else
                {
                    #region Solicitação com erro. É gerado um e-mail e enviado para TI e logística.

                    ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                    ImportaCHICService.Data.ApoloServiceEntities apoloService =
                        new ImportaCHICService.Data.ApoloServiceEntities();

                    if (retorno.Length == 1)
                    {
                        string msgErro = retorno[0].value;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Se ocorrer erro na rotina, será gerado um e-mail e enviado para TI.

                ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                ImportaCHICService.Data.ApoloServiceEntities apoloService =
                    new ImportaCHICService.Data.ApoloServiceEntities();

                string msgErro = ex.Message;
                if (ex.InnerException != null)
                    msgErro = msgErro + " / Erro interno: " + ex.InnerException.Message;

                #endregion
            }
        }

        public static int ParseInt32(string str, int defaultValue = 0)
        {
            int result;
            return Int32.TryParse(str, out result) ? result : defaultValue;
        }

        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            //return result.AddDays(-3).AddDays(6);
            return result.AddDays(-4);
        }

        #region Importações Manuais

        public static void GeraManutencaoPreventivaIncNMConfigApoloFluig()
        {
            FinanceiroEntities apolo = new FinanceiroEntities();
            Models.FLUIG.fluigtesteEntities fluigteste = new Models.FLUIG.fluigtesteEntities();

            int anoGerado = 2021;
            DateTime primeiroDiaAno = new DateTime(anoGerado, 1, 1);
            DateTime ultimoDiaAno = new DateTime(anoGerado, 12, 31);

            var listaTipoManutencao = apolo.OBJETO
                .Where(w => w.ObjCodEstr.Contains("003.")
                    && w.USERCodTipoManutEquip != null && w.USERCodTipoManutEquip != ""
                    //&& w.USERCodTipoManutEquip == "A0248"
                    )
                .OrderBy(o => o.USERCodTipoManutEquip)
                .ToList();

            foreach (var tipoManutencao in listaTipoManutencao)
            {
                int qtdOSPorPeriodo = (int)tipoManutencao.USERQtdeOSPeriodo;

                var motOcor = apolo.MOTIVO_OCOR
                    .Where(w => w.USERCodigoTPManut == tipoManutencao.USERCodTipoManutEquip)
                    .FirstOrDefault();

                if (motOcor != null)
                {
                    string motOcorCodEstr = motOcor.MotOcorCodEstr;

                    var listaEquipamentos = apolo.PRODUTO
                        .Where(w => w.ProdCodEstrNiv == "005.001"
                            && w.ProdNomeAlt1 != "" && w.ProdNomeAlt1 != null
                            && w.ProdStat == "Ativado"
                            && w.OBJETO.Any(o => o.ObjCodEstr == tipoManutencao.ObjCodEstr))
                        .ToList();

                    foreach (var equipamento in listaEquipamentos)
                    {
                        DateTime diaAtual = new DateTime(anoGerado, Convert.ToDateTime(motOcor.MotOcorDataValidIni).Month,
                            Convert.ToDateTime(motOcor.MotOcorDataValidIni).Day);
                        int anoDiaAtual = diaAtual.Year;
                        //int semanaDiaAtual = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(diaAtual, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                        int semanaDiaAtual = Convert.ToInt32(Math.Round(diaAtual.DayOfYear / 7.0m, 0)+1);

                        //int existe = fluigteste.VW_Manutencao_Preventiva
                        //    .Where(w => w.periodo_manutencao.Contains(tipoManutencao.USERCodTipoManutEquip)
                        //        && w.DataManutencao.Year == anoDiaAtual
                        //        //&& CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(w.DataManutencao, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) == semanaDiaAtual)
                        //        //&& (int)(Math.Round(w.DataManutencao.DayOfYear / 7.0m,0)+1) == semanaDiaAtual)
                        //        && w.SemanaAnoManutencao == semanaDiaAtual)
                        //    .Count();

                        int existe = 0;
                        //int existe = apolo.VU_Teste_Gera_Ocor_Auto
                        //    .Where(w => w.TipoServico.Contains(tipoManutencao.USERCodTipoManutEquip)
                        //        && w.DataServico.Year == anoDiaAtual && w.SemanaAnoServico == semanaDiaAtual)
                        //    .Count();

                        //while (existe >= qtdOSPorPeriodo && tipoManutencao.USERTipoPeriodo != "Semanal")
                        //{
                        //    diaAtual = diaAtual.AddDays(7);
                        //    anoDiaAtual = diaAtual.Year;
                        //    semanaDiaAtual = Convert.ToInt32(Math.Round(diaAtual.DayOfYear / 7.0m, 0) + 1);

                        //    existe = apolo.VU_Teste_Gera_Ocor_Auto
                        //        .Where(w => w.TipoServico.Contains(tipoManutencao.USERCodTipoManutEquip)
                        //            && w.DataServico.Year == anoDiaAtual && w.SemanaAnoServico == semanaDiaAtual)
                        //        .Count();
                        //}

                        while (diaAtual <= ultimoDiaAno)
                        {
                            ////existe = fluigteste.VW_Manutencao_Preventiva
                            ////    .Where(w => w.periodo_manutencao.Contains(tipoManutencao.USERCodTipoManutEquip) && w.DataManutencao == diaAtual).Count();
                            existe = apolo.VU_Teste_Gera_Ocor_Auto
                                .Where(w => w.TipoServico.Contains(tipoManutencao.USERCodTipoManutEquip) 
                                    && w.DataServico == diaAtual
                                    && w.TAG == equipamento.ProdNomeAlt1).Count();

                            if (existe == 0)
                            {
                                int semana = Convert.ToInt32(Math.Round(diaAtual.DayOfYear / 7.0m, 0) + 1);
                                DateTime primeiroDiaSemana = FirstDateOfWeekISO8601(anoGerado, semana);

                                // Insere no Fluig aqui
                                Teste_Gera_Ocor_Auto teste = new Teste_Gera_Ocor_Auto();
                                teste.TAG = equipamento.ProdNomeAlt1;
                                teste.TipoServico = tipoManutencao.USERCodTipoManutEquip;
                                teste.PeriodoServico = tipoManutencao.USERTipoPeriodo;
                                //teste.DataServico = diaAtual;
                                teste.DataServico = primeiroDiaSemana;
                                apolo.Teste_Gera_Ocor_Auto.AddObject(teste);
                                apolo.SaveChanges();
                            }

                            if (tipoManutencao.USERTipoPeriodo == "Semanal")
                                diaAtual = diaAtual.AddDays(7);
                            else if (tipoManutencao.USERTipoPeriodo == "Quinzenal")
                            {
                                DateTime data = diaAtual;
                                DateTime dataInicialQuinzenaAtual = (data.Day < 15 ? new DateTime(data.Year, data.Month, 1) : new DateTime(data.Year, data.Month, 16));
                                DateTime dataInicialQuinzenaProxima = (data.Day < 15 ? new DateTime(data.Year, data.Month, 16) :
                                    new DateTime(data.AddDays(15).Year, data.AddDays(15).Month, 1));
                                int diasDif = (data - dataInicialQuinzenaAtual).Days;
                                data = dataInicialQuinzenaProxima.AddDays(diasDif);
                                diaAtual = data;
                            }
                            else if (tipoManutencao.USERTipoPeriodo == "Mensal")
                                diaAtual = diaAtual.AddMonths(1);
                            else if (tipoManutencao.USERTipoPeriodo == "Bimestral")
                                diaAtual = diaAtual.AddMonths(2);
                            else if (tipoManutencao.USERTipoPeriodo == "Trimestral")
                                diaAtual = diaAtual.AddMonths(3);
                            else if (tipoManutencao.USERTipoPeriodo == "Semestral")
                                diaAtual = diaAtual.AddMonths(6);
                            else if (tipoManutencao.USERTipoPeriodo == "Anual")
                                diaAtual = diaAtual.AddYears(1);
                        }
                    }
                }
            }
        }

        public static void GeraPDI(int ano)
        {
            try
            {
                #region Cria Variáveis

                HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();

                var listaPDIPorColaborador = 
                    hlbappSession.PDI
                        .Where(w => w.Ano_Avaliação == ano
                            && (w.Usuario_Colaborador == "oqs974fmjcffgjn61538016128362" || w.Usuario_Colaborador == "na2m0tr0uj9ojwvz1538016128612")
                            )
                        .GroupBy(g => new 
                        {
                            g.Usuario_Colaborador,
                            g.Colaborador,
                            g.Usuario_Lider,
                            g.Líder
                        })
                        .Select(s => new
                        {
                            s.Key.Usuario_Colaborador,
                            s.Key.Colaborador,
                            s.Key.Usuario_Lider,
                            s.Key.Líder
                        })
                        .OrderBy(o => o.Colaborador)
                        .ToList();

                #endregion

                foreach (var colaborador in listaPDIPorColaborador)
                {
                    var listaComportamentos = hlbappSession.PDI
                        .Where(w => w.Ano_Avaliação == ano && w.Usuario_Colaborador == colaborador.Usuario_Colaborador)
                        .ToList();

                    List<keyValueDto> listaDadosFormulario = new List<keyValueDto>();

                    #region Calcula Prazos

                    // Como 2020 iniciou em maio, os "Qs" serão de 3 em 3 meses, ou seja Maio, Agosto e Novembro
                    string inicioQ1 = (Convert.ToDateTime("31/05/" + ano.ToString())).AddDays(-7).ToString("yyyy-MM-dd 00:00");
                    string prazoQ1 = ano.ToString() + "-05-31 23:59";
                    string inicioQ2 = (Convert.ToDateTime("31/08/" + ano.ToString())).AddDays(-7).ToString("yyyy-MM-dd 00:00");
                    string prazoQ2 = ano.ToString() + "-08-31 23:59";
                    string inicioQ3 = (Convert.ToDateTime("30/11/" + ano.ToString())).AddDays(-7).ToString("yyyy-MM-dd 00:00");
                    string prazoQ3 = ano.ToString() + "-11-31 23:59";
                    string inicioFinal = ano.ToString() + "-12-01 00:00";
                    string prazoFinal = (Convert.ToDateTime("31/01/" + (ano + 1).ToString())).ToString("yyyy-MM-dd 23:59");

                    #endregion

                    #region Carrega Dados Formulario

                    listaDadosFormulario.Add(new keyValueDto { key = "data_hora_inicio", value = DateTime.Now.ToString("yyyy-MM-ddThh:mm") });
                    listaDadosFormulario.Add(new keyValueDto { key = "identificador", value = colaborador.Colaborador + " - " + ano.ToString() });
                    listaDadosFormulario.Add(new keyValueDto { key = "ano_pdi", value = ano.ToString() });
                    listaDadosFormulario.Add(new keyValueDto { key = "inicio_q1", value = inicioQ1 });
                    listaDadosFormulario.Add(new keyValueDto { key = "prazo_q1", value = prazoQ1 });
                    listaDadosFormulario.Add(new keyValueDto { key = "inicio_q2", value = inicioQ2 });
                    listaDadosFormulario.Add(new keyValueDto { key = "prazo_q2", value = prazoQ2 });
                    listaDadosFormulario.Add(new keyValueDto { key = "inicio_q3", value = inicioQ3 });
                    listaDadosFormulario.Add(new keyValueDto { key = "prazo_q3", value = prazoQ3 });
                    listaDadosFormulario.Add(new keyValueDto { key = "inicio_final", value = inicioFinal });
                    listaDadosFormulario.Add(new keyValueDto { key = "prazo_final", value = prazoFinal });
                    listaDadosFormulario.Add(new keyValueDto { key = "colaborador_avaliado", value = colaborador.Colaborador });
                    listaDadosFormulario.Add(new keyValueDto { key = "usuario_colaborador_avaliado", value = colaborador.Usuario_Colaborador });
                    listaDadosFormulario.Add(new keyValueDto { key = "lider_avaliado", value = colaborador.Líder });
                    listaDadosFormulario.Add(new keyValueDto { key = "usuario_lider_avaliado", value = colaborador.Usuario_Lider });

                    int countItens = 1;
                    foreach (var comportamento in listaComportamentos)
                    {
                        listaDadosFormulario.Add(new keyValueDto { key = "descricao_comportamento___" + countItens, value = countItens.ToString() + " - " + comportamento.Comportamento });
                        listaDadosFormulario.Add(new keyValueDto { key = "sequencia___" + countItens, value = countItens.ToString() });
                        listaDadosFormulario.Add(new keyValueDto { key = "comportamento___" + countItens, value = comportamento.Comportamento });
                        listaDadosFormulario.Add(new keyValueDto { key = "tipo___" + countItens, value = comportamento.Competência });
                        listaDadosFormulario.Add(new keyValueDto { key = "media_aval_360___" + countItens, value = (Math.Round(Convert.ToDecimal(comportamento.Média_Final), 0)).ToString() });
                        listaDadosFormulario.Add(new keyValueDto { key = "acoes_evolucao___" + countItens, value = comportamento.PLANO_DE_AÇÃO_PARA_MELHORAR_O_COMPORTAMENTO });

                        countItens++;
                    }

                    #endregion

                    ECMWorkflowEngineServiceService client = new ECMWorkflowEngineServiceService();

                    keyValueDto[] dadosFormulario = listaDadosFormulario.ToArray();

                    processAttachmentDto[] attachments = new processAttachmentDto[] { };
                    processTaskAppointmentDto[] apps = new processTaskAppointmentDto[] { };

                    string processo = "PDI";

                    var retorno = client.startProcessClassic("sistemas@hyline.com.br", "123", 1, processo, 0,
                    //var retorno = client.startProcessClassic("fluig", "fluig", 1, processo, 0,
                        new String[] { }, "", "fluig", true, attachments, dadosFormulario,
                        apps, false);

                    if (retorno.Length > 1)
                    {
                        #region Solicitação Gerada, retorna numero para vincular nos pedidos.

                        string numSolicitacaoFluig = retorno[5].value;

                        #endregion
                    }
                    else
                    {
                        #region Solicitação com erro. É gerado um e-mail e enviado para TI e logística.

                        ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                        ImportaCHICService.Data.ApoloServiceEntities apoloService =
                            new ImportaCHICService.Data.ApoloServiceEntities();

                        if (retorno.Length == 1)
                        {
                            string msgErro = retorno[0].value;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                #region Se ocorrer erro na rotina, será gerado um e-mail e enviado para TI.

                ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();
                ImportaCHICService.Data.ApoloServiceEntities apoloService =
                    new ImportaCHICService.Data.ApoloServiceEntities();

                string msgErro = ex.Message;
                if (ex.InnerException != null)
                    msgErro = msgErro + " / Erro interno: " + ex.InnerException.Message;

                #endregion
            }
        }

        #endregion

        #endregion

        #region TARGET - CIOT e Pedágio

        #region Métodos do Sistema TARGET

        public static string CadastrarAtualizarTransportador(string usuario, string senha, string entCod)
        {
            /*
             * Método para inserir ou atualizar um transportador de qualquer tipo, pode acontecer de o transportador já existir 
             * em nossa base. Quando isso ocorrer, o Transportador será associado ao cliente que realizou esse Request 
             * e uma mensagem de sucesso será retornada juntamente com um objeto contendo os dados do transportador para conferência. 
             * Em caso de instruções de inserção e o transportador já exista, será retornado os dados do transportador em um objeto.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/403472476/CadastrarAtualizarTransportador
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region CadastrarAtualizarTransportador

                #region Carrega Dados do Tranportador do Apolo

                FinanceiroEntities apoloSession = new FinanceiroEntities();
                ENTIDADE entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == entCod).FirstOrDefault();
                ENTIDADE1 entidade1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == entCod).FirstOrDefault();
                CIDADE cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                ENT_WEB entWeb = apoloSession.ENT_WEB.Where(w => w.EntCod == entidade.EntCod && w.EntWebEMailPrinc == "Sim").FirstOrDefault();

                #endregion

                TransportadorRequest transp = new TransportadorRequest();
                transp.Instrucao = 1;
                transp.InstrucaoSpecified = true;
                transp.RNTRC = entidade1.USERRNTRC;
                transp.CPFCNPJ = entidade.EntCpfCgc;
                //transp.RNTRC = "12883901";
                //transp.CPFCNPJ = "10890596000104";
                if (entidade.EntTipoFJ == "Física")
                {
                    transp.Nome = entidade.EntNome;
                    transp.Sobrenome = null;
                    transp.RazaoSocial = null;
                    transp.DataNascimento = entidade.EntDesdeData;
                    transp.RG = entidade.EntRgIe;
                    transp.OrgaoEmissorRg = entidade.EntRgOrgExped;
                    transp.CNH = null;
                    transp.TipoCNH = null;
                    transp.DataValidadeCNH = null;
                    transp.Sexo = "S";
                    transp.Naturalidade = null;
                    transp.Nacionalidade = null;
                    transp.InscricaoEstadual = null;
                    transp.InscricaoMunicipal = null;
                    transp.NomeFantasia = null;
                }
                else
                {
                    transp.RazaoSocial = entidade.EntNome;
                    transp.InscricaoEstadual = entidade.EntRgIe;
                    transp.InscricaoMunicipal = entidade.EntInscMunic;
                    transp.NomeFantasia = entidade.EntNomeFant;
                }

                transp.DataInscricao = entidade.EntDesdeData;
                //transp.IdDmAtividadeEconomica = null;
                transp.Endereco = entidade.EntEnder;
                transp.NumeroEndereco = entidade.EntEnderNo;
                transp.EnderecoComplemento = entidade.EntEnderComp;
                transp.Bairro = entidade.EntBair;
                transp.CEP = entidade.EntCep;
                transp.CodigoIBGEMunicipio = Convert.ToInt32(cidade.CidCodMunDipj);
                transp.CodigoIBGEMunicipioSpecified = true;
                transp.IdentificadorEndereco = "NA";
                transp.TelefoneFixo = 1100000000;
                transp.TelefoneFixoSpecified = true;
                transp.TelefoneCelular = 11111111111;
                transp.TelefoneCelularSpecified = true;
                transp.EstadoCivil = 0;
                transp.EstadoCivilSpecified = true;
                //transp.Usuario = entidade.EntCpfCgc;

                if (entWeb != null)
                {
                    transp.Email = entWeb.EntWebEMail;
                    //transp.NomeContato = entWeb.EntWebWWW;
                }

                //transp.NomeContato = entidade.EntNome;
                //transp.CargoContato = "Dono";
                //transp.CPFCNPJContato = entidade.EntCpfCgc;
                //transp.EmailContato = transp.Email;
                //transp.RGContato = entidade.EntRgIe;
                //transp.OrgaoEmissorRgContato = "DETRAN";

                TransportadorResponse transpRet = new TransportadorResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;
                InformacaoServicoResponse testeRet = new InformacaoServicoResponse();
                //testeRet = client.ObterInformacaoServico(auth);

                transpRet = client.CadastrarAtualizarTransportador(auth, transp);

                #endregion

                if (transpRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CadastrarAtualizarTransportador': " + transpRet.Erro.MensagemErro;
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CadastrarAtualizarTransportador': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CadastrarAtualizarMotorista(string usuario, string senha, string cpfCnpjTransportador, string entCod)
        {
            /*
             * Método para inserir ou Atualizar os dados do motorista para o cliente, pode haver acontecer de o motorista já existir 
             * em nossa base. Quando isso ocorrer o Motorista será associado ao cliente que realizou esse Request e uma mensagem 
             * de sucesso será retornada juntamente com um objeto contendo os dados que possuímos em nossa base para caso desejado 
             * atualizar a base local do cliente.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/403308645/CadastrarAtualizarMotorista
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region CadastrarAtualizarMotorista

                #region Carrega Dados do Motorista do Apolo

                FinanceiroEntities apoloSession = new FinanceiroEntities();
                Models.ENTIDADE entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == entCod).FirstOrDefault();
                Models.ENTIDADE1 entidade1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == entCod).FirstOrDefault();
                Models.CIDADE cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                ENT_WEB entWeb = apoloSession.ENT_WEB.Where(w => w.EntCod == entidade.EntCod && w.EntWebEMailPrinc == "Sim").FirstOrDefault();
                Models.ENT_FONE entFoneRes = apoloSession.ENT_FONE
                    .Where(w => w.EntCod == entidade.EntCod && w.EntFoneTipo == "Residencial").FirstOrDefault();
                Models.ENT_FONE entFoneCel = apoloSession.ENT_FONE
                    .Where(w => w.EntCod == entidade.EntCod && w.EntFoneTipo == "Celular").FirstOrDefault();

                #endregion

                MotoristaRequest motorista = new MotoristaRequest();
                motorista.Instrucao = 1;
                motorista.InstrucaoSpecified = true;
                motorista.CPFCNPJTransportador = cpfCnpjTransportador;
                if (entidade1.USERIDTarget != null)
                {
                    motorista.IdMotorista = entidade1.USERIDTarget;
                    motorista.IdMotoristaSpecified = true;
                }
                motorista.Nome = entidade.EntNome;
                motorista.Sobrenome = entidade.EntNomeFant;
                motorista.CPF = entidade.EntCpfCgc;
                motorista.NumeroRG = entidade.EntRgIe;
                motorista.OrgaoEmissorRg = entidade.EntRgOrgExped;
                motorista.DataNascimento = entidade.EntDesdeData;
                motorista.DataNascimentoSpecified = true;
                motorista.Sexo = entidade.EntGenero;

                var estadoCivil = 0;
                if (entidade.EntEstCivil == "Solteiro(a)") estadoCivil = 1;
                else if (entidade.EntEstCivil == "Casado(a)") estadoCivil = 2;
                if (entidade.EntEstCivil == "Viúvo(a)") estadoCivil = 3;
                if (entidade.EntEstCivil == "Outro") estadoCivil = 4;
                if (entidade.EntEstCivil == "Divorciado(a)") estadoCivil = 5;
                if (entidade.EntEstCivil == "Separado(a)" || entidade.EntEstCivil == "Desquitado(a)") estadoCivil = 6;
                motorista.EstadoCivil = estadoCivil.ToString();

                motorista.NomePai = entidade.EntNomePai;
                motorista.NomeMae = entidade.EntNomeMae;
                if (entWeb != null) motorista.Email = entWeb.EntWebEMail;
                if (entFoneRes != null) motorista.Telefone = entFoneRes.EntFoneDDD + entFoneRes.EntFoneNum;
                if (entFoneCel != null) motorista.TelefoneCelular = entFoneCel.EntFoneDDD + entFoneCel.EntFoneNum;
                motorista.Nacionalidade = entidade.EntSegNacionPaisSigla;
                motorista.Endereco = entidade.EntEnder;
                motorista.NumeroEndereco = entidade.EntEnderNo;
                motorista.EnderecoComplemento = entidade.EntEnderComp;
                motorista.Bairro = entidade.EntBair;
                motorista.CEP = entidade.EntCep;
                motorista.CodigoIBGEMunicipio = Convert.ToInt32(cidade.CidCodMunDipj);
                motorista.CodigoIBGEMunicipioSpecified = true;
                motorista.FlagContaPoupanca = false;
                motorista.FlagContaPoupancaSpecified = true;
                motorista.Ativo = true;
                motorista.AtivoSpecified = true;

                MotoristaResponse motoristaRet = new MotoristaResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;
                
                motoristaRet = client.CadastrarAtualizarMotorista(auth, motorista);

                #endregion

                if (motoristaRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CadastrarAtualizarMotorista': " + motoristaRet.Erro.MensagemErro;
                }
                else
                {
                    #region Atualiza o IdMotorista da Target

                    entidade1.USERIDTarget = (short)motoristaRet.IdMotorista;
                    apoloSession.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CadastrarAtualizarMotorista': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CadastrarAtualizarParticipante(string usuario, string senha, string entCod)
        {
            /*
             * Realizar o cadastro de um Participante na base de dados da TARGET.
             * Observações:
             *  - O Participante é o Destinatário da viagem.
             *  - O Cadastro de Participante não é compartilhado com os Clientes TARGET MP, o mesmo é exclusivo para cada Cliente.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/403472480/CadastrarAtualizarParticipante
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region CadastrarAtualizarParticipante

                #region Carrega Dados do Cliente do Apolo

                FinanceiroEntities apoloSession = new FinanceiroEntities();
                Models.ENTIDADE entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == entCod).FirstOrDefault();
                Models.ENTIDADE1 entidade1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == entCod).FirstOrDefault();
                Models.CIDADE cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                ENT_WEB entWeb = apoloSession.ENT_WEB.Where(w => w.EntCod == entidade.EntCod && w.EntWebEMailPrinc == "Sim").FirstOrDefault();
                Models.ENT_FONE entFoneRes = apoloSession.ENT_FONE
                    .Where(w => w.EntCod == entidade.EntCod && w.EntFoneTipo == "Residencial").FirstOrDefault();
                Models.ENT_FONE entFoneCel = apoloSession.ENT_FONE
                    .Where(w => w.EntCod == entidade.EntCod && w.EntFoneTipo == "Celular").FirstOrDefault();

                #endregion

                ParticipanteRequest participante = new ParticipanteRequest();
                participante.Instrucao = 1;
                participante.InstrucaoSpecified = true;
                if (entidade1.USERIDTarget != null)
                {
                    participante.IdParticipante = entidade1.USERIDTarget;
                    participante.IdParticipanteSpecified = true;
                }
                if (entidade.EntTipoFJ == "Física")
                    participante.IdDmTipoPessoa = 1;
                else
                    participante.IdDmTipoPessoa = 2;
                participante.IdDmTipoPessoaSpecified = true;
                participante.Nome = entidade.EntNomeFant;
                participante.RazaoSocial = entidade.EntNome;
                participante.CPFCNPJ = entidade.EntCpfCgc;
                participante.Endereco = entidade.EntEnder + " - " + entidade.EntEnderNo;
                participante.Bairro = entidade.EntBair;
                participante.CEP = entidade.EntCep;
                participante.MunicipioCodigoIBGE = Convert.ToInt32(cidade.CidCodMunDipj);
                participante.MunicipioCodigoIBGESpecified = true;
                participante.Ativo = true;
                participante.AtivoSpecified = true;
                if (entWeb != null) participante.Email = entWeb.EntWebEMail;
                if (entFoneRes != null) participante.Telefone = entFoneRes.EntFoneDDD + entFoneRes.EntFoneNum.Replace("-","").Substring(0, 8);
                if (entFoneCel != null) participante.TelefoneCelular = entFoneCel.EntFoneDDD + entFoneCel.EntFoneNum.Replace("-", "").Substring(0, 9);

                ParticipanteResponse participanteRet = new ParticipanteResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                participanteRet = client.CadastrarAtualizarParticipante(auth, participante);

                #endregion

                if (participanteRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CadastrarAtualizarParticipante': " + participanteRet.Erro.MensagemErro;
                }
                else
                {
                    #region Atualiza o IdParticipante da Target

                    entidade1.USERIDTarget = (short)participanteRet.IdParticipante;
                    apoloSession.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CadastrarAtualizarParticipante': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CadastrarRoteiro(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * O objetivo desse método é realizar o cadastro de novas rotas.
             * Através dele que é realizado o cálculo do Pedágio.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404422983/CadastrarRoteiro
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;
                var veiculo = apoloSession.EQUIPAMENTO.Where(w => w.EquipVeicPlaca.Replace(" ", "") == carga.Placa).FirstOrDefault();
                if (veiculo == null)
                {
                    return "Veículo " + carga.Placa 
                        + " não cadastrado no Apolo! Realize o cadastro para prosseguir com a integração do target!";
                }

                #region Carrega lista Municipios

                int codigoIBGEMunicipioOrigem = 0;
                int codigoIBGEMunicipioDestino = 0;
                var listaCodigosIBGEMunicipioParadas = new List<int>();

                var listaPedidosCarga = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .GroupBy(g => new { g.CodigoCliente })
                    .Select(s => new { s.Key.CodigoCliente, Ordem = s.Min(m => m.Ordem) })
                    .OrderBy(o => o.Ordem)
                    .ToList();

                foreach (var pedido in listaPedidosCarga)
                {
                    var entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == pedido.CodigoCliente).FirstOrDefault();
                    var cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                    var codigo = Convert.ToInt32(cidade.CidCodMunDipj);

                    //if (listaPedidosCarga.IndexOf(pedido) == 0)
                    //    codigoIBGEMunicipioOrigem = codigo;
                    //else 
                    if ((listaPedidosCarga.IndexOf(pedido) + 1) == listaPedidosCarga.Count
                        && listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        codigoIBGEMunicipioDestino = Convert.ToInt32(cidade.CidCodMunDipj);
                    else if (listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        listaCodigosIBGEMunicipioParadas.Add(codigo);
                }

                var listaPedidosCargaDetalhes = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .OrderBy(o => o.Ordem)
                    .ToList();

                #region Carrega Origem pelo Incubatório do primeiro pedido

                var primeiroPedido = listaPedidosCargaDetalhes.OrderBy(o => o.Ordem).FirstOrDefault();
                if (primeiroPedido.LocalNascimento == "CH")
                    codigoIBGEMunicipioOrigem = 3533007;
                else if (primeiroPedido.LocalNascimento == "AJ")
                    codigoIBGEMunicipioOrigem = 3543907;
                else if (primeiroPedido.LocalNascimento == "NM")
                    codigoIBGEMunicipioOrigem = 3170206;

                #endregion

                int[] codigosIBGEMunicipioParadas = listaCodigosIBGEMunicipioParadas.ToArray();
                
                #endregion

                #endregion

                #region CadastrarRoteiro

                RoteiroRequest roteiro = new RoteiroRequest();
                roteiro.NomeRoteiro = nomeRoteiro;
                roteiro.CategoriaVeiculo = Convert.ToInt32(veiculo.EquipVeicCateg);
                roteiro.CategoriaVeiculoSpecified = true;
                roteiro.CodigoIBGEMunicipioOrigem = codigoIBGEMunicipioOrigem;
                roteiro.CodigoIBGEMunicipioOrigemSpecified = true;
                roteiro.CodigosIBGEMunicipioParadas = codigosIBGEMunicipioParadas;
                roteiro.CodigoIBGEMunicipioDestino = codigoIBGEMunicipioDestino;
                roteiro.CodigoIBGEMunicipioDestinoSpecified = true;
                roteiro.RotaOtimizada = true;
                roteiro.RotaOtimizadaSpecified = true;

                RoteiroResponse roteiroRet = new RoteiroResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                roteiroRet = client.CadastrarRoteiro(auth, roteiro);

                #endregion

                if (roteiroRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CadastrarRoteiro': " + roteiroRet.Erro.MensagemErro;
                }
                else
                {
                    #region Atualiza o IdRoteiro da Target

                    carga.IdRoteiroTarget = roteiroRet.IdRoteiro;
                    hlbappSesson.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CadastrarRoteiro': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CadastrarAtualizarOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos, int instrucao)
        {
            /*
             *Principal método da Emissão do CIOT, é nele que você irá passar todas as informações referentes ao CIOT. O usuário irá informar 
             *se é um CIOT Padrão ou de TAC-Agregado entre outras informações.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/402784304/CadastrarAtualizarOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;
                var transportadora = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCod).FirstOrDefault();
                var transportadora1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == carga.EntCod).FirstOrDefault();
                if (transportadora == null)
                {
                    return "Carga " + nomeRoteiro
                        + " sem transportadora relacionada! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }
                
                var veiculo = apoloSession.EQUIPAMENTO.Where(w => w.EquipVeicPlaca.Replace(" ", "") == carga.Placa).FirstOrDefault();
                if (veiculo == null)
                {
                    return "Veículo " + carga.Placa
                        + " não cadastrado no Apolo! Realize o cadastro para ser possível realizar a integração com a TARGET!";
                }

                if (carga.ValorTotal == null)
                {
                    return "Carga " + nomeRoteiro
                        + " sem valor de frete! Realize o cadastro do 'Valor p/ KM' na carga e a 'Qtde. de KM para os Clientes' "
                        + "para ser possível realizar a integração com a TARGET!";
                }
                var valorFrete = Convert.ToDecimal(carga.ValorTotal);

                if (carga.EntCodMotorista01 == null)
                {
                    return "Carga " + nomeRoteiro
                        + " sem Motorista 01 relacionado! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }
                var motorista01 = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCodMotorista01).FirstOrDefault();

                //if (carga.EntCodMotorista02 == null)
                //{
                //    return "Carga " + nomeRoteiro
                //        + " sem Motorista 02 relacionado! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                //}
                //var motorista02 = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCodMotorista02).FirstOrDefault();

                #region Carrega lista Municipios

                int codigoIBGEMunicipioOrigem = 0;
                int codigoIBGEMunicipioDestino = 0;
                var listaCodigosIBGEMunicipioParadas = new List<int>();
                var listaClientes = new List<Models.ENTIDADE>();

                var listaPedidosCarga = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .GroupBy(g => new { g.CodigoCliente })
                    .Select(s => new { s.Key.CodigoCliente, Ordem = s.Min(m => m.Ordem) })
                    .OrderBy(o => o.Ordem)
                    .ToList();

                if (listaPedidosCarga.Count == 0)
                {
                    return "Carga " + nomeRoteiro
                        + " sem pedidos relacionados! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }

                foreach (var pedido in listaPedidosCarga)
                {
                    var entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == pedido.CodigoCliente).FirstOrDefault();
                    var cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                    var codigo = Convert.ToInt32(cidade.CidCodMunDipj);

                    //if (listaPedidosCarga.IndexOf(pedido) == 0)
                    //    codigoIBGEMunicipioOrigem = codigo;
                    //else 
                    if ((listaPedidosCarga.IndexOf(pedido) + 1) == listaPedidosCarga.Count
                        && listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        codigoIBGEMunicipioDestino = Convert.ToInt32(cidade.CidCodMunDipj);
                    else if (listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        listaCodigosIBGEMunicipioParadas.Add(codigo);

                    if (listaClientes.Where(w => w.EntCod == pedido.CodigoCliente).Count() == 0)
                        listaClientes.Add(entidade);
                }

                int[] codigosIBGEMunicipioParadas = listaCodigosIBGEMunicipioParadas.ToArray();

                var listaPedidosCargaDetalhes = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .OrderBy(o => o.Ordem)
                    .ToList();

                var primeiroPedido = listaPedidosCargaDetalhes.OrderBy(o => o.Ordem).FirstOrDefault();
                DateTime dataHoraInicio = Convert.ToDateTime(
                    Convert.ToDateTime(primeiroPedido.DataProgramacao).ToString("yyyy-MM-dd") +
                    " " + primeiroPedido.InicioCarregamentoEsperado);
                var primeiroCliente = apoloSession.ENTIDADE.Where(w => w.EntCod == primeiroPedido.CodigoCliente).FirstOrDefault();

                #region Carrega Origem pelo Incubatório do primeiro pedido

                if (primeiroPedido.LocalNascimento == "CH")
                    codigoIBGEMunicipioOrigem = 3533007;
                else if (primeiroPedido.LocalNascimento == "AJ")
                    codigoIBGEMunicipioOrigem = 3543907;
                else if (primeiroPedido.LocalNascimento == "NM")
                    codigoIBGEMunicipioOrigem = 3170206;

                #endregion

                var ultimoPedido = listaPedidosCargaDetalhes.OrderByDescending(o => o.Ordem).FirstOrDefault();
                DateTime dataHoraTermino = Convert.ToDateTime(
                    Convert.ToDateTime(ultimoPedido.DataEntrega).ToString("yyyy-MM-dd") +
                    " " + ultimoPedido.ChegadaClienteEsperado);

                #endregion

                #region Carrega NCM

                var ncm = "01051110"; // Pintos
                if (listaPedidosCargaDetalhes.Where(w => w.Produto.Contains("H")).Count() > 0)
                    ncm = "04071100"; // Ovos

                #endregion

                var qtdeDeclaracoes = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID
                        && (w.Metodo == "DeclararOperacaoTransporte"))
                    .Count();

                var idIntegrador = carga.ID.ToString();
                if (qtdeDeclaracoes > 0)
                    idIntegrador += "-" + qtdeDeclaracoes.ToString();

                #endregion

                #region Verifica se tem valor de pedágio para selecionar o meio de pagamento correto

                int modoCompraValePedagio = 4;
                decimal valorPedagio = 0;
                var retornoCustoRota = ObterCustoRota(usuario, senha, iDProgDiariaTranspVeiculos);
                if (retornoCustoRota.Contains("Erro"))
                    return retornoCustoRota;
                else if (Decimal.TryParse(retornoCustoRota, out valorPedagio))
                    if (valorPedagio > 0)
                        modoCompraValePedagio = 2;

                #endregion

                #region CadastrarAtualizarOperacaoTransporte

                OperacaoTransporteRequest operacao = new OperacaoTransporteRequest();
                operacao.Instrucao = instrucao;
                operacao.InstrucaoSpecified = true;
                if (carga.IdOperacaoTransporte != null) operacao.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                operacao.IdOperacaoTransporteSpecified = true;
                operacao.NCM = ncm;
                operacao.ProprietarioCarga = 2;
                operacao.ProprietarioCargaSpecified = true;
                operacao.PesoCarga = 2000;
                operacao.PesoCargaSpecified = true;
                operacao.TipoOperacao = 1;
                operacao.TipoOperacaoSpecified = true;
                operacao.MunicipioOrigemCodigoIBGE = codigoIBGEMunicipioOrigem;
                operacao.MunicipioOrigemCodigoIBGESpecified = true;
                operacao.MunicipioDestinoCodigoIBGE = codigoIBGEMunicipioDestino;
                operacao.MunicipioDestinoCodigoIBGESpecified = true;
                operacao.DataHoraInicio = dataHoraInicio.AddDays(10);
                operacao.DataHoraInicioSpecified = true;
                operacao.DataHoraTermino = dataHoraTermino.AddDays(10);
                operacao.DataHoraTerminoSpecified = true;
                operacao.CPFCNPJContratado = transportadora.EntCpfCgc;
                operacao.ValorFrete = valorFrete;
                operacao.ValorFreteSpecified = true;
                operacao.ValorCombustivel = 0;
                operacao.ValorCombustivelSpecified = true;
                //operacao.ValorPedagio = 0;
                operacao.ValorPedagioSpecified = true;
                operacao.ValorDespesas = 0;
                operacao.ValorDespesasSpecified = true;
                operacao.ValorImpostoSestSenat = 0;
                operacao.ValorImpostoSestSenatSpecified = true;
                operacao.ValorImpostoIRRF = 0;
                operacao.ValorImpostoIRRFSpecified = true;
                operacao.ValorImpostoINSS = 0;
                operacao.ValorImpostoINSSSpecified = true;
                operacao.ValorImpostoIcmsIssqn = 0;
                operacao.ValorImpostoIcmsIssqnSpecified = true;
                operacao.ParcelaUnica = true;
                operacao.ParcelaUnicaSpecified = true;
                operacao.ModoCompraValePedagio = modoCompraValePedagio;
                operacao.ModoCompraValePedagioSpecified = true;
                operacao.CategoriaVeiculo = Convert.ToInt32(veiculo.EquipVeicCateg);
                operacao.CategoriaVeiculoSpecified = true;
                operacao.NomeMotorista = motorista01.EntNome;
                operacao.CPFMotorista = motorista01.EntCpfCgc;

                #region Parcelas

                List<OperacaoTransporteParcelaRequest> listaParcelas = new List<OperacaoTransporteParcelaRequest>();
                OperacaoTransporteParcelaRequest parcela = new OperacaoTransporteParcelaRequest();
                parcela.DescricaoParcela = "Adiantamento";
                parcela.Valor = valorFrete;
                parcela.ValorSpecified = true;
                parcela.NumeroParcela = 0;
                parcela.NumeroParcelaSpecified = true;
                parcela.DataVencimento = Convert.ToDateTime(carga.DataProgramacao);
                parcela.DataVencimentoSpecified = true;
                parcela.TipoDaParcela = 1;
                parcela.TipoDaParcelaSpecified = true;
                parcela.FormaPagamento = 1;
                parcela.FormaPagamentoSpecified = true;
                parcela.CodigoBanco = transportadora.BcoNum;
                parcela.AgenciaDeposito = transportadora.AgNum;
                parcela.ContaDeposito = transportadora.EntBcoAgCCorNum.Substring(0, transportadora.EntBcoAgCCorNum.IndexOf("-") -1);
                parcela.DigitoContaDeposito = transportadora.EntBcoAgCCorNum.Substring(transportadora.EntBcoAgCCorNum.IndexOf("-") + 1, 1);
                parcela.ProcessarAutomaticamente = true;
                parcela.ProcessarAutomaticamenteSpecified = true;
                parcela.FlagContaPoupanca = false;
                parcela.FlagContaPoupancaSpecified = false;
                listaParcelas.Add(parcela);
                operacao.Parcelas = listaParcelas.ToArray();

                #endregion

                #region Veículos

                List<OperacaoTransporteVeiculoRequest> listaVeiculos = new List<OperacaoTransporteVeiculoRequest>();
                OperacaoTransporteVeiculoRequest veiculoReq = new OperacaoTransporteVeiculoRequest();
                veiculoReq.Placa = carga.Placa;
                veiculoReq.RNTRC = transportadora1.USERRNTRC;
                listaVeiculos.Add(veiculoReq);
                operacao.Veiculos = listaVeiculos.ToArray();

                #endregion

                operacao.IdRotaModelo = carga.IdRoteiroTarget;
                operacao.IdRotaModeloSpecified = true;
                operacao.DeduzirImpostosSpecified = true;
                operacao.TarifasBancarias = 0;
                operacao.TarifasBancariasSpecified = true;
                operacao.QuantidadeTarifasBancarias = 8;
                operacao.QuantidadeTarifasBancariasSpecified = true;
                operacao.IdIntegrador = idIntegrador;
                operacao.ValorDescontoAntecipado = 0;
                operacao.ValorDescontoAntecipadoSpecified = true;
                operacao.CPFCNPJParticipanteDestinatario = primeiroCliente.EntCpfCgc;

                #region ParticipanteDestinatario (Clientes)

                List<ParticipanteDestinatarioAdicionalRequest> listaParticipantes = new List<ParticipanteDestinatarioAdicionalRequest>();
                foreach (var cliente in listaClientes)
                {
                    if (cliente.EntCod != primeiroCliente.EntCod)
                    {
                        var cliente1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == cliente.EntCod).FirstOrDefault();

                        ParticipanteDestinatarioAdicionalRequest participante = new ParticipanteDestinatarioAdicionalRequest();
                        participante.IdParticipante = cliente1.USERIDTarget;
                        participante.IdParticipanteSpecified = true;
                        participante.CPFCNPJ = cliente.EntCpfCgc;
                        listaParticipantes.Add(participante);
                    }
                }
                operacao.ListaDestinatariosAdicionais = listaParticipantes.ToArray();

                #endregion

                OperacaoTransporteResponse operacaoRet = new OperacaoTransporteResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.CadastrarAtualizarOperacaoTransporte(auth, operacao);

                #endregion

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CadastrarAtualizarOperacaoTransporte': " + operacaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "CadastrarAtualizarOperacaoTransporte";
                    log.DataHoraRegistro = DateTime.Now;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Atualiza o IdOperacaoTransporte da Target

                    carga.IdOperacaoTransporte = operacaoRet.IdOperacaoTransporte;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "CadastrarAtualizarOperacaoTransporte";
                    log.DataHoraRegistro = DateTime.Now;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);

                    #endregion

                    hlbappSesson.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CadastrarAtualizarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string ComprarPedagioAvulso(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             *Realizar compras de Vale Pedágio Cartão ou TAG - 'Sem Parar'.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/407601159/ComprarPedagioAvulso
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;
                var transportadora = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCod).FirstOrDefault();
                var transportadora1 = apoloSession.ENTIDADE1.Where(w => w.EntCod == carga.EntCod).FirstOrDefault();
                if (transportadora == null)
                {
                    return "Carga " + nomeRoteiro
                        + " sem transportadora relacionada! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }

                var veiculo = apoloSession.EQUIPAMENTO.Where(w => w.EquipVeicPlaca.Replace(" ", "") == carga.Placa).FirstOrDefault();
                if (veiculo == null)
                {
                    return "Veículo " + carga.Placa
                        + " não cadastrado no Apolo! Realize o cadastro para ser possível realizar a integração com a TARGET!";
                }

                if (carga.EntCodMotorista01 == null)
                {
                    return "Carga " + nomeRoteiro
                        + " sem Motorista 01 relacionado! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }
                var motorista01 = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCodMotorista01).FirstOrDefault();

                //if (carga.EntCodMotorista02 == null)
                //{
                //    return "Carga " + nomeRoteiro
                //        + " sem Motorista 02 relacionado! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                //}
                //var motorista02 = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCodMotorista02).FirstOrDefault();

                #region Carrega lista Municipios

                int codigoIBGEMunicipioOrigem = 0;
                int codigoIBGEMunicipioDestino = 0;
                var listaCodigosIBGEMunicipioParadas = new List<int>();
                var listaClientes = new List<Models.ENTIDADE>();

                var listaPedidosCarga = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .GroupBy(g => new { g.CodigoCliente })
                    .Select(s => new { s.Key.CodigoCliente, Ordem = s.Min(m => m.Ordem) })
                    .OrderBy(o => o.Ordem)
                    .ToList();

                if (listaPedidosCarga.Count == 0)
                {
                    return "Carga " + nomeRoteiro
                        + " sem pedidos relacionados! Realize o relacionamento para ser possível realizar a integração com a TARGET!";
                }

                foreach (var pedido in listaPedidosCarga)
                {
                    var entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == pedido.CodigoCliente).FirstOrDefault();
                    var cidade = apoloSession.CIDADE.Where(w => w.CidCod == entidade.CidCod).FirstOrDefault();
                    var codigo = Convert.ToInt32(cidade.CidCodMunDipj);

                    if (listaPedidosCarga.IndexOf(pedido) == 0)
                        codigoIBGEMunicipioOrigem = codigo;
                    else if ((listaPedidosCarga.IndexOf(pedido) + 1) == listaPedidosCarga.Count
                        && listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        codigoIBGEMunicipioDestino = Convert.ToInt32(cidade.CidCodMunDipj);
                    else if (listaCodigosIBGEMunicipioParadas.Where(w => w == codigo).Count() == 0)
                        listaCodigosIBGEMunicipioParadas.Add(codigo);

                    if (listaClientes.Where(w => w.EntCod == pedido.CodigoCliente).Count() == 0)
                        listaClientes.Add(entidade);
                }

                int[] codigosIBGEMunicipioParadas = listaCodigosIBGEMunicipioParadas.ToArray();

                var listaPedidosCargaDetalhes = hlbappSesson.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                            && w.NumVeiculo == carga.NumVeiculo)
                    .OrderBy(o => o.Ordem)
                    .ToList();

                var primeiroPedido = listaPedidosCargaDetalhes.OrderBy(o => o.Ordem).FirstOrDefault();
                DateTime dataHoraInicio = Convert.ToDateTime(
                    Convert.ToDateTime(primeiroPedido.DataProgramacao).ToString("yyyy-MM-dd") +
                    " " + primeiroPedido.InicioCarregamentoEsperado);
                var primeiroCliente = apoloSession.ENTIDADE.Where(w => w.EntCod == primeiroPedido.CodigoCliente).FirstOrDefault();

                #region Carrega Origem pelo Incubatório do primeiro pedido

                if (primeiroPedido.LocalNascimento == "CH")
                    codigoIBGEMunicipioOrigem = 3533007;
                else if (primeiroPedido.LocalNascimento == "AJ")
                    codigoIBGEMunicipioOrigem = 3543907;
                else if (primeiroPedido.LocalNascimento == "NM")
                    codigoIBGEMunicipioOrigem = 3170206;

                #endregion

                var ultimoPedido = listaPedidosCargaDetalhes.OrderByDescending(o => o.Ordem).FirstOrDefault();
                DateTime dataHoraTermino = Convert.ToDateTime(
                    Convert.ToDateTime(ultimoPedido.DataEntrega).ToString("yyyy-MM-dd") +
                    " " + ultimoPedido.ChegadaClienteEsperado);

                #endregion

                var qtdeDeclaracoes = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID
                        && (w.Metodo == "ComprarPedagioAvulso"))
                    .Count();

                var idIntegrador = carga.ID.ToString();
                if (qtdeDeclaracoes > 0)
                    idIntegrador += "-" + qtdeDeclaracoes.ToString();

                #endregion

                #region ComprarPedagioAvulso

                CompraValePedagioRequest operacao = new CompraValePedagioRequest();
                operacao.IdModoCompraValePedagio = 2;
                operacao.IdModoCompraValePedagioSpecified = true;
                operacao.IdRotaModelo = carga.IdRoteiroTarget;
                operacao.IdRotaModeloSpecified = true;
                operacao.CodigoCategoriaVeiculo = Convert.ToInt32(veiculo.EquipVeicCateg);
                operacao.CodigoCategoriaVeiculoSpecified = true;
                operacao.MunicipioOrigemCodigoIBGE = codigoIBGEMunicipioOrigem;
                operacao.MunicipioOrigemCodigoIBGESpecified = true;
                operacao.MunicipioDestinoCodigoIBGE = codigoIBGEMunicipioDestino;
                operacao.MunicipioDestinoCodigoIBGESpecified = true;
                operacao.Placa = carga.Placa;
                operacao.MotoristaNome = motorista01.EntNome;
                operacao.MotoristaCPF = motorista01.EntCpfCgc;
                operacao.MotoristaRNTRC = transportadora1.USERRNTRC;
                operacao.IdIntegrador = idIntegrador;
                operacao.InicioVigencia = dataHoraInicio.AddDays(10);
                operacao.InicioVigenciaSpecified = true;
                operacao.FimVigencia = dataHoraTermino.AddDays(10);
                operacao.FimVigenciaSpecified = true;
                operacao.CompraSimplesSpecified = true;

                #endregion

                CompraValePedagioResponse operacaoRet = new CompraValePedagioResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.ComprarPedagioAvulso(auth, operacao);

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'ComprarPedagioAvulso': " + operacaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "ComprarPedagioAvulso";
                    log.DataHoraRegistro = DateTime.Now;
                    log.IdCompraValePedagio = operacaoRet.IdCompraValePedagio;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Atualiza o IdCompraValePedagio da Target

                    carga.IdOperacaoTransporte = operacaoRet.IdCompraValePedagio;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "ComprarPedagioAvulso";
                    log.DataHoraRegistro = DateTime.Now;
                    log.IdCompraValePedagio = operacaoRet.IdCompraValePedagio;
                    log.Observacoes = "Valor da Compra: " + operacaoRet.ValorCompra.ToString("0.,00") +
                        " - " + operacaoRet.Mensagem;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);

                    #endregion

                    hlbappSesson.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'ComprarPedagioAvulso': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string ObterCustoRota(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             *Esse método tem como objetivo obter os valores de uma rota.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/407601207/ObterCustoRota
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var veiculo = apoloSession.EQUIPAMENTO.Where(w => w.EquipVeicPlaca.Replace(" ", "") == carga.Placa).FirstOrDefault();
                if (veiculo == null)
                {
                    return "Veículo " + carga.Placa
                        + " não cadastrado no Apolo! Realize o cadastro para ser possível realizar a integração com a TARGET!";
                }

                #endregion

                #region ObterCustoRota

                ObtencaoCustoRotaRequest operacao = new ObtencaoCustoRotaRequest();
                operacao.CategoriaVeiculo = Convert.ToInt32(veiculo.EquipVeicCateg);
                operacao.CategoriaVeiculoSpecified = true;
                operacao.IdRotaModelo = Convert.ToInt32(carga.IdRoteiroTarget);
                operacao.IdRotaModeloSpecified = true;
                operacao.ModoPagamentoRota = 2;
                operacao.ModoPagamentoRotaSpecified = true;

                #endregion

                ObtencaoCustoRotaResponse operacaoRet = new ObtencaoCustoRotaResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.ObterCustoRota(auth, operacao);

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'ObterCustoRota': " + operacaoRet.Erro.MensagemErro;
                }
                else
                {
                    retorno = operacaoRet.ValorPedagioTotal.ToString();
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'ObterCustoRota': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string BuscarOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             *Esse método retorna uma Operação de Transporte já cadastrada no Sistema TARGET Frete.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404389930/BuscarOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();

                #endregion

                #region BuscarOperacaoTransporte

                BuscaOperacaoTransporteRequest operacao = new BuscaOperacaoTransporteRequest();
                operacao.QuantidadeItensPorPagina = 10;
                operacao.QuantidadeItensPorPaginaSpecified = true;
                operacao.NumeroPagina = 1;
                operacao.NumeroPaginaSpecified = true;
                operacao.IdOperacao = carga.IdOperacaoTransporte;
                operacao.IdOperacaoSpecified = true;
                operacao.StatusOperacaoSpecified = true;

                #endregion

                ResultadoPaginadoOperacaoTransporteResponse operacaoRet = new ResultadoPaginadoOperacaoTransporteResponse();

                if (carga.IdOperacaoTransporte != null)
                {
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;
                    operacaoRet = client.BuscarOperacaoTransporte(auth, operacao);
                }
                else
                    return "";

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'BuscarOperacaoTransporte': " + operacaoRet.Erro.MensagemErro;
                }
                else
                {
                    if (operacaoRet.Itens.Count() > 0)
                        retorno = operacaoRet.Itens.FirstOrDefault().StatusOperacao;
                    else
                        retorno = "Erro no retorno do método 'BuscarOperacaoTransporte': Item não encontrado!";
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'BuscarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static ResultadoPaginadoOperacaoTransporteResponse BuscarOperacaoTransporteObj(string usuario, string senha, 
            int iDProgDiariaTranspVeiculos)
        {
            /*
             *Esse método retorna uma Operação de Transporte já cadastrada no Sistema TARGET Frete.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404389930/BuscarOperacaoTransporte
             */

            string retorno = "";
            ResultadoPaginadoOperacaoTransporteResponse operacaoRet = new ResultadoPaginadoOperacaoTransporteResponse();

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();

                #endregion

                #region BuscarOperacaoTransporte

                BuscaOperacaoTransporteRequest operacao = new BuscaOperacaoTransporteRequest();
                operacao.QuantidadeItensPorPagina = 10;
                operacao.QuantidadeItensPorPaginaSpecified = true;
                operacao.NumeroPagina = 1;
                operacao.NumeroPaginaSpecified = true;
                operacao.IdOperacao = carga.IdOperacaoTransporte;
                operacao.IdOperacaoSpecified = true;
                operacao.StatusOperacaoSpecified = true;

                #endregion

                if (carga.IdOperacaoTransporte != null)
                {
                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;
                    operacaoRet = client.BuscarOperacaoTransporte(auth, operacao);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'BuscarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();

                operacaoRet.Erro = new ErroResponse();
                operacaoRet.Erro.MensagemErro = retorno;
            }

            return operacaoRet;
        }

        public static string DeclararOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * Esse método tem como objetivo realizar a declaração de uma Operação de Transporte na ANTT.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/409436271/DeclararOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();

                #endregion

                #region DeclararOperacaoTransporte

                DeclaracaoOperacaoTransporteRequest declaracao = new DeclaracaoOperacaoTransporteRequest();
                declaracao.IdOperacaoTransporte = Convert.ToInt32(carga.IdOperacaoTransporte);
                declaracao.IdOperacaoTransporteSpecified = true;

                DeclaracaoOperacaoTransporteResponse declaracaoRet = new DeclaracaoOperacaoTransporteResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                declaracaoRet = client.DeclararOperacaoTransporte(auth, declaracao);

                #endregion

                if (declaracaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'DeclararOperacaoTransporte': " + declaracaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "DeclararOperacaoTransporte";
                    log.DataHoraRegistro = declaracaoRet.DataHoraRegistro;
                    log.NumeroCIOT = declaracaoRet.NumeroCIOT;
                    log.ProtocoloCIOT = declaracaoRet.ProtocoloCIOT;
                    log.DispensadoPelaANTT = (declaracaoRet.DispensadoPelaANTT == true ? 1 : 0);
                    log.ObservacoesANTT = declaracaoRet.ObservacoesANTT;
                    log.IdCompraValePedagio = declaracaoRet.IdCompraValePedagio;
                    log.ModoCompraValePedagio = declaracaoRet.ModoCompraValePedagio;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = declaracaoRet.IdOperacaoTransporte;
                    log.Metodo = "DeclararOperacaoTransporte";
                    log.DataHoraRegistro = declaracaoRet.DataHoraRegistro;
                    log.NumeroCIOT = declaracaoRet.NumeroCIOT;
                    log.ProtocoloCIOT = declaracaoRet.ProtocoloCIOT;
                    log.DispensadoPelaANTT = (declaracaoRet.DispensadoPelaANTT == true ? 1 : 0);
                    log.ObservacoesANTT = declaracaoRet.ObservacoesANTT;
                    log.IdCompraValePedagio = declaracaoRet.IdCompraValePedagio;
                    log.ModoCompraValePedagio = declaracaoRet.ModoCompraValePedagio;
                    log.IdParcelasOperacaoTransporte = declaracaoRet.IdsParcelasOperacaoTransporte.FirstOrDefault();
                    log.Observacoes = "";

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'DeclararOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CancelarOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos, string motivo)
        {
            /*
             * Método para cancelar uma Operação de Transporte. Uma Operação de Transporte só pode ser cancelada quando a mesma 
             * estiver com o status: Declarada.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404193284/CancelarOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;

                #endregion

                #region CancelarOperacaoTransporte

                CancelamentoOperacaoRequest operacao = new CancelamentoOperacaoRequest();
                operacao.IdOperacao = Convert.ToInt32(carga.IdOperacaoTransporte);
                operacao.IdOperacaoSpecified = true;
                operacao.MotivoCancelamento = motivo;

                #endregion

                CancelamentoOperacaoResponse operacaoRet = new CancelamentoOperacaoResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.CancelarOperacaoTransporte(auth, operacao);

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'CancelarOperacaoTransporte': " + operacaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "CancelarOperacaoTransporte";
                    log.DataHoraRegistro = DateTime.Now;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "CancelarOperacaoTransporte";
                    log.DataHoraRegistro = operacaoRet.DataCancelamento;
                    log.IdCompraValePedagio = operacaoRet.IdCancelamentoOperacaoTransporte;
                    log.ProtocoloCIOT = operacaoRet.ProtocoloCancelamento;
                    log.Observacoes = retorno;

                    carga.IdOperacaoTransporte = null;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion

                    #region Envia E-mail

                    var corpoEmail =
                        "Carga: " + nomeRoteiro + "<br />"
                        + "IdOperacaoTransporte: " + carga.IdOperacaoTransporte.ToString() + "<br />"
                        + "IdCancelamentoOperacaoTransporte: " + operacaoRet.IdCancelamentoOperacaoTransporte.ToString() + "<br />"
                        + "ProtocoloCancelamento: " + operacaoRet.ProtocoloCancelamento.ToString() + "<br />";

                    EnviarEmail("Paulo Alves", "palves@hyline.com.br", "", "CIOT CANCELADO - " + nomeRoteiro,
                        corpoEmail, iDProgDiariaTranspVeiculos, "");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CancelarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string FinalizarOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * Esse método tem como objetivo, finalizar uma Operação de Transporte na TARGET. 
             * Esse método deve ser invocado após o método - EncerrarOperacaoTransporte.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/409469058/FinalizarOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;

                #endregion

                #region FinalizarOperacaoTransporte

                FinalizacaoOperacaoTransporteRequest operacao = new FinalizacaoOperacaoTransporteRequest();
                operacao.IdOperacaoTransporte = Convert.ToInt32(carga.IdOperacaoTransporte);
                operacao.IdOperacaoTransporteSpecified = true;

                #endregion

                FinalizacaoOperacaoTransporteResponse operacaoRet = new FinalizacaoOperacaoTransporteResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.FinalizarOperacaoTransporte(auth, operacao);

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'FinalizarOperacaoTransporte': " + operacaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "FinalizarOperacaoTransporte";
                    log.DataHoraRegistro = DateTime.Now;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "FinalizarOperacaoTransporte";
                    log.DataHoraRegistro = operacaoRet.DataHoraFinalizacao;
                    log.Observacoes = operacaoRet.MensagemRetorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion

                    #region Envia E-mail

                    var corpoEmail =
                        "Carga: " + nomeRoteiro + "<br />"
                        + "IdOperacaoTransporte: " + carga.IdOperacaoTransporte.ToString() + "<br />"
                        + operacaoRet.MensagemRetorno;

                    EnviarEmail("Paulo Alves", "palves@hyline.com.br", "", "CIOT FINALIZADO - " + nomeRoteiro,
                        corpoEmail, iDProgDiariaTranspVeiculos, "");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'FinalizarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string ConfirmarPedagioTAG(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * Esse método tem com objetivo carregar o valor comprado de Vale Pedágio na TAG do caminhão.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404127784/ConfirmarPedagioTAG
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var ultimaDeclaracao = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID 
                        && (w.Metodo == "DeclararOperacaoTransporte" || w.Metodo == "ComprarPedagioAvulso"))
                    .OrderByDescending(o => o.DataHoraRegistro)
                    .FirstOrDefault();

                #endregion

                if (ultimaDeclaracao.IdCompraValePedagio != null)
                {
                    #region ConfirmarPedagioTAG

                    ConfirmacaoPedagioRequest confirmacao = new ConfirmacaoPedagioRequest();
                    confirmacao.IdCompraValePedagioViaFacil = Convert.ToInt32(ultimaDeclaracao.IdCompraValePedagio);
                    confirmacao.IdCompraValePedagioViaFacilSpecified = true;

                    ConfirmarPedagioResponse confirmacaoRet = new ConfirmarPedagioResponse();

                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                    confirmacaoRet = client.ConfirmarPedagioTAG(auth, confirmacao);

                    #endregion

                    if (confirmacaoRet.Erro != null)
                    {
                        retorno = "Erro no retorno do método 'ConfirmarPedagioTAG': " + confirmacaoRet.Erro.MensagemErro;

                        #region Insere LOG de Registro do Método

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                        log.Metodo = "ConfirmarPedagioTAG";
                        log.DataHoraRegistro = DateTime.Now;
                        log.IdCompraValePedagio = ultimaDeclaracao.IdCompraValePedagio;
                        log.Observacoes = retorno;

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region Insere LOG de Registro do Método

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                        log.Metodo = "ConfirmarPedagioTAG";
                        log.DataHoraRegistro = DateTime.Now;
                        log.IdCompraValePedagio = ultimaDeclaracao.IdCompraValePedagio;
                        log.Observacoes = confirmacaoRet.Mensagem;

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'ConfirmarPedagioTAG': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string CancelarCompraValePedagio(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * Esse método realiza o cancelamento das Compras de Vale Pedágio Cartão e TAG.

             * Vale Pedágio - TAG

             * Compra do Vale Pedágio Avulso TAG realizada, porém a confirmação de carregamento da TAG não foi feita através 
             * do método - ConfirmarPedagioTAG.
             * Resposta: Não haverá validações para esse cenário, o usuário conseguirá cancelar o mesmo com sucesso. 
             * O dinheiro da compra se manterá na conta do cliente.
             * 
             * Compra do Vale Pedágio Avulso TAG realizada, porém a confirmação de carregamento da TAG foi realizada através 
             * do método - ConfirmarPedagioTAG.
             * Resposta: O cancelamento da compra só poderá ser realizado se o mesmo for feito até no máximo 3h após da compra, 
             * caso o motorista passe por uma praça de pedágio no período das 3h, o usuário não conseguirá realizar o cancelamento da compra.
             * Resposta: Se o cancelamento for realizado em até 3h, o estorno será feito para a conta do cliente.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/404094993/CancelarCompraValePedagio
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var ultimaDeclaracao = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID && w.Metodo == "ComprarPedagioAvulso")
                    .OrderByDescending(o => o.DataHoraRegistro)
                    .FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;

                #endregion

                if (ultimaDeclaracao.IdCompraValePedagio != null)
                {
                    #region CancelarCompraValePedagio

                    CancelaCompraValePedagioRequest confirmacao = new CancelaCompraValePedagioRequest();
                    confirmacao.IdCompraValePedagio = Convert.ToInt32(ultimaDeclaracao.IdCompraValePedagio);
                    confirmacao.IdCompraValePedagioSpecified = true;
                    confirmacao.ViaFacil = true;
                    confirmacao.ViaFacilSpecified = true;

                    CancelaCompraValePedagioResponse confirmacaoRet = new CancelaCompraValePedagioResponse();

                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                    confirmacaoRet = client.CancelarCompraValePedagio(auth, confirmacao);

                    #endregion

                    if (confirmacaoRet.Erro != null)
                    {
                        retorno = "Erro no retorno do método 'CancelarCompraValePedagio': " + confirmacaoRet.Erro.MensagemErro;

                        #region Insere LOG de Registro do Método

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                        log.Metodo = "CancelarCompraValePedagio";
                        log.DataHoraRegistro = DateTime.Now;
                        log.IdCompraValePedagio = ultimaDeclaracao.IdCompraValePedagio;
                        log.Observacoes = retorno;

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region Insere LOG de Registro do Método

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                        log.Metodo = "CancelarCompraValePedagio";
                        log.DataHoraRegistro = DateTime.Now;
                        log.IdCompraValePedagio = ultimaDeclaracao.IdCompraValePedagio;
                        log.Observacoes = confirmacaoRet.Mensagem;

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion

                        #region Envia E-mail

                        var corpoEmail =
                            "Carga: " + nomeRoteiro + "<br />"
                            + "IdCompraValePedagio: " + ultimaDeclaracao.IdCompraValePedagio.ToString() + "<br />"
                            + confirmacaoRet.Mensagem;

                        EnviarEmail("Paulo Alves", "palves@hyline.com.br", "", "PEDÁGIO CANCELADO - " + nomeRoteiro,
                            corpoEmail, iDProgDiariaTranspVeiculos, "");

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'CancelarCompraValePedagio': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string BuscarCompraValePedagio(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             * Esse método tem como objetivo Buscar uma ou mais compras de Vale Pedágio.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/407732300/BuscarCompraValePedagio
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var ultimaDeclaracao = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID && w.Metodo == "ComprarPedagioAvulso")
                    .OrderByDescending(o => o.DataHoraRegistro)
                    .FirstOrDefault();

                #endregion

                if (ultimaDeclaracao != null)
                {
                    #region BuscarCompraValePedagio

                    BuscaCompraValePedagioRequest operacao = new BuscaCompraValePedagioRequest();
                    operacao.QuantidadeItensPorPagina = 10;
                    operacao.QuantidadeItensPorPaginaSpecified = true;
                    operacao.NumeroPagina = 1;
                    operacao.NumeroPaginaSpecified = true;
                    operacao.IdModoCompraValePedagio = 2;
                    operacao.IdModoCompraValePedagioSpecified = true;
                    operacao.IdStatusValePedagioSpecified = true;
                    operacao.DataInicioPeriodoSpecified = true;
                    operacao.DataFimPeriodoSpecified = true;
                    operacao.IdCompraValePedagio = Convert.ToInt32(ultimaDeclaracao.IdCompraValePedagio);
                    operacao.IdCompraValePedagioSpecified = true;
                    operacao.TipoBuscaUnitariaSpecified = true;

                    #endregion

                    ResultadoPaginadoBuscaCompraValePedagioResponse operacaoRet = new ResultadoPaginadoBuscaCompraValePedagioResponse();

                    if (ultimaDeclaracao.IdCompraValePedagio != null && carga.IdOperacaoTransporte == null)
                    {
                        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                        System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;
                        operacaoRet = client.BuscarCompraValePedagio(auth, operacao);
                    }
                    else
                        return "";                    

                    if (operacaoRet.Erro != null)
                    {
                        retorno = "Erro no retorno do método 'BuscarCompraValePedagio': " + operacaoRet.Erro.MensagemErro;
                    }
                    else
                    {
                        if (operacaoRet.Itens.Count() > 0)
                            retorno = operacaoRet.Itens.FirstOrDefault().IdStatusCompraValePedagio.ToString();
                        else
                            retorno = "Erro no retorno do método 'BuscarCompraValePedagio': Item não encontrado!";
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'BuscarCompraValePedagio': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public enum TipoOperacao
        {
            Vazio01,
            DeclaracaoOperacaoTransporte,
            Vazio02,
            ReciboPedagioCartao,
            ReciboPedagioTAG,
            ReciboPagamentoAvulso,
            ReciboCombustivelAvulso,
            ReciboPagamentoParcela
        };

        public static string EmitirDocumento(string usuario, string senha, int iDProgDiariaTranspVeiculos, TipoOperacao tipoOperacao)
        {
            /*
             * Esse método tem como objetivo emitir os documentos das Operações realizadas no Sistema TARGET Frete.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/409403531/EmitirDocumento
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var ultimaDeclaracao = hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET
                    .Where(w => w.IDProgDiariaTranspVeiculos == carga.ID
                        && (w.Metodo == "DeclararOperacaoTransporte" || w.Metodo == "ComprarPedagioAvulso"))
                    .OrderByDescending(o => o.DataHoraRegistro)
                    .FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;

                #region Carrega E-mails da Transportadora, do Escritório Contábil e do Depto. de Logística para envio

                var transpApolo = apoloSession.ENTIDADE.Where(w => w.EntCod == carga.EntCod).FirstOrDefault();
                var listaEmailsTransportadora = apoloSession.ENT_WEB.Where(w => w.EntCod == carga.EntCod).ToList();

                var emailTransportadora = listaEmailsTransportadora.Where(w => w.EntWebTipo == "Comercial").FirstOrDefault().EntWebEMail;
                var emailEscritorioContabil = listaEmailsTransportadora.Where(w => w.EntWebTipo == "Financeiro").FirstOrDefault().EntWebEMail;
                var deptoLogistica = "logistica@hyline.com.br";

                #endregion

                #endregion

                if (
                    (ultimaDeclaracao.IdCompraValePedagio != null && tipoOperacao == TipoOperacao.ReciboPedagioTAG)
                    ||
                    (tipoOperacao != TipoOperacao.ReciboPedagioTAG)
                   )
                {
                    int idEntidade = 0;
                    var descricao = "";
                    if (tipoOperacao == TipoOperacao.DeclaracaoOperacaoTransporte)
                    {
                        idEntidade = Convert.ToInt32(carga.IdOperacaoTransporte);
                        descricao = "CIOT";
                    }
                    else if (tipoOperacao == TipoOperacao.ReciboPedagioCartao)
                    {
                        idEntidade = Convert.ToInt32(ultimaDeclaracao.IdCompraValePedagio);
                        descricao = "PEDÁGIO";
                    }
                    else if (tipoOperacao == TipoOperacao.ReciboPedagioTAG)
                    {
                        idEntidade = Convert.ToInt32(ultimaDeclaracao.IdCompraValePedagio);
                        descricao = "PEDÁGIO";
                    }
                    else if (tipoOperacao == TipoOperacao.ReciboPagamentoParcela)
                    {
                        idEntidade = Convert.ToInt32(ultimaDeclaracao.IdParcelasOperacaoTransporte);
                        descricao = "PAG.";
                    }

                    #region EmitirDocumento

                    EmissaoDocumentoRequest documento = new EmissaoDocumentoRequest();
                    documento.Tipo = Convert.ToInt32(tipoOperacao);
                    documento.TipoSpecified = true;
                    documento.IdEntidade = idEntidade;
                    documento.IdEntidadeSpecified = true;

                    EmissaoDocumentoResponse documentoRet = new EmissaoDocumentoResponse();

                    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                    documentoRet = client.EmitirDocumento(auth, documento);

                    #endregion

                    if (documentoRet.Erro != null)
                    {
                        retorno = "Erro no retorno do método 'EmitirDocumento' " + tipoOperacao.ToString() + ": " + documentoRet.Erro.MensagemErro;

                        #region Insere LOG de Registro do Método

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = idEntidade;
                        log.Metodo = "EmitirDocumento";
                        log.DataHoraRegistro = DateTime.Now;
                        log.Observacoes = retorno;

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        #region Insere LOG de Registro do Método

                        string pdfFilePath = @"\\srv-riosoft-01\w\TARGET\" + tipoOperacao + "_" + carga.ID + ".pdf";
                        System.IO.File.WriteAllBytes(pdfFilePath, documentoRet.DocumentoBinario);

                        LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                        log.IDProgDiariaTranspVeiculos = carga.ID;
                        log.IdOperacaoTransporte = idEntidade;
                        log.Metodo = "EmitirDocumento";
                        log.DataHoraRegistro = DateTime.Now;
                        log.IdCompraValePedagio = ultimaDeclaracao.IdCompraValePedagio;
                        log.Observacoes = pdfFilePath;                        

                        hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                        hlbappSesson.SaveChanges();

                        #endregion

                        #region Envia E-mail

                        var corpoEmail = 
                            "Carga: " + nomeRoteiro + "<br />"
                            + "Documento " + tipoOperacao.ToString() + " emitido com sucesso! <br />"
                            + "Segue documento em anexo.<br />";

                        EnviarEmail(transpApolo.EntNomeFant, emailTransportadora, emailEscritorioContabil + ";" + deptoLogistica, descricao + " - " + nomeRoteiro,
                            corpoEmail, iDProgDiariaTranspVeiculos, pdfFilePath);

                        #endregion
                    }
                }
                else
                {
                    if(tipoOperacao == TipoOperacao.ReciboPedagioTAG)
                    {
                        #region Envia E-mail

                        var corpoEmail =
                            "Carga: " + nomeRoteiro + "<br />"
                            + "Não foi gerado pedágio porque não existe valor para o mesma nessa carga! <br />";

                        EnviarEmail(transpApolo.EntNomeFant, emailTransportadora, emailEscritorioContabil + ";" + deptoLogistica, "SEM PEDÁGIO - " + nomeRoteiro,
                            corpoEmail, iDProgDiariaTranspVeiculos, "");

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'EmitirDocumento': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        public static string EncerrarOperacaoTransporte(string usuario, string senha, int iDProgDiariaTranspVeiculos)
        {
            /*
             *Esse método tem como objetivo realizar o encerramento da Operação de Transporte na ANTT.
             * 
             * https://targetmp.atlassian.net/wiki/spaces/DOC/pages/409272449/EncerrarOperacaoTransporte
             */

            string retorno = "";

            try
            {
                #region Cria variáveis / objetos gerais

                BasicHttpBinding_FreteTMSService client = new BasicHttpBinding_FreteTMSService();

                AutenticacaoRequest auth = new AutenticacaoRequest();
                auth.Usuario = usuario;
                auth.Senha = senha;
                //auth.Token = null;

                #endregion

                #region Carrega Dados da Carga

                HLBAPPEntities1 hlbappSesson = new HLBAPPEntities1();
                FinanceiroEntities apoloSession = new FinanceiroEntities();
                var carga = hlbappSesson.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeiculos).FirstOrDefault();
                var nomeRoteiro = "ID " + carga.ID.ToString() + " - Carga Nº " + carga.NumVeiculo.ToString() + " - Nascimento: "
                    + Convert.ToDateTime(carga.DataProgramacao).ToShortDateString() + " - Resp.: "
                    + carga.EmpresaTranportador;

                #endregion

                #region EncerrarOperacaoTransporte

                EncerramentoOperacaoTransporteRequest operacao = new EncerramentoOperacaoTransporteRequest();
                operacao.CodigoOperacao = Convert.ToInt32(carga.IdOperacaoTransporte);
                operacao.CodigoOperacaoSpecified = true;

                List<OperacaoTransporteViagemRequest> listaViagens = new List<OperacaoTransporteViagemRequest>();
                operacao.Viagens = listaViagens.ToArray();

                #endregion

                EncerramentoOperacaoTransporteResponse operacaoRet = new EncerramentoOperacaoTransporteResponse();

                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                // Abaixo serve para mudar o protocolo de segurança para TLS 1.2. Como não tem o ENum no .NET 4.0, tem que colocar igual abaixo.
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)0x00000C00;

                operacaoRet = client.EncerrarOperacaoTransporte(auth, operacao);

                if (operacaoRet.Erro != null)
                {
                    retorno = "Erro no retorno do método 'EncerrarOperacaoTransporte': " + operacaoRet.Erro.MensagemErro;

                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "EncerrarOperacaoTransporte";
                    log.IdCompraValePedagio = operacaoRet.IdEncerramentoOperacaoTransporte;
                    log.DataHoraRegistro = DateTime.Now;
                    log.Observacoes = retorno;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion
                }
                else
                {
                    #region Insere LOG de Registro do Método

                    LOG_Prog_Diaria_Transp_Veiculos_TARGET log = new LOG_Prog_Diaria_Transp_Veiculos_TARGET();
                    log.IDProgDiariaTranspVeiculos = carga.ID;
                    log.IdOperacaoTransporte = carga.IdOperacaoTransporte;
                    log.Metodo = "EncerrarOperacaoTransporte";
                    log.IdCompraValePedagio = operacaoRet.IdEncerramentoOperacaoTransporte;
                    log.DataHoraRegistro = operacaoRet.DataEncerramento;
                    log.Observacoes = "Protocolo Encerramento: " + operacaoRet.ProtocoloEncerramento
                        + " - Tipo Operação: " + operacaoRet.TipoOperacao;

                    hlbappSesson.LOG_Prog_Diaria_Transp_Veiculos_TARGET.AddObject(log);
                    hlbappSesson.SaveChanges();

                    #endregion

                    #region Envia E-mail

                    var corpoEmail =
                        "Carga: " + nomeRoteiro + "<br />"
                        + "IdOperacaoTransporte: " + carga.IdOperacaoTransporte.ToString() + "<br />"
                        + "IdEncerramentoOperacaoTransporte: " + operacaoRet.IdEncerramentoOperacaoTransporte.ToString() + "<br />"
                        + "Protocolo Encerramento: " + operacaoRet.ProtocoloEncerramento.ToString() + "<br />"
                        + "Data Encerramento: " + operacaoRet.DataEncerramento.ToString("dd/MM/yyyy HH:mm") + "<br />"
                        + "Tipo Operação: " + operacaoRet.TipoOperacao + "<br />";

                    EnviarEmail("Paulo Alves", "palves@hyline.com.br", "", "CIOT ENCERRADO - " + nomeRoteiro,
                        corpoEmail, iDProgDiariaTranspVeiculos, "");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                retorno = "Erro ao chamar método 'EncerrarOperacaoTransporte': " + ex.Message;
                if (ex.InnerException != null) retorno = retorno + " / Erro interno: " + ex.InnerException.Message;
                retorno = retorno + " / Linha do Erro: " + linenum.ToString();
            }

            return retorno;
        }

        #endregion

        public static string IntegraTargetCIOTPadraoEValePedagioSemParar(int iDProgDiariaTranspVeic)
        {
            // Documentação - Cenário 11
            // https://targetmp.atlassian.net/wiki/spaces/DOC/pages/402227318/11-+WS+2.0+Frete+Cen+rio+11

            string retorno = "";

            // SERVIDOR HOMOLOGAÇÃO
            var usuario = "tms.grupohy";
            var senha = "123@Mudar";

            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
            FinanceiroEntities apoloSession = new FinanceiroEntities();
            var carga = hlbappSession.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeic).FirstOrDefault();

            //var entCodTransportador = "";
            //if (carga.EmpresaTranportador == "PL" || carga.EmpresaTranportador == "HN")
            //    entCodTransportador = carga.EntCod;
            //else
            //    entCodTransportador = "0000807";

            //var entCodTransportador = "0025548"; // Teste

            var entCodTransportador = "";
            if (carga.EntCod == null)
                return "Código da Transportadora não relacionado na carga! Verifique para poder gerar a Integração com a TARGET!";
            else
                entCodTransportador = carga.EntCod;

            var entidadeTransportador = apoloSession.ENTIDADE.Where(w => w.EntCod == entCodTransportador).FirstOrDefault();

            // CADASTRA / ATUALIZA TRANSPORTADOR
            retorno = CadastrarAtualizarTransportador(usuario, senha, entCodTransportador);
            if (retorno != "") return retorno;

            // CADASTRA / ATUALIZA MOTORISTA 01
            retorno = CadastrarAtualizarMotorista(usuario, senha, entidadeTransportador.EntCpfCgc, carga.EntCodMotorista01);
            if (retorno != "") return retorno;

            #region CADASTRA / ATUALIZA PARTICIPANTES

            var listaClientes = new List<Models.ENTIDADE>();

            var listaPedidosCarga = hlbappSession.Prog_Diaria_Transp_Pedidos
                .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                        && w.NumVeiculo == carga.NumVeiculo)
                .GroupBy(g => new { g.CodigoCliente })
                .Select(s => new { s.Key.CodigoCliente, Ordem = s.Min(m => m.Ordem) })
                .OrderBy(o => o.Ordem)
                .ToList();

            foreach (var pedido in listaPedidosCarga)
            {
                var entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == pedido.CodigoCliente).FirstOrDefault();
                if (listaClientes.Where(w => w.EntCod == pedido.CodigoCliente).Count() == 0)
                    listaClientes.Add(entidade);
            }

            foreach (var cliente in listaClientes)
            {
                retorno = CadastrarAtualizarParticipante(usuario, senha, cliente.EntCod);
                if (retorno != "") return retorno;
            }

            #endregion

            // CADASTRA ROTA / ROTEIRO
            retorno = CadastrarRoteiro(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            #region CADASTRA / ATUALIZA / ANULA OPERAÇÃO DE TRANSPORTE

            var retornoStatusOperacao = BuscarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic);
            if (retornoStatusOperacao.Contains("Erro"))
                return retornoStatusOperacao;
            else if (retornoStatusOperacao.Contains("Declarada"))
            {
                // CANCELA OPERAÇÃO DE TRANSPORTE
                retorno = CancelarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic, "Teste");
                if (retorno != "") return retorno;
            }
            else if (!retornoStatusOperacao.Contains("Cancelad") && retornoStatusOperacao != "")
            {
                // ANULA OPERAÇÃO DE TRANSPORTE
                retorno = CadastrarAtualizarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic, 4);
                if (retorno != "") return retorno;
            }

            // CADASTRA / ATUALIZA OPERAÇÃO DE TRANSPORTE
            retorno = CadastrarAtualizarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic, 1);
            if (retorno != "") return retorno;

            #endregion

            // DECLARA OPERAÇÃO DE TRANSPORTE
            retorno = DeclararOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            // CONFIRMA PEDAGIO TAG
            retorno = ConfirmarPedagioTAG(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            // EMITE DOCUMENTO - CIOT
            retorno = EmitirDocumento(usuario, senha, iDProgDiariaTranspVeic, TipoOperacao.DeclaracaoOperacaoTransporte);
            if (retorno != "") return retorno;

            // EMITE DOCUMENTO - PEDÁGIO
            retorno = EmitirDocumento(usuario, senha, iDProgDiariaTranspVeic, TipoOperacao.ReciboPedagioTAG);
            if (retorno != "") return retorno;

            return retorno;
        }

        public static string EncerrarOperacaoTransporte(int iDProgDiariaTranspVeic)
        {
            // Documentação - Cenário 11
            // https://targetmp.atlassian.net/wiki/spaces/DOC/pages/402227318/11-+WS+2.0+Frete+Cen+rio+11

            string retorno = "";

            // SERVIDOR HOMOLOGAÇÃO
            var usuario = "tms.grupohy";
            var senha = "123@Mudar";

            //var retornoOperacao = BuscarOperacaoTransporteObj(usuario, senha, iDProgDiariaTranspVeic);
            //var item = retornoOperacao.Itens.FirstOrDefault();

            // ENCERRAR OPERAÇÃO DE TRANSPORTE
            retorno = EncerrarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            // FINALIZAR OPERAÇÃO DE TRANSPORTE
            retorno = FinalizarOperacaoTransporte(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            return retorno;
        }

        public static string ValePedagioAvulsoSemParar(int iDProgDiariaTranspVeic)
        {
            // Documentação - Cenário 23
            // https://targetmp.atlassian.net/wiki/spaces/DOC/pages/402751533/23-+WS+2.0+Frete+Cen+rio+23

            string retorno = "";

            // SERVIDOR HOMOLOGAÇÃO
            var usuario = "tms.grupohy";
            var senha = "123@Mudar";

            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
            FinanceiroEntities apoloSession = new FinanceiroEntities();
            var carga = hlbappSession.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == iDProgDiariaTranspVeic).FirstOrDefault();

            var entCodTransportador = "";
            if (carga.EntCod == null)
                return "Código da Transportadora não relacionado na carga! Verifique para poder gerar a Integração com a TARGET!";
            else
                entCodTransportador = carga.EntCod;

            var entidadeTransportador = apoloSession.ENTIDADE.Where(w => w.EntCod == entCodTransportador).FirstOrDefault();

            // CADASTRA / ATUALIZA TRANSPORTADOR
            retorno = CadastrarAtualizarTransportador(usuario, senha, entCodTransportador);
            if (retorno != "") return retorno;

            // CADASTRA / ATUALIZA MOTORISTA 01
            retorno = CadastrarAtualizarMotorista(usuario, senha, entidadeTransportador.EntCpfCgc, carga.EntCodMotorista01);
            if (retorno != "") return retorno;

            #region CADASTRA / ATUALIZA PARTICIPANTES

            var listaClientes = new List<Models.ENTIDADE>();

            var listaPedidosCarga = hlbappSession.Prog_Diaria_Transp_Pedidos
                .Where(w => w.EmpresaTranportador == carga.EmpresaTranportador && w.DataProgramacao == carga.DataProgramacao
                        && w.NumVeiculo == carga.NumVeiculo)
                .GroupBy(g => new { g.CodigoCliente })
                .Select(s => new { s.Key.CodigoCliente, Ordem = s.Min(m => m.Ordem) })
                .OrderBy(o => o.Ordem)
                .ToList();

            foreach (var pedido in listaPedidosCarga)
            {
                var entidade = apoloSession.ENTIDADE.Where(w => w.EntCod == pedido.CodigoCliente).FirstOrDefault();
                if (listaClientes.Where(w => w.EntCod == pedido.CodigoCliente).Count() == 0)
                    listaClientes.Add(entidade);
            }

            foreach (var cliente in listaClientes)
            {
                retorno = CadastrarAtualizarParticipante(usuario, senha, cliente.EntCod);
                if (retorno != "") return retorno;
            }

            #endregion

            // CADASTRA / ROTEIRO
            retorno = CadastrarRoteiro(usuario, senha, iDProgDiariaTranspVeic);
            if (retorno != "") return retorno;

            #region Verifica se tem valor de pedágio para selecionar o meio de pagamento correto

            bool compraValePedagio = false;
            decimal valorPedagio = 0;
            var retornoCustoRota = ObterCustoRota(usuario, senha, iDProgDiariaTranspVeic);
            if (retornoCustoRota.Contains("Erro"))
                return retornoCustoRota;
            else if (Decimal.TryParse(retornoCustoRota, out valorPedagio))
                if (valorPedagio > 0)
                    compraValePedagio = true;

            #endregion

            // BUSCA VALE PEDÁGIO PARA CANCELAR SE EXISTIR
            var retornoStatusOperacao = BuscarCompraValePedagio(usuario, senha, iDProgDiariaTranspVeic);
            if (retornoStatusOperacao.Contains("Erro"))
                return retornoStatusOperacao;
            else if (!retornoStatusOperacao.Contains("3") && retornoStatusOperacao != "")
            {
                // CANCELA OPERAÇÃO DE TRANSPORTE
                retorno = CancelarCompraValePedagio(usuario, senha, iDProgDiariaTranspVeic);
                if (retorno != "") return retorno;
            }

            if (compraValePedagio)
            {
                // COMPRAR PEDÁGIO AVULSO
                retorno = ComprarPedagioAvulso(usuario, senha, iDProgDiariaTranspVeic);
                if (retorno != "") return retorno;

                // CONFIRMA PEDAGIO TAG
                retorno = ConfirmarPedagioTAG(usuario, senha, iDProgDiariaTranspVeic);
                if (retorno != "") return retorno;

                // EMITE DOCUMENTO - PEDÁGIO
                retorno = EmitirDocumento(usuario, senha, iDProgDiariaTranspVeic, TipoOperacao.ReciboPedagioTAG);
                if (retorno != "") return retorno;
            }

            return retorno;
        }

        public static void GeraCIOTValePedagioCargas()
        {
            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();
            Models.Apolo.ApoloEntities apoloSession = new Models.Apolo.ApoloEntities();

            DateTime dataInicio = Convert.ToDateTime("10/03/2021");
            DateTime dataOntem = DateTime.Today.AddDays(-1);
            DateTime dataHoje = DateTime.Today;

            var listaCargasSemTARGIT = hlbappSession.Prog_Diaria_Transp_Veiculos
                .Where(w => w.DataEmbarque >= dataOntem && w.DataEmbarque <= dataHoje
                        && w.EntCod != null
                        && w.EntCodMotorista01 != null
                        && w.EquipCodEstrVeiculo != null
                        && w.IdOperacaoTransporte == null
                        && (w.EmpresaTranportador == "TR" || w.EmpresaTranportador == "HN" || w.EmpresaTranportador == "PL")
                        // Data que vai iniciar o processo
                        && w.DataEmbarque >= dataInicio)
                .ToList();

            foreach (var item in listaCargasSemTARGIT)
            {
                var listaEmpresasNaCarga = hlbappSession.Prog_Diaria_Transp_Pedidos
                    .Where(w => w.EmpresaTranportador == item.EmpresaTranportador
                        && w.DataProgramacao == item.DataProgramacao
                        && w.NumVeiculo == item.NumVeiculo)
                    .GroupBy(g => new
                    {
                        g.Empresa
                    })
                    .Select(s => new
                    {
                        s.Key.Empresa
                    })
                    .ToList();

                /*
                 * 20/04/2021 - Só deve gerar CIOT ou Vale Pedágio somente se tiver uma empresa.
                 *              Se for carga compartilhada, não deve gerar nenhum.
                 *              Transema: somente Vale Pedágio por ser transportadora.
                 *              Outros: CIOT e Vale Pedágio por ser autônomo.
                */
                if (listaEmpresasNaCarga.Count == 1)
                {
                    #region Carrega os pedidos da carga

                    var listaPedidos = new List<string>();

                    var listaPedidosVenda = hlbappSession.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.EmpresaTranportador == item.EmpresaTranportador
                            && w.DataProgramacao == item.DataProgramacao
                            && w.NumVeiculo == item.NumVeiculo
                            && w.CHICNum != "")
                        .GroupBy(g => new
                        {
                            g.CHICNum,
                        })
                        .Select(s => new
                        {
                            s.Key.CHICNum
                        })
                        .ToList();

                    foreach (var venda in listaPedidosVenda)
                    {
                        listaPedidos.Add(venda.CHICNum);
                    }

                    var listaPedidosReposicao = hlbappSession.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.EmpresaTranportador == item.EmpresaTranportador
                            && w.DataProgramacao == item.DataProgramacao
                            && w.NumVeiculo == item.NumVeiculo
                            && w.CHICNumReposicao != "")
                        .GroupBy(g => new
                        {
                            g.CHICNumReposicao,
                        })
                        .Select(s => new
                        {
                            s.Key.CHICNumReposicao
                        })
                        .ToList();

                    foreach (var reposicao in listaPedidosReposicao)
                    {
                        listaPedidos.Add(reposicao.CHICNumReposicao);
                    }

                    #endregion

                    List<string> pedidos = new List<string>();

                    foreach (var pedido in listaPedidos)
                    {
                        #region Verifica se todos pedidos tem NF. Caso algum falte, não gera o TARGIT

                        var nf = apoloSession.NOTA_FISCAL
                           .Where(w => apoloSession.PED_VENDA1.Any(a => a.EmpCod == w.EmpCod && a.PedVendaNum == w.NFPedVenda && a.USERPEDCHIC == pedido)
                               && apoloSession.ITEM_NF.Any(i => i.EmpCod == w.EmpCod && i.CtrlDFModForm == w.CtrlDFModForm && i.CtrlDFSerie == w.CtrlDFSerie
                                   && i.NFNum == w.NFNum
                                   && apoloSession.PRODUTO.Any(p => i.ProdCodEstr == p.ProdCodEstr
                                           && (p.FxaProdCod == "7" || p.FxaProdCod == "8"))))
                           .FirstOrDefault();

                        if (nf != null)
                        {
                            if ((DateTime.Now - Convert.ToDateTime(nf.NFDataSaidaEntrada)).TotalHours > 4)
                                pedidos.Add(pedido);
                        }
                        else
                        {
                            pedidos = new List<string>();
                            break;
                        }

                        #endregion
                    }

                    if (pedidos.Count > 0)
                        if (item.EmpresaTranportador == "TR")
                            ValePedagioAvulsoSemParar(item.ID);
                        else
                            IntegraTargetCIOTPadraoEValePedagioSemParar(item.ID);
                }
            }
        }

        #endregion

        protected void imgbCopiaDadosManuais_Click(object sender, ImageClickEventArgs e)
        {
            VerificaSessao();

            HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            DateTime dataCopia = Convert.ToDateTime(txtDataProgramacao.Text);
            string empresaTransp = ddlEmpresaTransportadora.SelectedValue;
            DateTime dataNova = new DateTime();
            bool ok = true;
            if (txtDataProximoDia.Text.Replace("/","").Trim() == "")
            {
                lblMensagem2.Visible = true;
                lblMensagem2.Text = "Data do dia que irá receber a cópia é obrigatória!";
                txtDataProximoDia.Focus();
            }
            else
            {
                dataNova = Convert.ToDateTime(txtDataProximoDia.Text);

                if (dataCopia == dataNova)
                {
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "As datas devem ser diferentes!";
                    ok = false;
                }

                if (empresaTransp != "TO")
                {
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "A cópia só pode ser feita para 'Transferência de Ovos'!";
                    ok = false;
                }

                if (dataNova < dataCopia)
                {
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "A data do dia que irá receber a cópia deve ser maior que a da cópia!";
                    ok = false;
                }

                if (ok)
                {
                    lblMensagem2.Visible = false;
                    lblMensagem2.Text = "";

                    #region Copia Pedidos

                    List<Prog_Diaria_Transp_Pedidos> listaPedidosACopiar = hlbapp.Prog_Diaria_Transp_Pedidos
                        .Where(w => w.EmpresaTranportador == empresaTransp && w.DataProgramacao == dataCopia
                            && w.CodigoCliente == null)
                        .ToList();

                    foreach (var pedido in listaPedidosACopiar)
                    {
                        Prog_Diaria_Transp_Pedidos novoPedido = new Prog_Diaria_Transp_Pedidos();
                        novoPedido.DataProgramacao = dataNova;
                        novoPedido.NomeCliente = pedido.NomeCliente;
                        novoPedido.NumVeiculo = pedido.NumVeiculo;
                        novoPedido.Quantidade = pedido.Quantidade;
                        novoPedido.LocalEntrega = pedido.LocalEntrega;
                        novoPedido.Embalagem = pedido.Embalagem;
                        novoPedido.DataEntrega = dataNova;
                        novoPedido.Observacao = pedido.Observacao;
                        novoPedido.Ordem = pedido.Ordem;
                        novoPedido.EmpresaTranportador = pedido.EmpresaTranportador;
                        novoPedido.Empresa = pedido.Empresa;
                        novoPedido.Status = pedido.Status;
                        novoPedido.QuantidadeCaixa = pedido.QuantidadeCaixa;

                        hlbapp.Prog_Diaria_Transp_Pedidos.AddObject(novoPedido);
                    }

                    #endregion

                    #region Copia Cargas

                    List<Prog_Diaria_Transp_Veiculos> listaCargasACopiar = hlbapp.Prog_Diaria_Transp_Veiculos
                        .Where(w => w.EmpresaTranportador == empresaTransp && w.DataProgramacao == dataCopia)
                        .ToList();

                    foreach (var carga in listaCargasACopiar)
                    {
                        Prog_Diaria_Transp_Veiculos novaCarga = new Prog_Diaria_Transp_Veiculos();
                        novaCarga.DataProgramacao = dataNova;
                        novaCarga.NumVeiculo = carga.NumVeiculo;
                        novaCarga.Placa = carga.Placa;
                        novaCarga.Motorista01 = carga.Motorista01;
                        novaCarga.Motorista02 = carga.Motorista02;
                        novaCarga.QuantidadeTotal = carga.QuantidadeTotal;
                        novaCarga.QuantidadePorCaixa = carga.QuantidadePorCaixa;
                        novaCarga.QunatidadeCaixa = carga.QunatidadeCaixa;
                        novaCarga.ValorTotal = carga.ValorTotal;
                        novaCarga.EmpresaTranportador = carga.EmpresaTranportador;
                        novaCarga.InicioCarregamentoEsperado = carga.InicioCarregamentoEsperado;
                        novaCarga.HorarioEntregaNF = carga.HorarioEntregaNF;
                        novaCarga.Tranportadora = carga.Tranportadora;
                        novaCarga.ValorKM = carga.ValorKM;
                        novaCarga.UnidadeBaseEmbarcador = carga.UnidadeBaseEmbarcador;
                        novaCarga.CargaLiberada = carga.CargaLiberada;
                        novaCarga.DataEmbarque = dataNova;
                        novaCarga.OdometroVeiculoDataEmbarque = carga.OdometroVeiculoDataEmbarque;

                        hlbapp.Prog_Diaria_Transp_Veiculos.AddObject(novaCarga);
                    }

                    #endregion

                    hlbapp.SaveChanges();

                    AtualizaValoresVeiculos(dataNova, "PDT");

                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = "Dados da data " + dataCopia.ToShortDateString() + " copiado para a data "
                        + dataNova.ToShortDateString() + " com sucesso!";
                }
            }
        }

        #endregion

        #region Lista de Veículos

        #region ListView1 Methods

        protected void ListView1_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            #region Integração Embarcador (DESATIVADA)

            //string placaAntiga = e.OldValues[2].ToString();
            //string placaNova = e.NewValues[2].ToString();

            #region Localiza objeto da carga

            //HLBAPPEntities1 hlbapp = new HLBAPPEntities1();

            //int numCaminhao = Convert.ToInt32(e.NewValues[1].ToString());
            //string empresaTransportadora = ddlEmpresaTransportadora.SelectedValue;
            //DateTime dataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);

            //Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
            //    .Where(w => w.DataProgramacao == dataProgramacao
            //        && w.EmpresaTranportador == empresaTransportadora
            //        && w.NumVeiculo == numCaminhao).FirstOrDefault();

            #endregion

            //if (carga != null)
            //{
            #region Integra com o Embarcador

            #region Atualiza Veículo

            //string retornoEmbarcador = AtualizaVeiculoCargaEmbarcador(carga.ID, placaAntiga, placaNova);
            //if (retornoEmbarcador != "")
            //{
            //    lblMensagem2.Visible = true;
            //    lblMensagem2.Text = retornoEmbarcador;
            //    e.Cancel = true;
            //}
            //else
            //{
            //    lblMensagem2.Visible = false;
            //    lblMensagem2.Text = "";
            //}

            #endregion

            #endregion
            //}

            #endregion
        }

        protected void ListView1_ItemUpdated(object sender, ListViewUpdatedEventArgs e)
        {
            DateTime dataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);
            AtualizaValoresVeiculos(dataProgramacao, "PDT");
        }

        protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Label numVeiculo = (Label)e.Item.FindControl("NumVeiculoLabel");
            if (numVeiculo != null)
            {
                if (numVeiculo.Text.Equals("0"))
                {
                    #region Esconde os labels de valores do primeiro registro para aparecer somente os cabeçalhos

                    numVeiculo.Visible = false;
                    Label PlacaLabel = (Label)e.Item.FindControl("PlacaLabel");
                    if (PlacaLabel != null) PlacaLabel.Visible = false;
                    Label Motorista01Label = (Label)e.Item.FindControl("Motorista01Label");
                    if (Motorista01Label != null) Motorista01Label.Visible = false;
                    Label Motorista02Label = (Label)e.Item.FindControl("Motorista02Label");
                    if (Motorista02Label != null) Motorista02Label.Visible = false;
                    Label QuantidadeTotalLabel = (Label)e.Item.FindControl("QuantidadeTotalLabel");
                    if (QuantidadeTotalLabel != null) QuantidadeTotalLabel.Visible = false;
                    Label QuantidadePorCaixaLabel = (Label)e.Item.FindControl("QuantidadePorCaixaLabel");
                    if (QuantidadePorCaixaLabel != null) QuantidadePorCaixaLabel.Visible = false;
                    Label ValorTotalLabel = (Label)e.Item.FindControl("ValorTotalLabel");
                    if (ValorTotalLabel != null) ValorTotalLabel.Visible = false;
                    Label DataEmbarqueLabel = (Label)e.Item.FindControl("DataEmbarqueLabel");
                    if (DataEmbarqueLabel != null) DataEmbarqueLabel.Visible = false;
                    Label ValorKMLabel = (Label)e.Item.FindControl("ValorKMLabel");
                    if (ValorKMLabel != null) ValorKMLabel.Visible = false;
                    ImageButton ImageButton1 = (ImageButton)e.Item.FindControl("ImageButton1");
                    if (ImageButton1 != null) ImageButton1.Visible = false;

                    #endregion
                }
                else
                {
                    #region Esconde os labels de cabeçalho dos outros registros

                    Label lblNumVeiculo = (Label)e.Item.FindControl("lblNumVeiculo");
                    if (lblNumVeiculo != null) lblNumVeiculo.Visible = false;
                    Label lblPlaca = (Label)e.Item.FindControl("lblPlaca");
                    if (lblPlaca != null) lblPlaca.Visible = false;
                    Label lblMotorista01 = (Label)e.Item.FindControl("lblMotorista01");
                    if (lblMotorista01 != null) lblMotorista01.Visible = false;
                    Label lblMotorista02 = (Label)e.Item.FindControl("lblMotorista02");
                    if (lblMotorista02 != null) lblMotorista02.Visible = false;
                    Label lblQtdeTotal = (Label)e.Item.FindControl("lblQtdeTotal");
                    if (lblQtdeTotal != null) lblQtdeTotal.Visible = false;
                    Label lblQuantidadePorCaixa = (Label)e.Item.FindControl("lblQuantidadePorCaixa");
                    if (lblQuantidadePorCaixa != null) lblQuantidadePorCaixa.Visible = false;
                    Label lblQunatidadeCaixa = (Label)e.Item.FindControl("lblQunatidadeCaixa");
                    if (lblQunatidadeCaixa != null) lblQunatidadeCaixa.Visible = false;
                    Label lblValorTotal = (Label)e.Item.FindControl("lblValorTotal");
                    if (lblValorTotal != null) lblValorTotal.Visible = false;
                    Label lblDataEmbarque = (Label)e.Item.FindControl("lblDataEmbarque");
                    if (lblDataEmbarque != null) lblDataEmbarque.Visible = false;
                    Label lblInicioCarregReal = (Label)e.Item.FindControl("lblInicioCarregReal");
                    if (lblInicioCarregReal != null) lblInicioCarregReal.Visible = false;
                    Label lblTerminoCarregReal = (Label)e.Item.FindControl("lblTerminoCarregReal");
                    if (lblTerminoCarregReal != null) lblTerminoCarregReal.Visible = false;
                    Label lblValorKM = (Label)e.Item.FindControl("lblValorKM");
                    if (lblValorKM != null) lblValorKM.Visible = false;

                    #endregion
                }

                #region Esconte botão de alteração caso o usuário não tenha acesso a alterar

                if ((Convert.ToBoolean(Session["modoImpressao"])) || (Convert.ToBoolean(Session["modoExcel"]))
                    || (!MvcAppHyLinedoBrasil.Controllers.AccountController
                            .GetGroup("HLBAPP-ProgDiariaTranspAlterarDadosPedidos",
                            (System.Collections.ArrayList)Session["Direitos"])))
                {
                    ImageButton ImageButton1 = (ImageButton)e.Item.FindControl("ImageButton1");
                    if (ImageButton1 != null) ImageButton1.Visible = false;
                }

                #endregion
            }

            DropDownList ddlNumVeiculo = (DropDownList)e.Item.FindControl("ddlNumVeiculo");
            if (ddlNumVeiculo != null)
            {
                #region Carrega DropDownList das Placas e Motoristas

                DropDownList ddlPlacas = (DropDownList)e.Item.FindControl("ddlPlacas");
                DropDownList ddlMotorista01 = (DropDownList)e.Item.FindControl("ddlMotorista01");
                DropDownList ddlMotorista02 = (DropDownList)e.Item.FindControl("ddlMotorista02");

                if (ddlPlacas != null)
                {
                    string transportadora = "0000807";

                    HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();

                    DateTime dataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);
                    int numCarga = Convert.ToInt32(ddlNumVeiculo.SelectedValue);

                    int idPDTV = 0;
                    Prog_Diaria_Transp_Veiculos carga = hlbappSession.Prog_Diaria_Transp_Veiculos
                        .Where(w => w.EmpresaTranportador == "TR"
                            && w.DataProgramacao == dataProgramacao
                            && w.NumVeiculo == numCarga)
                        .FirstOrDefault();
                    if (carga != null) idPDTV = carga.ID;

                    CarregaPlacasEMotoristasDaTransportadora(transportadora, ddlPlacas, ddlMotorista01, ddlMotorista02, idPDTV);
                }

                #endregion

                #region Valida Campos

                //ImageButton ImageButton3 = (ImageButton)e.Item.FindControl("ImageButton3");
                //if (ddlPlacas.SelectedValue != "" || ddlMotorista01.SelectedValue == "")
                //    ImageButton3.Enabled = false;
                //else
                //    ImageButton3.Enabled = true;

                #endregion
            }
        }

        protected void ListView1_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            //ValidaCamposListaVeiculos();
        }

        public bool ValidaCamposListaVeiculos()
        {
            bool ok = true;
            string msg = "";
            ImageButton ImageButton3 = (ImageButton)ListView1.EditItem.FindControl("ImageButton3");

            DropDownList ddlPlacas = (DropDownList)ListView1.EditItem.FindControl("ddlPlacas");
            DropDownList ddlMotorista01 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista01");
            DropDownList ddlMotorista02 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista02");
            TextBox QuantidadePorCaixaTextBox = (TextBox)ListView1.EditItem.FindControl("QuantidadePorCaixaTextBox");
            TextBox txtDataEmbarque = (TextBox)ListView1.EditItem.FindControl("txtDataEmbarque");
            TextBox ValorKMTextBox = (TextBox)ListView1.EditItem.FindControl("ValorKMTextBox");
            int qtde = 0;
            decimal valor = 0;
            DateTime data = new DateTime();
            if (ddlPlacas.SelectedValue == "")
            {
                ok = false;
                msg = "PLACA Obrigatória!";
            }
            else if (ddlMotorista01.SelectedValue == "")
            {
                ok = false;
                msg = "MOTORISTA 01 Obrigatório!";
            }
            else if (ddlMotorista01.SelectedValue == ddlMotorista02.SelectedValue && ddlMotorista02.SelectedValue != "")
            {
                ok = false;
                msg = "MOTORISTA 01 não pode ser igual ao MOTORISTA 02!";
            }
            else if (!int.TryParse(QuantidadePorCaixaTextBox.Text, out qtde))
            {
                ok = false;
                msg = "QTDE. P/ CAIXA obrigatória!";
            }
            else if (!DateTime.TryParse(txtDataEmbarque.Text, out data))
            {
                ok = false;
                msg = "DATA DE EMBARQUE incorreta!";
            }
            else if (!decimal.TryParse(ValorKMTextBox.Text.Replace(".","").Replace(",","."), out valor))
            {
                ok = false;
                msg = "VALOR DO KM incorreto!";
            }
            else if (valor <= 0)
            {
                ok = false;
                msg = "VALOR DO KM não pode ser zerado!";
            }

            //ImageButton3.Enabled = ok;
            lblMensagem4.Visible = !ok;
            lblMensagem4.Text = msg;

            return ok;
        }

        protected void ddlPlacasListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlPlacas = (DropDownList)ListView1.EditItem.FindControl("ddlPlacas");
            TextBox PlacaTextBox = (TextBox)ListView1.EditItem.FindControl("PlacaTextBox");
            if (ddlPlacas.SelectedValue != "")
                PlacaTextBox.Text = ddlPlacas.SelectedItem.Text;
            else
                PlacaTextBox.Text = "";
            //ValidaCamposListaVeiculos();
        }

        protected void ddlMotorista01ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlMotorista01 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista01");
            TextBox Motorista01TextBox = (TextBox)ListView1.EditItem.FindControl("Motorista01TextBox");
            if (ddlMotorista01.SelectedValue != "")
                Motorista01TextBox.Text = ddlMotorista01.SelectedItem.Text;
            else
                Motorista01TextBox.Text = "";
            //ValidaCamposListaVeiculos();
        }

        protected void ddlMotorista02ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlMotorista02 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista02");
            TextBox Motorista02TextBox = (TextBox)ListView1.EditItem.FindControl("Motorista02TextBox");
            if (ddlMotorista02.SelectedValue != "")
                Motorista02TextBox.Text = ddlMotorista02.SelectedItem.Text;
            else
                Motorista02TextBox.Text = "";
            //ValidaCamposListaVeiculos();
        }

        protected void imgbUpdateVeiculoListView_Click(object sender, ImageClickEventArgs e)
        {
            if (!ValidaCamposListaVeiculos())
                return;

            DateTime data = Convert.ToDateTime(txtDataProgramacao.Text);
            DropDownList ddlNumVeiculo = (DropDownList)ListView1.EditItem.FindControl("ddlNumVeiculo");
            int numVeiculo = Convert.ToInt32(ddlNumVeiculo.Text);
            DropDownList ddlPlacas = (DropDownList)ListView1.EditItem.FindControl("ddlPlacas");
            DropDownList ddlMotorista01 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista01");
            DropDownList ddlMotorista02 = (DropDownList)ListView1.EditItem.FindControl("ddlMotorista02");
            TextBox QuantidadePorCaixaTextBox = (TextBox)ListView1.EditItem.FindControl("QuantidadePorCaixaTextBox");
            TextBox valorDespesas = (TextBox)ListView1.EditItem.FindControl("ValorTotalTextBox");
            TextBox txtDataEmbarque = (TextBox)ListView1.EditItem.FindControl("txtDataEmbarque");
            TextBox ValorKMTextBox = (TextBox)ListView1.EditItem.FindControl("ValorKMTextBox");

            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();

            var pDTV = hlbappSession.Prog_Diaria_Transp_Veiculos
                .Where(w => w.EmpresaTranportador == "TR" && w.DataProgramacao == data && w.NumVeiculo == numVeiculo)
                .FirstOrDefault();

            pDTV.Placa = ddlPlacas.SelectedItem.Text;
            pDTV.EquipCodEstrVeiculo = ddlPlacas.SelectedValue;
            pDTV.Motorista01 = ddlMotorista01.SelectedItem.Text;
            pDTV.EntCodMotorista01 = ddlMotorista01.SelectedValue;
            if (ddlMotorista02.SelectedValue != "")
            {
                pDTV.Motorista02 = ddlMotorista02.SelectedItem.Text;
                pDTV.EntCodMotorista02 = ddlMotorista02.SelectedValue;
            }
            else
            {
                pDTV.Motorista02 = "";
                pDTV.EntCodMotorista02 = "";
            }
            pDTV.QuantidadePorCaixa = Convert.ToInt32(QuantidadePorCaixaTextBox.Text);
            decimal valor = 0;
            if (decimal.TryParse(valorDespesas.Text, out valor))
                pDTV.ValorTotal = valor;
            pDTV.DataEmbarque = Convert.ToDateTime(txtDataEmbarque.Text);
            pDTV.ValorKM = Convert.ToDecimal(ValorKMTextBox.Text);

            hlbappSession.SaveChanges();

            AtualizaValoresVeiculos(data, "PDT");

            ListView1.EditIndex = -1;
            ListView1.DataBind();
        }

        #endregion

        #endregion

        #region Tabela de Veículo

        #region gdvVeiculos Methods

        protected void gdvVeiculos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            #region Esconde Linha Em Branco

            System.Web.UI.WebControls.Label lblID =
                        (System.Web.UI.WebControls.Label)e.Row.FindControl("Label9");
            if (lblID != null)
                if (lblID.Text.Equals("2682"))
                {
                    lblID.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("ImageButton1");
                    imgbEdit.Visible = false;
                    System.Web.UI.WebControls.ImageButton imgbCancelEdit =
                        (System.Web.UI.WebControls.ImageButton)e.Row.FindControl("ImageButton2");
                    imgbCancelEdit.Visible = false;
                    System.Web.UI.WebControls.Label lblNumCarga =
                        (System.Web.UI.WebControls.Label)e.Row.FindControl("Label1");
                    lblNumCarga.Visible = false;
                }

            #endregion

            #region Botões Editar e Inserir

            if (lblID != null)
            {
                int id = Convert.ToInt32(lblID.Text);

                Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos
                        .Where(w => w.ID == id).FirstOrDefault();

                if (carga != null)
                {
                    if (!MvcAppHyLinedoBrasil.Controllers.AccountController
                            .GetGroup("HLBAPP-ProgDiariaTranspAlterarDadosPedidos",
                            (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        ImageButton imgbEditItem = (ImageButton)e.Row.FindControl("ImageButton1");
                        imgbEditItem.Visible = false;
                        ImageButton imgDelete = (ImageButton)e.Row.FindControl("ImageButton2");
                        imgDelete.Visible = false;
                    }
                }
            }

            #endregion

            #region Carrega DropDownList das Placas e Motoristas

            System.Web.UI.WebControls.Label lblIDEdit =
                (System.Web.UI.WebControls.Label)e.Row.FindControl("Label1");

            if (lblIDEdit != null)
            {
                int id = 0;
                if (int.TryParse(lblIDEdit.Text, out id))
                {
                    //int id = Convert.ToInt32(lblID.Text);
                    Prog_Diaria_Transp_Veiculos carga = hlbapp.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == id).FirstOrDefault();
                    if (carga != null)
                    {
                        DropDownList ddlTransportadoras = (DropDownList)e.Row.FindControl("ddlTransportadoras");
                        DropDownList ddlPlacas = (DropDownList)e.Row.FindControl("ddlPlacas");
                        DropDownList ddlMotorista01 = (DropDownList)e.Row.FindControl("ddlMotorista01");
                        DropDownList ddlMotorista02 = (DropDownList)e.Row.FindControl("ddlMotorista02");

                        CarregaPlacasEMotoristasDaTransportadora(ddlTransportadoras.SelectedValue, ddlPlacas, ddlMotorista01, ddlMotorista02, carga.ID);

                        //System.Web.UI.WebControls.TextBox TransportadoraTextBox =
                        //    (System.Web.UI.WebControls.TextBox)e.Row.FindControl("TransportadoraTextBox");

                        //ENTIDADE1 entidade1Apolo = apolo.ENTIDADE1
                        //    .Where(w => apolo.ENTIDADE.Any(a => w.EntCod == a.EntCod
                        //            && a.EntNomeFant == carga.Tranportadora)
                        //        && w.USERParticipaProgTranspWEB == "Sim"
                        //        && w.USEREmpresaControlaTransp == ddlEmpresaTransportadora.SelectedValue)
                        //    .FirstOrDefault();

                        //if (entidade1Apolo == null)
                        //{
                        //    if (ddlTransportadoras != null) ddlTransportadoras.Visible = false;
                        //    if (TransportadoraTextBox != null) TransportadoraTextBox.Visible = true;
                        //}
                        //else
                        //{
                        //    if (ddlTransportadoras != null) ddlTransportadoras.Visible = true;
                        //    if (TransportadoraTextBox != null) TransportadoraTextBox.Visible = false;
                        //}
                    }
                }
            }

            #endregion
        }

        protected void gdvVeiculos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            lblMensagem4.Visible = false;
            lblMensagem4.Text = "";
            gdvVeiculos.EditIndex = e.NewEditIndex;
        }

        protected void gdvVeiculos_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            int id = Convert.ToInt32(e.Keys[0]);
            int idCarga = 0;

            #region Carrega carga do Embarcador

            string ret = Embarcador.buscaCargaCodigo(id);
            if (int.TryParse(ret, out idCarga)) idCarga = Convert.ToInt32(ret);

            #endregion

            if (idCarga > 0)
            {
                #region Apaga a carga

                bool cargaApagada = false;
                string retornoCarga = Embarcador.apagaCarga(idCarga);
                if (!Boolean.TryParse(retornoCarga, out cargaApagada)) retornoCarga = "Erro ao apagar carga: " + retornoCarga;

                if (!cargaApagada)
                {
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = retornoCarga;
                }
                else
                {
                    lblMensagem2.Visible = false;
                    lblMensagem2.Text = "";
                }

                #endregion
            }

            #region Disvincula pedidos da carga

            HLBAPPEntities1 hlbapp1 = new HLBAPPEntities1();

            int numVeiculo = Convert.ToInt32(e.Values[0]);
            string empresaTransportador = ddlEmpresaTransportadora.SelectedValue;
            DateTime dataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);
            var listaPedidos = hlbapp1.Prog_Diaria_Transp_Pedidos
                .Where(w => w.EmpresaTranportador == empresaTransportador
                    && w.DataProgramacao == dataProgramacao
                    && w.NumVeiculo == numVeiculo).ToList();

            foreach (var item in listaPedidos)
            {
                item.NumVeiculo = 0;
            }

            hlbapp1.SaveChanges();

            GridView1.DataBind();

            #endregion
        }

        protected void gdvVeiculos_Load(object sender, EventArgs e)
        {
            #region Esconde Dados de Exportação caso o Tipo não seja Exportação

            if (ddlEmpresaTransportadora.SelectedValue != "EX")
            {
                gdvVeiculos.Columns[19].Visible = false;
                gdvVeiculos.Columns[20].Visible = false;
                gdvVeiculos.Columns[21].Visible = false;
                gdvVeiculos.Columns[22].Visible = false;
            }
            else
            {
                gdvVeiculos.Columns[19].Visible = true;
                gdvVeiculos.Columns[20].Visible = true;
                gdvVeiculos.Columns[21].Visible = true;
                gdvVeiculos.Columns[22].Visible = true;
            }

            #endregion
        }

        protected void gdvVeiculos_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            lblMensagem4.Visible = false;
            lblMensagem4.Text = "";
        }

        #endregion

        #region CRUD Methods

        protected void imgbAddVeiculo_Click(object sender, ImageClickEventArgs e)
        {
            gdvVeiculos.ShowFooter = true;
        }

        protected void btnAddVeiculoNovo_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            TextBox txtNumVeiculo = (TextBox)gdvVeiculos.FooterRow.FindControl("txtNumVeiculo");
            if (ValidaValorVazioTabelaVeiculos(txtNumVeiculo.Text, "Nº CARGA")) return;
            DropDownList ddlTransportadoras = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlTransportadoras");
            if (ValidaValorVazioTabelaVeiculos(ddlTransportadoras.SelectedValue, "TRANSPORTADORA")) return;
            DropDownList ddlPlacas = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlPlacas");
            if (ValidaValorVazioTabelaVeiculos(ddlPlacas.SelectedValue, "PLACA")) return;
            DropDownList ddlMotorista01 = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlMotorista01");
            if (ValidaValorVazioTabelaVeiculos(ddlMotorista01.SelectedValue, "MOTORISTA 01")) return;
            DropDownList ddlMotorista02 = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlMotorista02");
            if (ddlMotorista01.SelectedValue == ddlMotorista02.SelectedValue)
            {
                lblMensagem4.Visible = true;
                lblMensagem4.Text = "O motorista 01 e 02 não podem ser o mesmo!";
            }
            else
            {
                lblMensagem4.Visible = false;
                lblMensagem4.Text = "";
            }
            //TextBox PlacaTextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("PlacaTextBox");
            //TextBox Motorista01TextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("Motorista01TextBox");
            //TextBox Motorista02TextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("Motorista02TextBox");
            TextBox txtInicioCarregEsperado = (TextBox)gdvVeiculos.FooterRow.FindControl("txtInicioCarregEsperado");
            TextBox txtHorarioEntregaNF = (TextBox)gdvVeiculos.FooterRow.FindControl("txtHorarioEntregaNF");
            //TextBox TransportadoraTextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("TransportadoraTextBox");
            //var entCod = "";
            //ENTIDADE1 entidade1Apolo = apolo.ENTIDADE1
            //    .Where(w => apolo.ENTIDADE.Any(a => w.EntCod == a.EntCod
            //            //&& a.EntNomeFant == ddlTransportadoras.SelectedValue)
            //            && a.EntCod == ddlTransportadoras.SelectedValue)
            //        && w.USERParticipaProgTranspWEB == "Sim"
            //        && w.USEREmpresaControlaTransp == ddlEmpresaTransportadora.SelectedValue).FirstOrDefault();

            //if (entidade1Apolo != null)
            //{
            //    entCod = entidade1Apolo.EntCod;
            //}

            TextBox ValorKMTextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("ValorKMTextBox");
            TextBox txtDataEmbarque = (TextBox)gdvVeiculos.FooterRow.FindControl("txtDataEmbarque");
            if (ValidaValorVazioTabelaVeiculos(txtDataEmbarque.Text, "DATA DE EMBARQUE")) return;
            DropDownList ddlAeroportoOrigem = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlAeroportoOrigem");
            TextBox txtHorarioChegadaAeroporto = (TextBox)gdvVeiculos.FooterRow.FindControl("txtHorarioChegadaAeroporto");
            DropDownList ddlDespachante = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlDespachante");
            TextBox txtDataInicioVazio = (TextBox)gdvVeiculos.FooterRow.FindControl("txtDataInicioVazio");
            TextBox OdometroVeiculoDataEmbarqueTextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("OdometroVeiculoDataEmbarqueTextBox");

            Prog_Diaria_Transp_Veiculos veiculo = new Prog_Diaria_Transp_Veiculos();
            veiculo.DataProgramacao = Convert.ToDateTime(txtDataProgramacao.Text);
            veiculo.NumVeiculo = Convert.ToInt32(txtNumVeiculo.Text);
            string placaAntiga = veiculo.Placa;
            //veiculo.Placa = PlacaTextBox.Text;
            //veiculo.Motorista01 = Motorista01TextBox.Text;
            //veiculo.Motorista02 = Motorista02TextBox.Text;
            veiculo.Tranportadora = ddlTransportadoras.SelectedItem.Text;
            //veiculo.EntCod = entCod;
            veiculo.EntCod = ddlTransportadoras.SelectedValue;
            veiculo.Placa = ddlPlacas.SelectedItem.Text;
            veiculo.EquipCodEstrVeiculo = ddlPlacas.SelectedValue;
            veiculo.Motorista01 = ddlMotorista01.SelectedItem.Text;
            veiculo.EntCodMotorista01 = ddlMotorista01.SelectedValue;
            if (ddlMotorista02.SelectedValue != "")
            {
                veiculo.Motorista02 = ddlMotorista02.SelectedItem.Text;
                veiculo.EntCodMotorista02 = ddlMotorista02.SelectedValue;
            }
            else
            {
                veiculo.Motorista02 = "";
                veiculo.EntCodMotorista02 = null;
            }
            veiculo.QuantidadeTotal = 0;
            veiculo.QuantidadePorCaixa = 0;
            veiculo.QunatidadeCaixa = 0;
            veiculo.ValorTotal = 0;
            veiculo.EmpresaTranportador = ddlEmpresaTransportadora.SelectedValue;
            veiculo.InicioCarregamentoEsperado = txtInicioCarregEsperado.Text;
            veiculo.HorarioEntregaNF = txtHorarioEntregaNF.Text;
            //veiculo.Tranportadora = TransportadoraTextBox.Text;
            veiculo.DataEmbarque = Convert.ToDateTime(txtDataEmbarque.Text);
            if (ValorKMTextBox.Text != "") veiculo.ValorKM = Convert.ToDecimal(ValorKMTextBox.Text);
            if (OdometroVeiculoDataEmbarqueTextBox.Text != "") veiculo.OdometroVeiculoDataEmbarque = Convert.ToDecimal(OdometroVeiculoDataEmbarqueTextBox.Text);

            if (ddlAeroportoOrigem != null) veiculo.AeroportoOrigem = ddlAeroportoOrigem.SelectedValue;
            if (txtHorarioChegadaAeroporto != null) veiculo.HorarioChegadaAeroporto = txtHorarioChegadaAeroporto.Text;
            if (ddlDespachante != null) veiculo.Despachante = ddlDespachante.SelectedValue;
            if (txtDataInicioVazio != null)
            {
                DateTime dataVazio = new DateTime();
                if (DateTime.TryParse(txtDataInicioVazio.Text, out dataVazio)) veiculo.DataInicioVazio = dataVazio;
            }

            hlbapp.Prog_Diaria_Transp_Veiculos.AddObject(veiculo);

            hlbapp.SaveChanges();

            #region Integra com o Embarcador

            #region Atualiza Veículo

            //if (veiculo.Placa != "" && placaAntiga != veiculo.Placa)
            //{
            //    string retornoEmbarcador = AtualizaVeiculoCargaEmbarcador(veiculo.ID, placaAntiga, veiculo.Placa);
            //    if (retornoEmbarcador != "")
            //    {
            //        lblMensagem2.Visible = true;
            //        lblMensagem2.Text = retornoEmbarcador;
            //        return;
            //    }
            //    else
            //    {
            //        lblMensagem2.Visible = false;
            //        lblMensagem2.Text = "";
            //    }
            //}

            #endregion

            #endregion

            gdvVeiculos.DataBind();

            DateTime data = Convert.ToDateTime(veiculo.DataProgramacao);
            AtualizaValoresVeiculos(data, "PDT");

            gdvVeiculos.ShowFooter = false;
        }

        public bool ValidaValorVazioTabelaVeiculos(string valor, string campo)
        {
            bool eVazio = false;

            if (valor == "")
            {
                eVazio = true;
                lblMensagem4.Visible = true;
                lblMensagem4.Text = "Campo " + campo + " obrigatório! Verifique!";
            }
            else
            {
                lblMensagem4.Visible = false;
                lblMensagem4.Text = "";
            }

            return eVazio;
        }

        protected void btnCancelVeiculoNovo_Click(object sender, EventArgs e)
        {
            gdvVeiculos.ShowFooter = false;
            lblMensagem4.Visible = false;
            lblMensagem4.Text = "";
        }

        protected void imgbUpdateVeiculo_Click(object sender, ImageClickEventArgs e)
        {
            int index = gdvVeiculos.EditIndex;
            int id = 0;
            try
            {
                #region Pega Dados dos Controles

                Label lblID = (Label)gdvVeiculos.Rows[index].FindControl("Label1");
                id = Convert.ToInt32(lblID.Text);

                Label lblDataProgramacao = (Label)gdvVeiculos.Rows[index].FindControl("Label2");
                DateTime dataProgramacao = Convert.ToDateTime(lblDataProgramacao.Text);

                TextBox txtNumVeiculo = (TextBox)gdvVeiculos.Rows[index].FindControl("txtNumVeiculo");
                if (ValidaValorVazioTabelaVeiculos(txtNumVeiculo.Text, "Nº CARGA")) return;
                DropDownList ddlTransportadoras = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlTransportadoras");
                if (ValidaValorVazioTabelaVeiculos(ddlTransportadoras.SelectedValue, "TRANSPORTADORA")) return;
                DropDownList ddlPlacas = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlPlacas");
                if (ValidaValorVazioTabelaVeiculos(ddlPlacas.SelectedValue, "PLACA")) return;
                DropDownList ddlMotorista01 = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlMotorista01");
                if (ValidaValorVazioTabelaVeiculos(ddlMotorista01.SelectedValue, "MOTORISTA 01")) return;
                DropDownList ddlMotorista02 = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlMotorista02");
                if (ddlMotorista01.SelectedValue == ddlMotorista02.SelectedValue)
                {
                    lblMensagem4.Visible = true;
                    lblMensagem4.Text = "O motorista 01 e 02 não podem ser o mesmo!";
                }
                else
                {
                    lblMensagem4.Visible = false;
                    lblMensagem4.Text = "";
                }
                //TextBox PlacaTextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("PlacaTextBox");
                //TextBox Motorista01TextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("Motorista01TextBox");
                //TextBox Motorista02TextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("Motorista02TextBox");
                TextBox txtInicioCarregEsperado = (TextBox)gdvVeiculos.Rows[index].FindControl("txtInicioCarregEsperado");
                TextBox txtHorarioEntregaNF = (TextBox)gdvVeiculos.Rows[index].FindControl("txtHorarioEntregaNF");
                //TextBox TransportadoraTextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("TransportadoraTextBox");

                //var entCod = ddlTransportadoras.SelectedValue;
                //ENTIDADE1 entidade1Apolo = apolo.ENTIDADE1
                //    .Where(w => apolo.ENTIDADE.Any(a => w.EntCod == a.EntCod
                //            && a.EntNomeFant == ddlTransportadoras.SelectedValue)
                //        && w.USERParticipaProgTranspWEB == "Sim"
                //        && w.USEREmpresaControlaTransp == ddlEmpresaTransportadora.SelectedValue).FirstOrDefault();

                //if (entidade1Apolo != null)
                //{
                //    entCod = entidade1Apolo.EntCod;
                //}

                TextBox ValorKMTextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("ValorKMTextBox");
                TextBox txtDataEmbarque = (TextBox)gdvVeiculos.Rows[index].FindControl("txtDataEmbarque");
                DropDownList ddlAeroportoOrigem = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlAeroportoOrigem");
                TextBox txtHorarioChegadaAeroporto = (TextBox)gdvVeiculos.Rows[index].FindControl("txtHorarioChegadaAeroporto");
                DropDownList ddlDespachante = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlDespachante");
                TextBox txtDataInicioVazio = (TextBox)gdvVeiculos.Rows[index].FindControl("txtDataInicioVazio");
                TextBox OdometroVeiculoDataEmbarqueTextBox = (TextBox)gdvVeiculos.Rows[index].FindControl("OdometroVeiculoDataEmbarqueTextBox");

                #endregion

                #region Atualiza Veiculo

                Prog_Diaria_Transp_Veiculos veiculo = hlbapp.Prog_Diaria_Transp_Veiculos
                    .Where(w => w.ID == id).FirstOrDefault();

                //veiculo.DataProgramacao = dataProgramacao;
                veiculo.NumVeiculo = Convert.ToInt32(txtNumVeiculo.Text);
                string placaAntiga = veiculo.Placa;
                veiculo.Tranportadora = ddlTransportadoras.SelectedItem.Text;
                veiculo.EntCod = ddlTransportadoras.SelectedValue;
                veiculo.Placa = ddlPlacas.SelectedItem.Text;
                veiculo.EquipCodEstrVeiculo = ddlPlacas.SelectedValue;
                veiculo.Motorista01 = ddlMotorista01.SelectedItem.Text;
                veiculo.EntCodMotorista01 = ddlMotorista01.SelectedValue;
                if (ddlMotorista02.SelectedValue != "")
                {
                    veiculo.Motorista02 = ddlMotorista02.SelectedItem.Text;
                    veiculo.EntCodMotorista02 = ddlMotorista02.SelectedValue;
                }
                else
                {
                    veiculo.Motorista02 = "";
                    veiculo.EntCodMotorista02 = null;
                }
                //veiculo.Placa = PlacaTextBox.Text;
                //veiculo.Motorista01 = Motorista01TextBox.Text;
                //veiculo.Motorista02 = Motorista02TextBox.Text;
                //veiculo.QuantidadeTotal = 0;
                //veiculo.QuantidadePorCaixa = 0;
                //veiculo.QunatidadeCaixa = 0;
                //veiculo.ValorTotal = 0;
                veiculo.EmpresaTranportador = ddlEmpresaTransportadora.SelectedValue;
                veiculo.InicioCarregamentoEsperado = txtInicioCarregEsperado.Text;
                veiculo.HorarioEntregaNF = txtHorarioEntregaNF.Text;
                //veiculo.Tranportadora = TransportadoraTextBox.Text;
                veiculo.DataEmbarque = Convert.ToDateTime(txtDataEmbarque.Text);
                if (ValorKMTextBox.Text != "")
                    veiculo.ValorKM = Convert.ToDecimal(ValorKMTextBox.Text);
                else
                    veiculo.ValorKM = null;
                if (OdometroVeiculoDataEmbarqueTextBox.Text != "")
                    veiculo.OdometroVeiculoDataEmbarque = Convert.ToDecimal(OdometroVeiculoDataEmbarqueTextBox.Text);
                else
                    veiculo.OdometroVeiculoDataEmbarque = null;

                if (ddlAeroportoOrigem != null) veiculo.AeroportoOrigem = ddlAeroportoOrigem.SelectedValue;
                if (txtHorarioChegadaAeroporto != null) veiculo.HorarioChegadaAeroporto = txtHorarioChegadaAeroporto.Text;
                if (ddlDespachante != null) veiculo.Despachante = ddlDespachante.SelectedValue;
                if (txtDataInicioVazio != null)
                {
                    DateTime dataVazio = new DateTime();
                    if (DateTime.TryParse(txtDataInicioVazio.Text, out dataVazio)) veiculo.DataInicioVazio = dataVazio;
                }

                hlbapp.SaveChanges();

                #endregion

                #region Integra com o Embarcador

                #region Atualiza Veículo

                //if (veiculo.Placa != "" && placaAntiga != veiculo.Placa)
                //{
                //    string retornoEmbarcador = AtualizaVeiculoCargaEmbarcador(veiculo.ID, placaAntiga, veiculo.Placa);
                //    if (retornoEmbarcador != "")
                //    {
                //        lblMensagem2.Visible = true;
                //        lblMensagem2.Text = retornoEmbarcador;
                //        return;
                //    }
                //    else
                //    {
                //        lblMensagem2.Visible = false;
                //        lblMensagem2.Text = "";
                //    }
                //}

                #endregion

                #endregion

                gdvVeiculos.Rows[index].RowState = DataControlRowState.Normal;
                gdvVeiculos.EditIndex = -1;
                gdvVeiculos.SelectedIndex = index;
                gdvVeiculos.DataBind();
            }
            catch (Exception ex)
            {
                gdvVeiculos.Rows[index].RowState = DataControlRowState.Normal;
                lblMensagem3.Visible = true;

                if (ex.Message.Length >= 35)
                {
                    if (ex.Message.Substring(0, 35) == "ORA-20102: Cannot update records!!!")
                    {
                        lblMensagem3.Text = "Erro na linha " + (id).ToString() + ": " + "Não existe esse Lote nesta Data de Produção no Estoque informada! Verifique!";
                    }
                    else
                    {
                        lblMensagem3.Text = "Erro na linha " + (id).ToString() + ": " + ex.Message;
                    }
                }
            }
        }

        protected void ddlTransportadoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gdvVeiculos.EditIndex;
            DropDownList ddlTransportadoras = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlTransportadoras");
            DropDownList ddlPlacas = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlPlacas");
            DropDownList ddlMotorista01 = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlMotorista01");
            DropDownList ddlMotorista02 = (DropDownList)gdvVeiculos.Rows[index].FindControl("ddlMotorista02");
            
            CarregaPlacasEMotoristasDaTransportadora(ddlTransportadoras.SelectedValue, ddlPlacas, ddlMotorista01, ddlMotorista02, 0);
        }

        protected void ddlTransportadoras_Footer_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlTransportadoras = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlTransportadoras");
            DropDownList ddlPlacas = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlPlacas");
            DropDownList ddlMotorista01 = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlMotorista01");
            DropDownList ddlMotorista02 = (DropDownList)gdvVeiculos.FooterRow.FindControl("ddlMotorista02");
            
            CarregaPlacasEMotoristasDaTransportadora(ddlTransportadoras.SelectedValue, ddlPlacas, ddlMotorista01, ddlMotorista02, 0);
        }

        public void CarregaPlacasEMotoristasDaTransportadora(string transportadora, DropDownList ddlPlacas, DropDownList ddlMotorista01,
            DropDownList ddlMotorista02, int idPDTV)
        {
            FinanceiroEntities apoloSession = new FinanceiroEntities();
            HLBAPPEntities1 hlbappSession = new HLBAPPEntities1();

            var carga = hlbappSession.Prog_Diaria_Transp_Veiculos.Where(w => w.ID == idPDTV).FirstOrDefault();

            ddlPlacas.Items.Clear();
            ddlMotorista01.Items.Clear();
            ddlMotorista02.Items.Clear();

            if (transportadora != "")
            {
                ddlPlacas.Items.Add(new ListItem { Text = "(Não existe veículo cadastrado!)", Value = "", Selected = true });
                ddlMotorista01.Items.Add(new ListItem { Text = "(Não existe motorista cadastrado!)", Value = "", Selected = true });
                ddlMotorista02.Items.Add(new ListItem { Text = "(Não existe motorista cadastrado!)", Value = "", Selected = true });
            }

            ENTIDADE1 entidade1Apolo = apoloSession.ENTIDADE1
                .Where(w => apoloSession.ENTIDADE.Any(a => w.EntCod == a.EntCod
                        && a.EntCod == transportadora)
                    //&& w.USERParticipaProgTranspWEB == "Sim"
                    //&& w.USEREmpresaControlaTransp == ddlEmpresaTransportadora.SelectedValue
                    )
                .FirstOrDefault();

            if (entidade1Apolo != null)
            {
                #region Carrega Placa (ANTIGO)

                //TextBox PlacaTextBox = (TextBox)gdvVeiculos.FooterRow.FindControl("PlacaTextBox");
                //PlacaTextBox.Text = entidade1Apolo.USERPlacaVeiculo;

                #endregion

                #region Carrega Lista de Placas (NOVO)

                var listaVeiculos = apoloSession.EQUIPAMENTO.Where(w => w.EquipVeicPropEntCod == entidade1Apolo.EntCod).ToList();

                if (listaVeiculos.Count > 0)
                {
                    ddlPlacas.Items.Clear();
                    bool selected = true;
                    if (carga != null) selected = false;
                    ddlPlacas.Items.Add(new ListItem { Text = "(Selecione uma Placa)", Value = "", Selected = selected });
                }

                foreach (var veiculo in listaVeiculos)
                {
                    bool selected = false;
                    if (carga != null) if (carga.EquipCodEstrVeiculo == veiculo.EquipCodEstr) selected = true;

                    ListItem item = new ListItem();
                    item.Text = veiculo.EquipVeicPlaca.Replace(" ","").Trim();
                    item.Value = veiculo.EquipCodEstr;
                    item.Selected = selected;
                    ddlPlacas.Items.Add(item);
                }

                #endregion

                #region Carrega Lista de Motoristas

                var listaMotoristas = apoloSession.ENT_CONTATO.Where(w => w.EntCod == entidade1Apolo.EntCod).ToList();

                if (listaMotoristas.Count > 0)
                {
                    bool selected = true;
                    if (carga != null) selected = false;
                    ddlMotorista01.Items.Clear();
                    ddlMotorista01.Items.Add(new ListItem { Text = "(Selecione um Motorista)", Value = "", Selected = selected });

                    if (listaMotoristas.Count > 1)
                    {
                        selected = true;
                        if (carga != null) if (carga.EntCodMotorista02 != null) selected = false;
                        ddlMotorista02.Items.Clear();
                        ddlMotorista02.Items.Add(new ListItem { Text = "(Selecione um Motorista)", Value = "", Selected = selected });
                    }
                }

                foreach (var motorista in listaMotoristas)
                {
                    var motoristaApolo = apoloSession.ENTIDADE.Where(w => w.EntCod == motorista.EntCodContato).FirstOrDefault();

                    bool selected = false;
                    if (carga != null) if (carga.EntCodMotorista01 == motoristaApolo.EntCod) selected = true;

                    ListItem item01 = new ListItem();
                    item01.Text = motoristaApolo.EntNome;
                    item01.Value = motoristaApolo.EntCod;
                    item01.Selected = selected;
                    ddlMotorista01.Items.Add(item01);

                    if (listaMotoristas.Count > 1)
                    {
                        selected = false;
                        if (carga != null) if (carga.EntCodMotorista02 == motoristaApolo.EntCod) selected = true;

                        ListItem item02 = new ListItem();
                        item02.Text = motoristaApolo.EntNome;
                        item02.Value = motoristaApolo.EntCod;
                        item02.Selected = selected;
                        ddlMotorista02.Items.Add(item02);
                    }
                }

                #endregion
            }
        }

        #endregion

        #endregion

        #region Other Methods

        /// <summary>
        /// Método que traz o nome do parâmetro do campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="memberAccess"></param>
        /// <returns></returns>
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }

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

        public void ValidaDireitos()
        {
            #region Valida Atualização de Pedidos do CHIC p/ WEB - 21/03/2021 (DESATIVADO DEVIDO A MIGRAÇÃO DE SISTEMA (CHIC P/ ANIPLAN)

            //if (MvcAppHyLinedoBrasil.Controllers.AccountController
            //    .GetGroup("HLBAPP-ProgDiariaTranspAtualizaPedidosWEB",
            //    (System.Collections.ArrayList)Session["Direitos"]))
            //{
            //    imgbRefresh.Visible = true;
            //    lblAtualizarDados.Visible = true;
            //}
            //else
            //{
            //    imgbRefresh.Visible = false;
            //    lblAtualizarDados.Visible = false;
            //}

            imgbRefresh.Visible = false;
            lblAtualizarDados.Visible = false;

            #endregion

            #region Valida Atualização de Informações de Transporte do WEB p/ CHIC - 21/03/2021 (DESATIVADO DEVIDO A MIGRAÇÃO DE SISTEMA (CHIC P/ ANIPLAN)

            //if (MvcAppHyLinedoBrasil.Controllers.AccountController
            //    .GetGroup("HLBAPP-ProgDiariaTranspAtualizaCHIC",
            //    (System.Collections.ArrayList)Session["Direitos"]))
            //{
            //    imgbRefreshCHIC.Visible = true;
            //    lblRefreshCHIC.Visible = true;
            //}
            //else
            //{
            //    imgbRefreshCHIC.Visible = false;
            //    lblRefreshCHIC.Visible = false;
            //}

            imgbRefreshCHIC.Visible = false;
            lblRefreshCHIC.Visible = false;

            #endregion

            #region Valida Visualização da Lista de Veículos

            if (MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetGroup("HLBAPP-ProgDiariaTranspVisualizaListVeiculos",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                ListView1.Visible = true;
            }
            else
            {
                ListView1.Visible = false;
            }

            #endregion

            #region Valida Opção de Relatório em EXCEL

            if (MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetGroup("HLBAPP-ProgDiariaTranspGeraRelatorioEXCEL",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                btnGerarExcel.Visible = true;
            }
            else
            {
                btnGerarExcel.Visible = false;
            }
            #endregion

            #region Valida Opção Copiar Dados Manuais

            if (MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetGroup("HLBAPP-ProgDiariaTranspCopiaDados",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                imgbCopiaDadosManuais.Visible = true;
                lblCopiaDadosManuais.Visible = true;
                txtDataProximoDia.Visible = true;
            }
            else
            {
                imgbCopiaDadosManuais.Visible = false;
                lblCopiaDadosManuais.Visible = false;
                txtDataProximoDia.Visible = false;
            }
            #endregion

            #region Valida Opção Alterar Dados

            if (MvcAppHyLinedoBrasil.Controllers.AccountController
                .GetGroup("HLBAPP-ProgDiariaTranspAlterarDadosPedidos",
                (System.Collections.ArrayList)Session["Direitos"]))
            {
                imgbAdd.Visible = true;
                lblAdd.Visible = true;
                imgbAddVeiculo.Visible = true;
                lblAddVeiculo.Visible = true;
            }
            else
            {
                imgbAdd.Visible = false;
                lblAdd.Visible = false;
                imgbAddVeiculo.Visible = false;
                lblAddVeiculo.Visible = false;
            }
            #endregion
        }

        public void ValidaExibicaoTransportadora()
        {
            #region Valida Visualização da Tabela de Veículos

            if (ddlEmpresaTransportadora.SelectedValue.Equals("TR"))
            {
                gdvVeiculos.Visible = false;
                ListView1.Visible = true;
                lblAddVeiculo.Visible = false;
                imgbAddVeiculo.Visible = false;
            }
            else
            {
                gdvVeiculos.Visible = true;
                ListView1.Visible = false;
                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-ProgDiariaTranspAlterarDadosPedidos",
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    lblAddVeiculo.Visible = true;
                    imgbAddVeiculo.Visible = true;
                }
            }

            #endregion
        }

        public List<Models.HLBAPP.Prog_Diaria_Transp_Veiculos> ExibeValoresVeiculos(DateTime data)
        {
            #region Atualiza Veículos da Programação Diária de Transporte

            string empresa = Session["empresa"].ToString();

            var listaPedidos = hlbapp.Prog_Diaria_Transp_Pedidos
                .Where(w => w.DataProgramacao == data
                    && empresa.IndexOf(w.Empresa) != 1).ToList();

            List<Prog_Diaria_Transp_Veiculos> listaVeiculos = new List<Prog_Diaria_Transp_Veiculos>();

            for (int i = 0; i <= 10; i++)
            {
                Prog_Diaria_Transp_Veiculos progVeiculo = listaVeiculos
                    .Where(w => w.DataProgramacao == data && w.NumVeiculo == i).FirstOrDefault();

                bool existe = true;

                if (progVeiculo == null)
                {
                    existe = false;
                    progVeiculo = new Prog_Diaria_Transp_Veiculos();
                }

                progVeiculo.DataProgramacao = data;
                progVeiculo.NumVeiculo = i;
                progVeiculo.QuantidadeTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.Quantidade);
                if (progVeiculo.QuantidadePorCaixa != null)
                {
                    if (progVeiculo.QuantidadePorCaixa > 0)
                    {
                        decimal qtdCaixaDecimal = Convert.ToDecimal(progVeiculo.QuantidadeTotal) /
                            Convert.ToDecimal(progVeiculo.QuantidadePorCaixa);
                        int qtdCaixaInt = Convert.ToInt32(progVeiculo.QuantidadeTotal) /
                            Convert.ToInt32(progVeiculo.QuantidadePorCaixa);

                        if ((qtdCaixaDecimal - qtdCaixaInt) > 0)
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt + 1;
                        else
                            progVeiculo.QunatidadeCaixa = qtdCaixaInt;
                    }
                }
                else
                    progVeiculo.QuantidadePorCaixa = 100;

                progVeiculo.EmpresaTranportador = "TR";

                //progVeiculo.ValorTotal = listaPedidos.Where(w => w.NumVeiculo == i).Sum(s => s.ValorTotal);

                if (!existe) listaVeiculos.Add(progVeiculo);
            }

            #endregion

            return listaVeiculos;
        }

        public static void EnviarEmail(string paraNome, string paraEmail, string copiaPara, string assunto,
            string corpoEmail, int idOrigem, string anexos)
        {
            #region Envia E-mail

            ImportaCHICService.Data.WORKFLOW_EMAIL email = new ImportaCHICService.Data.WORKFLOW_EMAIL();

            ImportaCHICService.Data.ApoloServiceEntities apoloService = new ImportaCHICService.Data.ApoloServiceEntities();

            ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

            apoloService.GerarCodigo("1", "WORKFLOW_EMAIL", numero);

            email.WorkFlowEmailSeq = Convert.ToInt32(numero.Value);
            email.WorkFlowEmailStat = "Enviar";
            email.WorkFlowEmailAssunto = assunto;
            email.WorkFlowEmailData = DateTime.Now;
            email.WorkFlowEmailParaNome = paraNome;
            email.WorkFlowEmailParaEmail = paraEmail;
            email.WorkFlowEmailDeNome = "Serviço de Integração";
            email.WorkFLowEmailDeEmail = "sistemas@hyline.com.br";
            email.WorkFlowEmailFormato = "Html";
            email.WorkFlowEmailCopiaPara = copiaPara;
            email.WorkFlowEmailDocEspec = "";
            email.WorkFlowEmailDocNum = idOrigem.ToString();
            email.WorkFlowEmailDocSerie = "";

            email.WorkFlowEmailCorpo = corpoEmail;
            email.WorkFlowEmailArquivosAnexos = anexos;

            apoloService.WORKFLOW_EMAIL.AddObject(email);
            apoloService.SaveChanges();

            #endregion
        }

        #endregion

        #region Report Methods

        public void ModoImpressao()
        {
            GridView1.AllowSorting = false;
            GridView1.DataBind();
            ListView1.DataBind();

            Label16.Visible = false;
            txtDataProgramacao.Visible = false;
            imgbRefresh.Visible = false;
            lblAtualizarDados.Visible = false;
            imgbRefreshCHIC.Visible = false;
            lblRefreshCHIC.Visible = false;
            imgbAdd.Visible = false;
            lblAdd.Visible = false;
            imgLogo.Visible = false;
            lblMensagem2.Visible = false;
            UpdateProgress1.Visible = false;
            btnModoImpressao.Visible = false;
            btnModoCompleto.Visible = false;
            btnGerarExcel.Visible = false;
            lkbDownload.Visible = true;
        }

        public void ModoCompleto()
        {
            GridView1.AllowSorting = true;
            GridView1.DataBind();
            ListView1.DataBind();

            Label16.Visible = true;
            txtDataProgramacao.Visible = true;
            imgbRefresh.Visible = true;
            lblAtualizarDados.Visible = true;
            imgbRefreshCHIC.Visible = true;
            lblRefreshCHIC.Visible = true;
            imgbAdd.Visible = true;
            lblAdd.Visible = true;
            imgLogo.Visible = true;
            lblMensagem2.Visible = false;
            UpdateProgress1.Visible = true;
            //btnModoImpressao.Visible = true;
            btnModoCompleto.Visible = false;
            btnGerarExcel.Visible = true;
            lkbDownload.Visible = false;
        }

        protected void btnModoImpressao_Click(object sender, EventArgs e)
        {
            Session["modoImpressao"] = true;
            Response.Redirect("~/WebForms/ProgDiarioTransp.aspx","_blank","");
        }

        protected void btnModoCompleto_Click(object sender, EventArgs e)
        {
            Session["modoImpressao"] = false;
        }

        protected void lkbDownload_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelExcel"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Prog_Diaria_Transp_" + 
                Convert.ToDateTime(txtDataProgramacao.Text).ToString("yyyy-MM-dd") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }

        protected void btnGerarExcel_Click(object sender, EventArgs e)
        {
            string destino = "";
            string pesquisa = "";
            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";

            destino = "C:\\inetpub\\wwwroot\\Relatorios\\Prog_Diario_Trasporte_" + Session["login"].ToString() + Session.SessionID + ".xlsx";
            Session["destinoRelExcel"] = destino;
            pesquisa = "*Prog_Diario_Trasporte_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string[] files = Directory.GetFiles(pasta, pesquisa);

            foreach (var item in files)
            {
                System.IO.File.Delete(item);
            }

            #region Excel Transema

            if (ddlEmpresaTransportadora.SelectedValue.Equals("TR")
                || ddlEmpresaTransportadora.SelectedValue.Equals("HN"))
            {
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Prog_Diario_Trasporte.xlsx", destino);

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

                // Parâmetros
                string dataStrSQLServer = Convert.ToDateTime(txtDataProgramacao.Text).ToString("yyyy-MM-dd");
                string empresaTransportadora = ddlEmpresaTransportadora.SelectedValue;

                //Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Prog Diária Transp - Resumido"];

                Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

                foreach (Excel.WorkbookConnection item in lista)
                {
                    item.OLEDBConnection.BackgroundQuery = false;

                    string commandTextCHICCabecalho = "";
                    string commandTextCHICTabelas = "";
                    string commandTextCHICCondicaoJoins = "";
                    string commandTextCHICCondicaoFiltros = "";
                    string commandTextCHICCondicaoParametros = "";
                    string commandTextCHICAgrupamento = "";
                    string commandTextCHICOrdenacao = "";

                    if (item.Name.Equals("Pedidos"))
                    {
                        commandTextCHICCabecalho =
                            "select * ";

                        commandTextCHICTabelas =
                            "from " +
                                "VU_Prog_Diaria_Transp_Pedidos_Excel ";

                        commandTextCHICCondicaoJoins = "";

                        commandTextCHICCondicaoFiltros = "where ";

                        commandTextCHICCondicaoParametros =
                                "DataProgramacaoFiltro = '" + dataStrSQLServer + "' and " +
                                "EmpresaTranportador = '" + empresaTransportadora + "' ";

                        commandTextCHICAgrupamento = "";

                        commandTextCHICOrdenacao = "order by 2, 1";

                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
                    else if (item.Name.Equals("Veiculos"))
                    {
                        commandTextCHICCabecalho =
                            "select * ";

                        commandTextCHICTabelas =
                            "from " +
                                "VU_Prog_Diaria_Transp_Veiculos_Excel ";

                        commandTextCHICCondicaoJoins = "";

                        commandTextCHICCondicaoFiltros = "where ";

                        commandTextCHICCondicaoParametros =
                                "DataProgramacaoFiltro = '" + dataStrSQLServer + "' and " +
                                "EmpresaTranportador = '" + empresaTransportadora + "' ";

                        commandTextCHICAgrupamento = "";

                        commandTextCHICOrdenacao = "order by 1";

                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
                    else if (item.Name.Equals("Pedidos1"))
                    {
                        commandTextCHICCabecalho =
                            "select * ";

                        commandTextCHICTabelas =
                            "from " +
                                "VU_Prog_Diaria_Transp_Pedidos_Completo_Excel ";

                        commandTextCHICCondicaoJoins = "";

                        commandTextCHICCondicaoFiltros = "where ";

                        commandTextCHICCondicaoParametros =
                                "DataProgramacaoFiltro = '" + dataStrSQLServer + "' and " +
                                "EmpresaTranportador = '" + empresaTransportadora + "' ";

                        commandTextCHICAgrupamento = "";

                        commandTextCHICOrdenacao = "order by 2, 1";

                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
                    else if (item.Name.Equals("Veiculos1"))
                    {
                        commandTextCHICCabecalho =
                            "select * ";

                        commandTextCHICTabelas =
                            "from " +
                                "VU_Prog_Diaria_Transp_Veiculos_Excel ";

                        commandTextCHICCondicaoJoins = "";

                        commandTextCHICCondicaoFiltros = "where ";

                        commandTextCHICCondicaoParametros =
                                "DataProgramacaoFiltro = '" + dataStrSQLServer + "' and " +
                                "EmpresaTranportador = '" + empresaTransportadora + "' ";

                        commandTextCHICAgrupamento = "";

                        commandTextCHICOrdenacao = "order by 1";

                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
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
            }

            #endregion

            #region Excel Planalto

            if (ddlEmpresaTransportadora.SelectedValue.Equals("PL"))
            {
                System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Prog_Diario_Trasporte_PL.xlsx", destino);

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

                // Parâmetros
                string dataStrSQLServer = Convert.ToDateTime(txtDataProgramacao.Text).ToString("yyyy-MM-dd");
                string empresaTransportadora = ddlEmpresaTransportadora.SelectedValue;

                Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Programação de Faturamento"];
                worksheet.Cells[2, 6] = Convert.ToDateTime(txtDataProgramacao.Text);

                Microsoft.Office.Interop.Excel.Connections lista = oBook.Connections;

                foreach (Excel.WorkbookConnection item in lista)
                {
                    item.OLEDBConnection.BackgroundQuery = false;

                    string commandTextCHICCabecalho = "";
                    string commandTextCHICTabelas = "";
                    string commandTextCHICCondicaoJoins = "";
                    string commandTextCHICCondicaoFiltros = "";
                    string commandTextCHICCondicaoParametros = "";
                    string commandTextCHICAgrupamento = "";
                    string commandTextCHICOrdenacao = "";

                    if (item.Name.Equals("Pedidos"))
                    {
                        commandTextCHICCabecalho =
                            "select * ";

                        commandTextCHICTabelas =
                            "from " +
                                "VU_Prog_Diario_Transporte_PL ";

                        commandTextCHICCondicaoJoins = "";

                        commandTextCHICCondicaoFiltros = "where ";

                        commandTextCHICCondicaoParametros =
                                "DataProgramacao = '" + dataStrSQLServer + "' and " +
                                "EmpresaTranportador = '" + empresaTransportadora + "' ";

                        commandTextCHICAgrupamento = "";

                        commandTextCHICOrdenacao = "order by 3, 9";

                        item.OLEDBConnection.CommandText =
                            commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                            commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                            commandTextCHICOrdenacao;
                    }
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
            }

            #endregion

            lkbDownload.Visible = true;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lkbDownload);

        }

        #endregion

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
                System.Web.UI.Page page = (System.Web.UI.Page)HttpContext.Current.Handler;

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
                ScriptManager.RegisterStartupScript(page, typeof(System.Web.UI.Page), "Redirect", script, true);
            }
        }
    }
}