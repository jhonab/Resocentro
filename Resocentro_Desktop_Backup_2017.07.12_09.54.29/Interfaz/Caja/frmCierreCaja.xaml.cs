using Resocentro_Desktop.DAO;
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
using System.Data;
using System.Data.SqlClient;

using Resocentro_Desktop.Entitys;
using Microsoft.Reporting.WinForms;

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmCierreCaja.xaml
    /// </summary>
    public partial class frmCierreCaja : Window
    {
        MySession session;
        public frmCierreCaja()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;

            //cargamos los combos segun usuario
            /*List<SucursalesxUsuario> lista = new List<SucursalesxUsuario>();
            lista.Add(new SucursalesxUsuario { codigoInt=0,nombreShort="TODOS" });
            lista.AddRange();*/
            cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt).ToList();
            cbosede.SelectedValuePath = "codigoInt";
            cbosede.DisplayMemberPath = "nombreShort";
            cbosede.SelectedIndex = 0;
            dtpInicio.SelectedDate = DateTime.Now;
            dtpFin.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {//108
            try
            {
                int unidadSede = int.Parse(cbosede.SelectedValue.ToString());
                int sede = 0, unidad = 0;
                unidad = unidadSede / 100;
                sede = unidadSede - (unidad * 100);

                List<ReporteCierreCaja> dt = new CobranzaDAO().getReporteCaja(dtpInicio.SelectedDate.Value, dtpFin.SelectedValue.Value, txtIni_Fac.Text, txtFin_Fac.Text, txtIni_Bol.Text, txtFin_Bol.Text, unidad, sede, chkrango.IsChecked.Value, txtIni_Cre.Text, txtFin_Cre.Text);
                _reportViewer.Reset();
                ReportDataSource ds = new ReportDataSource("DataSet2", dt);
                ReportParameter rp1 = new ReportParameter("usuario", session.shortuser);
                ReportParameter rp2 = new ReportParameter("sede", cbosede.Text);
                ReportParameter rp3 = new ReportParameter("inicio", dtpInicio.SelectedDate.Value.ToShortDateString());
                ReportParameter rp4 = new ReportParameter("fin", dtpFin.SelectedValue.Value.ToShortDateString());
                _reportViewer.LocalReport.ReportEmbeddedResource = "Resocentro_Desktop.Interfaz.Caja.ReporteCierreCaja.rdlc";
                _reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1, rp2, rp3, rp4 });
                _reportViewer.LocalReport.DataSources.Add(ds);
                _reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtIni_Fac_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)sender).Name == "txtIni_Fac" && txtFin_Fac.Text == "")
                txtFin_Fac.Text = txtIni_Fac.Text;
            if (((TextBox)sender).Name == "txtIni_Bol" && txtFin_Bol.Text == "")
                txtFin_Bol.Text = txtIni_Bol.Text;
            if (((TextBox)sender).Name == "txtIni_Cre" && txtFin_Cre.Text == "")
                txtFin_Cre.Text = txtIni_Cre.Text;
        }

    }
}
