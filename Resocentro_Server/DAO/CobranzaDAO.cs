using Resocentro_Server.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Resocentro_Server.DAO
{
    class CobranzaDAO
    {
        public static string PathDocumentosFacturacion { get { return @"\\serverweb\Facturacion\"; } }
        public static string PathCERTIFICADORESO { get { return @"\\serverweb\Facturacion\Cer-Reso\MPS20151222232684.pfx"; } }
        public static string PathCERTIFICADOEMETAC { get { return @"\\serverweb\Facturacion\Cer-Eme\MPS20160829351646.pfx"; } }




        public void insertarLogDocumento(string archivo, string tipo, string ticket, string estado = "")
        {
            var doc_split = archivo.Split('-');
            string doc = archivo;
            if (doc_split.Count() == 4)
                doc = doc_split[2] + "-" + doc_split[3];
            string sqlDocumento = "INSERT INTO Log_SUNAT (documento,fecha,archivo,tipo,ticket,estado) VALUES('" + doc.Replace(".zip", "") + "',getdate(),'" + archivo + "','" + tipo + "','" + ticket + "','" + estado + "');";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand();
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction("GuardarDocumento");
                    try
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;

                        command.CommandText = sqlDocumento;
                        command.Prepare();
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        string mensaje = "ERROR SQL: No se guardaron los registros de Libro Caja,Documento,Detalle Documento :\n-" + ex.Message;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            mensaje += "\n\nError Rollback";
                            mensaje += "\n-Exxeption Type: " + ex2.GetType();
                            mensaje += "\n-Message: " + ex2.Message;
                        }

                        throw new Exception(mensaje);

                    }
                    finally
                    {
                        transaction.Dispose();
                        command.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }
        }
        public static string getErrorSunat(string code)
        {
            code = code.ToLower().Replace("client.", "");
            switch (code)
            {
                case "0100": return "El sistema no puede responder su solicitud. Intente nuevamente o comuníquese con su Administrador ";
                case "0101": return "El encabezado de seguridad es incorrecto ";
                case "0102": return "Usuario o contraseña incorrectos ";
                case "0103": return "El Usuario ingresado no existe ";
                case "0104": return "La Clave ingresada es incorrecta ";
                case "0105": return "El Usuario no está activo ";
                case "0106": return "El Usuario no es válido ";
                case "0109": return "El sistema no puede responder su solicitud. (El servicio de autenticación no está disponible) ";
                case "0110": return "No se pudo obtener la informacion del tipo de usuario ";
                case "0111": return "No tiene el perfil para enviar comprobantes electronicos ";
                case "0112": return "El usuario debe ser secundario ";
                case "0113": return "El usuario no esta afiliado a Factura Electronica ";
                case "0125": return "No se pudo obtener la constancia ";
                case "0126": return "El ticket no le pertenece al usuario ";
                case "0127": return "El ticket no existe ";
                case "0130": return "El sistema no puede responder su solicitud. (No se pudo obtener el ticket de proceso) ";
                case "0131": return "El sistema no puede responder su solicitud. (No se pudo grabar el archivo en el directorio) ";
                case "0132": return "El sistema no puede responder su solicitud. (No se pudo grabar escribir en el archivo zip) ";
                case "0133": return "El sistema no puede responder su solicitud. (No se pudo grabar la entrada del log) ";
                case "0134": return "El sistema no puede responder su solicitud. (No se pudo grabar en el storage) ";
                case "0135": return "El sistema no puede responder su solicitud. (No se pudo encolar el pedido) ";
                case "0136": return "El sistema no puede responder su solicitud. (No se pudo recibir una respuesta del batch) ";
                case "0137": return "El sistema no puede responder su solicitud. (Se obtuvo una respuesta nula) ";
                case "0138": return "El sistema no puede responder su solicitud. (Error en Base de Datos)";
                case "0151": return "El nombre del archivo ZIP es incorrecto ";
                case "0152": return "No se puede enviar por este método un archivo de resumen ";
                case "0153": return "No se puede enviar por este método un archivo por lotes ";
                case "0154": return "El RUC del archivo no corresponde al RUC del usuario ";
                case "0155": return "El archivo ZIP esta vacio ";
                case "0156": return "El archivo ZIP esta corrupto ";
                case "0157": return "El archivo ZIP no contiene comprobantes ";
                case "0158": return "El archivo ZIP contiene demasiados comprobantes para este tipo de envío ";
                case "0159": return "El nombre del archivo XML es incorrecto ";
                case "0160": return "El archivo XML esta vacio ";
                case "0161": return "El nombre del archivo XML no coincide con el nombre del archivo ZIP ";
                case "0200": return "No se pudo procesar su solicitud. (Ocurrio un error en el batch) ";
                case "0201": return "No se pudo procesar su solicitud. (Llego un requerimiento nulo al batch)";
                case "0202": return "No se pudo procesar su solicitud. (No llego información del archivo ZIP)";
                case "0203": return "No se pudo procesar su solicitud. (No se encontro archivos en la informacion del archivo ZIP)";
                case "0204": return "No se pudo procesar su solicitud. (Este tipo de requerimiento solo acepta 1 archivo)";
                case "0250": return "No se pudo procesar su solicitud. (Ocurrio un error desconocido al hacer unzip) ";
                case "0251": return "No se pudo procesar su solicitud. (No se pudo crear un directorio para el unzip) ";
                case "0252": return "No se pudo procesar su solicitud. (No se encontro archivos dentro del zip) ";
                case "0253": return "No se pudo procesar su solicitud. (No se pudo comprimir la constancia) ";
                case "0300": return "No se encontró la raíz documento xml";
                case "0301": return "Elemento raiz del xml no esta definido";
                case "0302": return "Codigo del tipo de comprobante no registrado";
                case "0303": return "No existe el directorio de schemas ";
                case "0304": return "No existe el archivo de schema ";
                case "0305": return "El sistema no puede procesar el archivo xml ";
                case "0306": return "No se puede leer (parsear) el archivo XML ";
                case "0307": return "No se pudo recuperar la constancia ";
                case "0400": return "No tiene permiso para enviar casos de pruebas ";
                case "0401": return "El caso de prueba no existe ";
                case "0402": return "La numeracion o nombre del documento ya ha sido enviado anteriormente ";
                case "0403": return "El documento afectado por la nota no existe ";
                case "0404": return "El documento afectado por la nota se encuentra rechazado ";
                case "1001": return "ID - El dato SERIE-CORRELATIVO no cumple con el formato de acuerdo al tipo de comprobante ";
                case "1002": return "El XML no contiene informacion en el tag ID ";
                case "1003": return "InvoiceTypeCode - El valor del tipo de documento es invalido o no coincide con el nombre del archivo ";
                case "1004": return "El XML no contiene el tag o no existe informacion de InvoiceTypeCode ";
                case "1005": return "CustomerAssignedAccountID - El dato ingresado no cumple con el estandar ";
                case "1006": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del emisor del documento ";
                case "1007": return "AdditionalAccountID - El dato ingresado no cumple con el estandar ";
                case "1008": return "El XML no contiene el tag o no existe informacion de AdditionalAccountID del emisor del documento ";
                case "1009": return "IssueDate - El dato ingresado  no cumple con el patron YYYY-MM-DD";
                case "1010": return "El XML no contiene el tag IssueDate";
                case "1011": return "IssueDate- El dato ingresado no es valido";
                case "1012": return "ID - El dato ingresado no cumple con el patron SERIE-CORRELATIVO";
                case "1013": return "El XML no contiene informacion en el tag ID";
                case "1014": return "CustomerAssignedAccountID - El dato ingresado no cumple con el estándar";
                case "1015": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del emisor del documento";
                case "1016": return "AdditionalAccountID - El dato ingresado no cumple con el estándar";
                case "1017": return "El XML no contiene el tag AdditionalAccountID del emisor del documento";
                case "1018": return "IssueDate - El dato ingresado no cumple con el patron YYYY-MM-DD";
                case "1019": return "El XML no contiene el tag IssueDate";
                case "1020": return "IssueDate- El dato ingresado no es valido";
                case "1021": return "Error en la validacion de la nota de credito";
                case "1022": return "La serie o numero del documento modificado por la Nota Electrónica no cumple con el formato establecido ";
                case "1023": return "No se ha especificado el tipo de documento modificado por la Nota electronica ";
                case "1024": return "CustomerAssignedAccountID - El dato ingresado no cumple con el estándar";
                case "1025": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del emisor del documento";
                case "1026": return "AdditionalAccountID - El dato ingresado no cumple con el estándar";
                case "1027": return "El XML no contiene el tag AdditionalAccountID del emisor del documento";
                case "1028": return "IssueDate - El dato ingresado no cumple con el patron YYYY-MM-DD";
                case "1029": return "El XML no contiene el tag IssueDate";
                case "1030": return "IssueDate- El dato ingresado no es valido";
                case "1031": return "Error en la validacion de la nota de debito";
                case "1032": return "El comprobante fue informado previamente en una comunicacion de baja ";
                case "1033": return "El comprobante fue registrado previamente con otros datos ";
                case "1034": return "Número de RUC del nombre del archivo no coincide con el consignado en el contenido del archivo XML ";
                case "1035": return "Numero de Serie del nombre del archivo no coincide con el consignado en el contenido del archivo XML ";
                case "1036": return "Número de documento en el nombre del archivo no coincide con el consignado en el contenido del XML ";
                case "1037": return "El XML no contiene el tag o no existe informacion de RegistrationName del emisor del documento ";
                case "1038": return "RegistrationName - El nombre o razon social del emisor no cumple con el estandar ";
                case "1039": return "Solo se pueden recibir notas electronicas que modifican facturas ";
                case "1040": return "El tipo de documento modificado por la nota electronica no es valido ";
                case "2010": return "El contribuyente no esta activo ";
                case "2011": return "El contribuyente no esta habido ";
                case "2012": return "El contribuyente no está autorizado a emitir comprobantes electrónicos";
                case "2013": return "El contribuyente no cumple con tipo de empresa o tributos requeridos ";
                case "2014": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del receptor del documento ";
                case "2015": return "El XML no contiene el tag o no existe informacion de AdditionalAccountID del receptor del documento ";
                case "2016": return "AdditionalAccountID - El dato ingresado en el tipo de documento de identidad del receptor no cumple con el estandar ";
                case "2017": return "CustomerAssignedAccountID - El numero de documento de identidad del recepetor debe ser RUC ";
                case "2018": return "CustomerAssignedAccountID -  El dato ingresado no cumple con el estándar";
                case "2019": return "El XML no contiene el tag o no existe informacion de RegistrationName del emisor del documento";
                case "2020": return "RegistrationName - El nombre o razon social del emisor no cumple con el estándar";
                case "2021": return "El XML no contiene el tag o no existe informacion de RegistrationName del receptor del documento ";
                case "2022": return "RegistrationName - El dato ingresado no cumple con el estandar ";
                case "2023": return "El Numero de orden del item no cumple con el formato establecido ";
                case "2024": return "El XML no contiene el tag InvoicedQuantity en el detalle de los Items ";
                case "2025": return "InvoicedQuantity El dato ingresado no cumple con el estandar ";
                case "2026": return "El XML no contiene el tag cac:Item/cbc:Description en el detalle de los Items";
                case "2027": return "El XML no contiene el tag o no existe informacion de cac:Item/cbc:Description del item ";
                case "2028": return "Debe existir el tag cac:AlternativeConditionPrice con un elemento cbc:PriceTypeCode con valor 01 ";
                case "2029": return "PriceTypeCode El dato ingresado no cumple con el estándar";
                case "2030": return "El XML no contiene el tag cbc:PriceTypeCode";
                case "2031": return "LineExtensionAmount El dato ingresado no cumple con el estándar";
                case "2032": return "El XML no contiene el tag LineExtensionAmount en el detalle de los Items";
                case "2033": return "El dato ingresado en TaxAmount de la linea no cumple con el formato establecido ";
                case "2034": return "TaxAmount es obligatorio";
                case "2035": return "cac:TaxCategory/cac:TaxScheme/cbc:ID El dato ingresado no cumple con el estandar ";
                case "2036": return "El codigo del tributo es invalido ";
                case "2037": return "El XML no contiene el tag cac:TaxCategory/cac:TaxScheme/cbc:ID del Item";
                case "2038": return "cac:TaxScheme/cbc:Name del item - No existe el tag o el dato ingresado no cumple con el estandar ";
                case "2039": return "El XML no contiene el tag cac:TaxCategory/cac:TaxScheme/cbc:Name del Item";
                case "2040": return "El tipo de afectacion del IGV es incorrecto ";
                case "2041": return "El sistema de calculo del ISC es incorrecto ";
                case "2042": return "Debe indicar el IGV. Es un campo obligatorio ";
                case "2043": return "El dato ingresado en PayableAmount no cumple con el formato establecido ";
                case "2044": return "PayableAmount es obligatorio";
                case "2045": return "El valor ingresado en AdditionalMonetaryTotal/cbc:ID es incorrecto ";
                case "2046": return "AdditionalMonetaryTotal/cbc:ID debe tener valor";
                case "2047": return "Es obligatorio al menos un AdditionalMonetaryTotal con codigo 1001, 1002 o 1003 ";
                case "2048": return "El dato ingresado en TaxAmount no cumple con el formato establecido ";
                case "2049": return "TaxAmount es obligatorio";
                case "2050": return "TaxScheme ID - No existe el tag o el dato ingresado no cumple con el estandar ";
                case "2051": return "El codigo del tributo es invalido ";
                case "2052": return "El XML no contiene el tag TaxScheme ID de impuestos globales";
                case "2053": return "TaxScheme Name - No existe el tag o el dato ingresado no cumple con el estandar ";
                case "2054": return "El XML no contiene el tag TaxScheme Name de impuestos globales";
                case "2055": return "TaxScheme TaxTypeCode - El dato ingresado no cumple con el estandar ";
                case "2056": return "El XML no contiene el tag TaxScheme TaxTypeCode de impuestos globales";
                case "2057": return "El Name o TaxTypeCode debe corresponder con el Id para el IGV ";
                case "2058": return "El Name o TaxTypeCode debe corresponder con el Id para el ISC ";
                case "2059": return "El dato ingresado en TaxSubtotal/cbc:TaxAmount no cumple con el formato establecido ";
                case "2060": return "TaxSubtotal/cbc:TaxAmount es obligatorio";
                case "2061": return "El tag global cac:TaxTotal/cbc:TaxAmount debe tener el mismo valor que cac:TaxTotal/cac:Subtotal/cbc:TaxAmount ";
                case "2062": return "El dato ingresado en PayableAmount no cumple con el formato establecido ";
                case "2063": return "El XML no contiene el tag PayableAmount";
                case "2064": return "El dato ingresado en ChargeTotalAmount no cumple con el formato establecido ";
                case "2065": return "El dato ingresado en el campo Total Descuentos no cumple con el formato establecido ";
                case "2066": return "Debe indicar una descripcion para el tag sac:AdditionalProperty/cbc:Value";
                case "2067": return "cac:Price/cbc:PriceAmount - El dato ingresado no cumple con el estandar";
                case "2068": return "El XML no contiene el tag cac:Price/cbc:PriceAmount en el detalle de los Items ";
                case "2069": return "DocumentCurrencyCode - El dato ingresado no cumple con la estructura ";
                case "2070": return "El XML no contiene el tag o no existe informacion de DocumentCurrencyCode ";
                case "2071": return "La moneda debe ser la misma en todo el documento ";
                case "2072": return "CustomizationID - La versión del documento no es la correcta ";
                case "2073": return "El XML no contiene el tag o no existe informacion de CustomizationID ";
                case "2074": return "UBLVersionID - La versión del UBL no es correcta ";
                case "2075": return "El XML no contiene el tag o no existe informacion de UBLVersionID ";
                case "2076": return "cac:Signature/cbc:ID - Falta el identificador de la firma ";
                case "2077": return "El tag cac:Signature/cbc:ID debe contener informacion ";
                case "2078": return "cac:Signature/cac:SignatoryParty/cac:PartyIdentification/cbc:ID - Debe ser igual al RUC del emisor ";
                case "2079": return "El XML no contiene el tag cac:Signature/cac:SignatoryParty/cac:PartyIdentification/cbc:ID";
                case "2080": return "cac:Signature/cac:SignatoryParty/cac:PartyName/cbc:Name - No cumple con el estandar ";
                case "2081": return "El XML no contiene el tag cac:Signature/cac:SignatoryParty/cac:PartyName/cbc:Name";
                case "2082": return "cac:Signature/cac:DigitalSignatureAttachment/cac:ExternalReference/cbc:URI - No cumple con el estandar ";
                case "2083": return "El XML no contiene el tag cac:Signature/cac:DigitalSignatureAttachment/cac:ExternalReference/cbc:URI ";
                case "2084": return "ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/ds:Signature/@Id - No cumple con el estandar ";
                case "2085": return "El XML no contiene el tag ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/ds:Signature/@Id ";
                case "2086": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:CanonicalizationMethod/@Algorithm - No cumple con el estandar ";
                case "2087": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:CanonicalizationMethod/@Algorithm ";
                case "2088": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:SignatureMethod/@Algorithm - No cumple con el estandar ";
                case "2089": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:SignatureMethod/@Algorithm ";
                case "2090": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/@URI - Debe estar vacio para id ";
                case "2091": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/@URI";
                case "2092": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/.../ds:Transform@Algorithm - No cumple con el estandar ";
                case "2093": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/ds:Transform@Algorithm ";
                case "2094": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestMethod/@Algorithm - No cumple con el estandar ";
                case "2095": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestMethod/@Algorithm ";
                case "2096": return "ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestValue - No cumple con el estandar ";
                case "2097": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestValue ";
                case "2098": return "ext:UBLExtensions/.../ds:Signature/ds:SignatureValue - No cumple con el estandar ";
                case "2099": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:SignatureValue ";
                case "2100": return "ext:UBLExtensions/.../ds:Signature/ds:KeyInfo/ds:X509Data/ds:X509Certificate - No cumple con el estandar ";
                case "2101": return "El XML no contiene el tag ext:UBLExtensions/.../ds:Signature/ds:KeyInfo/ds:X509Data/ds:X509Certificate ";
                case "2102": return "Error al procesar la factura";
                case "2103": return "La serie ingresada no es válida";
                case "2104": return "Numero de RUC del emisor no existe";
                case "2105": return "Factura a dar de baja no se encuentra registrada en SUNAT ";
                case "2106": return "Factura a dar de baja ya se encuentra en estado de baja ";
                case "2107": return "Numero de RUC SOL no coincide con RUC emisor";
                case "2108": return "Presentacion fuera de fecha ";
                case "2109": return "El comprobante fue registrado previamente con otros datos";
                case "2110": return "UBLVersionID - La versión del UBL no es correcta";
                case "2111": return "El XML no contiene el tag o no existe informacion de UBLVersionID";
                case "2112": return "CustomizationID - La version del documento no es correcta";
                case "2113": return "El XML no contiene el tag o no existe informacion de CustomizationID";
                case "2114": return "DocumentCurrencyCode -  El dato ingresado no cumple con la estructura";
                case "2115": return "El XML no contiene el tag o no existe informacion de DocumentCurrencyCode";
                case "2116": return "El tipo de documento modificado por la Nota de credito debe ser factura electronica o ticket ";
                case "2117": return "La serie o numero del documento modificado por la Nota de Credito no cumple con el formato establecido ";
                case "2118": return "Debe indicar las facturas relacionadas a la Nota de Credito ";
                case "2119": return "La factura relacionada en la Nota de credito no esta registrada. ";
                case "2120": return "La factura relacionada en la nota de credito se encuentra de baja ";
                case "2121": return "La factura relacionada en la nota de credito esta registrada como rechazada ";
                case "2122": return "El tag cac:LegalMonetaryTotal/cbc:PayableAmount debe tener informacion valida";
                case "2123": return "RegistrationName -  El dato ingresado no cumple con el estandar";
                case "2124": return "El XML no contiene el tag RegistrationName del emisor del documento";
                case "2125": return "ReferenceID - El dato ingresado debe indicar SERIE-CORRELATIVO del documento al que se relaciona la Nota ";
                case "2126": return "El XML no contiene informacion en el tag ReferenceID del documento al que se relaciona la nota ";
                case "2127": return "ResponseCode - El dato ingresado no cumple con la estructura ";
                case "2128": return "El XML no contiene el tag o no existe informacion de ResponseCode ";
                case "2129": return "AdditionalAccountID - El dato ingresado en el tipo de documento de identidad del receptor no cumple con el estandar ";
                case "2130": return "El XML no contiene el tag o no existe informacion de AdditionalAccountID del receptor del documento ";
                case "2131": return "CustomerAssignedAccountID - El numero de documento de identidad del receptor debe ser RUC ";
                case "2132": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del receptor del documento ";
                case "2133": return "RegistrationName - El dato ingresado no cumple con el estandar ";
                case "2134": return "El XML no contiene el tag o no existe informacion de RegistrationName del receptor del documento ";
                case "2135": return "cac:DiscrepancyResponse/cbc:Description - El dato ingresado no cumple con la estructura ";
                case "2136": return "El XML no contiene el tag o no existe informacion de cac:DiscrepancyResponse/cbc:Description ";
                case "2137": return "El Número de orden del item no cumple con el formato establecido ";
                case "2138": return "CreditedQuantity/@unitCode - El dato ingresado no cumple con el estandar";
                case "2139": return "CreditedQuantity - El dato ingresado no cumple con el estandar ";
                case "2140": return "El PriceTypeCode debe tener el valor 01 ";
                case "2141": return "cac:TaxCategory/cac:TaxScheme/cbc:ID - El dato ingresado no cumple con el estandar ";
                case "2142": return "El codigo del tributo es invalido ";
                case "2143": return "cac:TaxScheme/cbc:Name del item - No existe el tag o el dato ingresado no cumple con el estandar ";
                case "2144": return "cac:TaxCategory/cac:TaxScheme/cbc:TaxTypeCode El dato ingresado no cumple con el estandar";
                case "2145": return "El tipo de afectacion del IGV es incorrecto ";
                case "2146": return "El Nombre Internacional debe ser VAT";
                case "2147": return "El sistema de calculo del ISC es incorrecto ";
                case "2148": return "El Nombre Internacional debe ser EXC ";
                case "2149": return "El dato ingresado en PayableAmount no cumple con el formato establecido ";
                case "2150": return "El valor ingresado en AdditionalMonetaryTotal/cbc:ID es incorrecto ";
                case "2151": return "AdditionalMonetaryTotal/cbc:ID debe tener valor ";
                case "2152": return "Es obligatorio al menos un AdditionalInformation";
                case "2153": return "Error al procesar la Nota de Credito";
                case "2154": return "TaxAmount - El dato ingresado en impuestos globales no cumple con el estandar";
                case "2155": return "El XML no contiene el tag TaxAmount de impuestos globales";
                case "2156": return "TaxScheme ID - El dato ingresado no cumple con el estandar ";
                case "2157": return "El codigo del tributo es invalido ";
                case "2158": return "El XML no contiene el tag o no existe informacion de TaxScheme ID de impuestos globales ";
                case "2159": return "TaxScheme Name - El dato ingresado no cumple con el estandar ";
                case "2160": return "El XML no contiene el tag o no existe informacion de TaxScheme Name de impuestos globales ";
                case "2161": return "CustomizationID - La version del documento no es correcta";
                case "2162": return "El XML no contiene el tag o no existe informacion de CustomizationID";
                case "2163": return "UBLVersionID - La versión del UBL no es correcta";
                case "2164": return "El XML no contiene el tag o no existe informacion de UBLVersionID";
                case "2165": return "Error al procesar la Nota de Debito";
                case "2166": return "RegistrationName - El dato ingresado no cumple con el estandar";
                case "2167": return "El XML no contiene el tag RegistrationName del emisor del documento";
                case "2168": return "DocumentCurrencyCode -  El dato ingresado no cumple con el formato establecido";
                case "2169": return "El XML no contiene el tag o no existe informacion de DocumentCurrencyCode";
                case "2170": return "ReferenceID - El dato ingresado debe indicar SERIE-CORRELATIVO del documento al que se relaciona la Nota ";
                case "2171": return "El XML no contiene informacion en el tag ReferenceID del documento al que se relaciona la nota ";
                case "2172": return "ResponseCode - El dato ingresado no cumple con la estructura ";
                case "2173": return "El XML no contiene el tag o no existe informacion de ResponseCode ";
                case "2174": return "cac:DiscrepancyResponse/cbc:Description - El dato ingresado no cumple con la estructura ";
                case "2175": return "El XML no contiene el tag o no existe informacion de cac:DiscrepancyResponse/cbc:Description ";
                case "2176": return "AdditionalAccountID - El dato ingresado en el tipo de documento de identidad del receptor no cumple con el estandar ";
                case "2177": return "El XML no contiene el tag o no existe informacion de AdditionalAccountID del receptor del documento ";
                case "2178": return "CustomerAssignedAccountID - El numero de documento de identidad del receptor debe ser RUC. ";
                case "2179": return "El XML no contiene el tag o no existe informacion de CustomerAssignedAccountID del receptor del documento ";
                case "2180": return "RegistrationName - El dato ingresado no cumple con el estandar ";
                case "2181": return "El XML no contiene el tag o no existe informacion de RegistrationName del receptor del documento ";
                case "2182": return "TaxScheme ID - El dato ingresado no cumple con el estandar ";
                case "2183": return "El codigo del tributo es invalido ";
                case "2184": return "El XML no contiene el tag o no existe informacion de TaxScheme ID de impuestos globales ";
                case "2185": return "TaxScheme Name - El dato ingresado no cumple con el estandar ";
                case "2186": return "El XML no contiene el tag o no existe informacion de TaxScheme Name de impuestos globales ";
                case "2187": return "El Numero de orden del item no cumple con el formato establecido ";
                case "2188": return "DebitedQuantity/@unitCode El dato ingresado no cumple con el estandar";
                case "2189": return "DebitedQuantity El dato ingresado no cumple con el estandar ";
                case "2190": return "El XML no contiene el tag Price/cbc:PriceAmount en el detalle de los Items ";
                case "2191": return "El XML no contiene el tag Price/cbc:LineExtensionAmount en el detalle de los Items";
                case "2192": return "EL PriceTypeCode debe tener el valor 01 ";
                case "2193": return "cac:TaxCategory/cac:TaxScheme/cbc:ID El dato ingresado no cumple con el estandar ";
                case "2194": return "El codigo del tributo es invalido ";
                case "2195": return "cac:TaxScheme/cbc:Name del item - No existe el tag o el dato ingresado no cumple con el estandar ";
                case "2196": return "cac:TaxCategory/cac:TaxScheme/cbc:TaxTypeCode El dato ingresado no cumple con el estandar";
                case "2197": return "El tipo de afectacion del IGV es incorrecto ";
                case "2198": return "El Nombre Internacional debe ser VAT";
                case "2199": return "El sistema de calculo del ISC es incorrecto ";
                case "2200": return "El Nombre Internacional debe ser EXC";
                case "2201": return "El tag cac:RequestedMonetaryTotal/cbc:PayableAmount debe tener informacion valida";
                case "2202": return "TaxAmount - El dato ingresado en impuestos globales no cumple con el estándar";
                case "2203": return "El XML no contiene el tag TaxAmount de impuestos globales";
                case "2204": return "El tipo de documento modificado por la Nota de Debito debe ser factura electronica o ticket ";
                case "2205": return "La serie o numero del documento modificado por la Nota de Debito no cumple con el formato establecido ";
                case "2206": return "Debe indicar los documentos afectados por la Nota de Debito ";
                case "2207": return "La factura relacionada en la nota de debito se encuentra de baja ";
                case "2208": return "La factura relacionada en la nota de debito esta registrada como rechazada ";
                case "2209": return "La factura relacionada en la Nota de debito no esta registrada ";
                case "2210": return "El dato ingresado no cumple con el formato RC-fecha-correlativo ";
                case "2211": return "El XML no contiene el tag ID ";
                case "2212": return "UBLVersionID - La versión del UBL del resumen de boletas no es correcta";
                case "2213": return "El XML no contiene el tag UBLVersionID";
                case "2214": return "CustomizationID - La versión del resumen de boletas no es correcta";
                case "2215": return "El XML no contiene el tag CustomizationID";
                case "2216": return "CustomerAssignedAccountID - El dato ingresado no cumple con el estandar ";
                case "2217": return "El XML no contiene el tag CustomerAssignedAccountID del emisor del documento ";
                case "2218": return "AdditionalAccountID - El dato ingresado no cumple con el estandar ";
                case "2219": return "El XML no contiene el tag AdditionalAccountID del emisor del documento ";
                case "2220": return "El ID debe coincidir con el nombre del archivo ";
                case "2221": return "El RUC debe coincidir con el RUC del nombre del archivo ";
                case "2222": return "El contribuyente no está autorizado a emitir comprobantes electronicos";
                case "2223": return "El archivo ya fue presentado anteriormente ";
                case "2224": return "Numero de RUC SOL no coincide con RUC emisor";
                case "2225": return "Numero de RUC del emisor no existe";
                case "2226": return "El contribuyente no esta activo";
                case "2227": return "El contribuyente no cumple con tipo de empresa o tributos requeridos";
                case "2228": return "RegistrationName - El dato ingresado no cumple con el estandar ";
                case "2229": return "El XML no contiene el tag RegistrationName del emisor del documento ";
                case "2230": return "IssueDate - El dato ingresado no cumple con el patron YYYY-MM-DD";
                case "2231": return "El XML no contiene el tag IssueDate";
                case "2232": return "IssueDate- El dato ingresado no es valido";
                case "2233": return "ReferenceDate - El dato ingresado no cumple con el patron YYYY-MM-DD ";
                case "2234": return "El XML no contiene el tag ReferenceDate";
                case "2235": return "ReferenceDate- El dato ingresado no es valido";
                case "2236": return "La fecha del IssueDate no debe ser mayor al Today ";
                case "2237": return "La fecha del ReferenceDate no debe ser mayor al Today ";
                case "2238": return "LineID - El dato ingresado no cumple con el estandar ";
                case "2239": return "LineID - El dato ingresado debe ser correlativo mayor a cero ";
                case "2240": return "El XML no contiene el tag LineID de SummaryDocumentsLine ";
                case "2241": return "DocumentTypeCode - El valor del tipo de documento es invalido ";
                case "2242": return "El XML no contiene el tag DocumentTypeCode ";
                case "2243": return "El dato ingresado no cumple con el patron SERIE ";
                case "2244": return "El XML no contiene el tag DocumentSerialID ";
                case "2245": return "El dato ingresado en StartDocumentNumberID debe ser numerico ";
                case "2246": return "El XML no contiene el tag StartDocumentNumberID ";
                case "2247": return "El dato ingresado en sac:EndDocumentNumberID debe ser numerico ";
                case "2248": return "El XML no contiene el tag sac:EndDocumentNumberID ";
                case "2249": return "Los rangos deben ser mayores a cero ";
                case "2250": return "En el rango de comprobantes, el EndDocumentNumberID debe ser mayor o igual al StartInvoiceNumberID ";
                case "2251": return "El dato ingresado en TotalAmount debe ser numerico mayor o igual a cero ";
                case "2252": return "El XML no contiene el tag TotalAmount";
                case "2253": return "El dato ingresado en TotalAmount debe ser numerico mayor a cero";
                case "2254": return "PaidAmount - El dato ingresado no cumple con el estandar ";
                case "2255": return "El XML no contiene el tag PaidAmount ";
                case "2256": return "InstructionID - El dato ingresado no cumple con el estandar ";
                case "2257": return "El XML no contiene el tag InstructionID ";
                case "2258": return "Debe indicar Referencia de Importes asociados a las boletas de venta";
                case "2259": return "Debe indicar 3 Referencias de Importes asociados a las boletas de venta ";
                case "2260": return "PaidAmount - El dato ingresado debe ser mayor o igual a 0.00";
                case "2261": return "cbc:Amount - El dato ingresado no cumple con el estandar ";
                case "2262": return "El XML no contiene el tag cbc:Amount";
                case "2263": return "ChargeIndicator - El dato ingresado no cumple con el estandar ";
                case "2264": return "El XML no contiene el tag ChargeIndicator";
                case "2265": return "Debe indicar Información acerca del Importe Total de Otros Cargos ";
                case "2266": return "Debe indicar cargos mayores o iguales a cero";
                case "2267": return "TaxScheme ID - El dato ingresado no cumple con el estandar ";
                case "2268": return "El codigo del tributo es invalido ";
                case "2269": return "El XML no contiene el tag TaxScheme ID de Información acerca del importe total de un tipo particular de impuesto ";
                case "2270": return "TaxScheme Name - El dato ingresado no cumple con el estandar ";
                case "2271": return "El XML no contiene el tag TaxScheme Name de impuesto ";
                case "2272": return "TaxScheme TaxTypeCode - El dato ingresado no cumple con el estandar";
                case "2273": return "TaxAmount - El dato ingresado no cumple con el estandar ";
                case "2274": return "El XML no contiene el tag TaxAmount";
                case "2275": return "Si el codigo de tributo es 2000, el nombre del tributo debe ser ISC ";
                case "2276": return "Si el codigo de tributo es 1000, el nombre del tributo debe ser IGV ";
                case "2277": return "No se ha consignado ninguna informacion del importe total de tributos ";
                case "2278": return "Debe indicar Información acerca del importe total de ISC e IGV ";
                case "2279": return "Debe indicar Items de consolidado de documentos";
                case "2280": return "Existen problemas con la informacion del resumen de comprobantes";
                case "2281": return "Error en la validacion de los rangos de los comprobantes";
                case "2282": return "Existe documento ya informado anteriormente ";
                case "2283": return "El dato ingresado no cumple con el formato RA-fecha-correlativo ";
                case "2284": return "El XML no contiene el tag ID";
                case "2285": return "El ID debe coincidir con el nombre del archivo ";
                case "2286": return "El RUC debe coincidir con el RUC del nombre del archivo ";
                case "2287": return "AdditionalAccountID - El dato ingresado no cumple con el estandar ";
                case "2288": return "El XML no contiene el tag AdditionalAccountID del emisor del documento ";
                case "2289": return "CustomerAssignedAccountID - El dato ingresado no cumple con el estándar";
                case "2290": return "El XML no contiene el tag CustomerAssignedAccountID del emisor del documento";
                case "2291": return "El contribuyente no esta autorizado a emitir comprobantes electrónicos";
                case "2292": return "Numero de RUC SOL no coincide con RUC emisor";
                case "2293": return "Numero de RUC del emisor no existe";
                case "2294": return "El contribuyente no esta activo";
                case "2295": return "El contribuyente no cumple con tipo de empresa o tributos requeridos";
                case "2296": return "RegistrationName - El dato ingresado no cumple con el estandar ";
                case "2297": return "El XML no contiene el tag RegistrationName del emisor del documento ";
                case "2298": return "IssueDate - El dato ingresado no cumple con el patron YYYY-MM-DD";
                case "2299": return "El XML no contiene el tag IssueDate ";
                case "2300": return "IssueDate - El dato ingresado no es valido";
                case "2301": return "La fecha del IssueDate no debe ser mayor al Today ";
                case "2302": return "ReferenceDate - El dato ingresado no cumple con el patron YYYY-MM-DD ";
                case "2303": return "El XML no contiene el tag ReferenceDate";
                case "2304": return "ReferenceDate - El dato ingresado no es valido";
                case "2305": return "LineID - El dato ingresado no cumple con el estandar ";
                case "2306": return "LineID - El dato ingresado debe ser correlativo mayor a cero ";
                case "2307": return "El XML no contiene el tag LineID de VoidedDocumentsLine ";
                case "2308": return "DocumentTypeCode - El valor del tipo de documento es invalido ";
                case "2309": return "El XML no contiene el tag DocumentTypeCode ";
                case "2310": return "El dato ingresado no cumple con el patron SERIE ";
                case "2311": return "El XML no contiene el tag DocumentSerialID ";
                case "2312": return "El dato ingresado en DocumentNumberID debe ser numerico y como maximo de 8 digitos ";
                case "2313": return "El XML no contiene el tag DocumentNumberID ";
                case "2314": return "El dato ingresado en VoidReasonDescription debe contener información válida ";
                case "2315": return "El XML no contiene el tag VoidReasonDescription ";
                case "2316": return "Debe indicar Items en VoidedDocumentsLine";
                case "2317": return "Error al procesar el resumen de anulados";
                case "2318": return "CustomizationID - La version del documento no es correcta";
                case "2319": return "El XML no contiene el tag CustomizationID";
                case "2320": return "UBLVersionID - La version del UBL  no es la correcta";
                case "2321": return "El XML no contiene el tag UBLVersionID";
                case "2322": return "Error en la validacion de los rangos";
                case "2323": return "Existe documento ya informado anteriormente en una comunicacion de baja ";
                case "2324": return "El archivo de comunicacion de baja ya fue presentado anteriormente ";
                case "2325": return "El certificado usado no es el comunicado a SUNAT ";
                case "2326": return "El certificado usado se encuentra de baja ";
                case "2327": return "El certificado usado no se encuentra vigente ";
                case "2328": return "El certificado usado se encuentra revocado ";
                case "2329": return "La fecha de emision se encuentra fuera del limite permitido ";
                case "2330": return "La fecha de generación de la comunicación debe ser igual a la fecha consignada en el nombre del archivo ";
                case "2331": return "Número de RUC del nombre del archivo no coincide con el consignado en el contenido del archivo XML";
                case "2332": return "Número de Serie del nombre del archivo no coincide con el consignado en el contenido del archivo XML";
                case "2333": return "Número de documento en el nombre del archivo no coincide con el consignado en el contenido del XML";
                case "2334": return "El documento electrónico ingresado ha sido alterado ";
                case "2335": return "El documento electrónico ingresado ha sido alterado ";
                case "2336": return "Ocurrió un error en el proceso de validación de la firma digital ";
                case "2337": return "La moneda debe ser la misma en todo el documento ";
                case "2338": return "La moneda debe ser la misma en todo el documento ";
                case "2339": return "El dato ingresado en PayableAmount no cumple con el formato establecido ";
                case "2340": return "El valor ingresado en AdditionalMonetaryTotal/cbc:ID es incorrecto ";
                case "2341": return "AdditionalMonetaryTotal/cbc:ID debe tener valor ";
                case "2342": return "Fecha de emision de la factura no coincide con la informada en la comunicacion ";
                case "2343": return "cac:TaxTotal/cac:TaxSubtotal/cbc:TaxAmount - El dato ingresado no cumple con el estandar ";
                case "2344": return "El XML no contiene el tag cac:TaxTotal/cac:TaxSubtotal/cbc:TaxAmount";
                case "2345": return "La serie no corresponde al tipo de comprobante ";
                case "2346": return "La fecha de generación del resumen debe ser igual a la fecha consignada en el nombre del archivo ";
                case "2347": return "Los rangos informados en el archivo XML se encuentran duplicados o superpuestos";
                case "2348": return "Los documentos informados en el archivo XML se encuentran duplicados";
                case "2349": return "Debe consignar solo un elemento sac:AdditionalMonetaryTotal con cbc:ID igual a 1001 ";
                case "2350": return "Debe consignar solo un elemento sac:AdditionalMonetaryTotal con cbc:ID igual a 1002 ";
                case "2351": return "Debe consignar solo un elemento sac:AdditionalMonetaryTotal con cbc:ID igual a 1003 ";
                case "2352": return "Debe consignar solo un elemento cac:TaxTotal a nivel global para IGV (cbc:ID igual a 1000) ";
                case "2353": return "Debe consignar solo un elemento cac:TaxTotal a nivel global para ISC (cbc:ID igual a 2000) ";
                case "2354": return "Debe consignar solo un elemento cac:TaxTotal a nivel global para Otros (cbc:ID igual a 9999) ";
                case "2355": return "Debe consignar solo un elemento cac:TaxTotal a nivel de item para IGV (cbc:ID igual a 1000) ";
                case "2356": return "Debe consignar solo un elemento cac:TaxTotal a nivel de item para ISC (cbc:ID igual a 2000) ";
                case "2357": return "Debe consignar solo un elemento sac:BillingPayment a nivel de item con cbc:InstructionID igual a 01 ";
                case "2358": return "Debe consignar solo un elemento sac:BillingPayment a nivel de item con cbc:InstructionID igual a 02 ";
                case "2359": return "Debe consignar solo un elemento sac:BillingPayment a nivel de item con cbc:InstructionID igual a 03 ";
                case "2360": return "Debe consignar solo un elemento sac:BillingPayment a nivel de item con cbc:InstructionID igual a 04";
                case "2361": return "Debe consignar solo un elemento cac:TaxTotal a nivel de item para Otros (cbc:ID igual a 9999) ";
                case "2362": return "Debe consignar solo un tag cac:AccountingSupplierParty/cbc:AdditionalAccountID ";
                case "2363": return "Debe consignar solo un tag cac:AccountingCustomerParty/cbc:AdditionalAccountID ";
                case "2364": return "El comprobante contiene un tipo y número de Guía de Remisión repetido ";
                case "2365": return "El comprobante contiene un tipo y número de Documento Relacionado repetido ";
                case "2366": return "El codigo en el tag sac:AdditionalProperty/cbc:ID debe tener 4 posiciones";
                case "2367": return "El dato ingresado en PriceAmount del Precio de venta unitario por item no cumple con el formato establecido ";
                case "2368": return "El dato ingresado en TaxSubtotal/cbc:TaxAmount del item no cumple con el formato establecido ";
                case "2369": return "El dato ingresado en PriceAmount del Valor de venta unitario por item no cumple con el formato establecido ";
                case "2370": return "El dato ingresado en LineExtensionAmount del item no cumple con el formato establecido ";
                case "2371": return "El XML no contiene el tag cbc:TaxExemptionReasonCode de Afectacion al IGV ";
                case "2372": return "El tag en el item cac:TaxTotal/cbc:TaxAmount debe tener el mismo valor que cac:TaxTotal/cac:TaxSubtotal/cbc:TaxAmount ";
                case "2373": return "Si existe monto de ISC en el ITEM debe especificar el sistema de calculo ";
                case "2374": return "La factura a dar de baja tiene una fecha de recepcion fuera del plazo permitido ";
                case "2375": return "Fecha de emision de la boleta no coincide con la fecha de emision consignada en la comunicacion ";
                case "2376": return "La boleta de venta a dar de baja fue informada en un resumen con fecha de recepcion fuera del plazo permitido ";
                case "2377": return "El Name o TaxTypeCode debe corresponder con el Id para el IGV ";
                case "2378": return "El Name o TaxTypeCode debe corresponder con el Id para el ISC ";
                case "2379": return "La numeracion de boleta de venta a dar de baja fue generada en una fecha fuera del plazo permitido ";
                case "2380": return "El documento tiene observaciones";
                case "2381": return "Comprobante no cumple con el Grupo 1: No todos los items corresponden a operaciones gravadas a IGV ";
                case "2382": return "Comprobante no cumple con el Grupo 2: No todos los items corresponden a operaciones inafectas o exoneradas al IGV ";
                case "2383": return "Comprobante no cumple con el Grupo 3: Falta leyenda con codigo 1002 ";
                case "2384": return "Comprobante no cumple con el Grupo 3: Existe item con operación onerosa ";
                case "2385": return "Comprobante no cumple con el Grupo 4: Debe exitir Total descuentos mayor a cero ";
                case "2386": return "Comprobante no cumple con el Grupo 5: Todos los items deben tener operaciones afectas a ISC ";
                case "2387": return "Comprobante no cumple con el Grupo 6: El monto de percepcion no existe o es cero ";
                case "2388": return "Comprobante no cumple con el Grupo 6: Todos los items deben tener código de Afectación al IGV igual a 10 ";
                case "2389": return "Comprobante no cumple con el Grupo 7: El codigo de moneda no es diferente a PEN ";
                case "2390": return "Comprobante no cumple con el Grupo 8: No todos los items corresponden a operaciones gravadas a IGV ";
                case "2391": return "Comprobante no cumple con el Grupo 9: No todos los items corresponden a operaciones inafectas o exoneradas al IGV ";
                case "2392": return "Comprobante no cumple con el Grupo 10: Falta leyenda con codigo 1002 ";
                case "2393": return "Comprobante no cumple con el Grupo 10: Existe item con operación onerosa ";
                case "2394": return "Comprobante no cumple con el Grupo 11: Debe existir Total descuentos mayor a cero ";
                case "2395": return "Comprobante no cumple con el Grupo 12: El codigo de moneda no es diferente a PEN ";
                case "2396": return "Si el monto total es mayor a S/. 700.00 debe consignar tipo y numero de documento del adquiriente ";
                case "2397": return "El tipo de documento del adquiriente no puede ser Numero de RUC ";
                case "2398": return "El documento a dar de baja se encuentra rechazado ";
                case "2399": return "El tipo de documento modificado por la Nota de credito debe ser boleta electronica";
                case "2400": return "El tipo de documento modificado por la Nota de debito debe ser boleta electronica ";
                case "2401": return "No se puede leer (parsear) el archivo XML ";
                case "2402": return "El caso de prueba no existe";
                case "2403": return "La numeracion o nombre del documento ya ha sido enviado anteriormente";
                case "2404": return "Documento afectado por la nota electronica no se encuentra autorizado ";
                case "2405": return "Contribuyente no se encuentra autorizado como emisor de boletas electronicas ";
                case "2406": return "Existe mas de un tag sac:AdditionalMonetaryTotal con el mismo ID ";
                case "2407": return "Existe mas de un tag sac:AdditionalProperty con el mismo ID ";
                case "2408": return "El dato ingresado en PriceAmount del Valor referencial unitario por item no cumple con el formato establecido ";
                case "2409": return "Existe mas de un tag cac:AlternativeConditionPrice con el mismo cbc:PriceTypeCode ";
                case "2410": return "Se ha consignado un valor invalido en el campo cbc:PriceTypeCode ";
                case "2411": return "Ha consignado mas de un elemento cac:AllowanceCharge con el mismo campo cbc:ChargeIndicator ";
                case "2412": return "Se ha consignado mas de un documento afectado por la nota (tag cac:BillingReference) ";
                case "2413": return "Se ha consignado mas de un motivo o sustento de la nota (tag cac:DiscrepancyResponse/cbc:Description) ";
                case "2414": return "No se ha consignado en la nota el tag cac:DiscrepancyResponse ";
                case "2415": return "Se ha consignado en la nota mas de un tag cac:DiscrepancyResponse ";
                case "2416": return "Si existe leyenda Transferida Gratuita debe consignar Total Valor de Venta de Operaciones Gratuitas";
                case "2417": return "Debe consignar Valor Referencial unitario por ítem en operaciones no onerosas";
                case "2418": return "Si consigna Valor Referencial unitario por ítem en operaciones no onerosas, la operación debe ser no onerosa";
                case "2419": return "El dato ingresado en AllowanceTotalAmount no cumple con el formato establecido";
                case "2420": return "Ya transcurrieron mas de 25 dias calendarios para concluir con su proceso de homologacion";
                case "4000": return "El documento ya fue presentado anteriormente.";
                case "4001": return "El numero de RUC del receptor no existe. ";
                case "4002": return "Para el TaxTypeCode, esta usando un valor que no existe en el catalogo. ";
                case "4003": return "El comprobante fue registrado previamente como rechazado.";
                case "4004": return "El DocumentTypeCode de las guias debe existir y tener 2 posiciones ";
                case "4005": return "El DocumentTypeCode de las guias debe ser 09 o 31 ";
                case "4006": return "El ID de las guias debe tener informacion de la SERIE-NUMERO de guia. ";
                case "4007": return "El XML no contiene el ID de las guias.";
                case "4008": return "El DocumentTypeCode de Otros documentos relacionados no cumple con el estandar. ";
                case "4009": return "El DocumentTypeCode de Otros documentos relacionados tiene valores incorrectos. ";
                case "4010": return "El ID de los documentos relacionados no cumplen con el estandar. ";
                case "4011": return "El XML no contiene el tag ID de documentos relacionados.";
                case "4012": return "El ubigeo indicado en el comprobante no es el mismo que esta registrado para el contribuyente. ";
                case "4013": return "El RUC del receptor no esta activo ";
                case "4014": return "El RUC del receptor no esta habido ";
                case "4015": return "Si el tipo de documento del receptor no es RUC, debe tener operaciones de exportacion ";
                case "4016": return "El total valor venta neta de oper. gravadas IGV debe ser mayor a 0.00 o debe existir oper. gravadas onerosas ";
                case "4017": return "El total valor venta neta de oper. inafectas IGV debe ser mayor a 0.00 o debe existir oper. inafectas onerosas o de export. ";
                case "4018": return "El total valor venta neta de oper. exoneradas IGV debe ser mayor a 0.00 o debe existir oper. exoneradas ";
                case "4019": return "El calculo del IGV no es correcto ";
                case "4020": return "El ISC no esta informado correctamente ";
                case "4021": return "Si se utiliza la leyenda con codigo 2000, el importe de percepcion debe ser mayor a 0.00 ";
                case "4022": return "Si se utiliza la leyenda con código 2001, el total de operaciones exoneradas debe ser mayor a 0.00 ";
                case "4023": return "Si se utiliza la leyenda con código 2002, el total de operaciones exoneradas debe ser mayor a 0.00 ";
                case "4024": return "Si se utiliza la leyenda con código 2003, el total de operaciones exoneradas debe ser mayor a 0.00 ";
                case "4025": return "Si usa la leyenda de Transferencia o Servivicio gratuito, todos los items deben ser no onerosos ";
                case "4026": return "No se puede indicar Guia de remision de remitente y Guia de remision de transportista en el mismo documento ";
                case "4027": return "El importe total no coincide con la sumatoria de los valores de venta mas los tributos mas los cargos ";
                case "4028": return "El monto total de la nota de credito debe ser menor o igual al monto de la factura ";
                case "4029": return "El ubigeo indicado en el comprobante no es el mismo que esta registrado para el contribuyente ";
                case "4030": return "El ubigeo indicado en el comprobante no es el mismo que esta registrado para el contribuyente ";
                case "4031": return "Debe indicar el nombre comercial ";
                case "4032": return "Si el código del motivo de emisión de la Nota de Credito es 03, debe existir la descripción del item ";
                case "4033": return "La fecha de generación de la numeración debe ser menor o igual a la fecha de generación de la comunicación ";
                case "4034": return "El comprobante fue registrado previamente como baja";
                case "4035": return "El comprobante fue registrado previamente como rechazado";
                case "4036": return "La fecha de emisión de los rangos debe ser menor o igual a la fecha de generación del resumen ";
                case "4037": return "El calculo del Total de IGV del Item no es correcto ";
                case "4038": return "El resumen contiene menos series por tipo de documento que el envío anterior para la misma fecha de emisión ";
                case "4039": return "No ha consignado información del ubigeo del domicilio fiscal ";
                case "4040": return "Si el importe de percepcion es mayor a 0.00, debe utilizar una leyenda con codigo 2000 ";
                case "4041": return "El codigo de pais debe ser PE ";

                default:
                    return "No se encontro la descripcion del código";
            }
        }

        public List<SendFacturasOffline> getFacturasPendientesSendSunat()
        {
            string sql = "select numerodocumento,estado,codigounidad,codigopaciente,replace(pathFile,'PDF','zip')filename from documento where numerodocumento like'F0%' and tipodocumento='01' and isSendSUNAT=0  and convert(date,fechaemitio)=convert(date,getdate())";
            List<SendFacturasOffline> lista = new List<SendFacturasOffline>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    lista.Add(new SendFacturasOffline
                                    {
                                        empresa = Convert.ToInt32(reader["codigounidad"].ToString()),
                                        paciente = Convert.ToInt32(reader["codigopaciente"].ToString()),
                                        filename = (reader["filename"].ToString()),
                                        documento = (reader["numerodocumento"].ToString()),
                                        estado = (reader["estado"].ToString()),
                                        resultado = "PENDIENTE"
                                    });
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                            connection.Close();
                        }
                    }
                }
            }
            return lista;
        }

        public bool verificarCDR(string pathCDR, out string result)
        {
            //descromprimimos el ZIP
            string ruta = Path.GetTempFileName() + @"result\";
            FileInfo fileToDecompress = new FileInfo(pathCDR);
            if (Directory.Exists(ruta))
                Directory.Delete(ruta);
            ZipFile.ExtractToDirectory(pathCDR, ruta);

            //Leer XML
            string filexml = ruta + fileToDecompress.Name.Substring(0, fileToDecompress.Name.Length - 3) + "xml";


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filexml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            nsmgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("cbc:Description");
            XmlElement xelement = (XmlElement)nodeList[0];
            result = "\n Desc. Error: " + xelement.InnerText;

            nodeList = xmlDoc.GetElementsByTagName("cbc:ResponseCode");
            xelement = (XmlElement)nodeList[0];
            result = "Cod. Error: " + xelement.InnerText + result;

            //Directory.Delete(ruta);

            return xelement.InnerText == "0";
        }

        public List<PreResumenBoleta> getListaResumen(string fecha,string unidad)
        {
            List<PreResumenBoleta> lista = new List<PreResumenBoleta>();
            
            if (fecha != null&& unidad != null)
            {
                string queryString = "dbo.getListaResumenBoletas";
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {

                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.Add("@fecha", SqlDbType.VarChar).Value = fecha;
                        command.Parameters.Add("@unidad", SqlDbType.VarChar).Value = unidad;
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        command.Prepare();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    PreResumenBoleta item = new PreResumenBoleta();
                                    item.serie = reader["serie"].ToString();
                                    item.correlativo = Convert.ToInt32(reader["correlativo"].ToString());
                                    item.tipodocumento = Convert.ToInt32(reader["tipodocumento"].ToString());
                                    item.tipoIGV = Convert.ToInt32(reader["tipoIGV"].ToString());
                                    item.preciounitario = Convert.ToDouble(reader["preciounitario"].ToString());
                                    item.valorventa = Convert.ToDouble(reader["valorventa"].ToString());
                                    item.valorigv = Convert.ToDouble(reader["valorigv"].ToString());
                                    item.subtotal = Convert.ToDouble(reader["subtotal"].ToString());
                                    item.descripcion = (reader["descripcion"].ToString());
                                    item.igv = Convert.ToDouble(reader["igv"].ToString());
                                    item.tipodocReferenciaNota = Convert.ToInt32(reader["tipodocReferenciaNota"].ToString());
                                    lista.Add(item);
                                }

                        }
                        finally
                        {
                            // Always call Close when done reading.
                            reader.Close();
                            connection.Close();

                        }
                    }
                }
            }

            return lista;
        }


        public string getNumeroDocumentoResumen(string empresa)
        {
            string correlativo = "";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                string sqlUpdate = "select correlativo+1 numerodocumento from SERIE where idserie='{0}';";

                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {

                    SqlCommand command = new SqlCommand(string.Format(sqlUpdate, (empresa == "1" ? "78" : "79")), connection);
                    connection.Open();
                    command.Prepare();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            correlativo = reader["numerodocumento"].ToString();
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        connection.Dispose();
                        connection.Close();
                    }
                    return correlativo;
                }
            }
        }

        public string crearDocumentoXML(DocumentoSunat documento)
        {
            string[] _numerodoc = documento.numeroDocumento.Split('-');
            string _serie = _numerodoc[0], _correlativo = _numerodoc[1];
            string cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2",
                cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2",
                ccts = "urn:un:unece:uncefact:documentation:2",
                ds = "http://www.w3.org/2000/09/xmldsig#",
                ext = "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2",
                qdt = "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2",
                sac = "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1",
                udt = "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2",
                xsi = "http://www.w3.org/2001/XMLSchema-instance";



            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                documento.numeroDocumentoSUNAT = "F" + documento.numeroDocumento;
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                documento.numeroDocumentoSUNAT = "B" + documento.numeroDocumento;
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO)
            {
                if (documento.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                    documento.numeroDocumentoSUNAT = "F" + documento.numeroDocumento;
                else if (documento.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    documento.numeroDocumentoSUNAT = "B" + documento.numeroDocumento;
                else { throw new Exception("No se asigno ningun tipo de documento"); }
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
            {
                if (documento.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                    documento.numeroDocumentoSUNAT = "F" + documento.numeroDocumento;
                else if (documento.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    documento.numeroDocumentoSUNAT = "B" + documento.numeroDocumento;
                else { throw new Exception("No se asigno ningun tipo de documento"); }
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
            {
                documento.numeroDocumentoSUNAT = "RA-" + documento.numeroDocumento;
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                documento.numeroDocumentoSUNAT = "RC-" + documento.numeroDocumento;
            }
            else { throw new Exception("No se asigno ningun tipo de documento"); }






            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "iso-8859-1", "no"));
            XmlNamespaceManager namespaces = new XmlNamespaceManager(doc.NameTable);
            namespaces.AddNamespace("ext", ext);
            XmlElement invoice;

            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                invoice = doc.CreateElement("Invoice");
                invoice.SetAttribute("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO)
            {
                invoice = doc.CreateElement("CreditNote");
                invoice.SetAttribute("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
            {
                invoice = doc.CreateElement("DebitNote");
                invoice.SetAttribute("xmlns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
            {
                invoice = doc.CreateElement("VoidedDocuments");
                invoice.SetAttribute("xmlns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                invoice = doc.CreateElement("SummaryDocuments");
                invoice.SetAttribute("xmlns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");
            }
            else
            {
                throw new Exception("No se asigno ningun tipo de documento");
            }


            invoice.SetAttribute("xmlns:cac", cac);
            invoice.SetAttribute("xmlns:cbc", cbc);
            invoice.SetAttribute("xmlns:ccts", ccts);
            invoice.SetAttribute("xmlns:ds", ds);
            invoice.SetAttribute("xmlns:ext", ext);
            invoice.SetAttribute("xmlns:qdt", qdt);
            invoice.SetAttribute("xmlns:sac", sac);
            invoice.SetAttribute("xmlns:udt", udt);
            invoice.SetAttribute("xmlns:xsi", xsi);
            doc.AppendChild(invoice);

            XmlElement ublExtensions = doc.CreateElement("ext", "UBLExtensions", ext);


            if (documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA && documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {

                XmlElement ublExtensionsInfoAdicional = doc.CreateElement("ext", "UBLExtension", ext);
                XmlElement ublExtensionsInfoAdicionalContent = doc.CreateElement("ext", "ExtensionContent", ext);
                XmlElement ublExtensionsInfoAdicionalInformation = doc.CreateElement("sac", "AdditionalInformation", sac);

                #region Total Ventas Operaciones Gravadas (1001)
                if (documento.isOpeGravadas)
                {
                    XmlElement operacionesGravadas = doc.CreateElement("sac", "AdditionalMonetaryTotal", sac);
                    XmlElement idGravadas = doc.CreateElement("cbc", "ID", cbc);
                    idGravadas.InnerText = "1001";
                    XmlElement payableAmountGravadas = doc.CreateElement("cbc", "PayableAmount", cbc);
                    payableAmountGravadas.SetAttribute("currencyID", documento.tipoMoneda);
                    payableAmountGravadas.InnerText = (documento.totalVenta_OpeGravadas - documento.desc_TotalVenta_OpeGravadas).ToString();
                    operacionesGravadas.AppendChild(idGravadas);
                    operacionesGravadas.AppendChild(payableAmountGravadas);
                    ublExtensionsInfoAdicionalInformation.AppendChild(operacionesGravadas);
                }
                #endregion

                #region Total Ventas Operaciones Inafectas (1002)
                if (documento.isOpeInafectas)
                {
                    XmlElement operacionesInafectas = doc.CreateElement("sac", "AdditionalMonetaryTotal", sac);
                    XmlElement idInafectas = doc.CreateElement("cbc", "ID", cbc);
                    idInafectas.InnerText = "1002";
                    XmlElement payableAmountInafectas = doc.CreateElement("cbc", "PayableAmount", cbc);
                    payableAmountInafectas.SetAttribute("currencyID", documento.tipoMoneda);
                    payableAmountInafectas.InnerText = (documento.totalVenta_OpeInafectas - documento.desc_TotalVenta_OpeInafectas).ToString();
                    operacionesInafectas.AppendChild(idInafectas);
                    operacionesInafectas.AppendChild(payableAmountInafectas);
                    ublExtensionsInfoAdicionalInformation.AppendChild(operacionesInafectas);
                }
                #endregion

                #region Total Ventas Operaciones Exoneradas (1003)
                if (documento.isOpeExoneradas)
                {
                    XmlElement operacionesExoneradas = doc.CreateElement("sac", "AdditionalMonetaryTotal", sac);
                    XmlElement idExoneradas = doc.CreateElement("cbc", "ID", cbc);
                    idExoneradas.InnerText = "1003";
                    XmlElement payableAmountExoneradas = doc.CreateElement("cbc", "PayableAmount", cbc);
                    payableAmountExoneradas.SetAttribute("currencyID", documento.tipoMoneda);
                    payableAmountExoneradas.InnerText = (documento.totalVenta_OpeExoneradas - documento.desc_TotalVenta_OpeExoneradas).ToString();
                    operacionesExoneradas.AppendChild(idExoneradas);
                    operacionesExoneradas.AppendChild(payableAmountExoneradas);
                    ublExtensionsInfoAdicionalInformation.AppendChild(operacionesExoneradas);
                }
                #endregion

                #region Total Ventas Operaciones Gratuitas (1004)
                if (documento.isOpeGratuitas)
                {
                    XmlElement operacionesGratuitas = doc.CreateElement("sac", "AdditionalMonetaryTotal", sac);
                    XmlElement idGratuitas = doc.CreateElement("cbc", "ID", cbc);
                    idGratuitas.InnerText = "1004";
                    XmlElement payableAmountGratuitas = doc.CreateElement("cbc", "PayableAmount", cbc);
                    payableAmountGratuitas.SetAttribute("currencyID", documento.tipoMoneda);
                    payableAmountGratuitas.InnerText = documento.totalVenta_OpeGratuitas.ToString();
                    operacionesGratuitas.AppendChild(idGratuitas);
                    operacionesGratuitas.AppendChild(payableAmountGratuitas);
                    ublExtensionsInfoAdicionalInformation.AppendChild(operacionesGratuitas);
                }
                #endregion

                #region Total Descuento (2005)
                if (documento.isTotalDescuento)
                {
                    XmlElement totalDescuento = doc.CreateElement("sac", "AdditionalMonetaryTotal", sac);
                    XmlElement idDescuento = doc.CreateElement("cbc", "ID", cbc);
                    idDescuento.InnerText = "2005";
                    XmlElement payableAmountDescuento = doc.CreateElement("cbc", "PayableAmount", cbc);
                    payableAmountDescuento.SetAttribute("currencyID", documento.tipoMoneda);
                    payableAmountDescuento.InnerText = documento.totalDescuento.ToString();
                    totalDescuento.AppendChild(idDescuento);
                    totalDescuento.AppendChild(payableAmountDescuento);
                    ublExtensionsInfoAdicionalInformation.AppendChild(totalDescuento);
                }
                #endregion


                #region Total Venta (1000)
                XmlElement totalVentaTexto = doc.CreateElement("sac", "AdditionalProperty", sac);
                XmlElement idVentaTexto = doc.CreateElement("cbc", "ID", cbc);
                idVentaTexto.InnerText = "1000";
                XmlElement valueVentaTexto = doc.CreateElement("cbc", "Value", cbc);
                valueVentaTexto.InnerText = documento.totalVentaTexto;
                totalVentaTexto.AppendChild(idVentaTexto);
                totalVentaTexto.AppendChild(valueVentaTexto);
                ublExtensionsInfoAdicionalInformation.AppendChild(totalVentaTexto);
                #endregion

                #region Operaciones Gratuitas
                if (documento.isOpeGratuitas)
                {
                    XmlElement totalVentaTextoGratuita = doc.CreateElement("sac", "AdditionalProperty", sac);
                    XmlElement idVentaGratuita = doc.CreateElement("cbc", "ID", cbc);
                    idVentaGratuita.InnerText = "1002";
                    XmlElement valueVentaGratuita = doc.CreateElement("cbc", "Value", cbc);
                    valueVentaGratuita.InnerText = "TRANSFERENCIA GRATUITA DE UN BIEN Y/O SERVICIO PRESTADO GRATUITAMENTE";
                    totalVentaTextoGratuita.AppendChild(idVentaGratuita);
                    totalVentaTextoGratuita.AppendChild(valueVentaGratuita);
                    ublExtensionsInfoAdicionalInformation.AppendChild(totalVentaTextoGratuita);
                }
                #endregion

                #region Total Venta (1000)
                if (documento.isAnticipo)
                {

                    XmlElement sacAnticipo = doc.CreateElement("sac", "SUNATTransaction", sac);
                    XmlElement idsacAntitipo = doc.CreateElement("cbc", "ID", cbc);
                    idsacAntitipo.InnerText = "04";
                    sacAnticipo.AppendChild(idsacAntitipo);
                    ublExtensionsInfoAdicionalInformation.AppendChild(sacAnticipo);

                }
                #endregion

                ublExtensionsInfoAdicionalContent.AppendChild(ublExtensionsInfoAdicionalInformation);
                ublExtensionsInfoAdicional.AppendChild(ublExtensionsInfoAdicionalContent);
                ublExtensions.AppendChild(ublExtensionsInfoAdicional);
            }
            #region Firma
            XmlElement ublExtensionsSign = doc.CreateElement("ext", "UBLExtension", ext);
            XmlElement ublExtensionsInfoAdicionalContentSign = doc.CreateElement("ext", "ExtensionContent", ext);
            ublExtensionsInfoAdicionalContentSign.InnerText = "";
            ublExtensionsSign.AppendChild(ublExtensionsInfoAdicionalContentSign);
            ublExtensions.AppendChild(ublExtensionsSign);
            invoice.AppendChild(ublExtensions);
            #endregion

            XmlElement versionUBL = doc.CreateElement("cbc", "UBLVersionID", cbc);
            versionUBL.InnerText = "2.0";
            invoice.AppendChild(versionUBL);

            XmlElement versionEstructura = doc.CreateElement("cbc", "CustomizationID", cbc);
            versionEstructura.InnerText = "1.0";
            invoice.AppendChild(versionEstructura);

            XmlElement numeroDocumento = doc.CreateElement("cbc", "ID", cbc);
            numeroDocumento.InnerText = documento.numeroDocumentoSUNAT;
            invoice.AppendChild(numeroDocumento);
            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                XmlElement fechaRefencia = doc.CreateElement("cbc", "ReferenceDate", cbc);
                fechaRefencia.InnerText = documento.fechaReferenciaDocumento;
                invoice.AppendChild(fechaRefencia);
            }
            XmlElement fechaEmision = doc.CreateElement("cbc", "IssueDate", cbc);
            fechaEmision.InnerText = documento.fechaEmision.ToString("yyyy-MM-dd");
            invoice.AppendChild(fechaEmision);

            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                XmlElement tipoDocumento = doc.CreateElement("cbc", "InvoiceTypeCode", cbc);
                tipoDocumento.InnerText = documento.tipoDocumento.ToString("D2");
                invoice.AppendChild(tipoDocumento);
            }
            if (documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA && documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                XmlElement tipoMoneda = doc.CreateElement("cbc", "DocumentCurrencyCode", cbc);
                tipoMoneda.InnerText = documento.tipoMoneda;
                invoice.AppendChild(tipoMoneda);
            }

            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
            {
                XmlElement DiscrepancyResponse = doc.CreateElement("cac", "DiscrepancyResponse", cac);
                XmlElement ReferenceID = doc.CreateElement("cbc", "ReferenceID", cbc);
                ReferenceID.InnerText = documento.numeroDocumentoReferencia;
                DiscrepancyResponse.AppendChild(ReferenceID);
                XmlElement ResponseCode = doc.CreateElement("cbc", "ResponseCode", cbc);
                ResponseCode.InnerText = documento.tipoNotaCreditoDebito.ToString("D2");
                DiscrepancyResponse.AppendChild(ResponseCode);
                XmlElement Description = doc.CreateElement("cbc", "Description", cbc);
                Description.InnerText = documento.descripcionNotaCreditoDebito;
                DiscrepancyResponse.AppendChild(Description);
                invoice.AppendChild(DiscrepancyResponse);

                XmlElement BillingReference = doc.CreateElement("cac", "BillingReference", cac);
                XmlElement InvoiceDocumentReference = doc.CreateElement("cac", "InvoiceDocumentReference", cac);
                XmlElement ID = doc.CreateElement("cbc", "ID", cbc);
                ID.InnerText = documento.numeroDocumentoReferencia;
                InvoiceDocumentReference.AppendChild(ID);
                XmlElement DocumentTypeCode = doc.CreateElement("cbc", "DocumentTypeCode", cbc);
                DocumentTypeCode.InnerText = documento.tipoDocumentoReferencia.ToString("D2");
                InvoiceDocumentReference.AppendChild(DocumentTypeCode);
                BillingReference.AppendChild(InvoiceDocumentReference);
                invoice.AppendChild(BillingReference);
            }

            #region Datos de la Firma
            XmlElement referenciaFirma = doc.CreateElement("cac", "Signature", cac);
            XmlElement idFirma = doc.CreateElement("cbc", "ID", cbc);
            idFirma.InnerText = "IDSignSP";
            referenciaFirma.AppendChild(idFirma);
            XmlElement datosFirmante = doc.CreateElement("cac", "SignatoryParty", cac);
            XmlElement partyIdentification = doc.CreateElement("cac", "PartyIdentification", cac);
            XmlElement idPartyIdentification = doc.CreateElement("cbc", "ID", cbc);
            idPartyIdentification.InnerText = documento.rucEmisor;
            partyIdentification.AppendChild(idPartyIdentification);
            datosFirmante.AppendChild(partyIdentification);
            XmlElement partyName = doc.CreateElement("cac", "PartyName", cac);
            XmlElement namePartyName = doc.CreateElement("cbc", "Name", cbc);
            namePartyName.InnerText = documento.razonSocialEmisor;
            partyName.AppendChild(namePartyName);
            datosFirmante.AppendChild(partyName);
            referenciaFirma.AppendChild(datosFirmante);

            XmlElement firmaAdjunta = doc.CreateElement("cac", "DigitalSignatureAttachment", cac);
            XmlElement infoFirmaAdjunta = doc.CreateElement("cac", "ExternalReference", cac);
            XmlElement uriFirmaAdjunta = doc.CreateElement("cbc", "URI", cbc);
            uriFirmaAdjunta.InnerText = "#" + documento.idFirmaDigital;//id de la firma generada
            infoFirmaAdjunta.AppendChild(uriFirmaAdjunta);
            firmaAdjunta.AppendChild(infoFirmaAdjunta);
            referenciaFirma.AppendChild(firmaAdjunta);
            invoice.AppendChild(referenciaFirma);
            #endregion

            #region Datos del Emisor
            XmlElement accountingSupplierPartyEmisor = doc.CreateElement("cac", "AccountingSupplierParty", cac);
            XmlElement customerAssignedAccountIDEmisor = doc.CreateElement("cbc", "CustomerAssignedAccountID", cbc);
            customerAssignedAccountIDEmisor.InnerText = documento.rucEmisor;
            accountingSupplierPartyEmisor.AppendChild(customerAssignedAccountIDEmisor);
            XmlElement additionalAccountIDEmisor = doc.CreateElement("cbc", "AdditionalAccountID", cbc);
            additionalAccountIDEmisor.InnerText = documento.tipoDocEmisor;
            accountingSupplierPartyEmisor.AppendChild(additionalAccountIDEmisor);
            XmlElement partyEmisory = doc.CreateElement("cac", "Party", cac);

            if (documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA && documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                XmlElement partyNameEmisor = doc.CreateElement("cac", "PartyName", cac);
                XmlElement namePartyNameEmisor = doc.CreateElement("cbc", "Name", cbc);
                namePartyNameEmisor.InnerText = documento.razonSocialEmisor;
                partyNameEmisor.AppendChild(namePartyNameEmisor);
                partyEmisory.AppendChild(partyNameEmisor);
                XmlElement postalAddressEmisor = doc.CreateElement("cac", "PostalAddress", cac);
                XmlElement idEmisor = doc.CreateElement("cbc", "ID", cbc);
                idEmisor.InnerText = documento.ubigeoEmisor;
                postalAddressEmisor.AppendChild(idEmisor);
                XmlElement calleEmisor = doc.CreateElement("cbc", "StreetName", cbc);
                calleEmisor.InnerText = documento.calleEmisor;
                postalAddressEmisor.AppendChild(calleEmisor);
                XmlElement urbanizacionEmisor = doc.CreateElement("cbc", "CitySubdivisionName", cbc);
                urbanizacionEmisor.InnerText = documento.urbanizacionEmisor;
                postalAddressEmisor.AppendChild(urbanizacionEmisor);
                XmlElement departamentoEmisor = doc.CreateElement("cbc", "CityName", cbc);
                departamentoEmisor.InnerText = documento.departamentoEmisor;
                postalAddressEmisor.AppendChild(departamentoEmisor);
                XmlElement provinciaEmisor = doc.CreateElement("cbc", "CountrySubentity", cbc);
                provinciaEmisor.InnerText = documento.provinciaEmisor;
                postalAddressEmisor.AppendChild(provinciaEmisor);
                XmlElement distritoEmisor = doc.CreateElement("cbc", "District", cbc);
                distritoEmisor.InnerText = documento.distritoEmisor;
                postalAddressEmisor.AppendChild(distritoEmisor);
                XmlElement paisEmisor = doc.CreateElement("cac", "Country", cac);
                XmlElement idpaisEmisor = doc.CreateElement("cbc", "IdentificationCode", cbc);
                idpaisEmisor.InnerText = documento.paisEmisor;
                paisEmisor.AppendChild(idpaisEmisor);
                postalAddressEmisor.AppendChild(paisEmisor);
                partyEmisory.AppendChild(postalAddressEmisor);
            }
            XmlElement razonsocialEmisor = doc.CreateElement("cac", "PartyLegalEntity", cac);
            XmlElement valuerazonsocialEmisor = doc.CreateElement("cbc", "RegistrationName", cbc);
            valuerazonsocialEmisor.InnerText = documento.razonSocialEmisor;
            razonsocialEmisor.AppendChild(valuerazonsocialEmisor);
            partyEmisory.AppendChild(razonsocialEmisor);
            accountingSupplierPartyEmisor.AppendChild(partyEmisory);
            invoice.AppendChild(accountingSupplierPartyEmisor);
            #endregion

            if (documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA && documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                #region Datos del Receptor
                XmlElement accountingCustomerrPartyReceptor = doc.CreateElement("cac", "AccountingCustomerParty", cac);
                XmlElement customerAssignedAccountIDReceptor = doc.CreateElement("cbc", "CustomerAssignedAccountID", cbc);
                customerAssignedAccountIDReceptor.InnerText = documento.rucReceptor;
                accountingCustomerrPartyReceptor.AppendChild(customerAssignedAccountIDReceptor);
                XmlElement additionalAccountIDReceptor = doc.CreateElement("cbc", "AdditionalAccountID", cbc);
                additionalAccountIDReceptor.InnerText = documento.tipoDocReceptor.ToString();
                accountingCustomerrPartyReceptor.AppendChild(additionalAccountIDReceptor);
                XmlElement partyReceptory = doc.CreateElement("cac", "Party", cac);
                XmlElement razonsocialReceptor = doc.CreateElement("cac", "PartyLegalEntity", cac);
                XmlElement valuerazonsocialReceptor = doc.CreateElement("cbc", "RegistrationName", cbc);
                valuerazonsocialReceptor.InnerText = documento.razonSocialReceptor;
                razonsocialReceptor.AppendChild(valuerazonsocialReceptor);
                partyReceptory.AppendChild(razonsocialReceptor);
                accountingCustomerrPartyReceptor.AppendChild(partyReceptory);
                invoice.AppendChild(accountingCustomerrPartyReceptor);
                #endregion

                #region Datos IGV
                if (documento.porcentajeDescuentoGlobal > 0)
                    documento.igvTotal = documento.igvTotal - (documento.igvTotal * (documento.porcentajeDescuentoGlobal / 100));

                XmlElement tax = doc.CreateElement("cac", "TaxTotal", cac);
                XmlElement taxTotal = doc.CreateElement("cbc", "TaxAmount", cbc);
                taxTotal.SetAttribute("currencyID", documento.tipoMoneda);
                taxTotal.InnerText = documento.igvTotal.ToString("######0.#0");
                tax.AppendChild(taxTotal);
                XmlElement taxSubtotal = doc.CreateElement("cac", "TaxSubtotal", cac);
                XmlElement taxTotal2 = doc.CreateElement("cbc", "TaxAmount", cbc);
                taxTotal2.SetAttribute("currencyID", documento.tipoMoneda);
                taxTotal2.InnerText = documento.igvTotal.ToString("######0.#0");
                taxSubtotal.AppendChild(taxTotal2);
                XmlElement taxCategory = doc.CreateElement("cac", "TaxCategory", cac);
                XmlElement taxScheme = doc.CreateElement("cac", "TaxScheme", cac);
                XmlElement taxid = doc.CreateElement("cbc", "ID", cbc);
                taxid.InnerText = "1000";
                taxScheme.AppendChild(taxid);
                XmlElement taxName = doc.CreateElement("cbc", "Name", cbc);
                taxName.InnerText = "IGV";
                taxScheme.AppendChild(taxName);
                XmlElement taxTypeCode = doc.CreateElement("cbc", "TaxTypeCode", cbc);
                taxTypeCode.InnerText = "VAT";
                taxScheme.AppendChild(taxTypeCode);
                taxCategory.AppendChild(taxScheme);
                taxSubtotal.AppendChild(taxCategory);
                tax.AppendChild(taxSubtotal);
                invoice.AppendChild(tax);
                #endregion

                XmlElement RequestedMonetaryTotal;
                if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
                    RequestedMonetaryTotal = doc.CreateElement("cac", "RequestedMonetaryTotal", cac);
                else
                    RequestedMonetaryTotal = doc.CreateElement("cac", "LegalMonetaryTotal", cac);


                if (documento.isTotalDescuento)
                {
                    XmlElement AllowanceTotalAmount = doc.CreateElement("cbc", "AllowanceTotalAmount", cbc);
                    AllowanceTotalAmount.SetAttribute("currencyID", documento.tipoMoneda);
                    AllowanceTotalAmount.InnerText = documento.totalDescuento.ToString();
                    RequestedMonetaryTotal.AppendChild(AllowanceTotalAmount);
                }


                XmlElement payableAmount = doc.CreateElement("cbc", "PayableAmount", cbc);
                payableAmount.SetAttribute("currencyID", documento.tipoMoneda);
                payableAmount.InnerText = documento.ventaTotal.ToString();
                RequestedMonetaryTotal.AppendChild(payableAmount);
                invoice.AppendChild(RequestedMonetaryTotal);

            }

            int countItem = 1;
            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
            {
                #region Resumen Diario de Boletas
                foreach (var item in documento.detalleResumenBoleta)
                {
                    XmlElement invoiceLine = doc.CreateElement("sac", "SummaryDocumentsLine", sac);
                    XmlElement invoiceLineID = doc.CreateElement("cbc", "LineID", cbc);
                    invoiceLineID.InnerText = countItem.ToString();
                    invoiceLine.AppendChild(invoiceLineID);
                    XmlElement invoiceLineDocumentTypeCode = doc.CreateElement("cbc", "DocumentTypeCode", cbc);
                    invoiceLineDocumentTypeCode.InnerText = item.tipodocumento.ToString("D2");
                    invoiceLine.AppendChild(invoiceLineDocumentTypeCode);
                    XmlElement invoiceLineDocumentSerialID = doc.CreateElement("sac", "DocumentSerialID", sac);
                    invoiceLineDocumentSerialID.InnerText = item.serie;
                    invoiceLine.AppendChild(invoiceLineDocumentSerialID);
                    XmlElement invoiceLineStartDocumentNumberID = doc.CreateElement("sac", "StartDocumentNumberID", sac);
                    invoiceLineStartDocumentNumberID.InnerText = item.inicioRango;
                    invoiceLine.AppendChild(invoiceLineStartDocumentNumberID);
                    XmlElement invoiceLineEndDocumentNumberID = doc.CreateElement("sac", "EndDocumentNumberID", sac);
                    invoiceLineEndDocumentNumberID.InnerText = item.finRango;
                    invoiceLine.AppendChild(invoiceLineEndDocumentNumberID);
                    XmlElement invoiceLineTotalAmount = doc.CreateElement("sac", "TotalAmount", sac);
                    invoiceLineTotalAmount.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLineTotalAmount.InnerText = item.totalVenta.ToString("######0.#0");
                    invoiceLine.AppendChild(invoiceLineTotalAmount);

                    XmlElement invoiceLineBillingPayment01 = doc.CreateElement("sac", "BillingPayment", sac);
                    XmlElement invoiceLineBillingPaymentPaidAmount01 = doc.CreateElement("cbc", "PaidAmount", cbc);
                    invoiceLineBillingPaymentPaidAmount01.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLineBillingPaymentPaidAmount01.InnerText = item.totalVentaGravadas.ToString("#######0.#0");
                    invoiceLineBillingPayment01.AppendChild(invoiceLineBillingPaymentPaidAmount01);
                    XmlElement invoiceLineBillingPaymentInstructionID01 = doc.CreateElement("cbc", "InstructionID", cbc);
                    invoiceLineBillingPaymentInstructionID01.InnerText = "01";//IGV GRAVADAS
                    invoiceLineBillingPayment01.AppendChild(invoiceLineBillingPaymentInstructionID01);
                    invoiceLine.AppendChild(invoiceLineBillingPayment01);

                    XmlElement invoiceLineBillingPayment02 = doc.CreateElement("sac", "BillingPayment", sac);
                    XmlElement invoiceLineBillingPaymentPaidAmount02 = doc.CreateElement("cbc", "PaidAmount", cbc);
                    invoiceLineBillingPaymentPaidAmount02.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLineBillingPaymentPaidAmount02.InnerText = item.totalVentaExonerada.ToString("#######0.#0");
                    invoiceLineBillingPayment02.AppendChild(invoiceLineBillingPaymentPaidAmount02);
                    XmlElement invoiceLineBillingPaymentInstructionID02 = doc.CreateElement("cbc", "InstructionID", cbc);
                    invoiceLineBillingPaymentInstructionID02.InnerText = "02";//IGV EXONERADAS
                    invoiceLineBillingPayment02.AppendChild(invoiceLineBillingPaymentInstructionID02);
                    invoiceLine.AppendChild(invoiceLineBillingPayment02);

                    XmlElement invoiceLineBillingPayment03 = doc.CreateElement("sac", "BillingPayment", sac);
                    XmlElement invoiceLineBillingPaymentPaidAmount03 = doc.CreateElement("cbc", "PaidAmount", cbc);
                    invoiceLineBillingPaymentPaidAmount03.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLineBillingPaymentPaidAmount03.InnerText = item.totalVentaInafectas.ToString("#######0.#0");
                    invoiceLineBillingPayment03.AppendChild(invoiceLineBillingPaymentPaidAmount03);
                    XmlElement invoiceLineBillingPaymentInstructionID03 = doc.CreateElement("cbc", "InstructionID", cbc);
                    invoiceLineBillingPaymentInstructionID03.InnerText = "03";//IGV INAFECTAS
                    invoiceLineBillingPayment03.AppendChild(invoiceLineBillingPaymentInstructionID03);
                    invoiceLine.AppendChild(invoiceLineBillingPayment03);

                    XmlElement invoiceLineDesxuento = doc.CreateElement("cac", "AllowanceCharge", cac);
                    XmlElement invoiceLineDesxuentoTipoIndicador = doc.CreateElement("cbc", "ChargeIndicator", cbc);
                    invoiceLineDesxuentoTipoIndicador.InnerText = "true";
                    invoiceLineDesxuento.AppendChild(invoiceLineDesxuentoTipoIndicador);
                    XmlElement invoiceLineDesxuentoPrecio = doc.CreateElement("cbc", "Amount", cbc);
                    invoiceLineDesxuentoPrecio.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLineDesxuentoPrecio.InnerText = (item.totalOtrosCargos).ToString("######0.#0");
                    invoiceLineDesxuento.AppendChild(invoiceLineDesxuentoPrecio);
                    invoiceLine.AppendChild(invoiceLineDesxuento);


                    XmlElement invoiceLineTax = doc.CreateElement("cac", "TaxTotal", cac);
                    XmlElement invoiceLinetaxTotal = doc.CreateElement("cbc", "TaxAmount", cbc);
                    invoiceLinetaxTotal.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLinetaxTotal.InnerText = item.totalISC.ToString("######0.#0");
                    invoiceLineTax.AppendChild(invoiceLinetaxTotal);
                    XmlElement invoiceLinetaxSubtotal = doc.CreateElement("cac", "TaxSubtotal", cac);
                    XmlElement invoiceLinetaxTotal2 = doc.CreateElement("cbc", "TaxAmount", cbc);
                    invoiceLinetaxTotal2.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLinetaxTotal2.InnerText = item.totalISC.ToString("######0.#0");
                    invoiceLinetaxSubtotal.AppendChild(invoiceLinetaxTotal2);
                    XmlElement invoiceLinetaxCategory = doc.CreateElement("cac", "TaxCategory", cac);
                    XmlElement invoiceLinetaxScheme = doc.CreateElement("cac", "TaxScheme", cac);
                    XmlElement invoiceLinetaxid = doc.CreateElement("cbc", "ID", cbc);
                    invoiceLinetaxid.InnerText = "2000";
                    invoiceLinetaxScheme.AppendChild(invoiceLinetaxid);
                    XmlElement invoiceLinetaxName = doc.CreateElement("cbc", "Name", cbc);
                    invoiceLinetaxName.InnerText = "ISC";
                    invoiceLinetaxScheme.AppendChild(invoiceLinetaxName);
                    XmlElement invoiceLinetaxTypeCode = doc.CreateElement("cbc", "TaxTypeCode", cbc);
                    invoiceLinetaxTypeCode.InnerText = "EXC";
                    invoiceLinetaxScheme.AppendChild(invoiceLinetaxTypeCode);
                    invoiceLinetaxCategory.AppendChild(invoiceLinetaxScheme);
                    invoiceLinetaxSubtotal.AppendChild(invoiceLinetaxCategory);
                    invoiceLineTax.AppendChild(invoiceLinetaxSubtotal);
                    invoiceLine.AppendChild(invoiceLineTax);

                    XmlElement invoiceLineTax10 = doc.CreateElement("cac", "TaxTotal", cac);
                    XmlElement invoiceLinetaxTotal10 = doc.CreateElement("cbc", "TaxAmount", cbc);
                    invoiceLinetaxTotal10.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLinetaxTotal10.InnerText = item.totalIGV.ToString("######0.#0");
                    invoiceLineTax10.AppendChild(invoiceLinetaxTotal10);
                    XmlElement invoiceLinetaxSubtotal10 = doc.CreateElement("cac", "TaxSubtotal", cac);
                    XmlElement invoiceLinetaxTotal210 = doc.CreateElement("cbc", "TaxAmount", cbc);
                    invoiceLinetaxTotal210.SetAttribute("currencyID", documento.tipoMoneda);
                    invoiceLinetaxTotal210.InnerText = item.totalIGV.ToString("######0.#0");
                    invoiceLinetaxSubtotal10.AppendChild(invoiceLinetaxTotal210);
                    XmlElement invoiceLinetaxCategory10 = doc.CreateElement("cac", "TaxCategory", cac);
                    XmlElement invoiceLinetaxScheme10 = doc.CreateElement("cac", "TaxScheme", cac);
                    XmlElement invoiceLinetaxid10 = doc.CreateElement("cbc", "ID", cbc);
                    invoiceLinetaxid10.InnerText = "1000";
                    invoiceLinetaxScheme10.AppendChild(invoiceLinetaxid10);
                    XmlElement invoiceLinetaxName10 = doc.CreateElement("cbc", "Name", cbc);
                    invoiceLinetaxName10.InnerText = "IGV";
                    invoiceLinetaxScheme10.AppendChild(invoiceLinetaxName10);
                    XmlElement invoiceLinetaxTypeCode10 = doc.CreateElement("cbc", "TaxTypeCode", cbc);
                    invoiceLinetaxTypeCode10.InnerText = "VAT";
                    invoiceLinetaxScheme10.AppendChild(invoiceLinetaxTypeCode10);
                    invoiceLinetaxCategory10.AppendChild(invoiceLinetaxScheme10);
                    invoiceLinetaxSubtotal10.AppendChild(invoiceLinetaxCategory10);
                    invoiceLineTax10.AppendChild(invoiceLinetaxSubtotal10);
                    invoiceLine.AppendChild(invoiceLineTax10);

                    if (item.totalotrosTributos > 0)
                    {
                        XmlElement invoiceLineTax99 = doc.CreateElement("cac", "TaxTotal", cac);
                        XmlElement invoiceLinetaxTotal99 = doc.CreateElement("cbc", "TaxAmount", cbc);
                        invoiceLinetaxTotal99.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLinetaxTotal99.InnerText = item.totalotrosTributos.ToString("######0.#0");
                        invoiceLineTax99.AppendChild(invoiceLinetaxTotal99);
                        XmlElement invoiceLinetaxSubtotal99 = doc.CreateElement("cac", "TaxSubtotal", cac);
                        XmlElement invoiceLinetaxTotal299 = doc.CreateElement("cbc", "TaxAmount", cbc);
                        invoiceLinetaxTotal299.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLinetaxTotal299.InnerText = item.totalotrosTributos.ToString("######0.#0");
                        invoiceLinetaxSubtotal99.AppendChild(invoiceLinetaxTotal299);
                        XmlElement invoiceLinetaxCategory99 = doc.CreateElement("cac", "TaxCategory", cac);
                        XmlElement invoiceLinetaxScheme99 = doc.CreateElement("cac", "TaxScheme", cac);
                        XmlElement invoiceLinetaxid99 = doc.CreateElement("cbc", "ID", cbc);
                        invoiceLinetaxid99.InnerText = "9999";
                        invoiceLinetaxScheme99.AppendChild(invoiceLinetaxid99);
                        XmlElement invoiceLinetaxName99 = doc.CreateElement("cbc", "Name", cbc);
                        invoiceLinetaxName99.InnerText = "OTROS";
                        invoiceLinetaxScheme99.AppendChild(invoiceLinetaxName99);
                        XmlElement invoiceLinetaxTypeCode99 = doc.CreateElement("cbc", "TaxTypeCode", cbc);
                        invoiceLinetaxTypeCode99.InnerText = "OTH";
                        invoiceLinetaxScheme99.AppendChild(invoiceLinetaxTypeCode99);
                        invoiceLinetaxCategory99.AppendChild(invoiceLinetaxScheme99);
                        invoiceLinetaxSubtotal99.AppendChild(invoiceLinetaxCategory99);
                        invoiceLineTax99.AppendChild(invoiceLinetaxSubtotal99);
                        invoiceLine.AppendChild(invoiceLineTax99);
                    }



                    invoice.AppendChild(invoiceLine);
                    countItem++;
                }
                #endregion
            }
            else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
            {
                //#region Cancelacion Documentos
                //foreach (var item in documento.detalleCancelar)
                //{
                //    XmlElement invoiceLine = doc.CreateElement("sac", "VoidedDocumentsLine", sac);
                //    XmlElement invoiceLineID = doc.CreateElement("cbc", "LineID", cbc);
                //    invoiceLineID.InnerText = countItem.ToString();
                //    invoiceLine.AppendChild(invoiceLineID);
                //    XmlElement invoiceLineDocumentTypeCode = doc.CreateElement("cbc", "DocumentTypeCode", cbc);
                //    invoiceLineDocumentTypeCode.InnerText = item.tipodocumento.ToString("D2");
                //    invoiceLine.AppendChild(invoiceLineDocumentTypeCode);
                //    XmlElement invoiceLineDocumentSerialID = doc.CreateElement("sac", "DocumentSerialID", sac);
                //    invoiceLineDocumentSerialID.InnerText = item.serie;
                //    invoiceLine.AppendChild(invoiceLineDocumentSerialID);
                //    XmlElement invoiceLineDocumentNumberID = doc.CreateElement("sac", "DocumentNumberID", sac);
                //    invoiceLineDocumentNumberID.InnerText = item.correlativo;
                //    invoiceLine.AppendChild(invoiceLineDocumentNumberID);
                //    XmlElement invoiceLineVoidReasonDescription = doc.CreateElement("sac", "VoidReasonDescription", sac);
                //    invoiceLineVoidReasonDescription.InnerText = item.motivo;
                //    invoiceLine.AppendChild(invoiceLineVoidReasonDescription);
                //    invoice.AppendChild(invoiceLine);
                //    countItem++;
                //}
                //#endregion
            }
            else
            {
                #region Detalle Documento
                foreach (var item in documento.detalleItems)
                {
                    if (item.tipoIGV != (int)TIPO_IGV.NO_DECLARAR_SUNAT)
                    {
                        XmlElement invoiceLine;
                        if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                            invoiceLine = doc.CreateElement("cac", "InvoiceLine", cac);
                        else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO)
                            invoiceLine = doc.CreateElement("cac", "CreditNoteLine", cac);
                        else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
                            invoiceLine = doc.CreateElement("cac", "DebitNoteLine", cac);
                        else { throw new Exception("No se asigno ningun tipo de documento"); }

                        XmlElement invoiceLineID = doc.CreateElement("cbc", "ID", cbc);
                        invoiceLineID.InnerText = countItem.ToString();
                        invoiceLine.AppendChild(invoiceLineID);

                        XmlElement invoiceLineQuantity;
                        if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                        {
                            invoiceLineQuantity = doc.CreateElement("cbc", "InvoicedQuantity", cbc);
                        }
                        else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO)
                        {
                            invoiceLineQuantity = doc.CreateElement("cbc", "CreditedQuantity", cbc);
                        }
                        else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
                        {
                            invoiceLineQuantity = doc.CreateElement(" cbc", "DebitedQuantity", cbc);
                        }
                        else { throw new Exception("No se asigno ningun tipo de documento"); }

                        invoiceLineQuantity.SetAttribute("unitCode", "NIU");
                        invoiceLineQuantity.InnerText = item.cantidad.ToString();
                        invoiceLine.AppendChild(invoiceLineQuantity);
                        XmlElement invoiceLineAmount = doc.CreateElement("cbc", "LineExtensionAmount", cbc);
                        invoiceLineAmount.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLineAmount.InnerText = Math.Round(item.valorVenta - (item.valorVenta * (documento.porcentajeDescuentoGlobal / 100)), 2).ToString("######0.#0");
                        invoiceLine.AppendChild(invoiceLineAmount);
                        XmlElement invoiceLinePricingReference = doc.CreateElement("cac", "PricingReference", cac);
                        XmlElement invoiceLineConditionPrice = doc.CreateElement("cac", "AlternativeConditionPrice", cac);
                        XmlElement invoiceLinePriceAmount = doc.CreateElement("cbc", "PriceAmount", cbc);
                        invoiceLinePriceAmount.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLinePriceAmount.InnerText = (item.valorUnitarioigv).ToString();
                        invoiceLineConditionPrice.AppendChild(invoiceLinePriceAmount);
                        XmlElement invoiceLinePriceType = doc.CreateElement("cbc", "PriceTypeCode", cbc);
                        invoiceLinePriceType.InnerText = "01";
                        invoiceLineConditionPrice.AppendChild(invoiceLinePriceType);
                        invoiceLinePricingReference.AppendChild(invoiceLineConditionPrice);
                        if (item.isGratuita)
                        {
                            XmlElement invoiceLineConditionPriceGratiuta = doc.CreateElement("cac", "AlternativeConditionPrice", cac);
                            XmlElement invoiceLinePriceAmountGratuita = doc.CreateElement("cbc", "PriceAmount", cbc);
                            invoiceLinePriceAmountGratuita.SetAttribute("currencyID", documento.tipoMoneda);
                            invoiceLinePriceAmountGratuita.InnerText = (item.valorReferencialIGV).ToString();
                            invoiceLineConditionPriceGratiuta.AppendChild(invoiceLinePriceAmountGratuita);
                            XmlElement invoiceLinePriceTypeGratuita = doc.CreateElement("cbc", "PriceTypeCode", cbc);
                            invoiceLinePriceTypeGratuita.InnerText = "02";
                            invoiceLineConditionPriceGratiuta.AppendChild(invoiceLinePriceTypeGratuita);
                            invoiceLinePricingReference.AppendChild(invoiceLineConditionPriceGratiuta);
                        }
                        invoiceLine.AppendChild(invoiceLinePricingReference);

                        /*if (item.isCortesia || item.porcentajeDescPromocion > 0)
                        {
                            XmlElement invoiceLineDesxuento = doc.CreateElement("cac", "AllowanceCharge", cac);
                            XmlElement invoiceLineDesxuentoTipoIndicador = doc.CreateElement("cbc", "ChargeIndicator", cbc);
                            invoiceLineDesxuentoTipoIndicador.InnerText = "false";
                            invoiceLineDesxuento.AppendChild(invoiceLineDesxuentoTipoIndicador);
                            XmlElement invoiceLineDesxuentoPrecio = doc.CreateElement("cbc", "Amount", cbc);
                            invoiceLineDesxuentoPrecio.SetAttribute("currencyID", documento.tipoMoneda);
                            invoiceLineDesxuentoPrecio.InnerText = (item.descxItem + item.descPromocion).ToString();
                            invoiceLineDesxuento.AppendChild(invoiceLineDesxuentoPrecio);
                            invoiceLine.AppendChild(invoiceLineDesxuento);
                        }*/
                        XmlElement invoiceLineTax = doc.CreateElement("cac", "TaxTotal", cac);
                        XmlElement invoiceLinetaxTotal = doc.CreateElement("cbc", "TaxAmount", cbc);
                        invoiceLinetaxTotal.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLinetaxTotal.InnerText = item.igvItem_old.ToString("######0.#0");
                        invoiceLineTax.AppendChild(invoiceLinetaxTotal);
                        XmlElement invoiceLinetaxSubtotal = doc.CreateElement("cac", "TaxSubtotal", cac);
                        XmlElement invoiceLinetaxTotal2 = doc.CreateElement("cbc", "TaxAmount", cbc);
                        invoiceLinetaxTotal2.SetAttribute("currencyID", documento.tipoMoneda);
                        invoiceLinetaxTotal2.InnerText = item.igvItem_old.ToString("######0.#0");
                        invoiceLinetaxSubtotal.AppendChild(invoiceLinetaxTotal2);
                        XmlElement invoiceLinetaxCategory = doc.CreateElement("cac", "TaxCategory", cac);
                        XmlElement invoiceLinetaxSubtotalTipoIGV = doc.CreateElement("cbc", "TaxExemptionReasonCode", cbc);
                        invoiceLinetaxSubtotalTipoIGV.InnerText = item.tipoIGV.ToString();
                        invoiceLinetaxCategory.AppendChild(invoiceLinetaxSubtotalTipoIGV);
                        XmlElement invoiceLinetaxScheme = doc.CreateElement("cac", "TaxScheme", cac);
                        XmlElement invoiceLinetaxid = doc.CreateElement("cbc", "ID", cbc);
                        invoiceLinetaxid.InnerText = "1000";
                        invoiceLinetaxScheme.AppendChild(invoiceLinetaxid);
                        XmlElement invoiceLinetaxName = doc.CreateElement("cbc", "Name", cbc);
                        invoiceLinetaxName.InnerText = "IGV";
                        invoiceLinetaxScheme.AppendChild(invoiceLinetaxName);
                        XmlElement invoiceLinetaxTypeCode = doc.CreateElement("cbc", "TaxTypeCode", cbc);
                        invoiceLinetaxTypeCode.InnerText = "VAT";
                        invoiceLinetaxScheme.AppendChild(invoiceLinetaxTypeCode);
                        invoiceLinetaxCategory.AppendChild(invoiceLinetaxScheme);
                        invoiceLinetaxSubtotal.AppendChild(invoiceLinetaxCategory);
                        invoiceLineTax.AppendChild(invoiceLinetaxSubtotal);
                        invoiceLine.AppendChild(invoiceLineTax);

                        XmlElement invoiceItem = doc.CreateElement("cac", "Item", cac);
                        XmlElement invoiceDescripcion = doc.CreateElement("cbc", "Description", cbc);
                        invoiceDescripcion.InnerText = item.descripcion;
                        invoiceItem.AppendChild(invoiceDescripcion);
                        XmlElement invoiceIdentitifation = doc.CreateElement("cac", "SellersItemIdentification", cac);
                        XmlElement invoiceIdentitifationID = doc.CreateElement("cbc", "ID", cbc);
                        invoiceIdentitifationID.InnerText = item.codigoitem;
                        invoiceIdentitifation.AppendChild(invoiceIdentitifationID);
                        invoiceItem.AppendChild(invoiceIdentitifation);
                        invoiceLine.AppendChild(invoiceItem);


                        XmlElement invoicePrice = doc.CreateElement("cac", "Price", cac);
                        XmlElement invoicePriceAmount = doc.CreateElement("cbc", "PriceAmount", cbc);
                        invoicePriceAmount.SetAttribute("currencyID", documento.tipoMoneda);
                        invoicePriceAmount.InnerText = item.valorUnitario.ToString();
                        invoicePrice.AppendChild(invoicePriceAmount);
                        invoiceLine.AppendChild(invoicePrice);

                        invoice.AppendChild(invoiceLine);
                        countItem++;
                    }
                }
                #endregion
            }

            string ruta = System.AppDomain.CurrentDomain.BaseDirectory;


            string pathMain = PathDocumentosFacturacion;
            string pathXML = Path.GetTempPath() + documento.codigopaciente.ToString();
            string pathZIP = pathMain + documento.codigopaciente.ToString() + "\\" + "\\ZIP";
            string pathPDF = pathMain + documento.codigopaciente.ToString() + "\\" + "\\PDF";
            string pathRESULT = pathMain + documento.codigopaciente.ToString() + "\\" + "\\RESULT";
            string pathCODEBAR = pathMain + documento.codigopaciente.ToString() + "\\" + "\\PDF417";
            string pathBAJA = pathMain + "\\BAJA";
            string pathRESUMEN = pathMain + "\\RESUMEN";
            string filename = "";
            if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA || documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
                filename = documento.rucEmisor + "-" + documento.numeroDocumentoSUNAT;
            else
                filename = documento.rucEmisor + "-" + documento.tipoDocumento.ToString("D2") + "-" + documento.numeroDocumentoSUNAT;

            string local_xmlArchivo = pathXML + "\\" + filename + ".xml";
            try
            {
                if (!Directory.Exists(pathMain))
                    Directory.CreateDirectory(pathMain);

                if (Directory.Exists(pathXML))
                    Directory.Delete(pathXML, true);

                Directory.CreateDirectory(pathXML);

                if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
                {
                    if (!Directory.Exists(pathRESUMEN))
                        Directory.CreateDirectory(pathRESUMEN);
                }
                else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
                {
                    if (!Directory.Exists(pathBAJA))
                        Directory.CreateDirectory(pathBAJA);
                }
                else
                {
                    if (!Directory.Exists(pathZIP))
                        Directory.CreateDirectory(pathZIP);

                    if (!Directory.Exists(pathPDF))
                        Directory.CreateDirectory(pathPDF);

                    if (!Directory.Exists(pathRESULT))
                        Directory.CreateDirectory(pathRESULT);

                    if (!Directory.Exists(pathCODEBAR))
                        Directory.CreateDirectory(pathCODEBAR);

                }

                doc.Save(local_xmlArchivo);

                try
                {
                    string local_typoDocumento;
                    string local_nombreXML = System.IO.Path.GetFileName(local_xmlArchivo);
                    if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
                        local_typoDocumento = "RA";
                    else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
                        local_typoDocumento = "RC";
                    else
                        local_typoDocumento = documento.tipoDocumento.ToString("D2");

                    //X509Certificate2 MiCertificado = new X509Certificate2(System.AppDomain.CurrentDomain.BaseDirectory + "\\DAO\\Cer-Reso\\MPS20151222232684.pfx", "RESO137CLI12");
                    X509Certificate2 MiCertificado;
                    if (documento.empresa == 1)
                        MiCertificado = new X509Certificate2(PathCERTIFICADORESO, "RESO137CLI12");
                    else
                        MiCertificado = new X509Certificate2(PathCERTIFICADOEMETAC, "EMET137CLI12");
                    //X509Certificate2 MiCertificado = new X509Certificate2(System.AppDomain.CurrentDomain.BaseDirectory + "\\DAO\\Cer-Emet\\MPS20160829351646.pfx", "EMET137CLI12");


                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(local_xmlArchivo);

                    SignedXml signedXml = new SignedXml(xmlDoc);

                    signedXml.SigningKey = MiCertificado.PrivateKey;

                    KeyInfo KeyInfo = new KeyInfo();

                    Reference _Reference = new Reference();
                    _Reference.Uri = "";

                    _Reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

                    signedXml.AddReference(_Reference);

                    X509Chain X509Chain = new X509Chain();
                    X509Chain.Build(MiCertificado);

                    X509ChainElement local_element = X509Chain.ChainElements[0];
                    KeyInfoX509Data x509Data = new KeyInfoX509Data(local_element.Certificate);
                    String subjectName = local_element.Certificate.Subject;

                    x509Data.AddSubjectName(subjectName);
                    KeyInfo.AddClause(x509Data);

                    signedXml.KeyInfo = KeyInfo;
                    signedXml.ComputeSignature();

                    XmlElement signature = signedXml.GetXml();
                    signature.Prefix = "ds";

                    /*foreach (XmlNode node in signature.SelectNodes(
      "descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
                    {
                        node.Prefix = "ds";
                    }*/
                    signedXml.ComputeSignature();
                    foreach (XmlNode node in signature.SelectNodes("descendant-or-self::*[namespace-uri()='http://www.w3.org/2000/09/xmldsig#']"))
                        if (node.LocalName == "Signature")
                        {
                            XmlAttribute newAttribute = xmlDoc.CreateAttribute("Id");
                            newAttribute.Value = documento.idFirmaDigital;
                            node.Attributes.Append(newAttribute);
                            break;
                        }


                    string local_xpath = "";
                    XmlNamespaceManager nsMgr;
                    nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsMgr.AddNamespace("sac", "urn:sunat:names:specification:ubl:peru:schema:xsd:SunatAggregateComponents-1");
                    nsMgr.AddNamespace("ccts", "urn:un:unece:uncefact:documentation:2");
                    nsMgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");


                    switch (local_typoDocumento)
                    {
                        case "01":
                        case "03":
                            //factura / boleta
                            nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                            local_xpath = "/tns:Invoice/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                            break;
                        case "07":
                            //credit note
                            nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2");
                            local_xpath = "/tns:CreditNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                            break;
                        case "08":
                            //debit note
                            nsMgr.AddNamespace("tns", "urn:oasis:names:specification:ubl:schema:xsd:DebitNote-2");
                            local_xpath = "/tns:DebitNote/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                            break;
                        case "20":
                            //Retencion
                            nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:Retention-1");
                            local_xpath = "/tns:Retention/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                            break;
                        case "40":
                            //Percepcion
                            nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:Perception-1");
                            local_xpath = "/tns:Perception/ext:UBLExtensions/ext:UBLExtension[2]/ext:ExtensionContent";
                            break;
                        case "RA":
                            //Communicacion de baja
                            nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:VoidedDocuments-1");
                            local_xpath = "/tns:VoidedDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                            break;
                        case "RC":
                            //Resumen de diario
                            nsMgr.AddNamespace("tns", "urn:sunat:names:specification:ubl:peru:schema:xsd:SummaryDocuments-1");
                            local_xpath = "/tns:SummaryDocuments/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent";
                            break;
                    }


                    nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                    nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
                    nsMgr.AddNamespace("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                    nsMgr.AddNamespace("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
                    nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                    nsMgr.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

                    xmlDoc.SelectSingleNode(local_xpath, nsMgr).AppendChild(xmlDoc.ImportNode(signature, true));

                    /*  int cont = 0;
                      foreach (XmlNode node in xmlDoc.SelectNodes("//ext:ExtensionContent", namespaces))
                      {
                          if (cont == 1)
                          {
                              node.AppendChild(xmlDoc.ImportNode(signature, true));
                              break;
                          }
                          cont++;
                      }*/

                    xmlDoc.Save(local_xmlArchivo);

                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ds:Signature");

                    //el nodo <ds:Signature> debe existir unicamente 1 vez
                    if (nodeList.Count != 1)
                        throw new Exception("Se produjo un error en la firma del documento");

                    signedXml.LoadXml((XmlElement)nodeList[0]);

                    //verificacion de la firma generada
                    if (signedXml.CheckSignature() == false)
                        throw new Exception("Se produjo un error en la firma del documento");

                    if (documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA && documento.tipoDocumento != (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
                    {
                        var _refe = signedXml.SignedInfo.References.ToArray();
                        string sss = "";
                        foreach (Reference item in _refe)
                        {
                            sss = Convert.ToBase64String(item.DigestValue).ToString();
                            break;
                        }
                        documento.codigoBarraPDF417String = documento.rucEmisor + "|" + documento.tipoDocumento.ToString() + "|" + _serie + "|" + _correlativo + "|" + documento.igvTotal.ToString() + "|" + documento.ventaTotal.ToString() + "|" + documento.fechaEmision.ToShortDateString() + "|" + documento.tipoDocReceptor.ToString() + "|" + documento.rucReceptor.ToString() + "|" + sss + "|" + Convert.ToBase64String(signedXml.Signature.SignatureValue).ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA)
                {
                    if (File.Exists(pathBAJA + "\\" + filename + ".zip"))
                        throw new Exception("El archivo ZIP " + pathBAJA + "\\" + filename + ".zip ya existe");
                    ZipFile.CreateFromDirectory(pathXML, pathBAJA + "\\" + filename + ".zip");
                }
                else if (documento.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS)
                {
                    if (File.Exists(pathRESUMEN + "\\" + filename + ".zip"))
                        throw new Exception("El archivo ZIP " + pathRESUMEN + "\\" + filename + ".zip ya existe");
                    ZipFile.CreateFromDirectory(pathXML, pathRESUMEN + "\\" + filename + ".zip");
                }
                else
                {
                    if (File.Exists(pathZIP + "\\" + filename + ".zip"))
                        throw new Exception("El archivo ZIP " + pathZIP + "\\" + filename + ".zip ya existe");
                    ZipFile.CreateFromDirectory(pathXML, pathZIP + "\\" + filename + ".zip");

                }

                return filename;
                //creacion de ZIP
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            /*finally
            {
                //ELIMINA EL XML DEL TEMPORAL
                Directory.Delete(pathXML, true);
            }*/
            // Save the signed XML document to a file specified
            // using the passed string.
            /* XmlTextWriter xmltw = new XmlTextWriter(@"D:\\Resocentro\\SUNAT\\HomologacionSunat\\HomologacionSunat\\Controllers\\xml\\text-sing.xml", new UTF8Encoding(false));
             doc.WriteTo(xmltw);
             xmltw.Close();*/

        }

        public DocumentoSunat calcularCabecera(DocumentoSunat documento)
        {
            #region Calculo CABECERA XML


            //TOTAL GRAVADA
            documento.isOpeGravadas = documento.detalleItems.Where(x => x.tipoIGV < (int)TIPO_IGV.EXONERADO_ONEROSA).ToList().Count() > 0;
            if (documento.isOpeGravadas)
                documento.totalVenta_OpeGravadas = Math.Round(documento.detalleItems.Where(x => x.tipoIGV < (int)TIPO_IGV.EXONERADO_ONEROSA).Sum(x => x.valorVenta), 2);

            //TOTAL EXONERADAS
            documento.isOpeExoneradas = documento.detalleItems.Where(x => x.tipoIGV < (int)TIPO_IGV.INAFECTO_RETIROBONIFICACION && x.tipoIGV > (int)TIPO_IGV.GRAVADO_RETIROENTREGATRABAJADORES).ToList().Count() > 0;
            if (documento.isOpeExoneradas)
                documento.totalVenta_OpeExoneradas = Math.Round(documento.detalleItems.Where(x => x.tipoIGV < (int)TIPO_IGV.INAFECTO_RETIROBONIFICACION && x.tipoIGV > (int)TIPO_IGV.GRAVADO_RETIROENTREGATRABAJADORES).Sum(x => x.valorReferencialIGV), 2);

            //TOTAL INAFECTAS
            documento.isOpeInafectas = documento.detalleItems.Where(x => x.tipoIGV > (int)TIPO_IGV.EXONERADO_TRANSFERENCIAGRATUITA && x.tipoIGV < (int)TIPO_IGV.EXPORTACION).ToList().Count() > 0;
            if (documento.isOpeInafectas)
                documento.totalVenta_OpeInafectas = Math.Round(documento.detalleItems.Where(x => x.tipoIGV > (int)TIPO_IGV.EXONERADO_TRANSFERENCIAGRATUITA && x.tipoIGV < (int)TIPO_IGV.EXPORTACION).Sum(x => x.valorReferencialIGV), 2);


            //TOTAL GRATUITAS
            documento.isOpeGratuitas = documento.detalleItems.Where(x => x.isGratuita).ToList().Count > 0;
            if (documento.isOpeGratuitas)
                documento.totalVenta_OpeGratuitas = Math.Round(documento.detalleItems.Where(x => x.isGratuita).Sum(x => x.valorReferencialIGV), 2);


            //TOTAL DESCUENTO
            documento.isTotalDescuento = (documento.porcentajeDescuentoGlobal > 0 || documento.detalleItems.Where(x => x.porcentajedescxItem > 0 || x.porcentajeDescPromocion > 0 || x.porcentajeCobSeguro > 0).ToList().Count() > 0);
            if (documento.isTotalDescuento)
            {
                var _descuentoGlobal = 0.0;
                var _sumaDescuentos =
                    documento.detalleItems.Sum(x => x.descxItem) +
                    documento.detalleItems.Sum(x => x.descPromocion) +
                    documento.detalleItems.Where(x => x.tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE).Sum(x => x.descxCobSeguro) +
                    documento.detalleItems.Where(x => x.tipo_cobranza == (int)TIPO_COBRANZA.ASEGURADORA).Sum(x => x.descxCobPaciente);
                ;
                documento.totalDescuento = Math.Round((documento.descuentoGlobal + _sumaDescuentos), 2);

                _descuentoGlobal = (documento.porcentajeDescuentoGlobal / 100);
                //GRAVADA
                documento.desc_TotalVenta_OpeGravadas = Math.Round(documento.totalVenta_OpeGravadas * _descuentoGlobal, 2);
                //EXONERADA
                documento.desc_TotalVenta_OpeExoneradas = Math.Round(documento.totalVenta_OpeExoneradas * _descuentoGlobal, 2);
                //INAFECTAS
                documento.desc_TotalVenta_OpeInafectas = Math.Round(documento.totalVenta_OpeInafectas * _descuentoGlobal, 2);
            }
            return documento;
            #endregion
        }

        public string enletras(string num)
        {
            string res, dec = "";
            Int64 entero;
            int decimales;
            double nro;
            try
            {
                nro = Convert.ToDouble(num);
            }
            catch
            {
                return "";
            }

            entero = Convert.ToInt64(Math.Truncate(nro));
            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2).ToString());
            // if (decimales > 0)
            //{
            dec = " Y " + decimales.ToString() + "/100";
            //}
            res = toText(Convert.ToDouble(entero)) + dec;
            return res;
        }
        private string toText(double value)
        {

            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);
            }
            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);
            }
            else if (value == 1000000000000) Num2Text = "UN BILLON";
            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }


            return Num2Text;


        }

        public void insertarResumen(DocumentoSunat documento, string ticket, string pathResumen, string pathRespuesta, string session)
        {
            string sqlResumen = "INSERT INTO SendResumen([empresa],[numeroResumen],[fechaEmision],[fechaReferencia],[ticket],[pathResumen],[pathRespuesta],[usuario])VALUES ('" + documento.empresa + "','" + documento.numeroDocumento + "',getdate(),'" + documento.fechaEmision.ToShortDateString() + "','" + ticket + "','" + pathResumen + "','" + pathRespuesta + "','" + session + "');";


            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand();
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction("GuardarDocumento");
                    try
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;

                        command.CommandText = sqlResumen;
                        command.Prepare();
                        command.ExecuteNonQuery();


                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {

                        string mensaje = "ERROR SQL: INSERT RESUMEN :\n-" + ex.Message;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            mensaje += "\n\nError Rollback";
                            mensaje += "\n-Exxeption Type: " + ex2.GetType();
                            mensaje += "\n-Message: " + ex2.Message;
                        }

                        throw new Exception(mensaje);

                    }
                    finally
                    {
                        transaction.Dispose();
                        command.Dispose();
                        connection.Dispose();
                        connection.Close();
                    }

                }
            }
        }


        public List<PreResumenBoleta> UpdateListaResumen(string fecha, string unidad, string pathfile)
        {
            List<PreResumenBoleta> lista = new List<PreResumenBoleta>();
            if (fecha != null && unidad != null)
            {
                string queryString = "dbo.updateListaResumenBoletas";
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                    {

                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.Add("@fecha", SqlDbType.VarChar).Value = fecha;
                        command.Parameters.Add("@unidad", SqlDbType.VarChar).Value = unidad;
                        command.Parameters.Add("@path", SqlDbType.VarChar).Value = pathfile;
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        command.Prepare();

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            // Always call Close when done reading.
                            command.Dispose();
                            connection.Close();

                        }
                    }
                }
            }

            return lista;
        }
    }
}
