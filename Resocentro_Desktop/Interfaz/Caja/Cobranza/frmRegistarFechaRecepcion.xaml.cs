using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmRegistarFechaRecepcion.xaml
    /// </summary>
    public partial class frmRegistarFechaRecepcion : Window
    {
        MySession session;
        List<DocumentosRecepcion> lista;
        public frmRegistarFechaRecepcion()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            rdpFecha.SelectedDate = DateTime.Now;
        }

        private void txtlote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if(txtlote.Text!="")
                listar(new CobranzaDAO().getDocumentoCiaxLote(1,txtlote.Text,"1"));
        }

        private void txtdocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (txtdocumento.Text != "")
                    listar(new CobranzaDAO().getDocumentoCiaxLote(3, txtdocumento.Text, "1"));
        }

        private void listar(List<DocumentosRecepcion> list)
        {
            lista = list;
            griddocumentos.ItemsSource = null;
            griddocumentos.ItemsSource = lista;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Count > 0)
            {
                new CobranzaDAO().setFechaRecepcionCia(String.Join("','", lista.Select(x=> x.numerodocumento).ToArray()), "1", rdpFecha.SelectedDate.Value.ToShortDateString());
                MessageBox.Show("Se actualizaron las fechas");
            }
        }

        private void txtloteInterno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (txtloteInterno.Text != "")
                    listar(new CobranzaDAO().getDocumentoCiaxLote(2, txtloteInterno.Text, "1"));
        }
         
    }
}
