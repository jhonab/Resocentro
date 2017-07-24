using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaMamaViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? p1 { get; set; }
        public string p1_0 { get; set; }
        public bool p1_1 { get; set; }
        public bool p1_2 { get; set; }
        public bool p1_3 { get; set; }
        public bool p1_4 { get; set; }
        public bool p1_5 { get; set; }
        public bool p1_6 { get; set; }
        public bool p1_1_1 { get; set; }
        public bool p1_2_2 { get; set; }
        public bool p1_3_1 { get; set; }
        public bool p1_4_1 { get; set; }
        public bool p1_5_1 { get; set; }
        public bool p1_6_1 { get; set; }
        public int? p2 { get; set; }
        public int p2_1 { get; set; }
        public bool? p3 { get; set; }
        public string p3_1 { get; set; }
        public bool? p4 { get; set; }
        public string p4_1 { get; set; }
        public string p4_2 { get; set; }
        public int p4_2_1 { get; set; }
        public bool p4_3 { get; set; }
        public string p4_4 { get; set; }
        public string p5 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public string p8 { get; set; }
        public bool? p9 { get; set; }
        public string p9_1 { get; set; }
        public int p9_2 { get; set; }
        public string ind_p1 { get; set; }
        public int ind_p1_1 { get; set; }

        public bool p10 { get; set; }
        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}