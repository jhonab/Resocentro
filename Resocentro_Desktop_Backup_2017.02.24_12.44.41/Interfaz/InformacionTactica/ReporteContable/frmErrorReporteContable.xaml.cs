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

namespace Resocentro_Desktop.Interfaz.InformacionTactica
{
    /// <summary>
    /// Lógica de interacción para frmErrorReporteContable.xaml
    /// </summary>
    public partial class frmErrorReporteContable : Window
    {
        List<RGVFACTU_Result> lstDetalle;
        public frmErrorReporteContable()
        {
            InitializeComponent();
        }
        public void cargarGUI(List<RGVFACTU_Result> lstDetalle, List<RGVCAFAC_Result> lstRepetidas, List<RGVCAFAC_Result> lstnoConcuerda, List<RGVCAFAC_Result> lstPendientes)
        {
            grid_NConcuerdan.ItemsSource = lstnoConcuerda;
            grid_repetidos.ItemsSource = lstRepetidas;
            grid_pendiente.ItemsSource = lstPendientes;
            this.lstDetalle = lstDetalle;
        }
        private void grid_Factura_RowActivated(object sender, Telerik.Windows.Controls.GridView.RowEventArgs e)
        {
            RGVCAFAC_Result item = (RGVCAFAC_Result)e.Row.DataContext;
            if (item != null)
            {
                grid_Detalle.ItemsSource = lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).ToList();
                lbldetalle_doc.Content = "Tipo Doc.:  " + item.TDORGV + "   N° Doc.:  " + item.NDORGV + "   Total S/.: " + item.TOTRGVS + "   Total $.: " + item.TOTRGVD + "   Diferencia S/.: " + (item.TOTRGVS - lstDetalle.Where(x => x.CODEAUX == item.CODEAUX).Sum(x => x.TOTRGVS));
            }
            else
                lbldetalle_doc.Content = "Detalle Documento";

        }
    }
}
