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

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmBloqueoTurno.xaml
    /// </summary>
    public partial class frmBloqueoTurno : Window
    {
        MySession session;
        public frmBloqueoTurno()
        {
            InitializeComponent();
        }
        public void cargarGUI( MySession session,DateTime fecha)
        {
            this.session = session;
            cboEquipo.ItemsSource = new UtilDAO().getEquipoxEmpresa(1);
            cboEquipo.SelectedValuePath = "codigoequipo";
            cboEquipo.DisplayMemberPath = "nombreequipo";
            cboEquipo.SelectedIndex = 1;

            dtpFecha.SelectedDate = fecha;
        }

        private void btnListar_Click(object sender, RoutedEventArgs e)
        {
            listarTurnos();
        }

        public void listarTurnos()
        {
            if (cboEquipo.SelectedValue != null)
                gridTurnos.ItemsSource = new CitaDAO().getListaAdminTurnos(cboEquipo.SelectedValue.ToString(), dtpFecha.SelectedDate.Value.Day, dtpFecha.SelectedDate.Value.Month, dtpFecha.SelectedDate.Value.Year);
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (gridTurnos.SelectedItems.Count > 0)
            {
                ContextMenuItemAdministrarTurno.Visibility = Visibility.Visible;
            }
            else
            {
                ContextMenuItemAdministrarTurno.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItemAdministrarTurno_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            List<Turno_Horario> lista = new List<Turno_Horario>();
            foreach (Turno_Horario item in gridTurnos.SelectedItems.ToList())
            {
                lista.Add(item);
            }
            frmConfirmacionBloquedo gui = new frmConfirmacionBloquedo();
            gui.cargarGUI(session, lista);
            gui.ShowDialog();
            listarTurnos();
        }
    }
}
