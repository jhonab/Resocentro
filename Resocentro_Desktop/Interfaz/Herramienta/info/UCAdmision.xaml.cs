using Resocentro_Desktop.Entitys;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Resocentro_Desktop.Interfaz.Herramienta.info
{
    /// <summary>
    /// Lógica de interacción para UCAdmision.xaml
    /// </summary>
    public partial class UCAdmision : UserControl
    {
        public UCAdmision()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            
            var button = (Button)sender;
            if (button != null)
            {
                var data = (DataAdmision)button.DataContext;
                var ruta = System.IO.Path.GetTempPath() + data.Nombrearchivo;
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
                byte[] cuerpo = (byte[])data.Cuerpoarchivo;
                System.IO.FileStream archivo = new System.IO.FileStream(ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                archivo.Write(cuerpo, 0, cuerpo.Length);
                archivo.Close();
                System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                d.FileName = ruta;
                //d.Verb = "pdf";
                System.Diagnostics.Process.Start(d);
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
