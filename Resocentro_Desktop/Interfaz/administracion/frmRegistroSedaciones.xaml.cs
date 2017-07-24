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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmRegistroSedaciones.xaml
    /// </summary>
    public partial class frmRegistroSedaciones : Window
    {
        MySession session;
        public frmRegistroSedaciones()
        {
            InitializeComponent();
            ObservableCollection<RadMenuItem> items = new ObservableCollection<RadMenuItem>();
            foreach (var item in new AdministracionDAO().listSedadores())
            {
                RadMenuItem newItem = new RadMenuItem();
                newItem.Header = item.medico;
                items.Add(newItem);
            }
            radcontextmenu.ItemsSource = null;
            radcontextmenu.ItemsSource = items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridExamen.ItemsSource = new AdministracionDAO().getSedacionesRealizadas(dtpfecha.SelectedDate.Value.ToShortDateString());
        }

        private void RadContextMenuExamen_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var lista = this.gridExamen.SelectedItems;
            if (lista.Count > 0)
                radcontextmenu.Visibility = Visibility.Visible;
            else
                radcontextmenu.Visibility = Visibility.Collapsed;

        }

        private void radcontextmenu_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadMenuItem item = e.OriginalSource as RadMenuItem;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var sedador = db.Com_Medico.SingleOrDefault(x => x.medico == item.Header.ToString());
                if (sedador != null)
                    foreach (AsignacionSedaciones examen in gridExamen.SelectedItems)
                    {
                        var cs = db.CONTROL_SEDACION.SingleOrDefault(x => x.examen == examen.codigo);
                        if (cs == null)
                        {
                            var cita = db.EXAMENXCITA.SingleOrDefault(x => x.codigoexamencita == examen.detallecita);
                            if (cita != null)
                            {
                                cita.sedador = sedador.codigousuario;
                                CONTROL_SEDACION registro = new CONTROL_SEDACION();
                                registro.codigoregistro = DateTime.Now.ToString("ddMMyyyy") + cita.codigopaciente;
                                registro.anestesiologo = sedador.medico;
                                registro.fecha = DateTime.Now;
                                registro.codigopaciente = cita.codigopaciente;
                                registro.cantidadestudio = 1;
                                registro.codigocompaniaseguro = cita.codigocompaniaseguro;
                                registro.codigoclinica = cita.CITA.codigoclinica;
                                registro.montosedacion = "";
                                registro.tipodecambio = 0;
                                registro.motivo = "";
                                registro.estado = "Realizado";
                                registro.tiposedacion = "Inhalatoria";
                                registro.examen = examen.codigo;
                                registro.usuario = session.codigousuario;
                                db.CONTROL_SEDACION.Add(registro);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("El registro ya existe.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
            }

        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
        }
    }
}
