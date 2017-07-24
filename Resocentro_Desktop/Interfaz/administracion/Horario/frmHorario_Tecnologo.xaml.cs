using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;
namespace Resocentro_Desktop.Interfaz.administracion.Horario
{
    /// <summary>
    /// Lógica de interacción para frmHorario_Tecnologo.xaml
    /// </summary>
    public partial class frmHorario_Tecnologo : Window
    {
        MySession session;
        ObservableCollection<ResourceType> listaPersonal;
        List<AsignacionHorarioTecnologo> listaturnos;
        ObservableCollection<Appointment> appointments;
        ObservableCollection<Category> Categories;
        int tipoPermiso = 0;
        public frmHorario_Tecnologo()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session, Tipo_Permiso tipo)
        {
            this.session = session;
            tipoPermiso = (int)tipo;
            dtpFecha.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            cboempresa.ItemsSource = new UtilDAO().getEmpresa();
            cboempresa.SelectedValuePath = "codigounidad";
            cboempresa.DisplayMemberPath = "nombre";
            cboempresa.SelectedIndex = 0;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                listaPersonal = new ObservableCollection<ResourceType>();
                ObservableCollection<Resource> personal = new ObservableCollection<Resource>();
                switch (tipo)
                {
                    case Tipo_Permiso.Horario_Tecnologo:
                        this.Title = "Horario de Tecnólogos";
                        foreach (var x in db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 7 && x.bloqueado == false).OrderBy(x => x.ShortName).ToList())
                        {
                            personal.Add(new Resource(x.ShortName + " - " + x.codigousuario));
                        }
                        //"Tecnologo"
                        var _typeTecnologo = new ResourceType("Tecnologo");
                        _typeTecnologo.Resources.AddRange(personal);
                        listaPersonal.Add(_typeTecnologo);
                        break;
                    case Tipo_Permiso.Horario_Encuestador:
                        this.Title = "Horario de Encuestador";
                        foreach (var x in db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 7 && x.bloqueado == false).OrderBy(x => x.ShortName).ToList())
                        {
                            personal.Add(new Resource(x.ShortName + " - " + x.codigousuario));
                        }
                        //"Tecnologo"
                        var _typeEncuestador = new ResourceType("Encuestador");
                        _typeEncuestador.Resources.AddRange(personal);
                        listaPersonal.Add(_typeEncuestador);
                        break;
                    case Tipo_Permiso.Horario_Supervisor:
                        this.Title = "Horario de Supervisor";
                        foreach (var x in db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 7 && x.bloqueado == false).OrderBy(x => x.ShortName).ToList())
                        {
                            personal.Add(new Resource(x.ShortName + " - " + x.codigousuario));
                        }
                        //"Tecnologo"
                        var _typeSupervisor = new ResourceType("Supervisor");
                        _typeSupervisor.Resources.AddRange(personal);
                        listaPersonal.Add(_typeSupervisor);
                        break;
                    case Tipo_Permiso.Horario_Enfermera:
                        this.Title = "Horario de Enfermera";
                        foreach (var x in db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 7 && x.bloqueado == false).OrderBy(x => x.ShortName).ToList())
                        {
                            personal.Add(new Resource(x.ShortName + " - " + x.codigousuario));
                        }
                        //"Tecnologo"
                        var _typeEnfermera = new ResourceType("Enfermera");
                        _typeEnfermera.Resources.AddRange(personal);
                        listaPersonal.Add(_typeEnfermera);
                        break;
                    case Tipo_Permiso.Horario_Postprocesado:
                        this.Title = "Horario de Postprocesado";
                        foreach (var x in db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 7 && x.bloqueado == false).OrderBy(x => x.ShortName).ToList())
                        {
                            personal.Add(new Resource(x.ShortName + " - " + x.codigousuario));
                        }
                        //"Tecnologo"
                        var _typePostProcesado = new ResourceType("PostProcesado");
                        _typePostProcesado.Resources.AddRange(personal);
                        listaPersonal.Add(_typePostProcesado);
                        break;
                    default:
                        throw new Exception("No se encontro ningun tipo relacionado");

                }
                rshorario.ResourceTypesSource = null;
                rshorario.ResourceTypesSource = listaPersonal;


            }
        }
        private void filtroequipo(object sender, System.EventArgs e)
        {
            System.Windows.Controls.Label lblequipo = (System.Windows.Controls.Label)sender;
            if (lblequipo.Content != null)
            {
                if (lblequipo.Content.ToString() != "TODOS")
                {
                    rshorario.AppointmentsSource = null;
                    rshorario.AppointmentsSource = appointments.Where(x => x.Location == lblequipo.Content.ToString()).ToList();
                    this.rshorario.Commit();
                    listarResumen();
                }
                else
                {
                    rshorario.AppointmentsSource = null;
                    rshorario.AppointmentsSource = appointments;
                    this.rshorario.Commit();
                    listarResumen();
                }
            }

        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            appointments = new ObservableCollection<Appointment>();
            Categories = new ObservableCollection<Category>();
            listaturnos = new List<AsignacionHorarioTecnologo>();
            listaturnos = await new AdministracionDAO().getListaHorarioTecnologo(dtpFecha.SelectedDate.Value, cboempresa.SelectedValue.ToString(), (tipoPermiso == 65 ? 1 : tipoPermiso == 66 ? 2 : tipoPermiso == 67 ? 3 : tipoPermiso == 68 ? 4 : tipoPermiso == 69 ? 5 : 0));
            // var rt = new ResourceType("Location");
            Random r = new Random();
            sktEquipo.Children.Clear();
            var lbltodos = new System.Windows.Controls.Label() { Content = "TODOS", BorderThickness = new Thickness(1, 1, 1, 1), BorderBrush = new SolidColorBrush(Colors.White), Margin = new Thickness(2.5, 0, 2.5, 0), Foreground = new SolidColorBrush(Colors.White) };
            lbltodos.MouseDoubleClick += filtroequipo;
            sktEquipo.Children.Add(lbltodos);
            foreach (var item in listaturnos.Select(x => x.nombreequipo).Distinct().ToList())
            {
                //rt.Resources.Add(new Resource(item));
                float red = r.Next(0, 70);
                float green = r.Next(0, 100);
                float blue = r.Next(0, 225);

               

                var color = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
                
                Categories.Add(new Category(item, color));
                var lblequipo=new System.Windows.Controls.Label() { Content = item, Background = color,BorderThickness=new Thickness(1,1,1,1),BorderBrush= new SolidColorBrush(Colors.White),Margin=new Thickness(2.5,0,2.5,0), Foreground = new SolidColorBrush(Colors.White) };
                lblequipo.MouseDoubleClick += filtroequipo;
                sktEquipo.Children.Add(lblequipo);
            }


            rshorario.CategoriesSource = null;
            rshorario.CategoriesSource = Categories;

            foreach (var item in listaturnos)
            {
                var appo = new Appointment();


                appo.Subject = item.nombreequipo + ": " + item.nombretecnologo;
                appo.Start = item.incio;
                appo.End = item.fin;
                appo.RecurrenceRule = null;
                appo.UniqueId = item.id.ToString();
                appo.Location = item.nombreequipo;
                appo.Category = this.rshorario.CategoriesSource.Cast<Category>().FirstOrDefault(c => c.CategoryName == item.nombreequipo);
                if (item.codigotecnologo != "")
                    appo.Resources.Add(listaPersonal.SingleOrDefault().Resources.SingleOrDefault(x => x.ResourceName.Contains(item.codigotecnologo)));

                appo.Body = "Horario autogenerado para el equipo " + item.nombreequipo;
                appointments.Add(appo);
            }


            rshorario.AppointmentsSource = null;
            rshorario.AppointmentsSource = appointments;
            this.rshorario.Commit();
            listarResumen();

        }

        private void rshorario_DialogClosing(object sender, CancelRoutedEventArgs e)
        {

            /*  this.rshorario.Commit(); 
              rshorario.AppointmentsSource = null;
              rshorario.AppointmentsSource = appointments;*/

        }

        private void rshorario_AppointmentEdited(object sender, AppointmentEditedEventArgs e)
        {
            var item = (Appointment)e.Appointment;
            var appo = appointments.SingleOrDefault(x => x.UniqueId == item.UniqueId);
            if (appo != null)
            {
                string[] tecnologo_excluido = new string[] { "HS272" };
                string tecnologo = "";
                if (item.Resources.Count > 0)
                {
                    tecnologo = item.Resources.SingleOrDefault().ResourceName;
                    var namepersonal = tecnologo.Split('-')[1].Trim();
                    if (!tecnologo_excluido.Contains(namepersonal))
                    {
                        var _appo_exist = listaturnos.Where(x => x.codigotecnologo == namepersonal && x.fecha.ToShortDateString() == appo.Start.ToShortDateString() && (appo.Start < x.fin && appo.End > x.incio)).ToList();

                        if (_appo_exist.Count > 0)
                        {
                            e.Handled = true;
                            e.Appointment.CancelEdit();
                            e.Appointment.EndEdit();
                            appo.Resources.Clear();
                            MessageBox.Show("Ya esta asignado el Colaborador en otro turno a la misma hora");
                            return;
                        }
                    }
                }

                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    int id = int.Parse(appo.UniqueId);
                    var turno = db.Horario_Tecnologo.SingleOrDefault(x => x.idProgramacion == id);
                    var local_turno = listaturnos.SingleOrDefault(x => x.id == id);
                    if (turno != null)
                    {
                        turno.user_update = session.codigousuario;
                        turno.fec_update = Tool.getDatetime();
                        if (tecnologo != "")
                        {
                            turno.tecnologo = tecnologo.Split('-')[1].Trim();
                            local_turno.nombretecnologo = tecnologo.Split('-')[0].Trim();
                            local_turno.codigotecnologo = tecnologo.Split('-')[1].Trim();
                            appo.Subject = turno.EQUIPO.nombreequipo + ": " + tecnologo.Split('-')[0].Trim();
                            appo.Resources.Clear();
                            appo.Resources.Add(listaPersonal.SingleOrDefault().Resources.SingleOrDefault(x => x.ResourceName.Contains(tecnologo.Split('-')[1].Trim())));
                        }
                        else
                        {
                            turno.tecnologo = null;
                            appo.Subject = turno.EQUIPO.nombreequipo + ": " + "*No Asignado";
                            local_turno.nombretecnologo = "*No Asignado";
                            local_turno.codigotecnologo = "";
                            appo.Resources.Clear();
                        }
                        db.SaveChanges();
                       


                        appo.Start = local_turno.incio;
                        appo.End = local_turno.fin;
                        appo.RecurrenceRule = null;
                        appo.IsAllDayEvent = false;
                        appo.UniqueId = local_turno.id.ToString();
                        appo.Category = this.rshorario.CategoriesSource.Cast<Category>().FirstOrDefault(c => c.CategoryName == local_turno.nombreequipo);


                        appo.Body = "Horario autogenerado para el equipo " + local_turno.nombreequipo;

                        listarResumen();
                    }
                }

            }

        }

        private void listarResumen()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                grid_resumen.IsBusy = true;
                grid_resumen.ItemsSource = listaturnos;
                grid_resumen.Rebind();
                grid_resumen.CalculateAggregates();
                grid_resumen.IsBusy = false;
            }
               ));


        }





    }
}
