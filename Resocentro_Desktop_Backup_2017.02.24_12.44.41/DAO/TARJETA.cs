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
    
    public partial class TARJETA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TARJETA()
        {
            this.FORMADEPAGO = new HashSet<FORMADEPAGO>();
        }
    
        public string descripcion { get; set; }
        public int codigotarjeta { get; set; }
        public bool IsEnabledTarjeta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMADEPAGO> FORMADEPAGO { get; set; }
    }
}
