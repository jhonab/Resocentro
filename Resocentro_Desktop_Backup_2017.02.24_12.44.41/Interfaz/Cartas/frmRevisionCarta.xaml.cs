using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Cartas;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmRevisionCarta.xaml
    /// </summary>
    public partial class frmRevisionCarta : Window
    {
        MySession session;
        public frmRevisionCarta()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            dtp_fecha_tramite.SelectedDate = DateTime.Now.AddDays(1);
            cargarCartas(dtp_fecha_tramite.SelectedDate.Value.ToShortDateString());
        }

        private void grid_Cartas_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            RevisionCarta item = (RevisionCarta)e.Row.DataContext;
            if (item != null)
            {
                var carta = (new CartaDAO()).getCartaxCodigo(item.codigocartagarantia, item.idpaciente);
                var detalle = new CartaDAO().getDetalleCartaxCodigo(item.codigocartagarantia, item.idpaciente);

                frmCarta gui = new frmCarta();
                gui.cargarGUI(session, false);
                new CartaDAO().insertLog(item.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la Carta N° " + item.codigocartagarantia.ToString()+" para revisar");
                gui.setCartaGarantia(carta, detalle, true);
                gui.ShowDialog();
                var _resultado = MessageBox.Show("¿La carta tiene alguna observación?", "ADVERTENCIA", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (_resultado == MessageBoxResult.No)
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var ca = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == item.codigocartagarantia && x.codigopaciente == item.idpaciente);
                        if (ca != null)
                        {
                            ca.isRevisada = true;
                            ca.user_revisa = session.codigousuario;
                            ca.fec_revisa = Tool.getDatetime();
                            ca.obs_revision = "";
                            db.SaveChanges();
                            cargarCartas(dtp_fecha_tramite.SelectedDate.Value.ToShortDateString());
                            new CartaDAO().insertLog(item.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se Revisó la Carta N° " + item.codigocartagarantia.ToString()+" sin observaciones");
                            MessageBox.Show("Se actualizo la Carta", "INFORMACION", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else if (_resultado == MessageBoxResult.Yes)
                {
                    frmObsRevision gui_obs = new frmObsRevision();
                    if (gui_obs.ShowDialog()==true)
                    {
                        using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                        {
                            var ca = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == item.codigocartagarantia && x.codigopaciente == item.idpaciente);
                            if (ca != null)
                            {
                                ca.isRevisada = true;
                                ca.user_revisa = session.codigousuario;
                                ca.fec_revisa = Tool.getDatetime();
                                ca.obs_revision = gui_obs.txtobservaciones.Text;
                                db.SaveChanges();
                                cargarCartas(dtp_fecha_tramite.SelectedDate.Value.ToShortDateString());
                                new CartaDAO().insertLog(item.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se Revisó la Carta N° " + item.codigocartagarantia.ToString() + " con observaciones: "+ca.obs_revision.ToUpper());
                                MessageBox.Show("Se actualizo la Carta", "INFORMACION", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
            }
        }

        private void btnbuscar_tramite_Click(object sender, RoutedEventArgs e)
        {
            cargarCartas(dtp_fecha_tramite.SelectedDate.Value.ToShortDateString());
        }
        public void cargarCartas(string fecha)
        {
            grid_Cartas.ItemsSource = null;
            grid_Cartas.ItemsSource = new CartaDAO().getRevisionCartas(fecha).OrderBy(x => x.paciente);
        }

        private void RadContextMenuCarta_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
           RevisionCarta item = (RevisionCarta)this.grid_Cartas.SelectedItem;
            if (item == null)
            {
                ContextMenu_ExcluirCarta.Visibility = Visibility.Collapsed;
            }
            else
            {
                ContextMenu_ExcluirCarta.Visibility = Visibility.Visible;
            }
        }

        private void MenuItemAbrir_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (grid_Cartas.SelectedItems!=null)
            {
                foreach (RevisionCarta item in grid_Cartas.SelectedItems)
                {


                    if (MessageBox.Show("¿Desea excluir de la revision la Carta N°" + item.codigocartagarantia + " Paciente: " + item.paciente + "?", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                        {
                            var ca = db.CARTAGARANTIA.SingleOrDefault(x => x.codigocartagarantia == item.codigocartagarantia && x.codigopaciente == item.idpaciente);
                            if (ca != null)
                            {
                                ca.isRevisada = true;
                                ca.fec_revisa = Tool.getDatetime(); 
                                db.SaveChanges();

                                new CartaDAO().insertLog(item.codigocartagarantia.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se excluyó de la revision la Carta N° " + item.codigocartagarantia.ToString());

                            }
                        }
                    }
                }
                cargarCartas(dtp_fecha_tramite.SelectedDate.Value.ToShortDateString());
            }
        }

        private void grid_Cartas_FilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }
    }
}
