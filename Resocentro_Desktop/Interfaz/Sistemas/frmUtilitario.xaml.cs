using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.frmUtil;
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

namespace Resocentro_Desktop.Interfaz.Sistemas
{
    /// <summary>
    /// Lógica de interacción para frmUtilitario.xaml
    /// </summary>
    public partial class frmUtilitario : Window
    {
        MySession session;
        public frmUtilitario()
        {
            InitializeComponent();
        }

        private void txtnum_examen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscardataestudio();
            }
        }

        private void buscardataestudio()
        {
            var lista = new SistemasDAO().getInfoChangeEstudio(txtnum_examen.Text);
            limpiarData();
            foreach (var item in lista)
            {
                if (item.Key == "unidad")
                {
                    txtunidad.Text = item.Value;
                    cboModalidad.ItemsSource = new UtilDAO().getModalidadxEmpresa(item.Value.Substring(0, 1));
                    cboModalidad.SelectedValuePath = "codigo";
                    cboModalidad.DisplayMemberPath = "nombre";
                    cboModalidad.SelectedIndex = 0;
                }
                if (item.Key == "paciente")
                    txtpaciente.Text = item.Value;
                if (item.Key == "nombreestudio")
                    txtnombre_estudio_old.Text = item.Value;
                if (item.Key == "codigocompaniaseguro")
                    txtaseguradora.Text = item.Value;
            }
        }

        private void limpiarData()
        {
            txtunidad.Text = "";
            cboModalidad.ItemsSource = null;
            txtpaciente.Text = "";
            txtnombre_estudio_old.Text = "";
            txtaseguradora.Text = "";
            txtid_estudio_new.Text = "";
            txtnombre_estudio_new.Text = "";
            btncambiar_Estudio.IsEnabled = false;
        }

        private void BtnBuscarEstudio_Click(object sender, RoutedEventArgs e)
        {
            var sucursal = txtunidad.Text;
            var modalidad = cboModalidad.SelectedValue.ToString();
            var aseguradora = txtaseguradora.Text;
            if (sucursal != "" && modalidad != "")
            {
                frmSearchEstudio gui = new frmSearchEstudio();
                gui.ampliacion = "0";
                gui.modalidad = modalidad;
                gui.sucursal = sucursal;
                gui.Owner = this;
                gui.getClase();
                gui.ShowDialog();
                if (gui.estudio != null)
                {
                    txtid_estudio_new.Text = gui.estudio.codigoestudio;
                    txtnombre_estudio_new.Text = gui.estudio.estudio;
                    btncambiar_Estudio.IsEnabled = true;
                }
            }
        }

        private void btnCambiarEstudio_Click(object sender, RoutedEventArgs e)
        {
            if (new SistemasDAO().cambiarEstudioxCodigo(txtnum_examen.Text, txtid_estudio_new.Text))
            {
                MessageBox.Show("El cambio se realizo correctamente");
                buscardataestudio();
            }
            else
                MessageBox.Show("No se realizo el cambio");
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }

        private async void btnEnviarnotificacion_Click(object sender, RoutedEventArgs e)
        {
            await session.Menu.Proxy.Invoke("notificarusuarios", txttiponotificacion.Text, txtmensajenotificaion.Text);
        }
    }
}
