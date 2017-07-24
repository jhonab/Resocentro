function getMedico() {
    var filtro = $("#filtro").val().trim();
    filtro = filtro.replace(" ", "%20");
    if (filtro != "") {
        if (filtro.length > 3) {
            
            $("#table_medico").html('<div class="row">' +
					'<div class="pace pace-active"><div class="pace-progress" data-progress="50" data-progress-text="50%" style="-webkit-transform: translate3d(50%, 0px, 0px); -ms-transform: translate3d(50%, 0px, 0px); transform: translate3d(50%, 0px, 0px);"><div class="pace-progress-inner"></div></div><div class="pace-activity"></div></div>' +
				'</div>');
            $("#table_medico").load("/Medico/ListaMedico?filtro=" + filtro, function (response, status, xhr) {
                if (status == "error") {
                    toastr.error('Error:\n' + xhr.status + " " + xhr.statusText, 'Error', opts);
                } 
            });
        } else
            toastr.warning("Ingrese mas datos para realizar la busqueda", "Advertencia", opts);
    }else
        toastr.error("Ingrese un texto para realizar la busqueda", "Error", opts);
}

function selectMedico(cmp, nombres) {
    $("#result_cmp").val(cmp);
    $("#result_nombres").val(nombres);
    $("#modal-search").modal('hide');
    
}