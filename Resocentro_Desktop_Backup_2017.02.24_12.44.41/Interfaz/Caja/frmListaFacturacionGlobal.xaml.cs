using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.frmUtil;
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
    /// Lógica de interacción para frmListaFacturacionGlobal.xaml
    /// </summary>
    public partial class frmListaFacturacionGlobal : Window
    {
        MySession session;
        bool isupdate;
        int idFac;
        public ASEGURADORA aseguradora { get; set; }
        public List<PreResumenFacGlobal> lista { get; set; }
        public frmListaFacturacionGlobal()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session, bool isupdate, string datos, int idFac = 0)
        {
            this.session = session;
            this.isupdate = isupdate;
            this.idFac = idFac;

            txtdatosadicionales.Text = datos;

            cboempresa.ItemsSource = new UtilDAO().getEmpresa();
            cboempresa.SelectedValuePath = "codigounidad";
            cboempresa.DisplayMemberPath = "nombre";
            cboempresa.SelectedIndex = 0;

            rdpinicio.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            rdpfin.SelectedDate = DateTime.Now;
            aseguradora = new ASEGURADORA();

        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (aseguradora.ruc != null)
            {
                lista = new CobranzaDAO().getListaFacGlobal(rdpinicio.SelectedDate.Value.ToShortDateString(), rdpfin.SelectedDate.Value.ToShortDateString(), aseguradora.ruc, cboempresa.SelectedValue.ToString());
                setLista();
                refreshResumen();
            }
            else
            {
                MessageBox.Show("Seleccione una Aseguradora");
            }
        }

        private void setLista()
        {
            grid_Atenciones.ItemsSource = null;
            grid_Atenciones.ItemsSource = lista;
        }

        private void refreshResumen()
        {
            try
            {
                lblresumen.Content = "Total Items: " + lista.Count().ToString() + "\t Seleccionadas: " + lista.Where(x => x.isSelected).Count().ToString() + "\t Monto Total Soles: " + lista.Where(x => x.isSelected && x.codigomoneda == 1).Sum(x => x.preciobruto).ToString() + "\t Monto Total Dolares: " + lista.Where(x => x.isSelected && x.codigomoneda == 2).Sum(x => x.preciobruto).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnReceptorEmpresa_Click(object sender, RoutedEventArgs e)
        {
            frmSearchCompania gui = new frmSearchCompania();
            gui.cargarGUI(session);
            gui.ShowDialog();
            if (gui.DialogResult.Value)
            {
                aseguradora.ruc = gui.empresa.ruc;
                aseguradora.razonsocial = gui.empresa.razonsocial;
                setAseguradora();
                grid_Atenciones.ItemsSource = null;
            }
        }

        private void setAseguradora()
        {
            lblaseguradora.Content = aseguradora.razonsocial;
        }

        private void grid_Atenciones_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            try
            {
                PreResumenFacGlobal det = (PreResumenFacGlobal)e.EditedItem;

                var _item = lista.SingleOrDefault(x => x.codigoexamencita == det.codigoexamencita);
                if (_item != null)
                {
                    _item = det;

                }
                refreshResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void RadMenuItemSeleccionar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            foreach (PreResumenFacGlobal item in this.grid_Atenciones.SelectedItems.ToList())
            {
                var _item = lista.SingleOrDefault(x => x.codigoexamencita == item.codigoexamencita);
                if (_item != null)
                {
                    _item.isSelected = !_item.isSelected;
                    _item = item;
                }
            }
            setLista();
            refreshResumen();
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {

            var cobranzaDao = new CobranzaDAO();
            try
            {
                if (!isupdate)
                {
                    int idFactura = cobranzaDao.insertAddFacturaGlobal(aseguradora.ruc, aseguradora.razonsocial, cboempresa.SelectedValue.ToString(), txtdatosadicionales.Text, session);
                    if (lista.Where(x => x.isSelected).Count() > 0)
                    {
                        cobranzaDao.insertAddDetalleFacturaGlobal(lista.Where(x => x.isSelected).ToList(), idFactura, txtdatosadicionales.Text, session);
                    }

                    MessageBox.Show("Se registro con exito Id de registro: " + idFactura.ToString());
                    this.Close();
                }
                else
                {
                    cobranzaDao.insertAddDetalleFacturaGlobal(lista.Where(x => x.isSelected).ToList(), idFac, txtdatosadicionales.Text, session);
                    MessageBox.Show("Se agregó con exito Id de registro: " + idFac.ToString());
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void RadMenuItemSoles_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            foreach (PreResumenFacGlobal item in this.grid_Atenciones.SelectedItems.ToList())
            {
                var _item = lista.SingleOrDefault(x => x.codigoexamencita == item.codigoexamencita);
                if (_item != null)
                {
                    _item.codigomoneda = 1;
                    _item = item;
                }
            }
            setLista();
            refreshResumen();
        }

        private void RadContextMenuDetalle_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            /*try
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

            }*/
        }

        private void RadMenuItemDolares_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            foreach (PreResumenFacGlobal item in this.grid_Atenciones.SelectedItems.ToList())
            {
                var _item = lista.SingleOrDefault(x => x.codigoexamencita == item.codigoexamencita);
                if (_item != null)
                {
                    _item.codigomoneda = 2;
                    _item = item;
                }
            }
            setLista();
            refreshResumen();
        }

        private void RadMenuItemAsignar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            frmSetPrecioFacGlobal gui = new frmSetPrecioFacGlobal();
            gui.Owner = this;
            if (gui.ShowDialog().Value)
            {
                foreach (PreResumenFacGlobal item in this.grid_Atenciones.SelectedItems.ToList())
                {
                    var _item = lista.SingleOrDefault(x => x.codigoexamencita == item.codigoexamencita);
                    if (_item != null)
                    {
                        _item.codigomoneda = gui.moneda;
                        _item.preciobruto = gui.precio;
                        _item.isSelected = true;
                        _item = item;
                    }
                }
                setLista();
                refreshResumen();
            }
        }
    }
}
