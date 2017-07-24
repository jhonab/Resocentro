using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaResocentro.Controllers
{
    [Authorize]
    public class AseguradoraController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        //CIE10
        public ActionResult SearchCie()
        {
            return View();
        }

        public ActionResult ListaCie(string filtro)
        {
            return View(db.CIE.Where(x => (x.descripcion.Contains(filtro) || x.codigo.Contains(filtro))).AsParallel().ToList());
        }
        //CIE10
        public ActionResult SearchBenefico(int idaseguradora, string idproducto)
        {
            ViewBag.idAseguradora = idaseguradora;
            ViewBag.idProducto = idproducto;
            return View();
        }

        public ActionResult ListaBenefico(string filtro, int idaseguradora,int idproducto)
        {
            if (idaseguradora == 37)
                return View(db.Sunasa_Cobertura.Where(x => (x.Nombre.Contains(filtro) &&x.SitedProductoId==idproducto && x.codigocompaniaseguro==37 && x.IsActive==true)).AsParallel().ToList());
            else
                return View(db.Sunasa_Cobertura.Where(x => (x.Nombre.Contains(filtro) && x.codigocompaniaseguro==0 && x.IsActive==true)).AsParallel().ToList());
        }
    }
}