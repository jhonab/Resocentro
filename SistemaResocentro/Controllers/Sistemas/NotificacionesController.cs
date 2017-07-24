using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaResocentro.Controllers.Sistemas
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Notificaciones
        public ActionResult getSolicitudesSoporte()
        {
            var lista = db.Incidente.Where(x => x.estado == "N")
                .Select(x =>
                  new
                  {
                      nombre = x.USUARIO1.ShortName,
                      id = x.idIncidente,
                      minutos = SqlFunctions.DateDiff("month", x.fec_registro, SqlFunctions.GetDate()) > 0 ?
                           SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", x.fec_registro, SqlFunctions.GetDate())) + " mes(es)" :
                           SqlFunctions.DateDiff("day", x.fec_registro, SqlFunctions.GetDate()) > 0 ?
                           SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", x.fec_registro, SqlFunctions.GetDate())) + " día(s)" :
                           SqlFunctions.DateDiff("minute", x.fec_registro, SqlFunctions.GetDate()) > 0 ?
                           SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", x.fec_registro, SqlFunctions.GetDate())) + " minuto(s)" :
                           "Ahora",
                  })
                .ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VisorSalaMedica()
        {
            return View();
        }
        public ActionResult VisorCargaEquipo()
        {
            return View();
        }
        public ActionResult getlistEncuesta(string sede, string[] total)
        {
            string formato = @"
                    <table class='table  table-bordered '>
                        <tbody>
                           {0}
                        </tbody>
                        <tbody></tbody>
                    </table>";

            var lista = (from ea in db.EXAMENXATENCION
                         join ec in db.EXAMENXCITA on new { estudio = ea.codigoestudio, cita = ea.numerocita } equals new { estudio = ec.codigoestudio, cita = ec.numerocita }
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                         from en in en_join.DefaultIfEmpty()
                         where
                         (ea.equipoAsignado == null || ea.equipoAsignado == 0)
                         && ea.estadoestudio == "A"
                         && ea.codigoestudio.Substring(1 - 1, 3) == sede
                         orderby ea.codigo
                         select new
                         {
                             apellidos = ea.ATENCION.PACIENTE.apellidos,
                             cita=ec.horacita,
                             numero = ea.codigo,
                             estudio = ea.ESTUDIO.nombreestudio,
                             min_transcurri = ea.ATENCION.fechayhora,
                         }
                             ).AsParallel().ToList();
            string items = "";
            bool alarm = false;
            foreach (var item in lista)
            {

                var pac = "";
                var hoy = DateTime.Now;
                var cita = new DateTime(hoy.Year, hoy.Month, hoy.Day, item.min_transcurri.Hour, item.min_transcurri.Minute, 0);
                TimeSpan ts = hoy - cita;
                var min = Math.Round(ts.TotalMinutes, 0);// int.Parse(item.min_transcurri);
                var claseCss = "";
                if (item.apellidos.Length > 14)
                    pac = item.apellidos.Substring(0, 14) + ".";
                else
                    pac = item.apellidos;
                if (min <= 3)
                    claseCss = "";
                else if (min <= 10)
                    claseCss = "bg-yellow";
                else
                    claseCss = "bg-red";

                if (total == null)
                {
                    total = new string[1];
                    total[0] = "0";
                }

                if (!total.Contains(item.numero.ToString()))
                {
                    alarm = true;
                    claseCss = "new-item";
                }

                items += string.Format(@" <tr class='odd'><td class='{0}'><div class='col-md-12' style='padding:0px;margin:0px'><div class='col-md-10' style='padding:0px;margin:0px'><div style='font-weight: bold;'>" + pac + "</div><div style='font-size:16px;' class='subtitulo'># " + item.numero.ToString() + " - " + item.estudio + " ("+item.cita.ToString("HH:mm")+") " + min + "'</div></div><div class='col-md-2' style='padding:0px;margin:0px'><i class='fa fa-edit  pull-right subtitulo' style='margin-top: 20px;'></i></div></div></td></tr>", claseCss);



            }
            total = lista.Select(x => x.numero.ToString()).ToArray();
            string resultado = string.Format(formato, items);
            return Json(new { resultado, total, alarm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getlistTecnologo(string sede, string[] total)
        {
            string formato = @"
                    <table class='table  table-bordered '>
                        <tbody>
                           {0}
                        </tbody>
                        <tbody></tbody>
                    </table>";

            var lista = (from ea in db.EXAMENXATENCION
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                         from en in en_join.DefaultIfEmpty()
                         join e in db.EQUIPO on new { equipoAsignado = (int)ea.equipoAsignado } equals new { equipoAsignado = e.codigoequipo } into e_join
                         from e in e_join.DefaultIfEmpty()
                         where
                          ea.estadoestudio == "A"
                         && ea.codigoestudio.Substring(1 - 1, 3) == sede
                          && en.estado >= 1
                          && en.estado <= 2
                          && en.fec_ini_tecno == null
                         orderby ea.codigo
                         select new
                         {

                             apellidos = ea.ATENCION.PACIENTE.apellidos,
                             numero = ea.codigo,
                             estudio = ea.ESTUDIO.nombreestudio,
                             equipo = e.nombreequipo,
                             min_transcurri = ea.horaatencion,
                         }
                             ).AsParallel().ToList();
            string items = "";
            bool alarm = false;
            foreach (var item in lista)
            {
                var pac = "";
                var hoy = DateTime.Now;
                var cita = new DateTime(hoy.Year, hoy.Month, hoy.Day, item.min_transcurri.Hour, item.min_transcurri.Minute, 0);
                TimeSpan ts = hoy - cita;
                var min = Math.Round(ts.TotalMinutes, 0);// int.Parse(item.min_transcurri);
                var claseCss = "";
                if (item.apellidos.Length > 14)
                    pac = item.apellidos.Substring(0, 14) + ".";
                else
                    pac = item.apellidos;

                if (min < 0)
                    min = 0;

                if (min <= 9)
                    claseCss = "";
                else if (min <= 20)
                    claseCss = "bg-yellow";
                else
                    claseCss = "bg-red";

                if (total == null)
                {
                    total = new string[1];
                    total[0] = "0";
                }

                if (!total.Contains(item.numero.ToString()))
                {
                    alarm = true;
                    claseCss = "new-item";
                }
                items += string.Format(@" <tr class='odd'><td class='{0}'><div class='col-md-12' style='padding:0px;margin:0px'><div class='col-md-10' style='padding:0px;margin:0px'><div style='font-weight: bold;'>" + pac + "</div><div style='font-size:16px;' class='subtitulo'># " + item.numero.ToString() + " - " + item.estudio + " (" + item.equipo + ") " + min + "'</div></div><div class='col-md-2' style='padding:0px;margin:0px'><i class='fa fa-joomla   pull-right subtitulo' style='margin-top: 20px;'></i></div></div></td></tr>", claseCss);

            }
            total = lista.Select(x => x.numero.ToString()).ToArray();
            string resultado = string.Format(formato, items);
            return Json(new { resultado, total, alarm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getlistSupervisor(string sede, string[] total)
        {
            string formato = @"
                    <table class='table  table-bordered '>
                        <tbody>
                           {0}
                        </tbody>
                        <tbody></tbody>
                    </table>";

            var lista = (from ea in db.EXAMENXATENCION
                         join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
                         from en in en_join.DefaultIfEmpty()
                         join e in db.EQUIPO on new { equipoAsignado = (int)ea.equipoAsignado } equals new { equipoAsignado = e.codigoequipo } into e_join
                         from e in e_join.DefaultIfEmpty()
                         where
                          ea.estadoestudio == "A"
                         && ea.codigoestudio.Substring(1 - 1, 3) == sede
                         && en.estado == 1
                             //&& en.estado < 4
                         && en.SolicitarValidacion == true
                         orderby ea.codigo
                         select new
                         {

                             apellidos = ea.ATENCION.PACIENTE.apellidos,
                             numero = ea.codigo,
                             estudio = ea.ESTUDIO.nombreestudio,
                             equipo = e.nombreequipo,
                             min_transcurri = en.fec_Solicitavalidacion,
                         }
                             ).AsParallel().ToList();
            string items = "";
            bool alarm = false;
            foreach (var item in lista)
            {
                var pac = "";
                var hoy = DateTime.Now;
                var cita = new DateTime(hoy.Year, hoy.Month, hoy.Day, item.min_transcurri.Value.Hour, item.min_transcurri.Value.Minute, 0);
                TimeSpan ts = hoy - cita;
                var min = Math.Round(ts.TotalMinutes, 0);// int.Parse(item.min_transcurri);
                var claseCss = "";
                if (item.apellidos.Length > 14)
                    pac = item.apellidos.Substring(0, 14) + ".";
                else
                    pac = item.apellidos;

                if (min < 0)
                    min = 0;

                if (min <= 9)
                    claseCss = "";
                else if (min <= 15)
                    claseCss = "bg-yellow";
                else
                    claseCss = "bg-red";
                if (total == null)
                {
                    total = new string[1];
                    total[0] = "0";
                }

                if (!total.Contains(item.numero.ToString()))
                {
                    alarm = true;
                    claseCss = "new-item";
                }
                items += string.Format(@" <tr class='odd'><td class='{0}'><div class='col-md-12' style='padding:0px;margin:0px'><div class='col-md-10' style='padding:0px;margin:0px'><div style='font-weight: bold;'>" + pac + "</div><div style='font-size:16px;' class='subtitulo'># " + item.numero.ToString() + " - " + item.estudio + " (" + item.equipo + ") " + min + "'</div></div><div class='col-md-2' style='padding:0px;margin:0px'><i class='fa fa-check-square-o   pull-right subtitulo' style='margin-top: 20px;'></i></div></div></td></tr>", claseCss);



            }
            total = lista.Select(x => x.numero.ToString()).ToArray();
            string resultado = string.Format(formato, items);
            return Json(new { resultado, total, alarm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getlistSatisfaccion(string sede, string[] total)
        {
            string formato = @"
                    <table class='table  table-bordered '>
                        <tbody>
                           {0}
                        </tbody>
                        <tbody></tbody>
                    </table>";

            var lista = (from ea in db.EXAMENXATENCION
                         join enc in db.Encuesta on ea.codigo equals enc.numeroexamen
                         join en in db.Encuesta_Satisfaccion on ea.numeroatencion equals en.numeroatecion
                         where
                          ea.estadoestudio == "R"
                          && ea.codigoestudio.Substring(1 - 1, 3) == sede
                         && en.isTerminado == null
                         group ea by new
                         {
                             ea.ATENCION.PACIENTE.apellidos,
                             ea.numeroatencion,
                             enc.fec_fin_tecno
                         } into atencion
                         select new
                         {

                             apellidos = atencion.Key.apellidos,
                             numero = atencion.Key.numeroatencion,
                             min_transcurri = atencion.Key.fec_fin_tecno,
                         }
                             ).AsParallel().ToList();
            string items = "";
            bool alarm = false;
            foreach (var item in lista)
            {
                var pac = "";
                var hoy = DateTime.Now;
                var cita = new DateTime(hoy.Year, hoy.Month, hoy.Day, item.min_transcurri.Value.Hour, item.min_transcurri.Value.Minute, 0);
                TimeSpan ts = hoy - cita;
                var min = Math.Round(ts.TotalMinutes, 0);// int.Parse(item.min_transcurri);
                var claseCss = "";
                if (item.apellidos.Length > 14)
                    pac = item.apellidos.Substring(0, 14) + ".";
                else
                    pac = item.apellidos;

                if (min < 0)
                    min = 0;

                if (min <= 9)
                    claseCss = "";
                else if (min <= 15)
                    claseCss = "bg-yellow";
                else
                    claseCss = "bg-red";

                if (total == null)
                {
                    total = new string[1];
                    total[0] = "0";
                }

                if (!total.Contains(item.numero.ToString()))
                {
                    alarm = true;
                    claseCss = "new-item";
                }

                items += string.Format(@" <tr class='odd'><td class='{0}'><div class='col-md-12' style='padding:0px;margin:0px'><div class='col-md-10' style='padding:0px;margin:0px'><div style='font-weight: bold;'>" + pac + "</div><div style='font-size:16px;' class='subtitulo'># " + item.numero.ToString() + " " + min + "'</div></div><div class='col-md-2' style='padding:0px;margin:0px'><i class='fa fa-child pull-right subtitulo' style='margin-top: 20px;'></i></div></div></td></tr>", claseCss);

            }
            total = lista.Select(x => x.numero.ToString()).ToArray();
            string resultado = string.Format(formato, items);
            return Json(new { resultado, total, alarm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getCargaEquipo(int sede, string[] total)
        {
            string format = @"<div class='col-md-x' style='font-size:medium;'>{0}<div class='panel panel-primary' style='margin-bottom:10px;'><div class='triangulo_sup'></div><div class='panel-heading'><h1 class='panel-title' style='font-size: 30px;text-align:center;'>{1}</h1></div></div></div>";

            var equipo = (from eq in db.EQUIPO
                          where eq.estado == "1" &&
                          (eq.codigounidad2 * 100 + eq.codigosucursal2) == sede //sucursales asignadas
                          select eq).AsParallel().ToList();

            string items = "";
            bool alarm = false;
            string claseCss = "";
            List<string> _equipo = new List<string>();
            foreach (var item in equipo)
            {

                var cant = (from ea in db.EXAMENXATENCION
                            join en in db.Encuesta on new { numeroexamen = ea.codigo } equals new { numeroexamen = en.numeroexamen }
                            where ea.equipoAsignado == item.codigoequipo
                            && ea.estadoestudio == "A"
                            && en.fec_fin_tecno == null

                            orderby ea.equipoAsignado
                            select ea).ToList().Count();
                string _can = "";
                if (cant == 0)
                {
                    if (total == null)
                    {
                        total = new string[1];
                        total[0] = "0";
                    }
                    if (!total.Contains(item.nombreequipo.ToString()))
                    {
                        alarm = true;
                        claseCss = "parpadeo";
                    }
                    _equipo.Add(item.nombreequipo);
                    _can = "<div style='text-align: center;font-size: 93px;'class='bg-red div-cant " + claseCss + "'>0 <small style='font-size: 33px;'>est.</small></div>";
                }
                else
                {
                    if (cant == 1)
                        _can = "<div style='text-align: center;font-size: 93px;' class='bg-yellow div-cant'>" + cant.ToString() + " <small style='font-size: 33px;'>est.</small></div>";
                    else
                        _can = "<div style='text-align: center;font-size: 93px;' class='div-cant'>" + cant.ToString() + " <small style='font-size: 33px;'>est.</small></div>";
                }
                items += string.Format(format, _can, item.nombreequipo);

            }
            total = _equipo.ToArray();
            return Json(new { resultado = items, total, alarm }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getListCarga(int sede, string[] total)
        {
            string format = @"<div class='col-md-x' style='font-size:medium;'><div class='panel panel-primary' style='margin-bottom:10px;'><div class='panel-heading'><h1 class='panel-title' style='font-size: 30px;text-align:center;'>{1}</h1></div></div><div class='triangulo'></div>{0}</div>";
            string format_data = @" <table class='table  table-bordered '>
                        <tbody>
                           {0}
                        </tbody>
                        <tbody></tbody>
                    </table>";

            var equipo = (from eq in db.EQUIPO
                          where eq.estado == "1" &&
                          (eq.codigounidad2 * 100 + eq.codigosucursal2) == sede //sucursales asignadas
                          select eq).AsParallel().ToList();

            string items = "";

            bool alarm = false;
            List<string> _equipo = new List<string>();
            foreach (var item in equipo)
            {

                var _lst = (from ea in db.EXAMENXATENCION
                            join en in db.Encuesta on new { numeroexamen = ea.codigo } equals new { numeroexamen = en.numeroexamen }
                            where ea.equipoAsignado == item.codigoequipo
                            && ea.estadoestudio == "A"
                          //  && en.fec_fin_tecno == null
                          //  && en.fec_ini_supervisa==null
                          //   && en.estado >= 1
                          //&& en.estado <= 3
                          && en.fec_fin_tecno == null
                            orderby ea.equipoAsignado
                            select new
                            {
                                apellidos = ea.ATENCION.PACIENTE.apellidos,
                                numero = ea.codigo,
                                equipo = item.nombreequipo,
                                min_transcurri = en.fec_paso3,
                                encuesta=en
                            }).ToList();
                string itemsData = "";
                foreach (var _det in _lst)
                {
                    var pac = "";
                    var hoy = DateTime.Now;
                    var cita = new DateTime(hoy.Year, hoy.Month, hoy.Day, _det.min_transcurri.Value.Hour, _det.min_transcurri.Value.Minute, 0);
                    TimeSpan ts = hoy - cita;
                    var min = Math.Round(ts.TotalMinutes, 0);// int.Parse(item.min_transcurri);
                    var claseCss = "";
                    var icono = "";
                    if (_det.apellidos.Length > 14)
                        pac = _det.apellidos.Substring(0, 14) + ".";
                    else
                        pac = _det.apellidos;

                    if (min < 0)
                        min = 0;

                    if (min <= 20)
                        claseCss = "";
                    else if (min <= 45)
                        claseCss = "bg-yellow";
                    else
                        claseCss = "bg-red";

                    if (!(_det.encuesta.fec_ini_supervisa == null))
                        icono = "<i class='fa fa-check  pull-right subtitulo' style='margin-top: 20px;'></i>";
                    else if (!(_det.encuesta.SolicitarValidacion == null))
                        icono = "<i class='fa fa-spinner fa-spin  pull-right subtitulo' style='margin-top: 20px;'></i>";
                    else if (_det.encuesta.fec_ini_tecno != null)
                        icono = "<i class='fa fa-joomla fa-spin  pull-right subtitulo' style='margin-top: 20px;'></i>";
                    else
                        icono = "";
                    if (total == null)
                    {
                        total = new string[1];
                        total[0] = "0";
                    }

                    if (!total.Contains(_det.numero.ToString()))
                    {
                        alarm = true;
                        claseCss = "new-item";
                    }
                    itemsData += string.Format(@" <tr class='odd'><td class='{0}'><div class='col-md-12' style='padding:0px;margin:0px'><div class='col-md-10' style='padding:0px;margin:0px'><div style='font-weight: bold;'>" + pac + "</div><div style='font-size:16px;' class='subtitulo'># " + _det.numero.ToString() + " " + min + "'</div></div><div class='col-md-2' style='padding:0px;margin:0px'>{1}</div></div></td></tr>", claseCss,icono);
                    _equipo.Add(_det.numero.ToString());
                }

                string tabla = string.Format(format_data, itemsData);
                items += string.Format(format, tabla, item.nombreequipo);

            }
            total = _equipo.ToArray();
            return Json(new { resultado = items, total, alarm }, JsonRequestBehavior.AllowGet);
        }
    }

}
public class Visor_Medico_Carga { }