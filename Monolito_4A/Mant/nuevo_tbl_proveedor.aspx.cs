using Capa_Datos;
using Capa_Negocio;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Monolito_4A.Mant
{
    public partial class nuevo_tbl_proveedor : System.Web.UI.Page
    {
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
            gvProveedores.DataSource = CN_tbl_proveedor.ListarProveedor();
            gvProveedores.DataBind();
        }

        private void BuscarProveedores()
        {
            string texto = txtBuscar.Text.Trim();

            if (texto == "")
            {
                CargarProveedores();
            }
            else
            {
                gvProveedores.DataSource = CN_tbl_proveedor.BuscarProveedor(texto);
                gvProveedores.DataBind();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreProveedor.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarModal("Faltan campos", "Ingrese el nombre del proveedor.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(nombre, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s\.\-]+$"))
            {
                MostrarModal("Nombre inválido", "El nombre solo debe contener letras, números, espacios, punto o guion.");
                return;
            }

            bool resultado;

            if (string.IsNullOrWhiteSpace(hfProveedorId.Value))
            {
                resultado = CN_tbl_proveedor.InsertarProveedor(nombre);

                lblMensaje.Text = resultado
                    ? "Proveedor registrado correctamente."
                    : "No se pudo registrar el proveedor.";

                if (resultado)
                    MostrarModal("Proveedor registrado", "El proveedor fue guardado correctamente.");
            }
            else
            {
                int id = Convert.ToInt32(hfProveedorId.Value);
                char estado = Convert.ToChar(ddlEstado.SelectedValue);

                resultado = CN_tbl_proveedor.ActualizarProveedor(id, nombre, estado);

                lblMensaje.Text = resultado
                    ? "Proveedor actualizado correctamente."
                    : "No se pudo actualizar el proveedor.";

                if (resultado)
                    MostrarModal("Proveedor actualizado", "Los datos fueron actualizados correctamente.");
            }

            if (resultado)
            {
                LimpiarCampos();
                CargarProveedores();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = 0;
            BuscarProveedores();
        }

        protected void gvProveedores_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProveedores.PageIndex = e.NewPageIndex;
            BuscarProveedores();
        }

        protected void gvProveedores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarProveedor")
            {
                tbl_proveedor proveedor = CN_tbl_proveedor.ObtenerProveedor(id);

                if (proveedor != null)
                {
                    hfProveedorId.Value = proveedor.prov_id.ToString();
                    txtNombreProveedor.Text = proveedor.prov_nombre;
                    ddlEstado.SelectedValue = proveedor.prov_estado.ToString();
                    boxEstado.Visible = true;
                    btnGuardar.Text = "Actualizar proveedor";
                    lblMensaje.Text = "Editando proveedor ID: " + proveedor.prov_id;
                }
            }

            if (e.CommandName == "EliminarProveedor")
            {
                bool eliminado = CN_tbl_proveedor.EliminarProveedor(id);

                lblMensaje.Text = eliminado
                    ? "Proveedor eliminado correctamente."
                    : "No se pudo eliminar el proveedor.";

                if (eliminado)
                {
                    MostrarModal("Proveedor eliminado", "El proveedor fue eliminado correctamente.");
                    LimpiarCampos();
                    CargarProveedores();
                }
            }
        }

        protected void btnPrevisualizarProveedor_Click(object sender, EventArgs e)
        {
            if (!fuProveedor.HasFile)
            {
                MostrarModal("Archivo requerido", "Debe seleccionar un archivo Excel o CSV.");
                return;
            }

            string extension = Path.GetExtension(fuProveedor.FileName).ToLower();
            string[] extensionesPermitidas = { ".csv", ".xls", ".xlsx" };

            if (!extensionesPermitidas.Contains(extension))
            {
                MostrarModal("Archivo inválido", "Solo se permiten archivos Excel o CSV: .csv, .xls, .xlsx.");
                return;
            }

            int maxMb = 50;
            int maxBytes = maxMb * 1024 * 1024;

            if (fuProveedor.PostedFile.ContentLength > maxBytes)
            {
                MostrarModal("Archivo muy pesado", "El archivo no debe superar " + maxMb + " MB.");
                return;
            }

            if (extension != ".csv")
            {
                MostrarModal("Formato no soportado todavía", "Por ahora la lectura está implementada para CSV. Guarde el Excel como CSV UTF-8 e inténtelo nuevamente.");
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("prov_id");
            dt.Columns.Add("prov_nombre");
            dt.Columns.Add("prov_estado");

            try
            {
                
                using (StreamReader sr = new StreamReader(fuProveedor.FileContent, Encoding.UTF8))
                {
                    bool primeraLinea = true;
                    int fila = 1;

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
                            fila++;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(linea))
                        {
                            fila++;
                            continue;
                        }

                        char separador = ',';

                        if (linea.Contains(";") && linea.Split(';').Length > linea.Split(',').Length)
                        {
                            separador = ';';
                        }

                        string[] datos = linea.Split(separador);
                        if (datos.Length < 3)
                        {
                            MostrarModal("Error en CSV", "La fila " + fila + " no tiene las 3 columnas requeridas.");
                            return;
                        }

                        int provIdTemp;
                        if (!int.TryParse(datos[0].Trim(), out provIdTemp))
                        {
                            MostrarModal("ID inválido", "El prov_id de la fila " + fila + " no es numérico.");
                            return;
                        }

                        string nombre = datos[1].Trim();
                        string estado = datos[2].Trim().ToUpper();

                        if (nombre == "")
                        {
                            MostrarModal("Nombre vacío", "El nombre del proveedor en la fila " + fila + " está vacío.");
                            return;
                        }

                        if (estado != "A" && estado != "I")
                        {
                            MostrarModal("Estado inválido", "El estado en la fila " + fila + " debe ser A o I.");
                            return;
                        }

                        dt.Rows.Add(
                            provIdTemp.ToString(),
                            nombre,
                            estado
                        );

                        fila++;
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    MostrarModal("Archivo vacío", "El archivo no contiene proveedores para importar.");
                    return;
                }

                Session["preview_proveedores"] = dt;

                gvPreviewProveedor.DataSource = dt;
                gvPreviewProveedor.DataBind();

                MostrarModal("Previsualización lista", "Revise los proveedores antes de importar.");
            }
            catch
            {
                MostrarModal("Error al leer archivo", "No se pudo leer el archivo. Verifique que sea un CSV válido.");
            }
        }

        protected void btnImportarProveedor_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["preview_proveedores"] as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                MostrarModal("Sin datos", "Primero debe previsualizar el archivo.");
                return;
            }

            int importados = 0;

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    int prov_id = Convert.ToInt32(row["prov_id"]);
                    string prov_nombre = row["prov_nombre"].ToString();
                    char prov_estado = Convert.ToChar(row["prov_estado"]);

                    CN_tbl_proveedor.ImportarProveedor(prov_id, prov_nombre, prov_estado);
                    importados++;
                }

                Session["preview_proveedores"] = null;
                gvPreviewProveedor.DataSource = null;
                gvPreviewProveedor.DataBind();

                CargarProveedores();

                MostrarModal("Importación completa", "Proveedores importados: " + importados);
            }
            catch
            {
                MostrarModal("Error al importar", "No se pudo importar el archivo. Revise los datos del CSV.");
            }
        }

        protected void btnDescargarProveedor_Click(object sender, EventArgs e)
        {
            var proveedores = CN_tbl_proveedor.ListarProveedor();

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = Encoding.UTF8;
            Response.AddHeader("content-disposition", "attachment;filename=proveedores.csv");
            Response.Charset = "utf-8";
            Response.ContentType = "text/csv";

            Response.Write("sep=,\n");
            Response.Write("prov_id,prov_nombre,prov_estado\n");

            foreach (var p in proveedores)
            {
                Response.Write(p.prov_id + "," + p.prov_nombre + "," + p.prov_estado + "\n");
            }

            Response.Flush();
            Response.End();
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            gvProveedores.PageIndex = 0;
            BuscarProveedores();
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetRevalidation(System.Web.HttpCacheRevalidation.AllCaches);
            Response.Cache.SetValidUntilExpires(false);

            Response.Redirect("~/Seguridad/Login.aspx");
        }

        private void LimpiarCampos()
        {
            hfProveedorId.Value = "";
            txtNombreProveedor.Text = "";
            btnGuardar.Text = "Guardar proveedor";
            txtBuscar.Text = "";

            boxEstado.Visible = false;
            ddlEstado.SelectedValue = "A";
        }

        private void MostrarModal(string titulo, string mensaje)
        {
            titulo = titulo.Replace("'", "\\'");
            mensaje = mensaje.Replace("'", "\\'");

            string script = "mostrarModalProveedor('" + titulo + "', '" + mensaje + "');";

            ScriptManager.RegisterStartupScript(
                Page,
                Page.GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }
    }
}