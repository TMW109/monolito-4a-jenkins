<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="nuevo_tbl_producto.aspx.cs" Inherits="Monolito_4A.Mant.nuevo_tbl_producto" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Nuevo producto | Requiem | JENKIENS</title>
    <link href="<%= ResolveUrl("~/css/nuevprod.css") %>" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
                            <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                        </nav>

                        <a href="<%= ResolveUrl("~/Mant/accion.aspx") %>" class="logout-link">Volver a acciones</a>
                    </aside>

                    <main class="content">

                        <header class="topbar">
                            <div>
                                <h1>Nuevo producto</h1>
                                <p>Registro de productos con proveedor, precio, cantidad e imágenes.</p>
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
                                <h3>Formulario producto</h3>
                                <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                            </div>

                            <div class="form-grid">
                                <div class="input-group">
                                    <span class="icon">📦</span>
                                    <asp:TextBox ID="txtNombre" runat="server" ClientIDMode="Static"
                                        CssClass="input" MaxLength="50" placeholder="Nombre del producto" />
                                </div>

                                <div class="input-group">
                                    <span class="icon">🔢</span>
                                    <asp:TextBox ID="txtCantidad" runat="server" ClientIDMode="Static"
                                        CssClass="input" MaxLength="6" placeholder="Cantidad" />
                                </div>

                                <div class="input-group">
                                    <span class="icon">💵</span>
                                    <asp:TextBox ID="txtPrecio" runat="server" ClientIDMode="Static"
                                        CssClass="input" MaxLength="10" placeholder="Precio" />
                                </div>

                                <div class="input-group">
                                    <span class="icon">🏭</span>
                                    <asp:DropDownList ID="ddlProveedor" runat="server"
                                        ClientIDMode="Static" CssClass="input" />
                                </div>

                                <div class="input-group full">
                                    <span class="icon">🖼</span>
                                    <asp:FileUpload ID="fuFotos" runat="server"
                                        ClientIDMode="Static" CssClass="input"
                                        AllowMultiple="true"
                                        accept=".jpg,.jpeg,.png,image/jpeg,image/png" />
                                </div>
                            </div>

                            <div class="rules">
                                <p>✔ Imágenes permitidas: JPG o PNG.</p>
                                <p>✔ Tamaño máximo: 5 MB por imagen.</p>
                                <p>✔ Puedes seleccionar varias imágenes.</p>
                            </div>

                            <div class="actions">
                                <asp:Button ID="btnPrevisualizar" runat="server"
                                    ClientIDMode="Static"
                                    Text="Previsualizar imágenes"
                                    CssClass="btn-secondary"
                                    OnClientClick="return validarFotosProducto();"
                                    OnClick="btnPrevisualizar_Click" />

                                <asp:Button ID="btnGuardar" runat="server"
                                    ClientIDMode="Static"
                                    Text="Guardar producto"
                                    CssClass="btn-main"
                                    OnClientClick="return validarProducto();"
                                    OnClick="btnGuardar_Click" />

                                <a href="<%= ResolveUrl("~/Mant/listar_tbl_producto.aspx") %>" class="btn-link">Ver productos</a>
                            </div>

                            <div id="previewContainer" runat="server" class="preview-container"></div>
                        </section>
                    </main>
                </div>

            </ContentTemplate>

            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrevisualizar" />
                <asp:PostBackTrigger ControlID="btnGuardar" />
            </Triggers>
        </asp:UpdatePanel>
    </form>

    <div id="modalProducto" class="modal">
        <div class="modal-content">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3>Mensaje</h3>
            <p>Texto</p>
            <button type="button" onclick="cerrarModalProducto()">Continuar</button>
        </div>
    </div>

    <div id="modalImagen" class="modal">
        <div class="modal-content modal-img-box">
            <img id="imgModalPreview" src="" />
            <button type="button" onclick="cerrarModalImagen()">Cerrar</button>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/nueprod.js?v=2") %>"></script>
</body>
</html>
