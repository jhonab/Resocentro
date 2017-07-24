
function validar_medico_update() {
    var result = true;
    $("#div_medico select.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
        }
    });

    $("#div_medico input.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
        }
    });



    $(".has-error").each(function () {
        result = false;
    });
    $(".has-warning").each(function () {
        result = false;
    });
    if (!result)
        toastr.error("Verifique los datos", "Error", opts);
  

    return result;
}


function updateMedico() {
    if (validar_medico_update()) {
        var persona = {
            nombres: $("#nombres").val(),
            apellidos: $("#apellidos").val(),
            telefono: $("#telefono").val(),
            direccion: $("#direccion").val(),
            codigoespecialidad: $("#codigoespecialidad").val(),
            cmp: $("#cmp").val(),
            tipomedico: $("#cbotipomedico").val()
        };
        $.ajax({
            async: false,
            cache: false,
            url: "/Medico/UpdateMedicoAjax",
            data: { item: JSON.stringify(persona) },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al enviar data', 'Error', opts);
            },
            success: function (data) {
                if (data.result) {
                    $("#cmp").val(data.cmp),
                    $("#modal-add").modal('hide');
                    toastr.success('Médico Modificado', 'Exito', opts);
                } else {
                    $("#codigopaciente").val("0"),
                    toastr.error('Error modificar médico intentelo nuevamente', 'Error', opts);
                }
            }
        });
    }
}