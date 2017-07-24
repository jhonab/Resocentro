using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
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

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmRegistarLlamada.xaml
    /// </summary>
    public partial class frmRegistarLlamada : Window
    {
        MySession session;
        CartaxCitar carta;
        string telefono;
        public bool isCitar;
        public frmRegistarLlamada()
        {
            InitializeComponent();
        }
        private bool validar(out string msj)
        {
            if (!isCitar)
            {
                var checkedButton = stkradiobutton.Children.OfType<RadioButton>()
                                         .FirstOrDefault(r => r.IsChecked.Value);
                msj = "";
                if (checkedButton == null)
                {
                    MessageBox.Show("Debe seleccionar una respuesta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (txtcomentario.Visibility == Visibility.Visible && txtcomentario.Text == "")
                {
                    MessageBox.Show("Debe ingresar un comentario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                msj = checkedButton.Content.ToString() + " " + txtcomentario.Text;
                return true;
            }
            else
            {
                msj = "Se Cito.";
                return true;
            }
        }

        private void btnLlamada_Click(object sender, RoutedEventArgs e)
        {
            isCitar = false;
            insertLlamada();
            DialogResult = true;
        }
        private void btnCitar_Click(object sender, RoutedEventArgs e)
        {
            isCitar = true;
            insertLlamada();
            DialogResult = true;
        }
        private void insertLlamada()
        {
            string msj = "";
            if (validar(out msj))
            {
                AUDITORIA_CARTAS_GARANTIA item = new AUDITORIA_CARTAS_GARANTIA();
                item.codigousuario = session.codigousuario;
                item.numerodecarta = carta.codigocarta;
                item.codigopaciente = carta.idpaciente;
                item.mensaje = msj;
                item.tel_registro = telefono;
                if(!isCitar)
                new CitaDAO().insertRegistroLlamada(item);
                MessageBox.Show("Se registro la Llamada.", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void mostarComentarios(bool isVisible)
        {
            txtcomentario.Text = "";
            if (isVisible)
                txtcomentario.Visibility = Visibility.Visible;
            else
                txtcomentario.Visibility = Visibility.Hidden;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            mostarComentarios(true);
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            mostarComentarios(false);
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            mostarComentarios(true);
        }

        private void RadioButton_Unchecked_1(object sender, RoutedEventArgs e)
        {
            mostarComentarios(false);
        }

        public void setLlamada(MySession session, CartaxCitar item, string telefono)
        {
            this.session = session;
            this.carta = item;
            this.telefono = telefono;
            tbkpaciente.Text = item.paciente;
            if (item.telefono != "" && item.telefono != "0")
                lbltelefono.Content = item.telefono;
            if (item.celular != "" && item.celular != "0")
                lblcelular.Content = item.celular;

            if (item.telefono == telefono)
            {
                lbltelefono.FontSize = 20;
                lbltelefono.FontWeight = FontWeights.Bold;
            }
            else
            {
                lblcelular.FontSize = 20;
                lblcelular.FontWeight = FontWeights.Bold;
            }

            gridLlamadas.ItemsSource = new CitaDAO().getHistorialLlamadas(item.codigocarta, item.idpaciente);
        }
    }
}
