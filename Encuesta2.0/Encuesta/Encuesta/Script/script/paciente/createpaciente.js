function validar_paciente_create() {
    var result = true;
    $("#div_paciente select.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
        }
    });

    $("#div_paciente input.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
        } else {
            if ($(this).attr("id") == "dni") {
                var tipo_doc = $("#tipo_doc").val();
                if (tipo_doc != '5') {
                    if (tipo_doc == "0") {
                        if ($(this).val().trim().length != 8) {
                            div_cont.addClass("has-warning");
                            toastr.warning("Se necesitan 8 caracteres en el DNI", "Advertencia", opts);
                        }
                    }
                }
            }
        }
    });
    $("#div_paciente textarea.validar ").each(function () {
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
        result = validarDocumento();

    return result;
}

function validarDocumento() {
    var _result = true;
    $.ajax({
        async: false,
        cache: false,
        url: '/Paciente/ValidarDocumento',
        data: { dni: $("#dni").val(), tdoc: $("#tipo_doc").val() },
        dataType: "json",
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#dni").parents(".form-group").first().addClass("has-error");
                toastr.error("El N° de Documento ya esta registrado", "Error", opts);
                _result = false;
            }
        },
        error: function () {
            $("#dni").parents(".form-group").first().addClass("has-error");
            toastr.error("El N° de Documento ya esta registrado", "Error", opts);
            _result = false;
        }
    });
    return _result;
}
function agregarPaciente() {
    if (validar_paciente_create()) {
        var persona = {
            nacionalidad: $("#nacionalidad").val(),
            direccion: $("#direccion").val(),
            email: $("#email").val(),
            celular: $("#celular").val(),
            telefono: $("#telefono").val(),
            fechanace: $("#fechanace").val(),
            sexo: $("#sexo").val(),
            nombres: $("#nombres").val(),
            apellidos: $("#apellidos").val(),
            dni: $("#dni").val(),
            tipo_doc: $("#tipo_doc").val(),
            codigopaciente: 0
        };
        $.ajax({
            async: false,
            cache: false,
            url: "/Paciente/CreatePacienteAjax",
            data: { item: JSON.stringify(persona) },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al enviar data', 'Error', opts);
            },
            success: function (data) {
                if (data.result) {
                    $("#codigopaciente").val(data.idpaciente),
                    $("#modal-add").modal('hide');
                    toastr.success('Paciente Agregado', 'Exito', opts);
                } else {
                    $("#codigopaciente").val("0"),
                    toastr.error('Error guardar paciente intentelo nuevamente', 'Error', opts);
                }
            }
        });
    }
}