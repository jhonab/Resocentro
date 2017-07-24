using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class List_TicketeraViewModels
    {
        public int id_tiket { get; set; }
        public string nombre { get; set; }
        public string minutos { get; set; }

        public string tipo { get; set; }

        public int? estado { get; set; }

        public bool? isActivo { get; set; }

        public bool isPreferencial { get; set; }

        public bool isRecojo { get; set; }

        public bool isAtendiendo { get; set; }

        public string min_aten { get; set; }

        public bool isEmergencia { get; set; }
    }
    public class List_PacienteViewModels
    {
        public int idcounter { get; set; }

        public string nombre { get; set; }

        public bool? isActivo { get; set; }

        public List<TKT_Ticketera> lst_pac { get; set; }
    }
    public class Reporte_ViewModel
    {
        public List<spu_ReportexFecha_Ticketera_Result> general { get; set; }
        public List<spu_ReportePromedioxFecha_Ticketera_Result> promedio { get; set; }
    }
}