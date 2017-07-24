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

namespace Resocentro_Desktop.Interfaz.Cartas
{
    /// <summary>
    /// Lógica de interacción para frmLogCarta.xaml
    /// </summary>
    public partial class frmLogCarta : Window
    {
        public frmLogCarta()
        {
            InitializeComponent();
        }

        public void cargarGUI(string codigocarta)
        {
            using(DATABASEGENERALEntities db = new DATABASEGENERALEntities()){
                gridLog.ItemsSource = db.CG_Log.Where(x => x.codigo == codigocarta).ToList();
                gridAudit.ItemsSource = (from ac in db.AUDITORIA_CARTAS_GARANTIA join u in db.USUARIO on ac.codigousuario equals u.codigousuario where ac.numerodecarta==codigocarta select new { ac.fecha, codigousuario = u.ShortName, ac.mensaje }).ToList();
            }
        }
    }
}
