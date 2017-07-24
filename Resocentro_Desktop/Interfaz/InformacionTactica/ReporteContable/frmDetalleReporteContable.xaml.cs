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

namespace Resocentro_Desktop.Interfaz.InformacionTactica
{
    /// <summary>
    /// Lógica de interacción para frmDetalleReporteContable.xaml
    /// </summary>
    public partial class frmDetalleReporteContable : Window
    {
        public frmDetalleReporteContable()
        {
            InitializeComponent();
        }

        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_Detalle_fac, true, false, false);
        }
    }
}
