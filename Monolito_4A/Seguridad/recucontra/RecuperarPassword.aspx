<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarPassword.aspx.cs" Inherits="Monolito_4A.Seguridad.recucontra.RecuperarPassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Recuperar contraseña | Requiem</title>
    <link href="~/css/recu.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="recu-page">

            <section class="recu-panel-left">
                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="logo-main" />
                <h1>T.M.W</h1>
                <span>requiem</span>
                <p>Recuperación segura de acceso</p>
            </section>

            <section class="recu-panel-right">
                <div class="recu-box">

                    <div class="recu-header">
                        <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" />
                        <div>
                            <h2>Recuperar contraseña</h2>
                            <p>Verifica tus datos y crea una nueva contraseña</p>
                        </div>
                    </div>

                    <asp:HiddenField ID="hfUsuarioId" runat="server" />

                    <div class="input-group">
                        <span>☣</span>
                        <asp:TextBox ID="txtCedula" runat="server" CssClass="input" placeholder="Cédula"></asp:TextBox>
                    </div>

                    <div class="input-group">
                        <span>✉</span>
                        <asp:TextBox ID="txtCorreo" runat="server" CssClass="input" placeholder="Correo registrado"></asp:TextBox>
                    </div>

                    <asp:Button ID="btnVerificar" runat="server"
                        Text="Verificar usuario"
                        CssClass="btn-recu"
                        OnClick="btnVerificar_Click" />

                    <div class="input-group">
                        <span>🔒</span>
                        <asp:TextBox ID="txtNuevaPassword" runat="server" ClientIDMode="Static"
                            CssClass="input password-input" TextMode="Password"
                            placeholder="Nueva contraseña" Enabled="false"></asp:TextBox>
                        <span class="eye" onclick="verPassword('txtNuevaPassword')">👁</span>
                    </div>

                    <div class="input-group">
                        <span>🔒</span>
                        <asp:TextBox ID="txtConfirmarPassword" runat="server" ClientIDMode="Static"
                            CssClass="input password-input" TextMode="Password"
                            placeholder="Confirmar contraseña" Enabled="false"></asp:TextBox>
                        <span class="eye" onclick="verPassword('txtConfirmarPassword')">👁</span>
                    </div>

                    <asp:Button ID="btnCambiar" runat="server"
                        Text="Cambiar contraseña"
                        CssClass="btn-recu"
                        Enabled="false"
                        OnClick="btnCambiar_Click" />

                    <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="btn-volver">Volver al login</a>
                    <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

                </div>
            </section>

        </div>

        <div id="modalOk" class="modal">
            <div class="modal-content">
                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
                <h3>Cambios realizados</h3>
                <p>Tu contraseña fue actualizada correctamente.</p>
            </div>
        </div>

    </form>

    <script>
        function verPassword(id) {
            var input = document.getElementById(id);
            if (!input) return;

            input.type = input.type === "password" ? "text" : "password";
        }
        function mostrarModalOk() {
            const modal = document.getElementById("modalOk");
            if (modal) modal.style.display = "flex";

            setTimeout(function () {
                window.location.href = "<%= ResolveUrl("~/Seguridad/Login.aspx") %>";
            }, 2500);
        }
    </script>

</body>
</html>
