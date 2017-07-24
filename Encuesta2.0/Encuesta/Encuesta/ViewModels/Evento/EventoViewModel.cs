using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Evento
{
    public class EventoViewModel
    {

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public DateTime FecNew { get; set; }

        public string TecnoReg { get; set; }

        public string evento { get; set; }

        public string equipo { get; set; }

        public string nota { get; set; }
    }

 
}