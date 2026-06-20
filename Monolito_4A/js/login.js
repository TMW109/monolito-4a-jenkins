function iniciarCargaLogin() {
    const cedula = document.getElementById("txtced").value.trim();
    const password = document.getElementById("txtPassword").value.trim();

    if (cedula === "" || password === "") {
        mostrarModalError();
        return false;
    }
    if (!validarCedulaEcuatoriana(cedula)) {
        mostrarModalError();
        return false;
    }

    const progress = document.getElementById("progressLogin");
    const btn = document.getElementById("btnLogin");

    if (progress) progress.style.display = "block";

    if (btn) {
        btn.value = "Verificando...";
        btn.style.pointerEvents = "none";
        btn.style.opacity = "0.8";
    }

    guardarRecordarme();

    return true;
}

function cargarLogin() {
    document.querySelectorAll("input").forEach(function (input) {
        input.setAttribute("autocomplete", "nope");
    });

    document.querySelectorAll("input[type='password']").forEach(function (input) {
        input.setAttribute("autocomplete", "new-password");
    });

    const cedula = document.getElementById("txtced");
    const recordarme = document.getElementById("recordarme");
    const cedulaGuardada = localStorage.getItem("cedula_recordada");

    if (cedulaGuardada && cedula && recordarme) {
        cedula.value = cedulaGuardada;
        recordarme.checked = true;
    }
}

if (window.Sys && Sys.Application) {
    Sys.Application.add_load(cargarLogin);
} else {
    window.addEventListener("load", cargarLogin);
}

function guardarRecordarme() {
    const cedula = document.getElementById("txtced");
    const recordarme = document.getElementById("recordarme");

    if (!cedula || !recordarme) return;

    if (recordarme.checked) {
        localStorage.setItem("cedula_recordada", cedula.value.trim());
    } else {
        localStorage.removeItem("cedula_recordada");
    }
}

function mostrarModalBienvenida() {
    ocultarCargaLogin();

    const modal = document.getElementById("modalBienvenida");
    if (modal) modal.style.display = "flex";
}

function mostrarModalError() {
    ocultarCargaLogin();

    const modal = document.getElementById("modalError");
    if (modal) modal.style.display = "flex";
}

function mostrarModalBloqueado() {
    ocultarCargaLogin();

    const modal = document.getElementById("modalBloqueado");
    if (modal) modal.style.display = "flex";
}

function ocultarCargaLogin() {
    const progress = document.getElementById("progressLogin");
    const btn = document.getElementById("btnLogin");

    if (progress) progress.style.display = "none";

    if (btn) {
        btn.value = "Acceder al sistema";
        btn.style.pointerEvents = "auto";
        btn.style.opacity = "1";
    }
}

function cerrarModal() {
    const modal = document.getElementById("modalBienvenida");
    if (modal) modal.style.display = "none";
}

function cerrarModalBienvenidaFondo(event) {
    if (event.target.id === "modalBienvenida") {
        cerrarModal();
    }
}

function cerrarModalError() {
    const modal = document.getElementById("modalError");
    if (modal) modal.style.display = "none";
}

function cerrarModalErrorFondo(event) {
    if (event.target.id === "modalError") {
        cerrarModalError();
    }
}

function cerrarModalBloqueado() {
    const modal = document.getElementById("modalBloqueado");
    if (modal) modal.style.display = "none";
}

function cerrarModalBloqueadoFondo(event) {
    if (event.target.id === "modalBloqueado") {
        cerrarModalBloqueado();
    }
}

function mostrarPassword() {
    const password = document.getElementById("txtPassword");

    if (!password) return;

    if (password.type === "password") {
        password.type = "text";
    } else {
        password.type = "password";
    }
}

function irRecuperarPassword() {
    window.location.href = "recucontra/tiporecu.aspx";
}

function validarCedulaEcuatoriana(cedula) {
    if (!/^[0-9]{10}$/.test(cedula)) return false;

    const provincia = parseInt(cedula.substring(0, 2), 10);
    if (provincia < 1 || provincia > 24) return false;

    const tercerDigito = parseInt(cedula[2], 10);
    if (tercerDigito >= 6) return false;

    let suma = 0;

    for (let i = 0; i < 9; i++) {
        let num = parseInt(cedula[i], 10);

        if (i % 2 === 0) {
            num *= 2;
            if (num > 9) num -= 9;
        }

        suma += num;
    }

    const digitoVerificador = parseInt(cedula[9], 10);
    const decena = Math.ceil(suma / 10) * 10;
    const resultado = decena - suma;

    const verificador = resultado === 10 ? 0 : resultado;

    return verificador === digitoVerificador;
}


function bloquearRegresoLogin() {
    window.history.forward();

    window.onpageshow = function (evt) {
        if (evt.persisted) {
            window.history.forward();
        }
    };

    window.onunload = function () { };
}

if (window.Sys && Sys.Application) {
    Sys.Application.add_load(bloquearRegresoLogin);
} else {
    window.addEventListener("load", bloquearRegresoLogin);
}