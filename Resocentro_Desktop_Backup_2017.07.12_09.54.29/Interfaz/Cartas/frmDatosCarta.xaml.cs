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

namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmDatosCarta.xaml
    /// </summary>
    public partial class frmDatosCarta : Window
    {
        public CARTAGARANTIA carta;
        public frmDatosCarta()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            carta = new CARTAGARANTIA();
            carta.codigocartagarantia2 = txtnumeroCarta.Text;
            carta.estadocarta = cboestado.Text;
            carta.cobertura = float.Parse(txtcobertura.Value.Value.ToString());
            DialogResult = true;
        }
    }
}
