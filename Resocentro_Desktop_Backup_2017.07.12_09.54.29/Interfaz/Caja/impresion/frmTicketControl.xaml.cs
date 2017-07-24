using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Cobranza;
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

namespace Resocentro_Desktop.Interfaz.Caja.impresion
{
    /// <summary>
    /// Lógica de interacción para frmTicketControl.xaml
    /// </summary>
    public partial class frmTicketControl : Window
    {
        MySession session;
        DocumentoSunat item;
        public frmTicketControl()
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
          
            lblrazonsocial.Content = item.razonSocialEmisor;
        
            lbltipodocumento.Text = item.nombretipoDocumento;
            lblnumero_documento.Content = item.numeroDocumentoSUNAT;
            lblruc_emisor.Content = item.documentoReceptorString;
            lblnumeroatencion.Text = "N° ATENCIÓN: " + item.numeroatencion;
            lblcliente.Text = "CLIENTE: " + item.razonSocialReceptor;
            lblpaciente.Text = "PACIENTE: " + item.paciente;
            lblcia.Text = "CIA.: " + item.aseguradora;         
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
                    lista.Add(new listTicket { descripcion = d.descripcion + "\n" + d.cantidad.ToString() + "\t" + d.valorUnitario + "\n", valor = d.valorventaImpresion.ToString("#,###,###0.#0") });

                if (d.isCortesia)
                    lista.Add(new listTicket { descripcion = ("DESCUENTO " + d.porcentajedescxItem.ToString("##0.#0") + "% (-" + d.descxItem.ToString("#,###,###0.#0")) + ")", valor = "" });
                if (d.porcentajeDescPromocion > 0)
                    lista.Add(new listTicket { descripcion = ("DESCUENTO X PROMOCION " + d.porcentajeDescPromocion.ToString("##0.#0") + "% (-" + d.descPromocion.ToString("#,###,###0.#0")) + ")", valor = "" });

            }

            grid_items.ItemsSource = lista;
            if (item.isOpeExoneradas)
            {
                lblexonerada.Visibility = Visibility.Visible;
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblexonerada.Content = "OP. EXONERADA      " + (Math.Round(item.totalVenta_OpeExoneradas * (1 + item.igvPorcentaje), 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else
                    lblexonerada.Content = "OP. EXONERADA      " + item.totalVenta_OpeExoneradas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.isOpeInafectas)
            {
                lblinafecta.Visibility = Visibility.Visible;
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblinafecta.Content = "OP. INAFECTA      " + (Math.Round(item.totalVenta_OpeInafectas * (1 + item.igvPorcentaje), 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else
                    lblinafecta.Content = "OP .INAFECTA      " + item.totalVenta_OpeInafectas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.isOpeGravadas)
            {
                lblgravada.Visibility = Visibility.Visible;
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)
                    lblgravada.Content = "OP. GRAVADA      " + (Math.Round(item.totalVenta_OpeGravadas * (1 + item.igvPorcentaje), 2)).ToString("#,###,###0.#0").PadLeft(10, ' ');
                else
                    lblgravada.Content = "OP. GRAVADA      " + item.totalVenta_OpeGravadas.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            if (item.porcentajeDescuentoGlobal > 0)
            {
                lbldescuento.Visibility = Visibility.Visible;
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA && !_isGratuita)

                    lbldescuento.Content = "DESC. " + item.porcentajeDescuentoGlobal + "%      " + ((Math.Round(item.descuentoGlobal * (1 + item.igvPorcentaje), 2) * -1).ToString("#,###,###0.#0")).PadLeft(10, ' ');
                else
                    lbldescuento.Content = "DESC. " + item.porcentajeDescuentoGlobal + "%      " + ((item.descuentoGlobal * -1).ToString("#,###,###0.#0")).PadLeft(10, ' ');
            }

            if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                lbligv.Content = "";
                lbligv.Visibility = Visibility.Collapsed;
            }
            else
            {
                lbligv.Content = "IGV.      " + item.igvTotal.ToString("#,###,###0.#0").PadLeft(10, ' ');
            }
            lbltotal.Content = "TOTAL.      " + item.ventaTotal.ToString("#,###,###0.#0").PadLeft(10, ' ');
         
            lblcajero.Content = "CAJERO: " + session.shortuser;


           
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

