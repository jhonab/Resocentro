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
    /// Lógica de interacción para frmRepoteNoFacturado.xaml
    /// </summary>
    public partial class frmRepoteNoFacturado : Window
    {
        List<FiltroAseguradora> lista;
        public frmRepoteNoFacturado()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;
            dtpInicio.SelectedDate = DateTime.Now;
            dtpFin.SelectedDate = DateTime.Now;
            getAseguradoras();
        }

        private void getAseguradoras()
        {
            lista = new CobranzaDAO().getAseguradoras();
            gridAseguradoras.ItemsSource = null;
            gridAseguradoras.ItemsSource = lista;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            List<FiltroAseguradora> lstaAseguradora = (List<FiltroAseguradora>)gridAseguradoras.Items.SourceCollection;
            string aseguradoras = String.Join(",", lstaAseguradora.Where(x => x.isSeleccionado).Select(x => x.ruc).ToArray());
            if (aseguradoras != "")
            {
                listarAtenciones(aseguradoras);

            }
            else
                MessageBox.Show("Seleccione una Empresa");
        }

        private async void listarAtenciones(string aseguradoras)
        {
            try
            {
                gridAtenciones.ItemsSource = null;
                gridAtenciones.ItemsSource = await new CobranzaDAO().getListaNoFacturadas(cbosucursal.SelectedValue.ToString(), dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), aseguradoras);
                MessageBox.Show("Terminado");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void gridAseguradoras_FilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }

        private void gridAseguradoras_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            FiltroAseguradora item = (FiltroAseguradora)e.Row.DataContext;
            item.isSeleccionado = !item.isSeleccionado;
        }


    }
}
