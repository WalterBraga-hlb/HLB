using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using MvcAppHyLinedoBrasil.Controllers;
using System.Globalization;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class NascimentoPorSetter : System.Web.UI.Page
    {
        #region Objetcs

        FLIPDataSet flip = new FLIPDataSet();
        HLBAPPEntities hlbapp = new HLBAPPEntities();

        #endregion

        #region Refresh Page

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

            #region Load Components Values

            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }

            if (ddlIncubatorios.SelectedValue.Equals("NM"))
            {
                ddlClassOvos.Enabled = true;
            }
            else
            {
                ddlClassOvos.Enabled = false;
            }

            #endregion

            if (IsPostBack == false)
            {
                #region Import Data by Hy-Line Colombia

                //DateTime minDate = Convert.ToDateTime("26/07/2018");
                //DateTime maxDate = Convert.ToDateTime("14/11/2019");
                //while (minDate <= maxDate)
                //{
                //    RefreshFLIP("MN", minDate);
                //    RefreshFLIP("PM", minDate);
                //    RefreshFLIP("MQ", minDate);
                //    RefreshFLIP("MA", minDate);
                //    minDate = minDate.AddDays(1);
                //}

                #endregion

                #region Load Page Components

                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                Calendar1.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Session["hatchLocal"] = ddlIncubatorios.SelectedValue;
                Session["setDate"] = Calendar1.SelectedDate;

                Session["linhagem"] = "";
                Session["age"] = "0";
                Session["qtde"] = "0";
                Session["dataNascimentoLote"] = "";

                //DateTime data = Convert.ToDateTime("09/07/2013");
                DateTime data = Calendar1.SelectedDate;

                AjustaTelaFechamento();
                ChangeLanguage();

                #endregion

                #region Load Hatcheries

                ddlIncubatorios.Items.Clear();

                FLIPDataSet.HATCHERY_CODESDataTable hDT = new FLIPDataSet.HATCHERY_CODESDataTable();
                HATCHERY_CODESTableAdapter hTA = new HATCHERY_CODESTableAdapter();

                hTA.Fill(hDT);

                foreach (var item in hDT)
                {
                    if (MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetGroup("HLBAPP-Acesso" + item.HATCH_LOC, (System.Collections.ArrayList)Session["Direitos"]))
                    {
                        ddlIncubatorios.Items.Add(new ListItem { Text = item.HATCH_DESC, Value = item.HATCH_LOC, Selected = false });
                    }
                }

                #endregion

                #region Refresh Data

                RefreshFLIP(ddlIncubatorios.SelectedValue, Calendar1.SelectedDate);

                #endregion
            }
        }

        public void AjustaTelaFechamento()
        {
            string company = GetFieldByHatchLoc(ddlIncubatorios.SelectedValue, "company");

            if (ExisteFechamentoEstoque(company, ddlIncubatorios.SelectedValue, Calendar1.SelectedDate))
            {
                string responsavel = GetFieldByHatchLoc(ddlIncubatorios.SelectedValue, "ORDENT_LOC");
                GridView3.Visible = false;
                lblMensagem3.Visible = true;
                lblMensagem3.Text = Translate("Estoque já fechado! Verifique com") + " "
                    + responsavel + " " + Translate("sobre a possibilidade da abertura!")
                    + Translate("Caso não seja aberto, o ajuste não pode ser realizado!");
            }
            else
            {
                GridView3.Visible = true;
                lblMensagem3.Visible = false;
                lblMensagem3.Text = "";
            }
        }

        #endregion

        #region Page Components

        protected void ddlIncubatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbtnExportar.Visible = false;
            AjustaTelaFechamento();
            RefreshComponents();
            RefreshFLIP(ddlIncubatorios.SelectedValue, Calendar1.SelectedDate);
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            lbtnExportar.Visible = false;
            AjustaTelaFechamento();
            RefreshComponents();
            RefreshFLIP(ddlIncubatorios.SelectedValue, Calendar1.SelectedDate);
        }

        public void RefreshComponents()
        {
            FormView1.ChangeMode(FormViewMode.ReadOnly);
            GridView3.SelectedIndex = -1;
            HatchFormDataSource.SelectParameters["Flock_id"].DefaultValue = "";
            HatchFormDataSource.SelectParameters["Machine"].DefaultValue = "";
            HatchFormDataSource.SelectParameters["Hatcher"].DefaultValue = "";
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = "";
        }

        #endregion

        #region Setting Eggs Table - GridView3

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
        
        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Load Variables

            FormView1.Visible = true;

            string hatchLoc = ddlIncubatorios.SelectedValue;
            DateTime setDate = Calendar1.SelectedDate;
            string flockID = GridView3.Rows[GridView3.SelectedIndex].Cells[3].Text;
            string setter = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            string hatcher = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            string classOvo = GridView3.Rows[GridView3.SelectedIndex].Cells[7].Text;

            #endregion

            #region Load Main Form Parameters

            HatchFormDataSource.SelectParameters["Flock_id"].DefaultValue = flockID;
            HatchFormDataSource.SelectParameters["Machine"].DefaultValue = setter;
            HatchFormDataSource.SelectParameters["Hatcher"].DefaultValue = hatcher;
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = classOvo;

            #endregion

            #region Load hatchery flock if exists

            HATCHERY_FLOCK_SETTER_DATA hatcherFlockSetter = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                    && w.Flock_id == flockID && w.Setter == setter && w.Hatcher == hatcher
                    && w.ClassOvo == classOvo)
                .FirstOrDefault();

            #endregion

            if (hatcherFlockSetter != null)
            {
                #region If exists hatchery flock, load form parameters with values

                HatchFormDataSource.SelectParameters["DataRetiradaReal"].DefaultValue =
                    Convert.ToDateTime(hatcherFlockSetter.DataRetiradaReal).ToShortDateString();
                HatchFormDataSource.SelectParameters["Hora_01_Retirada"].DefaultValue = (hatcherFlockSetter.Horario_01_Retirada == "" ? "00:00" : hatcherFlockSetter.Horario_01_Retirada);
                HatchFormDataSource.SelectParameters["Qtde_01_Retirada"].DefaultValue =
                    hatcherFlockSetter.Qtde_01_Retirada.ToString();
                HatchFormDataSource.SelectParameters["Hora_02_Retirada"].DefaultValue = (hatcherFlockSetter.Horario_02_Retirada == "" ? "00:00" : hatcherFlockSetter.Horario_02_Retirada);
                HatchFormDataSource.SelectParameters["Qtde_02_Retirada"].DefaultValue =
                    hatcherFlockSetter.Qtde_02_Retirada.ToString();
                HatchFormDataSource.SelectParameters["Eliminado"].DefaultValue =
                    hatcherFlockSetter.Eliminado.ToString();
                HatchFormDataSource.SelectParameters["Morto"].DefaultValue =
                    hatcherFlockSetter.Morto.ToString();
                HatchFormDataSource.SelectParameters["Macho"].DefaultValue =
                    hatcherFlockSetter.Macho.ToString();
                HatchFormDataSource.SelectParameters["Pintos_Vendaveis"].DefaultValue =
                    hatcherFlockSetter.Pintos_Vendaveis.ToString();
                HatchFormDataSource.SelectParameters["Refugo"].DefaultValue =
                    hatcherFlockSetter.Refugo.ToString();
                HatchFormDataSource.SelectParameters["Pinto_Terceira"].DefaultValue =
                    hatcherFlockSetter.Pinto_Terceira.ToString();

                HatchFormDataSource.SelectParameters["Infertil"].DefaultValue =
                    hatcherFlockSetter.Infertil.ToString();
                HatchFormDataSource.SelectParameters["Amostra"].DefaultValue =
                    hatcherFlockSetter.Amostra.ToString();
                HatchFormDataSource.SelectParameters["Inicial0a3"].DefaultValue =
                    hatcherFlockSetter.Inicial0a3.ToString();
                HatchFormDataSource.SelectParameters["Inicial4a7"].DefaultValue =
                    hatcherFlockSetter.Inicial4a7.ToString();
                HatchFormDataSource.SelectParameters["Media8a14"].DefaultValue =
                    hatcherFlockSetter.Media8a14.ToString();
                HatchFormDataSource.SelectParameters["Tardia15a18"].DefaultValue =
                    hatcherFlockSetter.Tardia15a18.ToString();
                HatchFormDataSource.SelectParameters["Tardia19a21"].DefaultValue =
                    hatcherFlockSetter.Tardia19a21.ToString();
                HatchFormDataSource.SelectParameters["Hemorragico"].DefaultValue =
                    hatcherFlockSetter.Hemorragico.ToString();
                HatchFormDataSource.SelectParameters["BicadoVivo"].DefaultValue =
                    hatcherFlockSetter.BicadoVivo.ToString();
                HatchFormDataSource.SelectParameters["BicadoMorto"].DefaultValue =
                    hatcherFlockSetter.BicadoMorto.ToString();
                HatchFormDataSource.SelectParameters["ContaminacaoBacteriana"].DefaultValue =
                    hatcherFlockSetter.ContaminacaoBacteriana.ToString();
                HatchFormDataSource.SelectParameters["Fungo"].DefaultValue =
                    hatcherFlockSetter.Fungo.ToString();
                HatchFormDataSource.SelectParameters["MaFormacaoCerebro"].DefaultValue =
                    hatcherFlockSetter.MaFormacaoCerebro.ToString();
                HatchFormDataSource.SelectParameters["MaFormacaoVisceras"].DefaultValue =
                    hatcherFlockSetter.MaFormacaoVisceras.ToString();
                HatchFormDataSource.SelectParameters["MalPosicionado"].DefaultValue =
                    hatcherFlockSetter.MalPosicionado.ToString();
                HatchFormDataSource.SelectParameters["Anormalidade"].DefaultValue =
                    hatcherFlockSetter.Anormalidade.ToString();
                HatchFormDataSource.SelectParameters["EliminadoCancelamento"].DefaultValue =
                    hatcherFlockSetter.EliminadoCancelamento.ToString();

                #region Campos Exclusivos Brasil

                HatchFormDataSource.SelectParameters["OvoVirado"].DefaultValue = hatcherFlockSetter.OvoVirado.ToString();
                HatchFormDataSource.SelectParameters["QuebradoTrincado"].DefaultValue = hatcherFlockSetter.QuebradoTrincado.ToString();
                HatchFormDataSource.SelectParameters["SetterEmbrio"].DefaultValue = (hatcherFlockSetter.SetterEmbrio == "" ? "A99" : hatcherFlockSetter.SetterEmbrio);
                HatchFormDataSource.SelectParameters["HatcherEmbrio"].DefaultValue = (hatcherFlockSetter.HatcherEmbrio == "" ? "A99" : hatcherFlockSetter.HatcherEmbrio);
                HatchFormDataSource.SelectParameters["QtdeNascidos"].DefaultValue = hatcherFlockSetter.QtdeNascidos.ToString();

                HatchFormDataSource.SelectParameters["PerdaUmidade"].DefaultValue = String.Format("{0:N2}",hatcherFlockSetter.PerdaUmidade);
                HatchFormDataSource.SelectParameters["ChickYeld"].DefaultValue = String.Format("{0:N2}", hatcherFlockSetter.ChickYeld);
                HatchFormDataSource.SelectParameters["TempCloaca"].DefaultValue = String.Format("{0:N2}", hatcherFlockSetter.TempCloaca);

                HatchFormDataSource.SelectParameters["Peso"].DefaultValue = String.Format("{0:N2}", hatcherFlockSetter.Peso);
                HatchFormDataSource.SelectParameters["Uniformidade"].DefaultValue = String.Format("{0:N2}", hatcherFlockSetter.Uniformidade);

                #endregion

                #endregion
            }
            else
            {
                #region If not exists hatchery flock, load form parameters with initial values

                HatchFormDataSource.SelectParameters["DataRetiradaReal"].DefaultValue =
                    Convert.ToDateTime(DateTime.Today).ToShortDateString();
                HatchFormDataSource.SelectParameters["Hora_01_Retirada"].DefaultValue = "00:00";
                HatchFormDataSource.SelectParameters["Qtde_01_Retirada"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Hora_02_Retirada"].DefaultValue = "00:00";
                HatchFormDataSource.SelectParameters["Qtde_02_Retirada"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Eliminado"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Morto"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Macho"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Pintos_Vendaveis"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Refugo"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Pinto_Terceira"].DefaultValue = "0";

                HatchFormDataSource.SelectParameters["Infertil"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Amostra"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Inicial0a3"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Inicial4a7"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Media8a14"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Tardia15a18"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Tardia19a21"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Hemorragico"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["BicadoVivo"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["BicadoMorto"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["ContaminacaoBacteriana"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Fungo"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["MaFormacaoCerebro"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["MaFormacaoVisceras"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["MalPosicionado"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["Anormalidade"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["EliminadoCancelamento"].DefaultValue = "0";

                #region Campos Exclusivos Brasil

                HatchFormDataSource.SelectParameters["OvoVirado"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["QuebradoTrincado"].DefaultValue = "0";
                HatchFormDataSource.SelectParameters["SetterEmbrio"].DefaultValue = "A99";
                HatchFormDataSource.SelectParameters["HatcherEmbrio"].DefaultValue = "A99";
                HatchFormDataSource.SelectParameters["QtdeNascidos"].DefaultValue = "0";

                HatchFormDataSource.SelectParameters["PerdaUmidade"].DefaultValue = String.Format("{0:N2}", 0);
                HatchFormDataSource.SelectParameters["ChickYeld"].DefaultValue = String.Format("{0:N2}", 0);
                HatchFormDataSource.SelectParameters["TempCloaca"].DefaultValue = String.Format("{0:N2}", 0);

                HatchFormDataSource.SelectParameters["Peso"].DefaultValue = String.Format("{0:N2}", 0);
                HatchFormDataSource.SelectParameters["Uniformidade"].DefaultValue = String.Format("{0:N2}", 0);

                #endregion

                #endregion
            }

            lblMensagem.Visible = false;
            FormView1.ChangeMode(FormViewMode.Edit);
        }

        #endregion

        #region Hatching Flock, Lay Date and Machine Form

        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                string company = GetFieldByHatchLoc(ddlIncubatorios.SelectedValue, "company");

                #region Translate Labels

                #region Nascimento

                System.Web.UI.WebControls.Label lblIncubadora = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblIncubadora");
                lblIncubadora.Text = Translate(lblIncubadora.Text);
                System.Web.UI.WebControls.Label lblNascedouro = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblNascedouro");
                lblNascedouro.Text = Translate(lblNascedouro.Text);
                System.Web.UI.WebControls.Label lblLoteCompleto = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblLoteCompleto");
                lblLoteCompleto.Text = Translate(lblLoteCompleto.Text);
                System.Web.UI.WebControls.Label lblNumLote = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblNumLote");
                lblNumLote.Text = Translate(lblNumLote.Text);
                System.Web.UI.WebControls.Label lblLinhagem = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblLinhagem");
                lblLinhagem.Text = Translate(lblLinhagem.Text);
                System.Web.UI.WebControls.Label lblIdadeLote = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblIdadeLote");
                lblIdadeLote.Text = Translate(lblIdadeLote.Text);
                System.Web.UI.WebControls.Label lblClasOvo = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblClasOvo");
                lblClasOvo.Text = Translate(lblClasOvo.Text);
                System.Web.UI.WebControls.Label lblOvosInc = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblOvosInc");
                lblOvosInc.Text = Translate(lblOvosInc.Text);
                System.Web.UI.WebControls.Label lblEliminadoSobras = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblEliminadoSobras");
                lblEliminadoSobras.Text = Translate(lblEliminadoSobras.Text);
                System.Web.UI.WebControls.Label lblRefugo = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblRefugo");
                lblRefugo.Text = Translate(lblRefugo.Text);
                System.Web.UI.WebControls.Label lblMacho = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMacho");
                lblMacho.Text = Translate(lblMacho.Text);
                System.Web.UI.WebControls.Label lblPintosVendaveis = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPintosVendaveis");
                lblPintosVendaveis.Text = Translate(lblPintosVendaveis.Text);
                System.Web.UI.WebControls.Label lblPinto3 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPinto3");
                lblPinto3.Text = Translate(lblPinto3.Text);
                System.Web.UI.WebControls.Label lblEliminadoCanc = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblEliminadoCanc");
                lblEliminadoCanc.Text = Translate(lblEliminadoCanc.Text);
                System.Web.UI.WebControls.Label lblDadosEclosao = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblDadosEclosao");
                lblDadosEclosao.Text = Translate(lblDadosEclosao.Text);

                System.Web.UI.WebControls.Label IncubadoraLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("IncubadoraLabel1");
                string incubadora = IncubadoraLabel1.Text;

                #endregion

                #region Múltiplo Estágio

                System.Web.UI.WebControls.Label lblRetirada = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblRetirada");
                lblRetirada.Text = Translate(lblRetirada.Text);
                System.Web.UI.WebControls.Label lblDataRetiradaReal = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblDataRetiradaReal");
                lblDataRetiradaReal.Text = Translate(lblDataRetiradaReal.Text);
                System.Web.UI.WebControls.Label lblHoraRetirada01 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHoraRetirada01");
                lblHoraRetirada01.Text = Translate(lblHoraRetirada01.Text);
                System.Web.UI.WebControls.Label lblQtdeRetirada01 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblQtdeRetirada01");
                lblQtdeRetirada01.Text = Translate(lblQtdeRetirada01.Text);
                System.Web.UI.WebControls.Label lblHoraRetirada02 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHoraRetirada02");
                lblHoraRetirada02.Text = Translate(lblHoraRetirada02.Text);
                System.Web.UI.WebControls.Label lblQtdeRetirada02 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblQtdeRetirada02");
                lblQtdeRetirada02.Text = Translate(lblQtdeRetirada02.Text);

                #endregion

                #region Embrio

                System.Web.UI.WebControls.Label lblEmbrio = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblEmbrio");
                lblEmbrio.Text = Translate(lblEmbrio.Text);
                System.Web.UI.WebControls.Label lblAmostra = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblAmostra");
                lblAmostra.Text = Translate(lblAmostra.Text);
                System.Web.UI.WebControls.Label lblInfertil = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblInfertil");
                lblInfertil.Text = Translate(lblInfertil.Text);
                System.Web.UI.WebControls.Label lblInicial0a3 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblInicial0a3");
                lblInicial0a3.Text = Translate(lblInicial0a3.Text);
                System.Web.UI.WebControls.Label lblInicial4a7 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblInicial4a7");
                lblInicial4a7.Text = Translate(lblInicial4a7.Text);
                System.Web.UI.WebControls.Label lblMedia8a14 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMedia8a14");
                lblMedia8a14.Text = Translate(lblMedia8a14.Text);
                System.Web.UI.WebControls.Label lblTardia15a18 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblTardia15a18");
                lblTardia15a18.Text = Translate(lblTardia15a18.Text);
                System.Web.UI.WebControls.Label lblTardia19a21 = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblTardia19a21");
                lblTardia19a21.Text = Translate(lblTardia19a21.Text);
                System.Web.UI.WebControls.Label lblHemorragico = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHemorragico");
                lblHemorragico.Text = Translate(lblHemorragico.Text);
                System.Web.UI.WebControls.Label lblBicadoVivo = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblBicadoVivo");
                lblBicadoVivo.Text = Translate(lblBicadoVivo.Text);
                System.Web.UI.WebControls.Label lblBicadoMorto = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblBicadoMorto");
                lblBicadoMorto.Text = Translate(lblBicadoMorto.Text);
                System.Web.UI.WebControls.Label lblContaminacaoBact = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblContaminacaoBact");
                lblContaminacaoBact.Text = Translate(lblContaminacaoBact.Text);
                System.Web.UI.WebControls.Label lblFungo = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblFungo");
                lblFungo.Text = Translate(lblFungo.Text);
                System.Web.UI.WebControls.Label lblMaFormacaoCerebro = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMaFormacaoCerebro");
                lblMaFormacaoCerebro.Text = Translate(lblMaFormacaoCerebro.Text);
                System.Web.UI.WebControls.Label lblMaFormacaoVisceras = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMaFormacaoVisceras");
                lblMaFormacaoVisceras.Text = Translate(lblMaFormacaoVisceras.Text);
                System.Web.UI.WebControls.Label lblMalPosicionado = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMalPosicionado");
                lblMalPosicionado.Text = Translate(lblMalPosicionado.Text);
                System.Web.UI.WebControls.Label lblAnormalidade = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblAnormalidade");
                lblAnormalidade.Text = Translate(lblAnormalidade.Text);

                #region Campos Exclusivos Brasil

                if (company != "HYBR")
                {
                    System.Web.UI.WebControls.Panel pnlEmbrioHYBRDados = (System.Web.UI.WebControls.Panel)FormView1.FindControl("pnlEmbrioHYBRDados");
                    pnlEmbrioHYBRDados.Visible = false;

                    System.Web.UI.WebControls.Panel pnlEmbrioHYBRMachines = (System.Web.UI.WebControls.Panel)FormView1.FindControl("pnlEmbrioHYBRMachines");
                    pnlEmbrioHYBRMachines.Visible = false;

                    System.Web.UI.WebControls.Panel pnlDadosNascimentoHYBR = (System.Web.UI.WebControls.Panel)FormView1.FindControl("pnlDadosNascimentoHYBR");
                    pnlDadosNascimentoHYBR.Visible = false;

                    //// Ovo Virado
                    //System.Web.UI.WebControls.Label lblOvoVirado = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblOvoVirado");
                    //lblOvoVirado.Visible = false;
                    //System.Web.UI.WebControls.TextBox txtOvoVirado = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtOvoVirado");
                    //txtOvoVirado.Visible = false;

                    //// Ovo Quebrado / Trincado
                    //System.Web.UI.WebControls.Label lblQuebradoTrincado = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblQuebradoTrincado");
                    //lblQuebradoTrincado.Visible = false;
                    //System.Web.UI.WebControls.TextBox txtQuebradoTrincado = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtQuebradoTrincado");
                    //txtQuebradoTrincado.Visible = false;

                    //// % Perda de Umidade
                    //System.Web.UI.WebControls.Label lblPerdaUmidade = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPerdaUmidade");
                    //lblPerdaUmidade.Visible = false;
                    //System.Web.UI.WebControls.TextBox txtPerdaUmidade = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtPerdaUmidade");
                    //txtPerdaUmidade.Visible = false;

                    //// % Chick Yeld
                    //System.Web.UI.WebControls.Label lblChickYeld = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblChickYeld");
                    //lblChickYeld.Visible = false;
                    //System.Web.UI.WebControls.TextBox txtChickYeld = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtChickYeld");
                    //txtChickYeld.Visible = false;

                    //// Temperatura de Cloaca
                    //System.Web.UI.WebControls.Label lblTempCloaca = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblTempCloaca");
                    //lblTempCloaca.Visible = false;
                    //System.Web.UI.WebControls.TextBox txtTempCloaca = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtTempCloaca");
                    //txtTempCloaca.Visible = false;
                }
                else
                {
                    #region 12/03/2021 - Chamado 74455 - Alterações da forma de lançamento

                    lblAnormalidade.Visible = false;
                    System.Web.UI.WebControls.TextBox AnormalidadeTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("AnormalidadeTextBox");
                    AnormalidadeTextBox.Visible = false;

                    lblMaFormacaoCerebro.Text = "Má formação:";

                    lblMaFormacaoVisceras.Visible = false;
                    System.Web.UI.WebControls.TextBox MaFormacaoViscerasTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MaFormacaoViscerasTextBox");
                    MaFormacaoViscerasTextBox.Visible = false;

                    #endregion

                    if (incubadora != "Todas")
                    {
                        System.Web.UI.WebControls.Panel pnlEmbrioHYBRMachines = (System.Web.UI.WebControls.Panel)FormView1.FindControl("pnlEmbrioHYBRMachines");
                        pnlEmbrioHYBRMachines.Visible = false;
                    }
                    else
                    {
                        // Incubadora Embrio
                        System.Web.UI.WebControls.TextBox txtIncubadoraEmbrio = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtIncubadoraEmbrio");
                        if (txtIncubadoraEmbrio.Text == "A99")
                        {
                            txtIncubadoraEmbrio.Text = "";
                        }

                        // Nascedouro Embrio
                        System.Web.UI.WebControls.TextBox txtNascedouroEmbrio = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtNascedouroEmbrio");
                        if (txtNascedouroEmbrio.Text == "A99")
                        {
                            txtNascedouroEmbrio.Text = "";
                        }
                    }
                }

                #endregion

                #endregion

                LinkButton UpdateButton = (LinkButton)FormView1.FindControl("UpdateButton");
                UpdateButton.Text = Translate(UpdateButton.Text);

                #endregion
            }
        }

        // Save Hatching
        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            HATCHERY_FLOCK_SETTER_DATA nascimento = new HATCHERY_FLOCK_SETTER_DATA();

            try
            {
                if (FormView1.CurrentMode == FormViewMode.Edit)
                {
                    #region Load Main Variables

                    DateTime setDate = Calendar1.SelectedDate;
                    string hatchLoc = ddlIncubatorios.SelectedValue;
                    HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                    hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
                    string location = flip.HATCHERY_CODES[0].LOCATION;
                    string company = GetFieldByHatchLoc(ddlIncubatorios.SelectedValue, "company");
                    string region = GetFieldByHatchLoc(ddlIncubatorios.SelectedValue, "region");

                    #endregion

                    #region Load Values by Form

                    #region Hatching Fields

                    System.Web.UI.WebControls.Label IncubadoraLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("IncubadoraLabel1");
                    string incubadora = IncubadoraLabel1.Text;
                    System.Web.UI.WebControls.Label NascedouroLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("NascedouroLabel1");
                    string nascedouro = NascedouroLabel1.Text;
                    System.Web.UI.WebControls.Label Flock_idLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("Flock_idLabel1");
                    string loteCompleto = Flock_idLabel1.Text;
                    System.Web.UI.WebControls.Label NumLoteLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("NumLoteLabel1");
                    string numLote = NumLoteLabel1.Text;
                    System.Web.UI.WebControls.Label VarietyLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("VarietyLabel1");
                    string linhagem = VarietyLabel1.Text;
                    System.Web.UI.WebControls.Label ClassOvoLabel1 = 
                        (System.Web.UI.WebControls.Label)FormView1.FindControl("ClassOvoLabel1");
                    string classOvo = ClassOvoLabel1.Text;
                    System.Web.UI.WebControls.Label Ovos_IncubadosLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("Ovos_IncubadosLabel1");
                    int qtdOvos = Convert.ToInt32(Ovos_IncubadosLabel1.Text);

                    System.Web.UI.WebControls.TextBox DataRetiradaRealTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("DataRetiradaRealTextBox");
                    DateTime dataRetiradaReal = Convert.ToDateTime(DataRetiradaRealTextBox.Text);
                    System.Web.UI.WebControls.TextBox Hora_01_RetiradaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Hora_01_RetiradaTextBox");
                    string horario01Retirada = Hora_01_RetiradaTextBox.Text;
                    System.Web.UI.WebControls.TextBox Qtde_01_RetiradaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Qtde_01_RetiradaTextBox");
                    int qtde01Retirada = Convert.ToInt32(Qtde_01_RetiradaTextBox.Text);
                    System.Web.UI.WebControls.TextBox Hora_02_RetiradaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Hora_02_RetiradaTextBox");
                    string horario02Retirada = Hora_02_RetiradaTextBox.Text;
                    System.Web.UI.WebControls.TextBox Qtde_02_RetiradaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Qtde_02_RetiradaTextBox");
                    int qtde02Retirada = Convert.ToInt32(Qtde_02_RetiradaTextBox.Text);

                    System.Web.UI.WebControls.TextBox EliminadoTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("EliminadoTextBox");
                    int eliminado = Convert.ToInt32(EliminadoTextBox.Text);
                    //System.Web.UI.WebControls.TextBox MortoTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MortoTextBox");
                    //int morto = Convert.ToInt32(MortoTextBox.Text);
                    System.Web.UI.WebControls.TextBox MachoTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MachoTextBox");
                    int macho = Convert.ToInt32(MachoTextBox.Text);
                    System.Web.UI.WebControls.TextBox Pintos_VendaveisTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Pintos_VendaveisTextBox");
                    int pintosVendaveis = Convert.ToInt32(Pintos_VendaveisTextBox.Text);
                    System.Web.UI.WebControls.TextBox RefugoTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("RefugoTextBox");
                    int refugo = Convert.ToInt32(RefugoTextBox.Text);
                    System.Web.UI.WebControls.TextBox Pinto_TerceiraTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Pinto_TerceiraTextBox");
                    int pintoTerceira = Convert.ToInt32(Pinto_TerceiraTextBox.Text);
                    System.Web.UI.WebControls.TextBox EliminadoCancelamentoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("EliminadoCancelamentoTextBox");
                    int eliminadoCancelamento = Convert.ToInt32(EliminadoCancelamentoTextBox.Text);

                    #region Verify Hatchability

                    //decimal hatchability = ((pintosVendaveis + refugo + pintoTerceira) / (qtdOvos * 1.0m) * 100.0m);/
                    // 05/03/2021 - Trocado para calcular o % de vendáveis devido solicitação da Ana Carolina Neves e autorização de Davi Nogueira
                    decimal hatchability = ((pintosVendaveis) / (qtdOvos * 1.0m) * 100.0m);
                    hatchability = Math.Round(hatchability, 2);
                    if (hatchability > 55)
                    {
                        lblMensagem.Visible = true;
                        lblMensagem.Text = Translate("A Eclosão de Pintos Vendáveis está maior que 55%") + "(" + String.Format("{0:N2}", hatchability) + "%)!" 
                            + Translate("Verifique!");
                        return;
                    }
                    else
                    {
                        lblMensagem.Text = "";
                        lblMensagem.Visible = false;
                    }

                    #endregion

                    #endregion

                    #region Embrio Fields

                    System.Web.UI.WebControls.TextBox AmostraTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("AmostraTextBox");
                    int amostra = Convert.ToInt32(AmostraTextBox.Text);
                    System.Web.UI.WebControls.TextBox InfertilTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("InfertilTextBox");
                    int infertil = Convert.ToInt32(InfertilTextBox.Text);
                    System.Web.UI.WebControls.TextBox Inicial0a3TextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Inicial0a3TextBox");
                    int inicial0a3 = Convert.ToInt32(Inicial0a3TextBox.Text);
                    System.Web.UI.WebControls.TextBox Inicial4a7TextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Inicial4a7TextBox");
                    int inicial4a7 = Convert.ToInt32(Inicial4a7TextBox.Text);
                    System.Web.UI.WebControls.TextBox Media8a14TextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Media8a14TextBox");
                    int media8a14 = Convert.ToInt32(Media8a14TextBox.Text);
                    System.Web.UI.WebControls.TextBox Tardia15a18TextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Tardia15a18TextBox");
                    int tardia15a18 = Convert.ToInt32(Tardia15a18TextBox.Text);
                    System.Web.UI.WebControls.TextBox Tardia19a21TextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Tardia19a21TextBox");
                    int tardia19a21 = Convert.ToInt32(Tardia19a21TextBox.Text);
                    System.Web.UI.WebControls.TextBox HemorragicoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("HemorragicoTextBox");
                    int hemorragico = Convert.ToInt32(HemorragicoTextBox.Text);
                    System.Web.UI.WebControls.TextBox BicadoVivoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("BicadoVivoTextBox");
                    int bicadoVivo = Convert.ToInt32(BicadoVivoTextBox.Text);
                    System.Web.UI.WebControls.TextBox BicadoMortoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("BicadoMortoTextBox");
                    int bicadoMorto = Convert.ToInt32(BicadoMortoTextBox.Text);
                    System.Web.UI.WebControls.TextBox ContaminacaoBacterianaTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("ContaminacaoBacterianaTextBox");
                    int contaminacaoBacteriana = Convert.ToInt32(ContaminacaoBacterianaTextBox.Text);
                    System.Web.UI.WebControls.TextBox FungoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("FungoTextBox");
                    int fungo = Convert.ToInt32(FungoTextBox.Text);
                    System.Web.UI.WebControls.TextBox MaFormacaoCerebroTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MaFormacaoCerebroTextBox");
                    int maFormacaoCerebro = Convert.ToInt32(MaFormacaoCerebroTextBox.Text);
                    int maFormacaoVisceras = 0;
                    System.Web.UI.WebControls.TextBox MalPosicionadoTextBox =
                        (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MalPosicionadoTextBox");
                    int malPosicionado = Convert.ToInt32(MalPosicionadoTextBox.Text);
                    int anormalidade = 0;

                    #region Campos Exclusivos Brasil

                    string incubadoraEmbrio = "";
                    string nascedouroEmbrio = "";
                    int ovoVirado = 0;
                    int ovoQuebradoTrincado = 0;
                    decimal perdaUmidade = 0;
                    decimal chickYeld = 0;
                    decimal tempCloaca = 0;
                    decimal peso = 0;
                    decimal uniformidade = 0;
                    int qtdeNascidos = 0;
                    if (company == "HYBR")
                    {
                        if (incubadora == "Todas")
                        {
                            //  Incubadora Embrio
                            System.Web.UI.WebControls.TextBox txtIncubadoraEmbrio = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtIncubadoraEmbrio");
                            incubadoraEmbrio = txtIncubadoraEmbrio.Text.Replace("___", "");

                            if (incubadoraEmbrio == "")
                            {
                                lblMensagem.Visible = true;
                                lblMensagem.Text = "Obrigatório informar a Incubadora do Embrio!";
                                txtIncubadoraEmbrio.Focus();
                                return;
                            }

                            //  Nascedouro Embrio
                            System.Web.UI.WebControls.TextBox txtNascedouroEmbrio = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtNascedouroEmbrio");
                            nascedouroEmbrio = txtNascedouroEmbrio.Text.Replace("___", "");

                            if (nascedouroEmbrio == "")
                            {
                                lblMensagem.Visible = true;
                                lblMensagem.Text = "Obrigatório informar o Nascedouro do Embrio!";
                                txtNascedouroEmbrio.Focus();
                                return;
                            }
                        }

                        // Ovo Virado
                        System.Web.UI.WebControls.TextBox txtOvoVirado = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtOvoVirado");
                        ovoVirado = Convert.ToInt32(txtOvoVirado.Text);

                        // Ovo Trincado / Quebrado
                        System.Web.UI.WebControls.TextBox txtQuebradoTrincado = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtQuebradoTrincado");
                        ovoQuebradoTrincado = Convert.ToInt32(txtQuebradoTrincado.Text);

                        // % Perda de Umidade
                        System.Web.UI.WebControls.TextBox txtPerdaUmidade = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtPerdaUmidade");
                        perdaUmidade = Convert.ToDecimal(txtPerdaUmidade.Text);

                        // % Chick Yeld
                        System.Web.UI.WebControls.TextBox txtChickYeld = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtChickYeld");
                        chickYeld = Convert.ToDecimal(txtChickYeld.Text);

                        // Temperatura de Cloaca
                        System.Web.UI.WebControls.TextBox txtTempCloaca = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtTempCloaca");
                        tempCloaca = Convert.ToDecimal(txtTempCloaca.Text);

                        // Qtde. Nascidos
                        System.Web.UI.WebControls.TextBox txtQtdeNascidos = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtQtdeNascidos");
                        qtdeNascidos = Convert.ToInt32(txtQtdeNascidos.Text);

                        // Peso
                        System.Web.UI.WebControls.TextBox txtPeso = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtPeso");

                        // Uniformidade
                        System.Web.UI.WebControls.TextBox txtUniformidade = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("txtUniformidade");

                        #region Verifica Peso

                        if (txtPeso.Text == "" || Convert.ToDecimal(txtPeso.Text) < 0)
                        {
                            lblMensagem.Visible = true;
                            txtPeso.Focus();
                            lblMensagem.Text = "Obrigatório inserir o Peso ou valor incorreto! Verifique!";
                            return;
                        }
                        else
                        {
                            peso = Convert.ToDecimal(txtPeso.Text);
                            lblMensagem.Text = "";
                            lblMensagem.Visible = false;
                        }

                        #endregion

                        #region Verifica Uniformidade

                        if (txtUniformidade.Text == "" || Convert.ToDecimal(txtUniformidade.Text) < 0 || Convert.ToDecimal(txtUniformidade.Text) > 100)
                        {
                            lblMensagem.Visible = true;
                            txtUniformidade.Focus();
                            lblMensagem.Text = "Obrigatório inserir a Uniformidade ou valor incorreto! Verifique!";
                            return;
                        }
                        else
                        {
                            uniformidade = Convert.ToDecimal(txtUniformidade.Text);
                            lblMensagem.Text = "";
                            lblMensagem.Visible = false;
                        }

                        #endregion

                        #region Verifica se Qtde. Amostra = QtdeEmbrio

                        int qtdeEmbrio = infertil + inicial0a3 + inicial4a7 + media8a14 + tardia15a18 + tardia19a21 + hemorragico + bicadoVivo + bicadoMorto
                            + contaminacaoBacteriana + fungo + maFormacaoCerebro + maFormacaoVisceras + malPosicionado + anormalidade + ovoVirado + ovoQuebradoTrincado
                            + qtdeNascidos;

                        if (qtdeEmbrio != amostra)
                        {
                            lblMensagem.Visible = true;
                            lblMensagem.Text = "A qtde. da Amostra é diferente das quantidades informadas no Embrio! Verifique!";
                            return;
                        }
                        else
                        {
                            lblMensagem.Text = "";
                            lblMensagem.Visible = false;
                        }

                        #endregion
                    }
                    else
                    {
                        System.Web.UI.WebControls.TextBox MaFormacaoViscerasTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MaFormacaoViscerasTextBox");
                        maFormacaoVisceras = Convert.ToInt32(MaFormacaoViscerasTextBox.Text);
                        System.Web.UI.WebControls.TextBox AnormalidadeTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("AnormalidadeTextBox");
                        anormalidade = Convert.ToInt32(AnormalidadeTextBox.Text);
                    }

                    #endregion

                    #endregion

                    #endregion

                    #region Insert in WEB

                    nascimento = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                        .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                            && w.Flock_id == loteCompleto && w.Setter == incubadora && w.ClassOvo == classOvo
                            && w.Hatcher == nascedouro)
                        .FirstOrDefault();

                    string operacao = "Insert";

                    if (nascimento == null)
                    {
                        nascimento = new HATCHERY_FLOCK_SETTER_DATA();
                    }
                    else
                    {
                        operacao = "Update";

                        #region Deleta valores no FLIP

                        UpdateHatchingDataFLIP(company, region, location, hatchLoc, setDate, loteCompleto,
                            nascimento, "Delete");

                        #endregion
                    }

                    #region Hatching Fields

                    nascimento.Hatch_Loc = hatchLoc;
                    nascimento.Set_date = setDate;
                    nascimento.Flock_id = loteCompleto;
                    nascimento.NumLote = numLote;
                    nascimento.Setter = incubadora;
                    nascimento.Hatcher = nascedouro;
                    nascimento.ClassOvo = classOvo;
                    nascimento.Eliminado = eliminado;
                    nascimento.Morto = 0;
                    nascimento.Macho = macho;
                    nascimento.Pintos_Vendaveis = pintosVendaveis;
                    nascimento.Refugo = refugo;
                    nascimento.Pinto_Terceira = pintoTerceira;
                    nascimento.Qtde_Incubada = qtdOvos;
                    nascimento.DataRetiradaReal = dataRetiradaReal;
                    nascimento.Horario_01_Retirada = horario01Retirada;
                    nascimento.Qtde_01_Retirada = qtde01Retirada;
                    nascimento.Horario_02_Retirada = horario02Retirada;
                    nascimento.Qtde_02_Retirada = qtde02Retirada;
                    nascimento.Variety = linhagem;
                    nascimento.EliminadoCancelamento = eliminadoCancelamento;

                    #endregion

                    #region Embrio Fields

                    nascimento.Amostra = amostra;
                    nascimento.Infertil = infertil;
                    nascimento.Inicial0a3 = inicial0a3;
                    nascimento.Inicial4a7 = inicial4a7;
                    nascimento.Media8a14 = media8a14;
                    nascimento.Tardia15a18 = tardia15a18;
                    nascimento.Tardia19a21 = tardia19a21;
                    nascimento.Hemorragico = hemorragico;
                    nascimento.BicadoVivo = bicadoVivo;
                    nascimento.BicadoMorto = bicadoMorto;
                    nascimento.ContaminacaoBacteriana = contaminacaoBacteriana;
                    nascimento.Fungo = fungo;
                    nascimento.MaFormacaoCerebro = maFormacaoCerebro;
                    nascimento.MaFormacaoVisceras = maFormacaoVisceras;
                    nascimento.MalPosicionado = malPosicionado;
                    nascimento.Anormalidade = anormalidade;

                    #region Campos Exclusivos Brasil

                    nascimento.SetterEmbrio = incubadoraEmbrio;
                    nascimento.HatcherEmbrio = nascedouroEmbrio;
                    nascimento.OvoVirado = ovoVirado;
                    nascimento.QuebradoTrincado = ovoQuebradoTrincado;
                    nascimento.PerdaUmidade = perdaUmidade;
                    nascimento.ChickYeld = chickYeld;
                    nascimento.TempCloaca = tempCloaca;
                    nascimento.QtdeNascidos = qtdeNascidos;

                    nascimento.Peso = peso;
                    nascimento.Uniformidade = uniformidade;

                    #endregion

                    #endregion

                    if (nascimento.ID.Equals(0)) hlbapp.HATCHERY_FLOCK_SETTER_DATA.AddObject(nascimento);

                    #region Insert LOG

                    InsereLOGHatcheryFlockSetterData(nascimento, DateTime.Now, operacao, 
                        Session["usuario"].ToString());

                    #endregion

                    #endregion

                    #region Insert values FLIP

                    UpdateHatchingDataFLIP(company, region, location, hatchLoc, setDate, loteCompleto, nascimento, "Insert");

                    #endregion

                    #region Save Changes in WEB

                    hlbapp.SaveChanges();

                    #endregion

                    #region Update Screen Components

                    EggInvDataSource.EnableCaching = false;
                    GridView3.DataBind();
                    EggInvDataSource.EnableCaching = true;
                    gvLotes.DataBind();
                    gvMaquinas.DataBind();
                    gvLinhagens.DataBind();
                    gvLotes0.DataBind();
                    gvMaquinas0.DataBind();
                    gvLinhagens0.DataBind();

                    RefreshComponents();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region If error, input LOG and show error message

                InsereLOGHatcheryFlockSetterData(nascimento, DateTime.Now, "Erro", Session["usuario"].ToString());

                lblMensagem.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem.Text = "Erro ao Transferir: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem.Text = "Erro ao Transferir: " + ex.Message;

                #endregion
            }
        }

        #endregion

        #region BD Methods

        #region FLIP
        
        #region New Methods

        #region Set Methods

        public void UpdateHatchingDataFLIP(string company, string region, string location, string hatchLoc, 
            DateTime setDate, string flockID, HATCHERY_FLOCK_SETTER_DATA hatchDataSetter, string operation)
        {
            #region Load General Variables

            string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR"))
                + hatchLoc + flockID;

            int qtyHatchingChicks = Convert.ToInt32(hatchDataSetter.Pintos_Vendaveis + hatchDataSetter.Refugo
                + hatchDataSetter.Pinto_Terceira);

            #endregion

            if (company == "HYBR")
            {
                #region HYBR

                #region Load BD objects

                FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, flockID);

                #endregion

                #region Nascimento Mais Cedo e Mais Tarde

                HLBAPPEntities hlbapp = new HLBAPPEntities();

                var listaNascimentoLote = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate && w.Flock_id == flockID)
                    .ToList();

                List<DateTime> listaData = new List<DateTime>();

                foreach (var item in listaNascimentoLote)
                {
                    if (item.DataRetiradaReal != null)
                    {
                        string dataRetirada = Convert.ToDateTime(item.DataRetiradaReal).ToString("dd/MM/yyyy");
                        DateTime data = new DateTime();
                        if (DateTime.TryParse(dataRetirada + " " + item.Horario_01_Retirada,
                            out data))
                        {
                            listaData.Add(data);
                        }
                    }
                }

                DateTime? dataNascimentoMaisCedo = null;
                DateTime? dataNascimentoMaisTarde = null;
                if (hatchDataSetter.DataRetiradaReal != null)
                {
                    dataNascimentoMaisCedo = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    dataNascimentoMaisTarde = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                }

                string horaNascimentoMaisCedo = "";
                string horaNascimentoMaisTarde = "";

                if (hatchDataSetter.Horario_01_Retirada != null)
                {
                    horaNascimentoMaisCedo = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                    horaNascimentoMaisTarde = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                }

                if (listaData.Count > 0)
                {
                    dataNascimentoMaisCedo = listaData.Min(m => m);
                    dataNascimentoMaisTarde = listaData.Max(m => m);
                    horaNascimentoMaisCedo = listaData.Min(m => m).ToString("HH:mm");
                    horaNascimentoMaisTarde = listaData.Max(m => m).ToString("HH:mm");
                }

                #endregion

                if (hfdDT.Count > 0)
                {
                    FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    if (operation.Equals("Insert"))
                    {
                        #region Insert

                        hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL + qtyHatchingChicks;
                        hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 + qtyHatchingChicks
                            + Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                        hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                        if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                        hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }
                    else if (operation.Equals("Delete"))
                    {
                        #region Delete

                        hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                        hatchData.ACTUAL = hatchData.ACTUAL - qtyHatchingChicks;
                        hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo
                            + hatchDataSetter.Pinto_Terceira);
                        hatchData.NUM_17 = hatchData.NUM_17 - qtyHatchingChicks
                            - Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                        hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                        if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                        hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }

                    hfdTA.Update(hatchData);
                }

                #endregion
            }
            else if (company == "HYCL")
            {
                #region HYCL

                #region Load BD objects

                ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable();
                ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                    new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, flockID);

                #endregion

                if (hfdDT.Count > 0)
                {
                    ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    if (operation.Equals("Insert"))
                    {
                        #region Insert

                        if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                        hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                        if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                        hatchData.ACTUAL = hatchData.ACTUAL + qtyHatchingChicks;
                        if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                        hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira);
                        if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                        hatchData.NUM_17 = hatchData.NUM_17 + qtyHatchingChicks + Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }
                    else if (operation.Equals("Delete"))
                    {
                        #region Delete

                        if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                        hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                        if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                        hatchData.ACTUAL = hatchData.ACTUAL - qtyHatchingChicks;
                        if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                        hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira);
                        if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                        hatchData.NUM_17 = hatchData.NUM_17 - qtyHatchingChicks - Convert.ToInt32(hatchDataSetter.Macho);
                        if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                        hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                        if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                        hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                        if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                        hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                        if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                        hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                        if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                        hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                        if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                        hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                        if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                        hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                        if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                        hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                        if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                        hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                        if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                        hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                        if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                        hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                        if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                        hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                        if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                        hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                        if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                        hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                        if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                        hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                        if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                        hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                        if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                        hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);

                        #endregion
                    }

                    hfdTA.Update(hatchData);
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable hedDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable();

                hedTA.FillByFlockData(hedDT, company, region, location, setDate, hatchLoc, flockID, null);

                var listGroupByFlocks = hedDT
                    .GroupBy(g => new
                    {
                        g.FLOCK_ID
                    })
                    .Select(s => new
                    {
                        s.Key.FLOCK_ID,
                        HatchEggTotal = s.Sum(m => m.EGGS_RCVD)
                    })
                    .ToList();

                var hatchEggAll = listGroupByFlocks.Sum(s => s.HatchEggTotal);

                foreach (var item in listGroupByFlocks)
                {
                    #region Load BD objects

                    ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                        new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable();
                    ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                        new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                    hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, item.FLOCK_ID);

                    #endregion

                    if (hfdDT.Count > 0)
                    {
                        var flocksQty = item.HatchEggTotal / hatchEggAll;

                        ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                        if (operation.Equals("Insert"))
                        {
                            #region Insert

                            if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                            hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado * flocksQty);
                            if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                            hatchData.ACTUAL = hatchData.ACTUAL + (qtyHatchingChicks * flocksQty);
                            if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                            hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32((hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira) * flocksQty);
                            if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                            hatchData.NUM_17 = hatchData.NUM_17 + ((qtyHatchingChicks + Convert.ToInt32(hatchDataSetter.Macho)) * flocksQty);
                            if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                            hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra * flocksQty);
                            if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                            hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil * flocksQty);
                            if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                            hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3 * flocksQty);
                            if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                            hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7 * flocksQty);
                            if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                            hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14 * flocksQty);
                            if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                            hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18 * flocksQty);
                            if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                            hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21 * flocksQty);
                            if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                            hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo * flocksQty);
                            if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                            hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto * flocksQty);
                            if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                            hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana * flocksQty);
                            if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                            hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo * flocksQty);
                            if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                            hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro * flocksQty);
                            if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                            hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras * flocksQty);
                            if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                            hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico * flocksQty);
                            if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                            hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade * flocksQty);
                            if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                            hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado * flocksQty);
                            if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                            hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento * flocksQty);

                            #endregion
                        }
                        else if (operation.Equals("Delete"))
                        {
                            #region Delete

                            if (hatchData.IsNUM_1Null()) hatchData.NUM_1 = 0;
                            hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado) * flocksQty;
                            if (hatchData.IsACTUALNull()) hatchData.ACTUAL = 0;
                            hatchData.ACTUAL = hatchData.ACTUAL - (qtyHatchingChicks * flocksQty);
                            if (hatchData.IsNUM_2Null()) hatchData.NUM_2 = 0;
                            hatchData.NUM_2 = hatchData.NUM_2 - ((Convert.ToInt32(hatchDataSetter.Refugo + hatchDataSetter.Pinto_Terceira)) * flocksQty);
                            if (hatchData.IsNUM_17Null()) hatchData.NUM_17 = 0;
                            hatchData.NUM_17 = hatchData.NUM_17 - ((qtyHatchingChicks - Convert.ToInt32(hatchDataSetter.Macho)) * flocksQty);
                            if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                            hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra * flocksQty);
                            if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                            hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil * flocksQty);
                            if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                            hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3 * flocksQty);
                            if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                            hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7 * flocksQty);
                            if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                            hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14 * flocksQty);
                            if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                            hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18 * flocksQty);
                            if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                            hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21 * flocksQty);
                            if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                            hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo * flocksQty);
                            if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                            hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto * flocksQty);
                            if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                            hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana * flocksQty);
                            if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                            hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo * flocksQty);
                            if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                            hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro * flocksQty);
                            if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                            hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras * flocksQty);
                            if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                            hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico * flocksQty);
                            if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                            hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade * flocksQty);
                            if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                            hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado * flocksQty);
                            if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                            hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento * flocksQty);

                            #endregion
                        }

                        hfdTA.Update(hatchData);
                    }
                }

                #endregion
            }
        }

        public void RefreshFLIP(string hatchLoc, DateTime setDate)
        {
            #region Load Values

            string company = GetFieldByHatchLoc(hatchLoc, "company");
            string region = GetFieldByHatchLoc(hatchLoc, "region");
            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
            hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
            string location = flip.HATCHERY_CODES[0].LOCATION;

            var listDelete = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Set_date == setDate && w.Hatch_Loc == hatchLoc)
                .GroupBy(g => new { g.Set_date, g.Flock_id })
                .ToList();

            var listInsert = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Set_date == setDate && w.Hatch_Loc == hatchLoc)
                .ToList();

            #endregion

            // Desativar para não atualizar NG e Ajapi devido eles fazerem pelo FLIP
            //if (hatchLoc != "CH" && hatchLoc != "TB")
            if (hatchLoc != "PH")
            {
                #region Delete Values

                if (company == "HYBR")
                {
                    #region HYBR

                    if (!ExisteFechamentoEstoque(company, hatchLoc, setDate))
                    {
                        #region Delete Values

                        foreach (var item in listDelete)
                        {
                            FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                            HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                            hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                                ddlIncubatorios.SelectedValue, item.Key.Flock_id);

                            if (hfdDT.Count > 0)
                            {
                                FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                                hatchData.NUM_1 = 0;
                                hatchData.ACTUAL = 0;
                                hatchData.NUM_2 = 0;
                                hatchData.NUM_17 = 0;
                                hatchData.NUM_13 = 0;
                                hatchData.NUM_19 = 0;
                                hatchData.NUM_20 = 0;
                                hatchData.NUM_4 = 0;
                                hatchData.NUM_5 = 0;
                                hatchData.NUM_6 = 0;
                                hatchData.NUM_7 = 0;
                                hatchData.NUM_8 = 0;
                                hatchData.NUM_21 = 0;
                                hatchData.NUM_11 = 0;
                                hatchData.NUM_10 = 0;
                                hatchData.NUM_24 = 0;
                                hatchData.NUM_23 = 0;
                                hatchData.NUM_9 = 0;
                                hatchData.NUM_12 = 0;
                                hatchData.NUM_16 = 0;
                                hatchData.NUM_28 = 0;

                                hfdTA.Update(hatchData);
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else if (company == "HYCL")
                {
                    #region HYCL

                    if (!ExisteFechamentoEstoque(company, hatchLoc, setDate))
                    {
                        #region Delete Values

                        foreach (var item in listDelete)
                        {
                            ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                                new ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATADataTable();
                            ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                                new ImportaIncubacao.Data.FLIP.CLFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                            hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                                ddlIncubatorios.SelectedValue, item.Key.Flock_id);

                            if (hfdDT.Count > 0)
                            {
                                ImportaIncubacao.Data.FLIP.CLFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                                hatchData.NUM_1 = 0;
                                hatchData.ACTUAL = 0;
                                hatchData.NUM_2 = 0;
                                hatchData.NUM_17 = 0;
                                hatchData.NUM_13 = 0;
                                hatchData.NUM_19 = 0;
                                hatchData.NUM_20 = 0;
                                hatchData.NUM_4 = 0;
                                hatchData.NUM_5 = 0;
                                hatchData.NUM_6 = 0;
                                hatchData.NUM_7 = 0;
                                hatchData.NUM_8 = 0;
                                hatchData.NUM_21 = 0;
                                hatchData.NUM_11 = 0;
                                hatchData.NUM_10 = 0;
                                hatchData.NUM_24 = 0;
                                hatchData.NUM_23 = 0;
                                hatchData.NUM_9 = 0;
                                hatchData.NUM_12 = 0;
                                hatchData.NUM_16 = 0;
                                hatchData.NUM_28 = 0;

                                hfdTA.Update(hatchData);
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
                else if (company == "HYCO")
                {
                    #region HYCO

                    if (!ExisteFechamentoEstoque(company, hatchLoc, setDate))
                    {
                        #region Delete Values

                        foreach (var item in listDelete)
                        {
                            ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable hfdDT =
                                new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATADataTable();
                            ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter hfdTA =
                                new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_FLOCK_DATATableAdapter();
                            hfdTA.FillByFlockData(hfdDT, company, region, location, Convert.ToDateTime(item.Key.Set_date),
                                ddlIncubatorios.SelectedValue, item.Key.Flock_id);

                            for (int i = 0; i < hfdDT.Count; i++)
                            {
                                ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_FLOCK_DATARow hatchData = hfdDT[i];
                                hatchData.NUM_1 = 0;
                                hatchData.ACTUAL = 0;
                                hatchData.NUM_2 = 0;
                                hatchData.NUM_17 = 0;
                                hatchData.NUM_13 = 0;
                                hatchData.NUM_19 = 0;
                                hatchData.NUM_20 = 0;
                                hatchData.NUM_4 = 0;
                                hatchData.NUM_5 = 0;
                                hatchData.NUM_6 = 0;
                                hatchData.NUM_7 = 0;
                                hatchData.NUM_8 = 0;
                                hatchData.NUM_21 = 0;
                                hatchData.NUM_11 = 0;
                                hatchData.NUM_10 = 0;
                                hatchData.NUM_24 = 0;
                                hatchData.NUM_23 = 0;
                                hatchData.NUM_9 = 0;
                                hatchData.NUM_12 = 0;
                                hatchData.NUM_16 = 0;
                                hatchData.NUM_28 = 0;

                                hfdTA.Update(hatchData);
                            }
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion

                #region Insert Values

                foreach (var item in listInsert)
                {
                    UpdateHatchingDataFLIP(company, region, location, hatchLoc, setDate,
                        item.Flock_id, item, "Insert");
                }

                #endregion
            }
        }

        #endregion

        #region Get Methods

        public static bool ExisteFechamentoEstoque(string company, string hatchLoc, DateTime dataMov)
        {
            bool closed = false;

            // Incrementar 21 dias para comparar data de nascimento
            dataMov = dataMov.AddDays(21);

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
                        DfDT.Where(w => w.DATA_FECH_LANC >= dataMov 
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

            return closed;
        }

        public string GetFieldByHatchLoc(string hatchLoc, string field)
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

        public void AtualizaTransferenciaFLIP(string location, string incubatorio, DateTime dataIncubacao, string loteCompleto,
            DateTime dataProducao, string incubadora, string nascedouro, int qtdOvos, string operacao)
        {
            string eggKey = "HYBRBR" + location + dataIncubacao.ToString("MM/dd/yy") + incubatorio
                + loteCompleto;

            string trackNo = "EXP" + dataProducao.ToString("yyMMdd");

            FLIPDataSet.HATCHERY_TRAN_DATADataTable htdDT = new FLIPDataSet.HATCHERY_TRAN_DATADataTable();
            HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();
            htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, dataProducao, incubadora, nascedouro);

            if (htdDT.Count == 0)
            {
                htdTA.Insert(eggKey, dataProducao, qtdOvos, incubadora, nascedouro, trackNo, null, null, null, null,
                    null, null, null, null, "", "", null);
            }
            else
            {
                FLIPDataSet.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                int qtdOvosTotal = 0;
                if (operacao.Equals("Inclusão"))
                    qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) + qtdOvos;
                else if (operacao.Equals("Exclusão"))
                    qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) - qtdOvos;
                else
                {
                    int qtdOvosInserido = Convert.ToInt32(hlbapp.HATCHERY_TRAN_DATA
                        .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                            && w.Flock_id == loteCompleto && w.Lay_date == dataProducao
                            && w.Setter == incubadora && w.Hatcher == nascedouro)
                        .Sum(s => s.Qtde_Ovos_Transferidos));

                    qtdOvosTotal = qtdOvosInserido;
                }

                transfFLIP.EGGS_TRAN = qtdOvosTotal;
                if (qtdOvosTotal == 0)
                    htdTA.Delete(eggKey, dataProducao, incubadora, nascedouro, trackNo);
                else
                    htdTA.Update(transfFLIP);
            }
        }

        public void AtualizaDadosNascimentoFLIP(string location, string incubatorio, DateTime dataIncubacao, string loteCompleto,
            HATCHERY_FLOCK_SETTER_DATA hatchDataSetter, string operacao)
        {
            string eggKey = "HYBRBR" + location + dataIncubacao.ToString("MM/dd/yy") + incubatorio
                + loteCompleto;

            int qtdPintosNascidos = Convert.ToInt32(hatchDataSetter.Pintos_Vendaveis + hatchDataSetter.Refugo
                + hatchDataSetter.Pinto_Terceira);

            FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
            HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
            hfdTA.FillByFlockData(hfdDT, "HYBR", "BR", location, dataIncubacao, incubatorio, loteCompleto);

            #region Nascimento Mais Cedo e Mais Tarde

            HLBAPPEntities hlbapp = new HLBAPPEntities();

            var listaNascimentoLote = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                    && w.Flock_id == loteCompleto)
                .ToList();

            List<DateTime> listaData = new List<DateTime>();

            foreach (var item in listaNascimentoLote)
            {
                if (item.DataRetiradaReal != null)
                {
                    string dataRetirada = Convert.ToDateTime(item.DataRetiradaReal).ToString("dd/MM/yyyy");
                    DateTime data = new DateTime();
                    if (DateTime.TryParse(dataRetirada + " " + item.Horario_01_Retirada,
                        out data))
                    {
                        listaData.Add(data);
                    }
                }
            }

            DateTime? dataNascimentoMaisCedo = null;
            DateTime? dataNascimentoMaisTarde = null;
            if (hatchDataSetter.DataRetiradaReal != null)
            {
                dataNascimentoMaisCedo = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                dataNascimentoMaisTarde = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
            }

            string horaNascimentoMaisCedo = "";
            string horaNascimentoMaisTarde = "";

            if (hatchDataSetter.Horario_01_Retirada != null)
            {
                horaNascimentoMaisCedo = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                horaNascimentoMaisTarde = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
            }

            if (listaData.Count > 0)
            {
                dataNascimentoMaisCedo = listaData.Min(m => m);
                dataNascimentoMaisTarde = listaData.Max(m => m);
                horaNascimentoMaisCedo = listaData.Min(m => m).ToString("HH:mm");
                horaNascimentoMaisTarde = listaData.Max(m => m).ToString("HH:mm");
            }

            #endregion

            if (hfdDT.Count > 0)
            {
                FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                if (operacao.Equals("Inclusão"))
                {
                    hatchData.NUM_1 = hatchData.NUM_1 + Convert.ToInt32(hatchDataSetter.Eliminado);
                    hatchData.ACTUAL = hatchData.ACTUAL + qtdPintosNascidos;
                    hatchData.NUM_2 = hatchData.NUM_2 + Convert.ToInt32(hatchDataSetter.Refugo
                        + hatchDataSetter.Pinto_Terceira);
                    hatchData.NUM_17 = hatchData.NUM_17 + qtdPintosNascidos
                        + Convert.ToInt32(hatchDataSetter.Macho);
                    if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                    hatchData.NUM_13 = hatchData.NUM_13 + Convert.ToInt32(hatchDataSetter.Amostra);
                    if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                    hatchData.NUM_19 = hatchData.NUM_19 + Convert.ToInt32(hatchDataSetter.Infertil);
                    if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                    hatchData.NUM_20 = hatchData.NUM_20 + Convert.ToInt32(hatchDataSetter.Inicial0a3);
                    if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                    hatchData.NUM_4 = hatchData.NUM_4 + Convert.ToInt32(hatchDataSetter.Inicial4a7);
                    if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                    hatchData.NUM_5 = hatchData.NUM_5 + Convert.ToInt32(hatchDataSetter.Media8a14);
                    if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                    hatchData.NUM_6 = hatchData.NUM_6 + Convert.ToInt32(hatchDataSetter.Tardia15a18);
                    if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                    hatchData.NUM_7 = hatchData.NUM_7 + Convert.ToInt32(hatchDataSetter.Tardia19a21);
                    if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                    hatchData.NUM_8 = hatchData.NUM_8 + Convert.ToInt32(hatchDataSetter.BicadoVivo);
                    if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                    hatchData.NUM_21 = hatchData.NUM_21 + Convert.ToInt32(hatchDataSetter.BicadoMorto);
                    if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                    hatchData.NUM_11 = hatchData.NUM_11 + Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                    if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                    hatchData.NUM_10 = hatchData.NUM_10 + Convert.ToInt32(hatchDataSetter.Fungo);
                    if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                    hatchData.NUM_24 = hatchData.NUM_24 + Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                    if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                    hatchData.NUM_23 = hatchData.NUM_23 + Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                    if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                    hatchData.NUM_9 = hatchData.NUM_9 + Convert.ToInt32(hatchDataSetter.Hemorragico);
                    if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                    hatchData.NUM_12 = hatchData.NUM_12 + Convert.ToInt32(hatchDataSetter.Anormalidade);
                    if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                    hatchData.NUM_16 = hatchData.NUM_16 + Convert.ToInt32(hatchDataSetter.MalPosicionado);
                    //hatchData.TEXT_2 = hatchDataSetter.Horario_01_Retirada.Replace(":","H");
                    //hatchData.TEXT_3 = hatchDataSetter.Horario_02_Retirada.Replace(":", "H");
                    //hatchData.DATE_1 = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                    hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                    if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                    hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                    if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                    hatchData.NUM_28 = hatchData.NUM_28 + Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);
                }
                else if (operacao.Equals("Exclusão"))
                {
                    hatchData.NUM_1 = hatchData.NUM_1 - Convert.ToInt32(hatchDataSetter.Eliminado);
                    hatchData.ACTUAL = hatchData.ACTUAL - qtdPintosNascidos;
                    hatchData.NUM_2 = hatchData.NUM_2 - Convert.ToInt32(hatchDataSetter.Refugo
                        + hatchDataSetter.Pinto_Terceira);
                    hatchData.NUM_17 = hatchData.NUM_17 - qtdPintosNascidos
                        - Convert.ToInt32(hatchDataSetter.Macho);
                    if (hatchData.IsNUM_13Null()) hatchData.NUM_13 = 0;
                    hatchData.NUM_13 = hatchData.NUM_13 - Convert.ToInt32(hatchDataSetter.Amostra);
                    if (hatchData.IsNUM_19Null()) hatchData.NUM_19 = 0;
                    hatchData.NUM_19 = hatchData.NUM_19 - Convert.ToInt32(hatchDataSetter.Infertil);
                    if (hatchData.IsNUM_20Null()) hatchData.NUM_20 = 0;
                    hatchData.NUM_20 = hatchData.NUM_20 - Convert.ToInt32(hatchDataSetter.Inicial0a3);
                    if (hatchData.IsNUM_4Null()) hatchData.NUM_4 = 0;
                    hatchData.NUM_4 = hatchData.NUM_4 - Convert.ToInt32(hatchDataSetter.Inicial4a7);
                    if (hatchData.IsNUM_5Null()) hatchData.NUM_5 = 0;
                    hatchData.NUM_5 = hatchData.NUM_5 - Convert.ToInt32(hatchDataSetter.Media8a14);
                    if (hatchData.IsNUM_6Null()) hatchData.NUM_6 = 0;
                    hatchData.NUM_6 = hatchData.NUM_6 - Convert.ToInt32(hatchDataSetter.Tardia15a18);
                    if (hatchData.IsNUM_7Null()) hatchData.NUM_7 = 0;
                    hatchData.NUM_7 = hatchData.NUM_7 - Convert.ToInt32(hatchDataSetter.Tardia19a21);
                    if (hatchData.IsNUM_8Null()) hatchData.NUM_8 = 0;
                    hatchData.NUM_8 = hatchData.NUM_8 - Convert.ToInt32(hatchDataSetter.BicadoVivo);
                    if (hatchData.IsNUM_21Null()) hatchData.NUM_21 = 0;
                    hatchData.NUM_21 = hatchData.NUM_21 - Convert.ToInt32(hatchDataSetter.BicadoMorto);
                    if (hatchData.IsNUM_11Null()) hatchData.NUM_11 = 0;
                    hatchData.NUM_11 = hatchData.NUM_11 - Convert.ToInt32(hatchDataSetter.ContaminacaoBacteriana);
                    if (hatchData.IsNUM_10Null()) hatchData.NUM_10 = 0;
                    hatchData.NUM_10 = hatchData.NUM_10 - Convert.ToInt32(hatchDataSetter.Fungo);
                    if (hatchData.IsNUM_24Null()) hatchData.NUM_24 = 0;
                    hatchData.NUM_24 = hatchData.NUM_24 - Convert.ToInt32(hatchDataSetter.MaFormacaoCerebro);
                    if (hatchData.IsNUM_23Null()) hatchData.NUM_23 = 0;
                    hatchData.NUM_23 = hatchData.NUM_23 - Convert.ToInt32(hatchDataSetter.MaFormacaoVisceras);
                    if (hatchData.IsNUM_9Null()) hatchData.NUM_9 = 0;
                    hatchData.NUM_9 = hatchData.NUM_9 - Convert.ToInt32(hatchDataSetter.Hemorragico);
                    if (hatchData.IsNUM_12Null()) hatchData.NUM_12 = 0;
                    hatchData.NUM_12 = hatchData.NUM_12 - Convert.ToInt32(hatchDataSetter.Anormalidade);
                    if (hatchData.IsNUM_16Null()) hatchData.NUM_16 = 0;
                    hatchData.NUM_16 = hatchData.NUM_16 - Convert.ToInt32(hatchDataSetter.MalPosicionado);
                    //hatchData.TEXT_2 = hatchDataSetter.Horario_01_Retirada.Replace(":", "H");
                    //hatchData.TEXT_3 = hatchDataSetter.Horario_02_Retirada.Replace(":", "H");
                    //hatchData.DATE_1 = Convert.ToDateTime(hatchDataSetter.DataRetiradaReal);
                    if (dataNascimentoMaisCedo != null) hatchData.DATE_1 = Convert.ToDateTime(dataNascimentoMaisCedo);
                    hatchData.TEXT_2 = horaNascimentoMaisCedo.Replace(":", "H");
                    if (dataNascimentoMaisTarde != null) hatchData.DATE_2 = Convert.ToDateTime(dataNascimentoMaisTarde);
                    hatchData.TEXT_3 = horaNascimentoMaisTarde.Replace(":", "H");
                    if (hatchData.IsNUM_28Null()) hatchData.NUM_28 = 0;
                    hatchData.NUM_28 = hatchData.NUM_28 - Convert.ToInt32(hatchDataSetter.EliminadoCancelamento);
                }

                hfdTA.Update(hatchData);
            }
        }

        public void AtualizaFLIP()
        {
            if (ddlIncubatorios.SelectedValue == "NM")
            {
                if (!ExisteFechamentoEstoque("", ddlIncubatorios.SelectedValue, Calendar1.SelectedDate))
                {
                    #region Deleta Valores

                    HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                    hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, ddlIncubatorios.SelectedValue);
                    string location = flip.HATCHERY_CODES[0].LOCATION;

                    var listaDelecao = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                        .Where(w => w.Set_date == Calendar1.SelectedDate
                            && w.Hatch_Loc == ddlIncubatorios.SelectedValue)
                        .GroupBy(g => new { g.Set_date, g.Flock_id })
                        .ToList();

                    foreach (var item in listaDelecao)
                    {
                        FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                        HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                        hfdTA.FillByFlockData(hfdDT, "HYBR", "BR", location, Convert.ToDateTime(item.Key.Set_date),
                            ddlIncubatorios.SelectedValue, item.Key.Flock_id);

                        if (hfdDT.Count > 0)
                        {
                            FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                            hatchData.NUM_1 = 0;
                            hatchData.ACTUAL = 0;
                            hatchData.NUM_2 = 0;
                            hatchData.NUM_17 = 0;
                            hatchData.NUM_13 = 0;
                            hatchData.NUM_19 = 0;
                            hatchData.NUM_20 = 0;
                            hatchData.NUM_4 = 0;
                            hatchData.NUM_5 = 0;
                            hatchData.NUM_6 = 0;
                            hatchData.NUM_7 = 0;
                            hatchData.NUM_8 = 0;
                            hatchData.NUM_21 = 0;
                            hatchData.NUM_11 = 0;
                            hatchData.NUM_10 = 0;
                            hatchData.NUM_24 = 0;
                            hatchData.NUM_23 = 0;
                            hatchData.NUM_9 = 0;
                            hatchData.NUM_12 = 0;
                            hatchData.NUM_16 = 0;
                            hatchData.NUM_28 = 0;

                            hfdTA.Update(hatchData);
                        }
                    }

                    #endregion

                    #region Insere Valores

                    var lista = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                        .Where(w => w.Set_date == Calendar1.SelectedDate
                            && w.Hatch_Loc == ddlIncubatorios.SelectedValue)
                        .ToList();

                    foreach (var item in lista)
                    {
                        AtualizaDadosNascimentoFLIP(location, ddlIncubatorios.SelectedValue, Calendar1.SelectedDate,
                            item.Flock_id, item, "Inclusão");
                    }

                    #endregion
                }
            }
        }

        public void AtualizaFLIPPlanaltoAll()
        {
            #region Deleta Valores

            string hatchLoc = "NM";
            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
            hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
            string location = flip.HATCHERY_CODES[0].LOCATION;

            var listaDelecao = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == hatchLoc)
                .GroupBy(g => new { g.Set_date, g.Flock_id })
                .OrderBy(o => o.Key.Set_date).ThenBy(t => t.Key.Flock_id)
                .ToList();

            foreach (var item in listaDelecao)
            {
                FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                hfdTA.FillByFlockData(hfdDT, "HYBR", "BR", location, Convert.ToDateTime(item.Key.Set_date),
                    hatchLoc, item.Key.Flock_id);

                if (hfdDT.Count > 0)
                {
                    FLIPDataSet.HATCHERY_FLOCK_DATARow hatchData = hfdDT[0];
                    hatchData.NUM_1 = 0;
                    hatchData.ACTUAL = 0;
                    hatchData.NUM_2 = 0;
                    hatchData.NUM_17 = 0;
                    hatchData.NUM_13 = 0;
                    hatchData.NUM_19 = 0;
                    hatchData.NUM_20 = 0;
                    hatchData.NUM_4 = 0;
                    hatchData.NUM_5 = 0;
                    hatchData.NUM_6 = 0;
                    hatchData.NUM_7 = 0;
                    hatchData.NUM_8 = 0;
                    hatchData.NUM_21 = 0;
                    hatchData.NUM_11 = 0;
                    hatchData.NUM_10 = 0;
                    hatchData.NUM_24 = 0;
                    hatchData.NUM_23 = 0;
                    hatchData.NUM_9 = 0;
                    hatchData.NUM_12 = 0;
                    hatchData.NUM_16 = 0;

                    hfdTA.Update(hatchData);
                }
            }

            #endregion

            #region Insere Valores

            var lista = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                .Where(w => w.Hatch_Loc == hatchLoc)
                .OrderBy(o => o.Set_date).ThenBy(t => t.Flock_id)
                .ToList();

            //DateTime setDate = Convert.ToDateTime("20/02/2017");
            //string location = "PP";
            //string hatchLoc = "NM";

            //var lista = hlbapp.HATCHERY_FLOCK_SETTER_DATA
            //    .Where(w => w.Hatch_Loc == hatchLoc 
            //        && w.Set_date == setDate && w.Flock_id == "JRP02-JRP026311L")
            //    .OrderBy(o => o.Set_date).ThenBy(t => t.Flock_id)
            //    .ToList();

            foreach (var item in lista)
            {
                AtualizaDadosNascimentoFLIP(location, hatchLoc, Convert.ToDateTime(item.Set_date),
                    item.Flock_id, item, "Inclusão");
            }

            #endregion
        }

        protected void btnAtualizaFLIPAllPlanalto_Click(object sender, EventArgs e)
        {
            AtualizaFLIPPlanaltoAll();
        }

        #endregion

        #endregion

        #region HLBAPP

        public void InsereLOGHatcheryFlockSetterData(HATCHERY_FLOCK_SETTER_DATA hfsd, DateTime dataHora,
            string operacao, string usuario)
        {
            HLBAPPEntities hlbappSession = new HLBAPPEntities();

            LOG_HATCHERY_FLOCK_SETTER_DATA log = new LOG_HATCHERY_FLOCK_SETTER_DATA();
            log.Data_Hora = dataHora;
            log.Operacao = operacao;
            log.Usuario = usuario;
            log.Hatch_Loc = hfsd.Hatch_Loc;
            log.Set_date = hfsd.Set_date;
            log.Flock_id = hfsd.Flock_id;
            log.NumLote = hfsd.NumLote;
            log.Setter = hfsd.Setter;
            log.Hatcher = hfsd.Hatcher;
            log.ClassOvo = hfsd.ClassOvo;
            log.Eliminado = hfsd.Eliminado;
            log.Morto = hfsd.Morto;
            log.Macho = hfsd.Macho;
            log.Pintos_Vendaveis = hfsd.Pintos_Vendaveis;
            log.Refugo = hfsd.Refugo;
            log.Pinto_Terceira = hfsd.Pinto_Terceira;
            log.Qtde_Incubada = hfsd.Qtde_Incubada;
            log.DataRetiradaReal = hfsd.DataRetiradaReal;
            log.Horario_01_Retirada = hfsd.Horario_01_Retirada;
            log.Qtde_01_Retirada = hfsd.Qtde_01_Retirada;
            log.Horario_02_Retirada = hfsd.Horario_02_Retirada;
            log.Qtde_02_Retirada = hfsd.Qtde_02_Retirada;
            log.Variety = hfsd.Variety;
            log.De0a4 = hfsd.De0a4;
            log.De5a12 = hfsd.De5a12;
            log.De13a17 = hfsd.De13a17;
            log.De18a21 = hfsd.De18a21;
            log.BicadoVivo = hfsd.BicadoVivo;
            log.BicadoMorto = hfsd.BicadoMorto;
            log.ContaminacaoBacteriana = hfsd.ContaminacaoBacteriana;
            log.Fungo = hfsd.Fungo;
            log.MalPosicionado = hfsd.MalPosicionado;
            log.MalFormado = hfsd.MalFormado;
            log.Infertil = hfsd.Infertil;
            log.Inicial0a3 = hfsd.Inicial0a3;
            log.Inicial4a7 = hfsd.Inicial4a7;
            log.Media8a14 = hfsd.Media8a14;
            log.Tardia15a18 = hfsd.Tardia15a18;
            log.Tardia19a21 = hfsd.Tardia19a21;
            log.MaFormacaoCerebro = hfsd.MaFormacaoCerebro;
            log.MaFormacaoVisceras = hfsd.MaFormacaoVisceras;
            log.Hemorragico = hfsd.Hemorragico;
            log.Anormalidade = hfsd.Anormalidade;
            log.Amostra = hfsd.Amostra;
            log.Infertilidade10Dias = hfsd.Infertilidade10Dias;
            log.EliminadoCancelamento = hfsd.EliminadoCancelamento;

            hlbappSession.LOG_HATCHERY_FLOCK_SETTER_DATA.AddObject(log);
            hlbappSession.SaveChanges();
        }

        #endregion

        #endregion

        #region Reports

        public string GeraRelatorioRetirada(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Incubacao_Transferencia_Eclosao.xlsx", destino);

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

            string commandTextCHICCabecalho =
                "select " +
                      "* ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Retirada_WEB ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataIncubacaoStrSQLServer = Calendar1.SelectedDate.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "[Data Incub.] = '" + dataIncubacaoStrSQLServer + "' and " +
                    "[Inc.] = '" + ddlIncubatorios.SelectedValue + "' ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "4,8";

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Controle Incub. - Transf. - Ecl"];

            //worksheet.Cells[2, 8] = dataInicial;
            //worksheet.Cells[3, 8] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("RetiradaWEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
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

        public string GeraRelatorioNascimento(string pesquisa, bool deletaArquivoAntigo, string pasta,
            string destino)
        {
            string[] files = Directory.GetFiles(pasta, pesquisa);

            if (deletaArquivoAntigo)
            {
                foreach (var item in files)
                {
                    System.IO.File.Delete(item);
                }
            }

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Eclosao_Maquina_Lote.xlsx", destino);

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

            string commandTextCHICCabecalho =
                "select " +
                      "* ";

            string commandTextCHICTabelas =
                "from " +
                    "VU_Nascimento_WEB ";

            string commandTextCHICCondicaoJoins =
                "where ";

            string commandTextCHICCondicaoFiltros = "";

            string dataIncubacaoStrSQLServer = Calendar1.SelectedDate.ToString("yyyy-MM-dd");

            string commandTextCHICCondicaoParametros =
                    "[Data Incub.] = '" + dataIncubacaoStrSQLServer + "' and " +
                    "[Inc.] = '" + ddlIncubatorios.SelectedValue + "' ";

            string commandTextCHICAgrupamento = "";

            string commandTextCHICOrdenacao =
                "order by " +
                    "4,8";

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Controle Eclo. - Maq. - Lote"];

            //worksheet.Cells[2, 8] = dataInicial;
            //worksheet.Cells[3, 8] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("NascimentoWEB"))
                    item.OLEDBConnection.CommandText =
                        commandTextCHICCabecalho + commandTextCHICTabelas + commandTextCHICCondicaoJoins +
                        commandTextCHICCondicaoFiltros + commandTextCHICCondicaoParametros + commandTextCHICAgrupamento +
                        commandTextCHICOrdenacao;
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
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Incubacao_Transferencia_Eclosao_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Relatorio_Incubacao_Transferencia_Eclosao_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            Session["destinoRelRetiradaWEB"] = GeraRelatorioRetirada(pesquisa, true, pasta, destino);

            lbtnExportar.Visible = true;

            Session["nomeDestino"] = "Relatorio_Incubacao_Transferencia_Eclosao_";

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lbtnExportar);
        }

        protected void lbtnExportar_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelRetiradaWEB"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Session["nomeDestino"].ToString()
                + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }

        protected void btnGerar02_Click(object sender, EventArgs e)
        {
            string pasta = "C:\\inetpub\\wwwroot\\Relatorios";
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Eclosao_Maquina_Lote_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Relatorio_Eclosao_Maquina_Lote_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            Session["destinoRelRetiradaWEB"] = GeraRelatorioNascimento(pesquisa, true, pasta, destino);

            lbtnExportar.Visible = true;

            Session["nomeDestino"] = "Relatorio_Eclosao_Maquina_Lote_";

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.lbtnExportar);
        }

        #endregion

        #region Other Methods

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
            return AccountController.Translate(text.Replace(":", ""), language);
        }

        public void ChangeLanguage()
        {
            string lg = Session["language"].ToString();

            if (lg != "pt-BR")
            {
                #region Change Header Setting Eggs Table

                GridView3.Columns[1].HeaderText = Translate(GridView3.Columns[1].HeaderText);
                GridView3.Columns[2].HeaderText = Translate(GridView3.Columns[2].HeaderText);
                GridView3.Columns[3].HeaderText = Translate(GridView3.Columns[3].HeaderText);
                GridView3.Columns[4].HeaderText = Translate(GridView3.Columns[4].HeaderText);
                GridView3.Columns[5].HeaderText = Translate(GridView3.Columns[5].HeaderText);
                GridView3.Columns[6].HeaderText = Translate(GridView3.Columns[6].HeaderText);
                GridView3.Columns[7].HeaderText = Translate(GridView3.Columns[7].HeaderText);
                GridView3.Columns[8].HeaderText = Translate(GridView3.Columns[8].HeaderText);
                GridView3.Columns[9].HeaderText = Translate(GridView3.Columns[9].HeaderText);

                #endregion

                #region Change Header Setter Conference Table 01

                gvMaquinas.Columns[0].HeaderText = Translate(gvMaquinas.Columns[0].HeaderText);
                gvMaquinas.Columns[1].HeaderText = Translate(gvMaquinas.Columns[1].HeaderText);
                gvMaquinas.Columns[2].HeaderText = Translate(gvMaquinas.Columns[2].HeaderText);
                gvMaquinas.Columns[3].HeaderText = Translate(gvMaquinas.Columns[3].HeaderText);
                gvMaquinas.Columns[4].HeaderText = Translate(gvMaquinas.Columns[4].HeaderText);
                gvMaquinas.Columns[5].HeaderText = Translate(gvMaquinas.Columns[5].HeaderText);
                gvMaquinas.Columns[6].HeaderText = Translate(gvMaquinas.Columns[6].HeaderText);
                gvMaquinas.Columns[7].HeaderText = Translate(gvMaquinas.Columns[7].HeaderText);
                gvMaquinas.Columns[8].HeaderText = Translate(gvMaquinas.Columns[8].HeaderText);

                #endregion

                #region Change Header Setter Conference Table 02

                gvMaquinas0.Columns[0].HeaderText = Translate(gvMaquinas0.Columns[0].HeaderText);
                gvMaquinas0.Columns[1].HeaderText = Translate(gvMaquinas0.Columns[1].HeaderText);
                gvMaquinas0.Columns[2].HeaderText = Translate(gvMaquinas0.Columns[2].HeaderText);
                gvMaquinas0.Columns[3].HeaderText = Translate(gvMaquinas0.Columns[3].HeaderText);
                gvMaquinas0.Columns[4].HeaderText = Translate(gvMaquinas0.Columns[4].HeaderText);
                gvMaquinas0.Columns[5].HeaderText = Translate(gvMaquinas0.Columns[5].HeaderText);
                gvMaquinas0.Columns[6].HeaderText = Translate(gvMaquinas0.Columns[6].HeaderText);
                gvMaquinas0.Columns[7].HeaderText = Translate(gvMaquinas0.Columns[7].HeaderText);
                gvMaquinas0.Columns[8].HeaderText = Translate(gvMaquinas0.Columns[8].HeaderText);
                gvMaquinas0.Columns[9].HeaderText = Translate(gvMaquinas0.Columns[9].HeaderText);
                gvMaquinas0.Columns[10].HeaderText = Translate(gvMaquinas0.Columns[10].HeaderText);
                gvMaquinas0.Columns[11].HeaderText = Translate(gvMaquinas0.Columns[11].HeaderText);
                gvMaquinas0.Columns[12].HeaderText = Translate(gvMaquinas0.Columns[12].HeaderText);
                gvMaquinas0.Columns[13].HeaderText = Translate(gvMaquinas0.Columns[13].HeaderText);
                gvMaquinas0.Columns[14].HeaderText = Translate(gvMaquinas0.Columns[14].HeaderText);
                gvMaquinas0.Columns[15].HeaderText = Translate(gvMaquinas0.Columns[15].HeaderText);
                gvMaquinas0.Columns[16].HeaderText = Translate(gvMaquinas0.Columns[16].HeaderText);

                #endregion

                #region Change Header Flock Conference Table 01

                gvLotes.Columns[0].HeaderText = Translate(gvLotes.Columns[0].HeaderText);
                gvLotes.Columns[1].HeaderText = Translate(gvLotes.Columns[1].HeaderText);
                gvLotes.Columns[2].HeaderText = Translate(gvLotes.Columns[2].HeaderText);
                gvLotes.Columns[3].HeaderText = Translate(gvLotes.Columns[3].HeaderText);
                gvLotes.Columns[4].HeaderText = Translate(gvLotes.Columns[4].HeaderText);
                gvLotes.Columns[5].HeaderText = Translate(gvLotes.Columns[5].HeaderText);
                gvLotes.Columns[6].HeaderText = Translate(gvLotes.Columns[6].HeaderText);
                gvLotes.Columns[7].HeaderText = Translate(gvLotes.Columns[7].HeaderText);
                gvLotes.Columns[8].HeaderText = Translate(gvLotes.Columns[8].HeaderText);

                #endregion

                #region Change Header Flock Conference Table 02

                gvLotes0.Columns[0].HeaderText = Translate(gvLotes0.Columns[0].HeaderText);
                gvLotes0.Columns[1].HeaderText = Translate(gvLotes0.Columns[1].HeaderText);
                gvLotes0.Columns[2].HeaderText = Translate(gvLotes0.Columns[2].HeaderText);
                gvLotes0.Columns[3].HeaderText = Translate(gvLotes0.Columns[3].HeaderText);
                gvLotes0.Columns[4].HeaderText = Translate(gvLotes0.Columns[4].HeaderText);
                gvLotes0.Columns[5].HeaderText = Translate(gvLotes0.Columns[5].HeaderText);
                gvLotes0.Columns[6].HeaderText = Translate(gvLotes0.Columns[6].HeaderText);
                gvLotes0.Columns[7].HeaderText = Translate(gvLotes0.Columns[7].HeaderText);
                gvLotes0.Columns[8].HeaderText = Translate(gvLotes0.Columns[8].HeaderText);
                gvLotes0.Columns[9].HeaderText = Translate(gvLotes0.Columns[9].HeaderText);
                gvLotes0.Columns[10].HeaderText = Translate(gvLotes0.Columns[10].HeaderText);
                gvLotes0.Columns[11].HeaderText = Translate(gvLotes0.Columns[11].HeaderText);
                gvLotes0.Columns[12].HeaderText = Translate(gvLotes0.Columns[12].HeaderText);
                gvLotes0.Columns[13].HeaderText = Translate(gvLotes0.Columns[13].HeaderText);
                gvLotes0.Columns[14].HeaderText = Translate(gvLotes0.Columns[14].HeaderText);
                gvLotes0.Columns[15].HeaderText = Translate(gvLotes0.Columns[15].HeaderText);
                gvLotes0.Columns[16].HeaderText = Translate(gvLotes0.Columns[16].HeaderText);

                #endregion

                #region Change Header Variety Conference Table 01

                gvLinhagens.Columns[0].HeaderText = Translate(gvLinhagens.Columns[0].HeaderText);
                gvLinhagens.Columns[1].HeaderText = Translate(gvLinhagens.Columns[1].HeaderText);
                gvLinhagens.Columns[2].HeaderText = Translate(gvLinhagens.Columns[2].HeaderText);
                gvLinhagens.Columns[3].HeaderText = Translate(gvLinhagens.Columns[3].HeaderText);
                gvLinhagens.Columns[4].HeaderText = Translate(gvLinhagens.Columns[4].HeaderText);
                gvLinhagens.Columns[5].HeaderText = Translate(gvLinhagens.Columns[5].HeaderText);
                gvLinhagens.Columns[6].HeaderText = Translate(gvLinhagens.Columns[6].HeaderText);
                gvLinhagens.Columns[7].HeaderText = Translate(gvLinhagens.Columns[7].HeaderText);
                gvLinhagens.Columns[8].HeaderText = Translate(gvLinhagens.Columns[8].HeaderText);

                #endregion

                #region Change Header Variety Conference Table 02

                gvLinhagens0.Columns[0].HeaderText = Translate(gvLinhagens0.Columns[0].HeaderText);
                gvLinhagens0.Columns[1].HeaderText = Translate(gvLinhagens0.Columns[1].HeaderText);
                gvLinhagens0.Columns[2].HeaderText = Translate(gvLinhagens0.Columns[2].HeaderText);
                gvLinhagens0.Columns[3].HeaderText = Translate(gvLinhagens0.Columns[3].HeaderText);
                gvLinhagens0.Columns[4].HeaderText = Translate(gvLinhagens0.Columns[4].HeaderText);
                gvLinhagens0.Columns[5].HeaderText = Translate(gvLinhagens0.Columns[5].HeaderText);
                gvLinhagens0.Columns[6].HeaderText = Translate(gvLinhagens0.Columns[6].HeaderText);
                gvLinhagens0.Columns[7].HeaderText = Translate(gvLinhagens0.Columns[7].HeaderText);
                gvLinhagens0.Columns[8].HeaderText = Translate(gvLinhagens0.Columns[8].HeaderText);
                gvLinhagens0.Columns[9].HeaderText = Translate(gvLinhagens0.Columns[9].HeaderText);
                gvLinhagens0.Columns[10].HeaderText = Translate(gvLinhagens0.Columns[10].HeaderText);
                gvLinhagens0.Columns[11].HeaderText = Translate(gvLinhagens0.Columns[11].HeaderText);
                gvLinhagens0.Columns[12].HeaderText = Translate(gvLinhagens0.Columns[12].HeaderText);
                gvLinhagens0.Columns[13].HeaderText = Translate(gvLinhagens0.Columns[13].HeaderText);
                gvLinhagens0.Columns[14].HeaderText = Translate(gvLinhagens0.Columns[14].HeaderText);
                gvLinhagens0.Columns[15].HeaderText = Translate(gvLinhagens0.Columns[15].HeaderText);
                gvLinhagens0.Columns[16].HeaderText = Translate(gvLinhagens0.Columns[16].HeaderText);

                #endregion

                #region Another Components

                hlBackHome.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("HL_Back_To_Home", Session["language"].ToString());
                Label5.Text = Translate(Label5.Text);
                Label9.Text = Translate(Label9.Text);
                Label4.Text = Translate(Label4.Text);
                lbtnExportar.Text = Translate(lbtnExportar.Text);
                btnGerar.Text = Translate(btnGerar.Text);
                btnGerar02.Text = Translate(btnGerar02.Text);
                Label3.Text = Translate(Label3.Text);
                Label8.Text = Translate(Label8.Text);

                foreach (ListItem item in DropDownList1.Items)
                {
                    item.Text = Translate(item.Text);
                }

                foreach (ListItem item in ddlClassOvos.Items)
                {
                    item.Text = Translate(item.Text);
                }

                btn_Pesquisar.Text = Translate(btn_Pesquisar.Text);
                Label10.Text = Translate(Label10.Text);
                Label11.Text = Translate(Label11.Text);
                Label12.Text = Translate(Label12.Text);
                Label13.Text = Translate(Label13.Text);

                #endregion
            }
        }

        #endregion
    }
}