var FILTER_FORM_ID = "formFilter";
var ALERT_FILTER_FORM_ID = "formFilterAlert";

var LOADING_ICON_ART_GALLERY = "artGalleryLoader";
var DIV_ART_GALLERY = "artGallery";

var CHEER_FORM_ID = "cheerForm";

var timeMsDelayLike = 1000;

var filtersControls = {
    tags: "#tbTags",
    type: "#sFilterType",
    product: "#sFilterProduct",
    productName: "#sFilterProductName",
    collection: "#sFilterCollection",
    characterName: "#sFilterCharacterName",
    model: "#sFilterModel",
    software: "#sFilterSoftware",
    paper: "#sFilterPaper",
    favorite: "#flexSwitchCheckChecked",
    sortby: "#sFilterSortBy",
}

function sendFormFilterGallery() {
    changeBasicArtUrl();

    $("#" + FILTER_FORM_ID).submit();
}

function changeBasicArtUrl() {

    changeArtUrl(
        $(filtersControls.tags).val(),
        $(filtersControls.type).val(),
        $(filtersControls.product).val(),
        $(filtersControls.productName).val(),
        $(filtersControls.collection).val(),
        $(filtersControls.characterName).val(),
        $(filtersControls.model).val(),
        $(filtersControls.software).val(),
        $(filtersControls.paper).val(),
        $(filtersControls.favorite).prop("checked"),
        $(filtersControls.sortby).val(),
        false);
}

function changeArtUrl(textQuery, type, productType, productName, collection, characterName, modelName, software, paper, favorites, sortby, submit) {
    // Obtén la URL base
    var baseUrl = window.location.protocol + "//" + window.location.host + window.location.pathname;
    
    var queryParams = [];

    queryParams.push(setFilterValue(filtersControls.tags, "TextQuery", textQuery));
    queryParams.push(setFilterValue(filtersControls.type, "Type", type, [-1]));
    queryParams.push(setFilterValue(filtersControls.product, "ProductType", productType, [-1]));
    queryParams.push(setFilterValue(filtersControls.productName, "ProductName", productName));
    queryParams.push(setFilterValue(filtersControls.collection, "Collection", collection));
    queryParams.push(setFilterValue(filtersControls.characterName, "CharacterName", characterName));
    queryParams.push(setFilterValue(filtersControls.model, "ModelName", modelName));
    queryParams.push(setFilterValue(filtersControls.software, "Software", software, [0]));
    queryParams.push(setFilterValue(filtersControls.paper, "Paper", paper, [0]));
    queryParams.push(setFilterValue(filtersControls.favorite, "Favorites", favorites, [false], true));
    queryParams.push(setFilterValue(filtersControls.sortby, "Sortby", sortby, ["date-desc"]));


    queryParams = queryParams.filter(x => x != null);

    var params = "";

    if (queryParams.length > 0) {
        params = "?" + queryParams.join("&");
    }

    var newUrl = baseUrl + params;
    // Cambia la URL sin recargar la página
    history.pushState(null, null, newUrl);

    // Puedes imprimir la nueva URL en la consola para verificar
    //console.log("Nueva URL:", window.location.href);

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
    $(filtersControls.tags).val("");
    $(filtersControls.type).val(-1);
    $(filtersControls.product).val(-1);
    $(filtersControls.productName).val("");
    $(filtersControls.collection).val("");
    $(filtersControls.characterName).val("");
    $(filtersControls.model).val("");
    $(filtersControls.software).val(0);
    $(filtersControls.paper).val(0);
    $(filtersControls.favorite).prop("checked", false);
    changeBasicArtUrl();
}

function onBeginFilter() {
    $("#artGallery").addClass("loading");
    $("#" + LOADING_ICON_ART_GALLERY).addClass("loading");
}

function onCompleteFilter() {
    $("#" + LOADING_ICON_ART_GALLERY).removeClass("loading");
    $("#artGallery").removeClass("loading");
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
    var componenteRoja = Math.floor(Math.random() * 192 + 64);
    var componenteVerde = 0;
    var componenteAzul = 0;
    return 'rgb(' + componenteRoja + ',' + componenteVerde + ',' + componenteAzul + ')';
}

function generateHeartSize() {
    var pixels = Math.floor(Math.random() * 20 + 10);
    return pixels + "px";
}

function capturarEnter(event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendFormFilterGallery();
    }
}

function onChangeRangeVote(event) {
    var score = event.target.value;
    $("#spScoreUser").text(score);
    
    $("#spScoreUser").removeClass("bad");
    $("#spScoreUser").removeClass("mild");
    $("#spScoreUser").removeClass("good");
    $("#spScoreUser").removeClass("platinum");

    if (score < 50) {
        $("#spScoreUser").addClass("bad");
    }
    else if (score < 65) {
        $("#spScoreUser").addClass("mild");
    }
    else if (score < 95) {
        $("#spScoreUser").addClass("good");
    }
    else if (score < 101) {
        $("#spScoreUser").addClass("platinum");
    }
}


function voteDrawing(event) {
    $("#voteForm").submit();
    $("#btnVote").attr("disabled", true);
}
function onBeginVote() {

}
function OnFailureVote() {

}
function onSuccessVote(data) {
    var newVotes = data?.newVotes;
    console.log(data);
    if (newVotes != undefined && newVotes != null && newVotes > 0) {
        var score = data.newScore;
        $("#spScoreUserVotes").text(data.newScoreHuman);
        $("#spNumberUserVotes").text(newVotes);
        $("#divScorePopular").addClass("scale-up-center");

        $("#spScoreUserVotes").removeClass("mr-score-bad");
        $("#spScoreUserVotes").removeClass("mr-score-mild");
        $("#spScoreUserVotes").removeClass("mr-score-good");
        $("#spScoreUserVotes").removeClass("mr-score-platinum");
        
        if (score < 50) {
            $("#spScoreUserVotes").addClass("mr-score-bad");
        }
        else if (score < 65) {
            $("#spScoreUserVotes").addClass("mr-score-mild");
        }
        else if (score < 95) {
            $("#spScoreUserVotes").addClass("mr-score-good");
        }
        else if (score < 101) {
            $("#spScoreUserVotes").addClass("mr-score-platinum");
        }

        document.getElementById("containerCollapseVoteForm").remove();
        document.getElementById("btnOpenVoteForm").remove();
    } else {
        alert("Error al enviar el voto. Por favor, vuelva a intentarlo más tarde");
    }
}
function onCompleteVote() {
    //setTimeout(function () {
    //    $("#btnCheer").attr("disabled", false);
    //    console.log("Button enabled!");
    //}, timeMsDelayLike);
}
