function getPaciente() {
    var apellidos = $("#apellidos").val().trim();
    apellidos=apellidos.replace(/\s/g,"%20");
    if (apellidos != "") {
        if (apellidos.length > 5) {
            
            $("#table_paciente").html('<div class="row">' +
					'<div class="pace pace-active"><div class="pace-progress" data-progress="50" data-progress-text="50%" style="-webkit-transform: translate3d(50%, 0px, 0px); -ms-transform: translate3d(50%, 0px, 0px); transform: translate3d(50%, 0px, 0px);"><div class="pace-progress-inner"></div></div><div class="pace-activity"></div></div>' +
				'</div>');
            $("#table_paciente").load("/Paciente/ListaPaciente?apellido=" + apellidos, function (response, status, xhr) {
                if (status == "error") {
                    toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
                } 
            });
        } else
            toastr.warning("Ingrese más datos para realizar la busqueda", "Advertencia", opts);
    }else
        toastr.error("Ingrese los apellidos del paciente", "Error", opts);
}

function selectPaciente(idpaciente, nombres) {
    $("#result_idpaciente").val(idpaciente);
    $("#result_nombres").val(nombres);
    $("#modal-search").modal('hide');
    
}