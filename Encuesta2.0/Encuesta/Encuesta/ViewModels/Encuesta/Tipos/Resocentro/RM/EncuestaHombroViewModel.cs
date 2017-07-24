using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaHombroViewModel
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
        public bool p1_6 { get; set; }
        public bool p1_7 { get; set; }
        public string p1_7_1 { get; set; }
        public bool? p2 { get; set; }
        public bool? p2_1 { get; set; }
        public int? p2_21 { get; set; }
        public int? p2_22 { get; set; }
        public string p3 { get; set; }
        public string p4_1 { get; set; }
        public int p4_2 { get; set; }
        public bool? p4_3 { get; set; }
        public string p4_3_1 { get; set; }
        public bool? p5 { get; set; }
        public string p5_1 { get; set; }
        public bool? p6 { get; set; }
        public string p6_1 { get; set; }
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
        public int p9_22 { get; set; }
        public string p9_3 { get; set; }
        public bool? p10 { get; set; }
        public int p10_1 { get; set; }
        public string p10_11 { get; set; }
        public string p10_21 { get; set; }
        public int p10_22 { get; set; }
        public string p10_3 { get; set; }
        public bool? p11 { get; set; }
        public int p11_1 { get; set; }
        public string p11_11 { get; set; }
        public int p11_2 { get; set; }
        public string p12A { get; set; }
        public bool p12Acheck { get; set; }
        public string p12B { get; set; }
        public bool p12Bcheck { get; set; }
        public string p13 { get; set; }
        public int? p14 { get; set; }
        public int p14_1 { get; set; }
        public bool p15 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }

    }
}
