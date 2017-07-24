using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class RealizarLlamadaViewModel
    {
        public PACIENTE paciente { get; set; }

        public CARTAGARANTIA carta { get; set; }

        public string telefono { get; set; }

        public List<DetalleRealizarLlamada> llamadas { get; set; }
    }
    public class DetalleRealizarLlamada
    {
        public DateTime fecha { get; set; }
        public string usuario { get; set; }
        public string mensaje { get; set; }

        public string telefono { get; set; }
    }
}