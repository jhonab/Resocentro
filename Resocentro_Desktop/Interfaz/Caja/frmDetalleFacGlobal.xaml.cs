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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmDetalleFacGlobal.xaml
    /// </summary>
    public partial class frmDetalleFacGlobal : Window
    {
        FacturaGlobal factura;
        public List<PreResumenFacGlobal> lista { get; set; }
        public frmDetalleFacGlobal()
        {
            InitializeComponent();
        }

        public void cargarGUI(FacturaGlobal factura)
        {
            this.factura = factura;
            lista = new List<PreResumenFacGlobal>();
            lblaseguradora.Content = this.factura.aseguradora;
            txtdatosadicionales.Text = this.factura.datosadicionales;
            listardetalle();
        }

        private void listardetalle()
        {
            lista = new List<PreResumenFacGlobal>();
            lista = new CobranzaDAO().getDetalleFacturaGlobal(factura.idFac);
            grid_Atenciones.ItemsSource = null;
            grid_Atenciones.ItemsSource = lista;
            refreshResumen();

        }

        private void MenuItemEliminar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PreResumenFacGlobal item = (PreResumenFacGlobal)this.grid_Atenciones.SelectedItem;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    if (MessageBox.Show("Está seguro de Eliminar el item", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.DetalleFacturaGlobal.RemoveRange(db.DetalleFacturaGlobal.Where(x => x.idDetFac == item.idDetFac).ToList());
                            lista.Remove(item);
                            db.SaveChanges();
                            listardetalle();
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
        private void refreshResumen()
        {
            lblresumen.Content = "Total Items: " + lista.Count().ToString() + "\t\t Monto Total: " + lista.Sum(x => x.preciobruto).ToString();
        }
        private void grid_Atenciones_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {

            refreshResumen();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var fac = db.FacturaGlobal.SingleOrDefault(x => x.idFac == factura.idFac);
                    if (fac != null)
                    {
                        fac.datosadicionales = txtdatosadicionales.Text.ToUpper();
                    }

                    foreach (var d in db.DetalleFacturaGlobal.Where(x => x.idFac == factura.idFac).ToList())
                    {
                        var dd = lista.SingleOrDefault(x => x.idDetFac == d.idDetFac);
                        if (dd != null)
                        {
                            d.precio = Convert.ToDecimal(dd.preciobruto.ToString());
                        }
                    }
                    db.SaveChanges();
                    MessageBox.Show("Se modificaron los datos exitosamente");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            grid_Atenciones.PrintPreview();
        }

        private void RadMenuItemSoles_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PreResumenFacGlobal item = (PreResumenFacGlobal)this.grid_Atenciones.SelectedItem;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    try
                    {
                        foreach (var d in db.DetalleFacturaGlobal.Where(x => x.idDetFac == item.idDetFac).ToList())
                        {
                            d.moneda = 1;
                        }
                        db.SaveChanges();
                        listardetalle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }

        private void RadMenuItemDolares_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PreResumenFacGlobal item = (PreResumenFacGlobal)this.grid_Atenciones.SelectedItem;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    try
                    {
                        foreach (var d in db.DetalleFacturaGlobal.Where(x => x.idDetFac == item.idDetFac).ToList())
                        {
                            d.moneda = 2;
                        }
                        db.SaveChanges();
                        listardetalle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }

        private void MenuContext_Pagos_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                grid_Atenciones.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_Atenciones, true, false, false);
        }

        private void MenuItemAsignar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            frmSetPrecioFacGlobal gui = new frmSetPrecioFacGlobal();
            gui.Owner = this;
            if (gui.ShowDialog().Value)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (PreResumenFacGlobal item in this.grid_Atenciones.SelectedItems.ToList())
                    {
                        foreach (var d in db.DetalleFacturaGlobal.Where(x => x.idDetFac == item.idDetFac).ToList())
                        {
                            d.moneda = gui.moneda;
                            d.precio = Convert.ToDecimal(gui.precio.ToString());
                        }
                        db.SaveChanges();


                    }
                }
                listardetalle();
            }
        }


    }
}
