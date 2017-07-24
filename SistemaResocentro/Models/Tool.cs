using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

public static class JavaScriptConvert
{
    public static IHtmlString SerializeObject(object value)
    {
        using (var stringWriter = new StringWriter())
        using (var jsonWriter = new JsonTextWriter(stringWriter))
        {
            var serializer = new JsonSerializer
            {
                // Let's use camelCasing as is common practice in JavaScript
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // We don't want quotes around object names
            jsonWriter.QuoteName = false;
            serializer.Serialize(jsonWriter, value);

            return new HtmlString(stringWriter.ToString());
        }
    }
}
namespace SistemaResocentro.Models
{
    public class Variable
    {
        public string getUrlpath { get { return @"\\ServerWeb\Perfiles\"; } }
        public string getUrlpathImgColaborador { get { return @"\\ServerWeb\PerfilesRRHH\"; } }
        public static string PathDocumentosAdjuntos { get { return @"\\serverweb\DocumentoAjunto\"; } }
        public static string PathAdjuntosMantenimiento { get { return @"\\serverweb\AdjuntosMantenimiento\"; } }
        public int preferencial { get { return 6; } }
        public int recogerresultado { get { return 7; } }
        public List<Object> getVacio()
        {
            return new List<Object>
            {
            };
        }
        public List<Object> getMotivosPlanificacion()
        {
            return new List<Object> { 
                 new { nombre = "Captación"  },
                 new { nombre = "Mantenimiento"  },
                 new { nombre = "Entrega Materiales"  },
                 new { nombre = "Entrega Producción"  },
                 new { nombre = "Oficina"  }                       
                    };
        }
        public List<Object> getTipoAfiliacion()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Regular" },
                       new { codigo=3,nombre ="Potestativa (Independiente)" },
                       new { codigo=4,nombre ="Complementario" },
                       new { codigo=5,nombre ="PEAS" }
                    };
        }
        public int calcularedad(DateTime birthday)
        {
            DateTime now = DateTime.Today;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age)) age--;

            return age;
        }
        public List<Object> getTipoDocumentoPago()
        {
            return new List<Object> { 
                       new { codigo="N",nombre ="Ninguno" },
                       new { codigo="F",nombre ="Factura" },
                       new { codigo="R",nombre ="Recibo" }
                    };
        }
        public List<Object> getEstadoCarta()
        {
            return new List<Object> { 
                       new { nombre ="TRAMITADA" },
                       new { nombre ="APROBADA" },
                       new { nombre ="RECHAZADA" },
                       new { nombre ="OBSERVADA" },
                       new { nombre ="SIN TRAMITAR" },
                       new { nombre ="AVERIADA" },
                       new { nombre ="CITADA" },
                       new { nombre ="CADUCADA" }
                    };
        }
        public List<Object> getTipoDocumento()
        {
            return new List<Object> { 
                       new { codigo=0,nombre ="DNI" },
                       new { codigo=1,nombre ="Carnet de Extranjeria" },
                       new { codigo=2,nombre ="RUC" },
                       new { codigo=3,nombre ="Pasaporte" },
                       new { codigo=4,nombre ="Cedula Diplomatica de Identidad" },
                       new { codigo=5,nombre ="Otros" }
                    };
        }
        public List<Object> getSexo()
        {
            return new List<Object> { 
                       new { codigo="M",nombre ="Masculino" },
                       new { codigo="F",nombre ="Femenino" }
                    };
        }
        public List<Object> getImportancia()
        {
            return new List<Object> { 
                       new { nombre ="Baja" },
                       new { nombre ="Normal" },
                       new { nombre ="Alta" },
                       new { nombre ="Muy Alta" }
                    };
        }
        public List<Object> getTipoDocumentoSUNAT()
        {
            return new List<Object> { 
                       new { codigo="1",nombre ="DNI" },
                       new { codigo="6",nombre ="RUC" },
                       new { codigo="4",nombre ="Carnet de Extranjeria" },
                       new { codigo="7",nombre ="Pasaporte" },
                       new { codigo="A",nombre ="Cedula Diplomatica" },
                       new { codigo="0",nombre ="DOC. TRIB. NO. DOM. SIN. RUC" }
                    };
        }
        public List<Object> getEstadoEquipoMantenimiento()
        {
            return new List<Object> { 
                       new { nombre ="Operativo Totalmente" },
                       new {nombre ="Operativo con Restricciones" },
                       new { nombre ="Inoperativo" }
                    };
        }
        public List<Object> getTipoInstitucionMedica()
        {
            return new List<Object> { 
                       new { nombre ="CENTRO MÉDICO" },
                       new { nombre ="CLÍNICA" },
                       new { nombre ="CLUB" },
                       new { nombre ="CONSULTORIO PARTICULAR" },
                       new { nombre ="HOSPITAL" },
                       new { nombre ="INSTITUTO" },
                       new { nombre ="OTROS" },
                       new { nombre ="POLICLÍNICO" },
                    };
        }
        public List<Object> getAplicativos()
        {
            return new List<Object> { 
                       new { nombre ="Módulo Carta Garantía" },
                       new { nombre ="Módulo Atención al Cliente" },
                       new { nombre ="Módulo Tecnólogo" },
                       new { nombre ="Módulo Representantes Médicos" },
                       new {nombre ="SAIR" },
                       new {nombre ="Otro" }
                    };
        }
        public List<Object> getEstadoIncidente()
        {
            return new List<Object> { 
                       new {codigo="N" ,nombre ="Nuevo" },
                       new {codigo="L" ,nombre ="Leído" },
                       new {codigo="P" ,nombre ="Pendiente" },
                       new {codigo="T" ,nombre ="Terminado" }
                    };
        }
        public List<Object> getEstadoCivil()
        {
            return new List<Object> { 
                       new {codigo ="Soltero(a)" },
                       new {codigo ="Casado(a)" },
                       new {codigo ="Viudo(a)" },
                       new {codigo ="Divorciado(a)" }
                    };
        }
        public List<Object> getTipoGastoPlanificacion()
        {
            return new List<Object> { 
                 new { codigo = "1" , nombre = "Gasto"  },
                 new { codigo = "2" , nombre = "Movilidad"  },
                 new { codigo = "3" , nombre = "Estacionamiento"  }
            };
        }
        public List<Object> getCategoriaCita()
        {
            return new List<Object> { 
                 new { nombre = "AMBULATORIO"  },
                 new {  nombre = "HOSPITALIZADO"  }
            };
        }
        public List<Object> getCategoriasGastoPlanificacion()
        {
            return new List<Object> { 
                 new { nombre = "Regalos"  },
                 new { nombre = "Desayuno"  },
                 new { nombre = "Almuerzo"  },
                 new { nombre = "Cenas"  },
                 new { nombre = "Utiles"  },
                 new { nombre = "Propinas"  }
            };
        }
        public int CantidadMinimaPlanificacion()
        {
            return 10;
        }
        public int DiasConcurrenciaPlanificacion()
        {
            return 15;
        }
        public string getClientName(string IP)
        {
            System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
            System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }
        public string getClientIP(string IP)
        {
            System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
            System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
            string ip = "";
            foreach (var ipAddress in GetIPHost.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
            {
                ip += ipAddress.ToString() + "/ ";
            }
            return ip;
        }
        public static DateTime getDatetime()
        {
            DateTime fecha = new DateTime(); ;
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    string query = "select getDate() fecha";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            fecha = Convert.ToDateTime((reader["fecha"]).ToString());
                        }
                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }
                }
            }

            return fecha;
        }
        public string getCuerpoEmail(string img, string titulo, string cuerpo)
        {
            img = img.Replace("'", "\"");
            titulo = titulo.Replace("'", "\"");
            cuerpo = cuerpo.Replace("'", "\"");

            return @"<!DOCTYPE html>

<html xmlns=" + '\u0022' + @"http://www.w3.org/1999/xhtml" + '\u0022' + @">
<head>
    <title></title>
    <meta http-equiv=" + '\u0022' + @"Content-Type" + '\u0022' + @" content=" + '\u0022' + @"text/html; charset=utf-8" + '\u0022' + @">   
    <!--[if gte mso 9]>
    <style>
      .column-top {
        mso-line-height-rule: exactly !important;
      }
    </style>
    <![endif]-->
    <meta name=" + '\u0022' + @"robots" + '\u0022' + @" content=" + '\u0022' + @"noindex,nofollow" + '\u0022' + @">
    <meta property=" + '\u0022' + @"og:title" + '\u0022' + @" content=" + '\u0022' + @"My First Campaign" + '\u0022' + @">
    <link href=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/social.min.css?h=A3DB20FCcapricorn" + '\u0022' + @" media=" + '\u0022' + @"screen,projection" + '\u0022' + @" rel=" + '\u0022' + @"stylesheet" + '\u0022' + @" type=" + '\u0022' + @"text/css" + '\u0022' + @">
</head>
<body style=" + '\u0022' + @"margin: 0;mso-line-height-rule: exactly;padding: 0;min-width: 100%;background-color: #ededed" + '\u0022' + @">
    <style type=" + '\u0022' + @"text/css" + '\u0022' + @">
   @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);.emb-editor-canvas,.wrapper,body{background-color:#ededed}.border{background-color:#dcdcdc}h1{color:#565656}.wrapper h1{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h1{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h1{line-height:44px}.two-col h1{line-height:34px}.three-col h1{line-height:26px}.wrapper .one-col-feature h1{line-height:58px}@media only screen and (max-width:620px){h1{line-height:44px!important}}h2{color:#555}.wrapper h2{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h2{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h2{line-height:34px}.two-col h2{line-height:26px}.three-col h2{line-height:22px}.wrapper .one-col-feature h2{line-height:50px}@media only screen and (max-width:620px){h2{line-height:34px!important}}h3{color:#555}.wrapper h3{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h3{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h3{line-height:26px}.two-col h3{line-height:24px}.three-col h3{line-height:20px}.wrapper .one-col-feature h3{line-height:40px}@media only screen and (max-width:620px){h3{line-height:26px!important}}ol,p,ul{color:#565656}.wrapper ol,.wrapper p,.wrapper ul{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper ol,.wrapper p,.wrapper ul{font-family:Roboto,Tahoma,sans-serif!important}}.one-col ol,.one-col p,.one-col ul{line-height:25px;Margin-bottom:25px}.two-col ol,.two-col p,.two-col ul{line-height:22px;Margin-bottom:22px}.three-col ol,.three-col p,.three-col ul{line-height:20px;Margin-bottom:20px}.wrapper .one-col-feature ol,.wrapper .one-col-feature p,.wrapper .one-col-feature ul{line-height:30px}.one-col-feature blockquote ol,.one-col-feature blockquote p,.one-col-feature blockquote ul{line-height:50px}@media only screen and (max-width:620px){ol,p,ul{line-height:25px!important;Margin-bottom:25px!important}}.image{color:#565656;font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.image{font-family:Roboto,Tahoma,sans-serif!important}}.wrapper a{color:#41637e}.wrapper a:hover{color:#30495c!important}.wrapper .logo div{color:#41637e;font-family:sans-serif}@media only screen and (min-width:0){.wrapper .logo div{font-family:Avenir,sans-serif!important}}.wrapper .logo div a{color:#41637e}.wrapper .logo div a:hover{color:#41637e!important}.wrapper .one-col-feature ol a,.wrapper .one-col-feature p a,.wrapper .one-col-feature ul a{border-bottom:1px solid #41637e}.wrapper .one-col-feature ol a:hover,.wrapper .one-col-feature p a:hover,.wrapper .one-col-feature ul a:hover{color:#30495c!important;border-bottom:1px solid #30495c!important}.wrapper .btn a{font-family:Tahoma,sans-serif;background-color:#41637e;color:#fff!important;outline-color:#41637e;text-shadow:0 1px 0 #3b5971}@media only screen and (min-width:0){.wrapper .btn a{font-family:Roboto,Tahoma,sans-serif!important}}.wrapper .btn a:hover{background-color:#3b5971!important;color:#fff!important;outline-color:#3b5971!important}.footer .padded,.preheader .title,.preheader .webversion{color:#737373;font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.footer .padded,.preheader .title,.preheader .webversion{font-family:Roboto,Tahoma,sans-serif!important}}.footer .padded a,.preheader .title a,.preheader .webversion a{color:#737373}.footer .padded a:hover,.preheader .title a:hover,.preheader .webversion a:hover{color:#4d4d4d!important}.footer .social .divider{color:#dcdcdc}.footer .social .social-text,.footer .social a{color:#737373}.wrapper .footer .social .social-text,.wrapper .footer .social a{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper .footer .social .social-text,.wrapper .footer .social a{font-family:Roboto,Tahoma,sans-serif!important}}.footer .social .social-text:hover,.footer .social a:hover{color:#4d4d4d!important}.image .border{background-color:#c8c8c8}.image-frame{background-color:#dadada}.image-background{background-color:#f7f7f7}
    </style>
    <center class=" + '\u0022' + @"wrapper" + '\u0022' + @" style=" + '\u0022' + @"display: table;table-layout: fixed;width: 100%;min-width: 620px;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%;background-color: #ededed" + '\u0022' + @">
        <table class=" + '\u0022' + @"gmail" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;width: 650px;min-width: 650px" + '\u0022' + @"><tbody><tr><td style=" + '\u0022' + @"padding: 0;vertical-align: top;font-size: 1px;line-height: 1px" + '\u0022' + @">&nbsp;</td></tr></tbody></table>
        <table class=" + '\u0022' + @"preheader centered" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto" + '\u0022' + @">
            <tbody>
                <tr>
                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;width: 602px" + '\u0022' + @">
                            <tbody>
                                <tr></tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>


        <table class=" + '\u0022' + @"border" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;font-size: 1px;line-height: 1px;background-color: #dcdcdc;Margin-left: auto;Margin-right: auto" + '\u0022' + @" width=" + '\u0022' + @"602" + '\u0022' + @">
            <tbody>
                <tr><td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @"></td></tr>
            </tbody>
        </table>

        <table class=" + '\u0022' + @"centered" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto" + '\u0022' + @">
            <tbody>
                <tr>
                    <td class=" + '\u0022' + @"border" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px" + '\u0022' + @"></td>
                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                        <table class=" + '\u0022' + @"one-col" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto;width: 600px;background-color: #ffffff;font-size: 14px;table-layout: fixed" + '\u0022' + @">
                            <tbody>
                                <tr>
                                    <td class=" + '\u0022' + @"column" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;text-align: left" + '\u0022' + @">

                                        <div class=" + '\u0022' + @"image" + '\u0022' + @" style=" + '\u0022' + @"font-size: 12px;Margin-bottom: 24px;mso-line-height-rule: at-least;color: #565656;font-family: Tahoma,sans-serif" + '\u0022' + @" align=" + '\u0022' + @"center" + '\u0022' + @">
                                            <img class=" + '\u0022' + @"gnd-corner-image gnd-corner-image-center gnd-corner-image-top" + '\u0022' + @" style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block;max-width: 659px" + '\u0022' + @" src=" + '\u0022' + @"" + img + @"" + '\u0022' + @" alt=" + '\u0022' + @"" + '\u0022' + @" width=" + '\u0022' + @"600" + '\u0022' + @" height=" + '\u0022' + @"138" + '\u0022' + @">
                                        </div>

                                        <table class=" + '\u0022' + @"contents" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td class=" + '\u0022' + @"padded" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;padding-left: 32px;padding-right: 32px;word-break: break-word;word-wrap: break-word" + '\u0022' + @"> <TABLE style=" + '\u0022' + @"BORDER-COLLAPSE: collapse; BORDER-SPACING: 0" + '\u0022' + @">
 <TBODY>
  <TR >
   <TD  style=" + '\u0022' + @"WIDTH: 80%;" + '\u0022' + @">"
    + titulo +
       @"
   </TD>
  </TR>
 </TBODY></TABLE> 
<div style=" + '\u0022' + @"padding-left: 32px;padding-right: 32px;" + '\u0022' + @">
" +
                                                        cuerpo
+ @"</div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                        <div class=" + '\u0022' + @"column-bottom" + '\u0022' + @" style=" + '\u0022' + @"font-size: 8px;line-height: 8px" + '\u0022' + @">&nbsp;</div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td class=" + '\u0022' + @"border" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px" + '\u0022' + @"></td>
                </tr>
            </tbody>
        </table>

        <table class=" + '\u0022' + @"border" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;font-size: 1px;line-height: 1px;background-color: #dcdcdc;Margin-left: auto;Margin-right: auto" + '\u0022' + @" width=" + '\u0022' + @"602" + '\u0022' + @">
            <tbody>
                <tr><td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @"></td></tr>
            </tbody>
        </table>


        <table class=" + '\u0022' + @"footer centered bg" + '\u0022' + @" style=" + '\u0022' + @" background: url(" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/pixelbg_5px.png" + '\u0022' + @"); border-collapse: collapse;border-spacing: 0;Margin-left: auto;height:40px; Margin-right: auto;width: 602px" + '\u0022' + @">
            <tbody>
                <tr style=" + '\u0022' + @"height:40px" + '\u0022' + @">
                    <td></td>
                </tr>
            </tbody>
        </table>
        <table class=" + '\u0022' + @"footer centered" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto;width: 602px" + '\u0022' + @">
            <tbody>
                <tr style=" + '\u0022' + @"" + '\u0022' + @">
                    <td class=" + '\u0022' + @" social" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;padding-top: 20px;padding-bottom: 20px" + '\u0022' + @" align=" + '\u0022' + @"center" + '\u0022' + @">
                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0" + '\u0022' + @">
                            <tbody>
                                <tr>
                                    <td class=" + '\u0022' + @"social-link" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://www.facebook.com/resocentro" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            <img style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block" + '\u0022' + @" src=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/fb.png" + '\u0022' + @" width=" + '\u0022' + @"26" + '\u0022' + @" height=" + '\u0022' + @"26" + '\u0022' + @">
                                                        </a>
                                                    </td>
                                                    <td class=" + '\u0022' + @"social-text" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://www.facebook.com/resocentro" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            Facebook
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class=" + '\u0022' + @"divider" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;font-family: sans-serif;font-size: 10px;line-height: 21px;text-align: center;padding-left: 14px;padding-right: 14px;color: #dcdcdc" + '\u0022' + @">
                                        <img style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block" + '\u0022' + @" src=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/diamond.png" + '\u0022' + @" width=" + '\u0022' + @"5" + '\u0022' + @" height=" + '\u0022' + @"21" + '\u0022' + @" alt=" + '\u0022' + @"" + '\u0022' + @">
                                    </td>
                                    <td class=" + '\u0022' + @"social-link" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://twitter.com/Resocentro" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            <img style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block" + '\u0022' + @" src=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/twt.png" + '\u0022' + @" width=" + '\u0022' + @"26" + '\u0022' + @" height=" + '\u0022' + @"21" + '\u0022' + @">
                                                        </a>
                                                    </td>
                                                    <td class=" + '\u0022' + @"social-text" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://twitter.com/Resocentro" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            Twitter
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class=" + '\u0022' + @"divider" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;font-family: sans-serif;font-size: 10px;line-height: 21px;text-align: center;padding-left: 14px;padding-right: 14px;color: #dcdcdc" + '\u0022' + @">
                                        <img style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block" + '\u0022' + @" src=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo//diamond.png" + '\u0022' + @" width=" + '\u0022' + @"5" + '\u0022' + @" height=" + '\u0022' + @"21" + '\u0022' + @" alt=" + '\u0022' + @"" + '\u0022' + @">
                                    </td>
                                    <td class=" + '\u0022' + @"social-link" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://plus.google.com/100373344420938205875/posts" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            <img style=" + '\u0022' + @"border: 0;-ms-interpolation-mode: bicubic;display: block" + '\u0022' + @" src=" + '\u0022' + @"http://extranet.resocentro.com:5050/PaginaWeb/correo/glg.png" + '\u0022' + @" width=" + '\u0022' + @"20" + '\u0022' + @" height=" + '\u0022' + @"20" + '\u0022' + @">
                                                        </a>
                                                    </td>
                                                    <td class=" + '\u0022' + @"social-text" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @">
                                                        <a style=" + '\u0022' + @"text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @" href=" + '\u0022' + @"https://plus.google.com/100373344420938205875/posts" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">
                                                            Google+
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>

                                    <td class=" + '\u0022' + @"social-link" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @"></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style=" + '\u0022' + @"padding: 0;vertical-align: top" + '\u0022' + @">
                        <table style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0" + '\u0022' + @">
                            <tbody>
                                <tr style=" + '\u0022' + @"background: #2c3e50;" + '\u0022' + @">
                                    <td class=" + '\u0022' + @"address" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;width: 250px;padding-top: 32px;padding-bottom: 32px" + '\u0022' + @">
                                        <table class=" + '\u0022' + @"contents" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td class=" + '\u0022' + @"padded" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;padding-left: 0;padding-right: 10px;word-break: break-word;word-wrap: break-word;text-align: left;font-size: 12px;line-height: 20px;color: #737373;font-family: Tahoma,sans-serif" + '\u0022' + @">
                                                        <div style=" + '\u0022' + @"text-align:right;" + '\u0022' + @">Copyright ©&nbsp;2015 Resocentro.</div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class=" + '\u0022' + @"subscription" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;width: 350px;padding-top: 32px;padding-bottom: 32px" + '\u0022' + @">
                                        <table class=" + '\u0022' + @"contents" + '\u0022' + @" style=" + '\u0022' + @"border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%" + '\u0022' + @">
                                            <tbody>
                                                <tr>
                                                    <td class=" + '\u0022' + @"padded" + '\u0022' + @" style=" + '\u0022' + @"padding: 0;vertical-align: top;padding-left: 10px;padding-right: 0;word-break: break-word;word-wrap: break-word;font-size: 12px;line-height: 20px;color: #737373;font-family: Tahoma,sans-serif;text-align: right" + '\u0022' + @">
                                                        <div style=" + '\u0022' + @"text-align:left;" + '\u0022' + @">Todos los Derechos Reservados</div>
                                                        <div>
                                                            <span class=" + '\u0022' + @"block" + '\u0022' + @">

                                                            </span>
                                                            
                                                        </div>
                                                    </td>

                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table><br/>
                         <span style=" + '\u0022' + @"color:#BA0E0E;font-size:11px" + '\u0022' + @">
                            No responder este e-mail ya que es autogenerado, tenga presente que el horario de envío de respuestas es de 9:00 am. a 5:00 p.m. <a href=" + '\u0022' + @"http://www.resocentro.com" + '\u0022' + @" target=" + '\u0022' + @"_blank" + '\u0022' + @">www.resocentro.com</a> 
                        </span>
                        </td>
                    </tr>
            </tbody>
        </table>
    </center>
</body>
</html>
";

        }
        public bool sendCorreo(string subject, string to, string cc, string body, string cco)
        {


            string sql = @"
EXEC msdb.dbo.sp_send_dbmail
@profile_name = 'Profile_Jhon',
@recipients = '{0}',
@reply_to='autorespuesta@resocentro.com',
@subject = '{1}',
@body = '{2}',  
@body_format = 'HTML' ,  
@importance='High'
{3}
{4} ";
            if (cc != "")
                cc = ",@copy_recipients='" + cc + "'";
            if (cco != "")
                cco = ",@blind_copy_recipients='" + cco + "'";


            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(string.Format(sql, to, subject, body, cc, cco), connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        return true;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            /* MailMessage msg = new MailMessage();
             System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

             try
             {
                 msg.Subject = subject;
                 msg.IsBodyHtml = true;
                 msg.BodyEncoding = System.Text.Encoding.UTF8;
                 msg.SubjectEncoding = System.Text.Encoding.UTF8;
                 msg.Body = body;
                 msg.Sender = new MailAddress("autorespuesta@resocentro.com", "Resocentro");
                 msg.From = new MailAddress("autorespuesta@resocentro.com", "Resocentro");
                 msg.ReplyToList.Add(new MailAddress("autorespuesta@resocentro.com", "Resocentro"));
                 msg.To.Add(to);
                 if (cc != "")
                     msg.CC.Add(cc);
                 if (cco != "")
                     msg.Bcc.Add(cco);
                 msg.IsBodyHtml = true;

                 client.Host = "smtp.gmail.com";
                 System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("alerta.resocentro@gmail.com", "Resocentro2013");
                 client.Port = int.Parse("587");
                 client.EnableSsl = true;
                 client.UseDefaultCredentials = false;
                 client.Credentials = basicauthenticationinfo;
                 //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                 client.Send(msg);
                 msg.Dispose();
                 return true;
             }
             catch (Exception ex)
             {
                 return false;
                 //log.Error(ex.Message);
             }*/
        }






    }
    public enum Estado_Ticket
    {
        Nuevo = 0,
        Llamada = 1,
        Rellamada = 2,
        Caja = 3,
        Cancelado = 4,
        Terminado = 5

    }
    public enum Tipo_Historial
    {
        OffLine = 1,
        OnLine = 2,
        Jalar = 3

    }
}