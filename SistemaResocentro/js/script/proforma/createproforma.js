var tipo_modal_add = 0;//0 ninguno,1 paciente,2 medico
var tipo_modal_search = 0;//0 ninguno,1 paciente,2 ,medico
$(function () {
    $("#cbounidadnegocio").change(cargarModalidad);
    $("#proforma_codigocompaniaseguro").change(deleteAllEstudios);    

    cerrarModalAdd();
    cerrarModalSearch();
    //ocultamos los botones de UPDATE
    $("#btnupdatePaciente").hide();
    $("#btnupdateMedico").hide();
});

function cargarModalidad() {
    var _unidad = $("#cbounidadnegocio").val().substring(0, 1);
    if (_unidad != "") {
        $.ajax({
            url: "/CartaGarantia/getModalidad",
            data: { idunidad: _unidad },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al cargar las modalidades', 'Error', opts);
            },
            success: function (data) {
                var items = "";
                items += "<option value='' >- Seleccione -</option>";
                $.each(data, function (i, item) {
                    items += "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
                });
                $("#cbomodalidad").html(items);
                $("#cbomodalidad").select2({
                    placeholder: "Select a state",
                    allowClear: true
                });
                deleteAllEstudios();
            }
        });
    } else {
        toastr.warning('Seleccione un Unidad de negocio', 'Informacion', opts);
    }

}

function cargarEstudios() {
    var _unidad = $("#cbounidadnegocio").val();
    var _modalidad = $("#cbomodalidad").val();
    var _clase = $("#cboclase").val();
    var _aseguradora = $("#proforma_codigocompaniaseguro").val();
    if (_unidad != "" && _modalidad != "" && _clase != "" && _aseguradora != "") {
        $("#lst_estudio").load("/CartaGarantia/getEstudio?idunidad=" + _unidad + "&idmodalidad=" + _modalidad + "&idclase=" + _clase + "&aseguradora=" + _aseguradora, function (response, status, xhr) {
            if (status == "error") {
                toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
            }
        });
    } else {
        toastr.warning('Seleccione un aseguradora, unidad de negocio, modalidad y la clase', 'Informacion', opts);
    }

}

function cerrarModalAdd() {
    $('#modal-add').on('hide.bs.modal', function (e) {
        //PACIENTE
        if (tipo_modal_add == 1) {
            if ($("#codigopaciente").val() >0) {
                $("#proforma_codigopaciente").val($("#codigopaciente").val());
                $("#paciente").val($("#apellidos").val() + ", " + $("#nombres").val());
                $("#btnupdatePaciente").show();
                $("#btnaddPaciente").hide();
            } else {
                $("#proforma_codigopaciente").val("");
                $("#paciente").val("");
                $("#btnupdatePaciente").hide();
                $("#btnaddPaciente").show();
            }
        }
        //MEDICO
        if (tipo_modal_add == 2) {
            if ($("#cmp").val() >0) {
                $("#proforma_cmp").val($("#cmp").val());
                $("#medico").val($("#apellidos").val() + ", " + $("#nombres").val());
                $("#btnupdateMedico").show();
                $("#btnaddMedico").hide();
            } else {
                $("#proforma_cmp").val("");
                $("#medico").val("");
                $("#btnupdateMedico").hide();
                $("#btnaddMedico").show();
            }
        }
    })
}

function cerrarModalSearch() {
    $('#modal-search').on('hide.bs.modal', function (e) {
        //PACIENTE
        if (tipo_modal_search == 1) {
            if ($("#result_idpaciente").val() != "" && $("#result_nombres").val() != "") {
                $("#proforma_codigopaciente").val($("#result_idpaciente").val());
                $("#paciente").val($("#result_nombres").val());
                $("#btnupdatePaciente").show();
                $("#btnaddPaciente").hide();
            } else {
                $("#proforma_codigopaciente").val("");
                $("#paciente").val("");
                $("#btnupdatePaciente").hide();
                $("#btnaddPaciente").show();
            }
        }
        //MEDICO
        if (tipo_modal_search == 2) {
            if ($("#result_cmp").val() != "" && $("#result_nombres").val() != "") {
                $("#proforma_cmp").val($("#result_cmp").val());
                $("#medico").val($("#result_nombres").val());
                $("#btnupdateMedico").show();
                $("#btnaddMedico").hide();
            } else {
                $("#proforma_cmp").val("");
                $("#medico").val("");
                $("#btnupdateMedico").hide();
                $("#btnaddMedico").show();
            }
        }
    })
}

//PACIENTE
function deletePaciente_modal() {
    $("#proforma_codigopaciente").val("");
    $("#paciente").val("");
    $("#btnupdatePaciente").hide();
    $("#btnaddPaciente").show();
}
function addPaciente_modal() {
    $("#modal-add_body").load("/Paciente/CreatePaciente", function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_add = 1//tipo modal agregar paciente
            $("#modal-add").modal('show');
        }
    });
}
function updatePaciente_modal() {
    var _codigopaciente = $("#proforma_codigopaciente").val().trim();
    if (_codigopaciente != "") {
    $("#modal-add_body").load("/Paciente/UpdatePaciente?codigopaciente=" + _codigopaciente, function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_add = 1;//tipo modal agregar paciente
            $("#modal-add").modal('show');
        }
    });
    } else
        toastr.error('Debe seleccionar un Paciente', 'Error', opts);
}
function searchPaciente_modal() {
    $("#modal-search_body").load("/Paciente/SearchPaciente", function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_search = 1;//tipo modal buscar paciente
            $("#modal-search").modal('show');
        }
    });
}

//MEDICO
function deleteMedico_modal() {
    $("#proforma_cmp").val("");
    $("#medico").val("");
    $("#btnupdateMedico").hide();
    $("#btnaddMedico").show();
}
function addMedico_modal() {
   $("#modal-add_body").load("/Medico/CreateMedico", function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_add = 2;//tipo modal agregar medico
            $("#modal-add").modal('show');
        }
    });
}
function updateMedico_modal() {
    var _cmp = $("#proforma_cmp").val().trim();
 
        $("#modal-add_body").load("/Medico/UpdateMedico?cmp=" + _cmp, function (response, status, xhr) {
            if (status == "error") {
                toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
            } else {
                tipo_modal_add = 2;//tipo modal agregar medico
                $("#modal-add").modal('show');
            }
        });
    
}
function searchMedico_modal() {
    $("#modal-search_body").load("/Medico/SearchMedico", function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_search = 2;//tipo modal buscar medico
            $("#modal-search").modal('show');
        }
    });
}


function validarProforma() {
    if (validar_proforma_create())
        return true;    
    return false;
}

function validar_proforma_create() {
    var result = true;
    $("#div_proforma select.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
        }
    });

    $("#div_proforma input.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
            if ($(this).attr("id") == "estudios") {
                $(".t1").parents(".form-group").first().addClass("has-error");
            }
        }
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
