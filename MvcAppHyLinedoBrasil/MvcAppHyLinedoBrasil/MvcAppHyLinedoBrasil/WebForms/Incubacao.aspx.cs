using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.Data;
using MvcAppHyLinedoBrasil.Data.FLIPDataSetTableAdapters;
using AjaxControlToolkit;
using MvcAppHyLinedoBrasil.Controllers;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class Incubacao : System.Web.UI.Page
    {
        #region Objects

        HLBAPPEntities bdSQLServer = new HLBAPPEntities();

        FLIPDataSet flipDataSet = new FLIPDataSet();

        SETDAY_DATATableAdapter setDayData = new SETDAY_DATATableAdapter();
        HATCHERY_FLOCK_DATATableAdapter hatcheryFlockData = new HATCHERY_FLOCK_DATATableAdapter();
        HATCHERY_EGG_DATATableAdapter hatcheryEggData = new HATCHERY_EGG_DATATableAdapter();
        FLOCK_DATATableAdapter flockData = new FLOCK_DATATableAdapter();
        EGGINV_DATATableAdapter eggInvData = new EGGINV_DATATableAdapter();
        FLOCKSTableAdapter flocks = new FLOCKSTableAdapter();

        public static string linhagem;
        public static int age;
        public static int qtde;
        public static DateTime dataNascimentoLote;

        public static string tipoCadastro;

        #endregion

        #region Egg Inventory Table

        protected void GridView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            HatchFormDataSource.SelectParameters["FLOCK_ID"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            HatchFormDataSource.SelectParameters["TRACK_NO"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[4].Text;
            HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = GridView3.Rows[GridView3.SelectedIndex].Cells[6].Text;
            linhagem = GridView3.Rows[GridView3.SelectedIndex].Cells[5].Text;

            string farmid = GridView3.Rows[GridView3.SelectedIndex].Cells[1].Text;
            string flockid = GridView3.Rows[GridView3.SelectedIndex].Cells[2].Text;
            DateTime layDate = Convert.ToDateTime(GridView3.Rows[GridView3.SelectedIndex].Cells[6].Text);

            //flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP", 
            //    farmid, flockid, layDate.ToString("dd/MM/yyyy"));
            flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP",
                farmid, flockid, layDate);

            age = Convert.ToInt32(flipDataSet.FLOCK_DATA[0].AGE);
            qtde = Convert.ToInt32(GridView3.Rows[GridView3.SelectedIndex].Cells[8].Text.Replace(".", ""));
            //try
            //{
            //    media = Convert.ToDecimal(GridView3.Rows[GridView3.SelectedIndex].Cells[8].Text);
            //}
            //catch (Exception ex)
            //{
            //    media = 0;
            //}
            //HatchFormDataSource.SelectParameters["Horario"].DefaultValue = DateTime.Now.ToString("HH:mm");
            lblMensagem.Visible = false;
            tipoCadastro = "Estoque Real";
            FormView1.ChangeMode(FormViewMode.Edit);
        }

        protected void GridView3_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            #region Change Label Language

            string language = Session["language"].ToString();

            if (language != "pt-BR")
            {
                #region ItemTemplate

                Label lblID = (Label)e.Row.FindControl("Label9");
                if (lblID != null)
                {
                    
                }

                #endregion
            }

            #endregion
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }
            if (TextBox1.Text.Equals(""))
            {
                TextBox1.Text = "0";
            }
            if (IsPostBack == false)
            {
                Calendar1.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                Session["hatchLocal"] = ddlIncubatorios.SelectedValue;
                Session["setDate"] = Calendar1.SelectedDate;

                //DateTime data = Convert.ToDateTime("09/07/2013");
                DateTime data = Calendar1.SelectedDate;

                AtualizaFLIP(data);
                AtualizaTotais();
            }
            else
            {
                if (Session["setDate"] == null)
                {
                    Response.Redirect("http://hlbapp.hyline.com.br");
                }
            }
            //GridView3.DataBind();
            //TextBox6.Text = "";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            if (TextBox6.Text.Equals(""))
            {
                TextBox6.Text = "0";
            }
            GridView3.DataBind();
        }

        protected void FormView1_DataBound(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                if (FARM_IDLabel1 != null)
                {
                    if (tipoCadastro == "Estoque Futuro")
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

                Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                if (FLOCK_IDLabel1 != null)
                {
                    if (tipoCadastro == "Estoque Futuro")
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

                Label TRACK_NOLabel1 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                if (TRACK_NOLabel1 != null)
                {
                    if (tipoCadastro == "Estoque Futuro")
                    {
                        TRACK_NOLabel1.Visible = false;
                    }
                    else
                    {
                        TRACK_NOLabel1.Visible = true;
                    }
                }

                Label DAT_Label1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                if (DAT_Label1 != null)
                {
                    if (tipoCadastro == "Estoque Futuro")
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

                Calendar Calendario = (Calendar)FormView1.FindControl("Lay_DateCalendar");
                if (Calendario != null)
                {
                    if (tipoCadastro == "Estoque Futuro")
                    {
                        Calendario.Visible = true;
                        Calendario.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                        Label TRACK_NOLabel2 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                        if (TRACK_NOLabel2 != null)
                        {
                            TRACK_NOLabel2.Visible = true;
                            TRACK_NOLabel2.Text = "EXP" + Calendario.SelectedDate.ToString("yyMMdd");
                        }
                    }
                    else
                    {
                        Calendario.Visible = false;
                    }
                }

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                if (EGG_UNITSTextBox != null)
                {
                    decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }
            }

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
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;
            if (TextBox1.Text.Equals(""))
            {
                TextBox1.Text = "0";
            }
            GridView1.DataBind();
        }

        protected void FormView1_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            DateTime dataIncubacao = Calendar1.SelectedDate;

            try
            {
                if (FormView1.CurrentMode == FormViewMode.Edit)
                {
                    string farmID = "";
                    string flockID = "";
                    Label TRACK_NOLabel1 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                    string trackNO = TRACK_NOLabel1.Text;
                    DateTime layDate;

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
                    //TextBox BandejasTextBox = (TextBox)FormView1.FindControl("BandejasTextBox");
                    //decimal bandejas = Convert.ToDecimal(BandejasTextBox.Text);
                    decimal bandejas = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(eggUnits) / 150.0), 1));
                    TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                    decimal media = Convert.ToDecimal(MediaEclosaoTextBox.Text);
                    TextBox ObservacaoTextBox = (TextBox)FormView1.FindControl("ObservacaoTextBox");
                    string observacao = ObservacaoTextBox.Text;

                    string status = "";
                    if (tipoCadastro == "Estoque Futuro")
                    {
                        DropDownList ddlFarm = (DropDownList)FormView1.FindControl("DropDownList4");
                        farmID = ddlFarm.SelectedValue;
                        DropDownList ddlLote = (DropDownList)FormView1.FindControl("DropDownList3");
                        flockID = ddlLote.SelectedValue;
                        Calendar clDataProducao = (Calendar)FormView1.FindControl("Lay_DateCalendar");
                        layDate = clDataProducao.SelectedDate;
                    }
                    else
                    {
                        Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                        farmID = FARM_IDLabel1.Text;
                        Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                        flockID = FLOCK_IDLabel1.Text;
                        Label LAY_DATELabel1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                        layDate = Convert.ToDateTime(LAY_DATELabel1.Text);
                        status = "Importado";
                    }

                    string numLote = "";
                    flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", farmID, flockID);
                    if (flipDataSet.FLOCKS.Count > 0)
                    {
                        dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
                        age = ((layDate - dataNascimentoLote).Days) / 7;
                        linhagem = flipDataSet.FLOCKS[0].VARIETY;
                        numLote = flipDataSet.FLOCKS[0].NUM_1.ToString();
                    }

                    // Insere na base SQL Server
                    var qtd = eggUnits;

                    int posicaoFiltro = Convert.ToInt32(posicao);
                    string flockIDFiltro = farmID + "-" + flockID;
                    int existeSQL = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Company == "HYBR" && h.Region == "BR" && h.Location == "PP" && h.Set_date == dataIncubacao
                        && h.Hatch_loc == ddlIncubatorios.SelectedValue && h.Flock_id == flockIDFiltro && h.Lay_date == layDate && h.Machine == machine
                        && h.Track_no == trackNO && h.Posicao == posicaoFiltro)
                    .Count();

                    if (existeSQL != 0)
                    {
                        //qtd = qtd + Convert.ToDecimal(hatcheryEggDataObjectDelete.Eggs_rcvd);
                        //bdSQLServer.HATCHERY_EGG_DATA.DeleteObject(hatcheryEggDataObjectDelete);
                        lblMensagem.Visible = true;
                        lblMensagem.Text = "Lote " + flockID + " já incluso na posição " + posicao.ToString() + ". Verifique!";
                    }
                    else
                    {
                        lblMensagem.Visible = false;

                        var hatcheryEggDataObject = new HATCHERY_EGG_DATA();

                        hatcheryEggDataObject.Company = "HYBR";
                        hatcheryEggDataObject.Region = "BR";
                        hatcheryEggDataObject.Location = "PP";
                        hatcheryEggDataObject.Set_date = dataIncubacao;
                        hatcheryEggDataObject.Hatch_loc = ddlIncubatorios.SelectedValue;
                        hatcheryEggDataObject.Flock_id = farmID + "-" + flockID;
                        hatcheryEggDataObject.Lay_date = layDate;
                        hatcheryEggDataObject.Eggs_rcvd = Convert.ToInt32(qtd);
                        hatcheryEggDataObject.Egg_key = "";
                        hatcheryEggDataObject.Machine = machine;
                        hatcheryEggDataObject.Track_no = trackNO;
                        hatcheryEggDataObject.Posicao = Convert.ToInt32(posicao);
                        hatcheryEggDataObject.Bandejas = Convert.ToInt32(bandejas);
                        hatcheryEggDataObject.Horario = horario;
                        hatcheryEggDataObject.Estimate = media;
                        hatcheryEggDataObject.Variety = linhagem.Replace("amp;", "");
                        hatcheryEggDataObject.Age = age;
                        hatcheryEggDataObject.Observacao = observacao;
                        hatcheryEggDataObject.Status = status;
                        hatcheryEggDataObject.Usuario = Session["usuario"].ToString();
                        hatcheryEggDataObject.Egg_key = numLote;

                        bdSQLServer.HATCHERY_EGG_DATA.AddObject(hatcheryEggDataObject);

                        if (status == "Importado")
                        {
                            // Insere na tabela da Data de Incubação
                            decimal existe = Convert.ToDecimal(setDayData.ExisteSetDayData(dataIncubacao, ddlIncubatorios.SelectedValue));

                            if (existe == 0)
                            {
                                decimal sequencia = Convert.ToDecimal(setDayData.UltimaSequenciaSetDayData(ddlIncubatorios.SelectedValue)) + 1;

                                setDayData.InsertQuery("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, sequencia);
                            }

                            existe = 0;

                            // Insere / Atualiza Incubação
                            existe = Convert.ToDecimal(hatcheryEggData.ExisteHatcheryEggDataAll("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO));

                            if (existe > 0)
                            {
                                eggUnits = eggUnits + Convert.ToDecimal(hatcheryEggData.QtdOvos("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO));
                                hatcheryEggData.Delete("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, machine, trackNO);
                            }

                            existe = 0;

                            // Verifica se existe Dados do Nascimento
                            existe = Convert.ToDecimal(hatcheryFlockData.ExisteHatcheryFlockData("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID));

                            if (existe == 0)
                            {
                                hatcheryFlockData.InsertQuery("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, media);
                            }
                            // 14/08/2014 - Ocorrência 99 - APONTES
                            // Ao calcular a idade no nascimento, ele não atualizava a mesma quando incubava mais 
                            // dados do mesmo lote. Sendo assim, cada vez que inserir, será atualizado para 
                            // o trigger de atualização da idade executar.
                            else
                            {
                                hatcheryFlockData.UpdateEstimate(media, "HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID);
                            }

                            if (eggUnits > 0)
                            {
                                hatcheryEggData.Insert("HYBR", "BR", "PP", dataIncubacao, ddlIncubatorios.SelectedValue, farmID + "-" + flockID, layDate, eggUnits, "", machine, trackNO,
                                    null, null, null, null, null, null, null, null, observacao, Session["login"].ToString());
                            }
                        }

                        bdSQLServer.SaveChanges();

                        GridView1.DataBind();
                        GridView3.DataBind();
                        gvMaquinas.DataBind();
                        gvLotes.DataBind();
                        gvLinhagens.DataBind();
                    }
                }
                AtualizaTotais();
            }
            catch (Exception ex)
            {
                lblMensagem.Visible = true;
                lblMensagem.Text = "Erro ao Incubar: " + ex.Message;
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblMensagem2.Visible = false;

                Label labelFlockID = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[4].FindControl("Label2");
                string flockID = labelFlockID.Text;

                Label labeltrackNO = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[6].FindControl("Label3");
                string trackNO = labeltrackNO.Text;

                Label labellayDate = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[8].FindControl("Label5");
                DateTime layDate = Convert.ToDateTime(labellayDate.Text);

                Label labelmachine = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[3].FindControl("Label1");
                string machine = labelmachine.Text;

                DateTime setDate = Calendar1.SelectedDate;

                Label labelposicao = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[14].FindControl("Label11");
                decimal posicao = Convert.ToDecimal(labelposicao.Text);

                Label labelqtdOvos = (Label)GridView1.Rows[GridView1.SelectedIndex].Cells[10].FindControl("Label7");
                decimal qtdOvos = Convert.ToDecimal(labelqtdOvos.Text);
                string incubatorio = ddlIncubatorios.SelectedValue;

                int posicaoFiltro = Convert.ToInt32(posicao);
                HATCHERY_EGG_DATA hatcheryEggDataObject = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Company == "HYBR" && h.Region == "BR" && h.Location == "PP" && h.Set_date == setDate
                        && h.Hatch_loc == incubatorio && h.Flock_id == flockID && h.Lay_date == layDate && h.Machine == machine
                        && h.Track_no == trackNO && h.Posicao == posicaoFiltro)
                    .First();

                bdSQLServer.DeleteObject(hatcheryEggDataObject);
                //bdSQLServer.SaveChanges();

                if (hatcheryEggDataObject.Status == "Importado")
                {
                    int existe = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == "HYBR" && h.Region == "BR" && h.Location == "PP" && h.Set_date == setDate
                            && h.Hatch_loc == ddlIncubatorios.SelectedValue && h.Flock_id == flockID && h.Lay_date == layDate && h.Machine == machine
                            && h.Track_no == trackNO)
                        .Count();

                    if (existe == 0)
                    {
                        hatcheryEggData.Delete("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue, flockID, layDate, machine, trackNO);

                        // Verifica se existe Dados do Nascimento
                        existe = 0;
                        existe = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForFlockData("HYBR",
                                        "BR", "PP", setDate,
                                        ddlIncubatorios.SelectedValue, flockID));
                        if (existe == 0)
                        {
                            hatcheryFlockData.Delete("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue, flockID);
                        }
                    }
                    else
                    {
                        qtdOvos = Convert.ToDecimal(hatcheryEggData.QtdOvos("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue, flockID, layDate, machine, trackNO)) - qtdOvos;
                        hatcheryEggData.Delete("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue, flockID, layDate, machine, trackNO);
                        hatcheryEggData.Insert("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue, flockID, layDate, qtdOvos, "", machine, trackNO,
                            null, null, null, null, null, null, null, null, null, Session["login"].ToString());
                    }
                }

                bdSQLServer.SaveChanges();

                GridView3.DataBind();
                gvMaquinas.DataBind();
                gvLotes.DataBind();
                gvLinhagens.DataBind();
                //GridView1.DataBind();

                object objeto = Button2;
                EventArgs e2 = new EventArgs();
                Button2_Click(objeto, e2);
                AtualizaTotais();
                
                lblMensagem2.Visible = true;
                lblMensagem2.Text = "Linha " + (GridView1.SelectedIndex + 1).ToString() + " excluída com sucesso!";
            }
            catch (Exception ex)
            {
                lblMensagem2.Visible = true;
                lblMensagem2.Text = "Erro ao excluir linha " + (GridView1.SelectedIndex + 1).ToString() + ": " + ex.InnerException.Message;
            }

        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            lblMensagem2.Visible = false;

            Session["setDate"] = Calendar1.SelectedDate;

            DateTime data = Calendar1.SelectedDate;

            AtualizaFLIP(data);
            AtualizaTotais();

            GridView1.DataBind();
        }

        public void AtualizaFLIP(DateTime setDate)
        {
            try
            {
                //List<HATCHERY_EGG_DATA> lista = new List<HATCHERY_EGG_DATA>();

                DateTime data = Convert.ToDateTime("01/07/2013");
                string incubatorio = ddlIncubatorios.SelectedValue;

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

                    int existeIncubacao = Convert.ToInt32(hatcheryEggData.ExisteHatcheryEggDataForSetDate("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue));

                    //if (lista.Count != existeIncubacao)
                    //{
                    // Verifica se existe mais no FLIP do que no HLBAPP. 
                    // Caso exista, serão deletados, pois no HLBAPP que é o correto.

                    if (lista.Count < existeIncubacao)
                    {
                        FLIPDataSet.HATCHERY_EGG_DATADataTable listaFLIP = hatcheryEggData.GetDataBySetDate("HYBR", "BR", "PP", setDate, ddlIncubatorios.SelectedValue);

                        foreach (var item in listaFLIP)
                        {
                            int existeHLBAPP = bdSQLServer.HATCHERY_EGG_DATA
                            .Where(h => h.Company == item.COMPANY && h.Region == item.REGION &&
                                h.Location == item.LOCATION && h.Set_date == item.SET_DATE &&
                                h.Hatch_loc == item.HATCH_LOC && h.Flock_id == item.FLOCK_ID &&
                                h.Lay_date == item.LAY_DATE && h.Machine == item.MACHINE &&
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

                            int tamanho = item.type.Flock_id.Length - 6;

                            /**** AJUSTE EGG INVENTORY PARA INCLUIR INCUBAÇÃO ****/

                            if (setDate < Convert.ToDateTime("06/02/2014"))
                            {
                                int existeAjuste = Convert.ToInt32(eggInvData.ScalarQueryOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                if (existeAjuste == 0)
                                {
                                    eggInvData.Insert(item.type.Company, item.type.Region, item.type.Location,
                                        item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                        item.type.Track_no, item.type.Lay_date, item.soma, "O", null, null, null, null, null, null, null, null,
                                        item.type.Hatch_loc, null);
                                }
                                else
                                {
                                    int qtdeOvosAjuste = Convert.ToInt32(eggInvData.QtdeOvosOpen(item.type.Flock_id, item.type.Track_no, item.type.Lay_date, ddlIncubatorios.SelectedValue));

                                    if (qtdeOvosAjuste < item.soma)
                                    {
                                        eggInvData.UpdateQueryEggs(item.soma, item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Flock_id.Substring(0, 5), item.type.Flock_id.Substring(6, tamanho),
                                            item.type.Track_no, item.type.Lay_date, "O", item.type.Hatch_loc);
                                    }
                                }
                            }
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

                                        setDayData.InsertQuery("HYBR", "BR", "PP", item.type.Set_date, item.type.Hatch_loc, sequencia);
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
                                        hatcheryFlockData.UpdateEstimate(item.estimate, item.type.Company, item.type.Region, item.type.Location,
                                            item.type.Set_date, item.type.Hatch_loc, item.type.Flock_id);
                                    }

                                    hatcheryEggData.Insert(item.type.Company, item.type.Region, item.type.Location, item.type.Set_date,
                                        item.type.Hatch_loc, item.type.Flock_id, item.type.Lay_date, item.soma, null,
                                        item.type.Machine, item.type.Track_no, null, null, null, null, null, null,
                                        null, null, item.observacao, Session["login"].ToString());
                                }
                            }
                        }
                    }
                    //}
                }
            }
            catch (Exception e)
            {

            }
        }

        protected void btn_AtualizaSetter_Click(object sender, EventArgs e)
        {
            if ((txt_SetterDe.Text != txt_SetterPara.Text) &&
                (txt_SetterDe.Text != string.Empty) &&
                (txt_SetterPara.Text != string.Empty))
            {
                hatcheryEggData.AtualizaSetter(txt_SetterPara.Text, Calendar1.SelectedDate, txt_SetterDe.Text);

                var listaAtualiza = bdSQLServer.HATCHERY_EGG_DATA
                    .Where(h => h.Set_date == Calendar1.SelectedDate && h.Machine == txt_SetterDe.Text)
                    .ToList();

                foreach (var item in listaAtualiza)
                {
                    item.Machine = txt_SetterPara.Text;
                }

                bdSQLServer.SaveChanges();

                GridView1.DataBind();
            }
        }

        protected void EGG_UNITSTextBox_TextChanged(object sender, EventArgs e)
        {
            if (FormView1.CurrentMode == FormViewMode.Edit)
            {
                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                if (EGG_UNITSTextBox.Text == "") { EGG_UNITSTextBox.Text = "0"; }
                decimal eggUnits = Convert.ToDecimal(EGG_UNITSTextBox.Text);

                // Verifica se a quantidade equivale a produzida e a já incubada.
                decimal qtdeProduzida = 0;
                int? qtdeIncubada = 0;

                if (tipoCadastro.Equals("Estoque Futuro"))
                {
                    DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                    DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                    Calendar clDataProducao = (Calendar)FormView1.FindControl("Lay_DateCalendar");

                    if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                    {
                        clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    }

                    //flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP", farm.SelectedValue, lote.SelectedValue, clDataProducao.SelectedDate);

                    //if (flipDataSet.FLOCK_DATA.Count > 0)
                    //    qtdeProduzida = flipDataSet.FLOCK_DATA[0].NUM_1;

                    qtdeProduzida = qtde;

                    string loteCompleto = farm.SelectedValue + "-" + lote.SelectedValue;
                    DateTime dataProducao = clDataProducao.SelectedDate;

                    qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company.Equals("HYBR") && h.Region.Equals("BR") && h.Location.Equals("PP") &&
                                    h.Flock_id.Equals(loteCompleto) && h.Lay_date.Equals(dataProducao))
                        .Sum(h => h.Eggs_rcvd);

                    if (qtdeIncubada == null)
                        qtdeIncubada = 0;

                    //qtde = Convert.ToInt32(qtdeProduzida);
                }
                else
                {
                    string farmID = "";
                    string flockID = "";
                    DateTime layDate;

                    Label FARM_IDLabel1 = (Label)FormView1.FindControl("FARM_IDLabel1");
                    farmID = FARM_IDLabel1.Text;
                    Label FLOCK_IDLabel1 = (Label)FormView1.FindControl("FLOCK_IDLabel1");
                    flockID = FLOCK_IDLabel1.Text;
                    Label LAY_DATELabel1 = (Label)FormView1.FindControl("LAY_DATELabel1");
                    layDate = Convert.ToDateTime(LAY_DATELabel1.Text);

                    flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP", farmID, flockID, layDate);

                    if (flipDataSet.FLOCK_DATA.Count > 0)
                        qtdeProduzida = flipDataSet.FLOCK_DATA[0].NUM_1;

                    string loteCompleto = farmID + "-" + flockID;

                    qtdeIncubada = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company.Equals("HYBR") && h.Region.Equals("BR") && h.Location.Equals("PP") &&
                                    h.Flock_id.Equals(loteCompleto) && h.Lay_date.Equals(layDate))
                        .Sum(h => h.Eggs_rcvd);

                    if (qtdeIncubada == null)
                        qtdeIncubada = 0;
                }

                if (qtde < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = "Quantidade maior que disponível! - (" + qtde.ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else if ((qtdeProduzida - qtdeIncubada) < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = "Quantidade maior que disponível do Lote nesta Data de Produção! - (Qtde. Produzida: " + qtdeProduzida.ToString() + " / Qtde. já Incubada: " + qtdeIncubada.ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else
                {
                    lblMensagem.Visible = false;
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            try
            {
                lblMensagem.Visible = false;
                HatchFormDataSource.SelectParameters["FLOCK_ID"].DefaultValue = GridView3.Rows[0].Cells[2].Text;
                HatchFormDataSource.SelectParameters["TRACK_NO"].DefaultValue = GridView3.Rows[0].Cells[4].Text;
                HatchFormDataSource.SelectParameters["LAY_DATE"].DefaultValue = GridView3.Rows[0].Cells[6].Text;
                linhagem = GridView3.Rows[0].Cells[5].Text;

                string farmid = GridView3.Rows[0].Cells[1].Text;
                string flockid = GridView3.Rows[0].Cells[2].Text;
                DateTime layDate = Convert.ToDateTime(GridView3.Rows[0].Cells[6].Text);

                flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP",
                    farmid, flockid, layDate);

                age = Convert.ToInt32(flipDataSet.FLOCK_DATA[0].AGE);
                qtde = Convert.ToInt32(GridView3.Rows[0].Cells[8].Text.Replace(".", ""));

                lblMensagem.Visible = false;
                tipoCadastro = "Estoque Futuro";
                FormView1.ChangeMode(FormViewMode.Edit);

                lblMensagem2.Visible = false;

                //AtualizaIdadesLinhagens();
            }
            catch (Exception ex)
            {
                lblMensagem.Visible = true;
                lblMensagem.Text = "ERRO: " + ex.Message;
            }
        }

        protected void Lay_DateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            Calendar Calendario = (Calendar)FormView1.FindControl("Lay_DateCalendar");
            if (Calendario != null)
            {
                Label TRACK_NOLabel2 = (Label)FormView1.FindControl("TRACK_NOLabel1");
                if (TRACK_NOLabel2 != null)
                {
                    TRACK_NOLabel2.Visible = true;
                    TRACK_NOLabel2.Text = "EXP" + Calendario.SelectedDate.ToString("yyMMdd");
                }

                //age = ((Calendario.SelectedDate - dataNascimentoLote).Days) / 7;

                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");

                flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", farm.SelectedValue, lote.SelectedValue);
                if (flipDataSet.FLOCKS.Count > 0)
                {
                    dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
                    age = ((Calendario.SelectedDate - dataNascimentoLote).Days) / 7;
                    linhagem = flipDataSet.FLOCKS[0].VARIETY;
                }

                if (tipoCadastro.Equals("Estoque Futuro"))
                {
                    flockData.FillFlockData(flipDataSet.FLOCK_DATA, "HYBR", "BR", "PP", farm.SelectedValue, lote.SelectedValue, Calendario.SelectedDate);

                    if (flipDataSet.FLOCK_DATA.Count > 0)
                    {
                        qtde = Convert.ToInt32(flipDataSet.FLOCK_DATA[0].NUM_1);
                        TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                        EGG_UNITSTextBox.Text = qtde.ToString();
                    }
                    else
                    {
                        DateTime ultimaProducao = Convert.ToDateTime(flockData.UltimaProducaoPorLote("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue));

                        int eggUnits = Convert.ToInt32(flockData.QtdeOvosIncubaveis("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue,
                                    ultimaProducao));

                        qtde = eggUnits;

                        TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                        EGG_UNITSTextBox.Text = eggUnits.ToString();
                    }
                }
            }
            lblMensagem2.Visible = false;
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
                decimal bandejas = (qtdeOvosIncubados / 150);
                lblQtdeOvosIncubados.Text = string.Format("{0:N0}", qtdeOvosIncubados) + " ovos";
                lblQtdeOvosIncubadosCx.Text = string.Format("{0:N0}", Decimal.Round(bandejas, 0)) + " bandejas";
            }
            
            lblMaquinas.Text = maquinasUtilizadas;
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            AtualizaTotais();
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
            LinkButton1.Visible = false;
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
            LinkButton1.Visible = true;
            FormView1.Visible = true;
            lblMensagem.Visible = true;
        }

        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((FormView1.CurrentMode == FormViewMode.Edit) && (tipoCadastro == "Estoque Futuro"))
            {
                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                Calendar clDataProducao = (Calendar)FormView1.FindControl("Lay_DateCalendar");

                if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                {
                    clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }

                DateTime ultimaProducao = Convert.ToDateTime(flockData.UltimaProducaoPorLote("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue));

                int eggUnits = Convert.ToInt32(flockData.QtdeOvosIncubaveis("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue,
                                    ultimaProducao));

                qtde = eggUnits;

                flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", farm.SelectedValue, lote.SelectedValue);
                if (flipDataSet.FLOCKS.Count > 0)
                {
                    dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
                    age = ((clDataProducao.SelectedDate - dataNascimentoLote).Days) / 7;
                    linhagem = flipDataSet.FLOCKS[0].VARIETY;
                }

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                EGG_UNITSTextBox.Text = eggUnits.ToString();

                if (qtde < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = "Quantidade maior que disponível! - (" + qtde.ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else
                {
                    lblMensagem.Visible = false;
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }

                DateTime ultimaIncubacao = Convert.ToDateTime(hatcheryFlockData.UltimaIncubacao("HYBR",
                                    "BR", "PP", ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

                decimal ultimaPercEclosao = Convert.ToDecimal(hatcheryFlockData.UltimaPercEclosao("HYBR",
                                    "BR", "PP", ultimaIncubacao, ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

                TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                MediaEclosaoTextBox.Text = ultimaPercEclosao.ToString();
            }
        }

        protected void DropDownList3_DataBound(object sender, EventArgs e)
        {
            if ((FormView1.CurrentMode == FormViewMode.Edit) && (tipoCadastro == "Estoque Futuro"))
            {
                DropDownList lote = (DropDownList)FormView1.FindControl("DropDownList3");
                DropDownList farm = (DropDownList)FormView1.FindControl("DropDownList4");
                Calendar clDataProducao = (Calendar)FormView1.FindControl("Lay_DateCalendar");

                if (clDataProducao.SelectedDate.ToShortDateString() == "01/01/0001")
                {
                    clDataProducao.SelectedDate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                }

                DateTime ultimaProducao = Convert.ToDateTime(flockData.UltimaProducaoPorLote("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue));

                int eggUnits = Convert.ToInt32(flockData.QtdeOvosIncubaveis("HYBR",
                                    "BR", "PP", farm.SelectedValue, lote.SelectedValue,
                                    ultimaProducao));

                flocks.FillBy(flipDataSet.FLOCKS, "HYBR", "BR", "PP", farm.SelectedValue, lote.SelectedValue);
                if (flipDataSet.FLOCKS.Count > 0)
                {
                    dataNascimentoLote = flipDataSet.FLOCKS[0].HATCH_DATE;
                    age = ((clDataProducao.SelectedDate - dataNascimentoLote).Days) / 7;
                    linhagem = flipDataSet.FLOCKS[0].VARIETY;
                }
                qtde = eggUnits;

                TextBox EGG_UNITSTextBox = (TextBox)FormView1.FindControl("EGG_UNITSTextBox");
                EGG_UNITSTextBox.Text = eggUnits.ToString();

                if (qtde < eggUnits)
                {
                    lblMensagem.Visible = true;
                    lblMensagem.Text = "Quantidade maior que disponível! - (" + qtde.ToString() + ")";
                    EGG_UNITSTextBox.Focus();
                }
                else
                {
                    lblMensagem.Visible = false;
                    ((TextBox)FormView1.FindControl("BandejasTextBox")).Text = (Decimal.Round(Convert.ToDecimal(Convert.ToDouble(EGG_UNITSTextBox.Text) / 150.0), 1)).ToString();
                }

                DateTime ultimaIncubacao = Convert.ToDateTime(hatcheryFlockData.UltimaIncubacao("HYBR",
                                    "BR", "PP", ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

                decimal ultimaPercEclosao = Convert.ToDecimal(hatcheryFlockData.UltimaPercEclosao("HYBR",
                                    "BR", "PP", ultimaIncubacao, ddlIncubatorios.SelectedValue, farm.SelectedValue + "-" + lote.SelectedValue));

                TextBox MediaEclosaoTextBox = (TextBox)FormView1.FindControl("MediaEclosaoTextBox");
                MediaEclosaoTextBox.Text = ultimaPercEclosao.ToString();
            }
        }

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

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            lblMensagem2.Visible = false;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            int index = GridView1.EditIndex;
            int id = 0;
            try
            {
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

                incubacao.Horario = horario;
                incubacao.Estimate = eclosao;
                incubacao.Observacao = obs;

                string trackNO = "EXP" + dataPrd.ToString("yyMMdd");

                if (incubacao.Status == "Importado")
                {
                    hatcheryFlockData.UpdateEstimate(eclosao, incubacao.Company, incubacao.Region, incubacao.Location,
                        incubacao.Set_date, incubacao.Hatch_loc, incubacao.Flock_id);

                    hatcheryEggData.Delete(incubacao.Company, incubacao.Region, incubacao.Location,
                        incubacao.Set_date, incubacao.Hatch_loc, incubacao.Flock_id, incubacao.Lay_date, incubacao.Machine,
                        incubacao.Track_no);

                    int? qtdIncubada = bdSQLServer.HATCHERY_EGG_DATA
                        .Where(h => h.Company == incubacao.Company && h.Region == incubacao.Region &&
                            h.Location == incubacao.Location && h.Set_date == incubacao.Set_date &&
                            h.Hatch_loc == incubacao.Hatch_loc && h.Flock_id == incubacao.Flock_id &&
                            h.Lay_date == incubacao.Lay_date && h.Machine == incubacao.Machine &&
                            h.Track_no == incubacao.Track_no)
                        .Sum(h => h.Eggs_rcvd);

                    hatcheryEggData.Insert(incubacao.Company, incubacao.Region, incubacao.Location,
                        incubacao.Set_date, incubacao.Hatch_loc, incubacao.Flock_id, dataPrd, qtdIncubada,
                        incubacao.Egg_key, incubacao.Machine, trackNO, null, null, null, null, null, null, null,
                        null, null, Session["login"].ToString());
                }

                //hatcheryEggData.UpdateLayDate(dataPrd, trackNO, incubacao.Company, incubacao.Region, incubacao.Location,
                //    incubacao.Set_date, incubacao.Hatch_loc, incubacao.Flock_id, incubacao.Lay_date, incubacao.Machine,
                //    incubacao.Track_no);

                incubacao.Lay_date = dataPrd;
                incubacao.Track_no = trackNO;

                bdSQLServer.SaveChanges();

                GridView1.Rows[index].RowState = DataControlRowState.Normal;
                lblMensagem2.Visible = true;
                lblMensagem2.Text = "Linha " + (id).ToString() + " alterada com sucesso!";
            }
            catch (Exception ex)
            {
                GridView1.Rows[index].RowState = DataControlRowState.Normal;
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
            }
        }

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
                lblMensagem2.Text = "Lote " + incubacao.Flock_id + " com a Data de Produção " + 
                    dataPrd.ToShortDateString() + " na posição " + incubacao.Posicao.ToString() + " já existe!";
                txtDataPrd.Focus();
            }
            else
            {
                lblMensagem2.Visible = false;
            }
        }

        protected void ddlIncubatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime data = Calendar1.SelectedDate;

            AtualizaFLIP(data);
        }
    }
}