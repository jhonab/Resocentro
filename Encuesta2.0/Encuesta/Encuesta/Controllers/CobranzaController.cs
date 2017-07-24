using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Tecnologo;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers
{
    public class CobranzaController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /Cobranza/

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
                               isequipoSearchable && c.nom_equipo_pro.ToLower().Contains(param.sSearch.ToLower())
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
                Convert.ToString(c.num_examen),
                c.nom_paciente,
                (c.hora_cita).ToShortTimeString(),
                Convert.ToString(c.hora_admision),
                Convert.ToString(c.min_transcurri),
                c.condicion,
                c.nom_estudio,
                c.nom_equipo_pro,
                Convert.ToString(c.num_examen),
                Convert.ToString(c.isVIP),
                Convert.ToString(c.isValidado),
                Convert.ToString(c.isValidacion),
                Convert.ToString(c.isSedacion ),
            Convert.ToString(c.isIniciado),
            Convert.ToString(c.isActivado),
            Convert.ToString(c.minrestante),
               Convert.ToString(c.isContraste ),
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
        private IList<RealizarExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Variable _u = new Variable();
            /*IList<RealizarExamenViewModel> allCompanies = (from ea in db.EXAMENXATENCION

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
                                                               hora_admision = ea.ATENCION.fechayhora,
                                                               min_transcurri =
                                    SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                    SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                               condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
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
            return allCompanies;
        }

    }
}
