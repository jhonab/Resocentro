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

namespace Resocentro_Desktop.Interfaz.frmDocumento
{
    /// <summary>
    /// Lógica de interacción para frmDocumento.xaml
    /// </summary>
    public partial class frmDocumento : Window
    {
        MySession session;
        int paciente;
        public frmDocumento()
        {
            InitializeComponent();
        }

        public void setDocumento(MySession session, string documento, string ruc, int codigopaciente,int empresa,int sede)
        {
            this.session = session;
            this.paciente = codigopaciente;
            var lista = new DocumentoDAO().getDocumento(documento, codigopaciente);
            gridDocumento.ItemsSource = lista;
            gridDetalleDocumento.ItemsSource = new DocumentoDAO().getDetalleDocumento(documento, codigopaciente);
            gridAseguradora.ItemsSource = new DocumentoDAO().getAseguradoraDocumento(lista.SingleOrDefault().RucAlterno);
            gridFormaPago.ItemsSource = new DocumentoDAO().getFormaPagoDocumento(documento, codigopaciente,empresa.ToString(),sede.ToString());
            gridCobranza.ItemsSource = new DocumentoDAO().getCobranzaDocumento(documento, codigopaciente);
            gridLibroCaja.ItemsSource = new DocumentoDAO().getLibroCajaDocumento(documento, codigopaciente);


        }

        private void RadContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DocumentoEntity item = (DocumentoEntity)this.gridDocumento.SelectedItem;
            if (item == null)
            {
                MenuContext_Carta.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (item.Carta != "")
                {
                    MenuContext_Carta.Visibility = Visibility.Visible;
                    MenuItemAbrirCarta.Header = "Abrir Carta: " + item.Carta;
                }
                else
                    MenuContext_Carta.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItemAbrir_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            DocumentoEntity item = (DocumentoEntity)this.gridDocumento.SelectedItem;
            if (item != null)
            {
                var _cart = new CartaDAO().getCartaxCodigo(item.Carta, paciente);
                var detalle = new CartaDAO().getDetalleCartaxCodigo(item.Carta, paciente);
                if (_cart != null)
                {
                    new CartaDAO().insertLog(item.Carta.ToString(), this.session.shortuser, (int)Tipo_Log.Lectura, "Se abrió la proforma N° " + item.Carta.ToString());
                    frmCarta.frmCarta gui = new frmCarta.frmCarta();
                    gui.cargarGUI(session, true);
                    gui.setCartaGarantia(_cart, detalle,false);
                    gui.Show();                    
                    
                }
            }
        }
    }
}
