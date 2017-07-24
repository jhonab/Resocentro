using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.frmUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Telerik.Windows.Data;

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmDocumentosEmitidos.xaml
    /// </summary>

    public partial class frmDocumentosEmitidos : Window
    {

        public MySession session { get; set; }
        public List<SucursalesxUsuario> lstSucursal { get; set; }
        public frmDocumentosEmitidos()
        {
            InitializeComponent();
        }

        private void txtapellidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(txtapellidos.Text, "1"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtnumerodocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(txtnumerodocumento.Text, "0"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void gridDocumento_LoadingRowDetails(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            DocumentosEmitidos item = (DocumentosEmitidos)e.Row.DataContext;
            if (item.listadetalle.Count == 0)
            {
                try
                {
                    item.listadetalle = new CobranzaDAO().getDetalleDocumentosEmitidos(item.numerodocumento, item.codigopaciente.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuInfoPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DocumentosEmitidos item = (DocumentosEmitidos)this.gridDocumento.SelectedItem;
            if (item != null)
            {
                try
                {
                    frmHistoriaPaciente gui = new frmHistoriaPaciente();
                    gui.cargarGUI(session);
                    gui.Show();
                    gui.buscarPaciente(item.codigopaciente.ToString(), 2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            var empresa = new UtilDAO().getEmpresa();
            lstSucursal=  new UtilDAO().getSucursales(session.sucursales);
            cbosucursal.ItemsSource = empresa;
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;

            cbosucursal1.ItemsSource = empresa;
            cbosucursal1.SelectedValuePath = "codigounidad";
            cbosucursal1.DisplayMemberPath = "nombre";
            cbosucursal1.SelectedIndex = 0;

            cbosucursal2.ItemsSource = empresa;
            cbosucursal2.SelectedValuePath = "codigounidad";
            cbosucursal2.DisplayMemberPath = "nombre";
            cbosucursal2.SelectedIndex = 0;

           
        }

        private void MenuInfoDocumento_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DocumentosEmitidos item = (DocumentosEmitidos)this.gridDocumento.SelectedItem;
            if (item != null)
            {
                try
                {
                    frmDocumento.frmDocumento gui = new frmDocumento.frmDocumento();
                    gui.Show();
                    gui.setDocumento(session, item.numerodocumento, item.ruc_alterno, item.codigopaciente, item.unidad, item.sede);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void MenuContext_Carta_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridDocumento.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                    var doc = (DocumentosEmitidos)cellRow.Item;
                    ObservableCollection<RadMenuItem> items = new ObservableCollection<RadMenuItem>();
                    foreach (var item in lstSucursal.Where(x=>x.codigounidad==doc.unidad).ToList())
                    {
                        RadMenuItem newItem = new RadMenuItem() { Header = item.codigoInt+" - "+item.nombre,CommandParameter=item.codigosucursal };
                        newItem.Click += new Telerik.Windows.RadRoutedEventHandler(menu_ItemChangeSucursalClick);
                        items.Add(newItem);
                    }
                    menuitemCanhgeSucursal.ItemsSource = null;
                    menuitemCanhgeSucursal.ItemsSource = items;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void menu_ItemChangeSucursalClick(Object sender, RoutedEventArgs e)
        {
            RadMenuItem item = e.OriginalSource as RadMenuItem;
            item.CommandParameter = item.CommandParameter;
              DocumentosEmitidos doc = (DocumentosEmitidos)this.gridDocumento.SelectedItem;
              if (doc != null)
              {
                  MessageBox.Show(doc.unidad + " - " + int.Parse(item.CommandParameter.ToString()));
                  await new CobranzaDAO().setSucursalDocumento(doc.numerodocumento, doc.unidad, int.Parse(item.CommandParameter.ToString()));
              }
            
            
        }

        private void rdpFecha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(rdpFecha.SelectedDate.Value.ToShortDateString(), "3", int.Parse(cbosucursal.SelectedValue.ToString())));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void listar(List<DocumentosEmitidos> lista)
        {
            gridDocumento.ItemsSource = null;
            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.Member = "serie";
            descriptor.DisplayContent = "Serie";
            descriptor.SortDirection = ListSortDirection.Ascending;
            GroupDescriptor empresa = new GroupDescriptor();
            empresa.Member = "empresa";
            empresa.DisplayContent = "Empresa";
            empresa.SortDirection = ListSortDirection.Ascending;
            GroupDescriptor tipodoc = new GroupDescriptor();
            tipodoc.Member = "tipodocumento";
            tipodoc.DisplayContent = "Tip. Doc.";
            tipodoc.SortDirection = ListSortDirection.Ascending;
            gridDocumento.GroupDescriptors.Add(empresa);
            gridDocumento.GroupDescriptors.Add(tipodoc);
            gridDocumento.GroupDescriptors.Add(descriptor);
            gridDocumento.ItemsSource = lista;
            gridDocumento.AutoExpandGroups = true;

            gridDocumentoconsolidado.ItemsSource = null;
            var listaconsolidado = lista.Select(x => new { x.razonsocial, x.ruc_alterno, x.total }).GroupBy(x => new { x.ruc_alterno, x.razonsocial }).Select(x => new { RUC = x.Key.ruc_alterno, RasonSocial = x.Key.razonsocial, Total = x.Sum(g => g.total) }).ToList();
            gridDocumentoconsolidado.ItemsSource = listaconsolidado;



        }

        private void rdpFechaRango_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(rdpInicio.SelectedDate.Value.ToShortDateString(), "4", int.Parse(cbosucursal.SelectedValue.ToString()), "", rdpFin.SelectedDate.Value.ToShortDateString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtruc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(txtruc.Text, "5", 1, cboTipoDocumento.Text.Substring(0, 2), rdpFinRUC.SelectedDate.Value.ToShortDateString(), rdpInicioRUC.SelectedDate.Value.ToShortDateString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtSerie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    listar(new CobranzaDAO().getDocumentosEmitidos(rdpInicio1.SelectedDate.Value.ToShortDateString(), "6", int.Parse(cbosucursal2.SelectedValue.ToString()), txtserie.Text, rdpFin1.SelectedDate.Value.ToShortDateString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(gridDocumento, true, false, false);
        }

        private void MenuItemExportar2_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(gridDocumentoconsolidado, true, false, false);
        }
    }
}
