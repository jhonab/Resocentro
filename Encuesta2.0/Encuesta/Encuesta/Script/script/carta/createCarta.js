var tipo_modal_add = 0;//0 ninguno,1 paciente,2 medico,3 cie,4 beneficio
var tipo_modal_search = 0;//0 ninguno,1 paciente,2 medico,3 cie, 4 beneficio
$(function () {
    $("#cbounidadnegocio").change(cargarModalidad);
    $("#carta_codigocompaniaseguro").change(AseguradoraCambio);
    $("#carta_SitedCodigoProducto").change(getBeneficio);
    verificarDetallesAseguradora();
    cerrarModalAdd();
    cerrarModalSearch();
    //ocultamos los botones de UPDATE
    $("#btnupdatePaciente").hide();
    $("#btnupdateMedico").hide();
    //oclutamos la lista de cartas
    $("#content_cartas_old").hide();
});

function AseguradoraCambio() {
    deleteAllEstudios();
    verificarDetallesAseguradora();
}

function getBeneficio() {
    var _aseguradora = $("#carta_codigocompaniaseguro").val();
    var _producto = $("#carta_SitedCodigoProducto").val();
    if (_aseguradora == 37) {
        $.ajax({
            url: "/CartaGarantia/getBeneficio",
            data: { idaseguradora: _aseguradora, idproducto: _producto },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al verificar Aseguradora y el Producto', 'Error', opts);
            },
            success: function (data) {
                var items = "";
                items += "<option value='' >- Seleccione -</option>";
                $.each(data, function (i, item) {
                    items += "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
                });
                $("#carta_Sunasa_CoberturaId").html(items);
            }
        });
    }
}

function verificarDetallesAseguradora() {
    var _aseguradora = $("#carta_codigocompaniaseguro").val();
    if (_aseguradora != "") {
        $.ajax({
            url: "/CartaGarantia/VerificarAseguradora",
            data: { idaseguradora: _aseguradora },
            dataType: "json",
            type: "POST",
            error: function () {
                toastr.error('Error al verificar Aseguradora', 'Error', opts);
            },
            success: function (data) {
                $("#carta_SitedCodigoProducto").html("");
                $("#carta_Sunasa_CoberturaId").html("");
                //producto
                if (data.producto) {
                    $("#div_producto").show();
                    var items = "";
                    items += "<option value='' >- Seleccione -</option>";
                    $.each(data.list_producto, function (i, item) {
                        items += "<option value=\"" + item.Value + "\">" + item.Text + "</option>";
                    });
                    $("#carta_SitedCodigoProducto").html(items);
                    $("#carta_SitedCodigoProducto").val("");

                }
                else
                    $("#div_producto").hide();

               
            }
        });
    } else {
        $("#div_producto").hide();
        //$("#div_beneficio").hide();
    }
}

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
    var _aseguradora = $("#carta_codigocompaniaseguro").val();
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
            if ($("#codigopaciente").val() > 0) {
                $("#carta_codigopaciente").val($("#codigopaciente").val());
                $("#paciente").val($("#apellidos").val() + ", " + $("#nombres").val());
                $("#btnupdatePaciente").show();
                $("#btnaddPaciente").hide();
            } else {
                $("#carta_codigopaciente").val("");
                $("#paciente").val("");
                $("#btnupdatePaciente").hide();
                $("#btnaddPaciente").show();
            }
        }
        //MEDICO
        if (tipo_modal_add == 2) {
            if ($("#cmp").val() > 0) {
                $("#carta_cmp").val($("#cmp").val());
                $("#medico").val($("#apellidos").val() + ", " + $("#nombres").val());
                $("#btnupdateMedico").show();
                $("#btnaddMedico").hide();
            } else {
                $("#carta_cmp").val("");
                $("#medico").val("");
                $("#btnupdateMedico").hide();
                $("#btnaddMedico").show();
            }
        }
        //CIE
        if (tipo_modal_add == 3) {
        }
    })
}

function cerrarModalSearch() {
    $('#modal-search').on('hide.bs.modal', function (e) {
        //PACIENTE
        if (tipo_modal_search == 1) {
            if ($("#result_idpaciente").val() != "" && $("#result_nombres").val() != "") {
                $("#carta_codigopaciente").val($("#result_idpaciente").val());
                $("#paciente").val($("#result_nombres").val());
                $("#btnupdatePaciente").show();
                $("#btnaddPaciente").hide();
                getCartasPaciente($("#result_idpaciente").val());
            } else {
                $("#carta_codigopaciente").val("");
                $("#paciente").val("");
                $("#btnupdatePaciente").hide();
                $("#btnaddPaciente").show();
            }
        }
        //MEDICO
        if (tipo_modal_search == 2) {
            if ($("#result_cmp").val() != "" && $("#result_nombres").val() != "") {
                $("#carta_cmp").val($("#result_cmp").val());
                $("#medico").val($("#result_nombres").val());
                $("#btnupdateMedico").show();
                $("#btnaddMedico").hide();
            } else {
                $("#carta_cmp").val("");
                $("#medico").val("");
                $("#btnupdateMedico").hide();
                $("#btnaddMedico").show();
            }
        }
        //CIE
        if (tipo_modal_search == 3) {
            if ($("#result_cie").val() != "" && $("#result_nombres").val() != "") {
                $("#carta_cie").val($("#result_cie").val());
                $("#cie").val($("#result_nombres").val());
            } else {
                $("#carta_cie").val("");
                $("#cie").val("");
            }
        }
        //BENEFICIO
        if (tipo_modal_search == 4) {
            if ($("#result_beneficio").val() != "" && $("#result_nombres").val() != "") {
                $("#carta_Sunasa_CoberturaId").val($("#result_beneficio").val());
                $("#beneficio").val($("#result_nombres").val());
            } else {
                $("#carta_Sunasa_CoberturaId").val("");
                $("#beneficio").val("");
            }
        }
    })
}

function getCartasPaciente(idpaciente) {
    if (idpaciente != "") {
        $("#content_cartas_old").show();
        $("#div_list_cartas").load("/CartaGarantia/getListCarta?idpaciente="+idpaciente, function (response, status, xhr) {
            if (status == "error") {
                toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
            } 
        });
    }
}

//PACIENTE
function deletePaciente_modal() {
    $("#carta_codigopaciente").val("");
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
    var _codigopaciente = $("#carta_codigopaciente").val().trim();
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
    $("#carta_cmp").val("");
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
    var _cmp = $("#carta_cmp").val().trim();

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
//CIE
function searchCIE_modal() {
    $("#modal-search_body").load("/Aseguradora/SearchCie", function (response, status, xhr) {
        if (status == "error") {
            toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
        } else {
            tipo_modal_search = 3;//tipo modal buscar cie
            $("#modal-search").modal('show');
        }
    });
}

//BENEFICIO
function searchBeneficio_modal() {
    var _aseguradora = $("#carta_codigocompaniaseguro").val();
    var _producto = 0;
    if (_aseguradora == 37) {
        _producto = $("#carta_SitedCodigoProducto").val();
    }
    if (_aseguradora != "") {
        $("#modal-search_body").load("/Aseguradora/SearchBenefico?idaseguradora=" + _aseguradora + "&idproducto=" + _producto, function (response, status, xhr) {
            if (status == "error") {
                toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
            } else {
                tipo_modal_search = 4;//tipo modal buscar cie
                $("#modal-search").modal('show');
            }
        });
    }else
        toastr.error('Verifique la Aseguradora y el tipo de Producto', 'Error', opts);
}

function validarCartaC() {

    if (validar_carta_create())
        return true;
    return false;
}

function validar_carta_create() {
    var result = true;

    $("#div_carta input.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val().trim() == "") {
            div_cont.addClass("has-error");
            if ($(this).attr("id") == "estudios") {
                $(".t1").parents(".form-group").first().addClass("has-error");
            }
        }
    });

    $("#div_carta select.validar").each(function () {
        var div_cont = $(this).parents(".form-group").first();
        div_cont.removeClass("has-error");
        div_cont.removeClass("has-warning");
        if ($(this).val() == "") {
            div_cont.addClass("has-error");
        }
    });

    var _estado = $("#carta_estadocarta").val();
    if (_estado == "APROBADA") {
        if ($("#carta_numerocarnetseguro").val() == "")
            $("#carta_numerocarnetseguro").parents(".form-group").first().addClass("has-error");
        if ($("#div_producto").is(":visible"))
            if ($("#carta_SitedCodigoProducto").val() == "")
                $("#carta_SitedCodigoProducto").parents(".form-group").first().addClass("has-error");
        if ($("#carta_Sunasa_CoberturaId").val() == "")
            $("#carta_Sunasa_CoberturaId").parents(".form-group").first().addClass("has-error");
        if ($("#carta_cie").val() == "")
            $("#carta_cie").parents(".form-group").first().addClass("has-error");
        if ($("#carta_cobertura").val() < 1 || $("#carta_cobertura").val() > 100)
            $("#carta_cobertura").parents(".form-group").first().addClass("has-error");
    }

    $(".has-error").each(function () {
        result = false;
    });
    $(".has-warning").each(function () {
        result = false;
    });
    if (!result)
        toastr.error("Verifique los datos", "Error", opts);
    else
        result = validarCodigoCarta();

    return result;
}
function validarCodigoCarta() {
    var _result = true;
    $.ajax({
        async: false,
        cache: false,
        url: '/CartaGarantia/validarCodigoCarta',
        data: { codigo: $("#carta_codigocartagarantia").val() },
        dataType: "json",
        type: "POST",
        success: function (data) {
            if (!data) {
                $("#carta_codigocartagarantia").parents(".form-group").first().addClass("has-error");
                toastr.error("El N° de Carta ya esta registrado", "Error", opts);
                _result = false;
            }
        },
        error: function () {
            $("#carta_codigocartagarantia").parents(".form-group").first().addClass("has-error");
            toastr.error("El N° de Carta ya esta registrado", "Error", opts);
            _result = false;
        }
    });
    return _result;
}
