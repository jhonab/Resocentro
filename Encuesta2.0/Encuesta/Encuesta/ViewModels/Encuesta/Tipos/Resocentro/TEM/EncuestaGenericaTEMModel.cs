using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaGenericaTEMModel
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
        public string p3_2 { get; set; }
        public bool? p4 { get; set; }
        public string p4_1 { get; set; }
        public int p5 { get; set; }
        public bool? p6 { get; set; }
        public string p6_1 { get; set; }
        public int p7 { get; set; }
        public bool p7_1 { get; set; }
        public bool p7_2 { get; set; }
        public bool p7_3 { get; set; }
        public bool p7_4 { get; set; }
        public bool p7_5 { get; set; }
        public bool p7_6 { get; set; }
        public bool p7_7 { get; set; }
        public bool p7_8 { get; set; }
        public bool p7_9 { get; set; }
        public bool p7_10 { get; set; }
        public string p8 { get; set; }
        public int p9 { get; set; }
        public bool p9_1 { get; set; }
        public bool p9_2 { get; set; }
        public bool p9_3 { get; set; }
        public bool p9_4 { get; set; }
        public bool p9_5 { get; set; }
        public bool p9_6 { get; set; }
        public bool p9_7 { get; set; }
        public bool p9_8 { get; set; }
        public bool p9_9 { get; set; }
        public bool p9_10 { get; set; }
        public bool p9_11 { get; set; }
        public bool p9_12 { get; set; }
        public string p10A_1 { get; set; }
        public bool p10A_2 { get; set; }
        public string p10B_1 { get; set; }
        public bool p10B_2 { get; set; }
        public string p11 { get; set; }
        public string p12 { get; set; }
        public int p12_1 { get; set; }


        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}