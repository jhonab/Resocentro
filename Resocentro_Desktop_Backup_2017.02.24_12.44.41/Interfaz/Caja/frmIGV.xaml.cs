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

namespace Resocentro_Desktop.Interfaz.Caja
{
    /// <summary>
    /// Lógica de interacción para frmIGV.xaml
    /// </summary>
    public partial class frmIGV : Window
    {
        MySession session;
        public frmIGV()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var fecha = Tool.getDatetime();
                foreach (var item in db.TIPOCAMBIO.Where(x => x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList())
                {
                    item.igv = float.Parse(txtigv.Value.Value.ToString());
                    item.preciodeventa = float.Parse(txtventa.Value.Value.ToString());
                    item.preciodecompra = float.Parse(txtcompra.Value.Value.ToString());
                    item.FecRev = fecha;
                    db.SaveChanges();
                }
            }
        }

        public void cargarGUI(MySession session)
        {
            this.session = session;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var fecha = Tool.getDatetime();
                lblfecha.Content = "Fecha: \t" + fecha.ToShortDateString();
                foreach (var item in db.TIPOCAMBIO.Where(x => x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList())
                {
                    txtigv.Value = item.igv;
                    txtventa.Value = item.preciodeventa;
                    txtcompra.Value = item.preciodecompra;
                }
            }
        }
    }
}
