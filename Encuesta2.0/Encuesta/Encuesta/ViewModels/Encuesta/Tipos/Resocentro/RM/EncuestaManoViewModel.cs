using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaManoViewModel
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
        public bool p1_11 { get; set; }
        public bool p1_12 { get; set; }
        public bool p1_13 { get; set; }
        public bool p1_14 { get; set; }
        public bool p1_15 { get; set; }
        public bool p1_16 { get; set; }
        public string p1_16_1 { get; set; }
        public bool p1_2 { get; set; }
        public string p1_21 { get; set; }
        public bool p1_3 { get; set; }
        public bool p1_4 { get; set; }
        public bool p1_5 { get; set; }
        public bool p1_6 { get; set; }
        public bool p1_7 { get; set; }
        public string p1_7_1 { get; set; }
        public bool? p2 { get; set; }
        public bool? p3 { get; set; }
        public string p3_1 { get; set; }
        public string p4_1 { get; set; }
        public int p4_2 { get; set; }
        public bool? p4_3 { get; set; }
        public string p4_3_1 { get; set; }
        public string p5 { get; set; }
        public string p5_1 { get; set; }
        public string p6 { get; set; }
        public string p6_1 { get; set; }
        public string p7 { get; set; }
        public string p8 { get; set; }
        public string p9 { get; set; }
        public bool? p10 { get; set; }
        public string p10_1 { get; set; }
        public string p10_21 { get; set; }
        public int p10_22 { get; set; }
        public string p10_3 { get; set; }
        public string p10_4 { get; set; }
        public bool? p11 { get; set; }
        public string p11_1 { get; set; }
        public int p11_21 { get; set; }
        public int p11_22 { get; set; }
        public bool? p12 { get; set; }
        public int p12_1 { get; set; }
        public string p12_11 { get; set; }
        public string p12_21 { get; set; }
        public int p12_22 { get; set; }
        public string p12_3 { get; set; }
        public bool? p13 { get; set; }
        public int p13_11 { get; set; }
        public string p13_12 { get; set; }
        public int p13_2 { get; set; }
        public string p14A_1 { get; set; }
        public bool p14A_2 { get; set; }
        public string p14B_1 { get; set; }
        public bool p14B_2 { get; set; }
        public string p15 { get; set; }
        public string p16 { get; set; }
        public int p16_1 { get; set; }

        public bool p17 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}