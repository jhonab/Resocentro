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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Resocentro_Desktop.Interfaz.Herramienta.info
{
    /// <summary>
    /// Lógica de interacción para UCCarta.xaml
    /// </summary>
    public partial class UCCarta : UserControl
    {
        public UCCarta()
        {
            InitializeComponent();
        }

        private void grid_adjuntos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Adjuntos_Desktop item = (Adjuntos_Desktop)e.Row.DataContext;
            if (item != null)
            {
                try
                {
                    if (!item.isFisico)
                    {
                        item.ruta = System.IO.Path.GetTempPath() + item.nombre;
                        if (System.IO.File.Exists(item.ruta))
                        {
                            System.IO.File.Delete(item.ruta);
                        }
                        System.IO.FileStream archivo = new System.IO.FileStream(item.ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                        archivo.Write(item.archivo, 0, item.archivo.Length);
                        archivo.Close();
                        System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                        d.FileName = item.ruta;
                        //d.Verb = "pdf";
                        System.Diagnostics.Process.Start(d);
                    }
                    else
                    {
                        if (File.Exists(item.ruta))
                        {
                            string ruta = System.IO.Path.GetTempPath() + item.nombre;
                            try
                            {
                                if (File.Exists(ruta))
                                    File.Delete(ruta);
                                File.Copy(item.ruta, ruta);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                            }
                            System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                            d.FileName = ruta;
                            //d.Verb = "pdf";
                            System.Diagnostics.Process.Start(d);
                        }
                        else
                            MessageBox.Show("El archivo esta dañado o no existe.\n" + item.ruta.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
