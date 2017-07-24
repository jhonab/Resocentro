using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
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

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmSolicitarEntreTurno.xaml
    /// </summary>
    public partial class frmSolicitarEntreTurno : Window
    {
        MySession session;
        public frmSolicitarEntreTurno()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboEquipo.ItemsSource = new UtilDAO().getEquipoxEmpresa(1);
            cboEquipo.SelectedValuePath = "codigoequipo";
            cboEquipo.DisplayMemberPath = "nombreequipo";
            cboEquipo.SelectedIndex = 0;

            cbousuario.ItemsSource = new UtilDAO().getUsuarios();
            cbousuario.SelectedValuePath = "codigousuario";
            cbousuario.DisplayMemberPath = "ShortName";
            cbousuario.SelectedIndex = 0;

            cbotipomotivo.ItemsSource = new UtilDAO().getMotivoEntreTurno();
            cbotipomotivo.SelectedValuePath = "idMotivo";
            cbotipomotivo.DisplayMemberPath = "descripcion";
            cbotipomotivo.SelectedIndex = 0;

            var time = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            dtpHora.SelectedTime = time;
            dtpHora.StartTime = time;
            dtpHora.EndTime = new TimeSpan(22, 0, 0); ;

            lblfecha.Content = DateTime.Now.ToShortDateString();
            lblusuario.Content = session.shortuser; 
        }

        private void btnSolicitar_Click(object sender, RoutedEventArgs e)
        {
            if (isvalidar())
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    Solicitud_Entre_Turno item = new Solicitud_Entre_Turno();
                    item.fecha_reg = Tool.getDatetime(); ;
                    item.hora_reg = new DateTime(item.fecha_reg.Year, item.fecha_reg.Month, item.fecha_reg.Day, dtpHora.SelectedTime.Value.Hours, dtpHora.SelectedTime.Value.Minutes, 0);
                    item.usu_reg = session.codigousuario;
                    item.codigoequipo_reg = int.Parse(cboEquipo.SelectedValue.ToString());
                    item.solicitante = cbousuario.SelectedValue.ToString();
                    item.codigomotivo = int.Parse(cbotipomotivo.SelectedValue.ToString());
                    item.comentario_usu = txtcomentarios.Text;
                    db.Solicitud_Entre_Turno.Add(item);
                    db.SaveChanges();
                    MessageBox.Show("Su solicitud de entre turno fue registrada con éxito, se le notificará al correo cuando tengamos una respuesta.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private bool isvalidar()
        {
            var hora = dtpHora.SelectedTime.Value;
            if (cboEquipo.SelectedValue== null)
            {
                MessageBox.Show("Seleccione un Equipo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (cbotipomotivo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un Motivo", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (dtpHora.SelectedTime == null)
            {
                MessageBox.Show("Ingrese un Hora", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (cbousuario.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un Usuario", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if(DateTime.Now.TimeOfDay>=hora)
            {
                MessageBox.Show("La hora ingresada es incorrecta es menor a la hora del sistema", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (dtpHora.EndTime< hora)
            {
                MessageBox.Show("La hora ingresada excede para la creaccion de entre turnos","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }           
            foreach (var item in (List<Turno_Horario>)gridTurnos.Items.SourceCollection)
            {
                if (item.hora.TimeOfDay == hora)
                {
                    MessageBox.Show("La hora ingresada es incorrecta por que se cruza con el estudio " + item.estudio.ToUpper() + " del paciente " + item.paciente.ToUpper(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }

        private void cboEquipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listarTurnos();
        }

        public void listarTurnos()
        {
            if (cboEquipo.SelectedValue != null)
            {
                gridTurnos.ItemsSource = new CitaDAO().getListaAdminTurnos(cboEquipo.SelectedValue.ToString(), DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                lbltitulogrid.Content = "Lista de Turnos del Equipo: " + ((EQUIPO)cboEquipo.SelectedItem).nombreequipo.ToUpper();
            }
            else
                lbltitulogrid.Content = "";
        }
    }
}
