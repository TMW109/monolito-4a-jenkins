using System;
using System.Web.UI;

namespace Monolito_4A
{
    public partial class Usu : System.Web.UI.Page
    {
        private const int INTENTOS_MAXIMOS = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null || Session["tusu_id"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
                return;
            }

            if (Convert.ToInt32(Session["tusu_id"]) != 4)
            {
                Response.Redirect("~/admin.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUsuario.Text = Session["usuario"].ToString();
                pnlInicio.Visible = true;
                pnlJuego.Visible = false;
                lblMensaje.Text = "";
            }
        }

        protected void btnIniciarJuego_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();

            Session["codigo_secreto"] = rnd.Next(1, 21);
            Session["intentos_restantes"] = INTENTOS_MAXIMOS;
            Session["juego_activo"] = true;

            pnlInicio.Visible = false;
            pnlJuego.Visible = true;

            txtNumero.Enabled = true;
            btnVerificar.Enabled = true;

            lblIntentos.Text = INTENTOS_MAXIMOS.ToString();
            lblMensaje.Text = "Protocolo iniciado. Ingresa un número del 1 al 20.";
            txtNumero.Text = "";
        }

        protected void btnVerificar_Click(object sender, EventArgs e)
        {
            if (Session["juego_activo"] == null || !(bool)Session["juego_activo"])
            {
                lblMensaje.Text = "Primero debes iniciar el juego.";
                pnlInicio.Visible = true;
                pnlJuego.Visible = false;
                return;
            }

            int numeroIngresado;

            if (!int.TryParse(txtNumero.Text.Trim(), out numeroIngresado))
            {
                lblMensaje.Text = "Debes ingresar un número válido.";
                return;
            }

            if (numeroIngresado < 1 || numeroIngresado > 20)
            {
                lblMensaje.Text = "El código debe estar entre 1 y 20.";
                return;
            }

            int codigoSecreto = Convert.ToInt32(Session["codigo_secreto"]);
            int intentosRestantes = Convert.ToInt32(Session["intentos_restantes"]);

            if (numeroIngresado == codigoSecreto)
            {
                Session["juego_activo"] = false;

                lblMensaje.Text = "✅ Código correcto. Virus desactivado correctamente.";
                lblIntentos.Text = intentosRestantes.ToString();

                txtNumero.Enabled = false;
                btnVerificar.Enabled = false;
                return;
            }

            intentosRestantes--;
            Session["intentos_restantes"] = intentosRestantes;
            lblIntentos.Text = intentosRestantes.ToString();

            if (intentosRestantes <= 0)
            {
                Session["juego_activo"] = false;

                lblMensaje.Text = "❌ Fallaste. El código correcto era: " + codigoSecreto;
                txtNumero.Enabled = false;
                btnVerificar.Enabled = false;
                return;
            }

            if (numeroIngresado < codigoSecreto)
            {
                lblMensaje.Text = "🔺 El código secreto es mayor.";
            }
            else
            {
                lblMensaje.Text = "🔻 El código secreto es menor.";
            }

            txtNumero.Text = "";
        }

        protected void btnReiniciar_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();

            Session["codigo_secreto"] = rnd.Next(1, 21);
            Session["intentos_restantes"] = INTENTOS_MAXIMOS;
            Session["juego_activo"] = true;

            pnlInicio.Visible = false;
            pnlJuego.Visible = true;

            txtNumero.Enabled = true;
            btnVerificar.Enabled = true;

            lblIntentos.Text = INTENTOS_MAXIMOS.ToString();
            lblMensaje.Text = "Juego reiniciado. Ingresa un nuevo código.";
            txtNumero.Text = "";
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Seguridad/Login.aspx");
        }
    }
}