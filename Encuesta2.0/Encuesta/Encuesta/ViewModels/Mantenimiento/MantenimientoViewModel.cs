using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Encuesta.ViewModels.Mantenimiento
{
    public class MantenimientoViewModel
    {
        public int idIncidente { get; set; }
        public int idEquipo { get; set; }
        public EquipoMantenimiento equipo { get; set; }
        public int idTipoIncidente { get; set; }
        public string tipoIncidente { get; set; }
        public string usuario_detecta { get; set; }
        public DateTime fecha_detecta { get; set; }
        public string falla_detectada { get; set; }
        public bool isSolucionado { get; set; }
        public DateTime fecha_solucionado { get; set; }
        public string solucion { get; set; }
        public List<DetalleMantenimiento> listaRevisiones { get; set; }


        public string telefono { get; set; }
    }
    public class DetalleMantenimiento
    {
        public int idIncidente { get; set; }
        public int idEquipo { get; set; }
        public int idDetalleMantenimiento { get; set; }
        public DateTime fecha_revision { get; set; }
        public string revision { get; set; }
        public string usuario_revisa { get; set; }
        public bool isCambioPieza { get; set; }
        public bool isSolucionado { get; set; }
        public string piezas_array { get; set; }
        public List<PiezasMantenimiento> listaPiezas { get; set; }


        public string tecnico { get; set; }

        public string estadoequipo { get; set; }
    }
    public class PiezasMantenimiento
    {
        public int idPieza { get; set; }
        public string nombre { get; set; }
        public string serie { get; set; }
    }
    public class EquipoMantenimiento
    {
        public int idEquipo { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string serie { get; set; }
        public string nombreEquipo { get; set; }
        public string shortNombreequipo { get; set; }
        public string LongNombreequipo { get; set; }
        public DateTime fechaAdquirio { get; set; }
        public double intensidad { get; set; }
        public int idEmpresa { get; set; }
        public string empresa { get; set; }
        public int idSede { get; set; }
        public string sucursal { get; set; }
        public bool isSupervisable { get; set; }
        public int idModalidad { get; set; }
        public string modalidad { get; set; }
        public bool isActivo { get; set; }
        public bool isEliminado { get; set; }
        public bool isIntegrador { get; set; }
        public string aeTitle { get; set; }
        public string stationName { get; set; }
        public int añoFabricacion { get; set; }
        public int idfrecuencia { get; set; }
        public string frecuencia { get; set; }
        public int idEmpresaMantenimiento { get; set; }
        public EmpresaMantenimiento empresaMantenimiento { get; set; }
        public DateTime fecha_emision_licencia { get; set; }
        public DateTime fecha_vencimiento_licencia { get; set; }
        public DateTime fecha_proximo_mantenimiento { get; set; }
        public string estado_mantenimiento { get; set; }
    }
    public class EmpresaMantenimiento
    {
        public int idEmpresaMantenimiento { get; set; }
        public string razonsocial { get; set; }
        public string ruc { get; set; }
        public string correoSoporte { get; set; }
        public string correoMantenimiento { get; set; }
        public string telefonos_array { get; set; }
        public List<Telefono_EmpresaMantenimiento> telefonos { get; set; }

    }
    public class Telefono_EmpresaMantenimiento
    {
        public int idTelefono { get; set; }
        public string numero { get; set; }
        public int idTipoTelefono { get; set; }
        public string descripcion { get; set; }
    }
}