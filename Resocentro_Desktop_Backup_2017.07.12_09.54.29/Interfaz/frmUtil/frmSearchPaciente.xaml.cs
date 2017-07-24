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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmSearchPaciente.xaml
    /// </summary>
    public partial class frmSearchPaciente : Window
    {
        public PACIENTE paciente;
        public frmSearchPaciente()
        {
            InitializeComponent();
            txtfiltro.Focus();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            buscarPaciente();
        }
        public void buscarPaciente()
        {
            string filtro = txtfiltro.Text.Trim();
            string nombres = txtnombre.Text.Trim();
            string dni = txtdni.Text.Trim();

            if (filtro+nombres+dni != "")
            {
                gridPacientes.ItemsSource = new PacienteDAO().getBuscarPacientexCoincidencia(filtro,nombres,dni);
                gridPacientes.Focus();
            }
            else
            {
                gridPacientes.ItemsSource = null;
                txtfiltro.Focus();
            }
        }

        private void txtfiltro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                buscarPaciente();
        }

        private void gridPacientes_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            var item = (PACIENTE)this.gridPacientes.SelectedItem;
            if (item != null)
            {
                paciente = item;
                DialogResult = true;
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            frmPaciente gui = new frmPaciente();
            gui.insertPaciente(new PACIENTE() { apellidos = txtfiltro.Text, tipo_doc = "0" });
            gui.ShowDialog();
            if (gui.paciente != null)
            {
                txtfiltro.Text = gui.paciente.apellidos.ToUpper().Trim();
                buscarPaciente();
            }
            else
            {
                txtfiltro.Text = "";
            }
        }

        private void gridPacientes_FilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if(stkFiltro.Visibility != Visibility.Collapsed)
            stkFiltro.Visibility = Visibility.Collapsed;
            else
                stkFiltro.Visibility = Visibility.Visible;
        }

        private void RadContextMenuPaciente_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridPacientes.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            PACIENTE item = (PACIENTE)this.gridPacientes.SelectedItem;
            if (item == null)
            {
                MenuItemInfoPaciente.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuItemInfoPaciente.Visibility = Visibility.Visible;                
            }
        }

        private void MenuItemInfoPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PACIENTE item = (PACIENTE)this.gridPacientes.SelectedItem;
            if (item != null)
            {
                frmPaciente gui = new frmPaciente();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    gui.setPaciente(item);
                    gui.ShowDialog();
                    buscarPaciente();
                }
            }
        }

    }
}
