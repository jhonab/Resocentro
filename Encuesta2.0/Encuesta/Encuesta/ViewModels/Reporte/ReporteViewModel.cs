using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Reporte
{
    public class ReporteViewModel
    {
        public string rangoFiltro { get; set; }
        public List<Data_reporte> encuestador { get; set; }
        public List<Data_calificacion> cali_encuestador { get; set; }
        public List<Data_reporte> supervisor { get; set; }
        public List<Data_reporte> tecnologo { get; set; }
        public List<Data_reporte> postproceso { get; set; }
        public List<Data_reporte> informante { get; set; }
        public List<int> contador_inf { get; set; }
        public List<int> contador_val { get; set; }
        public bool isRevisado { get; set; }



        public List<Data_reporte> enfermera { get; set; }
    }

    public class Data_reporte
    {
        public DateTime fecha { get; set; }
        public int examen { get; set; }
        public string paciente { get; set; }
        public string estudio { get; set; }
        public string equipo { get; set; }
        public int placas { get; set; }
        public bool imagenes { get; set; }
        public bool postproceso { get; set; }
        public bool sedacion { get; set; }
        public bool contraste { get; set; }
        public bool continuo { get; set; }
        public string encuestador { get; set; }
        public string supervisor { get; set; }
        public string tecnologo { get; set; }
        public string sedadora { get; set; }
        public string enfermera { get; set; }
        public DateTime encu_ini { get; set; }
        public DateTime encu_fin { get; set; }
        public DateTime ini_super { get; set; }
        public DateTime soli_ini { get; set; }
        public DateTime tec_ini { get; set; }
        public DateTime tec_fin { get; set; }

        public bool isRevisado { get; set; }

        public string modalidad { get; set; }

        public bool? isError { get; set; }

        public string error { get; set; }

        public bool? isError_p2 { get; set; }

        public string Error_p2 { get; set; }

        public bool? isError_p3 { get; set; }

        public string Error_p3 { get; set; }

        public bool? isError_p4 { get; set; }

        public string Error_p4 { get; set; }

        public string med_Calificador { get; set; }



        public string tecnica { get; set; }

        public int valorPro { get; set; }

        public string insumos { get; set; }

        public string tecno { get; set; }

        public string nombreinsumos { get; set; }

        public string claseestudio { get; set; }
    }

    public class Data_calificacion
    {

        public DateTime? fecha { get; set; }

        public int? examen { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public string supervisor { get; set; }

        public string error { get; set; }

        public bool? isError { get; set; }
    }

    public class DataReporteContraste
    {
        public List<Reporte_Contraste> detalle { get; set; }
        public List<Resumen_Reporte_Contraste> resumen { get; set; }
    }

    public class DataReporteFrasco
    {
        public List<Reporte_Frasco> frasco { get; set; }
    }

    public class Reporte_Frasco
    {
        public int frasco { get; set; }

        public double inicial { get; set; }

        public double usado { get; set; }

        public double final { get; set; }

        public string nombre { get; set; }
    }

    public class DataReporteMerma
    {
        public List<Reporte_Merma> merma { get; set; }
    }

    public class Reporte_Merma
    {
        public int frasco { get; set; }

        public double inicial { get; set; }

        public double usado { get; set; }

        public double final { get; set; }

        public string fecha { get; set; }

        public string nombre { get; set; }
    }

    public class DataReporteMensualContraste
    {
        public List<Reporte_Mensual_Contraste> mensual { get; set; }
    }

    public class Reporte_Mensual_Contraste
    {

        public string nom_contraste { get; set; }
        public string mes { get; set; }
        public int dia1 { get; set; }
        public int dia2 { get; set; }
        public int dia3 { get; set; }
        public int dia4 { get; set; }
        public int dia5 { get; set; }
        public int dia6 { get; set; }
        public int dia7 { get; set; }
        public int dia8 { get; set; }
        public int dia9 { get; set; }
        public int dia10 { get; set; }
        public int dia11 { get; set; }
        public int dia12 { get; set; }
        public int dia13 { get; set; }
        public int dia14 { get; set; }
        public int dia15 { get; set; }
        public int dia16 { get; set; }
        public int dia17 { get; set; }
        public int dia18 { get; set; }
        public int dia19 { get; set; }
        public int dia20 { get; set; }
        public int dia21 { get; set; }
        public int dia22 { get; set; }
        public int dia23 { get; set; }
        public int dia24 { get; set; }
        public int dia25 { get; set; }
        public int dia26 { get; set; }
        public int dia27 { get; set; }
        public int dia28 { get; set; }
        public int dia29 { get; set; }
        public int dia30 { get; set; }
        public int dia31 { get; set; }
        public int total { get; set; }
    }

    public class DataReporteMensualFContraste
    {
        public List<Reporte_Mensual_F_Contraste> mensual { get; set; }
    }

    public class Reporte_Mensual_F_Contraste
    {

        public string nom_contraste { get; set; }
        public string mes { get; set; }
        public double dia1 { get; set; }
        public double dia2 { get; set; }
        public double dia3 { get; set; }
        public double dia4 { get; set; }
        public double dia5 { get; set; }
        public double dia6 { get; set; }
        public double dia7 { get; set; }
        public double dia8 { get; set; }
        public double dia9 { get; set; }
        public double dia10 { get; set; }
        public double dia11 { get; set; }
        public double dia12 { get; set; }
        public double dia13 { get; set; }
        public double dia14 { get; set; }
        public double dia15 { get; set; }
        public double dia16 { get; set; }
        public double dia17 { get; set; }
        public double dia18 { get; set; }
        public double dia19 { get; set; }
        public double dia20 { get; set; }
        public double dia21 { get; set; }
        public double dia22 { get; set; }
        public double dia23 { get; set; }
        public double dia24 { get; set; }
        public double dia25 { get; set; }
        public double dia26 { get; set; }
        public double dia27 { get; set; }
        public double dia28 { get; set; }
        public double dia29 { get; set; }
        public double dia30 { get; set; }
        public double dia31 { get; set; }

        public object total { get { return dia1 + dia2 + dia3 + dia4 + dia5 + dia6 + dia7 + dia8 + dia9 + dia10 + dia11 + dia12 + dia13 + dia14 + dia15 + dia16 + dia17 + dia18 + dia19 + dia20 + dia21 + dia22 + dia23 + dia24 + dia25 + dia26 + dia27 + dia28 + dia29 + dia30 + dia31; } }
    }

    public class DataReporteMensualMLContraste
    {
        public List<Reporte_Mensual_ML_Contraste> mensual { get; set; }
    }

    public class Reporte_Mensual_ML_Contraste
    {

        public string nom_contraste { get; set; }
        public string mes { get; set; }
        public double dia1 { get; set; }
        public double dia2 { get; set; }
        public double dia3 { get; set; }
        public double dia4 { get; set; }
        public double dia5 { get; set; }
        public double dia6 { get; set; }
        public double dia7 { get; set; }
        public double dia8 { get; set; }
        public double dia9 { get; set; }
        public double dia10 { get; set; }
        public double dia11 { get; set; }
        public double dia12 { get; set; }
        public double dia13 { get; set; }
        public double dia14 { get; set; }
        public double dia15 { get; set; }
        public double dia16 { get; set; }
        public double dia17 { get; set; }
        public double dia18 { get; set; }
        public double dia19 { get; set; }
        public double dia20 { get; set; }
        public double dia21 { get; set; }
        public double dia22 { get; set; }
        public double dia23 { get; set; }
        public double dia24 { get; set; }
        public double dia25 { get; set; }
        public double dia26 { get; set; }
        public double dia27 { get; set; }
        public double dia28 { get; set; }
        public double dia29 { get; set; }
        public double dia30 { get; set; }
        public double dia31 { get; set; }

        public double total { get { return dia1 + dia2 + dia3 + dia4 + dia5 + dia6 + dia7 + dia8 + dia9 + dia10 + dia11 + dia12 + dia13 + dia14 + dia15 + dia16 + dia17 + dia18 + dia19 + dia20 + dia21 + dia22 + dia23 + dia24 + dia25 + dia26 + dia27 + dia28 + dia29 + dia30 + dia31;} }
    }

    public class Reporte_Contraste
    {

        public DateTime fecha { get; set; }

        public string fechaB { get; set; }

        public string sede { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string nombreestudio { get; set; }

        public string estadoestudio { get; set; }

        public int id_insumo { get; set; }
        public string producto { get; set; }

        public int cantidad { get; set; }
        public double usado { get; set; }

        public string aplicado { get; set; }

        public string lote { get; set; }

        public int edad { get; set; }

        public int peso { get; set; }

        public string equipo { get; set; }

        public string tecnologo { get; set; }

        public string enfermera { get; set; }

        public string tipo_aplicacion { get; set; }

        public DateTime fec_aplica { get; set; }

        public double merma { get; set; }
    }

    public class Resumen_Reporte_Contraste
    {
        public string insumo { get; set; }

        public double frascoSalida { get; set; }

        public double umedidaSalida { get; set; }

        public double frascoUsado { get; set; }

        public double umedidaUsado { get; set; }

        public double frascoSaldo { get; set; }

        public double umedidaSaldo { get; set; }
    }

    public class Reporte_ContrasteZ
    {

        public DateTime fecha { get; set; }

        public string sede { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string nombreestudio { get; set; }

        public string estadoestudio { get; set; }

        public int id_insumo { get; set; }
        public string producto { get; set; }

        public int cantidad { get; set; }

        public string aplicado { get; set; }

        public string lote { get; set; }

        public int edad { get; set; }

        public int peso { get; set; }

        public string equipo { get; set; }

        public string tecnologo { get; set; }

        public string enfermera { get; set; }

        public string tipo_aplicacion { get; set; }
    }

    public class Resumen_Reporte_ContrasteZ
    {
        public string insumo { get; set; }

        public double frascoSalida { get; set; }

        public double umedidaSalida { get; set; }

        public double frascoUsado { get; set; }

        public double umedidaUsado { get; set; }

        public double frascoSaldo { get; set; }

        public double umedidaSaldo { get; set; }
    }

    public class DataReporteSedacion
    {
        public List<Reporte_Sedacion> detalle { get; set; }
        public List<Resumen_Reporte_Sedacion> resumen { get; set; }
    }


    public class Reporte_Sedacion
    {

        public DateTime fecha { get; set; }

        public string fechaB { get; set; }

        public string sede { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string nombreestudio { get; set; }

        public string estadoestudio { get; set; }

        public int id_insumo { get; set; }
        public string producto { get; set; }

        public int cantidad { get; set; }

        public string aplicado { get; set; }

        public string lote { get; set; }

        public int edad { get; set; }

        public int peso { get; set; }

        public string equipo { get; set; }

        //public string tecnologo { get; set; }

        public string anestecia { get; set; }

        public string tipo_seda { get; set; }

        public string motivo_seda { get; set; }

        public string otros_seda { get; set; }

        public DateTime fec_aplica { get; set; }
    }

    public class Resumen_Reporte_Sedacion
    {
        public string insumo { get; set; }

        public double frascoSalida { get; set; }

        public double umedidaSalida { get; set; }

        public double frascoUsado { get; set; }

        public double umedidaUsado { get; set; }

        public double frascoSaldo { get; set; }

        public double umedidaSaldo { get; set; }
    }

}