using Microsoft.Reporting.WinForms;
using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using Resocentro_Desktop.Interfaz.Caja.impresion;
using Resocentro_Desktop.Interfaz.frmUtil;
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
    /// Lógica de interacción para frmGenearCaraturaFactura.xaml
    /// </summary>
    public partial class frmGenearCaraturaFactura : Window
    {
        MySession session;
        List<DocumentosRecepcion> lista;
        FacturaLote fac;
        EmpresaFacturacion empresa;

        public frmGenearCaraturaFactura()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cbosucursal.ItemsSource = new UtilDAO().getEmpresa();
            cbosucursal.SelectedValuePath = "codigounidad";
            cbosucursal.DisplayMemberPath = "nombre";
            cbosucursal.SelectedIndex = 0;
            empresa = new EmpresaFacturacion();
            lista = new List<DocumentosRecepcion>();
            fac = new FacturaLote();
        }


        private void btnBuscarAseguradora_Click(object sender, RoutedEventArgs e)
        {
            frmSearchCompania gui = new frmSearchCompania();
            gui.cargarGUI(session);
            gui.ShowDialog();
            if (gui.DialogResult.Value)
            {
                empresa.ruc = gui.empresa.ruc;
                empresa.razonsocial = gui.empresa.razonsocial;
                empresa.direccion = gui.empresa.direccion;
                lblAseguradora.Content = empresa.razonsocial;
            }
            else
            {
                empresa = new EmpresaFacturacion();
                lblAseguradora.Content = "";
            }
        }

        private void txtcorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (txtserie.Text != "" && txtcorrelativo.Text != "")
                {
                    var doc = new CobranzaDAO().getDocumentoCaratula(txtserie.Text + "-" + txtcorrelativo.Text, cbosucursal.SelectedValue.ToString());
                    if (doc.numerodocumento != null && lista.Where(x => x.numerodocumento == doc.numerodocumento).ToList().Count == 0)
                    {
                        lista.Add(doc);
                        listardocumentos();
                    }
                    else
                    {
                        MessageBox.Show("No se encontro ningun documento correcto.");
                    }
                }
        }

        private void listardocumentos()
        {
            griddocumentos.ItemsSource = null;
            griddocumentos.ItemsSource = lista.OrderBy(x => x.serie).ThenBy(x => x.correlativo).ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (empresa.razonsocial != null && empresa.razonsocial != "")
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    fac = new FacturaLote();
                    fac.empresa = empresa.razonsocial;
                    fac.fecha = Tool.getDatetime();
                    fac.usuario = session.codigousuario;
                    db.FacturaLote.Add(fac);
                    foreach (var item in lista)
                    {
                        FacturaDetalleLote det = new FacturaDetalleLote();
                        det.idLote = fac.idLote;
                        det.numerodocumento = item.numerodocumento;
                        det.fecha = item.fecha_emision;
                        det.monto = Convert.ToDecimal(item.total.ToString());
                        db.FacturaDetalleLote.Add(det);
                    }
                    db.SaveChanges();
                    imprimir();
                    MessageBox.Show("Se generó la carátula");
                }
            }
        }

        private void imprimir()
        {
            try
            {

                frmCaratula gui = new frmCaratula();
                CaratulaEntity item = new CaratulaEntity();
                item.detalle = lista.OrderBy(x => x.serie).ThenBy(x => x.correlativo).ToList();
                item.empresa = empresa.razonsocial.ToString();
                item.lote = fac.idLote.ToString();
                gui.cargarGUI(item);



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            imprimir();
        }

        private void griddocumentos_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            if (MessageBox.Show("¿Desea eliminar la factura?", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DocumentosRecepcion item = (DocumentosRecepcion)e.Row.DataContext;
                lista.Remove(item);
                listardocumentos();
            }



        }
    }
}
