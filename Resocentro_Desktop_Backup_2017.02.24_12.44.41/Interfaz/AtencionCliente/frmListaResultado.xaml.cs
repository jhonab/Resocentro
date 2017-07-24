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
            string apellidos = txtapellidos.Text;
            grid_Resultados.ItemsSource = null;
            grid_Resultados.ItemsSource = new AtencionClienteDAO().getListaResultados(apellidos, 1);
        }

        private void txtnum_examen_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string codigo = txtnum_examen.Text;
                var lista=new AtencionClienteDAO().getListaResultados(codigo, 2);
                grid_Resultados.ItemsSource = null;
                grid_Resultados.ItemsSource = lista;
                if (lista.Count == 1)
                {
                    abrirResultado(lista.First());
                }
                else
                {
                    MessageBox.Show("No se encontraron resultados por entregar", "ADVERTENCIA");
                }
            }
        }

        private async void abrirResultado(ResultadosEntity item)
        {
            if (new AtencionClienteDAO().verificarDeudacodigo(item.codigo))
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var numeroatencion = item.numeroatencion;
                    var Listcount = db.EXAMENXATENCION.Where(x => x.numeroatencion == numeroatencion).Select(x => x.codigo).ToList();
                    foreach (var codigo in Listcount)
                    {
                        var result = new AtencionClienteDAO().getListaResultados(codigo.ToString(), 2);
                        frmEntregarResultado gui = new frmEntregarResultado();
                        gui.cargarGUI(session, result.First());
                        gui.ShowDialog();
                    }
                }
               
            }
            else
                MessageBox.Show("El Paciente debe esta ATENCION,debera acercarse a Caja por el Examen: "+item.codigo, "ADVERTENCIA");
        }

        private void grid_Resultados_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            ResultadosEntity item = (ResultadosEntity)e.Row.DataContext;
            if (item != null)
            {
                abrirResultado(item);
            }
        }


    }
}
