var _$isCorregido = false;
var _$formulario = "";
$(function () {
    $(".validar").change(function () {
        if (_$isCorregido) {
            validar_form(_$formulario, false)
        };
    });
    solonumeros();
    solotexto();
   
});
function solonumeros() {
    $(".solo-numeros").keydown(function (event) {
        if (event.shiftKey) {
            event.preventDefault();
        }

        if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9) {
        }
        else {
            if (event.keyCode < 95) {
                if (event.keyCode < 48 || event.keyCode > 57) {
                    event.preventDefault();
                }
            }
            else {
                if (event.keyCode < 96 || event.keyCode > 105) {
                    event.preventDefault();
                }
            }
        }
    });
}

function solotexto() {
    $(".solo-letras").keydown(function (event) {
        if (event.keyCode == 220 || event.keyCode == 219 || event.keyCode == 221 || event.keyCode == 187 || event.keyCode == 186 || event.keyCode == 191 || event.keyCode == 222 || event.keyCode == 189 || event.keyCode == 190 || event.keyCode == 188) {
            event.preventDefault();
        }

        if (event.keyCode < 95) {
            if (!(event.keyCode < 48 || event.keyCode > 57)) {
                event.preventDefault();
            }

        }
        else {
            if (!(event.keyCode < 96 || event.keyCode > 111)) {
                event.preventDefault();
            }
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                event.preventDefault();
        }


    });
}

function validar_form(formulario, showMensaje) {
    _$isCorregido = true;
    var result = true;
    if (showMensaje == undefined)
        showMensaje = true;
    _$formulario = formulario;
    $("#" + formulario + " input.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.find(".form-control-feedback").remove();
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
            div_cont.addClass("has-feedback");
            div_cont.append("<i class='fa fa-times form-control-feedback'></i>")
        }
    });
    
    $("#" + formulario + " select.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.find(".form-control-feedback").remove();
        if ($(this).val() == "") {
            div_cont.addClass("has-error");
            div_cont.addClass("has-feedback");
            div_cont.append("<i class='fa fa-times form-control-feedback'></i>")
        }
    });
    $("#" + formulario + " textarea.validar ").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.find(".form-control-feedback").remove();
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
            div_cont.addClass("has-feedback");
            div_cont.append("<i class='fa fa-times form-control-feedback'></i>")
        }
    });

    $(".has-error").each(function () {
        result = false;
    });

    if (!result && showMensaje)
        toastr.error("Verifique los datos", "Error", opts);

    return result;
}

function validarRUC(campo) {
    var resultado = true;
    var valor = $("#" + campo).val();
    removeHasAll(campo);
    if (valor.length>0&&valor.length < 11) {
        resultado = false;
        addError(campo);
    }
    return resultado;
}


function addError(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.addClass("has-error");
    div_cont.addClass("has-feedback");
    div_cont.append("<i class='fa fa-times form-control-feedback'></i>")
}
function removeError(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.removeClass("has-error");
    div_cont.find(".form-control-feedback").remove();
}
function addWarning(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.addClass("has-warning");
    div_cont.addClass("has-feedback");
    div_cont.append("<i class='fa fa-exclamation-triangle form-control-feedback'></i>")
}
function removeWarning(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.removeClass("has-warning");
    div_cont.find(".form-control-feedback").remove();
}
function addSuccess(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.addClass("has-success");
    div_cont.addClass("has-feedback");
    div_cont.append("<i class='fa fa-check form-control-feedback'></i>")
}
function removeSuccess(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.removeClass("has-success");
    div_cont.find(".form-control-feedback").remove();
}
function removeHasAll(input) {
    var div_cont = $("#" + input).parents(".form-group").first();
    div_cont.removeClass("has-success");
    div_cont.removeClass("has-warning");
    div_cont.removeClass("has-error");
    div_cont.find(".form-control-feedback").remove();
}

