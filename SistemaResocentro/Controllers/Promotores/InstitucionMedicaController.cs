using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaResocentro.Models;
using System.Data.Entity.Validation;

namespace SistemaResocentro.Controllers.Promotores
{
    [Authorize]
    public class InstitucionMedicaController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        // GET: InstitucionMedica
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListaAsync()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var listaEncuestas = (from x in db.CLINICAHOSPITAL
                                  //where x.IsEnabled == true
                                  select x).ToList();
            IEnumerable<CLINICAHOSPITAL> listaEncuestasFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaEncuestasFilter = listaEncuestas
                     .Where(c => c.tipo.ToLower().ToString().Contains(param.sSearch.ToLower())
                                 ||
                                 c.razonsocial.ToLower().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var istipoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var israzonSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var istelefonoSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            var iswebSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            var isdireccionSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
            var isgeolocalizadoSortable = Convert.ToBoolean(Request["columns[5][orderable]"]);
            var isActivoSortable = Convert.ToBoolean(Request["columns[6][orderable]"]);

            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<CLINICAHOSPITAL, string> orderingFunction = (
                c => sortColumnIndex == 0 && istipoSortable ? c.tipo.ToString() :
                    sortColumnIndex == 1 && israzonSortable ? c.razonsocial :
                    sortColumnIndex == 2 && istelefonoSortable ? c.telefono.ToString() :
                    sortColumnIndex == 3 && iswebSortable ? c.web.ToString() :
                    sortColumnIndex == 4 && isdireccionSortable ? c.direccion.ToString() :
                    sortColumnIndex == 5 && isgeolocalizadoSortable ? (c.latitud != null && c.longitud != null).ToString() :
                    sortColumnIndex == 6 && isActivoSortable ? c.IsEnabled.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.tipo,
                   c.razonsocial,
                   c.telefono+" "+c.celular,
                   "<a href='"+c.web+"' target='_blank'>"+c.web+"</a>",
                   c.direccion,
                   (c.latitud!=null&&c.longitud!=null)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   (c.IsEnabled==null?false:c.IsEnabled.Value)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   "<a href='javascript:openModal(\"/InstitucionMedica/Details?id="+c.codigoclinica+"\")' title='Detalles de "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-list fa-lg'></i></a>   <a  href='javascript:openModal(\"/InstitucionMedica/Edit?id="+c.codigoclinica+"\")'  title='Editar "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-edit fa-lg'></i></a>   <a  href='javascript:openModalDelete(\""+c.codigoclinica.ToString()+"\","+c.IsEnabled.ToString().ToLower()+",\""+c.tipo+" - "+c.razonsocial+"\")' title='Eliminar "+c.tipo+": "+c.razonsocial+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
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
        // GET: InstitucionMedica/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLINICAHOSPITAL cLINICAHOSPITAL = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == id);
            if (cLINICAHOSPITAL == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoInstitucion = new SelectList(new Variable().getTipoInstitucionMedica(), "nombre", "nombre");
            return View(cLINICAHOSPITAL);
        }

        // GET: InstitucionMedica/Create
        public ActionResult Create()
        {
            ViewBag.TipoInstitucion = new SelectList(new Variable().getTipoInstitucionMedica(), "nombre", "nombre");
            return View();
        }

        // POST: InstitucionMedica/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CLINICAHOSPITAL item)
        {
            try{if (ModelState.IsValid)
            {
                item.codigoclinica = db.CLINICAHOSPITAL.Max(x => x.codigoclinica) + 1;
                item.codigozona = 1;
                item.latitud = "";
                item.longitud = "";
                item.IsEnabled = true;
                db.CLINICAHOSPITAL.Add(item);
               
                    db.SaveChanges();
                return RedirectToAction("Index");
            }}
            catch (DbEntityValidationException e)
{
    foreach (var eve in e.EntityValidationErrors)
    {
        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            eve.Entry.Entity.GetType().Name, eve.Entry.State);
        foreach (var ve in eve.ValidationErrors)
        {
            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                ve.PropertyName, ve.ErrorMessage);
        }
    }
    throw;
}
            ViewBag.TipoInstitucion = new SelectList(new Variable().getTipoInstitucionMedica(), "nombre", "nombre");
            return View(item);
        }

        // GET: InstitucionMedica/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLINICAHOSPITAL cLINICAHOSPITAL = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == id);
            if (cLINICAHOSPITAL == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoInstitucion = new SelectList(new Variable().getTipoInstitucionMedica(), "nombre", "nombre");
            return View(cLINICAHOSPITAL);
        }

        // POST: InstitucionMedica/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CLINICAHOSPITAL item)
        {
            CLINICAHOSPITAL ch = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == item.codigoclinica);
            if (ModelState.IsValid)
            {
                ch.tipo = item.tipo;
                ch.razonsocial = item.razonsocial;
                ch.rucClinica = item.rucClinica;
                ch.CodigoIpress = item.CodigoIpress;
                ch.telefono = item.telefono;
                ch.celular = item.celular;
                ch.email = item.email;
                ch.web = item.web;
                ch.direccion = item.direccion;
                ch.latitud = item.latitud;
                ch.longitud = item.longitud;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: InstitucionMedica/Delete/5
        public ActionResult Delete(int id)
        {
            CLINICAHOSPITAL cli = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == id);
            if (cli != null)
            {
                cli.IsEnabled = !cli.IsEnabled;
                db.SaveChanges();
            }
            return Json(true);
        }

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
