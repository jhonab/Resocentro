var Tipo_div_historial = 0;
$(document).ready(function () {

    iniciarForm();
    ingresoForm();
    //ocultamos el div de historial de examenes
    $("#div_historial").hide();
});

function iniciarForm() {
    //Muestra si esta checked
    $("input:radio[mostrar-respuesta]").each(function () {

        if ($(this).is(":checked")) {
            var _search = ($(this).attr("mostrar-respuesta"));
            if (_search != undefined)
                $("#" + _search).show();
        }
    });

    // Oculta si esta checked
    $("input:radio[ocultar-respuesta]").each(function () {
        if ($(this).is(":checked")) {
            var _search = ($(this).attr("ocultar-respuesta"));
            if (_search != undefined)
                $("#" + _search).hide();
        }
    });

    //Oculta si ninguno de su grupo esta checked
    $("input:radio[ocultar-respuesta]").each(function () {
        if ($(this).is(":checked") == false) {
            var _radioName = $(this).attr("name");
            //alert($("input:radio[name='grupo_mol_p1']:checked").val());
            if (_radioName != undefined && _radioName != "") {
                var _valorSelected = $("input:radio[name='" + _radioName + "']:checked").val();
                if (_valorSelected == undefined) {
                    var _search = ($(this).attr("ocultar-respuesta"));
                    if (_search != undefined)
                        $("#" + _search).hide();
                }
            }
        }
    });
    /*--------------- CHECKBOX ---------------------*/
    $("input:checkbox[mostrar-respuesta]").each(function () {
        var _search = ($(this).attr("mostrar-respuesta"));
        if (_search != undefined)
            if ($(this).is(":checked"))
                $("#" + _search).show();
            else
                $("#" + _search).hide();
    });
    /*--------------- SELECT ---------------------*/
    $("select[mostrar-respuesta]").each(function () {
        var _target = ($(this).attr("mostrar-respuesta"));
        var _value = ($(this).attr("mostrar-respuesta-value"));
        if (_target != undefined && _value != undefined) {
            if ($(this).val() == _value)
                $("#" + _target).show();
            else
                $("#" + _target).hide();
        }
    });
    /*--------------- CHECK BLOQUEA ---------------------*/
    $("input:checkbox[bloquear-respuesta]").each(function () {
        var _search = ($(this).attr("bloquear-respuesta"));
        if (_search != undefined)
            if ($(this).is(":checked"))
                $("#" + _search).attr("disabled", true);
            else
                $("#" + _search).removeAttr("disabled");
    });
}
function ingresoForm() {


    $("input:radio[mostrar-respuesta]").each(function () {
        $(this).on('click', function () {
            var _search = ($(this).attr("mostrar-respuesta"));
            if (_search != undefined)
                $("#" + _search).show();
        });
    });
    $("input:radio[ocultar-respuesta]").each(function () {
        $(this).on('click', function () {
            var _search = ($(this).attr("ocultar-respuesta"));
            if (_search != undefined)
                $("#" + _search).hide();
        });
    });
    /*--------------- CHECKBOX ---------------------*/
    $("input:checkbox[mostrar-respuesta]").each(function () {
        $(this).on('click', function () {
            var _search = ($(this).attr("mostrar-respuesta"));
            if (_search != undefined)
                if ($(this).is(":checked"))
                    $("#" + _search).show();
                else
                    $("#" + _search).hide();
        });
    });
    /*--------------- SELECT ---------------------*/
    $("select[mostrar-respuesta]").each(function () {
        $(this).on('change', function () {
            var _target = ($(this).attr("mostrar-respuesta"));
            var _value = ($(this).attr("mostrar-respuesta-value"));
            if (_target != undefined && _value != undefined) {
                if ($(this).val() == _value)
                    $("#" + _target).show();
                else
                    $("#" + _target).hide();
            }
        });
    });
    /*--------------- CHECK BLOQUEA ---------------------*/
    $("input:checkbox[bloquear-respuesta]").each(function () {
        $(this).on('click', function () {
            var _search = ($(this).attr("bloquear-respuesta"));
            if (_search != undefined)
                if ($(this).is(":checked"))
                    $("#" + _search).attr("disabled", true);
                else
                    $("#" + _search).removeAttr("disabled");
        });
    });


}
function verHistorial(codigopaciente) {
    if ($("#div_historial").is(":visible") && Tipo_div_historial==1) {
        $("#div_historial").hide("Fold");
        Tipo_div_historial == 0;
    } else {

        $.ajax({
            url: "/Herramientas/GetHistorialEncuesta",
            data: { nexamen: codigopaciente },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al mostrar los cambios realizados', opts);
            },
            success: function (data) {
                Tipo_div_historial = 1;
                $("#div_historial").html(data.msj);
                $("#div_historial").show("Blind");

            }
        });


    }
}

function verEncuesta(examen) {
    window.open('/LectorEncuesta/LectorEncuesta?examen=' + examen+'&isVisible=false', '_blank');
}

function recuperarExamen(examen) {
    if (examen != "") {
        $.ajax({
            url: "/Herramientas/SolicitarImportacionExamen",
            data: { examen: examen },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al mostrar los cambios realizados', opts);
            },
            success: function (data) {

                toastr.success("Se envio el mensaje con exito", "", opts);
            }
        });
    }
}