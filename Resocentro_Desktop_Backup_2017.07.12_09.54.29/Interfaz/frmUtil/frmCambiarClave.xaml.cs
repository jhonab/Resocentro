using Resocentro_Desktop.DAO;
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
using System.Windows.Shapes;

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmCambiarClave.xaml
    /// </summary>
    public partial class frmCambiarClave : Window
    {
        MySession session;
        public frmCambiarClave()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtclave.Password != "" && txtnewclave.Password != "" && txtconfirmarclave.Password != "")
            {
                if (txtconfirmarclave.Password == txtnewclave.Password)
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var _usuario = db.USUARIO.SingleOrDefault(x => x.contrasena == txtclave.Password && x.siglas == session.siglas);
                        if (_usuario != null)
                        {
                            _usuario.contrasena = txtnewclave.Password;
                            db.SaveChanges();
                            MessageBox.Show("Se cambio de contraseña", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                        }
                        else
                            MessageBox.Show("La contraseña es erronea", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                    MessageBox.Show("La nueva contraseña no conincide", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Ingrese la contraseña", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
