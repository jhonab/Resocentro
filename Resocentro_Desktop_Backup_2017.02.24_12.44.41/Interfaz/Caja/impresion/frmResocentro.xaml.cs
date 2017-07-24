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
    /// Lógica de interacción para frmResocentro.xaml
    /// </summary>
    public  class listDetalleDocumento
    {
        public string descripcion { get; set; }
        public string valor { get; set; }
    }
    public  partial class frmResocentro : Window
    {
        MySession session;
        DocumentoSunat item;
        string informacion;
        public frmResocentro()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, DocumentoSunat item, string informacion)
        {
            this.session = session;
            this.item = item;
            this.informacion = informacion;
        }
        public void printDocumento()
        {
            setValues();
            /**/
        }
        public  void printTicket()
        {
            setValues();
            /**/
        }
        private void setValues()
        {
            txtdireccion.Text = item.direccionReceptor;
            txtdocumento.Text = item.numeroDocumento;
            txtdni.Text = item.documentoReceptorString;
            txtreceptor.Text = item.razonSocialReceptor;
            txtfecha.Text = item.fechaEmision.ToLongDateString();
            txtigv_porcentaje.Text = "IGV " + (item.igvPorcentaje * 100).ToString() + "%";
            var _tipocobranza = TIPO_COBRANZA.PACIENTE;
            var _isgratuita = false;
            List<listDetalleDocumento> lista = new List<listDetalleDocumento>();
            foreach (var d in item.detalleItems)
            {
                _tipocobranza = (TIPO_COBRANZA)d.tipo_cobranza;
                _isgratuita = d.isGratuita;
                lista.Add(new listDetalleDocumento { descripcion = "" + d.cantidad + "  " + d.descripcion.ToUpper(), valor = d.valorventaImpresion.ToString("#,###,###0.#0") });
            }
            double monto = 0;
            if (_tipocobranza == TIPO_COBRANZA.PACIENTE)
            {
                monto = item.detalleItems.Sum(x => x.descxCobSeguro);
                
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    monto = Math.Round(monto * (item.igvPorcentaje + 1), 2);
                if (monto > 0)
                    lista.Add(new listDetalleDocumento { descripcion = "\nDESCUENTO DE COMPAÑIA DE SEGURO ( " + monto.ToString("#,###,###0.#0") + " )", valor = "" });
            }
            else
            {
                monto = item.detalleItems.Sum(x => x.descxCobPaciente);
                if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                    monto = Math.Round(monto * (item.igvPorcentaje + 1), 2);
                if (monto > 0)
                    lista.Add(new listDetalleDocumento { descripcion = "\nCOBRO DE COASEGURO( " + item.simboloMonedaImpresion+" " + monto.ToString("#,###,###0.#0") + " )", valor = "" });
            }

            lista.Add(new listDetalleDocumento { descripcion = "\n" + informacion, valor = "" });

            if (_isgratuita)
                txtcortesia.Visibility = Visibility.Visible;
            else
                txtcortesia.Visibility = Visibility.Collapsed;

            grid_items.ItemsSource = null;
            grid_items.ItemsSource = lista;

            if (item.tipoDocumento == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
            {
                txtsubtotal.Text = (item.simboloMonedaImpresion + (Math.Round(item.subTotal + item.igvTotal, 2)).ToString("#,###,###0.#0")).PadLeft(10, ' ');
                txtigv.Text = "";
                txtigv_porcentaje.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtsubtotal.Text = (item.simboloMonedaImpresion + item.subTotal.ToString("#,###,###0.#0")).PadLeft(10, ' ');
                txtigv.Text = (item.simboloMonedaImpresion + item.igvTotal.ToString("#,###,###0.#0")).PadLeft(10, ' ');

            }
            txttotal.Text = (item.simboloMonedaImpresion + item.ventaTotal.ToString("#,###,###0.#0")).PadLeft(10, ' ');
            txtmontosoles.Text = "SON: " + item.totalVentaTexto + " " + item.tipoMonedaImpresion;
            txtcajero.Text = "USUARIO: " + session.shortuser;

            if (item.ventaTotal > 700)
                txtretencion.Visibility = Visibility.Visible;
            else
                txtretencion.Visibility = Visibility.Collapsed;

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
            this.Show();
            ProcessUITasks();
            PrintDialog print = new PrintDialog();

            if (true)
            {
                PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);

                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / grid.ActualWidth,
                                        capabilities.PageImageableArea.ExtentHeight / grid.ActualHeight);

                Transform oldTransform = grid.LayoutTransform;

                grid.LayoutTransform = new ScaleTransform(scale, scale);

                Size oldSize = new Size(grid.ActualWidth, grid.ActualHeight);
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                grid.Measure(sz);
                ((UIElement)grid).Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
                    sz));

                PrintQueue printer = new PrintQueue(new PrintServer(), @"CAJA-MATRICIAL");
                //PrintQueue printer = new PrintQueue(new PrintServer(), @"SIS2013");
                print.PrintQueue = printer;
                print.PrintVisual(grid, "Print Results");

                grid.LayoutTransform = oldTransform;
                grid.Measure(oldSize);

                ((UIElement)grid).Arrange(new Rect(new Point(0, 0),
                    oldSize));

            }
            this.Close();
        }

    }
}
