using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Encuesta;
using Encuesta.ViewModels.Encuesta.Tipos;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Encuesta
{
    [Authorize(Roles = "1")]
    public class RealizarEncuestaController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: /Lista Realizar Encuesta de pacientes/
        public ActionResult ListaEspera()
        {

            return View();
        }

        //lista de examenes pendiestes async
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {
            var listaEncuestas = getExamenes();
            IEnumerable<EncuestaExamenViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var examenFilter = Convert.ToString(Request["sSearch_0"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);
                var sedeFilter = Convert.ToString(Request["sSearch_2"]);
                var estudioFilter = Convert.ToString(Request["sSearch_7"]);
                var equipoFilter = Convert.ToString(Request["sSearch_8"]);

                //Optionally check whether the columns are searchable at all 
                var isexamenSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var issedeSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_7"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_8"]);

                listaEncuestasFilter = getExamenes()
                   .Where(c => isexamenSearchable && c.num_examen.ToString().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.paciente.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               issedeSearchable && c.sede.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.nom_estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isequipoSearchable && c.nom_equipo.ToLower() == (param.sSearch.ToLower())
                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var isexamenSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var issedeSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var ishcitaSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var ishadminSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isminSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_7"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_8"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EncuestaExamenViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                    sortColumnIndex == 1 && ispacienteSortable ? c.paciente :
                    sortColumnIndex == 2 && issedeSortable ? c.sede.ToString() :
                    sortColumnIndex == 3 && ishcitaSortable ? c.hora_cita.ToString() :
                    sortColumnIndex == 4 && ishadminSortable ? c.hora_admision.ToString() :
                    sortColumnIndex == 5 && isminSortable ? c.min_transcurri :
                    sortColumnIndex == 6 && isestudioSortable ? c.nom_estudio :
                    sortColumnIndex == 7 && isequipoSortable ? c.nom_equipo :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                   Convert.ToString(c.num_examen),
                   c.paciente,
                   c.sede,
                   (c.hora_cita).ToShortTimeString(),
                   Convert.ToString(c.hora_admision),
                   c.nom_estudio,
                   c.nom_equipo,
                   Convert.ToString(c.num_examen),
                   Convert.ToString(c.isVIP),
                   Convert.ToString(c.isSedacion ),
                   Convert.ToString(c.isAsignado ),
                   Convert.ToString(c.docEscan),
                   Convert.ToString(c.isEncuesta),
                   Convert.ToString(c.docCarta),
                   Convert.ToString(c.tipo_encuesta.Value==-1?false:true),
                   Convert.ToString(c.isAyer)
               };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = listaEncuestas.Count(),
                iTotalDisplayRecords = listaEncuestasFilter.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }


        public ActionResult AsignarHopitalizado()
        {
            return View();
        }

        public ActionResult setHopitalizado(int codigo)
        {
            var _exam = db.EXAMENXATENCION.SingleOrDefault(x => x.codigo == codigo);
            if (_exam != null)
            {
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                _exam.prioridad = "URGENTE";
                _exam.prioridadMotivo = "Paciente HOSPITALIZADO detectado en ENCUESTA";
                _exam.prioridadUser = user.ProviderUserKey.ToString();
                _exam.prioridadFecReg = DateTime.Now;
                db.SaveChanges();
            }
            return Json(true);
        }

        //metodo que obtiene la data
        private IList<EncuestaExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool sedador = User.IsInRole("5");
            List<EncuestaExamenViewModel> allSupervisiones = new List<EncuestaExamenViewModel>();
            /* IList<EncuestaExamenViewModel> allSupervisiones = (from ea in db.EXAMENXATENCION
                                                                join e in db.EQUIPO on ea.codigoequipo equals e.codigoequipo into e_join
                                                                from e in e_join.DefaultIfEmpty()
                                                                join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                                                join en in db.Encuesta on ea.codigo equals en.numeroexamen into encu_join
                                                                from en in encu_join.DefaultIfEmpty()
                                                                join ca in db.CARTAGARANTIA on ea.ATENCION.CITA.codigocartagarantia equals ca.codigocartagarantia into carta_join
                                                                from ca in carta_join.DefaultIfEmpty()
                                                                join esc in db.ESCANADMISION on ea.numeroatencion equals esc.numerodeatencion into esc_join
                                                                from esc in esc_join.DefaultIfEmpty()
                                                                join su in db.SUCURSAL on new { codigounidad = ea.codigoestudio.Substring(0, 1), codigosucursal = ea.codigoestudio.Substring(2, 1) } equals new { codigounidad = SqlFunctions.StringConvert((double)(su.codigounidad)).Trim(), codigosucursal = SqlFunctions.StringConvert((double)(su.codigosucursal)).Trim() }
                                                                where ea.estadoestudio == "A" &&
                                                                    (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                                                    && (ea.equipoAsignado == null || ea.equipoAsignado == 0)

                                                                orderby ea.codigo
                                                                select new EncuestaExamenViewModel
                                                                  {
                                                                      num_examen = ea.codigo,
                                                                      tipo_encuesta = en.tipo_encu,
                                                                      isFinEncuesta = en.fec_paso3 != null ? true : false,
                                                                      paciente = p.apellidos + " " + p.nombres,
                                                                      sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc,
                                                                      hora_cita = ea.horaatencion,
                                                                      hora_admision = ea.ATENCION.fechayhora == null ? (new DateTime(2015, 1, 29, 10, 0, 0)) : ea.ATENCION.fechayhora,
                                                                      min_transcurri =
                                    SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                    SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                      condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
                                                                      nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                      nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
                                                                      isSedacion = ea.ATENCION.CITA.sedacion == null ? false : ea.ATENCION.CITA.sedacion,
                                                                      isVIP = ea.ATENCION.CITA.citavip == null ? false : ea.ATENCION.CITA.citavip,
                                                                      turnmedico = ea.turnomedico,
                                                                      isAsignado = ea.equipoAsignado != null ? true : false,
                                                                      isEncuesta = en.fec_paso3 != null ? true : false,
                                                                      docEscan = esc.numerodeatencion != null ? true : false,
                                                                      docCarta = ca.codigodocadjunto != null ? true : false,


                                                                  }).AsParallel().ToList();*/
            string queryString = @"exec dbo.getListaEncuesta '" + String.Join(",", user.sucursales.ToArray()) + "',1";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format(queryString, user.sucursales.ToString()), connection);
                connection.Open();
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        /*DateTime fecha = Convert.ToDateTime(reader["hora_admision"]);
                        TimeSpan tps = DateTime.Now - fecha;*/
                        allSupervisiones.Add(new EncuestaExamenViewModel
                                                                    {
                                                                        num_examen = Convert.ToInt32(reader["num_examen"]),
                                                                        tipo_encuesta = Convert.ToInt32(reader["tipo_encuesta"]),
                                                                        isFinEncuesta = Convert.ToBoolean(reader["isFinEncuesta"]),
                                                                        paciente = reader["paciente"].ToString(),
                                                                        sede = reader["sede"].ToString(),
                                                                        hora_cita = Convert.ToDateTime(reader["hora_cita"]),
                                                                        hora_admision = Convert.ToDateTime(reader["hora_admision"]),
                                                                        nom_estudio = reader["nom_estudio"].ToString(),
                                                                        nom_equipo = reader["nom_equipo"].ToString(),
                                                                        isSedacion = Convert.ToBoolean(reader["isSedacion"]),
                                                                        isVIP = Convert.ToBoolean(reader["isVIP"]),
                                                                        isAsignado = Convert.ToBoolean(reader["isAsignado"]),
                                                                        isEncuesta = Convert.ToBoolean(reader["isEncuesta"]),
                                                                        docEscan = Convert.ToBoolean(reader["docEscan"]),
                                                                        docCarta = Convert.ToBoolean(reader["docCarta"]),
                                                                        isAyer = Convert.ToBoolean(reader["isAyer"])
                                                                    });
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                    connection.Dispose();
                    connection.Close();
                }
            }
            return allSupervisiones;
        }

        //lista de equipos asignados por sucursal
        public ActionResult getSedes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            return Json(new SelectList((from su in db.SUCURSAL
                                        where (user.sucursales_int).Contains
                                        (su.codigounidad * 100 + su.codigosucursal) //sucursales asignadas
                                        select new { codigo = (su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc), id = (su.codigounidad * 100 + su.codigosucursal) }).AsQueryable(),
                                         "id",
                                         "codigo"),
                                         JsonRequestBehavior.AllowGet);

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
        public ActionResult getinfoAmpliacion(int examen)
        {
            //por el codigo de examen de origen
            List<spu_getIfIsAmpliInDemand_byCodigo_Ultimo_Result> lista = db.spu_getIfIsAmpliInDemand_byCodigo_Ultimo(examen).ToList();
            return View(lista);
        }

        public ActionResult getAmpliacioninfo(int examen)
        {
            //por el codigo de ampliacion
            List<spu_getIfIsAmpliInDemand_byCodigoAmpliacion_Ultimo_Result> lista = db.spu_getIfIsAmpliInDemand_byCodigoAmpliacion_Ultimo(examen).ToList();
            var item = lista.First();

            var estudio = db.EXAMENXATENCION.Where(x => x.codigo == item.codigo).SingleOrDefault();
            if (estudio != null)
                ViewBag.Estudio = estudio.ESTUDIO.nombreestudio;
            else
                ViewBag.Estudio = "No se relaciono correctamente en la encuesta.";
            return View(lista);
        }

        public ActionResult ValidarAmpliacion(int examen, int paciente)
        {
            string msj = "";
            bool rsl = false;
            var _encu = (from ea in db.EXAMENXATENCION
                         where ea.codigo == examen
                             && ea.codigopaciente == paciente
                         && ea.IsAmpliInDemand == true
                         select ea).ToList();
            if (_encu.Count > 0)
                rsl = true;
            return Json(new { result = rsl, msj = msj }, JsonRequestBehavior.DenyGet);
        }
        /****************************************************************************************/
        //AMPLIACION
        public ActionResult RealizarEncuestaAmpliacion(int examen)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            EncuestaAmpliacionViewModel item = new EncuestaAmpliacionViewModel();
            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();

            item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";

            item.numeroexamen = _exam.codigo;
            item.sexo = _paci.sexo;
            item._examen = _exam;
            item._paciente = _paci;

            ViewBag.Motivos = new SelectList((new Variable()).getAutorizaciones(), "codigo", "nombre");

            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarEncuestaAmpliacion(EncuestaAmpliacionViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();

            var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();

            item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";

            item.numeroexamen = _exam.numeroatencion;
            item.sexo = _paci.sexo;
            item._examen = _exam;
            item._paciente = _paci;
            ViewBag.Motivos = new SelectList((new Variable()).getAutorizaciones(), "codigo", "nombre");

            _exam.equipoAsignado = item.equipoAsignado;
            //_exam.CodigoDeEstudioOrigen = item.origen_examen;
            //origen 
            /*var exa_origen = db.EXAMENXATENCION.Where(x => x.codigo == item.origen_examen).SingleOrDefault();
            exa_origen.AmpliCodigo = item.numeroexamen;*/
            //ENCUESTA
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Models.Encuesta encu = new Models.Encuesta();
            encu.numeroexamen = _exam.codigo;
            encu.usu_reg_encu = user.ProviderUserKey.ToString();
            encu.fec_ini_encu = DateTime.Now;
            encu.fec_paso1 = encu.fec_ini_encu.AddSeconds(1);
            encu.fec_paso2 = encu.fec_paso1.Value.AddSeconds(1);
            encu.fec_paso3 = encu.fec_paso2.Value.AddSeconds(1);
            encu.tipo_encu = (int)EncuestaType.EncuestaAmpliacion;
            encu.estado = 1;
            encu.atencion = _exam.numeroatencion;
            db.Encuesta.Add(encu);
            if (db.Encuesta.Where(x => x.numeroexamen == _exam.codigo).ToList().Count() == 0)
            {
                if (!item.isInformeAmpliatorio)
                    try
                    {
                        db.spu_setAmpliacion(item.origen_examen, _exam.codigo);
                    }
                    catch (Exception)
                    {
                    }

                db.SaveChanges();
            }

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
            log.Info("Registro Ampliacion de Encuesta N° Examen " + encu.numeroexamen);

            return RedirectToAction("ListaEspera");
        }
        /****************************************************************************************/

        //ASIGNAR O REASIGNAR MODALIDAD
        public ActionResult setModalidad(int examen, int equipo)
        {
            //cambiar el equipo asignado
            bool result = true;
            var _exa = (from x in db.EXAMENXATENCION
                        where x.codigo == examen
                        select x).SingleOrDefault();
            _exa.equipoAsignado = equipo;
            //elimar integrador
            var integador = (from x in db.INTEGRACION
                             where x.numero_estudio == examen
                             select x).SingleOrDefault();
            if (integador != null)
            {
                db.INTEGRACION.Remove(integador);
                //eliminar MYSQL
                try
                {
                    new Variable().eliminarIntegrador(examen + ".1");
                }
                catch (Exception ex)
                {
                }

                /*using (pacsdbEntities mysql = new pacsdbEntities())
                {
                    string sps_id = examen + ".1";
                    var _inte = mysql.mwl_item.SingleOrDefault(x => x.sps_id == sps_id);
                    if (_inte != null)
                    {
                        mysql.mwl_item.Remove(_inte);
                        mysql.SaveChanges();
                    }

                }*/
            }

            //modificar encuesta
            var encu = (from x in db.Encuesta
                        where x.numeroexamen == examen
                        select x).FirstOrDefault();
            encu.fec_ini_tecno = null;
            db.SaveChanges();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
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
            log.Info("Cambio Tipo de Equipo " + equipo.ToString() + " N° Examen " + encu.numeroexamen);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        //CAMBIAR TIPO DE ENCUESTA
        public ActionResult setEncuesta(int examen, int encuesta)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                _encu.tipo_encu = encuesta;
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
                log.Info("Cambio Encuesta tipo de encuesta " + encuesta.ToString() + " N° Examen " + examen);
                db.SaveChanges();
            }
            return RedirectToAction("RealizarEncuesta", new { examen = examen });
        }

        /****************************************************************************************/
        //ENCUEStA
        public ActionResult RealizarEncuesta(int examen)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            if (_exam.codigoestudio.Substring(5, 1) == "0")//si no es ampliacion
            {
                var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
                EncuestaExamenViewModel encuesta = new EncuestaExamenViewModel();
                if (_encu == null)
                {
                    var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();

                    encuesta.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";

                    encuesta.numeroatencion = _exam.numeroatencion;
                    encuesta.fec_reg_ini = DateTime.Now;
                    encuesta.CheckBox31 = _exam.ATENCION.CITA.sedacion;
                    encuesta.encuesta = new Models.Encuesta();
                    encuesta.num_examen = _exam.codigo;
                    encuesta.sexo = _paci.sexo;
                    encuesta._examen = _exam;
                    encuesta._paciente = _paci;
                    ViewBag.Motivos = new SelectList((new Variable()).getAutorizaciones(), "codigo", "nombre");
                    var med_pre = db.MED_PRE_ENTREVISTA.Where(x => x.numeroatencion == _exam.numeroatencion).SingleOrDefault();
                    #region Cargar Pre-Entrevista
                    if (med_pre != null)
                    {
                        CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                        encuesta.usu_reg = user.ProviderUserKey.ToString();
                        encuesta.numeroatencion = med_pre.numeroatencion;
                        encuesta.p1 = med_pre.CheckBox1;
                        encuesta.p2 = med_pre.CheckBox2;
                        encuesta.CheckBox3 = med_pre.CheckBox3;
                        encuesta.CheckBox4 = med_pre.CheckBox4;
                        encuesta.CheckBox5 = med_pre.CheckBox5;
                        encuesta.CheckBox6 = med_pre.CheckBox6;
                        encuesta.CheckBox7 = med_pre.CheckBox7;
                        encuesta.CheckBox8 = med_pre.CheckBox8;
                        encuesta.CheckBox9 = med_pre.CheckBox9;
                        encuesta.CheckBox10 = med_pre.CheckBox10;
                        encuesta.CheckBox11 = med_pre.CheckBox11;
                        encuesta.CheckBox12 = med_pre.CheckBox12;
                        encuesta.CheckBox13 = med_pre.CheckBox13;
                        encuesta.CheckBox14 = med_pre.CheckBox14;
                        encuesta.CheckBox15 = med_pre.CheckBox15;
                        encuesta.CheckBox16 = med_pre.CheckBox16;
                        encuesta.CheckBox17 = med_pre.CheckBox17;
                        encuesta.CheckBox18 = med_pre.CheckBox18;
                        encuesta.CheckBox19 = med_pre.CheckBox19;
                        encuesta.CheckBox20 = med_pre.CheckBox20;
                        encuesta.CheckBox21 = med_pre.CheckBox21;
                        encuesta.txtOtros = med_pre.txtOtros;
                        encuesta.CheckBox30 = med_pre.CheckBox30;
                        encuesta.CheckBox31 = med_pre.CheckBox31;
                        encuesta.txtNumeroConsentimiento = med_pre.txtNumeroConsentimiento;
                        encuesta.isAutorizado = med_pre.isAutorizado;
                        if (med_pre.tipoAutorizacion != null)
                            encuesta.tipoAutorizacion = med_pre.tipoAutorizacion.Value;
                        encuesta.detalleAutorizacion = med_pre.detalleAutorizacion;
                        encuesta.fec_reg_ini = med_pre.fec_reg_ini;
                        encuesta.fec_reg_fin = med_pre.fec_reg_fin;
                    }
                    #endregion
                    return View(encuesta);
                }
                else
                {
                    EncuestaType tipo_encu = (EncuestaType)_encu.tipo_encu;
                    #region Redireccionar Encuesta
                    switch (tipo_encu)
                    {
                        case EncuestaType.Cerebro:
                            return RedirectToAction("EncuestaCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.HombroIzquierdo:
                        case EncuestaType.HombroDerecho:
                            return RedirectToAction("EncuestaHombro", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.RodillaIzquierda:
                        case EncuestaType.RodillaDerecha:
                            return RedirectToAction("EncuestaRodilla", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.ColumnaCervical:
                        case EncuestaType.ColumnaCervicoDorsal:
                            return RedirectToAction("EncuestaColumnaA", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.ColumnaDorsal:
                        case EncuestaType.ColumnaDorsoLumbar:
                        case EncuestaType.ColumnaLumboSacra:
                            return RedirectToAction("EncuestaColumnaB", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.ColumnaSacroCoxis:
                            return RedirectToAction("EncuestaColumnac", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.Caderaderecha:
                        case EncuestaType.Caderaizquierda:
                            return RedirectToAction("EncuestaCadera", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.Cododerecha:
                        case EncuestaType.Codoizquierda:
                            return RedirectToAction("EncuestaCodo", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.TobilloIzquierdo:
                        case EncuestaType.PieIzquierdo:
                        case EncuestaType.DedosIzquierdo:
                        case EncuestaType.TobilloDerecho:
                        case EncuestaType.PieDerecho:
                        case EncuestaType.DedosDerecho:
                            return RedirectToAction("EncuestaPie", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.MeniqueDerecho:
                        case EncuestaType.AnularDerecho:
                        case EncuestaType.MedioDerecho:
                        case EncuestaType.IndiceDerecho:
                        case EncuestaType.PulgarDerecho:
                        case EncuestaType.ManoDerecha:
                        case EncuestaType.MunecaDerecha:
                        case EncuestaType.MeniqueIzquierdo:
                        case EncuestaType.AnularIzquierdo:
                        case EncuestaType.MedioIzquierdo:
                        case EncuestaType.IndiceIzquierdo:
                        case EncuestaType.PulgarIzquierdo:
                        case EncuestaType.ManoIzquierdo:
                        case EncuestaType.MunecaIzquierdo:
                            return RedirectToAction("EncuestaMano", new { examen = _exam.codigo, tipo = _encu.tipo_encu });


                        case EncuestaType.Abdomen:
                            return RedirectToAction("EncuestaAbdomen", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.BrazoDerecho:
                        case EncuestaType.AntebrazoDerecho:
                        case EncuestaType.MusloDerecho:
                        case EncuestaType.PiernaDerecho:
                        case EncuestaType.BrazoIzquierdo:
                        case EncuestaType.AntebrazoIzquierdo:
                        case EncuestaType.MusloIzquierdo:
                        case EncuestaType.PiernaIzquierdo:
                            return RedirectToAction("EncuestaExtremidad", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.MamaDerecha:
                        case EncuestaType.MamaIzquierda:
                            return RedirectToAction("EncuestaMama", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.Oncologico:
                            return RedirectToAction("EncuestaOncologio", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.Colonoscopia:
                            return RedirectToAction("EncuestaColonoscopia", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.AngiotemCoronario:
                            return RedirectToAction("EncuestaAngioCoronario", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.AngiotemAorta:
                            return RedirectToAction("EncuestaAngioAorta", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.AngiotemCerebral:
                            return RedirectToAction("EncuestaTEMCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu });
                        //return RedirectToAction("EncuestaAngioCerebral", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.MusculoEsqueletico:
                            return RedirectToAction("EncuestaMusculoEsqueletico", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.NeuroCerebro:
                            return RedirectToAction("EncuestaNeuroCerebro", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.Generica:
                            return RedirectToAction("EncuestaGenericaTEM", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        case EncuestaType.EncuestaVacia:
                            return RedirectToAction("EncuestaVacia", new { examen = _exam.codigo, tipo = _encu.tipo_encu });

                        default:
                            return RedirectToAction("ListaEspera");
                    }
                    #endregion
                }
            }
            else
            {
                return RedirectToAction("RealizarEncuestaAmpliacion", new { examen = examen });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarEncuesta(EncuestaExamenViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.num_examen).SingleOrDefault();

            //Agregar Preentrevista
            var med_pre = db.MED_PRE_ENTREVISTA.Where(x => x.numeroatencion == _exam.numeroatencion).SingleOrDefault();
            if (med_pre == null)
            {
                #region Pre-Entrevista
                MED_PRE_ENTREVISTA pre = new MED_PRE_ENTREVISTA();
                pre.modalidad = int.Parse(_exam.codigoestudio.Substring(4, 1));
                pre.usu_reg = user.ProviderUserKey.ToString();
                pre.numeroatencion = item.numeroatencion;
                pre.CheckBox1 = item.p1;
                pre.CheckBox2 = item.p2;
                pre.CheckBox3 = item.CheckBox3;
                pre.CheckBox4 = item.CheckBox4;
                pre.CheckBox5 = item.CheckBox5;
                pre.CheckBox6 = item.CheckBox6;
                pre.CheckBox7 = item.CheckBox7;
                pre.CheckBox8 = item.CheckBox8;
                pre.CheckBox9 = item.CheckBox9;
                pre.CheckBox10 = item.CheckBox10;
                pre.CheckBox11 = item.CheckBox11;
                pre.CheckBox12 = item.CheckBox12;
                pre.CheckBox13 = item.CheckBox13;
                pre.CheckBox14 = item.CheckBox14;
                pre.CheckBox15 = item.CheckBox15;
                pre.CheckBox16 = item.CheckBox16;
                pre.CheckBox17 = item.CheckBox17;
                pre.CheckBox18 = item.CheckBox18;
                pre.CheckBox19 = item.CheckBox19;
                pre.CheckBox20 = item.CheckBox20;
                pre.CheckBox21 = item.CheckBox21;
                pre.txtOtros = item.txtOtros;
                pre.CheckBox30 = item.CheckBox30;
                pre.CheckBox31 = item.CheckBox31;
                pre.txtNumeroConsentimiento = item.txtNumeroConsentimiento;
                pre.isAutorizado = item.isAutorizado;
                pre.tipoAutorizacion = item.tipoAutorizacion;
                pre.detalleAutorizacion = item.detalleAutorizacion;
                pre.fec_reg_ini = item.fec_reg_ini;
                pre.fec_reg_fin = item.fec_reg_fin;
                db.MED_PRE_ENTREVISTA.Add(pre);
                #endregion
            }
            //Agregar Encuesta
            Models.Encuesta encu = new Models.Encuesta();
            encu.numeroexamen = _exam.codigo;
            encu.usu_reg_encu = user.ProviderUserKey.ToString();
            encu.fec_ini_encu = item.fec_reg_ini;
            encu.fec_paso1 = item.fec_reg_fin;
            encu.fec_paso2 = DateTime.Now;
            encu.tipo_encu = item.tipo_encuesta;
            encu.estado = 0;
            encu.atencion = _exam.numeroatencion;
            if (item.isHospitalizado != null)
                if (item.isHospitalizado.Value)
                {
                    _exam.prioridad = "URGENTE";
                    _exam.prioridadMotivo = "Paciente HOSPITALIZADO detectado en ENCUESTA";
                    _exam.prioridadUser = user.ProviderUserKey.ToString();
                    _exam.prioridadFecReg = DateTime.Now;
                }
            db.Encuesta.Add(encu);
            if (db.Encuesta.Where(x => x.numeroexamen == _exam.codigo).ToList().Count() == 0)
                db.SaveChanges();

            EncuestaType tipo_encu = (EncuestaType)item.tipo_encuesta;
            #region Redireccionar Encuesta
            switch (tipo_encu)
            {
                case EncuestaType.Cerebro:
                    return RedirectToAction("EncuestaCerebro", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.HombroIzquierdo:
                case EncuestaType.HombroDerecho:
                    return RedirectToAction("EncuestaHombro", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.RodillaIzquierda:
                case EncuestaType.RodillaDerecha:
                    return RedirectToAction("EncuestaRodilla", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.ColumnaCervical:
                case EncuestaType.ColumnaCervicoDorsal:
                    return RedirectToAction("EncuestaColumnaA", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.ColumnaDorsal:
                case EncuestaType.ColumnaDorsoLumbar:
                case EncuestaType.ColumnaLumboSacra:
                    return RedirectToAction("EncuestaColumnaB", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.ColumnaSacroCoxis:
                    return RedirectToAction("EncuestaColumnac", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Caderaderecha:
                case EncuestaType.Caderaizquierda:
                    return RedirectToAction("EncuestaCadera", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Cododerecha:
                case EncuestaType.Codoizquierda:
                    return RedirectToAction("EncuestaCodo", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.TobilloIzquierdo:
                case EncuestaType.PieIzquierdo:
                case EncuestaType.DedosIzquierdo:
                case EncuestaType.TobilloDerecho:
                case EncuestaType.PieDerecho:
                case EncuestaType.DedosDerecho:
                    return RedirectToAction("EncuestaPie", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.MeniqueDerecho:
                case EncuestaType.AnularDerecho:
                case EncuestaType.MedioDerecho:
                case EncuestaType.IndiceDerecho:
                case EncuestaType.PulgarDerecho:
                case EncuestaType.ManoDerecha:
                case EncuestaType.MunecaDerecha:
                case EncuestaType.MeniqueIzquierdo:
                case EncuestaType.AnularIzquierdo:
                case EncuestaType.MedioIzquierdo:
                case EncuestaType.IndiceIzquierdo:
                case EncuestaType.PulgarIzquierdo:
                case EncuestaType.ManoIzquierdo:
                case EncuestaType.MunecaIzquierdo:
                    return RedirectToAction("EncuestaMano", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Abdomen:
                    return RedirectToAction("EncuestaAbdomen", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.BrazoDerecho:
                case EncuestaType.AntebrazoDerecho:
                case EncuestaType.MusloDerecho:
                case EncuestaType.PiernaDerecho:
                case EncuestaType.BrazoIzquierdo:
                case EncuestaType.AntebrazoIzquierdo:
                case EncuestaType.MusloIzquierdo:
                case EncuestaType.PiernaIzquierdo:
                    return RedirectToAction("EncuestaExtremidad", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.MamaDerecha:
                case EncuestaType.MamaIzquierda:
                    return RedirectToAction("EncuestaMama", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Oncologico:
                    return RedirectToAction("EncuestaOncologio", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Colonoscopia:
                    return RedirectToAction("EncuestaColonoscopia", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.AngiotemCoronario:
                    return RedirectToAction("EncuestaAngioCoronario", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.AngiotemAorta:
                    return RedirectToAction("EncuestaAngioAorta", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.AngiotemCerebral:
                    return RedirectToAction("EncuestaTEMCerebro", new { examen = _exam.codigo, tipo = item.tipo_encuesta });
                //return RedirectToAction("EncuestaAngioCerebral", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.MusculoEsqueletico:
                    return RedirectToAction("EncuestaMusculoEsqueletico", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.NeuroCerebro:
                    return RedirectToAction("EncuestaNeuroCerebro", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.Generica:
                    return RedirectToAction("EncuestaGenericaTEM", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                case EncuestaType.EncuestaVacia:
                    return RedirectToAction("EncuestaVacia", new { examen = _exam.codigo, tipo = item.tipo_encuesta });
                case EncuestaType.Fibroscan:
                    return RedirectToAction("EncuestaFibroScan", new { examen = _exam.codigo, tipo = item.tipo_encuesta });

                default:
                    break;
            }
            #endregion


            return View(item);
        }
        /******************************************************************************************
         *      RM
        *******************************************************************************************/

        #region CEREBRO

        public ActionResult EncuestaCerebro(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {

                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaCerebroViewModel item = new EncuestaCerebroViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");

                if (db.HISTORIA_CLINICA_CEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio. ");
                }
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaCerebro(EncuestaCerebroViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();

                item._examen = _exam;
                item._paciente = _paci;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                try
                {
                    #region Conversion EntitiCerebro
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    ELCerebro item_ceb = new ELCerebro();
                    item_ceb.numeroexamen = item.numeroexamen;
                    item_ceb.usu_reg = user.ProviderUserKey.ToString();
                    item_ceb.fec_reg = DateTime.Now;

                    item_ceb.p1 = item.p1.Value;
                    if (item_ceb.p1)
                    {
                        item_ceb.p1aSet(
                            item.p1_1,
                             item.p1_2,
                             item.p1_3);
                        if (item.p1_3)
                        {
                            item_ceb.p1a31 = item.p1_4;
                        }
                    }

                    item_ceb.p2 = item.p2.Value;
                    if (item_ceb.p2)
                    {
                        item_ceb.p2a = item.p2_1.Value;
                    }

                    item_ceb.p3 = item.p3.Value;
                    if (item_ceb.p3)
                    {
                        item_ceb.p3a = item.p3_1.Value;
                        if (item_ceb.p3a)
                        {
                            item_ceb.p3a1 = item.p3_1_1.Value;
                            item_ceb.p3a2 = item.p3_1_2.Value;
                            item_ceb.p3a3Set(
                                item.p3_1_3,
                                item.p3_1_4,
                                item.p3_1_5);
                        }
                    }

                    item_ceb.p4 = item.p4.Value;
                    if (item_ceb.p4)
                    {
                        item_ceb.p4a = item.p4_1.Value;
                    }

                    item_ceb.p5 = item.p5.Value;
                    if (item_ceb.p5)
                    {
                        item_ceb.p5aSet(
                            item.p5_1,
                            item.p5_2,
                            item.p5_3,
                            item.p5_4);
                        if (item.p5_4)
                        {
                            item_ceb.p5a41 = item.p5_4_1;
                        }
                    }

                    item_ceb.p6 = item.p6.Value;
                    if (item_ceb.p6)
                    {
                        item_ceb.p6aSet(
                            item.p6_1,
                            item.p6_4,
                            item.p6_2,
                            item.p6_5,
                            item.p6_3,
                            item.p6_6,
                            item.p6_7);
                        if (item.p6_7)
                        {
                            item_ceb.p6a71 = item.p6_7_1;
                        }
                    }
                    item_ceb.p7 = item.p7; // siempre tendra aunqsea vacio
                    //PARTE - HISTORIA
                    item_ceb.p8Set(item.p8.Value.ToString() + "%", item.p8_1.ToString());

                    item_ceb.p9 = item.p9.Value;
                    item_ceb.p9a1 = item.p9_1;

                    item_ceb.p10 = item.p10.Value;
                    if (item_ceb.p10)
                    {
                        item_ceb.p10a71 = item.p10_1;
                    }

                    item_ceb.p11 = item.p11.Value;
                    if (item_ceb.p11)
                    {
                        item_ceb.p11a = item.p11_1.Value;
                        item_ceb.p11bSet(item.p11_2.Value.ToString() + "%", item.p11_3.ToString());
                        item_ceb.p11c = item.p11_4;
                    }

                    item_ceb.p12 = item.p12.Value;
                    if (item_ceb.p12)
                    {
                        //RESONANCIA
                        item_ceb.p12aa = item.p12_1;
                        if (item_ceb.p12aa)
                        {
                            item_ceb.p12a = item.p12_1_1.ToString();
                            if (item_ceb.p12a == "9")
                            {
                                item_ceb.p12a41Set(item.p12_1_2.Value);
                            }
                            item_ceb.p12bSet(
                                item.p12_1_3.ToString() + "%",
                                item.p12_1_4.ToString());
                            item_ceb.p12c = item.p12_1_5;
                        }
                        //TOMOGRAFIA
                        item_ceb.p12bb = item.p12_2;
                        if (item_ceb.p12bb)
                        {
                            item_ceb.p12d = item.p12_2_1.ToString();
                            if (item_ceb.p12d == "9")
                            {
                                item_ceb.p12d41Set(item.p12_2_2.Value);
                            }
                            item_ceb.p12eSet(
                                item.p12_2_3.ToString() + "%",
                                item.p12_2_4.ToString());
                            item_ceb.p12f = item.p12_2_5;
                        }
                    }

                    item_ceb.p13Acheck = item.p13A_1;
                    if (item_ceb.p13Acheck == false)
                        item_ceb.p13A = item.p13A;

                    item_ceb.p13Bcheck = item.p13B_1;
                    if (item_ceb.p13Bcheck == false)
                        item_ceb.p13B = item.p13B;

                    item_ceb.p14 = item.p14;

                    item_ceb.p15Set(
                        item.p15_1,
                        item.p15_2,
                        item.p15_3,
                        item.p15_4,
                        item.p15_5,
                        item.p15_6,
                        item.p15_7,
                        item.p15_8,
                        item.p15_9,
                        item.p15_10,
                        item.p15_11,
                        item.p15_12,
                        item.p15_13
                        );
                    if (item.p15_12)
                        item_ceb.p15a121 = item.p15_12_1;
                    if (item.p15_13)
                        item_ceb.p15a131 = item.p15_13_1;

                    item_ceb.p16Set(
                        item.p16_1,
                        item.p16_2,
                        item.p16_3,
                        item.p16_4,
                        item.p16_5,
                        item.p16_6,
                        item.p16_7,
                        item.p16_8,
                        item.p16_9,
                        item.p16_10
                        );
                    if (item.p16_10)
                        item_ceb.p16a101 = item.p16_10_1;

                    item_ceb.p17Set(item.p17.Value
                        );
                    if (item.p17.Value == 1)
                        item_ceb.p17_1 = item.p17_1;
                    else
                        item_ceb.p17_1 = 0;


                    #endregion

                    //verificar si esta operado
                    if (item.p11 != null)
                        _exam.isOperado = item.p11.Value;
                    //verificar si es Angio
                    if (item.p16_1)
                        _exam.isAngio = true;
                    if (item.p16_2)
                        _exam.isAngio = true;

                    db.HISTORIA_CLINICA_CEREBRO.Add(GetEntity(item_ceb));
                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        public HISTORIA_CLINICA_CEREBRO GetEntity(ELCerebro item)
        {
            var entidad = new HISTORIA_CLINICA_CEREBRO();
            entidad.numeroexamen = item.numeroexamen;
            entidad.usu_reg = item.usu_reg;
            entidad.fec_reg = item.fec_reg;

            entidad.p1 = item.p1;
            if (item.p1)
            {
                entidad.p1a = item.p1a;
                entidad.p1a31 = item.p1a31;
            }
            entidad.p2 = item.p2;
            if (item.p2)
            {
                entidad.p2a = item.p2a;
            }
            entidad.p3 = item.p3;
            if (item.p3)
            {
                entidad.p3a = item.p3a;
                if (item.p3a)
                {
                    entidad.p3a1 = item.p3a1;
                    entidad.p3a2 = item.p3a2;
                    entidad.p3a3 = item.p3a3;
                }
            }
            entidad.p4 = item.p4;
            if (item.p4)
            {
                entidad.p4a = item.p4;
            }
            entidad.p5 = item.p5;
            if (item.p5)
            {
                entidad.p5a = item.p5a;
                entidad.p5a41 = item.p5a41;
            }
            entidad.p6 = item.p6;
            if (item.p6)
            {
                entidad.p6a = item.p6a;
                entidad.p6a71 = item.p6a71;
            }
            entidad.p7 = item.p7;
            entidad.p8 = item.p8;
            entidad.p9 = item.p9;
            entidad.p9a1 = item.p9a1;
            entidad.p10 = item.p10;
            if (item.p10)
            {
                entidad.p10a = item.p10a;
                entidad.p10a71 = item.p10a71;
            }
            entidad.p11 = item.p11;
            if (item.p11)
            {
                entidad.p11a = item.p11a;
                entidad.p11b = item.p11b;
                entidad.p11c = item.p11c;
            }
            entidad.p12 = item.p12;
            if (item.p12)
            {
                entidad.p12aa = item.p12aa;
                if (item.p12aa)
                {
                    entidad.p12a = item.p12a;
                    entidad.p12a41 = item.p12a41;
                    entidad.p12b = item.p12b;
                    entidad.p12c = item.p12c;
                }
                entidad.p12bb = item.p12bb;
                if (item.p12bb)
                {
                    entidad.p12d = item.p12d;
                    entidad.p12d41 = item.p12d41;
                    entidad.p12e = item.p12e;
                    entidad.p12f = item.p12f;
                }
            }
            entidad.p13Acheck = item.p13Acheck;
            if (entidad.p13Acheck == false)
                entidad.p13A = item.p13A;
            entidad.p13Bcheck = item.p13Bcheck;
            if (entidad.p13Bcheck == false)
                entidad.p13B = item.p13B;

            entidad.p14 = item.p14;
            entidad.p15 = item.p15;
            entidad.p15a121 = item.p15a121;
            entidad.p15a131 = item.p15a131;

            entidad.p16 = item.p16;
            entidad.p16a101 = item.p16a101;
            entidad.p17 = item.p17;
            if (entidad.p17 == "1")
                entidad.p17_1 = item.p17_1;

            return entidad;
        }
        #endregion

        #region HOMBRO

        public ActionResult EncuestaHombro(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaHombroViewModel item = new EncuestaHombroViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_HOMBRO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaHombro(EncuestaHombroViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                try
                {
                    #region Cargar Hombro
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_HOMBRO encu = new HISTORIA_CLINICA_HOMBRO();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_5 = item.p1_5;
                        encu.p1_6 = item.p1_6;
                        encu.p1_7 = item.p1_7;
                        encu.p1_7_1 = item.p1_7_1;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2)
                    {
                        encu.p2_1 = item.p2_1;
                        encu.p2_21 = item.p2_21;
                        encu.p2_22 = item.p2_22;
                    }
                    encu.p3 = item.p3;
                    encu.p4_1 = item.p4_1;
                    encu.p4_2 = item.p4_2;
                    if (item.p4_3 != null)
                    {
                        encu.p4_3 = item.p4_3.Value;
                        encu.p4_3_1 = item.p4_3_1;
                    }

                    encu.p5 = item.p5.Value;
                    if (encu.p5.Value)
                    {
                        encu.p5_1 = item.p5_1;
                    }
                    encu.p6 = item.p6.Value;
                    if (encu.p6.Value)
                    {
                        encu.p6_1 = item.p6_1;
                    }
                    encu.p7 = item.p7;
                    encu.p8 = item.p8.Value;
                    if (encu.p8.Value)
                    {
                        encu.p8_1 = item.p8_1;
                        encu.p8_21 = item.p8_21;
                        encu.p8_22 = item.p8_22;
                        encu.p8_3 = item.p8_3;
                        encu.p8_4 = item.p8_4;
                    }
                    encu.p9 = item.p9.Value;
                    if (encu.p9.Value)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_21 = item.p9_21;
                        encu.p9_22 = item.p9_22;
                        encu.p9_3 = item.p9_3;
                    }

                    encu.p10 = item.p10.Value;
                    if (encu.p10.Value)
                    {
                        encu.p10_1 = item.p10_1;
                        if (encu.p10_1.Value == 9)
                        {
                            encu.p10_11 = item.p10_11;
                        }
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                    }
                    encu.p11 = item.p11.Value;
                    if (encu.p11.Value)
                    {
                        encu.p11_1 = item.p11_1;
                        if (encu.p11_1.Value == 6)
                        {
                            encu.p11_11 = item.p11_11;
                        }
                        encu.p11_2 = item.p11_2;
                    }

                    encu.p12Acheck = item.p12Acheck;
                    if (encu.p12Acheck == false)
                        encu.p12A = item.p12A;
                    encu.p12Bcheck = item.p12Bcheck;
                    if (encu.p12Bcheck == false)
                        encu.p12B = item.p12B;

                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    if (encu.p14 == 2)
                    {
                        encu.p14_1 = item.p14_1;
                    }

                    encu.p15 = item.p15;
                    db.HISTORIA_CLINICA_HOMBRO.Add(encu);
                    #endregion
                    //verificar si esta operado
                    if (item.p8 != null)
                        _exam.isOperado = item.p8.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        #endregion

        #region EXTREMIDAD
        public ActionResult EncuestaExtremidad(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaExtremidadesViewModel item = new EncuestaExtremidadesViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_EXTREMIDAD.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaExtremidad(EncuestaExtremidadesViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Extremidad
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_EXTREMIDAD encu = new HISTORIA_CLINICA_EXTREMIDAD();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_0 = item.p1_0;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_5 = item.p1_5;
                        encu.p1_1 = item.p1_1;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                    {
                        encu.p2_1 = item.p2_1;
                        encu.p2_2 = item.p2_2;
                        encu.p2_3 = item.p2_3;
                        encu.p2_4 = item.p2_4;
                        encu.p2_5 = item.p2_5;
                        encu.p2_5_1 = item.p2_5_1;
                    }

                    encu.p3 = item.p3.Value;
                    if (encu.p3.Value)
                    {
                        encu.p3_1 = item.p3_1;
                    }

                    encu.p4_1 = item.p4_1;
                    encu.p4_2 = item.p4_2;
                    if (encu.p4_1 == "")
                        encu.p4_3 = item.p4_3.Value;
                    encu.p4_3_1 = item.p4_3_1;

                    encu.p5 = item.p5;
                    if (encu.p5.Value)
                    {
                        encu.p5_1 = item.p5_1;
                    }

                    encu.p7 = item.p7;
                    encu.p9 = item.p9;
                    //encu.p9 = txt_p9_a_1.Text;

                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                        encu.p10_4 = item.p10_4;
                    }
                    encu.p11 = item.p11.Value;
                    if (encu.p11)
                    {
                        encu.p11_1 = item.p11_1;
                        if (encu.p11_1.Value == 6)
                        {
                            encu.p11_11 = item.p11_11;
                        }
                        encu.p11_21 = item.p11_21;
                        encu.p11_22 = item.p11_22;
                        encu.p11_3 = item.p11_3;
                    }
                    encu.p12 = item.p12.Value;
                    if (encu.p12)
                    {
                        encu.p12_1 = item.p12_1;
                        if (encu.p12_1.Value == 6)
                        {
                            encu.p12_2 = item.p12_2;
                        }
                        encu.p12_3 = item.p12_3;
                    }

                    encu.p13A_2 = item.p13A_2;
                    if (encu.p13A_2 == false)
                        encu.p13A_1 = item.p13A_1;

                    encu.p13B_2 = item.p13B_2;
                    if (encu.p13B_2 == false)
                        encu.p13B_1 = item.p13B_1;

                    encu.p14_1 = item.p14_1;
                    encu.p15 = item.p15;
                    if (encu.p15 == "2")
                    {
                        encu.p15_1 = item.p15_1;
                    }

                    encu.p16 = item.p16;

                    db.HISTORIA_CLINICA_EXTREMIDAD.Add(encu);
                    #endregion
                    //verificar si esta operado
                    if (item.p10 != null)
                        _exam.isOperado = item.p10.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region CODO
        public ActionResult EncuestaCodo(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaCodoViewModel item = new EncuestaCodoViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_CODO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaCodo(EncuestaCodoViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Codo
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_CODO encu = new HISTORIA_CLINICA_CODO();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_8 = item.p1_8;
                        if (encu.p1_8.Value)
                        {
                            encu.p1_8_1 = item.p1_8_1;
                        }
                    }
                    encu.p1_5 = item.p1_5;
                    if (encu.p1_5.Value)
                    {
                        encu.p1_51 = item.p1_51.Value;
                    }
                    encu.p1_6 = item.p1_6;
                    encu.p1_7 = item.p1_7;
                    if (encu.p1_7.Value)
                    {
                        encu.p1_71 = item.p1_71;
                    }

                    encu.p2_1 = item.p2_1 == null ? "" : item.p2_1;
                    encu.p2_2 = item.p2_2;
                    if (item.p2_3 != null)
                        encu.p2_3 = item.p2_3.Value;
                    encu.p2_3_1 = item.p2_3_1;

                    encu.p3 = item.p3;
                    if (encu.p3 != "3")
                    {
                        encu.p3_1 = item.p3_1;
                    }


                    encu.p4 = item.p4;
                    if (encu.p4 == "1")
                    {
                        encu.p4_1 = item.p4_1;
                    }
                    encu.p5 = item.p5;
                    encu.p6 = item.p6;
                    //encu.p7 = txt_p7_a_1.Text.Trim();


                    encu.p8 = item.p8.Value;
                    if (encu.p8)
                    {
                        encu.p8_1 = item.p8_1;
                        encu.p8_21 = item.p8_21;
                        encu.p8_22 = item.p8_22;
                        encu.p8_3 = item.p8_3;
                        encu.p8_4 = item.p8_4;
                    }

                    encu.p9 = item.p9.Value;
                    if (encu.p9)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_21 = item.p9_21.Value;
                        encu.p9_22 = item.p9_22;
                    }

                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        if (encu.p10_1 == 6)
                        {
                            encu.p10_11 = item.p10_11;
                        }
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                    }

                    encu.p11 = item.p11.Value;
                    if (encu.p11)
                    {
                        encu.p11_11 = item.p11_11;
                        if (encu.p11_11 == 6)
                        {
                            encu.p11_12 = item.p11_12;
                        }
                        encu.p11_2 = item.p11_2;
                    }

                    encu.p12A_2 = item.p12A_2;
                    if (encu.p12A_2 == false)
                        encu.p12A_1 = item.p12A_1;

                    encu.p12B_2 = item.p12B_2;
                    if (encu.p12B_2 == false)
                        encu.p12B_1 = item.p12B_1;

                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    if (encu.p14 == "2")
                        encu.p14_1 = item.p14_1;

                    encu.p15 = item.p15;

                    db.HISTORIA_CLINICA_CODO.Add(encu);
                    #endregion
                    //verificar si esta operado
                    if (item.p8 != null)
                        _exam.isOperado = item.p8.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region MAMA
        public ActionResult EncuestaMama(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaMamaViewModel item = new EncuestaMamaViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_MAMA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaMama(EncuestaMamaViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Mama
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_MAMA encu = new HISTORIA_CLINICA_MAMA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_5 = item.p1_5;
                        encu.p1_6 = item.p1_6;
                        //izquierda
                        encu.p1_1_1 = item.p1_1_1;
                        encu.p1_2_2 = item.p1_2_2;
                        encu.p1_3_1 = item.p1_3_1;
                        encu.p1_4_1 = item.p1_4_1;
                        encu.p1_5_1 = item.p1_5_1;
                        encu.p1_6_1 = item.p1_6_1;

                        encu.p1_0 = item.p1_0;
                    }
                    //2
                    encu.p2 = item.p2.Value;
                    encu.p2_1 = item.p2_1;
                    // 3
                    encu.p3 = item.p3.Value;
                    if (encu.p3.Value)
                        encu.p3_1 = item.p3_1;

                    // 4
                    encu.p4 = item.p4.Value;
                    if (encu.p4.Value)
                    {

                        encu.p4_1 = item.p4_1;
                        // B 
                        encu.p4_2 = item.p4_2;
                        encu.p4_2_1 = item.p4_2_1;
                        // C
                        encu.p4_3 = item.p4_3;
                        // D
                        encu.p4_4 = item.p4_4;
                    }

                    //5
                    encu.p5 = item.p5;

                    //6
                    encu.p6 = item.p6;

                    //7
                    encu.p7 = item.p7;

                    //8
                    encu.p8 = item.p8;

                    //9
                    encu.p9 = item.p9.Value;
                    if (encu.p9.Value)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_2 = item.p9_2;
                    }
                    //10

                    encu.ind_p1 = item.ind_p1;

                    encu.ind_p1_1 = item.ind_p1_1;

                    encu.p10 = item.p10;

                    db.HISTORIA_CLINICA_MAMA.Add(encu);
                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region ABDOMEN
        public ActionResult EncuestaAbdomen(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaAbdomenViewModel item = new EncuestaAbdomenViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_ABDOMEN.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");

                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaAbdomen(EncuestaAbdomenViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Abdomen
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_ABDOMEN encu = new HISTORIA_CLINICA_ABDOMEN();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        /*Donde*/
                        encu.p1_12 = item.p1_12;
                        encu.p1_13 = item.p1_13;
                        encu.p1_14 = item.p1_14;
                        encu.p1_15 = item.p1_15;
                        encu.p1_16 = item.p1_16;
                        encu.p1_16_1 = item.p1_16_1;

                        /*Caracteristica*/
                        /*Colicos*/
                        encu.p1_2 = item.p1_2;
                        encu.p1_2_1 = item.p1_2_1;
                        /*Constante*/
                        encu.p1_17 = item.p1_17;
                        /*Irradiado*/
                        encu.p1_1 = item.p1_1;
                        encu.p1_1_1 = item.p1_1_1;
                        /*Empeora con comidas*/
                        encu.p1_3 = item.p1_3;
                        encu.p1_3_1 = item.p1_3_1;
                        /*Otros*/
                        encu.p1_18 = item.p1_18;
                        encu.p1_18_1 = item.p1_18_1;
                    }

                    encu.p1_4 = item.p1_4;
                    /*Baha de Peso*/
                    encu.p1_5 = item.p1_5;
                    encu.p1_5_1 = item.p1_5_1;//cuanto?
                    encu.p1_5_2 = item.p1_5_2;//en cuanto?
                    encu.p1_6 = item.p1_6;
                    encu.p1_7 = item.p1_7;
                    encu.p1_8 = item.p1_8;
                    encu.p1_9 = item.p1_9;
                    encu.p1_10 = item.p1_10;
                    encu.p1_11 = item.p1_11;
                    encu.p1_11_1 = item.p1_11_1;

                    encu.p2_1 = item.p2_1 == null ? "" : item.p2_1;
                    encu.p2_2 = item.p2_2;
                    if (item.p2_3 != null)
                    {
                        encu.p2_3 = item.p2_3.Value;
                        encu.p2_3_1 = item.p2_3_1;
                    }


                    encu.p3_1 = item.p3_1;
                    encu.p4 = item.p4;
                    if (encu.p4 == "1")
                    {
                        encu.p4_1 = item.p4_1;
                    }
                    encu.p5 = item.p5;
                    encu.p6 = item.p6;
                    encu.p7 = item.p7;


                    encu.p8 = item.p8.Value;
                    if (encu.p8)
                    {
                        encu.p8_1 = item.p8_1;
                        encu.p8_4 = item.p8_4;
                    }

                    encu.p9 = item.p9.Value;
                    if (encu.p9)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_21 = item.p9_21;
                    }

                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        if (encu.p10_1 == 6)
                        {
                            encu.p10_11 = item.p10_11;
                        }
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                    }

                    encu.p11A_2 = item.p11A_2;
                    if (encu.p11A_2 == false)
                        encu.p11A_1 = item.p11A_1;

                    encu.p11B_2 = item.p11B_2;
                    if (encu.p11B_2 == false)
                        encu.p11B_1 = item.p11B_1;

                    encu.p12 = item.p12;
                    encu.p13 = item.p13;
                    if (encu.p13 == "2")
                        encu.p14_1 = item.p14_1;

                    encu.p15 = item.p15;

                    db.HISTORIA_CLINICA_ABDOMEN.Add(encu);
                    #endregion
                    //verificar si esta operado
                    if (item.p8 != null)
                        _exam.isOperado = item.p8.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region CADERA
        public ActionResult EncuestaCadera(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaCaderaViewModel item = new EncuestaCaderaViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_CADERA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaCadera(EncuestaCaderaViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Cadera
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_CADERA encu = new HISTORIA_CLINICA_CADERA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_7 = item.p1_7;
                        encu.p1_71 = item.p1_71;

                    }
                    encu.p1_4 = item.p1_4;
                    encu.p1_5 = item.p1_5;
                    encu.p1_6 = item.p1_6;
                    if (encu.p1_6.Value)
                    {
                        encu.p1_61 = item.p1_61;
                    }
                    encu.p2_1 = item.p2_1 == null ? "" : item.p2_1;
                    encu.p2_2 = item.p2_2;
                    if (item.p2_3 != null)
                    {
                        encu.p2_3 = item.p2_3.Value;
                        encu.p2_3_1 = item.p2_3_1;
                    }


                    encu.p3 = item.p3;
                    if (encu.p3 != "3")
                    {
                        encu.p3_1 = item.p3_1;
                    }

                    encu.p4 = item.p4;
                    if (encu.p4 == "1")
                    {
                        encu.p4_1 = item.p4_1;
                    }
                    encu.p5 = item.p5;
                    encu.p6 = item.p6;
                    encu.p7 = item.p7;


                    encu.p8 = item.p8.Value;
                    if (encu.p8)
                    {
                        encu.p8_1 = item.p8_1;
                        encu.p8_21 = item.p8_21;
                        encu.p8_22 = item.p8_22;
                        encu.p8_3 = item.p8_3;
                        encu.p8_4 = item.p8_4;
                    }

                    encu.p9 = item.p9.Value;
                    if (encu.p9)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_21 = item.p9_21;
                        encu.p9_22 = item.p9_22;
                    }

                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        if (encu.p10_1 == 6)
                        {
                            encu.p10_11 = item.p10_11;
                        }
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                    }

                    encu.p11 = item.p11.Value;
                    if (encu.p11)
                    {
                        encu.p11_11 = item.p11_11;
                        if (encu.p11_11 == 6)
                        {
                            encu.p11_12 = item.p11_12;
                        }
                        encu.p11_2 = item.p11_2;
                    }

                    encu.p12A_2 = item.p12A_2;
                    if (encu.p12A_2 == false)
                        encu.p12A_1 = item.p12A_1;

                    encu.p12B_2 = item.p12B_2;
                    if (encu.p12B_2 == false)
                        encu.p12B_1 = item.p12B_1;

                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    if (encu.p14 == "2")
                        encu.p14_1 = item.p14_1;

                    encu.p15 = item.p15;

                    db.HISTORIA_CLINICA_CADERA.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.p8 != null)
                        _exam.isOperado = item.p8.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region MANO
        public ActionResult EncuestaMano(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaManoViewModel item = new EncuestaManoViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_MANO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaMano(EncuestaManoViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Mano
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_MANO encu = new HISTORIA_CLINICA_MANO();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {

                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_5 = item.p1_5;
                        encu.p1_6 = item.p1_6;
                        encu.p1_7 = item.p1_7;
                        encu.p1_7_1 = item.p1_7_1;
                    }
                    /*limitacio funciopnal*/
                    encu.p1_1 = item.p1_1;
                    if (encu.p1_1.Value)
                    {
                        encu.p1_11 = item.p1_11;
                        encu.p1_12 = item.p1_12;
                        encu.p1_13 = item.p1_13;
                        encu.p1_14 = item.p1_14;
                        encu.p1_15 = item.p1_15;
                        encu.p1_16 = item.p1_16;
                        encu.p1_16_1 = item.p1_16_1;
                    }
                    /*otros*/
                    encu.p1_2 = item.p1_2;
                    if (encu.p1_2.Value)
                    {
                        encu.p1_21 = item.p1_21;
                    }
                    encu.p2 = item.p2.Value;
                    encu.p3 = item.p3.Value;
                    encu.p3_1 = item.p3_1;
                    encu.p4_1 = item.p4_1 == null ? "" : item.p4_1;
                    encu.p4_2 = item.p4_2;
                    if (item.p4_3 != null)
                    {
                        encu.p4_3 = item.p4_3.Value;
                        encu.p4_3_1 = item.p4_3_1;
                    }
                    encu.p5 = item.p5;
                    if (encu.p5 != "3")
                    {
                        encu.p5_1 = item.p5_1;
                    }

                    encu.p6 = item.p6;
                    if (encu.p6 == "1")
                    {
                        encu.p6_1 = item.p6_1;
                    }
                    encu.p7 = item.p7;


                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                        encu.p10_4 = item.p10_4;
                    }

                    encu.p11 = item.p11.Value;
                    if (encu.p11)
                    {
                        encu.p11_1 = item.p11_1;
                        encu.p11_21 = item.p11_21;
                        encu.p11_22 = item.p11_22;
                    }

                    encu.p12 = item.p12.Value;
                    if (encu.p12)
                    {
                        encu.p12_1 = item.p12_1;
                        if (encu.p12_1 == 6)
                        {
                            encu.p12_11 = item.p12_11;
                        }
                        encu.p12_21 = item.p12_21;
                        encu.p12_22 = item.p12_22;
                        encu.p12_3 = item.p12_3;
                    }

                    encu.p13 = item.p13.Value;
                    if (encu.p13)
                    {
                        encu.p13_11 = item.p13_11;
                        if (encu.p13_11 == 6)
                        {
                            encu.p13_12 = item.p13_12;
                        }
                        encu.p13_2 = item.p13_2;
                    }

                    encu.p14A_2 = item.p14A_2;
                    if (encu.p14A_2 == false)
                        encu.p14A_1 = item.p14A_1;

                    encu.p14B_2 = item.p14B_2;
                    if (encu.p14B_2 == false)
                        encu.p14B_1 = item.p14B_1;

                    encu.p15 = item.p15;
                    encu.p16 = item.p16;
                    if (encu.p16 == "2")
                        encu.p16_1 = item.p16_1;

                    encu.p17 = item.p17;

                    db.HISTORIA_CLINICA_MANO.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.p10 != null)
                        _exam.isOperado = item.p10.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region COLUMNA
        #region COLUMNA A
        public ActionResult EncuestaColumnaA(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaColumnaAViewModel item = new EncuestaColumnaAViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_COLUMNA_A.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);

            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaColumnaA(EncuestaColumnaAViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Columna A
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_COLUMNA_A encu = new HISTORIA_CLINICA_COLUMNA_A();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.mol_p1 = item.mol_p1.Value;
                    if (encu.mol_p1)
                    {
                        encu.mol_p1_1 = item.mol_p1_1;
                        encu.mol_p1_2 = item.mol_p1_2;
                        encu.mol_p1_3 = item.mol_p1_3;
                        encu.mol_p1_4 = item.mol_p1_4;
                        encu.mol_p1_5 = item.mol_p1_5;
                        encu.mol_p1_6 = item.mol_p1_6;
                        encu.mol_p1_7 = item.mol_p1_7;
                        encu.mol_p1_8 = item.mol_p1_8;
                        encu.mol_p1_9 = item.mol_p1_9;
                    }
                    // 2
                    encu.mol_p2 = item.mol_p2.Value;
                    if (encu.mol_p2)
                    {
                        encu.mol_p2_1 = item.mol_p2_1;
                        encu.mol_p2_2 = item.mol_p2_2;
                        encu.mol_p2_3 = item.mol_p2_3;
                        encu.mol_p2_4 = item.mol_p2_4;
                        encu.mol_p2_5 = item.mol_p2_5;
                        if (encu.mol_p2_5.Value)
                            encu.mol_p2_5_1 = item.mol_p2_5_1;
                        encu.mol_p2_6 = item.mol_p2_6;
                        if (encu.mol_p2_6.Value)
                            encu.mol_p2_6_1 = item.mol_p2_6_1;
                    }
                    // 3
                    encu.mol_p3_1 = item.mol_p3_1;
                    encu.mol_p3_2 = item.mol_p3_2;
                    encu.mol_p3_3 = item.mol_p3_3;
                    // 4
                    encu.mol_p4_1 = item.mol_p4_1;
                    encu.mol_p4_2 = item.mol_p4_2;
                    // 5
                    encu.mol_p5 = item.mol_p5.Value;
                    encu.mol_p5_1 = item.mol_p5_1;
                    // 6
                    encu.mol_p6 = item.mol_p6;
                    if (encu.mol_p6)
                    {
                        encu.mol_p6_1 = item.mol_p6_1;
                        encu.mol_p6_2 = item.mol_p6_2;
                        encu.mol_p6_3 = item.mol_p6_3;
                        encu.mol_p6_4 = item.mol_p6_4;
                    }
                    // 7
                    encu.mol_p7 = item.mol_p7;
                    // 8
                    encu.mol_p8 = item.mol_p8;
                    // PROCEDIMIENTOS RELACIONADOS 
                    // 1
                    encu.pro_p1 = item.pro_p1.Value;
                    if (encu.pro_p1)
                    {
                        // A
                        encu.pro_p1_1 = item.pro_p1_1;
                        // B 
                        encu.pro_p1_21 = item.pro_p1_21;
                        encu.pro_p1_22 = item.pro_p1_22;
                        // C
                        encu.pro_p1_3 = item.pro_p1_3;

                        // D
                        encu.pro_p1_4 = item.pro_p1_4;
                    }
                    // 2
                    encu.pro_p2 = item.pro_p2.Value;
                    if (encu.pro_p2)
                    {
                        // a
                        encu.pro_p2_1 = item.pro_p2_1;
                        if (encu.pro_p2_1.Value == 6)
                        {
                            encu.pro_p2_11 = item.pro_p2_11;
                        }
                        // b
                        encu.pro_p2_21 = item.pro_p2_21;
                        encu.pro_p2_22 = item.pro_p2_22;
                        // c
                        encu.pro_p2_3 = item.pro_p2_3;
                        // d
                        encu.pro_p2_4_1 = item.pro_p2_4_1;
                        encu.pro_p2_4_2 = item.pro_p2_4_2;
                    }
                    // PREGUNTAS A RESOLVER
                    // 1
                    encu.pre_p1Acheck = item.pre_p1Acheck;
                    if (encu.pre_p1Acheck == false)
                        encu.pre_p1A = item.pre_p1A;
                    // 2
                    encu.pre_p1Bcheck = item.pre_p1Bcheck;
                    if (encu.pre_p1Bcheck == false)
                        encu.pre_p1B = item.pre_p1B;
                    // 3
                    encu.pre_p2 = item.pre_p2;

                    // INDICACIONES
                    encu.ind_p1 = item.ind_p1;
                    if (encu.ind_p1 == "2")
                        encu.ind_p1_1 = item.ind_p1_1;

                    db.HISTORIA_CLINICA_COLUMNA_A.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.pro_p1 != null)
                        _exam.isOperado = item.pro_p1.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion
        #region COLUMNA B
        public ActionResult EncuestaColumnaB(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaColumnaBViewModel item = new EncuestaColumnaBViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_COLUMNA_B.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");

                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaColumnaB(EncuestaColumnaBViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Columna B
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_COLUMNA_B encu = new HISTORIA_CLINICA_COLUMNA_B();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.mol_p1 = item.mol_p1.Value;
                    if (encu.mol_p1)
                    {
                        encu.mol_p1_1 = item.mol_p1_1;
                        encu.mol_p1_2 = item.mol_p1_2;
                        encu.mol_p1_3 = item.mol_p1_3;
                        encu.mol_p1_4 = item.mol_p1_4;
                        encu.mol_p1_5 = item.mol_p1_5;
                        encu.mol_p1_6 = item.mol_p1_6;
                        encu.mol_p1_7 = item.mol_p1_7;
                        encu.mol_p1_7_1 = item.mol_p1_7_1;
                        encu.mol_p1_8 = item.mol_p1_8;
                        encu.mol_p1_8_1 = item.mol_p1_8_1;
                    }
                    // 2
                    encu.mol_p2 = item.mol_p2.Value;
                    if (encu.mol_p2)
                    {
                        encu.mol_p2_1 = item.mol_p2_1;
                        encu.mol_p2_2 = item.mol_p2_2;
                        encu.mol_p2_3 = item.mol_p2_3;
                        encu.mol_p2_4 = item.mol_p2_4;
                        encu.mol_p2_5 = item.mol_p2_5;
                        if (encu.mol_p2_5.Value)
                            encu.mol_p2_5_1 = item.mol_p2_5_1;
                        encu.mol_p2_6 = item.mol_p2_6;
                        if (encu.mol_p2_6.Value)
                            encu.mol_p2_6_1 = item.mol_p2_6_1;
                    }
                    // 3
                    encu.mol_p3_1 = item.mol_p3_1;
                    encu.mol_p3_2 = item.mol_p3_2;
                    encu.mol_p3_2_1 = item.mol_p3_2_1;
                    // 4
                    encu.mol_p4_1 = item.mol_p4_1;
                    encu.mol_p4_2 = item.mol_p4_2;
                    // 5
                    encu.mol_p5 = item.mol_p5.Value;
                    // 6
                    encu.mol_p6 = item.mol_p6;
                    if (encu.mol_p6)
                    {
                        encu.mol_p6_1 = item.mol_p6_1;
                        encu.mol_p6_2 = item.mol_p6_2;
                        encu.mol_p6_3 = item.mol_p6_3;
                        encu.mol_p6_4 = item.mol_p6_4;
                    }
                    // 7
                    encu.mol_p7 = item.mol_p7;
                    // 8
                    encu.mol_p8 = item.mol_p8;
                    // PROCEDIMIENTOS RELACIONADOS 
                    // 1
                    encu.pro_p1 = item.pro_p1.Value;
                    if (encu.pro_p1)
                    {
                        // A
                        encu.pro_p1_1 = item.pro_p1_1;
                        // B 
                        encu.pro_p1_21 = item.pro_p1_21;
                        encu.pro_p1_22 = item.pro_p1_22;
                        // C
                        encu.pro_p1_3 = item.pro_p1_3;

                        // D
                        encu.pro_p1_4 = item.pro_p1_4;
                    }
                    // 2
                    encu.pro_p2 = item.pro_p2.Value;
                    if (encu.pro_p2)
                    {
                        // a
                        encu.pro_p2_1 = item.pro_p2_1;
                        if (encu.pro_p2_1.Value == 6)
                        {
                            encu.pro_p2_11 = item.pro_p2_11;
                        }
                        // b
                        encu.pro_p2_21 = item.pro_p2_21;
                        encu.pro_p2_22 = item.pro_p2_22;
                        // c
                        encu.pro_p2_3 = item.pro_p2_3;
                        // d
                        encu.pro_p2_4_1 = item.pro_p2_4_1;
                        encu.pro_p2_4_2 = item.pro_p2_4_2;
                    }
                    // PREGUNTAS A RESOLVER
                    // 1
                    encu.pre_p1Acheck = item.pre_p1Acheck;
                    if (encu.pre_p1Acheck == false)
                        encu.pre_p1A = item.pre_p1A;
                    // 2
                    encu.pre_p1Bcheck = item.pre_p1Bcheck;
                    if (encu.pre_p1Bcheck == false)
                        encu.pre_p1B = item.pre_p1B;
                    // 3
                    encu.pre_p2 = item.pre_p2;

                    // INDICACIONES
                    encu.ind_p1 = item.ind_p1;
                    if (encu.ind_p1 == "2")
                        encu.ind_p1_1 = item.ind_p1_1;

                    db.HISTORIA_CLINICA_COLUMNA_B.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.pro_p1 != null)
                        _exam.isOperado = item.pro_p1.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion
        #region COLUMNA C
        public ActionResult EncuestaColumnaC(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaColumnaCViewModel item = new EncuestaColumnaCViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_COLUMNA_C.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaColumnaC(EncuestaColumnaCViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Columna C
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_COLUMNA_C encu = new HISTORIA_CLINICA_COLUMNA_C();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.mol_p1 = item.mol_p1.Value;
                    if (encu.mol_p1)
                    {
                        encu.mol_p1_1 = item.mol_p1_1;
                        encu.mol_p1_2 = item.mol_p1_2;
                        encu.mol_p1_3 = item.mol_p1_3;
                        if (encu.mol_p1_3.Value)
                            encu.mol_p1_3_1 = item.mol_p1_3_1;
                    }
                    // 2
                    encu.mol_p2 = item.mol_p2.Value;
                    if (encu.mol_p2)
                    {
                        encu.mol_p2_1 = item.mol_p2_1;
                        encu.mol_p2_2 = item.mol_p2_2;
                        if (encu.mol_p2_2.Value)
                            encu.mol_p2_2_1 = item.mol_p2_2_1;
                        encu.mol_p2_3 = item.mol_p2_3;
                        if (encu.mol_p2_3.Value)
                            encu.mol_p2_3_1 = item.mol_p2_3_1;
                    }
                    // 3
                    encu.mol_p3_1 = item.mol_p3_1;
                    // 4
                    encu.mol_p4_1 = item.mol_p4_1;
                    encu.mol_p4_2 = item.mol_p4_2;
                    // 5
                    encu.mol_p5 = item.mol_p5.Value;
                    // 6
                    encu.mol_p6 = item.mol_p6;
                    if (encu.mol_p6)
                    {
                        encu.mol_p6_1 = item.mol_p6_1;
                        encu.mol_p6_2 = item.mol_p6_2;
                        encu.mol_p6_3 = item.mol_p6_3;
                        encu.mol_p6_4 = item.mol_p6_4;
                    }
                    // 7
                    encu.mol_p7 = item.mol_p7;
                    // 8
                    encu.mol_p8 = item.mol_p8;
                    // PROCEDIMIENTOS RELACIONADOS 
                    // 1
                    encu.pro_p1 = item.pro_p1.Value;
                    if (encu.pro_p1)
                    {
                        // A
                        encu.pro_p1_1 = item.pro_p1_1;
                        // B 
                        encu.pro_p1_21 = item.pro_p1_21;
                        encu.pro_p1_22 = item.pro_p1_22;
                        // C
                        encu.pro_p1_3 = item.pro_p1_3;

                        // D
                        encu.pro_p1_4 = item.pro_p1_4;
                    }
                    // 2
                    encu.pro_p2 = item.pro_p2.Value;
                    if (encu.pro_p2)
                    {
                        // a
                        encu.pro_p2_1 = item.pro_p2_1;
                        if (encu.pro_p2_1.Value == 6)
                        {
                            encu.pro_p2_11 = item.pro_p2_11;
                        }
                        // b
                        encu.pro_p2_21 = item.pro_p2_21;
                        encu.pro_p2_22 = item.pro_p2_22;
                        // c
                        encu.pro_p2_3 = item.pro_p2_3;
                        // d
                        encu.pro_p2_4_1 = item.pro_p2_4_1;
                        encu.pro_p2_4_2 = item.pro_p2_4_2;
                    }
                    // PREGUNTAS A RESOLVER
                    // 1
                    encu.pre_p1Acheck = item.pre_p1Acheck;
                    if (encu.pre_p1Acheck == false)
                        encu.pre_p1A = item.pre_p1A;
                    // 2
                    encu.pre_p1Bcheck = item.pre_p1Bcheck;
                    if (encu.pre_p1Bcheck == false)
                        encu.pre_p1B = item.pre_p1B;
                    // 3
                    encu.pre_p2 = item.pre_p2;

                    // INDICACIONES
                    encu.ind_p1 = item.ind_p1;
                    if (encu.ind_p1 == "2")
                        encu.ind_p1_1 = item.ind_p1_1;

                    db.HISTORIA_CLINICA_COLUMNA_C.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.pro_p1 != null)
                        _exam.isOperado = item.pro_p1.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion
        #endregion

        #region RODILLA
        public ActionResult EncuestaRodilla(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaRodillaViewModel item = new EncuestaRodillaViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_RODILLA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaRodilla(EncuestaRodillaViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Rodilla
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_RODILLA encu = new HISTORIA_CLINICA_RODILLA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_4 = item.p1_4;
                        encu.p1_5 = item.p1_5;
                        encu.p1_6 = item.p1_6;
                        encu.p1_7 = item.p1_7;
                        encu.p1_8 = item.p1_8;
                        if (encu.p1_8.Value)
                            encu.p1_81 = item.p1_81;
                        encu.p1_9 = item.p1_9;
                    }

                    encu.p2 = item.p2.Value;
                    encu.p2_4 = item.p2_4;
                    encu.p2_5 = item.p2_5;
                    encu.p2_6 = item.p2_6;
                    encu.p2_7 = item.p2_7;
                    encu.p2_8 = item.p2_8;
                    if (encu.p2_8.Value)
                        encu.p2_81 = item.p2_81;
                    encu.p2_9 = item.p2_9;

                    encu.p3 = item.p3.Value;
                    encu.p4 = item.p4.Value;
                    encu.p5 = item.p5.Value;
                    encu.p6 = item.p6;

                    encu.p7_1 = item.p7_1 == null ? "" : item.p7_1;
                    encu.p7_11 = item.p7_11;
                    if (item.p7_2 != null)
                        encu.p7_2 = item.p7_2.Value;
                    encu.p7_22 = item.p7_22;


                    encu.p8 = item.p8;
                    if (encu.p8 != "3")
                        encu.p8_1 = item.p8_1;


                    encu.p9 = item.p9.Value;
                    if (encu.p9)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_21 = item.p9_21;
                        encu.p9_22 = item.p9_22;
                        encu.p9_3 = item.p9_3;
                        encu.p9_4 = item.p9_4;
                    }
                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        if (encu.p10_1 == 6)
                            encu.p10_11 = item.p10_11;
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                    }

                    encu.p11 = item.p11.Value;
                    encu.p11_1 = item.p11_1;
                    if (encu.p11_1.Value == 6)
                        encu.p11_11 = item.p11_11;
                    encu.p11_2 = item.p11_2;

                    encu.p12Acheck = item.p12Acheck;
                    if (encu.p12Acheck == false)
                        encu.p12A = item.p12A;

                    encu.p12Bcheck = item.p12Bcheck;
                    if (encu.p12Bcheck == false)
                        encu.p12B = item.p12B;

                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    if (encu.p14 == "2")
                        encu.p14_1 = item.p14_1;

                    encu.p15 = item.p15;

                    db.HISTORIA_CLINICA_RODILLA.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.p9 != null)
                        _exam.isOperado = item.p9.Value;
                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region PIE
        public ActionResult EncuestaPie(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaPieViewModel item = new EncuestaPieViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_PIE.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaPie(EncuestaPieViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Pie
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_PIE encu = new HISTORIA_CLINICA_PIE();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                        encu.p1_5 = item.p1_5;
                        encu.p1_4 = item.p1_4;
                        if (encu.p1_4.Value)
                        {
                            encu.p1_41 = item.p1_41;
                        }
                    }

                    encu.p2 = item.p2.Value;
                    encu.p3 = item.p3.Value;
                    encu.p3_1 = item.p3_1;

                    encu.p4_1 = item.p4_1 == null ? "" : item.p4_1;
                    encu.p4_2 = item.p4_2;
                    if (item.p4_3 != null)
                        encu.p4_3 = item.p4_3.Value;
                    encu.p4_3_1 = item.p4_3_1;

                    encu.p5 = item.p5;
                    if (encu.p5 != "3")
                    {
                        encu.p5_1 = item.p5_1;
                    }

                    encu.p6 = item.p6;
                    if (encu.p6 == "1")
                    {
                        encu.p6_1 = item.p6_1;
                    }
                    encu.p7 = item.p7;
                    encu.p8 = item.p8;
                    encu.p9 = item.p9;

                    encu.p10 = item.p10.Value;
                    if (encu.p10)
                    {
                        encu.p10_1 = item.p10_1;
                        encu.p10_21 = item.p10_21;
                        encu.p10_22 = item.p10_22;
                        encu.p10_3 = item.p10_3;
                        encu.p10_4 = item.p10_4;
                    }

                    encu.p11 = item.p11.Value;
                    if (encu.p11)
                    {
                        encu.p11_1 = item.p11_1;
                        encu.p11_21 = item.p11_21;
                        encu.p11_22 = item.p11_22;
                    }
                    //12
                    encu.p12 = item.p12.Value;
                    if (encu.p12)
                    {
                        encu.p12_1 = item.p12_1;
                        if (encu.p12_1 == 6)
                        {
                            encu.p12_11 = item.p12_11;
                        }
                        encu.p12_21 = item.p12_21;
                        encu.p12_22 = item.p12_22;
                        encu.p12_3 = item.p12_3;
                    }

                    encu.p13 = item.p13.Value;
                    if (encu.p13)
                    {
                        encu.p13_11 = item.p13_11;
                        if (encu.p13_11 == 6)
                        {
                            encu.p13_12 = item.p13_12;
                        }
                        encu.p13_2 = item.p13_2;
                    }

                    encu.p14A_2 = item.p14A_2;
                    if (encu.p14A_2 == false)
                        encu.p14A_1 = item.p14A_1;

                    encu.p14B_2 = item.p14B_2;
                    if (encu.p14B_2 == false)
                        encu.p14B_1 = item.p14B_1;

                    encu.p15 = item.p15;
                    encu.p16 = item.p16;
                    if (encu.p16 == "2")
                        encu.p16_1 = item.p16_1;

                    encu.p17 = item.p17;

                    db.HISTORIA_CLINICA_PIE.Add(encu);

                    #endregion
                    //verificar si esta operado
                    if (item.p10 != null)
                        _exam.isOperado = item.p10.Value;

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region ONCOLOGICO
        public ActionResult EncuestaOncologio(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaOncologicoViewModel item = new EncuestaOncologicoViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_ONCOLOGICO.Where(x => x.numeroestudio == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaOncologio(EncuestaOncologicoViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Oncologico
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_ONCOLOGICO encu = new HISTORIA_CLINICA_ONCOLOGICO();
                    encu.numeroestudio = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    encu.p1_1 = item.p1_1;

                    //2 diagnostico

                    encu.p2 = item.p2.Value;

                    encu.p2_1 = item.p2_1;
                    encu.p2_1_1 = item.p2_1_1;
                    encu.p2_1_2 = item.p2_1_2;
                    encu.p2_2 = item.p2_2;
                    encu.p2_3 = item.p2_3;

                    // 3 Motivo 
                    encu.p3 = item.p3;
                    encu.p3_1 = item.p3_1;
                    //4
                    encu.p4 = item.p4;
                    encu.p4_1 = item.p4_1;

                    //5
                    encu.p5 = item.p5;
                    //Histerectomia
                    encu.p5_1 = item.p5_1;
                    encu.p5_1_1 = item.p5_1_1;
                    encu.p5_1_2 = item.p5_1_2;
                    //Prostatectomia
                    encu.p5_2 = item.p5_2;
                    encu.p5_2_1 = item.p5_2_1;
                    encu.p5_2_2 = item.p5_2_2;
                    //Colecistectomia
                    encu.p5_3 = item.p5_3;
                    encu.p5_3_1 = item.p5_3_1;
                    encu.p5_3_2 = item.p5_3_2;
                    //Otra
                    encu.p5_4 = item.p5_4;

                    //6
                    encu.p6 = item.p6;
                    encu.p6_1 = item.p6_1;
                    encu.p6_2 = item.p6_2;
                    encu.p6_3 = item.p6_3;

                    //7
                    encu.p7 = item.p7;
                    encu.p7_1 = item.p7_1;
                    encu.p7_2 = item.p7_2;
                    encu.p7_3 = item.p7_3;

                    //8
                    encu.p8 = item.p8.Value;
                    if (encu.p8.Value)
                    {
                        encu.p8_1 = item.p8_1;
                        if (encu.p8_1 == 6)
                        {
                            encu.p8_2 = item.p8_1;
                        }
                        else
                        {
                            encu.p8_2 = 0;
                        }
                        encu.p8_3 = item.p8_3;
                        encu.p8_4 = item.p8_4;
                        encu.p8_5 = item.p8_5;
                    }

                    //9
                    encu.p9 = item.p9;
                    encu.p10 = item.p10;

                    db.HISTORIA_CLINICA_ONCOLOGICO.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region GENERICA RM (VACIA)
        public ActionResult EncuestaVacia(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaGenericaRMViewModel item = new EncuestaGenericaRMViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;


                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaVacia(EncuestaGenericaRMViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                try
                {

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion


        /*******************************************************************************************
         *                          FIBROSCAN
         * ****************************************************************************************/
        #region FIBROSCAN
        public ActionResult EncuestaFibroScan(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaFibroScanViewModel item = new EncuestaFibroScanViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                var _modalida = _exam.codigoestudio.Substring(3, 2);
                item.modalidad = _modalida == "01" ? "RM" : (_modalida == "02" ? "TEM" : (_modalida == "16" ? "FIB" : ""));
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_FIBROSCAN.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);

            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaFibroScan(EncuestaFibroScanViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar FIBROSCAN
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_FIBROSCAN encu = new HISTORIA_CLINICA_FIBROSCAN();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1;
                    // 2
                    encu.p2_1 = item.p2_1;
                    encu.p2_2 = item.p2_2;
                    encu.p2_3 = item.p2_3;
                    encu.p2_4 = item.p2_4;

                    // 3
                    encu.p3 = item.p3;

                    // 4
                    encu.p4 = item.p4;
                    // 5
                    encu.p5 = item.p5;
                    // 6
                    encu.p6 = item.p6;
                    // 7
                    encu.p7_1 = item.p7_1;
                    encu.p7_2 = item.p7_2;
                    encu.p7_3 = item.p7_3;
                    encu.p7_4 = item.p7_4;
                    encu.p7_4_1 = item.p7_4_1;
                    // 8
                    encu.p8_1 = item.p8_1;
                    encu.p8_2 = item.p8_2;
                    encu.p8_3 = item.p8_3;
                    encu.p8_4 = item.p8_4;

                    //9
                    encu.p9 = item.p9;                   
                    db.HISTORIA_CLINICA_FIBROSCAN.Add(encu);

                    #endregion                   
                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }    
        #endregion

        /*******************************************************************************************
         *      TEM
        *******************************************************************************************/



        #region COLONOSCOPIA
        public ActionResult EncuestaColonoscopia(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaColonoscopiaViewModel item = new EncuestaColonoscopiaViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaColonoscopia(EncuestaColonoscopiaViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Colonoscopia
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_COLONOSCOPIA encu = new HISTORIA_CLINICA_TEM_COLONOSCOPIA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                        encu.p2_1 = item.p2_1;

                    encu.p3_1 = item.p3_1;
                    encu.p3_2 = item.p3_2;
                    encu.p3_3 = item.p3_3;
                    encu.p3_4 = item.p3_4;
                    encu.p3_5 = item.p3_5;
                    encu.p3_6 = item.p3_6;
                    encu.p3_7 = item.p3_7;

                    encu.p4 = item.p4.Value;
                    if (encu.p4.Value)
                    {
                        encu.p4_1 = item.p4_1;
                        encu.p4_2 = item.p4_2;
                        encu.p4_3 = item.p4_3;
                    }

                    encu.p5 = item.p5.Value;
                    if (encu.p5.Value)
                    {
                        encu.p5_1 = item.p5_1;
                        encu.p5_2 = item.p5_2;
                        encu.p5_3 = item.p5_3;
                    }

                    encu.p6 = item.p6;
                    encu.p7 = item.p7;

                    encu.p8 = item.p8;

                    db.HISTORIA_CLINICA_TEM_COLONOSCOPIA.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region ANGIOTEM

        #region ANGIGOTEM CORONARIO
        public ActionResult EncuestaAngioCoronario(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaAngioCoronarioViewModel item = new EncuestaAngioCoronarioViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_ARTERIAS.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaAngioCoronario(EncuestaAngioCoronarioViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Coronario
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_ARTERIAS encu = new HISTORIA_CLINICA_TEM_ARTERIAS();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                        encu.p2_1 = item.p2_1;

                    encu.p3 = item.p3.Value;
                    encu.p3_1 = item.p3_1;

                    encu.p4 = item.p4.Value;
                    encu.p4_1 = item.p4_1;

                    encu.p5 = item.p5.Value;
                    encu.p5_1 = item.p5_1;

                    encu.p6 = item.p6.Value;
                    encu.p6_1 = item.p6_1;


                    encu.p7_1 = item.p7_1;
                    encu.p7_2 = item.p7_2;
                    encu.p7_3 = item.p7_3;
                    encu.p7_4 = item.p7_4;


                    encu.p8 = item.p8;
                    encu.p9 = item.p9;
                    encu.p10 = item.p10;
                    encu.p11 = item.p11;
                    encu.p12 = item.p12;

                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    encu.p15 = item.p15;
                    encu.p16 = item.p16;

                    db.HISTORIA_CLINICA_TEM_ARTERIAS.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region ANGIGOTEM AORTA
        public ActionResult EncuestaAngioAorta(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaAngioAortaViewModel item = new EncuestaAngioAortaViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_AORTA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaAngioAorta(EncuestaAngioAortaViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");

                try
                {
                    #region Cargar Colonoscopia
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_AORTA encu = new HISTORIA_CLINICA_TEM_AORTA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    encu.p2_1 = item.p2_1;

                    encu.p3 = item.p3.Value;
                    encu.p3_1 = item.p3_1;

                    encu.p4 = item.p4.Value;
                    encu.p4_1 = item.p4_1;

                    /*encu.p5_1 = item.p5_1;
                    encu.p5_2 = item.p5_2;*/
                    encu.p5_3 = item.p5_3;

                    encu.p6 = item.p6;
                    encu.p7_1 = item.p7_1;
                    if (encu.p7_1 == false)
                        encu.p7 = item.p7;

                    encu.p8_1 = item.p8_1;
                    if (encu.p8_1 == false)
                        encu.p8 = item.p8;

                    encu.p9 = item.p9;
                    encu.p10 = item.p10;

                    encu.p11_1 = item.p11_1;
                    encu.p11_2 = item.p11_2;
                    encu.p11_3 = item.p11_3;
                    encu.p11_4 = item.p11_4;
                    encu.p11_5 = item.p11_5;
                    encu.p11_6 = item.p11_6;
                    encu.p12 = item.p12;
                    //if (encu.p12 == "2")
                    encu.p12_1 = item.p12_1;

                    db.HISTORIA_CLINICA_TEM_AORTA.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region ANGIGOTEM CEREBRAL(Deprecate)
        //public ActionResult EncuestaAngioCerebral(int examen, int tipo)
        //{
        //    var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
        //    var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
        //    if (_encu != null)
        //    {
        //        var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
        //        EncuestaAngioCerebralViewModel item = new EncuestaAngioCerebralViewModel();
        //        item._examen = _exam;
        //        item._paciente = _paci;
        //        item.numeroexamen = _exam.codigo;
        //        item.equipoAsignado = 0;
        //        item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
        //        item.sexo = _paci.sexo;
        //        item.tipo_encu = tipo;

        //        ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
        //        ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
        //        ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
        //        ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
        //        ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
        //        if (db.HISTORIA_CLINICA_TEM_.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
        //            ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
        //        return View(item);


        //    }
        //    else
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EncuestaAngioCerebral(EncuestaAngioCerebralViewModel item)
        //{
        //    var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
        //    var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
        //    if (_encu != null)
        //    {
        //        var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
        //        item._examen = _exam;
        //        item._paciente = _paci;
        //        item.numeroexamen = _exam.codigo;
        //        item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
        //        item.sexo = _paci.sexo;

        //        ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
        //        ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
        //        ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
        //        ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
        //        ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

        //        try
        //        {
        //            #region Cargar Coronario
        //            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
        //            HISTORIA_CLINICA_TEM_CEREBRAL encu = new HISTORIA_CLINICA_TEM_CEREBRAL();
        //            encu.numeroexamen = item.numeroexamen;
        //            encu.usu_reg = user.ProviderUserKey.ToString();
        //            encu.fec_reg = DateTime.Now;

        //            encu.p1 = item.p1.Value;
        //            if (encu.p1.Value)
        //            {
        //                encu.p1_1 = item.p1_1;
        //                encu.p1_2 = item.p1_2;
        //                encu.p1_3 = item.p1_3;
        //            }

        //            encu.p2 = item.p2.Value;
        //            if (encu.p2.Value)
        //                encu.p2_1 = item.p2_1;

        //            encu.p3 = item.p3.Value;
        //            if (encu.p3.Value)
        //            {
        //                encu.p3_1 = item.p3_1;
        //                encu.p3_2 = item.p3_2;
        //            }

        //            encu.p4 = item.p4.Value;
        //            if (encu.p4.Value)
        //            {
        //                encu.p4_1 = item.p4_1;
        //                encu.p4_2 = item.p4_2;
        //            }

        //            encu.p5_1 = item.p5_1;
        //            encu.p5_2 = item.p5_2;
        //            encu.p5_3 = item.p5_3;
        //            encu.p5_4 = item.p5_4;

        //            encu.p6 = "";
        //            encu.p7 = item.p7;
        //            encu.p8 = item.p8;
        //            encu.p9 = item.p9;
        //            encu.p10 = item.p10;
        //            encu.p11 = 0;

        //            db.HISTORIA_CLINICA_TEM_CEREBRAL.Add(encu);

        //            #endregion

        //            _exam.equipoAsignado = item.equipoAsignado;
        //            _encu.fec_paso3 = DateTime.Now;
        //            _encu.estado = 1;
        //            db.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            return View(item);
        //        }
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //    }
        //    else
        //        return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        //}
        #endregion

        #region ANGIOTEM CEREBRO

        public ActionResult EncuestaTEMCerebro(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaTEMCerebroViewModel item = new EncuestaTEMCerebroViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_CEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaTEMCerebro(EncuestaTEMCerebroViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();

                item._examen = _exam;
                item._paciente = _paci;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                try
                {
                    #region Conversion EntitiCerebro
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    ELCerebroTEM item_ceb = new ELCerebroTEM();
                    item_ceb.numeroexamen = item.numeroexamen;
                    item_ceb.usu_reg = user.ProviderUserKey.ToString();
                    item_ceb.fec_reg = DateTime.Now;

                    item_ceb.p1 = item.p1.Value;
                    if (item_ceb.p1)
                    {
                        item_ceb.p1aSet(
                            item.p1_1,
                             item.p1_2,
                             item.p1_3);
                        if (item.p1_3)
                        {
                            item_ceb.p1a31 = item.p1_4;
                        }
                    }

                    item_ceb.p2 = item.p2.Value;
                    if (item_ceb.p2)
                    {
                        item_ceb.p2a = item.p2_1.Value;
                    }

                    item_ceb.p3 = item.p3.Value;
                    if (item_ceb.p3)
                    {
                        item_ceb.p3a = item.p3_1.Value;
                        if (item_ceb.p3a)
                        {
                            item_ceb.p3a1 = item.p3_1_1.Value;
                            item_ceb.p3a2 = item.p3_1_2.Value;
                            item_ceb.p3a3Set(
                                item.p3_1_3,
                                item.p3_1_4,
                                item.p3_1_5);
                        }
                    }

                    item_ceb.p4 = item.p4.Value;
                    if (item_ceb.p4)
                    {
                        item_ceb.p4a = item.p4_1.Value;
                    }

                    item_ceb.p5 = item.p5.Value;
                    if (item_ceb.p5)
                    {
                        item_ceb.p5aSet(
                            item.p5_1,
                            item.p5_2,
                            item.p5_3,
                            item.p5_4);
                        if (item.p5_4)
                        {
                            item_ceb.p5a41 = item.p5_4_1;
                        }
                    }

                    item_ceb.p6 = item.p6.Value;
                    if (item_ceb.p6)
                    {
                        item_ceb.p6aSet(
                            item.p6_1,
                            item.p6_4,
                            item.p6_2,
                            item.p6_5,
                            item.p6_3,
                            item.p6_6,
                            item.p6_7);
                        if (item.p6_7)
                        {
                            item_ceb.p6a71 = item.p6_7_1;
                        }
                    }
                    item_ceb.p7 = item.p7; // siempre tendra aunqsea vacio
                    //PARTE - HISTORIA
                    item_ceb.p8Set(item.p8.Value.ToString() + "%", item.p8_1.ToString());

                    item_ceb.p9 = item.p9.Value;
                    item_ceb.p9a1 = item.p9_1;

                    item_ceb.p10 = item.p10.Value;
                    if (item_ceb.p10)
                    {
                        item_ceb.p10a71 = item.p10_1;
                    }

                    item_ceb.p11 = item.p11.Value;
                    if (item_ceb.p11)
                    {
                        item_ceb.p11a = item.p11_1.Value;
                        item_ceb.p11bSet(item.p11_2.Value.ToString() + "%", item.p11_3.ToString());
                        item_ceb.p11c = item.p11_4;
                    }

                    item_ceb.p12 = item.p12.Value;
                    if (item_ceb.p12)
                    {
                        //RESONANCIA
                        item_ceb.p12aa = item.p12_1;
                        if (item_ceb.p12aa)
                        {
                            item_ceb.p12a = item.p12_1_1.ToString();
                            if (item_ceb.p12a == "9")
                            {
                                item_ceb.p12a41Set(item.p12_1_2.Value);
                            }
                            item_ceb.p12bSet(
                                item.p12_1_3.ToString() + "%",
                                item.p12_1_4.ToString());
                            item_ceb.p12c = item.p12_1_5;
                        }
                        //TOMOGRAFIA
                        item_ceb.p12bb = item.p12_2;
                        if (item_ceb.p12bb)
                        {
                            item_ceb.p12d = item.p12_2_1.ToString();
                            if (item_ceb.p12d == "9")
                            {
                                item_ceb.p12d41Set(item.p12_2_2.Value);
                            }
                            item_ceb.p12eSet(
                                item.p12_2_3.ToString() + "%",
                                item.p12_2_4.ToString());
                            item_ceb.p12f = item.p12_2_5;
                        }
                    }

                    item_ceb.p13Acheck = item.p13A_1;
                    if (item_ceb.p13Acheck == false)
                        item_ceb.p13A = item.p13A;

                    item_ceb.p13Bcheck = item.p13B_1;
                    if (item_ceb.p13Bcheck == false)
                        item_ceb.p13B = item.p13B;

                    item_ceb.p14 = item.p14;


                    item_ceb.p15_1 = item.p15_1;
                    item_ceb.p15_2 = item.p15_2;
                    item_ceb.p15_3 = item.p15_3;
                    item_ceb.p15_4 = item.p15_4;
                    item_ceb.p15_5 = item.p15_5;




                    item_ceb.p16Set(item.p16);
                    if (item.p16 == "1")
                        item.p16_1 = item.p16_1;
                    else
                        item.p16_1 = 0;


                    #endregion

                    db.HISTORIA_CLINICA_TEM_CEREBRO.Add(GetEntity_TEM(item_ceb));
                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        public HISTORIA_CLINICA_TEM_CEREBRO GetEntity_TEM(ELCerebroTEM item)
        {
            var entidad = new HISTORIA_CLINICA_TEM_CEREBRO();
            entidad.numeroexamen = item.numeroexamen;
            entidad.usu_reg = item.usu_reg;
            entidad.fec_reg = item.fec_reg;

            entidad.p1 = item.p1;
            if (item.p1)
            {
                entidad.p1a = item.p1a;
                entidad.p1a31 = item.p1a31;
            }
            entidad.p2 = item.p2;
            if (item.p2)
            {
                entidad.p2a = item.p2a;
            }
            entidad.p3 = item.p3;
            if (item.p3)
            {
                entidad.p3a = item.p3a;
                if (item.p3a)
                {
                    entidad.p3a1 = item.p3a1;
                    entidad.p3a2 = item.p3a2;
                    entidad.p3a3 = item.p3a3;
                }
            }
            entidad.p4 = item.p4;
            if (item.p4)
            {
                entidad.p4a = item.p4;
            }
            entidad.p5 = item.p5;
            if (item.p5)
            {
                entidad.p5a = item.p5a;
                entidad.p5a41 = item.p5a41;
            }
            entidad.p6 = item.p6;
            if (item.p6)
            {
                entidad.p6a = item.p6a;
                entidad.p6a71 = item.p6a71;
            }
            entidad.p7 = item.p7;
            entidad.p8 = item.p8;
            entidad.p9 = item.p9;
            entidad.p9a1 = item.p9a1;
            entidad.p10 = item.p10;
            if (item.p10)
            {
                entidad.p10a = item.p10a;
                entidad.p10a71 = item.p10a71;
            }
            entidad.p11 = item.p11;
            if (item.p11)
            {
                entidad.p11a = item.p11a;
                entidad.p11b = item.p11b;
                entidad.p11c = item.p11c;
            }
            entidad.p12 = item.p12;
            if (item.p12)
            {
                entidad.p12aa = item.p12aa;
                if (item.p12aa)
                {
                    entidad.p12a = item.p12a;
                    entidad.p12a41 = item.p12a41;
                    entidad.p12b = item.p12b;
                    entidad.p12c = item.p12c;
                }
                entidad.p12bb = item.p12bb;
                if (item.p12bb)
                {
                    entidad.p12d = item.p12d;
                    entidad.p12d41 = item.p12d41;
                    entidad.p12e = item.p12e;
                    entidad.p12f = item.p12f;
                }
            }
            entidad.p13Acheck = item.p13Acheck;
            if (entidad.p13Acheck == false)
                entidad.p13A = item.p13A;
            entidad.p13Bcheck = item.p13Bcheck;
            if (entidad.p13Bcheck == false)
                entidad.p13B = item.p13B;

            entidad.p14 = item.p14;
            entidad.p15_1 = item.p15_1;
            entidad.p15_2 = item.p15_2;
            entidad.p15_3 = item.p15_3;
            entidad.p15_4 = item.p15_4;
            entidad.p15_5 = item.p15_5;


            entidad.p16 = item.p16;
            if (entidad.p16 == "1")
                entidad.p16_1 = item.p16_1;

            return entidad;
        }
        #endregion

        #endregion

        #region MUSCULO ESQUELETICO
        public ActionResult EncuestaMusculoEsqueletico(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaMusculoEsqueleticoViewModel item = new EncuestaMusculoEsqueleticoViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_MUSCULO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaMusculoEsqueletico(EncuestaMusculoEsqueleticoViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Musculo Esqueletico
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_MUSCULO encu = new HISTORIA_CLINICA_TEM_MUSCULO();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                        encu.p2_1 = item.p2_1;

                    encu.p3_1 = item.p3_1;
                    encu.p3_2 = item.p3_2;

                    encu.p4 = item.p4;
                    if (encu.p4 != 3)
                    {
                        encu.p4_1 = item.p4_1;
                    }

                    encu.p5 = item.p5.Value;
                    if (encu.p5.Value)
                    {
                        encu.p5_1 = item.p5_1;
                    }
                    encu.p6 = item.p6;

                    encu.p7 = item.p7.Value;
                    if (encu.p7.Value)
                    {
                        encu.p7_1 = item.p7_1;
                        encu.p7_2 = item.p7_2;
                        encu.p7_3 = item.p7_3;

                    }

                    encu.p8 = item.p8.Value;
                    if (encu.p8.Value)
                    {
                        encu.p8_1 = item.p8_1 + "%" + item.p8_1_1;
                        encu.p8_2 = item.p8_2 + "%" + item.p8_2_1;

                    }

                    encu.p9 = item.p9.Value;
                    if (encu.p9.Value)
                    {
                        encu.p9_1 = item.p9_1;
                        encu.p9_3 = item.p9_3;
                    }

                    encu.p10_1 = item.p10_1;
                    if (encu.p10_1 == false)
                        encu.p10 = item.p10;

                    encu.p11_1 = item.p11_1;
                    if (encu.p11_1 == false)
                        encu.p11 = item.p11;

                    encu.p12 = item.p12;
                    encu.p13 = item.p13;

                    encu.p14 = item.p14;

                    db.HISTORIA_CLINICA_TEM_MUSCULO.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region NEURO CEREBRO
        public ActionResult EncuestaNeuroCerebro(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaNeuroCerebroViewModel item = new EncuestaNeuroCerebroViewModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaNeuroCerebro(EncuestaNeuroCerebroViewModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Colonoscopia
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_NEUROCEREBRO encu = new HISTORIA_CLINICA_TEM_NEUROCEREBRO();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                        encu.p2_1 = item.p2_1;

                    encu.p3 = item.p3.Value;
                    if (encu.p3.Value)
                    {
                        encu.p3_2 = item.p3_2;
                    }

                    encu.p4 = item.p4.Value;
                    if (encu.p4.Value)
                    {
                        encu.p4_1 = item.p4_1;

                    }

                    encu.p5 = item.p5;

                    encu.p6 = item.p6.Value;
                    if (encu.p6.Value)
                        encu.p6_1 = item.p6_1;

                    //7

                    encu.p7_1 = item.p7_1;
                    encu.p7_2 = item.p7_2;
                    encu.p7_3 = item.p7_3;
                    encu.p7_4 = item.p7_4;
                    encu.p7_5 = item.p7_5;

                    encu.p8 = item.p8;
                    encu.p9 = item.p9;
                    encu.p10 = item.p10;
                    encu.p11 = item.p11;
                    encu.p12 = item.p12;
                    encu.p13 = item.p13;
                    encu.p14 = item.p14;
                    encu.p15 = item.p15;
                    encu.p16 = item.p16;

                    encu.p17_1 = item.p17_1;
                    encu.p17_2 = item.p17_2;
                    encu.p17_3 = item.p17_3;
                    encu.p17_4 = item.p17_4;
                    encu.p17_5 = item.p17_5;

                    db.HISTORIA_CLINICA_TEM_NEUROCEREBRO.Add(encu);

                    #endregion

                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion

        #region GENERICA TEM
        public ActionResult EncuestaGenericaTEM(int examen, int tipo)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == examen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                EncuestaGenericaTEMModel item = new EncuestaGenericaTEMModel();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.equipoAsignado = 0;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;
                item.tipo_encu = tipo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");
                if (db.HISTORIA_CLINICA_TEM_GENERICA.Where(x => x.numeroexamen == examen).SingleOrDefault() != null)
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                return View(item);


            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EncuestaGenericaTEM(EncuestaGenericaTEMModel item)
        {
            var _exam = db.EXAMENXATENCION.Where(x => x.codigo == item.numeroexamen).SingleOrDefault();
            var _encu = db.Encuesta.Where(x => x.numeroexamen == item.numeroexamen).SingleOrDefault();
            if (_encu != null)
            {
                var _paci = db.PACIENTE.Where(x => x.codigopaciente == _exam.codigopaciente).SingleOrDefault();
                item._examen = _exam;
                item._paciente = _paci;
                item.numeroexamen = _exam.codigo;
                item.modalidad = _exam.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
                item.sexo = _paci.sexo;

                ViewBag.tiempo = new SelectList((new Variable()).getTiempoRM(), "codigo", "nombre");
                ViewBag.locales = new SelectList((new Variable()).getLocales(), "codigo", "nombre");
                ViewBag.contraste = new SelectList((new Variable()).getContraste(), "codigo", "nombre");
                ViewBag.deporte = new SelectList((new Variable()).getDeportes(), "codigo", "nombre");
                ViewBag.regularidad = new SelectList((new Variable()).getReguralidadDeporte(), "codigo", "nombre");

                try
                {
                    #region Cargar Generica
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    HISTORIA_CLINICA_TEM_GENERICA encu = new HISTORIA_CLINICA_TEM_GENERICA();
                    encu.numeroexamen = item.numeroexamen;
                    encu.usu_reg = user.ProviderUserKey.ToString();
                    encu.fec_reg = DateTime.Now;

                    encu.p1 = item.p1.Value;
                    if (encu.p1.Value)
                    {
                        encu.p1_1 = item.p1_1;
                        encu.p1_2 = item.p1_2;
                        encu.p1_3 = item.p1_3;
                    }

                    encu.p2 = item.p2.Value;
                    if (encu.p2.Value)
                        encu.p2_1 = item.p2_1;

                    encu.p3 = item.p3.Value;
                    if (encu.p3.Value)
                    {
                        encu.p3_2 = item.p3_2;
                    }

                    encu.p4 = item.p4.Value;
                    if (encu.p4.Value)
                    {
                        encu.p4_1 = item.p4_1;

                    }

                    encu.p5 = item.p5;

                    encu.p6 = item.p6;
                    if (encu.p6.Value)
                        encu.p6_1 = item.p6_1;

                    //7
                    encu.p7_1 = item.p7_1;
                    encu.p7_2 = item.p7_2;
                    encu.p7_3 = item.p7_3;
                    encu.p7_4 = item.p7_4;
                    encu.p7_5 = item.p7_5;
                    encu.p7_6 = item.p7_6;
                    encu.p7_7 = item.p7_7;
                    encu.p7_8 = item.p7_8;
                    encu.p7_9 = item.p7_9;
                    encu.p7_10 = item.p7_10;


                    encu.p8 = item.p8;


                    encu.p9_1 = item.p9_1;
                    encu.p9_2 = item.p9_2;
                    encu.p9_3 = item.p9_3;
                    encu.p9_4 = item.p9_4;
                    encu.p9_5 = item.p9_5;
                    encu.p9_6 = item.p9_6;
                    encu.p9_7 = item.p9_7;
                    encu.p9_8 = item.p9_8;
                    encu.p9_9 = item.p9_9;
                    encu.p9_10 = item.p9_10;
                    encu.p9_11 = item.p9_11;
                    encu.p9_12 = item.p9_12;
                    encu.p10A_1 = item.p10A_1;
                    encu.p10A_2 = item.p10A_2;
                    encu.p10B_1 = item.p10B_1;
                    encu.p10B_2 = item.p10B_2;
                    encu.p11 = item.p11;
                    encu.p12 = item.p12;
                    encu.p12_1 = item.p12_1;

                    db.HISTORIA_CLINICA_TEM_GENERICA.Add(encu);

                    #endregion
                    _exam.equipoAsignado = item.equipoAsignado;
                    _encu.fec_paso3 = DateTime.Now;
                    _encu.estado = 1;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Ya se registró una encuesta para estudio anteriormente. ");
                    return View(item);
                }
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
            }
            else
                return RedirectToAction("ListaEspera", new { sede = _exam.codigoestudio.Substring(0, 3) });
        }
        #endregion
    }


}
