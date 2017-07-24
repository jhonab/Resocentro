using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encuesta.ViewModels.Sedacion
{
    public class Sedacion_Insumo
    {
        public int idsedacion { get; set; }

        public string nom_sedacion { get; set; }

        //public double cantidad { get; set; }

        //public string unidad_medida { get; set; }

        public int subclase { get; set; }

        public string nomclase { get; set; }
    }

    public class Detalle_Sedacion_Insumo
    {
        public int iddetalle { get; set; }

        public string nom_insumo { get; set; }

        public double cantidad { get; set; }

        public string frasco { get; set; }

        //public string unidad_medida { get; set; }

        public bool aplicado { get; set; }

        public string tipoerroneo { get; set; }

    }
}