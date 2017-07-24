using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class Sedaciones_ProgramadasViewModel
    {
        public DateTime fecha { get; set; }

        public string numerocita { get; set; }

        public string paciente { get; set; }

        public string aseguradora { get; set; }

        public string clinica { get; set; }

        public string edad { get; set; }

        public string equipo { get; set; }

        public string estudio { get; set; }

        public string sedador { get; set; }
    }
}