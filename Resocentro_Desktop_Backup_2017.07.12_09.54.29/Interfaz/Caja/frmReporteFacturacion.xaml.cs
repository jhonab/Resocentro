using Microsoft.Reporting.WinForms;
using Resocentro_Desktop.DAO;
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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmReporteFacturacion.xaml
    /// </summary>
    public partial class frmReporteFacturacion : Window
    {

        MySession session;
        public frmReporteFacturacion()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int empresa = int.Parse(cbosucursal.SelectedValue.ToString());


                List<ReporteFacturacion> dt = new CobranzaDAO().getReporteFacturacion(cbosucursal.SelectedValue.ToString(), rdtfechainicio.SelectedDate.Value, rdtfechafin.SelectedValue.Value);
                _reportViewer.Reset();
                ReportDataSource ds = new ReportDataSource("DataSetFacturacion", dt);
                ReportParameter rp1 = new ReportParameter("usuario", session.shortuser);
                ReportParameter rp2 = new ReportParameter("empresa", cbosucursal.Text);
                ReportParameter rp3 = new ReportParameter("inicio", rdtfechainicio.SelectedDate.Value.ToShortDateString());
                ReportParameter rp4 = new ReportParameter("fin", rdtfechafin.SelectedValue.Value.ToShortDateString());
                _reportViewer.LocalReport.ReportEmbeddedResource = "Resocentro_Desktop.Interfaz.Caja.ReporteFact.rdlc";
                _reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4 });
                _reportViewer.LocalReport.DataSources.Add(ds);
                _reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
