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
    
    public partial class CLINICAHOSPITAL
    {
        public CLINICAHOSPITAL()
        {
            this.CITA = new HashSet<CITA>();
            this.Institucion_Promotor = new HashSet<Institucion_Promotor>();
            this.Institucion_Medico = new HashSet<Institucion_Medico>();
            this.Planificacion = new HashSet<Planificacion>();
        }
    
        public int codigoclinica { get; set; }
        public string razonsocial { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string web { get; set; }
        public int codigozona { get; set; }
        public string tipo { get; set; }
        public Nullable<int> IdIpress { get; set; }
        public string CodigoIpress { get; set; }
        public string UbigeoId { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string rucClinica { get; set; }
        public Nullable<System.DateTime> AuFecNew { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public Nullable<System.DateTime> AuFecUpd { get; set; }
        public string CodigoIpress2 { get; set; }
    
        public virtual ICollection<CITA> CITA { get; set; }
        public virtual ICollection<Institucion_Promotor> Institucion_Promotor { get; set; }
        public virtual ICollection<Institucion_Medico> Institucion_Medico { get; set; }
        public virtual ICollection<Planificacion> Planificacion { get; set; }
    }
}
