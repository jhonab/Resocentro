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
    
    public partial class DetalleFacturaGlobal
    {
        public int idDetFac { get; set; }
        public int idFac { get; set; }
        public int atencion { get; set; }
        public int paciente { get; set; }
        public string codigoestudio { get; set; }
        public string nombreestudio { get; set; }
        public decimal precio { get; set; }
        public int moneda { get; set; }
        public int companiaseguro { get; set; }
    
        public virtual FacturaGlobal FacturaGlobal { get; set; }
    }
}
