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
    
    public partial class Contraste
    {
        public Contraste()
        {
            this.Detalle_Contraste = new HashSet<Detalle_Contraste>();
        }
    
        public int idcontraste { get; set; }
        public int numeroexamen { get; set; }
        public int paciente { get; set; }
        public string usuario { get; set; }
        public string tecnologo { get; set; }
        public string tipo_contraste { get; set; }
        public string tipo_aplicacion_contraste { get; set; }
        public string insumo_medico { get; set; }
        public Nullable<int> volumen { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public Nullable<int> estado { get; set; }
        public Nullable<int> atencion { get; set; }
        public int consentimiento { get; set; }
    
        public virtual USUARIO USUARIO1 { get; set; }
        public virtual USUARIO USUARIO2 { get; set; }
        public virtual ICollection<Detalle_Contraste> Detalle_Contraste { get; set; }
    }
}
