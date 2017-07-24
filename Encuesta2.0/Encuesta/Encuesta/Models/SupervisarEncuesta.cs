//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Encuesta.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupervisarEncuesta
    {
        public int idsupervisar { get; set; }
        public Nullable<int> tipo_encuesta { get; set; }
        public Nullable<int> numeroexamen { get; set; }
        public Nullable<bool> p1 { get; set; }
        public string p1_1 { get; set; }
        public Nullable<bool> p2 { get; set; }
        public string p2_1 { get; set; }
        public Nullable<bool> p3 { get; set; }
        public string p3_1 { get; set; }
        public Nullable<int> p4 { get; set; }
        public string p4_1 { get; set; }
        public string anotaciones { get; set; }
        public string usu_reg { get; set; }
        public Nullable<System.DateTime> fecha_reg { get; set; }
        public Nullable<System.DateTime> fecha_termino { get; set; }
        public Nullable<bool> superviso_encuesta { get; set; }
        public Nullable<bool> superviso_tecnologo { get; set; }
        public Nullable<int> estado { get; set; }
        public Nullable<bool> superviso_imagenes { get; set; }
        public Nullable<bool> isInEcnuesta { get; set; }
        public Nullable<bool> isInImagenes { get; set; }
        public Nullable<bool> p1_informante { get; set; }
        public Nullable<bool> p2_informante { get; set; }
        public Nullable<bool> p3_informante { get; set; }
        public Nullable<int> p4_informante { get; set; }
        public string p1_1_informante { get; set; }
        public string p2_1_informante { get; set; }
        public string p3_1_informante { get; set; }
        public string p4_1_informante { get; set; }
        public Nullable<System.DateTime> fec_reg_inf { get; set; }
        public string usu_reg_inf { get; set; }
        public Nullable<bool> p1_validador { get; set; }
        public Nullable<bool> p2_validador { get; set; }
        public Nullable<bool> p3_validador { get; set; }
        public Nullable<int> p4_ivalidador { get; set; }
        public string p1_1_validador { get; set; }
        public string p2_1_validador { get; set; }
        public string p3_1_validador { get; set; }
        public string p4_1_validador { get; set; }
        public Nullable<System.DateTime> fec_reg_val { get; set; }
        public string usu_reg_val { get; set; }
        public string diagnostico_inf { get; set; }
        public Nullable<bool> diagnostico_info { get; set; }
        public string diagnostico_val { get; set; }
        public Nullable<bool> diagnostico_vali { get; set; }
        public Nullable<bool> p5 { get; set; }
        public string p5_1 { get; set; }
        public Nullable<bool> p5_informante { get; set; }
        public string p5_1_informante { get; set; }
        public Nullable<bool> p5_validador { get; set; }
        public string p5_1_validador { get; set; }
        public bool isSendEmailProcessSuperivor { get; set; }
        public bool isSendEmailProcessInformante { get; set; }
        public bool isSendEmailProcessValidador { get; set; }
        public string motivo_aplicacion_contraste { get; set; }
    
        public virtual USUARIO Informante { get; set; }
        public virtual USUARIO Supervisor { get; set; }
        public virtual USUARIO Validador { get; set; }
    }
}
