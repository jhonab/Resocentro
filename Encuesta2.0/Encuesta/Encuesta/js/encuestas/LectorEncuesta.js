$(function () {
   // activarModoLector(true, true);
    $(".main-content ").addClass("main-medico");
    $(".page-container ").addClass("sidebar-collapsed");
    //$("#div_PreEntrevista ").addClass("body-medico");
    $("#div-resumen ").addClass("row");
    $("#div-detallado ").addClass("row");
    $("#modelEncuesta_btnEncuesta").hide();
    $("textarea").attr("disabled", true);
    $("select").attr('disabled', 'true').css({ "background-color": "#eee" });
    $("input[type='text']").attr('disabled', 'true').css({ "background-color": "#eee" });
    $("input[type='number']").attr('disabled', 'true').css({ "background-color": "#eee" });
    $("input[type='radio']").addClass("disabled");
    $("#txt_esecifique").attr("disabled", false);
    $("#apellido_copy").attr("disabled", false);
   
});
jQuery(window).load(function () {
    Mostrardetalle(true);
});
function Mostrardetalle(first) {
//    $("#div-detallado").toggle();
    if ($("#div-detallado").hasClass('hidden')) {
        $("#div-detallado ").removeClass("hidden");
    }
    else{
        $("#div-detallado ").addClass("hidden");
    }
    
    //$("[data-widget='collapse']").each(function () {
    //    var box = $(this).parents(".box").first();
    //    if ((box.attr('id') != "Info-examen" && box.attr('id') != "calificacion-examen") && first == false) {
    //        //Find the body and the footer
    //        var bf = box.find(".box-body, .box-footer");
    //        if (!box.hasClass("collapsed-box")) {
    //            box.addClass("collapsed-box");
    //            //Convert minus into plus
    //            $(this).children(".fa-minus").removeClass("fa-minus").addClass("fa-plus");
    //            bf.slideUp();
    //            $("#btn-detalle").html('<i class="fa fa-minus fa-1x"></i>&nbsp;Detalle Encuesta')
    //        } else {
    //            box.removeClass("collapsed-box");
    //            //Convert plus into minus
    //            $(this).children(".fa-plus").removeClass("fa-plus").addClass("fa-minus");
    //            bf.slideDown();
    //            $("#btn-detalle").html('<i class="fa fa-plus fa-1x"></i>&nbsp;Detalle Encuesta')
    //        }
    //    }
    //});
}
function activarModoLector(hidenav, skinblack) {
    
    if (skinblack) {
        $("body").removeClass("skin-blue skin-black");
        $("body").addClass("skin-black");
    } else {
        $("body").removeClass("skin-blue skin-black");
        $("body").addClass("skin-blue");
        
    }
    if (hidenav) {
        $('.left-side').toggleClass("collapse-left");
        $(".right-side").toggleClass("strech");
    }
}