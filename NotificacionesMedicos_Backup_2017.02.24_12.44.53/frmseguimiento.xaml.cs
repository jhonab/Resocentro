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

namespace NotificacionesMedicos
{
    /// <summary>
    /// Lógica de interacción para frmseguimiento.xaml
    /// </summary>
    public partial class frmseguimiento : UserControl
    {
        string usuario = "";
        public frmseguimiento()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

            DispatcherTimer dispatcherTimer1;
            dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += new EventHandler(dispatcherTimer1_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer1.Start();

            usuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            ejecutar();
            ejecutarPerfifoneo();
        }

        private void ejecutarPerfifoneo()
        {
            var lista = new SisEncuesta().BuscarPerifoneo();
            string path = @"C:\sound";
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
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

           // ejecutar();
        }
        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {

            ejecutarPerfifoneo();
        }

        private void ejecutar()
        {
            /*var usu = new SisEncuesta().getusuario(usuario);
            lblusuario.Text = usuario + " - " + usu;
            var lista = new SisEncuesta().BuscarNewSupervisiones();
            lista = lista.Where(x => x.supervisor == usu).ToList();
            string estudios = "";
            foreach (var item in lista)
            {
                TimeSpan ts = DateTime.Now - item.inicio;
                if (ts.Minutes >= 5)
                    estudios += item.paciente + " - " + item.estudio + " - " + item.equipo + " - " + ts.Minutes + "' \n";
            }
            if (estudios != "")
                bloquear(estudios);*/


        }

        private void bloquear(string texto)
        {
            frmBloqueo gui = new frmBloqueo();
            gui.lblexamen.Text = texto;
            gui.Show();
            Process.Start("cmd", "/C rundll32.exe user32.dll,LockWorkStation");
        }
    }
}
