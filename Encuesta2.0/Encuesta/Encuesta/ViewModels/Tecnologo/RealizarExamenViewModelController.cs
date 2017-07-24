using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Encuesta.Models;
using Encuesta.ViewModels.Encuesta;

namespace Encuesta.ViewModels.Tecnologo
{
    public class RealizarExamenViewModel
    {
        public int num_examen { get; set; }
        public int id_paciente { get; set; }
        public string nom_paciente { get; set; }
        public int? estado { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime hora_cita { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? hora_admision { get; set; }
        public string min_transcurri { get; set; }
        public string condicion { get; set; }
        public string id_estudio { get; set; }
        public string nom_estudio { get; set; }
        public int id_equipo_pro { get; set; }
        public string nom_equipo_pro { get; set; }
        public bool isSedacion { get; set; }
        public bool isVIP { get; set; }
        public int id_equipo_rea { get; set; }
        public string nom_equipo_rea { get; set; }
        public DateTime fec_solici_supervision { get; set; }
        public bool? isValidacion { get; set; }
        public bool? isValidado { get; set; }
        public bool? isIniciado { get; set; }
        public PACIENTE paciente { get; set; }
        public EXAMENXATENCION examen { get; set; }
        public string anestesiologa { get; set; }
        public string enfermera { get; set; }
        public bool isContraste { get; set; }
        public bool istecnicas { get; set; }
        public List<EXAMENXCITA> tecnica { get; set; }

        public string contraste { get; set; }

        public bool isPostproceso { get; set; }

        public string inf_adicional { get; set; }

        public string postproceso { get; set; }

        public int placas { get; set; }

        public bool isContrasteContinuo { get; set; }

        public EncuestaDetalleViewModel encuesta { get; set; }

        public bool isActivado { get; set; }

        public int minrestante { get; set; }

        public int estadoContraste { get; set; }

        public int numerocita { get; set; }

        public int numeroatencion { get; set; }
        public int grupomedico { get; set; }


        public string modalidad { get; set; }
    }

 
}
