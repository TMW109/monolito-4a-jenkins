using System;
using System.Text;
using System.Web.UI;
using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;

namespace Monolito_4A.Seguridad.recucontra
{
    public partial class RecuperarWhatsapp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            if (!IsPostBack)
            {
                txtCelular.Enabled = false;
                btnEnviar.Enabled = false;
                lnkWhatsapp.Visible = false;
                lblMensaje.Text = "";
            }
        }

        protected void btnBuscarCedula_Click(object sender, EventArgs e)
        {
            string cedula = txtCedula.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                MostrarMensaje("Ingrese la cédula.");
                BloquearCelular();
                return;
            }

            if (cedula.Length != 10)
            {
                MostrarMensaje("La cédula debe tener 10 dígitos.");
                BloquearCelular();
                return;
            }

            tbl_usuario usuario = CN_tbl_usuario.ObtenerUsuarioPorCedulaTodos(cedula);

            if (usuario == null)
            {
                MostrarMensaje("No existe un usuario con esa cédula.");
                BloquearCelular();
                return;
            }

            Session["cedula_recuperacion_whatsapp"] = usuario.usu_cedula;

            txtCedula.Enabled = false;
            txtCelular.Enabled = true;
            btnEnviar.Enabled = true;
            lnkWhatsapp.Visible = false;

            MostrarMensaje("Cédula encontrada. Ahora ingrese el celular registrado.");
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (Session["cedula_recuperacion_whatsapp"] == null)
            {
                MostrarMensaje("Primero debe buscar una cédula válida.");
                BloquearCelular();
                return;
            }

            string cedula = Session["cedula_recuperacion_whatsapp"].ToString();
            string celular = txtCelular.Text.Trim();

            if (string.IsNullOrWhiteSpace(celular))
            {
                MostrarMensaje("Ingrese el celular registrado.");
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
                MostrarMensaje("Usuario no encontrado.");
                BloquearCelular();
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

            PrepararWhatsapp(usuario, claveTemporal);

            txtCelular.Enabled = false;
            btnEnviar.Enabled = false;
            Session.Remove("cedula_recuperacion_whatsapp");
        }

        private void PrepararWhatsapp(tbl_usuario usuario, string claveTemporal)
        {
            string numero = FormatearNumeroWhatsapp(usuario.usu_celular);

            string mensaje = "Tu clave temporal REQUIEM es: " + claveTemporal;

            string url = "https://wa.me/" + numero + "?text=" + Server.UrlEncode(mensaje);

            lnkWhatsapp.NavigateUrl = url;
            lnkWhatsapp.Visible = true;

            lnkWhatsappModal.NavigateUrl = url;
            lnkWhatsappModal.Visible = true;

            MostrarMensaje("Clave temporal generada. Presiona Abrir WhatsApp.");

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                "document.getElementById('modalExito').style.display='flex';",
                true
            );
        }

        private string FormatearNumeroWhatsapp(string numero)
        {
            numero = numero.Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("+", "");

            if (numero.StartsWith("0"))
            {
                numero = "593" + numero.Substring(1);
            }

            return numero;
        }

        private void BloquearCelular()
        {
            txtCedula.Enabled = true;
            txtCelular.Enabled = false;
            txtCelular.Text = "";
            btnEnviar.Enabled = false;
            lnkWhatsapp.Visible = false;
            Session.Remove("cedula_recuperacion_whatsapp");
        }

        private void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
        }
    }
}