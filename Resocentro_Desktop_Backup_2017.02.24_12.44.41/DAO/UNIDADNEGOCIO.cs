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
    
    public partial class UNIDADNEGOCIO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UNIDADNEGOCIO()
        {
            this.SUCURSAL = new HashSet<SUCURSAL>();
        }
    
        public string nombre { get; set; }
        public int codigounidad { get; set; }
        public string NombreSmall { get; set; }
        public Nullable<bool> estadoUnidad { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUCURSAL> SUCURSAL { get; set; }
    }
}
