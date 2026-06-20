using System;
using System.Web.UI;
using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Monolito_4A
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);

            if (!IsPostBack)
            {
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();
                using (DataClasses1DataContext dc = new DataClasses1DataContext())
                {
                    dc.sp_ReiniciarIntentosDiarios();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtced.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MostrarError();
                return;
            }

            string cedula = txtced.Text.Trim();
            if (!ValidarCedulaEcuatoriana(cedula))
            {
                MostrarError();
                return;
            }
            string password = txtPassword.Text.Trim();

            tbl_usuario usoinfo = CN_tbl_usuario.ObtenerUsuarioPorCedulaTodos(cedula);

            if (usoinfo == null)
            {
                MostrarError();
                return;
            }

            if (usoinfo.usu_estado == 'B')
            {
                MostrarCuentaBloqueada();
                return;
            }

            bool existecc = CN_tbl_usuario.ValidarPassword(cedula, password);

            if (existecc)
            {
                string codopt = RandomPassword.Generate(
                    8,
                    PasswordGroup.Uppercase,
                    PasswordGroup.Numeric,
                    PasswordGroup.Special
                );

                string optencripta = EncriptarOTP(codopt);
                byte[] qrBytes = GenerarQR(optencripta);

                Mail mail = new Mail();
                bool enviado = mail.EnviarQR(usoinfo.usu_correo, qrBytes);

                if (!enviado)
                {
                    MostrarError();
                    return;
                }
                Session["cedula_2fa"] = usoinfo.usu_cedula;
                bool actualizado = CN_tbl_usuario.RegistrarLoginCorrecto(
                    usoinfo.usu_id,
                    optencripta
                );

                if (!actualizado)
                {
                    MostrarError();
                    return;
                }

                Session["cedula_2fa"] = usoinfo.usu_cedula;

                Response.Redirect("~/Seguridad/VerificarOTP.aspx");
            }
            else
            {
                bool bloqueado = CN_tbl_usuario.RegistrarIntentoFallido(usoinfo.usu_id);

                if (bloqueado)
                {
                    MostrarCuentaBloqueada();
                    return;
                }

                MostrarError();
            }
        }

        private void MostrarError()
        {
            string script = @"
                var progress = document.getElementById('progressLogin');
                var btn = document.getElementById('btnLogin');
                var modal = document.getElementById('modalError');

                if (progress) progress.style.display = 'none';

                if (btn) {
                    btn.value = 'Acceder al sistema';
                    btn.style.pointerEvents = 'auto';
                    btn.style.opacity = '1';
                }

                if (modal) {
                    modal.style.display = 'flex';
                }
            ";

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        private void MostrarCuentaBloqueada()
        {
            string script = @"
                var progress = document.getElementById('progressLogin');
                var btn = document.getElementById('btnLogin');
                var modal = document.getElementById('modalBloqueado');

                if (progress) progress.style.display = 'none';

                if (btn) {
                    btn.value = 'Acceder al sistema';
                    btn.style.pointerEvents = 'auto';
                    btn.style.opacity = '1';
                }

                if (modal) {
                    modal.style.display = 'flex';
                } else {
                    alert('Cuenta bloqueada. Recupera tu contraseña.');
                }
            ";

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        private string EncriptarOTP(string texto)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(texto);
            string base64 = Convert.ToBase64String(bytes);

            string seguro = base64
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            return seguro;
        }

        private byte[] GenerarQR(string textoEncriptado)
        {
            QRCodeGenerator qrgenrador = new QRCodeGenerator();

            QRCodeData qrCodeData = qrgenrador.CreateQrCode(
                textoEncriptado,
                QRCodeGenerator.ECCLevel.Q
            );

            QRCode qrcode = new QRCode(qrCodeData);

            using (Bitmap qrImagen = qrcode.GetGraphic(30))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    qrImagen.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 10)
                return false;

            long numero;
            if (!long.TryParse(cedula, out numero))
                return false;

            int provincia = Convert.ToInt32(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
                return false;

            int tercerDigito = Convert.ToInt32(cedula.Substring(2, 1));
            if (tercerDigito >= 6)
                return false;

            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int digito = Convert.ToInt32(cedula.Substring(i, 1));

                if (i % 2 == 0)
                {
                    digito *= 2;
                    if (digito > 9)
                        digito -= 9;
                }

                suma += digito;
            }

            int digitoVerificador = Convert.ToInt32(cedula.Substring(9, 1));
            int residuo = suma % 10;
            int resultado = residuo == 0 ? 0 : 10 - residuo;

            return resultado == digitoVerificador;
        }
    }
}