using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Tecnologo;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Tecnologo
{
    [Authorize(Roles = "3")]
    public class RealizarExamenController : Controller
    {

        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        // GET: /Lista Realizar Examen de pacientes/


        public ActionResult ListaEspera()
        {

            return View();
        }

        public ActionResult ListaExamen()
        {
            return View();
        }
        //lista de examenes pendiestes
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var allCompanies = getExamenes();
            IEnumerable<RealizarExamenViewModel> filteredCompanies;

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
                               isequipoSearchable && c.nom_equipo_pro.ToLower() == (param.sSearch.ToLower())
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
            Func<RealizarExamenViewModel, string> orderingFunction = (c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                                                           sortColumnIndex == 1 && ispacienteSortable ? c.nom_paciente :
                                                           sortColumnIndex == 2 && ishcitaSortable ? c.hora_cita.ToString() :
                                                           sortColumnIndex == 3 && ishadminSortable ? c.hora_admision.ToString() :
                                                           sortColumnIndex == 4 && isminSortable ? c.min_transcurri :
                                                           sortColumnIndex == 5 && iscondicionSortable ? c.condicion :
                                                           sortColumnIndex == 6 && isestudioSortable ? c.nom_estudio :
                                                           sortColumnIndex == 7 && isequipoSortable ? c.nom_equipo_pro :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredCompanies = filteredCompanies.OrderBy(orderingFunction);
            else
                filteredCompanies = filteredCompanies.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             "",//0
                Convert.ToString(c.num_examen),//1
                c.nom_paciente,//2
                (c.hora_cita).ToShortTimeString(),//3
                Convert.ToString(c.hora_admision),//4
                Convert.ToString(c.min_transcurri),//5
                c.condicion,//6
                c.nom_estudio,//7
                c.nom_equipo_pro,//8
                Convert.ToString(c.num_examen),//9
                Convert.ToString(c.isVIP),//10
                Convert.ToString(c.isValidado),//11
                Convert.ToString(c.isValidacion),//12
                Convert.ToString(c.isSedacion ),//13
            Convert.ToString(c.isIniciado),//14
            Convert.ToString(c.isActivado),//15
            Convert.ToString(c.minrestante),//16
            Convert.ToString(c.numeroatencion),//17
            Convert.ToString(c.isContraste),
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

        //lista de equipos asignados por sucursal
        public ActionResult getEquipo(string tipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            return Json(new SelectList((from eq in db.EQUIPO
                                        where eq.estado == "1" &&
                                        (user.sucursales_int).Contains(eq.codigounidad2 * 100 + eq.codigosucursal2) //sucursales asignadas

                                        select eq).AsQueryable(), "codigoequipo", "nombreequipo"), JsonRequestBehavior.AllowGet);

        }

        //metodo que obtiene la data
        private IList<RealizarExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Variable _u = new Variable();
            List<RealizarExamenViewModel> allCompanies = new List<RealizarExamenViewModel>();
            string queryString = @"select 
ea.codigo,p.apellidos+', '+p.nombres paciente,ea.horaatencion,ea.numeroatencion,a.fechayhora,es.nombreestudio,ISNULL(e.nombreequipo,'No Asignado') nombreequipo,ISNULL(c.sedacion,0)sedacion,c.citavip,en.fec_ini_tecno
,(select count(*)from Contraste con where con.atencion=ea.numeroatencion and con.estado!=9) contraste,en.SolicitarValidacion,en.fec_ini_supervisa
 from EXAMENXATENCION ea 
inner join ATENCION a on ea.numeroatencion=a.numeroatencion
inner join ESTUDIO es on ea.codigoestudio=es.codigoestudio
inner join CITA c on ea.numerocita=c.numerocita
inner join PACIENTE p on ea.codigopaciente=p.codigopaciente
inner join Encuesta en on ea.codigo=en.numeroexamen
left join EQUIPO e on ea.equipoAsignado=e.codigoequipo
where ea.estadoestudio='A'
and en.estado between 1 and 2
and substring(ea.codigoestudio,1,3) in ({0})
order by ea.codigo";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(string.Format(queryString, String.Join(",", user.sucursales.ToArray())), connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        DateTime fecha = Convert.ToDateTime(reader["fechayhora"]);
                        TimeSpan tps = DateTime.Now - fecha;
                        allCompanies.Add(new RealizarExamenViewModel
                        {
                            num_examen = Convert.ToInt32(reader["codigo"]),
                            nom_paciente = reader["paciente"].ToString(),
                            hora_cita = Convert.ToDateTime(reader["horaatencion"]),
                            numeroatencion = Convert.ToInt32(reader["numeroatencion"]),
                            hora_admision = Convert.ToDateTime(reader["fechayhora"]),
                            min_transcurri = tps.Days == 0 ? tps.Hours.ToString() + ":" + tps.Minutes.ToString() : tps.Days.ToString() + " día(s) " + tps.Hours.ToString() + ":" + tps.Minutes.ToString(),
                            condicion = "",
                            nom_estudio = reader["nombreestudio"].ToString(),
                            nom_equipo_pro = reader["nombreequipo"].ToString(),
                            isSedacion = Convert.ToBoolean(reader["sedacion"]),
                            isVIP = Convert.ToBoolean(reader["citavip"]),
                            isIniciado = reader["fec_ini_tecno"] == DBNull.Value ? false : true,
                            isValidacion = reader["SolicitarValidacion"] == DBNull.Value ? false : Convert.ToBoolean(reader["SolicitarValidacion"]),
                            isValidado = reader["fec_ini_supervisa"] == DBNull.Value ? false : true,
                            isContraste = Convert.ToInt32(reader["contraste"]) > 0,
                            isActivado = true,//este campi valida si se puede seguir con el flujo se puede configurar un tiempo de espera despues de iniciar el examen
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
            /* IList<RealizarExamenViewModel> allCompanies = (from ea in db.EXAMENXATENCION

                                                            join e in db.EQUIPO on new { equipoAsignado = (int)ea.equipoAsignado } equals new { equipoAsignado = e.codigoequipo } into e_join
                                                            from e in e_join.DefaultIfEmpty()

                                                            join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                                            join en in db.Encuesta on new { numeroexamen = ea.codigo } equals new { numeroexamen = en.numeroexamen } into en_join
                                                            from en in en_join.DefaultIfEmpty()
                                                            //from en in db.Encuesta.DefaultIfEmpty()
                                                            where ea.estadoestudio == "A" //todos los admitidos
                                                            && (user.sucursales).Contains(ea.codigoestudio.Substring(1 - 1, 3)) //sucursales asignadas
                                                            && en.estado >= 1
                                                            && en.estado <= 2 // estado esta encuestado pero sin realizar ni supervisar

                                                            select new RealizarExamenViewModel
                                                            {
                                                                num_examen = ea.codigo,
                                                                nom_paciente = p.apellidos + ", " + p.nombres,
                                                                hora_cita = ea.horaatencion,
                                                                numeroatencion = ea.numeroatencion,
                                                                hora_admision = ea.ATENCION.fechayhora,
                                                                min_transcurri =
                                     SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                     SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(spu)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                condicion = "",
                                                                nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                nom_equipo_pro = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
                                                                isSedacion = ea.ATENCION.CITA.sedacion == null ? false : ea.ATENCION.CITA.sedacion,
                                                                isVIP = ea.ATENCION.CITA.citavip == null ? false : ea.ATENCION.CITA.citavip,
                                                                isIniciado = en.fec_ini_tecno == null ? false : true,
                                                                isValidacion = en.SolicitarValidacion == null ? false : en.SolicitarValidacion.Value,
                                                                isValidado = en.fec_ini_supervisa == null ? false : true,
                                                                isActivado = en.fec_ini_tecno != null ? SqlFunctions.DateDiff("minute", en.fec_ini_tecno, SqlFunctions.GetDate()) > _u.tEsperaSolicitud : false,
                                                                minrestante = (en.fec_ini_tecno != null ? _u.tEsperaSolicitud - SqlFunctions.DateDiff("minute", en.fec_ini_tecno, SqlFunctions.GetDate()) : 0).Value

                                                            }).ToList();*/
            return allCompanies;
        }

        //enviar datos a la modalidad
        public ActionResult enviarModalidad(int examen, int equipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = true;
            string msj = "";
            /*verificamos si tiene habilitada la opcion de Integrador */
            var _equipo = (from x in db.EQUIPO where x.codigoequipo == equipo select x).SingleOrDefault();
            var encu = (from x in db.Encuesta
                        where x.numeroexamen == examen
                        select x).Single();
            if (_equipo.isIntegrator.Value)
            {
                var inte = (from xi in db.INTEGRACION
                            where xi.numero_estudio == examen
                            select xi).SingleOrDefault();
                if (inte == null)
                {
                    db.integrador(examen, _equipo.AETitleConsulta);
                    result = true;
                    msj = "Los datos del <b>Examen: " + examen + "</b> fueron enviados correctamente";
                }

            }
            else
            {
                result = false;
                msj = "<div class='alert alert-info'> El Equipo <b>" + _equipo.ShortDesc + "</b> no es compatible con esta opción.<br/> - Ingrese los siguientes datos manualmente porfavor:</div>";

                msj += getDatosExamen(examen);

            }
            encu.fec_ini_tecno = DateTime.Now;//iniciamos tecnologo

            db.SaveChanges();

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //solicitar validacion
        public ActionResult SolicitarSupervicion(int examen)
        {
            bool result = true;
            bool pasa = false;
            string msj = "";
            /*verificamos si tiene habilitada la opcion de Supervicion */
            var encuesta = (from x in db.Encuesta where x.numeroexamen == examen select x).SingleOrDefault();
            if (encuesta.SolicitarValidacion == null)
            {
                encuesta.SolicitarValidacion = true;
                encuesta.fec_Solicitavalidacion = DateTime.Now;
                encuesta.usu_Solicitavalidacion = Membership.GetUser().ProviderUserKey.ToString();

                var _examen = (from x in db.EXAMENXATENCION
                               where x.codigo == examen
                               select x).SingleOrDefault();
                var _equipo = (from x in db.EQUIPO
                               where x.codigoequipo == _examen.equipoAsignado
                               select x).Single();

                
                
                if (DateTime.Now.ToString("dd-MM-yyyy") == "09-07-2017")
                {
                    encuesta.fec_ini_supervisa = encuesta.fec_Solicitavalidacion.Value.AddMilliseconds(10);
                    encuesta.fec_supervisa = DateTime.Now;
                    encuesta.usu_reg_super = Membership.GetUser().ProviderUserKey.ToString();
                    encuesta.estado = 2;
                    pasa = true;

                }



                if (!_equipo.isSupervisable)//verificamos que el equipo no solicite superviciones
                {
                    if (!_examen.ESTUDIO.IsValidacion.Value)//verificamos que el estudio no solicite superviciones
                    {
                        encuesta.fec_ini_supervisa = encuesta.fec_Solicitavalidacion.Value.AddMilliseconds(10);
                        encuesta.fec_supervisa = DateTime.Now;
                        encuesta.usu_reg_super = Membership.GetUser().ProviderUserKey.ToString();
                        encuesta.estado = 2;
                        pasa = true;

                    }
                }
                else
                {
                    using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
                    {
                        try
                        {
                            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                            AlertasEncuestas ae = new AlertasEncuestas();
                            ae.fecha = DateTime.Now;
                            ae.usuario = user.UserName;
                            ae.ubicacion_solicitante = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == _examen.equipoAsignado).ShortDesc.ToLower();
                            ae.tipo_solicitud = "Supervisor";
                            ae.isReproducido = false;
                            ae.examen = examen;
                            db1.AlertasEncuestas.Add(ae);
                            db1.SaveChanges();
                        }
                        catch (Exception) { }
                    }
                }

                db.SaveChanges();
                result = true;
                msj = "La solicitud de supervision del <b>Examen: " + examen + "</b> fue enviada correctamente";

            }
            else
            {
                result = false;
                msj = "La solicitud de supervision del <b>Examen" + examen + "</b> no fue enviada";
            }

            return Json(new { result = result, mensaje = msj, pasa = pasa }, JsonRequestBehavior.DenyGet);
        }

        //Rellamada Supervisor
        public ActionResult RellamadaSupervisor(int examen)
        {

            /*verificamos si tiene habilitada la opcion de Supervicion */
            var encuesta = (from x in db.Encuesta where x.numeroexamen == examen select x).SingleOrDefault();

            var _examen = (from x in db.EXAMENXATENCION
                           where x.codigo == examen
                           select x).SingleOrDefault();
            using (RepositorioJhonEntities db1 = new RepositorioJhonEntities())
            {
                try
                {
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    AlertasEncuestas ae = new AlertasEncuestas();
                    ae.fecha = DateTime.Now;
                    ae.usuario = user.UserName;
                    ae.ubicacion_solicitante = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == _examen.equipoAsignado).ShortDesc.ToLower();
                    ae.tipo_solicitud = "Supervisor";
                    ae.isReproducido = false;
                    ae.examen = examen;
                    db1.AlertasEncuestas.Add(ae);
                    db1.SaveChanges();
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.DenyGet);
                }
            }

            return Json(true, JsonRequestBehavior.DenyGet);

        }
        public ActionResult getEquipoxExamenPerifoneo(int examen)
        {

            var _examen = (from x in db.EXAMENXATENCION
                           where x.codigo == examen
                           select x).SingleOrDefault();
            string equipo = "";
            try
            {
                equipo = db.EQUIPO.SingleOrDefault(x => x.codigoequipo == _examen.equipoAsignado).ShortDesc.ToLower();

            }
            catch (Exception)
            {
                return Json("", JsonRequestBehavior.DenyGet);
            }
            return Json(equipo, JsonRequestBehavior.DenyGet);

        }

        public ActionResult DatosExamen(int examen, int tipo)
        {
            bool result = true;
            string msj = "";
            if (tipo == 1)//lectura
                msj = getDatosExamen(examen);
            else if (tipo == 2)//escritura
                msj = getDatosExamenModalidad(examen);
            else
            {
                result = false;
                msj = "";
            }
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //Insertar Contraste
        public ActionResult InsertarContraste(int codigo)
        {

            var examen = db.EXAMENXATENCION.Where(x => x.codigo == codigo).Single();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            try
            {
                #region Agregar Tabla Contraste
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
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }

        }

        public ActionResult VerificarValidacionContraste(int examen)
        {
            var cita = (from ci in db.Contraste
                        where ci.numeroexamen == examen
                        && ci.estado != 9
                        select ci).SingleOrDefault();
            if (cita == null)
                return Json(new { result = 99 }, JsonRequestBehavior.DenyGet);
            else
                return Json(new { result = (cita.estado.Value.ToString()) }, JsonRequestBehavior.DenyGet);
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


        //get datos de paciente para Modalidad
        private string getDatosExamenModalidad(int examen)
        {
            string msj = "";
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            _examen.ATENCION = (from x in db.ATENCION where x.numeroatencion == _examen.numeroatencion select x).SingleOrDefault();
            msj = "<script type='text/javascript'>" +
    "$(document).ready(function () {" +
        "$('#modal_Modalidad_Fecha').focusout(function () {" +
            "var ano_now=(new Date());" +
            "var year=$('#modal_Modalidad_Fecha').val();" +
            "var ano = new Date( parseInt(year.substring(0, 4)), parseInt(year.substring(5, 7)) - 1, parseInt(year.substring(8)));" +

            "var edad=CalculateDateDiff(ano,ano_now);" +
             "$('#div_edad').html('<label>Edad : </label> '+edad);" +

        "});" +
    "});" +
"</script>" +


                "<div class='form-group'>" +
                    "<label>N° Examen : </label> " +
                     _examen.codigo +
                     "<input type='hidden' id='modalidad_examen' value='" + _examen.codigo + "'/>" +
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
                   "<input type='date' class='form-control' id='modal_Modalidad_Fecha' value='" +
                    _paciente.fechanace.ToString("yyyy-MM-dd") + "' /></b>" +
                "</div>" +
                 "<div id='div_edad' class='form-group'>" +
                    "<label>Edad : </label> " +
                    (_paciente.fechanace.Year < DateTime.Now.Year ? (DateTime.Now.Year - _paciente.fechanace.Year).ToString() + " Años" : (DateTime.Now.Month - _paciente.fechanace.Month).ToString() + " Meses") +
                "</div>" +
                 "<div class='form-group'> " +
                    "<label>Peso (kg.):</label> " +
                     "<input type='number' class='form-control' id='modal_Modalidad_Peso' value='" + _examen.ATENCION.peso.ToString() + "'/>" +
                "</div>" +
                "<div class='form-group'> " +
                    "<label>Talla (mts.):</label> " +
                     "<input type='decimal' class='form-control' id='modal_Modalidad_talla' value='" + _examen.ATENCION.talla.ToString() + "'/>" +
                "</div>" +
                "<div class='form-group'> " +
                    "<label>Medico:</label> " +
                    "<b><i>(" + _examen.ATENCION.cmp.ToString().Trim() + ")</i></b> - " +
                    _examen.ATENCION.MEDICOEXTERNO.apellidos + " , " + _examen.ATENCION.MEDICOEXTERNO.nombres +
                "</div>";
            return msj;
        }

        //Actualizar Datos Paciente y Admision
        public ActionResult ActualizarDatosAdmision(int nexamen, decimal talla, int peso, DateTime fecha)
        {
            bool result = true;
            string msj = "";
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == nexamen select x).Single();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).Single();
            if (talla != (decimal)_examen.ATENCION.talla)
            {
                _examen.ATENCION.isChangeTalla = true;
                _examen.ATENCION.newTalla = _examen.ATENCION.talla;
                _examen.ATENCION.talla = (float)talla;
            }
            if (peso != _examen.ATENCION.peso)
            {
                _examen.ATENCION.isChangePeso = true;
                _examen.ATENCION.newPeso = _examen.ATENCION.peso;
                _examen.ATENCION.peso = peso;
            }
            if (fecha != _paciente.fechanace)
            {
                _examen.ATENCION.isChangeEdad = true;
                _examen.ATENCION.newEdad = fecha;
                _paciente.fechanace = fecha;
            }
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //Ingresar Tecnica
        public ActionResult IngresarTecnica(int nexamen, int ncita, string nestudio)
        {
            bool result = true;
            string msj = "";
            var cita = (from ci in db.EXAMENXCITA
                        where ci.numerocita == ncita &&
                        ci.codigoexamencita == (db.EXAMENXCITA.Where(ci1 => ci1.numerocita == ncita).Max(ci1 => ci1.codigoexamencita))
                        select ci).Single();
            //creamos la tecnica
            #region Registar Tecnica
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
                reg.numeroestudio = nexamen;
                reg.codigoestudio = db.EXAMENXATENCION.Where(x => x.codigo == nexamen).Select(x => x.codigoestudio).Single();
                reg.codigotecnica = nestudio;
                reg.nombretecnica = db.ESTUDIO.Where(x => x.codigoestudio == nestudio).Select(x => x.nombreestudio).Single();
                reg.codigoequipo = cita.codigoequipo;
                reg.codigoclase = int.Parse(nestudio.Substring(6, 1));
                db.REGISTRODETECNICA.Add(reg);
            }
            #endregion
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //Listar Tecnica
        public ActionResult ListarTecnicas(int ncita)
        {
            var _exa = db.EXAMENXATENCION.Where(x => x.codigo == ncita).Single();
            List<EXAMENXCITA> lista = new List<EXAMENXCITA>();
            if (_exa.codigoestudio.Substring(3, 2) == "01")
            {
                lista = db.EXAMENXCITA.Where(x => x.numerocita == _exa.numerocita && x.codigoestudio.Substring(7, 2) == "99" && x.codigoestudio.Substring(5, 1) == "0").ToList();

            }
            else if (_exa.codigoestudio.Substring(3, 2) == "02")
            {
                lista = db.EXAMENXCITA.Where(x => x.numerocita == _exa.numerocita && x.codigoestudio.Substring(6, 3) == "902").ToList();
            }


            return PartialView("_ListExamenCita", lista);
        }

        //Elimiar tecnica
        public ActionResult EliminarTecnica(int ncita)
        {
            bool result = true;
            string msj = "";
            var cita = (from ci in db.EXAMENXCITA
                        where ci.codigoexamencita == ncita
                        select ci).Single();

            db.EXAMENXCITA.Remove(cita);
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //verificar si llevo Contraste
        public ActionResult VerificarContraste(int ncita)
        {
            bool result;
            string msj = "";
            result = VerificarContrasteCita(ncita);
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //  Verificar si solicito contraste anteriormente
        public ActionResult VerificarRegistroContraste(int atencion)
        {
            bool result;
            string msj = "";
            var cita = (from ci in db.Contraste
                        where ci.atencion == atencion
                        && ci.estado != 9
                        select ci).ToList();

            if (cita.Count > 0)
                result = true;
            else
                result = false;
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        public bool VerificarContrasteCita(int ncita)
        {

            var cita = (from ci in db.EXAMENXCITA
                        where ci.numerocita == ncita
                        && ci.codigoestudio.Substring(7, 2) != "99"
&& ci.ESTUDIO.nombreestudio.Contains("CONTRASTE")
                        select ci).ToList();
            if (cita.Count > 0)
                return true;
            else
                return false;
        }
        // GET: /RealizarExamenViewModel/Create
        public ActionResult RealizarExamen(int examen, int equipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            RealizarExamenViewModel model = new RealizarExamenViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            //COntraste

            var _contraste = (from x in db.Contraste where x.numeroexamen == examen && x.estado != 9 select x).SingleOrDefault();

            //REALIZAREXAMEN
            model.modalidad = _examen.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";
            model.num_examen = _examen.codigo;
            model.id_paciente = _examen.codigopaciente;
            model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;
            /*if( _examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == _examen.codigoestudio).SingleOrDefault().sedador!=null)
                model.anestesiologa = _examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == _examen.codigoestudio).SingleOrDefault().sedador;*/
            //model.isSedacion = _examen.ATENCION.CITA.sedacion;
            model.id_equipo_rea = equipo;
            //Contraste
            model.isContraste = VerificarContrasteCita(_examen.numerocita);
            if (model.isContraste)
                model.estadoContraste = 1;
            else
            {
                if (_contraste != null)
                    model.estadoContraste = _contraste.estado.Value;
                else
                    model.estadoContraste = 99;
            }
            model.tecnica = new List<EXAMENXCITA>();//_examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio.Substring(7, 2) == "99" && x.codigoestudio.Substring(5, 1) == "0").ToList();

            //ENCUESTA
            var encuestador = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_encu).SingleOrDefault();
            var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_tecno).SingleOrDefault();
            var supervisor = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_super).SingleOrDefault();
            var _equi = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
            model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = _equi == null ? "" : _equi.ShortDesc, nexamen = _examen.codigo };
            //PACIENTE
            model.paciente = _paciente;
            //EXAMENXATENCION
            model.examen = _examen;
            //ANESTESIOLOGO
            ViewBag.anesteciologos = new SelectList((from u in db.USUARIO
                                                     where u.bloqueado == false
                                                     && u.isAnes == true
                                                     orderby u.ShortName
                                                     select u).ToList(), "codigousuario", "ShortName", model.anestesiologa);

            //ENFERMERA
            ViewBag.enfermeras = new SelectList((from u in db.USUARIO
                                                 where u.bloqueado == false
                                                 && u.isEnf == true
                                                 orderby u.ShortName
                                                 select u).ToList(), "codigousuario", "ShortName");

            //INSUMOCONTRASTE
            ViewBag.insumoContraste = new SelectList((from x in db.ESTUDIO
                                                      where x.nombreestudio.Contains("ampliacion") == false &&
                                                      x.nombreestudio.Contains("contraste") &&
                                                      x.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                                      x.codigoestudio.Substring(1, 2) == _examen.codigoestudio.Substring(1, 2) &&
                                                      x.codigoestudio.Substring(3, 2) == _examen.codigoestudio.Substring(3, 2)
                                                      select x).ToList(), "codigoestudio", "nombreestudio");

            //TECNICAS
            if (_examen.codigoestudio.Substring(4, 1) == "1")//si es RM
                ViewBag.tecnicas = (from es in db.ESTUDIO
                                    where es.codigoestudio.Substring(7, 2) == "99" &&
                                    es.codigoestudio.Substring(5, 1) == "0" &&
                                    es.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                    es.codigoestudio.Substring(2, 1) == _examen.codigoestudio.Substring(2, 1) &&
                                    es.codigoestudio.Substring(4, 1) == _examen.codigoestudio.Substring(4, 1)
                                    orderby es.nombreestudio
                                    select es).ToList();
            else if (_examen.codigoestudio.Substring(4, 1) == "2")//si es TEM
                ViewBag.tecnicas = (from es in db.ESTUDIO
                                    where
                                    es.codigoestudio.Substring(4, 1) == "2" &&
                                    es.codigoestudio.Substring(5, 1) == "0" &&
                                    es.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                    es.codigoestudio.Substring(2, 1) == _examen.codigoestudio.Substring(2, 1) &&
                                    es.codigoestudio.Substring(6, 3) == "902"
                                    orderby es.nombreestudio
                                    select es).ToList();
            else
                ViewBag.tecnicas = new List<ESTUDIO>();
            return View(model);
        }

        //
        // POST: /RealizarExamenViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarExamen(RealizarExamenViewModel item)
        {
            var encuesta = db.Encuesta.Where(x => x.numeroexamen == item.num_examen).Single();
            var examen = db.EXAMENXATENCION.Where(x => x.codigo == item.num_examen).Single();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _contraste = (from x in db.Contraste where x.numeroexamen == item.num_examen && x.estado != 9 select x).SingleOrDefault();

            item.modalidad = examen.codigoestudio.Substring(3, 2) == "01" ? "RM" : "TEM";

            if (db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == examen.codigo).ToList().Count <= 10)
            {

                encuesta.estado = 4;//si no necesita postproceso termina
                /*Sedacion*/
                if (item.isSedacion)
                {
                    #region Registar Sedacion
                    ESTUDIO sedacion = null;

                    if ((examen.codigocompaniaseguro == 27 || examen.codigocompaniaseguro == 37) && (examen.codigoestudio.Substring(0, 3) == "101"))
                    {
                        var edad = DateTime.Today.AddTicks(-examen.ATENCION.PACIENTE.fechanace.Ticks).Year - 1;
                        var filtro = "";
                        if (edad < 8)
                            filtro = "Sedacion (menor a 8 años)";
                        else
                            filtro = "Sedacion (mayor a 8 años)";

                        sedacion = (from ci in db.ESTUDIO
                                    where ci.nombreestudio.Contains(filtro) &&
                                    ci.codigoclase == 9 &&
                                    ci.codigoestudio.Substring(0, 3) == examen.codigoestudio.Substring(0, 3) &&
                                    ci.codigoestudio.Substring(4, 1) == examen.codigoestudio.Substring(4, 1) &&
                                    ci.codigoestudio.Substring(5, 1) == "0"
                                    select ci).SingleOrDefault();
                    }
                    if (sedacion == null)
                    {
                        sedacion = (from ci in db.ESTUDIO
                                    where ci.nombreestudio.Contains("Sedacion") &&
                                    (!ci.nombreestudio.Contains("8 años")) &&
                                       ci.codigoclase == 9 &&
                                       ci.codigoestudio.Substring(0, 3) == examen.codigoestudio.Substring(0, 3) &&
                                       ci.codigoestudio.Substring(4, 1) == examen.codigoestudio.Substring(4, 1) &&
                                       ci.codigoestudio.Substring(5, 1) == "0"
                                    select ci).SingleOrDefault();
                    }
                    #region Registar Tecnica Sedacion
                    var cita = (from ci in db.EXAMENXCITA
                                where ci.numerocita == examen.numerocita &&
                                ci.codigoexamencita == (db.EXAMENXCITA.Where(ci1 => ci1.numerocita == examen.numerocita).Max(ci1 => ci1.codigoexamencita))
                                select ci).Single();

                    var sedacita = (from ci in db.EXAMENXCITA
                                    where ci.numerocita == examen.numerocita &&
                                    ci.codigoclase == 9 && ci.codigoestudio == sedacion.codigoestudio
                                    select ci).SingleOrDefault();


                    if (sedacita == null)
                    {
                        EXAMENXCITA exc = new EXAMENXCITA();
                        exc.numerocita = cita.numerocita;
                        exc.codigopaciente = cita.codigopaciente;
                        exc.horacita = cita.horacita;
                        exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == cita.codigocompaniaseguro && cia.codigoestudio == sedacion.codigoestudio select cia.preciobruto).SingleOrDefault();
                        exc.codigocompaniaseguro = cita.codigocompaniaseguro;
                        exc.ruc = cita.ruc;
                        exc.codigoequipo = cita.codigoequipo;
                        exc.codigoestudio = sedacion.codigoestudio;
                        exc.codigoclase = int.Parse(sedacion.codigoestudio.Substring(6, 1));
                        exc.codigomodalidad = cita.codigomodalidad;
                        exc.codigounidad = cita.codigounidad;
                        exc.codigomoneda = cita.codigomoneda;
                        exc.estadoestudio = "R";
                        db.EXAMENXCITA.Add(exc);

                        if (exc.codigoestudio.Substring(7, 2) == "99")
                        {
                            REGISTRODETECNICA reg = new REGISTRODETECNICA();
                            reg.numeroestudio = examen.codigo;
                            reg.codigoestudio = examen.codigoestudio;
                            reg.codigotecnica = item.contraste;
                            reg.nombretecnica = db.ESTUDIO.Where(x => x.codigoestudio == item.contraste).SingleOrDefault().nombreestudio;
                            reg.codigoequipo = item.id_equipo_rea;
                            reg.codigoclase = int.Parse(item.contraste.Substring(6, 1));
                            db.REGISTRODETECNICA.Add(reg);
                        }
                    }

                    #endregion
                    #region Agregar Tabla Sedacion
                    var itemsedacion = (from x in db.Sedacion
                                        where x.atencion == examen.numeroatencion
                                        select x).SingleOrDefault();
                    if (itemsedacion != null)
                    {
                        itemsedacion.numeroexamen = examen.codigo;
                        itemsedacion.paciente = examen.codigopaciente;
                        itemsedacion.usuario = item.anestesiologa;
                        itemsedacion.tecnologo = user.ProviderUserKey.ToString();
                        itemsedacion.consentimiento = examen.consentimiento;
                        itemsedacion.fecha_tecnologo = DateTime.Now;
                        itemsedacion.atencion = examen.numeroatencion;
                        itemsedacion.estado = 0;
                    }
                    else
                    {

                        Sedacion seda = new Sedacion();
                        seda.numeroexamen = examen.codigo;
                        seda.paciente = examen.codigopaciente;
                        seda.usuario = item.anestesiologa;
                        seda.tecnologo = user.ProviderUserKey.ToString();
                        seda.consentimiento = examen.consentimiento;
                        seda.fecha_tecnologo = DateTime.Now;
                        seda.atencion = examen.numeroatencion;
                        seda.estado = 0;
                        db.Sedacion.Add(seda);

                    }
                    examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == examen.codigoestudio).Single().sedador = item.anestesiologa;
                    #endregion
                    #endregion

                }

                if (item.isPostproceso)
                {
                    #region Registar PostProceso
                    POSTPROCESO post = new POSTPROCESO();
                    post.numeroestudio = item.num_examen;
                    post.codigopaciente = examen.codigopaciente;
                    post.codigoestudio = "9999";
                    post.codigousuario = user.ProviderUserKey.ToString();
                    post.descripcion = item.postproceso == null ? "" : item.postproceso.ToString();
                    post.cantidadplaca = 0;
                    post.fecha = DateTime.Now;
                    post.estado = "N";
                    post.tipo = "E";
                    db.POSTPROCESO.Add(post);
                    #endregion
                    if (encuesta.fec_fin_proto == null)
                        encuesta.estado = 3;//si  necesita postproceso continua
                }



                /* if (item.isContraste && !item.isContrasteContinuo)
                 {
                     #region Registar Tecnica Contraste
                     var cita = (from ci in db.EXAMENXCITA
                                 where ci.numerocita == examen.numerocita &&
                                 ci.codigoexamencita == (db.EXAMENXCITA.Where(ci1 => ci1.numerocita == examen.numerocita).Max(ci1 => ci1.codigoexamencita))
                                 select ci).Single();


                     EXAMENXCITA exc = new EXAMENXCITA();
                     exc.numerocita = cita.numerocita;
                     exc.codigopaciente = cita.codigopaciente;
                     exc.horacita = cita.horacita;
                     exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == cita.codigocompaniaseguro && cia.codigoestudio == item.contraste select cia.preciobruto).SingleOrDefault();
                     exc.codigocompaniaseguro = cita.codigocompaniaseguro;
                     exc.ruc = cita.ruc;
                     exc.codigoequipo = cita.codigoequipo;
                     exc.codigoestudio = item.contraste;
                     exc.codigoclase = int.Parse(item.contraste.Substring(6, 1));
                     exc.codigomodalidad = cita.codigomodalidad;
                     exc.codigounidad = cita.codigounidad;
                     exc.codigomoneda = cita.codigomoneda;
                     exc.estadoestudio = "R";
                     db.EXAMENXCITA.Add(exc);
                     if (exc.codigoestudio.Substring(7, 2) == "99")
                     {
                         REGISTRODETECNICA reg = new REGISTRODETECNICA();
                         reg.numeroestudio = examen.codigo;
                         reg.codigoestudio = examen.codigoestudio;
                         reg.codigotecnica = item.contraste;
                         reg.nombretecnica = db.ESTUDIO.Where(x => x.codigoestudio == item.contraste).SingleOrDefault().nombreestudio;
                         reg.codigoequipo = item.id_equipo_rea;
                         reg.codigoclase = int.Parse(item.contraste.Substring(6, 1));
                         db.REGISTRODETECNICA.Add(reg);
                     }
                     #endregion
                     #region Agregar Tabla Contraste
                     Contraste enf = new Contraste();
                     enf.numeroexamen = examen.codigo;
                     enf.tecnologo = user.ProviderUserKey.ToString();
                     if (item.enfermera != "ENF01")
                         enf.usuario = item.enfermera;
                     else
                         enf.usuario = user.ProviderUserKey.ToString(); ;
                     enf.fecha_inicio = DateTime.Now;
                     enf.paciente = examen.codigopaciente;
                     enf.estado = 0;
                     enf.tipo_contraste = item.contraste;
                     enf.atencion = examen.numeroatencion;
                     db.Contraste.Add(enf);
                     #endregion
                 }*/

                //registar encuesta satisfaccion Paciente
                if (encuesta.usu_reg_super != null)
                {
                    var enpac = (from x in db.Encuesta_Satisfaccion
                                 where x.numeroatecion == examen.numeroatencion && x.paciente == examen.codigopaciente
                                 select x).SingleOrDefault();
                    if (enpac == null)
                    {
                        Encuesta_Satisfaccion ens = new Encuesta_Satisfaccion();
                        ens.numeroatecion = examen.numeroatencion;
                        ens.fec_inicio = DateTime.Now;
                        ens.paciente = examen.codigopaciente;
                        db.Encuesta_Satisfaccion.Add(ens);
                    }
                }
                /*VALIDACIONES DE ARTRO*/
                if (examen.codigoestudio.Substring(6, 1) == "7")
                {
                    db.InsertEstudioArto(examen.codigo);
                }

                /*Honorario Tecnologo*/
                #region Honorario Tecnologo
                HONORARIOTECNOLOGO ht = new HONORARIOTECNOLOGO();
                ht.codigohonorariotecnologo = examen.codigo;
                ht.codigopaciente = examen.codigopaciente;
                ht.codigoequipo = item.id_equipo_rea;
                ht.tecnologoturno = user.siglas;
                ht.horaestudio = DateTime.Now;
                ht.fechaestudio = DateTime.Now;
                if (ht.horaestudio.Hour > 7 && ht.horaestudio.Hour < 15)
                    ht.turno = "M";
                else if (ht.horaestudio.Hour >= 15 && ht.horaestudio.Hour < 24)
                    ht.turno = "T";
                else if (ht.horaestudio.Hour >= 0 && ht.horaestudio.Hour <= 7)
                    ht.turno = "N";
                ht.placamalogro = 0;
                ht.placauso = item.placas;
                ht.codequiporealizo = item.id_equipo_rea;
                ht.procesar = item.isPostproceso;
                ht.sedacion = item.isSedacion;
                ht.contraste = VerificarContrasteCita(examen.numerocita);
                ht.tecnicas = item.istecnicas;
                ht.contraste_continuo = item.isContrasteContinuo;
                ht.comentarios = item.inf_adicional;
                ht.isRevisado = false;
                db.HONORARIOTECNOLOGO.Add(ht);
                #endregion
                //cambiamos el estado a REALIZADO O S
                //if (examen.codigoestudio.Substring(5, 1) == "1")
                //    examen.estadoestudio = "S";
                //else

                examen.estadoestudio = "R";
                var integrador=db.INTEGRACION.SingleOrDefault(x => x.numero_estudio == examen.codigo);
                if (integrador != null)
                {
                    integrador.estado = 2;
                    new Variable().eliminarIntegradorRealizado(examen.codigo.ToString());
                }
               
                DateTime fecha_validar = DateTime.Now;

                if (fecha_validar.DayOfWeek == DayOfWeek.Sunday && examen.codigoestudio.Substring(0, 3) == "101")
                {
                    examen.GrupoMedico = item.grupomedico;
                }

                encuesta.fec_fin_tecno = DateTime.Now;
                encuesta.usu_reg_tecno = user.ProviderUserKey.ToString();
                #region GrupoMedico
                if (examen.codigoestudio.Substring(0, 3) != "101")
                    examen.GrupoMedico = item.grupomedico;
                #endregion
                //finalizamos estudio Visor Llamadas a Paciente
                try
                {
                    var xvi = (from xv in db.Visor
                               where xv.examen == examen.codigo
                               select xv).ToList();
                    if (xvi.Count > 0)
                    {
                        foreach (var item_visor in xvi)
                        {
                            item_visor.estado = "R";
                            item_visor.fec_fin = DateTime.Now;
                        }
                    }
                }
                catch (Exception)
                {

                }
                #region Ticketero
                var idTicket = examen.ATENCION.numeroTicket;
                if (idTicket != null)
                {
                    var ticket = db.TKT_Ticketera.Where(x => x.id_Ticket == idTicket).SingleOrDefault();
                    if (ticket != null)
                    {
                        //ticket.usu_caja = user.ProviderUserKey.ToString();
                        //ticket.fec_caja = DateTime.Now;
                        ticket.estado = 3;//Estado Caja

                    }
                }
                #endregion

                //cambiamos el estado en la Cita del estudio
                var lcitas = examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == examen.codigoestudio).ToList();
                foreach (var ci in lcitas)
                {
                    ci.estadoestudio = "R";
                }

                var fec_d = DateTime.Now;
                var domingo = Convert.ToInt32(fec_d.DayOfWeek);

                if (domingo == 7)
                {
                    db.spu_verificarTurno_Supervision(examen.codigo, item.grupomedico.ToString(), user.ProviderUserKey.ToString());
                }
                else if (examen.codigoestudio.Substring(2, 1) != "1")
                {
                    db.spu_verificarTurno_Supervision(examen.codigo, item.grupomedico.ToString(), user.ProviderUserKey.ToString());
                }
                //try
                //{


                //}
                //catch (Exception)
                //{
                //}

                //Guardar Data

                db.SaveChanges();


            }
            return RedirectToAction("ListaEspera", new { equipo = item.id_equipo_rea });
            //return Json(true);
        }


        //
        // GET: /RealizarExamenViewModel/Create
        public ActionResult UpdateExamen(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            RealizarExamenViewModel model = new RealizarExamenViewModel();
            var _examen = (from x in db.EXAMENXATENCION where x.codigo == examen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var _tecnologo = (from x in db.HONORARIOTECNOLOGO where x.codigohonorariotecnologo == examen select x).SingleOrDefault();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == examen select x).SingleOrDefault();
            if (_examen != null && _paciente != null && _tecnologo != null && _encuesta != null)
            {

                //REALIZAREXAMEN
                model.num_examen = _examen.codigo;
                model.id_paciente = _examen.codigopaciente;
                model.nom_paciente = _paciente.apellidos + ", " + _paciente.nombres;

                model.isSedacion = _tecnologo.sedacion == null ? false : _tecnologo.sedacion.Value;
                model.id_equipo_rea = _tecnologo.codequiporealizo;
                model.tecnica = _examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio.Substring(7, 2) == "99" && x.codigoestudio.Substring(5, 1) == "0").ToList();
                model.istecnicas = _tecnologo.tecnicas == null ? false : _tecnologo.tecnicas.Value;
                model.placas = _tecnologo.placauso;
                model.isPostproceso = _tecnologo.procesar;
                model.isContraste = _tecnologo.contraste == null ? false : _tecnologo.contraste.Value;
                model.isContrasteContinuo = _tecnologo.contraste_continuo == null ? false : _tecnologo.contraste_continuo.Value;
                model.inf_adicional = _tecnologo.comentarios;
                /*seleccionamos a la sedadora si registro*/
                if (model.isSedacion)
                    model.anestesiologa = _examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == _examen.codigoestudio).SingleOrDefault().sedador;

                /*seleccionamos a la contraste si registro*/
                if (model.isContraste && !model.isContrasteContinuo)
                {
                    var _con = db.Contraste.Where(x => x.numeroexamen == _examen.codigo && x.estado != 9).SingleOrDefault();
                    if (_con != null)
                    {
                        model.enfermera = _con.usuario;
                        model.contraste = _con.tipo_contraste;
                    }

                }
                //ENCUESTA
                var encuestador = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_encu).SingleOrDefault();
                var tecnoologo = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_tecno).SingleOrDefault();
                var supervisor = db.USUARIO.Where(x => x.codigousuario == _encuesta.usu_reg_super).SingleOrDefault();
                var _equi = db.EQUIPO.Where(x => x.codigoequipo == _examen.equipoAsignado).SingleOrDefault();
                model.encuesta = new ViewModels.Encuesta.EncuestaDetalleViewModel { encuestador = encuestador == null ? "" : encuestador.ShortName, tecnologo = tecnoologo == null ? "" : tecnoologo.ShortName, supervisor = supervisor == null ? "" : supervisor.ShortName, equipo = _equi == null ? "" : _equi.ShortDesc, nexamen = _examen.codigo };
                //PACIENTE
                model.paciente = _paciente;
                //EXAMENXATENCION
                model.examen = _examen;
                //ANESTESIOLOGO
                ViewBag.anesteciologos = new SelectList((from u in db.USUARIO
                                                         join e in db.EMPLEADO on u.dni equals e.dni
                                                         where u.bloqueado == false
                                                         && e.codigocargo == 10
                                                         orderby e.apellidos
                                                         select u).ToList(), "codigousuario", "ShortName", model.anestesiologa);

                //ENFERMERA
                ViewBag.enfermeras = new SelectList((from u in db.USUARIO
                                                     join e in db.EMPLEADO on u.dni equals e.dni
                                                     where u.bloqueado == false
                                                     && e.codigocargo == 16
                                                     orderby e.apellidos
                                                     select u).ToList(), "codigousuario", "ShortName");

                //INSUMOCONTRASTE
                ViewBag.insumoContraste = new SelectList((from x in db.ESTUDIO
                                                          where x.nombreestudio.Contains("ampliacion") == false &&
                                                          x.nombreestudio.Contains("contraste") &&
                                                          x.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                                          x.codigoestudio.Substring(1, 2) == _examen.codigoestudio.Substring(1, 2) &&
                                                          x.codigoestudio.Substring(3, 2) == _examen.codigoestudio.Substring(3, 2)
                                                          select x).ToList(), "codigoestudio", "nombreestudio");

                //TECNICAS
                if (_examen.codigoestudio.Substring(4, 1) == "1")//si es RM
                    ViewBag.tecnicas = (from es in db.ESTUDIO
                                        where es.codigoestudio.Substring(7, 2) == "99" &&
                                        es.codigoestudio.Substring(5, 1) == "0" &&
                                        es.codigoestudio.Substring(0, 1) == _examen.codigoestudio.Substring(0, 1) &&
                                        es.codigoestudio.Substring(2, 1) == _examen.codigoestudio.Substring(2, 1) &&
                                        es.codigoestudio.Substring(4, 1) == _examen.codigoestudio.Substring(4, 1)
                                        orderby es.nombreestudio
                                        select es).ToList();
                else
                    ViewBag.tecnicas = (from es in db.ESTUDIO
                                        where es.codigoestudio == "101020902000"//estudio colono
                                        orderby es.nombreestudio
                                        select es).ToList();


                return View(model);
            }
            return RedirectToAction("ListaEspera");
        }

        //
        // POST: /RealizarExamenViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateExamen(RealizarExamenViewModel item)
        {
            var examen = db.EXAMENXATENCION.Where(x => x.codigo == item.num_examen).Single();
            var cita = db.EXAMENXCITA.Where(x => x.numerocita == examen.numerocita).ToList();
            var oldHT = db.HONORARIOTECNOLOGO.Where(x => x.codigohonorariotecnologo == item.num_examen).Single();
            var encuesta = db.Encuesta.Where(x => x.numeroexamen == examen.codigo).Single();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;


            #region Sedacion
            if (oldHT.sedacion.Value != item.isSedacion)
            {
                if (!item.isSedacion)//elimina la sedacion
                {
                    bool _delete = false;
                    var _seda = db.Sedacion.Where(x => x.numeroexamen == item.num_examen).First();
                    db.Sedacion.Remove(_seda);
                    foreach (var ci in cita)
                    {
                        if (ci.codigoestudio == examen.codigoestudio) //eliminamos el anestesiologo del estudio
                            ci.sedador = null;
                        if (ci.ESTUDIO.nombreestudio.Contains("sedacion") && !_delete) //eliminamos solo una sedacion
                        {
                            db.EXAMENXCITA.Remove(ci);
                            _delete = true;
                        }
                    }

                }
                else //registra la sedacion
                {
                    #region Registar Sedacion
                    ESTUDIO sedacion = null;

                    if ((examen.codigocompaniaseguro == 27 || examen.codigocompaniaseguro == 37) && (examen.codigoestudio.Substring(0, 3) == "101"))
                    {
                        var edad = DateTime.Today.AddTicks(-examen.ATENCION.PACIENTE.fechanace.Ticks).Year - 1;
                        var filtro = "";
                        if (edad < 8)
                            filtro = "Sedacion (menor a 8 años)";
                        else
                            filtro = "Sedacion (mayor a 8 años)";

                        sedacion = (from ci in db.ESTUDIO
                                    where ci.nombreestudio.Contains(filtro) &&
                                    ci.codigoclase == 9 &&
                                    ci.codigoestudio.Substring(0, 3) == examen.codigoestudio.Substring(0, 3) &&
                                    ci.codigoestudio.Substring(4, 1) == examen.codigoestudio.Substring(4, 1) &&
                                    ci.codigoestudio.Substring(5, 1) == "0"
                                    select ci).SingleOrDefault();
                    }
                    if (sedacion == null)
                    {
                        sedacion = (from ci in db.ESTUDIO
                                    where ci.nombreestudio.Contains("Sedacion") &&
                                    (!ci.nombreestudio.Contains("8 años")) &&
                                       ci.codigoclase == 9 &&
                                       ci.codigoestudio.Substring(0, 3) == examen.codigoestudio.Substring(0, 3) &&
                                       ci.codigoestudio.Substring(4, 1) == examen.codigoestudio.Substring(4, 1) &&
                                       ci.codigoestudio.Substring(5, 1) == "0"
                                    select ci).SingleOrDefault();
                    }
                    #region Registar Tecnica Sedacion
                    var _c = cita.Where(x => x.codigoestudio == examen.codigoestudio).Single();
                    EXAMENXCITA exc = new EXAMENXCITA();
                    exc.numerocita = _c.numerocita;
                    exc.codigopaciente = _c.codigopaciente;
                    exc.horacita = DateTime.Now;
                    exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == _c.codigocompaniaseguro && cia.codigoestudio == sedacion.codigoestudio select cia.preciobruto).SingleOrDefault();
                    exc.codigocompaniaseguro = _c.codigocompaniaseguro;
                    exc.ruc = _c.ruc;
                    exc.codigoequipo = _c.codigoequipo;
                    exc.codigoestudio = sedacion.codigoestudio;
                    exc.codigoclase = int.Parse(sedacion.codigoestudio.Substring(6, 1));
                    exc.codigomodalidad = _c.codigomodalidad;
                    exc.codigounidad = _c.codigounidad;
                    exc.codigomoneda = _c.codigomoneda;
                    exc.estadoestudio = "R";
                    db.EXAMENXCITA.Add(exc);
                    #endregion
                    #region Agregar Tabla Sedacion
                    var itemsedacion = (from x in db.Sedacion
                                        where x.numeroexamen == examen.codigo
                                        select x).SingleOrDefault();

                    if (itemsedacion != null)
                    {
                        itemsedacion.numeroexamen = examen.codigo;
                        itemsedacion.paciente = examen.codigopaciente;
                        itemsedacion.usuario = item.anestesiologa;
                        itemsedacion.tecnologo = user.ProviderUserKey.ToString();
                        itemsedacion.consentimiento = examen.consentimiento;
                        itemsedacion.fecha_tecnologo = DateTime.Now;
                        itemsedacion.atencion = examen.numeroatencion;
                        itemsedacion.estado = 0;
                    }
                    else
                    {

                        Sedacion seda = new Sedacion();
                        seda.numeroexamen = examen.codigo;
                        seda.paciente = examen.codigopaciente;
                        seda.usuario = item.anestesiologa;
                        seda.tecnologo = user.ProviderUserKey.ToString();
                        seda.consentimiento = examen.consentimiento;
                        seda.fecha_tecnologo = DateTime.Now;
                        seda.atencion = examen.numeroatencion;
                        seda.estado = 0;
                        db.Sedacion.Add(seda);

                    }

                    examen.ATENCION.CITA.EXAMENXCITA.Where(x => x.codigoestudio == examen.codigoestudio).Single().sedador = item.anestesiologa;
                    #endregion
                    #endregion
                }
                oldHT.sedacion = item.isSedacion;
            }
            #endregion

            #region Contraste
            /*
            if (oldHT.contraste.Value && !oldHT.contraste_continuo.Value)// si esta registrado el contraste
            {
                //verificamos si se elimina 
                if (!(item.isContraste && !item.isContrasteContinuo))//que sea continuo o no se aplico
                {
                    bool existContrate = false;
                    foreach (var ci in cita)
                    {
                        if (ci.ESTUDIO.nombreestudio.Contains("contraste") && !existContrate)//eliminamos un contraste
                        {
                            db.EXAMENXCITA.Remove(ci);
                            existContrate = true;
                        }
                    }
                    //eliminamos el registro de contraste
                    var _con = db.Contraste.Where(x => x.numeroexamen == examen.codigo).Single();
                    db.Contraste.Remove(_con);
                }
            }
            else if (oldHT.contraste.Value && oldHT.contraste_continuo.Value)// si no esa registrado pero si indicado
            {
                //verificamos si se registra
                if ((item.isContraste && !item.isContrasteContinuo))// se aplico y no es continuo
                {
                    /* #region Registar Tecnica Contraste
                     var _c = cita.Where(x => x.codigoestudio == examen.codigoestudio).Single();

                     EXAMENXCITA exc = new EXAMENXCITA();
                     exc.numerocita = _c.numerocita;
                     exc.codigopaciente = _c.codigopaciente;
                     exc.horacita = _c.horacita;
                     exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == _c.codigocompaniaseguro && cia.codigoestudio == item.contraste select cia.preciobruto).SingleOrDefault();
                     exc.codigocompaniaseguro = _c.codigocompaniaseguro;
                     exc.ruc = _c.ruc;
                     exc.codigoequipo = _c.codigoequipo;
                     exc.codigoestudio = item.contraste;
                     exc.codigoclase = int.Parse(item.contraste.Substring(6, 1));
                     exc.codigomodalidad = _c.codigomodalidad;
                     exc.codigounidad = _c.codigounidad;
                     exc.codigomoneda = _c.codigomoneda;
                     exc.estadoestudio = "R";
                     db.EXAMENXCITA.Add(exc);
                     #endregion
                     #region Agregar Tabla Contraste
                     Contraste enf = new Contraste();
                     enf.numeroexamen = int.Parse(Request.QueryString["examen"]);
                     enf.tecnologo = user.ProviderUserKey.ToString();
                     enf.usuario = item.enfermera;
                     enf.fecha_inicio = DateTime.Now;
                     enf.paciente = examen.codigopaciente;
                     enf.estado = 0;
                     enf.tipo_contraste = item.contraste;
                     enf.atencion = examen.numeroatencion;
                     db.Contraste.Add(enf);
                     #endregion
                     *----------------
                }


            }
            else if (!oldHT.contraste.Value && !oldHT.contraste_continuo.Value)// si no esa registrado pero si indicado
            {
                //verificamos si se registra
                if ((item.isContraste && !item.isContrasteContinuo))// se aplico y no es continuo
                {
                    /*
                    #region Registar Tecnica Contraste
                    var _c = cita.Where(x => x.codigoestudio == examen.codigoestudio).Single();

                    EXAMENXCITA exc = new EXAMENXCITA();
                    exc.numerocita = _c.numerocita;
                    exc.codigopaciente = _c.codigopaciente;
                    exc.horacita = _c.horacita;
                    exc.precioestudio = (from cia in db.ESTUDIO_COMPANIA where cia.codigocompaniaseguro == _c.codigocompaniaseguro && cia.codigoestudio == item.contraste select cia.preciobruto).SingleOrDefault();
                    exc.codigocompaniaseguro = _c.codigocompaniaseguro;
                    exc.ruc = _c.ruc;
                    exc.codigoequipo = _c.codigoequipo;
                    exc.codigoestudio = item.contraste;
                    exc.codigoclase = int.Parse(item.contraste.Substring(6, 1));
                    exc.codigomodalidad = _c.codigomodalidad;
                    exc.codigounidad = _c.codigounidad;
                    exc.codigomoneda = _c.codigomoneda;
                    exc.estadoestudio = "R";
                    db.EXAMENXCITA.Add(exc);
                    #endregion
                    #region Agregar Tabla Contraste
                    Contraste enf = new Contraste();
                    enf.numeroexamen = int.Parse(Request.QueryString["examen"]);
                    enf.tecnologo = user.ProviderUserKey.ToString();
                    enf.usuario = item.enfermera;
                    enf.fecha_inicio = DateTime.Now;
                    enf.paciente = examen.codigopaciente;
                    enf.estado = 0;
                    enf.tipo_contraste = item.contraste;
                    enf.atencion = examen.numeroatencion;
                    db.Contraste.Add(enf);
                    #endregion
                     * --------------------
                }
            }
            else
            {

            }

            oldHT.contraste = item.isContraste;
            oldHT.contraste_continuo = item.isContrasteContinuo;
    */
            #endregion

            #region procesar
            if (oldHT.procesar != item.isPostproceso)
            {
                if (item.isPostproceso)
                {
                    #region Registar PostProceso
                    POSTPROCESO post = new POSTPROCESO();
                    post.numeroestudio = item.num_examen;
                    post.codigopaciente = examen.codigo;
                    post.codigoestudio = "9999";
                    post.codigousuario = user.ProviderUserKey.ToString();
                    post.descripcion = item.postproceso;
                    post.cantidadplaca = 0;
                    post.fecha = DateTime.Now;
                    post.estado = "N";
                    post.tipo = "E";
                    db.POSTPROCESO.Add(post);
                    #endregion

                    encuesta.estado = 3;//si  necesita postproceso continua
                }
                else
                {
                    var post = db.POSTPROCESO.Where(x => x.numeroestudio == examen.codigo && x.tipo == "E" && x.estado == "N").Single();
                    db.POSTPROCESO.Remove(post);
                }
                oldHT.procesar = item.isPostproceso;
            }
            #endregion

            oldHT.placauso = item.placas;
            oldHT.comentarios = item.inf_adicional;
            //Guardar Data
            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }


        public ActionResult EnviaEstudioModalidad()
        {
            if (DateTime.Now.Hour == 7)
                return View();
            else
                return RedirectToAction("ListaEspera");

        }

        public ActionResult getEquipoxExamen(int examen)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var _estudio = db.EXAMENXATENCION.Where(x => x.codigo == examen).Single().codigoestudio;
            var uni = int.Parse(_estudio.Substring(0, 1));
            var sede = int.Parse(_estudio.Substring(1, 2));
            return Json(new SelectList((from eq in db.EQUIPO
                                        where eq.estado == "1" &&
                                        eq.codigounidad2 == uni &&
                                        eq.codigosucursal2 == sede
                                        select eq).ToList(), "codigoequipo", "nombreequipo"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult enviarModalidadsinEncuesta(int examen, int equipo)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = true;
            string msj = "";
            /*verificamos si tiene habilitada la opcion de Integrador */
            var _equipo = (from x in db.EQUIPO where x.codigoequipo == equipo select x).SingleOrDefault();

            if (_equipo.isIntegrator.Value)
            {
                var inte = (from xi in db.INTEGRACION
                            where xi.numero_estudio == examen
                            select xi).SingleOrDefault();
                if (inte == null)
                {
                    db.integrador(examen, _equipo.AETitleConsulta);
                    result = true;
                    msj = "Los datos del <b>Examen: " + examen + "</b> fueron enviados correctamente";
                }
            }
            else
            {
                result = false;
                msj = "<div class='alert alert-info'> El Equipo <b>" + _equipo.ShortDesc + "</b> no es compatible con esta opción.<br/> - Ingrese los siguientes datos manualmente por favor:</div>";

                msj += getDatosExamen(examen);
            }
            db.SaveChanges();

            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }
    }
}
