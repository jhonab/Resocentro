function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined) {
        if ($("input[name='p1']:checked").val() == "true") {
            if ($("#rpta_p1 input:checkbox:checked").length == 0) {
                $("#rpta_p1").append("<p class='MessageValidation'>Debe seleccionar al menos un item</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Seleccionar uno o varios dolores\n";
            }
            //otros
            if ($("#p1_0").val().trim() == "") {
                $("#p1_0").parent().append("<p class='MessageValidation'>Debe ingresar la ubicación</p>");
                _IsValid = false;
                _msj += "Pregunta 1: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    // 2
    if ((($("#p2").val() <= 0) || ($("#p2_1").val() == ""))) {
        $("#p2_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Ingresar fecha\n";
    }
    //3
    if ($("input[name='p3']:checked").val() != undefined) {
        if ($("input[name='p3']:checked").val() == "true") {
            if ($("#p3_1").val().trim() == "") {
                $("#p3_1").parent().append("<p class='MessageValidation'>Debe especificar</p>");
                _IsValid = false;
                _msj += "Pregunta 3: Especificar respuesta\n";
            }
        }
    } else {
        $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar una opcion</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    }

    //PROCEDIMIENTOS RELACIONADOS
    //4
    if ($("input[name='p4']:checked").val() != undefined) {
        //SI
        if ($("input[name='p4']:checked").val() == "true") {
            //A 
            if ($("#p4_1").val().trim() == "") {
                $("#p4_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la veces</p>");
                _IsValid = false;
                _msj += "Pregunta 4.1: Seleccione respuesta\n";
            }
            //B 
            if ($("#p4_2").val() <= 0 || ($("#p4_2_1").val() == "")) {
                $("#p4_2").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 4.2: Seleccione respuesta\n";
            }
            //C
            if ($("#p4_4").val().trim() == "") {
                $("#p4_4").parent().parent().parent().append("<p class='MessageValidation'>Especifique la respuesta</p>");
                _IsValid = false;
                _msj += "Pregunta 4.3: Seleccione respuesta\n";
            }
        }
    } else {
        $("#p4").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccione respuesta\n";
    }




    //PREGUNTAS A RESOLVER
    // 8
    if (($("#p8").val().trim() == "")) {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Debe ingresar el motivo del examen</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Ingresar motivo\n";
    } else {
    }

    //9
    if ($("input[name='p9']:checked").val() != undefined) {
        if ($("input[name='p9']:checked").val() == "true") {
            if ((($("#p9_1").val() < 0) || ($("#p9_2").val() == ""))) {
                $("#p9_1").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 9: Ingresar fecha\n";
            }
        }
    } else {
        $("#p9").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar una opcion</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccionar respuesta\n";
    }

    //INDICACIONES
    // 10
    if ($("input[name='ind_p1']:checked").val() != undefined) {
        if ($("input[name='ind_p1']:checked").val() == 2) {
            if ($("#ind_p1_1").val() == "") {
                $("#ind_p1_1").parent().append("<p class='MessageValidation'>Seleccione el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 10: Seleccione el motivo\n";
            }
        }
    } else {
        $("#ind_p1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione el contraste</p>");
        _IsValid = false;
        _msj += "Pregunta 10: Seleccione el contraste\n";
    }


    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}