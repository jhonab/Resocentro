using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
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

namespace Resocentro_Desktop.Interfaz.Caja.RegistroCobranza
{
    /// <summary>
    /// Lógica de interacción para frmReciboProvisional.xaml
    /// </summary>
    public partial class frmReciboProvisional : Window
    {
        public MySession session;
        public Recibo_Provisional data;
        public double tipocambio;
        public List<DetalleReciboProvisional> lista;
        ReciboProvisional item;
        public frmReciboProvisional()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            data = new Recibo_Provisional();
            lista = new List<DetalleReciboProvisional>();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                cbotipomoneda.ItemsSource = db.MONEDA.Where(x => x.IsEnabledMoneda == true).ToList();
                cbotipomoneda.SelectedValuePath = "codigomoneda";
                cbotipomoneda.DisplayMemberPath = "descripcion";
                cbotipomoneda.SelectedValue = "1";

                cboformapago.ItemsSource = db.TARJETA.Where(x => x.IsEnabledTarjeta == true).ToList().OrderBy(x => x.descripcion);
                cboformapago.SelectedValuePath = "codigotarjeta";
                cboformapago.DisplayMemberPath = "descripcion";
                cboformapago.SelectedValue = "10";

                tipocambio = new CobranzaDAO().getTC();
                lblTCCompra.Content = "TC Compra: " + (Math.Round(tipocambio, 3, MidpointRounding.AwayFromZero)).ToString();
            }
        }
        public void cargarRecibo(ReciboProvisional recibo, List<DetalleReciboProvisional> detalle)
        {
            item = recibo;
            txtcita.Text = recibo.numerocita.ToString();
            buscarData(txtcita.Text = recibo.numerocita.ToString());
            txtmontototal.Value = Convert.ToDouble(recibo.monto_total.ToString());
            txtmotivo.Text = recibo.motivo;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                foreach (var d in detalle)
                {
                    lista.Add(new DetalleReciboProvisional
                    {
                        formapago = d.formapago,
                        TARJETA = db.TARJETA.SingleOrDefault(x => x.codigotarjeta == d.formapago),
                        monto = Convert.ToDecimal(d.monto.ToString()),
                        numeroreferencia = d.numeroreferencia,
                        moneda = d.moneda,
                        tipocambio = d.tipocambio
                    });
                }
            }
            gridFormaPago.ItemsSource = null;
            gridFormaPago.ItemsSource = lista;

            btnguardar.IsEnabled = false;

        }
        private void txtcita_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtcita.Text != "")
                if (e.Key == Key.Enter)
                {
                    buscarData(txtcita.Text);
                }
        }

        public void buscarData(string p)
        {
            data = new CobranzaDAO().getinfoReciboProvisional(txtcita.Text);
            lblpaciente.Text = data.paciente;
            gridEstudios.ItemsSource = data.estudio;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (data.paciente == "" || txtmotivo.Text == "" || lista.Count() == 0)
            {
                MessageBox.Show("Verifique los datos.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                try
                {
                    double monto = Convert.ToDouble(lista.Where(x => x.moneda == 1).Select(x => x.monto).Sum() + lista.Where(x => x.moneda == 2).Select(x => x.monto * x.tipocambio.Value).Sum());
                    if (monto < txtmontototal.Value.Value)
                        MessageBox.Show("Los montos ingresados no suman el Monto Total");
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        item = new ReciboProvisional();
                        item.numerocita = int.Parse(txtcita.Text);
                        item.empresa = data.empresa;
                        item.codigopaciente = data.codigopaciente;
                        item.fecha_reg = Tool.getDatetime();
                        item.motivo = txtmotivo.Text;
                        item.monto_recibido = Convert.ToDecimal(monto);
                        item.monto_total = Convert.ToDecimal(txtmontototal.Value.Value.ToString());
                        item.usuario = session.codigousuario;
                        item.isActivo = true;
                        db.ReciboProvisional.Add(item);

                        foreach (DetalleReciboProvisional d in lista)
                        {
                            d.TARJETA = null;
                            d.idRecibo = item.idRecibo;
                            d.fec_registro = Tool.getDatetime();
                            d.usuario = session.codigousuario;
                            d.tipocambio = Convert.ToDecimal(tipocambio.ToString());
                            db.DetalleReciboProvisional.Add(d);
                        }
                        db.SaveChanges();
                        print();
                        btnguardar.IsEnabled = false;
                        MessageBox.Show("Documento Generado");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void print()
        {
            frmReciboProvisionalprint gui = new frmReciboProvisionalprint();
            gui.cargarGUI(session, item, data);
            gui.printTicket(2);
        }

        private void btnPaciente_Click(object sender, RoutedEventArgs e)
        {
            frmHistoriaPaciente gui = new frmHistoriaPaciente();
            gui.CallbyOtherGUI(this);
            gui.Show();
        }

        private void btnagregar_Click(object sender, RoutedEventArgs e)
        {

            if (cboformapago.SelectedValue.ToString() != "10" && txtnumreferencia.Text == "")
            {
                MessageBox.Show("Verifique los datos.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                /* double monto = Convert.ToDouble(lista.Where(x => x.moneda == 1).Select(x => x.monto).Sum() + lista.Where(x => x.moneda == 2).Select(x => x.monto * x.tipocambio.Value).Sum()) + (Convert.ToInt32(cbotipomoneda.SelectedValue.ToString()) == 2 ? (txtmontoentregado.Value.Value * tipocambio) : txtmontoentregado.Value.Value);
                 if (monto > txtmontototal.Value.Value)
                     MessageBox.Show("Verifique el monto, no puede exceder el monto total");*/

                lista.Add(new DetalleReciboProvisional
                {
                    formapago = int.Parse(cboformapago.SelectedValue.ToString()),
                    TARJETA = (TARJETA)cboformapago.SelectedItem,
                    monto = Convert.ToDecimal(txtmontoentregado.Value.Value.ToString()),
                    numeroreferencia = txtnumreferencia.Text,
                    moneda = Convert.ToInt32(cbotipomoneda.SelectedValue.ToString()),
                    tipocambio = Convert.ToDecimal(tipocambio.ToString())
                });
                gridFormaPago.ItemsSource = null;
                gridFormaPago.ItemsSource = lista;
            }

        }

        private void gridFormaPago_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            DetalleReciboProvisional item = (DetalleReciboProvisional)e.Row.DataContext;
            if (item != null)
            {
                lista.Remove(item);
                gridFormaPago.ItemsSource = null;
                gridFormaPago.ItemsSource = lista;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            print();
        }

        private void gridEstudios_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            string item = (string)e.Row.DataContext;
            if (item != null)
            {
                gridEstudios.Items.Remove(item);
                gridEstudios.Items.Refresh();
            }
        }
    }
}
