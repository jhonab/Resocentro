using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Encuesta.Models;
using Encuesta.ViewModels.Encuesta;


namespace Encuesta.ViewModels.Supervisor
{
    public class SupervisarExamenViewModel
    {
        public int num_examen { get; set; }
        public string nom_paciente { get; set; }
        public int? estado { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime hora_cita { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime hora_admision { get; set; }
        public string min_transcurri { get; set; }
        public string condicion { get; set; }
        public string id_estudio { get; set; }
        public string nom_estudio { get; set; }
        public string nom_equipo { get; set; }
        public bool isSedacion { get; set; }
        public bool isVIP { get; set; }
        public bool isinEncuesta { get; set; }
        public bool isinImagenes { get; set; }
        public bool isEncuestaVisible { get; set; }
        public bool isTecnologoVisible { get; set; }
        public bool? p1 { get; set; }
        public string p1_1 { get; set; }
        public bool? p2 { get; set; }
        public string p2_1 { get; set; }
        public bool? p3 { get; set; }
        public string p3_1 { get; set; }
        public int? p4 { get; set; }
        public string p4_1 { get; set; }
        public bool? p5 { get; set; }
        public string p5_1 { get; set; }
        public int grupomedico { get; set; }
        public string diagnostico { get; set; }
        public DateTime inicio { get; set; }
        public PACIENTE paciente { get; set; }
        public EXAMENXATENCION examen { get; set; }
        public EncuestaDetalleViewModel encuesta { get; set; }

        public bool? p1_cambio { get; set; }
        public string p1_1_cambio { get; set; }
        public bool? p2_cambio { get; set; }
        public string p2_1_cambio { get; set; }
        public bool? p3_cambio { get; set; }
        public string p3_1_cambio { get; set; }
        public int? p4_cambio { get; set; }
        public string p4_1_cambio { get; set; }
        public int tipo_cambio { get; set; }

        public bool modo { get; set; }

        public bool docEscan { get; set; }

        public bool docCarta { get; set; }
        public bool isSupervisable { get; set; }

        public string modalidad { get; set; }

        public bool isurgente { get; set; }

        public string motivo_urgente { get; set; }

        public bool isPerfusion { get; set; }
        public string motivo_contraste { get; set; }
    }
    public class RevisionCalificacion
    {

        public int numeroexamen { get; set; }
        public string encuestador { get; set; }
        public string tecnologo { get; set; }
        public DateTime fecha_registro { get; set; }
        public bool? p1 { get; set; }
        public string p1_motivo { get; set; }
        public bool? p2 { get; set; }
        public string p2_motivo { get; set; }
        public bool? p3 { get; set; }
        public string p3_motivo { get; set; }
        public string dx_pre { get; set; }

        public string supervisor { get; set; }

        public bool? p1_inf { get; set; }

        public string p1_inf_motivo { get; set; }

        public bool? p2_inf { get; set; }

        public string p2_inf_motivo { get; set; }

        public bool? p3_inf { get; set; }

        public string p3_inf_motivo { get; set; }

        public bool? dx_pre_inf { get; set; }

        public string informante { get; set; }

        public bool? p1_val { get; set; }

        public string p1_val_motivo { get; set; }

        public bool? p2_val { get; set; }

        public string p2_val_motivo { get; set; }

        public bool? p3_val { get; set; }

        public string p3_val_motivo { get; set; }

        public bool? dx_pre_val { get; set; }

        public string validador { get; set; }

    }
}