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

            this.Hide();
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
            Main gui = new Main();
            gui.Show();
        }
        //private void Proceso()//(object sender, EventArgs e)
        //{
        //    bool isOpen = false;


        //}




    }
}
