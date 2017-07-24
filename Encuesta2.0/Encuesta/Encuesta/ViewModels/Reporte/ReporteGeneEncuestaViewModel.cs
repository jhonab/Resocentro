using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Reporte
{
    public class ReporteGeneEncuestaViewModel
    {
        public int num_encuesta { get; set; }
        public string estudio { get; set; }
        public string paciente { get; set; }
        public string encuestador { get; set; }
        public string supervisor { get; set; }
        public string tecnologo { get; set; }

        public DateTime fecha { get; set; }

        public string clase { get; set; }

        public string sede { get; set; }

        public double FE_encuestador_Res { get; set; }

        public double min_res_encu { get; set; }

        public double min_res_super { get; set; }

        public double min_dur_encu { get; set; }

        public double FE_encuestador_Dur { get; set; }

        public double FE_supervisor_Res { get; set; }



        public double min_dur_tecno { get; set; }

        public double FE_tecnologo_Dur { get; set; }

        public Models.Encuesta encuesta { get; set; }
    }
}