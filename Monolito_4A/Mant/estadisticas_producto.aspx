<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="estadisticas_producto.aspx.cs" Inherits="Monolito_4A.Mant.estadisticas_producto" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Estadísticas producto | Requiem</title>

    <link href="<%= ResolveUrl("~/css/estadisticas_producto.css") %>" rel="stylesheet" />
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
                    <a href="<%= ResolveUrl("~/admin.aspx") %>">☣ Inicio</a>
                    <a>👤 Perfil administrador</a>
                    <a>🔓 Desbloquear usuarios</a>
                    <a href="<%= ResolveUrl("~/Mant/accion.aspx") %>">📦 Gestión productos</a>
                    <a href="<%= ResolveUrl("~/Mant/nuevo_tbl_proveedor.aspx") %>">🏭 Proveedores</a>
                </nav>

                <a href="<%= ResolveUrl("~/Mant/listar_tbl_producto.aspx") %>" class="logout-link">Volver al listado</a>
            </aside>

            <main class="content">

                <header class="topbar">
                    <div>
                        <h1>Estadísticas producto</h1>
                        <p>Visualización de imágenes, datos y valores del producto.</p>
                    </div>

                    <div class="profile-card">
                        <asp:Image ID="imgPerfil" runat="server" CssClass="profile-img" />
                        <div>
                            <asp:Label ID="lblNombre" runat="server" CssClass="profile-name" />
                            <span>Administrador</span>
                        </div>
                    </div>
                </header>

                <section class="table-box">
                    <div class="table-header">
                        <h3>Producto seleccionado</h3>
                        <asp:Label ID="lblMensaje" runat="server" CssClass="message" />
                    </div>

                    <div class="product-header">
                        <div>
                            <h2>
                                <asp:Label ID="lblProducto" runat="server" /></h2>
                            <p>
                                Proveedor:
                                <asp:Label ID="lblProveedor" runat="server" />
                            </p>
                        </div>

                        <div class="badge">
                            ID:
                            <asp:Label ID="lblId" runat="server" />
                        </div>
                    </div>

                    <div class="stats-grid">
                        <div class="stat-card">
                            <span>📦</span>
                            <small>Cantidad</small>
                            <strong>
                                <asp:Label ID="lblCantidad" runat="server" /></strong>
                        </div>

                        <div class="stat-card">
                            <span>💵</span>
                            <small>Precio</small>
                            <strong>$
                                <asp:Label ID="lblPrecio" runat="server" /></strong>
                        </div>

                        <div class="stat-card">
                            <span>📊</span>
                            <small>Total inventario</small>
                            <strong>$
                                <asp:Label ID="lblTotal" runat="server" /></strong>
                        </div>
                    </div>
                </section>

                <section class="table-box">
                    <div class="table-header">
                        <h3>Carrusel de imágenes</h3>
                    </div>

                    <div class="carousel-box">
                        <button type="button" class="carousel-btn" onclick="moverCarrusel(-1)">‹</button>

                        <div class="carousel-track" id="carouselTrack">
                            <asp:Repeater ID="rpImagenes" runat="server">
                                <ItemTemplate>
                                    <div class="carousel-item">
                                        <img src='<%# ResolveUrl(Eval("pimg_ruta").ToString()) %>'
                                            onclick="abrirModalImagen(this.src)" />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <button type="button" class="carousel-btn" onclick="moverCarrusel(1)">›</button>
                    </div>
                </section>

                <section class="table-box">
                    <div class="table-header">
                        <h3>Gráfico del producto</h3>
                    </div>

                    <canvas id="chartProducto" height="130"></canvas>
                </section>
                <section class="table-box">
                    <div class="table-header">
                        <h3>Estadísticas generales</h3>
                    </div>

                    <div class="stats-grid">
                        <div class="stat-card">
                            <span>🏆</span>
                            <small>Mayor stock</small>
                            <strong>
                                <asp:Label ID="lblMayorStock" runat="server" /></strong>
                        </div>

                        <div class="stat-card">
                            <span>💎</span>
                            <small>Producto más caro</small>
                            <strong>
                                <asp:Label ID="lblMasCaro" runat="server" /></strong>
                        </div>

                        <div class="stat-card">
                            <span>📦</span>
                            <small>Productos activos</small>
                            <strong>
                                <asp:Label ID="lblProductosActivos" runat="server" /></strong>
                        </div>

                        <div class="stat-card">
                            <span>💰</span>
                            <small>Inventario general</small>
                            <strong>$
                                <asp:Label ID="lblInventarioGeneral" runat="server" /></strong>
                        </div>
                    </div>
                </section>

                <section class="table-box">
                    <div class="table-header">
                        <h3>Productos por proveedor</h3>
                    </div>

                    <asp:GridView ID="gvProductosProveedor" runat="server"
                        AutoGenerateColumns="False"
                        CssClass="admin-table"
                        GridLines="None">

                        <Columns>
                            <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                            <asp:BoundField DataField="CantidadProductos" HeaderText="Productos" />
                        </Columns>

                        <EmptyDataTemplate>
                            <div class="empty">No existen datos para mostrar.</div>
                        </EmptyDataTemplate>

                    </asp:GridView>
                </section>

            </main>

        </div>

        <asp:HiddenField ID="hfCantidad" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfPrecio" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hfTotal" runat="server" ClientIDMode="Static" />

    </form>

    <div id="modalImagenProducto" class="modal">
        <div class="modal-content modal-img-box">
            <img id="imgProductoGrande" src="" />
            <button type="button" onclick="cerrarModalImagen()">Cerrar</button>
        </div>
    </div>

    <script>
        let indiceCarrusel = 0;

        function moverCarrusel(direccion) {
            const track = document.getElementById("carouselTrack");
            const items = document.querySelectorAll(".carousel-item");

            if (!track || items.length === 0) return;

            indiceCarrusel += direccion;

            if (indiceCarrusel < 0) indiceCarrusel = items.length - 1;
            if (indiceCarrusel >= items.length) indiceCarrusel = 0;

            track.style.transform = "translateX(" + (-indiceCarrusel * 100) + "%)";
        }

        function abrirModalImagen(src) {
            document.getElementById("imgProductoGrande").src = src;
            document.getElementById("modalImagenProducto").style.display = "flex";
        }

        function cerrarModalImagen() {
            document.getElementById("modalImagenProducto").style.display = "none";
        }

        function dibujarGrafico() {
            const canvas = document.getElementById("chartProducto");
            if (!canvas) return;

            canvas.width = canvas.offsetWidth;
            canvas.height = 330;

            const ctx = canvas.getContext("2d");

            const cantidad = parseFloat(document.getElementById("hfCantidad").value || "0");
            const precio = parseFloat(document.getElementById("hfPrecio").value || "0");
            const total = parseFloat(document.getElementById("hfTotal").value || "0");

            const datos = [
                { nombre: "Cantidad", valor: cantidad },
                { nombre: "Precio", valor: precio },
                { nombre: "Total", valor: total }
            ];

            const max = Math.max(cantidad, precio, total, 1);
            const baseY = 260;
            const altoMax = 190;
            const anchoBarra = 90;
            const espacio = 95;
            const inicioX = 95;

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            ctx.fillStyle = "#30363b";
            ctx.font = "bold 18px Segoe UI";
            ctx.fillText("Resumen del producto seleccionado", 30, 35);

            datos.forEach((d, i) => {
                const x = inicioX + i * (anchoBarra + espacio);
                const alto = (d.valor / max) * altoMax;

                ctx.fillStyle = "#8f1111";
                ctx.fillRect(x, baseY - alto, anchoBarra, alto);

                ctx.fillStyle = "#30363b";
                ctx.font = "bold 15px Segoe UI";
                ctx.fillText(d.nombre, x, baseY + 28);

                ctx.fillStyle = "#8f1111";
                ctx.font = "bold 14px Segoe UI";
                ctx.fillText(d.valor.toFixed(2), x - 8, baseY - alto - 12);
            });
        }

        window.addEventListener("load", dibujarGrafico);
        window.addEventListener("resize", dibujarGrafico);
    </script>

</body>
</html>
