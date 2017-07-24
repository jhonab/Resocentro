using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
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
    /// Lógica de interacción para frmListaAtenciones.xaml
    /// </summary>
    public partial class frmListaAtenciones : Window
    {
        MySession session;
        public frmListaAtenciones()
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

            }
            else
            {
                this.Close();
                MessageBox.Show("No tiene ninguna sucursal asignada", "ERROR");
            }
        }

        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            var fecha =DateTime.Now;
            var sede = cbosede.SelectedValue.ToString();
            listarAtenciones(fecha, sede);
        }
        private void listarAtenciones(DateTime fecha, string sede, string numeroatencion = "",string numeroexamen="")
        {
            try
            {
                grid_Atenciones.ItemsSource = new CobranzaDAO().ListaAtenciones(fecha.ToString("dd/MM/yyyy"), sede, numeroatencion,numeroexamen);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grid_Atenciones_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            CobranzaViewModel item = (CobranzaViewModel)e.Row.DataContext;
            if (item != null)
            {
                int codigosede;
                if (int.TryParse(cbosede.SelectedValue.ToString(), out codigosede))
                {
                    int unidad = codigosede / 100;
                    frmRegistrarCobranza gui = new frmRegistrarCobranza();
                    gui.cargarGUI(session, unidad, codigosede - (unidad * 100), TIPO_COBRANZA.PACIENTE);
                    gui.cargarCobranza(item.numeroatencion);
                    gui.Show();
                    //gui.abrirInfoFacturacion();
                }
            }
        }

        private void txtnumeroatencion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key==Key.Enter)
            {
                var fecha = DateTime.Now;
                var sede = cbosede.SelectedValue.ToString();
                var numeroatencion = txtnumeroatencion.Text;
                int atencion = 0;
                if (int.TryParse(numeroatencion, out atencion))
                    listarAtenciones(fecha, sede, numeroatencion);
                else
                    MessageBox.Show("Ingrese un número de atención correcto", "Error");
            }
        }

        private void txtnumeroexamen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                var fecha = DateTime.Now;
                var sede = cbosede.SelectedValue.ToString();
                var numeroexamen = txtnumeroexamen.Text;
                int examen = 0;
                if (int.TryParse(numeroexamen, out examen))
                    listarAtenciones(fecha, sede, "",numeroexamen);
                else
                    MessageBox.Show("Ingrese un número de examen correcto", "Error");
            }
        }
    }
}
