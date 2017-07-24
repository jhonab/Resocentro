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
    
    public partial class CARTAGARANTIA
    {
        public CARTAGARANTIA()
        {
            this.CITA = new HashSet<CITA>();
        }
    
        public string seguimiento { get; set; }
        public string estadocarta { get; set; }
        public System.DateTime fechatramite { get; set; }
        public string contratante { get; set; }
        public string titular { get; set; }
        public string poliza { get; set; }
        public string codigocartagarantia { get; set; }
        public string cmp { get; set; }
        public int codigocompaniaseguro { get; set; }
        public string ruc { get; set; }
        public int codigopaciente { get; set; }
        public float cobertura { get; set; }
        public float monto { get; set; }
        public Nullable<System.DateTime> fechaprobacion { get; set; }
        public string codigousuario { get; set; }
        public string numerocarnetseguro { get; set; }
        public string codigodocadjunto { get; set; }
        public string cie { get; set; }
        public Nullable<int> beneficio { get; set; }
        public string codigocartagarantia2 { get; set; }
        public string SitedCodigoProducto { get; set; }
        public Nullable<int> Sunasa_CoberturaId { get; set; }
        public Nullable<int> codigoclinica { get; set; }
        public string num_referencia { get; set; }
        public Nullable<int> codigocompaniaseguro2 { get; set; }
        public Nullable<int> codigoclinica2 { get; set; }
        public Nullable<int> TipoAfiliacion { get; set; }
        public Nullable<bool> isRevisada { get; set; }
        public Nullable<System.DateTime> fec_update { get; set; }
        public string user_update { get; set; }
        public string user_revisa { get; set; }
        public Nullable<System.DateTime> fec_revisa { get; set; }
        public Nullable<bool> isCaducada { get; set; }
        public Nullable<int> numero_proforma { get; set; }
        public Nullable<int> codigounidad { get; set; }
        public Nullable<int> codigosucursal { get; set; }
        public bool isCitada { get; set; }
        public string codigocarta_coaseguro { get; set; }
        public bool sedacion_carta { get; set; }
        public bool ishospitalizado { get; set; }
        public string obs_revision { get; set; }
    
        public virtual MEDICOEXTERNO MEDICOEXTERNO { get; set; }
        public virtual COMPANIASEGURO COMPANIASEGURO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
        public virtual USUARIO USUARIO2 { get; set; }
        public virtual ICollection<CITA> CITA { get; set; }
    }
}
