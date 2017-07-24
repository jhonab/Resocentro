using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.Caja.RegistroCobranza;
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
    /// Lógica de interacción para frmListaRecibosProvisionales.xaml
    /// </summary>
    public partial class frmListaRecibosProvisionales : Window
    {
        MySession session;
        public frmListaRecibosProvisionales()
        {
            InitializeComponent();

        }

        private void MenuItemDevolver_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RecProList r = (RecProList)this.gridRecibos.SelectedItem;
            if (r != null)
            {
                frmDevolucionDineroRecibo gui = new frmDevolucionDineroRecibo();
                gui.Owner = this;
                gui.txtmonto.Value = Convert.ToDouble(r.monto_recibido);
                gui.ShowDialog();
                if (gui.monto > 0)
                {

                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {

                        /*DetalleReciboProvisional d = new DetalleReciboProvisional();
                        d.idRecibo = r.idRecibo;
                        d.formapago = 12;
                        d.monto = Convert.ToDecimal(gui.monto) * -1;
                        d.fec_registro = Tool.getDatetime();
                        d.usuario = session.codigousuario;
                        d.numeroreferencia = "";
                        d.moneda = 1;
                        d.tipocambio = 0;
                        db.DetalleReciboProvisional.Add(d);*/
                        var recibo = db.ReciboProvisional.SingleOrDefault(x => x.idRecibo == r.idRecibo);
                        recibo.isActivo = false;
                        recibo.fecha_cancelacion = Tool.getDatetime();
                        recibo.usu_cancela = session.codigousuario;
                        recibo.motivo = gui.txtmotivo.Text.Trim();
                        db.SaveChanges();
                        listarrecibos();
                    }
                }
            }
        }

        private void RadContextMenuDetalle_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RecProList item = (RecProList)this.gridRecibos.SelectedItem;
            if (item != null)
                if (!item.isactivo)
                    MenuItemDevolver.Visibility = Visibility.Collapsed;
                else
                    MenuItemDevolver.Visibility = Visibility.Visible;
            if (session.roles.Contains("59"))
                MenuItemAprobar.Visibility = Visibility.Visible;
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;

        }

        private void listarrecibos()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var empresa = int.Parse(cbosucursal.SelectedValue.ToString());
                gridRecibos.ItemsSource = null;
                gridRecibos.ItemsSource = db.ReciboProvisional.Where(x => (x.isActivo || x.fecha_aprueba == null) && x.empresa == empresa).Select(x => new RecProList
                {
                    idRecibo = x.idRecibo,
                    fecha_reg = x.fecha_reg,
                    paciente = x.PACIENTE.apellidos + ", " + x.PACIENTE.nombres,
                    monto_total = x.monto_total,
                    monto_recibido = x.monto_recibido,
                    motivo = x.motivo,
                    registra = x.USUARIO1.ShortName,
                    cancela = x.USUARIO3.ShortName,
                    fecha_cancelacion = x.fecha_cancelacion,
                    isactivo = x.isActivo,
                    motivocancelacion = x.comentarios
                }).ToList();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listarrecibos();
        }

        private void MenuItemAprobar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RecProList r = (RecProList)this.gridRecibos.SelectedItem;
            if (r != null)
            {
                if (MessageBox.Show("Desea confirmar la cancelación del Recibo", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var recibo = db.ReciboProvisional.SingleOrDefault(x => x.idRecibo == r.idRecibo);
                        recibo.fecha_aprueba = Tool.getDatetime();
                        recibo.usu_aprueba = session.codigousuario;
                        db.SaveChanges();
                        listarrecibos();
                    }


            }
        }

        private void MenuItemInfoRecibo_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RecProList r = (RecProList)this.gridRecibos.SelectedItem;
            if (r != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var recibo = db.ReciboProvisional.SingleOrDefault(x => x.idRecibo == r.idRecibo);
                    var detalle = db.DetalleReciboProvisional.Where(x => x.idRecibo == r.idRecibo).ToList();

                    frmReciboProvisional gui = new frmReciboProvisional();
                    gui.cargarGUI(session);
                    gui.cargarRecibo(recibo, detalle);
                    gui.Show();
                }
            }
        }
    }
    class RecProList
    {
        public int idRecibo { get; set; }

        public DateTime fecha_reg { get; set; }

        public string paciente { get; set; }

        public decimal monto_total { get; set; }

        public decimal monto_recibido { get; set; }

        public string registra { get; set; }
        public string cancela { get; set; }
        public DateTime? fecha_cancelacion { get; set; }

        public string motivo { get; set; }

        public bool isactivo { get; set; }

        public string motivocancelacion { get; set; }
    }
}
