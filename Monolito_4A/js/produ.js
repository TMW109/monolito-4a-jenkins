function iniciarValidacionesProducto() {
    soloNumeros("txtCantidad");
    soloDecimal("txtPrecio");
}

if (window.Sys && Sys.Application) {
    Sys.Application.add_load(iniciarValidacionesProducto);
} else {
    window.addEventListener("load", iniciarValidacionesProducto);
}

function validarProductoEditar() {

    const nombre = valor("txtNombre");
    const cantidad = valor("txtCantidad");
    const precio = valor("txtPrecio");
    const proveedor = valor("ddlProveedor");

    if (!nombre || !cantidad || !precio || proveedor === "0") {
        mostrarModalProducto(
            "Faltan campos",
            "Complete todos los campos obligatorios."
        );
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s\-_.]+$/.test(nombre)) {
        mostrarModalProducto(
            "Nombre inválido",
            "El nombre solo debe contener letras, números y espacios."
        );
        return false;
    }

    if (!/^[0-9]+$/.test(cantidad) || parseInt(cantidad) < 0) {
        mostrarModalProducto(
            "Cantidad inválida",
            "La cantidad debe ser un número entero válido."
        );
        return false;
    }

    const precioNormalizado = precio.replace(",", ".");

    if (
        !/^[0-9]+([.,][0-9]{1,2})?$/.test(precio) ||
        parseFloat(precioNormalizado) <= 0
    ) {
        mostrarModalProducto(
            "Precio inválido",
            "El precio debe ser mayor a cero y máximo con 2 decimales."
        );
        return false;
    }

    return true;
}

function validarProducto() {

    const nombre = valor("txtNombre");
    const cantidad = valor("txtCantidad");
    const precio = valor("txtPrecio");
    const proveedor = valor("ddlProveedor");

    if (!nombre || !cantidad || !precio || proveedor === "0") {
        mostrarModalProducto(
            "Faltan campos",
            "Complete todos los campos obligatorios."
        );
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s\-_.]+$/.test(nombre)) {
        mostrarModalProducto(
            "Nombre inválido",
            "El nombre solo debe contener letras, números y espacios."
        );
        return false;
    }

    if (!/^[0-9]+$/.test(cantidad) || parseInt(cantidad) < 0) {
        mostrarModalProducto(
            "Cantidad inválida",
            "La cantidad debe ser un número entero válido."
        );
        return false;
    }

    const precioNormalizado = precio.replace(",", ".");

    if (
        !/^[0-9]+([.,][0-9]{1,2})?$/.test(precio) ||
        parseFloat(precioNormalizado) <= 0
    ) {
        mostrarModalProducto(
            "Precio inválido",
            "El precio debe ser mayor a cero y máximo con 2 decimales."
        );
        return false;
    }

    const previewContainer = document.getElementById("previewContainer");

    if (
        previewContainer &&
        previewContainer.getElementsByTagName("img").length === 0
    ) {
        mostrarModalProducto(
            "Imágenes requeridas",
            "Primero debe seleccionar y previsualizar al menos una imagen."
        );
        return false;
    }

    return true;
}

function validarFotosProducto() {

    const fotos = document.getElementById("fuFotos");

    if (!fotos || fotos.files.length === 0) {
        mostrarModalProducto(
            "Imágenes requeridas",
            "Seleccione al menos una imagen."
        );
        return false;
    }

    return validarArchivosImagen(fotos);
}

function validarFotosProductoEditar() {

    const fotos = document.getElementById("fuFotos");

    if (!fotos || fotos.files.length === 0) {
        mostrarModalProducto(
            "Imágenes requeridas",
            "Seleccione al menos una imagen para previsualizar."
        );
        return false;
    }

    return validarArchivosImagen(fotos);
}

function validarArchivosImagen(fotos) {

    const maximo = 5 * 1024 * 1024;

    for (let i = 0; i < fotos.files.length; i++) {

        const archivo = fotos.files[i];
        const nombre = archivo.name.toLowerCase();

        const extensionValida =
            nombre.endsWith(".jpg") ||
            nombre.endsWith(".jpeg") ||
            nombre.endsWith(".png");

        if (!extensionValida) {

            mostrarModalProducto(
                "Archivo no permitido",
                "Solo se permiten imágenes JPG o PNG."
            );

            fotos.value = "";
            return false;
        }

        if (archivo.size > maximo) {

            mostrarModalProducto(
                "Imagen muy pesada",
                "Cada imagen no debe superar los 5 MB."
            );

            fotos.value = "";
            return false;
        }
    }

    return true;
}

function valor(id) {

    const control = document.getElementById(id);

    return control
        ? control.value.trim()
        : "";
}

function soloNumeros(id) {

    const input = document.getElementById(id);

    if (!input || input.dataset.eventos === "ok")
        return;

    input.dataset.eventos = "ok";

    input.addEventListener("keypress", function (e) {

        if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });

    input.addEventListener("paste", function (e) {

        e.preventDefault();

        const texto =
            (e.clipboardData || window.clipboardData)
                .getData("text");

        input.value = texto.replace(/\D/g, "");
    });
}

function soloDecimal(id) {

    const input = document.getElementById(id);

    if (!input || input.dataset.decimal === "ok")
        return;

    input.dataset.decimal = "ok";

    input.addEventListener("keypress", function (e) {

        if (!/[0-9.,]/.test(e.key)) {
            e.preventDefault();
        }

        if (
            (e.key === "." || e.key === ",") &&
            (input.value.includes(".") || input.value.includes(","))
        ) {
            e.preventDefault();
        }
    });

    input.addEventListener("paste", function (e) {

        e.preventDefault();

        let texto =
            (e.clipboardData || window.clipboardData)
                .getData("text");

        texto = texto.replace(/[^0-9.,]/g, "");

        input.value = texto;
    });
}

function mostrarModalProducto(titulo, mensaje) {

    const modal = document.getElementById("modalProducto");

    if (!modal) {
        alert(mensaje);
        return;
    }

    const h3 = modal.querySelector("h3");
    const p = modal.querySelector("p");

    if (h3) h3.innerText = titulo;
    if (p) p.innerText = mensaje;

    modal.style.display = "flex";
}

function cerrarModalProducto() {

    const modal = document.getElementById("modalProducto");

    if (modal)
        modal.style.display = "none";
}

function abrirModalImagenDesdeSrc(src) {

    const modal = document.getElementById("modalImagen");
    const imgModal = document.getElementById("imgModalPreview");

    if (!modal || !imgModal)
        return;

    imgModal.src = src;
    modal.style.display = "flex";
}

function cerrarModalImagen() {

    const modal = document.getElementById("modalImagen");

    if (modal)
        modal.style.display = "none";
}