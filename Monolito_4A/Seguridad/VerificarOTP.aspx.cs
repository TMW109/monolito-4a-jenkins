using System;
using System.Web;
using System.Web.UI;
using Capa_Datos;
using Capa_Negocio;

namespace Monolito_4A
{
    public partial class VerificarOTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            if (Session["cedula_2fa"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
            }
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            if (Session["cedula_2fa"] == null)
            {
                MostrarError();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCodigoOTP.Value))
            {
                MostrarError();
                return;
            }

            string cedula = Session["cedula_2fa"].ToString();

            tbl_usuario usuario = CN_tbl_usuario.ObtenerUsuarioPorCedulaTodos(cedula);
            if (usuario == null)
            {
                MostrarError();
                return;
            }

            string codigoIngresado = LimpiarCodigo(txtCodigoOTP.Value);
            string codigoBD = LimpiarCodigo(usuario.usu_codigo_OPT);

            if (codigoIngresado == codigoBD)
            {

                CN_tbl_usuario.LimpiarOTP(usuario.usu_cedula);
                Session["usuario"] = usuario.usu_nick;
                Session["cedula"] = usuario.usu_cedula;
                Session["tusu_id"] = usuario.tusu_id;

                Session.Remove("cedula_2fa");

                if (usuario.tusu_id == 3)
                {
                    Response.Redirect("~/admin.aspx");
                }
                else if (usuario.tusu_id == 4)
                {
                    Response.Redirect("~/Usu.aspx");
                }
                else
                {
                    Response.Redirect("~/Seguridad/Login.aspx");
                }
            }
            else
            {
                MostrarError();
            }
        }

        private string LimpiarCodigo(string codigo)
        {
            if (codigo == null)
                return "";

            codigo = HttpUtility.UrlDecode(codigo);

            return codigo
                .Trim()
                .Replace(" ", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "");
        }

        private void MostrarError()
        {
            ScriptManager.RegisterStartupScript(
                UpdatePanel1,
                UpdatePanel1.GetType(),
                Guid.NewGuid().ToString(),
                "mostrarModalError();",
                true
            );
        }
    }
}