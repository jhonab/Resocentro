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
    
    public partial class INTEGRACION
    {
        public int numero_estudio { get; set; }
        public string id_paciente { get; set; }
        public string apellidos { get; set; }
        public string nombres { get; set; }
        public System.DateTime fecha_nacimiento { get; set; }
        public string edad { get; set; }
        public string sexo { get; set; }
        public string cmp_medico_referente { get; set; }
        public System.DateTime hora_estudio { get; set; }
        public string codigo_procedimiento { get; set; }
        public string nombre_procedimiento { get; set; }
        public string tipo_modalidad { get; set; }
        public string modalidad { get; set; }
        public int estado { get; set; }
        public int peso { get; set; }
        public Nullable<int> numeroatencion { get; set; }
        public string uid { get; set; }
        public string departamento { get; set; }
        public string series_date { get; set; }
        public string series_time { get; set; }
        public string series_description { get; set; }
        public string study_description { get; set; }
        public string series_body_part { get; set; }
        public string series_images { get; set; }
        public string series_number { get; set; }
        public string station_name { get; set; }
        public int counter { get; set; }
    }
}
