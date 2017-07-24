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

namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmSearchBeneficio.xaml
    /// </summary>
    public partial class frmSearchBeneficio : Window
    {
        public Sunasa_Cobertura beneficio;
        public int compania = 0;
        public int producto = 0;
        public frmSearchBeneficio()
        {
            InitializeComponent();
        }
        private void buscarBeneficio()
        {
            if (txtnombre.Text != "")
            {
                var lista = new CartaDAO().BuscarBeneficio(txtnombre.Text, compania, producto);
                if (lista.Count > 0)
                    grid_medicos.ItemsSource = lista;
                else
                    MessageBox.Show("No se encontro ningun registro con los parametros ingresador", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void grid_medicos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Sunasa_Cobertura item = (Sunasa_Cobertura)e.Row.DataContext;
            beneficio = item;
            DialogResult = true;
        }

        private void txtnombre_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                buscarBeneficio();
        }

        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            buscarBeneficio();
        }
    }
}
