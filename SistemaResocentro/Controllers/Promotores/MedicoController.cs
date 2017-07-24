using SistemaResocentro.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaResocentro.Controllers
{
    [Authorize]
    public class MedicoController : Controller
    {
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        // GET: Paciente
        public ActionResult CreateMedico()
        {
            //ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre");
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            return View(new MEDICOEXTERNO());
        }
        public string generarCodigoMedico(int tipo)
        {
            string codigo = "";
            string queryString;
            switch (tipo)
            {
                case 1://medico
                    queryString = "select 'M'+convert(varchar(20), count(*)+1) codigo from MEDICOEXTERNO where cmp like 'M%'";
                    break;
                case 2://veterinario
                    queryString = "select 'V'+convert(varchar(20), count(*)+1) codigo from MEDICOEXTERNO where cmp like 'V%'";
                    break;
                case 3://odontologo
                    queryString = "select 'O'+convert(varchar(20), count(*)+1) codigo from MEDICOEXTERNO where cmp like 'O%'";
                    break;
                default:
                    queryString = "";
                    break;
            }
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            codigo = reader["codigo"].ToString();
                        }

                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Close();

                }
            }
            return codigo;
        }
        public ActionResult CreateMedicoAjax(string item)
        {

            MEDICOEXTERNO medico = Newtonsoft.Json.JsonConvert.DeserializeObject<MEDICOEXTERNO>(item);
            medico.cmpCorregido = medico.cmp;
            medico.cmp = generarCodigoMedico(1);
            medico.tipomedico = 1;
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            medico.activo = false;
            medico.isDocumento = false;
            medico.isactivo = true;
            db.MEDICOEXTERNO.Add(medico);
            try
            {
                //db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, cmp = medico.cmp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMedico(string cmp)
        {
            //ViewBag.Sexo = new SelectList(new Variable().getSexo(), "codigo", "nombre", "F");
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            var med = db.MEDICOEXTERNO.Where(x => x.cmp == cmp).SingleOrDefault();
            return View(med);
        }

        public ActionResult UpdateMedicoAjax(string item)
        {
            MEDICOEXTERNO medico = Newtonsoft.Json.JsonConvert.DeserializeObject<MEDICOEXTERNO>(item);
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            var _med = db.MEDICOEXTERNO.Where(x => x.cmp == medico.cmp).SingleOrDefault();
            if (_med != null)
            {
                _med.nombres = medico.nombres;
                _med.apellidos = medico.apellidos;
                _med.telefono = medico.telefono;
                _med.direccion = medico.direccion;
                _med.codigoespecialidad = medico.codigoespecialidad;

            }
            else
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, cmp = medico.cmp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();//db.MEDICOEXTERNO.Where(m => m.isactivo == true).ToList());

        }
        class xmedico
        {
            public bool isDocumento { get; set; }
            public string apellidos { get; set; }

            public string nombres { get; set; }



            public string dniMedico { get; set; }

            public string categoria { get; set; }

            public string cmpCorregido { get; set; }

            public string especialidad { get; set; }

            public bool activo { get; set; }

            public string tipo { get; set; }

            public string cmp { get; set; }
            public bool isActiveWeb { get; set; }
            public bool isPago { get; set; }
            public string documento { get; set; }
        }

        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {

            var listaEncuestas = (from x in db.MEDICOEXTERNO
                                 // where x.isactivo == true
                                  //x.cmp=="4105"
                                  select new xmedico
                                  {
                                      apellidos = x.apellidos,
                                      nombres = x.nombres,
                                      tipo = x.TipoMedico1.descripcion,
                                      dniMedico = x.dniMedico == null ? "" : x.dniMedico,
                                      cmp = x.cmp,
                                      cmpCorregido = x.cmpCorregido == null ? x.cmp : x.cmpCorregido,                                     
                                      especialidad = x.ESPECIALIDAD.descripcion,
                                      activo = x.isactivo,
                                      isActiveWeb = x.isActiveWeb,
                                      isPago=x.isPaga,
                                      documento = x.DocumentoPago
                                  }).ToList();
            IEnumerable<xmedico> listaEncuestasFilter;
            param.sSearch = (Request["search[value]"]);
            param.iDisplayStart = Convert.ToInt32(Request["start"]);
            param.iDisplayLength = Convert.ToInt32(Request["length"]);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                listaEncuestasFilter = listaEncuestas
                     .Where(c => c.apellidos.ToLower().ToString().Contains(param.sSearch.ToLower())
                                 ||
                                 c.nombres.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.tipo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.dniMedico.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                  c.cmpCorregido.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                 c.especialidad.ToLower().Contains(param.sSearch.ToLower())
                                 );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var isapellidoSortable = Convert.ToBoolean(Request["columns[0][orderable]"]);
            var isnombreSortable = Convert.ToBoolean(Request["columns[1][orderable]"]);
            var iscmpSortable = Convert.ToBoolean(Request["columns[3][orderable]"]);
            //var isespecialidadSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
            var isactivoSortable = Convert.ToBoolean(Request["columns[4][orderable]"]);
            var isactivowebSortable = Convert.ToBoolean(Request["columns[5][orderable]"]);

            var sortColumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            Func<xmedico, string> orderingFunction = (
                c => sortColumnIndex == 0 && isapellidoSortable ? c.apellidos.ToString() :
                    sortColumnIndex == 1 && isnombreSortable ? c.nombres :
                    sortColumnIndex == 3 && iscmpSortable ? c.tipo.ToString() :
                    //sortColumnIndex == 4 && isespecialidadSortable ? c.especialidad :
                    sortColumnIndex == 4 && isactivoSortable ? c.activo.ToString() :
                    sortColumnIndex == 5 && isactivowebSortable ? c.isActiveWeb.ToString() :
                    "");

            var sortDirection = Request["order[0][dir]"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = (from c in displayedCompanies
                          select new[]{ 
                   c.apellidos,
                   c.nombres,
                   c.tipo,
                   c.cmpCorregido,
                  // c.especialidad,
                   c.activo?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                    c.isActiveWeb?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' />",
                   ( c.isPago?"<input type='checkbox' checked disabled='' />":"<input type='checkbox' disabled='' /> " )+c.documento,
                   "<a href='javascript:openModal(\"/Medico/Details?id="+c.cmp.Trim()+"\")' title='Detalles de  Médico: "+c.apellidos+" "+c.nombres+"'><i class='fa fa-list fa-lg'></i></a>    <a  href='javascript:openModal(\"/Medico/Edit?id="+c.cmp.Trim()+"\")'  title='Editar Médico: "+c.apellidos+" "+c.nombres+"'><i class='fa fa-edit fa-lg'></i></a>   <a  href='javascript:openModalDelete(\""+c.cmp.Trim()+"\",\""+c.apellidos+" "+c.nombres+"\")'  title='Eliminar Médico: "+c.apellidos+""+c.nombres+"'><i class='fa fa-trash-o fa-lg'></i></a>",
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

        //
        // GET: /Medico/Details/5

        public ActionResult Details(string id)
        {

            MEDICOEXTERNO m = db.MEDICOEXTERNO.SingleOrDefault(x => x.cmp == id);
            if (m == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tipo = new SelectList(db.TipoMedico.OrderBy(x => x.descripcion).ToList(), "idTipo", "descripcion");
            ViewBag.DocumentoPago = new SelectList(new Variable().getTipoDocumentoPago(), "codigo", "nombre");
            return View(m);

        }

        //
        // GET: /Medico/Create

        public ActionResult Create()
        {
            ViewBag.Tipo = new SelectList(db.TipoMedico.OrderBy(x => x.descripcion).ToList(), "idTipo", "descripcion");
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            ViewBag.Civil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
            ViewBag.DocumentoPago = new SelectList(new Variable().getTipoDocumentoPago(), "codigo", "nombre");
            return View();

        }
        //
        // POST: /Medico/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MEDICOEXTERNO medicoexterno)
        {
            medicoexterno.activo = false;
            if (!medicoexterno.isPaga)
            {
                medicoexterno.isDocumento = false;
                medicoexterno.DocumentoPago = "N";
            }
            else
                if (!medicoexterno.isDocumento)
                    medicoexterno.DocumentoPago = "N";
            if (ModelState.IsValid)
            {
                medicoexterno.isactivo = true;
                medicoexterno.cmp = generarCodigoMedico(medicoexterno.tipomedico);
                db.MEDICOEXTERNO.Add(medicoexterno);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Tipo = new SelectList(db.TipoMedico.OrderBy(x => x.descripcion).ToList(), "idTipo", "descripcion");
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            ViewBag.Sexo = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
            ViewBag.DocumentoPago = new SelectList(new Variable().getTipoDocumentoPago(), "codigo", "nombre");

            return View(medicoexterno);

        }

        //
        // GET: /Medico/Edit/5

        public ActionResult Edit(string id)
        {

            MEDICOEXTERNO medicoexterno = db.MEDICOEXTERNO.SingleOrDefault(x => x.cmp == id);
            if (medicoexterno == null)
            {
                return HttpNotFound();
            }
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            ViewBag.Tipo = new SelectList(db.TipoMedico.OrderBy(x => x.descripcion).ToList(), "idTipo", "descripcion");
            ViewBag.Civil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");

            ViewBag.DocumentoPago = new SelectList(new Variable().getTipoDocumentoPago(), "codigo", "nombre",medicoexterno.DocumentoPago);

            return View(medicoexterno);


        }

        //
        // POST: /Medico/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MEDICOEXTERNO medicoexterno)
        {
            MEDICOEXTERNO m = db.MEDICOEXTERNO.SingleOrDefault(x => x.cmp == medicoexterno.cmp);
            if (m != null && ModelState.IsValid)
            {
                m.apellidos = medicoexterno.apellidos;
                m.nombres = medicoexterno.nombres;
                m.celular = medicoexterno.telefono;
                m.direccion = medicoexterno.direccion;
                m.email = medicoexterno.email;
                m.fechanacio = medicoexterno.fechanacio;
                m.codigoespecialidad = medicoexterno.codigoespecialidad;
                m.activo = medicoexterno.activo;
                m.celular = medicoexterno.celular;
                m.dniMedico = medicoexterno.dniMedico;
                m.SexoMedico = medicoexterno.SexoMedico;
                m.estado_civil = medicoexterno.estado_civil;
                m.isPaga = medicoexterno.isPaga;
                m.categoria = medicoexterno.categoria;
                m.hobby = medicoexterno.hobby;
                m.isDocumento = medicoexterno.isDocumento;
                m.DocumentoPago = medicoexterno.DocumentoPago;
                m.tipomedico = medicoexterno.tipomedico;
                m.cmpCorregido = medicoexterno.cmpCorregido;
                m.isactivo = true;
                m.activo = false;
                if (!medicoexterno.isPaga)
                {
                    m.isDocumento = false;
                    m.DocumentoPago = "N";
                }
                else
                    if (!m.isDocumento)
                        m.DocumentoPago = "N";

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Especialidad = new SelectList(db.ESPECIALIDAD.OrderBy(x => x.descripcion).ToList(), "codigoespecialidad", "descripcion");
            ViewBag.Civil = new SelectList(new Variable().getEstadoCivil(), "codigo", "codigo");
            ViewBag.DocumentoPago = new SelectList(new Variable().getTipoDocumentoPago(), "codigo", "nombre");

            return View(medicoexterno);

        }



        public ActionResult SearchMedico()
        {
            return View();
        }

        public ActionResult ListaMedico(string filtro)
        {
            return View(db.MEDICOEXTERNO.Where(x => (x.apellidos.Contains(filtro) || x.cmp.Contains(filtro)) && x.activo == false && x.isactivo == true).AsParallel().ToList());
        }

        public ActionResult ValidarCMP(string cmp, int tipo)
        {
            var _exist = db.MEDICOEXTERNO.Where(x => x.cmpCorregido == cmp && x.tipomedico == tipo).FirstOrDefault();
            return Json(_exist == null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisableMedico(string cmp)
        {
            var medico = db.MEDICOEXTERNO.Where(x => x.cmp == cmp).FirstOrDefault();
            medico.isactivo = !medico.isactivo;
            medico.activo = !medico.activo;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.DenyGet);
        }
    }
}