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
    
    public partial class USUARIO
    {
        public USUARIO()
        {
            this.CorrelativoMotivo = new HashSet<CorrelativoMotivo>();
            this.Encuesta_Satisfaccion = new HashSet<Encuesta_Satisfaccion>();
            this.HistorialCorrelativo = new HashSet<HistorialCorrelativo>();
            this.Incidente = new HashSet<Incidente>();
            this.Incidente1 = new HashSet<Incidente>();
            this.Incidente2 = new HashSet<Incidente>();
            this.Institucion_Promotor = new HashSet<Institucion_Promotor>();
            this.MotivoCodigo = new HashSet<MotivoCodigo>();
            this.MotivoCodigo1 = new HashSet<MotivoCodigo>();
            this.Planificacion_Documento = new HashSet<Planificacion_Documento>();
            this.Planificacion_Documento1 = new HashSet<Planificacion_Documento>();
            this.Planificacion_Documento2 = new HashSet<Planificacion_Documento>();
            this.Planificacion_Documento3 = new HashSet<Planificacion_Documento>();
            this.RESULTADODELIVERY = new HashSet<RESULTADODELIVERY>();
            this.RESULTADODELIVERY1 = new HashSet<RESULTADODELIVERY>();
            this.SUCURSALXUSUARIO = new HashSet<SUCURSALXUSUARIO>();
            this.SupervisarEncuesta = new HashSet<SupervisarEncuesta>();
            this.SupervisarEncuesta1 = new HashSet<SupervisarEncuesta>();
            this.SupervisarEncuesta2 = new HashSet<SupervisarEncuesta>();
            this.TKT_Counter = new HashSet<TKT_Counter>();
            this.Ubicacion = new HashSet<Ubicacion>();
            this.AUDITORIA_CARTAS_GARANTIA = new HashSet<AUDITORIA_CARTAS_GARANTIA>();
            this.CARTAGARANTIA = new HashSet<CARTAGARANTIA>();
            this.CARTAGARANTIA1 = new HashSet<CARTAGARANTIA>();
            this.CARTAGARANTIA2 = new HashSet<CARTAGARANTIA>();
            this.Planificacion = new HashSet<Planificacion>();
            this.Planificacion_CompaniaSeguro_Documento = new HashSet<Planificacion_CompaniaSeguro_Documento>();
            this.Planificacion_CompaniaSeguro_Documento1 = new HashSet<Planificacion_CompaniaSeguro_Documento>();
            this.Planificacion_CompaniaSeguro_Documento2 = new HashSet<Planificacion_CompaniaSeguro_Documento>();
            this.Planificacion_CompaniaSeguro_Documento3 = new HashSet<Planificacion_CompaniaSeguro_Documento>();
            this.Planificacion_CompaniaSeguro = new HashSet<Planificacion_CompaniaSeguro>();
            this.CompaniaSeguro_Promotor = new HashSet<CompaniaSeguro_Promotor>();
            this.AsignacionRepresentanteMedico = new HashSet<AsignacionRepresentanteMedico>();
            this.ConversacionIncidente = new HashSet<ConversacionIncidente>();
        }
    
        public int jerarquia { get; set; }
        public string estadousuario { get; set; }
        public string siglas { get; set; }
        public string codigousuario { get; set; }
        public string dni { get; set; }
        public bool bloqueado { get; set; }
        public string contrasena { get; set; }
        public string ShortName { get; set; }
        public Nullable<bool> isEnf { get; set; }
        public Nullable<bool> isAnes { get; set; }
        public string username { get; set; }
        public string Acro { get; set; }
        public Nullable<int> CurrentUnidad { get; set; }
        public bool isSessionLocked { get; set; }
        public string User_Uid { get; set; }
        public Nullable<int> AreaId { get; set; }
    
        public virtual ICollection<CorrelativoMotivo> CorrelativoMotivo { get; set; }
        public virtual ICollection<Encuesta_Satisfaccion> Encuesta_Satisfaccion { get; set; }
        public virtual ICollection<HistorialCorrelativo> HistorialCorrelativo { get; set; }
        public virtual ICollection<Incidente> Incidente { get; set; }
        public virtual ICollection<Incidente> Incidente1 { get; set; }
        public virtual ICollection<Incidente> Incidente2 { get; set; }
        public virtual ICollection<Institucion_Promotor> Institucion_Promotor { get; set; }
        public virtual ICollection<MotivoCodigo> MotivoCodigo { get; set; }
        public virtual ICollection<MotivoCodigo> MotivoCodigo1 { get; set; }
        public virtual ICollection<Planificacion_Documento> Planificacion_Documento { get; set; }
        public virtual ICollection<Planificacion_Documento> Planificacion_Documento1 { get; set; }
        public virtual ICollection<Planificacion_Documento> Planificacion_Documento2 { get; set; }
        public virtual ICollection<Planificacion_Documento> Planificacion_Documento3 { get; set; }
        public virtual ICollection<RESULTADODELIVERY> RESULTADODELIVERY { get; set; }
        public virtual ICollection<RESULTADODELIVERY> RESULTADODELIVERY1 { get; set; }
        public virtual ICollection<SUCURSALXUSUARIO> SUCURSALXUSUARIO { get; set; }
        public virtual ICollection<SupervisarEncuesta> SupervisarEncuesta { get; set; }
        public virtual ICollection<SupervisarEncuesta> SupervisarEncuesta1 { get; set; }
        public virtual ICollection<SupervisarEncuesta> SupervisarEncuesta2 { get; set; }
        public virtual ICollection<TKT_Counter> TKT_Counter { get; set; }
        public virtual ICollection<Ubicacion> Ubicacion { get; set; }
        public virtual ICollection<AUDITORIA_CARTAS_GARANTIA> AUDITORIA_CARTAS_GARANTIA { get; set; }
        public virtual ICollection<CARTAGARANTIA> CARTAGARANTIA { get; set; }
        public virtual ICollection<CARTAGARANTIA> CARTAGARANTIA1 { get; set; }
        public virtual ICollection<CARTAGARANTIA> CARTAGARANTIA2 { get; set; }
        public virtual ICollection<Planificacion> Planificacion { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro_Documento> Planificacion_CompaniaSeguro_Documento { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro_Documento> Planificacion_CompaniaSeguro_Documento1 { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro_Documento> Planificacion_CompaniaSeguro_Documento2 { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro_Documento> Planificacion_CompaniaSeguro_Documento3 { get; set; }
        public virtual ICollection<Planificacion_CompaniaSeguro> Planificacion_CompaniaSeguro { get; set; }
        public virtual ICollection<CompaniaSeguro_Promotor> CompaniaSeguro_Promotor { get; set; }
        public virtual ICollection<AsignacionRepresentanteMedico> AsignacionRepresentanteMedico { get; set; }
        public virtual EMPLEADO EMPLEADO { get; set; }
        public virtual ICollection<ConversacionIncidente> ConversacionIncidente { get; set; }
    }
}
