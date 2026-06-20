<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="listar_tbl_producto.aspx.cs" Inherits="Monolito_4A.Mant.listar_tbl_producto" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>Listar productos | Requiem</title>

    <link href="<%= ResolveUrl("~/css/listprodu.css") %>" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">

        <div class="admin-layout">

            <aside class="sidebar">

                <div class="brand">
                    <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" />
                    <h2>REQUIEM</h2>
                    <span>Security System</span>
                </div>

                <nav class="menu">
                    <a href="<%= ResolveUrl("~/admin.aspx") %>">☣ Inicio</a>
                    <a>👤 Perfil administrador</a>
                    <a>🔓 Desbloquear usuarios</a>
                    <a href="<%= ResolveUrl("~/Mant/accion.aspx") %>">📦 Gestión productos</a>
                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                </nav>

                <a href="<%= ResolveUrl("~/Mant/accion.aspx") %>" class="logout-link">Volver a acciones
                </a>

            </aside>

            <main class="content">

                <header class="topbar">
                    <div>
                        <h1>Productos</h1>
                        <p>Busca, edita o elimina productos registrados en el sistema.</p>
                    </div>

                    <div class="profile-card">
                        <asp:Image ID="imgPerfil" runat="server" CssClass="profile-img" />
                        <div>
                            <asp:Label ID="lblNombre" runat="server" CssClass="profile-name" />
                            <span>Administrador</span>
                        </div>
                    </div>
                </header>

                <section class="table-box">

                    <div class="table-header">
                        <h3>Productos registrados</h3>
                        <asp:Label ID="lblMensaje" runat="server" CssClass="message"></asp:Label>
                    </div>

                    <div class="search-box">
                        <asp:TextBox ID="txtBuscar"
                            runat="server"
                            CssClass="input"
                            placeholder="Buscar producto...">
                        </asp:TextBox>

                        <asp:Button ID="btnBuscar"
                            runat="server"
                            Text="Buscar"
                            CssClass="btn-main"
                            OnClick="btnBuscar_Click" />
                    </div>

                    <div class="table-wrap">

                        <asp:GridView ID="gvProductos"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="admin-table"
                            DataKeyNames="pro_id"
                            AllowPaging="True"
                            PageSize="4"
                            GridLines="None"
                            OnPageIndexChanging="gvProductos_PageIndexChanging"
                            OnRowCommand="gvProductos_RowCommand"
                            OnRowDataBound="gvProductos_RowDataBound">


                            <Columns>

                                <asp:TemplateField HeaderText="Imagen">
                                    <ItemStyle CssClass="td-img-producto" />
                                    <ItemTemplate>
                                        <img src='<%# ResolveUrl(Eval("ImagenRuta").ToString()) %>'
                                            class="img-producto"
                                            onclick="abrirModalImagen(this.src)" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="pro_id" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Producto">
                                    <ItemTemplate>
                                        <div class="producto-info">
                                            <strong><%# Eval("pro_nombre") %></strong>

                                            <asp:Panel ID="pnlSinFoto" runat="server" Visible='<%# Convert.ToBoolean(Eval("ImagenDefault")) %>' CssClass="badge-warning">
                                                Imagen por Default.Añadir foto al producto
                                            </asp:Panel>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Proveedor">
                                    <ItemTemplate>
                                        <span class='<%# Eval("prov_nombre").ToString() == "Sin proveedor" ? "badge-danger" : "badge-ok" %>'>
                                            <%# Eval("prov_nombre") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="pro_cantidad" HeaderText="Cantidad" />
                                <asp:BoundField DataField="pro_precio" HeaderText="Precio" DataFormatString="{0:N2}" />

                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>

                                        <asp:Button ID="btnEditar"
                                            runat="server"
                                            Text="✏"
                                            ToolTip="Editar producto"
                                            CssClass="btn-icon btn-edit"
                                            CommandName="EditarProducto"
                                            CommandArgument='<%# Eval("pro_id") %>' />

                                        <asp:Button ID="btnEliminar"
                                            runat="server"
                                            Text="🗑"
                                            ToolTip="Eliminar producto"
                                            CssClass="btn-icon btn-delete"
                                            CommandName="EliminarProducto"
                                            CommandArgument='<%# Eval("pro_id") %>'
                                            OnClientClick="return confirm('¿Seguro que deseas eliminar este producto?');" />

                                        <asp:Button ID="btnEstadisticas"
                                            runat="server"
                                            Text="📊"
                                            ToolTip="Ver estadísticas"
                                            CssClass="btn-icon btn-stats"
                                            CommandName="EstadisticasProducto"
                                            CommandArgument='<%# Eval("pro_id") %>' />


                                        <asp:Button ID="btnImagenes"
                                            runat="server"
                                            Text="🖼"
                                            ToolTip="Gestionar imágenes"
                                            CssClass="btn-icon btn-stats"
                                            CommandName="ImagenesProducto"
                                            CommandArgument='<%# Eval("pro_id") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <EmptyDataTemplate>
                                <div class="empty">
                                    No existen productos registrados.
                                </div>
                            </EmptyDataTemplate>

                            <PagerStyle CssClass="pager" />

                        </asp:GridView>

                    </div>

                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_producto.aspx") %>" class="btn-create">Nuevo producto
                    </a>

                    <section class="table-box import-section">

                        <div class="table-header">
                            <h3>Importar / exportar productos</h3>
                        </div>

                        <div class="import-info">
                            <p>✔ El CSV solo maneja datos del producto.</p>
                            <p>✔ No incluye imágenes ni prov_id_respaldo.</p>
                            <p>✔ Si el producto queda sin imagen, agréguela desde el botón 🖼.</p>
                            <p>✔ Para agregar o editar productos, descargue el CSV, edítelo en Excel, guárdelo como CSV y vuelva a importarlo.</p>
                        </div>

                        <div class="import-box">
                            <asp:FileUpload ID="fuProductoCsv" runat="server"
                                ClientIDMode="Static"
                                CssClass="input"
                                accept=".csv" />

                            <asp:Button ID="btnPrevisualizarProductosCsv" runat="server"
                                Text="Previsualizar CSV"
                                CssClass="btn-secondary"
                                OnClientClick="return validarArchivoProductoCsv();"
                                OnClick="btnPrevisualizarProductosCsv_Click" />

                            <asp:Button ID="btnImportarProductosCsv" runat="server"
                                Text="Importar productos"
                                CssClass="btn-main"
                                OnClick="btnImportarProductosCsv_Click" />

                            <asp:Button ID="btnDescargarProductosCsv" runat="server"
                                Text="Descargar CSV"
                                CssClass="btn-secondary"
                                OnClick="btnDescargarProductosCsv_Click" />
                        </div>
                        <button type="button"
                            id="btnVerPreviewCsv"
                            class="btn-secondary btn-ver-preview"
                            onclick="abrirPreviewCsv()"
                            style="display: none;">
                            Ver previsualización
                        </button>

                        <div id="modalPreviewCsv" class="modal-preview-csv">
                            <div class="modal-preview-box">

                                <div class="modal-preview-header">
                                    <h3>Previsualización CSV</h3>
                                    <div class="modal-preview-actions">
                                        <button type="button" onclick="cerrarPreviewCsv()">✖</button>
                                    </div>
                                </div>

                                <div class="preview-scroll">
                                    <asp:GridView ID="gvPreviewProductos" runat="server"
                                        AutoGenerateColumns="true"
                                        CssClass="admin-table preview-table"
                                        GridLines="None">
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>

                    </section>
                </section>

            </main>

        </div>

    </form>

    <div id="modalImagenProducto" class="modal">
        <div class="modal-content modal-img-box">
            <img id="imgProductoGrande" src="" />
            <button type="button" onclick="cerrarModalImagen()">Cerrar</button>
        </div>
    </div>

    <script>

        function validarArchivoProductoCsv() {
            const archivo = document.getElementById("fuProductoCsv");

            if (!archivo || archivo.files.length === 0) {
                alert("Seleccione un archivo CSV.");
                return false;
            }

            const file = archivo.files[0];
            const nombre = file.name.toLowerCase();

            if (!nombre.endsWith(".csv")) {
                alert("Solo se permite archivo .csv.");
                archivo.value = "";
                return false;
            }

            const maxMb = 50;
            const maxBytes = maxMb * 1024 * 1024;

            if (file.size > maxBytes) {
                alert("El archivo CSV no debe superar " + maxMb + " MB.");
                archivo.value = "";
                return false;
            }

            return true;
        }
        function abrirModalImagen(src) {
            document.getElementById("imgProductoGrande").src = src;
            document.getElementById("modalImagenProducto").style.display = "flex";
        }

        function cerrarModalImagen() {
            document.getElementById("modalImagenProducto").style.display = "none";
        }

        document.addEventListener("DOMContentLoaded", function () {
            activarBusquedaFacebook();
            noBack();
        });

        function activarBusquedaFacebook() {
            const input = document.getElementById("<%= txtBuscar.ClientID %>");
            const tabla = document.getElementById("<%= gvProductos.ClientID %>");
            const mensaje = document.getElementById("<%= lblMensaje.ClientID %>");

            if (!input || !tabla) return;

            let temporizador = null;

            input.addEventListener("input", function () {
                clearTimeout(temporizador);

                temporizador = setTimeout(function () {
                    filtrarTabla(input, tabla, mensaje);
                }, 80);
            });
        }

        function filtrarTabla(input, tabla, mensaje) {
            const texto = normalizar(input.value);
            const filas = tabla.querySelectorAll("tr");
            let encontrados = 0;

            for (let i = 1; i < filas.length; i++) {
                const fila = filas[i];

                if (fila.querySelector(".pager") || fila.classList.contains("pager")) {
                    continue;
                }

                if (fila.querySelector(".empty")) {
                    continue;
                }

                const contenido = normalizar(fila.innerText);

                if (texto === "" || contenido.includes(texto)) {
                    mostrarFila(fila);
                    encontrados++;
                } else {
                    ocultarFila(fila);
                }
            }

            if (mensaje) {
                if (texto === "") {
                    mensaje.innerText = "";
                } else if (encontrados === 0) {
                    mensaje.innerText = "No se encontraron productos.";
                } else {
                    mensaje.innerText = "Resultados encontrados: " + encontrados;
                }
            }
        }

        function mostrarFila(fila) {
            fila.style.display = "";
            fila.classList.remove("fila-oculta-busqueda");
            fila.classList.add("fila-visible-busqueda");
        }

        function ocultarFila(fila) {
            fila.classList.remove("fila-visible-busqueda");
            fila.classList.add("fila-oculta-busqueda");

            setTimeout(function () {
                if (fila.classList.contains("fila-oculta-busqueda")) {
                    fila.style.display = "none";
                }
            }, 160);
        }

        function normalizar(texto) {
            return texto
                .toLowerCase()
                .normalize("NFD")
                .replace(/[\u0300-\u036f]/g, "")
                .trim();
        }

        window.history.forward();

        function noBack() {
            window.history.forward();
        }

        window.onpageshow = function (evt) {
            if (evt.persisted) {
                noBack();
            }
        };

        window.onunload = function () { };

        function abrirPreviewCsv() {
            const modal = document.getElementById("modalPreviewCsv");
            const btnVer = document.getElementById("btnVerPreviewCsv");

            if (modal) {
                modal.classList.add("activo");
            }

            if (btnVer) {
                btnVer.style.display = "inline-block";
            }
        }

        function cerrarPreviewCsv() {
            const modal = document.getElementById("modalPreviewCsv");

            if (modal) {
                modal.classList.remove("activo");
            }
        }
    </script>

</body>
</html>
