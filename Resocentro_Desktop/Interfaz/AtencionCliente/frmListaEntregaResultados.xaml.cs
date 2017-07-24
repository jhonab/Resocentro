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
    /// Lógica de interacción para frmListaEntregaResultados.xaml
    /// </summary>
    public partial class frmListaEntregaResultados : Window
    {
        MySession session;
        public frmListaEntregaResultados()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            listadodiario();
        }

        private async void listadodiario()
        {
           try
            {
            grid_ResultadosHoy.IsBusy = true;
            grid_ResultadosHoy.ItemsSource = await new AtencionClienteDAO().getListaResultadosEntregadosDiario(session.codigousuario);
            grid_ResultadosHoy.IsBusy = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var filtro = txtapellidos.Text.Trim();
            if (filtro != "")
            {
                buscarPaciente(filtro);
            }
        }
        public void buscarPaciente(string filtro)
        {
            grid_Paciente.ItemsSource = null;
            List<PacienteEntity> lista = new List<PacienteEntity>();
            var result = new PacienteDAO().getBuscarPacientexCoincidencia(filtro, "", "");
            foreach (var item in result)
            {
                var p = new PacienteEntity();
                p.nacionalidad = item.nacionalidad;
                p.direccion = item.direccion;
                p.email = item.email;
                p.celular = item.celular;
                p.telefono = item.telefono;
                p.fechanace = item.fechanace;
                p.sexo = item.sexo;
                p.nombres = item.nombres;
                p.apellidos = item.apellidos;
                p.dni = item.dni;
                p.codigopaciente = item.codigopaciente;
                p.tipo_doc = item.tipo_doc;
                p.IsProtocolo = item.IsProtocolo == null ? false : item.IsProtocolo.Value;
                lista.Add(p);
            }
            grid_Paciente.ItemsSource = lista;
            if (lista.Count == 0)
            {
                MessageBox.Show("No se encontro ningun registro con el filtro ingresado \"" + filtro + "\". \nVuelva a intentar", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void grid_Paciente_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            PacienteEntity pac = (PacienteEntity)e.Row.DataContext;
            if (pac != null)
            {
                grid_Resultados.ItemsSource = null;
                grid_Resultados.ItemsSource = new AtencionClienteDAO().getListaResultadosEntregados(pac.codigopaciente);
            }
        }

        private void grid_Resultados_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            ResultadosEntregadosEntity item = (ResultadosEntregadosEntity)e.Row.DataContext;
            if (item != null)
            {
                if (item.despacho == "ENTREGADO PACIENTE")
                {
                    using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                    {
                        var r = db.RESULTADODELIVERY.SingleOrDefault(x => x.numerodelivery == item.codigodelivery);
                        if (r != null)
                        {
                            ResultadosEntity resul = new ResultadosEntity();
                            resul.paciente = r.personacontacto.ToUpper();
                            resul.direccion = r.direcciondelivery;
                            resul.foto = r.cantidadfoto;
                            resul.numerodelivery = r.numerodelivery;
                            resul.placa = r.cantidadplaca;
                            resul.observaciones = r.observacion;
                            resul.firma = r.firma;
                            resul.estudios = db.EXAMENXATENCION.Where(x => x.numeroatencion == r.numeroatencion && x.estadoestudio == "V").Select(x => new ListaEstudiosResultados { codigo = x.codigo, estudio = x.ESTUDIO.nombreestudio }).ToList();

                            frmEntregarResultado gui = new frmEntregarResultado();
                            gui.setItem(resul);
                            gui.ShowDialog();
                        }

                    }
                }
            }
        }

        private void txtapellidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var filtro = txtapellidos.Text.Trim();
                if (filtro != "")
                {
                    buscarPaciente(filtro);
                }
            }
        }

        private  void btnactualizar_Click(object sender, RoutedEventArgs e)
        {
            listadodiario();
        }

        
    }
}
