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
    
    public partial class Detalle_Contraste
    {
        public int idDetalle_contras { get; set; }
        public int id_contraste { get; set; }
        public Nullable<int> id_insumo { get; set; }
        public Nullable<decimal> cantidad { get; set; }
        public bool isaplicado { get; set; }
        public string lote { get; set; }
        public string tipoerroneo { get; set; }
        public Nullable<int> Al_InsumId { get; set; }
    
        public virtual Contraste Contraste { get; set; }
        public virtual Insumo_Cum_Sunasa Insumo_Cum_Sunasa { get; set; }
    }
}
