using Encuesta.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
namespace Encuesta.Util
{
    
    public class Variable
    {
        //especificados en milisegundos
        public string getCadenaConexion { get { return ConfigurationManager.ConnectionStrings["MyConecction"].ConnectionString; } }
        public int tDuracionEncuestador { get { return 300000; } }
        public int tDuracionTecnologo { get { return 2700000; } }
        //especificados en segundos
        public int tRespuestaEncuestador { get { return 300; } }
        public int tRespuestaSupervisor { get { return 300000; } }
        public int tEsperaSolicitud { get { return 7; } }
        public string getUrlpath { get { return @"\\192.168.0.5\Perfiles"; } }
        public string getUrlpathSERVER { get { return @"\\ServerWeb\Perfiles\"; } }
        public static string PathAdjuntosMantenimiento { get { return @"\\serverweb\AdjuntosMantenimiento\"; } }
        public List<Object> getTiempoRM()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Dias" },
                       new { codigo=2,nombre ="Semanas" },
                       new { codigo=3,nombre ="Meses" },
                       new { codigo=4,nombre ="Años" }
                    };
        }

        public string getClientName(string IP)
        {
            System.Net.IPAddress myIP = System.Net.IPAddress.Parse(IP);
            System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }

        public void eliminarIntegrador(string examen)
        {
            string myConnectionString;

            myConnectionString = "server=172.16.104.111;port=3306;user id=visual;password=sistemas;database=pacsdb;persistsecurityinfo=True;";

            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM mwl_item WHERE sps_id='"+examen+"'";

                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

            }
        }

        public void eliminarIntegradorRealizado(string examen)
        {
            string myConnectionString;

            myConnectionString = "server=172.16.104.111;port=3306;user id=visual;password=sistemas;database=pacsdb;persistsecurityinfo=True;";

            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    string query = string.Format(@"insert into mwl_item_delete (
    `mwl_item_delete`.`patient_fk`,
    `mwl_item_delete`.`sps_status`,
    `mwl_item_delete`.`sps_id`,
    `mwl_item_delete`.`start_datetime`,
    `mwl_item_delete`.`station_aet`,
    `mwl_item_delete`.`station_name`,
    `mwl_item_delete`.`modality`,
    `mwl_item_delete`.`perf_physician`,
    `mwl_item_delete`.`perf_phys_fn_sx`,
    `mwl_item_delete`.`perf_phys_gn_sx`,
    `mwl_item_delete`.`perf_phys_i_name`,
    `mwl_item_delete`.`perf_phys_p_name`,
    `mwl_item_delete`.`req_proc_id`,
    `mwl_item_delete`.`accession_no`,
    `mwl_item_delete`.`study_iuid`,
    `mwl_item_delete`.`updated_time`,
    `mwl_item_delete`.`created_time`,
    `mwl_item_delete`.`item_attrs`)
SELECT 
    `mwl_item`.`patient_fk`,
    `mwl_item`.`sps_status`,
    `mwl_item`.`sps_id`,
    `mwl_item`.`start_datetime`,
    `mwl_item`.`station_aet`,
    `mwl_item`.`station_name`,
    `mwl_item`.`modality`,
    `mwl_item`.`perf_physician`,
    `mwl_item`.`perf_phys_fn_sx`,
    `mwl_item`.`perf_phys_gn_sx`,
    `mwl_item`.`perf_phys_i_name`,
    `mwl_item`.`perf_phys_p_name`,
    `mwl_item`.`req_proc_id`,
    `mwl_item`.`accession_no`,
    `mwl_item`.`study_iuid`,
    `mwl_item`.`updated_time`,
    `mwl_item`.`created_time`,
    `mwl_item`.`item_attrs` FROM pacsdb.mwl_item  where sps_id = '{0}.1';
delete from mwl_item where sps_id='{0}.1';", examen.ToString());

                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

            }
        }

        public string getNombreTiempoRM(int tipo)
        {
            switch (tipo)
            {
                case 1:
                    return "Dia(s)";
                case 2:
                    return "Semana(s)";
                case 3:
                    return "Mes(es)";
                case 4:
                    return "Año(s)";
                default:
                    return "";
            }
        }

        public List<Object> getTiempoTEM()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Enero" },
                       new { codigo=2,nombre ="Febrero" },
                       new { codigo=3,nombre ="Marzo" },
                       new { codigo=4,nombre ="Abril" },
                       new { codigo=5,nombre ="Mayo" },
                       new { codigo=6,nombre ="Junio" },
                       new { codigo=7,nombre ="Julio" },
                       new { codigo=8,nombre ="Agosto" },
                       new { codigo=9,nombre ="Septiembre" },
                       new { codigo=10,nombre ="Octubre" },
                       new { codigo=11,nombre ="Noviembre" },
                       new { codigo=12,nombre ="Diciembre" }
                    };
        }

        public List<Object> getDeportes()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Tennis" },
                       new { codigo=2,nombre ="Natación" },
                       new { codigo=3,nombre ="Artes Marciales" },
                       new { codigo=4,nombre ="Atletismo" },
                       new { codigo=5,nombre ="Fútbol" },
                       new { codigo=6,nombre ="Otro" }
                    };
        }

        public string getNombreDeportes(int tipo)
        {
            switch (tipo)
            {
                case 1:
                    return "Tennis";
                case 2:
                    return "Natación";
                case 3:
                    return "Artes Marciales";
                case 4:
                    return "Atletismo";
                case 5:
                    return "Fútbol";
                case 6:
                    return "Otro";
                default:
                    return "";
            };
        }

        public List<Object> getAutorizaciones()
        {
            return new List<Object> { 
                    new{  codigo=1,nombre="Objeto(s) de riesgo menor controlable"},
                    new{  codigo=2,nombre="Objeto(s) compatible con la modalidad de examen"},
                    new{  codigo=99,nombre="Otros"}
                    };
        }

        public List<Object> getReguralidadDeporte()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Diariamente" },
                       new { codigo=2,nombre ="Interdiario" },
                       new { codigo=3,nombre ="Semanalmente" },
                       new { codigo=4,nombre ="Quincenalmente" },
                       new { codigo=5,nombre ="Mensualmente" },
                       new { codigo=6,nombre ="A veces" }
                    };
        }
        public string getNombreReguralidadDeporte(int tipo)
        {
            switch (tipo)
            {
                case 1:
                    return "Diariamente";
                case 2:
                    return "Interdiario";
                case 3:
                    return "Semanalmente";
                case 4:
                    return "Quincenalmente";
                case 5:
                    return "Mensualmente";
                case 6:
                    return "A veces";
                default:
                    return "";
            };
        }

        public List<Object> getCancelaciones()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Presenta Contraindicaciones" },
                       new { codigo=2,nombre ="El Paciente no colabora" },
                       new { codigo=3,nombre ="El Paciente no desea realizar el examen" },
                       new { codigo=4,nombre ="El Paciente presenta dolor en el examen " },
                       new { codigo=4,nombre ="Equipo averiado" },
                       new { codigo=5,nombre ="Presenta Contraindicaciones para la Sedacion" },
                       new { codigo=6,nombre ="Anestesia" },
                       new { codigo=7,nombre ="Claustrofobia" },
                       new { codigo=99,nombre ="Otros" }
                    };
        }
        public List<Object> getCancelacionesContraste()
        {
            return new List<Object> { 
                       new { codigo="Extravasación" },
                       new { codigo="Se derramó" },
                       new { codigo="Cancelación de Exámen" }
                    };
        }
        public List<Object> getCancelacionesSedacion()
        {
            return new List<Object> { 
                       new { codigo="Complicación Anestésica" },
                       new { codigo="Se derramó" },
                       new { codigo="Cancelación de Exámen" }
                    };
        }
        public List<Object> getLocales()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="Sede Central" },
                       new { codigo=2,nombre ="Sede El Golf" },
                       new { codigo=3,nombre ="Sede San Borja" },
                       new { codigo=4,nombre ="Sede Benavides" },
                       new { codigo=5,nombre ="Sede Piura" },
                       new { codigo=6,nombre ="Sede San Judas" },
                       new { codigo=6,nombre ="Sede Javier Prado" },
                       new { codigo=9,nombre ="Otro Centro" }
                    };
        }

        public string getNombreLocales(int tipo)
        {
            switch (tipo)
            {
                case 1:
                    return "la sede Central";
                case 2:
                    return "la sede El Golf";
                case 3:
                    return "la sede San Borja";
                case 4:
                    return "la sede Benavides";
                case 5:
                    return "la sede Piura";
                case 6:
                    return "la sede San Judas";
                case 9:
                    return "otro Centro";
                default:
                    return ""; ;
            }
        }
        public List<Object> getContraste()
        {
            return new List<Object> { 
                       new { codigo=1,nombre ="No tributario" },
                       new { codigo=2,nombre ="Alérgico" },
                       new { codigo=3,nombre ="Insuficiencia renal" },
                       new { codigo=5,nombre ="No Desea el Paciente" },
                       new { codigo=6,nombre ="No Desea Médico referente" },                       
                       new { codigo=6,nombre ="Paciente/familiar no autoriza uso de contraste" },
                    };
        }

        public string CuerpoMensaje(string _msj)
        {
            return @"
<!DOCTYPE html>

<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <title></title>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>   
    <!--[if gte mso 9]>
    <style>
      .column-top {
        mso-line-height-rule: exactly !important;
      }
    </style>
    <![endif]-->
    <meta name='robots' content='noindex,nofollow'>
    <meta property='og:title' content='My First Campaign'>
    <link href='http://extranet.resocentro.com:5050/PaginaWeb/correo/social.min.css?h=A3DB20FCcapricorn' media='screen,projection' rel='stylesheet' type='text/css'>
</head>
<body style='margin: 0;mso-line-height-rule: exactly;padding: 0;min-width: 100%;background-color: #ededed'>
<style type='text/css'>
   @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);@import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);.emb-editor-canvas,.wrapper,body{background-color:#ededed}.border{background-color:#dcdcdc}h1{color:#565656}.wrapper h1{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h1{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h1{line-height:44px}.two-col h1{line-height:34px}.three-col h1{line-height:26px}.wrapper .one-col-feature h1{line-height:58px}@media only screen and (max-width:620px){h1{line-height:44px!important}}h2{color:#555}.wrapper h2{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h2{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h2{line-height:34px}.two-col h2{line-height:26px}.three-col h2{line-height:22px}.wrapper .one-col-feature h2{line-height:50px}@media only screen and (max-width:620px){h2{line-height:34px!important}}h3{color:#555}.wrapper h3{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper h3{font-family:Roboto,Tahoma,sans-serif!important}}.one-col h3{line-height:26px}.two-col h3{line-height:24px}.three-col h3{line-height:20px}.wrapper .one-col-feature h3{line-height:40px}@media only screen and (max-width:620px){h3{line-height:26px!important}}ol,p,ul{color:#565656}.wrapper ol,.wrapper p,.wrapper ul{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper ol,.wrapper p,.wrapper ul{font-family:Roboto,Tahoma,sans-serif!important}}.one-col ol,.one-col p,.one-col ul{line-height:25px;Margin-bottom:25px}.two-col ol,.two-col p,.two-col ul{line-height:22px;Margin-bottom:22px}.three-col ol,.three-col p,.three-col ul{line-height:20px;Margin-bottom:20px}.wrapper .one-col-feature ol,.wrapper .one-col-feature p,.wrapper .one-col-feature ul{line-height:30px}.one-col-feature blockquote ol,.one-col-feature blockquote p,.one-col-feature blockquote ul{line-height:50px}@media only screen and (max-width:620px){ol,p,ul{line-height:25px!important;Margin-bottom:25px!important}}.image{color:#565656;font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.image{font-family:Roboto,Tahoma,sans-serif!important}}.wrapper a{color:#41637e}.wrapper a:hover{color:#30495c!important}.wrapper .logo div{color:#41637e;font-family:sans-serif}@media only screen and (min-width:0){.wrapper .logo div{font-family:Avenir,sans-serif!important}}.wrapper .logo div a{color:#41637e}.wrapper .logo div a:hover{color:#41637e!important}.wrapper .one-col-feature ol a,.wrapper .one-col-feature p a,.wrapper .one-col-feature ul a{border-bottom:1px solid #41637e}.wrapper .one-col-feature ol a:hover,.wrapper .one-col-feature p a:hover,.wrapper .one-col-feature ul a:hover{color:#30495c!important;border-bottom:1px solid #30495c!important}.wrapper .btn a{font-family:Tahoma,sans-serif;background-color:#41637e;color:#fff!important;outline-color:#41637e;text-shadow:0 1px 0 #3b5971}@media only screen and (min-width:0){.wrapper .btn a{font-family:Roboto,Tahoma,sans-serif!important}}.wrapper .btn a:hover{background-color:#3b5971!important;color:#fff!important;outline-color:#3b5971!important}.footer .padded,.preheader .title,.preheader .webversion{color:#737373;font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.footer .padded,.preheader .title,.preheader .webversion{font-family:Roboto,Tahoma,sans-serif!important}}.footer .padded a,.preheader .title a,.preheader .webversion a{color:#737373}.footer .padded a:hover,.preheader .title a:hover,.preheader .webversion a:hover{color:#4d4d4d!important}.footer .social .divider{color:#dcdcdc}.footer .social .social-text,.footer .social a{color:#737373}.wrapper .footer .social .social-text,.wrapper .footer .social a{font-family:Tahoma,sans-serif}@media only screen and (min-width:0){.wrapper .footer .social .social-text,.wrapper .footer .social a{font-family:Roboto,Tahoma,sans-serif!important}}.footer .social .social-text:hover,.footer .social a:hover{color:#4d4d4d!important}.image .border{background-color:#c8c8c8}.image-frame{background-color:#dadada}.image-background{background-color:#f7f7f7}
    </style>
    
    <center class='wrapper' style='display: table;table-layout: fixed;width: 100%;min-width: 620px;-webkit-text-size-adjust: 100%;-ms-text-size-adjust: 100%;background-color: #ededed'>
        <table class='gmail' style='border-collapse: collapse;border-spacing: 0;width: 650px;min-width: 650px'><tbody><tr><td style='padding: 0;vertical-align: top;font-size: 1px;line-height: 1px'>&nbsp;</td></tr></tbody></table>
        <table class='preheader centered' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto'>
            <tbody>
                <tr>
                    <td style='padding: 0;vertical-align: top'>
                        <table style='border-collapse: collapse;border-spacing: 0;width: 602px'>
                            <tbody>
                                <tr></tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>


        <table class='border' style='border-collapse: collapse;border-spacing: 0;font-size: 1px;line-height: 1px;background-color: #dcdcdc;Margin-left: auto;Margin-right: auto' width='602'>
            <tbody>
                <tr><td style='padding: 0;vertical-align: top'>?</td></tr>
            </tbody>
        </table>

        <table class='centered' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto'>
            <tbody>
                <tr>
                    <td class='border' style='padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px'>?</td>
                    <td style='padding: 0;vertical-align: top'>
                        <table class='one-col' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto;width: 600px;background-color: #ffffff;font-size: 14px;table-layout: fixed'>
                            <tbody>
                                <tr>
                                    <td class='column' style='padding: 0;vertical-align: top;text-align: left'>

                                        <div class='image' style='font-size: 12px;Margin-bottom: 24px;mso-line-height-rule: at-least;color: #565656;font-family: Tahoma,sans-serif' align='center'>
                                            <img class='gnd-corner-image gnd-corner-image-center gnd-corner-image-top' style='border: 0;-ms-interpolation-mode: bicubic;display: block;max-width: 659px' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/Calificaciontop.jpg' alt='' width='600' height='138'>
                                        </div>

                                        <table class='contents' style='border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%'>
                                            <tbody>
                                                <tr>
                                                    <td class='padded' style='padding: 0;vertical-align: top;padding-left: 32px;padding-right: 32px;word-break: break-word;word-wrap: break-word'>
" + _msj + @"                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                        <div class='column-bottom' style='font-size: 8px;line-height: 8px'>&nbsp;</div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td class='border' style='padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px'>?</td>
                </tr>
            </tbody>
        </table>

        <table class='border' style='border-collapse: collapse;border-spacing: 0;font-size: 1px;line-height: 1px;background-color: #dcdcdc;Margin-left: auto;Margin-right: auto' width='602'>
            <tbody>
                <tr><td style='padding: 0;vertical-align: top'>?</td></tr>
            </tbody>
        </table>


        <table class='footer centered bg' style=' background: url('http://extranet.resocentro.com:5050/PaginaWeb/correo/pixelbg_5px.png'); border-collapse: collapse;border-spacing: 0;Margin-left: auto;height:40px; Margin-right: auto;width: 602px'>
            <tbody>
                <tr style='height:40px'>
                    <td></td>
                </tr>
            </tbody>
        </table>
        <table class='footer centered' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto;width: 602px'>
            <tbody>
                <tr style=''>
                    <td class=' social' style='padding: 0;vertical-align: top;padding-top: 20px;padding-bottom: 20px' align='center'>
                        <table style='border-collapse: collapse;border-spacing: 0'>
                            <tbody>
                                <tr>
                                    <td class='social-link' style='padding: 0;vertical-align: top'>
                                        <table style='border-collapse: collapse;border-spacing: 0'>
                                            <tbody>
                                                <tr>
                                                    <td style='padding: 0;vertical-align: top'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://www.facebook.com/resocentro' target='_blank'>
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/fb.png' width='26' height='26'>
                                                        </a>
                                                    </td>
                                                    <td class='social-text' style='padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://www.facebook.com/resocentro' target='_blank'>
                                                            Facebook
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class='divider' style='padding: 0;vertical-align: top;font-family: sans-serif;font-size: 10px;line-height: 21px;text-align: center;padding-left: 14px;padding-right: 14px;color: #dcdcdc'>
                                        <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/diamond.png' width='5' height='21' alt=''>
                                    </td>
                                    <td class='social-link' style='padding: 0;vertical-align: top'>
                                        <table style='border-collapse: collapse;border-spacing: 0'>
                                            <tbody>
                                                <tr>
                                                    <td style='padding: 0;vertical-align: top'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://twitter.com/Resocentro' target='_blank'>
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/twt.png' width='26' height='21'>
                                                        </a>
                                                    </td>
                                                    <td class='social-text' style='padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://twitter.com/Resocentro' target='_blank'>
                                                            Twitter
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class='divider' style='padding: 0;vertical-align: top;font-family: sans-serif;font-size: 10px;line-height: 21px;text-align: center;padding-left: 14px;padding-right: 14px;color: #dcdcdc'>
                                        <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo//diamond.png' width='5' height='21' alt=''>
                                    </td>
                                    <td class='social-link' style='padding: 0;vertical-align: top'>
                                        <table style='border-collapse: collapse;border-spacing: 0'>
                                            <tbody>
                                                <tr>
                                                    <td style='padding: 0;vertical-align: top'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://plus.google.com/100373344420938205875/posts' target='_blank'>
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/glg.png' width='20' height='20'>
                                                        </a>
                                                    </td>
                                                    <td class='social-text' style='padding: 0;vertical-align: middle !important;height: 21px;font-size: 10px;font-weight: bold;text-decoration: none;text-transform: uppercase;color: #737373;font-family: Tahoma,sans-serif'>
                                                        <a style='text-decoration: none;transition: all .2s;color: #737373;font-family: Tahoma,sans-serif' href='https://plus.google.com/100373344420938205875/posts' target='_blank'>
                                                            Google+
                                                        </a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>

                                    <td class='social-link' style='padding: 0;vertical-align: top'></td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style='padding: 0;vertical-align: top'>
                        <table style='border-collapse: collapse;border-spacing: 0'>
                            <tbody>
                                <tr style='background: #2c3e50;'>
                                    <td class='address' style='padding: 0;vertical-align: top;width: 250px;padding-top: 32px;padding-bottom: 32px'>
                                        <table class='contents' style='border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%'>
                                            <tbody>
                                                <tr>
                                                    <td class='padded' style='padding: 0;vertical-align: top;padding-left: 0;padding-right: 10px;word-break: break-word;word-wrap: break-word;text-align: left;font-size: 12px;line-height: 20px;color: #737373;font-family: Tahoma,sans-serif'>
                                                        <div style='text-align:right;'>Copyright ©&nbsp;2015 Resocentro.</div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td class='subscription' style='padding: 0;vertical-align: top;width: 350px;padding-top: 32px;padding-bottom: 32px'>
                                        <table class='contents' style='border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%'>
                                            <tbody>
                                                <tr>
                                                    <td class='padded' style='padding: 0;vertical-align: top;padding-left: 10px;padding-right: 0;word-break: break-word;word-wrap: break-word;font-size: 12px;line-height: 20px;color: #737373;font-family: Tahoma,sans-serif;text-align: right'>
                                                        <div style='text-align:left;'>Todos los Derechos Reservados</div>
                                                        <div>
                                                            <span class='block'>

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
                         <span style='color:#BA0E0E;font-size:11px'>
                            No responder este e-mail ya que es autogenerado, tenga presente que el horario de envío de respuestas es de 9:00 am. a 5:00 p.m. <a href='http://www.resocentro.com' target='_blank'>www.resocentro.com</a> 
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
            body = body.Replace("'", "\"");
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
            //MailMessage msg = new MailMessage();
            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            //try
            //{
            //    msg.Subject = subject;
            //    msg.IsBodyHtml = true;
            //    msg.BodyEncoding = System.Text.Encoding.UTF8;
            //    msg.SubjectEncoding = System.Text.Encoding.UTF8;
            //    msg.Body = body;
            //    msg.Sender = new MailAddress("autorespuesta@resocentro.com", "Resocentro");
            //    msg.From = new MailAddress("autorespuesta@resocentro.com", "Resocentro");
            //    msg.ReplyToList.Add(new MailAddress("autorespuesta@resocentro.com", "Resocentro"));
            //    msg.To.Add(to);
            //    if (cc != "")
            //        msg.CC.Add(cc);
            //    if (cco != "")
            //        msg.Bcc.Add(cco);
            //    msg.IsBodyHtml = true;

            //    client.Host = "smtp.gmail.com";
            //    System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("alerta.resocentro@gmail.com", "Resocentro2013");
            //    client.Port = int.Parse("587");
            //    client.EnableSsl = true;
            //    client.UseDefaultCredentials = false;
            //    client.Credentials = basicauthenticationinfo;
            //    //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    client.Send(msg);
            //    msg.Dispose();
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //    //log.Error(ex.Message);
            //}
        }


        public List<Object> getVacio()
        {
            return new List<Object>
            {
            };
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



        public List<Object> getEstadoEquipoMantenimiento()
        {
            return new List<Object> { 
                       new { nombre ="Operativo Totalmente" },
                       new {nombre ="Operativo con Restricciones" },
                       new { nombre ="Inoperativo" }
                    };
        }
    }
    public enum EncuestaType
    {
        Cerebro = 1,

        //HOMBRO
        HombroIzquierdo = 2,
        HombroDerecho = 3,

        //RODILLA
        RodillaIzquierda = 4,
        RodillaDerecha = 5,

        //COLUMNA A
        ColumnaCervical = 6,
        ColumnaCervicoDorsal = 7,

        //COLUMNA B
        ColumnaDorsal = 8,
        ColumnaDorsoLumbar = 9,
        ColumnaLumboSacra = 10,

        //COLUMNA C
        ColumnaSacroCoxis = 11,

        //cadera
        Caderaderecha = 12,
        Caderaizquierda = 13,

        //codo
        Cododerecha = 14,
        Codoizquierda = 15,

        //pie
        TobilloIzquierdo = 16,
        PieIzquierdo = 17,
        DedosIzquierdo = 18,
        TobilloDerecho = 19,
        PieDerecho = 20,
        DedosDerecho = 21,

        //manoDERECHA
        MeniqueDerecho = 22,
        AnularDerecho = 23,
        MedioDerecho = 24,
        IndiceDerecho = 25,
        PulgarDerecho = 26,
        ManoDerecha = 27,
        MunecaDerecha = 28,
        //manoIZQUIERDA
        MeniqueIzquierdo = 29,
        AnularIzquierdo = 30,
        MedioIzquierdo = 31,
        IndiceIzquierdo = 32,
        PulgarIzquierdo = 33,
        ManoIzquierdo = 34,
        MunecaIzquierdo = 35,

        //abdomen
        Abdomen = 43,

        //extremidades DERECHA
        BrazoDerecho = 44,
        AntebrazoDerecho = 45,
        MusloDerecho = 46,
        PiernaDerecho = 47,

        //extremidades IZQUIERDA
        BrazoIzquierdo = 48,
        AntebrazoIzquierdo = 49,
        MusloIzquierdo = 50,
        PiernaIzquierdo = 51,

        //mama
        MamaDerecha = 55,
        MamaIzquierda = 56,

        /**/
        SupervisarEncuesta = 36,

        /*ENCUESTAS DE TEM*/
        Oncologico = 37,
        Colonoscopia = 38,

        //ARTERIAS
        AngiotemCoronario = 39,
        AngiotemAorta = 40,
        AngiotemCerebral = 41,
        MusculoEsqueletico = 42,
        NeuroCerebro = 52,
        Generica = 53,
        ProtocoloTEM = 54,//YA NO SE USA

        EncuestaVacia = 57,
        EncuestaAmpliacion = 58,


        //EMETAC
        EncuestaGenericaEME = 59,

        Fibroscan= 60


        //ultima 60


    }
}