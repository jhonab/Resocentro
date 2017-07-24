using Encuesta.Member;
using Encuesta.Models;
using Encuesta.ViewModels.Supervisor;
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
    [Authorize(Roles = "7")]
    public class SatisfaccionController : Controller
    {
        //
        // GET: /Satisfaccion/
        DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        public ActionResult ListaEspera()
        {

            return View();
        }
        //lista de examenes pendiestes
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var allCompanies = getExamenes();
            IEnumerable<SatisfaccionViewModel> filteredCompanies;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 
                var pacienteFilter = Convert.ToString(Request["sSearch_0"]);

                //Optionally check whether the columns are searchable at all 
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_0"]);


                filteredCompanies = getExamenes()
                   .Where(c => ispacienteSearchable && c.nom_paciente.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = allCompanies;
            }

            var ispacienteSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SatisfaccionViewModel, string> orderingFunction = (c => sortColumnIndex == 0 && ispacienteSortable ? c.nom_paciente.ToString() : "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredCompanies = filteredCompanies.OrderBy(orderingFunction);
            else
                filteredCompanies = filteredCompanies.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                   Convert.ToString(c.atencion),
                   c.nom_paciente,
                   Convert.ToString(c.hora_admision),
                   Convert.ToString(c.min_transcurri),
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
        private IList<SatisfaccionViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            IList<SatisfaccionViewModel> lista = (from sa in db.Encuesta_Satisfaccion
                                                  join a in db.ATENCION on sa.numeroatecion equals a.numeroatencion
                                                  where sa.isTerminado == null
                                                  orderby a.numeroatencion ascending
                                                  select new SatisfaccionViewModel
                                                      {
                                                          nom_paciente = sa.PACIENTE1.apellidos + " " + sa.PACIENTE1.nombres,
                                                          atencion = a.numeroatencion,
                                                          hora_admision = a.fechayhora,
                                                          min_transcurri =
                        SqlFunctions.DateDiff("month", sa.fec_inicio, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", sa.fec_inicio, SqlFunctions.GetDate())) + " mes(es)") :
                        SqlFunctions.DateDiff("day", sa.fec_inicio, SqlFunctions.GetDate()) > 0 ? (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", sa.fec_inicio, SqlFunctions.GetDate())) + " día(s)") : SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", sa.fec_inicio, SqlFunctions.GetDate()))
                                                      }).ToList();
            return lista;
        }

        public ActionResult SatisfaccionExamen(int atencion)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Encuesta_Satisfaccion model = db.Encuesta_Satisfaccion.Where(x => x.numeroatecion == atencion).SingleOrDefault();

            if (model != null)
            {
                model.fec_ini = DateTime.Now;
                model.usu_reg = user.ProviderUserKey.ToString();
                return View(model);
            }
            return RedirectToAction("ListaEspera");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SatisfaccionExamen(Encuesta_Satisfaccion item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            Encuesta_Satisfaccion model = db.Encuesta_Satisfaccion.Where(x => x.idencuesta == item.idencuesta).SingleOrDefault();

            if (model != null)
            {
                model.fec_ini = item.fec_ini;
                model.fec_fin = item.fec_fin;
                model.p1 = item.p1;
                model.p2 = item.p2;
                model.p3 = true;
                model.usu_reg = user.ProviderUserKey.ToString();
                model.isTerminado = true;
                db.SaveChanges();
                return RedirectToAction("ListaEspera");
            }
            return View(model);
        }
    }
}
