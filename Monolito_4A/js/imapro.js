window.onload = function () {
    inicializarImagenProducto();
};

function inicializarImagenProducto() {
    const nombre = document.getElementById("txtNombre");
    const imagen = document.getElementById("fuImagen");
    const previsualizada = document.getElementById("hfPrevisualizada");

    if (nombre) {
        nombre.setAttribute("autocomplete", "nope");
    }

    if (imagen && previsualizada) {
        imagen.addEventListener("change", function () {
            previsualizada.value = "0";
        });
    }
}

function validarImagenCliente() {
    const nombre = document.getElementById("txtNombre");
    const imagen = document.getElementById("fuImagen");

    if (!nombre || nombre.value.trim() === "") {
        mostrarModalMensaje("Nombre requerido", "Ingrese el nombre de la imagen antes de previsualizar.");
        return false;
    }

    if (!imagen || imagen.files.length === 0) {
        mostrarModalMensaje("Imagen requerida", "Seleccione al menos una imagen.");
        return false;
    }

    return validarArchivosImagen(imagen);
}

function validarFormularioImagen() {
    const nombre = document.getElementById("txtNombre");
    const previsualizada = document.getElementById("hfPrevisualizada");
    const imagenId = document.getElementById("hfImagenId");

    const editando = imagenId && imagenId.value.trim() !== "";

    if (!nombre || nombre.value.trim() === "") {
        mostrarModalMensaje("Nombre requerido", "Ingrese el nombre de la imagen.");
        return false;
    }

    if (nombre.value.trim().length < 3) {
        mostrarModalMensaje("Nombre muy corto", "El nombre debe tener al menos 3 caracteres.");
        return false;
    }

    if (!editando && (!previsualizada || previsualizada.value !== "1")) {
        mostrarModalMensaje("Previsualización requerida", "Primero previsualice la imagen antes de guardarla.");
        return false;
    }

    return true;
}

function validarArchivosImagen(input) {
    const maximo = 5 * 1024 * 1024;

    for (let i = 0; i < input.files.length; i++) {
        const archivo = input.files[i];
        const nombre = archivo.name.toLowerCase();

        const extensionValida =
            nombre.endsWith(".jpg") ||
            nombre.endsWith(".jpeg") ||
            nombre.endsWith(".png") ||
            nombre.endsWith(".webp");

        if (!extensionValida) {
            mostrarModalMensaje("Archivo no permitido", "Solo se permiten imágenes JPG, PNG o WEBP.");
            input.value = "";
            return false;
        }

        if (archivo.size > maximo) {
            mostrarModalMensaje("Imagen muy pesada", "Cada imagen no debe superar los 5 MB.");
            input.value = "";
            return false;
        }
    }

    return true;
}

function mostrarModalMensaje(titulo, mensaje) {
    const modal = document.getElementById("modalMensaje");
    const tituloModal = document.getElementById("tituloModal");
    const textoModal = document.getElementById("textoModal");

    if (!modal || !tituloModal || !textoModal) return;

    tituloModal.innerText = titulo;
    textoModal.innerText = mensaje;
    modal.style.display = "flex";
}

function cerrarModalMensaje() {
    const modal = document.getElementById("modalMensaje");
    if (modal) modal.style.display = "none";
}

function abrirModalImagen(src) {
    const modal = document.getElementById("modalImagenProducto");
    const imagen = document.getElementById("imgProductoGrande");

    if (!modal || !imagen) return;

    imagen.src = src;
    modal.style.display = "flex";
}

function abrirModalImagenDesdeSrc(src) {
    abrirModalImagen(src);
}

function cerrarModalImagen() {
    const modal = document.getElementById("modalImagenProducto");
    if (modal) modal.style.display = "none";
}

function validarArchivoImagenCsv() {
    const archivo = document.getElementById("fuImagenCsv");

    if (!archivo || archivo.files.length === 0) {
        mostrarModalMensaje("Archivo requerido", "Seleccione un archivo CSV de imágenes.");
        return false;
    }

    const file = archivo.files[0];
    const nombre = file.name.toLowerCase();

    if (!nombre.endsWith(".csv")) {
        mostrarModalMensaje("Archivo inválido", "Solo se permite archivo .csv.");
        archivo.value = "";
        return false;
    }

    const maxMb = 10;
    const maxBytes = maxMb * 1024 * 1024;

    if (file.size > maxBytes) {
        mostrarModalMensaje("Archivo muy pesado", "El CSV de imágenes no debe superar " + maxMb + " MB.");
        archivo.value = "";
        return false;
    }

    return true;
}