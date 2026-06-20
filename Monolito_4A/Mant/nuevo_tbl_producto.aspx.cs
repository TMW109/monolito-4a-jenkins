using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Monolito_4A.Mant
{
    public partial class nuevo_tbl_producto : System.Web.UI.Page
    {
        private const int MAX_FOTO_BYTES = 5 * 1024 * 1024;

        [Serializable]
        public class FotoProductoTemporal
        {
            public byte[] Bytes { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public string Ruta { get; set; }
        }

  

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarSesionAdmin();

            if (!IsPostBack)
            {
                CargarAdministrador();
                CargarProveedores();
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

        private void CargarProveedores()
        {
            ddlProveedor.DataSource = CN_tbl_proveedor.ListarProveedorActivo();
            ddlProveedor.DataTextField = "prov_nombre";
            ddlProveedor.DataValueField = "prov_id";
            ddlProveedor.DataBind();

            ddlProveedor.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Seleccione proveedor", "0"));
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFotos.HasFiles)
            {
                MostrarModal("Imágenes requeridas", "Seleccione al menos una imagen.");
                return;
            }

            if (!ValidarFotosServidor())
                return;

            List<FotoProductoTemporal> fotos = new List<FotoProductoTemporal>();
            previewContainer.InnerHtml = "";

            foreach (HttpPostedFile archivo in fuFotos.PostedFiles)
            {
                byte[] fotoBytes = new byte[archivo.ContentLength];
                archivo.InputStream.Read(fotoBytes, 0, archivo.ContentLength);

                FotoProductoTemporal foto = new FotoProductoTemporal();
                foto.Bytes = fotoBytes;
                foto.Nombre = Path.GetFileName(archivo.FileName);
                foto.Tipo = archivo.ContentType;

                fotos.Add(foto);

                string base64 = Convert.ToBase64String(fotoBytes);

                previewContainer.InnerHtml +=
                    "<img src='data:" + archivo.ContentType + ";base64," + base64 + "' " +
                    "class='img-preview' onclick='abrirModalImagenDesdeSrc(this.src)' />";
            }

            Session["fotos_producto"] = fotos;

            MostrarModal("Imágenes cargadas", "Las imágenes fueron previsualizadas correctamente.");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormularioServidor())
                return;

            try
            {
                string nombre = txtNombre.Text.Trim();
                int cantidad = Convert.ToInt32(txtCantidad.Text.Trim());
                decimal precio = ConvertirPrecio(txtPrecio.Text.Trim());
                int provId = Convert.ToInt32(ddlProveedor.SelectedValue);

                int proId = CN_tbl_producto.InsertarProducto(nombre, cantidad, precio, provId);

                if (proId <= 0)
                {
                    MostrarModal("Error", "No se pudo registrar el producto.");
                    return;
                }

                List<FotoProductoTemporal> fotos = Session["fotos_producto"] as List<FotoProductoTemporal>;

                if (fotos != null && fotos.Count > 0)
                {
                    string carpetaServidor = Server.MapPath("~/imagen/producto/");

                    if (!Directory.Exists(carpetaServidor))
                        Directory.CreateDirectory(carpetaServidor);

                    foreach (FotoProductoTemporal foto in fotos)
                    {
                        string extension = Path.GetExtension(foto.Nombre);
                        string nombreArchivo = "producto_" + proId + "_" + Guid.NewGuid().ToString("N") + extension;
                        string rutaServidor = Path.Combine(carpetaServidor, nombreArchivo);
                        string rutaWeb = "~/imagen/producto/" + nombreArchivo;

                        File.WriteAllBytes(rutaServidor, foto.Bytes);

                        CN_tbl_producto_imagen.InsertarImagenProducto(
                            proId,
                            nombreArchivo,
                            rutaWeb
                        );
                    }
                }

                Session.Remove("fotos_producto");
                LimpiarCampos();

                MostrarModal("Producto registrado", "El producto fue guardado correctamente.");
            }
            catch (Exception ex)
            {
                MostrarModal("Error", ex.Message);
            }
        }

      
       

       

        private bool ValidarFormularioServidor()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtCantidad.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                ddlProveedor.SelectedValue == "0")
            {
                MostrarModal("Faltan campos", "Complete todos los campos obligatorios.");
                return false;
            }

            int cantidad;
            if (!int.TryParse(txtCantidad.Text.Trim(), out cantidad) || cantidad < 0)
            {
                MostrarModal("Cantidad inválida", "La cantidad debe ser un número válido.");
                return false;
            }

            decimal precio;
            if (!TryConvertirPrecio(txtPrecio.Text.Trim(), out precio) || precio <= 0)
            {
                MostrarModal("Precio inválido", "El precio debe ser mayor a cero y máximo con 2 decimales.");
                return false;
            }

            if (Session["fotos_producto"] == null)
            {
                MostrarModal("Imágenes requeridas", "Primero debe previsualizar al menos una imagen.");
                return false;
            }

            return true;
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
                    MostrarModal("Imagen muy pesada", "Cada imagen no debe superar los 5 MB.");
                    return false;
                }
            }

            return true;
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtCantidad.Text = "";
            txtPrecio.Text = "";
            ddlProveedor.SelectedIndex = 0;
            previewContainer.InnerHtml = "";
        }

        private void MostrarModal(string titulo, string mensaje)
        {
            titulo = titulo.Replace("'", "\\'");
            mensaje = mensaje.Replace("'", "\\'");

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                "mostrarModalProducto('" + titulo + "', '" + mensaje + "');",
                true
            );
        }

        private bool TryConvertirPrecio(string texto, out decimal precio)
        {
            texto = texto.Trim().Replace(",", ".");

            return decimal.TryParse(
                texto,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out precio
            );
        }

        private decimal ConvertirPrecio(string texto)
        {
            texto = texto.Trim().Replace(",", ".");

            return decimal.Parse(
                texto,
                NumberStyles.AllowDecimalPoint, 
                CultureInfo.InvariantCulture
            );
        }


    }
}