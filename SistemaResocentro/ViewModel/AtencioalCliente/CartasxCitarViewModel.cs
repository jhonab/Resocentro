using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class CartasxCitarViewModel
    {
        public string codigocarta { get; set; }

        public string paciente { get; set; }

        public DateTime inicio { get; set; }

        public DateTime aprobacion { get; set; }

        public string telefono { get; set; }

        public string celular { get; set; }

        public int llamadas { get; set; }

        public string cobertura { get; set; }

        public string aseguradora { get; set; }

        public string estado { get; set; }

        public string adjunto { get; set; }

        public int idpaciente { get; set; }

        public bool isrevisada { get; set; }
    }
}