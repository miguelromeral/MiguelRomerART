
var localStorageKeys = {
    lightTheme: "light-theme",
    fontSize: "font-size",
    fontFamily: "font-family",
    fontWeight: "font-weight",
    showKudos: "show-kudos",
    hideViews: "hide-views",
    hideSpotify: "hide-spotify",
    hideCommentPros: "hide-comment-pros",
    hideCommentCons: "hide-comment-cons",
    hideScoreMiguel: "hide-score-miguel",
    hideScorePopular: "hide-score-popular",
}




var localStorageDefaultValues = {
    lightTheme: false,
    fontSize: "medium",
    fontFamily: "josefine",
    fontWeight: "normal",
    showKudos: "show",
    hideViews: "show",
    hideSpotify: "show",
    hideCommentPros: "show",
    hideCommentCons: "show",
    hideScoreMiguel: "show",
    hideScorePopular: "show",
}

var settingsControl = {
    selectorFontSize: "#fontSizeSelector",
    selectorFontFamily: "#fontFamilySelector",
    selectorFontWeight: "#fontWeightSelector",
    selectorShowKudos: "#showKudosSelector",
    selectorHideViews: "#hideViewsSelector",
    selectorHideSpotify: "#hideSpotifySelector",
    selectorHideCommentPros: "#hideCommentProsSelector",
    selectorHideCommentCons: "#hideCommentConsSelector",
    selectorHideScoreMiguel: "#hideScoreMiguelSelector",
    selectorHideScorePopular: "#hideScorePopularSelector",
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
    hideViews: "mr-hide-views",
    hideSpotify: "mr-hide-spotify",
    hideCommentPros: "mr-hide-comment-pros",
    hideCommentCons: "mr-hide-comment-cons",
    hideScoreMiguel: "mr-hide-score-miguel",
    hideScorePopular: "mr-hide-score-popular",
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
    loadLightTheme();
    loadFontSize();
    loadFontFamily();
    loadFontWeight();
    loadShowKudos();
    loadHideViews();
    loadHideSpotify();
    loadHideCommentPros();
    loadHideCommentCons();
    loadHideScoreMiguel();
    loadHideScorePopular();
}



function toggleTheme() {
    var isLightMode = getCacheItem(localStorageKeys.lightTheme, localStorageDefaultValues.lightTheme);
    setCacheItem(localStorageKeys.lightTheme, !isLightMode);
    loadConfig();
}

function changeLightModeButton() {
    if (getCacheItem(localStorageKeys.lightTheme, localStorageDefaultValues.lightTheme)) {
        document.body.classList.add(settingsClasses.lightTheme);
        //$("#theme-switch").addClass("btn-dark");
        //$("#theme-switch").removeClass("btn-light");
    } else {
        document.body.classList.remove(settingsClasses.lightTheme);
        //$("#theme-switch").addClass("btn-light");
        //$("#theme-switch").removeClass("btn-dark");
    }
}

function loadLightTheme() {
    const isLightMode = getCacheItem(localStorageKeys.lightTheme, localStorageDefaultValues.lightTheme);
    if (isLightMode) {
        document.body.classList.add(settingsClasses.lightTheme);
    } else {
        document.body.classList.remove(settingsClasses.lightTheme);
    }
}

function changeFontSize() {
    $(settingsControl.selectorFontSize).val(getCacheItem(localStorageKeys.fontSize, localStorageDefaultValues.fontSize));
}

function cambiarTamanio() {
    setCacheItem(localStorageKeys.fontSize, $(settingsControl.selectorFontSize).val());
        loadConfig();
}

function loadFontSize() {

    const tamanioGuardado = getCacheItem(localStorageKeys.fontSize, localStorageDefaultValues.fontSize);
    if (tamanioGuardado) {
        switch (tamanioGuardado) {
            case "small":
                document.body.classList.add(settingsClasses.textSize.small);
                document.body.classList.remove(settingsClasses.textSize.large);
                break;
            case localStorageDefaultValues.fontSize:
                document.body.classList.remove(settingsClasses.textSize.small);
                document.body.classList.remove(settingsClasses.textSize.large);
                break;
            case "large":
                document.body.classList.remove(settingsClasses.textSize.small);
                document.body.classList.add(settingsClasses.textSize.large);
                break;
        }
    }
}


function eventChangeFontFamily() {
    setCacheItem(localStorageKeys.fontFamily, $(settingsControl.selectorFontFamily).val());
        loadConfig();
}
function changeFontFamily() {
    $(settingsControl.selectorFontFamily).val(getCacheItem(localStorageKeys.fontFamily, localStorageDefaultValues.fontFamily));
}

function loadFontFamily() {
    const fontFamily = getCacheItem(localStorageKeys.fontFamily, localStorageDefaultValues.fontFamily);
    if (fontFamily) {
        switch (fontFamily) {
            case localStorageDefaultValues.fontFamily:
                document.body.classList.add(settingsClasses.textFamily.josefine);
                document.body.classList.remove(settingsClasses.textFamily.sansSerif);
                break;
            case "sans-serif":
                document.body.classList.remove(settingsClasses.textFamily.josefine);
                document.body.classList.add(settingsClasses.textFamily.sansSerif);
                break;
        }
    }
}

function cambiarPeso() {
    setCacheItem(localStorageKeys.fontWeight, $(settingsControl.selectorFontWeight).val());
        loadConfig();
}

function changeFontWeight() {
    $(settingsControl.selectorFontWeight).val(getCacheItem(localStorageKeys.fontWeight, localStorageDefaultValues.fontWeight));
}

function loadFontWeight() {

    const fontWeight = getCacheItem(localStorageKeys.fontWeight, localStorageDefaultValues.fontWeight);
    if (fontWeight) {
        switch (fontWeight) {
            case "light":
                document.body.classList.add(settingsClasses.textWeight.light);
                document.body.classList.remove(settingsClasses.textWeight.bold);
                break;
            case localStorageDefaultValues.fontWeight:
                document.body.classList.remove(settingsClasses.textWeight.light);
                document.body.classList.remove(settingsClasses.textWeight.bold);
                break;
            case "bold":
                document.body.classList.remove(settingsClasses.textWeight.light);
                document.body.classList.add(settingsClasses.textWeight.bold);
                break;
        }
    }
}


function eventChangeShowKudos() {
    setCacheItem(localStorageKeys.showKudos, $(settingsControl.selectorShowKudos).val());
    loadConfig();
}

function changeShowKudos() {
    $(settingsControl.selectorShowKudos).val(getCacheItem(localStorageKeys.showKudos, localStorageDefaultValues.showKudos));
}

function loadShowKudos() {
    const showKudos = getCacheItem(localStorageKeys.showKudos, localStorageDefaultValues.showKudos);
    if (showKudos) {
        switch (showKudos) {
            case localStorageDefaultValues.showKudos:
                document.body.classList.remove(settingsClasses.showKudos);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.showKudos);
                break;
        }
    }
}

function eventChangeHideViews() {
    setCacheItem(localStorageKeys.hideViews, $(settingsControl.selectorHideViews).val());
    loadConfig();
}

function changeHideViews() {
    $(settingsControl.selectorHideViews).val(getCacheItem(localStorageKeys.hideViews, localStorageDefaultValues.hideViews));
}


function loadHideViews() {
    const hideViews = getCacheItem(localStorageKeys.hideViews, localStorageDefaultValues.hideViews);
    if (hideViews) {
        switch (hideViews) {
            case localStorageDefaultValues.hideViews:
                document.body.classList.remove(settingsClasses.hideViews);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideViews);
                break;
        }
    }
}


function eventChangeHideSpotify() {
    setCacheItem(localStorageKeys.hideSpotify, $(settingsControl.selectorHideSpotify).val());
    loadConfig();
}
function changeHideSpotify() {
    $(settingsControl.selectorHideSpotify).val(getCacheItem(localStorageKeys.hideSpotify, localStorageDefaultValues.hideSpotify));
}

function loadHideSpotify() {
    const hideSpotify = getCacheItem(localStorageKeys.hideSpotify, localStorageDefaultValues.hideSpotify);
    if (hideSpotify) {
        switch (hideSpotify) {
            case localStorageDefaultValues.hideSpotify:
                document.body.classList.remove(settingsClasses.hideSpotify);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideSpotify);
                break;
        }
    }
}

function eventChangeHideCommentPros() {
    setCacheItem(localStorageKeys.hideCommentPros, $(settingsControl.selectorHideCommentPros).val());
    loadConfig();
}
function changeHideCommentPros() {
    $(settingsControl.selectorHideCommentPros).val(getCacheItem(localStorageKeys.hideCommentPros, localStorageDefaultValues.hideCommentPros));
}

function loadHideCommentPros() {
    const hide = getCacheItem(localStorageKeys.hideCommentPros, localStorageDefaultValues.hideCommentPros);
    if (hide) {
        switch (hide) {
            case localStorageDefaultValues.hideCommentPros:
                document.body.classList.remove(settingsClasses.hideCommentPros);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideCommentPros);
                break;
        }
    }
}


function eventChangeHideCommentCons() {
    setCacheItem(localStorageKeys.hideCommentCons, $(settingsControl.selectorHideCommentCons).val());
    loadConfig();
}
function changeHideCommentCons() {
    $(settingsControl.selectorHideCommentCons).val(getCacheItem(localStorageKeys.hideCommentCons, localStorageDefaultValues.hideCommentCons));
}

function loadHideCommentCons() {
    const hide = getCacheItem(localStorageKeys.hideCommentCons, localStorageDefaultValues.hideCommentCons);
    if (hide) {
        switch (hide) {
            case localStorageDefaultValues.hideCommentCons:
                document.body.classList.remove(settingsClasses.hideCommentCons);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideCommentCons);
                break;
        }
    }
}

function eventChangeHideScoreMiguel() {
    setCacheItem(localStorageKeys.hideScoreMiguel, $(settingsControl.selectorHideScoreMiguel).val());
    loadConfig();
}
function changeHideScoreMiguel() {
    $(settingsControl.selectorHideScoreMiguel).val(getCacheItem(localStorageKeys.hideScoreMiguel, localStorageDefaultValues.hideScoreMiguel));
}

function loadHideScoreMiguel() {
    const hide = getCacheItem(localStorageKeys.hideScoreMiguel, localStorageDefaultValues.hideScoreMiguel);
    if (hide) {
        switch (hide) {
            case localStorageDefaultValues.hideScoreMiguel:
                document.body.classList.remove(settingsClasses.hideScoreMiguel);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideScoreMiguel);
                break;
        }
    }
}
function eventChangeHideScorePopular() {
    setCacheItem(localStorageKeys.hideScorePopular, $(settingsControl.selectorHideScorePopular).val());
    loadConfig();
}
function changeHideScorePopular() {
    $(settingsControl.selectorHideScorePopular).val(getCacheItem(localStorageKeys.hideScorePopular, localStorageDefaultValues.hideScorePopular));
}

function loadHideScorePopular() {
    const hide = getCacheItem(localStorageKeys.hideScorePopular, localStorageDefaultValues.hideScorePopular);
    if (hide) {
        switch (hide) {
            case localStorageDefaultValues.hideScorePopular:
                document.body.classList.remove(settingsClasses.hideScorePopular);
                break;
            case "hide":
                document.body.classList.add(settingsClasses.hideScorePopular);
                break;
        }
    }
}

function resetConfig() {
    setCacheItem(localStorageKeys.lightTheme, localStorageDefaultValues.lightTheme);
    $(settingsControl.lightTheme).val(localStorageDefaultValues.lightTheme);

    setCacheItem(localStorageKeys.fontSize, localStorageDefaultValues.fontSize);
    $(settingsControl.selectorFontSize).val(localStorageDefaultValues.fontSize);

    setCacheItem(localStorageKeys.fontFamily, localStorageDefaultValues.fontFamily);
    $(settingsControl.selectorFontFamily).val(localStorageDefaultValues.fontFamily);

    setCacheItem(localStorageKeys.fontWeight, localStorageDefaultValues.fontWeight);
    $(settingsControl.selectorFontWeight).val(localStorageDefaultValues.fontWeight);

    setCacheItem(localStorageKeys.showKudos, localStorageDefaultValues.showKudos);
    $(settingsControl.selectorShowKudos).val(localStorageDefaultValues.showKudos);

    setCacheItem(localStorageKeys.hideViews, localStorageDefaultValues.hideViews);
    $(settingsControl.selectorHideViews).val(localStorageDefaultValues.hideViews);

    setCacheItem(localStorageKeys.hideCommentPros, localStorageDefaultValues.hideCommentPros);
    $(settingsControl.selectorHideCommentPros).val(localStorageDefaultValues.hideCommentPros);

    setCacheItem(localStorageKeys.hideCommentCons, localStorageDefaultValues.hideCommentCons);
    $(settingsControl.selectorHideCommentCons).val(localStorageDefaultValues.hideCommentCons);

    setCacheItem(localStorageKeys.hideSpotify, localStorageDefaultValues.hideSpotify);
    $(settingsControl.selectorHideSpotify).val(localStorageDefaultValues.hideSpotify);

    setCacheItem(localStorageKeys.hideScoreMiguel, localStorageDefaultValues.hideScoreMiguel);
    $(settingsControl.selectorHideScoreMiguel).val(localStorageDefaultValues.hideScoreMiguel);

    setCacheItem(localStorageKeys.hideScorePopular, localStorageDefaultValues.hideScorePopular);
    $(settingsControl.selectorHideScorePopular).val(localStorageDefaultValues.hideScorePopular);

    loadConfig();
}