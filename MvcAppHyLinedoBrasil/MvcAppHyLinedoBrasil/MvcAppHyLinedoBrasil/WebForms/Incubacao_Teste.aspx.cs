using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
//using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using ImportaIncubacao.Data.Apolo;
using AjaxControlToolkit;
using System.Data.Objects;
using MvcAppHyLinedoBrasil.Controllers;
using System.Globalization;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class Incubacao_Teste : System.Web.UI.Page
    {
        #region Objects

        HLBAPPEntities bdSQLServer = new HLBAPPEntities();

        FLIPDataSet flipDataSet = new FLIPDataSet();

        SETDAY_DATATableAdapter setDayData = new SETDAY_DATATableAdapter();
        HATCHERY_FLOCK_DATATableAdapter hatcheryFlockData = new HATCHERY_FLOCK_DATATableAdapter();
        HATCHERY_EGG_DATATableAdapter hatcheryEggData = new HATCHERY_EGG_DATATableAdapter();
        FLOCK_DATATableAdapter flockData = new FLOCK_DATATableAdapter();
        EGGINV_DATATableAdapter eggInvData = new EGGINV_DATATableAdapter();

        ImportaIncubacao.Data.FLIPDataSetTableAdapters.EGGINV_DATATableAdapter eggInvDataServico = new ImportaIncubacao.Data.FLIPDataSetTableAdapters.EGGINV_DATATableAdapter();
        ImportaIncubacao.Data.FLIPDataSetTableAdapters.FLOCKSTableAdapter flocksServico = new ImportaIncubacao.Data.FLIPDataSetTableAdapters.FLOCKSTableAdapter();
        ImportaIncubacao.Data.FLIPDataSet flipDataSetServico = new ImportaIncubacao.Data.FLIPDataSet();

        FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();
        HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();

        Apolo10EntitiesService bdApolo = new Apolo10EntitiesService();
        ImportaIncubacao.ImportaIncubacaoService service = new ImportaIncubacao.ImportaIncubacaoService();

        //public static string linhagem;
        //public static int age;
        //public static int qtde;
        //public static DateTime dataNascimentoLote;

        //public static string tipoCadastro;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

            if (IsPostBack == false)
            {
                #region Importação Inicial Colombia

                //DateTime dataIni = Convert.ToDateTime("26/07/2018");
                //DateTime dataFim = Convert.ToDateTime("05/12/2019");
                //while (dataIni <= dataFim)
                //{
                //    RefreshFLIP("MA", dataIni);
                //    RefreshFLIP("MN", dataIni);
                //    RefreshFLIP("MQ", dataIni);
                //    RefreshFLIP("PM", dataIni);
                //    dataIni = dataIni.AddDays(1);
                //}

                #endregion

                //DateTime dataIni = Convert.ToDateTime("20/09/2018");
                //RefreshFLIP("MA", dataIni);
                //dataIni = Convert.ToDateTime("30/08/2019");
                //RefreshFLIP("MQ", dataIni);
                //dataIni = Convert.ToDateTime("06/09/2019");
                //RefreshFLIP("MQ", dataIni);

                #region Load Page Components

                ChangeLanguage();

                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                //if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoHyline", (System.Collections.ArrayList)Session["Direitos"]))
                //    Image2.ImageUrl = "../Content/images/Logo_BR.png";
                //else if (MvcAppHyLinedoBrasil.Controllers.AccountController.GetGroup("HLBAPP-AcessoLohmann", (System.Collections.ArrayList)Session["Direitos"]))
                //    Image2.ImageUrl = "../Content/images/Logo_LB.png";
                //else
                //    Image2.ImageUrl = "../Content/images/Logo_HN.png";

                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-IncubacaoEstoqueFuturo", (System.Collections.ArrayList)Session["Direitos"]))
                {
                    btnEstoqueFuturo.Visible = true;
                    lblFiltroTipoEstoque.Visible = true;
                    ddlTipoEstoque.Visible = true;
                }
                else
                {
                    btnEstoqueFuturo.Visible = false;
                    lblFiltroTipoEstoque.Visible = false;
                    ddlTipoEstoque.Visible = false;
                }

                AjustaTelaIncubacaoManualPlanalto();

                bdApolo.CommandTimeout = 100000;
                bdSQLServer.CommandTimeout = 100000;

                Calendar1.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Session["setDate"] = Calendar1.SelectedDate;

                Session["linhagem"] = "";
                Session["age"] = "0";
                Session["qtde"] = "0";
                Session["dataNascimentoLote"] = "";
                Session["tipoCadastro"] = "";

                //DateTime data = Convert.ToDateTime("09/07/2013");
                DateTime data = Calendar1.SelectedDate;

                #endregion

                #region Load Hatcheries

                ddlIncubatorios.Items.Clear();

                HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
                hTA.Fill(flipDataSet.HATCHERY_CODES);

                foreach (var item in flipDataSet.HATCHERY_CODES)
                {
                    if (MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        ddlIncubatorios.Items.Add(new ListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                    }
                }

                #endregion

                #region Load Eggs Type

                if (GetCompanyAndRegionByHatchLoc(ddlIncubatorios.SelectedValue, "CLAS_EGG") != "NO")
                {
                    Label6.Visible = false;
                    txt_SetterDe.Visible = false;
                    Label7.Visible = false;
                    txt_SetterPara.Visible = false;
                    btn_AtualizaSetter.Visible = false;
                    ddlClassOvos.Visible = true;

                    ddlClassOvos.Items.Clear();
                    ddlClassOvos.Items.Add(new ListItem { Text = "(Todos)", Value = "T", Selected = true });

                    Models.HLBAPP.HLBAPPEntities1 hlbapp = new Models.HLBAPP.HLBAPPEntities1();
                    var listaTipoOvos = hlbapp.TIPO_CLASSFICACAO_OVO
                        .Where(w => w.Unidade == ddlIncubatorios.SelectedValue && w.AproveitamentoOvo == "Incubável").ToList();

                    foreach (var item in listaTipoOvos)
                    {
                        ddlClassOvos.Items.Add(new ListItem { Text = item.DescricaoTipo, Value = item.CodigoTipo, Selected = false });
                    }
                }
                else
                {
                    Label6.Visible = true;
                    txt_SetterDe.Visible = true;
                    Label7.Visible = true;
                    txt_SetterPara.Visible = true;
                    btn_AtualizaSetter.Visible = true;
                    ddlClassOvos.Visible = false;
                }

                #endregion

                #region Refresh Data

                //AtualizaFLIP(data);
                RefreshFLIP(ddlIncubatorios.SelectedValue, data, false);
                AtualizaNascimentoWEB(ddlIncubatorios.SelectedValue, data);
                //VerificaImportacaoApolo(data);
                AtualizaTotais();

                #endregion

                #region Refresh Estimate FLIP

                //DateTime dataIni = Convert.ToDateTime("01/01/2020");
                //DateTime dataFim = Convert.ToDateTime("03/07/2020");
                //while (dataIni <= dataFim)
                //{
                //    UpdateEstimateFLIP("HYBR", "BR", "TB", dataIni);
                //    dataIni = dataIni.AddDays(1);
                //}

                #endregion

                string hatchLoc = ddlIncubatorios.SelectedValue;
                string clasInc = GetCompanyAndRegionByHatchLoc(hatchLoc, "CLAS_INC");
                if (clasInc == "YES")
                    pnlTabelaOvosClassificados.Visible = true;
                else
                    pnlTabelaOvosClassificados.Visible = false;
            }

            #region Load Components Values

            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }
            if (TextBox1.Text.Equals(""))
            {
                TextBox1.Text = "0";
            }

            #endregion

            Session["hatchLocal"] = ddlIncubatorios.SelectedValue;

            #region Refresh Not Imported

            VerificaNaoImportados();

            #endregion

            DateTime dataTeste = Convert.ToDateTime("11/05/2020");
            var media = CalculaMediaEstimadaPonderadaEclosao("CH", dataTeste, "HLP10-P108133HB");
        }

        public void AjustaTelaIncubacaoManualPlanalto()
        {
            /*
            * 13/01/2015 - Solicitado por Davi Nogueira
            * 
            * Não realizar controle de estoque no Incubatório da Planalto (NM) até 31/12/2016,
            * pois ocorrem muitos erros de lançamentos de DEOs por parte da Granja
            * e não estão conseguindo ajustar.
            * */
            if (ddlIncubatorios.SelectedValue == "NM"
                && Calendar1.SelectedDate <= Convert.ToDateTime("31/12/2016"))
            {
                btnEstoqueFuturo.Visible = true;
                btnEstoqueFuturo.Text = "Inserir Incubação Manual";
                GridView3.Visible = false;
                Label3.Visible = false;
                Label8.Visible = false;
                TextBox6.Visible = false;
                DropDownList1.Visible = false;
                ddlClassOvos.Visible = false;
                btn_Pesquisar.Visible = false;
            }
            else
            {
                if (MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetGroup("HLBAPP-IncubacaoEstoqueFuturo", (System.Collections.ArrayList)Session["Direitos"]))
                    btnEstoqueFuturo.Visible = true;
                else
                    btnEstoqueFuturo.Visible = false;
                btnEstoqueFuturo.Text = "Inserir Estoque Futuro";
                GridView3.Visible = true;
                Label3.Visible = true;
                Label8.Visible = true;
                TextBox6.Visible = true;
                DropDownList1.Visible = true;
                ddlClassOvos.Visible = true;
                btn_Pesquisar.Visible = true;
            }
        }

        public void AtualizaTotais()
        {
            decimal qtdeOvosIncubados = 0;
            string maquinasUtilizadas = "";
            DateTime dataIncubacao = Calendar1.SelectedDate;
            string incubatorio = ddlIncubatorios.SelectedValue;

            lblQtdeOvosIncubados.Text = "";
            lblQtdeOvosIncubadosCx.Text = "";

            qtdeOvosIncubados = Convert.ToDecimal(bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Set_date == dataIncubacao && h.Hatch_loc == incubatorio)
                .Sum(h => h.Eggs_rcvd));

            decimal qtdeBandejasIncubadas = Convert.ToDecimal(bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Set_date == dataIncubacao && h.Hatch_loc == incubatorio)
                .Sum(h => h.Bandejas));

            var lista = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Set_date == dataIncubacao && h.Hatch_loc == incubatorio)
                    .GroupBy(h => new
                    {
                        h.Machine
                    })
                    .Select(h => new //HATCHERY_EGG_DATA
                    {
                        type = h.Key
                    })
                    .ToList();

            foreach (var item in lista)
            {
                maquinasUtilizadas = maquinasUtilizadas + " / " + item.type.Machine;
            }

            if (qtdeOvosIncubados > 0)
            {
                decimal bandejas = qtdeBandejasIncubadas;
                lblQtdeOvosIncubados.Text = string.Format("{0:N0}", qtdeOvosIncubados) + " "
                    + Translate("ovos");
                lblQtdeOvosIncubadosCx.Text = string.Format("{0:N0}", Decimal.Round(bandejas, 0)) + " "
                    + Translate("bandejas");
            }

            lblMaquinas.Text = maquinasUtilizadas;
        }

        protected void lbEsconderInv_Click(object sender, EventArgs e)
        {
            lbMostrarInv.Visible = true;
            lbEsconderInv.Visible = false;
            Label3.Visible = false;
            Label8.Visible = false;
            TextBox6.Visible = false;
            DropDownList1.Visible = false;
            btn_Pesquisar.Visible = false;
            GridView3.Visible = false;
            //LinkButton1.Visible = false;
            FormView1.Visible = false;
            lblMensagem.Visible = false;
        }

        protected void lbMostrarInv_Click(object sender, EventArgs e)
        {
            lbEsconderInv.Visible = true;
            lbMostrarInv.Visible = false;
            Label3.Visible = true;
            Label8.Visible = true;
            TextBox6.Visible = true;
            DropDownList1.Visible = true;
            btn_Pesquisar.Visible = true;
            GridView3.Visible = true;
            //LinkButton1.Visible = true;
            FormView1.Visible = true;
            lblMensagem.Visible = true;
        }

        #endregion

        #region Egg Inventory Table - GridView3

        // Search in Egg Invetory Table
        protected void Button1_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;
            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }
            GridView3.DataBind();
        }

        // Select flock and lay date to setting
        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Destivar inserir incubação a partir de 15/12/2021 devido a migração para o Poultry Suite

            if (Calendar1.SelectedDate >= Convert.ToDateTime("15/12/2021") 
                && (ddlIncubatorios.SelectedValue == "CH" || ddlIncubatorios.SelectedValue == "NM"))
            {
                lblMensagem3.Visible = true;
                lblMensagem3.Text = "NÃO É POSSÍVEL INSERIR INCUBAÇÃO A PARTIR DO DIA 15/12/2021 DEVIDO A MIGRAÇÃO PARA O POULTRY SUITE!!!";
                return;
            }

            #endregion

            #region Load Parameters Form if real egg inv

            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;
            HatchFormDataSource.SelectParameters["FLOCK_ID"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            //HatchFormDataSource.SelectParameters["TRACK_NO"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;
            HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[8].Text;
            Session["linhagem"] = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text.Replace("amp;","");

            string farmid = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            string flockid = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            DateTime layDate = Convert.ToDateTime(GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text);

            string incubatorio = ddlIncubatorios.SelectedValue;
            string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
            string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
            string location = GetLocation(company, incubatorio);
            GetNumLote(company, region, location, farmid, flockid, layDate);
            Session["qtde"] = Convert.ToInt32(GridView3.Rows[GridView3.SelectedIndex].Cells[7].Text.Replace(".",""));
            lblMensagem.Visible = false;
            Session["tipoCadastro"] = "Estoque Real";
            FormView1.ChangeMode(FormViewMode.Edit);

            #endregion
        }

        #endregion

        #region Setting Flock and Lay Date Form

        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                #region Translate Labels

                #region Egg Setting Fields

                Label lblFARM_ID = (Label)FormView1.FindControl("lblFARM_ID");
                lblFARM_ID.Text = Translate(lblFARM_ID.Text);
                Label lblFLOCK_ID = (Label)FormView1.FindControl("lblFLOCK_ID");
                lblFLOCK_ID.Text = Translate(lblFLOCK_ID.Text);
                Label lblLAY_DATE = (Label)FormView1.FindControl("lblLAY_DATE");
                lblLAY_DATE.Text = Translate(lblLAY_DATE.Text);
                Label lblSETTER = (Label)FormView1.FindControl("lblSETTER");
                lblSETTER.Text = Translate(lblSETTER.Text);
                Label lblEGGS_UNITS = (Label)FormView1.FindControl("lblEGGS_UNITS");
                lblEGGS_UNITS.Text = Translate(lblEGGS_UNITS.Text);
                Label lblMediaEclosao = (Label)FormView1.FindControl("lblMediaEclosao");
                lblMediaEclosao.Text = Translate(lblMediaEclosao.Text);
                Label lblHorario = (Label)FormView1.FindControl("lblHorario");
                lblHorario.Text = Translate(lblHorario.Text);
                Label lblPosicao = (Label)FormView1.FindControl("lblPosicao");
                lblPosicao.Text = Translate(lblPosicao.Text);
                Label lblBandejas = (Label)FormView1.FindControl("lblBandejas");
                lblBandejas.Text = Translate(lblBandejas.Text);
                Label lblObservacao = (Label)FormView1.FindControl("lblObservacao");
                lblObservacao.Text = Translate(lblObservacao.Text);

                #endregion

                #region Egg Sorting Fields

                Label lblClasOvos = (Label)FormView1.FindControl("lblClasOvos");
                lblClasOvos.Text = Translate(lblClasOvos.Text);
                Label lblOvosTrincados = (Label)FormView1.FindControl("lblOvosTrincados");
                lblOvosTrincados.Text = Translate(lblOvosTrincados.Text);
                Label lblOvosSujos = (Label)FormView1.FindControl("lblOvosSujos");
                lblOvosSujos.Text = Translate(lblOvosSujos.Text);
                Label lblOvosGrandes = (Label)FormView1.FindControl("lblOvosGrandes");
                lblOvosGrandes.Text = Translate(lblOvosGrandes.Text);
                Label lblOvosPequenos = (Label)FormView1.FindControl("lblOvosPequenos");
                lblOvosPequenos.Text = Translate(lblOvosPequenos.Text);
                Label lblOvosQuebrados = (Label)FormView1.FindControl("lblOvosQuebrados");
                lblOvosQuebrados.Text = Translate(lblOvosQuebrados.Text);
                Label lblOvosParaComercio = (Label)FormView1.FindControl("lblOvosParaComercio");
                lblOvosParaComercio.Text = Translate(lblOvosParaComercio.Text);

                #endregion

                LinkButton UpdateButton = (LinkButton)FormView1.FindControl("UpdateButton");
                UpdateButton.Text = Translate(UpdateButton.Text);

                #endregion

                #region Verify if have config Sorting Eggs in Setting Eggs

                string hatchLoc = ddlIncubatorios.SelectedValue;
                Panel pnlClasOvos = (Panel)FormView1.FindControl("pnlClasOvos");
                string clasInc = GetCompanyAndRegionByHatchLoc(hatchLoc, "CLAS_INC");
                if (clasInc == "YES")
                    pnlClasOvos.Visible = true;
                else
                    pnlClasOvos.Visible = false;

                #endregion

                #region Verify if have config Insert production date by period in Setting Eggs

                Panel pnlLayDateByPeriod = (Panel)FormView1.FindControl("pnlLayDateByPeriod");
                Panel pnlLayDateUnique = (Panel)FormView1.FindControl("pnlLayDateUnique");
                string periodLayDateInc = GetCompanyAndRegionByHatchLoc(hatchLoc, "PERIOD_LAY_DATE_INC");
                if (periodLayDateInc == "YES")
                {
                    pnlLayDateByPeriod.Visible = true;
                    pnlLayDateUnique.Visible = false;
                }
                else
                {
                    pnlLayDateByPeriod.Visible = false;
                    pnlLayDateUnique.Visible = true;
                }

                #endregion

                #region FARM_IDLabel1

                Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                if (FARM_IDLabel1 != null)
                {
                    if (Session["tipoCadastro"].ToString() == "Estoque Futuro")
                    {
                        FARM_IDLabel1.Visible = false;
                        DropDownList DropDownListFarms = (DropDownList)FormView1.FindControl("DropDownList4");
                        if (DropDownListFarms != null)
                        {
                            DropDownListFarms.Visible = true;
                        }
                    }
                    else
                    {
                        FARM_IDLabel1.Visible = true;
                        DropDownList DropDownListFarms = (DropDownList)FormView1.FindControl("DropDownList4");
                        if (DropDownListFarms != null)
                        {
                            DropDownListFarms.Visible = false;
                        }
                    }
                }

                #endregion

                #region FLOCK_IDLabel1

                Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                if (FLOCK_IDLabel1 != null)
                {
                    if (Session["tipoCadastro"].ToString() == "Estoque Futuro")
                    {
                        FLOCK_IDLabel1.Visible = false;
                        DropDownList DropDownListFlocks = (DropDownList)FormView1.FindControl("DropDownList3");
                        if (DropDownListFlocks != null)
                        {
                            DropDownListFlocks.Visible = true;
                        }
                    }
                    else
                    {
                        FLOCK_IDLabel1.Visible = true;
                        DropDownList DropDownListFlocks = (DropDownList)FormView1.FindControl("DropDownList3");
                        if (DropDownListFlocks != null)
                        {
                            DropDownListFlocks.Visible = false;
                        }
                    }
                }

                #endregion

                #region TRACK_NOLabel1

                //Label TRACK_NOLabel1 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                //if (TRACK_NOLabel1 != null)
                //{
                //    if (tipoCadastro == "Estoque Futuro")
                //    {
                //        TRACK_NOLabel1.Visible = false;
                //    }
                //    else
                //    {
                //        TRACK_NOLabel1.Visible = true;
                //    }
                //}

                #endregion

                #region DAT_Label1

                Label DAT_Label1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                if (DAT_Label1 != null)
                {
                    if (Session["tipoCadastro"].ToString() == "Estoque Futuro")
                    {
                        DateTime date = Convert.ToDateTime(DAT_Label1.Text);
                        DAT_Label1.Visible = false;
                    }
                    else
                    {
                        DAT_Label1.Visible = true;
                        DateTime date = Convert.ToDateTime(DAT_Label1.Text);
                        DAT_Label1.Text = string.Format("{0:d}", date);
                    }
                }

                #endregion

                #region Calendario

                System.Web.UI.WebControls.Calendar Calendario = 
                    (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");
                if (Calendario != null)
                {
                    if (Session["tipoCadastro"].ToString() == "Estoque Futuro")
                    {
                        Calendario.Visible = true;
                        Calendario.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                        //Label TRACK_NOLabel2 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                        //if (TRACK_NOLabel2 != null)
                        //{
                        //    TRACK_NOLabel2.Visible = true;
                        //    TRACK_NOLabel2.Text = "EXP" + Calendario.SelectedDate.ToString("yyMMdd");
                        //}
                    }
                    else
                    {
                        Calendario.Visible = false;
                    }
                }

                #endregion

                TextBox MachineTextBox = (TextBox)FormView1.FindControl("MachineTextBox");

                #region EGG_UNITSTextBox

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                if (EGG_UNITSTextBox != null)
                {
                    string machine = MachineTextBox.Text;

                    EQUIPAMENTO equip = bdApolo.EQUIPAMENTO.Where(w => w.EquipNome == machine).FirstOrDefault();
                    double qtdOvosPorBandeja = 150;
                    if (equip != null) qtdOvosPorBandeja = Convert.ToDouble(equip.EquipPotencia);

                    decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / qtdOvosPorBandeja), 1)).ToString();
                }

                #endregion

                #region Hatch_LocLabel

                Label Hatch_LocLabel = (Label)FormView1.FindControl("Hatch_LocLabel");
                if (Hatch_LocLabel != null)
                {
                    if (Session["tipoCadastro"].ToString() == "Estoque Futuro")
                    {
                        if (ddlIncubatorios.SelectedValue != "NM" ||
                            (ddlIncubatorios.SelectedValue == "NM"
                             && Calendar1.SelectedDate >= Convert.ToDateTime("01/01/2017")))
                        {
                            Hatch_LocLabel.Visible = true;
                            DropDownList ddlClasOvos = (DropDownList)FormView1.FindControl("ddlClasOvos");
                            if (ddlClasOvos != null)
                            {
                                ddlClasOvos.Visible = false;
                            }
                        }
                        else
                        {
                            Hatch_LocLabel.Visible = false;
                            DropDownList ddlClasOvos = (DropDownList)FormView1.FindControl("ddlClasOvos");
                            if (ddlClasOvos != null)
                            {
                                ddlClasOvos.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        Hatch_LocLabel.Visible = true;
                        DropDownList ddlClasOvos = (DropDownList)FormView1.FindControl("ddlClasOvos");
                        if (ddlClasOvos != null)
                        {
                            ddlClasOvos.Visible = false;
                        }
                    }
                }

                #endregion

                #region MachineTextBox

                if (MachineTextBox != null)
                {
                    MaskedEditExtender MaskedEditExtenderMachineTextBox = 
                        (MaskedEditExtender)FormView1.FindControl("MaskedEditExtender2");
                    if (ddlIncubatorios.SelectedValue.Equals("NM"))
                    {
                        MaskedEditExtenderMachineTextBox.Mask = "A99";
                        MaskedEditExtenderMachineTextBox.AutoCompleteValue = "";
                    }
                    else
                    {
                        MaskedEditExtenderMachineTextBox.Mask = "S-99";
                        MaskedEditExtenderMachineTextBox.AutoCompleteValue = "S-";
                    }
                }

                #endregion
            }

            #region COMENTADO

            //if (FormView1.CurrentMode == FormViewMode.Edit)
            //{
            //    DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
            //    DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
            //    TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");

            //    if ((lote != null) && (farm != null) && (EGG_UNITSTextBox != null))
            //    {

            //        DateTime ultimaProducao = Convert.ToDateTime(flockData.UltimaProducaoPorLote("HYBR",
            //                            "BR", "PP", farm.SelectedValue, lote.SelectedValue));

            //        int eggUnits = Convert.ToInt32(flockData.QtdeOvosIncubaveis("HYBR",
            //                            "BR", "PP", farm.SelectedValue, lote.SelectedValue,
            //                            ultimaProducao));

            //        qtde = eggUnits;


            //        EGG_UNITSTextBox.Text = eggUnits.ToString();

            //        if (qtde < eggUnits)
            //        {
            //            lblMensagem.Visible = true;
            //            lblMensagem.Text = "Quantidade maior que disponível! - (" + qtde.ToString() + ")";
            //        }
            //        else
            //        {
            //            lblMensagem.Visible = false;
            //            ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
            //        }

            //        DateTime ultimaIncubacao = Convert.ToDateTime(hatcheryFlockData.UltimaIncubacao("HYBR",
            //                            "BR", "PP", ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

            //        decimal ultimaPercEclosao = Convert.ToDecimal(hatcheryFlockData.UltimaPercEclosao("HYBR",
            //                            "BR", "PP", ultimaIncubacao, ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

            //        TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
            //        MediaEclosaoTextBox.Text = ultimaPercEclosao.ToString();
            //    }
            //}

            #endregion
        }

        // DESATIVADO
        protected void FormView1_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            //DateTime dataIncubacao = Calendar1.SelectedDate;

            //try
            //{
            //    if (FormView1.CurrentMode == FormViewMode.Edit)
            //    {
            //        string farmID = "";
            //        string flockID = "";
            //        //Label TRACK_NOLabel1 = (Label)FormView1.FindControl("TRACK_NOLabel1");
            //        //string trackNO = TRACK_NOLabel1.Text;
            //        DateTime layDate;

            //        TextBox MachineTextBox = (TextBox)FormView1.FindControl("MachineTextBox");
            //        string machine = MachineTextBox.Text;
            //        TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
            //        decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);
            //        TextBox HorarioTextBox = (TextBox)FormView1.FindControl("HorarioTextBox");
            //        string horario = HorarioTextBox.Text;
            //        TextBox PosicaoTextBox = (TextBox)FormView1.FindControl("PosicaoTextBox");
            //        decimal posicao = Convert.ToDecimal(PosicaoTextBox.Text);
            //        // 16/01/2014 - Visto por Sérica: 
            //        // Problemas de cáculo de bandejas no form. Fazendo o cálculo novamente quando salvar.
            //        //TextBox BandejasTextBox = (TextBox)FormView1.FindControl("BandejasTextBox");
            //        //decimal bandejas = Convert.ToDecimal(BandejasTextBox.Text);
            //        decimal bandejas = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(eggUnits) / 150.0), 1));
            //        TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
            //        decimal media = Convert.ToDecimal(MediaEclosaoTextBox.Text);
            //        TextBox ObservacaoTextBox = (TextBox)FormView1.FindControl("ObservacaoTextBox");
            //        string observacao = ObservacaoTextBox.Text;

            //        if (!service.ExisteFechamentoEstoque(dataIncubacao, ddlIncubatorios.SelectedValue))
            //        {
            //            string status = "";
            //            if (tipoCadastro == "Estoque Futuro")
            //            {
            //                DropDownList ddlFarm = (DropDownList)FormView1.FindControl("DropDownList4");
            //                farmID = ddlFarm.SelectedValue;
            //                DropDownList ddlLote = (DropDownList)FormView1.FindControl("DropDownList3");
            //                flockID = ddlLote.SelectedValue;
            //                Calendar clDataProducao = (Calendar)FormView1.FindControl("Lay_DateCalendar");
            //                layDate = clDataProducao.SelectedDate;
            //            }
            //            else
            //            {
            //                Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
            //                farmID = FARM_IDLabel1.Text;
            //                Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
            //                flockID = FLOCK_IDLabel1.Text;
            //                Label LAY_DATELabel1 = (Label)FormView1.FindControl("LAY_DATELabel1");
            //                layDate = Convert.ToDateTime(LAY_DATELabel1.Text);
            //                status = "Importado";
            //            }

            //            string trackNO = "EXP" + layDate.ToString("yyMMdd");

            //            string numLote = "";
            //            flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", farmID, flockID);
            //            if (flipDataSet.FLOCKS.Count > 0)
            //            {
            //                dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
            //                age = ((layDate - dataNascimentoLote).Days) / 7;
            //                linhagem = flipDataSet.FLOCKS[0].VARIETY;
            //                numLote = flipDataSet.FLOCKS[0].NUM_1.ToString();
            //            }

            //            // Insere na base SQL Server
            //            var qtd = eggUnits;

            //            int posicaoFiltro = Convert.ToInt32(posicao);
            //            string flockIDFiltro = farmID + "-" + flockID;
            //            int existeSQL = bdSQLServer.HATCHERY_EGG_DATA
            //            .Where(h => h.Company == "HYBR" && h.Region == "BR" && h.Location == "PP" && h.Set_date == dataIncubacao
            //                && h.Hatch_loc == ddlIncubatorios.SelectedValue && h.Flock_id == flockIDFiltro && h.Lay_date == layDate && h.Machine == machine
            //                && h.Track_no == trackNO && h.Posicao == posicaoFiltro)
            //            .Count();

            //            if (existeSQL != 0)
            //            {
            //                //qtd = qtd + Convert.ToDecimal(hatcheryEggDataObjectDelete.Eggs_rcvd);
            //                //bdSQLServer.HATCHERY_EGG_DATA.DeleteObject(hatcheryEggDataObjectDelete);
            //                lblMensagem.Visible = true;
            //                lblMensagem.Text = "Lote " + flockID + " já incluso na posição " + posicao.ToString() + ". Verifique!";
            //            }
            //            else
            //            {
            //                lblMensagem.Visible = false;

            //                var hatcheryEggDataObject = new HATCHERY_EGG_DATA();

            //                hatcheryEggDataObject.Company = "HYBR";
            //                hatcheryEggDataObject.Region = "BR";
            //                hatcheryEggDataObject.Location = "PP";
            //                hatcheryEggDataObject.Set_date = dataIncubacao;
            //                hatcheryEggDataObject.Hatch_loc = ddlIncubatorios.SelectedValue;
            //                hatcheryEggDataObject.Flock_id = farmID + "-" + flockID;
            //                hatcheryEggDataObject.Lay_date = layDate;
            //                hatcheryEggDataObject.Eggs_rcvd = Convert.ToInt32(qtd);
            //                hatcheryEggDataObject.Egg_key = "";
            //                hatcheryEggDataObject.Machine = machine;
            //                hatcheryEggDataObject.Track_no = trackNO;
            //                hatcheryEggDataObject.Posicao = Convert.ToInt32(posicao);
            //                hatcheryEggDataObject.Bandejas = Convert.ToInt32(bandejas);
            //                hatcheryEggDataObject.Horario = horario;
            //                hatcheryEggDataObject.Estimate = media;
            //                hatcheryEggDataObject.Variety = linhagem.Replace("amp;", "");
            //                hatcheryEggDataObject.Age = age;
            //                hatcheryEggDataObject.Observacao = observacao;
            //                hatcheryEggDataObject.Status = status;
            //                hatcheryEggDataObject.Usuario = Session["usuario"].ToString();
            //                hatcheryEggDataObject.Egg_key = numLote;

            //                bdSQLServer.HATCHERY_EGG_DATA.AddObject(hatcheryEggDataObject);

            //                if (status == "Importado")
            //                {
            //                    // Insere na tabela da Data de Incubação
            //                    decimal existe = Convert.ToDecimal(setDayData.ExisteSetDayData(dataIncubacao, ddlIncubatorios.SelectedValue));

            //                    if (existe == 0)
            //                    {
            //                        decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(ddlIncubatorios.SelectedValue)) + 1;

            //                        setDayData.InsertQuery("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, sequencia);
            //                    }

            //                    existe = 0;

            //                    // Insere / Atualiza Incubação
            //                    existe = Convert.ToDecimal(hatcheryEggData.ExisteHatcheryEggDataAll("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO));

            //                    if (existe > 0)
            //                    {
            //                        eggUnits = eggUnits + Convert.ToDecimal(hatcheryEggData.QtdOvos("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO));
            //                        hatcheryEggData.Delete("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO);
            //                    }

            //                    existe = 0;

            //                    // Verifica se existe Dados do Nascimento
            //                    existe = Convert.ToDecimal(hatcheryFlockData.ExisteHatcheryFlockData("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID));

            //                    if (existe == 0)
            //                    {
            //                        hatcheryFlockData.InsertQuery("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, media);
            //                    }
            //                    // 14/08/2014 - Ocorrência 99 - APONTES
            //                    // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            //                    // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            //                    // o trigger de atualização da idade executar.
            //                    else
            //                    {
            //                        hatcheryFlockData.UpdateEstimate(media, "HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID);
            //                    }

            //                    if (eggUnits > 0)
            //                    {
            //                        hatcheryEggData.Insert("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, eggUnits, "", machine, trackNO,
            //                            null, null, null, null, null, null, null, null, observacao, Session["login"].ToString());
            //                    }

            //                    #region Importa p/ Apolo

            //                    string incubatorio = ddlIncubatorios.SelectedValue;
            //                    string naturezaOperacao = "1.556.001";
            //                    decimal? valorUnitario = 0.25m;
            //                    string unidadeMedida = "UN";
            //                    short? posicaoUnidadeMedida = 1;
            //                    string tribCod = "040";
            //                    string itMovEstqClasFiscCodNbm = "04079000";
            //                    string clasFiscCod = "0000129";
            //                    string operacao = "Saída";

            //                    ITEM_MOV_ESTQ itemMovEstq;

            //                    string usuario;
            //                    if (Session["login"].ToString().Equals("palves"))
            //                        usuario = "RIOSOFT";
            //                    else
            //                        usuario = Session["login"].ToString().ToUpper();

            //                    EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
            //                        .Where(ef => ef.USERFLIPCod == incubatorio)
            //                        .FirstOrDefault();

            //                    LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
            //                        .Where(l => l.USERCodigoFLIP == incubatorio)
            //                        .FirstOrDefault();

            //                    PRODUTO produto = bdApolo.PRODUTO
            //                        .Where(p => p.ProdNomeAlt1 == linhagem)
            //                        .FirstOrDefault();

            //                    // Verifica se Existe a movimentação neste Incubatório e Produto
            //                    LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                        .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                            && i.ProdCodEstr == produto.ProdCodEstr
            //                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                                && m.MovEstqDataMovimento == dataIncubacao))
            //                        .FirstOrDefault();

            //                    if (locItemMovEstq != null)
            //                    {
            //                        itemMovEstq = bdApolo.ITEM_MOV_ESTQ
            //                            .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
            //                                && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
            //                            .FirstOrDefault();

            //                        itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
            //                        itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

            //                        locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + qtd;
            //                        locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

            //                        CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //                            .Where(c => c.EmpCod == locItemMovEstq.EmpCod && c.MovEstqChv == locItemMovEstq.MovEstqChv
            //                                && c.ProdCodEstr == locItemMovEstq.ProdCodEstr && c.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq
            //                                && c.LocArmazCodEstr == locItemMovEstq.LocArmazCodEstr && c.CtrlLoteNum == flockID
            //                                && c.CtrlLoteDataValid == layDate)
            //                            .FirstOrDefault();

            //                        // Verifica se Existe o lote
            //                        if (lote != null)
            //                        {
            //                            lote.CtrlLoteItMovEstqQtd = lote.CtrlLoteItMovEstqQtd + qtd;
            //                            lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;
            //                        }
            //                        else
            //                        {
            //                            lote = service.InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
            //                                locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flockID, layDate, qtd, operacao,
            //                                itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

            //                            bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // Verifica se Existe a movimentação neste Incubatório e não no Produto
            //                        locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                            .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                                && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                                    && m.MovEstqDataMovimento == dataIncubacao))
            //                            .FirstOrDefault();

            //                        if (locItemMovEstq != null)
            //                        {
            //                            MOV_ESTQ movEstq = bdApolo.MOV_ESTQ
            //                                .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
            //                                .FirstOrDefault();

            //                            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                                movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                                linhagem, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                                tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                            bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                            LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
            //                                service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                                itemMovEstq.ProdCodEstr, qtd, locItemMovEstq.LocArmazCodEstr);

            //                            bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                            CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                                itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
            //                                layDate, qtd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                                locArmazItemMovEstq.LocArmazCodEstr);

            //                            bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                        }
            //                        else
            //                        {
            //                            MOV_ESTQ movEstq = service.InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
            //                                dataIncubacao, usuario);

            //                            bdApolo.MOV_ESTQ.AddObject(movEstq);

            //                            itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                                movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                                linhagem, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                                tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                            bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                            LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
            //                                service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                                itemMovEstq.ProdCodEstr, qtd, locArmaz.LocArmazCodEstr);

            //                            bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                            CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                                itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
            //                                layDate, qtd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                                locArmazItemMovEstq.LocArmazCodEstr);

            //                            bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                        }
            //                    }

            //                    bdApolo.SaveChanges();

            //                    bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

            //                    bdApolo.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr,
            //                        itemMovEstq.ItMovEstqSeq, itemMovEstq.ItMovEstqDataMovimento, "INS");

            //                    #endregion
            //                }

            //                bdSQLServer.SaveChanges();

            //                //GridView1.DataSourceID = null;
            //                //GridView1.DataSource = HatchGridDataSource;
            //                //GridView1.DataSourceID = "HatchGridDataSource";
            //                GridView1.DataBind();

            //                //GridView3.DataSourceID = null;
            //                //GridView3.DataSource = EggInvDataSource;
            //                //GridView3.DataSourceID = "EggInvDataSource";
            //                GridView3.DataBind();

            //                gvMaquinas.DataBind();
            //                gvLotes.DataBind();
            //                gvLinhagens.DataBind();

            //                //object objeto = Calendar1;
            //                //EventArgs e2 = new EventArgs();
            //                //Calendar1_SelectionChanged(objeto, e2);

            //                AtualizaTotais();

            //                //FormView1.ChangeMode(FormViewMode.ReadOnly);
            //            }
            //        }
            //        else
            //        {
            //            lblMensagem.Visible = true;
            //            lblMensagem.Text = "Estoque já fechado! Verifique com o Depto. Contábil sobre a possibilidade da abertura!"
            //                + "Caso não seja aberto, a conferência não pode ser realizada!";
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    lblMensagem.Visible = true;
            //    lblMensagem.Text = "Erro ao Incubar: " + ex.Message;
            //}
        }

        protected void EGG_UNITSTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                #region Load Variables

                string incubatorio = ddlIncubatorios.SelectedValue;
                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string location = GetLocation(company, incubatorio);

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                if (EGG_UNITSTextBox.Text == "") { EGG_UNITSTextBox.Text = "0"; }
                decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);

                TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");

                // Verifica se a quantidade equivale a produzida e a já incubada.
                decimal qtdeProduzida = 0;
                int? qtdeIncubada = 0;

                string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");

                TextBox MachineTextBox = (TextBox)FormView1.FindControl("MachineTextBox");
                string machine = MachineTextBox.Text;

                EQUIPAMENTO equip = bdApolo.EQUIPAMENTO.Where(w => w.EquipNome == machine).FirstOrDefault();
                double qtdOvosPorBandeja = 150;
                if (equip != null) qtdOvosPorBandeja = Convert.ToDouble(equip.EquipPotencia);

                #endregion

                string periodLayDateInc = GetCompanyAndRegionByHatchLoc(incubatorio, "PERIOD_LAY_DATE_INC");

                if (periodLayDateInc != "YES")
                {
                    if (Session["tipoCadastro"].ToString().Equals("Estoque Futuro"))
                    {
                        #region Load Variables if future Egg Inv

                        DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                        DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                        System.Web.UI.WebControls.Calendar clDataProducao =
                            (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");

                        if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                        {
                            clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        }

                        qtdeProduzida = Convert.ToDecimal(Session["qtde"]);

                        string loteCompleto = farm.SelectedValue + "-" + lote.SelectedValue;
                        DateTime dataProducao = clDataProducao.SelectedDate;

                        qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company.Equals("HYBR") && h.Region.Equals("BR") && h.Location.Equals(location) &&
                                        h.Flock_id.Equals(loteCompleto) && h.Lay_date.Equals(dataProducao))
                            .Sum(h => h.Eggs_rcvd);

                        if (qtdeIncubada == null)
                            qtdeIncubada = 0;

                        #endregion
                    }
                    else
                    {
                        #region Load Variables if real Egg Inv

                        string farmID = "";
                        string flockID = "";
                        DateTime layDate;

                        Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                        farmID = FARM_IDLabel1.Text;
                        Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                        flockID = FLOCK_IDLabel1.Text;
                        Label LAY_DATELabel1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                        layDate = Convert.ToDateTime(LAY_DATELabel1.Text + " 00:00:00");

                        //string retornoField = GetFlockDataValueByField(company, region, location, farmID, flockID, layDate, "NUM_1");
                        //qtdeProduzida = retornoField != "" ? Convert.ToInt32(retornoField) : Convert.ToInt32(Session["qtde"]);
                        qtdeProduzida = GetQtyHatchingEggsProduced(company, region, location, farmID, flockID, layDate);

                        string loteCompleto = farmID + "-" + flockID;

                        qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company.Equals("HYBR") && h.Region.Equals("BR") && h.Location.Equals(location) &&
                                        h.Flock_id.Equals(loteCompleto) && h.Lay_date.Equals(layDate))
                            .Sum(h => h.Eggs_rcvd);

                        if (qtdeIncubada == null)
                            qtdeIncubada = 0;

                        #endregion
                    }

                    if (Convert.ToInt32(Session["qtde"]) < eggUnits)
                    {
                        #region Show warning message if Larger quantity available

                        lblMensagem.Visible = true;
                        lblMensagem.Text = Translate("Quantidade maior que disponível") + "! - (" + Session["qtde"].ToString() + ")";
                        ((TextBox)FormView1.FindControl("BandejasTextBox")).Text =
                            (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / qtdOvosPorBandeja), 1))
                            .ToString();
                        EGG_UNITSTextBox.Focus();

                        #endregion
                    }
                    else if ((qtdeProduzida - qtdeIncubada) < eggUnits)
                    {
                        #region Show warning message if Larger quantity available in Production Date

                        lblMensagem.Visible = true;
                        lblMensagem.Text = Translate("Quantidade maior que disponível do Lote nesta Data de Produção! - (Qtde. Produzida: ")
                            + qtdeProduzida.ToString() + " / " + Translate("Qtde. já Incubada: ") + qtdeIncubada.ToString()
                            + "). " + Translate("Verifique o Estoque do Apolo com o FLIP e a qtd. incubada!");
                        ((TextBox)FormView1.FindControl("BandejasTextBox")).Text =
                            (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / qtdOvosPorBandeja), 1))
                            .ToString();
                        EGG_UNITSTextBox.Focus();

                        #endregion
                    }
                    else
                    {
                        #region Update Tray Value

                        lblMensagem.Visible = false;
                        ((TextBox)FormView1.FindControl("BandejasTextBox")).Text =
                            (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / qtdOvosPorBandeja), 1))
                            .ToString();
                        MediaEclosaoTextBox.Focus();

                        #endregion
                    }
                }
            }
        }

        protected void btnEstoqueFuturo_Click(object sender, EventArgs e)
        {
            try
            {
                #region Load Parameters Form if future egg inv

                string incubatorio = ddlIncubatorios.SelectedValue;

                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string location = GetLocation(company, incubatorio);

                lblMensagem.Visible = false;
                HatchFormDataSource.SelectParameters["FLOCK_ID"].DefaultValue = GridView3.Rows[0].Cells[2].Text;
                //HatchFormDataSource.SelectParameters["TRACK_NO"].DefaultValue = GridView3.Rows[0].Cells[4].Text;
                HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = GridView3.Rows[0].Cells[5].Text;
                HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = GridView3.Rows[0].Cells[8].Text;
                Session["linhagem"] = GridView3.Rows[0].Cells[4].Text;

                string farmid = GridView3.Rows[0].Cells[1].Text;
                string flockid = GridView3.Rows[0].Cells[2].Text;
                DateTime layDate = Convert.ToDateTime(GridView3.Rows[0].Cells[5].Text);

                string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
                GetNumLote(company, region, location, farmid, flockid, layDate);

                Session["qtde"] = Convert.ToInt32(GridView3.Rows[0].Cells[7].Text.Replace(".", ""));

                lblMensagem.Visible = false;
                Session["tipoCadastro"] = "Estoque Futuro";
                FormView1.ChangeMode(FormViewMode.Edit);

                lblMensagem2.Visible = false;
                lblMensagemOvosClass.Visible = false;

                //AtualizaIdadesLinhagens();

                #endregion
            }
            catch (Exception ex)
            {
                lblMensagem.Visible = true;
                lblMensagem.Text = "ERRO: " + ex.Message;
            }
        }

        protected void Lay_DateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            #region Load Flock Data in Session Variables

            string incubatorio = ddlIncubatorios.SelectedValue;

            string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
            string location = GetLocation(company, incubatorio);
            string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");

            System.Web.UI.WebControls.Calendar Calendario = 
                (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");
            if (Calendario != null)
            {
                //Label TRACK_NOLabel2 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                //if (TRACK_NOLabel2 != null)
                //{
                //    TRACK_NOLabel2.Visible = true;
                //    TRACK_NOLabel2.Text = "EXP" + Calendario.SelectedDate.ToString("yyMMdd");
                //}

                //age = ((Calendario.SelectedDate - dataNascimentoLote).Days) / 7;

                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                GetNumLote(company, region, location, farm.SelectedValue, lote.SelectedValue, Calendario.SelectedDate);

                if (Session["tipoCadastro"].ToString().Equals("Estoque Futuro"))
                {
                    //string retornoField = GetFlockDataValueByField(company, region, location, farm.SelectedValue,
                    //    lote.SelectedValue, Calendario.SelectedDate, "NUM_1");
                    //Session["qtde"] = retornoField != "" ? Convert.ToInt32(retornoField) : Convert.ToInt32(Session["qtde"]);
                    Session["qtde"] = GetQtyHatchingEggsProduced(company, region, location, farm.SelectedValue, lote.SelectedValue, 
                        Calendario.SelectedDate);

                    flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", location, farm.SelectedValue, 
                        lote.SelectedValue, Calendario.SelectedDate);

                    //if (retornoField != "")
                    if (Convert.ToInt32(Session["qtde"]) > 0)
                    {
                        //Session["qtde"] = Convert.ToInt32(retornoField);
                        TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                        EGG_UNITSTextBox.Text = Session["qtde"].ToString();
                    }
                    else
                    {
                        DateTime ultimaProducao = Convert.ToDateTime(GetLastProductionDate(company,
                            region, location, farm.SelectedValue, lote.SelectedValue));

                        int eggUnits = Convert.ToInt32(GetQtyHatchingEggsProduced(company,
                            region, location, farm.SelectedValue, lote.SelectedValue, ultimaProducao));

                        Session["qtde"] = eggUnits;

                        TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                        EGG_UNITSTextBox.Text = eggUnits.ToString();
                    }
                }
            }
            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;

            #endregion
        }

        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((FormView1.CurrentMode == FormViewMode.Edit) && (Session["tipoCadastro"] == "Estoque Futuro"))
            {
                #region Load Flock Data if future egg inv

                string incubatorio = ddlIncubatorios.SelectedValue;
                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
                string location = GetLocation(company, incubatorio);

                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                System.Web.UI.WebControls.Calendar clDataProducao = 
                    (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");

                if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                {
                    clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }

                DateTime ultimaProducao = Convert.ToDateTime(GetLastProductionDate(company,
                    region, location, farm.SelectedValue, lote.SelectedValue));

                int eggUnits = Convert.ToInt32(GetQtyHatchingEggsProduced(company,
                    region, location, farm.SelectedValue, lote.SelectedValue, ultimaProducao));

                Session["qtde"] = eggUnits;

                GetNumLote(company, region, location, farm.SelectedValue, lote.SelectedValue, clDataProducao.SelectedDate);

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                EGG_UNITSTextBox.Text = eggUnits.ToString();

                if (Convert.ToInt32(Session["qtde"]) < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = Translate("Quantidade maior que disponível") + "! - (" 
                        + Session["qtde"].ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else
                {
                    lblMensagem.Visible = false;
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = 
                        (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }

                DateTime ultimaIncubacao = GetLastSetDate(company, region, location, ddlIncubatorios.SelectedValue, 
                    farm.SelectedValue + "-" + lote.SelectedValue);

                decimal ultimaPercEclosao = GetLastEstimate(company, region, location, ultimaIncubacao, 
                    ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue);

                TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                MediaEclosaoTextBox.Text = ultimaPercEclosao.ToString();

                #endregion
            }
        }

        protected void DropDownList3_DataBound(object sender, EventArgs e)
        {
            if ((FormView1.CurrentMode == FormViewMode.Edit) && (Session["tipoCadastro"].ToString() == "Estoque Futuro"))
            {
                #region Load Flock Data if future egg inv

                string incubatorio = ddlIncubatorios.SelectedValue;
                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
                string location = GetLocation(company, incubatorio);

                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                System.Web.UI.WebControls.Calendar clDataProducao = 
                    (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");

                if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                {
                    clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }

                DateTime ultimaProducao = Convert.ToDateTime(GetLastProductionDate(company,
                    region, location, farm.SelectedValue, lote.SelectedValue));

                int eggUnits = Convert.ToInt32(GetQtyHatchingEggsProduced(company,
                    region, location, farm.SelectedValue, lote.SelectedValue, ultimaProducao));

                Session["qtde"] = eggUnits;

                GetNumLote(company, region, location, farm.SelectedValue, lote.SelectedValue, clDataProducao.SelectedDate);

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                EGG_UNITSTextBox.Text = eggUnits.ToString();

                if (Convert.ToInt32(Session["qtde"]) < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = Translate("Quantidade maior que disponível") + "! - ("
                        + Session["qtde"].ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else
                {
                    lblMensagem.Visible = false;
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text =
                        (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }

                DateTime ultimaIncubacao = GetLastSetDate(company, region, location, ddlIncubatorios.SelectedValue,
                    farm.SelectedValue + "-" + lote.SelectedValue);

                decimal ultimaPercEclosao = GetLastEstimate(company, region, location, ultimaIncubacao,
                    ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue);

                TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                MediaEclosaoTextBox.Text = ultimaPercEclosao.ToString();

                #endregion
            }
        }

        // Save Setting
        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            #region Load page components data

            DateTime dataIncubacao = Calendar1.SelectedDate;
            string incubatorio = ddlIncubatorios.SelectedValue;
            string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
            string location = GetLocation(company, incubatorio);
            string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
            string periodLayDateInc = GetCompanyAndRegionByHatchLoc(incubatorio, "PERIOD_LAY_DATE_INC");
            
            #endregion

            try
            {
                if (FormView1.CurrentMode == FormViewMode.Edit)
                {
                    #region Load form data

                    //bdApolo.CommandTimeout = 10000;

                    #region Setting Eggs Fields

                    string farmID = "";
                    string flockID = "";
                    string loteCompleto = "";
                    //Label TRACK_NOLabel1 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                    //string trackNO = TRACK_NOLabel1.Text;
                    DateTime layDate;
                    DateTime layDateFim;
                    string hatchLocEgg = "";

                    TextBox MachineTextBox = (TextBox)FormView1.FindControl("MachineTextBox");
                    string machine = MachineTextBox.Text;
                    TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                    decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);
                    TextBox HorarioTextBox = (TextBox)FormView1.FindControl("HorarioTextBox");
                    string horario = HorarioTextBox.Text;
                    TextBox PosicaoTextBox = (TextBox)FormView1.FindControl("PosicaoTextBox");
                    decimal posicao = Convert.ToDecimal(PosicaoTextBox.Text);
                    // 16/01/2014 - Visto por Sérica: 
                    // Problemas de cáculo de bandejas no form. Fazendo o cálculo novamente quando salvar.
                    TextBox BandejasTextBox = (TextBox)FormView1.FindControl("BandejasTextBox");
                    decimal bandejas = Convert.ToDecimal(BandejasTextBox.Text);
                    //decimal bandejas = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(eggUnits) / 150.0), 1));
                    TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                    decimal media = Convert.ToDecimal(MediaEclosaoTextBox.Text);
                    TextBox ObservacaoTextBox = (TextBox)FormView1.FindControl("ObservacaoTextBox");
                    string observacao = ObservacaoTextBox.Text;

                    decimal qtdeProduzida = 0;
                    int? qtdeIncubada = 0;
                    string clasOvosStr = "";
                    
                    string status = "";

                    #endregion

                    #region Sorting Eggs Fields

                    int crackedEggs = 0;
                    TextBox txtOvosTrincados = (TextBox)FormView1.FindControl("txtOvosTrincados");
                    if (txtOvosTrincados != null) if (txtOvosTrincados.Text != "") crackedEggs = Convert.ToInt32(txtOvosTrincados.Text);
                    int dirtyEggs = 0;
                    TextBox txtOvosSujos = (TextBox)FormView1.FindControl("txtOvosSujos");
                    if (txtOvosSujos != null) if (txtOvosSujos.Text != "") dirtyEggs = Convert.ToInt32(txtOvosSujos.Text);
                    int bigEggs = 0;
                    TextBox txtOvosGrandes = (TextBox)FormView1.FindControl("txtOvosGrandes");
                    if (txtOvosGrandes != null) if (txtOvosGrandes.Text != "") bigEggs = Convert.ToInt32(txtOvosGrandes.Text);
                    int smallEggs = 0;
                    TextBox txtOvosPequenos = (TextBox)FormView1.FindControl("txtOvosPequenos");
                    if (txtOvosPequenos != null) if (txtOvosPequenos.Text != "") smallEggs = Convert.ToInt32(txtOvosPequenos.Text);
                    int brokenEggs = 0;
                    TextBox txtOvosQuebrados = (TextBox)FormView1.FindControl("txtOvosQuebrados");
                    if (txtOvosQuebrados != null) if (txtOvosQuebrados.Text != "") brokenEggs = Convert.ToInt32(txtOvosQuebrados.Text);
                    int salesEggs = 0;
                    TextBox txtOvosParaComercio = (TextBox)FormView1.FindControl("txtOvosParaComercio");
                    if (txtOvosParaComercio != null) if (txtOvosParaComercio.Text != "") salesEggs = Convert.ToInt32(txtOvosParaComercio.Text);

                    #endregion

                    #endregion

                    // Verifica se a quantidade equivale a produzida e a já incubada.
                    if (Session["tipoCadastro"].ToString().Equals("Estoque Futuro"))
                    {
                        #region Load Production and Setted qty. if Future Inventory

                        DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                        farmID = farm.SelectedValue;
                        DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                        flockID = lote.SelectedValue;
                        System.Web.UI.WebControls.Calendar clDataProducao = 
                            (System.Web.UI.WebControls.Calendar)FormView1.FindControl("Lay_DateCalendar");

                        #region Hatch_LocLabel

                        if (ddlIncubatorios.SelectedValue != "NM" ||
                            (ddlIncubatorios.SelectedValue == "NM"
                                && Calendar1.SelectedDate >= Convert.ToDateTime("01/01/2017")))
                        {
                            Label Hatch_LocLabel = (Label)FormView1.FindControl("Hatch_LocLabel");
                            clasOvosStr = Hatch_LocLabel.Text;
                        }
                        else
                        {
                            DropDownList ddlClasOvos = (DropDownList)FormView1.FindControl("ddlClasOvos");
                            clasOvosStr = ddlClasOvos.SelectedValue;
                        }

                        #endregion

                        if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                            clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        layDate = clDataProducao.SelectedDate;
                        layDateFim = clDataProducao.SelectedDate;

                        qtdeProduzida = Convert.ToInt32(Session["qtde"]);

                        loteCompleto = farm.SelectedValue + "-" + lote.SelectedValue;

                        qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company.Equals(company) && h.Region.Equals(region) 
                                && h.Location.Equals(location) && h.Flock_id.Equals(loteCompleto) 
                                && h.Lay_date.Equals(layDate))
                            .Sum(h => h.Eggs_rcvd);

                        if (qtdeIncubada == null)
                            qtdeIncubada = 0;

                        if (incubatorio == "NM" && dataIncubacao < Convert.ToDateTime("01/01/2017"))
                        {
                            status = "Importado";
                        }
                        hatchLocEgg = clasOvosStr;

                        //qtde = Convert.ToInt32(qtdeProduzida);

                        /*
                         * 13/01/2015 - Solicitado por Davi Nogueira
                         * 
                         * Não realizar controle de estoque no Incubatório da Planalto (NM) até 31/12/2016,
                         * pois ocorrem muitos erros de lançamentos de DEOs por parte da Granja
                         * e não estão conseguindo ajustar.
                         * */
                        if (incubatorio != "NM" || (incubatorio == "NM" && dataIncubacao >= Convert.ToDateTime("01/01/2017")))
                        {
                            HLBAPPEntities hlbappVerificaEstoque = new HLBAPPEntities();

                            CTRL_LOTE_LOC_ARMAZ_WEB existeEstoque = hlbappVerificaEstoque.CTRL_LOTE_LOC_ARMAZ_WEB
                                .Where(w => w.Local == incubatorio && w.Nucleo == farm.SelectedValue
                                    && w.LoteCompleto == lote.SelectedValue
                                    && w.DataProducao == layDate)
                                .FirstOrDefault();

                            if (existeEstoque != null)
                            {
                                lblMensagem.Visible = true;
                                lblMensagem.Text = "Já existe o Estoque lançado! " +
                                    "Estoque Futuro é somente para Lotes não lançados!";
                                FormView1.ChangeMode(FormViewMode.ReadOnly);
                                return;
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        #region Load Load Production and Setted qty. if Real Inventory

                        Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                        farmID = FARM_IDLabel1.Text;
                        Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                        flockID = FLOCK_IDLabel1.Text;
                        if (periodLayDateInc == "YES")
                        {
                            System.Web.UI.WebControls.Calendar calLayDateIni =
                            (System.Web.UI.WebControls.Calendar)FormView1.FindControl("calLayDateIni");
                            if (calLayDateIni.SelectedDate.ToShortDateString() == "01/01/0001")
                                calLayDateIni.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                            layDate = calLayDateIni.SelectedDate;

                            System.Web.UI.WebControls.Calendar calLayDateFim =
                            (System.Web.UI.WebControls.Calendar)FormView1.FindControl("calLayDateFim");
                            if (calLayDateFim.SelectedDate.ToShortDateString() == "01/01/0001")
                                calLayDateFim.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                            layDateFim = calLayDateFim.SelectedDate;
                        }
                        else
                        {
                            Label LAY_DATELabel1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                            layDate = Convert.ToDateTime(LAY_DATELabel1.Text);
                            layDateFim = Convert.ToDateTime(LAY_DATELabel1.Text);
                        }

                        Label Hatch_Loc_Label1 = (Label)FormView1.FindControl("Hatch_LocLabel");
                        hatchLocEgg = Hatch_Loc_Label1.Text;
                        status = "Importado";

                        qtdeProduzida = GetQtyHatchingEggsProduced(company, region, location, farmID, flockID, layDate);

                        loteCompleto = farmID + "-" + flockID;

                        qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company.Equals("HYBR") && h.Region.Equals("BR") && h.Location.Equals(location) &&
                                        h.Flock_id.Equals(loteCompleto) && h.Lay_date.Equals(layDate) && h.ClassOvo == hatchLocEgg)
                            .Sum(h => h.Eggs_rcvd);

                        if (qtdeIncubada == null)
                            qtdeIncubada = 0;

                        #endregion
                    }

                    decimal totalQty = eggUnits + crackedEggs + dirtyEggs + bigEggs + smallEggs + brokenEggs + salesEggs;

                    if (periodLayDateInc != "YES")
                    {
                        #region Verify if exists egg balance and if qty. requested is bigger then qty. produced

                        /*
                    * 13/01/2015 - Solicitado por Davi Nogueira
                    * 
                    * Não realizar controle de estoque no Incubatório da Planalto (NM) até 31/12/2016,
                    * pois ocorrem muitos erros de lançamentos de DEOs por parte da Granja
                    * e não estão conseguindo ajustar.
                    * */
                        if (incubatorio != "NM" || (incubatorio == "NM" && dataIncubacao >= Convert.ToDateTime("01/01/2017")))
                        {
                            if (Convert.ToInt32(Session["qtde"]) < totalQty)
                            {
                                lblMensagem.Visible = true;
                                lblMensagem.Text = Translate("Quantidade maior que disponível")
                                    + "! - (" + Session["qtde"].ToString() + ")";
                                EGG_UNITSTextBox.Focus();
                                return;
                            }
                            else if ((qtdeProduzida - qtdeIncubada) < totalQty)
                            {
                                lblMensagem.Visible = true;
                                lblMensagem.Text = Translate("Quantidade maior que disponível do Lote nesta Data de Produção! - (Qtde. Produzida: ")
                                    + qtdeProduzida.ToString() + " / " + Translate("Qtde. já Incubada: ")
                                    + qtdeIncubada.ToString()
                                    + "). " + Translate("Verifique o Estoque com o FLIP e a qtd. incubada!");
                                EGG_UNITSTextBox.Focus();
                                return;
                            }
                        }
                        //else
                        //{
                        //    lblMensagem.Visible = false;
                        //    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                        //    MediaEclosaoTextBox.Focus();
                        //}

                        #endregion
                    }
                    else
                    {
                        #region Verify if exists egg balance

                        int disponibleQty = VerifyQtyByLayDatePeriod(flockID, layDate, layDateFim);

                        if (disponibleQty < totalQty)
                        {
                            Label lblMensagemEggUnits = (Label)FormView1.FindControl("lblMensagemEggUnits");
                            lblMensagemEggUnits.Visible = true;
                            lblMensagemEggUnits.Text = Translate("Quantidade maior que disponível")
                                + "! - (" + String.Format("{0:N0}",disponibleQty) + ")";
                            EGG_UNITSTextBox.Focus();
                            return;
                        }

                        #endregion
                    }

                    if (!ExisteFechamentoEstoque(company, incubatorio, dataIncubacao) && !ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
                    {
                        decimal totalEggsUnits = eggUnits;
                        decimal totalCrackedEggs = crackedEggs;
                        decimal totalDirtyEggs = dirtyEggs;
                        decimal totalBigEggs = bigEggs;
                        decimal totalSmallEggs = smallEggs;
                        decimal totalBrokenEggs = brokenEggs;
                        decimal totalSalesEggs = salesEggs;

                        while (layDate <= layDateFim)
                        {
                            #region Load Values

                            string trackNO = "EXP" + layDate.ToString("yyMMdd");
                            string numLote = GetNumLote(company, region, location, farmID, flockID, layDate);

                            decimal currentEggsUnits = eggUnits;
                            decimal currentCrackedEggs = crackedEggs;
                            decimal currentDirtyEggs = dirtyEggs;
                            decimal currentBigEggs = bigEggs;
                            decimal currentSmallEggs = smallEggs;
                            decimal currentBrokenEggs = brokenEggs;
                            decimal currentSalesEggs = salesEggs;

                            int posicaoFiltro = Convert.ToInt32(posicao);
                            string flockIDFiltro = farmID + "-" + flockID;
                            int existeSQL = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Company == company && h.Region == region && h.Location == location
                                    && h.Set_date == dataIncubacao && h.Hatch_loc == ddlIncubatorios.SelectedValue
                                    && h.Flock_id == flockIDFiltro && h.Lay_date == layDate && h.Machine == machine
                                    && h.ClassOvo == hatchLocEgg
                                    && h.Track_no == trackNO && h.Posicao == posicaoFiltro)
                                .Count();

                            #endregion

                            if (existeSQL != 0 && periodLayDateInc != "YES")
                            {
                                #region If exists in the same position, show warning message

                                //qtd = qtd + Convert.ToDecimal(hatcheryEggDataObjectDelete.Eggs_rcvd);
                                //bdSQLServer.HATCHERY_EGG_DATA.DeleteObject(hatcheryEggDataObjectDelete);
                                lblMensagem.Visible = true;
                                lblMensagem.Text = Translate("Lote") + " "
                                    + flockID + " " + Translate("já incluso na posição") + " "
                                    + posicao.ToString() + ". " + Translate("Verifique!");

                                #endregion
                            }
                            else
                            {
                                bool existeSaldo = false;
                                if (periodLayDateInc != "YES")
                                {
                                    #region If not exists balance, show warning message

                                    int balance = VerificaEstoqueWEB(layDate, flockID, (int)totalQty,
                                        hatchLocEgg, 0);
                                    if (balance > 0)
                                    {
                                        lblMensagem.Visible = true;
                                        lblMensagem.Text = Translate("Saldo insuficiente") + "! " + Translate("Verifique!");
                                        return;
                                    }
                                    else 
                                        existeSaldo = true;

                                    #endregion
                                }
                                else
                                {
                                    currentEggsUnits = 0;
                                    currentCrackedEggs = 0;
                                    currentDirtyEggs = 0;
                                    currentBigEggs = 0;
                                    currentSmallEggs = 0;
                                    currentBrokenEggs = 0;
                                    currentSalesEggs = 0;

                                    #region If not exists balance, next Lay Date

                                    int balance = VerificaSaldoWEB(layDate, flockID, hatchLocEgg);
                                    decimal totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                        totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                    decimal totalRestanteInicial = totalRestante;
                                    if (balance > 0 && totalRestante > 0)
                                    {
                                        existeSaldo = true;

                                        if ((totalRestante - balance) >= 0)
                                        {
                                            #region Egg Units

                                            currentEggsUnits = Math.Round((totalEggsUnits / totalRestanteInicial) * balance, 0);
                                            totalEggsUnits = totalEggsUnits - currentEggsUnits;
                                            totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                totalSmallEggs + totalBrokenEggs + totalSalesEggs;

                                            #endregion

                                            #region Cracked Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentCrackedEggs = Math.Round((totalCrackedEggs / totalRestanteInicial) * balance, 0);
                                                totalCrackedEggs = totalCrackedEggs - currentCrackedEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Dirty Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentDirtyEggs = Math.Round((totalDirtyEggs / totalRestanteInicial) * balance, 0);
                                                totalDirtyEggs = totalDirtyEggs - currentDirtyEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Big Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentBigEggs = Math.Round((totalBigEggs / totalRestanteInicial) * balance, 0);
                                                totalBigEggs = totalBigEggs - currentBigEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Small Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentSmallEggs = Math.Round((totalSmallEggs / totalRestanteInicial) * balance, 0);
                                                totalSmallEggs = totalSmallEggs - currentSmallEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Broken Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentBrokenEggs = Math.Round((totalBrokenEggs / totalRestanteInicial) * balance, 0);
                                                totalBrokenEggs = totalBrokenEggs - currentBrokenEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Sales Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentSalesEggs = Math.Round((totalSalesEggs / totalRestanteInicial) * balance, 0);
                                                totalSalesEggs = totalSalesEggs - currentSalesEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Zera a quantidade do estoque ajustando o rateio

                                            decimal currentTotal = currentEggsUnits + currentCrackedEggs + currentDirtyEggs + currentBigEggs
                                                + currentSmallEggs + currentBrokenEggs + currentSalesEggs;

                                            decimal difEggsRateio = balance - currentTotal;

                                            while (difEggsRateio > 0)
                                            {
                                                #region Egg Units

                                                if (totalEggsUnits >= difEggsRateio)
                                                {
                                                    currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                    totalEggsUnits = totalEggsUnits - difEggsRateio;
                                                    difEggsRateio = 0;
                                                }
                                                else
                                                {
                                                    currentEggsUnits = currentEggsUnits + totalEggsUnits;
                                                    totalEggsUnits = 0;
                                                    difEggsRateio = difEggsRateio - totalEggsUnits;
                                                }

                                                #endregion

                                                #region Cracked Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalCrackedEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalCrackedEggs = totalCrackedEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentCrackedEggs = currentCrackedEggs + totalCrackedEggs;
                                                        totalCrackedEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalCrackedEggs;
                                                    }
                                                }

                                                #endregion

                                                #region Dirty Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalDirtyEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalDirtyEggs = totalDirtyEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentDirtyEggs = currentDirtyEggs + totalDirtyEggs;
                                                        totalDirtyEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalDirtyEggs;
                                                    }
                                                }

                                                #endregion

                                                #region Big Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalBigEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalBigEggs = totalBigEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentBigEggs = currentBigEggs + totalBigEggs;
                                                        totalBigEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalBigEggs;
                                                    }
                                                }

                                                #endregion

                                                #region Small Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalSmallEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalSmallEggs = totalSmallEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentSmallEggs = currentSmallEggs + totalSmallEggs;
                                                        totalSmallEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalSmallEggs;
                                                    }
                                                }

                                                #endregion

                                                #region Broken Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalBrokenEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalBrokenEggs = totalBrokenEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentBrokenEggs = currentBrokenEggs + totalBrokenEggs;
                                                        totalBrokenEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalBrokenEggs;
                                                    }
                                                }

                                                #endregion

                                                #region Sales Eggs

                                                if (difEggsRateio > 0)
                                                {
                                                    if (totalSalesEggs >= difEggsRateio)
                                                    {
                                                        currentEggsUnits = currentEggsUnits + difEggsRateio;
                                                        totalSalesEggs = totalSalesEggs - difEggsRateio;
                                                        difEggsRateio = 0;
                                                    }
                                                    else
                                                    {
                                                        currentSalesEggs = currentSalesEggs + totalSalesEggs;
                                                        totalSalesEggs = 0;
                                                        difEggsRateio = difEggsRateio - totalSalesEggs;
                                                    }
                                                }

                                                #endregion
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            #region Egg Units

                                            currentEggsUnits = totalEggsUnits;
                                            totalEggsUnits = totalEggsUnits - currentEggsUnits;
                                            totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                totalSmallEggs + totalBrokenEggs + totalSalesEggs;

                                            #endregion

                                            #region Cracked Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentCrackedEggs = totalCrackedEggs;
                                                totalCrackedEggs = totalCrackedEggs - currentCrackedEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Dirty Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentDirtyEggs = totalDirtyEggs;
                                                totalDirtyEggs = totalDirtyEggs - currentDirtyEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Big Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentBigEggs = totalBigEggs;
                                                totalBigEggs = totalBigEggs - currentBigEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Small Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentSmallEggs = totalSmallEggs;
                                                totalSmallEggs = totalSmallEggs - currentSmallEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Broken Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentBrokenEggs = totalBrokenEggs;
                                                totalBrokenEggs = totalBrokenEggs - currentBrokenEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion

                                            #region Sales Eggs

                                            if (totalRestante > 0)
                                            {
                                                currentSalesEggs = totalSalesEggs;
                                                totalSalesEggs = totalSalesEggs - currentSalesEggs;
                                                totalRestante = totalEggsUnits + totalCrackedEggs + totalDirtyEggs + totalBigEggs +
                                                    totalSmallEggs + totalBrokenEggs + totalSalesEggs;
                                            }

                                            #endregion
                                        }
                                    }

                                    #endregion
                                }

                                if (existeSaldo)
                                {
                                    if (eggUnits > 0)
                                    {
                                        #region Insere na base SQL Server

                                        lblMensagem.Visible = false;

                                        var hatcheryEggDataObject = new HATCHERY_EGG_DATA();

                                        hatcheryEggDataObject.Company = company;
                                        hatcheryEggDataObject.Region = region;
                                        hatcheryEggDataObject.Location = location;
                                        hatcheryEggDataObject.Set_date = dataIncubacao;
                                        hatcheryEggDataObject.Hatch_loc = ddlIncubatorios.SelectedValue;
                                        hatcheryEggDataObject.Flock_id = farmID + "-" + flockID;
                                        hatcheryEggDataObject.Lay_date = layDate;
                                        //hatcheryEggDataObject.Eggs_rcvd = Convert.ToInt32(qtd);
                                        hatcheryEggDataObject.Eggs_rcvd = Convert.ToInt32(currentEggsUnits);
                                        hatcheryEggDataObject.Machine = machine;
                                        hatcheryEggDataObject.Track_no = trackNO;
                                        hatcheryEggDataObject.Posicao = Convert.ToInt32(posicao);
                                        hatcheryEggDataObject.Bandejas = Convert.ToInt32(bandejas);
                                        hatcheryEggDataObject.Horario = horario;
                                        hatcheryEggDataObject.Estimate = media;
                                        hatcheryEggDataObject.Variety = Session["linhagem"].ToString().Replace("amp;", "");
                                        hatcheryEggDataObject.Age = Convert.ToInt32(Session["age"]);
                                        hatcheryEggDataObject.Observacao = observacao;
                                        hatcheryEggDataObject.Status = status;
                                        hatcheryEggDataObject.Usuario = Session["usuario"].ToString();
                                        hatcheryEggDataObject.Egg_key = numLote;
                                        //hatcheryEggDataObject.ImportadoApolo = "Não";
                                        /*
                                        * 13/01/2015 - Solicitado por Davi Nogueira
                                        * 
                                        * Não realizar controle de estoque no Incubatório da Planalto (NM) até 31/12/2016,
                                        * pois ocorrem muitos erros de lançamentos de DEOs por parte da Granja
                                        * e não estão conseguindo ajustar.
                                        * */
                                        if (incubatorio != "NM" ||
                                            (incubatorio == "NM" && dataIncubacao >= Convert.ToDateTime("01/01/2017")))
                                            hatcheryEggDataObject.ImportadoApolo = Session["tipoCadastro"].ToString();
                                        else
                                            hatcheryEggDataObject.ImportadoApolo = "Estoque Real";
                                        hatcheryEggDataObject.ClassOvo = hatchLocEgg;

                                        bdSQLServer.HATCHERY_EGG_DATA.AddObject(hatcheryEggDataObject);

                                        #endregion

                                        if (status == "Importado")
                                        {
                                            #region Insere na tabela da Data de Incubação - FLIP

                                            bool imported = UpdateSetFLIP(company, region, farmID, flockID, layDate, dataIncubacao, location,
                                                ddlIncubatorios.SelectedValue, currentEggsUnits, machine, trackNO, media,
                                                observacao);

                                            if (imported)
                                                hatcheryEggDataObject.ImportadoFLIP = "Sim";
                                            else
                                                hatcheryEggDataObject.ImportadoFLIP = "Não";

                                            #endregion

                                            #region Importa p/ Apolo - **** DESATIVADO ****

                                            //LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                                            //    .Where(l => l.USERCodigoFLIP == hatchLocEgg && l.USERTipoProduto == "Ovos Incubáveis")
                                            //    .FirstOrDefault();

                                            //CTRL_LOTE_LOC_ARMAZ tabLoteApolo = bdApolo.CTRL_LOTE_LOC_ARMAZ
                                            //    .Where(c => c.CtrlLoteNum == flockID && c.CtrlLoteDataValid == layDate
                                            //        && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr)
                                            //    .FirstOrDefault();

                                            //if (tabLoteApolo.USERQtdeIncNaoImportApolo == null) tabLoteApolo.USERQtdeIncNaoImportApolo = 0;
                                            //tabLoteApolo.USERQtdeIncNaoImportApolo = tabLoteApolo.USERQtdeIncNaoImportApolo + Convert.ToInt32(qtd);

                                            //bdApolo.SaveChanges();

                                            /*
                                            #region Carrega variáveis e objetos

                                            string naturezaOperacao = "5.101";
                                            decimal? valorUnitario = 0;
                                            if (incubatorio.Equals("NM"))
                                                valorUnitario = 0.90m;
                                            else
                                                valorUnitario = 0.25m;
                                            string unidadeMedida = "UN";
                                            short? posicaoUnidadeMedida = 1;
                                            string tribCod = "040";
                                            string itMovEstqClasFiscCodNbm = "04079000";
                                            string clasFiscCod = "0000129";
                                            string operacao = "Saída";

                                            ITEM_MOV_ESTQ itemMovEstq;

                                            string usuario;
                                            if (Session["login"].ToString().Equals("palves"))
                                                usuario = "RIOSOFT";
                                            else
                                                usuario = Session["login"].ToString().ToUpper();

                                            ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
                                                .Where(ef => ef.USERFLIPCod == empresaEstoque)
                                                .FirstOrDefault();

                                            locArmaz = bdApolo.LOC_ARMAZ
                                                .Where(l => l.USERCodigoFLIP == hatchLocEgg && l.USERTipoProduto == "Ovos Incubáveis")
                                                .FirstOrDefault();

                                            string linhagem = Session["linhagem"].ToString();

                                            PRODUTO produto = produto = bdApolo.PRODUTO
                                                .Where(p => p.ProdNomeAlt1 == linhagem)
                                                .FirstOrDefault();

                                            #endregion

                                            #region Se Incubatório de Terceiro, gera a transferência primeiro (Desativada - será feita manualmente pelo Apolo)

                                            //if ((locArmaz.USERCodigoFLIP != locArmaz.USERLocalEstoqueIncub) &&
                                            //    (locArmaz.USERLocalEstoqueIncub != null))
                                            //{
                                            //    #region Carrega variáveis e objetos

                                            //    empresa = bdApolo.EMPRESA_FILIAL
                                            //        .Where(ef => ef.USERFLIPCod == locArmaz.USERLocalEstoqueIncub)
                                            //        .FirstOrDefault();

                                            //    TRANSF_ESTQ_LOC_ARMAZ transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();
                                            //    ITEM_TRANSF_ESTQ_LOC_ARMAZ itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                                            //    LOC_ARMAZ localArmazSaida = bdApolo.LOC_ARMAZ
                                            //            .Where(l => l.USERCodigoFLIP == locArmaz.USERLocalEstoqueIncub && l.USERTipoProduto == "Ovos Incubáveis")
                                            //            .FirstOrDefault();

                                            //    string tipoLanc = localArmazSaida.USERTipoLancSaidaCom;

                                            //    #endregion

                                            //    #region Verifica se já tem a Transferência. Caso não tenha, será inserida.

                                            //    transfEstqLocArmaz = bdApolo.TRANSF_ESTQ_LOC_ARMAZ
                                            //        .Where(t => t.EmpCod == empresa.EmpCod && t.TransfEstqLocArmazData == dataIncubacao)
                                            //        .FirstOrDefault();

                                            //    if (transfEstqLocArmaz == null)
                                            //    {
                                            //        #region Insere Nova Transferência

                                            //        transfEstqLocArmaz = new TRANSF_ESTQ_LOC_ARMAZ();

                                            //        ObjectParameter numero = new ObjectParameter("codigo", typeof(global::System.String));

                                            //        bdApolo.gerar_codigo("1", "TRANSF_ESTQ_LOC_ARMAZ", numero);

                                            //        transfEstqLocArmaz.EmpCod = empresa.EmpCod;
                                            //        transfEstqLocArmaz.TipoLancCod = tipoLanc;
                                            //        transfEstqLocArmaz.TransfEstqLocArmazData = dataIncubacao;
                                            //        transfEstqLocArmaz.TransfEstqLocArmazNum = Convert.ToInt32(numero.Value);

                                            //        string nomeIncubatorio = ddlIncubatorios.SelectedItem.Text;

                                            //        transfEstqLocArmaz.TransfEstqLocArmazObs = "Transferência de Ovos Férteis p/ " + nomeIncubatorio + ".";

                                            //        bdApolo.TRANSF_ESTQ_LOC_ARMAZ.AddObject(transfEstqLocArmaz);

                                            //        #endregion
                                            //    }

                                            //    #endregion

                                            //    #region Verifica se existe o item. Caso não existe, será inserido.

                                            //    itemTransfEstqLocArmaz = bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ
                                            //        .Where(i => i.EmpCod == transfEstqLocArmaz.EmpCod
                                            //            && i.TransfEstqLocArmazNum == transfEstqLocArmaz.TransfEstqLocArmazNum
                                            //            && i.ProdCodEstr == produto.ProdCodEstr)
                                            //        .FirstOrDefault();

                                            //    if (itemTransfEstqLocArmaz == null)
                                            //    {
                                            //        int ultimaSequencia = bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ
                                            //                                .Where(i => i.EmpCod == transfEstqLocArmaz.EmpCod
                                            //                                    && i.TransfEstqLocArmazNum == transfEstqLocArmaz.TransfEstqLocArmazNum)
                                            //                                .Count();

                                            //        ultimaSequencia = ++ultimaSequencia;

                                            //        itemTransfEstqLocArmaz = new ITEM_TRANSF_ESTQ_LOC_ARMAZ();

                                            //        itemTransfEstqLocArmaz.EmpCod = transfEstqLocArmaz.EmpCod;
                                            //        itemTransfEstqLocArmaz.TransfEstqLocArmazNum = transfEstqLocArmaz.TransfEstqLocArmazNum;
                                            //        itemTransfEstqLocArmaz.ProdCodEstr = produto.ProdCodEstr;
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq = (short)ultimaSequencia;
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida = localArmazSaida.LocArmazCodEstr;
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada = locArmaz.LocArmazCodEstr;
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazObs = transfEstqLocArmaz.TransfEstqLocArmazObs;
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = qtd;

                                            //        bdApolo.ITEM_TRANSF_ESTQ_LOC_ARMAZ.AddObject(itemTransfEstqLocArmaz);
                                            //    }
                                            //    else
                                            //    {
                                            //        itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd = itemTransfEstqLocArmaz.ItTransfEstqLocArmazQtd + qtd;
                                            //    }

                                            //    #endregion

                                            //    #region Insere o Lote

                                            //    IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE itemTransfEstqLocArmazLote = bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE
                                            //        .Where(l => l.EmpCod == itemTransfEstqLocArmaz.EmpCod
                                            //            && l.TransfEstqLocArmazNum == itemTransfEstqLocArmaz.TransfEstqLocArmazNum
                                            //            && l.ItTransfEstqLocArmazSeq == itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq
                                            //            && l.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                            //            && l.CtrlLoteNum == flockID && l.CtrlLoteDataValid == layDate)
                                            //        .FirstOrDefault();

                                            //    if (itemTransfEstqLocArmazLote == null)
                                            //    {
                                            //        PROD_UNID_MED prodUnidMed = bdApolo.PROD_UNID_MED
                                            //            .Where(p => p.ProdCodEstr == itemTransfEstqLocArmaz.ProdCodEstr
                                            //                && p.ProdUnidMedCod == unidadeMedida)
                                            //            .FirstOrDefault();

                                            //        itemTransfEstqLocArmazLote = new IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE();

                                            //        itemTransfEstqLocArmazLote.EmpCod = itemTransfEstqLocArmaz.EmpCod;
                                            //        itemTransfEstqLocArmazLote.TransfEstqLocArmazNum = itemTransfEstqLocArmaz.TransfEstqLocArmazNum;
                                            //        itemTransfEstqLocArmazLote.ProdCodEstr = itemTransfEstqLocArmaz.ProdCodEstr;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSeq = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSeq;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazSaida = itemTransfEstqLocArmaz.ItTransfEstqLocArmazSaida;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazEntrada = itemTransfEstqLocArmaz.ItTransfEstqLocArmazEntrada;
                                            //        itemTransfEstqLocArmazLote.ProdUnidMedCod = prodUnidMed.ProdUnidMedCod;
                                            //        itemTransfEstqLocArmazLote.ProdUnidMedPos = prodUnidMed.ProdUnidMedPos;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = qtd;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                                            //        itemTransfEstqLocArmazLote.CtrlLoteNum = flockID;
                                            //        itemTransfEstqLocArmazLote.CtrlLoteDataValid = layDate;

                                            //        bdApolo.IT_TRANSF_ESTQ_LOC_ARMAZ_LOTE.AddObject(itemTransfEstqLocArmazLote);
                                            //    }
                                            //    else
                                            //    {
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd + qtd;
                                            //        itemTransfEstqLocArmazLote.ItTransfEstqLocArmLoteQtdCalc = itemTransfEstqLocArmazLote.ItTransfEstqLocArmazLoteQtd;
                                            //    }

                                            //    #endregion

                                            //    bdApolo.SaveChanges();

                                            //    #region Integra a transferência com o Estoque

                                            //    string nfNum = transfEstqLocArmaz.TransfEstqLocArmazNum.ToString();

                                            //    MOV_ESTQ movEstqSaida = bdApolo.MOV_ESTQ
                                            //        .Where(m => m.EmpCod == transfEstqLocArmaz.EmpCod
                                            //            && m.MovEstqDocEspec == "TLA"
                                            //            && m.MovEstqDocSerie == "00"
                                            //            && m.MovEstqDocNum == nfNum)
                                            //        .FirstOrDefault();

                                            //    //ObjectParameter msg = new ObjectParameter("rmensagem", typeof(global::System.String));

                                            //    //bdApolo.delete_movestq(movEstqSaida.EmpCod, movEstqSaida.MovEstqChv, "RIOSOFT", msg);

                                            //    if (movEstqSaida != null)
                                            //    {
                                            //        #region Atualiza Mov. Estq. de Transf. de Saída

                                            //        ITEM_MOV_ESTQ itemMovEstqSaida = bdApolo.ITEM_MOV_ESTQ
                                            //            .Where(i => i.EmpCod == movEstqSaida.EmpCod
                                            //                && i.MovEstqChv == movEstqSaida.MovEstqChv
                                            //                && i.ProdCodEstr == produto.ProdCodEstr)
                                            //            .FirstOrDefault();

                                            //        itemMovEstqSaida.ItMovEstqQtdProd = itemMovEstqSaida.ItMovEstqQtdProd + qtd;
                                            //        itemMovEstqSaida.ItMovEstqQtdCalcProd = itemMovEstqSaida.ItMovEstqQtdProd;

                                            //        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstqSaida = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                                            //            .Where(l => l.EmpCod == itemMovEstqSaida.EmpCod
                                            //                && l.MovEstqChv == itemMovEstqSaida.MovEstqChv
                                            //                && l.ItMovEstqSeq == itemMovEstqSaida.ItMovEstqSeq
                                            //                && l.ProdCodEstr == itemMovEstqSaida.ProdCodEstr
                                            //                && l.LocArmazCodEstr == localArmazSaida.LocArmazCodEstr)
                                            //            .FirstOrDefault();

                                            //        locArmazItemMovEstqSaida.LocArmazItMovEstqQtd = locArmazItemMovEstqSaida.LocArmazItMovEstqQtd + qtd;
                                            //        locArmazItemMovEstqSaida.LocArmazItMovEstqQtdCalc = locArmazItemMovEstqSaida.LocArmazItMovEstqQtd;

                                            //        CTRL_LOTE_ITEM_MOV_ESTQ ctrlLoteItemMovEstqSaida = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                            //            .Where(c => c.EmpCod == locArmazItemMovEstqSaida.EmpCod
                                            //                && c.MovEstqChv == locArmazItemMovEstqSaida.MovEstqChv
                                            //                && c.ItMovEstqSeq == locArmazItemMovEstqSaida.ItMovEstqSeq
                                            //                && c.ProdCodEstr == locArmazItemMovEstqSaida.ProdCodEstr
                                            //                && c.LocArmazCodEstr == locArmazItemMovEstqSaida.LocArmazCodEstr
                                            //                && c.CtrlLoteNum == flockID && c.CtrlLoteDataValid == layDate)
                                            //            .FirstOrDefault();

                                            //        if (ctrlLoteItemMovEstqSaida != null)
                                            //        {

                                            //            ctrlLoteItemMovEstqSaida.CtrlLoteItMovEstqQtd = ctrlLoteItemMovEstqSaida.CtrlLoteItMovEstqQtd + qtd;
                                            //            ctrlLoteItemMovEstqSaida.CtrlLoteItMovEstqQtdCalc = ctrlLoteItemMovEstqSaida.CtrlLoteItMovEstqQtd;
                                            //        }
                                            //        else
                                            //        {
                                            //            ctrlLoteItemMovEstqSaida = service.InsereLote(
                                            //                locArmazItemMovEstqSaida.MovEstqChv,
                                            //                locArmazItemMovEstqSaida.EmpCod,
                                            //                itemMovEstqSaida.TipoLancCod,
                                            //                locArmazItemMovEstqSaida.ItMovEstqSeq,
                                            //                locArmazItemMovEstqSaida.ProdCodEstr,
                                            //                flockID,
                                            //                layDate,
                                            //                qtd,
                                            //                "Saída",
                                            //                itemMovEstqSaida.ItMovEstqUnidMedCod,
                                            //                itemMovEstqSaida.ItMovEstqUnidMedPos,
                                            //                locArmazItemMovEstqSaida.LocArmazCodEstr);

                                            //            bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(ctrlLoteItemMovEstqSaida);
                                            //        }

                                            //        bdApolo.SaveChanges();

                                            //        bdApolo.calcula_mov_estq(itemMovEstqSaida.EmpCod, itemMovEstqSaida.MovEstqChv);

                                            //        #endregion

                                            //        #region Atualiza Mov. Estq. de Transf. de Entrada

                                            //        MOV_ESTQ movEstqEntrada = bdApolo.MOV_ESTQ
                                            //        .Where(m => m.EmpCod == transfEstqLocArmaz.EmpCod
                                            //            && m.MovEstqDocEspec == "TLA"
                                            //            && m.MovEstqDocSerie == "99"
                                            //            && m.MovEstqDocNum == nfNum)
                                            //        .FirstOrDefault();

                                            //        ITEM_MOV_ESTQ itemMovEstqEntrada = bdApolo.ITEM_MOV_ESTQ
                                            //            .Where(i => i.EmpCod == movEstqEntrada.EmpCod
                                            //                && i.MovEstqChv == movEstqEntrada.MovEstqChv
                                            //                && i.ProdCodEstr == produto.ProdCodEstr)
                                            //            .FirstOrDefault();

                                            //        itemMovEstqEntrada.ItMovEstqQtdProd = itemMovEstqEntrada.ItMovEstqQtdProd + qtd;
                                            //        itemMovEstqEntrada.ItMovEstqQtdCalcProd = itemMovEstqEntrada.ItMovEstqQtdProd;

                                            //        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstqEntrada = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                                            //            .Where(l => l.EmpCod == itemMovEstqEntrada.EmpCod
                                            //                && l.MovEstqChv == itemMovEstqEntrada.MovEstqChv
                                            //                && l.ItMovEstqSeq == itemMovEstqEntrada.ItMovEstqSeq
                                            //                && l.ProdCodEstr == itemMovEstqEntrada.ProdCodEstr
                                            //                && l.LocArmazCodEstr == locArmaz.LocArmazCodEstr)
                                            //            .FirstOrDefault();

                                            //        locArmazItemMovEstqEntrada.LocArmazItMovEstqQtd = locArmazItemMovEstqEntrada.LocArmazItMovEstqQtd + qtd;
                                            //        locArmazItemMovEstqEntrada.LocArmazItMovEstqQtdCalc = locArmazItemMovEstqEntrada.LocArmazItMovEstqQtd;

                                            //        CTRL_LOTE_ITEM_MOV_ESTQ ctrlLoteItemMovEstqEntrada = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                            //            .Where(c => c.EmpCod == locArmazItemMovEstqEntrada.EmpCod
                                            //                && c.MovEstqChv == locArmazItemMovEstqEntrada.MovEstqChv
                                            //                && c.ItMovEstqSeq == locArmazItemMovEstqEntrada.ItMovEstqSeq
                                            //                && c.ProdCodEstr == locArmazItemMovEstqEntrada.ProdCodEstr
                                            //                && c.LocArmazCodEstr == locArmazItemMovEstqEntrada.LocArmazCodEstr
                                            //                && c.CtrlLoteNum == flockID && c.CtrlLoteDataValid == layDate)
                                            //            .FirstOrDefault();

                                            //        if (ctrlLoteItemMovEstqEntrada != null)
                                            //        {
                                            //            ctrlLoteItemMovEstqEntrada.CtrlLoteItMovEstqQtd = ctrlLoteItemMovEstqEntrada.CtrlLoteItMovEstqQtd + qtd;
                                            //            ctrlLoteItemMovEstqEntrada.CtrlLoteItMovEstqQtdCalc = ctrlLoteItemMovEstqEntrada.CtrlLoteItMovEstqQtd;
                                            //        }
                                            //        else
                                            //        {
                                            //            ctrlLoteItemMovEstqEntrada = service.InsereLote(
                                            //                locArmazItemMovEstqEntrada.MovEstqChv,
                                            //                locArmazItemMovEstqEntrada.EmpCod,
                                            //                itemMovEstqEntrada.TipoLancCod,
                                            //                locArmazItemMovEstqEntrada.ItMovEstqSeq,
                                            //                locArmazItemMovEstqEntrada.ProdCodEstr,
                                            //                flockID,
                                            //                layDate,
                                            //                qtd,
                                            //                "Entrada",
                                            //                itemMovEstqEntrada.ItMovEstqUnidMedCod,
                                            //                itemMovEstqEntrada.ItMovEstqUnidMedPos,
                                            //                locArmazItemMovEstqEntrada.LocArmazCodEstr);

                                            //            bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(ctrlLoteItemMovEstqEntrada);
                                            //        }

                                            //        bdApolo.SaveChanges();

                                            //        bdApolo.calcula_mov_estq(itemMovEstqEntrada.EmpCod, itemMovEstqEntrada.MovEstqChv);

                                            //        #endregion

                                            //        bdApolo.atualiza_saldoestqdata(itemMovEstqSaida.EmpCod, itemMovEstqSaida.MovEstqChv,
                                            //            itemMovEstqSaida.ProdCodEstr, itemMovEstqSaida.ItMovEstqSeq, 
                                            //            itemMovEstqSaida.ItMovEstqDataMovimento, "UPD");
                                            //    }
                                            //    else
                                            //    {
                                            //        bdApolo.transflocarmaz_gera_movestq(transfEstqLocArmaz.EmpCod, transfEstqLocArmaz.TransfEstqLocArmazNum,
                                            //            usuario);
                                            //    }

                                            //    //bdApolo.delete_movestq(movEstqEntrada.EmpCod, movEstqEntrada.MovEstqChv, "RIOSOFT", msg);

                                            //    #endregion
                                            //}

                                            #endregion

                                            #region Insere Saida p/ Incubação

                                            // Verifica se Existe a movimentação neste Incubatório e Produto
                                            LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                                                .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                                    && i.ProdCodEstr == produto.ProdCodEstr
                                                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                                                        && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                                            .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                                                .FirstOrDefault();

                                            if (locItemMovEstq != null)
                                            {
                                                itemMovEstq = bdApolo.ITEM_MOV_ESTQ
                                                    .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
                                                        && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
                                                    .FirstOrDefault();

                                                itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + qtd;
                                                itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

                                                locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + qtd;
                                                locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

                                                CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                                    .Where(c => c.EmpCod == locItemMovEstq.EmpCod && c.MovEstqChv == locItemMovEstq.MovEstqChv
                                                        && c.ProdCodEstr == locItemMovEstq.ProdCodEstr && c.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq
                                                        && c.LocArmazCodEstr == locItemMovEstq.LocArmazCodEstr && c.CtrlLoteNum == flockID
                                                        && c.CtrlLoteDataValid == layDate)
                                                    .FirstOrDefault();

                                                // Verifica se Existe o lote
                                                if (lote != null)
                                                {
                                                    lote.CtrlLoteItMovEstqQtd = lote.CtrlLoteItMovEstqQtd + qtd;
                                                    lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;
                                                }
                                                else
                                                {
                                                    lote = service.InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
                                                        locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flockID, layDate, qtd, operacao,
                                                        itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

                                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                                                }
                                            }
                                            else
                                            {
                                                // Verifica se Existe a movimentação neste Incubatório e não no Produto
                                                locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                                                    .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                                                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                                                            && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                                            .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                                                    .FirstOrDefault();

                                                if (locItemMovEstq != null)
                                                {
                                                    MOV_ESTQ movEstq = bdApolo.MOV_ESTQ
                                                        .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
                                                        .FirstOrDefault();

                                                    itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
                                                        movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                                                        linhagem, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                                                        tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                                                    bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                    LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                                                        service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                                                        itemMovEstq.ProdCodEstr, qtd, locItemMovEstq.LocArmazCodEstr);

                                                    bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                                                    CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                                                        itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                                                        layDate, qtd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                                                        locArmazItemMovEstq.LocArmazCodEstr);

                                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                                                }
                                                else
                                                {
                                                    MOV_ESTQ movEstq = service.InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
                                                        dataIncubacao, usuario);

                                                    bdApolo.MOV_ESTQ.AddObject(movEstq);

                                                    itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
                                                        movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
                                                        linhagem, naturezaOperacao, qtd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
                                                        tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

                                                    bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

                                                    LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
                                                        service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
                                                        itemMovEstq.ProdCodEstr, qtd, locArmaz.LocArmazCodEstr);

                                                    bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

                                                    CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
                                                        itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
                                                        layDate, qtd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
                                                        locArmazItemMovEstq.LocArmazCodEstr);

                                                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
                                                }
                                            }
                                            bdApolo.SaveChanges();

                                            bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

                                            hatcheryEggDataObject.ImportadoApolo = "Sim";

                                            //bdApolo.CommandTimeout = 60;

                                            //bdApolo.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr,
                                            //    itemMovEstq.ItMovEstqSeq, itemMovEstq.ItMovEstqDataMovimento, "INS");

                                            #endregion

                                            */
                                            #endregion
                                        }

                                        #region Save Changes and Refresh Data

                                        bdSQLServer.SaveChanges();

                                        AtualizaQtdeIncubadaNascimentoWEB(hatcheryEggDataObject.Hatch_loc,
                                            hatcheryEggDataObject.Set_date, hatcheryEggDataObject.Flock_id,
                                            hatcheryEggDataObject.Machine, hatcheryEggDataObject.ClassOvo);

                                        //AtualizaFLIP(dataIncubacao);
                                        RefreshFLIP(ddlIncubatorios.SelectedValue, dataIncubacao, false);
                                        VerificaNaoImportados();

                                        #endregion

                                        #region Generate Tranf. Automatically if config

                                        HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
                                        hTA.Fill(flipDataSet.HATCHERY_CODES);
                                        var hatchLocObj = flipDataSet.HATCHERY_CODES.Where(w => w.HATCH_LOC == hatcheryEggDataObject.Hatch_loc)
                                            .FirstOrDefault();
                                        if (hatchLocObj.AUTO_TRANSF == "YES")
                                            GenerateTransfAutomatically(hatcheryEggDataObject.Company, hatcheryEggDataObject.Region,
                                                hatcheryEggDataObject.Location, hatcheryEggDataObject.Hatch_loc, hatcheryEggDataObject.Set_date,
                                                hatcheryEggDataObject.Lay_date, hatcheryEggDataObject.Flock_id, hatcheryEggDataObject.Machine,
                                                hatcheryEggDataObject.ClassOvo);

                                        #endregion
                                    }

                                    #region Generate Sorting Eggs Movement

                                    HLBAPPEntities hlbapp = new HLBAPPEntities();

                                    #region Cracked Eggs

                                    if (currentCrackedEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentCrackedEggs, incubatorio, "HT", "Huevos Trizados", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    #region Dirty Eggs

                                    if (currentDirtyEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentDirtyEggs, incubatorio, "HS", "Huevos Sucios", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    #region Big Eggs

                                    if (currentBigEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentBigEggs, incubatorio, "HG", "Huevos Grandes", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    #region Small Eggs

                                    if (currentSmallEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentSmallEggs, incubatorio, "HC", "Huevos Chicos", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    #region Broken Eggs

                                    if (currentBrokenEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentBrokenEggs, incubatorio, "HB", "Huevos Botados", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    #region Sales Eggs

                                    if (currentSalesEggs > 0)
                                    {
                                        LayoutDiarioExpedicaos deo = InsertDEOSortingEggs(incubatorio, farmID, flockID, Convert.ToDecimal(numLote),
                                            Convert.ToDecimal(Session["numGalpao"]), Session["linhagem"].ToString(), Convert.ToInt32(Session["age"]),
                                            layDate, currentSalesEggs, incubatorio, "HP", "Huevos in Packing", dataIncubacao);
                                        if (deo.ID == 0) hlbapp.LayoutDiarioExpedicaos.AddObject(deo);
                                    }

                                    #endregion

                                    hlbapp.SaveChanges();

                                    #endregion

                                    #region Update Data Tables Screen

                                    AtualizaTotais();

                                    GridView1.DataBind();
                                    GridView3.DataBind();
                                    gdvClasOvos.DataBind();
                                    gvMaquinas.DataBind();
                                    gvLotes.DataBind();
                                    gvLinhagens.DataBind();

                                    FormView1.ChangeMode(FormViewMode.ReadOnly);

                                    #endregion
                                }
                            }
                            
                            layDate = layDate.AddDays(1);
                        }
                    }
                    else
                    {
                        if (ExisteFechamentoEstoque(company, incubatorio, dataIncubacao))
                        {
                            #region If closed Egg Inventory, show warning message

                            string responsavel = GetResponsableByHatchery(incubatorio);
                            lblMensagem.Visible = true;
                            lblMensagem.Text = Translate("Estoque já fechado! Verifique com") + " "
                                + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                                + Translate("Caso não seja aberto, a incubação não pode ser realizada!");

                            #endregion
                        }
                        else if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
                        {
                            #region Se existir ajuste de estoque em aberto, exibir mensagem

                            lblMensagem.Visible = true;
                            lblMensagem.Text = Translate("Existe Solicitação de Ajuste de Estoque em aberto! "
                                + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!");

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region If erros, show error message

                lblMensagem.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem.Text = "Erro ao Incubar: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem.Text = "Erro ao Incubar: " + ex.Message;

                #endregion
            }
        }

        #endregion

        #region Setting Table - GridView1

        // Search Setting ready in Setting Table
        protected void Button2_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;
            if (TextBox1.Text.Equals(""))
            {
                TextBox1.Text = "0";
            }
            GridView1.DataBind();
        }

        // Delete Setting line
        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Load Variables

                lblMensagem2.Visible = false;
                lblMensagemOvosClass.Visible = false;

                Label labelFlockID = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[4].FindControl("Label2");
                string flockID = labelFlockID.Text;

                Label labellayDate = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[8].FindControl("Label5");
                DateTime layDate = Convert.ToDateTime(labellayDate.Text);

                string trackNO = "EXP" + layDate.ToString("yyMMdd");

                Label labelmachine = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[3].FindControl("Label1");
                string machine = labelmachine.Text;

                DateTime setDate = Calendar1.SelectedDate;

                Label labelposicao = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[14].FindControl("Label11");
                decimal posicao = Convert.ToDecimal(labelposicao.Text);

                Label labelqtdOvos = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[10].FindControl("Label7");
                decimal qtdOvos = Convert.ToDecimal(labelqtdOvos.Text);
                string incubatorio = ddlIncubatorios.SelectedValue;

                Label labelClassOvo = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[19].FindControl("Label17");
                string classOvo = labelClassOvo.Text;

                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string region = GetCompanyAndRegionByHatchLoc(incubatorio, "region");
                string location = GetLocation(company, incubatorio);

                #endregion

                if (!ExisteFechamentoEstoque(company, incubatorio, setDate) && !ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
                {
                    #region Load Variables

                    int posicaoFiltro = Convert.ToInt32(posicao);
                    HATCHERY_EGG_DATA hatcheryEggDataObject = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == company && h.Region == region && h.Location == location && h.Set_date == setDate
                            && h.Hatch_loc == incubatorio && h.Flock_id == flockID && h.Lay_date == layDate && h.Machine == machine
                            && h.Track_no == trackNO && h.Posicao == posicaoFiltro && h.ClassOvo == classOvo)
                        .First();

                    int ID = hatcheryEggDataObject.ID;

                    int posicaoHifen = flockID.IndexOf("-") + 1;
                    int tamanho = flockID.Length - posicaoHifen;
                    string flock = flockID.Substring(posicaoHifen, tamanho);

                    #endregion

                    #region Delete Line

                    if (!hatcheryEggDataObject.ImportadoApolo.Equals("Estoque Real"))
                    {
                        bdSQLServer.DeleteObject(hatcheryEggDataObject);
                        bdSQLServer.SaveChanges();
                    }
                    else
                    {
                        bdSQLServer.DeleteObject(hatcheryEggDataObject);
                        bdSQLServer.SaveChanges();

                        if (hatcheryEggDataObject.Status == "Importado")
                        {
                            int existe = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Company == company && h.Region == region && h.Location == location && h.Set_date == setDate
                                    && h.Hatch_loc == ddlIncubatorios.SelectedValue && h.Flock_id == flockID && h.Lay_date == layDate && h.Machine == machine
                                    && h.Track_no == trackNO && h.Posicao != posicaoFiltro && h.ClassOvo == classOvo)
                                .Count();

                            #region Generate Tranf. Automatically if config

                            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
                            hTA.Fill(flipDataSet.HATCHERY_CODES);
                            var hatchLocObj = flipDataSet.HATCHERY_CODES.Where(w => w.HATCH_LOC == incubatorio).FirstOrDefault();
                            if (hatchLocObj.AUTO_TRANSF == "YES")
                                GenerateTransfAutomatically(company, region, location, incubatorio,
                                    setDate, layDate, flockID, machine, classOvo);

                            #endregion

                            #region Delete on FLIP

                            DeleteHatcheryEggDataLine(existe, company, region, location, incubatorio, setDate, flock, flockID, layDate,
                                machine, trackNO, qtdOvos);

                            #endregion
                        }
                    }

                    #endregion

                    #region Refresh Hatching Data WEB

                    AtualizaQtdeIncubadaNascimentoWEB(hatcheryEggDataObject.Hatch_loc,
                        hatcheryEggDataObject.Set_date, hatcheryEggDataObject.Flock_id,
                        hatcheryEggDataObject.Machine, hatcheryEggDataObject.ClassOvo);

                    #endregion

                    #region Refresh Page Data

                    GridView3.DataBind();
                    gvMaquinas.DataBind();
                    gvLotes.DataBind();
                    gvLinhagens.DataBind();
                    //GridView1.DataBind();

                    VerificaNaoImportados();

                    object objeto = Button2;
                    EventArgs e2 = new EventArgs();
                    Button2_Click(objeto, e2);
                    AtualizaTotais();

                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = Translate("Linha") + " " 
                        + (GridView1.SelectedIndex + 1).ToString() + 
                        " " + Translate("excluída com sucesso!");

                    #endregion
                }
                else
                {
                    if (ExisteFechamentoEstoque(company, incubatorio, setDate))
                    {
                        #region Show warning message

                        string responsavel = GetResponsableByHatchery(incubatorio);
                        lblMensagem.Visible = true;
                        lblMensagem.Text = Translate("Estoque já fechado! Verifique com") + " "
                            + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                            + Translate("Caso não seja aberto, a exclusão não pode ser realizada!");

                        #endregion
                    }
                    else if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubatorio))
                    {
                        #region Show warning message

                        lblMensagem.Visible = true;
                        lblMensagem.Text = Translate("Existe Solicitação de Ajuste de Estoque em aberto! "
                                + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!");

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                #region Show error message

                lblMensagem2.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem2.Text = Translate("Erro ao excluir linha")
                        + " " + (GridView1.SelectedIndex + 1).ToString() + ": " + ex.InnerException.Message;
                else
                    if (ex.Message.Length >= 63)
                    {
                        if (ex.Message.Substring(37, 24) == "NA.HATCHERY_TRAN_DATA_FK")
                        {
                            lblMensagem2.Text = Translate("Erro ao excluir linha") 
                                + " " + (GridView1.SelectedIndex + 1).ToString() + 
                                ": " + Translate("Existem Transferências p/ Nascedouro Relacionadas nesta Incubação! Primeiro delete" +
                                " para depois deletar a Incubação!");
                        }
                        else
                            lblMensagem2.Text = Translate("Erro ao excluir linha") + " " 
                                + (GridView1.SelectedIndex + 1).ToString() + ": " + ex.Message;
                    }
                    else
                        lblMensagem2.Text = Translate("Erro ao excluir linha") + " " 
                            + (GridView1.SelectedIndex + 1).ToString() + ": " + ex.Message;

                #endregion
            }
        }

        protected void btn_AtualizaSetter_Click(object sender, EventArgs e)
        {
            if ((txt_SetterDe.Text != txt_SetterPara.Text) &&
                (txt_SetterDe.Text != string.Empty) &&
                (txt_SetterPara.Text != string.Empty))
            {
                var listaAtualiza = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Set_date == Calendar1.SelectedDate && h.Machine == txt_SetterDe.Text)
                    .ToList();

                foreach (var item in listaAtualiza)
                {
                    ChangeMachine(item.Company, item.Region, item.Location, item.Set_date, item.Hatch_loc, item.Flock_id,
                        item.Lay_date, txt_SetterDe.Text, txt_SetterPara.Text, item.Track_no, Convert.ToInt32(item.Eggs_rcvd));

                    item.Machine = txt_SetterPara.Text;
                }

                bdSQLServer.SaveChanges();

                GridView1.DataBind();

                AtualizaTotais();
            }
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            AtualizaTotais();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;
        }

        // Update setting row
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            #region Load variables output try

            int index = GridView1.EditIndex;
            int id = 0;

            #endregion

            try
            {
                #region Load variables 

                Label lblID = (Label)GridView1.Rows[index].FindControl("Label1");
                id = Convert.ToInt32(lblID.Text);

                TextBox txtHorario = (TextBox)GridView1.Rows[index].FindControl("TextBox9");
                string horario = txtHorario.Text;

                TextBox txtDataPrd = (TextBox)GridView1.Rows[index].FindControl("TextBox5");
                DateTime dataPrd = Convert.ToDateTime(txtDataPrd.Text);

                TextBox txtEclosao = (TextBox)GridView1.Rows[index].FindControl("TextBox10");
                decimal? eclosao = Convert.ToDecimal(txtEclosao.Text);

                TextBox txtOBS = (TextBox)GridView1.Rows[index].FindControl("TextBox13");
                string obs = txtOBS.Text;

                HATCHERY_EGG_DATA incubacao = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.ID == id)
                    .FirstOrDefault();

                string company = GetCompanyAndRegionByHatchLoc(incubacao.Hatch_loc, "company");
                string region = GetCompanyAndRegionByHatchLoc(incubacao.Hatch_loc, "region");
                string trackNO = "EXP" + dataPrd.ToString("yyMMdd");

                #endregion

                if (!ExisteFechamentoEstoque(company, incubacao.Hatch_loc, incubacao.Set_date) && !ExisteDEOSolicitacaoAjusteEstoqueAberto(incubacao.Hatch_loc))
                {
                    if ((incubacao.Status == "Importado") &&
                        ((incubacao.Lay_date != dataPrd) || (incubacao.Estimate != eclosao)))
                    {
                        #region Change values in WEB

                        incubacao.Horario = horario;
                        incubacao.Observacao = obs;
                        incubacao.Estimate = eclosao;
                        incubacao.Lay_date = dataPrd;
                        incubacao.Track_no = trackNO;

                        bdSQLServer.SaveChanges();

                        #endregion

                        #region Load Farm and Lote data

                        int start = incubacao.Flock_id.IndexOf("-") + 1;
                        int tamanho = incubacao.Flock_id.Length - start;

                        string lote = incubacao.Flock_id.Substring(start, tamanho);
                        string farm = incubacao.Flock_id.Substring(0, start - 1);

                        //int? qtdIncubada = bdSQLServer.HATCHERY_EGG_DATA
                        //    .Where(h => h.Company == incubacao.Company && h.Region == incubacao.Region &&
                        //        h.Location == incubacao.Location && h.Set_date == incubacao.Set_date &&
                        //        h.Hatch_loc == incubacao.Hatch_loc && h.Flock_id == incubacao.Flock_id &&
                        //        h.Lay_date == incubacao.Lay_date && h.Machine == incubacao.Machine &&
                        //        h.Track_no == incubacao.Track_no)
                        //    .Sum(h => h.Eggs_rcvd);

                        //decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(ddlIncubatorios.SelectedValue,
                        //    incubacao.Set_date, incubacao.Flock_id);

                        #endregion

                        #region Update FLIP

                        UpdateSetFLIP(company, region, farm, lote, incubacao.Lay_date, incubacao.Set_date, 
                            incubacao.Location, incubacao.Hatch_loc, 0, incubacao.Machine, trackNO, 0, 
                            incubacao.Observacao);

                        #endregion

                        #region Generate Tranf. Automatically if config

                        HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
                        hTA.Fill(flipDataSet.HATCHERY_CODES);
                        var hatchLocObj = flipDataSet.HATCHERY_CODES.Where(w => w.HATCH_LOC == incubacao.Hatch_loc).FirstOrDefault();
                        if (hatchLocObj.AUTO_TRANSF == "YES")
                            GenerateTransfAutomatically(company, region, incubacao.Location, incubacao.Hatch_loc, incubacao.Set_date, 
                                incubacao.Lay_date, incubacao.Flock_id, incubacao.Machine, incubacao.ClassOvo);

                        #endregion

                        #region Importação p/ Apolo - **** DESATIVADA ****

                        /*
                        string localEstq = incubacao.Hatch_loc;
                        if (incubacao.Hatch_loc.Equals("NM"))
                            localEstq = incubacao.ClassOvo;

                        ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
                                    .Where(ef => ef.USERFLIPCod == empresaEstoque)
                                    .FirstOrDefault();

                        LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                            .Where(l => l.USERCodigoFLIP == localEstq)
                            .FirstOrDefault();

                        PRODUTO produto = bdApolo.PRODUTO
                            .Where(p => p.ProdNomeAlt1 == incubacao.Variety)
                            .FirstOrDefault();

                        int posicaoHifen = incubacao.Flock_id.IndexOf("-") + 1;
                        int tamanho = incubacao.Flock_id.Length - posicaoHifen;
                        string flock = incubacao.Flock_id.Substring(posicaoHifen, tamanho);

                        CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
                                .Where(c => c.EmpCod == empresa.EmpCod
                                    && c.ProdCodEstr == produto.ProdCodEstr
                                    && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr && c.CtrlLoteNum == flock
                                    && c.CtrlLoteDataValid == incubacao.Lay_date
                                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == c.EmpCod && m.MovEstqChv == c.MovEstqChv &&
                                        m.MovEstqDataMovimento == incubacao.Set_date))
                                .FirstOrDefault();

                        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                                    .Where(l => l.EmpCod == lote.EmpCod && l.MovEstqChv == lote.MovEstqChv
                                        && l.ProdCodEstr == lote.ProdCodEstr && l.ItMovEstqSeq == lote.ItMovEstqSeq
                                        && l.LocArmazCodEstr == lote.LocArmazCodEstr)
                                    .FirstOrDefault();

                        ITEM_MOV_ESTQ itemMovEstq = bdApolo.ITEM_MOV_ESTQ
                                .Where(l => l.EmpCod == lote.EmpCod && l.MovEstqChv == lote.MovEstqChv
                                    && l.ProdCodEstr == lote.ProdCodEstr && l.ItMovEstqSeq == lote.ItMovEstqSeq)
                                .FirstOrDefault();

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(lote);

                        lote = service.InsereLote(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.TipoLancCod, itemMovEstq.ItMovEstqSeq,
                            itemMovEstq.ProdCodEstr, flock, dataPrd, qtdIncubada, "Saída", itemMovEstq.ItMovEstqUnidMedCod,
                            itemMovEstq.ItMovEstqUnidMedPos, locArmazItemMovEstq.LocArmazCodEstr);

                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);

                        bdApolo.SaveChanges();

                        bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);

                        //bdApolo.atualiza_saldoestqdata(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv, itemMovEstq.ProdCodEstr,
                        //    itemMovEstq.ItMovEstqSeq, itemMovEstq.ItMovEstqDataMovimento, "UPD");

                        */

                        #endregion
                    }

                    incubacao.Horario = horario;
                    incubacao.Observacao = obs;

                    bdSQLServer.SaveChanges();

                    #region Update Hatch Data WEB

                    AtualizaQtdeIncubadaNascimentoWEB(incubacao.Hatch_loc, incubacao.Set_date, incubacao.Flock_id,
                        incubacao.Machine, incubacao.ClassOvo);

                    #endregion

                    #region Update Data Table Screen

                    //GridView1.Rows[index].RowState = DataControlRowState.Normal;
                    GridView1.EditIndex = -1;
                    GridView1.DataBind();
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = Translate("Linha") + " " + (id).ToString() + " " + Translate("alterada com sucesso!");

                    #endregion
                }
                else
                {
                    if (ExisteFechamentoEstoque(company, incubacao.Hatch_loc, incubacao.Set_date))
                    {
                        #region If closed Egg Inventory, show warning message

                        //GridView1.Rows[index].RowState = DataControlRowState.Normal;
                        GridView1.EditIndex = -1;
                        GridView1.DataBind();
                        string responsavel = GetResponsableByHatchery(incubacao.Hatch_loc);
                        lblMensagem2.Visible = true;
                        lblMensagem2.Text = Translate("Estoque já fechado! Verifique com") + " "
                            + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                            + Translate("Caso não seja aberto, a incubação não pode ser alterada!");

                        #endregion
                    }
                    else if (ExisteDEOSolicitacaoAjusteEstoqueAberto(incubacao.Hatch_loc))
                    {
                        #region Se existir solicitação de ajuste em aberto, exibe mensagem

                        GridView1.EditIndex = -1;
                        GridView1.DataBind();
                        lblMensagem2.Visible = true;
                        lblMensagem2.Text = Translate("Existe Solicitação de Ajuste de Estoque em aberto! "
                                + "Para realizar qualquer lançamento ele deve ser aprovado ou excluído!");

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                #region If erros, show error message

                //GridView1.Rows[index].RowState = DataControlRowState.Normal;
                GridView1.EditIndex = -1;
                GridView1.DataBind();
                lblMensagem2.Visible = true;

                if (ex.Message.Length >= 35)
                {
                    if (ex.Message.Substring(0, 35) == "ORA-20102: Cannot update records!!!")
                    {
                        lblMensagem2.Text = "Erro na linha " + (id).ToString() + ": " + "Não existe esse Lote nesta Data de Produção no Estoque informada! Verifique!";
                    }
                    else
                    {
                        lblMensagem2.Text = "Erro na linha " + (id).ToString() + ": " + ex.Message;
                    }
                }

                #endregion
            }
        }

        // Verify duplicated lay date
        protected void TextBox5_TextChanged(object sender, EventArgs e)
        {
            int index = GridView1.EditIndex;
            int id = 0;

            Label lblID = (Label)GridView1.Rows[index].FindControl("Label1");
            id = Convert.ToInt32(lblID.Text);

            HATCHERY_EGG_DATA incubacao = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.ID == id)
                .FirstOrDefault();

            TextBox txtDataPrd = (TextBox)GridView1.Rows[index].FindControl("TextBox5");
            DateTime dataPrd = Convert.ToDateTime(txtDataPrd.Text);

            int existe = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Company == incubacao.Company && h.Region == incubacao.Region
                    && h.Location == incubacao.Location && h.Set_date == incubacao.Set_date
                    && h.Hatch_loc == incubacao.Hatch_loc && h.Flock_id == incubacao.Flock_id
                    && h.Lay_date == dataPrd && h.Machine == incubacao.Machine
                    && h.Posicao == incubacao.Posicao && h.ID != incubacao.ID)
                .Count();

            if (existe > 0)
            {
                lblMensagem2.Visible = true;
                lblMensagem2.Text = Translate("Lote") + " " + incubacao.Flock_id + " " + Translate("com a Data de Produção") + " " +
                    dataPrd.ToShortDateString() + " " + Translate("na posição") + " " + incubacao.Posicao.ToString() + " " 
                    + Translate("já existe") + "!";
                txtDataPrd.Focus();
            }
            else
            {
                lblMensagem2.Visible = false;
                lblMensagemOvosClass.Visible = false;
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int id = 0;
            Label lblID = (Label)e.Row.FindControl("Label15");

            if (lblID != null)
            {
                int.TryParse(lblID.Text, out id);

                HLBAPPEntities hlbapp = new HLBAPPEntities();
                HATCHERY_EGG_DATA incubacao = hlbapp.HATCHERY_EGG_DATA
                    .Where(w => w.ID == id).FirstOrDefault();

                if (incubacao != null)
                {
                    if (incubacao.ImportadoApolo.Equals("Estoque Futuro") &&
                        !incubacao.Status.Equals("Impotado"))
                    {
                        e.Row.BackColor = System.Drawing.Color.Yellow;
                        e.Row.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Sorting Table

        // Update Sorting Table
        protected void ibtnSaveClasOvo_Click(object sender, ImageClickEventArgs e)
        {
            #region Load variables output try

            int index = gdvClasOvos.EditIndex;
            string flockID = "";
            DateTime layDate = new DateTime();

            #endregion

            try
            {
                #region Load variables

                lblMensagemOvosClass.Visible = false;

                string hatchLoc = ddlIncubatorios.SelectedValue;
                DateTime setDate = Calendar1.SelectedDate;
                string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
                Label lblLoteCompleto = (Label)gdvClasOvos.Rows[index].FindControl("lblLoteCompleto");
                flockID = lblLoteCompleto.Text;
                Label lblDataProducao = (Label)gdvClasOvos.Rows[index].FindControl("lblDataProducao");
                layDate = Convert.ToDateTime(lblDataProducao.Text);
                TextBox txtOvosTrincados = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosTrincados");
                int crackedEggs = Convert.ToInt32(txtOvosTrincados.Text);
                TextBox txtOvosSujos = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosSujos");
                int dirtyEggs = Convert.ToInt32(txtOvosSujos.Text);
                TextBox txtOvosGrandes = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosGrandes");
                int bigEggs = Convert.ToInt32(txtOvosGrandes.Text);
                TextBox txtOvosPequenos = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosPequenos");
                int smallEggs = Convert.ToInt32(txtOvosPequenos.Text);
                TextBox txtOvosQuebrados = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosQuebrados");
                int brokenEggs = Convert.ToInt32(txtOvosQuebrados.Text);
                TextBox txtOvosComercio = (TextBox)gdvClasOvos.Rows[index].FindControl("txtOvosComercio");
                int salesEggs = Convert.ToInt32(txtOvosComercio.Text);

                #endregion

                if (!ExisteFechamentoEstoque(company, hatchLoc, setDate))
                {
                    #region Update Sorting Eggs

                    HLBAPPEntities hlbapp = new HLBAPPEntities();
                    var listaSortingDEOs = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.Granja == hatchLoc
                            && w.DataHoraCarreg == setDate
                            && w.LoteCompleto == flockID
                            && w.DataProducao == layDate
                            && w.TipoDEO == "Classificação de Ovos")
                        .ToList();

                    #region If not exists balance, show warning message

                    decimal totalQty = crackedEggs + dirtyEggs
                        + bigEggs + smallEggs + brokenEggs + salesEggs;
                    int balance = VerificaEstoqueWEB(layDate, flockID, (int)totalQty, ddlIncubatorios.SelectedValue, 
                        (int)listaSortingDEOs.Sum(s => s.QtdeOvos));
                    if (balance > 0)
                    {
                        lblMensagemOvosClass.Visible = true;
                        lblMensagemOvosClass.Text = Translate("Saldo insuficiente") + "! " + Translate("Verifique!");
                        return;
                    }

                    #endregion

                    foreach (var item in listaSortingDEOs)
                    {
                        if (item.TipoOvo == "HT") item.QtdeOvos = crackedEggs;
                        if (item.TipoOvo == "HS") item.QtdeOvos = dirtyEggs;
                        if (item.TipoOvo == "HG") item.QtdeOvos = bigEggs;
                        if (item.TipoOvo == "HC") item.QtdeOvos = smallEggs;
                        if (item.TipoOvo == "HB") item.QtdeOvos = brokenEggs;
                        if (item.TipoOvo == "HP") item.QtdeOvos = salesEggs;
                    }
                    
                    hlbapp.SaveChanges();

                    #endregion

                    #region Update Data Table Screen

                    GridView3.DataBind();
                    gdvClasOvos.EditIndex = -1;
                    gdvClasOvos.DataBind();
                    lblMensagemOvosClass.Visible = true;
                    lblMensagemOvosClass.Text = Translate("Lote") + " " + flockID + " - " + layDate.ToShortDateString() + " "
                        + Translate("alterada com sucesso!");

                    #endregion
                }
                else
                {
                    #region If closed Egg Inventory, show warning message

                    //GridView1.Rows[index].RowState = DataControlRowState.Normal;
                    GridView1.EditIndex = -1;
                    GridView1.DataBind();
                    string responsavel = GetResponsableByHatchery(hatchLoc);
                    lblMensagem2.Visible = true;
                    lblMensagem2.Text = Translate("Estoque já fechado! Verifique com") + " "
                        + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                        + Translate("Caso não seja aberto, a incubação não pode ser alterada!");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region If erros, show error message

                gdvClasOvos.EditIndex = -1;
                gdvClasOvos.DataBind();
                lblMensagemOvosClass.Visible = true;
                lblMensagemOvosClass.Text = "Erro no lote " + flockID + " - " + layDate.ToShortDateString() + ": " + ex.Message;
                
                #endregion
            }
        }

        // Delete Sorting Table
        protected void gdvClasOvos_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Load variables output try

            string flockID = "";
            DateTime layDate = new DateTime();

            #endregion

            try
            {
                #region Load Variables

                lblMensagemOvosClass.Visible = false;

                string hatchLoc = ddlIncubatorios.SelectedValue;
                string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
                DateTime setDate = Calendar1.SelectedDate;
                Label lblLoteCompleto = (Label)gdvClasOvos.Rows[gdvClasOvos.SelectedIndex].Cells[3].FindControl("lblLoteCompleto");
                flockID = lblLoteCompleto.Text;
                Label lblDataProducao = (Label)gdvClasOvos.Rows[gdvClasOvos.SelectedIndex].Cells[5].FindControl("lblDataProducao");
                layDate = Convert.ToDateTime(lblDataProducao.Text);
                
                #endregion

                if (!ExisteFechamentoEstoque(company, hatchLoc, setDate))
                {
                    #region Delete Sorting Eggs by FlockID and LayDate

                    HLBAPPEntities hlbapp = new HLBAPPEntities();
                    var listaSortingEggs = hlbapp.LayoutDiarioExpedicaos
                        .Where(w => w.Granja == hatchLoc
                            && w.DataHoraCarreg == setDate
                            && w.LoteCompleto == flockID
                            && w.DataProducao == layDate
                            && w.TipoDEO == "Classificação de Ovos")
                        .ToList();

                    foreach (var item in listaSortingEggs)
                    {
                        hlbapp.LayoutDiarioExpedicaos.DeleteObject(item);
                    }

                    hlbapp.SaveChanges();

                    #endregion

                    #region Refresh Page Data

                    GridView3.DataBind();
                    gdvClasOvos.DataBind();
                    lblMensagemOvosClass.Visible = true;
                    lblMensagemOvosClass.Text = Translate("Lote") + " " + flockID + " - " + layDate.ToShortDateString() +
                        " " + Translate("excluída com sucesso!");

                    #endregion
                }
                else
                {
                    #region Show warning message

                    string responsavel = GetResponsableByHatchery(hatchLoc);
                    lblMensagem.Visible = true;
                    lblMensagem.Text = Translate("Estoque já fechado! Verifique com") + " "
                        + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                        + Translate("Caso não seja aberto, a exclusão não pode ser realizada!");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Show error message

                gdvClasOvos.EditIndex = -1;
                gdvClasOvos.DataBind();
                lblMensagemOvosClass.Visible = true;
                lblMensagemOvosClass.Text = "Erro no lote " + flockID + " - " + layDate.ToShortDateString() + ": " + ex.Message;

                #endregion
            }
        }

        #endregion

        #region Page Components

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            lblMensagemOvosClass.Visible = false;

            Session["setDate"] = Calendar1.SelectedDate;

            DateTime data = Calendar1.SelectedDate;

            //AtualizaFLIP(data);
            RefreshFLIP(ddlIncubatorios.SelectedValue, data, false);
            AtualizaNascimentoWEB(ddlIncubatorios.SelectedValue, data);
            //VerificaImportacaoApolo(data);
            AtualizaTotais();

            VerificaNaoImportados();

            GridView1.DataBind();

            AjustaTelaIncubacaoManualPlanalto();

            FormView1.ChangeMode(FormViewMode.ReadOnly);
        }

        protected void ddlIncubatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime data = Calendar1.SelectedDate;

            //VerificaImportacaoApolo(data);
            //AtualizaFLIP(data);
            RefreshFLIP(ddlIncubatorios.SelectedValue, data, false);
            AtualizaNascimentoWEB(ddlIncubatorios.SelectedValue, data);

            VerificaNaoImportados();

            AjustaTelaIncubacaoManualPlanalto();

            if (GetCompanyAndRegionByHatchLoc(ddlIncubatorios.SelectedValue, "CLAS_EGG") != "NO")
            {
                Label6.Visible = false;
                txt_SetterDe.Visible = false;
                Label7.Visible = false;
                txt_SetterPara.Visible = false;
                btn_AtualizaSetter.Visible = false;
                ddlClassOvos.Visible = true;

                #region Load Eggs Type

                ddlClassOvos.Items.Clear();
                ddlClassOvos.Items.Add(new ListItem { Text = "(Todos)", Value = "T", Selected = true });

                Models.HLBAPP.HLBAPPEntities1 hlbapp = new Models.HLBAPP.HLBAPPEntities1();
                var listaTipoOvos = hlbapp.TIPO_CLASSFICACAO_OVO
                    .Where(w => w.Unidade == ddlIncubatorios.SelectedValue && w.AproveitamentoOvo == "Incubável").ToList();

                foreach (var item in listaTipoOvos)
                {
                    ddlClassOvos.Items.Add(new ListItem { Text = item.DescricaoTipo, Value = item.CodigoTipo, Selected = false });
                }

                #endregion
            }
            else
            {
                Label6.Visible = true;
                txt_SetterDe.Visible = true;
                Label7.Visible = true;
                txt_SetterPara.Visible = true;
                btn_AtualizaSetter.Visible = true;
                ddlClassOvos.Visible = false;
            }

            string hatchLoc = ddlIncubatorios.SelectedValue;
            string clasInc = GetCompanyAndRegionByHatchLoc(hatchLoc, "CLAS_INC");
            if (clasInc == "YES")
                pnlTabelaOvosClassificados.Visible = true;
            else
                pnlTabelaOvosClassificados.Visible = false;

            FormView1.ChangeMode(FormViewMode.ReadOnly);
        }

        #endregion

        #region Event Methods

        public void GenerateTransfAutomatically(string company, string region, string location, 
            string hatchLoc, DateTime setDate, DateTime layDate, string flockID, 
            string setter, string classOvo)
        {
            #region Insert / Update Transf. in WEB

            if (classOvo != "")
                GenerateTransfAutomaticallyWEB(hatchLoc, setDate, flockID, setter, classOvo);
            else
            {
                var listHED = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(w => w.Hatch_loc == hatchLoc && w.Set_date == setDate
                        && w.Flock_id == flockID && w.Machine == setter)
                    .ToList();

                foreach (var item in listHED)
                {
                    GenerateTransfAutomaticallyWEB(hatchLoc, setDate, flockID, setter, item.ClassOvo);
                }
            }


            #endregion

            #region Insert / Update Transf in FLIP

            SetTransfDataFLIP(company, region, location, hatchLoc, setDate, flockID, layDate, setter); 

            #endregion
        }

        #endregion

        #region BD Methods

        #region FLIP

        #region New Methods

        #region Set Methods

        public void RefreshFLIP(string hatchLoc, DateTime setDate, bool updateManual)
        {
            try
            {
                #region Load data components

                DateTime data = Convert.ToDateTime("01/07/2013");
                string incubatorio = hatchLoc;
                string company = GetCompanyAndRegionByHatchLoc(hatchLoc, "company");
                string region = GetCompanyAndRegionByHatchLoc(hatchLoc, "region");
                string location = GetLocation(company, incubatorio);

                #endregion

                if ((setDate >= data) ||
                    ((setDate == Convert.ToDateTime("19/06/2013")) && (incubatorio == "CH")) ||
                    ((setDate == Convert.ToDateTime("20/11/2013")) && (incubatorio == "TB"))) // erro de fechamento, por isso o dia 19/06.
                {
                    #region Load Hatchery Egg Data Web

                    var lista = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == company && h.Region == region
                            && h.Set_date == setDate && h.Status == "Importado" 
                            && h.Hatch_loc == incubatorio)// && h.Flock_id == "CTP03-CT3003BWN")
                        .GroupBy(h => new
                        {
                            h.Company,
                            h.Region,
                            h.Location,
                            h.Set_date,
                            h.Hatch_loc,
                            h.Flock_id,
                            h.Lay_date,
                            h.Machine,
                            h.Track_no//,
                            //h.ClassOvo
                        })
                        .Select(h => new //HATCHERY_EGG_DATA
                            {
                                type = h.Key,
                                soma = h.Sum(x => x.Eggs_rcvd),
                                estimate = h.Max(x => x.Estimate),
                                observacao = h.Max(x => x.Observacao)
                            })
                        .ToList();

                    #endregion

                    #region If exists in FLIP and not in WEB, update data

                    int existeIncubacao = ExistsHatcheryEggDataForSetDate(company, region, location, setDate, incubatorio);
                    int qtdLista = lista.Count;

                    // Verifica se existe mais no FLIP do que no HLBAPP. 
                    // Caso exista, serão deletados, pois no HLBAPP que é o correto.
                    if (qtdLista != existeIncubacao)
                    {
                        DeleteFLIPIfnotExistsWEB(company, region, location, setDate, incubatorio);
                    }

                    #endregion

                    foreach (var item in lista)
                    {
                        #region Load Data about Hatching Data

                        decimal qtdOvos = GetQtySettedEggs(item.type.Company, item.type.Region,
                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id,
                            item.type.Lay_date, item.type.Machine, item.type.Track_no);

                        int existeInc = ExistsHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                            item.type.Track_no);

                        #endregion

                        if ((qtdOvos != item.soma) || (existeInc == 0) || (updateManual))
                        {
                            #region Load Farm and Lote data

                            int start = item.type.Flock_id.IndexOf("-") + 1;
                            int tamanho = item.type.Flock_id.Length - start;

                            string lote = item.type.Flock_id.Substring(start, tamanho);
                            string farm = item.type.Flock_id.Substring(0, start - 1);

                            #endregion

                            #region Update FLIP

                            bool imported = UpdateSetFLIP(company, region, farm, lote, item.type.Lay_date, item.type.Set_date, 
                                item.type.Location, item.type.Hatch_loc, Convert.ToDecimal(item.soma), item.type.Machine, 
                                item.type.Track_no, Convert.ToDecimal(item.estimate), item.observacao);

                            #endregion

                            #region Generate Tranf. Automatically if config

                            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
                            hTA.Fill(flipDataSet.HATCHERY_CODES);
                            var hatchLocObj = flipDataSet.HATCHERY_CODES.Where(w => w.HATCH_LOC == incubatorio).FirstOrDefault();
                            if (hatchLocObj.AUTO_TRANSF == "YES")
                            {
                                GenerateTransfAutomatically(item.type.Company, item.type.Region, item.type.Location, item.type.Hatch_loc,
                                    item.type.Set_date, item.type.Lay_date,
                                    item.type.Flock_id, item.type.Machine, "");
                            }

                            #endregion

                            #region Check Hatchery Egg Data in WEB if imported in FLIP

                            string importadoFLIP = "Não";
                            if (imported) importadoFLIP = "Sim";

                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                    && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                    && h.Location == item.type.Location && h.Region == item.type.Region
                                    && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = importadoFLIP;
                            }

                            bdSQLServer.SaveChanges();

                            #endregion

                            #region MODO ANTIGO - DESATIVADO

                            /*
                            decimal qtdOvosSet = Convert.ToDecimal(eggInvData.QtdOvosByStatus(
                                lote, "S", item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            if (qtdOvosSet < item.soma)
                            {
                                if (qtdOvosSet == 0)
                                {
                                    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                        farm, lote,
                                        item.type.Track_no, item.type.Lay_date, item.soma, "S", null, null, null, null, null, null, null, null,
                                        item.type.Hatch_loc, null);
                                }
                                else
                                {
                                    decimal? qtdUpdate = qtdOvosSet + item.soma;

                                    eggInvData.UpdateQueryEggs(qtdUpdate, item.type.Company, item.type.Region, item.type.Location,
                                            farm, lote,
                                            item.type.Track_no, item.type.Lay_date, "S", item.type.Hatch_loc);
                                }
                            }

                            hatcheryEggData.Delete(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc,
                                item.type.Flock_id, item.type.Lay_date,
                                item.type.Machine, item.type.Track_no);

                            //int existeHLBAPP = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(item.type.Company,
                            //    item.type.Region, item.type.Location, item.type.Set_date.ToString("dd/MM/yyyy"),
                            //    item.type.Hatch_loc, item.type.Flock_id));

                            //if (existeHLBAPP == 0)
                            //{
                            //    hatcheryFlockData.Delete(item.type.Company,
                            //    item.type.Region, item.type.Location, item.type.Set_date,
                            //    item.type.Hatch_loc, item.type.Flock_id);
                            //}

                            // AJUSTE EGG INVENTORY PARA INCLUIR INCUBAÇÃO

                            int existeAjuste = Convert.ToInt32(eggInvData.ScalarQueryOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            if (existeAjuste == 0)
                            {
                                eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                    farm, lote,
                                    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                    item.type.Hatch_loc, null);
                            }
                            else
                            {
                                int qtdeOvosAjuste = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                if (qtdeOvosAjuste < item.soma)
                                {
                                    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                        farm, lote,
                                        item.type.Track_no, item.type.Lay_date, "O", item.type.Hatch_loc);
                                }
                            }

                            int existe = Convert.ToInt32(eggInvData.ScalarQueryOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            //if (existe == 0)
                            if (existe > 0)
                            {
                                //    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                //    item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                //    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                //    item.type.Hatch_loc, null);
                                //}

                                int qtdeOvos = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                if ((qtdeOvos - item.soma) >= 0)
                                {
                                    //    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                    //    item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                    //    item.type.Track_no, item.type.Lay_date.ToString("dd/MM/yyyy"), "O", item.type.Hatch_loc);
                                    //}

                                    decimal existeSetDay = Convert.ToDecimal(setDayData.ExisteSetDayData(item.type.Set_date, item.type.Hatch_loc));

                                    if (existeSetDay == 0)
                                    {
                                        decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(ddlIncubatorios.SelectedValue)) + 1;

                                        setDayData.InsertQuery("HYBR", "BR", location, item.type.Set_date, item.type.Hatch_loc, sequencia);
                                    }

                                    existe = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                                        item.type.Track_no));

                                    if (existe == 1)
                                    {
                                        hatcheryEggData.Delete(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                            item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine, item.type.Track_no);
                                    }

                                    existe = Convert.ToInt32(hatcheryFlockData.ExisteHatcheryFlockData(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id));

                                    if (existe == 0)
                                    {
                                        //hatcheryFlockData.Delete(item.type.Company, item.type.Region, item.type.Location,
                                        //    item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id);
                                        hatcheryFlockData.InsertQuery(item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.estimate);
                                    }
                                    // 14/08/2014 - Ocorrência 99 - APONTES
                                    // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
                                    // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
                                    // o trigger de atualização da idade executar.
                                    else
                                    {
                                        decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(
                                            ddlIncubatorios.SelectedValue,
                                            item.type.Set_date, item.type.Flock_id);

                                        hatcheryFlockData.UpdateEstimate(mediaIncubacao, item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id);
                                    }

                                    hatcheryEggData.Insert(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                        item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.soma, null,
                                        item.type.Machine, item.type.Track_no, null, null, null, null, null, null,
                                        null, null, item.observacao, Session["login"].ToString());

                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                            && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                            && h.Location == item.type.Location && h.Region == item.type.Region
                                            && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Sim";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                                else
                                {
                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                            && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                            && h.Location == item.type.Location && h.Region == item.type.Region
                                            && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Não";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                            }
                             * */

                            #endregion
                        }
                        else
                        {
                            #region Check Hatchery Egg Data in WEB as imported in FLIP

                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                    && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                    && h.Location == item.type.Location && h.Region == item.type.Region
                                    && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = "Sim";
                            }

                            bdSQLServer.SaveChanges();

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region If erros, show error message

                lblMensagem3.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem3.Text = "Erro ao Incubar: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem3.Text = "Erro ao Incubar: " + ex.Message;

                #endregion
            }
        }

        public void AjustaEggInvFLIP(string incubatorio, string lote, DateTime dataProducao, decimal qtdeOvos)
        {
            DateTime dataIncubacao = Calendar1.SelectedDate;

            LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
                    .Where(l => l.USERCodigoFLIP == incubatorio)
                    .FirstOrDefault();

            flocksServico.FillByFlockIDAndLocation(flipDataSetServico.FLOCKS, lote, locArmaz.USERGeracaoFLIP);
            string farmID = flipDataSetServico.FLOCKS[0].FARM_ID;
            string trackNO = "EXP" + dataProducao.ToString("yyMMdd");

            eggInvData.Delete("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, lote, trackNO, dataProducao,
                        "O", incubatorio);

            if (incubatorio != "NM" ||
                (incubatorio == "NM" && dataIncubacao >= Convert.ToDateTime("01/01/2017")))
            {
                #region Cadastro Correto EGG_INV

                //var lista = bdApolo.CTRL_LOTE_LOC_ARMAZ
                //    .Where(c => c.CtrlLoteLocArmazQtdSaldo > 0 && c.EmpCod == empresaEstoque
                //        && c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao
                //        && bdApolo.LOC_ARMAZ.Any(l => l.LocArmazCodEstr == c.LocArmazCodEstr
                //            && ((incubatorio == "CH" && !l.USERCodigoFLIP.Equals("SB") && !l.USERCodigoFLIP.Equals("PH")
                //                && !l.USERCodigoFLIP.Equals("TB") && !l.USERCodigoFLIP.Equals("NM")
                //                && !l.USERCodigoFLIP.Equals("PL"))
                //            || (incubatorio == "PH" && (l.USERCodigoFLIP.Equals("SB") || l.USERCodigoFLIP.Equals("PH")))
                //            || (incubatorio == "TB" && l.USERCodigoFLIP.Equals("TB"))
                //            || (incubatorio == "NM" && (l.USERCodigoFLIP.Equals("NM") || l.USERCodigoFLIP.Equals("PL")
                //                 || l.USERCodigoFLIP.Equals("T0") || l.USERCodigoFLIP.Equals("T1")
                //                 || l.USERCodigoFLIP.Equals("T2"))))))
                //    .ToList();

                var lista = bdSQLServer.CTRL_LOTE_LOC_ARMAZ_WEB
                    .Where(c => //c.Qtde > 0 && 
                        c.LoteCompleto == lote && c.DataProducao == dataProducao
                        && ((incubatorio == "CH" && !c.Local.Equals("SB") && !c.Local.Equals("PH")
                                && !c.Local.Equals("TB") && !c.Local.Equals("NM")
                                && !c.Local.Equals("PL"))
                            || (incubatorio == "PH" && (c.Local.Equals("SB") || c.Local.Equals("PH")))
                            || (incubatorio == "TB" && c.Local.Equals("TB"))
                            || (incubatorio == "NM" && (c.Local.Equals("NM") || c.Local.Equals("PL")
                                 || c.Local.Equals("T0") || c.Local.Equals("T1")
                                 || c.Local.Equals("T2")))))
                    .ToList();

                foreach (var item in lista)
                {
                    int existe = Convert.ToInt32(eggInvDataServico
                        .ScalarQueryOpen2(item.LoteCompleto, item.DataProducao, locArmaz.USERCodigoFLIP));

                    decimal? qtdeClassificado = bdSQLServer.CTRL_LOTE_LOC_ARMAZ_WEB
                        .Where(w => w.LoteCompleto == item.LoteCompleto && w.DataProducao == item.DataProducao
                            && (w.Local == incubatorio || bdSQLServer.TIPO_CLASSFICACAO_OVO_02
                                .Any(t => t.Unidade == incubatorio && t.CodigoTipo == w.Local && t.AproveitamentoOvo == "Incubável")))
                        .Sum(s => s.Qtde);

                    if (existe == 0)
                    {
                        //decimal? qtd = item.Qtde;
                        decimal? qtd = qtdeClassificado + qtdeOvos;

                        eggInvDataServico.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, item.LoteCompleto,
                            trackNO, item.DataProducao, qtd, "O", null,
                            null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
                    }
                    else
                    {
                        eggInvDataServico.FillByFlockLayDateStatus(flipDataSetServico.EGGINV_DATA,
                            item.LoteCompleto, "O", item.DataProducao);

                        var lista2 = flipDataSetServico.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
                            && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

                        foreach (var item2 in lista2)
                        {
                            //decimal? qtd = item2.EGG_UNITS + item.Qtde;
                            decimal? qtd = item2.EGG_UNITS + qtdeClassificado + qtdeOvos;
                            eggInvDataServico.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
                                farmID, item.LoteCompleto, trackNO, item.DataProducao, "O", locArmaz.USERCodigoFLIP);
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region Lançamento para Correção dos Dados da Planalto até 31/12/2016 - Solicitado por Davi Nogueira

                int existe = Convert.ToInt32(eggInvDataServico
                        .ScalarQueryOpen2(lote, dataProducao, locArmaz.USERCodigoFLIP));

                if (existe == 0)
                {
                    decimal? qtd = qtdeOvos;

                    eggInvDataServico.Insert("HYBR", "BR", locArmaz.USERGeracaoFLIP, farmID, lote,
                        trackNO, dataProducao, qtd, "O", null,
                        null, null, null, null, null, null, null, locArmaz.USERCodigoFLIP, null);
                }
                else
                {
                    eggInvDataServico.FillByFlockLayDateStatus(flipDataSetServico.EGGINV_DATA,
                        lote, "O", dataProducao);

                    var lista2 = flipDataSetServico.EGGINV_DATA.Where(e => e.LOCATION == locArmaz.USERGeracaoFLIP
                        && e.HATCH_LOC == locArmaz.USERCodigoFLIP).ToList();

                    foreach (var item2 in lista2)
                    {
                        decimal? qtd = item2.EGG_UNITS + qtdeOvos;
                        eggInvDataServico.UpdateQueryEggs(qtd, "HYBR", "BR", locArmaz.USERGeracaoFLIP,
                            farmID, lote, trackNO, dataProducao, "O", locArmaz.USERCodigoFLIP);
                    }
                }

                #endregion
            }
        }

        public bool UpdateSetFLIP(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO, 
            decimal estimate, string obs)
        {
            bool imported = false;

            if (company == "HYCL")
            {
                // Chile
                imported = UpdateSetFLIPCL(company, region, farmID, flockID, layDate, setDate, location, hatchLoc, eggUnits, 
                    machine, trackNO, estimate, obs);
            }
            else if (company == "HYBR")
            {
                // Brasil
                imported = UpdateSetFLIPBR(company, region, farmID, flockID, layDate, setDate, location, hatchLoc, eggUnits, 
                    machine, trackNO, estimate, obs);
            }
            else if (company == "HYCO")
            {
                // Colombia / Ecuador

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA = 
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.EGGINV_DATATableAdapter eiTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.EGGINV_DATATableAdapter();

                fdTA.FillByFlockAndTrxDate(fdDT, flockID, layDate);

                #region Delete Qty

                foreach (var item in fdDT)
                {
                    imported = UpdateSetFLIPHC(company, region, farmID, item.FLOCK_ID, layDate, setDate, location, hatchLoc, 0,
                        machine, trackNO, estimate, obs);
                    //if (!imported) return imported;
                }

                #endregion

                #region Insert / Update Qty

                int balance = (int)eggUnits; //100 = 50 + 25 + 50
                foreach (var item in fdDT)
                {
                    if (balance > 0)
                    {
                        //var listEggInv = eiTA.GetDataFlockAndTrxDate(company, region, location, farmID, item.FLOCK_ID, layDate);
                        var listEggInv = eiTA.GetDataFlockAndTrxDate02(company, farmID, item.FLOCK_ID, layDate);
                        var eggInv = listEggInv.Where(w => w.STATUS == "O").FirstOrDefault();
                        if (eggInv != null)
                        {
                            int settQty = 0;
                            if (balance > eggInv.EGG_UNITS)
                                settQty = (int)eggInv.EGG_UNITS;
                            else
                                settQty = balance;

                            balance = balance - settQty;

                            imported = UpdateSetFLIPHC(company, region, farmID, item.FLOCK_ID, layDate, setDate, location, hatchLoc, settQty,
                                machine, trackNO, estimate, obs);

                            if (!imported && balance == 0) return imported;
                        }
                    }
                }

                #endregion
            }

            return imported;
        }

        public bool UpdateSetFLIPBR(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO, 
            decimal estimate, string obs)
        {
            bool imported = false;

            AjustaEggInvFLIP(hatchLoc, flockID, layDate, eggUnits);

            decimal existe = Convert.ToDecimal(setDayData.ExisteSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(hatchLoc)) + 1;

                setDayData.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, 
                layDate, machine, trackNO);

            if (existe > 0)
            {
                eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                    setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                hatcheryEggData.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, 
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hatcheryFlockData.ExisteHatcheryFlockData(company, region,
                location, setDate, hatchLoc, farmID + "-" + flockID));

            if (existe == 0)
            {
                hatcheryFlockData.InsertQuery(company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate, 
                    farmID + "-" + flockID);

                hatcheryFlockData.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hatcheryEggData.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate, 
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs, 
                    Session["login"].ToString());

                imported = true;
            }

            return imported;
        }

        public bool UpdateSetFLIPCL(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits, string machine, string trackNO, 
            decimal estimate, string obs)
        {
            bool imported = false;

            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.SETDAY_DATATableAdapter sTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.SETDAY_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();

            decimal existe = Convert.ToDecimal(sTA.ExistsSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(sTA.LastSequenceByHatchLoc(hatchLoc)) + 1;
                sTA.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc, 
                farmID + "-" + flockID, layDate, machine, trackNO);

            if (existe > 0)
            {
                eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                    setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                hedTA.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hfdTA.ExistsHFD(company, region, location, setDate, hatchLoc, 
                farmID + "-" + flockID));

            if (existe == 0)
            {
                hfdTA.InsertQuery(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate,
                    farmID + "-" + flockID);

                hfdTA.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hedTA.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate,
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs,
                    Session["login"].ToString());

                imported = true;
            }

            return imported;
        }

        public bool UpdateSetFLIPHC(string company, string region, string farmID, string flockID, DateTime layDate,
            DateTime setDate, string location, string hatchLoc, decimal eggUnits,
            string machine, string trackNO, decimal estimate, string obs)
        {
            bool imported = false;

            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.SETDAY_DATATableAdapter sTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.SETDAY_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();

            decimal existe = Convert.ToDecimal(sTA.ExistsSetDayData(setDate, hatchLoc));

            if (existe == 0)
            {
                decimal sequencia = Convert.ToDecimal(sTA.LastSequenceByHatchLoc(hatchLoc)) + 1;
                sTA.InsertQuery(company, region, location, setDate, hatchLoc, sequencia);
            }

            existe = 0;

            // Insere / Atualiza Incubação
            existe = ExistsHatcheryEggDataAll(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID, layDate, machine, trackNO);

            if (existe > 0)
            {
                if (eggUnits > 0)
                {
                    eggUnits = eggUnits + GetQtySettedEggs(company, region, location,
                        setDate, hatchLoc, farmID + "-" + flockID, layDate, machine, trackNO);
                }
                hedTA.Delete(company, region, location, setDate, hatchLoc, farmID + "-" + flockID,
                    layDate, machine, trackNO);
            }

            existe = 0;

            // Verifica se existe Dados do Nascimento
            existe = Convert.ToDecimal(hfdTA.ExistsHFD(company, region, location, setDate, hatchLoc,
                farmID + "-" + flockID));

            if (existe == 0)
            {
                hfdTA.InsertQuery(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, estimate);
            }
            // 14/08/2014 - Ocorrência 99 - APONTES
            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
            // o trigger de atualização da idade executar.
            else
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate,
                    farmID + "-" + flockID);

                hfdTA.UpdateEstimate(mediaIncubacao, company, region, location, setDate,
                    hatchLoc, farmID + "-" + flockID);
            }

            if (eggUnits > 0)
            {
                hedTA.Insert(company, region, location, setDate, hatchLoc, farmID + "-" + flockID, layDate,
                    eggUnits, "", machine, trackNO, null, null, null, null, null, null, null, null, obs,
                    Session["login"].ToString());

                imported = true;
            }

            return imported;
        }

        public void DeleteByHatchLocAndSetDate(string company, DateTime setDate, string hatchLoc)
        {
            if (company == "HYBR")
            {
                hatcheryEggData.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                hedTA.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                hedTA.DeleteByHatchLocAndSetDate(setDate, hatchLoc);
            }
        }

        public void DeleteFLIPIfnotExistsWEB(string company, string region,
            string location, DateTime setDate, string hatchLoc)
        {
            // Verifica se existe mais no FLIP do que no HLBAPP. 
            // Caso exista, serão deletados, pois no HLBAPP que é o correto.

            if (company == "HYBR")
            {
                #region HYBR

                DeleteByHatchLocAndSetDate(company, setDate, ddlIncubatorios.SelectedValue);

                var listaFLIP = hatcheryEggData.GetDataBySetDate(company, region, location, setDate,
                    ddlIncubatorios.SelectedValue);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        hatcheryEggData.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        existeHLBAPP = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            hatcheryFlockData.Delete(item.COMPANY,
                                item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                var listaFLIP = hedTA.GetDataBySetDate(company, region, location, setDate, hatchLoc);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        hedTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        existeHLBAPP = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            hfdTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                var listaFLIP = hedTA.GetDataBySetDate(company, region, location, setDate, hatchLoc);

                foreach (var item in listaFLIP)
                {
                    int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                            h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                            h.Hatch_loc == item.HATCH_LOC && 
                            //h.Flock_id == item.FLOCK_ID &&
                            h.Flock_id == (item.FLOCK_ID.Substring(0, 12) + item.FLOCK_ID.Substring(13, 3)) &&
                            h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                            h.Track_no == item.TRACK_NO)
                        .Count();

                    if (existeHLBAPP == 0)
                    {
                        hedTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                        existeHLBAPP = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(item.COMPANY,
                            item.REGION, item.LOCATION, item.SET_DATE,
                            item.HATCH_LOC, item.FLOCK_ID));

                        if (existeHLBAPP == 0)
                        {
                            #region HATCHERY_TRAN_DATA - Se tem no FLIP e não no Web, deleta do FLIP

                            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA = new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();

                            string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) + hatchLoc + item.FLOCK_ID;

                            var listaItensDeletar = htdTA.GetDataByEggKey(eggKey);
                            foreach (var transfer in listaItensDeletar)
                            {
                                var existeTransfWeb = bdSQLServer.HATCHERY_TRAN_DATA
                                    .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                                        && w.Flock_id == item.FLOCK_ID)
                                .FirstOrDefault();

                                if (existeTransfWeb == null)
                                    htdTA.Delete(eggKey, transfer.LAY_DATE, transfer.MACHINE, transfer.HATCHER, transfer.TRACK_NO);
                            }

                            #endregion

                            hfdTA.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                item.HATCH_LOC, item.FLOCK_ID);
                        }
                    }
                }

                #endregion
            }
        }

        public void DeleteHatcheryEggDataLine(int exists, string company, string region, string location,
            string hatchLoc, DateTime setDate, string flock, string flockID, DateTime layDate, string machine,
            string trackNO, decimal qtyEggs)
        {
            if (company == "HYBR")
            {
                #region HYBR

                AjustaEggInvFLIP(hatchLoc, flock, layDate, 0);

                if (exists == 0)
                {
                    hatcheryEggData.Delete(company, region, location, setDate, hatchLoc, flockID, layDate, machine, trackNO);

                    // Verifica se existe Dados do Nascimento
                    exists = 0;
                    exists = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(company, region, location, setDate,
                        hatchLoc, flockID));
                    if (exists == 0)
                    {
                        hatcheryFlockData.Delete(company, region, location, setDate, hatchLoc, flockID);
                    }
                }
                else
                {
                    decimal qtdFlip = Convert.ToDecimal(hatcheryEggData.QtdOvos(company, region, location, setDate, 
                        hatchLoc, flockID, layDate, machine, trackNO));
                    decimal qtdOvosUpdFLIP = 0;
                    if (qtdFlip > 0)
                    {
                        hatcheryEggData.Delete(company, region, location, setDate, hatchLoc, flockID, layDate, machine, trackNO);
                        if ((qtdFlip - qtyEggs) >= 0)
                            qtdOvosUpdFLIP = qtdFlip - qtyEggs;
                        else
                            qtdOvosUpdFLIP = qtdFlip;
                        hatcheryEggData.Insert(company, region, location, setDate, ddlIncubatorios.SelectedValue, flockID, layDate, 
                            qtdOvosUpdFLIP, "", machine, trackNO, null, null, null, null, null, null, null, null, null, 
                            Session["login"].ToString());
                    }
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();

                if (exists == 0)
                {
                    hedTA.Delete(company, region, location, setDate, hatchLoc, flockID, layDate, machine, trackNO);

                    // Verifica se existe Dados do Nascimento
                    exists = 0;
                    exists = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(company, region, location, setDate,
                        hatchLoc, flockID));
                    if (exists == 0)
                    {
                        hfdTA.Delete(company, region, location, setDate, hatchLoc, flockID);
                    }
                }
                else
                {
                    decimal qtdFlip = Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                        hatchLoc, flockID, layDate, machine, trackNO));
                    decimal qtdOvosUpdFLIP = 0;
                    if (qtdFlip > 0)
                    {
                        hedTA.Delete(company, region, location, setDate, hatchLoc, flockID, layDate, machine, trackNO);
                        if ((qtdFlip - qtyEggs) >= 0)
                            qtdOvosUpdFLIP = qtdFlip - qtyEggs;
                        else
                            qtdOvosUpdFLIP = qtdFlip;
                        hedTA.Insert(company, region, location, setDate, ddlIncubatorios.SelectedValue, flockID, layDate,
                            qtdOvosUpdFLIP, "", machine, trackNO, null, null, null, null, null, null, null, null, null,
                            Session["login"].ToString());
                    }
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();
                fdTA.FillByFlockAndTrxDate(fdDT, flock, layDate);

                if (exists == 0)
                {
                    foreach (var item in fdDT)
                    {
                        hedTA.Delete(company, region, location, setDate, hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, machine, trackNO);

                        // Verifica se existe Dados do Nascimento
                        exists = 0;
                        exists = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForFlockData(company, region, location, setDate,
                            hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID));
                        if (exists == 0)
                        {
                            hfdTA.Delete(company, region, location, setDate, hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID);
                        }
                    }
                }
                else
                {
                    int balance = (int)qtyEggs; //100 = 50 + 25 + 50
                    foreach (var item in fdDT)
                    {
                        if (balance > 0)
                        {
                            decimal qtdFlip = Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                                hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, machine, trackNO));
                            decimal qtdOvosUpdFLIP = 0;
                            if (qtdFlip > 0)
                            {
                                hedTA.Delete(company, region, location, setDate, hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, machine, trackNO);
                                if ((qtdFlip - balance) >= 0)
                                    qtdOvosUpdFLIP = qtdFlip - balance;
                                else
                                    qtdOvosUpdFLIP = qtdFlip;
                                hedTA.Insert(company, region, location, setDate, ddlIncubatorios.SelectedValue, 
                                    item.FARM_ID + "-" + item.FLOCK_ID, layDate,
                                    qtdOvosUpdFLIP, "", machine, trackNO, null, null, null, null, null, null, null, null, null,
                                    Session["login"].ToString());
                            }
                            balance = balance - (int)qtdOvosUpdFLIP;
                        }
                    }
                }

                #endregion
            }
        }

        public void ChangeMachine(string company, string region, string location, DateTime setDate,
            string hatchLoc, string flockID, DateTime layDate, string setterFROM, string setterTO, 
            string trackNO, int qtyEggs)
        {
            if (company == "HYBR")
            {
                #region HYBR

                decimal? existeMesmoSetter = hatcheryEggData.ExisteHatcheryEggData2(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                decimal? existeAntigoSetter = hatcheryEggData.ExisteHatcheryEggData2(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterFROM, trackNO);

                if ((existeMesmoSetter > 0) && (existeAntigoSetter > 0))
                {
                    int qtdOvosAntigo = Convert.ToInt32(hatcheryEggData.QtdOvos(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO));

                    hatcheryEggData.UpdateEggs(qtdOvosAntigo + qtyEggs, company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                    hatcheryEggData.DeleteQuery1(company, region, location, setDate, hatchLoc, flockID, 
                        layDate, setterFROM, trackNO);
                }
                else
                {
                    hatcheryEggData.UpdateMachine(setterTO, company, region, location, setDate, hatchLoc, 
                        flockID, layDate, setterFROM, trackNO);
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();

                decimal? existeMesmoSetter = hedTA.ExistsHatcheryEggData2(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                decimal? existeAntigoSetter = hedTA.ExistsHatcheryEggData2(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterFROM, trackNO);

                if ((existeMesmoSetter > 0) && (existeAntigoSetter > 0))
                {
                    int qtdOvosAntigo = Convert.ToInt32(hedTA.EggsQty(company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO));

                    hedTA.UpdateEggs(qtdOvosAntigo + qtyEggs, company, region,
                    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                    hedTA.Delete1(company, region, location, setDate, hatchLoc, flockID,
                        layDate, setterFROM, trackNO);
                }
                else
                {
                    hedTA.UpdateMachine(setterTO, company, region, location, setDate, hatchLoc,
                        flockID, layDate, setterFROM, trackNO);
                }

                #endregion
            }
            else if (company == "HYCO") // DISABLE
            {
                #region HYCO

                //ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                //    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();

                //decimal? existeMesmoSetter = hedTA.ExistsHatcheryEggData2(company, region,
                //    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                //decimal? existeAntigoSetter = hedTA.ExistsHatcheryEggData2(company, region,
                //    location, setDate, hatchLoc, flockID, layDate, setterFROM, trackNO);

                //if ((existeMesmoSetter > 0) && (existeAntigoSetter > 0))
                //{
                //    int qtdOvosAntigo = Convert.ToInt32(hedTA.EggsQty(company, region,
                //    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO));

                //    hedTA.UpdateEggs(qtdOvosAntigo + qtyEggs, company, region,
                //    location, setDate, hatchLoc, flockID, layDate, setterTO, trackNO);

                //    hedTA.Delete1(company, region, location, setDate, hatchLoc, flockID,
                //        layDate, setterFROM, trackNO);
                //}
                //else
                //{
                //    hedTA.UpdateMachine(setterTO, company, region, location, setDate, hatchLoc,
                //        flockID, layDate, setterFROM, trackNO);
                //}

                #endregion
            }
        }

        public void SetTransfDataFLIP(string company, string region, string location, string hatchLoc, 
            DateTime setDate, string flockID, DateTime layDate, string setter)
        {
            #region Load Variables

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) 
                + hatchLoc + flockID;
            string trackNo = "EXP" + layDate.ToString("yyMMdd");
            string hatcher = "H-" + setter.Substring(2, 2);
            var listHTD = bdSQLServer.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == hatchLoc && w.Set_date == setDate
                    && w.Flock_id == flockID && w.Lay_date == layDate
                    && w.Machine == setter)
                .ToList();
            int qtyEggs = Convert.ToInt32(listHTD.Sum(s => s.Eggs_rcvd));
            DateTime transfDate = setDate.AddDays(19);
            string dataRetirada = transfDate.ToShortDateString();
            string transfTime = listHTD.Max(m => m.Horario);

            #endregion

            if (company == "HYBR")
            {
                #region HYBR

                #region Load Variables HYBR

                FLIPDataSet.HATCHERY_TRAN_DATADataTable htdDT = new FLIPDataSet.HATCHERY_TRAN_DATADataTable();
                HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();

                htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, layDate, setter, hatcher);

                #endregion

                if (htdDT.Count == 0)
                {
                    #region Insert

                    htdTA.Insert(eggKey, layDate, qtyEggs, setter, hatcher, trackNo, 0, 0,
                        null, null, null, null, null, null, dataRetirada, transfTime, null);

                    #endregion
                }
                else
                {
                    #region Update / Delete

                    FLIPDataSet.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                    if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                    transfFLIP.NUM_2 = 0;
                    if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                    transfFLIP.NUM_2 = 0;
                    transfFLIP.TEXT_1 = dataRetirada;
                    transfFLIP.TEXT_2 = transfTime;
                    transfFLIP.EGGS_TRAN = qtyEggs;
                    if (qtyEggs == 0)
                        htdTA.Delete(eggKey, layDate, setter, hatcher, trackNo);
                    else
                        htdTA.Update(transfFLIP);

                    #endregion
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                #region Load Variables HYBR

                ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_TRAN_DATADataTable htdDT =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_TRAN_DATADataTable();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA
                    = new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();

                htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, layDate, setter, hatcher);

                #endregion

                if (htdDT.Count == 0)
                {
                    #region Insert

                    htdTA.Insert(eggKey, layDate, qtyEggs, setter, hatcher, trackNo, 0, 0,
                        null, null, null, null, null, null, dataRetirada, transfTime);

                    #endregion
                }
                else
                {
                    #region Update / Delete

                    ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                    if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                    transfFLIP.NUM_2 = 0;
                    if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                    transfFLIP.NUM_2 = 0;
                    transfFLIP.TEXT_1 = dataRetirada;
                    transfFLIP.TEXT_2 = transfTime;
                    transfFLIP.EGGS_TRAN = qtyEggs;
                    if (qtyEggs == 0)
                        htdTA.Delete(eggKey, layDate, setter, hatcher, trackNo);
                    else
                        htdTA.Update(transfFLIP);

                    #endregion
                }

                #endregion
            }
            else if (company == "HYCO") // DISABLE
            {
                #region HYCO

                //ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                //    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                //ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                //    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();
                //fdTA.FillByFlockAndTrxDate(fdDT, flockID, layDate);

                //foreach (var item in fdDT)
                //{
                //    #region Load Variables HYBR

                //    ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATADataTable htdDT =
                //        new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATADataTable();
                //    ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA
                //        = new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();

                //    htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, layDate, setter, hatcher);

                //    #endregion

                //    if (htdDT.Count == 0)
                //    {
                //        #region Insert

                //        htdTA.Insert(eggKey, layDate, qtyEggs, setter, hatcher, trackNo, 0, 0,
                //            null, null, null, null, null, null, dataRetirada, transfTime);

                //        #endregion
                //    }
                //    else
                //    {
                //        #region Update / Delete

                //        ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                //        if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                //        transfFLIP.NUM_2 = 0;
                //        if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                //        transfFLIP.NUM_2 = 0;
                //        transfFLIP.TEXT_1 = dataRetirada;
                //        transfFLIP.TEXT_2 = transfTime;
                //        transfFLIP.EGGS_TRAN = qtyEggs;
                //        if (qtyEggs == 0)
                //            htdTA.Delete(eggKey, layDate, setter, hatcher, trackNo);
                //        else
                //            htdTA.Update(transfFLIP);

                //        #endregion
                //    }
                //}

                #endregion
            }
        }

        public void UpdateEstimateFLIP(string company, string region, string hatchLoc, DateTime setDate)
        {
            var lista = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Company == company && h.Region == region
                    && h.Set_date == setDate && h.Status == "Importado"
                    && h.Hatch_loc == hatchLoc)
                .GroupBy(h => new
                {
                    h.Company,
                    h.Region,
                    h.Location,
                    h.Set_date,
                    h.Hatch_loc,
                    h.Flock_id
                })
                .Select(h => new
                {
                    type = h.Key,
                    soma = h.Sum(x => x.Eggs_rcvd),
                    estimate = h.Max(x => x.Estimate),
                    observacao = h.Max(x => x.Observacao)
                })
                .ToList();

            foreach (var item in lista)
            {
                UpdateEstimateFLIPByFlock(item.type.Company, item.type.Region, item.type.Location, item.type.Hatch_loc, item.type.Set_date, item.type.Flock_id);
            }
        }

        public void UpdateEstimateFLIPByFlock(string company, string region, string location, string hatchLoc, DateTime setDate, string flockID)
        {
            decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(hatchLoc, setDate, flockID);

            hatcheryFlockData.UpdateEstimate(mediaIncubacao, company, region, location, setDate, hatchLoc, flockID);
        }

        #endregion

        #region Get Methods

        public int GetQtyHatchingEggsProduced(string company, string region, string location, string farmID,
            string flockID, DateTime layDate)
        {
            int qty = 0;

            if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                fdTA.FillFlockData(cl.FLOCK_DATA, company, region, location, farmID, flockID, layDate);

                if (cl.FLOCK_DATA.Count > 0)
                    qty = Convert.ToInt32(cl.FLOCK_DATA[0].NUM_1);
                else
                    qty = Convert.ToInt32(Session["qtde"]);

                #endregion
            }
            else if (company == "HYBR")
            {
                #region HYBR

                flockData.FillFlockData(flipDataSet.FLOCK_DATA, company, region, location, farmID, flockID, layDate);

                if (flipDataSet.FLOCK_DATA.Count > 0)
                    qty = Convert.ToInt32(flipDataSet.FLOCK_DATA[0].NUM_1);
                else
                    qty = Convert.ToInt32(Session["qtde"]);

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();
                fdTA.FillByFlockAndTrxDate(fdDT, flockID, layDate);

                foreach (var item in fdDT)
                {
                    ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                    //fdTA.FillFlockData(cl.FLOCK_DATA, company, region, location, farmID, item.FLOCK_ID, layDate);
                    fdTA.FillFlockData02(cl.FLOCK_DATA, company, farmID, item.FLOCK_ID, layDate);

                    if (cl.FLOCK_DATA.Count > 0)
                        qty = qty + Convert.ToInt32(cl.FLOCK_DATA[0].NUM_1);
                    //else
                    //    qty = Convert.ToInt32(Session["qtde"]);
                }

                #endregion
            }

            return qty;
        }

        public string GetNumLote(string company, string region, string location, string farmID, string flockID,
            DateTime layDate)
        {
            string numLote = "";
            if (company == "HYBR")
            {
                #region HYBR

                flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", location, farmID, flockID);
                if (flipDataSet.FLOCKS.Count > 0)
                {
                    Session["dataNascimentoLote"] = flipDataSet.FLOCKS[0].HATCH_DATE;
                    Session["age"] = ((layDate - flipDataSet.FLOCKS[0].HATCH_DATE).Days) / 7;
                    Session["linhagem"] = flipDataSet.FLOCKS[0].VARIETY;
                    Session["numGalpao"] = flipDataSet.FLOCKS[0].NUM_2;
                    numLote = flipDataSet.FLOCKS[0].NUM_1.ToString();
                }
                else
                {
                    Session["age"] = 0;
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCKSTableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                fTA.FillByFlockID(cl.FLOCKS, flockID);
                if (cl.FLOCKS.Count > 0)
                {
                    Session["dataNascimentoLote"] = cl.FLOCKS[0].HATCH_DATE;
                    Session["age"] = ((layDate - cl.FLOCKS[0].HATCH_DATE).Days) / 7;
                    Session["linhagem"] = cl.FLOCKS[0].VARIETY;
                    Session["numGalpao"] = cl.FLOCKS[0].NUM_2;
                    numLote = cl.FLOCKS[0].NUM_1.ToString();
                }
                else
                {
                    Session["age"] = 0;
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter fTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCKSTableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                fTA.FillByFlock(cl.FLOCKS, flockID);
                if (cl.FLOCKS.Count > 0)
                {
                    Session["dataNascimentoLote"] = cl.FLOCKS[0].HATCH_DATE;
                    Session["age"] = ((layDate - cl.FLOCKS[0].HATCH_DATE).Days) / 7;
                    Session["linhagem"] = cl.FLOCKS[0].VARIETY;
                    Session["numGalpao"] = cl.FLOCKS[0].NUM_2;
                    numLote = cl.FLOCKS[0].NUM_1.ToString();
                }
                else
                {
                    Session["age"] = 0;
                    Session["numGalpao"] = 0;
                }

                #endregion
            }

            return numLote;
        }

        public string GetLocation(string company, string hatchLoc)
        {
            string location = "";

            if (company == "HYBR")
            {
                MvcAppHyLinedoBrasil.Data.FLIPDataSet.HATCHERY_CODESDataTable hcDT =
                    new FLIPDataSet.HATCHERY_CODESDataTable();
                hatchCodes.FillByHatchLoc(hcDT, hatchLoc);
                location = hcDT[0].LOCATION;
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_CODESTableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                hcTA.FillByHatchLoc(cl.HATCHERY_CODES, hatchLoc);
                location = cl.HATCHERY_CODES[0].LOCATION;
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_CODESTableAdapter hcTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_CODESTableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                hcTA.FillByHatchLoc(cl.HATCHERY_CODES, hatchLoc);
                location = cl.HATCHERY_CODES[0].LOCATION;
            }

            return location;
        }

        public int ExistsHatcheryEggDataForSetDate(string company, string region, string location,
            DateTime setDate, string hatchLoc)
        {
            int exists = 0;

            if (company == "HYBR")
            {
                exists = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForSetDate(company, region, location, 
                    setDate, hatchLoc));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                exists = Convert.ToInt32(hedTA.ExistsHatcheryEggDataForSetDate(company, region, location, setDate, 
                    hatchLoc));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable hedDT = 
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable();
                hedTA.ExistsHatcheryEggDataForSetDate(hedDT, company, region, location, setDate, hatchLoc);
                exists = hedDT.Count;
            }

            return exists;
        }

        public decimal GetQtySettedEggs(string company, string region, string location, DateTime setDate, 
            string hatchLoc, string flockID, DateTime layDate, string setter, string trackNO)
        {
            decimal GetQtySettedEggs = 0;
            if (company == "HYBR")
            {
                GetQtySettedEggs = Convert.ToDecimal(hatcheryEggData.QtdOvos(company, region, location, setDate, 
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();

                GetQtySettedEggs = Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();

                int posicaoHifen = flockID.IndexOf("-") + 1;
                int tamanho = flockID.Length - posicaoHifen;
                string flock = flockID.Substring(posicaoHifen, tamanho);

                fdTA.FillByFlockAndTrxDate(fdDT, flock, layDate);
                foreach (var item in fdDT)
                {
                    GetQtySettedEggs = GetQtySettedEggs + Convert.ToDecimal(hedTA.EggsQty(company, region, location, setDate,
                        hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, setter, trackNO));
                }
            }

            return GetQtySettedEggs;
        }

        public int ExistsHatcheryEggDataAll(string company, string region, string location, DateTime setDate,
            string hatchLoc, string flockID, DateTime layDate, string setter, string trackNO)
        {
            int exists = 0;

            if (company == "HYBR")
            {
                exists = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                exists = Convert.ToInt32(hedTA.ExistsHEDAll(company, region, location, setDate,
                    hatchLoc, flockID, layDate, setter, trackNO));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable fdDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.FLOCK_DATADataTable();

                int posicaoHifen = flockID.IndexOf("-") + 1;
                int tamanho = flockID.Length - posicaoHifen;
                string flock = flockID.Substring(posicaoHifen, tamanho);

                fdTA.FillByFlockAndTrxDate(fdDT, flock, layDate);

                foreach (var item in fdDT)
                {
                    exists = exists + Convert.ToInt32(hedTA.ExistsHEDAll(company, region, location, setDate,
                        hatchLoc, item.FARM_ID + "-" + item.FLOCK_ID, layDate, setter, trackNO));
                }
            }

            return exists;
        }

        public string GetResponsableByHatchery(string hatchLoc)
        {
            string responsable = "";

            FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();

            hTA.Fill(hDT);

            foreach (var item in hDT)
            {
                if (item.HATCH_LOC == hatchLoc)
                    responsable = item.ORDENT_LOC;
            }

            return responsable;
        }

        public string GetFlockDataValueByField(string company, string region, string location, string farmID,
            string flockID, DateTime layDate, string field)
        {
            string fieldValue = "";

            if (company == "HYBR")
            {
                flockData.FillFlockData(flipDataSet.FLOCK_DATA, company, region, location, farmID, flockID, layDate);
                if (flipDataSet.FLOCK_DATA.Count > 0)
                    fieldValue = flipDataSet.FLOCK_DATA[0][field].ToString();
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                fdTA.FillFlockData(cl.FLOCK_DATA, company, region, location, farmID, flockID, layDate);

                if (cl.FLOCK_DATA.Count > 0)
                    fieldValue = cl.FLOCK_DATA[0][field].ToString();
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                fdTA.FillFlock(cl.FLOCK_DATA, company, region, location, farmID, flockID, layDate);

                if (cl.FLOCK_DATA.Count > 0)
                    fieldValue = cl.FLOCK_DATA[0][field].ToString();
            }

            return fieldValue;
        }

        public DateTime GetLastProductionDate(string company, string region, string location, string farmID,
            string flockID)
        {
            DateTime lastDate = new DateTime();

            if (company == "HYBR")
            {
                lastDate = Convert.ToDateTime(flockData.UltimaProducaoPorLote(company, region, location,
                    farmID, flockID));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                lastDate = Convert.ToDateTime(fdTA.LastProductionDate(company, region, location,
                    farmID, flockID));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter fdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                lastDate = Convert.ToDateTime(fdTA.LastProductionDateFlock(company, region, location,
                    farmID, flockID));
            }

            return lastDate;
        }

        public DateTime GetLastSetDate(string company, string region, string location, string hatchLoc, string flockID)
        {
            DateTime lastDate = new DateTime();

            if (company == "HYBR")
            {
                lastDate = Convert.ToDateTime(hatcheryFlockData.UltimaIncubacao(company, region, location,
                    hatchLoc, flockID));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                lastDate = Convert.ToDateTime(hfdTA.LastSetDate(company, region, location, hatchLoc, flockID));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                lastDate = Convert.ToDateTime(hfdTA.LastSetDateFlock(company, region, location, hatchLoc, flockID));
            }

            return lastDate;
        }

        public decimal GetLastEstimate(string company, string region, string location, DateTime setDate, string hatchLoc, string flockID)
        {
            decimal lastEstimate = 0;

            if (company == "HYBR")
            {
                lastEstimate = Convert.ToDecimal(hatcheryFlockData.UltimaPercEclosao(company, region, location,
                    setDate, hatchLoc, flockID));
            }
            else if (company == "HYCL")
            {
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.CLFLOCKS cl = new ImportaIncubacao.Data.FLIP.CLFLOCKS();
                lastEstimate = Convert.ToDecimal(hfdTA.LastEstimate(company, region, location,
                    setDate, hatchLoc, flockID));
            }
            else if (company == "HYCO")
            {
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS cl = new ImportaIncubacao.Data.FLIP.HCFLOCKS();
                lastEstimate = Convert.ToDecimal(hfdTA.LastEstimateFlock(company, region, location,
                    setDate, hatchLoc, flockID));
            }

            return lastEstimate;
        }

        public static bool ExisteFechamentoEstoque(string company, string hatchLoc, DateTime dataMov)
        {
            bool closed = false;

            if (company == "HYCL")
            {
                #region Fechamento Estoque - FLIP Chile

                ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable DfDT =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCDataTable();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter DfTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                DfTA.Fill(DfDT);

                if (DfDT.Count > 0)
                {
                    ImportaIncubacao.Data.FLIP.CLFLOCKS.DATA_FECH_LANCRow DfRow =
                        DfDT.Where(w => w.DATA_FECH_LANC >= dataMov && w.LOCATION == "Planta de Incubación")
                        .FirstOrDefault();

                    if (DfRow != null)
                        closed = true;
                    else
                        closed = false;
                }
                else
                    closed = false;

                #endregion
            }
            else if (company == "HYBR")
            {
                #region Fechamento Estoque - FLIP Brasil

                FLIPDataSet.DATA_FECH_LANCDataTable DfDT = new FLIPDataSet.DATA_FECH_LANCDataTable();
                DATA_FECH_LANCTableAdapter DfTA = new DATA_FECH_LANCTableAdapter();
                DfTA.Fill(DfDT);

                if (DfDT.Count > 0)
                {
                    FLIPDataSet.DATA_FECH_LANCRow DfRow = DfDT.Where(w => w.DATA_FECH_LANC >= dataMov
                        && w.LOCATION == hatchLoc)
                        .FirstOrDefault();

                    if (DfRow != null)
                        closed = true;
                    else
                        closed = false;
                }
                else
                    closed = false;

                #endregion
            }
            else if (company == "HYCO")
            {
                #region Fechamento Estoque - FLIP Colombia

                ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable DfDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCDataTable();
                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter DfTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.DATA_FECH_LANCTableAdapter();
                DfTA.Fill(DfDT);

                if (DfDT.Count > 0)
                {
                    ImportaIncubacao.Data.FLIP.HCFLOCKS.DATA_FECH_LANCRow DfRow =
                        DfDT.Where(w => w.DATA_FECH_LANC >= dataMov && w.LOCATION == hatchLoc)
                        .FirstOrDefault();

                    if (DfRow != null)
                        closed = true;
                    else
                        closed = false;
                }
                else
                    closed = false;

                #endregion
            }

            return closed;
        }

        public string GetCompanyAndRegionByHatchLoc(string hatchLoc, string field)
        {
            string fieldValue = "";

            HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();
            FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
            hTA.FillByHatchLoc(hDT, hatchLoc);
            if (hDT.Count > 0)
            {
                var hc = hDT.FirstOrDefault();
                fieldValue = hc[field].ToString();
            }

            return fieldValue;
        }

        #endregion

        #endregion

        #region Old Methods

        public void AtualizaIdadesLinhagens()
        {
            DateTime data = Convert.ToDateTime("01/10/2013");

            var lista = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Set_date >= data)
                .ToList();

            int tamanho = 0;

            foreach (var item in lista)
            {
                tamanho = item.Flock_id.Length - 6;

                flocks.FillBy(flipDataSet.FLOCKS, item.Company, item.Region, item.Location, item.Flock_id.Substring(0, 5),
                    item.Flock_id.Substring(6, tamanho));

                flockData.FillFlockData(flipDataSet.FLOCK_DATA, item.Company, item.Region, item.Location, item.Flock_id.Substring(0, 5),
                    item.Flock_id.Substring(6, tamanho), item.Lay_date);

                if (flipDataSet.FLOCKS.Count > 0)
                {
                    item.Variety = flipDataSet.FLOCKS[0].VARIETY;
                }

                if (flipDataSet.FLOCK_DATA.Count > 0)
                {
                    item.Age = Convert.ToInt32(flipDataSet.FLOCK_DATA[0].AGE);
                }
            }

            bdSQLServer.SaveChanges();
        }

        public void AtualizaFLIP(DateTime setDate)
        {
            try
            {
                #region Load data components

                DateTime data = Convert.ToDateTime("01/07/2013");
                string incubatorio = ddlIncubatorios.SelectedValue;
                string company = GetCompanyAndRegionByHatchLoc(incubatorio, "company");
                string location = GetLocation(company, incubatorio);

                #endregion

                if ((setDate >= data) ||
                    ((setDate == Convert.ToDateTime("19/06/2013")) && (incubatorio == "CH")) ||
                    ((setDate == Convert.ToDateTime("20/11/2013")) && (incubatorio == "TB"))) // erro de fechamento, por isso o dia 19/06.
                {
                    var lista = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Set_date == setDate && h.Status == "Importado" && h.Hatch_loc == incubatorio)// && h.Flock_id == "HLP04-P044292W")
                        .GroupBy(h => new
                        {
                            h.Company,
                            h.Region,
                            h.Location,
                            h.Set_date,
                            h.Hatch_loc,
                            h.Flock_id,
                            h.Lay_date,
                            h.Machine,
                            h.Track_no
                        })
                        .Select(h => new //HATCHERY_EGG_DATA
                        {
                            type = h.Key,
                            soma = h.Sum(x => x.Eggs_rcvd),
                            estimate = h.Max(x => x.Estimate),
                            observacao = h.Max(x => x.Observacao)
                        })
                        .ToList();

                    int existeIncubacao = Convert.ToInt32(hatcheryEggData
                        .ExisteHatcheryEggDataForSetDate("HYBR", "BR", location, setDate,
                        ddlIncubatorios.SelectedValue));

                    // Verifica se existe mais no FLIP do que no HLBAPP. 
                    // Caso exista, serão deletados, pois no HLBAPP que é o correto.

                    if (lista.Count != existeIncubacao)
                    {
                        hatcheryEggData.DeleteByHatchLocAndSetDate(setDate, ddlIncubatorios.SelectedValue);

                        FLIPDataSet.HATCHERY_EGG_DATADataTable listaFLIP =
                            hatcheryEggData.GetDataBySetDate("HYBR", "BR", location, setDate,
                            ddlIncubatorios.SelectedValue);

                        foreach (var item in listaFLIP)
                        {
                            int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                                .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                                    h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                                    h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                                    h.Lay_date == item.LAY_DATE && h.Machine.ToUpper() == item.MACHINE &&
                                    h.Track_no == item.TRACK_NO)
                                .Count();

                            if (existeHLBAPP == 0)
                            {
                                hatcheryEggData.Delete(item.COMPANY, item.REGION, item.LOCATION, item.SET_DATE,
                                    item.HATCH_LOC, item.FLOCK_ID, item.LAY_DATE, item.MACHINE, item.TRACK_NO);

                                existeHLBAPP = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(item.COMPANY,
                                    item.REGION, item.LOCATION, item.SET_DATE,
                                    item.HATCH_LOC, item.FLOCK_ID));

                                if (existeHLBAPP == 0)
                                {
                                    hatcheryFlockData.Delete(item.COMPANY,
                                    item.REGION, item.LOCATION, item.SET_DATE,
                                    item.HATCH_LOC, item.FLOCK_ID);
                                }
                            }
                        }
                    }

                    foreach (var item in lista)
                    {
                        //HATCHERY_EGG_DATA item = new HATCHERY_EGG_DATA();

                        //item = (HATCHERY_EGG_DATA)item2;

                        //int existe = bdSQLServer.HATCHERY_EGG_DATA
                        //    .Where(h => h.Company == item.type.Company && h.Region == item.type.Region &&
                        //        h.Location == item.type.Location && h.Set_date == item.type.Set_date &&
                        //        h.Hatch_loc == item.type.Hatch_loc && h.Flock_id == item.type.Flock_id &&
                        //        h.Lay_date == item.type.Lay_date && h.Machine == item.type.Machine &&
                        //        h.Track_no == item.type.Track_no)
                        //    .Count();


                        //Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, 
                        //item.type.Location,
                        //item.type.Set_date.ToString("dd/MM/yyyy"), item.type.Hatch_loc, item.type.Flock_id, 
                        //item.type.Lay_date.ToString("dd/MM/yyyy"), item.type.Machine,
                        //item.type.Track_no));

                        decimal qtdOvos = Convert.ToDecimal(hatcheryEggData.QtdOvos(item.type.Company, item.type.Region,
                            item.type.Location, item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id,
                            item.type.Lay_date, item.type.Machine, item.type.Track_no));


                        //if ((item.type.Flock_id == "HLP13-P134581LB") && (item.type.Lay_date == Convert.ToDateTime("31/10/2013")))
                        //{
                        //string teste = "entrou";

                        //decimal soma = bdSQLServer.HATCHERY_EGG_DATA
                        //    .Where(h => h.Company == item.type.Company && h.Region == item.type.Region &&
                        //        h.Location == item.type.Location && h.Set_date == item.type.Set_date &&
                        //        h.Hatch_loc == item.type.Hatch_loc && h.Flock_id == item.type.Flock_id &&
                        //        h.Lay_date == item.type.Lay_date && h.Machine == item.type.Machine &&
                        //        h.Track_no == item.type.Track_no && h.Machine
                        //}

                        int existeInc = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                                        item.type.Track_no));

                        if ((qtdOvos != item.soma) || (existeInc == 0))
                        {
                            int start = item.type.Flock_id.IndexOf("-") + 1;
                            int tamanho = item.type.Flock_id.Length - start;

                            string lote = item.type.Flock_id.Substring(start, tamanho);
                            string farm = item.type.Flock_id.Substring(0, start - 1);

                            decimal qtdOvosSet = Convert.ToDecimal(eggInvData.QtdOvosByStatus(
                                lote, "S", item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            if (qtdOvosSet < item.soma)
                            {
                                if (qtdOvosSet == 0)
                                {
                                    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                        farm, lote,
                                        item.type.Track_no, item.type.Lay_date, item.soma, "S", null, null, null, null, null, null, null, null,
                                        item.type.Hatch_loc, null);
                                }
                                else
                                {
                                    decimal? qtdUpdate = qtdOvosSet + item.soma;

                                    eggInvData.UpdateQueryEggs(qtdUpdate, item.type.Company, item.type.Region, item.type.Location,
                                            farm, lote,
                                            item.type.Track_no, item.type.Lay_date, "S", item.type.Hatch_loc);
                                }
                            }

                            hatcheryEggData.Delete(item.type.Company, item.type.Region, item.type.Location,
                                item.type.Set_date, item.type.Hatch_loc,
                                item.type.Flock_id, item.type.Lay_date,
                                item.type.Machine, item.type.Track_no);

                            //int existeHLBAPP = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData(item.type.Company,
                            //    item.type.Region, item.type.Location, item.type.Set_date.ToString("dd/MM/yyyy"),
                            //    item.type.Hatch_loc, item.type.Flock_id));

                            //if (existeHLBAPP == 0)
                            //{
                            //    hatcheryFlockData.Delete(item.type.Company,
                            //    item.type.Region, item.type.Location, item.type.Set_date,
                            //    item.type.Hatch_loc, item.type.Flock_id);
                            //}

                            /**** AJUSTE EGG INVENTORY PARA INCLUIR INCUBAÇÃO ****/

                            //if (setDate < Convert.ToDateTime("06/02/2014"))
                            //{
                            int existeAjuste = Convert.ToInt32(eggInvData.ScalarQueryOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            if (existeAjuste == 0)
                            {
                                eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                    farm, lote,
                                    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                    item.type.Hatch_loc, null);
                            }
                            else
                            {
                                int qtdeOvosAjuste = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                if (qtdeOvosAjuste < item.soma)
                                {
                                    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                        farm, lote,
                                        item.type.Track_no, item.type.Lay_date, "O", item.type.Hatch_loc);
                                }
                            }
                            //}
                            /****/

                            int existe = Convert.ToInt32(eggInvData.ScalarQueryOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                            //if (existe == 0)
                            if (existe > 0)
                            {
                                //    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                //    item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                //    item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                //    item.type.Hatch_loc, null);
                                //}

                                int qtdeOvos = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                if ((qtdeOvos - item.soma) >= 0)
                                {
                                    //    eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                    //    item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                    //    item.type.Track_no, item.type.Lay_date.ToString("dd/MM/yyyy"), "O", item.type.Hatch_loc);
                                    //}

                                    decimal existeSetDay = Convert.ToDecimal(setDayData.ExisteSetDayData(item.type.Set_date, item.type.Hatch_loc));

                                    if (existeSetDay == 0)
                                    {
                                        decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(ddlIncubatorios.SelectedValue)) + 1;

                                        setDayData.InsertQuery("HYBR", "BR", location, item.type.Set_date, item.type.Hatch_loc, sequencia);
                                    }

                                    existe = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataAll(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine,
                                        item.type.Track_no));

                                    if (existe == 1)
                                    {
                                        hatcheryEggData.Delete(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                            item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.type.Machine, item.type.Track_no);
                                    }

                                    existe = Convert.ToInt32(hatcheryFlockData.ExisteHatcheryFlockData(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id));

                                    if (existe == 0)
                                    {
                                        //hatcheryFlockData.Delete(item.type.Company, item.type.Region, item.type.Location,
                                        //    item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id);
                                        hatcheryFlockData.InsertQuery(item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id, item.estimate);
                                    }
                                    // 14/08/2014 - Ocorrência 99 - APONTES
                                    // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
                                    // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
                                    // o trigger de atualização da idade executar.
                                    else
                                    {
                                        decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(
                                            ddlIncubatorios.SelectedValue,
                                            item.type.Set_date, item.type.Flock_id);

                                        hatcheryFlockData.UpdateEstimate(mediaIncubacao, item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id);
                                    }

                                    hatcheryEggData.Insert(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                        item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.soma, null,
                                        item.type.Machine, item.type.Track_no, null, null, null, null, null, null,
                                        null, null, item.observacao, Session["login"].ToString());

                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                            && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                            && h.Location == item.type.Location && h.Region == item.type.Region
                                            && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Sim";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                                else
                                {
                                    var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                            && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                            && h.Location == item.type.Location && h.Region == item.type.Region
                                            && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                                    foreach (var naoImportado in listaNaoImportadoFLIP)
                                    {
                                        naoImportado.ImportadoFLIP = "Não";
                                    }

                                    bdSQLServer.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            var listaNaoImportadoFLIP = bdSQLServer.HATCHERY_EGG_DATA
                                        .Where(h => h.Flock_id == item.type.Flock_id && h.Lay_date == item.type.Lay_date
                                            && h.Set_date == item.type.Set_date && h.Machine == item.type.Machine
                                            && h.Location == item.type.Location && h.Region == item.type.Region
                                            && h.Company == item.type.Company && h.Hatch_loc == item.type.Hatch_loc)
                                        .ToList();

                            foreach (var naoImportado in listaNaoImportadoFLIP)
                            {
                                naoImportado.ImportadoFLIP = "Sim";
                            }

                            bdSQLServer.SaveChanges();
                        }
                    }
                    //}
                }
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #endregion

        #region WEB - HLBAPP

        public void GenerateTransfAutomaticallyWEB(string hatchLoc, DateTime setDate, string flockID,
            string setter, string classOvo)
        {
            #region Insert / Update Transf. in WEB

            var listHED = bdSQLServer.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == hatchLoc && w.Set_date == setDate
                    && w.Flock_id == flockID && w.Machine == setter
                    && w.ClassOvo == classOvo)
                .ToList();

            var transf = bdSQLServer.HATCHERY_TRAN_DATA
                .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                    && w.Flock_id == flockID && w.Setter == setter
                    && w.ClassOvo == classOvo)
                .FirstOrDefault();

            if (listHED.Count > 0)
            {
                DateTime lastLayDate = listHED.Max(m => m.Lay_date);
                string numLote = listHED.FirstOrDefault().Egg_key;
                string linhagem = listHED.FirstOrDefault().Variety;

                if (transf == null)
                {
                    transf = new HATCHERY_TRAN_DATA();
                    transf.Hatch_Loc = hatchLoc;
                    transf.Set_date = setDate;
                    transf.Flock_id = flockID;
                    transf.NumLote = numLote;
                    transf.Setter = setter;
                    transf.ClassOvo = classOvo;
                    transf.Variety = linhagem;
                }

                transf.Lay_date = listHED.Max(m => m.Lay_date);
                transf.Hatcher = "H-" + transf.Setter.Substring(2, 2);
                transf.Transf_date = setDate.AddDays(19);
                transf.Qtde_Ovos_Transferidos = listHED.Sum(s => s.Eggs_rcvd);
                //transf.Hora_Inicio = DateTime.Now.ToString("HH:mm", CultureInfo.GetCultureInfo("pt-BR"));
                transf.Hora_Inicio = listHED.Max(m => m.Horario);

                if (transf.ID == 0) bdSQLServer.HATCHERY_TRAN_DATA.AddObject(transf);
            }
            else
            {
                bdSQLServer.HATCHERY_TRAN_DATA.DeleteObject(transf);
            }

            bdSQLServer.SaveChanges();

            #endregion
        }

        public LayoutDiarioExpedicaos InsertDEOSortingEggs(string farm, string farmID, string flockID,
            decimal flockNumber, decimal shed, string variety, int age, DateTime trxDate, decimal qtyEggs,
            string hatchLoc, string eggSorting, string eggSortingDescription, DateTime setDate)
        {
            HLBAPPEntities bdSQL = new HLBAPPEntities();
            bdSQL.CommandTimeout = 10000;

            LayoutDiarioExpedicaos deo = bdSQL.LayoutDiarioExpedicaos
                .Where(w => w.Nucleo == farmID && w.LoteCompleto == flockID
                    && w.DataProducao == trxDate
                    && w.DataHoraCarreg == setDate
                    && w.Granja == farm
                    && w.Incubatorio == hatchLoc
                    && w.TipoDEO == "Classificação de Ovos"
                    && w.TipoOvo == eggSorting).FirstOrDefault();

            if (deo == null)
                deo = new LayoutDiarioExpedicaos();
            else
                qtyEggs = qtyEggs + deo.QtdeOvos;

            deo.Granja = farm;
            deo.Nucleo = farmID;
            deo.Galpao = shed.ToString();
            deo.Lote = flockNumber.ToString();
            deo.Idade = age;
            deo.Linhagem = variety;
            deo.LoteCompleto = flockID;
            deo.DataProducao = trxDate;
            deo.NumeroReferencia = DateTime.Now.DayOfYear.ToString();
            deo.QtdeOvos = qtyEggs;
            deo.QtdeBandejas = (qtyEggs / 360);
            deo.Usuario = "SISTEMA WEB";
            deo.DataHora = DateTime.Now;
            deo.DataHoraCarreg = setDate;
            deo.NFNum = "";
            deo.GTANum = "";
            deo.Importado = "Conferido";
            deo.Incubatorio = hatchLoc;
            deo.TipoDEO = "Classificação de Ovos";
            deo.DataHoraRecebInc = Convert.ToDateTime("01/01/1899");
            deo.ResponsavelCarreg = "";
            deo.ResponsavelReceb = "";
            deo.Observacao = "Diario de envío generado automáticamente al clasificar huevos tipo "
                + eggSortingDescription
                + " en la incubación WEB.";
            deo.TipoOvo = eggSorting;
            deo.QtdDiferenca = 0;
            deo.QtdeConferencia = 0;

            return deo;
        }

        #endregion

        #region APOLO - DESATIVADO

        protected void btnImportarEstqApolo_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    #region Importa p/ Apolo

            //    #region Carrega variáveis e objetos

            //    //bdApolo.CommandTimeout = 10000;

            //    DateTime dataIncubacao = Calendar1.SelectedDate;
            //    string incubatorio = ddlIncubatorios.SelectedValue;

            //    string naturezaOperacao = "5.101";
            //    decimal? valorUnitario = 0.25m;
            //    string unidadeMedida = "UN";
            //    short? posicaoUnidadeMedida = 1;
            //    string tribCod = "040";
            //    string itMovEstqClasFiscCodNbm = "04079000";
            //    string clasFiscCod = "0000129";
            //    string operacao = "Saída";

            //    ITEM_MOV_ESTQ itemMovEstq = null;

            //    string usuario;
            //    if (Session["login"].ToString().Equals("palves"))
            //        usuario = "RIOSOFT";
            //    else
            //        usuario = Session["login"].ToString().ToUpper();

            //    ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
            //        .Where(ef => ef.USERFLIPCod == "CH")
            //        .FirstOrDefault();

            //    LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ
            //        .Where(l => l.USERCodigoFLIP == incubatorio && l.USERTipoProduto == "Ovos Incubáveis")
            //        .FirstOrDefault();

            //    var listaIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //        .Where(h => h.Hatch_loc == incubatorio && h.Set_date == dataIncubacao && h.ImportadoApolo != "Sim")
            //        .ToList();

            //    HATCHERY_EGG_DATA egg = listaIncubacao.FirstOrDefault();

            //    PRODUTO produto = bdApolo.PRODUTO
            //            .Where(p => p.ProdNomeAlt1 == egg.Variety)
            //            .FirstOrDefault();

            //    LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //            .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                && i.ProdCodEstr == produto.ProdCodEstr
            //                && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                    && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                        .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //            .FirstOrDefault();

            //    if (locItemMovEstq != null)
            //    {
            //        ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

            //        bdApolo.delete_movestq(locItemMovEstq.EmpCod, locItemMovEstq.MovEstqChv, usuario, rmensagem);

            //        ITEM_MOV_ESTQ item = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == "1" && i.ProdCodEstr == produto.ProdCodEstr
            //            && i.ItMovEstqDataMovimento <= dataIncubacao).OrderByDescending(o => o.ItMovEstqDataMovimento).FirstOrDefault();

            //        //bdApolo.atualiza_saldoestqdata(item.EmpCod, item.MovEstqChv, item.ProdCodEstr, item.ItMovEstqSeq,
            //        //    item.ItMovEstqDataMovimento, "UPD");
            //    }

            //    #endregion

            //    foreach (var item in listaIncubacao)
            //    {
            //        produto = bdApolo.PRODUTO
            //            .Where(p => p.ProdNomeAlt1 == item.Variety)
            //            .FirstOrDefault();

            //        int tamanho = item.Flock_id.Length;
            //        tamanho = tamanho - 6;
            //        string flockID = item.Flock_id.Substring(6, tamanho);

            //        #region Insere Saida p/ Incubação

            //        // Verifica se Existe a movimentação neste Incubatório e Produto
            //        locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //            .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                && i.ProdCodEstr == produto.ProdCodEstr
            //                && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                    && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                        .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //            .FirstOrDefault();

            //        if (locItemMovEstq != null)
            //        {
            //            itemMovEstq = bdApolo.ITEM_MOV_ESTQ
            //                .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
            //                    && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
            //                .FirstOrDefault();

            //            itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + item.Eggs_rcvd;
            //            itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

            //            locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + item.Eggs_rcvd;
            //            locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

            //            CTRL_LOTE_ITEM_MOV_ESTQ lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //                .Where(c => c.EmpCod == locItemMovEstq.EmpCod && c.MovEstqChv == locItemMovEstq.MovEstqChv
            //                    && c.ProdCodEstr == locItemMovEstq.ProdCodEstr && c.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq
            //                    && c.LocArmazCodEstr == locItemMovEstq.LocArmazCodEstr && c.CtrlLoteNum == flockID
            //                    && c.CtrlLoteDataValid == item.Lay_date)
            //                .FirstOrDefault();

            //            // Verifica se Existe o lote
            //            if (lote != null)
            //            {
            //                lote.CtrlLoteItMovEstqQtd = lote.CtrlLoteItMovEstqQtd + item.Eggs_rcvd;
            //                lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;
            //            }
            //            else
            //            {
            //                lote = service.InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
            //                    locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flockID, item.Lay_date, item.Eggs_rcvd, operacao,
            //                    itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

            //                bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //            }
            //        }
            //        else
            //        {
            //            // Verifica se Existe a movimentação neste Incubatório e não no Produto
            //            locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                        && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                        .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //                .FirstOrDefault();

            //            if (locItemMovEstq != null)
            //            {
            //                MOV_ESTQ movEstq = bdApolo.MOV_ESTQ
            //                    .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
            //                    .FirstOrDefault();

            //                itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                    movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                    item.Variety, naturezaOperacao, item.Eggs_rcvd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                    tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
            //                    service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                    itemMovEstq.ProdCodEstr, item.Eggs_rcvd, locItemMovEstq.LocArmazCodEstr);

            //                bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                    itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
            //                    item.Lay_date, item.Eggs_rcvd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                    locArmazItemMovEstq.LocArmazCodEstr);

            //                bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //            }
            //            else
            //            {
            //                MOV_ESTQ movEstq = service.InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
            //                    dataIncubacao, usuario);

            //                bdApolo.MOV_ESTQ.AddObject(movEstq);

            //                itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                    movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                    item.Variety, naturezaOperacao, item.Eggs_rcvd, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                    tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq =
            //                    service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                    itemMovEstq.ProdCodEstr, item.Eggs_rcvd, locArmaz.LocArmazCodEstr);

            //                bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                CTRL_LOTE_ITEM_MOV_ESTQ lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                    itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flockID,
            //                    item.Lay_date, item.Eggs_rcvd, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                    locArmazItemMovEstq.LocArmazCodEstr);

            //                bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //            }
            //        }

            //        #endregion

            //        item.ImportadoApolo = "Sim";

            //        CTRL_LOTE_LOC_ARMAZ tabLoteApolo = bdApolo.CTRL_LOTE_LOC_ARMAZ
            //            .Where(c => c.CtrlLoteNum == flockID && c.CtrlLoteDataValid == item.Lay_date
            //                && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr)
            //            .FirstOrDefault();

            //        if (tabLoteApolo != null)
            //        {
            //            if (tabLoteApolo.USERQtdeIncNaoImportApolo == null) tabLoteApolo.USERQtdeIncNaoImportApolo = 0;
            //            tabLoteApolo.USERQtdeIncNaoImportApolo = tabLoteApolo.USERQtdeIncNaoImportApolo - Convert.ToInt32(item.Eggs_rcvd);
            //        }
            //        else
            //        {
            //            lblMensagem3.Visible = true;
            //            lblMensagem3.Text = "Lote " + flockID + " da Data de Produção " + item.Lay_date.ToShortDateString() + " sem saldo! Verifique os DEOs se foram deletados!";
            //            return;
            //        }

            //        bdSQLServer.SaveChanges();
            //        bdApolo.SaveChanges();
            //    }

            //    bdApolo.SaveChanges();

            //    if (itemMovEstq != null)
            //    {
            //        //var listaItensMovEstq = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == itemMovEstq.EmpCod
            //        //&& i.MovEstqChv == itemMovEstq.MovEstqChv).ToList();

            //        //foreach (var item in listaItensMovEstq)
            //        //{
            //        //    bdApolo.atualiza_saldoestqdata(item.EmpCod, item.MovEstqChv, item.ProdCodEstr,
            //        //        item.ItMovEstqSeq, item.ItMovEstqDataMovimento, "INS");
            //        //}

            //        bdApolo.calcula_mov_estq(itemMovEstq.EmpCod, itemMovEstq.MovEstqChv);
            //    }

            //    bdSQLServer.SaveChanges();

            //    VerificaNaoImportados();

            //    GridView1.DataBind();

            //    GridView3.DataBind();

            //    gvMaquinas.DataBind();
            //    gvLotes.DataBind();
            //    gvLinhagens.DataBind();

            //    AtualizaTotais();

            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    lblMensagem3.Visible = true;
            //    if (ex.InnerException != null)
            //        lblMensagem3.Text = "Erro ao Importar P/ Apolo: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
            //    else
            //        lblMensagem3.Text = "Erro ao Importar P/ Apolo: " + ex.Message;
            //}
        }

        protected void btnDeletaImportacaoApolo_Click(object sender, EventArgs e)
        {
            lblPerguntaConfirmaExclusaoImportacao.Visible = true;
            lbtnSim.Visible = true;
            lbtnNao.Visible = true;
        }

        protected void lbtnSim_Click(object sender, EventArgs e)
        {
            try
            {
                //bdApolo.CommandTimeout = 10000;

                DateTime dataIncubacao = Calendar1.SelectedDate;
                string incubatorio = ddlIncubatorios.SelectedValue;

                HATCHERY_EGG_DATA incubacao = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Hatch_loc == incubatorio && h.Set_date == dataIncubacao)
                    .FirstOrDefault();

                PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == incubacao.Variety).FirstOrDefault();
                LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ.Where(l => l.USERCodigoFLIP == incubacao.Hatch_loc).FirstOrDefault();

                string usuario;
                if (Session["login"].ToString().Equals("palves"))
                    usuario = "RIOSOFT";
                else
                    usuario = Session["login"].ToString().ToUpper();

                LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
                        .Where(i => i.EmpCod == "1" && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
                            && i.ProdCodEstr == produto.ProdCodEstr
                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
                                && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
                                    .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
                        .FirstOrDefault();

                if (locItemMovEstq != null)
                {
                    //ObjectParameter rmensagem = new ObjectParameter("rmensagem", typeof(global::System.String));

                    //bdApolo.delete_movestq(locItemMovEstq.EmpCod, locItemMovEstq.MovEstqChv, usuario, rmensagem);

                    //ITEM_MOV_ESTQ item = bdApolo.ITEM_MOV_ESTQ.Where(i => i.EmpCod == "1" && i.ProdCodEstr == produto.ProdCodEstr
                    //    && i.ItMovEstqDataMovimento <= dataIncubacao).OrderByDescending(o => o.ItMovEstqDataMovimento).FirstOrDefault();

                    //bdApolo.atualiza_saldoestqdata(item.EmpCod, item.MovEstqChv, item.ProdCodEstr, item.ItMovEstqSeq,
                    //    item.ItMovEstqDataMovimento, "UPD");

                    MOV_ESTQ movEstq = bdApolo.MOV_ESTQ.Where(m => m.EmpCod == locItemMovEstq.EmpCod
                        && m.MovEstqChv == locItemMovEstq.MovEstqChv).FirstOrDefault();

                    DeletaMovEstq(movEstq);
                }

                var listaIncubacao = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Hatch_loc == incubatorio && h.Set_date == dataIncubacao)
                    .ToList();

                foreach (var inc in listaIncubacao)
                {
                    int tamanho = inc.Flock_id.Length - 6;
                    string flock = inc.Flock_id.Substring(6, tamanho);

                    CTRL_LOTE_LOC_ARMAZ tabLoteApolo = bdApolo.CTRL_LOTE_LOC_ARMAZ
                                        .Where(c => c.CtrlLoteNum == flock && c.CtrlLoteDataValid == inc.Lay_date
                                            && c.LocArmazCodEstr == locArmaz.LocArmazCodEstr)
                                        .FirstOrDefault();

                    if (tabLoteApolo != null)
                    {
                        if (tabLoteApolo.USERQtdeIncNaoImportApolo == null) tabLoteApolo.USERQtdeIncNaoImportApolo = 0;
                        tabLoteApolo.USERQtdeIncNaoImportApolo = tabLoteApolo.USERQtdeIncNaoImportApolo + Convert.ToInt32(inc.Eggs_rcvd);
                    }

                    inc.ImportadoApolo = "Não";
                }

                bdApolo.SaveChanges();
                bdSQLServer.SaveChanges();

                VerificaNaoImportados();

                GridView1.DataBind();

                GridView3.DataBind();

                gvMaquinas.DataBind();
                gvLotes.DataBind();
                gvLinhagens.DataBind();

                AtualizaTotais();

                lblPerguntaConfirmaExclusaoImportacao.Visible = false;
                lbtnSim.Visible = false;
                lbtnNao.Visible = false;
            }
            catch (Exception ex)
            {
                lblMensagem.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem.Text = "Erro ao Deletar Importação: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem.Text = "Erro ao Deletar Importação: " + ex.Message;
            }
        }

        protected void lbtnNao_Click(object sender, EventArgs e)
        {
            lblPerguntaConfirmaExclusaoImportacao.Visible = false;
            lbtnSim.Visible = false;
            lbtnNao.Visible = false;
        }

        public void DeletaMovEstq(MOV_ESTQ movestq)
        {
            var listaLotes = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                        .ToList();

            foreach (var lote in listaLotes)
            {
                bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.DeleteObject(lote);
            }

            var listaLocal = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var local in listaLocal)
            {
                bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.DeleteObject(local);
            }

            var listaItens = bdApolo.ITEM_MOV_ESTQ.Where(c => c.EmpCod == movestq.EmpCod && c.MovEstqChv == movestq.MovEstqChv)
                .ToList();

            foreach (var itens in listaItens)
            {
                bdApolo.ITEM_MOV_ESTQ.DeleteObject(itens);
            }

            bdApolo.MOV_ESTQ.DeleteObject(movestq);
        }

        #endregion

        #endregion

        #region Check Methods

        public void VerificaImportacaoApolo(DateTime dataIncubacao)
        {
            //string incubatorio = ddlIncubatorios.SelectedValue;

            //var lista = bdSQLServer.HATCHERY_EGG_DATA
            //    .Where(h => h.Set_date == dataIncubacao && h.Status == "Importado" && h.Hatch_loc == incubatorio
            //        && h.ImportadoApolo == "Sim")
            //    .GroupBy(h => new
            //    {
            //        h.Company,
            //        h.Region,
            //        h.Location,
            //        h.Set_date,
            //        h.Hatch_loc,
            //        h.Flock_id,
            //        h.Lay_date,
            //        h.Variety,
            //        h.ClassOvo
            //    })
            //    .Select(h => new
            //    {
            //        type = h.Key,
            //        soma = h.Sum(x => x.Eggs_rcvd)
            //    })
            //    .ToList();

            //string empresaEstoque = "CH";
            //if (incubatorio.Equals("NM"))
            //    empresaEstoque = "PL";

            //ImportaIncubacao.Data.Apolo.EMPRESA_FILIAL empresa = bdApolo.EMPRESA_FILIAL
            //                        .Where(ef => ef.USERFLIPCod == empresaEstoque)
            //                        .FirstOrDefault();

            //string naturezaOperacao = "5.101";
            //decimal? valorUnitario = 0.25m;
            //if (incubatorio.Equals("NM"))
            //    valorUnitario = 0.90m;
            //string unidadeMedida = "UN";
            //short? posicaoUnidadeMedida = 1;
            //string tribCod = "040";
            //string itMovEstqClasFiscCodNbm = "04079000";
            //string clasFiscCod = "0000129";
            //string operacao = "Saída";

            //string usuario;
            //if (Session["login"].ToString().Equals("palves"))
            //    usuario = "RIOSOFT";
            //else
            //    usuario = Session["login"].ToString().ToUpper();

            //foreach (var item in lista)
            //{
            //    DateTime dataInicioPlanalto = Convert.ToDateTime("03/06/2016");
            //    if (item.type.Set_date <= dataInicioPlanalto && item.type.Hatch_loc.Equals("NM"))
            //    {
            //        var listaItensIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //                    .Where(h => h.Set_date == item.type.Set_date && h.Flock_id == item.type.Flock_id
            //                        && h.Lay_date == item.type.Lay_date && h.ImportadoApolo == "Sim"
            //                        && h.Status == "Importado").ToList();

            //        foreach (var itemIncubacao in listaItensIncubacao)
            //        {
            //            itemIncubacao.ImportadoApolo = "Sim";
            //        }
            //    }
            //    else
            //    {

            //        string entrou = "";
            //        if (item.type.Flock_id.Equals("HLP09-P095323B"))
            //        {
            //            entrou = "OK";
            //        }

            //        LOC_ARMAZ_ITEM_MOV_ESTQ locArmazItemMovEstq = new LOC_ARMAZ_ITEM_MOV_ESTQ();
            //        LOC_ARMAZ_ITEM_MOV_ESTQ locItemMovEstq = new LOC_ARMAZ_ITEM_MOV_ESTQ();
            //        LOC_ARMAZ_ITEM_MOV_ESTQ local = new LOC_ARMAZ_ITEM_MOV_ESTQ();
            //        CTRL_LOTE_ITEM_MOV_ESTQ lote = new CTRL_LOTE_ITEM_MOV_ESTQ();
            //        ITEM_MOV_ESTQ itemMov = new ITEM_MOV_ESTQ();
            //        ITEM_MOV_ESTQ itemMovEstq = new ITEM_MOV_ESTQ();
            //        MOV_ESTQ movEstq = new MOV_ESTQ();

            //        int posicaoHifen = item.type.Flock_id.IndexOf("-") + 1;
            //        int tamanho = item.type.Flock_id.Length - posicaoHifen;
            //        string flock = item.type.Flock_id.Substring(posicaoHifen, tamanho);

            //        PRODUTO produto = bdApolo.PRODUTO.Where(p => p.ProdNomeAlt1 == item.type.Variety).FirstOrDefault();

            //        string localEstq = item.type.Hatch_loc;
            //        if (incubatorio.Equals("NM"))
            //            localEstq = item.type.ClassOvo;

            //        LOC_ARMAZ locArmaz = bdApolo.LOC_ARMAZ.Where(l => l.USERCodigoFLIP == localEstq).FirstOrDefault();

            //        lote = bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ
            //                .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                    && i.ProdCodEstr == produto.ProdCodEstr
            //                    && i.CtrlLoteDataValid == item.type.Lay_date && i.CtrlLoteNum == flock
            //                    && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                        && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                            .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //                .FirstOrDefault();

            //        if (lote != null)
            //        {
            //            if (lote.CtrlLoteItMovEstqQtd != item.soma)
            //            {
            //                int saldo = VerificaSaldo(flock, item.type.Lay_date, locArmaz.LocArmazCodEstr) + Convert.ToInt32(lote.CtrlLoteItMovEstqQtd);
            //                if (saldo >= item.soma)
            //                {
            //                    local = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                        .Where(l => l.EmpCod == lote.EmpCod && l.MovEstqChv == lote.MovEstqChv && l.ProdCodEstr == lote.ProdCodEstr
            //                            && l.ItMovEstqSeq == lote.ItMovEstqSeq && l.LocArmazCodEstr == lote.LocArmazCodEstr).FirstOrDefault();

            //                    local.LocArmazItMovEstqQtd = local.LocArmazItMovEstqQtd - lote.CtrlLoteItMovEstqQtd;

            //                    itemMov = bdApolo.ITEM_MOV_ESTQ
            //                        .Where(l => l.EmpCod == lote.EmpCod && l.MovEstqChv == lote.MovEstqChv && l.ProdCodEstr == lote.ProdCodEstr
            //                            && l.ItMovEstqSeq == lote.ItMovEstqSeq).FirstOrDefault();

            //                    itemMov.ItMovEstqQtdProd = itemMov.ItMovEstqQtdProd - lote.CtrlLoteItMovEstqQtd;

            //                    lote.CtrlLoteItMovEstqQtd = item.soma;
            //                    lote.CtrlLoteItMovEstqQtdCalc = lote.CtrlLoteItMovEstqQtd;

            //                    local.LocArmazItMovEstqQtd = local.LocArmazItMovEstqQtd + lote.CtrlLoteItMovEstqQtd;
            //                    local.LocArmazItMovEstqQtdCalc = local.LocArmazItMovEstqQtd;
            //                    itemMov.ItMovEstqQtdProd = itemMov.ItMovEstqQtdProd + lote.CtrlLoteItMovEstqQtd;
            //                    itemMov.ItMovEstqQtdCalcProd = itemMov.ItMovEstqQtdProd;

            //                    var listaItensIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //                        .Where(h => h.Set_date == item.type.Set_date && h.Flock_id == item.type.Flock_id
            //                            && h.Lay_date == item.type.Lay_date && h.ImportadoApolo == "Sim"
            //                            && h.Status == "Importado").ToList();

            //                    foreach (var itemIncubacao in listaItensIncubacao)
            //                    {
            //                        itemIncubacao.ImportadoApolo = "Sim";
            //                    }

            //                    bdApolo.SaveChanges();
            //                }
            //                else
            //                {
            //                    var listaItensIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //                        .Where(h => h.Set_date == item.type.Set_date && h.Flock_id == item.type.Flock_id
            //                            && h.Lay_date == item.type.Lay_date && h.ImportadoApolo == "Sim"
            //                            && h.Status == "Importado").ToList();

            //                    foreach (var itemIncubacao in listaItensIncubacao)
            //                    {
            //                        itemIncubacao.ImportadoApolo = "S/ Saldo";
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            int saldo = VerificaSaldo(flock, item.type.Lay_date, locArmaz.LocArmazCodEstr);

            //            if (saldo >= item.soma)
            //            {
            //                // Verifica se Existe a movimentação neste Incubatório e Produto
            //                locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                    .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                        && i.ProdCodEstr == produto.ProdCodEstr
            //                        && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                            && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //                    .FirstOrDefault();

            //                if (locItemMovEstq != null)
            //                {
            //                    itemMovEstq = bdApolo.ITEM_MOV_ESTQ
            //                        .Where(im => im.EmpCod == locItemMovEstq.EmpCod && im.MovEstqChv == locItemMovEstq.MovEstqChv
            //                            && im.ProdCodEstr == locItemMovEstq.ProdCodEstr && im.ItMovEstqSeq == locItemMovEstq.ItMovEstqSeq)
            //                        .FirstOrDefault();

            //                    itemMovEstq.ItMovEstqQtdProd = itemMovEstq.ItMovEstqQtdProd + item.soma;
            //                    itemMovEstq.ItMovEstqQtdCalcProd = itemMovEstq.ItMovEstqQtdProd;

            //                    locItemMovEstq.LocArmazItMovEstqQtd = locItemMovEstq.LocArmazItMovEstqQtd + item.soma;
            //                    locItemMovEstq.LocArmazItMovEstqQtdCalc = locItemMovEstq.LocArmazItMovEstqQtd;

            //                    lote = service.InsereLote(locItemMovEstq.MovEstqChv, locItemMovEstq.EmpCod, itemMovEstq.TipoLancCod,
            //                            locItemMovEstq.ItMovEstqSeq, locItemMovEstq.ProdCodEstr, flock, item.type.Lay_date, item.soma, operacao,
            //                            itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos, locItemMovEstq.LocArmazCodEstr);

            //                    bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                }
            //                else
            //                {
            //                    // Verifica se Existe a movimentação neste Incubatório e não no Produto
            //                    locItemMovEstq = bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ
            //                        .Where(i => i.EmpCod == empresa.EmpCod && i.LocArmazCodEstr == locArmaz.LocArmazCodEstr
            //                            && bdApolo.MOV_ESTQ.Any(m => m.EmpCod == i.EmpCod && m.MovEstqChv == i.MovEstqChv
            //                                && m.MovEstqDataMovimento == dataIncubacao && bdApolo.TIPO_LANC
            //                                .Any(t => m.TipoLancCod == t.TipoLancCod && t.TipoLancNormTransf != "Transferência")))
            //                        .FirstOrDefault();

            //                    if (locItemMovEstq != null)
            //                    {
            //                        movEstq = bdApolo.MOV_ESTQ
            //                            .Where(m => m.EmpCod == locItemMovEstq.EmpCod && m.MovEstqChv == locItemMovEstq.MovEstqChv)
            //                            .FirstOrDefault();

            //                        itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                            item.type.Variety, naturezaOperacao, item.soma, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                        locArmazItemMovEstq =
            //                            service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                            itemMovEstq.ProdCodEstr, item.soma, locItemMovEstq.LocArmazCodEstr);

            //                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                        lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flock,
            //                            item.type.Lay_date, item.soma, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                            locArmazItemMovEstq.LocArmazCodEstr);

            //                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                    }
            //                    else
            //                    {
            //                        movEstq = service.InsereMovEstq(empresa.EmpCod, locArmaz.USERTipoLancSaidaInc, empresa.EntCod,
            //                            dataIncubacao, usuario);

            //                        bdApolo.MOV_ESTQ.AddObject(movEstq);

            //                        itemMovEstq = service.InsereItemMovEstq(movEstq.MovEstqChv,
            //                            movEstq.EmpCod, movEstq.TipoLancCod, movEstq.EntCod, movEstq.MovEstqDataMovimento,
            //                            item.type.Variety, naturezaOperacao, item.soma, valorUnitario, unidadeMedida, posicaoUnidadeMedida,
            //                            tribCod, itMovEstqClasFiscCodNbm, clasFiscCod);

            //                        bdApolo.ITEM_MOV_ESTQ.AddObject(itemMovEstq);

            //                        locArmazItemMovEstq =
            //                            service.InsereLocalArmazenagem(itemMovEstq.MovEstqChv, itemMovEstq.EmpCod, itemMovEstq.ItMovEstqSeq,
            //                            itemMovEstq.ProdCodEstr, item.soma, locArmaz.LocArmazCodEstr);

            //                        bdApolo.LOC_ARMAZ_ITEM_MOV_ESTQ.AddObject(locArmazItemMovEstq);

            //                        lote = service.InsereLote(locArmazItemMovEstq.MovEstqChv, locArmazItemMovEstq.EmpCod,
            //                            itemMovEstq.TipoLancCod, locArmazItemMovEstq.ItMovEstqSeq, locArmazItemMovEstq.ProdCodEstr, flock,
            //                            item.type.Lay_date, item.soma, operacao, itemMovEstq.ItMovEstqUnidMedCod, itemMovEstq.ItMovEstqUnidMedPos,
            //                            locArmazItemMovEstq.LocArmazCodEstr);

            //                        bdApolo.CTRL_LOTE_ITEM_MOV_ESTQ.AddObject(lote);
            //                    }
            //                }

            //                bdApolo.SaveChanges();

            //                var listaItensIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //                        .Where(h => h.Set_date == item.type.Set_date && h.Flock_id == item.type.Flock_id
            //                            && h.Lay_date == item.type.Lay_date && h.ImportadoApolo == "Sim"
            //                            && h.Status == "Importado").ToList();

            //                foreach (var itemIncubacao in listaItensIncubacao)
            //                {
            //                    itemIncubacao.ImportadoApolo = "Sim";
            //                }
            //            }
            //            else
            //            {
            //                var listaItensIncubacao = bdSQLServer.HATCHERY_EGG_DATA
            //                        .Where(h => h.Set_date == item.type.Set_date && h.Flock_id == item.type.Flock_id
            //                            && h.Lay_date == item.type.Lay_date && h.ImportadoApolo == "Sim"
            //                            && h.Status == "Importado").ToList();

            //                foreach (var itemIncubacao in listaItensIncubacao)
            //                {
            //                    itemIncubacao.ImportadoApolo = "S/ Saldo";
            //                }
            //            }
            //        }
            //    }
            //}

            //bdSQLServer.SaveChanges();
        }

        public int VerificaSaldo(string lote, DateTime dataProducao, string locArmazCodEstr)
        {
            int retorno = 0;

            CTRL_LOTE_LOC_ARMAZ saldo = bdApolo.CTRL_LOTE_LOC_ARMAZ
                .Where(c => c.CtrlLoteNum == lote && c.CtrlLoteDataValid == dataProducao && c.LocArmazCodEstr == locArmazCodEstr)
                .FirstOrDefault();

            if (saldo != null)
            {
                retorno = Convert.ToInt32(saldo.CtrlLoteLocArmazQtdSaldo);
            }

            return retorno;
        }

        public int VerificaEstoqueWEB(DateTime dataPrd, string numLote, int qtdOvos, string local,
            int qtdOvosDesconsiderar)
        {
            int retorno = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.LoteCompleto == numLote && w.DataProducao == dataPrd
                    && w.Local == local).FirstOrDefault();

            if (saldo != null)
            {
                if ((saldo.Qtde - qtdOvosDesconsiderar) < qtdOvos)
                {
                    retorno = Convert.ToInt32(saldo.Qtde);
                }
            }

            return retorno;
        }

        public int VerificaSaldoWEB(DateTime dataPrd, string numLote, string local)
        {
            int retorno = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            CTRL_LOTE_LOC_ARMAZ_WEB saldo = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.LoteCompleto == numLote && w.DataProducao == dataPrd
                    && w.Local == local).FirstOrDefault();

            if (saldo != null)
            {
                retorno = Convert.ToInt32(saldo.Qtde);
            }

            return retorno;
        }

        public int VerifyQtyByLayDatePeriod(string flockID, DateTime firstLayDate, DateTime lastLayDate)
        {
            int disponibleQty = 0;

            HLBAPPEntities hlbapp = new HLBAPPEntities();
            List<CTRL_LOTE_LOC_ARMAZ_WEB> listEggInventory = hlbapp.CTRL_LOTE_LOC_ARMAZ_WEB
                .Where(w => w.LoteCompleto == flockID
                    && w.DataProducao >= firstLayDate && w.DataProducao <= lastLayDate)
                .ToList();

            if (listEggInventory != null) disponibleQty = Convert.ToInt32(listEggInventory.Sum(s => s.Qtde));

            return disponibleQty;
        }

        public bool ExisteDEOSolicitacaoAjusteEstoqueAberto(string unidade)
        {
            bool retorno = false;

            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            var existe = hlbappSession.LayoutDiarioExpedicaos
                .Where(w => w.TipoDEO == "Solicitação Ajuste de Estoque"
                    && (
                        (w.Incubatorio == unidade)
                        ||
                        (hlbappSession.TIPO_CLASSFICACAO_OVO_02.Any(a => a.Unidade == unidade
                            && a.CodigoTipo == w.Incubatorio && a.AproveitamentoOvo == "Incubável"))
                       )
                    && w.Importado != "Conferido")
                .Count();

            if (existe > 0) retorno = true;

            return retorno;
        }

        #endregion

        #region Reports

        protected void rbListaExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["tipoRelatorio"] = rbListaExport.SelectedValue;
        }

        #endregion

        #region Calculate Methods

        public decimal CalculaMediaEstimadaPonderadaEclosao(string incubatorio, DateTime setDate,
            string loteCompleto)
        {
            var incubacao = bdSQLServer.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == incubatorio
                    && w.Set_date == setDate && w.Flock_id == loteCompleto)
                .GroupBy(g => new
                {
                    g.Hatch_loc,
                    g.Set_date,
                    g.Flock_id
                })
                .Select(s => new
                {
                    PintosEstimados = s.Sum(u => (u.Eggs_rcvd * (u.Estimate / 100.00m))),
                    TotalOvosIncubados = s.Sum(u => u.Eggs_rcvd)
                })
                .FirstOrDefault();

            decimal pintosEstimados = 0.00m;
            decimal totalOvosIncubados = 1.00m;
            if (incubacao != null)
            {
                pintosEstimados = Convert.ToDecimal(incubacao.PintosEstimados);
                totalOvosIncubados = Convert.ToDecimal(incubacao.TotalOvosIncubados);
            }

            return Convert.ToDecimal((pintosEstimados / totalOvosIncubados) * 100.00m);
        }

        public void RecalculaMediaEstimadaPonderadaEclosao()
        {
            var listaIncubacoesPlanalto = bdSQLServer.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == "NM")
                .GroupBy(g => new
                {
                    g.Hatch_loc,
                    g.Set_date,
                    g.Flock_id
                })
                .Select(s => new
                {
                    s.Key.Hatch_loc,
                    s.Key.Set_date,
                    s.Key.Flock_id
                })
                .OrderBy(o => o.Set_date).ThenBy(t => t.Flock_id)
                .ToList();

            foreach (var item in listaIncubacoesPlanalto)
            {
                decimal mediaIncubacao = CalculaMediaEstimadaPonderadaEclosao(item.Hatch_loc, item.Set_date,
                    item.Flock_id);

                hatcheryFlockData.UpdateEstimate(mediaIncubacao, "HYBR", "BR", 
                    "PP", item.Set_date, item.Hatch_loc, item.Flock_id);
            }
        }

        protected void btnRecalculaEstimativa_Click(object sender, EventArgs e)
        {
            RecalculaMediaEstimadaPonderadaEclosao();
        }

        public void AtualizaQtdeIncubadaNascimentoWEB(string hatchLoc, DateTime setDate,
            string lote, string setter, string classOvo)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();

            int qtde = 0;
            var listaIncubacoes = hlbapp.HATCHERY_EGG_DATA
                 .Where(w => w.Hatch_loc == hatchLoc && w.Set_date == setDate
                     && w.Flock_id == lote && w.Machine == setter && w.ClassOvo == classOvo)
                 .ToList();

            if (listaIncubacoes.Count > 0)
                qtde = Convert.ToInt32(listaIncubacoes.Sum(s => s.Eggs_rcvd));

            HATCHERY_FLOCK_SETTER_DATA nascimento = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                    && w.Flock_id == lote && w.Setter == setter && w.ClassOvo == classOvo)
                .FirstOrDefault();

            if (nascimento != null)
            {
                nascimento.Qtde_Incubada = qtde;
                hlbapp.SaveChanges();
            }
        }

        public void AtualizaNascimentoWEB(string hatchLoc, DateTime setDate)
        {
            HLBAPPEntities hlbapp = new HLBAPPEntities();
            var lista = hlbapp.HATCHERY_EGG_DATA
                .Where(w => w.Hatch_loc == hatchLoc && w.Set_date == setDate)
                .GroupBy(g => new
                {
                    g.Hatch_loc,
                    g.Set_date,
                    g.Flock_id,
                    g.Machine,
                    g.ClassOvo
                })
                .Select(s => new
                {
                    s.Key.Hatch_loc,
                    s.Key.Set_date,
                    s.Key.Flock_id,
                    s.Key.Machine,
                    s.Key.ClassOvo
                })
                .ToList();

            foreach (var item in lista)
            {
                AtualizaQtdeIncubadaNascimentoWEB(item.Hatch_loc, item.Set_date, item.Flock_id,
                    item.Machine, item.ClassOvo);
            }
        }

        #endregion

        #region Other Methods

        public void VerificaNaoImportados()
        {
            DateTime data = Calendar1.SelectedDate;
            string incubatorio = ddlIncubatorios.SelectedValue;
            int existe = 0;

            lblMensagem3.Text = "";

            #region Verifica Importação do Estoque WEB do Futuro

            string msgWEB = "";
            existe = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Hatch_loc == incubatorio && h.Set_date == data && h.ImportadoApolo == "Estoque Futuro"
                    && h.Status != "Importado")
                .Count();

            if (existe > 0)
            {
                msgWEB = "EXISTEM ITENS LANÇADOS COMO ESTOQUE FUTURO NÃO IMPORTADOS PARA O ESTOQUE! "
                    + "VERIFIQUE SE EXISTEM DIVERGÊNCIAS DE QUANTIDADE!";
            }

            #endregion

            #region Verifica Importação no FLIP

            string msgFLIP = "";
            existe = bdSQLServer.HATCHERY_EGG_DATA
                .Where(h => h.Hatch_loc == incubatorio && h.Set_date == data && (h.ImportadoFLIP != "Sim"
                    || h.ImportadoFLIP == null))
                .Count();

            if (existe > 0)
            {
                msgFLIP = lblMensagem3.Text +
                    Translate("EXISTEM ITENS PARA SEREM IMPORTADOS NO FLIP! VERIFIQUE A QUANTIDADE EM OPEN NO FLIP! "
                    + "AO ALTERAR DE DATA DE INCUBAÇÃO, APÓS O AJUSTE DO OPEN NO FLIP, ELE JÁ REINTEGRARÁ COM O FLIP!");
            }

            #endregion

            if (msgWEB.Equals("") && msgFLIP.Equals(""))
            {
                lblMensagem3.Visible = false;
                lblMensagem3.Text = "";
            }
            else
            {
                lblMensagem3.Visible = true;
                lblMensagem3.Text = msgWEB + " // " + msgFLIP;
            }
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

        public string Translate(string text)
        {
            string language = Session["language"].ToString();
            return AccountController.Translate(text.Replace(":",""), language);
        }

        public void ChangeLanguage()
        {
            string lg = Session["language"].ToString();

            if (lg != "pt-BR")
            {
                #region Change Header Egg Inventory Table

                GridView3.Columns[1].HeaderText = Translate(GridView3.Columns[1].HeaderText);
                GridView3.Columns[2].HeaderText = Translate(GridView3.Columns[2].HeaderText);
                GridView3.Columns[3].HeaderText = Translate(GridView3.Columns[3].HeaderText);
                GridView3.Columns[4].HeaderText = Translate(GridView3.Columns[4].HeaderText);
                GridView3.Columns[5].HeaderText = Translate(GridView3.Columns[5].HeaderText);
                GridView3.Columns[6].HeaderText = Translate(GridView3.Columns[6].HeaderText);
                GridView3.Columns[7].HeaderText = Translate(GridView3.Columns[7].HeaderText);
                GridView3.Columns[9].HeaderText = Translate(GridView3.Columns[9].HeaderText);
                GridView3.Columns[10].HeaderText = Translate(GridView3.Columns[10].HeaderText);
                GridView3.Columns[11].HeaderText = Translate(GridView3.Columns[11].HeaderText);
                GridView3.Columns[12].HeaderText = Translate(GridView3.Columns[12].HeaderText);
                GridView3.Columns[13].HeaderText = Translate(GridView3.Columns[13].HeaderText);
                GridView3.Columns[14].HeaderText = Translate(GridView3.Columns[14].HeaderText);

                #endregion

                #region Change Header Incubation Table

                GridView1.Columns[3].HeaderText = Translate(GridView1.Columns[3].HeaderText);
                GridView1.Columns[4].HeaderText = Translate(GridView1.Columns[4].HeaderText);
                GridView1.Columns[5].HeaderText = Translate(GridView1.Columns[5].HeaderText);
                GridView1.Columns[6].HeaderText = Translate(GridView1.Columns[6].HeaderText);
                GridView1.Columns[7].HeaderText = Translate(GridView1.Columns[7].HeaderText);
                GridView1.Columns[8].HeaderText = Translate(GridView1.Columns[8].HeaderText);
                GridView1.Columns[9].HeaderText = Translate(GridView1.Columns[9].HeaderText);
                GridView1.Columns[10].HeaderText = Translate(GridView1.Columns[10].HeaderText);
                GridView1.Columns[11].HeaderText = Translate(GridView1.Columns[11].HeaderText);
                GridView1.Columns[12].HeaderText = Translate(GridView1.Columns[12].HeaderText);
                GridView1.Columns[13].HeaderText = Translate(GridView1.Columns[13].HeaderText);
                GridView1.Columns[14].HeaderText = Translate(GridView1.Columns[14].HeaderText);
                GridView1.Columns[15].HeaderText = Translate(GridView1.Columns[15].HeaderText);
                GridView1.Columns[16].HeaderText = Translate(GridView1.Columns[16].HeaderText);
                GridView1.Columns[17].HeaderText = Translate(GridView1.Columns[17].HeaderText);
                GridView1.Columns[18].HeaderText = Translate(GridView1.Columns[18].HeaderText);

                #endregion

                #region Change Header Sorting Eggs Table

                gdvClasOvos.Columns[4].HeaderText = Translate(gdvClasOvos.Columns[4].HeaderText);
                gdvClasOvos.Columns[5].HeaderText = Translate(gdvClasOvos.Columns[5].HeaderText);
                gdvClasOvos.Columns[6].HeaderText = Translate(gdvClasOvos.Columns[6].HeaderText);
                gdvClasOvos.Columns[7].HeaderText = Translate(gdvClasOvos.Columns[7].HeaderText);
                gdvClasOvos.Columns[8].HeaderText = Translate(gdvClasOvos.Columns[8].HeaderText);
                gdvClasOvos.Columns[9].HeaderText = Translate(gdvClasOvos.Columns[9].HeaderText);
                gdvClasOvos.Columns[10].HeaderText = Translate(gdvClasOvos.Columns[10].HeaderText);
                gdvClasOvos.Columns[11].HeaderText = Translate(gdvClasOvos.Columns[11].HeaderText);
                gdvClasOvos.Columns[12].HeaderText = Translate(gdvClasOvos.Columns[12].HeaderText);
                gdvClasOvos.Columns[13].HeaderText = Translate(gdvClasOvos.Columns[13].HeaderText);

                #endregion

                #region Change Header Setter Conference Table

                gvMaquinas.Columns[0].HeaderText = Translate(gvMaquinas.Columns[0].HeaderText);
                gvMaquinas.Columns[1].HeaderText = Translate(gvMaquinas.Columns[1].HeaderText);
                gvMaquinas.Columns[2].HeaderText = Translate(gvMaquinas.Columns[2].HeaderText);

                #endregion

                #region Change Header Flock Conference Table

                gvLotes.Columns[0].HeaderText = Translate(gvLotes.Columns[0].HeaderText);
                gvLotes.Columns[1].HeaderText = Translate(gvLotes.Columns[1].HeaderText);
                gvLotes.Columns[2].HeaderText = Translate(gvLotes.Columns[2].HeaderText);

                #endregion

                #region Change Header Variety Conference Table

                gvLinhagens.Columns[0].HeaderText = Translate(gvLinhagens.Columns[0].HeaderText);
                gvLinhagens.Columns[1].HeaderText = Translate(gvLinhagens.Columns[1].HeaderText);
                gvLinhagens.Columns[2].HeaderText = Translate(gvLinhagens.Columns[2].HeaderText);

                #endregion

                #region Another Components

                hlBackHome.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("HL_Back_To_Home", Session["language"].ToString());
                Label5.Text = Translate(Label5.Text);
                Label9.Text = Translate(Label9.Text);
                Label4.Text = Translate(Label4.Text);
                hlExport.Text = Translate(hlExport.Text);
                Label3.Text = Translate(Label3.Text);
                Label8.Text = Translate(Label8.Text);

                foreach (ListItem item in DropDownList1.Items)
                {
                    item.Text = Translate(item.Text);
                }

                foreach (ListItem item in DropDownList2.Items)
                {
                    item.Text = Translate(item.Text);
                }

                foreach (ListItem item in ddlClassOvos.Items)
                {
                    item.Text = Translate(item.Text);
                }

                btn_Pesquisar.Text = Translate(btn_Pesquisar.Text);
                Label1.Text = Translate(Label1.Text);
                Label2.Text = Translate(Label2.Text);
                Button2.Text = Translate(Button2.Text);
                Label6.Text = Translate(Label6.Text);
                Label7.Text = Translate(Label7.Text);
                btn_AtualizaSetter.Text = Translate(btn_AtualizaSetter.Text);
                lblFiltroTipoEstoque.Text = Translate(lblFiltroTipoEstoque.Text);
                lblTotalOvosIncubados.Text = Translate(lblTotalOvosIncubados.Text);
                lblMaquinasUtilizadas.Text = Translate(lblMaquinasUtilizadas.Text);
                Label10.Text = Translate(Label10.Text);
                Label11.Text = Translate(Label11.Text);
                Label12.Text = Translate(Label12.Text);
                Label13.Text = Translate(Label13.Text);
                lblOvosClassificados.Text = Translate(lblOvosClassificados.Text);

                #endregion

                #region Form Model



                #endregion
            }
        }

        #endregion
    }
}