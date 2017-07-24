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
    public partial class frmSearchEstudio : Window
    {
        public string ampliacion, modalidad, sucursal;
        public bool iscaja { get; set; }
        public Search_Estudio estudio;
        public frmSearchEstudio()
        {
            InitializeComponent();
        }
        public void getClase()
        {
            var lista =new EstudiosDAO().getClase();
            if (iscaja)
                lista = lista.Where(x => x.codigoclase == 0).ToList();
            grid_Clase.ItemsSource = lista;
        }

        private void grid_Clase_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            CLASE _item = (CLASE)e.Row.DataContext;
            grid_Estudio.ItemsSource = null;
            grid_Estudio.ItemsSource = new EstudiosDAO().getListEstudio(_item.codigoclase.ToString(),ampliacion,modalidad,sucursal);
        }

        private void grid_Estudio_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Search_Estudio _item = (Search_Estudio)e.Row.DataContext;
            estudio = _item;
            DialogResult = true;
        }

        private void grid_Clase_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                grid_Estudio.Focus();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        
    }
}
