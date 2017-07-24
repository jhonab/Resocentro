using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.frmCarta;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Web;


namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            string[] activationData ="-,-".Split(',');
            // Application is running
            // Process command line args


           


            char[] myComma = { ',' };
            /*foreach (string arg in activationData)
            {
                string[] myList = activationData[0].Split(myComma,
                  StringSplitOptions.RemoveEmptyEntries);
                foreach (string oneItem in myList)
                    System.Diagnostics.Debug.Print("Item = {0}", oneItem);
            }*/


            string[] myParameters = activationData[0].Split(myComma,
                  StringSplitOptions.RemoveEmptyEntries);

            string usuario = "", clave = "", codigocarta = "", codigopaciente = "";
            bool iscarta = false;
            foreach (string arg in myParameters)
            {
                if (arg.Contains("user/"))
                    usuario = arg.Split('/')[1];
                if (arg.Contains("clave/"))
                    clave = arg.Split('/')[1];
                if (arg.Contains("carta/"))
                {
                    iscarta = true;
                    codigocarta = arg.Split('/')[1];
                }
                if (arg.Contains("paciente/"))
                    codigopaciente = arg.Split('/')[1];
            }
            /*
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i].Contains("user/"))
                    usuario = e.Args[i].Split('/')[1];
                if (e.Args[i].Contains("clave/"))
                    clave = e.Args[i].Split('/')[1];
                if (e.Args[i].Contains("carta/"))
                {
                    iscarta = true;
                    codigocarta = e.Args[i].Split('/')[1];
                }
                if (e.Args[i].Contains("paciente/"))
                    codigopaciente = e.Args[i].Split('/')[1];

            }
            */
            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.siglas.Text = usuario;
            mainWindow.codigousuario.Password = clave;
            if (usuario != "" && clave != "")
            {
                mainWindow.login();
                MySession session = mainWindow.session;
                if (iscarta && codigopaciente != "")
                {
                    var _cart = new CartaDAO().getCartaxCodigo(codigocarta, int.Parse(codigopaciente));
                    var detalle = new CartaDAO().getDetalleCartaxCodigo(codigocarta, int.Parse(codigopaciente));
                    if (_cart != null)
                    {
                        frmCarta gui = new frmCarta();
                        gui.cargarGUI(session, true);
                        gui.Show();
                        new CartaDAO().insertLog(codigocarta, session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + codigocarta);
                        gui.setCartaGarantia(_cart, detalle, false);
                    }
                }
            }
        }
    }
}
