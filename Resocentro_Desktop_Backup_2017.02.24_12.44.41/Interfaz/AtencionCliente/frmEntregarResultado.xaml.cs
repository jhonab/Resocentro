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
    /// Lógica de interacción para frmEntregarResultado.xaml
    /// </summary>
    public partial class frmEntregarResultado : Window
    {
        ResultadosEntity item;
        MySession session;
        public frmEntregarResultado()
        {
            InitializeComponent();
        }
        public void cargarGUI(MySession session, ResultadosEntity item)
        {
            this.Title = "Entregar Resultado - " + item.estudio.ToUpper();
            lblestudio.Content = item.estudio.ToUpper();
            txtentregado.Text = item.paciente;
            txtdocumento.Text = item.dni;
            txtdireccion.Text = item.direccion;
            this.session = session;
            this.item = item;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtfotos.Text != "" && txtplacas.Text != "")
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    RESULTADODELIVERY re = new RESULTADODELIVERY();
                    re.numeroatencion = item.numeroatencion;
                    re.codigodistrito = item.codigopaciente;
                    re.direcciondelivery = txtdireccion.Text;
                    re.personacontacto = txtentregado.Text;
                    re.telefonodelivery = item.telefono;
                    re.cantidadfoto = int.Parse(txtfotos.Text);
                    re.cantidadplaca = int.Parse(txtplacas.Text);
                    re.fechasalida = DateTime.Now;
                    re.observacion = txtObs.Text;
                    re.horasalida = DateTime.Now;
                    re.tipoentrega = "Normal";
                    re.codigousuario = session.codigousuario;
                    re.firma = null;
                    db.RESULTADODELIVERY.Add(re);
                    db.SaveChanges();

                    DialogResult = true;
                }
            }
            else
            {

            }
        }
    }
}
