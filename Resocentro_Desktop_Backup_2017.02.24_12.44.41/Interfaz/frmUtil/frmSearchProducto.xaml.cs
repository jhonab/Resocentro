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

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmSearchProducto.xaml
    /// </summary>
    public partial class frmSearchProducto : Window
    {
        public SitedProducto producto;
        public int compania;
        public frmSearchProducto()
        {
            InitializeComponent();
        }

        private void BuscarProducto()
        {
            if (txtnombre.Text != "" || txtsited.Text != "")
            {
                var lista = new CartaDAO().BuscarProducto(txtnombre.Text, txtsited.Text, compania);
                if (lista.Count > 0)
                    grid_medicos.ItemsSource = lista;
                else
                    MessageBox.Show("No se encontro ningun registro con los parametros ingresador", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void grid_medicos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {

            SitedProducto item = (SitedProducto)e.Row.DataContext;
            producto = item;
            DialogResult = true;
        }

        private void txtnombre_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BuscarProducto();
        }



        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            BuscarProducto();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
