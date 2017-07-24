using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SistemaResocentro.Models;
using System.Web.Security;
using SistemaResocentro.Member;
using System.Collections.Generic;

namespace SistemaResocentro.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string usuario, string clave, string returnUrl)
        {
            if (Membership.ValidateUser(usuario, clave))
            {
                using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
                {
                    var _usuario = db.USUARIO.SingleOrDefault(x => x.siglas == usuario && x.contrasena == clave);
                   

                    FormsAuthentication.SetAuthCookie(_usuario.codigousuario, false);
                }
                return Json(new { login_status = "success" }, JsonRequestBehavior.AllowGet);
            }
            // If we got this far, something failed, redisplay form

            return Json(new { login_status = "invalid" }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]

        public ActionResult LockScreen(string url)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string codigo = user.ProviderUserKey.ToString();
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                var usuario = db.USUARIO.SingleOrDefault(x => x.codigousuario == codigo);
                if (usuario != null)
                {
                    usuario.isSessionLocked = true;
                    db.SaveChanges();
                }
                ViewBag.ReturnUrl = url;
            }
            return View();
        }
        //
        // POST: /Account/LogOff
        [AllowAnonymous]
        public ActionResult imgBuddy(bool exist)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            if (exist)
                return File(u.getUrlpath + user.ProviderUserKey + ".png", "image/png");
            else
            {
                if (user.Comment == "M")
                    return File(u.getUrlpath + "hombre.png", "image/png");
                else
                    return File(u.getUrlpath + "mujer.png", "image/png");
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);

            return RedirectToAction("Login", "Account");

        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string _ruta = u.getUrlpath + user.ProviderUserKey + ".png"; ;
            var r = new List<UploadFilesResult>();
            foreach (string file in Request.Files)
            {

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                hpf.SaveAs(_ruta);
                r.Add(new UploadFilesResult()
                {
                    Name = hpf.FileName,
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }
            // Returns json
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
public class UploadFilesResult
{
    public string Name { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
}