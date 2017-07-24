using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
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

namespace Resocentro_Desktop.Interfaz.AtencionCliente
{
    /// <summary>
    /// Lógica de interacción para frmEntregarResultado.xaml
    /// </summary>
    public partial class frmEntregarResultado : Window
    {
        ResultadosEntity item;
        MySession session;
        public frmEntregarResultado()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, ResultadosEntity item)
        {
            this.Title = "Entregar Resultado ";
            lblestudio.Content = item.paciente.ToUpper();
            txtentregado.Text = item.paciente;
            txtdocumento.Text = item.dni;
            txtdireccion.Text = item.direccion;
            this.session = session;
            this.item = item;
            item.firma = null;
            listarestudios();

        }

        public void setItem(ResultadosEntity item)
        {
            this.Title = "Entregar Resultado ";
            lblestudio.Content = "";
            txtentregado.Text = item.paciente;
            txtdocumento.Text = "";
            txtdireccion.Text = item.direccion;
            if (item.foto > 0 || item.placa > 0)
            {
                lblplaca.Visibility = Visibility.Visible;
                lblfotos.Visibility = Visibility.Visible;
                txtplacas.Visibility = Visibility.Visible;
                txtfotos.Visibility = Visibility.Visible;

                txtfotos.Text = item.foto.ToString();
                txtplacas.Text = item.placa.ToString();
            }
            else
            {
                if (item.numerodelivery != 0 && item.estudios.Count() > 0)
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        foreach (var estudio in db.DetalleResultadoDelivery.Where(x => x.idResultado == item.numerodelivery).ToList())
                        {
                            var est = item.estudios.SingleOrDefault(x => x.codigo == estudio.examen);
                            if (est != null)
                            {
                                est.foto = estudio.fotos;
                                est.placa = estudio.placas;
                            }
                        }
                    }
            }
            txtObs.Text = item.observaciones;
            if (item.firma != null)
            {
                var image = new BitmapImage();
                using (var mem = new MemoryStream(item.firma))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                imgfirma.Source = image;
            }
            this.item = item;
            listarestudios();
            this.Height = 600;
            this.IsEnabled = false;
        }

        private void listarestudios()
        {
            gridestudios.ItemsSource = null;
            gridestudios.ItemsSource = item.estudios;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    try
                    {
                        RESULTADODELIVERY re = new RESULTADODELIVERY();
                        re.numeroatencion = item.numeroatencion;
                        re.codigopaciente = item.codigopaciente;
                        re.codigodistrito = item.codigopaciente;
                        re.direcciondelivery = txtdireccion.Text;
                        re.personacontacto = txtentregado.Text;
                        re.telefonodelivery = item.telefono;
                        re.cantidadfoto = int.Parse(txtfotos.Text);
                        re.cantidadplaca = int.Parse(txtplacas.Text);
                        re.fechasalida = Tool.getDatetime();
                        re.entregado = true;
                        re.observacion = txtObs.Text;
                        re.horasalida = re.fechasalida;
                        re.tipoentrega = "Normal";
                        re.codigousuario = session.codigousuario;
                        re.firma = item.firma;
                        db.RESULTADODELIVERY.Add(re);
                        foreach (ListaEstudiosResultados estudio in gridestudios.Items.SourceCollection)
                        {
                            DetalleResultadoDelivery dr = new DetalleResultadoDelivery();
                            dr.idResultado = re.numerodelivery;
                            dr.examen = estudio.codigo;
                            dr.placas = estudio.placa;
                            dr.fotos = estudio.foto;
                            db.DetalleResultadoDelivery.Add(dr);
                        }
                        db.SaveChanges();
                        new AtencionClienteDAO().setEntregadoPaciente(item.numeroatencion);
                        DialogResult = true;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        DialogResult = false;
                    }

                }
            }
        }

        private bool validar()
        {
            bool result = true;
            if (txtentregado.Text == "")
                result = false;
            if (txtdocumento.Text == "")
                result = false;
            if (txtdireccion.Text == "")
                result = false;
            foreach (ListaEstudiosResultados estudio in gridestudios.Items.SourceCollection)
            {
                if (estudio.foto == 0 && estudio.placa == 0)
                    result = false;
            }
            if (!result)
                MessageBox.Show("Verifique los datos ingresados");
            return result;
        }

        private void gridestudios_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            if (MessageBox.Show("Desea quitar el estudio de la lista de entrega?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ListaEstudiosResultados re = (ListaEstudiosResultados)e.Row.DataContext;
                item.estudios.Remove(re);
                listarestudios();
            }

        }

        private void bntFirma_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                captureSignatureHID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void captureSignatureHID()
        {
            wgssSTU.SerialInterface serialInterface = new wgssSTU.SerialInterface();
            wgssSTU.UsbDevices usbDevices = new wgssSTU.UsbDevices();

            if (usbDevices.Count != 0)
            {
                try
                {
                    wgssSTU.IUsbDevice usbDevice = usbDevices[0]; // select a device

                    frmFirmaDigital demo = new frmFirmaDigital(this, usbDevice, serialInterface, "", "", true);
                    var res = demo.ShowDialog();
                    //print("SignatureForm returned: " + res.ToString());
                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        DisplaySignature(demo);
                    }

                    demo.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No STU devices attached");
            }
        }
        private void DisplaySignature(frmFirmaDigital demo)
        {
            System.Drawing.Bitmap bitmap;
            bitmap = demo.GetSigImage();
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                item.firma = ms.ToArray();
            }
        }

    }
}




