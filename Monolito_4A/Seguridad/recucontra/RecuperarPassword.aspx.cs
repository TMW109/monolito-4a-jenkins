using Capa_Datos;
using Capa_Negocio;
using System;
using System.Web.UI;

namespace Monolito_4A.Seguridad.recucontra
{
    public partial class RecuperarPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MostrarMensaje("Ingrese cédula y correo.", false);
                return;
            }

            tbl_usuario usuario = CN_tbl_usuario.RecuperarPorCedulaCorreo(
                txtCedula.Text.Trim(),
                txtCorreo.Text.Trim()
            );

            if (usuario == null)
            {
                MostrarMensaje("No existe un usuario con esos datos.", false);
                return;
            }

            hfUsuarioId.Value = usuario.usu_id.ToString();

            txtNuevaPassword.Enabled = true;
            txtConfirmarPassword.Enabled = true;
            btnCambiar.Enabled = true;

            txtCedula.Enabled = false;
            txtCorreo.Enabled = false;
            btnVerificar.Enabled = false;

            MostrarMensaje("Usuario verificado. Ahora ingrese su nueva contraseña.", true);
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfUsuarioId.Value))
            {
                MostrarMensaje("Primero debe verificar el usuario.", false);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNuevaPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
            {
                MostrarMensaje("Ingrese y confirme la nueva contraseña.", false);
                return;
            }

            string nueva = txtNuevaPassword.Text.Trim();
            string confirmar = txtConfirmarPassword.Text.Trim();

            if (nueva != confirmar)
            {
                MostrarMensaje("Las contraseñas no coinciden.", false);
                return;
            }

            if (nueva.Length < 8)
            {
                MostrarMensaje("La contraseña debe tener mínimo 8 caracteres.", false);
                return;
            }

            bool ok = CN_tbl_usuario.RecuperarPassword(
                Convert.ToInt32(hfUsuarioId.Value),
                nueva
            );

            if (!ok)
            {
                MostrarMensaje("No se pudo cambiar la contraseña.", false);
                return;
            }

            string script = @"
    var modal = document.getElementById('modalOk');
    if (modal) {
        modal.style.display = 'flex';
    }

    setTimeout(function () {
        window.location.href = '../Login.aspx';
    }, 2500);
";

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        private void MostrarMensaje(string mensaje, bool correcto)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = correcto ? "mensaje ok" : "mensaje error";
        }
    }
}