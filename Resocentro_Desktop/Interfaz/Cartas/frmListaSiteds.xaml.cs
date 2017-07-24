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
    /// Lógica de interacción para frmListaSiteds.xaml
    /// </summary>
    public partial class frmListaSiteds : Window
    {
        public SitedsResult sitedresult;
        public frmListaSiteds()
        {
            InitializeComponent();
        }
        public void cargarGUI(){
            sitedresult = new SitedsResult();
            gridSiteds.ItemsSource = null;
            gridSiteds.ItemsSource = new CartaDAO().listaAutorizacionesSited();
        }

        private void gridSiteds_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            SitedsResult item = (SitedsResult)e.Row.DataContext;
            if (item != null)
            {
                sitedresult = item;
                if (MessageBox.Show("Se eliminará el registro de su lista de Siteds y no lo volverá a usar hasta que genera uno nuevo desde el Siteds.\n¿Desea continuar?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    new CartaDAO().deleteAutorizacionesSited(item.id);
                }
                DialogResult = true;
                
            }
        }
       
    }
}
