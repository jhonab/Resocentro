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

namespace Resocentro_Desktop.Interfaz.Cartas
{
    /// <summary>
    /// Lógica de interacción para frmReporteSedacionesCarta.xaml
    /// </summary>
    public partial class frmReporteSedacionesCarta : Window
    {
        MySession session;
        public frmReporteSedacionesCarta()
        {
            InitializeComponent();
            dtpfecha.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridSedaciones.ItemsSource = new CartaDAO().getSedaciones(dtpfecha.SelectedDate.Value.ToShortDateString());
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }

        private void gridSedaciones_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            Sedacion_Carta item = (Sedacion_Carta)e.Row.DataContext;
            if (item != null)
            {
                frmHistoriaPaciente gui = new frmHistoriaPaciente();
                gui.cargarGUI(session);
                gui.Show();
                gui.buscarPaciente(item.codigopaciente.ToString(), 2);
            }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                foreach (Sedacion_Carta item in gridSedaciones.Items.SourceCollection)
                {
                    var examen = db.EXAMENXATENCION.SingleOrDefault(x => x.codigo == item.codigo);
                    if (examen != null)
                    {
                        examen.isRevisadoCarta = item.revisado;
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Se actualizaron los registros.", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
