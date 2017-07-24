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
    
    public partial class Planificacion_CompaniaSeguro
    {
        public Planificacion_CompaniaSeguro()
        {
            this.Planificacion_CompaniaSeguro_Documento = new HashSet<Planificacion_CompaniaSeguro_Documento>();
        }
    
        public int id { get; set; }
        public int codigocompaniaseguro { get; set; }
        public int idColaborador { get; set; }
        public string codigopromotor { get; set; }
        public Nullable<System.DateTime> checkin { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public System.DateTime fecha { get; set; }
        public string motivo { get; set; }
        public bool isRealizada { get; set; }
        public string comentarios { get; set; }
        public bool isCorrecto { get; set; }
        public string ruc { get; set; }
    
        public virtual Colaborador Colaborador { get; set; }
        public virtual COMPANIASEGURO COMPANIASEGURO { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro_Documento> Planificacion_CompaniaSeguro_Documento { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}
