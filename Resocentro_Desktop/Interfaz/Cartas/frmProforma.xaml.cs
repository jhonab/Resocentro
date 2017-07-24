using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.Cartas.Print;
using Resocentro_Desktop.Interfaz.frmUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Excel = Microsoft.Office.Interop.Excel;
namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmProforma.xaml
    /// </summary>
    public partial class frmProforma : Window
    {
        MySession session;
        List<Search_Estudio> LstEstudios;
        List<Adjuntos_Desktop> LstAdjuntos;
        public frmProforma()
        {
            InitializeComponent();
        }
        public void iniciarGUI(MySession session)
        {
            this.session = session;
            LstAdjuntos = new List<Adjuntos_Desktop>();

            //cargamos los combos segun usuario
            cbosucursal.ItemsSource = new UtilDAO().getSucursales(session.sucursales);
            cbosucursal.SelectedValuePath = "codigoInt";
            cbosucursal.DisplayMemberPath = "nombreShort";
            cbosucursal.SelectedIndex = 0;


            txtcontraste.Value = 0;
            txtsedacion.Value = 0;
            //fijamos el cursor
            btnBuscarPaciente.Focus();
        }
        
      
        public void getModalidad(string empresa)
        {
            cboModalidad.ItemsSource = new UtilDAO().getModalidadxEmpresa(empresa.Substring(0, 1));
            cboModalidad.SelectedValuePath = "codigo";
            cboModalidad.DisplayMemberPath = "nombre";
            cboModalidad.SelectedIndex = 0;
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
        private void btnAseguradora_Click(object sender, RoutedEventArgs e)
        {
            frmSearchAseguradora gui = new frmSearchAseguradora();
            gui.ShowDialog();
            if (gui.aseguradora != null)
            {
                lblAseguradora.Content = gui.aseguradora.descripcion;
                txtidAseguradora.Text = gui.aseguradora.codigocompaniaseguro.ToString();
                lblrucAseguradora.Content = gui.aseguradora.ruc;
                btnAseguradora.Focus();
                btnEstudio.IsEnabled = true;
            }
            else
                btnEstudio.IsEnabled = false;

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
                    var _aseguradora = new UtilDAO().getAseguradoraxCodigo(txtidAseguradora.Text);
                    if (_aseguradora != null)
                    {
                        lblAseguradora.Content = _aseguradora.descripcion;
                        txtidAseguradora.Text = _aseguradora.codigocompaniaseguro.ToString();
                        lblrucAseguradora.Content = _aseguradora.ruc;
                        btnAseguradora.Focus();
                        btnEstudio.IsEnabled = true;
                    }
                    else
                        btnEstudio.IsEnabled = false;
                }
                else
                    btnEstudio.IsEnabled = false;

                LstEstudios = new List<Search_Estudio>();
                listarEstudios(null);
            }
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

        private void btnMedico_Click(object sender, RoutedEventArgs e)
        {
            frmSearchMedico gui = new frmSearchMedico();
            gui.ShowDialog();
            if (gui.medico != null)
            {
                lblMedico.Content = gui.medico.apellidos + ", " + gui.medico.nombres;
                txtcmp.Text = gui.medico.cmp;
                btnMedico.Focus();
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
                        btnMedico.Focus();
                    }
                }
            }
        }
        private void btnEstudio_Click(object sender, RoutedEventArgs e)
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
                            listarEstudios(gui.estudio);
                        }
                        else
                        {
                            MessageBox.Show("No esta registrado un precio en el sistema para el estudio.\n Comuniquese con el área de Atencion al Cliente o Convenios", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                        }
                    }
                }
            }
        }

        public void listarEstudios(Search_Estudio search_Estudio)
        {
            if (search_Estudio != null)
                if (LstEstudios.Where(x => x.codigoestudio == search_Estudio.codigoestudio).ToList().Count == 0)
                    LstEstudios.Add(search_Estudio);

            gridEstudios.ItemsSource = null;
            gridEstudios.ItemsSource = LstEstudios.ToList();
        }
        private void btnGuardarProforma_Click(object sender, RoutedEventArgs e)
        {
            PROFORMA proforma;
            List<DETALLEPROFORMA> lstdetalle;
            if (validarDatos(out proforma, out lstdetalle))
            {
                //guardarmos la proforma
                GuardarProforma(proforma, lstdetalle);
                MessageBox.Show("Se registró con éxito la Proforma: \n N° " + proforma.numerodeproforma.ToString(), "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                imprimirProforma();
                this.Close();
            }
        }
        private void GuardarProforma(PROFORMA proforma, List<DETALLEPROFORMA> lstdetalle)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                db.PROFORMA.Add(proforma);
                db.SaveChanges();
                foreach (var item in lstdetalle)
                {
                    item.numerodeproforma = proforma.numerodeproforma;
                    db.DETALLEPROFORMA.Add(item);
                    db.SaveChanges();
                }

                //agregando pdf a la BD
                
                var _paciente =db.PACIENTE.SingleOrDefault(x=> x.codigopaciente==proforma.codigopaciente);
                if (LstAdjuntos.Count > 0)
                {
                    //byte[] archivo;
                    string fileNameCombinate = System.IO.Path.GetTempPath() + DateTime.Now.ToString("ddMMyyyyHHmm") + _paciente.apellidos + ".pdf";
                    if (new Tool().CombinarPDF_Image(fileNameCombinate, LstAdjuntos.Select(x => x.ruta).ToArray()))
                    {
                        //leemos el archivo combinado
                        /* using (System.IO.FileStream ruta = new System.IO.FileStream(fileNameCombinate, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                         {
                             int longitud = (int)ruta.Length;
                             archivo = new byte[longitud];
                             ruta.Read(archivo, 0, archivo.Length);
                             ruta.Close();
                         }*/

                        DOCESCANEADO doc = new DOCESCANEADO();
                        var nombre = proforma.numerodeproforma.ToString() + "-" + proforma.codigopaciente.ToString();
                        doc.codigodocadjunto = nombre.Length > 20 ? nombre.Substring(0, 19) : nombre;
                        doc.nombrearchivo = _paciente.apellidos + ".pdf";
                        //doc.cuerpoarchivo = archivo;
                        doc.cuerpoarchivo = null;
                        doc.fecharegistro = DateTime.Now;
                        doc.codigousuario = session.codigousuario;
                        doc.isFisico = true;
                        proforma.codigodocescaneado = doc.codigodocadjunto;
                        db.DOCESCANEADO.Add(doc);
                        db.SaveChanges();
                        //copiamos el archivo 
                        Directory.CreateDirectory(Tool.PathDocumentosAdjuntos + doc.codigodocadjunto);
                        File.Copy(fileNameCombinate, Tool.PathDocumentosAdjuntos + doc.codigodocadjunto + @"\" + doc.nombrearchivo);

                    }
                }

                new CartaDAO().insertLog(proforma.numerodeproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la proforma N° " + proforma.numerodeproforma.ToString());
            }
        }

        private bool validarDatos(out PROFORMA proforma, out List<DETALLEPROFORMA> lstdetalle)
        {
            string msj = "";
            proforma = new PROFORMA();
            lstdetalle = new List<DETALLEPROFORMA>();
            int numero = 0;
            if (int.TryParse(lblidpaciente.Content.ToString(), out numero))
                proforma.codigopaciente = numero;
            else
                msj += "- Seleccione un Paciente.\n";
            if (txtcmp.Text != "")
                proforma.cmp = txtcmp.Text;
            else
                msj += "- Seleccione un Médico.\n";
            if (lblrucAseguradora.Content != null)
                if (lblrucAseguradora.Content.ToString() != "" && lblrucAseguradora.Content.ToString() != "-")
                {
                    proforma.codigocompaniaseguro = int.Parse(txtidAseguradora.Text);
                    proforma.ruc = lblrucAseguradora.Content.ToString();
                }
                else
                    msj += "- Seleccione una Aseguradora.\n";
            if (lblidClinica.Content != null)
                if (lblidClinica.Content.ToString() != "" && lblidClinica.Content.ToString() != "-")
                    proforma.codigoclinica = int.Parse(lblidClinica.Content.ToString());
                else
                    msj += "- Seleccione una Clínica de Procedencia.\n";
            if (LstEstudios.Count() == 0)
                msj += "- Ingrese los Estudios.\n";
            if (chkSedacion.IsChecked.Value && txtSedacion.Text == "")
                msj += "- Debe ingresar el motivo de sedación.\n";

            proforma.poliza = txtnumero_carnet.Text;
            proforma.tiempoenfermedad = "";
            proforma.fechaemision = Tool.getDatetime(); ;
            proforma.montototal = 0;
            proforma.titular = txttitular.Text;
            proforma.sedacion = chkSedacion.IsChecked.Value;
            proforma.contratante = txtempresa.Text;
            proforma.codigodocescaneado = "";
            proforma.codigousuario = session.codigousuario;
            proforma.estado = "INICIADA";
            proforma.observacion = (txtSedacion.Text + " " + txtcomentarios.Text).Trim();
            proforma.ishospitalizado = chkhospitalizado.IsChecked.Value;

            foreach (var item in LstEstudios)
            {
                DETALLEPROFORMA det = new DETALLEPROFORMA();
                det.numerodeproforma = proforma.numerodeproforma;
                det.monto = float.Parse(item.precio.ToString());
                det.codigoestudio = item.codigoestudio;
                det.codigoclase = item.codigoclase;
                proforma.montototal += det.monto;

                lstdetalle.Add(det);
            }

            if (msj != "")
            {
                MessageBox.Show("Verifique los siguientes errores:\n" + msj, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
                return true;


        }
        private void MenuItemAbrir_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Web item = (Adjuntos_Web)this.grid_adjuntos.SelectedItem;
            if (item != null)
            {
                System.Diagnostics.Process.Start(item.ruta);
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuItemAgregar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Carta|*.JPG;*.PNG;*.PDF";
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
                    LstAdjuntos.Add(item);
                    item.isFisico = true;
                    ListarAdjunto();
                }

            }
        }

        private void ListarAdjunto()
        {
            grid_adjuntos.ItemsSource = null;
            grid_adjuntos.ItemsSource = LstAdjuntos;

        }

        private void MenuItemEliminar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Desktop item = (Adjuntos_Desktop)this.grid_adjuntos.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea elimiar el archivo permamentemente?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    LstAdjuntos.Remove(item);
                    ListarAdjunto();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
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
            Adjuntos_Web _item = (Adjuntos_Web)e.Row.DataContext;
            System.Diagnostics.Process.Start(_item.ruta);
        }

        private void btnGuardarCarta_Click(object sender, RoutedEventArgs e)
        {
            PROFORMA proforma;
            List<DETALLEPROFORMA> lstdetalle;
            string msj = "";
            if (grid_adjuntos.DataContext != null)
            {
                if (((List<Adjuntos_Web>)grid_adjuntos.DataContext).Count == 0)
                    msj += "- Ingrese Documentos Adjuntos.\n";
            }
            else
                msj += "- Ingrese Documentos Adjuntos.\n";

            if (msj == "")
            {
                if (validarDatos(out proforma, out lstdetalle))
                {
                    proforma.estado = "TRAMITADA";
                    //guardarmos la proforma
                    GuardarProforma(proforma, lstdetalle);
                    frmDatosCarta gui = new frmDatosCarta();
                    gui.ShowDialog();
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        CARTAGARANTIA item = new CARTAGARANTIA();
                        item.seguimiento = proforma.observacion;
                        item.estadocarta = gui.carta.estadocarta;
                        item.fechatramite = Tool.getDatetime();
                        item.contratante = proforma.contratante;
                        item.titular = proforma.titular;
                        item.poliza = "";
                        item.codigocartagarantia = new CartaDAO().getNewCodigoCarta();
                        item.codigocartagarantia2 = gui.carta.codigocartagarantia2;
                        item.cmp = proforma.cmp;
                        item.codigocompaniaseguro = proforma.codigocompaniaseguro;
                        item.ruc = proforma.ruc;
                        item.codigopaciente = proforma.codigopaciente;
                        item.cobertura = gui.carta.cobertura;
                        item.monto = proforma.montototal;
                        item.codigousuario = session.codigousuario;
                        item.fechaprobacion = Tool.getDatetime();
                        item.numerocarnetseguro = proforma.poliza;
                        item.codigodocadjunto = proforma.codigodocescaneado;
                        item.sedacion_carta = proforma.sedacion;
                        item.codigocompaniaseguro2 = proforma.codigocompaniaseguro;
                        item.isRevisada = false;
                        item.isCaducada = false;
                        item.obs_revision = "";
                        item.numero_proforma = proforma.numerodeproforma;
                        db.CARTAGARANTIA.Add(item);
                        db.SaveChanges();
                        foreach (var detalle in lstdetalle)
                        {
                            ESTUDIO_CARTAGAR det = new ESTUDIO_CARTAGAR();
                            det.codigoestudio = detalle.codigoestudio;
                            det.codigocartagarantia = item.codigocartagarantia;
                            det.cmp = item.cmp;
                            det.codigocompaniaseguro = item.codigocompaniaseguro;
                            det.ruc = item.ruc;
                            det.codigopaciente = item.codigopaciente;
                            det.codigoclase = detalle.codigoclase;
                            det.preciobruto = detalle.monto;
                            det.cobertura_det = item.cobertura;
                            if (det.cobertura_det != null && det.preciobruto != null)
                                det.descuento = float.Parse((((det.cobertura_det.Value * det.preciobruto.Value) / 100) * -1.0).ToString());
                            db.ESTUDIO_CARTAGAR.Add(det);
                            db.SaveChanges();
                        }
                        new CartaDAO().insertLog(proforma.numerodeproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la Carta Garantia N° " + item.codigocartagarantia);
                        new CartaDAO().insertLog(item.codigocartagarantia, this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la Carta Garantia N° " + item.codigocartagarantia);

                        MessageBox.Show("Se registró con éxito :\n- Proforma N° " + proforma.numerodeproforma + "\n- Carta Grantia N°" + item.codigocartagarantia, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        imprimirProforma();
                        this.Close();
                    }
                }
            }
            else
                MessageBox.Show("Verifique los siguientes errores", "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            PROFORMA pro;
            List<DETALLEPROFORMA> det;
            if (validarDatos(out pro, out det))
            {
                try
                {
                    using(DATABASEGENERALEntities db = new DATABASEGENERALEntities())
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

                    var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == pro.codigopaciente);
                    //                    ws.Cells[1, 1] = "RESOCENTRO";
                    ws.Cells[2, 1] = "FECHA DE RECEPCIÓN:";
                    ws.Cells[2, 2] = currentDate.ToString("dd/MM/yyyy   hh:mm tt");
                    ws.Cells[3, 1] = "Número Proforma:";
                    ws.Cells[3, 2] = pro.numerodeproforma;
                    ws.Cells[4, 1] = "PACIENTE:";
                    ws.Cells[4, 2] = paciente.apellidos + ", " + paciente.nombres;
                    ws.Cells[5, 1] = "TITULAR:";
                    ws.Cells[5, 2] = this.txttitular.Text;
                    ws.Cells[6, 1] = "COMPAÑIA DE SEGUROS:";
                    ws.Cells[6, 2] = this.lblAseguradora.Content.ToString();
                    ws.Cells[7, 1] = "No. DE POLIZA/CREDENCIAL:";
                    ws.Cells[7, 2] = "";
                    ws.Cells[8, 1] = "EMPRESA CONTRATANTE:";
                    ws.Cells[8, 2] = this.txtempresa.Text;
                    ws.Cells[9, 1] = "TELEFONOS:";
                    ws.Cells[9, 2] = "";
                    ws.Cells[10, 1] = "CLINICA DE PROCEDENCIA:";
                    ws.Cells[10, 2] = this.lblclinica.Content.ToString();
                    ws.Cells[11, 1] = "MEDICO TRATANTE:";
                    ws.Cells[11, 2] = this.lblMedico.Content.ToString();
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
                    ws.Cells[fila, 4] = det.Sum(x => x.monto).ToString();

                    app.Columns.AutoFit();

                    ws.Range["A2", "A40"].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    ws.Range["A2", "A40"].Font.Bold = true;
                    ws.Range["A4", "B4"].Font.Bold = true;
                    ws.Range["A1", "C1"].Font.Bold = true;
                    ws.Range["A1", "C1"].Font.Size = 36;
                    ws.Range["A1", "C1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }
            }
                catch (Exception)
                {
                    MessageBox.Show("Ocurrio un error al exportar, intente nuevamente", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            imprimirProforma(); 
        }

        private void imprimirProforma()
        {
            if (MessageBox.Show("Desea Imprimir la Proforma","IMPRESION PROFORMA",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                try
                {
                    frmPrintProforma gui = new frmPrintProforma();
                    gui.cargarGUI(LstEstudios, lblidpaciente.Content.ToString(), lblAseguradora.Content.ToString(), lblMedico.Content.ToString(), lblclinica.Content.ToString(), txtempresa.Text, txtcomentarios.Text, txtcontraste.Value.Value, txtsedacion.Value.Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            buscarPaciente();
            
        }
        public void buscarPaciente()
        {
            try
            {
                frmSearchPaciente gui = new frmSearchPaciente();
                gui.ShowDialog();
                if (gui.paciente != null)
                {
                    
                    txtnombre_paciente.Text = gui.paciente.nombres.ToUpper().Trim();
                    txtapellidos_paciente.Text = gui.paciente.apellidos.ToUpper().Trim();
                    dtpNacimiento_paciente.SelectedDate = gui.paciente.fechanace;
                    cbotipo_doc_paciente.SelectedValue = gui.paciente.tipo_doc;
                    txtnum_doc_Paciente.Text = gui.paciente.dni;
                    txttelefono.Text = gui.paciente.telefono;
                    txtcelular.Text = gui.paciente.celular;
                    lblidpaciente.Content = gui.paciente.codigopaciente;
                }
                else
                {
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

        private void txtnombrepaciente_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            int idpaciente = 0;
            if (int.TryParse(lblidpaciente.Content.ToString(), out idpaciente))
                ContextMenuPaciente.Visibility = Visibility.Visible;
            else
                ContextMenuPaciente.Visibility = Visibility.Collapsed;
        }

        private void updatePaciente()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                #region Paciente
                int codigopaciente = 0;
                if (int.TryParse(lblidpaciente.Content.ToString(), out codigopaciente))
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


    }
}
