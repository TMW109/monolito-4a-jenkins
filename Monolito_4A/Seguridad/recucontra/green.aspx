<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="green.aspx.cs" Inherits="Monolito_4A.Seguridad.recucontra.green" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Recuperar por Green API | Requiem</title>
    <link href="~/css/recuperarwhatsapp.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="recu-page">

            <section class="left-panel">
                <div class="logo-wrapper">
                    <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                </div>

                <h1>T.M.W<br />
                    <span>requiem</span>
                </h1>

                <p class="subtitle">GREEN API WHATSAPP</p>
            </section>

            <section class="right-panel">
                <div class="recu-box">

                    <div class="recu-header">
                        <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="small-logo" />
                        <div>
                            <h2>Recuperar contraseña</h2>
                            <p>Clave temporal enviada automáticamente por WhatsApp</p>
                        </div>
                    </div>

                    <div class="info-box">
                        Ingresa tu cédula y celular registrado. La clave temporal llegará directo por WhatsApp.
                    </div>

                    <div class="input-group">
                        <span class="icon">🪪</span>
                        <asp:TextBox
                            ID="txtCedula"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            MaxLength="10"
                            placeholder="Cédula">
                        </asp:TextBox>
                    </div>

                    <div class="input-group">
                        <span class="icon">📱</span>
                        <asp:TextBox
                            ID="txtCelular"
                            runat="server"
                            ClientIDMode="Static"
                            CssClass="input"
                            MaxLength="10"
                            placeholder="Celular registrado">
                        </asp:TextBox>
                    </div>

                    <asp:Button
                        ID="btnEnviar"
                        runat="server"
                        ClientIDMode="Static"
                        Text="Enviar clave con Green API"
                        CssClass="btn-main"
                        OnClick="btnEnviar_Click" />

                    <div class="message-box">
                        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                    </div>

                    <a href="<%= ResolveUrl("~/Seguridad/recucontra/tiporecu.aspx") %>" class="btn-back">Volver</a>
                    <p class="meta">REQUIEM SECURITY SYSTEM</p>

                </div>
            </section>

        </div>

        <div id="modalExito" class="modal">
            <div class="modal-content">
                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
                <h3>Clave enviada</h3>
                <p>Tu clave temporal fue enviada automáticamente por WhatsApp.</p>
                <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="btn-main">Volver al login</a>
            </div>
        </div>

    </form>

    <script src="<%= ResolveUrl("~/js/recuperarwhatsapp.js") %>"></script>
</body>
</html>
