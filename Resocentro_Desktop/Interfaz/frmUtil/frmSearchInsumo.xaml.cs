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
    /// Lógica de interacción para frmSearchEstudio.xaml
    /// </summary>
    public partial class frmSearchInsumo : Window
    {
        public string ampliacion, modalidad, sucursal;
        public Search_Estudio estudio;
        public frmSearchInsumo()
        {
            InitializeComponent();
        }

        private void buscarInsumos()
        {
            grid_Estudio.ItemsSource = null;
            grid_Estudio.ItemsSource = new EstudiosDAO().getListInsumoTecnica(txtfiltro.Text, ampliacion, modalidad, sucursal);
        }
      

        private void grid_Estudio_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Search_Estudio _item = (Search_Estudio)e.Row.DataContext;
            estudio = _item;
            DialogResult = true;
        }

       

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void txtfiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtfiltro.Text != "")
                    buscarInsumos();
            }
        }
    }
}
