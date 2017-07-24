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

namespace Resocentro_Desktop.Interfaz.Cartas.Print
{
    /// <summary>
    /// Lógica de interacción para frmPrintProforma.xaml
    /// </summary>
    public partial class frmPrintProforma : Window
    {
        public frmPrintProforma()
        {
            InitializeComponent();
        }
        public void cargarGUI(List<Search_Estudio> detalle, string paciente, string aseguradora, string medico, string clinica, string empresa, string observaciones, double contraste, double sedacion)
        {
            grid_items.ItemsSource = detalle;
            lblfecharecepcion.Content = DateTime.Now.ToLongDateString();
            lblpaciente.Content = paciente.ToUpper();
            lblaseguradora.Content = aseguradora.ToUpper();
            lblmedico.Content = medico.ToUpper();
            lblclinica.Text = clinica.ToUpper();
            lblempresa.Content = empresa.ToUpper();
            lblcontraste.Content = "CONTRASTE      " + contraste.ToString("#,###,###0.#0").PadLeft(10, ' ');
            lblsedacion.Content =  "SEDACION       " + sedacion.ToString("#,###,###0.#0").PadLeft(10, ' ');

            this.Show();
            ProcessUITasks();
            PrintDialog print = new PrintDialog();
            
            // Display the dialog. This returns true if the user presses the Print button.
            if (print.ShowDialog().Value)
            {
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





                print.PrintVisual(grid, "PROFORMA");
                /*grid.LayoutTransform = oldTransform;
                  grid.Measure(oldSize);
            
                  ((UIElement)grid).Arrange(new Rect(new Point(0, 0),oldSize));*/


                this.Close();
            }



          
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
      
    }
}
