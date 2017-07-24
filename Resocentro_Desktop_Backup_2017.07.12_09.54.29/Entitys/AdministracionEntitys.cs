using Resocentro_Desktop.DAO;
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

        public bool estado { get; set; }
    }

    public class Colaboradores
    {
        public string codigo { get; set; }
        public string valor { get; set; }
    }

    public class ReporteAtencionesMensuales
    {

        public string sede { get; set; }

        public DateTime fecha { get; set; }

        public int codigo { get; set; }

        public string paciente { get; set; }

        public string tipopaciente { get; set; }

        public string estudio { get; set; }

        public string aseguradora { get; set; }

        public double cobertura { get; set; }

        public string documentos { get; set; }

        public string comentarios { get; set; }

        public string estadoestudio { get; set; }

        public bool dataLoaded { get; set; }
        public PACIENTE datos_paciente { get; set; }
        public DataCita datos_cita { get; set; }
        public DataCarta datos_carta { get; set; }
        public DataAdmision datos_admision { get; set; }
        public DataAdquisicion datos_adquisicion { get; set; }
        public List<DataDocumentos> datos_documentos { get; set; }

        public int codigopaciente { get; set; }

        public int numerocita { get; set; }

        public int numeroatencion { get; set; }

        public int unidad { get; set; }
    }
    public class DataCita
    {
        public string num_cita { get; set; }
        public string fecha_cita { get; set; }
        public string estado_cita { get; set; }
        public string usuario_registra { get; set; }
        public string observaciones_cita { get; set; }
    }
    public class DataAdmision
    {
        public string numero_admision { get; set; }
        public string fecha_admision { get; set; }
        public string peso_admision { get; set; }
        public string altura_admision { get; set; }
    }
    public class DataCarta
    {
        public string id_carta { get; set; }
        public string numero_carta { get; set; }
        public string aseguradora_carta { get; set; }
        public List<DetalleDataCarta> lista_estudios_carta { get; set; }
        public string observaciones_cara { get; set; }
    }
    public class DetalleDataCarta
    {
        public string estudio { get; set; }
        public double cobertura { get; set; }
    }
    public class DataAdquisicion
    {
        public string estudio { get; set; }
        public string encuestador { get; set; }
        public string tecnologo { get; set; }
        public string supervisor { get; set; }
        public string informante { get; set; }
        public string validador { get; set; }
    }
    public class DataDocumentos
    {
        public string numero_documento { get; set; }
        public string estado { get; set; }
        public double subtotal { get; set; }
        public double igv { get; set; }
        public double total { get; set; }
        public string path { get; set; }
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
