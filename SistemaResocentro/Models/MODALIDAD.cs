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
    
    public partial class MODALIDAD
    {
        public MODALIDAD()
        {
            this.ESTUDIO_COMPANIA = new HashSet<ESTUDIO_COMPANIA>();
            this.CITA = new HashSet<CITA>();
        }
    
        public string nombre { get; set; }
        public int codigomodalidad { get; set; }
        public int codigounidad { get; set; }
        public string abreviatura { get; set; }
        public Nullable<bool> IsEnabled { get; set; }
        public string codigomodalidad3 { get; set; }
        public bool isRadiacion { get; set; }
    
        public virtual ICollection<ESTUDIO_COMPANIA> ESTUDIO_COMPANIA { get; set; }
        public virtual UNIDADNEGOCIO UNIDADNEGOCIO { get; set; }
        public virtual ICollection<CITA> CITA { get; set; }
    }
}
