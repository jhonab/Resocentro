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
    
    public partial class PACIENTE
    {
        public PACIENTE()
        {
            this.ATENCION = new HashSet<ATENCION>();
            this.CARTAGARANTIA = new HashSet<CARTAGARANTIA>();
            this.Encuesta_Satisfaccion = new HashSet<Encuesta_Satisfaccion>();
            this.HONORARIOTECNOLOGO = new HashSet<HONORARIOTECNOLOGO>();
            this.INFORMEMEDICO = new HashSet<INFORMEMEDICO>();
            this.CITA = new HashSet<CITA>();
        }
    
        public string nacionalidad { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public string telefono { get; set; }
        public System.DateTime fechanace { get; set; }
        public string sexo { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string dni { get; set; }
        public int codigopaciente { get; set; }
        public string tipo_doc { get; set; }
        public Nullable<bool> IsProtocolo { get; set; }
        public string UbigeoId { get; set; }
        public Nullable<int> UbigeoPaisId { get; set; }
    
        public virtual ICollection<ATENCION> ATENCION { get; set; }
        public virtual ICollection<CARTAGARANTIA> CARTAGARANTIA { get; set; }
        public virtual ICollection<Encuesta_Satisfaccion> Encuesta_Satisfaccion { get; set; }
        public virtual ICollection<HONORARIOTECNOLOGO> HONORARIOTECNOLOGO { get; set; }
        public virtual ICollection<INFORMEMEDICO> INFORMEMEDICO { get; set; }
        public virtual ICollection<CITA> CITA { get; set; }
    }
}
