using Resocentro_Desktop.DAO;
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
    /// Lógica de interacción para frmVerificarBaja.xaml
    /// </summary>
    public partial class frmVerificarBaja : Window
    {
        MySession session;
        public frmVerificarBaja()
        {
            InitializeComponent();
        }

        public void cargarGUI(MySession session){
            this.session= session;
            if (session.sucursales.ToList().Count > 0)
            {
                this.session = session;
                //cargamos los combos segun usuario
                cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
                cbosede.SelectedValuePath = "codigoInt";
                cbosede.DisplayMemberPath = "nombreShort";
                cbosede.SelectedIndex = 0;
                cboTipoDocumento.SelectedIndex = 0;
                
            }
            else
            {
                this.Close();
                MessageBox.Show("No tiene ninguna sucursal asignada", "ERROR");
            }
        }
        private void openFile(string path)
        {
            System.Diagnostics.Process printJob = new System.Diagnostics.Process();
            printJob = new System.Diagnostics.Process();
            printJob.StartInfo.FileName = path;
            printJob.StartInfo.UseShellExecute = true;
            printJob.Start();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtticket.Text.ToString() != "")
            {
                try
                {
                    //lblticket.Content = "201700782991965";
                    string estado = "";

                    if (chkomitir__sunat.IsChecked.Value)
                    {
                        if (MessageBox.Show("Esta seguro de omitir la validación con sunat para anular este documento. Esto podrá causar problemas tributarios", "ADVERTENCIA", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                            estado = "0";
                        else
                            return;
                    }
                    else
                        estado = new ServiceSunat().getStatus(int.Parse(cbosede.SelectedValue.ToString().Substring(0, 1)), txtticket.Text, txtdocumento.Text);

                    if (estado == "0")
                    {
                        if (new CobranzaDAO().anularDocumento(txtdocumento.Text, cboTipoDocumento.Text.Substring(0, 2), cbosede.SelectedValue.ToString(), "ERROR", session))
                        {
                            DOCUMENTO doc = new DOCUMENTO();
                            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                            {
                                try
                                {
                                    var empresa = int.Parse(cbosede.SelectedValue.ToString().Substring(0, 1));
                                    doc = db.DOCUMENTO.SingleOrDefault(x => x.numerodocumento == txtdocumento.Text && x.codigounidad == empresa);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    return;
                                }

                            }
                            new CobranzaDAO().addWatermakerAnulado(doc.pathFile);
                            MessageBox.Show("Se ANULO con exito el documento en SUNAT");                            
                        }
                    }
                    else
                    {
                        if (estado.Contains("98"))
                            MessageBox.Show("ESTADO: EN PROCESO DE VALIDACION\nvuelva a verificar");
                        else
                        {
                            openFile(Tool.PathDocumentosFacturacion + "\\BAJA\\RESPUESTA-" + txtticket.Text.ToString() + ".zip");
                            MessageBox.Show("NO se anulo documento\n ESTADO: PROCESO CON ERRORES \n\nVERIFIQUE CON SISTEMAS");
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
