using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz;
using Resocentro_Desktop.Interfaz.administracion;
using Resocentro_Desktop.Interfaz.AtencionCliente;
using Resocentro_Desktop.Interfaz.Caja;
using Resocentro_Desktop.Interfaz.Caja.impresion;
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
using Telerik.Windows.Media.Imaging.FormatProviders;

using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
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


        public frmMenu()
        {
            InitializeComponent();
            cartaCommand.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control, "Ctrl + K"));
            historiaCommand.InputGestures.Add(new KeyGesture(Key.H, ModifierKeys.Control, "Ctrl + H"));
            pagoCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control, "Ctrl + P"));
            DataContext = session;
        }
        public void cargarBuddy()
        {

            if (session.ispathBuddy)
            {
                System.IO.File.Copy(session.PathBuddy, (System.IO.Path.GetTempPath() + session.codigousuario + ".png"), true);
                setImagen((System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
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
            MenuUsuario.Content = session.shortuser.Split(' ')[0];

            cargarBuddy();
            //Carta de Garantia
            if (session.roles.Contains("25") || session.roles.Contains("1") || session.roles.Contains("2") || session.roles.Contains("3") || session.roles.Contains("4"))
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
                     MenuCentralTel.Visibility = Visibility.Visible;
                 if (session.roles.Contains("40"))
                     MenuResultados.Visibility = Visibility.Visible;*/
            }
            //Cobranza
            if (session.roles.Contains("41") || session.roles.Contains("42") || session.roles.Contains("43") || session.roles.Contains("44") || session.roles.Contains("45") || session.roles.Contains("46") || session.roles.Contains("47") || session.roles.Contains("49") || session.roles.Contains("50") || session.roles.Contains("53") || session.roles.Contains("55") || session.roles.Contains("56") || session.roles.Contains("57"))
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
            if (session.roles.Contains("33") || session.roles.Contains("34") || session.roles.Contains("38"))
            {
                MenuAdministracion.Visibility = Visibility.Visible;
                //Pago a Personal
                if (session.roles.Contains("33") || session.roles.Contains("34"))
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








    }
}
