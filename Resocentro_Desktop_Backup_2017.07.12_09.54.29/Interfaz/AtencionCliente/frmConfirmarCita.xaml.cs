using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.frmUtil;
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

namespace Resocentro_Desktop.Interfaz.AtencionCliente
{
    /// <summary>
    /// Lógica de interacción para frmConfirmarCita.xaml
    /// </summary>
    public partial class frmConfirmarCita : Window
    {
        MySession session;
        CitarxConfirmar cita;
        string seleccion;
        public frmConfirmarCita()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session, CitarxConfirmar cita, string seleccion)
        {
            this.session = session;
            this.cita = cita;
            this.seleccion = seleccion;
            lblidpaciente.Content = cita.codigopaciente.ToString();
            lblpaciente.Content = cita.paciente.ToString();
            lbltelefono.Content = ("Telf.: " + cita.telefono + " - " + cita.celular).ToString();
            txtcomentarios.Text = cita.observacion;
            lblcodigocarta.Content = cita.codigocartagarantia;
            calcularPrecioContrasteSedacion();
        }

        private void calcularPrecioContrasteSedacion()
        {
            var listaestudios = cargarEstudios(cita.numerocita);
            gridEstudios.ItemsSource = listaestudios;
            var examen = listaestudios.FirstOrDefault();
            if (examen != null)
            {
                string moneda = "";
                double precio = 0;
                //Contraste
                precio = new CitaDAO().getPrecioSedacionContraste(examen.codigocompaniaseguro.ToString(), examen.codigoestudio.Substring(0, 3), int.Parse(examen.codigoestudio.Substring(3, 2)), 0, out moneda);
                txtcontraste.Text = precio.ToString();
                lblmonedacontraste.Content = moneda;
                if (cita.sedacion)
                {
                    //Sedacion
                    precio = new CitaDAO().getPrecioSedacionContraste(examen.codigocompaniaseguro.ToString(), examen.codigoestudio.Substring(0, 3), int.Parse(examen.codigoestudio.Substring(3, 2)), 1, out moneda);
                    txtsedacion.Text = precio.ToString();
                    lblmonedasedacion.Content = moneda;
                }
                else
                {
                    stkSedacion.Visibility = Visibility.Collapsed;
                }

            }

        }

        private List<DetalleCita> cargarEstudios(int numerocita)
        {
            return new CitaDAO().getDetalleCita(numerocita.ToString());
        }

        private void MenuContextCartaGarantia_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (cita.codigocartagarantia != "")
            {
                var _cart = new CartaDAO().getCartaxCodigo(cita.codigocartagarantia, cita.codigopaciente);
                var detalle = new CartaDAO().getDetalleCartaxCodigo(cita.codigocartagarantia, cita.codigopaciente);
                if (_cart != null)
                {
                    frmCarta.frmCarta gui = new frmCarta.frmCarta();
                    gui.cargarGUI(session, true);
                    gui.Show();
                    new CartaDAO().insertLog(cita.codigocartagarantia, this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + cita.codigocartagarantia);
                    gui.setCartaGarantia(_cart, detalle, false);
                }
            }
        }

        private void HCPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            frmHistoriaPaciente gui = new frmHistoriaPaciente();
            gui.cargarGUI(session);
            gui.Show();
            gui.buscarPaciente(cita.codigopaciente.ToString(), 2);

        }

        private void InfoPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            frmPaciente gui = new frmPaciente();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                gui.setPaciente(db.PACIENTE.SingleOrDefault(x => x.codigopaciente == cita.codigopaciente));
                gui.ShowDialog();
            }
        }

        private void gridEstudios_CellValidating(object sender, Telerik.Windows.Controls.GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "Precio")
            {
                int newValue = Int32.Parse(e.NewValue.ToString());
                if (newValue < 0)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "El valor debe ser mayor a 0.";
                }
            }
        }

        private void gridEstudios_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            DetalleCita item = e.NewData as DetalleCita;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var detalle = db.EXAMENXCITA.Where(x => x.codigoexamencita == item.codigoexamencita).SingleOrDefault();
                    if (detalle != null)
                    {
                        try
                        {
                            detalle.precioestudio = float.Parse(item.preciobruto.ToString());
                            db.SaveChanges();
                            gridEstudios.ItemsSource = cargarEstudios(cita.numerocita);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ocurrio un error en la Base de datos:" + ex.Message);
                        }
                    }
                }
            }
        }

        private void btnEnviarCorreo_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                if (chkCitaConfirmada.IsChecked.Value)
                {
                    var _cita = db.CITA.SingleOrDefault(x => x.numerocita == cita.numerocita);
                    if (cita != null)
                    {
                        _cita.isConfirmado = true;
                        _cita.observacion = txtnewcomentarios.Text.Trim();
                        foreach (var item in db.EXAMENXCITA.Where(x => x.numerocita == _cita.numerocita).ToList())
                        {
                            item.estadoestudio = "K";
                        }
                        db.SaveChanges();
                    }
                }
            }
            frmConfirmarDatosEnvioCorreo gui = new frmConfirmarDatosEnvioCorreo();
            gui.cagarGUI(session, cita, lblmonedacontraste.Content.ToString() + txtcontraste.Text, lblmonedasedacion.Content.ToString() + txtsedacion.Text);
            gui.ShowDialog();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                if (chkCitaConfirmada.IsChecked.Value)
                {
                    var _cita = db.CITA.SingleOrDefault(x => x.numerocita == cita.numerocita);
                    if (cita != null)
                    {
                        _cita.isConfirmado = true;
                        _cita.observacion = txtnewcomentarios.Text.Trim();
                        foreach (var item in db.EXAMENXCITA.Where(x => x.numerocita == _cita.numerocita).ToList())
                        {
                            item.estadoestudio = "K";
                        }
                        db.SaveChanges();
                    }
                }

                Confirmacion_Cita c = new Confirmacion_Cita();
                c.numerocita = cita.numerocita;
                c.fecha = Tool.getDatetime(); ;
                //tipo 1 Correo, Tipo 0 Llamada
                c.tipo = 0;
                c.codigousuario = session.codigousuario;
                c.telefono = seleccion;
                c.email = "";
                c.comentarios = "";
                db.Confirmacion_Cita.Add(c);
                DialogResult = true;
                MessageBox.Show("Se registro la llamada");

            }
        }




    }
}
