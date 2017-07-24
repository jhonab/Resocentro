using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaOncologicoViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? p1 { get; set; }
        public string p1_1 { get; set; }
        public bool? p2 { get; set; }
        public bool? p2_1 { get; set; }
        public int p2_1_1 { get; set; }
        public string p2_1_2 { get; set; }
        public string p2_2 { get; set; }
        public string p2_3 { get; set; }
        public int p3 { get; set; }
        public string p3_1 { get; set; }
        public bool? p4 { get; set; }
        public string p4_1 { get; set; }
        public bool? p5 { get; set; }
        public bool? p5_1 { get; set; }
        public int p5_1_1 { get; set; }
        public string p5_1_2 { get; set; }
        public bool? p5_2 { get; set; }
        public int p5_2_1 { get; set; }
        public string p5_2_2 { get; set; }
        public bool? p5_3 { get; set; }
        public int p5_3_1 { get; set; }
        public string p5_3_2 { get; set; }
        public string p5_4 { get; set; }
        public bool? p6 { get; set; }
        public bool? p6_1 { get; set; }
        public int p6_2 { get; set; }
        public string p6_3 { get; set; }
        public bool? p7 { get; set; }
        public bool? p7_1 { get; set; }
        public int p7_2 { get; set; }
        public string p7_3 { get; set; }
        public bool? p8 { get; set; }
        public int p8_1 { get; set; }
        public int p8_2 { get; set; }
        public string p8_3 { get; set; }
        public int p8_4 { get; set; }
        public string p8_5 { get; set; }
        public string p9 { get; set; }

        public bool p10 { get; set; }
        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}