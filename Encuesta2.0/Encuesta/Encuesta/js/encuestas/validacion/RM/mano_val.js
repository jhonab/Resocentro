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
                    $("#rpta_p1").append("<p class='MessageValidation'>Debe especificar ubicación</p>");
                    _IsValid = false;
                    _msj += "Pregunta 1: Ingresar respuesta\n";
                }
            }
        }

    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }
    //P1-1
    //Limitacion Funcional
    if ($("#p1_1").is(":checked")) {
        if ($("#rpta_1_1 input:checkbox:checked").length == 0) {
            $("#rpta_1_1").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
            _IsValid = false;
            _msj += "Pregunta 1: Seleccionar respuesta\n";
        } else {
            //otros
            if ($("#p1_16").is(":checked") && $("#p1_16_1").val().trim() == "") {
                $("#rpta_1_1").append("<p class='MessageValidation'>Debe especificar</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Ingresar respuesta\n";
            }
        }
    }
    //otros
    if ($("#p1_2").is(":checked") && $("#p1_21").val().trim() == "") {
        $("#rpta_p1_1").append("<p class='MessageValidation'>Debe ingresar el tipo de dolor</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Ingresar respuesta\n";

    }

    // 2
    if ($("input[name='p2']:checked").val() == undefined) {
        $("#p2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }

    // 3
    if ($("input[name='p3']:checked").val() != undefined) {
        if ($("input[name='p3']:checked").val() == "true") {
            if ($("#p3_1").val().trim() == "") {
                $("#p3_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
                _IsValid = false;
                _msj += "Pregunta 3: Ingresar detalle\n";
            }
        }
    } else {
        $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccione respuesta\n";
    }

    ////PROCEDIMIENTOS RELACIONADOS

    //// 4
    if ((($("#p4_1").val() > 0) && ($("#p4_2").val() != "")) ||
         ($("input[name='p4_3']:checked").val() != undefined)) {

        if ($("input[name='p4_3']:checked").val() == "false" && $("#p4_3_1").val().trim() == "") {
            $("#p2_3_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
            _IsValid = false;
            _msj += "Pregunta 4: Especifique fecha\n";
        }

    } else {
        $("#p4_1").parent().parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Ingresar fecha\n";
    }


    // 5
    if ($("input[name='p5']:checked").val() != undefined) {
        if ($("input[name='p5']:checked").val() != "3" && $("#p5_1").val().trim() == "") {
            $("#p5_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 5: Ingresar detalle\n";
        }
    } else {
        $("#p5").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccione respuesta\n";
    }

    // 6
    if ($("input[name='p6']:checked").val() != undefined) {
        if ($("input[name='p6']:checked").val() == "1" && $("#p6_1").val().trim() == "") {
            $("#p6_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 6: Ingresar detalle\n";
        }
    } else {
        $("#p6").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Seleccione respuesta\n";
    }




    // 8
    if ($("input[name='p10']:checked").val() != undefined) {
        //SI
        if ($("input[name='p10']:checked").val() == "true") {
            //A 
            if ($("#p10_1").val().trim() == "") {
                $("#p10_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 8.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p10_21").val() <= 0 || ($("#p10_22").val() == "")) {
                $("#p10_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8.2: Seleccione respuesta\n";
            }
            //C
            if ($("input[name='p10_3']:checked").val() == undefined) {
                $("#p10_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el resultado</p>");
                _IsValid = false;
                _msj += "Pregunta 8.3: Seleccione respuesta\n";
            }
            //D
            if ($("#p10_4").val().trim() == "") {
                $("#p10_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 8.4: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p10").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione respuesta\n";
    }

    // 9
    if ($("input[name='p11']:checked").val() != undefined) {
        //SI
        if ($("input[name='p11']:checked").val() == "true") {
            //A 
            if ($("#p11_1").val().trim() == "") {
                $("#p11_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 9.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p11_21").val() <= 0 || ($("#p11_22").val() == "")) {
                $("#p11_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 9.2: Seleccione respuesta\n";
            }

        }
    } else {
        $("#p11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione respuesta\n";
    }


    // 10
    if ($("input[name='p12']:checked").val() != undefined) {
        //SI
        if ($("input[name='p12']:checked").val() == "true") {
            //A 
            if ($("#p12_1").val() != "") {
                //Si es Otro Centro...
                if ($("#p12_1").val() == "9") {
                    if ($("input[name='p12_11']:checked").val() == undefined) {
                        $("#p12_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 10.1: Seleccione respuesta\n";
                    }
                }
            } else {
                $("#p12_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                _IsValid = false;
                _msj += "Pregunta 10.1: Seleccione respuesta\n";
            }
            //B 
            if (($("#p12_21").val() <= 0) || ($("#p12_22").val() == "")) {
                $("#p12_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 10.2: Ingresar Fecha\n";
            }
            //C
            if ($("#p12_3").val().trim() == "") {
                $("#p12_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 10.3: Especifique\n";
            }
        }
    } else {
        $("#p12").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 10: Seleccione respuesta\n";
    }

    // 11
    if ($("input[name='p13']:checked").val() != undefined) {
        if ($("input[name='p13']:checked").val() == "true") {
            if ($("#p13_11").val() != "") {
                //Si es Otro 
                if ($("#p13_11").val() == "6") {
                    if ($("#p13_12").val().trim() == "") {
                        $("#p13_12").parent().parent().parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 11.1: Ingrese respuesta\n";
                    }
                }
            } else {
                $("#p13_11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el deporte</p>");
                _IsValid = false;
                _msj += "Pregunta 11.1: Seleccione respuesta\n";
            }

            if ($("#p13_2").val() == "") {
                $("#p13_2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la frecuencia</p>");
                _IsValid = false;
                _msj += "Pregunta 11.2: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p13").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 11: Seleccione respuesta\n";
    }

    // 12 A
    if (($("#p14A_2").is(":checked") == false) && ($("#p14A_1").val().trim() == "")) {
        $("#p14A_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 12-A: Ingrese respuesta\n";
    }

    // 12 B
    if (($("#p14B_2").is(":checked") == false) && ($("#p14B_1").val().trim() == "")) {
        $("#p14B_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 12-B: Ingrese respuesta\n";
    }


    // 13
    if ($("#p15").val().trim() == "") {
        $("#p15").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 13: Ingrese respuesta\n";
    }

    //14
    if ($("input[name='p16']:checked").val() != undefined) {
        if ($("input[name='p16']:checked").val() == 2) {
            if ($("#p16_1").val() == "") {
                $("#p16_1").parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 14: Seleccione el motivo\n";
            }
        }
    } else {
        $("#p16").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 14: Seleccione el contraste\n";
    }

    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}
