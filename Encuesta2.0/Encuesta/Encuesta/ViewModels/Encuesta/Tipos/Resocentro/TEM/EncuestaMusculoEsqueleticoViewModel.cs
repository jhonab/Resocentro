using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaMusculoEsqueleticoViewModel
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
        public int p3_1 { get; set; }
        public string p3_2 { get; set; }
        public int p4 { get; set; }
        public string p4_1 { get; set; }
        public bool? p5 { get; set; }
        public string p5_1 { get; set; }
        public string p6 { get; set; }
        public bool? p7 { get; set; }
        public int p7_1 { get; set; }
        public string p7_2 { get; set; }
        public string p7_3 { get; set; }
        public bool? p8 { get; set; }
        public string p8_1 { get; set; }
        public string p8_1_1 { get; set; }
        public string p8_2 { get; set; }
        public string p8_2_1 { get; set; }
        public bool? p9 { get; set; }
        public int p9_1 { get; set; }
        public bool p9_3 { get; set; }
        public string p10 { get; set; }
        public bool p10_1 { get; set; }
        public string p11 { get; set; }
        public bool p11_1 { get; set; }
        public string p12 { get; set; }
        public string p13 { get; set; }
        public int p14 { get; set; }
        public int p14_1 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}