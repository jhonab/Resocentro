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
    /// Lógica de interacción para frmSearchMedico.xaml
    /// </summary>
    public partial class frmSearchMedico : Window
    {
        public MEDICOEXTERNO medico;
        public frmSearchMedico()
        {
            InitializeComponent();
            cbotipomedico.ItemsSource = new UtilDAO().getTipoMedico();
            cbotipomedico.DisplayMemberPath = "descripcion";
            cbotipomedico.SelectedValuePath = "idTipo";
            cbotipomedico.SelectedIndex = 0;
            txtMedico.Focus();
        }
        public void setGUI(string filtro)
        {
            if (filtro != "")
            {
                txtMedico.Text = filtro;
                buscarMedico();
            }
        }
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            buscarMedico();
        }

        private void buscarMedico()
        {
            try
            {
                if (cbotipomedico.SelectedValue != null)
                {
                    var tipo = cbotipomedico.SelectedValue.ToString();
                    var nombre = txtMedico.Text.Trim();
                    var cmp = txtcmp.Text.Trim();
                    if (nombre != "" || cmp != "")
                        gridMedico.ItemsSource = new UtilDAO().getMedico(nombre, cmp, tipo);
                    gridMedico.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void txtMedico_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscarMedico();
            }
        }
        private void gridMedico_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            MEDICOEXTERNO item = (MEDICOEXTERNO)e.Row.DataContext;
            if (item != null)
            {
                medico = item;
                DialogResult = true;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
