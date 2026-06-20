using Capa_Datos;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4A.Mant
{
    public partial class imagenes_producto : System.Web.UI.Page
    {
        private int proId;
        private const int MAX_FOTO_BYTES = 5 * 1024 * 1024;

        [Serializable]
        public class ImagenTemporal
        {
            public byte[] Bytes { get; set; }
            public string NombreArchivo { get; set; }
            public string ContentType { get; set; }
            public string Extension { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));

            if (Request.QueryString["pro_id"] == null ||
                !int.TryParse(Request.QueryString["pro_id"], out proId))
            {
                Response.Redirect("~/Mant/listar_tbl_producto.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarProducto();
                CargarImagenes();
                Session.Remove("imagenes_producto_preview");
                Session.Remove("preview_imagenes_csv");
            }
        }

        private void CargarProducto()
        {
            tbl_producto producto = CN_tbl_producto.ObtenerProducto(proId);

            if (producto == null)
            {
                Response.Redirect("~/Mant/listar_tbl_producto.aspx");
                return;
            }

            lblProducto.Text = producto.pro_nombre;
        }

        private void CargarImagenes()
        {
            gvImagenes.DataSource = CN_tbl_producto_imagen.ListarImagenesProducto(proId);
            gvImagenes.DataBind();
        }

        protected void gvImagenes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvImagenes.PageIndex = e.NewPageIndex;
            CargarImagenes();
        }

        protected void btnPrevisualizar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarModal("Nombre requerido", "Ingrese el nombre de la imagen antes de previsualizar.");
                return;
            }

            if (!fuImagen.HasFiles)
            {
                MostrarModal("Imagen requerida", "Seleccione al menos una imagen para previsualizar.");
                return;
            }



            if (!ValidarImagenesServidor())
                return;

            List<ImagenTemporal> imagenes = new List<ImagenTemporal>();
            previewContainer.InnerHtml = "";

            foreach (HttpPostedFile archivo in fuImagen.PostedFiles)
            {
                byte[] bytes = new byte[archivo.ContentLength];
                archivo.InputStream.Read(bytes, 0, archivo.ContentLength);

                ImagenTemporal temporal = new ImagenTemporal
                {
                    Bytes = bytes,
                    NombreArchivo = Path.GetFileName(archivo.FileName),
                    ContentType = archivo.ContentType,
                    Extension = Path.GetExtension(archivo.FileName).ToLower()
                };

                imagenes.Add(temporal);

                string base64 = Convert.ToBase64String(bytes);

                previewContainer.InnerHtml +=
                    "<img src='data:" + archivo.ContentType + ";base64," + base64 + "' " +
                    "class='preview-image' onclick='abrirModalImagenDesdeSrc(this.src)' />";
            }

            Session["imagenes_producto_preview"] = imagenes;
            hfPrevisualizada.Value = "1";

            MostrarModal("Imágenes cargadas", "Las imágenes fueron previsualizadas correctamente. Ahora puede guardarlas.");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarModal("Nombre requerido", "Ingrese el nombre de la imagen.");
                return;
            }

            if (nombre.Length < 3)
            {
                MostrarModal("Nombre muy corto", "El nombre debe tener al menos 3 caracteres.");
                return;
            }

            bool editando = !string.IsNullOrEmpty(hfImagenId.Value);

            if (!editando)
            {
                if (hfPrevisualizada.Value != "1" || Session["imagenes_producto_preview"] == null)
                {
                    MostrarModal("Previsualización requerida", "Primero previsualice la imagen antes de guardarla.");
                    return;
                }

                List<ImagenTemporal> imagenes = Session["imagenes_producto_preview"] as List<ImagenTemporal>;

                if (imagenes == null || imagenes.Count == 0)
                {
                    MostrarModal("Previsualización requerida", "Primero previsualice la imagen antes de guardarla.");
                    return;
                }

                int contador = 1;

                foreach (ImagenTemporal img in imagenes)
                {
                    string ruta = GuardarArchivoImagenDesdeTemporal(img);

                    string nombreFinal = imagenes.Count == 1
                        ? nombre
                        : nombre + " " + contador;

                    bool existeImagen;

                    using (var db = new DataClasses1DataContext())
                    {
                        existeImagen = db.tbl_producto_imagen.Any(i =>
                            i.pro_id == proId &&
                            i.pimg_ruta == ruta &&
                            i.pimg_estado == 'A'
                        );
                    }

                    if (!existeImagen)
                    {
                        CN_tbl_producto_imagen.InsertarImagenProducto(proId, nombreFinal, ruta);
                    }

                    contador++;
                }

                LimpiarFormulario();
                CargarImagenes();

                lblMensaje.Text = "Imágenes registradas correctamente.";
                MostrarModal("Registro exitoso", "Las imágenes fueron registradas correctamente.");
            }
            else
            {
                int id;

                if (!int.TryParse(hfImagenId.Value, out id))
                {
                    MostrarModal("Error", "No se pudo identificar la imagen a editar.");
                    return;
                }

                string ruta;

                if (fuImagen.HasFiles && hfPrevisualizada.Value != "1")
                {
                    MostrarModal("Previsualización requerida", "Primero previsualice la nueva imagen antes de actualizar.");
                    return;
                }

                if (hfPrevisualizada.Value == "1" && Session["imagenes_producto_preview"] != null)
                {
                    List<ImagenTemporal> imagenes = Session["imagenes_producto_preview"] as List<ImagenTemporal>;

                    if (imagenes == null || imagenes.Count == 0)
                    {
                        MostrarModal("Previsualización requerida", "Primero previsualice la nueva imagen antes de actualizar.");
                        return;
                    }

                    if (imagenes.Count > 1)
                    {
                        MostrarModal("Edición individual", "Para editar una imagen solo seleccione una nueva imagen.");
                        return;
                    }

                    ruta = GuardarArchivoImagenDesdeTemporal(imagenes[0]);
                }
                else
                {
                    var imgActual = CN_tbl_producto_imagen.ObtenerImagen(id);

                    if (imgActual == null)
                    {
                        MostrarModal("Error", "La imagen seleccionada ya no existe.");
                        return;
                    }

                    ruta = imgActual.pimg_ruta;
                }

                CN_tbl_producto_imagen.ActualizarImagenProducto(id, nombre, ruta);

                LimpiarFormulario();
                CargarImagenes();

                lblMensaje.Text = "Imagen actualizada correctamente.";
                MostrarModal("Actualización exitosa", "La imagen fue actualizada correctamente.");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblMensaje.Text = "";
        }

        protected void gvImagenes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;

            if (!int.TryParse(e.CommandArgument.ToString(), out id))
            {
                MostrarModal("Error", "No se pudo identificar la imagen.");
                return;
            }

            if (e.CommandName == "EditarImagen")
            {
                var imagen = CN_tbl_producto_imagen.ObtenerImagen(id);

                if (imagen != null)
                {
                    Session.Remove("imagenes_producto_preview");

                    hfImagenId.Value = imagen.pimg_id.ToString();
                    hfPrevisualizada.Value = "0";
                    txtNombre.Text = imagen.pimg_nombre;
                    btnGuardar.Text = "Actualizar imagen";

                    previewContainer.InnerHtml =
                        "<img src='" + ResolveUrl(imagen.pimg_ruta) + "' " +
                        "class='preview-image' onclick='abrirModalImagenDesdeSrc(this.src)' />";

                    lblMensaje.Text = "Modo edición activo. Puede cambiar el nombre o previsualizar una nueva imagen.";
                }
            }

            if (e.CommandName == "EliminarImagen")
            {
                bool eliminado = CN_tbl_producto_imagen.EliminarImagenProducto(id);

                if (eliminado)
                {
                    LimpiarFormulario();
                    CargarImagenes();
                    lblMensaje.Text = "Imagen eliminada correctamente.";
                }
                else
                {
                    MostrarModal("Error", "No se pudo eliminar la imagen.");
                }
            }
        }

        private string GuardarArchivoImagenDesdeTemporal(ImagenTemporal temporal)
        {
            string carpeta = Server.MapPath("~/imagen/producto/");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string archivo = "producto_" + proId + "_" + Guid.NewGuid().ToString("N") + temporal.Extension;
            string rutaFisica = Path.Combine(carpeta, archivo);

            File.WriteAllBytes(rutaFisica, temporal.Bytes);

            return "~/imagen/producto/" + archivo;
        }

        private bool ValidarImagenesServidor()
        {
            foreach (HttpPostedFile archivo in fuImagen.PostedFiles)
            {
                string extension = Path.GetExtension(archivo.FileName).ToLower();
                string tipo = archivo.ContentType.ToLower();

                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png" &&
                    extension != ".webp")
                {
                    MostrarModal("Archivo no permitido", "Solo se permiten imágenes JPG, PNG o WEBP.");
                    return false;
                }

                if (tipo != "image/jpeg" &&
                    tipo != "image/png" &&
                    tipo != "image/webp")
                {
                    MostrarModal("Archivo inválido", "El archivo seleccionado no es una imagen válida.");
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

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            hfImagenId.Value = "";
            hfPrevisualizada.Value = "0";
            btnGuardar.Text = "Guardar imagen";
            previewContainer.InnerHtml = "";
            Session.Remove("imagenes_producto_preview");
        }

        protected void btnDescargarImagenCsv_Click(object sender, EventArgs e)
        {
            var imagenes = CN_tbl_producto_imagen.ListarImagenesProducto(proId);

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attachment;filename=imagenes_producto_" + proId + ".csv");

            Response.Write("sep=,\n");
            Response.Write("pimg_id,pro_id,pimg_nombre,pimg_ruta,pimg_estado\n");

            foreach (var img in imagenes)
            {
                Response.Write(
                    img.pimg_id + "," +
                    LimpiarCsv(img.pro_id.ToString()) + "," +
                    LimpiarCsv(img.pimg_nombre) + "," +
                    LimpiarCsv(img.pimg_ruta) + "," +
                    img.pimg_estado + "\n"
                );
            }

            Response.End();
        }

        protected void btnPrevisualizarImagenCsv_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";

            if (!fuImagenCsv.HasFile)
            {
                MostrarModal("Archivo requerido", "Seleccione un archivo CSV de imágenes.");
                return;
            }
            string extension = Path.GetExtension(fuImagenCsv.FileName).ToLower();

            if (extension != ".csv")
            {
                MostrarModal("Archivo inválido", "Solo se permite archivo .csv.");
                return;
            }

            int maxMb = 10;
            int maxBytes = maxMb * 1024 * 1024;

            if (fuImagenCsv.PostedFile.ContentLength > maxBytes)
            {
                MostrarModal("Archivo muy pesado", "El CSV de imágenes no debe superar " + maxMb + " MB.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("pimg_id");
            dt.Columns.Add("pro_id");
            dt.Columns.Add("pimg_nombre");
            dt.Columns.Add("pimg_ruta");
            dt.Columns.Add("pimg_estado");

            using (StreamReader sr = new StreamReader(fuImagenCsv.FileContent, Encoding.UTF8))
            {
                bool primeraLinea = true;

                while (!sr.EndOfStream)
                {
                    string linea = sr.ReadLine();

                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    if (linea.Trim().ToLower().StartsWith("sep="))
                        continue;

                    if (primeraLinea)
                    {
                        primeraLinea = false;
                        continue;
                    }

                    char separador = ',';

                    if (linea.Contains(";") && linea.Split(';').Length > linea.Split(',').Length)
                        separador = ';';

                    string[] datos = linea.Split(separador);

                    if (datos.Length < 2)
                    {
                        MostrarModal("CSV inválido", "El CSV debe tener mínimo pimg_id y pro_id.");
                        return;
                    }

                    string pimgId = datos[0].Trim();
                    string productoId = datos[1].Trim();

                    int productoCsv;

                    if (!int.TryParse(productoId, out productoCsv))
                    {
                        MostrarModal("Producto inválido", "El pro_id debe ser numérico.");
                        return;
                    }

                    if (productoCsv != proId)
                    {
                        MostrarModal(
                            "Producto incorrecto",
                            "Este CSV pertenece al producto ID " + proId +
                            ". No puede importar imágenes del producto ID " + productoCsv + " desde esta pantalla."
                        );
                        return;
                    }

                    string nombre = datos.Length >= 3 ? datos[2].Trim() : "TMW.png";
                    string ruta = datos.Length >= 4 ? datos[3].Trim() : "~/imagen/TMW.png";
                    string estado = datos.Length >= 5 ? datos[4].Trim().ToUpper() : "A";

                    if (string.IsNullOrWhiteSpace(nombre))
                        nombre = "TMW.png";

                    if (string.IsNullOrWhiteSpace(ruta))
                        ruta = "~/imagen/TMW.png";

                    if (estado != "A" && estado != "I")
                    {
                        MostrarModal("Estado inválido", "El estado debe ser A o I.");
                        return;
                    }

                    dt.Rows.Add(pimgId, productoId, nombre, ruta, estado);
                }
            }

            Session["preview_imagenes_csv"] = dt;
            gvPreviewImagenCsv.DataSource = dt;
            gvPreviewImagenCsv.DataBind();

            lblMensaje.Text = "Previsualización correcta. Registros: " + dt.Rows.Count;
        }

        protected void btnImportarImagenCsv_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["preview_imagenes_csv"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarModal("Sin datos", "Primero debe previsualizar el CSV.");
                return;
            }

            int importadas = 0;
            int duplicadas = 0;

            using (var db = new DataClasses1DataContext())
            {
                foreach (DataRow row in dt.Rows)
                {
                    int pimgId;
                    int productoId;

                    if (!int.TryParse(row["pimg_id"].ToString(), out pimgId))
                        continue;

                    if (!int.TryParse(row["pro_id"].ToString(), out productoId))
                        continue;

                    if (productoId != proId)
                    {
                        MostrarModal("Producto incorrecto", "Solo puede importar imágenes del producto ID " + proId + ".");
                        return;
                    }

                    bool productoExiste = db.tbl_producto.Any(p => p.pro_id == productoId);

                    if (!productoExiste)
                        continue;

                    string nombre = row["pimg_nombre"].ToString();
                    string ruta = row["pimg_ruta"].ToString();
                    char estado = Convert.ToChar(row["pimg_estado"].ToString());

                    bool existeImagen = db.tbl_producto_imagen.Any(i =>
                        i.pro_id == productoId &&
                        i.pimg_ruta == ruta &&
                        i.pimg_estado == 'A' &&
                        i.pimg_id != pimgId
                    );

                    if (existeImagen)
                    {
                        duplicadas++;
                        continue;
                    }

                    db.sp_ImportarProductoImagen(
                        pimgId,
                        productoId,
                        nombre,
                        ruta,
                        estado
                    );

                    importadas++;
                }

                db.SubmitChanges();
            }

            Session["preview_imagenes_csv"] = null;
            gvPreviewImagenCsv.DataSource = null;
            gvPreviewImagenCsv.DataBind();

            CargarImagenes();

            MostrarModal("Importación completa", "Imágenes importadas: " + importadas + ". Duplicadas omitidas: " + duplicadas + ".");
        }

        private string LimpiarCsv(string texto)
        {
            if (texto == null)
                return "";

            return texto.Replace(",", " ").Replace("\n", " ").Replace("\r", " ").Trim();
        }

        protected void gvImagenes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ruta = DataBinder.Eval(e.Row.DataItem, "pimg_ruta").ToString();

                if (ruta == "~/imagen/TMW.png")
                {
                    e.Row.CssClass += " fila-imagen-pendiente";
                }
            }
        }

        private void MostrarModal(string titulo, string mensaje)
        {
            titulo = titulo.Replace("'", "\\'");
            mensaje = mensaje.Replace("'", "\\'");

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                Guid.NewGuid().ToString(),
                "mostrarModalMensaje('" + titulo + "', '" + mensaje + "');",
                true
            );
        }
    }
}