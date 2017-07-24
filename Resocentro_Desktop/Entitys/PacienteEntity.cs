using Resocentro_Desktop.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public int codigoexamencita { get; set; }

        public int codigopaciente { get; set; }

        public string empresa { get; set; }
        public bool isCortesia { get; set; }
        public bool isDescuento { get; set; }
        public double por_descuento { get; set; }
        public double monto_descuento { get; set; }
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
    public class DetalleHCAdquisicion:INotifyPropertyChanged
    {
        private string _tecnologoName;
        public string tecnologoName
        {
            get { return this._tecnologoName; }
            set
            {
                if (value != _tecnologoName)
                {
                    this._tecnologoName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _placasuso;
        public string placasuso
        {
            get { return this._placasuso; }
            set
            {
                if (value != _placasuso)
                {
                    this._placasuso = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _equipo;
        public string equipo
        {
            get { return this._equipo; }
            set
            {
                if (value != _equipo)
                {
                    this._equipo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _enfermeraName;
        public string enfermeraName
        {
            get { return this._enfermeraName; }
            set
            {
                if (value != _enfermeraName)
                {
                    this._enfermeraName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _contraste;
        public string contraste
        {
            get { return this._contraste; }
            set
            {
                if (value != _contraste)
                {
                    this._contraste = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _anestesiologoName;
        public string anestesiologoName
        {
            get { return this._anestesiologoName; }
            set
            {
                if (value != _anestesiologoName)
                {
                    this._anestesiologoName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _tipoSedacion;
        public string tipoSedacion
        {
            get { return this._tipoSedacion; }
            set
            {
                if (value != _tipoSedacion)
                {
                    this._tipoSedacion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _motivoSedacion;
        public string motivoSedacion
        {
            get { return this._motivoSedacion; }
            set
            {
                if (value != _motivoSedacion)
                {
                    this._motivoSedacion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _postprocesadorName;
        public string postprocesadorName
        {
            get { return this._postprocesadorName; }
            set
            {
                if (value != _postprocesadorName)
                {
                    this._postprocesadorName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleHCAdquision_Tecnica> _tecnicasTecnologo;
        public List<DetalleHCAdquision_Tecnica> tecnicasTecnologo
        {
            get { return this._tecnicasTecnologo; }
            set
            {
                if (value != _tecnicasTecnologo)
                {
                    this._tecnicasTecnologo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleHCAdquision_Insumo> _insumoEnfermera;
        public List<DetalleHCAdquision_Insumo> insumoEnfermera
        {
            get { return this._insumoEnfermera; }
            set
            {
                if (value != _insumoEnfermera)
                {
                    this._insumoEnfermera = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleHCAdquision_Insumo> _insumoSedacion;
        public List<DetalleHCAdquision_Insumo> insumoSedacion
        {
            get { return this._insumoSedacion; }
            set
            {
                if (value != _insumoSedacion)
                {
                    this._insumoSedacion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleHCAdquision_PostProceso> _tecnicasPostproceso;
        public List<DetalleHCAdquision_PostProceso> tecnicasPostproceso
        {
            get { return this._tecnicasPostproceso; }
            set
            {
                if (value != _tecnicasPostproceso)
                {
                    this._tecnicasPostproceso = value;
                    NotifyPropertyChanged();
                }
            }
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class DetalleHCAdquision_Tecnica : INotifyPropertyChanged
    {
        private string fecha;
        public string Fecha
        {
            get { return this.fecha; }
            set
            {
                if (value != fecha)
                {
                    this.fecha = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string estudio;
        public string Estudio
        {
            get { return this.estudio; }
            set
            {
                if (value != estudio)
                {
                    this.estudio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string serie;
        public string Serie
        {
            get { return this.serie; }
            set
            {
                if (value != serie)
                {
                    this.serie = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int cant;
        public int Cant
        {
            get { return this.cant; }
            set
            {
                if (value != cant)
                {
                    this.cant = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class DetalleHCAdquision_Insumo : INotifyPropertyChanged
    {
        private string insumo;
        public string Insumo
        {
            get { return this.insumo; }
            set
            {
                if (value != insumo)
                {
                    this.insumo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string cant;
        public string Cant
        {
            get { return this.cant; }
            set
            {
                if (value != cant)
                {
                    this.cant = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string frasco;
        public string Frasco
        {
            get { return this.frasco; }
            set
            {
                if (value != frasco)
                {
                    this.frasco = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool aplicado;
        public bool Aplicado
        {
            get { return this.aplicado; }
            set
            {
                if (value != aplicado)
                {
                    this.aplicado = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class DetalleHCAdquision_PostProceso : INotifyPropertyChanged
    {
        private string fecha;
        public string Fecha
        {
            get { return this.fecha; }
            set
            {
                if (value != fecha)
                {
                    this.fecha = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string tecnologo;
        public string Tecnologo
        {
            get { return this.tecnologo; }
            set
            {
                if (value != tecnologo)
                {
                    this.tecnologo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string equipo;
        public string Equipo
        {
            get { return this.equipo; }
            set
            {
                if (value != equipo)
                {
                    this.equipo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string tecnica;
        public string Tecnica
        {
            get { return this.tecnica; }
            set
            {
                if (value != tecnica)
                {
                    this.tecnica = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string placa;
        public string Placa
        {
            get { return this.placa; }
            set
            {
                if (value != placa)
                {
                    this.placa = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
