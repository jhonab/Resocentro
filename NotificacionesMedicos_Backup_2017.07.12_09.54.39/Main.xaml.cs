using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.AspNet.SignalR.Client;
using System.Reflection;

namespace NotificacionesMedicos
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        //Thread hilo;
        //MainWindow config;
        //frmencuesta notify;
        //frmsupervisor notify1;
        //frmdespedida notify2;
        //public bool isEncuesta;
        //public bool isSupervisor;
        //public bool isDespedida;
        //public string[] totalEncuesta;
        //public string[] totalSupervisor;
        //public string[] totalSatisfaccion;
        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://extranet.resocentro.com:5052/";
        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }

        public bool Active { get; set; }
        List<EQUIPO> lstEquipo { get; set; }

        public Main()
        {
            InitializeComponent();

            
            //DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            //dispatcherTimer.Start();
            //this.config = frmconfiguracion;
            //this.config.chkencuesta.Dispatcher.Invoke(new Action(
            //    () => isEncuesta = this.config.chkencuesta.IsChecked.Value));
            //this.config.chkSupervicion.Dispatcher.Invoke(new Action(
            //    () => isSupervisor = this.config.chkSupervicion.IsChecked.Value));
            //this.hilo =
            //    new Thread(new ThreadStart(this.Proceso));

            //this.hilo.Start();
            Active = true;
            Thread = new System.Threading.Thread(() =>
            {
                Connection = new HubConnection(Host);
                Proxy = Connection.CreateHubProxy("EmetacHub");

                Proxy.On<string, string>("perifoneo", (tipo, equipo) => perifonear(tipo, equipo));
                Connection.Start();

                while (Active)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }) { IsBackground = true };
            Thread.Start();
            //perifonear("supervisor", "achieva");
            this.Hide();
        }
        public string returnPath()
        {
            string folder = Environment.CurrentDirectory;
           return folder;
            
        }
        private void perifonear(string tipo, string equipo)
        {

            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {

                if ((supernotify.chksup.IsChecked.Value || supernotify.chkenf.IsChecked.Value || supernotify.chktec.IsChecked.Value) && Properties.Settings.Default.equipos.Split('|').Contains(equipo.ToLower()))
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
                        if (supernotify.chksup.IsChecked.Value && tipo == "supervisor.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            if(supernotify.chkVisual.IsChecked.Value)
                            showNotify("Supervicion Solicitada","Equipo - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else if (supernotify.chkenf.IsChecked.Value && tipo == "enfermera.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            if (supernotify.chkVisual.IsChecked.Value)
                            showNotify("Validación de Enfermera","Equipo - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else if (supernotify.chktec.IsChecked.Value && tipo == "tecnologo.mp3")
                        {
                            _mp3Player.Play();
                            reproducirequipo(ubicacion.FullName);
                            if (supernotify.chkVisual.IsChecked.Value)
                            showNotify("Validacion de Tecnólogo","Equipo - " + equipo.Replace(".mp3", "").ToUpper());
                        }
                        else { }
                    }

                }

            }));

        }
        private void showNotify(string titulo,string texto)
        {
            
            frmDetalleNotificacion noti = new frmDetalleNotificacion();
            noti.txttexto.Text = texto;
            noti.txttitulo.Text = titulo;
            tb.ShowCustomBalloon(noti, PopupAnimation.Slide,5000);

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
        private void Proceso()
        {
            //while (true)
            //{

            //    this.config.chkencuesta.Dispatcher.Invoke(new Action(
            //    () => notify = new frmencuesta()));

            //    this.config.chkSupervicion.Dispatcher.Invoke(new Action(
            //    () => notify1 = new frmsupervisor()));

            //    this.config.chkDespedida.Dispatcher.Invoke(new Action(
            //    () => notify2 = new frmdespedida()));

            //    this.config.chkencuesta.Dispatcher.Invoke(new Action(
            //    () => isEncuesta = this.config.chkencuesta.IsChecked.Value));
            //    this.config.chkSupervicion.Dispatcher.Invoke(new Action(
            //        () => isSupervisor = this.config.chkSupervicion.IsChecked.Value));
            //    this.config.chkDespedida.Dispatcher.Invoke(new Action(
            //        () => isDespedida = this.config.chkDespedida.IsChecked.Value));
            //    this.calcular(this.config, this.totalEncuesta);
            //    Thread.Sleep(30000);
            //}
        }

        private void calcular(MainWindow con, string[] te)
        {
           /* using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                bool isOpen = false;
                if (this.isEncuesta)
                {
                    var lista = (from ea in db.EXAMENXATENCION
                                 join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                                 from en in en_join.DefaultIfEmpty()
                                 where
                                 (ea.equipoAsignado == null || ea.equipoAsignado == 0)
                                 && ea.estadoestudio == "A"
                                 && ea.codigoestudio.Substring(1 - 1, 3) == "101"
                                 orderby ea.codigo
                                 select new
                                 {
                                     numero = ea.codigo
                                 }
                             ).AsParallel().ToList();

                    if (lista.Count() > 0)
                    {
                        this.notify.Dispatcher.Invoke(new Action(
                     () => this.notify.txttexto.Text = "Hay " + lista.Count.ToString() + " Encuestas por realizar"));
                        tb.ShowCustomBalloon(this.notify, PopupAnimation.Scroll, 10000);
                        isOpen = true;
                    }
                }
                while (isOpen)
                {
                    isOpen = !this.notify.isClosing;
                    Thread.Sleep(1000);
                }
                if (this.isSupervisor)
                {
                    var lista = (from ea in db.EXAMENXATENCION
                                 join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                                 from en in en_join.DefaultIfEmpty()
                                 where
                                  ea.estadoestudio == "A"
                                 && ea.codigoestudio.Substring(1 - 1, 3) == "101"
                                 && en.estado == 1
                                     //&& en.estado < 4
                                 && en.SolicitarValidacion == true
                                 orderby ea.codigo
                                 select new
                                 {
                                     numero = ea.codigo
                                 }
                             ).AsParallel().ToList();

                    if (lista.Count() > 0)
                    {
                        this.notify1.Dispatcher.Invoke(new Action(
                     () => this.notify1.txttexto.Text = "Hay " + lista.Count.ToString() + " Superviciones por realizar"));
                        tb.ShowCustomBalloon(this.notify1, PopupAnimation.Slide, 10000);
                        isOpen = true;
                    }
                }
                while (isOpen)
                {
                    isOpen = !this.notify1.isClosing;
                    Thread.Sleep(1000);
                }
                if (this.isDespedida)
                {
                    var lista = (from ea in db.EXAMENXATENCION
                                 join enc in db.Encuesta on ea.codigo equals enc.numeroexamen
                                 join en in db.Encuesta_Satisfaccion on ea.numeroatencion equals en.numeroatecion
                                 where
                                  ea.estadoestudio == "R"
                                 && en.isTerminado == null
                                 group ea by new
                                 {
                                     ea.numeroatencion
                                 } into atencion
                                 select new
                                 {
                                     numero = atencion.Key.numeroatencion,
                                  
                                 }
                             ).AsParallel().ToList();

                    if (lista.Count() > 0)
                    {
                        this.notify2.Dispatcher.Invoke(new Action(
                     () => this.notify2.txttexto.Text = "Hay " + lista.Count.ToString() + " Despedidas por realizar"));
                        tb.ShowCustomBalloon(this.notify2, PopupAnimation.Slide, 10000);
                        isOpen = true;
                    }
                }
                while (isOpen)
                {
                    isOpen = !this.notify1.isClosing;
                    Thread.Sleep(1000);
                }
            }*/

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Main gui = new Main();
            //gui.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string title = "WPF NotifyIcon";
            string text = "This is a standard balloon";
           
        }

      
        //private void Proceso()//(object sender, EventArgs e)
        //{
        //    bool isOpen = false;


        //}




    }
}
