function validarEncuesta() {
    $(".MessageValidation").remove();
    var _IsValid = true;
    var _msj = "";
    //MOLESTIAS
    // 1
    if ($("input[name='p1']:checked").val() != undefined) {
        
    } else {
        $("#p1").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 1: Seleccionar respuesta\n";
    }

   

    //3
    if ($("input[name='p3']:checked").val() != undefined) {      
    } else {
        $("#p3").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 3: Seleccionar respuesta\n";
    }

    //4
    if ($("input[name='p4']:checked").val() != undefined) {       
    } else {
        $("#p4").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 4: Seleccionar respuesta\n";
    }

    //5
    if ($("input[name='p5']:checked").val() != undefined) {
    } else {
        $("#p5").parent().parent().parent().append("<p class='MessageValidation'>Debe seleccionar respuesta</p>");
        _IsValid = false;
        _msj += "Pregunta 5: Seleccionar respuesta\n";
    }

    // 6
    if ($("#p6").val().trim() == "") {
        
            $("#p6").parent().parent().append("<p class='MessageValidation'>Debe seleccionar</p>");
            _IsValid = false;
            _msj += "Pregunta 6: Seleccione su repuesta\n";
        
    }

    // 9
    if ($("#p9").val().trim() == "") {

        $("#p9").parent().parent().append("<p class='MessageValidation'>Debe ingresar el motivo del examen</p>");
        _IsValid = false;
        _msj += "Pregunta 9: Seleccione su repuesta\n";

    }



    if (_IsValid == false) {
        toastr.error('Falta completar datos.<br/>Verifique las preguntas: <br/>' + _msj,opts);
        return false;
    }

    return true;
}
