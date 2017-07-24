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

namespace Resocentro_Desktop.Interfaz.frmCarta
{
    /// <summary>
    /// Lógica de interacción para frmCorreoSolicitu.xaml
    /// </summary>
    public partial class frmCorreoSolicitu : Window
    {
        int idSolicitud;
        MySession session;
        public frmCorreoSolicitu()
        {
            InitializeComponent();
        }
        public void cargarHistorial(MySession session, int idSolicitud, bool visor)
        {
            this.idSolicitud = idSolicitud;
            this.session = session;
            var solicitudes = new CartaDAO().getSolicitudesCarta(idSolicitud);

            foreach (var item in solicitudes.OrderByDescending(x => x.idDetSol))
            {
                txtcomentarioSolicitud.Text += item.usuario + " (" + item.fec_reg.Value.ToString("dd/MM/yy hh:mm") + ")\n\t" + item.comentarios + "\n";
            }
            if (visor)
            {
                gridResponder.Visibility = Visibility.Visible;
                btnEnviar.Visibility = Visibility.Visible;
                btnCerrar.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridResponder.Visibility = Visibility.Collapsed;
                btnEnviar.Visibility = Visibility.Collapsed;
                btnCerrar.Visibility = Visibility.Visible;
            }
            txtrespuesta.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtrespuesta.Text != "")
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    CG_SolicituCarta_Historial item = new CG_SolicituCarta_Historial();
                    item.idSolicitud = idSolicitud;
                    item.usuario = session.shortuser + " Carta";
                    item.comentarios = txtrespuesta.Text;
                    item.fec_reg = Tool.getDatetime(); ;
                    db.CG_SolicituCarta_Historial.Add(item);
                    db.SaveChanges();

                    new CartaDAO().insertLog(idSolicitud.ToString(), session.codigousuario, (int)Tipo_Log.Update, "Se envio correo al paciente ");

                    CG_SolicitudCarta cg_carta = new CG_SolicitudCarta();
                    cg_carta.idSolicitud = idSolicitud;
                    cg_carta.estado = (int)Estado_SolicitudTramite.Espera;
                    cg_carta.numeroproforma = null;
                    new CartaDAO().updateSolicitudCarta(cg_carta, session.codigousuario);
                    DialogResult = true;

                   
                    var _solicitud=db.CG_SolicitudCarta.Where(x=> x.idSolicitud==idSolicitud).SingleOrDefault();
                    if(_solicitud!=null){
                         #region msj
                    string titulo = @"<TABLE style='BORDER-COLLAPSE: collapse; BORDER-SPACING: 0'>
 <TBODY>
  <TR >
   <TD  style='WIDTH: 80%;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD class='padded'  style='WORD-WRAP: break-word; PADDING-LEFT: 0px; FONT-SIZE: 12px; FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: left;'>
        <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " +
                                                   _solicitud.nombres
                                      + @"</h1>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
   <TD  style='WIDTH: 20%; VERTICAL-ALIGN: top; PADDING-BOTTOM: 32px;'>
    <TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD style='WORD-WRAP: break-word;FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: right;'>
         <a href='http://www.resocentro.com/Paciente/SolicitudCarta?solicitud=" + _solicitud.idSolicitud + @"' style='TEXT-DECORATION: none; FONT-FAMILY: Tahoma,sans-serif;float: right;font-size: 15px;'>N° Solicitud: " + item.idSolicitud + @"</a>
       </TD>
      </TR>
     </TBODY>
    </TABLE>
   </TD>
  </TR>
 </TBODY>
</TABLE>", cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 15px;line-height: 25px;Margin-bottom: 24px'>
Hemos respondido a su solicitud, por favor ingrese a nuestra web en la sección <a href='http://www.resocentro.com/Carta'>Carta de Garantia</a> con el número de consulta (" + item.idSolicitud + @") o acceda mediante el siguiente <a href='http://www.resocentro.com/Paciente/SolicitudCarta?solicitud=" + _solicitud.idSolicitud + @"'>Enlace</a>.
</p>
", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";
                    #endregion
                    new Tool().sendCorreo("Respuesta a su Solicitud", _solicitud.email, "", new Tool().getCuerpoEmail(img, titulo, cuerpo), "");
                    }
                    

                }
            else
                MessageBox.Show("Ingrese su respuesta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
