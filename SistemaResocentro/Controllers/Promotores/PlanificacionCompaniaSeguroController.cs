using SistemaResocentro.Member;
using SistemaResocentro.Models;
using SistemaResocentro.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers.Promotores
{
    [Authorize]
    public class PlanificacionCompaniaSeguroController : Controller
    {

        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: PlanificacionCompaniaSeguro
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
            var lista = db.Planificacion_CompaniaSeguro.Where(x => x.codigopromotor == cod_usu && x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList();
            return View(lista);
        }
        public ActionResult RegistrarVisita()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            string sql = "select cp.codigocompania,c.descripcion from companiaseguro_promotor cp inner join COMPANIASEGURO c on cp.codigocompania=c.codigocompaniaseguro where cp.codigopromotor='" + cod_usu + "'";
            var lista = new List<Object>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        lista.Add(new
                        {
                            id = reader["codigocompania"].ToString(),
                            nombre = reader["descripcion"].ToString()
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            ViewBag.Institucion = new SelectList(lista, "id", "nombre");
            ViewBag.Medico = new SelectList(new Variable().getVacio(), "id", "nombre");
            ViewBag.Motivos = new SelectList(new Variable().getMotivosPlanificacion(), "nombre", "nombre");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarVisita(Planificacion_CompaniaSeguro item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();
            var _compa = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == item.codigocompaniaseguro);
            item.ruc = _compa.ruc;
            item.codigopromotor = cod_usu;
            item.isRealizada = false;
            item.isCorrecto = false;
            db.Planificacion_CompaniaSeguro.Add(item);
            db.SaveChanges();
            return RedirectToAction("VisitasPlanificadas", new { tab = 2 });
        }
        public ActionResult ConfirmarCheckin(int id)
        {
            return View(db.Planificacion_CompaniaSeguro.SingleOrDefault(x => x.id == id));
        }
        public ActionResult RegistrarPago(int id)
        {
            Pagos_Planificacion_CompaniaseguroViewModel pago = new Pagos_Planificacion_CompaniaseguroViewModel();
            pago.lista = db.Planificacion_CompaniaSeguro_Documento.Where(x => x.idplanificacion == id).ToList();
            pago.items = new Planificacion_CompaniaSeguro_Documento();
            pago.items.idplanificacion = id;
            ViewBag.Gasto = new SelectList(new Variable().getTipoGastoPlanificacion(), "codigo", "nombre");
            ViewBag.Categoria = new SelectList(new Variable().getCategoriasGastoPlanificacion(), "nombre", "nombre");
            return View(pago);
        }
        public ActionResult GuardarPago(string item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Planificacion_CompaniaSeguro_Documento pago = new Planificacion_CompaniaSeguro_Documento();
            pago = Newtonsoft.Json.JsonConvert.DeserializeObject<Planificacion_CompaniaSeguro_Documento>(item);
            pago.fecha = DateTime.Now;
            pago.isaprobado = false;
            pago.ispagado = false;
            pago.usu_register = user.ProviderUserKey.ToString();
            db.Planificacion_CompaniaSeguro_Documento.Add(pago);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegistrarCheckin(string lati, string lon, string comentarios, int id)
        {
            var plani = (db.Planificacion_CompaniaSeguro.SingleOrDefault(x => x.id == id));
            plani.latitud = lati;
            plani.longitud = lon;
            plani.comentarios = comentarios;
            plani.isRealizada = true;
            plani.checkin = DateTime.Now;
            db.SaveChanges();
            return Json(true);
        }
        public ActionResult getMedicos(int codigoclinica)
        {
            return Json(new SelectList((from x in db.CompaniaSeguro_Colaborador where x.codigocompania == codigoclinica select new { cmp = x.idColaborador, apellidos = x.Colaborador.apellidos + " " + x.Colaborador.nombres }).AsQueryable(), "cmp", "apellidos"));
        }
        public ActionResult validarCantidadPlanificaciones()
        {
            bool re = false;
            DateTime fecha = DateTime.Now;
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string cod_usu = user.ProviderUserKey.ToString();

            var his = (from x in db.Planificacion_CompaniaSeguro
                       where x.codigopromotor == cod_usu
                       && x.fecha.Year == fecha.Year
                       && x.fecha.Month == fecha.Month
                       && x.fecha.Day == fecha.Day
                       select x).Count();
            if (his >= (new Variable().CantidadMinimaPlanificacion()))
                re = true;
            return Json(re, JsonRequestBehavior.DenyGet);
        }
        public ActionResult validacionConcurrencia(int clinica, int cmp, DateTime fecha)
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
            var citas_realizadas = db.Planificacion_CompaniaSeguro.
                Where(x => x.codigocompaniaseguro == clinica
                    && x.idColaborador == cmp
                    && x.codigopromotor == cod_usu
                    && x.fecha >= fec_atras
                    && x.fecha <= fec_adelante).ToList();
            if (citas_realizadas.Count() > 0)
                return Json(new { resultado = false, msj = "Tiene visitas registradas para el Médico" }, JsonRequestBehavior.AllowGet);

            return Json(new { resultado = true, msj = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Herramientas()
        {
            ViewBag.Medicos = new SelectList(db.Colaborador.Where(x => x.activo == true).Select(x => new { cmp = x.cmp, nombre = x.apellidos.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " " + x.nombres.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " - " + x.cmp }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Procedencia = new SelectList(db.COMPANIASEGURO.Select(x => new { id = x.codigocompaniaseguro, nombre = x.descripcion.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u") + " - " + x.codigocompaniaseguro }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            return View();
        }
        public ActionResult PlanificacionRepresentantes(DateTime fecha)
        {

            var lista = db.Planificacion_CompaniaSeguro.Where(x => x.fecha.Year == fecha.Year && x.fecha.Month == fecha.Month && x.fecha.Day == fecha.Day).ToList();
            return View(lista);
        }
        public ActionResult EliminarGasto(int id)
        {
            var item = db.Planificacion_CompaniaSeguro_Documento.SingleOrDefault(x => x.id == id);
            db.Planificacion_CompaniaSeguro_Documento.Remove(item);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.DenyGet);
        }



        public ActionResult EditarPlanificacion_Documento(int id)
        {
            var item = db.Planificacion_CompaniaSeguro_Documento.Where(x => x.id == id).SingleOrDefault();
            ViewBag.Gasto = new SelectList(new Variable().getTipoGastoPlanificacion(), "codigo", "nombre");
            ViewBag.Categoria = new SelectList(new Variable().getCategoriasGastoPlanificacion(), "nombre", "nombre", item.categoria);
            return View(item);
        }
        public ActionResult ActualizarPlanificacion_Documento(string cadena)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Planificacion_CompaniaSeguro_Documento pago = new Planificacion_CompaniaSeguro_Documento();
            pago = Newtonsoft.Json.JsonConvert.DeserializeObject<Planificacion_CompaniaSeguro_Documento>(cadena);
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
            var item = db.Planificacion_CompaniaSeguro_Documento.Where(x => x.id == id).SingleOrDefault();
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



    }
}