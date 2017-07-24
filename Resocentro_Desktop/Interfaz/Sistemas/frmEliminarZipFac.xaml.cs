using Resocentro_Desktop.DAO;
using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Lógica de interacción para frmEliminarZipFac.xaml
    /// </summary>
    public partial class frmEliminarZipFac : Window
    {
        MySession session;
        public frmEliminarZipFac()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session)
        {
            if (session.sucursales.ToList().Count > 0)
            {
                this.session = session;
                //cargamos los combos segun usuario
                cbosede.ItemsSource = new UtilDAO().getSucursales(session.sucursales).OrderBy(x => x.codigoInt);
                cbosede.SelectedValuePath = "codigoInt";
                cbosede.DisplayMemberPath = "nombreShort";
                cbosede.SelectedIndex = 0;

            }
            else
            {
                this.Close();
                MessageBox.Show("No tiene ninguna sucursal asignada", "ERROR");
            }
        }
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            List<DOCUMENTO> doc = new List<DOCUMENTO>(); ;
            int codigosede = int.Parse(cbosede.SelectedValue.ToString());
            int unidad = codigosede / 100;
            int sede = codigosede - (unidad * 100);
            string numdoc = txtDocumento.Text.Trim();
            string tipodocumento = cboTipoDocumento.Text.Substring(0, 2);
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                doc = db.DOCUMENTO.Where(x => x.codigounidad == unidad && x.codigosucursal == sede && x.numerodocumento == numdoc && x.tipodocumento == tipodocumento).ToList();
                if (doc.Count > 1)
                {
                    MessageBox.Show("Se encontraron demasiadas conincidencias avise a sistemas");
                    return;
                }
                else if (doc.Count == 0)
                {
                    MessageBox.Show("No se encontraron demasiadas conincidencias ");
                    return;
                }
            }
            int paciente = doc.First().codigopaciente;
            string pathMain = Tool.PathDocumentosFacturacion;
            string pathXML = System.IO.Path.GetTempPath() + paciente.ToString();
            string pathZIP = pathMain + paciente.ToString() + "\\" + "\\ZIP";
            string pathPDF = pathMain + paciente.ToString() + "\\" + "\\PDF";
            string pathRESULT = pathMain + paciente.ToString() + "\\" + "\\RESULT";
            string pathCODEBAR = pathMain + paciente.ToString() + "\\" + "\\PDF417";
            string pathBAJA = pathMain + "\\BAJA";
            string pathRESUMEN = pathMain + "\\RESUMEN";
            string rucEmisor = unidad == 1 ? "20297451023" : "20101849831";
            string filename = rucEmisor + "-" + tipodocumento + "-" + txtDocumento.Text;



            try
            {
                if (File.Exists(pathZIP + "\\" + filename + ".zip"))
                    File.Delete(pathZIP + "\\" + filename + ".zip");

                if (File.Exists(pathRESULT + "\\" + "R-" + filename + ".zip"))
                    File.Delete(pathRESULT + "\\" + "R-" + filename + ".zip");

                if (File.Exists(pathCODEBAR + "\\" + filename + ".jpeg"))
                    File.Delete(pathCODEBAR + "\\" + filename + ".jpeg");

                if (File.Exists(pathPDF + "\\" + filename + ".pdf"))
                    File.Delete(pathPDF + "\\" + filename + ".pdf");


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
