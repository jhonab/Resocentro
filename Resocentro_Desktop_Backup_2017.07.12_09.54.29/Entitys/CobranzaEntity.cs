using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resocentro_Desktop.Entitys
{

    public class CobranzaViewModel
    {
        public string hora { get; set; }
        public int numeroatencion { get; set; }
        public int codigopaciente { get; set; }
        public string paciente { get; set; }
        public int numerocita { get; set; }
        public int estado { get; set; }
        public string pathEstado
        {
            get
            {
                switch (estado)
                {
                    case 0:
                    case 1:
                    case 2:
                        return "";
                    case 3:
                    case 4:
                        return "../../img/estadoEncuesta/check.png";
                    default: return "";
                }

            }
        }
    }
    public class DocumentoSunat
    {
        /// <summary>
        /// asigna o devuelve si es un documento de anticipo
        /// </summary>
        public bool isAnticipo { get; set; }
        //1001
        /// <summary>
        /// asigna o devuelve si hay operaciones gravadas(1001)
        /// </summary>
        public bool isOpeGravadas { get; set; }
        /// <summary>
        /// asigna o devuelve el total ventas gravadas(1001)
        /// </summary>
        public double totalVenta_OpeGravadas { get; set; }
        /// <summary>
        /// asigna o devuelve el descuento total ventas gravadas(1001)
        /// </summary>
        public double desc_TotalVenta_OpeGravadas { get; set; }
        /// <summary>
        /// asigna o devuelve si hay operaciones inafectas(1002)
        /// </summary>
        public bool isOpeInafectas { get; set; }
        /// <summary>
        /// asigna o devuelve el total de ventas inafectas(1002)
        /// </summary>
        public double totalVenta_OpeInafectas { get; set; }
        /// <summary>
        /// asigna o devuelve el descuento total ventas inafectas(1002)
        /// </summary>
        public double desc_TotalVenta_OpeInafectas { get; set; }
        /// <summary>
        /// asigna o devuelve si hay operaciones exoneradas(1003)
        /// </summary>
        public bool isOpeExoneradas { get; set; }
        /// <summary>
        /// asigna o devuelve el total de ventas exoneradas(1003)
        /// </summary>
        public double totalVenta_OpeExoneradas { get; set; }
        /// <summary>
        /// asigna o devuelve el descuento total de ventas exoneradas(1003)
        /// </summary>
        public double desc_TotalVenta_OpeExoneradas { get; set; }
        /// <summary>
        /// asigna o devuelve si hay operaciones gratuitas(1004)
        /// </summary>
        public bool isOpeGratuitas { get; set; }
        /// <summary>
        /// asigna o devuelve el total de ventas gratuitas(1004)
        /// </summary> 
        public double totalVenta_OpeGratuitas { get; set; }
        /// <summary>
        /// asigna o devuelve el total de descuentos (2005)
        /// </summary>
        public bool isTotalDescuento { get; set; }
        /// <summary>
        /// asigna o devuelve el total de descuentos(2005)
        /// </summary>
        public double totalDescuento { get; set; }
        /// <summary>
        /// return total de ventas en texto (1000)
        /// </summary>
        public string totalVentaTexto { get { return new CobranzaDAO().enletras(ventaTotal.ToString()); } }
        /// <summary>
        /// asigna o devuelve el numero del documento
        /// </summary>
        public string numeroDocumento { get; set; }
        public string numeroDocumentoSUNAT { get; set; }
        /// <summary>
        /// asigna o devuelve la fecha de emision del documento
        /// </summary>
        public DateTime fechaEmision { get; set; }
        /// <summary>
        /// asigna o devuelve el codigo de la sede
        /// </summary>
        public string codigosede { get; set; }
        /// <summary>
        /// asigna o devuelve la direccion de la sede
        /// </summary>
        public string direccionsede { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de documento(int) segun formato de SUNAT
        /// </summary>
        public int tipoDocumento { get; set; }

        /// <summary>
        /// devuelve el nombre del tipo de documento
        /// </summary>
        public string nombretipoDocumento
        {
            get
            {
                switch (tipoDocumento)
                {
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA:
                        return "FACTURA ELECTRÓNICA";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA:
                        return "BOLETA ELECTRÓNICA";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO:
                        return "NOTA DE CREDITOELECTRÓNICA";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO:
                        return "NOTA DE DEBITO ELECTRÓNICA";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.GUIA_REMISION_REMITENTE:
                        return "GUIA DE REMISION";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS:
                        return "RESUMEN DIARIO DE BOLETAS";
                    case (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA:
                        return "COMUNICACIÓN DE BAJA";
                    default:
                        return "DOCUMENTO ELECTRÓNICA";
                }
            }
        }
        /// <summary>
        /// asigna o devuelve codigo de moneda
        /// </summary>
        public int codigoMoneda { get; set; }
        /// <summary>
        /// return Tipo de moneda en String (PEN,USD)
        /// </summary>
        public string tipoMoneda
        {
            get
            {
                switch (codigoMoneda)
                {
                    case 1:
                        return "PEN";
                    case 2:
                        return "USD";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// return Moneda para impresion de comprobantes de venta(SOLES,DOLARES)
        /// </summary>
        public string tipoMonedaImpresion
        {
            get
            {
                switch (codigoMoneda)
                {
                    case 1:
                        return "SOLES";
                    case 2:
                        return "DOLARES AMERICANOS";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// return Simbolo Moneda para impresion de comprobantes de venta(S/.,$)
        /// </summary>
        public string simboloMonedaImpresion
        {
            get
            {
                switch (codigoMoneda)
                {
                    case 1:
                        return "S/ ";
                    case 2:
                        return "$ ";
                    default:
                        return "";
                }
            }
        }

        //EMISOR
        public int empresa { get; set; }
        public int sede { get; set; }
        /// <summary>
        /// asigna o devuelve el ID para la firma electronica
        /// </summary>
        public string idFirmaDigital { get; set; }
        /// <summary>
        /// devuelve el tipo de documento del emisor (int) segun SUNAT
        /// </summary>
        public string tipoDocEmisor { get { return ((int)TIPO_DOCUMENTOIDENTIDAD.RUC).ToString(); } }
        /// <summary>
        /// return RUC del emisor
        /// </summary>
        public string resolucionIntendencia
        {
            get
            {
                if (empresa == 1)//RESONANCIA
                    return "0320050000440";
                else//EMETAC
                    return "0320050000463";
            }
        }
        /// <summary>
        /// return RUC del emisor
        /// </summary>
        public string rucEmisor
        {
            get
            {
                if (empresa == 1)//RESONANCIA
                    return "20297451023";
                else//EMETAC
                    return "20101849831";
            }
        }
        /// <summary>
        /// return razon social del emisor
        /// </summary>
        public string razonSocialEmisor
        {
            get
            {
                if (empresa == 1)
                    return "RESONANCIA MEDICA S.R.LTDA.";
                else
                    return "EMETAC S.A.C.";
            }
        }
        /// <summary>
        /// return codigo de ubigeo del emisor segun INEI
        /// </summary>
        public string ubigeoEmisor { get { return "150122"; } }
        /// <summary>
        /// return direccion del emisor
        /// </summary>
        public string calleEmisor
        {
            get
            {
                if (empresa == 1)
                    return "Av. PETIT THOUARS NRO. 4443";
                else
                    return "AV. PETIT THOUARS NRO. 4350 INT. 101";
            }
        }
        /// <summary>
        /// return urbanzacion del emisor
        /// </summary>
        public string urbanizacionEmisor { get { return ""; } }
        /// <summary>
        /// return departamento del emisor
        /// </summary>
        public string departamentoEmisor { get { return "LIMA"; } }
        /// <summary>
        /// return provincia del emisor
        /// </summary>
        public string provinciaEmisor { get { return "LIMA"; } }
        /// <summary>
        /// return distrito del emiso
        /// </summary>
        public string distritoEmisor { get { return "MIRAFLORES"; } }
        /// <summary>
        /// return pais del emiso
        /// </summary>
        public string paisEmisor { get { return "PE"; } }
        /* public string rucEmisor { get { return "20101849831"; } }
         public string razonSocialEmisor { get { return "EMETAC E.I.R.L."; } }
         public string ubigeoEmisor { get { return "150122"; } }
         public string calleEmisor { get { return "Av. Petit Thouars #4350"; } }
         public string urbanizacionEmisor { get { return ""; } }
         public string departamentoEmisor { get { return "LIMA"; } }
         public string provinciaEmisor { get { return "LIMA"; } }
         public string distritoEmisor { get { return "MIRAFLORES"; } }
         public string paisEmisor { get { return "PE"; } }*/

        //RECEPTOR
        /// <summary>
        /// asigna o devuelveel tipo de documento de recepto (int) segun SUNAT
        /// </summary>
        public int tipoDocReceptor { get; set; }
        /// <summary>
        /// deveuelve el tipo de documento  concatenado con el numero de documento en String
        /// </summary>
        public string documentoReceptorString
        {
            get
            {
                switch ((TIPO_DOCUMENTOIDENTIDAD)tipoDocReceptor)
                {
                    case TIPO_DOCUMENTOIDENTIDAD.DOC_TRIB_NO_DOM_SIN_RUC:
                        return "DOC TRIB NO DOM SIN RUC: " + rucReceptor;
                    case TIPO_DOCUMENTOIDENTIDAD.DNI:
                        return "DNI: " + rucReceptor;
                    case TIPO_DOCUMENTOIDENTIDAD.CARNET_EXTRANJERIA:
                        return "CARNET DE EXTRANJERIA: " + rucReceptor;
                    case TIPO_DOCUMENTOIDENTIDAD.RUC:
                        return "RUC: " + rucReceptor;
                    case TIPO_DOCUMENTOIDENTIDAD.PASAPORTE:
                        return "PASAPORTE: " + rucReceptor;
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// deveuelve el tipo de documento  en String
        /// </summary>
        public string tipoDocumentoEmitidoString
        {
            get
            {
                switch ((TIPO_DOCUMENTO_ELECTRONICO)tipoDocumento)
                {
                    case TIPO_DOCUMENTO_ELECTRONICO.FACTURA:
                        return "FACTURA";
                    case TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA:
                        return "BOLETA DE VENTA";
                    case TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO:
                        return "NOTA DE CREDITO";
                    case TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO:
                        return "NOTA DE DEBITO";
                    case TIPO_DOCUMENTO_ELECTRONICO.GUIA_REMISION_REMITENTE:
                        return "GUIA DE REMISION REMITENTE";
                    case TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS:
                        return "RESUMEN DIARIO DE BOLETAS";
                    case TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA:
                        return "COMUNICACION DE BAJA";
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// asigna o devuelve el ruc del receptop
        /// </summary>
        public string rucReceptor { get; set; }
        /// <summary>
        /// asigna o devuelve la razon social del receptor
        /// </summary>
        public string razonSocialReceptor { get; set; }
        /// <summary>
        /// asigna o devuelve el Email del receptor
        /// </summary>
        public string emailReceptor { get; set; }
        /// <summary>
        /// asigna o devuelve la direccion del receptor
        /// </summary>
        public string direccionReceptor { get; set; }
        /// <summary>
        /// asigna o devuelve el monto total del IGV
        /// </summary>
        public double igvTotal { get; set; }
        /// <summary>
        /// asigna o devuelve el IGV del dia Ejem (0.18)
        /// </summary>
        public double igvPorcentaje { get; set; }
        /// <summary>
        /// asigna o devuelve el porcentaje (%) de descuento global 
        /// </summary>
        public double porcentajeDescuentoGlobal { get; set; }
        /// <summary>
        /// asigna o devuelve el valor total de descuento
        /// </summary>
        public double descuentoGlobal { get; set; }
        /// <summary>
        /// asigna o devuelve el valor total en SOLES
        /// </summary>
        public double ventaTotal { get; set; }
        /// <summary>
        /// Lista de Items a Cobrar
        /// </summary>
        public List<DetalleDocumentoSunat> detalleItems { get; set; }
        /// <summary>
        /// lista de Items a Cancelar
        /// </summary>
        public List<DocumentosxCancelar> detalleCancelar { get; set; }
        /// <summary>
        /// Lista de items a Rendir segun Boleta
        /// </summary>
        public List<DetalleResumenBoleta> detalleResumenBoleta { get; set; }

        //NOTA CREDITO DEBITO
        /// <summary>
        /// asigna o devuelve el numero de documento de referencia que se asigna a la nota de credito/debito
        /// </summary>
        public string numeroDocumentoReferencia { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de documento de referencia que se asigna a la nota de credito/debito
        /// </summary>
        public int tipoDocumentoReferencia { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de motivo por el cual se creara la nota de credito/debito
        /// </summary>
        public int tipoNotaCreditoDebito { get; set; }
        /// <summary>
        /// asigna o devuelve la descripcion de la nota de credito/debito
        /// </summary>
        public string descripcionNotaCreditoDebito { get; set; }
        /// <summary>
        /// asigna o devuelve la fecha de referencia  del documento de la nota de credito/debito
        /// </summary>
        public string fechaReferenciaDocumento { get; set; }
        /// <summary>
        /// asigna o devuelve el codigo de seguimiento web
        /// </summary>
        public string codigoSegWeb { get; set; }

        /// <summary>
        /// asigna o devuelve numero de atencion
        /// </summary>
        public int numeroatencion { get; set; }
        /// <summary>
        /// asigna o devuelve el codigo de la compañia de seguro
        /// </summary>
        public int codigocompaniaseguro { get; set; }
        /// <summary>
        /// asigna o devuelve el subtotal de la venta
        /// </summary>
        public double subTotal { get; set; }
        /// <summary>
        /// asigna o devuelve el codigo del paciente
        /// </summary>
        public int codigopaciente { get; set; }
        public string paciente { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de cambio venta del dia
        /// </summary>
        public double TCVenta { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de cambio compra del dia
        /// </summary>
        public double TCCompra { get; set; }

        /// <summary>
        /// asigna o devuelve observaciones del cita y carta
        /// </summary>
        public string observaciones { get; set; }
        /// <summary>
        /// asigna o devuelve el nombre de la compañia de seguro
        /// </summary>
        public string aseguradora { get; set; }
        /// <summary>
        /// return cobertura de carta
        /// </summary>
        public string infoCarta { get; set; }
        /// <summary>
        /// asigna o devuelve el nombre del titular de la carta
        /// </summary>
        public string titular_Carta { get; set; }
        /// <summary>
        /// asigna o devuelve el nombre de la empresa/contratante de la carta
        /// </summary>
        public string contratante_carta { get; set; }
        /// <summary>
        /// asigna o devuelve el numero de la poliza/numero de carnet de la carta
        /// </summary>
        public string poliza_carta { get; set; }
        /// <summary>
        /// asigna o devuelve el texto para generar el  codigo PDF$!/
        /// </summary>
        public string codigoBarraPDF417String { get; set; }
        /// <summary>
        /// asigna o devuelve la ruta donde se guarda el PDF417
        /// </summary>
        public string pathCODEBAR { get; set; }


        public double cobertura_carta { get; set; }

        public int numerocita { get; set; }

        public string cartascorelacionadas { get; set; }

        public string cmp { get; set; }

        public string aseguradora_ruc { get; set; }

        public int codigomodalidad { get; set; }

        public string dnipaciente { get; set; }

        public string numerocarnet_carta { get; set; }
        /// <summary>
        /// ruta donde se guardo el documento en pdf
        /// </summary>
        public string pathPDF { get; set; }

        public bool iscobranzaExterna { get; set; }

        public string textoinformacion { get; set; }

        public string carta_Coaseguro { get; set; }

        public string carta { get; set; }

        public bool hasCoaseguro { get; set; }

        public bool isPrintSegOnline { get; set; }

        public bool isSendSUNAT { get; set; }

        public List<int> atenciones { get; set; }


        public bool isFacGlobal { get; set; }
    }
    public class DetalleDocumentoSunat
    {
        private Tool tool = new Tool();
        /// <summary>
        /// asigna o devuelve el tipo de documento de referencia que se asigna a la nota de credito/debito
        /// </summary>
        public int tipoDocumentoReferencia { get; set; }
        /// <summary>
        /// tipo de cobranza a realizar Paciente o Aseguradora
        /// </summary>
        public int tipo_cobranza { get; set; }
        /// <summary>
        /// asigna o devuelve el tipo de documento(int) segun formato de SUNAT
        /// </summary>
        public int tipo_documento { get; set; }
        private int _tipoIGV { get; set; }
        /// <summary>
        /// Tipo de IGV 
        /// </summary>
        public int tipoIGV
        {
            get
            {
                //TECNICAS NO SE DECLARAN
                if (codigoitem.Substring(7, 2) == "99" && codigoitem.Substring(5, 1) == "0")
                {
                    if (valorUnitario == 0 && valorReferencial == 0)
                        return (int)TIPO_IGV.NO_DECLARAR_SUNAT;
                    else
                        return _tipoIGV;
                }
                else
                {
                    return _tipoIGV;
                }

            }
            set
            {
                _tipoIGV = value;
            }
        }
        /// <summary>
        /// cantidad de Items
        /// </summary>
        public int cantidad { get; set; }
        /// <summary>
        /// descripcion del item
        /// </summary>
        public string descripcion { get; set; }
        /// <summary>
        /// valor del Igv ejm (0.18)
        /// </summary>
        public double valorIgvActual { get; set; }//valor del igv
        /// <summary>
        /// valor referencial del item incluido IGV
        /// </summary>
        public double valorReferencialIGV { get; set; }
        /// <summary>
        /// valor referencial del item sin IGV
        /// </summary>
        public double valorReferencial
        {
            get
            {
                return Math.Round((valorReferencialIGV / (1 + valorIgvActual)), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
            }
        }
        private double _valorUnitarioigv;
        /// <summary>
        /// valor unitario del item incluido IGV
        /// </summary>
        public double valorUnitarioigv
        {
            get
            {
                if (isGratuita)
                    return 0;
                else
                    return _valorUnitarioigv;
            }
            set
            {
                _valorUnitarioigv = value;
            }
        }
        /// <summary>
        /// valor unitario del item sin IGV
        /// </summary>
        public double valorUnitario
        {
            get
            {
                return Math.Round((valorUnitarioigv / (1 + valorIgvActual)), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
            }
            set
            {
                _valorUnitarioigv = value;
                valorReferencialIGV = value;
            }
        }
        /// <summary>
        /// porcentaje de descuento por promociones
        /// </summary>
        public double porcentajeDescPromocion { get; set; }
        /// <summary>
        /// monto de descuento por promociones
        /// </summary>
        public double descPromocion
        {
            get
            {
                /*
                if (tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE)
                { return Math.Round(((valorUnitario - descxCobSeguro) * (porcentajeDescPromocion / 100)), 2); }
                else
                {
                    return Math.Round(((valorUnitario - descxCobPaciente) * (porcentajeDescPromocion / 100)), 2);
                }
                */
                return Math.Round(((valorUnitario) * (porcentajeDescPromocion / 100)), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
            }
        }
        /// <summary>
        /// porcentaje que cubre el paciente
        /// </summary>
        public double porcentajeCobPaciente { get; set; }
        /// <summary>
        /// monto que cubre el paciente
        /// </summary>
        public double descxCobPaciente
        {
            get
            {

                {
                    double valor = Math.Round(((valorUnitario - descPromocion) * (porcentajeCobPaciente / 100)), tool.cantidad_decimales, MidpointRounding.AwayFromZero);

                    if (valor >= (valor - montoMaximoAseguradora))
                        return valor;
                    else
                        return Math.Round(((valorUnitario - descPromocion) - montoMaximoAseguradora), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                }

            }
        }
        /// <summary>
        /// porcentaje que cubre la aseguradora
        /// </summary>
        public double porcentajeCobSeguro { get; set; }
        /// <summary>
        /// monto que cubre la aseguradora
        /// </summary>
        public double descxCobSeguro
        {
            get
            {

                {
                    double valor = Math.Round(((valorUnitario - descPromocion) * (porcentajeCobSeguro / 100)), tool.cantidad_decimales, MidpointRounding.AwayFromZero);

                    if (valor <= montoMaximoAseguradora)
                        return valor;
                    else
                        return Math.Round((montoMaximoAseguradora), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                }
            }
        }

        private double _porcentajedescxItem;
        /// <summary>
        /// porcentaje de descuento por cortesia
        /// </summary>
        public double porcentajedescxItem
        {
            get
            {
                if (isCortesia)
                    return _porcentajedescxItem;
                else
                    return 0;
            }
            set
            {
                _porcentajedescxItem = value;
            }
        }
        /// <summary>
        /// monto de descuento por cortesia
        /// </summary>
        public double descxItem
        {
            get
            {
                if (isCortesia)
                    if (tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE)
                    { return Math.Round(((valorUnitario - descxCobSeguro - descPromocion) * (porcentajedescxItem / 100)), tool.cantidad_decimales, MidpointRounding.AwayFromZero); }
                    else
                    { return Math.Round(((valorUnitario - descxCobPaciente - descPromocion) * (porcentajedescxItem / 100)), tool.cantidad_decimales, MidpointRounding.AwayFromZero); }
                else
                    return 0;
            }
        }

        public double montoMaximoAseguradora { get; set; }


        public double valorVenta
        {
            get
            {
                if (tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE)
                {
                    if (isGratuita)
                        return 0;
                    else
                        return Math.Round(((valorUnitario - descxCobSeguro - descxItem - descPromocion) * cantidad), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                }
                else
                { return Math.Round(((valorUnitario - descxCobPaciente - descxItem - descPromocion) * cantidad), tool.cantidad_decimales, MidpointRounding.AwayFromZero); }


            }
        }
        /// <summary>
        /// obtiene el valor venta cuando es una gratuidad del monto que deberia de pagarse
        /// </summary>
        public double valorReferencialVenta
        {
            get
            {
                if (tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE)
                { return Math.Round(((valorReferencial - descxCobSeguro - descPromocion) * cantidad) * (1 + valorIgvActual), tool.cantidad_decimales, MidpointRounding.AwayFromZero); }
                else
                { return Math.Round(((valorReferencial - descxCobPaciente - descPromocion) * cantidad) * (1 + valorIgvActual), tool.cantidad_decimales, MidpointRounding.AwayFromZero); }


            }
        }

        public double igvItem_old
        {
            get
            {
                if (isGratuita)
                    return 0;
                else if (tipoIGV > (int)TIPO_IGV.GRAVADO_RETIROENTREGATRABAJADORES && tipoIGV < (int)TIPO_IGV.EXPORTACION)
                    return 0;
                else
                {
                    return Math.Round((valorVenta * valorIgvActual), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                }
            }
        }

        public double valorIGVImpresion
        {
            get
            {
                if (tipo_documento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    return 0;
                else
                    return igvItem_old;
            }
        }
        public double valorventaImpresion
        {
            get
            {
                if (tipo_documento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    return Math.Round(valorVenta * (valorIgvActual + 1), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                else if (tipo_documento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                    return valorVenta;
                else
                {
                    if (tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                        return Math.Round(valorVenta * (valorIgvActual + 1), tool.cantidad_decimales, MidpointRounding.AwayFromZero);
                    else
                        return valorVenta;
                }
            }
        }
        //public double totalventa { get { return Math.Round((igvItem_old + valorVenta), 2); } }





        /*   public double valorVentaSeguro
        {
            get
            {
                if (isAsegurado != null)
                {//si tiene carta
                    if (iscortesia)
                        return 0;
                    else
                        return Math.Round((descxSeguro * cantidad), 2);
                }
                else
                {//si tiene carta
                    if (iscortesia)
                        return 0;
                    else
                        return Math.Round((valorUnitario * cantidad), 2);
                }
            }
        }
        public double igvItemSeguro { get { return Math.Round((valorVentaSeguro * igvUnitario), 2); } }
        public double totalventaSeguro { get { return Math.Round((valorVentaSeguro + igvItemSeguro), 2); } }

        */
        public string codigoitem { get; set; }
        public bool isCortesia { get; set; }
        public bool isGratuita { get; set; }

        public string simboloMoneda { get; set; }

        public bool isAsegurado { get; set; }



        public string codigoclase { get; set; }
    }
    public class DetalleResumenBoleta
    {
        public int tipodocumento { get; set; }
        public string tipodocumentoSUNAT { get { return Enum.GetName(typeof(TIPO_DOCUMENTO_ELECTRONICO), tipodocumento); } }
        public string serie { get; set; }
        public string inicioRango { get; set; }
        public string finRango { get; set; }
        public double totalVentaExonerada { get; set; }
        public double totalVentaInafectas { get; set; }
        public double totalVentaGravadas { get; set; }
        public double totalOtrosCargos { get; set; }
        public double totalIGV { get; set; }
        public double totalISC { get; set; }
        public double totalotrosTributos { get; set; }
        public double totalVenta { get { return totalVentaExonerada + totalVentaInafectas + totalVentaGravadas + totalOtrosCargos + totalIGV + totalISC + totalotrosTributos; } }
        public int cantidad { get { return int.Parse(finRango) - int.Parse(inicioRango); } }
    }
    public class DocumentosxCancelar
    {
        public int tipodocumento { get; set; }
        public string serie { get; set; }
        public string correlativo { get; set; }
        public string motivo { get; set; }
    }

    public class ReporteCierreCaja
    {
        public string numerodocumento { get; set; }

        public string tipodocumento { get; set; }

        public double totalsoles { get; set; }

        public string monedaOriginal { get; set; }

        public double totalOriginal { get; set; }

        public string usuario { get; set; }

        public string tipomoneda { get; set; }

        public DateTime fechaemitio { get; set; }

        public string paciente { get; set; }

        public double tipocambio { get; set; }

        public string usuarioFormaPago { get; set; }

        public int codigomoneda { get; set; }

        public string tarjeta { get; set; }

        public string fPagoMonedaOriginal { get; set; }

        public double fPagoMontoOriginal { get; set; }

        public double fPagoMontoSoles { get; set; }

        public double fPagoMontoDolares { get; set; }

        public DateTime? fechadepago { get; set; }

        public bool? cortesia { get; set; }

        public string estado { get; set; }

        public string cortesiatext { get; set; }

        public bool isregularizado { get; set; }

        public string numeroreferencia { get; set; }
    }
    public class ReporteFacturacion
    {
        public string numerodocumento { get; set; }

        public string tipodocumento { get; set; }

        public double totalsoles { get; set; }

        public string monedaOriginal { get; set; }

        public double totalOriginal { get; set; }

        public string usuario { get; set; }

        public string tipomoneda { get; set; }

        public DateTime fechaemitio { get; set; }

        public string paciente { get; set; }

        public double tipocambio { get; set; }

        public string estado { get; set; }

        public string numeroreferencia { get; set; }
    }

    public class DocumentosEmitidos
    {

        public string empresa { get; set; }

        public string numerodocumento { get; set; }

        public string tipodocumento { get; set; }

        public string paciente { get; set; }

        public double cobertura { get; set; }

        public string carta { get; set; }

        public string poliza { get; set; }

        public string titular { get; set; }

        public double subtotal { get; set; }

        public double igv { get; set; }

        public double total { get; set; }

        public double tipocambio { get; set; }

        public DateTime fechaemitio { get; set; }

        public int codigopaciente { get; set; }

        public List<DetalleDocumentoEmitidos> listadetalle { get; set; }

        public string pathfile { get; set; }
        public string pathfilexml { get { return pathfile.Replace("PDF", "ZIP").Replace("pdf", "zip"); } }

        public int tipomoneda { get; set; }

        public string moneda
        {
            get
            {

                return tipomoneda == 1 ? "Soles" : tipomoneda == 2 ? "Dolares" : "";
            }
        }

        public double valorIGV { get; set; }

        public int tipocobranza { get; set; }

        public string ruc_alterno { get; set; }

        public string estado { get; set; }

        public int unidad { get; set; }

        public int sede { get; set; }

        public string ruc { get; set; }

        public string serie { get; set; }

        public string razonsocial { get; set; }

        public bool isFacGlobal { get; set; }
        public string pathPreFactura { get { return isFacGlobal ? @"\\serverweb\Facturacion\PREFACTURA\PREFACTURA-" + idFac + ".pdf" : ""; } }

        public int idFac { get; set; }

        public bool isSendSunat { get; set; }

        public string sucursal { get; set; }
    }
    public class DetalleDocumentoEmitidos
    {
        public string descripcion { get; set; }

        public double valorunitario { set; get; }

        public double carta { get; set; }

        public double cortesia { get; set; }

        public double promocion { get; set; }

        public double valortotal { get; set; }

        public int cantidad { get; set; }

        public string codigoestudio { get; set; }

        public string codigoclase { get; set; }

        public double porDesPromo { get; set; }

        public double porDesCarta { get; set; }

        public double porDescuento { get; set; }

        public int tipoIGV { get; set; }
        public bool isSelected { get; set; }

        public bool isModificado { get; set; }
    }
    public class PreResumenFacGlobal
    {
        public bool isSelected { get; set; }
        public DateTime fecha { get; set; }
        public int numerocita { get; set; }

        public int numeroatencion { get; set; }
        public int codigopaciente { get; set; }
        public string paciente { get; set; }

        public string nombreestudio { get; set; }

        public string estadoestudio { get; set; }

        public double preciobruto { get; set; }

        public string codigoestudio { get; set; }

        public int codigomoneda { get; set; }
        public string moneda { get { return (codigomoneda == 1 ? "SOLES" : codigomoneda == 2 ? "DOLARES" : ""); } }
        public int codigocompania { get; set; }
        public string aseguradora { get; set; }

        public int codigoexamencita { get; set; }

        public int idDetFac { get; set; }

        public string preliquidaciones { get; set; }
    }
    public class PreResumenBoleta
    {

        public string serie { get; set; }

        public int correlativo { get; set; }

        public int tipodocumento { get; set; }

        public int tipoIGV { get; set; }
        public string tipoIGVSUNAT { get { return Enum.GetName(typeof(TIPO_IGV), tipoIGV); } }

        public double preciounitario { get; set; }

        public double valorventa { get; set; }

        public double valorigv { get; set; }

        public double subtotal { get; set; }

        public double igv { get; set; }

        public int tipodocReferenciaNota { get; set; }

        public string descripcion { get; set; }
    }

    public class AtencionesGlobales
    {
        public int atencion { get; set; }
        public int paciente { get; set; }
    }
    public class ListaInformes
    {

        public byte[] cuerpo { get; set; }

        public string examen { get; set; }

        public string filename { get; set; }
    }

    public class FiltroAseguradora
    {
        public bool isSeleccionado { get; set; }

        public string descripcion { get; set; }

        public string ruc { get; set; }
    }
    public class FacturacionEntity
    {
        public int numeroatencion { get; set; }

        public string numerodocumento { get; set; }

        public string paciente { get; set; }

        public string companiaseguro { get; set; }

        public int sucursal { get; set; }
        public bool isFacturado { get { return numerodocumento != ""; } }

        public string sede { get; set; }
    }
    public enum TIPO_IGV
    {
        GRAVADO_ONEROSA = 10,
        GRAVADO_RETIROPORPREMIO = 11,
        GRAVADO_RETIRODONACION = 12,
        GRAVADO_RETIRO = 13,
        GRAVADO_RETIROPORPUBLICIDAD = 14,
        GRAVADO_BONIFICACIONES = 15,
        GRAVADO_RETIROENTREGATRABAJADORES = 16,
        EXONERADO_ONEROSA = 20,
        EXONERADO_TRANSFERENCIAGRATUITA = 21,
        INAFECTO_RETIROBONIFICACION = 31,
        INAFECTO_RETIRO = 32,
        INAFECTO_RETIRO_POR_MUESTRA_MEDICA = 33,
        INAFECTO_RETIRO_POR_CONVENIO_COLECTIVO = 34,
        INAFECTO_RETIRO_POR_PREMIO = 35,
        INAFECTO_RETIRO_POR_PUBLICIDAD = 36,
        EXPORTACION = 40,
        NO_DECLARAR_SUNAT = 999
    }
    public enum TIPO_NOTA_DE_CREDITO
    {
        ANULACION_DE_LA_OPERACION = 1,//01
        ANULACION_POR_ERROR_EN_EL_RUC = 2,//02
        CORRECCION_ERROR_EN_LA_DESCRIPCION = 3,//03
        DESCUENTO_GLOBAL = 4,//04
        DESCUENTO_ITEM = 5,//05
        DEVOLUCION_TOTAL = 6,//06
        DEVOLUCION_PARCIAL = 7,//07
        BONIFICACION = 8,//08
        DISMINUCION_EN_EL_VALOR = 9,//09
        OTROS_CONCEPTOS = 10
    }
    public enum TIPO_NOTA_DE_DEBITO
    {
        INTERES_POR_MORA = 1,
        AUMENTO_EN_EL_VALOR = 2,
        PENALIDAD_OTROS_CONCEPTOS = 3
    }
    public enum TIPO_DOCUMENTO_ELECTRONICO
    {
        FACTURA = 1,
        BOLETA_DE_VENTA = 3,
        NOTA_DE_CREDITO = 7,
        NOTA_DE_DEBITO = 8,
        GUIA_REMISION_REMITENTE = 9,
        //--------------------------//
        RESUMEN_DIARIO_BOLETAS = 98,
        COMUNICACION_DE_BAJA = 99,

    }
    public enum TIPO_DOCUMENTOIDENTIDAD
    {
        DOC_TRIB_NO_DOM_SIN_RUC = 0,
        DNI = 1,
        CARNET_EXTRANJERIA = 4,
        RUC = 6,
        PASAPORTE = 7
    }

    public enum TIPO_COBRANZA
    {
        PACIENTE = 1,
        ASEGURADORA = 2
    }

    public enum TIPO_MONEDA
    {
        SOL = 1,
        DOLAR = 2
    }


    public class EmpresaFacturacion
    {
        public string ruc { get; set; }
        public int tipodocumento { get; set; }
        public string razonsocial { get; set; }
        public string direccion { get; set; }
        public int codigocompania { get; set; }

        public string aseguradora { get; set; }

        public string correo { get; set; }
    }

    public class DocumentosRecepcion
    {
        public int empresa { get; set; }
        public string numerodocumento { get; set; }
        public double total { get; set; }
        public string serie { get { return numerodocumento.Split('-')[0]; } }
        public int correlativo { get { return int.Parse(numerodocumento.Split('-')[1]); } }
        public DateTime fecha_recibido { get; set; }
        public DateTime fecha_emision { get; set; }
    }

    public class DocumentosCancelacion
    {
        public string numero_doc { get; set; }
        public double total { get; set; }
        public DateTime fec_emision { get; set; }
        public DateTime fec_recepcion { get; set; }
        public DateTime fec_pagp { get; set; }
        public string referencia { get; set; }
        public string resultado { get; set; }
    }

    public class Recibo_Provisional
    {
        public string paciente { get; set; }
        public int codigopaciente { get; set; }
        public List<string> estudio { get; set; }

        public int empresa { get; set; }
    }
    public class SendFacturasOffline
    {

        public int empresa { get; set; }

        public int paciente { get; set; }

        public string filename { get; set; }

        public string documento { get; set; }

        public string estado { get; set; }

        public string resultado { get; set; }
    }
}
