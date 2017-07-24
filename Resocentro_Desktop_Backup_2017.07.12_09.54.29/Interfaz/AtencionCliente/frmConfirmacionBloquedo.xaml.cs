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
    /// Lógica de interacción para frmConfirmacionBloquedo.xaml
    /// </summary>
    public partial class frmConfirmacionBloquedo : Window
    {
        MySession session;
        List<Turno_Horario> lstTurno;
        public frmConfirmacionBloquedo()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, List<Turno_Horario> lista)
        {
            this.session = session;
            lstTurno = lista.ToList();
            cbomotivo.ItemsSource = new UtilDAO().getMotivoBloqueo(false);
            cbomotivo.SelectedValuePath = "codigopaciente";
            cbomotivo.DisplayMemberPath = "apellidos";
            cbomotivo.SelectedIndex = 1;
        }

        private void btnBloquear_Click(object sender, RoutedEventArgs e)
        {
            string msj = "";
            if (lstTurno.Count > 1)
                msj = "¿Esta seguro(a) de Bloquear "+lstTurno.Count+" TURNOS?\nverifique que haya elegido los TURNOS correctos \n Desea Continuar?";
            else
                msj = "¿Esta seguro(a) de Bloquear el TURNOS?\nverifique que haya elegido el TURNO correcto \n Desea Continuar?";
            if (MessageBox.Show(msj, "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (var item in lstTurno)
                {
                    new UtilDAO().setBlockTurno(int.Parse(cbomotivo.SelectedValue.ToString()),item.codigohorario,session);
                }
                MessageBox.Show("Bloqueo realizado", "Exito");
                DialogResult = true;
            }
        }

        private void btnDesbloquear_Click(object sender, RoutedEventArgs e)
        {
            string msj = "";
            if (lstTurno.Count > 1)
                msj = "¿Esta seguro(a) de Desbloquear " + lstTurno.Count + " TURNOS?\nverifique que haya elegido los TURNOS correctos \n Desea Continuar?";
            else
                msj = "¿Esta seguro(a) de Desbloquear el TURNOS?\nverifique que haya elegido el TURNO correcto \n Desea Continuar?";
            if (MessageBox.Show(msj, "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                foreach (var item in lstTurno)
                {
                    new UtilDAO().setUnBlockTurno(int.Parse(cbomotivo.SelectedValue.ToString()), item.codigohorario, session);
                }
                MessageBox.Show("Desbloqueo realizado", "Exito");
                DialogResult = true;
            }
        }
    }
}
