function validar_medico_create() {
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
    else
        result = validarCMP();

    return result;
}

function validarCMP() {
    var _result = true;
    $.ajax({
        async: false,
        cache: false,
        url: '/Medico/ValidarCMP',
        data: { cmp: $("#cmp").val()},
        dataType: "json",
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#cmp").parents(".form-group").first().addClass("has-error");
                toastr.error("El N° de CMP ya esta registrado", "Error", opts);
                _result = false;
            }
        },
        error: function () {
            $("#cmp").parents(".form-group").first().addClass("has-error");
            toastr.error("El N° de CMP ya esta registrado", "Error", opts);
            _result = false;
        }
    });
    return _result;
}
function agregarMedico() {
    if (validar_medico_create()) {
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
            url: "/Medico/CreateMedicoAjax",
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
                    toastr.success('Medico Agregado', 'Exito', opts);
                } else {
                    $("#cmp").val("0"),
                    toastr.error('Error guardar medico intentelo nuevamente', 'Error', opts);
                }
            }
        });
    }
}