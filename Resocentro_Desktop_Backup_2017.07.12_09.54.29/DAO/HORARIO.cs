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
    
    public partial class HORARIO
    {
        public int codigohorario { get; set; }
        public bool atendido { get; set; }
        public bool confirmado { get; set; }
        public System.DateTime hora { get; set; }
        public System.DateTime fecha { get; set; }
        public string turnomedico { get; set; }
        public bool bloquear { get; set; }
        public Nullable<int> numerocita { get; set; }
        public Nullable<int> codigopaciente { get; set; }
        public string codigoestudio { get; set; }
        public Nullable<int> codigomodalidad { get; set; }
        public Nullable<int> codigounidad { get; set; }
        public int codigoequipo { get; set; }
        public Nullable<bool> IsEntre { get; set; }
    
        public virtual EQUIPO EQUIPO { get; set; }
    }
}
