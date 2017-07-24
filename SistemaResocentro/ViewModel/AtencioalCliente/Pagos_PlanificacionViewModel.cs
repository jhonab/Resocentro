using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class Pagos_PlanificacionViewModel
    {
        public List<Models.Planificacion_Documento> lista { get; set; }

        public Planificacion_Documento items { get; set; }
    }

    public class Pagos_Planificacion_CompaniaseguroViewModel
    {
        public List<Models.Planificacion_CompaniaSeguro_Documento> lista { get; set; }

        public Planificacion_CompaniaSeguro_Documento items { get; set; }
    }
}