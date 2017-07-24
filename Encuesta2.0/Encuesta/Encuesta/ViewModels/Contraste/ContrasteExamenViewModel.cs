using Encuesta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels
{
    public class ContrasteExamenViewModel
    {
        public int num_examen { get; set; }

        public string nom_paciente { get; set; }

        public string sede { get; set; }

        public DateTime hora_cita { get; set; }

        public DateTime hora_admision { get; set; }

        public string min_transcurri { get; set; }

        public string condicion { get; set; }
        public int cantidadContrasteCaja { get; set; }
        public string nom_estudio { get; set; }

        public string nom_equipo { get; set; }

        public bool isSedacion { get; set; }

        public bool isVIP { get; set; }

        public string enfermera { get; set; }
        public int consentimiento { get; set; }

        public int idcontraste { get; set; }
        public string tipoerror { get; set; }
        public PACIENTE paciente { get; set; }

        public EXAMENXATENCION examen { get; set; }

        public Contraste contraste { get; set; }

        public List<DetalleInsumosViewModel> insumos { get; set; }

        public string tipoInyector { get; set; }
        public string contrasteFacturacion { get; set; }

    }
}