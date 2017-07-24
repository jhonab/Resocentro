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

namespace Resocentro_Desktop.Interfaz.AtencionCliente
{
    /// <summary>
    /// Lógica de interacción para frmListaResultado.xaml
    /// </summary>
    public partial class frmListaResultado : Window
    {
        MySession session;
        public frmListaResultado()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BuscarResultados();
        }

        private async void BuscarResultados()
        {
            string apellidos = txtapellidos.Text;
            if (apellidos != "")
            {
                grid_Resultados.IsBusy = true;
                grid_Resultados.ItemsSource = null;
                var lista = await new AtencionClienteDAO().getListaResultados(apellidos, 1);
                grid_Resultados.IsBusy = false;
                grid_Resultados.ItemsSource = lista.OrderBy(x => x.fecha).ToList();
                if (lista.Count() == 0)
                    MessageBox.Show("No se encontraron resultados por entregar", "ADVERTENCIA");
            }
        }

        private async void txtnum_examen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && txtnum_examen.Text != "")
            {
                string codigo = txtnum_examen.Text;
                txtnum_examen.Text = "";
                var lista = await new AtencionClienteDAO().getListaResultados(codigo, 2);
                grid_Resultados.ItemsSource = null;
                grid_Resultados.ItemsSource = lista;
                if (lista.Count == 1)
                {
                    var item = lista.First();
                    //item.estudios = new AtencionClienteDAO().getEstudiosxAtencionResultados(item.numeroatencion);
                    abrirResultado(item);
                }
                else
                {
                    MessageBox.Show("No se encontraron resultados por entregar", "ADVERTENCIA");
                }
            }
        }

        private void abrirResultado(ResultadosEntity item)
        {
            frmEntregarResultado gui = new frmEntregarResultado();
            gui.cargarGUI(session, item);
            var cTask = new AtencionClienteDAO().verificarDeudacodigo(item.numeroatencion);
            cTask.ContinueWith(x =>
            {
                gui.lblmensaje.Visibility = Visibility.Collapsed;
                if (x.Result == true)
                {
                    MessageBox.Show("El Paciente presenta inconsistencias con su pago.\n VERIFIQUE EN EL SISTEMA.", "ADVERTENCIA");
                    /*var estudio = item.estudios.FirstOrDefault();
                    if (estudio != null)
                    {
                        frmHistoriaPaciente gui1 = new frmHistoriaPaciente();
                        gui1.cargarGUI(session);
                        gui1.Show();
                        gui1.buscarPaciente(estudio.codigo.ToString(),1);
                    }*/
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
            gui.ShowDialog();

        }

        private void grid_Resultados_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            ResultadosEntity item = (ResultadosEntity)e.Row.DataContext;
            if (item != null)
            {
                //item.estudios = new AtencionClienteDAO().getEstudiosxAtencionResultados(item.numeroatencion);
                abrirResultado(item);
            }
        }


        private void txtapellidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtapellidos.Text != "")
                BuscarResultados();
        }


    }
}
