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

namespace Resocentro_Desktop.Interfaz.Sistemas
{
    /// <summary>
    /// Lógica de interacción para frmSendFacturas.xaml
    /// </summary>
    public partial class frmSendFacturas : Window
    {
        MySession session;
        List<SendFacturasOffline> lista;
        public frmSendFacturas()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            this.session = session;
            cboempresa.ItemsSource = new UtilDAO().getEmpresa();
            cboempresa.SelectedValuePath = "codigounidad";
            cboempresa.DisplayMemberPath = "nombre";
            cboempresa.SelectedIndex = 0;
            lista = new List<SendFacturasOffline>();

        }
        private void btnbuscar_Click(object sender, RoutedEventArgs e)
        {
            lista = new CobranzaDAO().getFacturasPendientesSendSunat(cboempresa.SelectedValue.ToString(), txtfecha.SelectedDate.Value.ToShortDateString());
            listardocumentos();
        }

        private void listardocumentos()
        {
            griddocumento.ItemsSource = null;
            griddocumento.ItemsSource = lista;
        }

        private void btnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Count > 0)
            {
                var dao = new ServiceSunat();
                var dao1 = new CobranzaDAO();
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    foreach (var item in lista)
                    {
                        try
                        {
                            if (item.resultado == "PENDIENTE")
                            {
                                string pathCDR = dao.sendBill(item.empresa, item.paciente, System.IO.Path.GetFileName(item.filename).Split('.')[0]);

                                string msjResutlado = "";
                                if (dao1.verificarCDR(pathCDR, out msjResutlado))
                                {
                                    
                                    var doc=db.DOCUMENTO.SingleOrDefault(x => x.numerodocumento == item.documento && x.codigounidad == item.empresa);
                                    if (doc != null)
                                    {
                                        item.resultado = "EXITO";
                                        doc.isSendSUNAT = true;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                    item.resultado = msjResutlado.Trim();
                            }
                        }
                        catch (Exception ex)
                        {
                            listardocumentos();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                listardocumentos();
            }
        }
    }
}
