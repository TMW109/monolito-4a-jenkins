<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Usu.aspx.cs" Inherits="Monolito_4A.Usu" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Usuario | Requiem</title>

    <link href="~/css/usu.css" rel="stylesheet" runat="server" />
</head>

<body>
    <form id="form1" runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>

                <div class="user-page">

                    <aside class="sidebar">
                        <div class="logo-box">
                            <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="logo" />
                        </div>

                        <h1>T.M.W SYSTEM</h1>
                        <p>requiem</p>

                        <nav class="menu">
                            <a class="active">🎮 Juego</a>
                            <a>👤 Perfil usuario</a>

                            <asp:LinkButton
                                ID="btnCerrarSesion"
                                runat="server"
                                CssClass="menu-link"
                                OnClick="btnCerrarSesion_Click">
                                🚪 Cerrar sesión
                            </asp:LinkButton>
                        </nav>
                    </aside>

                    <main class="content">

                        <section class="header-card">
                            <div>
                                <h2>Panel de Usuario</h2>
                                <p>
                                    Bienvenido,
                                    <asp:Label ID="lblUsuario" runat="server" Text="Usuario"></asp:Label>
                                </p>
                            </div>

                            <div class="status">
                                <span>🧬 Misión activa</span>
                            </div>
                        </section>

                        <section class="game-card">

                            <div class="game-header">
                                <img src="<%= ResolveUrl("~/imagen/imglogo.png") %>" class="game-logo" />
                                <div>
                                    <h3>Desactivar el virus</h3>
                                    <p>Encuentra el código secreto del sistema entre 1 y 20.</p>
                                </div>
                            </div>

                            <asp:Panel ID="pnlInicio" runat="server" CssClass="start-box">
                                <p>
                                    El sistema REQUIEM detectó una amenaza digital.
                                    Tu misión es descubrir el código correcto antes de quedarte sin intentos.
                                </p>

                                <asp:Button
                                    ID="btnIniciarJuego"
                                    runat="server"
                                    Text="Iniciar juego"
                                    CssClass="btn-main"
                                    OnClick="btnIniciarJuego_Click"
                                    OnClientClick="animarBoton(this);" />
                            </asp:Panel>

                            <asp:Panel ID="pnlJuego" runat="server" CssClass="play-box" Visible="false">

                                <div class="game-info">
                                    <div>
                                        <strong>Intentos restantes</strong>
                                        <asp:Label ID="lblIntentos" runat="server" Text="5"></asp:Label>
                                    </div>

                                    <div>
                                        <strong>Rango</strong>
                                        <span>1 - 20</span>
                                    </div>
                                </div>

                                <div class="input-game">
                                    <span>☣</span>
                                    <asp:TextBox
                                        ID="txtNumero"
                                        runat="server"
                                        ClientIDMode="Static"
                                        CssClass="input"
                                        MaxLength="2"
                                        placeholder="Ingresa código"></asp:TextBox>
                                </div>

                                <asp:Button
                                    ID="btnVerificar"
                                    runat="server"
                                    Text="Verificar código"
                                    CssClass="btn-main"
                                    OnClick="btnVerificar_Click"
                                    OnClientClick="animarBoton(this);" />

                                <asp:Button
                                    ID="btnReiniciar"
                                    runat="server"
                                    Text="Reiniciar juego"
                                    CssClass="btn-secondary"
                                    OnClick="btnReiniciar_Click" />

                            </asp:Panel>

                            <div class="message-box">
                                <asp:Label ID="lblMensaje" runat="server" Text=""></asp:Label>
                            </div>

                        </section>

                    </main>

                </div>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>

    <script src="<%= ResolveUrl("~/js/usu.js") %>"></script>
</body>
</html>
