using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Supervisor
{
    public class SatisfaccionViewModel
    {
        public string nom_paciente { get; set; }
        public string min_transcurri { get; set; }

        public DateTime hora_admision { get; set; }

        public int atencion { get; set; }
    }
}