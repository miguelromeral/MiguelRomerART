
var localStorageKeys = {
    lightTheme: "light-theme",
    fontSize: "font-size",
    fontFamily: "font-family",
    fontWeight: "font-weight",
    showKudos: "show-kudos",
}

var settingsControl = {
    selectorFontSize: "#fontSizeSelector",
    selectorFontFamily: "#fontFamilySelector",
    selectorFontWeight: "#fontWeightSelector",
    selectorShowKudos: "#showKudosSelector",
}

var settingsClasses = {
    lightTheme: "light-theme",
    textSize: {
        small: "mr-setting-text-small",
        large: "mr-setting-text-large",
    },
    textFamily: {
        josefine: "mr-setting-font-josefine",
        sansSerif: "mr-setting-font-sans-serif",
    },
    textWeight: {
        light: "mr-setting-font-weight-light",
        bold: "mr-setting-font-weight-bold",
    },
    showKudos: "mr-hide-kudos",
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
        document.body.classList.add(settingsClasses.lightTheme);
    }

    const tamanioGuardado = getCacheItem(localStorageKeys.fontSize, "medium");
    if (tamanioGuardado) {
        switch (tamanioGuardado) {
            case "small":
                document.body.classList.add(settingsClasses.textSize.small);
                document.body.classList.remove(settingsClasses.textSize.large);
                break;
            case "medium":
                document.body.classList.remove(settingsClasses.textSize.small);
                document.body.classList.remove(settingsClasses.textSize.large);
                break;
            case "large":
                document.body.classList.remove(settingsClasses.textSize.small);
                document.body.classList.add(settingsClasses.textSize.large);
                break;
        }
    }

    const fontFamily = getCacheItem(localStorageKeys.fontFamily, "josefine");
    if (fontFamily) {
        switch (fontFamily) {
            case "josefine":
                document.body.classList.add(settingsClasses.textFamily.josefine);
                document.body.classList.remove(settingsClasses.textFamily.sansSerif);
                break;
            case "sans-serif":
                document.body.classList.remove(settingsClasses.textFamily.josefine);
                document.body.classList.add(settingsClasses.textFamily.sansSerif);
                break;
        }
    }

    const fontWeight = getCacheItem(localStorageKeys.fontWeight, "normal");
    if (fontWeight) {
        switch (fontWeight) {
            case "light":
                document.body.classList.add(settingsClasses.textWeight.light);
                document.body.classList.remove(settingsClasses.textWeight.bold);
                break;
            case "normal":
                document.body.classList.remove(settingsClasses.textWeight.light);
                document.body.classList.remove(settingsClasses.textWeight.bold);
                break;
            case "bold":
                document.body.classList.remove(settingsClasses.textWeight.light);
                document.body.classList.add(settingsClasses.textWeight.bold);
                break;
        }
    }

    const showKudos = getCacheItem(localStorageKeys.showKudos, "show");
    if (showKudos) {
        switch (showKudos) {
            case "show":
                document.body.classList.remove(settingsClasses.showKudos);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.showKudos);
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
        document.body.classList.add(settingsClasses.lightTheme);
        $("#theme-switch").addClass("btn-dark");
        $("#theme-switch").removeClass("btn-light");
    } else {
        document.body.classList.remove(settingsClasses.lightTheme);
        $("#theme-switch").addClass("btn-light");
        $("#theme-switch").removeClass("btn-dark");
    }
}

function changeFontSize() {
    $(settingsControl.selectorFontSize).val(getCacheItem(localStorageKeys.fontSize, "medium"));
}

function cambiarTamanio() {
    setCacheItem(localStorageKeys.fontSize, $(settingsControl.selectorFontSize).val());
    loadConfig();
}


function eventChangeFontFamily() {
    setCacheItem(localStorageKeys.fontFamily, $(settingsControl.selectorFontFamily).val());
    loadConfig();
}
function changeFontFamily() {
    $(settingsControl.selectorFontFamily).val(getCacheItem(localStorageKeys.fontFamily, "josefine"));
}


function cambiarPeso() {
    setCacheItem(localStorageKeys.fontWeight, $(settingsControl.selectorFontWeight).val());
    loadConfig();
}

function changeFontWeight() {
    $(settingsControl.selectorFontWeight).val(getCacheItem(localStorageKeys.fontWeight, "normal"));
}



function eventChangeShowKudos() {
    setCacheItem(localStorageKeys.showKudos, $(settingsControl.selectorShowKudos).val());
    loadConfig();
}

function changeShowKudos() {
    $(settingsControl.showKudos).val(getCacheItem(localStorageKeys.showKudos, "show"));
}