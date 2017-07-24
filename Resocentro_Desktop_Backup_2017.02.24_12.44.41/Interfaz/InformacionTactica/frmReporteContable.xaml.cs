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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.InformacionTactica
{
    /// <summary>
    /// Lógica de interacción para frmReporteContable.xaml
    /// </summary>
    public partial class frmReporteContable : Window
    {
        List<RGVCAFAC_Result> lstFactura;
        List<RGVCAFAC_Result> lstContraste;
        List<RGVCAFAC_Result> lstSedacion;
        List<RGVCAFAC_Result> lstRepetidas;
        List<RGVCAFAC_Result> lstnoConcuerda;
        List<RGVCAFAC_Result> lstPendiente;
        List<RGVFACTU_Result> lstDetalle;
        MySession session;
        public frmReporteContable()
        {
            InitializeComponent();
        }

        private void inicializar()
        {
            lstContraste = new List<RGVCAFAC_Result>();
            lstSedacion = new List<RGVCAFAC_Result>();
            lstFactura = new List<RGVCAFAC_Result>();
            lstRepetidas = new List<RGVCAFAC_Result>();
            lstnoConcuerda = new List<RGVCAFAC_Result>();
            lstPendiente = new List<RGVCAFAC_Result>();
            lstDetalle = new List<RGVFACTU_Result>();
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            inicializar();
            try
            {

                lstFactura =  new InfoTacticaDAO().getRGVCAFAC(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());
                lstDetalle = new InfoTacticaDAO().getRGVFACTU(dtpInicio.SelectedDate.Value.ToShortDateString(), dtpFin.SelectedDate.Value.ToShortDateString(), cboEmpresa.SelectedValue.ToString());
                List<string> nun_fac = new List<string>();
                int rep = 0;
                int ncon = 0;
                int pen = 0;
                double totcabS = 0, totdetS = 0, cabigv = 0, detigv = 0, cabtot = 0, dettot = 0;
                //Buscamos Facturas pendientes de pago
                var lstAtencion = lstFactura.Where(x => x.N_EXAMEN != "ANULADO" && x.N_EXAMEN != "ESPECIAL" && x.N_EXAMEN != "ESPECIAL II").GroupBy(x => x.N_EXAMEN).Select(x => new { nexam = x.Key, cobertura = x.Sum(z => z.PDSRGV) }).ToList();
                foreach (var item in lstAtencion)
                {
                    if (!(item.cobertura > 99))
                    {
                        pen++;
                        lstPendiente.AddRange(lstFactura.Where(x => x.N_EXAMEN == item.nexam).ToList());
                    }
                }

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
                   /* //ACTUALIZAMOS SUCURSAL Y UNIDAD NEGOCIO
                    var sucur = new InfoTacticaDAO().getSucursal(item.CODUNI, item.CODSUC);
                    if (sucur != null)
                    {
                        item.CODUNI = sucur.descripcion;
                        item.CODSUC = sucur.ShortDesc;
                    }*/

                    //Buscamos Facturas que no concuerden con el detalle
                    if ((Convert.ToDouble(item.TOTRGVS) - dettot) > 0.2)
                    {
                        ncon++;
                        lstnoConcuerda.Add(item);
                    }


                    //Buscamos Facturas repetidas                   
                    if (!nun_fac.Exists(x => x == item.CODEAUX))
                        nun_fac.Add(item.CODEAUX);
                    else
                    {
                        rep++;
                        lstRepetidas.AddRange(lstFactura.Where(x => x.CODEAUX == item.CODEAUX).ToList());
                    }


                    //Asignamos Medicos y promotores
                    int correcto = 0;
                    if (int.TryParse(item.N_EXAMEN,out correcto))
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


                }
                lblerrorfactu.Content = "Fact. Repetidas: " + rep;
                lblerrordeta.Content = "Fact. No Concue: " + ncon;
                lblerrorpend.Content = "Fact. Pendiente: " + pen;

                lbltotcabs.Content = "Sub. Cab.: S/. " + totcabS.ToString("0,0,0.00");
                lbltotdets.Content = "Sub. Det.: S/. " + totdetS.ToString("0,0,0.00");

                lblcabigv.Content = "IGV. Cab.: S/. " + cabigv.ToString("0,0,0.00");
                lbldetigv.Content = "IGV. Det.: S/. " + detigv.ToString("0,0,0.00");

                lblcabtot.Content = "Tot. Cab.: S/. " + cabtot.ToString("0,0,0.00");
                lbldettot.Content = "Tot. Det.: S/. " + dettot.ToString("0,0,0.00");


                grid_Factura.ItemsSource = lstFactura;
                grid_Sedacion.ItemsSource = lstSedacion;
                grid_Contraste.ItemsSource = lstContraste;
                MessageBox.Show("Terminado");

            }
            catch (Exception ex)
            {
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

        private void info_Sed_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)this.grid_Sedacion.SelectedItem;

            if (item != null)
            {
                cargarInfo(item.N_EXAMEN);
            }
            else
            {
                MessageBox.Show("Seleccione una Fila", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void info_Con_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)this.grid_Contraste.SelectedItem;

            if (item != null)
            {
                int nexa = 0;
                if (int.TryParse(item.N_EXAMEN, out nexa))

                    cargarInfo(item.N_EXAMEN);
                else
                    MessageBox.Show("Seleccione una fila con número de atencion", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
            gui.cargarGUI(lstDetalle, lstRepetidas, lstnoConcuerda, lstPendiente);
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
    }
}
