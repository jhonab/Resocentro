using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.Cobranza
{
    /// <summary>
    /// Lógica de interacción para frmResumenBoletas.xaml
    /// </summary>
    public partial class frmResumenBoletas : Window
    {
        MySession session;
        DocumentoSunat item;
        List<DetalleResumenBoleta> lstcabecera;
        public frmResumenBoletas()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboempresa.ItemsSource = new UtilDAO().getEmpresa();
            cboempresa.SelectedValuePath = "codigounidad";
            cboempresa.DisplayMemberPath = "nombre";
            cboempresa.SelectedIndex = 0;
            dtpfecha.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            item = new DocumentoSunat();
            item.tipoDocumento = (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS;
            item.fechaReferenciaDocumento = dtpfecha.SelectedDate.Value.ToString("yyyy-MM-dd");
            item.empresa = int.Parse(cboempresa.SelectedValue.ToString());
            item.detalleResumenBoleta = new List<DetalleResumenBoleta>();
            item.detalleResumenBoleta = lstcabecera;
            item.fechaEmision = DateTime.Now;
            item.idFirmaDigital = "JHONAB";
            var correlativo = new CobranzaDAO().getNumeroDocumentoResumen(item.empresa.ToString());
            if (correlativo == "")
                return;
            item.numeroDocumento = item.fechaEmision.ToString("yyyyMMdd") + "-" + correlativo;// +txtnumero.Text;
            item.numeroDocumentoSUNAT = item.numeroDocumento;
            item.codigopaciente = 123456789;
            item.codigoMoneda = 1;

            dtpfecha.IsEnabled = false;
            cboempresa.IsEnabled = false;

            var _cobranzaDao = new CobranzaDAO();
            try
            {
                string filename = _cobranzaDao.crearDocumentoXML(item);
                filename = filename + ".zip";
                if (filename != "")
                {
                    string pathMain = Tool.PathDocumentosFacturacion + "\\RESUMEN\\" + filename;

                    byte[] byteArray = File.ReadAllBytes(pathMain);
                    try
                    {
                        string ticket = new ServiceSunat().sendSummaryResumen(item.empresa, filename);
                        txtticket.Text = ticket;

                        new CobranzaDAO().insertarResumen(item, ticket, pathMain, (Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"), session);
                        btnEnviar.IsEnabled = false;
                        MessageBox.Show("Archivo enviado con exito");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            verificarResumen();          
        }

        private void verificarResumen()
        {
            try
            {
                string estado = new ServiceSunat().getStatusResumen(item.empresa, txtticket.Text.ToString(), item.numeroDocumento);

                txterror.Text = estado;
                if (estado == "0")
                {
                    //openFile((Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + txtticket.Text + ".zip"));
                    MessageBox.Show("Resumen Aceptado Correctamente");
                    new CobranzaDAO().UpdateListaResumen(dtpfecha.SelectedDate.Value.ToString("dd-MM-yyyy"), cboempresa.SelectedValue.ToString(), (Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + txtticket.Text + ".zip"));
                    dtpfecha.IsEnabled = true;
                    cboempresa.IsEnabled = true;
                    MessageBox.Show("Se actualizo correctamente los Documentos");
                    limpiarFormulario();
                }
                else if (estado.Contains("98"))
                    MessageBox.Show("Resumen en Proceso de Validacion\nIntente nuevamente en unos minutos");
                else
                {
                    openFile((Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + txtticket.Text + ".zip"));
                    dtpfecha.IsEnabled = true;
                    cboempresa.IsEnabled = true;
                    MessageBox.Show("Resumen con Errores");
                    limpiarFormulario();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void limpiarFormulario()
        {
            txtticket.Text = "";
            txterror.Text = "";
            grid_cabecera.ItemsSource = null;
            grid_detalle.ItemsSource = null;
            btnEnviar.IsEnabled = true;
            
        }

        private void openFile(string path)
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = path;
            printJob.StartInfo.UseShellExecute = true;
            printJob.Start();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            var listaresumen = new CobranzaDAO().getListaResumen(dtpfecha.SelectedDate.Value.ToString("dd-MM-yyyy"), cboempresa.SelectedValue.ToString());
            grid_detalle.ItemsSource = null;
            grid_detalle.ItemsSource = listaresumen;

            lstcabecera = new List<DetalleResumenBoleta>();
            foreach (var tipodoc in listaresumen.Select(x => x.tipodocumento).Distinct().AsParallel())
            {
                foreach (var serie in listaresumen.Where(x => x.tipodocumento == tipodoc).Select(x => x.serie).Distinct().AsParallel())
                {
                    DetalleResumenBoleta detCabecera = new DetalleResumenBoleta();
                    detCabecera.tipodocumento = tipodoc;
                    if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                        detCabecera.serie = "B" + serie;
                    else if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                        detCabecera.serie = "F" + serie;
                    else if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO || tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
                        detCabecera.serie = "B" + serie;
                    else
                    {
                        MessageBox.Show("Tipo erroneo de Documento");
                        return;
                    }
                    int max = int.MaxValue, count = 0;
                    foreach (var d in listaresumen.Where(x => x.tipodocumento == tipodoc && x.serie == serie).OrderBy(x => x.correlativo).ToList())
                    {
                        if (count != 0)
                        {
                            if (d.correlativo-1 != count && d.correlativo!=count)
                            {
                                count = 0;
                                max = int.MaxValue;
                                lstcabecera.Add(detCabecera);
                                detCabecera = new DetalleResumenBoleta();
                                detCabecera.tipodocumento = tipodoc;
                                if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.BOLETA_DE_VENTA)
                                    detCabecera.serie = "B" + serie;
                                else if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.FACTURA)
                                    detCabecera.serie = "F" + serie;
                                else if (tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_CREDITO || tipodoc == (int)TIPO_DOCUMENTO_ELECTRONICO.NOTA_DE_DEBITO)
                                    detCabecera.serie = "B" + serie;
                                else
                                {
                                    MessageBox.Show("Tipo erroneo de Documento");
                                    return;
                                }
                            }
                        }

                        if (d.correlativo < max)
                        {
                            max = d.correlativo;
                            detCabecera.inicioRango = d.correlativo.ToString();
                        }

                        detCabecera.finRango = d.correlativo.ToString();

                        if (d.tipoIGV < (int)TIPO_IGV.EXONERADO_ONEROSA)
                            detCabecera.totalVentaGravadas += d.valorventa;

                        if (d.tipoIGV < (int)TIPO_IGV.INAFECTO_RETIROBONIFICACION &&
                            d.tipoIGV > (int)TIPO_IGV.GRAVADO_RETIROENTREGATRABAJADORES)
                            detCabecera.totalVentaExonerada += d.preciounitario;

                        if (d.tipoIGV > (int)TIPO_IGV.EXONERADO_TRANSFERENCIAGRATUITA
                            && d.tipoIGV < (int)TIPO_IGV.EXPORTACION)
                            detCabecera.totalVentaInafectas += d.preciounitario;
                        detCabecera.totalOtrosCargos = 0;
                        detCabecera.totalISC = 0;
                        detCabecera.totalIGV += d.valorigv;
                        detCabecera.totalotrosTributos = 0;

                       
                        count = d.correlativo;
                    }
                    lstcabecera.Add(detCabecera);
                }
            }
            grid_cabecera.ItemsSource = null;
            grid_cabecera.ItemsSource = lstcabecera;

        }
    }
}
