using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.Cartas;
using Resocentro_Desktop.Interfaz.frmUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
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
using Telerik.Windows.Controls.GridView;
using Excel = Microsoft.Office.Interop.Excel;
namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmCarta.xaml
    /// </summary>
    public partial class frmCarta : Window
    {
        MySession session;
        List<Search_Estudio> LstEstudios;
        List<Adjuntos_Desktop> lstAdjuntos;
        List<Tipo_Adjunto> lsttipoadjunto;
        public bool isClose = false;
        bool isLectura;
        public frmCarta()
        {
            InitializeComponent();
            dtpNacimiento_paciente.SelectedDate = DateTime.Now;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                cbotipo_doc_paciente.ItemsSource = db.TIPO_DOCUMENTO_IDENTIDAD.ToList();
                cbotipo_doc_paciente.SelectedValuePath = "tipo_doc_id";
                cbotipo_doc_paciente.DisplayMemberPath = "tipo_doc_descripcion";
                cbotipo_doc_paciente.SelectedIndex = 0;
            }
        }
        public void cargarGUI(MySession session, bool isLectura)
        {
            this.session = session;
            this.isLectura = isLectura;
            //cargamos los combos segun usuario
            cbosucursal.ItemsSource = new UtilDAO().getSucursales(session.sucursales);
            cbosucursal.SelectedValuePath = "codigoInt";
            cbosucursal.DisplayMemberPath = "nombreShort";
            cbosucursal.SelectedIndex = 0;

            //cargamos el combo de afiliacion

            cboafiliacion.ItemsSource = new UtilDAO().getTipoAfiliacion();
            cboafiliacion.SelectedValuePath = "codigo";
            cboafiliacion.DisplayMemberPath = "nombre";
            cboafiliacion.SelectedValue = "1";

            cboestado.ItemsSource = new UtilDAO().getEstadoCarta();
            cboestado.SelectedValuePath = "nombre";
            cboestado.DisplayMemberPath = "nombre";
            cboestado.SelectedValue = "1";

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                lsttipoadjunto = db.Tipo_Adjunto.Where(x => x.isActivo == true).ToList();
                lstbAdjunto.ItemsSource = lsttipoadjunto;
                lstbAdjunto.SelectedValuePath = "idTipo";
                lstbAdjunto.DisplayMemberPath = "descripcion";
            }

            LstEstudios = new List<Search_Estudio>();

            lstAdjuntos = new List<Adjuntos_Desktop>();

            //Limpiar
            limpiarFormulario();

            //fijamos el cursor
            btnBuscarPaciente.Focus();

            if (session.roles.Contains(((int)Tipo_Permiso.Carta_Garantía).ToString()))
                this.stckBotones.Visibility = Visibility.Visible;
            else
                this.stckBotones.Visibility = Visibility.Collapsed;

            /*if (isLectura)
                this.stckBotones.Visibility = Visibility.Collapsed;*/

        }
        public void buscarPaciente()
        {
            try
            {
                frmSearchPaciente gui = new frmSearchPaciente();
                gui.ShowDialog();
                if (gui.paciente != null)
                {
                    txtnombrepaciente.Text = gui.paciente.apellidos.ToUpper().Trim() + ", " + gui.paciente.nombres.ToUpper().Trim();
                    txtnombre_paciente.Text = gui.paciente.nombres.ToUpper().Trim();
                    txtapellidos_paciente.Text = gui.paciente.apellidos.ToUpper().Trim();
                    dtpNacimiento_paciente.SelectedDate = gui.paciente.fechanace;
                    cbotipo_doc_paciente.SelectedValue = gui.paciente.tipo_doc;
                    txtnum_doc_Paciente.Text = gui.paciente.dni;
                    txttelefono.Text = gui.paciente.telefono;
                    txtcelular.Text = gui.paciente.celular;
                    lblidpaciente.Content = gui.paciente.codigopaciente;
                    buscarCarta(gui.paciente.codigopaciente);
                }
                else
                {
                    txtnombrepaciente.Text = "";
                    txtnombre_paciente.Text = "";
                    txtapellidos_paciente.Text = "";
                    dtpNacimiento_paciente.SelectedDate = DateTime.Now;
                    cbotipo_doc_paciente.SelectedValue = "0";
                    txtnum_doc_Paciente.Text = "";
                    txttelefono.Text = "";
                    txtcelular.Text = "";
                    lblidpaciente.Content = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buscarCarta(int codigopaciente)
        {
            grid_carta.ItemsSource = null;
            grid_carta.ItemsSource = new CartaDAO().getCartaxPaciente(codigopaciente);
        }

        private void btnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            buscarPaciente();
            radTab_carta.SelectedIndex = 1;
            rabtabdato_adicionales.SelectedIndex = 0;
            txtnombre_paciente.Focus();
        }

        private void btnAgregarPaciente_Click(object sender, RoutedEventArgs e)
        {
            frmPaciente gui = new frmPaciente();
            gui.ShowDialog();
            if (gui.paciente != null)
            {
                txtnombrepaciente.Text = gui.paciente.apellidos.ToUpper().Trim() + ", " + gui.paciente.nombres.ToUpper().Trim();
                lblidpaciente.Content = gui.paciente.codigopaciente;
            }
            else
            {
                txtnombrepaciente.Text = "";
                lblidpaciente.Content = "";
            }
        }

        private void cbosucursal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var idempresa = cbosucursal.SelectedValue;
            if (idempresa != null)
            {
                LstEstudios = new List<Search_Estudio>();
                listarEstudios(null);
                getModalidad(idempresa.ToString());
            }
        }
        public void listarEstudios(Search_Estudio search_Estudio)
        {
            if (search_Estudio != null)
                if (LstEstudios.Where(x => x.codigoestudio == search_Estudio.codigoestudio).ToList().Count == 0)
                {
                    search_Estudio.cobertura = search_Estudio.cobertura * 1.0;
                    LstEstudios.Add(search_Estudio);
                }

            gridEstudios.ItemsSource = null;
            gridEstudios.ItemsSource = LstEstudios.ToList();
            txtTotal.Text = LstEstudios.Sum(x => x.precioneto).ToString("#########.##");
            if (txtTotal.Text == "")
                txtTotal.Text = "0";
        }
        public void getModalidad(string empresa)
        {
            cboModalidad.ItemsSource = new UtilDAO().getModalidadxEmpresa(empresa.Substring(0, 1));
            cboModalidad.SelectedValuePath = "codigo";
            cboModalidad.DisplayMemberPath = "nombre";
            cboModalidad.SelectedIndex = 0;
        }

        private void txtcmp_KeyUp(object sender, KeyEventArgs e)
        {
            lblMedico.Content = "";
            if (e.Key == Key.Enter)
            {
                if (txtcmp.Text != "")
                {
                    var _medico = new UtilDAO().getMedicoxCMP(txtcmp.Text, 1);
                    if (_medico.apellidos != null)
                    {
                        lblMedico.Content = _medico.apellidos.ToUpper() + ", " + _medico.nombres.ToUpper();
                        txtcmp.Text = _medico.cmp;
                        btnBuscarMedico.Focus();
                    }
                }
            }
        }

        private void btnBuscarMedico_Click(object sender, RoutedEventArgs e)
        {
            frmSearchMedico gui = new frmSearchMedico();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.medico != null)
            {
                lblMedico.Content = gui.medico.apellidos.ToUpper() + ", " + gui.medico.nombres.ToUpper();
                txtcmp.Text = gui.medico.cmp;
                btnBuscarMedico.Focus();
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
                gui.ampliacion = "0";
                gui.modalidad = modalidad;
                gui.sucursal = sucursal;
                gui.Owner = this;
                gui.getClase();
                gui.ShowDialog();
                if (gui.estudio != null)
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
                            gui.estudio.cobertura = txtcobertura.Value.Value;
                            gui.estudio.descuento = ((gui.estudio.cobertura * gui.estudio.precio) / 100) * -1.0;
                            listarEstudios(gui.estudio);
                            btnBuscarEstudio.Focus();
                        }
                        else
                        {
                            MessageBox.Show("No esta registrado un precio en el sistema para el estudio.\n Comuniquese con el área de Atencion al Cliente o Convenios", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        }
                    }
                }
            }
        }

        private void btnBuscarInsumos_Click(object sender, RoutedEventArgs e)
        {
            var sucursal = cbosucursal.SelectedValue.ToString();
            var modalidad = cboModalidad.SelectedValue.ToString();
            var aseguradora = txtidAseguradora.Text;
            if (sucursal != "" && modalidad != "")
            {
                frmSearchInsumo gui = new frmSearchInsumo();
                gui.ampliacion = "0";
                gui.modalidad = modalidad;
                gui.sucursal = sucursal;
                gui.ShowDialog();
                if (gui.estudio != null)
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
                            gui.estudio.cobertura = txtcobertura.Value.Value;
                            gui.estudio.descuento = ((gui.estudio.cobertura * gui.estudio.precio) / 100) * -1.0;
                            listarEstudios(gui.estudio);
                            btnBuscarEstudio.Focus();
                        }
                        else
                        {
                            MessageBox.Show("No esta registrado un precio en el sistema para el estudio.\n Comuniquese con el área de Atencion al Cliente o Convenios", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        }
                    }
                }
            }
        }
        private void btnBuscarAseguradora_Click(object sender, RoutedEventArgs e)
        {
            frmSearchAseguradora gui = new frmSearchAseguradora();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.aseguradora != null)
            {
                lblAseguradora.Content = gui.aseguradora.descripcion.ToUpper();
                txtidAseguradora.Text = gui.aseguradora.codigocompaniaseguro.ToString();
                lblrucAseguradora.Content = gui.aseguradora.ruc;
                btnBuscarAseguradora.Focus();
                btnBuscarEstudio.IsEnabled = true;
                btnBuscarInsumos.IsEnabled = true;
            }
            else
            {
                btnBuscarEstudio.IsEnabled = false;
                btnBuscarInsumos.IsEnabled = false;
            }

            getProductos_Beneficio();
            LstEstudios = new List<Search_Estudio>();
            listarEstudios(null);
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
                            lblAseguradora.Content = _aseguradora.descripcion.ToUpper();
                            txtidAseguradora.Text = _aseguradora.codigocompaniaseguro.ToString();
                            lblrucAseguradora.Content = _aseguradora.ruc;
                            btnBuscarAseguradora.Focus();
                            btnBuscarEstudio.IsEnabled = true;
                            btnBuscarInsumos.IsEnabled = true;
                        }
                        else
                        {
                            btnBuscarEstudio.IsEnabled = false;
                            btnBuscarInsumos.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                }
                else
                {
                    btnBuscarEstudio.IsEnabled = false;
                    btnBuscarInsumos.IsEnabled = false;
                }

                LstEstudios = new List<Search_Estudio>();
                listarEstudios(null);
                getProductos_Beneficio();
            }
        }
        private void getProductos_Beneficio()
        {
            int idcompa = 0;
            if (int.TryParse(txtidAseguradora.Text, out idcompa))
            {
                if (new CartaDAO().isProductoActivo(idcompa))
                {
                    txtcodigoproducto.IsEnabled = true;
                    btnProducto.IsEnabled = true;
                }
                else
                {
                    txtcodigoproducto.IsEnabled = false;
                    btnProducto.IsEnabled = false;
                }
            }
            else
            {
                txtcodigoproducto.IsEnabled = false;
                btnProducto.IsEnabled = false;
            }
        }

        private void btnClinica_Click(object sender, RoutedEventArgs e)
        {
            frmSearchClinica gui = new frmSearchClinica();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.clinica != null)
            {
                lblidClinica.Content = gui.clinica.codigoclinica;
                lblclinica.Content = gui.clinica.razonsocial.ToUpper();
                //clinica de Negocio
                if (lblclinicaNegicio.Content.ToString() == "" || lblclinicaNegicio.Content == null)
                    lblidClinicaNegicio.Content = gui.clinica.codigoclinica;
                lblclinicaNegicio.Content = gui.clinica.razonsocial.ToUpper();
                btnClinica.Focus();
            }
        }

        private void btnClinicaNegocio_Click(object sender, RoutedEventArgs e)
        {
            frmSearchClinica gui = new frmSearchClinica();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.clinica != null)
            {
                lblidClinicaNegicio.Content = gui.clinica.codigoclinica;
                lblclinicaNegicio.Content = gui.clinica.razonsocial.ToUpper();
                btnClinicaNegocio.Focus();
            }
        }

        private void btnProducto_Click(object sender, RoutedEventArgs e)
        {
            int compania = 0;
            if (int.TryParse(txtidAseguradora.Text, out compania))
            {
                frmSearchProducto gui = new frmSearchProducto();
                gui.compania = compania;
                gui.Owner = this;
                gui.ShowDialog();
                gui.txtnombre.Focus();
                if (gui.producto != null)
                {
                    txtcodigoproducto.Text = gui.producto.SitedCodigoProducto.Trim();
                    lblProducto.Content = gui.producto.descripcion.ToString();
                    lblidProducto.Content = gui.producto.SitedProductoId.ToString();
                }
            }
        }

        private void txtcodigoproducto_KeyUp(object sender, KeyEventArgs e)
        {
            lblProducto.Content = "";
            lblidProducto.Content = "";
            if (e.Key == Key.Enter)
            {
                int compania = 0;
                if (int.TryParse(txtidAseguradora.Text, out compania))
                {
                    if (txtcodigoproducto.Text != "")
                    {
                        buscarProductoCodigoSiteds(compania, txtcodigoproducto.Text);
                    }
                    else
                    {
                        MessageBox.Show("Ingrese el código de Producto", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtcodigoproducto.Text = "";
                        lblProducto.Content = "";
                        lblidProducto.Content = "";
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione la Aseguradora", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtcodigoproducto.Text = "";
                    lblProducto.Content = "";
                    lblidProducto.Content = "";
                }
            }
        }

        private void buscarProductoCodigoSiteds(int compania, string producto)
        {
            var _producto = new CartaDAO().getProductoxNomenclatura(producto, compania);
            if (_producto.SitedCodigoProducto != null)
            {
                txtcodigoproducto.Text = _producto.SitedCodigoProducto.Trim();
                lblProducto.Content = _producto.descripcion.ToString();
                lblidProducto.Content = _producto.SitedProductoId.ToString();
            }
            else
            {
                MessageBox.Show("No existe un Producto con el codigo Ingresado\n Codigo Busqueda: "+producto, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                txtcodigoproducto.Text = "";
                lblProducto.Content = "";
                lblidProducto.Content = "";
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            limpiarFormulario();
            btnBuscarPaciente.Focus();
        }

        private void limpiarFormulario()
        {
            cleanError();
            txtnombrepaciente.Text = "";
            txtnombre_paciente.Text = "";
            txtapellidos_paciente.Text = "";
            dtpNacimiento_paciente.SelectedDate = DateTime.Now;
            cbotipo_doc_paciente.SelectedValue = "";
            txtnum_doc_Paciente.Text = "";
            txttelefono.Text = "";
            txtcelular.Text = "";
            lblidpaciente.Content = "";
            grid_carta.ItemsSource = null;
            lblnumeroProforma.Content = "";
            cbosucursal.SelectedIndex = 0;
            txtidCarta.Text = "";
            txtnumCarta.Text = "";
            txttitular.Text = "";
            txtpoliza.Text = "";
            cboafiliacion.SelectedIndex = -1;
            txtcmp.Text = "";
            lblMedico.Content = "";
            txtcertificado.Text = "";
            cboestado.SelectedIndex = -1;
            LstEstudios = new List<Search_Estudio>();
            lstAdjuntos = new List<Adjuntos_Desktop>();
            listarEstudios(null);
            radTab_carta.SelectedIndex = 0;
            rabtabdato_adicionales.SelectedIndex = 0;

            cboModalidad.SelectedIndex = 0;
            txtidAseguradora.Text = "";
            lblAseguradora.Content = "";
            lblrucAseguradora.Content = "";
            txtcarnet.Text = "";
            txtcontratante.Text = "";
            lblidClinica.Content = "";
            lblclinica.Content = "";
            lblidClinicaNegicio.Content = "";
            lblclinicaNegicio.Content = "";
            txtcodigoproducto.Text = "";
            lblProducto.Content = "";
            lblidProducto.Content = "";
            lblidBeneficio.Content = "";
            lblBeneficio.Content = "";
            txtidcie.Text = "";
            lblcie.Content = "";
            txtcomentarios.Text = "";
            txtnewcomentarios.Text = "";
            grid_adjuntos.ItemsSource = null;
            txtTotal.Text = "";
            txtcobertura.Value = 0;
            chkSedacion.IsChecked = false;

            lstbAdjunto.ItemsSource = null;
            lstbAdjunto.ItemsSource = lsttipoadjunto;

            stckCartaRevisada.Visibility = Visibility.Collapsed;
            stckCartaRevisadaComeentarios.Visibility = Visibility.Collapsed;
            btnSiteds.Visibility = Visibility.Collapsed;
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            CARTAGARANTIA c;
            List<ESTUDIO_CARTAGAR> est;
            List<string> adj;
            List<ADJUNTO_CARTAGARANTIA> tadj;
            if (isValidFormulario(out c, out est, out adj, out tadj))
            {
                try
                {
                    Excel.Application app = new Excel.Application();
                    app.Visible = true;
                    app.WindowState = Excel.XlWindowState.xlNormal;

                    Excel.Workbook wb = app.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                    Excel.Worksheet ws = wb.Worksheets[1];
                    DateTime currentDate = DateTime.Now;
                    Excel.Range oRange = (Excel.Range)ws.Cells[1, 1];
                    ws.Range["A1", "E1"].MergeCells = true;
                    string img = @"D:\Resocentro\SistemaResocentro\SistemaResocentro\Resocentro_Desktop\img\Logo_Azul.png";
                    ws.Shapes.AddPicture(img, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 30, 0, 250, 70);
                    oRange.RowHeight = 75;

                    //                    ws.Cells[1, 1] = "RESOCENTRO";
                    ws.Cells[2, 1] = "FECHA DE RECEPCIÓN:";
                    ws.Cells[2, 2] = currentDate.ToString("dd/MM/yyyy   hh:mm tt");
                    ws.Cells[3, 1] = "Número Carta:";
                    ws.Cells[3, 2] = txtnumCarta.Text;
                    ws.Cells[4, 1] = "PACIENTE:";
                    ws.Cells[4, 2] = this.txtnombrepaciente.Text.ToString();
                    ws.Cells[5, 1] = "TITULAR:";
                    ws.Cells[5, 2] = this.txttitular.Text;
                    ws.Cells[6, 1] = "COMPAÑIA DE SEGUROS:";
                    ws.Cells[6, 2] = this.lblAseguradora.Content.ToString().ToUpper();
                    ws.Cells[7, 1] = "No. DE POLIZA/CREDENCIAL:";
                    ws.Cells[7, 2] = this.txtpoliza.Text;
                    ws.Cells[8, 1] = "EMPRESA CONTRATANTE:";
                    ws.Cells[8, 2] = this.txtcontratante.Text;
                    ws.Cells[9, 1] = "TELEFONOS:";
                    ws.Cells[9, 2] = "";
                    ws.Cells[10, 1] = "CLINICA DE PROCEDENCIA:";
                    ws.Cells[10, 2] = this.lblclinica.Content.ToString();
                    ws.Cells[11, 1] = "MEDICO TRATANTE:";
                    ws.Cells[11, 2] = this.lblMedico.Content.ToString().ToUpper();
                    ws.Cells[12, 1] = "OBSERVACIONES:";
                    ws.Cells[12, 2] = this.txtcomentarios.Text;
                    ws.Cells[13, 1] = "TIEMPO DE ENFERMEDAD:";
                    ws.Cells[13, 2] = "";

                    ws.Cells[14, 1] = "CONTRASTE:";
                    ws.Cells[14, 3] = "0.0";
                    ws.Cells[15, 1] = "SEDACION:";
                    ws.Cells[15, 3] = "0.0";

                    ws.Cells[18, 1] = "EXAMENES SOLICITADOS";
                    ws.Cells[19, 2] = "Nombre Estudio";
                    ws.Cells[19, 3] = "Moneda";
                    ws.Cells[19, 4] = "Precio";
                    ws.Cells[19, 5] = "Cobertura";

                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 3d;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 3d;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 3].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 3d;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 4].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = 3d;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[19, 5].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;
                    int fila = 20;
                    foreach (var item in LstEstudios)
                    {
                        ws.Cells[fila, 2] = item.estudio;
                        ws.Cells[fila, 3] = item.moneda;
                        ws.Cells[fila, 4] = item.precio;
                        ws.Cells[fila, 5] = item.cobertura + " %";

                        ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                        ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

                        ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

                        ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;

                        ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                        ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        fila++;
                    }
                    fila -= 1;
                    ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = 3d;
                    ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 3].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 4].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;

                    ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = 3d;
                    ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    ws.Cells[fila, 5].Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = 3d;
                    fila++;
                    ws.Cells[fila, 1] = "TOTAL:";
                    ws.Cells[fila, 4] = txtTotal.Text;

                    app.Columns.AutoFit();

                    ws.Range["A2", "A40"].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    ws.Range["A2", "A40"].Font.Bold = true;
                    ws.Range["A4", "B4"].Font.Bold = true;
                    ws.Range["A1", "C1"].Font.Bold = true;
                    ws.Range["A1", "C1"].Font.Size = 36;
                    ws.Range["A1", "C1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }
                catch (Exception)
                {
                    MessageBox.Show("Ocurrio un error al exportar, intente nuevamente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            int idpaciente = 0;
            if (txtidCarta.Text != "" && int.TryParse(lblidpaciente.Content.ToString(), out idpaciente))
            {
                if (MessageBox.Show("¿Desea eliminar permanentemente la Carta de Garantía?", "Pregunta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        new CartaDAO().deleteCarta(txtidCarta.Text, idpaciente);
                        new CartaDAO().insertLog(txtidCarta.Text, this.session.shortuser, (int)Tipo_Log.Delete, "Se Elimino la Carta N° " + txtidCarta.Text);
                        MessageBox.Show("Se elimino la Carta de Garantía", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        limpiarFormulario();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                   
                }
            }
            else
                MessageBox.Show("Debe seleccionar una carta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnguardar_Click(object sender, RoutedEventArgs e)
        {
            CARTAGARANTIA carta;
            List<ESTUDIO_CARTAGAR> estudios;
            List<string> pathFile;
            List<ADJUNTO_CARTAGARANTIA> adjuntos;
            if (isValidFormulario(out carta, out estudios, out pathFile, out adjuntos))
            {
                try
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                       
                        carta.codigodocadjunto = "";
                        //verificamos si existe la carta
                        if (carta.codigocartagarantia == "")
                        {
                            //nuevo registro
                            carta.codigocartagarantia = new CartaDAO().getNewCodigoCarta();
                            carta.obs_revision = "";
                            db.CARTAGARANTIA.Add(carta);
                            while (db.CARTAGARANTIA.Where(x => x.codigocartagarantia == carta.codigocartagarantia).ToList().Count > 0)
                            {
                                carta.codigocartagarantia = new CartaDAO().getNewCodigoCarta();
                            }

                            if (carta.numero_proforma != null)
                            {
                                var _prof = db.PROFORMA.SingleOrDefault(x => x.numerodeproforma == carta.numero_proforma);
                                if (_prof != null)
                                {
                                    _prof.estado = "TRAMITADA";
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la Carta N° " + carta.codigocartagarantia.ToString());
                                }
                            }
                        }
                        else
                        {
                            var _carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == carta.codigocartagarantia && x.codigopaciente == carta.codigopaciente);
                            if (_carta != null)
                            {
                                if (_carta.seguimiento != carta.seguimiento)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se agregó comentarios de la Carta N° " + carta.codigocartagarantia.ToString() + " " + txtnewcomentarios.Text);
                                _carta.seguimiento = carta.seguimiento;

                                if (_carta.estadocarta != carta.estadocarta)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico estado de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.estadocarta);
                                _carta.estadocarta = carta.estadocarta;

                                if (_carta.contratante != carta.contratante)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico contratante de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.contratante);
                                _carta.contratante = carta.contratante;

                                if (_carta.titular != carta.titular)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico titular de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.titular);
                                _carta.titular = carta.titular;

                                if (_carta.poliza != carta.poliza)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico poliza de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.poliza);
                                _carta.poliza = carta.poliza;

                                if (_carta.cmp != carta.cmp)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico Médico de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.cmp);
                                _carta.cmp = carta.cmp;

                                if (_carta.codigocompaniaseguro != carta.codigocompaniaseguro)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico Aseguradora de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.codigocompaniaseguro);
                                _carta.codigocompaniaseguro = carta.codigocompaniaseguro;
                                _carta.ruc = carta.ruc;
                                _carta.codigopaciente = carta.codigopaciente;

                                if (_carta.cobertura != carta.cobertura)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico cobertura de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.cobertura);
                                _carta.cobertura = carta.cobertura;
                                _carta.monto = carta.monto;

                                if (_carta.codigocarta_coaseguro != carta.codigocarta_coaseguro)
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se modifico codigocarta_coaseguro de la Carta N° " + carta.codigocartagarantia.ToString() + " " + carta.codigocarta_coaseguro);
                                _carta.codigocarta_coaseguro = carta.codigocarta_coaseguro;

                                if (_carta.estadocarta == "APROBADA")
                                {
                                    _carta.fechaprobacion = Tool.getDatetime();
                                    new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se aprobó la Carta N° " + carta.codigocartagarantia.ToString());
                                }
                                _carta.numerocarnetseguro = carta.numerocarnetseguro;
                                _carta.cie = carta.cie;
                                _carta.codigocartagarantia2 = carta.codigocartagarantia2;
                                _carta.beneficio = carta.beneficio;
                                _carta.certificado = carta.certificado;
                                _carta.Sunasa_CoberturaId = carta.Sunasa_CoberturaId;
                                _carta.SitedCodigoProducto = carta.SitedCodigoProducto;
                                _carta.codigoclinica = carta.codigoclinica;
                                _carta.codigoclinica2 = carta.codigoclinica2;
                                _carta.TipoAfiliacion = carta.TipoAfiliacion;
                                _carta.user_update = session.codigousuario;
                                _carta.fec_update = Tool.getDatetime();
                                _carta.codigounidad = carta.codigounidad;
                                _carta.codigosucursal = carta.codigosucursal;
                                _carta.ishospitalizado = carta.ishospitalizado;
                                _carta.sedacion_carta = carta.sedacion_carta;

                                new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se actualizó la Carta N° " + carta.codigocartagarantia.ToString());
                                carta.codigodocadjunto = _carta.codigodocadjunto;
                            }
                        }
                        db.SaveChanges();
                        //Actualizamos los estudios
                        var lst = db.ESTUDIO_CARTAGAR.Where(x => x.codigocartagarantia == carta.codigocartagarantia && x.codigopaciente == carta.codigopaciente).ToList();
                        foreach (var item in lst)
                        {
                            db.ESTUDIO_CARTAGAR.Remove(item);
                        }
                        foreach (var dt in estudios)
                        {
                            dt.codigocartagarantia = carta.codigocartagarantia;
                            db.ESTUDIO_CARTAGAR.Add(dt);
                        }

                        //Actualizamos los tipo de Adjuntos
                        db.ADJUNTO_CARTAGARANTIA.RemoveRange(db.ADJUNTO_CARTAGARANTIA.Where(x => x.idCarta == carta.codigocartagarantia && x.idPaciente == carta.codigopaciente).ToList());
                        foreach (var dt in adjuntos)
                        {
                            dt.idCarta = carta.codigocartagarantia;
                            dt.idPaciente = carta.codigopaciente;
                            db.ADJUNTO_CARTAGARANTIA.Add(dt);
                        }

                        DOCESCANEADO doc;
                        //Juntamos todos los archivos en uno solo
                        if (pathFile.Count > 0)
                        {
                            string fileNameCombinate = System.IO.Path.GetTempPath() + DateTime.Now.ToString("ddMMyyyyHHmmss") + "Temporal.pdf";
                            if (new Tool().CombinarPDF(fileNameCombinate, pathFile.ToArray()))
                            {
                                if (carta.codigodocadjunto == "")
                                {
                                    carta = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == carta.codigocartagarantia && x.codigopaciente == carta.codigopaciente);
                                    doc = new DOCESCANEADO();
                                    var nombre = carta.codigocartagarantia.ToString() + "-" + DateTime.Now.ToString("HHmmssddMMyy");
                                    doc.codigodocadjunto = nombre.Length > 20 ? nombre.Substring(0, 19) : nombre;
                                    if (txtnombrepaciente.Text != "")
                                        doc.nombrearchivo = txtnombrepaciente.Text.Trim() + ".pdf";
                                    else
                                        doc.nombrearchivo = "Paciente.pdf";
                                    //doc.cuerpoarchivo = archivo;
                                    doc.cuerpoarchivo = null;
                                    doc.fecharegistro = DateTime.Now;
                                    doc.codigousuario = session.codigousuario;
                                    doc.isFisico = true;
                                    carta.codigodocadjunto = doc.codigodocadjunto;
                                    db.DOCESCANEADO.Add(doc);

                                }
                                else
                                {
                                    doc = db.DOCESCANEADO.SingleOrDefault(x => x.codigodocadjunto == carta.codigodocadjunto);
                                    if (doc != null)
                                    {
                                        doc.cuerpoarchivo = null;
                                        doc.isFisico = true;
                                        carta.codigodocadjunto = doc.codigodocadjunto;

                                    }
                                }

                                //copiamos el archivo 
                                var path = Tool.PathDocumentosAdjuntos + doc.codigodocadjunto;
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);

                                File.Copy(fileNameCombinate, path + @"\" + doc.nombrearchivo, true);
                                File.Delete(fileNameCombinate);

                            }
                            new CartaDAO().insertLog(carta.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Update, "Se actualizó los adjuntos de la Carta N° " + carta.codigocartagarantia.ToString());
                        }
                        db.SaveChanges();
                        MessageBox.Show("Los cambios se registraron exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (isClose)
                            this.Close();
                        limpiarFormulario();
                        btnBuscarPaciente.Focus();


                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            MessageBox.Show("- Property: \"" + ve.PropertyName + "\", Error: \"" + ve.ErrorMessage + "\"");
                        }
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "ERROR");
                }


            }
        }

        private void cleanError()
        {
            var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFABADB3"));
            var borde = new Thickness(1.0);
            txtnombrepaciente.BorderBrush = brush;
            txtnombrepaciente.BorderThickness = borde;
            cboestado.BorderBrush = brush;
            cboestado.BorderThickness = borde;
            txtnumCarta.BorderBrush = brush;
            txtnumCarta.BorderThickness = borde;
            btnClinica.BorderBrush = brush;
            btnClinica.BorderThickness = borde;
            btnClinicaNegocio.BorderBrush = brush;
            btnClinicaNegocio.BorderThickness = borde;
            cboafiliacion.BorderBrush = brush;
            cboafiliacion.BorderThickness = borde;
            txtcmp.BorderBrush = brush;
            txtcmp.BorderThickness = borde;
            gridEstudios.BorderBrush = brush;
            gridEstudios.BorderThickness = borde;
            txtidAseguradora.BorderBrush = brush;
            txtidAseguradora.BorderThickness = borde;
            txtcodigoproducto.BorderBrush = brush;
            txtcodigoproducto.BorderThickness = borde;
            btnBeneficio.BorderBrush = brush;
            btnBeneficio.BorderThickness = borde;
            btnCie.BorderBrush = brush;
            btnCie.BorderThickness = borde;
            grid_adjuntos.BorderBrush = brush;
            grid_adjuntos.BorderThickness = borde;
        }
        private bool isValidFormulario(out CARTAGARANTIA carta, out List<ESTUDIO_CARTAGAR> estudios, out List<string> pathFile, out List<ADJUNTO_CARTAGARANTIA> adjuntos)
        {
            carta = new CARTAGARANTIA();
            estudios = new List<ESTUDIO_CARTAGAR>();
            pathFile = new List<string>();
            adjuntos = new List<ADJUNTO_CARTAGARANTIA>();
            try
            {

                cleanError();
                string msj = "";
                int numero = 0;

                carta.seguimiento = txtcomentarios.Text.Replace("\n", " ").Trim() + "\t" + txtnewcomentarios.Text.Trim(); ;
                carta.fechatramite = Tool.getDatetime();
                carta.contratante = txtcontratante.Text;
                carta.titular = txttitular.Text;
                carta.poliza = txtpoliza.Text;
                carta.numerocarnetseguro = txtcarnet.Text;
                carta.codigocartagarantia = txtidCarta.Text;
                carta.cobertura = float.Parse(txtcobertura.Value.Value.ToString());
                carta.fechaprobacion = carta.fechatramite;
                carta.codigousuario = session.codigousuario;
                carta.codigodocadjunto = "";
                carta.beneficio = 0;
                carta.ishospitalizado = chkhospitalizado.IsChecked.Value;
                carta.sedacion_carta = chkSedacion.IsChecked.Value;
                carta.certificado = txtcertificado.Text.ToUpper().Trim();
                carta.codigounidad = int.Parse(cbosucursal.SelectedValue.ToString().Substring(0, 1));
                carta.codigosucursal = int.Parse(cbosucursal.SelectedValue.ToString().Substring(1, 2));
                carta.isRevisada = false;
                carta.codigocarta_coaseguro = txtcartacoaseguro.Text;

                var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF10707"));
                var borde = new Thickness(2.0);
                if (int.TryParse(lblnumeroProforma.Content.ToString(), out numero))
                    carta.numero_proforma = int.Parse(lblnumeroProforma.Content.ToString());

                if (!int.TryParse(lblidpaciente.Content.ToString(), out numero))
                {
                    msj += "- Seleccione un Paciente.\n";
                    txtnombrepaciente.BorderBrush = brush;
                    txtnombrepaciente.BorderThickness = borde;
                    txtnombrepaciente.Focus();
                }
                else
                {
                    carta.codigopaciente = int.Parse(lblidpaciente.Content.ToString());
                }
                if (cboestado.Text == null || cboestado.Text == "")
                {
                    msj += "- Seleccione el Estado de la Carta.\n";
                    cboestado.BorderBrush = brush;
                    cboestado.BorderThickness = borde;
                    cboestado.Focus();
                }
                else
                {
                    carta.estadocarta = cboestado.Text;
                }

                if (txtnumCarta.Text.Trim() == "" /*&& (txtidAseguradora.Text.Trim() != "37" || txtidAseguradora.Text.Trim() 
                != "27")*/)
                {
                    msj += "- Ingrese el número de Carta de Garantía.\n";
                    txtnumCarta.BorderBrush = brush;
                    txtnumCarta.BorderThickness = borde;
                    txtnumCarta.Focus();
                }
                else
                {
                    carta.codigocartagarantia2 = txtnumCarta.Text;
                }

                if (!int.TryParse(lblidClinica.Content.ToString(), out numero))
                {
                    msj += "- Seleccione la Procedencia.\n";
                    btnClinica.BorderBrush = brush;
                    btnClinica.BorderThickness = borde;
                    btnClinica.Focus();
                }
                else
                {
                    carta.codigoclinica = int.Parse(lblidClinica.Content.ToString());
                }
                if (!int.TryParse(lblidClinicaNegicio.Content.ToString(), out numero))
                {
                    msj += "- Seleccione la Procedencia de Negocio.\n";
                    btnClinicaNegocio.BorderBrush = brush;
                    btnClinicaNegocio.BorderThickness = borde;
                    btnClinicaNegocio.Focus();
                }
                else
                {
                    carta.codigoclinica2 = int.Parse(lblidClinicaNegicio.Content.ToString());
                }

                if (txtcmp.Text.Trim() == "")
                {
                    msj += "- Ingrese el Médico.\n";
                    txtcmp.BorderBrush = brush;
                    txtcmp.BorderThickness = borde;
                    txtcmp.Focus();
                }
                else
                {
                    carta.cmp = txtcmp.Text;
                }
                if (LstEstudios.Count == 0)
                {
                    msj += "- No hay ningún Estudio seleccionado.\n";
                    gridEstudios.BorderBrush = brush;
                    gridEstudios.BorderThickness = borde;
                    gridEstudios.Focus();
                }
                else
                {
                    carta.monto = float.Parse(txtTotal.Text);
                }
                if (carta.codigocartagarantia != "")
                {
                    if (cboafiliacion.SelectedValue != null)
                        carta.TipoAfiliacion = int.Parse(cboafiliacion.SelectedValue.ToString());

                }
                if (cboestado.Text == "TRAMITADA" && lstAdjuntos.Count == 0)
                {
                    msj += "- Ingrese un documento adjunto para proceder con el tramite.\n";
                }

                if (!int.TryParse(txtidAseguradora.Text.ToString(), out numero))
                {
                    msj += "- Seleccione una Aseguradora.\n";
                    txtidAseguradora.BorderBrush = brush;
                    txtidAseguradora.BorderThickness = borde;
                    txtidAseguradora.Focus();
                }
                else
                {
                    carta.codigocompaniaseguro = int.Parse(txtidAseguradora.Text);
                    carta.ruc = lblrucAseguradora.Content.ToString();

                    if (cboestado.Text != null)
                        if (cboestado.Text == "APROBADA")// || cboestado.Text == "CITADA")
                        {

                            if (new CartaDAO().isProductoActivo(numero))
                            {
                                if (lblidProducto.Content == null || lblidProducto.Content.ToString() == "")
                                {
                                    msj += "- Seleccione el Producto.\n";
                                    txtcodigoproducto.BorderBrush = brush;
                                    txtcodigoproducto.BorderThickness = borde;
                                    txtcodigoproducto.Focus();
                                }
                                else
                                {
                                    carta.SitedCodigoProducto = lblidProducto.Content.ToString();
                                }

                            }

                            if (cboafiliacion.SelectedValue == null)
                            {
                                msj += "- Seleccione el Tipo de Afiliación.\n";
                                cboafiliacion.BorderBrush = brush;
                                cboafiliacion.BorderThickness = borde;
                                cboafiliacion.Focus();
                            }
                            else
                            {
                                carta.TipoAfiliacion = int.Parse(cboafiliacion.SelectedValue.ToString());
                            }

                            if (cbosucursal.SelectedValue.ToString().Substring(0, 1) == "1")
                                if (lblidBeneficio.Content == null || lblidBeneficio.Content.ToString() == "")
                                {
                                    msj += "- Seleccione el Beneficio.\n";
                                    btnBeneficio.BorderBrush = brush;
                                    btnBeneficio.BorderThickness = borde;
                                    btnBeneficio.Focus();
                                }
                                else
                                {
                                    carta.Sunasa_CoberturaId = int.Parse(lblidBeneficio.Content.ToString());
                                }
                            if (cbosucursal.SelectedValue.ToString().Substring(0, 1) == "1")
                                if (txtidcie.Text == null || txtidcie.Text.ToString() == "")
                                {
                                    msj += "- Seleccione el CIE 10.\n";
                                    btnCie.BorderBrush = brush;
                                    btnCie.BorderThickness = borde;
                                    btnCie.Focus();
                                }
                                else
                                {
                                    carta.cie = txtidcie.Text.ToString();
                                }
                            if (LstEstudios.Count > 0)
                            {
                                foreach (var item in LstEstudios)
                                {
                                    if (item.cobertura == 0)
                                        msj += "- El estudio " + item.estudio + " tiene como cobertura cero.\n";
                                }
                            }
                            if (lstAdjuntos.Count == 0)
                            {
                                msj += "- No hay ningún documento adjunto.\n";
                                grid_adjuntos.BorderBrush = brush;
                                grid_adjuntos.BorderThickness = borde;
                                btnBuscarEstudio.Focus();
                            }
                        }
                        else
                        {
                            if (new CartaDAO().isProductoActivo(numero))
                            {
                                if (lblidProducto.Content != null && lblidProducto.Content.ToString() != "")
                                {
                                    carta.SitedCodigoProducto = lblidProducto.Content.ToString();
                                }
                            }
                            if (lblidBeneficio.Content != null && lblidBeneficio.Content.ToString() != "")
                            {
                                carta.Sunasa_CoberturaId = int.Parse(lblidBeneficio.Content.ToString());
                            }
                            if (txtidcie.Text != null && txtidcie.Text.ToString() != "")
                            {

                                carta.cie = txtidcie.Text.ToString();
                            }
                        }
                }

                //set Documentos

                foreach (var item in lstAdjuntos)
                {
                    if (!item.isFisico)//si esta en la BD
                    {
                        item.ruta = System.IO.Path.GetTempPath() + item.nombre;
                        if (!System.IO.File.Exists(item.ruta))
                        {
                            System.IO.FileStream archivo = new System.IO.FileStream(item.ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                            archivo.Write(item.archivo, 0, item.archivo.Length);
                            archivo.Close();
                        }
                    }
                    if (System.IO.File.Exists(item.ruta))
                        pathFile.Add(item.ruta);
                }

                //set estudios
                foreach (var item in LstEstudios)
                {
                    ESTUDIO_CARTAGAR det = new ESTUDIO_CARTAGAR();
                    det.codigoestudio = item.codigoestudio;
                    det.codigocartagarantia = "";
                    det.cmp = carta.cmp;
                    det.codigocompaniaseguro = carta.codigocompaniaseguro;
                    det.ruc = carta.ruc;
                    det.codigopaciente = carta.codigopaciente;
                    det.codigoclase = item.codigoclase;
                    det.moneda = item.idmoneda;
                    det.cobertura_det = float.Parse(item.cobertura.ToString());

                    det.preciobruto = float.Parse(item.precio.ToString());
                    det.descuento = float.Parse(item.descuento.ToString());

                    estudios.Add(det);
                }

                if (lstbAdjunto.SelectedItems != null)
                    foreach (Tipo_Adjunto item in lstbAdjunto.SelectedItems)
                    {
                        adjuntos.Add(new ADJUNTO_CARTAGARANTIA { idAdjunto = item.idTipo });
                    }

                if (cboafiliacion.SelectedValue == null && cboestado.Text == "CITADA")
                {
                    if (MessageBox.Show("Se guardara la carta sin registrar un tipo de Afiliación.\n¿Desea continuar?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return false;
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
            catch (Exception ex)
            {
                return false;
                MessageBox.Show(ex.Message);
            }

        }

        private void btnBeneficio_Click(object sender, RoutedEventArgs e)
        {
            int compania = 0, producto = 0;
            if (int.TryParse(txtidAseguradora.Text, out compania))
            {

                frmSearchBeneficio gui = new frmSearchBeneficio();
                if (new CartaDAO().isProductoActivo(compania))
                {
                    if (int.TryParse(lblidProducto.Content.ToString(), out producto))
                        gui.producto = producto;
                    else
                        MessageBox.Show("Seleccione el Producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                gui.compania = compania;
                gui.Owner = this;
                gui.txtnombre.Focus();
                gui.ShowDialog();
                if (gui.beneficio != null)
                {
                    lblidBeneficio.Content = gui.beneficio.Sunasa_CoberturaId.ToString().Trim();
                    lblBeneficio.Content = gui.beneficio.Nombre.ToString();
                }
            }
        }

        private void btnCie_Click(object sender, RoutedEventArgs e)
        {
            frmSearchCie gui = new frmSearchCie();
            gui.txtnombre.Focus();
            gui.Owner = this;
            gui.ShowDialog();
            if (gui.cie != null)
            {
                txtidcie.Text = gui.cie.codigo.ToString().Trim();
                lblcie.Content = gui.cie.descripcion.ToString();
            }
        }
        private void txtidcie_KeyUp(object sender, KeyEventArgs e)
        {
            lblcie.Content = "";
            if (e.Key == Key.Enter)
                if (txtidcie.Text != "")
                {
                    var _cie = (new CartaDAO()).getCiexCodigo(txtidcie.Text);
                    if (_cie.codigo != null)
                    {
                        txtidcie.Text = _cie.codigo.ToString().Trim();
                        lblcie.Content = _cie.descripcion.ToString();
                        btnCie.Focus();
                    }
                    else
                    {
                        MessageBox.Show("No existe un CIE10 con el codigo Ingresado", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtidcie.Text = "";
                        lblcie.Content = "";
                    }
                }
                else
                {
                    MessageBox.Show("Ingrese un codigo para realizar una busqueda", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtidcie.Text = "";
                    lblcie.Content = "";
                }

        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                gridEstudios.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            Search_Estudio item = (Search_Estudio)this.gridEstudios.SelectedItem;
            if (item == null)
            {
                ContextMenuItemModificarCobertura.Visibility = Visibility.Collapsed;
                ContextMenuItemEliminarEstudio.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContextMenuItemModificarCobertura.Visibility = Visibility.Visible;
                ContextMenuItemEliminarEstudio.Visibility = Visibility.Visible;
            }
        }

        private void MenuItemModificarCobertura_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Search_Estudio item = (Search_Estudio)this.gridEstudios.SelectedItem;
            if (item != null)
            {
                frmCobertura gui = new frmCobertura();
                gui.Owner = this;
                gui.estudio = item;
                gui.iniciarGUI(item);
                gui.ShowDialog();
                listarEstudios(gui.estudio);

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
                    listarEstudios(null);
                }
            }
        }
        private void RadContextMenuAdjunto_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                grid_adjuntos.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            Adjuntos_Desktop item = (Adjuntos_Desktop)this.grid_adjuntos.SelectedItem;
            if (item == null)
            {
                ContextMenu_AbrirAdj.Visibility = Visibility.Collapsed;
                ContextMenu_DeleteAdj.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContextMenu_AbrirAdj.Visibility = Visibility.Visible;
                ContextMenu_DeleteAdj.Visibility = Visibility.Visible;
            }
        }
        private void grid_adjuntos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Adjuntos_Desktop item = (Adjuntos_Desktop)e.Row.DataContext;
            abrirarchivo(item);
        }

        private void abrirarchivo(Adjuntos_Desktop item)
        {
            if (item != null)
            {
                try
                {
                    if (!item.isFisico)
                    {
                        item.ruta = System.IO.Path.GetTempPath() + item.nombre;
                        if (System.IO.File.Exists(item.ruta))
                        {
                            System.IO.File.Delete(item.ruta);
                        }
                        System.IO.FileStream archivo = new System.IO.FileStream(item.ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                        archivo.Write(item.archivo, 0, item.archivo.Length);
                        archivo.Close();
                        System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                        d.FileName = item.ruta;
                        //d.Verb = "pdf";
                        System.Diagnostics.Process.Start(d);
                    }
                    else
                    {
                        if (File.Exists(item.ruta))
                        {
                            string ruta = System.IO.Path.GetTempPath() + item.nombre;
                            try
                            {
                                if (File.Exists(ruta))
                                    File.Delete(ruta);
                                File.Copy(item.ruta, ruta);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                            }
                            System.Diagnostics.ProcessStartInfo d = new System.Diagnostics.ProcessStartInfo();
                            d.FileName = ruta;
                            //d.Verb = "pdf";
                            System.Diagnostics.Process.Start(d);
                        }
                        else
                            MessageBox.Show("El archivo esta dañado o no existe.\n" + item.ruta.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        private void MenuItemAbrir_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Desktop item = (Adjuntos_Desktop)this.grid_adjuntos.SelectedItem;
            abrirarchivo(item);
        }

        private void MenuItemAgregar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Carta|*.PDF";
            ofd.Title = "Seleccione los archivos";
            ofd.Multiselect = true;

            if (ofd.ShowDialog().Value)
            {
                foreach (var file in ofd.FileNames)
                {
                    var _f = new FileInfo(file);
                    Adjuntos_Desktop item = new Adjuntos_Desktop();
                    item.ruta = file;
                    item.nombre = _f.Name;
                    item.size = _f.Length / 1024;
                    item.isFisico = true;
                    lstAdjuntos.Add(item);
                    listarAdjuntos();
                }
            }
        }

        private void MenuItemEliminar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Desktop item = (Adjuntos_Desktop)this.grid_adjuntos.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea elimiar el archivo permamentemente?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    lstAdjuntos.Remove(item);
                    listarAdjuntos();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public void setCartaGarantia(CARTAGARANTIA item, List<ESTUDIO_CARTAGAR> detalle, bool openFile)
        {
            limpiarFormulario();
            if (item != null)
            {
                var _paciente = new PacienteDAO().getPaciente(item.codigopaciente);
                var _medico = new UtilDAO().getMedicoxCodigo(item.cmp);
                var _aseguradora = new UtilDAO().getAseguradoraxCodigo(item.codigocompaniaseguro.ToString());
                cbosucursal.SelectedIndex = 0;
                cboModalidad.SelectedIndex = 0;
                txtnombrepaciente.Text = _paciente.apellidos + ", " + _paciente.nombres;
                txtnombre_paciente.Text = _paciente.nombres.ToUpper().Trim();
                txtapellidos_paciente.Text = _paciente.apellidos.ToUpper().Trim();
                dtpNacimiento_paciente.SelectedDate = _paciente.fechanace;
                cbotipo_doc_paciente.SelectedValue = _paciente.tipo_doc;
                txtnum_doc_Paciente.Text = _paciente.dni;
                txttelefono.Text = _paciente.telefono;
                txtcelular.Text = _paciente.celular;

                lblidpaciente.Content = _paciente.codigopaciente;
                buscarCarta(_paciente.codigopaciente);
                if (item.codigounidad != null && item.codigosucursal != null)
                {
                    cbosucursal.SelectedValue = ((item.codigounidad * 100) + item.codigosucursal).ToString();
                    getModalidad(item.codigounidad.ToString());
                }
                txtidCarta.Text = item.codigocartagarantia;
                txtnumCarta.Text = item.codigocartagarantia2;
                txttitular.Text = item.titular;
                txtcobertura.Value = item.cobertura;
                txtpoliza.Text = item.poliza;
                if (item.TipoAfiliacion != null)
                    cboafiliacion.SelectedValue = item.TipoAfiliacion.ToString();
                txtcmp.Text = item.cmp;
                txtcertificado.Text = item.certificado;
                lblMedico.Content = _medico.apellidos.ToUpper() + ", " + _medico.nombres.ToUpper();
                if (item.estadocarta != "INICIADA")
                    cboestado.Text = item.estadocarta;

                if (item.TipoAfiliacion != null)
                    cboafiliacion.SelectedValue = item.TipoAfiliacion.ToString();

                txtidAseguradora.Text = item.codigocompaniaseguro.ToString();
                lblAseguradora.Content = _aseguradora.descripcion.ToUpper();
                lblrucAseguradora.Content = _aseguradora.ruc;
                btnBuscarEstudio.IsEnabled = true;
                btnBuscarInsumos.IsEnabled = true;
                getProductos_Beneficio();
                chkSedacion.IsChecked = item.sedacion_carta;
                chkhospitalizado.IsChecked = item.ishospitalizado;
                txtcarnet.Text = item.numerocarnetseguro;
                btnSiteds.Visibility = Visibility.Visible;
                txtcontratante.Text = item.contratante;
                if (item.codigocarta_coaseguro != null)
                    txtcartacoaseguro.Text = item.codigocarta_coaseguro;
                if (item.isRevisada != null)
                    if (item.isRevisada.Value)
                    {
                        if (item.obs_revision == "")
                        {
                            stckCartaRevisada.Visibility = Visibility.Visible;
                            stckCartaRevisadaComeentarios.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            stckCartaRevisada.Visibility = Visibility.Collapsed;
                            stckCartaRevisadaComeentarios.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        stckCartaRevisada.Visibility = Visibility.Collapsed;
                        stckCartaRevisadaComeentarios.Visibility = Visibility.Collapsed;
                    }
                else
                {
                    stckCartaRevisada.Visibility = Visibility.Collapsed;
                    stckCartaRevisadaComeentarios.Visibility = Visibility.Collapsed;
                }

                if (item.codigoclinica != null)
                {
                    var _clinica = new UtilDAO().getClinicaxCodigo(item.codigoclinica.Value);
                    lblidClinica.Content = _clinica.codigoclinica;
                    lblclinica.Content = _clinica.razonsocial;
                }
                if (item.codigoclinica2 != null)
                {
                    var _clinicaNegocio = new UtilDAO().getClinicaxCodigo(item.codigoclinica2.Value);
                    lblidClinicaNegicio.Content = _clinicaNegocio.codigoclinica;
                    lblclinicaNegicio.Content = _clinicaNegocio.razonsocial;
                }
                if (item.SitedCodigoProducto != null)
                {
                    if (item.SitedCodigoProducto != "")
                    {
                        int p = 0;
                        if (int.TryParse(item.SitedCodigoProducto, out p))
                        {
                            var _pro = new CartaDAO().getProductoxcodigo(p, item.codigocompaniaseguro);
                            if (_pro != null)
                            {
                                txtcodigoproducto.Text = _pro.SitedCodigoProducto;
                                lblProducto.Content = _pro.descripcion;
                                lblidProducto.Content = _pro.SitedProductoId.ToString();
                            }
                        }
                    }
                }
                if (item.Sunasa_CoberturaId != null)
                {
                    if (item.Sunasa_CoberturaId > 0)
                    {
                        var _beneficio = new CartaDAO().getBeneficioxcodigo(item.Sunasa_CoberturaId.Value);
                        lblidBeneficio.Content = _beneficio.Sunasa_CoberturaId;
                        lblBeneficio.Content = _beneficio.Nombre;
                    }
                }
                if (item.cie != null)
                {
                    if (item.cie != "")
                    {
                        var _cie = new CartaDAO().getCiexCodigo(item.cie);
                        txtidcie.Text = _cie.codigo;
                        lblcie.Content = _cie.descripcion;
                    }
                }
                txtcomentarios.Text = item.seguimiento;
                if (item.codigodocadjunto != null)
                {
                    if (item.codigodocadjunto != "")
                    {
                        var _doc = new CartaDAO().getDocescaneado(item.codigodocadjunto);
                        if (_doc != null)
                            lstAdjuntos.Add(new Adjuntos_Desktop()
                            {
                                isFisico = _doc.isFisico,
                                ruta = Tool.PathDocumentosAdjuntos + _doc.codigodocadjunto + @"\" + _doc.nombrearchivo,
                                nombre = _doc.nombrearchivo,
                                archivo = _doc.cuerpoarchivo,
                                size = _doc.cuerpoarchivo != null ? _doc.cuerpoarchivo.Length / 1024 : 0
                            });
                        listarAdjuntos();
                    }
                }
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (var adj in db.ADJUNTO_CARTAGARANTIA.Where(x => x.idCarta == item.codigocartagarantia && x.idPaciente == item.codigopaciente).Select(x => x.idAdjunto).ToList())
                    {
                        lstbAdjunto.SelectedItems.Add(lsttipoadjunto.SingleOrDefault(x => x.idTipo == adj));
                    }
                }


                if (item.numero_proforma != null)
                    lblnumeroProforma.Content = item.numero_proforma.ToString();

                if (detalle.Count > 0)
                {
                    var _est = detalle.FirstOrDefault();
                    cbosucursal.SelectedValue = _est.codigoestudio.Substring(0, 3);
                    cboModalidad.SelectedValue = (int.Parse(_est.codigoestudio.Substring(3, 2))).ToString();
                    foreach (var det in detalle)
                    {
                        var est = new EstudiosDAO().getEstudio(det.codigoestudio);
                        var detalle_insert = new EstudiosDAO().getPrecioEstudio(det.codigoestudio, item.codigocompaniaseguro);
                        Search_Estudio _estudio = new Search_Estudio();
                        if (est != null && detalle_insert != null)
                        {
                            item.monto += float.Parse(detalle_insert.precio.ToString());
                            _estudio.codigoestudio = det.codigoestudio;
                            _estudio.estudio = est.nombreestudio;
                            _estudio.codigoclase = det.codigoclase;
                            //precio
                            if (det.preciobruto == null)
                                _estudio.precio = detalle_insert.precio;
                            else
                                _estudio.precio = det.preciobruto.Value;
                            //cobertura
                            if (det.cobertura_det == null)
                                _estudio.cobertura = item.cobertura;
                            else
                                _estudio.cobertura = det.cobertura_det.Value;
                            //moneda
                            if (det.moneda == null)
                                _estudio.idmoneda = detalle_insert.idmoneda;
                            else
                                _estudio.idmoneda = det.moneda.Value;
                            //descuento
                            if (det.descuento == null)
                                _estudio.descuento = ((_estudio.cobertura * _estudio.precio) / 100) * -1.0;
                            else
                                _estudio.descuento = det.descuento.Value;

                            listarEstudios(_estudio);

                        }
                        else
                        {
                            MessageBox.Show("El estudio \"" + est.nombreestudio + "\" no tiene precio asignado con la compañia seleccionada", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }

                }

            }

            if (openFile)
            {

                foreach (Adjuntos_Desktop doc in lstAdjuntos)
                {
                    abrirarchivo(doc);
                }
            }

        }

        private void listarAdjuntos()
        {
            grid_adjuntos.ItemsSource = null;
            grid_adjuntos.ItemsSource = lstAdjuntos;
        }

        private void grid_carta_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            List_Carta item = (List_Carta)e.Row.DataContext;
            var _cart = new CartaDAO().getCartaxCodigo(item.codigocartagarantia, item.codigopaciente);
            var detalle = new CartaDAO().getDetalleCartaxCodigo(item.codigocartagarantia, item.codigopaciente);
            if (_cart != null)
            {
                new CartaDAO().insertLog(item.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + item.codigocartagarantia.ToString());
                setCartaGarantia(_cart, detalle, false);
                radTab_carta.SelectedIndex = 0;
            }
        }

        private void txtnombrepaciente_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            int idpaciente = 0;
            if (int.TryParse(lblidpaciente.Content.ToString(), out idpaciente))
                ContextMenuPaciente.Visibility = Visibility.Visible;
            else
                ContextMenuPaciente.Visibility = Visibility.Collapsed;
        }
        private void InfoPaciente_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            int idpaciente = 0;
            if (int.TryParse(lblidpaciente.Content.ToString(), out idpaciente))
            {
                frmPaciente gui = new frmPaciente();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    gui.setPaciente(db.PACIENTE.SingleOrDefault(x => x.codigopaciente == idpaciente));
                    gui.ShowDialog();
                }

            }
        }


        private void ContextMenuCartaCoaseguro_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                var lista = (List<List_Carta>)this.grid_carta.Items.SourceCollection;
                if (lista != null)
                {
                    ObservableCollection<RadMenuItem> items = new ObservableCollection<RadMenuItem>();
                    RadMenuItem itemNinguno = new RadMenuItem() { Header = "Ninguno" };
                    items.Add(itemNinguno);
                    foreach (var item in lista)
                    {
                        RadMenuItem newItem = new RadMenuItem() { Header = item.codigocartagarantia };
                        items.Add(newItem);
                    }
                    ContextMenuCartaCoaseguro.ItemsSource = null;
                    ContextMenuCartaCoaseguro.ItemsSource = items;
                }
            }
            catch (Exception ex) { }
        }

        private void ContextMenuCartaCoaseguro_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadMenuItem item = e.OriginalSource as RadMenuItem;
            if (item.Header.ToString() == "Ninguno")
                txtcartacoaseguro.Text = "";
            else
                txtcartacoaseguro.Text = item.Header.ToString();
        }

        private void txtcobertura_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (LstEstudios != null)
            {
                foreach (var item in LstEstudios)
                {
                    //if (item.cobertura == 0)
                    //{
                    item.cobertura = (e.NewValue.Value) * 1.0;
                    item.descuento = ((e.NewValue.Value * item.precio) / 100) * -1.0;
                    //}
                }
                gridEstudios.ItemsSource = null;
                gridEstudios.ItemsSource = LstEstudios.ToList();
            }
        }

        private void btnSiteds_Click(object sender, RoutedEventArgs e)
        {
            frmListaSiteds gui = new frmListaSiteds();
            gui.cargarGUI();
            gui.ShowDialog();
            if (gui.sitedresult != null)
                if (gui.sitedresult.asegurado != null)
                {
                    txttitular.Text = gui.sitedresult.titular;
                    txtcarnet.Text = gui.sitedresult.carnet;
                    txtcontratante.Text = gui.sitedresult.contrato;
                    txtpoliza.Text = gui.sitedresult.poliza;
                    txtcertificado.Text = gui.sitedresult.certificado;
                    if (gui.sitedresult.idafiliacion != null)
                    {
                        cboafiliacion.SelectedValue = gui.sitedresult.idafiliacion.ToString();
                    }
                    if (gui.sitedresult.producto != "" && gui.sitedresult.producto != null)
                    {
                        txtcodigoproducto.Text = gui.sitedresult.producto;

                        lblProducto.Content = "";
                        lblidProducto.Content = "";
                        int compania = 0;
                        if (int.TryParse(txtidAseguradora.Text, out compania))
                        {
                                buscarProductoCodigoSiteds(compania, txtcodigoproducto.Text);
                        }
                        else
                        {
                            MessageBox.Show("Seleccione la Aseguradora", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            txtcodigoproducto.Text = "";
                            lblProducto.Content = "";
                            lblidProducto.Content = "";
                        }


                    }

                }

        }

        private void ContextMenuCartaDetalle_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadContextMenu menu = (RadContextMenu)sender;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                GridViewRow cellRow = cell.ParentRow as GridViewRow;
                grid_carta.SelectedItem = null;
                if (cellRow != null)
                {
                    cellRow.IsSelected = true;
                    cellRow.IsCurrent = true;
                }
            }
            catch (Exception ex)
            {

            }
            List_Carta item = (List_Carta)this.grid_carta.SelectedItem;
            if (item == null)
                ContextMenuCartaDetalle.Visibility = Visibility.Collapsed;
            else
                ContextMenuCartaDetalle.Visibility = Visibility.Visible;
        }

        private void ContextMenuCartaDetalle_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (List_Carta)this.grid_carta.SelectedItem;
            if (item != null)
            {
                frmLogCarta gui = new frmLogCarta();
                gui.cargarGUI(item.codigocartagarantia);
                gui.Owner = this;
                gui.ShowDialog();
            }

        }

        private void gridEstudios_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            Search_Estudio itm = (Search_Estudio)e.EditedItem;
            var _item = LstEstudios.SingleOrDefault(x => x.codigoestudio == itm.codigoestudio);
            if (_item != null)
            {
                _item = itm;
                listarEstudios(null);
            }
        }

        private void btnPromocion_Click(object sender, RoutedEventArgs e)
        {
            int compania = 0;
            if (int.TryParse(txtidAseguradora.Text, out compania))
                if (LstEstudios.Count > 0 && compania > 0)
                {
                    LstEstudios = new CartaDAO().calularPrecio(LstEstudios, compania);

                }
            calcularPromocion();
            listarEstudios(null);
        }

        private void calcularPromocion()
        {
            int cantTomografia = 0;
            int[] companiasTEM = new Tool().CompaniasNOAfectasTEM;
            int compania = 0;
            if (int.TryParse(txtidAseguradora.Text, out compania))
                if (!companiasTEM.Contains(compania))
                {
                    cantTomografia = LstEstudios.Where(x =>
                        x.codigoestudio.Substring(3, 2) == "02" &&//TEM
                        x.codigoestudio.Substring(6, 1) != "9" &&//NO SEA CONTRASTE
                        x.codigoestudio.Substring(7, 2) != "99" &&//NO SEA TECNICA
                        !(x.estudio.ToLower().Replace('á', 'a').Contains("dinamica")) && //NO TEM DINAMICA
                        !(x.estudio.ToLower().Contains("artro"))//NO ARTROS
                        ).ToList().Count();
                    if (cantTomografia > 1)
                    {
                        foreach (var d in LstEstudios)
                        {
                            //Promociones de Tomografias  2 = 20% , 3 >= 30%
                            if (d.codigoestudio.Substring(3, 2) == "02" && // TEM
                                d.codigoestudio.Substring(6, 1) != "9" && //NO SEA CONTRASTE
                                d.codigoestudio.Substring(7, 2) != "99" &&//NO SEA TECNICA
                        !(d.estudio.ToLower().Replace('á', 'a').Contains("dinamica")) && //NO TEM DINAMICA
                        !(d.estudio.ToLower().Contains("artro"))//NO ARTROS
                                )
                            {
                                if (cantTomografia >= 3)
                                    d.precio = Math.Round((d.precio - (d.precio * 0.30)), 2);
                                else if (cantTomografia == 2)
                                    d.precio = Math.Round((d.precio - (d.precio * 0.20)), 2);
                                else
                                    d.precio = d.precio;

                                d.descuento = (d.precio * (d.cobertura / 100)) * -1;
                            }
                        }
                    }
                }

        }

        private void txtSearchcarta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSearchcarta.Text != "")
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        int codigopaciente = new CartaDAO().getPacientexCarta(txtSearchcarta.Text);
                        var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == codigopaciente);
                        if (paciente != null)
                        {
                            txtnombrepaciente.Text = paciente.apellidos.ToUpper().Trim() + ", " + paciente.nombres.ToUpper().Trim();
                            lblidpaciente.Content = codigopaciente;
                            buscarCarta(codigopaciente);
                            var _cart = new CartaDAO().getCartaxCodigo(txtSearchcarta.Text, codigopaciente);
                            var detalle = new CartaDAO().getDetalleCartaxCodigo(txtSearchcarta.Text, codigopaciente);
                            if (_cart != null)
                            {
                                new CartaDAO().insertLog(_cart.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + _cart.codigocartagarantia.ToString());
                                setCartaGarantia(_cart, detalle, false);
                            }
                            txtSearchcarta.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("No se encontro data.");
                        }
                    }
                }
            }
        }


        private void HCPaciente_Click(object sender, RoutedEventArgs e)
        {
            int idpaciente = 0;
            if (int.TryParse(lblidpaciente.Content.ToString(), out idpaciente))
            {
                frmHistoriaPaciente gui = new frmHistoriaPaciente();
                gui.cargarGUI(session);
                gui.Show();
                gui.buscarPaciente(idpaciente.ToString(), 2);
            }
        }

       

        private void updatePaciente()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                #region Paciente
                int codigopaciente = 0;
                if (int.TryParse(lblidpaciente.Content.ToString(),out codigopaciente))
                {
                    var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == codigopaciente);
                    if (paciente != null)
                    {
                        paciente.apellidos = txtapellidos_paciente.Text.ToUpper().Trim();
                        paciente.nombres = txtnombre_paciente.Text.ToUpper().Trim();
                        paciente.fechanace = dtpNacimiento_paciente.SelectedDate.Value;
                        paciente.dni = txtnum_doc_Paciente.Text.ToUpper().Trim();
                        paciente.tipo_doc = cbotipo_doc_paciente.SelectedValue.ToString();
                        paciente.telefono = txttelefono.Text;
                        paciente.celular = txtcelular.Text;
                        db.SaveChanges();
                    }
                }
                #endregion
            }
        }

      

       

        private void txtpacientes_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            updatePaciente();
        }

        private void cbotipo_doc_paciente_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            updatePaciente();
        }

        private void dtpNacimiento_paciente_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            updatePaciente();
        }

    

        





    }


}
