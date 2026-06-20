<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificarOTP.aspx.cs" Inherits="Monolito_4A.VerificarOTP" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Verificación OTP | Requiem</title>

    <link href="~/css/login.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <div class="login-page">

                    <!-- IZQUIERDA -->
                    <section class="left-panel">
                        <div class="logo-wrapper">
                            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                        </div>

                        <h1>T.M.W<br />
                            <span>requiem</span></h1>

                        <p class="subtitle">Doble verificación · Umbrella Security</p>

                        <div class="cards">
                            <div class="card card-one">
                                <img src="<%= ResolveUrl("~/imagen/bsaa.jpg") %>" />
                            </div>

                            <div class="card card-two">
                                <img src="<%= ResolveUrl("~/imagen/segurity.jpg") %>" />
                            </div>

                            <div class="card card-three">
                                <img src="<%= ResolveUrl("~/imagen/wolf.jpg") %>" />
                            </div>
                        </div>
                    </section>

                    <!-- DERECHA -->
                    <section class="right-panel">
                        <div class="login-box">

                            <div class="login-header">
                                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="small-logo" />
                                <div>
                                    <h2>Verifica que eres tú</h2>
                                    <p>Control de doble acceso</p>
                                </div>
                            </div>

                            <div class="password-rules">
                                <p>Se envió un código QR a tu correo.</p>
                                <p>Muestra el QR a la cámara para verificar automáticamente.</p>
                            </div>

                            <!-- 🔥 CAMPO OCULTO -->
                            <asp:HiddenField
                                ID="txtCodigoOTP"
                                runat="server"
                                ClientIDMode="Static" />

                            <!-- 🔥 BOTÓN OCULTO -->
                            <asp:Button
                                ID="btnVerificar"
                                runat="server"
                                ClientIDMode="Static"
                                Text="Verificar"
                                OnClick="btnVerificar_Click"
                                Style="display: none;" />

                            <!-- 🔥 CÁMARA -->
                            <div id="reader" style="width: 100%; max-width: 320px; margin: auto;"></div>

                            <div class="separator"></div>

                            <a href="<%= ResolveUrl("~/Seguridad/Login.aspx") %>" class="btn-create">Volver al login</a>
                            <p class="meta">REQUIEM SECURITY SYSTEM</p>

                        </div>
                    </section>

                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>

    <!-- MODAL ERROR -->
    <div id="modalError" class="modal">
        <div class="modal-content">
            <img src="../imagen/imglogo.png" class="modal-logo" />
            <h3>Acceso denegado</h3>
            <p>Código incorrecto o sesión expirada.</p>
            <button type="button" onclick="cerrarModalError()">Continuar</button>
        </div>
    </div>
    <div id="modalBienvenida" class="modal">
        <div class="modal-content">
            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="modal-logo" />
            <h3>Bienvenido</h3>
            <p>Verificación correcta. Accediendo al sistema...</p>
            <button type="button" onclick="irAdmin()">Continuar</button>
        </div>
    </div>

    <!-- LIBRERÍA -->
    <script src="https://unpkg.com/html5-qrcode"></script>

    <script>
        let scanner;

        function iniciarScanner() {

            scanner = new Html5Qrcode("reader");

            Html5Qrcode.getCameras().then(cameras => {
                if (cameras.length) {

                    scanner.start(
                        cameras[0].id,
                        {
                            fps: 15,
                            qrbox: { width: 320, height: 320 },
                            aspectRatio: 1.0
                        },
                        (decodedText) => {
                            console.log("QR leído:", decodedText);

                            document.getElementById("txtCodigoOTP").value = decodedText;

                            scanner.stop().then(() => {
                                setTimeout(function () {
                                    document.getElementById("btnVerificar").click();
                                }, 300);
                            });
                        },
                        (errorMessage) => {
                            // normal mientras busca QR
                        }
                    );

                }
            }).catch(err => {
                console.error("Error cámara:", err);
            });
        }

        window.onload = function () {
            iniciarScanner();
        };

        function mostrarModalError() {
            document.getElementById("modalError").style.display = "flex";
        }

        function cerrarModalError() {
            document.getElementById("modalError").style.display = "none";
        }

        function mostrarModalBienvenida() {
            document.getElementById("modalBienvenida").style.display = "flex";

            setTimeout(function () {
                window.location.href = "<%= ResolveUrl("~/admin.aspx") %>";
            }, 2000);
        }

        function irAdmin() {
            window.location.href = "<%= ResolveUrl("~/admin.aspx") %>";
        }
    </script>

</body>
</html>
