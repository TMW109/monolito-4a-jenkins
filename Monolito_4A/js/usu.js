function animarBoton(btn) {
    if (!btn) return true;

    btn.classList.remove("pulse");

    setTimeout(function () {
        btn.classList.add("pulse");
    }, 10);

    return true;
}

window.onload = function () {
    const input = document.getElementById("txtNumero");

    if (input) {
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
};