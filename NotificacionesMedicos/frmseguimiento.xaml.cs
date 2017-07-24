using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
using System.Reflection;

using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;

namespace NotificacionesMedicos
{
    /// <summary>
    /// Lógica de interacción para frmseguimiento.xaml
    /// </summary>
    public partial class frmseguimiento : UserControl
    {
        
        string usuario = "";
        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://extranet.resocentro.com:5052/";
        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }

        public bool Active { get; set; }
        List<EQUIPO> lstEquipo { get; set; }

        public int MyValue { get; set; }
        public frmseguimiento()
        {
            InitializeComponent();

            usuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            lstEquipo = new SisEncuesta().getEquipo();
            cboEquipo.ItemsSource = lstEquipo.OrderBy(x=> x.nombreequipo).ToList();
            cboEquipo.DisplayMemberPath = "nombreequipo";
            cboEquipo.SelectedValuePath = "codigoequipo";

            chksup.IsChecked = Properties.Settings.Default.supervicion;
            chkenf.IsChecked = Properties.Settings.Default.enfermeria;
            chktec.IsChecked = Properties.Settings.Default.tecnologo;
            chkVisual.IsChecked = Properties.Settings.Default.alertaVisual;
            
            var equipos = Properties.Settings.Default.equipos.Split('|');
            foreach (var item in equipos)
            {
                EQUIPO e = lstEquipo.SingleOrDefault(x => x.ShortDesc.ToLower().ToString() == item);
                if (e != null)
                    cboEquipo.SelectedItems.Add(e);
            }

         

            //Active = true;
            //Thread = new System.Threading.Thread(() =>
            //{
            //    Connection = new HubConnection(Host);
            //    Proxy = Connection.CreateHubProxy("EmetacHub");

            //    Proxy.On<string, string>("perifoneo", (tipo, equipo) => perifonear(tipo, equipo));
            //    Connection.Start();

            //    while (Active)
            //    {
            //        System.Threading.Thread.Sleep(10);
            //    }
            //}) { IsBackground = true };
            //Thread.Start();
            //perifonear("supervisor", "avanto");
            //perifonear("enfermera", "avanto");
            //perifonear("supervisor", "achieva");
        }

        private void ejecutarPerfifoneo()
        {
            var lista = new SisEncuesta().BuscarPerifoneo();
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"sound");
            //string path = @"C:\sound";
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            System.IO.FileInfo[] files = dir.GetFiles();

            MediaPlayer _mp3Player = new MediaPlayer();
            foreach (var item in lista)
            {
                var tipo = files.SingleOrDefault(x => x.Name == item.tipo);
                var ubicacion = files.SingleOrDefault(x => x.Name == item.ubicacion);

                if (tipo != null)
                {

                    Uri mp3 = new Uri(tipo.FullName);
                    _mp3Player.Open(mp3);
                    _mp3Player.Play();
                }
                if (item.tipo == "enfermera.mp3")
                    System.Threading.Thread.Sleep(2000);
                else
                    System.Threading.Thread.Sleep(1000);
                if (ubicacion != null)
                {
                    //_mp3Player.Stop();
                    Uri mp3 = new Uri(ubicacion.FullName);
                    _mp3Player.Open(mp3);
                    _mp3Player.Play();
                }
                System.Threading.Thread.Sleep(1500);
            }
        }

        private void bloquear(string texto)
        {
            frmBloqueo gui = new frmBloqueo();
            gui.lblexamen.Text = texto;
            gui.Show();
            Process.Start("cmd", "/C rundll32.exe user32.dll,LockWorkStation");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void perifonear(string tipo, string equipo)
        {

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {

                
               
            
                if ((chksup.IsChecked.Value || chkenf.IsChecked.Value || chktec.IsChecked.Value) && Properties.Settings.Default.equipos.Split('|').Contains(equipo))
                {
                    tipo = tipo.ToLower() + ".mp3";
                    equipo = equipo.ToLower() + ".mp3";

                    string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"sound");
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                    System.IO.FileInfo[] files = dir.GetFiles();

                    MediaPlayer _mp3Player = new MediaPlayer();

                    var tipo_audio = files.SingleOrDefault(x => x.Name == tipo);
                    var ubicacion = files.SingleOrDefault(x => x.Name == equipo);
                    if (tipo_audio != null && ubicacion != null)
                    {
                        Uri mp3 = new Uri(tipo_audio.FullName);
                        _mp3Player.Open(mp3);
                        if (chksup.IsChecked.Value && tipo == "supervisor.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            showNotify("Supervision - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else if (chkenf.IsChecked.Value && tipo == "enfermera.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            showNotify("Enfermera - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else if (chktec.IsChecked.Value && tipo == "tecnologo.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            showNotify("Tecnólogo - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else { }
                    }
                    
                }

            }));

        }

        private void reproducirequipo(string paht)
        {
            System.Threading.Thread.Sleep(1000);
            MediaPlayer _mp3Player = new MediaPlayer();
            //_mp3Player.Stop();
            Uri mp3 = new Uri(paht);
            _mp3Player.Open(mp3);
            _mp3Player.Play();
        }
        private void showNotify(string texto)
        {
            
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.supervicion = chksup.IsChecked.Value;
            Properties.Settings.Default.enfermeria = chkenf.IsChecked.Value;
            Properties.Settings.Default.tecnologo = chktec.IsChecked.Value;
            Properties.Settings.Default.alertaVisual = chkVisual.IsChecked.Value;
            var equipos = "";
            foreach (EQUIPO item in cboEquipo.SelectedItems)
            {
                equipos += item.ShortDesc.ToLower().ToString() + "|";
            }
            Properties.Settings.Default.equipos = equipos;
            Properties.Settings.Default.Save();
        }


    }
}
