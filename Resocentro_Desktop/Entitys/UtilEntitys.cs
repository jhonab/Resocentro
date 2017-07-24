using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    public class SucursalesxUsuario
    {
        public int codigoInt { get; set; }
        public string codigoString { get; set; }
        public string nombre { get; set; }
        public string nombreShort { get; set; }

        public int codigounidad { get; set; }

        public int codigosucursal { get; set; }
    }

    class ModalidadesxEmpresa
    {
        public int codigo { get; set; }
        public string nombre { get; set; }
        public string shortname { get; set; }
    }
}
