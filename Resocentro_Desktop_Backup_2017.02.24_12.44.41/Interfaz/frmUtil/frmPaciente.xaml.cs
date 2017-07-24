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

namespace Resocentro_Desktop.Interfaz.frmUtil
{
    /// <summary>
    /// Lógica de interacción para frmPaciente.xaml
    /// </summary>
    public partial class frmPaciente : Window
    {
        bool isupdate = false;
        public PACIENTE paciente;
        public frmPaciente()
        {
            InitializeComponent();
            dtpnace.SelectedDate = DateTime.Now;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                cbotipo_doc.ItemsSource = db.TIPO_DOCUMENTO_IDENTIDAD.ToList();
                cbotipo_doc.SelectedValuePath = "tipo_doc_id";
                cbotipo_doc.DisplayMemberPath = "tipo_doc_descripcion";
                cbotipo_doc.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (valido())
            {
                PACIENTE pac = getPaciente();
                if (isupdate)
                {
                    new PacienteDAO().updatePaciente(pac);
                    DialogResult = true;
                }
                else
                {
                    if (new PacienteDAO().validarPaciente(pac.dni, pac.tipo_doc))
                    {
                        new PacienteDAO().addPaciente(pac);
                        paciente = pac;
                        DialogResult = true;
                    }
                    else
                        MessageBox.Show("En número de documento ya esta registrado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void insertPaciente(PACIENTE pac)
        {
            txtnombre.Text = pac.nombres;
            txtapellido.Text = pac.apellidos;
            txttelefono.Text = pac.telefono;
            txtcelular.Text = pac.celular;
            txtemail.Text = pac.email;
            cbotipo_doc.SelectedIndex = 0;
            txtnumero_doc.Text = pac.dni;

        }
        private bool valido()
        {
            string msj = "";
            int numero = 0;
            if (cbotipo_doc.SelectedIndex == -1)
                msj += "- Seleccione el Tipo de Documento\n";

            switch (int.Parse(cbotipo_doc.SelectedValue.ToString()))
            {
                case 0:

                    if (txtnumero_doc.Text.Length != 8)
                        msj += "- El DNI debe contener 8 números\n";
                    else if (!int.TryParse(txtnumero_doc.Text, out numero))
                        msj += "- Ingrese un Número de DNI\n";
                    break;
                case 2:
                    if (txtnumero_doc.Text.Length != 11)
                        msj += "- El RUC debe contener 11 números\n";
                    else if (!int.TryParse(txtnumero_doc.Text, out numero))
                        msj += "- Ingrese un Número de RUC\n";
                    break;
                case 3:
                    if (txtnumero_doc.Text.Length != 12)
                        msj += "- El PASAPORTE debe contener hasta 12 caracteres alfanuméricos\n";
                    break;
                default:
                    break;
            }
            if (txtapellido.Text == "")
                msj += "- Ingrese los Apellidos\n";
            if (txtnombre.Text == "")
                msj += "- Ingrese los Nombres\n";
            if (txttelefono.Text == "")
                msj += "- Ingrese el Teléfono\n";
            if (cbosexo.Text == "")
                msj += "- Seleccione el Sexo\n";
            if (dtpnace.SelectedDate == null)
                msj += "- Seleccione la Fecha de Nacimiento\n";
            if (txtnacionalidad.Text == "")
                msj += "- Ingrese la Nacionalidad\n";
            if (txtemail.Text != "")
            {
                if (!new Tool().IsValidEmail(txtemail.Text))
                    msj += "- Verifique el correo ingresado.\n";
            }

            if (msj != "")
            {
                MessageBox.Show("Verifique los siguientes errores:\n" + msj, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;


        }
        public PACIENTE getPaciente()
        {
            PACIENTE p = new PACIENTE();
            if (txtcodigo.Text != "")
                p.codigopaciente = int.Parse(txtcodigo.Text);
            p.nombres = txtnombre.Text;
            p.apellidos = txtapellido.Text;
            p.fechanace = dtpnace.SelectedDate.Value;
            p.tipo_doc = cbotipo_doc.SelectedIndex.ToString();
            p.dni = txtnumero_doc.Text;
            p.sexo = cbosexo.Text.Substring(0, 1); ;
            p.telefono = txttelefono.Text;
            p.celular = txtcelular.Text;
            p.direccion = txtdireccion.Text;
            p.email = txtemail.Text;
            p.nacionalidad = txtnacionalidad.Text;
            p.IsProtocolo = cbkprotocolo.IsChecked;
            return p;
        }
        public void isLector()
        {
            btnguardar.Visibility = Visibility.Collapsed;
            this.IsEnabled = false;
        }
        public void setPaciente(PACIENTE p)
        {
            isupdate = true;
            txtcodigo.Text = p.codigopaciente.ToString();
            txtnombre.Text = p.nombres;
            txtapellido.Text = p.apellidos;
            dtpnace.Text = p.fechanace.ToShortDateString();
            cbosexo.Text = p.sexo == "M" ? "Masculino" : "Femenino";
            txttelefono.Text = p.telefono;
            txtcelular.Text = p.celular;
            txtdireccion.Text = p.direccion;
            txtemail.Text = p.email;
            txtnacionalidad.Text = p.nacionalidad;
            cbotipo_doc.SelectedIndex = int.Parse(p.tipo_doc);
            txtnumero_doc.Text = p.dni;
            cbkprotocolo.IsChecked = p.IsProtocolo;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)e.Source;
            wbPaciente.Navigate(link.NavigateUri);
            wbPaciente.Visibility = Visibility.Visible;
            this.Height = 750;

        }
    }
}
