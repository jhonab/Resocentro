using Encuesta.Member;
using Encuesta.Models;
using Encuesta.ViewModels.Tecnologo;
using System;
using System.Collections.Generic;
//using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Tecnologo
{
    [Authorize(Roles = "8")]
    public class PostProcesoController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        //
        // GET: /PostProceso/

        public ActionResult ListaEspera()
        {

            return View();
        }

        //lista de examenes pendiestes
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {
            var allCompanies = getExamenes();
            IEnumerable<PostProcesoViewModel> filteredCompanies;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                var examenFilter = Convert.ToString(Request["sSearch_0"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);
                var modalidadFilter = Convert.ToString(Request["sSearch_5"]);
                var estudioFilter = Convert.ToString(Request["sSearch_6"]);
                var equipoFilter = Convert.ToString(Request["sSearch_7"]);

                //Optionally check whether the columns are searchable at all 
                var isexamenSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var ismodalidadSearchable = Convert.ToBoolean(Request["bSearchable_5"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_6"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredCompanies = getExamenes()
                   .Where(c => isexamenSearchable && c.num_examen.ToString().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.nom_paciente.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               ismodalidadSearchable && c.modalidad.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.nom_estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isequipoSearchable && c.nom_equipo_pro.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = allCompanies;
            }

            var isexamenSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var ishadminSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isminSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var ismodalidadSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PostProcesoViewModel, string> orderingFunction = (c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                                                           sortColumnIndex == 1 && ispacienteSortable ? c.nom_paciente :
                                                           sortColumnIndex == 2 && ishadminSortable ? c.hora_admision.ToString() :
                                                           sortColumnIndex == 3 && isminSortable ? c.min_transcurri :
                                                           sortColumnIndex == 4 && ismodalidadSortable ? c.modalidad :
                                                           sortColumnIndex == 5 && isestudioSortable ? c.nom_estudio :
                                                           sortColumnIndex == 6 && isequipoSortable ? c.nom_equipo_pro :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredCompanies = filteredCompanies.OrderBy(orderingFunction);
            else
                filteredCompanies = filteredCompanies.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.num_examen),
                c.nom_paciente,
                Convert.ToString(c.hora_admision),
                Convert.ToString(c.min_transcurri),
                c.modalidad,
                c.nom_estudio,
                c.nom_equipo_pro,
                Convert.ToString(c.num_examen),
                Convert.ToString(c.isVIP),
                Convert.ToString(c.isSedacion ),
                ""};

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = allCompanies.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        //metodo que obtiene la data
        private IList<PostProcesoViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            List<PostProcesoViewModel> allCompanies = new List<PostProcesoViewModel>();
            string queryString = @"select ea.codigo, p.apellidos + ' , ' + p.nombres paciente, pp.fecha, es.nombreestudio, case when e.nombreequipo = null then 'No asignado' else e.nombreequipo end equipo, c.sedacion, c.citavip, case when substring(ea.codigoestudio,4,2) = '01' then 'RM' when substring(ea.codigoestudio,4,2) = '02' then 'TEM' else 'Otros' end modalidad from EXAMENXATENCION ea inner join EQUIPO e on ea.equipoAsignado = e.codigoequipo inner join PACIENTE p on ea.codigopaciente = p.codigopaciente inner join POSTPROCESO pp on ea.codigo = pp.numeroestudio inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio inner join CITA c on ea.numerocita = c.numerocita where pp.estado in ('N','P') AND pp.tipo = 'E' and ea.estadoestudio != 'X' and convert(date,pp.fecha) between convert(date,DATEADD(day,-5,GETDATE())) and  convert(date,GETDATE())   order by pp.fecha desc";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        DateTime fecha = Convert.ToDateTime(reader["fecha"]);
                        TimeSpan tps = DateTime.Now - fecha;
                        allCompanies.Add(new PostProcesoViewModel
                        {
                            num_examen = Convert.ToInt32(reader["codigo"]),
                            nom_paciente = reader["paciente"].ToString(),
                            hora_admision = Convert.ToDateTime(reader["fecha"]),
                            min_transcurri = tps.Days == 0 ? tps.Hours.ToString() + ":" + tps.Minutes.ToString() : tps.Days.ToString() + " día(s) " + tps.Hours.ToString() + ":" + tps.Minutes.ToString(),
                            nom_estudio = reader["nombreestudio"].ToString(),
                            nom_equipo_pro = reader["equipo"].ToString(),
                            isSedacion = Convert.ToBoolean(reader["sedacion"]),
                            isVIP = Convert.ToBoolean(reader["citavip"]),
                            modalidad = reader["modalidad"].ToString(),
                        });
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            //List<PostProcesoViewModel> allCompanies = (from ea in db.EXAMENXATENCION
            //                                            join e in db.EQUIPO on ea.equipoAsignado equals e.codigoequipo into e_join
            //                                            from e in e_join.DefaultIfEmpty()
            //                                            join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
            //                                            join en in db.Encuesta on ea.codigo equals en.numeroexamen into en_join
            //                                            from en in en_join.DefaultIfEmpty()
            //                                            join post in db.POSTPROCESO on ea.codigo equals post.numeroestudio
            //                                            where
            //                                            (new string[] { "N", "P" }).Contains(post.estado)
            //                                            && post.tipo == "E"
            //                                            && ea.estadoestudio != "X"
            //                                            && SqlFunctions.DateDiff("month", post.fecha, SqlFunctions.GetDate()) == DateTime.Now.Month
            //                                            select new PostProcesoViewModel
            //                                               {
            //                                                   num_examen = ea.codigo,
            //                                                   nom_paciente = p.apellidos + ", " + p.nombres,
            //                                                   hora_admision = post.fecha,
            //                                                   min_transcurri =
            //                        SqlFunctions.DateDiff("month", post.fecha, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", post.fecha, SqlFunctions.GetDate())) + " mes(es)") :
            //                        SqlFunctions.DateDiff("day", post.fecha, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", post.fecha, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", post.fecha, SqlFunctions.GetDate())),
            //                                                   nom_estudio = ea.ESTUDIO.nombreestudio,
            //                                                   nom_equipo_pro = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
            //                                                   isSedacion = ea.ATENCION.CITA.sedacion,
            //                                                   isVIP = ea.ATENCION.CITA.citavip,
            //                                                   modalidad = ea.codigoestudio.Substring(3, 2) == "01" ? "RM" : ea.codigoestudio.Substring(3, 2) == "02" ? "TEM" : "Otros"

            //                 }).ToList();
                return allCompanies;
        }

        //Listar Tecnica
        public ActionResult ListarTecnicas(int nexamen)
        {
            var lista = db.POSTPROCESO.Where(x => x.numeroestudio == nexamen && x.tipo == "D").ToList();
            return PartialView("_ListaPostProceso", lista);
        }

        //Ingresar Tecnica
        public ActionResult IngresarTecnica(int nexamen, string ctec, string tec, int cant, int cequipo)
        {
            bool result = true;
            string msj = "";
            var _examen = db.EXAMENXATENCION.Where(x => x.codigo == nexamen).Single();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            POSTPROCESO post = new POSTPROCESO();
            post.numeroestudio = nexamen;
            post.codigopaciente = _examen.codigopaciente;
            post.codigoestudio = ctec;
            post.codigousuario = user.ProviderUserKey.ToString();
            post.descripcion = tec;
            post.cantidadplaca = cant;
            post.fecha = DateTime.Now;
            post.placamalogro = 0;
            post.placaprueba = 0;
            post.placareimpresa = 0;
            post.codigoequipo = cequipo;
            post.estado = "R";
            post.tipo = "D";
            db.POSTPROCESO.Add(post);

            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult DatosExamen(int examen)
        {
            bool result = true;
            string msj = "";
            msj = getDatosExamen(examen);

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //get datos de paciente
        private string getDatosExamen(int examen)
        {
            string msj = "";
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();

            msj = "<div class='form-group'>" +
                    "<label>N° Examen : </label> " +
                     _examen.codigo +
                "</div>" +
                "<div class='form-group'>" +
                    "<label>Paciente : </label> " +
                   _paciente.apellidos + " " +
                   _paciente.nombres +
                "</div>" +
                "<div class='form-group'>" +
                    "<label>DNI : </label> " +
                    _paciente.dni +
                "</div>" +
                 "<div class='form-group'>" +
                    "<label>Nacimiento : </label> " +
                   _paciente.fechanace.ToShortDateString() +
                "</div>" +
                 "<div class='form-group'>" +
                    "<label>Edad : </label> " +
                    (_paciente.fechanace.Year < DateTime.Now.Year ? (DateTime.Now.Year - _paciente.fechanace.Year).ToString() + " años" : (DateTime.Now.Month - _paciente.fechanace.Month).ToString() + " meses") +
                "</div>" +
                 "<div class='form-group'> " +
                    "<label>Peso:</label> " +
                    _examen.ATENCION.peso.ToString() +
                "</div>" +
                "<div class='form-group'> " +
                    "<label>CMP:</label> " +
                    _examen.ATENCION.CITA.cmp.ToString() +
                "</div>";
            return msj;
        }

        //Elimiar tecnica
        public ActionResult EliminarTecnica(int npost)
        {
            bool result = true;
            string msj = "";
            var post = (from ci in db.POSTPROCESO
                        where ci.codigopostproceso == npost
                        select ci).Single();

            db.POSTPROCESO.Remove(post);
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult CancelarExamen(int examen, string motivo)
        {
            bool result = true;
            string msj = "";
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _post = db.POSTPROCESO.Where(x => x.numeroestudio == examen && x.tipo == "E" && x.estado != "R").Single();
            _post.estado = "X";
            _post.motivocancelacion = motivo;

            var _encu = db.Encuesta.Where(X => X.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {

                _encu.fec_ini_proto = DateTime.Now;
                _encu.fec_fin_proto = _encu.fec_ini_proto.Value.AddMilliseconds(3);
                _encu.usu_reg_proto = user.ProviderUserKey.ToString();
                _encu.estado = 4;
            }
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //
        // GET: /RealizarExamenViewModel/Create
        public ActionResult RealizarExamen(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            PostProcesoViewModel model = new PostProcesoViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            //REALIZAREXAMEN
            model.num_examen = _examen.codigo;
            model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;
            model.isSedacion = _examen.ATENCION.CITA.sedacion;
            model.tecnicas = new List<POSTPROCESO>();
            model.descripcion = db.POSTPROCESO.Where(x => x.numeroestudio == _examen.codigo && x.tipo == "E" && x.estado != "X").Single().descripcion;
            model.inicio = DateTime.Now;

            ViewBag.lstEquipo = new SelectList(db.Equipo_PostProceso.ToList(), "codigoequipo", "nombreequipo");
            var mod = int.Parse(_examen.codigoestudio.Substring(4, 1));
            ViewBag.tecnicas = db.Estudio_PostProceso.Where(x => x.modalidad == mod).ToList();
            //ENCUESTA
            if (_encuesta != null)
            {
                var encuestador = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_encu).SingleOrDefault();
                var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_tecno).SingleOrDefault();
                var supervisor = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_super).SingleOrDefault();
                var equipo = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
                model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = equipo == null ? "" : equipo.ShortDesc };
            }
            model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = "", tecnologo = "", supervisor = "", equipo = "" };
            //PACIENTE
            model.paciente = _paciente;
            //EXAMENXATENCION
            model.examen = _examen;

            //TECNICAS

            return View(model);
        }

        //
        // POST: /RealizarExamenViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarExamen(PostProcesoViewModel item)
        {
            var _post = db.POSTPROCESO.Where(x => x.numeroestudio == item.num_examen && x.tipo == "E" && x.estado != "R" && x.estado!="X").Single();
            var _encu = db.Encuesta.Where(X => X.numeroexamen == item.num_examen).SingleOrDefault();
            if (_encu != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                _encu.fec_ini_proto = item.inicio;
                _encu.fec_fin_proto = DateTime.Now;
                _encu.usu_reg_proto = user.ProviderUserKey.ToString();
                if (_encu.estado == 3)
                    _encu.estado = 4;
            }
            _post.estado = "R";
            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }

        public ActionResult UpdatePostProceso(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            PostProcesoViewModel model = new PostProcesoViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).Single();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).Single();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).Single();
            var _post = db.POSTPROCESO.Where(x => x.numeroestudio == _examen.codigo && x.tipo == "E").FirstOrDefault();
            if (_post != null)
            {
                //REALIZAREXAMEN
                model.num_examen = _examen.codigo;
                model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;
                model.isSedacion = _examen.ATENCION.CITA.sedacion;
                model.tecnicas = db.POSTPROCESO.Where(x => x.numeroestudio == _examen.codigo && x.tipo == "D").ToList();
                model.descripcion = db.POSTPROCESO.Where(x => x.numeroestudio == _examen.codigo && x.tipo == "E").First().descripcion;
                model.istecnicas = model.tecnicas.Count() > 0;

                ViewBag.lstEquipo = new SelectList(db.Equipo_PostProceso.ToList(), "codigoequipo", "nombreequipo");
                var mod = int.Parse(_examen.codigoestudio.Substring(4, 1));
                ViewBag.tecnicas = db.Estudio_PostProceso.Where(x => x.modalidad == mod).ToList();
                //ENCUESTA
                var encuestador = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_encu).SingleOrDefault();
                var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_tecno).SingleOrDefault();
                var supervisor = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_super).SingleOrDefault();
                var equipo = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
                model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = equipo == null ? "" : equipo.ShortDesc };

                //PACIENTE
                model.paciente = _paciente;
                //EXAMENXATENCION
                model.examen = _examen;
                return View(model);
            }
            return RedirectToAction("ListaEspera");

        }

        //
        // POST: /RealizarExamenViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePostProceso(PostProcesoViewModel item)
        {
            var _post = db.POSTPROCESO.Where(x => x.numeroestudio == item.num_examen && x.tipo == "E").FirstOrDefault();
            _post.estado = "R";
            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }

        //Registrar PostProceso
        public ActionResult RegistrarPostproceso(int examen, string descrip)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = true;
            string msj = "";
            var _ea = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            if (_ea != null)
            {
                
                    var _post = db.POSTPROCESO.Where(x => x.numeroestudio == _ea.codigo && x.tipo == "E").FirstOrDefault();
                    if (_post != null)
                    {
                        _post.estado = "N";
                        _post.descripcion = descrip == null ? "" : descrip.ToString();
                        _post.fecha = DateTime.Now;
                    }
                    else
                    {
                        POSTPROCESO post = new POSTPROCESO();
                        post.numeroestudio = _ea.codigo;
                        post.codigopaciente = _ea.codigopaciente;
                        post.codigoestudio = "9999";
                        post.codigousuario = user.ProviderUserKey.ToString();
                        post.descripcion = descrip == null ? "" : descrip.ToString();
                        post.cantidadplaca = 0;
                        post.fecha = DateTime.Now;
                        post.estado = "N";
                        post.tipo = "E";
                        db.POSTPROCESO.Add(post);
                    }

                    db.SaveChanges();
                
            }
            else
            {
                result = false;
                msj = "El numero de examen ingresado no existe";
            }
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

    }
}
