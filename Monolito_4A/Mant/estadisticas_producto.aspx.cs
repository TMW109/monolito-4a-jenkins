using System;
using System.Linq;
using Capa_Datos;
using Capa_Negocio;

namespace Monolito_4A.Mant
{
    public partial class estadisticas_producto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarSesionAdmin();

            if (!IsPostBack)
            {
                CargarAdministrador();
                CargarProducto();
                CargarEstadisticasGenerales();
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

            decimal precio = producto.pro_precio == null ? 0 : Convert.ToDecimal(producto.pro_precio);
            int cantidad = producto.pro_cantidad == null ? 0 : Convert.ToInt32(producto.pro_cantidad);
            decimal total = precio * cantidad;

            lblId.Text = producto.pro_id.ToString();
            lblProducto.Text = producto.pro_nombre;
            lblCantidad.Text = cantidad.ToString();
            lblPrecio.Text = precio.ToString("N2");
            lblTotal.Text = total.ToString("N2");

            hfCantidad.Value = cantidad.ToString();
            hfPrecio.Value = precio.ToString(System.Globalization.CultureInfo.InvariantCulture);
            hfTotal.Value = total.ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (producto.prov_id != null)
            {
                tbl_proveedor proveedor = CN_tbl_proveedor.ObtenerProveedor(Convert.ToInt32(producto.prov_id));

                lblProveedor.Text = proveedor != null
                    ? proveedor.prov_nombre
                    : "Sin proveedor";
            }
            else
            {
                lblProveedor.Text = "Sin proveedor";
            }

            var imagenes = CN_tbl_producto_imagen.ListarImagenesProducto(id);

            if (imagenes.Count == 0)
            {
                lblMensaje.Text = "Este producto no tiene imágenes registradas.";
            }

            rpImagenes.DataSource = imagenes;
            rpImagenes.DataBind();
        }

        private void CargarEstadisticasGenerales()
        {
            var productos = CN_tbl_producto.ListarProducto();

            if (productos == null || productos.Count == 0)
            {
                lblMayorStock.Text = "Sin datos";
                lblMasCaro.Text = "Sin datos";
                lblProductosActivos.Text = "0";
                lblInventarioGeneral.Text = "0.00";
                return;
            }

            var mayorStock = productos
                .OrderByDescending(p => p.pro_cantidad ?? 0)
                .FirstOrDefault();

            var masCaro = productos
                .OrderByDescending(p => p.pro_precio ?? 0)
                .FirstOrDefault();

            decimal inventarioGeneral = productos.Sum(p =>
                (p.pro_precio ?? 0) * (p.pro_cantidad ?? 0)
            );

            lblMayorStock.Text = mayorStock != null
                ? mayorStock.pro_nombre + " (" + mayorStock.pro_cantidad + ")"
                : "Sin datos";

            lblMasCaro.Text = masCaro != null
                ? masCaro.pro_nombre + " ($ " + Convert.ToDecimal(masCaro.pro_precio).ToString("N2") + ")"
                : "Sin datos";

            lblProductosActivos.Text = productos.Count.ToString();
            lblInventarioGeneral.Text = inventarioGeneral.ToString("N2");

            var proveedores = CN_tbl_proveedor.ListarProveedor();

            var lista = proveedores.Select(prov => new
            {
                Proveedor = prov.prov_nombre,
                CantidadProductos = productos.Count(p => p.prov_id == prov.prov_id)
            })
            .OrderByDescending(x => x.CantidadProductos)
            .ToList();

            gvProductosProveedor.DataSource = lista;
            gvProductosProveedor.DataBind();
        }
    }
}