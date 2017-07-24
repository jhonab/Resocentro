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
    [Authorize]
    public class AsignacionesController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();


        #region Institución - Promotor - Médico
        //// GET: Asignaciones
        //public ActionResult ListaInstitucionPromotorMedico()
        //{
        //    return View();
        //}

        //public ActionResult InstitucionPromotorMedico()
        //{
        //    JQueryDataTableParamModel param = new JQueryDataTableParamModel();
        //    var lista = (from x in db.Insti_Prom_Medico select x).ToList();
        //    IEnumerable<Insti_Prom_Medico> listaFilter;
        //    param.sSearch = (Request["search[value]"]);
        //    param.iDisplayStart = Convert.ToInt32(Request["start"]);
        //    param.iDisplayLength = Convert.ToInt32(Request["length"]);
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        listaFilter = lista
        //             .Where(c => c.nom_clinica.ToString().Contains(param.sSearch.ToLower())
        //                         ||
        //                         c.nom_promotor.ToString().Contains(param.sSearch.ToLower())
        //                         ||
        //                         c.nom_medico.ToString().Contains(param.sSearch.ToLower())
        //                         ||
        //                          c.cmp_medico.ToString().Contains(param.sSearch.ToLower()));
        //    }
        //    else
        //    {
        //        listaFilter = lista;
        //    }

        //    var isCodclinicaSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
        //    var isRazonSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
        //    var isCodPromotorSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
        //    var isNomPromotorSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
        //    var isCmpSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
        //    var isNomMedicoSortable = Convert.ToBoolean(Request["columns[5][orderable]"]);

        //    var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
        //    Func<Insti_Prom_Medico, string> orderingFunction = (
        //        c => sortColumnIndex == 0 && isCodclinicaSortable ? c.cod_clinica.ToString() :
        //            sortColumnIndex == 1 && isRazonSortable ? c.nom_clinica.ToString() :
        //            sortColumnIndex == 2 && isCodPromotorSortable ? c.cod_promotor.ToString() :
        //            sortColumnIndex == 3 && isNomPromotorSortable ? c.nom_promotor.ToString() :
        //            sortColumnIndex == 4 && isCmpSortable ? c.cmp_medico.ToString() :
        //            sortColumnIndex == 5 && isCmpSortable ? c.nom_medico.ToString() :
        //            sortColumnIndex == 6 && isCmpSortable ? c.estado.ToString() :
        //            "");

        //    var sortDirection = Request["order[0][dir]"]; // asc or desc
        //    if (sortDirection == "asc")
        //        listaFilter = listaFilter.OrderBy(orderingFunction);
        //    else
        //        listaFilter = listaFilter.OrderByDescending(orderingFunction);

        //    var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = (from c in displayedCompanies
        //                  select new[]{ 
        //          c.cod_clinica,
        //           c.nom_clinica,
        //           c.cod_promotor,
        //          c.nom_promotor,
        //           c.cmp_medico,
        //           c.nom_medico,
        //           (c.estado)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
        //           "<a  href='javascript:openModalDelete(\""+c.id_asig.ToString()+"\","+c.estado.ToString().ToLower()+",\""+c.cod_clinica+" - "+c.nom_clinica+"\",\"("+c.cod_promotor+") "+c.nom_promotor+" "+c.cmp_medico.Trim()+"\")'  title='Eliminar "+c.nom_medico+": "+c+"'><i class='fa fa-trash-o fa-lg'></i></a>",

        //       });

        //    return Json(new
        //    {
        //        draw = Request["draw"],
        //        recordsTotal = lista.Count(),
        //        recordsFiltered = listaFilter.Count(),
        //        data = result
        //    },
        //                JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult validarAsignacionInstitucionPromotorMedico(string idInstitucion, string cmp, string codigopromotor)
        //{
        //    var result = db.Insti_Prom_Medico.Where(x => x.cod_clinica == idInstitucion && x.cmp_medico == cmp && x.cod_promotor == codigopromotor).ToList().Count;
        //    return Json(result == 0, JsonRequestBehavior.AllowGet);
        //}

        //// GET: Asignaciones/Create
        //public ActionResult CreateInstitucionPromotorMedico()
        //{
        //    ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmpCorregido = x.cmpCorregido, nombre = x.cmpCorregido + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmpCorregido", "nombre");

        //    ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");

        //    ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateInstitucionPromotorMedico(Insti_Prom_Medico Insti_Prom_Medico)
        //{

        //    if (ModelState.IsValid)
        //    {

        //        // Insti_Prom_Medico.codigozona = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica.ToString() == Insti_Prom_Medico.cod_clinica &&  ).codigozona;
        //        //db.Insti_Prom_Medico.Add(Insti_Prom_Medico);
        //        //db.SaveChanges();
        //        //return RedirectToAction("ListaInstitucionPromotor");

        //        //Insti_Prom_Medico. = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == institucion_Promotor.codigoclinica).codigozona;
        //        //db.Insti_Prom_Medico.Add(Insti_Prom_Medico);
        //        //db.SaveChanges();
        //        //return RedirectToAction("ListaInstitucionPromotorMedico");
        //        //Insti_Prom_Medico.cod_clinica = db.Insti_Prom_Medico.SingleOrDefault(x => x.cod_clinica.ToString() == Insti_Prom_Medico.cod_clinica && x.cod_promotor == Insti_Prom_Medico.cod_promotor && x.cmp_medico == Insti_Prom_Medico.cmp_medico).cod_clinica;

        //        //Insti_Prom_Medico item = new Insti_Prom_Medico();
        //        //item.cod_clinica = Insti_Prom_Medico.cod_clinica;
        //        //item.nom_clinica = Insti_Prom_Medico.nom_clinica;
        //        //item.cod_promotor = Insti_Prom_Medico.cod_promotor;
        //        //item.nom_promotor = Insti_Prom_Medico.nom_promotor;
        //        //item.cmp_medico = Insti_Prom_Medico.cmp_medico;
        //        //item.nom_medico = Insti_Prom_Medico.nom_medico;
        //        //item.estado = true;

        //        //db.Insti_Prom_Medico.Add(item);
        //        //db.SaveChanges();
        //        //return RedirectToAction("ListaInstitucionPromotorMedico");
        //    }

        //    ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmpCorregido = x.cmpCorregido, nombre = x.cmpCorregido + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmpCorregido", "nombre");

        //    ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");
        //    ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
        //    return View(Insti_Prom_Medico);
        //}

        //public ActionResult Delete(int id)
        //{
        //    Institucion_Medico institucion_Medico = db.Institucion_Medico.Find(id);
        //    if (institucion_Medico != null)
        //    {
        //        institucion_Medico.isactivo = !institucion_Medico.isactivo;
        //        db.SaveChanges();
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Entrega de Dinero al Medico AFINIDAD
        // GET: Asignaciones
        public ActionResult ListaAsignacionAfinidad()
        {
            return View();
        }


        public ActionResult ListaAsyncAfinidaMedico()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var lista = (from x in db.AsignacionRepresentanteMedico select x).ToList();
            IEnumerable<AsignacionRepresentanteMedico> listaFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaFilter = lista
                     .Where(c => c.MEDICOEXTERNO.apellidos.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 (c.MEDICOEXTERNO.cmpCorregido == null ? c.MEDICOEXTERNO.cmp : c.MEDICOEXTERNO.cmpCorregido).ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.USUARIO.ShortName.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaFilter = lista;
            }

            var medicoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var cmpSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var promotorSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);



            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<AsignacionRepresentanteMedico, string> orderingFunction = (
                c => sortColumnIndex == 0 && medicoSortable ? c.MEDICOEXTERNO.apellidos.ToString() :
                    sortColumnIndex == 1 && cmpSortable ? c.idMedico :
                    sortColumnIndex == 2 && promotorSortable ? c.USUARIO.ShortName :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaFilter = listaFilter.OrderBy(orderingFunction);
            else
                listaFilter = listaFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.nombres,
                  c.MEDICOEXTERNO.cmp,
                   c.USUARIO.ShortName,     
                    ( c.MEDICOEXTERNO.isPaga?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' /> " )+c.MEDICOEXTERNO.DocumentoPago,
                   "<a href='javascript:openModal(\"/Medico/Details?id="+c.idMedico.Trim()+"\")' title='Detalles de  Médico: "+c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.nombres+"'><i class='fa fa-list fa-lg'></i></a>  "+
                   "<a href='javascript:openModalDelete("+c.idAsignacion+",\""+c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.nombres+"\")' title='Eliminar Afinidad: "+c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.nombres+"'><i class='fa fa-trash fa-lg'></i></a>",
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = lista.Count(),
                recordsFiltered = listaFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarAfinidad(int id)
        {
            var afinidad = db.AsignacionRepresentanteMedico.SingleOrDefault(x => x.idAsignacion == id);
            if (afinidad != null)
            {
                db.AsignacionRepresentanteMedico.Remove(afinidad);
                db.SaveChanges();
                return Json(true);
            }
            return Json(false);
        }


        // GET: Asignaciones/Create
        public ActionResult CreateAfinidadMedico()
        {
            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = x.cmp, nombre = x.cmp + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAfinidadMedico(AsignacionRepresentanteMedico item)
        {
            if (ModelState.IsValid)
            {
                item.isValid = true;
                item.FecRegistro = DateTime.Now;
                db.AsignacionRepresentanteMedico.Add(item);
                db.SaveChanges();
                return RedirectToAction("ListaAsignacionAfinidad");
            }

            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = x.cmp, nombre = x.cmp + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            return View(item);
        }
        public ActionResult validarAfinidadMedico(string cmp)
        {
            var result = db.AsignacionRepresentanteMedico.Where(x => x.idMedico == cmp).ToList().Count;
            return Json(result == 0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Médico Institución
        // GET: Asignaciones
        public ActionResult ListaInstitucionMedico()
        {
            return View();
        }
        public ActionResult ListaAsyncInstitucionMedico()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var lista = (from x in db.Institucion_Medico select x).ToList();
            IEnumerable<Institucion_Medico> listaFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaFilter = lista
                     .Where(c => c.CLINICAHOSPITAL.tipo.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.CLINICAHOSPITAL.razonsocial.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.MEDICOEXTERNO.ESPECIALIDAD.descripcion.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.MEDICOEXTERNO.apellidos.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.MEDICOEXTERNO.nombres.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.MEDICOEXTERNO.cmp.ToLower().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaFilter = lista;
            }

            var istipoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var israzonSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var isMedicoSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            var iscmpSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            var isespecialidadSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);


            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<Institucion_Medico, string> orderingFunction = (
                c => sortColumnIndex == 0 && istipoSortable ? c.CLINICAHOSPITAL.tipo.ToString() :
                    sortColumnIndex == 1 && israzonSortable ? c.CLINICAHOSPITAL.razonsocial :
                    sortColumnIndex == 2 && isMedicoSortable ? c.MEDICOEXTERNO.apellidos.ToString() :
                    sortColumnIndex == 3 && iscmpSortable ? c.MEDICOEXTERNO.cmp.ToString() :
                    sortColumnIndex == 4 && isespecialidadSortable ? c.isactivo.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaFilter = listaFilter.OrderBy(orderingFunction);
            else
                listaFilter = listaFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.CLINICAHOSPITAL.tipo,
                   c.CLINICAHOSPITAL.razonsocial,
                   c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.nombres,
                  c.MEDICOEXTERNO.cmp,
                   c.MEDICOEXTERNO.ESPECIALIDAD.descripcion,
                   (c.isactivo)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   "<a  href='javascript:openModalDelete(\""+c.id.ToString()+"\","+c.isactivo.ToString().ToLower()+",\""+c.CLINICAHOSPITAL.tipo+" - "+c.CLINICAHOSPITAL.razonsocial+"\",\"("+c.MEDICOEXTERNO.cmp.Trim()+") "+c.MEDICOEXTERNO.apellidos+" "+c.MEDICOEXTERNO.apellidos+"\")'  title='Eliminar "+c.CLINICAHOSPITAL.razonsocial+": "+c.MEDICOEXTERNO.apellidos+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = lista.Count(),
                recordsFiltered = listaFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateInstitucionMedico()
        {
            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = x.cmp, nombre = x.cmp + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInstitucionMedico(Institucion_Medico institucion_Medico)
        {
            if (ModelState.IsValid)
            {
                institucion_Medico.isactivo = true;
                institucion_Medico.codigozona = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == institucion_Medico.codigoclinica).codigozona;
                db.Institucion_Medico.Add(institucion_Medico);
                db.SaveChanges();
                return RedirectToAction("ListaInstitucionMedico");
            }

            ViewBag.Medicos = new SelectList(db.MEDICOEXTERNO.Where(x => x.isactivo == true).Select(x => new { cmp = x.cmp, nombre = x.cmp + " - " + x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");
            return View(institucion_Medico);
        }
        public ActionResult validarAsignacionInstitucionMedico(int idInstitucion, string cmp)
        {
            var result = db.Institucion_Medico.Where(x => x.codigoclinica == idInstitucion && x.cmp == cmp && x.isactivo == false).ToList().Count;
            return Json(result == 0, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Institución Promotor
        // GET: Asignaciones
        public ActionResult ListaInstitucionPromotor()
        {
            return View();
        }

        public ActionResult ListaAsyncListaInstitucionPromotor()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var lista = (from x in db.Institucion_Promotor select x).ToList();
            IEnumerable<Institucion_Promotor> listaFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaFilter = lista
                     .Where(c => c.CLINICAHOSPITAL.tipo.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.CLINICAHOSPITAL.razonsocial.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.USUARIO.ShortName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaFilter = lista;
            }

            var istipoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var israzonSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var isPromotorSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            var isespecialidadSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);


            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<Institucion_Promotor, string> orderingFunction = (
                c => sortColumnIndex == 0 && istipoSortable ? c.CLINICAHOSPITAL.tipo.ToString() :
                    sortColumnIndex == 1 && israzonSortable ? c.CLINICAHOSPITAL.razonsocial :
                    sortColumnIndex == 2 && isPromotorSortable ? c.USUARIO.ShortName.ToString() :
                    sortColumnIndex == 3 && isespecialidadSortable ? c.isactivo.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaFilter = listaFilter.OrderBy(orderingFunction);
            else
                listaFilter = listaFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.CLINICAHOSPITAL.tipo,
                   c.CLINICAHOSPITAL.razonsocial,
                   c.USUARIO.ShortName,
                   (c.isactivo)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   "<a  href='javascript:openModalDelete(\""+c.id.ToString()+"\","+c.isactivo.ToString().ToLower()+",\""+c.CLINICAHOSPITAL.tipo+" - "+c.CLINICAHOSPITAL.razonsocial+"\",\""+c.USUARIO.ShortName+"\")'  title='Eliminar "+c.CLINICAHOSPITAL.razonsocial+": "+c.USUARIO.ShortName+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = lista.Count(),
                recordsFiltered = listaFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        // GET: Asignaciones/Create
        public ActionResult CreateInstitucionPromotor()
        {
            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInstitucionPromotor(Institucion_Promotor institucion_Promotor)
        {
            if (ModelState.IsValid)
            {
                institucion_Promotor.codigozona = db.CLINICAHOSPITAL.SingleOrDefault(x => x.codigoclinica == institucion_Promotor.codigoclinica).codigozona;
                institucion_Promotor.isactivo = true;
                db.Institucion_Promotor.Add(institucion_Promotor);
                db.SaveChanges();
                return RedirectToAction("ListaInstitucionPromotor");
            }

            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            ViewBag.Instituciones = new SelectList(db.CLINICAHOSPITAL.Where(x => x.IsEnabled == true).Select(x => new { codigoclinica = x.codigoclinica, razonsocial = x.tipo + " - " + x.razonsocial, tipo = x.tipo }).OrderBy(x => x.tipo).ToList(), "codigoclinica", "razonsocial");
            return View(institucion_Promotor);
        }
        public ActionResult validarAsignacionInstitucionPromotor(int idInstitucion, string promotor)
        {
            int result = 0;
            /*if (promotor == "JAA44")//Jesus puede ver instituciones ya asignadas a otros representantes
                result = db.Institucion_Promotor.Where(x => x.codigoclinica == idInstitucion && x.codigopromotor == promotor).ToList().Count;
            else*/
                result = db.Institucion_Promotor.Where(x => x.codigoclinica == idInstitucion && x.isactivo==true).ToList().Count;
            return Json(result == 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteInstitucionPromotor(int id)
        {
            var institucion_promotor = db.Institucion_Promotor.Find(id);
            if (institucion_promotor != null)
            {
                institucion_promotor.isactivo = !institucion_promotor.isactivo;
                if (institucion_promotor.isactivo)
                    foreach (var item in db.Institucion_Promotor.Where(x => x.codigoclinica == institucion_promotor.codigoclinica && x.id != institucion_promotor.id).ToList())
                    {
                        item.isactivo = false;
                    }
                //institucion_promotor.isactivo = !institucion_promotor.isactivo;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Colaborador Compañia
        // GET: Asignaciones
        public ActionResult ListaColaboradoCompania()
        {
            return View();
        }

        public ActionResult ListaAsyncColaboradoCompania()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var lista = (from x in db.CompaniaSeguro_Colaborador select x).ToList();
            IEnumerable<CompaniaSeguro_Colaborador> listaFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaFilter = lista
                     .Where(c => c.COMPANIASEGURO.descripcion.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.Colaborador.apellidos.ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.Colaborador.nombres.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaFilter = lista;
            }

            var isCompaniaSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var isColaboradorSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            //var isMedicoSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            //var iscmpSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            //var isespecialidadSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);


            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<CompaniaSeguro_Colaborador, string> orderingFunction = (
                c => sortColumnIndex == 0 && isCompaniaSortable ? c.COMPANIASEGURO.descripcion.ToString() :
                    sortColumnIndex == 1 && isColaboradorSortable ? c.Colaborador.apellidos :
                    //sortColumnIndex == 2 && isMedicoSortable ? c.MEDICOEXTERNO.apellidos.ToString() :
                    //sortColumnIndex == 3 && iscmpSortable ? c.MEDICOEXTERNO.cmp.ToString() :
                    //sortColumnIndex == 4 && isespecialidadSortable ? c.isactivo.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaFilter = listaFilter.OrderBy(orderingFunction);
            else
                listaFilter = listaFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.COMPANIASEGURO.descripcion,
                   //c.COMPANIASEGURO.razonsocial,
                   c.Colaborador.apellidos+" "+c.Colaborador.nombres,
                  //c.MEDICOEXTERNO.cmp,
                   //c.MEDICOEXTERNO.ESPECIALIDAD.descripcion,
                   (c.isactivo)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   "<a  href='javascript:openModalDelete(\""+c.id.ToString()+"\","+c.isactivo.ToString().ToLower()+",\""+c.COMPANIASEGURO.descripcion+"\",\"("+c.Colaborador.idColaborador.ToString().Trim()+") "+c.Colaborador.apellidos+" "+c.Colaborador.nombres+"\")'  title='Eliminar "+c.COMPANIASEGURO.descripcion+": "+c.Colaborador.apellidos+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = lista.Count(),
                recordsFiltered = listaFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        // GET: Asignaciones/Create
        public ActionResult CreateColaboradorCompania()
        {
            ViewBag.Medicos = new SelectList(db.Colaborador.Where(x => x.activo == true).Select(x => new { cmp = x.idColaborador, nombre = x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Instituciones = new SelectList(db.COMPANIASEGURO.Select(x => new { codigoclinica = x.codigocompaniaseguro, razonsocial = x.descripcion }).OrderBy(x => x.razonsocial).ToList(), "codigoclinica", "razonsocial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateColaboradorCompania(CompaniaSeguro_Colaborador colaborador_compania)
        {
            if (ModelState.IsValid)
            {
                colaborador_compania.ruc = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == colaborador_compania.codigocompania).ruc;
                db.CompaniaSeguro_Colaborador.Add(colaborador_compania);
                db.SaveChanges();
                return RedirectToAction("ListaColaboradoCompania");
            }

            ViewBag.Medicos = new SelectList(db.Colaborador.Where(x => x.activo == true).Select(x => new { cmp = x.idColaborador, nombre = x.apellidos + " " + x.nombres }).OrderBy(x => x.nombre).ToList(), "cmp", "nombre");
            ViewBag.Instituciones = new SelectList(db.COMPANIASEGURO.Select(x => new { codigoclinica = x.codigocompaniaseguro, razonsocial = x.descripcion }).OrderBy(x => x.razonsocial).ToList(), "codigoclinica", "razonsocial");
            return View(colaborador_compania);
        }
        public ActionResult validarAsignacionColaboradorCompania(int idInstitucion, int cmp)
        {
            var result = db.CompaniaSeguro_Colaborador.Where(x => x.codigocompania == idInstitucion && x.idColaborador == cmp).ToList().Count;
            return Json(result == 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteColaboradorCompania(int id)
        {
            CompaniaSeguro_Colaborador institucion_Medico = db.CompaniaSeguro_Colaborador.Find(id);
            if (institucion_Medico != null)
            {
                institucion_Medico.isactivo = !institucion_Medico.isactivo;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Compania Promotor
        // GET: Asignaciones
        public ActionResult ListaCompaniaPromotor()
        {
            return View();
        }

        public ActionResult ListaAsyncListaCompaniaPromotor()
        {
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var lista = (from x in db.CompaniaSeguro_Promotor select x).ToList();
            IEnumerable<CompaniaSeguro_Promotor> listaFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaFilter = lista
                     .Where(c => c.COMPANIASEGURO.descripcion.ToLower().ToString().ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.USUARIO.ShortName.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                listaFilter = lista;
            }

            var isCompaniaSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var isPromotorSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            //var isPromotorSortable = Convert.ToBoolean(Request["columns[2][orderable]"]);
            //var isespecialidadSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);


            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<CompaniaSeguro_Promotor, string> orderingFunction = (
                c => sortColumnIndex == 0 && isCompaniaSortable ? c.COMPANIASEGURO.descripcion.ToString() :
                    sortColumnIndex == 1 && isPromotorSortable ? c.USUARIO.ShortName :
                    //sortColumnIndex == 2 && isPromotorSortable ? c.USUARIO.ShortName.ToString() :
                    //sortColumnIndex == 3 && isespecialidadSortable ? c.isactivo.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaFilter = listaFilter.OrderBy(orderingFunction);
            else
                listaFilter = listaFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.COMPANIASEGURO.descripcion,
                   //c.CLINICAHOSPITAL.razonsocial,
                   c.USUARIO.ShortName,
                   (c.isactivo)?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   "<a  href='javascript:openModalDelete(\""+c.id.ToString()+"\","+c.isactivo.ToString().ToLower()+",\""+c.COMPANIASEGURO.descripcion+"\",\""+c.USUARIO.ShortName+"\")'  title='Eliminar "+c.COMPANIASEGURO.descripcion+": "+c.USUARIO.ShortName+"'><i class='fa fa-trash-o fa-lg'></i></a>",
                   
               });

            return Json(new
            {
                draw = Request["draw"],
                recordsTotal = lista.Count(),
                recordsFiltered = listaFilter.Count(),
                data = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        // GET: Asignaciones/Create
        public ActionResult CreateCompaniaPromotor()
        {
            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            ViewBag.Instituciones = new SelectList(db.COMPANIASEGURO.Select(x => new { codigoclinica = x.codigocompaniaseguro, razonsocial = x.descripcion }).OrderBy(x => x.razonsocial).ToList(), "codigoclinica", "razonsocial");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCompaniaPromotor(CompaniaSeguro_Promotor institucion_Promotor)
        {
            if (ModelState.IsValid)
            {
                institucion_Promotor.ruc = db.COMPANIASEGURO.SingleOrDefault(x => x.codigocompaniaseguro == institucion_Promotor.codigocompania).ruc;
                db.CompaniaSeguro_Promotor.Add(institucion_Promotor);
                db.SaveChanges();
                return RedirectToAction("ListaCompaniaPromotor");
            }

            ViewBag.Promotor = new SelectList(db.USUARIO.Where(x => x.EMPLEADO.codigocargo == 4 && x.bloqueado == false).Select(x => new { id = x.codigousuario, nombre = x.ShortName }).OrderBy(x => x.nombre).ToList(), "id", "nombre");
            ViewBag.Instituciones = new SelectList(db.COMPANIASEGURO.Select(x => new { codigoclinica = x.codigocompaniaseguro, razonsocial = x.descripcion }).OrderBy(x => x.razonsocial).ToList(), "codigoclinica", "razonsocial");
            return View(institucion_Promotor);
        }
        public ActionResult validarAsignacionCompaniaPromotor(int idInstitucion, string promotor)
        {
            var result = db.CompaniaSeguro_Promotor.Where(x => x.codigocompania == idInstitucion).ToList().Count;
            return Json(result == 0, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteCompaniaPromotor(int id)
        {
            var institucion_promotor = db.CompaniaSeguro_Promotor.Find(id);
            if (institucion_promotor != null)
            {
                institucion_promotor.isactivo = !institucion_promotor.isactivo;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion
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