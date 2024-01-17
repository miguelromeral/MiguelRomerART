
var localStorageKeys = {
    lightTheme: "light-theme",
    fontSize: "font-size",
}

function getCacheItem(key, defaultValue) {
    if (typeof defaultValue == "boolean") {
        return JSON.parse(localStorage.getItem(key)) || defaultValue;
    } else {
        return localStorage.getItem(key) ?? defaultValue;
    }
}
function setCacheItem(key, value) {
    localStorage.setItem(key, value);
}

function loadConfig() {
    const isLightMode = getCacheItem(localStorageKeys.lightTheme, false);
    if (isLightMode) {
        document.body.classList.add('light-theme');
    }

    const tamanioGuardado = getCacheItem(localStorageKeys.fontSize, "medium");
    if (tamanioGuardado) {
        document.documentElement.style.setProperty('--mr-font-size-default', tamanioGuardado);
    }
}



function toggleTheme() {
    var isLightMode = getCacheItem(localStorageKeys.lightTheme, false);
    setCacheItem(localStorageKeys.lightTheme, !isLightMode);
    changeLightModeButton();
}

function changeLightModeButton() {
    if (getCacheItem(localStorageKeys.lightTheme, false)) {
        document.body.classList.add("light-theme");
        $("#theme-switch").addClass("btn-dark");
        $("#theme-switch").removeClass("btn-light");
    } else {
        document.body.classList.remove("light-theme");
        $("#theme-switch").addClass("btn-light");
        $("#theme-switch").removeClass("btn-dark");
    }
}

function changeFontSize() {
    $("#fontSizeSelector").val(getCacheItem(localStorageKeys.fontSize, "medium"));
}

function cambiarTamanio() {
    var selector = document.getElementById('fontSizeSelector');
    var tamanio = selector.options[selector.selectedIndex].value;

    console.log("--> " + tamanio);

    // Guardar la preferencia en localStorage para recordarla en futuras visitas
    setCacheItem(localStorageKeys.fontSize, tamanio);

    loadConfig();
}