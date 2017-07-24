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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmSearchCompania.xaml
    /// </summary>
    public partial class frmSearchCompania : Window
    {
        MySession session;
        public EmpresaFacturacion empresa { get; set; }
        public frmSearchCompania()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            txtruc.Focus();
        }

        private void txtruc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                buscarEmpresas(txtruc.Text.Trim());
        }

        private void buscarEmpresas(string empresa)
        {
            gridEmpresas.ItemsSource = null;
            gridEmpresas.ItemsSource = new CobranzaDAO().buscarEmpresas(empresa);
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {

            frmCompania gui = new frmCompania();
            gui.ShowDialog();
        }

        private void gridEmpresas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            EmpresaFacturacion item = (EmpresaFacturacion)e.Row.DataContext;
            if (item != null)
            {
                empresa = item;
                DialogResult = true;

            }
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridEmpresas.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
                EmpresaFacturacion item = (EmpresaFacturacion)this.gridEmpresas.SelectedItem;
                if (item == null)
                {
                    MenuItemModificarAseguradora.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MenuItemModificarAseguradora.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void MenuItemModificarAseguradora_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            EmpresaFacturacion item = (EmpresaFacturacion)this.gridEmpresas.SelectedItem;
            if (item != null)
            {
                try
                {

                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var data = db.ASEGURADORA.SingleOrDefault(x => x.ruc == item.ruc);
                        if (data != null)
                        {
                            frmCompania gui = new frmCompania();
                            gui.setAseguradora(data);
                            gui.ShowDialog();
                            buscarEmpresas(txtruc.Text.Trim());
                        }

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
