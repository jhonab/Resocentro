using Encuesta.Models;
using Encuesta.ViewModels.Encuesta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Tecnologo
{
    public class PostProcesoViewModel
    {
        public int num_examen { get; set; }
        public string nom_paciente { get; set; }
        public DateTime hora_cita { get; set; }
        public DateTime hora_admision { get; set; }
        public string min_transcurri { get; set; }
        public string nom_estudio { get; set; }
        public string nom_equipo_pro { get; set; }
        public bool isSedacion { get; set; }
        public bool isVIP { get; set; }
        public string modalidad { get; set; }
        public int idequipo { get; set; }
        public bool istecnicas { get; set; }
        public int cantidad { get; set; }
        public string descripcion { get; set; }
        public List<POSTPROCESO> tecnicas { get; set; }
        public PACIENTE paciente { get; set; }
        public EXAMENXATENCION examen { get; set; }
        public EncuestaDetalleViewModel encuesta { get; set; }

        public DateTime inicio { get; set; }
    }
}