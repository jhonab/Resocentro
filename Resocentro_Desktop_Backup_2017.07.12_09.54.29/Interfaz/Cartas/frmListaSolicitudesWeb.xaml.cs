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
using System.Windows.Threading;

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para frmListaSolicitudesWeb.xaml
    /// </summary>
    public partial class frmListaSolicitudesWeb : Window
    {
        MySession session;
        public frmListaSolicitudesWeb()
        {
            InitializeComponent();
        }
        public void iniciarGUI(MySession session)
        {
            this.session = session;
            cboEstadoSolicitud.ItemsSource = Enum.GetValues(typeof(Estado_SolicitudTramite)).Cast<Estado_SolicitudTramite>().ToList();
            cboEstadoSolicitud.SelectedValue = Estado_SolicitudTramite.Nuevo;
            getListaSolicitudescarta();

            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Interval = new TimeSpan(0, 5, 0);
            dispatcher.Tick += (s, a) =>
            {
                if (chkUpdate.IsChecked.Value)
                    getListaSolicitudescarta();
            };
            dispatcher.Start();
        }

        private void getListaSolicitudescarta()
        {
            CartaDAO daoCarta = new CartaDAO();
            grid_SolicitudesCarta.ItemsSource = daoCarta.getListaSolicitudesCarta((int)(Estado_SolicitudTramite)cboEstadoSolicitud.SelectedValue);
            lblrefresh.Content = "Última Actualización:  " + DateTime.Now.ToShortTimeString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getListaSolicitudescarta();
        }

        private void grid_SolicitudesCarta_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            CG_SolicitudCarta item = (CG_SolicitudCarta)e.Row.DataContext;
            if (item != null)
            {
                frmSolicitudWeb gui = new frmSolicitudWeb();
                gui.cargarGUI(session, item);
                gui.Show();
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuEspera_click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CG_SolicitudCarta item = (CG_SolicitudCarta)this.grid_SolicitudesCarta.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("Desea cambiar el estado a ESPERa la Solicitud del Paciente:\n" + item.apellidos + " " + item.nombres, "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item.estado = (int)Estado_SolicitudTramite.Espera;
                    item.fec_lectura = null;
                    item.numeroproforma = null;
                    new CartaDAO().updateSolicitudCarta(item, session.shortuser);
                    getListaSolicitudescarta();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void Menutramitado_click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CG_SolicitudCarta item = (CG_SolicitudCarta)this.grid_SolicitudesCarta.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("Desea cambiar el estado a TRAMITADO la Solicitud del Paciente:\n" + item.apellidos + " " + item.nombres, "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item.estado = (int)Estado_SolicitudTramite.Tramitado;
                    item.fec_lectura = null;
                    item.fec_lectura = null;
                    item.fec_lectura = null;
                    new CartaDAO().updateSolicitudCarta(item, session.shortuser);
                    getListaSolicitudescarta();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void MenuCancelado_click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CG_SolicitudCarta item = (CG_SolicitudCarta)this.grid_SolicitudesCarta.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("Desea cambiar el estado a CANCELADO la Solicitud del Paciente:\n" + item.apellidos + " " + item.nombres, "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    item.estado = (int)Estado_SolicitudTramite.Cancelado;
                    item.fec_lectura = null;
                    item.fec_lectura = null;
                    item.fec_lectura = null;
                    new CartaDAO().updateSolicitudCarta(item, session.shortuser);
                    getListaSolicitudescarta();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CG_SolicitudCarta item = (CG_SolicitudCarta)this.grid_SolicitudesCarta.SelectedItem;
            if (item != null)
            {
                MenuContext_Lista.Visibility = Visibility.Visible;
                if (item.estado == (int)Estado_SolicitudTramite.Tramitado || item.estado == (int)Estado_SolicitudTramite.Cancelado)
                {
                    MenuContext_Lista.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MenuContext_Lista.Visibility = Visibility.Collapsed;
            }
        }


    }
}
