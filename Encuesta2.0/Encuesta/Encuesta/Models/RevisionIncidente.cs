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
    
    public partial class RevisionIncidente
    {
        public RevisionIncidente()
        {
            this.PiezasRevision = new HashSet<PiezasRevision>();
        }
    
        public int idRevision { get; set; }
        public int idIncidente { get; set; }
        public System.DateTime fecha_revision { get; set; }
        public string user_revision { get; set; }
        public bool isCambioPiezas { get; set; }
        public string revision { get; set; }
        public string tecnico { get; set; }
        public string estadoequipo { get; set; }
    
        public virtual IncidenteEquipo IncidenteEquipo { get; set; }
        public virtual ICollection<PiezasRevision> PiezasRevision { get; set; }
    }
}
