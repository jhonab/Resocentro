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
    /// Lógica de interacción para frmTarifario.xaml
    /// </summary>
    public partial class frmTarifario : Window
    {
        MySession session;
        List<COMPANIASEGURO> listacompania;
        public List<Search_Estudio> listaestudios;
        public frmTarifario()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session,int compania=0,int empresa=1,string nombrecompania="")
        {
            this.session = session;
            cboempresa.ItemsSource = new UtilDAO().getEmpresa();
            cboempresa.SelectedValuePath = "codigounidad";
            cboempresa.DisplayMemberPath = "nombre";
            cboempresa.SelectedIndex = 0;
            listacompania = new CobranzaDAO().getCompanias();
            gridaseguradora.ItemsSource = listacompania;
            if (compania != 0)
            {
                cboempresa.SelectedValue = empresa.ToString();
                listaestudios = new CobranzaDAO().getEstudiosxCompania(compania, empresa);
                gridestudios.ItemsSource = listaestudios;
                txtaseguradora.Text = nombrecompania.ToLower();
                lblcompania.Content = "Compañia Seleccionada: " + nombrecompania.ToUpper();
            }
        }

        private void gridaseguradora_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            COMPANIASEGURO item = (COMPANIASEGURO)e.Row.DataContext;
            gridestudios.ItemsSource = null;
            txtestudio.Text = "";
            listaestudios = new List<Search_Estudio>();
            lblcompania.Content = "Compañia Seleccionada: " + item.descripcion.ToUpper();
            if (item != null)
            {
                listaestudios = new CobranzaDAO().getEstudiosxCompania(item.codigocompaniaseguro, int.Parse(cboempresa.SelectedValue.ToString()));
                gridestudios.ItemsSource = listaestudios;
            }
        }

        private void txtaseguradora_TextChanged(object sender, TextChangedEventArgs e)
        {
            gridaseguradora.ItemsSource = null;
            gridaseguradora.ItemsSource = listacompania.Where(x => x.descripcion.ToLower().Contains(txtaseguradora.Text)).ToList();
        }

        private void txtestudio_TextChanged(object sender, TextChangedEventArgs e)
        {
            gridestudios.ItemsSource = null;
            gridestudios.ItemsSource = listaestudios.Where(x => x.estudio.ToLower().Contains(txtestudio.Text)).ToList();
        }
    }
}
