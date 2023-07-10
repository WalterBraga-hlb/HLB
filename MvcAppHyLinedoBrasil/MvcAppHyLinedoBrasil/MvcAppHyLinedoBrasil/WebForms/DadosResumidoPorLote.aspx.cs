using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MvcAppHyLinedoBrasil.WebForms
{
    public partial class DadosResumidoPorLote : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                Image2.ImageUrl = "../Content/images/Logo_" + Session["logo"].ToString() + ".png";
            }
        }
    }
}