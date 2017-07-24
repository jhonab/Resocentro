using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resocentro_Desktop.Entitys
{
    public class ResultadosEntregadosEntity
    {
        public DateTime fecha { get; set; }
        public DateTime fechasalida { get; set; }
        public string estudio { get; set; }
        public int codigo { get; set; }
        public string impreso { get; set; }
        public string entregado { get; set; }
        public string despacho { get; set; }

        public int codigodelivery { get; set; }

        public string paciente { get; set; }

        public int numerodelivery { get; set; }
    }
    public class ResultadosEntity
    {

        public DateTime fecha { get; set; }
        // public int codigo { get; set; }

        public string paciente { get; set; }

        // public string estudio { get; set; }

        public string dni { get; set; }

        public string telefono { get; set; }

        public string direccion { get; set; }

        public int numeroatencion { get; set; }

        public int codigopaciente { get; set; }
        public List<ListaEstudiosResultados> estudios { get; set; }

        public int placa { get; set; }
        public int foto { get; set; }
        public byte[] firma { get; set; }


        public string observaciones { get; set; }

        public string nombreestudios { get; set; }

        public int numerodelivery { get; set; }
    }
    public class ListaEstudiosResultados
    {
        public int codigo { get; set; }
        public string estudio { get; set; }
        public int placa { get; set; }
        public int foto { get; set; }
    }

}
