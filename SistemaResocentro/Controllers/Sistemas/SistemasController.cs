using SistemaResocentro.Member;
using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers
{
    [Authorize(Roles = "5")]
    public class SistemasController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Sistemas
        public ActionResult Permisos()
        {
            ViewBag.Usuarios = new SelectList(db.USUARIO.Where(x => x.bloqueado == false).OrderBy(x => x.ShortName).Select(x => new { codigousuario=x.codigousuario,shortname=x.siglas+" - "+x.ShortName}).ToList(), "codigousuario", "ShortName");
            ViewBag.Permisos = new SelectList(db.Tipo_Permiso.Where(x => x.idaplicativo == 7).OrderBy(x => x.idPermiso).Select(x => new { x.idPermiso,permiso=x.idPermiso+" - "+x.permiso}).ToList(), "idPermiso", "permiso");
            return View();
        }

        public ActionResult getPermisos(string idusuario)
        {
            return View(db.Permiso.Where(x => x.codigousuario == idusuario && x.idaplicativo == 7).ToList());
        }
        public ActionResult deletePermiso(int idPermiso)
        {
            try
            {
                db.Permiso.Remove(db.Permiso.Where(x => x.idpermiso == idPermiso).SingleOrDefault());
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult addPermiso(string idusuario, int tipo, string descripcion)
        {
            try
            {
                Permiso p = new Permiso();
                p.codigousuario = idusuario;
                p.aplicativo = "SISTEMARESOCENTRO";
                p.tipo_permiso = tipo;
                p.descripcion = db.Tipo_Permiso.SingleOrDefault(x=> x.idPermiso==tipo&& x.idaplicativo==7).permiso;
                p.idaplicativo = 7;
                db.Permiso.Add(p);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

       

        public ActionResult SolicitudesPendientes(string estado)
        {
            ViewBag.estado = new SelectList(new Variable().getEstadoIncidente(), "codigo", "nombre", estado);
            return View(db.Incidente.Where(x => x.estado ==estado).ToList());
        }
        public ActionResult DetalleSolicitud(int id)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _incidente = db.Incidente.Where(x => x.idIncidente == id).SingleOrDefault();
            if (_incidente.estado == "N")
            {
                _incidente.usu_lectura = user.ProviderUserKey.ToString();
                _incidente.fec_lectura = DateTime.Now;
                _incidente.estado = "L";
            }
            db.SaveChanges();
            return View(_incidente);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetalleSolicitud(Incidente item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _incidente = db.Incidente.Where(x => x.idIncidente == item.idIncidente).SingleOrDefault();
            _incidente.estado = item.estado;
            if (item.estado == "T")
            {
                _incidente.fec_termino = DateTime.Now;
                _incidente.usu_termino = user.ProviderUserKey.ToString();
                string titulo = @"
        <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " +
                                             _incidente.USUARIO1.ShortName
                              + @"</h1>",
                              cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 15px;line-height: 25px;Margin-bottom: 24px'>
Hemos respondido a su Incidente registrado en el sistema.
</p>
", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";
                new Variable().sendCorreo("Reporte Incidente", _incidente.USUARIO1.EMPLEADO.email, "", new Variable().getCuerpoEmail(img,titulo,cuerpo),"");
            }
            _incidente.comentarios = item.comentarios;
            try
            {
                db.SaveChanges();
                return RedirectToAction("SolicitudesPendientes");
            }
            catch (Exception)
            {
                return View(_incidente);
            }

        }

        public ActionResult ListaAsignacionSerie()
        {
            return View(db.Serie_Equipo.ToList());
        }

        public ActionResult EliminarAsignacionSerie(int id)
        {
            db.Serie_Equipo.Remove(db.Serie_Equipo.SingleOrDefault(x => x.idAsignacion == id));
            db.SaveChanges();
            return RedirectToAction("ListaAsignacionSerie");
        }

        public ActionResult RegistrarAsignacionSerie()
        {
            ViewBag.Sucusales = new SelectList(db.SUCURSAL.Where(x => x.codigounidad == 1).ToList(), "codigosucursal", "descripcion");
            ViewBag.Series = new SelectList(db.SERIE.Select(x => new { codigo = x.idserie, value = (x.tipo == "01" ? "Factura" : x.tipo == "03" ? "Boleta" : "Nota Crédito") + " " + x.serie1 + "-" + x.correlativo }).OrderBy(x => x.value).ToList(), "codigo", "value");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarAsignacionSerie(Serie_Equipo item)
        {
            ViewBag.Sucusales = new SelectList(db.SUCURSAL.Where(x => x.codigounidad == 1).ToList(), "codigosucursal", "descripcion");
            ViewBag.Series = new SelectList(db.SERIE.Select(x => new { codigo = x.idserie, value = (x.tipo == "01" ? "Factura" : x.tipo == "03" ? "Boleta" : "Nota Crédito") + " " + x.serie1 + "-" + x.correlativo }).OrderBy(x => x.value).ToList(), "codigo", "value");
            item.NombrePc = item.NombrePc.ToUpper();
            item.Empresa = 1;
            item.isActive = true;
            db.Serie_Equipo.Add(item);
            db.SaveChanges();
            return RedirectToAction("ListaAsignacionSerie");
        }
      
    }
}