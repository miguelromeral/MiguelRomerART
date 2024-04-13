var SETTINGS = {
    FORM_DRAWING: "formDrawing",
    AZURE_FORM: "rowUploadToAzure",
}


function sendForm() {
    $(".zero-if-null").each((index, element) => {
        if (element.value == '') {
            element.value = 0;
        }
    });

    $(".false-if-empty-check").each((index, element) => {
        if (element.checked) {
            element.value = true;
        } else {
            element.value = false;
        }
    });

    $("#" + SETTINGS.FORM_DRAWING).submit();
}



//var timeouts_success_filter = [];
function onSuccessSaveDrawing(data) {
    //$("#" + ALERT_FILTER_FORM_ID).hide();

    console.log(data);

    if (data != undefined && data != null) {


     
        //$("." + CLASE_NOTA_SECCION).each((index, element) => {
        //    var property = $(element).data(DATA_CLASE_NOTA_SECCION);
        //    if (property != undefined && property != null && property != "") {

        //        var nuevaNota = "";

        //        if ($(element).data("tipo") == "bool") {

        //            nuevaNota = data[toCamelCase(property)].toString();

        //            if (nuevaNota == "true") {
        //                $(element).text("Sí");
        //            } else {
        //                $(element).text("No");

        //            }

        //        } else {

        //            nuevaNota = truncateDecimal(data[toCamelCase(property)].toString(), NUMERO_DECIMALES);
        //            $(element).text(nuevaNota);

        //        }

        //        $(element).removeClass("anim-glow-after");

        //        var anteriorNota = $(element).data("nota-previa");
        //        if (anteriorNota != nuevaNota) {
        //            $(element).addClass("anim-glow-after");
        //            //var to =
        //            setTimeout(() => {
        //                $(element).removeClass("anim-glow-after");
        //            }, TIME_ANIMATION_GLOW_EFFECT);
        //            //timeouts_success_filter.push(to);
        //        }
        //        $(element).data("nota-previa", nuevaNota);

        //    }
        //});

        //$("#" + ALERT_FILTER_SUCCESS_FORM_ID).addClass("show-alert");
        //$("#" + ALERT_FILTER_SUCCESS_FORM_ID).text("El pronóstico se ha guardado con éxito");

        //setTimeout(function () {
        //    $("#" + ALERT_FILTER_SUCCESS_FORM_ID).removeClass("show-alert");
        //}, TIME_ANIMATION_ALERT_SUCCESS);

        //$.alert({
        //    closeIcon: true,
        //    title: "Dibujo Guardado",
        //    content: "¡Tu dibujo se ha guardado correctamente!",
        //});
        alert("¡Tu dibujo se ha guardado correctamente!");

    } else {
        mostrarMensajeError("Ha ocurrido un fallo al guardar el dibujo. Por favor, inténtalo más tarde");
    }
}

function onFailureSaveDrawing() {
    mostrarMensajeError("Ha ocurrido un fallo al guardar el dibujo. Por favor, inténtalo más tarde");
}

function mostrarMensajeError(mensaje = "", titulo = "Error") {

    alert(mensaje);

    //$.alert({
    //    closeIcon: true,
    //    title: titulo,
    //    content: mensaje,
    //});
    //$("#" + ALERT_FILTER_FORM_ID).text(mensaje);
    //$("#" + ALERT_FILTER_FORM_ID).show();
}


function onChangeAzurePath(input) {
    var newPath = input.value;

    $.ajax({
        type: 'POST',
        url: '/Admin/CheckAzurePath',
        data: { id: newPath },
        success: function (response) {
            console.log("Response:" + response);
            if (response === true) {
                $("#" + SETTINGS.AZURE_FORM).hide();
            } else {
                $("#" + SETTINGS.AZURE_FORM).show();
                console.log("Esa ruta no existe!");
            }
        },
        error: function (xhr, status, error) {
            mostrarMensajeError(error);
        }
    });
}

function uploadAzureImage() {
    $("#btnUploadToAzure").prop("disabled", true);
    var fileInput = document.getElementById('azureImage');
    var thumbnailSizeInput = document.getElementById('azureImageThumbnailSize');
    var pathInput = document.getElementById('azurePath');

    // Verificar si se seleccionó un archivo
    if (fileInput.files.length === 0) {
        alert('Por favor, selecciona un archivo.');
        return;
    }

    var file = fileInput.files[0];
    var thumbnailSize = thumbnailSizeInput.value;
    var path = pathInput.value;

    // Crear objeto FormData
    var formData = new FormData();
    formData.append('path', path);
    formData.append('azureImage', file);
    formData.append('azureImageThumbnailSize', thumbnailSize);
    $.ajax({
        type: 'POST',
        url: '/Admin/UploadAzureImage',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            $("#btnUploadToAzure").prop("disabled", false);
            $("#" + SETTINGS.AZURE_FORM).hide();
            $("#imgPreviewFullSize").attr("src", response.url);
            $("#imgPreviewThumbnail").attr("src", response.url_tn);
            $("#iHiddenUrlThumbnail").val(response.tn);
            alert('Imagen subida exitosamente');
        },
        error: function (xhr, status, error) {
            $("#btnUploadToAzure").prop("disabled", false);
            mostrarMensajeError(error);
        }
    });
}



function mostrarImagenAzureTmp() {
    var input = document.getElementById('azureImage');
    var imgTmp = document.getElementById('imgTmp');

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            imgTmp.src = e.target.result;
            imgTmp.style.display = 'block'; // Mostrar la imagen
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function actualizarImagenReferencia(input) {
    
    $("#imgReferencia").attr("src",input.value);
}

function changeListComments(hiddenInputId, clase) {
    var inputs = $("." + clase);
    var comments = [];
    inputs.each((index, input) => {
        comments.push($(input).val());
    });
    $("#" + hiddenInputId).val(comments.join("#"));
    console.log(comments);
}


function addNewComment(idDivList, hiddenInputId, commentClass, text) {
    if (text == undefined || text == null) {
        text = "";
    }
    var container = document.getElementById(idDivList);

    var div = document.createElement("div");
    div.classList.add("mr-comment-container");

    var newInput = document.createElement("textarea");
    //newInput.type = "text";
    newInput.value = text ?? "";
    newInput.placeholder = "Comentario";
    newInput.rows = "5";
    newInput.classList.add("form-control");
    newInput.classList.add("mr-custom-text");
    newInput.classList.add(commentClass);
    newInput.addEventListener("change", () => { changeListComments(hiddenInputId, commentClass); });

    var btn = document.createElement("button");
    btn.classList.add("btn");
    btn.classList.add("btn-primary");
    btn.classList.add("mr-custom-btn");
    btn.type = "button";
    btn.addEventListener("click", () => { removeComment(btn, idDivList); changeListComments(hiddenInputId, commentClass); });

    var i = document.createElement("i");
    i.classList.add("bi");
    i.classList.add("bi-trash");
    btn.appendChild(i);

    div.appendChild(newInput);
    div.appendChild(btn);

    container.appendChild(div);
}

function removeComment(input, idDivList) {
    var parent = input.parentElement;
    var container = document.getElementById(idDivList);
    container.removeChild(parent);
}
