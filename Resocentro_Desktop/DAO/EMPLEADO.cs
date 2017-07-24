//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resocentro_Desktop.DAO
{
    using System;
    using System.Collections.Generic;
    
    public partial class EMPLEADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLEADO()
        {
            this.USUARIO = new HashSet<USUARIO>();
        }
    
        public string estadoactual { get; set; }
        public string modalidad { get; set; }
        public System.DateTime fechaingreso { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public string telefono { get; set; }
        public System.DateTime fechanacimiento { get; set; }
        public string sexo { get; set; }
        public string estadocivil { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string dni { get; set; }
        public Nullable<int> codigocargo { get; set; }
        public string CMP2 { get; set; }
        public string RNE { get; set; }
        public Nullable<bool> isCarta { get; set; }
        public bool isActivo { get; set; }
        public int cant_hijo { get; set; }
        public Nullable<decimal> sueldo { get; set; }
        public Nullable<int> idAFP { get; set; }
        public Nullable<bool> isAFP_Mixta { get; set; }
        public Nullable<bool> isEPS { get; set; }
        public string CC { get; set; }
        public Nullable<int> num_planilla { get; set; }
        public string email_personal { get; set; }
        public string pathfoto { get; set; }
        public string dni_corregido { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIO { get; set; }
        public virtual CARGO CARGO { get; set; }
    }
}
