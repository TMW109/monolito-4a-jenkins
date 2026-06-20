<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="imagenes_producto.aspx.cs" Inherits="Monolito_4A.Mant.imagenes_producto" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Imágenes producto | Requiem</title>
    <link href="<%= ResolveUrl("~/css/imapro.css") %>" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

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
                    <a href="<%= ResolveUrl("~/Mant/listar_tbl_producto.aspx") %>" class="active">🖼 Imágenes de productos</a>
                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                </nav>

                <a href="<%= ResolveUrl("~/Mant/listar_tbl_producto.aspx") %>" class="logout-link">Volver</a>
            </aside>

            <main class="content">

                <header class="topbar">
                    <div>
                        <h1>Imágenes del producto</h1>
                        <p>
                            Producto:
                            <asp:Label ID="lblProducto" runat="server" CssClass="product-name-title" />
                        </p>
                    </div>
                </header>

                <section class="table-box">

                    <div class="table-header">
                        <h3>Registrar imagen</h3>
                        <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                    </div>

                    <asp:HiddenField ID="hfImagenId" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="hfPrevisualizada" runat="server" ClientIDMode="Static" Value="0" />

                    <div class="search-box">
                        <asp:TextBox
                            ID="txtNombre"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            MaxLength="80"
                            placeholder="Nombre de la imagen" />

                        <asp:FileUpload
                            ID="fuImagen"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            AllowMultiple="true"
                            accept=".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp" />
                    </div>

                    <div class="button-grid button-grid-three">
                        <asp:Button
                            ID="btnPrevisualizar"
                            runat="server"
                            ClientIDMode="Static"
                            Text="Previsualizar imagen"
                            CssClass="btn-main btn-secondary"
                            OnClientClick="return validarImagenCliente();"
                            OnClick="btnPrevisualizar_Click" />

                        <asp:Button
                            ID="btnGuardar"
                            runat="server"
                            ClientIDMode="Static"
                            Text="Guardar imagen"
                            CssClass="btn-main"
                            OnClientClick="return validarFormularioImagen();"
                            OnClick="btnGuardar_Click" />

                        <asp:Button
                            ID="btnLimpiar"
                            runat="server"
                            ClientIDMode="Static"
                            Text="Limpiar"
                            CssClass="btn-main btn-clear"
                            OnClick="btnLimpiar_Click" />
                    </div>

                    <div id="previewContainer" runat="server" class="preview-container"></div>

                    <div class="rules-box">
                        <p>✔ Formatos permitidos: JPG, PNG y WEBP</p>
                        <p>✔ Tamaño máximo: 5 MB por imagen</p>
                        <p>✔ Puede seleccionar una o varias imágenes</p>
                        <p>✔ Para guardar primero debe previsualizar las imágenes</p>
                    </div>

                    <div class="import-image-box">
                        <h3>Importar / exportar rutas de imágenes</h3>

                        <div class="rules-box">
                            <p>✔ El CSV solo maneja rutas de imágenes.</p>
                            <p>✔ No sube archivos físicos al servidor.</p>
                            <p>✔ Si pimg_ruta viene vacía, se usará: ~/imagen/TMW.png.</p>
                            <p>✔ Para cambiar la imagen real, use el formulario superior.</p>
                        </div>

                        <asp:FileUpload ID="fuImagenCsv" runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            accept=".csv" />

                        <div class="button-grid button-grid-three">
                            <asp:Button ID="btnPrevisualizarImagenCsv" runat="server"
                                Text="Previsualizar CSV"
                                CssClass="btn-main btn-secondary"
                                OnClientClick="return validarArchivoImagenCsv();"
                                OnClick="btnPrevisualizarImagenCsv_Click" />

                            <asp:Button ID="btnImportarImagenCsv" runat="server"
                                Text="Importar imágenes"
                                CssClass="btn-main"
                                OnClick="btnImportarImagenCsv_Click" />

                            <asp:Button ID="btnDescargarImagenCsv" runat="server"
                                Text="Descargar CSV"
                                CssClass="btn-main btn-clear"
                                OnClick="btnDescargarImagenCsv_Click" />
                        </div>

                        <asp:GridView ID="gvPreviewImagenCsv" runat="server"
                            AutoGenerateColumns="true"
                            CssClass="admin-table"
                            GridLines="None" />
                    </div>

                    <div class="table-wrap">
                        <asp:GridView ID="gvImagenes"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="admin-table"
                            DataKeyNames="pimg_id"
                            GridLines="None"
                            AllowPaging="True"
                            PageSize="4"
                            OnPageIndexChanging="gvImagenes_PageIndexChanging"
                            OnRowCommand="gvImagenes_RowCommand"
                            OnRowDataBound="gvImagenes_RowDataBound">

                            <Columns>
                                <asp:TemplateField HeaderText="Imagen">
                                    <ItemTemplate>
                                        <img src='<%# ResolveUrl(Eval("pimg_ruta").ToString()) %>'
                                            class="img-producto"
                                            onclick="abrirModalImagen(this.src)" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="pimg_id" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Nombre">
                                    <ItemTemplate>
                                        <div class="imagen-info">
                                            <strong><%# Eval("pimg_nombre") %></strong>

                                            <asp:Panel ID="pnlImagenPendiente" runat="server"
                                                Visible='<%# Eval("pimg_ruta").ToString() == "~/imagen/TMW.png" %>'
                                                CssClass="badge-imagen-pendiente">
                                                Imagen pendiente: cambie la foto desde el CRUD
                                            </asp:Panel>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="pimg_fecha_creacion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />

                                <asp:TemplateField HeaderText="Acciones">
                                    <ItemTemplate>
                                        <asp:Button runat="server"
                                            Text="✏"
                                            CssClass="btn-icon btn-edit"
                                            CommandName="EditarImagen"
                                            CommandArgument='<%# Eval("pimg_id") %>' />

                                        <asp:Button runat="server"
                                            Text="🗑"
                                            CssClass="btn-icon btn-delete"
                                            CommandName="EliminarImagen"
                                            CommandArgument='<%# Eval("pimg_id") %>'
                                            OnClientClick="return confirm('¿Seguro que deseas eliminar esta imagen?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <EmptyDataTemplate>
                                <div class="empty">Este producto aún no tiene imágenes.</div>
                            </EmptyDataTemplate>

                            <PagerStyle CssClass="pager" />

                        </asp:GridView>
                    </div>

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

    <div id="modalMensaje" class="modal">
        <div class="modal-content modal-message-box">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3 id="tituloModal">Mensaje</h3>
            <p id="textoModal"></p>
            <button type="button" onclick="cerrarModalMensaje()">Cerrar</button>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/imapro.js") %>"></script>

</body>
</html>
