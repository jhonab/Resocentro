using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.frmUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using iTextSharp.text;
using OnBarcode.Barcode;
using System.Printing;
using System.Drawing.Printing;
using Resocentro_Desktop.Interfaz.Caja.impresion;
using Resocentro_Desktop.Interfaz.Caja;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
//using BarcodeLib.Barcode;

namespace Resocentro_Desktop.Interfaz.Cobranza
{
    /// <summary>
    /// Lógica de interacción para frmRegistrarCobranza.xaml
    /// </summary>
    public partial class frmRegistrarCobranza : Window
    {
        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://extranet.resocentro.com:5052/";
        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }
        public bool Active { get; set; }


        MySession session;
        DocumentoSunat item;
        TIPO_MONEDA tipomoneda = TIPO_MONEDA.SOL;
        TIPO_COBRANZA tipocobranza;
        int unidad, sede, edadpaciente = 0;
        private Tool tool = new Tool();
        //int _codigocompaniaOriginal = 0;
        string _pathPDF = "";
        string _docCarta = "";
        string _pathPDFLIQUIDACION = "";
        string filename = "";
        public frmRegistrarCobranza()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session, int unidad, int sede, TIPO_COBRANZA tipocobranza)
        {
            this.session = session;
            this.unidad = unidad;
            this.sede = sede;
            this.tipocobranza = tipocobranza;
            rnDescuento.Value = 0;


            if (tipocobranza == TIPO_COBRANZA.PACIENTE)
            {
                gvccobertura.IsVisible = true;
                gvcporcobertura.IsVisible = true;

                gvcdeducible.IsVisible = false;
                gvcpordeducible.IsVisible = false;

                cboTipoDocumento.SelectedIndex = 1;
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    cbotipomoneda.ItemsSource = db.MONEDA.Where(x => x.IsEnabledMoneda == true).ToList();
                    cbotipomoneda.SelectedValuePath = "codigomoneda";
                    cbotipomoneda.DisplayMemberPath = "descripcion";
                    cbotipomoneda.SelectedValue = "1";

                    cboTipoPago.ItemsSource = db.TARJETA.Where(x => x.IsEnabledTarjeta == true).ToList().OrderBy(x => x.descripcion);
                    cboTipoPago.SelectedValuePath = "codigotarjeta";
                    cboTipoPago.DisplayMemberPath = "descripcion";
                    cboTipoPago.SelectedValue = "10";
                }
                btnVerInforme.Visibility = Visibility.Collapsed;
                btnVerBoleta.Visibility = Visibility.Collapsed;
                btnAddAtencion.Visibility = Visibility.Collapsed;
            }
            else
            {
                gvccobertura.IsVisible = false;
                gvcporcobertura.IsVisible = false;

                gvcdeducible.IsVisible = true;
                gvcpordeducible.IsVisible = true;

                cboTipoDocumento.SelectedIndex = 0;
                gridFormaPago.Visibility = Visibility.Collapsed;
                btnprocesar.Visibility = Visibility.Collapsed;
                btnCobranza_Externa.Visibility = Visibility.Collapsed;
            }
        }

        public async void abrirInfoFacturacion()
        {
            if (tipocobranza == TIPO_COBRANZA.PACIENTE)
            {
                if (item.cartascorelacionadas != null)
                {
                    foreach (var carta in item.cartascorelacionadas.Split(';'))
                    {
                        if (carta != null || carta != "")
                        {
                            var _cart = new CartaDAO().getCartaxCodigo(carta, item.codigopaciente);
                            var detalle = new CartaDAO().getDetalleCartaxCodigo(carta, item.codigopaciente);
                            if (_cart != null)
                            {
                                frmCarta.frmCarta gui = new frmCarta.frmCarta();
                                gui.cargarGUI(session, true);
                                gui.Show();
                                new CartaDAO().insertLog(carta.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + carta.ToString());
                                gui.setCartaGarantia(_cart, detalle, false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (item.carta != "")
                {
                    var _cart = new CartaDAO().getCartaxCodigo(item.carta, item.codigopaciente);
                    var detalle = new CartaDAO().getDetalleCartaxCodigo(item.carta, item.codigopaciente);
                    if (_cart != null)
                    {
                        frmCarta.frmCarta gui = new frmCarta.frmCarta();
                        gui.cargarGUI(session, true);
                        gui.Show();
                        new CartaDAO().insertLog(item.carta.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + item.carta.ToString());
                        gui.setCartaGarantia(_cart, detalle, false);
                    }
                }
            }
        }

        public void cargarCobranza(int atencion, string carta_coaseguro = "", bool isCobranzaExterna = false)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                item = new DocumentoSunat();
                item.atenciones = new List<int>();
                item.atenciones.Add(atencion);
                var _atencion = db.ATENCION.SingleOrDefault(x => x.numeroatencion == atencion);

                if (_atencion != null)
                {
                    var _paciente = _atencion.PACIENTE;
                    edadpaciente = UtilDAO.calcularEdad(_paciente.fechanace);

                    if (tipocobranza == TIPO_COBRANZA.PACIENTE)
                    {
                        if (edadpaciente < 18)
                            MessageBox.Show("El Paciente es menor de Edad.\nFecha de Nacimiento:" + _paciente.fechanace.ToShortDateString() + "\n\n Debe cambiar de paciente para poder emitir un Documento Valido mayor a 700 soles.");

                        var tipodoc = db.TIPO_DOCUMENTO_IDENTIDAD.SingleOrDefault(x => x.tipo_doc_id == _paciente.tipo_doc);
                        item.tipoDocReceptor = int.Parse(tipodoc.tipo_doc_identificador);

                        item.rucReceptor = _paciente.dni;
                        item.razonSocialReceptor = _paciente.apellidos + " " + _paciente.nombres;
                        item.direccionReceptor = _paciente.direccion;
                        item.emailReceptor = _paciente.email;
                        item.iscobranzaExterna = isCobranzaExterna;
                    }
                    else if (tipocobranza == TIPO_COBRANZA.ASEGURADORA)
                    {
                        try
                        {
                            var _aseguradora = db.ASEGURADORA.SingleOrDefault(x => x.ruc == _atencion.ruc);
                            var _companiaseguro = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == _atencion.codigocompaniaseguro);
                            if (_aseguradora != null)
                            {

                                item.tipoDocReceptor = (int)TIPO_DOCUMENTOIDENTIDAD.RUC;
                                item.rucReceptor = _aseguradora.ruc;
                                item.razonSocialReceptor = _aseguradora.razonsocial;
                                item.direccionReceptor = _aseguradora.domiciliofiscal;
                                item.emailReceptor = _companiaseguro.correo_facturacion;
                            }
                            else
                                MessageBox.Show("ERROR", "No se encontro una Aseguradora con el RUC registrado");

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("ERROR", ex.Message);
                        }

                    }
                    else { }
                    item.isPrintSegOnline = _atencion.isImpresoSegOnline;
                    item.codigoSegWeb = _atencion.CodigoSegOnline;
                    item.paciente = _paciente.apellidos + ", " + _paciente.nombres;
                    item.dnipaciente = _paciente.dni;
                    item.codigopaciente = _atencion.codigopaciente;
                    item.numeroatencion = _atencion.numeroatencion;
                    item.codigocompaniaseguro = _atencion.codigocompaniaseguro;
                    item.aseguradora = _atencion.CITA.COMPANIASEGURO.descripcion;
                    item.aseguradora_ruc = _atencion.ruc;
                    item.numerocita = _atencion.numerocita;
                    item.cmp = _atencion.cmp;
                    item.codigomodalidad = _atencion.codigomodalidad;
                    item.empresa = unidad;
                    item.sede = sede;


                    //CITA
                    var _cita = db.CITA.SingleOrDefault(x => x.numerocita == _atencion.numerocita);
                    CARTAGARANTIA _carta;
                    if (carta_coaseguro == "")//si no tiene carta por el deducible
                    {
                        _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == _cita.codigocartagarantia && x.codigopaciente == _cita.codigopaciente);
                    }
                    else
                    {
                        _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == carta_coaseguro && x.codigopaciente == _cita.codigopaciente);
                        item.aseguradora = _carta.COMPANIASEGURO.descripcion;
                        //item.carta_Coaseguro = carta_coaseguro;
                        item.hasCoaseguro = true;
                    }

                    item.observaciones = "CITA\t\t\t\n" + _cita.observacion;
                    if (_carta != null)
                    {
                        item.carta = _carta.codigocartagarantia;
                        item.infoCarta = _carta.codigocartagarantia2 + " (" + _carta.codigocartagarantia + ")" + " - Cob.: " + _carta.cobertura.ToString("#0.#0") + "%";
                        item.observaciones = item.observaciones.Trim();
                        if (item.observaciones != "")
                            item.observaciones += "\n\n";
                        item.observaciones += "CARTA\t\t\t\n" + _carta.seguimiento;
                        item.titular_Carta = _carta.titular;
                        item.contratante_carta = _carta.contratante;
                        item.poliza_carta = _carta.poliza;
                        item.cobertura_carta = _carta.cobertura;
                        item.numerocarnet_carta = _carta.numerocarnetseguro;
                        item.cartascorelacionadas = _cita.codigocartagarantia + ";";
                        _docCarta = _carta.codigodocadjunto;

                    }
                    else
                    {
                        btnCarta.Visibility = Visibility.Collapsed;
                    }

                    cargarEstudios();
                    refreshData();


                }
            }

        }


        private void cargarEstudios()
        {
            try
            {
                var _hascoaseguro = item.hasCoaseguro;
                item.detalleItems = new List<DetalleDocumentoSunat>();
                foreach (var ate in item.atenciones)
                {
                    var _doc = new DocumentoSunat();
                    _doc.empresa = item.empresa;
                    _doc.iscobranzaExterna = item.iscobranzaExterna;
                    _doc.carta = item.carta;
                    _doc.sede = item.sede;
                    var _item = new CobranzaDAO().getEstudios(ate, _doc, tipocobranza, tipomoneda);

                    item.TCCompra = _doc.TCCompra;
                    item.TCVenta = _doc.TCVenta;
                    item.igvPorcentaje = _doc.igvPorcentaje;
                    item.codigoMoneda = _doc.codigoMoneda;
                    item.detalleItems.AddRange(_item.detalleItems);
                }
                gridDetalle.ItemsSource = null;
                gridDetalle.ItemsSource = item.detalleItems;



                if (item.hasCoaseguro && _hascoaseguro)
                    item.hasCoaseguro = false;
                //item.detalleItems = _item.detalleItems;
                setDetalleDocumento();
                listarformaPago();


                if (tipocobranza == TIPO_COBRANZA.ASEGURADORA && item.hasCoaseguro)
                {
                    frmRegistrarCobranza gui = new frmRegistrarCobranza();
                    gui.cargarGUI(session, item.empresa, item.sede, TIPO_COBRANZA.ASEGURADORA);
                    gui.cargarCobranza(item.numeroatencion, item.carta_Coaseguro);
                    gui.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void refreshData()
        {
            lbltipoDocumento.Content = item.documentoReceptorString;
            lblnumDocumentoReceptor.Text = item.rucReceptor;
            lblRazReceptor.Text = item.razonSocialReceptor;
            lbldireccionReceptor.Text = item.direccionReceptor;
            txtcorreoReceptor.Text = item.emailReceptor;
            txtcomentarios.Text = item.observaciones;
            setDetalleDocumento();
            lblTCCompra.Content = "TC Compra: " + (Math.Round(item.TCCompra, tool.cantidad_decimales, MidpointRounding.AwayFromZero)).ToString();
            lblTCVenta.Content = "TC Venta: " + (Math.Round(item.TCVenta, tool.cantidad_decimales, MidpointRounding.AwayFromZero)).ToString();
            string _atencion = "";
            foreach (var _ate in item.atenciones)
            {
                _atencion += _ate + "  ";
            }
            if (item.iscobranzaExterna)
                item.textoinformacion = ("PACIENTE: " + item.paciente + "\n"
                    + "N° ATENCIÓN: " + _atencion + "\n");
            else
                item.textoinformacion =
                     (item.infoCarta == null ? "" : "CARTA: " + item.infoCarta + "\n")
                    + ("N° ATENCIÓN: " + _atencion.ToString() + "\n"
                     + "PACIENTE: " + item.paciente + "\n"
                    + (item.titular_Carta == "" ? "TITULAR: EL MISMO" : "TITULAR: " + item.titular_Carta + "\n")
                    + (item.contratante_carta == "" ? "CONTRATANTE: EL MISMO" : "CONTRATANTE: " + item.contratante_carta + "\n")
                    + ("POLIZA: " + item.poliza_carta + "\n")
                    + ("N° CARNET: " + item.numerocarnet_carta + "\n")
                    + "CIA: " + item.aseguradora.ToUpper());

            txtinformacion.Text = item.textoinformacion;
        }


        private void setDetalleDocumento()
        {
            item.detalleItems = new CobranzaDAO().calcularPromociones(item);
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = item.detalleItems;

            calcularcabecera();


        }

        private void calcularcabecera()
        {
            item.subTotal = Math.Round(item.detalleItems.Sum(x => x.valorVenta), 2, MidpointRounding.AwayFromZero);
            /* double _total_igv = 0;
             foreach (var igv in item.detalleItems)
             {
                 _total_igv += igv.igvItem_old;
             }*/
            item.igvTotal = Math.Round(item.detalleItems.Sum(x => x.igvItem_old), 2, MidpointRounding.AwayFromZero);

            //item.igvTotal = _total_igv;
            item.descuentoGlobal = Math.Round((item.subTotal * (item.porcentajeDescuentoGlobal / 100)) + (item.igvTotal * (item.porcentajeDescuentoGlobal / 100)), 2, MidpointRounding.AwayFromZero);
            item.ventaTotal = Math.Round((item.subTotal + item.igvTotal) - item.descuentoGlobal, 2, MidpointRounding.AwayFromZero);


            txtsubtotal.Text = item.subTotal.ToString("#,###,###0.#0");
            txtigv.Text = item.igvTotal.ToString("#,###,###0.#0");
            txtdesc.Text = item.descuentoGlobal.ToString("#,###,###0.#0");
            txtdesc.Text = item.descuentoGlobal.ToString("#,###,###0.#0");
            if (tipocobranza == TIPO_COBRANZA.PACIENTE)
            {
                //txtredondeo.Text = (item.ventaTotal - Math.Round(item.ventaTotal, 1)).ToString("#,###,###0.#0");
                //txttotal.Text = (Math.Round(item.ventaTotal, 1)).ToString("#,###,###0.#0");

                txttotal.Text = (item.ventaTotal).ToString("#,###,###0.#0");
            }
            else
                txttotal.Text = (item.ventaTotal).ToString("#,###,###0.#0");
        }
        private void rnDescuento_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (item != null)
            {
                item.porcentajeDescuentoGlobal = e.NewValue.Value;
                setDetalleDocumento();
            }


        }
        private void gridDetalle_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            DetalleDocumentoSunat det = (DetalleDocumentoSunat)e.EditedItem;

            var _item = item.detalleItems.SingleOrDefault(x => x.codigoitem == det.codigoitem);
            if (_item != null)
            {
                if (det.isGratuita)
                    det.tipoIGV = (int)TIPO_IGV.INAFECTO_RETIRO;
                else
                {
                    if (det.codigoitem.Substring(7, 2) == "99")
                    {
                        if (det.valorUnitario > 0)
                            det.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                        else
                            det.tipoIGV = (int)TIPO_IGV.NO_DECLARAR_SUNAT;
                    }
                    else
                        det.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                }
                _item = det;
                setDetalleDocumento();
            }
        }

        private void btnguardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarDatos())
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var _cobranzaDao = new CobranzaDAO();

                        string tipodocumento = cboTipoDocumento.Text.Substring(0, 2);
                        var _numdoc = _cobranzaDao.getNumeroDocumento(tipodocumento, item.empresa.ToString());
                        if (_numdoc == "")
                        {
                            MessageBox.Show("No se asigno un número de documento para este registro", "ERROR!!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (MessageBox.Show("Está seguro de seguir con el registro del Documento N°:" + _numdoc, "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {

                            item.numeroDocumento = _numdoc;
                            item.numeroDocumentoSUNAT = item.numeroDocumento;
                            //item.numeroDocumento = "B04-123";
                            item.fechaEmision = Tool.getDatetime();
                            item.tipoDocumento = int.Parse(tipodocumento);
                            item.idFirmaDigital = "JHONAB";
                            item.isSendSUNAT = false;
                            item.direccionsede = _cobranzaDao.getDireccionSede(item.empresa.ToString(), item.sede.ToString());
                            foreach (var dd in item.detalleItems)
                            {
                                dd.tipo_documento = item.tipoDocumento;
                            }
                            item = _cobranzaDao.calcularCabecera(item);


                            if (new CobranzaDAO().VerificarNumeroDocumento(item.tipoDocumento.ToString().PadLeft(2, '0'), item.numeroDocumento, item.empresa.ToString()))
                            {

                                lblnumerodocumento.Content = item.numeroDocumento;
                                try
                                {
                                    filename = _cobranzaDao.crearDocumentoXML(item);

                                    if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)//FACTURAS
                                    {

                                        if (filename != "")
                                        {
                                            try
                                            {
                                                //string pathCDR = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
                                                if (new CobranzaDAO().isSunatServiceActive())
                                                {
                                                    string pathCDR = new ServiceSunat().sendBill(item.empresa, item.codigopaciente, filename);

                                                    string msjResutlado = "";
                                                    if (_cobranzaDao.verificarCDR(pathCDR, out msjResutlado))
                                                    {
                                                        item.isSendSUNAT = true;
                                                        //adelantamos el correlativo                                              
                                                        procesarDocumentoElectronico();
                                                    }
                                                    else
                                                        MessageBox.Show(msjResutlado, "ERROR AL ENVIAR DOCUMENTO");
                                                }
                                                else
                                                {
                                                    item.isSendSUNAT = false;
                                                    procesarDocumentoElectronico();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                btnguardar.Visibility = Visibility.Collapsed;
                                                btnReenviar.Visibility = Visibility.Visible;
                                                btnVerificarCDR.Visibility = Visibility.Visible;
                                                MessageBox.Show(ex.Message);
                                            }

                                        }
                                    }
                                    else //BOLETAS
                                    {
                                        procesarDocumentoElectronico();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }


                            }
                            else
                            {
                                MessageBox.Show("El numero de Documento ya está Registrado", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                try
                {
                    new CobranzaDAO().eliminarArchivos(item);
                }
                catch (Exception ex1)
                {
                    mensaje += "\n\n ELIMINACION ARCHIVOS:" + ex1.Message;
                }
                MessageBox.Show(mensaje);
            }

        }

        private void procesarDocumentoElectronico()
        {
            #region GENERAR IMPRESION GRAFICA
            string filename = item.rucEmisor + "-" + item.tipoDocumento.ToString("D2") + "-" + item.numeroDocumentoSUNAT;
            string pathCODEBAR = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\PDF417\" + filename + ".jpeg";
            _pathPDF = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\PDF\" + filename + ".pdf";
            _pathPDFLIQUIDACION = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\PDF\LIQUI-" + filename + ".pdf";
            /*PDF417 _pdf417 = new PDF417();
            _pdf417.Data = item.codigoBarraPDF417String.Trim();
            _pdf417.RowCount = 20;
            _pdf417.ColumnCount = 20;
            _pdf417.ECL = OnBarcode.Barcode.PDF417ECL.Level_0;

            //_pdf417.Compact = false;
            _pdf417.DataMode = PDF417DataMode.Text;
            _pdf417.UOM = UnitOfMeasure.CM;
            //_pdf417.BackColor = System.Drawing.Color.White;
            //_pdf417.ForeColor = System.Drawing.Color.Black;
            _pdf417.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            _pdf417.Resolution = 72;
            //_pdf417.X = 2;
            _pdf417.LeftMargin = 0;
            _pdf417.RightMargin = 0;
            _pdf417.TopMargin = 0;
            _pdf417.BottomMargin = 0;
            _pdf417.Rotate = Rotate.Rotate0;
            //_pdf417.ResizeImage = true;
            //_pdf417.Resolution = 100;

            item.pathCODEBAR = pathCODEBAR;
            _pdf417.drawBarcode(item.pathCODEBAR);*/

            item.pathCODEBAR = pathCODEBAR;
            QrEncoder qrencoder = new QrEncoder(ErrorCorrectionLevel.Q);
            QrCode qrcode = new QrCode();
            qrencoder.TryEncode(item.codigoBarraPDF417String.Trim(), out qrcode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero));
            using (MemoryStream ms = new MemoryStream())
            {
                renderer.WriteToStream(qrcode.Matrix, ImageFormat.Jpeg, ms);
                var imagetemporal = new Bitmap(ms);
                var imagen = new Bitmap(imagetemporal, new System.Drawing.Size(new System.Drawing.Point(200, 200)));
                imagen.Save(item.pathCODEBAR, ImageFormat.Jpeg);
            }

            new CobranzaDAO().generarRepresentacionGrafica(item, _pathPDF, pathCODEBAR);
            //new CobranzaDAO().generarRepresentacionGraficaPRELIQUIDACION(item, _pathPDFLIQUIDACION);
            item.pathPDF = _pathPDF;
            //Using below code we can print any document
            #endregion
            guardarDocumentoBaseDatos(filename);
        }


        private bool validarDatos()
        {
            bool result = true;

            int _tipodocumento = int.Parse(cboTipoDocumento.Text.Substring(0, 2));
            string _rucReceptor = lblnumDocumentoReceptor.Text.ToString();
            int _rucReceptorLength = _rucReceptor.Length;

            //FACTURA
            if (_tipodocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
            {
                if (_rucReceptorLength != 11)
                    result = false;

                if (!CobranzaDAO.ValidarRUC(_rucReceptor))
                    result = false;
            }
            //BOLETA
            if (_tipodocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                if (item.tipoDocReceptor == (int)TIPO_DOCUMENTOIDENTIDAD.DNI && _rucReceptorLength != 8)
                    result = false;
                if (tipocobranza == TIPO_COBRANZA.PACIENTE)
                    if (edadpaciente < 18 && item.ventaTotal > 700)
                        result = false;
            }

            if (!result)
                MessageBox.Show("Verificar el Tipo y Número de Documento del Cliente, la edad del Paciente", "ADVERTENCIA", MessageBoxButton.OK);

            if (item.igvPorcentaje == 0)
            {
                result = false;
                MessageBox.Show("Verifique el valor del IGV", "ADVERTENCIA", MessageBoxButton.OK);
            }




            /*
            if (item.ventaTotal == 0)
            {
                result = false;
                if (MessageBox.Show("El estudio tiene un monto a cero. \n ¿Desea procesarlo al 100%?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    new CobranzaDAO().cambiarestadoPagado(item);
                }
            }*/
            return result;
        }

        private async void printComprobante()
        {
            try
            {
                #region IMPRESION DOCUMENTOS ELECTRONICOS (SUNAT)
                if (tipocobranza == TIPO_COBRANZA.PACIENTE)
                {
                    frmTicketComprobante tkt = new frmTicketComprobante();
                    tkt.cargarGUI(session, item);
                    tkt.printTicket();
                    frmTicketControl tkt1 = new frmTicketControl();
                    tkt1.cargarGUI(session, item);
                    tkt1.printTicket();

                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var atencion = db.ATENCION.SingleOrDefault(x => x.numeroatencion == item.numeroatencion);
                        if (atencion != null)
                        {
                            var ticket = db.TKT_Ticketera.SingleOrDefault(x => x.id_Ticket == atencion.numeroTicket);
                            if (ticket != null)
                            {
                                ticket.fec_caja = Tool.getDatetime();
                                ticket.usu_caja = session.codigousuario;
                                ticket.estado = 5;
                                db.SaveChanges();
                                await SendMessage();
                            }
                        }
                    }

                }
                else
                {
                    /*
                        System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                        printJob.StartInfo.FileName = _pathPDFLIQUIDACION;
                        printJob.StartInfo.UseShellExecute = true;
                        printJob.StartInfo.Verb = "printto";
                        printJob.StartInfo.CreateNoWindow = true;
                        printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        printJob.StartInfo.Arguments = "LIQUIDACION";
                        printJob.Start();
                    */
                    System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                    printJob = new System.Diagnostics.Process();
                    printJob.StartInfo.FileName = _pathPDF;
                    printJob.StartInfo.UseShellExecute = true;
                    printJob.StartInfo.Verb = "printto";
                    printJob.StartInfo.CreateNoWindow = true;
                    printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    printJob.StartInfo.Arguments = "FACTURA";
                    printJob.Start();
                    printJob.Start();
                    /*else
           {
               System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(_pathPDF);
               info.Verb = "Print";
               info.CreateNoWindow = true;
               info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
               System.Diagnostics.Process.Start(info);
           }*/
                #endregion
                }
                printSegOnline();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }




            #region IMPRESION DE DOCUMENTOS
            /*  if (tipocobranza == TIPO_COBRANZA.ASEGURADORA && item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                frmBoletaResocentro tkt = new frmBoletaResocentro();
                tkt.cargarGUI(session, item, txtinformacion.Text);
                tkt.printTicket();
            }
            else
            {
                frmResocentro tkt = new frmResocentro();
                tkt.cargarGUI(session, item, txtinformacion.Text);
                tkt.printTicket();
            }*/
            #endregion
        }

        private void printSegOnline()
        {
            try
            {

                if (tipocobranza == TIPO_COBRANZA.PACIENTE && item.empresa == 1)
                {
                    if (!item.isPrintSegOnline)
                    {
                        var nombre = item.paciente.Split(',')[1].ToString();
                        frmResultadosOnline tkt = new frmResultadosOnline();
                        tkt.cargarGUI(nombre.ToUpper().Trim(), item.numeroatencion.ToString(), item.codigoSegWeb.ToUpper().Trim());
                        tkt.printTicket();
                        try
                        {
                            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                            {
                                var ate = db.ATENCION.SingleOrDefault(x => x.numeroatencion == item.numeroatencion);
                                ate.isImpresoSegOnline = true;
                                db.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void guardarDocumentoBaseDatos(string filename)
        {
            try
            {
                if (new CobranzaDAO().insertarDocumento(item, session))
                {
                    btnguardar.IsEnabled = false;
                    gridDetalle.IsEnabled = false;
                    txtmonto.Value = item.ventaTotal;
                    new CobranzaDAO().updateSerieCorrelativo(item.tipoDocumento.ToString().PadLeft(2, '0'), item.empresa.ToString());
                    new CobranzaDAO().cambiarestadoPagado(item);
                    printComprobante();
                    if (item.emailReceptor != "")
                    {
                        try
                        {
                            CobranzaDAO.sendCorreoDocumentoGenerado(item, filename);
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }

                    }
                    MessageBox.Show("Documento Generado");
                    new CobranzaDAO().cambiarestadoOcultos(item.numerocita.ToString());
                    if (tipocobranza == TIPO_COBRANZA.ASEGURADORA)
                        this.Close();
                }
                else
                    MessageBox.Show("No se guardo el Documento", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void txtentregado_TextChanged(object sender, TextChangedEventArgs e)
        {
            double monto_entregado = 0;
            if (double.TryParse(txtentregado.Text, out monto_entregado))
            {
                txtvuelto.Text = Math.Round((monto_entregado - item.ventaTotal), 2, MidpointRounding.AwayFromZero).ToString("#,###,###0.#0");

            }
            else
            {
                txtvuelto.Text = "";
            }
        }

        private void RadRadioButton_Click(object sender, RoutedEventArgs e)
        {
            tipomoneda = TIPO_MONEDA.DOLAR;
            cargarEstudios();
        }

        private void RadRadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            tipomoneda = TIPO_MONEDA.SOL;
            cargarEstudios();
        }

        private void btnReceptorEmpresa_Click(object sender, RoutedEventArgs e)
        {
            string tipodocumento = cboTipoDocumento.Text.Substring(0, 2);
            if (tipodocumento == "03")//boleta
            {
                frmSearchPaciente gui = new frmSearchPaciente();
                gui.ShowDialog();
                if (gui.paciente != null)
                {
                    edadpaciente = UtilDAO.calcularEdad(gui.paciente.fechanace);
                    if (edadpaciente > 17)
                    {
                        using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                        {
                            var _tipodoc = db.TIPO_DOCUMENTO_IDENTIDAD.SingleOrDefault(x => x.tipo_doc_id == gui.paciente.tipo_doc);
                            item.tipoDocReceptor = int.Parse(_tipodoc.tipo_doc_identificador);
                            item.rucReceptor = gui.paciente.dni;
                            item.razonSocialReceptor = gui.paciente.apellidos + " " + gui.paciente.nombres;
                            if (item.codigocompaniaseguro != 3)//BANCO CENTRAL DE RESERVA
                                item.direccionReceptor = gui.paciente.direccion;
                            if (item.carta != "")//SI NO ES ASEGURADO NO TIENE CARTA
                            {
                                item.detalleItems = new CobranzaDAO().calularPrecio(item, item.codigocompaniaseguro);
                                setDetalleDocumento();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("El Paciente debe ser mayor de Edad.\n - Fecha de Nacimiento:" + gui.paciente.fechanace.ToShortDateString() + ".\n\n Seleccione a otro paciente.");
                    }
                }
            }
            else
            {
                frmSearchCompania gui = new frmSearchCompania();
                gui.cargarGUI(session);
                gui.ShowDialog();
                if (gui.DialogResult.Value)
                {
                    item.tipoDocReceptor = (int)TIPO_DOCUMENTOIDENTIDAD.RUC;
                    item.rucReceptor = gui.empresa.ruc;
                    item.razonSocialReceptor = gui.empresa.razonsocial;
                    item.direccionReceptor = gui.empresa.direccion;
                    item.emailReceptor = gui.empresa.correo;
                    if (item.carta != "")
                    {
                        item.detalleItems = new CobranzaDAO().calularPrecio(item, gui.empresa.codigocompania == -1 ? item.codigocompaniaseguro : gui.empresa.codigocompania);
                        setDetalleDocumento();
                    }
                }
            }

            refreshData();

        }

        private void btnAgregarFormaPago_Click(object sender, RoutedEventArgs e)
        {
            agregarFormaPago();
        }

        private void agregarFormaPago()
        {
            if (cboTipoPago.SelectedValue.ToString() != "10" && txtnumeroReferencia.Text == "")
            {
                MessageBox.Show("Verifique los datos.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {


                try
                {
                    FORMADEPAGO forma = new FORMADEPAGO();
                    forma.monto = Convert.ToSingle(txtmonto.Value.Value);
                    forma.codigomodalidadpago = 1;
                    forma.codigomoneda = int.Parse(cbotipomoneda.SelectedValue.ToString());
                    forma.numerodocumento = item.numeroDocumento;
                    forma.ruc = item.aseguradora_ruc;
                    forma.codigopaciente = item.codigopaciente;
                    forma.codigousuario = session.codigousuario;
                    forma.codigotarjeta = int.Parse(cboTipoPago.SelectedValue.ToString());
                    forma.fechadepago = Tool.getDatetime();
                    forma.codigounidad = unidad;
                    forma.codigosucursal = sede;
                    /* if (forma.codigoformapago == 11)//Recibo provisional
                         forma.numeroReferencia = "R-"+txtnumeroReferencia.Text;
                     else*/
                    forma.numeroReferencia = txtnumeroReferencia.Text;
                    /* if (forma.codigoformapago == 11)
                     {
                         using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                         {
                             int numrecibo=int.Parse(txtnumeroReferencia.Text);
                             var recibo = db.ReciboProvisional.SingleOrDefault(x=>x.idRecibo==numrecibo && x.isActivo);
                             if (recibo == null)
                             {
                                 MessageBox.Show("No existe el Recibo Provisional ingresado");
                                 return;
                             }
                             if(Convert.ToDouble(recibo.monto_recibido)>txtmonto.Value.Value)
                             {
                                 MessageBox.Show("El monto ingresado es mayor al Monto del Recibo Provisional");
                                 return;
                             }
                             else
                             {
                                 recibo.isActivo = false;
                                 db.SaveChanges();
                             }

                         }
                     }*/
                    new CobranzaDAO().registrarFormaPago(forma);
                    listarformaPago();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void limpiarDatosFormaPago()
        {
            txtmonto.Value = 0;
            cbotipomoneda.SelectedIndex = 0;
            cboTipoPago.SelectedValue = "10";
            txtnumeroReferencia.Text = "";
        }

        private void listarformaPago()
        {
            if (tipocobranza == TIPO_COBRANZA.PACIENTE)
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    grid_formaPago.ItemsSource = null;
                    grid_formaPago.ItemsSource = new DocumentoDAO().getFormaPagoDocumento(item.numeroDocumento, item.codigopaciente, item.empresa.ToString(), item.sede.ToString());
                    limpiarDatosFormaPago();
                }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            printComprobante();
        }

        private void BtnAgregar_Estudio_Click(object sender, RoutedEventArgs e)
        {
            frmSeleccionarModalidad guimodalidad = new frmSeleccionarModalidad();
            guimodalidad.Owner = this;
            guimodalidad.cargarGUI(unidad);

            if (guimodalidad.ShowDialog().Value)
            {
                frmSearchEstudio gui = new frmSearchEstudio();
                gui.ampliacion = "0";
                gui.Owner = this;
                gui.modalidad = guimodalidad.modalidad;
                gui.sucursal = ((unidad * 100) + sede).ToString();
                gui.iscaja = true;
                gui.getClase();
                gui.ShowDialog();
                if (gui.estudio != null)
                {
                    try
                    {
                        using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                        {
                            db.InsertEstudioCaja(item.numerocita, gui.estudio.codigoestudio);
                            cargarEstudios();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
            }
        }

        private void RadContextMenuDetalle_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat item = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (item == null)
                RadContextMenuDetalle.Visibility = Visibility.Collapsed;
            else
            {
                RadContextMenuDetalle.Visibility = Visibility.Visible;

                if (item.isGratuita)
                    itemCortesia.Header = "Desactivar Cortesia";
                else
                    itemCortesia.Header = "Activar Cortesia";

                if (item.isCortesia)
                    itemDescuento.Header = "Desactivar Descuento";
                else
                    itemDescuento.Header = "Activar Descuento";

                if (item.porcentajeDescPromocion > 0)
                    itemPromocion.Visibility = Visibility.Visible;
                else
                    itemPromocion.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItemEliminarItem_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat detalle = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (detalle != null)
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    if (MessageBox.Show("¿Desea ocultar el estudio?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        /* var _det = item.detalleItems.Where(x => x.codigoitem == detalle.codigoitem).SingleOrDefault();
                         if (_det != null)
                         {*/
                        detalle.cantidad = detalle.cantidad - 1;
                        if (detalle.cantidad == 0)
                            item.detalleItems.Remove(detalle);
                        gridDetalle.ItemsSource = null;
                        gridDetalle.ItemsSource = item.detalleItems;
                        calcularcabecera();
                        /* }*/
                    }

                }

        }

        private void btncancrlar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void regresarEstado()
        {
            if (item.iscobranzaExterna)
            {
                foreach (var ate in item.atenciones)
                {
                    new CobranzaDAO().cambiarestadoOcultos(item.numerocita.ToString());
                }

            }
        }

        private void BtnVerCarta_Click(object sender, RoutedEventArgs e)
        {
            abrirInfoFacturacion();
        }

        private void grid_formaPago_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            if (MessageBox.Show("¿Desea eliminar la Forma de Pago?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    try
                    {
                        FormaPagoEntity item = (FormaPagoEntity)e.Row.DataContext;

                        db.FORMADEPAGO.Remove(db.FORMADEPAGO.SingleOrDefault(x => x.codigoformapago == item.ID));
                        db.SaveChanges();
                        listarformaPago();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void BtnCobranzaExterna_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                if (MessageBox.Show("¿Desea realizar una cobranza con esta atencion?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        item.cobertura_carta = 0;
                        item.numeroDocumento = "";
                        item.iscobranzaExterna = true;
                        listarformaPago();
                        refreshData();
                        new CobranzaDAO().ocultarEstudios(item.numerocita.ToString(), item.codigopaciente.ToString());
                        cargarEstudios();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

            }
        }

        private void btnprocesar_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                if (MessageBox.Show("¿Desea procesar todos los el estudio al 100%?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        new CobranzaDAO().procesarEstudios(item.numerocita.ToString(), item.codigopaciente.ToString(), item.numeroatencion.ToString(), item.empresa);
                        //cargarEstudios();
                        printSegOnline();
                        MessageBox.Show("Se proceso la atención al 100 %");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }

            }
        }

        private void lbldireccionReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.direccionReceptor = lbldireccionReceptor.Text.Trim();
        }

        private void txtcorreoReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.emailReceptor = txtcorreoReceptor.Text;
        }

        private void BtnVerInforme_Click(object sender, RoutedEventArgs e)
        {
            var lista = new CobranzaDAO().getInforme(item.numeroatencion);
            foreach (var doc in lista)
            {
                string filepath = Path.GetTempPath() + doc.filename;
                if (File.Exists(filepath))
                    File.Delete(filepath);
                if (!File.Exists(filepath))
                {
                    FileStream archivo = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                    archivo.Write(doc.cuerpo, 0, doc.cuerpo.Length);
                    archivo.Close();
                    //   File.WriteAllBytes(filepath, doc.cuerpo);
                }

                System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                d.FileName = filepath;
                //d.Verb = "pdf";
                System.Diagnostics.Process.Start(d);
            }
        }

        private void txtinformacion_LostFocus(object sender, RoutedEventArgs e)
        {
            item.textoinformacion = txtinformacion.Text;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            regresarEstado();
            if (tipocobranza == TIPO_COBRANZA.PACIENTE && btnguardar.IsEnabled == false)
            {
                if (grid_formaPago.Items.Count == 0)
                    if (MessageBox.Show("No tiene registrado ninguna forma de pago.\n¿Desea realmente cerrar la ventana?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        e.Cancel = true;
            }
        }

        private void btnCortesia_Click(object sender, RoutedEventArgs e)
        {
            foreach (var de in item.detalleItems)
            {
                de.isGratuita = !de.isGratuita;
                if (de.isGratuita)
                    de.tipoIGV = (int)TIPO_IGV.INAFECTO_RETIRO;
                else
                {
                    de.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                }
                if (de.isGratuita && de.isCortesia)
                    de.isCortesia = false;

            }
            setDetalleDocumento();
        }

        private void HCPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            frmHistoriaPaciente gui = new frmHistoriaPaciente();
            gui.cargarGUI(session);
            gui.Show();
            gui.buscarPaciente(item.codigopaciente.ToString(), 2);
        }

        private void txtmonto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                agregarFormaPago();
        }

        private void lblRazReceptor_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenuPaciente.Visibility = Visibility.Visible;
        }

        private void gridDetalle_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "valorUnitario")
            {
                if (e.NewData.ToString() != e.OldData.ToString())
                {
                    DetalleDocumentoSunat detalle = (DetalleDocumentoSunat)e.Cell.DataContext;

                    new CobranzaDAO().updatePrecioFinal(item.numerocita.ToString(), detalle.codigoitem, detalle.valorUnitarioigv.ToString());
                }
            }
        }

        private void BtnVerAdjuntos_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var escanAdmin = db.ESCANADMISION.SingleOrDefault(x => x.numerodeatencion == item.numeroatencion);
                if (escanAdmin != null)
                {
                    string pathfile = Path.GetTempPath() + escanAdmin.nombrearchivo;
                    if (System.IO.File.Exists(pathfile))
                        System.IO.File.Delete(pathfile);

                    System.IO.File.WriteAllBytes(pathfile, escanAdmin.cuerpoarchivo);
                    System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                    printJob = new System.Diagnostics.Process();
                    printJob.StartInfo.FileName = pathfile;
                    printJob.Start();
                }
                var docescaneado = db.DOCESCANEADO.SingleOrDefault(x => x.codigodocadjunto == _docCarta);
                if (docescaneado != null)
                {
                    if (docescaneado.isFisico)
                    {
                        System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                        printJob = new System.Diagnostics.Process();
                        printJob.StartInfo.FileName = @"\\serverweb\DocumentoAjunto\" + docescaneado.codigodocadjunto + @"\" + docescaneado.nombrearchivo;
                        printJob.Start();
                    }
                    else
                    {
                        string pathfile = Path.GetTempPath() + docescaneado.nombrearchivo;
                        if (System.IO.File.Exists(pathfile))
                            System.IO.File.Delete(pathfile);

                        System.IO.File.WriteAllBytes(pathfile, docescaneado.cuerpoarchivo);
                        System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                        printJob = new System.Diagnostics.Process();
                        printJob.StartInfo.FileName = pathfile;
                        printJob.Start();

                    }
                }
            }
        }

        private void btnVerificarCDR_Click(object sender, RoutedEventArgs e)
        {
            /*filename = @"\\serverweb\Facturacion\319302\ZIP\20297451023-01-FF02-752.zip";
            item.empresa = 1;
            item.numeroDocumento = "F02-752";
            item.numeroDocumentoSUNAT = "F"+item.numeroDocumento;
            item.tipoDocumento = 1;
            item.fechaEmision = Tool.getDatetime();
            new CobranzaDAO().crearDocumentoXML(item);*/
            if (filename != "")
            {
                try
                {
                    string pathCDR = new ServiceSunat().getCDR(item);
                    string msjResutlado = "";
                    if (new CobranzaDAO().verificarCDR(pathCDR, out msjResutlado))
                    {
                        item.isSendSUNAT = true;
                        //adelantamos el correlativo                                              
                        procesarDocumentoElectronico();
                    }
                    else
                        MessageBox.Show(msjResutlado, "ERROR AL VERIFICAR DOCUMENTO");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    /*   */
                }

            }
        }

        private void MenuItemQuitarCobertura_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat detalle = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (detalle != null)
                if (MessageBox.Show("¿Desea quitar la Cobertura?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (tipocobranza == TIPO_COBRANZA.PACIENTE)
                    {
                        detalle.porcentajeCobSeguro = 0;
                        detalle.porcentajeCobPaciente = 100 - detalle.porcentajeCobSeguro;
                    }
                    else
                    {
                        detalle.porcentajeCobSeguro = 100;
                        detalle.porcentajeCobPaciente = 100 - detalle.porcentajeCobSeguro;
                    }
                    var _item = item.detalleItems.SingleOrDefault(x => x.codigoitem == detalle.codigoitem);
                    if (_item != null)
                    {
                        _item = detalle;
                    }
                    gridDetalle.ItemsSource = null;
                    gridDetalle.ItemsSource = item.detalleItems;
                    setDetalleDocumento();
                }


        }

        private void MenuItemQuitarPromocionClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat detalle = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (detalle != null)
                if (MessageBox.Show("¿Desea quitar la promoción?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    detalle.porcentajeDescPromocion = 0;
                    gridDetalle.ItemsSource = null;
                    gridDetalle.ItemsSource = item.detalleItems;
                    calcularcabecera();
                }


        }

        private void btnAddAtencion_Click(object sender, RoutedEventArgs e)
        {
            frmAddAtencion gui = new frmAddAtencion();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.numero != 0)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var atencion = db.ATENCION.Where(x => x.numeroatencion == gui.numero && x.codigopaciente == item.codigopaciente).SingleOrDefault();
                    if (atencion != null)
                    {
                        if (!item.atenciones.Contains(gui.numero))
                            item.atenciones.Add(gui.numero);
                        refreshData();
                        cargarEstudios();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro una atencion con los datos brindados");
                    }
                }

            }

        }

        private void btnReenviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathCDR = new ServiceSunat().sendBill(item.empresa, item.codigopaciente, filename);

                string msjResutlado = "";
                if (new CobranzaDAO().verificarCDR(pathCDR, out msjResutlado))
                {
                    item.isSendSUNAT = true;
                    //adelantamos el correlativo                                              
                    procesarDocumentoElectronico();
                }
                else
                    MessageBox.Show(msjResutlado, "ERROR AL ENVIAR DOCUMENTO");
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);

            }
        }

        private void btnTarifario_Click(object sender, RoutedEventArgs e)
        {
            frmTarifario gui = new frmTarifario();
            gui.cargarGUI(session, item.codigocompaniaseguro, item.empresa, item.aseguradora);
            gui.Show();
        }

        private void MenuItemCortesia_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat det = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (det != null)
            {
                var _item = item.detalleItems.SingleOrDefault(x => x.codigoitem == det.codigoitem);
                if (_item != null)
                {
                    det.isGratuita = !det.isGratuita;
                    if (det.isGratuita)
                        det.tipoIGV = (int)TIPO_IGV.INAFECTO_RETIRO;
                    else
                    {
                        if (det.codigoitem.Substring(7, 2) == "99")
                            det.tipoIGV = (int)TIPO_IGV.NO_DECLARAR_SUNAT;
                        else
                            det.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                    }
                    _item = det;
                    setDetalleDocumento();
                }
            }
        }

        private void MenuItemDescuento_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat det = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (det != null)
            {
                var _item = item.detalleItems.SingleOrDefault(x => x.codigoitem == det.codigoitem);
                if (_item != null)
                {
                    det.isCortesia = !det.isCortesia;
                    _item = det;
                    setDetalleDocumento();
                }
            }
        }

        private void btnTipoCambio_Click(object sender, RoutedEventArgs e)
        {
            frmIGV gui = new frmIGV();
            gui.cargarGUI(session);
            gui.Owner = this;
            gui.ShowDialog();
        }

        private void BtnVerBoleta_Click(object sender, RoutedEventArgs e)
        {
            List<string> documentos = new CobranzaDAO().getDocumentosCobranza(item.empresa, item.codigopaciente, item.atenciones);
            foreach (var doc in documentos)
            {
                System.Diagnostics.Process printJob = new System.Diagnostics.Process();
                printJob.StartInfo.FileName = doc;
                printJob.Start();
            }
        }

        private void gridDetalle_Deleting(object sender, Telerik.Windows.Controls.GridViewDeletingEventArgs e)
        {
            DetalleDocumentoSunat detalle = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (detalle != null)
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    if (MessageBox.Show("¿Desea ocultar el estudio?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        /* var _det = item.detalleItems.Where(x => x.codigoitem == detalle.codigoitem).SingleOrDefault();
                         if (_det != null)
                         {*/
                        detalle.cantidad = detalle.cantidad - 1;
                        if (detalle.cantidad == 0)
                            item.detalleItems.Remove(detalle);
                        gridDetalle.ItemsSource = null;
                        gridDetalle.ItemsSource = item.detalleItems;
                        calcularcabecera();
                        /* }*/
                    }
                    else
                        e.Cancel = true;

                }
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            Active = true;
            Thread = new System.Threading.Thread(() =>
            {
                Connection = new HubConnection(Host);
                Proxy = Connection.CreateHubProxy("EmetacHub");

                Connection.Start();

                while (Active)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }) { IsBackground = true };
            Thread.Start();
        }

        private async Task SendMessage()
        {
            await Proxy.Invoke("CallPaciente2");
        }



    }
}

