using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels
{
    public class AmpliacionViewModel
    {
        public EXAMENXCITA cita{ get; set; }
        public EXAMENXATENCION examen { get; set; }
    }
}