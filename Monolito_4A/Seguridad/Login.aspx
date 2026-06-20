<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Monolito_4A.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login | Requiem</title>

    <link href="~/css/login.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server" autocomplete="off">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <div class="login-page">

                    <section class="left-panel">
                        <div class="logo-wrapper">
                            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="umbrella-logo" />
                        </div>

                        <h1>T.M.W<br />
                            <span>requiem</span>
                        </h1>

                        <p class="subtitle">Acceso restringido · Umbrella Security</p>

                        <div class="cards">
                            <div class="card card-one">
                                <img src="<%= ResolveUrl("~/imagen/elpis.jpg") %>" />
                            </div>

                            <div class="card card-two">
                                <img src="<%= ResolveUrl("~/imagen/plaga.png") %>" />
                            </div>

                            <div class="card card-three">
                                <img src="<%= ResolveUrl("~/imagen/OIP.png") %>" />
                            </div>
                        </div>
                    </section>

                    <section class="right-panel">
                        <div class="login-box">

                            <div class="login-header">
                                <img src="../imagen/imglogo.png" class="small-logo" />
                                <div>
                                    <h2>Iniciar sesión</h2>
                                    <p>Control de acceso</p>
                                </div>
                            </div>

                            <div class="input-group">
                                <span class="icon">☣</span>
                                <asp:TextBox
                                    ID="txtced"
                                    runat="server"
                                    ClientIDMode="Static"
                                    CssClass="input"
                                    AutoCompleteType="Disabled"
                                    autocomplete="nope"
                                    placeholder="Cedula">

                                </asp:TextBox>
                            </div>

                            <div class="input-group">
                                <span class="icon">🔒</span>
                                <asp:TextBox
                                    ID="txtPassword"
                                    runat="server"
                                    ClientIDMode="Static"
                                    CssClass="input"
                                    TextMode="Password"
                                    AutoCompleteType="Disabled"
                                    autocomplete="new-password"
                                    placeholder="Contraseña segura">
                                </asp:TextBox>

                                <span class="show-pass" onclick="mostrarPassword()">👁</span>
                            </div>

                            <div class="password-rules">
                                <p id="ruleLength">✖ Mínimo 8 caracteres</p>
                                <p id="ruleUpper">✖ Una letra mayúscula</p>
                                <p id="ruleLower">✖ Una letra minúscula</p>
                                <p id="ruleNumber">✖ Un número</p>
                                <p id="ruleSymbol">✖ Un símbolo</p>
                            </div>

                            <div class="options">
                                <label>
                                    <input type="checkbox" id="recordarme" />
                                    Recordarme
                                </label>

                                <a href="<%= ResolveUrl("~/Seguridad/recucontra/tiporecu.aspx") %>">¿Olvidaste tu contraseña?</a>
                            </div>

                            <asp:Button
                                ID="btnLogin"
                                runat="server"
                                ClientIDMode="Static"
                                Text="Acceder al sistema"
                                CssClass="btn-login"
                                OnClientClick="return iniciarCargaLogin();"
                                OnClick="btnLogin_Click" />

                            <div id="progressLogin" class="progress-box">
                                <div class="progress-bar">
                                    <div class="progress-fill"></div>
                                </div>
                                <p>Verificando credenciales...</p>
                            </div>

                            <div class="separator"></div>

                            <button type="button" class="btn-social google">Conectarse con Google</button>

                            <a href='<%= ResolveUrl("~/Seguridad/recucontra/tiporecu.aspx") %>' class="btn-social github">Recuperar contraseña
                            </a>

                            <button type="button" class="btn-secondary">Acceso externo</button>

                            <a href='<%= ResolveUrl("~/Seguridad/Registro.aspx") %>' class="btn-create">Registrar nuevo agente
                            </a>

                            <p class="meta">REQUIEM SECURITY SYSTEM</p>

                        </div>
                    </section>

                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>

    <div id="modalBienvenida" class="modal" onclick="cerrarModalBienvenidaFondo(event)">
        <div class="modal-content" onclick="event.stopPropagation()">
            <button type="button" class="modal-close" onclick="cerrarModal()">×</button>

            <img src="../imagen/imglogo.png" class="modal-logo" />
            <h3>Acceso autorizado</h3>
            <p>Bienvenido al sistema REQUIEM.</p>

            <button type="button" onclick="cerrarModal()">Continuar</button>
        </div>
    </div>

    <div id="modalError" class="modal" onclick="cerrarModalErrorFondo(event)">
        <div class="modal-content" onclick="event.stopPropagation()">
            <button type="button" class="modal-close" onclick="cerrarModalError()">×</button>

            <img src="../imagen/imglogo.png" class="modal-logo" />
            <h3>Acceso denegado</h3>
            <p>Cédula o contraseña incorrecta.</p>

            <button type="button" onclick="cerrarModalError()">Continuar</button>
        </div>
    </div>

    <div id="modalBloqueado" class="modal" onclick="cerrarModalBloqueadoFondo(event)">
        <div class="modal-content" onclick="event.stopPropagation()">

            <button type="button" class="modal-close" onclick="cerrarModalBloqueado()">×</button>

            <img src="../imagen/imglogo.png" class="modal-logo" />
            <h3>Cuenta bloqueada</h3>
            <p>Has superado el límite de intentos.</p>

            <a href='<%= ResolveUrl("~/Seguridad/recucontra/tiporecu.aspx") %>' class="btn-social">Recuperar contraseña
            </a>

        </div>
    </div>

    <script src="<%= ResolveUrl("~/js/login.js") %>"></script>
</body>
</html>
