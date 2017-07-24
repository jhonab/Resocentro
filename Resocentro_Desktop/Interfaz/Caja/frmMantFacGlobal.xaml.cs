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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmMantFacGlobal.xaml
    /// </summary>
    public partial class frmMantFacGlobal : Window
    {
        MySession session;
        public frmMantFacGlobal()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frmListaFacturacionGlobal gui = new frmListaFacturacionGlobal();
            gui.cargarGUI(session, false,"");
            gui.Show();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            listarfacturas();
        }

        private void listarfacturas()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                gridFacturas.ItemsSource = null;
                gridFacturas.ItemsSource = db.FacturaGlobal.Where(x => x.usuario == session.codigousuario && x.numerodocumento == null).ToList();
            }

        }

        private void MenuItemModificar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            FacturaGlobal item = (FacturaGlobal)this.gridFacturas.SelectedItem;
            if (item != null)
            {
                frmDetalleFacGlobal gui = new frmDetalleFacGlobal();
                gui.Show();
                gui.cargarGUI(item);
            }
        }

        private void MenuItemAgregar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            FacturaGlobal item = (FacturaGlobal)this.gridFacturas.SelectedItem;
            if (item != null)
            {
                frmListaFacturacionGlobal gui = new frmListaFacturacionGlobal();
                gui.Show();
                gui.cargarGUI(session, true,item.datosadicionales, item.idFac);
            }
        }

        private void MenuItemEliminar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            FacturaGlobal item = (FacturaGlobal)this.gridFacturas.SelectedItem;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    if (MessageBox.Show("Está seguro de Eliminar la Pre-Facturacion ", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.DetalleFacturaGlobal.RemoveRange(db.DetalleFacturaGlobal.Where(x => x.idFac == item.idFac).ToList());
                            db.FacturaGlobal.RemoveRange(db.FacturaGlobal.Where(x => x.idFac == item.idFac).ToList());
                            db.SaveChanges();
                            listarfacturas();
                            MessageBox.Show("Se eliminaron los datos exitosamente");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    }
                }
            }
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
                        listarfacturas();
        }



    }
}
