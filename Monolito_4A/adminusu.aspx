<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="adminusu.aspx.cs" Inherits="Monolito_4A.adminusu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Usuarios | Requiem</title>
    <link href="~/css/adminusu.css" rel="stylesheet" runat="server" />
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
                            <a class="active" href="<%= ResolveUrl("~/adminusu.aspx") %>">👥 Administrar usuarios</a>
                            <a href="<%= ResolveUrl("~/admin.aspx") %>">🔓 Desbloquear usuarios</a>
                        </nav>

                        <asp:Button ID="btnCerrarSesion" runat="server"
                            Text="Cerrar sesión"
                            CssClass="logout"
                            OnClick="btnCerrarSesion_Click" />
                    </aside>

                    <main class="content">

                        <header class="topbar">
                            <div>
                                <h1>Administrar usuarios</h1>
                                <p>CRUD de usuarios registrados en REQUIEM</p>
                            </div>
                        </header>

                        <section class="table-box">
                            <div class="table-header">
                                <h3>Formulario usuario</h3>
                                <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                            </div>

                            <asp:HiddenField ID="hfUsuarioId" runat="server" ClientIDMode="Static" />

                            <div class="form-grid">

                                <asp:TextBox ID="txtCedula" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="10" placeholder="Cédula" />
                                <asp:TextBox ID="txtNombres" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="50" placeholder="Nombres" />
                                <asp:TextBox ID="txtApellidos" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="50" placeholder="Apellidos" />
                                <asp:TextBox ID="txtDireccion" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="100" placeholder="Dirección" />
                                <asp:TextBox ID="txtCelular" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="10" placeholder="Celular" />
                                <asp:TextBox ID="txtCorreo" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="150" placeholder="Correo" />
                                <asp:TextBox ID="txtFechaCumple" runat="server" ClientIDMode="Static" CssClass="input" TextMode="Date" />
                                <asp:TextBox ID="txtNick" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="50" placeholder="Nick" />

                                <div class="input-pass">
                                    <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" CssClass="input" TextMode="Password" placeholder="Contraseña nueva opcional" />
                                    <button type="button" class="show-pass" onclick="mostrarPassword('txtPassword', this)">👁</button>
                                </div>

                                <asp:DropDownList ID="ddlTipoUsuario" runat="server" ClientIDMode="Static" CssClass="input" />

                                <asp:DropDownList ID="ddlEstado" runat="server" ClientIDMode="Static" CssClass="input">
                                    <asp:ListItem Value="A">Activo</asp:ListItem>
                                    <asp:ListItem Value="B">Bloqueado</asp:ListItem>
                                </asp:DropDownList>

                                <asp:FileUpload
                                    ID="fuFotos"
                                    runat="server"
                                    ClientIDMode="Static"
                                    CssClass="input"
                                    AllowMultiple="true"
                                    accept=".jpg,.jpeg,.png,image/jpeg,image/png" />
                            </div>

                            <div class="password-rules">
                                <p id="ruleLength">✖ Mínimo 8 caracteres</p>
                                <p id="ruleUpper">✖ Una letra mayúscula</p>
                                <p id="ruleLower">✖ Una letra minúscula</p>
                                <p id="ruleNumber">✖ Un número</p>
                                <p id="ruleSymbol">✖ Un símbolo</p>
                                <p>✔ Fotos permitidas: JPG o PNG, máximo 5 MB por imagen</p>
                            </div>

                            <div class="actions">
                                <asp:Button ID="btnPrevisualizar" runat="server"
                                    Text="Previsualizar fotos"
                                    CssClass="btn-secondary"
                                    OnClientClick="return validarFotoCliente();"
                                    OnClick="btnPrevisualizar_Click" />

                                <asp:Button ID="btnGuardar" runat="server"
                                    Text="Guardar usuario"
                                    CssClass="btn-main"
                                    OnClientClick="return validarAdminUsuario();"
                                    OnClick="btnGuardar_Click" />

                                <asp:Button ID="btnLimpiar" runat="server"
                                    Text="Limpiar"
                                    CssClass="btn-secondary"
                                    OnClick="btnLimpiar_Click" />
                            </div>

                            <div id="previewContainer" runat="server" class="preview-container"></div>

                        </section>

                        <section class="table-box">
                            <div class="table-header">
                                <h3>Usuarios registrados</h3>
                            </div>

                            <asp:GridView ID="gvUsuarios" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="admin-table"
                                DataKeyNames="usu_id"
                                OnRowCommand="gvUsuarios_RowCommand">

                                <Columns>
                                    <asp:BoundField DataField="usu_id" HeaderText="ID" />
                                    <asp:BoundField DataField="usu_cedula" HeaderText="Cédula" />
                                    <asp:BoundField DataField="usu_nombres" HeaderText="Nombres" />
                                    <asp:BoundField DataField="usu_apellidos" HeaderText="Apellidos" />
                                    <asp:BoundField DataField="usu_correo" HeaderText="Correo" />
                                    <asp:BoundField DataField="usu_nick" HeaderText="Nick" />
                                    <asp:BoundField DataField="usu_estado" HeaderText="Estado" />
                                    <asp:BoundField DataField="tusu_id" HeaderText="Tipo" />

                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <div class="table-actions">
                                                <asp:Button runat="server"
                                                    Text="Editar"
                                                    CssClass="btn-table btn-edit"
                                                    CommandName="EditarUsuario"
                                                    CommandArgument='<%# Eval("usu_id") %>' />

                                                <asp:Button runat="server"
                                                    Text="Lógico"
                                                    CssClass="btn-table btn-delete"
                                                    CommandName="EliminarLogico"
                                                    CommandArgument='<%# Eval("usu_id") %>'
                                                    OnClientClick="return confirm('¿Seguro que desea eliminar lógicamente este usuario?');" />

                                                <asp:Button runat="server"
                                                    Text="Físico"
                                                    CssClass="btn-table btn-delete-dark"
                                                    CommandName="EliminarFisico"
                                                    CommandArgument='<%# Eval("usu_id") %>'
                                                    OnClientClick="return confirm('¿Seguro que desea eliminar físicamente este usuario?');" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                                <EmptyDataTemplate>
                                    <div class="empty">No existen usuarios registrados.</div>
                                </EmptyDataTemplate>

                            </asp:GridView>
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

    <div id="modalAdminUsu" class="modal">
        <div class="modal-content">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3>Mensaje</h3>
            <p>Texto</p>
            <button type="button" onclick="cerrarModalAdmin()">Continuar</button>
        </div>
    </div>

    <div id="modalImagen" class="modal">
        <div class="modal-content modal-img-box">
            <img id="imgModalPreview" src="" />
            <button type="button" onclick="cerrarModalImagen()">Cerrar</button>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/adminusu.js") %>"></script>
</body>
</html>
