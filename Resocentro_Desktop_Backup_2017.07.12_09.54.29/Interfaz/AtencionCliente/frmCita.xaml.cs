using Resocentro_Desktop.DAO;
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

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmCita.xaml
    /// </summary>
    public partial class frmCita : Window
    {
        MySession session;
        List<Search_Estudio> LstEstudios;
        public frmCita()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            //cargamos los combos segun usuario
            cbosucursal.ItemsSource = new UtilDAO().getSucursales(session.sucursales);
            cbosucursal.SelectedValuePath = "codigoInt";
            cbosucursal.DisplayMemberPath = "nombreShort";

            LstEstudios = new List<Search_Estudio>();
            dtpFecha_reserva.SelectedDate = DateTime.Now;
            dtpFecha_reserva.DisplayDateStart = DateTime.Now;
            limpiarFormulario();
        }
        public void listarEstudios()
        {
            gridEstudios.ItemsSource = null;
            gridEstudios.ItemsSource = LstEstudios.ToList();
            listarIndicaciones();
            txtTotal.Text = LstEstudios.Sum(x => x.precioneto).ToString();
            getPrecioSedacionContraste();
        }
        public void listarIndicaciones()
        {
            txtindicaciones.Text = "";
            foreach (var item in LstEstudios)
            {
                txtindicaciones.Text += item.estudio + ":\n";
                txtindicaciones.Text += item.indicaciones + ".\n\n";
            }
        }
        private void txtidAseguradora_KeyUp(object sender, KeyEventArgs e)
        {
            lblAseguradora.Content = "";
            lblrucAseguradora.Content = "";
            if (e.Key == Key.Enter)
            {
                if (txtidAseguradora.Text != "")
                {
                    try
                    {
                        var _aseguradora = new UtilDAO().getAseguradoraxCodigo(txtidAseguradora.Text);
                        if (_aseguradora != null)
                        {
                            lblAseguradora.Content = _aseguradora.descripcion;
                            txtidAseguradora.Text = _aseguradora.codigocompaniaseguro.ToString();
                            lblrucAseguradora.Content = _aseguradora.ruc;
                            verificarCarta(_aseguradora.ruc);
                            btnBuscarAseguradora.Focus();
                            btnBuscarEstudio.IsEnabled = true;
                        }
                        else
                            btnBuscarEstudio.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                }
                else
                    btnBuscarEstudio.IsEnabled = false;

                LstEstudios = new List<Search_Estudio>();
                listarEstudios();
            }
        }
        private void txtcmp_KeyUp(object sender, KeyEventArgs e)
        {
            lblMedico.Content = "";
            if (e.Key == Key.Enter)
            {
                if (txtcmp.Text != "")
                {
                    var _medico = new UtilDAO().getMedicoxCMP(txtcmp.Text, 1);
                    if (_medico != null)
                    {
                        lblMedico.Content = _medico.apellidos + ", " + _medico.nombres;
                        txtcmp.Text = _medico.cmp;
                        btnBuscarMedico.Focus();
                    }
                }
            }
        }
        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Search_Estudio item = (Search_Estudio)this.gridEstudios.SelectedItem;
            if (item == null)
            {
                ContextMenuItemModificarEstudio.Visibility = Visibility.Collapsed;
                ContextMenuItemEliminarEstudio.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContextMenuItemModificarEstudio.Visibility = Visibility.Visible;
                ContextMenuItemEliminarEstudio.Visibility = Visibility.Visible;
            }
        }
        private void MenuItemEliminarEstudio_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Search_Estudio item = (Search_Estudio)this.gridEstudios.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea eliminar permanentemente el estudio?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    LstEstudios.Remove(item);
                    listarEstudios();
                }
            }
        }
        private void MenuItemModificarHorario_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Search_Estudio item = (Search_Estudio)this.gridEstudios.SelectedItem;
            if (item != null)
            {
                frmTurnoMedico guiTurno = new frmTurnoMedico();
                guiTurno.cargarGUI(item, dtpFecha_reserva.SelectedDate.Value, int.Parse(cbosucursal.SelectedValue.ToString()), cboModalidad.SelectedValue.ToString());
                guiTurno.ShowDialog();
                if (item.fechareserva != dtpFecha_reserva.SelectedDate.Value)
                    if (!validarHorario(item))
                    {
                        item.fechareserva = dtpFecha_reserva.SelectedDate.Value;
                        item.turno = null;
                        MessageBox.Show("No se acepta la misma HORA en varios estudios", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                listarEstudios();

            }
        }
        private void cbosucursal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idempresa = cbosucursal.SelectedValue;
            if (idempresa != null)
            {
                LstEstudios = new List<Search_Estudio>();
                listarEstudios();
                getModalidad(idempresa.ToString());
            }
        }
        public void getModalidad(string empresa)
        {
            cboModalidad.ItemsSource = new UtilDAO().getModalidadxEmpresa(empresa.Substring(0, 1));
            cboModalidad.SelectedValuePath = "codigo";
            cboModalidad.DisplayMemberPath = "nombre";
            cboModalidad.SelectedIndex = 0;
        }
        private void btnClinica_Click(object sender, RoutedEventArgs e)
        {
            frmSearchClinica gui = new frmSearchClinica();
            gui.ShowDialog();
            if (gui.clinica != null)
            {
                lblidClinica.Content = gui.clinica.codigoclinica;
                lblclinica.Content = gui.clinica.razonsocial;
                btnClinica.Focus();
            }
        }
        private void btnBuscarMedico_Click(object sender, RoutedEventArgs e)
        {
            frmSearchMedico gui = new frmSearchMedico();
            gui.ShowDialog();
            if (gui.medico != null)
            {
                lblMedico.Content = gui.medico.apellidos + ", " + gui.medico.nombres;
                txtcmp.Text = gui.medico.cmp;
                btnBuscarMedico.Focus();
            }
        }
        private void btnBuscarAseguradora_Click(object sender, RoutedEventArgs e)
        {
            frmSearchAseguradora gui = new frmSearchAseguradora();
            gui.ShowDialog();
            if (gui.aseguradora != null)
            {
                lblAseguradora.Content = gui.aseguradora.descripcion;
                txtidAseguradora.Text = gui.aseguradora.codigocompaniaseguro.ToString();
                lblrucAseguradora.Content = gui.aseguradora.ruc;
                verificarCarta(gui.aseguradora.ruc);
                btnBuscarAseguradora.Focus();
                btnBuscarEstudio.IsEnabled = true;
            }
            else
                btnBuscarEstudio.IsEnabled = false;

            LstEstudios = new List<Search_Estudio>();
            listarEstudios();
        }
        private void getPrecioSedacionContraste()
        {
            if (LstEstudios.Count > 0 && txtidAseguradora.Text != "")
            {
                var estudio = LstEstudios.FirstOrDefault();
                if (estudio != null)
                {
                    string moneda = "";
                    //sedacion RM
                    double precio = new CitaDAO().getPrecioSedacionContraste(txtidAseguradora.Text, estudio.codigoestudio.Substring(0, 3), 1, 1, out moneda);
                    lblsedacionRMMoneda.Content = moneda + " ";
                    lblsedacionRM.Content = precio.ToString();
                    lblsedacionRMprecio.Content = precio.ToString();
                    precio = 0;
                    //Sedacion TEM
                    precio = new CitaDAO().getPrecioSedacionContraste(txtidAseguradora.Text, estudio.codigoestudio.Substring(0, 3), 2, 1, out moneda);
                    lblsedacionTEMMoneda.Content = moneda + " ";
                    lblsedacionTEM.Content = precio.ToString();
                    lblsedacionTEMprecio.Content = precio.ToString();
                    precio = 0;
                    //contraste RM
                    precio = new CitaDAO().getPrecioSedacionContraste(txtidAseguradora.Text, estudio.codigoestudio.Substring(0, 3), 1, 0, out moneda);
                    lblcontrasteRMMoneda.Content = moneda + " ";
                    lblcontrasteRM.Content = precio.ToString();
                    lblcontrasteRMprecio.Content = precio.ToString();
                    precio = 0;
                    //contraste TEM
                    precio = new CitaDAO().getPrecioSedacionContraste(txtidAseguradora.Text, estudio.codigoestudio.Substring(0, 3), 2, 0, out moneda);
                    lblcontrasteTEMMoneda.Content = moneda + " ";
                    lblcontrasteTEM.Content = precio.ToString();
                    lblcontrasteTEMprecio.Content = precio.ToString();

                }
            }
            else
            {
                lblsedacionRMMoneda.Content = "";
                lblsedacionRM.Content = "";
                lblsedacionRMprecio.Content = "";
                lblsedacionTEMMoneda.Content = "";
                lblsedacionTEM.Content = "";
                lblsedacionTEMprecio.Content = "";
                lblcontrasteRMMoneda.Content = "";
                lblcontrasteRM.Content = "";
                lblcontrasteRMprecio.Content = "";
                lblcontrasteTEMMoneda.Content = "";
                lblcontrasteTEM.Content = "";
                lblcontrasteTEMprecio.Content = "";
            }
        }
        private void calcularSedacionContraste(double cobertura)
        {
            if (lblcodigocarta.Content != null && lblcodigocarta.Content.ToString() != "")
            {
                double precio = 0;
                //sedacion RM
                if (double.TryParse(lblsedacionRMprecio.Content.ToString(), out precio))
                    lblsedacionRM.Content = (precio - ((precio * cobertura) / 100)).ToString();
                precio = 0;
                //sedacion TEM
                if (double.TryParse(lblsedacionTEMprecio.Content.ToString(), out precio))
                    lblsedacionTEM.Content = (precio - ((precio * cobertura) / 100)).ToString();
                precio = 0;
                //Contraste RM
                if (double.TryParse(lblcontrasteRMprecio.Content.ToString(), out precio))
                    lblcontrasteRM.Content = (precio - ((precio * cobertura) / 100)).ToString();
                precio = 0;
                //Contraste TEM
                if (double.TryParse(lblcontrasteTEMprecio.Content.ToString(), out precio))
                    lblcontrasteTEM.Content = (precio - ((precio * cobertura) / 100)).ToString();
            }
        }

        private void verificarCarta(string ruc)
        {
            if (ruc != "10000000000" && ruc != "20131257750")
            {
                MessageBox.Show("Esta Aseguradora requiere crear CARTA DE GARANTIA", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnBuscarEstudio_Click(object sender, RoutedEventArgs e)
        {
            var sucursal = cbosucursal.SelectedValue.ToString();
            var modalidad = cboModalidad.SelectedValue.ToString();
            var aseguradora = txtidAseguradora.Text;
            if (sucursal != "" && modalidad != "")
            {
                frmSearchEstudio gui = new frmSearchEstudio();
                gui.ampliacion = chkAmpliacion.IsChecked.Value ? "1" : "0";
                //gui.ampliacion = "0";
                gui.modalidad = modalidad;
                gui.sucursal = sucursal;
                gui.getClase();
                gui.ShowDialog();
                if (gui.estudio != null)
                {
                    if (!validarEstudioRepetido(gui.estudio))
                    {
                        int idaseguradora;
                        if (int.TryParse(aseguradora, out idaseguradora))
                        {
                            var _est = new EstudiosDAO().getPrecioEstudio(gui.estudio.codigoestudio,
                        idaseguradora);
                            if (_est != null)
                            {
                                gui.estudio.precio = _est.precio;
                                gui.estudio.idmoneda = _est.idmoneda;
                                frmTurnoMedico guiTurno = new frmTurnoMedico();
                                guiTurno.cargarGUI(gui.estudio, dtpFecha_reserva.SelectedDate.Value, int.Parse(cbosucursal.SelectedValue.ToString()), cboModalidad.SelectedValue.ToString());
                                guiTurno.ShowDialog();
                                if (gui.estudio.fechareserva != dtpFecha_reserva.SelectedDate.Value)
                                    if (!validarHorario(gui.estudio))
                                    {
                                        gui.estudio.fechareserva = dtpFecha_reserva.SelectedDate.Value;
                                        gui.estudio.turno = null;
                                        MessageBox.Show("No se acepta la misma HORA en varios estudios.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                                    }
                                if (lblcodigocarta.Content != null || lblcodigocarta.Content.ToString() != "")
                                {
                                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                                    {
                                        int paciente = int.Parse(lblidpaciente.Content.ToString());
                                        string idestudio = gui.estudio.codigoestudio.Substring(3);
                                        var carta=
                                            db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == lblcodigocarta.Content.ToString() && x.codigopaciente == paciente);
                                        var estudio = db.ESTUDIO_CARTAGAR.SingleOrDefault(x => x.codigocartagarantia == lblcodigocarta.Content.ToString() && x.codigopaciente == paciente && x.codigoestudio.Contains(idestudio));
                                        if (estudio != null)
                                        {
                                            //cobertura
                                            if (estudio.cobertura_det == null)
                                                gui.estudio.cobertura = carta.cobertura;
                                            else
                                                gui.estudio.cobertura = estudio.cobertura_det.Value;
                                            //descuento
                                            if (estudio.descuento == null)
                                                gui.estudio.descuento = ((gui.estudio.cobertura * gui.estudio.precio) / 100) * -1.0;
                                            else
                                                gui.estudio.descuento = estudio.descuento.Value;

                                        }
                                    }
                                }
                                LstEstudios.Add(gui.estudio);

                                listarEstudios();
                            }
                            else
                            {
                                MessageBox.Show("No esta registrado un precio en el sistema para el estudio.\n Comuniquese con el área de Atencion al Cliente o Convenios", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                            }
                        }
                    }
                    else
                        MessageBox.Show("El estudio seleccionado ya esta registrado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            }
        }
        private bool validarEstudioRepetido(Search_Estudio estudio)
        {
            return LstEstudios.Where(x => x.codigoestudio == estudio.codigoestudio).ToList().Count > 0;
        }
        private bool validarHorario(Search_Estudio estudio)
        {
            foreach (var item in LstEstudios)
            {
                if (item.codigoestudio != estudio.codigoestudio)
                    if (item.fechareserva.ToShortTimeString() == estudio.fechareserva.ToShortTimeString())
                        return false;
            }
            return true;
        }
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            limpiarFormulario();
        }
        private void limpiarFormulario()
        {
            lblpaciente.Content = "";
            lblidpaciente.Content = "";
            cbosucursal.SelectedIndex = 0;
            txtcmp.Text = "";
            lblMedico.Content = "";
            LstEstudios = new List<Search_Estudio>();
            listarEstudios();

            cboModalidad.SelectedIndex = 0;
            txtidAseguradora.Text = "";
            lblAseguradora.Content = "";
            lblrucAseguradora.Content = "";
            lblidClinica.Content = "";
            lblclinica.Content = "";
            lblcodigocarta.Content = "";
            txtcomentarios.Text = "";
            txtnewcomentarios.Text = "";
            txtindicaciones.Text = "";
            txtTotal.Text = "";
            dtpFecha_reserva.SelectedDate = DateTime.Now;
            cboModalidad.SelectedIndex = 0;


            //chkAmpliacion.IsChecked = false;
            chkSedacion.IsChecked = false;
            chkClaustrofobico.IsChecked = false;
            chkVip.IsChecked = false;
        }
        private void btnguardar_Click(object sender, RoutedEventArgs e)
        {

            var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF10707"));
            var borde = new Thickness(2.0);
            if (isValid())
            {
                try
                {
                    CITA cita = new CITA();
                    List<EXAMENXCITA> estudios = new List<EXAMENXCITA>();
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        cita.codigopaciente = int.Parse(lblidpaciente.Content.ToString());
                        cita.codigoclinica = int.Parse(lblidClinica.Content.ToString());
                        var clinica = db.CLINICAHOSPITAL.Where(x => x.codigoclinica == cita.codigoclinica).SingleOrDefault();
                        if (clinica != null)
                            cita.codigozona = clinica.codigozona;
                        cita.citavip = chkVip.IsChecked.Value;
                        cita.sedacion = chkSedacion.IsChecked.Value;
                        cita.montototal = float.Parse(txtTotal.Text);
                        cita.fechacreacion = Tool.getDatetime(); ;
                        cita.fechareserva = dtpFecha_reserva.SelectedDate.Value;
                        cita.claustrofobico = chkClaustrofobico.IsChecked.Value;
                        cita.observacion = txtcomentarios.Text + "\n" + txtnewcomentarios.Text;
                        cita.cmp = txtcmp.Text;
                        cita.codigocompaniaseguro = int.Parse(txtidAseguradora.Text);
                        cita.ruc = lblrucAseguradora.Content.ToString();
                        cita.codigomodalidad = int.Parse(cboModalidad.SelectedValue.ToString());
                        cita.codigounidad = int.Parse(cbosucursal.SelectedValue.ToString().Substring(0, 1));
                        cita.ampliatorio = chkAmpliacion.IsChecked.Value;
                        cita.nombrepc = Environment.MachineName;
                        cita.codigousuario = session.codigousuario;
                        cita.categoria = cboatencion.Text;
                        if (lblcodigocarta.Content != null && lblcodigocarta.Content.ToString() != "")
                            cita.codigocartagarantia = lblcodigocarta.Content.ToString();

                        //estudios
                        foreach (var item in LstEstudios)
                        {
                            EXAMENXCITA est = new EXAMENXCITA();
                            est.codigopaciente = cita.codigopaciente;
                            est.horacita = item.turno.hora;
                            est.precioestudio = float.Parse(item.precio.ToString());
                            est.codigocompaniaseguro = cita.codigocompaniaseguro;
                            est.ruc = cita.ruc;
                            est.codigoequipo = item.turno.codigoequipo;
                            est.codigoclase = item.codigoclase;
                            est.codigomodalidad = int.Parse(item.codigoestudio.Substring(3, 2));
                            est.codigounidad = int.Parse(item.codigoestudio.Substring(0, 1));
                            est.estadoestudio = "C";
                            est.codigomoneda = item.idmoneda;
                            estudios.Add(est);
                        }

                        if (new CitaDAO().addCita(cita, LstEstudios))
                        {
                            MessageBox.Show("Se registro la Cita con éxito", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            limpiarFormulario();
                        }
                        else
                            MessageBox.Show("No se pudo registrar la Cita", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private bool isValid()
        {
            string msj = "";
            if (lblidpaciente.Content.ToString() == "")
                msj += "- Seleccione un Paciente.\n";
            if (txtidAseguradora.Text == "")
                msj += "- Seleccione una Aseguradora.\n";
            if (lblidClinica.Content.ToString() == "")
                msj += "- Seleccione una Clinica de Procedencia.\n";
            if (txtcmp.Text == "")
                msj += "- Seleccione un Médico Referente.\n";



            //verificar que hay más de 1 estudio
            if (LstEstudios.Count == 0)
                msj += "- Debe agregar por lo menos un estudio.\n";
            else
            {
                //que todos los estudios tengan turnos
                if (LstEstudios.Where(x => x.turno == null).ToList().Count > 0)
                    msj += "- Todos los estudios deben tener turno.\n";
                else
                {
                    //Verificando si es sedacion sea apartie de las 8:15
                    if (chkSedacion.IsChecked.Value)
                    {
                        var inicio = LstEstudios.OrderBy(x => x.fechareserva).FirstOrDefault();
                        DateTime sedacion = new DateTime(inicio.fechareserva.Year, inicio.fechareserva.Month, inicio.fechareserva.Day, 8, 15, 0);
                        if (DateTime.Compare(sedacion, inicio.fechareserva) > 0)
                            msj += "- No se aceptan SEDACIONES antes de las 08:15.\nComuniquese con la Dra. Milagritos Cerdan.\n";
                    }
                    // verificacion de restricciones de citas
                    string[] estudios = new string[] { "101010508000", "101010512000", "101010511000", "101010513000" };
                    if (LstEstudios.Where(x => estudios.Contains(x.codigoestudio)).ToList().Count > 0)
                    {
                        int cantidadcitada = new CitaDAO().getCantEstudiosRestringidos(dtpFecha_reserva.SelectedDate.Value.ToShortDateString());
                        int cantidadmaxima = new CitaDAO().getCantEstudioPermitidos(1);
                        if (cantidadcitada >= cantidadmaxima)
                            msj += "- No se puede registrar esta CITA, por que sobrepasa el limite máximo permitido de estudios restringidos por Día.\nEsta es una disposicion dada por la Dirección Médica.\n";
                    }

                    //verificacion de turnos Libres
                    foreach (var item in LstEstudios)
                    {
                        if (!new CitaDAO().isTurnoLibre(item.fechareserva, item.turno.codigoequipo))
                            msj += "- En turno que eligio para el estudio de " + item.estudio + " esta ocupado busque otro que este disponible.\n";
                    }
                }
            }

            if (msj != "")
            {
                MessageBox.Show("Verifique los siguientes errores:\n" + msj, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;
        }
        private void cleanError()
        {
            var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFABADB3"));
            var borde = new Thickness(1.0);
            btnClinica.BorderBrush = brush;
            btnClinica.BorderThickness = borde;
            txtcmp.BorderBrush = brush;
            txtcmp.BorderThickness = borde;
            gridEstudios.BorderBrush = brush;
            gridEstudios.BorderThickness = borde;
            txtidAseguradora.BorderBrush = brush;
            txtidAseguradora.BorderThickness = borde;

        }
        private void btnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            buscarPaciente();
        }
        public void buscarPaciente()
        {
            frmSearchPaciente gui = new frmSearchPaciente();
            gui.ShowDialog();
            if (gui.paciente != null)
            {
                lblpaciente.Content = gui.paciente.apellidos.ToUpper().Trim() + ", " + gui.paciente.nombres.ToUpper().Trim();
                lblidpaciente.Content = gui.paciente.codigopaciente;
            }
            else
            {
                lblpaciente.Content = "";
                lblidpaciente.Content = "";
            }
        }

        private void dtpFecha_reserva_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in LstEstudios)
            {
                item.fechareserva = dtpFecha_reserva.SelectedDate.Value;
                item.turno = null;
            }
            listarEstudios();
        }

        private void btnSepararTurno_Click(object sender, RoutedEventArgs e)
        {
            frmBloqueoTurno gui = new frmBloqueoTurno();
            gui.cargarGUI(session, dtpFecha_reserva.SelectedDate.Value);
            gui.Show();
            gui.listarTurnos();
        }

        private void btnSolicitarTurno_Click(object sender, RoutedEventArgs e)
        {
            frmSolicitarEntreTurno gui = new frmSolicitarEntreTurno();
            gui.cargarGUI(session);
            gui.ShowDialog();
        }

        private void RadContextHistoria_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

            if (lblidpaciente.Content == null && lblidpaciente.Content.ToString() == "")
            {
                MenuItemHistoria.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuItemHistoria.Visibility = Visibility.Visible;
                MenuItemHistoria.Header = "Buscar Historia del Paciente:\"" + lblpaciente.Content.ToString() + "\"";
            }
        }

        private void MenuItemHistoria_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            int codigopaciente = 0;
            if (lblidpaciente.Content != null)
            {
                if (int.TryParse(lblidpaciente.Content.ToString(), out codigopaciente))
                {
                    frmHistoriaPaciente gui = new frmHistoriaPaciente();
                    gui.Show();
                    gui.buscarPaciente(codigopaciente.ToString(), 2);
                }
                else
                    MessageBox.Show("Ocurrio un error al buscar la hitoria, vuelva a intentar o seleccione nuevamente el paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Seleccione un Paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void RadContextCarta_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (lblcodigocarta.Content == null && lblcodigocarta.Content.ToString() == "")
            {
                MenuItemCarta.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuItemCarta.Visibility = Visibility.Visible;
                MenuItemCarta.Header = "Buscar Carta:\"" + lblcodigocarta.Content.ToString() + "\"";
            }
        }

        private void MenuItemCarta_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var carta = lblcodigocarta.Content.ToString();
            var paciente = lblidpaciente.Content.ToString();

            var _cart = new CartaDAO().getCartaxCodigo(carta, int.Parse(paciente));
            var detalle = new CartaDAO().getDetalleCartaxCodigo(carta, int.Parse(paciente));
            if (_cart != null)
            {
                frmCarta.frmCarta gui = new frmCarta.frmCarta();
                gui.cargarGUI(session, true);
                gui.Show();
                new CartaDAO().insertLog(carta.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + carta.ToString());
                gui.setCartaGarantia(_cart, detalle, false);
            }

        }

        public void setCita(CITA cita, List<Search_Estudio> estudios, bool bloquearDatos)
        {
            if (bloquearDatos)
            {

                btnBuscarPaciente.IsEnabled = false;
                btnBuscarAseguradora.IsEnabled = false;
                txtidAseguradora.IsEnabled = false;
                btnBuscarMedico.IsEnabled = false;
                txtcmp.IsEnabled = false;
                btnBuscarAseguradora.IsEnabled = false;
            }
            btnBuscarEstudio.IsEnabled = true;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                lblidpaciente.Content = cita.codigopaciente;
                var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == cita.codigopaciente);
                lblpaciente.Content = paciente.apellidos.ToUpper().Trim() + ", " + paciente.nombres.ToUpper().Trim();
                lblidClinica.Content = cita.codigoclinica;
                var clinica = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == cita.codigoclinica);
                lblclinica.Content = clinica.razonsocial;
                if (cita.categoria == "" || cita.categoria == null)
                    cboatencion.SelectedIndex = 0;
                else
                    cboatencion.Text = cita.categoria;

                chkClaustrofobico.IsChecked = cita.claustrofobico;
                chkVip.IsChecked = cita.citavip;
                chkSedacion.IsChecked = cita.sedacion;
                chkAmpliacion.IsChecked = cita.ampliatorio;

                txtidAseguradora.Text = cita.codigocompaniaseguro.ToString();
                lblrucAseguradora.Content = cita.ruc;
                var _aseguradora = new UtilDAO().getAseguradoraxCodigo(txtidAseguradora.Text);
                lblAseguradora.Content = _aseguradora.descripcion;
                txtcmp.Text = cita.cmp;
                var medico = db.MEDICOEXTERNO.SingleOrDefault(x => x.cmp == cita.cmp);
                lblMedico.Content = medico.apellidos + ", " + medico.nombres;

                cboModalidad.SelectedValue = cita.codigomodalidad.ToString();
                dtpFecha_reserva.SelectedDate = cita.fechareserva;
                //estudios
                LstEstudios = estudios;
                listarEstudios();

                txtcomentarios.Text = cita.observacion;

                if (cita.codigocartagarantia != null)
                {
                    lblcodigocarta.Content = cita.codigocartagarantia;
                    txtcobertura_sedacion.Value = estudios.Select(x => x.cobertura).Max();
                    calcularSedacionContraste(txtcobertura_sedacion.Value.Value);
                }


            }

        }

        private void chkSedacion_Checked(object sender, RoutedEventArgs e)
        {
            if (lblcodigocarta.Content != null && lblcodigocarta.Content.ToString() != "")
                txtcobertura_sedacion.Visibility = Visibility.Visible;
        }

        private void chkSedacion_Unchecked(object sender, RoutedEventArgs e)
        {
            if (lblcodigocarta.Content != null && lblcodigocarta.Content.ToString() != "")
                txtcobertura_sedacion.Visibility = Visibility.Collapsed;
        }

        private void txtcobertura_sedacion_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            calcularSedacionContraste(e.NewValue.Value);
        }




    }
}
