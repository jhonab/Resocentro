using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaResocentro.Models;

namespace SistemaResocentro.Controllers.Promotores
{
    public class CompaniaSeguroController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        // GET: CompaniaSeguro
        public ActionResult Index()
        {
            return View();
        }

     
        public ActionResult ListaAsync()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var listaEncuestas = (from x in db.COMPANIASEGURO
                                  //where x.IsEnabled == true
                                  select x).ToList();
            IEnumerable<COMPANIASEGURO> listaEncuestasFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaEncuestasFilter = listaEncuestas
                     .Where(c => c.descripcion.ToLower().ToString().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var isCodigoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var isDescripcionSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            //var istelefonoSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            //var iswebSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            //var isdireccionSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
            //var isgeolocalizadoSortable = Convert.ToBoolean(Request["columns[5][orderable]"]);
            //var isActivoSortable = Convert.ToBoolean(Request["columns[6][orderable]"]);

            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<COMPANIASEGURO, string> orderingFunction = (
                c => sortColumnIndex == 0 && isCodigoSortable ? c.codigocompaniaseguro.ToString() :
                    sortColumnIndex == 1 && isDescripcionSortable ? c.descripcion :
                    //sortColumnIndex == 2 && istelefonoSortable ? c.telefono.ToString() :
                    //sortColumnIndex == 3 && iswebSortable ? c.web.ToString() :
                    //sortColumnIndex == 4 && isdireccionSortable ? c.direccion.ToString() :
                    //sortColumnIndex == 5 && isgeolocalizadoSortable ? (c.latitud != null && c.longitud != null).ToString() :
                    //sortColumnIndex == 6 && isActivoSortable ? c.IsEnabled.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.codigocompaniaseguro.ToString(),
                   c.descripcion,
                   "",
                   //"<a href='javascript:openModal(\"/InstitucionMedica/Details?id="+c.codigoclinica+"\")' title='Detalles de "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-list fa-lg'></i></a>   <a  href='javascript:openModal(\"/InstitucionMedica/Edit?id="+c.codigoclinica+"\")'  title='Editar "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-edit fa-lg'></i></a>   <a  href='javascript:openModalDelete(\""+c.codigoclinica.ToString()+"\","+c.IsEnabled.ToString().ToLower()+",\""+c.tipo+" - "+c.razonsocial+"\")' title='Eliminar "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = listaEncuestas.Count(),
                recordsFiltered = listaEncuestasFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);


        }
        /*
     // GET: CompaniaSeguro/Details/5
     public ActionResult Details(int? id)
     {
         if (id == null)
         {
             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
         }
         COMPANIASEGURO cOMPANIASEGURO = db.COMPANIASEGURO.Find(id);
         if (cOMPANIASEGURO == null)
         {
             return HttpNotFound();
         }
         return View(cOMPANIASEGURO);
     }

     // GET: CompaniaSeguro/Create
     public ActionResult Create()
     {
         return View();
     }

     // POST: CompaniaSeguro/Create
     // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
     // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
     [HttpPost]
     [ValidateAntiForgeryToken]
     public ActionResult Create([Bind(Include = "codigocompaniaseguro,ruc,descripcion,IsAgrupable,IsRequested,IsSCTR,IsAp,IsObsolet,GrupoId")] COMPANIASEGURO cOMPANIASEGURO)
     {
         if (ModelState.IsValid)
         {
             db.COMPANIASEGURO.Add(cOMPANIASEGURO);
             db.SaveChanges();
             return RedirectToAction("Index");
         }

         return View(cOMPANIASEGURO);
     }

     // GET: CompaniaSeguro/Edit/5
     public ActionResult Edit(int? id)
     {
         if (id == null)
         {
             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
         }
         COMPANIASEGURO cOMPANIASEGURO = db.COMPANIASEGURO.Find(id);
         if (cOMPANIASEGURO == null)
         {
             return HttpNotFound();
         }
         return View(cOMPANIASEGURO);
     }

     // POST: CompaniaSeguro/Edit/5
     // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
     // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
     [HttpPost]
     [ValidateAntiForgeryToken]
     public ActionResult Edit([Bind(Include = "codigocompaniaseguro,ruc,descripcion,IsAgrupable,IsRequested,IsSCTR,IsAp,IsObsolet,GrupoId")] COMPANIASEGURO cOMPANIASEGURO)
     {
         if (ModelState.IsValid)
         {
             db.Entry(cOMPANIASEGURO).State = EntityState.Modified;
             db.SaveChanges();
             return RedirectToAction("Index");
         }
         return View(cOMPANIASEGURO);
     }

     // GET: CompaniaSeguro/Delete/5
     public ActionResult Delete(int? id)
     {
         if (id == null)
         {
             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
         }
         COMPANIASEGURO cOMPANIASEGURO = db.COMPANIASEGURO.Find(id);
         if (cOMPANIASEGURO == null)
         {
             return HttpNotFound();
         }
         return View(cOMPANIASEGURO);
     }

     // POST: CompaniaSeguro/Delete/5
     [HttpPost, ActionName("Delete")]
     [ValidateAntiForgeryToken]
     public ActionResult DeleteConfirmed(int id)
     {
         COMPANIASEGURO cOMPANIASEGURO = db.COMPANIASEGURO.Find(id);
         db.COMPANIASEGURO.Remove(cOMPANIASEGURO);
         db.SaveChanges();
         return RedirectToAction("Index");
     }


     */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
