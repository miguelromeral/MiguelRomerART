
var localStorageKeys = {
    lightTheme: "light-theme",
    fontSize: "font-size",
    fontFamily: "font-family",
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

        var sizeSmall = "small";
        var sizeLarge = "large";

        switch (tamanioGuardado) {
            case "small":
                sizeSmall = "x-small";
                sizeLarge = "medium";
                break;
            case "large":
                sizeSmall = "medium";
                sizeLarge = "x-large";
                break;
        }

        document.documentElement.style.setProperty('--mr-font-size-small', sizeSmall);
        document.documentElement.style.setProperty('--mr-font-size-default', tamanioGuardado);
        document.documentElement.style.setProperty('--mr-font-size-large', sizeLarge);
    }

    const fontFamily = getCacheItem(localStorageKeys.fontFamily, "var(--mr-font-family-default)");
    if (fontFamily) {
        document.documentElement.style.setProperty('--mr-font-family', fontFamily);
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


    // Guardar la preferencia en localStorage para recordarla en futuras visitas
    setCacheItem(localStorageKeys.fontSize, tamanio);

    loadConfig();
}


function eventChangeFontFamily() {
    var selector = document.getElementById('fontFamilySelector');
    var family = selector.options[selector.selectedIndex].value;
    
    setCacheItem(localStorageKeys.fontFamily, family);

    loadConfig();
}
function changeFontFamily() {
    $("#fontFamilySelector").val(getCacheItem(localStorageKeys.fontFamily, "var(--mr-font-family-default)"));
}
