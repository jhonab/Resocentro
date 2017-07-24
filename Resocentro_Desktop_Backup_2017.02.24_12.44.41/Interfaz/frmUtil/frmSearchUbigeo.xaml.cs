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

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmSearchUbigeo.xaml
    /// </summary>
    public partial class frmSearchUbigeo : Window
    {
        DocumentoSunat item;
        string tipo;
        public frmSearchUbigeo()
        {
            InitializeComponent();
            
        }

        private void grid_Cartas_FilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }

        public void cargarGUI(DocumentoSunat item,string tipo,string filtro)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                gridUbigeo.ItemsSource = db.UBIGEO.ToList();
                this.item = item;
                this.tipo = tipo;
                //DataContext = new { filtro = filtro };
            }
        }

        private void gridUbigeo_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
           
            DialogResult = true;
        }
    }
}
