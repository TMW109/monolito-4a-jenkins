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

function validarAdminUsuario() {
    const id = valor("hfUsuarioId");
    const cedula = valor("txtCedula");
    const nombres = valor("txtNombres");
    const apellidos = valor("txtApellidos");
    const direccion = valor("txtDireccion");
    const celular = valor("txtCelular");
    const correo = valor("txtCorreo");
    const fecha = valor("txtFechaCumple");
    const nick = valor("txtNick");
    const password = valor("txtPassword");
    const tipo = valor("ddlTipoUsuario");

    if (!cedula || !nombres || !apellidos || !direccion || !celular || !correo || !fecha || !nick || tipo === "0") {
        mostrarModalAdmin("Faltan campos", "Complete todos los campos obligatorios.");
        return false;
    }

    if (!validarCedulaEcuatoriana(cedula)) {
        mostrarModalAdmin("Cédula inválida", "Ingrese una cédula ecuatoriana válida.");
        return false;
    }


    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(nombres)) {
        mostrarModalAdmin("Dato incorrecto", "Los nombres solo deben contener letras.");
        return false;
    }

    if (!/^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/.test(apellidos)) {
        mostrarModalAdmin("Dato incorrecto", "Los apellidos solo deben contener letras.");
        return false;
    }

    if (!/^[0-9]{10}$/.test(celular)) {
        mostrarModalAdmin("Dato incorrecto", "El celular debe tener 10 dígitos.");
        return false;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) {
        mostrarModalAdmin("Correo inválido", "Ingrese un correo válido.");
        return false;
    }
    if (!esMayorDeEdad(fecha)) {
        mostrarModalAdmin("Edad no permitida", "El usuario debe tener mínimo 18 años.");
        return false;
    }

    if (!id) {
        if (!password) {
            mostrarModalAdmin("Contraseña requerida", "Ingrese contraseña para nuevo usuario.");
            return false;
        }

        if (!passwordSegura(password)) {
            mostrarModalAdmin("Contraseña débil", "La contraseña debe tener mínimo 8 caracteres, mayúscula, minúscula, número y símbolo.");
            return false;
        }

        const previewContainer = document.getElementById("previewContainer");
        if (!previewContainer || previewContainer.getElementsByTagName("img").length === 0) {
            mostrarModalAdmin("Fotos requeridas", "Primero debe seleccionar y previsualizar al menos una foto.");
            return false;
        }
    } else {
        if (password && !passwordSegura(password)) {
            mostrarModalAdmin("Contraseña débil", "La nueva contraseña debe tener mínimo 8 caracteres, mayúscula, minúscula, número y símbolo.");
            return false;
        }
    }

    return true;
}

function validarFotoCliente() {
    const foto = document.getElementById("fuFotos");

    if (!foto || foto.files.length === 0) {
        mostrarModalAdmin("Fotos requeridas", "Seleccione al menos una foto.");
        return false;
    }

    const maximo = 5 * 1024 * 1024;

    for (let i = 0; i < foto.files.length; i++) {
        const archivo = foto.files[i];
        const nombre = archivo.name.toLowerCase();

        if (!nombre.endsWith(".jpg") && !nombre.endsWith(".jpeg") && !nombre.endsWith(".png")) {
            mostrarModalAdmin("Archivo no permitido", "Solo se permiten imágenes JPG o PNG.");
            foto.value = "";
            return false;
        }

        if (archivo.size > maximo) {
            mostrarModalAdmin("Imagen muy pesada", "Cada foto no debe superar los 5 MB.");
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
        if (!/[0-9]/.test(e.key)) e.preventDefault();
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
        if (!/[A-Za-zÁÉÍÓÚáéíóúÑñ\s]/.test(e.key)) e.preventDefault();
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

function mostrarModalAdmin(titulo, mensaje) {
    const modal = document.getElementById("modalAdminUsu");

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

function cerrarModalAdmin() {
    const modal = document.getElementById("modalAdminUsu");
    if (modal) modal.style.display = "none";
}

function abrirModalImagenDesdeSrc(src) {
    const modal = document.getElementById("modalImagen");
    const imgModal = document.getElementById("imgModalPreview");

    if (!modal || !imgModal) return;

    imgModal.src = src;
    modal.style.display = "flex";
}

function cerrarModalImagen() {
    const modal = document.getElementById("modalImagen");
    if (modal) modal.style.display = "none";
}

function validarCedulaEcuatoriana(cedula) {
    if (!/^[0-9]{10}$/.test(cedula)) return false;

    const provincia = parseInt(cedula.substring(0, 2), 10);
    if (provincia < 1 || provincia > 24) return false;

    const tercerDigito = parseInt(cedula.substring(2, 3), 10);
    if (tercerDigito >= 6) return false;

    let suma = 0;

    for (let i = 0; i < 9; i++) {
        let digito = parseInt(cedula.substring(i, i + 1), 10);

        if (i % 2 === 0) {
            digito *= 2;
            if (digito > 9) digito -= 9;
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