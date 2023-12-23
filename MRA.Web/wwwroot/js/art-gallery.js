var FILTER_FORM_ID = "formFilter";
var ALERT_FILTER_FORM_ID = "formFilterAlert";

var LOADING_ICON_ART_GALLERY = "artGalleryLoader";
var DIV_ART_GALLERY = "artGallery";


function sendFormFilterGallery() {
    $("#" + FILTER_FORM_ID).submit();
}

function onBeginFilter() {
    $("#" + DIV_ART_GALLERY).hide();
    $("#" + LOADING_ICON_ART_GALLERY).show();
}

function onCompleteFilter() {
    $("#" + LOADING_ICON_ART_GALLERY).hide();
    $("#" + DIV_ART_GALLERY).show();
}

function onSuccessFilter(data) {
    $("#" + ALERT_FILTER_FORM_ID).hide();
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