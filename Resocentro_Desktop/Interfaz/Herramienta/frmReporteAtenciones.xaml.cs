using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.Herramienta
{
    /// <summary>
    /// Lógica de interacción para frmListaAtenciones.xaml
    /// </summary>
    public partial class frmReporteAtenciones : Window
    {
        MySession session;
        private List<ReporteAtencionesMensuales> listaResultado = new List<ReporteAtencionesMensuales>();
        public List<ReporteAtencionesMensuales> ListaResultado
        {
            get { return listaResultado; }
            set { listaResultado = value; }
        }
        public frmReporteAtenciones()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboAnio.ItemsSource = new UtilDAO().getAño(5);
            cboAnio.DisplayMemberPath = "nombre";
            cboAnio.SelectedValuePath = "codigo";
            cboAnio.SelectedValue = DateTime.Now.Year;

            var empresas = new UtilDAO().getEmpresa();
            cbosucursal.ItemsSource = empresas;
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;

            cbosucursal1.ItemsSource = empresas;
            cbosucursal1.SelectedValuePath = "codigounidad";
            cbosucursal1.DisplayMemberPath = "nombre";
            cbosucursal1.SelectedIndex = 0;

            cbosucursaldoc.ItemsSource = empresas;
            cbosucursaldoc.SelectedValuePath = "codigounidad";
            cbosucursaldoc.DisplayMemberPath = "nombre";
            cbosucursaldoc.SelectedIndex = 0;

            cbosucursaldocFac.ItemsSource = empresas;
            cbosucursaldocFac.SelectedValuePath = "codigounidad";
            cbosucursaldocFac.DisplayMemberPath = "nombre";
            cbosucursaldocFac.SelectedIndex = 0;

            cbosucursalCortesia.ItemsSource = empresas;
            cbosucursalCortesia.SelectedValuePath = "codigounidad";
            cbosucursalCortesia.DisplayMemberPath = "nombre";
            cbosucursalCortesia.SelectedIndex = 0;

            if (session.roles.Contains("60"))
                radtab2.Visibility = Visibility.Visible;
            else
                radtab2.Visibility = Visibility.Collapsed;


        }

        private async void BtnListaMensual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gridResultado.IsBusy = true;
                gridResultado.ItemsSource = await new AdministracionDAO().getReportAtenciones(1, cboMes.SelectedIndex, int.Parse(cboAnio.SelectedValue.ToString()), int.Parse(cbosucursal.SelectedValue.ToString()), DateTime.Now, DateTime.Now);
                gridResultado.IsBusy = false;
                gridResultado.CalculateAggregates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridResultado.IsBusy = false;
            }

        }

        private async void rdpFechaRango_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {

                    gridResultado.IsBusy = true;
                    //gridResultado.ItemsSource = await new AdministracionDAO().getReportAtenciones(2, 1, 1, int.Parse(cbosucursal.SelectedValue.ToString()), rdpInicio.SelectedDate.Value, rdpFin.SelectedDate.Value);
                    ListaResultado = await new AdministracionDAO().getReportAtenciones(2, 1, 1, int.Parse(cbosucursal.SelectedValue.ToString()), rdpInicio.SelectedDate.Value, rdpFin.SelectedDate.Value);
                    this.DataContext = new { ListaResultado = ListaResultado };
                    gridResultado.IsBusy = false;
                    //  worker.RunWorkerAsync();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridResultado.IsBusy = false;
            }
        }




        private void MenuItemExportar_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                RadMenuItem item = (RadMenuItem)sender;
                if (item.CommandParameter.ToString() == "atencion")
                    new Tool().exportGrid(gridResultado, true, false, false);
                else if (item.CommandParameter.ToString() == "documento")
                    new Tool().exportGrid(gridDocumentos, true, false, false);
                else { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItemHistoria_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                var item = (ReporteAtencionesMensuales)this.gridResultado.SelectedItem;
                if (item != null)
                {
                    frmHistoriaPaciente gui = new frmHistoriaPaciente();
                    gui.cargarGUI(session);
                    gui.Show();
                    await gui.buscarPaciente(item.Codigo.ToString(), 1);
                }
                else
                {
                    var item2 = (ReporteAtencionesMensuales)this.gridResultado.SelectedItem;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void rdpFechaRangodoc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentos(int.Parse(cbosucursaldoc.SelectedValue.ToString()), rdpIniciodoc.SelectedDate.Value, rdpFindoc.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async void rdpFechaRangodocFAC_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentosFactura(int.Parse(cbosucursaldocFac.SelectedValue.ToString()), rdpIniciodocFAC.SelectedDate.Value, rdpFindocFAC.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async void rdpFechaRangoCortesia_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    gridDocumentos.IsBusy = true;
                    gridDocumentos.ItemsSource = await new AdministracionDAO().getReportDocumentosCortesias(int.Parse(cbosucursalCortesia.SelectedValue.ToString()), rdpInicioCortesia.SelectedDate.Value, rdpFinCortesia.SelectedDate.Value);
                    gridDocumentos.IsBusy = false;
                    gridDocumentos.CalculateAggregates();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                gridDocumentos.IsBusy = false;
            }
        }

        private async void gridResultado_LoadingRowDetails(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            try
            {
                ReporteAtencionesMensuales item = (ReporteAtencionesMensuales)e.Row.DataContext;

                item.isBusy = true;
                await getDetalleInfo(item).ContinueWith((result) =>
                {
                    item = result.Result;
                    item.Datos_paciente = new PacienteDAO().getPaciente(item.Codigopaciente);



                    item.Datos_adquisicion.estudio = item.Codigo + " - " + item.Estudio;
                    item.List_proceso = new List<Procesos_Examen>();
                    item.List_proceso.Add(new Procesos_Examen()
                    {
                        Proceso = "CITA",
                        Responsable = item.Datos_cita.Usuario_registra,
                        Num_ref = item.Numerocita.ToString(),
                        Incio = item.Datos_cita.Fecha_registro
                    });
                    item.List_proceso.Add(new Procesos_Examen()
                    {
                        Proceso = "ADMISIÓN",
                        Responsable = item.Datos_admision.Usuario,
                        Num_ref = item.Datos_admision.Numero_admision.ToString(),
                        Incio = item.Datos_admision.Fecha_admision
                    });
                    if (item.Datos_adquisicion.encuestador != "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "ENCUESTA",
                            Responsable = item.Datos_adquisicion.encuestador,
                            Num_ref = item.Codigo.ToString(),
                            Incio = item.Datos_adquisicion.ini_encuestador,
                            Fin = item.Datos_adquisicion.fin_encuestador
                        });
                    if (item.Datos_adquisicion.tecnologo != "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "ADQUISICIÓN",
                            Responsable = item.Datos_adquisicion.tecnologo,
                            Num_ref = item.Codigo.ToString(),
                            Incio = item.Datos_adquisicion.ini_tecnologo,
                            Fin = item.Datos_adquisicion.fin_tecnologo
                        });
                    if (item.Datos_adquisicion.supervisor != "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "SUPERVISOR",
                            Responsable = item.Datos_adquisicion.supervisor,
                            Num_ref = item.Codigo.ToString(),
                            Incio = item.Datos_adquisicion.ini_supervisor,
                            Fin = item.Datos_adquisicion.fin_supervisor
                        });
                    if (item.Datos_adquisicion.informante != "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "INFORMANTE",
                            Responsable = item.Datos_adquisicion.informante,
                            Num_ref = item.Codigo.ToString(),
                            Fin = item.Datos_adquisicion.fin_informante
                        });
                    if (item.Datos_adquisicion.validador != "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "VALIDADOR",
                            Responsable = item.Datos_adquisicion.validador,
                            Num_ref = item.Codigo.ToString(),
                            Fin = item.Datos_adquisicion.fin_validador
                        });
                    if (item.Datos_adquisicion.impresion!= "")
                        item.List_proceso.Add(new Procesos_Examen()
                        {
                            Proceso = "IMPRESION",
                            Responsable = item.Datos_adquisicion.impresion,
                            Num_ref = item.Codigo.ToString(),
                            Fin = item.Datos_adquisicion.fin_impresion
                        });

                    item.List_insumos = new List<DetalleHCAdquision_Insumo>();
                    item.List_insumos.AddRange(item.Datos_adquisicion.detalleAdquisicion.insumoEnfermera);
                    item.List_insumos.AddRange(item.Datos_adquisicion.detalleAdquisicion.insumoSedacion);

                    TimeSpan ts;
                    if (item.Datos_adquisicion.fin_validador != null)
                    {
                        ts = Convert.ToDateTime(item.Datos_adquisicion.fin_validador)-item.Fecha;
                        item.Tiempo_final = ts.TotalHours.ToString("###.0#");
                    }
                    else
                    {
                        ts = DateTime.Now - item.Fecha;
                        item.Tiempo_final = ts.TotalHours.ToString("###.0#");
                    }

                    //gridResultado.Items.Refresh();


                });
                item.isBusy = false;

            }
            catch (Exception)
            {

                //throw;
            }


        }

        private async Task<ReporteAtencionesMensuales> getDetalleInfo(ReporteAtencionesMensuales item)
        {
            try
            {
                var dao = new AdministracionDAO();
                item = await dao.getDetalleAtencion(item);
                item.Datos_adquisicion.detalleAdquisicion = new DetalleHCAdquisicion();
                item.Datos_adquisicion.detalleAdquisicion.insumoEnfermera = new List<DetalleHCAdquision_Insumo>();
                item.Datos_adquisicion.detalleAdquisicion.insumoSedacion = new List<DetalleHCAdquision_Insumo>();
                item.Datos_adquisicion.detalleAdquisicion = await new EAtencionDAO().getDetalleAdquisicion(item.Codigo);
                /*item.datos_cita = dao.getInfoCita(item.numerocita).Result;
                item.datos_carta = dao.getInfoCarta(item.datos_cita.codigocarta, item.datos_paciente.codigopaciente.ToString()).Result;
                item.datos_admision = dao.getInfoAdmision(item.numerocita).Result;
                item.Datos_adquisicion = dao.getInfoAdquisicion(item.codigo).Result;*/

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            return item;
        }








    }
}
