<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="Monolito_4A.admin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Admin | Requiem</title>

    <link href="~/css/admin.css" rel="stylesheet" runat="server" />
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
                    <a class="active">☣ Inicio</a>
                    <a>👤 Perfil administrador</a>
                    <a>🔓 Desbloquear usuarios</a>
                    <a href="<%= ResolveUrl("~/Mant/accion.aspx") %>">📦 Gestión productos</a>
                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                </nav>

                <asp:Button ID="btnCerrarSesion" runat="server"
                    Text="Cerrar sesión"
                    CssClass="logout"
                    OnClick="btnCerrarSesion_Click" />
            </aside>

            <main class="content">

                <header class="topbar">
                    <div>
                        <h1>Panel administrador</h1>
                        <p>Gestión de seguridad y desbloqueo de usuarios</p>
                    </div>

                    <div class="profile-card">
                        <asp:Image ID="imgPerfil" runat="server" CssClass="profile-img" />
                        <div>
                            <asp:Label ID="lblNombre" runat="server" CssClass="profile-name" />
                            <span>Administrador</span>
                        </div>
                    </div>
                </header>

                <section class="hero">
                    <h2>Centro de control REQUIEM</h2>
                    <p>Administra los accesos bloqueados del sistema.</p>
                </section>

                <section class="table-box">
                    <div class="table-header">
                        <h3>Usuarios bloqueados</h3>
                        <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                    </div>

                    <asp:GridView ID="gvBloqueados" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="admin-table"
                        DataKeyNames="usu_id"
                        OnRowCommand="gvBloqueados_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="usu_id" HeaderText="ID" />
                            <asp:BoundField DataField="usu_cedula" HeaderText="Cédula" />
                            <asp:BoundField DataField="usu_nick" HeaderText="Usuario" />
                            <asp:BoundField DataField="usu_correo" HeaderText="Correo" />
                            <asp:BoundField DataField="usu_intentos" HeaderText="Intentos" />

                            <asp:TemplateField HeaderText="Acción">
                                <ItemTemplate>
                                    <asp:Button ID="btnDesbloquear" runat="server"
                                        Text="Desbloquear"
                                        CssClass="btn-unlock"
                                        CommandName="Desbloquear"
                                        CommandArgument='<%# Eval("usu_id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <EmptyDataTemplate>
                            <div class="empty">No existen usuarios bloqueados.</div>
                        </EmptyDataTemplate>

                    </asp:GridView>
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
