$(document).ready(function () {
    $("#msj").hide()//ocultar el msj
    ocultarListas();
    $("#lsttipo").change(cargarMedico);
});

function ocultarListas() {
    $("#div_partial_encuesta").hide();
    $("#div_partial_Cambio").hide(); 
    $("#div_datos").hide();
}

function mostrarListas() {
         var btn = ($("#btncargarEncuesta").html());
        if (btn.indexOf('Buscar') !== -1) {
            
                $("#num_exam").prop("disabled", true)
                $("#btncargarEncuesta").html("<i class='fa fa-search'></i>&nbsp;&nbsp;Nueva Busqueda");
                $("#div_partial_encuesta").show();
                $("#div_partial_Cambio").show();
                $("#div_datos").show();
           
        }
        else {
            $("#num_exam").prop("disabled", false)
            $("#btncargarEncuesta").html("<i class='fa fa-search'></i>&nbsp;&nbsp;Buscar");
            $("#num_exam").val("");
            $("#partial_cambio").html("");
            $("#partial_encuesta").html("");
            $("#lsttipo").val("0");
            $("#lstmedico").html('<option value>- Seleccione el tipo -</option>');
            ocultarListas();
        }

}

