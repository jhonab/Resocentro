using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
         private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {

                //195011,195012,195013,195014,195043,195044,195045,195046,195047
                string[] doc = { "", "" };
                //var listaDoc = db.DOCUMENTO.Where(x => doc.Contains(x.numerodocumento)).ToList();
                var listaDoc = db.DOCUMENTO.Where(x => x.numerodocumento == txtnumerodocumento.Text).ToList();
                foreach (var d in listaDoc)
                {
                    var _aseguradora = db.ASEGURADORA.SingleOrDefault(x => x.ruc == d.ruc);
                    var _paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == d.codigopaciente);
                    DocumentoSunat item = new DocumentoSunat();
                    item.tipoDocReceptor = (int)TIPO_DOCUMENTOIDENTIDAD.RUC;
                    item.rucReceptor = d.ruc;
                    item.razonSocialReceptor = _aseguradora.razonsocial;
                    item.direccionReceptor = _aseguradora.domiciliofiscal;
                    item.emailReceptor = "";
                    item.fechaEmision = d.fechaemitio;
                    item.numeroDocumento = d.numerodocumento;
                    item.tipoDocumento = int.Parse(d.tipodocumento);
                    item.direccionsede = db.SUCURSAL.SingleOrDefault(x => x.codigounidad == d.codigounidad && x.codigosucursal == d.codigosucursal).direccionfactura;
                    item.isPrintSegOnline = true;
                    item.codigoSegWeb = "";
                    item.paciente = _paciente.apellidos + ", " + _paciente.nombres;
                    item.dnipaciente = _paciente.dni;
                    item.codigopaciente = _paciente.codigopaciente;
                    item.empresa = d.codigounidad.Value;
                    item.sede = d.codigosucursal.Value;
                    var _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == d.codigocarta && x.codigopaciente == d.codigopaciente);
                    if (_carta != null)
                    {
                        item.carta = _carta.codigocartagarantia;
                        item.infoCarta = _carta.codigocartagarantia2 + " (" + _carta.codigocartagarantia + ")" + " - Cob.: " + _carta.cobertura.ToString("#0.#0") + "%";
                        item.observaciones = "";
                        item.titular_Carta = _carta.titular;
                        item.contratante_carta = _carta.contratante;
                        item.poliza_carta = _carta.poliza;
                        item.cobertura_carta = _carta.cobertura;
                        item.numerocarnet_carta = _carta.numerocarnetseguro;
                        item.cartascorelacionadas = _carta.codigocartagarantia + ";";

                    }

                    item.textoinformacion =
                   (item.infoCarta == null ? "" : "CARTA: " + item.infoCarta + "\n")
                  + ("PACIENTE: " + item.paciente + "\n"
                  + (item.titular_Carta == "" ? "TITULAR: EL MISMO" : "TITULAR: " + item.titular_Carta + "\n")
                  + (item.contratante_carta == "" ? "CONTRATANTE: EL MISMO" : "CONTRATANTE: " + item.contratante_carta + "\n")
                  + ("POLIZA: " + item.poliza_carta + "\n")
                  + ("N° CARNET: " + item.numerocarnet_carta + "\n")
                  + "");

                    //DETALLE DOCUMENTO
                    item.TCCompra = d.tipocambio;
                    item.TCVenta = d.tipocambio;
                    item.igvPorcentaje = Convert.ToDouble(d.valorIGV.ToString());
                    item.detalleItems = new List<DetalleDocumentoSunat>();
                    foreach (var dd in db.DETALLEDOCUMENTO.Where(x => x.numerodocumento == d.numerodocumento && x.codigopaciente == d.codigopaciente).ToList())
                    {
                        DetalleDocumentoSunat det = new DetalleDocumentoSunat();
                        det.cantidad = dd.cantidad;
                        det.tipo_documento = item.tipoDocumento;
                        det.valorIgvActual = item.igvPorcentaje;
                        det.tipo_documento = item.tipoDocumento;
                        det.porcentajedescxItem = 0;//cortesia
                        det.tipo_cobranza = (int)TIPO_COBRANZA.ASEGURADORA;
                        item.codigoMoneda = int.Parse(d.tipomoneda);
                        det.simboloMoneda = item.codigoMoneda.ToString() == "1" ? "S/." : item.codigoMoneda.ToString() == "2" ? "$" : "";

                        if (d.codigocarta.ToString().Trim() == "")
                            det.isAsegurado = false;
                        else
                            det.isAsegurado = true;

                        det.codigoitem = dd.codigoestudio;
                        det.codigoclase = dd.codigoclase.ToString();
                        det.valorUnitarioigv = Math.Round((Convert.ToDouble(dd.preciounitario.ToString()) * (1 + det.tipoIGV)), 2);

                        det.valorReferencialIGV = item.igvPorcentaje;
                        det.descripcion = dd.descripcion;
                        det.tipoIGV = dd.tipoIGV;
                        if (det.isAsegurado && det.codigoclase != "0")
                        {
                            {
                                det.porcentajeCobSeguro = d.cobertura;
                                det.porcentajeCobPaciente = 100 - det.porcentajeCobSeguro;
                                det.montoMaximoAseguradora = Math.Round(((det.valorUnitario - det.descPromocion) * (det.porcentajeCobSeguro / 100)), 2);

                            }

                        }
                        item.detalleItems.Add(det);
                    }

                    item.detalleItems = new CobranzaDAO().calcularPromociones(item);
                    item.subTotal = d.subtotal;
                    item.igvTotal = d.igv;
                    item.descuentoGlobal = Math.Round((item.subTotal * (item.porcentajeDescuentoGlobal / 100)) + (item.igvTotal * (item.porcentajeDescuentoGlobal / 100)), 2);
                    item.ventaTotal = d.total;

                    item = new CobranzaDAO().calcularCabecera(item);

                    new CobranzaDAO().generarRepresentacionGraficaPRELIQUIDACION(item, @"\\sistemas\PRELIQUIDACION\PRELIQUIDACION-" + item.numeroDocumento + ".pdf");
                }
            }
        }
    }
}

