
function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined) {
        if ($("input[name='p1']:checked").val() == "true") {
            if (($("#p1_1").val() <= 0) && ($("#p1_2").val() == "")) {
                $("#p1_2").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Ingresar fecha\n";
            }
            if ($("#p1_3").val().trim() == "") {
                $("#p1_3").parent().append("<p class='MessageValidation'>Ingrese el diagnostco</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    //PROCEDIMIENTOS RELACIONADOS
    // 3
    if ($("#rpta_p3 input:checkbox:checked").length == 0) {
        $("#rpta_p3").append("<p class='MessageValidation'>Seleccione los motivos</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Ingrese respuesta\n";
    }

    // 4
    if ($("input:radio[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() == "true") {
            if ($("#p4_1").val().trim() == "") {
                $("#p4_1").parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 4: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p4").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccionar respuesta\n";
    }

    // 5
    if ($("input:radio[name='p5']:checked").val() != undefined) {
        if ($("input[name='p5']:checked").val() == "true") {
            if ($("#p5_1").val().trim() == "") {
                $("#p5_1").parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 5: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p5").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccionar respuesta\n";
    }

    // 6    
    if ($("#p6").val() == "") {
        $("#p6").parent().parent().append("<p class='MessageValidation'>Ingrese la respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Ingrese respuesta\n";
    }

    // 7
    if ($("#p7").val() == "") {
        $("#p7").parent().parent().append("<p class='MessageValidation'>Ingrese la respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 7: Ingrese respuesta\n";
    }



    //8
    if ($("input[name='p8']:checked").val() == undefined) {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione el contraste\n";
    }



    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}