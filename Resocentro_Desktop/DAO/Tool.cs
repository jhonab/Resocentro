using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace Resocentro_Desktop.DAO
{
    public class Tool
    {
        public int cantidad_decimales
        {
            get
            {
               return 5;
                try
                {
                    foreach (string key in ConfigurationManager.AppSettings)
                    {
                        if (key == "cantidadDecimales")
                            return int.Parse(ConfigurationManager.AppSettings[key]);   
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);               
                    return 0;
                }
            }
        }
        public int[] CompaniasNOAfectasTEM
        {
            get
            {
                return new int[]{8, 31, 21, 25, 28, 29, 119, 207, 208, 248, 250};
                try
                {
                    foreach (string key in ConfigurationManager.AppSettings)
                    {
                        if (key == "CompaniasNOAfectasTEM")
                        {
                            string value = (ConfigurationManager.AppSettings[key]);
                            return Array.ConvertAll((value).Split(','), s => int.Parse(s));
                        }
                    }
                    return new int[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return new int[0];
                }
            }
        }

        bool invalid = false;
        public static string connectionStringSitedsV9
        {
            get
            {
                return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\SITEDS CLIENTE 9.1 (Rev. 1.3)\epslog.mdb";
            }
        }
        public static string connectionStringSiteds
        {
            get
            {
                return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\SUSALUD\SITEDS\IPRESSLog.mdb";
            }
        }

        public static string PathImgBuddy { get { return @"\\Serverweb\Perfiles\"; } }
        public static string PathFileSolicitudes { get { return @"\\192.168.0.14\Carta\"; } }
        public static string UrlDocumentosAdjuntos { get { return @"http://serverweb:5002/"; } }
        public static string PathDocumentosAdjuntos { get { return @"\\serverweb\DocumentoAjunto\"; } }
        public static string PathCERTIFICADORESO { get { return @"\\serverweb\Facturacion\Cer-Reso\MPS20151222232684.pfx"; } }
        public static string PathCERTIFICADOEMETAC { get { return @"\\serverweb\Facturacion\Cer-Eme\MPS20160829351646.pfx"; } }
        public static string PathDocumentosFacturacion { get { return @"\\serverweb\Facturacion\"; } }
        //private int[] _companias = { 8, 26, 31, 21, 25, 28, 29, 119, 207, 208, 248, 250 };


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
        public List<Adjuntos_Web> getAdjuntosWeb(string idsolicitud)
        {
            string path = PathFileSolicitudes + idsolicitud;
            List<Adjuntos_Web> lista = new List<Adjuntos_Web>();
            if (Directory.Exists(path))
            {
                // Process the list of files found in the directory. 
                string[] fileEntries = Directory.GetFiles(path);
                foreach (string fileName in fileEntries)
                {
                    Adjuntos_Web item = new Adjuntos_Web();
                    item.ruta = fileName;
                    var _x = fileName.Split('\\');
                    item.nombre = _x[_x.Length - 1];
                    item.size = (new FileInfo(item.ruta).Length) / 1024;
                    lista.Add(item);
                }
            }
            return lista;
        }
        public string getCuerpoEmail(string img, string titulo, string cuerpo)
        {
            img = img.Replace("'", "\"");
            titulo = titulo.Replace("'", "\"");
            cuerpo = cuerpo.Replace("'", "\"");

            string msj = @"
<html xmlns='http://www.w3.org/1999/xhtml'><head>
    <title></title>
    <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
    <style type='text/css'>    
        body {
            margin: 0;
            mso-line-height-rule: exactly;
            padding: 0;
            min-width: 100%;
        }

        table {
            border-collapse: collapse;
            border-spacing: 0;
        }

        td {
            padding: 0;
            vertical-align: top;
        }

        .spacer,
        .border {
            font-size: 1px;
            line-height: 1px;
        }

        .spacer {
            width: 100%;
        }
        .bg{
            /*background: url('http://extranet.resocentro.com:5050/PaginaWeb/correo/pixelbg_5px.png') repeat center;*/
            background: url('http://extranet.resocentro.com:5050/PaginaWeb/correo/pixelbg_5px.png');
            width:100%;
        }
        img {
            border: 0;
            -ms-interpolation-mode: bicubic;
        }

        .image {
            font-size: 12px;
            Margin-bottom: 24px;
            mso-line-height-rule: at-least;
        }

            .image img {
                display: block;
            }

        .logo {
            mso-line-height-rule: at-least;
        }

            .logo img {
                display: block;
            }

        strong {
            font-weight: bold;
        }

        h1,
        h2,
        h3,
        p,
        ol,
        ul,
        li {
            Margin-top: 0;
        }

        ol,
        ul,
        li {
            padding-left: 0;
        }

        blockquote {
            Margin-top: 0;
            Margin-right: 0;
            Margin-bottom: 0;
            padding-right: 0;
        }

        .column-top {
            font-size: 32px;
            line-height: 32px;
        }

        .column-bottom {
            font-size: 8px;
            line-height: 8px;
        }

        .column {
            text-align: left;
        }

        .contents {
            table-layout: fixed;
            width: 100%;
        }

        .padded {
            padding-left: 32px;
            padding-right: 32px;
            word-break: break-word;
            word-wrap: break-word;
        }

        .wrapper {
            display: table;
            table-layout: fixed;
            width: 100%;
            min-width: 620px;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }

        table.wrapper {
            table-layout: fixed;
        }

        .one-col,
        .two-col,
        .three-col {
            Margin-left: auto;
            Margin-right: auto;
            width: 600px;
        }

        .centered {
            Margin-left: auto;
            Margin-right: auto;
        }

        .two-col .image {
            Margin-bottom: 23px;
        }

        .two-col .column-bottom {
            font-size: 9px;
            line-height: 9px;
        }

        .two-col .column {
            width: 300px;
        }

        .three-col .image {
            Margin-bottom: 21px;
        }

        .three-col .column-bottom {
            font-size: 11px;
            line-height: 11px;
        }

        .three-col .column {
            width: 200px;
        }

        .three-col .first .padded {
            padding-left: 32px;
            padding-right: 16px;
        }

        .three-col .second .padded {
            padding-left: 24px;
            padding-right: 24px;
        }

        .three-col .third .padded {
            padding-left: 16px;
            padding-right: 32px;
        }

        @media only screen and (min-width: 0) {
            .wrapper {
                text-rendering: optimizeLegibility;
            }
        }

        @media only screen and (max-width: 620px) {
            [class=wrapper] {
                min-width: 318px !important;
                width: 100% !important;
            }

                [class=wrapper] .one-col,
                [class=wrapper] .two-col,
                [class=wrapper] .three-col {
                    width: 318px !important;
                }

                [class=wrapper] .column,
                [class=wrapper] .gutter {
                    display: block;
                    float: left;
                    width: 318px !important;
                }

                [class=wrapper] .padded {
                    padding-left: 32px !important;
                    padding-right: 32px !important;
                }

                [class=wrapper] .block {
                    display: block !important;
                }

                [class=wrapper] .hide {
                    display: none !important;
                }

                [class=wrapper] .image {
                    margin-bottom: 24px !important;
                }

                    [class=wrapper] .image img {
                        height: auto !important;
                        width: 100% !important;
                    }
        }

        .wrapper h1 {
            font-weight: 700;
        }

        .wrapper h2 {
            font-style: italic;
            font-weight: normal;
        }

        .wrapper h3 {
            font-weight: normal;
        }

        .one-col blockquote,
        .two-col blockquote,
        .three-col blockquote {
            font-style: italic;
        }

        .one-col-feature h1 {
            font-weight: normal;
        }

        .one-col-feature h2 {
            font-style: normal;
            font-weight: bold;
        }

        .one-col-feature h3 {
            font-style: italic;
        }

        td.border {
            width: 1px;
        }

        tr.border {
            background-color: #e9e9e9;
            height: 1px;
        }

            tr.border td {
                line-height: 1px;
            }

        .one-col,
        .two-col,
        .three-col,
        .one-col-feature {
            background-color: #ffffff;
            font-size: 14px;
            table-layout: fixed;
        }

        .one-col,
        .two-col,
        .three-col,
        .one-col-feature,
        .preheader,
        .header,
        .footer {
            Margin-left: auto;
            Margin-right: auto;
        }

            .preheader table {
                width: 602px;
            }

            .preheader .title,
            .preheader .webversion {
                padding-top: 10px;
                padding-bottom: 12px;
                font-size: 12px;
                line-height: 21px;
            }

            .preheader .title {
                text-align: left;
            }

            .preheader .webversion {
                text-align: right;
                width: 300px;
            }

        .header {
            width: 602px;
        }

            .header .logo {
                padding: 32px 0;
            }

                .header .logo div {
                    font-size: 26px;
                    font-weight: 700;
                    letter-spacing: -0.02em;
                    line-height: 32px;
                }

                    .header .logo div a {
                        text-decoration: none;
                    }

                    .header .logo div.logo-center {
                        text-align: center;
                    }

                        .header .logo div.logo-center img {
                            Margin-left: auto;
                            Margin-right: auto;
                        }

        .gmail {
            width: 650px;
            min-width: 650px;
        }

            .gmail td {
                font-size: 1px;
                line-height: 1px;
            }

        .wrapper a {
            text-decoration: underline;
            transition: all .2s;
        }

        .wrapper h1 {
            font-size: 36px;
            Margin-bottom: 18px;
        }

        .wrapper h2 {
            font-size: 26px;
            line-height: 32px;
            Margin-bottom: 20px;
        }

        .wrapper h3 {
            font-size: 18px;
            line-height: 22px;
            Margin-bottom: 16px;
        }

            .wrapper h1 a,
            .wrapper h2 a,
            .wrapper h3 a {
                text-decoration: none;
            }

        .one-col blockquote,
        .two-col blockquote,
        .three-col blockquote {
            font-size: 14px;
            border-left: 2px solid #e9e9e9;
            Margin-left: 0;
            padding-left: 16px;
        }

        table.divider {
            width: 100%;
        }

        .divider .inner {
            padding-bottom: 24px;
        }

        .divider table {
            background-color: #e9e9e9;
            font-size: 2px;
            line-height: 2px;
            width: 60px;
        }

        .wrapper .gray {
            background-color: #f7f7f7;
        }

            .wrapper .gray blockquote {
                border-left-color: #dddddd;
            }

            .wrapper .gray .divider table {
                background-color: #dddddd;
            }

        .padded .image {
            font-size: 0;
        }

        .image-frame {
            padding: 8px;
        }

        .image-background {
            display: inline-block;
            font-size: 12px;
        }

        .btn {
            Margin-bottom: 24px;
            padding: 2px;
        }

            .btn a {
                border: 1px solid #ffffff;
                display: inline-block;
                font-size: 13px;
                font-weight: bold;
                line-height: 15px;
                outline-style: solid;
                outline-width: 2px;
                padding: 10px 30px;
                text-align: center;
                text-decoration: none !important;
            }

        .one-col .column table:nth-last-child(2) td h1:last-child,
        .one-col .column table:nth-last-child(2) td h2:last-child,
        .one-col .column table:nth-last-child(2) td h3:last-child,
        .one-col .column table:nth-last-child(2) td p:last-child,
        .one-col .column table:nth-last-child(2) td ol:last-child,
        .one-col .column table:nth-last-child(2) td ul:last-child {
            Margin-bottom: 24px;
        }

        .one-col p,
        .one-col ol,
        .one-col ul {
            font-size: 16px;
            line-height: 24px;
        }

        .one-col ol,
        .one-col ul {
            Margin-left: 18px;
        }

        .two-col .column table:nth-last-child(2) td h1:last-child,
        .two-col .column table:nth-last-child(2) td h2:last-child,
        .two-col .column table:nth-last-child(2) td h3:last-child,
        .two-col .column table:nth-last-child(2) td p:last-child,
        .two-col .column table:nth-last-child(2) td ol:last-child,
        .two-col .column table:nth-last-child(2) td ul:last-child {
            Margin-bottom: 23px;
        }

        .two-col .image-frame {
            padding: 6px;
        }

        .two-col h1 {
            font-size: 26px;
            line-height: 32px;
            Margin-bottom: 16px;
        }

        .two-col h2 {
            font-size: 20px;
            line-height: 26px;
            Margin-bottom: 18px;
        }

        .two-col h3 {
            font-size: 16px;
            line-height: 20px;
            Margin-bottom: 14px;
        }

        .two-col p,
        .two-col ol,
        .two-col ul {
            font-size: 14px;
            line-height: 23px;
        }

        .two-col ol,
        .two-col ul {
            Margin-left: 16px;
        }

        .two-col li {
            padding-left: 5px;
        }

        .two-col .divider .inner {
            padding-bottom: 23px;
        }

        .two-col .btn {
            Margin-bottom: 23px;
        }

        .two-col blockquote {
            padding-left: 16px;
        }

        .three-col .column table:nth-last-child(2) td h1:last-child,
        .three-col .column table:nth-last-child(2) td h2:last-child,
        .three-col .column table:nth-last-child(2) td h3:last-child,
        .three-col .column table:nth-last-child(2) td p:last-child,
        .three-col .column table:nth-last-child(2) td ol:last-child,
        .three-col .column table:nth-last-child(2) td ul:last-child {
            Margin-bottom: 21px;
        }

        .three-col .image-frame {
            padding: 4px;
        }

        .three-col h1 {
            font-size: 20px;
            line-height: 26px;
            Margin-bottom: 12px;
        }

        .three-col h2 {
            font-size: 16px;
            line-height: 22px;
            Margin-bottom: 14px;
        }

        .three-col h3 {
            font-size: 14px;
            line-height: 18px;
            Margin-bottom: 10px;
        }

        .three-col p,
        .three-col ol,
        .three-col ul {
            font-size: 12px;
            line-height: 21px;
        }

        .three-col ol,
        .three-col ul {
            Margin-left: 14px;
        }

        .three-col li {
            padding-left: 6px;
        }

        .three-col .divider .inner {
            padding-bottom: 21px;
        }

        .three-col .btn {
            Margin-bottom: 21px;
        }

            .three-col .btn a {
                font-size: 12px;
                line-height: 14px;
                padding: 8px 19px;
            }

        .three-col blockquote {
            padding-left: 16px;
        }

        .one-col-feature .column-top {
            font-size: 36px;
            line-height: 36px;
        }

        .one-col-feature .column-bottom {
            font-size: 4px;
            line-height: 4px;
        }

        .one-col-feature .column {
            text-align: center;
            width: 600px;
        }

        .one-col-feature .image {
            Margin-bottom: 32px;
        }

        .one-col-feature .column table:nth-last-child(2) td h1:last-child,
        .one-col-feature .column table:nth-last-child(2) td h2:last-child,
        .one-col-feature .column table:nth-last-child(2) td h3:last-child,
        .one-col-feature .column table:nth-last-child(2) td p:last-child,
        .one-col-feature .column table:nth-last-child(2) td ol:last-child,
        .one-col-feature .column table:nth-last-child(2) td ul:last-child {
            Margin-bottom: 32px;
        }

        .one-col-feature h1,
        .one-col-feature h2,
        .one-col-feature h3 {
            text-align: center;
        }

        .one-col-feature h1 {
            font-size: 52px;
            Margin-bottom: 22px;
        }

        .one-col-feature h2 {
            font-size: 42px;
            Margin-bottom: 20px;
        }

        .one-col-feature h3 {
            font-size: 32px;
            line-height: 42px;
            Margin-bottom: 20px;
        }

        .one-col-feature p,
        .one-col-feature ol,
        .one-col-feature ul {
            font-size: 21px;
            line-height: 32px;
            Margin-bottom: 32px;
        }

            .one-col-feature p a,
            .one-col-feature ol a,
            .one-col-feature ul a {
                text-decoration: none;
            }

        .one-col-feature p {
            text-align: center;
        }

        .one-col-feature ol,
        .one-col-feature ul {
            Margin-left: 40px;
            text-align: left;
        }

        .one-col-feature li {
            padding-left: 3px;
        }

        .one-col-feature .btn {
            Margin-bottom: 32px;
            text-align: center;
        }

        .one-col-feature .divider .inner {
            padding-bottom: 32px;
        }

        .one-col-feature blockquote {
            border-bottom: 2px solid #e9e9e9;
            border-left-color: #ffffff;
            border-left-width: 0;
            border-left-style: none;
            border-top: 2px solid #e9e9e9;
            Margin-bottom: 32px;
            Margin-left: 0;
            padding-bottom: 42px;
            padding-left: 0;
            padding-top: 42px;
            position: relative;
        }

            .one-col-feature blockquote:before,
            .one-col-feature blockquote:after {
                background: -moz-linear-gradient(left, #ffffff 25%, #e9e9e9 25%, #e9e9e9 75%, #ffffff 75%);
                background: -webkit-gradient(linear, left top, right top, color-stop(25%, #ffffff), color-stop(25%, #e9e9e9), color-stop(75%, #e9e9e9), color-stop(75%, #ffffff));
                background: -webkit-linear-gradient(left, #ffffff 25%, #e9e9e9 25%, #e9e9e9 75%, #ffffff 75%);
                background: -o-linear-gradient(left, #ffffff 25%, #e9e9e9 25%, #e9e9e9 75%, #ffffff 75%);
                background: -ms-linear-gradient(left, #ffffff 25%, #e9e9e9 25%, #e9e9e9 75%, #ffffff 75%);
                background: linear-gradient(to right, #ffffff 25%, #e9e9e9 25%, #e9e9e9 75%, #ffffff 75%);
                content: '';
                display: block;
                height: 2px;
                left: 0;
                outline: 1px solid #ffffff;
                position: absolute;
                right: 0;
            }

            .one-col-feature blockquote:before {
                top: -2px;
            }

            .one-col-feature blockquote:after {
                bottom: -2px;
            }

            .one-col-feature blockquote p,
            .one-col-feature blockquote ol,
            .one-col-feature blockquote ul {
                font-size: 42px;
                line-height: 48px;
                Margin-bottom: 48px;
            }

                .one-col-feature blockquote p:last-child,
                .one-col-feature blockquote ol:last-child,
                .one-col-feature blockquote ul:last-child {
                    Margin-bottom: 0 !important;
                }

        .footer {
            width: 602px;
        }

            .footer .padded {
                font-size: 12px;
                line-height: 20px;
            }

        .social {
            padding-top: 32px;
            padding-bottom: 22px;
        }

            .social img {
                display: block;
            }

            .social .divider {
                font-family: sans-serif;
                font-size: 10px;
                line-height: 21px;
                text-align: center;
                padding-left: 14px;
                padding-right: 14px;
            }

            .social .social-text {
                height: 21px;
                vertical-align: middle !important;
                font-size: 10px;
                font-weight: bold;
                text-decoration: none;
                text-transform: uppercase;
            }

                .social .social-text a {
                    text-decoration: none;
                }

        .address {
            width: 250px;
        }

            .address .padded {
                text-align: left;
                padding-left: 0;
                padding-right: 10px;
            }

        .subscription {
            width: 350px;
        }

            .subscription .padded {
                text-align: right;
                padding-right: 0;
                padding-left: 10px;
            }

        .address,
        .subscription {
            padding-top: 32px;
            padding-bottom: 64px;
        }

            .address a,
            .subscription a {
                font-weight: bold;
                text-decoration: none;
            }

            .address table,
            .subscription table {
                width: 100%;
            }

        @media only screen and (max-width: 651px) {
            .gmail {
                display: none !important;
            }
        }

        @media only screen and (max-width: 620px) {
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td h1:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td h1:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td h1:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td h1:last-child,
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td h2:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td h2:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td h2:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td h2:last-child,
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td h3:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td h3:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td h3:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td h3:last-child,
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td p:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td p:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td p:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td p:last-child,
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td ol:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td ol:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td ol:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td ol:last-child,
            [class=wrapper] .one-col .column:last-child table:nth-last-child(2) td ul:last-child,
            [class=wrapper] .two-col .column:last-child table:nth-last-child(2) td ul:last-child,
            [class=wrapper] .three-col .column:last-child table:nth-last-child(2) td ul:last-child,
            [class=wrapper] .one-col-feature .column:last-child table:nth-last-child(2) td ul:last-child {
                Margin-bottom: 24px !important;
            }

            [class=wrapper] .address,
            [class=wrapper] .subscription {
                display: block;
                float: left;
                width: 318px !important;
                text-align: center !important;
            }

            [class=wrapper] .address {
                padding-bottom: 0 !important;
            }

            [class=wrapper] .subscription {
                padding-top: 0 !important;
            }

            [class=wrapper] h1 {
                font-size: 36px !important;
                line-height: 42px !important;
                Margin-bottom: 18px !important;
            }

            [class=wrapper] h2 {
                font-size: 26px !important;
                line-height: 32px !important;
                Margin-bottom: 20px !important;
            }

            [class=wrapper] h3 {
                font-size: 18px !important;
                line-height: 22px !important;
                Margin-bottom: 16px !important;
            }

            [class=wrapper] p,
            [class=wrapper] ol,
            [class=wrapper] ul {
                font-size: 16px !important;
                line-height: 24px !important;
                Margin-bottom: 24px !important;
            }

            [class=wrapper] ol,
            [class=wrapper] ul {
                Margin-left: 18px !important;
            }

            [class=wrapper] li {
                padding-left: 2px !important;
            }

            [class=wrapper] blockquote {
                padding-left: 16px !important;
            }

            [class=wrapper] .two-col .column:nth-child(n + 3) {
                border-top: 1px solid #e9e9e9;
            }

            [class=wrapper] .btn {
                margin-bottom: 24px !important;
            }

                [class=wrapper] .btn a {
                    display: block !important;
                    font-size: 13px !important;
                    font-weight: bold !important;
                    line-height: 15px !important;
                    padding: 10px 30px !important;
                }

            [class=wrapper] .column-bottom {
                font-size: 8px !important;
                line-height: 8px !important;
            }

            [class=wrapper] .first .column-bottom,
            [class=wrapper] .three-col .second .column-bottom {
                display: none;
            }

            [class=wrapper] .second .column-top,
            [class=wrapper] .third .column-top {
                display: none;
            }

            [class=wrapper] .image-frame {
                padding: 4px !important;
            }

            [class=wrapper] .header .logo {
                padding-left: 10px !important;
                padding-right: 10px !important;
            }

                [class=wrapper] .header .logo div {
                    font-size: 26px !important;
                    line-height: 32px !important;
                }

                    [class=wrapper] .header .logo div img {
                        display: inline-block !important;
                        max-width: 280px !important;
                        height: auto !important;
                    }

            [class=wrapper] table.border,
            [class=wrapper] .header,
            [class=wrapper] .webversion,
            [class=wrapper] .footer {
                width: 320px !important;
            }

                [class=wrapper] .preheader .webversion,
                [class=wrapper] .header .logo a {
                    text-align: center !important;
                }

            [class=wrapper] .preheader table,
            [class=wrapper] .border td {
                width: 318px !important;
            }

                [class=wrapper] .border td.border {
                    width: 1px !important;
                }

            [class=wrapper] .image .border td {
                width: auto !important;
            }

            [class=wrapper] .title {
                display: none;
            }

            [class=wrapper] .footer .padded {
                text-align: center !important;
            }

            [class=wrapper] .footer .subscription .padded {
                padding-top: 20px !important;
            }

            [class=wrapper] .footer .social-link {
                display: block !important;
            }

                [class=wrapper] .footer .social-link table {
                    margin: 0 auto 10px !important;
                }

            [class=wrapper] .footer .divider {
                display: none !important;
            }

            [class=wrapper] .one-col-feature .btn {
                margin-bottom: 28px !important;
            }

            [class=wrapper] .one-col-feature .image {
                margin-bottom: 28px !important;
            }

            [class=wrapper] .one-col-feature .divider .inner {
                padding-bottom: 28px !important;
            }

            [class=wrapper] .one-col-feature h1 {
                font-size: 42px !important;
                line-height: 48px !important;
                margin-bottom: 20px !important;
            }

            [class=wrapper] .one-col-feature h2 {
                font-size: 32px !important;
                line-height: 36px !important;
                margin-bottom: 18px !important;
            }

            [class=wrapper] .one-col-feature h3 {
                font-size: 26px !important;
                line-height: 32px !important;
                margin-bottom: 20px !important;
            }

            [class=wrapper] .one-col-feature p,
            [class=wrapper] .one-col-feature ol,
            [class=wrapper] .one-col-feature ul {
                font-size: 20px !important;
                line-height: 28px !important;
                margin-bottom: 28px !important;
            }

            [class=wrapper] .one-col-feature blockquote {
                font-size: 18px !important;
                line-height: 26px !important;
                margin-bottom: 28px !important;
                padding-bottom: 26px !important;
                padding-left: 0 !important;
                padding-top: 26px !important;
            }

                [class=wrapper] .one-col-feature blockquote p,
                [class=wrapper] .one-col-feature blockquote ol,
                [class=wrapper] .one-col-feature blockquote ul {
                    font-size: 26px !important;
                    line-height: 32px !important;
                }

                    [class=wrapper] .one-col-feature blockquote p:last-child,
                    [class=wrapper] .one-col-feature blockquote ol:last-child,
                    [class=wrapper] .one-col-feature blockquote ul:last-child {
                        margin-bottom: 0 !important;
                    }

            [class=wrapper] .one-col-feature .column table:last-of-type h1:last-child,
            [class=wrapper] .one-col-feature .column table:last-of-type h2:last-child,
            [class=wrapper] .one-col-feature .column table:last-of-type h3:last-child {
                margin-bottom: 28px !important;
            }
        }

        @media only screen and (max-width: 320px) {
            [class=wrapper] td.border {
                display: none;
            }

            [class=wrapper] table.border,
            [class=wrapper] .header,
            [class=wrapper] .webversion,
            [class=wrapper] .footer {
                width: 318px !important;
            }
        }
    </style>
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
        @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);
        @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);
        @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);
        @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);
        @import url(https://fonts.googleapis.com/css?family=Roboto:400,700,400italic,700italic);

        body, .wrapper, .emb-editor-canvas {
            background-color: #ededed;
        }

        .border {
            background-color: #dcdcdc;
        }

        h1 {
            color: #565656;
        }

        .wrapper h1 {
        }

        .wrapper h1 {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper h1 {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        h1 {
        }

        .one-col h1 {
            line-height: 44px;
        }

        .two-col h1 {
            line-height: 34px;
        }

        .three-col h1 {
            line-height: 26px;
        }

        .wrapper .one-col-feature h1 {
            line-height: 58px;
        }

        @media only screen and (max-width: 620px) {
            h1 {
                line-height: 44px !important;
            }
        }

        h2 {
            color: #555;
        }

        .wrapper h2 {
        }

        .wrapper h2 {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper h2 {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        h2 {
        }

        .one-col h2 {
            line-height: 34px;
        }

        .two-col h2 {
            line-height: 26px;
        }

        .three-col h2 {
            line-height: 22px;
        }

        .wrapper .one-col-feature h2 {
            line-height: 50px;
        }

        @media only screen and (max-width: 620px) {
            h2 {
                line-height: 34px !important;
            }
        }

        h3 {
            color: #555;
        }

        .wrapper h3 {
        }

        .wrapper h3 {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper h3 {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        h3 {
        }

        .one-col h3 {
            line-height: 26px;
        }

        .two-col h3 {
            line-height: 24px;
        }

        .three-col h3 {
            line-height: 20px;
        }

        .wrapper .one-col-feature h3 {
            line-height: 40px;
        }

        @media only screen and (max-width: 620px) {
            h3 {
                line-height: 26px !important;
            }
        }

        p, ol, ul {
            color: #565656;
        }

        .wrapper p, .wrapper ol, .wrapper ul {
        }

        .wrapper p, .wrapper ol, .wrapper ul {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper p, .wrapper ol, .wrapper ul {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        p, ol, ul {
        }

        .one-col p, .one-col ol, .one-col ul {
            line-height: 25px;
            Margin-bottom: 25px;
        }

        .two-col p, .two-col ol, .two-col ul {
            line-height: 22px;
            Margin-bottom: 22px;
        }

        .three-col p, .three-col ol, .three-col ul {
            line-height: 20px;
            Margin-bottom: 20px;
        }

        .wrapper .one-col-feature p, .wrapper .one-col-feature ol, .wrapper .one-col-feature ul {
            line-height: 30px;
        }

        .one-col-feature blockquote p, .one-col-feature blockquote ol, .one-col-feature blockquote ul {
            line-height: 50px;
        }

        @media only screen and (max-width: 620px) {
            p, ol, ul {
                line-height: 25px !important;
                Margin-bottom: 25px !important;
            }
        }

        .image {
            color: #565656;
        }

        .image {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .image {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        .wrapper a {
            color: #41637e;
        }

            .wrapper a:hover {
                color: #30495c !important;
            }

        .wrapper .logo div {
            color: #41637e;
        }

        .wrapper .logo div {
            font-family: sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper .logo div {
                font-family: Avenir,sans-serif !important;
            }
        }

        .wrapper .logo div a {
            color: #41637e;
        }

            .wrapper .logo div a:hover {
                color: #41637e !important;
            }

        .wrapper .one-col-feature p a, .wrapper .one-col-feature ol a, .wrapper .one-col-feature ul a {
            border-bottom: 1px solid #41637e;
        }

            .wrapper .one-col-feature p a:hover, .wrapper .one-col-feature ol a:hover, .wrapper .one-col-feature ul a:hover {
                color: #30495c !important;
                border-bottom: 1px solid #30495c !important;
            }

        .btn a {
        }

        .wrapper .btn a {
        }

        .wrapper .btn a {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper .btn a {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        .wrapper .btn a {
            background-color: #41637e;
            color: #fff !important;
            outline-color: #41637e;
            text-shadow: 0 1px 0 #3b5971;
        }

            .wrapper .btn a:hover {
                background-color: #3b5971 !important;
                color: #fff !important;
                outline-color: #3b5971 !important;
            }

        .preheader .title, .preheader .webversion, .footer .padded {
            color: #737373;
        }

        .preheader .title, .preheader .webversion, .footer .padded {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .preheader .title, .preheader .webversion, .footer .padded {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        .preheader .title a, .preheader .webversion a, .footer .padded a {
            color: #737373;
        }

            .preheader .title a:hover, .preheader .webversion a:hover, .footer .padded a:hover {
                color: #4d4d4d !important;
            }

        .footer .social .divider {
            color: #dcdcdc;
        }

        .footer .social .social-text, .footer .social a {
            color: #737373;
        }

        .wrapper .footer .social .social-text, .wrapper .footer .social a {
        }

        .wrapper .footer .social .social-text, .wrapper .footer .social a {
            font-family: Tahoma,sans-serif;
        }

        @media only screen and (min-width: 0) {
            .wrapper .footer .social .social-text, .wrapper .footer .social a {
                font-family: Roboto,Tahoma,sans-serif !important;
            }
        }

        .footer .social .social-text, .footer .social a {
        }

            .footer .social .social-text:hover, .footer .social a:hover {
                color: #4d4d4d !important;
            }

        .image .border {
            background-color: #c8c8c8;
        }

        .image-frame {
            background-color: #dadada;
        }

        .image-background {
            background-color: #f7f7f7;
        }
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
                <tr><td style='padding: 0;vertical-align: top'></td></tr>
            </tbody>
        </table>

        <table class='centered' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto'>
            <tbody>
                <tr>
                    <td class='border' style='padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px'></td>
                    <td style='padding: 0;vertical-align: top'>
                        <table class='one-col' style='border-collapse: collapse;border-spacing: 0;Margin-left: auto;Margin-right: auto;width: 600px;background-color: #ffffff;font-size: 14px;table-layout: fixed'>
                            <tbody>
                                <tr>
                                    <td class='column' style='padding: 0;vertical-align: top;text-align: left'>

                                        <div class='image' style='font-size: 12px;Margin-bottom: 24px;mso-line-height-rule: at-least;color: #565656;font-family: Tahoma,sans-serif' align='center'>
<img class='gnd-corner-image gnd-corner-image-center gnd-corner-image-top' style='border: 0;-ms-interpolation-mode: bicubic;display: block;max-width: 659px' src='" +
                                         img
                                        + @"' alt='' width='600' height='138' />
                                        </div>

                                        <table class='contents' style='border-collapse: collapse;border-spacing: 0;table-layout: fixed;width: 100%'>
                                            <tbody>
                                                <tr>
                                                    <td class='padded' style='padding: 0;vertical-align: top;padding-left: 32px;padding-right: 32px;word-break: break-word;word-wrap: break-word'><TABLE style='BORDER-COLLAPSE: collapse; BORDER-SPACING: 0'>
 <TBODY>
  <TR >
   <TD  style='WIDTH: 80%;'>"
    + titulo +
       @"
   </TD>
  </TR>
 </TBODY>
</TABLE>
<div style='padding-left: 32px;padding-right: 32px;'>
" +
                                                        cuerpo
+ @"</div>
</td>
                                                </tr>
                                            </tbody>
                                        </table>

                                        <div class='column-bottom' style='font-size: 8px;line-height: 8px'>&nbsp;</div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td class='border' style='padding: 0;vertical-align: top;font-size: 1px;line-height: 1px;background-color: #dcdcdc;width: 1px'></td>
                </tr>
            </tbody>
        </table>

        <table class='border' style='border-collapse: collapse;border-spacing: 0;font-size: 1px;line-height: 1px;background-color: #dcdcdc;Margin-left: auto;Margin-right: auto' width='602'>
            <tbody>
                <tr><td style='padding: 0;vertical-align: top'></td></tr>
            </tbody>
        </table>


        <table class='footer centered bg' style=' background: url(' http:='' extranet.resocentro.com:5050='' paginaweb='' correo='' pixelbg_5px.png');='' border-collapse:='' collapse;border-spacing:='' 0;margin-left:='' auto;height:40px;='' margin-right:='' auto;width:='' 602px'=''>
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
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/fb.png' width='26' height='23'>
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
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/twt.png' width='26' height='23'>
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
                                                            <img style='border: 0;-ms-interpolation-mode: bicubic;display: block' src='http://extranet.resocentro.com:5050/PaginaWeb/correo/glg.png' width='26' height='23'>
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
                        </table><br>
                         <span style='color:#BA0E0E;font-size:11px'>
                            No responder este e-mail ya que es autogenerado, tenga presente que el horario de envío de respuestas es de 9:00 am. a 5:00 p.m. <a href='http://www.resocentro.com' target='_blank'>www.resocentro.com</a> 
                        </span>
                        </td>
                    </tr>
            </tbody>
        </table>
    </center>

</body></html>
";
            return msj.Replace("'", "\"");
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

        }
        public bool CombinarPDF_Image(string strFileTarget, string[] arrStrFilesSource)
        {
            bool blnMerged = false;
            //Document objDocument = new Document();
            // Crea el PDF de salida
            try
            {
                //PdfWriter objWriter = PdfWriter.GetInstance(objDocument, new FileStream(strFileTarget, FileMode.OpenOrCreate));
                //objDocument.Open();
                //// Recorre los archivos
                //for (int intIndexFile = 0; intIndexFile < arrStrFilesSource.Length; intIndexFile++)
                //{

                //    if (!File.Exists(arrStrFilesSource[intIndexFile]))
                //    {
                //        throw new System.InvalidOperationException("El archivo no exite o se movio de la ruta \"" + arrStrFilesSource[intIndexFile] + "\"");
                //    }
                //    string[] extencion = arrStrFilesSource[intIndexFile].Split('.');
                //    if (extencion[extencion.Count() - 1] == "pdf")
                //    {
                //        PdfReader objReader = new PdfReader(arrStrFilesSource[intIndexFile]);
                //        int intNumberOfPages = objReader.NumberOfPages;

                //        // Añade las páginas
                //        for (int intPage = 0; intPage < intNumberOfPages; intPage++)
                //        {
                //            int intRotation = objReader.GetPageRotation(intPage + 1);
                //            PdfImportedPage objPage = objWriter.GetImportedPage(objReader, intPage + 1);

                //            // Asigna el tamaño de la página
                //            objDocument.SetPageSize(objReader.GetPageSizeWithRotation(intPage + 1));
                //            // Crea una nueva página
                //            objDocument.NewPage();
                //            // Añade la página leída
                //            if (intRotation == 90 || intRotation == 270)
                //                objWriter.DirectContent.AddTemplate(objPage, 0, -1f, 1f, 0, 0,
                //                                                    objReader.GetPageSizeWithRotation(intPage + 1).Height);
                //            else
                //                objWriter.DirectContent.AddTemplate(objPage, 1f, 0, 0, 1f, 0, 0);
                //        }
                //        objReader.Close();
                //    }
                //    else
                //    {
                //        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(arrStrFilesSource[intIndexFile]);
                //        image.SetAbsolutePosition(1, 1);
                //        image.SpacingBefore = 10f;
                //        image.Alignment = Element.ALIGN_MIDDLE;
                //        objDocument.NewPage();
                //        objDocument.Add(image);
                //    }
                //}
                //// Indica que se ha creado el documento
                //blnMerged = true;
                using (System.IO.FileStream stmFile = new System.IO.FileStream(strFileTarget, System.IO.FileMode.Create))
                {
                    Document objDocument = null;
                    PdfWriter objWriter = null;

                    // Recorre los archivos
                    for (int intIndexFile = 0; intIndexFile < arrStrFilesSource.Length; intIndexFile++)
                    {
                        string[] extencion = arrStrFilesSource[intIndexFile].Split('.');
                        if (extencion[extencion.Count() - 1] == "pdf")
                        {
                            PdfReader objReader = new PdfReader(arrStrFilesSource[intIndexFile]);
                            int intNumberOfPages = objReader.NumberOfPages;

                            // La primera vez, inicializa el documento y el escritor
                            if (intIndexFile == 0)
                            { // Asigna el documento y el generador
                                objDocument = new Document(objReader.GetPageSizeWithRotation(1));
                                objWriter = PdfWriter.GetInstance(objDocument, stmFile);
                                // Abre el documento
                                objDocument.Open();
                            }
                            // Añade las páginas
                            for (int intPage = 0; intPage < intNumberOfPages; intPage++)
                            {
                                int intRotation = objReader.GetPageRotation(intPage + 1);
                                PdfImportedPage objPage = objWriter.GetImportedPage(objReader, intPage + 1);

                                // Asigna el tamaño de la página
                                objDocument.SetPageSize(objReader.GetPageSizeWithRotation(intPage + 1));
                                // Crea una nueva página
                                objDocument.NewPage();
                                // Añade la página leída
                                if (intRotation == 90 || intRotation == 270)
                                    objWriter.DirectContent.AddTemplate(objPage, 0, -1f, 1f, 0, 0,
                                                                        objReader.GetPageSizeWithRotation(intPage + 1).Height);
                                else
                                    objWriter.DirectContent.AddTemplate(objPage, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                        else
                        {
                            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(arrStrFilesSource[intIndexFile]);
                            //image.SetAbsolutePosition(1, 1);

                            if (intIndexFile == 0)
                            { // Asigna el documento y el generador
                                objDocument = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                                objWriter = PdfWriter.GetInstance(objDocument, stmFile);

                                // Abre el documento
                                objDocument.Open();
                            }
                            image.ScaleToFit(objDocument.PageSize);
                            image.SpacingBefore = 10f;
                            image.Alignment = Element.ALIGN_LEFT;
                            objDocument.NewPage();
                            objDocument.Add(image);

                        }

                    }
                    // Cierra el documento
                    if (objDocument != null)
                        objDocument.Close();
                    // Cierra el stream del archivo
                    stmFile.Close();
                }
                // Indica que se ha creado el documento
                blnMerged = true;
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.Message, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //finally
            //{
            //    objDocument.Close();
            //}
            // Devuelve el valor que indica si se han mezclado los archivos
            return blnMerged;
        }
        public bool CombinarPDF(string strFileTarget, string[] arrStrFilesSource)
        {
            bool blnMerged = false;

            //Creamos un tipo de archivo que solo se cargará en la memoria principal
            Document objDocument = new Document();
            try
            {
                PdfCopy pdfCopy = new PdfCopy(objDocument, new FileStream(strFileTarget, FileMode.OpenOrCreate));
                objDocument.Open();

                // Recorre los archivos
                for (int intIndexFile = 0; intIndexFile < arrStrFilesSource.Length; intIndexFile++)
                {

                    if (!File.Exists(arrStrFilesSource[intIndexFile]))
                    {
                        throw new System.InvalidOperationException("El archivo no exite o se movio de la ruta \"" + arrStrFilesSource[intIndexFile] + "\"");
                    }
                    string[] extencion = arrStrFilesSource[intIndexFile].Split('.');
                    if (extencion[extencion.Count() - 1] == "pdf")
                    {
                        PdfReader objReader = new PdfReader(arrStrFilesSource[intIndexFile]);
                        int intNumberOfPages = objReader.NumberOfPages;

                        // Añade las páginas
                        for (int intPage = 1; intPage <= intNumberOfPages; intPage++)
                        {
                            objDocument.NewPage();
                            PdfImportedPage importedPage = pdfCopy.GetImportedPage(objReader, intPage);
                            PdfCopy.PageStamp pageStamp = pdfCopy.CreatePageStamp(importedPage);

                            pageStamp.AlterContents();
                            pdfCopy.AddPage(importedPage);
                        }
                        pdfCopy.FreeReader(objReader);
                        objReader.Close();
                    }
                    else
                    {
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(arrStrFilesSource[intIndexFile]);
                        image.SetAbsolutePosition(1, 1);
                        image.SpacingBefore = 10f;
                        image.Alignment = Element.ALIGN_CENTER;
                        objDocument.NewPage();
                        objDocument.Add(image);
                    }
                }


                blnMerged = true;
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.Message, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {

                objDocument.Close();
            }
            // Devuelve el valor que indica si se han mezclado los archivos
            return blnMerged;
        }


        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }


        public void exportGrid(RadGridView grid, bool header, bool footer, bool group)
        {
            string extension = "xls";
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "Excel"),
                FilterIndex = 1
            };
            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    grid.Export(stream,
                     new GridViewExportOptions()
                     {
                         Format = ExportFormat.ExcelML,
                         ShowColumnHeaders = header,
                         ShowColumnFooters = footer,
                         ShowGroupFooters = group,
                     });
                }
            }
        }
    }
    public enum Estado_SolicitudTramite
    {
        Nuevo = 0,
        Leido = 1,
        Espera = 2,
        Tramitado = 3,
        Cancelado = 4
    }
    public enum Tipo_Log
    {
        Lectura = 0,
        Update = 1,
        Insert = 2,
        Delete = 3,
    }
    public enum Tipo_Permiso
    {
        Todos=0,
Proforma = 1,
Carta_Garantía = 2,
Revisar_Carta = 3,
Visor_Carta = 4,
Sistemas = 5,
Generar_Código = 6,
Registrar_Motivo = 7,
Historial_Código = 8,
Herramientas_Cartas = 9,
Herramientas_Atención_al_Cliente = 10,
Ticketera = 11,
Reporte_Ticketera = 12,
Mantenimiento_Ticketera = 13,
Enviar_Modalidad = 14,
Consultas_Web = 15,
Médicos = 16,
Instituciones_Médicas = 17,
Asignaciones_Médicas = 18,
Planificación = 19,
Herramientas_Representantes_Médicos = 20,
Revisar_Planificación_Representante = 21,
Reporte_Envio_Médicos = 22,
Reporte_Envio_Procedencia = 23,
Ubicaciones_Representantes = 24,
Solicitudes_Carta_Web = 25,
Visor_Sala_de_Médicos = 26,
Realizar_Citas = 27,
Confirma_Citas = 28,
Aprobar_Entre_Turno = 29,
Central_Telefonica = 30,
Compañia_de_Seguro = 31,
Colaborador = 32,
Pago_Anestesiólogos = 33,
Pago_Médico = 34,
Pago_Postprocesadores = 35,
Pago_Tecnólogo = 36,
Asignación_Anestesiólogo = 37,
Control_Área_Médica = 38,
Colaboradores = 39,
Entrega_Resultados = 40,
Realizar_Cobranza = 41,
Documetos_Emitidos = 42,
Cierre_Caja = 43,
Facturar_Compañias = 44,
Registrar_Forma_de_Pago = 45,
Realizar_Nota_CreditoDebito = 46,
Resumen_Diario = 47,
Cobranza_Externa = 48,
PreFacturación_Global = 49,
Facturacion_Global = 50,
Reporte_Contable = 51,
Reporte_Produccion = 52,
Anular_Documento = 53,
Reporte_Ventas = 54,
Registrat_Fecha_Recibo_CIA = 55,
Generar_Caratula_Factura_CIA = 56,
Actualizar_Fecha_Pago_CIA = 57,
Recibo_Provisional = 58,
Aprobar_Cancelacion_Recibo_Provisional = 59,
Documentos_Cancelados = 60,
MANT__Empresa_Servicio = 61,
MANT__Incidente = 62,
MANT__Equipos = 63,
Asignar_Cortesias = 64,
Horario_Tecnologo=65,
Horario_Encuestador = 66,
Horario_Supervisor = 67,
Horario_Enfermera = 68,
Horario_Postprocesado = 69
    }

    public enum ERROR_ANULACION_DOCUMENTO
    {

        NO_EXISTE = 0,
        DOCUMENTO_RESTRINGIDO = 1,
        ERROR_CONSULTA = 2,
        CORRECTO = 3,
        DOCUMENTO_NO_ENVIADO=4
    }

}
