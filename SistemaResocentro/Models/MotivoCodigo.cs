//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SistemaResocentro.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MotivoCodigo
    {
        public MotivoCodigo()
        {
            this.CorrelativoMotivo = new HashSet<CorrelativoMotivo>();
            this.HistorialCorrelativo = new HashSet<HistorialCorrelativo>();
        }
    
        public int idMotivo { get; set; }
        public string descripcion { get; set; }
        public string userRegister { get; set; }
        public System.DateTime fecha { get; set; }
        public string nomenclatura { get; set; }
        public Nullable<System.DateTime> lastUpdate { get; set; }
        public string user_lastUpdate { get; set; }
    
        public virtual ICollection<CorrelativoMotivo> CorrelativoMotivo { get; set; }
        public virtual ICollection<HistorialCorrelativo> HistorialCorrelativo { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}
