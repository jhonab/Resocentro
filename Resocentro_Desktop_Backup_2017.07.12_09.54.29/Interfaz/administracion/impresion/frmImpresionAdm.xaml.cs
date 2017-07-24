using Microsoft.Reporting.WinForms;
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

namespace Resocentro_Desktop.Interfaz.administracion.impresion
{
    /// <summary>
    /// Lógica de interacción para frmImpresionAdm.xaml
    /// </summary>
    public partial class frmImpresionAdm : Window
    {
        public frmImpresionAdm()
        {
            InitializeComponent();
        }
        public void cargarGUI(List<SedacionesAplicadas> item)
        {
            this.Show();
            ReportDataSource ds = new ReportDataSource("DataSet1", item);
            //ReportParameter rp1 = new ReportParameter("fecha", "Miraflores, " + DateTime.Now.ToLongDateString());
            //ReportParameter rp2 = new ReportParameter("empresa", item.empresa);
            //ReportParameter rp3 = new ReportParameter("lote", item.lote.ToString());
            _reportViewer.LocalReport.ReportEmbeddedResource = "Resocentro_Desktop.Interfaz.administracion.impresion.ReportSedacion.rdlc";
           // _reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3 });
            _reportViewer.LocalReport.DataSources.Add(ds);
            _reportViewer.RefreshReport();

        }
    }
}
