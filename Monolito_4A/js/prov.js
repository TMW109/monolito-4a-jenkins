function cargarProveedor() {
    soloTextoProveedor("txtNombreProveedor");
    activarBusquedaInstantanea();
}

if (window.Sys && Sys.Application) {
    Sys.Application.add_load(cargarProveedor);
} else {
    window.addEventListener("load", cargarProveedor);
}

function activarBusquedaInstantanea() {
    const input = document.getElementById("txtBuscar");
    const tabla = document.getElementById("gvProveedores");

    if (!input || !tabla || input.dataset.busqueda === "ok") return;

    input.dataset.busqueda = "ok";

    input.addEventListener("input", function () {
        const texto = input.value.toLowerCase().trim();
        const filas = tabla.querySelectorAll("tr");

        for (let i = 1; i < filas.length; i++) {
            const fila = filas[i];

            if (fila.querySelector(".empty")) continue;

            const contenido = fila.innerText.toLowerCase();

            if (contenido.includes(texto)) {
                fila.style.display = "";
            } else {
                fila.style.display = "none";
            }
        }
    });
}

function validarProveedor() {
    const nombre = valor("txtNombreProveedor");

    if (!nombre) {
        mostrarModalProveedor("Faltan campos", "Ingrese el nombre del proveedor.");
        return false;
    }

    if (nombre.length < 3) {
        mostrarModalProveedor("Nombre muy corto", "El nombre debe tener mínimo 3 caracteres.");
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s.\-]+$/.test(nombre)) {
        mostrarModalProveedor("Nombre inválido", "Solo se permiten letras, números, espacios, punto y guion.");
        return false;
    }

    return true;
}

function validarArchivoProveedor() {
    const archivo = document.getElementById("fuProveedor");

    if (!archivo || archivo.files.length === 0) {
        mostrarModalProveedor("Archivo requerido", "Debe seleccionar un archivo Excel o CSV.");
        return false;
    }

    const file = archivo.files[0];
    const nombre = file.name.toLowerCase();

    const extensionesPermitidas = [".csv", ".xls", ".xlsx"];
    const extensionValida = extensionesPermitidas.some(ext => nombre.endsWith(ext));

    if (!extensionValida) {
        mostrarModalProveedor("Archivo inválido", "Solo se permiten archivos Excel o CSV: .csv, .xls, .xlsx.");
        archivo.value = "";
        return false;
    }

    const maxMb = 50;
    const maxBytes = maxMb * 1024 * 1024;

    if (file.size > maxBytes) {
        mostrarModalProveedor("Archivo muy pesado", "El archivo no debe superar " + maxMb + " MB.");
        archivo.value = "";
        return false;
    }

    if (!nombre.endsWith(".csv")) {
        mostrarModalProveedor(
            "Formato pendiente",
            "El sistema acepta Excel o CSV, pero por ahora la lectura está implementada para CSV. Guarde el Excel como CSV UTF-8."
        );
        archivo.value = "";
        return false;
    }

    return true;
}

function valor(id) {
    const control = document.getElementById(id);
    return control ? control.value.trim() : "";
}

function soloTextoProveedor(id) {
    const input = document.getElementById(id);
    if (!input || input.dataset.eventos === "ok") return;

    input.dataset.eventos = "ok";

    input.addEventListener("keypress", function (e) {
        if (!/[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s.\-]/.test(e.key)) {
            e.preventDefault();
        }
    });

    input.addEventListener("paste", function (e) {
        e.preventDefault();

        const texto = (e.clipboardData || window.clipboardData).getData("text");
        input.value = texto.replace(/[^A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s.\-]/g, "");
    });
}

function mostrarModalProveedor(titulo, mensaje) {
    const modal = document.getElementById("modalProveedor");

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

function cerrarModalProveedor() {
    const modal = document.getElementById("modalProveedor");

    if (modal) {
        modal.style.display = "none";
    }
}