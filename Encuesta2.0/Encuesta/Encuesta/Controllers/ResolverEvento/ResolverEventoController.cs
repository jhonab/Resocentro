using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels;
using Encuesta.ViewModels.Evento;
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
    public class ResolverEventoController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        public ActionResult ListaEsperaEvento()
        {
            return View();
        }

        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var listaEncuestas = getExamenes();
            IEnumerable<EventoViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var codigoFilter = Convert.ToString(Request["sSearch_0"]);
                var tecnoFilter = Convert.ToString(Request["sSearch_1"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_2"]);
                var estudioFilter = Convert.ToString(Request["sSearch_3"]);
                var equipoFilter = Convert.ToString(Request["sSearch_4"]);
                var eventoFilter = Convert.ToString(Request["sSearch_5"]);
                var notaFilter = Convert.ToString(Request["sSearch_5"]);

                //Optionally check whether the columns are searchable at all 
                var iscodigoSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var istecnoSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_4"]);
                var iseventoSearchable = Convert.ToBoolean(Request["bSearchable_5"]);
                var isnotaSearchable = Convert.ToBoolean(Request["bSearchable_6"]);



                listaEncuestasFilter = getExamenes()
                    .Where(c => iscodigoSearchable && c.codigo.ToString().Contains(param.sSearch.ToLower())
                               ||
                               istecnoSearchable && c.TecnoReg.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.paciente.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isequipoSearchable && c.equipo.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               iseventoSearchable && c.evento.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isnotaSearchable && c.nota.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var iscodigoSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var isfecnewSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var istecnoSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var iseventoSortable = Convert.ToBoolean(Request["bSortable_6"]);
            var isnotaSortable = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EventoViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && iscodigoSortable ? c.codigo.ToString() :
                    sortColumnIndex == 1 && ispacienteSortable ? c.paciente.ToString() :
                    sortColumnIndex == 2 && isestudioSortable ? c.estudio.ToString() :
                     sortColumnIndex == 3 && isequipoSortable ? c.equipo.ToString() :
                    sortColumnIndex == 4 && isfecnewSortable ? c.FecNew.ToString() :
                    sortColumnIndex == 5 && istecnoSortable ? c.TecnoReg.ToString() :
                    sortColumnIndex == 6 && iseventoSortable ? c.evento :
                    sortColumnIndex == 7 && isnotaSortable ? c.nota :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                    Convert.ToString(c.codigo),//0
                     Convert.ToString(c.paciente),
                      Convert.ToString(c.estudio),
                       Convert.ToString(c.equipo),
                    Convert.ToString(c.FecNew),//1
                    Convert.ToString(c.TecnoReg),//2
                    Convert.ToString(c.evento),//3
                     Convert.ToString(c.nota),//3
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

        private List<EventoViewModel> getExamenes()
        {
            //CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            //IList<ExamenXEvento> allSupervisiones = (from ee in db.ExamenXEvento
            //                                         join u in db.USUARIO on ee.codigousuario equals u.codigousuario into                                   u_join
            //                                                    from e in u_join.DefaultIfEmpty()

            //                                         where ee.TecnoReg == user.ProviderUserKey &&
            //                                                      ee.EstadoExamen == "P" && 
            //                                                      ee.FecNew => "15/03/2016"
                                                          
            //                                                    select new ExamenXEvento
            //                                                    {
            //                                                        idcontraste = co.idcontraste,
            //                                                        num_examen = ea.codigo,
            //                                                        nom_paciente = p.apellidos + " " + p.nombres,
            //                                                        sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc,
            //                                                        hora_cita = ea.horaatencion,
            //                                                        hora_admision = ea.ATENCION.fechayhora,
            //                                                        min_transcurri =
            //                                                        SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
            //                                                        (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
            //                                                        SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
            //                                                        (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") :
            //                                                        SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
            //                                                        condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
            //                                                        nom_estudio = ea.ESTUDIO.nombreestudio,
            //                                                        nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
            //                                                        isSedacion = ea.ATENCION.CITA.sedacion,
            //                                                        isVIP = ea.ATENCION.CITA.citavip,
            //                                                        enfermera = co.USUARIO2.ShortName
            //                                                    }).AsParallel().ToList();

            //return allSupervisiones;


            List<EventoViewModel> lista = new List<EventoViewModel>();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            string sqlPost = @" select ee.codigo as codigo,p.apellidos + '' + p.nombres as paciente, es.nombreestudio as estudio , eq.nombreequipo, ee.FecNew as fec, u.ShortName as nom, e.Nombre as evento, ee.Nota from ExamenXEvento ee inner join USUARIO u on ee.codigousuario = u.codigousuario inner join Evento e on ee.EventoId = e.EventoId inner join EXAMENXATENCION ea on ee.codigo = ea.codigo inner join PACIENTE p on ea.codigopaciente = p.codigopaciente inner join ESTUDIO es on ea.codigoestudio = es.codigoestudio inner join EQUIPO eq on ea.equipoAsignado = eq.codigoequipo where ee.TecnoFin is null and month(ee.FecNew) = month(GETDATE()) and year(ee.FecNew) = year(GETDATE())";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_evento = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_evento.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new EventoViewModel();
                        item.codigo = Convert.ToInt32(reader["codigo"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.estudio = (reader["estudio"]).ToString();
                        item.equipo = (reader["nombreequipo"]).ToString();
                        item.FecNew = Convert.ToDateTime(reader["fec"]);
                        item.TecnoReg = (reader["nom"]).ToString();
                        item.evento = (reader["evento"]).ToString();
                        item.nota = (reader["nota"]).ToString();
                        lista.Add(item);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

            }
            return lista;

        }

        public ActionResult InsertarSolucion(int codigo, string notaFinal)
        {
            var evento = db.ExamenXEvento.SingleOrDefault(x => x.codigo == codigo);

            if (evento != null)
            {
                //var examen = db.EXAMENXATENCION.Where(x => x.codigo == codigo).Single();
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                #region Agregar Tabla Evento
                //ExamenXEvento ev = new ExamenXEvento();


                evento.TecnoFin = user.ProviderUserKey.ToString();
                evento.TecnoFecFin = DateTime.Now;
                evento.TecnoFinNotas = notaFinal;
                //db.ExamenXEvento.Add(ev);
                db.SaveChanges();
                #endregion
                return Json(true);
            }
            else
            {
                return Json(false);
            }

            //return RedirectToAction("ListaEsperaEvento");
        }


        public string Contador()
        {
           var cont = 0;
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            string sqlPost = @"select count(*) as total from ExamenXEvento where TecnoFin is null and month(FecNew) = month(GETDATE()) and year(FecNew) = year(GETDATE())";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_evento = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_evento.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        cont = Convert.ToInt32(reader["total"]);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

            }
            return "Médicos: " + cont.ToString();

        }

        public ActionResult ContadorEventos()
        {

            return Json(Contador());
             
            //return RedirectToAction("ListaEsperaEvento");
        }


    }
}
