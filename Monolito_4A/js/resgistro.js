window.onload = function () {
    soloNumeros("txtCedula");
    soloNumeros("txtCelular");
    soloLetras("txtNombres");
    soloLetras("txtApellidos");

    const pass = document.getElementById("txtPassword");

    if (pass) {
        pass.addEventListener("keyup", validarReglasPassword);
        pass.addEventListener("change", validarReglasPassword);
        validarReglasPassword();
    }
};

function iniciarRegistro() {
    if (!validarRegistro()) {
        return false;
    }

    const progress = document.getElementById("progressRegistro");
    const btn = document.getElementById("btnRegistrar");

    if (progress) {
        progress.style.display = "block";
    }

    if (btn) {
        btn.value = "Registrando...";
        btn.style.pointerEvents = "none";
        btn.style.opacity = "0.8";
    }

    return true;
}

function validarRegistro() {
    const cedula = valor("txtCedula");
    const nombres = valor("txtNombres");
    const apellidos = valor("txtApellidos");
    const direccion = valor("txtDireccion");
    const celular = valor("txtCelular");
    const fecha = valor("txtFechaCumple");
    if (!esMayorDeEdad(fecha)) {
        mostrarModalMensaje("Edad no permitida", "Debe tener mínimo 18 años para registrarse.");
        return false;
    }
    const tipo = valor("ddlTipoUsuario");

    if (!cedula || !nombres || !apellidos || !direccion || !celular || !fecha || tipo === "0") {
        mostrarModalMensaje("Faltan campos", "Complete todos los campos obligatorios.");
        return false;
    }

    if (!validarCedulaEcuatoriana(cedula)) {
        mostrarModalMensaje("Cédula inválida", "Ingrese una cédula ecuatoriana válida.");
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(nombres)) {
        mostrarModalMensaje("Dato incorrecto", "Los nombres solo deben contener letras.");
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(apellidos)) {
        mostrarModalMensaje("Dato incorrecto", "Los apellidos solo deben contener letras.");
        return false;
    }

    if (!/^[0-9]{10}$/.test(celular)) {
        mostrarModalMensaje("Dato incorrecto", "El teléfono debe tener 10 números.");
        return false;
    }

    const previewContainer = document.getElementById("previewContainer");

    if (!previewContainer || previewContainer.getElementsByTagName("img").length === 0) {
        mostrarModalMensaje("Fotos requeridas", "Primero debe seleccionar y previsualizar al menos una foto.");
        return false;
    }

    return true;
}

function validarFotoCliente() {
    const foto = document.getElementById("fuFoto");

    if (!foto || foto.files.length === 0) {
        mostrarModalMensaje("Fotos requeridas", "Seleccione al menos una foto.");
        return false;
    }

    const maximo = 5 * 1024 * 1024;

    for (let i = 0; i < foto.files.length; i++) {
        const archivo = foto.files[i];
        const nombre = archivo.name.toLowerCase();

        const extensionValida =
            nombre.endsWith(".jpg") ||
            nombre.endsWith(".jpeg") ||
            nombre.endsWith(".png");

        if (!extensionValida) {
            mostrarModalMensaje("Archivo no permitido", "Solo se permiten imágenes JPG o PNG.");
            foto.value = "";
            return false;
        }

        if (archivo.size > maximo) {
            mostrarModalMensaje("Imagen muy pesada", "Cada foto no debe superar los 5 MB.");
            foto.value = "";
            return false;
        }
    }

    return true;
}

function valor(id) {
    const control = document.getElementById(id);
    return control ? control.value.trim() : "";
}

function soloNumeros(id) {
    const input = document.getElementById(id);
    if (!input) return;

    input.addEventListener("keypress", function (e) {
        if (!/[0-9]/.test(e.key)) {
            e.preventDefault();
        }
    });

    input.addEventListener("paste", function (e) {
        e.preventDefault();
        const texto = (e.clipboardData || window.clipboardData).getData("text");
        input.value = texto.replace(/\D/g, "");
    });
}

function soloLetras(id) {
    const input = document.getElementById(id);
    if (!input) return;

    input.addEventListener("keypress", function (e) {
        if (!/[A-Za-zÁÉÍÓÚáéíóúÑñ\s]/.test(e.key)) {
            e.preventDefault();
        }
    });

    input.addEventListener("paste", function (e) {
        e.preventDefault();
        const texto = (e.clipboardData || window.clipboardData).getData("text");
        input.value = texto.replace(/[^A-Za-zÁÉÍÓÚáéíóúÑñ\s]/g, "");
    });
}

function passwordSegura(password) {
    return password.length >= 8 &&
        /[A-Z]/.test(password) &&
        /[a-z]/.test(password) &&
        /[0-9]/.test(password) &&
        /[^A-Za-z0-9]/.test(password);
}

function validarReglasPassword() {
    const input = document.getElementById("txtPassword");
    if (!input) return;

    const password = input.value;

    cambiarRegla("ruleLength", password.length >= 8, "Mínimo 8 caracteres");
    cambiarRegla("ruleUpper", /[A-Z]/.test(password), "Una letra mayúscula");
    cambiarRegla("ruleLower", /[a-z]/.test(password), "Una letra minúscula");
    cambiarRegla("ruleNumber", /[0-9]/.test(password), "Un número");
    cambiarRegla("ruleSymbol", /[^A-Za-z0-9]/.test(password), "Un símbolo");
}

function cambiarRegla(id, valido, texto) {
    const regla = document.getElementById(id);
    if (!regla) return;

    if (valido) {
        regla.classList.add("valid");
        regla.innerHTML = "✔ " + texto;
    } else {
        regla.classList.remove("valid");
        regla.innerHTML = "✖ " + texto;
    }
}

function mostrarPassword(idCampo, boton) {
    const input = document.getElementById(idCampo);
    if (!input) return;

    if (input.type === "password") {
        input.type = "text";
        boton.innerHTML = "🙈";
    } else {
        input.type = "password";
        boton.innerHTML = "👁";
    }
}

function mostrarModalMensaje(titulo, mensaje) {
    const modal = document.getElementById("modalRegistro");
    if (!modal) return;

    const h3 = modal.querySelector("h3");
    const p = modal.querySelector("p");

    if (h3) h3.innerText = titulo;
    if (p) p.innerText = mensaje;

    window.redirigirLogin = false;
    modal.style.display = "flex";
}

function mostrarModalRegistro() {
    const progress = document.getElementById("progressRegistro");
    const btn = document.getElementById("btnRegistrar");
    const modal = document.getElementById("modalRegistro");

    if (progress) progress.style.display = "none";

    if (btn) {
        btn.value = "Registrar agente";
        btn.style.pointerEvents = "auto";
        btn.style.opacity = "1";
    }

    if (modal) {
        const h3 = modal.querySelector("h3");
        const p = modal.querySelector("p");

        if (h3) h3.innerText = "Agente creado con éxito";
        if (p) p.innerText = "El nuevo agente fue registrado correctamente. Redirigiendo al login...";

        window.redirigirLogin = true;
        modal.style.display = "flex";

        setTimeout(function () {
            window.location.href = "Login.aspx";
        }, 2500);
    }
}

function cerrarModal() {
    const modal = document.getElementById("modalRegistro");

    if (modal) {
        modal.style.display = "none";
    }

    if (window.redirigirLogin) {
        window.location.href = "Login.aspx";
    }
}

function abrirModalImagenDesdeSrc(src) {
    const modal = document.getElementById("modalImagen");
    const imgModal = document.getElementById("imgModalPreview");

    if (!modal || !imgModal) return;

    imgModal.src = src;
    modal.style.display = "flex";
}

function abrirModalImagen() {
    const preview = document.querySelector("#previewContainer img");
    const modal = document.getElementById("modalImagen");
    const imgModal = document.getElementById("imgModalPreview");

    if (!preview || !modal || !imgModal) return;

    imgModal.src = preview.src;
    modal.style.display = "flex";
}

function cerrarModalImagen() {
    const modal = document.getElementById("modalImagen");

    if (modal) {
        modal.style.display = "none";
    }
}

window.addEventListener("load", function () {
    document.querySelectorAll("input").forEach(function (input) {
        input.setAttribute("autocomplete", "nope");
    });

    document.querySelectorAll("input[type='password']").forEach(function (input) {
        input.setAttribute("autocomplete", "new-password");
    });
});


function validarCedulaEcuatoriana(cedula) {
    if (!/^[0-9]{10}$/.test(cedula)) {
        return false;
    }

    const provincia = parseInt(cedula.substring(0, 2), 10);
    if (provincia < 1 || provincia > 24) {
        return false;
    }

    const tercerDigito = parseInt(cedula.substring(2, 3), 10);
    if (tercerDigito >= 6) {
        return false;
    }

    let suma = 0;

    for (let i = 0; i < 9; i++) {
        let digito = parseInt(cedula.substring(i, i + 1), 10);

        if (i % 2 === 0) {
            digito = digito * 2;

            if (digito > 9) {
                digito = digito - 9;
            }
        }

        suma += digito;
    }

    const digitoVerificador = parseInt(cedula.substring(9, 10), 10);
    const residuo = suma % 10;
    const resultado = residuo === 0 ? 0 : 10 - residuo;

    return resultado === digitoVerificador;
}

function esMayorDeEdad(fechaTexto) {
    if (!fechaTexto) return false;

    const nacimiento = new Date(fechaTexto + "T00:00:00");
    const hoy = new Date();

    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const mes = hoy.getMonth() - nacimiento.getMonth();

    if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }

    return edad >= 18;
}