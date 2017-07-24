using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SistemaResocentro.Controllers.Promotores
{
    [Authorize]
    public class PlanificacionController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Planificacion
        public ActionResult VisitasPlanificadas()
        {
            return View();
        }

        public ActionResult VisitasxDia(DateTime fecha, bool? show)
        {
            ViewBag.Visible = show == null ? true : show.Value;
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            var _u = db.USUARIO.SingleOrDefault(x => x.codigousuario == cod_usu);
            ViewBag.Restringido = _u.User_Uid == null ? "" : _u.User_Uid;
            var lista = db.Planificacion.Where(x => x.codigopromotor == cod_usu && x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList();
            return View(lista);
        }
        public ActionResult RegistrarVisita()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            ViewBag.Institucion = new SelectList(db.Institucion_Promotor.Where(x => x.codigopromotor == cod_usu).Select(x => new { id = x.codigoclinica, nombre = x.CLINICAHOSPITAL.razonsocial }), "id", "nombre");
            ViewBag.Medico = new SelectList(new Variable().getVacio(), "id", "nombre");
            ViewBag.Motivos = new SelectList(new Variable().getMotivosPlanificacion(), "nombre", "nombre");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarVisita(Planificacion item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            item.codigozona = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == item.codigoclinica).codigozona;
            item.codigopromotor = cod_usu;
            item.isRealizada = false;
            item.isCorrecto = false;//
            db.Planificacion.Add(item);
            db.SaveChanges();
            return RedirectToAction("VisitasPlanificadas", new { tab = 2 });
        }
        public ActionResult ConfirmarCheckin(int id)
        {
            return View(db.Planificacion.SingleOrDefault(x => x.id == id));
        }
        public ActionResult RegistrarPago(int id)
        {
            Pagos_PlanificacionViewModel pago = new Pagos_PlanificacionViewModel();
            pago.lista = db.Planificacion_Documento.Where(x => x.idplanificacion == id).ToList();
            pago.items = new Planificacion_Documento();
            pago.items.idplanificacion = id;
            ViewBag.Gasto = new SelectList(new Variable().getTipoGastoPlanificacion(), "codigo", "nombre");
            ViewBag.Categoria = new SelectList(new Variable().getCategoriasGastoPlanificacion(), "nombre", "nombre");
            return View(pago);
        }
        public ActionResult GuardarPago(string item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Planificacion_Documento pago = new Planificacion_Documento();
            pago = Newtonsoft.Json.JsonConvert.DeserializeObject<Planificacion_Documento>(item);
            pago.fecha = DateTime.Now;
            pago.isaprobado = false;
            pago.ispagado = false;
            pago.usu_register = user.ProviderUserKey.ToString();
            db.Planificacion_Documento.Add(pago);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegistrarCheckin(string lati, string lon, string comentarios, int id, bool correcto)
        {
            var plani = (db.Planificacion.SingleOrDefault(x => x.id == id));
            plani.latitud = lati;
            plani.longitud = lon;
            plani.comentarios = comentarios;
            plani.isRealizada = true;
            plani.isCorrecto = correcto;
            plani.checkin = DateTime.Now;
            db.SaveChanges();
            return Json(true);
        }
        public ActionResult getMedicos(int codigoclinica)
        {
            return Json(new SelectList((from x in db.Institucion_Medico where x.codigoclinica == codigoclinica select new { cmp = x.cmp, apellidos = x.cmp + " - " + x.MEDICOEXTERNO.apellidos + " " + x.MEDICOEXTERNO.nombres }).AsQueryable(), "cmp", "apellidos"));
        }
        public ActionResult validarCantidadPlanificaciones()
        {
            bool re = false;
            DateTime fecha = DateTime.Now;
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();

            var his = (from x in db.Planificacion
                       where x.codigopromotor == cod_usu
                       && x.fecha.Year == fecha.Year
                       && x.fecha.Month == fecha.Month
                       && x.fecha.Day == fecha.Day
                       select x).Count();
            if (his >= (new Variable().CantidadMinimaPlanificacion()))
                re = true;
            return Json(re, JsonRequestBehavior.DenyGet);
        }
        public ActionResult validacionConcurrencia(int clinica, string cmp, DateTime fecha)
        {

            if (clinica != 303)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                string cod_usu = user.ProviderUserKey.ToString();
                int concurrencia = new Variable().DiasConcurrenciaPlanificacion();
                DateTime fec_atras = fecha.AddDays(concurrencia * -1);
                DateTime fec_adelante = fecha.AddDays(concurrencia);
                TimeSpan ts = new TimeSpan(0, 0, 1);
                fec_atras = fec_atras.Date + ts;
                ts = new TimeSpan(23, 59, 59);
                fec_adelante = fec_adelante.Date + ts;
                var citas_realizadas = db.Planificacion.
                    Where(x => x.codigoclinica == clinica
                        && x.cmp == cmp
                        && x.codigopromotor == cod_usu
                        && x.fecha >= fec_atras
                        && x.fecha <= fec_adelante).ToList();
                if (citas_realizadas.Count() > 0)
                    return Json(new { resultado = false, msj = "Tiene visitas registradas para el Médico" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { resultado = true, msj = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Herramientas()
        {
            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = (x.cmpCorregido != null ? x.cmpCorregido : x.cmp), nombre = (x.apellidos.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " " + x.nombres.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")).ToUpper() + " - " + (x.cmpCorregido != null ? x.cmpCorregido : x.cmp) }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Procedencia = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { id = x.codigoclinica, nombre = x.razonsocial.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " - " + x.codigoclinica }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            var lista = new List<Object>();
            for (int i = 2014; i <= DateTime.Now.Year; i++)
            {
                lista.Add(new { codigo = i });
            }
            ViewBag.Year = new SelectList(lista, "codigo", "codigo");
            return View();
        }
        public ActionResult PlanificacionRepresentantes(DateTime fecha)
        {

            var lista = db.Planificacion.Where(x => x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList();
            ViewBag.Fecha = fecha.ToShortDateString();
            return View(lista);
        }

        public ActionResult getVisitasRealizadas(string promotor, DateTime fecha)
        {
            //DateTime fecha = DateTime.Now;
            var data = (from x in db.Planificacion
                        where x.codigopromotor == promotor && x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day
                        select new
                        {
                            a = x.codigopromotor,
                            lat = x.latitud,
                            lon = x.longitud,
                            medico = x.MEDICOEXTERNO.apellidos + "," + x.MEDICOEXTERNO.nombres,
                            clinica = x.CLINICAHOSPITAL.razonsocial,
                            promotor = x.USUARIO.ShortName,
                            checkin = x.checkin.ToString(),
                            correcto = x.isCorrecto ? "Si" : "No",
                            comentarios = x.comentarios
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListaPago(int id)
        {
            ViewBag.Gasto = new SelectList(new Variable().getTipoGastoPlanificacion(), "codigo", "nombre");
            ViewBag.Categoria = new SelectList(new Variable().getCategoriasGastoPlanificacion(), "nombre", "nombre");
            ViewBag.idPlanificacion = id;
            var lista = db.Planificacion_Documento.Where(x => x.idplanificacion == id).ToList();
            return View(lista);
        }
        public ActionResult EditarPlanificacion_Documento(int id)
        {
            var item = db.Planificacion_Documento.Where(x => x.id == id).SingleOrDefault();
            ViewBag.Gasto = new SelectList(new Variable().getTipoGastoPlanificacion(), "codigo", "nombre");
            ViewBag.Categoria = new SelectList(new Variable().getCategoriasGastoPlanificacion(), "nombre", "nombre", item.categoria);
            return View(item);
        }
        public ActionResult ActualizarPlanificacion_Documento(string cadena)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Planificacion_Documento pago = new Planificacion_Documento();
            pago = Newtonsoft.Json.JsonConvert.DeserializeObject<Planificacion_Documento>(cadena);
            var item = db.Planificacion_Documento.SingleOrDefault(x => x.id == pago.id);
            item.tipo = pago.tipo;
            item.ndocumento = pago.ndocumento;
            item.proveedor = pago.proveedor;
            item.categoria = pago.categoria;
            item.persona_destino = pago.persona_destino;
            item.origen = pago.origen;
            item.destino = pago.destino;
            if (item.tipo == 1 || item.tipo == 4)//si son gastos
            {
                item.origen = "";
                item.destino = "";
            }
            else
            {
                item.ndocumento = "";
                item.proveedor = "";
                item.categoria = "";
                item.persona_destino = "";
            }
            item.total = pago.total;
            item.isaprobado = false;
            item.usu_update = user.ProviderUserKey.ToString();
            item.fec_update = DateTime.Now;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CambiarAprobacionGasto(int id)
        {
            var item = db.Planificacion_Documento.Where(x => x.id == id).SingleOrDefault();
            if (item != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                item.isaprobado = !item.isaprobado;
                item.fecha_isaprobado = DateTime.Now;
                item.usu_aprueba = user.ProviderUserKey.ToString();
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.DenyGet);
        }
        public ActionResult EliminarGasto(int id)
        {
            var item = db.Planificacion_Documento.SingleOrDefault(x => x.id == id);
            db.Planificacion_Documento.Remove(item);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.DenyGet);
        }
        public ActionResult ReporteEnviosdeMedico(string cmp)
        {
            try
            {
                DateTime fecha = DateTime.Now.AddMonths(-6);
                List<DetalleReporteComisionesMedico> lista = new List<DetalleReporteComisionesMedico>();
                var lista_select=from ea in db.EXAMENXATENCION
                             join a in db.ATENCION on ea.numeroatencion equals a.numeroatencion
                             join c in db.CITA on a.numerocita equals c.numerocita
                             join ch in db.CLINICAHOSPITAL on c.codigoclinica equals ch.codigoclinica
                             where a.cmp == cmp
                             && !ch.razonsocial.ToLower().Contains("ssalud")
                              && ea.codigoestudio.Substring(0, 1) == "1"
                              && ea.codigoestudio.Substring(5, 1) == "0"
                             && a.fechayhora > fecha
                             && ea.estadoestudio!="X"
                             && ea.estadoestudio != "N"
                             orderby a.fechayhora
                             select new{ 
                                 codigo=ea.codigo,
                             fechayhora=a.fechayhora,
                             modalidad = ea.codigoestudio.Substring(4, 1) == "1" ? "RM" : (ea.codigoestudio.Substring(4, 1) == "2"?"TEM":"OTR"),
                            procedencia=ch.razonsocial
                             };
                foreach (var item in  lista_select)
                {
                    lista.Add(new DetalleReporteComisionesMedico {
                        codigo=item.codigo,
                        fechayhora = item.fechayhora,
                        modalidad = item.modalidad,
                        procedencia = item.procedencia
                    });
                }
                
                return View(lista);
            }
            catch (Exception ex)
            {
                throw new Exception("sssssss"+ex.Message);
            }

        }


        public ActionResult ReporteEnviosdeMedicoAnual()
        {
            return View();
        }
        public ActionResult ReporteEnviosdeMedicoAnualData(string cmp, int ano)
        {
            var lista = (from x in db.DetalleReporteComisionesMedico
                         where x.idmedico == cmp
                         && x.fechayhora.Value.Year == ano
                         orderby x.idReporte
                         select x).ToList();

            var _dataA = new List<Object>();
            for (int i = 1; i < 13; i++)
            {
                _dataA.Add(lista.Where(x => x.fechayhora.Value.Month == i).ToList().Count().ToString());
            };
            var _dataB = new List<Object>();
            for (int i = 1; i < 13; i++)
            {
                _dataB.Add(lista.Where(x => x.fechayhora.Value.Month == i && x.isPagado == true).ToList().Count().ToString());
            }
            var _dataC = new List<Object>();
            foreach (var compania in lista.Select(x => x.procedencia).Distinct())
            {
                var _datacompania = new List<Object>();
                for (int i = 1; i < 13; i++)
                {
                    _datacompania.Add(lista.Where(x => x.fechayhora.Value.Month == i && x.procedencia == compania).ToList().Count().ToString());
                }
                _dataC.Add(new { name = compania + "-" + ano, data = _datacompania });
            }
            var listYear = new List<Object>
            {
                new{
                name = ano.ToString(),
                data = _dataA
                },
                 new{
                name = ano.ToString(),
                data = _dataB
                },
                new{
                name = "compania",
                data = _dataC
                }
                 
            };

            //var resultjson = new JavaScriptSerializer().Serialize(listYear);
            return Json(listYear, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReporteEnviosxProcedencia(int id)
        {
            DateTime fecha = DateTime.Now.AddMonths(-6);
            var lista = (from x in db.DetalleReporteComisionesMedico
                         where x.idprocedencia == id
                          && x.sede.Contains("Reso")
                             && x.ReporteComisionesMedico.fecha > fecha
                         orderby x.idReporte
                         select x).ToList();

            return View(lista);
        }
        public ActionResult ReporteEnviosxProcedenciaRepresentante(int id)
        {
            DateTime fecha = DateTime.Now.AddMonths(-6);
            var lista = (from x in db.DetalleReporteComisionesMedico
                         where x.idprocedencia == id
                             //&& x.sede.Contains("Reso")
                             && x.ReporteComisionesMedico.fecha > fecha
                         orderby x.idReporte
                         select x).ToList();

            return View(lista);
        }
        public ActionResult ReporteEnviosdeMedicoRepresentante(string cmp)
        {
            DateTime fecha = DateTime.Now.AddMonths(-6);
            var lista = (from x in db.DetalleReporteComisionesMedico
                         where x.idmedico == cmp
                             //&& x.sede.Contains("Reso")
                         && x.ReporteComisionesMedico.fecha.Value > fecha
                         orderby x.idReporte
                         select x).ToList();
            return View(lista);
        }


        public ActionResult HerramientasRepresentante()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = (x.cmpCorregido != null ? x.cmpCorregido : x.cmp), nombre = x.apellidos.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " " + x.nombres.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " - " + x.cmp }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");

            ViewBag.Procedencia = new SelectList(db.Institucion_Promotor.Where(x => x.isactivo == true && x.codigopromotor == cod_usu).Select(x => new { id = x.codigoclinica, nombre = x.CLINICAHOSPITAL.razonsocial.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " - " + x.codigoclinica }).OrderBy(x => x.nombre).ToList(), "id", "nombre");

            return View();
        }

        public ActionResult UbicacionesRegistradasPromotores(DateTime fecha)
        {
            var lista = db.Ubicacion.Where(x => x.USUARIO.EMPLEADO.CARGO.codigocargo == 4 && x.fecha.Value.Year == fecha.Year && x.fecha.Value.Month == fecha.Month && x.fecha.Value.Day == fecha.Day).ToList();
            return View(lista);
        }
        public ActionResult isFile(string exam)
        {
            int examen = int.Parse(exam);
            var isAdj = (from ea in db.EXAMENXATENCION
                         join ca in db.CARTAGARANTIA on ea.ATENCION.CITA.codigocartagarantia equals ca.codigocartagarantia into carta_join
                         from ca in carta_join.DefaultIfEmpty()
                         join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                         from esc in esc_join.DefaultIfEmpty()
                         where ea.codigo == examen
                         select new
                         {
                             docEscan = esc.numerodeatencion != null ? true : false,
                             docCarta = ca.codigodocadjunto != null ? true : false,
                         }).SingleOrDefault();

            if (isAdj.docEscan || isAdj.docCarta)
                return Json(new { result = true, doc = isAdj.docEscan, carta = isAdj.docCarta });
            else
                return Json(new { result = false });

        }
        public ActionResult GetAdjuntos(int examen, int tipo)
        {
            string fileName;
            if (tipo == 1)
            {
                var _a = (from ea in db.EXAMENXATENCION
                          join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                          from esc in esc_join.DefaultIfEmpty()
                          where ea.codigo == examen
                          select esc).SingleOrDefault();
                if (_a != null)
                {
                    byte[] archivo = _a.cuerpoarchivo;
                    fileName = _a.nombrearchivo;

                    return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            else
            {
                var _a = (from ea in db.EXAMENXATENCION
                          join ca in db.CARTAGARANTIA on ea.ATENCION.CITA.codigocartagarantia equals ca.codigocartagarantia into carta_join
                          from ca in carta_join.DefaultIfEmpty()
                          join dca in db.DOCESCANEADO on ca.codigodocadjunto equals dca.codigodocadjunto into dcarta_join
                          from dca in dcarta_join.DefaultIfEmpty()
                          where ea.codigo == examen
                          select dca).SingleOrDefault();
                if (_a != null)
                {
                    byte[] archivo = _a.cuerpoarchivo;
                    fileName = _a.nombrearchivo;

                    return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
            }
            return View();

        }
    }
}