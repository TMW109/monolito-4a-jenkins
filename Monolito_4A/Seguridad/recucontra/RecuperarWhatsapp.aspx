<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarWhatsapp.aspx.cs" Inherits="Monolito_4A.Seguridad.recucontra.RecuperarWhatsapp" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Recuperar por WhatsApp | Requiem</title>

    <link href="~/css/recuperarwhatsapp.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="recu-page">

            <!-- PANEL IZQUIERDO -->
            <section class="left-panel">
                <div class="logo-wrapper">
                    <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                </div>

                <h1>T.M.W<br />
                    <span>requiem</span>
                </h1>

                <p class="subtitle">RECUPERACIÓN POR WHATSAPP</p>
            </section>

            <!-- PANEL DERECHO -->
            <section class="right-panel">
                <div class="recu-box">

                    <div class="recu-header">
                        <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="small-logo" />
                        <div>
                            <h2>Recuperar contraseña</h2>
                            <p>Clave temporal por WhatsApp</p>
                        </div>
                    </div>

                    <div class="info-box">
                        Primero ingresa tu cédula. Si existe, se habilitará el campo celular.
                    </div>

                    <!-- CEDULA -->
                    <div class="input-group">
                        <span class="icon">🪪</span>
                        <asp:TextBox
                            ID="txtCedula"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            MaxLength="10"
                            AutoCompleteType="Disabled"
                            autocomplete="nope"
                            placeholder="Cédula">
                        </asp:TextBox>
                    </div>

                    <asp:Button
                        ID="btnBuscarCedula"
                        runat="server"
                        ClientIDMode="Static"
                        Text="Buscar cédula"
                        CssClass="btn-main"
                        OnClick="btnBuscarCedula_Click" />

                    <!-- CELULAR -->
                    <div class="input-group">
                        <span class="icon">📱</span>
                        <asp:TextBox
                            ID="txtCelular"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            MaxLength="10"
                            Enabled="false"
                            AutoCompleteType="Disabled"
                            autocomplete="nope"
                            placeholder="Celular registrado">
                        </asp:TextBox>
                    </div>

                    <asp:Button
                        ID="btnEnviar"
                        runat="server"
                        ClientIDMode="Static"
                        Text="Enviar clave temporal"
                        CssClass="btn-main"
                        Enabled="false"
                        OnClick="btnEnviar_Click" />

                    <!-- LINK WHATSAPP -->
                    <asp:HyperLink
                        ID="lnkWhatsapp"
                        runat="server"
                        CssClass="btn-whatsapp"
                        Target="_blank"
                        Visible="false">
                        Abrir WhatsApp
                    </asp:HyperLink>

                    <!-- MENSAJE -->
                    <div class="message-box">
                        <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                    </div>

                    <a href="<%= ResolveUrl("~/Seguridad/recucontra/tiporecu.aspx") %>" class="btn-back">Volver</a>
                    <p class="meta">REQUIEM SECURITY SYSTEM</p>

                </div>
            </section>

        </div>

        <!-- MODAL DE ÉXITO -->
        <div id="modalExito" class="modal">
            <div class="modal-content">
                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
                <h3>Proceso completado</h3>
                <p>Clave temporal generada. Presiona Abrir WhatsApp y luego vuelve al login.</p>

                <asp:HyperLink
                    ID="lnkWhatsappModal"
                    runat="server"
                    CssClass="btn-whatsapp"
                    Target="_blank"
                    Visible="false">
                    Abrir WhatsApp
                </asp:HyperLink>

                <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="btn-main">Volver al login</a>
            </div>
        </div>

    </form>

    <script src="<%= ResolveUrl("~/js/recuperarwhatsapp.js") %>"></script>
</body>
</html>
