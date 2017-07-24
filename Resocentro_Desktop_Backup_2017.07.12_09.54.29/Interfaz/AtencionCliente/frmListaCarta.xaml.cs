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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmListaCarta.xaml
    /// </summary>
    public partial class frmListaCarta : Window
    {
        MySession session;
        public frmListaCarta()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboestado.Text = "APROBADA";
        }

        public void cargarLista()
        {
            string estado = cboestado.Text;
            if (estado != "")
                gridCartas.ItemsSource = new CitaDAO().getCartasxCitas(estado);
        }

        private void btnTelefono_Click(object sender, RoutedEventArgs e)
        {
            var button = (RadButton)e.OriginalSource;
            var item = (CartaxCitar)button.CommandParameter;
            if (item != null)
                if (button.Content != null)
                    if (button.Content.ToString() != "" && button.Content.ToString() != "0")
                    {
                        abrirFormulario(item, button);
                    }
        }

        private void btnCelular_Click(object sender, RoutedEventArgs e)
        {
            var button = (RadButton)e.OriginalSource;
            var item = (CartaxCitar)button.CommandParameter;
            if (item != null)
                if (button.Content != null)
                    if (button.Content.ToString() != "" && button.Content.ToString() != "0")
                    {
                        abrirFormulario(item, button);
                    }
        }

        private void abrirFormulario(CartaxCitar item, RadButton button)
        {
            frmRegistarLlamada gui = new frmRegistarLlamada();
            gui.setLlamada(session, item, button.Content.ToString());
            gui.ShowDialog();
            if (gui.isCitar)
            {
                CITA c = new CITA();
                c.codigopaciente = item.idpaciente;
                c.codigoclinica = item.codigoclinica;
                c.montototal = float.Parse(item.monto.ToString());
                c.observacion = item.comentarios;
                c.cmp = item.cmp;
                c.codigocompaniaseguro = item.codigocompaniaseguro;
                c.ruc = item.ruc;
                c.codigomodalidad = 1;
                c.codigocartagarantia = item.codigocarta;
                c.fechareserva = DateTime.Now.Date;
                

                List<Search_Estudio> estudios = new List<Search_Estudio>();
                var lest = new CartaDAO().getDetalleCartaxCodigo(item.codigocarta, item.idpaciente);
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (var est in lest)
                    {
                        var _est = db.ESTUDIO.SingleOrDefault(x => x.codigoestudio == est.codigoestudio);
                        var detalle_insert = new EstudiosDAO().getPrecioEstudio(est.codigoestudio, item.codigocompaniaseguro);
                        Search_Estudio es = new Search_Estudio();
                        es.codigoestudio = est.codigoestudio;
                        es.estudio = _est.nombreestudio;
                        es.codigoclase = est.codigoclase;
                        //moneda
                        if (est.moneda == null)
                            es.idmoneda = detalle_insert.idmoneda;
                        else
                            es.idmoneda = est.moneda.Value;
                        //precio bruto
                        if (est.preciobruto == null)
                            es.precio = detalle_insert.precio;
                        else
                            es.precio = est.preciobruto.Value;
                        //cobertura
                        if (est.cobertura_det == null)
                            es.cobertura = item.cobertura;
                        else
                            es.cobertura = est.cobertura_det.Value;
                        //descuento
                        if (est.descuento == null)
                            es.descuento = ((es.cobertura * es.precio) / 100) * -1.0;
                        else
                            es.descuento = est.descuento.Value;
                        es.indicaciones = _est.indicacion;
                        estudios.Add(es);
                    }
                }
                frmCita guiCita = new frmCita();
                guiCita.cargarGUI(session);
                guiCita.setCita(c, estudios, true);
                guiCita.ShowDialog();
            }
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CartaxCitar item = (CartaxCitar)this.gridCartas.SelectedItem;
            if (item == null)
            {
                ContextMenuItemCaducarCarta.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContextMenuItemCaducarCarta.Visibility = Visibility.Visible;
                ContextMenuItemCaducarCarta.Header = "Caducar Carta: \"" + item.codigocarta + "\"";
            }
        }

        private void MenuItemCaducarCarta_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            CartaxCitar item = (CartaxCitar)this.gridCartas.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea cambiar el estado de la Carta a \"CADUCADA\"?", "Pregunta", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var carta = db.CARTAGARANTIA.Where(x => x.codigocartagarantia == item.codigocarta && x.codigopaciente == item.idpaciente).SingleOrDefault();
                        if (carta != null)
                        {
                            carta.estadocarta = "CADUCADA";
                            db.SaveChanges();
                            new CartaDAO().insertLog(item.codigocarta, this.session.shortuser, (int)Tipo_Log.Delete, "Se Cambio el estado la Carta N° " + item.codigocarta + " a CADUCADA");
                        }
                    }
                }
            }
        }

        private void btnCitar_Click(object sender, RoutedEventArgs e)
        {
            frmCita gui = new frmCita();
            gui.cargarGUI(session);
            gui.Show();
        }

        private void btnListar_Click(object sender, RoutedEventArgs e)
        {
            cargarLista();
        }

    }
}
