var FILTER_FORM_ID = "formFilter";
var ALERT_FILTER_FORM_ID = "formFilterAlert";

var LOADING_ICON_ART_GALLERY = "artGalleryLoader";
var DIV_ART_GALLERY = "artGallery";


function sendFormFilterGallery() {
    $("#" + FILTER_FORM_ID).submit();
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
    sortDivCollection(data);
    $(".mr-img-thumbnail").each((index, img) => {
        if (data.find(d => d.id == $(img).data("drawingid")) != null) {
            $(img).show();
        } else {
            $(img).hide();
        }
    });
}

function onFailureFilter() {
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