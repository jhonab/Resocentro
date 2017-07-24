var isCorrecto = false;
var isCorrectoDNI = false;
var isUpdate = false;

$(function () {
    $('.datetime').datetimepicker({
        locale: 'es',
        format: 'DD/MM/YYYY',
        maxDate: new Date()
    });

    $("#emp_nombres").change(procesarNombres);
    $("#emp_apellidos").change(procesarNombres);
    $("#usu_siglas").change(validarusuario);
    $("#usu_codigousuario").change(validarusuario);
    $("#emp_dni").change(validarDNI);

    emailEmpresa();

    if (!isUpdate && $("#emp_dni").val() != "")
        validarDNI();
    else
        isCorrectoDNI = true;

    if (!isUpdate && $("#usu_siglas").val() != "")
        validarusuario();
    else
        isCorrecto = true;

    if (!isUpdate && $("#usu_codigousuario").val() != "")
        validarusuario();
    else
        isCorrecto = true;

    $("#usu_ShortName").autocomplete({
        source: function (request, callback) {
            var lst = ListSugerencia();
            callback(lst);
        }
    }).focus(function () {
        $("#usu_ShortName").autocomplete("search", " ");
    })
});
function validarDNI() {
    var result;
    var _dni = $("#emp_dni").val();
    removeHasAll("emp_dni");
    if (_dni != "")
        $.ajax({
            async: false,
            cache: false,
            url: '/Empleado/validarDNI',
            data: { dni: _dni },
            dataType: "json",
            type: "POST",
            success: function (data) {
                if (data)
                    addSuccess("emp_dni");
                else
                    addWarning("emp_dni");
                result = result && data;
            },
            error: function () {
                toastr.error("Ocurrio un error y no se pudo validar el DNI", "Error", opts);
            }
        });
    isCorrectoDNI = result;
}

function validar() {
    return (validar_form("div_empleado") && isCorrecto && isCorrectoDNI);
}

function procesarNombres() {
    if (!isUpdate)
        createusuario();
    emailEmpresa();
}

function emailEmpresa() {

    if ($("#emp_email").val() == "") {
        var _nombres = "", _apellidos = "";
        if ($("#emp_nombres").val() != "")
            _nombres = $("#emp_nombres").val().split(" ")[0];
        if ($("#emp_apellidos").val() != "")
            _apellidos = $("#emp_apellidos").val().split(" ")[0];

        $("#emp_email").val(_nombres.toLowerCase() + "." + _apellidos.toLowerCase() + "@resocentro.com");
    }
}
function createusuario() {
    var _nombres = "", _apellidos = "", _siglas = "";
    var random = Math.floor(Math.random() * 1000);
    if ($("#emp_nombres").val() != "")
        _nombres = $("#emp_nombres").val().split(" ");
    if ($("#emp_apellidos").val() != "")
        _apellidos = $("#emp_apellidos").val().split(" ");
    if (_nombres != "")
        for (var i = 0; i < _nombres.length; i++) {
            _siglas += _nombres[i].substring(0, 1);
        }
    if (_apellidos != "")
        for (var i = 0; i < _apellidos.length; i++) {
            _siglas += _apellidos[i].substring(0, 1);
        }

    if (_siglas != "") {
        $("#usu_siglas").val(_siglas.toUpperCase());
        $("#usu_codigousuario").val(_siglas.toUpperCase() + random);

    }
    if (_nombres != "" && _apellidos != "") {
        validarusuario();
        $("#usu_ShortName").val(_nombres[0] + " " + _apellidos[0]);

    }

}
function ListSugerencia() {
    var sugerencias = [];
    var _nombres = $("#emp_nombres").val().split(" ");
    var _apellidos = $("#emp_apellidos").val().split(" ");
    for (var i = 0; i < _nombres.length; i++) {
        for (var y = 0; y < _apellidos.length; y++) {
            sugerencias.push(_nombres[i] + " " + _apellidos[y]);
        }
    }

    return sugerencias;
}
function validarusuario() {
    var _siglas = $("#usu_siglas").val();
    var _clave = $("#usu_codigousuario").val();
    var result = true;
    removeHasAll("usu_siglas");
    removeHasAll("usu_codigousuario");
    if (_siglas != "")
        $.ajax({
            async: false,
            cache: false,
            url: '/Empleado/validarSiglas',
            data: { siglas: _siglas },
            dataType: "json",
            type: "POST",
            success: function (data) {
                if (data)
                    addSuccess("usu_siglas");
                else
                    addWarning("usu_siglas");
                result = result && data;
            },
            error: function () {
                toastr.error("Ocurrio un error y no se pudo validar el Usuario", "Error", opts);
            }
        });
    if (_clave != "")
        $.ajax({
            async: false,
            cache: false,
            url: '/Empleado/validarClave',
            data: { clave: _clave },
            dataType: "json",
            type: "POST",
            success: function (data) {
                if (data)
                    addSuccess("usu_codigousuario");
                else
                    addWarning("usu_codigousuario");
                result = result && data;
            },
            error: function () {
                toastr.error("Ocurrio un error y no se pudo validar la Clave", "Error", opts);
            }
        });
    isCorrecto = result;
}