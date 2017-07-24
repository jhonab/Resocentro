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
    
    public partial class IncidenteEquipo
    {
        public IncidenteEquipo()
        {
            this.RevisionIncidente = new HashSet<RevisionIncidente>();
            this.ConversacionIncidente = new HashSet<ConversacionIncidente>();
        }
    
        public int idIncidente { get; set; }
        public int idEquipo { get; set; }
        public int idTipoIncidente { get; set; }
        public string user_detecta { get; set; }
        public System.DateTime fecha_detecta { get; set; }
        public string falla_detecta { get; set; }
        public bool isSolucionado { get; set; }
        public Nullable<System.DateTime> fecha_solucionado { get; set; }
        public string telefono { get; set; }
        public string estadoequipo { get; set; }
        public Nullable<int> numero_informe { get; set; }
    
        public virtual EQUIPO EQUIPO { get; set; }
        public virtual TipoIncidente TipoIncidente { get; set; }
        public virtual ICollection<RevisionIncidente> RevisionIncidente { get; set; }
        public virtual ICollection<ConversacionIncidente> ConversacionIncidente { get; set; }
    }
}
