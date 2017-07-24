using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{
    public class PacienteEntity
    {
        public string nacionalidad { get; set; }
        public string direccion { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public string telefono { get; set; }
        public DateTime fechanace { get; set; }
        public string sexo { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string dni { get; set; }
        public int codigopaciente { get; set; }
        public string tipo_doc { get; set; }
        public bool IsProtocolo { get; set; }
        public string imgProtocolo { get { return IsProtocolo ? "../img/protocolo.png" : ""; } }
    }
    public class HistoriaClinicaPaciente
    {

        public List<HCCita> lstCita { get; set; }

        public List<HCAdmision> lstAdmision { get; set; }

        public List<HCCarta> lstCarta { get; set; }

        public List<HCAdquisicion> lstAdquisicion { get; set; }

        public List<HCPagos> lstPagos { get; set; }
    }
    public class HCCita
    {
        public string estado { get; set; }
        public bool vip { get; set; }
        public string imgVip { get { return vip ? "../img/vip.png" : ""; } }
        public bool sedacion { get; set; }
        public string imgSedacion { get { return sedacion ? "../img/sedacion.png" : ""; } }
        public int num_cita { get; set; }

        public DateTime fecha { get; set; }

        public string estudio { get; set; }

        public string precio { get; set; }

        public string cobertura { get; set; }

        public string equipo { get; set; }

        public string obs { get; set; }

        public string usuario { get; set; }

        public string clinica { get; set; }

        public string carta { get; set; }
        public bool isSeleccionado { get; set; }

        public DateTime fechacreacion { get; set; }

        public string codigoestudio { get; set; }

        public string codigoexamencita { get; set; }

        public int codigopaciente { get; set; }

        public string empresa { get; set; }
    }
    public class HCAdmision
    {
        public string estado { get; set; }
        public string ubicacion { get; set; }

        public DateTime fecha { get; set; }

        public int atencion { get; set; }

        public int examen { get; set; }

        public string estudio { get; set; }

        public string aseguradora { get; set; }

        public string prioridad { get; set; }

        public string medico { get; set; }

        public string turno { get; set; }

        public string usuario { get; set; }

        public int cita { get; set; }
        public bool isSeleccionado { get; set; }

        public string codigoestudio { get; set; }

        public int codigopaciente { get; set; }

        public string empresa { get; set; }
    }
    public class HCPagos
    {
        public string estado { get; set; }
        public string empresa { get; set; }

        public DateTime emision { get; set; }

        public DateTime pago { get; set; }

        public string documento { get; set; }

        public string numero { get; set; }

        public string total { get; set; }
        public string ruc { get; set; }

        public int cita { get; set; }
        public bool isSeleccionado { get; set; }

        public int codigopaciente { get; set; }

        public string codigoempresa { get; set; }

        public string codigosede { get; set; }

        public string pathFile { get; set; }
    }
    public class HCAdquisicion
    {
        public int examen { get; set; }
        public string estudio { get; set; }

        public int placas { get; set; }

        public bool sedacion { get; set; }

        public bool contraste { get; set; }

        public DateTime fecha { get; set; }

        public string tecnologo { get; set; }

        public string equipo { get; set; }

        public int cita { get; set; }

        public string informa { get; set; }

        public string revisa { get; set; }
        public bool isSeleccionado { get; set; }

        public string empresa { get; set; }
    }
    public class HCCarta
    {
        public string estado { get; set; }
        public DateTime fecha { get; set; }

        public string id { get; set; }

        public string id2 { get; set; }

        public string aseguradora { get; set; }

        public string cobertura { get; set; }

        public int cita { get; set; }
        public bool isSeleccionado { get; set; }

        public int codigopaciente { get; set; }

        public bool isRevisada { get; set; }
        public string imgRevisada
        {
            get
            {
                if (isRevisada)
                {
                    if (obs_revision == "")
                        return "../img/check.png";
                    else
                        return "../img/cancelar.png";
                }
                else
                    return "";
            }
        }
        public string obs_revision { get; set; }

        public string empresa { get; set; }
    }
    public class DetalleHCAdquisicion
    {

        public string tecnologoName { get; set; }

        public string placasuso { get; set; }
        public string equipo { get; set; }
        public string enfermeraName { get; set; }

        public string contraste { get; set; }

        public string anestesiologoName { get; set; }

        public string tipoSedacion { get; set; }

        public string motivoSedacion { get; set; }

        public string postprocesadorName { get; set; }

        public List<DetalleHCAdquision_Tecnica> tecnicasTecnologo { get; set; }

        public List<DetalleHCAdquision_Insumo> insumoEnfermera { get; set; }

        public List<DetalleHCAdquision_PostProceso> tecnicasPostproceso { get; set; }

    }
    public class DetalleHCAdquision_Tecnica
    {
        public string Fecha { get; set; }
        public string Estudio { get; set; }

        public string Serie { get; set; }

        public int Cant { get; set; }
    }
    public class DetalleHCAdquision_Insumo
    {
        public string Insumo { get; set; }
        public string Cant { get; set; }

        public string Frasco { get; set; }

        public bool Aplicado { get; set; }


    }
    public class DetalleHCAdquision_PostProceso
    {
        public string Fecha { get; set; }
        public string Tecnologo { get; set; }

        public string Equipo { get; set; }

        public string Tecnica { get; set; }

        public string Placa { get; set; }

    }
}
