// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function sendFormFilterGallery() {
    $("#formFilter").submit();
}

function onBeginFilter() {
}

function onCompleteFilter() {
}

function onSuccessFilter(data) {
    $("#formFilterAlert").hide();
    $(".mr-img-thumbnail").each((index, img) => {
        if (data.find(d => d.id == $(img).data("drawingid")) != null) {
            $(img).show();
        } else {
            $(img).hide();
        }
    });
}

function onFailureFilter() {
    $("#formFilterAlert").text("An error happened while filtering data. Please, try again later.");
    $("#formFilterAlert").show();
}