using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaCodoViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? p1 { get; set; }
        public bool p1_1 { get; set; }
        public bool p1_2 { get; set; }
        public bool p1_3 { get; set; }
        public bool p1_4 { get; set; }
        public bool p1_5 { get; set; }
        public int? p1_51 { get; set; }
        public bool p1_6 { get; set; }
        public bool p1_7 { get; set; }
        public string p1_71 { get; set; }
        public bool p1_8 { get; set; }
        public string p1_8_1 { get; set; }
        public string p2_1 { get; set; }
        public int p2_2 { get; set; }
        public bool? p2_3 { get; set; }
        public string p2_3_1 { get; set; }
        public string p3 { get; set; }
        public string p3_1 { get; set; }
        public string p4 { get; set; }
        public string p4_1 { get; set; }
        public string p5 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public bool? p8 { get; set; }
        public string p8_1 { get; set; }
        public string p8_21 { get; set; }
        public int p8_22 { get; set; }
        public string p8_3 { get; set; }
        public string p8_4 { get; set; }
        public bool? p9 { get; set; }
        public string p9_1 { get; set; }
        public int? p9_21 { get; set; }
        public int p9_22 { get; set; }
        public bool? p10 { get; set; }
        public int p10_1 { get; set; }
        public string p10_11 { get; set; }
        public string p10_21 { get; set; }
        public int p10_22 { get; set; }
        public string p10_3 { get; set; }
        public bool? p11 { get; set; }
        public int p11_11 { get; set; }
        public string p11_12 { get; set; }
        public int p11_2 { get; set; }
        public string p12A_1 { get; set; }
        public bool p12A_2 { get; set; }
        public string p12B_1 { get; set; }
        public bool p12B_2 { get; set; }
        public string p13 { get; set; }
        public string p14 { get; set; }
        public int p14_1 { get; set; }

        public bool p15 { get; set; }
        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}