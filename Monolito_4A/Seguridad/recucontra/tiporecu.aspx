<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="tiporecu.aspx.cs" Inherits="Monolito_4A.Seguridad.recucontra.tiporecu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Tipo recuperación | Requiem</title>

    <link href="~/css/tiporecu.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <div class="recu-page">

            <section class="left-panel">
                <div class="logo-wrapper">
                    <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                </div>

                <h1>T.M.W<br />
                    <span>requiem</span>
                </h1>

                <p class="subtitle">RECUPERACIÓN SEGURA DE ACCESO</p>
            </section>

            <section class="right-panel">
                <div class="recu-box">

                    <div class="recu-header">
                        <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="small-logo" />
                        <div>
                            <h2>Recuperar contraseña</h2>
                            <p>Selecciona el método de recuperación</p>
                        </div>
                    </div>

                    <div class="method-grid">

                        <asp:LinkButton
                            ID="btnCorreo"
                            runat="server"
                            CssClass="method-card"
                            OnClick="btnCorreo_Click">
                            <span class="method-icon">✉</span>
                            <strong>Recuperar por correo</strong>
                            <small>Recibe o cambia tu contraseña usando tu correo registrado.</small>
                        </asp:LinkButton>

                        <asp:LinkButton
                            ID="btnWhatsapp"
                            runat="server"
                            CssClass="method-card"
                            OnClick="btnWhatsapp_Click">
                            <span class="method-icon">📱</span>
                            <strong>Recuperar por WhatsApp</strong>
                            <small>Recibe una clave temporal mediante WhatsApp.</small>
                        </asp:LinkButton>

                        <asp:LinkButton
                            ID="btnGreen"
                            runat="server"
                            CssClass="method-card"
                            OnClick="btnGreen_Click">
                            <span class="method-icon">🟢</span>
                            <strong>Recuperar por Green API</strong>
                            <small>Recibe una clave temporal automática por WhatsApp usando Green API.</small>
                        </asp:LinkButton>

                    </div>

                    <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="btn-back">Volver al login</a>
                    <p class="meta">REQUIEM SECURITY SYSTEM</p>

                </div>
            </section>

        </div>

    </form>
</body>
</html>
