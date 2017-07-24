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
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmAsignacionContraste.xaml
    /// </summary>
    public partial class frmAsignacionContraste : Window
    {
        MySession session;
        public frmAsignacionContraste()
        {
            InitializeComponent();
            gridAsignacion.ItemsSource = new AdministracionDAO().listAsignacionInsumo();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridAsignacion.ItemsSource = new AdministracionDAO().listAsignacionInsumo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                foreach (AsignacionInsumos insumo in gridAsignacion.Items.SourceCollection)
                {
                    var enfermeria = db.AL_Enfermeria.SingleOrDefault(x => x.Al_InsumoId == insumo.idinsumo);

                    if (enfermeria == null)
                    {
                        AL_Enfermeria item = new AL_Enfermeria();
                        item.Al_InsumoId = insumo.idinsumo;
                        item.isVisibleEnfermeria = insumo.estado;
                        db.AL_Enfermeria.Add(item);
                    }
                    else
                    {
                        enfermeria.isVisibleEnfermeria = insumo.estado;
                    }
                    db.SaveChanges();


                }
                MessageBox.Show("Se actualizaron los registros.", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }

    }
}
