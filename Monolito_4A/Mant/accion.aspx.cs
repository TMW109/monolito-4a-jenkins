using System;
using System.Linq;
using Capa_Datos;
using Capa_Negocio;

namespace Monolito_4A.Mant
{
    public partial class accion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarSesionAdmin();

            if (!IsPostBack)
            {
                CargarAdministrador();
            }
        }

        private void ValidarSesionAdmin()
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);

            if (Session["cedula"] == null || Session["tusu_id"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Convert.ToInt32(Session["tusu_id"]) != 3)
            {
                Session.Clear();
                Session.Abandon();

                Response.Redirect("~/Seguridad/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            tbl_usuario usuarioSesion = CN_tbl_usuario.traerxcedTodos(Session["cedula"].ToString());

            if (usuarioSesion == null || usuarioSesion.tusu_id != 3)
            {
                Session.Clear();
                Session.Abandon();

                Response.Redirect("~/Seguridad/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void CargarAdministrador()
        {
            string cedula = Session["cedula"].ToString();

            tbl_usuario usuario = CN_tbl_usuario.traerxcedTodos(cedula);

            if (usuario == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            lblNombre.Text = usuario.usu_nick;

            try
            {
                if (usuario.tbl_usuario_imagen != null &&
                    usuario.tbl_usuario_imagen.Any(i => i.uimg_estado == 'A'))
                {
                    byte[] foto = usuario.tbl_usuario_imagen
                        .Where(i => i.uimg_estado == 'A')
                        .First()
                        .uimg_data
                        .ToArray();

                    imgPerfil.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(foto);
                }
                else
                {
                    imgPerfil.ImageUrl = ResolveUrl("~/imagen/imglogo.png");
                }
            }
            catch
            {
                imgPerfil.ImageUrl = ResolveUrl("~/imagen/imglogo.png");
            }
        }
    }
}