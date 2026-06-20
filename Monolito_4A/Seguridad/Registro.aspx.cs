using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Capa_Datos;
using Capa_Negocio;
using SimpleCrypto;

namespace Monolito_4A.Seguridad
{
    public partial class Registro : System.Web.UI.Page
    {
        private tbl_usuario obusuario = new tbl_usuario();
        private const int MAX_FOTO_BYTES = 5 * 1024 * 1024;

        [Serializable]
        public class FotoTemporal
        {
            public byte[] Bytes { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            if (!IsPostBack)
            {
                CargarTipoUsuario();
                LimpiarCampos();
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            MantenerPasswords();

            if (!fuFoto.HasFiles)
            {
                MostrarModal("Fotos requeridas", "Seleccione al menos una foto.");
                return;
            }

            if (!ValidarFotosServidor())
            {
                return;
            }

            List<FotoTemporal> fotos = new List<FotoTemporal>();
            previewContainer.InnerHtml = "";

            foreach (HttpPostedFile archivo in fuFoto.PostedFiles)
            {
                byte[] fotoBytes = new byte[archivo.ContentLength];
                archivo.InputStream.Read(fotoBytes, 0, archivo.ContentLength);

                FotoTemporal foto = new FotoTemporal();
                foto.Bytes = fotoBytes;
                foto.Nombre = Path.GetFileName(archivo.FileName);
                foto.Tipo = archivo.ContentType;

                fotos.Add(foto);

                string base64 = Convert.ToBase64String(fotoBytes);

                previewContainer.InnerHtml +=
                    "<img src='data:" + archivo.ContentType + ";base64," + base64 + "' " +
                    "class='img-preview-multiple' onclick='abrirModalImagenDesdeSrc(this.src)' />";
            }

            Session["fotos_usuario"] = fotos;

            MostrarModal("Fotos cargadas", "Las fotos fueron previsualizadas correctamente.");
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            MantenerPasswords();
            string passwordFinal = ObtenerPassword();
            string confirmarFinal = ObtenerConfirmarPassword();
            if (!ValidarCedulaEcuatoriana(txtCedula.Text.Trim()))
            {
                MostrarModal("Cédula inválida", "Ingrese una cédula ecuatoriana válida.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtCelular.Text) ||
                string.IsNullOrWhiteSpace(txtFechaCumple.Text) ||
                ddlTipoUsuario.SelectedValue == "0" ||
                Session["fotos_usuario"] == null)
            {
                MostrarModal("Faltan campos", "Complete todos los campos obligatorios y previsualice al menos una foto.");
                return;
            }
            if (!EsMayorDeEdad(txtFechaCumple.Text))
            {
                MostrarModal("Edad no permitida", "Debe tener mínimo 18 años para registrarse.");
                return;
            }

            if (passwordFinal != confirmarFinal)
            {
                MostrarModal("Contraseñas diferentes", "La contraseña y la confirmación no coinciden.");
                return;
            }

            try
            {
                tbl_usuario nuevo = new tbl_usuario();

                nuevo.usu_cedula = txtCedula.Text.Trim();
                nuevo.usu_nombres = txtNombres.Text.Trim();
                nuevo.usu_apellidos = txtApellidos.Text.Trim();
                nuevo.usu_direccion = txtDireccion.Text.Trim();
                nuevo.usu_celular = txtCelular.Text.Trim();
                nuevo.usu_correo = txtCorreo.Text.Trim();
                nuevo.usu_fecha_creacion = DateTime.Now;
                nuevo.usu_fecha_cumple = Convert.ToDateTime(txtFechaCumple.Text);
                nuevo.usu_nick = txtUsuario.Text.Trim();

                nuevo.usu_contrasenia = new System.Data.Linq.Binary(
                    System.Text.Encoding.UTF8.GetBytes(passwordFinal.Trim())
                );

                nuevo.usu_intentos = 0;
                nuevo.usu_codigo_OPT = null;
                nuevo.usu_estado = 'A';
                nuevo.tusu_id = Convert.ToInt32(ddlTipoUsuario.SelectedValue);
                nuevo.usu_fecha_ultimo_intento = null;

                int respuesta = CN_tbl_usuario.RegistrarUsuario(nuevo);

                if (respuesta > 0)
                {
                    List<FotoTemporal> fotos = Session["fotos_usuario"] as List<FotoTemporal>;

                    if (fotos == null || fotos.Count == 0)
                    {
                        MostrarModal("Error", "Usuario registrado, pero no se encontraron fotos para guardar.");
                        return;
                    }

                    foreach (FotoTemporal foto in fotos)
                    {
                        string respFoto = CN_tbl_usuario_imagen.GuardarImagen(
                            respuesta,
                            foto.Bytes,
                            foto.Nombre,
                            foto.Tipo
                        );

                        if (respFoto != "OK")
                        {
                            MostrarModal("Error", "Usuario registrado, pero no se pudo guardar una de las fotos.");
                            return;
                        }
                    }

                    Session.Remove("fotos_usuario");

                    LimpiarCampos();

                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        Guid.NewGuid().ToString(),
                        "setTimeout(function(){ mostrarModalRegistro(); }, 300);",
                        true
                    );
                }
                else
                {
                    string mensaje = "";

                    if (respuesta == -1)
                        mensaje = "Cédula existente";
                    else if (respuesta == -2)
                        mensaje = "Correo existente";
                    else if (respuesta == -3)
                        mensaje = "Nick existente";
                    else
                        mensaje = "Error al registrar usuario";

                    MostrarModal("Error", mensaje);
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message.Replace("'", "\\'");
                MostrarModal("Error", mensaje);
            }
        }

        private bool ValidarFotosServidor()
        {
            foreach (HttpPostedFile archivo in fuFoto.PostedFiles)
            {
                string tipo = archivo.ContentType.ToLower();
                string extension = Path.GetExtension(archivo.FileName).ToLower();

                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    MostrarModal("Archivo no permitido", "Solo se permiten imágenes JPG o PNG.");
                    return false;
                }

                if (tipo != "image/jpeg" && tipo != "image/png")
                {
                    MostrarModal("Archivo no permitido", "El archivo debe ser una imagen JPG o PNG.");
                    return false;
                }

                if (archivo.ContentLength > MAX_FOTO_BYTES)
                {
                    MostrarModal("Imagen muy pesada", "Cada foto no debe superar los 5 MB.");
                    return false;
                }
            }

            return true;
        }

        private void MostrarModal(string titulo, string mensaje)
        {
            titulo = titulo.Replace("'", "\\'");
            mensaje = mensaje.Replace("'", "\\'");

            ScriptManager.RegisterStartupScript(
                UpdatePanel1,
                UpdatePanel1.GetType(),
                Guid.NewGuid().ToString(),
                $"mostrarModalMensaje('{titulo}', '{mensaje}');",
                true
            );
        }

        private void CargarTipoUsuario()
        {
            ddlTipoUsuario.DataSource = CN_tbl_tipo_usuario.ListarTipoUsuario();
            ddlTipoUsuario.DataTextField = "tusu_nombre";
            ddlTipoUsuario.DataValueField = "tusu_id";
            ddlTipoUsuario.DataBind();

            ddlTipoUsuario.Items.Insert(0, new ListItem("Seleccione tipo de usuario", "0"));
        }

        private void LimpiarCampos()
        {
            txtCedula.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtDireccion.Text = "";
            txtCelular.Text = "";
            txtCorreo.Text = "";
            txtFechaCumple.Text = "";
            txtUsuario.Text = "";
            txtPassword.Text = "";
            txtConfirmar.Text = "";
            txtPassword.Attributes["value"] = "";
            txtConfirmar.Attributes["value"] = "";
            ddlTipoUsuario.SelectedIndex = 0;

            previewContainer.InnerHtml = "";
        }

        protected void txtApellidos_TextChanged(object sender, EventArgs e)
        {
            string[] nom = txtNombres.Text
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string[] ape = txtApellidos.Text
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (nom.Length == 0 || ape.Length == 0)
            {
                txtCorreo.Text = "";

                MostrarModal("Campos incompletos", "Debe ingresar al menos un nombre y un apellido.");
                return;
            }

            string primerNombre = nom[0].ToLower();
            string primerApellido = ape[0].ToLower();

            txtCorreo.Text = primerNombre + "." + primerApellido + "@gmail.com";

            string passwordGenerada = RandomPassword.Generate(
                8,
                PasswordGroup.Numeric,
                PasswordGroup.Lowercase,
                PasswordGroup.Uppercase,
                PasswordGroup.Special
            );

            txtPassword.Text = passwordGenerada;
            txtPassword.Attributes["value"] = passwordGenerada;

            txtConfirmar.Text = passwordGenerada;
            txtConfirmar.Attributes["value"] = passwordGenerada;

            Random rnd = new Random();

            string segundoNombre = nom.Length > 1 ? nom[1] : nom[0];
            string segundoApellido = ape.Length > 1 ? ape[1] : ape[0];

            char[] ced = txtCedula.Text.ToCharArray();

            string nick =
                primerNombre.Substring(0, 1).ToUpper() +
                segundoNombre.Substring(0, 1).ToLower() +
                segundoApellido.Substring(0, 1).ToUpper() +
                rnd.Next(100, 1000).ToString() +
                RandomPassword.Generate(1, PasswordGroup.Special);

            if (ced.Length > 0)
            {
                nick += ced[rnd.Next(ced.Length)].ToString();
                nick += ced[rnd.Next(ced.Length)].ToString();
            }

            txtUsuario.Text = nick;
        }

        //METODOS PARA QUE LA CONTRASNIA NO SE RECARGUE POR EL PREVISUALIZAR AL REALIZAR UN POSTBACK
        private string ObtenerPassword()
        {
            string pass = Request.Form[txtPassword.UniqueID];

            if (string.IsNullOrEmpty(pass))
                pass = txtPassword.Text;

            return pass;
        }

        private string ObtenerConfirmarPassword()
        {
            string confirmar = Request.Form[txtConfirmar.UniqueID];

            if (string.IsNullOrEmpty(confirmar))
                confirmar = txtConfirmar.Text;

            return confirmar;
        }

        private void MantenerPasswords()
        {
            string pass = ObtenerPassword();
            string confirmar = ObtenerConfirmarPassword();

            txtPassword.Attributes["value"] = pass;
            txtConfirmar.Attributes["value"] = confirmar;
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
        private bool EsMayorDeEdad(string fechaTexto)
        {
            DateTime fechaNacimiento;

            if (!DateTime.TryParse(fechaTexto, out fechaNacimiento))
                return false;

            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;

            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad >= 18;
        }
    }
}