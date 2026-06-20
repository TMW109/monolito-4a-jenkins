using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4A
{
    public partial class adminusu : System.Web.UI.Page
    {
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

            if (Session["cedula"] == null)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
                return;
            }

            tbl_usuario usuarioSesion = CN_tbl_usuario.traerxcedTodos(Session["cedula"].ToString());

            if (usuarioSesion == null || usuarioSesion.tusu_id != 3)
            {
                Response.Redirect("~/Seguridad/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarTipoUsuario();
                CargarUsuarios();
            }
        }

        private void CargarTipoUsuario()
        {
            ddlTipoUsuario.DataSource = CN_tbl_tipo_usuario.ListarTipoUsuario();
            ddlTipoUsuario.DataTextField = "tusu_nombre";
            ddlTipoUsuario.DataValueField = "tusu_id";
            ddlTipoUsuario.DataBind();
            ddlTipoUsuario.Items.Insert(0, new ListItem("Seleccione tipo usuario", "0"));
        }

        private void CargarUsuarios()
        {
            gvUsuarios.DataSource = CN_tbl_usuario.ListarUsuariosAdmin();
            gvUsuarios.DataBind();
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            MantenerPassword();

            if (!fuFotos.HasFiles)
            {
                MostrarModal("Fotos requeridas", "Seleccione al menos una foto.");
                return;
            }

            if (!ValidarFotosServidor())
                return;

            List<FotoTemporal> fotos = new List<FotoTemporal>();
            previewContainer.InnerHtml = "";

            foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
            {
                byte[] bytes = new byte[archivo.ContentLength];
                archivo.InputStream.Read(bytes, 0, archivo.ContentLength);

                fotos.Add(new FotoTemporal
                {
                    Bytes = bytes,
                    Nombre = Path.GetFileName(archivo.FileName),
                    Tipo = archivo.ContentType
                });

                string base64 = Convert.ToBase64String(bytes);

                previewContainer.InnerHtml +=
                    "<img src='data:" + archivo.ContentType + ";base64," + base64 + "' class='img-preview-multiple' onclick='abrirModalImagenDesdeSrc(this.src)' />";
            }

            Session["fotos_admin_usuario"] = fotos;

            MostrarModal("Fotos cargadas", "Previsualización correcta.");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos())
                return;

            if (string.IsNullOrWhiteSpace(hfUsuarioId.Value))
                RegistrarUsuario();
            else
                ActualizarUsuario();

            CargarUsuarios();
        }

        private void RegistrarUsuario()
        {
            string password = ObtenerPassword();

            tbl_usuario nuevo = new tbl_usuario
            {
                usu_cedula = txtCedula.Text.Trim(),
                usu_nombres = txtNombres.Text.Trim(),
                usu_apellidos = txtApellidos.Text.Trim(),
                usu_direccion = txtDireccion.Text.Trim(),
                usu_celular = txtCelular.Text.Trim(),
                usu_correo = txtCorreo.Text.Trim(),
                usu_fecha_creacion = DateTime.Now,
                usu_fecha_cumple = Convert.ToDateTime(txtFechaCumple.Text),
                usu_nick = txtNick.Text.Trim(),
                usu_contrasenia = new System.Data.Linq.Binary(System.Text.Encoding.UTF8.GetBytes(password)),
                usu_estado = Convert.ToChar(ddlEstado.SelectedValue),
                tusu_id = Convert.ToInt32(ddlTipoUsuario.SelectedValue)
            };

            int id = CN_tbl_usuario.RegistrarUsuarioAdmin(nuevo);

            if (id <= 0)
            {
                MostrarModal("Error", "Datos duplicados.");
                return;
            }

            GuardarFotos(id);
            MostrarModal("Correcto", "Usuario registrado.");
            LimpiarCampos();
        }

        private void ActualizarUsuario()
        {
            int id = Convert.ToInt32(hfUsuarioId.Value);

            tbl_usuario datos = new tbl_usuario
            {
                usu_id = id,
                usu_cedula = txtCedula.Text.Trim(),
                usu_nombres = txtNombres.Text.Trim(),
                usu_apellidos = txtApellidos.Text.Trim(),
                usu_direccion = txtDireccion.Text.Trim(),
                usu_celular = txtCelular.Text.Trim(),
                usu_correo = txtCorreo.Text.Trim(),
                usu_fecha_cumple = Convert.ToDateTime(txtFechaCumple.Text),
                usu_nick = txtNick.Text.Trim(),
                tusu_id = Convert.ToInt32(ddlTipoUsuario.SelectedValue),
                usu_estado = Convert.ToChar(ddlEstado.SelectedValue)
            };

            if (!CN_tbl_usuario.ActualizarUsuarioAdmin(datos))
            {
                MostrarModal("Error", "No se pudo actualizar.");
                return;
            }

            string password = ObtenerPassword();

            if (!string.IsNullOrWhiteSpace(password))
                CN_tbl_usuario.CambiarPasswordAdmin(id, password);

            GuardarFotos(id);

            MostrarModal("Correcto", "Usuario actualizado.");
            LimpiarCampos();
        }

        private void GuardarFotos(int id)
        {
            var fotos = Session["fotos_admin_usuario"] as List<FotoTemporal>;

            if (fotos == null) return;

            foreach (var f in fotos)
            {
                CN_tbl_usuario_imagen.GuardarImagen(id, f.Bytes, f.Nombre, f.Tipo);
            }

            Session.Remove("fotos_admin_usuario");
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarUsuario")
                CargarUsuarioParaEditar(id);

            else if (e.CommandName == "EliminarFisico")
            {
                if (CN_tbl_usuario.UsuarioTieneVinculos(id))
                {
                    MostrarModal("Error", "Usuario vinculado a otra tabla.");
                    return;
                }

                CN_tbl_usuario.EliminarUsuarioFisico(id);
                MostrarModal("Correcto", "Usuario eliminado.");
                CargarUsuarios();
            }
        }

        private void CargarUsuarioParaEditar(int id)
        {
            var u = CN_tbl_usuario.ObtenerUsuarioPorId(id);

            hfUsuarioId.Value = u.usu_id.ToString();
            txtCedula.Text = u.usu_cedula;
            txtNombres.Text = u.usu_nombres;
            txtApellidos.Text = u.usu_apellidos;
            txtDireccion.Text = u.usu_direccion;
            txtCelular.Text = u.usu_celular;
            txtCorreo.Text = u.usu_correo;
            if (u.usu_fecha_cumple != null)
                txtFechaCumple.Text = Convert.ToDateTime(u.usu_fecha_cumple).ToString("yyyy-MM-dd");
            txtNick.Text = u.usu_nick;
            ddlTipoUsuario.SelectedValue = u.tusu_id.ToString();
            ddlEstado.SelectedValue = u.usu_estado.ToString();
        }

        private bool ValidarCampos()
        {
            string password = ObtenerPassword();

            if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text))
            {
                MostrarModal("Error", "Campos obligatorios.");
                return false;
            }

            if (!ValidarCedulaEcuatoriana(txtCedula.Text))
            {
                MostrarModal("Error", "Cédula inválida.");
                return false;
            }

            if (!EsMayorDeEdad(txtFechaCumple.Text))
            {
                MostrarModal("Edad no permitida", "El usuario debe tener mínimo 18 años.");
                return false;
            }

            if (!PasswordSegura(password) && string.IsNullOrWhiteSpace(hfUsuarioId.Value))
            {
                MostrarModal("Error", "Contraseña débil.");
                return false;
            }

            return true;
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

        private string ObtenerPassword()
        {
            return Request.Form[txtPassword.UniqueID] ?? txtPassword.Text;
        }

        private void MantenerPassword()
        {
            txtPassword.Attributes["value"] = ObtenerPassword();
        }

        private void MostrarModal(string t, string m)
        {
            ScriptManager.RegisterStartupScript(
                UpdatePanel1,
                UpdatePanel1.GetType(),
                Guid.NewGuid().ToString(),
                $"mostrarModalAdmin('{t}','{m}');",
                true);
        }

        private bool PasswordSegura(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) return false;

            return p.Length >= 8 &&
                   System.Text.RegularExpressions.Regex.IsMatch(p, "[A-Z]") &&
                   System.Text.RegularExpressions.Regex.IsMatch(p, "[a-z]") &&
                   System.Text.RegularExpressions.Regex.IsMatch(p, "[0-9]") &&
                   System.Text.RegularExpressions.Regex.IsMatch(p, "[^A-Za-z0-9]");
        }

        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (cedula.Length != 10) return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int d = int.Parse(cedula[i].ToString());
                if (i % 2 == 0)
                {
                    d *= 2;
                    if (d > 9) d -= 9;
                }
                suma += d;
            }

            int ver = int.Parse(cedula[9].ToString());
            int res = suma % 10 == 0 ? 0 : 10 - (suma % 10);

            return res == ver;
        }
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            MostrarModal("Formulario limpio", "Los campos fueron limpiados correctamente.");
        }
        private bool ValidarFotosServidor()
        {
            foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
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

        private void LimpiarCampos()
        {
            hfUsuarioId.Value = "";
            txtCedula.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtDireccion.Text = "";
            txtCelular.Text = "";
            txtCorreo.Text = "";
            txtFechaCumple.Text = "";
            txtNick.Text = "";
            txtPassword.Text = "";
            txtPassword.Attributes["value"] = "";

            ddlTipoUsuario.SelectedIndex = 0;
            ddlEstado.SelectedValue = "A";

            previewContainer.InnerHtml = "";
            Session.Remove("fotos_admin_usuario");
        }


        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Seguridad/Login.aspx");
        }

    }
}