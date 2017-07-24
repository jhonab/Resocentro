using Resocentro_Desktop.Complemento;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Resocentro_Desktop.Interfaz.frmCita
{
    /// <summary>
    /// Lógica de interacción para frmVisorCitas.xaml
    /// </summary>
    public partial class frmVisorCitas : Window
    {
        MySession session;
        private List<VisorCitaEjmX> _data;
        public List<VisorCitaEjmX> data
            {
            get { return _data; }
            set
            {
                _data = value;
                
            }
        }
        private List<VisorCitaEjm> _Lst1;

        public List<VisorCitaEjm> Lst1
        {
            get { return _Lst1; }
            set
            {
                _Lst1 = value;
                
            }
        }
        private List<VisorCitaEjm> _Lst2;

        public List<VisorCitaEjm> Lst2
        {
            get { return _Lst2; }
            set
            {
                _Lst2 = value;
                
            }
        }
        
        public frmVisorCitas()
        {
            InitializeComponent();

            DataContext = this;
            
        }
        
        public void cargarGUI(MySession session)
        {
            this.session = session;
            dtpFecha.SelectedDate = DateTime.Now;
            cboEmpresa.ItemsSource = new CitaDAO().getEmpresaSucursal();
            cboEmpresa.SelectedValuePath = "codigoShort";
            cboEmpresa.DisplayMemberPath = "concatenado";
            cboEmpresa.SelectedIndex = 0;

            cboEquipo.ItemsSource = new CitaDAO().getEquipoEmpresaSucursal();
            cboEquipo.SelectedValuePath = "codigoequipo";
            cboEquipo.DisplayMemberPath = "equipo";
            cboEquipo.SelectedIndex = 0;
            
        }
        /// <summary>
        /// Obtiene el visor de citas segun empresas o equipos
        /// </summary>
        /// <param name="tipo">1 Empresa,2 Modalidad, 3 Equipo</param>
        public void cargarVisorCita(int tipo)
        {
            _data = new List<VisorCitaEjmX>();
            _Lst1 = new List<VisorCitaEjm>();
            _Lst2 = new List<VisorCitaEjm>();
            var lista = new CitaDAO().getVisorCita(tipo, 0, dtpFecha.SelectedDate.Value.Day, dtpFecha.SelectedDate.Value.Month, dtpFecha.SelectedDate.Value.Year, int.Parse(cboEquipo.SelectedValue.ToString()), int.Parse(cboEmpresa.SelectedValue.ToString().Substring(0, 1)), int.Parse(cboEmpresa.SelectedValue.ToString().Substring(1)));
            int count = 1;

            foreach (var idequipo in lista.OrderBy(x=> x.nombreequipo).Select(x => x.equipo).Distinct().ToList().AsParallel())
            {
                var equipo = lista.First(x => x.equipo == idequipo);
                var detalle = lista.Where(x => x.equipo == idequipo).AsParallel().ToList();
                VisorCitaEjm _x = new VisorCitaEjm();
                _x.titulo=equipo.nombreequipo+" Total Turnos: " + detalle.Count() + "\t Disponible: " + detalle.Where(x => x.paciente == "").ToList().AsParallel().Count();
                _x.estudios = detalle;
                if (count % 2 == 0)
                    _Lst2.Add(_x);
                else
                    _Lst1.Add(_x);

                count++;

            }

            _data.Add(new VisorCitaEjmX() { stk1 = _Lst1, stk2 = _Lst2 });
            itcLista.ItemsSource = _data;
            //this.DataContext = data;
            //this.DataContext = _Lst1;
            //this.DataContext = _Lst2;
            MessageBoxTemporal.Show("Cargado ", "Exito", 3, true);
                /*

                System.Windows.Controls.Label lblnombre = new System.Windows.Controls.Label();
                
                lblnombre.Content = equipo.nombreequipo;
                lblnombre.Name = "lbl" + equipo.equipo.ToString();
                lblnombre.Foreground = Brushes.White;
                lblnombre.FontSize = 15;
                var margin = lblnombre.Margin;
                margin.Top = 10;
                margin.Bottom = 5;
                lblnombre.Margin = margin;
                RadGridView grid = new RadGridView();
                grid.Name = "grid" + equipo.equipo.ToString();
                grid.IsReadOnly = true;
                grid.Height = 300;
                grid.ShowGroupPanel = false;
                grid.CanUserResizeColumns = false;
                grid.CanUserInsertRows = false;
                grid.AutoGenerateColumns = false;
                grid.RowIndicatorVisibility = Visibility.Collapsed;



                GridViewImageColumn colSedacion = new GridViewImageColumn();
                colSedacion.DataMemberBinding = new Binding("imgSedacion");
                colSedacion.Width = 20;
                colSedacion.IsFilterable = false;
                colSedacion.TextAlignment = TextAlignment.Center;
                grid.Columns.Add(colSedacion);

                GridViewDataColumn colHora = new GridViewDataColumn();
                colHora.Header = "Hora";
                colHora.DataMemberBinding = new Binding("hora");
                colHora.DataFormatString = "{0:HH:mm}";
                grid.Columns.Add(colHora);

                GridViewDataColumn colPaciente = new GridViewDataColumn();
                colPaciente.Header = "Paciente";
                colPaciente.DataMemberBinding = new Binding("paciente");
                grid.Columns.Add(colPaciente);

                GridViewDataColumn colEstudio = new GridViewDataColumn();
                colEstudio.Header = "Estudio";
                colEstudio.DataMemberBinding = new Binding("estudio");
                grid.Columns.Add(colEstudio);

                GridViewDataColumn colEstado = new GridViewDataColumn();
                colEstado.Header = "E";
                colEstado.DataMemberBinding = new Binding("estado");
                grid.Columns.Add(colEstado);

                GridViewDataColumn colTurno = new GridViewDataColumn();
                colTurno.Header = "TM";
                colTurno.DataMemberBinding = new Binding("turno");
                grid.Columns.Add(colTurno);

                GridViewDataColumn colCita = new GridViewDataColumn();
                colCita.Header = "N° Cita";
                colCita.DataMemberBinding = new Binding("cita");
                grid.Columns.Add(colCita);
                
                grid.ItemsSource = detalle;
                lblnombre.Content += " Total Turnos: " + detalle.Count() + "\t Disponible: " + detalle.Where(x => x.paciente == "").ToList().AsParallel().Count();

                if (count % 2 == 0)
                {
                    _Lst1.Add(grid);

                }
                else
                {
                    _Lst1.Add(grid);
                    //control.Add(grid.Name, grid);
                    //stck1.Children.Add(lblnombre);
                    //stck1.Children.Add(grid);
                }
                
               */
                
            
            

        }

        public void limpiarVisor()
        {
            _Lst1 = new List<VisorCitaEjm>();
            _Lst2 = new List<VisorCitaEjm>();
        }
        private void btnBuscarXEmpresa_Click(object sender, RoutedEventArgs e)
        {
            if (dtpFecha.SelectedDate != null)
            {
                if (cboEmpresa.SelectedValue != null)
                    cargarVisorCita(1);
                else
                {
                    MessageBox.Show("Seleccione una Empresa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    limpiarVisor();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una Fecha.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                limpiarVisor();
            }
        }

        private void btnBuscarXEquipo_Click(object sender, RoutedEventArgs e)
        {
            if (dtpFecha.SelectedDate != null)
            {
                if (cboEquipo.SelectedValue != null)
                    cargarVisorCita(3);
                else
                {
                    MessageBox.Show("Seleccione una Empresa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    limpiarVisor();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una Fecha.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                limpiarVisor();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            var button = (Button)e.OriginalSource;
            int idpaciente = 0;
            int cita= 0;
            if (int.TryParse(button.CommandParameter.ToString(), out idpaciente))
            {
               /* frmHistoriaPaciente gui = new frmHistoriaPaciente();
                gui.cargarGUI(session);
                gui.Show();
                gui.buscarPaciente(idpaciente.ToString(), 2);
                if (int.TryParse(button.TabIndex.ToString(), out cita))
                {
                    gui.seleccionarItem(cita);
                }*/

            }
        }
    }
}
