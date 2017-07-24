function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined) {
        if ($("input[name='p1']:checked").val() == "true" && $("#p1_1").val().trim() == "") {
            $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe especificar respuesta</p>");
            _IsValid = false;
            _msj += "Pregunta 1: Ingrese respuesta\n";
        }
    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

    // 2
    if ($("input[name='p2']:checked").val() != undefined) {
        if ($("input[name='p2']:checked").val() == "true") {
            //cancer
            if ($("input[name='p2_1']:checked").val() != undefined) {
                if ($("input[name='p2_1']:checked").val() == "true") {
                    if ($("input[name='p2_1_1']:checked").val() != undefined) {
                        if ($("input[name='p2_1_1']:checked").val() == "8" && $("#p2_1_2").val().trim() == "") {
                            $("#p2_1").parent().parent().append("<p class='MessageValidation'>Debe especificar respuesta</p>");
                            _IsValid = false;
                            _msj += "Pregunta 2: Ingrese respuesta\n";
                        }
                    } else {
                        $("#p2_1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar un item</p>");
                        _IsValid = false;
                        _msj += "Pregunta 2: Seleccionar respuesta\n";
                    }
                }
            } else {
                $("#p2_1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Seleccionar respuesta\n";
            }
            //anatomia
            if ($("#p2_2").val().trim() == "") {
                $("#p2_2").parent().append("<p class='MessageValidation'>Debe especificar</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Ingresar respuesta\n";
            }
            //grado
            if ($("#p2_3").val().trim() == "") {
                $("#p2_3").parent().append("<p class='MessageValidation'>Debe especificar</p>");
                _IsValid = false;
                _msj += "Pregunta 2: Ingresar respuesta\n";
            }
        }
    } else {
        $("#p2").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 2: Seleccionar respuesta\n";
    }

    //3
    if ($("input[name='p3']:checked").val() == undefined) {
        $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar una opcion</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    } else {
        if ($("input[name='p2_1_1']:checked").val() == "8" && $("#p2_1_2").val().trim() == "") {
            $("#p2_1").parent().parent().append("<p class='MessageValidation'>Debe especificar respuesta</p>");
            _IsValid = false;
            _msj += "Pregunta 2: Ingrese respuesta\n";
        }
    }

    //4
    if ($("input[name='p4']:checked").val() != undefined) {
        if ($("input[name='p4']:checked").val() == "4" && $("#p3_1").val().trim() == "") {
            $("#p4").parent().parent().parent().append("<p class='MessageValidation'>Debe especificar respuesta</p>");
            _IsValid = false;
            _msj += "Pregunta 4: Ingrese respuesta\n";
        }
    } else {
        $("#p4").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccionar respuesta\n";
    }

    //PROCEDIMIENTOS RELACIONADOS
    // 5
    if ($("input:radio[name='p5']:checked").val() != undefined) {
        if ($("input[name='p5']:checked").val() == "true") {
            //Histerectomia
            if ($("input:radio[name='p5_1']:checked").val() != undefined) {
                if ($("input[name='p5_1']:checked").val() == "true") {
                    if (($("#p5_1_1").val() <= 0) && ($("#p5_1_2").val() == "")) {
                        $("#p5_1_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                        _IsValid = false;
                        _msj += "Pregunta 5-1: Ingresar fecha\n";
                    }
                }
            } else {
                $("#p5_1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 5-1: Seleccionar respuesta\n";
            }
            //Prostatectomia
            if ($("input:radio[name='p5_2']:checked").val() != undefined) {
                if ($("input[name='p5_2']:checked").val() == "true") {
                    if (($("#p5_2_1").val() <= 0) && ($("#p5_2_2").val() == "")) {
                        $("#p5_2_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                        _IsValid = false;
                        _msj += "Pregunta 5-2: Ingresar fecha\n";
                    }
                }
            } else {
                $("#p5_2").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 5-2: Seleccionar respuesta\n";
            }
            //Colecistectomia
            if ($("input:radio[name='p5_3']:checked").val() != undefined) {
                if ($("input[name='p5_3']:checked").val() == "true") {
                    if (($("#p5_3_1").val() <= 0) && ($("#p5_3_2").val() == "")) {
                        $("#p5_3_1").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                        _IsValid = false;
                        _msj += "Pregunta 5-3: Ingresar fecha\n";
                    }
                }
            } else {
                $("#p5_3").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 5-3: Seleccionar respuesta\n";
            }
            //Otra
            if (($("#p5_4").val().trim() == "") &&
                ($("input[name='p5_1']:checked").val() == "false" &&
                $("input[name='p5_2']:checked").val() == "false" &&
                $("input[name='p5_3']:checked").val() == "false")) {
                $("#p5_4").parent().append("<p class='MessageValidation'>Especificar cual</p>");
                _IsValid = false;
                _msj += "Pregunta 5-4: Especificar cual\n";
            }

        }
    } else {
        $("#p5").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccionar respuesta\n";
    }

    //6
    if ($("input:radio[name='p6']:checked").val() != undefined) {
        if ($("input[name='p6']:checked").val() == "true") {
            if ($("input:radio[name='p6_1']:checked").val() == undefined) {
                $("#p6_1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 6: Seleccionar respuesta\n";
            }
            if (($("#p6_3").val() <= 0) || ($("#p6_2").val() == "")) {
                $("#p6_3").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 6: Ingresar fecha\n";
            }
        }
    } else {
        $("#p6").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 6: Seleccionar respuesta\n";
    }

    //7
    if ($("input:radio[name='p7']:checked").val() != undefined) {
        if ($("input[name='p7']:checked").val() == "true") {
            if ($("input:radio[name='p7_1']:checked").val() == undefined) {
                $("#p7_1").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
                _IsValid = false;
                _msj += "Pregunta 7: Seleccionar respuesta\n";
            }
            if (($("#p7_3").val() <= 0) || ($("#p7_2").val() == "")) {
                $("#p7_3").parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 7: Ingresar fecha\n";
            }
        }
    } else {
        $("#p7").parent().parent().append("<p class='MessageValidation'>Debe seleccionar Si o No</p>");
        _IsValid = false;
        _msj += "Pregunta 7: Seleccionar respuesta\n";
    }

    //8
    if ($("input[name='p8']:checked").val() != undefined) {
        //SI
        if ($("input[name='p8']:checked").val() == "true") {
            //A 
            if ($("#p8_1").val() != "") {
                //Si es Otro Centro...
                if ($("#p8_1").val() == "9") {
                    if ($("input[name='p8_2']:checked").val() == undefined) {
                        $("#p8_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
                        _IsValid = false;
                        _msj += "Pregunta 8.1: Seleccione respuesta\n";
                    }
                }
            } else {
                $("#p8_1").parent().parent().parent().append("<p class='MessageValidation'>Seleccione la sede</p>");
                _IsValid = false;
                _msj += "Pregunta 8.1: Seleccione respuesta\n";
            }
            //B 
            if (($("#p8_3").val() <= 0) || ($("#p8_4").val() == "")) {
                $("#p8_3").parent().parent().parent().append("<p class='MessageValidation'>Ingrese la fecha</p>");
                _IsValid = false;
                _msj += "Pregunta 8.2: Ingresar Fecha\n";
            }
            //C
            if ($("#p8_5").val().trim() == "") {
                $("#p8_5").parent().parent().parent().append("<p class='MessageValidation'>Ingrese el motivo</p>");
                _IsValid = false;
                _msj += "Pregunta 8.3: Especifique\n";
            }
        }
    } else {
        $("#p8").parent().parent().parent().append("<p class='MessageValidation'>Seleccione respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 8: Seleccione respuesta\n";
    }


    if (_IsValid == false) {
        alert('Falta completar datos.\nVerifique las preguntas: \n' + _msj);
        return false;
    }

    return true;
}