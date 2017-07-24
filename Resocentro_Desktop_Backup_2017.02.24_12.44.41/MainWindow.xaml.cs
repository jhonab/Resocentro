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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MySession session;
        List<string> Parametros = new List<string>();
        public MainWindow()
        {
            
            //StyleManager.ApplicationTheme = new Office2016Theme()//new SummerTheme();
            
            InitializeComponent();
           /*  for (int index = 1; index < args.Length; index += 2)
            {
                Parametros.Add(args[index + 1]);
            }
             siglas.Text = Parametros[0];
             codigousuario.Password = Parametros[0];*/
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login();
        }

        public void login()
        {
            lblerror.Visibility = Visibility.Hidden;
            string _siglas =siglas.Text,codigo=codigousuario.Password.ToString();
            if (_siglas != "" && codigo != "")
            {
                AccountDAO dao = new AccountDAO();
                session = dao.Login(_siglas, codigo);
                if (session.isLogon)
                {
                    frmMenu gui = new frmMenu();
                    gui.setSession(session);
                    this.Close();
                    gui.Show();
                }
                else
                {
                    lblerror.Content = "Usuario y/o Contraseña son incorrectos";
                    lblerror.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lblerror.Content = "Ingrese el Usuario y Contraseña";
                lblerror.Visibility = Visibility.Visible;
            }
        }

        private void codigousuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                login();
        }

        private void codigousuario_GotFocus(object sender, RoutedEventArgs e)
        {
            codigousuario.SelectAll();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
