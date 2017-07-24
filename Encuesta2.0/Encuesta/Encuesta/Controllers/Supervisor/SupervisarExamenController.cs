using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Supervisor;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Supervisor
{

    public class SupervisarExamenController : Controller
    {

        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: /Lista Realizar Examen de pacientes/
        [Authorize(Roles = "2")]
        public ActionResult ListaEspera()
        {

            return View();
        }

        //lista de examenes pendiestes
        [Authorize(Roles = "2")]
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var allCompanies = getExamenes();
            IEnumerable<SupervisarExamenViewModel> filteredCompanies;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                var examenFilter = Convert.ToString(Request["sSearch_0"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);
                var condicionFilter = Convert.ToString(Request["sSearch_5"]);
                var estudioFilter = Convert.ToString(Request["sSearch_6"]);
                var equipoFilter = Convert.ToString(Request["sSearch_7"]);

                //Optionally check whether the columns are searchable at all 
                var isexamenSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var iscondicionSearchable = Convert.ToBoolean(Request["bSearchable_5"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_6"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_7"]);

                filteredCompanies = getExamenes()
                   .Where(c => isexamenSearchable && c.num_examen.ToString().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.nom_paciente.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               iscondicionSearchable && c.condicion.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.nom_estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isequipoSearchable && c.nom_equipo.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = allCompanies;
            }

            var isexamenSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var ishcitaSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var ishadminSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var isminSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var iscondicionSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_6"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SupervisarExamenViewModel, string> orderingFunction = (c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                                                           sortColumnIndex == 1 && ispacienteSortable ? c.nom_paciente :
                                                           sortColumnIndex == 2 && ishcitaSortable ? c.hora_cita.ToString() :
                                                           sortColumnIndex == 3 && ishadminSortable ? c.hora_admision.ToString() :
                                                           sortColumnIndex == 4 && isminSortable ? c.min_transcurri :
                                                           sortColumnIndex == 5 && iscondicionSortable ? c.condicion :
                                                           sortColumnIndex == 6 && isestudioSortable ? c.nom_estudio :
                                                           sortColumnIndex == 7 && isequipoSortable ? c.nom_equipo :
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
                   (c.hora_cita).ToShortTimeString(),
                   Convert.ToString(c.hora_admision),
                   Convert.ToString(c.min_transcurri),
                   c.condicion,
                   c.nom_estudio,
                   c.nom_equipo,
                   Convert.ToString(c.num_examen),
                   Convert.ToString(c.isVIP),
                   Convert.ToString(c.isSedacion ),
                   Convert.ToString(c.docEscan),
                   Convert.ToString(c.docCarta),
                   Convert.ToString(c.isSupervisable),
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
        [Authorize(Roles = "2")]
        private IList<SupervisarExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            IList<SupervisarExamenViewModel> allSupervisiones = (from ea in db.EXAMENXATENCION
                                                                 join e in db.EQUIPO on ea.equipoAsignado equals e.codigoequipo into e_join
                                                                 from e in e_join.DefaultIfEmpty()
                                                                 join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                                                 join en in db.Encuesta on ea.codigo equals en.numeroexamen
                                                                 join sup in db.SupervisarEncuesta on ea.codigo equals sup.numeroexamen into sup_join
                                                                 from sup in sup_join.DefaultIfEmpty()
                                                                 join ca in db.CARTAGARANTIA on new { carta = ea.ATENCION.CITA.codigocartagarantia, paciente = ea.codigopaciente } equals new { carta = ca.codigocartagarantia, paciente = ca.codigopaciente } into carta_join
                                                                 from ca in carta_join.DefaultIfEmpty()
                                                                 join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                                                                 from esc in esc_join.DefaultIfEmpty()
                                                                 where ea.estadoestudio != "X" &&
                                                                     (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                                                     && en.estado == 1
                                                                     //&& en.estado < 4
                                                                     && en.SolicitarValidacion == true
                                                                 select new SupervisarExamenViewModel
                                                                 {
                                                                     num_examen = ea.codigo,
                                                                     estado = en.estado,
                                                                     nom_paciente = p.apellidos + " " + p.nombres,
                                                                     hora_cita = ea.horaatencion,
                                                                     hora_admision = ea.ATENCION.fechayhora,
                                                                     min_transcurri =
                                   SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                   SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                     condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
                                                                     nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                     nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
                                                                     isSedacion = ea.ATENCION.CITA.sedacion != null ? true : false,
                                                                     isVIP = ea.ATENCION.CITA.citavip != null ? true : false,
                                                                     docEscan = esc.numerodeatencion != null ? true : false,
                                                                     docCarta = ca.codigodocadjunto != null ? true : false,
                                                                     isSupervisable = en.SolicitarValidacion == null ? false : en.SolicitarValidacion.Value
                                                                 }).ToList();
            return allSupervisiones;
        }

        [Authorize(Roles = "2")]
        public ActionResult DatosExamen(int examen)
        {
            bool result = true;
            string msj = "";
            msj = getDatosExamen(examen);

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //get datos de paciente
        [Authorize(Roles = "2")]
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



        [Authorize(Roles = "2")]
        public ActionResult SupervisarExamen(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            SupervisarExamenViewModel model = new SupervisarExamenViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _encu = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            //var super = db.SupervisarEncuesta.Where(x => x.numeroexamen == _examen.codigo).SingleOrDefault();
            //REALIZAREXAMEN
            model.modalidad = _examen.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            model.num_examen = _examen.codigo;
            model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;
            model.isinEncuesta = false;
            model.isinImagenes = false;
            model.isEncuestaVisible = _encu.usu_reg_encu == user.ProviderUserKey.ToString() ? false : true;
            model.isEncuestaVisible = _encu.usu_reg_encu == user.ProviderUserKey.ToString() ? false : true;
            model.inicio = DateTime.Now;
            model.isSupervisable = _encu.SolicitarValidacion == null ? false : _encu.SolicitarValidacion.Value;
            //if (super != null)
            //{
            //    model.p1 = super.p1.Value;
            //    model.p1_1 = super.p1_1;
            //    model.p2 = super.p2.Value;
            //    model.p2_1 = super.p2_1;
            //    model.p3 = super.p3.Value;
            //    model.p3_1 = super.p3_1;
            //    model.p4 = super.p4.Value;
            //    model.p4_1 = super.p4_1;
            //    model.grupomedico = _examen.GrupoMedico.Value;
            //    model.diagnostico = super.anotaciones;
            //    model.isinEncuesta = super.isInEcnuesta.Value;
            //    model.isinImagenes = super.isInImagenes.Value;

            //}
            //ENCUESTA
            var encuestador = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_encu).SingleOrDefault();
            var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_tecno).SingleOrDefault();
            var supervisor = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_super).SingleOrDefault();
            var equipo = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
            model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = equipo == null ? "" : equipo.ShortDesc, nexamen = _examen.codigo };
            //PACIENTE
            model.paciente = _paciente;
            //EXAMENXATENCION
            model.examen = _examen;

            return View(model);
        }

        //
        // POST: /RealizarExamenViewModel/Create
        [Authorize(Roles = "2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupervisarExamen(SupervisarExamenViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var encuesta = db.Encuesta.Where(x => x.numeroexamen == item.num_examen).Single();
            var examen = db.EXAMENXATENCION.Where(x => x.codigo == item.num_examen).Single();
            var encuestador = db.USUARIO.Where(x => x.codigousuario == encuesta.usu_reg_encu).SingleOrDefault();
            var tecnologo = db.USUARIO.Where(x => x.codigousuario == encuesta.usu_Solicitavalidacion).SingleOrDefault();
            //modificamos datos de la encuesta
            encuesta.fec_ini_supervisa = item.inicio;
            encuesta.fec_supervisa = DateTime.Now;
            encuesta.usu_reg_super = user.ProviderUserKey.ToString(); ;
            encuesta.estado = 2;
            item.modalidad = examen.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            //ENCUESTA
            var supervisor = db.USUARIO.Where(x => x.codigousuario == encuesta.usu_reg_super).SingleOrDefault();
            var equipo = db.EQUIPO.Where(x => x.codigoequipo == examen.equipoAsignado).SingleOrDefault();
            item.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnologo == null ? "" : tecnologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = equipo == null ? "" : equipo.ShortDesc, nexamen = examen.codigo };
            //PACIENTE
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == examen.codigopaciente select x).SingleOrDefault();
            item.paciente = _paciente;
            //EXAMENXATENCION
            item.examen = examen;
            string _msj = "";
            var _s = db.SupervisarEncuesta.Where(x => x.numeroexamen == item.num_examen).SingleOrDefault();
            if (_s == null)
            {
                //SupervisonEncuesta
                SupervisarEncuesta _sup = new SupervisarEncuesta();
                _sup.numeroexamen = examen.codigo;
                _sup.tipo_encuesta = encuesta.tipo_encu;
                _sup.usu_reg = user.ProviderUserKey.ToString();
                _sup.fecha_reg = item.inicio;
                _sup.fecha_termino = DateTime.Now;
                _sup.superviso_encuesta = item.isEncuestaVisible;
                _sup.superviso_tecnologo = true;
                if (_sup.superviso_encuesta.Value)
                {
                    _sup.p1 = item.p1;
                    _sup.p1_1 = item.p1_1;
                    if (!_sup.p1.Value)
                        if (encuestador != null)
                            if (encuestador.EMPLEADO.email != "")
                            {
                                _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + encuestador.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 1 (Calificación Encuesta), el comentario registrado por " + user.UserName + @" fue:<strong>" + _sup.p1_1 + @"</strong>
                              </p>";
                                // new Variable().sendCorreo("Auditoría en Sistema de Encuesta", encuestador.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                            }
                }
                else
                {
                    _sup.p1 = null;
                    _sup.p1_1 = null;
                }

                _sup.p2 = item.p2;
                _sup.p2_1 = item.p2_1;
                if (!_sup.p2.Value)
                    if (tecnologo != null)
                        if (tecnologo.EMPLEADO.email != "")
                        {
                            _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + tecnologo.ShortName + @"</h1>
                                  <p> Se encontró una discrepancia en la pregunta 2 (Calificación Imágenes), el comentario registrado por " + user.UserName + @" fue:<strong>" + _sup.p2_1 + @"</strong>
                              </p>";
                            //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", tecnologo.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                        }
                _sup.p3 = item.p3;
                _sup.p3_1 = item.p3_1;
                if (!_sup.p3.Value)
                    if (tecnologo != null)
                        if (tecnologo.EMPLEADO.email != "")
                        {
                            _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + tecnologo.ShortName + @"</h1>
                                  <p> Se encontró una discrepancia en la pregunta 3 (Calificación Objetivo Clínico), el comentario registrado por " + user.UserName + @" fue:<strong>" + _sup.p3_1 + @"</strong>
                                   <br/>Para más detalles en el siguiente <a href='http://192.168.0.40:5126/LectorEncuesta/LectorEncuesta?examen=" + _sup.numeroexamen + @"&isEditable=true'>Enlace</a>
                              </p>";
                            //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", tecnologo.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                        }
                _sup.p4 = item.p4;
                _sup.p4_1 = item.p4_1;
                _sup.p5 = item.p5;
                _sup.p5_1 = item.p5_1;
                _sup.anotaciones = item.diagnostico.Trim();
                _sup.motivo_aplicacion_contraste = item.motivo_contraste;
                _sup.isInEcnuesta = item.isinEncuesta;
                _sup.isInImagenes = item.isinImagenes;
                //Agregamos la supervision
                if (db.SupervisarEncuesta.Where(x => x.numeroexamen == examen.codigo).ToList().Count <= 0)
                    db.SupervisarEncuesta.Add(_sup);
                try
                {

                    db.spu_verificarTurno_Supervision(examen.codigo, item.grupomedico.ToString(), user.ProviderUserKey.ToString());
                }
                catch (Exception)
                {
                }
                examen.GrupoMedico = item.grupomedico;
                if (item.isurgente)
                {
                    examen.prioridad = "URGENTE";
                    examen.prioridadMotivo = item.motivo_urgente;
                    examen.prioridadUser = user.ProviderUserKey.ToString();
                    examen.prioridadFecReg = DateTime.Now;
                }

                if (item.isPerfusion)
                {
                    var contraste = (from ci in db.Contraste
                                     where ci.atencion == examen.numeroatencion
                                     && ci.estado != 9
                                     select ci).ToList();

                    if (contraste.Count == 0)
                    {
                        var cita = (from ci in db.EXAMENXCITA
                                    where ci.numerocita == examen.numerocita &&
                                    ci.codigoexamencita == (db.EXAMENXCITA.Where(ci1 => ci1.numerocita == examen.numerocita).Max(ci1 => ci1.codigoexamencita))
                                    select ci).Single();
                        #region Registar Tecnica
                        string nestudio = "";
                        if (examen.codigoestudio.Substring(6, 1) == "1")
                            nestudio = "101010199050";
                        else if (examen.codigoestudio.Substring(6, 1) == "5")
                            nestudio = "101010599040";
                        else if
                            (examen.codigoestudio.Substring(6, 1) == "6")
                            nestudio = "101010699040";
                        else { }
                        if (nestudio != "")
                        {
                            EXAMENXCITA exc = new EXAMENXCITA();
                            exc.numerocita = cita.numerocita;
                            exc.codigopaciente = cita.codigopaciente;
                            exc.horacita = cita.horacita;
                            exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == cita.codigocompaniaseguro && cia.codigoestudio == nestudio select cia.preciobruto).SingleOrDefault();
                            exc.codigocompaniaseguro = cita.codigocompaniaseguro;
                            exc.ruc = cita.ruc;
                            exc.codigoequipo = cita.codigoequipo;
                            exc.codigoestudio = nestudio;
                            exc.codigoclase = int.Parse(nestudio.Substring(6, 1));
                            exc.codigomodalidad = cita.codigomodalidad;
                            exc.codigounidad = cita.codigounidad;
                            exc.codigomoneda = cita.codigomoneda;
                            exc.estadoestudio = "R";
                            db.EXAMENXCITA.Add(exc);
                            if (exc.codigoestudio.Substring(7, 2) == "99")
                            {
                                REGISTRODETECNICA reg = new REGISTRODETECNICA();
                                reg.numeroestudio = examen.codigo;
                                reg.codigoestudio = db.EXAMENXATENCION.Where(x => x.codigo == examen.codigo).Select(x => x.codigoestudio).Single();
                                reg.codigotecnica = nestudio;
                                reg.nombretecnica = db.ESTUDIO.Where(x => x.codigoestudio == nestudio).Select(x => x.nombreestudio).Single();
                                reg.codigoequipo = cita.codigoequipo;
                                reg.codigoclase = int.Parse(nestudio.Substring(6, 1));
                                db.REGISTRODETECNICA.Add(reg);
                            }
                        #endregion
                            #region Contraste
                            Contraste enf = new Contraste();
                            enf.numeroexamen = examen.codigo;

                            enf.tecnologo = user.ProviderUserKey.ToString();
                            enf.fecha_inicio = DateTime.Now;
                            enf.paciente = examen.codigopaciente;
                            enf.estado = 0;
                            enf.tipo_contraste = "";
                            enf.atencion = examen.numeroatencion;
                            db.Contraste.Add(enf);
                            db.SaveChanges();
                            if (examen.codigoestudio.StartsWith("1"))
                                if (examen.equipoAsignado != null)
                                    using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
                                    {
                                        try
                                        {
                                            AlertasEncuestas ae = new AlertasEncuestas();
                                            ae.fecha = DateTime.Now;
                                            ae.usuario = user.UserName;
                                            ae.ubicacion_solicitante = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == examen.equipoAsignado).ShortDesc.ToLower();
                                            ae.tipo_solicitud = "Enfermera";
                                            ae.isReproducido = false;
                                            db1.AlertasEncuestas.Add(ae);
                                            db1.SaveChanges();
                                        }
                                        catch (Exception) { }
                                    }
                            #endregion
                        }
                    }
                }
                db.SaveChanges();


                return RedirectToAction("ListaEspera");
            }
            else
            {
                return View(item);
            }
        }

        public ActionResult UpdateSupervisar(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            SupervisarExamenViewModel model = new SupervisarExamenViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _encu = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var super = db.SupervisarEncuesta.Where(x => x.numeroexamen == _examen.codigo).SingleOrDefault();
            if (super != null)
            {
                //REALIZAREXAMEN
                model.num_examen = _examen.codigo;
                model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;
                model.isinEncuesta = false;
                model.isinImagenes = false;
                model.isEncuestaVisible = _encu.usu_reg_encu == user.ProviderUserKey.ToString() ? true : false;
                model.inicio = DateTime.Now;
                model.isEncuestaVisible = _encu.usu_reg_encu == user.ProviderUserKey.ToString() ? false : true;
                model.p1 = super.p1;
                model.p1_1 = super.p1_1;
                model.p2 = super.p2.Value;
                model.p2_1 = super.p2_1;
                model.p3 = super.p3.Value;
                model.p3_1 = super.p3_1;
                model.p4 = super.p4.Value;
                model.p4_1 = super.p4_1;
                model.grupomedico = _examen.GrupoMedico.Value;
                model.diagnostico = super.anotaciones;
                model.isinEncuesta = super.isInEcnuesta.Value;
                model.isinImagenes = super.isInImagenes.Value;

                //ENCUESTA
                var encuestador = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_encu).SingleOrDefault();
                var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_tecno).SingleOrDefault();
                var supervisor = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_super).SingleOrDefault();
                var equipo = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
                model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = equipo == null ? "" : equipo.ShortDesc, nexamen = _examen.codigo };
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
        [Authorize(Roles = "2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateSupervisar(SupervisarExamenViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var encuesta = db.Encuesta.Where(x => x.numeroexamen == item.num_examen).Single();
            var examen = db.EXAMENXATENCION.Where(x => x.codigo == item.num_examen).Single();
            var super = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen.codigo).SingleOrDefault();

            encuesta.fec_ini_supervisa = item.inicio;
            encuesta.fec_supervisa = DateTime.Now;
            encuesta.usu_reg_super = user.ProviderUserKey.ToString(); ;

            //SupervisonEncuesta

            super.numeroexamen = examen.codigo;
            super.tipo_encuesta = encuesta.tipo_encu;
            super.usu_reg = user.ProviderUserKey.ToString();
            super.fecha_reg = item.inicio;
            super.fecha_termino = DateTime.Now;
            super.superviso_encuesta = item.isEncuestaVisible;
            super.superviso_tecnologo = true;
            if (super.superviso_encuesta.Value)
            {
                super.p1 = item.p1;
                super.p1_1 = item.p1_1;
            }
            else
            {
                super.p1 = null;
                super.p1_1 = null;
            }
            super.p2 = item.p2;
            super.p2_1 = item.p2_1;

            super.p3 = item.p3;
            super.p3_1 = item.p3_1;
            super.p4 = item.p4;
            super.p4_1 = item.p4_1;
            super.anotaciones = item.diagnostico.Trim();

            examen.GrupoMedico = item.grupomedico;

            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }

        [Authorize(Roles = "2")]
        public ActionResult CalificacionEncuesta(int examen)
        {
            var super = (from x in db.SupervisarEncuesta
                         where x.numeroexamen == examen
                         select new SupervisarExamenViewModel
                         {
                             num_examen = x.numeroexamen.Value,
                             p1 = x.p1,
                             p1_1 = x.p1_1,
                             p2 = x.p2,
                             p2_1 = x.p2_1,
                             p3 = x.p3,
                             p3_1 = x.p3_1,
                             p4 = x.p4,
                             p4_1 = x.p4_1,
                             diagnostico = x.anotaciones,
                             isEncuestaVisible = x.superviso_encuesta.Value,
                             isTecnologoVisible = x.superviso_tecnologo.Value

                         }).SingleOrDefault();
            /*if (super != null)
            {
                */
            super.examen = db.EXAMENXATENCION.Where(z => z.codigo == examen).SingleOrDefault();
            var _encu = (from x in db.Encuesta where x.numeroexamen == examen select x).SingleOrDefault();
            super.paciente = (from x in db.PACIENTE where x.codigopaciente == super.examen.codigopaciente select x).SingleOrDefault();

            var encuestador = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_encu).SingleOrDefault();
            var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_tecno).SingleOrDefault();
            var supervisor = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_super).SingleOrDefault();
            var equipo = db.EQUIPO.Where(x => x.codigoequipo == super.examen.equipoAsignado).SingleOrDefault();
            super.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel
            {
                encuestador = encuestador == null ? "" : encuestador.ShortName,
                tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName,
                supervisor = supervisor == null ? "" : supervisor.ShortName,
                equipo = equipo == null ? "" : equipo.ShortDesc,
                nexamen = examen
            };
            /* }*/
            return View(super);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupervicionExterna(int examen, bool p1, string p1_1, bool isInencuesta, bool impax)
        {
            var _super = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_super != null)
            {
                _super.p1 = p1;
                _super.p1_1 = p1_1;
                _super.superviso_encuesta = true;
                _super.isInEcnuesta = isInencuesta;
                db.SaveChanges();
            }
            if (impax)
                return RedirectToAction("CalificacionEncuesta", new { examen = examen, impax = true });
            else
                return RedirectToAction("CalificacionEncuesta", new { examen = examen });

        }

        [Authorize(Roles = "2")]
        public ActionResult CambioCalificacion(SupervisarExamenViewModel cambio)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var super = (from x in db.SupervisarEncuesta
                         where x.numeroexamen == cambio.num_examen
                         select new SupervisarExamenViewModel
                         {
                             num_examen = x.numeroexamen.Value,
                             p1 = x.p1,
                             p1_1 = x.p1_1,
                             p2 = x.p2,
                             p2_1 = x.p2_1,
                             p3 = x.p3,
                             p3_1 = x.p3_1,
                             p4 = x.p4,
                             p4_1 = x.p4_1,
                             diagnostico = x.anotaciones,
                             isEncuestaVisible = x.superviso_encuesta.Value,
                             isTecnologoVisible = x.superviso_tecnologo.Value

                         }).SingleOrDefault();
            if (super != null)
            {
                var _s = db.SupervisarEncuesta.Where(x => x.numeroexamen == cambio.num_examen).Single();
                if (cambio.tipo_cambio == 1)
                {
                    _s.p1_informante = cambio.p1_cambio;
                    _s.p1_1_informante = cambio.p1_1_cambio;
                    _s.p2_informante = cambio.p2_cambio;
                    _s.p2_1_informante = cambio.p2_1_cambio;
                    _s.p3_informante = cambio.p3_cambio;
                    _s.p3_1_informante = cambio.p3_1_cambio;
                    _s.p4_informante = cambio.p4_cambio;
                    _s.p4_1_informante = cambio.p4_1_cambio;
                    _s.fec_reg_inf = DateTime.Now;
                    _s.usu_reg_inf = user.ProviderUserKey.ToString();
                }
                else
                {
                    _s.p1_validador = cambio.p1_cambio;
                    _s.p1_1_validador = cambio.p1_1_cambio;
                    _s.p2_validador = cambio.p2_cambio;
                    _s.p2_1_validador = cambio.p2_1_cambio;
                    _s.p3_validador = cambio.p3_cambio;
                    _s.p3_1_validador = cambio.p3_1_cambio;
                    _s.p4_ivalidador = cambio.p4_cambio;
                    _s.p4_1_validador = cambio.p4_1_cambio;
                    _s.fec_reg_val = DateTime.Now;
                    _s.usu_reg_val = user.ProviderUserKey.ToString();
                }

                db.SaveChanges();
            }

            super.examen = db.EXAMENXATENCION.Where(z => z.codigo == cambio.num_examen).SingleOrDefault();
            var _encu = (from x in db.Encuesta where x.numeroexamen == cambio.num_examen select x).SingleOrDefault();
            super.paciente = (from x in db.PACIENTE where x.codigopaciente == super.examen.codigopaciente select x).SingleOrDefault();

            var encuestador = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_encu).SingleOrDefault();
            var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_tecno).SingleOrDefault();
            var supervisor = db.USUARIO.Where(x => x.codigousuario == _encu.usu_reg_super).SingleOrDefault();
            var equipo = db.EQUIPO.Where(x => x.codigoequipo == super.examen.equipoAsignado).SingleOrDefault();
            super.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel
            {
                encuestador = encuestador == null ? "" : encuestador.ShortName,
                tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName,
                supervisor = supervisor == null ? "" : supervisor.ShortName,
                equipo = equipo == null ? "" : equipo.ShortDesc,
                nexamen = cambio.num_examen
            };
            if (cambio.modo)
                return RedirectToAction("CalificacionEncuesta", new { examen = cambio.num_examen, impax = true });
            else
                return RedirectToAction("CalificacionEncuesta", new { examen = cambio.num_examen });

        }

        [Authorize(Roles = "1,2,5,16")]
        public ActionResult CancelarExamen(int examen, bool all, string motivo, string detalle)
        {
            bool result = true;
            string msj = "";
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _exame = (from x in db.EXAMENXATENCION where x.codigo == examen select x).Single();
            var _cita = _exame.ATENCION.CITA.EXAMENXCITA.ToList();
            _exame.estadoestudio = "X";
            _exame.motivo = motivo;
            _exame.otros_motivos = detalle;
            _exame.fecha_cancela = DateTime.Now;
            _exame.usu_cancela = user.ProviderUserKey.ToString();
            foreach (var c in _cita)
            {
                if (c.codigoestudio == _exame.codigoestudio)
                {
                    c.estadoestudio = "X";
                    c.motivo = motivo;
                    c.otros_motivos = detalle;
                    c.fecha_cancela = DateTime.Now;
                    c.usu_cancela = user.ProviderUserKey.ToString();
                }
            }
            if (all)
            {
                var e = db.EXAMENXATENCION.Where(x => x.numeroatencion == _exame.numeroatencion).ToList();
                foreach (var itema in e)
                {
                    itema.estadoestudio = "X";
                    itema.motivo = motivo;
                    itema.otros_motivos = detalle;
                    itema.fecha_cancela = DateTime.Now;
                    itema.usu_cancela = user.ProviderUserKey.ToString();
                }
                foreach (var item in _cita)
                {
                    item.estadoestudio = "X";
                    item.motivo = motivo;
                    item.otros_motivos = detalle;
                    item.fecha_cancela = DateTime.Now;
                    item.usu_cancela = user.ProviderUserKey.ToString();
                }
            }

            db.SaveChanges();
            log4net.LogicalThreadContext.Properties["username"] = user.UserName.ToString();
            string host;
            try
            {
                host = (new Variable()).getClientName(Request.UserHostName);
            }
            catch (Exception)
            {
                host = "No Disponible";
            }

            log4net.LogicalThreadContext.Properties["host"] = host;
            log.Info("Cancelacion N° Examen " + _exame.codigo);
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }


        public ActionResult ChangeCalificacionEncuesta(int pregunta, int examen, int tipo, bool? rpta, string rpta_det, int rpta4)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            SupervisarEncuesta cali = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (cali == null)
            {
                cali = new SupervisarEncuesta();
                cali.numeroexamen = examen;
                var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
                if (_encu != null)
                    cali.tipo_encuesta = _encu.tipo_encu;
                db.SupervisarEncuesta.Add(cali);
            }
            string _msj = @"", user_medico = user.UserName.ToString();


            #region Pregunta 1
            if (pregunta == 1)
            {
                if (tipo == 1)
                {
                    cali.p1_informante = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p1_1_informante = rpta_det;
                            if (cali.Supervisor != null)
                                if (cali.Supervisor.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " + cali.Supervisor.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 1 (Calificación de Encuesta) del Examen:" + cali.numeroexamen + @", el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </p>";
                                    // new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Supervisor.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p1_1_informante = null;
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    cali.p1_validador = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p1_1_validador = rpta_det;
                            if (cali.Informante != null)
                                if (cali.Informante.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " + cali.Informante.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 1 (Calificación de Encuesta) del Examen:" + cali.numeroexamen + @", el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </p>";
                                    // new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Informante.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p1_1_validador = null;
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion
            #region Pregunta 2
            else if (pregunta == 2)
            {
                if (tipo == 1)
                {
                    cali.p2_informante = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p2_1_informante = rpta_det;
                            if (cali.Supervisor != null)
                                if (cali.Supervisor.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " + cali.Supervisor.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 2 (Calificación de Imágenes) del Examen:" + cali.numeroexamen + @", el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </p>";
                                    //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Supervisor.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p2_1_informante = null;
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    cali.p2_validador = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p2_1_validador = rpta_det;
                            if (cali.Informante != null)
                                if (cali.Informante.EMPLEADO.email != "")
                                {
                                    _msj = @"  <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a): " + cali.Informante.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 2 (Calificación de Imágenes) del Examen:" + cali.numeroexamen + @", el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </p>";
                                    //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Informante.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p2_1_validador = null;
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion
            #region Pregunta 3
            else if (pregunta == 3)
            {
                if (tipo == 1)
                {
                    cali.p3_informante = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p3_1_informante = rpta_det;
                            if (cali.Supervisor != null)
                                if (cali.Supervisor.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + cali.Supervisor.ShortName + @"</h1>
                                   <p>Se encontró una discrepancia en la pregunta 3 (Calificación Objetivo Clínico), el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </p>";
                                    //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Supervisor.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p3_1_informante = null;
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    cali.p3_validador = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p3_1_validador = rpta_det;
                            if (cali.Informante != null)
                                if (cali.Informante.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + cali.Informante.ShortName + @"
                                   Se encontró una discrepancia en la pregunta 3 (Calificación Objetivo Clínico), el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </h1>";
                                    //new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Informante.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                        else
                            cali.p3_1_validador = null;
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion
            #region Pregunta 4
            else if (pregunta == 4)
            {
                if (tipo == 1)
                {
                    if (rpta4 == 10)
                        cali.p4_informante = null;
                    else
                        cali.p4_informante = rpta4;

                    if (rpta4 == 0)
                        cali.p4_1_informante = rpta_det;
                    else
                        cali.p4_1_validador = null;
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    if (rpta4 == 10)
                        cali.p4_ivalidador = null;
                    else
                        cali.p4_ivalidador = rpta4;

                    if (rpta4 == 0)
                        cali.p4_1_validador = rpta_det;
                    else
                        cali.p4_1_validador = null;
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion
            #region Pregunta 5
            else if (pregunta == 5)
            {
                if (tipo == 1)
                {
                    cali.diagnostico_info = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.diagnostico_inf = rpta_det;
                            if (cali.Supervisor != null)
                                if (cali.Supervisor.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + cali.Supervisor.ShortName + @"
                                   Se encontró una discrepancia en la pregunta de 4 (Diagnóstico Preliminar), el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </h1>";
                                    // new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Supervisor.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    cali.diagnostico_vali = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                        {
                            cali.p4_1_validador = rpta_det;
                            if (cali.Informante != null)
                                if (cali.Informante.EMPLEADO.email != "")
                                {
                                    _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + cali.Informante.ShortName + @"
                                   Se encontró una discrepancia en la pregunta de 4 (Diagnóstico Preliminar), el comentario registrado por " + user.UserName + @" fue:<strong>" + rpta_det + @"</strong>
                              </h1>";
                                    // new Variable().sendCorreo("Auditoría en Sistema de Encuesta", cali.Informante.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                                }
                        }
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion
            #region Pregunta 6
            else if (pregunta == 6)
            {
                if (tipo == 1)
                {
                    cali.p5_informante = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                            cali.p5_1_informante = rpta_det;
                        else
                            cali.p5_1_informante = null;
                    cali.usu_reg_inf = user.ProviderUserKey.ToString();
                    cali.fec_reg_inf = DateTime.Now;
                }
                else
                {
                    cali.p5_validador = rpta;
                    if (rpta != null)
                        if (rpta.Value == false)
                            cali.p5_1_validador = rpta_det;
                        else
                            cali.p5_1_validador = null;
                    cali.usu_reg_val = user.ProviderUserKey.ToString();
                    cali.fec_reg_val = DateTime.Now;
                }
            }
            #endregion


            if (cali.p1_informante == null && cali.p2_informante == null && cali.p3_informante == null && cali.p4_informante == null && cali.diagnostico_info == null && cali.p5_informante == null)
            {
                cali.usu_reg_inf = null;
                cali.fec_reg_inf = null;
                //user_medico = "-";
            }
            if (cali.p1_validador == null && cali.p2_validador == null && cali.p3_validador == null && cali.p4_ivalidador == null && cali.diagnostico_vali == null && cali.p5_validador == null)
            {
                cali.usu_reg_val = null;
                cali.fec_reg_val = null;
                //user_medico = "-";
            }

            db.SaveChanges();

            return Json(new { result = true, data = user_medico }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult VerificarCalificacion(int examen, int tipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            SupervisarEncuesta cali = db.SupervisarEncuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            string result = "0";
            if (cali != null)
                if (tipo == 1)
                {
                    if (cali.p1_informante != null && cali.p2_informante != null && cali.p3_informante != null && !(cali.anotaciones != null && cali.diagnostico_info == null))
                    {
                        result = "1";
                        if (cali.p1_informante == null)
                            cali.p1_informante = true;
                        if (cali.p2_informante == null)
                            cali.p2_informante = true;
                        if (cali.p3_informante == null)
                            cali.p3_informante = true;
                        if (cali.diagnostico_info == null)
                            cali.diagnostico_info = true;
                    }
                }
                else
                {
                    if (cali.p1_validador != null && cali.p2_validador != null && cali.p3_validador != null && !(cali.anotaciones != null && cali.diagnostico_vali == null))
                    {
                        result = "1";
                        if (cali.p1_validador == null)
                            cali.p1_validador = true;
                        if (cali.p2_validador == null)
                            cali.p2_validador = true;
                        if (cali.p3_validador == null)
                            cali.p3_validador = true;
                        if (cali.diagnostico_vali == null)
                            cali.diagnostico_vali = true;
                    }
                }

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ListadoSupervisiones(string inicio, string fin)
        {
            string sql = @"select 
s.numeroexamen,
(select shortname from USUARIO u where u.codigousuario=e.usu_reg_encu) encuesta,
(select shortname from USUARIO u where u.codigousuario=e.usu_reg_tecno) tecnologo,
--supervisor
s.fecha_reg,
s.p1,
s.p1_1,
s.p2,
s.p2_1,
s.p3,
s.p3_1,
s.anotaciones,
(select shortname from USUARIO u where u.codigousuario=s.usu_reg) supervisor,
--informante
s.p1_informante,
s.p1_1_informante,
s.p2_informante,
s.p2_1_informante,
s.p3_informante,
s.p3_1_informante,
s.diagnostico_info,
(select shortname from USUARIO u where u.codigousuario=i.medicoinforma) informante,
--validador
s.p1_validador,
s.p1_1_validador,
s.p2_validador,
s.p2_1_validador,
s.p3_validador,
s.p3_1_validador,
s.diagnostico_vali,
(select shortname from USUARIO u where u.codigousuario=i.medicorevisa) validador
 from SupervisarEncuesta  s
 inner join Encuesta e on s.numeroexamen=e.numeroexamen
 inner join  EXAMENXATENCION ea on e.numeroexamen=ea.codigo
 inner join INFORMEMEDICO i on ea.codigo=i.numeroinforme
 where
 --s.numeroexamen=710885
 convert(date,s.fecha_reg) between '" + inicio+"' and '"+fin+@"'
 and ea.estadoestudio ='V'
 and 
 (
 s.p1_informante=0 or s.p2_informante=0 or s.p3_informante=0 or s.diagnostico_info=0 or
 s.p1_validador=0 or s.p2_validador=0 or s.p3_validador=0 or s.diagnostico_vali=0
 )";
            List<RevisionCalificacion> lista = new List<RevisionCalificacion>();
            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(sql, connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            RevisionCalificacion item = new RevisionCalificacion();
                            item.numeroexamen = Convert.ToInt32(reader["numeroexamen"].ToString());
                            item.encuestador = (reader["encuesta"].ToString());
                            item.tecnologo = reader["tecnologo"].ToString();
                            item.fecha_registro = Convert.ToDateTime(reader["fecha_reg"].ToString());
                            #region Supervisor
                            if (reader["p1"] != DBNull.Value)
                                item.p1 = Convert.ToBoolean(reader["p1"].ToString());
                            if (reader["p1_1"] != DBNull.Value)
                                item.p1_motivo = (reader["p1_1"].ToString());

                            if (reader["p2"] != DBNull.Value)
                                item.p2 = Convert.ToBoolean(reader["p2"].ToString());
                            if (reader["p2_1"] != DBNull.Value)
                                item.p2_motivo = (reader["p2_1"].ToString());

                            if (reader["p3"] != DBNull.Value)
                                item.p3 = Convert.ToBoolean(reader["p3"].ToString());
                            if (reader["p3_1"] != DBNull.Value)
                                item.p3_motivo = (reader["p3_1"].ToString());

                            if (reader["anotaciones"] != DBNull.Value)
                                item.dx_pre = (reader["anotaciones"].ToString());

                            item.supervisor = (reader["supervisor"].ToString());
                            #endregion
                            #region Informante
                            if (reader["p1_informante"] != DBNull.Value)
                                item.p1_inf = Convert.ToBoolean(reader["p1_informante"].ToString());
                            if (reader["p1_1_informante"] != DBNull.Value)
                                item.p1_inf_motivo = (reader["p1_1_informante"].ToString());

                            if (reader["p2_informante"] != DBNull.Value)
                                item.p2_inf = Convert.ToBoolean(reader["p2_informante"].ToString());
                            if (reader["p2_1_informante"] != DBNull.Value)
                                item.p2_inf_motivo = (reader["p2_1_informante"].ToString());

                            if (reader["p3_informante"] != DBNull.Value)
                                item.p3_inf = Convert.ToBoolean(reader["p3_informante"].ToString());
                            if (reader["p3_1_informante"] != DBNull.Value)
                                item.p3_inf_motivo = (reader["p3_1_informante"].ToString());

                            if (reader["p3_1_informante"] != DBNull.Value)
                                item.p3_inf_motivo = (reader["p3_1_informante"].ToString());
                            if (reader["diagnostico_info"] != DBNull.Value)
                                item.dx_pre_inf = Convert.ToBoolean(reader["diagnostico_info"].ToString());

                            item.informante = (reader["informante"].ToString());
                            #endregion
                            #region Validador
                            if (reader["p1_validador"] != DBNull.Value)
                                item.p1_val = Convert.ToBoolean(reader["p1_validador"].ToString());
                            if (reader["p1_1_validador"] != DBNull.Value)
                                item.p1_val_motivo = (reader["p1_1_validador"].ToString());

                            if (reader["p2_validador"] != DBNull.Value)
                                item.p2_val = Convert.ToBoolean(reader["p2_validador"].ToString());
                            if (reader["p2_1_validador"] != DBNull.Value)
                                item.p2_val_motivo = (reader["p2_1_validador"].ToString());

                            if (reader["p3_validador"] != DBNull.Value)
                                item.p3_val = Convert.ToBoolean(reader["p3_validador"].ToString());
                            if (reader["p3_1_validador"] != DBNull.Value)
                                item.p3_val_motivo = (reader["p3_1_validador"].ToString());

                            if (reader["diagnostico_vali"] != DBNull.Value)
                                item.dx_pre_val = Convert.ToBoolean(reader["diagnostico_vali"].ToString());

                            item.validador = (reader["validador"].ToString());
                            #endregion

                            lista.Add(item);

                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }



                }
            }
            return View(lista); 
        }

    }
}


