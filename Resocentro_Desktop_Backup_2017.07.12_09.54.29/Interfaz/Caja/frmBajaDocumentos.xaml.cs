using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
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

namespace Resocentro_Desktop.Interfaz.Cobranza
{
    /// <summary>
    /// Lógica de interacción para frmBajaDocumentos.xaml
    /// </summary>
    public partial class frmBajaDocumentos : Window
    {
        MySession session;
        DateTime fechareferencia;
        DocumentoSunat item = new DocumentoSunat();
        DOCUMENTO doc = null;
        string filename = "";
        public frmBajaDocumentos()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (validarDocumento())
            {
                string[] numerodocumentosplit = txtnumerodocumento.Text.Split('-');
                item.tipoDocumento = (int)TIPO_DOCUMENTO_ELECTRONICO.COMUNICACION_DE_BAJA;
                item.empresa = int.Parse(cbosede.SelectedValue.ToString().Substring(0, 1));
                item.sede = int.Parse(cbosede.SelectedValue.ToString().Substring(1, 2));
                item.detalleCancelar = new List<DocumentosxCancelar>();
                item.fechaEmision = DateTime.Now;
                item.idFirmaDigital = "JHONAB";
                item.numeroDocumento = DateTime.Now.ToString("yyyyMMdd") + "-" + (new CobranzaDAO().getNumeroDocumentoAnulacion(cbosede.SelectedValue.ToString()));//se obtendra el correlativo se la tabla seria para anular 
                item.codigopaciente = 123456789;
                item.fechaReferenciaDocumento = fechareferencia.ToString("yyyy-MM-dd");//fecha de realizacion del documento
                DocumentosxCancelar dc = new DocumentosxCancelar();
                dc.tipodocumento = int.Parse(cboTipoDocumento.Text.Substring(0, 2));
                dc.serie = cboTipoDocumento.Text.Substring(5, 1) + numerodocumentosplit[0];
                dc.correlativo = numerodocumentosplit[1];
                dc.motivo = txtmotivo.Text;
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    try
                    {
                        doc = db.DOCUMENTO.SingleOrDefault(x => x.numerodocumento == txtnumerodocumento.Text && x.codigounidad == item.empresa);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                }
                item.detalleCancelar.Add(dc);
                try
                {
                    filename = new CobranzaDAO().crearDocumentoXML(item) + ".zip";
                    new CobranzaDAO().addSerieAnulacion(item.empresa.ToString());
                    enviarSunat();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }

        private bool validarDocumento()
        {

            if (MessageBox.Show("¿Estas seguro(a) de anular este documento?\nse recomienda que verificar bien los datos ingresados", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                try
                {
                    var result = new CobranzaDAO().validarAnulacionDocumento(txtnumerodocumento.Text, cboTipoDocumento.Text.Substring(0, 2), cbosede.SelectedValue.ToString(), out fechareferencia);
                    //return true;
                    switch (result)
                    {
                        case ERROR_ANULACION_DOCUMENTO.NO_EXISTE:
                            MessageBox.Show("No exite el Documento");
                            break;
                        case ERROR_ANULACION_DOCUMENTO.DOCUMENTO_RESTRINGIDO:
                            MessageBox.Show("El Documento esta restringido por Contabilidad");
                            break;
                        case ERROR_ANULACION_DOCUMENTO.ERROR_CONSULTA:
                            break;
                        case ERROR_ANULACION_DOCUMENTO.CORRECTO:
                            break;
                        case ERROR_ANULACION_DOCUMENTO.DOCUMENTO_NO_ENVIADO:
                            if (txtnumerodocumento.Text.StartsWith("F"))
                                MessageBox.Show("El Documento no fue enviado a SUNAT");
                            else
                                result = ERROR_ANULACION_DOCUMENTO.CORRECTO;
                            break;
                        default:
                            break;
                    }
                    return result == ERROR_ANULACION_DOCUMENTO.CORRECTO;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }

            }
            return false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            verificarBaja();
        }

        public void verificarBaja()
        {
            if (lblticket.Content.ToString() != "")
            {
                try
                {
                    //lblticket.Content = "201700782991965";
                    string estado = new ServiceSunat().getStatus(item.empresa, lblticket.Content.ToString(), txtnumerodocumento.Text);

                    if (estado == "0")
                    {
                        if (new CobranzaDAO().anularDocumento(txtnumerodocumento.Text, cboTipoDocumento.Text.Substring(0, 2), cbosede.SelectedValue.ToString(), txtmotivo.Text, session))
                        {
                            new CobranzaDAO().addWatermakerAnulado(doc.pathFile);
                            MessageBox.Show("Se ANULO con exito el documento en SUNAT");
                            printAnulacion(doc.pathFile);
                            lblticket.Content = "";
                            this.IsEnabled = false;
                        }
                    }
                    else
                    {
                        if (estado.Contains("98"))
                            MessageBox.Show("ESTADO: EN PROCESO DE VALIDACION\nvuelva a verificar");
                        else
                        {
                            openFile(Tool.PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + lblticket.Content.ToString() + ".zip");
                            MessageBox.Show("NO se anulo documento\n ESTADO: PROCESO CON ERRORES \n\nVERIFIQUE CON SISTEMAS");

                            lblticket.Content = "";
                            this.IsEnabled = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void openFile(string path)
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = path;
            printJob.StartInfo.UseShellExecute = true;
            printJob.Start();
        }
        private void printAnulacion(string path)
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = path;
            printJob.Start();
        }

        public void cargarGUI(MySession session)
        {
            if (session.sucursales.ToList().Count > 0)
            {
                this.session = session;
                //cargamos los combos segun usuario
                cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
                cbosede.SelectedValuePath = "codigoInt";
                cbosede.DisplayMemberPath = "nombreShort";
                cbosede.SelectedIndex = 0;
                cboTipoDocumento.SelectedIndex = 0;
                lblticket.Content = "";
            }
            else
            {
                this.Close();
                MessageBox.Show("No tiene ninguna sucursal asignada", "ERROR");
            }
        }
        //REENVIAR
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            enviarSunat();
        }

        private void enviarSunat()
        {
            var _cobranzaDao = new CobranzaDAO();
            try
            {

                if (filename != "")
                {
                    string ticket = new ServiceSunat().sendSummary(item.empresa, filename);
                    lblticket.Content = ticket;

                    if (ticket != "")
                    {
                        btnAnular.IsEnabled = false;
                        btnReenviar.IsEnabled = false;
                        MessageBox.Show("Se esta procesando su solicitud ...");
                        verificarBaja();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lblticket.Content.ToString() != "")
            {
                e.Cancel = true;
                MessageBox.Show("El documento no está anulado, siga verificando hasta terminar el proceso.", "ADVERTENCIA", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}