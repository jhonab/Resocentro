using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class SolicitudesEntreTurno
    {
        public int idSolicitud { get; set; }
        public DateTime hora { get; set; }
        public int tipo { get; set; }
        public string tipo_solicitud { get; set; }
        public string solicitante { get; set; }
        public string tramitador { get; set; }
        public string motivo { get; set; }
        public string comentarios { get; set; }
        public string equipo { get; set; }
        public int equipomedico { get; set; }
        public DateTime hora_medico { get; set; }
        public string comentarios_medico { get; set; }
    }
    public class Turno_Horario
    {
        public int codigohorario { get; set; }
        public DateTime hora { get; set; }
        public bool isBloqueado { get; set; }
        public string paciente { get; set; }
        public string estudio { get; set; }
        public string turno { get; set; }
    }
}