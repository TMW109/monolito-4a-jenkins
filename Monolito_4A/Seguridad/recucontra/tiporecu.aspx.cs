using System;
using System.Web.UI;

namespace Monolito_4A.Seguridad.recucontra
{
    public partial class tiporecu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        }

        protected void btnCorreo_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Seguridad/recucontra/RecuperarPassword.aspx");
        }

        protected void btnWhatsapp_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Seguridad/recucontra/RecuperarWhatsapp.aspx");
        }

        protected void btnGreen_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Seguridad/recucontra/green.aspx");
        }
    }
}