var SETTINGS = {
    FORM_DRAWING: "formDrawing",
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