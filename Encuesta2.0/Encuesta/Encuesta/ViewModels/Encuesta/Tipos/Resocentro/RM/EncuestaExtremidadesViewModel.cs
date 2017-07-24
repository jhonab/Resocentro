using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaExtremidadesViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? p1 { get; set; }
        public bool p1_0 { get; set; }
        public string p1_1 { get; set; }
        public bool p1_2 { get; set; }
        public bool p1_3 { get; set; }
        public bool p1_4 { get; set; }
        public bool p1_5 { get; set; }        
        public bool? p2 { get; set; }
        public bool p2_1 { get; set; }
        public bool p2_2 { get; set; }
        public bool p2_3 { get; set; }
        public bool p2_4 { get; set; }
        public bool p2_5 { get; set; }
        public string p2_5_1 { get; set; }
        public bool? p3 { get; set; }
        public string p3_1 { get; set; }
        public string p4_1 { get; set; }
        public int p4_2 { get; set; }
        public bool? p4_3 { get; set; }
        public string p4_3_1 { get; set; }
        public bool? p5 { get; set; }
        public string p5_1 { get; set; }
        public string p6 { get; set; }
        public string p7_1 { get; set; }
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
        public int p11_1 { get; set; }
        public string p11_11 { get; set; }
        public string p11_21 { get; set; }
        public int p11_22 { get; set; }
        public string p11_3 { get; set; }
        public bool? p12 { get; set; }
        public int p12_1 { get; set; }
        public string p12_2 { get; set; }
        public int p12_3 { get; set; }
        public string p13A_1 { get; set; }
        public bool p13A_2 { get; set; }
        public string p13B_1 { get; set; }
        public bool p13B_2 { get; set; }
        public string p14_1 { get; set; }
        public bool p14_2 { get; set; }
        public string p15 { get; set; }
        public int p15_1 { get; set; }

        public bool p16 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}