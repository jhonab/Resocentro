using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SistemaResocentro.ViewModel
{
    public class ProformaViewModel
    {
        public PROFORMA proforma { get; set; }
        public string estudios { get; set; }
        
        public List<Estudios_proforma> list_estudio { get; set; }
       
        //impresion
        public string medico { get; set; }
        public string aseguradora { get; set; }
        public string paciente { get; set; }
        public string clinica{ get; set; }
        public string sede { get; set; }
    }
    public class Estudios_proforma
    {
        public string idestudio { get; set; }
        public int idclase { get; set; }
        public string nombre { get; set; }
        public double precio { get; set; }
        public double cob { get; set; }
    }
}
