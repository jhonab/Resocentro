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
    /// Lógica de interacción para frmImpAnulacion.xaml
    /// </summary>
    public partial class frmImpAnulacion : Window
    {
        string informacion;
        public frmImpAnulacion()
        {
            InitializeComponent();
        }


        public void cargarGUI( string informacion)
        {
            this.informacion = informacion;
        }
        public void printDocumento()
        {
            setValues();
            /**/
        }
        public void printTicket()
        {
            setValues();
            /**/
        }
        private void setValues()
        {
            txtmotivoanulacion.Text = informacion;
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

                PrintQueue printer = new PrintQueue(new PrintServer(), @"CAJA-MATRICIAL-ANULACION");
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
