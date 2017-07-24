using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmListProforma.xaml
    /// </summary>
    public partial class frmListProforma : Window
    {
        MySession session;
        public frmListProforma()
        {
            InitializeComponent();
        }
        public void iniciarGUI(MySession session)
        {
            this.session = session;
            cargarProformas();
            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Interval = new TimeSpan(0, 5, 0);
            dispatcher.Tick += (s, a) =>
            {
                if (chkUpdate.IsChecked.Value)
                    cargarProformas();
            };
            dispatcher.Start();
        }

        private void cargarProformas()
        {
            if (cboestado.Text.ToUpper() != "INICIADA" && txtfiltro.Text == "")
            {
                MessageBox.Show("Ingrese un filtro para la Busqueda");
                return;
            }
            gridProformas.ItemsSource = null;
            gridProformas.ItemsSource = new CartaDAO().getListaProformas(cboestado.Text.ToUpper(), txtfiltro.Text);
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            cargarProformas();
        }
        private void btnNewCarta_Click(object sender, RoutedEventArgs e)
        {
            frmCarta gui = new frmCarta();
            gui.cargarGUI(session, false);
            gui.Show();
        }

        private void MenuItemCancelar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            List_Proforma item = (List_Proforma)this.gridProformas.SelectedItem;
            if (item != null)
            {
                if (MessageBox.Show("¿Desea Cancelar la Proforma?", "Pregunta?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    new CartaDAO().setCancelarProforma(item.numeroproforma);
                    new CartaDAO().insertLog(item.numeroproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se canceló la proforma N° " + item.numeroproforma.ToString());
                    cargarProformas();
                }
            }
        }
        private void MenuItemAgregar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            List_Proforma item = (List_Proforma)this.gridProformas.SelectedItem;
            if (item != null)
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var proforma = db.PROFORMA.SingleOrDefault(x => x.numerodeproforma == item.numeroproforma);
                    if (proforma != null)
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Archivos de Carta|*.JPG;*.PNG;*.PDF";
                        ofd.Title = "Seleccione los archivos";
                        ofd.Multiselect = true;
                        List<string> filesUpload = new List<string>();
                        if (ofd.ShowDialog().Value)
                        {
                            foreach (var file in ofd.FileNames)
                            {
                                var _f = new FileInfo(file);
                                filesUpload.Add(file);
                            }
                                if (proforma.codigodocescaneado == "")
                                {
                                    DOCESCANEADO doc = new DOCESCANEADO();
                                    var nombre = proforma.numerodeproforma.ToString() + "-" + proforma.codigopaciente.ToString();
                                    doc.codigodocadjunto = nombre.Length > 20 ? nombre.Substring(0, 19) : nombre;
                                    doc.nombrearchivo = (item.paciente.Split(',')[1]).Trim() + ".pdf";
                                    //doc.cuerpoarchivo = archivo;
                                    doc.cuerpoarchivo = null;
                                    doc.fecharegistro = DateTime.Now;
                                    doc.codigousuario = session.codigousuario;
                                    doc.isFisico = true;
                                    proforma.codigodocescaneado = doc.codigodocadjunto;
                                    db.DOCESCANEADO.Add(doc);
                                    db.SaveChanges();
                                    //copiamos el archivo 
                                    
                                    string fileNameCombinate = System.IO.Path.GetTempPath() + DateTime.Now.ToString("ddMMyyyyHHmm") + (item.paciente.Split(',')[1]).Trim() + ".pdf";
                                    if (new Tool().CombinarPDF_Image(fileNameCombinate, filesUpload.ToArray()))
                                    {
                                        Directory.CreateDirectory(Tool.PathDocumentosAdjuntos + doc.codigodocadjunto);
                                        File.Copy(fileNameCombinate, Tool.PathDocumentosAdjuntos + doc.codigodocadjunto + @"\" + doc.nombrearchivo);
                                    }
                                }
                                else
                                {
                                    var documento = db.DOCESCANEADO.SingleOrDefault(x => x.codigodocadjunto == proforma.codigodocescaneado);
                                    if (documento != null)
                                    {
                                        var ruta = System.IO.Path.GetTempPath() + (item.paciente.Split(',')[1]).Trim() + ".pdf";
                                        if (System.IO.File.Exists(ruta))
                                        {
                                            System.IO.File.Delete(ruta);
                                        }
                                        if (documento.isFisico)
                                        {
                                            File.Copy(Tool.PathDocumentosAdjuntos + documento.codigodocadjunto + @"\" + documento.nombrearchivo, ruta, true);
                                        }
                                        else
                                        {
                                            System.IO.FileStream archivo = new System.IO.FileStream(ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                            archivo.Write(documento.cuerpoarchivo, 0, documento.cuerpoarchivo.Length);
                                            archivo.Close();
                                        }
                                        filesUpload.Add(ruta);
                                        string fileNameCombinate = System.IO.Path.GetTempPath() + DateTime.Now.ToString("ddMMyyyyHHmm") + (item.paciente.Split(',')[1]).Trim() + ".pdf";
                                        if (new Tool().CombinarPDF_Image(fileNameCombinate, filesUpload.ToArray()))
                                        {
                                            Directory.CreateDirectory(Tool.PathDocumentosAdjuntos + documento.codigodocadjunto);
                                            File.Copy(fileNameCombinate, Tool.PathDocumentosAdjuntos + documento.codigodocadjunto + @"\" + documento.nombrearchivo,true);
                                        }
                                    }
                                }
                            
                            MessageBox.Show("Se agrego el documento");
                            new CartaDAO().insertLog(item.numeroproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Insert, "Se agrego un documento adjunto a la proforma N° " + item.numeroproforma.ToString());
                            cargarProformas();
                        }
                    }

                }

            }
        }
        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            List_Proforma item = (List_Proforma)this.gridProformas.SelectedItem;
            if (item == null)
            {
                MenuContext_Lista.Visibility = Visibility.Collapsed;
            }
            else
            {
                MenuContext_Lista.Visibility = Visibility.Visible;
            }
        }

        private void gridProformas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            List_Proforma item = (List_Proforma)e.Row.DataContext;
            if (item != null)
            {
                var proforma = new CartaDAO().getProformaxCodigo(item.numeroproforma);
                var detalle = new CartaDAO().getDetalleProformaxCodigo(item.numeroproforma);
                if (proforma != null)
                {
                    new CartaDAO().insertLog(item.numeroproforma.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la proforma N° " + item.numeroproforma.ToString());
                    CARTAGARANTIA carta = new CARTAGARANTIA();
                    carta.seguimiento = proforma.observacion;
                    carta.estadocarta = "INICIADA";
                    carta.fechatramite = Tool.getDatetime();
                    carta.contratante = proforma.contratante;
                    carta.titular = proforma.titular;
                    carta.poliza = proforma.poliza;
                    carta.codigocartagarantia = "";
                    carta.cmp = proforma.cmp;
                    carta.codigoclinica = proforma.codigoclinica;
                    carta.codigoclinica2 = proforma.codigoclinica;
                    carta.codigocompaniaseguro = proforma.codigocompaniaseguro;
                    carta.ruc = proforma.ruc;
                    carta.codigopaciente = proforma.codigopaciente;
                    carta.cobertura = 0;
                    carta.monto = 0;
                    carta.codigodocadjunto = proforma.codigodocescaneado;
                    carta.fechaprobacion = carta.fechatramite;
                    carta.codigousuario = "";
                    carta.numero_proforma = proforma.numerodeproforma;
                    carta.isRevisada = false;
                    carta.sedacion_carta = proforma.sedacion;
                    carta.ishospitalizado = proforma.ishospitalizado == null ? false : proforma.ishospitalizado.Value;
                    List<ESTUDIO_CARTAGAR> estudios = new List<ESTUDIO_CARTAGAR>();
                    if (detalle.Count > 0)
                    {
                        foreach (var d in detalle)
                        {
                            ESTUDIO_CARTAGAR est = new ESTUDIO_CARTAGAR();
                            est.codigoestudio = d.codigoestudio;
                            est.codigocartagarantia = "";
                            estudios.Add(est);
                        }
                    }

                    frmCarta gui = new frmCarta();
                    gui.cargarGUI(session, false);
                    gui.isClose = true;
                    gui.Show();
                    gui.setCartaGarantia(carta, estudios, false);
                }




            }
            else
                MessageBox.Show("Debe seleccionar una solicitud web", "Advertencia!!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnNewProforma_Click(object sender, RoutedEventArgs e)
        {
            frmProforma gui = new frmProforma();
            gui.iniciarGUI(session);
            gui.Show();
        }

        private void MenuItemExcel_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(gridProformas, true, false, false);
        }


    }
}
