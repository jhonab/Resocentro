using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using OnBarcode.Barcode;
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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frnNotasdeCreditoDebito.xaml
    /// </summary>
    public partial class frnNotasdeCreditoDebito : Window
    {
        MySession session;
        DocumentoSunat doc;
        string filename = "";
        string _pathPDF = "";

        public frnNotasdeCreditoDebito()
        {
            InitializeComponent();

        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboempresa.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
            cboempresa.SelectedValuePath = "codigoInt";
            cboempresa.DisplayMemberPath = "nombreShort";
            cboempresa.SelectedIndex = 0;

            cbotipoNota.ItemsSource = Enum.GetValues(typeof(TIPO_NOTA_DE_CREDITO)).Cast<TIPO_NOTA_DE_CREDITO>();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtnumeroDocumento.Text != "")
                buscarDocumento();
            else
                MessageBox.Show("Ingrese el N° del Documento");
        }
        private void txtnumeroDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtnumeroDocumento.Text != "")
                    buscarDocumento();
                else
                    MessageBox.Show("Ingrese el N° del Documento");
            }
        }
        private void buscarDocumento()
        {
            try
            {
                gridDocumento.ItemsSource = new CobranzaDAO().getDocumentosEmitidos(txtnumeroDocumento.Text, "2", int.Parse(cboempresa.SelectedValue.ToString().Substring(0, 1)), cboTipoDocumentoReferencia.Text.Substring(0, 2));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void gridDocumento_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            DocumentosEmitidos item = (DocumentosEmitidos)e.Row.DataContext;
            if (item != null)
            {
                if (item.estado != "A")
                {
                    if (item.tipodocumento == "FACTURA" && !item.isSendSunat)
                    {
                        MessageBox.Show("El Documento no fue enviado a Sunat");
                        return;
                    }
                    try
                    {
                        gridDetalleDocumento.ItemsSource = null;
                        gridDetalleDocumento.ItemsSource = new CobranzaDAO().getDetalleDocumentosEmitidos(item.numerodocumento, item.codigopaciente.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }


                }
                else
                    MessageBox.Show("El Documento esta anulado");

            }
        }

        private void btnguardar_Click(object sender, RoutedEventArgs e)
        {
            if (validarDatos())
            {
                DocumentosEmitidos item = (DocumentosEmitidos)gridDocumento.SelectedItem;
                doc = new DocumentoSunat();
                List<DetalleDocumentoEmitidos> listadetalle = (List<DetalleDocumentoEmitidos>)gridDetalleDocumento.Items.SourceCollection;
                if (listadetalle.Where(x => x.isSelected).ToList().Count() > 0)
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var _cobranzaDao = new CobranzaDAO();

                        int codigosede = int.Parse(cboempresa.SelectedValue.ToString());
                        int unidad = codigosede / 100;
                        int sede = codigosede - (unidad * 100);
                        string tipoDocumentoRerefencia = cboTipoDocumentoReferencia.Text.Substring(0, 2);
                        string tipoDocumento = cboTipoDocumento.Text.Substring(0, 2);
                        var _numdoc = _cobranzaDao.getNumeroDocumento(tipoDocumento, unidad.ToString());
                        if (MessageBox.Show("Está seguro de seguir con el registro del Documento N°:" + _numdoc, "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {

                            doc.numeroDocumento = _numdoc;
                            doc.numeroDocumentoSUNAT = doc.numeroDocumento;
                            doc.empresa = unidad;
                            doc.sede = sede;
                            doc.fechaEmision = Tool.getDatetime();
                            doc.tipoDocumento = int.Parse(tipoDocumento);
                            doc.idFirmaDigital = "JHONAB";
                            doc.isSendSUNAT = false;
                            doc.direccionsede = db.SUCURSAL.SingleOrDefault(x => x.codigounidad == unidad && x.codigosucursal == sede).direccionfactura;
                            doc.detalleItems = new List<DetalleDocumentoSunat>();
                            doc.TCCompra = item.tipocambio;
                            doc.TCVenta = item.tipocambio;
                            doc.igvPorcentaje = item.valorIGV;
                            doc.textoinformacion = "";
                            doc.tipoDocumentoReferencia = int.Parse(tipoDocumentoRerefencia);
                            /*if (doc.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                                doc.numeroDocumentoReferencia = "B" + txtnumeroDocumento.Text;
                            else*/
                            doc.numeroDocumentoReferencia = txtnumeroDocumento.Text.Substring(0, 1) + txtnumeroDocumento.Text;

                            doc.descripcionNotaCreditoDebito = txtmotivo.Text;
                            /*RECEPTOR*/
                            try
                            {
                                //*RELACIONAMOS EL DOCUMENTO A LA ATENCION*//
                                var _aseguradora = db.ASEGURADORA.SingleOrDefault(x => x.ruc == item.ruc_alterno);
                                var _paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == item.codigopaciente);
                                var numerolibrocaja = int.Parse(doc.numeroDocumentoReferencia.Split('-')[1]);
                                int? atencion = 8;


                                atencion = db.LIBROCAJA.Where(x => x.numerodocumento == numerolibrocaja && x.codigopaciente == _paciente.codigopaciente).Select(x => x.numeroatencion).SingleOrDefault();
                                if (atencion == 0)
                                    atencion = db.COBRANZACIASEGURO.Where(x => x.numerodocumento == doc.numeroDocumentoReferencia.Substring(1) && x.codigopaciente == _paciente.codigopaciente).Select(x => x.numeroatencion).SingleOrDefault();

                                if (atencion == 0)
                                    atencion = 8;

                                var _atencion = db.ATENCION.SingleOrDefault(x => x.numeroatencion == atencion);
                                //var _companiaseguro = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == _atencion.codigocompaniaseguro);
                                if (item.ruc_alterno == "10000000000")
                                {
                                    var tipodoc = db.TIPO_DOCUMENTO_IDENTIDAD.SingleOrDefault(x => x.tipo_doc_id == _paciente.tipo_doc);
                                    doc.tipoDocReceptor = int.Parse(tipodoc.tipo_doc_identificador);
                                    doc.rucReceptor = _paciente.dni;
                                    doc.razonSocialReceptor = _paciente.apellidos + " " + _paciente.nombres;
                                    doc.direccionReceptor = _paciente.direccion;
                                }
                                else
                                {
                                    doc.tipoDocReceptor = (int)TIPO_DOCUMENTOIDENTIDAD.RUC;
                                    doc.rucReceptor = _aseguradora.ruc;
                                    doc.razonSocialReceptor = _aseguradora.razonsocial;
                                    doc.direccionReceptor = _aseguradora.domiciliofiscal;
                                }
                                doc.paciente = _paciente.apellidos + ", " + _paciente.nombres;
                                doc.dnipaciente = _paciente.dni;
                                doc.codigopaciente = _paciente.codigopaciente;
                                doc.aseguradora = _atencion.CITA.COMPANIASEGURO.descripcion;
                                doc.aseguradora_ruc = item.ruc;
                                doc.numerocita = _atencion.numerocita;
                                doc.cmp = _atencion.cmp;
                                doc.codigomodalidad = _atencion.codigomodalidad;
                                doc.carta = _atencion.CITA.codigocartagarantia;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERROR al obtener al EMISOR\n" + ex.Message);
                                return;
                            }



                            if (doc.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO)
                                doc.tipoNotaCreditoDebito = (int)Enum.Parse(typeof(TIPO_NOTA_DE_CREDITO), cbotipoNota.SelectedValue.ToString());
                            else
                                doc.tipoNotaCreditoDebito = (int)Enum.Parse(typeof(TIPO_NOTA_DE_DEBITO), cbotipoNota.SelectedValue.ToString());
                            foreach (DetalleDocumentoEmitidos dd in listadetalle.Where(x => x.isSelected).ToList())
                            {
                                var det = new DetalleDocumentoSunat();
                                det.tipoDocumentoReferencia = doc.tipoDocumentoReferencia;
                                det.cantidad = dd.cantidad;
                                det.valorIgvActual = doc.igvPorcentaje;
                                det.tipo_documento = doc.tipoDocumento;
                                det.porcentajedescxItem = 0;//cortesia
                                det.tipo_cobranza = item.tipocobranza;
                                doc.codigoMoneda = item.tipomoneda;
                                det.simboloMoneda = doc.codigoMoneda.ToString() == "1" ? "S/." : doc.codigoMoneda.ToString() == "2" ? "$" : "";
                                det.tipo_cobranza = cboTipoCobranza.SelectedIndex;
                                det.codigoitem = dd.codigoestudio;
                                det.codigoclase = dd.codigoclase.ToString();
                                det.valorUnitarioigv = Math.Round((Convert.ToDouble(dd.valorunitario.ToString()) * (1 + doc.igvPorcentaje)), 2);

                                det.valorReferencialIGV = doc.igvPorcentaje;
                                det.descripcion = dd.descripcion;
                                det.tipoIGV = dd.tipoIGV;
                                if (det.tipoIGV == (int)TIPO_IGV.INAFECTO_RETIRO)
                                {
                                    det.isGratuita = true;
                                    det.isCortesia = true;
                                    det.valorReferencialIGV = dd.valorunitario;
                                }

                                det.porcentajeDescPromocion = dd.porDesPromo;
                                det.porcentajedescxItem = dd.porDescuento;

                                if (item.carta.ToString().Trim() == "")
                                    det.isAsegurado = false;
                                else
                                    det.isAsegurado = true;
                                if (!dd.isModificado)
                                {
                                    if (det.isAsegurado)
                                    {
                                        if (det.tipo_cobranza == (int)TIPO_COBRANZA.PACIENTE)
                                        {
                                            det.porcentajeCobSeguro = dd.porDesCarta;
                                            det.porcentajeCobPaciente = 100 - det.porcentajeCobSeguro;
                                            det.montoMaximoAseguradora = Math.Round(((det.valorUnitario - det.descPromocion) * (det.porcentajeCobSeguro / 100)), 2);
                                        }
                                        else
                                        {
                                            det.porcentajeCobPaciente = dd.porDesCarta;
                                            det.porcentajeCobSeguro = 100 - det.porcentajeCobPaciente;
                                            det.montoMaximoAseguradora = Math.Round(((det.valorUnitario - det.descPromocion) * (det.porcentajeCobPaciente / 100)), 2);
                                        }
                                    }
                                }
                                doc.detalleItems.Add(det);

                            }
                            doc.subTotal = Math.Round(doc.detalleItems.Sum(x => x.valorVenta), 2);
                            doc.igvTotal = Math.Round(doc.detalleItems.Sum(x => x.igvItem_old), 2);
                            doc.descuentoGlobal = Math.Round((doc.subTotal * (doc.porcentajeDescuentoGlobal / 100)) + (doc.igvTotal * (doc.porcentajeDescuentoGlobal / 100)), 2);
                            doc.ventaTotal = Math.Round((doc.subTotal + doc.igvTotal) - doc.descuentoGlobal, 2);

                            doc = _cobranzaDao.calcularCabecera(doc);
                            if (_cobranzaDao.VerificarNumeroDocumento(doc.tipoDocumento.ToString().PadLeft(2, '0'), doc.numeroDocumento, doc.empresa.ToString()))
                            {
                                try
                                {
                                    filename = _cobranzaDao.crearDocumentoXML(doc);

                                    //printComprobante();
                                    if (doc.tipoDocumentoReferencia == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)//FACTURAS
                                    {
                                        //procesarDocumentoElectronico();
                                        if (filename != "")
                                        {
                                            try
                                            {
                                                //string pathCDR = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\RESULT\R-" + filename + ".zip";
                                                string pathCDR = new ServiceSunat().sendBill(doc.empresa, doc.codigopaciente, filename);
                                                string msjResutlado = "";
                                                if (_cobranzaDao.verificarCDR(pathCDR, out msjResutlado))
                                                {
                                                    doc.isSendSUNAT = true;
                                                    //adelantamos el correlativo                                              
                                                    procesarDocumentoElectronico();
                                                }
                                                else
                                                    MessageBox.Show(msjResutlado, "ERROR AL ENVIAR DOCUMENTO");
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
                                    //lblnumerodocumento.Content = item.numeroDocumento;
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
                else
                    MessageBox.Show("Seleccione un detalle para generar el documento", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Verifique los Datos", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void procesarDocumentoElectronico()
        {
            #region GENERAR IMPRESION GRAFICA
            string filename = doc.rucEmisor + "-" + doc.tipoDocumento.ToString("D2") + "-" + doc.numeroDocumentoSUNAT;
            string pathCODEBAR = Tool.PathDocumentosFacturacion + doc.codigopaciente.ToString() + @"\PDF417\" + filename + ".jpeg";
            _pathPDF = Tool.PathDocumentosFacturacion + doc.codigopaciente.ToString() + @"\PDF\" + filename + ".pdf";
            /*PDF417 _pdf417 = new PDF417();
            _pdf417.Data = doc.codigoBarraPDF417String.Trim();
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

            doc.pathCODEBAR = pathCODEBAR;
            _pdf417.drawBarcode(doc.pathCODEBAR);
            */

            doc.pathCODEBAR = pathCODEBAR;
            QrEncoder qrencoder = new QrEncoder(ErrorCorrectionLevel.Q);
            QrCode qrcode = new QrCode();
            qrencoder.TryEncode(doc.codigoBarraPDF417String.Trim(), out qrcode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero));
            using (MemoryStream ms = new MemoryStream())
            {
                renderer.WriteToStream(qrcode.Matrix, ImageFormat.Jpeg, ms);
                var imagetemporal = new Bitmap(ms);
                var imagen = new Bitmap(imagetemporal, new System.Drawing.Size(new System.Drawing.Point(200, 200)));
                imagen.Save(doc.pathCODEBAR, ImageFormat.Jpeg);
            }

            new CobranzaDAO().generarRepresentacionGrafica(doc, _pathPDF, pathCODEBAR);
            //new CobranzaDAO().generarRepresentacionGraficaPRELIQUIDACION(item, _pathPDFLIQUIDACION);
            doc.pathPDF = _pathPDF;
            //Using below code we can print any document
            #endregion
            guardarDocumentoBaseDatos(filename);
        }
        private void guardarDocumentoBaseDatos(string filename)
        {
            try
            {
                if (new CobranzaDAO().insertarNotaCreditoDebito(doc, session))
                {
                    btnguardar.IsEnabled = false;
                    lblnumerodocumento.Content = doc.numeroDocumento;
                    new CobranzaDAO().updateSerieCorrelativo(doc.tipoDocumento.ToString().PadLeft(2, '0'), doc.empresa.ToString());
                    printComprobante();
                    MessageBox.Show("Documento Generado");
                }
                else
                    MessageBox.Show("No se guardo el Documento", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
        private void printComprobante()
        {
            #region IMPRESION DOCUMENTOS ELECTRONICOS (SUNAT)

            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = _pathPDF;
            printJob.StartInfo.UseShellExecute = true;
            printJob.StartInfo.Verb = "printto";
            printJob.StartInfo.CreateNoWindow = true;
            printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            printJob.StartInfo.Arguments = "FACTURA";
            printJob.Start();
            #endregion

        }
        private bool validarDatos()
        {
            var item = (DocumentosEmitidos)gridDocumento.SelectedItem;
            if (item == null)
                return false;
            else if (cboTipoCobranza.SelectedIndex == 0)
                return false;
            else if (txtmotivo.Text == "")
                return false;
            else
                return true;
        }

        private void btncancrlar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            printComprobante();
        }

        private void cboTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTipoDocumento.SelectedValue != null && cbotipoNota != null)
            {
                cbotipoNota.ItemsSource = null;
                string tipoDocumento = cboTipoDocumento.SelectedItem.ToString().Split(':')[1].Substring(1, 2);
                if (tipoDocumento == "07")
                    cbotipoNota.ItemsSource = Enum.GetValues(typeof(TIPO_NOTA_DE_CREDITO)).Cast<TIPO_NOTA_DE_CREDITO>();
                else if (tipoDocumento == "08")
                    cbotipoNota.ItemsSource = Enum.GetValues(typeof(TIPO_NOTA_DE_DEBITO)).Cast<TIPO_NOTA_DE_DEBITO>();
                else { }
                cbotipoNota.SelectedIndex = 0;
            }
        }

        private void gridDetalleDocumento_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {

            List<DetalleDocumentoEmitidos> listadetalle = (List<DetalleDocumentoEmitidos>)gridDetalleDocumento.Items.SourceCollection;
            DocumentosEmitidos doc = (DocumentosEmitidos)gridDocumento.SelectedItem;
            var subtotal = listadetalle.Where(x => x.isSelected).Sum(x => x.valortotal);
            var igv = subtotal * doc.valorIGV;
            lblsubtotal.Content = "SubTotal: " + subtotal.ToString("#,###,###0.#0");
            lbligv.Content = "IGV: " + igv.ToString("#,###,###0.#0");
            lbltotal.Content = "Total: " + (subtotal + igv).ToString("#,###,###0.#0");
            gridDetalleDocumento.Items.Refresh();
        }

        private void btnVerificarCDR_Click(object sender, RoutedEventArgs e)
        {

            if (filename != "")
            {
                try
                {
                    string pathCDR = new ServiceSunat().getCDR(doc);
                    string msjResutlado = "";
                    if (new CobranzaDAO().verificarCDR(pathCDR, out msjResutlado))
                    {
                        doc.isSendSUNAT = true;
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

        private void btnReenviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathCDR = new ServiceSunat().sendBill(doc.empresa, doc.codigopaciente, filename);

                string msjResutlado = "";
                if (new CobranzaDAO().verificarCDR(pathCDR, out msjResutlado))
                {
                    doc.isSendSUNAT = true;
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

        private void gridDetalleDocumento_CellEditEnded(object sender, Telerik.Windows.Controls.GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "valorunitario")
            {
                if (e.NewData.ToString() != e.OldData.ToString())
                {
                    DocumentosEmitidos doc = (DocumentosEmitidos)gridDocumento.SelectedItem;

                    DetalleDocumentoEmitidos detalle = (DetalleDocumentoEmitidos)e.Cell.DataContext;
                    detalle.carta = 0;
                    detalle.cortesia = 0;
                    detalle.promocion = 0;
                    detalle.porDesPromo = 0;
                    detalle.porDesPromo = 0;
                    detalle.porDesCarta = 100;
                    detalle.valorunitario = Convert.ToDouble(e.NewData) / (1 + doc.valorIGV);
                    detalle.valortotal = detalle.cantidad * detalle.valorunitario;
                    detalle.isModificado = true;
                }
            }
        }


    }
}
