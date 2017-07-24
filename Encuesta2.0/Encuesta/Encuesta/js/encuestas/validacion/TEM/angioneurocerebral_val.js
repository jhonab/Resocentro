﻿function validarEncuesta() {
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
    if ($("input:radio[name='p3']:checked").val() != undefined) {
        if ($("input[name='p3']:checked").val() == "true") {
            if (($("#p3_2").val() == "")) {
                $("#p3_2").parent().append("<p class='MessageValidation'> Especifique</p>");
                _IsValid = false;
                _msj += "Pregunta 3: Ingresar respuesta\n";
            }
            
        }
    } else {
        $("#p3").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    }

    // 4
    if ($("input:radio[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() == "true") {
            if (($("#p4_1").val() == "")) {
                $("#p4_1").parent().append("<p class='MessageValidation'>Especifique</p>");
                _IsValid = false;
                _msj += "Pregunta 4: Ingresar respuesta\n";
            }
           
        }
    } else {
        $("#p4").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccionar respuesta\n";
    }

    //5
    if ($("input:radio[name='p5']:checked").val() == undefined) {
        $("#p5").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccionar respuesta\n";
    }

    // 6
    if ($("input:radio[name='p6']:checked").val() != undefined) {
        if ($("input[name='p6']:checked").val() == "true") {
            if (($("#p6_1").val() == "")) {
                $("#p6_1").parent().append("<p class='MessageValidation'>Especifique</p>");
                _IsValid = false;
                _msj += "Pregunta 6: Ingresar respuesta\n";
            }

        }
    } else {
        $("#p6").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Seleccionar respuesta\n";
    }


    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}