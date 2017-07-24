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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmRegistrarFormaPago.xaml
    /// </summary>
    public partial class frmRegistrarFormaPago : Window
    {
        MySession session;
        DOCUMENTO item;
        public frmRegistrarFormaPago()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
            cbosede.SelectedValuePath = "codigoInt";
            cbosede.DisplayMemberPath = "nombreShort";
            cbosede.SelectedIndex = 0;
            cboTipoDocumento.SelectedIndex = 0;
            dtpfecha.SelectedDate = DateTime.Now;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                cbotipomoneda.ItemsSource = db.MONEDA.Where(x => x.IsEnabledMoneda == true).ToList();
                cbotipomoneda.SelectedValuePath = "codigomoneda";
                cbotipomoneda.DisplayMemberPath = "descripcion";
                cbotipomoneda.SelectedValue = "1";

                cboTipoPago.ItemsSource = db.TARJETA.Where(x => x.IsEnabledTarjeta == true).ToList().OrderBy(x => x.descripcion);
                cboTipoPago.SelectedValuePath = "codigotarjeta";
                cboTipoPago.DisplayMemberPath = "descripcion";
                cboTipoPago.SelectedValue = "10";
            }
        }

        private void btnAgregarFormaPago_Click(object sender, RoutedEventArgs e)
        {
            if (cboTipoPago.SelectedValue.ToString() != "10" && txtnumeroReferencia.Text == "")
            {
                MessageBox.Show("Verifique los datos.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                try
                {
                    FORMADEPAGO forma = new FORMADEPAGO();
                    forma.monto = Convert.ToSingle(txtmonto.Value.Value);
                    forma.codigomodalidadpago = 1;
                    forma.codigomoneda = int.Parse(cbotipomoneda.SelectedValue.ToString());
                    forma.numerodocumento = txtnumerodocumento.Text;
                    forma.ruc = item.ruc;
                    forma.codigopaciente = item.codigopaciente;
                    forma.codigousuario = item.codigousuario;
                    forma.codigotarjeta = int.Parse(cboTipoPago.SelectedValue.ToString());
                    forma.fechadepago = dtpfecha.SelectedDate.Value;
                    forma.codigounidad = item.codigounidad;
                    forma.codigosucursal = item.codigosucursal;
                    forma.numeroReferencia = txtnumeroReferencia.Text;
                    new CobranzaDAO().registrarFormaPago(forma);
                    listarformaPago();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        private void listarformaPago()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                grid_formaPago.ItemsSource = null;
                grid_formaPago.ItemsSource = new DocumentoDAO().getFormaPagoDocumento(item.numerodocumento, item.codigopaciente, item.codigounidad.ToString(), item.codigosucursal.ToString());
                limpiarDatosFormaPago();
            }
        }
        private void limpiarDatosFormaPago()
        {
            txtmonto.Value = 0;
            cbotipomoneda.SelectedIndex = 0;
            cboTipoPago.SelectedIndex = 0;
            txtnumeroReferencia.Text = "";
        }
        private void txtnumerodocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buscarDocumento();
            }
        }

        private void buscarDocumento()
        {
            string _empresa = cbosede.SelectedValue.ToString();
            item = new CobranzaDAO().getInfoDocumento(txtnumerodocumento.Text, cboTipoDocumento.Text.Substring(0, 2),
                _empresa.Substring(0, 1),
                (int.Parse(_empresa.Substring(1))).ToString()
                );
            if (item == null)
            {
                MessageBox.Show("No se encontro ningun documento con los datos ingresados");
                btnAgregar.IsEnabled = false;
            }
            else
            {
                btnAgregar.IsEnabled = true;
                listarformaPago();
            }
        }

        private void grid_formaPago_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            if (MessageBox.Show("¿Desea eliminar la forma de pago?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    FormaPagoEntity item = (FormaPagoEntity)e.Row.DataContext;
                    var forma = db.FORMADEPAGO.SingleOrDefault(x => x.codigoformapago == item.ID);
                    if (forma != null)
                    {
                        db.FORMADEPAGO.Remove(forma);
                        db.SaveChanges();
                    }
                    listarformaPago();
                }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            buscarDocumento();
        }
    }
}
