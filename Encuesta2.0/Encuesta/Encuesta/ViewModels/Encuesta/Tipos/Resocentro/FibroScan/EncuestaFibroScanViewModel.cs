using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaFibroScanViewModel
    {
        public EXAMENXATENCION _examen { get; set; }

        public PACIENTE _paciente { get; set; }

        public int numeroexamen { get; set; }

        public int equipoAsignado { get; set; }

        public string modalidad { get; set; }

        public string sexo { get; set; }

        public int tipo_encu { get; set; }

        public int p1 { get; set; }
        public bool p2_1 { get; set; }
        public bool p2_2 { get; set; }
        public bool p2_3 { get; set; }
        public bool p2_4{ get; set; }
        public bool? p3 { get; set; }
        public bool? p4 { get; set; }
        public bool? p5 { get; set; }
        public int p6{get;set;}
        public bool p7_1 { get; set; }
        public bool p7_2 { get; set; }
        public bool p7_3 { get; set; }
        public bool p7_4 { get; set; }
        public string p7_4_1 { get; set; }
        public bool p8_1 { get; set; }
        public bool p8_2 { get; set; }
        public bool p8_3 { get; set; }
        public bool p8_4 { get; set; }
        public string p9 { get; set; }

    }
}