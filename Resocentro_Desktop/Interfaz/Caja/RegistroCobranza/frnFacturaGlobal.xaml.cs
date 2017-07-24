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
    /// Lógica de interacción para frnFacturaGlobal.xaml
    /// </summary>
    public partial class frnFacturaGlobal : Window
    {
        MySession session;

        DocumentoSunat item;
        FacturaGlobal facturaGlobal;
        List<PreResumenFacGlobal> lista;
        int edadpaciente = 0;
        TIPO_MONEDA tipomoneda = TIPO_MONEDA.SOL;
        TIPO_COBRANZA tipocobranza;
        public int codigopaciente;
        string _pathPDF = "";
        string filename = "";
        string pathLiquidacion = "";
        public frnFacturaGlobal()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            item = new DocumentoSunat();
            item.detalleItems = new List<DetalleDocumentoSunat>();
            item.atenciones = new List<int>();
            tipocobranza = TIPO_COBRANZA.ASEGURADORA;
            facturaGlobal = new FacturaGlobal();
            lista = new List<PreResumenFacGlobal>();
        }
        private void btnBuscarFacturas_Click(object sender, RoutedEventArgs e)
        {
            frmSearchFacturas gui = new frmSearchFacturas();
            gui.ShowDialog();
            if (gui.cabecera != null)
            {
                setfactura(gui.cabecera);
            }
        }

        private void setfactura(FacturaGlobal cabecera)
        {
            facturaGlobal = cabecera;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var _aseguradora = db.ASEGURADORA.Where(x => x.ruc == facturaGlobal.ruc).Select(x => x).SingleOrDefault();
                if (_aseguradora != null)
                {
                    item.tipoDocReceptor = (int)TIPO_DOCUMENTOIDENTIDAD.RUC;
                    item.rucReceptor = _aseguradora.ruc;
                    item.razonSocialReceptor = _aseguradora.razonsocial;
                    item.direccionReceptor = _aseguradora.domiciliocomercial;
                    item.emailReceptor = _aseguradora.email;
                    if (facturaGlobal.datosadicionales != null)
                        item.textoinformacion = facturaGlobal.datosadicionales.Trim();
                    item.numeroatencion = 0;
                    item.codigocompaniaseguro = 49;//PARTICULAR
                    item.aseguradora = "PARTICULARES";
                    item.aseguradora_ruc = "10000000000";
                    item.numerocita = 0;
                    item.iscobranzaExterna = true;
                    item.cmp = "0";
                    item.codigomodalidad = 1;
                    item.empresa = facturaGlobal.empresa;
                    item.sede = 1;
                    item.carta = "";
                    item.infoCarta = "";
                    item.observaciones = "";
                    item.titular_Carta = "";
                    item.contratante_carta = "";
                    item.poliza_carta = "";
                    item.cobertura_carta = 0;
                    item.numerocarnet_carta = "";
                    item.cartascorelacionadas = "";
                    item.igvPorcentaje = new CobranzaDAO().getIGV();
                    item.TCCompra = new CobranzaDAO().getTC();
                    item.TCVenta = new CobranzaDAO().getTC();
                    cargarEstudios();
                    refreshData();

                }
            }
        }
        private void refreshData()
        {
            lbltipoDocumento.Content = item.documentoReceptorString;
            lblnumDocumentoReceptor.Text = item.rucReceptor;
            lblRazReceptor.Text = item.razonSocialReceptor;
            lbldireccionReceptor.Text = item.direccionReceptor;
            txtcorreoReceptor.Text = item.emailReceptor;
            lblTCCompra.Content = "TC Compra: " + (Math.Round(item.TCCompra, 2, MidpointRounding.AwayFromZero)).ToString();
            lblTCVenta.Content = "TC Venta: " + (Math.Round(item.TCVenta, 2, MidpointRounding.AwayFromZero)).ToString();


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
            /* try
             {
                 var _item = new CobranzaDAO().getEstudios(item.numeroatencion, item, tipocobranza, tipomoneda);
                 item.detalleItems = new List<DetalleDocumentoSunat>();
                 using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                 {
                     var lista = db.DetalleFacturaGlobal.Where(x => x.idFac == facturaGlobal.idFac).ToList();

                     foreach (var clase in lista.Select(x => x.codigoestudio.Substring(6, 1)).Distinct())
                     {

                         if (int.Parse(clase) > 0 && int.Parse(clase) < 9)
                         {
                             foreach (var modalidad in lista.Where(x => x.codigoestudio.Substring(6, 1) == clase).Select(x => x.codigoestudio.Substring(3, 2)).Distinct())
                             {
                                 var _codigoestudio = item.empresa.ToString() + "01" + modalidad + "0010000";
                                 var _estudio = db.ESTUDIO.Where(x => x.codigoestudio == _codigoestudio).SingleOrDefault();
                                 if (_estudio != null)
                                 {
                                     DetalleDocumentoSunat d = new DetalleDocumentoSunat();
                                     d.cantidad = lista.Where(x => x.codigoestudio.Substring(6, 1) == clase).Count();
                                     d.valorIgvActual = item.igvPorcentaje;
                                     d.tipo_documento = item.tipoDocumento;
                                     d.porcentajedescxItem = 0;//cortesia
                                     d.tipo_cobranza = (int)tipocobranza;
                                     item.codigoMoneda = (int)tipomoneda;
                                     d.simboloMoneda = item.codigoMoneda.ToString() == "1" ? "S/." : item.codigoMoneda.ToString() == "2" ? "$" : "";
                                     d.isAsegurado = false;

                                     d.codigoitem = _estudio.codigoestudio;
                                     d.descripcion = _estudio.nombreestudio;
                                     d.codigoclase = d.codigoitem.Substring(6, 1);
                                     var valor_total = Convert.ToDouble(lista.Where(x => x.codigoestudio.Substring(6, 1) == clase).Sum(x => x.precio).ToString());
                                     d.valorUnitarioigv = Convert.ToDouble(
                                         ((valor_total ) / d.cantidad
                                         )
                                         .ToString());
                                     d.valorReferencialIGV = d.valorUnitarioigv;
                                     d.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                                     item.detalleItems.Add(d);
                                 }
                             }

                         }
                         else
                         {

                             var _codigoestudio = item.empresa.ToString() + "0101001000";

                             int cantidadContrastes = lista.Where(x => x.codigoestudio.Substring(6, 1) == clase && x.nombreestudio.ToLower().Contains("contraste")).Count();
                             if (cantidadContrastes > 0)
                             {
                                 var _estudioContraste = db.ESTUDIO.Where(x => x.codigoestudio == _codigoestudio + "1").SingleOrDefault();
                                 if (_estudioContraste != null)
                                 {
                                     DetalleDocumentoSunat d = new DetalleDocumentoSunat();
                                     d.cantidad = cantidadContrastes;
                                     d.valorIgvActual = item.igvPorcentaje;
                                     d.tipo_documento = item.tipoDocumento;
                                     d.porcentajedescxItem = 0;//cortesia
                                     d.tipo_cobranza = (int)tipocobranza;
                                     item.codigoMoneda = (int)tipomoneda;
                                     d.simboloMoneda = item.codigoMoneda.ToString() == "1" ? "S/." : item.codigoMoneda.ToString() == "2" ? "$" : "";
                                     d.isAsegurado = false;

                                     d.codigoitem = _estudioContraste.codigoestudio;
                                     d.descripcion = _estudioContraste.nombreestudio;
                                     d.codigoclase = d.codigoitem.Substring(6, 1);
                                     double valor_total = Convert.ToDouble(lista.Where(x => x.codigoestudio.Substring(6, 1) == clase && x.nombreestudio.ToLower().Contains("contras")).Sum(x => x.precio).ToString());
                                     d.valorUnitarioigv = Convert.ToDouble(
                                         ((valor_total ) / d.cantidad
                                         )
                                         .ToString());
                                     d.valorReferencialIGV = d.valorUnitarioigv;
                                     d.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                                     item.detalleItems.Add(d);
                                 }
                             }
                             int cantidadSedaciones = lista.Where(x => x.codigoestudio.Substring(6, 1) == clase && x.nombreestudio.ToLower().Contains("sedaci")).Count();
                             if (cantidadSedaciones > 0)
                             {
                                 var _estudioSedacion = db.ESTUDIO.Where(x => x.codigoestudio == _codigoestudio + "2").SingleOrDefault();
                                 if (_estudioSedacion != null)
                                 {
                                     DetalleDocumentoSunat d = new DetalleDocumentoSunat();
                                     d.cantidad = cantidadSedaciones;
                                     d.valorIgvActual = item.igvPorcentaje;
                                     d.tipo_documento = item.tipoDocumento;
                                     d.porcentajedescxItem = 0;//cortesia
                                     d.tipo_cobranza = (int)tipocobranza;
                                     item.codigoMoneda = (int)tipomoneda;
                                     d.simboloMoneda = item.codigoMoneda.ToString() == "1" ? "S/." : item.codigoMoneda.ToString() == "2" ? "$" : "";
                                     d.isAsegurado = false;

                                     d.codigoitem = _estudioSedacion.codigoestudio;
                                     d.descripcion = _estudioSedacion.nombreestudio;
                                     d.codigoclase = d.codigoitem.Substring(6, 1);
                                     var valor_total = Convert.ToDouble(lista.Where(x => x.codigoestudio.Substring(6, 1) == clase && x.nombreestudio.ToLower().Contains("sedaci")).Sum(x => x.precio).ToString());
                                     d.valorUnitarioigv = Convert.ToDouble(
                                         ((valor_total ) / d.cantidad
                                         )
                                         .ToString());
                                     d.valorReferencialIGV = d.valorUnitarioigv;
                                     d.tipoIGV = (int)TIPO_IGV.GRAVADO_ONEROSA;
                                     item.detalleItems.Add(d);
                                 }
                             }

                         }


                     }
                 }


                 setDetalleDocumento();

             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message, "Error");
             }*/
            try
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    //var lista = db.DetalleFacturaGlobal.Where(x => x.idFac == facturaGlobal.idFac).ToList();
                    //item.atenciones = lista.Select(x => x.atencion).Distinct().ToList();                   
                    lista = new CobranzaDAO().getDetalleFacturaGlobal(facturaGlobal.idFac);
                    txtprefacturacion.Text = "ID Pre-Fac: " + facturaGlobal.idFac +
                        "\nTotal Pre-Fac Soles: S/" + lista.Where(x => x.codigomoneda == 1).Sum(x => x.preciobruto).ToString("#,###,###0.#0") +
                        "\nTotal Pre-Fac Dolares: $" + lista.Where(x => x.codigomoneda == 2).Sum(x => x.preciobruto).ToString("#,###,###0.#0");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }
        private void RadRadioButton_Click(object sender, RoutedEventArgs e)
        {
            tipomoneda = TIPO_MONEDA.DOLAR;

            cambiotipomoneda();
        }

        private void cambiotipomoneda()
        {
            if (item.codigoMoneda != (int)tipomoneda)
            {
                item.codigoMoneda = (int)tipomoneda;
                foreach (var det in item.detalleItems)
                {
                    if (tipomoneda == TIPO_MONEDA.SOL)
                        det.valorUnitarioigv = Math.Round(det.valorUnitarioigv * item.TCVenta, 2);
                    else if (tipomoneda == TIPO_MONEDA.DOLAR)
                        det.valorUnitarioigv = Math.Round(det.valorUnitarioigv / item.TCVenta, 2);
                    else { }
                }
                setDetalleDocumento();
            }
        }
        private void RadRadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            tipomoneda = TIPO_MONEDA.SOL;
            cambiotipomoneda();
        }
        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            printComprobante();
        }
        private void printComprobante()
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = _pathPDF;
            printJob.StartInfo.UseShellExecute = true;
            printJob.StartInfo.Verb = "printto";
            printJob.StartInfo.CreateNoWindow = true;
            printJob.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            printJob.StartInfo.Arguments = "FACTURA";
            printJob.Start();

            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = pathLiquidacion;
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
            pathLiquidacion = @"\\serverweb\Facturacion\PREFACTURA\PREFACTURA-" + facturaGlobal.idFac + ".pdf";
            new CobranzaDAO().generarRepresentacionGraficaPREFACTURA(lista, facturaGlobal, pathLiquidacion);
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
                    new CobranzaDAO().updateFacturaGlobal(facturaGlobal.idFac, item.numeroDocumento);
                    if (item.emailReceptor != "")
                    {
                        try
                        {
                            CobranzaDAO.sendCorreoDocumentoGenerado(item, filename, pathLiquidacion);
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
                    /*   */
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
        private void lbldireccionReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.direccionReceptor = lbldireccionReceptor.Text.Trim();
        }
        private void txtcorreoReceptor_LostFocus(object sender, RoutedEventArgs e)
        {
            item.emailReceptor = txtcorreoReceptor.Text;
        }
        private void gridDetalle_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            DetalleDocumentoSunat det = (DetalleDocumentoSunat)e.EditedItem;


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

                if (_rucReceptorLength == 11)
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
            //PACIENTE
            if (item.codigopaciente == 0)
            {
                result = false;
                MessageBox.Show("Seleccione el Paciente", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //PREFACTURA
            if (lista.Count == 0)
            {
                result = false;
                MessageBox.Show("Seleccione la Pre-Factura", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

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
        private void btnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            frmSearchPaciente gui = new frmSearchPaciente();
            gui.ShowDialog();
            if (gui.paciente != null)
            {
                item.paciente = gui.paciente.apellidos.ToUpper().Trim() + ", " + gui.paciente.nombres.ToUpper().Trim();
                item.dnipaciente = gui.paciente.dni;
                item.codigopaciente = gui.paciente.codigopaciente;
                txtpaciente.Text = item.paciente;
                codigopaciente = item.codigopaciente;
            }
            else
            {
                item.paciente = "";
                item.dnipaciente = "";
                item.codigopaciente = 0;
                txtpaciente.Text = "";
                codigopaciente = 0;
            }
        }

        private void BtnAgregar_Estudio_Click(object sender, RoutedEventArgs e)
        {
            frmSeleccionarModalidad guimodalidad = new frmSeleccionarModalidad();
            guimodalidad.cargarGUI(item.empresa);

            if (guimodalidad.ShowDialog().Value)
            {
                frmSearchEstudio gui = new frmSearchEstudio();
                gui.ampliacion = "0";
                gui.modalidad = guimodalidad.modalidad;
                gui.sucursal = ((item.empresa * 100) + item.sede).ToString();
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
                            item.isFacGlobal = true;
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
    }
}
