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
    
    public partial class Encuesta
    {
        public int idencuesta { get; set; }
        public int numeroexamen { get; set; }
        public System.DateTime fec_ini_encu { get; set; }
        public Nullable<System.DateTime> fec_paso1 { get; set; }
        public Nullable<System.DateTime> fec_paso2 { get; set; }
        public Nullable<int> tipo_encu { get; set; }
        public Nullable<System.DateTime> fec_paso3 { get; set; }
        public string usu_reg_encu { get; set; }
        public Nullable<System.DateTime> fec_ini_supervisa { get; set; }
        public Nullable<System.DateTime> fec_supervisa { get; set; }
        public string usu_reg_super { get; set; }
        public Nullable<System.DateTime> fec_ini_tecno { get; set; }
        public Nullable<System.DateTime> fec_fin_tecno { get; set; }
        public string usu_reg_tecno { get; set; }
        public Nullable<System.DateTime> fec_ini_proto { get; set; }
        public Nullable<System.DateTime> fec_fin_proto { get; set; }
        public string usu_reg_proto { get; set; }
        public Nullable<int> estado { get; set; }
        public Nullable<int> atencion { get; set; }
        public Nullable<bool> SolicitarValidacion { get; set; }
        public Nullable<System.DateTime> fec_Solicitavalidacion { get; set; }
        public string usu_Solicitavalidacion { get; set; }
    }
}
