using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Encuesta.Models;
using Encuesta.ViewModels.Encuesta;
using Encuesta.Member;
using System.Web.Security;
using System.Data.Entity.SqlServer;
using Encuesta.ViewModels;
using System.Data.SqlClient;
using Encuesta.Util;

namespace Encuesta.Controllers.Atenciones
{
    [Authorize(Roles = "16,26,27")]
    public class AtencionController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        //
        // GET: /Atencion/
        // GET: /Lista Realizar Encuesta de pacientes/
        public ActionResult ListaEspera()
        {
            return View();
        }
        public ActionResult AprobarEntreTurno()
        {
            string queryString = "select se.IdSolicitud,se.hora_reg hora,m.tipo,s.ShortName solicitante,r.ShortName tramitador,m.descripcion motivo,se.comentario_usu comentarios,e.ShortDesc equipo from Solicitud_Entre_Turno se inner join USUARIO r on se.usu_reg=r.codigousuario inner join USUARIO s on se.solicitante=s.codigousuario inner join Motivo_Entre_Turno m on se.codigomotivo=m.idMotivo inner join EQUIPO e on se.codigoequipo_reg=e.codigoequipo where m.tipo=0 and se.isaprobado is null and convert(date,se.fecha_reg)=convert(date,GETDATE())";
            //tipo de motivo 0 medico y 1 administrativo
            List<SolicitudesEntreTurno> lista = new List<SolicitudesEntreTurno>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        lista.Add(new SolicitudesEntreTurno
                        {
                            idSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                            hora = Convert.ToDateTime(reader["hora"]),
                            tipo = Convert.ToInt32(reader["tipo"]),
                            solicitante = reader["solicitante"].ToString(),
                            tramitador = reader["tramitador"].ToString(),
                            motivo = (reader["motivo"]).ToString(),
                            comentarios = (reader["comentarios"]).ToString(),
                            equipo = (reader["equipo"]).ToString(),
                        });
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }

            return View(lista);
        }
        public ActionResult DetalleEntreTurno(string id)
        {
            string queryString = "select se.IdSolicitud,se.hora_reg hora,m.tipo,s.ShortName solicitante,r.ShortName tramitador,m.descripcion motivo,se.comentario_usu comentarios,e.ShortDesc equipo,se.codigoequipo_reg from Solicitud_Entre_Turno se inner join USUARIO r on se.usu_reg=r.codigousuario inner join USUARIO s on se.solicitante=s.codigousuario inner join Motivo_Entre_Turno m on se.codigomotivo=m.idMotivo inner join EQUIPO e on se.codigoequipo_reg=e.codigoequipo where se.IdSolicitud=" + id;
            //tipo de motivo 0 medico y 1 administrativo
            SolicitudesEntreTurno item = new SolicitudesEntreTurno();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        item.idSolicitud = Convert.ToInt32(reader["IdSolicitud"]);
                        item.hora = Convert.ToDateTime(reader["hora"]);
                        item.tipo = Convert.ToInt32(reader["tipo"]);
                        item.solicitante = reader["solicitante"].ToString();
                        item.tramitador = reader["tramitador"].ToString();
                        item.motivo = (reader["motivo"]).ToString();
                        item.comentarios = (reader["comentarios"]).ToString();
                        item.equipo = (reader["equipo"]).ToString();
                        item.equipomedico = Convert.ToInt32(reader["codigoequipo_reg"].ToString());
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            ViewBag.Equipo = new SelectList((from eq in db.EQUIPO
                                             where eq.estado == "1" &&
                                             (user.sucursales_int).Contains(eq.codigounidad2 * 100 + eq.codigosucursal2) //sucursales asignadas

                                             select eq).AsQueryable(), "codigoequipo", "nombreequipo");
            return View(item);
        }
        public ActionResult getTurnos(string equipo, string hour)
        {
            List<Turno_Horario> listaCarta = new List<Turno_Horario>();
            string queryString = @"SELECT top(10) h.codigohorario AS Id,CONVERT(CHAR(5),h.hora,108)AS Hora,h.bloquear AS E,ISNULL(p.apellidos,'') paciente,ISNULL(es.nombreestudio,'') estudio,h.turnomedico AS Turno FROM HORARIO h left join PACIENTE p on h.codigopaciente=p.codigopaciente left join ESTUDIO es on h.codigoestudio=es.codigoestudio  WHERE h.codigoequipo='{0}' AND DAY(h.fecha)='{1}' AND MONTH(h.fecha)='{2}' AND YEAR(h.fecha)='{3}'  and h.bloquear<>1 and convert(time,h.hora)>convert(time,'{4}:00') ORDER BY h.hora;";
            using (DATABASEGENERALEntities db = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(string.Format(queryString, equipo, DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, hour), connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            listaCarta.Add(new Turno_Horario
                            {
                                codigohorario = Convert.ToInt32((reader["id"]).ToString()),
                                hora = Convert.ToDateTime((reader["hora"]).ToString()),
                                isBloqueado = Convert.ToBoolean((reader["e"]).ToString()),
                                paciente = ((reader["paciente"]).ToString()),
                                estudio = ((reader["estudio"]).ToString()),
                                turno = ((reader["turno"]).ToString())
                            });
                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
                string formato = @"
                <tr>
                    <td> {0}</td>
                    <td> {1}</td>
                </tr>";
                string result = "";
                foreach (var item in listaCarta)
                {
                    result += string.Format(formato, item.hora.ToString("hh:mmtt"), item.estudio);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult setSolicitudEntreTurno(int id, bool e, int eq, int h, int m, string c)
        {
            string queryString = "select top(1)* from HORARIO where codigoequipo=" + eq + " and CONVERT(date,fecha)=convert(date,'" + DateTime.Now.ToShortDateString() + "') and CONVERT(time,hora)<CONVERT(time,'" + h + ":" + m + "') order by codigohorario desc";
            var turno = ""; var unidad = 0;
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        turno = ((reader["turnomedico"]).ToString());
                        unidad = Convert.ToInt32((reader["codigounidad"]).ToString());
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool result = false;
            var solicitud = db.Solicitud_Entre_Turno.SingleOrDefault(x => x.IdSolicitud == id);
            if (solicitud != null)
            {
                solicitud.isaprobado = e;
                solicitud.comentario_med = c;
                solicitud.fec_aprueba = DateTime.Now;
                solicitud.usu_med = user.ProviderUserKey.ToString();
                if (e)
                {
                    var f = DateTime.Now;
                    solicitud.codigoequipo_med = eq;
                    solicitud.hora_med = new DateTime(f.Year, f.Month, f.Day, h, m, 0);
                    //creacion de entreturno


                    HORARIO horario = new HORARIO();
                    horario.atendido = false;
                    horario.confirmado = false;
                    horario.hora = solicitud.hora_med.Value;
                    horario.fecha = solicitud.hora_med.Value;
                    horario.turnomedico = turno;
                    horario.bloquear = false;
                    horario.codigounidad = unidad;
                    horario.codigoequipo = eq;
                    horario.IsEntre = true;
                    db.HORARIO.Add(horario);

                }
                db.SaveChanges();
                var usuario = db.USUARIO.Where(x => x.codigousuario == solicitud.usu_reg).SingleOrDefault();
                if (usuario != null)
                {
                    if (usuario.EMPLEADO != null)
                        if (usuario.EMPLEADO.email != null)
                        {
                            string _msj = @" <h1 style='Margin-top: 0;color: #565656;font-weight: 700;font-size: 20px;Margin-bottom: 18px;font-family: Tahoma,sans-serif;line-height: 44px'>
                                Estimado(a):" + usuario.ShortName + @"</h1>
                                   <p>La Solicitud de Entre Turno  <b>" + (e ? "fue APROBADA" : "NO fue APROBADA") + "</b> por el médico " + user.UserName + " para la hora: " + solicitud.hora_med.Value.ToShortTimeString() + ". </p>";
                            new Variable().sendCorreo("Respuesta Solicitud Entre Turno", usuario.EMPLEADO.email, "", new Variable().CuerpoMensaje(_msj), "");
                        }
                }

                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
                               isequipoSearchable && c.nom_equipo.ToLower().Contains(param.sSearch.ToLower())
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
            var iscondicionSortable = Convert.ToBoolean(Request["bSortable_6"]);
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
                    sortColumnIndex == 6 && iscondicionSortable ? c.condicion :
                    sortColumnIndex == 7 && isestudioSortable ? c.nom_estudio :
                    sortColumnIndex == 8 && isequipoSortable ? c.nom_equipo :
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
                   Convert.ToString(c.min_transcurri),
                   c.condicion,
                   c.nom_estudio,
                   c.nom_equipo,
                    Convert.ToString(c.estado) ,
                   Convert.ToString(c.num_examen),
                   Convert.ToString(c.isVIP),
                   Convert.ToString(c.isSedacion ),
                   Convert.ToString(c.isAsignado ),
                   Convert.ToString(c.docEscan),
                   Convert.ToString(c.isEncuesta),
                   Convert.ToString(c.docCarta),
                   ""
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

        //metodo que obtiene la data
        private IList<EncuestaExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            bool sedador = User.IsInRole("5");
            IList<EncuestaExamenViewModel> allSupervisiones = (from ea in db.EXAMENXATENCION
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

                                                               orderby ea.codigo
                                                               select new EncuestaExamenViewModel
                                                               {
                                                                   num_examen = ea.codigo,
                                                                   tipo_encuesta = en.tipo_encu,
                                                                   isFinEncuesta = en.fec_paso3 != null ? true : false,
                                                                   paciente = p.apellidos + " " + p.nombres,
                                                                   sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc,
                                                                   hora_cita = ea.horaatencion,
                                                                   hora_admision = ea.ATENCION.fechayhora,
                                                                   min_transcurri =
                                   SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                   SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                   condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
                                                                   nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                   nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
                                                                   isSedacion = ea.ATENCION.CITA.sedacion == null ? false : ea.ATENCION.CITA.sedacion,
                                                                   isVIP = ea.ATENCION.CITA.citavip == null ? false : ea.ATENCION.CITA.citavip,
                                                                   estado = (en.estado),
                                                                   isAsignado = ea.equipoAsignado != null ? true : false,
                                                                   isEncuesta = en.fec_paso3 != null ? true : false,
                                                                   docEscan = esc.numerodeatencion != null ? true : false,
                                                                   docCarta = ca.codigodocadjunto != null ? true : false,


                                                               }).AsParallel().ToList();

            return allSupervisiones;
        }

        //lista de equipos asignados por sucursal
        public ActionResult getSedes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            return Json(new SelectList((from su in db.SUCURSAL
                                        where (user.sucursales_int).Contains
                                        (su.codigounidad * 100 + su.codigosucursal) //sucursales asignadas
                                        select new { codigo = (su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc) }).AsQueryable(),
                                         "codigo",
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
        #region Calificacion Paciente
        public ActionResult ListaCalificacion()
        {
            return View();
        }
        public ActionResult ListaCalificacionAsync(JQueryDataTableParamModel param)
        {

            var listaEncuestas = getAdmisiones();

            IEnumerable<CalificacionPacienteViewModels> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar

                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);

                //Optionally check whether the columns are searchable at all 

                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);


                listaEncuestasFilter = getAdmisiones()
                   .Where(c => ispacienteSearchable && c.paciente.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }


            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<CalificacionPacienteViewModels, string> orderingFunction = (
                c => sortColumnIndex == 1 && ispacienteSortable ? c.paciente :

                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);



            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                   c.atencion.ToString(),
                   c.paciente.ToString(),
                   "<button type='button' class='btn btn-success' onclick='abrirEncuesta("+c.atencion.ToString()+")'><i class='fa fa-edit fa-1x'></i> Calificar</button>"
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

        private IList<CalificacionPacienteViewModels> getAdmisiones()
        {
            List<CalificacionPacienteViewModels> lista = new List<CalificacionPacienteViewModels>();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                string queryString = @"select max(a.numeroatencion) atencion,p.apellidos+' '+p.nombres paciente 
from EXAMENXATENCION ea 
inner join ATENCION a on ea.numeroatencion = a.numeroatencion
inner join PACIENTE p on ea.codigopaciente = p.codigopaciente
inner join HONORARIOTECNOLOGO h on ea.codigo=h.codigohonorariotecnologo
left join Calificacion_Paciente c on a.numeroatencion=c.numeroatencion

where convert(date,a.fechayhora)=convert(date,getdate())
and ea.estadoestudio='P'
and SUBSTRING(ea.codigoestudio,1,3) in (SELECT convert(varchar(10), splitdata) FROM fnSplitString('" + String.Join(",", user.sucursales.ToArray())+ @"', ','))
and c.numeroatencion is null
and convert(time,h.horaestudio) between  convert(varchar(20),DATEADD(minute,-30,getdate()),108) and convert(varchar(20),getdate(),108)
--and ea.numeroatencion=589987

group by p.apellidos,p.nombres
order by p.apellidos";

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        lista.Add(new CalificacionPacienteViewModels
                        {
                            atencion = Convert.ToInt32(reader["atencion"].ToString()),
                            paciente = (reader["paciente"].ToString())
                        });
                    }
                }
                catch (Exception) { }
                finally
                {
                    reader.Close();
                    connection.Dispose();
                    connection.Close();
                }

            }
            return lista;
        }
        public ActionResult CalificarAtencion(int atencion)
        {
            Calificacion_Paciente item = new Calificacion_Paciente();
            item.numeroatencion = atencion;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalificarAtencion(Calificacion_Paciente item)
        {
            try
            {
                item.fecha = DateTime.Now;
                db.Calificacion_Paciente.Add(item);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex = ex;
            }
            
            return View("ListaCalificacion");
        }


        public ActionResult ReporteCalificarAtencion(DateTime inicio, DateTime fin)
        {
            List<CalificacionPacienteViewModels> lista = new List<CalificacionPacienteViewModels>();
            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                string queryString = @"select * from Calificacion_Paciente cp where convert(date,cp.fecha) between '" + inicio.ToString("dd/MM/yyyy") + "' and '" + fin.ToString("dd/MM/yyyy") + "'";

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.Prepare();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        lista.Add(new CalificacionPacienteViewModels
                        {
                            atencion = Convert.ToInt32(reader["numeroatencion"].ToString()),
                            p1 = Convert.ToInt32(reader["p1"].ToString()),
                            p2 = Convert.ToInt32(reader["p2"].ToString()),
                            p3 = Convert.ToInt32(reader["p3"].ToString()),
                            p4 = Convert.ToInt32(reader["p4"].ToString()),
                            p5 = Convert.ToInt32(reader["p5"].ToString()),
                            p6 = Convert.ToInt32(reader["p6"].ToString()),
                            p7 = Convert.ToInt32(reader["p7"].ToString()),
                            p8 = Convert.ToInt32(reader["p8"].ToString())
                        });
                    }
                }
                catch (Exception) { }
                finally
                {
                    reader.Close();
                    connection.Dispose();
                    connection.Close();
                }

            }
            return View(lista);
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}








