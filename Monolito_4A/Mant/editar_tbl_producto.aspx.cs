using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Globalization;

namespace Monolito_4A.Mant
{
    public partial class editar_tbl_producto : System.Web.UI.Page
    {
        private const int MAX_FOTO_BYTES = 5 * 1024 * 1024;

        [Serializable]
        public class FotoProductoTemporal
        {
            public byte[] Bytes { get; set; }
            public string Nombre { get; set; }
            public string Tipo { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarSesionAdmin();

            if (!IsPostBack)
            {
                CargarAdministrador();
                CargarProveedores();
                CargarProducto();
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
            ddlProveedor.DataSource = CN_tbl_proveedor.ListarProveedor();
            ddlProveedor.DataTextField = "prov_nombre";
            ddlProveedor.DataValueField = "prov_id";
            ddlProveedor.DataBind();

            ddlProveedor.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Seleccione proveedor", "0"));
        }

        private void CargarProducto()
        {
            if (Request.QueryString["id"] == null)
            {
                Response.Redirect("~/Mant/listar_tbl_producto.aspx");
                return;
            }

            int id = Convert.ToInt32(Request.QueryString["id"]);
            tbl_producto producto = CN_tbl_producto.ObtenerProducto(id);

            if (producto == null)
            {
                Response.Redirect("~/Mant/listar_tbl_producto.aspx");
                return;
            }

            hfProductoId.Value = producto.pro_id.ToString();
            txtNombre.Text = producto.pro_nombre;
            txtCantidad.Text = producto.pro_cantidad.ToString();
            txtPrecio.Text = Convert.ToDecimal(producto.pro_precio)
                .ToString("0.00", CultureInfo.InvariantCulture);

            if (producto.prov_id != null)
            {
                ddlProveedor.SelectedValue = producto.prov_id.ToString();
            }

            CargarImagenActual(producto.pro_id);
        }

        private void CargarImagenActual(int proId)
        {
            var imagen = CN_tbl_producto_imagen.ObtenerImagenPrincipal(proId);

            previewContainer.InnerHtml = "";

            if (imagen != null && !string.IsNullOrWhiteSpace(imagen.pimg_ruta))
            {
                previewContainer.InnerHtml =
                    "<img src='" + ResolveUrl(imagen.pimg_ruta) + "' " +
                    "class='img-preview' onclick='abrirModalImagenDesdeSrc(this.src)' />";
            }
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            if (!fuFotos.HasFiles)
            {
                MostrarModal("Imágenes requeridas", "Seleccione al menos una imagen para previsualizar.");
                return;
            }

            if (!ValidarFotosServidor())
            {
                return;
            }

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

            Session["fotos_producto_editar"] = fotos;

            MostrarModal("Imágenes cargadas", "Las imágenes fueron previsualizadas correctamente.");
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormularioServidor())
            {
                return;
            }

            try
            {
                int id = Convert.ToInt32(hfProductoId.Value);
                string nombre = txtNombre.Text.Trim();
                int cantidad = Convert.ToInt32(txtCantidad.Text.Trim());
                decimal precio = ConvertirPrecio(txtPrecio.Text.Trim());
                int provId = Convert.ToInt32(ddlProveedor.SelectedValue);

                bool actualizado = CN_tbl_producto.ActualizarProducto(id, nombre, cantidad, precio, provId);

                if (!actualizado)
                {
                    MostrarModal("Error", "No se pudo actualizar el producto.");
                    return;
                }

                List<FotoProductoTemporal> fotos = Session["fotos_producto_editar"] as List<FotoProductoTemporal>;

                if (fotos != null && fotos.Count > 0)
                {
                    CN_tbl_producto_imagen.InactivarImagenesProducto(id);

                    string carpetaServidor = Server.MapPath("~/imagen/producto/");

                    if (!Directory.Exists(carpetaServidor))
                    {
                        Directory.CreateDirectory(carpetaServidor);
                    }

                    foreach (FotoProductoTemporal foto in fotos)
                    {
                        string extension = Path.GetExtension(foto.Nombre);
                        string nombreArchivo = "producto_" + id + "_" + Guid.NewGuid().ToString("N") + extension;
                        string rutaServidor = Path.Combine(carpetaServidor, nombreArchivo);
                        string rutaWeb = "~/imagen/producto/" + nombreArchivo;

                        File.WriteAllBytes(rutaServidor, foto.Bytes);

                        CN_tbl_producto_imagen.InsertarImagenProducto(
                            id,
                            nombreArchivo,
                            rutaWeb
                        );
                    }

                    Session.Remove("fotos_producto_editar");
                }

                MostrarModal("Producto actualizado", "El producto fue actualizado correctamente.");
                CargarImagenActual(id);
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

        private void MostrarModal(string titulo, string mensaje)
        {
            titulo = titulo.Replace("'", "\\'");
            mensaje = mensaje.Replace("'", "\\'");

            ScriptManager.RegisterStartupScript(
                UpdatePanel1,
                UpdatePanel1.GetType(),
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