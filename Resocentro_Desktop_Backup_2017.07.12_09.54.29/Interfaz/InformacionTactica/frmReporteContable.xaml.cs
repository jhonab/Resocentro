using Microsoft.Win32;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Resocentro_Desktop.Interfaz.InformacionTactica
{
    /// <summary>
    /// Lógica de interacción para frmReporteContable.xaml
    /// </summary>
    public partial class frmReporteContable : Window
    {
        //List<RGVCAFAC_Result> lstContraste;
        //List<RGVCAFAC_Result> lstSedacion;
        //List<RGVCAFAC_Result> lstRepetidas;
        //List<RGVCAFAC_Result> lstPendiente;

        List<RGVCAFAC_Result> lstFactura;
        List<RGVFACTU_Result> lstDetalle;
        List<RGVCAFAC_Result> lstnoConcuerda;
        List<DetalleResumenBoleta> lstCorrelativos;
        List<RGVCAFAC_Result> lstInafectas;

        List<CabeceraDocumentoOracle> lstFacturaOra;
        List<DetalleDocumentoOracle> lstDetalleOra;
        List<CabeceraDocumentoOracle> lstnoConcuerdaOracle;
        List<DetalleResumenBoleta> lstCorrelativosOracle;

        List<DiscordanciaSQLOracle> lstDiscordanciaCabeceraMonto;
        List<DiscordanciaSQLOracle> lstDiscordanciadocumento;

        MySession session;
        public frmReporteContable()
        {
            InitializeComponent();
        }

        private void inicializar()
        {
            //lstContraste = new List<RGVCAFAC_Result>();
            //lstSedacion = new List<RGVCAFAC_Result>();
            //lstPendiente = new List<RGVCAFAC_Result>();

            lstCorrelativos = new List<DetalleResumenBoleta>();
            lstCorrelativosOracle = new List<DetalleResumenBoleta>();
            lstInafectas = new List<RGVCAFAC_Result>();
            lstFactura = new List<RGVCAFAC_Result>();
            lstnoConcuerda = new List<RGVCAFAC_Result>();
            lstnoConcuerdaOracle = new List<CabeceraDocumentoOracle>();
            lstDetalle = new List<RGVFACTU_Result>();
            lstFacturaOra = new List<CabeceraDocumentoOracle>();
            lstDetalleOra = new List<DetalleDocumentoOracle>();
            lstDiscordanciaCabeceraMonto = new List<DiscordanciaSQLOracle>();
            lstDiscordanciadocumento = new List<DiscordanciaSQLOracle>();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboEmpresa.ItemsSource = new UtilDAO().getEmpresa();
            cboEmpresa.SelectedValuePath = "codigounidad";
            cboEmpresa.DisplayMemberPath = "nombre";
            dtpInicio.SelectedDate = DateTime.Now;
            dtpFin.SelectedDate = DateTime.Now;
            cboEmpresa.SelectedIndex = 0;
            inicializar();
        }


        public async Task<bool> getCabeceraSQL()
        {
            int ncon = 0;
            double totcabS = 0, totdetS = 0, cabigv = 0, detigv = 0, cabtot = 0, dettot = 0;
            lstFactura = await new InfoTacticaDAO().getRGVCAFAC(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());


            lstDetalle = await new InfoTacticaDAO().getRGVFACTU(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());

            //obtenemos lista de correlativos
            #region obtenemos lista de correlativos
            foreach (var tipodoc in lstFactura.Select(x => x.TDORGV).Distinct().AsParallel())
            {
                foreach (var serie in lstFactura.Where(x => x.TDORGV == tipodoc).Select(x => x.SERIE.Substring(1)).Distinct().AsParallel())
                {
                    DetalleResumenBoleta detCabecera = new DetalleResumenBoleta();
                    detCabecera.tipodocumento = int.Parse(tipodoc);
                    detCabecera.serie = serie;

                    int max = int.MaxValue, count = 0;
                    foreach (var d in lstFactura.Where(x => x.TDORGV == tipodoc && x.SERIE.Substring(1) == serie).OrderBy(x => x.CORRELATIVO).ToList())
                    {
                        if (count != 0)
                        {
                            if (d.CORRELATIVO - 1 != count && d.CORRELATIVO != count)
                            {
                                count = 0;
                                max = int.MaxValue;
                                lstCorrelativos.Add(detCabecera);
                                detCabecera = new DetalleResumenBoleta();
                                detCabecera.tipodocumento = int.Parse(tipodoc);
                                detCabecera.serie = serie;
                            }
                        }

                        if (d.CORRELATIVO < max)
                        {
                            max = d.CORRELATIVO;
                            detCabecera.inicioRango = d.CORRELATIVO.ToString();
                        }

                        detCabecera.finRango = d.CORRELATIVO.ToString();


                        count = d.CORRELATIVO;
                    }
                    lstCorrelativos.Add(detCabecera);
                }
            }
            #endregion
            #region d
            foreach (var item in lstFactura)
            {
                //obtenemos el detalle de la factura
                var det = lstDetalle.Where(x => x.CODEAUX == item.CODEAUX && x.PACEST == item.PACEST).ToList();

                if (item.N_EXAMEN != "ANULADO")
                {
                    //Sumamos los totales de Cabeceras
                    totcabS += Convert.ToDouble(item.VVTRGVS);
                    cabigv += Convert.ToDouble(item.IGVRGVS);
                    cabtot += Convert.ToDouble(item.TOTRGVS);
                    //Sumamos los totales de DETALEE
                    totdetS += Convert.ToDouble(det.Sum(x => x.VVTRGVS));
                    detigv += Convert.ToDouble(det.Sum(x => x.IGVRGVS));
                    dettot += Convert.ToDouble(det.Sum(x => x.TOTRGVS));
                }

                if (det.Where(x => x.tipoIGV.Contains("INAFECTO")).ToList().Count() > 0)
                    lstInafectas.Add(item);

                //Buscamos Facturas que no concuerden con el detalle

                if ((Convert.ToDouble(item.TOTRGVS) - Convert.ToDouble(det.Sum(x => x.TOTRGVS))) > 0.2)
                {
                    ncon++;
                    lstnoConcuerda.Add(item);
                }


                //Buscamos Facturas repetidas                   
                /* if (!nun_fac.Exists(x => x == item.CODEAUX))
                     nun_fac.Add(item.CODEAUX);
                 else
                 {
                     rep++;
                     lstRepetidas.AddRange(lstFactura.Where(x => x.CODEAUX == item.CODEAUX).ToList());
                 }
                 */

                //Asignamos Medicos y promotores
                /* int correcto = 0;
                 if (int.TryParse(item.N_EXAMEN, out correcto))
                 {
                     var lista = new InfoTacticaDAO().getMedicopromotorAtencion(item.N_EXAMEN);
                     if (lista.Count > 0)
                     {
                         item.Medico = lista[0];
                         item.Promotor = lista[1];
                     }
                     else
                     {
                         item.Medico = "-";
                         item.Promotor = "-";
                     }
                 }
                 else
                 {
                     item.Medico = "-";
                     item.Promotor = "-";
                 }

                 foreach (var item_det in det)
                 {
                     item_det.Medico = item.Medico;
                     item_det.Promotor = item.Promotor;
                     //BUSCANDO FACTURAS CON SEDACION
                     if (item_det.ESTUDIO.Contains("Sedacion"))
                         if (lstSedacion.Where(x => x.CODEAUX == item.CODEAUX).SingleOrDefault() == null)
                             lstSedacion.Add(item);

                     //BUSCANDO FACTURAS CON CONTRASTE
                     if (item_det.ESTUDIO.Contains("Contraste"))
                         if (lstContraste.Where(x => x.CODEAUX == item.CODEAUX).SingleOrDefault() == null)
                             lstContraste.Add(item);
                          
                 }
                 */

            }
            #endregion
            //lblerrorfactu.Content = "Fact. Repetidas: " + rep;
            lblerrordeta.Content = "Fact. No Concue: " + ncon;
            //lblerrorpend.Content = "Fact. Pendiente: " + pen;

            lbltotcabs.Content = "Sub. Cab.: S/. " + totcabS.ToString("0,0,0.00");
            lbltotdets.Content = "Sub. Det.: S/. " + totdetS.ToString("0,0,0.00");

            lblcabigv.Content = "IGV. Cab.: S/. " + cabigv.ToString("0,0,0.00");
            lbldetigv.Content = "IGV. Det.: S/. " + detigv.ToString("0,0,0.00");

            lblcabtot.Content = "Tot. Cab.: S/. " + cabtot.ToString("0,0,0.00");
            lbldettot.Content = "Tot. Det.: S/. " + dettot.ToString("0,0,0.00");

            grid_Factura.ItemsSource = lstFactura;
            grid_correlatividad.ItemsSource = lstCorrelativos;
            grid_Inafectas.ItemsSource = lstInafectas;
            grid_NConcuerdan.ItemsSource = lstnoConcuerda;
            return true;
        }


        public async Task<bool> getCabeceraOracle()
        {
            int ncon = 0;
            double totcabSOracle = 0, totdetSOracle = 0, cabigvOracle = 0, detigvOracle = 0, cabtotOracle = 0, dettotOracle = 0;

            lstFacturaOra = await new InfoTacticaDAO().getCabeceraOracle(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());

            lstDetalleOra = await new InfoTacticaDAO().getDetalleOracle(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());

            #region obtenemos lista de correlativos ORACLE
            foreach (var tipodoc in lstFacturaOra.Select(x => x.tipodocumento).Distinct().AsParallel())
            {
                foreach (var serie in lstFacturaOra.Where(x => x.tipodocumento == tipodoc).Select(x => x.serie.Substring(1)).Distinct().AsParallel())
                {
                    DetalleResumenBoleta detCabecera = new DetalleResumenBoleta();
                    detCabecera.tipodocumento = int.Parse(tipodoc);
                    detCabecera.serie = serie;
                    
                    int max = int.MaxValue, count = 0;
                    foreach (var d in lstFacturaOra.Where(x => x.tipodocumento == tipodoc && x.serie.Substring(1) == serie).OrderBy(x => x.correlativo).ToList())
                    {
                        if (count != 0)
                        {
                            if (d.correlativo - 1 != count && d.correlativo != count)
                            {
                                count = 0;
                                max = int.MaxValue;
                                lstCorrelativosOracle.Add(detCabecera);
                                detCabecera = new DetalleResumenBoleta();
                                detCabecera.tipodocumento = int.Parse(tipodoc);
                                detCabecera.serie = serie;
                            }
                        }

                        if (d.correlativo < max)
                        {
                            max = d.correlativo;
                            detCabecera.inicioRango = d.correlativo.ToString();
                        }

                        detCabecera.finRango = d.correlativo.ToString();
                        count = d.correlativo;
                    }
                    lstCorrelativosOracle.Add(detCabecera);
                }
            }
            #endregion
            foreach (var item in lstFacturaOra)
            {

                //obtenemos el detalle de la factura
                var det = lstDetalleOra.Where(x => x.num_doc == item.num_doc).ToList();

                if (item.estado_string != "ANULADO")
                {
                    //Sumamos los totales de Cabeceras
                    totcabSOracle += Convert.ToDouble(item.subtotal);
                    cabigvOracle += Convert.ToDouble(item.igv);
                    cabtotOracle += Convert.ToDouble(item.total);
                    //Sumamos los totales de DETALEE
                    totdetSOracle += Convert.ToDouble(det.Sum(x => x.precio));
                    detigvOracle += Convert.ToDouble(det.Sum(x =>x.total-x.precio));
                    dettotOracle += Convert.ToDouble(det.Sum(x => x.total));
                }
                //Buscamos Facturas que no concuerden con el detalle
                if ((Convert.ToDouble(item.total) - det.Sum(x => x.total)) > 0.2)
                {
                    ncon++;
                    lstnoConcuerdaOracle.Add(item);
                }

            }
            lblerrordetaoracle.Content = "Fact. No Concue: " + ncon;
            //lblerrorpend.Content = "Fact. Pendiente: " + pen;

            lbltotcabsoracle.Content = "Sub. Cab.: S/. " + totcabSOracle.ToString("0,0,0.00");
            lbltotdetsoracle.Content = "Sub. Det.: S/. " + totdetSOracle.ToString("0,0,0.00");

            lblcabigvoracle.Content = "IGV. Cab.: S/. " + cabigvOracle.ToString("0,0,0.00");
            lbldetigvoracle.Content = "IGV. Det.: S/. " + detigvOracle.ToString("0,0,0.00");

            lblcabtotoracle.Content = "Tot. Cab.: S/. " + cabtotOracle.ToString("0,0,0.00");
            lbldettotoracle.Content = "Tot. Det.: S/. " + dettotOracle.ToString("0,0,0.00");


            grid_OraCabecera.ItemsSource = lstFacturaOra;
            grid_correlatividadoracle.ItemsSource = lstCorrelativosOracle;
            grid_NoConcuerdaOracle.ItemsSource = lstnoConcuerdaOracle;

            return true;
        }
        private async void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            inicializar();
            try
            {
                pbStatus.Value = 0;
                lblestado.Content = "Ejecutando Consulta...";
                pbStatus.IsIndeterminate = true;

                await Task.WhenAll(getCabeceraSQL(), getCabeceraOracle()).ContinueWith(task =>
                {

                    //Comparamos SQL ORACLE
                    foreach (var item in lstFactura)
                    {
                        var foracle = lstFacturaOra.SingleOrDefault(x => x.num_doc == item.SERIE + "-" + item.CORRELATIVO);
                        if (foracle == null)
                            lstDiscordanciadocumento.Add(new DiscordanciaSQLOracle
                            {
                                NDOC = item.NDORGV,
                                MONTO_SQL = item.TOTRGVS,
                                MONTO_ORACLE = 0,
                                ERROR = "Existe solo SQL"
                            });
                        else
                        {
                            var diferencia = Convert.ToDouble(item.TOTRGVS.Value) - foracle.total;
                            string error = "";
                            if (diferencia > 0.2)
                                error = "EXCEDE SQL";
                            else if (diferencia < -0.2)
                                error = "EXCEDE ORACLE";
                            else error = "";
                            if (error != "")
                            {
                                lstDiscordanciadocumento.Add(new DiscordanciaSQLOracle
                                {
                                    NDOC = item.NDORGV,
                                    MONTO_SQL = item.TOTRGVS,
                                    MONTO_ORACLE = foracle.total,
                                    ERROR = error
                                });
                            }
                        }
                    }

                    foreach (var item in lstFacturaOra)
                    {
                        var fsql = lstFactura.SingleOrDefault(x => (x.SERIE + "-" + x.CORRELATIVO) == item.num_doc);
                        if (fsql == null)
                            lstDiscordanciadocumento.Add(new DiscordanciaSQLOracle
                            {
                                NDOC = item.num_doc,
                                MONTO_SQL = 0,
                                MONTO_ORACLE = item.total,
                                ERROR = "Existe solo ORACLE"
                            });

                    }

                });


                grid_DiscordanciaMontos.ItemsSource = lstDiscordanciaCabeceraMonto;
                grid_DiscordanciaDocumento.ItemsSource = lstDiscordanciadocumento;

                grid_Factura.CalculateAggregates();
                grid_correlatividad.CalculateAggregates();
                grid_Detalle.CalculateAggregates();
                grid_Inafectas.CalculateAggregates();
                grid_OraCabecera.CalculateAggregates();
                grid_correlatividadoracle.CalculateAggregates();
                grid_DiscordanciaMontos.CalculateAggregates();
                grid_OraDetalle.CalculateAggregates();
                grid_NoConcuerdaOracle.CalculateAggregates();
                grid_DiscordanciaDocumento.CalculateAggregates();
                grid_NConcuerdan.CalculateAggregates();
                grid_DetalleError.CalculateAggregates();

                lblestado.Content = "Terminado.";
                pbStatus.IsIndeterminate = false;
            }
            catch (Exception ex)
            {
                lblestado.Content = "Ocurrio errores en el proceso.";
                pbStatus.IsIndeterminate = false;
                MessageBox.Show(ex.Message);
            }

        }

        private void abrirDetalle()
        {
            frmDetalleReporteContable gui = new frmDetalleReporteContable();
            gui.Show();
            gui.grid_Detalle_fac.ItemsSource = lstDetalle;
        }

        private void grid_Factura_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)e.Row.DataContext;
            if (item != null)
            {
                grid_Detalle.ItemsSource = lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).ToList();
                lbldetalle_doc.Content = "Tipo Doc.:  " + item.TDORGV + "   N° Doc.:  " + item.NDORGV + "   Total S/.: " + item.TOTRGVS + "   Total $.: " + item.TOTRGVD + "   Diferencia S/.: " + (item.TOTRGVS - lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).Sum(x => x.TOTRGVS));
            }
            else
                lbldetalle_doc.Content = "Detalle Documento";

        }
        private void grid_FacturaError_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)e.Row.DataContext;
            if (item != null)
            {
                grid_DetalleError.ItemsSource = lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).ToList();
                lbldetalle_docError.Content = "Tipo Doc.:  " + item.TDORGV + "   N° Doc.:  " + item.NDORGV + "   Total S/.: " + item.TOTRGVS + "   Total $.: " + item.TOTRGVD + "   Diferencia S/.: " + (item.TOTRGVS - lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).Sum(x => x.TOTRGVS));
            }
            else
                lbldetalle_doc.Content = "Detalle Documento";

        }
        private void grid_Inafecta_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)e.Row.DataContext;
            if (item != null)
            {
                grid_Detalle.ItemsSource = lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).ToList();
                lbldetalle_doc.Content = "Tipo Doc.:  " + item.TDORGV + "   N° Doc.:  " + item.NDORGV + "   Total S/.: " + item.TOTRGVS + "   Total $.: " + item.TOTRGVD + "   Diferencia S/.: " + (item.TOTRGVS - lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).Sum(x => x.TOTRGVS));
            }
            else
                lbldetalle_doc.Content = "Detalle Documento";
        }
        private void Info_FAC_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)this.grid_Factura.SelectedItem;

            if (item != null)
            {
                cargarInfo(item.N_EXAMEN);

            }
            else
            {
                MessageBox.Show("Seleccione una Fila", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void cargarInfo(string atencion)
        {
            frmHistoriaPaciente gui = new frmHistoriaPaciente();
            gui.cargarGUI(session);
            gui.Show();
            gui.buscarPaciente(atencion, 3);
        }

        private void btnDetaller_Click(object sender, RoutedEventArgs e)
        {
            if (lstDetalle != null)
                if (lstDetalle.Count > 0)
                    abrirDetalle();
                else
                    MessageBox.Show("Busque un rango de facturas");
        }

        private void btnError_Click(object sender, RoutedEventArgs e)
        {
            abrirErrores();
        }

        private void abrirErrores()
        {
            frmErrorReporteContable gui = new frmErrorReporteContable();
            gui.Show();
            gui.cargarGUI(lstDetalle, new List<RGVCAFAC_Result>(), lstnoConcuerda, new List<RGVCAFAC_Result>());
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string extension = "xls";
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (*.{0})|*.{0}|All files(*.*)|*.*", extension, "Excel"),
                FilterIndex = 1,
                FileName = "Cabecera.xls"
            };

            if (dialog.ShowDialog() == true)
            {

                using (Stream stream = dialog.OpenFile())
                {
                    grid_Factura.Export(stream,
                        new GridViewExportOptions()
                        {
                            Format = ExportFormat.ExcelML,
                            ShowColumnHeaders = true,
                            ShowColumnFooters = true,
                            ShowGroupFooters = true,

                        });
                }
            }
            dialog.FileName = "Detalle.xls";
            if (dialog.ShowDialog() == true)
            {
                grid_Detalle.ItemsSource = lstDetalle;
                using (Stream stream = dialog.OpenFile())
                {
                    grid_Detalle.Export(stream,
                       new GridViewExportOptions()
                       {
                           Format = ExportFormat.ExcelML,
                           ShowColumnHeaders = true,
                           ShowColumnFooters = true,
                           ShowGroupFooters = false,

                       });
                }
                grid_Detalle.ItemsSource = null;
                MessageBox.Show("Exportado");
            }
        }
        
        private void MenuItemExportar1_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_Factura, true, false, false);
        }

        private void MenuItemExportar2_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_Detalle, true, false, false);
        }

        private void MenuItemExportar3_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_correlatividad, true, false, false);
        }

        private void MenuItemExportar4_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_Inafectas, true, false, false);
        }
        private void MenuItemExportardiscoDoc_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_DiscordanciaDocumento, true, false, false);
        }

        private void MenuItemExportarDiscoMonto_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_DiscordanciaMontos, true, false, false);
        }
        private void grid_OraCabecera_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            CabeceraDocumentoOracle item = (CabeceraDocumentoOracle)e.Row.DataContext;
            if (item != null)
            {
                grid_OraDetalle.ItemsSource = lstDetalleOra.Where(x => x.num_doc == item.num_doc).ToList();
                lbldetalleoracle_doc.Content = "Tipo Doc.:  " + item.tipodocumento_string + "   N° Doc.:  " + item.num_doc + "   Total S/.: " + item.total + "   Total $.: " + item.total_dolares + "   Diferencia S/.: " + Math.Round((item.total - lstDetalleOra.Where(x => x.num_doc == item.num_doc).Sum(x => x.total)), 2);
            }
            else
                lbldetalle_doc.Content = "Detalle Documento";
        }

        private void MenuItemExportarDetalleaOra_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_OraDetalle, true, false, false);
        }

        private void MenuItemExportarCabeceraOra_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_OraCabecera, true, false, false);
        }

        private void MenuItemExportarConcuerdaOracleOra_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            new Tool().exportGrid(grid_NoConcuerdaOracle, true, false, false);
        }









    }
}
