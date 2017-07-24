function mostrarmensaje(tipo, msj) {
    if (tipo == 1) {//success
        toastr.success(msj,"", opts);
    }
    else if (tipo == 2) {//info
        toastr.info(msj, "", opts);
    }
    else if (tipo == 3) {//warning
        toastr.warning(msj, "", opts);
    }
    else if (tipo == 4) {//danger
        toastr.error(msj, "", opts);
    }
    else { }
}