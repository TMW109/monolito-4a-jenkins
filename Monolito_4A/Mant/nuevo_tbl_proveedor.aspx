<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="nuevo_tbl_proveedor.aspx.cs" Inherits="Monolito_4A.Mant.nuevo_tbl_proveedor" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Proveedores | Requiem</title>

    <link href="<%= ResolveUrl("~/css/prov.css") %>" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

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
                            <a class="active" href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                        </nav>

                        <asp:Button ID="btnCerrarSesion" runat="server"
                            Text="Cerrar sesión"
                            CssClass="logout"
                            OnClick="btnCerrarSesion_Click" />
                    </aside>

                    <main class="content">

                        <header class="topbar">
                            <div>
                                <h1>Proveedores</h1>
                                <p>Registro, edición y mantenimiento de proveedores.</p>
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
                                <h3>Formulario proveedor</h3>
                                <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                            </div>

                            <asp:HiddenField ID="hfProveedorId" runat="server" ClientIDMode="Static" />

                            <div class="form-grid">
                                <div class="input-group">
                                    <span class="icon">🏭</span>

                                    <asp:TextBox ID="txtNombreProveedor" runat="server"
                                        ClientIDMode="Static"
                                        CssClass="input"
                                        MaxLength="50"
                                        placeholder="Nombre del proveedor" />

                                    <div id="boxEstado" runat="server" class="input-group estado-box" visible="false">
                                        <span class="icon">⚙</span>

                                        <asp:DropDownList ID="ddlEstado" runat="server"
                                            ClientIDMode="Static"
                                            CssClass="input">
                                            <asp:ListItem Value="A">Activo</asp:ListItem>
                                            <asp:ListItem Value="I">Inactivo</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="actions">
                                <asp:Button ID="btnGuardar" runat="server"
                                    ClientIDMode="Static"
                                    Text="Guardar proveedor"
                                    CssClass="btn-main"
                                    OnClientClick="return validarProveedor();"
                                    OnClick="btnGuardar_Click" />

                                <asp:Button ID="btnLimpiar" runat="server"
                                    Text="Limpiar"
                                    CssClass="btn-secondary"
                                    OnClick="btnLimpiar_Click" />

                                <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_producto.aspx") %>" class="btn-link">Nuevo producto</a>
                            </div>
                        </section>

                        <section class="table-box import-section">
                            <div class="table-header">
                                <h3>Importar / exportar proveedores</h3>
                            </div>

                            <div class="import-box">
                                <asp:FileUpload ID="fuProveedor" runat="server"
                                    ClientIDMode="Static"
                                    CssClass="input"
                                    accept=".csv,.xls,.xlsx" />

                                <asp:Button ID="btnPrevisualizarProveedor" runat="server"
                                    Text="Previsualizar CSV"
                                    CssClass="btn-secondary"
                                    OnClientClick="if (!validarArchivoProveedor()) return false;"
                                    OnClick="btnPrevisualizarProveedor_Click" />

                                <asp:Button ID="btnImportarProveedor" runat="server"
                                    Text="Importar proveedores"
                                    CssClass="btn-main"
                                    OnClick="btnImportarProveedor_Click" />

                                <asp:Button ID="btnDescargarProveedor" runat="server"
                                    Text="Descargar CSV"
                                    CssClass="btn-secondary"
                                    OnClick="btnDescargarProveedor_Click" />
                            </div>

                            <asp:GridView ID="gvPreviewProveedor" runat="server"
                                AutoGenerateColumns="true"
                                CssClass="admin-table"
                                GridLines="None">
                            </asp:GridView>
                        </section>

                        <section class="table-box">
                            <div class="table-header">
                                <h3>Proveedores registrados</h3>
                            </div>

                            <div class="search-facebook">
                                <span class="search-icon">🔍</span>

                                <asp:TextBox ID="txtBuscar" runat="server"
                                    ClientIDMode="Static"
                                    CssClass="search-input"
                                    placeholder="Buscar proveedor rápido..." />

                                <asp:Button ID="btnBuscar" runat="server"
                                    Text="Buscar"
                                    CssClass="btn-search"
                                    OnClick="btnBuscar_Click" />
                            </div>

                            <div class="table-wrap">
                                <asp:GridView ID="gvProveedores" runat="server"
                                    AutoGenerateColumns="False"
                                    CssClass="admin-table"
                                    DataKeyNames="prov_id"
                                    AllowPaging="True"
                                    PageSize="4"
                                    GridLines="None"
                                    OnPageIndexChanging="gvProveedores_PageIndexChanging"
                                    OnRowCommand="gvProveedores_RowCommand">

                                    <Columns>
                                        <asp:BoundField DataField="prov_id" HeaderText="ID" />
                                        <asp:BoundField DataField="prov_nombre" HeaderText="Proveedor" />

                                        <asp:TemplateField HeaderText="Estado">
                                            <ItemTemplate>
                                                <%# Eval("prov_estado").ToString() == "A" ? "Activo" : "Inactivo" %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Acciones">
                                            <ItemTemplate>
                                                <asp:Button ID="btnEditar" runat="server"
                                                    Text="✏"
                                                    ToolTip="Editar proveedor"
                                                    CssClass="btn-icon btn-edit"
                                                    CommandName="EditarProveedor"
                                                    CommandArgument='<%# Eval("prov_id") %>' />

                                                <asp:Button ID="btnEliminar" runat="server"
                                                    Text="🗑"
                                                    ToolTip="Eliminar proveedor"
                                                    CssClass="btn-icon btn-delete"
                                                    CommandName="EliminarProveedor"
                                                    CommandArgument='<%# Eval("prov_id") %>'
                                                    OnClientClick="return confirm('¿Seguro que deseas eliminar este proveedor?');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>

                                    <EmptyDataTemplate>
                                        <div class="empty">No existen proveedores registrados.</div>
                                    </EmptyDataTemplate>

                                    <PagerStyle CssClass="pager" />
                                </asp:GridView>
                            </div>
                        </section>

                    </main>
                </div>

            </ContentTemplate>

            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrevisualizarProveedor" />
                <asp:PostBackTrigger ControlID="btnImportarProveedor" />
                <asp:PostBackTrigger ControlID="btnDescargarProveedor" />
            </Triggers>
        </asp:UpdatePanel>

    </form>

    <div id="modalProveedor" class="modal">
        <div class="modal-content">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3>Mensaje</h3>
            <p>Texto</p>
            <button type="button" onclick="cerrarModalProveedor()">Continuar</button>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/prov.js?v=2") %>"></script>
</body>
</html>
