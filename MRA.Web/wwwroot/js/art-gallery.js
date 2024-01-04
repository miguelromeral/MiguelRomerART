var FILTER_FORM_ID = "formFilter";
var ALERT_FILTER_FORM_ID = "formFilterAlert";

var LOADING_ICON_ART_GALLERY = "artGalleryLoader";
var DIV_ART_GALLERY = "artGallery";

var CHEER_FORM_ID = "cheerForm";

var timeMsDelayLike = 1000;
function sendFormFilterGallery() {
    var type = $("#sFilterType").val();

    // If Digital, show filter
    if (type == 0) {
        $("#sFilterSoftware").show();
        $("#sFilterPaper").show();
    } else if (type == 2) {
        $("#sFilterPaper").hide();
        $("#sFilterPaper").val(0);

        $("#sFilterSoftware").show();
    } else {
        $("#sFilterSoftware").hide();
        $("#sFilterSoftware").val(0);

        $("#sFilterPaper").show();
    }

    $("#" + FILTER_FORM_ID).submit();
}

function resetFilters() {
    $("#sFilterType").val(-1);
    $("#sFilterProduct").val(-1);
    $("#sFilterProductName").val("");
    $("#sFilterModel").val("");
    $("#sFilterSoftware").val(0);
    $("#sFilterPaper").val(0);
    $("#flexSwitchCheckChecked").attr("checked", false);
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


function cheerDrawing() {
    console.log("Cheering drawing!");
    $("#" + CHEER_FORM_ID).submit();
    $("#btnCheer").attr("disabled", true);
    $("#iCheerIcon").addClass("bi-heart-fill");
    $("#iCheerIcon").removeClass("bi-heart");
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