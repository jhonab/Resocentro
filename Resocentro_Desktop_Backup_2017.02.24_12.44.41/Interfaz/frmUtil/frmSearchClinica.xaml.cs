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
    /// Lógica de interacción para frmSearchClinica.xaml
    /// </summary>
    public partial class frmSearchClinica : Window
    {
        public CLINICAHOSPITAL clinica;
        public frmSearchClinica()
        {
            InitializeComponent();
            txtclinica.Focus();
        }
        public void setGUI(string filtro)
        {
            if (filtro != "")
            {
                txtclinica.Text = filtro;
                buscarClinica();
            }
        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            buscarClinica();
        }

        private void buscarClinica()
        {
            var nombre = txtclinica.Text.Trim();
            if (nombre != "")
                gridClinica.ItemsSource = new UtilDAO().getClinica(nombre);
            gridClinica.Focus();
        }
        private void txtclinica_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscarClinica();
            }
        }
        private void gridClinica_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            CLINICAHOSPITAL item = (CLINICAHOSPITAL)e.Row.DataContext;
            if (item != null)
            {
                clinica = item;
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
