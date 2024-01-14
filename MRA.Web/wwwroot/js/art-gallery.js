var FILTER_FORM_ID = "formFilter";
var ALERT_FILTER_FORM_ID = "formFilterAlert";

var LOADING_ICON_ART_GALLERY = "artGalleryLoader";
var DIV_ART_GALLERY = "artGallery";

var CHEER_FORM_ID = "cheerForm";

var timeMsDelayLike = 1000;

function sendFormFilterGallery() {

    changeArtUrl(
        $("#tbTags").val(),
        $("#sFilterType").val(),
        $("#sFilterProduct").val(),
        $("#sFilterProductName").val(),
        $("#sFilterCollection").val(),
        $("#sFilterCharacterName").val(),
        $("#sFilterModel").val(),
        $("#sFilterSoftware").val(),
        $("#sFilterPaper").val(),
        $("#flexSwitchCheckChecked").prop("checked"),
        false);

    $("#" + FILTER_FORM_ID).submit();
}


function changeArtUrl(textQuery, type, productType, productName, collection, characterName, modelName, software, paper, favorites, submit) {
    // Obtén la URL base
    var baseUrl = window.location.protocol + "//" + window.location.host + window.location.pathname;
    
    var queryParams = [];

    queryParams.push(setFilterValue("#tbTags", "TextQuery", textQuery));
    queryParams.push(setFilterValue("#sFilterType", "Type", type, [-1]));
    queryParams.push(setFilterValue("#sFilterProduct", "ProductType", productType, [-1]));
    queryParams.push(setFilterValue("#sFilterProductName", "ProductName", productName));
    queryParams.push(setFilterValue("#sFilterCollection", "Collection", collection));
    queryParams.push(setFilterValue("#sFilterCharacterName", "CharacterName", characterName));
    queryParams.push(setFilterValue("#sFilterModel", "ModelName", modelName));
    queryParams.push(setFilterValue("#sFilterSoftware", "Software", software, [0]));
    queryParams.push(setFilterValue("#sFilterPaper", "Paper", paper, [0]));
    queryParams.push(setFilterValue("#flexSwitchCheckChecked", "Favorites", favorites, [false], true));


    queryParams = queryParams.filter(x => x != null);

    var params = "";

    if (queryParams.length > 0) {
        params = "?" + queryParams.join("&");
    }

    var newUrl = baseUrl + params;
    // Cambia la URL sin recargar la página
    history.pushState(null, null, newUrl);

    // Puedes imprimir la nueva URL en la consola para verificar
    console.log("Nueva URL:", window.location.href);

    if (submit) {
        sendFormFilterGallery();
    }
}

function setFilterValue(querySelector, name, value, ommitValues, isSwitch) {
    if (ommitValues == undefined || ommitValues == null) {
        ommitValues = [];
    }
    if (value != undefined && value != null && value != "" && ommitValues.filter(x => x == value).length == 0) {
        if (isSwitch != undefined && isSwitch != null && isSwitch) {
            $(querySelector).prop("checked" , value);
        } else {
            $(querySelector).val(value);
        }
        return name + "=" + value;
    }
    return null;
}

function resetFilters() {
    $("#sFilterType").val(-1);
    $("#sFilterProduct").val(-1);
    $("#sFilterCharacterName").val("");
    $("#sFilterProductName").val("");
    $("#tbTags").val("");
    $("#sFilterModel").val("");
    $("#sFilterSoftware").val(0);
    $("#sFilterCollection").val("");
    $("#sFilterPaper").val(0);
    $("#flexSwitchCheckChecked").prop("checked", false);
    sendFormFilterGallery();
}

function onBeginFilter() {
    //document.getElementById(DIV_ART_GALLERY).style.visibility = 'hidden';
    $("#" + LOADING_ICON_ART_GALLERY).show();
}

function onCompleteFilter() {
    $("#" + LOADING_ICON_ART_GALLERY).hide();
    //document.getElementById(DIV_ART_GALLERY).style.visibility = 'visible';
}

function onSuccessFilter(data) {
    $("#" + ALERT_FILTER_FORM_ID).hide();
    $("#spResultsCount").text(data.length);
    if (data.length == 1) {
        $("#spTextoImagen").show();
        $("#spTextoImagenes").hide();
    } else {
        $("#spTextoImagen").hide();
        $("#spTextoImagenes").show();
    }

    sortDivCollection(data);
    $(".mr-img-thumbnail").each((index, img) => {
        if (data.find(d => d.id == $(img).data("drawingid")) != null) {
            $(img).show();
        } else {
            $(img).hide();
        }
    });

    if (data.length > 0) {
        $("#divNoResults").hide();
    } else {
        $("#divNoResults").show();
    }
}

function onFailureFilter() {
    $("#spResultsCount").text(0);
    $("#" + ALERT_FILTER_FORM_ID).text("An error happened while filtering data. Please, try again later.");
    $("#" + ALERT_FILTER_FORM_ID).show();
}


function sortDivCollection(orderedCollection) {
    // Supongamos que tienes una lista de divs desordenados
    const unorderedDivs = document.querySelectorAll(".mr-img-thumbnail");

    // Convierte la lista de divs en un array para poder ordenarlos
    const divsArray = Array.from(unorderedDivs);

    // Ordena el array de divs basándote en el orden de la colección
    divsArray.sort((a, b) => {
        const idA = a.getAttribute('data-drawingId');
        const idB = b.getAttribute('data-drawingId');

        const indexA = orderedCollection.findIndex(item => item.id === idA);
        const indexB = orderedCollection.findIndex(item => item.id === idB);

        return indexA - indexB;
    });

    // Ahora, los divs en divsArray están ordenados de la misma manera que la colección
    // Puedes agregar estos divs ordenados de nuevo al DOM
    const container = document.querySelector('#artGallery');
    divsArray.forEach(div => container.appendChild(div));

}

var mensajesAgradecimiento = [
    "Muchas gracias por el like 😉",
    "Tú sí que eres una obra de arte 😁",
    "Eres muy amable, ¡gracias! 😘",
    "Significa mucho para mí 🥺",
    "Todos estos corazones son tuyos 💖",
    "¡Gracias, gracias, gracias! 🙏",
    "Me alegro de que te guste 😊",
    "¿Le diste sin querer? aú así, ¡gracias! 😂",
    "Haré saber al modelo que le gustas 🙊",
    "Eres muy grande 🙂",
    "¡Gracias, un abrazo! 🤗",
    "¡Gracias por tocar 2 veces la imagen! ✌",
    "Gracias, generoso 😏",
    "Gracias, eres un sol ☀",
    "Se te cayó esto, mi rey 👑",
    "Se te cayó esto, mi reina 👑",
    "Mereció la pena dibujarlo por esto 🥰",
    "¿Tienes curiosidad por ver todas las frases? 🙃",
    "Eres mu' salao' 🧂",
    "¡Gracias! 😁",
    "Thank you! 😎",
    "Si apagas la pantalla ahora, verás algo más bello aún 😜",
    "¡Gracias! No olvides ver el resto de la galería 🖼",
];
function obtenerMensajeAleatorio() {
    var indiceAleatorio = Math.floor(Math.random() * mensajesAgradecimiento.length);
    return mensajesAgradecimiento[indiceAleatorio];
}

function cheerDrawing(event) {
    $("#" + CHEER_FORM_ID).submit();
    $("#btnCheer").attr("disabled", true);
    $("#iCheerIcon").addClass("bi-heart-fill");
    $("#iCheerIcon").removeClass("bi-heart");
    
    var element = document.createElement("div");
    element.innerHTML = obtenerMensajeAleatorio();


    element.style.left = (event.pageX - 20)+ 'px';
    element.style.top = (event.pageY) + 'px';
    element.style.position = 'absolute';
    element.classList.add("mr-thanks-message");
    element.classList.add("dissapear-message-thanks");
    document.body.appendChild(element);


    var interval = setTimeout(() => {
        element.remove();
    }, 3000);

    showHearts();
}

function onBeginCheer() {

}
function OnFailureCheer() {

}
function onSuccessCheer() {
    var childId = "spLikesCounter";
    var numberElement = document.getElementById(childId);
    var newNumber = parseInt(numberElement.textContent) + 1;

    // Ocultar el número actual
    document.getElementById("divContainerLikes").classList.remove('bounce-bottom');

    // Configurar el nuevo número después de un breve tiempo
    setTimeout(function () {
        numberElement.textContent = newNumber;

        // Mostrar el nuevo número
        document.getElementById("divContainerLikes").classList.add('bounce-bottom');
    }, 500);
}
function onCompleteCheer() {
    setTimeout(function () {
        $("#btnCheer").attr("disabled", false);
        console.log("Button enabled!");
    }, timeMsDelayLike);
}




function createHeart() {
    const body = document.querySelector(".heart-rain");
    const heart = document.createElement("div");
    heart.className = "fas fa-heart";
    heart.style.left = (Math.random() * 100) + "vw";
    heart.style.color = generateRedTone();
    heart.style.animationDuration = (Math.random() * 3) + 2 + "s"
    heart.style.fontSize = generateHeartSize();
    body.appendChild(heart);
}


function showHearts() {
    var i1 = setInterval(createHeart, 0);
    var i2 = setInterval(function name(params) {
        var heartArr = document.querySelectorAll(".fa-heart")
        if (heartArr.length > 200) {
            heartArr[0].remove()
        }
    }, 100)

    setTimeout(() => {
        clearInterval(i1);
        clearInterval(i2);
    }, timeMsDelayLike);

}   

function generateRedTone() {
    // Genera un valor aleatorio para la componente roja (R)
    var componenteRoja = Math.floor(Math.random() * 192 + 64);

    // Establece las componentes verde (G) y azul (B) en cero
    var componenteVerde = 0;
    var componenteAzul = 0;

    return 'rgb(' + componenteRoja + ',' + componenteVerde + ',' + componenteAzul + ')';
}

function generateHeartSize() {
    // Genera un valor aleatorio para la componente roja (R)
    var pixels = Math.floor(Math.random() * 20 + 10);
    
    return pixels + "px";
}

function capturarEnter(event) {
    if (event.key === "Enter") {
        event.preventDefault();
        //console.log("Se presionó la tecla Enter (evento prevenido)");
        sendFormFilterGallery();
    }
}