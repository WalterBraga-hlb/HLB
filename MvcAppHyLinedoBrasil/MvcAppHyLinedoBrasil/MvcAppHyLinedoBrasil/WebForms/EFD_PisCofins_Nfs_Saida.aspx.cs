using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.EntityWebForms.FISCAL_ITEM_NF;
using MvcAppHyLinedoBrasil.Models;
using MvcAppHyLinedoBrasil.EntityWebForms.HATCHERY_EGG_DATA;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class AtualizaNF_EFD_PisCofins : System.Web.UI.Page
    {
        Apolo10Entities bdApolo = new Apolo10Entities();
        FinanceiroEntities bdApolo2 = new FinanceiroEntities();

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
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            GridView1.DataSourceID = "FiscalNFs";
            Panel1.Visible = false;
            GridView1.DataBind();            
            Panel2.Visible = true;
            Label6.Text = "BLOCOS C / D";
        }

        protected void btn_Nova_Pesquisa_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            //GridView1.DataBind();
            Panel2.Visible = false;
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btn_Pesquisar_F130_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            btn_Nova_Pesquisa.Visible = true;
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                CheckBox ch = (CheckBox)row.FindControl("CheckBox2");
                if (ch != null)
                {
                    ch.Checked = (sender as CheckBox).Checked;
                }
            }
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                string empresa = row.Cells[2].Text;
                int chave = Convert.ToInt32(row.Cells[3].Text);
                int seq = Convert.ToInt32(row.Cells[4].Text);

                CheckBox ch = (CheckBox)row.FindControl("CheckBox2");
                if (ch != null)
                {
                    if (ch.Checked)
                    {
                        var item = bdApolo.FISCAL_ITEM_NF.First(f => f.EmpCod == empresa &&
                                                                     f.FiscalNFChv == chave &&
                                                                     f.FiscalItNFSeq == seq);

                        if (item != null)
                        {
                            if (DropDownList3.SelectedValue != "00")
                                item.FiscalItNFConfTribCofinsCod = DropDownList3.SelectedValue;
                            else
                                item.FiscalItNFConfTribCofinsCod = null;
                            if (DropDownList4.SelectedValue != "00")
                                item.FiscalItNFConfTribPisCod = DropDownList4.SelectedValue;
                            else
                                item.FiscalItNFConfTribPisCod = null;
                            if (DropDownList6.SelectedValue != "00")
                                item.EFDBaseCalcCredCod = DropDownList6.SelectedValue;
                            else
                                item.EFDBaseCalcCredCod = null;
                        }

                        bdApolo.SaveChanges();
                    }
                }
            }

            FiscalNFs.DataBind();
            GridView1.DataBind();
        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
             
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                lblMensagem3.Visible = false;
                lblMensagem3.Text = "";
                Apolo10Entities bdApoloSession = new Apolo10Entities();

                GridViewRow row = GridView1.Rows[e.RowIndex];

                string empresa = row.Cells[2].Text;
                int chave = Convert.ToInt32(row.Cells[3].Text);
                int seq = Convert.ToInt32(row.Cells[4].Text);

                //var item = bdApoloSession.FISCAL_ITEM_NF.First(f => f.EmpCod == empresa &&
                //                f.FiscalNFChv == chave &&
                //                f.FiscalItNFSeq == seq);

                var item = bdApoloSession.FISCAL_ITEM_NF.Where(f => f.EmpCod == empresa &&
                                f.FiscalNFChv == chave &&
                                f.FiscalItNFSeq == seq).FirstOrDefault();

                TextBox ch = (TextBox)row.FindControl("TextBox4");
                TextBox CSTPis = (TextBox)row.FindControl("TextBox2");
                TextBox CSTCofins = (TextBox)row.FindControl("TextBox1");
                TextBox tipoEFD = (TextBox)row.FindControl("TextBox3");

                TextBox fiscalItNFValBasePis = (TextBox)row.FindControl("TextBox5");
                if (fiscalItNFValBasePis.Text != "") item.FiscalItNFValBasePis = Convert.ToDecimal(fiscalItNFValBasePis.Text);
                TextBox fiscalItNFAliqPis = (TextBox)row.FindControl("TextBox6");
                if (fiscalItNFAliqPis.Text != "") item.FiscalItNFAliqPis = Convert.ToDecimal(fiscalItNFAliqPis.Text);
                TextBox fiscalItNFValPis = (TextBox)row.FindControl("TextBox7");
                if (fiscalItNFValPis.Text != "") item.FiscalItNFValPis = Convert.ToDecimal(fiscalItNFValPis.Text);
                TextBox fiscalItNFValPisRec = (TextBox)row.FindControl("TextBox8");
                if (fiscalItNFValPisRec.Text != "") item.FiscalItNFValPisRec = Convert.ToDecimal(fiscalItNFValPisRec.Text);

                TextBox fiscalItNFValBaseCofins = (TextBox)row.FindControl("TextBox9");
                if (fiscalItNFValBaseCofins.Text != "") item.FiscalItNFValBaseCofins = Convert.ToDecimal(fiscalItNFValBaseCofins.Text);
                TextBox fiscalItNFAliqCofins = (TextBox)row.FindControl("TextBox10");
                if (fiscalItNFAliqCofins.Text != "") item.FiscalItNFAliqCofins = Convert.ToDecimal(fiscalItNFAliqCofins.Text);
                TextBox fiscalItNFValCofins = (TextBox)row.FindControl("TextBox11");
                if (fiscalItNFValCofins.Text != "") item.FiscalItNFValCofins = Convert.ToDecimal(fiscalItNFValCofins.Text);
                TextBox fiscalItNFValCofinsRec = (TextBox)row.FindControl("TextBox12");
                if (fiscalItNFValCofinsRec.Text != "") item.FiscalItNFValCofinsRec = Convert.ToDecimal(fiscalItNFValCofinsRec.Text);

                item.FiscalItNFNatOpCodEstr = ch.Text.Substring(0, 5);

                if (CSTCofins.Text != "")
                {
                    if (CSTCofins.Text != "00" && CSTCofins.Text != "00")
                        item.FiscalItNFConfTribCofinsCod = CSTCofins.Text;
                    else
                        item.FiscalItNFConfTribCofinsCod = "00";
                }

                if (CSTPis.Text != "")
                {
                    if (CSTPis.Text != "00" && CSTPis.Text != "")
                        item.FiscalItNFConfTribPisCod = CSTPis.Text;
                    else
                        item.FiscalItNFConfTribPisCod = "00";
                }

                if (tipoEFD.Text != "")
                {
                    if (tipoEFD.Text != "00" && tipoEFD.Text != "")
                        item.EFDBaseCalcCredCod = tipoEFD.Text;
                    else
                        item.EFDBaseCalcCredCod = "00";
                }

                bdApoloSession.SaveChanges();

                //AtualizaFISCALNFeOrigem(item.EmpCod, item.FiscalNFChv, item.FiscalItNFSeq, item.ProdCodEstr, item.FiscalItNFIndServ);
                AtualizaFISCALNFeOrigem(item.EmpCod, item.FiscalNFChv);
            }
            catch (Exception ex)
            {
                GridView1.Rows[e.RowIndex].RowState = DataControlRowState.Normal;
                int linenum = Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

                lblMensagem3.Visible = true;
                lblMensagem3.Text = "Erro ao salvar alteração: ";
                if (ex.InnerException == null)
                    lblMensagem3.Text = lblMensagem3.Text + ex.Message;
                else
                    lblMensagem3.Text = lblMensagem3.Text + ex.Message + " / " + ex.InnerException.Message;

                lblMensagem3.Text = (char)10 + (char)13 + "Nº da Linha de Código do Erro (passar para o depto. de TI): " + linenum.ToString();
            }
        }

        public void AtualizaFISCALNFeOrigem(string empCod, int fiscalNFChv)
        {
            #region Carrega BD

            Apolo10Entities bdApoloSession = new Apolo10Entities();
            FinanceiroEntities bdApolo2Session = new FinanceiroEntities();

            #endregion

            #region Atualiza FISCAL_NF

            FISCAL_NF nf = bdApoloSession.FISCAL_NF.Where(w => w.EmpCod == empCod && w.FiscalNFChv == fiscalNFChv).FirstOrDefault();

            var listaItens = bdApoloSession.FISCAL_ITEM_NF
                .Where(w => w.EmpCod == empCod && w.FiscalNFChv == fiscalNFChv)
                .ToList();

            decimal valBasePis = 0;
            decimal valPis = 0;
            decimal valPisRec = 0;

            decimal valBaseCofins = 0;
            decimal valCofins = 0;
            decimal valCofinsRec = 0;
            string servico = "Não";
            foreach (var item in listaItens)
            {
                valBasePis += Convert.ToDecimal(item.FiscalItNFValBasePis);
                valPis += Convert.ToDecimal(item.FiscalItNFValPis);
                valPisRec += Convert.ToDecimal(item.FiscalItNFValPisRec);

                valBaseCofins += Convert.ToDecimal(item.FiscalItNFValBaseCofins);
                valCofins += Convert.ToDecimal(item.FiscalItNFValCofins);
                valCofinsRec += Convert.ToDecimal(item.FiscalItNFValCofinsRec);

                servico = item.FiscalItNFIndServ;
            }

            if (servico == "Sim")
            {
                nf.FISCALNFVALBASEPISSERV = valBasePis;
                nf.FISCALNFVALPISSERV = valPis;

                nf.FISCALNFVALBASECOFINSSERV = valBaseCofins;
                nf.FISCALNFVALCOFINSSERV = valCofins;
            }
            else
            {
                nf.FiscalNFValBasePis = valBasePis;
                nf.FiscalNFValPis = valPis;

                nf.FiscalNFValBaseCofins = valBaseCofins;
                nf.FiscalNFValCofins = valCofins;
            }

            #endregion

            #region Atualiza ORIGEM

            #region Carrega os dados de Origem

            string empOrig = nf.FiscalNFOrigEmpCod;
            int chaveOrig = Convert.ToInt32(nf.FiscalNFOrigChv);
            string origem = nf.FiscalNFOrigDoc;

            #endregion

            if (origem == "Estoque")
            {
                #region Carrega MOV_ESTQ

                MOV_ESTQ movEstq = bdApolo2Session.MOV_ESTQ
                    .Where(w => w.EmpCod == empOrig && w.MovEstqChv == chaveOrig)
                    .FirstOrDefault();

                var listaConfMovEstqVal = new List<CONF_MOV_ESTQ_VALORES>();

                #endregion

                #region Atualiza ITEM_MOV_ESTQ

                foreach (var item in listaItens)
                {
                    ITEM_MOV_ESTQ ime = bdApolo2Session.ITEM_MOV_ESTQ
                        .Where(w => w.EmpCod == empOrig && w.MovEstqChv == chaveOrig
                            && w.ItMovEstqSeq == item.FiscalItNFOrigSeq)
                        .FirstOrDefault();

                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValBasePis", ime.ItMovEstqValBasePis.ToString(), item.FiscalItNFValBasePis.ToString()));
                    ime.ItMovEstqValBasePis = Convert.ToDecimal(item.FiscalItNFValBasePis);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqPercPis", ime.ItMovEstqPercPis.ToString(), item.FiscalItNFAliqPis.ToString()));
                    ime.ItMovEstqPercPis = Convert.ToDecimal(item.FiscalItNFAliqPis);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValPis", ime.ItMovEstqValPis.ToString(), item.FiscalItNFValPis.ToString()));
                    ime.ItMovEstqValPis = Convert.ToDecimal(item.FiscalItNFValPis);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValPisRec", ime.ItMovEstqValPisRec.ToString(), item.FiscalItNFValPisRec.ToString()));
                    ime.ItMovEstqValPisRec = Convert.ToDecimal(item.FiscalItNFValPisRec);

                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValBaseCofins", ime.ItMovEstqValBaseCofins.ToString(), item.FiscalItNFValBaseCofins.ToString()));
                    ime.ItMovEstqValBaseCofins = Convert.ToDecimal(item.FiscalItNFValBaseCofins);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqPercCofins", ime.ItMovEstqPercCofins.ToString(), item.FiscalItNFAliqCofins.ToString()));
                    ime.ItMovEstqPercCofins = Convert.ToDecimal(item.FiscalItNFAliqCofins);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValCofins", ime.ItMovEstqValCofins.ToString(), item.FiscalItNFValCofins.ToString()));
                    ime.ItMovEstqValCofins = Convert.ToDecimal(item.FiscalItNFValCofins);
                    listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("ITEM_MOV_ESTQ", ime.ItMovEstqSeq, ime.ProdCodEstr, "Alteração", "ItMovEstqValCofinsRec", ime.ItMovEstqValCofinsRec.ToString(), item.FiscalItNFValCofinsRec.ToString()));
                    ime.ItMovEstqValCofinsRec = Convert.ToDecimal(item.FiscalItNFValCofinsRec);
                }

                #endregion

                #region Atualiza MOV_ESTQ

                listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("MOVE_ESTQ", 0, "", "Alteração", "MovEstqValBasePis", movEstq.MovEstqValBasePis.ToString(), valBasePis.ToString()));
                movEstq.MovEstqValBasePis = valBasePis;
                listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("MOVE_ESTQ", 0, "", "Alteração", "MovEstqValPis", movEstq.MovEstqValPis.ToString(), valPis.ToString()));
                movEstq.MovEstqValPis = valPis;

                listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("MOVE_ESTQ", 0, "", "Alteração", "MovEstqValBaseCofins", movEstq.MovEstqValBaseCofins.ToString(), valBaseCofins.ToString()));
                movEstq.MovEstqValBaseCofins = valBaseCofins;
                listaConfMovEstqVal.Add(GeraCONFMOVESTQVALORES("MOVE_ESTQ", 0, "", "Alteração", "MovEstqValCofins", movEstq.MovEstqValCofins.ToString(), valCofins.ToString()));
                movEstq.MovEstqValCofins = valCofins;

                #endregion

                #region Gera LOG de Conferência

                HLBAPPEntities hlbappSession = new HLBAPPEntities();

                CONF_MOV_ESTQ confME = new CONF_MOV_ESTQ();
                confME.EmpCod = movEstq.EmpCod;
                confME.MovEstqChv = movEstq.MovEstqChv;
                confME.DataHoraConferencia = DateTime.Now;
                confME.UsuarioConferencia = Session["login"].ToString();
                confME.Observacao = "Alteração dos valores via módulo de AJUSTE DE NFS - EFD PIS COFINS.";
                hlbappSession.CONF_MOV_ESTQ.AddObject(confME);
                hlbappSession.SaveChanges();

                foreach (var item in listaConfMovEstqVal)
                {
                    item.IDConfItemMovEstq = confME.ID;
                    hlbappSession.CONF_MOV_ESTQ_VALORES.AddObject(item);
                }

                hlbappSession.SaveChanges();

                #endregion
            }

            #endregion

            

            bdApoloSession.SaveChanges();
        }

        public CONF_MOV_ESTQ_VALORES GeraCONFMOVESTQVALORES(string tabela, int seq, string prodCodEstr,
            string operacao, string campo, string valorAntigo, string valorNovo)
        {
            var confMEV = new CONF_MOV_ESTQ_VALORES();
            confMEV.Tabela = tabela;
            confMEV.ItMovEstqSeq = seq;
            confMEV.ProdCodEstr = prodCodEstr;
            confMEV.Operacao = operacao;
            confMEV.Campo = campo;
            confMEV.ValorAntigo = valorAntigo;
            confMEV.ValorNovo = valorNovo;

            return confMEV;
        }

        protected void btn_Pesquisar_C_D0_Click(object sender, EventArgs e)
        {
            GridView1.DataSourceID = "FiscalNFsGeral";
            GridView1.DataBind();
            Panel1.Visible = false;
            GridView1.DataBind();
            Panel2.Visible = true;
            Label6.Text = "OUTRAS NOTAS FISCAIS";
        }
    }
}