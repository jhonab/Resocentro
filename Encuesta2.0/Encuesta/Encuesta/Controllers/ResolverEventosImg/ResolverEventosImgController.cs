using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels;
using Encuesta.ViewModels.Img;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.ResolverEventosImg
{
    public class ResolverEventosImgController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        public ActionResult ListaEsperaEventoImg()
        {
            return View();
        }

        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var listaEncuestas = getExamenes();
            IEnumerable<ImgViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var codigoFilter = Convert.ToString(Request["sSearch_0"]);
                var tecnoFilter = Convert.ToString(Request["sSearch_1"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_2"]);
                var estudioFilter = Convert.ToString(Request["sSearch_3"]);
                var eventoFilter = Convert.ToString(Request["sSearch_4"]);

                //Optionally check whether the columns are searchable at all 
                var iscodigoSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var istecnoSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
                var iseventoSearchable = Convert.ToBoolean(Request["bSearchable_4"]);



                listaEncuestasFilter = getExamenes()
                    .Where(c => iscodigoSearchable && c.codigo.ToString().Contains(param.sSearch.ToLower())
                               ||
                               istecnoSearchable && c.TecnoReg.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.paciente.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isestudioSearchable && c.estudio.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               iseventoSearchable && c.evento.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var iscodigoSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isfecnewSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var istecnoSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var iseventoSortable = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ImgViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && iscodigoSortable ? c.codigo.ToString() :
                    sortColumnIndex == 1 && ispacienteSortable ? c.paciente.ToString() :
                    sortColumnIndex == 2 && isestudioSortable ? c.estudio.ToString() :
                    sortColumnIndex == 3 && isfecnewSortable ? c.FecNew.ToString() :
                    sortColumnIndex == 4 && istecnoSortable ? c.TecnoReg.ToString() :
                    sortColumnIndex == 5 && iseventoSortable ? c.evento :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             Convert.ToString(c.FecNew),
                    Convert.ToString(c.codigo),//0
                     Convert.ToString(c.paciente),
                      Convert.ToString(c.estudio),
                    //1
                    Convert.ToString(c.TecnoReg),//2
                    Convert.ToString(c.evento),//3
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


        private List<ImgViewModel> getExamenes()
        {

            List<ImgViewModel> lista = new List<ImgViewModel>();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            string sqlPost = @" select a.fechayhora,
ea.numeroatencion,
pa.apellidos + ' ' + pa.nombres as paciente,
eq.nombreequipo,
(case when count(UrlXero) = 0 then 'Revisar en el IMPAX' end)  mensaje,
sum(convert(int,ea.estadoImagen)) as estado 
 from EXAMENXATENCION ea
inner join ATENCION a on ea.numeroatencion = a.numeroatencion 
inner join PACIENTE pa on ea.codigopaciente = pa.codigopaciente 
inner join EQUIPO eq on (case when ea.equipoAsignado is null then ea.codigoequipo else ea.equipoAsignado end) = eq.codigoequipo
where convert(date,a.fechayhora) = convert(date,GETDATE()-1) 
 and a.codigounidad = 1 and ea.estadoestudio != 'X' group by ea.numeroatencion,pa.apellidos,pa.nombres,a.fechayhora,eq.nombreequipo  having count(UrlXero) = 0  and sum(convert(int,ea.estadoImagen)) = 0 order by ea.numeroatencion
";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_evento = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_evento.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new ImgViewModel();
                        item.codigo = Convert.ToInt32(reader["numeroatencion"]);
                        item.paciente = (reader["paciente"]).ToString();
                        item.estudio = (reader["nombreequipo"]).ToString();
                        item.FecNew = Convert.ToDateTime(reader["fechayhora"]);
                        //item.TecnoReg = (reader["ShortName"]).ToString();
                        item.evento = (reader["mensaje"]).ToString();
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


        public ActionResult InsertarSolucionIMG(int atencion, string notaFinal)
        {
            var evento = db.EventosImpax.SingleOrDefault(x => x.atencion == atencion);
            var atencionZ = db.EXAMENXATENCION.Where(x => x.numeroatencion == atencion && x.estadoImagen == false).FirstOrDefault();
            //var codigo = db.EXAMENXATENCION.Where(x => x.codigo ==  Convert.ToInt32(atencionZ));
            if (evento == null)
            {
                //var examen = db.EXAMENXATENCION.Where(x => x.codigo == codigo).Single();
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                #region Agregar Tabla Evento
                EventosImpax ev = new EventosImpax();
                ev.atencion = atencion;
                ev.TecnoFin = user.ProviderUserKey.ToString();
                ev.TecnoFecFin = DateTime.Now;
                ev.TecnoFinNotas = notaFinal;
                db.EventosImpax.Add(ev);
                //db.ExamenXEvento.Add(ev);
                atencionZ.estadoImagen = true;
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


        public string ContadorCasos()
        {
            var cont = 0;
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            string sqlPost = @"select count(a.total) as totalF from
(select 
count(*) as total, ea.numeroatencion
 from EXAMENXATENCION ea
inner join ATENCION a on ea.numeroatencion = a.numeroatencion 
inner join PACIENTE pa on ea.codigopaciente = pa.codigopaciente 
inner join EQUIPO eq on (case when ea.equipoAsignado is null then ea.codigoequipo else ea.equipoAsignado end) = eq.codigoequipo
where convert(date,a.fechayhora) = convert(date,GETDATE()-1) --and ea.estadoImagen = 0
and a.codigounidad = 1 and ea.estadoestudio != 'X' group by ea.numeroatencion,pa.apellidos,pa.nombres,a.fechayhora,eq.nombreequipo 
having count(UrlXero) = 0  and sum(convert(int,ea.estadoImagen)) = 0 ) a";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_evento = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_evento.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        cont = Convert.ToInt32(reader["totalF"]);
                    }
                }
                finally
                {
                    reader.Close();
                    connection.Close();
                }

            }
            return "IMPAX: " + cont.ToString();

        }

        public ActionResult ContadorEventosImg()
        {

            return Json(ContadorCasos());

            //return RedirectToAction("ListaEsperaEvento");
        }


        public string ContadorM()
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

        public ActionResult ContadorEventosM()
        {

            return Json(ContadorM());

            //return RedirectToAction("ListaEsperaEvento");
        }

    }
}
