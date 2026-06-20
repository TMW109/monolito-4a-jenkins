<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="accion.aspx.cs" Inherits="Monolito_4A.Mant.accion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Gestión productos | Requiem</title>

    <link href="<%= ResolveUrl("~/css/accion.css") %>" rel="stylesheet" />
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
                    <a href="<%= ResolveUrl("~/adminusu.aspx") %>">👥 Administrar usuarios</a>
                    <a class="active" href="<%= ResolveUrl("~/Mant/accion.aspx") %>">📦 Gestión productos</a>
                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>


                </nav>

                <a href="<%= ResolveUrl("~/admin.aspx") %>" class="logout-link">Volver al panel</a>
            </aside>

            <main class="content">

                <header class="topbar">
                    <div>
                        <h1>Gestión productos</h1>
                        <p>Administración de productos, proveedores e imágenes</p>
                    </div>

                    <div class="profile-card">
                        <asp:Image ID="imgPerfil" runat="server" CssClass="profile-img" />
                        <div>
                            <asp:Label ID="lblNombre" runat="server" CssClass="profile-name" />
                            <span>Administrador</span>
                        </div>
                    </div>
                </header>

                <section class="table-box acciones-box">
                    <div class="table-header">
                        <h3>Acciones disponibles</h3>
                        <span class="message">Mantenimiento de productos</span>
                    </div>

                    <div class="method-grid">
                        <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>" class="method-card">
                            <span class="method-icon">🏭</span>
                            <strong>Proveedores</strong>
                            <small>Registra, edita y elimina proveedores del sistema.</small>
                        </a>

                        <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_producto.aspx") %>" class="method-card">
                            <span class="method-icon">➕</span>
                            <strong>Nuevo producto</strong>
                            <small>Registra productos con proveedor, precio, cantidad e imágenes.</small>
                        </a>

                        <a href="<%= ResolveUrl("~/Mant/listar_tbl_producto.aspx") %>" class="method-card">
                            <span class="method-icon">📋</span>
                            <strong>Listado productos</strong>
                            <small>Consulta, edita y elimina productos registrados.</small>
                        </a>
                    </div>
                </section>

            </main>

        </div>

    </form>
</body>
<script>
    window.history.forward();

    function noBack() {
        window.history.forward();
    }

    window.onload = noBack;

    window.onpageshow = function (evt) {
        if (evt.persisted) {
            noBack();
        }
    };

    window.onunload = function () { };
</script>
</html>
