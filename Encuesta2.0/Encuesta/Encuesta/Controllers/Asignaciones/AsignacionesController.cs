using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Asignaciones;
using Encuesta.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;

namespace Encuesta.Controllers.Asignaciones
{
    public class AsignacionesController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();

        public ActionResult AsignacionesInsti_Prom_Med()
        {
            return View();
        }

        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var listaEncuestas = getExamenes();
            IEnumerable<AsignacionesViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var codcliFilter = Convert.ToString(Request["sSearch_0"]);
                var nomcliFilter = Convert.ToString(Request["sSearch_1"]);
                var codproFilter = Convert.ToString(Request["sSearch_2"]);
                var nomproFilter = Convert.ToString(Request["sSearch_3"]);
                var cmpmedFilter = Convert.ToString(Request["sSearch_4"]);
                var nommedFilter = Convert.ToString(Request["sSearch_5"]);

                //Optionally check whether the columns are searchable at all 
                var iscodcliSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var isnomcliSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var iscodproSearchable = Convert.ToBoolean(Request["bSearchable_2"]);
                var isnomproSearchable = Convert.ToBoolean(Request["bSearchable_3"]);
                var iscmpmedSearchable = Convert.ToBoolean(Request["bSearchable_4"]);
                var isnommedSearchable = Convert.ToBoolean(Request["bSearchable_5"]);



                listaEncuestasFilter = getExamenes()
                    .Where(c => iscodcliSearchable && c.codigo_cli.ToString().Contains(param.sSearch.ToLower())
                               ||
                               isnomcliSearchable && c.nombre_cli.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               iscodproSearchable && c.codigo_pro.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isnomproSearchable && c.nombre_pro.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               iscmpmedSearchable && c.cmp_med.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isnommedSearchable && c.nombre_med.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                listaEncuestasFilter = listaEncuestas;
            }

            var iscodcliSortable = Convert.ToBoolean(Request["bSortable_0"]);
            var isnomcliSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var iscodproSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isnomproSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var iscmpmedSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isnommedSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isestadoSortable = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AsignacionesViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && iscodcliSortable ? c.codigo_cli.ToString() :
                    sortColumnIndex == 1 && isnomcliSortable ? c.nombre_cli.ToString() :
                    sortColumnIndex == 2 && iscodproSortable ? c.codigo_pro.ToString() :
                    sortColumnIndex == 3 && isnomproSortable ? c.nombre_pro.ToString() :
                    sortColumnIndex == 4 && iscmpmedSortable ? c.cmp_med.ToString() :
                    sortColumnIndex == 5 && isnommedSortable ? c.nombre_med.ToString() :
                    sortColumnIndex == 6 && isestadoSortable ? c.estado.ToString() :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                             Convert.ToString(c.codigo_cli),
                    Convert.ToString(c.nombre_cli),//0
                    Convert.ToString(c.codigo_pro),
                    Convert.ToString(c.nombre_pro),//1
                    Convert.ToString(c.cmp_med),//2
                    Convert.ToString(c.nombre_med),//3
                    Convert.ToString(c.estado),//3
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

        private List<AsignacionesViewModel> getExamenes()
        {

            List<AsignacionesViewModel> lista = new List<AsignacionesViewModel>();
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;

            string sqlPost = @" select cod_clinica,nom_clinica,cod_promotor,nom_promotor,cmp_medico,nom_medico, fec_asig,usu_asig,case when estado = 0 then 'Inactivo' else 'Activo' end estado from Insti_Prom_Medico";

            using (SqlConnection connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command_evento = new SqlCommand(sqlPost, connection);
                connection.Open();
                SqlDataReader reader = command_evento.ExecuteReader();
                try
                {

                    while (reader.Read())
                    {
                        var item = new AsignacionesViewModel();
                        item.codigo_cli = (reader["cod_clinica"]).ToString();
                        item.nombre_cli = (reader["nom_clinica"]).ToString();
                        item.codigo_pro = (reader["cod_promotor"]).ToString();
                        item.nombre_pro = (reader["nom_promotor"]).ToString();
                        item.cmp_med = (reader["cmp_medico"]).ToString();
                        item.nombre_med = (reader["nom_medico"]).ToString();
                        item.estado = (reader["estado"]).ToString();
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


        public JsonResult getinstituciones()
        {

            return Json(new SelectList((from a in db.CLINICAHOSPITAL
                                        where a.IsEnabled == true
                                        orderby a.razonsocial
                                        select new { codigo = (a.codigoclinica), nom = (a.razonsocial) }).AsQueryable(),
                                       "codigo",
                                       "nom"),
                                       JsonRequestBehavior.AllowGet);

        }


        public JsonResult getpromotores()
        {
            return Json(new SelectList((from a in db.USUARIO
                                        where
                        a.EMPLEADO.codigocargo == 4 &&
                        a.bloqueado == false
                                        orderby a.ShortName
                                        select new { codigo = (a.codigousuario), nom = (a.ShortName) }).AsQueryable(),
                                       "codigo",
                                       "nom"),
                                       JsonRequestBehavior.AllowGet);
        }

        public JsonResult getmedicos()
        {
            return Json(new SelectList((from a in db.MEDICOEXTERNO
                                        where
                                        a.isactivo == true
                                        orderby a.apellidos
                                        select new { codigo = (a.cmp), nom = (a.apellidos + " " + a.nombres) }).AsQueryable(),
                                      "codigo",
                                      "nom"),
                                      JsonRequestBehavior.AllowGet);

        }


        public ActionResult RegistrarAsignación(string codcli, string nomcli,string codpro, string nompro, string cmpmed, string nommed)
        {
            var asig = db.Insti_Prom_Medico.SingleOrDefault(x => x.cod_clinica == codcli && x.cod_promotor == codpro && x.cmp_medico == cmpmed);
            //var atencionZ = db.EXAMENXATENCION.Where(x => x.numeroatencion == atencion).FirstOrDefault();
            //var codigo = db.EXAMENXATENCION.Where(x => x.codigo ==  Convert.ToInt32(atencionZ));
            if (asig == null)
            {
                //var examen = db.EXAMENXATENCION.Where(x => x.codigo == codigo).Single();
                CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                #region Agregar Tabla Evento
                Insti_Prom_Medico a = new Insti_Prom_Medico();
                a.cod_clinica = codcli;
                a.nom_clinica = nomcli;
                a.cod_promotor = codpro;
                a.nom_promotor = nompro;
                a.cmp_medico = cmpmed;
                a.nom_medico = nommed;
                a.fec_asig = DateTime.Now;
                a.usu_asig = user.ProviderUserKey.ToString();
                a.estado = true;
                db.Insti_Prom_Medico.Add(a);
                //db.ExamenXEvento.Add(ev);
                //atencionZ.estadoImagen = true;
                db.SaveChanges(); 
                #endregion
                return Json(true);
            }
            else
            {
                return Json(false);
            }

            //return RedirectToAction("AsignacionesInsti_Prom_Med");
        }

        public ActionResult CambiarEstado(string idcli, string idpro, string idmed)
        {
            var _asig = db.Insti_Prom_Medico.SingleOrDefault(x => x.cod_clinica == idcli && x.cod_promotor == idpro && x.cmp_medico == idmed);

            if (_asig != null)
            {
                if (_asig.estado == true)
                {
                    _asig.estado = false;
                }
                else
                {
                    _asig.estado = true;
                }

                db.SaveChanges();

                return Json(true);
            }
            else
            {
                return Json(false);
            }
           
        }

    }
}
