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

namespace Resocentro_Desktop.Interfaz.Caja.impresion
{
    /// <summary>
    /// Lógica de interacción para frmPlanillaCobranza.xaml
    /// </summary>
    public partial class frmPlanillaCobranza : Window
    {
        public frmPlanillaCobranza()
        {
            InitializeComponent();
        }

        public void cargarGUI(List<DocumentosCancelacion>  detalle)
        {
            this.Show();
            ReportDataSource ds = new ReportDataSource("DataSet1", detalle);
            _reportViewer.LocalReport.ReportEmbeddedResource = "Resocentro_Desktop.Interfaz.Caja.impresion.PlanillaCobranza.rdlc";
            _reportViewer.LocalReport.DataSources.Add(ds);
            _reportViewer.RefreshReport();

        }
    }
}
