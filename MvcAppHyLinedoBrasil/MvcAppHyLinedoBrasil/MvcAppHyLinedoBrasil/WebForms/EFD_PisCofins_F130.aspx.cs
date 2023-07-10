using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.EntityWebForms.PAT_BEM;
using MvcAppHyLinedoBrasil.Models;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class EFD_PisCofins_F130 : System.Web.UI.Page
    {
        Apolo10Entities1 bdApolo = new Apolo10Entities1();

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

        protected void Button1_Click1(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                string empresa = row.Cells[2].Text;
                string codReduzido = row.Cells[3].Text;

                CheckBox ch = (CheckBox)row.FindControl("CheckBox1");
                if (ch != null)
                {
                    if (ch.Checked)
                    {
                        var item = bdApolo.PAT_BEM.First(p => p.EmpCod == empresa &&
                                                              p.PatBemCodRed == codReduzido);

                        if (item != null)
                        {
                            item.PatBemAnoMesInicEfdPisCofins = TextBox8.Text;
                            item.PatBemAnoMesFimEfdPisCofins = TextBox9.Text;
                        }

                        bdApolo.SaveChanges();
                    }
                }
            }

            Imobilizado.DataBind();
            GridView1.DataBind();
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in GridView1.Rows)
            {
                CheckBox ch = (CheckBox)row.FindControl("CheckBox1");
                if (ch != null)
                {
                    ch.Checked = (sender as CheckBox).Checked;
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            GridView1.DataBind();
            Panel2.Visible = true;
        }

        protected void btn_Nova_Pesquisa_Click(object sender, EventArgs e)
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
        }
    }
}