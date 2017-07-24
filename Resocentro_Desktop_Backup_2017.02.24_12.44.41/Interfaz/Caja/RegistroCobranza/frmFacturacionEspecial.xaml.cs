using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using OnBarcode.Barcode;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Cobranza;
using Resocentro_Desktop.Interfaz.frmUtil;
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
    /// Lógica de interacción para frmFacturacionEspecial.xaml
    /// </summary>
    public partial class frmFacturacionEspecial : Window
    {
        MySession session;
        DocumentoSunat item;
        TIPO_MONEDA tipomoneda = TIPO_MONEDA.SOL;
        TIPO_COBRANZA tipocobranza;
        public int codigopaciente;
        int unidad, sede, edadpaciente = 0;
        string _pathPDF = "";
        string filename = "";
        public frmFacturacionEspecial()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, int unidad, int sede)
        {
            this.session = session;
            this.unidad = unidad;
            this.sede = sede;
        }
        public void cargarCobranza(int codigopaciente)
        {
            this.codigopaciente = codigopaciente;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                item = new DocumentoSunat();
                var _paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == codigopaciente);

                if (_paciente != null)
                {
                    edadpaciente = UtilDAO.calcularEdad(_paciente.fechanace);

                    tipocobranza = TIPO_COBRANZA.ASEGURADORA;

                    if (edadpaciente < 18)
                        MessageBox.Show("El Paciente es menor de Edad.\nFecha de Nacimiento:" + _paciente.fechanace.ToShortDateString() + "\n\n Debe cambiar de paciente para poder emitir un Documento Valido mayor a 700 soles.");

                    var tipodoc = db.TIPO_DOCUMENTO_IDENTIDAD.SingleOrDefault(x => x.tipo_doc_id == _paciente.tipo_doc);
                    item.tipoDocReceptor = int.Parse(tipodoc.tipo_doc_identificador);

                    item.rucReceptor = _paciente.dni;
                    item.razonSocialReceptor = _paciente.apellidos + " " + _paciente.nombres;
                    item.direccionReceptor = _paciente.direccion;
                    item.emailReceptor = _paciente.email;


                    item.paciente = _paciente.apellidos + ", " + _paciente.nombres;
                    item.dnipaciente = _paciente.dni;
                    item.codigopaciente = _paciente.codigopaciente;
                    item.numeroatencion = 0;
                    item.codigocompaniaseguro = 49;//PARTICULAR
                    item.aseguradora = "PARTICULARES";
                    item.aseguradora_ruc = "10000000000";
                    item.numerocita = 0;
                    item.iscobranzaExterna = true;
                    item.cmp = "0";
                    item.codigomodalidad = 1;
                    item.empresa = unidad;
                    item.sede = sede;
                    item.carta = "";
                    item.infoCarta = "";
                    item.observaciones = "";
                    item.titular_Carta = "";
                    item.contratante_carta = "";
                    item.poliza_carta = "";
                    item.cobertura_carta = 0;
                    item.numerocarnet_carta = "";
                    item.cartascorelacionadas = "";

                    cargarEstudios();
                    refreshData();


                }
            }

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
                }
            }
            refreshData();
        }
        private void refreshData()
        {
            lbltipoDocumento.Content = item.documentoReceptorString;
            lblnumDocumentoReceptor.Text = item.rucReceptor;
            lblRazReceptor.Text = item.razonSocialReceptor;
            lbldireccionReceptor.Text = item.direccionReceptor;
            txtcorreoReceptor.Text = item.emailReceptor;
            setDetalleDocumento();
            lblTCCompra.Content = "TC Compra: " + (Math.Round(item.TCCompra, 2, MidpointRounding.AwayFromZero)).ToString();
            lblTCVenta.Content = "TC Venta: " + (Math.Round(item.TCVenta, 2, MidpointRounding.AwayFromZero)).ToString();

            item.textoinformacion = "PACIENTE: " + item.paciente + "\n";
            item.atenciones = new List<int>();
            txtinformacion.Text = item.textoinformacion;
        }
        private void setDetalleDocumento()
        {
            gridDetalle.ItemsSource = null;
            gridDetalle.ItemsSource = item.detalleItems;
            calcularcabecera();
        }
        private void calcularcabecera()
        {
            item.subTotal = Math.Round(item.detalleItems.Sum(x => x.valorVenta), 2, MidpointRounding.AwayFromZero);
            item.igvTotal = Math.Round(item.detalleItems.Sum(x => x.igvItem_old), 2, MidpointRounding.AwayFromZero);
            item.descuentoGlobal = Math.Round((item.subTotal * (item.porcentajeDescuentoGlobal / 100)) + (item.igvTotal * (item.porcentajeDescuentoGlobal / 100)), 2, MidpointRounding.AwayFromZero);
            item.ventaTotal = Math.Round((item.subTotal + item.igvTotal) - item.descuentoGlobal, 2, MidpointRounding.AwayFromZero);

            txtsubtotal.Text = item.subTotal.ToString("#,###,###0.#0");
            txtigv.Text = item.igvTotal.ToString("#,###,###0.#0");
            txttotal.Text = (item.ventaTotal).ToString("#,###,###0.#0");
        }
        private void cargarEstudios()
        {
            try
            {
                item.detalleItems = new List<DetalleDocumentoSunat>();
                var _item = new CobranzaDAO().getEstudios(item.numeroatencion, item, tipocobranza, tipomoneda);

                setDetalleDocumento();

                /*
                if (tipocobranza == TIPO_COBRANZA.ASEGURADORA && item.hasCoaseguro)
                {
                    frmRegistrarCobranza gui = new frmRegistrarCobranza();
                    gui.cargarGUI(session, item.empresa, item.sede, TIPO_COBRANZA.ASEGURADORA);
                    gui.cargarCobranza(item.numeroatencion, item.carta_Coaseguro);
                    gui.Show();
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        private void BtnAgregar_Estudio_Click(object sender, RoutedEventArgs e)
        {
            frmSeleccionarModalidad guimodalidad = new frmSeleccionarModalidad();
            guimodalidad.cargarGUI(unidad);

            if (guimodalidad.ShowDialog().Value)
            {
                frmSearchEstudio gui = new frmSearchEstudio();
                gui.ampliacion = "0";
                gui.modalidad = guimodalidad.modalidad;
                gui.sucursal = ((unidad * 100) + sede).ToString();
                gui.iscaja = true;
                gui.getClase();
                gui.ShowDialog();
                if (gui.estudio != null)
                {
                    try
                    {
                        DetalleDocumentoSunat d = new DetalleDocumentoSunat();
                        d.cantidad = 1;
                        d.valorIgvActual = item.igvPorcentaje;
                        d.tipo_documento = item.tipoDocumento;
                        d.porcentajedescxItem = 0;//cortesia
                        d.tipo_cobranza = (int)tipocobranza;
                        item.codigoMoneda = (int)tipomoneda;
                        d.simboloMoneda = item.codigoMoneda.ToString() == "1" ? "S/." : item.codigoMoneda.ToString() == "2" ? "$" : "";
                        d.isAsegurado = false;

                        d.codigoitem = gui.estudio.codigoestudio;
                        d.codigoclase = d.codigoitem.Substring(6, 1);
                        d.valorUnitarioigv = 0;
                        d.valorReferencialIGV = d.valorUnitarioigv;
                        d.descripcion = gui.estudio.estudio;
                        d.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                        item.detalleItems.Add(d);
                        setDetalleDocumento();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                }
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

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            printComprobante();
        }
        private void printComprobante()
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = _pathPDF;
            printJob.StartInfo.UseShellExecute = true;
            printJob.StartInfo.Verb = "printto";
            printJob.StartInfo.CreateNoWindow = true;
            printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            printJob.StartInfo.Arguments = "FACTURA";
            printJob.Start();
        }
        private void procesarDocumentoElectronico()
        {
            #region GENERAR IMPRESION GRAFICA
            string filename = item.rucEmisor + "-" + item.tipoDocumento.ToString("D2") + "-" + item.numeroDocumentoSUNAT;
            string pathCODEBAR = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\PDF417\" + filename + ".jpeg";
            _pathPDF = Tool.PathDocumentosFacturacion + item.codigopaciente.ToString() + @"\PDF\" + filename + ".pdf";
            //PDF417 _pdf417 = new PDF417();
            //_pdf417.Data = item.codigoBarraPDF417String.Trim();
            //_pdf417.RowCount = 20;
            //_pdf417.ColumnCount = 20;
            //_pdf417.ECL = OnBarcode.Barcode.PDF417ECL.Level_0;

            ////_pdf417.Compact = false;
            //_pdf417.DataMode = PDF417DataMode.Text;
            //_pdf417.UOM = UnitOfMeasure.CM;
            ////_pdf417.BackColor = System.Drawing.Color.White;
            ////_pdf417.ForeColor = System.Drawing.Color.Black;
            //_pdf417.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            //_pdf417.Resolution = 72;
            ////_pdf417.X = 2;
            //_pdf417.LeftMargin = 0;
            //_pdf417.RightMargin = 0;
            //_pdf417.TopMargin = 0;
            //_pdf417.BottomMargin = 0;
            //_pdf417.Rotate = Rotate.Rotate0;
            ////_pdf417.ResizeImage = true;
            ////_pdf417.Resolution = 100;

            //item.pathCODEBAR = pathCODEBAR;
            //_pdf417.drawBarcode(item.pathCODEBAR);

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
        private void guardarDocumentoBaseDatos(string filename)
        {
            try
            {
                if (new CobranzaDAO().insertarDocumento(item, session))
                {
                    btnguardar.IsEnabled = false;
                    gridDetalle.IsEnabled = false;
                    new CobranzaDAO().updateSerieCorrelativo(item.tipoDocumento.ToString().PadLeft(2, '0'), item.empresa.ToString());
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
                }
                else
                    MessageBox.Show("No se guardo el Documento", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
        private void btnVerificarCDR_Click(object sender, RoutedEventArgs e)
        {
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

            }
        }

        private void btncancrlar_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void txtinformacion_LostFocus(object sender, RoutedEventArgs e)
        {
            item.textoinformacion = txtinformacion.Text;
        }
        private void gridDetalle_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            DetalleDocumentoSunat det = (DetalleDocumentoSunat)e.EditedItem;

            /*var _item = item.detalleItems.SingleOrDefault(x => x.codigoitem == det.codigoitem);
            if (_item != null
            {
                _item = det;*/
            setDetalleDocumento();

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
                                try
                                {
                                    filename = _cobranzaDao.crearDocumentoXML(item);

                                    if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)//FACTURAS
                                    {

                                        if (filename != "")
                                        {
                                            try
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
                                            catch (Exception ex)
                                            {
                                                btnguardar.Visibility = Visibility.Collapsed;
                                                btnVerificarCDR.IsEnabled = true;
                                                MessageBox.Show(ex.Message);
                                            }

                                        }
                                    }
                                    else //BOLETAS
                                    {
                                        procesarDocumentoElectronico();
                                    }
                                    lblnumerodocumento.Content = item.numeroDocumento;
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

        private void lbldireccionReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.direccionReceptor = lbldireccionReceptor.Text.Trim();
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
                        /*}*/
                    }

                }
        }

        private void RadContextMenuDetalle_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DetalleDocumentoSunat item = (DetalleDocumentoSunat)this.gridDetalle.SelectedItem;
            if (item == null)
                RadContextMenuDetalle.Visibility = Visibility.Collapsed;
            else
                RadContextMenuDetalle.Visibility = Visibility.Visible;
        }

        private void txtcorreoReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.emailReceptor = txtcorreoReceptor.Text;
        }
    }
}
