
var localStorageKeys = {
    lightTheme: "light-theme",
    fontSize: "font-size",
    fontFamily: "font-family",
    fontWeight: "font-weight",
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
        switch (tamanioGuardado) {
            case "small":
                document.body.classList.add('mr-setting-text-small');
                document.body.classList.remove('mr-setting-text-large');
                break;
            case "medium":
                document.body.classList.remove('mr-setting-text-small');
                document.body.classList.remove('mr-setting-text-large');
                break;
            case "large":
                document.body.classList.remove('mr-setting-text-small');
                document.body.classList.add('mr-setting-text-large');
                break;
        }
    }

    const fontFamily = getCacheItem(localStorageKeys.fontFamily, "josefine");
    if (fontFamily) {
        switch (fontFamily) {
            case "josefine":
                document.body.classList.add('mr-setting-font-josefine');
                document.body.classList.remove('mr-setting-font-sans-serif');
                break;
            case "sans-serif":
                document.body.classList.remove('mr-setting-font-josefine');
                document.body.classList.add('mr-setting-font-sans-serif');
                break;
        }
    }

    const fontWeight = getCacheItem(localStorageKeys.fontWeight, "normal");
    if (fontWeight) {
        switch (fontWeight) {
            case "light":
                document.body.classList.add('mr-setting-font-weight-light');
                document.body.classList.remove('mr-setting-font-weight-bold');
                break;
            case "normal":
                document.body.classList.remove('mr-setting-font-weight-light');
                document.body.classList.remove('mr-setting-font-weight-bold');
                break;
            case "bold":
                document.body.classList.remove('mr-setting-font-weight-light');
                document.body.classList.add('mr-setting-font-weight-bold');
                break;
        }
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
    $("#fontFamilySelector").val(getCacheItem(localStorageKeys.fontFamily, "josefine"));
}


function cambiarPeso() {
    var selector = document.getElementById('fontWeightSelector');
    var tamanio = selector.options[selector.selectedIndex].value;
    
    // Guardar la preferencia en localStorage para recordarla en futuras visitas
    setCacheItem(localStorageKeys.fontWeight, tamanio);

    loadConfig();
}

function changeFontWeight() {
    $("#fontWeightSelector").val(getCacheItem(localStorageKeys.fontWeight, "normal"));
}
