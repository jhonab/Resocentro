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

namespace Resocentro_Desktop.Interfaz.Cobranza
{
    /// <summary>
    /// Lógica de interacción para frmSeleccionarModalidad.xaml
    /// </summary>
    public partial class frmSeleccionarModalidad : Window
    {
        public string modalidad = "";
        public int empresa = 1;
        public frmSeleccionarModalidad()
        {
            InitializeComponent();
        }
        public void cargarGUI( int empresa)
        {
            this.empresa = empresa;
            cboModalidad.ItemsSource = new UtilDAO().getModalidadxEmpresa(empresa.ToString());
            cboModalidad.SelectedValuePath = "codigo";
            cboModalidad.DisplayMemberPath = "nombre";
            cboModalidad.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            modalidad = cboModalidad.SelectedValue.ToString();
            DialogResult = true;
        }
    }
}
