function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='mol_p1']:checked").val() != undefined) {
        if ($("input[name='mol_p1']:checked").val() == "true") {
            if ($("#rpta_p1 input:checkbox:checked").length == 0) {
                $("#rpta_p1").append("<br/><p class='MessageValidation'> Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Seleccionar respuesta\n";
            } else {
               
                //otros
                if ($("#mol_p1_3").is(":checked") && $("#mol_p1_3_1").val().trim() == "") {
                    $("#mol_p1_3").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
                    _IsValid = false;
                    _msj += "Pregunta 1: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#mol_p1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    // 2
    if ($("input[name='mol_p2']:checked").val() != undefined) {
        if ($("input[name='mol_p2']:checked").val() == "true") {
            if ($("#rpta_p2 input:checkbox:checked").length == 0) {
                $("#rpta_p2").append("<p class='MessageValidation'>Debe seleccionar por los menos un  item</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Seleccionar respuesta\n";
            } else {
                //Sensacion de anestesia
                if ($("#mol_p2_2").is(":checked") && $("#mol_p2_2_1").val().trim() == "") {
                    $("#mol_p2_2").parent().parent().append("<p class='MessageValidation'>Debe ingresar la ubicación</p>");
                    _IsValid = false;
                    _msj += "Pregunta 2: Ingresar respuesta\n";
                }
                //otros
                if ($("#mol_p2_3").is(":checked") && $("#mol_p2_3_1").val().trim() == "") {
                    $("#mol_p2_3").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
                    _IsValid = false;
                    _msj += "Pregunta 2: Ingresar respuesta\n";
                }
            }
        }
    } else {
        $("#mol_p2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }


    //// 4
    if (($("#mol_p4_1").val() <= 0) || ($("#mol_p4_2").val() == "")) {
        $("#mol_p4_1").parent().parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Ingresar fecha\n";
    }

    // 5
    if ($("input[name='mol_p5']:checked").val() != undefined) {
        if ($("input[name='mol_p5']:checked").val() == "true" && $("#mol_p5_1").val().trim() == "") {
            $("#mol_p5_1").parent().parent().append("<p class='MessageValidation'>Debe especificar</p>");
            _IsValid = false;
            _msj += "Pregunta 5: Ingresar detalle\n";
        }
    } else {
        $("#mol_p5").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccione respuesta\n";
    }



    ////PROCEDIMIENTOS RELACIONADOS

    // 8
    if ($("input[name='pro_p1']:checked").val() != undefined) {
        //SI
        if ($("input[name='pro_p1']:checked").val() == "true") {
            //A 
            if ($("#pro_p1_1").val().trim() == "") {
                $("#pro_p1_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 8.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#pro_p1_21").val() <= 0 || ($("#pro_p1_22").val() == "")) {
                $("#pro_p1_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8.2: Seleccione respuesta\n";
            }
            //C
            if ($("input[name='pro_p1_3']:checked").val() == undefined) {
                $("#pro_p1_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el resultado</p>");
                _IsValid = false;
                _msj += "Pregunta 8.3: Seleccione respuesta\n";
            }
            //D
            if ($("#pro_p1_4").val().trim() == "") {
                $("#pro_p1_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 8.4: Seleccione respuesta\n";
            }
        }
    } else {
        $("#pro_p1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione respuesta\n";
    }

    // 9
    if ($("input[name='pro_p2']:checked").val() != undefined) {
        //SI
        if ($("input[name='pro_p2']:checked").val() == "true") {
            //A 
            if ($("#pro_p2_1").val() != "") {
                //Si es Otro Centro...
                if ($("#pro_p2_1").val() == "9") {
                    if ($("input[name='pro_p2_11']:checked").val() == undefined) {
                        $("#pro_p2_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 9.1: Seleccione respuesta\n";
                    }
                }
            } else {
                $("#pro_p2_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                _IsValid = false;
                _msj += "Pregunta 9.1: Seleccione respuesta\n";
            }
            //B 
            if (($("#pro_p2_21").val() <= 0) || ($("#pro_p2_22").val() == "")) {
                $("#pro_p2_21").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 9.2: Ingresar Fecha\n";
            }
            //C
            if ($("#pro_p2_3").val().trim() == "") {
                $("#pro_p2_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 9.3: Especifique\n";
            }
        }
    } else {
        $("#pro_p2").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione respuesta\n";
    }



    // 10 A
    if (($("#pre_p1Acheck").is(":checked") == false) && ($("#pre_p1A").val().trim() == "")) {
        $("#pre_p1Acheck").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la orden médica</p>");
        _IsValid = false;
        _msj += "Pregunta 10-A: Ingrese respuesta\n";
    }

    // 10 B
    if (($("#pre_p1Bcheck").is(":checked") == false) && ($("#pre_p1B").val().trim() == "")) {
        $("#pre_p1Bcheck").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que comenta el paciente</p>");
        _IsValid = false;
        _msj += "Pregunta 10-B: Ingrese respuesta\n";
    }


    // 11
    if ($("#pre_p2").val().trim() == "") {
        $("#pre_p2").parent().parent().parent().append("<p class='MessageValidation'>Ingrese lo que se desea resolver</p>");
        _IsValid = false;
        _msj += "Pregunta 11: Ingrese respuesta\n";
    }

    //12
    if ($("input[name='ind_p1']:checked").val() != undefined) {
        if ($("input[name='ind_p1']:checked").val() == 2) {
            if ($("#ind_p1_1").val() == "") {
                $("#ind_p1_1").parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 12: Seleccione el motivo\n";
            }
        }
    } else {
        $("#ind_p1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 12: Seleccione el contraste\n";
    }

    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}
