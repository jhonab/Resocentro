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
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Interfaz.frmUtil;
using System.IO;
using Microsoft.Win32;
using Resocentro_Desktop.Interfaz.frmCarta;
namespace Resocentro_Desktop
{
    /// <summary>
    /// Lógica de interacción para frmSolicitudWeb.xaml
    /// </summary>
    public partial class frmSolicitudWeb : Window
    {
        MySession session;
        CG_SolicitudCarta solicitud;
        List<Search_Estudio> LstEstudios;
        public frmSolicitudWeb()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session, CG_SolicitudCarta item)
        {
            this.session = session;
            this.solicitud = item;
            //Datos de Solicitud
            lblnombreSolicitud.Content = solicitud.apellidos.Trim().ToUpper() + ", " + solicitud.nombres.Trim().ToUpper();
            lbltelefonosSolicitud.Content = solicitud.telefono.ToString().Trim().ToUpper() + " " + solicitud.celular.ToString().Trim().ToUpper();
            lblcorreoSolicitud.Content = solicitud.email.Trim().ToLower();
            lblaseguradoraSolicitud.Content = solicitud.aseguradora.Trim().ToUpper();
            lblclinicaSolicitud.Content = solicitud.clinica.Trim().ToUpper();
            lblmedicoSolicitud.Content = solicitud.apellido_medico.Trim().ToUpper() + ", " + solicitud.nombre_medico.Trim().ToUpper();
            txtcomentarioSolicitud.Text = solicitud.comentario.Trim().ToLower();

            //cargamos Pacientes que conicidan con el DNI
            if (solicitud.num_doc != "")
            {
                chkdni.IsChecked = true;
                buscarPaciente(solicitud.num_doc, "", "");
            }

            //cargamos los combos segun usuario
            cbosucursal.ItemsSource = new UtilDAO().getSucursales(session.sucursales);
            cbosucursal.SelectedValuePath = "codigoInt";
            cbosucursal.DisplayMemberPath = "nombreShort";
            cbosucursal.SelectedIndex = 0;

            //cargarmos los adjuntos
            ListarAdjunto();

            //
            LstEstudios = new List<Search_Estudio>();
            //fijamos el cursor
            txttitular.Focus();
            //actualizamos a leido
            if (solicitud.estado == (int)Estado_SolicitudTramite.Nuevo)
            {
                solicitud.estado = (int)Estado_SolicitudTramite.Leido;
                solicitud.fec_lectura = Tool.getDatetime(); 
                solicitud.usu_lectura = session.codigousuario;
                solicitud.numeroproforma = null;
                new CartaDAO().updateSolicitudCarta(solicitud, session.shortuser);
            }
            else
            {
                if (solicitud.estado == (int)Estado_SolicitudTramite.Tramitado)
                {
                    btnGuardarProforma.Visibility = Visibility.Collapsed;
                    btnGuardarCarta.Visibility = Visibility.Collapsed;
                }
            }
            new CartaDAO().insertLog(this.solicitud.idSolicitud.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Abrio la la solicitud");


        }
        public void ListarAdjunto()
        {
            grid_adjuntos.ItemsSource = null;
            grid_adjuntos.ItemsSource = new Tool().getAdjuntosWeb(solicitud.idSolicitud.ToString());
        }
        public void buscarPaciente(string dni, string apellidos, string nombre)
        {
            if (dni != "" || apellidos != "" || nombre != "")
                gridPacientes.ItemsSource = new PacienteDAO().getBuscarPacienteExacto(dni, apellidos, nombre);
            else
                gridPacientes.ItemsSource = null;
        }
        private void chkChange_Click(object sender, RoutedEventArgs e)
        {
            string dni = "", apellido = "", nombre = "";
            if (chkdni.IsChecked.Value)
                dni = solicitud.num_doc;
            if (chkapellidos.IsChecked.Value)
                apellido = solicitud.apellidos;
            if (chknombre.IsChecked.Value)
                nombre = solicitud.nombres;

            buscarPaciente(dni, apellido, nombre);
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
            gui.setGUI(solicitud.aseguradora);
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
            gui.setGUI(solicitud.clinica);
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
            gui.setGUI(solicitud.apellido_medico);
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
                    var _medico = new UtilDAO().getMedicoxCMP(txtcmp.Text,1);
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
                this.Close();
            }
        }
        private void btnGuardarCarta_Click(object sender, RoutedEventArgs e)
        {
            PROFORMA proforma;
            List<DETALLEPROFORMA> lstdetalle;
            string msj = "";
            if (grid_adjuntos.ItemsSource != null)
            {
                if (((List<Adjuntos_Web>)grid_adjuntos.ItemsSource).Count == 0)
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
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        CARTAGARANTIA item = new CARTAGARANTIA();
                        item.seguimiento = proforma.observacion;
                        item.estadocarta = "TRAMITADA";
                        item.fechatramite = Tool.getDatetime();
                        item.contratante = proforma.contratante;
                        item.titular = proforma.titular;
                        item.poliza = "";
                        item.codigocartagarantia = new CartaDAO().getNewCodigoCarta();
                        item.cmp = proforma.cmp;
                        item.codigocompaniaseguro = proforma.codigocompaniaseguro;
                        item.ruc = proforma.ruc;
                        item.codigopaciente = proforma.codigopaciente;
                        item.cobertura = 0;
                        item.monto = proforma.montototal;
                        item.codigousuario = session.codigousuario;
                        item.fechaprobacion = Tool.getDatetime();
                        item.numerocarnetseguro = "";
                        item.codigodocadjunto = proforma.codigodocescaneado;
                        item.codigocompaniaseguro2 = proforma.codigocompaniaseguro;
                        item.isRevisada = false;
                        item.isCaducada = false;
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
                            det.cobertura_det = 0;
                            db.ESTUDIO_CARTAGAR.Add(det);
                            db.SaveChanges();
                        }
                        new CartaDAO().insertLog(this.solicitud.idSolicitud.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la Carta Garantia N° " + item.codigocartagarantia);
                        new CartaDAO().insertLog(item.codigocartagarantia, this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la Carta Garantia N° " + item.codigocartagarantia);

                        MessageBox.Show("Se registró con éxito :\n- Proforma N° " + proforma.numerodeproforma + "\n- Carta Grantia N°" + item.codigocartagarantia, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (MessageBox.Show("¿Desea enviar un correo al paciente con la confirmación de su tramite?", "Pregunta??", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            #region msj
                            string titulo = @"<TABLE style='BORDER-COLLAPSE: collapse; BORDER-SPACING: 0'>
 <TBODY>
  <TR >
   <TD  style='WIDTH: 80%;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD class='padded'  style='WORD-WRAP: break-word; PADDING-LEFT: 0px; FONT-SIZE: 12px; FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: left;'>
        <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " +
                                                           solicitud.nombres
                                              + @"</h1>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
   <TD  style='WIDTH: 20%; VERTICAL-ALIGN: top; PADDING-BOTTOM: 32px;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD style='WORD-WRAP: break-word;FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: right;'>
         <a style='TEXT-DECORATION: none; FONT-FAMILY: Tahoma,sans-serif;float: right;font-size: 15px;'>N° Solicitud: " + solicitud.idSolicitud + @"</a>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
  </TR>
 </TBODY>
</TABLE>", cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 15px;line-height: 25px;Margin-bottom: 24px'>
Su solicitud fue procesada en nuestro sistema y se inicio el proceso en la Aseguradora, estamos a la espera a la información para proceder con la cita .
</p>
", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";
                            #endregion
                            new Tool().sendCorreo("Respuesta a su Solicitud", solicitud.email, "", new Tool().getCuerpoEmail(img, titulo, cuerpo), "");
                        }
                        this.Close();
                    }
                }
            }
            else
                MessageBox.Show("Verifique los siguientes errores", "Error!!", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var lstadj = new Tool().getAdjuntosWeb(solicitud.idSolicitud.ToString());
                if (lstadj.Count > 0)
                {
                    //byte[] archivo;
                    string fileNameCombinate = System.IO.Path.GetTempPath() + DateTime.Now.ToString("ddMMyyyyHHmm") + solicitud.apellidos + ".pdf";
                    if (new Tool().CombinarPDF_Image(fileNameCombinate, lstadj.Select(x => x.ruta).ToArray()))
                    {
                        //leemos el archivo combinado
                       /* using (System.IO.FileStream ruta = new System.IO.FileStream(fileNameCombinate, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            int longitud = (int)ruta.Length;
                            archivo = new byte[longitud];
                            ruta.Read(archivo, 0, archivo.Length);
                            ruta.Close();
                        }
                        */
                        DOCESCANEADO doc = new DOCESCANEADO();
                        var nombre = proforma.numerodeproforma.ToString() + "-" + proforma.codigopaciente.ToString();
                        doc.codigodocadjunto = nombre.Length > 20 ? nombre.Substring(0, 19) : nombre;
                        doc.nombrearchivo = solicitud.apellidos + ".pdf";
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
                        File.Copy(fileNameCombinate, Tool.PathDocumentosAdjuntos + doc.codigodocadjunto+@"\"+doc.nombrearchivo);


                    }
                }

                new CartaDAO().insertLog(this.solicitud.idSolicitud.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la proforma N° " + proforma.numerodeproforma.ToString());
                new CartaDAO().insertLog(proforma.numerodeproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se creó la proforma N° " + proforma.numerodeproforma.ToString());

                solicitud.estado = (int)Estado_SolicitudTramite.Tramitado;
                solicitud.numeroproforma = proforma.numerodeproforma;
                new CartaDAO().updateSolicitudCarta(solicitud, session.codigousuario);
            }
        }

        private bool validarDatos(out PROFORMA proforma, out List<DETALLEPROFORMA> lstdetalle)
        {
            string msj = "";
            proforma = new PROFORMA();
            lstdetalle = new List<DETALLEPROFORMA>();
            if ((PACIENTE)this.gridPacientes.SelectedItem != null)
                proforma.codigopaciente = ((PACIENTE)this.gridPacientes.SelectedItem).codigopaciente;
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

            proforma.tiempoenfermedad = "";
            proforma.fechaemision = Tool.getDatetime();
            proforma.montototal = 0;
            proforma.titular = txttitular.Text;
            proforma.sedacion = chkSedacion.IsChecked.Value;
            proforma.contratante = txtempresa.Text;
            proforma.codigodocescaneado = "";
            proforma.poliza = "";
            proforma.codigousuario = session.codigousuario;
            proforma.estado = "INICIADA";
            proforma.observacion = (txtSedacion.Text + " " + txtcomentarios.Text).Trim();
            proforma.ishospitalizado = false;

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

        private void grid_adjuntos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Adjuntos_Web _item = (Adjuntos_Web)e.Row.DataContext;
            System.Diagnostics.Process.Start(_item.ruta);
        }
        private void Historial_Click(object sender, RoutedEventArgs e)
        {
            frmCorreoSolicitu gui = new frmCorreoSolicitu();
            gui.cargarHistorial(session, solicitud.idSolicitud, false);
            gui.ShowDialog();
        }
        private void Responder_Click(object sender, RoutedEventArgs e)
        {
            frmCorreoSolicitu gui = new frmCorreoSolicitu();
            gui.cargarHistorial(session, solicitud.idSolicitud, true);
            gui.ShowDialog();
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
                    var _x = file.Split('\\');
                    string path = Tool.PathFileSolicitudes + solicitud.idSolicitud;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    File.Copy(file, path + "\\" + _x[_x.Length - 1], true);
                    new CartaDAO().insertLog(this.solicitud.idSolicitud.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se  agregó un adjunto " + _x[_x.Length - 1]);
                    ListarAdjunto();
                }

            }
        }

        private void MenuItemEliminar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Web item = (Adjuntos_Web)this.grid_adjuntos.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea elimiar el archivo permamentemente?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete(item.ruta);
                    new CartaDAO().insertLog(this.solicitud.idSolicitud.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se  agregó un adjunto " + item.nombre);
                    ListarAdjunto();
                }
            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Adjuntos_Web item = (Adjuntos_Web)this.grid_adjuntos.SelectedItem;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frmPaciente gui = new frmPaciente();
            PACIENTE p = new PACIENTE();
            p.nombres = solicitud.nombres;
            p.apellidos = solicitud.apellidos;
            p.email = solicitud.email;
            p.telefono = solicitud.telefono.ToString();
            p.celular = solicitud.celular.ToString();
            p.dni = solicitud.num_doc;
            gui.insertPaciente(p);
            gui.ShowDialog();
            buscarPaciente(solicitud.num_doc, "", "");
        }







    }
}
