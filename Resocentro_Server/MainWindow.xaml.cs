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
        int seconds = 0, seconds_verificacion = 0;
        string formatoCorreo = "N° Documento:{0} <br/> Empresa:{1}<br/> Tipo:{2} <br/> Obs:{3} <br/><br/>";
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
            txtminutos_verificacion.Value = 5;

        }


        private void ejecutarMetodos()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                try
                {
                    seconds--;
                    seconds_verificacion--;
                    lblcontadorfactura.Content = "Faltan " + seconds + " segundos";
                    lblcontadorVerificacion.Content = "Faltan " + seconds_verificacion + " segundos";
                    //cada 10 minutos se envian facturas
                    if (seconds == 0)
                    {
                        seconds = int.Parse(txtminutos_factura.Value.Value.ToString()) * 60;
                        enviar_facturas();
                        txtresumen.Select(txtresumen.Text.Length, 1);
                    }

                    //segun programacion de GUI se envian las boletas
                    if (rtpEnvio_Boletas.SelectedTime == new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0))
                    {
                        enviar_Boletas();
                        txtresumenBoletas.Select(txtresumen.Text.Length, 1);
                    }
                    //cada 5 minutos se verificaran envios
                    if (seconds_verificacion == 0)
                    {
                        seconds_verificacion = int.Parse(txtminutos_verificacion.Value.Value.ToString()) * 60;
                        VerificarEnvios();
                        txtminutos_verificacion.Select(txtresumen.Text.Length, 1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            ));

        }

        private void VerificarEnvios()
        {
            string errorCorreo = "";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var lista = db.VerificacionSunat.Where(x => x.isActivo == true).ToList();
                if (lista.Count > 0)
                {

                    txtresumen_verificacion.Text += "\n" + DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Se encontraron " + lista.Count() + " pendiente(s) de envio.\n";

                    foreach (var item in lista)
                    {
                        #region BOLETAS
                        if (item.tipo_envio == "RESUMEN")
                        {
                            string estado = "98";
                            int count = 0;
                            while (estado.Contains("98"))
                            {
                                estado = new Sunat().getStatusResumen(int.Parse(item.empresa), item.idTicket, item.numerodocumento);

                                if (estado == "0")
                                {
                                    //openFile((Tool.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + txtticket.Text + ".zip"));
                                    txtresumen_verificacion.Text += "\n Resumen Aceptado Correctamente: RESPUESTA-" + item.idTicket + ".zip";
                                    new CobranzaDAO().UpdateListaResumen(item.fecha.Value.ToString("dd-MM-yyyy"), item.empresa.ToString(), (CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + item.idTicket + ".zip"));
                                    item.isActivo = false;
                                    txtresumen_verificacion.Text += "\n Se actualizo correctamente los Documentos  EMPRESA:" + item.empresa;

                                }
                                else if (estado.Contains("98"))
                                {
                                    txtresumen_verificacion.Text += "\n Resumen en Proceso de Validacion se intentara en 30 segundod  EMPRESA:" + item.empresa;
                                    System.Threading.Thread.Sleep(30000);
                                }
                                else
                                {
                                    openFile((CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + item.idTicket + ".zip"));

                                    txtresumen_verificacion.Text += "\n  Resumen con Errores  EMPRESA:" + item.empresa;
                                    errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "VERIFICACION SUNAT", "Resumen con errores : RESPUESTA-" + item.idTicket + ".zip");
                                }
                                count++;
                                if (count == 20)
                                {
                                    txtresumen_verificacion.Text += "\n  Se intentaron 20 veces sin exito se cancelo intentar nuevamente EMPRESA:" + item.empresa + " N° Ticket" + item.idTicket + " Documento: " + item.numerodocumento;
                                    errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "VERIFICACION SUNAT", "Se intento verificar más de 20 veces sin exito, se ingreso en Verificacion SUNAT :" + item.numerodocumento);
                                    break;
                                }
                            }
                        }
                        #endregion
                        #region FACTURAS
                        if (item.tipo_envio == "FACTURA")
                        {
                            string msjResutlado = "";
                            string pathCDR = new Sunat().getCDR(int.Parse(item.empresa), item.numerodocumento, item.codigopaciente.Value, System.IO.Path.GetFileName(item.pathfile).ToString());
                            if (new CobranzaDAO().verificarCDR(pathCDR, out msjResutlado))
                            {
                                item.isActivo = false;
                                txtresumen_verificacion.Text += "\n  SE ACEPTO FACTURA EMPRESA:" + item.empresa + " Documento: " + item.numerodocumento + "\n" + msjResutlado + "\n\n";
                            }
                            else
                            {
                                txtresumen_verificacion.Text += "\n  NO SE PROCESO FACTURA EMPRESA:" + item.empresa + " Documento: " + item.numerodocumento + "\n" + msjResutlado + "\n\n";
                                errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "VERIFICACION SUNAT", "Error de CDR");
                                break;
                            }

                        }
                        #endregion
                        #region BAJAS
                        if (item.tipo_envio == "BAJA")
                        {
                            try
                            {
                                if (item.idTicket == "-")
                                    item.idTicket = new Sunat().sendSummary(int.Parse(item.empresa), item.pathfile,item.numerodocumento);
                              
                                string estado = "98";
                                int count = 0;
                                while (estado.Contains("98"))
                                {
                                    estado = new Sunat().getStatus(int.Parse(item.empresa), item.idTicket, item.numerodocumento);

                                    if (estado == "0")
                                    {
                                        new CobranzaDAO().anularDocumento(item.numerodocumento, item.tipodocumento, (item.empresa + item.sucursal.Value.ToString("D2")), "Error", item.usuario);
                                        item.isActivo = false;
                                        txtresumen_verificacion.Text += "\n Se anulo correctamente el Documentos  EMPRESA:" + item.empresa +" => "+item.numerodocumento;

                                    }
                                    else if (estado.Contains("98"))
                                    {
                                        txtresumen_verificacion.Text += "\n Resumen en Proceso de Validacion se intentara en 30 segundod  EMPRESA:" + item.empresa;
                                        System.Threading.Thread.Sleep(30000);
                                    }
                                    else
                                    {
                                        openFile(CobranzaDAO.PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + item.idTicket + ".zip");

                                        txtresumen_verificacion.Text += "\n  BAJA con errores  EMPRESA:" + item.empresa;
                                        errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "ENVIO BAJA", "Resumen con errores : RESPUESTA-" + item.idTicket + ".zip");
                                    }
                                    count++;
                                    if (count == 20)
                                    {
                                        txtresumen_verificacion.Text += "\n  Se intentaron 20 veces sin exito se cancelo intentar nuevamente EMPRESA:" + item.empresa + " N° Ticket" + item.idTicket + " Documento: " + item.numerodocumento;
                                        errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "ENVIO BAJA", "Se intento verificar más de 20 veces sin exito, se ingreso en Verificacion SUNAT :" + item.numerodocumento);
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                txtresumen_verificacion.Text += "\n  NO SE PROCESO BAJA EMPRESA:" + item.empresa + " Documento: " + item.numerodocumento + "\n" + ex.Message + "\n\n";
                                errorCorreo += string.Format(formatoCorreo, item.numerodocumento, item.empresa, "ENVIO BAJA", "");
                            }
                           

                        }
                        #endregion
                    }
                    db.SaveChanges();
                }
            }
            if (errorCorreo != "")
                CobranzaDAO.sendCorreoDocumentoGenerado(errorCorreo);
        }

        private void enviar_Boletas()
        {
            string errorCorreo = "";
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
                                txtresumenBoletas.Text += "\n\n N° Ticket RESUMEN: " + ticket;

                                new CobranzaDAO().insertarResumen(item, ticket, pathMain, (CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"), "JAB456");

                                txtresumenBoletas.Text += "\n Archivo enviado con exito  EMPRESA:" + empresa;



                                string estado = "98";
                                int count = 0;
                                while (estado.Contains("98"))
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
                                    {
                                        txtresumenBoletas.Text += "\n Resumen en Proceso de Validacion se intentara en 10 segundos EMPRESA:" + empresa;
                                        System.Threading.Thread.Sleep(10000);
                                    }
                                    else
                                    {
                                        openFile((CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"));

                                        txtresumenBoletas.Text += "\n  Resumen con Errores  EMPRESA:" + empresa;
                                        errorCorreo += string.Format(formatoCorreo, item.numeroDocumento, item.empresa, "ENVIO DE BOLETAS", "RESUMEN CON ERRORES");

                                    }
                                    count++;
                                    if (count == 20)
                                    {
                                        txtresumenBoletas.Text += "\n  Se intentaron 20 veces sin exito se cancelo intentar nuevamente EMPRESA:" + empresa + " N° Ticket" + ticket + " Documento: " + item.numeroDocumento;
                                        new CobranzaDAO().UpdateListaResumen(fecha.ToString("dd-MM-yyyy"), empresa.ToString(), (CobranzaDAO.PathDocumentosFacturacion + "\\RESUMEN\\RESPUESTA-" + ticket + ".zip"));
                                        using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                                        {
                                            VerificacionSunat ver = new VerificacionSunat();
                                            ver.idTicket = ticket;
                                            ver.empresa = empresa.ToString();
                                            ver.numerodocumento = item.numeroDocumento;
                                            ver.fecha = fecha;
                                            ver.resultado = "-";
                                            ver.isActivo = true;
                                            ver.codigopaciente = 0;
                                            ver.tipo_envio = "RESUMEN";
                                            ver.pathfile = filename.Replace(".zip", "");
                                            db.VerificacionSunat.Add(ver);
                                            db.SaveChanges();
                                            errorCorreo += string.Format(formatoCorreo, item.numeroDocumento, item.empresa, "ENVIO DE BOLETAS", "Se intento verificar más de 20 veces sin exito, se ingreso en Verificacion SUNAT");
                                        }
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                txtresumenBoletas.Text += "\n\n ********************************************************************\nERROR******************************************************************** \n" +
 ex.Message.ToString() + "\n\n";
                                errorCorreo += string.Format(formatoCorreo, item.numeroDocumento, item.empresa, "ENVIO DE BOLETAS", ex.Message.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        txtresumenBoletas.Text += "\n\n ********************************************************************ERROR******************************************************************** \n" +
 ex.Message.ToString() + "\n\n";

                        errorCorreo += string.Format(formatoCorreo, "-", "-", "ENVIO DE BOLETAS", ex.Message.ToString());
                    }

                    #endregion
                }
            }
            if (errorCorreo != "")
                CobranzaDAO.sendCorreoDocumentoGenerado(errorCorreo);


        }

        public void enviar_facturas()
        {
            string errorCorreo = "";
            var lista = new CobranzaDAO().getFacturasPendientesSendSunat();
            txtresumen.Text += "\n" + DateTime.Now.ToString("dd/MM/yy HH:mm") + " - Se encontraron " + lista.Count() + " factura(s) pendiente(s) de envio.\n";
            if (lista.Count > 0)
            {
                var dao = new Sunat();
                var dao1 = new CobranzaDAO();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (var item in lista)
                    {
                        var doc = db.DOCUMENTO.SingleOrDefault(x => x.numerodocumento == item.documento && x.codigounidad == item.empresa);
                        try
                        {
                            if (item.resultado == "PENDIENTE")
                            {
                                string pathCDR = dao.sendBill(item.empresa, item.paciente, System.IO.Path.GetFileName(item.filename).Split('.')[0]);

                                string msjResutlado = "";
                                if (dao1.verificarCDR(pathCDR, out msjResutlado))
                                {

                                    if (doc != null)
                                    {
                                        item.resultado = "EXITO";
                                        doc.isSendSUNAT = true;
                                    }
                                }
                                else
                                {
                                    item.resultado = "ERROR - " + msjResutlado.Trim();
                                    errorCorreo += string.Format(formatoCorreo, item.documento, item.empresa, "VERIFICACION CDR", msjResutlado.ToString());
                                }
                            }

                            txtresumen.Text += "\n- " + item.documento + "  =>  " + item.resultado + " EMPRESA:" + item.empresa;
                        }
                        catch (Exception ex)
                        {
                            string code = ex.Message.Split('|')[0];

                            if (code.Contains(""))
                            {

                                VerificacionSunat ver = new VerificacionSunat();
                                ver.idTicket = "-";
                                ver.empresa = item.empresa.ToString();
                                ver.numerodocumento = item.documento;
                                ver.fecha = DateTime.Now;
                                ver.resultado = "-";
                                ver.isActivo = true;
                                ver.tipo_envio = "FACTURA";
                                ver.codigopaciente = item.paciente;
                                ver.pathfile = item.filename;
                                db.VerificacionSunat.Add(ver);
                                db.SaveChanges();
                                doc.isSendSUNAT = true;
                                errorCorreo += string.Format(formatoCorreo, item.documento, item.empresa, "VERIFICACION CDR", "Se ingreso a Verificacion de SUNAT");
                            }
                            txtresumen.Text += "\n\n ********************************************************************ERROR******************************************************************** \n" +
ex.Message.ToString() + "\n\n";
                        }
                    }
                    db.SaveChanges();
                }

                txtresumen.Text += "\n\nRESUMEN RESOCENTRO => Total " + lista.Where(x => x.empresa == 1).Count() + " Facturas, Correctas: " + lista.Where(x => x.empresa == 1 && x.resultado == "EXITO").ToList().Count() + " , Pendientes: " + lista.Where(x => x.empresa == 1 && x.resultado == "PENDIENTE").ToList().Count() + " , Correctas: " + lista.Where(x => x.empresa == 1 && x.resultado.Contains("ERROR")).ToList().Count();

                txtresumen.Text += "\n\nRESUMEN EMETAC => Total " + lista.Where(x => x.empresa == 2).Count() + " Facturas, Correctas: " + lista.Where(x => x.empresa == 2 && x.resultado == "EXITO").ToList().Count() + " , Pendientes: " + lista.Where(x => x.empresa == 2 && x.resultado == "PENDIENTE").ToList().Count() + " , Correctas: " + lista.Where(x => x.empresa == 2 && x.resultado.Contains("ERROR")).ToList().Count() + "\n ";
                if (errorCorreo != "")
                    CobranzaDAO.sendCorreoDocumentoGenerado(errorCorreo);

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
            seconds = int.Parse(txtminutos_factura.Value.Value.ToString()) * 60;
        }

        private void txtminutos_verificacion_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            seconds_verificacion = int.Parse(txtminutos_verificacion.Value.ToString()) * 60;
        }

        private void btnCleanfacturas_Click(object sender, RoutedEventArgs e)
        {
            txtresumen.Text = "";
        }

        private void btnCleanBoletas_Click(object sender, RoutedEventArgs e)
        {
            txtresumenBoletas.Text = "";
        }

        private void btnCleanConfirmacion_Click(object sender, RoutedEventArgs e)
        {
            txtresumen_verificacion.Text = "";
        }


    }
}
