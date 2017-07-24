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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Resocentro_Desktop.DAO;
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.AtencionCliente
{
    /// <summary>
    /// Lógica de interacción para frmTicketera.xaml
    /// </summary>
    public partial class frmTicketera : Window
    {

        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://serverweb:5132/";
        //public string Host = "http://localhost:7467/";
        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }

        public TKT_Counter tkt_counter;
        public TKT_Ticketera tkt_ticketera;
        private DispatcherTimer dispatcher = new DispatcherTimer();
        private int i = 0;
        public bool Active { get; set; }

        MySession session;
        public frmTicketera()
        {
            InitializeComponent();
        }

        #region SignalR
        private async Task SendMessage()
        {
            await Proxy.Invoke("CallPaciente2");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Active = true;
            Thread = new System.Threading.Thread(() =>
            {
                Connection = new HubConnection(Host);
                Proxy = Connection.CreateHubProxy("TicketeraHub");

                Proxy.On("callPaciente2", () => OnSendData(""));
                Proxy.On("actualizarPaciente2", (mensaje) => OnSendData(mensaje));
                Connection.Start();

                while (Active)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }) { IsBackground = true };
            Thread.Start();
        }
        private void OnSendData(string message)
        {
            //Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => MessagesListBox.Items.Insert(0, message)));

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    lblpaciente.Content = getCantidadPacientes();
                    if (expEspera.IsExpanded == true)
                        listarEspera();
                }
                ));

        }

        private string getCantidadPacientes()
        {
            return new AtencionClienteDAO().getCantidadPacientes(tkt_counter.idempresa, tkt_counter.idSede.Value);
        }
        #endregion

        public void cargarGUI(MySession session)
        {
            try
            {
                this.session = session;
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    tkt_counter = db.TKT_Counter.SingleOrDefault(x => x.nombre == session.codigousuario);
                    if (tkt_counter == null)
                        throw new Exception("No tiene permisos de Counter");
                    dispatcher.Interval = new TimeSpan(0, 0, 1);
                    dispatcher.Tick += (s, a) =>
                    {
                        i++;
                        TimeSpan result = TimeSpan.FromSeconds(i);
                        lbltiempo.Content = "Tiempo: " + result.ToString("mm':'ss");
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void ButtonLlamar_Click(object sender, RoutedEventArgs e)
        {
            tkt_ticketera = new AtencionClienteDAO().getTicket(tkt_counter.idempresa, tkt_counter.idSede.Value, chkCaja.IsChecked.Value, tkt_counter.idcounter);
            if (tkt_ticketera != null)
            {
                try
                {


                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var tipo_atencion = db.TKT_Tipo_Atencion.SingleOrDefault(x => x.id_tipo_aten == tkt_ticketera.id_tipo_aten);
                        if (tipo_atencion != null)
                        {
                            txtpaciente.Text = tkt_ticketera.nombre;
                            if (chkCaja.IsChecked.Value)
                                txttipo.Text = "CAJA";
                            else
                                txttipo.Text = tipo_atencion.nombre;
                            btnLlamar.IsEnabled = false;
                            dispatcher.Start();
                            if (expAtendido.IsExpanded == true)
                                listarAtendidos();
                            if (expEspera.IsExpanded == true)
                                listarEspera();

                            await SendMessage();
                        }
                        else
                        {
                            txtpaciente.Text = "";
                            txttipo.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private async void ButtonRellamada_Click(object sender, RoutedEventArgs e)
        {
            if (tkt_ticketera != null)
            {
                try
                {
                    new AtencionClienteDAO().getTicketRellamada(tkt_ticketera.id_Ticket);

                    await SendMessage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private async void ButtonLiberar_Click(object sender, RoutedEventArgs e)
        {
            if (tkt_ticketera != null)
            {
                try
                {
                    if (!chkCaja.IsChecked.Value)
                    {
                        new AtencionClienteDAO().getTicketLiberar(tkt_ticketera.id_Ticket);
                        btnLlamar.IsEnabled = true;
                        txtpaciente.Text = "";
                        txttipo.Text = "";
                        detenerCronometro();
                        await SendMessage();
                        if (expAtendido.IsExpanded == true)
                            listarAtendidos();
                        if (expEspera.IsExpanded == true)
                            listarEspera();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void detenerCronometro()
        {
            i = 0;
            dispatcher.Stop();
            lbltiempo.Content = "";
        }

        private async void ButtonAusente_Click(object sender, RoutedEventArgs e)
        {
            if (tkt_ticketera != null)
            {
                try
                {
                    new AtencionClienteDAO().getTicketAusente(tkt_ticketera.id_Ticket, this.session);
                    btnLlamar.IsEnabled = true;
                    txtpaciente.Text = "";
                    txttipo.Text = "";
                    detenerCronometro();
                    await SendMessage();
                    if (expAtendido.IsExpanded == true)
                        listarAtendidos();
                    if (expEspera.IsExpanded == true)
                        listarEspera();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void ButtonTerminado_Click(object sender, RoutedEventArgs e)
        {
            if (tkt_ticketera != null)
            {
                try
                {
                    new AtencionClienteDAO().getTicketTerminado(tkt_ticketera.id_Ticket, this.session, chkCaja.IsChecked.Value);
                    btnLlamar.IsEnabled = true;
                    txtpaciente.Text = "";
                    txttipo.Text = "";
                    detenerCronometro();
                    await SendMessage();
                    if (expAtendido.IsExpanded == true)
                        listarAtendidos();
                    if (expEspera.IsExpanded == true)
                        listarEspera();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (btnLlamar.IsEnabled == false)
            {
                e.Cancel = true;
                MessageBox.Show("Termine con la atención antes de cerrar.", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RadExpander_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.Width = 800;
            listarAtendidos();
        }

        private void RadExpander_Collapsed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.Width = 480;
        }

        private void RadExpander_Expanded_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.Height = 500;
            listarEspera();
        }

        private void RadExpander_Collapsed_1(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            this.Height = 210;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listarAtendidos();
        }

        private void listarAtendidos()
        {
            grid_atendidos.ItemsSource = null;
            grid_atendidos.ItemsSource = new AtencionClienteDAO().getTicketAtendidos(tkt_counter.idempresa, tkt_counter.idSede.Value, tkt_counter.idcounter);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            listarEspera();
        }

        private void listarEspera()
        {
            grid_espera.ItemsSource = null;
            grid_espera.ItemsSource = new AtencionClienteDAO().getTicketEspera(tkt_counter.idempresa, tkt_counter.idSede.Value);
        }

        private async void btnAtenderticket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = (RadButton)e.OriginalSource;
                var item = (int)button.CommandParameter;

                if (btnLlamar.IsEnabled == true)
                {
                    tkt_ticketera = new AtencionClienteDAO().getTicketxId(item, tkt_counter.idcounter);
                    if (tkt_ticketera != null)
                    {
                        try
                        {
                            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                            {
                                var tipo_atencion = db.TKT_Tipo_Atencion.SingleOrDefault(x => x.id_tipo_aten == tkt_ticketera.id_tipo_aten);
                                if (tipo_atencion != null)
                                {
                                    txtpaciente.Text = tkt_ticketera.nombre;
                                    if (chkCaja.IsChecked.Value)
                                        txttipo.Text = "CAJA";
                                    else
                                        txttipo.Text = tipo_atencion.nombre;

                                    btnLlamar.IsEnabled = false;
                                    dispatcher.Start();
                                    listarEspera();
                                    await SendMessage();
                                }
                                else
                                {
                                    txtpaciente.Text = "";
                                    txttipo.Text = "";
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
