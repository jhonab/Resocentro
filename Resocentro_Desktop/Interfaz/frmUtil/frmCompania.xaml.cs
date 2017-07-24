using Resocentro_Desktop.DAO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmCompania.xaml
    /// </summary>
    public partial class frmCompania : Window
    {
        public ASEGURADORA item { get; set; }
        public bool isUpdate { get; set; }
        public frmCompania()
        {
            InitializeComponent();
            item = new ASEGURADORA();
            item.telefono = "00";
            item.fax = "00";
            item.email = "00";
            item.web = "00";
            isUpdate = false;
            this.DataContext = new { item = item };
            this.wbsunat.Navigate("http://www.sunat.gob.pe/cl-ti-itmrconsruc/FrameCriterioBusquedaMovil.jsp");
            //this.wbsunat.Navigate("http://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        public void setAseguradora(ASEGURADORA data)
        {
            item = data;
            isUpdate = true;
            this.DataContext = new { item = item };
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (item.ruc != null && item.razoncomercial != null && item.razonsocial != null && item.telefono != null && item.domiciliofiscal != null)
            {
                if (CobranzaDAO.ValidarRUC(item.ruc))
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        try
                        {
                            if (isUpdate)
                            {
                                var aseguradora = db.ASEGURADORA.SingleOrDefault(x => x.ruc == item.ruc);
                                if (aseguradora != null)
                                {
                                    aseguradora.razonsocial = item.razonsocial;
                                    aseguradora.razoncomercial = item.razoncomercial;
                                    aseguradora.telefono = item.telefono;
                                    aseguradora.fax = item.fax;
                                    aseguradora.email = item.email;
                                    aseguradora.web = item.web;
                                    aseguradora.domiciliocomercial = item.domiciliocomercial;
                                    aseguradora.domiciliofiscal = item.domiciliofiscal;
                                    db.SaveChanges();
                                    DialogResult = true;
                                }
                            }
                            else
                            {
                                var lista = db.ASEGURADORA.Where(x => x.ruc == item.ruc).ToList();
                                if (lista.Count == 0)
                                {
                                    db.ASEGURADORA.Add(item);
                                    db.SaveChanges();
                                    DialogResult = true;
                                }
                                else
                                    MessageBox.Show("El RUC ya esta registrado", "ADVERTENCIA");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }


                    }
                }
                else
                    MessageBox.Show("El RUC ingresado no es valido", "ADVERTENCIA");
            }
            else
                MessageBox.Show("Ingrese los todos los datos", "ERROR");
        }
        //CONSULTAR SUNAT
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //refreshCapcha();
        }

        private void refreshCapcha()
        {
            //imgCapcha.Source = obtieneImagenCaptcha();
           // txtCapcha.Text = "";
        }

        private void RadRadioButton_Click(object sender, RoutedEventArgs e)
        {
            this.wbsunat.Navigate("http://www.sunat.gob.pe/cl-ti-itmrconsruc/FrameCriterioBusquedaMovil.jsp");
        }

        private void RadRadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.wbsunat.Navigate("http://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS00Alias");
        }


        #region ONLINE SUNAT
       /* private CookieContainer miCookie { get; set; }
        private BitmapImage obtieneImagenCaptcha()
        {
            BitmapImage bitmap = new BitmapImage();
            HttpWebRequest UrlCaptcha = WebRequest.Create("http://www.sunat.gob.pe/cl-ti-itmrconsruc/captcha?accion=image") as HttpWebRequest;
            UrlCaptcha.CookieContainer = miCookie;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            UrlCaptcha.Credentials = CredentialCache.DefaultCredentials;
            WebResponse Captcha = UrlCaptcha.GetResponse();
            Stream imgCaptchaBinario = Captcha.GetResponseStream();
            Byte[] buffer = new Byte[Captcha.ContentLength];
            using (MemoryStream byteStream = new MemoryStream(buffer))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = byteStream;
                bi.EndInit();

                byteStream.Close();
            }
            return bitmap;

        }*/
        #endregion
    }
}
