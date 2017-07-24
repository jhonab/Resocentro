using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta
{
    public class VisorCargaViewModell
    {
        public int codigoequipo { get; set; }
        public string nombreequipo { get; set; }
        public List<EstudiosA> estudios { get; set; }
    }
    public class EstudiosA
    {
        public int codigo { get; set; }
        public string paciente { get; set; }
        public string estudio { get; set; }


        public DateTime? inicio { get; set; }

        public string min_transcurri { get; set; }
    }
}