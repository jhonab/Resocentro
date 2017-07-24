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
    /// Lógica de interacción para frmSearchFacturas.xaml
    /// </summary>
    public partial class frmSearchFacturas : Window
    {
        public FacturaGlobal cabecera { get; set; }
        public frmSearchFacturas()
        {
            InitializeComponent();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                //gridFacturas.ItemsSource = db.FacturaGlobal.Where(x => x.numerodocumento == null).ToList();
                gridFacturas.ItemsSource = new CobranzaDAO().getPreFacturas();
            }
        }

        private void gridFacturas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            cabecera = (FacturaGlobal)e.Row.DataContext;
            DialogResult = true;
        }
    }
}
