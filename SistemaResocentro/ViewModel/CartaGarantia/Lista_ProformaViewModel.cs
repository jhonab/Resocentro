using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class Lista_ProformaViewModel
    {
        public int numero_proforma { get; set; }
        public DateTime fechaEmision { get; set; }
        public string titular { get; set; }
        public string poliza { get; set; }
        public string contratante { get; set; }
        public string tiempo_enf { get; set; }
        public bool sedacion { get; set; }
        public string comentarios { get; set; }
        public string idadjunto { get; set; }
        public string tramitador { get; set; }

        public List<DETALLEPROFORMA> estudios { get; set; }
        public string pac { get; set; }
        public PACIENTE paciente { get; set; }
        public COMPANIASEGURO companiaseguro { get; set; }
        public CLINICAHOSPITAL clinica { get; set; }
        public MEDICOEXTERNO medico { get; set; }
        public ESPECIALIDAD especialidad { get; set; }
    }
}