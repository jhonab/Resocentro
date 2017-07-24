using Encuesta.Models;
using Encuesta.ViewModels.Encuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta
{
    public class EncuestaGenericaTEMEMETACModel
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
        public int p2_1 { get; set; }
        public int p2_2 { get; set; }
        public bool? p3 { get; set; }
        public string p4 { get; set; }
        public int p5 { get; set; }
        public string p6 { get; set; }
        public string p7 { get; set; }
        public bool? p8 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}