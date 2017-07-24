using Resocentro_Desktop.DAO;
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
    /// Lógica de interacción para frmTurnoMedico.xaml
    /// </summary>
    public partial class frmTurnoMedico : Window
    {
        Search_Estudio estudio { get; set; }
        public frmTurnoMedico()
        {
            InitializeComponent();
        }

        public void cargarGUI(Search_Estudio estudio, DateTime fecha,int codigosucursal,string modalidad)
        {
            this.estudio = estudio;
            this.estudio.fechareserva = fecha;
            lblfecha.Content = fecha.ToString("dd \"de\" MMMM \"del\" yyyy");
            gridTurnos.ItemsSource = new EstudiosDAO().getTurnoCita(codigosucursal.ToString().Substring(0, 1), (codigosucursal - 100).ToString(), modalidad, fecha.Day, fecha.Month, fecha.Year);
        }

        private void gridTurnos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Turno_Citar item = (Turno_Citar)e.Row.DataContext;
            if (item != null)
            {
                estudio.fechareserva = new DateTime(estudio.fechareserva.Year, estudio.fechareserva.Month, estudio.fechareserva.Day, item.hora.Hour, item.hora.Minute, 0);
                estudio.turno = item;
                DialogResult = true;
            }
        }
    }
}
