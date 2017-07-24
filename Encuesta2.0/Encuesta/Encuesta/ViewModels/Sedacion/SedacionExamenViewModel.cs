using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Sedacion
{
    public class SedacionExamenViewModel
    {
        public int idsedacion { get; set; }

        public int num_examen { get; set; }

        public string nom_paciente { get; set; }

        public string sede { get; set; }

        public DateTime hora_cita { get; set; }

        public DateTime hora_admision { get; set; }

        public string min_transcurri { get; set; }

        public string condicion { get; set; }

        public string nom_estudio { get; set; }

        public string nom_equipo { get; set; }

        public bool isSedacion { get; set; }

        public bool isVIP { get; set; }

        public string tipoerror { get; set; }

        public string enfermera { get; set; }

        public Models.Sedacion sedacion { get; set; }

        public List<Detalle_Sedacion_Insumo> insumos { get; set; }

        public Models.PACIENTE paciente { get; set; }

        public Models.EXAMENXATENCION examen { get; set; }

        public string tiposedacion { get; set; }

        public string motivosedacion { get; set; }

        public string motivo_otros { get; set; }

        public string consentimiento { get; set; }

        public DateTime? fec_ini { get; set; }

        public DateTime? fec_fin { get; set; }



        public int atencion { get; set; }

        public int cantidad { get; set; }
    }
}