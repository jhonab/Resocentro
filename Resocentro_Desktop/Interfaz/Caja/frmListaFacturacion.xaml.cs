using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja;
using Resocentro_Desktop.Interfaz.Cobranza;
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

namespace Resocentro_Desktop.Interfaz.Facturacion
{
    /// <summary>
    /// Lógica de interacción para frmListaFacturacion.xaml
    /// </summary>
    public partial class frmListaFacturacion : Window
    {
        MySession session;
        public frmListaFacturacion()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            rdtfecha.SelectedDate = DateTime.Now;
            rdtfechafin.SelectedDate = DateTime.Now;
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;
        }

        private void gridAtenciones_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            FacturacionEntity item = (FacturacionEntity)e.Row.DataContext;
            if (item != null)
            {
                int unidad = int.Parse(cbosucursal.SelectedValue.ToString());
                int codigosede = (unidad * 100) + item.sucursal;
                frmRegistrarCobranza gui = new frmRegistrarCobranza();
                gui.cargarGUI(session, unidad, codigosede - (unidad * 100), TIPO_COBRANZA.ASEGURADORA);
                gui.cargarCobranza(item.numeroatencion);
                gui.Show();
                //gui.abrirInfoFacturacion();

            }
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            var lista = new CobranzaDAO().getListaFacturacion(cbosucursal.SelectedValue.ToString(), rdtfecha.SelectedDate.Value.ToShortDateString(), rdtfechafin.SelectedDate.Value.ToShortDateString());
            gridAtenciones.ItemsSource = null;
            gridAtenciones.ItemsSource = lista;
            gridAtenciones.CalculateAggregates();

            lblresumen.Content = "Atenciones del día: " + rdtfecha.SelectedDate.Value.ToShortDateString() + "\tTotal para Facturar: " + lista.Where(x => x.numerodocumento == "").ToList().Count() + "\t\t" + "Total Facturados: " + lista.Where(x => x.numerodocumento != "").ToList().Count() + "";
        }

        private void btnReporte_Click(object sender, RoutedEventArgs e)
        {
            frmReporteFacturacion gui = new frmReporteFacturacion();
            gui.cargarGUI(session);
            gui.Show();
        }



        /*private void MenuItemUnirCobranza_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var codigopaciente = "";
            foreach (FacturacionEntity item in this.gridAtenciones.SelectedItems.ToList())
            {

                if (codigopaciente != "")
                    if (codigopaciente != item.paciente)
                    {
                        MessageBox.Show("Las atenciones no son del mismo paciente");
                        break;
                    }
                codigopaciente = item.paciente;
            }
        }*/

    }
}
