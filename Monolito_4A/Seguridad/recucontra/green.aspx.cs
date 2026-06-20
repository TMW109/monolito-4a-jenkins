using System;
using System.Web.UI;
using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;

namespace Monolito_4A.Seguridad.recucontra
{
    public partial class green : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        }

        protected async void btnEnviar_Click(object sender, EventArgs e)
        {
            string cedula = txtCedula.Text.Trim();
            string celular = txtCelular.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(celular))
            {
                MostrarMensaje("Ingrese cédula y celular.");
                return;
            }

            if (cedula.Length != 10)
            {
                MostrarMensaje("La cédula debe tener 10 dígitos.");
                return;
            }

            if (celular.Length != 10)
            {
                MostrarMensaje("El celular debe tener 10 dígitos.");
                return;
            }

            tbl_usuario usuario = CN_tbl_usuario.ObtenerUsuarioPorCedulaTodos(cedula);

            if (usuario == null)
            {
                MostrarMensaje("No existe un usuario con esa cédula.");
                return;
            }

            if (usuario.usu_celular != celular)
            {
                MostrarMensaje("El celular no coincide con el usuario registrado.");
                return;
            }

            string claveTemporal = RandomPassword.Generate(
                8,
                PasswordGroup.Uppercase,
                PasswordGroup.Lowercase,
                PasswordGroup.Numeric
            );

            bool actualizado = CN_tbl_usuario.RecuperarPasswordTemporalWhatsapp(
                usuario.usu_cedula,
                claveTemporal
            );

            if (!actualizado)
            {
                MostrarMensaje("No se pudo actualizar la clave temporal.");
                return;
            }

            string mensaje = "Tu clave temporal REQUIEM es: " + claveTemporal;

            bool enviado = await GreenApiWhatsApp.EnviarMensajeAsync(
                usuario.usu_celular,
                mensaje
            );

            if (!enviado)
            {
                MostrarMensaje("La clave temporal fue generada, pero Green API no pudo enviar el mensaje.");
                return;
            }

            txtCedula.Enabled = false;
            txtCelular.Enabled = false;
            btnEnviar.Enabled = false;

            MostrarMensaje("Clave temporal enviada correctamente por WhatsApp.");

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                "document.getElementById('modalExito').style.display='flex';",
                true
            );
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
        }
    }
}