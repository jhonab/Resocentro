using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.administracion;
using Resocentro_Desktop.Interfaz.AtencionCliente;
using Resocentro_Desktop.Interfaz.Caja;
using Resocentro_Desktop.Interfaz.Cartas;
using Resocentro_Desktop.Interfaz.Cobranza;
using Resocentro_Desktop.Interfaz.Facturacion;
using Resocentro_Desktop.Interfaz.frmCarta;
using Resocentro_Desktop.Interfaz.frmCita;
using Resocentro_Desktop.Interfaz.frmUtil;
using Resocentro_Desktop.Interfaz.InformacionTactica;
using Resocentro_Desktop.Interfaz.Sistemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Microsoft.AspNet.SignalR.Client;
using Resocentro_Desktop.Interfaz.Herramienta;
using Resocentro_Desktop.Interfaz.Caja.RegistroCobranza;
using Resocentro_Desktop.Entitys;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using Resocentro_Desktop.Interfaz.Chat;
using Resocentro_Desktop.Complemento;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using Resocentro_Desktop.Interfaz.administracion.Horario;

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para frmMenu.xaml
    /// </summary>
    public partial class frmMenu : Window
    {
        MySession session;
        public static RoutedCommand cartaCommand = new RoutedCommand();
        public static RoutedCommand historiaCommand = new RoutedCommand();
        public static RoutedCommand pagoCommand = new RoutedCommand();
        public RadDesktopAlertManager SelectedDesktopAlertManager { get; set; }
        public System.Threading.Thread Thread { get; set; }
        public string Host = "http://extranet.resocentro.com:5052/";
        public IHubProxy Proxy { get; set; }
        public HubConnection Connection { get; set; }
        public bool Active { get; set; }
        public bool isOpen { get; set; }
        public ClientChat userChat { get; set; }
        public frmMenu()
        {
            InitializeComponent();
            cartaCommand.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control, "Ctrl + K"));
            historiaCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl + H"));
            pagoCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control, "Ctrl + P"));
            DataContext = session;
            userChat = new ClientChat();
            SelectedDesktopAlertManager = new RadDesktopAlertManager(AlertScreenPosition.BottomRight, 5d);
        }
        public async void cargarBuddy()
        {

            if (session.ispathBuddy)
            {
                try
                {
                    System.IO.File.Copy(session.PathBuddy, (System.IO.Path.GetTempPath() + session.codigousuario + ".png"), true);
                    setImagen((System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
                }
                catch (Exception)
                {

                }
                //imgbuddy.Source = new BitmapImage(new Uri(System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
            }
            else
                imgbuddy.Source = new BitmapImage(new Uri("pack://application:,,,/Resocentro_Desktop;component" + session.PathBuddy));




            this.Title = session.shortuser + " | Resocentro - Ver. " + GetPublishedVersion();





        }
        private string GetPublishedVersion()
        {
            string version = null;
            try
            {
                //// get deployment version
                version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (System.Deployment.Application.InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                version = "not installed";
            }
            return version;
        }
        private void setImagen(string file)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(file))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = memoryStream;
                    imageSource.EndInit();
                    // Assign the Source property of your image
                    imgbuddy.Source = imageSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        public void setSession(MySession session)
        {
            this.session = session;
            session.Menu = this;
            session.helper = new FlashWindowHelper(Application.Current);

            MenuUsuario.Content = session.shortuser.Split(' ')[0];
            cargarBuddy();
            if (session.roles.Count() == 0)
            {
                MessageBox.Show("No tiene ningun permiso asignado");
                return;
            }
            //Carta de Garantia
            if (session.roles.Contains("25") || session.roles.Contains("1") || session.roles.Contains("2") || session.roles.Contains("3") || session.roles.Contains("4") || session.roles.Contains("9"))
            {
                MenuCartaGarantia.Visibility = Visibility.Visible;
                if (session.roles.Contains("25"))
                    MenuSolicitudesWeb.Visibility = Visibility.Visible;
                if (session.roles.Contains("1"))
                    MenuListaProforma.Visibility = Visibility.Visible;
                if (session.roles.Contains("2"))
                    MenuNewCarta.Visibility = Visibility.Visible;
                if (session.roles.Contains("3"))
                {
                    MenuRevisarCarta.Visibility = Visibility.Visible;
                    MenuRevisarSedaciones.Visibility = Visibility.Visible;
                }
                if (session.roles.Contains("4"))
                    MenuVisorCarta.Visibility = Visibility.Visible;
                if (session.roles.Contains("9"))
                    MenuHerramientasCarta.Visibility = Visibility.Visible;
            }

            //Atencion al Cliente
            if (session.roles.Contains("11"))
            {
                MenuClente.Visibility = Visibility.Visible;
                if (session.roles.Contains("11"))
                    MenuItemTiketera.Visibility = Visibility.Visible;
                /* if (session.roles.Contains("27"))
                     MenuCita.Visibility = Visibility.Visible;
                 if (session.roles.Contains("28"))
                     MenuConfirmacion.Visibility = Visibility.Visible;
                 if (session.roles.Contains("30"))
                     MenuCentralTel.Visibility = Visibility.Visible;*/
                if (session.roles.Contains("40"))
                {
                    MenuResultados.Visibility = Visibility.Visible;
                    MenuListaEntregaResultados.Visibility = Visibility.Visible;
                }
            }
            //Cobranza
            if (session.roles.Contains("41") || session.roles.Contains("42") || session.roles.Contains("43") || session.roles.Contains("44") || session.roles.Contains("45") || session.roles.Contains("46") || session.roles.Contains("47") || session.roles.Contains("49") || session.roles.Contains("50") || session.roles.Contains("53") || session.roles.Contains("55") || session.roles.Contains("56") || session.roles.Contains("57") || session.roles.Contains("58"))
            {
                MenuCaja.Visibility = Visibility.Visible;
                if (session.roles.Contains("41"))//Realizar Cobranza
                    MenuCobranza.Visibility = Visibility.Visible;
                if (session.roles.Contains("42"))//documentos emitidos
                    MenuDocumentosEmitidos.Visibility = Visibility.Visible;
                if (session.roles.Contains("43"))//cierre caja
                    MenuCierreCaja.Visibility = Visibility.Visible;
                if (session.roles.Contains("44"))//Facturar Copanias
                {
                    MenuItemPreLiquidacion.Visibility = Visibility.Visible;
                    MenuItemFacturacion.Visibility = Visibility.Visible;
                    MenuItemNoFacturado.Visibility = Visibility.Visible;
                }
                if (session.roles.Contains("45"))//Registrar Forma de Pago
                    MenuRegistrarPago.Visibility = Visibility.Visible;
                if (session.roles.Contains("46"))//Registrar Nota Credito Debito
                    MenuItemNotaCreditoDebito.Visibility = Visibility.Visible;
                if (session.roles.Contains("47"))//RESUMEN DiARIO
                    MenuResumen.Visibility = Visibility.Visible;
                if (session.roles.Contains("49"))//Pre Faturacion Global
                    MenuGlobal.Visibility = Visibility.Visible;
                if (session.roles.Contains("50"))//Faturacion Global
                    MenuFacturaGlobal.Visibility = Visibility.Visible;
                if (session.roles.Contains("53"))//Anular Documento
                    MenuBaja.Visibility = Visibility.Visible;

                if (session.roles.Contains("55"))//Registrar Fact. Compania
                    MenuItemFechaCIA.Visibility = Visibility.Visible;

                if (session.roles.Contains("56"))//Generar Caratula
                    MenuItemGenerarCaratula.Visibility = Visibility.Visible;
                if (session.roles.Contains("57"))//FECHA PAGO CIA
                    MenuItemPagoCIA.Visibility = Visibility.Visible;
                if (session.roles.Contains("58"))//Recibo Provisional
                {
                    MenuReciboProvisional.Visibility = Visibility.Visible;
                    MenuListaReciboProvisional.Visibility = Visibility.Visible;
                }
            }
            //Informacion Tactica
            if (session.roles.Contains("51") || session.roles.Contains("52") || session.roles.Contains("54"))
            {
                MenuInfoTactica.Visibility = Visibility.Visible;
                if (session.roles.Contains("51"))
                    MenuItemReporteConta.Visibility = Visibility.Visible;
                if (session.roles.Contains("52"))
                    MenuItemReporteProduc.Visibility = Visibility.Visible;
                if (session.roles.Contains("54"))
                    MenuItemReporteVentas.Visibility = Visibility.Visible;
            }

            //Administracion
            if (session.roles.Contains("33") || session.roles.Contains("34") || session.roles.Contains("35") || session.roles.Contains("36") || session.roles.Contains("38") || session.roles.Contains("65"))
            {
                MenuAdministracion.Visibility = Visibility.Visible;
                //Pago a Personal
                if (session.roles.Contains("33") || session.roles.Contains("34") || session.roles.Contains("35") || session.roles.Contains("36"))
                {
                    MenuPagosPersonal.Visibility = Visibility.Visible;
                    //Anestesiologos
                    if (session.roles.Contains("33"))
                    {
                        MenuPagoAnestesiologo.Visibility = Visibility.Visible;
                    }
                    //Médicos
                    if (session.roles.Contains("34"))
                    {
                        MenuPagoMedico.Visibility = Visibility.Visible;
                    }
                    //Post Procesadores
                    if (session.roles.Contains("35"))
                    {
                        MenuPagoPostprocesadores.Visibility = Visibility.Visible;
                    }
                    //Post Procesadores
                    if (session.roles.Contains("36"))
                    {
                        MenuPagoTecnologo.Visibility = Visibility.Visible;
                        MenuConfirmacionPago.Visibility = Visibility.Visible;
                    }
                }
                //Horario
                if (session.roles.Contains("65") || session.roles.Contains("66") || session.roles.Contains("67") || session.roles.Contains("68") || session.roles.Contains("69"))
                {
                    MenuHorarioPersonal.Visibility = Visibility.Visible;
                    //Tecnologo
                    if (session.roles.Contains("65"))
                    {
                        menuItemHoraio_tecnologo.Visibility = Visibility.Visible;
                    }
                    //Encuestador
                    if (session.roles.Contains("66"))
                    {
                        menuItemHoraio_encuestador.Visibility = Visibility.Visible;
                    }
                    //Supervisor
                    if (session.roles.Contains("67"))
                    {
                        menuItemHoraio_supervisor.Visibility = Visibility.Visible;
                    }
                    //Enfermera
                    if (session.roles.Contains("68"))
                    {
                        menuItemHoraio_enfermera.Visibility = Visibility.Visible;
                    }
                    //PostProcesador
                    if (session.roles.Contains("69"))
                    {
                        menuItemHoraio_Postprocesado.Visibility = Visibility.Visible;
                    }
                }

                if (session.roles.Contains("38"))
                {
                    MenuControlMedico.Visibility = Visibility.Visible;
                }
            }

            if (session.roles.Contains("5"))
            {
                MenuSistemas.Visibility = Visibility.Visible;
            }




        }

        private void MenuSolicitudesWeb_Click(object sender, RoutedEventArgs e)
        {
            frmListaSolicitudesWeb gui = new frmListaSolicitudesWeb();
            gui.iniciarGUI(session);
            gui.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectChat();
            Application.Current.Shutdown();
        }


        private void MenuListaProforma_Click(object sender, RoutedEventArgs e)
        {
            frmListProforma gui = new frmListProforma();
            gui.iniciarGUI(session);
            gui.Show();
        }

        private void MenuRevisarCarta_Click(object sender, RoutedEventArgs e)
        {
            frmRevisionCarta gui = new frmRevisionCarta();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuVisorCarta_Click(object sender, RoutedEventArgs e)
        {
            frmVisorCarta gui = new frmVisorCarta();
            gui.Show();
            gui.cargarGUI(session);
        }
        private void MenuHerramientasCarta_Click(object sender, RoutedEventArgs e)
        {
            frmHerramientasCarta gui = new frmHerramientasCarta();
            gui.Show();
            gui.cargarGUI(session);
        }
        private void MenuCentralTel_Click(object sender, RoutedEventArgs e)
        {
            frmListaCarta gui = new frmListaCarta();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuCita_Click(object sender, RoutedEventArgs e)
        {
            frmCita gui = new frmCita();
            gui.cargarGUI(session);
            gui.Show();
            //gui.cargarLista();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void sendAlert(string titulo, string contenido)
        {
            SelectedDesktopAlertManager.ShowAnimation = new SlideAnimation { Direction = AnimationDirection.In, SlideMode = SlideMode.Bottom, SpeedRatio = 0.3d, PixelsToAnimate = 100 };
            SelectedDesktopAlertManager.HideAnimation = new SlideAnimation { Direction = AnimationDirection.Out, SlideMode = SlideMode.Bottom, SpeedRatio = 0.2d, PixelsToAnimate = 100 };
            SelectedDesktopAlertManager.AlertsDistance = 5;
            SelectedDesktopAlertManager.ShowAlert(new DesktopAlertParameters
            {
                Header = titulo,
                Content = contenido,
                ShowDuration = 10000,
                CanMove = false,
                CanAutoClose = true,
                Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,,/Resocentro_Desktop;component/img/favicon.ico")), Width = 48, Height = 48 },
                IconColumnWidth = 48,
                IconMargin = new Thickness(10, 0, 20, 0),
                ShowMenuButton = false,
                ShowCloseButton = true
            });
        }

        private void MenuItemClave_Click(object sender, RoutedEventArgs e)
        {
            frmCambiarClave gui = new frmCambiarClave();
            gui.cargarGUI(session);
            gui.ShowDialog();
        }

        private void MenuItemCambioImagen_Clik(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.Filter = "Imagenes (.png)|*.png";
            dlg.Multiselect = false;
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                if (session.ispathBuddy)
                    System.IO.File.Delete(Tool.PathImgBuddy + session.codigousuario + ".png");

                System.IO.File.Copy(filename, Tool.PathImgBuddy + session.codigousuario + ".png", true);
                System.IO.File.Copy(session.PathBuddy, (System.IO.Path.GetTempPath() + session.codigousuario + ".png"), true);
                setImagen((System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
                /*
                string extension = System.IO.Path.GetExtension(dlg.SafeFileName).ToLower();
                 Stream stream = dlg.OpenFile();
                 IImageFormatProvider formatProvider = ImageFormatProviderManager.GetFormatProviderByExtension(extension);
                 if (formatProvider != null)
                 {
                     frmCambiarImagen gui = new frmCambiarImagen();
                     gui.radImage.Image = formatProvider.Import(stream);
                     gui.ShowDialog();
                 }*/
            }
        }

        private void MenuVisorCita_Click(object sender, RoutedEventArgs e)
        {
            frmVisorCitas gui = new frmVisorCitas();
            gui.cargarGUI(session);

            gui.Show();
        }

        private void MenuPagoAnestesiologo_Click(object sender, RoutedEventArgs e)
        {
            frmPagoAnestesiolog gui = new frmPagoAnestesiolog();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuPagoMedico_Click(object sender, RoutedEventArgs e)
        {
            frmPagoMedico gui = new frmPagoMedico();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuPagoTecnologo_Click(object sender, RoutedEventArgs e)
        {
            frmPagoTecnologo gui = new frmPagoTecnologo();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuPagoPostprocesadores_Click(object sender, RoutedEventArgs e)
        {
            frmPagoPostProceso gui = new frmPagoPostProceso();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void cartaCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            abrirCarta();
        }

        private void abrirCarta()
        {
            frmCarta gui = new frmCarta();
            gui.cargarGUI(session, false);
            gui.Show();
        }

        private void historiaCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            abrirHistoria();
        }

        private void abrirHistoria()
        {
            frmHistoriaPaciente gui = new frmHistoriaPaciente();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void CommandCarta_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (session.roles.Contains("2"))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void MenuNewCarta_Click(object sender, RoutedEventArgs e)
        {
            abrirCarta();
        }

        private void MenuHistoriaPaciente_Click(object sender, RoutedEventArgs e)
        {
            abrirHistoria();
        }

        private void MenuConfirmacionLlamada_Click(object sender, RoutedEventArgs e)
        {
            frmListaConfirmacion gui = new frmListaConfirmacion();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemNombre_Click(object sender, RoutedEventArgs e)
        {
            frmCambiarNombre gui = new frmCambiarNombre();
            gui.cargarGUI(session);
            gui.ShowDialog();
            MenuUsuario.Content = session.shortuser.Split(' ')[0];
        }

        private void MenuRegistroSedaciones_Click(object sender, RoutedEventArgs e)
        {
            frmRegistroSedaciones gui = new frmRegistroSedaciones();
            gui.cargarGUI(session);
            gui.ShowDialog();
        }

        private void MenuRevisarSedacion_Click(object sender, RoutedEventArgs e)
        {
            frmReporteSedacionesCarta gui = new frmReporteSedacionesCarta();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuAsignacionContraste_Click(object sender, RoutedEventArgs e)
        {
            frmAsignacionContraste gui = new frmAsignacionContraste();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuCobranza_Click(object sender, RoutedEventArgs e)
        {
            if (session.sucursales.ToList().Count > 0)
            {
                frmListaAtenciones gui = new frmListaAtenciones();
                gui.cargarGUI(session);
                gui.Show();
            }
            else
                MessageBox.Show("No tiene sucursales asignadas", "ERROR!");
        }

        private void MenuBaja_Click(object sender, RoutedEventArgs e)
        {
            frmBajaDocumentos gui = new frmBajaDocumentos();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuResumen_Click(object sender, RoutedEventArgs e)
        {
            frmResumenBoletas gui = new frmResumenBoletas();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemFacturacion_Click(object sender, RoutedEventArgs e)
        {
            frmListaFacturacion gui = new frmListaFacturacion();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuResultados_Click(object sender, RoutedEventArgs e)
        {
            frmListaResultado gui = new frmListaResultado();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuEntregaResultados_Click(object sender, RoutedEventArgs e)
        {
            frmListaEntregaResultados gui = new frmListaEntregaResultados();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuCierreCaja_Click(object sender, RoutedEventArgs e)
        {
            frmCierreCaja gui = new frmCierreCaja();
            gui.cargarGUI(session);
            gui.Show();
        }
        private void MenuDocumentosEmitidosa_Click(object sender, RoutedEventArgs e)
        {
            frmDocumentosEmitidos gui = new frmDocumentosEmitidos();
            gui.cargarGUI(session);
            gui.Show();

        }

        private void MenuEmiminarZipFac_Click(object sender, RoutedEventArgs e)
        {
            frmEliminarZipFac gui = new frmEliminarZipFac();
            gui.cargarGUI(session);
            gui.Show();

        }

        private void MenuConfirmacionPago_Click(object sender, RoutedEventArgs e)
        {
            frmConfirmacionPago gui = new frmConfirmacionPago();
            gui.Show();
        }

        private void MenuRegistrarPago_Click(object sender, RoutedEventArgs e)
        {
            frmRegistrarFormaPago gui = new frmRegistrarFormaPago();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemNoFacturado_Click(object sender, RoutedEventArgs e)
        {
            frmRepoteNoFacturado gui = new frmRepoteNoFacturado();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemPreLiquidacion_Click(object sender, RoutedEventArgs e)
        {
            Window1 gui = new Window1();
            gui.Show();
        }

        private void MenuItemNotaCreditoDebito_Click(object sender, RoutedEventArgs e)
        {
            frnNotasdeCreditoDebito gui = new frnNotasdeCreditoDebito();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuGlobal_Click(object sender, RoutedEventArgs e)
        {
            frmMantFacGlobal gui = new frmMantFacGlobal();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuFacturaGlobal_Click(object sender, RoutedEventArgs e)
        {
            frnFacturaGlobal gui = new frnFacturaGlobal();
            gui.cargarGUI(session);
            gui.Show();

        }

        private void MenuItemReporteConta_Click(object sender, RoutedEventArgs e)
        {
            frmReporteContable gui = new frmReporteContable();
            gui.cargarGUI(session);
            gui.Show();

        }

        private void PagoCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (session.sucursales.ToList().Count > 0)
            {
                frmListaAtenciones gui = new frmListaAtenciones();
                gui.cargarGUI(session);
                gui.Show();
            }
            else
                MessageBox.Show("No tiene sucursales asignadas", "ERROR!");
        }

        private void CommandPago_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (session.roles.Contains("41"))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void MenuItemReporteProduc_Click(object sender, RoutedEventArgs e)
        {
            frmReporteProduccion gui = new frmReporteProduccion();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemReporteVentas_Click(object sender, RoutedEventArgs e)
        {
            frmReporteVentas gui = new frmReporteVentas();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemFechaCIA_Click(object sender, RoutedEventArgs e)
        {
            frmRegistarFechaRecepcion gui = new frmRegistarFechaRecepcion();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemGenerarCaratula_Click(object sender, RoutedEventArgs e)
        {
            frmGenearCaraturaFactura gui = new frmGenearCaraturaFactura();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemTiketera_Click(object sender, RoutedEventArgs e)
        {
            frmTicketera gui = new frmTicketera();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuItemPagoCIA_Click(object sender, RoutedEventArgs e)
        {
            frmUploadFacturas gui = new frmUploadFacturas();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuVerificarBaja_Click(object sender, RoutedEventArgs e)
        {
            frmVerificarBaja gui = new frmVerificarBaja();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuListaAtencione_sClick(object sender, RoutedEventArgs e)
        {
            frmReporteAtenciones gui = new frmReporteAtenciones();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuReciboProvisional_Click(object sender, RoutedEventArgs e)
        {
            frmReciboProvisional gui = new frmReciboProvisional();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuEnviarFacturas_Click(object sender, RoutedEventArgs e)
        {
            frmSendFacturas gui = new frmSendFacturas();
            gui.cargarGUI(session);
            gui.Show();
        }


        private void MenuListaReciboProvisional_Click(object sender, RoutedEventArgs e)
        {
            frmListaRecibosProvisionales gui = new frmListaRecibosProvisionales();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void MenuVerificarConexion_Click(object sender, RoutedEventArgs e)
        {
            frmTest gui = new frmTest();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Active = true;
                Thread = new System.Threading.Thread(() =>
                {
                    Connection = new HubConnection(Host);
                    Proxy = Connection.CreateHubProxy("EmetacHub");

                    Proxy.On<string, List<ClientChat>>("updateUsers", (userid, list) => updateUsers(userid, list));
                    Proxy.On<string, string, string, string>("broadcastMessage", (userid, nickname, msj, reciver) => pushMessge(userid, nickname, msj, reciver));
                    Proxy.On<string, string>("notificarusuarios", (tipo, mensaje) => AlertUser(tipo, mensaje));
                    Connection.Start().ContinueWith(x => conectar());

                    while (Active)
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                }) { IsBackground = true };
                Thread.Start();
                resoChat.session = this.session;
                resoChat.menu = this;
                resoChat.cargarGUI();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }
        private void updateUsers(string userid, List<ClientChat> list)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                this.userChat.id = session.codigousuario;
                this.userChat.username = session.shortuser;
                resoChat.RefreshUser(list.Where(x => x.id != userid).OrderBy(x => x.username).ToList());
            }
               ));


        }
        private void pushMessge(string userid, string nickname, string msj, string reciver)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (reciver == userChat.id)
                {
                    resoChat.showChat(userid, nickname, msj, true);
                    if (nickname != session.shortuser)
                        sendAlert("Nuevo Chat", "Conversacion de " + nickname);
                }
            }
               ));


        }
        private void AlertUser(string tipo, string mensaje)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (session.roles.Contains(tipo) || tipo == "0")
                    switch ((Tipo_Permiso)int.Parse(tipo))
                    {
                        case Tipo_Permiso.Todos:
                            sendAlert("Resocentro", mensaje);
                            break;
                        case Tipo_Permiso.Proforma:
                            sendAlert("Proforma", mensaje);
                            break;
                        case Tipo_Permiso.Carta_Garantía:
                            sendAlert("Carta Garantía", mensaje);
                            break;
                        case Tipo_Permiso.Revisar_Carta:
                            sendAlert("Revisar Carta", mensaje);
                            break;
                        case Tipo_Permiso.Sistemas:
                            sendAlert("Sistemas", mensaje);
                            break;
                        case Tipo_Permiso.Generar_Código:
                            sendAlert("Generar Código", mensaje);
                            break;
                        case Tipo_Permiso.Ticketera:
                            sendAlert("Ticketera", mensaje);
                            break;
                        case Tipo_Permiso.Consultas_Web:
                            sendAlert("Consultas Web", mensaje);
                            break;
                        case Tipo_Permiso.Médicos:
                            sendAlert("Médicos", mensaje);
                            break;
                        case Tipo_Permiso.Instituciones_Médicas:
                            sendAlert("Instituciones Médicas", mensaje);
                            break;
                        case Tipo_Permiso.Asignaciones_Médicas:
                            sendAlert("Asignaciones Médicas", mensaje);
                            break;
                        case Tipo_Permiso.Planificación:
                            sendAlert("Planificación", mensaje);
                            break;
                        case Tipo_Permiso.Colaborador:
                            sendAlert("Colaborador", mensaje);
                            break;
                        case Tipo_Permiso.Realizar_Cobranza:
                            sendAlert("Realizar Cobranza", mensaje);
                            break;
                        case Tipo_Permiso.Cierre_Caja:
                            sendAlert("Cierre Caja", mensaje);
                            break;
                        case Tipo_Permiso.Facturar_Compañias:
                            sendAlert("Facturar Compañias", mensaje);
                            break;
                        case Tipo_Permiso.Realizar_Nota_CreditoDebito:
                            sendAlert("Realizar Nota Credito Debito", mensaje);
                            break;
                        case Tipo_Permiso.Anular_Documento:
                            sendAlert("Anular Documento", mensaje);
                            break;
                        case Tipo_Permiso.Recibo_Provisional:
                            sendAlert("Recibo Provisional", mensaje);
                            break;
                        case Tipo_Permiso.Aprobar_Cancelacion_Recibo_Provisional:
                            sendAlert("Aprobar Cancelacion Recibo Provisional", mensaje);
                            break;
                        case Tipo_Permiso.MANT__Empresa_Servicio:
                            sendAlert("Empresa de Servicio", mensaje);
                            break;
                        case Tipo_Permiso.MANT__Incidente:
                            sendAlert("Incidente Equipo", mensaje);
                            break;
                        case Tipo_Permiso.MANT__Equipos:
                            sendAlert("Equpo Biomedico", mensaje);
                            break;
                        case Tipo_Permiso.Asignar_Cortesias:
                            sendAlert("Cortesia", mensaje);
                            break;
                        default:
                            break;
                    }

            }
               ));


        }
        public async void conectar()
        {
            await ConnectChat();
        }
        public async Task ConnectChat()
        {
            try
            {
                await Proxy.Invoke("Connect", session.shortuser, session.codigousuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public async Task DisconnectChat()
        {

            await Proxy.Invoke("Disconnect", userChat.id);
        }



        private void MenuHerramientaSistemas_Click(object sender, RoutedEventArgs e)
        {
            frmUtilitario gui = new frmUtilitario();
            gui.cargarGUI(session);
            gui.Show();
        }


        private void MenuItemHorarioTecnologo_Click(object sender, RoutedEventArgs e)
        {
            abrirPlanificadorTurno(Tipo_Permiso.Horario_Tecnologo);
        }

        private void MenuItemHorarioEncuestador_Click(object sender, RoutedEventArgs e)
        {
            abrirPlanificadorTurno(Tipo_Permiso.Horario_Encuestador);
        }

        private void MenuItemHorarioSupervisor_Click(object sender, RoutedEventArgs e)
        {
            abrirPlanificadorTurno(Tipo_Permiso.Horario_Supervisor);
        }

        private void MenuItemHorarioEnfermera_Click(object sender, RoutedEventArgs e)
        {
            abrirPlanificadorTurno(Tipo_Permiso.Horario_Enfermera);
        }

        private void MenuItemHorarioPostprocesado_Click(object sender, RoutedEventArgs e)
        {
            abrirPlanificadorTurno(Tipo_Permiso.Horario_Postprocesado);
        }

        private void abrirPlanificadorTurno(Tipo_Permiso tipo_Permiso)
        {
            try
            {
                frmHorario_Tecnologo gui = new frmHorario_Tecnologo();
                gui.cargarGUI(session, tipo_Permiso);
                gui.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menuPacientes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmSearchPaciente gui = new frmSearchPaciente();
                gui.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuEmpresa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmSearchCompania gui = new frmSearchCompania();
                gui.cargarGUI(session);
                gui.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuReenviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            frnEnviarCorreofactura gui = new frnEnviarCorreofactura();
            gui.cargarGUI(session);
            gui.Show();
        }


    }
}
