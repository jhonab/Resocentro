using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaAngioCerebralViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? p1 { get; set; }
        public int p1_1 { get; set; }
        public string p1_2 { get; set; }
        public string p1_3 { get; set; }
        public bool? p2 { get; set; }
        public string p2_1 { get; set; }
        public bool? p3 { get; set; }
        public int p3_1 { get; set; }
        public bool? p3_2 { get; set; }
        public bool? p4 { get; set; }
        public int p4_1 { get; set; }
        public bool? p4_2 { get; set; }
        public bool p5_1 { get; set; }
        public bool p5_2 { get; set; }
        public bool p5_3 { get; set; }
        public string p5_4 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public string p8 { get; set; }
        public string p9 { get; set; }
        public string p10 { get; set; }
        public int p11 { get; set; }
    }
}