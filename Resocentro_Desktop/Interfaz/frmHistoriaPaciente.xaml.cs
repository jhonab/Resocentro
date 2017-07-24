using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz;
using Resocentro_Desktop.Interfaz.administracion;
using Resocentro_Desktop.Interfaz.Caja;
using Resocentro_Desktop.Interfaz.Caja.RegistroCobranza;
using Resocentro_Desktop.Interfaz.Cobranza;
using Resocentro_Desktop.Interfaz.frmCarta;
using Resocentro_Desktop.Interfaz.frmDocumento;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para frmHistoriaPaciente.xaml
    /// </summary>
    public partial class frmHistoriaPaciente : Window
    {
        MySession session;
        bool isCallbyOtherGUI = false;
        Window windowsCall = null;
        public frmHistoriaPaciente()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
        }
        private async void txtfiltro_KeyDown(object sender, KeyEventArgs e)
        {
            var filtro = txtfiltro.Text.Trim();
            string nombre = txtnombre.Text.Trim(), dni = txtDNi.Text.Trim();
            if (Key.Enter == e.Key)
                if (filtro + nombre + dni != "")
                {
                    gridPacientes.IsBusy = true;
                    await buscarPaciente(filtro, 0, nombre, dni);
                    gridPacientes.IsBusy = false;
                    txtfiltro.Text = "";
                    txtnombre.Text = "";
                    txtDNi.Text = "";
                }
        }
        private async void txtnum_examen_KeyDown(object sender, KeyEventArgs e)
        {
            var filtro = 0;
            if (Key.Enter == e.Key)
                if (int.TryParse(txtnum_examen.Text, out filtro))
                {
                    gridPacientes.IsBusy = true;
                   await buscarPaciente(filtro.ToString(), 1);
                   gridPacientes.IsBusy = false;
                   txtnum_examen.Text = "";
                }
        }
        private async void txtnum_admision_KeyDown(object sender, KeyEventArgs e)
        {
            var filtro = 0;
            if (Key.Enter == e.Key)
                if (int.TryParse(txtadmision.Text, out filtro))
                {
                    gridPacientes.IsBusy = true;
                    await buscarPaciente(filtro.ToString(),3);
                    gridPacientes.IsBusy = false;
                    txtadmision.Text = "";
                }
        }
        /// <summary>
        /// buscar historia clinica de pacientes
        /// </summary>
        /// <param name="filtro">parametro para realizar busqueda</param>
        /// <param name="tipo">0 filtro por nombre,apellido o dni,1 filtro por numero de examen y 2 filtro por codigo de paciente, 3 filtro por atencion</param>
        /// 
        public async Task<bool> buscarPaciente(string filtro, int tipo, string nombre = "", string dni = "")
        {
            try
            {
                gridPacientes.ItemsSource = null;
                gridPacientes.IsBusy = true;
                List<PacienteEntity> lista = new List<PacienteEntity>();
                //por conincidencia
                if (tipo == 0)
                {

                    var result = new PacienteDAO().getBuscarPacientexCoincidencia(filtro, nombre, dni);
                    foreach (var item in result)
                    {
                        var p = new PacienteEntity();
                        p.nacionalidad = item.nacionalidad;
                        p.direccion = item.direccion;
                        p.email = item.email;
                        p.celular = item.celular;
                        p.telefono = item.telefono;
                        p.fechanace = item.fechanace;
                        p.sexo = item.sexo;
                        p.nombres = item.nombres;
                        p.apellidos = item.apellidos;
                        p.dni = item.dni;
                        p.codigopaciente = item.codigopaciente;
                        p.tipo_doc = item.tipo_doc;
                        p.IsProtocolo = item.IsProtocolo == null ? false : item.IsProtocolo.Value;
                        lista.Add(p);
                    }
                    gridPacientes.ItemsSource = lista;
                }
                else if (tipo == 1)
                {
                    var eatencion = new EAtencionDAO().getEAtencionxCodigo(int.Parse(filtro));
                    if (eatencion != null)
                    {
                        var item = new PacienteDAO().getPaciente(eatencion.codigopaciente);
                        var p = new PacienteEntity();
                        p.nacionalidad = item.nacionalidad;
                        p.direccion = item.direccion;
                        p.email = item.email;
                        p.celular = item.celular;
                        p.telefono = item.telefono;
                        p.fechanace = item.fechanace;
                        p.sexo = item.sexo;
                        p.nombres = item.nombres;
                        p.apellidos = item.apellidos;
                        p.dni = item.dni;
                        p.codigopaciente = item.codigopaciente;
                        p.tipo_doc = item.tipo_doc;
                        p.IsProtocolo = item.IsProtocolo == null ? false : item.IsProtocolo.Value;
                        lista.Add(p);
                        gridPacientes.ItemsSource = lista;
                        if (lista.Count > 0)
                        {
                            setTitulos(p.apellidos.ToUpper() + ", " + p.nombres.ToUpper());
                            await buscarHistoriapaciente(eatencion.codigopaciente, eatencion.numerocita);
                        }
                    }
                }
                else if (tipo == 2)
                {
                    var item = new PacienteDAO().getPaciente(int.Parse(filtro));
                    var p = new PacienteEntity();
                    p.nacionalidad = item.nacionalidad;
                    p.direccion = item.direccion;
                    p.email = item.email;
                    p.celular = item.celular;
                    p.telefono = item.telefono;
                    p.fechanace = item.fechanace;
                    p.sexo = item.sexo;
                    p.nombres = item.nombres;
                    p.apellidos = item.apellidos;
                    p.dni = item.dni;
                    p.codigopaciente = item.codigopaciente;
                    p.tipo_doc = item.tipo_doc;
                    p.IsProtocolo = item.IsProtocolo == null ? false : item.IsProtocolo.Value;
                    lista.Add(p);
                    gridPacientes.ItemsSource = lista;
                    if (lista.Count > 0)
                    {
                        setTitulos(p.apellidos.ToUpper() + ", " + p.nombres.ToUpper());
                        await buscarHistoriapaciente(item.codigopaciente);

                    }
                }
                else if (tipo == 3)
                {
                    var eatencion = new EAtencionDAO().getEAtencionxAtencion(int.Parse(filtro));
                    if (eatencion != null)
                    {
                        var item = new PacienteDAO().getPaciente(eatencion.codigopaciente);
                        var p = new PacienteEntity();
                        p.nacionalidad = item.nacionalidad;
                        p.direccion = item.direccion;
                        p.email = item.email;
                        p.celular = item.celular;
                        p.telefono = item.telefono;
                        p.fechanace = item.fechanace;
                        p.sexo = item.sexo;
                        p.nombres = item.nombres;
                        p.apellidos = item.apellidos;
                        p.dni = item.dni;
                        p.codigopaciente = item.codigopaciente;
                        p.tipo_doc = item.tipo_doc;
                        p.IsProtocolo = item.IsProtocolo == null ? false : item.IsProtocolo.Value;
                        lista.Add(p);
                        gridPacientes.ItemsSource = lista;
                        if (lista.Count > 0)
                        {
                            setTitulos(p.apellidos.ToUpper() + ", " + p.nombres.ToUpper());
                            await buscarHistoriapaciente(eatencion.codigopaciente, eatencion.numerocita);
                        }
                    }
                }
                else { }

                gridPacientes.IsBusy = false;
                if (lista.Count == 0)
                {
                    MessageBox.Show("No se encontro ningun registro con el filtro ingresado \"" + filtro + "\". \nVuelva a intentar", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private async Task<bool> buscarHistoriapaciente(int codigopaciente, int cita = 0)
        {
            var result = true;
            //txtfiltro.Text = "";
            txtnum_examen.Text = "";

            gridCitas.ItemsSource = null;
            gridAtenciones.ItemsSource = null;
            gridPagos.ItemsSource = null;
            gridAquisicion.ItemsSource = null;
            gridCartas.ItemsSource = null;
            //var hClinica = new EAtencionDAO().getHclinicaPaciente(codigopaciente);
            //HistoriaClinicaPaciente hClinica = new HistoriaClinicaPaciente();

           /* 
            await cargarcitas(codigopaciente);
            await cargarAtenciones(codigopaciente);
            await cargarPagos(codigopaciente);
            await cargarAdquisicion(codigopaciente);
            await cargarCarta(codigopaciente);
            */
           
            await Task.WhenAll(cargarcitas(codigopaciente),cargarAtenciones(codigopaciente),cargarPagos(codigopaciente),cargarAdquisicion(codigopaciente),cargarCarta(codigopaciente));

            if (cita != 0)
                seleccionarItem(cita);
            return result;
        }
        public async Task<bool> cargarcitas(int codigopaciente)
        {
            gridCitas.IsBusy = true;
            gridCitas.ItemsSource = await new EAtencionDAO().getHclinicaPacienteCita(codigopaciente);
            gridCitas.IsBusy = false;
            return true;
        }
        public async Task<bool> cargarAtenciones(int codigopaciente)
        {
            gridAtenciones.IsBusy = true;
            gridAtenciones.ItemsSource = await new EAtencionDAO().getHclinicaPacienteAdmision(codigopaciente);
            gridAtenciones.IsBusy = false;
            return true;

        }
        public async Task<bool> cargarPagos(int codigopaciente)
        {
            gridPagos.IsBusy = true;
            gridPagos.ItemsSource = await new EAtencionDAO().getHclinicaPacienteDocumento(codigopaciente);
            gridPagos.IsBusy = false;
            return true;

        }
        public async Task<bool> cargarAdquisicion(int codigopaciente)
        {
            gridAquisicion.IsBusy = true;
            gridAquisicion.ItemsSource = await new EAtencionDAO().getHclinicaPacienteTecnologo(codigopaciente);
            gridAquisicion.IsBusy = false;
            return true;

        }
        public async Task<bool> cargarCarta(int codigopaciente)
        {
            gridCartas.IsBusy = true;
            gridCartas.ItemsSource = await new EAtencionDAO().getHclinicaPacienteCarta(codigopaciente);
            gridCartas.IsBusy = false;
            return true;

        }
        public void seleccionarItem(int cita)
        {
            try
            {
                int count = 0;
                //cita
                var lstcita = (List<HCCita>)gridCitas.Items.SourceCollection;
                foreach (var item in lstcita)
                {
                    if (item.num_cita == cita)
                    {
                        item.isSeleccionado = true;
                        gridCitas.ScrollIndexIntoView(count);
                    }
                    else
                        item.isSeleccionado = false;
                    count++;
                }
                //admision
                var lstadmision = (List<HCAdmision>)gridAtenciones.Items.SourceCollection;
                count = 0;
                foreach (var item in lstadmision)
                {
                    if (item.cita == cita)
                    {
                        item.isSeleccionado = true;
                        gridAtenciones.ScrollIndexIntoView(count);
                    }
                    else
                        item.isSeleccionado = false;
                    count++;
                }
                //Pagos
                var lstpagos = (List<HCPagos>)gridPagos.Items.SourceCollection;
                count = 0;
                foreach (var item in lstpagos)
                {
                    if (item.cita == cita)
                    {
                        item.isSeleccionado = true;
                        gridPagos.ScrollIndexIntoView(count);
                    }
                    else
                        item.isSeleccionado = false;
                    count++;
                }
                //Adquisiones
                var lstadquisicion = (List<HCAdquisicion>)gridAquisicion.Items.SourceCollection;
                count = 0;
                foreach (var item in lstadquisicion)
                {
                    if (item.cita == cita)
                    {
                        item.isSeleccionado = true;
                        gridAquisicion.ScrollIndexIntoView(count);
                    }
                    else
                        item.isSeleccionado = false;
                    count++;
                }
                //Cartas
                var lstcarta = (List<HCCarta>)gridCartas.Items.SourceCollection;
                count = 0;
                foreach (var item in lstcarta)
                {
                    if (item.cita == cita)
                    {
                        item.isSeleccionado = true;
                        gridCartas.ScrollIndexIntoView(count);
                    }
                    else
                        item.isSeleccionado = false;
                    count++;
                }

                gridCitas.ItemsSource = null;
                gridAtenciones.ItemsSource = null;
                gridPagos.ItemsSource = null;
                gridAquisicion.ItemsSource = null;
                gridCartas.ItemsSource = null;

                gridCitas.ItemsSource = lstcita;
                gridAtenciones.ItemsSource = lstadmision;
                gridPagos.ItemsSource = lstpagos;
                gridAquisicion.ItemsSource = lstadquisicion;
                gridCartas.ItemsSource = lstcarta;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void gridPacientes_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            PacienteEntity item = (PacienteEntity)e.Row.DataContext;
            if (item != null)
            {
                setTitulos(item.apellidos.ToUpper() + ", " + item.nombres.ToUpper());
                await buscarHistoriapaciente(item.codigopaciente);
            }
            else
            {
                setTitulos("");
            }
        }
        public void setTitulos(string nombre)
        {
            lblcitas.Content = "Citas de " + nombre.ToUpper();
            lbladmision.Content = "Admisiones de " + nombre.ToUpper();
            lblpagos.Content = "Pagos de " + nombre.ToUpper();
            lbladquisicion.Content = "Adquisiciones de " + nombre.ToUpper();
            lblcarta.Content = "Cartas de " + nombre.ToUpper();
        }
        private void gridCitas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            HCCita item = (HCCita)e.Row.DataContext;
            if (isCallbyOtherGUI)
            {
                var gui = ((frmReciboProvisional)windowsCall);
                gui.txtcita.Text = item.num_cita.ToString();
                gui.buscarData(item.num_cita.ToString());
                this.Close();
            }
            else
            {

                if (item != null)
                    if (item.num_cita > 0)
                        seleccionarItem(item.num_cita);
                    else
                        seleccionarItem(-1);
                else
                    seleccionarItem(-1);
            }
        }
        private void gridAtenciones_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            HCAdmision item = (HCAdmision)e.Row.DataContext;
            if (item != null)
                if (item.cita > 0)
                    seleccionarItem(item.cita);
                else
                    seleccionarItem(-1);
            else
                seleccionarItem(-1);
        }
        private void gridPagos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            HCPagos item = (HCPagos)e.Row.DataContext;
            if (item != null)
                if (item.cita > 0)
                    seleccionarItem(item.cita);
                else
                    seleccionarItem(-1);
            else
                seleccionarItem(-1);
        }
        private void gridAquisicion_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            HCAdquisicion item = (HCAdquisicion)e.Row.DataContext;
            if (item != null)
                if (item.cita > 0)
                    seleccionarItem(item.cita);
                else
                    seleccionarItem(-1);
            else
                seleccionarItem(-1);
        }
        private void gridCartas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            HCCarta item = (HCCarta)e.Row.DataContext;
            if (item != null)
                if (item.cita > 0)
                    seleccionarItem(item.cita);
                else
                    seleccionarItem(-1);
            else
                seleccionarItem(-1);
        }
        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridCartas.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }

            HCCarta item = (HCCarta)this.gridCartas.SelectedItem;
            if (item == null)
            {
                MenuContext_Carta.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuContext_Carta.Visibility = Visibility.Visible;
                MenuItemAbrirCarta.Header = "Abrir Carta: \"" + item.id + "\"";
            }
        }
        private void MenuItemAbrir_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCCarta item = (HCCarta)this.gridCartas.SelectedItem;
            if (item != null)
            {
                var _cart = new CartaDAO().getCartaxCodigo(item.id, item.codigopaciente);
                var detalle = new CartaDAO().getDetalleCartaxCodigo(item.id, item.codigopaciente);
                if (_cart != null)
                {
                    frmCarta gui = new frmCarta();
                    gui.cargarGUI(session, true);
                    gui.Show();
                    new CartaDAO().insertLog(item.id.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + item.id.ToString());
                    gui.setCartaGarantia(_cart, detalle, false);
                }
            }
        }
        private void RadContextMenuPago_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridPagos.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            HCPagos item = (HCPagos)this.gridPagos.SelectedItem;
            if (item == null)
            {
                MenuContext_Pagos.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuContext_Pagos.Visibility = Visibility.Visible;
                MenuItemAbrirPagos.Header = "Abrir " + item.documento + ": \"" + item.numero + "\"";
            }
        }
        private void MenuItemAbrirPago_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCPagos item = (HCPagos)this.gridPagos.SelectedItem;
            if (item != null)
            {
                frmDocumento gui = new frmDocumento();
                gui.Show();
                gui.setDocumento(session, item.numero, item.ruc, item.codigopaciente, int.Parse(item.codigoempresa), int.Parse(item.codigosede));
            }
        }
        private void RadContextMenuPaciente_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridPacientes.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            PacienteEntity item = (PacienteEntity)this.gridPacientes.SelectedItem;
            if (item == null)
            {
                MenuItemInfoPaciente.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuItemInfoPaciente.Visibility = Visibility.Visible;
                if (session.roles.Contains("48"))
                    MenuItemCobranzaExterna.Visibility = Visibility.Visible;
            }
        }
        private void MenuItemInfoPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PacienteEntity item = (PacienteEntity)this.gridPacientes.SelectedItem;
            if (item != null)
            {
                frmPaciente gui = new frmPaciente();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    gui.setPaciente(db.PACIENTE.SingleOrDefault(x => x.codigopaciente == item.codigopaciente));
                    // gui.isLector();
                    gui.ShowDialog();
                }
            }

        }
        private void gridPacientes_FilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }
        private void RadContextMenuCita_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridCitas.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            HCCita item = (HCCita)this.gridCitas.SelectedItem;
            if (item != null)
            {
                if (session.roles.Contains(((int)Tipo_Permiso.Sistemas).ToString())) 
                {
                    MenuContext_cambiarEstadoCita.Visibility = Visibility.Visible;
                    MenuContext_eliminarcita.Visibility = Visibility.Visible;
                }
                if (session.roles.Contains(((int)Tipo_Permiso.Asignar_Cortesias).ToString()) && item.estado!="X")
                {
                    MenuContext_AsignarCortesia.Visibility = Visibility.Visible;
                }
                else
                {
                    MenuContext_AsignarCortesia.Visibility = Visibility.Collapsed;
                }
            }
            
        }
        private void MenuItemCortesias_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCCita item = (HCCita)this.gridCitas.SelectedItem;
            frmCortesias gui = new frmCortesias();
            gui.Owner = this;
            gui.cargarGUI(session, item.num_cita,item.codigopaciente);
            gui.ShowDialog();

        }
        private void MenuItemCambiarEstadoDetalle_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCCita item = (HCCita)this.gridCitas.SelectedItem;
            frmCambiarEstadoCita gui = new frmCambiarEstadoCita();
            gui.cboestado.Text = item.estado;
            gui.ShowDialog();
            if (gui.DialogResult == true)
            {
                var estado = gui.cboestado.Text;
                if (estado != "I" && estado != "V")
                {
                    if (estado != "X")
                    {
                        if (new CitaDAO().UpdateEstadoDetalleCita(item.codigoexamencita, estado))
                        {
                            var task = buscarHistoriapaciente(item.codigopaciente);

                            MessageBoxTemporal.Show("Se actualizo la Cita", "Exito", 10, true);
                        }
                        else
                            MessageBoxTemporal.Show("No se actualizo la Cita", "Exito", 10, true);
                    }
                    else
                    {
                        frmCancelarExamen guicancelar = new frmCancelarExamen();
                        guicancelar.cbomotivo.SelectedIndex = 0;
                        guicancelar.ShowDialog();
                        if (guicancelar.DialogResult == true)
                        {
                            new CitaDAO().UpdateEstadoDetalleCita(item.codigoexamencita, estado, guicancelar.cbomotivo.Text, session.codigousuario);
                            MessageBoxTemporal.Show("Se actualizo la Cita", "Exito", 10, true);

                        }
                    }
                }
                else
                    MessageBoxTemporal.Show("No se puede asignar el estado a una Cita", "Exito", 10, true);
            }
        }
        private void MenuItemDeleteDetalle_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (MessageBox.Show("¿Desea eliminar permanentemente el registro de la Cita?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                HCCita item = (HCCita)this.gridCitas.SelectedItem;
                if (new CitaDAO().EliminarDetalleCita(item.codigoexamencita))
                {
                    var task = buscarHistoriapaciente(item.codigopaciente);
                    MessageBoxTemporal.Show("Se elimino la cita", "Exito", 10, true);
                }
                else
                    MessageBoxTemporal.Show("No se elimino la cita", "Exito", 10, true);
            }
        }
        private void RadContextMenuAtencion_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridAtenciones.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            if (session.roles.Contains(((int)Tipo_Permiso.Sistemas).ToString()) || session.roles.Contains(((int)Tipo_Permiso.Realizar_Cobranza).ToString()))
            {
                HCAdmision item = (HCAdmision)this.gridAtenciones.SelectedItem;
                if (item != null)
                {
                    MenuContext_Atencion.Visibility = Visibility.Visible;
                    if (session.roles.Contains(((int)Tipo_Permiso.Sistemas).ToString()))
                        MenuCambiarEstado.Visibility = Visibility.Visible;
                    if (session.roles.Contains(((int)Tipo_Permiso.Realizar_Cobranza).ToString()))
                        MenuRealizarCobranza.Visibility = Visibility.Visible;

                }
                else
                    MenuContext_Atencion.Visibility = Visibility.Collapsed;
            }
            else
                MenuContext_Atencion.Visibility = Visibility.Collapsed;
        }
        private void MenuItemCambiarEstadoDetalleAtencion_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCAdmision item = (HCAdmision)this.gridAtenciones.SelectedItem;
            frmCambiarEstadoCita gui = new frmCambiarEstadoCita();
            gui.cboestado.Text = item.estado;
            gui.ShowDialog();
            if (gui.DialogResult == true)
            {
                try
                {
                    var estado = gui.cboestado.Text;
                    if (estado != "C" && estado != "K")
                    {
                        if (estado != "X")
                        {
                            if (new CitaDAO().UpdateEstadoDetalleAtencion(item.examen.ToString(), estado))
                            {
                                var task = buscarHistoriapaciente(item.codigopaciente);
                                MessageBoxTemporal.Show("Se actualizo la Atención", "Exito", 10, true);
                                int codigoecita = new CitaDAO().getCodigoDetalleCIta(item.cita.ToString(), item.codigoestudio);
                                if (codigoecita > 0)
                                {
                                    if (estado == "A" || estado == "R")
                                    {
                                        if (new CitaDAO().UpdateEstadoDetalleCita(codigoecita, estado))
                                        {
                                            task = buscarHistoriapaciente(item.codigopaciente);
                                            MessageBoxTemporal.Show("Se actualizo la Cita", "Exito", 10, true);
                                        }
                                        else
                                            MessageBoxTemporal.Show("No se actualizo la Cita", "Exito", 10, true);
                                    }
                                }
                            }
                            else
                                MessageBoxTemporal.Show("No se actualizo la Atención", "Exito", 10, true);
                        }
                        else
                        {
                            frmCancelarExamen guicancelar = new frmCancelarExamen();
                            guicancelar.cbomotivo.SelectedIndex = 0;
                            guicancelar.ShowDialog();
                            if (guicancelar.DialogResult == true)
                            {
                                new CitaDAO().UpdateEstadoDetalleAtencion(item.examen.ToString(), estado);
                                MessageBoxTemporal.Show("Se actualizo la Atención", "Exito", 10, true);

                                /*int numeroexamen = new CitaDAO().getCodigoDetalleCIta(item.cita.ToString(), item.codigoestudio);
                                if (numeroexamen > 0)
                                {
                                    new CitaDAO().UpdateEstadoDetalleCita(numeroexamen, estado, guicancelar.cbomotivo.Text, session.codigousuario);
                                    MessageBoxTemporal.Show("Se actualizo la Cita", "Exito", 10, true);
                                }*/
                            }
                        }

                    }

                    else
                        MessageBoxTemporal.Show("No se puede asignar el estado a una Atencíon", "Exito", 10, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private async void MenuItemDetalleAdquisicion_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCAdquisicion item = (HCAdquisicion)this.gridAquisicion.SelectedItem;
            if (item != null)
            {
                DetalleHCAdquisicion detalle = await new EAtencionDAO().getDetalleAdquisicion(item.examen);
                frmAdquisicion gui = new frmAdquisicion();
                gui.cargarGUI(detalle);
                gui.Show();
            }
        }
        private void RadContextMenuAdquisicion_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridAquisicion.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            HCAdquisicion item = (HCAdquisicion)this.gridAquisicion.SelectedItem;
            if (item != null)
                MenuContext_Aquisicion.Visibility = Visibility.Visible;
            else
                MenuContext_Aquisicion.Visibility = Visibility.Collapsed;
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void MenuItemRealizarCobranza_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {

            HCAdmision item = (HCAdmision)this.gridAtenciones.SelectedItem;
            if (item != null)
            {
                frmSelecionarSede gui1 = new frmSelecionarSede();
                gui1.cargarGUI(session);
                gui1.ShowDialog();
                if (gui1.codigoempresasede != 99)
                {
                    int codigosede = gui1.codigoempresasede;
                    int unidad = codigosede / 100;
                    frmRegistrarCobranza gui = new frmRegistrarCobranza();
                    gui.cargarGUI(session, unidad, codigosede - (unidad * 100), TIPO_COBRANZA.PACIENTE);
                    gui.cargarCobranza(item.atencion, "", true);
                    gui.Show();
                }
            }
        }

        private void MenuItemCobranzaExterna_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            PacienteEntity item = (PacienteEntity)this.gridPacientes.SelectedItem;
            if (item != null)
            {
                frmSelecionarSede gui1 = new frmSelecionarSede();
                gui1.cargarGUI(session);
                gui1.ShowDialog();
                if (gui1.codigoempresasede != 99)
                {
                    int codigosede = gui1.codigoempresasede;
                    int unidad = codigosede / 100;
                    frmFacturacionEspecial gui = new frmFacturacionEspecial();
                    gui.cargarGUI(session, unidad, codigosede - (unidad * 100));
                    gui.cargarCobranza(item.codigopaciente);
                    gui.Show();
                }
            }
        }

        private void MenuItemEncuesta_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCAdmision item = (HCAdmision)this.gridAtenciones.SelectedItem;
            if (item != null)
            {
                string ruta = @"http://serverweb:5126/LectorEncuesta/LectorEncuesta?examen={0}&isVisible=false";
                System.Diagnostics.Process.Start(string.Format(ruta, item.examen));
            }
        }

        private void MenuItemAdjunto_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            HCAdmision item = (HCAdmision)this.gridAtenciones.SelectedItem;
            if (item != null)
            {
                bool carta = false, admision = false;

                if (new CartaDAO().getisFile(item.examen.ToString(), out admision, out carta))
                {
                    if (admision)
                        System.Diagnostics.Process.Start(string.Format(@"http://serverweb:5126/Adjuntos/getFile?examen={0}&tipo=1", item.examen));
                    if (carta)
                        System.Diagnostics.Process.Start(string.Format(@"http://serverweb:5126/Adjuntos/getFile?examen={0}&tipo=2", item.examen));
                }


            }
        }

        public void CallbyOtherGUI(Window gui)
        {
            isCallbyOtherGUI = true;
            windowsCall = gui;
        }

       

       
    }
}
