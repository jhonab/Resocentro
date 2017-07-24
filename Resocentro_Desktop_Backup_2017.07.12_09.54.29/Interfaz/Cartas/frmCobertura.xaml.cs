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
    /// Lógica de interacción para frmCobertura.xaml
    /// </summary>
    public partial class frmCobertura : Window
    {
        public Search_Estudio estudio;
        public frmCobertura()
        {
            InitializeComponent();
            txtcobertura.SelectAll();
            txtcobertura.Focus();
        }
        public void iniciarGUI(Search_Estudio estudio)
        {
            this.estudio = estudio;
            txtpreciobase.Value = estudio.precio;
            txtcobertura.Value = estudio.cobertura;
            txtmontocobertura.Value = estudio.descuento;
            txtcobertura.SelectAll();
            txtcobertura.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            guardar();
        }

        private void guardar()
        {
            estudio.precio = txtpreciobase.Value.Value;
            estudio.cobertura = txtcobertura.Value.Value;
            estudio.descuento = txtmontocobertura.Value.Value;
            estudio.isEditable = true;
            DialogResult = true;
        }

        private void txtcobertura_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                guardar();
        }

        private void txtcobertura_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (estudio != null)
            {
                double newvalue = e.NewValue.Value;
                var cobertura = newvalue;
                var des = ((cobertura * estudio.precio) / 100) * -1.0;
                estudio.cobertura = cobertura;
                estudio.descuento = des;
                txtmontocobertura.Value = des;
            }
        }

        private void txtmontocobertura_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtmontocobertura_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            double newvalue = e.NewValue.Value;
            if (newvalue > 0)
                newvalue *= -1;
            txtmontocobertura.Value = newvalue;
        }

        private void txtpreciobase_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void txtpreciobase_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (estudio != null)
            {
                estudio.precio = e.NewValue.Value;
                txtmontocobertura.Value = estudio.precio * (estudio.cobertura / 100);
            }
        }
    }
}
