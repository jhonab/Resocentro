using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    public class AsignacionSedaciones
    {
        public string sedador { get; set; }

        public DateTime fecha { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string estudio { get; set; }

        public int detallecita { get; set; }

        public bool existe { get; set; }
    }

    public class AsignacionInsumos
    {
        public int idinsumo { get; set; }

        public string nombre { get; set; }

        public string comentario { get; set; }

        public string correlativo { get; set; }

        public bool  estado { get; set; }
    }

    public class Colaboradores
    {
        public string codigo { get; set; }
        public string valor { get; set; }
    }
}
