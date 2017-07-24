using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaResocentro.ViewModel
{
    public class CobranzaViewModel
    {
        public string hora { get; set; }
        public int numeroatencion { get; set; }
        public int codigopaciente{ get; set; }
        public string paciente { get; set; }
        public int numerocita { get; set; }
    }
    public class DocumentoSunat
    {
        //1001
        public bool isOpeGravadas { get; set; }
        public double totalVenta_OpeGravadas { get; set; }
        //1002
        public bool isOpeInafectas { get; set; }
        public double totalVenta_OpeInafectas { get; set; }
        //1003
        public bool isOpeExoneradas { get; set; }
        public double totalVenta_OpeExoneradas { get; set; }
        //1004
        public bool isOpeGratuitas { get; set; }
        public double totalVenta_OpeGratuitas { get; set; }
        //2005
        public bool isTotalDescuento { get; set; }
        public double totalDescuento { get; set; }
        //1000
        public string totalVentaTexto { get; set; }


        public string numeroDocumento { get; set; }

        public DateTime fechaEmision { get; set; }

        public string tipoDocumento { get; set; }

        public string tipoMoneda { get; set; }
        public string idFirmaDigital { get; set; }
        public string tipoDocEmisor { get { return "6"; } }
        public string rucEmisor { get { return "20297451023"; } }
        public string razonSocialEmisor { get { return "RESONANCIA MEDICA S.R.LTDA."; } }
        public string ubigeoEmisor { get { return "150122"; } }
        public string calleEmisor { get { return "Av. Petit Thouars #4443"; } }
        public string urbanizacionEmisor { get { return ""; } }
        public string departamentoEmisor { get { return "LIMA"; } }
        public string provinciaEmisor { get { return "LIMA"; } }
        public string distritoEmisor { get { return "MIRAFLORES"; } }
        public string paisEmisor { get { return "PE"; } }

        //RECEPTOR
        public string tipoDocReceptor { get; set; }
        public string rucReceptor { get; set; }
        public string razonSocialReceptor { get; set; }
        public string emailReceptor { get; set; }
        public string celularReceptor { get; set; }
        public string telefonoReceptor { get; set; }
        public string ubigeoReceptor { get; set; }
        public string departamentoReceptor { get; set; }
        public string provinciaReceptor { get; set; }
        public string distritoReceptor { get; set; }
        public string direccionReceptor { get; set; }

        //ASEGURADORA
        public string razSocialAseguradora { get; set; }
        public string rucAseguradora { get; set; }
        public string tipodocAseguradora { get; set; }
        public string ubigeoAseguradora { get; set; }
        public string departamentoAseguradora { get; set; }
        public string provinciaAseguradora { get; set; }
        public string distritoAseguradora { get; set; }
        public string direccionAseguradora { get; set; }

        public double igvTotal { get; set; }
        public double igvPorcentaje { get; set; }
        public double montoTotal { get; set; }
        public double tipoCambio { get; set; }
        public List<DetalleDocumentoSunat> detalle { get; set; }

       
    }

    public class DetalleDocumentoSunat
    {
        public int cantidad { get; set; }
        public double igvUnitario { get; set; }
        public double valorUnitario { get { return Math.Round((valorUnitarioigv / (1 + igvUnitario)), 2); } }
        public double descxItem { get; set; }
        public double porcentajeCoaseguro { get; set; }
        public double descxCoaseguro { get; set; }       
        public double valorUnitarioigv { get; set; }
        public double valorVenta { get {
            if (porcentajeCoaseguro > 0)
                return Math.Round((descxCoaseguro * cantidad) - (descxItem), 2);
            else
                return  Math.Round((valorUnitario * cantidad) - (descxItem), 2);
        }
        }
        public double igvItem { get { return Math.Round((valorVenta * igvUnitario), 2); } }
        public double totalventa { get { return Math.Round((igvItem  +valorVenta), 2); } }

        public double porcentajeSeguro { get; set; }
        public double descxSeguro { get; set; }
        public double valorVentaSeguro
        {
            get
            {
                if (porcentajeSeguro > 0)
                    return Math.Round((descxSeguro * cantidad) , 2);
                else
                    return Math.Round((valorUnitario * cantidad), 2);
            }
        }
        public double igvItemSeguro { get { return Math.Round((valorVentaSeguro * igvUnitario), 2); } }
        public double totalventaSeguro { get { return Math.Round((valorVentaSeguro + igvItemSeguro), 2); } }


        public string descripcion { get; set; }
        public string codigoitem { get; set; }
        public int codigoexamencita { get; set; }

        public string simboloMoneda { get; set; }
    }

    public class GenerarDocumento
    {
        public DocumentoSunat encabezado { get; set; }
        public List<DetalleDocumentoSunat> detalle { get; set; }

        public double TCVenta { get; set; }

        public double TCCompra { get; set; }
    }

    public class EmpresaSunat
    {
        public string ruc { get; set; }

        public string razonsocial { get; set; }

        public string domicilio { get; set; }

        public string ubigeoId { get; set; }

        public string compania { get; set; }

        public int codigocompaniaseguro { get; set; }
    }
}