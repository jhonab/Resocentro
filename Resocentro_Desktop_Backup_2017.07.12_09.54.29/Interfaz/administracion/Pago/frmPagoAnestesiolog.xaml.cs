using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.administracion.impresion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Telerik.Windows.Data;

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmPagoAnestesiolog.xaml
    /// </summary>
    public partial class frmPagoAnestesiolog : Window
    {
        MySession session;
        public frmPagoAnestesiolog()
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
                listarSedaciones(ano, mes);
            }
        }

        private void listarSedaciones(int ano, int mes)
        {
            //GroupDescriptor anestesiologo = new GroupDescriptor();
            //anestesiologo.Member = "anestesiologo";
            //anestesiologo.DisplayContent = "Anestesiólogo";
            //anestesiologo.SortDirection = ListSortDirection.Ascending;
            //this.gridSedaciones.GroupDescriptors.Add(anestesiologo);
            //GroupDescriptor tipo = new GroupDescriptor();
            //tipo.Member = "tipo";
            //tipo.DisplayContent = "Tipo";
            //tipo.SortDirection = ListSortDirection.Ascending;
            //this.gridSedaciones.GroupDescriptors.Add(tipo);
            //GroupDescriptor cantidad = new GroupDescriptor();
            //cantidad.Member = "cantidad";
            //cantidad.DisplayContent = "Cant.";
            //cantidad.SortDirection = ListSortDirection.Ascending;
            //this.gridSedaciones.GroupDescriptors.Add(cantidad);

            //gridSedaciones.ItemsSource = null;
            var lista = new PagosDAO().getListSedaciones(mes, ano);
            gridSedaciones.ItemsSource = lista;
            gridcantidad.ItemsSource = lista.GroupBy(l => l.anestesiologo).Select(x => new { anestesiologo = x.Key, total = x.Sum(l => l.cantidad), seguro = x.Where(l => l.tipo == "Asegurado").Sum(l => l.cantidad), social = x.Where(l => l.tipo == "Particular").Sum(l => l.cantidad), almenara = x.Where(l => l.tipo == "Almenara").Sum(l => l.cantidad), sabogal = x.Where(l => l.tipo == "Sabogal").Sum(l => l.cantidad) }).ToList();
            gridSoles.ItemsSource = lista.GroupBy(l => l.anestesiologo).Select(x => new { anestesiologo = x.Key, total = x.Sum(l => l.comision), seguro = x.Where(l => l.tipo == "Asegurado").Sum(l => l.comision), social = x.Where(l => l.tipo == "Particular").Sum(l => l.comision), almenara = x.Where(l => l.tipo == "Almenara").Sum(l => l.comision), sabogal = x.Where(l => l.tipo == "Sabogal").Sum(l => l.comision) }).ToList();
        }

        private void MenuItemExport_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (gridSedaciones.ItemsSource != null)
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
                        gridSedaciones.Export(stream,
                         new GridViewExportOptions()
                         {
                             Format = ExportFormat.ExcelML,
                             ShowColumnHeaders = true,
                             ShowColumnFooters = true,
                             ShowGroupFooters = true,
                         });
                    }
                }
            }
        }

        private void RadContextGridSedacion_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (gridSedaciones.SelectedItem != null)
            {
                var item = (SedacionesAplicadas)gridSedaciones.SelectedItem;
                MenuItemInfo.Header = "Historia Pac.: " + item.paciente;
                MenuItemInfo.Visibility = Visibility.Visible;
            }
            else
            {
                MenuItemInfo.Visibility = Visibility.Visible;
            }
        }

        private void MenuItemInfo_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (SedacionesAplicadas)gridSedaciones.SelectedItem;
            if (item != null)
            {
                frmHistoriaPaciente gui = new frmHistoriaPaciente();
                gui.cargarGUI(session);
                gui.Show();
                gui.buscarPaciente(item.codigopaciente.ToString(), 2);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (gridSedaciones.ItemsSource != null)
            {
                try
                {
                    frmImpresionAdm gui = new frmImpresionAdm();
                    gui.cargarGUI((List<SedacionesAplicadas>)gridSedaciones.ItemsSource);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


    }
}
