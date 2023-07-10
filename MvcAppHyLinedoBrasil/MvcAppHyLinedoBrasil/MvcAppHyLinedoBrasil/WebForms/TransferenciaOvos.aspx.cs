using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
using AjaxControlToolkit;
using System.IO;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Globalization;
using MvcAppHyLinedoBrasil.Controllers;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class TransferenciaOvos : System.Web.UI.Page
    {
        #region Objetcs

        FLIPDataSet flip = new FLIPDataSet();
        HLBAPPEntities hlbapp = new HLBAPPEntities();

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            VerificaSessao();

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

            if (IsPostBack == false)
            {
                #region Update FLIP Colombia - Import

                //DateTime minDate = Convert.ToDateTime("2018-07-26 00:00:00.000");
                //DateTime maxDate = Convert.ToDateTime("2019-11-14 00:00:00.000");
                //while (minDate <= maxDate)
                //{
                //    RefreshFLIP("MN", minDate);
                //    RefreshFLIP("PM", minDate);
                //    RefreshFLIP("MQ", minDate);
                //    RefreshFLIP("MA", minDate);
                //    minDate = minDate.AddDays(1);
                //}

                #endregion

                #region Update FLIP Brasil - Import

                //DateTime minDate = Convert.ToDateTime("2020-03-23 00:00:00.000");
                //DateTime maxDate = Convert.ToDateTime("2020-05-06 00:00:00.000");
                //while (minDate <= maxDate)
                //{
                //    RefreshFLIP("CH", minDate);
                //    minDate = minDate.AddDays(1);
                //}

                #endregion

                ChangeLanguage();

                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                Calendar1.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Session["hatchLocal"] = ddlIncubatorios.SelectedValue;
                Session["setDate"] = Calendar1.SelectedDate;

                Session["linhagem"] = "";
                Session["age"] = "0";
                Session["qtde"] = "0";
                Session["dataNascimentoLote"] = "";
                Session["tipoCadastro"] = "";

                DateTime data = Calendar1.SelectedDate;

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

                #region Load Eggs Type

                if (GetFieldValueHatcheryCodes(ddlIncubatorios.SelectedValue, "CLAS_EGG") != "NO")
                {
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
                    ddlClassOvos.Visible = false;
                }

                #endregion

                string hatchLoc = ddlIncubatorios.SelectedValue;
                RefreshFLIP(hatchLoc, data);
            }
            AtualizaTotais();
        }

        public void AtualizaTotais()
        {
            decimal qtdeOvosIncubados = 0;
            string maquinasUtilizadas = "";
            DateTime dataIncubacao = Calendar1.SelectedDate;
            string incubatorio = ddlIncubatorios.SelectedValue;

            lblQtdeOvosIncubados.Text = "";
            //lblQtdeOvosIncubadosCx.Text = "";

            qtdeOvosIncubados = Convert.ToDecimal(hlbapp.HATCHERY_TRAN_DATA
                .Where(h => h.Set_date == dataIncubacao && h.Hatch_Loc == incubatorio)
                .Sum(h => h.Qtde_Ovos_Transferidos));

            var lista = hlbapp.HATCHERY_TRAN_DATA
                    .Where(h => h.Set_date == dataIncubacao && h.Hatch_Loc == incubatorio)
                    .GroupBy(h => new
                    {
                        h.Hatcher
                    })
                    .Select(h => new //HATCHERY_EGG_DATA
                    {
                        type = h.Key
                    })
                    .ToList();

            foreach (var item in lista)
            {
                maquinasUtilizadas = maquinasUtilizadas + " / " + item.type.Hatcher;
            }

            if (qtdeOvosIncubados > 0)
            {
                decimal bandejas = (qtdeOvosIncubados / 150);
                //lblQtdeOvosIncubados.Text = string.Format("{0:N0}", qtdeOvosIncubados) + " ovos";
                //lblQtdeOvosIncubadosCx.Text = string.Format("{0:N0}", Decimal.Round(bandejas, 0)) + " bandejas";
                lblQtdeOvosIncubados.Text = string.Format("{0:N0}", qtdeOvosIncubados) + " "
                    + Translate("ovos");
            }

            lblMaquinas.Text = maquinasUtilizadas;
        }

        #endregion

        #region Page Components

        protected void ddlIncubatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Load Eggs Type

            if (GetFieldValueHatcheryCodes(ddlIncubatorios.SelectedValue, "CLAS_EGG") != "NO")
            {
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
                ddlClassOvos.Visible = false;
            }

            #endregion

            lbtnExportar.Visible = false;
            AtualizaTotais();
            DateTime data = Calendar1.SelectedDate;
            string hatchLoc = ddlIncubatorios.SelectedValue;
            RefreshFLIP(hatchLoc, data);
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            lbtnExportar.Visible = false;
            AtualizaTotais();
            DateTime data = Calendar1.SelectedDate;
            string hatchLoc = ddlIncubatorios.SelectedValue;
            RefreshFLIP(hatchLoc, data);
        }

        #endregion

        #region Transfer Flock and Lay Date Form

        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                #region Translate Labels

                System.Web.UI.WebControls.Label lblMACHINE = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblMACHINE");
                lblMACHINE.Text = Translate(lblMACHINE.Text);
                System.Web.UI.WebControls.Label lblVariety = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblVariety");
                lblVariety.Text = Translate(lblVariety.Text);
                System.Web.UI.WebControls.Label lblFLOCK_ID = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblFLOCK_ID");
                lblFLOCK_ID.Text = Translate(lblFLOCK_ID.Text);
                System.Web.UI.WebControls.Label lblNumLote = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblNumLote");
                lblNumLote.Text = Translate(lblNumLote.Text);
                System.Web.UI.WebControls.Label lblLayDate = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblLayDate");
                lblLayDate.Text = Translate(lblLayDate.Text);
                System.Web.UI.WebControls.Label lblClasOvo = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblClasOvo");
                lblClasOvo.Text = Translate(lblClasOvo.Text);
                System.Web.UI.WebControls.Label lblHatcher = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHatcher");
                lblHatcher.Text = Translate(lblHatcher.Text);
                System.Web.UI.WebControls.Label lblQtdeOvos = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblQtdeOvos");
                lblQtdeOvos.Text = Translate(lblQtdeOvos.Text);
                System.Web.UI.WebControls.Label lblDataTransf = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblDataTransf");
                lblDataTransf.Text = Translate(lblDataTransf.Text);
                System.Web.UI.WebControls.Label lblHorarioInicio = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHorarioInicio");
                lblHorarioInicio.Text = Translate(lblHorarioInicio.Text);
                System.Web.UI.WebControls.Label lblContaminadoTransf = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblContaminadoTransf");
                lblContaminadoTransf.Text = Translate(lblContaminadoTransf.Text);
                System.Web.UI.WebControls.Label lblContaminadoRodizio = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblContaminadoRodizio");
                lblContaminadoRodizio.Text = Translate(lblContaminadoRodizio.Text);
                System.Web.UI.WebControls.Label lblBicados = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblBicados");
                lblBicados.Text = Translate(lblBicados.Text);
                System.Web.UI.WebControls.Label lblTransfTrincado = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblTransfTrincado");
                lblTransfTrincado.Text = Translate(lblTransfTrincado.Text);
                System.Web.UI.WebControls.Label lblTransfRodizio = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblTransfRodizio");
                lblTransfRodizio.Text = Translate(lblTransfRodizio.Text);
                System.Web.UI.WebControls.Label lblGrudados = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblGrudados");
                lblGrudados.Text = Translate(lblGrudados.Text);
                System.Web.UI.WebControls.Label lblPintosNascidos = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPintosNascidos");
                lblPintosNascidos.Text = Translate(lblPintosNascidos.Text);
                System.Web.UI.WebControls.Label lblPerdidosTransf = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPerdidosTransf");
                lblPerdidosTransf.Text = Translate(lblPerdidosTransf.Text);
                System.Web.UI.WebControls.Label lblPerdidosRodizio = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblPerdidosRodizio");
                lblPerdidosRodizio.Text = Translate(lblPerdidosRodizio.Text);
                System.Web.UI.WebControls.Label lblClaros = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblClaros");
                lblClaros.Text = Translate(lblClaros.Text);
                System.Web.UI.WebControls.Label lblHoraTermino = (System.Web.UI.WebControls.Label)FormView1.FindControl("lblHoraTermino");
                lblHoraTermino.Text = Translate(lblHoraTermino.Text);

                LinkButton UpdateButton = (LinkButton)FormView1.FindControl("UpdateButton");
                UpdateButton.Text = Translate(UpdateButton.Text);

                #endregion

                #region Mask to Hatcher Field

                System.Web.UI.WebControls.TextBox MachineTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MachineTextBox");
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
                        MaskedEditExtenderMachineTextBox.Mask = "H-99";
                        MaskedEditExtenderMachineTextBox.AutoCompleteValue = "H-";
                    }
                }

                #endregion
            }
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (FormView1.CurrentMode == FormViewMode.Edit)
                {
                    #region Carrega variáveis

                    DateTime dataIncubacao = Calendar1.SelectedDate;
                    string incubatorio = ddlIncubatorios.SelectedValue;
                    string company = GetFieldValueHatcheryCodes(incubatorio, "company");
                    string region = GetFieldValueHatcheryCodes(incubatorio, "region");
                    string location = GetLocation(company, incubatorio);

                    System.Web.UI.WebControls.Label MACHINELabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("MACHINELabel1");
                    string incubadora = MACHINELabel1.Text.ToUpper();
                    System.Web.UI.WebControls.Label VarietyLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("VarietyLabel1");
                    string linhagem = VarietyLabel1.Text;
                    System.Web.UI.WebControls.Label FLOCK_IDLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("FLOCK_IDLabel1");
                    string loteCompleto = FLOCK_IDLabel1.Text;
                    System.Web.UI.WebControls.Label EGG_KEYLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("EGG_KEYLabel1");
                    string numLote = EGG_KEYLabel1.Text;
                    //System.Web.UI.WebControls.Label LAY_DATELabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("LAY_DATELabel1");
                    //DateTime dataProducao = Convert.ToDateTime(LAY_DATELabel1.Text);
                    System.Web.UI.WebControls.Label ClassOvoLabel1 = (System.Web.UI.WebControls.Label)FormView1.FindControl("ClassOvoLabel1");
                    string classOvo = ClassOvoLabel1.Text;
                    System.Web.UI.WebControls.TextBox MachineTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("MachineTextBox");
                    string nascedouro = MachineTextBox.Text.ToUpper();
                    System.Web.UI.WebControls.TextBox EGG_UNITSTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                    int qtdOvos = Convert.ToInt32(EGG_UNITSTextBox.Text);
                    System.Web.UI.WebControls.TextBox TransfDateTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("TransfDateTextBox");
                    DateTime transfDate = Convert.ToDateTime(TransfDateTextBox.Text);
                    System.Web.UI.WebControls.TextBox HorarioTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("HorarioTextBox");
                    string horarioInicio = HorarioTextBox.Text;
                    System.Web.UI.WebControls.TextBox ContaminadoTransfTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("ContaminadoTransfTextBox");
                    int contaminadoTransf = Convert.ToInt32(ContaminadoTransfTextBox.Text);
                    System.Web.UI.WebControls.TextBox ContaminadoRodizioTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("ContaminadoRodizioTextBox");
                    int contaminadoRodizio = Convert.ToInt32(ContaminadoRodizioTextBox.Text);
                    System.Web.UI.WebControls.TextBox BicadosTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("BicadosTextBox");
                    int bicados = Convert.ToInt32(BicadosTextBox.Text);
                    System.Web.UI.WebControls.TextBox Trincados_TransferenciaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Trincados_TransferenciaTextBox");
                    int trincadosTransferencia = Convert.ToInt32(Trincados_TransferenciaTextBox.Text);
                    System.Web.UI.WebControls.TextBox Trincados_RodizioTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Trincados_RodizioTextBox");
                    int trincadosRodizio = Convert.ToInt32(Trincados_RodizioTextBox.Text);
                    System.Web.UI.WebControls.TextBox Num_GrudadosTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Num_GrudadosTextBox");
                    int numGrudados = Convert.ToInt32(Num_GrudadosTextBox.Text);
                    System.Web.UI.WebControls.TextBox Pintos_NascidosTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Pintos_NascidosTextBox");
                    int pintosNascidos = Convert.ToInt32(Pintos_NascidosTextBox.Text);
                    System.Web.UI.WebControls.TextBox Perdidos_TransferenciaTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Perdidos_TransferenciaTextBox");
                    int perdidosTransferencia = Convert.ToInt32(Perdidos_TransferenciaTextBox.Text);
                    System.Web.UI.WebControls.TextBox Perdidos_RodizioTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Perdidos_RodizioTextBox");
                    int perdidosRodizio = Convert.ToInt32(Perdidos_RodizioTextBox.Text);
                    System.Web.UI.WebControls.TextBox ClarosTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("ClarosTextBox");
                    int claros = Convert.ToInt32(ClarosTextBox.Text);
                    System.Web.UI.WebControls.TextBox Hora_TerminoTextBox = (System.Web.UI.WebControls.TextBox)FormView1.FindControl("Hora_TerminoTextBox");
                    string horarioTermino = Hora_TerminoTextBox.Text;

                    #endregion

                    #region Insere no WEB / FLIP

                    var listaIncubacaoDataProducao = hlbapp.HATCHERY_EGG_DATA
                        .Where(w => w.Set_date == dataIncubacao && w.Hatch_loc == incubatorio
                            && w.Flock_id == loteCompleto && w.Machine == incubadora
                            && w.ClassOvo == classOvo)
                        .GroupBy(g => g.Lay_date)
                        .OrderBy(g => g.Key)
                        .ToList();

                    DateTime maiorDataPrd = listaIncubacaoDataProducao.Max(m => m.Key);

                    HATCHERY_TRAN_DATA hatchTransf = new HATCHERY_TRAN_DATA();

                    hatchTransf.Hatch_Loc = incubatorio;
                    hatchTransf.Set_date = dataIncubacao;
                    hatchTransf.Flock_id = loteCompleto;
                    hatchTransf.NumLote = numLote;

                    hatchTransf.Lay_date = maiorDataPrd;

                    hatchTransf.Setter = incubadora;
                    hatchTransf.Hatcher = nascedouro;
                    hatchTransf.ClassOvo = classOvo;
                    hatchTransf.Transf_date = transfDate;
                    hatchTransf.Hora_Inicio = horarioInicio;
                    hatchTransf.Contaminado_Transferencia = contaminadoTransf;
                    hatchTransf.Contaminado_Rodizio = contaminadoRodizio;
                    hatchTransf.Bicados = bicados;
                    hatchTransf.Trincados_Transferencia = trincadosTransferencia;
                    hatchTransf.Trincados_Rodizio = trincadosRodizio;
                    hatchTransf.Num_Grudados = numGrudados;
                    hatchTransf.Pintos_Nascidos = pintosNascidos;
                    hatchTransf.Perdidos_Transferencia = perdidosTransferencia;
                    hatchTransf.Perdidos_Rodizio = perdidosRodizio;
                    hatchTransf.Hora_Termino = horarioTermino;
                    hatchTransf.Qtde_Ovos_Transferidos = qtdOvos;
                    hatchTransf.Variety = linhagem;
                    hatchTransf.Claros = claros;

                    hlbapp.HATCHERY_TRAN_DATA.AddObject(hatchTransf);

                    foreach (var item in listaIncubacaoDataProducao)
                    {
                        int rateioQtdOvos = 0;
                        rateioQtdOvos = qtdOvos / listaIncubacaoDataProducao.Count;

                        #region Insere / Atualiza no FLIP

                        UpdateTransferFLIP(company, region, location, incubatorio, dataIncubacao, loteCompleto,
                            maiorDataPrd, incubadora, nascedouro, rateioQtdOvos, "Inclusão", 
                            Convert.ToDateTime(hatchTransf.Transf_date), hatchTransf.Hora_Inicio, claros,
                            contaminadoTransf);

                        #endregion
                    }

                    hlbapp.SaveChanges();

                    #endregion

                    #region Refresh Tables

                    HatchGridDataSource.EnableCaching = false;
                    GridView1.DataBind();
                    HatchGridDataSource.EnableCaching = true;
                    EggInvDataSource.EnableCaching = false;
                    GridView3.DataBind();
                    EggInvDataSource.EnableCaching = true;
                    gvLotes.DataBind();
                    gvMaquinas.DataBind();
                    gvLinhagens.DataBind();

                    FormView1.ChangeMode(FormViewMode.ReadOnly);
                    //FormView1.Visible = false;

                    #endregion

                    AtualizaTotais();
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem.Text = "Erro ao Transferir: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem.Text = "Erro ao Transferir: " + ex.Message;
            }
        }

        #endregion

        #region Setting Eggs Table - GridView3

        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormView1.Visible = true;
            lblMensagem2.Visible = false;
            string loteCompleto = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;
            string incubadora = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            string classOvo = GridView3.Rows[GridView3.SelectedIndex].Cells[7].Text;

            HatchFormDataSource.SelectParameters["Flock_id"].DefaultValue = loteCompleto;
            HatchFormDataSource.SelectParameters["Machine"].DefaultValue = incubadora;

            //var listaIncubacaoDataProducao = hlbapp.HATCHERY_EGG_DATA
            //    .Where(w => w.Set_date == Calendar1.SelectedDate && w.Hatch_loc == ddlIncubatorios.SelectedValue
            //        && w.Flock_id == loteCompleto && w.Machine == incubadora
            //        && w.ClassOvo == classOvo)
            //    .GroupBy(g => g.Lay_date)
            //    .OrderBy(g => g.Key)
            //    .ToList();

            //DateTime maiorDataProducao = listaIncubacaoDataProducao.Max(m => m.Key);

            //HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[7].Text;
            //HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = maiorDataProducao.ToShortDateString();
            HatchFormDataSource.SelectParameters["ClassOvo"].DefaultValue = classOvo;
            
            lblMensagem.Visible = false;
            Session["tipoCadastro"] = "Estoque Real";
            FormView1.ChangeMode(FormViewMode.Edit);
        }

        #endregion

        #region Transfer Table - GridView1

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                #region Carrega Variáveis

                DateTime dataIncubacao = Calendar1.SelectedDate;
                string incubatorio = ddlIncubatorios.SelectedValue;
                string company = GetFieldValueHatcheryCodes(incubatorio, "company");
                string region = GetFieldValueHatcheryCodes(incubatorio, "region");
                string location = GetLocation(company, incubatorio);

                GridViewRow row = (GridViewRow)GridView1.Rows[e.RowIndex];

                string loteCompleto = row.Cells[3].Text;
                DateTime dataProducao = Convert.ToDateTime(row.Cells[5].Text);
                string incubadora = row.Cells[6].Text;
                string nascedouro = row.Cells[7].Text;
                string classOvo = row.Cells[8].Text;
                int qtdOvos = Convert.ToInt32(row.Cells[10].Text);

                DateTime dataTransf = DateTime.Today;
                string horaTransf = "";
                int claros = 0;
                int contTransf = 0;
                HATCHERY_TRAN_DATA transferencia = hlbapp.HATCHERY_TRAN_DATA
                    .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                        && w.Flock_id == loteCompleto && w.Lay_date == dataProducao
                        && w.Setter == incubadora && w.Hatcher == nascedouro)
                    .FirstOrDefault();

                if (transferencia != null)
                {
                    dataTransf = Convert.ToDateTime(transferencia.Transf_date);
                    horaTransf = transferencia.Hora_Inicio;
                    claros = (int)transferencia.Claros;
                    contTransf = (int)transferencia.Contaminado_Transferencia;
                }

                #endregion

                #region Insere / Atualiza no FLIP

                UpdateTransferFLIP(company, region, location, incubatorio, dataIncubacao, loteCompleto,
                    dataProducao, incubadora, nascedouro, qtdOvos, "Exclusão",
                    dataTransf, horaTransf, claros, contTransf);

                #endregion

                EggInvDataSource.DataBind();
                GridView3.DataBind();
                gvLotes.DataBind();
                gvMaquinas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensagem2.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem2.Text = "Erro ao Deletar Transferência: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem2.Text = "Erro ao Deletar Transferência: " + ex.Message;
            }
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

            try
            {
                #region Carrega Variáveis

                DateTime dataIncubacao = Calendar1.SelectedDate;
                string incubatorio = ddlIncubatorios.SelectedValue;
                HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, incubatorio);
                string location = flip.HATCHERY_CODES[0].LOCATION;

                string loteCompleto = e.Keys[3].ToString();
                DateTime dataProducao = Convert.ToDateTime(e.Keys[5].ToString());
                string incubadora = e.Keys[6].ToString();
                string nascedouro = e.Keys[7].ToString();
                string clasOvo = e.Keys[8].ToString();

                #endregion

                #region Atualiza Dados Retirada / Nascimento WEB

                HATCHERY_FLOCK_SETTER_DATA nasc = hlbapp.HATCHERY_FLOCK_SETTER_DATA
                    .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                        && w.Flock_id == loteCompleto && w.Setter == incubadora
                        && w.Hatcher == nascedouro
                        && w.ClassOvo == clasOvo).FirstOrDefault();

                if (nasc != null)
                {
                    List<HATCHERY_TRAN_DATA> listTransf = hlbapp.HATCHERY_TRAN_DATA
                        .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
                            && w.Flock_id == loteCompleto && w.Setter == incubadora
                            && w.Hatcher == nascedouro
                            && w.ClassOvo == clasOvo).ToList();

                    if (listTransf.Count == 0)
                    {
                        hlbapp.HATCHERY_FLOCK_SETTER_DATA.DeleteObject(nasc);
                    }
                }

                hlbapp.SaveChanges();

                AtualizaTotais();

                #endregion

                GridView3.DataBind();
                gvLinhagens.DataBind();
                gvLotes.DataBind();
                gvMaquinas.DataBind();
            }
            catch (Exception ex)
            {
                lblMensagem2.Visible = true;
                if (ex.InnerException != null)
                    lblMensagem2.Text = "Erro ao Deletar Transferência: " + ex.Message + " / Segundo erro: " + ex.InnerException.Message;
                else
                    lblMensagem2.Text = "Erro ao Deletar Transferência: " + ex.Message;
            }

        }

        protected void ibtnDeleteItemTranferido_Click(object sender, ImageClickEventArgs e)
        {
            
        }

        #endregion

        #region BD Methods

        #region Set Methods
        //HYBR BR PP 05/28/21 NMGEP17-GE178902LS
        public void UpdateTransferFLIP(string company, string region, string location, string hatchLoc, DateTime setDate, 
            string flockID, DateTime layDate, string setter, 
            string hatcher, int eggsQty, string operation, DateTime transferDate, string transferTime,
            int clearEggs, int contTransf)
        {
            int posicaoHifen = flockID.IndexOf("-") + 1;
            int tamanho = flockID.Length - posicaoHifen;
            string flock = flockID.Substring(posicaoHifen, tamanho);

            string eggKey = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) 
                + hatchLoc + flockID;

            string trackNo = "EXP" + layDate.ToString("yyMMdd");

            string dataRetirada = transferDate.ToShortDateString();
            string horaRetirada = transferTime;

            if (company == "HYBR")
            {
                #region HYBR

                HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();

                #region Se tem no FLIP, deleta do FLIP

                var listaItensDeletar = htdTA.GetDataByEggKeyAndSetter(eggKey, setter);
                foreach (var item in listaItensDeletar)
                {
                    var existeWeb = hlbapp.HATCHERY_TRAN_DATA
                        .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                            && w.Flock_id == flockID
                            && w.Setter == setter && w.Hatcher == item.HATCHER)
                    .FirstOrDefault();

                    if (existeWeb == null)
                        htdTA.DeleteByEggKeyAndSetter(eggKey, setter);
                }

                #endregion

                FLIPDataSet.HATCHERY_TRAN_DATADataTable htdDT = new FLIPDataSet.HATCHERY_TRAN_DATADataTable();
                htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, layDate, setter, hatcher);

                if (htdDT.Count == 0)
                {
                    if (operation.Equals("Inclusão"))
                        //Segundo o chamado 90839 - Maria 
                        htdTA.Insert(eggKey, layDate, eggsQty, setter, hatcher, trackNo, contTransf, clearEggs,
                            // Inserido número 1 no campo NUM_8 para identificar que veio do Web
                            null, null, null, null, null, 1, dataRetirada, transferTime, null);
                }
                else
                {
                    FLIPDataSet.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                    int qtdOvosTotal = 0;
                    if (operation.Equals("Inclusão"))
                        qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) + eggsQty;
                    else if (operation.Equals("Exclusão"))
                        qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) - eggsQty;
                    else
                    {
                        int qtdOvosInserido = Convert.ToInt32(hlbapp.HATCHERY_TRAN_DATA
                            .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                                && w.Flock_id == flockID && w.Lay_date == layDate
                                && w.Setter == setter && w.Hatcher == hatcher)
                            .Sum(s => s.Qtde_Ovos_Transferidos));

                        qtdOvosTotal = qtdOvosInserido;
                    }

                    if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                    transfFLIP.NUM_1 = contTransf;
                    if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                    transfFLIP.NUM_2 = clearEggs;
                    transfFLIP.TEXT_1 = dataRetirada;
                    transfFLIP.TEXT_2 = horaRetirada;
                    transfFLIP.EGGS_TRAN = qtdOvosTotal;
                        if (qtdOvosTotal == 0)
                        htdTA.Delete(eggKey, layDate, setter, hatcher, trackNo);
                    else
                        htdTA.Update(transfFLIP);

                    #region Update "Contaminado na Transferência" inside Hatch Data

                    FLIPDataSet.HATCHERY_FLOCK_DATADataTable hfdDT = new FLIPDataSet.HATCHERY_FLOCK_DATADataTable();
                    HATCHERY_FLOCK_DATATableAdapter hfdTA = new HATCHERY_FLOCK_DATATableAdapter();
                    hfdTA.FillByFlockData(hfdDT, company, region, location, setDate, hatchLoc, flockID);

                    HLBAPPEntities hlbappSession = new HLBAPPEntities();
                    var listaTransfPorLote = hlbappSession.HATCHERY_TRAN_DATA
                        .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate && w.Flock_id == flockID).ToList();

                    if (listaTransfPorLote.Count > 0)
                    {
                        var contTransfLote = listaTransfPorLote.Sum(s => s.Contaminado_Transferencia);

                        FLIPDataSet.HATCHERY_FLOCK_DATARow hfdRow = hfdDT[0];
                        if (!hfdRow.IsNUM_18Null()) hfdRow.NUM_18 = 0;
                        hfdRow.NUM_18 = Convert.ToDecimal(contTransfLote);
                        hfdTA.Update(hfdRow);
                    }

                    #endregion
                }

                #endregion
            }
            else if (company == "HYCO")
            {
                #region HYCO

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter htdTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_TRAN_DATATableAdapter();

                ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter hedTA =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKSTableAdapters.HATCHERY_EGG_DATATableAdapter();
                ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable hedDT =
                    new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_EGG_DATADataTable();

                #region Se tem no FLIP e não no Web, deleta do FLIP

                var listaItensDeletar = htdTA.GetDataByEggKAndLayDateAndSetterAndHatcher(eggKey, layDate, setter, hatcher);
                foreach (var item in listaItensDeletar)
                {
                    var existeWeb = hlbapp.HATCHERY_TRAN_DATA
                        .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                            && w.Flock_id == flockID
                            && w.Lay_date == layDate
                            && w.Setter == setter && w.Hatcher == item.HATCHER)
                    .FirstOrDefault();

                    if (existeWeb == null)
                        htdTA.Delete(eggKey, layDate, setter, hatcher, trackNo);
                }

                #endregion

                hedTA.FillByFlockData(hedDT, company, region, location, setDate, hatchLoc, flockID, layDate);
                
                int rateioQtdOvos = 0;
                int qtdRegistrosHED = 1;
                if (hedDT.Count > 0) qtdRegistrosHED = hedDT.Count;
                rateioQtdOvos = eggsQty / qtdRegistrosHED;

                foreach (var item in hedDT)
                {
                    string eggKey02 = company + region + location + setDate.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR"))
                        + hatchLoc + item.FLOCK_ID;

                    ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATADataTable htdDT =
                        new ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATADataTable();
                    htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey02, layDate, setter, hatcher);

                    if (htdDT.Count == 0)
                    {
                        if (operation.Equals("Inclusão"))
                            htdTA.Insert(eggKey02, layDate, rateioQtdOvos, setter, hatcher, trackNo, contTransf, clearEggs,
                                null, null, null, null, null, null, dataRetirada, transferTime);
                    }
                    else
                    {
                        ImportaIncubacao.Data.FLIP.HCFLOCKS.HATCHERY_TRAN_DATARow transfFLIP = htdDT[0];
                        int qtdOvosTotal = 0;
                        if (operation.Equals("Inclusão"))
                            qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) + rateioQtdOvos;
                        else if (operation.Equals("Exclusão"))
                            qtdOvosTotal = Convert.ToInt32(transfFLIP.EGGS_TRAN) - rateioQtdOvos;
                        else
                        {
                            int qtdOvosInserido = Convert.ToInt32(hlbapp.HATCHERY_TRAN_DATA
                                .Where(w => w.Hatch_Loc == hatchLoc && w.Set_date == setDate
                                    && w.Flock_id == flockID && w.Lay_date == layDate
                                    && w.Setter == setter && w.Hatcher == hatcher)
                                .Sum(s => s.Qtde_Ovos_Transferidos)) / hedDT.Count;

                            qtdOvosTotal = qtdOvosInserido;
                        }

                        if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                        transfFLIP.NUM_1 = contTransf;
                        if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                        transfFLIP.NUM_2 = clearEggs;
                        transfFLIP.TEXT_1 = dataRetirada;
                        transfFLIP.TEXT_2 = horaRetirada;
                        transfFLIP.EGGS_TRAN = qtdOvosTotal;
                        if (qtdOvosTotal == 0)
                            htdTA.Delete(eggKey02, layDate, setter, hatcher, trackNo);
                        else
                            htdTA.Update(transfFLIP);
                    }
                }

                #endregion
            }
        }

        public void RefreshFLIP(string hatchLoc, DateTime data)
        {
            //DateTime data = Convert.ToDateTime("26/10/2016");
            //string hatchLoc = "NM";

            string company = GetFieldValueHatcheryCodes(hatchLoc, "company");
            string region = GetFieldValueHatcheryCodes(hatchLoc, "region");
            string location = GetLocation(company, hatchLoc);

            var lista = hlbapp.HATCHERY_TRAN_DATA
                .Where(w => w.Set_date == data && w.Hatch_Loc == hatchLoc)
                .GroupBy(g => new {
                    g.Hatch_Loc,
                    g.Set_date,
                    g.Flock_id,
                    g.Setter,
                    g.Hatcher
                })
                .Select(s => new { 
                    s.Key.Hatch_Loc,
                    s.Key.Set_date,
                    s.Key.Flock_id,
                    Lay_date = s.Max(m => m.Lay_date),
                    s.Key.Setter,
                    s.Key.Hatcher,
                    Qtde_Ovos_Transferidos = s.Sum(sum => sum.Qtde_Ovos_Transferidos),
                    Transf_date = s.Max(m => m.Transf_date),
                    Hora_Inicio = s.Max(m => m.Hora_Inicio),
                    Claros = s.Sum(sum => sum.Claros),
                    Contaminado_Transferencia = s.Sum(sum => sum.Contaminado_Transferencia)
                })
                .ToList();

            #region Deleta os Dados Todos (COMENTADO - SOMENTE ATUALIZAÇÃO MANUAL)

            //var listaAll = hlbapp.HATCHERY_TRAN_DATA
            //    .GroupBy(g => new
            //    {
            //        g.Hatch_Loc,
            //        g.Set_date
            //    })
            //    .ToList();

            //foreach (var item in listaAll)
            //{
            //    HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();
            //    string eggKey = "HYBRBR" + location + Convert.ToDateTime(item.Key.Set_date).ToString("MM/dd/yy")
            //        + item.Key.Hatch_Loc;
            //    htdTA.DeleteBySetDateAndHatchLoc(eggKey);
            //}

            #endregion

            foreach (var item in lista)
            {
                #region Carrega Variáveis

                DateTime dataIncubacao = Convert.ToDateTime(item.Set_date);
                string incubatorio = item.Hatch_Loc;
                string loteCompleto = item.Flock_id;
                DateTime dataProducao = Convert.ToDateTime(item.Lay_date);
                string incubadora = item.Setter;
                string nascedouro = item.Hatcher;
                //string classOvo = item.ClassOvo;
                int qtdOvos = Convert.ToInt32(item.Qtde_Ovos_Transferidos);
                DateTime dataTransferencia = Convert.ToDateTime(item.Transf_date);
                string horaTransferencia = item.Hora_Inicio;
                int claros = (int)item.Claros;
                int contTransf = (int)item.Contaminado_Transferencia;

                #endregion

                #region Atualiza Dados

                var listaIncubacaoDataProducao = hlbapp.HATCHERY_EGG_DATA
                    .Where(w => w.Set_date == dataIncubacao && w.Hatch_loc == incubatorio
                        && w.Flock_id == loteCompleto && w.Machine == incubadora)
                        //&& w.ClassOvo == classOvo)
                    .GroupBy(g => g.Lay_date)
                    .OrderBy(g => g.Key)
                    .ToList();

                foreach (var inc in listaIncubacaoDataProducao)
                {
                    int rateioQtdOvos = 0;
                    rateioQtdOvos = qtdOvos / listaIncubacaoDataProducao.Count;

                    #region Deleta os Dados

                    UpdateTransferFLIP(company, region, location, incubatorio, dataIncubacao, loteCompleto,
                        inc.Key, incubadora, nascedouro, rateioQtdOvos, "Exclusão",
                        dataTransferencia, horaTransferencia, claros, contTransf);

                    #endregion

                    #region Insere / Atualiza no FLIP

                    UpdateTransferFLIP(company, region, location, incubatorio, dataIncubacao, loteCompleto,
                        inc.Key, incubadora, nascedouro, rateioQtdOvos, "Inclusão",
                        dataTransferencia, horaTransferencia, claros, contTransf);

                    #endregion
                }

                #endregion
            }
        }

        #endregion

        #region Get Methods

        public string GetFieldValueHatcheryCodes(string hatchLoc, string field)
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

        public string GetLocation(string company, string hatchLoc)
        {
            string location = "";

            if (company == "HYBR")
            {
                HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
                hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
                location = flip.HATCHERY_CODES[0].LOCATION;
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

        #endregion

        #region Old Methods

        public void AtualizaTransferenciaFLIP(string location, string incubatorio, DateTime dataIncubacao,
            string loteCompleto, DateTime dataProducao, bool consideraDataProducao, string incubadora,
            string nascedouro, int qtdOvos, string operacao, DateTime dataTransferencia, string horaTransferencia,
            int claros, int contTransf)
        {
            string eggKey = "HYBRBR" + location + dataIncubacao.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) + incubatorio
                + loteCompleto;

            string trackNo = "EXP" + dataProducao.ToString("yyMMdd");

            //HATCHERY_TRAN_DATA transferencia = hlbapp.HATCHERY_TRAN_DATA
            //    .Where(w => w.Hatch_Loc == incubatorio && w.Set_date == dataIncubacao
            //        && w.Flock_id == loteCompleto && w.Lay_date == dataProducao
            //        && w.Setter == incubadora && w.Hatcher == nascedouro)
            //    .FirstOrDefault();

            string dataRetirada = dataTransferencia.ToShortDateString();
            string horaRetirada = horaTransferencia;
            //if (transferencia != null)
            //{
            //    dataRetirada = Convert.ToDateTime(transferencia.Transf_date).ToShortDateString();
            //    horaRetirada = transferencia.Hora_Inicio.Replace(":", "H");
            //}

            FLIPDataSet.HATCHERY_TRAN_DATADataTable htdDT = new FLIPDataSet.HATCHERY_TRAN_DATADataTable();
            HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();

            if (consideraDataProducao)
            {
                #region Ajusta Considerando Data de Produção

                htdTA.FillByEggKeyAndLayDateAndSetterAndHatcher(htdDT, eggKey, dataProducao, incubadora, nascedouro);

                if (htdDT.Count == 0)
                {
                    if (operacao.Equals("Inclusão"))
                        htdTA.Insert(eggKey, dataProducao, qtdOvos, incubadora, nascedouro, trackNo, contTransf, claros,
                            null, null, null, null, null, null, dataRetirada, horaTransferencia, null);
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

                    if (!transfFLIP.IsNUM_1Null()) transfFLIP.NUM_1 = 0;
                    transfFLIP.NUM_2 = contTransf;
                    if (!transfFLIP.IsNUM_2Null()) transfFLIP.NUM_2 = 0;
                    transfFLIP.NUM_2 = claros;
                    transfFLIP.TEXT_1 = dataRetirada;
                    transfFLIP.TEXT_2 = horaRetirada;
                    transfFLIP.EGGS_TRAN = qtdOvosTotal;
                    if (qtdOvosTotal == 0)
                        htdTA.Delete(eggKey, dataProducao, incubadora, nascedouro, trackNo);
                    else
                        htdTA.Update(transfFLIP);
                }

                #endregion
            }
            else
            {
                #region Ajusta Desconsiderando Data de Produção

                if (operacao.Equals("Exclusão"))
                {
                    htdTA.FillByEggKeyAndSetterAndHatcher(htdDT, eggKey, incubadora, nascedouro);

                    foreach (var item in htdDT)
                    {
                        htdTA.Delete(item.EGG_KEY, item.LAY_DATE, item.MACHINE, item.HATCHER, item.TRACK_NO);
                    }
                }

                #endregion
            }
        }

        public void AtualizaFLIP()
        {
            DateTime data = Calendar1.SelectedDate;
            string hatchLoc = ddlIncubatorios.SelectedValue;
            //DateTime data = Convert.ToDateTime("26/10/2016");
            //string hatchLoc = "NM";

            HATCHERY_CODESTableAdapter hatchCodes = new HATCHERY_CODESTableAdapter();
            hatchCodes.FillByHatchLoc(flip.HATCHERY_CODES, hatchLoc);
            string location = flip.HATCHERY_CODES[0].LOCATION;

            var lista = hlbapp.HATCHERY_TRAN_DATA
                .Where(w => w.Set_date == data && w.Hatch_Loc == hatchLoc)
                .ToList();

            #region Deleta os Dados Todos (COMENTADO - SOMENTE ATUALIZAÇÃO MANUAL)

            //var listaAll = hlbapp.HATCHERY_TRAN_DATA
            //    .GroupBy(g => new
            //    {
            //        g.Hatch_Loc,
            //        g.Set_date
            //    })
            //    .ToList();

            //foreach (var item in listaAll)
            //{
            //    HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();
            //    string eggKey = "HYBRBR" + location + Convert.ToDateTime(item.Key.Set_date).ToString("MM/dd/yy")
            //        + item.Key.Hatch_Loc;
            //    htdTA.DeleteBySetDateAndHatchLoc(eggKey);
            //}

            #endregion

            #region Deleta Dados p/ Incubação e Incubatório

            HATCHERY_TRAN_DATATableAdapter htdTA = new HATCHERY_TRAN_DATATableAdapter();
            string eggKey = "HYBRBR" + location + data.ToString("MM/dd/yy", CultureInfo.GetCultureInfo("pt-BR")) + hatchLoc;
            htdTA.DeleteBySetDateAndHatchLoc(eggKey);

            #endregion

            foreach (var item in lista)
            {
                #region Carrega Variáveis

                DateTime dataIncubacao = Convert.ToDateTime(item.Set_date);
                string incubatorio = item.Hatch_Loc;
                string loteCompleto = item.Flock_id;
                DateTime dataProducao = Convert.ToDateTime(item.Lay_date);
                string incubadora = item.Setter;
                string nascedouro = item.Hatcher;
                string classOvo = item.ClassOvo;
                int qtdOvos = Convert.ToInt32(item.Qtde_Ovos_Transferidos);
                DateTime dataTransferencia = Convert.ToDateTime(item.Transf_date);
                string horaTransferencia = item.Hora_Inicio;
                int claros = (int)item.Claros;
                int contTransf = (int)item.Contaminado_Transferencia;

                #endregion

                #region Deleta os Dados

                AtualizaTransferenciaFLIP(location, incubatorio, dataIncubacao, loteCompleto,
                    dataProducao, false, incubadora, nascedouro, qtdOvos, "Exclusão",
                    dataTransferencia, horaTransferencia, claros, contTransf);

                #endregion

                #region Insere Novamente

                var listaIncubacaoDataProducao = hlbapp.HATCHERY_EGG_DATA
                    .Where(w => w.Set_date == dataIncubacao && w.Hatch_loc == incubatorio
                        && w.Flock_id == loteCompleto && w.Machine == incubadora
                        && w.ClassOvo == classOvo)
                    .GroupBy(g => g.Lay_date)
                    .OrderBy(g => g.Key)
                    .ToList();

                foreach (var inc in listaIncubacaoDataProducao)
                {
                    int rateioQtdOvos = 0;
                    rateioQtdOvos = qtdOvos / listaIncubacaoDataProducao.Count;

                    #region Insere / Atualiza no FLIP

                    AtualizaTransferenciaFLIP(location, incubatorio, dataIncubacao, loteCompleto,
                        inc.Key, true, incubadora, nascedouro, rateioQtdOvos, "Inclusão",
                        dataTransferencia, horaTransferencia, claros, contTransf);

                    #endregion
                }

                #endregion
            }
        }

        #endregion

        #endregion

        #region Reports

        protected void lbtnExportar_Click(object sender, EventArgs e)
        {
            string destino = Session["destinoRelTransfWEB"].ToString();
            Response.AddHeader("Content-Length", new System.IO.FileInfo(destino).Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=Relatorio_Transferencia_Eclosao_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".xlsx");
            Response.ContentType = "application/vnd.ms-excel";
            Response.TransmitFile(destino);
        }

        public string GeraRelatorioTransf(string pesquisa, bool deletaArquivoAntigo, string pasta,
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

            System.IO.File.Copy("C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Transferencia_Eclosao.xlsx", destino);

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
                    "VU_Transferencia_WEB ";

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
                    "5,6";

            Excel._Worksheet worksheet = (Excel._Worksheet)oBook.Worksheets["Controle de Transf. - Eclosão"];

            //worksheet.Cells[2, 8] = dataInicial;
            //worksheet.Cells[3, 8] = dataFinal;

            Connections lista = oBook.Connections;

            foreach (Excel.WorkbookConnection item in lista)
            {
                item.OLEDBConnection.BackgroundQuery = false;
                if (item.Name.Equals("TransferenciaWEB"))
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
            string destino = "C:\\inetpub\\wwwroot\\Relatorios\\Relatorio_Transferencia_Eclosao_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            string pesquisa = "*Relatorio_Transferencia_Eclosao_" + Session["login"].ToString() + Session.SessionID + ".xlsx";

            Session["destinoRelTransfWEB"] = GeraRelatorioTransf(pesquisa, true, pasta, destino);

            lbtnExportar.Visible = true;

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

                #endregion

                #region Change Header Transfer Eggs Table

                GridView1.Columns[1].HeaderText = Translate(GridView1.Columns[1].HeaderText);
                GridView1.Columns[2].HeaderText = Translate(GridView1.Columns[2].HeaderText);
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
                GridView1.Columns[19].HeaderText = Translate(GridView1.Columns[19].HeaderText);
                GridView1.Columns[20].HeaderText = Translate(GridView1.Columns[20].HeaderText);
                GridView1.Columns[21].HeaderText = Translate(GridView1.Columns[21].HeaderText);
                GridView1.Columns[22].HeaderText = Translate(GridView1.Columns[22].HeaderText);

                #endregion

                #region Change Header Setter Conference Table

                gvMaquinas.Columns[0].HeaderText = Translate(gvMaquinas.Columns[0].HeaderText);
                gvMaquinas.Columns[1].HeaderText = Translate(gvMaquinas.Columns[1].HeaderText);

                #endregion

                #region Change Header Flock Conference Table

                gvLotes.Columns[0].HeaderText = Translate(gvLotes.Columns[0].HeaderText);
                gvLotes.Columns[1].HeaderText = Translate(gvLotes.Columns[1].HeaderText);

                #endregion

                #region Change Header Variety Conference Table

                gvLinhagens.Columns[0].HeaderText = Translate(gvLinhagens.Columns[0].HeaderText);
                gvLinhagens.Columns[1].HeaderText = Translate(gvLinhagens.Columns[1].HeaderText);

                #endregion

                #region Another Components

                hlBackHome.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("HL_Back_To_Home", Session["language"].ToString());
                Label5.Text = Translate(Label5.Text);
                Label9.Text = Translate(Label9.Text);
                Label4.Text = Translate(Label4.Text);
                lbtnExportar.Text = Translate(lbtnExportar.Text);
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

                btn_Pesquisar.Text = Translate(btn_Pesquisar.Text);
                Label1.Text = Translate(Label1.Text);
                Label2.Text = Translate(Label2.Text);
                Button2.Text = Translate(Button2.Text);
                lblTotalOvosIncubados.Text = Translate(lblTotalOvosIncubados.Text);
                lblMaquinasUtilizadas.Text = Translate(lblMaquinasUtilizadas.Text);
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