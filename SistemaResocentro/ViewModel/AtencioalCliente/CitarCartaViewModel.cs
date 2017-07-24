using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class CitarCartaViewModel
    {
        public CITA cita { get; set; }

        public MEDICOEXTERNO medico { get; set; }

        public string sede { get; set; }

        public List<EXAMENXCITA> estudios { get; set; }

        public CARTAGARANTIA carta { get; set; }

        public int idsede { get; set; }
    }
    public class DetalleCitarCarta
    {
        public string codigoestudio { get; set; }
        public int codigoclase { get; set; }
        public string nombreestudio { get; set; }
        public int codigomoneda { get; set; }
        public double precioestudio { get; set; }
        public int codigoequipo { get; set; }
        public string equipo { get; set; }
        public int idhorario { get; set; }
        public string horacita { get; set; }
    }



}