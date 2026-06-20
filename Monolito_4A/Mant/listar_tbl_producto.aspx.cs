using Capa_Datos;
using Capa_Negocio;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4A.Mant
{
    public partial class listar_tbl_producto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarSesionAdmin();

            if (!IsPostBack)
            {
                CargarAdministrador();
                CargarProductos();
            }
        }

        public class ProductoCsv
        {
            public int pro_id { get; set; }
            public string pro_nombre { get; set; }
            public int? pro_cantidad { get; set; }
            public decimal? pro_precio { get; set; }
            public char pro_estado { get; set; }
            public int? prov_id { get; set; }
            public string prov_nombre { get; set; }
            public string pimg_ruta { get; set; }

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

        private void CargarProductos()
        {
            using (var db = new DataClasses1DataContext())
            {
                var datos = (from p in db.tbl_producto
                             join pr in db.tbl_proveedor.Where(x => x.prov_estado == 'A')
                             on p.prov_id equals pr.prov_id into proveedores
                             from pr in proveedores.DefaultIfEmpty()
                             where p.pro_estado == 'A'
                             orderby p.pro_id descending
                             select new
                             {
                                 p.pro_id,
                                 p.pro_nombre,
                                 prov_nombre = pr == null ? "Sin proveedor" : pr.prov_nombre,
                                 p.pro_cantidad,
                                 p.pro_estado,
                                 p.pro_precio
                             }).ToList();

                var lista = datos.Select(p =>
                {
                    string ruta = ObtenerRutaImagenProducto(p.pro_id);

                    return new
                    {
                        p.pro_id,
                        p.pro_nombre,
                        p.prov_nombre,
                        p.pro_cantidad,
                        p.pro_estado,
                        p.pro_precio,
                        ImagenRuta = ruta,
                        ImagenDefault = ruta == "~/imagen/TMW.png"
                    };
                }).ToList();

                gvProductos.DataSource = lista;
                gvProductos.DataBind();
            }
        }

        private void BuscarProductos()
        {
            string texto = txtBuscar.Text.Trim();

            using (var db = new DataClasses1DataContext())
            {
                var datos = (from p in db.tbl_producto
                             join pr in db.tbl_proveedor.Where(x => x.prov_estado == 'A')
                             on p.prov_id equals pr.prov_id into proveedores
                             from pr in proveedores.DefaultIfEmpty()
                             where (p.pro_estado == 'A')
                             && (texto == "" || p.pro_nombre.Contains(texto))
                             orderby p.pro_id descending
                             select new
                             {
                                 p.pro_id,
                                 p.pro_nombre,
                                 prov_nombre = pr == null ? "Sin proveedor" : pr.prov_nombre,
                                 p.pro_cantidad,
                                 p.pro_estado,
                                 p.pro_precio
                             }).ToList();

                var lista = datos.Select(p =>
                {
                    string ruta = ObtenerRutaImagenProducto(p.pro_id);

                    return new
                    {
                        p.pro_id,
                        p.pro_nombre,
                        p.prov_nombre,
                        p.pro_cantidad,
                        p.pro_estado,
                        p.pro_precio,
                        ImagenRuta = ruta,
                        ImagenDefault = ruta == "~/imagen/TMW.png"
                    };
                }).ToList();

                gvProductos.DataSource = lista;
                gvProductos.DataBind();
            }
        }

        private string ObtenerRutaImagenProducto(int proId)
        {
            var imagen = CN_tbl_producto_imagen.ObtenerImagenPrincipal(proId);

            if (imagen != null && !string.IsNullOrWhiteSpace(imagen.pimg_ruta))
            {
                return imagen.pimg_ruta;
            }

            return "~/imagen/TMW.png";
        }



        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0;
            BuscarProductos();
        }

        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            BuscarProductos();
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarProducto")
            {
                Response.Redirect("~/Mant/editar_tbl_producto.aspx?id=" + id);
            }

            if (e.CommandName == "EstadisticasProducto")
            {
                Response.Redirect("~/Mant/estadisticas_producto.aspx?id=" + id);
            }

            if (e.CommandName == "EliminarProducto")
            {
                bool eliminado = CN_tbl_producto.EliminarProducto(id);

                lblMensaje.Text = eliminado
                    ? "Producto eliminado correctamente."
                    : "No se pudo eliminar el producto.";

                CargarProductos();
            }

            if (e.CommandName == "ImagenesProducto")
            {
                Response.Redirect("~/Mant/imagenes_producto.aspx?pro_id=" + id);
            }
        }
        //metodoexel 
        protected void btnPrevisualizarProductosCsv_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            if (!fuProductoCsv.HasFile)
            {
                lblMensaje.Text = "Debe seleccionar un archivo CSV.";
                return;
            }

            string extension = Path.GetExtension(fuProductoCsv.FileName).ToLower();

            if (extension != ".csv")
            {
                lblMensaje.Text = "Solo se permite archivo .csv.";
                return;
            }
            int maxMb = 50;
            int maxBytes = maxMb * 1024 * 1024;

            if (fuProductoCsv.PostedFile.ContentLength > maxBytes)
            {
                lblMensaje.Text = "El archivo CSV no debe superar " + maxMb + " MB.";
                return;
            }
            //db temporal guardar el scv
            DataTable dt = new DataTable();
            dt.Columns.Add("pro_id");
            dt.Columns.Add("pro_nombre");
            dt.Columns.Add("pro_cantidad");
            dt.Columns.Add("pro_precio");
            dt.Columns.Add("pro_estado");
            dt.Columns.Add("prov_id");
            dt.Columns.Add("prov_nombre");
            dt.Columns.Add("pimg_ruta");


            //crea y destruye
            //en forma de bytes
            //linea xlinea lee
            //transorma bytes a texto
            using (StreamReader sr = new StreamReader(fuProductoCsv.FileContent, Encoding.UTF8))
            {
                bool primeraLinea = true;

                while (!sr.EndOfStream)
                {
                    string linea = sr.ReadLine();

                    if (linea.Trim().ToLower().StartsWith("sep="))
                    {
                        continue;
                    }

                    if (primeraLinea)
                    {
                        primeraLinea = false;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    char separador = ',';

                    if (linea.Contains(";") && linea.Split(';').Length > linea.Split(',').Length)
                    {
                        separador = ';';
                    }
                    //liena a colum
                    string[] datos = linea.Split(separador);

                    if (datos.Length < 6)
                    {
                        lblMensaje.Text = "Fila inválida. Revise que el CSV tenga mínimo: pro_id, pro_nombre, pro_cantidad, pro_precio, pro_estado, prov_id. También puede estar separado por comas o punto y coma.";
                        return;
                    }

                    string provNombre = "Sin proveedor";
                    string rutaImagen = "";

                    if (datos.Length >= 7 && !string.IsNullOrWhiteSpace(datos[6]))
                    {
                        provNombre = datos[6].Trim();
                    }

                    if (datos.Length >= 8 && !string.IsNullOrWhiteSpace(datos[7]))
                    {
                        rutaImagen = datos[7].Trim();
                    }

                    dt.Rows.Add(
                        datos[0].Trim(),
                        datos[1].Trim(),
                        datos[2].Trim(),
                        datos[3].Trim(),
                        datos[4].Trim().ToUpper(),
                        datos[5].Trim(),
                        provNombre,
                        rutaImagen
                    );


                }
            }

            Session["preview_productos"] = dt;
            gvPreviewProductos.DataSource = dt;
            gvPreviewProductos.DataBind();
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "abrirPreviewCsv",
                "setTimeout(function(){ abrirPreviewCsv(); }, 200);",
                true
            );


            lblMensaje.Text = "Previsualización correcta. Registros listos: " + dt.Rows.Count;
        }

        protected void btnImportarProductosCsv_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
            DataTable dt = Session["preview_productos"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                lblMensaje.Text = "Primero debe previsualizar el archivo.";
                return;
            }

            int importados = 0;

            foreach (DataRow row in dt.Rows)
            {
                int proId = Convert.ToInt32(row["pro_id"]);
                string nombre = row["pro_nombre"].ToString();
                int cantidad = Convert.ToInt32(row["pro_cantidad"]);
                decimal precio = Convert.ToDecimal(row["pro_precio"], CultureInfo.InvariantCulture);
                char estado = Convert.ToChar(row["pro_estado"].ToString());
                string provNombre = row["prov_nombre"].ToString();
                string rutaImagen = row["pimg_ruta"].ToString();
                int? provId = null;
                string provTexto = row["prov_id"].ToString();

                if (!string.IsNullOrWhiteSpace(provTexto))
                    provId = Convert.ToInt32(provTexto);

                CN_tbl_producto.ImportarProducto(proId, nombre, cantidad, precio, estado, provId, provNombre);
                if (!string.IsNullOrWhiteSpace(rutaImagen) && rutaImagen != "~/imagen/TMW.png")
                {
                    string nombreImagen = Path.GetFileName(rutaImagen);

                    CN_tbl_producto_imagen.InsertarImagenProducto(
                        proId,
                        nombreImagen,
                        rutaImagen
                    );
                }
                importados++;
            }

            Session["preview_productos"] = null;
            gvPreviewProductos.DataSource = null;
            gvPreviewProductos.DataBind();

            CargarProductos();

            lblMensaje.Text = "Productos importados: " + importados + ". Si algún producto no tiene imagen, agréguela desde el botón 🖼.";
        }


        protected void btnDescargarProductosCsv_Click(object sender, EventArgs e)
        {
            using (var db = new DataClasses1DataContext())
            {
                var productos = db.ExecuteQuery<ProductoCsv>(
               @"SELECT 
                    p.pro_id,
                    p.pro_nombre,
                    p.pro_cantidad,
                    p.pro_precio,
                    p.pro_estado,
                    ISNULL(p.prov_id, p.prov_id_respaldo) AS prov_id,
                    ISNULL(pr.prov_nombre, 'Sin proveedor') AS prov_nombre,
                    ISNULL((
                        SELECT TOP 1 pi.pimg_ruta
                        FROM tbl_producto_imagen pi
                        WHERE pi.pro_id = p.pro_id
                        AND pi.pimg_estado = 'A'
                        ORDER BY pi.pimg_id ASC
                    ), '~/imagen/TMW.png') AS pimg_ruta
                FROM tbl_producto p
                LEFT JOIN tbl_proveedor pr
                ON ISNULL(p.prov_id, p.prov_id_respaldo) = pr.prov_id
                AND pr.prov_estado = 'A'
                ORDER BY p.pro_id"
               ).ToList();

                Response.Clear();
                Response.Buffer = true;
                Response.ContentEncoding = Encoding.UTF8;
                Response.AddHeader("content-disposition", "attachment;filename=productos.csv");
                Response.Charset = "utf-8";
                Response.ContentType = "text/csv";

                Response.Write("sep=,\n");
                Response.Write("pro_id,pro_nombre,pro_cantidad,pro_precio,pro_estado,prov_id,prov_nombre,pimg_ruta\n");
                foreach (var p in productos)
                {
                    Response.Write(
                      p.pro_id + "," +
                      LimpiarCsv(p.pro_nombre) + "," +
                      p.pro_cantidad + "," +
                      Convert.ToDecimal(p.pro_precio).ToString("0.00", CultureInfo.InvariantCulture) + "," +
                      p.pro_estado + "," +
                      p.prov_id + "," +
                      LimpiarCsv(p.prov_nombre) + "," +
                      LimpiarCsv(p.pimg_ruta) + "\n"
                  );
                }

                Response.Flush();
                Response.End();
            }
        }
        private string LimpiarCsv(string texto)
        {
            if (texto == null)
                return "";

            return texto.Replace(",", " ").Replace("\n", " ").Replace("\r", " ").Trim();
        }

        protected void gvProductos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool imagenDefault = Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem, "ImagenDefault"));
                string proveedor = DataBinder.Eval(e.Row.DataItem, "prov_nombre").ToString();

                if (imagenDefault)
                {
                    e.Row.CssClass += " fila-sin-foto";
                }

                if (proveedor == "Sin proveedor")
                {
                    e.Row.CssClass += " fila-sin-proveedor";
                }
            }
        }
    }
}