using Microsoft.Reporting.WinForms;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Resocentro_Desktop.Interfaz.Caja.impresion
{
    /// <summary>
    /// Lógica de interacción para frmCaratula.xaml
    /// </summary>
    public partial class frmCaratula : Window
    {
        public frmCaratula()
        {
            InitializeComponent();
        }
        public void cargarGUI(CaratulaEntity item)
        {
            this.Show();
             ReportDataSource ds = new ReportDataSource("DataSet1", item.detalle);
                ReportParameter rp1 = new ReportParameter("fecha", "Miraflores, "+DateTime.Now.ToLongDateString());
                ReportParameter rp2 = new ReportParameter("empresa", item.empresa);
                ReportParameter rp3 = new ReportParameter("lote", item.lote.ToString());
                _reportViewer.LocalReport.ReportEmbeddedResource = "Resocentro_Desktop.Interfaz.Caja.impresion.Caratula.rdlc";
                _reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3 });
                _reportViewer.LocalReport.DataSources.Add(ds);
                _reportViewer.RefreshReport();


            //grid_items.ItemsSource = item.detalle;
            //lbllote.Content = "Lote: " + item.lote;
            //txtfecha.Text = "Miraflores, " + DateTime.Now.ToLongDateString();
            //txtremitente.Text = "Señores\n" + item.empresa.ToUpper();
            //txttotal.Text = item.detalle.Sum(x => x.total).ToString("#,###,###0.#0");

            //this.Show();
            //ProcessUITasks();
            //PrintDialog print = new PrintDialog();

            //// Display the dialog. This returns true if the user presses the Print button.
            //if (print.ShowDialog().Value)
            //{
            //    System.Printing.PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);

            //    //get scale of the print wrt to screen of WPF visual
            //    double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / grid.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
            //                   grid.ActualHeight);

            //    //Transform the Visual to scale
            //    grid.LayoutTransform = new ScaleTransform(scale, scale);

            //    //get the size of the printer page
            //    Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

            //    //update the layout of the visual to the printer page size.
            //    grid.Measure(sz);
            //    grid.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));


            //    print.PrintVisual(grid, "Caratural");
            //    /*grid.LayoutTransform = oldTransform;
            //      grid.Measure(oldSize);
            
            //      ((UIElement)grid).Arrange(new Rect(new Point(0, 0),oldSize));*/


            //    this.Close();
            //}




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

public class CaratulaEntity
{
    public List<DocumentosRecepcion> detalle { get; set; }
    public string empresa { get; set; }
    public string lote { get; set; }
}