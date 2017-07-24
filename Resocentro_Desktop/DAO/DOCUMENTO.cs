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
    
    public partial class DOCUMENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DOCUMENTO()
        {
            this.DETALLEDOCUMENTO = new HashSet<DETALLEDOCUMENTO>();
            this.COBRANZACIASEGURO = new HashSet<COBRANZACIASEGURO>();
            this.FORMADEPAGO = new HashSet<FORMADEPAGO>();
        }
    
        public string numerodocumento { get; set; }
        public string ruc { get; set; }
        public int codigopaciente { get; set; }
        public string codigousuario { get; set; }
        public string poliza { get; set; }
        public float cobertura { get; set; }
        public string tipodocumento { get; set; }
        public string codigocarta { get; set; }
        public string titular { get; set; }
        public float total { get; set; }
        public float igv { get; set; }
        public float subtotal { get; set; }
        public System.DateTime fechaemitio { get; set; }
        public float tipocambio { get; set; }
        public string estado { get; set; }
        public string tipomoneda { get; set; }
        public string porconcepto { get; set; }
        public string rucalterno { get; set; }
        public Nullable<int> codigounidad { get; set; }
        public Nullable<int> codigosucursal { get; set; }
        public System.DateTime fechadepago { get; set; }
        public string numnotacredito { get; set; }
        public string condicionpago { get; set; }
        public Nullable<int> codigosucursalReal { get; set; }
        public Nullable<bool> isPagado { get; set; }
        public Nullable<System.DateTime> fecha_recibio_cia { get; set; }
        public Nullable<System.DateTime> fec_isPagado { get; set; }
        public string referencia { get; set; }
        public string numnotacreditoTipoDoc { get; set; }
        public string dniAlterno { get; set; }
        public string pathFile { get; set; }
        public bool isSendSUNAT { get; set; }
        public decimal valorIGV { get; set; }
        public int tipoCobranza { get; set; }
        public string razonsocialAlterno { get; set; }
        public bool isFacGlobal { get; set; }
        public Nullable<System.DateTime> fecha_cancelacion { get; set; }
        public string usu_cancela { get; set; }
        public string motivos_adicionales { get; set; }
        public string serie { get; set; }
        public int correlativo { get; set; }
        public string pathFileSummary { get; set; }
        public string motivos_anulacion { get; set; }
    
        public virtual ASEGURADORA ASEGURADORA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DETALLEDOCUMENTO> DETALLEDOCUMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COBRANZACIASEGURO> COBRANZACIASEGURO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMADEPAGO> FORMADEPAGO { get; set; }
    }
}
