function validarWhatsapp() {
    const cedula = document.getElementById("txtCedula").value.trim();
    const celular = document.getElementById("txtCelular").value.trim();

    if (cedula === "" || celular === "") {
        alert("Ingrese la cédula y el celular registrado.");
        return false;
    }

    if (!/^[0-9]{10}$/.test(cedula)) {
        alert("La cédula debe tener 10 dígitos.");
        return false;
    }

    if (!/^[0-9]{10}$/.test(celular)) {
        alert("El celular debe tener 10 dígitos.");
        return false;
    }

    return true;
}

window.onload = function () {
    soloNumeros("txtCedula");
    soloNumeros("txtCelular");
};

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