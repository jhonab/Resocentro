using Microsoft.Win32;

using Resocentro_Desktop.DAO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
                    setVaariablesGlobales();
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

        private void setVaariablesGlobales()
        {
            var cantidad_decimales = 0;
            string CompaniasNOAfectasTEM = "";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string query = "select valor from table_config where id=2";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            cantidad_decimales = Convert.ToInt32((reader["valor"]).ToString());
                        }

                        query = "select valor from table_config where id=1";
                        command.CommandText = query;
                        reader.Close();
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            CompaniasNOAfectasTEM = (reader["valor"]).ToString();
                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }

            // Open App.Config of executable
            System.Configuration.Configuration config =ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Add an Application Setting.
            config.AppSettings.Settings.Add("cantidadDecimales",cantidad_decimales.ToString());
            config.AppSettings.Settings.Add("CompaniasNOAfectasTEM", CompaniasNOAfectasTEM.ToString());
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
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
