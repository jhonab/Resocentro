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

        public bool estado { get; set; }
    }

    public class Colaboradores
    {
        public string codigo { get; set; }
        public string valor { get; set; }
    }

    public class ReporteAtencionesMensuales : INotifyPropertyChanged
    {
        private string sede;
        public string Sede
        {
            get { return this.sede; }
            set
            {
                if (value != sede)
                {
                    this.sede = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime fecha = DateTime.Now;
        public DateTime Fecha
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

        private int codigo = 0;
        public int Codigo
        {
            get { return this.codigo; }
            set
            {
                if (value != codigo)
                {
                    this.codigo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string paciente;
        public string Paciente
        {
            get { return this.paciente; }
            set
            {
                if (value != paciente)
                {
                    this.paciente = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string tipopaciente;
        public string Tipopaciente
        {
            get { return this.tipopaciente; }
            set
            {
                if (value != tipopaciente)
                {
                    this.tipopaciente = value;
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

        public string ProcesoEstudio
        {
            get
            {
                if (estadoestudio == "A") return "Admitido";
                else if (estadoestudio == "R") return "Realizado";
                else if (estadoestudio == "P") return "Pagado";
                else if (estadoestudio == "I") return "Informado";
                else if (estadoestudio == "V") return "Validado";
                else if (estadoestudio == "N") return "Sin Informe";
                else return "-";
            }
        }

        private string aseguradora;
        public string Aseguradora
        {
            get { return this.aseguradora; }
            set
            {
                if (value != aseguradora)
                {
                    this.aseguradora = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double cobertura = 0;
        public double Cobertura
        {
            get { return this.cobertura; }
            set
            {
                if (value != cobertura)
                {
                    this.cobertura = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string documentos;
        public string Documentos
        {
            get { return this.documentos; }
            set
            {
                if (value != documentos)
                {
                    this.documentos = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string comentarios;
        public string Comentarios
        {
            get { return this.comentarios; }
            set
            {
                if (value != comentarios)
                {
                    this.comentarios = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string estadoestudio;
        public string Estadoestudio
        {
            get { return this.estadoestudio; }
            set
            {
                if (value != estadoestudio)
                {
                    this.estadoestudio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private PACIENTE datos_paciente = new PACIENTE();
        public PACIENTE Datos_paciente
        {
            get { return this.datos_paciente; }
            set
            {
                if (value != datos_paciente)
                {
                    this.datos_paciente = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DataCita datos_cita;
        public DataCita Datos_cita
        {
            get { return this.datos_cita; }
            set
            {
                if (value != datos_cita)
                {
                    this.datos_cita = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DataCarta datos_carta;
        public DataCarta Datos_carta
        {
            get { return this.datos_carta; }
            set
            {
                if (value != datos_carta)
                {
                    this.datos_carta = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DataAdmision datos_admision;
        public DataAdmision Datos_admision
        {
            get { return this.datos_admision; }
            set
            {
                if (value != datos_admision)
                {
                    this.datos_admision = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DataAdquisicion datos_adquisicion;
        public DataAdquisicion Datos_adquisicion
        {
            get { return this.datos_adquisicion; }
            set
            {
                if (value != datos_adquisicion)
                {
                    this.datos_adquisicion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DataDocumentos> datos_documentos;
        public List<DataDocumentos> Datos_documentos
        {
            get { return this.datos_documentos; }
            set
            {
                if (value != datos_documentos)
                {
                    this.datos_documentos = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int codigopaciente = 0;
        public int Codigopaciente
        {
            get { return this.codigopaciente; }
            set
            {
                if (value != codigopaciente)
                {
                    this.codigopaciente = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int numerocita = 0;
        public int Numerocita
        {
            get { return this.numerocita; }
            set
            {
                if (value != numerocita)
                {
                    this.numerocita = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int numeroatencion = 0;
        public int Numeroatencion
        {
            get { return this.numeroatencion; }
            set
            {
                if (value != numeroatencion)
                {
                    this.numeroatencion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int unidad = 0;
        public int Unidad
        {
            get { return this.unidad; }
            set
            {
                if (value != unidad)
                {
                    this.unidad = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private string codigoestudio;
        public string Codigoestudio
        {
            get { return this.codigoestudio; }
            set
            {
                if (value != codigoestudio)
                {
                    this.codigoestudio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string modalidad;
        public string Modalidad
        {
            get { return this.modalidad; }
            set
            {
                if (value != modalidad)
                {
                    this.modalidad = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string clase;
        public string Clase
        {
            get { return this.clase; }
            set
            {
                if (value != clase)
                {
                    this.clase = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _isBusy;
        public bool isBusy
        {
            get { return this._isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    this._isBusy = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string tiempo_final;
        public string Tiempo_final
        {
            get { return this.tiempo_final; }
            set
            {
                if (value != tiempo_final)
                {
                    this.tiempo_final = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<Procesos_Examen> list_proceso;
        public List<Procesos_Examen> List_proceso
        {
            get { return this.list_proceso; }
            set
            {
                if (value != list_proceso)
                {
                    this.list_proceso = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleHCAdquision_Insumo> list_insumos;
        public List<DetalleHCAdquision_Insumo> List_insumos
        {
            get { return this.list_insumos; }
            set
            {
                if (value != list_insumos)
                {
                    this.list_insumos = value;
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
    public class Procesos_Examen : INotifyPropertyChanged
    {
        private string proceso;
        public string Proceso
        {
            get { return this.proceso; }
            set
            {
                if (value != proceso)
                {
                    this.proceso = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string responsable;
        public string Responsable
        {
            get { return this.responsable; }
            set
            {
                if (value != responsable)
                {
                    this.responsable = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string num_ref;
        public string Num_ref
        {
            get { return this.num_ref; }
            set
            {
                if (value != num_ref)
                {
                    this.num_ref = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string incio;
        public string Incio
        {
            get { return this.incio; }
            set
            {
                if (value != incio)
                {
                    this.incio = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string fin;
        public string Fin
        {
            get { return this.fin; }
            set
            {
                if (value != fin)
                {
                    this.fin = value;
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
    public class DataCita : INotifyPropertyChanged
    {
        private string num_cita;
        public string Num_cita
        {
            get { return this.num_cita; }
            set
            {
                if (value != num_cita)
                {
                    this.num_cita = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string fecha_registro;
        public string Fecha_registro
        {
            get { return this.fecha_registro; }
            set
            {
                if (value != fecha_registro)
                {
                    this.fecha_registro = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string fecha_cita;
        public string Fecha_cita
        {
            get { return this.fecha_cita; }
            set
            {
                if (value != fecha_cita)
                {
                    this.fecha_cita = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string estado_cita;
        public string Estado_cita
        {
            get { return this.estado_cita; }
            set
            {
                if (value != estado_cita)
                {
                    this.estado_cita = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string usuario_registra;
        public string Usuario_registra
        {
            get { return this.usuario_registra; }
            set
            {
                if (value != usuario_registra)
                {
                    this.usuario_registra = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string observaciones_cita;
        public string Observaciones_cita
        {
            get { return this.observaciones_cita; }
            set
            {
                if (value != observaciones_cita)
                {
                    this.observaciones_cita = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string codigocarta;
        public string Codigocarta
        {
            get { return this.codigocarta; }
            set
            {
                if (value != codigocarta)
                {
                    this.codigocarta = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<DetalleDataCita> listaestudios;
        public List<DetalleDataCita> Listaestudios
        {
            get { return this.listaestudios; }
            set
            {
                if (value != listaestudios)
                {
                    this.listaestudios = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public class DetalleDataCita : INotifyPropertyChanged
        {
            private string codigoestudio;
            public string Codigoestudio
            {
                get { return this.codigoestudio; }
                set
                {
                    if (value != codigoestudio)
                    {
                        this.codigoestudio = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string nombreestudio;
            public string Nombreestudio
            {
                get { return this.nombreestudio; }
                set
                {
                    if (value != nombreestudio)
                    {
                        this.nombreestudio = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string horacita;
            public string Horacita
            {
                get { return this.horacita; }
                set
                {
                    if (value != horacita)
                    {
                        this.horacita = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string precioestudio;
            public string Precioestudio
            {
                get { return this.precioestudio; }
                set
                {
                    if (value != precioestudio)
                    {
                        this.precioestudio = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string estadoestudio;
            public string Estadoestudio
            {
                get { return this.estadoestudio; }
                set
                {
                    if (value != estadoestudio)
                    {
                        this.estadoestudio = value;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



    }

    public class DataAdmision : INotifyPropertyChanged
    {
        private string numero_admision;
        public string Numero_admision
        {
            get { return this.numero_admision; }
            set
            {
                if (value != numero_admision)
                {
                    this.numero_admision = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string fecha_admision;
        public string Fecha_admision
        {
            get { return this.fecha_admision; }
            set
            {
                if (value != fecha_admision)
                {
                    this.fecha_admision = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string peso_admision;
        public string Peso_admision
        {
            get { return this.peso_admision; }
            set
            {
                if (value != peso_admision)
                {
                    this.peso_admision = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string altura_admision;
        public string Altura_admision
        {
            get { return this.altura_admision; }
            set
            {
                if (value != altura_admision)
                {
                    this.altura_admision = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string usuario;
        public string Usuario
        {
            get { return this.usuario; }
            set
            {
                if (value != usuario)
                {
                    this.usuario = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool isAdjunto;
        public bool IsAdjunto
        {
            get { return this.isAdjunto; }
            set
            {
                if (value != isAdjunto)
                {
                    this.isAdjunto = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string ticket;
        public string Ticket
        {
            get { return this.ticket; }
            set
            {
                if (value != ticket)
                {
                    this.ticket = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string siteds;
        public string Siteds
        {
            get { return this.siteds; }
            set
            {
                if (value != siteds)
                {
                    this.siteds = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string nombrearchivo;
        public string Nombrearchivo
        {
            get { return this.nombrearchivo; }
            set
            {
                if (value != nombrearchivo)
                {
                    this.nombrearchivo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private byte[] cuerpoarchivo;
        public byte[] Cuerpoarchivo
        {
            get { return this.cuerpoarchivo; }
            set
            {
                if (value != cuerpoarchivo)
                {
                    this.cuerpoarchivo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string clinica;
        public string Clinica
        {
            get { return this.clinica; }
            set
            {
                if (value != clinica)
                {
                    this.clinica = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string medico;
        public string Medico
        {
            get { return this.medico; }
            set
            {
                if (value != medico)
                {
                    this.medico = value;
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
    public class DataCarta : INotifyPropertyChanged
    {
        private string _id_carta;
        public string id_carta
        {
            get { return this._id_carta; }
            set
            {
                if (value != _id_carta)
                {
                    this._id_carta = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _numero_carta;
        public string numero_carta
        {
            get { return this._numero_carta; }
            set
            {
                if (value != _numero_carta)
                {
                    this._numero_carta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _tramitado;
        public string tramitado
        {
            get { return this._tramitado; }
            set
            {
                if (value != _tramitado)
                {
                    this._tramitado = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _actualizado;
        public string actualizado
        {
            get { return this._actualizado; }
            set
            {
                if (value != _actualizado)
                {
                    this._actualizado = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _revisado;
        public string revisado
        {
            get { return this._revisado; }
            set
            {
                if (value != _revisado)
                {
                    this._revisado = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _aseguradora_carta;
        public string aseguradora_carta
        {
            get { return this._aseguradora_carta; }
            set
            {
                if (value != _aseguradora_carta)
                {
                    this._aseguradora_carta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _observaciones_cara;
        public string observaciones_cara
        {
            get { return this._observaciones_cara; }
            set
            {
                if (value != _observaciones_cara)
                {
                    this._observaciones_cara = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<DetalleDataCarta> _lista_estudios_carta;
        public List<DetalleDataCarta> lista_estudios_carta
        {
            get { return this._lista_estudios_carta; }
            set
            {
                if (value != _lista_estudios_carta)
                {
                    this._lista_estudios_carta = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<Adjuntos_Desktop> _lista_adjuntos_carta;
        public List<Adjuntos_Desktop> lista_adjuntos_carta
        {
            get { return this._lista_adjuntos_carta; }
            set
            {
                if (value != _lista_adjuntos_carta)
                {
                    this._lista_adjuntos_carta = value;
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
    public class DetalleDataCarta : INotifyPropertyChanged
    {
        private string _estudio;
        public string estudio
        {
            get { return this._estudio; }
            set
            {
                if (value != _estudio)
                {
                    this._estudio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _cobertura;
        public double cobertura
        {
            get { return this._cobertura; }
            set
            {
                if (value != _cobertura)
                {
                    this._cobertura = value;
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
    public class DataAdquisicion : INotifyPropertyChanged
    {
        private string _estudio;
        public string estudio
        {
            get { return this._estudio; }
            set
            {
                if (value != _estudio)
                {
                    this._estudio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _encuestador;
        public string encuestador
        {
            get { return this._encuestador; }
            set
            {
                if (value != _encuestador)
                {
                    this._encuestador = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ini_encuestador;
        public string ini_encuestador
        {
            get { return this._ini_encuestador; }
            set
            {
                if (value != _ini_encuestador)
                {
                    this._ini_encuestador = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_encuestador;
        public string fin_encuestador
        {
            get { return this._fin_encuestador; }
            set
            {
                if (value != _fin_encuestador)
                {
                    this._fin_encuestador = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _tecnologo;
        public string tecnologo
        {
            get { return this._tecnologo; }
            set
            {
                if (value != _tecnologo)
                {
                    this._tecnologo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ini_tecnologo;
        public string ini_tecnologo
        {
            get { return this._ini_tecnologo; }
            set
            {
                if (value != _ini_tecnologo)
                {
                    this._ini_tecnologo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_tecnologo;
        public string fin_tecnologo
        {
            get { return this._fin_tecnologo; }
            set
            {
                if (value != _fin_tecnologo)
                {
                    this._fin_tecnologo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _supervisor;
        public string supervisor
        {
            get { return this._supervisor; }
            set
            {
                if (value != _supervisor)
                {
                    this._supervisor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ini_supervisor;
        public string ini_supervisor
        {
            get { return this._ini_supervisor; }
            set
            {
                if (value != _ini_supervisor)
                {
                    this._ini_supervisor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_supervisor;
        public string fin_supervisor
        {
            get { return this._fin_supervisor; }
            set
            {
                if (value != _fin_supervisor)
                {
                    this._fin_supervisor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _informante;
        public string informante
        {
            get { return this._informante; }
            set
            {
                if (value != _informante)
                {
                    this._informante = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_informante;
        public string fin_informante
        {
            get { return this._fin_informante; }
            set
            {
                if (value != _fin_informante)
                {
                    this._fin_informante = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _validador;
        public string validador
        {
            get { return this._validador; }
            set
            {
                if (value != _validador)
                {
                    this._validador = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_validador;
        public string fin_validador
        {
            get { return this._fin_validador; }
            set
            {
                if (value != _fin_validador)
                {
                    this._fin_validador = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _impresion;
        public string impresion
        {
            get { return this._impresion; }
            set
            {
                if (value != _impresion)
                {
                    this._impresion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _fin_impresion;
        public string fin_impresion
        {
            get { return this._fin_impresion; }
            set
            {
                if (value != _fin_impresion)
                {
                    this._fin_impresion = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DetalleHCAdquisicion _detalleAdquisicion;
        public DetalleHCAdquisicion detalleAdquisicion
        {
            get { return this._detalleAdquisicion; }
            set
            {
                if (value != _detalleAdquisicion)
                {
                    this._detalleAdquisicion = value;
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
    public class DataDocumentos : INotifyPropertyChanged
    {
        private string _numero_documento;
        public string Numero_documento
        {
            get { return this._numero_documento; }
            set
            {
                if (value != _numero_documento)
                {
                    this._numero_documento = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _estado;
        public string Estado
        {
            get { return this._estado; }
            set
            {
                if (value != _estado)
                {
                    this._estado = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _subtotal;
        public double Subtotal
        {
            get { return this._subtotal; }
            set
            {
                if (value != _subtotal)
                {
                    this._subtotal = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _igv;
        public double Igv
        {
            get { return this._igv; }
            set
            {
                if (value != _igv)
                {
                    this._igv = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _total;
        public double Total
        {
            get { return this._total; }
            set
            {
                if (value != _total)
                {
                    this._total = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _path;
        public string Path
        {
            get { return this._path; }
            set
            {
                if (value != _path)
                {
                    this._path = value;
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

    public class ReporteDocumentosMensuales
    {
        public double tipocambio { get; set; }
        public string documento { get; set; }
        public int pagos { get; set; }
        public double montopagos { get; set; }
        public double saldo { get; set; }
        public List<PagosDocumento> listapagos { get; set; }

        public string sede { get; set; }

        public string tipodocumento { get; set; }

        public string codigomonedadoc { get; set; }

        public string monedadoc { get; set; }

        public double total { get; set; }

        public string usuariodoc { get; set; }

        public string razonsocial { get; set; }

        public string estado { get; set; }

        public DateTime fechadoc { get; set; }

        public string pdf { get; set; }

        public string paciente { get; set; }

        public string serie { get; set; }
    }
    public class PagosDocumento
    {


        public DateTime? fechapago { get; set; }
        public double tipocambio { get; set; }
        public string codigomonedapago { get; set; }

        public string tarjeta { get; set; }

        public string monedapago { get; set; }

        public double montopago { get; set; }

        public string usuariopago { get; set; }
    }

    public class CabeceraDocumentoOracle
    {
        public DateTime fecha { get; set; }
        public string num_doc { get; set; }
        public double subtotal { get; set; }
        public double igv { get; set; }
        public double total { get; set; }
        public double subtotal_dolares { get; set; }
        public double igv_dolares { get; set; }
        public double total_dolares { get; set; }
        public int estado { get; set; }
        public string estado_string { get { return estado == 1 ? "PAGADO" : "ANULADO"; } }
        public int moneda { get; set; }
        public string moneda_string { get { return moneda == 1 ? "DOLARES" : "SOLES"; } }
        public string tipodocumento { get; set; }
        public string tipodocumento_string
        {
            get
            {
                switch (tipodocumento)
                {
                    case "01": return "FACTURA";
                    case "03": return "BOLETA";
                    case "07": return "NOTA DE CREDITO";
                    case "08": return "NOTA DE DEBITO";
                    default: return "-";
                }
            }
        }
        public double tipo_cambio { get; set; }
        public int empresa { get; set; }
        public string razonsocial { get; set; }
        public string ruc { get; set; }

        public string serie { get; set; }

        public int correlativo
        {
            get
            {
                var split = num_doc.Split('-');
                if (split.Count() == 2)
                    if (split[1] != null)
                        return int.Parse(split[1]);

                return 0;
            }
        }
    }
    public class DetalleDocumentoOracle
    {
        public string num_doc { get; set; }

        public string descripcion { get; set; }

        public double precio { get; set; }
        public double total { get { return precio * 1.18; } }

        public string tipodocumento { get; set; }

        public string registro_ventas { get; set; }

        public double precio_dol { get; set; }
    }

    public class DiscordanciaSQLOracle
    {

        public string NDOC { get; set; }

        public string ERROR { get; set; }

        public decimal? MONTO_SQL { get; set; }

        public double MONTO_ORACLE { get; set; }
    }

    public class AsignacionHorarioTecnologo
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }

        public DateTime incio { get; set; }

        public DateTime fin { get; set; }

        public int codigoequipo { get; set; }

        public string nombreequipo { get; set; }

        public string codigotecnologo { get; set; }

        public string nombretecnologo { get; set; }

        public int turno { get { return 1; } }
        public double horas
        {
            get
            {
                TimeSpan tps = fin - incio;
                return Math.Round(tps.Hours + (Math.Round((tps.Minutes * 1.0) / 60, 2)), 2);
            }
        }

    }
}
