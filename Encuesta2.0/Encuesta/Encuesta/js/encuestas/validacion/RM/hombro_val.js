function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined) {
        if ($("input[name='p1']:checked").val() == "true") {
            if ($("#rpta_p1 input:checkbox:checked").length == 0) {
                $("#rpta_p1").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Seleccionar respuesta\n";
            } else {
                //otros
                if ($("#p1_7").is(":checked") && $("#p1_7_1").val().trim() == "") {
                    $("#rpta_p1").append("<p class='MessageValidation'>Debe ingresar el tipo de dolor</p>");
                    _IsValid = false;
                    _msj += "Pregunta 1: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    //// 2
    if ($("input[name='p2']:checked").val() != undefined) {
        if ($("input[name='p2']:checked").val() == "true") {
            if ($("input[name='p2_1']:checked").val() == undefined) {
                $("#p2_1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Seleccionar respuesta\n";
            }
            if (($("#p2_21").val() <= 0) || ($("#p2_22").val() == "")) {
                $("#p2_21").parent().parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Ingresar fecha\n";
            }
        }
    } else {
        $("#p2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }

    //// 3
    if (
    ($("input[name='p1']:checked").val() == "false") &&
     ($("input[name='p2']:checked").val() == "false")) {
        if ($("#p3").val().trim() == "") {
            $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Debe ingresar una molestia</p>");
            _IsValid = false;
            _msj += "Pregunta 3: Ingresar respuesta\n";
        }
    }

    //// 4
    if ((($("#p4_1").val() > 0) && ($("#p4_2").val() != "")) ||
         ($("input[name='p4_3']:checked").val() != undefined)) {

        if ($("input[name='p4_3']:checked").val() == "false" && $("#p4_3_1").val().trim() == "") {
            $("#p4_3_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
            _IsValid = false;
            _msj += "Pregunta 4: Especifique fecha\n";
        }

    } else {
        $("#p4_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Ingresar fecha\n";
    }


    // 5
    if ($("input[name='p5']:checked").val() != undefined) {
        if ($("input[name='p5']:checked").val() == "true" && $("#p5_1").val().trim() == "") {
            $("#p5_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 5: Ingresar detalle\n";
        }
    } else {
        $("#p5").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccione respuesta\n";
    }

    //6
    if ($("input[name='p6']:checked").val() != undefined) {
        if ($("input[name='p6']:checked").val() == "true" && $("#p6_1").val().trim() == "") {
            $("#p6_1").parent().parent().append("<p class='MessageValidation'>Debe ingresar los antecedentes</p>");
            _IsValid = false;
            _msj += "Pregunta 6: Ingresar antecedentes\n";
        }
    } else {
        $("#p6").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Seleccione respuesta\n";
    }

    ////PROCEDIMIENTOS RELACIONADOS

    //8
    if ($("input[name='p8']:checked").val() != undefined) {
        //SI
        if ($("input[name='p8']:checked").val() == "true") {
            //A 
            if ($("#p8_1").val().trim() == "") {
                $("#p8_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 8.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p8_21").val() <= 0 || ($("#p8_22").val() == "")) {
                $("#p8_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8.2: Seleccione respuesta\n";
            }
            //C
            if ($("input[name='p8_3']:checked").val() == undefined) {
                $("#p8_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el resultado</p>");
                _IsValid = false;
                _msj += "Pregunta 8.3: Seleccione respuesta\n";
            }
            //D
            if ($("#p8_4").val().trim() == "") {
                $("#p8_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 8.4: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione respuesta\n";
    }

    //9
    if ($("input[name='p9']:checked").val() != undefined) {
        //SI
        if ($("input[name='p9']:checked").val() == "true") {
            //A 
            if ($("#p9_1").val().trim() == "") {
                $("#p9_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 9.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p9_21").val() <= 0 || ($("#p9_22").val() == "")) {
                $("#p9_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 9.2: Seleccione respuesta\n";
            }
            //C
            if ($("input[name='p9_3']:checked").val() == undefined) {
                $("#p9_3").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el resultado</p>");
                _IsValid = false;
                _msj += "Pregunta 9.3: Seleccione respuesta\n";
            }

        }
    } else {
        $("#p9").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione respuesta\n";
    }


    // 10
    if ($("input[name='p10']:checked").val() != undefined) {
        //SI
        if ($("input[name='p10']:checked").val() == "true") {
            //A 
            if ($("#p10_1").val() != "") {
                //Si es Otro Centro...
                if ($("#p10_1").val() == "9") {
                    if ($("input[name='p10_11']:checked").val() == undefined) {
                        $("#p10_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 10.1: Seleccione respuesta\n";
                    }
                }
            } else {
                $("#p10_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                _IsValid = false;
                _msj += "Pregunta 10.1: Seleccione respuesta\n";
            }
            //B 
            if (($("#p10_21").val() <= 0) || ($("#p10_21").val() == "")) {
                $("#p10_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 10.2: Ingresar Fecha\n";
            }
            //C
            if ($("#p10_3").val().trim() == "") {
                $("#p10_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 10.3: Especifique\n";
            }
        }
    } else {
        $("#p10").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 10: Seleccione respuesta\n";
    }

    //11
    if ($("input[name='p11']:checked").val() != undefined) {
        if ($("input[name='p11']:checked").val() == "true") {
            if ($("#p11_1").val() != "") {
                //Si es Otro 
                if ($("#p11_1").val() == "6") {
                    if ($("#p11_11").val().trim() == "") {
                        $("#p11_11").parent().parent().parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 11.1: Ingrese respuesta\n";
                    }
                }
            } else {
                $("#p11_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el deporte</p>");
                _IsValid = false;
                _msj += "Pregunta 11.1: Seleccione respuesta\n";
            }

            if ($("#p11_2").val() == "") {
                $("#p11_2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la frecuencia</p>");
                _IsValid = false;
                _msj += "Pregunta 11.2: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 11: Seleccione respuesta\n";
    }

    // 12 A
    if (($("#p12Acheck").is(":checked") == false) && ($("#p12A").val().trim() == "")) {
        $("#p12A").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 12-A: Ingrese respuesta\n";
    }

    // 12 B
    if (($("#p12Bcheck").is(":checked") == false) && ($("#p12B").val().trim() == "")) {
        $("#p12B").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 12-B: Ingrese respuesta\n";
    }


    // 13
    if ($("#p13").val().trim() == "") {
        $("#p13").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 13: Ingrese respuesta\n";
    }

    //14
    if ($("input[name='p14']:checked").val() != undefined) {
        if ($("input[name='p14']:checked").val() == 2) {
            if ($("#p14_1").val() == "") {
                $("#p14_1").parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 14: Seleccione el motivo\n";
            }
        }
    } else {
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
