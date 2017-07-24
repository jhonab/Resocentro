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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmDevolucionDineroRecibo.xaml
    /// </summary>
    public partial class frmDevolucionDineroRecibo : Window
    {
        public double monto=0;
        public frmDevolucionDineroRecibo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            monto = txtmonto.Value.Value;
            DialogResult = true;
        }
    }
}
