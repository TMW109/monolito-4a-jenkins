<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="Monolito_4A.Seguridad.Registro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Registro | Requiem</title>

    <link href="~/css/registro.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <div class="register-page">

                    <section class="left-panel">
                        <div class="logo-wrapper">
                            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                        </div>

                        <h1>T.M.W SYSTEM<span>requiem</span></h1>

                        <p class="subtitle">Registro de nuevo agente</p>
                    </section>

                    <section class="right-panel">
                        <div class="register-box">

                            <div class="register-header">
                                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="small-logo" />
                                <div>
                                    <h2>Registrar usuario</h2>
                                    <p>Formulario de acceso</p>
                                </div>
                            </div>

                            <div class="form-grid">

                                <div class="input-group">
                                    <span class="icon">🪪</span>
                                    <asp:TextBox ID="txtCedula" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="10" AutoCompleteType="Disabled"
                                        autocomplete="nope"
                                        placeholder="Cédula"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">👤</span>
                                    <asp:TextBox ID="txtNombres" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="50"
                                        AutoCompleteType="Disabled"
                                        autocomplete="nope" placeholder="Nombres"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">👥</span>
                                    <asp:TextBox ID="txtApellidos" runat="server" AutoCompleteType="Disabled"
                                        autocomplete="nope" ClientIDMode="Static" CssClass="input" MaxLength="50" placeholder="Apellidos" AutoPostBack="true" OnTextChanged="txtApellidos_TextChanged"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">📍</span>
                                    <asp:TextBox ID="txtDireccion" runat="server" AutoCompleteType="Disabled"
                                        autocomplete="nope" ClientIDMode="Static" CssClass="input" MaxLength="100" placeholder="Dirección"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">📱</span>
                                    <asp:TextBox ID="txtCelular" runat="server" ClientIDMode="Static" CssClass="input" MaxLength="10" AutoCompleteType="Disabled"
                                        autocomplete="nope" placeholder="Teléfono / Celular"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">✉</span>
                                    <asp:TextBox ID="txtCorreo" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                        autocomplete="nope" CssClass="input" MaxLength="150" placeholder="Correo electrónico"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">🎂</span>
                                    <asp:TextBox ID="txtFechaCumple" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                        autocomplete="nope" CssClass="input" TextMode="Date"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">☣</span>
                                    <asp:TextBox ID="txtUsuario" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                        autocomplete="nope" CssClass="input" MaxLength="50" placeholder="Nombre de usuario"></asp:TextBox>
                                </div>

                                <div class="input-group">
                                    <span class="icon">🔐</span>
                                    <asp:TextBox ID="txtPassword" runat="server" AutoCompleteType="Disabled"
                                        autocomplete="new-password" ClientIDMode="Static" CssClass="input" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                                    <button type="button" class="show-pass" onclick="mostrarPassword('txtPassword', this)">👁</button>
                                </div>

                                <div class="input-group">
                                    <span class="icon">🔒</span>
                                    <asp:TextBox ID="txtConfirmar" runat="server" ClientIDMode="Static" AutoCompleteType="Disabled"
                                        autocomplete="new-password" CssClass="input" TextMode="Password" placeholder="Confirmar contraseña"></asp:TextBox>
                                    <button type="button" class="show-pass" onclick="mostrarPassword('txtConfirmar', this)">👁</button>
                                </div>

                                <div class="input-group full">
                                    <span class="icon">🧬</span>
                                    <asp:DropDownList ID="ddlTipoUsuario" runat="server" ClientIDMode="Static" CssClass="input"></asp:DropDownList>
                                </div>

                                <div class="input-group full">
                                    <span class="icon">🖼</span>
                                    <asp:FileUpload
                                        ID="fuFoto"
                                        runat="server"
                                        ClientIDMode="Static"
                                        CssClass="input"
                                        AllowMultiple="true"
                                        accept=".jpg,.jpeg,.png,image/jpeg,image/png" />
                                </div>

                                <div class="input-group full">
                                    <asp:Button
                                        ID="btnPrevisualizar"
                                        runat="server"
                                        ClientIDMode="Static"
                                        Text="Previsualizar fotos"
                                        CssClass="btn-register"
                                        OnClientClick="return validarFotoCliente();"
                                        OnClick="btnPrevisualizar_Click" />
                                </div>

                                <div class="input-group full">
                                    <div id="previewContainer" runat="server" class="preview-container"></div>
                                </div>

                            </div>

                            <div class="password-rules">
                                <p id="ruleLength">✖ Mínimo 8 caracteres</p>
                                <p id="ruleUpper">✖ Una letra mayúscula</p>
                                <p id="ruleLower">✖ Una letra minúscula</p>
                                <p id="ruleNumber">✖ Un número</p>
                                <p id="ruleSymbol">✖ Un símbolo</p>
                                <p>✔ Fotos permitidas: JPG o PNG, máximo 5 MB por imagen</p>
                            </div>

                            <asp:Button
                                ID="btnRegistrar"
                                runat="server"
                                ClientIDMode="Static"
                                Text="Registrar agente"
                                CssClass="btn-register"
                                OnClientClick="return iniciarRegistro();"
                                OnClick="btnRegistrar_Click" />

                            <div id="progressRegistro" class="progress-box">
                                <div class="progress-bar">
                                    <div class="progress-fill"></div>
                                </div>
                                <p>Registrando agente...</p>
                            </div>

                            <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="back-login">Ya tengo cuenta</a>
                        </div>
                    </section>

                </div>

            </ContentTemplate>

            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrevisualizar" />
                <asp:PostBackTrigger ControlID="btnRegistrar" />
            </Triggers>
        </asp:UpdatePanel>

    </form>

    <div id="modalRegistro" class="modal">
        <div class="modal-content">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3>Registro exitoso</h3>
            <p>El nuevo agente fue registrado correctamente.</p>
            <button type="button" onclick="cerrarModal()">Continuar</button>
        </div>
    </div>

    <div id="modalImagen" class="modal">
        <div class="modal-content" style="width: 520px; max-width: 90%;">
            <img id="imgModalPreview"
                src=""
                style="width: 100%; max-height: 520px; object-fit: contain; border-radius: 16px; margin-bottom: 18px;" />

            <button type="button" onclick="cerrarModalImagen()">Cerrar</button>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/resgistro.js") %>"></script>
    k</body>
</html>
