using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta
{
    public class EncuestaDetalleViewModel
    {
        public string encuestador { get; set; }
        public string supervisor { get; set; }
        public string tecnologo { get; set; }
        public string equipo { get; set; }
        public int nexamen { get; set; }
        public bool isVisible { get; set; }
        public SupervisarEncuesta calificacion { get; set; }
        public bool isampliacion { get; set; }
        public string AmpliMotivo { get; set; }
        public string encuestadorAmpli { get; set; }
        public string tecnoAmpli { get; set; }
        public string superAmpli { get; set; }


        public string equipoAmpli { get; set; }

        public string comentariosTecnologo { get; set; }

        public SupervisarEncuesta calificacion_Ampli { get; set; }

        public int nexamenAmpli { get; set; }

        public string comentariosTecnologoAmpli { get; set; }
        public string tecnicas { get; set; }

        public bool contraste { get; set; }
    }
}