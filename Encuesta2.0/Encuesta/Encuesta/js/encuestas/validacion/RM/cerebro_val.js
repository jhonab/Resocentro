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
                if ($("#p1_3").is(":checked") && $("#p1_4").val().trim() == "") {
                    $("#rpta_p1").append("<p class='MessageValidation'>Debe ingresar el tipo de dolor</p>");
                    _IsValid = false;
                    _msj += "Pregunta 1: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#p1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    //// 2
    if ($("input[name='p2']:checked").val() != undefined) {
        if ($("input[name='p2']:checked").val() == "true") {
            if ($("input[name='p2_1']:checked").val() == undefined) {
                $("#rpta_p2").append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Seleccionar respuesta\n";
            }
        }
    } else {
        $("#p2").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }

    //// 3
    if ($("input[name='p3']:checked").val() != undefined) {
        // SI ?
        if ($("input[name='p3']:checked").val() == "true") {
            if ($("input[name='p3_1']:checked").val() != undefined) {
                //Seguido de Perdidad de conciencia = SI ?
                if ($("input[name='p3_1']:checked").val() == "true") {
                    //CUando
                    if ($("input[name='p3_1_1']:checked").val() == undefined) {
                        $("#p3_1_1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 3.1: Seleccionar respuesta\n";
                    }
                    //Tiempo
                    if ($("input[name='p3_1_2']:checked").val() == undefined) {
                        $("#p3_1_2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 3.2: Seleccionar respuesta\n";
                    }
                    //Seguido
                    if ($("#rpta_p3_1 input:checkbox:checked").length == 0) {
                        $("#p3_1_3").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 3.2: Seleccionar respuesta\n";
                    }
                }

            } else {
                $("#rpta_p3").append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 3: Seleccionar respuesta\n";
            }
        }
    } else {
        $("#p3").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    }

    //// 4
    if ($("input[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() == "true") {
            if ($("input[name='p4_1']:checked").val() == undefined) {
                $("#rpta_p4").append("<p class='MessageValidation'>Debe seleccionar su respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 4: Seleccionar respuesta\n";
            }
        }
    } else {
        $("#p4").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccionar respuesta\n";
    }

    //// 5
    if ($("input[name='p5']:checked").val() != undefined) {
        if ($("input[name='p5']:checked").val() == "true") {
            if ($("#rpta_p5 input:checkbox:checked").length == 0) {
                $("#rpta_p5").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 5: Seleccionar respuesta\n";
            } else {
                //otros
                if ($("#p5_4").is(":checked") && $("#p5_4_1").val().trim() == "") {
                    $("#rpta_p5").append("<p class='MessageValidation'>Debe ingresar el tipo de razón</p>");
                    _IsValid = false;
                    _msj += "Pregunta 5: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#p5").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccionar respuesta\n";
    }


    //// 6
    if ($("input[name='p6']:checked").val() != undefined) {
        if ($("input[name='p6']:checked").val() == "true") {
            if ($("#rpta_p6 input:checkbox:checked").length == 0) {
                $("#rpta_p6").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 6: Seleccionar respuesta\n";
            } else {
                //otros
                if ($("#p6_7").is(":checked") && $("#p6_7_1").val().trim() == "") {
                    $("#rpta_p6").append("<p class='MessageValidation'>Debe ingresar el tipo el motivo</p>");
                    _IsValid = false;
                    _msj += "Pregunta 6: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#p6").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Seleccionar respuesta\n";
    }

    //// 7
    if (
     ($("input[name='p1']:checked").val() == "false") &&
      ($("input[name='p2']:checked").val() == "false") &&
      ($("input[name='p3']:checked").val() == "false") &&
      ($("input[name='p4']:checked").val() == "false") &&
      ($("input[name='p5']:checked").val() == "false") &&
      ($("input[name='p6']:checked").val() == "false")
     ) {
        if ($("#p7").val().trim() == "") {
            $("#p7").parent().parent().append("<p class='MessageValidation'>Debe ingresar una molestia</p>");
            _IsValid = false;
            _msj += "Pregunta 7: Ingresar respuesta\n";
        }
    }

    //// 8
    if (($("#p8").val() <= 0) || ($("#p8_1").val() == "")) {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Ingresar Fecha\n";
    }

    //// 9
    if ($("input[name='p9']:checked").val() != undefined) {
        if ($("input[name='p9']:checked").val() == "true" && $("#p9_1").val().trim() == "") {
            $("#p9").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 9: Ingresar detalle\n";
        }
    } else {
        $("#p9").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione respuesta\n";
    }

    //// 10
    if ($("input[name='p10']:checked").val() != undefined) {
        if ($("input[name='p10']:checked").val() == "true" && $("#p10_1").val().trim() == "") {
            $("#rpta_p10").append("<p class='MessageValidation'>Debe ingresar los antecedentes</p>");
            _IsValid = false;
            _msj += "Pregunta 10: Ingresar antecedentes\n";
        }
    } else {
        $("#p10").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 10: Seleccione respuesta\n";
    }

    ////PROCEDIMIENTOS RELACIONADOS

    //// 11
    if ($("input[name='p11']:checked").val() != undefined) {
        //SI
        if ($("input[name='p11']:checked").val() == "true") {
            //A 
            if ($("#p11_1").val().trim() == "") {
                $("#p11_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 11.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p11_2").val() <= 0 || ($("#p11_3").val() == "")) {
                $("#p11_2").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 11.2: Seleccione respuesta\n";
            }
            //C
            if ($("#p11_4").val().trim() == "") {
                $("#p11_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 11.3: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p11").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 11: Seleccione respuesta\n";
    }

    // 12
    if ($("input[name='p12']:checked").val() != undefined) {
        //SI
        if ($("input[name='p12']:checked").val() == "true") {
            // Clic en "Resonancia" o "Tomografia"
            if ($("#rpta_p12 input:checkbox:checked").length > 0) {
                //Clic en RESONANCIA
                if ($("#p12_1").is(":checked")) {
                    //A 
                    if ($("#p12_1_1").val() != "") {
                        //Si es Otro Centro...
                        if ($("#p12_1_1").val() == "9") {
                            if ($("input[name='p12_1_2']:checked").val() == undefined) {
                                $("#p12_1_2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                                _IsValid = false;
                                _msj += "Pregunta 12.1: Seleccione respuesta\n";
                            }
                        }
                    } else {
                        $("#p12_1_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.1: Seleccione respuesta\n";
                    }
                    //B 
                    if (($("#p12_1_3").val() <= 0) || ($("#p12_1_4").val() == "")) {
                        $("#p12_1_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.1: Ingresar Fecha\n";
                    }
                    //C
                    if ($("#p12_1_5").val().trim() == "") {
                        $("#p12_1_5").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.1: Especifique\n";
                    }
                }
                //CLic en TOMOGRAFIA
                if ($("#p12_2").is(":checked")) {
                    //A 
                    if ($("#p12_2_1").val() != "") {
                        //Si es Otro Centro...
                        if ($("#p12_2_1").val() == "9") {
                            if ($("input[name='p12_2_2']:checked").val() == undefined) {
                                $("#p12_2_2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                                _IsValid = false;
                                _msj += "Pregunta 12.2: Seleccione respuesta\n";
                            }
                        }

                    } else {
                        $("#p12_2_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.2: Seleccione respuesta\n";
                    }
                    //B 
                    if (($("#p12_2_3").val() <= 0) || ($("#p12_2_4").val() == "")) {
                        $("#p12_2_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.2: Ingresar Fecha\n";
                    }
                    //C
                    if ($("#p12_2_5").val().trim() == "") {
                        $("#p12_2_5").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                        _IsValid = false;
                        _msj += "Pregunta 12.2: Especifique\n";
                    }
                }
            } else {
                $("#p12").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 12: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p12").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 12: Seleccione respuesta\n";
    }

    // 13 A
    if (($("#p13A_1").is(":checked") == false) && ($("#p13A").val().trim() == "")) {
        $("#p13A").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 13-A: Ingrese respuesta\n";
    }

    // 13 B
    if (($("#p13B_1").is(":checked") == false) && ($("#p13B").val().trim() == "")) {
        $("#p13B").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 13-B: Ingrese respuesta\n";
    }


    // 14
    if ($("#p14").val().trim() == "") {
        $("#p14").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 14: Ingrese respuesta\n";
    }


    //15
    if ($("#rpta_15 input:checkbox:checked").length == 0) {
        $("#rpta_15").append("<p class='MessageValidation'>Seleccione los protocolos a realizar</p>");
        _IsValid = false;
        _msj += "Pregunta 15: Ingrese respuesta\n";
    } else {
        // Protocolos de Investigacion
        if ($("#p15_12").is(":checked") && $("#p15_12_1").val().trim() == "") {
            $("#p15_12").parent().parent().parent().append("<p class='MessageValidation'>Ingrese los protocolos</p>");
            _IsValid = false;
            _msj += "Pregunta 15: Ingrese respuesta\n";
        }

        //Otros
        if ($("#p15_13").is(":checked") && $("#p15_13_1").val().trim() == "") {
            $("#p15_13").parent().parent().parent().append("<p class='MessageValidation'>Ingrese los protocolos</p>");
            _IsValid = false;
            _msj += "Pregunta 15: Ingrese respuesta\n";
        }
    }
    //16
    if ($("#rpta_16 input:checkbox:checked").length == 0) {
        $("#rpta_16").append("<p class='MessageValidation'>Seleccione las secuencias</p>");
        _IsValid = false;
        _msj += "Pregunta 16: Ingrese respuesta\n";
    } else {
        //Otros
        if ($("#p16_10").is(":checked") && $("#p16_10_1").val().trim() == "") {
            $("#p16_10").parent().parent().parent().append("<p class='MessageValidation'>Ingrese las secuencias</p>");
            _IsValid = false;
            _msj += "Pregunta 16: Ingrese respuesta\n";
        }
    }
    
    //17
    if ($("input[name='p17']:checked").val() != undefined) {
        if ($("input[name='p17']:checked").val() == 1) {
            if ($("#p17_1").val() == "") {
                $("#p17_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 17: Seleccione el motivo\n";
            }
        }
    } else {
        $("#p17_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 17: Seleccione el contraste\n";
    }



    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}