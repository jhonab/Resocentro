using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaResocentro.Controllers.Sistemas
{
    public class HerramientasController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Herramientas
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
                    if (_a.isFisico)
                    {
                        return Redirect("http://192.168.0.5:5002/" + _a.codigodocadjunto + "/" + _a.nombrearchivo.Replace(" ", "%20") + "");
                    }
                    else
                    {
                        byte[] archivo = _a.cuerpoarchivo;
                        fileName = _a.nombrearchivo;

                        return File(archivo, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                    }
                }
            }
            return View();

        }
        public ActionResult getCartaAdjutno(string id, string tipo)
        {
            if (tipo == "1")
            {
                var archivo = db.DOCESCANEADO.Where(x => x.codigodocadjunto == id).SingleOrDefault();
                if (archivo != null)
                {
                    if (archivo.isFisico)
                        return Redirect("http://192.168.0.5:5002/" + archivo.codigodocadjunto + "/" + archivo.nombrearchivo.Replace(" ", "%20") + "");
                    else
                        return File(archivo.cuerpoarchivo, "application/pdf", archivo.nombrearchivo);
                }
                else
                    return View();

            }
            else
            {
                return View();
            }
        }

        public ActionResult getProvincia(string dep)
        {
            return Json(new SelectList((from x in db.UBIGEO where x.Departamento == dep select new { codigo = x.Provincia }).Distinct().AsQueryable(), "codigo", "codigo"));
        }
        public ActionResult getDistrito(string dep,string pro)
        {
            return Json(new SelectList((from x in db.UBIGEO where x.Departamento == dep && x.Provincia == pro select new { codigo = x.IneiId, value = x.Distrito }).Distinct().AsQueryable(), "codigo", "value"));
        }
    }
}