using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Encuesta.Tipos
{
    public class EncuestaColumnaCViewModel
    {
        public EXAMENXATENCION _examen { get; set; }
        public PACIENTE _paciente { get; set; }
        public int numeroexamen { get; set; }
        public int equipoAsignado { get; set; }
        public string modalidad { get; set; }
        public string sexo { get; set; }
        public int tipo_encu { get; set; }

        public bool? mol_p1 { get; set; }
        public bool mol_p1_1 { get; set; }
        public bool mol_p1_2 { get; set; }
        public bool mol_p1_3 { get; set; }
        public string mol_p1_3_1 { get; set; }
        public bool? mol_p2 { get; set; }
        public bool mol_p2_1 { get; set; }
        public bool mol_p2_2 { get; set; }
        public string mol_p2_2_1 { get; set; }
        public bool mol_p2_3 { get; set; }
        public string mol_p2_3_1 { get; set; }
        public string mol_p3_1 { get; set; }
        public string mol_p4_1 { get; set; }
        public int mol_p4_2 { get; set; }
        public bool? mol_p5 { get; set; }
        public bool mol_p6 { get; set; }
        public bool mol_p6_1 { get; set; }
        public bool mol_p6_2 { get; set; }
        public string mol_p7 { get; set; }
        public string mol_p8 { get; set; }
        public bool? pro_p1 { get; set; }
        public string pro_p1_1 { get; set; }
        public string pro_p1_21 { get; set; }
        public int pro_p1_22 { get; set; }
        public string pro_p1_3 { get; set; }
        public string pro_p1_4 { get; set; }
        public bool? pro_p2 { get; set; }
        public int pro_p2_1 { get; set; }
        public string pro_p2_11 { get; set; }
        public string pro_p2_21 { get; set; }
        public int pro_p2_22 { get; set; }
        public string pro_p2_3 { get; set; }
        public bool pro_p2_4_1 { get; set; }
        public bool pro_p2_4_2 { get; set; }
        public string pre_p1A { get; set; }
        public bool pre_p1Acheck { get; set; }
        public string pre_p1B { get; set; }
        public bool pre_p1Bcheck { get; set; }
        public string pre_p2 { get; set; }
        public string ind_p1 { get; set; }
        public int ind_p1_1 { get; set; }
        public bool mol_p6_3 { get; set; }
        public bool mol_p6_4 { get; set; }
        public string mol_p5_1 { get; set; }

        public EncuestaDetalleViewModel _encuesta { get; set; }
    }
}