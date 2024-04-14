// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




function errorLoadingImage(drawingId) {
    console.log("Drawing ID not loaded: " + drawingId);
    var selector = ".mr-error-load-placeholder[data-error-drawing-id='" + drawingId + "']";
    //console.log(selector);
    $(selector).addClass("show");
}


function updateCheckValue(checkbox, hiddenId) {
    var hiddenInput = document.getElementById(hiddenId);
    if (checkbox.checked) {
        hiddenInput.value = true;
    } else {
        hiddenInput.value = false;
    }
}


function onChangeInputDate(input, id) {
    var newValue = $(input).val();
    $("#" + id).val(newValue.toString().replace("-", "/").replace("-", "/"));   
}