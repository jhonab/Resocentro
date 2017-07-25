using Resocentro_Server.DAO;
using Resocentro_Server.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Resocentro_Server
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
            int seconds = 0;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer dispathcer = new DispatcherTimer();

            //EL INTERVALO DE TIMER ES DE 1 SEGUNDO 
            dispathcer.Interval = new TimeSpan(0, 0, 1);
            dispathcer.Tick += (s, a) =>
            {
                ejecutarMetodos();

            };
            dispathcer.Start();
            rtpEnvio_Boletas.SelectedTime = new TimeSpan(2, 0, 0);
            txtminutos_factura.Value = 10;
        }

        private void ejecutarMetodos()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                seconds--;
                lblcontadorfactura.Content = "Faltan "+seconds+" segundos";
                //cada 10 minutos se envian facturas
                if (seconds == 0)
                {
                    enviar_facturas();
                    seconds = int.Parse(txtminutos_factura.Value.Value.ToString())*60;
                }

                //segun programacion de GUI se envian las boletas
                if (rtpEnvio_Boletas.SelectedTime == new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0))
                {
                    enviar_Boletas();
                }
            }
            ));

        }

        private void enviar_Boletas()
        {
            DateTime fecha = DateTime.Now.AddDays(-1);
            int[] empresas = new int[] { 1, 2 };
            foreach (int empresa in empresas)
            {
                List<PreResumenBoleta> listaresumen = new CobranzaDAO().getListaResumen(fecha.ToShortDateString(), empresa.ToString());
                if (listaresumen.Count > 0)
                {
                    List<DetalleResumenBoleta> lstcabecera = new List<DetalleResumenBoleta>();
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
                                txtresumenBoletas.Text += "\n\n ********************************************************************ERROR******************************************************************** \n Tipo erroneo de Documento \n\n";
                                return;
                            }
                            int max = int.MaxValue, count = 0;
                            foreach (var d in listaresumen.Where(x => x.tipodocumento == tipodoc && x.serie == serie).OrderBy(x => x.correlativo).ToList())
                            {
                                if (count != 0)
                                {
                                    if (d.correlativo - 1 != count && d.correlativo != count)
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
                                            txtresumenBoletas.Text += "\n\n ********************************************************************ERROR******************************************************************** \n Tipo erroneo de Documento \n\n";
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

                    #region generar XML
                    DocumentoSunat item = new DocumentoSunat();

                    item.tipoDocumento = (int)TIPO_DOCUMENTO_ELECTRONICO.RESUMEN_DIARIO_BOLETAS;
                    item.fechaReferenciaDocumento = fecha.ToString("yyyy-MM-dd");
                    item.empresa = empresa;
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

                    var _cobranzaDao = new CobranzaDAO();
                    try
                    {
                        string filename = _cobranzaDao.crearDocumentoXML(item);
                        filename = filename + ".zip";
                        if (filename != "")
                        {
                            string pathMain = CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\" + filename;

                            byte[] byteArray = File.ReadAllBytes(pathMain);
                            try
                            {
                                string ticket = new Sunat().sendSummaryResumen(item.empresa, filename);
                                txtresumenBoletas.Text += "\n N° Ticket RESUMEN: " + ticket;

                                new CobranzaDAO().insertarResumen(item, ticket, pathMain, (CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"), "JAB456");

                                txtresumenBoletas.Text += "\n Archivo enviado con exito  EMPRESA:"+empresa;



                                string estado = "98";
                                int count = 0;
                                while (estado == "98")
                                {
                                    estado = new Sunat().getStatusResumen(item.empresa, ticket.ToString(), item.numeroDocumento);

                                    if (estado == "0")
                                    {
                                        //openFile((Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + txtticket.Text + ".zip"));
                                        txtresumenBoletas.Text += "\n Resumen Aceptado Correctamente";
                                        new CobranzaDAO().UpdateListaResumen(fecha.ToString("dd-MM-yyyy"), empresa.ToString(), (CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"));

                                        txtresumenBoletas.Text += "\n Se actualizo correctamente los Documentos  EMPRESA:" + empresa;

                                    }
                                    else if (estado.Contains("98"))
                                        txtresumenBoletas.Text += "\n Resumen en Proceso de Validacion se intentara en unos minutos  EMPRESA:" + empresa;
                                    else
                                    {
                                        openFile((CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"));

                                        txtresumenBoletas.Text += "\n  Resumen con Errores  EMPRESA:" + empresa;

                                    }
                                    count++;
                                    if (count == 20)
                                    {
                                        txtresumenBoletas.Text += "\n  Se intentaron 20 veces sin exito se cancelo intentar nuevamente EMPRESA:"+empresa+" N° Ticket" + ticket;
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                txtresumenBoletas.Text += "\n\n ********************************************************************\nERROR******************************************************************** \n" +
 ex.Message.ToString() + "\n\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        txtresumenBoletas.Text += "\n\n ********************************************************************ERROR******************************************************************** \n" +
 ex.Message.ToString() + "\n\n";
                    }

                    #endregion
                }
            }



        }

        public void enviar_facturas()
        {
            var lista = new CobranzaDAO().getFacturasPendientesSendSunat();
            txtresumen.Text += "\n"+DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Se encontraron " + lista.Count() + " factura(s) pendiente(s) de envio.\n";
            if (lista.Count > 0)
            {
                var dao = new Sunat();
                var dao1 = new CobranzaDAO();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (var item in lista)
                    {
                        try
                        {
                            if (item.resultado == "PENDIENTE")
                            {
                                string pathCDR = dao.sendBill(item.empresa, item.paciente, System.IO.Path.GetFileName(item.filename).Split('.')[0]);

                                string msjResutlado = "";
                                if (dao1.verificarCDR(pathCDR, out msjResutlado))
                                {

                                    var doc = db.DOCUMENTO.SingleOrDefault(x => x.numerodocumento == item.documento && x.codigounidad == item.empresa);
                                    if (doc != null)
                                    {
                                        item.resultado = "EXITO";
                                        doc.isSendSUNAT = true;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                    item.resultado = "ERROR - " + msjResutlado.Trim();
                            }

                            txtresumen.Text += "\n- " + item.documento + "  =>  " + item.resultado + " EMPRESA:" + item.empresa;
                        }
                        catch (Exception ex)
                        {
                            txtresumen.Text += "\n\n ********************************************************************ERROR******************************************************************** \n" +
ex.Message.ToString() + "\n\n";
                        }
                    }
                }

                txtresumen.Text += "\n\nRESUMEN RESOCENTRO => Total " + lista.Where(x => x.empresa == 1).Count() + " Facturas, Correctas: " + lista.Where(x => x.empresa == 1 && x.resultado == "EXITO").ToList().Count() + " , Pendientes: " + lista.Where(x => x.empresa == 1 && x.resultado == "PENDIENTE").ToList().Count() + " , Correctas: " + lista.Where(x => x.empresa == 1 && x.resultado.Contains("ERROR")).ToList().Count();

                txtresumen.Text += "\n\nRESUMEN EMETAC => Total " + lista.Where(x => x.empresa == 2).Count() + " Facturas, Correctas: " + lista.Where(x => x.empresa == 2 && x.resultado == "EXITO").ToList().Count() + " , Pendientes: " + lista.Where(x => x.empresa == 2 && x.resultado == "PENDIENTE").ToList().Count() + " , Correctas: " + lista.Where(x => x.empresa == 2 && x.resultado.Contains("ERROR")).ToList().Count();

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
        private void btnPausefacturas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnpauseBoletas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtminutos_factura_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            seconds = int.Parse(txtminutos_factura.Value.Value.ToString())*60;
        }

       
    }
}
