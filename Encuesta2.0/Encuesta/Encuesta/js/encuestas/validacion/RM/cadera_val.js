function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined ) {
        if ($("input[name='p1']:checked").val() == "true") {
            if ($("#rpta_p1 input:checkbox:checked").length == 0) {
                $("#rpta_p1").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Seleccionar respuesta\n";
            } else {
                //otros
                if ($("#p1_7").is(":checked") && $("#p1_71").val().trim() == "") {
                    $("#rpta_p1").append("<p class='MessageValidation'>Debe ingresar el tipo de dolor</p>");
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
    //otros
    if ($("#p1_6").is(":checked") && $("#p1_61").val().trim() == "") {
        $("#rpta_p1_1").append("<p class='MessageValidation'>Debe ingresar el tipo de dolor</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Ingresar respuesta\n";
        alert
    }

    //// 2
    if ((($("#p2_1").val() > 0) && ($("#p2_2").val() != "")) ||
         ($("input[name='p2_3']:checked").val() != undefined)) {

        if ($("input[name='p2_3']:checked").val() == "false" && $("#p2_3_1").val().trim() == "") {
            $("#p2_3_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
            _IsValid = false;
            _msj += "Pregunta 2: Especifique fecha\n";
        }

    } else {
        $("#p2_1").parent().parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Ingresar fecha\n";
    }

    // 3
    if ($("input[name='p3']:checked").val() != undefined) {
        if ($("input[name='p3']:checked").val() != "3" && $("#p3_1").val().trim() == "") {
            $("#p3_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 3: Ingresar detalle\n";
        }
    } else {
        $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccione respuesta\n";
    }

    // 4
    if ($("input[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() == "1" && $("#p4_1").val().trim() == "") {
            $("#p4_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 4: Ingresar detalle\n";
        }
    } else {
        $("#p4").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccione respuesta\n";
    }

   

    ////PROCEDIMIENTOS RELACIONADOS

    // 7
    if ($("input[name='p8']:checked").val() != undefined) {
        //SI
        if ($("input[name='p8']:checked").val() == "true") {
            //A 
            if ($("#p8_1").val().trim() == "") {
                $("#p8_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 7.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p8_21").val() <= 0 || ($("#p8_22").val() == "")) {
                $("#p8_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 7.2: Seleccione respuesta\n";
            }
            //C
            if ($("input[name='p8_3']:checked").val() == undefined) {
                $("#p8_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el resultado</p>");
                _IsValid = false;
                _msj += "Pregunta 7.3: Seleccione respuesta\n";
            }
            //D
            if ($("#p8_4").val().trim() == "") {
                $("#p8_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 7.4: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 7: Seleccione respuesta\n";
    }

    // 8
    if ($("input[name='p9']:checked").val() != undefined) {
        //SI
        if ($("input[name='p9']:checked").val() == "true") {
            //A 
            if ($("#p9_1").val().trim() == "") {
                $("#p9_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 8.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p9_21").val() <= 0 || ($("#p9_22").val() == "")) {
                $("#p9_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8.2: Seleccione respuesta\n";
            }
           

        }
    } else {
        $("#p9").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione respuesta\n";
    }


    // 9
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
                        _msj += "Pregunta 9.1: Seleccione respuesta\n";
                    }
                }
            } else {
                $("#p10_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                _IsValid = false;
                _msj += "Pregunta 9.1: Seleccione respuesta\n";
            }
            //B 
            if (($("#p10_21").val() <= 0) || ($("#p10_22").val() == "")) {
                $("#p10_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 9.2: Ingresar Fecha\n";
            }
            //C
            if ($("#p10_3").val().trim() == "") {
                $("#p10_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 9.3: Especifique\n";
            }
        }
    } else {
        $("#p10").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione respuesta\n";
    }

    // 10
    if ($("input[name='p11']:checked").val() != undefined) {
        if ($("input[name='p11']:checked").val() == "true") {
            if ($("#p11_11").val() != "") {
                //Si es Otro 
                if ($("#p11_11").val() == "6") {
                    if ($("#p11_12").val().trim() == "") {
                        $("#p11_12").parent().parent().parent().append("<p class='MessageValidation'>Ingrese respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 10.1: Ingrese respuesta\n";
                    }
                }
            } else {
                $("#p11_11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el deporte</p>");
                _IsValid = false;
                _msj += "Pregunta 11.1: Seleccione respuesta\n";
            }

            if ($("#p11_2").val() == "") {
                $("#p11_2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la frecuencia</p>");
                _IsValid = false;
                _msj += "Pregunta 10.2: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 10: Seleccione respuesta\n";
    }

    // 11 A
    if (($("#p12A_2").is(":checked") == false) && ($("#p12A_1").val().trim() == "")) {
        $("#p12A_2").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 11-A: Ingrese respuesta\n";
    }

    // 11 B
    if (($("#p12B_2").is(":checked") == false) && ($("#p12B_1").val().trim() == "")) {
        $("#p12B_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 11-B: Ingrese respuesta\n";
    }


    // 12
    if ($("#p13").val().trim() == "") {
        $("#p13").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 12: Ingrese respuesta\n";
    }

    //13
    if ($("input[name='p14']:checked").val() != undefined) {
        if ($("input[name='p14']:checked").val() == 2) {
            if ($("#p14_1").val() == "") {
                $("#p14_1").parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 13: Seleccione el motivo\n";
            }
        }
    } else {
        $("#p14").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 13: Seleccione el contraste\n";
    }

    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}
