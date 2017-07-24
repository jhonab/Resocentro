$(document).ready(function () {
    $("#msj").fadeOut()//ocultar el msj
    ocultarListas();
});

function ocultarListas() {
    $("#div_sede").hide();
    $("#div_permiso").hide();
}

function mostrarListas() {
    var user = $("#idusuario").val();
    var btn = ($("#btncargarUser").html());

    if (btn.indexOf('Buscar') !== -1) {
        if (user != "") {
            $("#idusuario").prop("disabled", true)
            $("#btncargarUser").html("<i class='fa fa-search'></i>&nbsp;&nbsp;Nueva Busqueda");
            $("#div_sede").show();
            $("#div_permiso").show();
        }
        
    }
    else {
        $("#idusuario").prop("disabled", false)
        $("#btncargarUser").html("<i class='fa fa-search'></i>&nbsp;&nbsp;Buscar");
        $("#idusuario").val("");
        ocultarListas();
    }
}

