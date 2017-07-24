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

namespace Resocentro_Desktop.Interfaz
{
    /// <summary>
    /// Lógica de interacción para frmAdquisicion.xaml
    /// </summary>
    public partial class frmAdquisicion : Window
    {
        public frmAdquisicion()
        {
            InitializeComponent();
        }

        public void cargarGUI(DetalleHCAdquisicion item)
        {
            //Tecnologo
            lbltecnologo.Content = item.tecnologoName;
            lblequipo.Content = item.equipo;
            lblplaca.Content = item.placasuso;
            gridTecnicas.ItemsSource = item.tecnicasTecnologo;
            //Enfermera
            lblenfermera.Content = item.enfermeraName;
            lblcontraste.Content = item.contraste;
            gridInsumo.ItemsSource = item.insumoEnfermera;
            //Anestesiologo
            lblsedador.Content = item.anestesiologoName;
            lbltiposedacion.Content = item.tipoSedacion;
            lblmotivosedacion.Content = item.motivoSedacion;
            gridInsumosedacion.ItemsSource = item.insumoSedacion;
            //Postprocesador
            gridPostproceso.ItemsSource = item.tecnicasPostproceso;
        }
    }
}
