using SistemaResocentro.Member;
using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SistemaResocentro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Solicitudes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string usuario = user.ProviderUserKey.ToString();
            return View(db.Incidente.Where(x => x.idusuario == usuario).OrderByDescending(x => x.fec_registro).ToList());
        }
        public ActionResult getSolicitudes(int solicitud)
        {
            var item = db.Incidente.SingleOrDefault(x => x.idIncidente == solicitud);
            return View(item);
        }
        public ActionResult RegistrarIncidente(string problema, string importancia, string aplicativo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string host, ip;
            try
            {
                host = (new Variable()).getClientName(Request.UserHostName);
            }
            catch (Exception)
            {
                host = "No Disponible";
            }
            try
            {
                ip = (new Variable()).getClientIP(Request.UserHostName);
            }
            catch (Exception)
            {
                ip = "No Disponible";
            }
            Incidente i = new Incidente();
            i.idusuario = user.ProviderUserKey.ToString();
            i.usuario = user.UserName;
            i.problema = problema;
            i.aplicativo = aplicativo;
            i.importancia = importancia;
            i.anexo = user.telefono;
            i.fec_registro = DateTime.Now;
            i.estado = "N";
            i.nombre_equipo = host;
            i.ip = ip;
            db.Incidente.Add(i);

           
            //sendCorreo("Confirmación de Delivery", "jhon.alvarez@resocentro.com", "jefferson.oviedo@resocentro.com", mensaje);
            string titulo = @"
        <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " +
                                    user.UserName
                     + @"</h1>",
                     cuerpo = @"<p style='Margin-top: 0;color: #565656;font-family: Tahoma,sans-serif;font-size: 15px;line-height: 25px;Margin-bottom: 24px'>
 Su solicitud se registró en nuestro sistema, a la brevedad le responderemos al siguiente correo: <a href='mailto:"+user.Email+@"'>"+user.Email+@"</a>
</p>
", img = "http://extranet.resocentro.com:5050/PaginaWeb/correo/Contactenostop.jpg";
            new Variable().sendCorreo("Reporte Incidente", user.Email, "", new Variable().getCuerpoEmail(img, titulo, cuerpo), "sistemas@resocentro.com");

            try
            {
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult RegistrarUbicacion(string latitud, string longitud)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            Ubicacion ubi = new Ubicacion();
            ubi.codigousuario = user.ProviderUserKey.ToString();
            ubi.latitud = latitud;
            ubi.longitud = longitud;
            ubi.fecha = DateTime.Now;
            db.Ubicacion.Add(ubi);
            try
            {
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ValidarContrasena(string clave)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            return Json(clave.ToUpper() == user.clave.ToUpper());
        }
        public ActionResult ChangeContrasena(string clave)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string pass_user=user.ProviderUserKey.ToString();
            var usu=db.USUARIO.SingleOrDefault(x => x.codigousuario == pass_user);
            if (usu != null)
            {
                usu.contrasena = clave.ToUpper();
                db.SaveChanges();
            }
            return Json(true);
        }
        public ActionResult ChangeShortName(string name)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string pass_user = user.ProviderUserKey.ToString();
            var usu = db.USUARIO.SingleOrDefault(x => x.codigousuario == pass_user);
            if (usu != null)
            {
                usu.ShortName = name;
                db.SaveChanges();
            }
            return Json(true);
        }
    }
}