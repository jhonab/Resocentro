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

namespace Resocentro_Desktop.Interfaz
{
    /// <summary>
    /// Lógica de interacción para frmSetPrecioFacGlobal.xaml
    /// </summary>
    public partial class frmSetPrecioFacGlobal : Window
    {
        public int moneda = 1;
        public double precio = 0;
        public frmSetPrecioFacGlobal()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbomoneda.SelectedIndex != 0 && txtprecio.Value.Value > 0)
            {
                moneda = cbomoneda.SelectedIndex;
                precio = txtprecio.Value.Value;
                DialogResult = true;
            }
            else
                MessageBox.Show("Seleccione el Tipo de Moneda y asigne el Precio");
        }
    }
}
