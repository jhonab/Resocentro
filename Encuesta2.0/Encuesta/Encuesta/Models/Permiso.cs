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
    
    public partial class Permiso
    {
        public int idpermiso { get; set; }
        public string codigousuario { get; set; }
        public string aplicativo { get; set; }
        public Nullable<int> tipo_permiso { get; set; }
        public string descripcion { get; set; }
        public Nullable<int> idaplicativo { get; set; }
    
        public virtual Aplicativo Aplicativo1 { get; set; }
    }
}
