using Resocentro_Desktop.DAO;
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

namespace Resocentro_Desktop.Interfaz.Caja.impresion
{
    /// <summary>
    /// Lógica de interacción para frmReciboProvisional.xaml
    /// </summary>
    public partial class frmReciboProvisionalprint : Window
    {
        MySession session;
        ReciboProvisional item;
        Recibo_Provisional data;
        int cantidad = 0;
        public frmReciboProvisionalprint()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, ReciboProvisional item, Recibo_Provisional data)
        {
            this.session = session;
            this.item = item;
            this.data = data;
        }
        public void printTicket(int copias)
        {
            cantidad = copias;
            setValues();
            /**/
        }

        private void setValues()
        {
            if (item.empresa == 2)
            {
                lblrazonsocial.Content = "EMETAC SAC";
                imglogo.Source = new BitmapImage(new Uri("pack://application:,,,/Resocentro_Desktop;component/img/logo_emetac.png", UriKind.Absolute));
            }

            lbltipodocumento.Text = "RECIBO PROVISIONAL";
            lblnumero_documento.Content = item.idRecibo;
            lblnumeroatencion.Text = "N° CITA: " + item.numerocita;
            lblcliente.Text = "CLIENTE: " + data.paciente;
            lblpaciente.Text = "PACIENTE: " + data.paciente;
            lblfecha_emision.Content = "FECHA DE EMISIÓN: " + item.fecha_reg;
            lbltipomoneda.Content = "MONEDA: SOLES";

            grid_items.ItemsSource = data.estudio;

            lbltotalentregado.Content = "MONTO TOTAL      " + item.monto_total.ToString("#,###,###0.#0").PadLeft(10, ' ');
            //}
            lbltotal.Content = "TOTAL ENTREGADO      " + item.monto_recibido.ToString("#,###,###0.#0").PadLeft(10, ' ');


            lblcajero.Content = "CAJERO: " + session.shortuser;


            //imgbarcode.Source = new BitmapImage(new Uri(item.pathCODEBAR, UriKind.Relative)); ;
            print();
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
            try
            {
                this.Show();
                ProcessUITasks();
                PrintDialog print = new PrintDialog();
                PrintQueue printer = new PrintQueue(new PrintServer(), @"TICKET-CAJA");
                print.PrintQueue = printer;
                System.Printing.PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / grid.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               grid.ActualHeight);
                this.LayoutTransform = new ScaleTransform(scale, scale);
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                grid.Measure(sz);
                grid.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
                for (int i = 0; i < cantidad; i++)
                {
                    print.PrintVisual(grid, "RECIBO PROVISIONAL " + item.idRecibo);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Close();
        }
    }
}
