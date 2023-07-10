using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MvcAppHyLinedoBrasil.Controllers;
using MvcAppHyLinedoBrasil.Models;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class RelatoriosFLIP : System.Web.UI.Page
    {
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

            //Image1.ImageUrl = "~/Content/images/Logo_" + Session["empresaLayout"].ToString() + ".png";

            if (!IsPostBack)
            {
                Label5.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("Title_Menu_Reports_FLIP_WebDesktop",
                        Session["language"].ToString());

                Image1.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";

                hlBackHome.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                    .GetTextOnLanguage("HL_Back_To_Home", Session["language"].ToString());

                if (!AccountController.GetGroup("HLBAPP-RelDiarioCompleto", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelDiarioCompleto.Visible = false;
                    imgRelDiarioCompleto.Visible = false;
                }
                else
                {
                    hplRelDiarioCompleto.Text = MvcAppHyLinedoBrasil.Controllers.AccountController
                        .GetTextOnLanguage("Title_Report_Rel_Diario_Completo_WebDesktop", 
                            Session["language"].ToString());
                    hplRelDiarioCompleto.Visible = true;
                    imgRelDiarioCompleto.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-ControleQualidadeCargas", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    imgControleQualidadeCargas.Visible = false;
                    hplControleQualidadeCargas.Visible = false;
                }
                else
                {
                    imgControleQualidadeCargas.Visible = true;
                    hplControleQualidadeCargas.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelEggInv", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelEggInv.Visible = false;
                    imgRelEggInv.Visible = false;
                }
                else
                {
                    hplRelEggInv.Visible = true;
                    imgRelEggInv.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelConfIncWebXFlip", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelConfIncWebXFlip.Visible = false;
                    imgRelConfIncWebXFlip.Visible = false;
                }
                else
                {
                    hplRelConfIncWebXFlip.Visible = true;
                    imgRelConfIncWebXFlip.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelDEOGranja", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelDEOGranja.Visible = false;
                    imgRelDEOGranja.Visible = false;
                }
                else
                {
                    hplRelDEOGranja.Visible = true;
                    imgRelDEOGranja.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelNascEmbrio", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelNascEmbrio.Visible = false;
                    imgRelNascEmbrio.Visible = false;
                }
                else
                {
                    hplRelNascEmbrio.Visible = true;
                    imgRelNascEmbrio.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelEstoqueOvos", 
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelEstoqueOvos.Visible = false;
                    imgRelEstoqueOvos.Visible = false;
                }
                else
                {
                    hplRelEstoqueOvos.Visible = true;
                    imgRelEstoqueOvos.Visible = true;
                }

                if (!AccountController.GetGroup("HLBAPP-RelRastreabilidade",
                    (System.Collections.ArrayList)Session["Direitos"]))
                {
                    hplRelRastreabilidade.Visible = false;
                    imgRelRastreabilidade.Visible = false;
                }
                else
                {
                    hplRelRastreabilidade.Visible = true;
                    imgRelRastreabilidade.Visible = true;
                }
            }
        }
    }
}