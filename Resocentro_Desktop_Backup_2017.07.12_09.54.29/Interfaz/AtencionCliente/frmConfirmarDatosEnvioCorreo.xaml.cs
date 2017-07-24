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
    /// Lógica de interacción para frmConfirmarDatosEnvioCorreo.xaml
    /// </summary>
    public partial class frmConfirmarDatosEnvioCorreo : Window
    {
        MySession session;
        CitarxConfirmar cita;
        string contraste;
        string sedacion;
        List<DetalleCita> LstEstudios;
        SUCURSAL sede;
        public frmConfirmarDatosEnvioCorreo()
        {
            InitializeComponent();
        }

        public void cagarGUI(MySession session, CitarxConfirmar cita, string contraste, string sedacion)
        {
            this.session = session;
            this.cita = cita;
            this.contraste = contraste;
            this.sedacion = sedacion;
            cargarDatos();
        }

        private void cargarDatos()
        {
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == cita.codigopaciente);
                txtcorreopaciente.Text = paciente.email;
                txtobservaciones.Text = @"Traer su documento de identidad y llegar 15 minutos antes de su cita.Es indispensable presentar su orden médica.";
                LstEstudios = (new CitaDAO().getDetalleCita(cita.numerocita.ToString()));
                var estudio = LstEstudios.FirstOrDefault();
                if (estudio != null)
                {
                    var idsede = int.Parse(estudio.codigoestudio.Substring(1, 2));
                    sede = db.SUCURSAL.SingleOrDefault(x => x.codigosucursal == idsede && x.codigounidad == 1);

                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var paciente = db.PACIENTE.SingleOrDefault(x => x.codigopaciente == cita.codigopaciente);
                paciente.email = txtcorreopaciente.Text;
                
                Confirmacion_Cita c = new Confirmacion_Cita();
                c.numerocita = cita.numerocita;
                c.fecha = Tool.getDatetime(); ;
                //tipo 1 Correo, Tipo 0 Llamada
                c.tipo = 1;
                c.codigousuario = session.codigousuario;
                c.telefono = "";
                c.email = txtcorreopaciente.Text;
                c.comentarios = "Preración: " + txtpreparacion.Text + "\nObservacion: " + txtobservaciones.Text;
                db.Confirmacion_Cita.Add(c);
                db.SaveChanges();
                #region mesaje
                string titulo = @"<TABLE class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'>
     <TBODY>
      <TR>
       <TD class='padded'  style='WORD-WRAP: break-word; PADDING-LEFT: 0px; FONT-SIZE: 12px; FONT-FAMILY: Tahoma,sans-serif; VERTICAL-ALIGN: top;TEXT-ALIGN: left;'>    
                              <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>" + paciente.sexo == "M" ? "Estimado: " : @"Estimada: " + paciente.nombres.ToUpper() + @"</h1>
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
         <a href='#' style='TEXT-DECORATION: none; FONT-FAMILY: Tahoma,sans-serif;float: right;font-size: 15px;padding-right: 32px;'>N° Cita: " + cita.numerocita + @"</a>
       </TD>
      </TR>
     </TBODY>
    </TABLE>"
    ,

                    cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 14px;line-height: 25px;Margin-bottom: 24px'>A continuación, encontrará el detalle de su cita:<br/><b>Lugar: </b>" + sede.nombreweb + " (" + sede.direccion + ").<br/> <b>Estudio(s):</b>", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";

                string detalle = "", tabla = @"

<TABLE  class=contents style='WIDTH: 100%; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-SPACING: 0'><THEAD>
<TR>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Hora</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Estudio</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Pago</TH>
<TH style='BORDER-TOP: #ddd 1px solid; FONT-FAMILY: sans-serif !important; BORDER-RIGHT: #ddd 1px solid; VERTICAL-ALIGN: bottom; BORDER-BOTTOM: #ddd 1px solid; COLOR: #303641; PADDING-BOTTOM: 8px; PADDING-TOP: 8px; PADDING-LEFT: 8px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 8px'>Moneda</TH></TR></THEAD>
<TBODY>
{0}
</TBODY></TABLE>";
                foreach (var item in LstEstudios.OrderBy(x => x.fecha).ToList())
                {
                    detalle += "<TR><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.fecha.ToString("dd/MM/yy hh:mm tt") + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.nombreestudio + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px;text-align:center!important;'>" + item.precioneto + "</TD><TD style='FONT-SIZE: 12px; BORDER-TOP: #ddd 1px solid; BORDER-RIGHT: #ddd 1px solid; BORDER-BOTTOM: #ddd 1px solid; PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 4px; BORDER-LEFT: #ddd 1px solid; PADDING-RIGHT: 4px'>" + item.moneda + "</TD><TD></TR>";
                }
                tabla = string.Format(tabla, detalle);
                if (contraste != "" || cita.sedacion)
                {
                    tabla += "<b>Adicionales:</b><br/>";
                    if (contraste != "")
                        tabla += "Contraste: " + contraste + "<br/>";
                    if (cita.sedacion)
                        tabla += "Sedación: " + sedacion + "<br/>";
                }
                tabla += "<b>Entrega de resultados:</b> " + txtresultados.Text + @".<br/>";
                if (txtpreparacion.Text != "")
                    tabla += "<b>Preparacion para la el estudio:</b><br/>" + txtpreparacion.Text + ".<br/>";
                if (txtobservaciones.Text != "")
                    tabla += "<b>Observaciones: </b><br/>" + txtobservaciones.Text + "<br/>";

                tabla += "<b>Ubicación:</b><br/><a href='" + sede.urlGoogleMaps + "'><img style='text-align:center!important;'  width='80%'src='http://extranet.resocentro.com:5050/PaginaWeb/sedesgooglemaps/" + sede.imageName + "'/></a>Para mayor informacion visite nuestra página web <a href='http://www.resocentro.com'>www.resocentro.com</a><br/></div>";
                tabla += "</p>";
                #endregion
                new Tool().sendCorreo("Confirmación de Cita", txtcorreopaciente.Text, "", new Tool().getCuerpoEmail(img, titulo, cuerpo + "" + tabla), "");
                DialogResult = true;
                MessageBox.Show("Se envio el mesaje con exito");
            }
        }
    }
}
