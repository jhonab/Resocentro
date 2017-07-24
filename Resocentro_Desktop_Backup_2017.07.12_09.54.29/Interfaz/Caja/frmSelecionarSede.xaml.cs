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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmSelecionarSede.xaml
    /// </summary>
    public partial class frmSelecionarSede : Window
    {
        MySession session;
        public int codigoempresasede { get; set; }
        public frmSelecionarSede()
        {
            InitializeComponent();

        }
        public void cargarGUI(MySession session)
        {
            if (session.sucursales.ToList().Count > 0)
            {
                this.session = session;
                //cargamos los combos segun usuario
                cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
                cbosede.SelectedValuePath = "codigoInt";
                cbosede.DisplayMemberPath = "nombreShort";
                cbosede.SelectedIndex = 0;
                codigoempresasede = 9999;

            }
            else
            {
                this.Close();
                MessageBox.Show("No tiene ninguna sucursal asignada", "ERROR");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            codigoempresasede = int.Parse(cbosede.SelectedValue.ToString());
            DialogResult = true;
        }
    }
}
