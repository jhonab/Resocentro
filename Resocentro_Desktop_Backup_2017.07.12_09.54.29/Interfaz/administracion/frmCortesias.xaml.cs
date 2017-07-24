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
using System.Windows.Threading;

namespace Resocentro_Desktop.Interfaz.administracion
{
    /// <summary>
    /// Lógica de interacción para frmCortesias.xaml
    /// </summary>
    public partial class frmCortesias : Window
    {
        MySession session;
        List<HCCita> lista;
        public frmCortesias()
        {
            InitializeComponent();
        }

        private void gridCitas_RowEditEnded(object sender, Telerik.Windows.Controls.GridViewRowEditEndedEventArgs e)
        {
            HCCita det = (HCCita)e.EditedItem;

            var _item = lista.SingleOrDefault(x => x.codigoexamencita == det.codigoexamencita);
            if (_item != null)
            {
                if (det.isCortesia)
                {
                    det.isDescuento = false;
                    det.monto_descuento = 0;
                    det.por_descuento = 0;
                }
                if (det.isDescuento)
                    det.isCortesia = false;
                else
                {
                    det.monto_descuento = 0;
                    det.por_descuento = 0;
                }

                _item = det;
                refreshEstudios(det.num_cita);
            }
        }

        public async void cargarGUI(MySession session, int numerocita, int codigopaciente)
        {
            this.session = session;
            await new EAtencionDAO().getHclinicaPacienteCita(codigopaciente).ContinueWith((_result) =>
            {
                lista = _result.Result;
                refreshEstudios(numerocita);
            });

        }

        private void refreshEstudios(int numerocita)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                gridCitas.ItemsSource = lista.Where(x => x.num_cita == numerocita && x.estado!="X").ToList();
            }
                ));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in lista)
            {

                if (item.isDescuento && item.por_descuento == 0 )
                {
                    MessageBox.Show("ingrese el porcentaje o el monto de descuento");
                    return;
                }
                if (item.isCortesia && double.Parse(item.cobertura.Replace("%",""))>0)
                {
                    MessageBox.Show("No se puede dar cortesia a coaseguro");
                    return;
                }

            }
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                foreach (var item in lista)
                {
                    var ec = db.EXAMENXCITA.SingleOrDefault(x => x.codigoexamencita == item.codigoexamencita);
                    ec.isCortesia = item.isCortesia;
                    ec.isDescuento = item.isDescuento;
                    ec.monto_descuento = Convert.ToDecimal(item.monto_descuento.ToString());
                    ec.por_descuento = Convert.ToDecimal(item.por_descuento.ToString());
                    ec.usu_cortesia = session.codigousuario;
                    ec.fec_cortesia = Tool.getDatetime();
                    db.SaveChanges();
                }
                MessageBox.Show("Se actualizo correctamente");
                this.DialogResult = true;
            }
        }





    }
}
