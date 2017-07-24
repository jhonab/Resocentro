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

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmConfirmacionPago.xaml
    /// </summary>
    public partial class frmConfirmacionPago : Window
    {
        public frmConfirmacionPago()
        {
            InitializeComponent();
           /* cboempleado.ItemsSource = new AdministracionDAO().getEmpleados();
            cboempleado.SelectedValuePath = "codigo";
            cboempleado.DisplayMemberPath = "valor";
            cboempleado.SelectedIndex = 0;*/
            cboAño.ItemsSource = new UtilDAO().getAño(5);
            cboAño.DisplayMemberPath = "nombre";
            cboAño.SelectedValuePath = "codigo";
            cboAño.SelectedValue = DateTime.Now.Year;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridLog.ItemsSource = null;
            gridLog.ItemsSource = new AdministracionDAO().getLogCnfirmacionPago(cbomes.SelectedIndex,int.Parse(cboAño.SelectedValue.ToString() ));
        }

        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(gridLog, true, false, false);
        }
    }
}
