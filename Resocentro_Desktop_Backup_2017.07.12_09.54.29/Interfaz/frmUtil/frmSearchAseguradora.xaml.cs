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
    /// Lógica de interacción para frmSearchAseguradora.xaml
    /// </summary>
    public partial class frmSearchAseguradora : Window
    {
        public COMPANIASEGURO aseguradora;
        public frmSearchAseguradora()
        {
            InitializeComponent();
            txtaseguradora.Focus();
        }
        public void setGUI(string filtro)
        {
            if (filtro != "")
            {
                txtaseguradora.Text = filtro; 
                buscarAseguradoras();
            }
        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            buscarAseguradoras();
        }

        private void buscarAseguradoras()
        {
            var nombre = txtaseguradora.Text.Trim();
            if (nombre != "")
               gridAseguradora.ItemsSource= new UtilDAO().getAseguradora(nombre);
            gridAseguradora.Focus();
        }

        private void txtaseguradora_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscarAseguradoras();
            }
        }

        private void gridAseguradora_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
             COMPANIASEGURO item = (COMPANIASEGURO)e.Row.DataContext;
             if (item != null)
             {
                 aseguradora = item;
                 DialogResult = true;
             }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

       
    }
}
