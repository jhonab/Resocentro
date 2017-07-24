using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaAbdomenViewModel
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
        public string p1_1_1 { get; set; }
        public bool p1_2 { get; set; }
        public string p1_2_1 { get; set; }
        public bool p1_3 { get; set; }
        public string p1_3_1 { get; set; }
        public bool p1_4 { get; set; }
        public bool p1_5 { get; set; }
        public string p1_5_1 { get; set; }
        public string p1_5_2 { get; set; }
        public bool p1_6 { get; set; }
        public bool p1_7 { get; set; }
        public bool p1_8 { get; set; }
        public bool p1_9 { get; set; }
        public bool p1_10 { get; set; }
        public bool p1_11 { get; set; }
        public string p1_11_1 { get; set; }
        public bool p1_12 { get; set; }
        public bool p1_13 { get; set; }
        public bool p1_14 { get; set; }
        public bool p1_15 { get; set; }
        public bool p1_16 { get; set; }
        public string p1_16_1 { get; set; }
        public bool p1_17 { get; set; }
        public bool p1_18 { get; set; }
        public string p1_18_1 { get; set; }
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
        public string p9_21 { get; set; }
        public bool? p10 { get; set; }
        public int p10_1 { get; set; }
        public string p10_11 { get; set; }
        public string p10_21 { get; set; }
        public int p10_22 { get; set; }
        public string p10_3 { get; set; }
        public string p11A_1 { get; set; }
        public bool p11A_2 { get; set; }
        public string p11B_1 { get; set; }
        public bool p11B_2 { get; set; }
        public string p12 { get; set; }
        public string p13 { get; set; }
        public int p14_1 { get; set; }

        public bool p15 { get; set; }
        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}