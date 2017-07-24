using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resocentro_Desktop.Entitys
{
    public class ResultadosEntity
    {
        public DateTime fecha { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public string dni { get; set; }

        public string telefono { get; set; }

        public string direccion { get; set; }

        public int numeroatencion { get; set; }

        public int codigopaciente { get; set; }
    }
}
