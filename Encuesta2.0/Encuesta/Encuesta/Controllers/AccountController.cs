using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Encuesta.Models;

using Encuesta.Member;
using Encuesta.Util;

namespace Encuesta.Controllers
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
                    //MembershipUser use = Membership.GetUser(_usuario.codigousuario);
                    FormsAuthentication.SetAuthCookie(_usuario.codigousuario, false);
                }

                return Json(new { login_status = "success" }, JsonRequestBehavior.AllowGet);

              //  return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            //ModelState.AddModelError("", "El Usuario o contraseña es incorrecto.");
            return Json(new { login_status = "invalid" }, JsonRequestBehavior.AllowGet);
            //return View(user);
        }

        public ActionResult LoginiMedical(string siglas)
        {
            DATABASEGENERALEntities db = new DATABASEGENERALEntities();
            var _usu = db.USUARIO.SingleOrDefault(x => x.siglas == siglas);

            if (Membership.ValidateUser(_usu.siglas, _usu.codigousuario))
            {
                MembershipUser use = Membership.GetUser(_usu.codigousuario);

                FormsAuthentication.SetAuthCookie(_usu.codigousuario, false);

                return RedirectToLocal("Home/Index");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "El Usuario o contraseña es incorrecto.");
            return View();
        }

        //
        // POST: /Account/LogOff
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
        public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            string _ruta = u.getUrlpathSERVER + user.ProviderUserKey + ".png"; ;
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
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            return RedirectToAction("Login", "Account");

        }

        [HttpPost]
        public ActionResult UploadFiles_OLD(IEnumerable<HttpPostedFileBase> files)
        {
            Variable u = new Variable();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            foreach (HttpPostedFileBase file in files)
            {
                string filePath = u.getUrlpath + user.ProviderUserKey + ".png";
                file.SaveAs(filePath);
            }

            return Json("All files have been successfully stored.");
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