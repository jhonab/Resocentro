using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using System;
using System.Collections.Generic;
using System.IO;
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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmPagoMedico.xaml
    /// </summary>
    public partial class frmPagoMedico : Window
    {
        MySession session;
        public frmPagoMedico()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;

            cboAño.ItemsSource = new UtilDAO().getAño(5);
            cboAño.DisplayMemberPath = "nombre";
            cboAño.SelectedValuePath = "codigo";
            cboAño.SelectedValue = DateTime.Now.Year;
            cboMes.ItemsSource = new UtilDAO().getMes();
            cboMes.DisplayMemberPath = "nombre";
            cboMes.SelectedValuePath = "codigo";
            cboMes.SelectedValue = DateTime.Now.Month;
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (cboAño.SelectedValue != null && cboMes.SelectedValue != null)
            {
                int ano = int.Parse(cboAño.SelectedValue.ToString());
                int mes = int.Parse(cboMes.SelectedValue.ToString());
                listarMedico(ano, mes);
            }
        }

        private void listarMedico(int ano, int mes)
        {
            var lista = new PagosDAO().getListMedicos(mes, ano);
            gridMedicos.ItemsSource = lista;
        }
        private void MenuItemExport_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (gridMedicos.ItemsSource != null)
            {
                string extension = "xls";
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    DefaultExt = extension,
                    Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "Excel"),
                    FilterIndex = 1
                };
                if (dialog.ShowDialog() == true)
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        gridMedicos.Export(stream,
                         new GridViewExportOptions()
                         {
                             Format = ExportFormat.Html,
                             ShowColumnHeaders = true,
                             ShowColumnFooters = true,
                             ShowGroupFooters = true,
                         });
                    }
                }
            }
        }
    }
}
