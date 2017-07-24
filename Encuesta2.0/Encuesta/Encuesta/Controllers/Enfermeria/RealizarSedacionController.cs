using Encuesta.Member;
using Encuesta.Models;
using Encuesta.Util;
using Encuesta.ViewModels.Sedacion;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Encuesta.Controllers.Enfermeria
{
    public class RealizarSedacionController : Controller
    {
        private DATABASEGENERALEntities db = new DATABASEGENERALEntities();
        //
        // GET: /ContrasteExamenViewModel/

        public ActionResult ListaEspera()
        {
            return View();
        }

        //lista de examenes pendiestes async
        public ActionResult ListaAsync(JQueryDataTableParamModel param)
        {


            var listaEncuestas = getExamenes();
            IEnumerable<SedacionExamenViewModel> listaEncuestasFilter;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //determinamos las columnas a filtar
                var examenFilter = Convert.ToString(Request["sSearch_0"]);
                var pacienteFilter = Convert.ToString(Request["sSearch_1"]);
                var enfermeraFilter = Convert.ToString(Request["sSearch_4"]);
                var estudioFilter = Convert.ToString(Request["sSearch_5"]);
                var equipoFilter = Convert.ToString(Request["sSearch_6"]);


                //Optionally check whether the columns are searchable at all 
                var isexamenSearchable = Convert.ToBoolean(Request["bSearchable_0"]);
                var ispacienteSearchable = Convert.ToBoolean(Request["bSearchable_1"]);
                var isenfermeraSearchable = Convert.ToBoolean(Request["bSearchable_4"]);
                var isestudioSearchable = Convert.ToBoolean(Request["bSearchable_5"]);
                var isequipoSearchable = Convert.ToBoolean(Request["bSearchable_6"]);


                listaEncuestasFilter = getExamenes()
                   .Where(c => isexamenSearchable && c.num_examen.ToString().Contains(param.sSearch.ToLower())
                               ||
                               ispacienteSearchable && c.nom_paciente.ToLower().Contains(param.sSearch.ToLower())
                                ||
                               isenfermeraSearchable && c.enfermera.ToLower().Contains(param.sSearch.ToLower())
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
            var ishadminSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isminSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var isenfermeraSortable = Convert.ToBoolean(Request["bSortable_4"]);
            var isestudioSortable = Convert.ToBoolean(Request["bSortable_5"]);
            var isequipoSortable = Convert.ToBoolean(Request["bSortable_6"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SedacionExamenViewModel, string> orderingFunction = (
                c => sortColumnIndex == 0 && isexamenSortable ? c.num_examen.ToString() :
                    sortColumnIndex == 1 && ispacienteSortable ? c.nom_paciente :
                    sortColumnIndex == 2 && ishadminSortable ? c.hora_admision.ToString() :
                    sortColumnIndex == 3 && isminSortable ? c.min_transcurri :
                    sortColumnIndex == 4 && isenfermeraSortable ? c.condicion :
                    sortColumnIndex == 5 && isestudioSortable ? c.nom_estudio :
                    sortColumnIndex == 6 && isequipoSortable ? c.nom_equipo :
                    "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                listaEncuestasFilter = listaEncuestasFilter.OrderBy(orderingFunction);
            else
                listaEncuestasFilter = listaEncuestasFilter.OrderByDescending(orderingFunction);

            var displayedCompanies = listaEncuestasFilter.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                    Convert.ToString(c.atencion),//0
                    c.nom_paciente,//1
                    Convert.ToString(c.hora_admision),//2
                    Convert.ToString(c.min_transcurri),//3
                    c.enfermera,//4
                    c.nom_estudio,//5
                    c.nom_equipo,//6
                    Convert.ToString(c.idsedacion),//7
                    Convert.ToString(c.isVIP),//8
                    Convert.ToString(c.isSedacion ),//9
                    Convert.ToString(c.cantidad),//10
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
        private IList<SedacionExamenViewModel> getExamenes()
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            IList<SedacionExamenViewModel> allSupervisiones = (from ea in db.EXAMENXATENCION
                                                               join e in db.EQUIPO on ea.equipoAsignado equals e.codigoequipo into e_join
                                                               from e in e_join.DefaultIfEmpty()
                                                               join p in db.PACIENTE on ea.codigopaciente equals p.codigopaciente
                                                               join en in db.Encuesta on ea.codigo equals en.numeroexamen
                                                               join se in db.Sedacion on ea.codigo equals se.numeroexamen
                                                               join su in db.SUCURSAL on new { codigounidad = ea.codigoestudio.Substring(0, 1), codigosucursal = ea.codigoestudio.Substring(2, 1) }
                                                               equals new
                                                               {
                                                                   codigounidad = SqlFunctions.StringConvert((double)(su.codigounidad)).Trim(),
                                                                   codigosucursal = SqlFunctions.StringConvert((double)
                                                                       (su.codigosucursal)).Trim()
                                                               }
                                                               where se.estado == 0 &&
                                                               se.usuario == user.ProviderUserKey
                                                               //ea.ATENCION.fechayhora == DateTime.Now &&
                                                               //    (user.sucursales).Contains(ea.codigoestudio.Substring(1, 3)) //sucursales asignadas

                                                               orderby ea.codigo
                                                               select new SedacionExamenViewModel
                                                                {
                                                                    idsedacion = se.idsedacion,
                                                                    num_examen = ea.codigo,
                                                                    atencion=ea.numeroatencion,
                                                                    cantidad=ea.ATENCION.EXAMENXATENCION.Count(),
                                                                    nom_paciente = p.apellidos + " " + p.nombres,
                                                                    sede = su.UNIDADNEGOCIO.nombre + " - " + su.ShortDesc,
                                                                    hora_cita = ea.horaatencion,
                                                                    hora_admision = ea.ATENCION.fechayhora,
                                                                    min_transcurri =
                                                                    SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
                                                                    (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("month", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " mes(es)") :
                                                                    SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate()) > 0 ?
                                                                    (SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("day", ea.ATENCION.fechayhora, SqlFunctions.GetDate())) + " día(s)") :
                                                                    SqlFunctions.StringConvert((Double)SqlFunctions.DateDiff("minute", ea.ATENCION.fechayhora, SqlFunctions.GetDate())),
                                                                    condicion = ea.ATENCION.CITA.categoria == "AMBULATORIO" ? "" : ea.ATENCION.CITA.categoria,
                                                                    nom_estudio = ea.ESTUDIO.nombreestudio,
                                                                    nom_equipo = e.nombreequipo == null ? "No Asignado" : e.nombreequipo,
                                                                    isSedacion = ea.ATENCION.CITA.sedacion,
                                                                    isVIP = ea.ATENCION.CITA.citavip,
                                                                    enfermera = se.USUARIO2.ShortName
                                                                }).AsParallel().ToList();

            return allSupervisiones;
        }


        //
        // GET: /Contraste/Create

        public ActionResult RealizarSedacion(int sedacion)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            SedacionExamenViewModel model = new SedacionExamenViewModel();
            var _sedacion = db.Sedacion.Where(x => x.idsedacion == sedacion).SingleOrDefault();


            var _examen = (from x in db.EXAMENXATENCION where x.codigo == _sedacion.numeroexamen select x).SingleOrDefault();
            var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
            var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
            //Contraste
            model.sedacion = _sedacion;
            model.idsedacion = _sedacion.idsedacion;
            model.insumos = getDetalleInsumos(_sedacion.idsedacion);//db.Detalle_Sedacion.Where(x => x.id_sedacion == sedacion).ToList();
            
            //PACIENTE
            model.paciente = _paciente;
            //EXAMENXATENCION
            model.examen = _examen;
            //INSUMOS

            //ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
            //                    where insu.isactivo == true &&
            //                   insu.isRM == false &&
            //                   insu.isTEM == false &&
            //                   insu.isSedacion == true
            //                    orderby insu.producto
            //                    select insu).ToList();

            ViewBag.tecnicas = getListarInsumos();

            //MOTIVO SEDACION
            ViewBag.motivo_sedacion = new SelectList((new List<Object> { new { codigo = "Claustrofobico" }
                , new { codigo = "Pediatrico" }
            , new { codigo = "Dolor" } 
            , new { codigo = "Movimientos Involuntarios" } 
            , new { codigo = "Transtorno del Sensorio" } 
            , new { codigo = "Ventilacion Mecanica" } 
            , new { codigo = "Otros" } }), "codigo", "codigo");


            //TIPO SEDACION
            ViewBag.tipo_sedacion = new SelectList((new List<Object> { new { codigo = "Inhalatoria" }
                , new { codigo = "Mixta" }
            , new { codigo = "Endovenosa" } }), "codigo", "codigo");

            ViewBag.Error = new SelectList(new Variable().getCancelacionesSedacion(), "codigo", "codigo");

            return View(model);
        }

        public ActionResult AddContraste(int examen)
        {
            var _examen = db.EXAMENXATENCION.Where(x => x.codigo == examen).SingleOrDefault();
            if (_examen != null)
                if (_examen.estadoestudio == "X")
                {
                    CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
                    #region Agregar Tabla Contraste
                    Sedacion enf = new Sedacion();
                    enf.numeroexamen = _examen.codigo;

                    enf.tecnologo = user.ProviderUserKey.ToString();
                    enf.fecha_inicio = DateTime.Now;
                    enf.paciente = _examen.codigopaciente;
                    enf.estado = 0;
                    enf.tipo_sedacion = "";
                    enf.atencion = _examen.numeroatencion;
                    db.Sedacion.Add(enf);
                    db.SaveChanges();
                    #endregion
                    RedirectToAction("RealizarSedacion", "RealizarSedacion", new { sedacion = enf.idsedacion });
                }

            return RedirectToAction("ListaEspera");
        }

        //Ingresar Insumo
        public ActionResult IngresarInsumo(int sedacion, int insumo, double cant, bool isaplicado, string lote, string error)
        {
            bool result = true;
            string msj = "";

            #region Registar Tecnica
            Detalle_Sedacion dc = new Detalle_Sedacion();
            dc.id_sedacion = sedacion;
            dc.Al_InsumId = insumo;
            //dc.id_insumo = insumo;
            dc.cantidad = (decimal)cant;
            dc.isaplicado = !isaplicado;
            dc.lote = lote;
            dc.tipoerroneo = error;
            db.Detalle_Sedacion.Add(dc);

            #endregion
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        //Listar insumos
        //public ActionResult ListarInsumos(int id_sedacion)
        //{
        //    var lista = db.Detalle_Sedacion.Where(x => x.id_sedacion == id_sedacion).ToList();


        //    return PartialView("_ListInsumos", lista);
        //}

        public List<Detalle_Sedacion_Insumo> getDetalleInsumos(int id_sedacion)
        {
            List<Detalle_Sedacion_Insumo> lista = new List<Detalle_Sedacion_Insumo>();

            string sqlPost = @"select d.idDetalle_seda,case when d.id_insumo is null then  (	select i.Nombre from AL_Stock_Item  si inner join AL_Insum i on si.AL_InsumId = i.Al_InsumId  where si.AL_Stock_ItemID=d.Al_InsumId) else (select i.producto from Insumo_Cum_Sunasa i where i.idInsumo=d.id_insumo)  end insumo,d.cantidad,d.lote,d.isaplicado,
isnull(d.tipoerroneo,'') tipoerroneo from Detalle_Sedacion d where d.id_sedacion='{0}'";

            //inner join AL_Insum i on d.id_insumo = i.Al_InsumId inner join IN_UnidadMedida u on i.VolumenUM = u.IN_UnidadMedidaId

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, id_sedacion.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            var item = new Detalle_Sedacion_Insumo();

                            item.iddetalle = Convert.ToInt32(reader["idDetalle_seda"]);
                            item.nom_insumo = (reader["insumo"]).ToString();
                            item.cantidad = Convert.ToDouble(reader["cantidad"].ToString());
                            item.frasco = (reader["lote"]).ToString();
                            item.aplicado = Convert.ToBoolean(reader["isaplicado"].ToString());
                            item.tipoerroneo = (reader["tipoerroneo"]).ToString();
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
            return lista;

        }

        public ActionResult getListaFrascos(int idinsumo, string almacen)
        {

            List<Object> lista = new List<Object>();


            string sqlPost = @"select a.AL_Stock_ItemID,a.Correlativo,a.usado from(
			select si.AL_Stock_ItemID,si.Correlativo,so.FecSalida,si.AL_InsumId,
			 (select isnull(sum(ds.cantidad),0) from Detalle_Sedacion ds where ds.lote = convert(varchar,si.Correlativo)and ds.Al_InsumId = si.AL_Stock_ItemID) usado,(select ii.volumen from AL_Stock_Item sii inner join AL_Insum ii on sii.Al_InsumId = ii.Al_InsumId where sii.AL_Stock_ItemID = si.AL_Stock_ItemID) total
            from AL_Stock_Item si 
            inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId 
            inner join IN_UnidadMedida u on i.VolumenUM = u.IN_UnidadMedidaId
            inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
            inner join IN_UnidadMedida um on i.ConcentracionUM = um.IN_UnidadMedidaId
            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
            inner join Al_Stock_Consumo so on si.Al_Stock_ConsumoID=so.Al_Stock_ConsumoID
            inner join AL_Almacen al on si.AL_AlmacenId = al.AL_AlmacenId
             where e.isVisibleEnfermeria = '1' and
            (so.AL_TipoConsumoId is null or so.AL_TipoConsumoId = 1)
			and si.Al_InsumId = '{0}' and al.CodigoEquivalente ='{1}'
			)as A where 
			
--si esta cerrado
--si esta cerrado
((a.usado=0 ) or 

--si esta abierto y tiene menos de 24 horas
((a.total > a.usado )and (convert(date,(select min(fecha_fin) from Sedacion s inner join Detalle_Sedacion ds on s.idsedacion=ds.id_sedacion where ds.Al_InsumId=a.AL_Stock_ItemID)) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))))

or
--conray no tiene fecha de vencimiento
(((a.total > a.usado ) and a.AL_InsumId = '1031' /*and (convert(date,a.FecSalida) between convert(date,DATEADD(MONTH,-1,GETDATE())) and  convert(date,GETDATE()))*/  ))
/*
or

((a.total > a.usado ) and (convert(date,a.FecSalida) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()) ))*/
order by a.Correlativo
";
//            and (convert(date,(select min(fecha_inicio) from Sedacion s inner join Detalle_Sedacion ds on s.idsedacion=ds.id_sedacion where ds.Al_InsumId=a.AL_Stock_ItemID))

//between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()))))

//or

//(((a.total > a.usado ) and a.AL_InsumId = '1031' and (convert(date,a.FecSalida) between convert(date,DATEADD(MONTH,-1,GETDATE())) and  convert(date,GETDATE()))))


//or

//((a.total > a.usado ) and (convert(date,a.FecSalida) between convert(date,DATEADD(day,-1,GETDATE())) and  convert(date,GETDATE()) ))
//order by a.Correlativo";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, idinsumo.ToString(), almacen), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {

                            lista.Add(new
                            {
                                id = reader["AL_Stock_ItemID"].ToString(),
                                frasco = reader["Correlativo"].ToString()
                            });
                        }
                    }
                    finally
                    {
                        reader.Close();

                        connection.Close();
                    }
                }
            }
            return Json(new SelectList(lista, "id", "frasco"), JsonRequestBehavior.AllowGet);

        }

        //Listar insumos
        public ActionResult ListarInsumos(int idsedacion)
        {
            List<Detalle_Sedacion_Insumo> lista = getDetalleInsumos(idsedacion);
            return PartialView("_ListInsumos", lista);
        }
        public List<Sedacion_Insumo> getListarInsumos()
        {

            List<Sedacion_Insumo> lista = new List<Sedacion_Insumo>();
            //string condicion = "";
            //if (clase == -1)
            //    condicion = " in ('1','2')";
            //else
            //    condicion = " = " + clase.ToString();

//            string sqlPost = @"select i.Al_InsumId, i.Nombre nomInsumo, SC.IN_SubClaseId,sc.Nombre  nomClase, i.Volumen, u.Acronimo
//            from AL_Insum i inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
//            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
//			inner join IN_UnidadMedida u on i.VolumenUM = u.IN_UnidadMedidaId
//            where e.isVisibleEnfermeria = '1' and sc.IN_SubClaseId = '3' ";

            string sqlPost = @"select i.Al_InsumId, i.Nombre nomInsumo, SC.IN_SubClaseId,sc.Nombre  nomClase
            from AL_Insum i inner join AL_Enfermeria e on i.Al_InsumId = e.Al_InsumoId
            inner join IN_SubClase sc on  i.IN_SubClaseId = sc.IN_SubClaseId
            where e.isVisibleEnfermeria = '1' and sc.IN_SubClaseId = '3' ";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(sqlPost, connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {

                        while (reader.Read())
                        {
                            var item = new Sedacion_Insumo();

                            item.idsedacion = Convert.ToInt32(reader["Al_InsumId"]);
                            item.nom_sedacion = (reader["nomInsumo"]).ToString();
                            //item.cantidad = Convert.ToInt32(reader["Volumen"]);
                            //item.unidad_medida = (reader["Acronimo"]).ToString();
                            item.subclase = Convert.ToInt32(reader["IN_SubClaseId"]);
                            item.nomclase = (reader["nomClase"]).ToString();

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
            return lista;
        }


        //Elimiar insumo
        public ActionResult EliminarInsumo(int nco)
        {
            bool result = true;
            string msj = "";
            var dc = (from ci in db.Detalle_Sedacion
                      where ci.idDetalle_seda == nco
                      select ci).Single();

            db.Detalle_Sedacion.Remove(dc);
            db.SaveChanges();
            return Json(new { result = result, mensaje = msj }, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Delete(int id)
        {
            var _sedacion = db.Sedacion.SingleOrDefault(x => x.idsedacion == id);
            _sedacion.estado = 9;
            db.SaveChanges();
            return RedirectToAction("ListaEspera");
        }

        //
        // POST: /ContrasteExamenViewModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarSedacion(SedacionExamenViewModel item)
        {
            CustomMembershipUser user = Membership.GetUser(true) as CustomMembershipUser;
            var id = item.idsedacion;
            var sed = db.Sedacion.Where(x => x.idsedacion == id).SingleOrDefault();
            if (sed != null)
            {
                var _examen = (from x in db.EXAMENXATENCION where x.codigo == sed.numeroexamen select x).SingleOrDefault();
                var _paciente = (from x in db.PACIENTE where x.codigopaciente == _examen.codigopaciente select x).SingleOrDefault();
                var _encuesta = (from x in db.Encuesta where x.numeroexamen == _examen.codigo select x).SingleOrDefault();
                //Contraste
                item.sedacion = sed;
                item.idsedacion = sed.idsedacion;
                item.insumos = getDetalleInsumos(sed.idsedacion);//db.Detalle_Sedacion.Where(x => x.id_sedacion == sed.idsedacion).ToList();

                //PACIENTE
                item.paciente = _paciente;
                //EXAMENXATENCION
                item.examen = _examen;
                //INSUMOS
                //ViewBag.tecnicas = (from insu in db.Insumo_Cum_Sunasa
                //                    where insu.isactivo == true &&
                //                   insu.isRM == false &&
                //                   insu.isTEM == false &&
                //                   insu.isSedacion == true
                //                    orderby insu.producto
                //                    select insu).ToList();

                ViewBag.tecnicas = getListarInsumos();

                //MOTIVO SEDACION
                ViewBag.motivo_sedacion = new SelectList((new List<Object> { new { codigo = "Claustrofobico" }
                , new { codigo = "Pediatrico" }
            , new { codigo = "Dolor" } 
            , new { codigo = "Movimientos Involuntarios" } 
            , new { codigo = "Transtorno del Sensorio" } 
            , new { codigo = "Ventilacion Mecanica" } 
            , new { codigo = "Otros" } }), "codigo", "codigo");


                //TIPO SEDACION
                ViewBag.tipo_sedacion = new SelectList((new List<Object> { new { codigo = "Inhalatoria" }
                , new { codigo = "Mixta" }
            , new { codigo = "Endovenosa" } }), "codigo", "codigo");

                ViewBag.Error = new Variable().getCancelacionesSedacion();

                sed.consentimiento = item.consentimiento;
                sed.tipo_sedacion = item.tiposedacion;
                sed.motivo_sedacion = item.motivosedacion;
                sed.motivo_otros = item.motivo_otros;
                sed.estado = 1;
                //sed.fecha_inicio = item.fec_ini;
                //sed.fecha_impresion = DateTime.Now;
                sed.fecha_fin = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ListaEspera");


            }

            return View(item);
        }

        public ActionResult getStockSedacion(int frasco, int stock)
        {

            string sqlPost = @"select 
            (select isnull(sum(ds.cantidad),0) from Detalle_Sedacion ds where ds.lote = '{0}' and ds.Al_InsumId = '{1}') usado,
            (select i.volumen from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId where si.AL_Stock_ItemID = '{1}') total, (select u.Acronimo  from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join IN_UnidadMedida u on i.VolumenUM  = u.IN_UnidadMedidaId  where si.AL_Stock_ItemID = '{1}') unidadMedida";
            double usado = 0;
            double total = 0;
            string medida = "";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, frasco.ToString(), stock.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            usado = Convert.ToDouble(reader["usado"].ToString());
                            total = Convert.ToDouble(reader["total"].ToString());
                            medida = reader["unidadMedida"].ToString();
                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }



                }
            }
            return Json("Stock Disponible: " + (total - usado).ToString() + " " + medida, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getValidacionSedacion(int frasco, int stock, double cantidad)
        {

            string sqlPost = @"select 
            (select isnull(sum(cantidad),0) from Detalle_Contraste dc where lote = '{0}' and dc.Al_InsumId = '{1}') usado,
            (select i.volumen from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId where si.AL_Stock_ItemID = '{1}') total, (select u.Acronimo  from AL_Stock_Item si inner join AL_Insum i on si.Al_InsumId = i.Al_InsumId inner join IN_UnidadMedida u on i.VolumenUM  = u.IN_UnidadMedidaId  where si.AL_Stock_ItemID = '{1}') unidadMedida";
            double usado = 0;
            double total = 0;
            bool result = false;
            double faltante = 0;
            string medida = "";

            using (DATABASEGENERALEntities db1 = new DATABASEGENERALEntities())
            {
                using (SqlConnection connection = new SqlConnection(db1.Database.Connection.ConnectionString))
                {
                    SqlCommand command_contraste = new SqlCommand(string.Format(sqlPost, frasco.ToString(), stock.ToString()), connection);
                    connection.Open();
                    SqlDataReader reader = command_contraste.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            usado = Convert.ToDouble(reader["usado"].ToString());
                            total = Convert.ToDouble(reader["total"].ToString());
                            medida = reader["unidadMedida"].ToString();
                        }

                    }
                    finally
                    {
                        reader.Close();
                        connection.Close();
                    }

                    if (usado < total)
                    {
                        if (usado + cantidad <= total)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            faltante = total - usado;
                        }

                    }
                    else
                    {
                        result = false;
                        faltante = total - usado;
                    }

                }
            }
            return Json(new { result, faltante, medida }, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}