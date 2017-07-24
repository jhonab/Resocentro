using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using System.Windows.Threading;

namespace Resocentro_Desktop.Interfaz.Cobranza
{
    /// <summary>
    /// Lógica de interacción para frmTicketComprobante.xaml
    /// </summary>
    public class listTicket
    {
        public string descripcion { get; set; }
        public string valor { get; set; }
    }
    public partial class frmTicketComprobante : Window
    {
        MySession session;
        DocumentoSunat item;
        public frmTicketComprobante()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, DocumentoSunat item)
        {
            this.session = session;
            this.item = item;
        }
        public void printTicket()
        {
            setValues();
            /**/
        }

        private void setValues()
        {
            if (item.empresa == 2)
                imglogo.Source = new BitmapImage(new Uri("pack://application:,,,/Resocentro_Desktop;component/img/logo_emetac.png", UriKind.Absolute));
            lblrazonsocial.Content = item.razonSocialEmisor;
            lblruc.Content = item.rucEmisor;
            txtdireccion.Text = item.direccionsede;
            lbltipodocumento.Text = item.nombretipoDocumento;
            lblnumero_documento.Content = item.numeroDocumentoSUNAT;
            lblruc_emisor.Content = item.documentoReceptorString;
            lblnumeroatencion.Text = "N° ATENCIÓN: " + item.numeroatencion;
            lblcliente.Text = "CLIENTE: " + item.razonSocialReceptor;
            lblpaciente.Text = "PACIENTE: " + item.paciente;
            lblcia.Text = "CIA.: " + item.aseguradora;
            if (item.titular_Carta == null)

                lbltitular.Visibility = Visibility.Collapsed;
            else
                lbltitular.Content = "TITULAR: " + item.titular_Carta;

            if (item.contratante_carta == null)

                lblempresa.Visibility = Visibility.Collapsed;
            else
                lblempresa.Content = "EMPRESA: " + item.contratante_carta;

            if (item.poliza_carta == null)
                lblpoliza.Visibility = Visibility.Collapsed;
            else
                lblpoliza.Content = "N° POLIZA: " + item.poliza_carta;

            if (item.infoCarta == null)

                lblcarta.Visibility = Visibility.Collapsed;
            else
                lblcarta.Content = "CARTA: " + item.infoCarta;


            lblfecha_emision.Content = "FECHA DE EMISIÓN: " + item.fechaEmision;
            lbltipomoneda.Content = "MONEDA: " + item.tipoMonedaImpresion;
            lbligv.Content = (item.igvPorcentaje * 100).ToString();
            var _isGratuita = false;
            List<listTicket> lista = new List<listTicket>();
            foreach (var d in item.detalleItems)
            {
                if (d.isGratuita)
                {
                    _isGratuita = true;
                    lista.Add(new listTicket { descripcion = d.descripcion + "\n" + d.cantidad.ToString() + "\t" + d.valorUnitario + "\nSERVICIO PRESTADO GRATUITAMENTE", valor = d.valorReferencialIGV.ToString("#,###,###0.#0") });
                }
                else
                    lista.Add(new listTicket { descripcion = d.descripcion + "\n" + d.cantidad.ToString() + "\t" + d.valorUnitario + "\n", valor = d.valorVenta/*valorventaImpresion*/.ToString("#,###,###0.#0") });

                if (d.isCortesia)
                    lista.Add(new listTicket { descripcion = ("DESCUENTO " + d.porcentajedescxItem.ToString("##0.#0") + "% (-" + d.descxItem.ToString("#,###,###0.#0")) + ")", valor = "" });
                if (d.porcentajeDescPromocion > 0)
                    lista.Add(new listTicket { descripcion = ("DESCUENTO X PROMOCION " + d.porcentajeDescPromocion.ToString("##0.#0") + "% (-" + d.descPromocion.ToString("#,###,###0.#0")) + ")", valor = "" });

            }

            grid_items.ItemsSource = lista;
            if (item.isOpeExoneradas)
            {
                lblexonerada.Visibility = Visibility.Visible;
               /* if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblexonerada.Content = "OP. EXONERADA      " + (Math.Round(item.totalVenta_OpeExoneradas, 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else*/
                    lblexonerada.Content = "OP. EXONERADA      " + item.totalVenta_OpeExoneradas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.isOpeInafectas)
            {
                lblinafecta.Visibility = Visibility.Visible;
               /* if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblinafecta.Content = "OP. INAFECTA      " + (Math.Round(item.totalVenta_OpeInafectas , 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else*/
                    lblinafecta.Content = "OP .INAFECTA      " + item.totalVenta_OpeInafectas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.isOpeGravadas)
            {
                lblgravada.Visibility = Visibility.Visible;
                /*if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblgravada.Content = "OP. GRAVADA      " + (Math.Round(item.totalVenta_OpeGravadas , 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else*/
                    lblgravada.Content = "OP. GRAVADA      " + item.totalVenta_OpeGravadas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.porcentajeDescuentoGlobal > 0)
            {
                lbldescuento.Visibility = Visibility.Visible;
                /*if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)

                    lbldescuento.Content = "DESC. " + item.porcentajeDescuentoGlobal + "%      " + ((Math.Round(item.descuentoGlobal /** (1 + item.igvPorcentaje)*//*, 2) * -1).ToString("#,###,###0.#0")).PadLeft(10, ' ');
                else*/
                    lbldescuento.Content = "DESC. " + item.porcentajeDescuentoGlobal + "%      " + ((item.descuentoGlobal * -1).ToString("#,###,###0.#0")).PadLeft(10, ' ');
            }

           /* if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                lbligv.Content = "";
                lbligv.Visibility = Visibility.Collapsed;
            }
            else
            {*/
                lbligv.Content = "IGV.      " + item.igvTotal.ToString("#,###,###0.#0").PadLeft(10, ' ');
            //}
            lbltotal.Content = "TOTAL.      " + item.ventaTotal.ToString("#,###,###0.#0").PadLeft(10, ' ');

            txttotal_texto.Text = "SON: " + item.totalVentaTexto + " " + item.tipoMonedaImpresion;
            lblcajero.Content = "CAJERO: " + session.shortuser;


            //imgbarcode.Source = new BitmapImage(new Uri(item.pathCODEBAR, UriKind.Relative)); ;
            rad_barcode.Text = String.Empty;
            rad_barcode.Mode = QRClassLibrary.Modes.CodeMode.Alphanumeric;
            rad_barcode.Text = item.codigoBarraPDF417String;
            rad_barcode.ErrorCorrectionLevel = QRClassLibrary.Modes.ErrorCorrectionLevel.Q;
            //rad_barcode.Version = 1;

            string _retencion = "OPERACION SUJETA AL SISTEMA DE PAGO DE OBLIGACIONES TRIBUTARIAS CON EL GOBIERNO CENTRAL\nCTA. CTE. N° {0} BANCO DE LA NACIÓN";
            if (item.empresa == 1)
            {
                _retencion = string.Format(_retencion, "00000 - 783773");
                txtretencionrreso.Visibility = Visibility.Visible;
            }
            else
            {
                _retencion = string.Format(_retencion, "00000 - 564656");
                txtretencionrreso.Visibility = Visibility.Collapsed;
            }

            txtretencion.Text = _retencion;
            if (item.ventaTotal > 700)
                txtretencion.Visibility = Visibility.Visible;
            else
                txtretencion.Visibility = Visibility.Collapsed;
            print();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /* PrintDialog pd = new PrintDialog();
             PrintQueue printer = new PrintQueue(new PrintServer(), @"TICKET-CAJA");
             pd.PrintQueue = printer; 
             pd.PrintVisual(grid, "TICKET-CAJA");*/





        }
        public static void ProcessUITasks()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);
            Dispatcher.PushFrame(frame);
        }
        private void print()
        {
            this.Show();
            ProcessUITasks();
            PrintDialog print = new PrintDialog();
            PrintQueue printer = new PrintQueue(new PrintServer(), @"TICKET-CAJA");
            //PrintQueue printer = new PrintQueue(new PrintServer(), @"SIS2013");
            print.PrintQueue = printer;


            System.Printing.PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);

            //get scale of the print wrt to screen of WPF visual
            double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / grid.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                           grid.ActualHeight);

            //Transform the Visual to scale
            this.LayoutTransform = new ScaleTransform(scale, scale);

            //get the size of the printer page
            Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            //update the layout of the visual to the printer page size.
            grid.Measure(sz);
            grid.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));





            print.PrintVisual(grid, item.nombretipoDocumento + " " + item.numeroDocumento);
            /*grid.LayoutTransform = oldTransform;
              grid.Measure(oldSize);
            
              ((UIElement)grid).Arrange(new Rect(new Point(0, 0),oldSize));*/


            this.Close();
        }
    }
}
