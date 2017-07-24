
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

    //2
    if ($("input[name='p2']:checked").val() != undefined) {
        if ($("input[name='p2']:checked").val() == "true" && $("#p2_1").val().trim() == "") {
            $("#p2").parent().parent().parent().append("<p class='MessageValidation'>Debe especificar respuesta</p>");
            _IsValid = false;
            _msj += "Pregunta 2: Ingrese respuesta\n";
        }
    } else {
        $("#p2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }
    //PROCEDIMIENTOS RELACIONADOS
    // 3

    if (($("#p3_1").val() <= 0) && ($("#p3_2").val() == "")) {   
        $("#p3").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    }

    // 4
    if ($("input:radio[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() != "3" ) {
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

    // 7
    if ($("input:radio[name='p7']:checked").val() != undefined) {
        if ($("input[name='p7']:checked").val() == "true") {
            if (($("#p7_2").val() <= 0) && ($("#p7_1").val() == "")) {
                $("#p7_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 7: Seleccionar respuesta\n";
            }
            if ($("#p7_3").val().trim() == "") {
                $("#p7_3").parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 7: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p7").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 7: Seleccionar respuesta\n";
    }

    // 8
    if ($("input:radio[name='p8']:checked").val() != undefined) {
        if ($("input[name='p8']:checked").val() == "true") {
            if (($("#p8_1_1").val() <= 0) && ($("#p8_1").val() == "")) {
                $("#p8_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8: Seleccionar respuesta\n";
            }
            if (($("#p8_2_1").val() <= 0) && ($("#p8_2").val() == "")) {
                $("#p8_2").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8: Seleccionar respuesta\n";
            }
        }
    } else {
        $("#p8").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccionar respuesta\n";
    }

   

    // 9
    if ($("input:radio[name='p9']:checked").val() != undefined) {
        if ($("input[name='p9']:checked").val() == "true") {
            if (($("#p9_1").val() == "")) {
                $("#p9_1").parent().append("<p class='MessageValidation'> Seleccione donde</p>");
                _IsValid = false;
                _msj += "Pregunta 9: Ingresar respuesta\n";
            }
            if ($("input:radio[name='p9_3']:checked").val() == undefined) {
                $("#p9_3").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 9: Seleccionar respuesta\n";
            }
        }
    } else {
        $("#p9").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccionar respuesta\n";
    }


    // 10 A
    if (($("#p10_1").is(":checked") == false) && ($("#p10").val().trim() == "")) {
        $("#p10_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 10-A: Ingrese respuesta\n";
    }

    // 10 B
    if (($("#p11_1").is(":checked") == false) && ($("#p11").val().trim() == "")) {
        $("#p11_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 10-B: Ingrese respuesta\n";
    }


    // 11
    if ($("#p12").val().trim() == "") {
        $("#p12").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 11: Ingrese respuesta\n";
    }
    //13
    if ($("input[name='p14']:checked").val() == undefined) {
        $("#p14").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 14: Seleccione el contraste\n";
    }


    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}