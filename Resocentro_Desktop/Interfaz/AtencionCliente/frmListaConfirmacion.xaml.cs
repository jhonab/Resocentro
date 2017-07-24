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
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.Interfaz.AtencionCliente
{
    /// <summary>
    /// Lógica de interacción para frmListaConfirmacion.xaml
    /// </summary>
    public partial class frmListaConfirmacion : Window
    {
        MySession session;
        public frmListaConfirmacion()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            dtpFecha.SelectedDate = DateTime.Now;
        }
        private void btnListar_Click(object sender, RoutedEventArgs e)
        {
            ListarCitas();
        }

        private void ListarCitas()
        {
            gridCitas.ItemsSource = new CitaDAO().getCitasxConfirmar(dtpFecha.SelectedDate.Value.ToShortDateString());
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            var button = (RadButton)e.OriginalSource;
            var item = (CitarxConfirmar)button.CommandParameter;
            if (item != null)
                if (button.Content != null)
                    if (button.Content.ToString() != "" && button.Content.ToString() != " ")
                    {
                        ConfimacionLlamada(item, button.Content.ToString());
                    }
        }

        private void btnTelefono_Click(object sender, RoutedEventArgs e)
        {
            var button = (RadButton)e.OriginalSource;
            var item = (CitarxConfirmar)button.CommandParameter;
            if (item != null)
                if (button.Content != null)
                    if (button.Content.ToString() != "" && button.Content.ToString() != "0")
                    {
                        ConfimacionLlamada(item, button.Content.ToString());
                    }
        }

        private void btnCelular_Click(object sender, RoutedEventArgs e)
        {
            var button = (RadButton)e.OriginalSource;
            var item = (CitarxConfirmar)button.CommandParameter;
            if (item != null)
                if (button.Content != null)
                    if (button.Content.ToString() != "" && button.Content.ToString() != "0")
                    {
                        ConfimacionLlamada(item, button.Content.ToString());
                    }
        }

        private void ConfimacionLlamada(CitarxConfirmar item , string seleccion)
        {
            frmConfirmarCita gui = new frmConfirmarCita();
            gui.cargarGUI(session, item,seleccion);
            gui.ShowDialog();
        }

        
    }
}
