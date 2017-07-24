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

namespace Resocentro_Desktop.Interfaz.Cartas
{
    /// <summary>
    /// Lógica de interacción para frmHerramientasCarta.xaml
    /// </summary>
    public partial class frmHerramientasCarta : Window
    {
        MySession session;
        public frmHerramientasCarta()
        {
            InitializeComponent();
        }

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            int cita = 0, paciente = 0;
            if (int.TryParse(txtcita.Text, out cita) && int.TryParse(txtpaciente.Text, out paciente) )//&& txtcarta.Text!="")
            {
                try
                {
                    ((Button)sender).IsEnabled = false;
                    if (txtresultados.Text != "")
                        txtresultados.Text += "\n";
                    txtresultados.Text += "Se inicio el cambio de carta... espere";
                    new CartaDAO().CambiarCartaxCita(cita, txtcarta.Text, paciente).ContinueWith(x =>
                    {
                        if (x.Result == true)
                            txtresultados.Text += "\n¡¡¡¡Se realizo el cambio correctamente  a la cita " + cita.ToString() + "!!!!";
                        else
                            txtresultados.Text += "\nVerifique los datos y vuelva a intentar nuevamente***";

                        ((Button)sender).IsEnabled = true;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (Exception ex)
                {
                    txtresultados.Text += "\nVerifique los datos y vuelva a intentar nuevamente***";
                    MessageBox.Show(ex.Message);
                }
               

            }
            else
                txtresultados.Text += "\nVerifique los datos ingresados***";
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }
    }
}
