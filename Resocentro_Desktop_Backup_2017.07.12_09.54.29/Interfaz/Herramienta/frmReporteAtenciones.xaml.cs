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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.Herramienta
{
    /// <summary>
    /// Lógica de interacción para frmListaAtenciones.xaml
    /// </summary>
    public partial class frmReporteAtenciones : Window
    {
        MySession session;
        public frmReporteAtenciones()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboAnio.ItemsSource = new UtilDAO().getAño(5);
            cboAnio.DisplayMemberPath = "nombre";
            cboAnio.SelectedValuePath = "codigo";
            cboAnio.SelectedValue = DateTime.Now.Year;

            var empresas = new UtilDAO().getEmpresa();
            cbosucursal.ItemsSource = empresas;
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;

            cbosucursal1.ItemsSource = empresas;
            cbosucursal1.SelectedValuePath = "codigounidad";
            cbosucursal1.DisplayMemberPath = "nombre";
            cbosucursal1.SelectedIndex = 0;

            cbosucursaldoc.ItemsSource = empresas;
            cbosucursaldoc.SelectedValuePath = "codigounidad";
            cbosucursaldoc.DisplayMemberPath = "nombre";
            cbosucursaldoc.SelectedIndex = 0;

            cbosucursaldocFac.ItemsSource = empresas;
            cbosucursaldocFac.SelectedValuePath = "codigounidad";
            cbosucursaldocFac.DisplayMemberPath = "nombre";
            cbosucursaldocFac.SelectedIndex = 0;

            cbosucursalCortesia.ItemsSource = empresas;
            cbosucursalCortesia.SelectedValuePath = "codigounidad";
            cbosucursalCortesia.DisplayMemberPath = "nombre";
            cbosucursalCortesia.SelectedIndex = 0;

            if (session.roles.Contains("60"))
                radtab2.Visibility = Visibility.Visible;
            else
                radtab2.Visibility = Visibility.Collapsed;
        }

        private async void BtnListaMensual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gridResultado.IsBusy = true;
                gridResultado.ItemsSource = await new AdministracionDAO().getReportAtenciones(1, cboMes.SelectedIndex, int.Parse(cboAnio.SelectedValue.ToString()), int.Parse(cbosucursal.SelectedValue.ToString()), DateTime.Now, DateTime.Now);
                gridResultado.IsBusy = false;
                gridResultado.CalculateAggregates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridResultado.IsBusy = false;
            }

        }

        private async void rdpFechaRango_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridResultado.IsBusy = true;
                    gridResultado.ItemsSource = await new AdministracionDAO().getReportAtenciones(2, 1, 1, int.Parse(cbosucursal.SelectedValue.ToString()), rdpInicio.SelectedDate.Value, rdpFin.SelectedDate.Value);
                    gridResultado.IsBusy = false;
                    gridResultado.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridResultado.IsBusy = false;
            }
        }

        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadMenuItem item = (RadMenuItem)sender;
                if (item.CommandParameter.ToString() == "atencion")
                    new Tool().exportGrid(gridResultado, true, false, false);
                else if (item.CommandParameter.ToString() == "documento")
                    new Tool().exportGrid(gridDocumentos, true, false, false);
                else { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItemHistoria_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                var item = (ReporteAtencionesMensuales)this.gridResultado.SelectedItem;
                if (item != null)
                {
                    frmHistoriaPaciente gui = new frmHistoriaPaciente();
                    gui.cargarGUI(session);
                    gui.Show();
                    await gui.buscarPaciente(item.codigo.ToString(), 1);
                }
                else
                {
                    var item2 = (ReporteAtencionesMensuales)this.gridResultado.SelectedItem;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void rdpFechaRangodoc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentos(int.Parse(cbosucursaldoc.SelectedValue.ToString()), rdpIniciodoc.SelectedDate.Value, rdpFindoc.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async void rdpFechaRangodocFAC_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentosFactura(int.Parse(cbosucursaldocFac.SelectedValue.ToString()), rdpIniciodocFAC.SelectedDate.Value, rdpFindocFAC.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async void rdpFechaRangoCortesia_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentosCortesias(int.Parse(cbosucursalCortesia.SelectedValue.ToString()), rdpInicioCortesia.SelectedDate.Value, rdpFinCortesia.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async  void gridResultado_LoadingRowDetails(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            ReporteAtencionesMensuales item = (ReporteAtencionesMensuales)e.Row.DataContext;
            if (!item.dataLoaded)
            {
                try
                {
                   await Task.Run(() =>
                    {
                        var dao = new AdministracionDAO();
                        item.datos_paciente = new PacienteDAO().getPaciente(item.codigopaciente);
                        item.datos_cita = dao.getInfoCita(item.numerocita);
                        item.datos_carta = dao.getInfoCarta(item.numerocita);
                        item.datos_admision = dao.getInfoAdmision(item.numerocita);
                        item.datos_adquisicion = dao.getInfoAdquisicion(item.codigo);
                        item.datos_documentos = dao.getInfoDocumento(item.codigopaciente.ToString(),item.unidad.ToString(),item.numeroatencion.ToString());
                        item.datos_adquisicion.estudio = item.codigo + " - " + item.estudio;
                        item.dataLoaded = true;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
        }


    }
}
