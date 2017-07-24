using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encuesta.ViewModels
{
    public class Insumo
    {
        public int insumoid { get; set; }

        public string nombre { get; set; }

        public int subclase { get; set; }

        public string nomclase { get; set; }

    }

    public class DetalleInsumosViewModel {

        public int iddetalle { get; set; }

        public string nom_insumo { get; set; }

        public double cantidad { get; set; }

        public string frasco { get; set; }

        public bool aplicado { get; set; }

        public string tipoerroneo { get; set; }
    }
}
