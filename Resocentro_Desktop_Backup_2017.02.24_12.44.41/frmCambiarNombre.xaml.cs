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

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para frmCambiarNombre.xaml
    /// </summary>
    public partial class frmCambiarNombre : Window
    {
        MySession session;
        public frmCambiarNombre()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            txtnombre.Text = session.shortuser;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            guardarNombre();
        }

        private void guardarNombre()
        {
            if (txtnombre.Text != "")
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var usuario = db.USUARIO.SingleOrDefault(x => x.codigousuario == session.codigousuario);
                    if (usuario != null)
                    {
                        session.shortuser = txtnombre.Text;
                        usuario.ShortName = txtnombre.Text;
                        db.SaveChanges();
                    }
                    this.DialogResult = true;
                }
            }
            else
            {
                MessageBox.Show("Ingrese un Nombre y Apellido", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtnombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                guardarNombre();
            }
        }
    }
}
